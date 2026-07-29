// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Ilya Navrotskiy

using System.Text.RegularExpressions;

namespace Aspose.Words.RW.Markdown.FormatDetector
{
    /// <summary>
    /// The markdown List detector.
    /// </summary>
    /// <remarks>
    /// List is a line with special character +, -, * or with digit at the beginning.
    /// </remarks>
    internal class ListDetector : MarkdownDetectorBase
    {
        /// <summary>
        /// Detects List block in a specified context.
        /// </summary>
        protected override bool Detect(MarkdownDetectorContext context)
        {
            if ((context.PrevLineDetectedFeatures == null) || MarkdownDetectorContext.CanBreakParagraph(context.PrevLineDetectedFeatures))
                mIsListStarted = false;

            Match match = mListRegex.Match(context.Line);
            if (match.Success)
            {
                int prevQuoteLevel = mQuoteLevel;
                mQuoteLevel = QuoteDetector.GetQuoteLevel(match.Value);

                char prevListMarker = mListMarker;
                mListMarker = GetListMarker(match.Value);
                
                // If it is not a continuation of a previous List, then start a new one.
                if (!mIsListStarted)
                {
                    Count++;
                    mIsListStarted = true;
                    return true;
                }

                // If previous and current Lists are inside Quotes of different levels,
                // then we need to start a new List.
                if (prevQuoteLevel != mQuoteLevel)
                {
                    Count++;
                    return true;
                }

                // Changing the bullet or ordered list marker starts a new list.
                if (prevListMarker != mListMarker)
                {
                    Count++;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns list marker for a specified list string.
        /// </summary>
        private static char GetListMarker(string list)
        {
            Debug.Assert(StringUtil.HasChars(list));

            int markerIdx = list.IndexOfAny(new[] {'*', '-', '+'});
            // If none of markers for unordered lists were found, then we have an ordered list that starts from a digit.
            if (markerIdx == -1)
                return '1';

            return list[markerIdx];
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
            get { return MarkdownFeatureType.List; }
        }

        /// <summary>
        /// Indicates whether a List block is started.
        /// </summary>
        private bool mIsListStarted;

        /// <summary>
        /// A marker of the current List.
        /// </summary>
        private char mListMarker;

        /// <summary>
        /// A level of the Quote where List block is located (if any).
        /// </summary>
        private int mQuoteLevel;

        /// <summary>
        /// The regex that matches a markdown List block.
        /// </summary>
        private readonly Regex mListRegex = new Regex(@"(^(\s{0,3}([>]+\s{0,4})?)?) ((\d+\.|[*+-])$ | (\d+\.|[*+-])\s+)",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
    }
}
