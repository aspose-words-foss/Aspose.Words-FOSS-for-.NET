// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2016 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'min-width' CSS property.
    /// </summary>
    internal class CssMinWidthPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMinWidthPropertyDef()
            : base(
                "min-width",
                false,
                CssValue.Zero,
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage))
        {
            // Empty constructor.
        }
    }
}
