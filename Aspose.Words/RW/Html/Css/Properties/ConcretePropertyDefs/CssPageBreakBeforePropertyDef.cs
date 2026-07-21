// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'page-break-before' CSS property.
    /// </summary>
    internal class CssPageBreakBeforePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssPageBreakBeforePropertyDef()
            : base(
                "page-break-before",
                false,
                CssValue.Auto,
                CssValueFilter.Values(
                    CssValue.Auto,
                    CssValue.Always,
                    CssValue.Avoid,
                    CssValue.Left,
                    CssValue.Right))
        {
            // Empty constructor.
        }
    }
}
