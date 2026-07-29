// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2020 by Alexey Noskov

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Allows to specify additional options when loading Pdf document into a <see cref="Document"/> object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    public class PdfLoadOptions : LoadOptions
    {
        /// <summary>
        /// Gets or sets the 0-based index of the first page to read. Default is 0.
        /// </summary>
        public int PageIndex
        {
            get { return mPageIndex; }
            set { mPageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the number of pages to read. Default is MaxValue which means all pages of the document will be read.
        /// </summary>
        public int PageCount
        {
            get { return mPageCount; }
            set { mPageCount = value; }
        }

        /// <summary>
        /// Gets or sets the flag indicating whether images must be skipped while loading PDF document. Default is <c>false</c>.
        /// </summary>
        public bool SkipPdfImages
        {
            get { return mSkipPdfImages; }
            set { mSkipPdfImages = value; }
        }

        private int mPageCount = int.MaxValue;
        private int mPageIndex;
        private bool mSkipPdfImages;
    }
}
