// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/10/2016 by Dmitry Sokolov

using Aspose.Words.Drawing.Core.Dml.Path;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Position coordinate.
    /// This class is common for spec parts 20.1.9.17 pos (Shape Position Coordinate) 
    /// and 20.1.9.20 pt (Shape Path Point).
    /// </summary>
    internal class DmlAdjustablePoint
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">The x value. Can be a guide name or a number.</param>
        /// <param name="y">The y.  Can be a guide name or a number.</param>
        internal DmlAdjustablePoint(string x, string y)
        {
            X = new DmlAdjustableCoordinate(x);
            Y = new DmlAdjustableCoordinate(y);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        internal DmlAdjustablePoint(double x, double y)
        {
            X = new DmlAdjustableCoordinate(x);
            Y = new DmlAdjustableCoordinate(y);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal DmlAdjustablePoint()
        {
            X = new DmlAdjustableCoordinate(0);
            Y = new DmlAdjustableCoordinate(0);
        }

        /// <summary>
        /// Clones this instance of <see cref="DmlAdjustablePoint"/>.
        /// </summary>
        internal DmlAdjustablePoint Clone()
        {
            DmlAdjustablePoint lhs = (DmlAdjustablePoint)MemberwiseClone();

            lhs.mX = mX.Clone();
            lhs.mY = mY.Clone();

            return lhs;
        }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        internal DmlAdjustableCoordinate X
        {
            get { return mX; }
            set { mX = (value != null) ? value : new DmlAdjustableCoordinate(0); }
        }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        internal DmlAdjustableCoordinate Y
        {
            get { return mY; }
            set { mY = (value != null) ? value : new DmlAdjustableCoordinate(0); }
        }

        private DmlAdjustableCoordinate mX;
        private DmlAdjustableCoordinate mY;
    }
}
