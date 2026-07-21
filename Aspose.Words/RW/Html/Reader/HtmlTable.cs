// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents an HTML table during import.
    /// </summary>
    internal class HtmlTable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="cssStyleTracker">CSS style tracker.</param>
        /// <param name="isXhtml">
        /// Indicates whether the input document is XHTML and has been parsed according to XHTML rules.
        /// </param>
        private HtmlTable(
            CssStyleTracker cssStyleTracker,
            bool isXhtml)
        {
            Debug.Assert(cssStyleTracker != null);

            mCssStyleTracker = cssStyleTracker;
            mRows = new List<HtmlRow>();
            mCaptions = new List<HtmlTableCaption>();
            mHtmlCols = new List<HtmlCol>();

            CssDeclarationCollection tableDeclarations = mCssStyleTracker.ElementDeclarations;
            CssBorders = CssBoxBorders.CreateBorders(tableDeclarations, false);

            CssDeclaration borderCollapseDeclaration = tableDeclarations["border-collapse"];
            IsBorderCollapsed = (borderCollapseDeclaration != null) &&
                                 borderCollapseDeclaration.Value.Equals(CssValue.Collapse);

            mIsXhtml = isXhtml;
        }

        /// <summary>
        /// Fills and returns an HtmlTable object from an HtmlNode that represents a table.
        /// </summary>
        /// <param name="tableNode">Should be a table element.</param>
        /// <param name="cssStyleTracker">CSS style tracker.</param>
        /// <param name="isXhtml">Indicates whether the input document is XHTML.</param>
        internal static HtmlTable Create(
            HtmlElementNode tableNode,
            CssStyleTracker cssStyleTracker,
            bool isXhtml)
        {
            Debug.Assert(tableNode != null);
            Debug.Assert(cssStyleTracker != null);
            Debug.Assert(tableNode == cssStyleTracker.CurrentElement);

            // Need to check that the node name is 'table' because this tag does not depend on "display" style
            // and is always processed as table.
            Debug.Assert((tableNode.Name == "table") || (cssStyleTracker.ElementDisplayType == CssDisplayType.Table) ||
                         (cssStyleTracker.ElementDisplayType == CssDisplayType.InlineTable));

            HtmlTable htmlTable = new HtmlTable(cssStyleTracker, isXhtml);
            htmlTable.Fill(tableNode);
            return htmlTable;
        }

        /// <summary>
        /// Gets <see cref="HtmlTableCaption"/> object specified by index.
        /// </summary>
        internal HtmlTableCaption GetCaption(int index)
        {
            return mCaptions[index];
        }

        /// <summary>
        /// Gets <see cref="HtmlCol"/> object by the specified column index.
        /// </summary>
        /// <param name="colIndex">Column index.</param>
        internal HtmlCol GetCol(int colIndex)
        {
            return mHtmlCols[colIndex];
        }

        /// <summary>
        /// Fills the HtmlTable object from an HtmlNode that represents a table.
        /// </summary>
        /// <param name="tableNode">TABLE element or element with 'table' or 'inline-table' css 'display' type.</param>
        private void Fill(HtmlElementNode tableNode)
        {
            IsDisplayTable = tableNode.Name != "table";

            ProcessNode(tableNode);

            VerticalMerge();

            // WORDSNET-20250 Remove extra empty cells that are added because of "colspan" attributes.
            RemoveExtraEmptyCells();

            MakeRowsSameLength();

            ColumnCount = GetMaxColumnCount();
        }

        private void ProcessNode(HtmlElementNode node)
        {
            // Counters should be updated only once per element and they will be updated when this element will be processed
            // by the HTML table reader.
            mCssStyleTracker.PushElement(node, false);

            if (mCssStyleTracker.ElementDisplayState == HtmlElementDisplayState.Visible)
            {
                if (HandleNodeStart(node))
                {
                    foreach (HtmlNode child in node.Children)
                    {
                        if (child is HtmlElementNode)
                        {
                            ProcessNode((HtmlElementNode)child);
                        }
                    }
                    HandleNodeEnd(node);
                }
            }

            mCssStyleTracker.PopElement();
        }

        /// <summary>
        /// Called for every node in the table when the node processing is started.
        /// </summary>
        /// <returns>Returns false if don't need to process children of this node and don't need to process end of this node.</returns>
        private bool HandleNodeStart(HtmlElementNode node)
        {
            // WORDSNET-21365 TR and TD elements should be processed before a check for 'display: table' CSS
            // property.
            if ((mIsCssTable && (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableRow)) ||
                (node.Name == "tr"))
            {
                // In case the table consists of non-table tags (display: table-cell, table-row, etc ),
                // sometimes it is required to create missing elements if they are not explicitly defined.
                if (node.Name != "tr")
                    HandleGroupNodeStart(node);

                return HandleRowNodeStart(node);
            }

            if ((mIsCssTable && (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableCell)) ||
                (node.Name == "td") || (node.Name == "th"))
            {
                // In case the table consists of non-table tags (display: table-cell, table-row, etc ),
                // sometimes it is required to create missing elements if they are not explicitly defined.
                if (mIsCssTable && (mRows.Count == 0) && ((node.Name != "td") || (node.Name != "th")))
                {
                    HandleGroupNodeStart(node);
                    HandleRowNodeStart(node);
                }

                HandleCellNodeStart(node);
                return true;
            }

            if ((mCssStyleTracker.ElementDisplayType == CssDisplayType.Table) ||
                (mCssStyleTracker.ElementDisplayType == CssDisplayType.InlineTable) ||
                (node.Name == "table"))
            {
                if (mIsInTable)
                    return false; // Do not process nested table here. HtmlTable represents one table.
                mIsInTable = true;
                mIsCssTable = node.Name != "table";
                return true;
            }

            if ((mIsCssTable && (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableCaption)) ||
                (node.Name == "caption"))
            {
                mCaptions.Add(new HtmlTableCaption(node, mCssStyleTracker.ElementDeclarations));
                // Do not search for table elements inside captions.
                return false;
            }

            if ((mIsCssTable &&
                 ((mCssStyleTracker.ElementDisplayType == CssDisplayType.TableHeaderGroup) ||
                  (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableFooterGroup) ||
                  (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableRowGroup))) ||
                (node.Name == "thead") || (node.Name == "tbody") || (node.Name == "tfoot"))
            {
                HandleGroupNodeStart(node);
                return true;
            }

            if ((mIsCssTable && (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableColumnGroup)) ||
                (node.Name == "colgroup"))
            {
                HandleColumnGroupNodeStart();
                return true;
            }

            if ((mIsCssTable && (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableColumn)) ||
                (node.Name == "col"))
            {
                if (mCurrentHtmlColGroup == null)
                    mCurrentHtmlColGroup = new HtmlColGroup();

                // WORDSNET-10157 If 'col' element has 'span' attribute, then 'col' attributes should be applied a 'span'
                // number times.
                // WORDSNET-19917 The "span" value of a "col" attribute must be greater than 0 and less
                // than or equal to 1000, and its default value is 1.
                // See https://html.spec.whatwg.org/multipage/tables.html#attr-col-span
                int span = node.Attributes.GetAttributeValue("span", 1, 1000);
                for (int i = 0; i < span; i++)
                    mHtmlCols.Add(new HtmlCol(mCssStyleTracker.ElementDeclarations, mCurrentHtmlColGroup));

                // Do not search for table elements inside col.
                return false;
            }

            return false;
        }

        private void HandleColumnGroupNodeStart()
        {
            mCurrentHtmlColGroup = new HtmlColGroup(mCssStyleTracker.ElementDeclarations);
        }

        private void HandleGroupNodeStart(HtmlElementNode node)
        {
            mCurrentHtmlRowGroup = new HtmlRowGroup(node, mCssStyleTracker.ElementDeclarations);
        }

        private void HandleCellNodeStart(HtmlElementNode node)
        {
            // WORDSNET-8117 In order to provide export/import roundtrip
            // here we should ignore unnecessary cells previously added on HTML import.
            string style = node.Attributes.GetAttributeValue("style", string.Empty);
            Regex styleRegex = new Regex(@"width:0pt; height:\d+(\.\d+)?pt; border:none");
            if ((node.GetInnerText() == string.Empty) && styleRegex.IsMatch(style))
                return;

            // If have no rows but got a TD, it is invalid table, ignore TD.
            if (mRows.Count == 0)
                return;

            // WORDSNET-19917 The "colspan" value of a "td" element must be greater than 0 and less
            // than or equal to 1000.
            // See https://html.spec.whatwg.org/multipage/tables.html#attr-tdth-colspan
            int colSpan = node.Attributes.GetAttributeValue("colspan", 1, 1000);

            // WORDSNET-19917 The "rowspan" value of a "td" element must be greater than 0 and less
            // than or equal to 65534.
            // See https://html.spec.whatwg.org/multipage/tables.html#attr-tdth-rowspan
            int rowSpan = node.Attributes.GetAttributeValue("rowspan", 1, 65534);

            HtmlCell firstCell = new HtmlCell(node, CssBoxBorders.CreateBorders(mCssStyleTracker.ElementDeclarations, true));
            firstCell.ColSpan = colSpan;
            firstCell.UnrestrictedColSpan = colSpan;
            firstCell.RowSpan = rowSpan;
            CurRow.Add(firstCell);

            if (colSpan > 1)
            {
                //Process horizontal merge by adding more cells to the row.
                firstCell.HorizontalMerge = CellMerge.First;

                for (int remainingColSpan = colSpan - 1; remainingColSpan > 0; --remainingColSpan)
                {
                    HtmlCell cell = new HtmlCell(firstCell.CssBorders);
                    cell.HorizontalMerge = CellMerge.Previous;
                    cell.HorizontalMergeFirstCell = firstCell;

                    // Store 'rowspan' value in generated empty cells in case the original cell has both
                    // 'colspan' and 'rowspan' attributes specified. 'rowspan' will be processed later, after
                    // all rows are loaded.
                    cell.ColSpan = remainingColSpan;
                    cell.UnrestrictedColSpan = remainingColSpan;
                    cell.RowSpan = rowSpan;
                    CurRow.Add(cell);
                }
            }

            // WORDSNET-9219 Consider only cells which span one row.
            if (rowSpan == 1)
            {
                CssDeclaration heightDeclaration = mCssStyleTracker.ElementDeclarations["height"];
                if (heightDeclaration != null)
                {
                    double cellHeight = CssUtil.LengthToPoint(heightDeclaration.Value);
                    if (!MathUtil.IsMinValue(cellHeight) && (cellHeight > CurRow.MaxCellHeightInRow))
                        CurRow.MaxCellHeightInRow = cellHeight;
                }
            }
        }

        private bool HandleRowNodeStart(HtmlElementNode node)
        {
            // WORDSNET-968 Remove the zero height extra row appended by AW to the HTML table during export.
            // The last table row with empty cells can be used to define column widths. It would be correct not to delete it
            // but sometimes it's enclosed with <!--[if !supportMisalignedColumns]--> - <!--[endif]--> brackets.
            // MS Word skips such row but we don't support this syntax and so it's better to just remove empty row.
            if (IsColumnAlignmentRow(node, mCssStyleTracker.ElementDeclarations))
            {
                return false;
            }

            // WORDSNET-8838 If a table has no row group elements, the HTML 5 parser creates a TBODY element enclosing all
            // table rows. The XHTML parser, however, returns such tables without any correction. As a result, tables in XHTML
            // documents may have no row group elements.
            // In order to work around that issue, here we create a new fake TBODY element without any style declarations.
            HtmlRowGroup rowGroup = mCurrentHtmlRowGroup;
            if ((rowGroup == null) && mIsXhtml)
            {
                rowGroup = new HtmlRowGroup(new HtmlElementNode("tbody", false), new CssDeclarationCollection());
            }

            mRows.Add(new HtmlRow(node, mCssStyleTracker.ElementDeclarations, rowGroup));

            return true;
        }

        /// <summary>
        /// Called for every node in the table when the node processing is finished.
        /// </summary>
        private void HandleNodeEnd(HtmlElementNode node)
        {
            if ((mIsCssTable &&
                    ((mCssStyleTracker.ElementDisplayType == CssDisplayType.TableHeaderGroup) ||
                     (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableFooterGroup) ||
                     (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableRowGroup))) ||
                (node.Name == "thead") || (node.Name == "tbody") || (node.Name == "tfoot"))
            {
                mCurrentHtmlRowGroup = null;
                return;
            }

            if ((mIsCssTable && (mCssStyleTracker.ElementDisplayType == CssDisplayType.TableColumnGroup)) ||
                (node.Name == "colgroup"))
            {
                Debug.Assert(mCurrentHtmlColGroup != null);

                // "Colgroup" attributes are applied if the column group doesn't contain "col" elements.
                if ((mHtmlCols.Count == 0) || (mHtmlCols[mHtmlCols.Count - 1].ColGroup != mCurrentHtmlColGroup))
                {
                    // A <colgroup> element can cover multiple columns if the "span" attribute is specified.
                    // WORDSNET-19917 The "span" value of a "colgroup" attribute must be greater than 0 and less
                    // than or equal to 1000, and its default value is 1.
                    // See https://html.spec.whatwg.org/multipage/tables.html#attr-colgroup-span
                    int span = node.Attributes.GetAttributeValue("span", 1, 1000);
                    for (int i = 0; i < span; i++)
                    {
                        mHtmlCols.Add(new HtmlCol(mCurrentHtmlColGroup.Declarations, mCurrentHtmlColGroup));
                    }
                }

                mCurrentHtmlColGroup = null;
                return;
            }
        }

        /// <summary>
        /// Inserts a cell to the specified row. Shifts the remaining cells in such a way that vertically merged cells
        /// do not change their positions. This is necessary to properly build a table when colspan and rowspan
        /// are used together.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="cellIndex"></param>
        /// <param name="cell"></param>
        private void InsertCell(int rowIndex, int cellIndex, HtmlCell cell)
        {
            HtmlRow row = this[rowIndex];

            // Add a placeholder cell.
            row.Add(null);

            int dstIndex = row.CellCount - 1;
            int srcIndex = dstIndex - 1;

            while (srcIndex >= cellIndex)
            {
                // Decrement the source index until it points to a non-merged cell.
                while (row[srcIndex].VerticalMerge != CellMerge.None)
                {
                    srcIndex--;

                    // WORDSNET-20270 If "rowspan" of a HTML cell is greater than the number of available rows,
                    // don't try to insert a cell to a non-existent row.
                    if (srcIndex < 0)
                    {
                        // Remove the cell that we've added at the beginning of this method.
                        row.RemoveLastCell();
                        return;
                    }
                }

                // Shift the cell.
                row[dstIndex] = row[srcIndex];

                dstIndex = srcIndex;
                srcIndex = dstIndex - 1;
            }

            // Insert the target cell.
            row[cellIndex] = cell;
        }

        private void AddCellsToRow(int rowIndex, int cellCount, CssBoxBorders cellBorders)
        {
            for (int i = 0; i < cellCount; i++)
                this[rowIndex].Add(new HtmlCell(cellBorders));
        }

        private int GetMaxColumnCount()
        {
            int result = 0;
            foreach (HtmlRow row in mRows)
            {
                result = System.Math.Max(result, row.CellCount);
            }
            return result;
        }

        /// <summary>
        /// Adds empty cells to rows that are shorter than the maximum column count.
        /// Some rows could be shorter due to merged cells or just could be because it is so in the HTML.
        /// Our algorithms require all rows to have same number of cells to work properly.
        ///
        /// Note neither Word nor IE require that.
        /// </summary>
        private void MakeRowsSameLength()
        {
            int columnCount = GetMaxColumnCount();

            foreach (HtmlRow row in mRows)
            {
                while (row.CellCount < columnCount)
                {
                    row.Add(new HtmlCell(CssBoxBorders.CreateEmpty()));
                }
            }
        }

        private void RemoveExtraEmptyCells()
        {
            int maxColumnCountWithoutExtraEmptyCells = 0;
            foreach (HtmlRow row in mRows)
            {
                maxColumnCountWithoutExtraEmptyCells = System.Math.Max(
                    maxColumnCountWithoutExtraEmptyCells,
                    row.GetColumnCountWithoutTrailingEmptyExtraCells());
            }

            // Remove all empty extra cells at the end of each row.
            foreach (HtmlRow row in mRows)
            {
                while (row.CellCount > maxColumnCountWithoutExtraEmptyCells)
                {
                    HtmlCell cell = row[row.CellCount - 1];
                    HtmlCell firstMergedCell = cell.HorizontalMergeFirstCell;
                    if ((firstMergedCell != null) && (firstMergedCell.ColSpan > 1))
                    {
                        firstMergedCell.ColSpan--;
                    }

                    row.RemoveLastCell();
                }
            }
        }

        /// <summary>
        /// Creates a required amount of empty row span cells and marks them with vertical merge
        /// flag as specified by row span.
        ///
        /// <see cref="MakeRowsSameLength"/> simply adds empty cells to the end of the rows and it seems to
        /// be not enough when processing rowspans. To correctly build a table, we should add
        /// (rowspan - 1) empty cells below a cell with rowspan attribute.
        /// </summary>
        private void VerticalMerge()
        {
            int columnCount = GetMaxColumnCount();

            for (int columnIndex = 0; columnIndex < columnCount; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < mRows.Count; rowIndex++)
                {
                    HtmlRow row = this[rowIndex];

                    if (row.CellCount <= columnIndex)
                        continue;

                    HtmlCell firstCell = row[columnIndex];

                    // Limit rowspan by the number of remaining rows below.
                    int indexAfterRowSpan = System.Math.Min(rowIndex + firstCell.RowSpan, mRows.Count);

                    // Start with 1 because this is for the second and consecutive cells.
                    for (int spanRowIndex = rowIndex + 1; spanRowIndex < indexAfterRowSpan; spanRowIndex++)
                    {
                        // WORDSNET-15611 Rowspans cannot cross row group borders.
                        if (this[spanRowIndex].RowGroup != row.RowGroup)
                        {
                            break;
                        }

                        // The first (0 index) cell in the span gets this attribute, but only if there are rows to span.
                        firstCell.VerticalMerge = CellMerge.First;

                        if (columnIndex + 1 > this[spanRowIndex].CellCount)
                        {
                            // The next row is shorter than that with the rowspan cell, so append empty cells.
                            AddCellsToRow(spanRowIndex, columnIndex - this[spanRowIndex].CellCount + 1, firstCell.CssBorders);
                        }
                        else
                        {
                            // Otherwise insert an empty cell at cellIndex.
                            if (this[spanRowIndex][columnIndex].HorizontalMerge != CellMerge.Previous)
                                InsertCell(spanRowIndex, columnIndex, new HtmlCell(firstCell.CssBorders));
                        }

                        HtmlCell curCell = this[spanRowIndex][columnIndex];
                        curCell.VerticalMerge = CellMerge.Previous;
                        curCell.VerticalMergeFirstCell = firstCell;

                        // WORDSNET-12435 Don't break existing horizontal merge.
                        if (curCell.HorizontalMerge != CellMerge.Previous)
                        {
                            // Copy the horizontal merge attribute from the top cell to all vertically merged
                            // cells below. This covers the situation when a cell has both colspan and rowspan.
                            curCell.HorizontalMerge = firstCell.HorizontalMerge;
                            curCell.HorizontalMergeFirstCell = firstCell.HorizontalMergeFirstCell;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether an HTML node is a special row (tr) added by MS Word or Aspose.Words during export because
        /// the table contains misaligned cells.
        /// </summary>
        private static bool IsColumnAlignmentRow(HtmlElementNode node, CssDeclarationCollection declarations)
        {
            // The special row is always a <tr> element.
            if (node.Name != "tr")
            {
                return false;
            }

            // The special row always has zero length.
            CssDeclaration heightDeclaration = declarations["height"];
            if ((heightDeclaration == null) || !MathUtil.IsZero(heightDeclaration.Value.FirstValue.DoubleValue))
            {
                return false;
            }

            // The special row is always last in a table.
            if (node.NextSibling != null)
            {
                if (!(node.NextSibling is HtmlTextNode)
                    || node.NextSibling.ContainsText()
                    || (node.NextSibling.NextSibling != null))
                {
                    // There is an element or meaninful text after this row.
                    return false;
                }
            }

            // The special row contains one or more empty cells that are always <td> elements. Each cell can contain nothing
            // but a single whitespace child.
            int cellCount = 0;
            foreach (HtmlNode child in node.Children)
            {
                if (child is HtmlElementNode)
                {
                    HtmlElementNode childElement = (HtmlElementNode)child;
                    if ((childElement.Name != "td") || (childElement.Children.Count > 1))
                    {
                        return false;
                    }
                    if (childElement.Children.Count == 1)
                    {
                        if (!(childElement.Children[0] is HtmlTextNode) || childElement.Children[0].ContainsText())
                        {
                            return false;
                        }
                    }
                    ++cellCount;
                }
                else if (child.ContainsText())
                {
                    return false;
                }
            }

            return cellCount > 0;
        }

        internal HtmlRow this[int index]
        {
            get { return mRows[index]; }
        }

        internal int RowCount
        {
            get { return mRows.Count; }
        }

        /// <summary>
        /// Count of table captions.
        /// </summary>
        internal int CaptionCount
        {
            get { return mCaptions.Count; }
        }

        /// <summary>
        /// This is the maximum number of columns, all rows are padded with cells to match this.
        /// </summary>
        internal int ColumnCount { get; private set; }

        /// <summary>
        /// Gets count of 'col' elements declared in the table.
        /// </summary>
        internal int ColElementCount
        {
            get { return mHtmlCols.Count; }
        }

        /// <summary>
        /// CSS borders of the table.
        /// </summary>
        internal CssBoxBorders CssBorders { get; }

        /// <summary>
        /// Indicates whether the table borders are collapsed into a single border ('border-collapse:collapse').
        /// </summary>
        internal bool IsBorderCollapsed { get; }

        /// <summary>
        /// Indicates whether the table is defined not with 'table' tag,
        /// but with "display: table" and "display:inline-table" property.
        /// </summary>
        internal bool IsDisplayTable { get; private set; }

        internal CssBorder FirstRowFirstCellLeftBorder { get; set; }

        internal CssBorder FirstRowLastCellRightBorder { get; set; }

        /// <summary>
        /// Gets the current HTML row.
        /// </summary>
        private HtmlRow CurRow
        {
            get { return mRows[mRows.Count - 1]; }
        }

        private readonly List<HtmlRow> mRows;
        private readonly CssStyleTracker mCssStyleTracker;
        private readonly List<HtmlTableCaption> mCaptions;
        private readonly List<HtmlCol> mHtmlCols;
        private HtmlRowGroup mCurrentHtmlRowGroup;
        private HtmlColGroup mCurrentHtmlColGroup;

        /// <summary>
        /// This helps to stop processing nested tables.
        /// </summary>
        private bool mIsInTable;

        /// <summary>
        /// WORDSNET-16075, WORDSNET-13896 Indicates, if table is CSS table (not HTML table).
        /// Looks like that browsers don't mix HTML tables with CSS tables, and we do the same.
        /// </summary>
        private bool mIsCssTable;
        private readonly bool mIsXhtml;
    }
}
