// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'caption-side' CSS property.
    /// </summary>
    internal class CssCaptionSidePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssCaptionSidePropertyDef()
            : base(
                "caption-side",
                true,
                CssValue.Top,
                CssValueFilter.Values(CssValue.Top, CssValue.Bottom))
        {
            // Empty constructor.
        }
    }
}
