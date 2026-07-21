// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'background-attachment' CSS property.
    /// </summary>
    internal class CssBackgroundAttachmentPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBackgroundAttachmentPropertyDef()
            : base(
                "background-attachment",
                false,
                CssValue.Scroll,
                // scroll | fixed
                CssValueFilter.Values(CssValue.Scroll, CssValue.Fixed))
        {
            // Empty constructor.
        }
    }
}
