// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/03/2020 by Alexander Sevidov

using System.IO;
using System.Text;
using Aspose.IO;

namespace Aspose.Common
{
    /// <summary>
    /// This class implements working with compressed data.
    /// </summary>
    /// <remarks>
    /// This is first step to handle EMF images compressed.
    /// Currently it solves WORDSNET-18977 and gives significant performance boost.
    /// </remarks>
    public static class CompressedData
    {
        /// <summary>
        /// Creates data chunk marked as compressed.
        /// </summary>
        /// <remarks>
        /// Adds a required for decompression header to the beginning of the chunk.
        /// </remarks>
        public static byte[] Create(byte[] compressedData, long length, bool isCompressed)
        {
            const int zipEntryHeaderLen = 13;

            int headerSize = CompressedDataSignature.Length + zipEntryHeaderLen;

            using (MemoryStream stream = new MemoryStream(headerSize + compressedData.Length))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(Encoding.ASCII.GetBytes(CompressedDataSignature));
                    writer.Write(length);
                    writer.Write(isCompressed);
                    writer.Write(compressedData.Length);
                    writer.Write(compressedData);

                    return stream.ToArray();
                }
            }
        }

        /// <summary>
        /// Returns decompressed bytes if compressed, or returns bytes itself otherwise.
        /// </summary>
        [JavaAttributes.JavaConvertCheckedExceptions]
        public static byte[] GetData(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                if (!IsCompressedData(stream))
                    return bytes;
            }

            using (MemoryStream stream = new MemoryStream(bytes, 
                CompressedDataSignature.Length, bytes.Length - CompressedDataSignature.Length))
            {
                using (MemoryStream zipStream = ZipReaderPal.LoadCompressedEntryToMemory(stream))
                {
                    return zipStream.ToArray();
                }
            }
        }

        /// <summary>
        /// If compressed returns <see cref="ZipStream"/> which allows reading decompressed data 
        /// or returns <paramref name="stream"/> itself otherwise.
        /// </summary>
        public static Stream GetStream(Stream stream)
        {
            if (!IsCompressedData(stream))
                return stream;

            // Otherwise create ZipStream to extract data runtime.
            long savedPos = stream.Position;

            stream.Position += CompressedDataSignature.Length;
            MemoryStream zipStream = ZipReaderPal.LoadCompressedEntryToMemory(stream);

            stream.Position = savedPos;

            return zipStream;
        }

        /// <summary>
        /// Returns true when source stream is compressed, false otherwise.
        /// </summary>
        private static bool IsCompressedData(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            if (!StreamUtil.HasEnoughBytesToRead(reader, CompressedDataSignature.Length))
                return false;

            long savedPos = stream.Position;
            string header = Encoding.ASCII.GetString(reader.ReadBytes(CompressedDataSignature.Length));
            stream.Position = savedPos;

            return header == CompressedDataSignature;
        }

        private const string CompressedDataSignature = "{60b5c89b-4108-45a6-8493-673a7dd2ea68}";
    }
}
