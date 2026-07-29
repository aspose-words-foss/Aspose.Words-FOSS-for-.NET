// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/04/2020 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'baseline-shift' CSS property for SVG.
    /// </summary>
    internal class CssBaseLineShiftPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBaseLineShiftPropertyDef()
            : base(
                "baseline-shift",
                false,
                CssValue.Zero,
                CssValueFilter.AnyOf(
                    CssValueFilter.Number,
                    CssValueFilter.Length,
                    CssValueFilter.Percentage,
                    CssValueFilter.Values(CssValue.Sub, CssValue.Super)))
        {
            // Empty constructor.
        }
    }
}
