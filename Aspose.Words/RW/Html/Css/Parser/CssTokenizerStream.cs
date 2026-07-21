// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2019 by Victor Chebotok

using System;
using Aspose.Bidi;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Provides access to a CSS text with consume/reconsume operations, look-ahead buffer, and character pre-processing,
    /// as required by <see cref="CssTokenizer"/>.
    /// </summary>
    internal class CssTokenizerStream
    {
        /// <summary>
        /// Constructor. Creates a new instance of the stream over the specified CSS text.
        /// </summary>
        internal CssTokenizerStream(string css)
        {
            mCss = css;

            // Fill up the look-ahead buffer.
            mPositionInBuffer = ReconsumeBufferDepth;
            for (int i = mPositionInBuffer + 1; i < mBuffer.Length; i++)
            {
                mBuffer[i] = ReadNextChar();
            }
        }

        /// <summary>
        /// Consumes another character (steps forward).
        /// </summary>
        internal bool ConsumeChar()
        {
            if (mPositionInBuffer < ReconsumeBufferDepth)
            {
                ++mPositionInBuffer;
            }
            else
            {
                // Shift the look-ahead buffer forward.
                for (int i = 1; i < mBuffer.Length; i++)
                {
                    mBuffer[i - 1] = mBuffer[i];
                }
                // Read another character.
                mBuffer[mBuffer.Length - 1] = ReadNextChar();
            }
            return mBuffer[mPositionInBuffer] != EofChar;
        }

        /// <summary>
        /// Reconsumes the current character (steps backward).
        /// </summary>
        /// <remarks>
        /// Only one character can be reconsumed at a time. Repeated calls to this method will throw an exception.
        /// </remarks>
        internal void ReconsumeChar()
        {
            if (mPositionInBuffer > 0)
            {
                --mPositionInBuffer;
                return;
            }
            throw new InvalidOperationException("Tried to reconsume too many characters.");
        }

        /// <summary>
        /// Returns a value indicating whether the end of stream has been reached and the next character is
        /// <see cref="EofChar"/>.
        /// </summary>
        internal bool IsAtEof()
        {
            return Next == EofChar;
        }

        /// <summary>
        /// The last consumed character.
        /// </summary>
        internal char Current
        {
            get { return mBuffer[mPositionInBuffer]; }
        }

        /// <summary>
        /// The character to be consumed next.
        /// </summary>
        internal char Next
        {
            get { return mBuffer[mPositionInBuffer + 1]; }
        }

        /// <summary>
        /// The the character to be consumed after <see cref="Next"/>.
        /// </summary>
        internal char Next2
        {
            get { return mBuffer[mPositionInBuffer + 2]; }
        }

        /// <summary>
        /// The the character to be consumed after <see cref="Next2"/>.
        /// </summary>
        internal char Next3
        {
            get { return mBuffer[mPositionInBuffer + 3]; }
        }

        /// <summary>
        /// A special character indicating that the end of the CSS text has been reached and there are no more characters to
        /// consume.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char EofChar = '\0';

        /// <summary>
        /// Reads another character from the CSS text.
        /// </summary>
        /// <remarks>
        /// Characters are preprocessed as desrcibed here: https://www.w3.org/TR/css-syntax-3/#input-preprocessing
        /// </remarks>
        private char ReadNextChar()
        {
            if (mPositionInCss >= mCss.Length)
            {
                return EofChar;
            }

            char nextChar = mCss[mPositionInCss];
            ++mPositionInCss;

            switch (nextChar)
            {
                case '\r':
                {
                    // Replace "\r\n" with a single '\n' character.
                    if ((mPositionInCss < mCss.Length) && (mCss[mPositionInCss] == '\n'))
                    {
                        ++mPositionInCss;
                    }
                    // Normalize line endings.
                    return '\n';
                }
                case '\f':
                    // Normalize line endings.
                    return '\n';
                case '\0':
                    // Replace zeroes.
                    return UnicodeUtil.ReplacementChar;
                default:
                    return nextChar;
            }
        }

        /// <summary>
        /// The original CSS text being processed.
        /// </summary>
        private readonly string mCss;

        /// <summary>
        /// Index of the character to read next from the original CSS text.
        /// </summary>
        private int mPositionInCss;

        /// <summary>
        /// Max number of next characters the calling code can examine without consuming.
        /// </summary>
        private const int LookAheadBufferDepth = 3;

        /// <summary>
        /// Max number of characters the calling code can reconsume.
        /// </summary>
        private const int ReconsumeBufferDepth = 1;

        /// <summary>
        /// Buffer of pre-processed characters.
        /// </summary>
        /// <remarks>
        /// The buffer contains pre-processed characters (see <see cref="ReadNextChar"/>) in the same order as in the original
        /// CSS text. The structure of the buffer from the beginning to the end is as follows: reconsumed characters
        /// (the reconsume buffer), the current character (see <see cref="Current"/>), and next characters that has not been
        /// consumed yet (the look-ahead buffer, see <see cref="Next"/> and similar properties).
        /// </remarks>
        private readonly char[] mBuffer = new char[LookAheadBufferDepth + ReconsumeBufferDepth + 1];

        /// <summary>
        /// Index of the current character in <see cref="mBuffer"/>.
        /// </summary>
        /// <remarks>
        /// This index moves backward when the current character is reconsumed and moves forward when a reconsumed character
        /// is later consumed again.
        /// </remarks>
        private int mPositionInBuffer;
    }
}
