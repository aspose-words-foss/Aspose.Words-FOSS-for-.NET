// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2014 by Alexey Noskov

namespace Aspose.Bidi
{
    public static class CharacterClassifier
    {
        #region General

        /// <summary>
        /// Indicates whether the specified character is a radix-10 digit.
        /// </summary>
        public static bool IsDecimalDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        /// <summary>
        /// Indicates whether the specified character is European numeral.
        /// </summary>
        public static bool IsEuropeanNumeral(char ch)
        {
            return (ch >= '0' && ch <= '9');
        }

        /// <summary>
        /// Indicates whether the specified character is uppercase or lowercase letter.
        /// </summary>
        public static bool IsLetter(char ch)
        {
            UnicodeGeneralCategory category = UnicodeCharacterDataResolver.GetUnicodeGeneralCategory(ch);
            return (category == UnicodeGeneralCategory.Lu || category == UnicodeGeneralCategory.Ll);
        }

        /// <summary>
        /// Indicates whether the specified character is punctuation or symbol.
        /// </summary>
        public static bool IsPunctuation(char ch)
        {
            UnicodeGeneralCategory category = UnicodeCharacterDataResolver.GetUnicodeGeneralCategory(ch);

            switch (category)
            {
                case UnicodeGeneralCategory.Ps:
                case UnicodeGeneralCategory.Pe:
                case UnicodeGeneralCategory.Pi:
                case UnicodeGeneralCategory.Pf:
                case UnicodeGeneralCategory.Po:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether the specified character is Mathematical operators.
        /// </summary>
        public static bool IsMathematicalOperators(char ch)
        {
            return (ch >= 0x2200 && ch <= 0x22FF);
        }
        #endregion

        #region RTL Scripts
        /// <summary>
        /// Indicates whether the specified character belongs to Arabic or Hebrew Unicode blocks.
        /// </summary>
        /// <remarks>
        /// Some characters returning by this function, actually, have [Inherited] Unicode Script and just belong to Arabic or Hebrew Unicode blocks.
        /// </remarks>
        public static bool IsRtlScript(char ch)
        {
            return IsHebrew(ch) || IsArabic(ch);
        }

        /// <summary>
        /// Indicates whether the specified character belongs to Hebrew Unicode blocks.
        /// </summary>
        public static bool IsHebrew(char ch)
        {
            return ((ch >= 0x0590 && ch <= 0x05FF) || (ch >= 0xFB1D && ch <= 0xFB4F));
        }

        /// <summary>
        /// Indicates whether the specified character belongs to Arabic Unicode blocks.
        /// </summary>
        public static bool IsArabic(int ch)
        {
            return UnicodeUtil.IsArabicCharacter(ch);
        }

        /// <summary>
        /// Indicates whether the specified character is numeral separator.
        /// </summary>
        /// <param name="ch">The character to evaluate.</param>
        /// <param name="isHebrewLocale">Indicates that current locale is Hebrew.</param>
        /// <returns>true if ch is a numeral separator; otherwise, false.</returns>
        public static bool IsNumeralSeparator(char ch, bool isHebrewLocale)
        {
            return (IsCommonNumeralSeparator(ch)
                || IsArabicNumeralSeparator(ch)
                || (isHebrewLocale && IsHebrewNumeralSeparator(ch)));
        }

        /// <summary>
        /// Indicates whether the specified character is common numeral separator.
        /// </summary>
        private static bool IsCommonNumeralSeparator(char ch)
        {
            switch (ch)
            {
                case '.':
                case ',':
                case ':':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether the specified character is Arabic numeral separator.
        /// </summary>
        public static bool IsArabicNumeralSeparator(char ch)
        {
            switch (ch)
            {
                case '\u060C':
                case '\u066B':
                case '\u066C':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether the specified character is Hebrew numeral separator.
        /// </summary>
        public static bool IsHebrewNumeralSeparator(char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                case '/':
                case '%':
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Chinese/Japanese
        /// <summary>
        /// Indicates whether the specified character is CJK (Chinese, Japanese or Korean)
        /// </summary>
        public static bool IsCjk(int ch)
        {
            // Unicode blocks from [CJK Radicals] to [Yi Radicals]
            if ((ch >= 0x2e80) && (ch <= 0xa4cf))
                return true;

            // Unicode block [Hangul Jamo Extended-A]
            if ((ch >= 0xa960) && (ch <= 0xa97f))
                return true;

            // Unicode blocks [Hangul Syllables] and [Hangul Jamo Extended-B]
            if ((ch >= 0xac00) && (ch <= 0xd7ff))
                return true;

            // Unicode block [CJK Compatibility Ideographs]
            if ((ch >= 0xf900) && (ch <= 0xfaff))
                return true;

            // Unicode block [CJK Compatibility Forms]
            if ((ch >= 0xfe30) && (ch <= 0xfe4f))
                return true;

            // Unicode block [Halfwidth and Fullwidth Forms]
            if ((ch >= 0xff00) && (ch <= 0xffef))
                return true;

            return false;
        }

        /// <summary>
        /// Indicates whether the specified character is CJK (Chinese, Japanese or Korean)
        /// </summary>
        public static bool IsCjk(char ch)
        {
            return IsCjk((int)ch);
        }

        /// <summary>
        /// Indicates whether the specified character belongs to Chinese or Japanese script.
        /// </summary>
        /// <remarks>
        /// Used in Java version of GraphicsPal.
        /// </remarks>
        public static bool IsChineseJapanese(char c)
        {
            return ((c >= 0x4E00 && c <= 0x9FFF) || // isChinese
                    (c >= 0x3040 && c <= 0x30FF) || // isJapaneseKana
                    (c >= 0xFF01 && c <= 0xFF5E) || // isFullWidthChar
                    (c >= 0x3000 && c <= 0x303F));  // isIdeographicPunctuation (source: http://www.fileformat.info/info/unicode/block/cjk_symbols_and_punctuation/index.htm)
        }

        /// <summary>
        /// Indicates whether the specified character belongs to Korean script.
        /// </summary>
        /// <remarks>
        /// Used in Java version of GraphicsPal.
        /// </remarks>
        public static bool IsKorean(char c)
        {
            return (c >= 0xA960) && (c <= 0xA97F) || // Unicode block [Hangul Jamo Extended-A]
                   (c >= 0xAC00 && c <= 0xD7AF);     // Unicode blocks [Hangul Syllables] and [Hangul Jamo Extended-B]
        }

        /// <summary>
        /// Indicates whether the specified code is a CJK character which is coded as surrogate pair.
        /// </summary>
        public static bool IsSurrogateCjk(int code)
        {
            // Unicode block [CJK Unified Ideographs Extension B]
            if ((code >= 0x20000) && (code <= 0x2a6df))
                return true;

            // Unicode block [CJK Unified Ideographs Extension C]
            if ((code >= 0x2a700) && (code <= 0x2b73f))
                return true;

            // Unicode block [CJK Unified Ideographs Extension D]
            if ((code >= 0x2b740) && (code <= 0x2b81f))
                return true;

            // Unicode block [CJK Unified Ideographs Extension E]
            if ((code >= 0x2b820) && (code <= 0x2ceaf))
                return true;

            // Unicode block [CJK Compatibility Ideographs Supplement U]
            if ((code >= 0x2f800) && (code <= 0x2fa1f))
                return true;

            return false;
        }

        #endregion

        #region Other

        /// <summary>
        /// Indicates whether the specified character belongs to Thai script.
        /// </summary>
        public static bool IsThai(char ch)
        {
            return (ch >= 0x0E00 && ch <= 0x0E7F);
        }

        /// <summary>
        /// Indicates whether the specified character belongs to Devanagari script.
        /// </summary>
        /// <remarks>
        /// Used in Java version of GraphicsPal
        /// </remarks>
        public static bool IsDevanagari(char ch)
        {
            return ((ch >= 0x0900 && ch <= 0x097F) || // Devanagari
                    (ch >= 0xA8E0 && ch <= 0xA8FF) || // Devanagari Extended
                    (ch >= 0x1CD0 && ch <= 0x1CFF));  // Vedic Extensions
        }

        /// <summary>
        /// Indicates whether the specified character belongs to Bengali script.
        /// </summary>
        /// <remarks>
        /// Used in Java version of GraphicsPal
        /// </remarks>
        public static bool IsBengali(char ch)
        {
            return (ch >= 0x0980 && ch <= 0x09FF);
        }


        #endregion
    }
}
