// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssMsoBidiFontStylePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMsoBidiFontStylePropertyDef()
            : base(
                "mso-bidi-font-style",
                true,
                CssValue.Normal,
                // normal | italic | oblique
                CssValueFilter.Values(
                    CssValue.Normal,
                    CssValue.Italic,
                    CssValue.Oblique))
        {
            // Empty constructor.
        }
    }
}
