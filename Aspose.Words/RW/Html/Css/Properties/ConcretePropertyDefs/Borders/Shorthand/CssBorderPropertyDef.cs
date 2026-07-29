// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'border' CSS property.
    /// </summary>
    /// <remarks>
    /// The 'border' property is a shorthand property for setting the same width, color, and style for all four borders of a box.
    /// http://www.w3.org/TR/CSS2/box.html#propdef-border
    /// </remarks>
    internal class CssBorderPropertyDef : CssOrderInsensitiveShorthandPropertyDef
    {
        internal CssBorderPropertyDef()
            : base("border", false)
        {
            // Empty constructor.
        }

        internal override void Reduce(CssDeclarationCollectionBuilder declarationsBuilder)
        {
            // Border declarations are already reduced to border-(top|right|bottom|left) shorthand properties before
            // because CssShorthandPropertyDef.Reduce() reduces them first.
            CssDeclaration borderTopDeclaration = declarationsBuilder["border-top"];
            CssDeclaration borderRightDeclaration = declarationsBuilder["border-right"];
            CssDeclaration borderBottomDeclaration = declarationsBuilder["border-bottom"];
            CssDeclaration borderLeftDeclaration = declarationsBuilder["border-left"];

            if ((borderTopDeclaration != null) &&
                borderTopDeclaration.Equals(borderRightDeclaration) &&
                borderTopDeclaration.Equals(borderBottomDeclaration) &&
                borderTopDeclaration.Equals(borderLeftDeclaration))
            {
                declarationsBuilder.Remove(borderTopDeclaration.Property);
                declarationsBuilder.Remove(borderRightDeclaration.Property);
                declarationsBuilder.Remove(borderBottomDeclaration.Property);
                declarationsBuilder.Remove(borderLeftDeclaration.Property);

                declarationsBuilder.AddOrReplace(new CssSpecifiedDeclaration(
                    Name,
                    borderTopDeclaration.Value,
                    borderTopDeclaration.Important));
            }
        }

        protected override CssDeclarationCollection CreateIndividualDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            // border: [border-style] [border-width] [border-color]|initial|inherit;
            if (cssValues.Count == 0)
                return null;

            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();
            CssValueList cssValuesCopy = new CssValueList(cssValues);

            CssDeclarationCollection styleDeclarations = GetPropertyDeclarations(
                CssPropertyDefFactory.GetIndividualPropertyDef("border-top-style"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-right-style"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-bottom-style"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-left-style"),
                cssValuesCopy,
                important,
                isInQuirksMode);
            if (styleDeclarations.Count != 0)
                result.AddOrReplace(styleDeclarations);

            // WORDSNET-14913 Unlike individual "border-xxx-width" properties, the shorthand "border" property
            // does not accept quirky length values (unitless number values). That's why here we always create border width
            // declarations in the Standards mode and ignore the "is in Quirks mode" flag.
            CssDeclarationCollection widthDeclarations = GetPropertyDeclarations(
                CssPropertyDefFactory.GetIndividualPropertyDef("border-top-width"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-right-width"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-bottom-width"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-left-width"),
                cssValuesCopy,
                important,
                false);
            if (widthDeclarations.Count != 0)
                result.AddOrReplace(widthDeclarations);

            CssDeclarationCollection colorDeclarations = GetPropertyDeclarations(
                CssPropertyDefFactory.GetIndividualPropertyDef("border-top-color"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-right-color"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-bottom-color"),
                CssPropertyDefFactory.GetIndividualPropertyDef("border-left-color"),
                cssValuesCopy,
                important,
                isInQuirksMode);
            if (colorDeclarations.Count != 0)
                result.AddOrReplace(colorDeclarations);

            return (cssValuesCopy.Count == 0)
                ? result.GetDeclarations()
                : null;
        }

        private static CssDeclarationCollection GetPropertyDeclarations(
            CssIndividualPropertyDef topPropertyDef,
            CssIndividualPropertyDef rightLeftPropertyDefs,
            CssIndividualPropertyDef bottomPropertyDefs,
            CssIndividualPropertyDef leftPropertyDefs,
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            CssDeclarationCollectionBuilder declarationBuilder = new CssDeclarationCollectionBuilder();

            int valueIndex = 0;
            while (valueIndex < cssValues.Count)
            {
                int affectedValuesCount;
                CssDeclaration topDeclaration = topPropertyDef.CreateIndividualDeclaration(
                    cssValues,
                    valueIndex,
                    important,
                    isInQuirksMode,
                    out affectedValuesCount);
                if (topDeclaration == null)
                {
                    valueIndex++;
                    continue;
                }

                CssValueList affectedValues = new CssValueList();
                while (affectedValuesCount != 0)
                {
                    affectedValues.Add(cssValues[valueIndex]);
                    cssValues.RemoveAt(valueIndex);
                    affectedValuesCount--;
                }

                declarationBuilder.AddOrReplace(topDeclaration);
                foreach (CssIndividualPropertyDef edgePropertyDef in
                    new CssIndividualPropertyDef[] { rightLeftPropertyDefs, bottomPropertyDefs, leftPropertyDefs })
                {
                    declarationBuilder.AddOrReplace(edgePropertyDef.CreateDeclarations(
                        affectedValues,
                        important,
                        isInQuirksMode));
                }

                return declarationBuilder.GetDeclarations();
            }

            foreach (CssIndividualPropertyDef edgePropertyDef in
                new CssIndividualPropertyDef[] { topPropertyDef, rightLeftPropertyDefs, bottomPropertyDefs, leftPropertyDefs })
            {
                CssDeclaration initialDeclaration = edgePropertyDef.CreateInitialDeclaration(important);
                if (initialDeclaration != null)
                    declarationBuilder.AddOrReplace(initialDeclaration);
            }

            return declarationBuilder.GetDeclarations();
        }

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations)
        {
            Debug.Assert(false, "Should not be called.");
            return null;
        }

        protected override ShorthandPropertyPart[] GetIndividualProperties()
        {
            if (mIndividualProperties != null)
                return mIndividualProperties;

            mIndividualProperties = new ShorthandPropertyPart[]
            {
                new ShorthandPropertyPart("border-top-style", true),
                new ShorthandPropertyPart("border-right-style", true),
                new ShorthandPropertyPart("border-bottom-style", true),
                new ShorthandPropertyPart("border-left-style", true),
                new ShorthandPropertyPart("border-top-width", false),
                new ShorthandPropertyPart("border-right-width", false),
                new ShorthandPropertyPart("border-bottom-width", false),
                new ShorthandPropertyPart("border-left-width", false),
                new ShorthandPropertyPart("border-top-color", false),
                new ShorthandPropertyPart("border-right-color", false),
                new ShorthandPropertyPart("border-bottom-color", false),
                new ShorthandPropertyPart("border-left-color", false)
            };
            return mIndividualProperties;
        }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
