// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Alexey Butalov

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Arabic style numbering (1., 1.1.2.)
    /// </summary>
    internal class ArabicDotTxtNumberingStyle : TxtNumberingStyle
    {
        internal ArabicDotTxtNumberingStyle()
            : base(false, true, NumberStyle.Arabic)
        {
            // WORDSNET-10612 We should take in account list items with entirely empty list label.
            // (?!.) was added to the regex pattern to check it.
            // WORDSNET-10993 To correctly recognize trailing '.' we need to find all digits, including leading '0'. So, in regex pattern ^[1-9]\d* was replaced with ^\d+
            // But as '0' can not start a list, we should check it additionally.
            mRegex = new Regex(@"^\d+(\.|\s|(?!.))", RegexOptions.Compiled);
        }

        internal override bool IsStartNumber(string value)
        {
            return value == "1";
        }

        internal override string GetNextNumber(string prevNumber)
        {
            int number = FormatterPal.TryParseInt(prevNumber);
            return number == int.MinValue
                       ? ""
                       : (number + 1).ToString();
        }

        internal override TxtNumberingInfo DetectNumbering(string text)
        {
            // Resilience for text, which is consists from a single integer number. For example see test file for WORDSNET-9958
            if (FormatterPal.IsInteger(text))
                return null;

            if ((text.Length > 0) && text[0] == '0')
                return null;

            List<string> numberList = new List<string>();
            int length = 0;
            string textBuffer = text;
            char trailingListNumChar = '\0';
            while (true)
            {
                Match match = mRegex.Match(textBuffer);
                if (!match.Success)
                    break;

                // WORDSNET-10612 If there is no '.' or whitespace after list number,
                // then whole 'match' is a value of list number part, otherwise we should strip last '.' or whitespace.
                trailingListNumChar = match.Value[match.Value.Length - 1];
                string value = StringUtil.IsDigit(trailingListNumChar) ? match.Value : match.Value.Substring(0, match.Value.Length-1);
                numberList.Add(value);

                length += match.Value.Length;
                textBuffer = textBuffer.Remove(0, match.Length);

                if (StringUtil.Contains(match.Value, " ", true))
                    break;
            }

            if (numberList.Count == 0)
                return null;

            // WORDSNET-10993 Don't make list item if we are not detecting numbering with whitespaces, but list number ends with whitespace.
            // Not allowed trailing whitespace is the same as required dot in our design.
            if (!DetectNumberingWithWhitespaces && (trailingListNumChar != '.'))
                return null;

            string[] numbers = new string[numberList.Count];
            for (int i = 0; i < numberList.Count; i++)
                numbers[i] = numberList[i];
            return new TxtNumberingInfo(numbers, text.Substring(0, length).Trim());
        }

        internal override string GetNumberFormat(int level)
        {
            string format = "";
            for (int i = 0; i <= level; i++)
                format += Convert.ToChar(i) + ".";
            return format;
        }

        private readonly Regex mRegex;
    }
}
