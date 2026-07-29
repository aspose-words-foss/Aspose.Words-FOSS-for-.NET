// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Words.Loading;
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Tests how TxtFormatDetector works with various files.
    /// </summary>
    [TestFixture]
    public class TestTxtFormatDetector
    {
        [SetUp]
        public void SetUp()
        {
#if NETSTANDARD
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif
        }

        [Test]
        public void TestSimpleNumbering1()
        {
            VerifyTextFile("SimpleNumbering1.txt", true);
        }

        [Test]
        public void TestSimpleWord()
        {
            VerifyTextFile("SimpleWord.txt", true);
        }

        [Test]
        public void TestSimpleNumber()
        {
            VerifyTextFile("SimpleNumber.txt", true);
        }

        // FOSS TestBinaryJpg removed: its binary1.jpg sample was deleted as unsafe TestData; TestBinaryBin
        // (binary2.bin) still covers "a binary file is not detected as text".

        [Test]
        public void TestBinaryBin()
        {
            VerifyTextFile("binary2.bin", false);
        }

        [Test]
        public void TestBinaryExe()
        {
            VerifyTextFile("BinaryExePart.bin", false);
        }

        [Test]
        public void TestBinaryLib()
        {
            VerifyTextFile("BinaryLibFile.lib", false);
        }

        [Test]
        public void TestIniFile()
        {
            VerifyTextFile("IniFile.ini", true);
        }

        [Test]
        public void TestRegFile()
        {
            VerifyTextFile("RegFile.reg", true);
        }

        /// <summary>
        /// WORDSNET-7000 UnsupportedFileFormatException occurs when loading a TEXT file into DOM.
        /// The problem occurs because of too strong criteria for text files detecting.
        /// The criteria are weaker now.
        /// </summary>
        [Test]
        public void TestJira7000()
        {
            VerifyTextFile("TestJira7000.txt", true);
        }




        /// <summary>
        /// WORDSNET-8196 UnsupportedFileFormatException occurs when loading a TEXT file into DOM
        /// </summary>
        [Test]
        public void TestJira8196()
        {
            VerifyTextFile("TestJira8196.txt", true);
        }


        /// <summary>
        /// Test some binary file, detect them as text file.
        /// </summary>
        [Test]
        public void TestBinaryFiles()
        {
            string binaryFilesFolder = @"ImportTxt\FormatDetector\BinaryFiles\";
            string[] files = Directory.GetFiles(TestUtil.BuildTestFileName(binaryFilesFolder));
            foreach (string file in files)
            {
                VerifyTextFile(file, false, true);
            }
        }


        /// <summary>
        /// Tests that the page break (12) and cell end (7) characters are allowed control characters for text.
        /// </summary>
        [Test]
        public void TestPageBreakAndCellEndChars()
        {
            VerifyText("text\x0C\x0C\x0C\x0C cell\x07 cell\x07 cell\x07\x0C", true);
        }

        /// <summary>
        /// Tests text that contains non-printable characters.
        /// </summary>
        [TestCase("text\0", true, TestName = "TestNonPrintableCharsA")]
        [TestCase("Text that has 74 chars. Text that has 74 chars. Text that has 74 chars.\x01\x01\x01", false, TestName = "TestNonPrintableCharsB")]
        [TestCase("Text that has 178 chars. Text that has 178 chars. Text that has 178 chars. Text that has 178 " +
            "chars. Text that has 178 chars. Text that has 178 chars. Text that has 178 chars. \x01\x01\x01", true, TestName = "TestNonPrintableCharsC")]
        public void TestNonPrintableChars(string data, bool expectedAsText)
        {
            VerifyText(data, expectedAsText);
        }

        /// <summary>
        /// WORDSNET-13777 Text document was detected as unknown format.
        /// </summary>
        [Test]
        public void TestJira13777()
        {
            VerifyTextFile("TestJira13777.txt", true);
        }

        // FOSS TestJira14361 removed: its TestJira14361.doc sample was deleted as unsafe TestData.


        /// <summary>
        /// WORDSNET-23607 "Unsupported file format: Unknown" on loading TXT file.
        /// Tweaked CharsetDetector for Shift-JIS encoding and allowed 0x80 control character
        /// for Shift-JIS encoding in TxtFormatDetector.
        /// </summary>
        [Test]
        public void Test23607()
        {
            string testPath = TestUtil.GetInTestDataPath(@"ImportTxt\EncodingDetector\Test23607.txt");
            VerifyTextFile(testPath, true);
        }

        /// <summary>
        /// WORDSNET-26228 Aspose.Words does not throw exception upon loading corrupted document.
        /// Decrease confidence on bad words length.
        /// </summary>
        [Test]
        public void Test26228()
        {
            Assert.That(FileFormatUtil.DetectFileFormat(
                TestUtil.BuildTestFileName(@"ImportTxt\FormatDetector\Test26228.docx")).LoadFormat, Is.EqualTo(LoadFormat.Unknown));
        }

        /// <summary>
        /// Checks whether a file is a text file or not.
        /// </summary>
        /// <param name="fileName">File name to test.</param>
        /// <param name="expected"></param>
        private static void VerifyTextFile(string fileName, bool expected)
        {
            VerifyTextFile(fileName, expected, false);
        }

        /// <summary>
        /// Checks whether a file is a text file or not.
        /// </summary>
        /// <param name="fileName">File name to test.</param>
        /// <param name="expected"></param>
        private static void VerifyTextFile(string fileName, bool expected, bool checkAllEncoding)
        {
            Assert.That(IsTextFile(fileName, checkAllEncoding), Is.EqualTo(expected), "file " + fileName + " is text");
        }

        /// <summary>
        /// Checks whether the specified string data is text or not.
        /// </summary>
        /// <param name="data">Data to test.</param>
        /// <param name="expected">The data is expected to be detected as text.</param>
        private static void VerifyText(string data, bool expected)
        {
            string fileName = TestUtil.GetInTestOutPath(@"ImportTxt\FormatDetector\detecting.txt");
            TestUtil.EnsureDirectoryForFileExists(fileName);
            File.WriteAllText(fileName, data);

            Assert.That(IsTextFile(fileName, false), Is.EqualTo(expected), "Wrong detection of the following data as text:\r\n" + data);
        }

        /// <summary>
        /// Returns true if the file is text file
        /// </summary>
        private static bool IsTextFile(string fileName, bool checkAllEncoding)
        {
            string fullPath = Path.IsPathRooted(fileName)
                ? fileName
                : TestUtil.BuildTestFileName(@"ImportTxt\FormatDetector\" + fileName);

            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                // Disable Debug.AssertCallingClass assertion called in CustomTextReader constructor
#if !CPLUSPLUS
                Debug.ShowDialogOnAssert(false);
#endif
                Debug.ThrowOnAssert(false);

                CustomTextReader textReader = new CustomTextReader(fs);
                textReader.SetEncodingByBom();

                FileFormatInfo fileFormatInfo = TxtFormatDetector.Detect(textReader);

                return (fileFormatInfo != null) && (fileFormatInfo.LoadFormat == LoadFormat.Text);
            }
        }
    }
}
