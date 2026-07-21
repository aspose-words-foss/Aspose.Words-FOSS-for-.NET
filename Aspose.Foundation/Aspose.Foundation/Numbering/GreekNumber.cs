// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2020 by Vasiliy Stepchenko

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its method converts a number into Greek numbering style.
    /// </summary>
    /// <remarks>Algorithm is based on Word's behaviour and https://en.wikipedia.org/wiki/Greek_numerals page.</remarks>
    internal static class GreekNumber
    {
        /// <summary>
        /// Returns Greek counting system number.
        /// </summary>
        /// <param name="value">number to convert.</param>
        /// <returns>String representation of the given number.</returns>
        internal static string ToGreek(long value)
        {
            if (value < 1)
                return string.Empty;

            //Replicate Word's behaviour. After 9999 Word starts numeration from scratch.
            while (value > 9999)
                value -= 9999;

            List<int> groups = GroupSplitter.SplitToGroups(value, 1);
            StringBuilder sb = new StringBuilder();

            for (int exponent = groups.Count-1; exponent >= 0; exponent--)
            {
                int groupValue = groups[exponent];
                if (groupValue > 0)
                {
                    sb.Append(GetDigit(groupValue, exponent));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns string representation of the number.
        /// The number is specified by two parameters: digit value and exponent. In other words number = value*10^exponent.
        /// </summary>
        /// <param name="value">Digit value of the number.</param>
        /// <param name="exponent">Exponent of the number.</param>
        /// <returns>String representation of the number.</returns>
        private static string GetDigit(int value, int exponent)
        {
            switch (exponent)
            {
                case 3:
                    return GreekThousandSign + GetDigit(value, 0);
                case 2:
                    return Hundreds[value - 1].ToString();
                case 1:
                    return Tens[value - 1].ToString();
                case 0:
                    return (value == 6) ? GreekSix : Digits[value - 1].ToString();
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected exponent: '{0}'", exponent));
            }
        }

        private const string Digits = "αβγδε?ζηθ";
        private const string Tens = "ικλμνξοπϟ";
        private const string Hundreds = "ρστυφχψωϡ";
        private const string GreekThousandSign = ",";
        private const string GreekSix = "στ";
    }
}
