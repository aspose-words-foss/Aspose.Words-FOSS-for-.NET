// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'font-variant' CSS property.
    /// </summary>
    internal class CssFontVariantPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFontVariantPropertyDef()
            : base(
                "font-variant",
                true,
                CssValue.Normal,
                // normal | small-caps
                CssValueFilter.Values(
                    CssValue.Normal,
                    CssValue.SmallCaps))
        {
            // Empty constructor.
        }
    }
}
