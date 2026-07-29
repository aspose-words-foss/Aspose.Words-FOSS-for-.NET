// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.IO;
using System.Text;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Covers detection of file format, performing by <see cref="FileFormatUtil"/> class.
    /// </summary>
    [TestFixture]
    public class TestFileFormatUtil
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests that HTML documents with different variants of empty HTML comments are detected correctly.
        /// </summary>
        [TestCase("<!-->")]
        [TestCase("<!--->")]
        [TestCase("<!---->")]
        public void TestDetectHtmlWithEmptyComment(string comment)
        {
            // The empty HTML comment at the beginning wasn't parsed correctly and prevented the HTML detector from
            // reading other HTML tags.
            string html = comment + "<html><p>Text</p></html>";
            CheckHtmlFileFormatDetection(html);
        }

        [Test]
        public void TestDetectMhtml()
        {
            // First header in MIME heading is very long, more than initial buffer size. Will succeed from the second chance.
            CheckDetectFileFormat(@"ImportMhtml\DetectFileFormat\TestDetectFileFormat LongMimeHeading.mht",
                LoadFormat.Mhtml,
                Encoding.ASCII,
                0);
        }




        /// <summary>
        /// WORDSNET-5527 Behavior changed! Now this file is considered as not Html. There are too few Html specific features.
        /// </summary>
        [Test]
        public void TestJira5527B()
        {
            // No Bom, no extended code points - Java thinks this is Utf-8.
            string expectedEncodingName =
#if JAVA
                "utf-8";
#else
                PlatformUtilPal.IsUnixLike() ? "iso-8859-1" : "Windows-1252";
#endif
            // .Net: one tag is _not_ enough for HTML/XML detection.
            // Java: one tag is enough for XML detection. Moreover, we have here 2 tags.
            LoadFormat loadFormat =
#if JAVA
                LoadFormat.Xml;
#else
                LoadFormat.Text;
#endif
            CheckDetectFileFormat(@"ImportHtml\DetectFileFormat\TestDetectFileFormat LongFirstTag.html",
                loadFormat, Encoding.GetEncoding(expectedEncodingName), 0);
        }


        /// <summary>
        /// WORDSNET-7954 Content from Text file is not read during loading into DOM. The issue was in incorrect work of MhtmlDetector
        /// </summary>
        [Test]
        public void TestJira7954()
        {
            string filename = TestUtil.BuildTestFileName("ImportTxt/TestJira7954.txt");
            string expectedEncodingName = PlatformUtilPal.IsUnixLike() ? "iso-8859-2" : "Windows-1250";
            CheckDetectFileFormat(filename, LoadFormat.Text, Encoding.GetEncoding(expectedEncodingName), 0);
        }

        /// <summary>
        /// WORDSNET-5987 Add utility methods FileFormatUtil.ContentTypeToLoadFormat(string contentType),
        /// FileFormatUtil.ContentTypeToSaveFormat(string contentType).
        /// </summary>
        [Test]
        public void TestContentTypeToLoadSaveFormats()
        {
            CheckContentTypeToLoadSaveFormats("image/tiff", LoadFormat.Unknown, SaveFormat.Tiff);
            CheckContentTypeToLoadSaveFormats("image/bmp", LoadFormat.Unknown, SaveFormat.Bmp);
            CheckContentTypeToLoadSaveFormats("image/png", LoadFormat.Unknown, SaveFormat.Png);
            CheckContentTypeToLoadSaveFormats("image/gif", LoadFormat.Unknown, SaveFormat.Gif);
            CheckContentTypeToLoadSaveFormats("image/jpeg", LoadFormat.Unknown, SaveFormat.Jpeg);
            CheckContentTypeToLoadSaveFormats("image/x-emf", LoadFormat.Unknown, SaveFormat.Emf);
            CheckContentTypeToLoadSaveFormats("application/vnd.ms-xpsdocument", LoadFormat.Unknown, SaveFormat.Xps);
            CheckContentTypeToLoadSaveFormats("application/pdf", LoadFormat.Pdf, SaveFormat.Pdf);
            CheckContentTypeToLoadSaveFormats("image/svg+xml", LoadFormat.Unknown, SaveFormat.Svg);
            CheckContentTypeToLoadSaveFormats("application/x-mobi8-ebook", LoadFormat.Azw3, SaveFormat.Azw3);
            CheckContentTypeToLoadSaveFormats("application/x-mobipocket-ebook", LoadFormat.Mobi, SaveFormat.Mobi);
            CheckContentTypeToLoadSaveFormats("application/epub+zip", LoadFormat.Epub, SaveFormat.Epub);
            CheckContentTypeToLoadSaveFormats("text/html", LoadFormat.Html, SaveFormat.Html);
            CheckContentTypeToLoadSaveFormats("text/plain", LoadFormat.Text, SaveFormat.Text);
            CheckContentTypeToLoadSaveFormats("application/msword", LoadFormat.Doc, SaveFormat.Doc);
            CheckContentTypeToLoadSaveFormats("application/rtf", LoadFormat.Rtf, SaveFormat.Rtf);
            CheckContentTypeToLoadSaveFormats("text/xml", LoadFormat.Unknown, SaveFormat.Unknown);
            CheckContentTypeToLoadSaveFormats("multipart/related", LoadFormat.Mhtml, SaveFormat.Mhtml);
            CheckContentTypeToLoadSaveFormats("application/vnd.openxmlformats-officedocument.wordprocessingml.document", LoadFormat.Docx, SaveFormat.Docx);
            CheckContentTypeToLoadSaveFormats("application/vnd.ms-word.document.macroEnabled.12", LoadFormat.Docm, SaveFormat.Docm);
            CheckContentTypeToLoadSaveFormats("application/vnd.openxmlformats-officedocument.wordprocessingml.template", LoadFormat.Dotx, SaveFormat.Dotx);
            CheckContentTypeToLoadSaveFormats("application/vnd.ms-word.template.macroEnabled.12", LoadFormat.Dotm, SaveFormat.Dotm);
            CheckContentTypeToLoadSaveFormats("application/vnd.oasis.opendocument.text", LoadFormat.Odt, SaveFormat.Odt);
            CheckContentTypeToLoadSaveFormats("application/vnd.oasis.opendocument.text-template", LoadFormat.Ott, SaveFormat.Ott);
            CheckContentTypeToLoadSaveFormats("application/vnd.openxmlformats-officedocument.wordprocessingml.document+xml", LoadFormat.FlatOpc, SaveFormat.FlatOpc);

            CheckContentTypeToLoadSaveFormats("application/xaml+xml", LoadFormat.Unknown, SaveFormat.XamlFlow);
            CheckContentTypeToLoadSaveFormats("application/xml", LoadFormat.Unknown, SaveFormat.Unknown);

            CheckContentTypeToLoadSaveFormats("beem", LoadFormat.Unknown, SaveFormat.Unknown);
            CheckContentTypeToLoadSaveFormats(null, LoadFormat.Unknown, SaveFormat.Unknown);
        }

        [Test]
        public void TestImageTypeToExtension()
        {
            Assert.That(".emf", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Emf)));
            Assert.That(".wmf", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Wmf)));
            Assert.That(".pict", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Pict)));
            Assert.That(".jpeg", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Jpeg)));
            Assert.That(".png", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Png)));
            Assert.That(".bmp", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Bmp)));
            Assert.That(".eps", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Eps)));
            Assert.That(".webp", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.WebP)));
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void TestNoImageToExtension()
        {
            Assert.That("", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.NoImage)));
        }

        [ExpectedException(typeof(ArgumentException))]
        [Test]
        public void TestUnknownImageToExtension()
        {
            Assert.That("", Is.EqualTo(FileFormatUtil.ImageTypeToExtension(ImageType.Unknown)));
        }

        /// <summary>
        /// WORDSNET-8073 Fix FileFormatDetector to detect the file as text
        /// </summary>
        [Test]
        public void TestJira8073()
        {
            string filename = TestUtil.BuildTestFileName("ImportTxt/TestJira8073.txt");
            string expectedEncodingName = PlatformUtilPal.IsUnixLike() ? "iso-8859-1" : "Windows-1252";
            CheckDetectFileFormat(filename, LoadFormat.Text, Encoding.GetEncoding(expectedEncodingName), 0);
        }


        [Test]
        public void TestJira8491()
        {
            CheckDetectFileFormat(@"ImportMhtml\DetectFileFormat\TestJira8491.mhtml", LoadFormat.Mhtml, Encoding.ASCII, 0);
        }

        /// <summary>
        /// WORDSNET-20799 FileFormatUtil.DetectFileFormat throws Aspose.Words.FileCorruptedException.
        /// Zip entry comment reading was fixed.
        /// </summary>
        [Test]
        public void Test20799()
        {
            CheckDetectFileFormat(@"Other\Test20799.xlsx", LoadFormat.Unknown, false);
        }


        /// <summary>
        /// WORDSNET-23492 FileFormatUtil.DetectFileFormat() detects some TIFF images as text.
        /// Should check images in detector explicitly to filter out.
        /// </summary>
        [Test]
        public void Test23492()
        {
            CheckDetectFileFormat(@"Other\Test23492.tif", LoadFormat.Unknown, false);
        }


        /// <summary>
        /// Relates to WORDSNET-25454
        /// Checks file format detection in addition to main test <see cref="TestOther.Test25454"/>.
        /// </summary>
        [Test]
        public void Test25454()
        {
            CheckDetectFileFormat(@"Other\Test25454.mso", LoadFormat.Unknown, false);
        }

        /// <summary>
        /// Relates to WORDSNET-25454
        /// Checks file format detection in addition to main test <see cref="TestOther.Test25454A"/>.
        /// </summary>
        [Test]
        public void Test25454A()
        {
            CheckDetectFileFormat(@"Other\Test25454A.mso", LoadFormat.Unknown, null, 0);
        }

        /// <summary>
        /// WORDSNET-26980 Increased max supported length of HTML comments in the HTML format detector.
        /// </summary>
        [Test]
        public void Test26980()
        {
            CheckDetectFileFormat(@"ImportHtml\DetectFileFormat\Test26980.html", LoadFormat.Html, Encoding.UTF8, 0);
        }



        private static void CheckDetectApplicationType(
            string fileName,
            ApplicationType appType)
        {
            using (Stream stream = File.OpenRead(TestUtil.BuildTestFileName(fileName)))
            {
                FileFormatInfo info = FileFormatUtil.DetectFileFormat(stream);
                Assert.That(info.Application, Is.EqualTo(appType));
            }
        }

        private static void CheckDetectFileFormat(
            string fileName,
            LoadFormat expectedFormat,
            Encoding expectedEncoding,
            int expectedPosition)
        {
            using (Stream stream = File.OpenRead(TestUtil.BuildTestFileName(fileName)))
            {
                CheckDetectFileFormat(stream, expectedFormat, false, expectedEncoding, expectedPosition);
            }
        }

        private static void CheckDetectFileFormat(
            string fileName,
            LoadFormat expectedFormat,
            bool expectedEncrypted)
        {
            using (Stream stream = File.OpenRead(TestUtil.BuildTestFileName(fileName)))
            {
                CheckDetectFileFormat(stream, expectedFormat, expectedEncrypted, null, 0);
            }
        }

        private static void CheckHtmlFileFormatDetection(string html)
        {
            using (Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(html)))
            {
                CheckDetectFileFormat(stream, LoadFormat.Html, false, Encoding.UTF8, 0);
            }
        }

        private static void CheckDetectFileFormat(
            Stream stream,
            LoadFormat expectedFormat,
            bool expectedEncrypted,
            Encoding expectedEncoding,
            int expectedPosition)
        {
            FileFormatInfo info = FileFormatUtil.DetectFileFormat(stream);
            Assert.That(info.LoadFormat, Is.EqualTo(expectedFormat));
            Assert.That(info.IsEncrypted, Is.EqualTo(expectedEncrypted));
            Assert.That(info.Encoding, Is.EqualTo(expectedEncoding));
            // Check that the stream was returned to the original position or advanced to skip a particular BOM.
            Assert.That(stream.Position, Is.EqualTo(expectedPosition));
        }

        private static void CheckContentTypeToLoadSaveFormats(
            string contentType,
            LoadFormat expectedLoadFormat,
            SaveFormat expectedSaveFormat)
        {
            try
            {
                Assert.That(FileFormatUtil.ContentTypeToLoadFormat(contentType), Is.EqualTo(expectedLoadFormat));
                if (expectedLoadFormat == LoadFormat.Unknown)
                    Assert.Fail("An exception must be thrown instead of LoadFormat.Unknown.");
            }
            catch (ArgumentException)
            {
                if (expectedLoadFormat != LoadFormat.Unknown)
                    Assert.Fail("An exception must no be thrown is expected LoadFormat is valid.");
            }

            try
            {
                Assert.That(FileFormatUtil.ContentTypeToSaveFormat(contentType), Is.EqualTo(expectedSaveFormat));
                if (expectedSaveFormat == SaveFormat.Unknown)
                    Assert.Fail("An exception must be thrown instead of SaveFormat.Unknown.");
            }
            catch (ArgumentException)
            {
                if (expectedSaveFormat != SaveFormat.Unknown)
                    Assert.Fail("An exception must no be thrown is expected SaveFormat is valid.");
            }
        }
    }
}
