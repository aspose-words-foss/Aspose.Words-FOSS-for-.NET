// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/05/2016 by Dmitry Matveenko

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Implements re-calculation of table column widths from table/cell properties.
    /// </summary>
    /// <remarks>
    /// The methods are used both for fixed and auto-fit table layout.
    /// </remarks>
    internal class TableGridConstructor
    {
        /// <summary>
        /// Gets an instance or TableGridConstructor initialized with the given values.
        /// </summary>
        internal TableGridConstructor(IntList gridFromDocument, IList<RowSpan> rows,
            int halfSpacing, bool isAutoFit, bool useWord2003Rules, bool isBinaryDocRtf)
        {
            IsAutoFit = isAutoFit;
            HalfSpacing = halfSpacing;
            IsBinaryDocRtf = isBinaryDocRtf;

            GridFromDocumentOriginal = gridFromDocument == null
                ? new IntList()
                : gridFromDocument;

            RemoveInvalidGridColumns(GridFromDocumentOriginal);

            Rows = rows;

            GridColumnTypes = new List<GridCellMatchType>();
            EndingCellPreferredMaxList = new List<int[]>();

            BlockGrids = new List<TableGrid>();
            NextBlockStartRows = new IntList();
            BlockGridsFromDocument = new List<IntList>();
            CurrentBlock = CurrentBlockNotSet;

            UseWord2003Rules = useWord2003Rules;

            // Remove unneeded columns that match only parts of wide cells.
            MergeNarrowOriginalGridColumns();
        }

        /// <summary>
        /// Truncates the grid starting from the first invalid column.
        /// </summary>
        /// <remarks>
        /// Sometimes (TestJira10983) the grid is present in the document, but with zero values.
        /// It seems, MS Word disregards such columns.
        /// </remarks>
        internal static void RemoveInvalidGridColumns(IntList grid)
        {
            if (grid == null)
                return;

            for (int i = 0; i < grid.Count; ++i)
            {
                if (grid[i] <= 0)
                {
                    int allTheRest = grid.Count - i;
                    grid.RemoveRange(i, allTheRest);
                    break;
                }
            }
        }

        /// <summary>
        /// Implements table grid construction from table/cell properties.
        /// </summary>
        /// <remarks>
        /// Table and container width is not taken into account, resizing to table width is implemented by a different class.
        /// </remarks>
        internal void GetGridFromCells()
        {
            do
            {
                // Clear columns but preserve the state.
                Grid.RemoveAllColumns();

                // Get the next chunk of rows that has no more than 64 columns.
                GetNextRowBlock();

                // Update from single-column cells.
                ConstructCellSpans();

                // Update column types and preferred widths from wide cells and widthBefore/widthAfter.
                UpdatePreferredFromWideCells();

                // Remove unneeded width before/after.
                RemoveWidthBeforeAfterAuto();

                // Update column minimums so that they are no less than the default minimum values.
                AdjustGridMinimumsFromDefaults();
                // The logic is a bit different for different modes.

                // Distribute wide cell minimums, maximums and preferred widths.
                DistributeWideCells();

            } while (RemoveIntersectionsBeforeResizing());
        }

        /// <summary>
        /// Moves to the next row block and adds data elements to store the block data.
        /// </summary>
        /// <returns>A boolean value indicating if the next row block exists.</returns>
        internal bool PrepareNextRowBlock()
        {
            Debug.Assert(GridFromDocumentOriginal != null);

            MoveToNextRowBlock();

            bool haveMoreRows = (RowBlockStart < Rows.Count);

            // Set grid from document to the original.
            if (haveMoreRows)
            {
                TableGrid newBlockGrid = new TableGrid(IsAutoFit);
                BlockGrids.Add(newBlockGrid);

                IntList newGridFromDocumentBlock = new IntList(GridFromDocumentOriginal);
                BlockGridsFromDocument.Add(newGridFromDocumentBlock);

                // This will be overwritten when the next block of rows is defined.
                NextBlockStartRows.Add(RowBlockStart);
            }

            return haveMoreRows;
        }

        /// <summary>
        /// Moves to the next row block created during previous grid construction stages.
        /// </summary>
        /// <returns>A boolean value indicating if the next row block exists.</returns>
        internal bool MoveToNextRowBlock()
        {
            Debug.Assert(BlockGrids.Count == NextBlockStartRows.Count && BlockGrids.Count == BlockGridsFromDocument.Count);

            if (CurrentBlock == CurrentBlockNotSet)
            {
                CurrentBlock = 0;
                RowBlockStart = 0;
            }
            else if (CurrentBlock < BlockGrids.Count)
            {
                RowBlockStart = RowBlockNext;
                ++CurrentBlock;
            }

            return (CurrentBlock < BlockGrids.Count);
        }

        /// <summary>
        /// Resets the current row block index preparing so that the next move is to the first block.
        /// </summary>
        internal void ResetToFirstRowBlock()
        {
            CurrentBlock = CurrentBlockNotSet;
        }

        /// <summary>
        /// Computes a vector of maximum wide cell "hard" minimums stored in the first spanned column.
        /// </summary>
        /// <remarks>
        /// This is needed to imitate some "special" MS Word behavior when resizing an auto-fit table.
        /// See comments in auto fit grid resizer.
        /// </remarks>
        internal int[] GetWideCellHardMinimums()
        {
            int[] wideCellMins = new int[Grid.Count];

            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];
                int column = row.Before.GridSpan;
                foreach (CellSpan cell in row.Cells)
                {
                    if ((cell.GridSpan > 1) && (wideCellMins[column] < cell.Minimum))
                        wideCellMins[column] = cell.Minimum;

                    column += cell.GridSpan;
                }
            }

            return wideCellMins;
        }

        /// <summary>
        /// Merges narrow grid columns in the original tblGrid data.
        /// </summary>
        /// <remarks>
        /// This is done before any other grid construction steps.
        /// </remarks>
        private void MergeNarrowOriginalGridColumns()
        {
            // Some special handling for rare cases when grid before row is wider than any previous row.
            int maxRowSpanCount = RemoveExtraGridBeforeRows();

            // Prepare a list where columns that cannot be merged with the next column are marked.
            List<GridCellEnd> gridCellMatch = new List<GridCellEnd>(Rows[0].SpanCount);

            const int UnlimitedColumnCount = int.MaxValue;
            int nextRowBlockStart = DetectNextBlock(UnlimitedColumnCount, 0, gridCellMatch);
            Debug.Assert(nextRowBlockStart == Rows.Count);

            // Add missing columns to "original grid" values.
            int extraGrid = maxRowSpanCount - GridFromDocumentOriginal.Count;
            for (; extraGrid > 0; --extraGrid)
            {
                // Add the default width (360 twips).
                GridFromDocumentOriginal.Add(TableGridMetrics.DefaultFixedColumnWidthTwips);
                // For the case when the number of columns exceeds 64, the logic is confirmed by TestIntersectionVsGrid64columns and other tests.
            }

            if (extraGrid < 0)
            {
                // TestJira14978: A grid with extra columns is in the document.
                GridFromDocumentOriginal.RemoveRange(maxRowSpanCount, -extraGrid);
            }

            // Merge columns not matching any cell end, or narrow cell end columns.
            MergeNarrowColumns(0, Rows.Count, GridFromDocumentOriginal, gridCellMatch);
        }

        /// <summary>
        /// Implements merging narrow grid columns and updating row/cell data.
        /// </summary>
        /// <remarks>
        /// This is used both before and during grid construction.
        /// </remarks>
        private void MergeNarrowColumns(int rowBlockStart, int rowBlockEnd,
            IntList gridFromDocument, List<GridCellEnd> gridCellMatch)
        {
            for (int column = 0; column < (gridFromDocument.Count - 1);)
            {
                int tblGridColumnWidth = gridFromDocument[column];
                bool isNarrowColumn = (tblGridColumnWidth <= TableGridMetrics.MinimalWidthTwips);
                GridCellEnd columnCellMatch = gridCellMatch[column];
                bool mergeColumn = (columnCellMatch == GridCellEnd.None) ||
                    (isNarrowColumn && (columnCellMatch != GridCellEnd.Single));
                if (mergeColumn)
                {
                    UpdateCellSpansOnColumnRemoval(rowBlockStart, rowBlockEnd, column, gridCellMatch);

                    // Merge the grid columns.
                    int nextColumn = column + 1;
                    if (nextColumn < gridFromDocument.Count)
                    {
                        gridFromDocument[nextColumn] += gridFromDocument[column];
                    }
                    gridFromDocument.RemoveAt(column);

                    // Remove from the locked columns list;
                    gridCellMatch.RemoveAt(column);

                    TableGridDebugLogger.DebugWriteLine(string.Format("Narrow column index {0} merged with the next column.", column));
                }
                else
                {
                    ++column;
                }
            }
        }

        /// <summary>
        /// Detects the next row block and prepares for further steps of grid construction.
        /// </summary>
        /// <remarks>
        /// For tables having more than 64 columns, the rows are separated into "row blocks" having no more than 64 columns.
        /// As a single row cannot have more than 63 cells, it is always possible to define such block with a least one row.
        /// The extra columns in the row block are merged and a block grid is constructed as if it were a separate table.
        /// </remarks>
        private void GetNextRowBlock()
        {
            bool isSameBlock = (RowBlockStart < RowBlockNext);
            if (isSameBlock)
                return;

            // Prepare a list where columns that cannot be merged with the next column are marked.
            List<GridCellEnd> gridCellMatch = new List<GridCellEnd>(Rows[RowBlockStart].SpanCount);
            int rowBlockNext = DetectNextBlock(ColumnCountLimit64, RowBlockStart, gridCellMatch);
            // Remember the end of the current row block.
            NextBlockStartRows[CurrentBlock] = rowBlockNext;

            if (rowBlockNext < Rows.Count || RowBlockStart > 0)
            {
                TableGridDebugLogger.DebugWriteLine(
                    string.Format("### Row block {0} - {1} separated because of more than {2} columns in the grid.",
                        RowBlockStart, rowBlockNext - 1, ColumnCountLimit64));
                // TestJira16756Full and TestJira13686 have tables with hundreds of columns.
                // The logic below appears to break them. They work perfectly without merging the narrow columns.
                // Keep the current logic for such tables for now.
            }

            ResetVMergeForRowBlockStart(Rows[RowBlockStart]);

            int extraGrid = GridFromDocumentBlock.Count - gridCellMatch.Count;
            // Grid column count cannot be less than the row block column count:
            // the missing columns are added when handling the original grid before splitting the table into row blocks.
            Debug.Assert(extraGrid >= 0);
            if (extraGrid > 0)
            {
                // Original grid may be wider than the number of columns in the current block: Test64columnsNested().
                // Remove extra columns.
                GridFromDocumentBlock.RemoveRange(gridCellMatch.Count, extraGrid);
            }

            Debug.Assert(RowBlockStart < RowBlockNext);

            // Merge columns not matching any cell end, or narrow cell end columns.
            MergeNarrowColumns(RowBlockStart, RowBlockNext, GridFromDocumentBlock, gridCellMatch);
        }

        /// <summary>
        /// Goes through the list of rows and marks and finds the end of the next row block.
        /// </summary>
        /// <remarks>
        /// The method also updates <paramref name="gridCellMatch"/> so that extra column in the detected block can be removed.
        /// </remarks>
        /// <returns>Id of the row after the detected block.</returns>
        private int DetectNextBlock(int maxBlockColumnCount, int rowBlockStart, List<GridCellEnd> gridCellMatch)
        {
            IntList newCellEnds = new IntList(gridCellMatch.Count);
            IntList newSingleCells = new IntList(gridCellMatch.Count);

            // I think it will be simpler after "new" construction is used for auto-fit as well.
            int blockColumnCount = 0;
            int rowIdx;
            for (rowIdx = rowBlockStart; rowIdx < Rows.Count; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];
                newCellEnds.Clear();
                newSingleCells.Clear();

                // Add missing columns to the cell end list.
                for (int extraSpan = row.SpanCount - gridCellMatch.Count; extraSpan > 0; --extraSpan)
                    gridCellMatch.Add(GridCellEnd.None);

                // Process gridBefore.
                if (row.Before.GridSpan > 0)
                {
                    int gridBeforeEndColumn = row.Before.GridSpan - 1;
                    if (gridCellMatch[gridBeforeEndColumn] == GridCellEnd.None)
                    {
                        newCellEnds.Add(gridBeforeEndColumn);
                        // Even with gridBefore = 1 the first column may still be merged (removing the grid before).
                        // Do not set single-cell column flag.
                    }
                }

                // Process cells.
                int cellStartColumn = row.Before.GridSpan;
                foreach (CellSpan cell in row.Cells)
                {
                    int cellSpan = cell.GridSpan;

                    if ((cellSpan == 1) && (gridCellMatch[cellStartColumn] != GridCellEnd.Single))
                        newSingleCells.Add(cellStartColumn);

                    int cellEndColumn = cellStartColumn + cell.GridSpan - 1;
                    if (gridCellMatch[cellEndColumn] == GridCellEnd.None)
                        newCellEnds.Add(cellEndColumn);

                    cellStartColumn += cellSpan;
                }

                if (blockColumnCount + newCellEnds.Count > maxBlockColumnCount)
                {
                    // The current row makes more than the maximum number of columns.
                    // Need to process rows before that first.

                    // The block cannot be empty as an individual row cannot have more than 63 columns.
                    // So at least one row should be in the block.
                    if ((blockColumnCount <= 0) || (rowIdx <= rowBlockStart))
                    {
                        // This should not happen.
                        Debug.Fail("Unexpected: row block is empty.");
                        IsUnsupportedConditionDetected = true;
                    }

                    break;
                }

                // Mark the new columns after it is known that the row is included in the block.
                for (int column = 0; column < newCellEnds.Count; ++column)
                    gridCellMatch[newCellEnds[column]] = GridCellEnd.Wide;
                for (int column = 0; column < newSingleCells.Count; ++column)
                    gridCellMatch[newSingleCells[column]] = GridCellEnd.Single;

                blockColumnCount += newCellEnds.Count;
            }

            int nextRowBlockStart = rowIdx;
            return nextRowBlockStart;
        }

        /// <summary>
        /// Resets vertical cell merges for the given row.
        /// </summary>
        /// <remarks>
        /// The code was introduced for tables with more than 64 columns.
        /// Row blocks can split the table across vertical merges.
        /// In that case vertically merged cell properties in the first row of the block are taken into account.
        /// The logic is also applicable to the first row of a regular table:
        /// cells marked as vertical merge continuation in the first row should be treated as not-merged cells.
        /// </remarks>
        private void ResetVMergeForRowBlockStart(RowSpan firstBlockRow)
        {
            foreach (CellSpan cell in firstBlockRow.Cells)
                cell.IsVerticallyMerged = false;
            // Even if the cell is no longer treated as vertically merged,
            // merge attributes remain unchanged on saving to docx: TestVmerge64columnsA().
            // It actually makes sense as they are still treated as merged cell
            // if they are still aligned after combining the split rows back into a single table: TestVmerge64columnsB().
            // So v-merged is reset inside grid construction only. The change is not propagated to the model.
            // Layout has some logic that determines if v-merged cells are actually aligned, too.
        }

        /// <summary>
        /// Updates the rows with grid before wider than any previous row span count.
        /// </summary>
        private int RemoveExtraGridBeforeRows()
        {
            int maxRowSpanCount = 0;
            for (int rowIdx = 0; rowIdx < Rows.Count; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];

                Debug.Assert(row.Cells.Count > 0);
                if (row.Cells.Count <= 0)
                    continue;

                // It appears that MS Word 2019 behavior for some cases in TestWidthBeforeAutoB changed
                // at some point between Nov'23 when the test was added and Nov'24:
                // In MS Word 2019 the result no longer depends on the row  order.
                // MS Word 2013 still demonstrates the original behavior on my machine.
                // I'm keeping the original behavior for now.
                // To imitate the new 2019 logic, grid before should not be removed from the first row(s) here:
                if (row.Before.GridSpan > System.Math.Max(maxRowSpanCount, GridFromDocumentOriginal.Count))
                {
                    // gridBefore specifies more columns than exists on the grid constructed from the previous rows
                    // or grid columns in tblGrid element.
                    // According to the specs, the first cell should start from the first column (gridBefore is removed).
                    int gridBefore = row.Before.GridSpan;
                    row.Before.GridSpan = 0;
                    row.Before.PreferredWidth = PreferredWidth.Auto;

                    // Just expand the first cell grid span. TestBeforeOverflowWeird.
                    CellSpan firstCell = row.Cells[0];
                    firstCell.GridSpan += gridBefore;
                    // Row span remains the same.

                    // There was some code here that handled the situation in a different way, but eventually
                    // it turned out that the above approach seems to work for all cases.
                    // Code removed when working on gridBefore in Dec'23.
                }
                maxRowSpanCount = System.Math.Max(maxRowSpanCount, row.SpanCount);
            }

            return maxRowSpanCount;
        }

        /// <summary>
        /// Update rows and cell spans after grid column was removed.
        /// </summary>
        /// <remarks>
        /// The logic is slightly different depending on whether <paramref name="gridCellMatch"/> is specified.
        /// It is used before grid construction only. Different ways of tracking grid column - row cell match are used
        /// depending on this parameter.
        /// </remarks>
        private void UpdateCellSpansOnColumnRemoval(int rowBlockStart, int rowBlockEnd, int removedIdx, List<GridCellEnd> gridCellMatch)
        {
            bool isBeforeGridConstruction = (gridCellMatch != null);

            for (int rowIdx = rowBlockStart; rowIdx < rowBlockEnd; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];
                if (row.Before.GridSpan > removedIdx)
                {
                    int newGridBefore = row.Before.GridSpan - 1;
                    Debug.Assert(newGridBefore >= 0);
                    // Setting new grid before to zero might be legitimate, see comments in UpdateBeforeSpanCount() below.

                    row.UpdateBeforeSpanCount(newGridBefore);
                    // The method updates row.SpanCount as well.

                    continue;
                }

                // No action is required for row.After before grid construction.
                // Row.After is constructed at a later stage, no need to update it before grid construction.
                if (!isBeforeGridConstruction && (row.SpanCount <= removedIdx))
                {
                    // Row cells end before the removed column. Row should have row after.
                    if (row.After.GridSpan > 0)
                    {
                        row.After.GridSpan--;
                        // It is possible that row.After grid span becomes zero.
                        // Remove widthAfter in that case. TestWidthAfterSpan2AutoB.
                        if (row.After.GridSpan == 0)
                            row.After.PreferredWidth = CellSpan.NoWidth;
                    }
                    else
                    {
                        Debug.Assert(IsUnsupportedConditionDetected,
                            "Unexpected row grid after value during column removal (insufficient span count).");
                        // Test21613 triggers the condition, but it is a jagged table with percent widths.
                        // Leave it be for now.
                    }
                    continue;
                }

                int rowGridIdx = row.Before.GridSpan;
                foreach (CellSpan cell in row.Cells)
                {
                    if (rowGridIdx + cell.GridSpan > removedIdx)
                    {
                        // This cell spans the column being removed.
                        // Decrease cell and row span count.
                        cell.GridSpan--;
                        row.SpanCount--;

                        // TestColumnRemovalGridSpan0 shows a case when MS Word actually saves grid span 0 for a v-merged cell.
                        // In another table in the same test grid span becomes 0 in AW for a cell that is not 0 in MS Word however.
                        if (cell.GridSpan <= 0)
                        {
                            TableGridDebugLogger.DebugWriteLine("Cell grid span became 0 after column removal; grid calculation failed.");
                            IsUnsupportedConditionDetected = true;
                        }

                        if (cell.GridSpan == 1)
                        {
                            // Cell became a single-column cell, handling  depends on the context.
                            int remainingGridColumnIdx = (rowGridIdx < removedIdx)
                                ? rowGridIdx
                                : removedIdx + 1;

                            if (isBeforeGridConstruction)
                            {
                                // Updating before grid construction. Mark that the matching grid column is now not removable.
                                gridCellMatch[remainingGridColumnIdx] = GridCellEnd.Single;
                            }
                            else
                            {
                                // Cell became a single-column cell, grid-cell match needs updating.
                                TableGridColumn remainingGridColumn = Grid[remainingGridColumnIdx];

                                // Column metrics may need updating from cell as well.
                                remainingGridColumn.Metrics.Update(cell);
                                remainingGridColumn.SingleCellMatch.Update(cell.PreferredWidth);
                            }
                        }
                        break;
                    }
                    rowGridIdx += cell.GridSpan;
                } // loop by cells
            } // loop by rows
        }

        /// <summary>
        /// Goes through the table and updates the grid column properties from single-column cells.
        /// </summary>
        private void ConstructCellSpans()
        {
            // Initialize the new structures for wide cell handling.
            GridColumnTypes.Clear();
            EndingCellPreferredMaxList.Clear();

            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];

                UpdateGridForGridBeforeSpan(row.Before);

                int column = row.Before.GridSpan;

                foreach (CellSpan cell in row.Cells)
                {
                    bool setFakeAutoWidthForFixedLayout = !IsAutoFit &&
                        !cell.PreferredWidth.IsPercent &&
                        !cell.PreferredWidth.IsFixed;

                    if (setFakeAutoWidthForFixedLayout)
                        SetAutoCellPreferred(column, cell, row);

                    // Add spans to the grid.
                    UpdateGridForCell(column, cell);

                    column += cell.GridSpan;
                }

                Grid.DebugWrite(
                    string.Format("Row #{0,2} processed: span count: {1, 2}, cell count: {2, 2}\r\n" + new string(' ', 19) +
                                  "gridBefore: {3}\r\n" + new string(' ', 19) +
                                  "gridAfter:  {4}",
                        rowIdx, row.SpanCount, row.Cells.Count, row.Before, row.After));
            }

            // Run some checks for width before and width after handling.
            // Also, the conditions computed here are needed to imitate some weird MS Word behavior
            // that may use last encountered width before/after value in a jagged table.
            bool firstColumnHasPreferredFromCells = PrepareWidthBefore();
            bool lastColumnHasPreferredFromCells = PrepareWidthAfter();

            // Remember cell preferred data so that MS Word logic for wide cell preferred can be imitated.
            // It is important that width before and width after are also processed in the row order: TestWidthAfterOrder.
            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];

                UpdateCellEndsFromRowBefore(row.Before, firstColumnHasPreferredFromCells);

                int endColumn = row.Before.GridSpan - 1;
                foreach (CellSpan cell in row.Cells)
                {
                    // With TestColumnRemovalGridSpan0, the calculator does not exit after setting the failed state right away.
                    Debug.Assert(cell.GridSpan > 0 || IsUnsupportedConditionDetected);

                    // For fixed layout, "auto" cell width is set in the previous loop.
                    Debug.Assert(cell.PreferredWidth.IsPercent || cell.PreferredWidth.IsFixed || IsAutoFit);

                    endColumn += cell.GridSpan;

                    if (cell.IsVerticallyMerged)
                        continue;

                    // Imitate MS Word logic for setting column preferred type.
                    RememberCellEndMaxPreferred(endColumn, cell);
                }

                UpdateCellEndsForRowAfter(row.After, lastColumnHasPreferredFromCells);
            }
        }

        private void UpdateGridForGridBeforeSpan(CellSpan gridBefore)
        {
            UpdateGridForCellSpan(0, gridBefore, false);
        }

        private void UpdateGridForCell(int column, CellSpan cell)
        {
            UpdateGridForCellSpan(column, cell, true);
        }

        /// <summary>
        /// Updates the grid for the given cell span starting at the specified column.
        /// </summary>
        private void UpdateGridForCellSpan(int column, CellSpan cell, bool isTableCell)
        {
            int spansToAdd = column + cell.GridSpan - Grid.Count;
            spansToAdd = System.Math.Max(0, spansToAdd);

            AddSpans(spansToAdd);

            if (isTableCell && (cell.GridSpan == 1))
            {
                // Update the grid column for the current cell.
                UpdateColumn(cell, column);
            }
            // Wide cells spanning more than one column are processed later in the column order.
            // Spans from gridBefore are also processed later after all cells in all rows are processed.
        }

        /// <summary>
        /// Adds the given number of spans with a zero width to the grid and respective null elements to wide cells list.
        /// </summary>
        private void AddSpans(int spanCount)
        {
            for (int spansToAdd = spanCount; spansToAdd > 0; --spansToAdd)
            {
                TableGridColumn gridColumn = new TableGridColumn();
                Grid.Add(gridColumn);

                GridColumnTypes.Add(GridCellMatchType.None);
                EndingCellPreferredMaxList.Add(new int[Grid.Count]);
            }
        }

        /// <summary>
        /// Updates the grid column width from the given cell properties.
        /// </summary>
        /// <remarks>
        /// Content min/max and preferred width are increased if the cell specifies greater values.
        /// </remarks>
        private void UpdateColumn(CellSpan cell, int columnIndex)
        {
            // Wide cells are updated differently.
            Debug.Assert(cell.GridSpan == 1);
            if (cell.GridSpan != 1)
                return;

            TableGridColumn gridColumn = Grid[columnIndex];

            bool columnHasCellsWithPreferrreWidthInPreviousRows = gridColumn.SingleCellMatch.IsSet(GridCellMatchType.Twip);

            // There is a Test16089 where updating metrics from a v-merged cell fixes the calculation.
            // However it breaks other tests, so the case in 16089 must be under some special condition.
            if (cell.IsVerticallyMerged)
                return;
            // Test24974() shows that cell match should not be updated from a v-merged cell.

            // Mark that the column matches a cell.
            gridColumn.SingleCellMatch.Update(cell.PreferredWidth);

            gridColumn.Metrics.Update(cell);

            switch (cell.PreferredWidth.Type)
            {
                case PreferredWidthType.Points:
                    // A crude fix for v-merged cells:
                    if (cell.PreferredWidth.ValueTwips > 0)
                    {
                        gridColumn.Twips = System.Math.Max(gridColumn.Twips, cell.PreferredWidth.ValueRaw);
                        if (!UseWord2003Rules && !columnHasCellsWithPreferrreWidthInPreviousRows)
                        {
                            // Forget content max on encountering a cell with preferred width specified.
                            gridColumn.Metrics.ContentMaximum = gridColumn.ContentMinimum;
                            // Percent cells order does not affect content maximum reset: TestTcwAutoMaxGtPreferredMix.
                        }
                    }
                    break;
                case PreferredWidthType.Percent:
                    gridColumn.Percent = System.Math.Max(gridColumn.Percent, cell.PreferredWidth.ValueRaw);
                    break;
                case PreferredWidthType.Auto:
                    if (IsAutoFit)
                    {
                        // Treat content maximum as preferred if there were cells with preferred width before this one in the same column.
                        if (columnHasCellsWithPreferrreWidthInPreviousRows)
                            gridColumn.Twips = System.Math.Max(gridColumn.Twips, cell.ContentMaximum);

                        if (UseWord2003Rules || !columnHasCellsWithPreferrreWidthInPreviousRows)
                        {
                            // Always update content maximum in 2003 mode.
                            // In later modes, update content maximum only in case there were no cells with fixed preferred width before (TestTcwAutoMaxGtPreferredMix).
                            gridColumn.Metrics.ContentMaximum = System.Math.Max(gridColumn.ContentMaximum, cell.Metrics.ContentMaximum);
                        }
                    }
                    else
                    {
                        // There is no such thing as "auto" cell width in fixed layout.
                        // It is actually replaced with a fixed width taken from the grid of from the defaults.
                        Debug.Fail("Unexpected auto cell in fixed layout.");
                    }

                    break;
                default:
                    throw new InvalidOperationException("Unexpected cell width type.");
            }
        }

        /// <summary>
        /// Gets the width of a column to represent the given number of spans.
        /// </summary>
        /// <remarks>
        /// The width is taken from grid in the document, depending on useGridFromDocument parameter.
        /// If there is no grid, ColumnWidth.DefaultFixedSpanWidthTwips is used.
        /// </remarks>
        private int GetMissingSpanWidth(int columnIndex, int spanCount)
        {
            int tblGridSpanCount = 0;
            int tblGridRawWidth = 0;

            if (GridFromDocumentBlock != null)
            {
                // From grid column.
                for (int gridColumnIndex = columnIndex;
                     (gridColumnIndex < GridFromDocumentBlock.Count) && (tblGridSpanCount < spanCount);
                     gridColumnIndex++)
                {
                    tblGridRawWidth += GridFromDocumentBlock[gridColumnIndex];
                    tblGridSpanCount++;
                }
            }

            // What can be taken from grid, is taken from grid.
            // The default width is counted for the rest of spanCount.
            int newSpanWidth = tblGridRawWidth +
                TableGridMetrics.DefaultFixedColumnWidthTwips * (spanCount - tblGridSpanCount);

            // The width from grid is used as preferred width. It can be less than minimum cell width.
            // It is resolved later.
            return newSpanWidth;
        }

        /// <summary>
        /// Adds the given cell data to the appropriate lists of wide cell preferred widths.
        /// </summary>
        private void RememberCellEndMaxPreferred(int endColumn, CellSpan cell)
        {
            Debug.Assert(cell.GridSpan > 0);

            // Do not treat zero pct cells as Auto (fixes Test19871auto).
            if (IsAutoFit && cell.PreferredWidth.IsAuto && (cell.PreferredWidth.Type != PreferredWidthType.Percent))
            {
                // Set end column type to auto to track that there is an auto cell ending in the column.
                if (GridColumnTypes[endColumn] == GridCellMatchType.None)
                    GridColumnTypes[endColumn] = GridCellMatchType.Auto;
                return;
            }

            GridCellMatchType currentColumnType = GridColumnTypes[endColumn];
            // For fixed layout, treat "Auto" preferred with as twips.
            GridCellMatchType cellType = cell.PreferredWidth.IsPercent
                ? GridCellMatchType.Percent
                : GridCellMatchType.Twip;

            bool skipTwipCellsForPctColumns = (cellType != GridCellMatchType.Percent) &&
              (currentColumnType == GridCellMatchType.Percent);

            if (skipTwipCellsForPctColumns)
                return;

            int[] lastColumnMaxPrefs = EndingCellPreferredMaxList[endColumn];
            int cellPreferredRaw = cell.PreferredWidth.ValueRaw;
            int gridSpanIdx = cell.GridSpan - 1;

            if (cellType == currentColumnType)
            {
                int currentMaxPreferred = lastColumnMaxPrefs[gridSpanIdx];
                if (currentMaxPreferred < cellPreferredRaw)
                    lastColumnMaxPrefs[gridSpanIdx] = cellPreferredRaw;
            }
            else
            {
                // In case of preferred type change from twips to pct this will overwrite earlier stored max preferred.
                lastColumnMaxPrefs[gridSpanIdx] = cellPreferredRaw;
                // But it will only do that for the current grid span.
                // For other grid spans a value set earlier from twips may remain in place.
                // This appears to be an MS Word glitch that we must imitate.

                // Set the column type.
                GridColumnTypes[endColumn] = cellType;
            }
        }

        /// <summary>
        /// Goes through the rows and removes invalid widthBefore values.
        /// </summary>
        /// <returns>A boolean value indicating if there are cells with preferred width other than Auto
        /// spanning the first column.</returns>
        private bool PrepareWidthBefore()
        {
            LastWidthBeforeInFirstColumn = null;
            bool firstColumnHasPreferredFromCells = false;
            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];

                // Forget invalid width before values.
                if (row.Before.PreferredWidth.ValueRaw <= 0)
                    row.Before.PreferredWidth = CellSpan.NoWidth;

                if (row.Before.GridSpan <= 0)
                {
                    row.Before.PreferredWidth = CellSpan.NoWidth;

                    if ((row.FirstCell.PreferredWidth.Type != PreferredWidthType.Auto) &&
                        !row.FirstCell.IsVerticallyMerged)
                        firstColumnHasPreferredFromCells = true;
                }
            }

            return firstColumnHasPreferredFromCells;
        }


        /// <summary>
        /// Goes through the rows, removes invalid widthAfter values and updates missing values.
        /// </summary>
        /// <returns>A boolean value indicating if there are cells with preferred width other than Auto
        /// spanning the last column.</returns>
        private bool PrepareWidthAfter()
        {
            LastWidthAfterInLastColumn = null;
            bool lastColumnHasPreferredFromCells = false;
            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];

                // Update row gridAfter to the actual number of missing columns.
                row.After.GridSpan = Grid.Count - row.SpanCount;

                if (row.After.GridSpan <= 0)
                {
                    // Forget width after, if any was specified.
                    row.After.PreferredWidth = CellSpan.NoWidth;

                    if ((row.LastCell.PreferredWidth.Type != PreferredWidthType.Auto) &&
                        !row.LastCell.IsVerticallyMerged)
                        lastColumnHasPreferredFromCells = true;
                }
                else if (row.After.HasNoWidth)
                {
                    // It can happen that widthAfter is not set in presence of gridAfter. Same for widthBefore.
                    // This happens when the missing columns have auto width.
                    // Also happens in generated documents with inconsistent grid/width before/after.
                    // Use the default width from the grid.
                    int defaultWidth = GetMissingSpanWidth(row.SpanCount, row.After.GridSpan);
                    row.After.PreferredWidth = PreferredWidth.FromTwipsSafe(defaultWidth);
                    // The behavior is different from widthBefore.
                };
            }

            return lastColumnHasPreferredFromCells;
        }

        /// <summary>
        /// Updates <see cref="EndingCellPreferredMaxList"/> for the given width before value.
        /// </summary>
        private void UpdateCellEndsFromRowBefore(CellSpan rowBefore, bool firstColumnHasPreferredFromCells)
        {
            int lastHangingColumn = rowBefore.GridSpan - 1;

            if (rowBefore.GridSpan == 1)
            {
                if (firstColumnHasPreferredFromCells)
                {
                    // Add wBefore width for the first column.
                    CellSpan effectiveRowBefore = rowBefore;

                    if (rowBefore.HasNoWidth)
                    {
                        // Magic 5 twips are used for width before for some reason.
                        // A number of tests depend on it, see, TestBeforeDefault*, TestTableMergeBorders*, TestTableMergeCellMargins*.
                        PreferredWidth effectiveWidthBefore = PreferredWidth.FromTwipsSafe(TableGridMetrics.MinimalWidthTwips);
                        effectiveRowBefore = new CellSpan(1, effectiveWidthBefore);
                        // The same tests confirm that magic 5 twips are not saved to the model.
                    }

                    LastWidthBeforeInFirstColumn = effectiveRowBefore.PreferredWidth;
                    RememberCellEndMaxPreferred(lastHangingColumn, effectiveRowBefore);
                }
                else
                    rowBefore.PreferredWidth = CellSpan.NoWidth;
            }

            if ((rowBefore.GridSpan > 1) && !rowBefore.HasNoWidth)
            {
                // Remember the hanging width in the list of wide cells for processing later,
                // but not for widthBefore when there is no explicit width (TestWidthBeforeAuto).
                // It is different from widthAfter: TestWidthAfterAuto.
                RememberCellEndMaxPreferred(lastHangingColumn, rowBefore);
            }
        }

        /// <summary>
        /// Updates <see cref="EndingCellPreferredMaxList"/> for the given width after value.
        /// </summary>
        private void UpdateCellEndsForRowAfter(CellSpan rowAfter, bool lastColumnHasPreferredFromCells)
        {
            if (rowAfter.GridSpan == 0)
                return;

            if (!rowAfter.HasNoWidth && rowAfter.PreferredWidth.IsAuto)
            {
                // TestWidthAfterAutoValue and TestZeroWidthAfter
                // show that explicitly specified wAfter values with type Auto should be ignored,
                // regardless of the value.
                return;
                // This is different from the situation when wAfter is actually missing.
                // It can also cause table layout change on re-saving,
                // as MS Word removes wAfter with type Auto, and with it removed,
                // "default" width calculated from the grid becomes applicable.
            }

            int lastColumn = Grid.Count - 1;
            if (rowAfter.GridSpan == 1)
            {
                if (lastColumnHasPreferredFromCells)
                {
                    LastWidthAfterInLastColumn = rowAfter.PreferredWidth;
                    RememberCellEndMaxPreferred(lastColumn, rowAfter);
                }
                else
                {
                    rowAfter.PreferredWidth = CellSpan.NoWidth;
                    // It is not quite logical though that last column with auto cells may be updated from
                    // grid after spanning several rows, but not a single row. But MS Word seems to work that way.
                    // See TestWidthAfterAuto*().
                }
            }
            else
            {
                // It will not store width after in twips if the column is marked as pct already.
                RememberCellEndMaxPreferred(lastColumn, rowAfter);
                // So, encountering pct width after prevents using preferred width in twips from cells: TestWidthAfterPctG.
            }

            // After processing wide cells it may turn out that fixed width before/after is not applicable.
            // In that case row.After will be updated (width after removed) in RemoveWidthBeforeAfterAuto().
            // I'm not sure it is actually what MS Word does, but it seems to work so far.
        }

        /// <summary>
        /// Removes width before/after spanning columns with auto width.
        /// </summary>
        /// <remarks>
        /// Width before/after is removed if after processing wide cells it spans a column with preferred width in twips.
        /// It means that the width is not applicable and it has no effect on the grid.
        /// MS Word removes such widths.
        /// It appears that the logic is still needed after jagged tables with pct widths are supported.
        /// </remarks>
        private void RemoveWidthBeforeAfterAuto()
        {
            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];
                // Remove width after if a column with no twip width is spanned by grid after,
                // except the last column. See TestWidthAfterAuto():
                // In table 3, last column type is set to twip from a 2-column width after,
                // even though the column has auto cells only.
                // In a similar table 4, changing the previous column type to auto prevents such setting.
                // One-column grid after is a different story altogether: TestWidthAfterAutoA().
                RemoveWidthHangingAboveAutoColumns(row.After, row.SpanCount, Grid.Count - 1);

                // WidthBefore removal is checked by TestWidthBeforeAutoA.
                RemoveWidthHangingAboveAutoColumns(row.Before, 0, row.Before.GridSpan);
            }
        }

        /// <summary>
        /// Implements removing row widthBefore/widthAfter value.
        /// </summary>
        /// <remarks>
        /// Goes through the given range of columns and sets hanging width to Auto
        /// if a column without twip width specified is encountered.
        /// </remarks>
        private void RemoveWidthHangingAboveAutoColumns(CellSpan rowHanging, int firstColumn, int stopColumn)
        {
            for (int colIdx = firstColumn; colIdx < stopColumn; ++colIdx)
            {
                TableGridColumn column = Grid[colIdx];
                if (column.SingleCellMatch.IsAuto)
                {
                    // Remove the hanging width value.
                    rowHanging.PreferredWidth = PreferredWidth.Auto;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the preferred widths in the grid from cells wider than a single column.
        /// </summary>
        /// <remarks>
        /// The cells are processed in the order of the columns. So, first all cells ending at column 2 are processed,
        /// then all cells ending at column 3 and so on. It seems, MS Word works this way (TestTblW0GridSpanTcw).
        /// </remarks>
        private void UpdatePreferredFromWideCells()
        {
            // New logic based on the analysis of 2003 fixed layout handling.
            // It relies on cell preferred lists constructed in the order of rows. The order is important.

            bool isNewPreferredSetFromWideCell = false;
            // A temporal array of preferred widths computed from wide cells.
            int[] computedPrefs = new int[Grid.Count];
            // The computed values may not be applied to the grid depending on the conditions below.

            for (int column = 0; column < Grid.Count; ++column)
            {
                TableGridColumn gridColumn = Grid[column];

                // Skip columns not matching any cell end (that is, columns to be removed).
                GridCellMatchType endColumnType = GridColumnTypes[column];
                if (endColumnType == GridCellMatchType.None || endColumnType == GridCellMatchType.Auto)
                    continue;

                Debug.Assert(endColumnType == GridCellMatchType.Twip || endColumnType == GridCellMatchType.Percent);

                int[] maxWideCellsByGridSpans = EndingCellPreferredMaxList[column];
                int computedPreferred = maxWideCellsByGridSpans[0];
                // Computed preferred may account for 1-column widthBefore/widthAfter.
                // It will be used only on the condition tracked by isNewPreferredSetFromWideCell below.
                // (That is, in case a column gets a greater width as a remainder of a wide cell preferred,
                // or wide width before/width after).

                computedPrefs[column] = computedPreferred;

                // Go through max wide cell preferred and check if anything remains
                // after subtracting the values computed for the previous columns.
                for (int spanIndex = 1; spanIndex <= column; ++spanIndex)
                {
                    int gridSpan = spanIndex + 1;
                    int firstWideCellColumn = column - gridSpan + 1;
                    int wideCellRemainder = maxWideCellsByGridSpans[spanIndex];
                    for (int lookBackColumn = column - 1; lookBackColumn >= firstWideCellColumn; lookBackColumn--)
                    {
                        // Break if the current column type does not match the previous column type.
                        // The logic appears to be different for tables with 64 or more columns.
                        // Test11439 seems to confirm that "None" columns should prevent using wide cell remainder for regular (64 columns or less) tables.
                        // Wide cell preferred widths are handle during wide cell distribution at a later stage.
                        // Test16756* and TestJira7090 show that wide cell remainder should be assigned for tables with more than 64
                        // despite intersecting "None" columns not matching any cells.
                        // Wide cell distribution produces incorrect results for those tests.
                        GridCellMatchType lookBackType = GridColumnTypes[lookBackColumn];
                        bool lookBackTypeMatches = (Grid.Count <= ColumnCountLimit64)
                            ? (lookBackType == endColumnType)
                            : (lookBackType == endColumnType) || (lookBackType == GridCellMatchType.None);
                        if (!lookBackTypeMatches)
                        {
                            // Wide cell spans a column with a different type of preferred width.
                            // Remaining wide cell preferred cannot be computed.
                            wideCellRemainder = 0;
                            break;
                        }

                        int lookBackColumnPreferred = computedPrefs[lookBackColumn];
                        wideCellRemainder -= lookBackColumnPreferred;
                    }

                    // TestJira6913_customer: subtract spacing, for twip columns only.
                    if (endColumnType == GridCellMatchType.Twip)
                        wideCellRemainder -= HalfSpacing * 2 * (gridSpan - 1);

                    if (wideCellRemainder > computedPreferred)
                    {
                        // A new preferred width is computed from a wide cell remaining width.
                        computedPrefs[column] = wideCellRemainder;
                        computedPreferred = wideCellRemainder;
                        // This is a very important condition which imitates a strange twist in MS Word logic.
                        // Having a wide cell preferred remainder *in any column*
                        // triggers assigning of the computed values *for all columns*.
                        // As a result, the glitchy logic that does not clear wide cell preferred remembered for twip columns
                        // actually affects the grid when the value computed from mismatched preferred is applied.
                        isNewPreferredSetFromWideCell = true;

                        TableGridDebugLogger.DebugWriteLine(string.Format("A new preferred width {1} is assigned to column {0}", column, wideCellRemainder));
                    }
                } // End of look-back loop for wide cells.
            }

            if (isNewPreferredSetFromWideCell)
            {
                // Apply the computed values.
                SetWidthsFromeWideCells(computedPrefs);
            }
            else
            {
                // Set the first column width from the last width before having span 1.
                // Checked but TestWidthBeforePct*, TestWidthBeforeAuto[B,C] and some other tests.
                TableGridColumn column = Grid[0];
                SetColumnWidthFromWidthBeforeAfter(column, LastWidthBeforeInFirstColumn);

                // Set the last column width from the last width after having span 1.
                // Imitate some strange MS Word behavior from TestWidthAfterPct[C,D,E].
                column = Grid[Grid.Count - 1];
                SetColumnWidthFromWidthBeforeAfter(column, LastWidthAfterInLastColumn);
            }

            Grid.DebugWrite("After wide cells processing completed:");
        }

        /// <summary>
        /// Sets grid column width from wide cell remainders computed by the caller.
        /// </summary>
        private void SetWidthsFromeWideCells(int[] computedWidths)
        {
            Debug.Assert(computedWidths.Length == Grid.Count);

            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];

                // The computed values should not be assigned "on the fly" in the loop that computes them.
                // They are assigned all together here if there is a wide cell remainder greater than the current column width.
                int computedWidth = computedWidths[i];

                // Do not set 0 width.
                // TestTcwMixedC shows that column type should not be changed when the computed pct pref is 0.
                if (computedWidth <= 0)
                    continue;

                // This may change the column type if a percent value was not assigned to he column earlier.
                if (GridColumnTypes[i] == GridCellMatchType.Percent)
                {
                    column.Percent = computedWidth;
                }
                else
                {
                    if (column.SingleCellMatch.IsAuto && (computedWidth < column.ContentMaximum))
                    {
                        // This is the logic similar to using content maximum as preferred on encountering a twip column in UpdateColumn().
                        // Only here instead of a single-column cell with preferred twips, the preferred width comes from a wide cell.
                        TableGridDebugLogger.DebugWriteLine(string.Format("Computed preferred width is replaced with content max {1} for column {0}", i, computedWidth));
                        computedWidth = column.ContentMaximum;
                        // Forget the content maximum.
                        column.Metrics.ContentMaximum = column.ContentMinimum;
                        // This works the same way for both 2003 and later modes: TestTcwAutoMaxGtPreferredMixA.
                    }

                    column.Twips = computedWidth;
                    // TestTcwMixedK: Assigning a greater twip width from a wide cell prevents the column from treating as
                    // "intersection column" during resizing to tblw.
                    column.SingleCellMatch.Add(GridCellMatchType.Twip);
                    // So, after this assignment, the column not matching any single cell may have SingleCellMatch.Twip set.

                    // TODO rework the column types and make them closer to MS Word logic.
                }
            }
        }

        /// <summary>
        /// Sets first or last column width from a width before/after value.
        /// </summary>
        /// <remarks>
        /// The logic imitates some strange MS Word behavior that looks like a mistake.
        /// If no columns are found to assign a new width from a wide cell remainder (see the caller logic),
        /// MS Word appears to set the width of the first and last column from the last encountered 1-column width before/after.
        /// A weird behavior that took a long time to understand.
        /// </remarks>
        private void SetColumnWidthFromWidthBeforeAfter(TableGridColumn column, PreferredWidth hangingWidth)
        {
            if (hangingWidth == null)
                return;

            int width = hangingWidth.ValueRaw;

            if (hangingWidth.Type == PreferredWidthType.Percent)
            {
                if (column.Percent < width)
                    column.Percent = width;
            }
            else
            {
                if (column.Twips < width)
                    column.Twips = width;
                // TestWidthAfterTwipB: 5-twip minimum is set later, if needed.

                // Is needed for TestJira9485GridBefore:
                column.SingleCellMatch.Add(GridCellMatchType.Twip);
            }
        }

        /// <summary>
        /// Gets missing span width from grid in the document or defaults and sets it to the given cell.
        /// </summary>
        private int SetAutoCellPreferred(int firstCellColumn, CellSpan cell, RowSpan row)
        {
            Debug.Assert(cell.PreferredWidth.IsAuto);

            int cellPreferred;
            if (IsAutoFit)
            {
                // Do not use cell width from grid or default cell width for span calculation in an auto-fit table.
                cellPreferred = 0;
            }
            else
            {
                cellPreferred = GetMissingSpanWidth(firstCellColumn, cell.GridSpan);

                if (HasSpacing)
                {
                    // Deduct spacing width from the grid column width.
                    cellPreferred -= HalfSpacing * 2;

                    bool isLeftmostCell = firstCellColumn == row.Before.GridSpan;
                    if (isLeftmostCell)
                        cellPreferred -= HalfSpacing;

                    bool isRightmostCell = (firstCellColumn + cell.GridSpan == row.SpanCount);
                    if (isRightmostCell)
                        cellPreferred -= HalfSpacing;

                    cellPreferred = System.Math.Max(1, cellPreferred);
                }

                // In fixed table layout, MS Word seems to treat "auto" preferred widths 
                // as if they have preferred width in twips.
                // It saves them with preferred width, so they are not auto on re-saving.
                // This helps to get the same width after the missing grid is added during saving.

                cell.PreferredWidth = PreferredWidth.FromTwipsSafe(cellPreferred);
            }

            return cellPreferred;
        }

        private bool RemoveIntersectionsBeforeResizing()
        {
            if (UseWord2003Rules)
            {
                TableGridDebugLogger.DebugWriteLine("Unmatched span removal before resizing skipped for 2003 mode.");
                return false;
            }

            // Keep narrow intersection columns. They can become wider after resizing.
            bool checkResizedWidth = false;
            return RemoveIntersectionColumns(checkResizedWidth);

            // However, intersection columns having preferred widths assigned so far will still be removed
            // as in TestMinDistributionAffectsSpansTblw and some other tests.
        }

        /// <summary>
        /// Removes narrow intersection grid columns after resizing and reconstructs the grid after removal if needed.
        /// </summary>
        internal bool RemoveIntersectionsAfterResizing()
        {
            // Remove narrow intersection columns from the grid.
            bool checkResizedWidth = true;
            bool isColumnRemoved = RemoveIntersectionColumns(checkResizedWidth);
            // Removing narrow columns and calculation restart after that overwrites "grid on opening".
            // See comments in TestMinDistributionAffectsSpans() and TestWidthAfterSpan2AutoB().
            // So the condition to restart the calculation is not triggered in some cases to preserve "grid on opening".
            // See the implementation in RemoveIntersectionColumns().

            // WORDSNET-24196 no point to continue grid construction if unsupported condition is detected.
            // It leads to assertions/exceptions with the document from 24196 because of zero grid spans.
            if (IsUnsupportedConditionDetected)
                return false;

            // There is a very special document in Test22689(), where MS Word changes the grid for a generated table on re-saving.
            // The grid changes 3 times on each re-saving before it settles. Though the current logic produces the correct grid for
            // the first 2 re-savings, the grid spans do not match after re-saving 2. See the comments in the test.
            // So the logic is not perfect, but the problems should hopefully happen only with generated tables
            // when MS Word behavior is also unstable. The case should be rare.
            if (isColumnRemoved)
            {
                TableGridDebugLogger.DebugWriteLine("Grid calculation restarted because a narrow intersection column was removed after resizing");
                // Re-construct the grid after columns were removed.
                GetGridFromCells();
            }

            return isColumnRemoved;
        }

        /// <summary>
        /// Removes grid columns not corresponding to any cell from the grid.
        /// </summary>
        /// <remarks>
        /// Grid columns not having a corresponding cell can be the result of intersection of wider cells in different row:
        /// ----------------------
        /// | span 2     | span 1|
        /// ----------------------
        /// | span 1 |  span 2   |
        /// ----------------------
        /// For the table above, the grid will have 3 columns, and the middle column will not have a corresponding table cell.
        /// Such columns are sometimes removed by MS Word on re-saving the document.
        /// Specifically, they are removed if they are not assigned preferred width as a result of splitting a wider cell.
        /// See TestTblW0MinSplitA.
        /// This method implements the removal.
        /// </remarks>
        private bool RemoveIntersectionColumns(bool checkResizedWidth)
        {
            Grid.DebugWrite("Before unmatched span removal: ");

            // There was some logic to retain gridBefore spans for which no widthBefore was specified.
            // Eventually I decided to remove this logic as MS Word removes gridBefore on editing the document in such cases.
            // The removed logic can be found @e349df89106a (dmatv_832_TableMerge).

            // It seems, the logic for MS Word 2013 is no longer special.
            // Originally there was a special condition to keep narrow intersections in 2013 mode.
            // The condition was introduced in Feb-2018 when TestFixedVsAutoDifferentSpans* series was added.
            // But when I added more tests in Nov-2020, the logic in MS Word was matching compatibility mode already.
            // I decided to update the tests and remove the old condition as the same behavior for both modes looks more logical.

            // Do not keep narrow intersections after grid resizing.
            bool keepNarrowIntersection = !checkResizedWidth;
            // Cases for more than 64 columns are handled in a different way.
            Debug.Assert(Grid.Count <= ColumnCountLimit64);

            int gridCountBeforeRemoval = Grid.Count;
            int gridIdx = 0;
            int removedColumnWidthSum = 0;
            while (gridIdx < Grid.Count)
            {
                TableGridColumn gridColumn = Grid[gridIdx];

                bool columnIsRemovable = gridColumn.IsRemovable(keepNarrowIntersection, checkResizedWidth, HalfSpacing);
                if (columnIsRemovable)
                {
                    // The value is actually important for checking the final width (resized) width only.
                    removedColumnWidthSum += checkResizedWidth
                        ? gridColumn.Width
                        : gridColumn.TwipsOrContentMinimum;

                    RemoveGridColumn(gridIdx);

                    if (GridFromDocumentBlock != null && gridIdx < GridFromDocumentBlock.Count)
                    {
                        int removedDocumentGridWidth = GridFromDocumentBlock[gridIdx];
                        GridFromDocumentBlock.RemoveAt(gridIdx);
                        if (gridIdx < GridFromDocumentBlock.Count)
                            GridFromDocumentBlock[gridIdx] += removedDocumentGridWidth;
                        // TestTcwPctSplit0 shows a case where the last grid column is removed.
                    }

                    // The current gridIdx now points to the column next after the removed column.

                    // TestTcwMixedK seems to confirm that when 2003 mode is off, the narrow columns are removed in a single pass,
                    // but every other column is skipped. Perhaps MS Word tries to "merge" adjacent columns,
                    // or maybe it is just an error in an index update. The below condition reproduces the logic.
                    bool skipNextColumn = !UseWord2003Rules;
                    if (skipNextColumn)
                        gridIdx++;

                }
                else
                {
                    gridIdx++;
                }
            }

            return Grid.IsAutoFit
                ? (0 < removedColumnWidthSum)
                : (Grid.Count < gridCountBeforeRemoval);
            // Quitting without restarting when only zero-width columns are removed fixes Test22689*.
            // (Though it seems that MS Word removes the columns for Test22689 after resize, and AW does it before).
            // Anyway, the approach allows to emulate MS Word logic when the grid changes on saving to .docx.
            // The logic is demonstrated by TestMinDistributionAffectsSpans*
            // and some other tests referencing TestGridUtil.CheckGridChangeAfterLayoutUpdate().
            // With this approach, after the first layout update the grid matches MS Word layout "on opening".
            // With the next layout update the grid matches MS Word layout "on saving".

            // A somewhat similar issue happens with Test12991(). See details in the test.

            // The approach does not currently work for fixed table layouts
            // as the second table layout update always takes place before saving when document validator runs.
            // An example of fixed layout document which demonstrates "grid change on re-saving" is in TestLayoutTableGridFixed.TestTcwMixed().
            // If the above condition is changed, "grid on opening" would be present in AW model on document opening.
            // But as currently there is no way to save it to any format, I decided that it would be confusing
            // and preserved the logic that restarts grid calculation for fixed layout.
        }

        private void RemoveGridColumn(int removedIdx)
        {
            UpdateCellSpansOnColumnRemoval(RowBlockStart, RowBlockNext, removedIdx, null);

            // Remove the grid column.
            Grid.RemoveColumn(removedIdx);

            Grid.DebugWrite(string.Format("After grid column {0} removal: ", removedIdx));
        }

        /// <summary>
        /// Examines the wide cells and adjusts spanned grid columns to accommodate wide cell minimums and preferred widths.
        /// </summary>
        /// <remarks>
        /// Cells may not be narrower than a minimum width defined mostly by cell's border/margins for fixed layout,
        /// plus minimum "word" length for auto-fit.
        /// When a cell with a large minimum spans several columns, 
        /// its minimum may be greater than the sum of minimums in the spanned columns (derived from cells matching those columns):
        /// --------------------
        /// |       min 500    |
        /// --------------------
        /// | min 10 | min 100 |
        /// --------------------
        /// So the minimums of the spanned columns must be increased in such cases. It is not a trivial operation.
        /// In the example above, the sum of two minimums must be 500, but how exactly it is divided is tricky.
        /// The correct minimum is important, because it may define the cell width if the minimum is larger than the preferred width.
        /// It is also used when resizing the grid to match the table width.
        /// This method updates the minimums from the wide cells.
        /// Some similar logic may apply for distributing preferred widths if it is still needed after the previous grid construction steps.
        /// </remarks>
        private void DistributeWideCells()
        {
            for (int rowIdx = RowBlockStart; rowIdx < RowBlockNext; ++rowIdx)
            {
                RowSpan row = Rows[rowIdx];
                int column = row.Before.GridSpan;
                foreach (CellSpan cell in row.Cells)
                {
                    if (cell.GridSpan > 1 && !cell.IsVerticallyMerged)
                    {
                        DistributeWideCell(cell, column);
                    }
                    column += cell.GridSpan;
                }
            }
        }

        /// <summary>
        /// Implements updating spanned grid columns from wide cell properties.
        /// </summary>
        private void DistributeWideCell(CellSpan cell, int firstCellColumn)
        {
            int totalMinimum = 0;
            int totalTwipsOrMinimum = 0;
            int totalPercent = 0;
            int totalPercentMinimum = 0;
            int totalPercentTwips = 0;
            bool spansNonPctColumns = false;
            // This should be false for fixed layout.
            bool spansAutofitAutoOnly = IsAutoFit;

            for (int cellColumn = 0; cellColumn < cell.GridSpan; ++cellColumn)
            {
                int gridColumn = firstCellColumn + cellColumn;
                TableGridColumn column = Grid[gridColumn];
                int columnWidth = Grid.GetColumnWidth(gridColumn);
                int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                totalMinimum += columnMinimum;
                totalTwipsOrMinimum += System.Math.Max(columnMinimum, columnWidth);

                // TestTcwMixedC seems to confirm that pct columns having 0 percent and HasPercentPreferred set to true
                // are treated in the same way as twip columns without pct width during wide cell pct distribution.
                if (column.HasPreferredPercent)
                {
                    totalPercent += column.Percent;
                    totalPercentMinimum += columnMinimum;
                    totalPercentTwips += columnWidth;
                }
                else
                {
                    spansNonPctColumns = true;
                    if (column.Twips > 0)
                        spansAutofitAutoOnly = false;
                }
            }

            if (totalPercent > TableGridMetrics.HundredPercent)
                totalPercent = TableGridMetrics.HundredPercent;

            // Distribute wide cell width and metrics between the spanned columns.
            if (UseWord2003Rules)
            {
                DistributeWideCell2003(cell, firstCellColumn,
                    totalMinimum, totalTwipsOrMinimum,
                    totalPercentMinimum, totalPercentTwips,
                    totalPercent);
            }
            else
            {
                DistributeWideCell2003Off(cell, firstCellColumn,
                    totalMinimum, totalPercent,
                    spansNonPctColumns);
            }
        }

        private void DistributeWideCell2003Off(CellSpan wideCell, int firstCellColumn,
            int totalMinimum, int totalPercent,
            bool wideCellSpansNonPctColumns)
        {
            // TODO need to convert fixed layout tests created for specific cases like pct/twip mix to auto-fit.

            // The spanned columns may be assigned pct preferred values as a result of wide cell processing.
            totalPercent = SetPctPreferredFromWideCell2003Off(wideCell, firstCellColumn, totalPercent);
            // Do it even if there is no extra minimum (which is logical).

            int wideCellMinimum = GetCellMinimumForDistribution(wideCell);

            int extraMinimum = System.Math.Max(0, wideCellMinimum - totalMinimum);
            // TestTcwMixedF: MS Word does not distribute the minimum if the differences is less than 5 twips.
            bool isMinimumDistributionNeeded = (extraMinimum >= TableGridMetrics.MinimalWidthTwips);
            bool wideCellIsPercent = wideCell.PreferredWidth.IsPercent;

            if (isMinimumDistributionNeeded)
            {
                // If the wide cell has no pct, but spans pct columns, the minimum is distributed among the cells with pct preferred.
                bool spansNonPctColumns = !wideCellIsPercent && wideCellSpansNonPctColumns;
                int pctCalculatedMinSum = SetPctMinimumsFromWideCell2003Off(wideCell, firstCellColumn, totalPercent, spansNonPctColumns);

                // Distribute the minimum remaining after pct columns across non-pct columns.
                // TestTcwMixedF: Do it even though the difference may be less than 5 twips after distribution to pct cells.
                extraMinimum = SetAbsMinimumsFromWideCell2003Off(wideCell, firstCellColumn, pctCalculatedMinSum);
                // The same TestTcwMixedF for 2003 mode confirms that there is no "minimum extra width condition" for 2003.

                FixRounding(Grid, firstCellColumn, wideCell.GridSpan, extraMinimum, GridRoundingColumn.Minimum);
            }

            // Distribute wide cell preferred in twips, or content maximum, across the spanned columns.
            SetAbsWidthFromWideCell(wideCell, firstCellColumn);
        }

        /// <summary>
        /// Gets the width of spacing spanned by the specified cell span.
        /// </summary>
        /// <remarks>It is used for wide cells, spacing on the edges of the cell is not included.</remarks>
        private int GetSpannedSpacing(CellSpan cell)
        {
            return (cell.GridSpan - 1) * HalfSpacing * 2;
        }

        /// <summary>
        /// Gets cell minimum to use for wide cell distribution, taking spacing into account.
        /// </summary>
        private int GetCellMinimumForDistribution(CellSpan cell)
        {
            int cellMinimum = IsAutoFit
                ? cell.ContentMinimum
                : cell.Minimum;

            // Decrease the distributed wide cell minimum for spacing between the spanned cells.
            cellMinimum -= GetSpannedSpacing(cell);

            return cellMinimum;
        }

        /// <summary>
        /// Handles wide cell metrics distribution across the spanned cells in 2003 mode.
        /// </summary>
        private void DistributeWideCell2003(CellSpan wideCell, int firstCellColumn,
            int totalMinimum, int totalTwipsOrMinimum,
            int totalPercentMinimum, int totalPctTwips,
            int totalPercent)
        {
            // TODO need to convert fixed layout tests created for specific cases like pct/twip mix to auto-fit.

            int wideCellMinimum = GetCellMinimumForDistribution(wideCell);

            int wideMinimumExtra = System.Math.Max(0, wideCellMinimum - totalMinimum);
            int wideTwipExtra = 0;
            int widePctExtra = 0;
            int wideCellTwips = wideCellMinimum;
            if (wideCell.PreferredWidth.IsPercent)
            {
                widePctExtra = System.Math.Max(0, wideCell.PreferredWidth.ValueRaw - totalPercent);
            }
            else
            {
                int wideCellWidth = IsAutoFit && wideCell.PreferredWidth.IsAuto
                    ? System.Math.Max(wideCell.PreferredTwips, wideCell.ContentMaximum)
                    : wideCell.PreferredTwips;

                wideCellWidth -= GetSpannedSpacing(wideCell);

                // This is an important condition that fixes TestTcwMixedJ in a distribution loop below.
                // Wide cell preferred is used in a condition that determines the way of minimum distribution.
                // But if wide cell minimum is greater than preferred, it should be used instead.
                wideCellTwips = System.Math.Max(wideCellMinimum, wideCellWidth);
                wideTwipExtra = System.Math.Max(0, wideCellTwips - totalTwipsOrMinimum);
            }

            bool isTwipColumnSpanned = (totalMinimum > totalPercentMinimum);
            int totalPctForDistribution = isTwipColumnSpanned && !wideCell.PreferredWidth.IsPercent
                ? TableGridMetrics.HundredPercent
                : totalPercent;

            int wideTwipExtraForPctColumns = 0;
            int wideTwipExtraForTwipColumns = 0;
            int wideMinExtraForPctColumns = 0;
            int wideMinExtraForTwipColumns = 0;

            bool isPctColumnSpanned = totalPercentMinimum > 0;
            if (isPctColumnSpanned)
            {
                int pctRemainderForTwipColumns = totalPctForDistribution - totalPercent;
                Debug.Assert(isTwipColumnSpanned ? pctRemainderForTwipColumns >= 0 : pctRemainderForTwipColumns == 0);

                if (wideTwipExtra > 0)
                {
                    wideTwipExtraForPctColumns = (totalPctForDistribution > 0)
                        ? MathUtil.Divide(wideCellTwips * totalPercent, totalPctForDistribution)
                        : 0;

                    wideTwipExtraForPctColumns -= totalPctTwips;
                    if (wideTwipExtraForPctColumns < 0)
                        wideTwipExtraForPctColumns = 0;

                    wideTwipExtraForTwipColumns = (totalPctForDistribution > 0)
                        ? MathUtil.Divide(wideCellTwips * pctRemainderForTwipColumns, totalPctForDistribution)
                        : wideCellTwips;
                    wideTwipExtraForTwipColumns += totalPctTwips - totalTwipsOrMinimum;
                    // So, wideTwipExtraForPctColumns + wideTwipExtraForTwipColumns == wideCell.PreferredTwips - totalTwipsOrMinimum.
                    // However, the above will not be true in case wideTwipExtraForPctColumns was less than 0 originally.
                    // It can happen if pct columns are already assigned a greater width.
                    // In this case wideTwipExtraForTwipColumns may become greater than is actually needed.

                    if (wideTwipExtraForTwipColumns < 0)
                        wideTwipExtraForTwipColumns = 0;
                }

                if (wideMinimumExtra > 0)
                {
                    wideMinExtraForPctColumns = (totalPctForDistribution > 0)
                        ? MathUtil.Divide(wideCellMinimum * totalPercent, totalPctForDistribution)
                        : 0;
                    wideMinExtraForPctColumns -= totalPercentMinimum;
                    if (wideMinExtraForPctColumns < 0)
                        wideMinExtraForPctColumns = 0;

                    wideMinExtraForTwipColumns = (totalPctForDistribution > 0)
                        ? MathUtil.Divide(wideCellMinimum * pctRemainderForTwipColumns, totalPctForDistribution)
                        : wideCellMinimum;
                    wideMinExtraForTwipColumns += totalPercentMinimum - totalMinimum;
                    // So, wideMinExtraForPctColumns + wideMinExtraForTwipColumns == wideCellMinimum - totalMinimum.
                    // But similar to twip distribution above, this may not be true in case pct columns already have greater minimum.
                    // In that case wideMinExtraForTwipColumns may become greater than is actually needed.

                    if (wideMinExtraForTwipColumns < 0)
                        wideMinExtraForTwipColumns = 0;
                }
            }
            else
            {
                // No pct columns spanned.
                wideTwipExtraForTwipColumns = wideTwipExtra;
                wideMinExtraForTwipColumns = wideMinimumExtra;
            }

            // Distribute wide cell extras.
            int newMinimumSum = 0;
            int newTwipSum = 0;
            int newTwipSumOfPctColumns = 0;
            int nonPctColumnCount = 0;
            for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
            {
                int gridColumn = firstCellColumn + cellColumn;
                TableGridColumn column = Grid[gridColumn];
                int columnWidth = Grid.GetColumnWidth(gridColumn);
                int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                if (widePctExtra > 0)
                {
                    // Divide wide cell pct. All columns will have pct type after that, even if 0 pct is assigned.
                    int wideCellPercent = wideCell.PreferredWidth.ValueRaw;
                    totalPctForDistribution = wideCellPercent;
                    // Note that pct column type is not assigned to the spanned non-pct columns
                    // when wide cell preferred percent distribution is not needed.

                    wideMinExtraForPctColumns = wideMinimumExtra;

                    if (column.HasPreferredPercent)
                    {
                        // Apparently nothing to do.
                        // Strangely enough, MS Word seems to ignore distribution of extra pct preferred to pct columns: TestWidePctSplit.
                        // The case should be rare with regular documents saved by MS Word:
                        // normally, most columns have pct and wide cell remainder is assigned to the column at a previous stage.
                    }
                    else
                    {
                        // Set wide percent share for a non-percent column.
                        int widePctShare;

                        int twipColSum = totalTwipsOrMinimum - totalPctTwips;
                        if (twipColSum > 0)
                        {
                            widePctShare = MathUtil.Divide(columnWidth * widePctExtra, twipColSum);
                        }
                        else
                        {
                            Debug.Fail("Spanned column width sum 0 is not expected.");
                            // Currently , the width sum cannot be 0 as a minimum of 1 is assigned to all remaining columns.
                            // This is a stub that will assign a value in this case.
                            int dummyNonPctColumnCount = 1;
                            // If the logic changes and zero columns become possible, the actual of columns should be used here.

                            widePctShare = MathUtil.Divide(widePctExtra, dummyNonPctColumnCount);
                        }

                        // This will also set column HasPreferredPercent.
                        column.Percent = widePctShare;
                    }
                }

                // Distribute wide cell absolute preferred and minimum.
                int newTwips = 0;
                int newMin = 0;
                if (column.HasPreferredPercent)
                {
                    // Using totalPctForDistribution below ignores new column pct values that may have been assigned above.
                    // As it works correctly with the test documents it appears that MS Word works this way.
                    if (wideTwipExtraForPctColumns > 0)
                        newTwips = MathUtil.Divide(column.Percent * wideCellTwips, totalPctForDistribution);

                    if (wideMinExtraForPctColumns > 0)
                        newMin = MathUtil.Divide(column.Percent * wideCellMinimum, totalPctForDistribution);
                }
                else
                {
                    // Wide cell preferred in twips are not distributed to non-pct columns in this loop, only minimums.
                    // Wide cell preferred in twips is distributed only after the minimum distribution rounding correction below.
                    nonPctColumnCount++;
                    if (wideMinExtraForTwipColumns > 0)
                    {
                        int newMinimumShare;

                        int nonPctWidthSum = totalTwipsOrMinimum - totalPctTwips;

                        int totalNonPctMinimum = totalMinimum - totalPercentMinimum;

                        // This is an important condition which imitates the logic for all tests up to TestTcwMixedJ.
                        // The tricky thing is that wide twip extra may be actually calculated from wide twip minimum when it is greater.
                        if (wideTwipExtraForTwipColumns <= 0)
                        {
                            int nonPctAboveMinSum = nonPctWidthSum - totalNonPctMinimum;
                            if (nonPctAboveMinSum > 0)
                            {
                                // Proportionally to the width above minimum.
                                int columnAboveMin = columnWidth - columnMinimum;
                                newMinimumShare = MathUtil.Divide(wideMinExtraForTwipColumns * columnAboveMin, nonPctAboveMinSum);
                            }
                            else if (totalNonPctMinimum > 0)
                            {
                                // Proportionally to minimum sum.
                                newMinimumShare = MathUtil.Divide(wideMinExtraForTwipColumns * columnMinimum, totalNonPctMinimum);
                            }
                            else
                            {
                                Debug.Fail("Spanned column minimum sum 0 is not expected.");
                                // Currently , the width sum cannot be 0 as a minimum of 1 is assigned to all remaining columns.
                                // This is a stub that will assign a value in this case.
                                // If the logic changes and zero columns become possible, the actual of columns should be used here.
                                int dummyNonPctColumnCount = 1;
                                newMinimumShare = MathUtil.Divide(wideMinExtraForTwipColumns, dummyNonPctColumnCount);
                            }
                        }
                        else
                        {
                            // Proportionally to width.
                            newMinimumShare = MathUtil.Divide(wideMinExtraForTwipColumns * columnWidth, nonPctWidthSum);
                        }

                        newMin = columnMinimum + newMinimumShare;
                    }
                }

                if (newTwips > columnWidth)
                    column.Twips = newTwips;

                if (newMin > columnMinimum)
                {
                    columnMinimum = newMin;
                    if (IsAutoFit)
                    {
                        column.Metrics.ContentMinimum = newMin;
                        column.Metrics.ContentMaximum = System.Math.Max(newMin, column.ContentMaximum);
                    }
                    else
                        column.Metrics.Minimum = newMin;
                }

                columnWidth = Grid.GetColumnWidth(gridColumn);

                newMinimumSum += columnMinimum;
                newTwipSum += columnWidth;
                if (column.HasPreferredPercent)
                    newTwipSumOfPctColumns += columnWidth;
            }

            // Because of rounding, the sum of the new minimums may be less than the wide cell minimum.
            int undistributedMinimum = wideCellMinimum - newMinimumSum;
            if (undistributedMinimum > 0)
            {
                // TestMinDistributionRounding3: the difference is just assigned to the last spanned column.
                // TestTblWGridSpanMin3AutoA checks the logic for auto-fit.
                int lasSpannedColumnIdx = firstCellColumn + wideCell.GridSpan - 1;
                TableGridColumn lastSpannedColumn = Grid[lasSpannedColumnIdx];

                int previousWidth = Grid.GetColumnWidth(lasSpannedColumnIdx);

                if (IsAutoFit)
                {
                    lastSpannedColumn.Metrics.ContentMinimum += undistributedMinimum;
                    lastSpannedColumn.Metrics.ContentMaximum = System.Math.Max(lastSpannedColumn.ContentMinimum, lastSpannedColumn.ContentMaximum);
                }
                else
                    lastSpannedColumn.Metrics.Minimum += undistributedMinimum;

                int newLastColumnWidth = Grid.GetColumnWidth(lasSpannedColumnIdx);

                if (previousWidth < newLastColumnWidth)
                {
                    int correction = newLastColumnWidth - previousWidth;
                    newTwipSum += correction;
                    if (lastSpannedColumn.HasPreferredPercent)
                        newTwipSumOfPctColumns += correction;
                }
                // If the sum is greater than the wide cell minimum, no correction is done.
            }

            Grid.DebugWrite("After 2003 mode stage 1 of wide cell distribution (pct and minimums):");

            // Distribute wide cell preferred in twips to twip columns.
            // It seems, MS Word distributes wide cell preferred to non-pct columns only after the minimum correction above.
            wideTwipExtraForTwipColumns = wideCellTwips - newTwipSum;
            if (wideTwipExtraForTwipColumns > 0)
            {
                if (nonPctColumnCount > 0)
                {
                    int currentNonPctSum = newTwipSum - newTwipSumOfPctColumns;
                    int wideShareForNonPctColumns = wideCellTwips - newTwipSumOfPctColumns;

                    // With the current logic, a minimum of 1 twip is assigned to all columns at an earlier stage, so the sum cannot be 0.
                    Debug.Assert(currentNonPctSum > 0);
                    // If this changes, the actual column count should be used here.
                    int equalShare = wideShareForNonPctColumns / nonPctColumnCount;

                    newTwipSum = 0;
                    for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                    {
                        int gridColumn = firstCellColumn + cellColumn;
                        TableGridColumn column = Grid[gridColumn];
                        int columnWidth = Grid.GetColumnWidth(gridColumn);

                        int newWidth = columnWidth;
                        if (!column.HasPreferredPercent)
                        {
                            newWidth = (currentNonPctSum > 0)
                                ? MathUtil.Divide(wideShareForNonPctColumns * columnWidth, currentNonPctSum)
                                : equalShare;

                            if (newWidth > columnWidth)
                            {
                                bool updateContentMax = IsAutoFit && column.Twips <= 0;
                                if (updateContentMax)
                                    column.Metrics.ContentMaximum = newWidth;
                                else
                                    column.Twips = newWidth;
                            }
                        }

                        newTwipSum += newWidth;
                    }
                }

                // It can happen that the wide cell preferred is still not distributed because of rounding,
                // or, like in TestTcwMixedK, because all spanned columns have percent type (but 0 percent width).
                if (wideCellTwips > newTwipSum)
                {
                    // TestTcwMixedK: Just write the remainder to the last spanned column, regardless of the column type.
                    // TestJira16078 checks the logic for auto-fit.
                    int lastSpannedColumnIdx = firstCellColumn + wideCell.GridSpan - 1;
                    TableGridColumn lastSpannedColumn = Grid[lastSpannedColumnIdx];
                    int lastColumnWidth = Grid.GetColumnWidth(lastSpannedColumnIdx);

                    int newWidth = lastColumnWidth + wideCellTwips - newTwipSum;
                    bool updateContentMax = IsAutoFit && lastSpannedColumn.Twips <= 0;
                    if (updateContentMax)
                        lastSpannedColumn.Metrics.ContentMaximum = newWidth;
                    else
                        lastSpannedColumn.Twips = newWidth;
                }

                Grid.DebugWrite("After 2003 mode stage 2 of wide cell distribution (twips and content max):");
            }
        }

        /// <summary>
        /// Update grid column minimums from magical default "1" and "5" values.
        /// </summary>
        private void AdjustGridMinimumsFromDefaults()
        {
            Set1TwipMinimumsIn2003Mode();

            SetDocMinimumsForNotZeroColumns();
        }

        /// <summary>
        /// Goes through the grid and updates zero minimums to 1 twip in 2003 mode.
        /// </summary>
        private void Set1TwipMinimumsIn2003Mode()
        {
            if (!UseWord2003Rules)
                return;

            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];
                TableGridMetrics metrics = column.Metrics;
                int absoluteMinimum = 1;

                // Adding spacing here fixes calculation for some tables in Test20586a.
                // The spacing is only applied to "auto" column minimums for some reason.
                if (!column.HasPreferredPercent && !column.HasPreferredTwips)
                    absoluteMinimum += HalfSpacing * 2;
                // Removing the above condition breaks TestSpacingMinDistribution().

                metrics.Minimum = System.Math.Max(metrics.Minimum, absoluteMinimum);
                metrics.ContentMinimum = System.Math.Max(metrics.ContentMinimum, absoluteMinimum);
            }
            // It appears that in MS Word setting 1+spacing is actually done during wide cell distribution for 2003 mode.

            Grid.DebugWrite("After updating grid minimums to 1 in 2003 mode:");
        }

        /// <summary>
        /// Updates intersection column minimums to 5 twips.
        /// </summary>
        /// <remarks>
        /// Non-intersection columns (those matching a single-column cell) will have 5-twip minimum right away.
        /// For intersections, the logic is different for many-column tables.
        /// </remarks>
        private void SetDocMinimumsForNotZeroColumns()
        {
            // In TestJira7090, TestJira16756Full there are complex tables with more than 64 columns where columns less than 6 twips (even 1 twip) are preserved.
            // The tables, however, are handled by splitting them into row blocks having no more than 64 columns in each block.
            Debug.Assert(Grid.Count <= ColumnCountLimit64);

            for (int i = 0; i < Grid.Count; ++i)
            {
                TableGridColumn column = Grid[i];

                if (column.IsAuto)
                    continue;

                if (column.Minimum < TableGridMetrics.MinimalWidthTwips)
                    column.Metrics.Minimum = TableGridMetrics.MinimalWidthTwips;

                // TODO Consider doing it in metrics (it did not work straight away).
                if (IsAutoFit)
                {
                    if (column.ContentMinimum < column.Minimum)
                        column.Metrics.ContentMinimum = column.Minimum;

                    if (column.ContentMaximum < column.Minimum)
                        column.Metrics.ContentMaximum = column.Minimum;
                }
            }

            Grid.DebugWrite("After setting 5-twip column minimums for intersection columns:");
        }

        /// <summary>
        /// Implements wide cell minimum distribution when 2003 mode is turned off.
        /// </summary>
        /// <remarks>
        /// The main idea is to distribute proportionally, but reproduce an MS Word glitch.
        /// The approach is also different depending on the spanned cell preferred sum.
        /// </remarks>
        private int SetAbsMinimumsFromWideCell2003Off(CellSpan wideCell, int firstCellColumn, int pctCalculatedMinSum)
        {
            int twipColumnMinSum = 0;
            int twipColumnPrefSum = 0;
            int twipColumnCount = 0;
            int pctSum = 0;
            int pctColumnMinSum = 0;
            int pctColumnWidthSum = 0;
            // Re-calculate minimum sums after distribution to percent cells.
            for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
            {
                int gridColumn = firstCellColumn + cellColumn;
                TableGridColumn column = Grid[gridColumn];
                int columnWidth = Grid.GetColumnWidth(gridColumn);
                int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                if (column.Percent > 0)
                {
                    pctSum += column.Percent;
                    pctColumnMinSum += columnMinimum;
                    pctColumnWidthSum += columnWidth;
                }
                else
                {
                    ++twipColumnCount;
                    twipColumnMinSum += columnMinimum;
                    twipColumnPrefSum += columnWidth;
                    // This may pick up "obsolete" preferred twips from columns with type set to percent, but 0 percent width.
                    // Such columns may occur as a result of assigning pct type from a wide cell.
                    // It would seem that "old" twip preferred should have no effect, but
                    // TestTcwMixedG seems to confirm that MS Word still uses them in this case.
                }
            }

            int wideCellMinimum = GetCellMinimumForDistribution(wideCell);

            // A strange condition that replaces minimum with preferred sum if it is slightly less.
            // It seems to fix  the logic for TestTblWGridSpanMinAllAuto1.
            int magicFive = TableGridMetrics.MinimalWidthTwips;
            if ((twipColumnPrefSum < wideCellMinimum) && (wideCellMinimum < twipColumnPrefSum + magicFive))
                wideCellMinimum = twipColumnPrefSum;

            // This reproduces some strange MS Word logic for TestTcwMixedJ.
            // As the sum of computed pct column minimums may be less than the sum of the actual minimums,
            // more than needed may be distributed to twip columns.
            int wideMinimumForTwipColumns = wideCellMinimum - pctCalculatedMinSum;

            // Nothing to do if there is no twip columns.
            if (twipColumnCount == 0)
                return wideMinimumForTwipColumns;

            if (wideMinimumForTwipColumns <= twipColumnMinSum)
                return 0;

            int extraMinimum = 0;
            // Originally the condition was if (wideMinimumForTwipColumns <= twipColumnPrefSum), which is more logical.
            // Condition was changed for TestWidthAfterPct (tables 3, 4): it appears that MS Word still tries to distribute the minimum this way,
            // even if there is not enough space in twip columns. Strange logic, as nothing at all is distributed for those tables as a result.
            if (wideCellMinimum <= twipColumnPrefSum + pctColumnWidthSum)
            {
                // Do not try to distribute anything if there is nothing in all the cells (happens with TestWidthAfterPct (tables 3, 4)).
                int minPlusPrefSum = twipColumnMinSum + twipColumnPrefSum;
                if (minPlusPrefSum > 0)
                {
                    // The minimum will be distributed so that no column preferred width increases.
                    // It means that columns with a greater difference between minimum and preferred may get a greater share
                    // even though the columns are narrower than the other columns.
                    double minPlusPrefSumD = minPlusPrefSum;
                    double[] minShares = new double[wideCell.GridSpan];

                    // Calculate (cellMinimum + cellPreferred) / sum(cellMinimum + cellPreferred).
                    for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                    {
                        int gridColumn = firstCellColumn + cellColumn;
                        TableGridColumn column = Grid[gridColumn];
                        if (column.Percent > 0)
                            continue;

                        // Minimum width of 5 twips should be assigned to the intersection at earlier stages, if needed.
                        // There should be no need to handle it here.
                        // It seems, when this distribution logic applies, 5 twips is assigned to the intersection correctly: TestIntersectionMin().
                        // There are tests with a complex grid computed correctly when 5 twips are not assigned to the intersection: TestJira16756Full().

                        int columnWidth = Grid.GetColumnWidth(gridColumn);
                        int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                        int columnTwipPrefSum = columnMinimum + columnWidth;

                        minShares[cellColumn] = columnTwipPrefSum / minPlusPrefSumD;
                    }

                    double minShareSumLteTwips = 0d;
                    double minShareOverflowSum = 0d;
                    double extraMinimumD = wideMinimumForTwipColumns - twipColumnMinSum;
                    bool[] minShareOverflows = new bool[wideCell.GridSpan];

                    // Compute new column minimums by adding an extra minimum share.
                    // Handle minimums exceeding the preferred widths.
                    for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                    {
                        int gridColumn = firstCellColumn + cellColumn;
                        TableGridColumn column = Grid[gridColumn];
                        if (column.Percent > 0)
                            continue;

                        int columnWidth = Grid.GetColumnWidth(gridColumn);
                        int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                        double columnShare = minShares[cellColumn];
                        double computedMin = columnMinimum + columnShare * extraMinimumD;
                        if (computedMin <= columnWidth)
                        {
                            // Leave (min + pref)/(wideMin + widePref) as is.
                            minShareSumLteTwips += columnShare;
                            minShareOverflows[cellColumn] = false;
                        }
                        else
                        {
                            minShareOverflows[cellColumn] = true;
                            int columnPrefExtra = columnWidth - columnMinimum;
                            // Set new share as (pref - min) / extraMin. extraMin is wideMinimum - sum(cellMinimum).
                            double newColumnShare = (columnPrefExtra > 0)
                                ? columnPrefExtra / extraMinimumD
                                : 0d;

                            // This is because computedMin > column.Twips here.
                            // columnShare * extraMinimum - newShare * extraMinimum = computedMin - columnMinimum - (column.Twips - columnMinimum) > 0.
                            Debug.Assert(columnShare > newColumnShare);
                            minShareOverflowSum += columnShare - newColumnShare;
                            minShares[cellColumn] = newColumnShare;
                        }
                    }

                    // Now "overflowed" columns are not included in minShareSumLteTwips.
                    // Sum(minShares) may not equal 1 now.
                    // (Sum(minShares) + minShareOverflowSum) is still 1.

                    // Go through the columns where computedMin <= column.Twip (no overflow in the previous loop),
                    // and distribute the sum of overflow proportionally to the previously assigned minimum share.
                    // Strange logic.
                    double debugShareSum = 0d;
                    for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                    {
                        if (!minShareOverflows[cellColumn])
                            minShares[cellColumn] += minShareOverflowSum * minShares[cellColumn] / minShareSumLteTwips;
                        debugShareSum += minShares[cellColumn];
                    }
                    // Sum(minShares) is 1 again.
                    const double Eps = 0.01d;
                    Debug.Assert(System.Math.Abs(debugShareSum - 1d) < Eps);

                    // Finally use the computed shares to assign new cell minimums.
                    for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                    {
                        int gridColumn = firstCellColumn + cellColumn;
                        TableGridColumn column = Grid[gridColumn];
                        if (column.Percent > 0)
                            continue;

                        int columnWidth = Grid.GetColumnWidth(gridColumn);
                        int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                        // No rounding, cast off the remainder. It will be distributed later.
                        int spanningMinimumShare = (int)(extraMinimumD * minShares[cellColumn]);

                        // Still do not go above the preferred width.
                        if (columnMinimum + spanningMinimumShare > columnWidth)
                        {
                            spanningMinimumShare = columnWidth - columnMinimum;
                        }

                        if (IsAutoFit)
                            column.Metrics.ContentMinimum += spanningMinimumShare;
                        else
                            column.Metrics.Minimum += spanningMinimumShare;

                        // This imitates a nasty MS Word glitch.
                        extraMinimumD -= spanningMinimumShare;
                        // The ratio is counted from the total extra minimum.
                        // But it is taken from the remaining extra minimum on each iteration,
                        // So columns after the first column get a progressively lower share.
                    }

                    Grid.DebugWrite("Wide min distribution (2003 off): after step 1 (progressive glitchy).");

                    // The sum of the new minimums will NOT match the wide cell because of the glitch above.
                    // The logic below will take care of this and of the rounding differences as well.
                    extraMinimum = (int)extraMinimumD;
                    // No rounding should happen here as only int values were subtracted from extraMinimumD.
                    Debug.Assert(extraMinimumD - extraMinimum < Eps);

                    // Distribute the remaining part of the wide cell minimum by simply adding 1 to every column
                    // unless cell minimum has reached the preferred width.
                    while (extraMinimum > 0)
                    {
                        int extraminimumPrevious = extraMinimum;
                        // The loop goes through all spanned columns and may "distribute" more than extra width: TestMinSplitRoundingC.
                        for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                        {
                            int gridColumn = firstCellColumn + cellColumn;
                            TableGridColumn column = Grid[gridColumn];
                            // TestTcwMixedJ: though the previous logic deals with twip columns only, this loop affects pct columns as well.

                            int columnMinimum = Grid.GetColumnMinimum(gridColumn);
                            int columnWidth = Grid.GetColumnWidth(gridColumn);

                            // Do not assign minimum greater than the preferred.
                            // As the sum of the preferred widths was greater than the wide cell minimum, it is possible
                            // to distribute the wide cell minimum without going above any of the preferred widths.
                            if (columnMinimum < columnWidth)
                            {
                                // A condition on columnIsAuto used to be here, but it caused a minor difference with TestJava2062.
                                if (IsAutoFit)
                                    column.Metrics.ContentMinimum++;
                                else
                                    column.Metrics.Minimum++;

                                extraMinimum--;
                            }
                        }

                        // Added after fixing WORDSNET-23678
                        if (extraMinimum == extraminimumPrevious)
                        {
                            // This should not happen.
                            Debug.Fail("Unexpected situation when distributing wide cell minimum.");
                            // Set the extra minimum to 0 to prevent further handling as apparently something went wrong already.
                            extraMinimum = 0;
                            break;
                        }
                    }
                    // After all these steps, the wide cell minimum (or a bit more) should be distributed across the spanned columns,
                    // and no column minimum should exceed the column preferred.
                    Debug.Assert(-wideCell.GridSpan <= extraMinimum && extraMinimum <= 0);

                    Grid.DebugWrite("Wide min distribution (2003 off): after step 2 (decremental).");
                }
            }
            else
            {
                // It is not possible to distribute wide minimum without exceeding some of the preferred column widths
                // because wideMinimumForTwipColumns <= twipColumnPrefSum.

                int minPrefSum = twipColumnMinSum + twipColumnPrefSum;
                // Distribute proportionally to (minimum + preferred) for each column.

                extraMinimum = wideMinimumForTwipColumns;
                int equalShare = MathUtil.Divide(wideMinimumForTwipColumns, twipColumnCount);
                for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                {
                    int gridColumn = firstCellColumn + cellColumn;
                    TableGridColumn column = Grid[gridColumn];

                    if (column.Percent <= 0)
                    {
                        int columnWidth = Grid.GetColumnWidth(gridColumn);
                        int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                        // Assign the rounded value. Extra width will remain as is: TestMinSplitRounding*.
                        int newMinimum = (minPrefSum > 0)
                            ? MathUtil.Divide(wideMinimumForTwipColumns * (columnMinimum + columnWidth), minPrefSum)
                            : equalShare;

                        if (IsAutoFit)
                            column.Metrics.ContentMinimum = newMinimum;
                        else
                            column.Metrics.Minimum = newMinimum;

                        extraMinimum -= newMinimum;
                    }
                }

                Grid.DebugWrite("Wide min distribution (2003 off, proportional to min+pref)");
            }

            return extraMinimum;
        }

        /// <summary>
        /// Adds 1 to the values in the first columns to compensate for the given extra width.
        /// </summary>
        /// <remarks> The method is used to compensate rounding errors so that the sum of the calculated values assigned to the columns
        /// would match the total distributed among the columns during calculation.</remarks>
        /// <param name="grid">Table grid being adjusted.</param>
        /// <param name="firstCellColumn">The first column to adjust.</param>
        /// <param name="gridSpan">The number of columns to adjust.</param>
        /// <param name="extraWidth">Extra width to be distributed among the columns.</param>
        /// <param name="roundingColumn">The type of the column width to adjust.</param>
        internal static void FixRounding(TableGrid grid, int firstCellColumn, int gridSpan, int extraWidth, GridRoundingColumn roundingColumn)
        {
            // The sum of the assigned minimums may not match the wide cell because of rounding.
            // The difference should not be greater than the number of columns (rounding error is less than 1 for each column).
            Debug.Assert(extraWidth <= gridSpan);

            if (extraWidth <= 0)
                return;

            int remainingExtra = extraWidth;
            // Add 1 twip to each of the first columns.
            for (int cellColumn = 0; (remainingExtra > 0) && (cellColumn < gridSpan); ++cellColumn)
            {
                TableGridColumn column = grid[firstCellColumn + cellColumn];

                switch (roundingColumn)
                {
                    case GridRoundingColumn.TwipsOrContentMax:
                        bool updateContentMax = grid.IsAutoFit && column.Twips <= 0;
                        if (updateContentMax)
                            column.Metrics.ContentMaximum++;
                        else
                            column.Twips++;
                        break;
                    case GridRoundingColumn.Minimum:
                        if (grid.IsAutoFit)
                            column.Metrics.ContentMinimum++;
                        else
                            column.Metrics.Minimum++;
                        break;
                    case GridRoundingColumn.Width:
                        column.Width++;
                        break;
                    default:
                        Debug.Fail("Unexpected rounding type.");
                        break;
                }

                remainingExtra--;
            }

            grid.DebugWrite(string.Format(
                "1 twip added to {0} columns starting from {1} to compensate for rounding:", extraWidth, firstCellColumn));
        }

        /// <summary>
        /// Distributes wide cell percent preferred width across the spanned columns.
        /// </summary>
        /// <returns> The sum of the percent preferred width of the spanned columns.</returns>
        /// <remarks>
        /// If a wide cell has a percent preferred width, all spanned columns are assigned a percent preferred width.
        /// The column preferred type is changed to percent even if the assigned percent value is zero.
        /// </remarks>
        private int SetPctPreferredFromWideCell2003Off(CellSpan wideCell, int firstCellColumn, int totalPercent)
        {
            Debug.Assert(!UseWord2003Rules);

            // Nothing to do if there is no percent columns and no need to distribute wide cell percent preferred.
            if (!wideCell.PreferredWidth.IsPercent && (totalPercent <= 0))
                return 0;

            int widePercentExtra = 0;
            int twipSumForNoPct = 0;
            int noPctColumnCount = 0;
            if (wideCell.PreferredWidth.IsPercent)
            {
                // Set percent type for all spanned columns.
                for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
                {
                    int gridColumn = firstCellColumn + cellColumn;
                    TableGridColumn column = Grid[gridColumn];
                    if (!column.HasPreferredPercent)
                    {
                        // This will set pct type the current column.
                        column.Percent = 0;
                        TableGridDebugLogger.DebugWriteLine(
                            string.Format("Percent type is assigned to grid column {0} from a wide cell", firstCellColumn + cellColumn));
                    }

                    // For the purpose of wide cell percent preferred distribution, all spanned columns having 0 pct are treated as if having no pct.
                    // TestTcwMixedC.
                    if (column.Percent <= 0)
                    {
                        int columnWidth = Grid.GetColumnWidth(gridColumn);
                        twipSumForNoPct += columnWidth;
                        noPctColumnCount++;
                    }
                }

                int wideCellPercent = System.Math.Min(TableGridMetrics.HundredPercent, wideCell.PreferredWidth.ValueRaw);
                widePercentExtra = wideCellPercent - totalPercent;
            }

            int pctPreferredSum;
            // TestTcwMixedL: For some reason, wide cell percent preferred is not distributed at all
            // if all spanned columns have non-zero pct width. Looks like a glitch in MS Word.
            bool isPctDistributionNeeded = (widePercentExtra > 0) && (noPctColumnCount > 0);
            if (isPctDistributionNeeded)
            {
                // Distribute extra wide cell percent width across the columns without the percent width.
                pctPreferredSum = 0;
                int equalShare = widePercentExtra / noPctColumnCount;

                for (int column = 0; column < wideCell.GridSpan; ++column)
                {
                    int columnIdx = firstCellColumn + column;
                    TableGridColumn gridColumn = Grid[columnIdx];
                    Debug.Assert(gridColumn.HasPreferredPercent);

                    int columnPct = gridColumn.Percent;
                    int columnWidth = Grid.GetColumnWidth(columnIdx);
                    // This will assign a new width to the pct columns with pct pref 0 as well.
                    // TestTcwMixedC seems to confirm that this is the correct way.
                    if (columnPct <= 0)
                        columnPct = (twipSumForNoPct > 0)
                            ? widePercentExtra * columnWidth / twipSumForNoPct
                            : equalShare;

                    if (columnPct > gridColumn.Percent)
                        gridColumn.Percent = columnPct;

                    pctPreferredSum += columnPct;
                }

                Grid.DebugWrite("After distributing pct preferred from a wide cell:");
            }
            else
            {
                // No distribution, just return the current pct sum.
                pctPreferredSum = System.Math.Min(TableGridMetrics.HundredPercent, totalPercent);
            }


            // TestTcwMixedE shows a case where all spanned columns still get 0 pct after distribution.
            Debug.Assert(0 <= pctPreferredSum && pctPreferredSum <= TableGridMetrics.HundredPercent);

            return pctPreferredSum;
        }

        /// <summary>
        /// Distributes wide cell twip preferred width across the spanned columns.
        /// </summary>
        private void SetAbsWidthFromWideCell(CellSpan wideCell, int firstCellColumn)
        {
            Debug.Assert(!UseWord2003Rules);

            // Nothing to do for percent cells in fixed layout.
            if (wideCell.PreferredWidth.IsPercent && !IsAutoFit)
                return;

            int twipSum = 0;
            bool isPercentColumnSpanned = false;
            for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
            {
                int gridColumn = firstCellColumn + cellColumn;
                TableGridColumn column = Grid[gridColumn];
                int columnWidth = Grid.GetColumnWidth(gridColumn);

                isPercentColumnSpanned |= column.HasPreferredPercent;

                twipSum += columnWidth;
            }

            // For wide cells width fixed preferred width, distribute preferred.
            // The minimum is already distributed.
            // For percent and auto cells, distribute content maximum in auto-fit mode.
            int wideCellWidth = wideCell.PreferredWidth.IsFixed
                ? wideCell.PreferredWidth.ValueTwips
                : IsAutoFit ? wideCell.ContentMaximum : 0;
            // TestTcwAutoSplit() confirms (pseudo) auto cell distribution width in fixed layout.

            wideCellWidth -= GetSpannedSpacing(wideCell);

            if (wideCellWidth <= twipSum)
                return;

            int equalShare = isPercentColumnSpanned
                ? 0
                : MathUtil.Divide(wideCellWidth, wideCell.GridSpan);
            int widePrefRemainder = wideCellWidth;

            // Distribute extra wide cell preferred width.
            for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
            {
                int gridColumn = firstCellColumn + cellColumn;
                TableGridColumn column = Grid[gridColumn];
                int columnWidth = Grid.GetColumnWidth(gridColumn);

                // It is possible that twipSum is 0 in a mix of pct and twip cells: TestTcwMixedK.
                // However in that case wide cell twip distribution does not seem to affect the final grid as pct column logic applies.
                int newPreferred = (twipSum > 0)
                    ? MathUtil.Divide(columnWidth * wideCellWidth, twipSum)
                    : equalShare;

                bool updateContentMax = IsAutoFit && column.Twips <= 0;
                if (updateContentMax)
                    column.Metrics.ContentMaximum = newPreferred;
                else
                    column.Twips = newPreferred;

                widePrefRemainder -= newPreferred;
            }

            Grid.DebugWrite("After distributing absolute preferred from a wide cell:");
            // TestWideTwipDistributionRounding seems to confirm that no action to make the new sum to match the wide cell is needed.

            FixRounding(Grid, firstCellColumn, wideCell.GridSpan, widePrefRemainder, GridRoundingColumn.TwipsOrContentMax);
        }

        /// <summary>
        /// Distributes wide cell minimum across the columns with percent preferred width.
        /// </summary>
        /// <returns> The sum of the calculated minimums for percent columns.</returns>
        /// <remarks>
        /// The return value is the sum of the calculated minimums that may be less than the minimum currently assigned to the column.
        /// The value is later used to reproduce a glitch demonstrated by a case in TestTcwMixedJ:
        /// More than needed is distributed when splitting wide cell minimum among non-pct columns because of this value.
        /// </remarks>
        private int SetPctMinimumsFromWideCell2003Off(CellSpan wideCell, int firstCellColumn,
            int pctPreferredSum, bool wideCellSpansNonPctColumns)
        {
            Debug.Assert(!UseWord2003Rules);

            if (pctPreferredSum <= 0)
                return 0;
            // TestTcwMixedE shows a case when all spanned columns get 0 pct value, so pct sum is 0.
            // In this case, they will be handled by the logic for absolute columns.

            int pctTotal = pctPreferredSum;

            // TestTcwMixedD: If there are non-pct columns, distribute the minimum among pct columns
            // using 100% as the total. The rest will go to non-pct columns.
            if (wideCellSpansNonPctColumns && !wideCell.PreferredWidth.IsPercent)
                pctTotal = TableGridMetrics.HundredPercent;

            int wideCellMinimum = GetCellMinimumForDistribution(wideCell);
            // The caller checks that the minimum distribution is actually needed.
            Debug.Assert(wideCellMinimum > 0);

            // Distribute wide cell minimum across pct columns.
            int pctSumRemainder = pctTotal;
            // A weird sum of calculated pct column minimums.
            // A calculated value is included even if it is less than the current minimum.
            // This is needed to reproduce (possibly faulty) MS Word logic for TestTcwMixedJ.
            int pctCalculatedMinSum = 0;
            for (int cellColumn = 0; cellColumn < wideCell.GridSpan; ++cellColumn)
            {
                int gridColumn = firstCellColumn + cellColumn;
                TableGridColumn column = Grid[gridColumn];
                int columnPct = column.Percent;
                // Handling of pct greater than 100% is different from 2003 mode here.
                if (pctSumRemainder < columnPct)
                    columnPct = pctSumRemainder;
                pctSumRemainder -= columnPct;

                int newMinimum = wideCellMinimum * columnPct / pctTotal;
                // Count the sum of the calculated values even if they may be actually less than the current values.
                pctCalculatedMinSum += newMinimum;

                int columnMinimum = Grid.GetColumnMinimum(gridColumn);

                if (newMinimum > columnMinimum)
                {
                    if (IsAutoFit)
                        column.Metrics.ContentMinimum = newMinimum;
                    else
                        column.Metrics.Minimum = newMinimum;
                }
            }

            Grid.DebugWrite("After setting minimums from wide cell (pct columns when 2003 mode is off):");
            // The sum of the new minimums may still not match the wide cell because of rounding.
            // The rounding difference is handled by the caller.

            return pctCalculatedMinSum;
        }

        /// <summary>
        /// Gets the table metrics by going through each row block grid.
        /// </summary>
        internal TableGridMetrics GetGridMetricsForTable(PreferredWidth tblw)
        {
            int minimum = 0;
            int contentMinimum = 0;
            int contentMaximum = 0;

            // Get metrics as column metric sums from the grid(s).
            foreach (TableGrid rowBlockGrid in BlockGrids)
            {
                TableGridMetrics blockGridMetrics = rowBlockGrid.GetMetricsForTable();

                int spacingSum = HalfSpacing * 2 * (rowBlockGrid.Count + 1);

                minimum = System.Math.Max(minimum, blockGridMetrics.Minimum + spacingSum);
                contentMinimum = System.Math.Max(contentMinimum, blockGridMetrics.ContentMinimum + spacingSum);
                contentMaximum = System.Math.Max(contentMaximum, blockGridMetrics.ContentMaximum + spacingSum);
            }

            // Take tblw into account.
            if (tblw.IsFixed)
            {
                if (tblw.ValueTwips > 0)
                {
                    contentMinimum = System.Math.Max(contentMinimum, tblw.ValueTwips);
                    contentMaximum = contentMinimum;
                    // There are tests for nested tables when preferred width used 
                    // as a content maximum (for calculating container table width from containing cell tcw in pct units in TestNestedContainerTcwPct)
                    // and as a content minimum (for calculating container cell auto width in TestNestedTblw).
                }
                else
                {
                    // Fixed preferred width type, but zero width.
                    // This is a not standard situation which must be handled specially.
                    // Setting content minimum to content maximum fixed a table in TestTcw0DxaNested and some golds in Test20767.
                    contentMinimum = contentMaximum;
                }
            }
            else
            {
                // Get auto table width.
                // It is used for metrics even when preferred table width is in pct units
                // (which is not quite logical as the tblw may be less than 100% in that case): see TestNestedTblwPctA etc.
                int autoTableWidth = 0;
                foreach (TableGrid rowBlockGrid in BlockGrids)
                {
                    int spacingSum = HalfSpacing * 2 * (rowBlockGrid.Count + 1);
                    autoTableWidth = System.Math.Max(autoTableWidth,
                        TableGridResizer.GetAutoTableWidth(rowBlockGrid) + spacingSum);
                }

                // Use auto table width calculated from pct tcw as a content maximum.
                // Do not use it as a content minimum.
                contentMaximum = System.Math.Max(autoTableWidth, contentMinimum);
            }

            return new TableGridMetrics(minimum, contentMinimum, contentMaximum);
        }

        /// <summary>
        /// Makes a combined grid from row block grids and applies it to the table in the document model.
        /// </summary>
        internal TableGridAttr ApplyCombinedGrid(Table table, bool isGridApplicable)
        {
            int[] calculatedGridSums = CombineBlockGrids();

            if (HasSpacing && HalfSpacing > 0)
            {
                AddSpacing(HalfSpacing, calculatedGridSums);
                TableGridDebugLogger.DebugWriteLine(string.Format("Cell spacing {0} added to the combined grid.", HalfSpacing * 2));
            }

            // Whatever is above, should work with "new grid" attributes only: CellPr.GridSpan,
            // TablePr.Grid, TablePr.GridBefore, TablePr.GridAfter.
            // "Old" attributes should only be changed if the new grid is to be applied.

            TableGridAttr calculatedGrid = ToTableGridAttr(calculatedGridSums);
            TableGridDebugLogger.DebugWriteCalculatedGrid(calculatedGrid);

            // Do not apply grid to the table in case of revisions, spacing and other unsupported cases.
            if (isGridApplicable && (calculatedGridSums != null))
            {
                ApplyGrid(table, calculatedGridSums, calculatedGrid);
            }
            else
            {
                calculatedGrid.State = GridState.CalculationFailed;
                TableGridDebugLogger.DebugWriteLine("Grid not applied because of an unsupported case.");
            }

            return calculatedGrid;
        }

        /// <summary>
        /// Adds spacing to the vector of cumulative column grid sums.
        /// </summary>
        private static void AddSpacing(int halfSpacing, int[] gridSums)
        {
            if (gridSums.Length < 2)
            {
                Debug.Fail("Unexpected grid sums array length.");
                return;
            }

            // Extra half spacing is added to the first column.
            int spacingSum = halfSpacing;
            // Full spacing (two halves is added to each column.
            int spacing = halfSpacing * 2;

            for (int col = 1; col < gridSums.Length; ++col)
            {
                spacingSum += spacing;
                gridSums[col] += spacingSum;
            }

            // Extra half spacing is added to the last column.
            gridSums[gridSums.Length - 1] += halfSpacing;
        }

        /// <summary>
        /// Combines row block grids into a single grid that may have more than 64 columns.
        /// </summary>
        private int[] CombineBlockGrids()
        {
            if ((BlockGrids.Count != NextBlockStartRows.Count) ||
                (BlockGrids.Count == 0))
            {
                // This should not happen.
                Debug.Fail("Invalid state: no grids to combine.");
                return null;
            }

            int combinedColumnCountEstimation = BlockGrids.Count * System.Math.Max(ColumnCountLimit64, BlockGrids[0].Count);
            SortedIntegerListGeneric<object> combinedGrid = new SortedIntegerListGeneric<object>(combinedColumnCountEstimation);
            combinedGrid.Add(0, null);
            int[][] gridSumsList = new int[BlockGrids.Count][];
            int[] gridSums;
            // Go through the grids, compute incremental widths and add them to the sorted list.
            for (int gridIdx = 0; gridIdx < BlockGrids.Count; ++gridIdx)
            {
                TableGrid grid = BlockGrids[gridIdx];
                // A list of incremental widths simplifies grid span width calculation.
                // TODO consider moving to the table grid class and re-using.
                gridSums = new int[grid.Count + 1];
                // Having zero in the first column simplifies the logic.
                gridSums[0] = 0;
                gridSumsList[gridIdx] = gridSums;
                int columnRight = 0;
                for (int colIdx = 0; colIdx < grid.Count;)
                {
                    TableGridColumn column = grid[colIdx];
                    columnRight += column.Width;

                    // Add to the combined list.
                    if (!combinedGrid.ContainsKey(columnRight))
                        combinedGrid[columnRight] = null;

                    // Add to the sums and advance.
                    gridSums[++colIdx] = columnRight;
                }
            }

            if (BlockGrids.Count == 1)
            {
                // Nothing to merge.
                return gridSumsList[0];
            }

            // Now go through the rows and update the spans with values calculated from the combined grid.
            int combinedColumnCount = combinedGrid.Count - 1;
            int blockIdx = 0;
            int nextBlockRowIdx = NextBlockStartRows[0];
            gridSums = gridSumsList[0];
            for (int rowIdx = 0; rowIdx < Rows.Count;)
            {
                RowSpan row = Rows[rowIdx];

                if (rowIdx == nextBlockRowIdx)
                {
                    // Advance to next block.
                    ++blockIdx;
                    gridSums = gridSumsList[blockIdx];
                    nextBlockRowIdx = NextBlockStartRows[blockIdx];
                }

                int cellRight;
                int cellColumn = 0;
                int cellColumnCombined = 0;
                int gridSpan;
                if (row.Before.GridSpan > 0)
                {
                    gridSpan = row.Before.GridSpan;
                    // Get the new grid span.
                    cellRight = gridSums[gridSpan];
                    cellColumnCombined = combinedGrid.IndexOfKey(cellRight);
                    Debug.Assert(cellColumnCombined > 0);

                    // Set the new grid span.
                    row.Before.GridSpan = cellColumnCombined;

                    // Advance.
                    cellColumn = gridSpan;
                }

                foreach (CellSpan cell in row.Cells)
                {
                    gridSpan = cell.GridSpan;
                    int nextCellColumn = cellColumn + gridSpan;

                    // Get the new grid span.
                    cellRight = gridSums[nextCellColumn];
                    int nextCellColumnCombined = combinedGrid.IndexOfKey(cellRight);
                    Debug.Assert(cellColumnCombined < nextCellColumnCombined);

                    // Set the new grid span.
                    cell.GridSpan = nextCellColumnCombined - cellColumnCombined;

                    // Advance.
                    cellColumnCombined = nextCellColumnCombined;
                    cellColumn = nextCellColumn;
                }
                row.SpanCount = cellColumnCombined;

                // Row gridAfter should be re-calculated from the rightmost column,
                // gridAfter calculated from a block grid may not be correct as the combined grid may be wider.
                int combinedGridAfter = combinedColumnCount - row.SpanCount;
                Debug.Assert(combinedGridAfter >= 0);
                combinedGridAfter = System.Math.Max(combinedGridAfter, 0);
                // Set the new grid span.
                if (row.After.GridSpan != combinedGridAfter || !row.After.PreferredWidth.IsAuto)
                {
                    row.After.GridSpan = combinedGridAfter;
                    // TestWidthAfterTwip64 shows that removing widthAfter should not be needed.
                    // However, keeping width after width makes output worse for TestJira7090.
                    if (IsBinaryDocRtf)
                    {
                        // The output of TestJira7090 with this line does not match MS Word precisely either,
                        // but removing the line causes different line wrapping in one of the columns.
                        // Most likely, the reason is incorrect assignment of widths or spans from binary doc attributes.
                        // and the line should be eventually removed.
                        // Keep it for now to preserve the better gold.

                        // Set widthAfter to none: it will be taken from the combined grid.
                        row.After.PreferredWidth = CellSpan.NoWidth;
                    }
                }

                // Advance to next row.
                ++rowIdx;
            }

            return CombinedGridToSumsArray(combinedGrid);
        }

        /// <summary>
        /// Creates and array of cumulative grid column widths sum from the grid represented by the argument.
        /// </summary>
        private static int[] CombinedGridToSumsArray(SortedIntegerListGeneric<object> combinedGrid)
        {
            int[] gridSums = new int[combinedGrid.Count];
            for (int column = 0; column < combinedGrid.Count; ++column)
                gridSums[column] = combinedGrid.GetKey(column);

            return gridSums;
        }

        /// <summary>
        /// Converts an array of cumulative grid column width sums to a list of column widths.
        /// </summary>
        private static IntList GridSumsToIntList(int[] gridSums)
        {
            if (gridSums == null || gridSums.Length < 2)
            {
                Debug.Fail("Invalid arguments.");
                return null;
            }

            int columnCount = gridSums.Length - 1;
            IntList widths = new IntList(columnCount);
            int widthBefore = 0;
            for (int column = 1; column < gridSums.Length; ++column)
            {
                int widthBeforeNext = gridSums[column];
                int width = widthBeforeNext - widthBefore;
                widths.Add(width);
                widthBefore = widthBeforeNext;
            }

            return widths;
            // An array could be easily used instead of IntList here.
            // I believe that IntList is originally used in TableGridAttr so that a column could be easily added or removed.
            // I'm not sure if it is needed now.
        }

        /// <summary>
        /// Converts an array of cumulative grid column widths to table grid attribute with the given column widths.
        /// </summary>
        private static TableGridAttr ToTableGridAttr(int[] gridSums)
        {
            TableGridAttr gridAttr;
            if (gridSums != null && gridSums.Length > 1)
            {
                gridAttr = new TableGridAttr(GridSumsToIntList(gridSums));
                // Grid state is to be set by the caller.
            }
            else
            {
                gridAttr = new TableGridAttr();
                gridAttr.State = GridState.CalculationFailed;
            }

            return gridAttr;
        }

        /// <summary>
        /// Applies the grid represented by the given cumulative grid sums to the table in the document model.
        /// </summary>
        private void ApplyGrid(Table table, int[] gridSums, TableGridAttr calculatedGrid)
        {
            NodeCollection tableRows = table.Rows;
            int tableRowIndex = 0;

            // It is not needed for auto-fit tables as merges are normalized before the layout building.
            if (!table.AllowAutoFit)
                table.NormalizeHorizontalMerge();

            foreach (RowSpan row in Rows)
            {
                Row tableRow;
                do // Skip rows without cells, they were not added to mRows.
                {
                    tableRow = (Row)tableRows[tableRowIndex];
                    tableRowIndex++;
                } while (tableRow.Cells.Count == 0);

                // Update row grid before and width before.
                int gridBefore = row.Before.GridSpan;
                int widthBeforeColumn = gridSums[gridBefore];
                ApplyGridBefore(tableRow.TablePr, row.Before);

                // Update width and span count for each cell.
                NodeCollection tableCells = tableRow.Cells;
                int tableCellIndex = 0;
                int columnIndex = row.Before.GridSpan;
                foreach (CellSpan cell in row.Cells)
                {
                    Cell tableCell = (Cell)tableCells[tableCellIndex];
                    tableCellIndex++;

                    int nextColumn = columnIndex += cell.GridSpan;
                    int widthBeforeNextColumn = gridSums[nextColumn];
                    int width = widthBeforeNextColumn - widthBeforeColumn;
                    tableCell.CellPr.Width = width;
                    tableCell.CellPr.GridSpan = cell.GridSpan;

                    // Even if the cell may no longer be treated as vertically merged,
                    // merge attributes remain unchanged on saving to docx: TestVmerge64columnsA().

                    // Update cell preferred type for fixed layout tables.
                    if (!table.AllowAutoFit && tableCell.CellPr.PreferredWidth.IsAuto)
                    {
                        // Replace auto preferred with absolute.
                        // MS Word seems to behave this way.
                        // It helps to get the same cell width on re-opening a document with updated grid,
                        // Otherwise auto cells would get the preferred width from the new grid which might be different.
                        tableCell.CellPr.PreferredWidth = cell.PreferredWidth;
                    }
                    columnIndex = nextColumn;
                    widthBeforeColumn = widthBeforeNextColumn;
                }

                ApplyGridAfter(tableRow.TablePr, row.After);
            }

            // Save the grid, overwrite the grid from the document.
            table.TablePr.SetAttr(TableAttr.Sys_TableGrid, calculatedGrid.Columns);

            calculatedGrid.State = GridState.Applied;
        }

        /// <summary>
        /// Updates row attributes in the document model according to the given value.
        /// </summary>
        private void ApplyGridBefore(TablePr rowPr, CellSpan rowBefore)
        {
            ApplyGridBeforeAfter(rowPr, rowBefore, TableAttr.Sys_GridBefore, TableAttr.WidthBefore);
        }

        /// <summary>
        /// Updates row attributes in the document model according to the given value.
        /// </summary>
        private static void ApplyGridAfter(TablePr rowPr, CellSpan rowAfter)
        {
            ApplyGridBeforeAfter(rowPr, rowAfter, TableAttr.Sys_GridAfter, TableAttr.WidthAfter);
        }

        /// <summary>
        /// Updates the specified attributes from the given width before/after value.
        /// </summary>
        private static void ApplyGridBeforeAfter(TablePr rowPr, CellSpan value, int gridSpanAttr, int widthAttr)
        {
            // Do not calculate width from the grid.
            // It should have the value calculated during grid construction,
            // before resizing and even before wide cell distribution.
            int width = value.PreferredWidth.ValueRaw;
            int gridSpan = value.GridSpan;

            PreferredWidth originalWidth = (PreferredWidth)rowPr.GetDirectAttr(widthAttr);
            bool removeZeroWidth = (originalWidth != null) && (originalWidth.ValueRaw <= 0);

            // Update row attributes in the model.
            if (removeZeroWidth || value.HasNoWidth || width <= 0)
            {
                // Remove width attribute from the model if present.
                rowPr.Remove(widthAttr);

                // It is OK to have things like gridAfter > 0 but no widthAfter.
                // MS Word works this way when the missing columns all have auto widths.
                // See TestWidthAfterAuto().
            }
            else
            {
                rowPr.SetAttr(widthAttr, value.PreferredWidth);

                Debug.Assert(gridSpan > 0);
                gridSpan = System.Math.Max(1, gridSpan);
            }

            if (gridSpan > 0)
            {
                rowPr.SetAttr(gridSpanAttr, gridSpan);
            }
            else
            {
                // Remove grid span from the model if present.
                rowPr.Remove(gridSpanAttr);
            }
        }

        /// <summary>
        /// Stores the working copy of the original tblGrid data.
        /// </summary>
        private IntList GridFromDocumentOriginal { get; }

        /// <summary>
        /// Table grid derived from the original table grid for the current row block.
        /// </summary>
        /// <remarks>
        /// The grid is used when a complex table is split into separate row blocks for independent grid construction.
        /// The columns are originally copied from the grid in the document.
        /// Some columns may be merged or removed as the row block grid cannot have more than 64 columns.
        /// </remarks>
        private IntList GridFromDocumentBlock
        {
            get { return BlockGridsFromDocument[CurrentBlock]; }
        }

        /// <summary>
        /// Gets the current row block grid (that is, the grid being constructed).
        /// </summary>
        internal TableGrid Grid
        {
            get { return BlockGrids[CurrentBlock]; }
        }

        internal bool IsUnsupportedConditionDetected { get; private set; }

        private bool IsAutoFit { get; }

        /// <summary>
        /// Indicates if spacing is specified for the table in this grid constructor.
        /// </summary>
        /// <remarks>
        /// This includes zero spacing.
        /// The logic should be reworked when different spacing for rows in the same table are supported.
        /// </remarks>
        private bool HasSpacing { get { return mHalfSpacing >= 0; } }

        /// <summary>
        /// Stores table spacing (half of it as in MS Word ooxml format).
        /// </summary>
        /// <remarks>
        /// The method should be used in conjunction with <see cref="HasSpacing"/> as negative value is never returned,
        /// but it has a special meaning (no spacing).
        /// </remarks>
        private int HalfSpacing
        {
            get { return HasSpacing ? mHalfSpacing : 0; }
            set { mHalfSpacing = value; }
        }
        private int mHalfSpacing;

        private IList<RowSpan> Rows { get; }

        /// <summary>
        /// A list of grid column types that takes wide cells endings into account.
        /// </summary>
        /// <remarks>
        /// This is different from the column type in Grid and it should eventually supersede it.
        /// TODO merge with grid after reworking cell match data there.
        /// </remarks>
        private List<GridCellMatchType> GridColumnTypes { get; }

        /// <summary>
        /// Stores maximum preferred widths of cells ending in each grid column, by grid spans.
        /// </summary>
        /// <remarks>
        /// For each grid column and int array for all possible grid spans ending there is stored.
        /// So for column 0, int[1] is stored, for column 1, int[2] and so on.
        /// </remarks>
        private List<int[]> EndingCellPreferredMaxList { get; }

        /// <summary>
        /// Stores the last widthBefore value encountered in the first column.
        /// </summary>
        /// <remarks>
        /// The value is used under certain conditions.
        /// </remarks>
        private PreferredWidth LastWidthBeforeInFirstColumn { get; set; }

        /// <summary>
        /// Stores the last widthAfter value encountered in the last column.
        /// </summary>
        /// <remarks>
        /// The value is used under certain conditions.
        /// </remarks>
        private PreferredWidth LastWidthAfterInLastColumn { get; set; }

        /// <summary>
        /// Stores the grids constructed for each row block.
        /// </summary>
        private List<TableGrid> BlockGrids { get; }

        /// <summary>
        /// The list stores the id of the row that starts the next row block.
        /// </summary>
        /// <remarks>
        /// So NextBlockStartRows[i] stores the id of the first row after block i.
        /// </remarks>
        private IntList NextBlockStartRows { get; }

        /// <summary>
        /// Stores "grid from document" for each row block that is derived fro the original "grid from document"
        /// after merging the columns not present in the block.
        /// </summary>
        private List<IntList> BlockGridsFromDocument { get; }

        /// <summary>
        /// Id of the row that starts the current block.
        /// </summary>
        private int RowBlockStart { get; set; }

        /// <summary>
        /// Id of the row that starts the next block.
        /// </summary>
        private int RowBlockNext
        {
            get { return NextBlockStartRows[CurrentBlock]; }
        }

        /// <summary>
        /// Id of the current row block.
        /// </summary>
        private int CurrentBlock { get; set; }

        /// <summary>
        /// Represents that the current row block is not set.
        /// </summary>
        private const int CurrentBlockNotSet = -1;

        private readonly bool UseWord2003Rules;

        /// <summary>
        /// Indicates if the document is a binary doc/rtf.
        /// </summary>
        /// <remarks>
        /// It is only needed to preserve a better (but not perfect) gold for a complex document in TestJira7090.
        /// See comments in <see cref="CombineBlockGrids"/>
        /// </remarks>
        private bool IsBinaryDocRtf { get; }

        internal const int ColumnCountLimit64 = 64;
    }
}
