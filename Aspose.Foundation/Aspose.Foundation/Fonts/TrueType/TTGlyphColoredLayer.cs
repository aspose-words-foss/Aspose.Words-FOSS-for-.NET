// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/11/2021 by Konstantin Kornilov

using Aspose.Drawing;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents single layer of the layered colored glyph.
    /// </summary>
    public class TTGlyphColoredLayer
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public TTGlyphColoredLayer(int glyphIndex, DrColor color)
        {
            GlyphIndex = glyphIndex;
            Color = color;
        }

        /// <summary>
        /// Glyph index of the glyph in the layer.
        /// </summary>
        public int GlyphIndex { get; }

        /// <summary>
        /// Color of the layer.
        /// </summary>
        public DrColor Color { get; }
    }
}
