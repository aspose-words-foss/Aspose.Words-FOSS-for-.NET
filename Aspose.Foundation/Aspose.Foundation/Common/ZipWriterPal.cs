// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/07/2010 by Roman Korchagin

using System;
using System.IO;
using Aspose.JavaAttributes;
using Aspose.Zip;

namespace Aspose.Common
{
    /// <summary>
    /// Implement this class in Java manually.
    ///
    /// This class abstracts the ZIP library.
    /// </summary>
    [JavaManual("Platform abstraction for the ZIP library. Manual porting by design.")]
    public sealed class ZipWriterPal : IDisposable
    {
        public ZipWriterPal(Stream dstStream)
            : this (dstStream, DefaultCompressionLevel, DefaultZip64Mode)
        {
        }

        /// <summary>
        /// Creates a new ZipWriterPal object with a specific destination stream and compression level.
        /// </summary>
        public ZipWriterPal(Stream dstStream, ZipCompressionLevel compressionLevel, Zip64Option zip64)
        {
            mZipFile = new ZipFile();
            mZipFile.UseZip64WhenSaving = zip64;
            mZipFile.CompressionLevel = (CompressionLevel)compressionLevel;
            mDstStream = dstStream;
        }

        public void Dispose()
        {
            mZipFile.Dispose();
        }

        /// <summary>
        /// Adds a new entry to the zip archive.
        /// The caller needs to keep the entry stream open until your call <see cref="Finish"/>.
        /// The caller is responsible for disposing the entry stream after that.
        /// </summary>
        public void AddEntry(string fileName, Stream srcStream)
        {
            AddEntry(fileName, srcStream, DateTime.MinValue);
        }

        /// <summary>
        /// Adds a new entry to the zip archive.
        /// If useEntryDateTime is true, then entryDateTime will be used for the LastModified property of the entry.
        /// </summary>
        internal void AddEntry(string fileName, Stream srcStream, DateTime entryDateTime)
        {
            ZipEntry entry = mZipFile.AddEntry(fileName, null, srcStream);

            // This condition means that entryDateTime wasn't set.
            if (!DateTime.MinValue.Equals(entryDateTime))
            {
                entry.LastModified = new DateTime(entryDateTime.ToUniversalTime().Ticks, DateTimeKind.Local);
            }
#if CPLUSPLUS
            else
            {
                // C++ ZipFile implementation know nothing about our test mode
                // Let's initialize it the same way as C# (and as tests expects)
                entry.LastModified = DateTimeUtil.GetNow().ToUniversalTime();
            }
#endif
        }

        /// <summary>
        /// Adds a stored (uncompressed) entry to the zip archive.
        /// </summary>
        public void AddStoredEntry(string fileName, byte[] data)
        {
            ZipEntry entry = mZipFile.AddEntry(fileName, null, data);
            entry.CompressionMethod = 0x00; // No compression.
        }

        /// <summary>
        /// Finalizes writing of the zip archive.
        /// </summary>
        public void Finish()
        {
            mZipFile.Save(mDstStream);
        }

        internal const ZipCompressionLevel DefaultCompressionLevel = ZipCompressionLevel.Level5;
        internal const Zip64Option DefaultZip64Mode = Zip64Option.Never;

        /// <summary>
        /// Although <see cref="ZipFile"/> is IDisposable, it is not responsible for any resources in our case hence we don't dispose it.
        /// </summary>
        private readonly ZipFile mZipFile;
        private readonly Stream mDstStream;
    }
}
