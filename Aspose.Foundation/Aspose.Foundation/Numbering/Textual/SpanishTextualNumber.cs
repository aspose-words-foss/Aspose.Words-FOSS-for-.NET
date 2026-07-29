// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2013 by Denis Darkin

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Spanish language.
    /// </summary>
    internal class SpanishTextualNumber : EuropeanTextualNumberProvider
    {
        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            Debug.Assert((hundreds >= 1) && (hundreds <= 9));
            AppendWord(builder, isCardinal ? gCardinalHundreds[hundreds - 1] : gOrdinalHundreds[hundreds - 1], true);
        }

        protected override bool MergeOrdinalThousands (int thousand)
        {
           return true;
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return "ésimo"; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return false; }
        }

        internal override void AppendHundred(StringBuilder builder, int value, int hundreds, bool isCardinal,
            bool hasHigherDigit, bool hasLowerDigit, bool isCardinalOriginal, List<int> thousands,
            int thousandIndex)
        {
            if (value < 20)
            {
                // WORDSNET-4984 Index out of range when converting 100.
                if (value > 0)
                    AppendWord(builder, GetNumeralFirst19(value, isCardinal), true);
            }
            else if ((value > 20) && (value < 30))
            {
                int lastDigit = value - 20;
                if (isCardinal)
                {
                    AppendWord(builder, gCardinalTwenties[lastDigit - 1], true);
                }
                else
                {
                    AppendWord(builder, GetNumeralTen(2, false) + Separator + GetNumeralFirst19(lastDigit, false), true);
                }

            }
            else // 20, 30-99
            {
                int tens = value/10;
                value -= 10*tens;

                if (value == 0)
                {
                    // 20, 30, 40, ..., 90
                    AppendWord(builder, GetNumeralTen(tens, isCardinal), true);
                }
                else
                {
                    AppendWord(builder,
                        GetNumeralTen(tens, isCardinal) +
                        ((isCardinal) ? SeparatorTensOnes : Separator) +
                        GetNumeralFirst19(value, isCardinal), true);
                }
            }
        }

        protected override string SeparatorTensOnes
        {
            get { return " y "; }
        }

        internal override string And
        {
            get { return "con"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return "cero";
        }

        protected override string[] CardinalFirst19
        {
            get { return gCardinalFirst19; }
        }

        protected override string[] CardinalTens
        {
            get { return gCardinalTens; }
        }

        internal override string GetBigNumeral(int thousandIndex, bool isCardinal, int thousand, List<int> thousands)
        {
            return gBigNumerals[thousandIndex - 1];
        }

        protected override string[] OrdinalFirst19
        {
            get { return gOrdinalFirst19; }
        }

        protected override string[] OrdinalTens
        {
            get { return gOrdinalTens; }
        }

        private static readonly string[] gCardinalFirst19 =
        {
            "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez",
            "once", "doce", "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "primero", "segundo", "tercero", "cuarto", "quinto", "sexto", "séptimo", "octavo", "noveno", "décimo", "undécimo",
            "duodécimo", "decimotercero", "decimocuarto", "decimoquinto", "decimosexto", "decimoséptimo", "decimoctavo", "decimonoveno"
        };

        private static readonly string[] gCardinalTens =
        {
            "veinte", "treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa"
        };

        private static readonly string[] gCardinalTwenties =
        {
            "veintiuno", "veintidós", "veintitrés", "veinticuatro", "veinticinco", "veintiséis", "veintisiete", "veintiocho", "veintinueve"
        };

        private static readonly string[] gOrdinalTens =
        {
            "vigésimo", "trigésimo", "cuadragésimo", "quincuagésimo", "sexagésimo", "septuagésimo", "octogésimo", "nonagésimo"
        };

        private static readonly string[] gCardinalHundreds =
        {
            "ciento", "doscientos", "trescientos", "cuatrocientos", "quinientos", "seiscientos", "setecientos", "ochocientos", "novecientos"
        };

        private static readonly string[] gOrdinalHundreds =
        {
            "centésimo", "ducentésimo", "tricentésimo", "cuadringentésimo", "quingentésimo", "sexcentésimo", "septingentésimo", "octingésimo", "noningentésimo"
        };

        private static readonly string[] gBigNumerals = { "mil", "millón", "mil millones" };
    }
}
