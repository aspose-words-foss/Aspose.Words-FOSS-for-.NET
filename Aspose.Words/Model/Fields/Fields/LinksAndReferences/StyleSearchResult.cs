// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a result of the search performed by <see cref="StyleFinder"/>.
    /// </summary>
    internal class StyleSearchResult
    {
        /// <summary>
        /// Creates a <see cref="StyleSearchResult"/> instance if the given arguments correspond to a valid capture,
        /// i.e. a paragraph break and/or a run range are captured because they have the target style applied.
        /// Otherwise, returns <c>null</c>.
        /// </summary>
        internal static StyleSearchResult CreateIfNeeded(
            Paragraph paragraph,
            int startIndex,
            int endIndex,
            bool isForwardDirection,
            bool isParagraphBreakIncluded,
            bool isParagraphBreakStyleApplied)
        {
            if (isParagraphBreakStyleApplied || IsValidIndexRange(startIndex, endIndex))
                return new StyleSearchResult(paragraph, startIndex, endIndex, isForwardDirection, isParagraphBreakIncluded);

            return null;
        }

        internal static StyleSearchResult CreateWholeParagraphResult(Paragraph paragraph, bool isForwardDirection)
        {
            // The whole paragraph is captured. MS Word does not include a paragraph break in this case.
            NodeCollection inlines = StyleFinder.GetParagraphInlines(paragraph);
            bool hasInlines = inlines.Count != 0;
            return new StyleSearchResult(
                paragraph,
                hasInlines ? 0 : -1,
                hasInlines ? inlines.Count - 1 : 0,
                isForwardDirection,
                false);
        }

        /// <summary>
        /// Returns a value indicating whether the specified index range is valid.
        /// </summary>
        private static bool IsValidIndexRange(int startIndex, int endIndex)
        {
            return ((startIndex >= 0) && (endIndex >= 0));
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private StyleSearchResult(
            Paragraph paragraph,
            int startIndex,
            int endIndex,
            bool isForwardDirection,
            bool isParagraphBreakIncluded)
        {
            Paragraph = paragraph;
            StartIndex = startIndex;
            EndIndex = endIndex;
            IsForwardDirection = isForwardDirection;
            IsParagraphBreakIncluded = isParagraphBreakIncluded;
        }

        /// <summary>
        /// Gets the paragraph that is captured by this instance.
        /// </summary>
        internal Paragraph Paragraph { get; }

        /// <summary>
        /// Gets the index of the first run in the paragraph (according to <see cref="IsForwardDirection"/>)
        /// that is captured by this instance.
        /// </summary>
        internal int StartIndex { get; }

        /// <summary>
        /// Gets the index of the last run in the paragraph (according to <see cref="IsForwardDirection"/>)
        /// that is captured by this instance.
        /// </summary>
        internal int EndIndex { get; }

        /// <summary>
        /// Gets a value indicating whether the direction of the used search is forward.
        /// </summary>
        internal bool IsForwardDirection { get; }

        /// <summary>
        /// Gets a value indicating whether the paragraph's break is captured by this instance.
        /// </summary>
        internal bool IsParagraphBreakIncluded { get; }

        /// <summary>
        /// Gets a value indicating whether a run range is captured by this instance.
        /// </summary>
        internal bool HasIndexRange
        {
            get { return IsValidIndexRange(StartIndex, EndIndex); }
        }
    }
}
