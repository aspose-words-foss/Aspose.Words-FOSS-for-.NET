// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using Aspose.Drawing;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    /// <summary>
    /// Test how creating tables using DocumentBuilder works.
    /// </summary>
    [TestFixture]
    public class TestDocBuilderTable
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Creates a chess like table to test borders and shading.
        /// </summary>
        [Test]
        public void TestChess()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Size = 8;
            builder.Font.Name = "Tahoma";
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            builder.RowFormat.Height = 0.75 * 72;
            builder.RowFormat.HeightRule = HeightRule.Exactly;

            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(0.75 * 72);
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            builder.CellFormat.Orientation  = TextOrientation.Downward;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    builder.InsertCell();
                    builder.Write(string.Format("X{0}, Y{1}", x, y));

                    bool isBlack = (((x + y) & 0x01) != 0);
                    builder.CellFormat.Shading.BackgroundPatternColorInternal =
                        (isBlack) ? DrColor.LightGreen : DrColor.LightYellow;
                }
                builder.EndRow();
            }

            Table table = builder.EndTable();
            table.AllowAutoFit = false;
            table.SetBorders(LineStyle.Single, 2, Color.Blue);


            builder.ParagraphFormat.ClearFormatting();
            builder.ParagraphFormat.KeepWithNext = true;
            builder.ParagraphFormat.SpaceAfter = 12;
            builder.ParagraphFormat.SpaceBefore = 11;
            builder.Font.Name = "Arial";
            builder.Font.Bold = true;
            builder.Font.Underline = Underline.Thick;
            builder.Font.Size = 12;
            builder.Writeln("blah blah");

            TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestChess.docx");
        }

        /// <summary>
        /// Some checks to the table structure and also checks
        /// that formatting we apply after the end of the table works.
        /// </summary>
        [Test]
        public void TestEndTable()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.Write("InTable");
            builder.EndRow();
            //This will be called implicitly if you don't call this, but sometimes it is better to call it.
            builder.EndTable();
            builder.ParagraphFormat.SpaceBefore = 12;
            builder.Write("OutsideTable");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestEndTable.docx", null, false);

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(2));

            Paragraph p = (Paragraph)paras[0];
            Assert.That(p.GetText(), Is.EqualTo("InTable\x0007"));        //Cell
            Assert.That(p.IsInCell, Is.EqualTo(true));
            Assert.That(p.IsEndOfCell, Is.EqualTo(true));

            p = (Paragraph)paras[1];
            Assert.That(p.GetText(), Is.EqualTo("OutsideTable\x000c"));    //After table
            Assert.That(p.IsInCell, Is.EqualTo(false));
            Assert.That(p.IsEndOfCell, Is.EqualTo(false));
            Assert.That(p.ParagraphFormat.SpaceBefore, Is.EqualTo(12.0));
        }

        /// <summary>
        /// Test a variety of document builder methods and properties by creating a simple table.
        /// </summary>
        [Test]
        public void TestTable2x2()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Paragraph before table.");

            builder.RowFormat.HeightRule = HeightRule.Exactly;
            builder.RowFormat.Height = 24;
            Cell cell = builder.InsertCell();
            Assert.That(cell, IsNot.Null());
            builder.CellFormat.Width = 1 * 72;
            builder.Writeln("Row1, Cell1");
            builder.InsertCell();
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            builder.CellFormat.Width = 1.5 * 72;
            builder.Write("Row1, Cell2");
            builder.EndRow();

            builder.RowFormat.HeightRule = HeightRule.Auto;
            builder.InsertCell();
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Top;
            builder.CellFormat.Width = 1 * 72;
            builder.Write("Row2, Cell1");
            builder.InsertCell();
            builder.CellFormat.Width = 1.5 * 72;
            builder.Font.Color = Color.Red;
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            builder.Write("Row2, Cell2");
            builder.EndRow();

            Table table = builder.EndTable();
            table.AllowAutoFit = false;
            table.PreferredWidth = PreferredWidth.Auto;
            table.LeftIndent = 2 * 72;

            builder.Font.ClearFormatting();
            builder.Writeln("Paragraph after table.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestTable2x2.docx");

            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Paragraph before table.\r" +
                "Row1, Cell1\r\x07Row1, Cell2\x07\x07Row2, Cell1\x07Row2, Cell2\x07\x07" +
                "Paragraph after table.\r\x0c"));

            Row row = (Row)doc.SelectSingleNode("//Row[1]");
            TablePr tablePr = row.TablePr;
            Assert.That(row.Cells.Count, Is.EqualTo(2));

            //Check cell widths.
            Assert.That(tablePr.LeftIndent, Is.EqualTo(2 * 1440));
            Assert.That(row.Cells[0].CellPr.Width, Is.EqualTo(1 * 1440));
            Assert.That(row.Cells[1].CellPr.Width, Is.EqualTo((int)(1.5 * 1440)));

            //Check borders are okay.
            Assert.That(row.Cells[0].CellPr.BorderTop, Is.EqualTo(null));

            row = (Row)doc.SelectSingleNode("//Row[2]");
            //Check left indent for the further rows remained okay, I saw it broken.
            Assert.That(row.TablePr.LeftIndent, Is.EqualTo(2 * 1440));
        }

        /// <summary>
        /// If you try to start a table in the middle of a paragraph, it inserts a paragraph break.
        /// </summary>
        [Test]
        public void TestMidParagraph()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("Text before table.");
            builder.InsertCell();
            builder.Write("Cell1");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Cell2");
            builder.EndRow();
            builder.Write("Text after table.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestMidParagraph.docx", null, false);

            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Text before table.\r" +
                "Cell1\x0007\x0007Cell2\x0007\x0007" +
                "Text after table.\x000c"));
        }

        [Test]
        public void TestAtDocumentStart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("Text after table.");
            builder.MoveToDocumentStart();
            builder.InsertCell();
            builder.Write("Cell1");
            builder.EndRow();
            builder.EndTable();

            builder.MoveToDocumentStart();
            builder.Write("This gets into the first cell.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestAtDocumentStart.docx", null, false);

            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("This gets into the first cell.Cell1\x0007\x0007" +
                "Text after table.\x000c"));
        }

        [Test]
        public void TestHorizontalMerge()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.CellFormat.HorizontalMerge = CellMerge.First;
            builder.Write("Text in the merged cells.");

            builder.InsertCell();
            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
            builder.EndRow();

            builder.InsertCell();
            builder.CellFormat.HorizontalMerge = CellMerge.None;
            builder.Write("Text in one cell.");

            builder.InsertCell();
            builder.Write("Text in another cell.");
            builder.EndRow();

            // There was a roundtrip via DOC here. In the FOSS version it is removed because DOCX normalizes horizontal merge out.

            Row row = (Row)doc.SelectSingleNode("//Row");

            CellPr cellPr = row.Cells[0].CellPr;
            Assert.That(cellPr.HorizontalMerge, Is.EqualTo(CellMerge.First));

            cellPr = row.Cells[1].CellPr;
            Assert.That(cellPr.HorizontalMerge, Is.EqualTo(CellMerge.Previous));
        }

        [Test]
        public void TestVerticalMerge()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.CellFormat.VerticalMerge = CellMerge.First;
            builder.Write("Text in the merged cells.");

            builder.InsertCell();
            builder.CellFormat.VerticalMerge = CellMerge.None;
            builder.Write("Text in one cell");
            builder.EndRow();

            builder.InsertCell();
            //This cell is vertically merged to the cell above and shoudl be empty.
            builder.CellFormat.VerticalMerge = CellMerge.Previous;

            builder.InsertCell();
            builder.CellFormat.VerticalMerge = CellMerge.None;
            builder.Write("Text in another cell");
            builder.EndRow();

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestVerticalMerge.docx", null, false);

            Row row = (Row)doc.SelectSingleNode("//Row[1]");
            Assert.That(row.Cells[0].CellPr.VerticalMerge, Is.EqualTo(CellMerge.First));

            row = (Row)doc.SelectSingleNode("//Row[2]");
            Assert.That(row.Cells[0].CellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
        }

        /// <summary>
        /// WORDSNET-8866 Word 2003 eats 100% of CPU and hangs during opening document produced by Aspose.Words.
        /// </summary>
        [Test]
        public void TestVerticalMergeInLastRow()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            for (int i = 0; i < 44; ++i)
                builder.Writeln("line " + i.ToString());

            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            builder.CellFormat.Borders.LineWidth = 1;
            builder.StartTable();

            builder.InsertCell();
            // RK This start of a vertical merge in the last row can cause Word 2003 to fail.
            builder.CellFormat.VerticalMerge = CellMerge.First;

            //table in table
            builder.StartTable();
            builder.InsertCell();
            builder.CellFormat.ClearFormatting();
            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            builder.CellFormat.Borders.Color = Color.Black;
            builder.CellFormat.Borders.LineWidth = 1;
            builder.Write("celA");
            builder.EndRow();

            builder.InsertCell();
            builder.Write("celB");
            builder.EndRow();

            builder.InsertCell();
            builder.Write("celC");
            builder.EndRow();

            builder.InsertCell();
            builder.Write("celD");
            builder.EndRow();
            builder.EndTable();
            //end table in table

            builder.EndRow();
            builder.EndTable();

            builder.Write("End doc");

            //new table
            builder.StartTable();
            builder.CellFormat.ClearFormatting();
            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            builder.CellFormat.Borders.LineWidth = 1;
            builder.InsertCell();
            builder.Write("celX");
            builder.InsertCell();
            builder.Write("celY");
            builder.InsertCell();
            builder.Write("celZ");
            builder.EndRow();
            builder.EndTable();

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestVerticalMergeInLastRow.docx", null, false);

            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            Assert.That(cell.CellFormat.VerticalMerge, Is.EqualTo(CellMerge.None));
        }

        /// <summary>
        /// Cannot insert section breaks etc while building a table.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestFailBreak()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.Writeln("Cell1");
            builder.InsertBreak(BreakType.SectionBreakNewPage);
        }

        /// <summary>
        /// Tests inserting a row into existing table. See comments below.
        /// </summary>
        [Test]
        public void TestAddRow()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create a 2x2 table. Imagine like this is the table you have in the template.
            builder.InsertCell();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(2.5 * 72);
            builder.Write("Cell1.1");
            builder.InsertCell();
            builder.Write("Cell1.2");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Cell2.1");
            builder.InsertCell();
            builder.Write("Cell2.2");
            builder.EndRow();
            builder.EndTable();

            // Insert a new row between the first two rows. See comments below.
            builder.MoveToCell(0, 1, 0, 0);
            builder.InsertCell();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(1 * 72);
            builder.Write("CellX.1");
            builder.InsertCell();
            builder.Write("CellX.2");
            builder.EndRow();

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestAddRow.docx", null, false);

            NodeList rows = doc.SelectNodes("//Row");
            Assert.That(rows.Count, Is.EqualTo(3));

            //Due to a "defect" in the old code this was working "correctly" inserting a new row
            //into an existing table. But I after some refactorings (after version 2.4.1) this "fixed"
            //the original defect, but broke the ability to insert rows into existing tables.
            //Now it inserts a new nested table into a cell.
            Assert.That(doc.SelectNodes("//Table").Count, Is.EqualTo(2));

            Row row = (Row)rows[0];
            Assert.That(row.GetText(), Is.EqualTo("Cell1.1\x0007Cell1.2\x0007\x0007"));

            row = (Row)rows[1];
            Assert.That(row.GetText(), Is.EqualTo("CellX.1\x0007CellX.2\x0007\x0007Cell2.1\x0007" +    //The nested table got into this cell.
                "Cell2.2\x0007\x0007"));
        }

        [Test]
        public void TestCreateNested()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table1 = builder.StartTable();
            Assert.That(table1, IsNot.Null());

            builder.RowFormat.HeightRule = HeightRule.AtLeast;
            builder.RowFormat.Height = 32;
            builder.RowFormat.Borders.LineStyle = LineStyle.Single;
            builder.RowFormat.Borders.LineWidth = 0.75;
            builder.RowFormat.Borders.Color = DrColor.Black.ToNativeColor();

            builder.CellFormat.Width = 4 * 72;
            builder.InsertCell();

                builder.StartTable();
                builder.RowFormat.Borders.LineStyle = LineStyle.Single;
                builder.RowFormat.Borders.LineWidth = 0.75;
                builder.RowFormat.Borders.Color = DrColor.Red.ToNativeColor();

                builder.CellFormat.Width = 3 * 72;
                builder.RowFormat.Height = 16;
                builder.InsertCell();
                builder.Writeln("Nested");
                builder.EndRow();
                builder.EndTable();

            Row row = builder.EndRow();
            Assert.That(row, IsNot.Null());

            Table table2 = builder.EndTable();
            Assert.That(table2, Is.EqualTo(table1));

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestCreateNested.docx", null, false);

            //Nested cells and rows end with \r while 1st level table ends with \x0007.
            Assert.That(doc.GetText(), Is.EqualTo("Nested\r\x0007\x0007\x0007\x0007\x000c"));

            NodeList rows = doc.SelectNodes("//Row");

            //Check cell and row attributes where properly applied.

            row = (Row)rows[0];
            Assert.That(row.Cells[0].CellPr.Width, Is.EqualTo(4 * 72 * 20));
            Assert.That(row.TablePr.Height, Is.EqualTo(32 * 20));

            row = (Row)rows[1];
            Assert.That(row.Cells[0].CellPr.Width, Is.EqualTo(3 * 72 * 20));
            Assert.That(row.TablePr.Height, Is.EqualTo(16 * 20));
        }

        /// <summary>
        /// Moves to a cell in existing table and changes cell and row format.
        /// </summary>
        [Test]
        public void TestChangeExisting()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create the table
            builder.RowFormat.Borders.LineStyle = LineStyle.Single;
            builder.RowFormat.Borders.LineWidth = 0.75;
            builder.RowFormat.Borders.Color = DrColor.Black.ToNativeColor();
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();
            builder.EndTable();

            builder.MoveToCell(0, 0, 0, 0);
            builder.Write("Red");
            builder.CellFormat.Shading.BackgroundPatternColor = Color.Red;
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(2 * 72);

            builder.MoveToCell(0, 0, 1, 0);
            builder.Write("Blue");
            builder.CellFormat.Shading.BackgroundPatternColor = Color.Blue;
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(3 * 72);

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestChangeExisting.docx", null, false);

            NodeList rows = doc.SelectNodes("//Row");

            // Check cell formatting was changed.
            Row row = (Row)rows[0];

            CellPr cellPr = row.Cells[0].CellPr;
            Assert.That(cellPr.Shading.BackgroundPatternColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(cellPr.PreferredWidth.ValueTwips, Is.EqualTo(2 * 1440));

            cellPr = row.Cells[1].CellPr;
            Assert.That(cellPr.Shading.BackgroundPatternColor.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
            Assert.That(cellPr.PreferredWidth.ValueTwips, Is.EqualTo(3 * 1440));

            // Check cells in this row were not changed.
            row = (Row)rows[1];
            Assert.That(row.Cells[0].CellPr.PreferredWidth, Is.EqualTo(PreferredWidth.Auto));
            Assert.That(row.Cells[1].CellPr.PreferredWidth, Is.EqualTo(PreferredWidth.Auto));
        }

        /// <summary>
        /// Paragraph alignment in cells seems to be screwed on Word 2000.
        /// </summary>
        [Test]
        public void TestAlign2000()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            builder.Write("Right");
            builder.EndRow();
            builder.EndTable();

            TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestAlign2000.docx", null, false);
        }

        /// <summary>
        /// Test that we can keep rows on the same page.
        /// </summary>
        [Test]
        public void TestKeepRows()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            for (int i = 0; i < 45; i++)
                builder.InsertParagraph();

            builder.InsertCell();
            builder.ParagraphFormat.KeepWithNext = true;
            builder.Write("Row 1");
            builder.EndRow();
            builder.InsertCell();
            builder.ParagraphFormat.KeepWithNext = false;
            builder.Write("Row 2");
            builder.EndRow();
            builder.EndTable();
            TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestKeepRows.docx", null, false);
            //There is really no check here.
        }

        [Test]
        public void TestCellPadding()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();
            builder.InsertCell();
            builder.Write("Hello");
            builder.EndRow();
            builder.EndTable();
            TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestCellPadding.docx");

            Row row = (Row)doc.SelectSingleNode("//Row");
            TablePr tablePr = row.TablePr;
            Assert.That(tablePr.LeftPadding, Is.EqualTo(108));
            Assert.That(tablePr.RightPadding, Is.EqualTo(108));
            Assert.That(tablePr.TopPadding, Is.EqualTo(0));
            Assert.That(tablePr.BottomPadding, Is.EqualTo(0));
        }

        /// <summary>
        /// After inserting text into a cell with preferred width set, all further created cells had that width.
        /// </summary>
        [Test]
        public void TestCellWidthAfterDetach()
        {
            Document doc = TestUtil.Open(@"DocBuilder\Table\TestCellWidthAfterDetach.docx");

            // Skip table width validation in the document.
            doc.DocPr.CompatibilityOptions.GrowAutofit = true;

            DocumentBuilder builder = new DocumentBuilder(doc);

            // This position is inside a table, inside a cell that has preferred width 4cm set.
            // DocumentBuilder.CellFormat becomes "bound" to the current cell.
            builder.MoveToMergeField("F1");
            builder.Write("XXX");

            // Now, when we move the cursor out of that table, the CellFormat is "unbound" from the cell,
            // but it still was containing its original formatting, including the 4cm preferred width.
            // When the code below set only CellFormtat.Width, MS Word or the table layout algorithm would calculate the width
            // according to the preferred width. The new cells would be 4cm instead of 4" and 3". This would be confusing to the user.
            //
            // The I made was in DocumentBuilder not to clone CellPr and RowPr when the cursor is moved, but create new ones.
            builder.MoveToMergeField("F2");

            builder.StartTable();
            builder.RowFormat.Borders.LineStyle = LineStyle.Double;
            builder.RowFormat.Borders.LineWidth = 0.75;
            builder.RowFormat.Borders.Color = DrColor.Black.ToNativeColor();
            builder.InsertCell();
            builder.CellFormat.Width = 4 * 72;
            builder.Write("4 inch cell");
            builder.InsertCell();
            builder.CellFormat.Width = 3 * 72;
            builder.Write("3 inch cell");
            builder.EndRow();
            builder.EndTable();

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestCellWidthAfterDetach.docx", null, false);

            Row row = (Row)doc.GetChild(NodeType.Row, 1, true);
            Assert.That(row.Cells.Count, Is.EqualTo(2));

            CellPr cellPr = row.Cells[0].CellPr;
            Assert.That(cellPr.Width, Is.EqualTo(4 * 72 * 20));
            Assert.That(cellPr.PreferredWidth.IsFixed, Is.True);
            Assert.That(cellPr.PreferredWidth.ValueTwips, Is.EqualTo(4 * 72 * 20));

            cellPr = row.Cells[1].CellPr;
            Assert.That(cellPr.Width, Is.EqualTo(3 * 72 * 20));
            Assert.That(cellPr.PreferredWidth.IsFixed, Is.True);
            Assert.That(cellPr.PreferredWidth.ValueTwips, Is.EqualTo(3 * 72 * 20));
        }

        /// <summary>
        /// This method was introduced just because one of the customers really needed
        /// to produce tables with zero margins.
        /// </summary>
        [Test]
        public void TestClearCellMargins()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.Write("Cell1");
            builder.InsertCell();
            builder.Write("Cell2");
            builder.EndRow();

            Table table = builder.EndTable();
            table.LeftPadding = 0;
            table.RightPadding = 0;
            table.TopPadding = 0;
            table.BottomPadding = 0;

            TestUtil.Save(doc, @"DocBuilder\Table\TestCellMargins.docx", null, false);

            Row row = (Row)doc.SelectSingleNode("//Row");
            TablePr tablePr = row.TablePr;
            Assert.That(tablePr.LeftPadding, Is.EqualTo(0));
            Assert.That(tablePr.RightPadding, Is.EqualTo(0));
            Assert.That(tablePr.TopPadding, Is.EqualTo(0));
            Assert.That(tablePr.BottomPadding, Is.EqualTo(0));
        }

        /// <summary>
        /// Testing to use default borders.
        /// </summary>
        [Test]
        public void TestDefaultBorders()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            //Set all borders to 3 pt
            builder.RowFormat.Borders.LineStyle = LineStyle.Single;
            builder.RowFormat.Borders.LineWidth = 3;

            //Set intra cell borders to 1pt
            builder.RowFormat.Borders[BorderType.Horizontal].LineWidth = 1;
            builder.RowFormat.Borders[BorderType.Vertical].LineWidth = 1;

            builder.InsertCell();

            builder.Write("A");
            builder.InsertCell();
            builder.Write("B");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("C");
            builder.InsertCell();
            builder.Write("D");
            builder.EndRow();
            builder.EndTable();

            TestUtil.Save(doc, @"DocBuilder\Table\TestDefaultBorders.docx", null, false);

            //Check top left cell
            Row row = (Row)doc.SelectSingleNode("//Row");
            Cell cell = (Cell)row.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(cell.CellFormat.Borders[BorderType.Top].LineWidth, Is.EqualTo(3.0));
            Assert.That(cell.CellFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(3.0));
            Assert.That(cell.CellFormat.Borders[BorderType.Bottom].LineWidth, Is.EqualTo(1.0));
            Assert.That(cell.CellFormat.Borders[BorderType.Right].LineWidth, Is.EqualTo(1.0));
        }

        /// <summary>
        /// WORDSNET-905 Generated document hangs up in Word 2000.
        /// The problem was caused by first cell in a row being merged to previous.
        /// </summary>
        [Test]
        public void TestDefect905()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.CellFormat.HorizontalMerge = CellMerge.Previous;
            builder.Write("A");
            builder.InsertCell();
            builder.Write("B");
            builder.EndRow();
            builder.EndTable();

            // The first cell is created as merged to previous by the user.
            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            Assert.That(cell.CellFormat.HorizontalMerge, Is.EqualTo(CellMerge.Previous));

            // Roundtrip in the FOSS version is removed because DOCX normalizes horizontal merge out.
        }

        [Test]
        public void TestDefaultCellWidth()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Cell cell = builder.InsertCell();
            Assert.That(cell.CellFormat.Width, Is.EqualTo(0));
            Assert.That(cell.CellFormat.PreferredWidth, Is.EqualTo(PreferredWidth.Auto));
        }

        [Test]
        public void TestMoveToCellStart()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToCell.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToCell(0, 1, 1, 0);
            builder.Write("Inserted text.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestMoveToCellStart.docx", null, false);
            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("\x0007\x0007\x0007" +
                "Existing text.\x0007Inserted text.\x0007\x0007\x000c"));
        }

        [Test]
        public void TestMoveToCellEnd()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToCell.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToCell(0, 0, 0, -1);
            builder.Write("Inserted text 1.");
            builder.MoveToCell(0, 1, 0, -1);
            builder.Write(" Inserted text 2.");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestDocBuilderMoveToCellEnd.docx", null, false);
            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Inserted text 1.\x0007\x0007\x0007" +
                "Existing text. Inserted text 2.\x0007\x0007\x0007\x000c"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestMoveToCellBeyondDoc()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestMoveToCell.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToCell(0, 0, 2, 0);
        }

        [Test]
        public void TestCreateTableAndMoveToCell()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.Write("Cell 1");
            builder.InsertCell();
            builder.EndRow();

            builder.Write("Between tables");

            builder.InsertCell();
            builder.Write("Cell 3");
            builder.EndRow();
            builder.EndTable();

            builder.MoveToCell(0, 0, 1, 0);
            builder.Write("Cell 2");

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\TestCreateTableAndMoveToCell.docx", null, false);
            Assert.That(doc.Sections[0].Body.GetText(), Is.EqualTo("Cell 1\x0007Cell 2\x0007\x0007Between tables\rCell 3\x0007\x0007\x000c"));
        }

        [Test]
        public void TestDefect26962()
        {
            // Create empty document and DocumentBuilder object.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Configure DocumentBuilder
            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            builder.CellFormat.Borders.Color = Color.Black;

            // Build rows with merged cells in various combination
            for (int i = 0; i < gMerges.Length/6; i++)
                BuildTestMergedRow(builder, 9, 40, gMerges[i, 0], gMerges[i, 1], gMerges[i, 2], gMerges[i, 3], gMerges[i, 4], gMerges[i, 5]);
            builder.EndTable();

            // Roundtrip via DOC was here and it worked before FOSS.
            // In the FOSS version roundtrip via DOCX normalizes the horizontal merges and therefore
            // produces a different result. Therefore rountrip is removed.

            // Verify all read merged cell ranges.
            Table table = (Table)doc.GetChildNodes(NodeType.Table, true)[0];
            for (int i = 0; i < gMerges.Length/6; i++)
                VerifyTestMergedRow(table.Rows[i], gMerges[i, 0], gMerges[i, 1], gMerges[i, 2], gMerges[i, 3], gMerges[i, 4], gMerges[i, 5]);
        }

        /// <summary>
        /// Utility procedure, builds row with merged cells. Used in TestDefect26962.
        /// AM. Don't like to implement classes and collection for cell ranges so use simple argument set to define merged cell ranges.
        /// (first1-last1), (first2-last2), (first3-last3) argument define merged ranges.
        /// Think three ranges are enough to test various combinations.
        /// </summary>
        private static void BuildTestMergedRow(DocumentBuilder builder, int count, int width, int first1, int last1, int first2, int last2, int first3, int last3)
        {
            for (int i = 0; i < count; i++)
            {
                builder.InsertCell();
                builder.CellFormat.Width = width;

                if (i == first1 || i == first2 || i == first3)
                {
                    builder.CellFormat.HorizontalMerge = CellMerge.First;
                    builder.Write(string.Format("Merged cells ({0}:{1})", i, (i == first1) ? last1 : ((i == first2 ? last2 : last3))));
                }
                else if (
                    (first1 < i && i <= last1) ||
                    (first2 < i && i <= last2) ||
                    (first3 < i && i <= last3))
                {
                    builder.CellFormat.HorizontalMerge = CellMerge.Previous;
                }
                else
                {
                    builder.CellFormat.HorizontalMerge = CellMerge.None;
                    builder.Write(i.ToString());
                }
            }

            builder.EndRow();
        }

        /// <summary>
        /// Utility procedure, verifies row with merged cells. Used in TestDefect26962.
        /// AM. Argument meaning is almost the same as for BuildTestMergedRow().
        /// </summary>
        private static void VerifyTestMergedRow(Row row, int first1, int last1, int first2, int last2, int first3, int last3)
        {
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (i == first1 || i == first2 || i == first3)
                    Assert.That(row.Cells[i].CellFormat.HorizontalMerge, Is.EqualTo(CellMerge.First));
                else if (
                    (first1 < i && i <= last1) ||
                    (first2 < i && i <= last2) ||
                    (first3 < i && i <= last3))
                    Assert.That(row.Cells[i].CellFormat.HorizontalMerge, Is.EqualTo(CellMerge.Previous));
                else
                    Assert.That(row.Cells[i].CellFormat.HorizontalMerge, Is.EqualTo(CellMerge.None));
            }
        }

        /// <summary>
        /// Defines merged cells combination for TestDefect26962.
        /// </summary>
        private static readonly int[,] gMerges = new int[,]
        {
            { -1, -1, -1, -1, -1, -1 },
            {  0,  1, -1, -1, -1, -1 },
            {  0,  7, -1, -1, -1, -1 },
            {  0,  8, -1, -1, -1, -1 },
            {  0,  2,  4,  6, -1, -1 },
            {  1,  2,  4,  5, -1, -1 },
            {  2,  4,  6,  7, -1, -1 },
            {  0,  1,  3,  4,  6,  7 },
            {  4,  7, -1, -1, -1, -1 },
            {  0,  1,  2,  3,  4,  5 },
            {  4,  8, -1, -1, -1, -1 }
        };


        /// <summary>
        /// Checks that ClearFormatting does not clear Width and PreferredWidth and ClearAllFormatting clears all.
        /// </summary>
        [Test]
        public void TestCellFormatClearFormattig()
        {
            DocumentBuilder builder = new DocumentBuilder();

            Assert.That(builder.GetCellPrCopy().Count, Is.EqualTo(0));

            // Set 3 attributes.
            builder.CellFormat.LeftPadding = 3;
            builder.CellFormat.Width = 100;
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPercent(100);

            Assert.That(builder.GetCellPrCopy().Count, Is.EqualTo(3));

            builder.CellFormat.ClearFormatting();
            // Width and PreferredWidth are not cleared.
            Assert.That(builder.GetCellPrCopy().Count, Is.EqualTo(2));
            Assert.That(builder.CellFormat.Width, Is.EqualTo(100));
            Assert.That(builder.CellFormat.PreferredWidth, Is.EqualTo(PreferredWidth.FromPercent(100)));

            builder.CellFormat.ClearAllFormatting();
            Assert.That(builder.GetCellPrCopy().Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Checks that ClearFormatting actually reverts to the default attributes.
        /// </summary>
        [Test]
        public void TestRowFormatClearFormatting()
        {
            DocumentBuilder builder = new DocumentBuilder();

            // RowFormat properties are set by default.
            TablePr defaultTablePr = TablePr.CreateMSWordLooking();
            Assert.That(builder.GetTablePrCopy().Count, Is.EqualTo(defaultTablePr.Count));
            RowFormat  builderRowFormat = builder.RowFormat;

            Assert.That(GetAttrTwips(builder, TableAttr.LeftPadding), Is.EqualTo(defaultTablePr.LeftPadding));
            Assert.That(GetAttrTwips(builder, TableAttr.LeftPadding) > 0d, Is.True);
            Assert.That(GetAttrTwips(builder, TableAttr.RightPadding), Is.EqualTo(defaultTablePr.RightPadding));
            Assert.That(GetAttrTwips(builder, TableAttr.RightPadding) > 0d, Is.True);
            Assert.That(GetAttrTwips(builder, TableAttr.LeftIndent), Is.EqualTo(0d));

            // Reset row formatting to the default.
            builderRowFormat.ClearFormatting();
            Assert.That(GetAttrTwips(builder, TableAttr.LeftPadding), Is.EqualTo(defaultTablePr.LeftPadding));
            Assert.That(builder.GetTablePrCopy().Count, Is.EqualTo(defaultTablePr.Count));
            Assert.That(GetAttrTwips(builder, TableAttr.LeftIndent), Is.EqualTo(0d));

            // Clear all attributes
            ((IRowAttrSource)builder).ClearRowAttrs();
            Assert.That(builder.GetTablePrCopy().Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-9141 RowFormat.HeadingFormat does not repeat table header row on every page.
        /// DocumentBuilder copies first row properties to every row when row is started.
        /// Customer sets RowFormat.HeadingFormat to false right after row is ended but before row is started.
        /// </summary>
        [Test]
        public void TestJira9141()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.RowFormat.HeadingFormat = true;
            builder.InsertCell();
            builder.Writeln("Heading row");
            builder.EndRow();

            builder.RowFormat.HeadingFormat = false;

            for (int i = 0; i < 50; i++)
            {
                builder.InsertCell();
                builder.Write("Column Text");
                builder.EndRow();
            }

            Assert.That(table.Rows.Count, Is.EqualTo(51));
            Assert.That(table.Rows[0].TablePr[TableAttr.HeadingFormat], Is.EqualTo(true));

            for (int i = 1; i < 51; i++)
                Assert.That(table.Rows[i].TablePr[TableAttr.HeadingFormat], Is.EqualTo(false));
        }

        /// <summary>
        /// WORDSNET-10543 Document.Save method throws System.NullReferenceException.
        /// The table was started, but cells and rows was not added.
        /// So, Table.FirstRow threw an exception in DocumentValidator.IsNextSiblingAjancentTable().
        /// </summary>
        [Test]
        public void TestJira10543()
        {
            Document doc = new Document();
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            // This a customer's scenario: two adjacent tables and one of them hasn't any rows and cells.

            // First empty table
            docBuilder.StartTable();
            docBuilder.EndTable();
            // Next adjacent table
            docBuilder.StartTable();
            docBuilder.InsertCell();
            docBuilder.Write("abc");
            docBuilder.EndTable();

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Table\TestJira10543", UnifiedScenario.Docx2Docx| UnifiedScenario.NoGold);

            // Checks that tables were successfully saved. The empty one treated as non-adjacent and had to be removed upon saving.
            // So checks, that there is only one table and it has only one row.
            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);
            Assert.That(tables.Count, Is.EqualTo(1));
            Assert.That(((Table)tables[0]).Rows.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-13480 Formatting in CellFormat.Borders changes after read-only access.
        /// Incorrect setting of border's parent caused a problem when working with DocumentBuilder.
        /// </summary>
        [Test]
        public void TestJira13480()
        {
            DocumentBuilder builder = new DocumentBuilder();

            // We are not inside any cell yet.
            CheckLineStyle(builder.CellFormat, LineStyle.None);

            Cell cell1 = builder.InsertCell();

            // We are inside a cell now.
            CheckLineStyle(builder.CellFormat, LineStyle.Single);
            CheckLineStyle(cell1.CellFormat, LineStyle.Single);

            // Go out from table.
            builder.MoveToDocumentEnd();

            // We are not in any cell again.
            CheckLineStyle(builder.CellFormat, LineStyle.None);
            // Cell's LineStyle must not be changed.
            CheckLineStyle(cell1.CellFormat, LineStyle.Single);

            // Change global LineStyle to something but not default.
            builder.CellFormat.Borders.LineStyle = LineStyle.Dot;

            // Go to inside cell again.
            builder.MoveTo(cell1.FirstParagraph);

            // We are inside existing cell and we changed builder's LineStyle outside of this cell.
            // So, LineStyle of this cell must stay unchanged.
            CheckLineStyle(builder.CellFormat, LineStyle.Single);
            CheckLineStyle(cell1.CellFormat, LineStyle.Single);

            // Inserting new cell when we are inside another cell will rewrite global LineStyle.
            Cell cell2 = builder.InsertCell();
            CheckLineStyle(cell2.CellFormat, LineStyle.Single);

            // Change builder's LineStyle and it affects current cell, but not previously created cell.
            builder.CellFormat.Borders.LineStyle = LineStyle.DotDash;
            CheckLineStyle(builder.CellFormat, LineStyle.DotDash);
            CheckLineStyle(cell2.CellFormat, LineStyle.DotDash);

            // First cell must stay unchanged.
            CheckLineStyle(cell1.CellFormat, LineStyle.Single);

            // Go out from table.
            builder.MoveToDocumentEnd();
            // Builder's LineStyle now is global and it was rewritten when new cell was inserted.
            CheckLineStyle(builder.CellFormat, LineStyle.Single);

            builder.CellFormat.Borders.LineStyle = LineStyle.DoubleWave;
            // LineStyle of existing cells must not be changed.
            CheckLineStyle(cell1.CellFormat, LineStyle.Single);
            CheckLineStyle(cell2.CellFormat, LineStyle.DotDash);

            // This will create new table. Builder's global LineStyle should affect this table.
            Cell cell3 = builder.InsertCell();
            CheckLineStyle(cell3.CellFormat, LineStyle.DoubleWave);
        }

        /// <summary>
        /// WORDSNET-15014 Aspose.Words Objects are very verbose.
        /// SetPaddings method was added.
        /// </summary>
        [Test]
        public void TestJira15014()
        {
            DocumentBuilder builder = new DocumentBuilder();

            // Set cell paddings.
            builder.CellFormat.SetPaddings(30, 40, 50, 60);

            // Check paddings are set.
            Assert.That(builder.CellFormat.LeftPadding, Is.EqualTo(30));
            Assert.That(builder.CellFormat.TopPadding, Is.EqualTo(40));
            Assert.That(builder.CellFormat.RightPadding, Is.EqualTo(50));
            Assert.That(builder.CellFormat.BottomPadding, Is.EqualTo(60));
        }

        /// <summary>
        /// WORDSNET-16216 Issue with vertical merging in table while saving as DOCX when AllowAutoFit is false.
        /// Fix vertical merging of cells if table has AllowAutoFit = false and
        /// before cell with VerticalMerge exist cell with horizontal merge.
        /// </summary>
        [Test]
        public void TestJira16216()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Table tbl = builder.StartTable();
            InsertCell(builder, "row 1 cell 1", CellMerge.First, CellMerge.None);
            InsertCell(builder, "row 1 cell 2", CellMerge.None, CellMerge.None);
            InsertCell(builder, "row 1 cell 3", CellMerge.First, CellMerge.None);
            InsertCell(builder, "row 1 cell 4", CellMerge.None, CellMerge.None);
            builder.EndRow();

            InsertCell(builder, "", CellMerge.Previous, CellMerge.None);
            InsertCell(builder, "row 2 cell 2", CellMerge.None, CellMerge.None);
            InsertCell(builder, "", CellMerge.Previous, CellMerge.None);
            InsertCell(builder, "row 2 cell 4", CellMerge.None, CellMerge.None);
            builder.EndRow();

            InsertCell(builder, "row 3 cell 1", CellMerge.None, CellMerge.First);
            InsertCell(builder, "", CellMerge.None, CellMerge.Previous);
            InsertCell(builder, "", CellMerge.Previous, CellMerge.None);
            InsertCell(builder, "row 3 cell 4", CellMerge.None, CellMerge.None);
            builder.EndRow();

            tbl.SetBorders(LineStyle.Single, 1, Color.Black);
            tbl.PreferredWidth = PreferredWidth.FromPercent(100);
            tbl.AllowAutoFit = false;
            builder.EndTable();

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            // Cells from first row must be merged with next rows
            tbl = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(tbl.Rows[0].Cells[2].CellPr.VerticalMerge, Is.EqualTo(CellMerge.First));
            Assert.That(tbl.Rows[1].FirstCell.CellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
            Assert.That(tbl.Rows[2].Cells.Count, Is.EqualTo(3));
            Assert.That(tbl.Rows[2].Cells[1].CellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
        }

        /// <summary>
        /// WORDSNET-19247 Merged cells in table are broken when AllowAutoFit is false.
        /// Decision that cells may be merged is made when cells span the same number of grid columns.
        /// However currently AW compares spans which produces by different algorithms and may have different
        /// results for the same cell. Use similar sources of grid spans to fix the issue.
        /// </summary>
        [Test]
        public void Test19247()
        {
            DocumentBuilder builder = new DocumentBuilder();
            PreferredWidth width50 = PreferredWidth.FromPoints(ConvertUtil.MillimeterToPoint(50));
            PreferredWidth width10 = PreferredWidth.FromPoints(ConvertUtil.MillimeterToPoint(10));

            Table tbl = builder.StartTable();
            InsertCell(builder, "Merged col", CellMerge.First, CellMerge.None, width50);
            InsertCell(builder, "x", CellMerge.None, CellMerge.None, width10);
            InsertCell(builder, "Data 1", CellMerge.None, CellMerge.None, width50);
            builder.EndRow();

            InsertCell(builder, "", CellMerge.Previous, CellMerge.None, width50);
            InsertCell(builder, "x", CellMerge.None, CellMerge.None, width10);
            InsertCell(builder, "Data 2", CellMerge.None, CellMerge.None, width50);
            builder.EndRow();

            InsertCell(builder, "", CellMerge.Previous, CellMerge.None, width50);
            InsertCell(builder, "x", CellMerge.None, CellMerge.None, width10);
            InsertCell(builder, "Data 3", CellMerge.None, CellMerge.None, width50);
            builder.EndRow();

            InsertCell(builder, "", CellMerge.Previous, CellMerge.None, width50);
            InsertCell(builder, "x", CellMerge.None, CellMerge.None, width10);
            InsertCell(builder, "Data 4", CellMerge.None, CellMerge.None, width50);
            builder.EndRow();

            tbl.AllowAutoFit = false;
            builder.EndTable();

            TestUtil.ExecuteValidator(builder.Document, SaveFormat.Docx);

            tbl = (Table) builder.Document.GetChild(NodeType.Table, 0, true);
            Assert.That(tbl.Rows[0].FirstCell.CellPr.VerticalMerge, Is.EqualTo(CellMerge.First));
            Assert.That(tbl.Rows[1].FirstCell.CellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
            Assert.That(tbl.Rows[2].FirstCell.CellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
            Assert.That(tbl.Rows[3].FirstCell.CellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
            Assert.That(tbl.Rows.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// WORDSNET-19483 DocumentBuilder.EndTable sets wrong font size after PushFont/PopFont.
        /// ParagraphBreakRunPr doesn't restore state after popping the font.
        /// PushFont/PopFont should consider ParagraphBreakRunPr.
        /// </summary>
        [Test]
        public void Test19483()
        {
            DocumentBuilder builder = new DocumentBuilder();
            CheckFont(12, 20, false, builder);

            builder.PushFont();

            builder.Font.Size = 14;
            builder.Font.Bold = true;

            CheckFont(14, 28, true, builder);
            builder.Writeln("Bold 14");

            builder.PopFont();
            CheckFont(12, 20, false, builder);

            builder.StartTable();
            builder.InsertCell();
            CheckFont(12, 20, false, builder);
            builder.Writeln("No Bold 12");
            builder.EndTable();

            CheckFont(12, 20, false, builder);
        }

        /// <summary>
        /// WORDSNET-21602 Table row is lost when HTML and Table is inserted into document using DocumentBuilder.
        /// CellFormat cell merge is now changes to default value on the TableEnd.
        /// </summary>
        [Test]
        public void Test21602()
        {
            Document doc = new Document();
            DocumentBuilder documentBuilder = new DocumentBuilder(doc);

            // AddVerticallyMergedTable
            documentBuilder.Writeln("Vertically merged table");

            Table tmpTable = documentBuilder.StartTable();

            Cell tmpCell = documentBuilder.InsertCell();
            documentBuilder.Write("Title");
            tmpCell.CellFormat.VerticalMerge = CellMerge.First;
            documentBuilder.EndRow();

            tmpCell = documentBuilder.InsertCell();
            tmpCell.CellFormat.VerticalMerge = CellMerge.Previous;
            documentBuilder.EndRow();

            documentBuilder.EndTable();

            // AddSimpleTable
            documentBuilder.Writeln("Simple table");

            tmpTable = documentBuilder.StartTable();

            documentBuilder.InsertCell();
            documentBuilder.Write("Title");
            documentBuilder.EndRow();

            documentBuilder.InsertCell();
            documentBuilder.Write("Text");
            documentBuilder.EndRow();

            documentBuilder.EndTable();

            Cell cell = doc.FirstSection.Body.Tables[1].FirstRow.FirstCell;
            Assert.That(cell.CellFormat.VerticalMerge, Is.EqualTo(CellMerge.None));
        }

        /// <summary>
        /// WORDSNET-23389 Issue when changing Font immediately after DocumentBuilder.StartTable.
        /// Tests the new flatFormatting option to support sequential table formatting.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public void Test23389(bool сontextTableFormatting)
        {
            Document doc = new Document();
            DocumentBuilderOptions opt = new DocumentBuilderOptions();
            opt.ContextTableFormatting = сontextTableFormatting;
            DocumentBuilder builder = new DocumentBuilder(doc, opt);
            builder.Writeln("12 size");

            // Special case for ClearFormatting() inside the table.
            Table table1 = builder.StartTable();
            builder.Font.Size = 5;
            builder.InsertCell();
            builder.Write("5 size");
            builder.InsertCell();
            builder.Font.ClearFormatting();
            builder.EndRow();
            builder.EndTable();
            if (сontextTableFormatting)
                builder.Writeln("5 size");
            else
                builder.Writeln("12 size");

            // Common case for ContextTableFormatting option.
            Table table2 = builder.StartTable();
            builder.Font.Size = 5;
            builder.InsertCell();
            builder.Write("5 size");
            builder.InsertCell();
            builder.Font.Size = 25;
            builder.Write("25 size");
            builder.EndRow();
            builder.EndTable();
            if (сontextTableFormatting)
                builder.Writeln("5 size");
            else
                builder.Writeln("25 size");

            Run run1 = doc.FirstSection.Body.Paragraphs[1].FirstRun;
            Run table1Run1 = table1.FirstRow.FirstCell.FirstParagraph.FirstRun;
            Run table2Run1 = table2.FirstRow.FirstCell.FirstParagraph.FirstRun;
            Run table2Run2 = table2.FirstRow.LastCell.FirstParagraph.FirstRun;
            Run run2 = doc.FirstSection.Body.Paragraphs[2].FirstRun;

            double run1FontSizeExp = сontextTableFormatting
                ? 5
                : 12;
            double run2FontSizeExp = сontextTableFormatting
                ? 5
                : 25;
            Assert.That(run1.Font.Size, Is.EqualTo(run1FontSizeExp));
            Assert.That(table1Run1.Font.Size, Is.EqualTo(5));
            Assert.That(table2Run1.Font.Size, Is.EqualTo(5));
            Assert.That(table2Run2.Font.Size, Is.EqualTo(25));
            Assert.That(run2.Font.Size, Is.EqualTo(run2FontSizeExp));
        }

        /// <summary>
        /// WORDSNET-15676 Different Font color after builder.EndTable.
        /// WORDSNET-18449 DocumentBuilder.EndTable sets DocumentBuilder.Font.Bold to true.
        /// WORDSNET-18451 DocumentBuilder.EndTable sets DocumentBuilder.Font properties unexpectedly.
        /// Relates to WORDSNET-23389
        /// </summary>
        [Test]
        public void Test18449()
        {
            DocumentBuilderOptions opt = new DocumentBuilderOptions();
            opt.ContextTableFormatting = false;
            DocumentBuilder builder = new DocumentBuilder(opt);

            builder.StartTable();
            builder.Font.Bold = true;
            builder.Font.Color = Color.Blue;
            builder.InsertCell();
            builder.Write("Row 1, Cell 1 Content.");
            builder.InsertCell();
            builder.Write("Row 1, Cell 2 Content.");
            builder.EndRow();
            builder.Font.Color = Color.Red;
            builder.Font.Bold = false;

            Assert.That(builder.Font.Bold, Is.False);
            Assert.That(builder.Font.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            builder.EndTable();
            Assert.That(builder.Font.Bold, Is.False);
            Assert.That(builder.Font.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            builder.Write("This should not be bold and blue.");
            Font lastFont = builder.Document.FirstSection.Body.LastParagraph.GetLastRun().Font;
            Assert.That(lastFont.Bold, Is.False);
            Assert.That(lastFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
        }

        /// <summary>
        /// Insert new cell to table and set appropriate vertical/horizontal merge to it.
        /// </summary>
        private static void InsertCell(DocumentBuilder builder, string text, CellMerge vertical,
            CellMerge horizontal, PreferredWidth preferredWidth = null)
        {
            builder.InsertCell();
            builder.Write(text);
            builder.CellFormat.VerticalMerge = vertical;
            builder.CellFormat.HorizontalMerge = horizontal;
            builder.CellFormat.PreferredWidth = (preferredWidth == null) ? PreferredWidth.FromPercent(25) : preferredWidth;
        }

        /// <summary>
        /// Checks <see cref="LineStyle"/> of right border in the specified <paramref name="cellFormat"/>.
        /// </summary>
        private static void CheckLineStyle(CellFormat cellFormat, LineStyle expectedLineStyle)
        {
            Assert.That(cellFormat.Borders.Right.LineStyle, Is.EqualTo(expectedLineStyle));
        }

        /// <summary>
        /// Sets border line style and text orientation for row and cell via DocumentBuilder or CurrnetParagraph.
        /// </summary>
        private static void SetTableFormatting(LineStyle lineStyle, TextOrientation orientation, DocumentBuilder builder, bool setViaDocumentBuilder)
        {
            if (setViaDocumentBuilder || builder.CurrentParagraph.IsInCell)
            {
                RowFormat rowFormat = (setViaDocumentBuilder) ? builder.RowFormat : builder.CurrentParagraph.ParentRow.RowFormat;
                CellFormat cellFormat = (setViaDocumentBuilder) ? builder.CellFormat : builder.CurrentParagraph.ParentCell.CellFormat;

                rowFormat.Borders.Left.LineStyle = lineStyle;
                cellFormat.Orientation = orientation;
            }
        }

        /// <summary>
        /// Checks border line style and text orientation for row and cell in DocumentBuilder and CurrnetParagraph.
        /// </summary>
        private static void CheckTableFormatting(LineStyle lineStyle, TextOrientation orientation, DocumentBuilder builder)
        {
            CheckTableFormattingInDocumentBuilder(lineStyle, orientation, builder);
            CheckTableFormattingInCurrentParagraph(lineStyle, orientation, builder);
        }

        /// <summary>
        /// Checks border line style and text orientation for row and cell in CurrnetParagraph.
        /// </summary>
        private static void CheckTableFormattingInCurrentParagraph (LineStyle lineStyle, TextOrientation orientation, DocumentBuilder builder)
        {
            Assert.That(builder.CurrentParagraph.IsInCell, Is.True);
            Assert.That(builder.CurrentParagraph.ParentRow.RowFormat.Borders.Left.LineStyle, Is.EqualTo(lineStyle));
            Assert.That(builder.CurrentParagraph.ParentCell.CellFormat.Orientation, Is.EqualTo(orientation));
        }

        /// <summary>
        /// Checks border line style and text orientation for row and cell in DocumentBuilder.
        /// </summary>
        private static void CheckTableFormattingInDocumentBuilder(LineStyle lineStyle, TextOrientation orientation, DocumentBuilder builder)
        {
            Assert.That(builder.RowFormat.Borders.Left.LineStyle, Is.EqualTo(lineStyle));
            Assert.That(builder.CellFormat.Orientation, Is.EqualTo(orientation));
        }

        private static int GetAttrTwips(DocumentBuilder builder, int attrKey)
        {
            TablePr tablePr = builder.GetTablePrCopy();
            object value = tablePr[attrKey];
            return (value != null)
                ? (int)value
                : 0;
        }

        /// <summary>
        /// Helper method, relates to WORDSNET-9040
        /// </summary>
        private static void WriteFaultTableTOC(DocumentBuilder docBuilder, Document doc)
        {
            docBuilder.StartTable();
            docBuilder.ParagraphFormat.ClearFormatting();

            Style style = doc.Styles["Normal"]; //$NON-NLS-1$
            if (style != null)
            {
                docBuilder.ParagraphFormat.Style = style;
            }

            docBuilder.Font.ClearFormatting();

            docBuilder.RowFormat.Height = 0;

            Cell cell1 = docBuilder.InsertCell();
            docBuilder.ParagraphFormat.ClearFormatting();

            docBuilder.CellFormat.VerticalMerge = CellMerge.None;
            docBuilder.CellFormat.HorizontalMerge = CellMerge.First;

            Cell rowOneCell = null;
            for (int j = 1; j < 11; ++j)
            {
                rowOneCell = docBuilder.InsertCell();

                docBuilder.CellFormat.VerticalMerge = CellMerge.None;
                docBuilder.CellFormat.HorizontalMerge = CellMerge.Previous;
            }

            docBuilder.MoveTo(cell1.FirstParagraph);

            docBuilder.CellFormat.Borders.LineStyle = LineStyle.Single;
            docBuilder.CellFormat.Borders.LineWidth = 1;

            docBuilder.Write("Test string in first row.");

            docBuilder.EndRow();

            docBuilder.InsertCell();
            docBuilder.ParagraphFormat.ClearFormatting();

            Cell cell3 = docBuilder.InsertCell();
            docBuilder.ParagraphFormat.ClearFormatting();

            docBuilder.CellFormat.VerticalMerge = CellMerge.None;
            docBuilder.CellFormat.HorizontalMerge = CellMerge.First;

            Cell rowTwoCell = null;
            for (int j = 1; j < 10; ++j)
            {
                rowTwoCell = docBuilder.InsertCell();

                docBuilder.CellFormat.VerticalMerge = CellMerge.None;
                docBuilder.CellFormat.HorizontalMerge = CellMerge.Previous;
            }

            docBuilder.MoveTo(cell3.FirstParagraph);

            docBuilder.CellFormat.Borders.LineStyle = LineStyle.Single;
            docBuilder.CellFormat.Borders.LineWidth = 1;

            docBuilder.Write("Test string in second row.");

            docBuilder.EndRow();

            Table table = docBuilder.EndTable();
            table.PreferredWidth = PreferredWidth.Auto;

            rowOneCell.CellFormat.Width = 497.25;
            rowOneCell.CellFormat.PreferredWidth = PreferredWidth.FromPoints(497.25);

            rowTwoCell.CellFormat.Width = 497.25;
            rowTwoCell.CellFormat.PreferredWidth = PreferredWidth.FromPoints(497.25);

            table.AllowAutoFit = true;
        }

        /// <summary>
        /// Helper method, relates to WORDSNET-19483
        /// </summary>
        private static void CheckFont(int fontSize, int paraBreakFontSize, bool bold, DocumentBuilder builder)
        {
            Assert.That(builder.Font.Size, Is.EqualTo(fontSize));
            Assert.That(builder.Font.Bold, Is.EqualTo(bold));
            Assert.That(builder.CurrentParagraph.ParagraphBreakRunPr.Size, Is.EqualTo(paraBreakFontSize));
            Assert.That(builder.CurrentParagraph.ParagraphBreakRunPr.Bold.ToBool(), Is.EqualTo(bold));
        }
    }
}
