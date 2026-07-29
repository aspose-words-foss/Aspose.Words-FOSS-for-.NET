// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2016 by Denis Shvydkiy

using Aspose.Fonts.TrueType;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Implements methods responsible for character advance width calculation based on vertical metrics.
    /// </summary>
    public class TTFontVerticalMetrics : TTFontMetrics
    {
        internal TTFontVerticalMetrics(TTFont font, float sizePoints)
            : base(font, sizePoints)
        {
        }

        public override float GetCharWidthPoints(int c, float sizePoints)
        {
            return GetRawCharWidthPoints(c, sizePoints);
        }

        public override float GetRawCharWidthPoints(int c, float sizePoints)
        {
            return Font.DesignUnitsToPoints(Font.GetCharAdvanceHeightDesignUnits(c), sizePoints);
        }
    }
}
