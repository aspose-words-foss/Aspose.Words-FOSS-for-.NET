// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'background' shorthand CSS property.
    /// </summary>
    /// <remarks>
    /// The background shorthand property sets all the background properties in one declaration.
    /// The properties that can be set, are: background-color, background-position, background-size, 
    /// background-repeat, background-origin, background-clip, background-attachment, and background-image.
    /// Syntax:
    ///   background: color position size repeat origin clip attachment image;
    /// </remarks>
    internal class CssBackgroundPropertyDef : CssOrderInsensitiveShorthandPropertyDef
    {
        internal CssBackgroundPropertyDef()
            : base("background", false)
        {
            // Empty constructor.
        }

        protected override CssPropertyValue CreateShorthandValue(CssDeclarationCollection individualDeclarations)
        {
            CssDeclaration colorDeclaration = individualDeclarations["background-color"];
            CssDeclaration imageDeclaration = individualDeclarations["background-image"];
            CssDeclaration repeatDeclaration = individualDeclarations["background-repeat"];
            CssDeclaration attachmentDeclaration = individualDeclarations["background-attachment"];
            CssDeclaration positionDeclaration = individualDeclarations["background-position"];

            Debug.Assert((colorDeclaration != null) || (imageDeclaration != null) ||
                         (repeatDeclaration != null) || (attachmentDeclaration != null) ||
                         (positionDeclaration != null));
            Debug.Assert((positionDeclaration == null) || (positionDeclaration.Value is CssBackgroundPositionPropertyValue));

            return new CssBackgroundPropertyValue(
                (colorDeclaration != null) ? colorDeclaration.Value.FirstValue : null,
                (imageDeclaration != null) ? imageDeclaration.Value.FirstValue : null,
                (repeatDeclaration != null) ? repeatDeclaration.Value.FirstValue : null,
                (attachmentDeclaration != null) ? attachmentDeclaration.Value.FirstValue : null,
                (positionDeclaration != null)
                    ? (CssBackgroundPositionPropertyValue)positionDeclaration.Value
                    : null);
        }

        protected override ShorthandPropertyPart[] GetIndividualProperties()
        {
            if (mIndividualProperties != null)
                return mIndividualProperties;

            mIndividualProperties = new ShorthandPropertyPart[]
            {
                new ShorthandPropertyPart("background-color", false),
                new ShorthandPropertyPart("background-image", false),
                new ShorthandPropertyPart("background-repeat", false),
                new ShorthandPropertyPart("background-attachment", false),
                new ShorthandPropertyPart("background-position", false)
            };
            return mIndividualProperties;
        }

        private ShorthandPropertyPart[] mIndividualProperties;
    }
}
