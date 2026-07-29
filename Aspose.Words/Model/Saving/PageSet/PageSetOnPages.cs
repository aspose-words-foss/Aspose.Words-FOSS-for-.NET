// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2020 by Dmitry Burov

using System;
using Aspose.Collections;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Describes a random set of pages based on exact pages.
    /// </summary>
    internal class PageSetOnPages : PageSetBase
    {
        /// <summary>
        /// Creates a page set based on exact page indices.
        /// </summary>
        /// <param name="pages">Zero-based indices of pages.</param>
        /// <remarks>
        /// <see cref="int.MaxValue"/> means the last page in the document.
        /// </remarks>
        internal PageSetOnPages(params int[] pages)
        {
            CheckForNegativeIndices(pages);

            // Protect against external data changes.
            mPages = (int[])pages.Clone();
        }

        /// <summary>
        /// Creates a page set based on the page numbers, separated by commas.
        /// For example, "3, 5" includes page 3 and page 5.
        /// </summary>
        /// <param name="pages">String description of pages.</param>
        internal PageSetOnPages(string pages)
        {
            IntList pageSet = new IntList();

            // Build a list of pages.
            foreach (string pageString in pages.Split(','))
            {
                int page = int.Parse(pageString.Trim());
                pageSet.Add(page);                
            }

            mPages = new int[pageSet.Count];
            pageSet.CopyTo(mPages);

            CheckForNegativeIndices(mPages);
        }

        private static void CheckForNegativeIndices(int[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
                if (pages[i] < 0)
                    throw new ArgumentOutOfRangeException("pages", "Negative page index specified.");
        }

        public override PageSetEnumerator GetEnumerator(int documentPageCount)
        {
            return new PageSetOnPagesEnumerator(mPages, documentPageCount);
        }

        private readonly int[] mPages;
    }
}
