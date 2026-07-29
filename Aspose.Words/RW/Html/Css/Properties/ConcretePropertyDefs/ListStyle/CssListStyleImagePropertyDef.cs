// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/02/2014 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'list-style-image' CSS property.
    /// </summary>
    internal class CssListStyleImagePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssListStyleImagePropertyDef()
            : base(
                "list-style-image",
                true,
                CssValue.None,
                CssValueFilter.AnyOf(
                    CssValueFilter.Value(CssValue.None),
                    CssValueFilter.Uri))
        {
            // Empty constructor.
        }
    }
}
