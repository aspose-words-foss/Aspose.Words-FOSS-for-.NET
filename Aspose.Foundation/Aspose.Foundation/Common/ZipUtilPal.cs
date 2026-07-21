// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2006 by Roman Korchagin

using System;
using System.IO;

using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Zip;

namespace Aspose.Common
{
    /// <summary>
    /// Utility functions that simplify work with the zip library.
    /// </summary>
    [JavaManual("Platform abstraction for the ZIP library. Manual porting by design.")]
    public static class ZipUtilPal
    {
        /// <summary>
        /// Checks a stream to see if it contains a valid zip archive.
        /// </summary>
        /// <param name="stream">The stream to check.</param>
        /// <returns>true if the stream contains a valid zip archive.</returns>
        public static bool IsZipFile(Stream stream)
        {
            return ZipFile.IsZipFile(stream, false);
        }


        /// <summary>
        /// Decompresses data that was compressed using the Deflate or Zlib compression.
        /// </summary>
        /// <param name="srcBytes">The compressed data.</param>
        /// <param name="uncompressedLength">The size of the data when decompressed. If you don't know, just pass zero.</param>
        /// <param name="method">Deflation method that was used to deflate the data
        /// supposed to be inflated. One of the <see cref="ZipMethod"/></param>
        /// <returns>Decompressed data.</returns>
        public static byte[] Inflate(byte[] srcBytes, int uncompressedLength, ZipMethod method)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBytes))
                return Inflate(srcStream, uncompressedLength, method);
        }

        /// <summary>
        /// Decompresses data that was compressed using the Deflate or Zlib compression.
        /// </summary>
        /// <param name="srcStream">The compressed data.</param>
        /// <param name="uncompressedLength">The size of the data when decompressed. If you don't know, just pass zero.</param>
        /// <param name="method">Deflation method that was used to deflate the data
        /// supposed to be inflated. One of the <see cref="ZipMethod"/></param>
        /// <returns>Decompressed data.</returns>
        public static byte[] Inflate(Stream srcStream, int uncompressedLength, ZipMethod method)
        {
            if (uncompressedLength == 0)
                uncompressedLength = (int)srcStream.Length;
            MemoryStream dstStream = new MemoryStream(uncompressedLength);

            using (Stream inflaterStream = CreateInflaterOrDeflaterStream(method, srcStream, CompressionMode.Decompress))
                StreamUtil.CopyStream(inflaterStream, dstStream);

            return dstStream.ToArray();
        }
        
        /// <summary>
        /// Compresses a byte array using the Deflate or Zlib compression.
        /// </summary>
        /// <param name="srcBytes">The original data.</param>
        /// <param name="method">Deflation method to be used. One of the <see cref="ZipMethod"/></param>
        /// <returns>The compressed data.</returns>
        public static byte[] Deflate(byte[] srcBytes, ZipMethod method)
        {
            using (MemoryStream srcStream = new MemoryStream(srcBytes))
                return Deflate(srcStream, method);
        }

        /// <summary>
        /// Compresses a stream using the Deflate, Zlib or GZip compression.
        /// Suitable, for example, for compressing EMF and WMF to WMZ.
        /// </summary>
        /// <param name="srcStream">The original data.</param>
        /// <param name="method">Deflation method to be used. One of the <see cref="ZipMethod"/></param>
        /// <returns>The compressed data.</returns>
        public static byte[] Deflate(Stream srcStream, ZipMethod method)
        {
            using (MemoryStream dstStream = new MemoryStream())
            {
                Deflate(srcStream, dstStream, method);
                return dstStream.ToArray();
            }
        }

        /// <summary>
        /// Compresses a stream into a memory stream. 
        /// The destination memory stream can be at any position.
        /// </summary>
        /// <param name="srcStream">The original data.</param>
        /// <param name="dstStream">Where the compressed is written. Written at the current position.</param>
        /// <param name="method">Deflation method to be used. One of the <see cref="ZipMethod"/></param>
        /// <returns>The length of the compressed data.</returns>
        public static int Deflate(Stream srcStream, Stream dstStream, ZipMethod method)
        {
            int startPos = (int)dstStream.Position;

            using (Stream deflaterStream = CreateInflaterOrDeflaterStream(method, dstStream, CompressionMode.Compress))
                StreamUtil.CopyStream(srcStream, deflaterStream);

            return (int)dstStream.Position - startPos;
        }

        public static void Deflate(byte[] bytes, int offset, int count, Stream dstStream, ZipMethod method)
        {
            using (Stream deflaterStream = CreateInflaterOrDeflaterStream(method, dstStream, CompressionMode.Compress))
                deflaterStream.Write(bytes, offset, count);
        }

        private static Stream CreateInflaterOrDeflaterStream(ZipMethod method, Stream stream, CompressionMode compressionMode)
        {
            switch (method)
            {
                case ZipMethod.Deflate:
                    return new DeflateStream(stream, compressionMode, true);
                case ZipMethod.Zlib:
                    return new ZlibStream(stream, compressionMode, true);
                default:
                    throw new InvalidOperationException("Unknown compression method specified.");
            }
        }
    }
}
