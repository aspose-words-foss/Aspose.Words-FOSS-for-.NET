// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2011 by Dmitry Matveenko
// The class is moved to a separate module to separate up-down and right-left moving logic per issue 24122

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Enumerates cells in the range.
    /// </summary>
    internal abstract class OneDimensionCellRangeEnumerator : IEnumerator<Cell>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        protected OneDimensionCellRangeEnumerator(OneDimensionCellRange range)
        {
            Range = range;
            CurrentPosition = Range.ReferencePosition.Clone();
        }

        public static OneDimensionCellRangeEnumerator CreateInstance(OneDimensionCellRange range)
        {
            OneDimensionCellRangeEnumerator instance = null;
            switch (range.Direction)
            {
                case CellEnumerationDirection.Up:
                case CellEnumerationDirection.Down:
                    instance = new UpDownCellRangeEnumerator(range);
                    break;
                case CellEnumerationDirection.Left:
                case CellEnumerationDirection.Right:
                    instance = new RightLeftCellRangeEnumerator(range);
                    break;
                default:
                    ThrowOnInvalidDirection();
                    break;
            }
            return instance;
        }

        protected static void ThrowOnInvalidDirection()
        {
            throw new InvalidOperationException("Unknown cell enumeration direction.");
        }

        public bool MoveNext()
        {
            bool isMoveSuccessfull;
            // WORDSNET-25839 Ignore merged cells.
            do
            {
                isMoveSuccessfull = MoveToNextCell();

                Current = isMoveSuccessfull ? CurrentPosition.Cell : null;

                // WORDSNET-25839 Ignore merged cells. Move on if the cell is merged.
            } while (isMoveSuccessfull && IsCellMerged(Current));

            return isMoveSuccessfull;
        }

        /// <summary>
        /// Checks if the cell is merged with a previous cell either vertically or horizontally.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static bool IsCellMerged(Cell cell)
        {
            return (cell.CellPr.VerticalMerge == CellMerge.Previous)
                || (cell.CellPr.HorizontalMerge == CellMerge.Previous);
        }

        protected abstract bool MoveToNextCell();

        public void Reset()
        {
            CurrentPosition = Range.ReferencePosition.Clone();
            Current = null;
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public Cell Current { get; private set; }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        protected readonly OneDimensionCellRange Range;
        protected CellPosition CurrentPosition;
    }
}
