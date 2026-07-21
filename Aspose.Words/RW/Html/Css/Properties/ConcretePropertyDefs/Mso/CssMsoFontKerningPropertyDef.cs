// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssMsoFontKerningPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMsoFontKerningPropertyDef()
            : base(
                "mso-font-kerning",
                true,
                null,
                CssValueFilter.AnyOf(
                    CssValueFilter.Value(CssValue.None),
                    CssValueFilter.Length,
                    CssValueFilter.Number))
        {
            // Empty constructor.
        }
    }
}
