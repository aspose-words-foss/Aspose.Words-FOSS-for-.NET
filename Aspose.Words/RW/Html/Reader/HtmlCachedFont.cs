// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2014 by Victor Chebotok

using System.Drawing;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Holds precalculated information about availability of glyphs in a font. This class is used to speed up
    /// glyph availability checks.
    /// </summary>
    internal class HtmlCachedFont
    {
        internal HtmlCachedFont(IFontProvider fontProvider, string familyName)
        {
            // We fetch fonts only to check glyph availability, so it is OK to use the regular font style here.
            // We do not expect fonts that differ just in style to contain different sets of glyphs.
            mFont = fontProvider.GetTTFont(familyName, FontStyle.Regular);

            mIsSymbolFont = familyName == "Symbol";
        }

        internal bool HasGlyphFor(int utf32Char)
        {
            // SPEED. Glyph availability for the Basic Multilingual Plane (BMP) is cached in order to speed up repeated checks.
            // WORDSNET-28606 We used to check and cache all BMP characters in advance but this proved to be slow. Now we
            // check and cache only BMP characters that really appear in the source document.
            if (utf32Char < mBmpGlyphAvailabilityCache.Length)
            {
                sbyte cachedResult = mBmpGlyphAvailabilityCache[utf32Char];
                if (cachedResult == 0)
                {
                    cachedResult = FontHasGlyphFor(utf32Char)
                        ? (sbyte)1
                        : (sbyte)-1;
                    mBmpGlyphAvailabilityCache[utf32Char] = cachedResult;
                }
                return cachedResult > 0;
            }

            // We do not cache glyph availability for Supplementary Planes, because they are uncommon and doing so would waste
            // a lot of memory.
            return FontHasGlyphFor(utf32Char);
        }

        /// <summary>
        /// Returns a fallback font family name for a character code.
        /// Returns null if no fallback font is found.
        /// </summary>
        internal string GetFallbackFontName(int utf32Char, IFontProvider fontProvider)
        {
            Debug.Assert(fontProvider != null);

            // WORDSNET-13609 Replace font for 'no-break space' characters (U+00A0) in 'Symbol' so that they will
            // not become visible in Word.
            if (mIsSymbolFont && (utf32Char == NoBreakSpace))
            {
                return "Times New Roman";
            }

            // WORDSNET-14917 We turn off AW's character replacements because they aren't suitable for HTML import.
            TTFont fallbackFont = fontProvider.GetFallbackFont(mFont, utf32Char, false);
            return fallbackFont != null ? fallbackFont.FamilyName : null;
        }

        private static bool IsControlCharacter(int utf32Char)
        {
            return ((utf32Char >= 0x00) && (utf32Char <= 0x1F)) ||
                ((utf32Char >= 0x80) && (utf32Char <= 0x9F));
        }

        private bool FontHasGlyphFor(int utf32Char)
        {
            // WORDSNET-13609 Replace font for 'no-break space' characters (U+00A0) in 'Symbol' so that they
            // will not become visible in Word.
            if (mIsSymbolFont && (utf32Char == NoBreakSpace))
            {
                return false;
            }

            // WORDSNET-23599 Use the U+0020 `Space` character instead of U+2002 'En Space' and U+2003 'Em Space'
            // when checking for glyph availability. Mimic MS Word behavior.
            if ((utf32Char == 0x2002) || (utf32Char == 0x2003))
                utf32Char = 0x20;

            // 1. Control characters have a special meaning, and we skip them during checks (pretend that every font contains
            //    glyphs for control characters).
            // 2. If the font is not installed (is null), we cannot get its glyphs. In this case, we act as if the font
            //    contained all required glyphs thus effectively ignoring unknown fonts.
            // 3. Try to find glyph in Private Use Area.
            // WORDSNET-14917 We turn off AW's character replacements because they aren't suitable for HTML import.
            return IsControlCharacter(utf32Char) ||
                (mFont == null) ||
                mFont.Glyphs.ContainsCharCode(utf32Char, false) ||
                IsGlyphMappedToPrivateUseArea(utf32Char);
        }

        /// <summary>
        /// When a symbolic font is loaded to AW, its glyphs are available via PUA character codes.
        /// However, in IE, Chrome, MS Word symbolic glyphs are also available via indexes.
        /// This function checks if glyph is mapped to PUA.
        /// </summary>
        private bool IsGlyphMappedToPrivateUseArea(int utf32Char)
        {
            if (!mFont.IsSymbolic)
                return false;

            if ((utf32Char < 0x20) || (utf32Char > 0xFF))
                return false;

            return mFont.Glyphs.ContainsCharCode(0xF000 + utf32Char, false);
        }

        /// <summary>
        /// The 'NO-BREAK SPACE' character code.
        /// </summary>
        private const int NoBreakSpace = 0xA0;

        private readonly TTFont mFont;

        /// <summary>
        /// Cached glyph availability information for characters of the Basic Multilingual Plane (most common characters).
        /// </summary>
        /// <remarks>
        /// Values:
        ///      0 = not checked yet;
        ///      1 = glyph is available;
        ///     -1 = no glyph
        /// </remarks>
        private readonly sbyte[] mBmpGlyphAvailabilityCache = new sbyte[0x10000];

        /// <summary>
        /// Indicates whether this instance represents the 'Symbol' font.
        /// </summary>
        private readonly bool mIsSymbolFont;
    }
}
