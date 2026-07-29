// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'font-size' CSS property.
    /// </summary>
    internal class CssFontSizePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFontSizePropertyDef()
            : base(
                "font-size",
                true,
                CssValue.Medium,
                // <absolute-size> | <relative-size> | <length> | <percentage> | inherit
                CssValueFilter.AnyOf(
                    CssValueFilter.Values(
                        CssValue.XxSmall,
                        CssValue.XSmall,
                        CssValue.Small,
                        CssValue.Medium,
                        CssValue.XxLarge,
                        CssValue.XLarge,
                        CssValue.Large,
                        CssValue.XxxLarge,
                        CssValue.Larger,
                        CssValue.Smaller),
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage))
        {
            // Empty constructor.
        }

        protected override CssPropertyValue ToComputedValueCore(CssPropertyValue specifiedValue, CssCascadeContext cssContex)
        {
            if (specifiedValue.Count == 1)
            {
                // Replace identifiers (like 'smaller' or 'larger') with equivalent lengths.
                // Also replace percentages with lengths in Em units.
                CssValue cssValue = specifiedValue.FirstValue;
                CssValue equivalentLengthValue = GetEquivalentLengthValue(cssValue);
                if (!ReferenceEquals(equivalentLengthValue, cssValue))
                {
                    specifiedValue = new CssPropertyValue(equivalentLengthValue);
                }

                // Computation of relative 'font-size' values differs from other CSS values. 'em' and 'ex' for this property
                // refer to 'font-size' of the parent element, not to this element itself.
                // Note that this rule also further converts values created for special identifiers like 'smaller' or 'larger'.
                CssLengthValue valueAsLength = specifiedValue.FirstValue as CssLengthValue;
                if ((valueAsLength != null) && valueAsLength.IsRelative)
                {
                    double absoluteLength = CssUtil.RelativeLengthToPoint(
                        valueAsLength,
                        cssContex.RootElementDeclarations,
                        CssUtil.ComputeFontSize(cssContex.ParentElementDeclarations));
                    if (!MathUtil.AreEqual(absoluteLength, double.MinValue))
                    {
                        specifiedValue =
                            new CssPropertyValue(new CssLengthValue(absoluteLength, CssUnit.Pt));
                    }
                }
            }

            return specifiedValue;
        }

        /// <summary>
        /// Gets a length value (absolute or relative) equivalent to the specified value if possible.
        /// </summary>
        /// <returns>
        /// If the specified value is an identifier, the method returns a length value equivalent to that identifier.
        /// If the specified value is a percentage, the method returns a value in Em units equivalent to the percentage.
        /// Otherwise, the method does nothing and returns the specified value.
        /// </returns>
        private static CssValue GetEquivalentLengthValue(CssValue value)
        {
            CssValue result = value;
            switch (value.ValueType)
            {
                case CssValueType.Identifier:
                {
                    double fontSize = -1;
                    if (value.Equals(CssValue.XxSmall))
                    {
                        fontSize = 7;
                    }
                    else if (value.Equals(CssValue.XSmall))
                    {
                        fontSize = 7.5;
                    }
                    else if (value.Equals(CssValue.Small))
                    {
                        fontSize = 10;
                    }
                    else if (value.Equals(CssValue.Medium))
                    {
                        fontSize = 12;
                    }
                    else if (value.Equals(CssValue.Large))
                    {
                        fontSize = 13.5;
                    }
                    else if (value.Equals(CssValue.XLarge))
                    {
                        fontSize = 18;
                    }
                    else if (value.Equals(CssValue.XxLarge))
                    {
                        fontSize = 24;
                    }
                    else if (value.Equals(CssValue.XxxLarge))
                    {
                        fontSize = 24 * 1.5;
                    }
                    else
                    {
                        if (value.Equals(CssValue.Larger))
                            result = new CssLengthValue(LargerSmallerRatio, CssUnit.Em);
                        else if (value.Equals(CssValue.Smaller))
                            result = new CssLengthValue(1 / LargerSmallerRatio, CssUnit.Em);
                    }
                    if (fontSize > 0)
                        result = new CssLengthValue(fontSize, CssUnit.Pt);
                    break;
                }
                case CssValueType.Percentage:
                {
                    CssPercentageValue percentageValue = (CssPercentageValue)value;
                    double percents = percentageValue.DoubleValue / 100;
                    result = new CssLengthValue(percents, CssUnit.Em);
                    break;
                }
                case CssValueType.String:
                case CssValueType.Number:
                case CssValueType.Length:
                case CssValueType.Uri:
                case CssValueType.Comma:
                case CssValueType.Solidus:
                {
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }
            return result;
        }

        // We use a scale 1.2 but, there is no standard and browser results will differ.
        private const double LargerSmallerRatio = 1.2;
    }
}
