// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents base class for individual 'padding-xxx' CSS properties. 
    /// </summary>
    internal abstract class CssPaddingIndividualPropertyDefBase : CssIndividualSimplePropertyDef
    {
        protected CssPaddingIndividualPropertyDefBase(string name)
            : base(
                name,
                false,
                CssValue.Zero,
                // <length> | <percentage>
                CssValueFilter.AnyOf(
                    CssValueFilter.NonNegativeQuirkyLength,
                    CssValueFilter.NonNegativePercentage))
        {
            // Empty constructor.
        }

        /// <summary>
        /// Computes and returns padding value measured in points.
        /// </summary>
        /// <param name="propertyValue">Property value.</param>
        /// <returns>A padding value in points; zero, if the property value is illegal or unsupported.</returns>
        internal static double GetPadding(CssPropertyValue propertyValue)
        {
            double padding = 0;
            if (propertyValue.Count == 1)
                ConvertValueToPadding(propertyValue.FirstValue, out padding);
            return padding;
        }

        /// <summary>
        /// Converts a CSS value to padding value measured in points.
        /// </summary>
        /// <param name="cssValue">CSS value</param>
        /// <param name="paddingPoints">Padding value, points.</param>
        /// <returns>true, if the value was successfully converted; false, otherwise. </returns>
        protected static bool ConvertValueToPadding(CssValue cssValue, out double paddingPoints)
        {
            double length = CssUtil.LengthToPoint(cssValue);
            if (!MathUtil.AreEqual(length, double.MinValue))
            {
                paddingPoints = length;
                return true;
            }

            paddingPoints = 0;
            return false;
        }
    }
}
