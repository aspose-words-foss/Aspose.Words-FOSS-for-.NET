// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'padding' shorthand CSS property.
    /// </summary>
    /// <remarks>
    /// The top, right, bottom, and left padding can be changed independently using separate properties.
    /// A shorthand padding property can also be used, to change all paddings at once.
    /// Syntax:
    ///   padding: padding-width{1,4} | inherit
    /// </remarks>
    internal class CssPaddingPropertyDef : CssShorthand14PropertyDefBase
    {
        internal CssPaddingPropertyDef()
            : base(
                  "padding",
                  "padding-top",
                  "padding-right",
                  "padding-bottom",
                  "padding-left")
        {
            // Empty constructor.
        }
    }
}
