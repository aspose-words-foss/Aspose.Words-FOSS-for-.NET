// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/04/2020 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'dominant-baseline' CSS property for SVG.
    /// </summary>
    internal class CssDominantBaseLinePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssDominantBaseLinePropertyDef()
            : base(
                "dominant-baseline",
                true,
                CssValue.Auto,
                // auto | text-bottom | alphabetic | ideographic | middle | central | mathematical | hanging | text-top
                CssValueFilter.Values(
                    CssValue.Auto,
                    CssValue.TextBottom,
                    CssValue.Alphabetic,
                    CssValue.Ideographic,
                    CssValue.Middle,
                    CssValue.Central,
                    CssValue.Mathematical,
                    CssValue.Hanging,
                    CssValue.TextTop))
        {
            // Empty constructor.
        }
    }
}
