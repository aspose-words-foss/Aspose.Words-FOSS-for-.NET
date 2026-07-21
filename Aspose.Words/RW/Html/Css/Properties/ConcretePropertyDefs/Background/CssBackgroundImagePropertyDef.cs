// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'background-image' CSS property.
    /// </summary>
    internal class CssBackgroundImagePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBackgroundImagePropertyDef()
            : base(
                "background-image",
                false,
                CssValue.None,
                // <uri> | none
                CssValueFilter.AnyOf(
                    CssValueFilter.Uri,
                    CssValueFilter.Value(CssValue.None)))
        {
            // Empty constructor.
        }
    }
}
