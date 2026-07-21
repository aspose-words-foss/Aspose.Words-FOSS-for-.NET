// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2012 by Konstantin Kornilov

using System;
using System.Collections;
using System.Collections.Generic;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Bidi
{
    /// <summary>
    /// Enumerates UTF-32 code points in <see cref="string"/> instance.
    /// </summary>
    public sealed class StringUtf32Enumerator : IEnumerable<int>, IEnumerator<int>
    {
        public StringUtf32Enumerator(string text)
        {
            mNextCharPosition = 0;
            mCurrentOffset = 0;
            mText = text ?? string.Empty;
            mCurrentChar = '\0';
        }

        public IEnumerator<int> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (mNextCharPosition >= mText.Length)
            {
                return false;
            }

            char currentChar = mText[mNextCharPosition];
            mCurrentOffset = mNextCharPosition;
            ++mNextCharPosition;

            if (UnicodeUtil.IsLeadSurrogate(currentChar) &&
                (mNextCharPosition < mText.Length) &&
                UnicodeUtil.IsTailSurrogate(mText[mNextCharPosition]))
            {
                mCurrentChar = UnicodeUtil.ConvertToUtf32(currentChar, mText[mNextCharPosition]);
                ++mNextCharPosition;
            }
            else
            {
                mCurrentChar = currentChar;
            }

            return true;
        }

        public void Reset()
        {
            mNextCharPosition = 0;
            mCurrentOffset = 0;
        }

        public void Dispose()
        {
            // Nothing to free. But you should call this Dispose if some code will be added here.
        }

        public void SetText(string text)
        {
            mText = (text == null)
                ? string.Empty
                : text;
            Reset();
        }

        /// <summary>
        /// Returns string length in UTF-32 code units.
        /// </summary>
        // kvk:  GetStringLength result depends on enumerator implementation.
        //       If we implement another enumerator then it is better to keep this method in enumerator class instead of
        //       having two different methods in UnicodeUtil class.
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)] // replaced ICU method call
        public static int GetStringLength([CodePorting.Translator.Cs2Cpp.CppArgumentKind(ArgumentKind.ConstReference)] string text)
        {
            int length = 0;
            StringUtf32Enumerator stringUtf32Enumerator = new StringUtf32Enumerator(text);

            while (stringUtf32Enumerator.MoveNext())
                length++;

            return length;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Current
        {
            get
            {
                if (mNextCharPosition == 0)
                    throw new InvalidOperationException("Enumerator has not been started.");
                return mCurrentChar;
            }
        }

        /// <summary>
        /// Gets the index of the current character in the source string.
        /// </summary>
        public int Offset
        {
            get { return mCurrentOffset; }
        }
        object IEnumerator.Current
        {
            get { return Current; }
        }

        private string mText;
        private int mNextCharPosition;
        private int mCurrentChar;
        private int mCurrentOffset;
    }
}
