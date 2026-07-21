// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2012 by Daria

using System;
using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a parameter of function in a field.
    /// </summary>
    internal class FunctionParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionParameter"/> class with the specified parameter value.
        /// </summary>
        /// <param name="parameterValue">A parameter value.</param>
        public FunctionParameter(string parameterValue)
        {
            Initialize(parameterValue);
        }

        /// <summary>
        /// Gets a value indicating whether this parameter is preceded by a space.
        /// </summary>
        public bool IsSpaceBefore
        {
            get { return (Peculiarities & ParameterPeculiarities.SpaceBefore) == ParameterPeculiarities.SpaceBefore; }
        }

        /// <summary>
        /// Gets a value indicating whether this parameter is followed by a space.
        /// </summary>
        public bool IsSpaceAfter
        {
            get { return (Peculiarities & ParameterPeculiarities.SpaceAfter) == ParameterPeculiarities.SpaceAfter; }
        }

        /// <summary>
        /// Gets a value indicating whether this parameter is a radix-10 number: in additional to digits
        /// contains only negative sign, currency and/or fraction chars.
        /// </summary>
        public bool IsNumber
        {
            get { return (Peculiarities & ParameterPeculiarities.Number) == ParameterPeculiarities.Number; }
        }

        /// <summary>
        /// Gets a value indicating whether this parameter is a fractional number.
        /// </summary>
        public bool IsFractional
        {
            get { return (Peculiarities & ParameterPeculiarities.Fractional) == ParameterPeculiarities.Fractional; }
        }

        /// <summary>
        /// Gets a value indicating whether the parameter value is a negative number.
        /// </summary>
        public bool IsNegative
        {
            get { return (Peculiarities & ParameterPeculiarities.Negative) == ParameterPeculiarities.Negative; }
        }

        /// <summary>
        /// Gets a value indicating whether a currency sign appears at the beginning of parameter.
        /// </summary>
        public bool IsCurrencyAtStart
        {
            get { return (Peculiarities & ParameterPeculiarities.CurrencyAtStart) == ParameterPeculiarities.CurrencyAtStart; }
        }

        /// <summary>
        /// Gets a value indicating whether a currency sign appears at the end of parameter.
        /// </summary>
        public bool IsCurrencyAtEnd
        {
            get { return (Peculiarities & ParameterPeculiarities.CurrencyAtEnd) == ParameterPeculiarities.CurrencyAtEnd; }
        }

        /// <summary>
        /// Gets a value indicating whether the parameter contains one of supported Word expression operators.
        /// </summary>
        public bool ContainsOperator
        {
            get { return (Peculiarities & ParameterPeculiarities.ContainsOperator) == ParameterPeculiarities.ContainsOperator; }
        }

        /// <summary>
        /// Gets the length of integer part of number.
        /// </summary>
        public int LengthOfIntegerPart { get; private set; }

        /// <summary>
        /// Gets the number of digits at start.
        /// </summary>
        public int NumberOfDigitsAtStart { get; private set; }

        /// <summary>
        /// Gets or sets a string representing parameter.
        /// </summary>
        public string ParameterValue { get; private set; }

        internal void Initialize(string value)
        {
            Peculiarities = ParameterPeculiarities.None;
            LengthOfIntegerPart = 0;
            NumberOfDigitsAtStart = 0;

            if (!StringUtil.HasChars(value))
            {
                ParameterValue = string.Empty;
                return;
            }

            if (StringUtil.IsWhiteSpace(value[0]))
            {
                Peculiarities |= ParameterPeculiarities.SpaceBefore;
            }

            if (StringUtil.IsWhiteSpace(value[value.Length - 1]))
            {
                Peculiarities |= ParameterPeculiarities.SpaceAfter;
            }

            ParameterValue = value.Trim();
            if (ParameterValue.Length < 1)
            {
                Peculiarities = ParameterPeculiarities.None;
                return;
            }

            int position = 0;
            if (ParameterValue[position] == '-')
            {
                if (ParameterValue.Length < 2)
                {
                    Peculiarities = ParameterPeculiarities.None;
                    return;
                }

                Peculiarities |= ParameterPeculiarities.Negative;
                position++;
            }

            string currency = FormatterPal.GetCurrencySymbolCurrent();
            if ((position + currency.Length <= ParameterValue.Length) && ParameterValue.Substring(position, currency.Length).Equals(currency, StringComparison.Ordinal))
            {
                position += currency.Length;
                if (position == ParameterValue.Length)
                {
                    Peculiarities = ParameterPeculiarities.None;
                    return;
                }

                Peculiarities |= ParameterPeculiarities.CurrencyAtStart;
            }

            char decimalSeparator = FormatterPal.GetDecimalSeparatorCurrent();
            int firstDigitPosition = position;

            foreach (char c in ParameterValue)
            {
                if (!char.IsDigit(c))
                    break;

                NumberOfDigitsAtStart++;
            }

            if (ParameterValue.IndexOfAny(gOperators, position, ParameterValue.Length - position) != -1)
            {
                Peculiarities |= ParameterPeculiarities.ContainsOperator;
                return;
            }

            for (; position < ParameterValue.Length; position++)
            {
                if (!char.IsDigit(ParameterValue, position))
                {
                    if (!IsFractional && ParameterValue[position] == decimalSeparator)
                    {
                        Peculiarities |= ParameterPeculiarities.Fractional;
                        LengthOfIntegerPart = position - firstDigitPosition;
                        // check for cases: "$.", ".€" - when parameter contains no digit.
                        if (LengthOfIntegerPart == 0 &&
                            (position == ParameterValue.Length - 1 || !char.IsDigit(ParameterValue, position + 1)))
                        {
                            Peculiarities = ParameterPeculiarities.None;
                            return;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (!IsFractional)
            {
                LengthOfIntegerPart = position - firstDigitPosition;
            }

            if (position < ParameterValue.Length)
            {
                if (!IsCurrencyAtStart &&
                    ParameterValue.Length - position == currency.Length &&
                    ParameterValue.Substring(position).Equals(currency, StringComparison.Ordinal))
                {
                    Peculiarities |= ParameterPeculiarities.CurrencyAtEnd;
                }
                else
                {
                    Peculiarities = ParameterPeculiarities.None;
                    LengthOfIntegerPart = 0;
                    return;
                }
            }

            Peculiarities |= ParameterPeculiarities.Number;
        }

        /// <summary>
        /// Gets a parameter peculiarities.
        /// </summary>
        internal ParameterPeculiarities Peculiarities { get; private set; }

        private static readonly char[] gOperators = {'+', '-', '*', '/', '%', '^', '=', '>', '<'};
    }
}
