// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2012 by Alexey Butalov

using System;
using Aspose.Words.Loading;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Detects numbering or bullet of a text line.
    /// </summary>
    internal static class TxtNumberingDetector
    {
        /// <summary>
        /// Parses string for numbering or bullet.
        /// </summary>
        /// <param name="text">Source string</param>
        /// <param name="detectNumberingWithWhitespaces"></param>
        /// <returns>Numbering info if found; otherwise null.</returns>
        /// <remarks>
        /// The method is replaced with <see cref="Detect(string, TxtLoadOptions)"/>
        /// and remained just for backward compatibility in tests.
        /// </remarks>
        internal static TxtNumbering Detect(string text, bool detectNumberingWithWhitespaces)
        {
            TxtLoadOptions loadOptions = new TxtLoadOptions(){DetectNumberingWithWhitespaces = detectNumberingWithWhitespaces};
            return Detect(text, loadOptions);
        }

        /// <summary>
        /// Parses string for numbering or bullet.
        /// </summary>
        /// <param name="text">Source string</param>
        /// <param name="loadOptions"> The load options.</param>
        /// <returns>Numbering info if found; otherwise null.</returns>
        internal static TxtNumbering Detect(string text, TxtLoadOptions loadOptions)
        {
            if (!loadOptions.AutoNumberingDetection)
                return null;

            Debug.Assert(!text.StartsWith(" ", StringComparison.Ordinal), "Leading spaces should be removed.");

            // WORDSNET-10993 For ArabicDotTxtNumberingStyle it is allowed to control the way of lists recognition.
            gNumberingStyles[0].DetectNumberingWithWhitespaces = loadOptions.DetectNumberingWithWhitespaces;

            foreach (TxtNumberingStyle numberingStyle in gNumberingStyles)
            {
                TxtNumberingInfo numberingInfo = numberingStyle.DetectNumbering(text);
                if ((numberingInfo != null) &&
                    (numberingInfo.Numbers.Length <= NumberingMaxLevelCount) &&
                    LetterOrRomanNumberingConform(numberingStyle, numberingInfo)) //So as not to confuse Roman and Letter
                {
                    return new TxtNumbering(numberingStyle, numberingInfo.Numbers, numberingInfo.Text);
                }
            }

            return null;
        }

        private static bool LetterOrRomanNumberingConform(TxtNumberingStyle curStyle, TxtNumberingInfo curNumbering)
        {
            bool retValue = true;

            char curNumberingTextChar = char.ToLower(curNumbering.Text[0]);

            if ((curStyle.NumberStyle == NumberStyle.LowercaseRoman || curStyle.NumberStyle == NumberStyle.UppercaseRoman) &&
                Array.IndexOf(TxtLettersHelper.gLowercaseRomanNumbers, curNumberingTextChar) != -1)
            {
                    retValue = (char)(curNumberingTextChar - 1) != mPrevNumberingTextChar;
            }

            if(retValue)
                mPrevNumberingTextChar = curNumberingTextChar;

            return retValue;
        }

        private static char mPrevNumberingTextChar = ' ';

        private const int NumberingMaxLevelCount = 8;

        private static readonly TxtNumberingStyle[] gNumberingStyles;

        static TxtNumberingDetector()
        {
            gNumberingStyles = new TxtNumberingStyle[]
                                   {
                                       new ArabicDotTxtNumberingStyle(),
                                       new ArabicRightBracketTxtNumberingStyle(),
                                       new LowercaseRomanDotTxtNumberingStyle(),
                                       new UppercaseRomanDotTxtNumberingStyle(),
                                       new LowercaseRomanRightBracketTxtNumberingStyle(),
                                       new UppercaseRomanRightBracketTxtNumberingStyle(),
                                       new LowercaseLetterDotTxtNumberingStyle(),
                                       new UppercaseLetterDotTxtNumberingStyle(),
                                       new LowercaseLetterRightBracketTxtNumberingStyle(),
                                       new UppercaseLetterRightBracketTxtNumberingStyle(),
                                       new AsteriskTxtNumberingStyle(),
                                       new DashTxtNumberingStyle(),
                                       new CourierCircleTxtNumberingStyle(),
                                       new Bullet0X95TxtNumberingStyle()
                                   };
        }
    }
}
