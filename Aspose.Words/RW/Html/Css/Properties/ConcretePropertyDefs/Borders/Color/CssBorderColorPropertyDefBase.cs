// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for 'border-top-color', 'border-right-color', 'border-bottom-color', 
    /// 'border-left-color' CSS properties. 
    /// </summary>
    /// <remarks>CSS border color is applied to a model format with <see cref="CssBorderStyleConverter"/></remarks>
    internal abstract class CssBorderColorPropertyDefBase : CssIndividualSimplePropertyDef
    {
        protected CssBorderColorPropertyDefBase(string name)
            : base(
                name,
                false,
                // WORDSNET-27791 The default value of 'border-xxx-color' properties used to be unspecified but was changed
                // to 'currentcolor'. Not only this complies with the CSS standard but also allows border color values to be
                // correctly overriden by other declarations (such as inline styles or the 'bordercolor' attribute).
                CssValue.CurrentColor,
                // Note that we don't accept 'currentcolor' as a property value in HTML. This is not fully implemented yet.
                // <color> | transparent
                CssValueFilter.AnyOf(
                    CssValueFilter.Color,
                    CssValueFilter.Value(CssValue.Transparent)))
        {
            // Empty constructor.
        }
    }
}
