// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2008 by Viktor Sazhaev

using System.IO;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose.Words.RW.MarkupLanguage.Writer
{
    /// <summary>
    /// One part of document subsidiary content.
    /// </summary>
    internal abstract class SubsidiaryContentPart
    {
        protected SubsidiaryContentPart(Encoding textEncoding, string contentType, string uri)
        {
            mTextEncoding = textEncoding;
            mContentType = contentType;
            mUri = uri;
        }

        /// <summary>
        /// Creates a stream to access content. Should be disposed.
        /// </summary>
        [JavaThrows(true)]  // IO Exceptions.
        internal abstract MemoryStream CreateStream();

        /// <summary>
        /// Indicates whether subsidiary part should be output as text.
        /// Is is assumed that for any text part encoding is known.
        /// </summary>
        internal bool IsText
        {
            get { return mTextEncoding != null; }
        }

        /// <summary>
        /// Returns text encoding. Applicable for text parts.
        /// </summary>
        internal Encoding TextEncoding
        {
            get { return mTextEncoding; }
        }

        /// <summary>
        /// Returns content type
        /// </summary>
        internal string ContentType
        {
            get { return mContentType; }
        }

        /// <summary>
        /// Returns URI
        /// </summary>
        internal string Uri
        {
            get { return mUri; }
        }

        private readonly Encoding mTextEncoding;
        private readonly string mContentType;
        private readonly string mUri;
    }
}
