// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2013 by Alexey Butalov

using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS style to a model horizontal rule noshade property.
    /// </summary>
    internal static class CssHRuleNoShadeStyleConverter
    {
        /// <summary>
        /// Aapplies CSS style to a model horizontal rule noshade property.
        /// </summary>
        internal static void ToHorizontalRule(CssDeclarationCollection declarations, Shape horizontalRule)
        {
            // Detect noshade property.
            CssDeclaration backgroundColor = declarations["background-color"];
            CssDeclaration borderTopColor = declarations["border-top-color"];
            CssDeclaration borderRightColor = declarations["border-right-color"];
            CssDeclaration borderBottomColor = declarations["border-bottom-color"];
            CssDeclaration borderLeftColor = declarations["border-left-color"];
            if ((backgroundColor != null) && (borderTopColor != null) && (borderRightColor != null) &&
                (borderBottomColor != null) && (borderLeftColor != null) &&
                backgroundColor.Value.Equals(borderTopColor.Value) &&
                backgroundColor.Value.Equals(borderRightColor.Value) &&
                backgroundColor.Value.Equals(borderBottomColor.Value) &&
                backgroundColor.Value.Equals(borderLeftColor.Value))
            {
                horizontalRule.HorizontalRule.NoShade = true;
            }

            CssDeclaration borderTopStyleDeclaration = declarations["border-top-style"];
            CssDeclaration borderRightStyleDeclaration = declarations["border-right-style"];
            CssDeclaration borderBottomStyleDeclaration = declarations["border-bottom-style"];
            CssDeclaration borderLeftStyleDeclaration = declarations["border-left-style"];
            if ((borderTopStyleDeclaration != null) && borderTopStyleDeclaration.Value.Equals(CssValue.None) &&
                (borderRightStyleDeclaration != null) && borderRightStyleDeclaration.Value.Equals(CssValue.None) &&
                (borderBottomStyleDeclaration != null) && borderBottomStyleDeclaration.Value.Equals(CssValue.None) &&
                (borderLeftStyleDeclaration != null) && borderLeftStyleDeclaration.Value.Equals(CssValue.None))
            {
                horizontalRule.HorizontalRule.NoShade = true;
            }
        }
    }
}