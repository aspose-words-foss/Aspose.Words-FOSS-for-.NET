// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the accent function, consisting of a base and a combining diacritical mark. 
    /// </summary>
    /// <remarks>
    /// If accPr is omitted, the default accent is U+0302 (COMBINING CIRCUMFLEX ACCENT).
    /// </remarks>
    internal class MathObjectAccent : MathObject
    {
        /// <summary>
        /// Returns <c>true</c> if the specified char is allowed for accent function (22.1.2.20 of ISO/IEC 29500-1).
        /// </summary>
        /// <remarks><para>
        /// This is from the Office 2010 documentation:
        /// https://docs.microsoft.com/en-us/previous-versions/office/developer/office-2010/cc804018(v=office.14)
        /// </para><para>
        /// But modern documentation has no restrictions on the use of the accent character:
        /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.office.interop.word.omathacc.char
        /// </para></remarks>
        internal static bool IsValidAccent(char value)
        {
            return (
                ((value >= '\u0300') && (value <= '\u036F')) ||
                ((value >= '\u20D0') && (value <= '\u20EF')) || IsAdditionalValidAccent(value)
            );
        }

        /// <summary>
        /// Validates the specified character.
        /// </summary>
        private static char ValidateCharacter(char value)
        {
            if (value < '\u0300')
                return '\u0300';

            // Char is located between to valid ranges, so see which boundary is closer and use this boundary
            // instead of char val U+121F is the middle of middle of (U+036F to U+20D0) interval.
            if ((value > '\u036F') && (value < '\u20D0'))
                return (value < '\u121F') ? '\u036F' : '\u20D0';

            return ((value > '\u20EF') && !IsAdditionalValidAccent(value)) ? '\u20EF' : value;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified char is allowed for accent function (extensions).
        /// </summary>
        /// <remarks><para>
        /// MS Word allows to use characters from Unicode Arrows block as accent. (2190—21FF, more info: 
        /// https://unicode-table.com/en/blocks/arrows/)
        /// </para><para>
        /// MS Word allows to use characters from Unicode Miscellaneous Technical block as accent. (2300—23FF,
        /// more info: https://unicode-table.com/en/blocks/miscellaneous-technical/)
        /// </para><para>
        /// May be it allows all characters, but for now limit only to this block.
        /// </para></remarks>
        private static bool IsAdditionalValidAccent(char value)
        {
            return (
                ((value >= '\u2190') && (value <= '\u21FF')) || // WORDSNET-21728
                ((value >= '\u2300') && (value <= '\u23FF'))    // WORDSNET-16200
            );
        }

        /// <summary>
        /// MathObject type.
        /// </summary>
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Accent; }
        }

        /// <summary>
        /// Specifies the character to be attached to the base of this accent.
        /// Default value is U+0302 (COMBINING CIRCUMFLEX ACCENT).
        /// This value should be within the range of (U+0300–U+036F) or (U+20D0–U+20EF), otherwise mapped to the closest boundary.
        /// </summary>
        internal char Character
        {
            get { return (char)FetchAttr(MathAttr.AccentCharacter); }
            set
            {
                char validatedValue = ValidateCharacter(value);
                SetAttr(MathAttr.AccentCharacter, validatedValue, validatedValue != DefaultCharacter);
            }
        }

        /// <summary>
        /// Default character.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultCharacter = '\u0302';
    }
}
