// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2013 by Alexey Butalov

using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS height to a model horizontal rule height.
    /// </summary>
    internal static class CssHRuleHeightStyleConverter
    {
        /// <summary>
        /// Applies CSS height to a model horizontal rule height.
        /// </summary>
        internal static void ToHorizontalRule(CssDeclarationCollection declarations, Shape horizontalRule)
        {
            double height = 0;
            CssDeclaration heightDeclaration = declarations["height"];
            if (heightDeclaration != null)
            {
                height = CssUtil.LengthToPoint(heightDeclaration.Value);
                if (MathUtil.IsMinValue(height))
                    height = 0;
            }

            // Correct shape height with HTML borders.
            CssDeclaration borderTopStyleDeclaration = declarations["border-top-style"];
            CssDeclaration borderBottomStyleDeclaration = declarations["border-bottom-style"];

            double borderTopWidthPt = 0;
            double borderBottomWidthPt = 0;
            if ((borderTopStyleDeclaration != null) && !borderTopStyleDeclaration.Value.Equals(CssValue.None))
            {
                CssDeclaration borderTopWidthDeclaration = declarations["border-top-width"];
                if (borderTopWidthDeclaration != null)
                    borderTopWidthPt = CssUtil.GetBorderLineWidth(borderTopWidthDeclaration.Value);
            }
            if ((borderBottomStyleDeclaration != null) && !borderBottomStyleDeclaration.Value.Equals(CssValue.None))
            {
                CssDeclaration borderBottomWidthDeclaration = declarations["border-bottom-width"];
                if (borderBottomWidthDeclaration != null)
                    borderBottomWidthPt = CssUtil.GetBorderLineWidth(borderBottomWidthDeclaration.Value);
            }
            horizontalRule.SetHeightSafe(height + borderTopWidthPt + borderBottomWidthPt);
        }
    }
}
