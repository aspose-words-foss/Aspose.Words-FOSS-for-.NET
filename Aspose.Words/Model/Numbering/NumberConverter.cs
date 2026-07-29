// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2012 by Andrey Soldatov

using System;
using System.Threading;
using Aspose.Numbering;

namespace Aspose.Words
{
    /// <summary>
    /// Converts a number into a specific style.
    /// </summary>
    /// <dev>
    /// This class is a facade for <see cref="NumberConverterCore"/> accepting <see cref="NumberStyle"/> instead of
    /// <see cref="NumberStyleCore"/>.
    /// </dev>
    internal static class NumberConverter
    {
        /// <summary>
        /// Converts an int value into a string using the specified number style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="numberStyle">The number style to use.</param>
        /// <param name="isFirstLetterUppercase">Whether to make the first letter uppercase for textual formats:
        /// <see cref="NumberStyle.Number"/> and <see cref="NumberStyle.OrdinalText"/>.</param>
        internal static string NumberToString(int value, NumberStyle numberStyle, bool isFirstLetterUppercase)
        {
            return NumberToString(value, numberStyle, "", isFirstLetterUppercase);
        }

        /// <summary>
        /// Converts an int value into a string using the specified number style.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="numberStyle">The number style to use.</param>
        /// <param name="customNumberStyle">Custom number style string. Meaningful if <paramref name="numberStyle"/> is <see cref="NumberStyle.Custom"/></param>
        /// <param name="isFirstLetterUppercase">Whether to make the first letter uppercase for textual formats:
        /// <see cref="NumberStyle.Number"/> and <see cref="NumberStyle.OrdinalText"/>.</param>
        internal static string NumberToString(int value, NumberStyle numberStyle, string customNumberStyle, bool isFirstLetterUppercase)
        {
            NumberStyleCore numberStyleCore = ToNumberStyleCore(numberStyle, customNumberStyle);

            // By default, language will be English.
            Language language = GetLanguage(customNumberStyle);

            return NumberConverterCore.NumberToString(value, numberStyleCore, isFirstLetterUppercase, language);
        }

        /// <summary>
        /// Converts an int value into a string using the specified number style and the locale identifier (localeId).
        /// </summary>
        internal static string NumberToLocalizedString(long value, NumberStyle numberStyle, string customNumberStyle, bool isFirstLetterUppercase, int localeId)
        {
            NumberStyleCore numberStyleCore = ToNumberStyleCore(numberStyle, customNumberStyle);

            // For a moment language is used for all 'letter' number styles,
            // such as NumberStyleCore.UppercaseLetter, NumberStyleCore.LowercaseLetter and so on.
            // So, when custom style is defined with some of the letter styles,
            // then language should be obtained from the custom style string itself.
            if ((numberStyleCore == NumberStyleCore.LowercaseLetter) || (numberStyleCore == NumberStyleCore.UppercaseLetter))
            {
                // WORDSNET-23954 Determine language by custom style only when the custom style is defined.
                // Also, Word replaces all locales except of SwedishSweden (and this is quite strange) with EnglishUs locale
                // when retrieves localized string for list label. Probably, there are some other locales for which it does
                // the same, but I found only Swedish for a moment.
                if (StringUtil.HasChars(customNumberStyle))
                {
                    localeId = (int)GetLanguage(customNumberStyle);
                }
                else if ((Language)localeId != Language.SwedishSweden)
                {
                    localeId = (int)Language.EnglishUS;
                }
            }

            return NumberConverterCore.NumberToLocalizedString(value, numberStyleCore, isFirstLetterUppercase, localeId);
        }

        /// <summary>
        /// Converts an int value into a string using the specified number style and the current locale.
        /// </summary>
        internal static string NumberToLocalizedString(long value, NumberStyle numberStyle, bool isFirstLetterUppercase)
        {
            int localeId = Thread.CurrentThread.CurrentCulture.LCID;
            return NumberToLocalizedString(value, numberStyle, isFirstLetterUppercase, localeId);
        }

        /// <summary>
        /// Converts an int value into a string using the specified number style and the locale identifier (localeId).
        /// </summary>
        internal static string NumberToLocalizedString(long value, NumberStyle numberStyle, bool isFirstLetterUppercase, int localeId)
        {
            return NumberToLocalizedString(value, numberStyle, "", isFirstLetterUppercase, localeId);
        }

        /// <summary>
        /// Parses the specified sequence of characters to its numeric representation. 'A' means 0, 'B' means 1,
        /// ... , 'Z' means 25, 'AA' means 26, 'AB' means 27 and so forth. The characters are case insensitive.
        /// If was passed invalid column name returns negative value as error indication
        /// </summary>
        internal static int ParseExcelColumnName(string text)
        {
            return NumberConverterCore.ParseExcelColumnName(text);
        }

        /// <summary>
        /// Converts the specified value to its alphabetic representation. 0 means 'A', 1 means 'B', ...,
        /// 25 means 'Z', 26 means 'AA', 27 means 'AB' and so forth.
        /// If was passed invalid column index returns a null.
        /// </summary>
        internal static string ToExcelColumnName(int value)
        {
            return NumberConverterCore.ToExcelColumnName(value);
        }

        /// <summary>
        /// Converts <see cref="NumberStyle"/> to <see cref="NumberStyleCore"/>.
        /// </summary>
        /// <param name="numberStyle">Number style to convert.</param>
        /// <param name="customNumberStyle">Custom number style string. Meaningful if <paramref name="numberStyle"/> is <see cref="NumberStyle.Custom"/></param>
        private static NumberStyleCore ToNumberStyleCore(NumberStyle numberStyle, string customNumberStyle)
        {
            switch (numberStyle)
            {
                case NumberStyle.None:
                    return NumberStyleCore.None;
                case NumberStyle.Arabic:
                    return NumberStyleCore.Arabic;
                case NumberStyle.UppercaseRoman:
                    return NumberStyleCore.UppercaseRoman;
                case NumberStyle.LowercaseRoman:
                    return NumberStyleCore.LowercaseRoman;
                case NumberStyle.UppercaseLetter:
                    return NumberStyleCore.UppercaseLetter;
                case NumberStyle.LowercaseLetter:
                    return NumberStyleCore.LowercaseLetter;
                case NumberStyle.Ordinal:
                    return NumberStyleCore.Ordinal;
                case NumberStyle.Number:
                    return NumberStyleCore.Number;
                case NumberStyle.OrdinalText:
                    return NumberStyleCore.OrdinalText;
                case NumberStyle.Hex:
                    return NumberStyleCore.Hex;
                case NumberStyle.ChicagoManual:
                    return NumberStyleCore.ChicagoManual;
                case NumberStyle.Kanji:
                    return NumberStyleCore.Kanji;
                case NumberStyle.KanjiDigit:
                    return NumberStyleCore.KanjiDigit;
                case NumberStyle.AiueoHalfWidth:
                    return NumberStyleCore.AiueoHalfWidth;
                case NumberStyle.IrohaHalfWidth:
                    return NumberStyleCore.IrohaHalfWidth;
                case NumberStyle.ArabicFullWidth:
                    return NumberStyleCore.ArabicFullWidth;
                case NumberStyle.ArabicHalfWidth:
                    return NumberStyleCore.ArabicHalfWidth;
                case NumberStyle.KanjiTraditional:
                    return NumberStyleCore.KanjiTraditional;
                case NumberStyle.KanjiTraditional2:
                    return NumberStyleCore.KanjiTraditional2;
                case NumberStyle.NumberInCircle:
                    return NumberStyleCore.NumberInCircle;
                case NumberStyle.DecimalFullWidth:
                    return NumberStyleCore.DecimalFullWidth;
                case NumberStyle.Aiueo:
                    return NumberStyleCore.Aiueo;
                case NumberStyle.Iroha:
                    return NumberStyleCore.Iroha;
                case NumberStyle.LeadingZero:
                    return NumberStyleCore.LeadingZero;
                case NumberStyle.Bullet:
                    return NumberStyleCore.Bullet;
                case NumberStyle.Ganada:
                    return NumberStyleCore.Ganada;
                case NumberStyle.Chosung:
                    return NumberStyleCore.Chosung;
                case NumberStyle.GB1:
                    return NumberStyleCore.GB1;
                case NumberStyle.GB2:
                    return NumberStyleCore.GB2;
                case NumberStyle.GB3:
                    return NumberStyleCore.GB3;
                case NumberStyle.GB4:
                    return NumberStyleCore.GB4;
                case NumberStyle.Zodiac1:
                    return NumberStyleCore.Zodiac1;
                case NumberStyle.Zodiac2:
                    return NumberStyleCore.Zodiac2;
                case NumberStyle.Zodiac3:
                    return NumberStyleCore.Zodiac3;
                case NumberStyle.TradChinNum1:
                    return NumberStyleCore.TradChinNum1;
                case NumberStyle.TradChinNum2:
                    return NumberStyleCore.TradChinNum2;
                case NumberStyle.TradChinNum3:
                    return NumberStyleCore.TradChinNum3;
                case NumberStyle.TradChinNum4:
                    return NumberStyleCore.TradChinNum4;
                case NumberStyle.SimpChinNum1:
                    return NumberStyleCore.SimpChinNum1;
                case NumberStyle.SimpChinNum2:
                    return NumberStyleCore.SimpChinNum2;
                case NumberStyle.SimpChinNum3:
                    return NumberStyleCore.SimpChinNum3;
                case NumberStyle.SimpChinNum4:
                    return NumberStyleCore.SimpChinNum4;
                case NumberStyle.HanjaRead:
                    return NumberStyleCore.HanjaRead;
                case NumberStyle.HanjaReadDigit:
                    return NumberStyleCore.HanjaReadDigit;
                case NumberStyle.Hangul:
                    return NumberStyleCore.Hangul;
                case NumberStyle.Hanja:
                    return NumberStyleCore.Hanja;
                case NumberStyle.Hebrew1:
                    return NumberStyleCore.Hebrew1;
                case NumberStyle.Arabic1:
                    return NumberStyleCore.Arabic1;
                case NumberStyle.Hebrew2:
                    return NumberStyleCore.Hebrew2;
                case NumberStyle.Arabic2:
                    return NumberStyleCore.Arabic2;
                case NumberStyle.HindiLetter1:
                    return NumberStyleCore.HindiLetter1;
                case NumberStyle.HindiLetter2:
                    return NumberStyleCore.HindiLetter2;
                case NumberStyle.HindiArabic:
                    return NumberStyleCore.HindiArabic;
                case NumberStyle.HindiCardinalText:
                    return NumberStyleCore.HindiCardinalText;
                case NumberStyle.ThaiLetter:
                    return NumberStyleCore.ThaiLetter;
                case NumberStyle.ThaiArabic:
                    return NumberStyleCore.ThaiArabic;
                case NumberStyle.ThaiCardinalText:
                    return NumberStyleCore.ThaiCardinalText;
                case NumberStyle.VietCardinalText:
                    return NumberStyleCore.VietCardinalText;
                case NumberStyle.NumberInDash:
                    return NumberStyleCore.NumberInDash;
                case NumberStyle.LowercaseRussian:
                    return NumberStyleCore.LowercaseRussian;
                case NumberStyle.UppercaseRussian:
                    return NumberStyleCore.UppercaseRussian;
                case NumberStyle.Custom:
                    return ToNumberStyleCore(customNumberStyle);
                default:
                    throw new ArgumentException(string.Format("Unexpected number style: '{0}'", numberStyle));
            }
        }

        /// <summary>
        /// Returns corresponding <see cref="NumberStyleCore"/> by the specified <paramref name="customNumberStyle"/> string.
        /// </summary>
        /// <remarks>
        /// In MS Word we can define various custom number formats, such as:
        /// "1, 2, 3, ...",
        /// "I, II, III, ...",
        /// "i, ii, iii, ...",
        /// "A, B, C, ..."
        /// "a, b, c, ...",
        /// "1st, 2nd, 3rd, ..." and so on.
        /// But seems MS Word writes them all as general number formats: 'decimal', 'upperRoman', 'lowerRoman' and so on.
        /// Even "01, 02, 03, ..." custom style it writes just as 'decimalZero'.
        /// Only "001, 002, 003, ...", "0001, 0002, 0003, ...",  "00001, 00002, 00003, ..."
        /// or locale-specific (for example Turkish lower letters = "a, ç, ĝ, ...")
        /// number formats MS Word writes as 'custom'.
        /// </remarks>
        private static NumberStyleCore ToNumberStyleCore(string customNumberStyle)
        {
            if (!StringUtil.HasChars(customNumberStyle))
                return NumberStyleCore.None;

            char firstLetter = customNumberStyle[0];
            if (char.IsLower(firstLetter))
                return NumberStyleCore.LowercaseLetter;
            if (char.IsUpper(firstLetter))
                return NumberStyleCore.UppercaseLetter;

            switch (customNumberStyle)
            {
                case "001, 002, 003, ...":
                    return NumberStyleCore.LeadingZero2;
                case "0001, 0002, 0003, ...":
                    return NumberStyleCore.LeadingZero3;
                case "00001, 00002, 00003, ...":
                    return NumberStyleCore.LeadingZero4;
                default:
                {
                    throw new ArgumentException(
                        string.Format("Unexpected custom number format style: '{0}'", customNumberStyle));
                }
            }
        }

        /// <summary>
        /// Returns corresponding <see cref="Language"/> by the <paramref name="customNumberStyle"/> string.
        /// </summary>
        private static Language GetLanguage(string customNumberStyle)
        {
            switch (customNumberStyle)
            {
                case "A, Ç, Ĝ, ...":
                case "a, ç, ĝ, ...":
                    return Language.TurkishTurkey;
                case "Α, Β, Γ, ...":
                case "α, β, γ, ...":
                    return Language.GreekGreece;
                default:
                    return Language.EnglishUS;
            }
        }
    }
}
