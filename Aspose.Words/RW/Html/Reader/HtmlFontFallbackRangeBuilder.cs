// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/12/2014 by Victor Chebotok

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Helps to group text fragments into an array <see cref="HtmlFontFallbackRange"/> so that no adjacent ranges in that array
    /// have the same fallback font name.
    /// </summary>
    internal class HtmlFontFallbackRangeBuilder
    {
        /// <summary>
        /// Appends a text fragment to font fallback ranges.
        /// </summary>
        /// <param name="text">A text fragment.</param>
        /// <param name="fallbackFontName">
        /// Name of the fallback font that should be used to render the text. May be <c>null</c> if no font fallback is required.
        /// </param>
        /// <remarks>
        /// Adjacent text fragments are concatenated if they have same fallback font name.
        /// </remarks>
        internal void Append(string text, string fallbackFontName)
        {
            if (mCurrentRangeFallbackFontName != fallbackFontName)
            {
                CloseCurrentRange();
            }
            mCurrentRangeText.Append(text);
            mCurrentRangeFallbackFontName = fallbackFontName;
        }

        /// <summary>
        /// Gets font fallback ranges that were accumulated by the builder.
        /// </summary>
        internal HtmlFontFallbackRange[] GetRanges()
        {
            CloseCurrentRange();
            return mRanges.ToArray();
        }

        private void CloseCurrentRange()
        {
            if (mCurrentRangeText.Length > 0)
            {
                mRanges.Add(new HtmlFontFallbackRange(mCurrentRangeText.ToString(), mCurrentRangeFallbackFontName));
                mCurrentRangeText.Length = 0;
            }
        }

        private readonly List<HtmlFontFallbackRange> mRanges = new List<HtmlFontFallbackRange>();
        private readonly StringBuilder mCurrentRangeText = new StringBuilder();
        private string mCurrentRangeFallbackFontName;
    }
}
