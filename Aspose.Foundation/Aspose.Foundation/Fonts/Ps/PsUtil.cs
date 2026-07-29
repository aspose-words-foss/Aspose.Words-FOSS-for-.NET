// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/07/2013 by Alexey Noskov

using System;
using Aspose.Collections.Generic;

namespace Aspose.Fonts.Ps
{
    /// <summary>
    /// Contains static utility methods for PostScript fonts.
    /// </summary>
    internal static class PsUtil
    {
        internal static string GetGlyphName(int glyphCode)
        {
            // Generate glyph names.
            if (glyphCode == 0 || glyphCode == 0xffff)
                return ".notdef";

            return (IsIsoLatinGlyph(glyphCode))
                ? gIsoLatin1Encoding[glyphCode]
                : string.Format("uni{0:X4}", glyphCode);
        }

        internal static bool IsIsoLatinGlyph(int glyphCode)
        {
            return glyphCode < gIsoLatin1Encoding.Length;
        }

        /// <summary>
        /// Returns sorted by glyphs index array of CMap entries.
        /// </summary>
        internal static PsCMapEntry[] GetUsedIndices(SortedIntegerListGeneric<int> usedChars)
        {
            PsCMapEntry[] indices = new PsCMapEntry[usedChars.Count];

            for (int i = 0; i < usedChars.Count; i++)
            {
                PsCMapEntry entry = new PsCMapEntry(usedChars.GetKey(i), usedChars.GetByIndex(i));
                indices[i] = entry;
            }

            Array.Sort(indices);
            return indices;
        }
        
        /// <summary>
        /// Array of glyphs names for Latin encoding. 
        /// These names are used for generation Encoding in Type42 font if source true type font
        /// does not contain glyphs names ('post' table of version 3 or higher).
        /// </summary>
        private static readonly string[] gIsoLatin1Encoding = new string[]
        {
            ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef",
            ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef",
            ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef",
            ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef",
            "space", "exclam", "quotedbl", "numbersign", "dollar", "percent", "ampersand", "quoteright",
            "parenleft", "parenright", "asterisk", "plus", "comma", "minus", "period", "slash",
            "zero", "one", "two", "three", "four", "five", "six", "seven",
            "eight", "nine", "colon", "semicolon", "less", "equal", "greater", "question",
            "at", "A", "B", "C", "D", "E", "F", "G",
            "H", "I", "J", "K", "L", "M", "N", "O",
            "P", "Q", "R", "S", "T", "U", "V", "W",
            "X", "Y", "Z", "bracketleft", "backslash", "bracketright", "asciicircum", "underscore",
            "quoteleft", "a", "b", "c", "d", "e", "f", "g",
            "h", "i", "j", "k", "l", "m", "n", "o",
            "p", "q", "r", "s", "t", "u", "v", "w",
            "x", "y", "z", "braceleft", "bar", "braceright", "asciitilde", ".notdef",
            ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef",
            ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef", ".notdef",
            "dotlessi", "grave", "acute", "circumflex", "tilde", "macron", "breve", "dotaccent",
            "dieresis", ".notdef", "ring", "cedilla", ".notdef", "hungarumlaut", "ogonek", "caron",
            "space", "exclamdown", "cent", "sterling", "currency", "yen", "brokenbar", "section",
            "dieresis", "copyright", "ordfeminine", "guillemotleft", "logicalnot", "hyphen", "registered", "macron",
            "degree", "plusminus", "twosuperior", "threesuperior", "acute", "mu", "paragraph", "periodcentered",
            "cedilla", "onesuperior", "ordmasculine", "guillemotright", "onequarter", "onehalf", "threequarters", "questiondown",
            "Agrave", "Aacute", "Acircumflex", "Atilde", "Adieresis", "Aring", "AE", "Ccedilla",
            "Egrave", "Eacute", "Ecircumflex", "Edieresis", "Igrave", "Iacute", "Icircumflex", "Idieresis",
            "Eth", "Ntilde", "Ograve", "Oacute", "Ocircumflex", "Otilde", "Odieresis", "multiply",
            "Oslash", "Ugrave", "Uacute", "Ucircumflex", "Udieresis", "Yacute", "Thorn", "germandbls",
            "agrave", "aacute", "acircumflex", "atilde", "adieresis", "aring", "ae", "ccedilla",
            "egrave", "eacute", "ecircumflex", "edieresis", "igrave", "iacute", "icircumflex", "idieresis",
            "eth", "ntilde", "ograve", "oacute", "ocircumflex", "otilde", "odieresis", "divide",
            "oslash", "ugrave", "uacute", "ucircumflex", "udieresis", "yacute", "thorn", "ydieresis"
        };
    }
}
