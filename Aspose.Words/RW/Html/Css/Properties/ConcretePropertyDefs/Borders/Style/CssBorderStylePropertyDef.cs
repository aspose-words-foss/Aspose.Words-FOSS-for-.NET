// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'border-style' CSS property. 
    /// The 'border-style' property sets the style of the four borders. It can have from one to four component values, and the values are set on the different sides.
    /// </summary>
    /// <remarks>
    /// Syntax:
    ///      border-color: border-style{1,4} | inherit
    /// </remarks>
    internal class CssBorderStylePropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssBorderStylePropertyDef()
            : base(
                  "border-style",
                  "border-top-style",
                  "border-right-style",
                  "border-bottom-style",
                  "border-left-style")
        {
            // Empty constructor.
        }
    }
}
