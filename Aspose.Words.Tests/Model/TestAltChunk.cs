// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/05/2012 by Andrey Noskov

using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Basic AltChunk functionality tests.
    /// </summary>
    /// <remarks>
    /// FOSS: tests exercising HTML/RTF-content altChunks were removed - their altChunk parts are .htm/.html/
    /// .xhtml/.rtf and require the HTML/RTF readers, which are removed in FOSS (load throws
    /// NotSupportedException "FOSS: HTML file format is not supported" / "FOSS"). altChunks whose content is
    /// OOXML (.docx) still load, so those tests remain.
    /// </remarks>
    [TestFixture]
    public class TestAltChunk
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// WORDSNET-1937 Support AltChunk tags.
        /// andrnosk: Fixed by adding mechanism to read AltChunk to the model.
        /// AltChunk is inserted after destination body.
        /// </summary>
        [Test]
        public void AltChunkAfterBody()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\AltChunk\AltChunkAfterBody", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.FirstSection.Body.Paragraphs[1].GetText().Trim(), Is.EqualTo("This is first AltChunk paragraph"));
            Assert.That(doc.FirstSection.Body.Paragraphs[2].GetText().Trim(), Is.EqualTo("This is second AltChunk paragraph"));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(5));
        }



        /// <summary>
        /// Test case when Docx is inserted to the document as AltChunk.
        /// </summary>
        [Test]
        public void AltChunkContentDocx()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\AltChunk\AltChunkContentDocx", UnifiedScenario.Docx2DocxNoGold);

            Assert.That(doc.FirstSection.Body.Paragraphs[0].GetText().Trim(), Is.EqualTo("AltChunk Сontent"));
            Assert.That(doc.FirstSection.Body.Paragraphs[6].GetText().Trim(), Is.EqualTo("Test test"));

        }

        /// <summary>
        /// Test case when AltChunk document contains styles.
        /// </summary>
        [Test]
        public void TestAltChunkStyles()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\AltChunk\AltChunkStyles", UnifiedScenario.Docx2DocxNoGold);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            Assert.That(paragraphs[0].ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Heading1));
            Assert.That(paragraphs[1].ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Heading2));
            Assert.That(paragraphs[2].ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Heading3));
            Assert.That(paragraphs[3].ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Subtitle));
            Assert.That(paragraphs[4].ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
            Assert.That(paragraphs[4].ParagraphStyle.Name, Is.EqualTo("CustomStyle"));
        }


        /// <summary>
        /// Test AltChunk document with content controls.
        /// </summary>
        [Test]
        public void TestAltChunkContentControls()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\AltChunk\AltChunkContentControls", UnifiedScenario.Docx2DocxNoGold);

            // Check content controls count.
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Test AltChunk document with Headers/Footers.
        /// </summary>
        [Test]
        public void TestAltChunkHeaderFooter()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\AltChunk\AltChunkHeaderFooter.docx");

            Assert.That(doc.Sections.Count, Is.EqualTo(3));

            HeaderFooterCollection headerFooters0 = doc.Sections[0].HeadersFooters;
            HeaderFooterCollection headerFooters1 = doc.Sections[1].HeadersFooters;

            Assert.That(headerFooters0.Count, Is.EqualTo(3));
            Assert.That(headerFooters0[HeaderFooterType.HeaderPrimary].Paragraphs[0].ToString(SaveFormat.Text), Is.EqualTo("DestinationHeader0\r\n"));
            Assert.That(headerFooters0[HeaderFooterType.HeaderPrimary].Paragraphs[2].ToString(SaveFormat.Text), Is.EqualTo("Section0_Header\r\n"));

            Assert.That(headerFooters0[HeaderFooterType.FooterPrimary].Paragraphs[0].ToString(SaveFormat.Text), Is.EqualTo("DestinationFooter0\r\n"));
            Assert.That(headerFooters0[HeaderFooterType.FooterPrimary].Paragraphs[2].ToString(SaveFormat.Text), Is.EqualTo("Section0_Footer\r\n"));

            Assert.That(headerFooters1.Count, Is.EqualTo(2));
            Assert.That(headerFooters1[HeaderFooterType.HeaderFirst].Paragraphs[0].ToString(SaveFormat.Text), Is.EqualTo("Section1_Header\r\n"));

            Assert.That(headerFooters1[HeaderFooterType.FooterFirst].Paragraphs[0].ToString(SaveFormat.Text), Is.EqualTo("Section1_Footer\r\n"));

            Assert.That(doc.Sections[2].HeadersFooters.Count, Is.EqualTo(2));
        }





        /// <summary>
        /// WORDSNET-10432 Text formatting is not preserved during open/save a DOCX
        /// Word uses ImportFormatMode.UseDestinationStyles to import altchunks.
        /// </summary>
        [Test]
        public void TestJira10432()
        {
            Document doc = TestUtil.Open(@"Model\AltChunk\TestJira10432.docx");

            NodeCollection paras = doc.FirstSection.Body.GetChildNodes(NodeType.Paragraph, true);

            // Verify that altchunks are imported with UseDestinationStyle.
            Paragraph p = (Paragraph)paras[0];
            Assert.That(p.ParaPr[ParaAttr.Istd], Is.EqualTo(0x01));
            Assert.That(p.FirstRun.RunPr[FontAttr.Istd], Is.Null);

            // AM. There is minor problem with second paragraph. Paragraph is imported properly but it has theme fonts.
            // Master document has theme part missing.
            //
            // Word creates theme with 'Calibri Light' + 'Calibri' fonts and I could not understand why.
            // Newly created Word document has 'Cambria' + 'Calibri' fonts. Maybe some special theme is used when AltChunks are imported?
            // Lets postpone this problem and wait till customer complains.
            p = (Paragraph)paras[2];
            Assert.That(p.ParaPr[ParaAttr.Istd], Is.EqualTo(0x02));
            Assert.That(p.FirstRun.RunPr[FontAttr.Istd], Is.Null);

            p = (Paragraph)paras[4];
            Assert.That(p.ParaPr[ParaAttr.Istd], Is.Null);
            Assert.That(p.FirstRun.RunPr[FontAttr.Istd], Is.Null);

            // Verify against gold.
            TestUtil.SaveOpen(doc, @"Model\AltChunk\TestJira10432.docx");
        }



        /// <summary>
        /// WORDSNET-16555 Aspose.Words drops header/footer from altchunk document.
        /// AW does not process headers/footers, when "AltChunk" document has just one section. Word takes in attention
        /// headers/footers of "AltChunk" document, which placed as first child in the section body.
        /// </summary>
        [Test]
        public void TestJira16555()
        {
            Document doc = TestUtil.Open(@"Model\AltChunk\TestJira16555.docx");
            Assert.That(doc.Sections.Count, Is.EqualTo(1));

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(3));
            Assert.That(paras[0].FirstRun.Text, Is.EqualTo("Text from altchunk document"));
            Assert.That(paras[1].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
            Assert.That(paras[2].FirstRun.Text, Is.EqualTo("Text from main document"));

            HeaderFooter header = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Assert.That(header.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(header.FirstParagraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(header.FirstParagraph.FirstRun.Text, Is.EqualTo("Header from altchunk document"));

            HeaderFooter footer = doc.FirstSection.HeadersFooters[HeaderFooterType.FooterPrimary];
            Assert.That(footer.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(footer.FirstParagraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(footer.FirstParagraph.FirstRun.Text, Is.EqualTo("Footer from altchunk document"));
        }

        /// <summary>
        /// Related to WORDSNET-16555
        /// The case, when first element in the document body is not "AltChunk" document.
        /// </summary>
        [Test]
        public void TestJira16555NoHeaderFooter()
        {
            Document doc = TestUtil.Open(@"Model\AltChunk\TestJira16555NoHeaderFooter.docx");
            Assert.That(doc.FirstSection.HeadersFooters.Count, Is.EqualTo(0));

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(3));
            Assert.That(paras[0].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
            Assert.That(paras[1].FirstRun.Text, Is.EqualTo("Text from altchunk document"));
            Assert.That(paras[2].FirstRun.Text, Is.EqualTo("Text from main document"));
        }
    }
}
