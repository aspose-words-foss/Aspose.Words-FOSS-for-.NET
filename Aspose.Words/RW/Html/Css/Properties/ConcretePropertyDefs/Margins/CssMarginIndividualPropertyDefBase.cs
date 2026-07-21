// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents base class for individual 'margin-xxx' CSS properties.
    /// </summary>
    internal abstract class CssMarginIndividualPropertyDefBase : CssIndividualSimplePropertyDef
    {
        protected CssMarginIndividualPropertyDefBase(string name)
            : base(
                name,
                false,
                CssValue.Zero,
                CssValueFilter.AnyOf(
                    CssValueFilter.Value(CssValue.Auto),
                    CssValueFilter.Percentage,
                    CssValueFilter.QuirkyLength))
        {
            // Empty constructor.
        }

        /// <summary>
        /// Computes and returns indent value measured in points.
        /// </summary>
        /// <param name="propertyValue">Property value.</param>
        /// <returns>An indent value in points; zero, if the property value is illegal or unsupported.</returns>
        internal static double GetIndent(CssPropertyValue propertyValue)
        {
            double indentValue = CssUtil.LengthToPoint(propertyValue);
            if (MathUtil.AreEqual(indentValue, double.MinValue))
                indentValue = 0;
            return indentValue;
        }
    }
}
