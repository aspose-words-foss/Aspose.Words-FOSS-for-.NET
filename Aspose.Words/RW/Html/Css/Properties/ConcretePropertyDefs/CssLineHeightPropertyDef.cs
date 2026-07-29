// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'line-height' CSS property.
    /// </summary>
    /// <remarks>CSS line height is applied to a model format with <see cref="CssLineHeightStyleConverter"/></remarks>
    internal class CssLineHeightPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssLineHeightPropertyDef()
            : base(
                "line-height",
                true,
                CssValue.Normal,
                // normal | <number> | <length> | <percentage>
                CssValueFilter.AnyOf(
                    CssValueFilter.Value(CssValue.Normal),
                    CssValueFilter.NonNegativeNumber,
                    CssValueFilter.NonNegativeLength,
                    CssValueFilter.NonNegativePercentage))
        {
            // Empty constructor.
        }

        protected override CssPropertyValue ToComputedValueCore(CssPropertyValue specifiedValue, CssCascadeContext cssContext)
        {
            if ((specifiedValue.Count == 1) &&
                (specifiedValue.FirstValue.ValueType == CssValueType.Percentage) &&
                !MathUtil.IsZero(cssContext.ElementFontSize))
            {
                CssPercentageValue percentage = (CssPercentageValue)specifiedValue.FirstValue;
                double lineHeightInPoints = cssContext.ElementFontSize * percentage.DoubleValue / 100;
                CssLengthValue computedLineHeight = new CssLengthValue(lineHeightInPoints, CssUnit.Pt);
                specifiedValue = new CssPropertyValue(computedLineHeight);
            }
            return ComputeRelativeLengthValue(specifiedValue, cssContext);
        }
    }
}
