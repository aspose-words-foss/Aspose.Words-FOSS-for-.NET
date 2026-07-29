// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/05/2018 by Konstantin Sidorenko

using System.IO;
using System.Text;
using Aspose.Charset;
using Aspose.Words.Loading;
using NUnit.Framework;

namespace Aspose.Words.Tests.Other
{
    [TestFixture]
    public class TestCharsetDetector
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-16775 Charset is detected incorrectly if file has less then 10 symbols.
        /// CharsetMultiByteRecognizer didn't consider "good" chars count for short (less then 10 symbols) strings.
        /// </summary>
        [Test]
        public void TestJira16775()
        {
            Encoding encoding = CharsetDetector.Detect(File.Open(TestUtil.BuildTestFileName(@"ImportTxt\TestJira16775.txt"), FileMode.Open, FileAccess.Read));
            Assert.That(encoding.CodePage, Is.EqualTo(932));    // Shift-JIS
        }

        /// <summary>
        /// WORDSJAVA-1754 UnsupportedFileFormatException for non-detectable encoding.
        /// </summary>
        [Test]
        public void TestJiraJ1754()
        {
            Document doc = TestUtil.Open(@"ImportTxt\TestJiraJ1754.txt");

            string text = ((Run)((Paragraph)((Body)((Section)doc.FirstChild).FirstChild).FirstChild).FirstChild).Text;
            Assert.That("あああ", Is.EqualTo(text));
        }

        [Test]
        public void TestJiraJ1754SetWrongEncoding()
        {
            // Set wrong encoding. Chinese encoding conflicts with Japanese symbols but doesn't throw.
            LoadOptions options = new LoadOptions();
            options.Encoding = Encoding.GetEncoding("big5");

            Document doc = TestUtil.Open(@"ImportTxt\TestJiraJ1754.txt", options);
            string text = ((Run)((Paragraph)((Body)((Section)doc.FirstChild).FirstChild).FirstChild).FirstChild).Text;

            // Unsupported symbols substituted by '?':.Net returns "???", Java - "?????".
            Assert.That(text == "???" || text == "?????", Is.True);
        }

        /// <summary>
        /// WORDSNET-18929 TxtSaveOptions converts single quote ' into â€.
        /// The problem is incorrect encoding detection due to a insufficient number of analyzed
        /// bytes taken from the beginning of the document. To solve the problem,
        /// we increased the number of bytes in detection buffer from 512 bytes to 3kb.
        /// </summary>
        // FOSS: the Test18929_UTF8.html case was dropped — its input was removed as unsafe customer data (commit 56e13acbaa3).
        [TestCase("Test18929_Win1252.html", CodePage.WindowsLatin1CodePage)]
        public void Test18929(string testFileName, int expectedCodePage)
        {
            CheckCodepage(testFileName, expectedCodePage);
        }

        /// <summary>
        /// WORDSNET-23607 "Unsupported file format: Unknown" on loading TXT file.
        /// Tweaked CharsetDetector for Shift-JIS encoding and allowed 0x80 control character
        /// for Shift-JIS encoding in TxtFormatDetector.
        /// </summary>
        [Test]
        public void Test23607()
        {
            CheckCodepage("Test23607.txt", CodePage.WindowsJapaneseShiftJis);
        }


        /// <summary>
        /// WORDSNET-25453 Hebrew text is imported improperly from HTML.
        /// Improved skip of base64 images in <see cref="CharsetDetector.StripBase64Images"/>.
        /// </summary>
        [Test]
        public void Test25453()
        {
            CheckCodepage("Test25453.html", CodePage.CodePageUtf8);

            // FOSS: HTML cannot be loaded into a Document, so verify format detection
            // instead of loading the file and inspecting its content.
            CheckHtmlFormat("Test25453.html");
        }



        /// <summary>
        /// Checks expected CodePage for a specified test file.
        /// </summary>
        private static void CheckCodepage(string testFileName, int expectedCodePage)
        {
            string testName = string.Format(@"ImportTxt\EncodingDetector\{0}", testFileName);
            using (Stream stream = File.Open(TestUtil.BuildTestFileName(testName), FileMode.Open, FileAccess.Read))
            {
                Encoding encoding = CharsetDetector.Detect(stream);
                Assert.That(encoding.CodePage, Is.EqualTo(expectedCodePage));
            }
        }

        /// <summary>
        /// Checks that the specified test file is detected as HTML format.
        /// The FOSS version cannot load HTML into a <see cref="Document"/>, so we verify
        /// format detection only instead of loading the file and inspecting its content.
        /// </summary>
        private static void CheckHtmlFormat(string testFileName)
        {
            string testName = string.Format(@"ImportTxt\EncodingDetector\{0}", testFileName);
            FileFormatInfo info = FileFormatUtil.DetectFileFormat(TestUtil.BuildTestFileName(testName));
            Assert.That(info.LoadFormat, Is.EqualTo(LoadFormat.Html));
        }
    }
}
