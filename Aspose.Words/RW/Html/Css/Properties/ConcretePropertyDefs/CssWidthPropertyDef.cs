// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'width' CSS property.
    /// </summary>
    internal class CssWidthPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssWidthPropertyDef()
            : base(
                "width",
                false,
                CssValue.Auto,
                // <length> | <percentage> | auto
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage,
                    CssValueFilter.Values(CssValue.Auto)))
        {
            // Empty constructor.
        }

        internal override void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
            Debug.Assert(propertyValue.Count == 1);

            PreferredWidth preferredWidth = CssWidthStyleConverter.GetPreferredWidth(propertyValue);
            if (preferredWidth != null)
                cellFormat.PreferredWidth = preferredWidth;
        }

        internal override void ToHorizontalRule(CssPropertyValue propertyValue, Shape horizontalRuleShape)
        {
            Debug.Assert(propertyValue.Count == 1);

            PreferredWidth preferredWidth = CssWidthStyleConverter.GetPreferredWidth(propertyValue);
            if (preferredWidth == null)
                return;

            switch (preferredWidth.Type)
            {
                case PreferredWidthType.Auto:
                {
                    // Ignore auto value.
                    break;
                }
                case PreferredWidthType.Percent:
                {
                    Section section = (Section)horizontalRuleShape.GetAncestor(NodeType.Section);
                    Debug.Assert(section != null);
                    if (preferredWidth.Value >= 0)
                    {
                        horizontalRuleShape.HorizontalRule.SetPercentSafe(preferredWidth.Value);
                        horizontalRuleShape.SetWidthSafe(section.PageSetup.ContentWidth * horizontalRuleShape.HorizontalRule.Percent / 100.0);
                    }
                    break;
                }
                case PreferredWidthType.Points:
                {
                    if (preferredWidth.Value >= 0)
                    {
                        horizontalRuleShape.SetWidthSafe(preferredWidth.Value);
                        horizontalRuleShape.HorizontalRule.Percent = 0;
                    }
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }
        }
    }
}
