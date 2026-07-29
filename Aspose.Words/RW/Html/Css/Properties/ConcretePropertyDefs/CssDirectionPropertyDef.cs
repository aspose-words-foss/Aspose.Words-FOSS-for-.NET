// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'direction' CSS property.
    /// </summary>
    internal class CssDirectionPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssDirectionPropertyDef()
            : base(
                "direction",
                true,
                CssValue.Ltr,
                CssValueFilter.Values(CssValue.Ltr, CssValue.Rtl))
        {
            // Empty constructor.
        }
    }
}
