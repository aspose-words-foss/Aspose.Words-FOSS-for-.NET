// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2016 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'min-height' CSS property.
    /// </summary>
    internal class CssMinHeightPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssMinHeightPropertyDef() :
            base("min-height", false, CssValue.Zero,
                // <length> | <percentage>
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage))
        {
            // Empty constructor.
        }
    }
}
