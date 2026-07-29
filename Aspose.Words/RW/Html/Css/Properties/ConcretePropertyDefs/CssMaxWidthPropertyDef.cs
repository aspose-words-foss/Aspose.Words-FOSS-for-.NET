// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2016 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'max-width' CSS property.
    /// </summary>
    internal class CssMaxWidthPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMaxWidthPropertyDef()
            : base(
                "max-width",
                false,
                CssValue.None,
                // <length> | <percentage> | none
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage,
                    CssValueFilter.Value(CssValue.None)))
        {
            // Empty constructor.
        }
    }
}
