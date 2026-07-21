// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'font-style' CSS property.
    /// </summary>
    internal class CssFontStylePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFontStylePropertyDef()
            : base(
                "font-style",
                true,
                CssValue.Normal,
                // normal | italic | oblique
                CssValueFilter.Values(
                    CssValue.Normal,
                    CssValue.Italic,
                    CssValue.Oblique))
        {
            // Empty constructor.
        }
    }
}
