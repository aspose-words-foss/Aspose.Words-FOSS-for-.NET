// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2014 by Edward Voronov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Encapsulates ordinal/cardinal nuances of French.
    /// </summary>
    internal class FrenchTextualNumber : EuropeanTextualNumberProvider
    {
        internal override void AppendHundred(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            if (value == 0)
                return;

            int tens = value/10;
            int ones = value - 10*tens;

            if (value < 70) // 1-69
            {
                if (value == 1 && !isCardinal && !hasHigherDigit && !hasLowerDigit)
                    AppendWord(builder, First, true); // 1
                else
                    AppendWord(builder, ConvertCore(value, isCardinal), true); // 2-69
            }
            else if (value < 80) // 70-79
            {
                AppendWord(builder,
                    GetFrenchNumeralTen(6, true) + GetSeparatorTensOnes(7, ones) + ConvertCore(value - 60, isCardinal),
                    true);
            }
            else if (value == 80) // 80
            {
                AppendWord(builder, isCardinalOriginal ? CardinalEighty : (hasLowerDigit ? Eighty : OrdinalEighty), true);
            }
            else // 81-99
            {
                AppendWord(builder, Eighty + GetSeparatorTensOnes(tens, ones) + ConvertCore(value - 80, isCardinal), true);
            }
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            bool cardinal = isCardinal || isNonZeroHundreds;
            if (hundreds != 1)
                AppendWord(builder, GetNumeralFirst16(hundreds, true), true);
            AppendWord(builder, Hundred(hundreds, cardinal, isNonZeroHundreds), true);
        }

        internal override void AppendThousandsOrdinalSuffix(StringBuilder builder)
        {
            // mille -> millième
            if (builder[builder.Length - 1] == 'e')
                builder.Length -= 1;

            base.AppendThousandsOrdinalSuffix(builder);
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return OrdinalSuffix; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return false; }
        }

        internal override string And
        {
            get { return "et"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return "zéro";
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            return gBigNumerals[thousandIndex - 1];
        }

        protected override string[] OrdinalFirst19
        {
            get { throw new System.NotImplementedException("The OrdinalFirst19 property not actual for FrenchTextualNumber"); }
        }

        protected override string[] CardinalFirst19
        {
            get { throw new System.NotImplementedException("The CardinalFirst19 property not actual for FrenchTextualNumber"); }
        }

        protected override string[] OrdinalTens
        {
            get { throw new System.NotImplementedException("The OrdinalTens property not actual for FrenchTextualNumber"); }
        }

        protected override string[] CardinalTens
        {
            get { throw new System.NotImplementedException("The CardinalTens property not actual for FrenchTextualNumber"); }
        }

        private static string GetNumeralFirst16(int n, bool isCardinal)
        {
            int index = n - 1;
            return isCardinal ? gCardinalFirst16[index] : gOrdinalFirst16[index];
        }

        private static string GetFrenchNumeralTen(int tens, bool isCardinal)
        {
            int index = tens - 1;
            return isCardinal ? gCardinalTensFirst6[index] : gOrdinalTensFirst6[index];
        }

        private static string Hundred(int hundreds, bool isCardinal, bool isNonZeroHundreds)
        {
            return isCardinal
                       ? (hundreds != 1 && !isNonZeroHundreds ? CardinalPluralHundred : CardinalSingularHundred)
                       : OrdinalHundred;
        }

        private static string GetSeparatorTensOnes(int tens, int ones)
        {
            if (tens > 7 || ones != 1)
            {
                return "-";
            }

            return " et ";
        }

        private static string ConvertCore(int value, bool isCardinal)
        {
            if (value <= 0)
            {
                return "";
            }

            if (value < 17) // 1-16
            {
                return GetNumeralFirst16(value, isCardinal);
            }

            if (value < 70) // 17-69
            {
                int tens = value / 10;
                value -= 10 * tens;

                if (value == 0)
                {
                    // 20, 30, 40, 50, 60
                    return GetFrenchNumeralTen(tens, isCardinal);
                }

                return GetFrenchNumeralTen(tens, true) + GetSeparatorTensOnes(tens, value) + GetNumeralFirst16(value, isCardinal);
            }

            return "";
        }

        private static readonly string[] gCardinalFirst16 =
        {
            "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "dix",
            "onze", "douze", "treize", "quatorze", "quinze", "seize"
        };

        private static readonly string[] gOrdinalFirst16 =
        {
            "unième", "deuxième", "troisième", "quatrième", "cinquième",  "sixième", "septième", "huitième", "neuvième", "dixième",
            "onzième", "douzième", "treizième", "quatorzième", "quinzième", "seizième"
        };

        private static readonly string[] gCardinalTensFirst6 =
        {
            "dix", "vingt", "trente", "quarante", "cinquante", "soixante"
        };

        private static readonly string[] gOrdinalTensFirst6 =
        {
            "dixième", "vingtième", "trentième", "quarantième", "cinquantième", "soixantième"
        };

        private static readonly string[] gBigNumerals = { "mille", "million", "milliard" };

        private const string OrdinalSuffix = "ième";

        private const string CardinalSingularHundred = "cent";

        private const string CardinalPluralHundred = CardinalSingularHundred + "s";

        private const string OrdinalHundred = CardinalSingularHundred + OrdinalSuffix;

        private const string CardinalEighty = "quatre-vingts";

        private const string Eighty = "quatre-vingt";

        private const string OrdinalEighty = "quatre-vingtième";

        private const string First = "premier";
    }
}
