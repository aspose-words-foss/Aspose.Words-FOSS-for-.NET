// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/05/2016 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'list-style' shorthand CSS property.
    /// </summary>
    /// <remarks>
    /// The 'list-style' property is a shorthand notation for setting the three properties 
    /// 'list-style-type', 'list-style-image', and 'list-style-position' at the same place in the style sheet.
    /// https://www.w3.org/TR/CSS2/generate.html#propdef-list-style
    /// </remarks>
    internal class CssListStylePropertyDef : CssOrderInsensitiveShorthandPropertyDef
    {
        internal CssListStylePropertyDef()
            : base("list-style", true)
        {
            // Empty constructor.
        }

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations)
        {
            CssDeclaration typeDeclaration = individualDeclarations["list-style-type"];
            CssDeclaration imageDeclaration = individualDeclarations["list-style-image"];
            CssDeclaration positionDeclaration = individualDeclarations["list-style-position"];

            Debug.Assert((typeDeclaration != null) || (imageDeclaration != null) || (positionDeclaration != null));

            return new CssListStylePropertyValue(
                (typeDeclaration != null) ? typeDeclaration.Value.FirstValue : null,
                (imageDeclaration != null) ? imageDeclaration.Value.FirstValue : null,
                (positionDeclaration != null) ? positionDeclaration.Value.FirstValue : null);
        }

        protected override ShorthandPropertyPart[] GetIndividualProperties()
        {
            if (mIndividualProperties == null)
            {
                mIndividualProperties = new ShorthandPropertyPart[]
                {
                    new ShorthandPropertyPart("list-style-type", false),
                    new ShorthandPropertyPart("list-style-image", false),
                    new ShorthandPropertyPart("list-style-position", false)
                };
            }
            return mIndividualProperties;
        }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
