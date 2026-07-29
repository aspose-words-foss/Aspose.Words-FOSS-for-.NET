// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2018 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'stroke-dashoffset' CSS property for SVG.
    /// </summary>
    internal class CssStrokeDashOffsetPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssStrokeDashOffsetPropertyDef()
            : base(
                "stroke-dashoffset",
                true,
                CssValue.Zero,
                CssValueFilter.AnyOf(
                    CssValueFilter.Number,
                    CssValueFilter.Percentage,
                    CssValueFilter.Length))
        {
            // Nothing to do.
        }
    }
}
