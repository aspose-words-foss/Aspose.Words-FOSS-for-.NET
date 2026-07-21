// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2020 by Vasiliy Stepchenko

using System.IO;
using System.Text;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Gold.Txt;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export.Txt
{
    /// <summary>
    /// Tests of issues specific for export to Txt format.
    /// </summary>
    [TestFixture]
    public class TestExportTxt : TestGoldTxtBase
    {
        /// <summary>
        /// WORDSNET-21385 System.ArgumentOutOfRangeException was thrown while saving DOCX to TXT.
        /// <see cref="TxtTableBuilder"/> was improved to handle the nested tables properly.
        /// </summary>
        [TestCase(@"ExportTxt\Test21385A.docx")]
        [TestCase(@"ExportTxt\Test21385B.docx")]
        [TestCase(@"ExportTxt\Test21385C.docx")]
        [Test]
        public void Test21385(string fileName)
        {
            Document doc = TestUtil.Open(fileName);

            TxtSaveOptions options = new TxtSaveOptions();
            options.SaveFormat = SaveFormat.Text;
            options.PreserveTableLayout = true;

            // Verify no exception on saving and fixate output by gold file.
            VerifyTextGold(doc, doc.OriginalFileName, "", GoldLevel.ExportOnly, options);
        }

        /// <summary>
        /// WORDSNET-22090 Each Line in TXT File should have a Fix Length of Characters in Line.
        /// Implemented new <see cref="TxtSaveOptions.MaxCharactersPerLine"/> option.
        /// </summary>
        [TestCase(60)]
        [TestCase(120)]
        [TestCase(7)]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(-100)]
        public void Test22090(int lineLength)
        {
            Document doc = TestUtil.Open(@"ExportTxt\Test22090.docx");

            TxtSaveOptions options = new TxtSaveOptions();
            // The value -100 is just chosen to check behavior by default.
            if (lineLength != -100)
                options.MaxCharactersPerLine = lineLength;

            string suffix = string.Format(@"({0})", lineLength);
            VerifyTextGold(doc, doc.OriginalFileName, suffix, GoldLevel.ExportOnly, options);
        }



        /// <summary>
        /// WORDSNET-25287 Put comment content into a separate line when export to TXT.
        /// To mimic Word we should start all comments from a new line in <see cref="TextWriter"/>.
        /// </summary>
        [Test]
        public void Test25287()
        {
            VerifyExport(@"..\ExportTxt\Test25287.docx");
        }

        /// <summary>
        /// WORDSNET-26261 NullReferenceException is thrown when using ToString method on a cell
        /// with nested table and PreserveTableLayout option.
        /// Tests support of PreserveTableLayout option for ToString() method.
        /// </summary>
        [Test]
        public void Test26261()
        {
            const string cellText = "Test\r\nTest                 Test\r\nTest                 Test";
            const string rowText = "Test                                         c                  " +
              "                          c\r\nTest                 Test\r\nTest                 Test";

            Document doc = TestUtil.Open(@"ExportTxt\Test26261.docx");

            TxtSaveOptions opt = new TxtSaveOptions();
            opt.PreserveTableLayout = true;
            Assert.That(cellText, Is.EqualTo(doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.ToString(opt).Trim()));
            Assert.That(rowText, Is.EqualTo(doc.FirstSection.Body.Tables[0].FirstRow.ToString(opt).Trim()));
        }


        internal const string BlockQuoteHtmlSnippet =
                @"<blockquote>
                    Some quoted text_1
                    <br />
                    More quoted text_1
                </blockquote>
                <blockquote>
                    Some quoted text_2
                    <br />
                    More quoted text_2
                    <blockquote>
                        Some nested quoted text
                        <br />
                        ...as blockquotes can be nested
                        <blockquote>
                            Deepest nested quoted textA
                            <br />
                            Deepest nested quoted textB
                            <br />
                            Deepest nested quoted textC
                            <br />
                            Deepest nested quoted textD
                        </blockquote>
                    </blockquote>
                </blockquote>";
    }
}
