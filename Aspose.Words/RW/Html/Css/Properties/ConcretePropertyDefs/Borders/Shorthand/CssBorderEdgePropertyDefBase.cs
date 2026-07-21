// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for 'border-[top|right|bottom|left]' CSS property.
    /// </summary>
    /// <remarks>
    /// This CSS property is a shorthand that sets the values of border-edge-color, border-edge-style, and border-edge-width. 
    /// The three values of the shorthand property can be specified in any order, and one or two of them may be omitted.
    /// </remarks>
    internal abstract class CssBorderEdgePropertyDefBase : CssOrderInsensitiveShorthandPropertyDef
    {
        protected CssBorderEdgePropertyDefBase(
            string shorthandPropertyName,
            string widthPropertyName,
            string stylePropertyName,
            string colorPropertyName)
            : base(shorthandPropertyName, false)
        {
            Debug.Assert(StringUtil.HasChars(widthPropertyName));
            Debug.Assert(StringUtil.HasChars(stylePropertyName));
            Debug.Assert(StringUtil.HasChars(colorPropertyName));

            Debug.Assert(StringUtil.IsAsciiLowerCase(widthPropertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(stylePropertyName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(colorPropertyName));

            WidthPropertyName = widthPropertyName;
            StylePropertyName = stylePropertyName;
            ColorPropertyName = colorPropertyName;
        }

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection declarations)
        {
            // Note that the shorthand property always sets all of the corresponding longhand properties.
            // If no value is specified for a longhand property, it is reset to the initial value.
            CssValue widthValue = GetLonghandValue(declarations, WidthPropertyName);
            CssValue styleValue = GetLonghandValue(declarations, StylePropertyName);
            CssValue colorValue = GetLonghandValue(declarations, ColorPropertyName);

            // All three longhand property values must be specified or equal to 'initial'.
            if ((widthValue == null) ||
                (styleValue == null) ||
                (colorValue == null))
            {
                return null;
            }

            if (widthValue.Equals(CssValue.Initial) &&
                styleValue.Equals(CssValue.Initial) &&
                colorValue.Equals(CssValue.Initial))
            {
                // This is the default value for the 'border-xxx-style' property, and 'border-xxx:none' is essentially
                // an equivalent of 'border-xxx:initial'. Since 'border-xxx:none' is used more often, we use this variant.
                return new CssPropertyValue(CssValue.None);
            }

            if (widthValue.Equals(CssValue.Inherit) &&
                styleValue.Equals(CssValue.Inherit) &&
                colorValue.Equals(CssValue.Inherit))
            {
                return new CssPropertyValue(CssValue.Inherit);
            }

            CssValueList values = new CssValueList();

            // No need to write 'initial' values, because they are implied by the shorthand property.
            if (!widthValue.Equals(CssValue.Initial))
            {
                values.Add(widthValue);
            }
            if (!styleValue.Equals(CssValue.Initial))
            {
                values.Add(styleValue);
            }
            if (!colorValue.Equals(CssValue.Initial))
            {
                values.Add(colorValue);
            }

            return new CssPropertyValue(values);
        }

        private static CssValue GetLonghandValue(
            CssDeclarationCollection declarations,
            string propertyName)
        {
            CssDeclaration declaration = declarations[propertyName];
            if ((declaration == null) || (declaration.Value.Count != 1))
            {
                return null;
            }
            CssIndividualPropertyDef propertyDef = CssPropertyDefFactory.GetIndividualPropertyDef(propertyName);
            if (propertyDef == null)
            {
                return null;
            }
            CssValue value = declaration.Value.FirstValue;
            if (Equals(propertyDef.InitialValue, value))
            {
                return CssValue.Initial;
            }
            return value;
        }

        protected override ShorthandPropertyPart[] GetIndividualProperties()
        {
            if (mIndividualProperties != null)
                return mIndividualProperties;

            mIndividualProperties = new ShorthandPropertyPart[]
            {
                new ShorthandPropertyPart(WidthPropertyName, false),
                new ShorthandPropertyPart(StylePropertyName, false),
                new ShorthandPropertyPart(ColorPropertyName, false)
            };
            return mIndividualProperties;
        }

        protected string WidthPropertyName { get; }

        protected string StylePropertyName { get; }

        protected string ColorPropertyName { get; }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
