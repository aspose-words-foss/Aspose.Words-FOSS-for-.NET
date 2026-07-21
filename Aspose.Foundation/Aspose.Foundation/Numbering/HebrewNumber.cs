// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2015 by Ilya Navrotskiy

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its methods convert a number into several Hebrew numbering styles.
    /// </summary>
    /// <remarks>See algorithms description in ISO29500 p1, 17.18.59 ST_NumberFormat (Numbering Format).</remarks>
    internal static class HebrewNumber
    {
        /// <summary>
        /// Returns a Hebrew1 number.
        /// </summary>
        internal static string ToHebrew1(long value)
        {
            if (value == 0)
                return "";

            int cycledValue = NumberConverterCore.GetCycledValue(value);

            List<int> digits = GroupSplitter.SplitToGroups(cycledValue, 1);
            StringBuilder stringBuilder = new StringBuilder();

            for (int digitPos = digits.Count - 1; digitPos >= 0; digitPos--)
            {
                // If the remainder is 15 or 16, we should replace them with the special strings and stop.
                string specialNumber = ProcessHebrew1SpecialNumbers(digits, digitPos);
                if (specialNumber != null)
                {
                    stringBuilder.Append(specialNumber);
                    return stringBuilder.ToString();
                }

                int digit = digits[digitPos];
                if (digit != 0)
                    stringBuilder.Append(gHebrew1Digits[digitPos][digit - 1]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns a Hebrew2 number.
        /// </summary>
        internal static string ToHebrew2(long value)
        {
            Debug.Assert(value >= 0);

            int cycledValue = NumberConverterCore.GetCycledValue(value);

            StringBuilder stringBuilder = new StringBuilder();

            int tavCount = cycledValue / Hebrew2Divisor;
            stringBuilder.Append(new string(TavChar, tavCount));

            int remainder = cycledValue % Hebrew2Divisor;
            if (remainder > 0)
                stringBuilder.Append(Hebrew2Numerals[remainder - 1]);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Checks that the remaining digits from the specified position
        /// form number '15' or '16' and returns corresponding string.
        /// </summary>
        /// <remarks>
        /// In accordance with the specification, numbers '15' and '16' have special representation.
        /// </remarks>
        /// <param name="digits">List of digits where search for special numbers.</param>
        /// <param name="digitPos">Position in list of digits from which start to search.</param>
        /// <returns>String designation for special numbers or 'null' if not found.</returns>
        private static string ProcessHebrew1SpecialNumbers(List<int> digits, int digitPos)
        {
            // Check that exactly two digits remained starting from the specified position.
            if (digitPos != 1)
                return null;

            // Check number is '15' or '16' and return corresponding string.
            int digit = digits[digitPos];
            if (digit == 1)
            {
                digit = digits[digitPos-1];

                // Digits form number '15'.
                if (digit == 5)
                    return "טו";

                // Digits form number '16'.
                if (digit == 6)
                    return "טז";
            }

            return null;
        }

        /// <summary>
        /// These characters mean:
        /// 1, 2, 3, 4, 5, 6, 7, 8, 9;
        /// 10, 20, 30, 40, 50, 60, 70, 80, 90;
        /// 100, 200, 300.
        /// </summary>
        private static readonly char[][] gHebrew1Digits =
        {
            new char[] { 'א', 'ב', 'ג', 'ד', 'ה', 'ו', 'ז', 'ח', 'ט' },
            new char[] { 'י', 'כ', 'ל', 'מ', 'נ', 'ס', 'ע', 'פ', 'צ' },
            new char[] { 'ק', 'ר', 'ש' }
        };

        /// <summary>
        /// This string represents Hebrew2 numbering alphabet.
        /// </summary>
        private const string Hebrew2Numerals = "אבגדהוזחטיכלמנסעפצקרש";

        /// <summary>
        /// Length of Hebrew2 numbering alphabet + 1 (Hebrew2Numerals.Length + 1).
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int Hebrew2Divisor = 22;

        /// <summary>
        /// Twenty-second and last letter in Hebrew2.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char TavChar = 'ת';
    }
}
