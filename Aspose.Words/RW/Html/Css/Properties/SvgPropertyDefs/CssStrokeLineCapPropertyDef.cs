// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2018 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    internal class CssStrokeLineCapPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssStrokeLineCapPropertyDef()
            : base(
                "stroke-linecap",
                true,
                CssValue.Butt,
                // butt | round | square
                CssValueFilter.Values(
                    CssValue.Butt,
                    CssValue.Round,
                    CssValue.Square))
        {
            // Nothing to do.
        }
    }
}
