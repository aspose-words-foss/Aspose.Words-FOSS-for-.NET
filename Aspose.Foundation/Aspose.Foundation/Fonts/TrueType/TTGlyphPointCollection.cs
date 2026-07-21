// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/02/2010 by Alexey Noskov

using System;
using System.Collections.Generic;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Class represent set of points, which are used to draw glyph.
    /// </summary>
    internal class TTGlyphPointCollection
    {
        public TTGlyphPointCollection()
        {
            mPoints = new List<TTGlyphPoint>();
            XMin = int.MaxValue;
            XMax = int.MinValue;
            YMin = int.MaxValue;
            YMax = int.MinValue;
        }

        /// <summary>
        /// Retrieves a <see cref="TTGlyphPoint"/> at the given index.
        /// Returns null if index is out of range
        /// </summary>
        /// <param name="index">An index into the collection of points.</param>
        public TTGlyphPoint this[int index]
        {
            get { return (index >= 0 && index < mPoints.Count) ? mPoints[index] : null; }
        }

        /// <summary>
        /// Adds a new point.
        /// </summary>
        /// <param name="x">Absolute X coordinate.</param>
        /// <param name="y">Absolute Y coordinate.</param>
        /// <param name="isOnCurve">Flag indicates whether point is on curve.</param>
        /// <param name="isEndOfContour">Flag indicates whether point is end of curve.</param>
        public void Add(int x, int y, bool isOnCurve, bool isEndOfContour)
        {
            TTGlyphPoint point = new TTGlyphPoint(x, y, isOnCurve, isEndOfContour);
            // WORDSNET-25135 There should not be two end of contour points. If such case occurs just ignore the second end of contour point.
            if (LastPoint != null && LastPoint.IsEndOfContour && point.IsEndOfContour)
                return;

            mPoints.Add(point);
            XMin = Math.Min(x, XMin);
            XMax = Math.Max(x, XMax);
            YMin = Math.Min(y, YMin);
            YMax = Math.Max(y, YMax);
        }

        public void TranslatePoints(int xOffset, int yOffset)
        {
            foreach (TTGlyphPoint point in mPoints)
                point.Translate(xOffset, yOffset);
        }

        public int Count
        {
            get { return mPoints.Count; }
        }

        private TTGlyphPoint LastPoint
        {
            get { return this[mPoints.Count - 1]; }
        }

        public int XMin { get; private set; }
        public int YMin { get; private set; }
        public int XMax { get; private set; }
        public int YMax { get; private set; }

        private readonly List<TTGlyphPoint> mPoints;
    }
}
