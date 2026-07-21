// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2010 by Roman Korchagin
// 03/09/2020 by Dmitry Burov

using System;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Represents a continuous range of pages.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    public sealed class PageRange
    {
        /// <summary>
        /// Creates a new page range object.
        /// </summary>
        /// <param name="from">
        /// The starting page zero-based index.
        /// </param>
        /// <param name="to">
        /// The ending page zero-based index. 
        /// If it exceeds the index of the last page in the document, 
        /// it is truncated to fit in the document on rendering.
        /// </param>
        /// <remarks>
        /// <ms><see cref="int.MaxValue"/></ms><java><b>Integer.MAX_VALUE</b></java><cpp><see cref="int.MaxValue"/></cpp> means the last page in the document.
        /// </remarks>
        public PageRange(int from, int to)
        {
            if (from < 0)
                throw new ArgumentOutOfRangeException("from", NegativePageIndexMessage);

            if (to < from)
                throw new ArgumentOutOfRangeException("to", ToLessThanFromMessage);

            mFrom = from;
            mTo = to;
        }

        /// <summary>
        /// Initializes the range for the actual number of pages in the document.
        /// </summary>
        /// <param name="documentPageCount">Actual number of pages in the document.</param>
        /// <remarks>
        /// Replaces the references to maximum page indices (<see cref="int.MaxValue"/>) with 
        /// the actual index of the last page in the document specified.
        /// Truncates the ending page index if it exceeds the index of the last page in the document.
        /// Pages indices are also validated.
        /// </remarks>
        internal PageRange Initialize(int documentPageCount)
        {
            if ((documentPageCount < 1) || (documentPageCount == int.MaxValue))
                throw new ArgumentOutOfRangeException("documentPageCount");

            int maxPageIndex = documentPageCount - 1;
            int from = mFrom;
            int to = mTo;
            
            if (from == int.MaxValue)
                from = maxPageIndex;

            if (to > maxPageIndex)
                to = maxPageIndex;

            if (from > maxPageIndex)
                throw new InvalidOperationException(PageCountExceeded);

            if (to < from)
                throw new InvalidOperationException(ToLessThanFromMessage);
            
            return new PageRange(from, to);
        }

        /// <summary>
        /// Gets the starting page zero-based index.
        /// </summary>
        internal int From
        {
            get { return mFrom; }
        }

        /// <summary>
        /// Gets the ending page zero-based index.
        /// <see cref="int.MaxValue"/> means the last page in the document.
        /// </summary>
        internal int To
        {
            get { return mTo; }
        }

        /// <summary>
        /// Gets pages count.
        /// </summary>
        internal int Count
        {
            get
            {
                return mTo - mFrom + 1;
            }
        }

        /// <summary>
        /// Gets the even pages count.
        /// </summary>
        /// <remarks>
        /// <p>Even pages have odd indices since page indices are zero-based.</p>
        /// </remarks>
        internal int CountEven
        {
            get
            {
                // int.MaxValue is not expected here. It should be resolved via Initialize() first.
                int countEven = (mTo - mFrom) / 2;

                // if either 'From' or 'To' is even (the actual indices are odd since they are zero-based).
                if (mFrom % 2 != 0 || mTo % 2 != 0)
                    countEven++;

                return countEven;
            }
        }

        /// <summary>
        /// Gets the odd pages count.
        /// </summary>
        /// <remarks>
        /// <p>Odd pages have even indices since page indices are zero-based.</p>
        /// </remarks>        
        internal int CountOdd
        {
            get
            {
                // int.MaxValue is not expected here. It should be resolved via Initialize() first.
                return Count - CountEven;
            }
        }

        // int.MaxValue is the last page marker. 
        // The range must be initialized with a valid number of document pages before being used.
        private readonly int mFrom;
        private readonly int mTo;

        private const string NegativePageIndexMessage = "Page index must be positive.";
        private const string ToLessThanFromMessage = "Index of the last page in range must not be less than the index of the first page.";
        private const string PageCountExceeded = "Index of the first page in range must be less than document page count.";

#if DEBUG
        public override string ToString()
        {
            return string.Format("from {0} to {1}", mFrom, mTo);
        }
#endif
    }
}
