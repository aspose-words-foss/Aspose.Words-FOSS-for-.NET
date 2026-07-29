// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2013 by Denis Darkin

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Encapsulates ordinal/cardinal nuances of English.
    /// </summary>
    internal class EnglishTextualNumber : EuropeanTextualNumberProvider
    {
        protected override void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            AppendWord(builder, GetNumeralFirst19(value, isCardinal), true);
        }

        protected override void AppendHundredTen(StringBuilder builder, int tens, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            AppendWord(builder, GetNumeralTen(tens, isCardinal), true);
        }

        protected override void AppendHundredOther(StringBuilder builder, int tens, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            bool requireSpecialSeparator = hasHigherDigit && isCardinal;
            AppendWord(builder, GetNumeralTen(tens, true) + SeparatorTensOnes + GetNumeralFirst19(value, isCardinal), true, requireSpecialSeparator ? SeparatorHundredsTens : Separator);
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            bool cardinal = isCardinal || isNonZeroHundreds;
            AppendWord(builder, GetNumeralFirst19(hundreds, true), true);
            AppendWord(builder, Hundred(cardinal), true);
        }

        internal override string And
        {
            get { return "and"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return "zero";
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return "th"; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return true; }
        }

        protected override string[] CardinalFirst19
        {
            get { return gCardinalFirst19; }
        }

        protected override string[] CardinalTens
        {
            get { return gCardinalTens; }
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            return gBigNumerals[thousandIndex - 1];
        }

        protected override string[] OrdinalFirst19
        {
            get { return gOrdinalFirst19; }
        }

        protected override string[] OrdinalTens
        {
            get { return gOrdinalTens; }
        }

        private static string Hundred(bool isCardinal)
        {
            return (isCardinal) ? "hundred" : "hundredth";
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
            "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "ninth", "tenth", "eleventh",
            "twelfth", "thirteenth", "fourteenth", "fifteenth", "sixteenth", "seventeenth", "eighteenth", "nineteenth"
        };

        private static readonly string[] gCardinalTens =
        {
            "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"
        };

        private static readonly string[] gOrdinalTens =
        {
            "twentieth", "thirtieth", "fortieth", "fiftieth", "sixtieth", "seventieth", "eightieth", "ninetieth"
        };

        private static readonly string[] gBigNumerals = { "thousand", "million", "billion" };
    }
}
