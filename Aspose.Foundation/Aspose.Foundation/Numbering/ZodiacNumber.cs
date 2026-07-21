// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2016 by Ilya Navrotskiy

using Aspose.Common;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its methods convert a number into several Zodiac numbering styles.
    /// </summary>
    /// <remarks>See algorithms description in ISO29500 p1, 17.18.59 ST_NumberFormat (Numbering Format).</remarks>
    internal static class ZodiacNumber
    {
        /// <summary>
        /// Returns Zodiac1 (Traditional Ideograph Format) number.
        /// </summary>
        internal static string ToZodiac1(long value)
        {
            if ((value > 0) && (value < 11))
                return gIdeographTraditional[(int)value - 1].ToString();

            return FormatterPal.IntToStr(value);
        }

        /// <summary>
        /// Returns Zodiac2 (Zodiac Ideograph Format) number.
        /// </summary>
        internal static string ToZodiac2(long value)
        {
            if ((value > 0) && (value < 13))
                return gZodiacIdeograph[(int)value - 1].ToString();

            return FormatterPal.IntToStr(value);
        }

        /// <summary>
        /// Returns Zodiac3 (Traditional Zodiac Ideograph Format) number.
        /// </summary>
        internal static string ToZodiac3(long value)
        {
            if (value < 1)
                return FormatterPal.IntToStr(value);

            // This system uses a set of character pairs to represent numbers 1-60.
            // For values greater than the size of the set it repeatedly subtract the size of the set (60)
            // from the value until the result is equal to or less than the size of the set.
            int charPair = (int)(value % 60);
            if (charPair == 0)
                charPair = 60;

            // First digit in pair is number from Ideograph Traditional Format.
            int firstDigit = charPair % 10;
            if (firstDigit == 0)
                firstDigit = 10;

            // Second digit in pair is number from Zodiac Ideograph Format.
            int secondDigit = charPair % 12;
            if (secondDigit == 0)
                secondDigit = 12;

            return string.Format("{0}{1}", gIdeographTraditional[firstDigit - 1], gZodiacIdeograph[secondDigit - 1]);
        }

        /// <summary>
        /// These are characters of Ideograph Traditional Format.
        /// They mean digits 1-10.
        /// </summary>
        private static readonly char[] gIdeographTraditional = {'甲', '乙', '丙', '丁', '戊', '己', '庚', '辛', '壬', '癸'};

        /// <summary>
        /// These are characters of Zodiac Ideograph Format.
        /// They mean digits 1-12.
        /// </summary>
        private static readonly char[] gZodiacIdeograph = { '子', '丑', '寅', '卯', '辰', '巳', '午', '未', '申', '酉', '戍', '亥' };
    }
}
