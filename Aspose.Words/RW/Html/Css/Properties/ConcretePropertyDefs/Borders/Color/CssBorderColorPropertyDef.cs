// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'border-color' CSS property. 
    /// The border color properties specify the color of a box's border.
    /// </summary>
    /// <remarks>
    /// Syntax:
    ///      border-color:     [color|transparent]{1,4}|inherit
    /// </remarks>
    internal class CssBorderColorPropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssBorderColorPropertyDef()
            : base(
                  "border-color",
                  "border-top-color",
                  "border-right-color",
                  "border-bottom-color",
                  "border-left-color")
        {
            // Empty constructor.
        }
    }
}
