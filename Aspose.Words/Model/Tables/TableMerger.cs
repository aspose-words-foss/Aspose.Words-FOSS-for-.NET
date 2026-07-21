// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2016 by Dmitry Matveenko

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.TableLayout;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Implements methods for merging tables adjacent in the document model.
    /// </summary>
    internal class TableMerger
    {
        static TableMerger()
        {
            // Sort the attributes array, to be on the safe side if someone changes the numbers.
            Array.Sort(gAttributesIgnored);

            // Merge old and new attributes to use with the new logic.
            gAttributesIgnoredCombined = new int[gAttributesIgnored.Length + gAttributesIgnoredNew.Length];
            Array.Copy(gAttributesIgnored, gAttributesIgnoredCombined, gAttributesIgnored.Length);
            Array.Copy(gAttributesIgnoredNew, 0, gAttributesIgnoredCombined,
                gAttributesIgnored.Length, gAttributesIgnoredNew.Length);
            Array.Sort(gAttributesIgnoredCombined);
        }

        /// <summary>
        /// Checks if there are adjacent table to be combined with the given table after it,
        /// and joins the table with adjacent tables before it if there are none.
        /// </summary>
        /// <returns>The combined table or null if there are combinable tables after.</returns>
        /// <remarks>
        /// <para>
        /// The method is supposed to be called from DocumentVisitor.
        /// It actually joins the table only when last table that can be merged is found.
        /// This allows the visitor to do whatever it does on cell and row level before the tables are joined.
        /// </para>
        /// <para>
        /// The method emulates MS Word behavior by combining table grids stored in the document for the merged tables.
        /// It is to be called from DocumentPostLoader.
        /// </para>
        /// </remarks>
        internal Table JoinAdjacentTablesCombineGrids(Table table)
        {
            // Set "missing default table style condition" only when combining grids (in postLoader).
            SetIsTableNormalMissingOnLoading(table);
            return JoinAdjacentTables(table, true);
        }

        /// <summary>
        /// Checks if there are adjacent table to be combined with the given table after it,
        /// and joins the table with adjacent tables before it if there are none.
        /// </summary>
        /// <returns>The combined table or null if there are combinable tables after.</returns>
        /// <remarks>
        /// <para>
        /// The method is supposed to be called from DocumentVisitor.
        /// It actually joins the table only when last table that can be merged is found.
        /// This allows the visitor to do whatever it does on cell and row level before the tables are joined.
        /// </para>
        /// <para>
        /// The methods just moves table rows to the first table.
        /// It discards table grids for the removed tables.
        /// It does not update cell grid spans.
        /// This is how table merge was originally implemented in document validator.
        /// </para>
        /// </remarks>
        internal Table JoinAdjacentTablesDiscardGrids(Table table)
        {
            return JoinAdjacentTables(table, false);
        }

        /// <summary>
        /// Implements adjacent table merge.
        /// </summary>
        private Table JoinAdjacentTables(Table table, bool combineGrids)
        {
            if (IsNextTableMergeNeeded(table, combineGrids))
            {
                // Just remember the table, it will be merged when the last table to be merged is found.
                RememberTable(table);
                return null;
            }

            if (!HasMergeableTablesBefore(table))
            {
                // The table does not need merging, just return the table.
                return table;
            }

            List<Table> adjacentTables = mAdjacentTableLists.Pop();
            Debug.Assert(adjacentTables.Count > 0);

            Table firstTable = adjacentTables[0];
            adjacentTables.Add(table);

            bool allGridsApplied = true;
            foreach (Table tbl in adjacentTables)
                allGridsApplied = allGridsApplied && tbl.IsCalculatedGridApplied;

            // Use the new logic that combines grids:
            // - on loading, always;
            // - on validation (before saving), if the new grids are applied.
            if (combineGrids || allGridsApplied)
            {
                SortedIntegerListGeneric<object> combinedGrid = GetCombinedGrid(adjacentTables);
                UpdateSpanCountsForCombinedGrid(adjacentTables, combinedGrid);

                // Set the combined grid.
                TableGridColumnsAttr combinedWidths = new TableGridColumnsAttr(ToIntList(combinedGrid));
                firstTable.TablePr.SetAttr(TableAttr.Sys_TableGridForNewAlgorithm, combinedWidths);

                // I've tried removing unneeded indents in the merged tables, but it broke a couple of tests.
                // TestJira14246 and TestDefect5221 do save-open, checking the opened and re-saved document against the same gold.
                // On the first save, some tables are merged, and if indents are removed, a gold without indents must be accepted.
                // But on opening the saved document, AW adds indents all the same, so the new gold does not work for the second save.
                // It appears that unneeded indents should be handled at the model level. Leave them be for now.
            }
            else
            {
                // Use the older logic that just moves all rows into a single table
                // not trying to re-calculate grid spans or taking grids into account.

                // WORDSNET-13599 Allow the re-calculation of the grid and spans of the cells,
                // while document writing.
                // DM: The calculation will actually fall back to using cell widths.
                // If cell widths are not correct, the result will be incorrect as well.
                TableGridAttr calculatedGrid = firstTable.TablePr.CalculatedGrid;
                if (calculatedGrid != null)
                    calculatedGrid.State = GridState.CalculationFailed;
            }

            // Move all rows to the first merged table.
            MergeTables(adjacentTables);

            return firstTable;
        }

        /// <summary>
        /// Registers the given table in a list of tables that should be merged together.
        /// </summary>
        private void RememberTable(Table table)
        {
            if (!HasMergeableTablesBefore(table))
            {
                // The table is on a different level (can happen for nested tables).
                // Add a new list.
                mAdjacentTableLists.Push(new List<Table>());
            }

            List<Table> topList = mAdjacentTableLists.Peek();
            topList.Add(table);
        }

        /// <summary>
        /// Gets a boolean value indicating if tables that should be merged with the given table were already registered.
        /// </summary>
        private bool HasMergeableTablesBefore(Table table)
        {
            if (mAdjacentTableLists.Count == 0)
                return false;

            List<Table> topList = mAdjacentTableLists.Peek();
            Debug.Assert(topList.Count > 0);

            Table firstInMerge = topList[0];
            return (firstInMerge.ParentNode == table.ParentNode);
        }

        /// <summary>
        /// Checks if the two adjacent tables should be merged into one.
        /// </summary>
        private static bool IsTableMergeNeeded(Table table1, Table table2, bool combineGrids)
        {
            // WORDSNET-10543 If at least one of the tables has no cells, then we do not compare their formatting at all.
            if (!table1.HasCells || !table2.HasCells)
                return false;

            ValidateTableStyle(table1);
            ValidateTableStyle(table2);

            Row row1 = table1.FirstRow;
            Row row2 = table2.FirstRow;

            // TestJira6025HideMarkCustomer shows that different styles prevent merging.
            // One exception is missing style vs default style. Compare resolved Istds.
            if (row1.TablePr.Istd != row2.TablePr.Istd)
                return false;

            bool bothGridsApplied = table1.IsCalculatedGridApplied && table2.IsCalculatedGridApplied;
            int[] ignoredAttributes = bothGridsApplied || combineGrids
                ? gAttributesIgnoredCombined
                : gAttributesIgnored;
            // Compare table formatting excluding row exclusive attributes.
            if (!row1.TablePr.Equals(row2.TablePr, ignoredAttributes))
                return false;

            // Finally do deep comparison including inner paragraphs.
            if (!row1.IsSameRowFloatingPositioning(row2))
                return false;

            return true;
        }

        /// <summary>
        /// Checks that table has valid table style, assigns TableNormal otherwise.
        /// </summary>
        /// <remarks>
        /// This validation is actually performed later but
        /// we need it before we decide whether we need to merge tables.
        /// </remarks>
        private static void ValidateTableStyle(Table table)
        {
            TablePr tablePr = table.FirstRow.TablePr;

            if (tablePr.Contains(TableAttr.Istd))
            {
                Style tableStyle = table.Document.Styles.GetByIstd(tablePr.Istd, false);
                if(tableStyle != null && tableStyle.Type != StyleType.Table)
                    tablePr.SetAttr(TableAttr.Istd, StyleIndex.TableNormal);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating if the next sibling of the given table is a table that should be merged with it.
        /// </summary>
        private static bool IsNextTableMergeNeeded(Table table, bool combineGrids)
        {
            // Merging tables when one or both of them are in SDTs is not implemented.
            // MS Word itself does not handle the case very well.
            // TestTableMergeSdt demonstrates different cases when an error occurs in MS Word during saving
            // if editing the document results in adjacent tables wrapped in SDT.

            // So the code below just checks siblings, it does not ascend/descend SDTs.
            Node nextNode = table.NextNonAnnotationSibling;
            bool isNextSiblingTable = (nextNode != null) && (nextNode.NodeType == NodeType.Table);
            bool isNextTableMergeNeeded = isNextSiblingTable &&
                                  IsTableMergeNeeded(table, (Table)nextNode, combineGrids);

            return isNextTableMergeNeeded;
        }

        /// <summary>
        /// Joins the tables in the specified list.
        /// </summary>
        private static void MergeTables(List<Table> adjacentTables)
        {
            Table firstTable = adjacentTables[0];
            Debug.Assert(firstTable != null);

            // Start from the next table.
            for (int tableIndex = 1; tableIndex < adjacentTables.Count; ++tableIndex)
            {
                Table table = adjacentTables[tableIndex];

                // WORDSNET-10636 Table in glossary may not have parent.
                // Also table can be removed by TableValidator.
                if (table.ParentNode == null)
                    continue;

                Table prevTable = table.PreviousNonAnnotationSibling as Table;
                Debug.Assert(firstTable == prevTable);
                if (prevTable == null)
                    continue;

                // Move annotation nodes between tables to row level.
                prevTable.InsertAfter(prevTable.NextSibling, table, prevTable.LastChild);

                // Move rows.
                while (table.FirstChild != null)
                    prevTable.AppendChild(table.FirstChild);

                table.Remove();
            }
        }

        /// <summary>
        /// Converts a hashset of column boundaries to a list of column widths.
        /// </summary>
        private static IntList ToIntList(SortedIntegerListGeneric<object> columns)
        {
            IntList grid = new IntList(System.Math.Max(columns.Count - 1, 0));
            for (int columnIndex = 1; columnIndex < columns.Count; columnIndex++)
            {
                int columnWidth = columns.GetKey(columnIndex) - columns.GetKey(columnIndex - 1);
                grid.Add(columnWidth);
            }

            return grid;
        }

        /// <summary>
        /// Gets a combined list of column boundaries for the given table list according to table grids.
        /// </summary>
        /// <returns>A sorted list that stores relative boundaries of all columns in a grid
        /// made from overlapping of table grids for tables in the list.</returns>
        /// <remarks>
        /// List keys represent the boundaries, the values are null.
        /// </remarks>
        private SortedIntegerListGeneric<object> GetCombinedGrid(List<Table> tables)
        {
            Debug.Assert(tables.Count > 0);

            Table leftmostTable = GetLeftmostTable(tables);

            // Test26496(): a very special behavior under a weird condition.
            SetWriteWidthBeforeTableX(leftmostTable);

            SortedIntegerListGeneric<object> combinedGrid = new SortedIntegerListGeneric<object>();
            int minTableX = GetTableX(leftmostTable);
            AddToGrid(combinedGrid, minTableX);

            foreach (Table table in tables)
            {
                // Insert column boundaries for this table into combined grid.
                int tableX = GetTableX(table);

                // Test26496(): insert the left table boundary into the grid under a special condition.
                if (WriteWidthBeforeTableX)
                    AddToGrid(combinedGrid, tableX);

                int columnRight = tableX;
                IntList tableGrid = GetTableGrid(table);
                // Column count may be greater than the grid count in case grid columns are missing.
                int columnCount = GetColumnCount(table);
                // The right table boundary is treated specially.
                for (int col = 0; col < columnCount; ++col)
                {
                    // Default width is returned for columns missing in the grid.
                    int columnWidth = GetGridColumnWidth(tableGrid, col);
                    columnRight += columnWidth;
                    AddToGrid(combinedGrid, columnRight);
                }
            }

            return combinedGrid;
        }

        /// <summary>
        /// Gets the table with the leftmost position from the given list.
        /// </summary>
        /// <remarks>
        /// If several tables have the same position, the first one is returned.
        /// </remarks>
        private static Table GetLeftmostTable(List<Table> tables)
        {
            Debug.Assert(tables.Count > 0);

            Table leftmostTable = null;
            int minTableLeft = int.MaxValue;
            foreach (Table table in tables)
            {
                int tableLeft = GetTableX(table);
                if (tableLeft < minTableLeft)
                {
                    minTableLeft = tableLeft;
                    leftmostTable = table;
                }
            }

            return leftmostTable;
        }

        /// <summary>
        /// Sets a special condition about TableNormal style missing.
        /// </summary>
        private void SetIsTableNormalMissingOnLoading(Table table)
        {
            // If already set, do not reset.
            if (IsTableNormalMissingOnLoading)
                return;

            if (table.Document == null || table.Document.Styles == null)
                return;

            IsTableNormalMissingOnLoading =
                table.Document.Styles.GetBySti(StyleIdentifier.TableNormal, false) == null;
        }

        /// <summary>
        /// Stores a special condition about TableNormal style missing.
        /// </summary>
        /// <remarks>
        /// This is a rare condition that affects merging tables on loading the document.
        /// See usages for details.
        /// </remarks>
        private bool IsTableNormalMissingOnLoading { get; set; }

        /// <summary>
        /// Calculates a condition to imitate some special MS Word logic.
        /// </summary>
        /// <remarks>
        /// Test26496(): MS Word has a very special behavior under a weird condition.
        /// The behavior is imitated by storing widthBefore for the rows of the indented merged tables.
        /// </remarks>
        private void SetWriteWidthBeforeTableX(Table leftmostTable)
        {
            // A weird condition that fixes Test26946.
            // Actually, MS Word behavior seems to depend on presence of a table style with w:default="1".
            // But since we do not read this style attribute, this is what I came up with.
            // The case of missing default table styles with adjacent tables in the document present should be rare.
            WriteWidthBeforeTableX = IsTableNormalMissingOnLoading && (leftmostTable.Alignment != TableAlignment.Left);
            // TODO: what happens if there is a mix of differently aligned tables at 0 position?
        }

        /// <summary>
        /// Indicates if special logic should be used for setting widthBefore values.
        /// </summary>
        /// <remarks>
        /// The logic affects Test26496() and TestMergeAndSaveToPdf().
        /// The case is rare.
        /// </remarks>
        private bool WriteWidthBeforeTableX { get; set; }

        private static void AddToGrid(SortedIntegerListGeneric<object> grid, int column)
        {
            if (!grid.ContainsKey(column))
                grid[column] = null;
        }

        private static int GetGridColumnWidth(IntList grid, int column)
        {
            return (column < grid.Count)
                ? grid[column]
                : TableGridMetrics.DefaultFixedColumnWidthTwips;
        }

        /// <summary>
        /// Gets the column widths (grid) to use with the new logic that relies on table grids.
        /// </summary>
        private static IntList GetTableGrid(Table table)
        {
            IntList grid;
            if (table.IsCalculatedGridApplied)
            {
                // Use the calculated grid if there is any.
                grid = table.TablePr.CalculatedGrid.Columns;
            }
            else
            {
                // Use the original grid stored in the document.
                TableGridColumnsAttr gridFromDoc = (TableGridColumnsAttr)table.TablePr.GetDirectAttr(TableAttr.Sys_TableGridForNewAlgorithm);

                // Return an empty list if there is no grid in the document.
                grid = (gridFromDoc == null)
                    ? new IntList()
                    : gridFromDoc.GridColumns;
            }

            TableGridConstructor.RemoveInvalidGridColumns(grid);
            return grid;
        }

        private static int GetColumnCount(Table table)
        {
            int maxColumnCount = 0;
            foreach (Row row in table.Rows)
            {
                // Only spanCounts are used here. The widths are not used.
                int columnCount = row.TablePr.GridBefore;
                foreach (Cell cell in row.Cells)
                    columnCount += cell.CellPr.GridSpan;

                if (maxColumnCount < columnCount)
                    maxColumnCount = columnCount;
            }

            return maxColumnCount;
        }

        /// <summary>
        /// Updates cell span counts in a combined table to match the combined grid.
        /// </summary>
        /// <remarks>
        /// The cell spans are calculated from table grids, even if the grids are not correct.
        /// Extra grid columns should not affect anything.
        /// They may only cause span count increase in other tables, but the increased span will match the same grid width.
        /// Missing grid columns will cause cell spans to be re-computed from the default width.
        /// Cell spans may change when the combined table grid is re-calculated:
        /// grid columns matching wide cell intersections may (or may not) be removed.
        /// It seems, the same logic works for auto-fit tables.
        /// </remarks>
        private void UpdateSpanCountsForCombinedGrid(List<Table> tables, SortedIntegerListGeneric<object> combinedGrid)
        {
            // The left of the first column (table.X) is present in the combined grid, decrease 1 to get column count.
            int combinedGridColumnCount = combinedGrid.Count - 1;
            int combinedGridRight = combinedGrid.GetKey(combinedGridColumnCount);
            int combinedGridX = combinedGrid.GetKey(0);

            foreach (Table table in tables)
            {
                IntList tableGrid = GetTableGrid(table);

                int tableX = GetTableX(table);
                // tableX is (normally) not written to the combined grid, so there may not be a matching grid column boundary.
                // Look for maximum combined grid column that is before (or equals) this tableX.
                int tableXGrid = 0;
                for (int colIdx = 0; colIdx < combinedGrid.Count; ++colIdx)
                {
                    int nextColumn = combinedGrid.GetKey(colIdx);
                    if (nextColumn <= tableX)
                        tableXGrid = nextColumn;
                    else
                        break;
                }
                // This will effectively widen the first cell (or grid before)
                // in tables indented relatively to the leftmost tables,
                // unless they happen to align with a combined grid column boundary.
                // See tests like TestTableMergeIndentE().

                foreach (Row row in table.Rows)
                {
                    // Only spanCounts are used here. The widths are not used.
                    int firstCellColumn = 0;
                    int cellLeft = 0;
                    int cellRight = 0;
                    int combinedColumnRight = tableXGrid;

                    int cellSpanCount = row.TablePr.GridBefore;
                    if (cellSpanCount > 0)
                    {
                        cellRight = GetColumnRightFromTableGrid(tableGrid, firstCellColumn, cellLeft, cellSpanCount);
                        combinedColumnRight = tableX + cellRight;
                    }
                    int combinedColumns = GetColumnCountFromCombinedGrid(combinedGrid, combinedColumnRight);

                    // WidthBefore without gridBefore is ignored by the above code.
                    // It seems, MS Word currently works this way, see the document in TestTableMergWidthBeforeWtGridBefore().

                    row.TablePr.SetAttr(TableAttr.Sys_GridBefore, combinedColumns);
                    // Only write widthBefore under a very special and rare condition.
                    if (combinedColumns > 0 && WriteWidthBeforeTableX)
                    {
                        int widthBeforeTwips = combinedColumnRight - combinedGridX;
                        PreferredWidth widthBefore = PreferredWidth.FromTwipsSafe(widthBeforeTwips);
                        row.TablePr.SetAttr(TableAttr.WidthBefore, widthBefore);
                        // Set the original width before as it is no longer valid for the combined table. Test26496().
                        row.TablePr.SetAttr(TableAttr.WidthBeforeOriginal, widthBefore);
                    }

                    firstCellColumn += cellSpanCount;
                    cellLeft = cellRight;
                    int combinedColumnsBeforeCell = combinedColumns;

                    foreach (Cell cell in row.Cells)
                    {
                        // For cells with horizontal merge it will produce a strange mix of cells
                        // where both merge and grid spans are set.
                        // It does not affect grid calculation.
                        cellSpanCount = cell.CellPr.GridSpan;
                        cellRight = GetColumnRightFromTableGrid(tableGrid, firstCellColumn, cellLeft, cellSpanCount);

                        combinedColumnRight = tableX + cellRight;
                        combinedColumns = GetColumnCountFromCombinedGrid(combinedGrid, combinedColumnRight);
                        int combinedColumnsCount = combinedColumns - combinedColumnsBeforeCell;

                        // WORDSJAVA-2144 - Can't load RTF with 0-columns merged table inside.
                        Debug.Assert(combinedColumnsCount >= 0);

                        cell.CellPr.GridSpan = combinedColumnsCount;
                        // Test19871autoB: do not set twip width to zero pct preferred.
                        PreferredWidth cellPreferred = cell.CellPr.PreferredWidth;
                        if (!table.AllowAutoFit && cellPreferred.IsAuto && cellPreferred.Type != PreferredWidthType.Percent)
                        {
                            // Without this setting, "auto" fixed width may be wrong in a combined table.
                            cell.CellPr.PreferredWidth = PreferredWidth.FromTwipsSafe(cellRight - cellLeft);
                            // Actually, MS Word seems to always assign preferred width to cells in fixed layout tables if it is missing.
                            // But I do it only here and during fixed grid calculation.
                            // TODO Consider doing it in PostLoader for all fixed layout tables.
                            // Cells with auto width in auto-fit tables should remain at zero as TestFixedVsAutoDifferentSpansA shows.
                        }

                        firstCellColumn += cellSpanCount;
                        cellLeft = cellRight;
                        combinedColumnsBeforeCell = combinedColumns;
                    }

                    // Update widthAfter.
                    int widthAfter = combinedGridRight - combinedColumnRight;
                    if (widthAfter > 0)
                    {
                        row.TablePr.WidthAfterTwips = widthAfter;
                        int gridAfter = combinedGridColumnCount - combinedColumns;
                        Debug.Assert(gridAfter > 0);
                        row.TablePr.SetAttr(TableAttr.Sys_GridAfter, gridAfter);
                    }
                }
            }
        }

        private static int GetTableX(Table table)
        {
            // Disregard negative indents.
            int indent = System.Math.Max(0, table.FirstRow.TablePr.LeftIndent);

            // The code somewhat parallels the code in layout, because it calculates similar things.
            // See RowMetrics.GetXForWord2010OrPriorCompatible() and CellMetrics.GetSpaceLeft().
            // I see no easy way to extract the common logic. Let it be for now.
            int borderAndPaddingShift = 0;
            Cell firstCell = table.FirstRow.FirstCell;
            Debug.Assert(firstCell != null);
            if (firstCell != null)
            {
                borderAndPaddingShift = table.Document.DocPr.CompatibilityOptions.IsWord2013OrLaterCompatible
                    ? GetBorderShift(firstCell)
                    : GetBorderAndPaddingShiftCompatibility(firstCell);
            }

            // TestTableMergeIndentI confirms that border and padding must be accounted for.
            return indent + borderAndPaddingShift;
        }

        private static int GetBorderAndPaddingShiftCompatibility(Cell firstCell)
        {
            int borderAndPaddingShift;
            if (firstCell.ParentTable.IsNested)
            {
                borderAndPaddingShift = GetBorderShift(firstCell);
            }
            else
            {
                // In compatibility mode, tables are shifted to the left of the column margins because of borders and cell padding.
                // It affects how they are merged.
                int padding = Extensions.ResolvedLeftPadding(firstCell);
                int borderShift = GetBorderShift(firstCell);

                borderAndPaddingShift = -System.Math.Max(padding, borderShift);
            }

            return borderAndPaddingShift;
        }

        private static int GetBorderShift(Cell firstCell)
        {
            // Even if there is widthBefore the first cell in the first row, the table position is still determined by its left border width.
            int borderWidth = Extensions.GetLeftBorderTwips(firstCell);

            // In 2013 mode, the border of the first cell is aligned to the container margin.
            // So the grid actually starts half a border later.
            // The behavior is the same for tables nested in shapes, tables etc.
            // In compatibility mode, only nested tables behave this way.
            return MathUtil.Divide(borderWidth, 2);
        }

        private static int GetColumnRightFromTableGrid(IntList tableGrid, int firstColumn, int columnLeft,
                                                       int columnCount)
        {
            int columnRight = columnLeft;
            for (int i = 0; i < columnCount; ++i)
            {
                int column = firstColumn + i;
                bool isTableGridColumnPresent = (column < tableGrid.Count);
                columnRight += isTableGridColumnPresent
                    ? tableGrid[column]
                    : TableGridMetrics.DefaultFixedColumnWidthTwips;
            }
            // An array with cumulative column widths sums could be more effective.
            // Leave it be for now, this way there is less code , there is no need for additional arrays
            // and it is not a performance problem.

            return columnRight;
        }

        private static int GetColumnCountFromCombinedGrid(SortedIntegerListGeneric<object> combinedGrid, int columnRight)
        {
            int columnIndex = combinedGrid.IndexOfKey(columnRight);

            // All cell boundaries should be present in the combined grid.
            Debug.Assert(columnIndex >= 0);
            return columnIndex;
        }

        /// <summary>
        /// Stack of lists of adjacent tables to be merged before table grid calculation.
        /// </summary>
        private readonly Stack<List<Table>> mAdjacentTableLists = new Stack<List<Table>>();

        /// <summary>
        /// Attributes that do not prevent merging adjacent tables, both in the "old" and the "new" logic.
        /// </summary>
        private static readonly int[] gAttributesIgnored = new int[]
        {
            // TableAttr.AllowAutoFit:
            // It appears that after some update that happened around Mar or Apr' 25, MS Word demonstrates a new behavior:
            // consecutive tables with different tblLayout values are no longer merged.
            // The new behavior is more logical, especially in the case when there are nested tables in merged tables,
            // as nested table layouts should be updated to match the container table.
            // TestTableMergeLayout*() demonstrate the new behavior.
            // Several customers requested the new behavior.
            // So TableAttr.AllowAutoFit was excluded from this list per WORDSNET-28521.
            // It broke several tests, and in some of those the layout differed significantly
            // and the customer had specifically wanted the old behavior (Test13434, Test20814).
            // But as the breaking change was actually in MS Word first, I believe that is OK.
            // AW logic now matches the current MS Word behavior.

            TableAttr.LeftPadding, //4020
            TableAttr.HeadingFormat, //4040
            TableAttr.BorderTop, //4050
            TableAttr.BorderLeft, //4060
            TableAttr.BorderBottom, //4070
            TableAttr.BorderRight, //4080
            TableAttr.BorderHorizontal, //4090
            TableAttr.BorderVertical, //4100
            TableAttr.RowHeight, //4120
            TableAttr.PreferredWidth, //4230
            TableAttr.WidthBefore, //4250
            TableAttr.WidthAfter, //4260
            TableAttr.RightPadding, //4320
            TableAttr.AllowBreakAcrossPages, //4360
            TableAttr.RsidTr, //4400
            TableAttr.Sys_GridAfter, //5105
            TableAttr.Sys_CalculatedTableGrid, //5106
            TableAttr.Sys_TableGridForNewAlgorithm, //5107
        };

        /// <summary>
        /// Attributes that do not prevent merging adjacent tables, in the "new" logic only.
        /// </summary>
        private static readonly int[] gAttributesIgnoredNew = new int[]
        {
            TableAttr.Istd, // 4005
            TableAttr.Alignment, //4010
            TableAttr.StyleOptions, //4140
            TableAttr.WidthBeforeOriginal, //4251
            TableAttr.WidthAfterOriginal, //4261
            TableAttr.Shading, // 4330
            // Table with different indents are combined, but there is some special logic to handle indents.
            TableAttr.LeftIndent, // 4340
            TableAttr.Sys_GridBefore, //5104
        };

        /// <summary>
        /// Attributes that do not prevent merging adjacent tables, in the "new" logic.
        /// </summary>
        /// <remarks>
        /// This is filled in the static constructor.
        /// </remarks>
        private static readonly int[] gAttributesIgnoredCombined;
    }
}
