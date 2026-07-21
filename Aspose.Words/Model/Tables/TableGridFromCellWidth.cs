// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/12/2006 by Vladimir Averkin

using System;
using Aspose.Collections;
using Aspose.Collections.Generic;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// This is the class to use if you need to calculate a "table grid".
    ///
    /// The table grid is a definition of the set of grid columns which define all of the shared vertical
    /// edges of the table, as well as default widths for each of these grid columns.
    /// </summary>
    /// <remarks>
    /// dmatv: This class calculates cell grid spans from CellAttr.Width.
    /// Though it is still used, the results may not match MS Word when cell width data are missing or not correct for some reason.
    /// So for formats that do not store cell width explicitly in the document (such as docx), grid calculation is done by different (new) logic when possible.
    /// I renamed the class from TableGrid to TableGridFromCellWidth so that this class does not look like the main table grid entity.
    ///
    /// For binary doc and rtf formats, this class is used both by the new table layout logic as well.
    /// Unlike docx/wml, these formats actually store cell widths inside the document.
    /// They do not store a tblGrid equivalent, so it must be recreated from cell widths for doc/rtf.
    /// </remarks>
    internal class TableGridFromCellWidth
    {
        /// <summary>
        /// Create an instance of this class when you visit a table.
        /// </summary>
        internal TableGridFromCellWidth(Table table) : this(table, false) { }

        /// <summary>
        /// Creates table grid object for either original or revised table attributes.
        /// </summary>
        internal TableGridFromCellWidth(Table table, bool isRevision, bool useSystemAttrs = false)
        {
            mRevisionsView = isRevision ? RevisionsView.Final : RevisionsView.Original;
            // Set strategy for getting width before row.
            WidthBeforeMethod = GetWidthBeforeMethod(table, useSystemAttrs);

            CalcColumnWidthsAndRowIndents(table);
        }

        /// <summary>
        /// Creates an instance of this class for handling doc/rtf input.
        /// </summary>
        /// <remarks>
        /// The method is used by the "new" table grid logic.
        /// </remarks>
        internal static TableGridFromCellWidth GetDocRtfGridFromAttrs(Table table)
        {
            bool useFinalRevision = true;
            bool useSystemAttrs = true;
            return new TableGridFromCellWidth(table, useFinalRevision, useSystemAttrs);
        }

        /// <summary>
        /// Calculate column widths array for the specified table.
        /// </summary>
        private void CalcColumnWidthsAndRowIndents(Table table)
        {
            SortedIntegerListGeneric<object> columnPositions = new SortedIntegerListGeneric<object>();

            // Calculate column positions and width before rows.
            for (Row row = table.FirstRow; row != null; row = row.NextRow)
            {
                // Add all column positions of all rows to the sorted array as keys.
                // I don't use the values in this sorted array (only use keys), so insert nulls.
                // In the end we get a sorted positions array, which can then be used to calculate widths.

                int position = GetWidthBeforeRow(row);
                columnPositions[position] = null;

                foreach (Cell cell in row.Cells)
                {
                    // Use CellPr used by MS Word to render cell to calculate grid.
                    int cellWidth = (int)cell.CellPr.FetchAttr(CellAttr.Width, mRevisionsView);
                    position += cellWidth;
                    columnPositions[position] = null;
                }
            }

            int[] columnWidths = new int[System.Math.Max(columnPositions.Count - 1, 0)];
            int leftmostRowPosition = columnPositions.GetKey(0);
            int columnLeft = leftmostRowPosition;
            for (int columnIndex = 0; columnIndex < columnWidths.Length; columnIndex++)
            {
                int columnRight = columnPositions.GetKey(columnIndex + 1);
                columnWidths[columnIndex] = columnRight - columnLeft;
                // Next column.
                columnLeft = columnRight;
            }

            // Save the computed properties.
            mColumnWidths = columnWidths;
            LeftmostRowPosition = leftmostRowPosition;
            // TestJira15558 demonstrates a case when the leftmost row position is negative.
            // It should not happen when widthBefor is specified explicitly,
            // but when computed from sprmTDxaLeft and sprmTDxaGapHalf it is apparently possible.
            // Remember the value as it is needed to compute the actual width before rows.
        }

        /// <summary>
        /// Determines which way of computing 'width before' rows to use.
        /// </summary>
        private WidthBeforeType GetWidthBeforeMethod(Table table, bool useSystemAttrs)
        {
            WidthBeforeType method;
            if (useSystemAttrs)
            {
                // There are three different ways do get widthBefore for doc/rtf.
                bool useWord97Logic = (bool)table.FirstRow.TablePr.FetchAttr(TableAttr.Sys_Word97Logic);
                if (useWord97Logic)
                {
                    // Use the value computed by readers from older Word97 attributes.
                    method = WidthBeforeType.FromWord97Attributes;
                }
                else if (IsWidthBeforeApplicable(table))
                {
                    // TableAttr.WidthBefore is present, use it.
                    method = WidthBeforeType.AttrDirect;
                    // The new algorithm does not handle revisions yet.
                }
                else
                {
                    // Compute from doc/rtf specific row attributes.
                    method = WidthBeforeType.FromDxaDocRtfAttributes;
                }
            }
            else
            {
                // Read from TableAttr.WidthBefore, use revisions view.
                method = WidthBeforeType.AttrRevisionAware;
            }
            return method;
        }

        /// <summary>
        /// Determines if taking 'width before' value from TableAttr.WidthBefore is applicable to the table.
        /// </summary>
        /// <remarks>
        /// For doc/rtf in absence of explicitly specified width before values, a method that uses
        /// different attributes is applied.
        /// </remarks>
        private static bool IsWidthBeforeApplicable(Table table)
        {
            // Relying on doc/rtf specific attributes makes no sense for other formats.
            // Always rely on widthBefore for formats other than doc/rtf.
            if (!DocRtfGridHandler.IsGridStoredInCellWidths(table))
                return true;

            // Check if width before is (was) actually present for a row at the time of reading.
            // Discarded width before values specified in percent units count as well (Test26203*).
            foreach (Row row in table.Rows)
            {
                PreferredWidth widthBefore = (PreferredWidth)row.TablePr.GetDirectAttr(TableAttr.WidthBefore);
                if ((widthBefore != null) && !widthBefore.IsPercent)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets 'width before' value for a row using a method determined earlier for the whole table.
        /// </summary>
        private int GetWidthBeforeRow(Row row)
        {
            int widthBefore;
            switch (WidthBeforeMethod)
            {
                case WidthBeforeType.AttrRevisionAware:
                    widthBefore = GetWidthBeforeFromAttributes(row.TablePr);
                    break;
                case WidthBeforeType.FromWord97Attributes:
                    // Tests like TestJira6544 and TestDefect18346 use values computed by the reader from Word97-specific attributes.
                    int baseIndent = (int)row.ParentTable.FirstRow.TablePr.FetchAttr(TableAttr.Sys_LeftIndent97Base);
                    // Test18512: for RTL tables, the Sys_LeftIndent97 specifies width after the row.
                    bool isRtl = row.ParentTable.FirstRow.TablePr.Contains(TableAttr.Sys_BidiTable97);
                    int indent = isRtl
                        ? 0
                        : (int)row.TablePr.FetchAttr(TableAttr.Sys_LeftIndent97);
                    widthBefore = indent - baseIndent;

                    // TODO consider moving parallel code from doc and rtf readers it this class or maybe DocRtfGridHandler.
                    // Otherwise it is the same attribute as below.
                    break;
                case WidthBeforeType.AttrDirect:
                    // Tests like TestCeller1 seem to use the value from the special attribute.
                    // SprmCode.TWidthBefore for doc format; equivalent in trwWidthB and trftsWidthB for rtf.
                    // Currently the readers are somewhat different in handling the value type.
                    // Values in twips should be reliable.
                    // If specified, they should be used in favor of explicit cell positions handled below.
                    widthBefore = GetWidthBeforeFromAttributes(row.TablePr);
                    // However TestJira7090 shows a complex table with inconsistent SprmCode.TWidthBefore.
                    // The values are different from the values saved by MS Word to docx,
                    // and the values for rows 1 and 2 are different even though MS Word saves the same gridBefore 2 to docx.
                    // Perhaps it can be explained by removing an intersection column spanned by width before,
                    // but I was not able to reconstruct MS Word computation for 7090 precisely.
                    // I think it should be addressed when WidthTolerance is set to 0,
                    // which is not possible at the moment as it breaks some tests because of another issue.
                    // See comments in WidthTolerance.
                    break;
                case WidthBeforeType.FromDxaDocRtfAttributes:
                    // Use the value computed from the attributes specifying row position explicitly.
                    // sprmTDxaLeft and sprmTDxaGapHalf for binary doc.
                    // It appears that there is no rtf equivalent (97-mode attributes are used).
                    widthBefore = GetWidthBeforeFromDxaDocRtfAttributes(row.TablePr);
                    // The logic that falls back here in absence of SprmCode.TWidthBefore looks kind of strange,
                    // but actually in the DOC specs TWidthBefore value may indicate that TWidthBefore width value should be ignored.
                    // So in case that TWidthBefore is ignored (or absent),
                    // it is logical that space before row in a jagged table is specified elsewhere.
                    break;
                default:
                    throw new InvalidOperationException("Unknown widthBefore getting method.");
            }
            return widthBefore;
        }

        /// <summary>
        /// Gets widthBefore in twips from <see cref="TableAttr.Sys_DxaLeft"/> and <see cref="TableAttr.Sys_DxaGapHalf"/>
        /// </summary>
        private static int GetWidthBeforeFromDxaDocRtfAttributes(TablePr tablePr)
        {
            int dxaGapHalf = (int)tablePr.FetchAttr(TableAttr.Sys_DxaGapHalf);
            int dxaLeft = (int)tablePr.FetchAttr(TableAttr.Sys_DxaLeft);
            return dxaLeft - dxaGapHalf;
        }

        /// <summary>
        /// Gets widthBefore in twips from <see cref="TableAttr.WidthBefore"/> and for the value in percent units
        /// falls back to <see cref="GetWidthBeforeFromDxaDocRtfAttributes(TablePr)"/>.
        /// </summary>
        private int GetWidthBeforeFromAttributes(TablePr tablePr)
        {
            PreferredWidth attrValue = (PreferredWidth)tablePr.FetchAttr(TableAttr.WidthBefore, mRevisionsView);
            // Fall back to dxa attributes for width before in percent units.
            int widthBefore = attrValue.IsPercent
                ? GetWidthBeforeFromDxaDocRtfAttributes(tablePr)
                : attrValue.ValueTwipsSafe;
            return widthBefore;
        }

        /// <summary>
        /// Call this when you visit a row. Updates <see cref="Before"/>, <see cref="GridBefore"/>,
        /// <see cref="After"/> and <see cref="GridAfter"/> with values for the current row.
        /// </summary>
        internal void NextRow(Row row)
        {
            int columnIndex = 0;
            mCellIndex = 0;

            // Calculate before and grid before.
            int widthBefore = GetWidthBeforeRow(row);
            // Do not remove gridBefore common for all rows for the "old" grid algorithm to preserve the current behavior.
            // Test21928NotApplied().
            if (WidthBeforeMethod != WidthBeforeType.AttrRevisionAware)
                widthBefore -= LeftmostRowPosition;

            int indent = 0;
            while ((indent < widthBefore) && (columnIndex < mColumnWidths.Length))
            {
                indent += mColumnWidths[columnIndex];
                columnIndex++;
            }

            mBefore = widthBefore;
            mGridBefore = columnIndex;

            // Calculate grid spans.
            CellCollection cells = row.Cells;
            mGridSpans = new int[cells.Count];

            for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
            {
                int cellWidth = (int)cells[cellIndex].CellPr.FetchAttr(CellAttr.Width, mRevisionsView);
                int gridSpan = GetGridSpan(columnIndex, cellWidth);

                mGridSpans[cellIndex] = gridSpan;
                columnIndex += gridSpan;
            }

            // Calculate after and grid after.
            PreferredWidth widthAfter = (PreferredWidth)row.TablePr.FetchAttr(TableAttr.WidthAfter, mRevisionsView);
            mAfter = widthAfter.ValueTwipsSafe;
            mGridAfter = mColumnWidths.Length - columnIndex;

            if (mGridAfter == 0)
            {
                // There are no columns after the row in the constructed grid, so there is no width after the row,
                // even if it is specified in the row properties.
                mAfter = 0;
            }
        }

        private int GetGridSpan(int columnIndex, int cellWidth)
        {
            int gridSpan = 0;
            int remainingWidth = cellWidth;

            while ((remainingWidth > WidthTolerance) && (columnIndex < mColumnWidths.Length))
            {
                remainingWidth -= mColumnWidths[columnIndex];
                columnIndex++;
                gridSpan++;
            }

            return gridSpan;
        }

        /// <summary>
        /// Widths of columns in the table grid.
        /// </summary>
        internal int[] ColumnWidths
        {
            get { return mColumnWidths; }
        }

        /// <summary>
        /// Call this when you visit a cell. Returns the grid span for the next cell in the current row.
        /// </summary>
        internal int NextCell()
        {
            return mGridSpans[mCellIndex++];
        }

        /// <summary>
        /// Indicates if the current cell is the last of the grid spans constructed for the current row.
        /// </summary>
        internal bool IsLastCell()
        {
            return (mGridSpans != null) && (mCellIndex >= mGridSpans.Length);
        }

        /// <summary>
        /// Count of grid columns before the first cell in the current row.
        /// </summary>
        internal int GridBefore
        {
            get { return mGridBefore; }
        }

        /// <summary>
        /// Width before the first cell in the current row.
        /// </summary>
        internal int Before
        {
            get { return mBefore; }
        }

        /// <summary>
        /// Count of grid columns after the last cell in the current row.
        /// </summary>
        internal int GridAfter
        {
            get { return mGridAfter; }
        }

        /// <summary>
        /// Width after the last cell in the current row.
        /// </summary>
        internal int After
        {
            get { return mAfter; }
        }

        /// <summary>
        /// Creates a new IntList instance and populates it with the values from ColumnWidths.
        /// </summary>
        internal IntList ToIntList()
        {
            IntList widthList = new IntList(ColumnWidths.Length);
            foreach (int column in ColumnWidths)
                widthList.Add(column);

            return widthList;
        }

        // *** These fields are calculated when the grid is created and remain unchanged until the grid is discarded.
        private int[] mColumnWidths;

        // *** This field is used when processing a single cell.
        /// <summary>
        /// Current cell index.
        /// </summary>
        private int mCellIndex;

        // *** These fields are used when processing one row.
        /// <summary>
        /// Current row grid spans. One item per cell node.
        /// </summary>
        private int[] mGridSpans;
        private int mGridBefore;
        private int mBefore;
        private int mGridAfter;
        private int mAfter;

        /// <summary>
        /// Controls should table grid builder get original (before-changes) or final (after-changes) properties.
        /// </summary>
        private readonly RevisionsView mRevisionsView;

        internal static int WidthTolerance
        {
            get
            {
                return 1;
                // This should be 0 for the new algorithm:
                // return UseSystemAttrs ? 0 : 1.
                // Using 0 tolerance fixes some tests with "zero grid span" condition, like Test16430.
                // In those tests, cells with width 1 computed from cell attributes get grid span 0 in the model,
                // because width 1 is within tolerance.
                // Because of that, the new logic is not applied as grid span 0 is nonsense for a cell.
                // Using 0 tolerance fixes those tests.
                // However, it breaks some other tests like Test8319.
                // In 8319, with tolerance 1, table becomes jagged with a 1-twip width after some rows.
                // As cell widths are in pct units, the new logic becomes not applicable to a jagged table with pct cells.
                // AW falls back to the old logic that never handled the table in 8319 correctly.

                // So setting tolerance 0 here depends on supporting jagged tables with width before/after with pct units.
                // It depends on an unresolved WORDSNET-23404 in the model
                // (neither model no readers/writers support pct units for those attributes).
            }
        }

        /// <summary>
        /// Gets or sets leftmost row position.
        /// </summary>
        /// <remarks>
        /// It may happen that all rows in the table are shifted right,
        /// according to the attributes used to construct the grid.
        /// Space before the leftmost row is not needed in the grid, so this value is used to exclude it.
        /// </remarks>
        private int LeftmostRowPosition { get; set; }

        /// <summary>
        /// Represents the method of getting width before from row attributes.
        /// </summary>
        private enum WidthBeforeType
        {
            /// <summary>
            /// This is the original methods used by the old algorithm
            /// that uses TableAttr.WidthBefore taken from an appropriate revisions view.
            /// </summary>
            AttrRevisionAware,

            // The methods below are used by the new algorithm.
            // Revisions are not handled yet.

            /// <summary>
            /// Use the value computed by doc/rtf readers from older Word97 indent attributes.
            /// </summary>
            FromWord97Attributes,

            /// <summary>
            /// Use the value currently stored in <see cref="TableAttr.WidthBefore"/>.
            /// </summary>
            AttrDirect,

            /// <summary>
            /// Compute the value from row attributes specific to doc/rtf.
            /// </summary>
            FromDxaDocRtfAttributes,
        }

        /// <summary>
        /// Stores the method of getting width before value from row attributes.
        /// </summary>
        private WidthBeforeType WidthBeforeMethod { get; }
    }
}
