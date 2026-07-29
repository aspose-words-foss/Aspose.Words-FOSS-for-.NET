// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2006 by Roman Korchagin

namespace Aspose.Words
{
    // Most of the members are taken from Microsoft Office 2003 Edition XML Schema References: Schema References | Word | Concepts | Using WordprocessingML (look in the end of article).
    // Names are made similar to VBA ( referenced in http://msdn.microsoft.com/library/default.asp?url=/library/en-us/vbawd11/html/wohowConstants_HV01049731.asp ).
    /// <summary>
    /// Specifies the number style for a list, footnotes and endnotes, page numbers.
    /// </summary>
    /// <dev>
    /// Should keep members of this enum in sync with <see cref="Aspose.Numbering.NumberStyleCore"/>.
    /// </dev>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum NumberStyle
    {
        /// <summary>
        /// Arabic numbering (1, 2, 3, ...)
        /// </summary>
        Arabic = 0,

        /// <summary>
        /// Upper case Roman (I, II, III, ...)
        /// </summary>
        UppercaseRoman = 1,

        /// <summary>
        /// Lower case Roman (i, ii, iii, ...)
        /// </summary>
        LowercaseRoman = 2,

        /// <summary>
        /// Upper case Letter (A, B, C, ...)
        /// </summary>
        UppercaseLetter = 3,

        /// <summary>
        /// Lower case letter (a, b, c, ...)
        /// </summary>
        LowercaseLetter = 4,

        /// <summary>
        /// Ordinal (1st, 2nd, 3rd, ...)
        /// </summary>
        Ordinal = 5,

        /// <summary>
        /// Numbered (One, Two, Three, ...)
        /// </summary>
        Number = 6,

        /// <summary>
        /// Ordinal (text) (First, Second, Third, ...)
        /// </summary>
        OrdinalText = 7,

        /// <summary>
        /// Hexadecimal: 8, 9, A, B, C, D, E, F, 10, 11, 12
        /// </summary>
        Hex = 8,

        /// <summary>
        /// Chicago Manual of Style: *, †, †
        /// </summary>
        ChicagoManual = 9,

        /// <summary>
        /// Ideograph-digital
        /// </summary>
        Kanji = 10,

        /// <summary>
        /// Japanese counting
        /// </summary>
        KanjiDigit = 11,

        /// <summary>
        /// Aiueo
        /// </summary>
        AiueoHalfWidth = 12,

        /// <summary>
        /// Iroha
        /// </summary>
        IrohaHalfWidth = 13,

        /// <summary>
        /// Full-width Arabic: 1, 2, 3, 4
        /// </summary>
        ArabicFullWidth = 14,

        /// <summary>
        /// Half-width Arabic: 1, 2, 3, 4
        /// </summary>
        ArabicHalfWidth = 15,

        /// <summary>
        /// Japanese legal
        /// </summary>
        KanjiTraditional = 16,

        /// <summary>
        /// Japanese digital ten thousand
        /// </summary>
        KanjiTraditional2 = 17,

        /// <summary>
        /// Enclosed circles
        /// </summary>
        NumberInCircle = 18,

        /// <summary>
        /// Decimal full width: 1, 2, 3, 4
        /// </summary>
        DecimalFullWidth = 19,

        /// <summary>
        /// Aiueo full width
        /// </summary>
        Aiueo = 20,

        /// <summary>
        /// Iroha full width
        /// </summary>
        Iroha = 21,

        /// <summary>
        /// Leading Zero (01, 02,..., 09, 10, 11,..., 99, 100, 101,...)
        /// </summary>
        LeadingZero = 22,

        /// <summary>
        /// Bullet (check the character code in the text)
        /// </summary>
        Bullet = 23,

        /// <summary>
        /// Korean Ganada
        /// </summary>
        Ganada = 24,

        /// <summary>
        /// Korea Chosung
        /// </summary>
        Chosung = 25,

        /// <summary>
        /// Enclosed full stop
        /// </summary>
        GB1 = 26,

        /// <summary>
        /// Enclosed parenthesis
        /// </summary>
        GB2 = 27,

        /// <summary>
        /// Enclosed circle Chinese
        /// </summary>
        GB3 = 28,

        /// <summary>
        /// Ideograph enclosed circle
        /// </summary>
        GB4 = 29,

        /// <summary>
        /// Ideograph traditional
        /// </summary>
        Zodiac1 = 30,

        /// <summary>
        /// Ideograph Zodiac
        /// </summary>
        Zodiac2 = 31,

        /// <summary>
        /// Ideograph Zodiac traditional
        /// </summary>
        Zodiac3 = 32,

        /// <summary>
        /// Taiwanese counting
        /// </summary>
        TradChinNum1 = 33,

        /// <summary>
        /// Ideograph legal traditional
        /// </summary>
        TradChinNum2 = 34,

        /// <summary>
        /// Taiwanese counting thousand
        /// </summary>
        TradChinNum3 = 35,

        /// <summary>
        /// Taiwanese digital
        /// </summary>
        TradChinNum4 = 36,

        /// <summary>
        /// Chinese counting
        /// </summary>
        SimpChinNum1 = 37,

        /// <summary>
        /// Chinese legal simplified
        /// </summary>
        SimpChinNum2 = 38,

        /// <summary>
        /// Chinese counting thousand
        /// </summary>
        SimpChinNum3 = 39,

        /// <summary>
        /// Chinese (not implemented)
        /// </summary>
        SimpChinNum4 = 40,

        /// <summary>
        /// Korean digital
        /// </summary>
        HanjaRead = 41,

        /// <summary>
        /// Korean counting
        /// </summary>
        HanjaReadDigit = 42,

        /// <summary>
        /// Korea legal
        /// </summary>
        Hangul = 43,

        /// <summary>
        /// Korea digital2
        /// </summary>
        Hanja = 44,

        /// <summary>
        /// Hebrew-1
        /// </summary>
        Hebrew1 = 45,

        /// <summary>
        /// Arabic alpha
        /// </summary>
        Arabic1 = 46,

        /// <summary>
        /// Hebrew-2
        /// </summary>
        Hebrew2 = 47,

        /// <summary>
        /// Arabic abjad
        /// </summary>
        Arabic2 = 48,

        /// <summary>
        /// Hindi vowels
        /// </summary>
        HindiLetter1 = 49,

        /// <summary>
        /// Hindi consonants
        /// </summary>
        HindiLetter2 = 50,

        /// <summary>
        /// Hindi numbers
        /// </summary>
        HindiArabic = 51,

        /// <summary>
        /// Hindi descriptive (cardinals)
        /// </summary>
        HindiCardinalText = 52,

        /// <summary>
        /// Thai letters
        /// </summary>
        ThaiLetter = 53,

        /// <summary>
        /// Thai numbers
        /// </summary>
        ThaiArabic = 54,

        /// <summary>
        /// Thai descriptive (cardinals)
        /// </summary>
        ThaiCardinalText = 55,

        /// <summary>
        /// Vietnamese descriptive (cardinals)
        /// </summary>
        VietCardinalText = 56,

        /// <summary>
        /// Page number format: - 1 -, - 2 -, - 3 -, - 4 -
        /// </summary>
        NumberInDash = 57,

        /// <summary>
        /// Lowercase Russian alphabet
        /// </summary>
        LowercaseRussian = 58,

        /// <summary>
        /// Uppercase Russian alphabet
        /// </summary>
        UppercaseRussian = 59,

        /// <summary>
        /// No bullet or number.
        /// </summary>
        None = 255,

        /// <summary>
        /// Custom number format. It is supported by DOCX format only.
        /// </summary>
        Custom = 0xFF00

        // The following NumberStyle members are additionally described in http://msdn.microsoft.com/library/default.asp?url=/library/en-us/vbawd11/html/wohowConstants_HV01049731.asp
        //   PictureBullet    249
        //   Legal            253
        //   LegalLZ        254
        // They have no analogs in WordML.
    }
}
