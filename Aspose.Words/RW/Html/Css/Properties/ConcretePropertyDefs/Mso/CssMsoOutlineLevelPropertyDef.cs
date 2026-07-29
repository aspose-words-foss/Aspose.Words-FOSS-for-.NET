// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2025 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssMsoOutlineLevelPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMsoOutlineLevelPropertyDef()
            : base(
                  "mso-outline-level",
                  false,
                  CssValue.BodyText,
                  CssValueFilter.AnyOf(
                    CssValueFilter.Value(CssValue.BodyText),
                    CssValueFilter.NonNegativeNumber))
        {
            // Empty constructor.
        }
    }
}
