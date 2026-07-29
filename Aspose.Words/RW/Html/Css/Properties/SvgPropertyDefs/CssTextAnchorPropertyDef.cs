// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'text-anchor' CSS property for SVG.
    /// </summary>
    internal class CssTextAnchorPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssTextAnchorPropertyDef()
            : base(
                "text-anchor",
                true,
                CssValue.Start,
                // start | middle | end
                CssValueFilter.Values(
                    CssValue.Start,
                    CssValue.Middle,
                    CssValue.End))
        {
            // Nothing to do.
        }
    }
}
