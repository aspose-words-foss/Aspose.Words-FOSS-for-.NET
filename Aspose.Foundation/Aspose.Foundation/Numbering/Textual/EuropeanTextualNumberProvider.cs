// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2013 by Denis Darkin

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    ///  Abstracts ordinal and cardinal textual number representation in European languages.
    /// </summary>
    internal abstract class EuropeanTextualNumberProvider
    {
        internal virtual void AppendHundred(StringBuilder builder, int value, int hundreds, bool isCardinal, bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            if (value < 20)
            {
                // WORDSNET-4984 Index out of range when converting 100.
                if (value > 0)
                {
                    AppendHundredFirst19(builder, value, hundreds, isCardinal, hasHigherDigit, hasLowerDigit, isCardinalOriginal, thousands, thousandIndex);
                }
            }
            else // 20-99
            {
                int tens = value / 10;
                value -= 10 * tens;

                if (value == 0) // 20, 30, 40, ..., 90
                {
                    AppendHundredTen(builder, tens, hundreds, isCardinal, hasHigherDigit, hasLowerDigit, isCardinalOriginal, thousands, thousandIndex);
                }
                else
                {
                    AppendHundredOther(builder, tens, value, hundreds, isCardinal, hasHigherDigit, hasLowerDigit, isCardinalOriginal, thousands, thousandIndex);
                }
            }
        }

        protected virtual void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal, bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            bool requireSpecialSeparator = RequireHundredSpecialSeparator(isCardinal, hasHigherDigit);

            // German cardinal numerals have "eins" for one, but for any 1-containing number it is "ein", not "eins"
            string word = ((value == 1) && IsGerman && isCardinal && !hasLowerDigit) ? "eins" : GetNumeralFirst19(value, isCardinal);

            AppendWord(builder, word, true, requireSpecialSeparator ? SeparatorTensOnes : Separator);
        }

        protected virtual void AppendHundredTen(StringBuilder builder, int tens, int hundreds, bool isCardinal, bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            AppendWord(builder, GetNumeralTen(tens, isCardinal), true);
        }

        protected virtual void AppendHundredOther(StringBuilder builder, int tens, int value, int hundreds, bool isCardinal, bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            bool requireSpecialSeparator = RequireHundredSpecialSeparator(isCardinal, hasHigherDigit);

            string separator = (isCardinal || IsGerman) ? SeparatorTensOnes : Separator; // German ordinals have Separator between Tens and Ones too.
            string word = !IsGerman
                ? GetNumeralTen(tens, isCardinal) + separator + GetNumeralFirst19(value, isCardinal)
                : GetNumeralFirst19(value, true) + separator + GetNumeralTen(tens, isCardinal);

            AppendWord(builder, word, true, requireSpecialSeparator ? SeparatorHundredsTens : Separator);
        }

        private bool RequireHundredSpecialSeparator(bool isCardinal, bool hasHigherDigit)
        {
            // in German there are no separators between digits (exept in below 99)
            return hasHigherDigit && isCardinal && !IsGerman;
        }

        internal abstract void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds, bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex);

        internal virtual void AppendThousandsOrdinalSuffix(StringBuilder builder)
        {
            builder.Append(ThousandsOrdinalSuffix);
        }

        /// <summary>
        /// In certain languages numbers are prepended (e.g. German) so we will have to
        /// delegate Appending procedure to concrete classes.
        /// </summary>
        /// <remarks>
        /// Default implementation appends a space (' ') and then a word to a given StringBuilder.
        /// </remarks>
        internal void AppendWord(StringBuilder builder, string word, bool useSeparator)
        {
            AppendWord(builder, word, useSeparator, Separator);
        }

        protected static void AppendWord(StringBuilder builder, string word, bool useSeparator, string separator)
        {
            if (useSeparator && builder.Length != 0)
                builder.Append(separator);
            builder.Append(word);
        }

        protected string GetNumeralFirst19(int n, bool isCardinal)
        {
            int index = n - 1;
            return isCardinal ? CardinalFirst19[index] : OrdinalFirst19[index];
        }

        protected string GetNumeralTen(int tens, bool isCardinal)
        {
            int index = tens - 2;
            return isCardinal ? CardinalTens[index] : OrdinalTens[index];
        }

        /// <summary>
        /// Specifies if leading digit of ordinal big numbers should be merged.
        /// For English it is false: 1000 is written as one thousandth.
        /// For Spanish it is true: 1000 is written as onethousandth
        /// </summary>
        /// <param name="thousand">Number of thousands. Example 1000 - 1, 23000 - 23</param>
        protected virtual bool MergeOrdinalThousands(int thousand)
        {
            return false;
        }

        /// <summary>
        /// Specifies using separators for ordinal or cardinal big numbers.
        /// </summary>
        /// <param name="isCardinal"></param>
        /// <param name="thousand"></param>
        /// <param name="thousands"></param>
        /// <param name="thousandIndex"></param>
        /// <returns></returns>
        internal virtual bool UseThousandSeparator(bool isCardinal, int thousand, List<int> thousands, int thousandIndex)
        {
            return isCardinal || !MergeOrdinalThousands(thousand);
        }

        /// <summary>
        /// Specifies suffix to add to thousands in ordinal enumeration.
        /// E.g. for English it is "th" to get "thousandth" from 10000.
        /// </summary>
        protected abstract string ThousandsOrdinalSuffix { get; }

        /// <summary>
        /// For some languages e.g. English 1000 is written like one thousand.
        /// But in others, e.g. Spanish, writing "one" is excessive.
        /// This property specifies this behavior for each concrete language.
        /// </summary>
        internal abstract bool IncludeOneInBigNumbers { get; }

        /// <summary>
        /// German numbers between 21 and 99 that are not multiples of ten (30, 40, 50 ...) are expressed in reverse:
        /// one-and-twenty, two-and-twenty, etc.
        /// Note that these are the same numbers that are combined with a hyphen when written out in English.
        /// </summary>
        protected virtual bool IsGerman
        {
            get { return false; }
        }

        /// <summary>
        /// Default separator between textualized numbers.
        /// </summary>
        protected virtual string Separator
        {
            get { return " "; }
        }

        /// <summary>
        /// Separator between first and second digit: e.g. between 8 and 5 in number 1985.
        /// in certain languages it is "-" e.g. English, but in some - analog of " and "
        /// </summary>
        protected virtual string SeparatorTensOnes
        {
            get { return "-"; }
        }

        /// <summary>
        /// Separator between second and third digit: e.g. between 9 and 8 in number 1985.
        /// in certain languages it is " ", but in some an analog of " and "
        /// </summary>
        protected virtual string SeparatorHundredsTens
        {
            get { return " "; }
        }

        /// <summary>
        /// Returns a boolean value indicating either to include numeral for thousands word.
        /// E.g. For 1000: "one thousand" instead of "thousand".
        /// </summary>
        internal virtual bool IncludeThousandsWord(long value)
        {
            return true;
        }

        /// <summary>
        /// Indicates whether the first letter is always uppercase.
        /// </summary>
        internal virtual bool IsFirstLetterAlwaysUppercase
        {
            get { return false; }
        }

        internal abstract string And { get; }

        internal abstract string Zero(bool isCardinal);

        internal abstract string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands);

        internal virtual string GetNumberPrefix(long value, bool isCardinal)
        {
            return string.Empty;
        }

        internal virtual string GetNumberSuffix(long value, bool isCardinal)
        {
            return string.Empty;
        }

        protected abstract string[] OrdinalFirst19 { get; }
        protected abstract string[] CardinalFirst19 { get; }
        protected abstract string[] OrdinalTens { get; }
        protected abstract string[] CardinalTens { get; }

        protected static bool IsLastNonZeroThousand(List<int> thousands, int thousandIndex)
        {
            for (int i = 0; i < thousandIndex; i++)
                if (thousands[i] != 0)
                    return false;

            return true;
        }
    }
}
