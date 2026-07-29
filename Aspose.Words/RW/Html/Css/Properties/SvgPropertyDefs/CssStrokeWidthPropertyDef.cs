// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'stroke-width' CSS property for SVG.
    /// </summary>
    internal class CssStrokeWidthPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssStrokeWidthPropertyDef()
            : base(
                  "stroke-width",
                  true,
                  new CssNumberValue(1),
                  // <percentage> | <length>
                  // Note that this is a SVG property so it also accepts unitless lengths (numbers).
                  CssValueFilter.AnyOf(
                      CssValueFilter.Percentage,
                      CssValueFilter.Number,
                      CssValueFilter.Length))
        {
            // Nothing to do.
        }
    }
}
