// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2012 by Alexey Butalov

using System;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Base class for letter styles
    /// </summary>
    internal class LetterTxtNumberingStyleBase : TxtNumberingStyle
    {
        internal LetterTxtNumberingStyleBase(char[] letterSet, char separator, NumberStyle numberStyle)
            : base(false, false, numberStyle)
        {
            if (letterSet == null) 
                throw new ArgumentNullException("letterSet");

            mLetterSet = letterSet;
            mSeparator = separator;
        }

        internal override bool IsStartNumber(string value)
        {
            return (value.Length != 0) && (value[0] == mLetterSet[0]);
        }

        internal override string GetNextNumber(string prevNumber)
        {
            int letterIndex = FindLetterIndex(prevNumber);
            return ((letterIndex == -1) || (letterIndex == mLetterSet.Length - 1))
                ? string.Empty
                : Convert.ToString(mLetterSet[letterIndex + 1]);
        }

        internal override TxtNumberingInfo DetectNumbering(string text)
        {
            if ((text.Length < 2) || ((text[1] != mSeparator)))
                return null;

            for (int i = 0; i < mLetterSet.Length; i++)
            {
                if (text[0] == mLetterSet[i])
                    return new TxtNumberingInfo(Convert.ToString(mLetterSet[i]), text.Substring(0, 2));
            }

            return null;
        }

        internal override string GetNumberFormat(int level)
        {
            Debug.Assert(level == 0, "Only level 0 is allowed!");
            return "\x0000" + mSeparator;
        }

        private int FindLetterIndex(string number)
        {
            if (number.Length != 1)
                return -1;

            for (int i = 0; i < mLetterSet.Length; i++)
            {
                if (mLetterSet[i] == number[0])
                    return i;
            }

            return -1;
        }

        private readonly char[] mLetterSet;
        private readonly char mSeparator;
    }
}
