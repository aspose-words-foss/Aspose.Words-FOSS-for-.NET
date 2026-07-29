// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'margin' shorthand CSS property.
    /// </summary>
    /// <remarks>
    /// The top, right, bottom, and left margin can be changed independently using separate properties. 
    /// A shorthand margin property can also be used, to change all margins at once.
    /// Syntax:
    ///   margin: margin-width{1,4} | inherit
    /// </remarks>
    internal class CssMarginPropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssMarginPropertyDef()
            : base(
                  "margin",
                  "margin-top",
                  "margin-right",
                  "margin-bottom",
                  "margin-left")
        {
            // Empty constructor.
        }
    }
}
