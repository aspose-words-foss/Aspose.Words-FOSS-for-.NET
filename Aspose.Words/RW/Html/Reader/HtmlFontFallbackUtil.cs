// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/11/2014 by Victor Chebotok

using Aspose.Bidi;
using Aspose.Collections;
using Aspose.Fonts;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Provides methods to get fallback fonts required to render a text.
    /// </summary>
    internal class HtmlFontFallbackUtil
    {
        internal HtmlFontFallbackUtil(IFontProvider fontProvider)
        {
            Debug.Assert(fontProvider != null);
            mFontProvider = fontProvider;
        }

        /// <summary>
        /// Gets fallback fonts required to render a text.
        /// </summary>
        internal HtmlFontFallbackRange[] GetFontFallbackRanges(string text, Font font)
        {
            Debug.Assert(font != null);

            int[] missingCharacterIndexes = GetIndexesOfCharactersWithoutGlyphs(text, font);

            return (missingCharacterIndexes.Length == 0)
                // The fast path. No font changes are needed.
                ? new HtmlFontFallbackRange[] { new HtmlFontFallbackRange(text) }
                // The slow path. Find fallback fonts for each missing glyph.
                : SplitIntoFontFallbackRanges(text, font, missingCharacterIndexes);
        }

        /// <summary>
        /// Gets indexes of characters that have no glyphs in the specified fonts and should be rendered with fallback fonts.
        /// </summary>
        private int[] GetIndexesOfCharactersWithoutGlyphs(string text, Font font)
        {
            // We delay instatiation of the resulting list, because it is rarely needed (situations where a font contains
            // no glyph for a character are uncommon).
            IntList result = null;

            // The category here was chosen arbitrarily. We just need something to start with.
            CharacterCategory lastCategory = CharacterCategory.Ascii;
            HtmlCachedFont cachedFont = GetCachedFont(lastCategory, font);

            // Process the text character-by-character.
            StringUtf32Enumerator enumerator = new StringUtf32Enumerator(text);
            while (enumerator.MoveNext())
            {
                int code = enumerator.Current;
                CharacterCategory category = GetCharacterCategory(code);

                // The font changes only when the character category changes, so we can reuse the last font if the category
                // has not changed.
                if (category != lastCategory)
                {
                    cachedFont = GetCachedFont(category, font);
                    lastCategory = category;
                }

                // Check glyph availability.
                if (!cachedFont.HasGlyphFor(code))
                {
                    // Delayed instatiation of the resulting list.
                    if (result == null)
                    {
                        result = new IntList();
                    }

                    result.Add(enumerator.Offset);
                }
            }

            return (result == null)
                ? gEmptyIndexes
                : result.ToArray();
        }

        /// <summary>
        /// Splits a text into ranges by fallback font required to render them.
        /// </summary>
        private HtmlFontFallbackRange[] SplitIntoFontFallbackRanges(string text, Font font, int[] missingCharacterIndexes)
        {
            int lastUnprocessedIndex = 0;
            HtmlFontFallbackRangeBuilder rangeBuilder = new HtmlFontFallbackRangeBuilder();
            foreach (int missingCharacterIndex in missingCharacterIndexes)
            {
                Debug.Assert(missingCharacterIndex >= lastUnprocessedIndex);

                // Append a range of characters that do not require fallback (have glyphs in the font).
                if (missingCharacterIndex > lastUnprocessedIndex)
                {
                    string rangeText = text.Substring(lastUnprocessedIndex, missingCharacterIndex - lastUnprocessedIndex);
                    rangeBuilder.Append(rangeText, null);
                }

                // Determine a fallback font required to render the current missing character.
                int missingCharacterCode = UnicodeUtil.ConvertToUtf32(text, missingCharacterIndex);
                CharacterCategory category = GetCharacterCategory(missingCharacterCode);
                HtmlCachedFont cachedFont = GetCachedFont(category, font);
                // If the fallback font cannot be found, the fallback font name will be null
                // and font fallback will be disabled for the character.
                string fallbackFontName = cachedFont.GetFallbackFontName(missingCharacterCode, mFontProvider);
                // Append the current missing character to fallback ranges.
                string missingCharacterText = UnicodeUtil.ConvertFromUtf32(missingCharacterCode);
                rangeBuilder.Append(missingCharacterText, fallbackFontName);
                lastUnprocessedIndex = missingCharacterIndex + missingCharacterText.Length;
            }
            // Append the last range of characters that do not require fallback. This range lasts from the last missing character
            // to the end of text.
            if (lastUnprocessedIndex < text.Length)
            {
                rangeBuilder.Append(text.Substring(lastUnprocessedIndex), null);
            }

            return rangeBuilder.GetRanges();
        }

        private static CharacterCategory GetCharacterCategory(int utf32Char)
        {
            // We should probably also consider character category hints here, but this would complicate the code.
            // It is unlikely that we will have a category hint, however, because this code is used mainly when building
            // a document from scratch.
            return (utf32Char <= 0xFFFF)
                ? WordUtil.GetCharacterCategory((char)utf32Char)
                : CharacterCategory.Other;
        }

        private HtmlCachedFont GetCachedFont(CharacterCategory category, Font font)
        {
            string familyName;
            switch (category)
            {
                case CharacterCategory.Ascii:
                    familyName = font.NameAscii;
                    break;
                case CharacterCategory.ComplexScript:
                    familyName = font.NameBi;
                    break;
                case CharacterCategory.FarEast:
                    familyName = font.NameFarEast;
                    break;
                case CharacterCategory.Other:
                    familyName = font.NameOther;
                    break;
                default:
                    Debug.Fail("Unknown character category.");
                    familyName = font.NameOther;
                    break;
            }

            HtmlCachedFont result = mCachedFonts[familyName];
            if (result == null)
            {
                result = new HtmlCachedFont(mFontProvider, familyName);
                mCachedFonts.Add(familyName, result);
            }
            return result;
        }

        private readonly IFontProvider mFontProvider;

        /// <summary>
        /// Caches information about availability of glyphs in each font family. Keys are font family names (strings).
        /// Values are instances of <see cref="HtmlCachedFont"/>.
        /// </summary>
        private readonly StringToObjDictionary<HtmlCachedFont> mCachedFonts = new StringToObjDictionary<HtmlCachedFont>(false);

        private static readonly int[] gEmptyIndexes = new int[0];
    }
}
