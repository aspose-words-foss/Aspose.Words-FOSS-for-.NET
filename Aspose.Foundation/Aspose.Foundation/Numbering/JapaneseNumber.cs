// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/03/2012 by Andrey Soldatov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its methods convert a number into several Japanese numbering styles.
    /// Some of these methods are used independently.
    /// </summary>
    public static class JapaneseNumber
    {
        internal static string ToAiueo(long value, bool isHalfWidth)
        {
            string[] numbers = isHalfWidth ? gAiueoHalfWidthNumbers : gAiueoNumbers;
            return (value == 0) ? "0" : numbers[(int)(value % numbers.Length)];
        }

        internal static string ToIroha(long value)
        {
            return (value == 0) ? "0" : gIrohaNumbers[(int)(value % gIrohaNumbers.Length)];
        }

        /// <summary>
        /// <para>Returns Japanese number ('一', '二', '三') written by digits.</para>
        /// <para>Each decimal digit is converted to one corresponding Japanese character.</para>
        /// <para>Returns "二二一" for 221 number, compare with <see cref="ToJapaneseCounting"/>.</para>
        /// <para>Zero characters are prepended before number to return at least <paramref name="minChars"/> characters.</para>
        /// </summary>
        public static string ToJapaneseDigital(long value, int minChars)
        {
            List<int> digits = GroupSplitter.SplitToGroups(value, 1);

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = minChars - digits.Count; i > 0; i--)
                stringBuilder.Append(gKanjiDigits[0]);

            for (int i = digits.Count - 1; i >= 0; i--)
                stringBuilder.Append(gKanjiDigits[digits[i]]);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// <para>Returns Japanese number ('一', '二', '三') written in words.</para>
        /// <para>Returns "二百二十一" for 221 number, compare with <see cref="ToJapaneseDigital"/>.</para>
        /// </summary>
        /// <remarks>http://en.wikipedia.org/wiki/Japanese_numerals</remarks>
        public static string ToJapaneseCounting(long value)
        {
            if (value < 10)
                return gKanjiDigits[(int)value];

            StringBuilder stringBuilder = new StringBuilder();

            // There is a special treatment of the highest '1' digit if a value >= 10000
            bool isHighestDigit = (value >= 10000);

            List<int> myriadGroups = GroupSplitter.SplitToGroups(value, 4);

            for (int myriadPos = myriadGroups.Count - 1; myriadPos >= 0; myriadPos--)
            {
                List<int> digits = GroupSplitter.SplitToGroups(myriadGroups[myriadPos], 1);

                for (int digitPos = digits.Count - 1; digitPos >= 0; digitPos--)
                {
                    int digit = digits[digitPos];

                    if (digit == 0)
                        continue;

                    // Only digits larger than one are written for KanjiDigit numbering.
                    // There are two exceptions when digit is written even if it is '1':
                    // 1. The lowest digit.
                    // 2. The highest digit, if Number >= 10000.
                    if ((digit > 1) || (digitPos == 0) || isHighestDigit)
                        stringBuilder.Append(gKanjiDigits[digit]);

                    // Order character is written after digit or without digit character(if the digit is 1).
                    stringBuilder.Append(gKanjiOrderChars[digitPos]);

                    isHighestDigit = false;
                }

                stringBuilder.Append(gKanjiMyriadChars[myriadPos]);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// These characters mean: '46', '1', '2', ..., '45'.
        /// </summary>
        private static readonly string[] gAiueoNumbers =
        {
            "ン", "ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ",
            "コ", "サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ",
            "ト", "ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ",
            "ホ", "マ", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ",
            "リ", "ル", "レ", "ロ", "ワ", "ヲ"
        };

        /// <summary>
        /// These characters mean: '46', '1', '2', ..., '45'.
        /// </summary>
        private static readonly string[] gAiueoHalfWidthNumbers =
        {
            "ﾝ", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ",
            "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ",
            "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ",
            "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ",
            "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ"
        };

        /// <summary>
        /// These characters mean: '48', '1', '2', ..., '47'.
        /// </summary>
        private static readonly string[] gIrohaNumbers =
        {
            "ン", "イ", "ロ", "ハ", "ニ", "ホ", "ヘ", "ト", "チ", "リ",
            "ヌ", "ル", "ヲ", "ワ", "カ", "ヨ", "タ", "レ", "ソ", "ツ",
            "ネ", "ナ", "ラ", "ム", "ウ", "ヰ", "ノ", "オ", "ク", "ヤ",
            "マ", "ケ", "フ", "コ", "エ", "テ", "ア", "サ", "キ", "ユ",
            "メ", "ミ", "シ", "ヱ", "ヒ", "モ", "セ", "ス"
        };

        /// <summary>
        /// These characters mean: '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'.
        /// </summary>
        private static readonly string[] gKanjiDigits = { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };

        /// <summary>
        /// These characters mean: "ten", "hundred", "thousand". Empty string when nothing to add.
        /// </summary>
        private static readonly string[] gKanjiOrderChars = { "", "十", "百", "千" };

        /// <summary>
        /// These characters mean: "ten thousand", "hundred million". Empty string when nothing to add.
        /// </summary>
        private static readonly string[] gKanjiMyriadChars = { "", "万", "億" };
    }
}
