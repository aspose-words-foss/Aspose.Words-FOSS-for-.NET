// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Croatian language.
    /// </summary>
    internal class CroatianTextualNumber : EuropeanTextualNumberProvider
    {
        protected override void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            string word = GetNumeralFirst19(value, isCardinal, thousandIndex);
            AppendWord(builder, word, true, Separator);
        }

        protected override void AppendHundredTen(StringBuilder builder, int tens, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            AppendWord(builder, GetNumeralTen(tens, isCardinal, isCardinalOriginal), true);
        }

        protected override void AppendHundredOther(StringBuilder builder, int tens, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            string word = GetNumeralTen(tens, isCardinal, isCardinalOriginal) + Separator + GetNumeralFirst19(value, isCardinal);
            AppendWord(builder, word, true, Separator);
        }

        private string GetNumeralFirst19(int value, bool isCardinal, int thousandIndex)
        {
            if (isCardinal && (thousandIndex != 0))
            {
                switch (value)
                {
                    case 1:
                        return "jedna";
                    case 2:
                        return "dvije";
                    default:
                        // Ignore.
                        break;
                }
            }

            return GetNumeralFirst19(value, isCardinal);
        }

        private string GetNumeralTen(int value, bool isCardinal, bool isCardinalOriginal)
        {
            if (isCardinal && !isCardinalOriginal && (value == 3))
                return "tridest";

            return GetNumeralTen(value, isCardinal);
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            if (hundreds != 1)
                AppendWord(builder, GetHundredsNumeral(hundreds, isCardinal, isNonZeroHundreds), true);

            AppendWord(builder, Hundred(isCardinal, isNonZeroHundreds), hundreds == 1);
        }

        private string GetHundredsNumeral(int hundreds, bool isCardinal, bool isNonZeroHundreds)
        {
            switch (hundreds)
            {
                case 2:
                    return "dvje";
                case 6:
                    if (isCardinal || isNonZeroHundreds)
                        return "še";
                    break;
                default:
                    // Ignore.
                    break;
            }

            return GetNumeralFirst19(hundreds, true);
        }

        private static string Hundred(bool isCardinal, bool isNonZeroHundreds)
        {
            if (!isCardinal && !isNonZeroHundreds)
                return "stoti";

            return "sto";
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return string.Empty; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return false; }
        }

        protected override string SeparatorTensOnes
        {
            get { return " "; }
        }

        internal override string And
        {
            get { return "i"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return isCardinal ? "nula" : "nulti";
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            switch (thousandIndex)
            {
                case 1:
                    if (!isCardinal && IsLastNonZeroThousand(thousands, thousandIndex))
                        return "tisućiti";

                    int value = -1;

                    int hundred = thousand % 100;
                    if (hundred == 0)
                        value = thousand / 100;
                    else if ((hundred < 10) || (hundred >= 20))
                        value = thousand % 10;

                    switch (value)
                    {
                        case 0:
                        case 1:
                            return "tisuću";
                        case 2:
                        case 3:
                        case 4:
                            return "tisuće";
                        default:
                            return "tisuća";
                    }
                case 2:
                    return "milijun";
                case 3:
                    return "milijarda";
                default:
                    throw new ArgumentOutOfRangeException("thousandIndex");
            }
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
            "jedan", "dva", "tri", "četiri", "pet", "šest", "sedam", "osam", "devet", "deset",
            "jedanaest", "dvanaest", "trinaest", "četrnaest", "petnaest", "šesnaest", "sedamnaest", "osamnaest", "devetnaest"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "prvi", "drugi", "treći", "četvrti", "peti", "šesti", "sedmi", "osmi", "deveti", "deseti",
            "jedanaesti", "dvanaesti", "trinaesti", "četrnaesti", "petnaesti", "šesnaesti", "sedamnaesti", "osamnaesti", "devetnaesti"
        };

        private static readonly string[] gCardinalTens =
        {
            "dvadeset", "trideset", "četrdeset", "pedeset", "šezdeset", "sedamdeset", "osamdeset", "devedeset"
        };

        private static readonly string[] gOrdinalTens =
        {
            "dvadeseti", "trideseti", "četrdeseti", "pedeseti", "šezdeseti", "sedamdeseti", "osamdeseti", "devedeseti"
        };
    }
}
