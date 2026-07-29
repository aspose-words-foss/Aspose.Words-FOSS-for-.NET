// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Alexey Butalov

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'height' CSS property.
    /// </summary>
    internal class CssHeightPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssHeightPropertyDef()
            : base(
                "height",
                false,
                CssValue.Auto,
                // <length> | <percentage> | auto
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage,
                    CssValueFilter.Value(CssValue.Auto)))
        {
            // Empty constructor.
        }

        internal override void ToRow(CssPropertyValue propertyValue, Row row)
        {
            Debug.Assert(propertyValue.Count == 1);

            double height = CssUtil.LengthToPoint(propertyValue);
            if (!MathUtil.AreEqual(height, double.MinValue))
            {
                row.RowFormat.Height = height;
                row.RowFormat.HeightRule = HeightRule.AtLeast;
            }
        }
    }
}
