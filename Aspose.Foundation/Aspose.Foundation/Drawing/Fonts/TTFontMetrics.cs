// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2016 by Denis Shvydkiy

using System;
using Aspose.Bidi;
using Aspose.Fonts.TrueType;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Implements IDrFontMetrics and provides metrics from TTFont class.
    /// </summary>
    public abstract class TTFontMetrics : IDrFontMetrics
    {
        protected TTFontMetrics(TTFont font, float sizePoints, bool adjustCjkFontMetrics, bool useWord97FontMetrics)
        {
            Font = font;

            float ascent = font.Ascent;
            float descent = font.Descent;
            float lineSpacing = font.LineSpacing;

            UseWord97FontMetrics = useWord97FontMetrics;

            if (useWord97FontMetrics)
            {
                float externalLeading = lineSpacing - (ascent + descent);
                externalLeading = RoundLikeWord97(externalLeading, Font.EmHeight);
                ascent = RoundLikeWord97(ascent, Font.EmHeight);
                descent = RoundLikeWord97(descent, Font.EmHeight);
                lineSpacing = ascent + descent + externalLeading;
            }

            SetOriginalMetrics(ascent, descent, sizePoints);

            if (adjustCjkFontMetrics && font.IsCjkMetrics)
                AdjustAndSetMetrics(ascent, descent, sizePoints);
            else
                SetMetrics(ascent, descent, lineSpacing, sizePoints);
        }

        public abstract float GetCharWidthPoints(int c, float sizePoints);

        public abstract float GetRawCharWidthPoints(int c, float sizePoints);

        public float GetTextWidthPoints(string text, float sizePoints)
        {
            float result = 0;
            // Performance optimization. Use the field instead of creating a new StringUtf32Enumerator object
            // to reduce memory allocations and fragmentation.
            mStringEnumerator.SetText(text);
            foreach (int c in mStringEnumerator)
                result += GetCharWidthPoints(c, sizePoints);

            return result;
        }

        public float AscentPoints { get; set; }

        public float DescentPoints { get; set; }

        public float LineSpacingPoints { get; set; }

        public float AscentRawPoints { get; set; }

        public float DescentRawPoints { get; set; }

        private void AdjustAndSetMetrics(float ascent, float descent, float sizePoints)
        {
            // WORDSNET-687 It seems that MS Word uses increased line spacing for fonts with CJK support.
            // These computations are taken from OpenOffice sources (vcl/win/source/gdi/salgdi3.cxx file)
            // and some experiments. I'm not sure what does they mean but seems to work.
            int externalLeading = (int)(ascent + descent - Font.EmHeight);
            int nHalfTmpExtLeading = externalLeading / 2;
            int nOtherHalfTmpExtLeading = externalLeading - nHalfTmpExtLeading;
            int cjkExternalLeading = (int)(0.30 * (ascent + descent)) - externalLeading;

            externalLeading = Math.Max(cjkExternalLeading, 0);

            float adjustedAscent = ascent + nHalfTmpExtLeading;
            float adjustedDescent = descent + nOtherHalfTmpExtLeading;
            float lineSpacing = adjustedAscent + adjustedDescent + externalLeading;

            // With line spacing increased, Word positions baseline higher for some CJK fonts.
            // The doubling of the descent below helps get baseline close to Word's, though not identical.
            // See TestAsian.TestIdeographicBaseline for examples.
            int doubledDescent = (int)(adjustedDescent * 2);
            if (doubledDescent + adjustedAscent <= lineSpacing)
                adjustedDescent = doubledDescent;

            SetMetrics(adjustedAscent, adjustedDescent, lineSpacing, sizePoints);
        }

        private void SetMetrics(float ascent, float descent, float lineSpacing, float sizePoints)
        {
            AscentPoints = Font.DesignUnitsToPoints(ascent, sizePoints);
            DescentPoints = Font.DesignUnitsToPoints(descent, sizePoints);
            LineSpacingPoints = Font.DesignUnitsToPoints(lineSpacing, sizePoints);
        }

        private void SetOriginalMetrics(float ascent, float descent, float sizePoints)
        {
            AscentRawPoints = Font.DesignUnitsToPoints(ascent, sizePoints);
            DescentRawPoints = Font.DesignUnitsToPoints(descent, sizePoints);
        }

        private static float RoundLikeWord97(float val, int emHeight)
        {
            float roundedVal = (float)Math.Round(val * EmHeightWord97 / emHeight, MidpointRounding.AwayFromZero);
            return roundedVal * emHeight / EmHeightWord97;
        }

        protected static float DesignUnitsToPointsLikeWord97(float charWidthDesignUnits, int emHeight, float sizePoints)
        {
            float roundedVal = (float)Math.Round((charWidthDesignUnits * EmHeightWord97 / emHeight), MidpointRounding.AwayFromZero) * sizePoints / EmHeightWord97;
            return (float)((int)(roundedVal * ConvertUtilCore.WordLisPerPoint)) / ConvertUtilCore.WordLisPerPoint;
        }

        protected TTFont Font { get; }
        protected bool UseWord97FontMetrics { get; }

        private readonly StringUtf32Enumerator mStringEnumerator = new StringUtf32Enumerator(string.Empty);

        private const float EmHeightWord97 = 1000f;
    }
}
