// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Czech language.
    /// </summary>
    internal class CzechTextualNumber : EuropeanTextualNumberProvider
    {
        protected override void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            bool useSeparator = UseHundredSeparator(isCardinalOriginal, hundreds, hasLowerDigit);
            string word = GetNumeralFirst19(value, isCardinalOriginal);
            AppendWord(builder, word, true, useSeparator ? Separator : string.Empty);
        }

        protected override void AppendHundredTen(StringBuilder builder, int tens, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            bool useSeparator = UseHundredSeparator(isCardinalOriginal, hundreds, hasLowerDigit);
            AppendWord(builder, GetNumeralTen(tens, isCardinalOriginal), useSeparator);
        }

        protected override void AppendHundredOther(StringBuilder builder, int tens, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            bool useSeparator = UseHundredSeparator(isCardinalOriginal, hundreds, hasLowerDigit);
            string tensSeparator = isCardinalOriginal || !hasLowerDigit || ((tens != 7) && (tens != 8))
                        ? Separator
                        : string.Empty;
            string word = GetNumeralTen(tens, isCardinalOriginal) + tensSeparator + GetNumeralFirst19(value, isCardinalOriginal);
            AppendWord(builder, word, true, useSeparator ? Separator : string.Empty);
        }

        private static bool UseHundredSeparator(bool isCardinalOriginal, int hundreds, bool hasLowerDigit)
        {
            return !isCardinalOriginal || (hundreds == 0) || !hasLowerDigit;
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            if ((hundreds != 1) || (isCardinalOriginal && hasLowerDigits))
                AppendWord(builder, GetHundredsNumeral(hundreds, isCardinalOriginal, hasLowerDigits), true);

            bool useSeparator = isCardinalOriginal
                ? !hasLowerDigits
                : hundreds == 1;

            AppendWord(builder, Hundred(hundreds, isCardinalOriginal, hasLowerDigits), useSeparator);
        }

        private static string Hundred(int hundreds, bool isCardinal, bool hasLowerDigits)
        {
            if (isCardinal)
            {
                if (hasLowerDigits)
                    return "sto";

                switch (hundreds)
                {
                    case 1:
                        return "sto";
                    case 2:
                        return "stě";
                    case 3:
                    case 4:
                        return "sta";
                    default:
                        return "set";
                }
            }

            return "stý";
        }

        private string GetHundredsNumeral(int hundreds, bool isCardinal, bool hasLowerDigits)
        {
            if (!isCardinal)
                return gOrdinalHundreds[hundreds - 1];

            if (!hasLowerDigits && (hundreds == 2))
                return "dvě";

            return GetNumeralFirst19(hundreds, true);
        }

        internal override bool UseThousandSeparator(bool isCardinal, int thousand, List<int> thousands, int thousandIndex)
        {
            if (!base.UseThousandSeparator(isCardinal, thousand, thousands, thousandIndex))
                return false;

            if (isCardinal)
                return (thousand % 100 != 0);

            switch (thousand % 100)
            {
                case 10:
                case 70:
                case 80:
                    return false;
                default:
                    return true;
            }
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
            get { return "a"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return isCardinal ? "nula" : "nultý";
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            switch (thousandIndex)
            {
                case 1:
                    if (!isCardinal)
                        return "tisící";

                    switch (thousand)
                    {
                        case 200:
                        case 300:
                        case 400:
                            return "tisíce";
                        default:
                            // Ignore.
                            break;
                    }

                    int hundred = thousand % 100;
                    switch (thousand % 10)
                    {
                        case 2:
                        case 3:
                        case 4:
                            if ((hundred < 10) || (hundred > 20))
                                return "tisíce";
                            break;
                        default:
                            // Ignore.
                            break;
                    }

                    return "tisíc";
                case 2:
                    return "miliónů";
                case 3:
                    return "miliarda";
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

        protected override string SeparatorTensOnes
        {
            get { return " "; }
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "jedna", "dva", "tři", "čtyři", "pět", "šest", "sedm", "osm", "devět", "deset",
            "jedenáct", "dvanáct", "třináct", "čtrnáct", "patnáct", "šestnáct", "sedmnáct", "osmnáct", "devatenáct"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "první", "druhý", "třetí", "čtvrtý", "pátý", "šestý", "sedmý", "osmý", "devátý", "desátý",
            "jedenáctý", "dvanáctý", "třináctý", "čtrnáctý", "patnáctý", "šestnáctý", "sedmnáctý", "osmnáctý", "devatenáctý"
        };

        private static readonly string[] gOrdinalHundreds =
        {
            string.Empty, "dvou", "tří", "čtyř", "pěti", "šesti", "sedmi", "osmi", "devíti"
        };

        private static readonly string[] gCardinalTens =
        {
            "dvacet", "třicet", "čtyřicet", "padesát", "šedesát", "sedmdesát", "osmdesát", "devadesát"
        };

        private static readonly string[] gOrdinalTens =
        {
            "dvacátý", "třicátý", "čtyřicátý", "padesátý", "šedesátý", "sedmdesátý", "osmdesátý", "devadesátý"
        };
    }
}
