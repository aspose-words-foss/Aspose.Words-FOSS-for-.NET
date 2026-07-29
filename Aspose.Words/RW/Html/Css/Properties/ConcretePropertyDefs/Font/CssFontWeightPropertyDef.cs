// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'font-weight' CSS property.
    /// </summary>
    internal class CssFontWeightPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFontWeightPropertyDef()
            : base(
                "font-weight",
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
