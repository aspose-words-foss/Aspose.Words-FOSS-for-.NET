// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/05/2021 by Alexey Maslov

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Txt.Reader
{
    internal class RomanTxtNumberingStyleBase : TxtNumberingStyle
    {
        internal RomanTxtNumberingStyleBase(char[] letterSet, char separator, NumberStyle numberStyle)
            : base(false, false, numberStyle)
        {
            if (letterSet == null)
                throw new ArgumentNullException("letterSet");

            mLetterSet = letterSet;
            mSeparator = separator;
            mNumberStyle = numberStyle;

            switch (numberStyle)
            {
                case NumberStyle.LowercaseRoman:
                    mRegex = new Regex(@"(?=[mdclxvi])m*(c[md]|d?c{0,3})(x[cl]|l?x{0,3})(i[xv]|v?i{0,3})", RegexOptions.Compiled);
                    break;
                case NumberStyle.UppercaseRoman:
                    mRegex = new Regex(@"(?=[MDCLXVI])M*(C[MD]|D?C{0,3})(X[CL]|L?X{0,3})(I[XV]|V?I{0,3})", RegexOptions.Compiled);
                    break;
                default:
                    throw new ArgumentException("numberStyle");
            }
        }

        internal override TxtNumberingInfo DetectNumbering(string text)
        {
            if (text.Length == 0)
                return null;

            int index;

            StringBuilder sb = new StringBuilder();
            for (index = 0; index < text.Length; index++)
            {
                char c = text[index];
                if (c == mSeparator)
                    return GetNumberingInfo(text, index, sb.ToString());

                if (Array.IndexOf(mLetterSet, c) == -1)
                    return null;

                sb.Append(c);
            }

            return null;
        }

        private TxtNumberingInfo GetNumberingInfo(string text, int separatorIndex, string detectedNumber)
        {
            if (!IsValidRomanNumber(detectedNumber))
                return null;

            string textNumbering = text.Substring(0, separatorIndex + 1);

            return new TxtNumberingInfo(detectedNumber, textNumbering.Trim());
        }

        internal override string GetNextNumber(string prevNumber)
        {
            return IsValidRomanNumber(prevNumber) ? GetNextRomanNumber(prevNumber) : String.Empty;
        }

        internal override string GetNumberFormat(int level)
        {
            Debug.Assert(level == 0, "Only level 0 is allowed!");
            return "\x0000" + mSeparator;
        }

        internal override bool IsStartNumber(string value)
        {
            return (value.Length != 0) && (value[0] == mLetterSet[0]);
        }

        private bool IsValidRomanNumber(string textBuffer)
        {
            return mRegex.IsMatch(textBuffer);
        }

        private string GetNextRomanNumber(string prevNumber)
        {
            return ConvertArabicToRoman(ConvertRomanToArabic(prevNumber) + 1);
        }

        private static int ConvertRomanToArabic(string roman)
        {
            Dictionary<char, int> RomanDictionary
                = new Dictionary<char, int>{
                                                 {'I', 1},
                                                 {'V', 5},
                                                 {'X', 10},
                                                 {'L', 50},
                                                 {'C', 100},
                                                 {'D', 500},
                                                 {'M', 1000}
                                            };

            roman = roman.ToUpper();

            int total = 0;
            int minus = 0;

            for (int i = 0; i < roman.Length; i++)
            {
                int thisNumeral = RomanDictionary[roman[i]] - minus;
                
                if (i >= roman.Length - 1 || thisNumeral + minus >= RomanDictionary[roman[i + 1]])
                {
                    total += thisNumeral;
                    minus = 0;
                }
                else
                {
                    minus = thisNumeral;
                }
            }

            return total;
        }

        private string ConvertArabicToRoman(int arabic)
        {
            int[] arabicNums = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            string[] romanNums = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < arabicNums.Length && arabic != 0; i++)
            {
                while (arabic >= arabicNums[i])
                {
                    arabic -= arabicNums[i];
                    stringBuilder.Append(romanNums[i]);
                }
            }

            return mNumberStyle == NumberStyle.UppercaseRoman ? stringBuilder.ToString() : stringBuilder.ToString().ToLower();
        }

        private readonly char[] mLetterSet;
        private readonly char mSeparator;
        private readonly Regex mRegex;
        private readonly NumberStyle mNumberStyle;
    }
}
