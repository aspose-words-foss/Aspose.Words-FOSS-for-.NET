// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'text-decoration' CSS property.
    /// </summary>
    internal class CssTextDecorationPropertyDef : CssIndividualPropertyDef
    {
        internal CssTextDecorationPropertyDef()
            : base("text-decoration", false, CssTextDecorationPropertyValue.None)
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
            Debug.Assert(false, "This property cannot be a shorthand property part.");
            affectedValues = 0;
            return null;
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            // text-decoration CSS property accepts the values: none | [ underline || overline || line-through || blink ] | inherit.
            CssPropertyValue propertyValue;
            if ((cssValues.Count == 1) && cssValues[0].Equals(CssValue.None))
            {
                propertyValue = CssTextDecorationPropertyValue.None;
            }
            else if (cssValues.Count >= 1)
            {
                bool allValuesAreValid = true;
                foreach (CssValue value in cssValues)
                {
                    if (!IsTextDecorationValue(value))
                    {
                        allValuesAreValid = false;
                        break;
                    }
                }
                propertyValue = allValuesAreValid
                    ? new CssTextDecorationPropertyValue(cssValues)
                    : null;
            }
            else
            {
                propertyValue = null;
            }

            return propertyValue;
        }

        private static bool IsTextDecorationValue(CssValue value)
        {
            return value.EqualsAny(CssValue.Underline, CssValue.Overline, CssValue.LineThrough, CssValue.Blink);
        }
    }
}
