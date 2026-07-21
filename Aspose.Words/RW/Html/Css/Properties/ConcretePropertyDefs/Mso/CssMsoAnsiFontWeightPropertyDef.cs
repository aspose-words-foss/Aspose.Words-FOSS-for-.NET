// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssMsoAnsiFontWeightPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMsoAnsiFontWeightPropertyDef()
            : base(
                "mso-ansi-font-weight",
                true,
                CssValue.Normal,
                // normal | bold | bolder | lighter | 100 | 200 | 300 | 400 | 500 | 600 | 700 | 800 | 900
                CssValueFilter.Values(
                    CssValue.Normal,
                    CssValue.Bold,
                    CssValue.Bolder,
                    CssValue.Lighter,
                    new CssNumberValue(100),
                    new CssNumberValue(200),
                    new CssNumberValue(300),
                    new CssNumberValue(400),
                    new CssNumberValue(500),
                    new CssNumberValue(600),
                    new CssNumberValue(700),
                    new CssNumberValue(800),
                    new CssNumberValue(900)))
        {
            // Empty constructor.
        }
    }
}
