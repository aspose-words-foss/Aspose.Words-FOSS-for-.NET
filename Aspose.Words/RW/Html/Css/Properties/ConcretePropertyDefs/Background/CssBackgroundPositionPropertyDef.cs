// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'background-position' CSS property.
    /// </summary>
    internal class CssBackgroundPositionPropertyDef : CssIndividualPropertyDef
    {
        internal CssBackgroundPositionPropertyDef() :
            base("background-position", false, new CssBackgroundPositionPropertyValue(CssValue.ZeroPercentage, CssValue.ZeroPercentage))
        {
        }

        internal override CssDeclaration CreateIndividualDeclaration(
            CssValueList cssValues,
            int startIndex,
            bool important,
            bool isInQuirksMode,
            out int affectedValues)
        {
            affectedValues = 0;
            if (startIndex >= cssValues.Count)
                return null;

            CssDeclaration declaration = null;
            if (IsSupportedXValue(cssValues[startIndex]) && (startIndex + 1 < cssValues.Count) &&
                IsSupportedYValue(cssValues[startIndex + 1]))
            {
                declaration = CreateDeclaration(
                    new CssValueList(cssValues[startIndex], cssValues[startIndex + 1]),
                    important,
                    isInQuirksMode);
                if (declaration != null)
                    affectedValues = 2;
            }

            if ((declaration == null) && IsSupportedXValue(cssValues[startIndex]))
            {
                declaration = CreateDeclaration(
                    new CssValueList(cssValues[startIndex]),
                    important,
                    isInQuirksMode);
                if (declaration != null)
                    affectedValues = 1;
            }

            return declaration;
        }

        internal override CssDeclaration CreateInheritDeclaration(bool important)
        {
            return new CssSpecifiedDeclaration(Name,
                new CssBackgroundPositionPropertyValue(CssValue.Inherit, CssValue.Inherit),
                important);
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            // [ [ <percentage> | <length> | left | center | right ] [ <percentage> | <length> | top | center | bottom ]? ] | [ [ left | center | right ] || [ top | center | bottom ] ] | inherit
            CssPropertyValue propertyValue = null;
            switch (cssValues.Count)
            {
                case 1:
                {
                    // If only one value is specified, the second value is assumed to be 'center'.
                    if (IsSupportedXValue(cssValues[0]))
                        propertyValue = new CssBackgroundPositionPropertyValue(cssValues[0], CssValue.Center);
                    break;
                }
                case 2:
                {
                    if (IsSupportedXValue(cssValues[0]) && IsSupportedYValue(cssValues[1]))
                        propertyValue = new CssBackgroundPositionPropertyValue(cssValues[0], cssValues[1]);
                    break;
                }
                default:
                    // SQ fix: nothing to do.
                    break;
            }

            return propertyValue;
        }

        private static bool IsSupportedXValue(CssValue cssValue)
        {
            return (cssValue.ValueType == CssValueType.Length) ||
                   (cssValue.ValueType == CssValueType.Percentage) ||
                   cssValue.Equals(CssValue.Left) ||
                   cssValue.Equals(CssValue.Center) ||
                   cssValue.Equals(CssValue.Right) ||
                   // WORDSNET-10223 We must read 'x' position in number values
                   // which is applied by some browsers in quirks mode.
                   (cssValue.ValueType == CssValueType.Number);
        }

        private static bool IsSupportedYValue(CssValue cssValue)
        {
            return (cssValue.ValueType == CssValueType.Length) ||
                   (cssValue.ValueType == CssValueType.Percentage) ||
                   cssValue.Equals(CssValue.Top) ||
                   cssValue.Equals(CssValue.Center) ||
                   cssValue.Equals(CssValue.Bottom) ||
                   // WORDSNET-10223 We must read 'y' position in number values
                   // which is applied by some browsers in quirks mode.
                   (cssValue.ValueType == CssValueType.Number);
        }
    }
}
