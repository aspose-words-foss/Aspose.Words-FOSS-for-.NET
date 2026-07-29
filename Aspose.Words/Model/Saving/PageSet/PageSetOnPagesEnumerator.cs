// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2020 by Dmitry Burov

using System;
using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    internal class PageSetOnPagesEnumerator : PageSetEnumerator, IEnumerator<int>
    {
        internal PageSetOnPagesEnumerator(int[] pages, int documentPageCount)
        {
            int maxPageIndex = documentPageCount - 1;
            mPages = new int[pages.Length];
            for (int i = 0; i < pages.Length; i++)
            {
                int value = pages[i];
                if (value == int.MaxValue)
                    value = maxPageIndex;
                else if (value >= documentPageCount)
                    throw new ArgumentOutOfRangeException("pageIndex",
                        "Page index " + value +" exceeds the index of the last page "+(documentPageCount - 1) + " in the document.");

                mPages[i] = value;
            }
        }

        public override bool MoveNext()
        {
            mCurrentItem++;
            return mCurrentItem < mPages.Length;
        }

        public override void Reset()
        {
            mCurrentItem = -1;
        }

        public override int Current
        {
            get { return mPages[mCurrentItem]; }
        }

        private readonly int[] mPages;
        private int mCurrentItem = -1;
    }
}
