// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/09/2022 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Words.Markup;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    /// <summary>
    /// Test moving cursor to a <see cref="StructuredDocumentTag"/>.
    /// </summary>
    [TestFixture]
    public class TestDocBuilderStructuredDocumentTag
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }


        /// <summary>
        /// Tests moving the cursor to an inline-level SDT.
        /// </summary>
        [Test]
        public void TestMovingToInlineLevelSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);

            const int sdtIndex = 1;
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.StructuredDocumentTags[sdtIndex];
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Inline));
            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[3];
            Assert.That(paragraph.GetText(), Is.EqualTo("Text. Inline-level SDT. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("Inline-level SDT."));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.Write("At start. ");

            builder.MoveToStructuredDocumentTag(sdt, -1);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.Write(" At end.");
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));

            Assert.That(paragraph.GetText(), Is.EqualTo("Text. At start. Inline-level SDT. At end. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("At start. Inline-level SDT. At end."));

            sdt.RemoveAllChildren();
            Assert.That(paragraph.GetText(), Is.EqualTo("Text.  Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo(""));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            builder.Write("In SDT.");

            Assert.That(paragraph.GetText(), Is.EqualTo("Text. In SDT. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("In SDT."));

            sdt.RemoveAllChildren();
            builder.MoveToStructuredDocumentTag(sdt, -1);
            builder.Write("In SDT.");

            Assert.That(paragraph.GetText(), Is.EqualTo("Text. In SDT. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("In SDT."));

            builder.MoveToStructuredDocumentTag(sdt, 3);
            builder.Write("inline ");

            Assert.That(paragraph.GetText(), Is.EqualTo("Text. In inline SDT. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("In inline SDT."));

            builder.MoveToStructuredDocumentTag(sdtIndex, -5);
            builder.Write("level ");

            Assert.That(paragraph.GetText(), Is.EqualTo("Text. In inline level SDT. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("In inline level SDT."));

            // Move to the end of the SDT.
            builder.MoveToStructuredDocumentTag(sdtIndex, 20);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.Write(" At end.");
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);

            // Move to the beginning of the SDT.
            builder.MoveToStructuredDocumentTag(sdtIndex, -29);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.Write("At start.");

            Assert.That(paragraph.GetText(), Is.EqualTo("Text. At start.In inline level SDT. At end. Text.\r"));
            Assert.That(sdt.GetText(), Is.EqualTo("At start.In inline level SDT. At end."));

            builder.InsertNode(new Run(doc, " "));
            Assert.That(sdt.GetText(), Is.EqualTo("At start. In inline level SDT. At end."));
        }

        /// <summary>
        /// Tests moving the cursor to a block-level SDT.
        /// </summary>
        [Test]
        public void TestMovingToBlockLevelSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const int sdtIndex = 0;
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.StructuredDocumentTags[sdtIndex];
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Block));
            Assert.That(sdt.GetText(), Is.EqualTo("Block-level structured document tag.\rSecond line.\r"));
            // First paragraph of the document is inside the sdt.
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            builder.Write("At start. ");

            builder.MoveToStructuredDocumentTag(sdt, -2);
            builder.Write(" At end.");

            Assert.That(sdt.GetText(), Is.EqualTo("At start. Block-level structured document tag.\rSecond line. At end.\r"));

            builder.MoveToStructuredDocumentTag(sdt, 45);
            builder.Write(" contains 2 lines");

            builder.MoveToStructuredDocumentTag(sdtIndex, -11);
            builder.Write(" is last");

            Assert.That(sdt.GetText(), Is.EqualTo("At start. Block-level structured document tag contains 2 lines.\rSecond line is last. At end.\r"));

            // Move cursor outside of the removing nodes.
            builder.MoveToParagraph(2, 0);
            sdt.RemoveAllChildren();
            builder.MoveToStructuredDocumentTag(sdtIndex, -1);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.InsertParagraph();
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            Assert.That(sdt.GetText(), Is.EqualTo("\r"));

            sdt.RemoveAllChildren();
            builder.MoveToStructuredDocumentTag(sdt, 0);
            builder.InsertParagraph();
            Assert.That(sdt.GetText(), Is.EqualTo("\r"));

            builder.MoveToStructuredDocumentTag(sdtIndex, -2);
            builder.Write("456");

            builder.MoveToStructuredDocumentTag(sdt, 0);
            builder.Write("123");

            Assert.That(sdt.GetText(), Is.EqualTo("123456\r"));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            builder.InsertParagraph();
            builder.Write("0");

            builder.MoveToStructuredDocumentTag(sdtIndex, -1);
            builder.InsertParagraph();
            builder.Write("7");

            Assert.That(sdt.GetText(), Is.EqualTo("\r0123456\r7\r"));

            // Move to the end of the SDT.
            builder.MoveToStructuredDocumentTag(sdtIndex, 11);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            builder.InsertParagraph();
            builder.Write("8");

            // Move to the beginning of the SDT.
            builder.MoveToStructuredDocumentTag(sdtIndex, -14);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            builder.Write("9");

            Assert.That(sdt.GetText(), Is.EqualTo("9\r0123456\r7\r8\r"));

            sdt.RemoveAllChildren();
            builder.MoveToStructuredDocumentTag(sdt, 0);
            builder.Write("1");
            Assert.That(sdt.GetText(), Is.EqualTo("1\r"));

            builder.MoveToStructuredDocumentTag(sdt, -1);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.InsertNode(new Paragraph(doc));
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(sdt.GetText(), Is.EqualTo("1\r\r"));
        }

        /// <summary>
        /// Tests moving the cursor to a cell-level SDT.
        /// </summary>
        [Test]
        public void TestMovingToCellLevelSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const int sdtIndex = 6;
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.StructuredDocumentTags[sdtIndex];
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Cell));
            Assert.That(sdt.GetText(), Is.EqualTo("Cell 1.\a"));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.Write("At start. ");

            builder.MoveToStructuredDocumentTag(sdt, -2);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            builder.Write(" At end.");

            Assert.That(sdt.GetText(), Is.EqualTo("At start. Cell 1. At end.\a"));

            builder.MoveToStructuredDocumentTag(sdt, 15);
            builder.Write("#");

            builder.MoveToStructuredDocumentTag(sdtIndex, -10);
            builder.Write("\rSecond line.");

            Assert.That(sdt.GetText(), Is.EqualTo("At start. Cell #1.\rSecond line. At end.\a"));

            // Move cursor outside of the removing nodes.
            builder.MoveToParagraph(2, 0);
            Cell cell = (Cell)sdt.GetChild(NodeType.Cell, 0, false);
            cell.FirstParagraph.Remove();
            cell.FirstParagraph.RemoveAllChildren();
            Assert.That(sdt.GetText(), Is.EqualTo("\a"));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            builder.Write("456");

            // Move to the beginning of the SDT.
            builder.MoveToStructuredDocumentTag(sdt, -5);
            builder.Write("123");

            Assert.That(sdt.GetText(), Is.EqualTo("123456\a"));

            builder.MoveToStructuredDocumentTag(sdt, -1);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            sdt.RemoveAllChildren();
            cell = new Cell(doc);
            builder.InsertNode(cell);
            cell.EnsureMinimum();
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(sdt.GetText(), Is.EqualTo("\a"));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "Cannot insert text at this cursor position.",
            MatchType = MessageMatch.Exact)]
        public void TestWritingToCellLevelSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const int sdtIndex = 6;
            builder.MoveToStructuredDocumentTag(sdtIndex, -1);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            builder.Write("123");
        }

        /// <summary>
        /// Tests moving the cursor to a row-level SDT.
        /// </summary>
        [Test]
        public void TestMovingToRowLevelSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const int sdtIndex = 5;
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.StructuredDocumentTags[sdtIndex];
            Row row = (Row)sdt.GetChild(NodeType.Row, 0, false);
            Cell firstCell = row.FirstCell;
            Cell middleCell = row.Cells[1];
            Cell lastCell = row.LastCell;
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Row));
            Assert.That(sdt.GetText(), Is.EqualTo("Cell 1.\aCell 2.\aCell 3.\a\a"));

            builder.MoveToStructuredDocumentTag(sdtIndex, 0);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            builder.Write("At start. ");

            builder.MoveToStructuredDocumentTag(sdt, -3);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.False);
            builder.Write(" At end.");

            Assert.That(firstCell.GetText(), Is.EqualTo("At start. Cell 1.\a"));
            Assert.That(lastCell.GetText(), Is.EqualTo("Cell 3. At end.\a"));

            builder.MoveToStructuredDocumentTag(sdt, 18);
            builder.Write("Start of middle cell. ");

            builder.MoveToStructuredDocumentTag(sdtIndex, -19);
            builder.Write(" End of middle cell.");

            Assert.That(middleCell.GetText(), Is.EqualTo("Start of middle cell. Cell 2. End of middle cell.\a"));

            firstCell.FirstParagraph.RemoveAllChildren();
            middleCell.FirstParagraph.RemoveAllChildren();
            lastCell.FirstParagraph.RemoveAllChildren();
            Assert.That(sdt.GetText(), Is.EqualTo("\a\a\a\a"));

            // Move to the beginning of the SDT.
            builder.MoveToStructuredDocumentTag(sdtIndex, -5);
            builder.Write("123");

            builder.MoveToStructuredDocumentTag(sdt, 5);
            builder.Write("456");

            Assert.That(sdt.GetText(), Is.EqualTo("123\a\a456\a\a"));

            builder.MoveToStructuredDocumentTag(sdt, -1);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(sdt));
            sdt.RemoveAllChildren();
            row = new Row(doc);
            builder.InsertNode(row);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(sdt.GetText(), Is.EqualTo("\a"));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "Cannot insert text at this cursor position.",
            MatchType = MessageMatch.Exact)]
        public void TestWritingToRowLevelSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const int sdtIndex = 5;
            builder.MoveToStructuredDocumentTag(sdtIndex, -1);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            builder.Write("123");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The node must be a block or an inline.",
            MatchType = MessageMatch.Exact)]
        public void TestMovingToRow()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            const int sdtIndex = 5;
            builder.MoveToStructuredDocumentTag(sdtIndex, -2);
        }

        /// <summary>
        /// Tests that the font properties are taken from an SDT when text is inserted at the end of an empty
        /// inline-level SDT.
        /// </summary>
        [Test]
        public void TestFontAtEndOfStructuredDocumentTag()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdts.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            StructuredDocumentTag blockSdt = (StructuredDocumentTag)doc.FirstSection.Body.StructuredDocumentTags[0];
            blockSdt.ContentsFont.Size = 5;
            ((Paragraph)blockSdt.LastNonAnnotationChild).ParagraphBreakRunPr.Size = 40;
            builder.MoveToStructuredDocumentTag(blockSdt, -1);
            builder.Write("123");

            StructuredDocumentTag inlineSdt = (StructuredDocumentTag)doc.FirstSection.Body.StructuredDocumentTags[1];
            inlineSdt.ContentsFont.Size = 5;
            inlineSdt.RemoveAllChildren();
            builder.MoveToStructuredDocumentTag(inlineSdt, -1);
            builder.Write("123");

            TestUtil.SaveCheckGoldExportOnly(doc, @"DocBuilder\TestFontAtEndOfStructuredDocumentTag.docx");
        }

        /// <summary>
        /// Test a possible user case to insert a row level repeating section SDT.
        /// </summary>
        [Test]
        public void TestInsertingRepeatingSection()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.Null);

            CustomXmlPart xmlPart = doc.CustomXmlParts.Add("C8FFD3B6-7EF8-4963-895B-3565F68A0000",
                "<root><row><cell1>Cell 1</cell1><cell2>Cell 2</cell2></row></root>");

            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Header 1");
            builder.InsertCell();
            builder.Write("Header 2");
            builder.EndRow();

            builder.EndTable();

            StructuredDocumentTag repeatingSection =
                new StructuredDocumentTag(doc, SdtType.RepeatingSection, MarkupLevel.Row);
            table.AppendChild(repeatingSection);
            repeatingSection.XmlMapping.SetMapping(xmlPart, "/root/row", null);

            StructuredDocumentTag repeatingSectionItem =
                new StructuredDocumentTag(doc, SdtType.RepeatingSectionItem, MarkupLevel.Row);
            repeatingSection.AppendChild(repeatingSectionItem);
            Row row = new Row(doc);
            // repeatingSectionItem.AppendChild(row) can be used instead of the following. Just check moving cursor.
            builder.MoveToStructuredDocumentTag(repeatingSectionItem, -1);
            Assert.That(builder.IsAtEndOfStructuredDocumentTag, Is.True);
            Assert.That(builder.CurrentStructuredDocumentTag, Is.SameAs(repeatingSectionItem));
            builder.InsertNode(row);

            StructuredDocumentTag cell1Sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Cell);
            row.AppendChild(cell1Sdt);
            cell1Sdt.XmlMapping.SetMapping(xmlPart, "/root/row/cell1", null);

            StructuredDocumentTag cell2Sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Cell);
            row.AppendChild(cell2Sdt);
            cell2Sdt.XmlMapping.SetMapping(xmlPart, "/root/row/cell2", null);

            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2013);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Markup\TestInsertingRepeatingSection.docx");
        }

        /// <summary>
        /// WORDSNET-24490 DocumentBuilder.InsertField methods do not support cursor position at the end of
        /// a structured document tag.
        /// Checks the main issue of the request.
        /// </summary>
        [Test]
        public void TestInsertingFieldToSdtEnd()
        {
            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Inline);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            paragraph.AppendChild(sdt);

            sdt.IsShowingPlaceholderText = false;
            sdt.RemoveAllChildren();

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToStructuredDocumentTag(sdt, -1);
            builder.InsertField(@"CITATION Don18 \l 2055 ", "(Donin, Bueno, & Campos, 2018)");

            Assert.That(paragraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(5));
            Assert.That(sdt.FirstChild.NodeType, Is.EqualTo(NodeType.FieldStart));
            Assert.That(sdt.LastChild.NodeType, Is.EqualTo(NodeType.FieldEnd));
        }

        /// <summary>
        /// Tests SDT insertion at document end while document end is inside existing SDT.
        /// </summary>
        [Test]
        public void TestInsertAtDocumentEndInside()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentEnd();
            builder.InsertStructuredDocumentTag(SdtType.RichText);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];
            StructuredDocumentTag newSdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[1];

            // MS Word changes level of existing SDT to inline and moves to previously contained paragraph.
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Inline));

            // New SDT is inline and is placed right after existing SDT.
            Assert.That(newSdt.Level, Is.EqualTo(MarkupLevel.Inline));
            Assert.That(sdt.NextSibling, Is.EqualTo(newSdt));

            // New SDT inherited formatting.
            Assert.That(((Run)newSdt.FirstChild).CharacterStyle.Name, Is.EqualTo("Placeholder Text"));
        }


        /// <summary>
        /// Test SDT insertion at existing SDT end.
        /// </summary>
        [Test]
        public void TestInsertAtSdtEnd()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToStructuredDocumentTag((StructuredDocumentTag)doc.Range.StructuredDocumentTags[0], -1);
            builder.InsertStructuredDocumentTag(SdtType.RichText);

            // New SDT inherited formatting.
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];
            StructuredDocumentTag newSdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[1];

            // Existing SDT remains at Block level.
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Block));

            // New SDT is placed as child of existing SDT.
            Assert.That(newSdt.ParentNode, Is.EqualTo(sdt));
            Assert.That(newSdt.Level, Is.EqualTo(MarkupLevel.Block));

            // New SDT inherited formatting.
            Assert.That(((Paragraph)newSdt.FirstChild).ParagraphStyle.Name, Is.EqualTo("Caption"));
            Assert.That(((Paragraph)newSdt.FirstChild).FirstRun.CharacterStyle.Name, Is.EqualTo("Placeholder Text"));
        }

        /// <summary>
        /// Move cursor to the document end, insert paragraph break and then insert new SDT.
        /// Case to produce customer expected output.
        /// </summary>
        /// <remarks>
        /// AM. This case introduces breaking changes in DocumentBuilder.InsertParagraph().
        ///
        /// In order to create expected document we need to insert new paragraph AFTER existing SDT.
        ///
        /// That's why we detect when cursor position at the document end and document end is inside of SDT and
        /// insert new paragraph after SDT. Otherwise there is no way to create document expected by customer.
        /// </remarks>
        [Test]
        public void Test25317Customer()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentEnd();

            // This paragraph will be inserted AFTER existing SDT.
            builder.InsertParagraph();
            builder.InsertStructuredDocumentTag(SdtType.RichText);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];
            StructuredDocumentTag newSdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[1];
            Assert.That(sdt, Is.EqualTo(newSdt.PreviousSibling));    // new SDT is placed AFTER existing SDT.

            // New SDT inherited formatting.
            Assert.That(((Paragraph)newSdt.FirstChild).ParagraphStyle.Name, Is.EqualTo("Caption"));
            Assert.That(((Paragraph)newSdt.FirstChild).FirstRun.CharacterStyle.Name, Is.EqualTo("Placeholder Text"));

            // There was some XML mapping issue with output, check by gold.
            TestUtil.SaveCheckGold(doc, @"Model\Markup\Test25317Customer.docx");
        }

        /// <summary>
        /// Tests SDT insertion after new paragraph has been inserted at existing SDT end.
        /// </summary>
        [Test]
        public void Test25317B()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToStructuredDocumentTag((StructuredDocumentTag)doc.Range.StructuredDocumentTags[0], -1);
            builder.InsertParagraph();

            StructuredDocumentTag newSdt = builder.InsertStructuredDocumentTag(SdtType.RichText);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];

            Assert.That(sdt, Is.EqualTo(newSdt.ParentNode));    // new SDT is placed INSIDE existing SDT.
            Assert.That(((Paragraph)newSdt.FirstChild).ParagraphFormat.StyleName, Is.EqualTo("Caption"));

            // There was some XML mapping issue with output, check by gold.
            TestUtil.SaveCheckGold(doc, @"Model\Markup\Test25317B.docx");
        }

        /// <summary>
        /// Tests RichText SDT insertion at different positions.
        /// </summary>
        [Test]
        public void Test25317RichText()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317 Template1.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertStructuredDocumentTag(SdtType.RichText);
            builder.MoveTo(builder.CurrentParagraph, 43);
            builder.InsertStructuredDocumentTag(SdtType.RichText);

            // Both SDTs should have the same formatting and inserted at Inline level.
            foreach (IStructuredDocumentTag isdt in doc.Range.StructuredDocumentTags)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)isdt;
                Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Inline));

                Assert.That(sdt.ContentsRunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));
                Assert.That(sdt.ContentsRunPr[FontAttr.ItalicBi], Is.EqualTo(AttrBoolEx.True));

                Assert.That(((Run)sdt.FirstChild).CharacterStyle.Name, Is.EqualTo("Placeholder Text"));
            }
        }

        /// <summary>
        /// Tests RichText SDT insertion at an empty paragraph.
        /// </summary>
        [Test]
        public void TestInsertAtEmptyParagraph()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317 Template1.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToParagraph(1, -1);
            StructuredDocumentTag newSdt = builder.InsertStructuredDocumentTag(SdtType.RichText);

            // SDT should be inserted at block level and formatting should be inherited.
            Assert.That(newSdt.Level, Is.EqualTo(MarkupLevel.Block));
            Assert.That(((Paragraph)newSdt.FirstChild).ParagraphStyle.Name, Is.EqualTo("Caption"));
            Assert.That(((Paragraph)newSdt.FirstChild).FirstRun.CharacterStyle.Name, Is.EqualTo("Placeholder Text"));
        }

        /// <summary>
        /// Tests that both RepeatingSection and RepeatingSectionItem SDTs cannot be inserted.
        /// </summary>
        [TestCase(SdtType.RepeatingSection), ExpectedException(typeof(InvalidOperationException))]
        [TestCase(SdtType.RepeatingSectionItem)]
        public void Test25317RepeatingSection(SdtType type)
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317 Template1.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertStructuredDocumentTag(type);
        }

        /// <summary>
        /// Tests CheckBox SDT insertion at table cell.
        /// </summary>
        [Test]
        public void TestInsertCheckboxAtCell()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317 Template1.docx");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToCell(0, 0, 1, 0);

            StructuredDocumentTag newSdt = builder.InsertStructuredDocumentTag(SdtType.Checkbox);

            // SDT should be inserted at inline level and formatting should be inherited.
            Assert.That(newSdt.Level, Is.EqualTo(MarkupLevel.Inline));
            Assert.That(newSdt.ContentsRunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(newSdt.ContentsRunPr[FontAttr.BoldBi], Is.EqualTo(AttrBoolEx.True));

            Run sdtRun = (Run)newSdt.FirstChild;
            Assert.That(sdtRun.RunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(sdtRun.RunPr[FontAttr.BoldBi], Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Tests RichText SDT insertion at table cell.
        /// </summary>
        [Test]
        public void TestInsertRichTextAtCell()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317 Template1.docx");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToCell(0, 0, 1, 0);

            StructuredDocumentTag newSdt = builder.InsertStructuredDocumentTag(SdtType.RichText);

            // SDT should be inserted at inline level and formatting should be inherited.
            Assert.That(newSdt.Level, Is.EqualTo(MarkupLevel.Inline));
            Assert.That(newSdt.ContentsRunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(newSdt.ContentsRunPr[FontAttr.BoldBi], Is.EqualTo(AttrBoolEx.True));

            // Character formatting is not applied to placeholder text.
            Run sdtRun = (Run)newSdt.FirstChild;
            Assert.That(sdtRun.RunPr[FontAttr.Bold], Is.Null);
            Assert.That(sdtRun.RunPr[FontAttr.BoldBi], Is.Null);

            // Character style is applied from placeholder.
            Assert.That(sdtRun.CharacterStyle.Name, Is.EqualTo("Placeholder Text"));
        }

        /// <summary>
        /// Tests PlainText/RichText SDTs insertion at different positions.
        /// </summary>
        [TestCase(SdtType.PlainText)]
        [TestCase(SdtType.RichText)]
        public void TestInsertTextSdts(SdtType type)
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25317 Template2.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentStart();
            builder.InsertStructuredDocumentTag(type);
            builder.MoveToParagraph(0, -1);
            builder.InsertStructuredDocumentTag(type);
            builder.MoveToParagraph(1, 0);
            builder.InsertStructuredDocumentTag(type);
            builder.MoveToParagraph(2, 0);
            builder.InsertStructuredDocumentTag(type);

            List <StructuredDocumentTag> sdts =
                doc.GetChildNodes(NodeType.StructuredDocumentTag, true).ToList<StructuredDocumentTag>();

            Assert.That(sdts[0].Level, Is.EqualTo(MarkupLevel.Inline));
            Assert.That(sdts[1].Level, Is.EqualTo(MarkupLevel.Inline));
            Assert.That(sdts[2].Level, Is.EqualTo(MarkupLevel.Block));
            Assert.That(sdts[3].Level, Is.EqualTo(MarkupLevel.Block));
        }

        /// <summary>
        /// Tests insertion of SDT having allowed type and represented in MS Word UI.
        /// Checked all these types, behaviour is the same in Word.
        /// </summary>
        [TestCase(SdtType.RichText)]
        [TestCase(SdtType.Checkbox)]
        [TestCase(SdtType.PlainText)]
        [TestCase(SdtType.DropDownList)]
        [TestCase(SdtType.Date)]
        [TestCase(SdtType.Picture)]
        [TestCase(SdtType.ComboBox)]
        public void TestInsertAllowedTypes(SdtType sdtType)
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestInsertAllowedTypes.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentStart();
            StructuredDocumentTag sdt1 = builder.InsertStructuredDocumentTag(sdtType);

            builder.MoveToParagraph(0, -1);
            StructuredDocumentTag sdt2 = builder.InsertStructuredDocumentTag(sdtType);

            builder.MoveTo(doc.FirstSection.Body.Paragraphs[1]);
            StructuredDocumentTag sdt3 = builder.InsertStructuredDocumentTag(sdtType);

            // 1st SDT is inserted at inline level.
            Assert.That(sdt1.SdtType, Is.EqualTo(sdtType));
            Assert.That(sdt1.Level, Is.EqualTo(MarkupLevel.Inline));

            // 2nd SDT is inserted at inline level.
            Assert.That(sdt2.SdtType, Is.EqualTo(sdtType));
            Assert.That(sdt2.Level, Is.EqualTo(MarkupLevel.Inline));

            // 3rd SDT is inserted at block level because paragraph was empty.
            Assert.That(sdt3.SdtType, Is.EqualTo(sdtType));
            Assert.That(sdt3.Level, Is.EqualTo(MarkupLevel.Block));

            // All SDT is showing placeholder.
            Assert.That(sdt1.IsShowingPlaceholderText, Is.True);
            Assert.That(sdt2.IsShowingPlaceholderText, Is.True);
            Assert.That(sdt3.IsShowingPlaceholderText, Is.True);
        }

        /// <summary>
        /// WORDSNET-27645 Part of content inserted using DocumentBuilder is outside SDT.
        /// Ensure that whole multi-line text is inserted into SDT.
        /// </summary>
        [Test]
        public void Test27645()
        {
            const string text = "Lorem ipsum dolor sit amet,\r\n pretium.";

            Document doc = new Document();
            var builder = new DocumentBuilder(doc);
            StructuredDocumentTag textSdt = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.LastSection.Body.AppendChild(textSdt);
            var paraText2 = new Paragraph(doc);
            textSdt.AppendChild(paraText2);
            builder.MoveTo(paraText2);
            builder.Write(text.Replace("\n", ControlChar.Lf).Replace("\r", ControlChar.Cr));

            // Check that whole content is inserted into SDT.
            StructuredDocumentTag sdt = doc.Range.StructuredDocumentTags[0] as StructuredDocumentTag;
            Assert.That(sdt.GetText(), Is.EqualTo("Click here to enter text.\rLorem ipsum dolor sit amet,\r pretium.\f"));
        }
    }
}
