// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2020 by Dmitry Burov

using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Base for classes describing a random set of pages.
    /// </summary>
    internal abstract class PageSetBase
    {
        /// <summary>
        /// Gets the total number of pages specified in set.
        /// </summary>
        internal int GetPageCount(int documentPageCount)
        {
            int pageCount = 0;
            foreach (int unused in GetEnumerator(documentPageCount))
                pageCount++;

            return pageCount;
        }

        /// <summary>
        /// Gets the zero-based index of the first page in the set.
        /// </summary>
        internal int GetStartIndex(int documentPageCount)
        {
            IEnumerator<int> enumerator = GetEnumerator(documentPageCount);
            enumerator.MoveNext();
            return enumerator.Current;
        }

        public abstract PageSetEnumerator GetEnumerator(int documentPageCount);
    }
}
