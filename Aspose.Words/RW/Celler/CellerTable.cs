// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections.Generic;
using Aspose.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Celler
{
    [Flags]
    internal enum CellerTableOptions
    {
        None = 0x00,

        /// <summary>
        /// Option to resolve inherited paddings on cells.
        /// </summary>
        InheritedPadding = 0x01,

        /// <summary>
        /// Specifies that celler should visit merged cells when go to next cell.
        /// </summary>
        GotoMergedCells = 0x02
    }

    /// <summary>
    /// Converts a Word table into an intermediate model that is easier to write into PDF and HTML formats.
    /// 
    /// The main problem when converting DOC to HTML/PDF tables is that columns in different rows
    /// in a DOC file do not have to align vertically and do not have to have the same number of cells.
    /// This is not possible in HTML/PDf and the solution is to create more columns (for every vertical 
    /// position found in the DOC file for all rows) and then create HTML cells that span more than one 
    /// column if necessary. I call this algorithm "celler".
    /// 
    /// So this class cellerizes a document table into a CellerTable consisting of CellerCell objects.
    /// Each CellerCell is set to have the correct row and col span and width. Also optionally resolves any
    /// default table borders and borders for the pad cells.
    /// 
    /// Also, this class provides a mechanism to iterate over the cells.
    /// Strictly speaking, iterator should be separate, but because we have to support nested tables,
    /// it is easier to bundle the iterator right into here.
    /// 
    /// In the finished CellerTable there could be several types of CellerCells:
    /// 
    /// 1. Empty pad cells. These can occur at the beginning and end of a row, have IsPad = true, 
    /// borders will be resolved to match adjacent cell borders.
    ///
    /// 2. Normal non-merged cells that match cells in the document.
    /// These will have merge attributes and their borders set to be the same as in the document cell.
    ///
    /// 3. Normal merged cells that match cells in the document. These cells are merged to a previous 
    /// cell vertically or horizontally and will normally be skipped by the output algorithm.
    ///
    /// 4. Extra merged cells introduced by the celler algorithm. These cells do not match cell nodes in the 
    /// document, always merged horizontally to the previous cell and are skipped by the output algorithm.
    /// </summary>
    internal class CellerTable
    {
        /// <summary>
        /// Cellerizes the document table with default options.
        /// </summary>
        internal CellerTable(Table tableNode) : this(tableNode, CellerTableOptions.None) { }

        /// <summary>
        /// Cellerizes the document table.
        /// </summary>
        internal CellerTable(Table tableNode, CellerTableOptions options)
        {
            mTableNode = tableNode;

            // AM. It seems all callers want to resolve borders and do not populate empty pad borders so I removed these options to simplicity.
            // If later someone need this options again it can be easily reverted.
            mResolveInheritedBorders = true;
            mPopulateEmptyPadBorders = false;

            mResolveInheritedPaddings = (options & CellerTableOptions.InheritedPadding) != 0;
            mGotoMergedCells = (options & CellerTableOptions.GotoMergedCells) != 0;

            CalcColumnPositions();
            if (ColumnCount == 0)
                return;

            CreateCells();
            CalcCellsH();
            CalcCellsV();
            ProcessMergedRows();

            if (mHasPadCells)
                UpdatePadCellsBorders();

            MarkRowGroups();
        }

        /// <summary>
        /// Selects the first row.
        /// </summary>
        internal void GotoFirstRow()
        {
            mCurRowIdx = 0;
        }

        /// <summary>
        /// Selects next row.
        /// </summary>
        internal void GotoNextRow()
        {
            mCurRowIdx++;
        }

        /// <summary>
        /// Select first cell in the row.
        /// </summary>
        internal void GotoFirstCell()
        {
            mCurColIdx = -1;
            GotoNextCell();
        }

        /// <summary>
        /// Selects next cell that is not horizontally merged to previous cells in the row.
        /// </summary>
        internal void GotoNextCell()
        {
            if (mGotoMergedCells)
            {
                mCurColIdx++;
            }
            else
            {
                do
                {
                    mCurColIdx++;
                }
                while ((CurCell != null) && CurCell.CellPr.IsMergedToPrevious);
            }
        }

        internal int GetColumnWidth(int index)
        {
            return GetColumnWidth(index, 1);
        }

        /// <summary>
        /// Gets width of several columns.
        /// </summary>
        private int GetColumnWidth(int startIndex, int count)
        {
            Debug.Assert(count >= 0);
            return GetColumnPosition(startIndex + count) - GetColumnPosition(startIndex);
        }

        /// <summary>
        /// Returns an array containing the column widths.
        /// </summary>
        internal int[] GetColumnWidths()
        {
            int[] result = new int[mColumnPositions.Count - 1];
            for (int i = 0; i < result.Length; i++)
                result[i] = GetColumnWidth(i);
            return result;
        }

        internal int GetColumnPosition(int index)
        {
            return mColumnPositions.GetKey(index);
        }

        internal Table TableNode
        {
            get { return mTableNode; }
        }

        /// <summary>
        /// Gets total (cellerized) number of columns in the table.
        /// </summary>
        internal int ColumnCount
        {
            get { return mColumnPositions.Count - 1; }
        }

        internal int TableWidth
        {
            get { return GetColumnWidth(0, ColumnCount); }
        }

        /// <summary>
        /// Returns current row or null if the cursor is beyond the last row in the table.
        /// </summary>
        internal CellerRow CurRow
        {
            get { return (mCurRowIdx < mRows.Count) ? mRows[mCurRowIdx] : null; }
        }

        /// <summary>
        /// Returns current cell or null if the cursor is beyond the last cell in the row.
        /// </summary>
        internal CellerCell CurCell
        {
            get { return ((CurRow != null) && (mCurColIdx < ColumnCount)) ? CurRow.Cells[mCurColIdx] : null; }
        }

        /// <summary>
        /// Creates an array of all vertical column positions for every row in the table.
        /// </summary>
        private void CalcColumnPositions()
        {
            for (Row row = mTableNode.FirstRow; row != null; row = row.NextRow)
            {
                // Add all rows' column positions to the sorted array as keys. 
                // I don't use the values in this sorted array (only use keys), so insert nulls.
                int x = GetRowLeftOrigin(row);
                mColumnPositions[x] = null;

                foreach (Cell cell in row.Cells)
                {
                    x += GetCellWidth(cell);
                    mColumnPositions[x] = null;
                }
            }
        }

        /// <summary>
        /// Creates cells in such a way that allows us to handle columns that are not aligned vertically.
        /// </summary>
        private void CreateCells()
        {
            RowCollection rowNodes = mTableNode.Rows;
            int rowNodeCount = rowNodes.Count;

            mHasRowWithoutMisalignedColumns = false;

            for (int rowNodeIdx = 0; rowNodeIdx < rowNodeCount; ++rowNodeIdx)
            {
                CellerRow row = new CellerRow(ColumnCount, rowNodes[rowNodeIdx]);
                mRows.Add(row);

                CreatePadCells(row);

                CellCollection cellNodes = row.RowNode.Cells;
                int cellNodeCount = cellNodes.Count;

                bool hasMisalignedColumns = false;

                // This loop is for the normal (non pad) cells.
                for (int cellNodeIdx = 0; cellNodeIdx < cellNodeCount; ++cellNodeIdx)
                {
                    Cell curCellNode = cellNodes[cellNodeIdx];
                    CellPr curCellNodePr = curCellNode.CellPr.FinalPr;
                    int curCellWidth = GetCellWidth(curCellNode);

                    // WORDSNET-1058 Document fails with 'Index was outside the bounds of the array' exception when saved in AsposePdf xml.
                    // Cells that are merged to previous and have zero width seem to be
                    // handled in the loop for extra merged cells below, so skip them here.
                    if (curCellNodePr.IsMergedToPrevious && (curCellWidth == 0))
                        continue;

                    // This creates a celler cell with a copy of cell properties from the model cell.
                    CellerCell cell = CellerCell.CreateNormalCell(curCellNodePr, curCellNode);

                    int leftOfCell = GetCellLeftOrigin(row.RowNode, cellNodeIdx);
                    int cellIdx = mColumnPositions.IndexOfKey(leftOfCell);
                    int rightOfCell = leftOfCell + GetCellWidth(curCellNode);
                    int nextCellIdx = mColumnPositions.IndexOfKey(rightOfCell);
                    row.Cells[cellIdx] = cell;

                    // WORDSNET-1572 Borders are output incorrectly for the table with many merged cells.
                    // For merged cells right and bottom borders should be taken from the respective _last_ cells
                    // in horizontal and vertical chain. Since borders are taken from first cells when exporting anywhere
                    // make this trick only for them
                    Cell cellNodeRightBorderSource = curCellNode;
                    Cell cellNodeBottomBorderSource = curCellNode;
                    if (curCellNodePr.HorizontalMerge == CellMerge.First)
                    {
                        for (int cellNodeIdx2 = cellNodeIdx + 1; cellNodeIdx2 < cellNodeCount; ++cellNodeIdx2)
                        {
                            Cell rightNeighbourCellNode = cellNodes[cellNodeIdx2];
                            if (rightNeighbourCellNode.CellPr.FinalPr.HorizontalMerge != CellMerge.Previous)
                                break;

                            cellNodeRightBorderSource = rightNeighbourCellNode;
                        }
                    }
                    if (curCellNodePr.VerticalMerge == CellMerge.First)
                    {
                        for (int rowNodeIdx2 = rowNodeIdx + 1; rowNodeIdx2 < rowNodeCount; ++rowNodeIdx2)
                        {
                            Cell bottomNeighbourCellNode = FindCellMatchingLeft(rowNodes[rowNodeIdx2], leftOfCell);
                            if ((bottomNeighbourCellNode == null) ||
                                (bottomNeighbourCellNode.CellPr.FinalPr.VerticalMerge != CellMerge.Previous))
                            {
                                break;
                            }

                            cellNodeBottomBorderSource = bottomNeighbourCellNode;
                        }
                    }

                    // Resolve any default cell borders if requested.
                    // SPEED optimized.
                    CellPr cellPr = cell.CellPr;
                    GetCellBorderForExport(cellPr, curCellNode, CellAttr.BorderTop);
                    GetCellBorderForExport(cellPr, curCellNode, CellAttr.BorderLeft);
                    GetCellBorderForExport(cellPr, cellNodeBottomBorderSource, CellAttr.BorderBottom);
                    GetCellBorderForExport(cellPr, cellNodeRightBorderSource, CellAttr.BorderRight);
                    GetCellBorderForExport(cellPr, curCellNode, CellAttr.BorderDiagonalDown);
                    GetCellBorderForExport(cellPr, curCellNode, CellAttr.BorderDiagonalUp);

                    // The same for paddings.
                    GetCellPaddingForExport(cellPr, curCellNode, CellAttr.TopPadding);
                    GetCellPaddingForExport(cellPr, curCellNode, CellAttr.LeftPadding);
                    GetCellPaddingForExport(cellPr, curCellNode, CellAttr.BottomPadding);
                    GetCellPaddingForExport(cellPr, curCellNode, CellAttr.RightPadding);

                    // Create extra merged cells (due to cellerization) if necessary.
                    for (int extraCellIdx = cellIdx + 1; extraCellIdx < nextCellIdx; ++extraCellIdx)
                        row.Cells[extraCellIdx] = CellerCell.CreateHorizontallyMergedCell(cellPr, curCellNode);

                    if (nextCellIdx - cellIdx > 1)
                        hasMisalignedColumns = true;
                }

                if (!hasMisalignedColumns)
                    mHasRowWithoutMisalignedColumns = true;
            }
        }

        /// <summary>
        /// See <see cref="GetCellAttrsForExport" />.
        /// </summary>
        private void GetCellBorderForExport(CellPr destPr, Cell srcNode, int key)
        {
            GetCellAttrsForExport(mResolveInheritedBorders, destPr, srcNode, key);
        }

        /// <summary>
        /// See <see cref="GetCellAttrsForExport" />.
        /// </summary>
        private void GetCellPaddingForExport(CellPr destPr, Cell srcNode, int key)
        {
            GetCellAttrsForExport(mResolveInheritedPaddings, destPr, srcNode, key);
        }

        /// <summary>
        /// There are two possibilities for attributes being copied: either direct or inherited.
        /// </summary>
        private static void GetCellAttrsForExport(bool resolveInherited, CellPr destPr, Cell srcNode, int key)
        {
            ICellAttrSource cellAttrSource = srcNode;

            object value = resolveInherited
                ? cellAttrSource.FetchCellAttr(key)
                : cellAttrSource.GetDirectCellAttr(key);

            // WORDSNET-24112 Inherited borders of edge cells in tables with non-zero cell spacing are not the borders
            // that are actually rendered by MS Word. Inherited borders are copied from edge borders of the containing row
            // (left, right, top, or bottom) but rendered borders are copied from inner borders of the row (vertical
            // or horizontal). Here we copy rendered borders directly to cells.
            if (resolveInherited &&
                srcNode.ParentTable.AllowCellSpacing &&
                (cellAttrSource.GetDirectCellAttr(key) == null))
            {
                switch (key)
                {
                    case CellAttr.BorderTop:
                    case CellAttr.BorderBottom:
                        value = ((IRowAttrSource)srcNode.ParentRow).FetchRowAttr(TableAttr.BorderHorizontal);
                        break;
                    case CellAttr.BorderLeft:
                    case CellAttr.BorderRight:
                        value = ((IRowAttrSource)srcNode.ParentRow).FetchRowAttr(TableAttr.BorderVertical);
                        break;
                    default:
                        // Keep other values unchanged.
                        break;
                }
            }

            if (value != null)
                destPr.SetAttr(key, value);
        }

        /// <summary>
        /// Create pad cells:
        ///   - on the left of the row if this row is indented to the right;
        ///   - on the right of the row if the end of the row is indented to the left.
        /// </summary>
        private void CreatePadCells(CellerRow row)
        {
            int leftOfFirstCell = GetRowLeftOrigin(row.RowNode);
            int firstCellIdx = mColumnPositions.IndexOfKey(leftOfFirstCell);
            for (int i = 0; i < firstCellIdx; ++i)
                row.Cells[i] = CellerCell.CreatePadCell();

            int rightOfLastCell = GetCellLeftOrigin(row.RowNode, row.RowNode.Cells.Count);
            int beyondLastCellIdx = mColumnPositions.IndexOfKey(rightOfLastCell);
            for (int i = beyondLastCellIdx; i < ColumnCount; ++i)
                row.Cells[i] = CellerCell.CreatePadCell();

            // Mark as having both misaligned cells and pad cells if at least one of loops executed at least once.
            if ((firstCellIdx > 0) || (beyondLastCellIdx < ColumnCount))
                mHasPadCells = true;
        }

        /// <summary>
        /// This loop updates col spans and widths of the cells.
        /// </summary>
        private void CalcCellsH()
        {
            for (int rowIdx = 0; rowIdx < mRows.Count; rowIdx++)
            {
                int colSpan = 1;
                //This is a loop from the last column to the first.
                for (int colIdx = ColumnCount - 1; colIdx >= 0; colIdx--)
                {
                    CellerCell cell = mRows[rowIdx].Cells[colIdx];
                    switch (cell.CellPr.HorizontalMerge)
                    {
                        case CellMerge.First:
                        case CellMerge.None:
                            cell.CellPr.Width = GetColumnWidth(colIdx, colSpan);
                            cell.ColSpan = colSpan;
                            colSpan = 1;
                            break;
                        case CellMerge.Previous:
                            colSpan++;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown CellMerge type.");
                    }
                }
            }
        }

        /// <summary>
        /// Updates row spans so the cells will come out vertically merged.
        /// </summary>
        private void CalcCellsV()
        {
            for (int colIdx = 0; colIdx < ColumnCount; colIdx++)
            {
                int rowSpan = 1;
                for (int rowIdx = mRows.Count - 1; rowIdx >= 0; rowIdx--)
                {
                    CellerCell cell = mRows[rowIdx].Cells[colIdx];
                    switch (cell.CellPr.VerticalMerge)
                    {
                        case CellMerge.First:
                        case CellMerge.None:
                            cell.RowSpan = rowSpan;
                            rowSpan = 1;
                            break;
                        case CellMerge.Previous:
                            rowSpan++;
                            break;
                        default:
                            throw new InvalidOperationException("Unknown CellMerge type.");
                    }
                }
            }
        }

        /// <summary>
        /// Updates borders of the empty pad cells at the beginning and end of the rows.
        /// This code assumes that at least one non-padding cell exists in every row.
        /// Here we should determine whether we need empty borders in inheritance mode.
        /// Using assumption the same as in PDF: taking properties from the first row.
        /// </summary>
        private void UpdatePadCellsBorders()
        {
            Debug.Assert(mHasPadCells);
            Debug.Assert(ColumnCount > 1);

            if (mPopulateEmptyPadBorders)
            {
                TablePr tablePr = mTableNode.FirstRow.TablePr.FinalPr;
                mTableLeftVisible = tablePr.ContainsKey(TableAttr.BorderLeft);
                mTableRightVisible = tablePr.ContainsKey(TableAttr.BorderRight);
                mTableTopVisible = tablePr.ContainsKey(TableAttr.BorderTop);
                mTableBottomVisible = tablePr.ContainsKey(TableAttr.BorderBottom);
                mTableHorizontalVisible = tablePr.ContainsKey(TableAttr.BorderHorizontal);
                mTableVerticalVisible = tablePr.ContainsKey(TableAttr.BorderVertical);

                bool tableAnyVisible = mTableLeftVisible || mTableRightVisible || mTableTopVisible || mTableBottomVisible ||
                    mTableHorizontalVisible || mTableVerticalVisible;
                mEmptyBorder = tableAnyVisible ? new Border(LineStyle.None, 0, DrColor.Empty) : null;
            }

            for (int rowIdx = 0; rowIdx < mRows.Count; ++rowIdx)
            {
                CellerRow row = mRows[rowIdx];
                for (int colIdx = 0; UpdateOnePadCellBorder(row.Cells[colIdx], rowIdx, colIdx); ++colIdx)
                {
                    // Nothing to do, empty block.
                }
                for (int colIdx = ColumnCount - 1; UpdateOnePadCellBorder(row.Cells[colIdx], rowIdx, colIdx); --colIdx)
                {
                    // Nothing to do, empty block.
                }
            }
        }

        private bool UpdateOnePadCellBorder(CellerCell cell, int rowIdx, int colIdx)
        {
            bool result = cell.IsPad;
            if (result)
            {
                CellPr cellPr = cell.CellPr;

                bool topCell = (rowIdx == 0);
                bool bottomCell = (rowIdx == mRows.Count - 1);

                // The top border of the pad cell becomes the bottom border of the top cell unless at the top row already.
                bool topAssigned = !topCell && AssignCellAttr(cellPr, CellAttr.BorderTop, mRows[rowIdx - 1].Cells[colIdx].CellPr, CellAttr.BorderBottom);
                // The bottom border of the pad cell becomes the top border of the bottom row.
                bool bottomAssigned = !bottomCell && AssignCellAttr(cellPr, CellAttr.BorderBottom, mRows[rowIdx + 1].Cells[colIdx].CellPr, CellAttr.BorderTop);
                // Left and right borders of the pad cells look ok in the browser so don't worry about them.
                // (Unless we add empty borders.)

                // Need empty borders? Add them. At this point mEmptyBorder is initialized if they are needed.
                if (mEmptyBorder != null)
                {
                    Debug.Assert(mPopulateEmptyPadBorders);

                    if ((topCell && mTableTopVisible) || (!topCell && mTableHorizontalVisible && !topAssigned))
                        cellPr.BorderTop = mEmptyBorder;

                    if ((bottomCell && mTableBottomVisible) || (!bottomCell && mTableHorizontalVisible && !bottomAssigned))
                        cellPr.BorderBottom = mEmptyBorder;

                    // Here we have to do everything analogically to horizontal borders.
                    bool leftCell = (colIdx == 0);
                    bool rightCell = (colIdx == ColumnCount - 1);

                    bool leftAssigned = !leftCell && AssignCellAttr(cellPr, CellAttr.BorderLeft, mRows[rowIdx].Cells[colIdx - 1].CellPr, CellAttr.BorderRight);
                    bool rightAssigned = !rightCell && AssignCellAttr(cellPr, CellAttr.BorderRight, mRows[rowIdx].Cells[colIdx + 1].CellPr, CellAttr.BorderLeft);

                    if ((leftCell && mTableLeftVisible) || (!leftCell && mTableVerticalVisible && !leftAssigned))
                        cellPr.BorderLeft = mEmptyBorder;

                    if ((rightCell && mTableRightVisible) || (!rightCell && mTableVerticalVisible && !rightAssigned))
                        cellPr.BorderRight = mEmptyBorder;
                }
            }
            return result;
        }

        /// <summary>
        /// Assigns one attribute on source to another attribute on destination. Returns whether attribute is not null.
        /// </summary>
        private static bool AssignCellAttr(CellPr dest, int destKey, CellPr src, int srcKey)
        {
            object value = src.GetDirectAttr(srcKey);
            bool result = (value != null);
            if (result)
                dest.SetAttr(destKey, value);
            return result;
        }

        /// <summary>
        /// Returns true if the table has pad cells.
        /// </summary>
        internal bool HasPadCells
        {
            get { return mHasPadCells; }
        }

        /// <summary>
        /// Returns true if the table has at least one row without misaligned columns.
        /// </summary>
        internal bool HasRowWithoutMisalignedColumns
        {
            get { return mHasRowWithoutMisalignedColumns; }
        }

        /// <summary>
        /// Finds out merged rows and calculates related values.
        /// </summary>
        private void ProcessMergedRows()
        {
            FindMergedRows();
            CalculateMergedRowHeight();
            CalculateUnmergedRowSpans();
        }

        /// <summary>
        /// Marks merged rows and calculates the running total of merged row count.
        /// </summary>
        private void FindMergedRows()
        {
            // Indicates how many merged rows are located above the current row (including the current row itself).
            int mergedRowsAbove = 0;

            foreach (CellerRow row in mRows)
            {
                bool isMerged = true;

                // A row is merged if all its cells are vertically merged.
                foreach (CellerCell cell in row.Cells)
                {
                    if (cell.CellPr.VerticalMerge != CellMerge.Previous)
                    {
                        isMerged = false;
                        break;
                    }
                }

                if (isMerged)
                {
                    ++mergedRowsAbove;
                }

                row.IsMerged = isMerged;
                row.MergedRowsAbove = mergedRowsAbove;
            }
        }

        /// <summary>
        /// Calculates sum of heights for all adjacent merged rows and the last unmerged row before them.
        /// </summary>
        private void CalculateMergedRowHeight()
        {
            double sumMergedRowHeight = 0;
            CellerRow lastUnmergedRow = mRows[0];
            foreach (CellerRow row in mRows)
            {
                if (row.IsMerged)
                {
                    // Calculate sum of height of adjacent merged rows.
                    sumMergedRowHeight += GetRowHeightAproximate(row.RowNode);
                }
                else
                {
                    // End of a merged row range. Update unmerged row range info.
                    if (sumMergedRowHeight > 0)
                    {
                        lastUnmergedRow.MergedRowsHeight = sumMergedRowHeight + GetRowHeightAproximate(lastUnmergedRow.RowNode);
                    }
                    // Start a new merged row range.
                    lastUnmergedRow = row;
                    sumMergedRowHeight = 0;
                }
            }
            // A merged row range also ends the end of the table. Update unmerged row range info.
            if (sumMergedRowHeight > 0)
            {
                lastUnmergedRow.MergedRowsHeight = sumMergedRowHeight + GetRowHeightAproximate(lastUnmergedRow.RowNode);
            }
        }

        /// <summary>
        /// Calculates cell's row spans taking merged rows below them into account.
        /// </summary>
        private void CalculateUnmergedRowSpans()
        {
            for (int rowIndex = 0; rowIndex < mRows.Count; rowIndex++)
            {
                CellerRow row = mRows[rowIndex];
                foreach (CellerCell cell in row.Cells)
                {
                    if (cell.RowSpan > 1)
                    {
                        // Row span value of each cell is reduced by the number of merged rows that are inside the row span.
                        CellerRow lastRowInRowSpan = mRows[rowIndex + cell.RowSpan - 1];
                        cell.UnmergedRowSpan = cell.RowSpan - (lastRowInRowSpan.MergedRowsAbove - row.MergedRowsAbove);
                    }
                }
            }
        }

        private static double GetRowHeightAproximate(Row row)
        {
            double rowHeight = 0;
            // Row height depends on the first cell's font size.
            if ((row.FirstCell != null) && (row.FirstCell.FirstParagraph != null))
            {
                Font font = row.FirstCell.FirstParagraph.ParagraphBreakFont;
                DrFont drFont = row.Document.FontProvider.FetchDrFont(
                    font.Name,
                    (float)font.Size,
                    FontStyle.Regular);

                rowHeight = drFont.LineSpacingPoints;
            }
            // Takes larger of the table row's height and aproximate row height.
            return System.Math.Max(row.RowFormat.Height, rowHeight);
        }

        /// <summary>
        /// Calculates left coordinate of the row.
        /// The same result as GetCellLeftOrigin(rowNode, 0)
        /// </summary>
        private static int GetRowLeftOrigin(Row rowNode)
        {
            TablePr tablePr = rowNode.TablePr.FinalPr;
            return rowNode.ParentTable.GetTableLeft() + tablePr.WidthBeforeTwips;
        }

        /// <summary>
        /// Calculates left coordinate of the cell in the row with given index.
        /// Moved from Row class. Was marked as "Temporary hack used by CellerTable."
        /// </summary>
        private static int GetCellLeftOrigin(Row rowNode, int cellIndex)
        {
            int result = GetRowLeftOrigin(rowNode);

            CellCollection cells = rowNode.Cells;
            for (int i = 0; i < cellIndex; ++i)
                result += GetCellWidth(cells[i]);

            return result;
        }

        /// <summary>
        /// Finds a cell in the neibour row which can be merged.
        /// Let's find a cell exactly matching at left without care of right.
        /// For instance we can merge these four cells in MS Word:
        /// [ narrow ] [               wide               ]
        /// [               wide               ] [ narrow ]
        /// So checking left is enough to find the proper border I guess.
        /// </summary>
        private static Cell FindCellMatchingLeft(Row neighbourRowNode, int left)
        {
            int neighbourLeft = GetRowLeftOrigin(neighbourRowNode);
            if (neighbourLeft <= left)
            {
                CellCollection neighbourCells = neighbourRowNode.Cells;
                foreach (Cell cell in neighbourCells)
                {
                    if (neighbourLeft == left)
                        return cell;

                    neighbourLeft += GetCellWidth(cell);
                }
            }

            return null;
        }

        private static int GetCellWidth(Cell cell)
        {
            CellPr cellPr = cell.CellPr.FinalPr;
            int width = cellPr.Width;

            // WORDSNET-28434 It is possible in certain edge cases that an unmerged model cell will have zero width.
            // It is a problem for the celler algorithm, because zero width cells don't generate celler columns and,
            // as a result, no celler cell is generated for the model cell node, which leads to an exception.
            // In order to work around the problem, it was decided to process zero-width unmerged cells as min width cells.
            if ((width == 0) && !cellPr.IsMergedToPrevious)
            {
                width = 1;
            }

            return width;
        }

        /// <summary>
        /// Sets flags that indicate the position of each row in its row group (header or body).
        /// </summary>
        private void MarkRowGroups()
        {
            bool tableHasHeader = false;
            CellerRow previousCellerRow = null;
            foreach (CellerRow cellerRow in mRows)
            {
                // WORDSNET-20327 Write only one <thead> per table and make sure <thead> always precedes <tbody>.
                // Heading rows in the document model can come after normal (body) rows but in HTML this is not allowed.
                // That's why if we meet a normal row, we consider all rows below it as normal too.
                cellerRow.IsHeading = ((previousCellerRow == null) || previousCellerRow.IsHeading) &&
                    IsHeadingRow(cellerRow.RowNode);

                if (cellerRow.IsHeading)
                {
                    tableHasHeader = true;
                }

                // Please notice that we don't mark rows if the table has no other row groups besides body. As a result,
                // simple tables are exported without explicit row group elements. For example, in HTML most tables will
                // contain <tr> elements directly, without enclosing them into <tbody>.
                if (tableHasHeader &&
                    ((previousCellerRow == null) || (previousCellerRow.IsHeading != cellerRow.IsHeading)))
                {
                    cellerRow.IsFirstInGroup = true;
                }

                previousCellerRow = cellerRow;
            }
        }

        /// <summary>
        /// Indicates whether a table row is heading.
        /// </summary>
        private static bool IsHeadingRow(Row row)
        {
            // Table row is heading if it has any vertical merged cell with previous heading row.
            bool isHeading = row.TablePr.FinalPr.HeadingFormat;
            if (!isHeading && (row.PreviousRow != null) && row.PreviousRow.TablePr.FinalPr.HeadingFormat)
            {
                Cell cell = row.FirstCell;
                while (cell != null)
                {
                    if (cell.CellPr.VerticalMerge == CellMerge.Previous)
                    {
                        isHeading = true;
                        break;
                    }
                    cell = cell.NextCell;
                }
            }
            return isHeading;
        }

        /// <summary>
        /// Table node being cellerized
        /// </summary>
        private readonly Table mTableNode;

        /// <summary>
        /// Option to resolve inherited borders on cells.
        /// </summary>
        private readonly bool mResolveInheritedBorders;

        /// <summary>
        /// Option to populate empty borders to pad cells.
        /// Needed when table borders are specified.
        /// </summary>
        private readonly bool mPopulateEmptyPadBorders;

        /// <summary>
        /// Option to resolve inherited paddings on cells.
        /// </summary>
        private readonly bool mResolveInheritedPaddings;

        /// <summary>
        /// Specifies if celler should visit merged cells when move to next cell.
        /// </summary>
        private readonly bool mGotoMergedCells;

        /// <summary>
        /// These several indidates helps populate empty pad borders only where they are needed.
        /// </summary>
        private bool mTableLeftVisible;
        private bool mTableRightVisible;
        private bool mTableTopVisible;
        private bool mTableBottomVisible;
        private bool mTableHorizontalVisible;
        private bool mTableVerticalVisible;
        private Border mEmptyBorder;
        /// <summary>
        /// Keys are sorted column positions for all rows of the table.
        /// </summary>
        private readonly SortedIntegerListGeneric<object> mColumnPositions = new SortedIntegerListGeneric<object>();
        private readonly List<CellerRow> mRows = new List<CellerRow>();
        private int mCurRowIdx;
        private int mCurColIdx;
        private bool mHasRowWithoutMisalignedColumns;
        private bool mHasPadCells;
    }
}
