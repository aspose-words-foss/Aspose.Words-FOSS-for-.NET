// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssMsoBidiLanguagePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMsoBidiLanguagePropertyDef()
            : base(
                "mso-bidi-language",
                true,
                null,
                CssValueFilter.AnyOf(
                    CssValueFilter.Identifier,
                    CssValueFilter.String,
                    CssValueFilter.Hash))
        {
            // Empty constructor.
        }
    }
}
