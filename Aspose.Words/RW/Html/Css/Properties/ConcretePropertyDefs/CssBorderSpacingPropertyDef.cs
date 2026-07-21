// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'border-spacing' CSS property.
    /// </summary>
    internal class CssBorderSpacingPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBorderSpacingPropertyDef()
            : base(
                "border-spacing",
                true,
                CssValue.Zero,
                // <length>
                CssValueFilter.NonNegativeQuirkyLength)
        {
            // Empty constructor.
        }
    }
}
