// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for CSS individual properties with one CSS value.
    /// e.g: margin-left: 10px
    /// </summary>
    internal abstract class CssIndividualSimplePropertyDef : CssIndividualPropertyDef
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="inherited">Indicates that the property is normally inherited.</param>
        /// <param name="initialValue">Initial value of the property. Can be null.</param>
        /// <param name="valueFilter">Filters for values accepted by this property.</param>
        protected CssIndividualSimplePropertyDef(
            string name,
            bool inherited,
            CssValue initialValue,
            ICssValueFilter valueFilter)
            : base(
                name,
                inherited,
                (initialValue != null) ? new CssPropertyValue(initialValue) : null)
        {
            Debug.Assert(valueFilter != null);

            mValueFilter = valueFilter;
        }

        internal override CssDeclaration CreateIndividualDeclaration(
            CssValueList cssValues,
            int startIndex,
            bool important,
            bool isInQuirksMode,
            out int affectedValues)
        {
            CssPropertyValue propertyValue = CreatePropertyValue(cssValues[startIndex], isInQuirksMode);
            if (propertyValue != null)
            {
                affectedValues = 1;
                return new CssSpecifiedDeclaration(Name, propertyValue, important);
            }

            affectedValues = 0;
            return null;
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            // Only one CSS value is allowed for this property type.
            return (cssValues.Count == 1)
                ? CreatePropertyValue(cssValues[0], isInQuirksMode)
                : null;
        }

        /// <summary> 
        /// Returns a CSS property value if the property can accept the specified value.
        /// </summary>
        /// <param name="cssValue">CSS value.</param>
        /// <param name="isInQuirksMode">
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </param>
        /// <returns>CSS property value if the property can accept the specified value; null otherwise.</returns>
        private CssPropertyValue CreatePropertyValue(CssValue cssValue, bool isInQuirksMode)
        {
            return mValueFilter.Accepts(cssValue, isInQuirksMode)
                ? new CssPropertyValue(cssValue)
                : null;
        }

        /// <summary>
        /// A filter that accepts (passes) only CSS values that are valid for this property.
        /// </summary>
        private readonly ICssValueFilter mValueFilter;
    }
}
