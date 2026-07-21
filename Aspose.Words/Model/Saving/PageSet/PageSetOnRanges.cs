// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2020 by Dmitry Burov

using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Describes a random set of pages based on page ranges.
    /// </summary>
    internal class PageSetOnRanges : PageSetBase
    {
        /// <summary>
        /// Creates a page set based on ranges.
        /// </summary>
        /// <param name="parity">Parity of pages to select.</param>
        /// <param name="ranges">Array of page ranges.</param>
        /// <remarks>
        /// Even pages have odd indices and vice versa, since page indices are zero-based.
        /// </remarks>
        internal PageSetOnRanges(PageParity parity, params PageRange[] ranges)
        {
            mParity = parity;

            // Protect against external data changes.
            mRanges = CopyRanges(ranges);            
        }

        /// <summary>
        /// Creates a page set based on the page numbers and page ranges, separated by commas.
        /// For example, "3, 5-8" includes page 3 and pages 5 through 8.
        /// </summary>
        /// <param name="pages">String description of pages and page ranges</param>
        internal PageSetOnRanges(string pages)
        {
            List<PageRange> pageSet = new List<PageRange>();

            // Build a list of page ranges.
            foreach (string pageRangeString in pages.Split(','))
            {
                string[] pageRangeParts = pageRangeString.Split('-');

                int from = int.Parse(pageRangeParts[0].Trim());
                int to = (pageRangeParts.Length > 1) ? int.Parse(pageRangeParts[1].Trim()) : from;

                PageRange pageRange = new PageRange(from, to);
                pageSet.Add(pageRange);
            }

            mRanges = new PageRange[pageSet.Count];
            pageSet.CopyTo(mRanges);
        }

        private static PageRange[] CopyRanges(PageRange[] ranges)
        {
            PageRange[] copy = new PageRange[ranges.Length];
            for (int i = 0; i < ranges.Length; i++)
                copy[i] = ranges[i];

            return copy;
        }

        public override PageSetEnumerator GetEnumerator(int documentPageCount)
        {
            return new PageSetOnRangesEnumerator(mParity, mRanges, documentPageCount);
        }

        private readonly PageParity mParity = PageParity.Any;
        private readonly PageRange[] mRanges;
    }
}
