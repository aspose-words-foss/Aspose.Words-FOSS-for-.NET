// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Collection of true type fonts of the same family name.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable and therefore thread-safe.
    /// </remarks>
    internal sealed class TTFontFamily
    {
        internal TTFontFamily(string familyName, IEnumerable<TTFont> fonts)
        {
            Debug.Assert(fonts != null);

            mFamilyName = familyName;
            mFonts = new IntToObjDictionary<TTFont>();
            foreach (TTFont font in fonts)
            {
                // We allow for font duplication in the source collection.
                mFonts[(int)font.Style] = font;
            }
        }

        internal string FamilyName
        {
            get { return mFamilyName; }
        }

        internal int Count
        {
            get { return mFonts.Count; }
        }

        /// <summary>
        /// Gets a font by style.
        /// </summary>
        /// <param name="style">The font style.</param>
        /// <param name="isExactStyle">When true, returns null if a font file with the
        /// requested style is not found. When false, returns a most suitable font
        /// within the same family if the font with the requested style is not found.</param>
        /// <returns>Can return null if no suitable font was found.</returns>
        internal TTFont GetFont(FontStyle style, bool isExactStyle)
        {
            //Try an exact match first.
            TTFont font = mFonts[(int)style];
            if (font != null)
                return font;

            if (isExactStyle)
                return null;

            // Try a non underline font
            FontStyle tryStyle = (style & (~FontStyle.Underline));
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            // Try a non strikeout font
            tryStyle = (style & (~FontStyle.Strikeout));
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            //Try a non italic font.
            tryStyle = (style & (~FontStyle.Italic));
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            //Try a non bold font. Need to try this last - more precedence since it is more important
            //to get wide characters than to get italic characters. Italic is easier to simulate.
            tryStyle = (style & (~FontStyle.Bold));
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            //Now try the regular font.
            tryStyle = FontStyle.Regular;
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            // WORDSNET-24634 In case when regular style is not available MW seems to prefer Italic over Bold or BoldItalic.
            tryStyle = FontStyle.Italic;
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            // WORDSNET-25845 In this case there is only bold styles in family. We should use BoldItalic for Italic instead of Bold.
            tryStyle = style | FontStyle.Bold;
            font = mFonts[(int)tryStyle];
            if (font != null)
                return font;

            //Finally, return any font.
            foreach (TTFont obj in mFonts.Values)
                return obj;

            return null;
        }

        private readonly string mFamilyName;

        /// <summary>
        /// Map of FontStyle to TTFont.
        /// </summary>
        /// <remarks>AS It looks like only Regular, Bold, Italic and Bold Italic should be used as keys.
        /// The other font style bits represent internal font information only.
        /// See http://http://www.microsoft.com/typography/otspec/name.htm: "Up to four fonts
        /// can share the Font Family name, forming a font style linking group (regular, italic, bold,
        /// bold italic - as defined by OS/2.fsSelection bit settings)".
        /// </remarks>
        private readonly IntToObjDictionary<TTFont> mFonts;
    }
}
