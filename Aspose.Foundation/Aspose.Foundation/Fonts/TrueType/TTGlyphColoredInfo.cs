// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2021 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents information about colored version of a glyph.
    /// </summary>
    /// <remarks>
    /// At the moment only layered colored glyphs are supported. Later the support of bitmap and SVG colored glyphs
    /// may be added.
    /// </remarks>
    public class TTGlyphColoredInfo
    {
        public TTGlyphColoredInfo(List<TTGlyphColoredLayer> layers)
        {
            Layers = layers;
        }

        /// <summary>
        /// List of colored layers.
        /// </summary>
        public List<TTGlyphColoredLayer> Layers { get; }
    }
}
