// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2022 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Italian language.
    /// </summary>
    internal class ItalianTextualNumber : EuropeanTextualNumberProvider
    {
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
            if (hundreds != 1)
                AppendWord(builder, GetNumeralFirst19(hundreds, true), false);

            AppendWord(builder, Hundred(thousands, thousandIndex, isCardinal), false);
        }

        private static string Hundred(IList<int> thousands, int thousandIndex, bool isCardinal)
        {
            int thousand = thousands[thousandIndex];
            int hundred = thousand % 100;
            int tens = hundred / 10;

            if (isCardinal)
                return "cento";

            if ((tens == 8) || (hundred <= 1) || (hundred == 8))
                return "cent";

            return "cento";
        }

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
            if (!isCardinal && TryAppendOrdinalHundred(builder, value, hundreds, thousands))
                return;

            base.AppendHundred(builder, value, hundreds, true, hasHigherDigit, hasLowerDigit, isCardinalOriginal, thousands, thousandIndex);

            if (isCardinal)
                return;

            TrimTrailingVowel(builder);
            RemoveTrailingAcuteAccent(builder);
        }

        private bool TryAppendOrdinalHundred(StringBuilder builder, int value, int hundreds, IList<int> thousands)
        {
            if ((thousands.Count == 1) && (hundreds == 0) && (value < 10))
            {
                AppendWord(builder, gOrdinalFirst9[value - 1], false);
                return true;
            }

            if (value == 10)
            {
                AppendWord(builder, "decimo", false);
                return true;
            }

            return false;
        }

        private static void TrimTrailingVowel(StringBuilder builder)
        {
            if (!IsVowel(builder[builder.Length - 1]))
                return;

            if (IsVowel(builder[builder.Length - 2]))
                return;

            builder.Length -= 1;
        }

        private static bool IsVowel(char c)
        {
            return Array.IndexOf(gVowels, c) != -1;
        }

        private static void RemoveTrailingAcuteAccent(StringBuilder builder)
        {
            if (builder[builder.Length - 1] == 'é')
                builder[builder.Length - 1] = 'e';
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
                    if (!isCardinal && (thousands[0] == 0))
                        return "mill";

                    return thousand == 1
                        ? "mille"
                        : "mila";
                case 2:
                    return isCardinal ? "milione" : "milion";
                case 3:
                    return isCardinal ? "miliardo" : "miliard";
                default:
                    throw new ArgumentOutOfRangeException("thousandIndex");
            }
        }

        protected override void AppendHundredFirst19(
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
            Debug.Assert(isCardinal);

            if (!hasLowerDigit && (value == 3) && ((hundreds != 0) || hasHigherDigit))
            {
                AppendWord(builder, ThreeWithAcuteAccent, false);
                return;
            }

            base.AppendHundredFirst19(builder, value, hundreds, true, hasHigherDigit, hasLowerDigit, isCardinalOriginal, thousands, thousandIndex);
        }

        protected override void AppendHundredOther(
            StringBuilder builder,
            int tens,
            int value,
            int hundreds,
            bool isCardinal,
            bool hasHigherDigit,
            bool hasLowerDigit,
            bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            Debug.Assert(isCardinal);

            if ((value == 1) || (value == 8))
            {
                string word = GetNumeralTen(tens, true).TrimEnd(gVowels) + GetNumeralFirst19(value, true);
                AppendWord(builder, word, false);
                return;
            }

            if (!hasLowerDigit && (value == 3))
            {
                string word = GetNumeralTen(tens, true) + ThreeWithAcuteAccent;
                AppendWord(builder, word, false);
                return;
            }

            base.AppendHundredOther(builder, tens, value, hundreds, true, hasHigherDigit, hasLowerDigit, isCardinalOriginal, thousands, thousandIndex);
        }

        internal override string GetNumberSuffix(long value, bool isCardinal)
        {
            if (isCardinal)
                return string.Empty;

            if ((value <= 10) || (value % 100 == 10))
                return string.Empty;

            return "esimo";
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return string.Empty; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return false; }
        }

        internal override string And
        {
            get { return "e"; }
        }

        protected override string[] OrdinalFirst19
        {
            get {throw new InvalidOperationException(); }
        }

        protected override string[] CardinalFirst19
        {
            get { return gCardinalFirst19; }
        }

        protected override string[] OrdinalTens
        {
            get { throw new InvalidOperationException(); }
        }

        protected override string[] CardinalTens
        {
            get { return gCardinalTens; }
        }

        protected override string SeparatorTensOnes
        {
            get { return string.Empty; }
        }

        protected override string SeparatorHundredsTens
        {
            get { return string.Empty; }
        }

        protected override string Separator
        {
            get { return string.Empty; }
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "uno", "due", "tre", "quattro", "cinque", "sei", "sette", "otto", "nove", "dieci",
            "undici", "dodici", "tredici", "quattordici", "quindici", "sedici", "diciassette", "diciotto", "diciannove"
        };

        private static readonly string[] gCardinalTens =
        {
            "venti", "trenta", "quaranta", "cinquanta", "sessanta", "settanta", "ottanta", "novanta"
        };

        private static readonly string[] gOrdinalFirst9 =
        {
            "primo", "secondo", "terzo", "quarto", "quinto", "sesto", "settimo", "ottavo", "nono"
        };

        private static readonly char[] gVowels = { 'a', 'o', 'i', 'e' };

        private const string ThreeWithAcuteAccent = "tré";
    }
}
