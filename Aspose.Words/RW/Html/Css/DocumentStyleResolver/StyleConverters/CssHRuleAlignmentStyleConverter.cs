// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2013 by Alexey Butalov

using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS margins to a model horizontal rule alignment.
    /// </summary>
    internal static class CssHRuleAlignmentStyleConverter
    {
        /// <summary>
        /// Applies CSS margins style to a model horizontal rule alignment.
        /// </summary>
        internal static void ToHorizontalRule(CssDeclarationCollection declarations, Shape horizontalRule)
        {
            // Set shape alignment.
            CssDeclaration marginLeft = declarations["margin-left"];
            CssDeclaration marginRight = declarations["margin-right"];
            if ((marginLeft != null) && (marginRight != null))
            {
                if (marginLeft.Value.Equals(CssValue.Auto) && marginRight.Value.Equals(CssValue.Auto))
                    horizontalRule.HorizontalRule.Align = HorizontalRuleAlignment.Center;
                else if (marginLeft.Value.Equals(CssValue.Zero) && marginRight.Value.Equals(CssValue.Auto))
                    horizontalRule.HorizontalRule.Align = HorizontalRuleAlignment.Left;
                else if (marginLeft.Value.Equals(CssValue.Auto) && marginRight.Value.Equals(CssValue.Zero))
                    horizontalRule.HorizontalRule.Align = HorizontalRuleAlignment.Right;
            }
        }
    }
}
