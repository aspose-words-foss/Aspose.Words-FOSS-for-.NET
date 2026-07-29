// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2016 by Denis Shvydkiy

using Aspose.Bidi;
using Aspose.Fonts.TrueType;

namespace Aspose.Drawing.Fonts
{
    /// <summary>
    /// Implements IDrFontMetrics and provides metrics from TTFont class.
    /// </summary>
    public abstract class TTFontMetrics : IDrFontMetrics
    {
        protected TTFontMetrics(TTFont font, float sizePoints)
        {
            Font = font;

            float ascent = font.Ascent;
            float descent = font.Descent;
            float lineSpacing = font.LineSpacing;

            AscentRawPoints = Font.DesignUnitsToPoints(ascent, sizePoints);
            DescentRawPoints = Font.DesignUnitsToPoints(descent, sizePoints);

            AscentPoints = Font.DesignUnitsToPoints(ascent, sizePoints);
            DescentPoints = Font.DesignUnitsToPoints(descent, sizePoints);
            LineSpacingPoints = Font.DesignUnitsToPoints(lineSpacing, sizePoints);
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

        protected TTFont Font { get; }

        private readonly StringUtf32Enumerator mStringEnumerator = new StringUtf32Enumerator(string.Empty);
    }
}
