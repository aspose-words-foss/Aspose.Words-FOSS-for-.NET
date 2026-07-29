// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Alexey Butalov

using System;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Information about the numbering: list of numbers and text.
    /// </summary>
    internal class TxtNumberingInfo
    {
        internal TxtNumberingInfo(string[] numbers, string text)
        {
            if (numbers == null) 
                throw new ArgumentNullException("numbers");
            Debug.Assert(StringUtil.HasChars(text));

            mNumbers = numbers;
            mText = text;
        }

        internal TxtNumberingInfo(string number, string text)
        {
            Debug.Assert(StringUtil.HasChars(number));
            Debug.Assert(StringUtil.HasChars(text));

            mNumbers = new string[1];
            mNumbers[0] = number;
            mText = text;
        }

        /// <summary>
        /// Array of numbers
        /// </summary>
        internal string[] Numbers
        {
            get { return mNumbers; }
        }

        /// <summary>
        /// Text of numbering part
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        private readonly string[] mNumbers;
        private readonly string mText;
    }
}
