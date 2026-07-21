// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// Represents dml font.
    /// 21.1.2.3.1 cs (Complex Script Font)
    /// 21.1.2.3.3 ea (East Asian Font)
    /// 21.1.2.3.7 latin (Latin Font)
    /// 21.1.2.3.10 sym (Symbol Font)
    /// </summary>
    internal class DmlFont
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        internal DmlFont()
        {
        }

        /// <summary>
        /// Ctor with specifying font typeface.
        /// </summary>
        internal DmlFont(string typeface)
        {
            mTextTypeface = typeface;
        }

        /// <summary>
        /// Creates a copy of this object instance.
        /// </summary>
        internal DmlFont Clone()
        {
            return (DmlFont)MemberwiseClone();
        }

        /// <summary>
        /// Specifies the typeface, or name of the font that is to be used.
        /// The typeface is a string name of the specific font that should be
        /// used in rendering the presentation. If this font is not available
        /// within the font list of the generating application than font substitution
        /// logic should be utilized in order to select an alternate font.
        /// </summary>
        internal string TextTypeface
        {
            get
            {
                if (mTextTypeface == null)
                    mTextTypeface = String.Empty;
                return mTextTypeface;
            }
            set { mTextTypeface = value; }
        }

        /// <summary>
        /// Specifies the font pitch as well as the font family for the corresponding font.
        /// </summary>
        internal int SimilarFontFamily
        {
            get { return mSimilarFontFamily; }
            set { mSimilarFontFamily = value; }
        }

        /// <summary>
        /// Specifies the Panose-1 classification number for the current font
        /// using the mechanism defined in 4.2.7.17 of ISO/IEC 14496-22:2007.
        /// </summary>
        internal string PanoseSetting
        {
            get
            {
                if (mPanoseSetting == null)
                    mPanoseSetting = String.Empty;

                return mPanoseSetting;
            }
            set { mPanoseSetting = value; }
        }

        /// <summary>
        /// Specifies the character set which is supported by the parent font.
        /// This information can be used in font substitution logic to locate
        /// an appropriate substitute font when this font is not available.
        /// This information is determined by querying the font when present
        /// and shall not be modified when the font is not available.
        /// Default is 1.
        /// </summary>
        internal int SimilarCharacterSet
        {
            get { return mSimilarCharacterSet; }
            set { mSimilarCharacterSet = value; }
        }

        private string mPanoseSetting = String.Empty;
        private int mSimilarCharacterSet = 1;
        private int mSimilarFontFamily;
        private string mTextTypeface = String.Empty;
    }
}
