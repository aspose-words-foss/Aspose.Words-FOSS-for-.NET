// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2020 by Artem Shabarshin

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements '-aw-border-style' CSS extended border property. 
    /// The '-aw-border-style' property sets the extension style of the four borders. It can have from one to four component values, and the values are set on the different sides.
    /// </summary>
    /// <remarks>
    /// Syntax:
    ///      -aw-border-style{1,4}
    /// </remarks>
    internal class CssAWBorderStylePropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssAWBorderStylePropertyDef()
            : base(
                  HtmlConstants.AsposeBorderStyle,
                  HtmlConstants.AsposeBorderTopStyle,
                  HtmlConstants.AsposeBorderRightStyle,
                  HtmlConstants.AsposeBorderBottomStyle,
                  HtmlConstants.AsposeBorderLeftStyle)
        {
            // Empty constructor.
        }
    }
}
