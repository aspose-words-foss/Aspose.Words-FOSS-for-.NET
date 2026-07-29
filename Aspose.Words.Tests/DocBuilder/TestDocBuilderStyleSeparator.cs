// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2017 by Dmitry Sokolov

using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    /// <summary>
    /// Implements tests to check insertion of the style separator.
    /// </summary>
    [TestFixture]
    public class TestDocBuilderStyleSeparator
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-15943 Support to insert Style Separator to put different Paragraph styles.
        /// Ability to insert style separator was introduced. Client case.
        /// </summary>
        [Test]
        public void TestJira15943()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());

            Style paraStyle = builder.Document.Styles.Add(StyleType.Paragraph, "MyParaStyle");
            paraStyle.Font.Bold = false;
            paraStyle.Font.Size =8;
            paraStyle.Font.Name ="Arial";

            // Insert a table of contents at the beginning of the document.
            Field field = builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Write("Heading 1");

            // Currently document does not contain separate paragraph for "Heading 1".
            // So, model has not paragraph to which style separator has to be applied.
            // Update field and extract appropriate paragraph. 
            field.Update();

            // Document builder is positioned on the paragraph with TOC field. 
            // Move document builder to destination paragraph and apply style separator.
            builder.MoveTo(builder.CurrentParagraph.NextSibling);
            builder.InsertStyleSeparator();

            // Append text with another style.
            builder.ParagraphFormat.StyleName = paraStyle.Name;
            builder.Write("This is text with some other formatting ");

            // One more separator like in example attached to issue.
            builder.InsertStyleSeparator();

            // One gold for client issue.
            TestUtil.SaveOpen(builder.Document, @"DocBuilder\StyleSeparator\TestJira15943.docx", UnifiedScenario.Docx2Docx);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// One of base tests with style separator. See explanations inside test.
        /// </summary>
        [Test]
        public void TestJira15943CheckContent()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            Body body = builder.Document.FirstSection.Body;

            // 1. Document contains single paragraph. Expected behavior: "Hidden" attributes added to current paragraph, 
            //    new paragraph terminates current line. Current paragraph will be changed. 
            builder.Write("Text line");
            Paragraph para = builder.CurrentParagraph;

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(para, builder.CurrentParagraph, 2);
            CheckParagraphContent(builder, builder.CurrentParagraph, new string[] { " " }, body.LastParagraph);

            // 2. Document contains two paragraphs with style separator and one more paragraph in new line. Builder positioned to
            //    the end of first paragraph and new style separator is inserted. 
            //    Expected behavior: Paragraphs are line up. Builder position is not changed.
            para = body.AppendChild(new Paragraph(builder.Document));
            para.AppendChild(new Run(builder.Document, "Second text"));
            para.AppendChild(new Run(builder.Document, " line"));
            builder.MoveTo(body.FirstParagraph, -1);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[1], body.LastParagraph, 3);
            CheckParagraphContent(builder, body.LastParagraph, new string[] { " ", "Second text", " line" },
                body.FirstParagraph);
            Assert.That(body.FirstParagraph == builder.CurrentParagraph, Is.True);

            // 3. Document contains 3 paragraphs. Builder positioned to the end of first of them.
            //    Two style separators will be inserted. This case checks how spaces have to be added
            //    into lined up paragraphs.
            Assert.That(body.Paragraphs.Count, Is.EqualTo(3));
            para = body.AppendChild(new Paragraph(builder.Document));
            para.AppendChild(new Run(builder.Document, "Text line."));
            para = body.AppendChild(new Paragraph(builder.Document));
            para.AppendChild(new Run(builder.Document, "Second text line."));
            para = body.AppendChild(new Paragraph(builder.Document));
            para.AppendChild(new Run(builder.Document, " Third line."));
            builder.MoveTo(body.Paragraphs[3], -1);

            builder.InsertStyleSeparator();
            CheckParagraphContent(builder, body.Paragraphs[4], new string[] { " ", "Second text line." },
                body.Paragraphs[4].Runs[1]);

            builder.MoveTo(body.Paragraphs[4], -1);
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[4], body.Paragraphs[5], 6);
            CheckParagraphContent(builder, body.Paragraphs[5], new string[] { " Third line." }, body.Paragraphs[5].FirstRun);
            Assert.That(body.Paragraphs.Count, Is.EqualTo(6));

            // 4. This case checks breaking condition of the iteration over paragraphs.
            builder.MoveTo(body.Paragraphs[4], -1);
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[5], body.LastParagraph, 7);
            CheckParagraphContent(builder, body.LastParagraph, new string[] { " " }, body.Paragraphs[4]);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks how builder positon has to be changed after style separator insertion.
        /// </summary>
        [Test]
        public void TestJira15943CheckBuilderPosition()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            Body body = builder.Document.FirstSection.Body;

            // 1. Insert separator when builder is positioned to the middle of paragraph.
            //    Expected behavior: Position of the builder will not be changed and new paragraph will be added.
            builder.Write("Text ");
            Node cursor = body.FirstParagraph.FirstRun;
            builder.Write("line");
            builder.MoveTo(cursor);
            Assert.That(body.Paragraphs.Count, Is.EqualTo(1));

            builder.InsertStyleSeparator();
            Assert.That(body.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(body.FirstParagraph == builder.CurrentParagraph, Is.True);
            Assert.That(ReferenceEquals(cursor, builder.CurrentNode), Is.True);

            // 2. Two paragraphs with content. First paragraph has style separator. 
            //    Builder positioned in the middle of the second paragraph. Insert one more style separator.
            //    Expected behavior: Position of the builder will not be changed and new paragraph will be added.
            Paragraph curPara = body.LastParagraph;
            cursor = curPara.AppendChild(new Run(builder.Document, "Second line"));
            curPara.AppendChild(new Run(builder.Document, " text"));
            builder.MoveTo(cursor);
            Assert.That(body.Paragraphs.Count, Is.EqualTo(2));

            builder.InsertStyleSeparator();
            Assert.That(body.Paragraphs.Count, Is.EqualTo(3));
            Assert.That(curPara == builder.CurrentParagraph, Is.True);
            Assert.That(ReferenceEquals(cursor, builder.CurrentNode), Is.True);

            // 3. Two paragraphs with contents and style separators. Builder positioned in the end of the second paragraph. 
            //    Insert one more style separator. Expected behavior: Position of the builder will be changed and new paragraph
            //    will be added.
            builder.MoveTo(body.LastParagraph, -1);

            builder.InsertStyleSeparator();
            Assert.That(body.Paragraphs.Count, Is.EqualTo(4));
            Paragraph lastPara = body.LastParagraph;
            Assert.That(lastPara == builder.CurrentParagraph, Is.True);
            Assert.That(builder.IsAtEndOfParagraph, Is.True);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks how style separators have to be inserted into document with multiple sections.
        /// </summary>
        [Test]
        public void TestJira15943MultiSections1()
        {
            Document doc = TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943MultiSections.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            Body body = doc.FirstSection.Body;

            // 1. First section of the document contains two paragraphs. Style separator has to be inserted.
            //    Expected behavior: First paragraph will be marked as style separator.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.LastParagraph, 3);
            CheckParagraphContent(builder, body.LastParagraph,
                new string[] { " ", "Second text line" }, body.FirstParagraph.FirstRun);

            // 2. Expected behavior: Second paragraph will be marked as style separator.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.LastParagraph, 4);
            CheckStyleSeparatorAttr(null, doc.LastSection.Body.FirstParagraph, 4);
            Assert.That(doc.LastSection.Body.Paragraphs.Count, Is.EqualTo(1));

            // 3. Move builder position to the end of the last paragraph of the first section.           
            //    Expected behavior: Last paragraph will be marked as style separator.
            builder.MoveTo(body.LastParagraph);
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.LastParagraph, 5);
            CheckStyleSeparatorAttr(null, doc.LastSection.Body.FirstParagraph, 5);
            Assert.That(doc.LastSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Case description:
        /// First section contains two paragraphs which have style separator properties.
        /// One more style separator will be inserted.
        /// Expected behavior: Nothing will change.
        /// </summary>
        [Test]
        public void TestJira15943MultiSections2()
        {
            Document doc = TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943MultiSections2.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            Body body = doc.FirstSection.Body;

            builder.InsertStyleSeparator();
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Write("Heading 1");
            
            CheckStyleSeparatorAttr(body.FirstParagraph, null, 5);
            CheckStyleSeparatorAttr(body.Paragraphs[1], null, 5);
            CheckParagraphContent(builder, body.Paragraphs[1],
                new string[] { " ", "dfvfd" }, body.FirstParagraph.Runs[1]);
            CheckStyleSeparatorAttr(null, body.LastParagraph, 5);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// One of base tests with SDT. See explanations inside test.
        /// </summary>
        [Test]
        public void TestJira15943SDT1()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT1.docx"));
            Body body = builder.Document.FirstSection.Body;
            Node expectedPos = body.FirstParagraph.FirstRun;
            Assert.That(body.GetChildNodes(NodeType.Paragraph, true).Count, Is.EqualTo(5));
            StructuredDocumentTag sdt = (StructuredDocumentTag)body.GetChild(NodeType.StructuredDocumentTag, 0, false);
            NodeCollection sdtParas = sdt.GetChildNodes(NodeType.Paragraph, true);

            // 1. Documents contains paragraph and block level SDT. Builder is positioned in the first run of the document.
            //    Style separator will be inserted. Expected behavior: First paragraph is marked as style separator. New
            //    paragraph with the space is added after style separator. Content of the SDT is not changed.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.Paragraphs[1], 6);           
            CheckStyleSeparatorAttr(null, (Paragraph)sdtParas[0], 6); // First para into SDT.
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], new string[] { "Rich" }, expectedPos);            
            CheckStyleSeparatorAttr(null, body.Paragraphs[1], 6); // Additional paragraph was inserted.
            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, expectedPos);

            // 2. Expected behavior: Style separator properties were applied to additional paragraph inserted above with
            //    the style separator and first paragraph into SDT.           
            builder.InsertStyleSeparator();
            // First paragraph still has style separator properties.
            CheckStyleSeparatorAttr(body.FirstParagraph, null, 6);
            // Additional paragraph, which was inserted before SDT.
            CheckStyleSeparatorAttr(body.Paragraphs[1], null, 6);
            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, expectedPos);
            // First paragraph into SDT also has style separator properties.
            CheckStyleSeparatorAttr((Paragraph)sdtParas[0], null, 6);
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], new string[] { "Rich" }, expectedPos);
            // Second paragraph of the SDT produces new line.
            CheckStyleSeparatorAttr(null, (Paragraph)sdtParas[1], 6);
            CheckParagraphContent(builder, (Paragraph)sdtParas[1], new string[] { " ", "Text" }, expectedPos);

            // 3. Expected behavior: Style separator properties were applied to next paragraph into SDT.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr((Paragraph)sdtParas[1], (Paragraph)sdtParas[2], 6);
            CheckParagraphContent(builder, (Paragraph)sdtParas[1], new string[] { " ", "Text" }, expectedPos);
            CheckParagraphContent(builder, (Paragraph)sdtParas[2], new string[] { " ", "Control" }, expectedPos);

            // 4. Expected behavior: Style separator properties were applied to next paragraph into SDT.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr((Paragraph)sdtParas[2], (Paragraph)sdtParas[3], 6);
            CheckParagraphContent(builder, (Paragraph)sdtParas[2], new string[] { " ", "Control" }, expectedPos);
            CheckParagraphContent(builder, (Paragraph)sdtParas[3], new string[] { " ", "Content" }, expectedPos);

            // 5. Expected behavior: Additional paragraph will be added after SDT. Style separator properties will
            //    be applied to last paragraph of the SDT.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr((Paragraph)sdtParas[3], body.LastParagraph, 7);
            CheckParagraphContent(builder, (Paragraph)sdtParas[3], new string[] { " ", "Content" }, expectedPos);
            CheckParagraphContent(builder, body.LastParagraph, new string[] { " " }, expectedPos);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// One of Base test with SDT. See explanations inside test.
        /// </summary>
        [Test]
        public void TestJira15943SDT2()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT1.docx"));
            Body body = builder.Document.FirstSection.Body;

            StructuredDocumentTag sdt = (StructuredDocumentTag)body.GetChild(NodeType.StructuredDocumentTag, 0, false);
            NodeCollection sdtParas = sdt.GetChildNodes(NodeType.Paragraph, true);

            // Move builder to the end of the first paragraph.
            builder.MoveTo(body.FirstParagraph, -1);

            // 1. Builder is positioned to the end of the first paragraph.
            //    Style separator will be inserted. 
            //    Expected behavior: First paragraph is marked as style separator. New paragraph with the space is added after
            //    style separator. Content of the SDT is not changed. Builder position is changed to the second run of the
            //    first paragraph inside SDT. However, actually document markup does not contain space before first run of the
            //    SDT. It looks like some inconsistent between document presentation and markup. So, just move to end of
            //    paragraph which was added.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.Paragraphs[1], 6);
            CheckStyleSeparatorAttr(null, (Paragraph)sdtParas[0], 6);

            Node expectedPosition = body.Paragraphs[1];
            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, expectedPosition);
            // Check content of the first paragraph into SDT.
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], new string[] { "Rich" }, expectedPosition);

            // 2. MSW does not add next style separator when client is positioned to the end of the paragraph before SDT.
            //    Check this case.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.Paragraphs[1], 6);
            CheckStyleSeparatorAttr(null, (Paragraph)sdtParas[0], 6);

            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, expectedPosition);
            // Check content of the first paragraph into SDT.
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], new string[] { "Rich" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Case checks style separator insertion when paragraph before SDT has style separator attributes.
        /// </summary>
        [Test]
        public void TestJira15943SDT3()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT3.docx"));
            Body body = builder.Document.FirstSection.Body;
            Node expectedPosition = body.FirstParagraph;

            StructuredDocumentTag sdt = (StructuredDocumentTag)body.GetChild(NodeType.StructuredDocumentTag, 0, false);
            NodeCollection sdtParas = sdt.GetChildNodes(NodeType.Paragraph, true);

            builder.MoveTo(body.FirstParagraph, -1);
            builder.InsertStyleSeparator();

            CheckStyleSeparatorAttr(body.FirstParagraph, null, 5);
            CheckStyleSeparatorAttr((Paragraph)sdtParas[0], null, 5);
            CheckStyleSeparatorAttr(null, (Paragraph)sdtParas[1], 5);

            CheckParagraphContent(builder, body.FirstParagraph, new string[] { "Text line" }, expectedPosition);
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], new string[] { "Rich" }, expectedPosition);
            CheckParagraphContent(builder, (Paragraph)sdtParas[1], new string[] { " ", "Text" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Case checks style separator insertion for two sequently positioned SDT's.
        /// </summary>
        [Test]
        public void TestJira15943SDT4()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT4.docx"));
            Body body = builder.Document.FirstSection.Body;

            StructuredDocumentTag sdt = (StructuredDocumentTag)body.GetChild(NodeType.StructuredDocumentTag, 0, false);
            NodeCollection sdtParas = sdt.GetChildNodes(NodeType.Paragraph, true);

            builder.InsertStyleSeparator();
            Node expectedPosition = ((Paragraph)sdtParas[1]).Runs[1];
            CheckStyleSeparatorAttr((Paragraph)sdtParas[0], (Paragraph)sdtParas[1], 4);
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], null, expectedPosition);
            CheckParagraphContent(builder, (Paragraph)sdtParas[1], new string[] { " ", "SDT" }, expectedPosition);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr((Paragraph)sdtParas[1], body.Paragraphs[2], 5);
            CheckParagraphContent(builder, (Paragraph)sdtParas[1], new string[] { " ", "SDT" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { " " }, expectedPosition);

            sdt = (StructuredDocumentTag)body.GetChild(NodeType.StructuredDocumentTag, 1, false);
            sdtParas = sdt.GetChildNodes(NodeType.Paragraph, true);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[2], null, 5);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { " " }, expectedPosition);

            CheckStyleSeparatorAttr((Paragraph)sdtParas[0], (Paragraph)sdtParas[1], 5);
            CheckParagraphContent(builder, (Paragraph)sdtParas[0], new string[] { "One" }, expectedPosition);
            CheckParagraphContent(builder, (Paragraph)sdtParas[1], new string[] { " ", "More" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Case with nested SDT.
        /// </summary>
        [Test]
        public void TestJira15943SDT5()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT5.docx"));
            Body body = builder.Document.FirstSection.Body;
            Node expectedPosition = body.FirstParagraph.FirstRun;

            // One paragraph in the body and two paragraph in nested SDT.
            // First style separator.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[0], body.Paragraphs[1], 5);
            CheckParagraphContent(builder, body.Paragraphs[0], new string[] { "Line1" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, expectedPosition);

            // Second style separator.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[1], null, 6);
            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, expectedPosition);

            CheckStyleSeparatorAttr(body.Paragraphs[2], body.Paragraphs[3], 6);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { "Line2" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[3], new string[] { " " }, expectedPosition);

            // Third style separator.
            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[3], null, 6);
            CheckParagraphContent(builder, body.Paragraphs[3], new string[] { " " }, expectedPosition);

            CheckStyleSeparatorAttr(body.Paragraphs[4], body.Paragraphs[5], 6);
            CheckParagraphContent(builder, body.Paragraphs[4], new string[] { "Line3" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[5], new string[] { " " }, expectedPosition);

            Assert.That(body.Paragraphs[5].ParentNode == body, Is.True);
            Assert.That(body.GetChild(NodeType.StructuredDocumentTag, 1, true) == body.Paragraphs[4].ParentNode, Is.True);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Case with nested SDT.
        /// </summary>
        [Test]
        public void TestJira15943SDT6()
        {   
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT6.docx"));
            Body body = builder.Document.FirstSection.Body;
            builder.MoveTo(body.Paragraphs[2].FirstRun);
            Node expectedPosition = body.Paragraphs[2].FirstRun;

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[2], body.Paragraphs[3], 6);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { "Line3" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[3], new string[] { " " }, expectedPosition);
            Assert.That(body.GetChild(NodeType.StructuredDocumentTag, 0, true) == body.Paragraphs[3].ParentNode, Is.True);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[3], null, 6);
            CheckParagraphContent(builder, body.Paragraphs[3], new string[] { " " }, expectedPosition);

            CheckStyleSeparatorAttr(body.Paragraphs[4], body.Paragraphs[5], 6);
            CheckParagraphContent(builder, body.Paragraphs[4], new string[] { "Line4" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[5], new string[] { " " }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Case with nested SDT. Style separator inserts for paragraph into SDT which positioned before another SDT.
        /// </summary>
        [Test]
        public void TestJira15943SDT7()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT7.docx"));
            Body body = builder.Document.FirstSection.Body;
            Node expectedPosition = body.Paragraphs[1];
            builder.MoveTo(body.Paragraphs[1], -1);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[1], body.Paragraphs[2], 4);

            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { "Line2" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { " ", "Line3" }, expectedPosition);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[2], body.Paragraphs[3], 4);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { " ", "Line3" }, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[3], new string[] { " " }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks that inline SDT is skipped while style separator inserting. 
        /// </summary>
        [Test]
        public void TestJira15943SDT8()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT8.docx"));
            Body body = builder.Document.FirstSection.Body;
            Node expectedPosition = body.Paragraphs[1].FirstRun;
            builder.MoveTo(body.Paragraphs[0]);

            builder.InsertStyleSeparator();
            builder.InsertStyleSeparator();

            CheckStyleSeparatorAttr(body.Paragraphs[0], null, 3);
            CheckStyleSeparatorAttr(body.Paragraphs[1], null, 3);
            CheckStyleSeparatorAttr(null, body.Paragraphs[2], 3);

            CheckParagraphContent(builder, body.Paragraphs[0], null, expectedPosition);
            CheckParagraphContent(builder, body.Paragraphs[2], new string[] { " ", "Line3" }, expectedPosition);

            Assert.That(body.Paragraphs[1].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
            Assert.That(body.Paragraphs[1].BreakIsStyleSeparator, Is.True);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks the case when document builder is positioned to the empty paragraph before SDT.
        /// AW behavior: Nothing is changed. MSW behavior: One more paragraph with the space is added.
        /// </summary>
        [Test]
        public void TestJira15943SDT9()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT9.docx"));
            Body body = builder.Document.FirstSection.Body;
            builder.MoveTo(body.FirstParagraph);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.Paragraphs[0], body.Paragraphs[1], 3);
            CheckParagraphContent(builder, body.Paragraphs[0], new string[] { "Line1" }, body.Paragraphs[1]);
            CheckParagraphContent(builder, body.Paragraphs[1], new string[] { " " }, body.Paragraphs[1]);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(null, body.Paragraphs[1], 3);
            CheckStyleSeparatorAttr(null, body.Paragraphs[2], 3);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks the case when paragraph is the last paragraph inside SDT and this paragraph is last in last section.
        /// Also paragraph is empty and builder is positioned to this paragraph. Expected behavior: nothing is happened.
        /// </summary>
        [Test]
        public void TestJira15943SDT10()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943SDT10.docx"));
            Body body = builder.Document.FirstSection.Body;
            builder.MoveTo(body.LastParagraph);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(null, body.LastParagraph, 2);
            CheckParagraphContent(builder, body.LastParagraph, null, body.LastParagraph);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks how style separators have to be inserted into document with table.
        /// </summary>
        [Test]
        public void TestJira15943Tbl1()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Tbl1.docx"));
            Body body = builder.Document.FirstSection.Body;
            Cell cell = body.Tables[0].FirstRow.FirstCell;
            builder.MoveTo(body.FirstParagraph);

            Node expectedPosition = cell.FirstParagraph.FirstRun;
            
            builder.InsertStyleSeparator();
            builder.InsertStyleSeparator();
            builder.InsertStyleSeparator();

            Assert.That(cell.Paragraphs.Count, Is.EqualTo(3));
            CheckStyleSeparatorAttr(body.FirstParagraph, null, 8);
            CheckStyleSeparatorAttr(cell.Paragraphs[0], null, 8);
            CheckStyleSeparatorAttr(cell.Paragraphs[1], null, 8);
            CheckStyleSeparatorAttr(null, cell.Paragraphs[2], 8);

            CheckParagraphContent(builder, body.FirstParagraph, new string[] { "Text line" }, expectedPosition);
            CheckParagraphContent(builder, cell.Paragraphs[0], new string[] { " ", "Cell1" }, expectedPosition);
            CheckParagraphContent(builder, cell.Paragraphs[1], new string[] { " ", "Text" }, expectedPosition);
            CheckParagraphContent(builder, cell.Paragraphs[2], new string[] { " ", "Line" }, expectedPosition);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(null, cell.Paragraphs[2], 8);
            CheckParagraphContent(builder, cell.Paragraphs[2], new string[] { " ", "Line" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks the case when style separators insert within table cell which nested into SDT and another tables.
        /// </summary>
        [Test]
        public void TestJira15943Tbl2()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Tbl2.docx"));
            Body body = builder.Document.FirstSection.Body;
            Cell cell = ((Table)body.GetChild(NodeType.Table, 1, true)).FirstRow.FirstCell;
            Node expectedPosition = body.FirstParagraph.FirstRun;

            builder.InsertStyleSeparator();
            builder.InsertStyleSeparator();

            // Actually MSW removes SDT at this case. Currently implemented simplified solution.
            CheckStyleSeparatorAttr(body.FirstParagraph, null, 10);
            CheckStyleSeparatorAttr(body.Paragraphs[1], cell.Paragraphs[0], 10);
            CheckStyleSeparatorAttr(null, cell.Paragraphs[1], 10);

            CheckParagraphContent(builder, body.FirstParagraph, new string[] { "Line1" }, expectedPosition);
            CheckParagraphContent(builder, cell.Paragraphs[0], new string[] { " ", "NestedCell1" }, expectedPosition);
            CheckParagraphContent(builder, cell.Paragraphs[1], new string[] { "Line1.5" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks the case when style separators insert into document where table is following after block level SDT.
        /// </summary>
        [Test]
        public void TestJira15943Tbl3()
        {
            // SDT before table, check behavior.
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Tbl3.docx"));
            Body body = builder.Document.FirstSection.Body;
            Cell cell = body.Tables[0].FirstRow.FirstCell;
            builder.MoveTo(body.FirstParagraph);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, cell.Paragraphs[0], 5);
            CheckParagraphContent(builder, body.FirstParagraph, new string[] { "Line1" }, body.FirstParagraph);
            CheckParagraphContent(builder, cell.Paragraphs[0], new string[] { " ", "Cell1" }, body.FirstParagraph);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks the case when style separator inserts into table cell which placed within shape content.
        /// </summary>

        [Test]
        public void TestJira15943Tbl4()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Tbl4.docx"));
            Table tbl = (Table)builder.Document.FirstSection.Body.GetChild(NodeType.Table, 0, true);
            builder.MoveTo(tbl.FirstRow.FirstCell.FirstParagraph);

            TestWarningCallback wc = new TestWarningCallback();
            builder.Document.WarningCallback = wc;

            builder.InsertStyleSeparator();

            Assert.That(wc.Count, Is.EqualTo(1));
            Assert.That(wc.Contains(WarningSource.Unknown, WarningType.UnexpectedContent,
                "Insertion of style separator allowed in the main text story."), Is.True);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks the case when style separators insert into cell where SDT following after the paragraph. 
        /// </summary>
        [Test]
        public void TestJira15943Tbl5()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Tbl5.docx"));
            Cell cell = ((Table)builder.Document.FirstSection.Body.GetChild(NodeType.Table, 0, true)).FirstRow.FirstCell;
            Node expectedPos = cell.FirstParagraph.FirstRun;
            builder.MoveTo(expectedPos);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(cell.FirstParagraph, cell.Paragraphs[1], 9);
            CheckParagraphContent(builder, cell.Paragraphs[1], new string[] { " " }, expectedPos);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(cell.Paragraphs[1], null, 9);
            CheckStyleSeparatorAttr(cell.Paragraphs[2], cell.Paragraphs[3], 9);
            CheckParagraphContent(builder, cell.Paragraphs[2], new string[] { "SDT line 2" }, expectedPos);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(cell.Paragraphs[3], cell.Paragraphs[4], 9);
            CheckParagraphContent(builder, cell.Paragraphs[4], new string[] { " " }, expectedPos);

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(cell.Paragraphs[4], cell.Paragraphs[5], 9);
            CheckParagraphContent(builder, cell.Paragraphs[5], null, expectedPos);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks that style separator can not be inserted within header.
        /// </summary>
        [Test]
        public void TestJira15943Header()
        {
            CheckHeaderFooterStyleSeparator(HeaderFooterType.HeaderPrimary);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks that style separator can not be inserted within footer.
        /// </summary>

        [Test]
        public void TestJira15943Footer()
        {
            CheckHeaderFooterStyleSeparator(HeaderFooterType.FooterPrimary);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks that style separator can not be inserted within shape content.
        /// </summary>
        [Test]
        public void TestJira15943Shape1()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Shape.docx"));
            Shape shape = builder.Document.FirstSection.Body.Shapes[0];
            builder.MoveTo(shape.FirstParagraph);

            TestWarningCallback wc = new TestWarningCallback();
            builder.Document.WarningCallback = wc;

            builder.InsertStyleSeparator();

            Assert.That(wc.Count, Is.EqualTo(1));
            Assert.That(wc.Contains(WarningSource.Unknown, WarningType.UnexpectedContent,
                "Insertion of style separator allowed in the main text story."), Is.True);
            CheckStyleSeparatorAttr(null, shape.FirstParagraph, 3);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Picture with block level content occurred in paragraph. 
        /// Current case checks that processing is skipped for such content.
        /// </summary>  
        [Test]
        public void TestJira15943Shape2()
        {
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(@"DocBuilder\StyleSeparator\TestJira15943Tbl4.docx"));
            Body body = builder.Document.FirstSection.Body;
            builder.MoveTo(body.FirstParagraph);

            builder.InsertStyleSeparator();
            builder.InsertStyleSeparator();

            CheckStyleSeparatorAttr(body.FirstParagraph, null, 7);
            CheckStyleSeparatorAttr(body.Paragraphs[1], body.LastParagraph, 7);
            CheckStyleSeparatorAttr(null, body.Shapes[0].FirstParagraph, 7);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks insertion of style separator to footnote.
        /// </summary>
        [Test]
        public void TestJira15943Footnote()
        {
            const string path = @"DocBuilder\StyleSeparator\TestJira15943Common1.docx";
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(path));
            Footnote footnote = (Footnote)builder.Document.GetChild(NodeType.Footnote, 0, true);
            Assert.That(FootnoteType.Footnote, Is.EqualTo(footnote.FootnoteType));

            builder.MoveTo(footnote.FirstParagraph);
            Node expectedPosition = footnote.LastParagraph.FirstRun;

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(footnote.FirstParagraph, footnote.LastParagraph, 10);
            CheckParagraphContent(builder, footnote.LastParagraph, new string[] { " ", "text line" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Checks insertion of style separator to endnote.
        /// </summary>
        [Test]
        public void TestJira15943Endnote()
        {
            const string path = @"DocBuilder\StyleSeparator\TestJira15943Common1.docx";
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(path));
            Footnote footnote = (Footnote)builder.Document.GetChild(NodeType.Footnote, 1, true);
            Assert.That(FootnoteType.Endnote, Is.EqualTo(footnote.FootnoteType));

            builder.MoveTo(footnote.FirstParagraph);
            Node expectedPosition = footnote.LastParagraph.FirstRun;

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(footnote.FirstParagraph, footnote.LastParagraph, 10);
            CheckParagraphContent(builder, footnote.LastParagraph, new string[] { " ", "line" }, expectedPosition);
        }

        /// <summary>
        /// This test is the part of the WORDSNET-15943
        /// Simple test which inserts style separator to document with one paragraph in the body.
        /// </summary>
        [Test]
        public void TestJira15943OnePara()
        {
            const string path = @"DocBuilder\StyleSeparator\TestJira15943Common2.docx";
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(path));
            Body body = builder.Document.FirstSection.Body;

            builder.InsertStyleSeparator();
            CheckStyleSeparatorAttr(body.FirstParagraph, body.LastParagraph, 2);
        }


        /// <summary>
        /// WORDSNET-24053 Style Separator is not inserted if add an empty section before building the document.
        /// The redundant check removed.
        /// </summary>
        [Test]
        public void Test24053()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            doc.Sections.Add(new Section(doc));
            // Append text with "Heading 1" style.
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Write("Heading 1");
            builder.InsertStyleSeparator();
            // Append text with another style.
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;
            builder.Write("This is text with some other formatting ");

            TestUtil.SaveOpen(doc, @"DocBuilder\StyleSeparator\Test24053", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Sections.Count, Is.EqualTo(2));
            Assert.That(doc.FirstSection.Body.FirstParagraph.BreakIsStyleSeparator, Is.True);
        }

        /// <summary>
        /// Checks that style separator can not be inserted to the header/footer of the specified type.
        /// </summary>
        private static void CheckHeaderFooterStyleSeparator(HeaderFooterType type)
        {
            const string path = @"DocBuilder\StyleSeparator\TestJira15943Common1.docx";
            DocumentBuilder builder = new DocumentBuilder(TestUtil.Open(path));
            HeaderFooter header = builder.Document.FirstSection.HeadersFooters[type];
            builder.MoveTo(header.FirstParagraph);

            TestWarningCallback wc = new TestWarningCallback();
            builder.Document.WarningCallback = wc;

            builder.InsertStyleSeparator();
            Assert.That(wc.Count, Is.EqualTo(1));
            Assert.That(wc.Contains(WarningSource.Unknown, WarningType.UnexpectedContent,
                "Insertion of style separator allowed in the main text story."), Is.True);
            CheckStyleSeparatorAttr(null, header.FirstParagraph, 10);
        }

        /// <summary>
        /// Checks content of the specified paragraph and current position of the document builder.
        /// </summary>
        private static void CheckParagraphContent(DocumentBuilder builder, Paragraph para,
            string[] expectedContent, Node expectedPosition)
        {
            int expectedLenght = (expectedContent == null) ? 0 : expectedContent.Length;
            Assert.That(para.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(expectedLenght));

            if (expectedLenght > 0)
            {
                NodeCollection runs = para.GetChildNodes(NodeType.Run, false);

                for (int i = 0; i < expectedContent.Length; ++i)
                    Assert.That(((Run)runs[i]).Text, Is.EqualTo(expectedContent[i]));
            }

            if (expectedPosition.NodeType == NodeType.Paragraph)
            {
                Assert.That(builder.IsAtEndOfParagraph, Is.True);
                Assert.That(builder.CurrentParagraph == expectedPosition, Is.True);
            }
            else if (expectedPosition.NodeType == NodeType.Run)
            {
                Assert.That(expectedPosition == builder.CurrentNode, Is.True);
                Assert.That(builder.CurrentParagraph == expectedPosition.ParentNode, Is.True);
            }
            else
            {
                Assert.Fail("Builder was positioned to unexpected node type.");
            }
        }

        /// <summary>
        /// Checks style separator attributes are set for "styleSeparatorPara" and does not exist for "lineBreakPara".
        /// Also checks total amount of paragraphs in the document.
        /// </summary>
        private static void CheckStyleSeparatorAttr(Paragraph styleSeparatorPara,
            Paragraph lineBreakPara, int expectedParaCount)
        {
            RunPr paraRunPr;
            DocumentBase doc = null;

            if (styleSeparatorPara != null)
            {
                doc = styleSeparatorPara.Document;                
                Assert.That(styleSeparatorPara.BreakIsStyleSeparator, Is.True);
            }

            if (lineBreakPara != null)
            {
                doc = lineBreakPara.Document;
                paraRunPr = lineBreakPara.ParagraphBreakRunPr;
                Assert.That(paraRunPr.Contains(FontAttr.SpecialHidden), Is.False);
                Assert.That(paraRunPr.Contains(FontAttr.Hidden), Is.False);
            }

            if (doc != null)
                Assert.That(doc.GetChildNodes(NodeType.Paragraph, true).Count, Is.EqualTo(expectedParaCount));
        }
    }
}
