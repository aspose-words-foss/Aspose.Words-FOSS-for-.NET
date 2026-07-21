// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2016 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'max-height' CSS property.
    /// </summary>
    internal class CssMaxHeightPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMaxHeightPropertyDef()
            : base(
                "max-height",
                false,
                CssValue.None,
                // <length> | <percentage> | none
                // Note that this property also accepts unitless lengths in the Quirks mode.
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage,
                    CssValueFilter.Value(CssValue.None)))
        {
            // Empty constructor.
        }
    }
}
