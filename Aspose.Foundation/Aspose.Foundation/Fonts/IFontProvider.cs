// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2015 by Konstantin Kornilov

using System.Drawing;
using Aspose.Fonts.TrueType;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    /// <summary>
    /// Interface for the font provider.
    /// </summary>
    public interface IFontProvider
    {
        /// <summary>
        /// Gets the specified font. Returns null if font is not available.
        /// </summary>
        TTFont GetTTFont(string familyName, FontStyle style);

        /// <summary>
        /// Fetches the TrueType font and tries to perform a simple font substitution.
        /// </summary>
        [JavaThrows(true)]
        TTFont FetchTTFont(string familyName, FontStyle style);

        /// <summary>
        /// Returns fallback font for the specified font and character code.
        /// Returns null if fallback font is not found.
        /// </summary>
        /// <param name="font">Font for which fallback font is searched.</param>
        /// <param name="charCode">Character code.</param>
        /// <param name="useCharacterReplacements">
        /// Specifies if character replacements should be used when checking fallback font for glyph availability.
        /// </param>
        TTFont GetFallbackFont(TTFont font, int charCode, bool useCharacterReplacements);
    }
}
