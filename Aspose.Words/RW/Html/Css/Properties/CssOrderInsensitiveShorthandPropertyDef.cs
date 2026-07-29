// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2013 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base definition class for CSS shorthand properties where order of individual properties isn't important.
    /// </summary>
    internal abstract class CssOrderInsensitiveShorthandPropertyDef : CssShorthandPropertyDef
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="inherited">Indicates that the property is normally inherited.</param>
        protected CssOrderInsensitiveShorthandPropertyDef(string name, bool inherited)
            : base(name, inherited)
        {
        }

        protected override CssDeclarationCollection CreateIndividualDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            if (cssValues.Count == 0)
                return null;

            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();
            List<ShorthandPropertyPart> individualProperties = new List<ShorthandPropertyPart>(GetIndividualProperties());

            int valueIndex = 0;
            while (valueIndex < cssValues.Count)
            {
                bool propertyFound = false;
                int propertyIndex = 0;
                while ((propertyIndex < individualProperties.Count) && !propertyFound)
                {
                    ShorthandPropertyPart individualProperty = individualProperties[propertyIndex];
                    int affectedValues;
                    CssDeclarationCollection indivdualDeclarations = individualProperty.CreateIndividualDeclarations(
                        cssValues,
                        valueIndex,
                        important,
                        isInQuirksMode,
                        out affectedValues);
                    if (indivdualDeclarations != null)
                    {
                        propertyFound = true;
                        result.AddOrReplace(indivdualDeclarations);
                        individualProperties.RemoveAt(propertyIndex);
                        valueIndex += affectedValues;
                    }

                    propertyIndex++;
                }

                if (!propertyFound)
                    return null;
            }

            if (result.Count == 0)
                return null;

            foreach (ShorthandPropertyPart individualProperty in individualProperties)
            {
                if (individualProperty.Required)
                    return null;

                CssDeclarationCollection initialDeclarations = individualProperty.CreateInitialDeclarations(important);
                if (initialDeclarations != null)
                    result.AddOrReplace(initialDeclarations);
            }

            return result.GetDeclarations();
        }
    }
}
