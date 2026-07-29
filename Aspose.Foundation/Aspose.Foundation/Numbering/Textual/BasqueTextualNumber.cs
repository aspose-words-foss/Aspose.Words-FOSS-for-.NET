// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/03/2021 by Vasiliy Stepchenko

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    internal class BasqueTextualNumber : EuropeanTextualNumberProvider
    {
        // The textual values have been taken from Word (1 - 10000), 0 and bigger than 10K from internet.
        // https://www.languagesandnumbers.com/how-to-count-in-basque/en/eus/.

        internal override void AppendHundred(StringBuilder builder, int value, int hundreds, bool isCardinal, bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands, int thousandIndex)
        {
            if (value == 0)
                return;

            // There is a very special word for 1, not the same as in 21, 41, 101 and so on.
            if (!isCardinal && value == 1 && hundreds == 0 && thousands.Count == 1)
            {
                AppendWord(builder, OrdinalFirst, false);
                return;
            }

            // In values from the 1000 to the 1999 the word 'mila' (thousand) should be without word 'bat' (one).
            // But from the 2000 there should be the appropriate word for the number of thousands, 2000 -> 'bi' (two) 'mila'.
            if (thousands.Count == 2 && thousands[1] == 1 && thousandIndex == 1)
                return;

            bool useSeparator = ((hundreds > 0) || (thousands.Count > 0));

            if (value < 20)
            {
                AppendWord(builder, GetNumeralFirst19(value, true), useSeparator, SeparatorHundredsTens);
            }
            else
            {
                int twenty = value / 20;
                value -= 20 * twenty;

                AppendWord(builder, gCardinalTwenties[twenty - 1], useSeparator, SeparatorHundredsTens);

                if (value != 0)
                    AppendWord(builder, GetNumeralFirst19(value, true), true, SeparatorTensOnes);
            }

            if (!isCardinal)
                AppendWord(builder, OrdinalSuffix, false);
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds, bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal, List<int> thousands, int thousandIndex)
        {
            AppendWord(builder, gCardinalHundreds[hundreds - 1], hasHigherDigits);

            if (!isCardinal && !isNonZeroHundreds)
                AppendWord(builder, OrdinalSuffix, false);
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            return gBigCardinals[thousandIndex - 1];
        }

        internal override string Zero(bool isCardinal)
        {
            return isCardinal ? "zero" : "nulua";
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return "garren"; }
        }

        protected override string SeparatorTensOnes
        {
            get { return "ta "; }
        }

        protected override string SeparatorHundredsTens
        {
            get { return " eta "; }
        }

        protected override string Separator
        {
            get { return " "; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return true; }
        }

        /// <summary>
        /// For Basque only for 1000 no need to include "one" in big numbers.
        /// </summary>
        internal override bool IncludeThousandsWord(long value)
        {
            return value != 1000;
        }

        internal override bool UseThousandSeparator(bool isCardinal, int thousand, List<int> thousands, int thousandIndex)
        {
            return true;
        }

        protected override string[] CardinalFirst19
        {
            get { return gCardinalFirst19; }
        }

        protected override string[] OrdinalFirst19
        {
            get { throw new InvalidOperationException(); }
        }

        protected override string[] OrdinalTens
        {
            get { throw new InvalidOperationException(); }
        }

        protected override string[] CardinalTens
        {
            get { throw new InvalidOperationException(); }
        }

        internal override string And
        {
            get { return "koma"; }
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "bat", "bi", "hiru", "lau", "bost", "sei", "zazpi", "zortzi", "bederatzi", "hamar", "hamaika", "hamabi", "hamairu", "hamalau", "hamabost", "hamasei", "hamazazpi", "hemezortzi", "hemeretzi"
        };

        private static readonly string[] gCardinalTwenties =
        {
            "hogei", "berrogei", "hirurogei", "laurogei"
        };

        private static readonly string[] gCardinalHundreds =
        {
            "ehun", "berrehun", "hirurehun", "laurehun", "bostehun", "seiehun", "zazpiehun", "zortziehun", "bederatziehun"
        };

        private static readonly string[] gBigCardinals =
        {
            "mila", "milioi", "miliar"
        };

        private const string OrdinalSuffix = "garren";
        private const string OrdinalFirst = "lehenengo";
    }
}
