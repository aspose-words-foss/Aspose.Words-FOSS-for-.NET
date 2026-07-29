// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies a single row of a matrix.
    /// </summary>
    internal class MathObjectMatrixRow : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.MatrixRow; }
        }

        internal override bool CanBeArgument
        {
            get { return false; }
        }

        /// <summary>
        /// Flag is used for rendering EQ field overstrike token only.
        /// </summary>
        internal bool IsOverstrikeRow
        {
            get { return mIsOverstrikeRow; }
            set { mIsOverstrikeRow = value; }
        }

        private bool mIsOverstrikeRow;
    }
}
