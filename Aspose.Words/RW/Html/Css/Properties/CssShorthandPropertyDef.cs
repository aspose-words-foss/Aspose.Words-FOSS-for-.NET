// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2013 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base definition class for CSS shorthand properties.
    /// Shorthand properties are CSS properties that let you set the values of several other CSS properties simultaneously.
    /// e.g: background: #ffffff url('img_tree.png') no-repeat right top;
    /// </summary>
    internal abstract class CssShorthandPropertyDef : CssPropertyDef
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="inherited">Indicates that the property is normally inherited.</param>
        protected CssShorthandPropertyDef(string name, bool inherited)
            : base(name, inherited)
        {
            // Empty constructor.
        }

        internal override CssDeclarationCollection CreateDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            if (cssValues.Count == 0)
                return null;

            CssDeclarationCollection result;
            if ((cssValues.Count == 1) && cssValues[0].Equals(CssValue.Inherit))
            {
                CssDeclarationCollectionBuilder inheritDeclarations = new CssDeclarationCollectionBuilder();
                foreach (ShorthandPropertyPart individualProperty in GetIndividualProperties())
                {
                    CssDeclarationCollection propertyInheritDeclarations = individualProperty.CreateInheritDeclarations(important);
                    inheritDeclarations.AddOrReplace(propertyInheritDeclarations);
                }
                result = inheritDeclarations.GetDeclarations();
            }
            else if ((cssValues.Count == 1) && cssValues[0].Equals(CssValue.Initial))
            {
                // The initial CSS keyword applies the initial value of a property to an element. 
                // It is allowed on every CSS property and causes the element for which it is specified to use the initial value of the property.
                CssDeclarationCollectionBuilder initialDeclarations = new CssDeclarationCollectionBuilder();
                foreach (ShorthandPropertyPart individualProperty in GetIndividualProperties())
                {
                    CssDeclarationCollection propertyInitialDeclarations = individualProperty.CreateInitialDeclarations(important);
                    if (propertyInitialDeclarations != null)
                        initialDeclarations.AddOrReplace(propertyInitialDeclarations);
                }
                result = initialDeclarations.GetDeclarations();
            }
            else
            {
                result = CreateIndividualDeclarations(cssValues, important, isInQuirksMode);
            }

            return ((result != null) && (result.Count != 0))
                ? result
                : null;
        }

        /// <summary>
        /// Checks whether any properties in a collection can be reduced to this shorthand.
        /// </summary>
        internal virtual bool CanReduce(CssDeclarationCollection declarationsBuilder)
        {
            ShorthandPropertyPart[] individualProperties = GetIndividualProperties();
            bool foundAnything = false;
            foreach (ShorthandPropertyPart shorthandPropertyPart in individualProperties)
            {
                if (declarationsBuilder[shorthandPropertyPart.PropertyName] != null)
                {
                    foundAnything = true;
                }
                else if (shorthandPropertyPart.Required)
                {
                    return false;
                }
            }
            return foundAnything;
        }

        /// <summary>
        /// Reduces appropriate properties in a collection to this shorthand. 
        /// Removes the reduced properties from the collection and adds a shorthand property into it.
        /// </summary>
        internal virtual void Reduce(CssDeclarationCollectionBuilder declarationsBuilder)
        {
            ShorthandPropertyPart[] individualProperties = GetIndividualProperties();

            List<CssDeclaration> foundDeclarations = new List<CssDeclaration>(individualProperties.Length);

            int inheritDeclarationsCount = 0;
            int importantCount = 0;
            for (int i = 0; i < individualProperties.Length; i++)
            {
                Debug.Assert(individualProperties[i].PropertyName != null);
                CssDeclaration declaration = declarationsBuilder[individualProperties[i].PropertyName];
                if (declaration == null)
                    return;

                foundDeclarations.Add(declaration);
                if (declaration.Important)
                    importantCount++;
                if (declaration.Value.IsInherit)
                    inheritDeclarationsCount++;
            }

            // We reduce only two or more individual declarations into a shorthand one. One individual declaration we ignore.
            if ((foundDeclarations.Count > 1) &&
                ((importantCount == 0) || (importantCount == foundDeclarations.Count)) &&
                ((inheritDeclarationsCount == 0) || (inheritDeclarationsCount == foundDeclarations.Count)))
            {
                CssPropertyValue shorthandValue = CreateShorthandValue(new CssDeclarationCollection(foundDeclarations));
                if (shorthandValue != null)
                {
                    foreach (CssDeclaration declaration in foundDeclarations)
                    {
                        declarationsBuilder.Remove(declaration.Property);
                    }
                    declarationsBuilder.AddOrReplace(new CssSpecifiedDeclaration(Name, shorthandValue, importantCount != 0));
                }
            }
        }

        protected abstract CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations);

        /// <summary> 
        /// Implement in derived classes and return CSS declarations if the property can accept the specified values.
        /// </summary>
        /// <remarks>
        /// You don't need to handle 'initial' and 'inherit' CSS values in this function.
        /// </remarks>
        /// <param name="cssValues">CSS values.</param>
        /// <param name="important">Indicates that the declaration should be marked as !important.</param>
        /// <param name="isInQuirksMode">
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </param>
        /// <returns>CSS declarations if the property can accept the specified values; null otherwise.</returns>
        protected abstract CssDeclarationCollection CreateIndividualDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode);

        /// <summary>
        /// Implement in derived classes and return individual properties, which are parts of the shorthand property.
        /// </summary>
        /// <returns>Individual properties array.</returns>
        protected abstract ShorthandPropertyPart[] GetIndividualProperties();
    }
}
