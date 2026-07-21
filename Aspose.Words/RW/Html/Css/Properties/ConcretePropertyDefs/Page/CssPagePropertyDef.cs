// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'page' CSS property.
    /// </summary>
    /// <remarks>
    /// The ‘page’ property is used to specify a particular type of page (called a named page) on which an element must be displayed.
    /// http://www.w3.org/TR/css3-page/#page
    /// </remarks>
    internal class CssPagePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssPagePropertyDef()
            : base(
                "page",
                false,
                CssValue.Auto,
                CssValueFilter.Identifier)
        {
            // Empty constructor.
        }
    }
}
