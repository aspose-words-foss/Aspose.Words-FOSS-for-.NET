// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'fill' CSS property for SVG.
    /// </summary>
    internal class CssFillPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFillPropertyDef()
            : base(
                  "fill",
                  true,
                  CssValue.Black,
                  // none | currentColor | <color> [<icccolor>] | <funciri> [none | currentColor | <color> [<icccolor>]]
                  // Identifiers are included here for named colors (green, black, etc).
                  CssValueFilter.AnyOf(
                      CssValueFilter.Color,
                      CssValueFilter.Identifier,
                      CssValueFilter.Uri))
        {
            // Nothing to do.
        }
    }
}
