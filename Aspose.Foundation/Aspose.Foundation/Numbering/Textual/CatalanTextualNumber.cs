// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2024 by Ilya Navrotskiy

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Catalan language.
    /// </summary>
    internal class CatalanTextualNumber : EuropeanTextualNumberProvider
    {
        /// <summary>
        /// Appends hundreds.
        /// </summary>
        internal override void AppendHundreds(
            StringBuilder builder,
            int hundreds,
            bool isCardinal,
            bool isNonZeroHundreds,
            bool hasHigherDigits,
            bool hasLowerDigits,
            bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            Debug.Assert((hundreds >= 1) && (hundreds <= 9));

            // Hundreds equal to '1' are not written.
            if (hundreds > 1)
            {
                string hundred = hasLowerDigits ? gBigHundreds[hundreds - 1] : GetNumeralFirst19(hundreds, true);
                AppendWord(builder, string.Format("{0}{1}", hundred, SeparatorOther), hasHigherDigits);
            }

            // When not a round hundred, like 100, 200,...900, the cardinal hundred has to be written.
            if (isNonZeroHundreds)
                AppendWord(builder, CardinalHundred, (hundreds == 1));
            else
                AppendWord(builder, isCardinal ? CardinalHundred : OrdinalHundred, hasHigherDigits && (hundreds == 1));

            // Add 's' for plural cardinal.
            if (isCardinal && hundreds > 1)
                AppendWord(builder, "s", false);
        }

        /// <summary>
        /// Appends value within hundreds (1-99).
        /// </summary>
        internal override void AppendHundred(
            StringBuilder builder,
            int value,
            int hundreds,
            bool isCardinal,
            bool hasHigherDigit,
            bool hasLowerDigit,
            bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            if (value < 1)
                return;

            if (value < 20)
            {
                AppendWord(builder, GetFirst19(value, isCardinal, hasHigherDigit, hasLowerDigit), true);
                return;
            }

            int tens = value / 10;
            int ones = value - 10 * tens;

            // 20, 30, 40, ..., 90
            // The special value 'vine' should be written for round cardinal '20' in big numbers.
            bool isBigRound20 = hasLowerDigit && (ones == 0) && (tens == 2);
            AppendWord(builder, (isBigRound20 && isCardinalOriginal) ? Big20 : GetNumeralTen(tens, isCardinal), true);

            if (ones == 0)
                return;

            // Resolve separator between tens and ones.
            if (!hasLowerDigit)
            {
                // Very first thousand.
                AppendWord(builder, SeparatorTensOnes, false);
            }
            else if (isCardinalOriginal)
            {
                // Cardinals.
                if (tens != 2)
                    AppendWord(builder, SeparatorHundredsTens, false);
            }
            else
            {
                // Ordinals.
                AppendWord(builder, (tens == 2) ? SeparatorTensOnes : SeparatorOther, false);
            }

            // Before 'mil' (1000) digit '1' should not be written.
            if (isCardinalOriginal && hasLowerDigit && (ones == 1))
                return;

            string one = GetFirst19(ones, isCardinal, true, hasLowerDigit);
            AppendWord(builder, one, false);
        }

        /// <summary>
        /// Catalan language not adds 1 'un' only for the first big number 'mil' and
        /// only for the very first thousand (>=2000).
        /// </summary>
        internal override bool IncludeThousandsWord(long value)
        {
            return (value >= 2000);
        }

        internal override bool UseThousandSeparator(bool isCardinal, int thousand, List<int> thousands, int thousandIndex)
        {
            bool hasLowerDigit = thousandIndex > 0 && !IsLastNonZeroThousand(thousands, thousandIndex);

            int ones = thousand % 10;
            int tens = (thousand - ones) / 10 % 10;
            // Do not add separator before thousand in the following cases,o be added already
            // (see details in resolving separators in AppendHundred())
            if (isCardinal && (ones == 1) && hasLowerDigit && (tens > 2))
                return false;

            return base.UseThousandSeparator(isCardinal, thousand, thousands, thousandIndex);
        }

        internal override void AppendThousandsOrdinalSuffix(StringBuilder builder)
        {
            // Add suffix only for mils.
            if (!StringUtil.IsEndsWith(builder, gBigNumerals[0]))
                return;

            base.AppendThousandsOrdinalSuffix(builder);
        }

        internal override string Zero(bool isCardinal)
        {
            return isCardinal ? "zero" : "";
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            return gBigNumerals[thousandIndex - 1];
        }

        /// <summary>
        /// Returns numbers 1-19.
        /// </summary>
        private string GetFirst19(int value, bool isCardinal, bool hasHigherDigit, bool hasLowerDigit)
        {
            if (value < 1)
                return "";

            // Catalan ordinal digits 1-3 are ordinal only when they are on their own,
            // i.e., equal exactly to 1, 2 or 3.
            bool useCardinal = (value <= 3) && (hasLowerDigit || hasHigherDigit);
            string s = GetNumeralFirst19(value, useCardinal || isCardinal);

            // Special processing for '1', '2' and '3'.
            // Cardinal one(and its derivatives) takes the reduced
            // form 'u' as a noun to refer to the Digit and the number 1.
            // In other cases it uses long form 'un'. And in ordinal numbers also adds suffix 'è'.
            // The last is true for '2' and '3' as well.
            const string longFormSuffix = "n";
            if ((value == 1) && ((!isCardinal && hasHigherDigit) || hasLowerDigit))
            {
                // Add 'n' to number 1 ('u[n]').
                s = string.Format("{0}{1}", s, longFormSuffix);
            }

            const string longOrdinalSuffix = "è";
            if (useCardinal && !isCardinal)
            {
                // Add 'é' to ordinal numbers 1-3 ('un[é]', 'dos[é]', 'tres[é]').
                s = string.Format("{0}{1}", s, hasLowerDigit ? string.Empty : longOrdinalSuffix);
            }

            return s;
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return true; }
        }

        internal override string And
        {
            get { return "coma"; }
        }

        protected override string[] CardinalFirst19
        {
            get { return gCardinalFirst19; }
        }

        protected override string[] CardinalTens
        {
            get { return gCardinalTens; }
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return OrdinalSuffix; }
        }

        protected override string[] OrdinalFirst19
        {
            get { return gOrdinalFirst19; }
        }

        protected override string[] OrdinalTens
        {
            get { return gOrdinalTens; }
        }

        protected override string SeparatorTensOnes
        {
            get { return "-i-"; }
        }

        protected override string SeparatorHundredsTens
        {
            get { return " y "; }
        }

        private const string SeparatorOther = "-";

        /// <summary>
        /// Catalan language uses the following hundred digits for numbers >= 100000.
        /// Note the digits 2, 3 and 5 are the same, as usual <see cref="gCardinalFirst19"/>,
        /// but lets use here full set of 1-9 for convenience.
        /// </summary>
        private static readonly string[] gBigHundreds =
        {
            "", "dos", "tres", "cuatro", "cinc", "seis", "sete", "ocho", "nove"
        };

        /// <summary>
        /// Cardinal 1-19.
        /// </summary>
        private static readonly string[] gCardinalFirst19 =
        {
            "u", "dos", "tres", "quatre", "cinc", "sis", "set", "vuit", "nou", "deu",
            "onze", "dotze", "tretze", "catorze", "quinze", "setze", "disset", "divuit", "dinou"
        };

        /// <summary>
        /// Ordinal 1-19.
        /// </summary>
        private static readonly string[] gOrdinalFirst19 =
        {
            "primer", "segon", "tercer", "quart", "cinquè", "sisè", "setè", "vuitè", "novè", "desè", "onzè",
            "dozè", "tretzè", "catorzè", "quinzè", "setzè", "dissetè", "divuitè", "dinovè"
        };

        /// <summary>
        /// Cardinal tens 20-90.
        /// </summary>
        private static readonly string[] gCardinalTens =
        {
            "vint", // 20
            "trenta", // 30
            "quaranta", // 40
            "cinquanta", // 50
            "seixanta", // 60
            "setanta", // 70
            "vuitanta", //80
            "noranta" //90
        };

        /// <summary>
        /// Ordinal tens 20-90.
        /// </summary>
        /// <remarks>
        /// There are some sources that describes another numerals for Ordinals, for example:
        /// https://omniglot.com/language/numbers/catalan.htm.
        /// But Word uses Cardinal tens the same as Ordinal ones, described here:
        /// https://ca.wikipedia.org/wiki/Numerals_en_català
        /// </remarks>
        private static readonly string[] gOrdinalTens = gCardinalTens;

        /// <summary>
        /// Big numbers '1000', '1.000.000', '1.000.000.000'.
        /// https://ca.wikipedia.org/wiki/Numerals_en_català
        /// </summary>
        private static readonly string[] gBigNumerals = { "mil", "milió", "miliard" };


        /// <summary>
        /// The special value 'vine' should be written for round cardinal '20' in big numbers.
        /// </summary>
        private const string Big20 = "vine";

        private const string OrdinalSuffix = "é";
        private const string CardinalHundred = "cent";
        private const string OrdinalHundred = "cené";
    }
}
