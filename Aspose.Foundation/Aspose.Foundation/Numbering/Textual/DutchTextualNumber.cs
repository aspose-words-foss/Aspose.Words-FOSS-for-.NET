// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2016 by Edward Voronov

using System.Collections.Generic;
using System.Text;
using Aspose.Common;

namespace Aspose.Numbering
{
    /// <summary>
    /// Encapsulates ordinal/cardinal nuances of Dutch.
    /// </summary>
    internal class DutchTextualNumber : EuropeanTextualNumberProvider
    {
        protected override void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            string word = GetNumeralFirst19(value, isCardinal);
            AppendWord(builder, word, false, string.Empty);
        }

        protected override void AppendHundredTen(StringBuilder builder, int tens, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            AppendWord(builder, GetNumeralTen(tens, isCardinal), false);
        }

        protected override void AppendHundredOther(StringBuilder builder, int tens, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            string numeralFirst19 = GetNumeralFirst19(value, isCardinal);
            //The dots on the ë (called trema) are used in Dutch to separate clashing vowels.
            string word = numeralFirst19 + (numeralFirst19[numeralFirst19.Length - 1] == 'e' ? "ën" : "en") + GetNumeralTen(tens, isCardinal) + (isCardinal ? string.Empty : "ste");
            AppendWord(builder, word, false, string.Empty);
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            AppendWord(builder, GetNumeralFirst19(hundreds, true), false);
            AppendWord(builder, Hundred(), false);
        }

        protected override string SeparatorTensOnes
        {
            get { return ""; }
        }

        internal override bool UseThousandSeparator(bool isCardinal, int thousand, List<int> thousands, int thousandIndex)
        {
            if ((thousand >= 1 && thousand <= 19) || (thousand >= 100 && thousand <= 119))
            {
                return false;
            }
            if (thousand >= 200)
            {
                char secondDigit = FormatterPal.IntToStr(thousand)[1];
                if (secondDigit == '0' || secondDigit == '1')
                {
                    return false;
                }
            }
            return isCardinal;
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return "ste"; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return true; }
        }

        internal override string And
        {
            get { return "en"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return "nul";
        }

        protected override string Separator
        {
            get { return " "; }
        }

        /// <summary>
        /// For Dutch only for 1000 no need to include "one" in big numbers.
        /// </summary>
        internal override bool IncludeThousandsWord(long value)
        {
            return value != 1000;
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            return gBigNumerals[thousandIndex - 1];
        }

        protected override string[] OrdinalFirst19
        {
            get { return gOrdinalFirst19; }
        }

        protected override string[] CardinalFirst19
        {
            get { return gCardinalFirst19; }
        }

        protected override string[] OrdinalTens
        {
            get { return gCardinalTens; }
        }

        protected override string[] CardinalTens
        {
            get { return gCardinalTens; }
        }

        private static string Hundred()
        {
            return "honderd";
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "één", "twee", "drie", "vier", "vijf", "zes", "zeven", "acht", "negen", "tien",
            "elf", "twaalf", "dertien", "veertien", "vijftien", "zestien", "zeventien", "achttien", "negentien"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "eerste", "tweede", "derde", "vierde", "vijfde", "zesde", "zevende", "achtse", "negende", "tiende", "elfde",
            "twaalfde", "dertiende", "veertiende", "vijftiende", "zestiende", "zeventiende", "achttiende", "negentiende"
        };

        private static readonly string[] gCardinalTens =
        {
            "twintig", "dertig", "veertig", "vijftig", "zestig", "zeventig", "tachtig", "negentig"
        };

        private static readonly string[] gBigNumerals = { "duizend", "miljoen", "miljard" };
    }
}
