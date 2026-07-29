// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2016 by Andrey Noskov

using System.IO;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Provides data for the <see cref="IPageSavingCallback.PageSaving"/> event.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    public class PageSavingArgs
    {
        /// <summary>
        /// Set current page index.
        /// </summary>
        internal void SetPageIndex(int pageIndex)
        {
            mPageIndex = pageIndex;
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal UserStreamWrapper CreateUserStreamWrapper()
        {
            return new UserStreamWrapper(mPageStream, mKeepPageStreamOpen);
        }

        /// <summary>
        /// Allows to specify the stream where the document page will be saved to.
        /// </summary>
        /// <remarks>
        /// <para>This property allows you to save document pages to streams instead of files.</para>
        /// 
        /// <para>The default value is <c>null</c>. When this property is <c>null</c>, the document page 
        /// will be saved to a file specified in the <see cref="PageFileName"/> property.</para>
        /// <para>If both <see cref="PageStream"/> and <see cref="PageFileName"/> are set, then PageStream will be used.</para>
        /// <seealso cref="PageFileName"/>
        /// <seealso cref="KeepPageStreamOpen"/>
        /// </remarks>
        /// <javaName>PageStream(java.io.OutputStream)</javaName>
        [JavaAttributes.JavaUseSecondApiChangeMap]
        [CppIOStreamWrapper(IOStreamType.OStream)]
        public Stream PageStream
        {
            get { return mPageStream; }
            set { mPageStream = value; }
        }

        /// <summary>
        /// Specifies whether Aspose.Words should keep the stream open or close it after saving a document page.
        /// </summary>
        /// <remarks>
        /// <para>Default is <c>false</c> and Aspose.Words will close the stream you provided
        /// in the <see cref="PageStream"/> property after writing a document page into it.
        /// Specify <c>true</c> to keep the stream open.</para>
        /// 
        /// <seealso cref="PageStream"/>
        /// </remarks>
        public bool KeepPageStreamOpen
        {
            get { return mKeepPageStreamOpen; }
            set { mKeepPageStreamOpen = value; }
        }

        /// <summary>
        /// Gets or sets the file name where the document page will be saved to.
        /// </summary>
        /// <remarks>
        /// If not specified then page file name and path will be generated automatically using original file name.
        /// </remarks>
        public string PageFileName
        {
            get { return mPageFileName; }
            set { mPageFileName = value; }
        }

        /// <summary>
        /// Current page index.
        /// </summary>
        public int PageIndex
        {
            get { return mPageIndex; }
        }

        /// <summary>
        /// Exists to make calling code autoportable.
        /// </summary>
        internal bool HasUserStream
        {
            get { return mPageStream != null; }
        }

        private Stream mPageStream;
        private string mPageFileName;
        private int mPageIndex;
        private bool mKeepPageStreamOpen;
    }
}
