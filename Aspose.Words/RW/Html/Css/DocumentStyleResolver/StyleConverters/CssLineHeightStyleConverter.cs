// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to apply CSS line-height style to a model.
    /// </summary>
    internal static class CssLineHeightStyleConverter
    {
        /// <summary>
        /// Applies CSS line-height style to a model paragraph format.
        /// </summary>
        internal static void ToParagraphFormat(
            CssDeclarationCollection declarations,
            ParagraphFormat paragraphFormat,
            bool applyFormattingAsMsWord)
        {
            CssDeclaration lineHeightDeclaration = declarations["line-height"];
            if (lineHeightDeclaration == null)
                return;
            if (lineHeightDeclaration.Value.Count != 1)
                return;

            CssComputedDeclaration computedLineHeight = lineHeightDeclaration as CssComputedDeclaration;
            bool isPercentageSpecified = (computedLineHeight != null) &&
                (computedLineHeight.OriginalSpecifiedValue.FirstValue.ValueType == CssValueType.Percentage);
            if (isPercentageSpecified)
            {
                // Percentage is computed into absolute length during style resolution but it has a special meaning for HTML
                // import, because we convert percentage into line height multiplicator. Thus we need to know whether
                // the 'line-height' value was specified in percentage terms.
                // Because of this behavior, line heights in imported documents differ from that of documents rendered in
                // browsers. However, MS Word produces the same results. See WORDSNET-11773
                CssValue specifiedValue = computedLineHeight.OriginalSpecifiedValue.FirstValue;
                paragraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                double lines = ((CssPercentageValue)specifiedValue).DoubleValue / 100;
                // MS Word has a little bit another rounding for the line spacing value.
                if (applyFormattingAsMsWord)
                {
                    int twips = (int)(ConvertUtilCore.LineToPoint(lines) * ConvertUtilCore.TwipsPerPoint);
                    paragraphFormat.LineSpacing = ConvertUtilCore.TwipToPoint(twips);
                }
                else
                {
                    paragraphFormat.LineSpacing = ConvertUtilCore.LineToPoint(lines);
                }
            }
            else
            {
                CssValue cssValue = lineHeightDeclaration.Value.FirstValue;
                switch (cssValue.ValueType)
                {
                    case CssValueType.Percentage:
                    {
                        // Percentage must be computed into absolute length during style resolution, and we should
                        // never get here.
                        Debug.Assert(false);
                        break;
                    }
                    case CssValueType.Number:
                    {
                        double numberValue = ((CssNumberValue)cssValue).DoubleValue;
                        if (numberValue > 0)
                        {
                            CssDeclaration fontSizeDeclaration = declarations["font-size"];
                            double fontSize = (fontSizeDeclaration != null)
                                ? CssUtil.LengthToPoint(fontSizeDeclaration.Value)
                                : CssUtil.DefaultFontSize;
                            if (!MathUtil.AreEqual(fontSize, double.MinValue))
                            {
                                paragraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                                paragraphFormat.LineSpacing = fontSize * numberValue;
                            }
                        }
                        break;
                    }
                    default:
                    {
                        double length = CssUtil.LengthToPoint(cssValue);
                        if (!MathUtil.AreEqual(length, double.MinValue))
                        {
                            paragraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                            paragraphFormat.LineSpacing = length;
                        }
                        break;
                    }
                }
            }
        }
    }
}
