// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Konstantin Kornilov

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents the font family.
    /// </summary>
    public enum FontFamilyCore
    {
        /// <summary>
        /// Specifies a generic family name. This name is used when information about a font 
        /// does not exist or does not matter. The default font is used.
        /// </summary>
        Auto = 0,
        /// <summary>
        /// Specifies a proportional font with serifs. An example is Times New Roman.
        /// </summary>
        Roman = 1,
        /// <summary>
        /// Specifies a proportional font without serifs. An example is Arial.
        /// </summary>
        Swiss = 2,
        /// <summary>
        /// Specifies a monospace font with or without serifs. Monospace fonts are 
        /// usually modern; examples include Pica, Elite, and Courier New.
        /// </summary>
        Modern = 3,
        /// <summary>
        /// Specifies a font that is designed to look like handwriting; examples include Script and Cursive.
        /// </summary>
        Script = 4,
        /// <summary>
        /// Specifies a novelty font. An example is Old English.
        /// </summary>
        Decorative = 5
    }
}
