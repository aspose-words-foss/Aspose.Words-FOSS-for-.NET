// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'opacity' CSS property for SVG.
    /// </summary>
    internal class CssOpacityPropertyDef : CssIndividualSimplePropertyDef
    {
        // The documentation says that 'opacity' is not inherited by default, 
        // but this workaround ensures satisfactory results on import.
        internal CssOpacityPropertyDef()
            : base("opacity", true, new CssNumberValue(1), CssValueFilter.Number)
        {
            // Nothing to do.
        }
    }
}
