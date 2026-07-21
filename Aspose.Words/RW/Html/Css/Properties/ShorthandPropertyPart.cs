// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/10/2015 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements an individual property, which can be a shorthand property part.
    /// </summary>
    internal class ShorthandPropertyPart
    {
        internal ShorthandPropertyPart(string propertyName, bool required)
        {
            mPropertyDef = CssPropertyDefFactory.GetIndividualPropertyDef(propertyName);
            Required = required;
        }

        /// <summary> 
        /// Creates and returns CSS declarations for the individual property, which is part of shorthand property.
        /// </summary>
        /// <remarks>
        /// 'initial' and 'inherit' values aren't available for individual property, which is part of shorthand property. 
        /// </remarks>
        /// <param name="cssValues">CSS values.</param>
        /// <param name="startIndex">The zero-based index at which looking begins.</param>
        /// <param name="important">Indicates that the declarations should be marked as !important.</param>
        /// <param name="isInQuirksMode">
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </param>
        /// <param name="affectedValues">Affected values count.</param>
        /// <returns>CSS declarations if the property can accept the specified values; null otherwise.</returns>
        internal CssDeclarationCollection CreateIndividualDeclarations(
            CssValueList cssValues,
            int startIndex,
            bool important,
            bool isInQuirksMode,
            out int affectedValues)
        {
            CssDeclaration declaration = mPropertyDef.CreateIndividualDeclaration(
                cssValues,
                startIndex,
                important,
                isInQuirksMode,
                out affectedValues);
            return (declaration != null)
                ? new CssDeclarationCollection(declaration)
                : null;
        }

        /// <summary> 
        /// Returns CSS declarations with 'inherit' values.
        /// </summary>
        /// <param name="important">Indicates that the declarations should be marked as !important.</param>
        /// <returns>CSS declarations with 'inherit' values.</returns>
        internal CssDeclarationCollection CreateInheritDeclarations(bool important)
        {
            return new CssDeclarationCollection(mPropertyDef.CreateInheritDeclaration(important));
        }

        /// <summary> 
        /// Returns CSS declarations with initial values of the properties.
        /// </summary>
        /// <param name="important">Indicates that the declarations should be marked as !important.</param>
        /// <returns>CSS declarations with initial values of the properties if the property has initial value; null otherwise.</returns>
        internal CssDeclarationCollection CreateInitialDeclarations(bool important)
        {
            CssDeclaration initialDeclaration = mPropertyDef.CreateInitialDeclaration(important);
            return (initialDeclaration != null)
                ? new CssDeclarationCollection(initialDeclaration)
                : null;
        }

        /// <summary>
        /// Whether the property is required.
        /// </summary>
        internal bool Required { get; }

        /// <summary>
        /// Individual property name.
        /// </summary>
        internal string PropertyName
        {
            get { return mPropertyDef.PropertyName; }
        }

        private readonly CssIndividualPropertyDef mPropertyDef;
    }
}
