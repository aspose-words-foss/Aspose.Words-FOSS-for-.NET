// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2017 by Nikolay Sezganov

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css.SvgElementDefs
{
    /// <summary>
    /// Represents a record for a SVG element in the user agent style sheet.
    /// Derived classes created for concrete SVG elements
    /// should be registered in <see cref="DefaultElementStyleResolver" />.
    /// </summary>
    internal class SvgElementDef : ElementDef
    {
        protected override void ApplyStyles(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            Debug.Assert(element.ElementNamespace == W3CNamespaces.Svg);

            HandleFontFamilyProperty(element, cssDeclarations);
            HandleTextDecorationProperty(element, cssDeclarations);
            HandleStrokeDasharrayProperty(element, cssDeclarations);
            HandleSimpleValueAttributes(element, cssDeclarations);
        }

        private static void HandleStrokeDasharrayProperty(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            string value = element.GetAttributeValue("stroke-dasharray");
            if ((value == null) || !StringUtil.HasChars(value.Trim()))
            {
                return;
            }

            CssValueList cssValueCollection = new CssValueList();
            foreach (string dashLengthString in GetNonWhitespaceSubstrings(value, ',', ' '))
            {
                double dashLength = FormatterPal.ParseDouble(dashLengthString);
                cssValueCollection.Add(new CssNumberValue(dashLength));
            }

            CssPropertyDef propertyDef = CssPropertyDefFactory.GetIndividualPropertyDef("stroke-dasharray");
            CssDeclarationCollection declarations = propertyDef.CreateDeclarations(
                cssValueCollection,
                false,
                false);

            if (declarations == null)
                return;

            cssDeclarations.AddOrReplace(declarations);
        }

        private static void HandleTextDecorationProperty(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            string value = element.GetAttributeValue("text-decoration");
            if ((value == null) || !StringUtil.HasChars(value.Trim()))
            {
                return;
            }

            CssValueList cssValueCollection = new CssValueList();
            foreach (string decorationValue in GetNonWhitespaceSubstrings(value, ' '))
            {
                cssValueCollection.Add(new CssIdentifierValue(decorationValue));
            }

            CssIndividualPropertyDef propertyDef = CssPropertyDefFactory.GetIndividualPropertyDef("text-decoration");
            CssDeclarationCollection declarations = propertyDef.CreateDeclarations(
                cssValueCollection,
                false,
                false);

            if (declarations == null)
                return;

            cssDeclarations.AddOrReplace(declarations);
        }

        private static void HandleFontFamilyProperty(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            string value = element.GetAttributeValue("font-family");
            if ((value == null) || !StringUtil.HasChars(value.Trim()))
            {
                return;
            }

            CssValueList cssValueCollection = new CssValueList();
            foreach (string fontName in GetNonWhitespaceSubstrings(value, ','))
            {
                // WORDSNET-17895 Replace first pair of inner quotes to support for quoted family names
                // if the passed string complies with requirements.
                // The requirements are different from one browser to another. Here is the rules of IE.

                Regex quotedFontNameRegEx = new Regex("^([\"\'])([^\"\'\\s])+?(.)*$");
                string unquotedFontName = quotedFontNameRegEx.IsMatch(fontName)
                    ? Regex.Replace(fontName, "^[\"\']|[\"\']$", "")
                    : fontName;

                if (cssValueCollection.Count > 0)
                {
                    cssValueCollection.Add(CssValue.Comma);
                }
                cssValueCollection.Add(CssValue.CreateFontFamilyValue(unquotedFontName));
            }

            CssIndividualPropertyDef propertyDef = CssPropertyDefFactory.GetIndividualPropertyDef("font-family");
            CssDeclarationCollection declarations = propertyDef.CreateDeclarations(
                cssValueCollection,
                false,
                false);

            if (declarations == null)
                return;

            cssDeclarations.AddOrReplace(declarations);
        }

        private static void HandleSimpleValueAttributes(
            IElementProvider element,
            CssDeclarationCollectionBuilder cssDeclarations)
        {
            foreach (string attributeName in gStylingAttributeNames)
            {
                string value = element.GetAttributeValue(attributeName);
                if ((value == null) || !StringUtil.HasChars(value.Trim()))
                {
                    continue;
                }

                CssValue cssValue = CssParser.ParseValue(value);
                if (cssValue == null)
                {
                    continue;
                }

                if ((cssValue.ValueType == CssValueType.Number) &&
                    ((attributeName == "width") ||
                     (attributeName == "height") ||
                     (attributeName == "font-size") ||
                     (attributeName == "letter-spacing")))
                {
                    cssValue = new CssLengthValue(cssValue.DoubleValue, CssUnit.Px);
                }

                CssPropertyDef propertyDef = CssPropertyDefFactory.GetPropertyDef(attributeName);
                CssDeclarationCollection declarations = propertyDef.CreateDeclarations(cssValue, false, false);
                if (declarations != null)
                {
                    cssDeclarations.AddOrReplace(declarations);
                }
            }
        }

        /// <summary>
        /// Splits a string into substrings by the specified separators. Returns only non-whitespace substrings.
        /// </summary>
        private static List<string> GetNonWhitespaceSubstrings(string value, params char[] separators)
        {
            Debug.Assert(value != null);

            List<string> subValueList = new List<string>();

            string[] subValues = value.Split(separators);
            foreach (string subValue in subValues)
            {
                string trimmedSubValue = subValue.Trim();
                if (!string.IsNullOrEmpty(trimmedSubValue))
                {
                    subValueList.Add(trimmedSubValue);
                }
            }

            return subValueList;
        }

        /// <summary>
        /// Names of attributes whose values are translated to CSS properties.
        /// </summary>
        /// <remarks>
        /// Note that the following attributes are not in this list, because they have complex values that are processed
        /// separately: 'font-family', 'text-decoration', and 'stroke-dasharray'.
        /// </remarks>
        private static readonly string[] gStylingAttributeNames = new string[]
        {
            "height",
            "width",
            //Font properties:
            "font",
            "font-size",
            "font-size-adjust",
            "font-stretch",
            "font-style",
            "font-variant",
            "font-weight",
            //Text properties:
            "direction",
            "letter-spacing",
            "unicode-bidi",
            "word-spacing",
            //Other properties for visual media:
            "clip",
            "color",
            "cursor",
            "display",
            "overflow",
            "visibility",
            //The following SVG properties are not defined in CSS2.
            //Clipping, Masking and Compositing properties:
            "clip-path",
            "clip-rule",
            "mask",
            "opacity",
            //Filter Effects properties:
            "enable-background",
            "filter",
            "flood-color",
            "flood-opacity",
            "lighting-color",
            //Gradient properties:
            "stop-color",
            "stop-opacity",
            //Interactivity properties:
            "pointer-events",
            //Color and Painting properties:
            "color-interpolation",
            "color-interpolation-filters",
            "color-profile",
            "color-rendering",
            "fill",
            "fill-opacity",
            "fill-rule",
            "image-rendering",
            "marker",
            "marker-end",
            "marker-mid",
            "marker-start",
            "shape-rendering",
            "stroke",
            "stroke-dashoffset",
            "stroke-linecap",
            "stroke-linejoin",
            "stroke-miterlimit",
            "stroke-opacity",
            "stroke-width",
            "text-rendering",
            //Text properties:
            "alignment-baseline",
            "baseline-shift",
            "dominant-baseline",
            "glyph-orientation-horizontal",
            "glyph-orientation-vertical",
            "kerning",
            "text-anchor",
            "writing-mode",
        };
    }
}
