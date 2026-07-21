// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'size' CSS property of @page rule.
    /// </summary>
    /// <remarks>
    /// This property specifies the target size and orientation of the page box’s containing block. In the general case,
    /// where one page box is rendered onto one page sheet, the ‘size’ property also indicates the size of the destination page sheet.
    /// http://www.w3.org/TR/css3-page/#page-size-prop
    /// </remarks>
    internal class CssPageSizePropertyDef : CssIndividualPropertyDef
    {
        internal CssPageSizePropertyDef()
            : base("size", false, CssPageSizePropertyValue.CreateAuto())
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
            // Value: <length>{1,2} | auto | [ <page-size> || [ portrait | landscape] ]
            switch (cssValues.Count)
            {
                case 1:
                {
                    CssValue value = cssValues[0];

                    if (value.Equals(CssValue.Auto))
                        return CssPageSizePropertyValue.CreateAuto();

                    if (value.ValueType == CssValueType.Identifier)
                        return CssPageSizePropertyValue.CreatePageSize((CssIdentifierValue)value);

                    // MS Word accepts unitless lengths in this case.
                    CssLengthValue valueAsLength = value.ToLength(true);
                    if (valueAsLength != null)
                    {
                        return CssPageSizePropertyValue.CreateLength(valueAsLength);
                    }

                    break;
                }
                case 2:
                {
                    CssValue left = cssValues[0];
                    CssValue right = cssValues[1];
                    if ((left.ValueType == CssValueType.Identifier) && (right.ValueType == CssValueType.Identifier))
                    {
                        return CssPageSizePropertyValue.CreatePageSize((CssIdentifierValue)left, (CssIdentifierValue)right);
                    }

                    // MS Word accepts unitless lengths in this case.
                    CssLengthValue leftAsLength = left.ToLength(true);
                    CssLengthValue rightAsLength = right.ToLength(true);

                    if ((leftAsLength != null) && (rightAsLength != null))
                    {
                        return CssPageSizePropertyValue.CreateLength(leftAsLength, rightAsLength);
                    }

                    break;
                }
                default:
                    break;
            }
            return null;
        }
    }
}
