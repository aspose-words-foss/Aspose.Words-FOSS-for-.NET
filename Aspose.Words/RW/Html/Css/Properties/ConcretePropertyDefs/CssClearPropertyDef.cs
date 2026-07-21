// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'clear' CSS property.
    /// </summary>
    internal class CssClearPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssClearPropertyDef()
            : base(
                "clear",
                false,
                CssValue.None,
                // none | left | right | both
                CssValueFilter.Values(CssValue.None, CssValue.Left, CssValue.Right, CssValue.Both))
        {
            // Empty constructor.
        }
    }
}
