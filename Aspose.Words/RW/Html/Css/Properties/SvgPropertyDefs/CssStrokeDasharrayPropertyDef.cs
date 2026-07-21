// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2017 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'stroke-dasharray' CSS property for SVG.
    /// </summary>
    internal class CssStrokeDasharrayPropertyDef : CssIndividualPropertyDef
    {
        internal CssStrokeDasharrayPropertyDef()
            : base("stroke-dasharray", true, CssDashArrayPropertyValue.None)
        {
            // Nothing to do.
        }

        protected override CssPropertyValue CreatePropertyValue(
            CssValueList cssValues,
            bool isInQuirksMode)
        {
            // stroke-dasharray CSS property accepts the values: none | < dasharray > | inherit.
            CssPropertyValue propertyValue;
            if ((cssValues.Count == 1) && cssValues[0].Equals(CssValue.None))
            {
                propertyValue = CssDashArrayPropertyValue.None;
            }
            else if (cssValues.Count >= 1)
            {
                bool legalValues = true;
                CssValueList values = new CssValueList();
                foreach (CssValue value in cssValues)
                {
                    if ((value.ValueType != CssValueType.Number) &&
                        (value.ValueType != CssValueType.Length) &&
                        (value.ValueType != CssValueType.Percentage) &&
                        (value.ValueType != CssValueType.Comma))
                    {
                        legalValues = false;
                        break;
                    }

                    if (value.ValueType != CssValueType.Comma)
                        values.Add(value);

                }

                propertyValue = (legalValues)
                    ? new CssDashArrayPropertyValue(values)
                    : null;
            }
            else
            {
                propertyValue = null;
            }

            return propertyValue;
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
    }
}
