// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2009 by Roman Korchagin

namespace Aspose.Words.Themes
{
    /// <summary>
    /// Corresponds to 5.1.4.1.16 font (Font)
    /// </summary>
    internal class ThemeSupplementalFont
    {
        internal ThemeSupplementalFont(string script, string typeface)
        {
            mScript = script;
            mTypeface = typeface;
        }

        /// <summary>
        /// Clones this instance of supplemental font.
        /// </summary>
        internal ThemeSupplementalFont Clone()
        {
            return (ThemeSupplementalFont)MemberwiseClone();
        }

        /// <summary>
        /// Can be null.
        /// Specifies the script, or language, in which the typeface is supposed to be used.
        /// Cannot set because this is used as a key in the collection of supplemental fonts.
        /// 
        /// I think the values here are from this table http://unicode.org/iso15924/iso15924-codes.html
        /// </summary>
        internal string Script
        {
            get { return mScript; }
        }

        /// <summary>
        /// Can be null.
        /// Specifies the font face to use.
        /// </summary>
        internal string Typeface
        {
            get { return mTypeface; }
            set { mTypeface = value; }
        }

        private readonly string mScript;
        private string mTypeface;
    }
}
