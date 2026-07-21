// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2006 by Roman Korchagin

namespace Aspose.Numbering
{
    /// <summary>
    /// Specifies a number style for a number-to-text conversion.
    /// </summary>
    /// <dev>
    /// This is a copycat of Aspose.Words.NumberStyle. Should keep members of these enums in sync.
    /// </dev>
    public enum NumberStyleCore
    {
        /// <summary>
        /// No bullet or number.
        /// </summary>
        None,
        /// <summary>
        /// Arabic numbering (1, 2, 3, ...)
        /// </summary>
        Arabic,
        /// <summary>
        /// Upper case Roman (I, II, III, ...)
        /// </summary>
        UppercaseRoman,
        /// <summary>
        /// Lower case Roman (i, ii, iii, ...)
        /// </summary>
        LowercaseRoman,
        /// <summary>
        /// Upper case Letter (A, B, C, ...)
        /// </summary>
        UppercaseLetter,
        /// <summary>
        /// Lower case letter (a, b, c, ...)
        /// </summary>
        LowercaseLetter,
        /// <summary>
        /// Ordinal (1st, 2nd, 3rd, ...)
        /// </summary>
        Ordinal,
        /// <summary>
        /// Numbered (One, Two, Three, ...)
        /// </summary>
        Number,
        /// <summary>
        /// Ordinal (text) (First, Second, Third, ...)
        /// </summary>
        OrdinalText,
        /// <summary>
        /// Hexadecimal: 8, 9, A, B, C, D, E, F, 10, 11, 12
        /// </summary>
        Hex,
        /// <summary>
        /// Chicago Manual of Style: *, †, †
        /// </summary>
        ChicagoManual,
        /// <summary>
        /// Ideograph-digital
        /// </summary>
        Kanji,
        /// <summary>
        /// Japanese counting
        /// </summary>
        KanjiDigit,
        /// <summary>
        /// Aiueo
        /// </summary>
        AiueoHalfWidth,
        /// <summary>
        /// Iroha
        /// </summary>
        IrohaHalfWidth,
        /// <summary>
        /// Full-width Arabic: 1, 2, 3, 4
        /// </summary>
        ArabicFullWidth,
        /// <summary>
        /// Half-width Arabic: 1, 2, 3, 4
        /// </summary>
        ArabicHalfWidth,
        /// <summary>
        /// Japanese legal
        /// </summary>
        KanjiTraditional,
        /// <summary>
        /// Japanese digital ten thousand
        /// </summary>
        KanjiTraditional2,
        /// <summary>
        /// Enclosed circles
        /// </summary>
        NumberInCircle,
        /// <summary>
        /// Decimal full width: 1, 2, 3, 4
        /// </summary>
        DecimalFullWidth,
        /// <summary>
        /// Aiueo full width
        /// </summary>
        Aiueo,
        /// <summary>
        /// Iroha full width
        /// </summary>
        Iroha,
        /// <summary>
        /// Leading Zero (01, 02,..., 09, 10, 11,..., 99, 100, 101,...)
        /// </summary>
        LeadingZero,
        /// <summary>
        /// Leading Zero (001, 002,..., 009, 010, 011,..., 099, 100, 101,...)
        /// </summary>
        LeadingZero2,
        /// <summary>
        /// Leading Zero (0001, 0002,..., 0009, 0010, 0011,..., 0099, 0100, 0101,..., 0999, 1000, 1001,...)
        /// </summary>
        LeadingZero3,
        /// <summary>
        /// Leading Zero (00001, 00002,..., 00009, 00010, 00011,..., 00099, 00100, 00101,..., 00999, 01000, 01001,..., 09999, 10000, 10001,...)
        /// </summary>
        LeadingZero4,
        /// <summary>
        /// Bullet (check the character code in the text)
        /// </summary>
        Bullet,
        /// <summary>
        /// Korean Ganada
        /// </summary>
        Ganada,
        /// <summary>
        /// Korea Chosung
        /// </summary>
        Chosung,
        /// <summary>
        /// Enclosed full stop
        /// </summary>
        GB1,
        /// <summary>
        /// Enclosed parenthesis
        /// </summary>
        GB2,
        /// <summary>
        /// Enclosed circle Chinese
        /// </summary>
        GB3,
        /// <summary>
        /// Ideograph enclosed circle
        /// </summary>
        GB4,
        /// <summary>
        /// Ideograph traditional
        /// </summary>
        Zodiac1,
        /// <summary>
        /// Ideograph Zodiac
        /// </summary>
        Zodiac2,
        /// <summary>
        /// Ideograph Zodiac traditional
        /// </summary>
        Zodiac3,
        /// <summary>
        /// Taiwanese counting
        /// </summary>
        TradChinNum1,
        /// <summary>
        /// Ideograph legal traditional
        /// </summary>
        TradChinNum2,
        /// <summary>
        /// Taiwanese counting thousand
        /// </summary>
        TradChinNum3,
        /// <summary>
        /// Taiwanese digital
        /// </summary>
        TradChinNum4,
        /// <summary>
        /// Chinese counting
        /// </summary>
        SimpChinNum1,
        /// <summary>
        /// Chinese legal simplified
        /// </summary>
        SimpChinNum2,
        /// <summary>
        /// Chinese counting thousand
        /// </summary>
        SimpChinNum3,
        /// <summary>
        /// Chinese (not implemented)
        /// </summary>
        SimpChinNum4,
        /// <summary>
        /// Korean digital
        /// </summary>
        HanjaRead,
        /// <summary>
        /// Korean counting
        /// </summary>
        HanjaReadDigit,
        /// <summary>
        /// Korea legal
        /// </summary>
        Hangul,
        /// <summary>
        /// Korea digital2
        /// </summary>
        Hanja,
        /// <summary>
        /// Hebrew-1
        /// </summary>
        Hebrew1,
        /// <summary>
        /// Arabic alpha
        /// </summary>
        Arabic1,
        /// <summary>
        /// Hebrew-2
        /// </summary>
        Hebrew2,
        /// <summary>
        /// Arabic abjad
        /// </summary>
        Arabic2,
        /// <summary>
        /// Hindi vowels
        /// </summary>
        HindiLetter1,
        /// <summary>
        /// Hindi consonants
        /// </summary>
        HindiLetter2,
        /// <summary>
        /// Hindi numbers
        /// </summary>
        HindiArabic,
        /// <summary>
        /// Hindi descriptive (cardinals)
        /// </summary>
        HindiCardinalText,
        /// <summary>
        /// Thai letters
        /// </summary>
        ThaiLetter,
        /// <summary>
        /// Thai numbers
        /// </summary>
        ThaiArabic,
        /// <summary>
        /// Thai descriptive (cardinals)
        /// </summary>
        ThaiCardinalText,
        /// <summary>
        /// Vietnamese descriptive (cardinals)
        /// </summary>
        VietCardinalText,
        /// <summary>
        /// Page number format: - 1 -, - 2 -, - 3 -, - 4 -
        /// </summary>
        NumberInDash,
        /// <summary>
        /// Lowercase Russian alphabet
        /// </summary>
        LowercaseRussian,
        /// <summary>
        /// Uppercase Russian alphabet
        /// </summary>
        UppercaseRussian
    }
}
