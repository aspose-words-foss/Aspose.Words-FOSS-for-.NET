// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/05/2013 by Denis Darkin

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// Ordinal/cardinal textual representation for Portuguese language.
    /// </summary>
    internal class PortugueseTextualNumber : EuropeanTextualNumberProvider
    {
        internal override void AppendHundreds(StringBuilder builder, int hundreds, bool isCardinal, bool isNonZeroHundreds,
            bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal,
            List<int> thousands, int thousandIndex)
        {
            Debug.Assert((hundreds >= 1) && (hundreds <= 9));

            if ((hundreds == 1) && !isNonZeroHundreds && isCardinal)
            {
                AppendWord(builder, "cem", true);
            }
            else
            {
                string cardinalNumber = gCardinalHundreds[hundreds - 1];
                AppendWord(builder, isCardinal ? cardinalNumber : gOrdinalHundreds[hundreds - 1], isCardinal);
            }
        }

        protected override string ThousandsOrdinalSuffix
        {
            get { return "ésimo"; }
        }

        protected override string  SeparatorTensOnes
        {
            get { return " e "; }
        }

        protected override string SeparatorHundredsTens
        {
            get { return " e "; }
        }

        internal override bool IncludeOneInBigNumbers
        {
            get { return false; }
        }

        internal override string And
        {
            get { return "e"; }
        }

        internal override string Zero(bool isCardinal)
        {
            return "zero";
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
            "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove", "dez",
            "onze", "doze", "treze", "catorze", "quinze", "dezasseis", "dezassete", "dezoito", "dezanove"
        };

        private static readonly string[] gOrdinalFirst19 =
        {
            "primeiro", "segundo", "terceiro", "quarto", "quinto", "sexto", "sétimo", "oitavo", "nono", "décimo", "décimo primeiro",
            "décimo segundo", "décimo terceiro", "décimo quarto", "décimo quinto", "décimo sexto", "décimo sétimo", "décimo oitavo", "décimo nono"
        };

        private static readonly string[] gCardinalTens =
        {
            "vinte", "trinta", "quarenta", "cinquenta", "sessenta", "setenta", "oitenta", "noventa"
        };

        private static readonly string[] gOrdinalTens =
        {
            "vigésimo", "trigésimo", "quadragésimo", "quinquagésimo", "sexagésimo", "septuagésimo", "octagésimo", "nonagésimo"
        };

        private static readonly string[] gCardinalHundreds =
        {
            "cento", "duzentos", "trezentos", "quatrocentos", "quinhentos", "seiscentos", "setecentos", "oitocentos", "novecentos"
        };

        private static readonly string[] gOrdinalHundreds =
        {
            "centésimo", "ducentésimo", "tricentésimo", "quadrigentésimo", "quingentésimo", "seiscentésimo", "septigentésimo", "octigentésimo", "nongentésimo"
        };

        private static readonly string[] gBigNumerals = { "mil", "milhão", "bilião" };
    }
}
