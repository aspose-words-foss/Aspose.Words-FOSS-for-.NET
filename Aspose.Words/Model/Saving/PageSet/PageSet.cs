// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2020 by Dmitry Burov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Describes a random set of pages.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    public sealed class PageSet : IEnumerable<int>
    {
        /// <summary>
        /// Gets a set with all the pages of the document in their original order.
        /// </summary>
        public static PageSet All
        {
            get { return new PageSet(PageParity.Any, new PageRange(0, int.MaxValue)); }
        }

        /// <summary>
        /// Gets a set with all the even pages of the document in their original order.
        /// </summary>
        /// <remarks>
        /// Even pages have odd indices since page indices are zero-based.
        /// </remarks>
        public static PageSet Even
        {
            get { return new PageSet(PageParity.Even, new PageRange(0, int.MaxValue)); }
        }

        /// <summary>
        /// Gets a set with all the odd pages of the document in their original order.
        /// </summary>
        /// <remarks>
        /// Odd pages have even indices since page indices are zero-based.
        /// </remarks>
        public static PageSet Odd
        {
            get { return new PageSet(PageParity.Odd, new PageRange(0, int.MaxValue)); }
        }

        /// <summary>
        /// Creates an one-page set based on exact page index.
        /// </summary>
        /// <param name="page">Zero-based index of the page.</param>
        /// <remarks>
        /// If a page is encountered that is not in the document, an exception will be thrown during rendering.
        /// <ms><see cref="int.MaxValue"/></ms><java><b>Integer.MAX_VALUE</b></java><cpp><see cref="int.MaxValue"/></cpp> means the last page in the document.
        /// </remarks>
        public PageSet(int page)
        {
            mSet = new PageSetOnPages(new int[] { page });
        }

        /// <summary>
        /// Creates a page set based on exact page indices.
        /// </summary>
        /// <param name="pages">Zero-based indices of pages.</param>
        /// <remarks>
        /// If a page is encountered that is not in the document, an exception will be thrown during rendering.
        /// <ms><see cref="int.MaxValue"/></ms><java><b>Integer.MAX_VALUE</b></java><cpp><see cref="int.MaxValue"/></cpp> means the last page in the document.
        /// </remarks>
        public PageSet(params int[] pages)
        {
            mSet = new PageSetOnPages(pages);
        }

        /// <summary>
        /// Creates a page set based on ranges.
        /// </summary>
        /// <param name="ranges">Array of page ranges.</param>
        /// <remarks>
        /// If a range is encountered that starts after the last page in the document, 
        /// an exception will be thrown during rendering.
        /// All ranges that end after the last page are truncated to fit in the document.
        /// </remarks>
        public PageSet(params PageRange[] ranges)
            : this(PageParity.Any, ranges)
        {
        }

        public IEnumerator<int> GetEnumerator()
        {
            return mSet.GetEnumerator(int.MaxValue - 1);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates a page set based on the page numbers and page ranges, separated by commas.
        /// For example, "3, 5-8" includes page 3 and pages 5 through 8.
        /// </summary>
        /// <param name="pages">String description of pages and page ranges</param>
        /// <remarks>
        /// If a range or a page is encountered that starts after the last page in the document, 
        /// an exception will be thrown during rendering.
        /// All ranges that end after the last page are truncated to fit in the document.
        /// </remarks>
        internal PageSet(string pages)
        {
            mSet = pages.Contains("-")
                ? new PageSetOnRanges(pages)
                : (PageSetBase)new PageSetOnPages(pages);
        }

        /// <summary>
        /// Creates a page set based on ranges.
        /// </summary>
        /// <param name="parity">Parity of pages to select.</param>
        /// <param name="ranges">Array of page ranges</param>
        /// <remarks>
        /// Even pages have odd indices and vice versa, since page indices are zero-based.
        /// </remarks>
        internal PageSet(PageParity parity, params PageRange[] ranges)
        {
            mSet = new PageSetOnRanges(parity, ranges);
        }

        /// <summary>
        /// Internal implementation of page set.
        /// </summary>
        /// <remarks>
        /// Using this wrapping since we decided to hide all the methods,
        /// including enumerator interface, from the public API.
        /// </remarks>
        internal PageSetBase Core
        {
            get { return mSet; }
        }

        private readonly PageSetBase mSet;
    }
}
