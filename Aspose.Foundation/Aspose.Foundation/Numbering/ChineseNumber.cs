// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/11/2013 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its methods convert a number into several Chinese numbering styles.
    /// </summary>
    /// <remarks>See algorithms description in ISO29500 p1, 17.18.59 ST_NumberFormat (Numbering Format).</remarks>
    internal static class ChineseNumber
    {
        /// <summary>
        /// Returns Chinese Counting System number.
        /// </summary>
        internal static string ToChineseCountingSystem(long value, NumberStyleCore numberStyle)
        {
            Debug.Assert(value >= 0);

            switch (numberStyle)
            {
                case NumberStyleCore.TradChinNum2:
                case NumberStyleCore.TradChinNum3:
                case NumberStyleCore.SimpChinNum2:
                case NumberStyleCore.SimpChinNum3:
                    return ToChineseCountingThousand(value, numberStyle);
                case NumberStyleCore.SimpChinNum1:
                case NumberStyleCore.TradChinNum1:
                    return ToChineseCounting(value, numberStyle);
                default:
                    throw new ArgumentOutOfRangeException("numberStyle");
            }
        }

        /// <summary>
        /// Returns Chinese Counting Thousand number.
        /// </summary>
        private static string ToChineseCountingThousand(long value, NumberStyleCore numberStyle)
        {
            if (value == 0)
                return "";

            List<int> digits = GroupSplitter.SplitToGroups(value, 1);
            StringBuilder stringBuilder = new StringBuilder();

            bool isPrevZero = false;
            for (int digitPos = digits.Count - 1; digitPos >= 0; digitPos--)
            {
                int digit = digits[digitPos];

                // Don`t write any for zero, but writes special symbol later on first non-zero digit
                if (digit == 0)
                {
                    isPrevZero = true;
                    continue;
                }

                // Write special symbol, if previous digits was any number of consecutive zeros.
                if (isPrevZero)
                {
                    stringBuilder.Append(ZeroToSymbol(numberStyle));
                    isPrevZero = false;
                }

                // Write down the symbol, depending on current digit position in number.
                stringBuilder.Append(DigitToSymbolInThousandCountingSystem(digit, digitPos, value, numberStyle));

            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns Chinese Counting number.
        /// </summary>
        private static string ToChineseCounting(long value, NumberStyleCore numberStyle)
        {
            if (value == 0)
                return ZeroToSymbol(numberStyle).ToString();

            // Single digit. Simply convert it and return.
            if (value < 10)
                return DigitToSymbol((int)value, numberStyle).ToString();

            // 'Ten' has special symbol, process it separately.
            if (value == 10)
                return ChineseCountingTen.ToString();

            List<int> digits = GroupSplitter.SplitToGroups(value, 1);
            StringBuilder stringBuilder = new StringBuilder();

            if (value < 20)
            {
                // If (11 <= value <= 19), then we should start from the 'Chinese Ten'
                // and then append last digit.
                stringBuilder.Append(ChineseCountingTen);
                stringBuilder.Append(DigitToSymbol(digits[0], numberStyle));
            }
            else if (value < 100)
            {
                // If (20 <= value <= 99), then we should append number of tens,
                // then append 'Chinese Ten' and then append last digit, if it is not zero.
                stringBuilder.Append(DigitToSymbol(digits[1], numberStyle));
                stringBuilder.Append(ChineseCountingTen);
                if (digits[0] != 0)
                    stringBuilder.Append(DigitToSymbol(digits[0], numberStyle));
            }
            else
            {
                // For all other values we should take each digit,
                // convert it to the corresponding Chinese symbol and append to the resulting string.
                for (int digitPos = digits.Count - 1; digitPos >= 0; digitPos--)
                    stringBuilder.Append(DigitToSymbol(digits[digitPos], numberStyle));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns symbol, for digit in number at specified position for thousand counting system.
        /// <param name="digit">Digit at position in original number.</param>
        /// <param name="digitPos">Position in original number</param>
        /// <param name="originalValue">Original number</param>
        /// <param name="numberStyle">Style of the counting system for which to convert.</param>
        /// </summary>
        private static string DigitToSymbolInThousandCountingSystem(int digit, int digitPos, long originalValue, NumberStyleCore numberStyle)
        {
            StringBuilder stringBuilder = new StringBuilder();

            //  In Thousand Counting Systems for numbers less then 20, we don`t write number of tens (symbol 'one')
            if (((numberStyle != NumberStyleCore.TradChinNum3) && (numberStyle != NumberStyleCore.SimpChinNum3)) || (originalValue >= 20) || (digitPos != 1))
                stringBuilder.Append(DigitToSymbol(digit, numberStyle));

            // Length of the groups (-1) for Counting System
            const int groupLength = 4;

            // Write down the symbol, depending on the current digit position in the group and numbering style.
            int powPos = digitPos % groupLength;
            if (powPos != 0)
                stringBuilder.Append(PowToSymbol(powPos, numberStyle));

            // Write down symbol 'ten thousand', if value >= 100000.
            stringBuilder.Append(TenThousandToSymbol(numberStyle), digitPos / groupLength);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Returns symbol, which mean digit 1-9 for the specified counting system.
        /// <param name="digit">Digit to convert.</param>
        /// <param name="numberStyle">Style of the counting system for which to convert.</param>
        /// </summary>
        private static char DigitToSymbol(int digit, NumberStyleCore numberStyle)
        {
            if (digit == 0)
                return ZeroToSymbol(numberStyle);

            char symbol;
            switch (numberStyle)
            {
                case NumberStyleCore.TradChinNum2:
                    symbol = gIdeographLegalTraditionalDigits[0][--digit];
                    break;
                case NumberStyleCore.SimpChinNum2:
                    symbol = gIdeographLegalSimplifiedDigits[0][--digit];
                    break;
                case NumberStyleCore.TradChinNum3:
                case NumberStyleCore.SimpChinNum3:
                case NumberStyleCore.TradChinNum1:
                case NumberStyleCore.SimpChinNum1:
                    symbol = gCountingThousandDigits[0][--digit];
                    break;
                default:
                    throw new ArgumentOutOfRangeException("numberStyle");
            }

            return symbol;
        }

        /// <summary>
        /// Returns symbol, which mean some power of ten for the specified counting system (10, 100, 1000, 10000).
        /// <param name="power">Power of ten.</param>
        /// <param name="numberStyle">Style of the counting system for which to convert.</param>
        /// </summary>
        private static char PowToSymbol(int power, NumberStyleCore numberStyle)
        {
            if (power == 4)
                return TenThousandToSymbol(numberStyle);

            char powerSymbol;
            switch (numberStyle)
            {
                case NumberStyleCore.TradChinNum2:
                    powerSymbol = gIdeographLegalTraditionalDigits[1][--power];
                    break;
                case NumberStyleCore.SimpChinNum2:
                    powerSymbol = gIdeographLegalSimplifiedDigits[1][--power];
                    break;
                case NumberStyleCore.TradChinNum3:
                case NumberStyleCore.SimpChinNum3:
                    powerSymbol = gCountingThousandDigits[1][--power];
                    break;
                default:
                    throw new ArgumentOutOfRangeException("numberStyle");
            }

            return powerSymbol;
        }

        /// <summary>
        /// Returns symbol, which mean '10 000' for specified counting system.
        /// <param name="numberStyle">Style of the counting system for which to convert.</param>
        /// </summary>
        private static char TenThousandToSymbol(NumberStyleCore numberStyle)
        {
            char tenThousand;
            switch (numberStyle)
            {
                // Ideograph Legal Traditional 'ten thosand' symbol is same as in Taiwanese Counting Thousand System
                case NumberStyleCore.TradChinNum2:
                case NumberStyleCore.TradChinNum3:
                    tenThousand = TaiwaneseCountingThousandTenThousand;
                    break;
                case NumberStyleCore.SimpChinNum2:
                case NumberStyleCore.SimpChinNum3:
                    tenThousand = ChineseCountingThousandTenThousand;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("numberStyle");
            }
            return tenThousand;
        }

        /// <summary>
        /// Returns symbol, which mean any sequence of zeroes for the specified counting system.
        /// <param name="numberStyle">Style of the counting system for which to convert.</param>
        /// </summary>
        private static char ZeroToSymbol(NumberStyleCore numberStyle)
        {
            char zero;
            switch (numberStyle)
            {
                // Ideograph Legal Traditional 'zero' symbol is the same as in
                // Taiwanese Counting Thousand System and Chinese Legal Simplified Format
                case NumberStyleCore.TradChinNum2:
                case NumberStyleCore.TradChinNum3:
                case NumberStyleCore.SimpChinNum2:
                    zero = TaiwaneseCountingThousandZero;
                    break;
                case NumberStyleCore.SimpChinNum3:
                    zero = ChineseCountingThousandZero;
                    break;
                case NumberStyleCore.TradChinNum1:
                case NumberStyleCore.SimpChinNum1:
                    zero = ChineseCountingZero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("numberStyle");
            }
            return zero;
        }

        /// <summary>
        /// This is only specifical part of Ideograph Legal Traditional Counting System
        /// These characters mean:
        /// 1, 2, 3, 4, 5, 6, 7, 8, 9;
        /// 10, 100, 1000;
        /// </summary>
        private static readonly char[][] gIdeographLegalTraditionalDigits =
        {
            new char[] { '壹', '貳', '參', '肆', '伍', '陸', '柒', '捌', '玖' },
            new char[] { '拾', '佰', '仟' }
        };

        /// <summary>
        /// This is only specifical part of Ideograph Chinese Legal Simplified Counting System
        /// These characters mean:
        /// 1, 2, 3, 4, 5, 6, 7, 8, 9;
        /// 10, 100, 1000;
        /// </summary>
        private static readonly char[][] gIdeographLegalSimplifiedDigits =
        {
            new char[] { '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖' },
            new char[] { '拾', '佰', '仟' }
        };

        /// <summary>
        /// This is only common part of different Counting Thousand Systems
        /// These characters mean:
        /// 1, 2, 3, 4, 5, 6, 7, 8, 9;
        /// 10, 100, 1000;
        /// </summary>
        private static readonly char[][] gCountingThousandDigits =
        {
            new char[] { '一', '二', '三', '四', '五', '六', '七', '八', '九' },
            new char[] { '十', '百', '千' }
        };

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char TaiwaneseCountingThousandTenThousand = '萬';
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ChineseCountingThousandTenThousand = '万';

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char TaiwaneseCountingThousandZero = '零';
        // The standard states for the chineseCountingThousand enumeration value, if the sequence number is between 10,000 and 100,000
        // and no groups are formed, and the number is not a mulitple of 1,000, the symbol U+96F6 (零) should be written.
        // Word does not write out the symbol 零, but writes symbol '〇' instead.
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ChineseCountingThousandZero = '〇';

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ChineseCountingZero = '○';
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ChineseCountingTen = '十';
    }
}
