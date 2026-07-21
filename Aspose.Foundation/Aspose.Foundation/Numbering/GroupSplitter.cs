// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/06/2016 by Ivan Lyagin

using System.Collections.Generic;

namespace Aspose.Numbering
{
    /// <summary>
    /// Utility for splitting numbers into groups of digits.
    /// </summary>
    internal static class GroupSplitter
    {
        /// <summary>
        /// <para>Returns list of integers where each element consists of several digits of a given integer.</para>
        /// <para>The lowest digit has index of 0 in the list.</para>
        /// <para>For example, SplitToGroups(12345, 3) gives list: {345, 12}.</para>
        /// </summary>
        public static List<int> SplitToGroups(long value, int digitsInGroup)
        {
            Debug.Assert(value >= 0);
            List<int> digitsList = new List<int>(10);
            int divisor = DivisorByDigitNumber(digitsInGroup);

            do
            {
                digitsList.Add((int)(value % divisor));
                value /= divisor;
            }
            while (value > 0);

            return digitsList;
        }

        private static int DivisorByDigitNumber(int value)
        {
            switch (value)
            {
                case 1: return 10;
                case 3: return 1000;
                case 4: return 10000;
                case 5: return 100000;
                default:
                    Debug.Fail("For now, other values aren't used. Add them if needed.");
                    return 0;
            }
        }
    }
}
