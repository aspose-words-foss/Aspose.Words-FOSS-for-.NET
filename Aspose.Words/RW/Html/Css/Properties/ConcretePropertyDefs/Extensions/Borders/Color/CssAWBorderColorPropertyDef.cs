// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/09/2025 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements '-aw-border-color' CSS property. 
    /// The border color properties specify the color of a box's border.
    /// </summary>
    /// <remarks>
    /// Syntax:
    ///      -aw-border-color:     [color]{1,4}
    /// </remarks>
    internal class CssAWBorderColorPropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssAWBorderColorPropertyDef()
            : base(
                  HtmlConstants.AsposeBorderColor,
                  HtmlConstants.AsposeBorderTopColor,
                  HtmlConstants.AsposeBorderRightColor,
                  HtmlConstants.AsposeBorderBottomColor,
                  HtmlConstants.AsposeBorderLeftColor)
        {
            // Empty constructor.
        }
    }
}
