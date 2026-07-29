// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2009 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.Fonts;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// Represents a collection of fonts in the font scheme, allowing to specify different fonts for different languages <see cref="Latin"/>, <see cref="EastAsian"/> and <see cref="ComplexScript"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-styles-and-themes/">Working with Styles and Themes</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// Corresponds to 5.1.4.1.24 majorFont (Major Font) or 5.1.4.1.25 minorFont (Minor fonts).
    /// </dev>
    public class ThemeFonts
    {
        internal ThemeFonts()
        {
        }

        /// <summary>
        /// Clones this instance of theme fonts.
        /// </summary>
        internal ThemeFonts Clone()
        {
            ThemeFonts lhs = (ThemeFonts)MemberwiseClone();

            if (ComplexScriptFontInfo != null)
                lhs.ComplexScriptFontInfo = ComplexScriptFontInfo.Clone();

            if (EastAsianFontInfo != null)
                lhs.EastAsianFontInfo = EastAsianFontInfo.Clone();

            if (LatinFontInfo != null)
                lhs.LatinFontInfo = LatinFontInfo.Clone();

            lhs.mSupplementalFonts = new Dictionary<string, ThemeSupplementalFont>();
            foreach (KeyValuePair<string, ThemeSupplementalFont> entry in mSupplementalFonts)
                lhs.mSupplementalFonts.Add(entry.Key, entry.Value.Clone());

            return lhs;
        }

        /// <summary>
        /// Specifies font name for Latin characters.
        /// </summary>
        public string Latin
        {
            get { return LatinFontInfo != null ? LatinFontInfo.Name : ""; }
            set
            {
                if (value == Latin)
                    return;

                LatinFontInfo = StringUtil.HasChars(value) ? new FontInfo(value) : null;
                mIsModified = true;
            }
        }

        /// <summary>
        /// Specifies font name for EastAsian characters.
        /// </summary>
        public string EastAsian
        {
            get { return EastAsianFontInfo != null ? EastAsianFontInfo.Name : ""; }
            set
            {
                if (value == EastAsian)
                    return;

                EastAsianFontInfo = StringUtil.HasChars(value) ? new FontInfo(value) : null; 
                mIsModified = true;
            }
        }

        /// <summary>
        /// Specifies font name for ComplexScript characters.
        /// </summary>
        public string ComplexScript
        {
            get { return ComplexScriptFontInfo != null ? ComplexScriptFontInfo.Name : ""; }
            set
            {
                if (value == ComplexScript)
                    return;

                ComplexScriptFontInfo = StringUtil.HasChars(value) ? new FontInfo(value) : null;
                mIsModified = true;
            }
        }

        /// <summary>
        /// Can be null.
        /// </summary>
        internal FontInfo ComplexScriptFontInfo;
        /// <summary>
        /// Can be null.
        /// </summary>
        internal FontInfo EastAsianFontInfo;
        /// <summary>
        /// Can be null.
        /// </summary>
        internal FontInfo LatinFontInfo;

        /// <summary>
        /// Key is a string value of <see cref="ThemeSupplementalFont.Script"/>, 
        /// value is a <see cref="ThemeSupplementalFont"/> object.
        /// </summary>
        internal IDictionary<string, ThemeSupplementalFont> SupplementalFonts
        {
            get { return mSupplementalFonts; }
        }

        /// <summary>
        /// Indicates that the object was modified by public accessors.
        /// </summary>
        internal bool IsModified
        {
            get { return mIsModified; }
        }

        private bool mIsModified;

        private Dictionary<string, ThemeSupplementalFont> mSupplementalFonts = 
            new Dictionary<string, ThemeSupplementalFont>();
    }
}
