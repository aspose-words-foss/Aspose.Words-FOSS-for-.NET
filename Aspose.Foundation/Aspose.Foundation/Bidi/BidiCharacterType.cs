// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

namespace Aspose.Bidi
{
    /// <summary>
    /// Bidirectional Character Types (Table 4 in the Unicode BiDi algorithm https://unicode.org/reports/tr9/).
    /// </summary>
    public enum BidiCharacterType
    {
        #region Strong Types
        /// <summary>
        /// Left-to-Right
        /// </summary>
        /// <example>
        /// LRM, most alphabetic, syllabic, Han ideographs, non-European or non-Arabic digits, ...
        /// </example>
        /// <remarks>Strong Type</remarks>
        L,

        /// <summary>
        /// Right-to-Left
        /// </summary>
        /// <example>
        /// RLM, Hebrew alphabet, and related punctuation
        /// </example>
        /// <remarks>Strong Type</remarks>
        R,

        /// <summary>
        /// Right-to-Left Arabic
        /// </summary>
        /// <example>
        /// Arabic, Thaana, and Syriac alphabets, most punctuation specific to those scripts, ...
        /// </example>
        /// <remarks>Strong Type</remarks>
        AL,
        #endregion

        #region Weak Types
        /// <summary>
        /// European Number
        /// </summary>
        /// <example>
        /// European digits, Eastern Arabic-Indic digits, ...
        /// </example>
        /// <remarks>Weak Type</remarks>
        EN,

        /// <summary>
        /// European Number Separator
        /// </summary>
        /// <example>
        /// Plus sign, minus sign
        /// </example>
        /// <remarks>Weak Type</remarks>
        ES,

        /// <summary>
        /// European Number Terminator
        /// </summary>
        /// <example>
        /// Degree sign, currency symbols, ...
        /// </example>
        /// <remarks>Weak Type</remarks>
        ET,

        /// <summary>
        /// Arabic Number
        /// </summary>
        /// <example>
        /// Arabic-Indic digits, Arabic decimal and thousands separators, ...
        /// </example>
        /// <remarks>Weak Type</remarks>
        AN,

        /// <summary>
        /// Common Number Separator
        /// </summary>
        /// <example>
        /// Colon, comma, full stop (period), No-break space, ...
        /// </example>
        /// <remarks>Weak Type</remarks>
        CS,

        /// <summary>
        /// Nonspacing Mark
        /// </summary>
        /// <example>
        /// Characters marked Mn (Nonspacing_Mark) and Me (Enclosing_Mark) in the Unicode Character Database
        /// </example>
        /// <remarks>Weak Type</remarks>
        NSM,

        /// <summary>
        /// Boundary Neutral
        /// </summary>
        /// <example>
        /// Most formatting and control characters, other than those explicitly given types above
        /// </example>
        /// <remarks>Weak Type</remarks>
        BN,
        #endregion

        #region Neutral Types
        /// <summary>
        /// Paragraph Separator
        /// </summary>
        /// <example>
        /// Paragraph separator, appropriate Newline Functions, higher-level protocol paragraph determination
        /// </example>
        /// <remarks>Neutral Type</remarks>
        B,
        /// <summary>
        /// Segment Separator
        /// </summary>
        /// <example>
        /// Tab
        /// </example>
        /// <remarks>Neutral Type</remarks>
        S,
        /// <summary>
        /// Whitespace
        /// </summary>
        /// <example>
        /// Space, figure space, line separator, form feed, General Punctuation spaces, ...
        /// </example>
        /// <remarks>Neutral Type</remarks>
        WS,
        /// <summary>
        /// Other Neutrals
        /// </summary>
        /// <example>
        /// All other characters, including OBJECT REPLACEMENT CHARACTER
        /// </example>
        /// <remarks>Neutral Type</remarks>
        ON,
        #endregion

        #region Explicit Formatting
        /// <summary>
        /// Pop Directional Format
        /// </summary>
        /// <example>
        /// PDF
        /// </example>
        /// <remarks>Explicit Formatting</remarks>
        PDF,

        /// <summary>
        /// Left-to-Right Embedding
        /// </summary>
        /// <example>
        /// LRE
        /// </example>
        /// <remarks>Explicit Formatting</remarks>
        LRE,
        /// <summary>
        /// Left-to-Right Override
        /// </summary>
        /// <example>
        /// LRO
        /// </example>
        /// <remarks>Explicit Formatting</remarks>
        LRO,
        /// <summary>
        /// Right-to-Left Embedding
        /// </summary>
        /// <example>
        /// RLE
        /// </example>
        /// <remarks>Explicit Formatting</remarks>
        RLE,
        /// <summary>
        /// Right-to-Left Override
        /// </summary>
        /// <example>
        /// RLO
        /// </example>
        /// <remarks>Explicit Formatting</remarks>
        RLO
        #endregion Explicit Formatting
    }
}
