// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Romanian language.
    /// </summary>
    internal class RomanianTextualNumber : EuropeanTextualNumberProvider
    {
        protected override void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            string word = GetNumeralFirst19(value, isCardinal, isCardinalOriginal, thousandIndex, hasHigherDigit);

            AppendWord(builder, word, true, Separator);
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
            string word = GetNumeralTen(tens, isCardinal) + " și " + GetNumeralFirst19(value, isCardinal, isCardinalOriginal, 0, true);
            AppendWord(builder, word, true, Separator);
        }

        private string GetNumeralFirst19(int value, bool isCardinal, bool isCardinalOriginal, int thousandIndex, bool hasHigherDigit)
        {
            if (thousandIndex > 0)
                return GetBigNumeralFirst19(value, isCardinal, thousandIndex);

            if (!isCardinalOriginal)
            {
                if ((value == 1) && !hasHigherDigit)
                    return "întâi";

                if (value == 8)
                    return "optu";
            }

            return GetNumeralFirst19(value, isCardinal);
        }

        private string GetBigNumeralFirst19(int value, bool isCardinal, int thousandIndex)
        {
            switch (value)
            {
                case 1:
                    return thousandIndex < 2
                        ? "o"
                        : "un";
                case 2:
                    return "două";
                default:
                    return GetNumeralFirst19(value, isCardinal);
            }
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            AppendWord(builder, GetBigNumeralFirst19(hundreds, true, 0), true);
            AppendWord(builder, Hundred(hundreds), true);
        }

        private static string Hundred(int hundreds)
        {
           return hundreds == 1 ? "sută" : "sute";
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return string.Empty; }
        }

        protected override string SeparatorTensOnes
        {
            get { return " "; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return true; }
        }

        internal override string And
        {
            get { return "şi"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return "zero";
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            switch (thousandIndex)
            {
                case 1:
                    int hundred = thousand % 100;

                    if ((hundred == 1) || (hundred == 2))
                        return "mie";

                    if ((hundred == 0) || (hundred >= 20))
                        return "de mii";

                    return "mii";
                case 2:
                    return "milion";
                case 3:
                    return "miliard";
                default:
                    throw new ArgumentOutOfRangeException("thousandIndex");
            }
        }

        internal override string GetNumberPrefix(long value, bool isCardinal)
        {
            if (UsePrefixAndSuffix(value, isCardinal))
                return "al ";

            return base.GetNumberPrefix(value, isCardinal);
        }

        internal override string GetNumberSuffix(long value, bool isCardinal)
        {
            if (UsePrefixAndSuffix(value, isCardinal))
                return value < 1000000 ? "lea" : "ulea";

            return base.GetNumberSuffix(value, isCardinal);
        }

        private static bool UsePrefixAndSuffix(long value, bool isCardinal)
        {
            return !isCardinal && value > 1;
        }

        protected override string[] OrdinalFirst19
        {
            get { return gFirst19; }
        }

        protected override string[] CardinalFirst19
        {
            get { return gFirst19; }
        }

        protected override string[] OrdinalTens
        {
            get { return gTens; }
        }

        protected override string[] CardinalTens
        {
            get { return gTens; }
        }

        private static readonly string[] gFirst19 =
        {
            "unu", "doi", "trei", "patru", "cinci", "șase", "șapte", "opt", "nouă", "zece",
            "unsprezece", "doisprezece", "treisprezece", "paisprezece", "cincisprezece", "șaisprezece", "șaptesprezece", "optsprezece", "nouăsprezece"
        };

        private static readonly string[] gTens =
        {
            "douăzeci", "treizeci", "patruzeci", "cincizeci", "șaizeci", "șaptezeci", "optzeci", "nouăzeci"
        };
    }
}
