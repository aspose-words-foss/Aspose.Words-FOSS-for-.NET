// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'border-collapse' CSS property.
    /// </summary>
    internal class CssBorderCollapsePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBorderCollapsePropertyDef()
            : base(
                "border-collapse",
                true,
                CssValue.Separate,
                // collapse | separate
                CssValueFilter.Values(CssValue.Collapse, CssValue.Separate))
        {
            // Empty constructor.
        }
    }
}
