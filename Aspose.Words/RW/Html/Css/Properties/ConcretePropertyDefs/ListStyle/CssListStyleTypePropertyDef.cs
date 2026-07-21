// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'list-style-type' CSS property.
    /// </summary>
    /// <remarks>
    /// We support values specified in CSS 2.1 (see https://www.w3.org/TR/CSS21/generate.html#propdef-list-style-type) and
    /// some values from newer CSS versions (see https://www.w3.org/TR/css-counter-styles-3/#predefined-counters).
    /// </remarks>
    internal class CssListStyleTypePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssListStyleTypePropertyDef()
            : base(
                "list-style-type",
                true,
                CssValue.Disc,
                CssValueFilter.Values(
                    CssValue.Disc,
                    CssValue.Circle,
                    CssValue.Square,
                    CssValue.Decimal,
                    CssValue.DecimalLeadingZero,
                    CssValue.LowerRoman,
                    CssValue.UpperRoman,
                    CssValue.LowerGreek,
                    CssValue.LowerLatin,
                    CssValue.UpperLatin,
                    CssValue.Armenian,
                    CssValue.Georgian,
                    CssValue.ArabicIndic, // WORDSNET-26977 Add support for "arabic-indic" list style.
                    CssValue.LowerAlpha,
                    CssValue.UpperAlpha,
                    CssValue.None))
        {
            // Empty constructor.
        }
    }
}
