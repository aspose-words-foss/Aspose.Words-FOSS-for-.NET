// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for 'border-top-width', 'border-right-width', 'border-bottom-width', 
    /// 'border-left-width' CSS properties. 
    /// </summary>
    /// <remarks>CSS border width is applied to a model format with <see cref="CssBorderStyleConverter"/></remarks>
    internal abstract class CssBorderWidthPropertyDefBase : CssIndividualSimplePropertyDef
    {
        protected CssBorderWidthPropertyDefBase(string name)
            : base(
                name,
                false,
                CssValue.Medium,
                // thin | medium | thick | <length>
                CssValueFilter.AnyOf(
                    CssValueFilter.Values(
                        CssValue.Thin,
                        CssValue.Medium,
                        CssValue.Thick),
                    CssValueFilter.NonNegativeQuirkyLength))
        {
            // Empty constructor.
        }
    }
}
