// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/12/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies a general format that is applied to a numeric, text, or any field result.
    /// A field may have a combination of general formats.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum GeneralFormat
    {
        /// <summary>
        /// Used to specify a missing general format.
        /// </summary>
        None,

        /// <summary>
        /// Numeric formatting. Formats a numeric result using hiragana characters in the traditional a-i-u-e-o order.
        /// </summary>
        Aiueo = 1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result as one or more occurrences of an uppercase alphabetic Latin character.
        /// </summary>
        UppercaseAlphabetic,
        /// <summary>
        /// Numeric formatting. Formats a numeric result as one or more occurrences of an lowercase alphabetic Latin character.
        /// </summary>
        LowercaseAlphabetic,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Arabic cardinal numerals.
        /// </summary>
        Arabic,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using ascending Abjad numerals.
        /// </summary>
        ArabicAbjad,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using characters in the Arabic alphabet.
        /// </summary>
        ArabicAlpha,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Arabic cardinal numerals, with a prefix of "- " and a suffix of " -".
        /// </summary>
        ArabicDash,
        /// <summary>
        /// Numeric formatting. Formats a numeric result in the Thai counting system.
        /// </summary>
        BahtText,
        /// <summary>
        /// Numeric formatting. Cardinal text (One, Two, Three, ...).
        /// </summary>
        CardText,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using ascending numbers from the appropriate counting system.
        /// </summary>
        ChineseNum1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the appropriate legal format.
        /// </summary>
        ChineseNum2,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the appropriate counting thousand system.
        /// </summary>
        ChineseNum3,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the Korean Chosung format.
        /// </summary>
        Chosung,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using decimal numbering enclosed in a circle, using the
        /// enclosed alphanumeric glyph character for numbers in the range 1–20.
        /// </summary>
        CircleNum,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using double-byte Arabic numbering.
        /// </summary>
        DBChar,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential digital ideographs, using the appropriate character.
        /// </summary>
        DBNum1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the appropriate counting system.
        /// </summary>
        DBNum2,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the appropriate legal counting system.
        /// </summary>
        DBNum3,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the appropriate digital counting system.
        /// </summary>
        DBNum4,
        /// <summary>
        /// Numeric formatting. Dollar text (One, Two, Three, ... + AND 55/100).
        /// </summary>
        DollarText,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the Korean Ganada format.
        /// </summary>
        Ganada,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using decimal numbering followed by a period, using
        /// the enclosed alphanumeric glyph character.
        /// </summary>
        GB1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using decimal numbering enclosed in parenthesis,
        /// using the enclosed alphanumeric glyph character.
        /// </summary>
        GB2,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using decimal numbering enclosed in a circle, using the
        /// enclosed alphanumeric glyph character.
        /// </summary>
        GB3,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using decimal numbering enclosed in a circle, using the
        /// enclosed alphanumeric glyph character.
        /// </summary>
        GB4,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Hebrew numerals.
        /// </summary>
        Hebrew1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using the Hebrew alphabet.
        /// </summary>
        Hebrew2,
        /// <summary>
        /// Numeric formatting. Formats the numeric result using uppercase hexadecimal digits.
        /// </summary>
        Hex,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Hindi numbers.
        /// </summary>
        HindiArabic,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the Hindi counting system.
        /// </summary>
        HindiCardText,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Hindi vowels.
        /// </summary>
        HindiLetter1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Hindi consonants.
        /// </summary>
        HindiLetter2,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using the Japanese iroha.
        /// </summary>
        Iroha,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using a Japanese style using the appropriate counting system.
        /// </summary>
        KanjiNum1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using the appropriate counting system.
        /// </summary>
        KanjiNum2,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using the appropriate counting system.
        /// </summary>
        KanjiNum3,
        /// <summary>
        /// Numeric formatting. Ordinal (1st, 2nd, 3rd, ...).
        /// </summary>
        Ordinal,
        /// <summary>
        /// Numeric formatting. Ordinal text (First, Second, Third, ...).
        /// </summary>
        OrdText,
        /// <summary>
        /// Numeric formatting. Uppercase Roman (I, II, III, ...).
        /// </summary>
        UppercaseRoman,
        /// <summary>
        /// Numeric formatting. Lowercase Roman (i, ii, iii, ...).
        /// </summary>
        LowercaseRoman,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using single-byte Arabic numbering.
        /// </summary>
        SBChar,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Thai numbers.
        /// </summary>
        ThaiArabic,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numbers from the Thai counting system.
        /// </summary>
        ThaiCardText,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Thai letters.
        /// </summary>
        ThaiLetter,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using Vietnamese numerals.
        /// </summary>
        VietCardText,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential numerical traditional ideographs.
        /// </summary>
        Zodiac1,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential zodiac ideographs.
        /// </summary>
        Zodiac2,
        /// <summary>
        /// Numeric formatting. Formats a numeric result using sequential traditional zodiac ideographs.
        /// </summary>
        Zodiac3,

        /// <summary>
        /// Text formatting. Capitalizes the first letter of each word.
        /// </summary>
        Caps,
        /// <summary>
        /// Text formatting. Capitalizes the first letter of the first word.
        /// </summary>
        FirstCap,
        /// <summary>
        /// Text formatting. All letters are lowercase.
        /// </summary>
        Lower,
        /// <summary>
        /// Text formatting. All letters are uppercase.
        /// </summary>
        Upper,

        /// <summary>
        /// Field result formatting. The CHARFORMAT instruction.
        /// </summary>
        CharFormat,
        /// <summary>
        /// Field result formatting. The MERGEFORMAT instruction.
        /// </summary>
        MergeFormat,
        /// <summary>
        /// Field result formatting. The MERGEFORMATINET instruction.
        /// </summary>
        MergeFormatInet
    }
}
