// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

using System.Text;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Preprocesses an HTML text according to HTML 5 rules and provides means to read its contents.
    /// </summary>
    internal class HtmlStream
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="html">An HTML text read from file.</param>
        internal HtmlStream(string html)
        {
            Debug.Assert(html != null);

            // Preprocess the HTML as described here: http://www.w3.org/TR/html5/syntax.html#preprocessing-the-input-stream
            // The leading BOM character is not removed, because it was removed when the HTML text was loaded.
            StringBuilder preprocessedHtml = new StringBuilder(html);
            // Convert CRLF to LF, single CR to LF. The order of the line ending replacements is important.
            preprocessedHtml.Replace("\r\n", "\n");
            preprocessedHtml.Replace('\r', '\n');

            // It is unlikely that we meet an end-of-file character in an HTML document, but let's secure ourselves against 
            // that situation by replacing the end-of-file character with the null character, which will be processed
            // by the HTML tokenizer.
            preprocessedHtml.Replace(EofChar, '\u0000');

            mHtml = preprocessedHtml.ToString();

            // Set the current position just before the first character, so the first character will be consumed next.
            Seek(-1);
        }

        /// <summary>
        /// Moves the stream pointer forward one character if possible.
        /// </summary>
        internal void ConsumeChar()
        {
            ConsumeChar(1);
        }

        /// <summary>
        /// Moves the stream pointer forward the specified number of characters.
        /// </summary>
        /// <param name="count">The number of characters the stream pointer will be moved.</param>
        /// <remarks>
        /// This method either moves the stream pointer exactly the specified number of characters forward, or fails.
        /// </remarks>
        internal void ConsumeChar(int count)
        {
            int newPosition = mPosition + count;
            Debug.Assert(newPosition <= mHtml.Length);
            Seek(newPosition);
        }

        /// <summary>
        /// Moves the stream pointer back one character if possible.
        /// </summary>
        internal void UnconsumeChar()
        {
            int newPosition = mPosition - 1;
            Debug.Assert(newPosition >= -1);
            Seek(newPosition);
        }

        /// <summary>
        /// Compares next characters of the stream with the specified string.
        /// </summary>
        /// <param name="str">The string to compare with.</param>
        /// <param name="asciiCaseInsensitive">
        /// Whether to use ASCII case-insensitive comparison. Otherwise, ordinal case-sensitive comparison is used.
        /// </param>
        /// <remarks>This method does not move the stream pointer.</remarks>
        internal bool NextStringEquals(string str, bool asciiCaseInsensitive)
        {
            Debug.Assert(StringUtil.HasChars(str));

            int position = mPosition;
            for (int i = 0; i < str.Length; i++)
            {
                ++position;
                if (!IsInsideHtml(position))
                {
                    return false;
                }
                char c1 = mHtml[position];
                char c2 = str[i];
                if (asciiCaseInsensitive)
                {
                    c1 = StringUtil.AsciiLowerCase(c1);
                    c2 = StringUtil.AsciiLowerCase(c2);
                }
                if (c1 != c2)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Stores the current stream pointer in a bookmark.
        /// </summary>
        /// <returns>A bookmark instance storing the current stream pointer value.</returns>
        internal HtmlStreamBookmark SetBookmark()
        {
            return new HtmlStreamBookmark(this, mPosition);
        }

        /// <summary>
        /// Moves the stream pointer to the position of the specified bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark storing the position to which the stream pointer will be moved.</param>
        internal void UnconsumeToBookmark(HtmlStreamBookmark bookmark)
        {
            Debug.Assert(bookmark != null);
            Debug.Assert(bookmark.Stream == this);

            Seek(bookmark.Position);
        }

        /// <summary>
        /// Gets text from the stream starting from the specified bookmark and up to the current position.
        /// The current character is not included.
        /// </summary>
        internal string GetBookmarkedText(HtmlStreamBookmark bookmark)
        {
            Debug.Assert(bookmark != null);
            Debug.Assert(bookmark.Stream == this);

            int start = System.Math.Max(0, bookmark.Position);
            int length = System.Math.Max(0, mPosition - start);

            return mHtml.Substring(start, length);
        }

        /// <summary>
        /// Gets the character that has been consumed last. Does not move the stream pointer.
        /// </summary>
        internal char CurrentChar
        {
            get { return mCurrentChar; }
        }

        /// <summary>
        /// Gets the character that will be consumed (read) next. Does not move the stream pointer.
        /// </summary>
        internal char NextChar
        {
            get { return mNextChar; }
        }

        /// <summary>
        /// The special character that indicates the absence of any character (the end-of-file character).
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char EofChar = '\uFFFF';

        /// <summary>
        /// Moves the stream pointer to the specified position and reads current and next characters.
        /// </summary>
        /// <param name="position">The position to which the stream pointer will be moved.</param>
        private void Seek(int position)
        {
#if DEBUG
            Debug.Assert(position >= -1);
            Debug.Assert(position <= mHtml.Length);
#endif
            mPosition = position;

            mCurrentChar = (IsInsideHtml(position))
                ? mHtml[position]
                : EofChar;

            int nextCharPosition = position + 1;
            mNextChar = (IsInsideHtml(nextCharPosition))
                ? mHtml[nextCharPosition]
                : EofChar;
        }

        /// <summary>
        /// Checks whether the specified position is a valid index of the preprocessed HTML text.
        /// </summary>
        /// <param name="position">The position that will be checked.</param>
        /// <returns><c>true</c> if the position is a valid index; <c>false</c> otherwise.</returns>
        private bool IsInsideHtml(int position)
        {
            return (position >= 0) && (position < mHtml.Length);
        }

        /// <summary>
        /// The preprocessed HTML text.
        /// </summary>
        private readonly string mHtml;

        /// <summary>
        /// The stream pointer. Index of the character that has been consumed last.
        /// </summary>
        private int mPosition;

        /// <summary>
        /// The character that has been consumed last.
        /// </summary>
        private char mCurrentChar;

        /// <summary>
        /// The character that will be consumed next.
        /// </summary>
        private char mNextChar;
    }
}
