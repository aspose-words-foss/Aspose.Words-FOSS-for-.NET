// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/06/2017 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides utility functions for <see cref="GeneralFormat"/> enum.
    /// </summary>
    internal static class GeneralFormatUtil
    {
        static GeneralFormatUtil()
        {
            AddMapEntry(GeneralFormat.Aiueo, "AIUEO");
            AddMapEntry(GeneralFormat.UppercaseAlphabetic, "ALPHABETIC");
            AddMapEntry(GeneralFormat.LowercaseAlphabetic, "alphabetic");
            AddMapEntry(GeneralFormat.Arabic, "Arabic");
            AddMapEntry(GeneralFormat.ArabicAbjad, "ARABICABJAD");
            AddMapEntry(GeneralFormat.ArabicAlpha, "ARABICALPHA");
            AddMapEntry(GeneralFormat.ArabicDash, "ArabicDash");
            AddMapEntry(GeneralFormat.BahtText, "BAHTTEXT");
            AddMapEntry(GeneralFormat.CardText, "CardText");
            AddMapEntry(GeneralFormat.ChineseNum1, "CHINESENUM1");
            AddMapEntry(GeneralFormat.ChineseNum2, "CHINESENUM2");
            AddMapEntry(GeneralFormat.ChineseNum3, "CHINESENUM3");
            AddMapEntry(GeneralFormat.Chosung, "CHOSUNG");
            AddMapEntry(GeneralFormat.CircleNum, "CIRCLENUM");
            AddMapEntry(GeneralFormat.DBChar, "DBCHAR");
            AddMapEntry(GeneralFormat.DBNum1, "DBNUM1");
            AddMapEntry(GeneralFormat.DBNum2, "DBNUM2");
            AddMapEntry(GeneralFormat.DBNum3, "DBNUM3");
            AddMapEntry(GeneralFormat.DBNum4, "DBNUM4");
            AddMapEntry(GeneralFormat.DollarText, "DollarText");
            AddMapEntry(GeneralFormat.Ganada, "GANADA");
            AddMapEntry(GeneralFormat.GB1, "GB1");
            AddMapEntry(GeneralFormat.GB2, "GB2");
            AddMapEntry(GeneralFormat.GB3, "GB3");
            AddMapEntry(GeneralFormat.GB4, "GB4");
            AddMapEntry(GeneralFormat.Hebrew1, "HEBREW1");
            AddMapEntry(GeneralFormat.Hebrew2, "HEBREW2");
            AddMapEntry(GeneralFormat.Hex, "Hex");
            AddMapEntry(GeneralFormat.HindiArabic, "HINDIARABIC");
            AddMapEntry(GeneralFormat.HindiCardText, "HINDICARDTEXT");
            AddMapEntry(GeneralFormat.HindiLetter1, "HINDILETTER1");
            AddMapEntry(GeneralFormat.HindiLetter2, "HINDILETTER2");
            AddMapEntry(GeneralFormat.Iroha, "IROHA");
            AddMapEntry(GeneralFormat.KanjiNum1, "KANJINUM1");
            AddMapEntry(GeneralFormat.KanjiNum2, "KANJINUM2");
            AddMapEntry(GeneralFormat.KanjiNum3, "KANJINUM3");
            AddMapEntry(GeneralFormat.Ordinal, "Ordinal");
            AddMapEntry(GeneralFormat.OrdText, "OrdText");
            AddMapEntry(GeneralFormat.UppercaseRoman, "Roman");
            AddMapEntry(GeneralFormat.LowercaseRoman, "roman");
            AddMapEntry(GeneralFormat.SBChar, "SBCHAR");
            AddMapEntry(GeneralFormat.ThaiArabic, "THAIARABIC");
            AddMapEntry(GeneralFormat.ThaiCardText, "THAICARDTEXT");
            AddMapEntry(GeneralFormat.ThaiLetter, "THAILETTER");
            AddMapEntry(GeneralFormat.VietCardText, "VIETCARDTEXT");
            AddMapEntry(GeneralFormat.Zodiac1, "ZODIAC1");
            AddMapEntry(GeneralFormat.Zodiac2, "ZODIAC2");
            AddMapEntry(GeneralFormat.Zodiac3, "ZODIAC3");

            AddMapEntry(GeneralFormat.Caps, "Caps");
            AddMapEntry(GeneralFormat.FirstCap, "FirstCap");
            AddMapEntry(GeneralFormat.Lower, "Lower");
            AddMapEntry(GeneralFormat.Upper, "Upper");

            AddMapEntry(GeneralFormat.CharFormat, "CHARFORMAT");
            AddMapEntry(GeneralFormat.MergeFormat, "MERGEFORMAT");
            AddMapEntry(GeneralFormat.MergeFormatInet, "MERGEFORMATINET");
        }

        private static void AddMapEntry(GeneralFormat format, string formatString)
        {
            gGeneralFormatToStringMap.Add(format, formatString);
            if (!gStringToGeneralFormatMap.ContainsKey(formatString.ToUpper()))
                gStringToGeneralFormatMap.Add(formatString.ToUpper(), format);
        }

        internal static string GeneralFormatToString(GeneralFormat format)
        {
            return gGeneralFormatToStringMap.GetValueOrNull(format);
        }

        internal static GeneralFormat StringToGeneralFormat(string formatString)
        {
            // Special case for the alphabetic and Roman formats.
            switch (formatString.ToLower())
            {
                case "alphabetic":
                    return (char.IsLower(formatString[0]))
                        ? GeneralFormat.LowercaseAlphabetic
                        : GeneralFormat.UppercaseAlphabetic;
                case "roman":
                    return (char.IsLower(formatString[0]))
                        ? GeneralFormat.LowercaseRoman
                        : GeneralFormat.UppercaseRoman;
                default:
                    break;
            }

            return gStringToGeneralFormatMap.GetValueOrDefault(formatString.ToUpper(), GeneralFormat.None);
        }

        internal static bool IsNumericFormat(GeneralFormat format)
        {
            return GeneralFormatToNumberStyle(format) != NumberStyle.None;
        }

        internal static NumberStyle GeneralFormatToNumberStyle(GeneralFormat format)
        {
            // ECMA $17.16.4.3.1 matches several number formats to multiple number styles. AW uses the first one in such cases.
            switch (format)
            {
                case GeneralFormat.Aiueo:
                    return NumberStyle.Aiueo;
                case GeneralFormat.UppercaseAlphabetic:
                    return NumberStyle.UppercaseLetter;
                case GeneralFormat.LowercaseAlphabetic:
                    return NumberStyle.LowercaseLetter;
                case GeneralFormat.Arabic:
                    return NumberStyle.Arabic;
                case GeneralFormat.ArabicAbjad:
                    return NumberStyle.Arabic2;
                case GeneralFormat.ArabicAlpha:
                    return NumberStyle.Arabic1;
                case GeneralFormat.ArabicDash:
                    return NumberStyle.NumberInDash;
                case GeneralFormat.BahtText:
                    // NOTE: the NumberStyle enum misses 'bahtText' value.
                    return NumberStyle.Arabic;
                case GeneralFormat.CardText:
                    return NumberStyle.Number;
                case GeneralFormat.ChineseNum1:
                    // ECMA $17.16.4.3.1: chineseCounting(zh-CN) or taiwaneseCounting (zn-TW)
                    return NumberStyle.SimpChinNum1;
                case GeneralFormat.ChineseNum2:
                    // ECMA $17.16.4.3.1: chineseLegalSimplified (zh-CN) or ideographLegalTraditional (zh-TW)
                    return NumberStyle.SimpChinNum2;
                case GeneralFormat.ChineseNum3:
                    // ECMA $17.16.4.3.1: chineseCountingThousand (zh-CN) or taiwaneseCountingThousand (zh-TW)
                    return NumberStyle.SimpChinNum3;
                case GeneralFormat.Chosung:
                    return NumberStyle.Chosung;
                case GeneralFormat.CircleNum:
                    return NumberStyle.NumberInCircle;
                case GeneralFormat.DBChar:
                    return NumberStyle.ArabicFullWidth;
                case GeneralFormat.DBNum1:
                    // ECMA $17.16.4.3.1: ideographDigital (ja-JP) or koreanDigital (ko-KR)
                    return NumberStyle.Kanji;
                case GeneralFormat.DBNum2:
                    // ECMA $17.16.4.3.1: japaneseCounting (ja-JP) or koreanCounting (ko-KR)
                    return NumberStyle.KanjiDigit;
                case GeneralFormat.DBNum3:
                    // ECMA $17.16.4.3.1: japaneseLegal(ja-JP) or koreanLegal (ko-KR)
                    return NumberStyle.KanjiTraditional;
                case GeneralFormat.DBNum4:
                    // ECMA $17.16.4.3.1: japaneseDigitalTenThousand (ja-JP) or koreanDigital2 (ko-KR) or taiwaneseDigital (zh-TW)
                    return NumberStyle.KanjiTraditional2;
                case GeneralFormat.DollarText:
                    // NOTE: the NumberStyle enum misses 'dollarText' value.
                    return NumberStyle.Arabic;
                case GeneralFormat.Ganada:
                    return NumberStyle.Ganada;
                case GeneralFormat.GB1:
                    return NumberStyle.GB1;
                case GeneralFormat.GB2:
                    return NumberStyle.GB2;
                case GeneralFormat.GB3:
                    return NumberStyle.GB3;
                case GeneralFormat.GB4:
                    return NumberStyle.GB4;
                case GeneralFormat.Hebrew1:
                    return NumberStyle.Hebrew1;
                case GeneralFormat.Hebrew2:
                    return NumberStyle.Hebrew2;
                case GeneralFormat.Hex:
                    return NumberStyle.Hex;
                case GeneralFormat.HindiArabic:
                    return NumberStyle.HindiArabic;
                case GeneralFormat.HindiCardText:
                    return NumberStyle.HindiCardinalText;
                case GeneralFormat.HindiLetter1:
                    // NOTE: the NumberStyle enum misses 'hindiVowels' value.
                    return NumberStyle.Arabic;
                case GeneralFormat.HindiLetter2:
                    return NumberStyle.HindiLetter2;
                case GeneralFormat.Iroha:
                    return NumberStyle.Iroha;
                case GeneralFormat.KanjiNum1:
                    // ECMA $17.16.4.3.1: koreanDigital(ko-KR), ideographDigital (ja-JP), chineseCounting (zh-CN), or taiwaneseCounting (zh-TW)
                    return NumberStyle.HanjaRead;
                case GeneralFormat.KanjiNum2:
                    // ECMA $17.16.4.3.1: koreanCounting(ko-KR), chineseCountingThousand (ja-JP), chineseLegalSimplified (zh-CN), or ideographLegalTraditional (zh-TW)
                    return NumberStyle.HanjaReadDigit;
                case GeneralFormat.KanjiNum3:
                    // ECMA $17.16.4.3.1: koreanLegal (koKR) or japaneseLegal (ja-JP) or chineseCountingThousand (zh-CN) or taiwaneseCountingThousand (zh-TW)
                    return NumberStyle.Hangul;
                case GeneralFormat.Ordinal:
                    return NumberStyle.Ordinal;
                case GeneralFormat.OrdText:
                    return NumberStyle.OrdinalText;
                case GeneralFormat.UppercaseRoman:
                    return NumberStyle.UppercaseRoman;
                case GeneralFormat.LowercaseRoman:
                    return NumberStyle.LowercaseRoman;
                case GeneralFormat.SBChar:
                    return NumberStyle.ArabicHalfWidth;
                case GeneralFormat.ThaiArabic:
                    return NumberStyle.ThaiArabic;
                case GeneralFormat.ThaiCardText:
                    return NumberStyle.ThaiCardinalText;
                case GeneralFormat.ThaiLetter:
                    return NumberStyle.ThaiLetter;
                case GeneralFormat.VietCardText:
                    return NumberStyle.VietCardinalText;
                case GeneralFormat.Zodiac1:
                    return NumberStyle.Zodiac1;
                case GeneralFormat.Zodiac2:
                    return NumberStyle.Zodiac2;
                case GeneralFormat.Zodiac3:
                    return NumberStyle.Zodiac3;
                default:
                    return NumberStyle.None;
            }
        }

        internal static CharCase GeneralFormatToCharCase(GeneralFormat format)
        {
            switch (format)
            {
                case GeneralFormat.Caps:
                    return CharCase.Caps;
                case GeneralFormat.FirstCap:
                    return CharCase.FirstCap;
                case GeneralFormat.Lower:
                    return CharCase.Lower;
                case GeneralFormat.Upper:
                    return CharCase.Upper;
                case GeneralFormat.DBChar:
                    return CharCase.DbChar;
                default:
                    return CharCase.Default;
            }
        }

        private static readonly Dictionary<GeneralFormat, string> gGeneralFormatToStringMap = new Dictionary<GeneralFormat, string>();
        private static readonly Dictionary<string, GeneralFormat> gStringToGeneralFormatMap = new Dictionary<string, GeneralFormat>();
    }
}
