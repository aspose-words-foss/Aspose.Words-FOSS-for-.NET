// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'fill-opacity' CSS property for SVG.
    /// </summary>
    internal class CssFillOpacityPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFillOpacityPropertyDef()
            : base(
                  "fill-opacity",
                  true,
                  new CssNumberValue(1),
                  // <opacity-value>
                  CssValueFilter.Number)
        {
            // Empty constructor.
        }
    }
}
