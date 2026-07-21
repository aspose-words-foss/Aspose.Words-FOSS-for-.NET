// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2010 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;

using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Zip;

namespace Aspose.Common
{
    /// <summary>
    /// This class abstracts the ZIP library.
    /// 
    /// Note this class is disposable only for autoporting to Java. This class does not close the stream!
    /// </summary>
    [JavaManual("Platform abstraction for the ZIP library. Manual porting by design.")]
    public sealed class ZipReaderPal : IDisposable
    {
        public ZipReaderPal(Stream stream) : this(stream, null)
        {
        }

        public ZipReaderPal(Stream stream, string password)
        {
            mZipFile = ZipFile.Read(stream);
            mZipFile.Password = password;
            mZipEntriesEnumerator = ((IEnumerable<ZipEntry>)mZipFile).GetEnumerator();
        }

        public ZipReaderPal(string fileName)
        {
            mZipFile = new ZipFile(fileName);
            mZipEntriesEnumerator = ((IEnumerable<ZipEntry>)mZipFile).GetEnumerator();
        }

        public bool MoveToNextEntry()
        {
            return mZipEntriesEnumerator.MoveNext();
        }

        public string EntryFileName
        {
            get { return CurrentEntry.FileName; }
        }

        public short EntryCompressionMethod
        {
            get { return CurrentEntry.CompressionMethod; }
        }

        public static byte[] FileHeaderSignature
        {
            get { return gFileHeaderSignature; }
        }

        /// <summary>
        /// Extract content of the current zip entry into a new memory stream.
        /// The returned stream's position is set to 0.
        /// </summary>
        public MemoryStream LoadEntryToMemory()
        {
            MemoryStream result = new MemoryStream();
            ExtractEntryToStream(result);
            result.Position = 0;
            return result;
        }

        /// <summary>
        /// Extract content of the current zip entry into a new byte array.
        /// </summary>
        public byte[] LoadEntryToByteArray()
        {
            return StreamUtil.CopyStreamToByteArray(LoadEntryToMemory());
        }

        /// <summary>
        /// Extract content of the current zip entry into the specified stream.
        /// </summary>
        public void ExtractEntryToStream(Stream stream)
        {
            CurrentEntry.Extract(stream);
        }

        internal long EntryLength
        {
            get { return CurrentEntry.UncompressedSize; }
        }

        /// <summary>
        /// Loads compressed content of the current zip entry into a new memory stream.
        /// The returned stream should decompress entry while reading the stream.
        /// </summary>
        public MemoryStream LoadCompressedEntryToMemory()
        {
            return new ZipStream(new MemoryStream(GetEntryCompressedBytes()), EntryLength, IsCompressedEntry);
        }

        /// <summary>
        /// Loads compressed content of the current zip entry into a new byte array.
        /// Stores it as a special compressed data chunk which is marked by a header.
        /// </summary>
        public byte[] LoadCompressedEntryToByteArray()
        {
            return CompressedData.Create(GetEntryCompressedBytes(), EntryLength, IsCompressedEntry);
        }

        /// <summary>
        /// Creates a new memory stream which is based on the compressed stream.
        /// The returned stream should decompress data while reading the stream.
        /// </summary>
        /// <remarks>
        /// Returns ZipStream on .Net. 
        /// Since Java hasn't ZipStream, returns already decompressed MemoryStream on Java.
        /// </remarks>
        internal static MemoryStream LoadCompressedEntryToMemory(Stream inputStream)
        {
            try
            {
                BinaryReader reader = new BinaryReader(inputStream);
                long length = reader.ReadInt64();
                bool isCompressed = reader.ReadBoolean();
                int compressedSize = reader.ReadInt32();
                MemoryStream compressedData = new MemoryStream(reader.ReadBytes(compressedSize));
                return new ZipStream(compressedData, length, isCompressed);
            }
            catch (Exception ex)
            {
                if (ex is ZipException)
                    throw;
                else
                    throw new ZipException("Cannot create ZipStream from byte array", ex);
            }
        }

        /// <summary>
        /// Returns LastModified property of the current entry.
        /// </summary>
        internal DateTime EntryLastModified
        {
            get { return CurrentEntry.LastModified; }
        }

        /// <summary>
        /// Returns compressed bytes of the current entry.
        /// </summary>
        private byte[] GetEntryCompressedBytes()
        {
            try
            {
                return CurrentEntry.GetCompressedBytes();
            }
            catch (Exception ex)
            {
                if (ex is ZipException)
                    throw;
                else
                    throw new ZipException("Cannot extract compressed bytes", ex);
            }
        }

        /// <summary>
        /// Returns true if current entry stream is compressed.
        /// </summary>
        private bool IsCompressedEntry
        {
            get { return (CurrentEntry.CompressionMethod == 0x08); }
        }

        private ZipEntry CurrentEntry
        {
            get { return (ZipEntry)mZipEntriesEnumerator.Current; }
        }

        /// <summary>
        /// This method is used in Java only. Leave it here for auto-porting.
        /// </summary>
        public void Dispose()
        {
            // This method is required for autoporting to Java.
            mZipFile.Dispose();
        }

        private readonly ZipFile mZipFile;
        private readonly IEnumerator<ZipEntry> mZipEntriesEnumerator;
        private static readonly byte[] gFileHeaderSignature = new byte[] { 80, 75, 3, 4 };
    }
}
