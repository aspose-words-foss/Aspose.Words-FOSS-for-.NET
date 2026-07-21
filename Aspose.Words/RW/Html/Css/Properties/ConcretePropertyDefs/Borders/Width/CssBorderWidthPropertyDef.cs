// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'border-width' CSS property. 
    /// This property is a shorthand property for setting 'border-top-width', 'border-right-width', 
    /// 'border-bottom-width', and 'border-left-width' at the same place in the style sheet.
    /// </summary>
    /// <remarks>
    /// If there is only one component value, it applies to all sides. If there are two values, 
    /// the top and bottom borders are set to the first value and the right and left are set to the second. 
    /// If there are three values, the top is set to the first value, the left and right are set to 
    /// the second, and the bottom is set to the third. If there are four values, they apply to the top, 
    /// right, bottom, and left, respectively.
    /// Syntax:
    ///      border-width: width{1,4} | inherit
    /// </remarks>
    internal class CssBorderWidthPropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssBorderWidthPropertyDef()
            : base(
                  "border-width",
                  "border-top-width",
                  "border-right-width",
                  "border-bottom-width",
                  "border-left-width")
        {
            // Empty constructor.
        }
    }
}
