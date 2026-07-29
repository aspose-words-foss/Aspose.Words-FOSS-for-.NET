// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2025 by Konstantin Kornilov

namespace Aspose.Fonts.TrueType
{
    internal class SimpleGlyfPoint
    {
        internal SimpleGlyfPoint(short dx, short dy, bool isOnCurve, bool isEndOfContour)
            : this(dx, dy, isOnCurve, isEndOfContour, false)
        {
        }

        internal SimpleGlyfPoint(short dx, short dy, bool isOnCurve, bool isEndOfContour, bool isOverlap)
        {
            DX = dx;
            DY = dy;
            IsOnCurve = isOnCurve;
            IsEndOfContour = isEndOfContour;
            IsOverlap = isOverlap;
        }

        public short DX { get; set; }

        public short DY { get; set; }

        public bool IsOnCurve { get; }

        public bool IsEndOfContour { get; }

        public bool IsOverlap { get; }
    }
}
