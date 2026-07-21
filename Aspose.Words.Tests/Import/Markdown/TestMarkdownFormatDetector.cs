// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/04/2019 by Denis Panov

using System.IO;
using Aspose.Common;
using Aspose.Words.RW.Markdown.FormatDetector;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown
{
    /// <summary>
    /// The class for testing MarkdownFormatDetector.
    /// </summary>
    [TestFixture]
    public class TestMarkdownFormatDetector
    {
        /// <summary>
        /// Tests markdown detector with empty file.
        /// </summary>
        [Test]
        public void TestEmptyFile()
        {
            VerifyFile("TestEmpty.mrdwn", false);
        }

        /// <summary>
        /// Tests that there are no false positives while detecting non-Markdown files.
        /// </summary>
        // FOSS binary1.jpg sample was deleted as unsafe TestData.
        [TestCase("binary2.bin")]
        [TestCase("BinaryExePart.bin")]
        [TestCase("BinaryLibFile.lib")]
        [TestCase("IniFile.ini")]
        [TestCase("SimpleWord.txt")]
        [TestCase("RegFile.reg")]
        public void TestNonMarkdownFiles(string fileName)
        {
            VerifyFile(fileName, false);
        }


        /// <summary>
        /// WORDSNET-18865 FileFormatUtil.DetectFileFormat(Stream) recognizes some PNG images as Markdown.
        /// The number of unprintable characters should be checked relative to a number of actually read characters
        /// instead of whole file.
        /// </summary>
        [Test]
        public void Test18865()
        {
            VerifyFile("Test18865.png", false);
        }



        /// <summary>
        /// WORDSNET-26236 Without explicitly specifying 'LoadFormat.Markdown',
        /// Aspose.Words cannot determine the MD format in the user's MD file.
        ///
        /// The significance of the FencedCode feature has been increased.
        /// </summary>
        [Test]
        public void Test26236()
        {
            VerifyFile("Test26236.mrdwn", true);
        }

        /// <summary>
        /// WORDSNET-28614 Xmpeg file is detected as Markdown by FileFormatUtil.
        /// Introduced a limit on a sentence length in <see cref="MarkdownFormatDetector"/>.
        /// </summary>
        [Test]
        public void Test28614()
        {
            VerifyFile("Test28614.xmpeg", false);
        }

        /// <summary>
        /// Verifies whether the input file is markdown or not.
        /// </summary>
        private static void VerifyFile(string fileName, bool isMarkdown)
        {
            Assert.That(IsMarkdownFile(fileName), Is.EqualTo(isMarkdown), "file "+ fileName + (isMarkdown ? " isn't" : " is") + " detected as markdown");
        }

        /// <summary>
        /// Returns true, if a specified file has a markdown format.
        /// </summary>
        private static bool IsMarkdownFile(string fileName)
        {
            string fullPath = TestUtil.BuildTestFileName(string.Format(@"ImportMarkdown\FormatDetector\{0}", fileName));
            // We use some files from a related ImportTxt directory, so try it too.
            if (!File.Exists(fullPath))
                fullPath = TestUtil.BuildTestFileName(string.Format(@"ImportTxt\FormatDetector\{0}", fileName));

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                CustomTextReader reader = new CustomTextReader(fs);
                reader.SetEncodingByBom();

                FileFormatInfo fileFormatInfo = MarkdownFormatDetector.Detect(reader);

                return (fileFormatInfo != null) && (fileFormatInfo.LoadFormat == LoadFormat.Markdown);
            }
        }
    }
}
