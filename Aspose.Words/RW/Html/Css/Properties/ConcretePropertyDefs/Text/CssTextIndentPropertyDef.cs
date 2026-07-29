// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'text-indent' CSS property.
    /// </summary>
    internal class CssTextIndentPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssTextIndentPropertyDef()
            : base(
                "text-indent",
                true,
                CssValue.Zero,
                CssValueFilter.AnyOf(
                    CssValueFilter.Percentage,
                    CssValueFilter.QuirkyLength))
        {
            // Empty constructor.
        }
    }
}
