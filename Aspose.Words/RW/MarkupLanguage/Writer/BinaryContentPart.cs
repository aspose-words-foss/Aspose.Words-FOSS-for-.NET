// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2014 by Alexey Butalov

using System.IO;
using System.Text;

namespace Aspose.Words.RW.MarkupLanguage.Writer
{
    /// <summary>
    /// Binary subsidiary content.
    /// </summary>
    internal class BinaryContentPart : SubsidiaryContentPart
    {
        internal BinaryContentPart(
            string contentType,
            string uri,
            byte[] data)
            : this(null, contentType, uri, data, 0, data.Length)
        {
            // Empty constructor.
        }

        internal BinaryContentPart(
            string contentType,
            string uri,
            MemoryStream stream)
            : this(null, contentType, uri, stream)
        {
            // Empty constructor.
        }

        internal BinaryContentPart(
            string contentType,
            string uri,
            byte[] data,
            int offset,
            int length)
            : this(null, contentType, uri, data, offset, length)
        {
            // Empty constructor.
        }

        internal BinaryContentPart(
            Encoding textEncoding,
            string contentType,
            string uri,
            MemoryStream stream)
            : this(textEncoding, contentType, uri, stream.GetBuffer(), 0, stream.Length)
        {
            // Empty constructor.
        }

        internal BinaryContentPart(
            Encoding textEncoding,
            string contentType,
            string uri,
            byte[] data,
            int offset,
            long length)
            : base(textEncoding, contentType, uri)
        {
            Debug.Assert(data != null);
            Debug.Assert(offset >= 0);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= int.MaxValue);
            Debug.Assert((offset + length) <= data.Length);

            mData = data;
            mOffset = offset;
            mLength = (int)length;
        }

        internal override MemoryStream CreateStream()
        {
            // Return a read-only stream. However, we keep the base array publicly visible, because it was publicly visible
            // when this class was created.
            return new MemoryStream(mData, mOffset, mLength, false, true);
        }

        private readonly byte[] mData;
        private readonly int mOffset;
        private readonly int mLength;
    }
}
