// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'text-transform' CSS property.
    /// </summary>
    internal class CssTextTransformPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssTextTransformPropertyDef()
            : base(
                "text-transform",
                true,
                CssValue.None,
                // capitalize | uppercase | lowercase | none
                CssValueFilter.Values(
                    CssValue.Capitalize,
                    CssValue.UpperCase,
                    CssValue.LowerCase,
                    CssValue.None))
        {
            // Empty constructor.
        }
    }
}
