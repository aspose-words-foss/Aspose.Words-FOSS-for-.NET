// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Collections;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// A map of character codes into TTGlyph objects.
    /// </summary>
    public class TTGlyphHashtable
    {
        public TTGlyphHashtable(int numGlyphs)
        {
            CharMap = new IntToObjDictionary<TTGlyph>(numGlyphs);
            Glyphs = new IntToObjDictionary<TTGlyph>(numGlyphs);
        }

        /// <summary>
        /// Gets a glyph by a charCode.
        /// </summary>
        public TTGlyph GetGlyphByCharCode(int charCode)
        {
            // First try to find glyph for specified charCode.
            TTGlyph glyph = CharMap[charCode];
            if (glyph != null)
                return glyph;

            // If there is no glyph for the specified charCode then try to find glyph for the charCode replacements.
            IntList characters = GetCharacterReplacements(charCode);
            for (int i = 0; i < characters.Count; i++)
            {
                glyph = CharMap[characters[i]];
                if (glyph != null)
                    return glyph;
            }

            return null;
        }

        /// <summary>
        /// Returns a glyph for the character code.
        /// If the glyph is not found, returns missing glyph.
        /// </summary>
        public TTGlyph FetchGlyphByCharCode(int charCode)
        {
            TTGlyph glyph = GetGlyphByCharCode(charCode);
            if (glyph != null)
                return glyph;

            return MissingGlyph;
        }

        public int CharCodeToGlyphId(int charCode)
        {
            return FetchGlyphByCharCode(charCode).GlyphIndex;
        }

        /// <summary>
        /// Determines if glyph is available for specified charCode.
        /// </summary>
        public bool ContainsCharCode(int charCode)
        {
            return ContainsCharCode(charCode, true);
        }

        /// <summary>
        /// Determines if glyph is available for specified charCode.
        /// </summary>
        /// <param name="charCode">Character code.</param>
        /// <param name="useCharacterReplacements">
        /// Specifies if character replacements should be used during glyph lookup.
        /// </param>
        public bool ContainsCharCode(int charCode, bool useCharacterReplacements)
        {
            TTGlyph glyph = (useCharacterReplacements)
                ? GetGlyphByCharCode(charCode)
                : CharMap[charCode];
            return glyph != null;
        }

        private IntList GetCharacterReplacements(int c)
        {
            IntList replacements = new IntList();
            foreach (CharacterReplacerBase replacer in mReplacers)
                replacer.Replace(c, replacements);

            return replacements;
        }

        /// <summary>
        /// Returns a glyph by its index.
        /// </summary>
        public TTGlyph GetGlyphByIndex(int index)
        {
            return Glyphs[index];
        }

        /// <summary>
        /// Returns a glyph by its index.
        /// If the glyph is not found, returns missing glyph.
        /// </summary>
        public TTGlyph FetchGlyphByIndex(int index)
        {
            TTGlyph result = GetGlyphByIndex(index);
            return result != null ? result : MissingGlyph;
        }

        /// <summary>
        /// Adds glyph to the table.
        /// </summary>
        public void AddGlyph(TTGlyph glyph)
        {
            Glyphs[glyph.GlyphIndex] = glyph;
        }

        /// <summary>
        /// Adds char code mapping.
        /// </summary>
        public void AddCharCodeMapping(int charCode, int glyphIndex)
        {
            TTGlyph glyph = Glyphs[glyphIndex];

            CharMap[charCode] = glyph;

            // Use first charcode assigned for the glyph.
            if (glyph.CharCode == 0)
                glyph.CharCode = charCode;
        }

        /// <summary>
        /// This is the glyph that is defined in the font with the glyph index 0.
        /// </summary>
        public TTGlyph MissingGlyph { get; set; }

        /// <summary>
        /// Glyphs by index.
        /// </summary>
        public IntToObjDictionary<TTGlyph> Glyphs { get; }

        /// <summary>
        /// Character map. Key - character code, value - glyph.
        /// </summary>
        public IntToObjDictionary<TTGlyph> CharMap { get; }

        /// <summary>
        /// List of character replacers to use with this font.
        /// </summary>
        internal List<CharacterReplacerBase> Replacers
        {
            get { return mReplacers; }
            set { mReplacers = value; }
        }

        private List<CharacterReplacerBase> mReplacers = new List<CharacterReplacerBase>();
    }
}
