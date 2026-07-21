// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.IO;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderOther
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }





        [Test]
        public void TestMoveToBookmark()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToBookmark.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            Assert.That(builder.MoveToBookmark("Test1", true, false), Is.True);
            builder.Write("[BeforeStart]");
            Assert.That(builder.MoveToBookmark("Test1"), Is.True);
            Assert.That(builder.MoveToBookmark("NonExistantName"), Is.False);
            builder.Write("[AfterStart]");
            builder.MoveToBookmark("Test1", false, false);
            builder.Write("[BeforeEnd]");
            builder.MoveToBookmark("Test1", false, true);
            builder.Write("[AfterEnd]");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestMoveToBookmark.docx", null, false);
            doc.JoinRunsWithSameFormatting();

            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Has [BeforeStart][AfterStart]bookmark[BeforeEnd][AfterEnd] inside.\x000c"));

            BookmarkStart bmkStart = (BookmarkStart)doc.GetChild(NodeType.BookmarkStart, 0, true);
            Assert.That(bmkStart.NextSibling.GetText(), Is.EqualTo("[AfterStart]bookmark[BeforeEnd]"));
        }

        [Test]
        public void TestInsertTOC()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestInsertTOC.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertTableOfContents(@"\o ""1-3"" \h \z \u");
            TestUtil.SaveOpen(doc, @"DocBuilder\TestInsertTOC.docx", null, false);
        }

        /// <summary>
        /// WORDSNET-940 Add support for formula fields to DocumentBuilder.InsertField
        /// </summary>
        [Test]
        public void TestInsertFormula()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestInsertFormula.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToCell(0, 3, 1, 0);
            builder.InsertField("=SUM(ABOVE)", "");

            TestUtil.Save(doc, @"DocBuilder\TestInsertFormula.docx",  null, false);

            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 7, true);
            Assert.That(cell.GetText(), Is.EqualTo("\x0013=SUM(ABOVE)\x0014\x0015\x0007"));
        }

        /// <summary>
        /// Open the document in MS Word and check.
        /// </summary>
        [Test]
        public void TestInsertDeadField()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertField("TC TestTC", null);

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestInsertDeadFields.docx", null, false);

            FieldExtractorToCollection extractor = new FieldExtractorToCollection();
            extractor.Extract(doc);
            Assert.That(extractor.Fields.Count, Is.EqualTo(1));

            Field field = extractor.Fields[0];

            Assert.That(field.Start.FieldType, Is.EqualTo(FieldType.FieldTOCEntry));

            Assert.That(field.End.FieldType, Is.EqualTo(FieldType.FieldTOCEntry));
            Assert.That(field.End.HasSeparator, Is.EqualTo(false));

            Assert.That(field.GetFieldCode(), Is.EqualTo("TC TestTC"));
        }

        /// <summary>
        /// Test inserting form text fields.
        /// </summary>
        [Test]
        public void TestInsertTextInput()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            //This creates soft refresheable collection of form fields.
            Assert.That(doc.Range.FormFields.Count, Is.EqualTo(0));

            FormField formField = builder.InsertTextInput("Test1", TextFormFieldType.Regular, "", "test test", 0);
            Assert.That(formField, IsNot.Null());

            builder.InsertParagraph();
            builder.InsertTextInput("Test2", TextFormFieldType.Number, "0.00", "\x2002\x2002 123", 5);

            //Magic! the collection of form fields got updated.
            FormField f1 = doc.Range.FormFields["Test1"];
            Assert.That(f1 != null, Is.True);
            Assert.That(f1.Result, Is.EqualTo("test test"));

            FormField f2 = doc.Range.FormFields["Test2"];
            Assert.That(f2 != null, Is.True);
            Assert.That(f2.Result, Is.EqualTo("\x2002\x2002 123"));

            //Check bookmark is indeed created.
            Assert.That(doc.Range.Bookmarks["Test1"] != null, Is.True);

            TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderInsertTextInput.docx", null, false);
        }

        /// <summary>
        /// WORDSNET-5287 DocumentBuilder behaved improperly on inserting a multi-line formfield.
        /// Fixed by setting correct DocumentBuilder cursor position at <see cref="DocumentBuilder.InsertTextInput"/>
        /// </summary>
        [Test]
        public void Test5287()
        {
            const string runText = "This text must be outside the formfield.";

            // 1) insert TextInput with a Name specified as a last child.
            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertTextInput("test", TextFormFieldType.Regular, "", "line1\nline2", 100);
            builder.Writeln(runText);

            // This BookmarkEnd points to the end of the FormField nodes.
            BookmarkEnd bookmarkEnd = builder.Document.Range.Bookmarks["test"].BookmarkEnd;

            // Next node must be a Run with a text: "This text must be outside the formfield."
            VerifyRun(runText, bookmarkEnd.NextSibling);


            // 2) insert TextInput without a specified Name.
            builder = new DocumentBuilder();
            builder.InsertTextInput("", TextFormFieldType.Regular, "", "line1\nline2", 100);
            builder.Writeln(runText);

            // This FieldEnd points to the end of the FormField nodes.
            FieldEnd fieldEnd = builder.Document.Range.FormFields[0].Field.End;

            // Next node must be a Run with a text: "This text must be outside the formfield."
            VerifyRun(runText, fieldEnd.NextSibling);
        }

        [Test]
        public void TestInsertCheckBox()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            FormField formField = builder.InsertCheckBox("Test1", true, 22);
            Assert.That(formField, IsNot.Null());

            builder.InsertParagraph();
            builder.InsertCheckBox("Test2", false, 0);

            FormField f1 = doc.Range.FormFields["Test1"];
            Assert.That(f1 != null, Is.True);
            Assert.That(f1.Result, Is.EqualTo("1"));

            FormField f2 = doc.Range.FormFields["Test2"];
            Assert.That(f2 != null, Is.True);
            Assert.That(f2.Result, Is.EqualTo("0"));

            TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderInsertCheckBox.docx", null, false);
        }

        /// <summary>
        /// Test inserting a checkbox form field with setting checked and default properties separately.
        /// </summary>
        [Test]
        public void TestInsertCheckBoxWithCheckedAndDefault()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            FormField formField = builder.InsertCheckBox("Test1", true, false, 50);
            Assert.That(formField, IsNot.Null());

            builder.InsertParagraph();
            builder.InsertCheckBox("Test2", false, true, 50);

            FormField field1 = doc.Range.FormFields["Test1"];
            Assert.That(field1, IsNot.Null());
            Assert.That(field1.Checked, Is.EqualTo(false), "Field 1 checled");
            Assert.That(field1.Default, Is.EqualTo(true), "Field 1 default value");

            FormField field2 = doc.Range.FormFields["Test2"];
            Assert.That(field2, IsNot.Null());
            Assert.That(field2.Checked, Is.EqualTo(true), "Field 2 checled");
            Assert.That(field2.Default, Is.EqualTo(false), "Field 2 default value");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestInsertCheckBoxWithCheckedAndDefault.docx", null, false);

            field1 = doc.Range.FormFields["Test1"];
            Assert.That(field1, IsNot.Null());
            Assert.That(field1.Checked, Is.EqualTo(false), "Field 1 checled after reopen");
            Assert.That(field1.Default, Is.EqualTo(true), "Field 1 default value after reopen");

            field2 = doc.Range.FormFields["Test2"];
            Assert.That(field2, IsNot.Null());
            Assert.That(field2.Checked, Is.EqualTo(true), "Field 2 checled after reopen");
            Assert.That(field2.Default, Is.EqualTo(false), "Field 2 default value after reopen");
        }

        [Test]
        public void TestInsertComboBox()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            FormField formField = builder.InsertComboBox(
                "Test1",
                new string[] {"Line1", "Line2", "Line3"},
                1);
            Assert.That(formField, IsNot.Null());

            FormField f1 = doc.Range.FormFields["Test1"];
            Assert.That(f1 != null, Is.True);
            Assert.That(f1.Result, Is.EqualTo("Line2"));

            TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderInsertComboBox.docx", null, false);
        }

        [Test]
        public void TestDocumentSet()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.Write("Doc1, center.");
            Document doc1 = builder.Document;

            //Assign new doc to the builder.
            Document doc2 = new Document();
            builder.Document = doc2;
            builder.Write("Doc2, left.");

            builder.ParagraphFormat.StyleName = "Normal";

            Assert.That(doc1.GetText(), Is.EqualTo("Doc1, center.\x000c"));
            Paragraph para = (Paragraph)doc1.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));

            Assert.That(doc2.GetText(), Is.EqualTo("Doc2, left.\x000c"));
            para = (Paragraph)doc2.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Left));
        }

        /// <summary>
        /// WORDSNET-3052 Poor quality foot notes.
        /// Added a method to insert a footnote to document builder.
        /// </summary>
        [Test]
        public void TestInsertFootnote()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("Some text is added.");
            builder.InsertFootnote(FootnoteType.Footnote, "Footnote line 1.\rFootnote line 2.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestInsertFootnote.docx", null, false);
            doc.JoinRunsWithSameFormatting();

            // Check footnote.
            Footnote ftn = (Footnote)doc.FirstSection.Body.FirstParagraph.LastChild;
            Assert.That(ftn.FootnoteType, Is.EqualTo(FootnoteType.Footnote));
            Assert.That(ftn.IsAuto, Is.EqualTo(true));
            Assert.That(ftn.Font.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteReference));

            Paragraph para = (Paragraph)ftn.FirstChild;
            Assert.That(para.GetText(), Is.EqualTo("\x0002 Footnote line 1.\r"));
            Assert.That(para.ParagraphFormat.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteText));

            // Chekc footnote number.
            SpecialChar ftnNum = (SpecialChar)para.FirstChild;
            Assert.That(ftnNum.GetText(), Is.EqualTo("\x0002"));
            Assert.That(ftnNum.Font.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteReference));

            // Check text of the first paragraph.
            Run run = (Run)ftnNum.NextSibling;
            Assert.That(run.GetText(), Is.EqualTo(" Footnote line 1."));
            Assert.That(run.Font.StyleIdentifier, Is.EqualTo(StyleIdentifier.DefaultParagraphFont));

            // Check text of the second paragraph.
            para = (Paragraph)para.NextSibling;
            Assert.That(para.GetText(), Is.EqualTo("Footnote line 2.\r"));
            Assert.That(para.ParagraphFormat.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteText));
        }

        /// <summary>
        /// WORDSNET-11107 Support Custom Mark for FootNote
        ///
        /// InsertFootnote with reference mark must add 3 runs, and first run must contain reference mark.
        /// </summary>
        [Test]
        public void TestDefect11107InsertFootnote()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            string customMark = "test123456";
            string text = "testfootnote";
            Footnote footnote = builder.InsertFootnote(FootnoteType.Footnote, text, customMark);

            Assert.That(footnote, IsNot.Null());
            Assert.That(footnote.IsAuto, Is.False);
            Assert.That(footnote.ReferenceMark, Is.EqualTo(customMark));
            Assert.That(footnote.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(footnote.FirstParagraph.Runs.Count, Is.EqualTo(3));

            Assert.That(footnote.FirstParagraph.Runs[0].Text, Is.EqualTo(customMark));
            Assert.That(footnote.FirstParagraph.Runs[1].Text, Is.EqualTo(" "));
            Assert.That(footnote.FirstParagraph.Runs[2].Text, Is.EqualTo(text));
        }


        /// <summary>
        /// Tests the <see cref="DocumentBuilder.MoveTo(Node)"/> method on moving to block level bookmarks.
        /// </summary>
        [Test]
        public void TestMoveToBlockLevelBookmark()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            // Generate document of structure:
            // bookmarkStart1
            // paragraph1
            // bookmarkEnd1
            // sdt1
            //   paragraph2
            //     run1
            //   bookmarkStart2
            // table1
            //   bookmarkEnd2
            //   row1
            //     sdt2
            //       cell1
            //         paragraph3
            //         bookmarkStart3
            //   row2
            //     cell2
            //       paragraph4
            //         run2
            //       bookmarkEnd3
            // paragraph5

            BookmarkStart bookmarkStart1 = new BookmarkStart(doc, "BM1");
            body.AppendChild(bookmarkStart1);
            Paragraph paragraph1 = new Paragraph(doc);
            body.AppendChild(paragraph1);
            BookmarkEnd bookmarkEnd1 = new BookmarkEnd(doc, "BM1");
            body.AppendChild(bookmarkEnd1);

            StructuredDocumentTag sdt1 = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            sdt1.RemoveAllChildren();
            body.AppendChild(sdt1);
            Paragraph paragraph2 = new Paragraph(doc);
            sdt1.AppendChild(paragraph2);
            Run run1 = new Run(doc);
            paragraph2.AppendChild(run1);
            BookmarkStart bookmarkStart2 = new BookmarkStart(doc, "BM2");
            sdt1.AppendChild(bookmarkStart2);

            Table table1 = new Table(doc);
            body.AppendChild(table1);
            BookmarkEnd bookmarkEnd2 = new BookmarkEnd(doc, "BM2");
            table1.AppendChild(bookmarkEnd2);

            Row row1 = new Row(doc);
            table1.AppendChild(row1);
            StructuredDocumentTag sdt2 = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Cell);
            sdt2.RemoveAllChildren();
            row1.AppendChild(sdt2);
            Cell cell1 = new Cell(doc);
            sdt2.AppendChild(cell1);
            Paragraph paragraph3 = new Paragraph(doc);
            cell1.AppendChild(paragraph3);
            BookmarkStart bookmarkStart3 = new BookmarkStart(doc, "BM3");
            cell1.AppendChild(bookmarkStart3);

            Row row2 = new Row(doc);
            table1.AppendChild(row2);
            Cell cell2 = new Cell(doc);
            row2.AppendChild(cell2);
            Paragraph paragraph4 = new Paragraph(doc);
            cell2.AppendChild(paragraph4);
            Run run2 = new Run(doc);
            paragraph4.AppendChild(run2);
            BookmarkEnd bookmarkEnd3 = new BookmarkEnd(doc, "BM3");
            cell2.AppendChild(bookmarkEnd3);

            Paragraph paragraph5 = new Paragraph(doc);
            body.AppendChild(paragraph5);

            // Test moving to bookmarks.
            DocumentBuilder builder = new DocumentBuilder(doc);
            CheckMove(builder, bookmarkStart1, paragraph1, null);
            CheckMove(builder, bookmarkStart2, paragraph3, null);
            CheckMove(builder, bookmarkStart3, paragraph4, run2);
            CheckMove(builder, bookmarkEnd1, paragraph2, run1);
            CheckMove(builder, bookmarkEnd2, paragraph3, null);
            CheckMove(builder, bookmarkEnd3, paragraph5, null);
        }

        /// <summary>
        /// Checks that the <see cref="DocumentBuilder.MoveTo(Node)"/> method moves cursor to expected node.
        /// </summary>
        private static void CheckMove(DocumentBuilder builder, Node moveTo,
            Paragraph expectedCurrentParagraph, Node expectedCurrentNode)
        {
            builder.MoveTo(moveTo);
            Assert.That(builder.CurrentParagraph, Is.SameAs(expectedCurrentParagraph));
            Assert.That(builder.CurrentNode, Is.SameAs(expectedCurrentNode));
        }


        /// <summary>
        /// WORDSNET-8216 DocumentBuilder does not inherit Bookmark formatting when moving cursor to that Bookmark.
        /// Try to get formatting from previous inline. Seems Word do the same.
        /// </summary>
        [Test]
        public void TestJira8216()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestJira8216.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToBookmark("bold");
            builder.Write("This is a very cool bookmark.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestJira8216.docx", null, false);
            Run run = doc.FirstSection.Body.FirstParagraph.Runs[2];
            Assert.That(run.GetText(), Is.EqualTo("This is a very cool bookmark."));
            Assert.That(run.Font.Bold, Is.True);
        }

        /// <summary>
        /// WORDSNET-9823 Setting Font.Color in DocumentBuilder does not change the color of the first paragraph's
        /// list label.
        /// andrnosk: It seems the problem occurs because upon SetRunAttr through the DocumentBuilder,
        /// if CurrentParagraph is empty, we have to set ParagraphBreakRunPr attrs too.
        /// </summary>
        [Test]
        public void TestJira9823()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Font.Bold = true;
            builder.Font.Color = Color.Red;

            builder.ListFormat.ApplyNumberDefault();
            builder.Writeln("Item 1");
            builder.Writeln("Item 2");
            builder.Write("Item 3");

            ParagraphCollection papas = builder.Document.FirstSection.Body.Paragraphs;
            Assert.That(papas[0].ListLabel.Font.Bold, Is.True);
            Assert.That(Color.Red.ToArgb(), Is.EqualTo(papas[0].ListLabel.Font.Color.ToArgb()));
            Assert.That(papas[1].ListLabel.Font.Bold, Is.True);
            Assert.That(papas[2].ListLabel.Font.Bold, Is.True);
        }

        /// <summary>
        /// WORDSNET-10765 DocumentBuilder.Font formatting is incorrectly applied using 14.8.0.
        /// When we set run attributes, we also set them for ParagraphBreakRunPr of CurrentParagraph (see WORDSNET-9823).
        /// Therefore, clearing these attributes should clear them for ParagraphBreakRunPr too.
        /// </summary>
        [Test]
        public void TestJira10765()
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.Writeln();
            builder.Font.Color = Color.Green;
            builder.Write("Green text");

            builder.Font.ClearFormatting();
            builder.Write(" non green");

            builder.Writeln();
            builder.Writeln();

            builder.ListFormat.ApplyBulletDefault();
            builder.Font.Color = Color.Blue;
            builder.Font.ClearFormatting();
            builder.Write("list item 1");

            builder.Font.ClearFormatting();
            builder.Writeln();

            builder.Font.Color = Color.Blue;
            builder.Write("list item 2");

            builder.Font.ClearFormatting();
            builder.Writeln();

            builder.Font.Color = Color.Blue;
            builder.Write("list item 3");
            builder.Font.ClearFormatting();

            ParagraphCollection paras = builder.Document.FirstSection.Body.Paragraphs;

            CheckParaBreakRunPrColor(paras[1]);

            CheckParaBreakRunPrColor(paras[3]);
            CheckParaListLabelColor(paras[3]);

            CheckParaBreakRunPrColor(paras[4]);
            CheckParaListLabelColor(paras[4]);

            CheckParaBreakRunPrColor(paras[5]);
            CheckParaListLabelColor(paras[5]);
        }

        /// <summary>
        /// WORDSNET-8087 DocumentBuilder.Font.ClearFormatting do not reset to default font formatting.
        /// Should change ParagraphBreakRunPr in SetRunAttr() and ClearRunAttrs() for empty paragraphs too.
        /// </summary>
        [Test]
        public void TestJira8087()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());

            // We need a bookmark by a customer scenario.
            builder.Document.FirstSection.Body.FirstParagraph.AppendChild(new BookmarkStart(builder.Document));
            builder.MoveToDocumentStart();

            builder.Writeln("Sample Document");
            Paragraph para = (Paragraph)builder.CurrentParagraph.PreviousSibling;
            Assert.That(para.FirstRun.Font.Size, Is.EqualTo(12).Within(0.1));
            Assert.That(builder.CurrentParagraph.ParagraphBreakFont.Size, Is.EqualTo(12).Within(0.1));

            builder.Font.Size = 48;
            builder.Writeln("Test Large Text");
            para = (Paragraph)builder.CurrentParagraph.PreviousSibling;
            Assert.That(para.FirstRun.Font.Size, Is.EqualTo(48).Within(0.1));
            Assert.That(builder.CurrentParagraph.ParagraphBreakFont.Size, Is.EqualTo(48).Within(0.1));

            builder.InsertBreak(BreakType.SectionBreakNewPage);

            builder.Font.ClearFormatting();

            // Table 1
            builder.StartTable();
            builder.InsertCell();
            builder.Write("Tbl 1");
            para = builder.CurrentParagraph;
            Assert.That(para.FirstRun.Font.Size, Is.EqualTo(12).Within(0.1));
            Assert.That(para.ParagraphBreakFont.Size, Is.EqualTo(12).Within(0.1));
            builder.EndTable();

            builder.InsertParagraph();
            Assert.That(builder.CurrentParagraph.ParagraphBreakFont.Size, Is.EqualTo(12).Within(0.1));

            // Table 2
            builder.StartTable();
            builder.InsertCell();
            builder.Write("Tbl 2");
            para = builder.CurrentParagraph;
            Assert.That(para.FirstRun.Font.Size, Is.EqualTo(12).Within(0.1));
            Assert.That(para.ParagraphBreakFont.Size, Is.EqualTo(12).Within(0.1));
        }


        /// <summary>
        /// WORDSNET-6124 Allow DocumentBuilder to move correctly to StructuredDocumentTags
        /// check that DocumentBuilder can move to sdt block level and tables
        /// </summary>
        [Test]
        public void TestJira6124()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestJira6124.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            // inlineLevelTag
            StructuredDocumentTag inlineSdtNode = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            builder.MoveTo(inlineSdtNode);
            Assert.That(inlineSdtNode, Is.EqualTo(builder.CurrentNode));

            // blockLevelTag
            StructuredDocumentTag blockSdtNode = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 1, true);

            Table tableNode = (Table)doc.GetChild(NodeType.Table, 0, true);

            foreach (CompositeNode node in new CompositeNode[] { blockSdtNode, tableNode })
            {
                builder.MoveTo(node);
                Assert.That(builder.CurrentParagraph, Is.EqualTo(node.GetChild(NodeType.Paragraph, 0, true)));
            }
        }

        /// <summary>
        /// WORDSNET-17397 Add feature to insert Horizontal Rule into document.
        /// </summary>
        [Test]
        public void TestJira17397()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertHorizontalRule();

            //Tests that the shape was saved successfully.
            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestJira17397.docx", new OoxmlSaveOptions(), false);
            Shape shapeHorizontalRule = (Shape)doc.GetChildNodes(NodeType.Shape, true)[0];

            Assert.That(shapeHorizontalRule.MarkupLanguage, Is.EqualTo(ShapeMarkupLanguage.Vml));
            Assert.That(shapeHorizontalRule.Height, Is.EqualTo(1.5));
            Assert.That(shapeHorizontalRule.Width, Is.EqualTo(doc.FirstSection.PageSetup.ContentWidth));
            Assert.That(shapeHorizontalRule.ShapeType, Is.EqualTo(ShapeType.Rectangle));
            Assert.That(WrapType.Inline, Is.EqualTo(shapeHorizontalRule.WrapType));
        }

        /// <summary>
        /// Tests that check box can be inserted into inline SDT.
        /// </summary>
        [Test]
        public void TestJira17319Builder()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestJira17319Builder.docx");

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            sdt.IsShowingPlaceholderText = false;

            DocumentBuilder builder = new DocumentBuilder(doc);
            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);

            builder.MoveTo(run);
            builder.InsertCheckBox("test", true, 0);
        }

        /// <summary>
        /// WORDSNET-18182 Add feature to set properties of horizontal rule.
        /// </summary>
        [TestCase(HorizontalRuleAlignment.Right, 50, 10, -16776961, true, false)]
        [TestCase(HorizontalRuleAlignment.Left, 100, 1.5, -8355712, false, true)] // Checks defaults.
        public void Test18182(HorizontalRuleAlignment expectedAlignment, double expectedWidthPercent, double expectedHeight,
            int argbColor, bool expectedNoShade, bool isDefault)
        {
            DocumentBuilder builder = new DocumentBuilder();

            if (!isDefault)
            {
                HorizontalRuleFormat horizontalRuleFormat = builder.InsertHorizontalRule().HorizontalRuleFormat;
                horizontalRuleFormat.Alignment = expectedAlignment;
                horizontalRuleFormat.WidthPercent = expectedWidthPercent;
                horizontalRuleFormat.Height = expectedHeight;
                horizontalRuleFormat.Color = Color.FromArgb(argbColor);
                horizontalRuleFormat.NoShade = expectedNoShade;
            }
            else
                builder.InsertHorizontalRule();

            OoxmlSaveOptions so = new OoxmlSaveOptions();
            Document doc = TestUtil.SaveOpen(builder.Document, @"DocBuilder\Test18182.docx", so, false);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0,true);
            HorizontalRuleFormat savedHorizontalRuleFormat = shape.HorizontalRuleFormat;

            Assert.That(savedHorizontalRuleFormat.Alignment, Is.EqualTo(expectedAlignment));
            Assert.That(savedHorizontalRuleFormat.WidthPercent, Is.EqualTo(expectedWidthPercent));
            Assert.That(savedHorizontalRuleFormat.Height, Is.EqualTo(expectedHeight));
            Assert.That(savedHorizontalRuleFormat.Color.ToArgb(), Is.EqualTo(argbColor));
            Assert.That(savedHorizontalRuleFormat.NoShade, Is.EqualTo(expectedNoShade));
        }

        /// <summary>
        /// Relates to WORDSNET-18182. Checks the behavior of a validation for boundary values.
        /// </summary>
        [TestCase(0, -1)]
        [TestCase(101, 1585)]
        public void Test18182CheckBoundaryValues(double widthPercent,  double height)
        {
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            try
            {
                SystemPal.SetStandardCulture();
                SystemPal.SetStandardUICulture();

                DocumentBuilder builder = new DocumentBuilder();
                HorizontalRuleFormat horizontalRuleFormat = builder.InsertHorizontalRule().HorizontalRuleFormat;

                int countThrows = 0;
                try
                {
                    horizontalRuleFormat.WidthPercent = widthPercent;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Assert.That(e.Message.TrimEnd('\'', ')').EndsWith("WidthPercent"), Is.True);
                    countThrows++;
                }

                try
                {
                    horizontalRuleFormat.Height = height;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Assert.That(e.Message.TrimEnd('\'', ')').EndsWith("Height"), Is.True);
                    countThrows++;
                }

                Assert.That(countThrows, Is.EqualTo(2));
            }
            finally
            {
                SystemPal.RestoreCulture();
                SystemPal.RestoreUICulture();
            }
        }

        /// <summary>
        /// Case for WORDSNET-13983
        /// Writing emphasis marks for simple runs.
        /// </summary>
        [Test]
        public void Test13983_EmphasisRun()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.EmphasisMark = EmphasisMark.UnderSolidCircle;

            builder.Write("Emphasis text");
            builder.Writeln();
            builder.Font.ClearFormatting();
            builder.Write("Simple text");

            ParagraphCollection paras = builder.Document.FirstSection.Body.Paragraphs;
            Assert.That(paras[0].FirstRun.Font.EmphasisMark, Is.EqualTo(EmphasisMark.UnderSolidCircle));
            Assert.That(paras[1].FirstRun.Font.EmphasisMark, Is.EqualTo(EmphasisMark.None));
        }

        /// <summary>
        /// Case for WORDSNET-13983
        /// Writing emphasis marks for a numbered list.
        /// </summary>
        [Test]
        public void Test13983_EmphasisList()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.EmphasisMark = EmphasisMark.UnderSolidCircle;

            builder.ListFormat.ApplyNumberDefault();
            builder.Writeln("Item 1");
            builder.Writeln("Item 2");
            builder.Write("Item 3");

            ParagraphCollection paras = builder.Document.FirstSection.Body.Paragraphs;
            Assert.That(paras[0].ListLabel.Font.EmphasisMark, Is.EqualTo(EmphasisMark.UnderSolidCircle));
            Assert.That(paras[1].ListLabel.Font.EmphasisMark, Is.EqualTo(EmphasisMark.UnderSolidCircle));
            Assert.That(paras[2].ListLabel.Font.EmphasisMark, Is.EqualTo(EmphasisMark.UnderSolidCircle));
        }

        /// <summary>
        /// WORDSNET-10148 Tests cursor moving to a specific character index within a paragraph.
        /// </summary>
        [Test]
        public void TestMoveToParagraphCharIndex()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Paragraph 1");
            builder.Write("Run 1 ");
            builder.Write("Run 2 ");
            builder.Writeln("Run 3");

            Paragraph paragraph1 = doc.FirstSection.Body.Paragraphs[0];
            Paragraph paragraph2 = doc.FirstSection.Body.Paragraphs[1];

            Assert.That(paragraph1.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));

            builder.MoveToParagraph(0, 9);

            // Runs are split only after inserting contents.
            Assert.That(builder.CurrentNode, Is.SameAs(paragraph1.FirstRun));
            Assert.That(paragraph1.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));

            const string insertedText = " (Inserted)";
            builder.Write(insertedText);
            Assert.That(paragraph1.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            Assert.That(paragraph1.Runs[0].Text, Is.EqualTo("Paragraph"));
            Assert.That(paragraph1.Runs[1].Text, Is.EqualTo(insertedText));
            Assert.That(paragraph1.Runs[2].Text, Is.EqualTo(" 1"));
            Assert.That(builder.CurrentNode, Is.SameAs(paragraph1.Runs[2]));

            builder.MoveToParagraph(0, -(2 + insertedText.Length) - 1);
            Assert.That(builder.CurrentNode, Is.SameAs(paragraph1.Runs[1]));

            builder.Write(insertedText);

            Assert.That(paragraph1.GetText(), Is.EqualTo("Paragraph" + insertedText + insertedText + " 1\r"));

            builder.MoveToParagraph(1, 6);
            Assert.That(builder.CurrentNode, Is.SameAs(paragraph2.Runs[1]));
            Assert.That(paragraph2.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));

            builder.Write(">");
            builder.Write("> ");
            Assert.That(paragraph2.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(5));
            Assert.That(paragraph2.Runs[0].Text, Is.EqualTo("Run 1 "));
            Assert.That(paragraph2.Runs[1].Text, Is.EqualTo(">"));
            Assert.That(paragraph2.Runs[2].Text, Is.EqualTo("> "));
            Assert.That(paragraph2.Runs[3].Text, Is.EqualTo("Run 2 "));
            Assert.That(paragraph2.Runs[4].Text, Is.EqualTo("Run 3"));
            Assert.That(builder.CurrentNode, Is.SameAs(paragraph2.Runs[3]));

            builder.MoveToParagraph(1, -2);
            builder.Write(">>");

            Assert.That(paragraph2.GetText(), Is.EqualTo("Run 1 >> Run 2 Run >>3\r"));
        }

        /// <summary>
        /// WORDSNET-10148 Tests cursor moving to the special character indexes (0, -1) within a paragraph.
        /// </summary>
        [Test]
        public void TestMoveToParagraphSpecialCharIndex()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartBookmark("Bookmark1");
            builder.Write("Run 1 ");
            builder.Write("Run 2 ");
            builder.EndBookmark("Bookmark1");
            builder.Writeln("");

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(paragraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));

            builder.MoveToParagraph(0, -1);

            Assert.That(builder.CurrentNode, Is.Null);
            Assert.That(builder.CurrentParagraph, Is.SameAs(paragraph));

            builder.MoveToParagraph(0, 0);

            Assert.That(builder.CurrentNode.NodeType, Is.EqualTo(NodeType.BookmarkStart));
            Assert.That(builder.CurrentParagraph, Is.SameAs(paragraph));
        }

        /// <summary>
        /// WORDSNET-10148 Tests cursor moving to a specific character index within a table cell.
        /// </summary>
        [Test]
        public void TestMoveToCellCharIndex()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();

            Cell cell1 = builder.InsertCell();
            builder.Writeln("Cell 1");
            builder.Write("Paragraph 2");

            Cell cell2 = builder.InsertCell();
            builder.Writeln("Cell 2");
            builder.Write("Paragraph 2");

            builder.EndRow();
            builder.EndTable();

            Debug.Assert(cell1.Paragraphs.Count == 2);
            Debug.Assert(cell2.Paragraphs.Count == 2);

            builder.MoveToCell(0, 0, 1, -1);
            Assert.That(builder.CurrentNode, Is.Null);
            Assert.That(builder.CurrentParagraph, Is.SameAs(cell2.LastParagraph));

            builder.Write(" Test");
            Assert.That(cell2.LastParagraph.GetText(), Is.EqualTo("Paragraph 2 Test\a"));

            builder.MoveToCell(0, 0, 1, 0);
            Assert.That(builder.CurrentNode, Is.SameAs(cell2.FirstParagraph.FirstRun));
            Assert.That(builder.CurrentParagraph, Is.SameAs(cell2.FirstParagraph));

            builder.Write("Test ");
            Assert.That(cell2.FirstParagraph.GetText(), Is.EqualTo("Test Cell 2\r"));

            builder.MoveToCell(0, 0, 0, -14);
            Assert.That(builder.CurrentNode, Is.SameAs(cell1.FirstParagraph.FirstRun));
            Assert.That(builder.CurrentParagraph, Is.SameAs(cell1.FirstParagraph));

            builder.Write("#");

            builder.MoveToCell(0, 0, 0, 18);
            Assert.That(builder.CurrentNode, Is.SameAs(cell1.LastParagraph.FirstRun));
            Assert.That(builder.CurrentParagraph, Is.SameAs(cell1.LastParagraph));

            builder.Write("#");
            Assert.That(cell1.GetText(), Is.EqualTo("Cell #1\rParagraph #2\a"));
        }


        /// <summary>
        /// Relates to WORDSNET-27240.
        /// Tests InsertGroupShape() with an empty list of shapes.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The shapes parameter shall contain at least one shape.")]
        public void Test27240_Empty()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            builder.InsertGroupShape();
        }

        /// <summary>
        /// Relates to WORDSNET-27240.
        /// Tests InsertGroupShape() with dimensions with an empty list of shapes.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The shapes parameter shall contain at least one shape.")]
        public void Test27240_EmptyWithSize()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            builder.InsertGroupShape(100, 100, 100, 100);
        }



        /// <summary>
        /// WORDSNET-27467 Grouping group shapes resets inner group shapes dimensions.
        /// Tests transformation data update when grouping shapes.
        /// </summary>
        [Test]
        public void Test27467_Level0()
        {
            double[][] expTransforms = {
                // Group 100017
                new double[] { 0, 0, 723900, 1206500, 0, 0, 723900, 1206500 },
                new double[] { 0, 0, 590550, 1143000 },
                new double[] { 127000, 63500, 596900, 1143000 },
                // Group 100018
                new double[] { 0, 0, 723900, 1206500, 0, 0, 723900, 1206500 },
                new double[] { 0, 0, 590550, 1143000 },
                new double[] { 127000, 63500, 596900, 1143000 },
                // Group 100019
                new double[] { 0, 0, 723900, 1206500, 0, 0, 723900, 1206500 },
                new double[] { 0, 0, 590550, 1143000 },
                new double[] { 127000, 63500, 596900, 1143000 },
                // Group 100020
                new double[] { 0, 0, 723900, 1206500, 0, 0, 723900, 1206500 },
                new double[] { 0, 0, 590550, 1143000 },
                new double[] { 127000, 63500, 596900, 1143000 },
            };

            Document doc = TestUtil.Open(@"DocBuilder\Test27467_Lvl0.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            List<Shape> shapes = doc.FirstSection.Body.GetChildNodes(NodeType.Shape, true).ToList<Shape>();
            for (int i = 0; i < 4; i++)
            {
                builder.MoveTo(doc.FirstSection.Body.FirstParagraph);
                Shape shape1 = shapes[i * 2];
                Shape shape2 = shapes[i * 2 + 1];
                GroupShape group = builder.InsertGroupShape(shape1, shape2);

                // Checks transformations after GroupShape insert.
                CheckTransform(group.DmlNode.Transform, expTransforms[i * 3]);
                CheckTransform(shape1.DmlNode.Transform, expTransforms[i * 3 + 1]);
                CheckTransform(shape2.DmlNode.Transform, expTransforms[i * 3 + 2]);
            }
        }

        /// <summary>
        /// WORDSNET-27467 Grouping group shapes resets inner group shapes dimensions.
        /// Tests transformation data update when grouping group shapes.
        /// </summary>
        [Test]
        public void Test27467_Level1()
        {
            double[][] expTransforms = {
                // Group 100025
                new double[] { 0, 0, 1616075, 1209675, 0, 0, 1616075, 1209675 },
                new double[] { 895350, 0, 720725, 1209675, 0, 0, 720725, 1209675 },
                new double[] { 0, 0, 730250, 1209675, 0, 0, 730250, 1209675 },
                // Group 100026
                new double[] { 0, 0, 1606550, 1209675, 0, 0, 1606550, 1209675 },
                new double[] { 885825, 0, 720725, 1209675, 0, 0, 720725, 1209675 },
                new double[] { 0, 0, 720725, 1209675, 0, 0, 720725, 1209675 },
            };

            Document doc = TestUtil.Open(@"DocBuilder\Test27467_Lvl1.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            List<GroupShape> grShapes = doc.FirstSection.Body.GetChildNodes(NodeType.GroupShape, true).ToList<GroupShape>();
            for (int i = 0; i < 2; i++)
            {
                builder.MoveTo(doc.FirstSection.Body.FirstParagraph);
                GroupShape grShape1 = grShapes[i * 2];
                GroupShape grShape2 = grShapes[i * 2 + 1];
                GroupShape newGroup = builder.InsertGroupShape(grShape1, grShape2);

                // Checks transformations after GroupShape insert.
                CheckTransform(newGroup.DmlNode.Transform, expTransforms[i * 3]);
                CheckTransform(grShape1.DmlNode.Transform, expTransforms[i * 3 + 1]);
                CheckTransform(grShape2.DmlNode.Transform, expTransforms[i * 3 + 2]);
            }
        }

        /// <summary>
        /// WORDSNET-27467 Grouping group shapes resets inner group shapes dimensions.
        /// Tests transformation data update when grouping nested group shapes.
        /// </summary>
        [Test]
        public void Test27467_Level2()
        {
            double[][] expTransforms = {
                new double[] { 0, 0, 3387725, 1209675, 0, 0, 3387725, 1209675 },
                new double[] { 1771650, 0, 1616075, 1209675, 0, 0, 1616075, 1209675 },
                new double[] { 0, 0, 1606550, 1209675, 0, 0, 1606550, 1209675 },
            };

            Document doc = TestUtil.Open(@"DocBuilder\Test27467_Lvl2.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveTo(doc.FirstSection.Body.FirstParagraph);
            GroupShape grShape1 = (GroupShape)doc.FirstSection.Body.GetChild(NodeType.GroupShape, 0, true);
            GroupShape grShape2 = (GroupShape)doc.FirstSection.Body.GetChild(NodeType.GroupShape, 3, true);
            GroupShape newGroup = builder.InsertGroupShape(grShape1, grShape2);

            // Checks transformations after GroupShape insert.
            CheckTransform(newGroup.DmlNode.Transform, expTransforms[0]);
            CheckTransform(grShape1.DmlNode.Transform, expTransforms[1]);
            CheckTransform(grShape2.DmlNode.Transform, expTransforms[2]);
        }

        /// <summary>
        /// Checks DML transformation data.
        /// </summary>
        private static void CheckTransform(DmlTransform transform, params double[] expTrans)
        {
            Assert.That(transform.X, Is.EqualTo(expTrans[0]).Within(0.1));
            Assert.That(transform.Y, Is.EqualTo(expTrans[1]).Within(0.1));
            Assert.That(transform.Width, Is.EqualTo(expTrans[2]).Within(0.1));
            Assert.That(transform.Height, Is.EqualTo(expTrans[3]).Within(0.1));

            DmlGroupTransform grTransform = transform as DmlGroupTransform;
            if (grTransform == null)
                return;

            Assert.That(grTransform.ChildX, Is.EqualTo(expTrans[4]).Within(0.1));
            Assert.That(grTransform.ChildY, Is.EqualTo(expTrans[5]).Within(0.1));
            Assert.That(grTransform.ChildWidth, Is.EqualTo(expTrans[6]).Within(0.1));
            Assert.That(grTransform.ChildHeight, Is.EqualTo(expTrans[7]).Within(0.1));
        }


        private static DataTable GetDataTable()
        {
            DataTable table = TestTableUtil.GetTestTable("Asset Industry Benchmark");
            TestTableUtil.AddColumns(table, "Asset Name", "IRS", "Average IRS", "IRSDelta", "IRSArrow", "IRSPerDelta", "RRS", "Average RRS", "RRSDelta");
            TestTableUtil.AddRow(table, "Test Asset", "768", "520", "-248", "▲", "-48", "768", null, null);

            return table;
        }

        /// <summary>
        /// Checks color of paragraph break.
        /// </summary>
        private static void CheckParaBreakRunPrColor(Paragraph para)
        {
            Assert.That(para.ParagraphBreakRunPr.Color, Is.EqualTo(DrColor.Empty));
        }

        /// <summary>
        /// Checks color of paragraph list label.
        /// </summary>
        private static void CheckParaListLabelColor(Paragraph para)
        {
            Assert.That(para.ListLabel.Font.Color, Is.EqualTo(Color.Empty));
        }

        /// <summary>
        /// Verifies <paramref name="node"/> type and text.
        /// </summary>
        private static void VerifyRun(string runText, Node node)
        {
            Assert.That(node, IsNot.Null());
            Assert.That(node.NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(node.GetText(), Is.EqualTo(runText));
        }


    }
}
