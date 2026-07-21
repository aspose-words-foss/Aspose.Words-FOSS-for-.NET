// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2013 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'font' CSS shorthand property. 
    /// </summary>
    /// <remarks>
    /// The font property is a shorthand property for setting the individual font properties (i.e., 'font-style', 'font-variant', 
    /// 'font-weight', 'font-size', 'line-height' and 'font-family') at the same place in the style sheet.
    /// The properties that can be set, are (in order): "font-style font-variant font-weight font-size/line-height font-family"
    /// The font-size and font-family values are required. If one of the other values are missing, the default values will be inserted, if any.
    /// </remarks>
    internal class CssFontPropertyDef : CssShorthandPropertyDef
    {
        internal CssFontPropertyDef()
            : base("font", true)
        {
            // Empty constructor.
        }

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations)
        {
            CssDeclaration fontStyleDeclaration = individualDeclarations["font-style"];
            CssDeclaration fontSizeDeclaration = individualDeclarations["font-size"];
            CssDeclaration fontFamilyDeclaration = individualDeclarations["font-family"];
            CssDeclaration fontVariantDeclaration = individualDeclarations["font-variant"];
            CssDeclaration fontWeightDeclaration = individualDeclarations["font-weight"];
            CssDeclaration lineHeightDeclaration = individualDeclarations["line-height"];

            return new CssFontPropertyValue(
                (fontStyleDeclaration != null) ? fontStyleDeclaration.Value.FirstValue : null,
                (fontVariantDeclaration != null) ? fontVariantDeclaration.Value.FirstValue : null,
                (fontWeightDeclaration != null) ? fontWeightDeclaration.Value.FirstValue : null,
                fontSizeDeclaration.Value.FirstValue,
                (lineHeightDeclaration != null) ? lineHeightDeclaration.Value.FirstValue : null,
                fontFamilyDeclaration.Value.FirstValue);
        }

        protected override ShorthandPropertyPart[] GetIndividualProperties()
        {
            // The properties that can be set, are (in order): "font-style font-variant font-weight font-size/line-height font-family"
            if (mIndividualProperties != null)
                return mIndividualProperties;

            mIndividualProperties = new ShorthandPropertyPart[]
            {
                new ShorthandPropertyPart("font-style", false),
                new ShorthandPropertyPart("font-variant", false),
                new ShorthandPropertyPart("font-weight", false),
                new ShorthandPropertyPart("font-size", true),
                new ShorthandPropertyPart("line-height", false),
                new ShorthandPropertyPart("font-family", true)
            };
            return mIndividualProperties;
        }

        protected override CssDeclarationCollection CreateIndividualDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            // [ [ <'font-style'> || <'font-variant'> || <'font-weight'> ]? <'font-size'> [ / <'line-height'> ]? <'font-family'> ] | 
            // caption | icon | menu | message-box | small-caption | status-bar | inherit
            if (cssValues.Count == 0)
                return null;

            // Try to find caption | icon | menu | message-box | small-caption | status-bar | inherit
            if (cssValues.Count == 1)
            {
                CssDeclarationCollection systemFontDeclarations = TryCreateFromSystemFont(cssValues[0]);
                if (systemFontDeclarations != null)
                    return systemFontDeclarations;
            }

            // The properties that can be set, are (in order):
            //   font-style font-variant font-weight font-size/line-height font-family
            // The font-size and font-family values are required. If one of the other values are missing, the default values
            // will be inserted, if any.
            // The order of the values is not completely free: font-style, font-variant and font-weight must be defined,
            // if any, before the font-size value.
            // The line-height value must be defined immediately after the font-size, preceded by a mandatory /. Finally,
            // the font-family is mandatory and must be the last value defined (inherit value does not work).

            CssValueList cssValuesCopy = new CssValueList(cssValues);
            CssDeclarationCollectionBuilder result = new CssDeclarationCollectionBuilder();

            int affectedValues;
            CssDeclaration sizeDeclaration = null;
            int fontSizeValueIndex = -1;
            for (int index = cssValuesCopy.Count - 1; index >= 0; index--)
            {
                if ((index > 0) && cssValuesCopy[index - 1].Equals(CssValue.Solidus))
                    continue;

                // Font-size value cannot be number in standards and quirks modes.
                if (cssValuesCopy[index].ValueType == CssValueType.Number)
                    continue;

                CssIndividualPropertyDef fontSizePropertyDef = CssPropertyDefFactory.GetIndividualPropertyDef("font-size");
                sizeDeclaration = fontSizePropertyDef.CreateIndividualDeclaration(
                    cssValuesCopy,
                    index,
                    important,
                    isInQuirksMode,
                    out affectedValues);
                if (sizeDeclaration != null)
                {
                    result.Add(sizeDeclaration);
                    fontSizeValueIndex = index;
                    for (int i = 0; i < affectedValues; i++)
                        cssValuesCopy.RemoveAt(fontSizeValueIndex);
                    break;
                }
            }

            // font-size part is mandatory.
            if (sizeDeclaration == null)
                return null;
            // font-family is mandatory and must be the last value defined (must be after font-size).
            if (fontSizeValueIndex > cssValuesCopy.Count - 1)
                return null;

            CssIndividualPropertyDef lineHeigthPropertyDef = CssPropertyDefFactory.GetIndividualPropertyDef("line-height");
            if (cssValuesCopy[fontSizeValueIndex].Equals(CssValue.Solidus))
            {
                cssValuesCopy.RemoveAt(fontSizeValueIndex);
                if (fontSizeValueIndex > cssValuesCopy.Count - 1)
                    return null;

                CssDeclaration lineHeightDeclaration = lineHeigthPropertyDef.CreateIndividualDeclaration(
                    cssValuesCopy,
                    fontSizeValueIndex,
                    important,
                    isInQuirksMode,
                    out affectedValues);
                // Line height part is mandatory when solidus exists.
                if (lineHeightDeclaration == null)
                    return null;

                result.Add(lineHeightDeclaration);
                for (int i = 0; i < affectedValues; i++)
                    cssValuesCopy.RemoveAt(fontSizeValueIndex);
            }
            else
            {
                result.AddOrReplace(lineHeigthPropertyDef.CreateInitialDeclaration(important));
            }

            // font-family is mandatory and must be the last value defined (must be after font-size).
            if (fontSizeValueIndex > cssValuesCopy.Count - 1)
                return null;

            CssIndividualPropertyDef fontFamilyPropertyDef = CssPropertyDefFactory.GetIndividualPropertyDef("font-family");
            CssDeclaration fontFamilyDeclaration = fontFamilyPropertyDef.CreateIndividualDeclaration(
                cssValuesCopy,
                fontSizeValueIndex,
                important,
                isInQuirksMode,
                out affectedValues);
            if (fontFamilyDeclaration == null)
                return null;

            result.Add(fontFamilyDeclaration);
            for (int i = 0; i < affectedValues; i++)
                cssValuesCopy.RemoveAt(fontSizeValueIndex);

            List<CssIndividualPropertyDef> additionalProperties = new List<CssIndividualPropertyDef>();
            additionalProperties.Add(CssPropertyDefFactory.GetIndividualPropertyDef("font-style"));
            additionalProperties.Add(CssPropertyDefFactory.GetIndividualPropertyDef("font-variant"));
            additionalProperties.Add(CssPropertyDefFactory.GetIndividualPropertyDef("font-weight"));

            int propertyIndex = 0;
            while ((propertyIndex < additionalProperties.Count) && (cssValuesCopy.Count != 0))
            {
                CssIndividualPropertyDef property = additionalProperties[propertyIndex];
                CssDeclaration propertyDeclaration = property.CreateIndividualDeclaration(
                    cssValuesCopy,
                    0,
                    important,
                    isInQuirksMode,
                    out affectedValues);
                if (propertyDeclaration != null)
                {
                    result.AddOrReplace(propertyDeclaration);
                    additionalProperties.RemoveAt(propertyIndex);
                    for (int i = 0; i < affectedValues; i++)
                        cssValuesCopy.RemoveAt(0);
                    propertyIndex = 0;
                }
                else
                {
                    propertyIndex++;
                }
            }

            if (cssValuesCopy.Count != 0)
                return null;

            foreach (CssIndividualPropertyDef property in additionalProperties)
                result.AddOrReplace(((CssIndividualSimplePropertyDef)property).CreateInitialDeclaration(important));

            return result.GetDeclarations();
        }

        /// <summary>
        /// Processes system fonts (caption | icon | menu | message-box | small-caption | status-bar).
        /// </summary>
        private static CssDeclarationCollection TryCreateFromSystemFont(CssValue cssValue)
        {
            // At the moment we just recognize these values, but don't do anything with the font.
            if (cssValue.Equals(CssValue.Caption) || cssValue.Equals(CssValue.Icon) ||
                cssValue.Equals(CssValue.Menu) || cssValue.Equals(CssValue.MessageBox) ||
                cssValue.Equals(CssValue.SmallCaption) || cssValue.Equals(CssValue.StatusBar))
            {
                return CssDeclarationCollection.Empty;
            }

            return null;
        }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
