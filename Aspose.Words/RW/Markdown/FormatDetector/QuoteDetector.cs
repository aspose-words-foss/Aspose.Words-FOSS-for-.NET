// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown Quote detector.
    /// </summary>
    internal class QuoteDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Returns Quote level from a specified line of text.
        /// </summary>
        internal static int GetQuoteLevel(string txtLine)
        {
            int startIdx = txtLine.IndexOf('>');
            if (startIdx == -1)
                return -1;

            int level = 0;
            while (((startIdx + level) < txtLine.Length) && txtLine[startIdx + level] == '>')
                level++;

            return level;
        }

        /// <summary>
        /// Detects a Quote block in a specified context.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            if (context.IsPrevParaBroken)
                mQuoteLevel = -1;

            Match match = mQuoteRegex.Match(context.Line);
            if (match.Success)
            {
                int prevQuoteLevel = mQuoteLevel;
                mQuoteLevel = GetQuoteLevel(match.Value);

                if (mQuoteLevel > prevQuoteLevel)
                {
                    Count++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets markdown feature significance of the detector.
        /// </summary>
        protected override MarkdownFeatureSignificance Significance
        {
            get { return MarkdownFeatureSignificance.Medium; }
        }

        /// <summary>
        /// Gets markdown feature type of the detector.
        /// </summary>
        protected override MarkdownFeatureType Type
        {
            get { return MarkdownFeatureType.Quote; }
        }

        /// <summary>
        /// The regex that matches > with up to three whitespace characters before it.
        /// </summary>
        private readonly Regex mQuoteRegex = new Regex(@"^[ ]{0,3}>", RegexOptions.Compiled);

        /// <summary>
        /// The last processed Quote level.
        /// </summary>
        private int mQuoteLevel;
    }
}
