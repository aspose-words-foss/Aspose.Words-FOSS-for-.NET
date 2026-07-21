// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2020 by Artem Shabarshin

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for '-aw-border-[top|right|bottom|left]' CSS extended border property.
    /// </summary>
    /// <remarks>
    /// This CSS property is a shorthand that sets the values of -aw-border-edge-color, -aw-border-edge-style, and -aw-border-edge-width. 
    /// The three values of the shorthand property can be specified in any order, and one or two of them may be omitted.
    /// </remarks>
    internal abstract class CssAWBorderEdgePropertyDefBase : CssOrderInsensitiveShorthandPropertyDef
    {
        protected CssAWBorderEdgePropertyDefBase(
            string shorthandPropertyName,
            string widthPropertyName,
            string stylePropertyName,
            string colorPropertyName) :
            base(shorthandPropertyName, false)
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

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations)
        {
            CssValueList values = new CssValueList();

            CssDeclaration widthDeclaration = individualDeclarations[WidthPropertyName];
            if (widthDeclaration != null)
            {
                values.Add(widthDeclaration.Value.FirstValue);
            }

            CssDeclaration styleDeclaration = individualDeclarations[StylePropertyName];
            if (styleDeclaration != null)
            {
                values.Add(styleDeclaration.Value.FirstValue);
            }

            CssDeclaration colorDeclaration = individualDeclarations[ColorPropertyName];
            if (colorDeclaration != null)
            {
                values.Add(colorDeclaration.Value.FirstValue);
            }

            return new CssPropertyValue(values);
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
