// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2016 by Denis Shvydkiy

using Aspose.Collections;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Implements methods responsible for character advance width calculation based on horizontal metrics.
    /// </summary>
    public class TTFontHorizontalMetrics : TTFontMetrics
    {
        internal TTFontHorizontalMetrics(TTFont font, float sizePoints, bool useBoldSimulationScale, bool adjustCjkFontMetrics, bool useWord97FontMetrics)
            : base(font, sizePoints, adjustCjkFontMetrics, useWord97FontMetrics)
        {
            mIsCourierNewFont = StringUtil.EqualsIgnoreCase(Font.FamilyName, "Courier New");

            // WORDSNET-13358 MW scales bold simulation char advance only for some fonts. It is unclear how
            // MW decides it. Just use the confirmed list of fonts.
            mBoldSimulationScale = useBoldSimulationScale
                ? GetBoldSimulationScale(font.FamilyName)
                : 0f;
        }

        public override float GetCharWidthPoints(int c, float sizePoints)
        {
            return GetRawCharWidthPoints(c, sizePoints) + GetBoldSimulationAdvance(sizePoints);
        }

        private float GetBoldSimulationAdvance(float sizePoints)
        {
            // WORDSNET-14832 Seems that MW uses product of font size as addition to char advance.
            return sizePoints * mBoldSimulationScale;
        }

        public override float GetRawCharWidthPoints(int c, float sizePoints)
        {
            // WORDSNET-15895, 18110: Use zero advance hack until AT supported.
            if (mIsCourierNewFont && CourierNewHackUtil.IsZeroAdvanceChar(c))
                return 0;

            int charWidthDesignUnits =  Font.GetCharAdvanceWidthDesignUnits(c);

            return UseWord97FontMetrics ?
                DesignUnitsToPointsLikeWord97(charWidthDesignUnits, Font.EmHeight, sizePoints) :
                Font.DesignUnitsToPoints(charWidthDesignUnits, sizePoints);
        }

        private readonly bool mIsCourierNewFont;

        static TTFontHorizontalMetrics()
        {
            const double scaleCjk = 0.004;
            gBoldSimulationScale["FangSong"] = scaleCjk;
            gBoldSimulationScale["KaiTi"] = scaleCjk;
            gBoldSimulationScale["MS Gothic"] = scaleCjk;
            gBoldSimulationScale["MS Mincho"] = scaleCjk;
            gBoldSimulationScale["MS PGothic"] = scaleCjk;
            gBoldSimulationScale["MS PMincho"] = scaleCjk;
            gBoldSimulationScale["MS UI Gothic"] = scaleCjk;
            gBoldSimulationScale["NSimSun"] = scaleCjk;
            gBoldSimulationScale["SimHei"] = scaleCjk;
            gBoldSimulationScale["SimSun"] = scaleCjk;
            gBoldSimulationScale["SimSun-ExtB"] = scaleCjk;
            gBoldSimulationScale["Lucida Console"] = 0.0008;
        }

        public static float GetBoldSimulationScale(string fontFamilyName)
        {
            if (!gBoldSimulationScale.ContainsKey(fontFamilyName))
                return 0;

            return (float)gBoldSimulationScale[fontFamilyName];
        }

        private readonly float mBoldSimulationScale;
        private static readonly ObjToDoubleDictionary<string> gBoldSimulationScale = new ObjToDoubleDictionary<string>();
    }
}
