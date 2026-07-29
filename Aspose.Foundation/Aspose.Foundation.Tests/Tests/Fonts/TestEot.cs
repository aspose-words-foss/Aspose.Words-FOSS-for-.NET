// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using System.IO;
using Aspose.Collections;
using Aspose.Fonts.EmbeddedOpenType;
using Aspose.Fonts.EmbeddedOpenType.Ctf;
using Aspose.Fonts.EmbeddedOpenType.LzComp;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    [TestFixture]
    public class TestEot
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestFxUtil.SetUpTests();
            Directory.CreateDirectory(TestFxUtil.GetInTestOutPath("TrueType"));
        }

        [Test]
        public void TestUncompressedEot()
        {
            string fontPath = TestFxUtil.BuildTestFileName(@"TrueType\ROC.EOT");

            byte[] eotData = StreamUtil.CopyFileToByteArray(fontPath);
            byte[] openTypeData = EotFontFiler.TryExtractOpenTypeFromEot(eotData);

            TTFont font = TTFontBuilder.ReadOpenType(openTypeData);

            Assert.That(font.FamilyName, Is.EqualTo("Rockwell"));
        }

        [Test]
        public void TestCompressedEot()
        {
            string fontPath = TestFxUtil.BuildTestFileName(@"TrueType\Calibri.eot");

            byte[] eotData = StreamUtil.CopyFileToByteArray(fontPath);
            byte[] openTypeData = EotFontFiler.TryExtractOpenTypeFromEot(eotData);

            TTFont font = TTFontBuilder.ReadOpenType(openTypeData);

            Assert.That(font.FamilyName, Is.EqualTo("Calibri"));
        }

        /// <summary>
        /// Tests that compression works for simple file.
        /// </summary>
        [Test]
        public void TestLzCompSimpleFileCompression()
        {
            VerifyLzCompCompression(
                TestFxUtil.BuildTestFileName(@"TrueType\SimpleFile.txt"),
                TestFxUtil.BuildTestFileName(@"TrueType\SimpleFile.lzcomp"));
        }

        /// <summary>
        /// Tests that decompression works for simple file.
        /// File has been compressed using original C code.
        /// </summary>
        [Test]
        public void TestLzCompSimpleFileDecompression()
        {
            VerifyLzCompDecompression(
                TestFxUtil.BuildTestFileName(@"TrueType\SimpleFile.lzcomp"),
                TestFxUtil.BuildTestFileName(@"TrueType\SimpleFile.txt"));
        }

        /// <summary>
        /// Tests that compression works with Rle enabled.
        /// </summary>
        [Test]
        public void TestLzCompRleCompression()
        {
            VerifyLzCompCompression(
                TestFxUtil.BuildTestFileName(@"TrueType\Rle.txt"),
                TestFxUtil.BuildTestFileName(@"TrueType\Rle.lzcomp"));
        }

        /// <summary>
        /// Tests that decompression works with Rle enabled.
        /// File has been compressed using original C code.
        /// </summary>
        [Test]
        public void TestLzCompRleDecompression()
        {
            VerifyLzCompDecompression(
                TestFxUtil.BuildTestFileName(@"TrueType\Rle.lzcomp"),
                TestFxUtil.BuildTestFileName(@"TrueType\Rle.txt"));
        }

        /// <summary>
        /// Tests that decompression works for compressed Calibri font.
        /// File has been compressed using MS Word font embedding function and decompressed
        /// using original C code.
        /// </summary>
        [TestCase(@"TrueType\CalibriBlock1.lzcomp", @"TrueType\CalibriBlock1.bin")]
        [TestCase(@"TrueType\CalibriBlock2.lzcomp", @"TrueType\CalibriBlock2.bin")]
        [TestCase(@"TrueType\CalibriBlock3.lzcomp", @"TrueType\CalibriBlock3.bin")]
        public void TestLzCompCalibri(string compressedFile, string decompressedFile)
        {
            VerifyLzCompDecompression( TestFxUtil.BuildTestFileName(compressedFile), TestFxUtil.BuildTestFileName(decompressedFile));
        }

        [Test]
        public void TestMagnitudeReader()
        {
            // These values are taken from MTX specification.
            byte[] data = new byte[] {0x90, 0x48, 0x84};
            short[] expectedResult = new short[] {0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0};

            using (MemoryStream stream = new MemoryStream(data))
            {
                MagnitudeReader reader = new MagnitudeReader(stream);

                for (int i = 0; i < expectedResult.Length; i++)
                {
                    Assert.That(reader.ReadValue(), Is.EqualTo(expectedResult[i]));
                }
            }
        }

        /// <summary>
        /// Tests <see cref="MagnitudeWriter"/>.
        /// </summary>
        [Test]
        public void TestMagnitudeWriter()
        {
            // These values are taken from MTX specification.
            // As there is just a code snippet, we should extend it with values which are assumed throughout a fragment of codes.
            short[] data = new short[] { 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1};
            byte[] expectedResult = new byte[] { 0x90, 0x48, 0x84, 0x00 };

            using (MemoryStream stream = new MemoryStream())
            {
                MagnitudeWriter writer = new MagnitudeWriter(stream);
                for (int i = 0; i < data.Length; i++)
                    writer.WriteValue(data[i]);
                writer.Flush();

                Assert.That(stream.ToArray(), Is.EqualTo(expectedResult));
            }
        }

        /// <summary>
        /// This is additional test for <see cref="MagnitudeReader"/> and <see cref="MagnitudeWriter"/>
        /// that verifies round-trip with a various values.
        /// </summary>
        [Test]
        public void TestMagnitudeRoundtrip()
        {
            short[] testValues = { 0, 1, 2, 3, -1, -2, -3, 8, -8, 9, -9, 20, -20, 127, -127, 128, -128, 32767, -32767, 0x90, 0x48, 0x84 };

            using (MemoryStream writeStream = new MemoryStream())
            {
                // Write test values to a stream.
                MagnitudeWriter writer = new MagnitudeWriter(writeStream);
                for (int i = 0; i < testValues.Length; i++)
                    writer.WriteValue(testValues[i]);
                writer.Flush();

                using (MemoryStream readStream = new MemoryStream(writeStream.ToArray()))
                {
                    MagnitudeReader reader = new MagnitudeReader(readStream);
                    // Read values and compare them with the original values.
                    for (int i = 0; i < testValues.Length; i++)
                        Assert.That(reader.ReadValue(), Is.EqualTo(testValues[i]));

                    Assert.That(readStream.Length, Is.EqualTo(readStream.Position));
                }
            }
        }

        /// <summary>
        /// Tests CVT table coding round-trip.
        /// </summary>
        [Test]
        public void TestCvtRoundtrip()
        {
            byte[] originalCvt = {0x20, 0xA8, 0xD9, 0x10, 0xAA, 0xBB, 0xFE, 0xEE, 0xEE, 0x01, 0xEE, 0xFF, 0xEE, 0xFE, 0xFF, 0xFF, 0xFA, 0xFA};

            byte[] encodedCvt = CtfEncoder.EncodeCvtTable(originalCvt);
            byte[] decodedCvt = CtfDecoder.DecodeCvtTable(encodedCvt);

            Assert.That(decodedCvt, Is.EqualTo(originalCvt));
        }

        /// <summary>
        /// Tests CVT table encoding.
        /// </summary>
        [TestCase("segoeui")]
        [TestCase("arial")]
        [TestCase("angsau")]
        [TestCase("symbol")]
        [TestCase("tahoma")]
        [TestCase("wingding")]
        [TestCase("MATURASC")]
        [TestCase("ActionMan.Regular")]
        [TestCase("ActionMan.Bold")]
        [TestCase("ActionMan.Italic")]
        [TestCase("ActionMan.BoldItalic")]
        public void TestCvtEncoding(string fontName)
        {
            string ttfFile = BuildFontFileName(fontName, "ttf");
            using (Stream stream = File.OpenRead(ttfFile))
            {
                SfntReader fontTablesReader = new SfntReader(stream);
                fontTablesReader.ReadHeader();


                byte[] uncompressedCvt = fontTablesReader.ReadTable(OpenTypeTableTag.Cvt);
                byte[] encodedCvt = CtfEncoder.EncodeCvtTable(uncompressedCvt);

                CheckFontData(fontName, "eot.cvt", encodedCvt);
            }
        }

        /// <summary>
        /// Tests HDMX table encoding.
        /// </summary>
        [TestCase("segoeui")]
        [TestCase("LiteEuro")]
        [TestCase("ActionMan.Regular")]
        [TestCase("ActionMan.Bold")]
        [TestCase("ActionMan.Italic")]
        [TestCase("ActionMan.BoldItalic")]
        public void TestHdmxEncoding(string fontName)
        {
            string ttfFile = BuildFontFileName(fontName, "ttf");
            using (Stream stream = File.OpenRead(ttfFile))
            {
                SfntReader fontTablesReader = new SfntReader(stream);
                fontTablesReader.ReadHeader();

                // Read all helper tables that are needed for encoding HDMX table.
                fontTablesReader.SeekToTable(OpenTypeTableTag.Head);
                FontHeader head = FontHeader.Read(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Maxp);
                MaximumProfile maxp = new MaximumProfile(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Hhea);
                HorizontalHeader hhea = new HorizontalHeader(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Hmtx);
                HorizontalMetrics hmtx = HorizontalMetrics.Read(fontTablesReader.BinaryReader, hhea.NumberOfHMetrics, maxp.NumGlyphs);

                byte[] uncompressedHdmx = fontTablesReader.ReadTable(OpenTypeTableTag.Hdmx);
                byte[] encodedHdmx = CtfEncoder.EncodeHdmxTable(uncompressedHdmx, maxp.NumGlyphs, head.UnitsPerEm, hmtx);

                CheckFontData(fontName, "eot.hdmx", encodedHdmx);
            }
        }

        /// <summary>
        /// Tests VDMX table encoding.
        /// </summary>
        [TestCase("arial")] // This font contains groups that refers to the different offsets in table.
        [TestCase("symbol")] // This font contains groups that refers to the same offset in table.
        [TestCase("segoeui")]
        public void TestVdmxEncoding(string fontName)
        {
            string ttfFile = BuildFontFileName(fontName, "ttf");
            using (Stream stream = File.OpenRead(ttfFile))
            {
                SfntReader fontTablesReader = new SfntReader(stream);
                fontTablesReader.ReadHeader();

                byte[] uncompressedVdmx = fontTablesReader.ReadTable(OpenTypeTableTag.Vdmx);
                byte[] encodedVdmx = CtfEncoder.EncodeVdmxTable(uncompressedVdmx);

                CheckFontData(fontName, "eot.vdmx", encodedVdmx);
            }
        }

        /// <summary>
        /// Tests CTF encoding of glyph table.
        /// </summary>
        [TestCase("segoeui")]
        [TestCase("ActionMan.Regular")]
        [TestCase("ActionMan.Bold")]
        [TestCase("ActionMan.Italic")]
        [TestCase("ActionMan.BoldItalic")]
        public void TestGlyphTableCtfEncoding(string fontName)
        {
            string ttfFile = BuildFontFileName(fontName, "ttf");

            using (Stream stream = File.OpenRead(TestFxUtil.BuildTestFileName(ttfFile)))
            {
                SfntReader fontTablesReader = new SfntReader(stream);
                fontTablesReader.ReadHeader();
                fontTablesReader.SeekToTable(OpenTypeTableTag.Head);
                FontHeader head = FontHeader.Read(fontTablesReader.BinaryReader);

                CtfEncodedData ctfEncodedData = CtfGlyphEncoder.EncodeGlyphData(fontTablesReader, head.IsLocaShort);

                CheckFontData(fontName, "eot.GlyphTable", ctfEncodedData.FontTable);
                CheckFontData(fontName, "eot.PushData", ctfEncodedData.PushData);
                CheckFontData(fontName, "eot.Instructions", ctfEncodedData.Instructions);
            }
        }

        /// <summary>
        /// Tests round-trip of push data.
        /// </summary>
        [Test]
        public void TestPushData()
        {
            IntList originalValues = new IntList(new int[]
                {0, 1, 250, 251, 252, 253, 254, 255, 256, 506, 507, 508, 509, 761, 762, 32767,
                    -1,-250, -251, -252, -253, 254, -255, -256, -506, -507, -508, -509, -761, -762, -32768,
                    5, 6, 5, 7, 5, 8, 2, // Hop3Code
                    2, 7, 2, 5, 2, 8, 2, 9, 2, 3, //Hop4Code
                    23, 45, 23, 54, 23, 67, 23, 34, 23, 76, 23}); // Hop3Code 2 times

            using (MemoryStream encodedStream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(encodedStream);
                CtfGlyphEncoder.EncodePushData(originalValues, writer);

                encodedStream.Position = 0;
                BigEndianBinaryReader reader = new BigEndianBinaryReader(encodedStream);
                short[] decodedValues = CtfGlyphDecoder.DecodePushData(reader, (ushort)originalValues.Count);

                Assert.That(decodedValues.Length, Is.EqualTo(originalValues.Count));
                for (int i = 0; i < originalValues.Count; i++)
                    Assert.That(decodedValues[i], Is.EqualTo(originalValues[i]));
            }
        }

        /// <summary>
        /// Tests 255UShort values round-trip.
        /// </summary>
        [Test]
        public void Test255UShort()
        {
            int [] originalValues = {0, 1, 253, 250, 254, 255, 256, 506, 507, 508, 509, 761, 762, 65535};

            using (MemoryStream encodedStream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(encodedStream);
                for (int i = 0; i < originalValues.Length; i++)
                    UShort255.Write(originalValues[i], writer);

                int[] decodedValues = new int[originalValues.Length];
                encodedStream.Position = 0;
                BigEndianBinaryReader reader = new BigEndianBinaryReader(encodedStream);
                for (int i = 0; i < originalValues.Length; i++)
                    decodedValues[i] = UShort255.Read(reader);

                Assert.That(decodedValues, Is.EqualTo(originalValues));
            }
        }

        /// <summary>
        /// Tests 255Short values round-trip.
        /// </summary>
        [Test]
        public void Test255Short()
        {
            short[] originalValues =
                {0, 1, 253, 250, 254, 255, 256, 506, 507, 508, 509, 761, 762, 32767,
                -1, -253, -250, -254, -255, -256, -506, -507, -508, -509, -761, -762, -32768};

            using (MemoryStream encodedStream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(encodedStream);
                for (int i = 0; i < originalValues.Length; i++)
                    Short255.Write(originalValues[i], writer);

                short[] decodedValues = new short[originalValues.Length];
                encodedStream.Position = 0;
                BigEndianBinaryReader reader = new BigEndianBinaryReader(encodedStream);
                for (int i = 0; i < originalValues.Length; i++)
                    decodedValues[i] = Short255.Read(reader);

                Assert.That(decodedValues, Is.EqualTo(originalValues));
            }
        }

        /// <summary>
        /// Checks specified font data equality to the specified expected font data array.
        /// </summary>
        private static void CheckFontData(string expectedFontDataName, string expectedFontDataExtension, byte[] fontData)
        {
            string expectedFontDataFile = BuildFontFileName(expectedFontDataName, expectedFontDataExtension);
            byte[] expectedFontData = StreamUtil.CopyFileToByteArray(expectedFontDataFile);
            Assert.That(fontData, Is.EqualTo(expectedFontData));
        }

        /// <summary>
        /// Verifies decompression with LzComp.
        /// </summary>
        private void VerifyLzCompDecompression(string compressedFile, string decompressedFile)
        {
            string outFilePath = TestFxUtil.BuildOutFileName(@"TrueType\LzCompDecompressionReslut", "", "");
            byte[] compressedData = StreamUtil.CopyFileToByteArray(compressedFile);
            byte[] uncompressedData = LzCompDecoder.UnpackData(compressedData);

            using (Stream file = File.Open(outFilePath, FileMode.Create))
            {
                file.Write(uncompressedData, 0, uncompressedData.Length);
            }

            Assert.That(TestFxUtil.CompareFiles(decompressedFile, outFilePath), Is.True);
        }

        /// <summary>
        /// Verifies compression with LzComp.
        /// </summary>
        private void VerifyLzCompCompression(string uncompressedFile, string compressedFile)
        {
            string outFilePath = TestFxUtil.BuildOutFileName(@"TrueType\LzCompCompressionReslut", "", "");
            byte[] uncompressedData = StreamUtil.CopyFileToByteArray(uncompressedFile);
            byte[] compressedData = LzCompEncoder.PackData(uncompressedData);

            using (Stream file = File.Open(outFilePath, FileMode.Create))
            {
                file.Write(compressedData, 0, compressedData.Length);
            }

            Assert.That(TestFxUtil.CompareFiles(compressedFile, outFilePath), Is.True);
        }

        /// <summary>
        /// Returns font file name constructed from the specified font file name and extension
        /// relative to the 'TestData\TrueType\EotFonts\' folder.
        /// </summary>
        private static string BuildFontFileName(string fontFileNameWithoutExtension, string fontExtension)
        {
            return TestFxUtil.BuildTestFileName(string.Format("TrueType\\EotFonts\\{0}.{1}", fontFileNameWithoutExtension, fontExtension));
        }
    }
}
