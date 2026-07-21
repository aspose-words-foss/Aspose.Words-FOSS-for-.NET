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

        internal TTFontVerticalMetrics(TTFont font, float sizePoints, bool useBoldSimulationScale, bool adjustCjkFontMetrics)
            : base(font, sizePoints, adjustCjkFontMetrics, false)
        {
            mUseBoldSimulationScale = useBoldSimulationScale;
        }

        public override float GetRawCharWidthPoints(int c, float sizePoints)
        {
            return Font.DesignUnitsToPoints(Font.GetCharAdvanceHeightDesignUnits(c), sizePoints);
        }

        public override float GetCharWidthPoints(int c, float sizePoints)
        {
            int advanceDesignUnits = Font.GetCharAdvanceHeightDesignUnits(c);
            // WORDSNET-26794 - MW uses 0.98 bold simulation scale for vertical text.
            if (mUseBoldSimulationScale)
                advanceDesignUnits = (int)(advanceDesignUnits * BoldSimulationScale);
            return Font.DesignUnitsToPoints(advanceDesignUnits, sizePoints);
        }

        private readonly bool mUseBoldSimulationScale;

        private const float BoldSimulationScale = 0.98f;
    }
}
