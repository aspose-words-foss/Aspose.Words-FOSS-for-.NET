// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2019 by Ilya Navrotskiy

using System.IO;
using System.Text;
using Aspose.Words.Loading;
using Aspose.Words.RW.Txt.Reader;

namespace Aspose.Words.RW.Markdown.Reader
{
    /// <summary>
    /// Imports markdown file into a Document object.
    /// </summary>
    internal class MarkdownReader : IDocumentReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal MarkdownReader(Stream stream, LoadOptions loadOptions, FileFormatInfo fileFormatInfo, Document document)
        {
            Debug.Assert(stream != null);
            Debug.Assert(document != null);

            Encoding encoding = TxtReader.GetEncoding(loadOptions, fileFormatInfo);
            mStreamReader = new StreamReader(stream, encoding);

            mLoadOptions = loadOptions as MarkdownLoadOptions;
            if (mLoadOptions == null)
                mLoadOptions = new MarkdownLoadOptions(loadOptions);

            mContext = new MarkdownReaderContext(document);
            mDocument = document;
        }

        /// <summary>
        /// Reads markdown file into the model.
        /// </summary>
        public void Read()
        {
            MarkdownDocument markdownDocument = new MarkdownDocument(mLoadOptions);
            LoadingProgressProcessor progressProcessor = new LoadingProgressProcessor(mDocument, mLoadOptions);

            markdownDocument.Read(mStreamReader, progressProcessor);
            markdownDocument.Write(mContext);
        }

        #region IDocumentReader implementation

        public Stream Decrypt()
        {
            Debug.Assert(false, "Not supported");
            return null;
        }

        public bool IsEncrypted
        {
            get { return false; }
        }

        #endregion

        private readonly StreamReader mStreamReader;
        private readonly MarkdownReaderContext mContext;
        private readonly Document mDocument;
        private readonly MarkdownLoadOptions mLoadOptions;
    }
}
