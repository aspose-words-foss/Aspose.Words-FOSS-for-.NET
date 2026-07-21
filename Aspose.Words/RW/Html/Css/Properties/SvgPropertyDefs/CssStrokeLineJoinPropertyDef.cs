// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2018 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    internal class CssStrokeLineJoinPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssStrokeLineJoinPropertyDef()
            : base(
                "stroke-linejoin",
                true,
                CssValue.Miter,
                // miter | round | bevel
                CssValueFilter.Values(
                    CssValue.Miter,
                    CssValue.Round,
                    CssValue.Bevel))
        {
            // Nothing to do.
        }
    }
}
