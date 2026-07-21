// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2012 by Andrey Soldatov

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Aspose.Common;

namespace Aspose.Numbering
{
    /// <summary>
    /// Converts a number to text using a specific style.
    /// </summary>
    public static class NumberConverterCore
    {
        /// <summary>
        /// Converts an int value into a string using the specified number style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="numberStyle">The number style to use.</param>
        /// <param name="isFirstLetterUppercase">Whether to make the first letter uppercase for textual formats:
        /// <see cref="NumberStyleCore.Number"/> and <see cref="NumberStyleCore.OrdinalText"/>.</param>
        /// <param name="language">The language to use in conversion.</param>
        public static string NumberToString(long value, NumberStyleCore numberStyle, bool isFirstLetterUppercase, Language language)
        {
            switch (numberStyle)
            {
                case NumberStyleCore.Aiueo:
                    return JapaneseNumber.ToAiueo(value, false);
                case NumberStyleCore.AiueoHalfWidth:
                    return JapaneseNumber.ToAiueo(value, true);
                case NumberStyleCore.Arabic:
                    return ToArabic(value);
                case NumberStyleCore.ArabicFullWidth:
                    return ToArabicFullWidth(value);
                case NumberStyleCore.Bullet:
                    Debug.Fail("Bullets must not be handled by NumberConverter.");
                    return string.Empty;
                case NumberStyleCore.Hebrew1:
                    return HebrewNumber.ToHebrew1(value);
                case NumberStyleCore.Hebrew2:
                    return HebrewNumber.ToHebrew2(value);
                case NumberStyleCore.Hex:
                    return ToHex(value);
                case NumberStyleCore.Iroha:
                    return JapaneseNumber.ToIroha(value);
                case NumberStyleCore.Kanji:
                    // Yes. ToJapaneseDigital corresponds to NumberStyle.Kanji, not NumberStyle.KanjiDigit.
                    return JapaneseNumber.ToJapaneseDigital(value, 0);
                case NumberStyleCore.KanjiDigit:
                    return JapaneseNumber.ToJapaneseCounting(value);
                case NumberStyleCore.LeadingZero:
                    return FormatterPal.IntToStrD2(value);
                case NumberStyleCore.LeadingZero2:
                    return FormatterPal.IntToStrD3(value);
                case NumberStyleCore.LeadingZero3:
                    return FormatterPal.IntToStrD4(value);
                case NumberStyleCore.LeadingZero4:
                    return FormatterPal.IntToStrD5(value);
                case NumberStyleCore.LowercaseLetter:
                    return ToLowercaseLetter(value, language);
                case NumberStyleCore.LowercaseRoman:
                    return ToLowercaseRoman(value);
                case NumberStyleCore.LowercaseRussian:
                    return ToLowercaseLetter(value, Language.RussianRussia);
                case NumberStyleCore.Number:
                    return TextualNumber.ToFullString(value, true, isFirstLetterUppercase, language);
                case NumberStyleCore.NumberInCircle:
                    return ToNumberInCircle(value);
                case NumberStyleCore.NumberInDash:
                    return ToNumberInDash(value);
                case NumberStyleCore.Ordinal:
                    return OrdinalNumber.ToOrdinalNumber(value);
                case NumberStyleCore.OrdinalText:
                    return TextualNumber.ToFullString(value, false, isFirstLetterUppercase, language);
                case NumberStyleCore.UppercaseLetter:
                    return ToUppercaseLetter(value, language);
                case NumberStyleCore.UppercaseRoman:
                    return ToUppercaseRoman(value);
                case NumberStyleCore.UppercaseRussian:
                    return ToUppercaseLetter(value, Language.RussianRussia);
                case NumberStyleCore.ChicagoManual:
                    return ToChicagoManualOfStyle(value);
                case NumberStyleCore.TradChinNum2:
                case NumberStyleCore.TradChinNum3:
                case NumberStyleCore.SimpChinNum3:
                case NumberStyleCore.SimpChinNum2:
                case NumberStyleCore.SimpChinNum1:
                // WORDSNET-28954 Implemented taiwaneseCounting numbering.
                case NumberStyleCore.TradChinNum1:
                    return ChineseNumber.ToChineseCountingSystem(value, numberStyle);
                case NumberStyleCore.Arabic1:
                case NumberStyleCore.Arabic2:
                    return ToArabicAlphaOrAbjad(value, numberStyle);
                case NumberStyleCore.Zodiac1:
                    return ZodiacNumber.ToZodiac1(value);
                case NumberStyleCore.Zodiac2:
                    return ZodiacNumber.ToZodiac2(value);
                case NumberStyleCore.Zodiac3:
                    return ZodiacNumber.ToZodiac3(value);
                case NumberStyleCore.GB1:
                    return ToDecimalEnclosedFullstop(value);
                case NumberStyleCore.GB2:
                    return ToDecimalEnclosedParen(value);
                case NumberStyleCore.GB3:
                    return ToDecimalEnclosedCircleChinese(value);
                case NumberStyleCore.GB4:
                    return ToIdeographEnclosedCircle(value);
                case NumberStyleCore.None:
                    return "";
                case NumberStyleCore.ThaiLetter:
                    // WORDSNET-16501 Process "ThaiLetter" number style.
                    return ToLowercaseLetter(value, Language.Thai);
                case NumberStyleCore.VietCardinalText:
                    return VietnameseNumber.ToVietnameseCardinal(value);
                case NumberStyleCore.ThaiArabic:
                    return ThaiArabicNumber.ToThaiArabic(value);
                // WORDSNET-26909 Implemented Korean Ganada format.
                case NumberStyleCore.Ganada:
                    return KoreanNumber.ToGanada(value);
                // WORDSNET-27187 Implemented KoreanDigital2 format.
                case NumberStyleCore.Hanja:
                    return KoreanNumber.ToKoreanDigital2(value);
                // WORDSNET-27573 Implemented Chosung format.
                case NumberStyleCore.Chosung:
                    return KoreanNumber.ToChosung(value);
                // WORDSNET-27573 Implemented Korean Legal format.
                case NumberStyleCore.Hangul:
                    return KoreanNumber.ToHangul(value);
                default:
                    return ToArabic(value);
            }
        }

        /// <summary>
        /// Converts an int value into a string using the specified number style and the locale identifier.
        /// </summary>
        public static string NumberToLocalizedString(long value, NumberStyleCore numberStyle, bool isFirstLetterUppercase, int localeId)
        {
            switch (numberStyle)
            {
                case NumberStyleCore.Ordinal:
                    return OrdinalNumber.ToLocalizedOrdinalNumber(value, localeId);
                default:
                    return NumberToString(value, numberStyle, isFirstLetterUppercase, (Language)localeId);
            }
        }

        /// <summary>
        /// Parses the specified sequence of characters to its numeric representation. 'A' means 0, 'B' means 1,
        /// ... , 'Z' means 25, 'AA' means 26, 'AB' means 27 and so forth. The characters are case insensitive.
        /// If was passed invalid column name returns negative value as error indication
        /// </summary>
        public static int ParseExcelColumnName(string text)
        {
            // WORDSNET-6456 Instead of throwing exception we return a negative value as error indication.
            // This is needed to mimic Word's behavior.
            if (!StringUtil.HasChars(text) || (text.Length > MaxExcelColumnNameLength))
                return InvalidColumnIndex;

            long result = 0;

            for (int i = 0; i < text.Length - 1; i++)
            {
                result += StringUtil.LetterToInt(text[i]) + 1;
                result *= AlphabetLength;
            }

            result += StringUtil.LetterToInt(text[text.Length - 1]);

            if (result > int.MaxValue)
                return InvalidColumnIndex;

            return (int)result;
        }

        /// <summary>
        /// Converts the specified value to its alphabetic representation. 0 means 'A', 1 means 'B', ...,
        /// 25 means 'Z', 26 means 'AA', 27 means 'AB' and so forth.
        /// If was passed invalid column index returns a null.
        /// </summary>
        public static string ToExcelColumnName(int value)
        {
            // WORDSNET-6456 Instead of throwing an exception, we return a null, and process it in the appropriate places.
            // This is needed to mimic Word's behavior.
            if (value < 0)
                return null;

            int pos = MaxExcelColumnNameLength;
            StringBuilder stringBuilder = new StringBuilder("FXSHRXX");

            while (value >= 0)
            {
                stringBuilder[--pos] = (char)(value % AlphabetLength + 'A');
                value = value / AlphabetLength - 1;
            }

            return stringBuilder.ToString(pos, MaxExcelColumnNameLength - pos);
        }

        /// <summary>
        /// MS Words supports numbers 1-392 for <see cref="NumberStyleCore.Hebrew1"/>, <see cref="NumberStyleCore.Arabic1"/>
        /// and <see cref="NumberStyleCore.Arabic2"/> styles and cycles larger numbers from 1 again. And so we do.
        /// </summary>
        internal static int GetCycledValue(long value)
        {
            return (int)((value - 1) % 392) + 1;
        }

        /// <summary>
        /// Returns an Arabic number in dash (- 1 -, - 2 -, - 3 -, ...).
        /// </summary>
        private static string ToNumberInDash(long value)
        {
            return string.Format("- {0} -", value);
        }

        /// <summary>
        /// Returns an Arabic number (1, 2, 3...).
        /// </summary>
        private static string ToArabic(long value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Returns a hexadecimal number (1CD...).
        /// </summary>
        private static string ToHex(long value)
        {
            return ((value >= int.MinValue) && (value <= int.MaxValue))
                ? FormatterPal.IntToStrX((int)value)
                : FormatterPal.Int64ToStrX(value);
        }

        /// <summary>
        /// Converts to symbols defined in Chicago (Chicago Manual of Style), see ISO29500 17.18.59 ST_NumberFormat for @ details.
        /// </summary>
        private static string ToChicagoManualOfStyle(long value)
        {
            return ToAlphabetCore(value, "*†‡§");
        }

        /// <summary>
        /// Returns a lower case Roman (i, ii, iii, ...).
        /// </summary>
        private static string ToLowercaseRoman(long value)
        {
            Debug.Assert((value >= 0) && (value <= 32767));

            List<int> thousands = GroupSplitter.SplitToGroups(value, 3);
            StringBuilder stringBuilder = new StringBuilder();

            if (thousands.Count > 1)
                for (int i = 0; i < thousands[1]; i++)
                    stringBuilder.Append(RomanThousand);

            List<int> digits = GroupSplitter.SplitToGroups(thousands[0], 1);
            for (int i = digits.Count - 1; i >= 0; i--)
                stringBuilder.Append(gRomanDigits[i, digits[i]]);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns an upper case Roman (I, II, III, ...).
        /// </summary>
        private static string ToUppercaseRoman(long value)
        {
            // We use the invariant version of ToUpper here, because this method must be locale-insensitive.
            // For example, we don't want 'i' to turn into 'İ' in the Turkish locale.
            return ToLowercaseRoman(value).ToUpperInvariant();
        }

        private static string ToArabicFullWidth(long value)
        {
            List<int> digits = GroupSplitter.SplitToGroups(value, 1);
            StringBuilder stringBuilder = new StringBuilder();

            // The 'Fullwidth Digit Zero' char has Unicode code 0xFF10.
            for (int i = digits.Count - 1; i >= 0; i--)
                stringBuilder.Append((char)('０' + digits[i]));

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns a number in circle (①, ②, ③, ..., ⑳, 21, ...).
        /// </summary>
        /// <remarks>
        /// MS Word does not allow 0 (zero) value for this numbering style but I don't see a reason
        /// why don't return "0" for the case. This probably will not happen for real documents.
        /// MS Word shows all numbers larger than 20 by Arabic style.
        /// </remarks>
        private static string ToNumberInCircle(long value)
        {
            Debug.Assert(value >= 0);
            return ToUnicodeCharacter(value, 0x2460, 0x2473);
        }

        /// <summary>
        /// Returns a number converted to the Arabic Alpha or the Arabic Abjad numeral system.
        /// </summary>
        private static string ToArabicAlphaOrAbjad(long value, NumberStyleCore style)
        {
            Debug.Assert(value >= 0);
            Debug.Assert(style == NumberStyleCore.Arabic1 || style == NumberStyleCore.Arabic2);

            if (value <= 0)
                return "";

            string digits = (style == NumberStyleCore.Arabic1 ? ArabicAlpha : ArabicAbjad);
            int cycledValue = GetCycledValue(value) - 1;
            return new string(digits[cycledValue % digits.Length], cycledValue / digits.Length + 1);
        }

        /// <summary>
        /// Returns a lower case or uppercase letter format.
        /// For now, used for Russian, English, Turkish, Thai alphabets and ChicagoManualOfStyle.
        ///  </summary>
        private static string ToAlphabetCore(long value, string alphabet)
        {
            // WORDSNET-5451 Do not throw here but simply return an empty string.
            if (value < 1)
                return string.Empty;

            if (value > int.MaxValue)
                return string.Empty;

            int intValue = (int)value;

            intValue--;

            int repeat = intValue / alphabet.Length;
            int posInAlphabet = intValue - (repeat * alphabet.Length);

            return new string(alphabet[posInAlphabet], repeat + 1);
        }

        /// <summary>
        /// Returns a lower case letter using specified <paramref name="language"/>.
        /// </summary>
        private static string ToLowercaseLetter(long value, Language language)
        {
            switch (language)
            {
                case Language.GreekGreece:
                {
                    return GreekNumber.ToGreek(value);
                }
                default:
                {
                    // By default return repeated letters of a specified language.
                    string alphabet = GetAlphabetLetters(language);
                    return ToAlphabetCore(value, alphabet);
                }
            }
        }

        /// <summary>
        /// Returns a upper case letter using specified <paramref name="language"/>.
        /// </summary>
        private static string ToUppercaseLetter(long value, Language language)
        {
            return ToLowercaseLetter(value, language).ToUpper(new CultureInfo((int)language));
        }

        /// <summary>
        /// Returns a set of letters (alphabet) for the specified <paramref name="language"/> in a lower case.
        /// </summary>
        private static string GetAlphabetLetters(Language language)
        {
            switch (language)
            {
                case Language.RussianRussia:
                    return "абвгдежзиклмнопрстуфхцчшщыэюя";
                case Language.TurkishTurkey:
                    return "abcçdefgğhıijklmnoöprsştuüvyz";
                case Language.Thai:
                    // Thai letters, which used in the Word lists.
                    // Thai letters range starts at 0x0E01 and ends at 0x0E2E.
                    // However, MSW does not use these chars: 0x0E03, 0x0E05, 0x0E06, 0x0E24, 0x0E26.
                    return "กขคงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬอฮ";
                case Language.SwedishSweden:
                    return "abcdefghijklmnopqrstuvwxyzåäö";
                default:
                    // Return English by default.
                    return "abcdefghijklmnopqrstuvwxyz";
            }
        }

        /// <summary>
        /// Returns decimal numbers followed by a period: ⒈, ⒉, ⒊, ...
        /// </summary>
        private static string ToDecimalEnclosedFullstop(long value)
        {
            // The numbers 1-20 are represented with Unicode characters U+2488 – U+249B respectively.
            return ToUnicodeCharacter(value, 0x2488, 0x249B);
        }

        /// <summary>
        /// Returns decimal numbers enclosed in parenthesis: ⑴, ⑵, ⑶, ...
        /// </summary>
        private static string ToDecimalEnclosedParen(long value)
        {
            // The numbers 1-20 are represented with Unicode characters U+2474 – U+2487 respectively.
            return ToUnicodeCharacter(value, 0x2474, 0x2487);
        }

        /// <summary>
        /// Returns decimal numbers enclosed in a circle: ①, ②, ③, ...
        /// </summary>
        private static string ToDecimalEnclosedCircleChinese(long value)
        {
            // Identical to decimalEnclosedCircle.
            return ToNumberInCircle(value);
        }

        /// <summary>
        /// Returns ideographs enclosed in a circle: ㈠, ㈡, ㈢, ...
        /// </summary>
        private static string ToIdeographEnclosedCircle(long value)
        {
            // The numbers 1-10 are represented with Unicode characters U+3220 – U+3229 respectively.
            return ToUnicodeCharacter(value, 0x3220, 0x3229);
        }

        /// <summary>
        /// Converts numbers 1 - N to Unicode characters U+<paramref name="minCharCode"/> – U+<paramref name="maxCharCode"/> respectively.
        /// For all other values, returns an Arabic number.
        /// </summary>
        private static string ToUnicodeCharacter(long value, int minCharCode, int maxCharCode)
        {
            long valueCharCode = value - 1 + minCharCode;
            if (valueCharCode >= minCharCode && valueCharCode <= maxCharCode)
                return ((char)valueCharCode).ToString();

            return ToArabic(value);
        }

        private static readonly string[,] gRomanDigits =
        {
            { "", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix" },
            { "", "x", "xx", "xxx", "xl", "l", "lx", "lxx", "lxxx", "xc" },
            { "", "c", "cc", "ccc", "cd", "d", "dc", "dcc", "dccc", "cm" }
        };

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char RomanThousand = 'm';

        /// <summary>
        /// List of letters that represents numerals in Arabic Alpha numbering style.
        /// </summary>
        private const string ArabicAlpha = "أبتثجحخدذرزسشصضطظعغفقكلمنهوي";
        /// <summary>
        /// List of letters that represents numerals in Arabic Abjad numbering style.
        /// </summary>
        private const string ArabicAbjad = "أبجدهوزحطيكلمنسعفصقرشتثخذضغظ";

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int AlphabetLength = 26;

        /// <summary>
        /// Length of int.MaxValue ~ "FXSHRXX".
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxExcelColumnNameLength = 7;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int InvalidColumnIndex = -1;
    }
}
