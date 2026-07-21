// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderDeleteRow
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestDeleteRow()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            CreateTwoRowTable(builder);

            Row row = builder.DeleteRow(0, 0);
            Assert.That(row, IsNot.Null());
            //This text will be appended to the end of the document because its where the cursor is now.
            builder.Writeln("Text2");

            Assert.That(doc.SelectNodes("//Row").Count, Is.EqualTo(1));
            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderDeleteRow.docx", null, false);
            Assert.That(doc.GetText(), Is.EqualTo("Cell 2.1\x0007\x0007Text1\rText2\r\x000c"));
        }

        /// <summary>
        /// Deleting a row when it contains the cursor should move the cursor to the next row.
        /// </summary>
        [Test]
        public void TestDeleteRowWithCursor()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            CreateTwoRowTable(builder);

            builder.MoveToCell(0, 0, 0, 0);
            builder.DeleteRow(0, 0);
            //The text should be inserted into the first cell of the second row as it is the
            //row after the deleted row.
            builder.Writeln("Text2");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderDeleteRowWithCursor.docx", null, false);
            Assert.That(doc.GetText(), Is.EqualTo("Text2\rCell 2.1\x0007\x0007Text1\r\x000c"));
        }

        /// <summary>
        /// Deleting a last row in a table when it contains the cursor should move the cursor to beyond the table.
        /// </summary>
        [Test]
        public void TestDeleteLastRowWithCursor()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            CreateTwoRowTable(builder);

            builder.MoveToCell(0, 1, 0, 0);
            builder.DeleteRow(0, 1);
            //The text should be inserted after the table because the cursor should have moved to beyond the table.
            builder.Writeln("Text2");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderDeleteLastRowWithCursor.docx", null, false);
            Assert.That(doc.GetText(), Is.EqualTo("Cell 1.1\x0007\x0007Text2\rText1\r\x000c"));
        }

        /// <summary>
        /// Deleting a row that has only one table should delete the whole table.
        /// </summary>
        [Test]
        public void TestDeleteSingleRow()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            CreateTwoRowTable(builder);

            builder.MoveToCell(0, 0, 0, 0);
            builder.DeleteRow(0, 0);
            builder.DeleteRow(0, 0);
            builder.Writeln("Text2");

            Assert.That(doc.SelectNodes("//Table").Count, Is.EqualTo(0));
            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderDeleteLastRowWithCursor.docx", null, false);
            Assert.That(doc.GetText(), Is.EqualTo("Text2\rText1\r\x000c"));
        }

        private static void CreateTwoRowTable(DocumentBuilder builder)
        {
            builder.InsertCell();
            builder.Write("Cell 1.1");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Cell 2.1");
            builder.EndRow();
            builder.EndTable();
            builder.Writeln("Text1");
        }
    }
}
