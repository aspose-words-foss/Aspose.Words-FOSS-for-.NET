// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2016 by Alexander Zhiltsov

using Aspose.Words.Loading;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Other
{
    /// <summary>
    /// This class contains tests for classes and members that are used on text extraction: <see cref="Node.GetText"/>,
    /// <see cref="LoadOptions.SkipFormatting"/>, <see cref="PlainTextDocument"/> etc.
    /// </summary>
    [TestFixture]
    public class TestTextExtraction
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-13301 The sectPr elements that are located in the pPr element were not loaded when
        /// <see cref="LoadOptions.SkipFormatting"/> is 'true'. Headers/footers might be not extrated in this mode.
        /// </summary>
        [Test]
        public void TestJira13301()
        {
            Document doc = TestUtil.Open(@"DocBuilder\InsertDocument\TestMultiSectionDoc.docx",
                CreateOptionsForSkipFormatting());
            Assert.That(doc.Sections.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Creates load options with <see cref="LoadOptions.SkipFormatting"/> is set to 'true'.
        /// </summary>
        private static LoadOptions CreateOptionsForSkipFormatting()
        {
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadMode = LoadMode.SkipFormatting;
            return loadOptions;
        }

        /// <summary>
        /// WORDSNET-13301 If w:r elements are located in w:rPr elements, their text was not extracted
        /// when <see cref="PlainTextDocument"/> is used.
        /// </summary>
        [Test]
        public void TestRunInRprElement()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira11751.docx", CreateOptionsForSkipFormatting());
            Paragraph para = doc.FirstSection.HeadersFooters[0].Paragraphs[0];
            Assert.That(para.GetText(), Is.EqualTo("Bold underlined text in header.\r"));
        }

        /// <summary>
        /// WORDSNET-13301 The second and later cells in a merged cell range have text that is not displayed
        /// by MS Word, but was extracted by Aspose.Words in the skip-formatting mode. (For fixed table layout only.)
        /// </summary>
        [Test]
        public void TestMergedCellsInFixedLayout()
        {
            Document doc = TestUtil.Open(@"Model\Table\Grid\Fixed\2003\TestHMerge.docx",
                CreateOptionsForSkipFormatting());
            Table table = doc.FirstSection.Body.Tables[1];
            Assert.That(table.GetText(), Is.EqualTo("C1\x0007\x0007"));
        }


        /// <summary>
        /// Checks that <see cref="Document.GetText"/> and <see cref="PlainTextDocument.Text"/> return identical text.
        /// </summary>
        private static void CheckExtractedText(string docFileName)
        {
            Document doc = TestUtil.Open(docFileName);
            PlainTextDocument plainTextDoc = new PlainTextDocument(TestUtil.BuildTestFileName(docFileName));
            Assert.That(plainTextDoc.Text, Is.EqualTo(doc.GetText()));
        }

    }
}
