// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2020 by Dmitry Burov

using System;
using System.Collections.Generic;

namespace Aspose.Words.Saving
{
    internal class PageSetOnRangesEnumerator : PageSetEnumerator, IEnumerator<int>
    {
        internal PageSetOnRangesEnumerator(PageParity parity, PageRange[] ranges, int documentPageCount)
        {
            mParity = parity;
            mRanges = new PageRange[ranges.Length];

            for (int i = 0; i < ranges.Length; i++)
                mRanges[i] = ranges[i].Initialize(documentPageCount);
        }

        public override bool MoveNext()
        {
            bool moveResult = MoveNextUnconditional();

            switch (mParity)
            {
                case PageParity.Any:
                    break;
                case PageParity.Even:
                    while (moveResult && !IsCurrentEven)
                        moveResult = MoveNextUnconditional();
                    break;
                case PageParity.Odd:
                    while (moveResult && IsCurrentEven)
                        moveResult = MoveNextUnconditional();
                    break;
                default:
                    throw new InvalidOperationException("Unknown parity type encountered.");
            }

            return moveResult;
        }

        /// <summary>
        /// Since page indices are zero-based, even pages have odd indices.
        /// </summary>
        private bool IsCurrentEven
        {
            get { return (Current % 2) != 0; }
        }

        private bool MoveNextUnconditional()
        {
            if (mCurrentItem < 0)
            {
                // Empty set?
                if (mRanges.Length == 0)
                    return false;

                // Move iterator to start.
                mCurrentItem = 0;
                mCurrentOffset = 0;
                return true;
            }
            else
            {
                // Move to the next page in range if possible.
                if (mRanges[mCurrentItem].From + mCurrentOffset < mRanges[mCurrentItem].To)
                {
                    mCurrentOffset++;
                    return true;
                }
                // Otherwise try to switch to the next range.
                else if (mCurrentItem < mRanges.Length - 1)
                {
                    mCurrentItem++;
                    mCurrentOffset = 0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override void Reset()
        {
            mCurrentItem = -1;
            mCurrentOffset = -1;
        }

        public override int Current
        {
            get { return mRanges[mCurrentItem].From + mCurrentOffset; }
        }

        private readonly PageParity mParity;
        private readonly PageRange[] mRanges;
        private int mCurrentItem = -1;
        private int mCurrentOffset = -1;
    }
}
