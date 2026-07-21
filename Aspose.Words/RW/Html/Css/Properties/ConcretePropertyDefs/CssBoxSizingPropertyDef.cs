// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/06/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'box-sizing' CSS property.
    /// </summary>
    /// <remarks>
    /// The property is a part of CSS 3. Specification: http://www.w3.org/TR/css3-ui/#box-sizing
    /// </remarks>
    internal class CssBoxSizingPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBoxSizingPropertyDef()
            : base(
                "box-sizing",
                true,
                CssValue.Separate,
                // content-box | padding-box | border-box
                CssValueFilter.Values(CssValue.ContentBox, CssValue.PaddingBox, CssValue.BorderBox))
        {
            // Empty constructor.
        }
    }
}
