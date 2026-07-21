// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2015 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a font shorthand CSS property value.
    /// </summary>
    internal class CssFontPropertyValue : CssPropertyValue
    {
        internal CssFontPropertyValue(
            CssValue fontStyle,
            CssValue fontVariant,
            CssValue fontWeight,
            CssValue fontSize,
            CssValue lineHeight,
            CssValue fontFamily)
            : base(CreateValues(fontStyle, fontVariant, fontWeight, fontSize, lineHeight, fontFamily))
        {
            Debug.Assert(fontSize != null);
            Debug.Assert(fontFamily != null);

            mFontStyle = fontStyle;
            mFontVariant = fontVariant;
            mFontWeight = fontWeight;
            mFontSize = fontSize;
            mLineHeight = lineHeight;
            mFontFamily = fontFamily;
        }

        internal CssFontPropertyValue(
            CssValue fontSize,
            CssValue fontFamily)
            : this(null, null, null, fontSize, null, fontFamily)
        {
            // Empty constructor.
        }

        protected override void ValueToCss(StringBuilder sb)
        {
            const char delimiter = ' ';
            bool delimiterNeeded = false;

            if (mFontStyle != null)
            {
                mFontStyle.ToCss(sb);
                delimiterNeeded = true;
            }

            if (mFontVariant != null)
            {
                if (delimiterNeeded)
                    sb.Append(delimiter);
                mFontVariant.ToCss(sb);
                delimiterNeeded = true;
            }

            if (mFontWeight != null)
            {
                if (delimiterNeeded)
                    sb.Append(delimiter);
                mFontWeight.ToCss(sb);
                delimiterNeeded = true;
            }

            if (delimiterNeeded)
                sb.Append(delimiter);
            mFontSize.ToCss(sb);

            if (mLineHeight != null)
            {
                sb.Append('/');
                mLineHeight.ToCss(sb);
            }

            sb.Append(delimiter);
            mFontFamily.ToCss(sb);
        }

        private static CssValueList CreateValues(
            CssValue fontStyle,
            CssValue fontVariant,
            CssValue fontWeight,
            CssValue fontSize,
            CssValue lineHeight,
            CssValue fontFamily)
        {
            Debug.Assert(fontSize != null);
            Debug.Assert(fontFamily != null);

            CssValueList values = new CssValueList();
            if (fontStyle != null)
                values.Add(fontStyle);
            if (fontVariant != null)
                values.Add(fontVariant);
            if (fontWeight != null)
                values.Add(fontWeight);
            values.Add(fontSize);
            if (lineHeight != null)
                values.Add(lineHeight);
            values.Add(fontFamily);
            return values;
        }

        private readonly CssValue mFontStyle;
        private readonly CssValue mFontVariant;
        private readonly CssValue mFontWeight;
        private readonly CssValue mFontSize;
        private readonly CssValue mLineHeight;
        private readonly CssValue mFontFamily;
    }
}
