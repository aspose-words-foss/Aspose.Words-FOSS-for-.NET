// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2016 by Roman Korchagin

using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.IO;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Pal
{
    /// <summary>
    /// Minimal functionality tests for PalZipXXX classes.
    /// </summary>
    [TestFixture]
    public class TestPalZip
    {
        [Test]
        public void TestReader()
        {
            using (Stream stream = File.OpenRead(TestFxUtil.BuildTestFileName(@"Zip\TestReader.zip")))
            {
                CheckContainsFile1File2(stream);
            }
        }

        [Test]
        public void TestWriter()
        {
            string fileName = TestFxUtil.GetInTestOutPath(@"Zip\TestWriter.zip");
            TestFxUtil.EnsureDirectoryForFileExists(fileName);
            using (Stream stream = File.Create(fileName))
            {
                ZipWriterPal writer = new ZipWriterPal(stream);
                writer.AddEntry("File 1.txt", new MemoryStream(Encoding.ASCII.GetBytes("This is file 1.\r\n")));
                writer.AddStoredEntry("File 2.txt", Encoding.ASCII.GetBytes("Hello World!\r\nThis is file 1.\r\n"));
                writer.Finish();

                // Check the zip that we have created using our own reader code.
                stream.Position = 0;
                CheckContainsFile1File2(stream);
            }
        }

        /// <summary>
        /// This is a "standard" check for zip file contents using ZipReaderPal that we reuse in several tests.
        /// </summary>
        private static void CheckContainsFile1File2(Stream stream)
        {
            ZipReaderPal reader = new ZipReaderPal(stream);

            Assert.That(reader.MoveToNextEntry(), Is.True);
            Assert.That(reader.EntryFileName, Is.EqualTo("File 1.txt"));
            byte[] file1 = reader.LoadEntryToByteArray();
            Assert.That(Encoding.ASCII.GetString(file1), Is.EqualTo("This is file 1.\r\n"));

            Assert.That(reader.MoveToNextEntry(), Is.True);
            Assert.That(reader.EntryFileName, Is.EqualTo("File 2.txt"));
            byte[] file2 = reader.LoadEntryToByteArray();
            Assert.That(Encoding.ASCII.GetString(file2), Is.EqualTo("Hello World!\r\nThis is file 1.\r\n"));

            Assert.That(reader.MoveToNextEntry(), Is.False);
        }

        [Test]
        public void TestDeflate()
        {
            CheckCompressionMethod(ZipMethod.Deflate, 298);
        }

        [Test]
        public void TestZLib()
        {
            CheckCompressionMethod(ZipMethod.Zlib, 304);
        }

        private static void CheckCompressionMethod(ZipMethod compressionMethod, int compressedSize)
        {
            using (Stream srcStream = File.OpenRead(TestFxUtil.BuildTestFileName(@"Zip\TestCompressionMethod.txt")))
            {
                const int UncompressedLength = 536;
                Assert.That(srcStream.Length, Is.EqualTo(UncompressedLength));

                // Check compression makes it expected length.
                MemoryStream dstStream = new MemoryStream();
                int length = ZipUtilPal.Deflate(srcStream, dstStream, compressionMethod);
                Assert.That(length, Is.EqualTo(compressedSize));
                Assert.That(dstStream.Position, Is.EqualTo(compressedSize));
                Assert.That(dstStream.Length, Is.EqualTo(compressedSize));


                // Verify the decompression results in the original data.
                dstStream.Position = 0;
                byte[] uncompressedBytes = ZipUtilPal.Inflate(dstStream, UncompressedLength, compressionMethod);
                byte[] originalBytes = StreamUtil.CopyStreamToByteArray(srcStream);
                Assert.That(ArrayUtil.IsArrayEqual(originalBytes, uncompressedBytes), Is.True);
            }
        }

        [Test]
        public void TestCompressedData()
        {
            using (Stream stream = File.OpenRead(TestFxUtil.BuildTestFileName(@"Zip\TestReader.zip")))
            {
                ZipReaderPal reader = new ZipReaderPal(stream);

                Assert.That(reader.MoveToNextEntry(), Is.True);
                Assert.That(reader.EntryFileName, Is.EqualTo("File 1.txt"));
                byte[] compressedFile1 = reader.LoadCompressedEntryToByteArray();
                byte[] file1 = CompressedData.GetData(compressedFile1);
                Assert.That(Encoding.ASCII.GetString(file1), Is.EqualTo("This is file 1.\r\n"));

                Assert.That(reader.MoveToNextEntry(), Is.True);
                Assert.That(reader.EntryFileName, Is.EqualTo("File 2.txt"));
                byte[] compressedFile2 = reader.LoadCompressedEntryToByteArray();
                byte[] file2 = CompressedData.GetData(compressedFile2);
                Assert.That(Encoding.ASCII.GetString(file2), Is.EqualTo("Hello World!\r\nThis is file 1.\r\n"));

                Assert.That(reader.MoveToNextEntry(), Is.False);
            }
        }
    }
}
