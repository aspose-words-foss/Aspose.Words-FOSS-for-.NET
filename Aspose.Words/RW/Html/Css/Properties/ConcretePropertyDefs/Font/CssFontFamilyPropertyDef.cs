// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'font-family' CSS property.
    /// </summary>
    internal class CssFontFamilyPropertyDef : CssIndividualPropertyDef
    {
        internal CssFontFamilyPropertyDef(string propertyName)
            : base(propertyName, true, null) // Initial value depends on user agent
        {
            // Empty constructor.
        }

        internal CssFontFamilyPropertyDef()
            : this("font-family")
        {
            // Empty constructor.
        }

        internal override CssDeclaration CreateIndividualDeclaration(
            CssValueList cssValues,
            int startIndex,
            bool important,
            bool isInQuirksMode,
            out int affectedValues)
        {
            bool endOfSet = false;
            int valueIndex = startIndex;
            CssValueType prevValueType = CssValueType.Comma;
            bool wrongValue = false;
            affectedValues = 0;
            while ((valueIndex < cssValues.Count) && !endOfSet && !wrongValue)
            {
                CssValue value = cssValues[valueIndex];
                switch (prevValueType)
                {
                    case CssValueType.Comma:
                    {
                        if ((value.ValueType == CssValueType.String) || (value.ValueType == CssValueType.Identifier))
                            affectedValues++;
                        else
                            wrongValue = true;
                        break;
                    }
                    case CssValueType.Identifier:
                    {
                        if ((value.ValueType == CssValueType.Identifier) || (value.ValueType == CssValueType.Comma))
                            affectedValues++;
                        else
                            endOfSet = true;
                        break;
                    }
                    case CssValueType.String:
                    {
                        if (value.ValueType == CssValueType.Comma)
                            affectedValues++;
                        else
                            endOfSet = true;
                        break;
                    }
                    default:
                        Debug.Assert(false);
                        break;
                }
                prevValueType = value.ValueType;
                valueIndex++;
            }

            if ((affectedValues == 0) || wrongValue || (prevValueType == CssValueType.Comma))
            {
                affectedValues = 0;
                return null;
            }

            return CreateDeclaration(cssValues.GetRange(startIndex, affectedValues), important, isInQuirksMode);
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            // [[ <family-name> | <generic-family> ] [, <family-name>| <generic-family>]* ] | inherit
            // Font family names must either be given quoted as strings, or unquoted as a sequence of one or more identifiers. 
            CssValueList fontFamilyValues = new CssValueList();
            bool legalValues = true;
            CssValueType prevValueType = CssValueType.Comma;
            string familyNameIdentifier = string.Empty;
            int i = 0;
            while ((i < cssValues.Count) && legalValues)
            {
                CssValue value = cssValues[i];
                switch (prevValueType)
                {
                    case CssValueType.Comma:
                    {
                        if (value.ValueType == CssValueType.String)
                            fontFamilyValues.Add(value);
                        else if (value.ValueType == CssValueType.Identifier)
                            // Font family name can be a sequence of one or more identifiers.
                            familyNameIdentifier = ((CssIdentifierValue)value).Value;
                        else
                            legalValues = false;
                        break;
                    }
                    case CssValueType.Identifier:
                    {
                        if (value.ValueType == CssValueType.Identifier)
                        {
                            if (familyNameIdentifier != string.Empty)
                                familyNameIdentifier += " ";
                            familyNameIdentifier += ((CssIdentifierValue)value).Value;
                        }
                        else if (value.ValueType == CssValueType.Comma)
                        {
                            Debug.Assert(familyNameIdentifier != string.Empty);
                            CssValue fontFamilyValue = (familyNameIdentifier.Contains(" "))
                                ? (CssValue)new CssStringValue(familyNameIdentifier)
                                : new CssIdentifierValue(familyNameIdentifier);
                            fontFamilyValues.Add(fontFamilyValue);
                            familyNameIdentifier = string.Empty;
                        }
                        else
                        {
                            legalValues = false;
                        }

                        break;
                    }
                    case CssValueType.String:
                        legalValues = value.ValueType == CssValueType.Comma;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
                prevValueType = value.ValueType;
                i++;
            }
            if (legalValues && (familyNameIdentifier != string.Empty))
            {
                fontFamilyValues.Add(CssValue.CreateFontFamilyValue(familyNameIdentifier));
            }

            return legalValues
                ? new CssFontFamilyPropertyValue(fontFamilyValues)
                : null;
        }
    }
}
