// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderPara
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }




        [Test]
        public void TestWritelnEmpty()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("");
            Assert.That(doc.GetText(), Is.EqualTo("\r\x0c"));
        }




        [Test]
        public void TestLeftIndentWord2000()
        {
            DocumentBuilder b = new DocumentBuilder();
            b.ParagraphFormat.LeftIndent = 72;    //1"
            b.Write("Hello");
            ParagraphFormat pf = ((Paragraph)b.Document.GetChild(NodeType.Paragraph, 0, true)).ParagraphFormat;
            Assert.That(pf.LeftIndent, Is.EqualTo(72.0));
        }

        /// <summary>
        /// Need to visually check in Word2000 and WordXP because writes rtl
        /// attributes that are only in the binary file, but not in the model.
        /// </summary>
        [Test]
        public void TestCreateRtlPara()
        {
            DocumentBuilder b = new DocumentBuilder();
            b.ParagraphFormat.Bidi = true;
            b.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            b.ParagraphFormat.RightIndent = 36;
            b.ParagraphFormat.LeftIndent = 72;
            b.Writeln("Hello");
            TestUtil.Save(b.Document, @"DocBuilder\TestCreateRtlPara.docx", null, false);
        }

        [Test]
        public void TestMoveToParaInSection()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToPara.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            //Move to beginning of section
            builder.MoveToSection(1);
            builder.Writeln("Test1");

            //Just out of interest, to make sure inserting sections works okay.
            builder.InsertBreak(BreakType.SectionBreakContinuous);

            //Move to end of section
            builder.MoveToSection(2);
            builder.MoveToParagraph(-1, -1);
            builder.Write(" Test2");

            TestUtil.Save(doc, @"DocBuilder\TestMoveToParaInSection.docx");

            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Section 1, Paragraph 1.\r" +
                "Section 1, Paragraph 2.\x000c"));

            Assert.That(doc.Sections[1].Body.GetText(), Is.EqualTo("Test1\r\x000c"));

            Assert.That(doc.Sections[2].Body.GetText(), Is.EqualTo("Section 2, Paragraph 1. Test2\x000c"));
        }

        [Test]
        public void TestMoveToParaStart()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToPara.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToParagraph(1, 0);
            builder.Writeln("Inserted text.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestMoveToParaStart.docx", null, false);

            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Section 1, Paragraph 1.\r" +
                "Inserted text.\r" +
                "Section 1, Paragraph 2.\x000c"));

            Assert.That(doc.Sections[1].Body.GetText(), Is.EqualTo("Section 2, Paragraph 1.\x000c"));
        }

        [Test]
        public void TestMoveToParaEnd()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToPara.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToParagraph(0, -1);
            builder.Write(" Inserted text.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestMoveToParaEnd.docx", null, false);
            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Section 1, Paragraph 1. Inserted text.\r" +
                "Section 1, Paragraph 2.\x000c"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestMoveToParaBeyondDoc()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToPara.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(3, 0);
        }

        /// <summary>
        /// WORDSNET-19480 ParagraphFormat.SpaceAfter validation issue.
        /// Word doesn't allow setting values ​​outside the range [0; 1584] for "SpaceAfter". Mimic Word.
        /// </summary>
        [TestCase(-2, true)]
        [TestCase(-1, true)]
        [TestCase(0, false)]
        [TestCase(10, false)]
        [TestCase(1584, false)]
        [TestCase(1585, true)]
        public void Test19480(double spaceAfter, bool expectedIsThrown)
        {
            bool isThrown = false;
            DocumentBuilder builder = new DocumentBuilder();
            builder.CurrentParagraph.ParagraphFormat.SpaceAfterAuto = false;

            try
            {
                builder.CurrentParagraph.ParagraphFormat.SpaceAfter = spaceAfter;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.That(e.ParamName, Is.EqualTo("SpaceAfter"));
                isThrown = true;
            }

            Assert.That(isThrown, Is.EqualTo(expectedIsThrown));
            Assert.That(builder.CurrentParagraph.ParagraphFormat.SpaceAfter, Is.EqualTo(isThrown ? 0 : spaceAfter));
        }

        /// <summary>
        /// Relates to WORDSNET-19480 Checks validation for "SpaceBefore".
        /// </summary>
        [TestCase(-2, true)]
        [TestCase(-1, true)]
        [TestCase(0, false)]
        [TestCase(10, false)]
        [TestCase(1584, false)]
        [TestCase(1585, true)]
        public void Test19480SpaceBefore(double spaceBefore, bool expectedIsThrown)
        {
            bool isThrown = false;
            DocumentBuilder builder = new DocumentBuilder();
            builder.CurrentParagraph.ParagraphFormat.SpaceBeforeAuto = false;

            try
            {
                builder.CurrentParagraph.ParagraphFormat.SpaceBefore = spaceBefore;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.That(e.ParamName, Is.EqualTo("SpaceBefore"));
                isThrown = true;
            }

            Assert.That(isThrown, Is.EqualTo(expectedIsThrown));
            Assert.That(builder.CurrentParagraph.ParagraphFormat.SpaceBefore, Is.EqualTo(isThrown ? 0 : spaceBefore));
        }

        /// <summary>
        /// Relates to WORDSNET-19667
        /// Checks how document builder inserts paragraph in the Word manner.
        /// </summary>
        [Test]
        public void Test19667InsertPara()
        {
            Document template = TestUtil.Open(@"DocBuilder\Test19667InsertPara.docx");

            // 1. Positioned to paragraph.
            Document doc = template.Clone();
            CheckParagraphInsertion(new DocumentBuilder(doc), doc.FirstSection.Body.FirstParagraph,
                DrColor.Yellow, DrColor.Blue, "ab\r", "\f");

            // 2. Positioned to first run.
            doc = template.Clone();
            CheckParagraphInsertion(new DocumentBuilder(doc), doc.FirstSection.Body.FirstParagraph.FirstRun,
                DrColor.Red, DrColor.Blue, "\r", "ab\f");

            // 3. Positioned to last run.
            doc = template.Clone();
            CheckParagraphInsertion(new DocumentBuilder(doc), doc.FirstSection.Body.FirstParagraph.GetLastRun(),
                DrColor.Red, DrColor.Blue, "a\r", "b\f");
        }

        /// <summary>
        /// Relates to WORDSNET-19667
        /// Checks what revisions are generated by AW while paragraph inserting.
        /// </summary>
        [Test]
        public void Test19667InsertParaRevision()
        {
            Document doc = TestUtil.Open(@"DocBuilder\Test19667InsertPara.docx");
            doc.StartTrackRevisions("am");

            CheckParagraphInsertion(new DocumentBuilder(doc), doc.FirstSection.Body.FirstParagraph.GetLastRun(),
                DrColor.Red, DrColor.Blue, "a\r", "b\f");

            Assert.That(doc.Revisions.Count, Is.EqualTo(1));
            Assert.That(doc.Revisions[0].RevisionType, Is.EqualTo(RevisionType.Insertion));
            Assert.That(ReferenceEquals(doc.FirstSection.Body.FirstParagraph, doc.Revisions[0].ParentNode), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-19667
        /// Checks with an image in the paragraph content.
        /// </summary>
        [Test]
        public void Test19667InsertParaImg()
        {
            Document doc = TestUtil.Open(@"DocBuilder\Test19667InsertParaImg.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.RunPr[FontAttr.Color], Is.EqualTo(DrColor.Green));

            CheckParagraphInsertion(new DocumentBuilder(doc), doc.FirstSection.Body.FirstParagraph.GetLastRun(),
                DrColor.Red, DrColor.Blue, "a\r", "b\f");
        }

        /// <summary>
        /// Relates to WORDSNET-19667
        /// Checks how document builder inserts paragraph to another empty one.
        /// </summary>
        [Test]
        public void Test19667InsertToEmptyPara()
        {
            Document doc = new Document();
            doc.FirstSection.Body.FirstParagraph.ParagraphBreakRunPr[FontAttr.Color] = DrColor.Red;

            CheckParagraphInsertion(new DocumentBuilder(doc), doc.FirstSection.Body.FirstParagraph,
                DrColor.Red, DrColor.Red, "\r", "\f");
        }

        /// <summary>
        /// WORDSNET-22834 Page break at bookmark position does not work.
        /// Word adds paragraph after the page break depending from the compatibility setting "splitPgBreakAndParaMark" and
        /// compatibility mode. Mimic the Word behavior for the client case to fix the problem.
        /// </summary>
        [Test]
        public void Test22834()
        {
            const string bookmarkName = "MYBOOKMARK";
            Document doc = TestUtil.Open(@"DocBuilder\Test22834.docx");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToBookmark(bookmarkName);

            builder.InsertBreak(BreakType.PageBreak);

            BookmarkEnd bookmarkEnd = (BookmarkEnd)builder.CurrentNode;
            Assert.That(bookmarkEnd.Name, Is.EqualTo(bookmarkName));
            Assert.That(bookmarkEnd.PreviousSibling, Is.Null);

            Run breakRun = (Run)((Paragraph)bookmarkEnd.ParentNode.PreviousSibling).LastChild;
            Assert.That(breakRun.Text, Is.EqualTo(ControlChar.PageBreak));

            BookmarkStart bookmarkStart = (BookmarkStart)breakRun.PreviousSibling;
            Assert.That(bookmarkStart.Name, Is.EqualTo(bookmarkName));

            // Add gold because of enterprise level of the client.
            TestUtil.OpenSaveOpen(@"DocBuilder\Test22834.docx");
        }

        /// <summary>
        /// Related with WORDSNET-22834
        /// The case checks that additional paragraph should not be added depending from the compatibility settings.
        /// </summary>
        [Test]
        public void Test22834NoPara()
        {
            Document doc = TestUtil.Open(@"DocBuilder\Test22834NoPara.docx");
            Paragraph para = doc.FirstSection.Body.LastParagraph;

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveTo(doc.FirstSection.Body.LastParagraph);

            builder.InsertBreak(BreakType.PageBreak);

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(31));
            Assert.That(ReferenceEquals(para, doc.FirstSection.Body.LastParagraph), Is.True);

            Assert.That(para.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(para.FirstRun.Text, Is.EqualTo(ControlChar.PageBreak));
        }

        /// <summary>
        /// Test that when the cursor is moved, the <see cref="ParagraphFormat"/> property retains its value, but
        /// the formatting properties are updated.
        /// </summary>
        [Test]
        public void TestParagraphFormat()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.PrependChild(new Paragraph(doc));
            body.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            body.PrependChild(new Paragraph(doc));
            body.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(0, 0);
            ParagraphFormat paragraphFormat = builder.ParagraphFormat;
            Assert.That(paragraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));

            builder.MoveToParagraph(1, 0);
            Assert.That(builder.ParagraphFormat, Is.SameAs(paragraphFormat));
            Assert.That(builder.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));
        }

        /// <summary>
        /// Related with WORDSNET-22834
        /// Checks the case when cursor positioned in the content of the paragraph.
        /// Actually the Word adds two paragraphs at this case. AW preserves previous behavior for a while.
        /// </summary>
        [TestCase("Test22834Content.docx")]
        public void Test22834Content(string fileName)
        {
            const string bookmarkName = "MYBOOKMARK";
            Document doc = TestUtil.Open(@"DocBuilder\" + fileName);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToBookmark(bookmarkName);

            builder.InsertBreak(BreakType.PageBreak);

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(31));
            Paragraph para = doc.FirstSection.Body.LastParagraph;

            Run run = (Run)para.FirstChild;
            Assert.That(run.Text, Is.EqualTo("A"));

            BookmarkStart bookmarkStart = (BookmarkStart)run.NextSibling;
            run = (Run)bookmarkStart.NextSibling;
            Assert.That(run.Text, Is.EqualTo(ControlChar.PageBreak));

            BookmarkEnd bookmarkEnd = (BookmarkEnd)run.NextSibling;
            run = (Run)bookmarkEnd.NextSibling;
            Assert.That(run.Text, Is.EqualTo("BC"));
        }

        /// <summary>
        /// WORDSNET-24033 <see cref="DocumentBuilder.InsertParagraph"/> works incorrectly when cursor is
        /// inside inline-level SDT
        /// It is implemented that <see cref="InvalidOperationException"/> is thrown in this case.
        /// </summary>
        [TestCase(0)]
        [TestCase(8)]
        [TestCase(-1)]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "Cannot insert a node of this type at this location.")]
        public void Test24033(int positionInSdt)
        {
            Document doc = TestUtil.Open(@"DocBuilder\Test24033.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const string expectedParagraphText = "Text before  Text in SDT Text after SDT\f";
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            Assert.That(paragraph.GetText(), Is.EqualTo(expectedParagraphText));

            StructuredDocumentTag sdt =
                (StructuredDocumentTag)paragraph.GetChild(NodeType.StructuredDocumentTag, 0, false);
            builder.MoveToStructuredDocumentTag(sdt, positionInSdt);

            // InsertParagraph throws an exception because the cursor is inside an inline-level SDT.
            builder.InsertParagraph();
        }

        private void CheckParagraphInsertion(DocumentBuilder builder, Node position,
                DrColor color1, DrColor color2, string content1, string content2)
        {
            builder.MoveTo(position);
            builder.InsertParagraphAsWord();

            Body body = builder.Document.FirstSection.Body;
            Assert.That(body.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(body.FirstParagraph.ParagraphBreakRunPr[FontAttr.Color], Is.EqualTo(color1));
            Assert.That(body.LastParagraph.ParagraphBreakRunPr[FontAttr.Color], Is.EqualTo(color2));

            Assert.That(body.FirstParagraph.Range.Text, Is.EqualTo(content1));
            Assert.That(body.LastParagraph.Range.Text, Is.EqualTo(content2));
        }
    }
}
