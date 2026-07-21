// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2006 by Roman Korchagin

using System.Drawing;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies a two dimensional point of a shape geometry.
    /// Coordinates can contain references to shape formulas.
    /// 
    /// 2.2.55 POINT in the DOC SPEC.
    /// </summary>
    internal class PathPoint
    {
        internal PathPoint() : this(Point.Empty)
        {
        }

        /// <summary>
        /// Creates a path point with values that are not formulas.
        /// </summary>
        internal PathPoint(int x, int y) : 
            this(new Point(x, y))
        {
        }

        /// <summary>
        /// Creates a path point with values that are not formulas.
        /// </summary>
        internal PathPoint(Point point) : 
            this(new PathValue(point.X, false), new PathValue(point.Y, false))
        {
        }

        /// <summary>
        /// Creates a path point with values that could be coordinates or formulas.
        /// </summary>
        internal PathPoint(PathValue x, PathValue y)
        {
            mX = x;
            mY = y;
        }

        /// <summary>
        /// The X value of the point.
        /// The coordinate system used for this value is dependent on the scenario in which it is used.
        /// </summary>
        internal PathValue X
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mX; }
        }

        /// <summary>
        /// The Y value of the point.
        /// The coordinate system used for this value is dependent on the scenario in which it is used.
        /// </summary>
        internal PathValue Y
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mY; }
        }

        public override string ToString()
        {
            return string.Format("X:{0}, Y:{1}", X.ToString(), Y.ToString());
        }

        private readonly PathValue mX;
        private readonly PathValue mY;
    }
}
