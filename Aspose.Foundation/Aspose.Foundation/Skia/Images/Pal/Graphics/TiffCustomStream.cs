// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2024 by Denis Panov

#if NETSTANDARD || NET
using System.IO;
using BitMiracle.LibTiff.Classic;

namespace Aspose.Skia.Images.Pal.Graphics
{
    /// <summary>
    /// Class used by the LibTiff for TIFF reading and writing.
    /// </summary>
    /// <remarks>
    /// Overridden methods from <see cref="BitMiracle.LibTiff.Classic.TiffStream"/>.
    /// Differences from the original implementation:
    /// -Casting operations removed
    /// -Close method should not close the stream
    /// </remarks>
    internal class TiffCustomStream : TiffStream
    {
        public TiffCustomStream(Stream imageData)
        {
            mImageData = imageData;
        }

        public override int Read(object clientData, byte[] buffer, int offset, int count)
        {
            return mImageData.Read(buffer, offset, count);
        }

        public override void Write(object clientData, byte[] buffer, int offset, int count)
        {
            mImageData.Write(buffer, offset, count);
        }

        public override long Seek(object clientData, long offset, SeekOrigin origin)
        {
            if (offset == -1)
            {
                return -1L;
            }

            return mImageData.Seek(offset, origin);
        }

        public override void Close(object clientData)
        {
            // Don't close the stream because it's managed by AW.
        }

        public override long Size(object clientData)
        {
            return mImageData.Length;
        }

        private readonly Stream mImageData;
    }
}
#endif
