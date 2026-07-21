// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Polish language.
    /// </summary>
    internal class PolishTextualNumber : EuropeanTextualNumberProvider
    {
        protected override void AppendHundredFirst19(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            bool useSeparator = UseHundredSeparator(hundreds, isCardinal, hasHigherDigit, isCardinalOriginal, thousands, thousandIndex);
            string word = GetNumeralFirst19(value, isCardinalOriginal, thousands, thousandIndex);
            AppendWord(builder, word, true, useSeparator ? Separator : string.Empty);
        }

        protected override void AppendHundredTen(StringBuilder builder, int tens, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            bool useSeparator = UseHundredSeparator(hundreds, isCardinal, hasHigherDigit, isCardinalOriginal, thousands, thousandIndex);
            AppendWord(builder, GetNumeralTen(tens, isCardinalOriginal, thousands, thousandIndex), useSeparator);
        }

        protected override void AppendHundredOther(StringBuilder builder, int tens, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal,
            List<int> thousands,
            int thousandIndex)
        {
            bool useSeparator = UseHundredSeparator(hundreds, isCardinal, hasHigherDigit, isCardinalOriginal, thousands, thousandIndex);
            string tensSeparator = (isCardinal || !hasLowerDigit) && (isCardinalOriginal || (thousandIndex == 0) || !IsLastNonZeroThousand(thousands, thousandIndex))
                ? Separator
                : string.Empty;
            string word = GetNumeralTen(tens, isCardinalOriginal, thousands, thousandIndex) + tensSeparator + GetNumeralFirst19(value, isCardinalOriginal, thousands, thousandIndex);
            AppendWord(builder, word, true, useSeparator ? Separator : string.Empty);
        }

        private static bool UseHundredSeparator(int hundreds, bool isCardinal, bool hasHigherDigit, bool isCardinalOriginal, List<int> thousands, int thousandIndex)
        {
            return (hundreds != 0 || UseSeparator(isCardinal, hasHigherDigit, thousands, thousandIndex)) &&
                   (isCardinalOriginal || (thousandIndex == 0) || !IsLastNonZeroThousand(thousands, thousandIndex));
        }

        private string GetNumeralFirst19(int value, bool isCardinal, List<int> thousands, int thousandIndex)
        {
            if (isCardinal)
                return GetNumeralFirst19(value, true);

            if (thousandIndex == 0)
                return GetNumeralFirst19(value, false);

            if (!IsLastNonZeroThousand(thousands, thousandIndex))
                return GetNumeralFirst19(value, true);

            return gOrdinalBigs[value - 1];
        }

        private string GetNumeralTen(int value, bool isCardinal, List<int> thousands, int thousandIndex)
        {
            if (isCardinal)
                return GetNumeralTen(value, true);

            if (thousandIndex == 0)
                return GetNumeralTen(value, false);

            if (!IsLastNonZeroThousand(thousands, thousandIndex))
                return GetNumeralTen(value, true);

            return gOrdinalBigTens[value - 1];
        }

        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            bool useSeparator = UseSeparator(isCardinal, hasHigherDigits, thousands, thousandIndex);
            if (hundreds != 1)
                AppendWord(builder, GetHundredsNumeral(hundreds, isCardinalOriginal, isNonZeroHundreds, thousands, thousandIndex), useSeparator);

            AppendWord(builder, Hundred(hundreds, isCardinalOriginal, isNonZeroHundreds, thousands, thousandIndex), hundreds == 1 && useSeparator);
        }

        private static bool UseSeparator(bool isCardinal, bool hasHigherDigit, List<int> thousands, int thousandIndex)
        {
            return isCardinal || !hasHigherDigit || (thousands[thousandIndex + 1] % 100 < 20) || (thousands[thousandIndex + 1] % 10 != 0);
        }

        private static string Hundred(int hundreds, bool isCardinal, bool isNonZeroHundreds, List<int> thousands, int thousandIndex)
        {
            if (isCardinal || ((thousandIndex == 0) && isNonZeroHundreds) || ((thousandIndex != 0) && !IsLastNonZeroThousand(thousands, thousandIndex)))
            {
                switch (hundreds)
                {
                    case 1:
                        return "sto";
                    case 2:
                        return "ście";
                    case 3:
                    case 4:
                        return "sta";
                    default:
                        return "set";
                }
            }

            if (thousandIndex == 0)
                return "setny";

            return hundreds < 5 ? "stu" : "set";
        }

        private string GetHundredsNumeral(int hundreds, bool isCardinal, bool isNonZeroHundreds, List<int> thousands, int thousandIndex)
        {
            if (isCardinal)
            {
                if (hundreds == 2)
                    return "dwie";
            }
            else
            {
                if (thousandIndex == 0)
                {
                    if (hundreds == 2)
                        return isNonZeroHundreds ? "dwie" : "dwu";
                }
                else
                {
                    if (IsLastNonZeroThousand(thousands, thousandIndex))
                    {
                        switch (hundreds)
                        {
                            case 2:
                                return "dwu";
                            case 6:
                                return "szeuść";
                            default:
                                break;
                        }
                    }
                    else
                    {
                        if (hundreds == 2)
                            return "dwie";
                    }
                }
            }

            return GetNumeralFirst19(hundreds, true);
        }

        internal override bool UseThousandSeparator(bool isCardinal, int thousand, List<int> thousands, int thousandIndex)
        {
            if (!base.UseThousandSeparator(isCardinal, thousand, thousands, thousandIndex))
                return false;

            if (isCardinal)
                return true;

            return !IsLastNonZeroThousand(thousands, thousandIndex);
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
            get { return "i"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return isCardinal ? "zero" : "zerowy";
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            switch (thousandIndex)
            {
                case 1:
                    if (thousand == 1)
                    {
                        return !isCardinal && (thousands[0] == 0)
                            ? "tysiączny"
                            : "tysiąc";
                    }

                    if (!isCardinal && (thousands[0] == 0))
                        return "tysięczny";

                    switch (thousand % 10)
                    {
                        case 2:
                        case 3:
                        case 4:
                            int hundred = thousand % 100;
                            if ((hundred < 10) || (hundred > 20))
                                return "tysiące";
                            break;
                        default:
                            break;
                    }

                    return "tysięcy";
                case 2:
                    return isCardinal ? "milion" : "milionowy";
                case 3:
                    return isCardinal ? "miliard" : "miliardowy";
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
            "jeden", "dwa", "trzy", "cztery", "pięć", "sześć", "siedem", "osiem", "dziewięć", "dziesięć",
            "jedenaście", "dwanaście", "trzynaście", "czternaście", "piętnaście", "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "pierwszy", "drugi", "trzeci", "czwarty", "piąty", "szósty", "siódmy", "ósmy", "dziewiąty", "dziesiąty",
            "jedenasty", "dwunasty", "trzynasty", "czternasty", "piętnasty", "szesnasty", "siedemnasty", "osiemnasty", "dziewiętnasty"
        };

        private static readonly string[] gCardinalTens =
        {
            "dwadzieścia", "trzydzieści", "czterdzieści", "pięćdziesiąt", "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt"
        };

        private static readonly string[] gOrdinalTens =
        {
            "dwudziesty", "trzydziesty", "czterdziesty", "pięćdziesiąty", "sześćdziesiąty", "siedemdziesiąty", "osiemdziesiąty", "dziewięćdziesiąty"
        };

        private static readonly string[] gOrdinalBigTens =
        {
            "dziesiącio", "dwudziesto", "trzydziesto", "czterdziesto", "pięćdziesiącio", "sześćdziesiącio", "siedemdziesiącio", "osiemdziesiącio", "dziewięćdziesiącio"
        };

        private static readonly string[] gOrdinalBigs =
        {
            "jedno", "dwu", "trzy", "cztero", "piącio", "szeącio", "siedmio", "óąmio", "dziewiącio", "dziesiącio",
            "jedenasto", "dwunasto", "trzynasto", "czternasto", "piętnasto", "szesnasto", "siedemnasto", "osiemnasto", "dziewiętnasto"
        };
    }
}
