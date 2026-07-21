// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2014 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'visibility' CSS property.
    /// </summary>
    internal class CssVisibilityPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssVisibilityPropertyDef()
            : base(
                "visibility",
                true,
                CssValue.Visible,
                CssValueFilter.Values(CssValue.Visible, CssValue.Hidden, CssValue.Collapse))
        {
            // Empty constructor.
        }
    }
}
