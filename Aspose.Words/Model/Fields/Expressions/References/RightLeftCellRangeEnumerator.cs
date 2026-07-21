// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2011 by Dmitry Matveenko
// The class is created to separate right-left moving logic per issue 24122

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Enumerates cells in a one-dimensional range by moving right or left.
    /// </summary>
    internal class RightLeftCellRangeEnumerator : OneDimensionCellRangeEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        internal RightLeftCellRangeEnumerator(OneDimensionCellRange range)
            : base(range)
        {
            const int stepLeft = -1;
            const int stepRight = 1;

            switch (range.Direction)
            {
                case CellEnumerationDirection.Left:
                    mStep = stepLeft;
                    break;
                case CellEnumerationDirection.Right:
                    mStep = stepRight;
                    break;
                default:
                    ThrowOnInvalidDirection();
                    break;
            }
        }

        protected override bool MoveToNextCell()
        {
            return CurrentPosition.MoveRight(mStep);
        }

        /// <summary>
        /// Stores column index step. 1 to move right, -1 to move left.
        /// </summary>
        private readonly int mStep;
    }
}
