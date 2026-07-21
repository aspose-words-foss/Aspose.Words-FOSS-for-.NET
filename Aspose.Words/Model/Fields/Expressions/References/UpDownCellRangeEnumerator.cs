// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/03/2011 by Dmitry Matveenko
// The class is created to separate up-down moving logic per issue 24122

using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Enumerates cells in a one-dimensional range in the by moving by moving up or down.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cell widths are used to calculate which cells are actually above/below the reference cell, regardless of the cell column index.
    /// </para>
    /// <para>
    /// Thus, for each formula with ABOVE/BELOW range many (maybe all) cells in the table are visited.
    /// If there is more than one such formula, a more effective way would be to calculate the actual cell position for each cell in the table once and remember it.
    /// Then there is no need to re-calculate mostly the same data for each formula.
    /// This was not implemented, however, because I assume that tables with large amount of cells and many above/below formulas are not very common.
    /// Even for such tables it is not clear if cell position re-calculations would be the bottleneck.
    /// So this comment just documents a possible optimization possibility.
    /// </para>
    /// </remarks>
    internal class UpDownCellRangeEnumerator : OneDimensionCellRangeEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        internal UpDownCellRangeEnumerator(OneDimensionCellRange range)
            : base(range)
        {
            const int stepUp = -1;
            const int stepDown = 1;

            switch (range.Direction)
            {
                case CellEnumerationDirection.Up:
                    mStep = stepUp;
                    break;
                case CellEnumerationDirection.Down:
                    mStep = stepDown;
                    break;
                default:
                    ThrowOnInvalidDirection();
                    break;
            }

            mReferenceCellRightEnd = CalculateCellWriteEnd(CurrentPosition.Cell);
        }

        private static double CalculateCellWriteEnd(Cell cell)
        {
            double rightMarginPosition = .0;
            Row parentRow = cell.ParentRow;
            for (int cellColumnIndex = 0; cellColumnIndex <= cell.ColumnIndex; cellColumnIndex++)
            {
                rightMarginPosition += parentRow.Cells[cellColumnIndex].CellFormat.Width;
            }
            return rightMarginPosition;
        }

        /// <summary>
        /// Moves to the next cell in the range.
        /// </summary>
        /// <returns></returns>
        protected override bool MoveToNextCell()
        {
            if (mForceEndOfRange)
            {
                return false;
            }

            // Move to the next row.
            bool isMoveSuccessful = CurrentPosition.MoveDown(mStep);
            if (isMoveSuccessful)
            {
                // Move to the cell which Word considers to be on the same "column" with the referenced cell.
                // If there is no such cell, the range is over.
                isMoveSuccessful = MoveToCellOnTheReferncedColumn();

                if (!isMoveSuccessful)
                {
                    // However, if the row is right above/below the referenced row...
                    bool isNextToTheReferenceRow =
                        (System.Math.Abs(CurrentPosition.RowIndex - Range.ReferencePosition.RowIndex) == 1);
                    if (isNextToTheReferenceRow)
                    {
                        // ... use the cell with the same column index regardless of the width.
                        // This is kind of weird, but Word behaves this way.
                        CurrentPosition.ColumnIndex = 0;
                        isMoveSuccessful = CurrentPosition.MoveRight(Range.ReferencePosition.ColumnIndex);

                        // The next move should fail unconditionally.
                        mForceEndOfRange = true;
                    }
                }
            }

            return isMoveSuccessful;
        }

        /// <summary>
        /// Moves to the cell in the current row which is on the same "column" with the reference cell.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The cell must be visually above/below the reference cell. Words takes the rightmost cell which has a portion above the reference cell.
        /// In this table, cell 2 will be taken for SUM(ABOVE):
        /// |  1         | 2  |
        /// | =SUM(ABOVE) | 0 |
        /// </para>
        /// </remarks>
        /// <returns></returns>
        private bool MoveToCellOnTheReferncedColumn()
        {
            // Get the cells in the current row.
            CellCollection rowCells =
                Range.ReferencePosition.Cell.ParentTable.Rows[CurrentPosition.RowIndex].Cells;

            // Get the first cell that ends as the reference cell, or righter.
            double rowCellStart = .0;
            int cellColumnIndex = 0;
            bool isValidCellFound = false;
            while (cellColumnIndex < rowCells.Count)
            {
                // The right end of the current cell.
                double rowCellEnd = rowCellStart + rowCells[cellColumnIndex].CellFormat.Width;

                // WORDSNET-14013 Compare double values with small epsilon because of precision error.
                isValidCellFound = (rowCellEnd >= mReferenceCellRightEnd - Epsilon);
                if(isValidCellFound)
                {
                    break;
                }
                else
                {
                    // Move to the next cell.
                    rowCellStart = rowCellEnd;
                    cellColumnIndex++;
                }
            }

            // If the table is jagged, cell above/below may not be found in the given row.
            if(isValidCellFound)
            {
                CurrentPosition = new CellPosition(rowCells[cellColumnIndex]);
            }
            return isValidCellFound;
        }

        /// <summary>
        /// Stores 1 if the iterator moves down the tables rows or -1 if it moves up.
        /// </summary>
        private readonly int mStep;
        /// <summary>
        /// Stores the right margin of the reference cell.
        /// </summary>
        private readonly double mReferenceCellRightEnd;
        /// <summary>
        /// A boolean value, indicating whether tne next move should fail unconditionally.
        /// </summary>
        /// <remarks>
        /// It is used when for a jagged table we must return a cell on the current move, but will ignore all the subsequent rows above/below.
        /// </remarks>
        private bool mForceEndOfRange;

        private const double Epsilon = 1E-13;
    }
}
