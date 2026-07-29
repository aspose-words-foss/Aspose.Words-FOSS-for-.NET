// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown Emphasis detector.
    /// </summary>
    internal class EmphasisDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Detects markdown inline blocks, such as emphases and code spans.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            if (context.IsPrevParaBroken)
                Reset();

            char prevChar = '\0';
            for (int i = 0; i < context.Line.Length; i++)
            {
                char curChar = context.Line[i];
                if (curChar == prevChar)
                    continue;

                switch (curChar)
                {
                    case '`':
                    {
                        // If it was opened already, then now we encounter a closing sequence
                        // and should count it as completed code span.
                        if (mIsBacktick)
                            Count++;

                        mIsBacktick = !mIsBacktick;
                        break;
                    }
                    case '~':
                    {
                        // If it was opened already, then now we encounter a closing sequence
                        // and should count it as completed strike-through.
                        if (mIsStrikethrough)
                            Count++;

                        mIsStrikethrough = !mIsStrikethrough;
                        break;
                    }
                    case '*':
                    {
                        // Exclude emphasis that is more likely is a wildcard in a path.
                        if (((prevChar == '\\') || (prevChar == '/')) &&
                            ((i + 1) < context.Line.Length) && (context.Line[i + 1] == '.'))
                            break;

                        // If it was opened already, then now we encounter a closing sequence
                        // and should count it as completed emphasis.
                        if (mIsAsterisk)
                        {
                            // WORDSNET-26035 To be a closing sequence of emphasis there at least should not
                            // be any whitespace characters just before this sequence.
                            if (!char.IsWhiteSpace(prevChar))
                            {
                                // If length of closing sequence is greater than 2,
                                // then we treat it as two different emphases (bold + italic).
                                int end = i;
                                while ((end < context.Line.Length) && (context.Line[end] == '*'))
                                    end++;
                                int emphasesCount = ((end - i) > 2) ? 2 : 1;
                                Count += emphasesCount;

                                mIsAsterisk = false;
                            }

                        }
                        else
                        {
                            // WORDSNET-26035 To be an opening sequence of emphasis there at least should not
                            // be any whitespace characters just after this sequence.
                            if ((i + 1) < context.Line.Length)
                            {
                                char nextChar = context.Line[i + 1];
                                if (!char.IsWhiteSpace(nextChar))
                                    mIsAsterisk = true;
                            }
                        }

                        break;
                    }
                    case '_':
                    {
                        // A sequence of '_' is not allowed inside words to be emphasis by spec.
                        int nextCharIdx = StringUtil.IndexOfNotEqualTo(context.Line, '_', (i + 1));
                        char nextChar = (nextCharIdx == -1) ? '\0' : context.Line[nextCharIdx];
                        if(MarkdownUtil.IsIntraword(prevChar, nextChar))
                            break;

                        // If it was opened already, then now we encounter a closing sequence
                        // and should count it as completed emphasis.
                        if (mIsUnderscore)
                        {
                            // If length of closing sequence is greater than 2,
                            // then we treat it as two different emphases (bold + italic).
                            if (nextCharIdx == -1)
                                nextCharIdx = context.Line.Length;
                            int emphasesCount = ((nextCharIdx - i) > 2) ? 2 : 1;
                            Count += emphasesCount;
                        }

                        mIsUnderscore = !mIsUnderscore;
                        break;
                    }
                    default:
                        // Do nothing by default.
                        break;
                }

                prevChar = curChar;
            }

            return (Count > 0);
        }

        /// <summary>
        /// Gets markdown feature significance of the detector.
        /// </summary>
        protected override MarkdownFeatureSignificance Significance
        {
            get { return MarkdownFeatureSignificance.Low; }
        }

        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected override MarkdownFeatureType Type
        {
            get { return MarkdownFeatureType.Emphasis; }
        }

        /// <summary>
        /// Resets accumulated states of all found features.
        /// </summary>
        private void Reset()
        {
            mIsAsterisk = false;
            mIsUnderscore = false;
            mIsBacktick = false;
            mIsStrikethrough = false;
        }

        /// <summary>
        /// Keeps state of found inline feature.
        /// </summary>
        private bool mIsAsterisk;
        private bool mIsUnderscore;
        private bool mIsBacktick;
        private bool mIsStrikethrough;
    }
}
