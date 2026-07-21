// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System.Text;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.RW.Txt.Reader;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown format detector context.
    /// </summary>
    /// <remarks>
    /// Allows to read data from a text reader and keeps various information about it to use in markdown detectors.
    /// </remarks>
    internal class MarkdownDetectorContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MarkdownDetectorContext(CustomTextReader reader)
        {
            mReader = reader;
        }

        /// <summary>
        /// Gets a boolean value indicating whether a specified collection contains
        /// markdown features that can break a paragraph.
        /// </summary>
        internal static bool CanBreakParagraph(HashSetGeneric<MarkdownFeatureType> featureTypes)
        {
            Debug.Assert(featureTypes != null);

            foreach (MarkdownFeatureType featureType in featureTypes)
            {
                if (CanBreakParagraph(featureType))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Reads a new line from a reader.
        /// </summary>
        internal bool ReadLine()
        {
            mPrevLine = mLine;
            mPrevLineDetectedFeatures = mCurLineDetectedFeatures;
            mCurLineDetectedFeatures = new HashSetGeneric<MarkdownFeatureType>();

            StringBuilder sb = new StringBuilder();

            while (CanRead)
            {
                char c = mReader.ReadChar();

                // WORDSNET-28614 Account sentences length.
                if (IsEndOfSentence)
                {
                    if (mCurSentenceLength > MaxSentenceLength)
                        mSentencesWithExceededLengthTotalLength += mCurSentenceLength;
                    mCurSentenceLength = 0;
                }
                else
                {
                    mCurSentenceLength++;
                }

                if (TxtFormatDetector.IsControlChar(c) && !TxtFormatDetector.IsAllowedControlChar(c, Encoding.CodePage))
                    mNonPrintableCharsCount++;

                switch (c)
                {
                    case ControlChar.ParagraphBreakChar:
                    {
                        // Normalize input - use \n for end of line instead of \r\n.
                        sb.Append(ControlChar.LineFeedChar);
                        // Skip \n that follows just after \r.
                        Read(ControlChar.LineFeedChar);

                        mLine = sb.ToString();
                        return true;
                    }
                    case ControlChar.LineFeedChar:
                    {
                        sb.Append(ControlChar.LineFeedChar);
                        mLine = sb.ToString();

                        return true;
                    }
                    case ControlChar.TabChar:
                    {
                        // Expand tabs to detect markdown blocks, such as InlineCode.
                        sb.Append(new string(' ', MarkdownUtil.TabSize));
                        break;
                    }
                    default:
                        sb.Append(c);
                        break;
                }
            }

            mLine = sb.ToString();

            return (mLine.Length > 0);
        }

        /// <summary>
        /// Reads a next character from the reader, if it is equal to a specified one.
        /// </summary>
        private void Read(char c)
        {
            if (CanRead)
            {
                char curChar = mReader.ReadChar();
                if (c != curChar)
                    mReader.StepBack();
            }
        }

#if DEBUG
        public override string ToString()
        {
            return Line;
        }
#endif
        /// <summary>
        /// Gets last read line.
        /// </summary>
        internal string Line
        {
            get { return mLine; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the previous paragraph was broken.
        /// </summary>
        internal bool IsPrevParaBroken
        {
            get { return (IsPrevLineBlank || CanBreakParagraph(mPrevLineDetectedFeatures)); }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the reader has available characters to read.
        /// </summary>
        internal bool EndOfFile
        {
            get { return !mReader.HasChars; }
        }

        /// <summary>
        /// The collection of markdown features detected in a current line of text.
        /// </summary>
        internal HashSetGeneric<MarkdownFeatureType> CurLineDetectedFeatures
        {
            get { return mCurLineDetectedFeatures; }
        }

        /// <summary>
        /// The collection of markdown features detected in a previous line of text.
        /// </summary>
        internal HashSetGeneric<MarkdownFeatureType> PrevLineDetectedFeatures
        {
            get { return mPrevLineDetectedFeatures; }
        }

        /// <summary>
        /// Gets Encoding object of the reader.
        /// </summary>
        internal Encoding Encoding
        {
            get { return mReader.Encoding; }
        }

        /// <summary>
        /// Gets the total number of characters being read by the reader.
        /// </summary>
        internal long Length
        {
            // WORDSNET-18865 Limit total length with maximum allowed number of characters to read.
            get { return System.Math.Min(mReader.Stream.Length, MaxCharsToRead); }
        }

        /// <summary>
        /// Gets count of the non-printable characters that were read in the context.
        /// </summary>
        internal int NonPrintableCharsCount
        {
            get { return mNonPrintableCharsCount; }
        }

        /// <summary>
        /// Gets integer value representing total length of sentences
        /// which length exceeds maximum allowed <see cref="MaxSentenceLength"/>.
        /// </summary>
        internal int LongSentencesTotalLength
        {
            get
            {
                int sentencesWithExceededLengthTotalLength = mSentencesWithExceededLengthTotalLength;
                if (mCurSentenceLength > MaxSentenceLength)
                    sentencesWithExceededLengthTotalLength += mCurSentenceLength;

                return sentencesWithExceededLengthTotalLength;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether a specified markdown feature can break a paragraph.
        /// </summary>
        private static bool CanBreakParagraph(MarkdownFeatureType type)
        {
            return ((type == MarkdownFeatureType.Heading) || (type == MarkdownFeatureType.HorizontalRule));
        }

        /// <summary>
        /// Gets a boolean value indicating whether the previous line is blank.
        /// </summary>
        private bool IsPrevLineBlank
        {
            get { return StringUtil.ContainsOnlyWhitespaces(mPrevLine); }
        }

        /// <summary>
        /// Returns true, if we currently at the end of a sentence.
        /// </summary>
        private bool IsEndOfSentence
        {
            get
            {
                char curChar = mReader.CurChar;
                if ((curChar != '.') && (curChar != '!') && (curChar != '?'))
                    return false;

                char nextChar = mReader.NextChar;
                if ((nextChar == '\0') && CanRead)
                {
                    nextChar = mReader.ReadChar();
                    mReader.StepBack();
                }

                return (nextChar == '\0') || char.IsWhiteSpace(nextChar);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether a new character can be read from the underlying reader.
        /// </summary>
        private bool CanRead
        {
            get
            {
                if (mReader.TotalCharsRead >= MaxCharsToRead)
                    return false;

                if (!mReader.HasChars)
                {
                    // When the reader has no more characters to read, the reason of that can be of two types:
                    // 1) The underlying stream has no more bytes.
                    // 2) The read limit is reached.
                    // As we have not yet exceeded the total allowed number of characters to read, we need to reset
                    // read limit to try read a new portion of bytes from the stream to a buffer.
                    mReader.ResetReadLimit();
                    if (!mReader.HasChars)
                        return false;
                }

                return true;
            }
        }

        private readonly CustomTextReader mReader;

        private string mPrevLine;
        private string mLine;

        private HashSetGeneric<MarkdownFeatureType> mCurLineDetectedFeatures;
        private HashSetGeneric<MarkdownFeatureType> mPrevLineDetectedFeatures;

        private int mNonPrintableCharsCount;

        private int mCurSentenceLength;
        private int mSentencesWithExceededLengthTotalLength;

        /// <summary>
        /// Limits the maximum number of characters to read.
        /// </summary>
        /// <remarks>
        /// This value is obtained by experimental means. Feel free to adjust it if needed.
        /// </remarks>
        private const int MaxCharsToRead = 1024;

        /// <summary>
        /// Specifies the maximum length of a sentence, beyond which it will be considered too long.
        /// </summary>
        /// <remarks>
        /// This is an approximate value chosen based on the following considerations.
        /// There can be about 15-20 words in a sentence and each word consists of about 7-8 letters in average.
        /// Actually, this may be too much or too little in some cases. Then, we can reduce the value in the future,
        /// if needed.
        /// </remarks>
        private const int MaxSentenceLength = 160;
    }
}
