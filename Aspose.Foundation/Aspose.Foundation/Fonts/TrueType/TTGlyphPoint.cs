// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/02/2010 by Alexey Noskov

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Class represent one point from set of points, which are used to draw glyph.
    /// </summary>
    public class TTGlyphPoint
    {
        /// <summary>
        /// Creates new instance.
        /// </summary>
        /// <param name="absoluteX">Absolute X coordinate of the point.</param>
        /// <param name="absoluteY">Absolute Y coordinate of the point.</param>
        /// <param name="isOnCurve">Flag indicated whether point is on-curve or off-curve.</param>
        /// <param name="isEndOfContour">Flag indicates whether point is end of contour.</param>
        internal TTGlyphPoint(int absoluteX, int absoluteY, bool isOnCurve, bool isEndOfContour)
        {
            X = absoluteX;
            Y = absoluteY;
            IsOnCurve = isOnCurve;
            IsEndOfContour = isEndOfContour;
        }

        /// <summary>
        /// Translates point to the specified offsets.
        /// Only absolute coordinates are changed deltas remain the same.
        /// </summary>
        internal void Translate(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        /// <summary>
        /// Returns absolute X coordinate of the point.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// Returns absolute Y coordinate of the point.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// Flag indicates whether this is on-curve point or off-curve.
        /// </summary>
        public bool IsOnCurve { get; }

        /// <summary>
        /// Flag indicates whether this is point is last point of contour.
        /// </summary>
        public bool IsEndOfContour { get; }

        /// <summary>
        /// Set this property if point is used as reference point upon converting glyph to path.
        /// </summary>
        public bool IsRefPoint { get; set; }
    }
}
