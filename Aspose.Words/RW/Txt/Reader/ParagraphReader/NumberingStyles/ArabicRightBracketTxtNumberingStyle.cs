// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Alexey Butalov

using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Arabic style like 1), 2)
    /// </summary>
    internal class ArabicRightBracketTxtNumberingStyle : TxtNumberingStyle
    {
        internal ArabicRightBracketTxtNumberingStyle()
            : base(false, false, NumberStyle.Arabic)
        {
            mRegex = new Regex(@"^[1-9]\d*\)", RegexOptions.Compiled);
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
            Match match = mRegex.Match(text);
            if (!match.Success)
                return null;

            string value = match.Value.Substring(0, match.Value.Length - 1);
            return new TxtNumberingInfo(new string[] {value}, match.Value);
        }

        internal override string GetNumberFormat(int level)
        {
            Debug.Assert(level == 0, "Only level 0 is allowed!");
            return "\x0000)";
        }

        private readonly Regex mRegex;
    }
}