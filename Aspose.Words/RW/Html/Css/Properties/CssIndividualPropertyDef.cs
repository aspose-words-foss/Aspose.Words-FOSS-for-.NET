// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for CSS individual properties definitions.
    /// Individual properties are CSS properties that let you set the value of one CSS property only.
    /// e.g: margin-left: 10px
    ///      background-position: 10px 30px
    /// </summary>
    internal abstract class CssIndividualPropertyDef : CssPropertyDef
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="inherited">Indicates that the property is normally inherited.</param>
        /// <param name="initialValue">Initial value of the property. Can be null.</param>
        protected CssIndividualPropertyDef(string name, bool inherited, CssPropertyValue initialValue)
            : base(name, inherited)
        {
            mInitialValue = initialValue;
        }

        internal CssDeclaration CreateInitialDeclaration(bool important)
        {
            return (mInitialValue != null)
                       ? new CssSpecifiedDeclaration(Name, mInitialValue, important)
                       : null;
        }

        internal abstract CssDeclaration CreateIndividualDeclaration(
            CssValueList cssValues,
            int startIndex,
            bool important,
            bool isInQuirksMode,
            out int affectedValues);

        internal virtual CssDeclaration CreateInheritDeclaration(bool important)
        {
            return new CssSpecifiedDeclaration(Name, new CssPropertyValue(CssValue.Inherit), important);
        }

        internal override CssDeclarationCollection CreateDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            CssDeclaration declaration = CreateDeclaration(cssValues, important, isInQuirksMode);
            return (declaration != null)
                ? new CssDeclarationCollection(declaration)
                : null;
        }

        /// <summary>
        /// Replaces the "initial" keyword with an initial value of a CSS property and calls a method that resolves a
        /// specified value to a computed one.
        /// </summary>
        internal override CssPropertyValue ToComputedValue(CssPropertyValue specifiedValue, CssCascadeContext cssContext)
        {
            if ((specifiedValue.Count > 0) &&
                specifiedValue.IsInitial &&
                (mInitialValue != null))
            {
                specifiedValue = mInitialValue;
            }

            return ToComputedValueCore(specifiedValue, cssContext);
        }

        /// <summary>
        /// Resolves a specified property value to a computed one.
        /// </summary>
        /// <remarks>
        /// This method is overridden in some properties that compute their values in a special way.
        /// </remarks>
        protected virtual CssPropertyValue ToComputedValueCore(
            CssPropertyValue specifiedValue,
            CssCascadeContext cssContext)
        {
            return ComputeRelativeLengthValue(specifiedValue, cssContext);
        }

        /// <summary>
        /// Computes a relative length value. If the value is not a relative length, returns it unchanged.
        /// </summary>
        protected static CssPropertyValue ComputeRelativeLengthValue(
            CssPropertyValue specifiedValue,
            CssCascadeContext cssContext)
        {
            if ((specifiedValue.Count == 1) && (specifiedValue.FirstValue.ValueType == CssValueType.Length))
            {
                CssLengthValue cssLength = (CssLengthValue)specifiedValue.FirstValue;
                if (cssLength.IsRelative)
                {
                    double absoluteLength = CssUtil.RelativeLengthToPoint(
                        cssLength,
                        cssContext.RootElementDeclarations,
                        cssContext.ElementFontSize);
                    if (!MathUtil.AreEqual(absoluteLength, double.MinValue))
                        return new CssPropertyValue(new CssLengthValue(absoluteLength, CssUnit.Pt));
                }
            }

            return specifiedValue;
        }

        protected CssDeclaration CreateDeclaration(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            CssPropertyValue propertyValue;
            // The 'inherit' and 'initial' values are allowed for all CSS properties.
            if ((cssValues.Count == 1) && cssValues[0].Equals(CssValue.Inherit))
                propertyValue = new CssPropertyValue(CssValue.Inherit);
            else if ((cssValues.Count == 1) && cssValues[0].Equals(CssValue.Initial))
                propertyValue = new CssPropertyValue(CssValue.Initial);
            else
                propertyValue = CreatePropertyValue(cssValues, isInQuirksMode);

            return (propertyValue != null)
                ? new CssSpecifiedDeclaration(Name, propertyValue, important)
                : null;
        }

        /// <summary> 
        /// Implement in derived classes and return a CSS property value if the property can accept the specified values.
        /// You should not process 'inherit' and 'initial' input values in this function.
        /// </summary>
        /// <param name="cssValues">CSS values.</param>
        /// <param name="isInQuirksMode">
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </param>
        /// <returns>CSS property value if the property can accept the specified values; null otherwise.</returns>
        protected abstract CssPropertyValue CreatePropertyValue(CssValueList cssValues, bool isInQuirksMode);

        internal string PropertyName
        {
            get { return Name; }
        }

        /// <summary>
        /// Initial value of the property i.e. the value to be used in the absence of any stylesheet or equivalent. Can be null.
        /// </summary>
        internal override CssPropertyValue InitialValue
        {
            get { return mInitialValue; }
        }

        private readonly CssPropertyValue mInitialValue;
    }
}
