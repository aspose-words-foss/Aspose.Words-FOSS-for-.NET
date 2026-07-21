// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'stroke' CSS property for SVG.
    /// </summary>
    internal class CssStrokePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssStrokePropertyDef() 
            : base(
                "stroke",
                true,
                CssValue.None,
                // none | currentColor | <color> [<icccolor>] | <funciri> [none | currentColor | <color> [<icccolor>]]
                // Identifiers are included here for named colors (green, black, etc) and they also cover
                // the "none" and "currentColor" values.
                CssValueFilter.AnyOf(
                    CssValueFilter.Identifier,
                    CssValueFilter.Color,
                    CssValueFilter.Uri))
        {
            // Nothing to do.
        }
    }
}
