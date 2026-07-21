// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2020 by Artem Shabarshin

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements '-aw-border' CSS extended border property.
    /// </summary>
    /// <remarks>
    /// The '-aw-border' extension property is a shorthand property for setting the same width and style
    /// for all four borders of a box.
    /// </remarks>
    internal class CssAWBorderPropertyDef : CssOrderInsensitiveShorthandPropertyDef
    {
        internal CssAWBorderPropertyDef()
            : base(HtmlConstants.AsposeBorder, false)
        {
            // Empty constructor.
        }

        internal override void Reduce(CssDeclarationCollectionBuilder declarationsBuilder)
        {
            // AWBorder declarations are already reduced to -aw-border-(top|right|bottom|left) shorthand properties before
            // because CssShorthandPropertyDef.Reduce() reduces them first.
            CssDeclaration borderTopDeclaration = declarationsBuilder[HtmlConstants.AsposeBorderTop];
            CssDeclaration borderRightDeclaration = declarationsBuilder[HtmlConstants.AsposeBorderRight];
            CssDeclaration borderBottomDeclaration = declarationsBuilder[HtmlConstants.AsposeBorderBottom];
            CssDeclaration borderLeftDeclaration = declarationsBuilder[HtmlConstants.AsposeBorderLeft];

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
            // -aw-border: [-aw-border-style] [-aw-border-width] [-aw-border-color];
            if (cssValues.Count == 0)
                return null;

            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();
            CssValueList cssValuesCopy = new CssValueList(cssValues);

            CssDeclarationCollection styleDeclarations = GetPropertyDeclarations(
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderTopStyle),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderRightStyle),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderBottomStyle),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderLeftStyle),
                cssValuesCopy,
                important,
                isInQuirksMode);
            if (styleDeclarations.Count != 0)
                result.AddOrReplace(styleDeclarations);

            CssDeclarationCollection widthDeclarations = GetPropertyDeclarations(
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderTopWidth),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderRightWidth),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderBottomWidth),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderLeftWidth),
                cssValuesCopy,
                important,
                isInQuirksMode);
            if (widthDeclarations.Count != 0)
                result.AddOrReplace(widthDeclarations);

            CssDeclarationCollection colorDeclarations = GetPropertyDeclarations(
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderTopColor),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderRightColor),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderBottomColor),
                CssPropertyDefFactory.GetIndividualPropertyDef(HtmlConstants.AsposeBorderLeftColor),
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
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderTopStyle, true),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderRightStyle, true),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderBottomStyle, true),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderLeftStyle, true),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderTopWidth, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderRightWidth, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderBottomWidth, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderLeftWidth, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderTopColor, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderRightColor, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderBottomColor, false),
                new ShorthandPropertyPart(HtmlConstants.AsposeBorderLeftColor, false)
            };
            return mIndividualProperties;
        }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
