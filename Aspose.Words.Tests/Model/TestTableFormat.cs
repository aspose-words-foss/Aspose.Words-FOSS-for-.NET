// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests related to WORDSNET-581 Add TableFormat property to Table object.
    /// This is about creating a public API to set table properties on the table, not on the rows.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class TestTableFormat
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }


        // FOSS: TestFloatingTablesJira6797 removed — loads a WordML (.wml) input and saves PDF;
        // both the WML reader and the PDF save path are removed.

        /// <summary>
        /// WORDSNET-5709 Layout gets disturbed upon converting DOCX to PDF
        /// Added special handling of floating table properties upon docx import.
        /// </summary>
        /// <remarks>
        /// MS Word shows negative tblpy as tblpYSpec = VerticalAlignment.Bottom in Table Positioning Props dialog,
        /// but this change is not written upon roundtrip. We could have potential layout bug with this.
        /// </remarks>
        [Test]
        public void TestTestFLoatingTablesJira5709()
        {
            // MS Word removes tblpPr when vAnchor = margin and no tblpY or zero tblpY specified only.
            Assert.That(GetTablePrFromTestFile("MsHacksHDefaultVMarginNoTblpY.docx").IsFloating, Is.False);
            Assert.That(GetTablePrFromTestFile("MsHacksHDefaultVMarginZeroTblpY.docx").IsFloating, Is.False);

            ValidateDefaultBehavior("MsHacksHMarginVMarginOneTblpY.docx", true);
            ValidateDefaultBehavior("MsHacksHDefaultVMarginOneTblpY.docx", false);
        }



        [Test]
        public void TestJira17857_Margin()
        {
            const string path = @"Model\FloatingTables\TestJira17857_margin";
            // FOSS: Docx2Doc and Docx2Rtf round-trips removed (Doc/Rtf writers gone).
            TestUtil.OpenSaveOpen(path, UnifiedScenario.Docx2Docx);
        }

        // FOSS: TestJira17857_Wml removed — every case is a WordML (.wml) Wml2Wml round-trip,
        // and both the WML reader and writer are removed. The same floating-table scenarios stay
        // covered by the Docx2Docx cases in TestJira17857.

        private static TablePr GetTablePrFromTestFile(string fileName)
        {
            Document doc = TestUtil.Open(String.Format(@"Model\FloatingTables\{0}", fileName));
            return ((Table)doc.GetChild(NodeType.Table, 0, true)).FirstRow.TablePr;
        }

        /// <summary>
        /// Validate that AW follows default behavior defined in specs/impl. notes: removal of default values and doing +1/-1 trick for FrameLeft.
        /// </summary>
        private static void ValidateDefaultBehavior(string fileName, bool isNondefaultHorizontalAnchor)
        {
            TablePr tablePr = GetTablePrFromTestFile(fileName);
            Assert.That(tablePr.IsFloating, Is.True);
            Assert.That(tablePr.RelativeVerticalPosition, Is.EqualTo(RelativeVerticalPosition.Margin));
            Assert.That(tablePr.HasNondefaultRelativeHorizontalPosition, Is.EqualTo(isNondefaultHorizontalAnchor));
            Assert.That(tablePr.HasNondefaultRelativeVerticalPosition, Is.False);
            Assert.That(tablePr.FrameLeft, Is.EqualTo(0));
        }

        /// <summary>
        /// Shows how various table formatting can now be specified on the table object.
        /// </summary>
        [Test]
        public void TestTableFormatGeneral()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);


            Table table = CreateTestTable(builder, "Default Table");


            table = CreateTestTable(builder, "Right Aligned, 300pt Width, AutoFit Off");
            Assert.That(table.Alignment, Is.EqualTo(TableAlignment.Left));
            table.Alignment = TableAlignment.Right;
            Assert.That(table.PreferredWidth, Is.EqualTo(PreferredWidth.FromPercent(100)));
            table.PreferredWidth = PreferredWidth.FromPoints(300);
            Assert.That(table.AllowAutoFit, Is.EqualTo(true));
            table.AllowAutoFit = false;


            table = CreateTestTable(builder, "Right-To-Left, Padding, Spacing");
            Assert.That(table.Bidi, Is.EqualTo(false));
            table.Bidi = true;
            Assert.That(table.TopPadding, Is.EqualTo(0));
            table.TopPadding = 5;
            Assert.That(table.BottomPadding, Is.EqualTo(0));
            table.BottomPadding = 10;
            Assert.That(table.LeftPadding, Is.EqualTo(5.4));
            table.LeftPadding = 0;
            Assert.That(table.RightPadding, Is.EqualTo(5.4));
            table.RightPadding = 3;
            Assert.That(table.CellSpacing, Is.EqualTo(0));
            table.CellSpacing = 1;


            table = CreateTestTable(builder, "Left Indent, Borders, Shading");
            Assert.That(table.LeftIndent, Is.EqualTo(0));
            table.LeftIndent = -50;
            table.SetBorder(BorderType.Left, LineStyle.Double, 3, DrColor.Red.ToNativeColor(), true);
            table.SetBorder(BorderType.Horizontal, LineStyle.Dot, 1, DrColor.Blue.ToNativeColor(), true);
            table.SetShading(TextureIndex.Texture20Percent, DrColor.LightGreen.ToNativeColor(), DrColor.Violet.ToNativeColor());


            table = CreateTestTable(builder, "No Borders");
            table.ClearBorders();


            TestUtil.Save(doc, @"Model\Nodes\TestTableFormatGeneral.docx", null, true);
        }

        /// <summary>
        /// Shows how we can apply table style now to the table.
        /// </summary>
        [Test]
        public void TestTableFormatStyle()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);


            Table table = CreateTestTable(builder, "TableNormal");
            table.ClearBorders();
            table.Style = doc.Styles[StyleIdentifier.TableNormal];


            table = CreateTestTable(builder, "Set style using identifier.");
            table.ClearBorders();
            table.StyleIdentifier = StyleIdentifier.TableSimple1;


            table = CreateTestTable(builder, "Set style using name.");
            table.ClearBorders();
            table.StyleName = "Medium Grid 1 Accent 1";


            table = CreateTestTable(builder, "Set style using object.");
            table.ClearBorders();
            table.Style = doc.Styles[StyleIdentifier.TableList8];


            TestUtil.Save(doc, @"Model\Nodes\TestTableFormatStyle.docx", null, true);
        }

        [Test]
        public void TestTableFormatStyleLook()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = CreateTestTable(builder, "None");
            table.ClearBorders();
            table.StyleIdentifier = StyleIdentifier.MediumGrid3Accent1;
            table.StyleOptions = TableStyleOptions.None;

            table = CreateTestTable(builder, "First and Last Rows");
            table.ClearBorders();
            table.StyleIdentifier = StyleIdentifier.MediumGrid3Accent1;
            table.StyleOptions = TableStyleOptions.FirstRow | TableStyleOptions.LastRow;

            table = CreateTestTable(builder, "First and Last Cols");
            table.ClearBorders();
            table.StyleIdentifier = StyleIdentifier.MediumGrid3Accent1;
            table.StyleOptions = TableStyleOptions.FirstColumn | TableStyleOptions.LastColumn;

            table = CreateTestTable(builder, "Row Bands");
            table.ClearBorders();
            table.StyleIdentifier = StyleIdentifier.MediumGrid3Accent1;
            table.StyleOptions = TableStyleOptions.RowBands;

            TestUtil.Save(doc, @"Model\Nodes\TestTableFormatStyleLook.docx", null, true);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestSetNonExistantStyle()
        {
            Document doc = new Document();
            Table table = new Table(doc);
            table.StyleName = "AAA";
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestSetWrongStyleType()
        {
            Document doc = new Document();
            Table table = new Table(doc);
            table.StyleName = "Normal";
        }

        private static Table CreateTestTable(DocumentBuilder builder, string headingText)
        {
            builder.ParagraphFormat.StyleName = "Heading 1";
            builder.Writeln(headingText);

            builder.ParagraphFormat.ClearFormatting();

            builder.InsertCell();
            builder.Write("AAA");
            builder.InsertCell();
            builder.Write("BBBBBBBBB");

            builder.InsertCell();
            builder.CellFormat.Borders.LineWidth = 3;
            builder.Write("CCCCCC");
            builder.EndRow();

            builder.InsertCell();
            builder.CellFormat.Borders.LineWidth = 0.5;
            builder.Write("DDD");

            builder.InsertCell();
            builder.CellFormat.Borders.LineWidth = 3;
            builder.Write("EEEEEEEEE");

            builder.InsertCell();
            builder.CellFormat.Borders.LineWidth = 0.5;
            builder.Write("FFFFFF");
            builder.EndRow();

            builder.InsertCell();
            builder.Write("GGG");
            builder.InsertCell();
            builder.Write("HHHHHHHHH");
            builder.InsertCell();
            builder.Write("IIIIII");
            builder.EndRow();

            Table table = builder.EndTable();

            builder.Writeln();
            return table;
        }

        // FOSS: TestJira7926A removed — it builds the table by loading an HTML string, and the
        // HTML reader is removed ("HTML file format is not supported in the FOSS version"). The
        // HTML import is the scenario (applying a style to an HTML-imported table with non-default
        // cell/row formatting), so it cannot be reproduced without the HTML reader. The sibling
        // TestJira7926B covers the style-application logic on a builder-created table.

        /// <summary>
        /// There is table with cell and row attributes applied,
        /// we have to set new style in the same way like MS Word does.
        /// </summary>
        [Test]
        public void TestJira7926B()
        {
            // This document was created in MSW 2013.
            const string filePath = @"Model\Nodes\TestJira7926B.docx";
            Document doc = TestUtil.Open(filePath);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);

            // LightListAccent1 style does not exist in AllStyles2013.docx in this case we
            // try to get this style from another BuiltInStyles collections (2007 or 2003) and import to the document.
            table.StyleIdentifier = StyleIdentifier.LightListAccent1;

            doc = TestUtil.SaveOpen(doc, filePath);
            table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Row row = table.Rows[1];
            Assert.That(row.RowFormat.HeightRule, Is.EqualTo(HeightRule.AtLeast));
            Assert.That(row.RowFormat.Borders[BorderType.Top].BorderWidth, Is.EqualTo(1.0f));
            Assert.That(row.FirstCell.CellPr[CellAttr.VerticalAlignment], Is.Null);
            Assert.That(row.TablePr[TableAttr.CellSpacing], Is.Null);
        }


        /// <summary>
        /// WORDSNET-14947 Question about values of margins in tables
        /// Added read-only public API for DistanceFromText table properties set.
        /// </summary>
        [Test]
        public void TestJira14947()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira14947.docx");
            Table table = doc.FirstSection.Body.Tables[1];

            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.DistanceTop, Is.EqualTo(11.35));
            Assert.That(table.DistanceBottom, Is.EqualTo(26.35));
            Assert.That(table.DistanceLeft, Is.EqualTo(9.05));
            Assert.That(table.DistanceRight, Is.EqualTo(22.7));
        }

        /// <summary>
        /// WORDSNET-15817 Expose Table.HorizontalAlignment property public.
        /// Added read-only public API for relative table alignments.
        /// </summary>
        [Test]
        public void TestJira15817()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira15817.docx");
            Table table = doc.FirstSection.Body.Tables[3];

            Assert.That(table.TextWrapping, Is.EqualTo(TextWrapping.Around));
            Assert.That(table.RelativeHorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center));
            Assert.That(table.RelativeVerticalAlignment, Is.EqualTo(VerticalAlignment.Default));
        }

        /// <summary>
        /// WORDSNET-15981 Get position of floating table in public API.
        /// Added read-only public API for all positioning attributes of floating table.
        /// </summary>
        [Test]
        public void TestJira15981()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira15981.xml");
            TableCollection tables = doc.FirstSection.Body.Tables;

            CheckTableCore15981(tables[0], RelativeHorizontalPosition.Page, RelativeVerticalPosition.Page, 171.15, 84.15, false);
            CheckTableCore15981(tables[1], RelativeHorizontalPosition.Margin, RelativeVerticalPosition.Margin, 183.3, 125.35, true);
            CheckTableCore15981(tables[2], RelativeHorizontalPosition.Column, RelativeVerticalPosition.Paragraph, 0, 0, false);
        }

        /// <summary>
        /// The helper method for validating floating table properties in <see cref="TestJira15981"/>.
        /// </summary>
        private static void CheckTableCore15981(Table table,
            RelativeHorizontalPosition hAnchor, RelativeVerticalPosition vAnchor,
            double hDistance, double vDistance,
            bool allowOverlap)
        {
            Assert.That(table.TextWrapping, Is.EqualTo(TextWrapping.Around));
            Assert.That(table.HorizontalAnchor, Is.EqualTo(hAnchor));
            Assert.That(table.VerticalAnchor, Is.EqualTo(vAnchor));
            Assert.That(MathUtil.AreEqual(hDistance, table.AbsoluteHorizontalDistance), Is.True, string.Format("expected: {0}, but was: {1}", hDistance, table.AbsoluteHorizontalDistance));
            Assert.That(MathUtil.AreEqual(vDistance, table.AbsoluteVerticalDistance), Is.True, string.Format("expected: {0}, but was: {1}", vDistance, table.AbsoluteVerticalDistance));
            Assert.That(table.AllowOverlap, Is.EqualTo(allowOverlap));
        }


        /// <summary>
        /// WORDSNET-17330 Added public property <see cref="Table.AllowCellSpacing"/>
        /// </summary>
        [Test]
        public void TestAllowCellSpacing()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = CreateTestTable(builder, "None");

            // Verify getter.
            // AllowCellSpacing is false by default.
            Assert.That(table.AllowCellSpacing, Is.False);

            // Specify cell spacing. It changes AllowCellSpacing to true;
            table.CellSpacing = 0;
            Assert.That(table.AllowCellSpacing, Is.True);

            // Verify setter.
            // Setting AllowCellSpacing to false removes CellSpacing.
            table.AllowCellSpacing = false;
            Assert.That(table.FirstRow.TablePr[TableAttr.CellSpacing], Is.Null);

            // Setting AllowCellSpacing to true adds initial values of CellSpacing, if it was not specified.
            table.AllowCellSpacing = true;
            Assert.That(table.CellSpacing, Is.EqualTo(ConvertUtilCore.TwipToPoint(Table.InitialCellSpacing)));

            // Setting AllowCellSpacing to true, when CellSpacing was specified, doesn't change CellSpacing.
            table.CellSpacing = 100;
            table.AllowCellSpacing = true;
            Assert.That(table.CellSpacing, Is.EqualTo(100));
        }

        /// <summary>
        /// WORDSNET-19655 Remove invalid nested table indent values during validation.
        /// Nested table indent handling reworked.
        /// </summary>
        [Test]
        public void Test19655()
        {
            const string fileName = @"Model\Table\Test19655";

            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            Table table = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.Tables[1];

            // At this point, it is clear that the negative LeftIndent for the nested table
            // was set to zero during the post-loading stage.

            // Checking that negative values ​​in the LeftIndent public setter for nested tables are ignored.
            table.LeftIndent = -30;
            Assert.That(table.LeftIndent, Is.EqualTo(0));

            // Preparing the negative indentation for the nested table to verify
            // (at the next step) that this indent is updated during the validation stage.
            table.TablePr.SetAttr(TableAttr.LeftIndent, -600);

            foreach (Row row in table.Rows)
                row.TablePr.SetAttr(TableAttr.LeftIndent, -600);

            Assert.That(table.LeftIndent, Is.EqualTo(-30));

            // Check that negative indentation for nested tables is updated during the validation stage.
            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx);

            table = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.Tables[1];
            Assert.That(table.LeftIndent, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-22302 Remove invalid nested table indent values during validation.
        /// SetShading() implementation extended to whole table.
        /// </summary>
        [Test]
        public void Test22302()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Client code to create the table.
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.InsertCell();
            builder.EndRow();
            builder.InsertCell();
            builder.InsertCell();
            table.CellSpacing = 8;

            table.SetShading(TextureIndex.Texture10Percent, Color.Red, Color.Red);

            foreach (Row row in table.Rows)
            {
                Assert.That(row.TablePr.Shading.Texture, Is.EqualTo(TextureIndex.Texture10Percent));
                Assert.That(row.TablePr.Shading.BackgroundPatternColorInternal, Is.EqualTo(DrColor.Red));

                foreach (Cell cell in row.Cells)
                {
                    Assert.That(cell.CellPr.Shading.Texture, Is.EqualTo(TextureIndex.Texture10Percent));
                    Assert.That(cell.CellPr.Shading.BackgroundPatternColorInternal, Is.EqualTo(DrColor.Red));
                }
            }
        }
    }
}
