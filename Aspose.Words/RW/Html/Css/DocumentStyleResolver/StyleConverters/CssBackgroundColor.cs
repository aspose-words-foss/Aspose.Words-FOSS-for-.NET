// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/01/2017 by Victor Chebotok

using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Stores 'background-color' CSS value. Used for background color resolution.
    /// </summary>
    internal class CssBackgroundColor
    {
        internal CssBackgroundColor(CssDeclarationCollection declarations)
            : this(declarations["background-color"])
        {
            // Empty constructor.
        }

        internal CssBackgroundColor(CssDeclaration declaration)
        {
            if (declaration == null)
            {
                return;
            }

            DrColor color = declaration.Value.ParseAsColor();
            if (color != null)
            {
                IsDefined = true;
                mColor = color;
            }
        }

        internal void ToShading(IRunAttrSource runAttrSource)
        {
            if (IsDefined)
            {
                Shading shading = new Shading();
                ToShading(shading);
                runAttrSource.SetRunAttr(FontAttr.Shading, shading);
            }
        }

        internal void ToShading(Shading shading)
        {
            if (IsDefined)
            {
                // Plain background color corresponds to BackgroundPatternColor and TextureNone in the model.
                shading.Texture = TextureIndex.TextureNone;
                shading.BackgroundPatternColorInternal = mColor;
            }
        }

        internal bool IsDefined { get; }

        private readonly DrColor mColor;
    }
}
