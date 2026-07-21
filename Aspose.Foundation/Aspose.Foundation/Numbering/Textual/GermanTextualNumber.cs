// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2013 by Denis Darkin

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    ///  Implements german-specific numbering nuances.
    /// </summary>
    /// <remarks>Unfortunately some of very "different" numbering rules could not be fully abtracted and leaked away into the
    /// EuropeanTextualNumberProvider base class where they are isolated by using virtual <see cref="IsGerman"/> member.
    /// Example of this is reverse writing of numbers in <see cref="EuropeanTextualNumberProvider.AppendHundred"/></remarks>
    internal class GermanTextualNumber : EuropeanTextualNumberProvider
    {
        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            Debug.Assert((hundreds >= 1) && (hundreds <= 9));

            if (hundreds != 1 || hasHigherDigits)
                AppendWord(builder, gCardinalFirst19[hundreds - 1], false);
            AppendWord(builder, "hundert", false);

            if (!(isCardinal || isNonZeroHundreds))
                AppendWord(builder, "ste", false);
        }

        protected override bool MergeOrdinalThousands(int thousand)
        {
             return true;
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
            get { return "und"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return isCardinal ? "null" : "nullte";
        }

        internal override bool IsFirstLetterAlwaysUppercase
        {
            get
            {
                return true;
            }
        }

        protected override bool IsGerman
        {
            get { return true; }
        }

        protected override string Separator
        {
            get { return ""; }
        }

        protected override string SeparatorHundredsTens
        {
            get { return ""; }
        }

        protected override string SeparatorTensOnes
        {
            get { return "und"; }
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
            get { return gOrdinalTens; }
        }

        protected override string[] CardinalTens
        {
            get { return gCardinalTens; }
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "ein", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun", "zehn",
            "elf", "zwölf", "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn"
        };

        private static readonly string[] gCardinalTens =
        {
            "zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "erste", "zweite", "dritte", "vierte", "fünfte", "sechste", "siebente", "achte", "neunte", "zehnte", "elfte",
            "zwölfte", "dreizehnte", "vierzehnte", "fünfzehnte", "sechzehnte", "siebzehnte", "achtzehnte", "neunzehnte"
        };

        private static readonly string[] gOrdinalTens =
        {
            "zwanzigste", "dreißigste", "vierzigste", "fünfzigste", "sechzigste", "siebzigste", "achtzigste", "neunzigste"
        };

        private static readonly string[] gBigNumerals = { "tausend", "million", "millarde" };
    }
}
