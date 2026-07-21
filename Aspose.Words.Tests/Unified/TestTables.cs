// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Notes;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for tables.
    ///
    /// Most of the detailed table property reading and writing is done in TestTablePrFiler.
    /// These tests check that the whole thing hangs together and the produced document can be read back again.
    /// </summary>
    [TestFixture]
    public class TestTables : UnifiedTestsBase
    {
        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSimple(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestSimple", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("1.1\x0007\x0007\x00072.1\x00072.2\x0007\x0007\x000c"));
            Assert.That(doc.SelectNodes("//Table").Count, Is.EqualTo(1));
            Assert.That(doc.SelectNodes("//Row").Count, Is.EqualTo(2));
            Assert.That(doc.SelectNodes("//Cell").Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Check that table properties are read properly.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableMisc(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTable", lf, sf);

            NodeList rows = doc.SelectNodes("//Row");
            Assert.That(rows.Count, Is.EqualTo(2));

            Row row = (Row)rows[0];
            Assert.That(row.SelectNodes("Cell").Count, Is.EqualTo(4));
            Assert.That(row.GetText(), Is.EqualTo("A1\rAnother paragraph in a cell.\x0007B1\x0007C1\rC2\x0007D1\x0007\x0007"));

            // Check first row properties
            TablePr tablePr = row.TablePr;
            Assert.That(tablePr.LeftIndent, Is.EqualTo(0));
            Assert.That(tablePr.AllowAutoFit, Is.EqualTo(true));
            Assert.That(tablePr.LeftPadding, Is.EqualTo(0));
            Assert.That(tablePr.RightPadding, Is.EqualTo(0));
            Assert.That(row.Cells.Count, Is.EqualTo(4));
            Assert.That(row.Cells[0].CellPr.Width, Is.EqualTo(ConvertUtilCore.InchToTwip(1.5)));
            Assert.That(row.Cells[1].CellPr.Width, Is.EqualTo(ConvertUtilCore.InchToTwip(1)));
            Assert.That(row.Cells[2].CellPr.Width, Is.EqualTo(ConvertUtilCore.InchToTwip(2)));
            Assert.That(row.Cells[3].CellPr.Width, Is.EqualTo(ConvertUtilCore.InchToTwip(1.5)));

            // Test diagonal borders
            CellPr cellPr = row.Cells[0].CellPr;
            Assert.That(cellPr.BorderDiagonalDown.LineStyle, Is.EqualTo(LineStyle.Single));

            // Check the first vertically merged cell.
            cellPr = row.Cells[2].CellPr;
            Assert.That(cellPr.VerticalAlignment, Is.EqualTo(CellVerticalAlignment.Bottom));
            Assert.That(cellPr.VerticalMerge, Is.EqualTo(CellMerge.First));

            // Check second row
            row = (Row)rows[1];
            Assert.That(row.Cells.Count, Is.EqualTo(5));

            // Check cell border
            cellPr = row.Cells[1].CellPr;
            Assert.That(cellPr.BorderLeft.LineWidth, Is.EqualTo(3d));

            // Check the vertically merged cell. It is essentially empty, only fVertMerge is set.
            cellPr = row.Cells[2].CellPr;
            Assert.That(cellPr.VerticalMerge, Is.EqualTo(CellMerge.Previous));
        }


        /// <summary>
        /// Check that we write huge papx properly.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableHugePapxReadWrite(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTableHugePapx", lf, sf);

            NodeList rows = doc.SelectNodes("//Row");
            Assert.That(rows.Count, Is.EqualTo(4));
            Row row = (Row)rows[0];


            CellPr cellPr;
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Rtf2RtfNoGold:
                {
                    cellPr = row.Cells[0].CellPr;
                    break;
                }
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Wml:
                {
                    // In MS generated WordML this formatting is defined in 'w:tblStylePr' element of table style.
                    TableStyle style = (TableStyle)doc.Styles["Table Classic 3"];
                    ConditionalStyle firstRowPr = style.ConditionalStyles.FirstRow;
                    cellPr = firstRowPr.CellPr;
                    break;
                }
                default:
                    throw new InvalidOperationException("Unexpected scenario.");
            }
            // Check shading because I had problems with it.
            Shading shd = cellPr.Shading;
            // This is dark blue, but Word dark blue is 0x80 and Windows dark blue is 0x8b.
            Assert.That(shd.ForegroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0, 0, 0x80)));
            Assert.That(shd.BackgroundPatternColor.ToArgb(), Is.EqualTo(Color.White.ToArgb()));
            Assert.That(shd.Texture, Is.EqualTo(TextureIndex.TextureSolid));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableNested(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTableNested", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("Nested 1.1.1\x0007\x0007Nested 1.1\x0007Nested 1.2\x0007\x0007Outer 1\x0007" +
                "Outer 2\rNested 2.1\x0007Nested 2.2\x0007\x0007\x0007\x0007\x000c"));

            Cell l1Cell = (Cell)doc.SelectSingleNode("//Cell[1]");

            //Yes, there is a nested table available.
            Row l2Row = (Row)l1Cell.SelectSingleNode(".//Row");
            Assert.That(l2Row, IsNot.Null());

            //There are two cells in this row and one at deeper level.
            NodeList cells = l2Row.SelectNodes(".//Cell");
            Assert.That(cells.Count, Is.EqualTo(3));
            //Just check the content of the most nested cell.
            Cell l3Cell = (Cell)cells[1];
            Assert.That(l3Cell.GetText(), Is.EqualTo("Nested 1.1.1\x0007"));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTablePreferredWidthPercent(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTablePreferredWidthPercent", lf, sf);

            Row row = (Row)doc.SelectSingleNode("//Row");
            TablePr tablePr = row.TablePr;
            Assert.That(tablePr.PreferredWidth.Type, Is.EqualTo(PreferredWidthType.Percent));
            Assert.That(tablePr.PreferredWidth.ValueRaw, Is.EqualTo((int)(97.5 * 50)));    //Specified in 1/50 percent.
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellPadding(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestCellPadding", lf, sf);

            Table table = (Table)doc.GetChild(NodeType.Table, 2, true);

            //Table defaults.
            Assert.That(table.LeftPadding, Is.EqualTo(0d));
            Assert.That(table.TopPadding, Is.EqualTo(1d));
            Assert.That(table.BottomPadding, Is.EqualTo(2d));
            Assert.That(table.RightPadding, Is.EqualTo(3d));

            Row row = table.FirstRow;

            //This cell overrides left and right padding.
            Cell cell = (Cell)row.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(cell.CellFormat.LeftPadding, Is.EqualTo(10d));
            Assert.That(cell.CellFormat.TopPadding, Is.EqualTo(1d));
            Assert.That(cell.CellFormat.BottomPadding, Is.EqualTo(2d));
            Assert.That(cell.CellFormat.RightPadding, Is.EqualTo(10d));

            //This cell inherits all padding from the table.
            cell = (Cell)row.GetChildNodes(NodeType.Any, false)[2];
            Assert.That(cell.CellFormat.LeftPadding, Is.EqualTo(0d));
            Assert.That(cell.CellFormat.TopPadding, Is.EqualTo(1d));
            Assert.That(cell.CellFormat.BottomPadding, Is.EqualTo(2d));
            Assert.That(cell.CellFormat.RightPadding, Is.EqualTo(3d));
        }

        /// <summary>
        /// MS Word has different default for left and right cell padding in DOC/RTF vs DOCX/WML.
        /// This checks that we produce correct files that appear the same regardless of the saved format.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellPaddingCreatedTable(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            Table table = new Table(doc);
            doc.FirstSection.Body.AppendChild(table);
            Row row = new Row(doc);
            table.AppendChild(row);
            Cell cell = new Cell(doc);
            row.AppendChild(cell);
            Paragraph para = new Paragraph(doc);
            cell.AppendChild(para);
            cell.CellFormat.Borders.LineWidth = 1;
            cell.CellFormat.Borders.Color = Color.Black;
            cell.CellFormat.Borders.LineStyle = LineStyle.Single;
            Run run = new Run(doc, "Test");
            para.AppendChild(run);

            row = (Row)row.Clone(true);
            table.AppendChild(row);
            row.TablePr.LeftPadding = 200;

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestCellPaddingCreatedTable", lf, sf);
            row = (Row)doc.GetChild(NodeType.Row, 0, true);
            TablePr tablePr = row.TablePr;

            switch (sf)
            {
                case SaveFormat.Docx:
                case SaveFormat.Doc:
                case SaveFormat.WordML:
                    Assert.That(tablePr[TableAttr.LeftPadding], Is.EqualTo(108));
                    Assert.That(tablePr[TableAttr.RightPadding], Is.EqualTo(108));
                    break;
                case SaveFormat.Rtf:
                    // WORDSNET-12347 Direct attributes are collapsed over style.
                    Assert.That(tablePr[TableAttr.LeftPadding], Is.EqualTo(null));
                    Assert.That(tablePr[TableAttr.RightPadding], Is.EqualTo(null));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format");
            }
        }

        /// <summary>
        /// See comments in RowFormat, cell shading is MS Word does not actually seem to be inherited.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInheritCellShading(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestInheritCellShading", lf, sf);

            Row row = (Row)doc.SelectSingleNode("//Row");

            //Default table shading
            Cell cell = (Cell)row.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(cell.CellFormat.Shading.BackgroundPatternColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

            //Explicit shading off
            cell = (Cell)row.GetChildNodes(NodeType.Any, false)[1];
            Assert.That(cell.CellFormat.Shading.BackgroundPatternColor.ToArgb(), Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInheritCellBorders(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestInheritCellBorders", lf, sf);

            Row row = (Row)doc.SelectSingleNode("//Row[1]");
            Cell cell;

            // RK This info is stored in the table style in DOCX, but in other formats it is direct formatting.
            if (!(lf == LoadFormat.Docx || lf == LoadFormat.Doc))
            {
                // Check default table borders
                Assert.That(row.RowFormat.Borders[BorderType.Top].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(row.RowFormat.Borders[BorderType.Left].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(row.RowFormat.Borders[BorderType.Bottom].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(row.RowFormat.Borders[BorderType.Right].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(row.RowFormat.Borders[BorderType.Horizontal].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(row.RowFormat.Borders[BorderType.Vertical].LineStyle, Is.EqualTo(LineStyle.Single));

                // Check the cell inherits default table borders
                cell = (Cell)row.GetChildNodes(NodeType.Any, false)[0];
                Assert.That(cell.CellFormat.Borders[BorderType.Top].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders[BorderType.Left].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders[BorderType.Bottom].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders[BorderType.Right].LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders[BorderType.DiagonalDown].LineStyle, Is.EqualTo(LineStyle.None));
                Assert.That(cell.CellFormat.Borders[BorderType.DiagonalUp].LineStyle, Is.EqualTo(LineStyle.None));
            }

            // Borders are explicitly off for this cell
            cell = (Cell)row.GetChildNodes(NodeType.Any, false)[2];
            Assert.That(cell.CellFormat.Borders[BorderType.Top].LineStyle, Is.EqualTo(LineStyle.None));
            Assert.That(cell.CellFormat.Borders[BorderType.Left].LineStyle, Is.EqualTo(LineStyle.None));
            Assert.That(cell.CellFormat.Borders[BorderType.Bottom].LineStyle, Is.EqualTo(LineStyle.None));
            Assert.That(cell.CellFormat.Borders[BorderType.Right].LineStyle, Is.EqualTo(LineStyle.None));

            // Borders are explicitly set
            row = (Row)doc.SelectSingleNode("//Row[2]");
            cell = (Cell)row.GetChildNodes(NodeType.Any, false)[1];
            Assert.That(cell.CellFormat.Borders[BorderType.Top].Color.ToArgb(), Is.EqualTo(0));    //Default
            Assert.That(cell.CellFormat.Borders[BorderType.Left].Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(cell.CellFormat.Borders[BorderType.Bottom].Color.ToArgb(), Is.EqualTo(0));//Default
            Assert.That(cell.CellFormat.Borders[BorderType.Right].Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
        }

        /// <summary>
        /// Test that borders for different cell sides are correctly resolved not only
        /// to left, top, right and bottom, but also for horizontal and vertical (intra cell) borders.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInheritCellBordersInside(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestInheritCellBordersInside", lf, sf);

            //Check default table borders
            Row row = (Row)doc.SelectSingleNode("//Row[1]");
            Assert.That(row.RowFormat.Borders[BorderType.Top].LineWidth, Is.EqualTo(3d));
            Assert.That(row.RowFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(3d));
            Assert.That(row.RowFormat.Borders[BorderType.Bottom].LineWidth, Is.EqualTo(3d));
            Assert.That(row.RowFormat.Borders[BorderType.Right].LineWidth, Is.EqualTo(3d));
            Assert.That(row.RowFormat.Borders[BorderType.Horizontal].LineWidth, Is.EqualTo(0.75));
            Assert.That(row.RowFormat.Borders[BorderType.Vertical].LineWidth, Is.EqualTo(0.75));

            //Check top left cell
            Cell cell = (Cell)row.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(cell.CellFormat.Borders[BorderType.Top].LineWidth, Is.EqualTo(3d));
            Assert.That(cell.CellFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(3d));
            Assert.That(cell.CellFormat.Borders[BorderType.Bottom].LineWidth, Is.EqualTo(0.75));
            Assert.That(cell.CellFormat.Borders[BorderType.Right].LineWidth, Is.EqualTo(0.75));

            //Check bottom right cell
            row = (Row)doc.SelectSingleNode("//Row[3]");
            cell = (Cell)row.GetChildNodes(NodeType.Any, false)[2];
            Assert.That(cell.CellFormat.Borders[BorderType.Top].LineWidth, Is.EqualTo(0.75));
            Assert.That(cell.CellFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(0.75));
            Assert.That(cell.CellFormat.Borders[BorderType.Bottom].LineWidth, Is.EqualTo(3d));
            Assert.That(cell.CellFormat.Borders[BorderType.Right].LineWidth, Is.EqualTo(3d));
        }

        /// <summary>
        /// Had a problem with writing textbox text anchored to a cell of a table.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShapeAtRow(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestShapeAtRow", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("Textbox\r" +                        //end of para
                "Cell in Textbox\r" +                //end of para
                "Nested Cell in Textbox\x0007\x0007" +    //end of nested cell and end of nested row
                "\x0007\x0007" +                    //end of the outer cell and row
                "\r" +                                //last char in the textbox
                "Cell\r" +                            //end if para
                "Nested Cell\x0007\x0007" +            //end of nested cell and roe
                "\x0007\x0007" +                    //end of outer cell and row
                "\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestManyColumns(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestManyColumns", lf, sf);

            // alexnosk: Originally XPath "//Row[1]/Cell" was used to select cells, chnged code for C++ porting.
            NodeCollection cells = ((Row)doc.GetChild(NodeType.Row, 0, true)).Cells;

            Assert.That(cells.Count, Is.EqualTo(53));
            Assert.That(((Cell)cells[0]).CellPr.Shading.BackgroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0xff, 0, 0)));
            Assert.That(((Cell)cells[25]).CellPr.Shading.BackgroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0, 0xff, 0)));
            Assert.That(((Cell)cells[50]).CellPr.Shading.BackgroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0, 0, 0xff)));
        }



        /// <summary>
        /// WORDSNET-837 RowFormat.AllowBreakAcrossPages is not saved when document is saved by Aspose.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect837(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestDefect837", lf, sf);

            Row row = (Row)doc.GetChild(NodeType.Row, 0, true);
            row.RowFormat.AllowBreakAcrossPages = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestDefect837 Modified", lf, sf);

            row = (Row)doc.GetChild(NodeType.Row, 0, true);
            Assert.That(row.RowFormat.AllowBreakAcrossPages, Is.EqualTo(false));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCloneRowsInvalidatesEnumerator(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestCloneRowsInvalidatesEnumerator", lf, sf);

            Table srcTable = doc.Sections[0].Body.Tables[0];
            Table dstTable = doc.Sections[0].Body.Tables[1];

            foreach (Row srcRow in srcTable.Rows)
                dstTable.Rows.Add(srcRow.Clone(true));

            Assert.That(dstTable.GetText(), Is.EqualTo("Row2\x0007\x0007Row1\x0007\x0007"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEmptyCell(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestEmptyCell", lf, sf);
            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            Paragraph para = cell.FirstParagraph;
            Assert.That(para.ParagraphBreakFont.Bold, Is.EqualTo(true));
            Assert.That(para.ParagraphBreakFont.Size, Is.EqualTo(20d));
        }



        /// <summary>
        /// Verifies padding in table rows. Relates to Defect833.
        /// </summary>
        private static void VerifyTestDefect833(Document doc, LoadFormat lf, SaveFormat sf)
        {
            // first row properties
            Row row1 = (Row)doc.GetNodeById("0.1.0.0");
            object left1 = row1.TablePr[TableAttr.LeftPadding];
            object right1 = row1.TablePr[TableAttr.RightPadding];

            // second row properties
            Row row2 = (Row)doc.GetNodeById("1.1.0.0");
            object left2 = row2.TablePr[TableAttr.LeftPadding];
            object right2 = row2.TablePr[TableAttr.RightPadding];

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Wml:
                    Assert.That(left1, Is.EqualTo(null));
                    Assert.That(right1, Is.EqualTo(0));
                    Assert.That(left2, Is.EqualTo(null));
                    Assert.That(right2, Is.EqualTo(108));
                    break;

                case UnifiedScenario.Doc2Rtf:
                    Assert.That(left1, Is.EqualTo(null));
                    Assert.That(right1, Is.EqualTo(0));
                    Assert.That(left2, Is.EqualTo(null));
                    // 'right2' has direct value '108' in the testing DOC.
                    // When this document is exported to RTF and then opened again this value
                    // will be equal to 'null' due to collapsed over the style. So, it has different values
                    // for different formats and therefore, we are not checking it here.
                    break;

                case UnifiedScenario.Rtf2RtfNoGold:
                    // WORDSNET-12347 Direct attributes are collapsed over table style.
                    Assert.That(left1, Is.EqualTo(null));
                    Assert.That(right1, Is.EqualTo(null));
                    Assert.That(left2, Is.EqualTo(null));
                    Assert.That(right2, Is.EqualTo(null));
                    break;

                default:
                    throw new InvalidOperationException("Unknown unified scenario.");
            }
        }


        /// <summary>
        /// Code example how to replace text in a cell with new text, yet preserve formatting.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestReplaceCell(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestReplaceCell", lf, sf);
            DocumentBuilder b = new DocumentBuilder(doc);

            // Use the document builder to move to the cell and insert text.
            // When using document builder, the newly inserted text will take on
            // formatting at the current position in the document.
            b.MoveToCell(0, 0, 0, 0);
            b.Writeln("New text");

            //If the last call was Writeln, then the cursor is at the beginning
            //of the first paragraph with the original text.
            //The original text in the cell can be deleted in a loop like this.
            Node node = b.CurrentParagraph;
            while (node != null)
            {
                Node nextNode = node.NextSibling;
                node.Remove();
                node = nextNode;
            }

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestReplaceCell Modified", lf, sf);

            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            Assert.That(cell.GetText(), Is.EqualTo("New text\x0007"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestReplaceCell1(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestReplaceCell", lf, sf);

            Cell cell = doc.Sections[0].Body.Tables[0].Rows[0].Cells[0];
            //"Insert after null" actually inserts before the first child.
            cell.FirstParagraph.InsertAfter(new BookmarkStart(doc, "bmk"), null);
            cell.LastParagraph.AppendChild(new BookmarkEnd(doc, "bmk"));

            Bookmark bmk = cell.Range.Bookmarks["bmk"];
            bmk.Text = "New text";

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestReplaceCell1 Modified", lf, sf);

            cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            Assert.That(cell.GetText(), Is.EqualTo("New text\x0007"));
        }

        /// <summary>
        /// WORDSNET-854 Error while saving document in AsposePdf xml.
        /// Forum thread:
        ///   'save to stream as FormatAsposePdf throws exception'
        ///   https://www.aspose.com/Community/Forums/43433/ShowPost.aspx
        /// </summary>
        [Test]
        public void TestSaveEmptyTable()
        {
            Document doc = new Document();
            doc.FirstSection.Body.AppendChild(new Table(doc));

            doc.Save(new MemoryStream(), SaveFormat.Markdown);
        }

        /// <summary>
        /// WORDSNET-897 Imported table is treated by MS Word as 'corrupted' in the resulting document.
        /// The problem was caused by table being at the very end of a document. Document needs a paragraph at the end.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableAtDocumentEnd(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTableAtDocumentEnd", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("Test\x0007\x0007\x000c"));

            Body body = doc.FirstSection.Body;

            // There is a table and a paragraph in the document.
            // Move the table to be after the paragraph and attempt to save.
            Node table = body.FirstChild;
            table.Remove();
            body.AppendChild(table);

            // When saving in DOC format, a new empty paragraph is automatically appended at the end
            // because a table is not allowed to be the last element in a document.
            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestTableAtDocumentEnd Modified", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("\rTest\x0007\x0007\x000c"));
        }

        /// <summary>
        /// Test getting/setting CellFormat.FitText property.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellFitText(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(72);
            builder.CellFormat.FitText = true;
            builder.Write("This is a very very very very very long text.");

            builder.InsertCell();
            builder.CellFormat.FitText = false;
            builder.Write("This is a very very very very very long text.");

            Table table = builder.EndTable();
            table.PreferredWidth = PreferredWidth.Auto;
            table.AllowAutoFit = false;


            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestCellFitText", lf, sf);

            Row row = doc.FirstSection.Body.Tables[0].Rows[0];
            Assert.That(row.Cells[0].CellFormat.FitText, Is.True);
            Assert.That(row.Cells[1].CellFormat.FitText, Is.False);
        }

        /// <summary>
        /// Test getting/setting CellFormat.WrapText property.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellWrapText(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();

            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertCell();
            builder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(72);
            builder.CellFormat.WrapText = true;
            builder.Write("This is a very very very very very long text.");

            builder.InsertCell();
            builder.CellFormat.WrapText = false;
            builder.Write("This is a very very very very very long text.");

            Table table = builder.EndTable();
            table.PreferredWidth = PreferredWidth.Auto;
            table.AllowAutoFit = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestCellWrapText", lf, sf);

            Row row = doc.FirstSection.Body.Tables[0].Rows[0];
            Assert.That(row.Cells[0].CellFormat.WrapText, Is.True);
            Assert.That(row.Cells[1].CellFormat.WrapText, Is.False);
        }


        /// <summary>
        /// WORDSNET-1133 Setting Borders.LineWidth to value greater than 0 without explicit
        /// setting of Borders.LineStyle to value other than LineStyle.None crashes Word 2000 and 2002.
        /// </summary>
        [Test]
        public void TestDefect1133()
        {
            DocumentBuilder b = new DocumentBuilder();
            b.InsertCell();
            b.RowFormat.Borders.LineWidth = 0;
            b.RowFormat.Borders.LineStyle = LineStyle.None;

            b.RowFormat.Borders.LineWidth = 1;
            // Line style is automatically changed to single when non zero border width is set.
            Assert.That(b.RowFormat.Borders.LineStyle, Is.EqualTo(LineStyle.Single));

            //The other way around too. Setting line style none automatically sets zero line width.
            b.RowFormat.Borders.LineStyle = LineStyle.None;
            Assert.That(b.RowFormat.Borders.LineWidth, Is.EqualTo(0d));
        }

        /// <summary>
        /// WORDSNET-1277 Inserting a HTML table using InsertHtml makes tables inserted after corrupted.
        /// </summary>
        [Test]
        public void TestDefect1277()
        {
            CellPr cellPr = new CellPr();
            CellFormat cellFormat = new CellFormat(cellPr);
            cellFormat.LeftPadding = 100;
            cellFormat.Width = 110;
            cellFormat.PreferredWidth = PreferredWidth.FromPoints(120);

            // It was that CellFormat.ClearFormatting reset cell width to zero and that value stayed in the builder.
            cellFormat.ClearFormatting();

            // Most of the cell format values do indeed get reset.
            Assert.That(cellFormat.LeftPadding, Is.EqualTo(0.0));

            // But width of the cell does not get reset.
            Assert.That(cellFormat.Width, Is.EqualTo(110.0));
            Assert.That((PreferredWidth.FromPoints(120).Equals(cellFormat.PreferredWidth)), Is.True);
        }

        /// <summary>
        /// WORDSNET-1348 Setting borders properties for cell that is not appended to the row yet results in NullReferenceException.
        /// </summary>
        [Test]
        public void TestDefect1348()
        {
            Document doc = new Document();

            Cell cell = new Cell(doc);
            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Single;
            Assert.That(cell.CellFormat.Borders.Left.LineStyle, Is.EqualTo(LineStyle.Single));

            Row row = new Row(doc);
            row.RowFormat.Borders.Left.LineStyle = LineStyle.Double;
            Assert.That(row.RowFormat.Borders.Left.LineStyle, Is.EqualTo(LineStyle.Double));

            // This was not in the original bug, but lets try it anyway.
            Run run = new Run(doc);
            run.Font.Border.LineStyle = LineStyle.Dot;
            Assert.That(run.Font.Border.LineStyle, Is.EqualTo(LineStyle.Dot));


            Paragraph para = new Paragraph(doc);
            para.ParagraphFormat.Borders.Left.LineStyle = LineStyle.DotDash;
            Assert.That(para.ParagraphFormat.Borders.Left.LineStyle, Is.EqualTo(LineStyle.DotDash));
        }

        /// <summary>
        /// WORDSNET-1420 Sequential floating tables are put into one table in the model.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFloatingTables(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestFloatingTables", lf, sf);

            Assert.That(doc.GetChildNodes(NodeType.Table, true).Count, Is.EqualTo(3));
            CheckTable1420(doc, 0, "January 2006\x0007\x0007", 7);
            CheckTable1420(doc, 1, "February 2006\x0007\x0007", 7);
            CheckTable1420(doc, 2, "March 2006\x0007\x0007", 7);
        }

        private static void CheckTable1420(Document doc, int tableIndex, string firstRowText, int rowCount)
        {
            Table table = (Table)doc.GetChild(NodeType.Table, tableIndex, true);
            Assert.That(table.FirstRow.GetText(), Is.EqualTo(firstRowText));
            Assert.That(table.Rows.Count, Is.EqualTo(rowCount));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDirectionInCell(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestDirectionInCell", lf, sf);
            CheckCellDirection(doc, 0, TextOrientation.Horizontal);
            CheckCellDirection(doc, 1, TextOrientation.Upward);
            CheckCellDirection(doc, 2, TextOrientation.VerticalFarEast);
            CheckCellDirection(doc, 3, TextOrientation.Downward);
            CheckCellDirection(doc, 4, TextOrientation.HorizontalRotatedFarEast);
        }

        /// <summary>
        /// Checks text orientation for the cell with the specified index within the document.
        /// </summary>
        private static void CheckCellDirection(Document doc, int cellIndex, TextOrientation expectedOrientation)
        {
            Cell cell = (Cell)doc.GetChild(NodeType.Cell, cellIndex, true);
            Assert.That(cell.CellFormat.Orientation, Is.EqualTo(expectedOrientation));
        }

        /// <summary>
        /// WORDSNET-3381 Error with table containing in a cell a Paragraph and a Sub-table
        /// https://www.aspose.com/Community/Forums/thread/84251.aspx
        /// Nested table corrupts DOC document.
        /// Open the resulting document in Word 2003.
        /// Fixed. A cell should not end with a table, it should end with a paragraph.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect3381(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();

            doc.FirstSection.Body.RemoveAllChildren();

            Table table = new Table(doc);
            Row row = new Row(doc);
            table.Rows.Add(row);
            table.PreferredWidth = PreferredWidth.Auto;
            table.AllowAutoFit = false;

            Cell cell = new Cell(doc);
            cell.CellFormat.Width = 150;
            row.Cells.Add(cell);

            Table subTable = new Table(doc);
            Row subRow = new Row(doc);
            subTable.Rows.Add(subRow);
            table.PreferredWidth = PreferredWidth.Auto;
            table.AllowAutoFit = false;

            Cell subCell0 = new Cell(doc);
            subCell0.CellFormat.Width = 30;
            subRow.Cells.Add(subCell0);

            cell.AppendChild(new Paragraph(doc));
            cell.AppendChild(subTable);

            doc.FirstSection.Body.AppendChild(table);

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestDefect3381", lf, sf);

            // First empty paragraph, cell end, row end, cell end, row end, section end.
            Assert.That(doc.GetText(), Is.EqualTo("\r\x0007\x0007\x0007\x0007\x000c"));
        }

        /// <summary>
        /// WORDSNET-4237 Table alignment does not work in created DOC files.
        /// The problem was related to writing FIB. I made writing FIB in Word 2000 format for now.
        /// There is no actual test here, just open the generated files in Word and see.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect4237(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder docBuilder = new DocumentBuilder();

            docBuilder.Write("pre");

            docBuilder.CellFormat.PreferredWidth = PreferredWidth.FromPoints(72);
            docBuilder.InsertCell();
            docBuilder.Write("A1");
            docBuilder.InsertCell();
            docBuilder.Write("B1");
            docBuilder.EndRow();
            docBuilder.InsertCell();
            docBuilder.Write("A2");
            docBuilder.InsertCell();
            docBuilder.Write("B2");
            docBuilder.EndRow();

            Table table = docBuilder.EndTable();

            table.PreferredWidth = PreferredWidth.Auto;
            table.AllowAutoFit = false;
            table.Alignment = TableAlignment.Center;

            docBuilder.Write("post");

            TestUtil.SaveOpen(docBuilder.Document, @"Model\Table\TestDefect4237", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAll(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestAll", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorders(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestBorders", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellAlignment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestCellAlignment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellBorders(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestCellBorders", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellProperties(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestCellProperties", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFloating(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestFloating", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFormattingRevision(LoadFormat lf, SaveFormat sf)
        {
            const string testName = @"Model\Table\TestFormattingRevision";
            Document doc;
            UnifiedScenario us = TestUtil.GetUnifiedScenario(lf, sf);

            // WORDSNET-5855 andrnosk: Do not perform comparing with gold because in some cases
            // we have difference between processing Width and PreferredWidth upon reading Doc and Docx/Wml.
            if ((us == UnifiedScenario.Doc2Docx) || (us == UnifiedScenario.Doc2Wml))
            {
                // Open/Save/Open without comparing with gold
                doc = TestUtil.Open(testName, lf);
                string outFileName = TestUtil.Save(doc, testName, SaveOptions.CreateSaveOptions(sf), false);
                doc = TestUtil.Open(outFileName);
            }
            else
            {
                doc = TestUtil.OpenSaveOpen(testName, lf, sf);
            }

            // Check that original indent is not set i.e null.
            Row row = (Row) doc.GetNodeById("0.0.0.0");
            TablePr tablePr = row.TablePr;
            CellPr cellPr = row.FirstCell.CellPr;


            switch (us)
            {
                case UnifiedScenario.Doc2Doc:
                    // No explicit indent, formatting revision is ok.
                    Assert.That(tablePr[TableAttr.LeftIndent], Is.EqualTo(null));
                    Assert.That(tablePr.FormatRevision.RevPr[TableAttr.LeftIndent], Is.EqualTo(200));
                    break;

                case UnifiedScenario.Doc2Rtf:
                    // WORDSNET-12347 Table styles are supported in RTF. Direct attributes should be collapsed with style.
                    // Indent is resolved, formatting revision is lost.
                    Assert.That(tablePr[TableAttr.LeftIndent], Is.EqualTo(0));
                    Assert.That(tablePr.FormatRevision, Is.EqualTo(null));
                    break;


                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Docx2DocxNoGold:
                    // Indent is resolved, formatting revision is lost.
                    Assert.That(tablePr[TableAttr.LeftIndent], Is.EqualTo(200));
                    Assert.That(tablePr.FormatRevision, Is.EqualTo(null));
                    Assert.That(cellPr.FormatRevision, IsNot.Null());
                    Assert.That(cellPr.Count, Is.EqualTo(3));
                    // AM. I tested this and there are actually two properties:
                    // cell width and vertical alignment.
                    CellPr revisedCellPr = (CellPr)cellPr.FormatRevision.RevPr;
                    Assert.That(revisedCellPr.Count, Is.EqualTo(2));
                    // Original width.
                    Assert.That(cellPr[CellAttr.Width], Is.EqualTo(9571));
                    // Final width.
                    Assert.That(revisedCellPr[CellAttr.Width], Is.EqualTo(9371));
                    break;

                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    Assert.That(tablePr[TableAttr.LeftIndent], Is.EqualTo(200));
                    Assert.That(tablePr.FormatRevision, Is.EqualTo(null));
                    break;

                default:
                    throw new InvalidOperationException("Unknown unified scenario.");

            }
        }


        /// <summary>
        /// Inserting and deleting row logs insert or delete run revisions OK.
        ///
        /// Format a cell logged as a formatting revision in TablePr OK.
        ///
        /// Delete, split and merge cell operations bring up a dialog box that says this
        /// operation will not result in a tracked change.
        ///
        /// BUT Inserting a cell does not bring up a warning, but it DOES NOT FULLY WORK either.
        /// Sometimes it is possible to reject the change that inserted a cell, but sometimes
        /// it is not possible in MS Word. It is usually not possible when the cell is inserted
        /// before the first cell, but maybe there are other scenarios.
        ///
        /// When a cell was inserted, it seems to log TablePr with the new number of CellPr both
        /// in the original and in the revised TablePr. I guess it does not create any problems for me.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInsertCellRevision(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestInsertCellRevision", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNested(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestNested", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNestedEmptyPara(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestNestedEmptyPara", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRightMargin(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestRightMargin", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRowHeight(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestRowHeight", lf, sf);

            Row row = (Row)doc.GetChild(NodeType.Row, 0, true);
            Assert.That(row.TablePr.Height, Is.EqualTo(720));
            Assert.That(row.TablePr.HeightRule, Is.EqualTo(HeightRule.AtLeast));

            row = (Row)doc.GetChild(NodeType.Row, 1, true);
            Assert.That(row.TablePr.Height, Is.EqualTo(0));
            Assert.That(row.TablePr.HeightRule, Is.EqualTo(HeightRule.Auto));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRowProperties(LoadFormat lf, SaveFormat sf)
        {
           TestUtil.OpenSaveOpen(@"Model\Table\TestRowProperties", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRtl(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestRtl", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShading(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestShading", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShading1(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestShading1", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStyle(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestStyle", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableAlignment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestTableAlignment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableBorders(LoadFormat lf, SaveFormat sf)
        {
           TestUtil.OpenSaveOpen(@"Model\Table\TestTableBorders", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableProperties(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTableProperties", lf, sf);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Row row = table.FirstRow;
            Assert.That(table.PreferredWidth.IsAuto, Is.EqualTo(true));
            Assert.That(table.Alignment, Is.EqualTo(TableAlignment.Left));
            Assert.That(table.AllowAutoFit, Is.EqualTo(false));
            Assert.That(table.CellSpacing, Is.EqualTo(0.0));
            Assert.That(table.LeftIndent, Is.EqualTo(20.0));
            Assert.That(row.Cells[0].CellPr.Width, Is.EqualTo(4785));
            Assert.That(row.Cells[1].CellPr.Width, Is.EqualTo(4786));

            table = (Table)doc.GetChild(NodeType.Table, 1, true);
            row = table.FirstRow;
            Assert.That(table.PreferredWidth.Value, Is.EqualTo(80.5));
            Assert.That(table.Alignment, Is.EqualTo(TableAlignment.Center));
            Assert.That(table.AllowAutoFit, Is.EqualTo(true));
            Assert.That(table.CellSpacing, Is.EqualTo(1.0)); // It is half of the spacing.
            Assert.That(table.LeftIndent, Is.EqualTo(0.0));
            Assert.That(row.Cells[0].CellPr.Width, Is.EqualTo(4154));
            Assert.That(row.Cells[1].CellPr.Width, Is.EqualTo(3671));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableStyles(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestTableStyles", lf, sf);

            // RTF format misses "NoList" Style, same as in MSW.
            int stylesCount = (sf == SaveFormat.Rtf) ? 49 : 50;

            Assert.That(doc.Styles.Count, Is.EqualTo(stylesCount));

            // Test some details in one table style.
            TableStyle tableStyle = (TableStyle)doc.Styles.GetByName("Table 3D effects 1", false);
            Assert.That(tableStyle.Name, Is.EqualTo("Table 3D effects 1"));
            Assert.That(tableStyle.CellPr.Shading.ForegroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0xc0, 0xc0, 0xc0)));
            ConditionalStyle conditionalStyle = tableStyle.ConditionalStyles.FirstRow;
            Assert.That(conditionalStyle.RunPr.Color.Equals(new DrColor(0xff, 0x80, 0x00, 0x80)), Is.True);

            // Test another table style.
            tableStyle = (TableStyle)doc.Styles.GetByName("Table Colorful 2", false);
            Assert.That(tableStyle.TablePr.BorderBottom.Equals(new Border(LineStyle.Single, 12,
                                                                  new DrColor(0xff, 0x00, 0x00, 0x00))), Is.True);
            // There is difference between doc and docx reading. DOC reads Border.Empty, DOCX reads null.
            switch(lf)
            {
                case LoadFormat.Docx:
                case LoadFormat.WordML:
                case LoadFormat.Rtf:
                    Assert.That(tableStyle.TablePr.BorderTop == null, Is.True);
                    break;

                default:
                    Assert.That(tableStyle.TablePr.BorderTop.Equals(Border.Empty), Is.True);
                    break;
            }

            // style has no conditional formatting for last row
            Assert.That(tableStyle.ConditionalStyles.ContainsTableStyleOverride(TableStyleOverrideType.LastRow), Is.False);
            conditionalStyle = tableStyle.ConditionalStyles.FirstRow;
            RunPr firstRowPr = conditionalStyle.RunPr;
            Assert.That(firstRowPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(firstRowPr.Italic, Is.EqualTo(AttrBoolEx.True));

            // WML files has explicit shading written. Is it some kind of compatibility?
            // RTF files has explicit borders and shading written.
            if ((lf != LoadFormat.WordML) && (sf != SaveFormat.Rtf))
            {
                // Ensure none cell has explicit border or shading.
                NodeCollection cells = doc.GetChildNodes(NodeType.Cell, true);
                foreach (Cell cell in cells)
                {
                    Assert.That(cell.CellPr[CellAttr.BorderLeft], Is.Null);
                    Assert.That(cell.CellPr[CellAttr.BorderRight], Is.Null);
                    Assert.That(cell.CellPr[CellAttr.BorderTop], Is.Null);
                    Assert.That(cell.CellPr[CellAttr.BorderBottom], Is.Null);
                    Assert.That(cell.CellPr[CellAttr.Shading], Is.Null);
                }

                NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
                foreach (Run run in runs)
                {
                    Assert.That(run.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
                    Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));
                    Assert.That(DrColor.Empty, Is.EqualTo(run.RunPr.Color));
                }

            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestZeroVisibleBorder(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Table\TestZeroVisibleBorder", lf, sf);
        }


        /// <summary>
        /// WORDSNET-3623 Position of list item is incorrect after converting RTF to DOC/DOCX.
        /// </summary>
        /// <remarks>
        /// To avoid the condition where the tblpPr element is ignored, Word adds 1 twentieth of a point
        /// to the actual distance value when it saves a file and subtracts 1 twentieth of a point to the actual distance value when it opens a file.
        /// See "MS-DOC 2.9.354 YAS_plusOne, 2.9.348 XAS_plusOne".
        /// See "MS-OI29500 17.4.58. tblpPr (Floating Table Positioning)".
        /// </remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira3623(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestJira3623", lf, sf);
            Row row = (Row)doc.GetNodeById("0.1.0.0");
            Assert.That(row.TablePr.FrameTop, Is.EqualTo(0));
            Assert.That(row.TablePr.FrameLeft, Is.EqualTo(108));
        }

        /// <summary>
        /// This test verifies that we automatically add one empty paragraph after a table
        /// if the table is the last node in a story (in any story).
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEnsureStoryBreakAfterTable(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            doc.RemoveAllChildren();

            Section sect = new Section(doc);
            doc.AppendChild(sect);

            // non empty header with table
            HeaderFooter headerWithTableAtEnd = new HeaderFooter(doc, HeaderFooterType.HeaderPrimary);
            sect.AppendChild(headerWithTableAtEnd);
            headerWithTableAtEnd.AppendChild(Create1x1Table(doc));

            // empty footer
            HeaderFooter emptyHeader = new HeaderFooter(doc, HeaderFooterType.FooterPrimary);
            sect.AppendChild(emptyHeader);

            // body
            Body body = new Body(doc);
            sect.AppendChild(body);

            Paragraph paraInBody = new Paragraph(doc);
            body.AppendChild(paraInBody);

            // empty textbox
            Shape emptyTextbox = new Shape(doc, ShapeType.TextBox);
            paraInBody.AppendChild(emptyTextbox);
            emptyTextbox.Width = 100;
            emptyTextbox.Height = 100;
            emptyTextbox.WrapType = WrapType.Inline;

            // non empty textbox
            Shape textboxWithTableAtEnd = new Shape(doc, ShapeType.TextBox);
            paraInBody.AppendChild(textboxWithTableAtEnd);
            textboxWithTableAtEnd.AppendChild(Create1x1Table(doc));
            textboxWithTableAtEnd.Width = 100;
            textboxWithTableAtEnd.Height = 100;
            textboxWithTableAtEnd.WrapType = WrapType.Inline;

            // empty comment
            Comment emptyComment = new Comment(doc);
            paraInBody.AppendChild(emptyComment);

            // non empty comment
            Comment commentWithTableAtEnd = new Comment(doc);
            paraInBody.AppendChild(commentWithTableAtEnd);
            commentWithTableAtEnd.AppendChild(Create1x1Table(doc));

            // empty footnote
            Footnote emptyFootnote = new Footnote(doc, FootnoteType.Footnote);
            paraInBody.AppendChild(emptyFootnote);

            // non empty footnote
            Footnote footnoteWithTableAtEnd = new Footnote(doc, FootnoteType.Footnote);
            paraInBody.AppendChild(footnoteWithTableAtEnd);
            footnoteWithTableAtEnd.AppendChild(Create1x1Table(doc));

            // talbe in body
            Table tableInBody = Create1x1Table(doc);
            body.AppendChild(tableInBody);

            // nested table
            Table tableInCell = Create1x1Table(doc);
            tableInBody.FirstRow.FirstCell.AppendChild(tableInCell);

            Assert.That(doc.GetText(), Is.EqualTo("\x0007" +          // end of row, but no paragraph for empty cell
                "" +                // no paragraph break after a table

                "" +                // nothing for empty footer

                // body start here
                "" +                // nothing for empty textbox

                "\x0007" +          // end of row, but no paragraph for empty cell
                "" +                // no paragraph break after a table

                "" +                // nothing for empty comment

                "\x0007" +          // end of row, but no paragraph for empty cell
                "" +                // no paragraph break after a table

                "" +                // nothing for empty footnote

                "\x0007" +          // end of row, but no paragraph for empty cell
                "" +                // no paragraph break after a table

                "\r" +              // end of normal para

                "\x0007" +          // end of nested row

                "\x0007" +          // end of the outer row

                ""));

            doc = TestUtil.SaveOpen(doc, @"Model\Other\TestEnsureStoreBreakAfterTable", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("\x0007\x0007" +    // table in the header
                "" +                // we do not auto generate an extra paragraph at end of header

                "" +                // nothing for the empty footer

                // body starts here
                "" +                // nothing for the empty textbox

                "\x0007\x0007" +    // table in the textbox
                "\r" +              // auto generated end of the textbox

                "\r" +              // auto generated for empty comment.

                "\x0007\x0007" +    // table in the comment (auto generated paragraph for empty cell and end of row mark)
                "\r" +              // auto generated end of the comment

                "\r" +              // auto generated for empty footnote

                "\x0007\x0007" +    // table in the comment (auto generated paragraph for empty cell and end of row mark)
                "\r" +              // auto generated end of the footnote

                "\r" +              // end of a normal para in the document.

                "\x0007\x0007" +    // table in cell (auto generated paragraph for empty cell and end of row mark)
                "\x0007\x0007" +    // table in body (auto generated paragraph at end of cell and end of row mark)

                "\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCellHideMark(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestHideMarkTable", lf, sf);

            Table table = doc.FirstSection.Body.Tables[0];
            Cell cellWithHideMark = table.FirstRow.Cells[1];;
            Cell cellWithoutHideMark = table.FirstRow.Cells[0];

            Assert.That(cellWithHideMark.CellPr.HideMark, Is.True);
            Assert.That(cellWithoutHideMark.CellPr.HideMark, Is.False);
            Assert.That(cellWithHideMark.CellPr.Width, Is.EqualTo(1980));
            Assert.That(cellWithoutHideMark.CellPr.Width, Is.EqualTo(1980));
        }

        /// <summary>
        /// WORDSNET-2523 Support Hidden attribute for row.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira2523(LoadFormat lf, SaveFormat sf)
        {
            if (lf == LoadFormat.WordML || sf == SaveFormat.WordML)
                return;

            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestJira2523", lf, sf);

            // Verify both rows are hidden in model after export/import.
            foreach (Row row in doc.GetChildNodes(NodeType.Row, true))
                Assert.That(row.TablePr[TableAttr.Hidden], Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-2523 Support Hidden attribute for row.
        /// AM. More complex file that I used to get logic how Word set TableAttr.Hidden when loads other than DOCX formats.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHidden(LoadFormat lf, SaveFormat sf)
        {
            if (lf == LoadFormat.WordML || sf == SaveFormat.WordML)
                return;

            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestHidden", lf, sf);

            // Verify that only two first rows are hidden but last one is not.
            foreach (Row row in doc.GetChildNodes(NodeType.Row, true))
            {
                object hidden = row.TablePr[TableAttr.Hidden];

                // Only first row is hidden except DOCX is source document.
                // See TestJira13772Hidden fro details.
                if ((row.RowIndex == 0) || ((row.RowIndex == 1) && (lf == LoadFormat.Docx)))
                    Assert.That((bool) hidden, Is.True);
                else
                    Assert.That(hidden, Is.Null);
            }
        }

        /// <summary>
        /// Setting the font hidden doesn't hide Row in DOCX.
        /// Fixed, now it does. Added updating TableAttr.Hidden into DocumentValidator before saving.
        /// </summary>
        [Test]
        public void TestJira6714()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartTable();
            builder.InsertCell();
            builder.Write("Visible");
            builder.EndRow();

            builder.Font.Hidden = true;
            builder.InsertCell();
            builder.Write("Hidden");
            builder.EndRow();
            builder.EndTable();

            Document doc = TestUtil.SaveOpen(builder.Document, @"Model\Table\TestJira6714.docx");

            Row row = (Row)doc.GetChild(NodeType.Row, 1, true);
            Assert.That(row.TablePr.Hidden, Is.True);
        }


        /// <summary>
        /// WORDSNET-7369 Table Row is expanding more than the Page width in Pdf.
        /// Problem happened because when AW clones TablePr, TableAttr.Sys_Cells was shallow cloned.
        /// TableAttr.Sys_Cells is cloned deeply now.
        /// </summary>
        [Test]
        public void TestJira7369()
        {
            TablePr sourceTablePr= new TablePr();
            CellPr cellPr = new CellPr();
            cellPr.Width = 123;
            sourceTablePr.SetAttr(TableAttr.Sys_Cells, new CellPrCollection());
            sourceTablePr.CellsPr.Add(cellPr);

            TablePr cloneTablePr = sourceTablePr.Clone();
            sourceTablePr.CellsPr[0].Width = 0;
            Assert.That(cloneTablePr.CellsPr[0].Width, Is.EqualTo(123));
        }


        /// <summary>
        /// Tests that TextWrapping is get/set correctly.
        /// This is 'reverted' version of TestWrappingA to see how Word acts when TextWrapping is reverted back.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTextWrappingB(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Table\TestTextWrappingB", lf);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            table.TextWrapping = TextWrapping.None;

            table = (Table)doc.GetChild(NodeType.Table, 1, true);
            table.TextWrapping = TextWrapping.Around;

            // Test modified document.
            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestTextWrappingB Modified", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.TextWrapping, Is.EqualTo(TextWrapping.None));

            TablePr firstRowPr = table.FirstRow.TablePr;
            Assert.That(firstRowPr.Contains(TableAttr.RelativeVerticalPosition), Is.False);
            Assert.That(firstRowPr.Contains(TableAttr.FrameTop), Is.False);
            Assert.That(firstRowPr.Contains(TableAttr.FrameLeft), Is.False);
            Assert.That(firstRowPr.Contains(TableAttr.FrameDistanceFromLeft), Is.False);
            Assert.That(firstRowPr.Contains(TableAttr.FrameDistanceFromRight), Is.False);
            Assert.That(firstRowPr.Contains(TableAttr.AllowOverlap), Is.False);

            // FrameLeft does not go to LeftIndent.
            Assert.That(firstRowPr.FetchAttr(TableAttr.LeftIndent), Is.EqualTo(0));

            table = (Table)doc.GetChild(NodeType.Table, 1, true);
            Assert.That(table.TextWrapping, Is.EqualTo(TextWrapping.Around));

            firstRowPr = table.FirstRow.TablePr;
            Assert.That(firstRowPr.GetDirectAttr(TableAttr.HorizontalAlignment), Is.EqualTo(HorizontalAlignment.Right));
            Assert.That(firstRowPr.Contains(TableAttr.Alignment), Is.False);
        }

        /// <summary>
        /// Mimic MSW behavior: If there is a one-column table, where no height rule specified for v-merged cells,
        /// then we should add "AtLeast" height rule with a value, which determined according to a font size value.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestVMergeOneColumnTable(LoadFormat lf, SaveFormat sf)
        {
            // Create one-column table, without any height rule attributes.
            Document doc = new Document();
            Table table = new Table(doc);
            doc.FirstSection.Body.AppendChild(table);
            for (int i = 0; i < 3; i++)
            {
                Row row = new Row(doc);
                table.AppendChild(row);
                Cell cell = new Cell(doc);
                row.AppendChild(cell);
                Paragraph para = new Paragraph(doc);
                cell.AppendChild(para);
                cell.CellPr.VerticalMerge = (i == 0) ? CellMerge.First : CellMerge.Previous;
                para.ParagraphBreakFont.Size = 12 + i;
            }

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestVertMergeOneColumnTable", lf, sf);

            // Check, that "AtLeast" height rule with a proper value was added.
            RowCollection rows = ((Table)doc.GetChild(NodeType.Table, 0, true)).Rows;
            int[] expectedHeights = new[] { 273, 296, 319 };

            switch (sf)
            {
                case SaveFormat.Docx:
                case SaveFormat.Doc:
                case SaveFormat.WordML:
                case SaveFormat.Rtf:
                    {
                        foreach (Row checkedRow in rows)
                        {
                            Assert.That(expectedHeights[checkedRow.RowIndex], Is.EqualTo(checkedRow.TablePr.Height));
                            Assert.That(HeightRule.AtLeast, Is.EqualTo(checkedRow.TablePr.HeightRule));
                        }
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format");
            }
        }


        /// <summary>
        /// WORDSNET-4838 Vertical alignment of table cell defined using table style, is lost during converting and rendering.
        /// AW always sets vertical alignment for cell to 'top' and this overrides an alignment, defined by table style.
        /// Removed setting default cell attributes. See TestTables.UnifiedTestJira6383 for details.
        /// </summary>
        [Test]
        public void TestJira4838()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira4838.docx");

            NodeCollection tables = doc.GetChildNodes(NodeType.Table, true);

            Cell cell = ((Table)tables[0]).GetCell(0, 0);
            // This cell has direct cell formatting attribute VerticalAlignment='center' and should be set in model.
            Assert.That((CellVerticalAlignment)cell.CellPr[CellAttr.VerticalAlignment], Is.EqualTo(CellVerticalAlignment.Center));

            cell = ((Table)tables[1]).GetCell(0, 0);
            // This cell has no direct cell formatting attribute VerticalAlignment and should not be set in model.
            Assert.That(cell.CellPr[CellAttr.VerticalAlignment], Is.Null);

            cell = ((Table)tables[2]).GetCell(0, 0);
            // This cell has direct cell formatting attribute VerticalAlignment='bottom' and should be set in model.
            Assert.That((CellVerticalAlignment)cell.CellPr[CellAttr.VerticalAlignment], Is.EqualTo(CellVerticalAlignment.Bottom));
        }

        /// <summary>
        /// WORDSNET-6383 Default cell attributes is read into model.
        /// AW always treats vertical alignment 'top' for a cell as a default value and never writes it,
        /// even if it has overriding parent table style.
        /// See TestTables.UnifiedTestJira6383 for details.
        /// </summary>
        [Test]
        public void TestJira6383A()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestJira6383A.docx");
            Cell cell = (Cell)doc.GetChildNodes(NodeType.Cell, true)[3];
            CellVerticalAlignment verticalAlignment = (CellVerticalAlignment) cell.CellPr[CellAttr.VerticalAlignment];
            // Check the default cell value of vertical alignment 'top' was not lost on save.
            Assert.That(verticalAlignment, Is.EqualTo(CellVerticalAlignment.Top));
        }



        /// <summary>
        /// Relates to WORDSNET-9592
        /// Tests how borders specified in table style exposed in CellFormat.
        /// </summary>
        [Test]
        public void TestJira9592A()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira9592A.docx");

            Table table = doc.FirstSection.Body.Tables[0];

            // 1st row.
            VerifyCellFormatBorders(table.GetCell(0, 0), LineStyle.Double, LineStyle.Double, LineStyle.Double, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(1, 0), LineStyle.Double, LineStyle.Double, LineStyle.None, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(2, 0), LineStyle.Double, LineStyle.Double, LineStyle.None, LineStyle.Double);

            // 2nd row.
            VerifyCellFormatBorders(table.GetCell(0, 1), LineStyle.Dot, LineStyle.Dot, LineStyle.Dot, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(1, 1), LineStyle.Dot, LineStyle.Dot, LineStyle.None, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(2, 1), LineStyle.Dot, LineStyle.Dot, LineStyle.None, LineStyle.Dot);

            // 3rd row.
            VerifyCellFormatBorders(table.GetCell(0, 2), LineStyle.Single, LineStyle.Single, LineStyle.Single, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(1, 2), LineStyle.Single, LineStyle.Single, LineStyle.None, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(2, 2), LineStyle.Single, LineStyle.Single, LineStyle.None, LineStyle.Single);

            // 4th row.
            VerifyCellFormatBorders(table.GetCell(0, 3), LineStyle.Dot, LineStyle.Dot, LineStyle.Dot, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(1, 3), LineStyle.Dot, LineStyle.Dot, LineStyle.None, LineStyle.None);
            VerifyCellFormatBorders(table.GetCell(2, 3), LineStyle.Dot, LineStyle.Dot, LineStyle.None, LineStyle.Dot);

            // 5th row.
            VerifyCellFormatBorders(table.GetCell(0, 4), LineStyle.Triple, LineStyle.Triple, LineStyle.Triple, LineStyle.Triple);
            VerifyCellFormatBorders(table.GetCell(1, 4), LineStyle.Wave, LineStyle.Wave, LineStyle.Wave, LineStyle.Wave);
            VerifyCellFormatBorders(table.GetCell(2, 4), LineStyle.Wave, LineStyle.Wave, LineStyle.Wave, LineStyle.Wave);
        }

        /// <summary>
        /// Relates to WORDSNET-9592
        /// Borders are defined in table style table formatting and should be inherited by cell.
        /// </summary>
        [Test]
        public void TestJira9592B()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira9592B.docx");

            foreach (Cell cell in doc.GetChildNodes(NodeType.Cell, true))
            {
                Assert.That(cell.CellFormat.Borders.Top.LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders.Bottom.LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders.Left.LineStyle, Is.EqualTo(LineStyle.Single));
                Assert.That(cell.CellFormat.Borders.Right.LineStyle, Is.EqualTo(LineStyle.Single));
            }
        }

        /// <summary>
        /// Relates to WORDSNET-9592
        /// Tests how shading defined in conditional formatting is exposed in CellFormat.
        /// </summary>
        [Test]
        public void TestJira9592C()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira9592C.docx");

            // Verify that there are no cells with shading applied directly.
            foreach (Cell cell in doc.GetChildNodes(NodeType.Cell, true))
                Assert.That(cell.CellPr[CellAttr.Shading], Is.Null);

            DrColor c1 = DrColor.FromArgb(0xED, 0x7D, 0x31);
            DrColor c2 = DrColor.FromArgb(0xF7, 0xCA, 0xAC);
            DrColor c3 = DrColor.FromArgb(0xFB, 0xE4, 0xD5);

            Table table = doc.FirstSection.Body.Tables[0];

            // 1st row.
            Assert.That(table.GetCell(0, 0).CellFormat.Shading.BackgroundPatternColorInternal, Is.EqualTo(c1));
            Assert.That(table.GetCell(1, 0).CellFormat.Shading.BackgroundPatternColorInternal, Is.EqualTo(c1));

            // 2nd row.
            Assert.That(table.GetCell(0, 1).CellFormat.Shading.BackgroundPatternColorInternal, Is.EqualTo(c1));
            Assert.That(table.GetCell(1, 1).CellFormat.Shading.BackgroundPatternColorInternal, Is.EqualTo(c2));

            // 3rd row.
            Assert.That(table.GetCell(0, 2).CellFormat.Shading.BackgroundPatternColorInternal, Is.EqualTo(c1));
            Assert.That(table.GetCell(1, 2).CellFormat.Shading.BackgroundPatternColorInternal, Is.EqualTo(c3));
        }




        /// <summary>
        /// WORDSNET-11802 System.NullReferenceException during document saving to any format when table cell is merged
        /// with previous and has no child nodes. Fixed by improving <see cref="Table.SpecifyRowHeightRule"/>.
        /// </summary>
        [Test]
        public void TestJira11802()
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.StartTable();

            Cell c1 = builder.InsertCell();
            c1.CellFormat.VerticalMerge = CellMerge.First;
            builder.EndRow();

            Cell c2 = builder.InsertCell();
            c2.CellFormat.VerticalMerge = CellMerge.Previous;
            builder.EndRow();

            builder.EndTable();

            c2.GetChildNodes(NodeType.Any, false).Clear();

            // Check for no exception.
            builder.Document.Save(new MemoryStream(), SaveFormat.Docx);

            Table table = builder.Document.Sections[0].Body.Tables[0];

            // Verify the assigned values.
            Assert.That(table.FirstRow.TablePr.Height, Is.EqualTo(273));
            Assert.That(table.FirstRow.TablePr.HeightRule, Is.EqualTo(HeightRule.AtLeast));
        }


        /// <summary>
        /// WORDSNET-12114 LineStyle.None does not work for table's borders
        /// Made public Border.LineStyle create Nil border for LineStyle.None.
        /// </summary>
        /// <remarks>
        /// Word has no line style for Nil border and creates Nil border when we set wdLineStyleNone to border in VBA.
        /// Lets do the same while preserve actual line style during roundtrip.
        ///
        /// </remarks>
        [Test]
        public void TestJira12114()
        {
            // Customer use case.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();

            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;
            builder.Font.Size = 9.0;
            builder.Font.Bold = true;

            Cell cell = builder.InsertCell();
            cell.CellFormat.Borders.Top.LineStyle = LineStyle.Thick;
            cell.CellFormat.Borders.Bottom.LineStyle = LineStyle.None;

            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Thick;
            cell.CellFormat.Borders.Right.LineStyle = LineStyle.Thick;
            builder.Write("a1");
            cell = builder.InsertCell();
            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Single;
            builder.Write("b1");
            cell = builder.InsertCell();
            cell.CellFormat.Borders.Right.LineStyle = LineStyle.Thick;
            builder.Write("c1");
            builder.EndRow();

            cell = builder.InsertCell();
            cell.CellFormat.Borders.Top.LineStyle = LineStyle.None;
            cell.CellFormat.Borders.Bottom.LineStyle = LineStyle.None;

            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Thick;
            cell.CellFormat.Borders.Right.LineStyle = LineStyle.Thick;
            builder.Write("a2");
            cell = builder.InsertCell();
            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Single;
            builder.Write("b2");
            cell = builder.InsertCell();
            cell.CellFormat.Borders.Right.LineStyle = LineStyle.Thick;
            builder.Write("c2");
            builder.EndRow();

            cell = builder.InsertCell();
            cell.CellFormat.Borders.Top.LineStyle = LineStyle.None;
            cell.CellFormat.Borders.Bottom.LineStyle = LineStyle.Single;

            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Thick;
            cell.CellFormat.Borders.Right.LineStyle = LineStyle.Thick;
            builder.Write("a3");
            cell = builder.InsertCell();
            cell.CellFormat.Borders.Left.LineStyle = LineStyle.Single;
            builder.Write("b3");
            cell = builder.InsertCell();
            cell.CellFormat.Borders.Right.LineStyle = LineStyle.Thick;
            builder.Write("c3");
            builder.EndRow();

            builder.EndTable();
            // Compare gold to see that customer get desired output.
            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestJira12114.docx");

            Table table = doc.FirstSection.Body.Tables[0];
            Border border = (Border)table.FirstRow.FirstCell.CellPr[CellAttr.BorderBottom];
            Assert.That(border.IsNil, Is.True);
        }

        /// <summary>
        /// Another case for TestJira12114.
        /// </summary>
        [Test]
        public void TestJira12114A()
        {
            // Customer use case.
            Document doc = new Document();
            Table table = new Table(doc);

            for (int i = 0; i < 5; i++)
            {
                Row wordRow = new Row(doc);
                for (int j = 0; j < 5; j++)
                {
                    Cell cell = new Cell(doc);

                    //making sure LineStyle = None.
                    cell.CellFormat.Borders.LineStyle = LineStyle.None;

                    Paragraph wordsParagraph = new Paragraph(doc);
                    Run wordRun = new Run(doc);
                    wordRun.Text = Convert.ToString(j);
                    wordsParagraph.AppendChild(wordRun);
                    cell.AppendChild(wordsParagraph);
                    cell.CellFormat.Borders.ClearFormatting();
                    wordRow.AppendChild(cell);
                }

                table.Rows.Add(wordRow);
            }

            doc.FirstSection.Body.GetChildNodes(NodeType.Any, false).Add(table);

            // Compare gold to see that customer get desired output.
            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestJira12114A.docx");

            table = doc.FirstSection.Body.Tables[0];
            foreach (int key in new int[]{CellAttr.BorderTop, CellAttr.BorderBottom, CellAttr.BorderRight, CellAttr.BorderLeft})
            {
                Border border = (Border)table.FirstRow.FirstCell.CellPr[key];
                Assert.That(border.IsNil, Is.True);
            }
        }




        private static void CheckCellShading(Table table, int rowIndex, DrColor color)
        {
            Assert.That(table.Rows[rowIndex].FirstCell.CellPr.Shading.BackgroundPatternColorInternal, Is.EqualTo(color));
        }

        /// <summary>
        /// Checks that borders are exposed correctly in CellFormat.
        /// </summary>
        private static void VerifyCellFormatBorders(Cell cell, LineStyle topLineStyle, LineStyle bottomLineStyle, LineStyle leftLineStyle, LineStyle rightLineStyle)
        {
            Assert.That(cell.CellFormat.Borders.Top.LineStyle, Is.EqualTo(topLineStyle));
            Assert.That(cell.CellFormat.Borders.Bottom.LineStyle, Is.EqualTo(bottomLineStyle));
            Assert.That(cell.CellFormat.Borders.Left.LineStyle, Is.EqualTo(leftLineStyle));
            Assert.That(cell.CellFormat.Borders.Right.LineStyle, Is.EqualTo(rightLineStyle));
        }

        /// <summary>
        /// WORDSNET-12240 Model reports incorrect border width.
        /// We need to isolate adjustment of border width to DocumentBuilder only. Zero width border is valid border for MS Word.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira12240(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestJira12240", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            foreach (Table table in doc.FirstSection.Body.Tables)
            {
                Cell cell = table.FirstRow.FirstCell;
                double borderWidth = cell.CellFormat.Borders.Left.LineWidth;
                Assert.That(borderWidth, Is.EqualTo(0d), string.Format("Incorrect border width {0} for cell {1}.", borderWidth, cell));
            }
        }


        /// <summary>
        /// WORDSNET-12235 Content cutting from left edge, Table disappears from top and unwanted rows appear in PDF
        /// </summary>
        [Test]
        public void TestJira12235A()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira12235A.docx");
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(2));

            // Verify that tables are not joined during validation.
            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Relates to WORDSNET-12235
        /// Shows that we have to split tables for DOCX format the same way we did previously for DOC.
        /// </summary>
        [Test]
        public void TestJira12235B()
        {
            const string testName = @"Model\Table\TestJira12235B";

            Document doc = TestUtil.Open(testName, LoadFormat.Docx);
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(4));

            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Check that exception is thrown for DOCX format when cells count exceeds allowed limit.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira862B()
        {
            MakeDocWithCellLimit(SaveFormat.Docx);
        }

        /// <summary>
        /// Allow table to have more than 63 cell per row for other formats.
        /// </summary>
        [Test]
        public void TestJira862D()
        {
            MakeDocWithCellLimit(SaveFormat.Markdown);
        }


        /// <summary>
        /// Creates document with table which has more than 63 cells in a row.
        /// </summary>
        private static void MakeDocWithCellLimit(SaveFormat sf)
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            builder.CurrentSection.PageSetup.Orientation = Orientation.Landscape;
            builder.StartTable();

            for (int i = 0; i < 64; i++)
            {
                builder.InsertCell();
                builder.Font.Size = 2;
                builder.Write(string.Format("{0}", i));
            }

            builder.EndTable();
            builder.Document.Save(new MemoryStream(), sf);
        }



        /// <summary>
        /// WORDSNET-14411 Cell revisions is written incorrectly.
        /// Skip width validation for table with cell revisions.
        /// </summary>
        [Test]
        public void TestJira14411()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\TestJira14411", UnifiedScenario.Docx2DocxNoGold);

            // Check original and final table grids are preserved.
            Table table = doc.FirstSection.Body.Tables[0];

            IntList tableGridOriginal = (IntList)table.TablePr[TableAttr.Sys_TableGrid];
            ArrayUtil.CheckArraysEqual(tableGridOriginal.ToArray(), new int[] { 4952, 1609, 3344 });

            IntList tableGridFinal = (IntList)table.TablePr.FormatRevision.RevPr[TableAttr.Sys_TableGrid];
            ArrayUtil.CheckArraysEqual(tableGridFinal.ToArray(), new int[] { 6561, 3344 });
        }

        /// <summary>
        /// WORDSNET-14501 Paragraph alignment in cells in a TFG data table is incorrect
        /// Alignment is defined in table style conditional formatting. Attribute fetcher is improved.
        /// </summary>
        [Test]
        public void TestJira14501()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira14501.docx");
            Paragraph p = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.FirstParagraph;
            Assert.That(p.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Left));
        }


        /// <summary>
        /// WORDSNET-5890 Tests reading/writing table title and description.
        /// </summary>
        [Test]
        public void TestReadingWritingTitle()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            TableBuilder tableBuilder = new TableBuilder(builder, false);

            Table table = tableBuilder.StartTable();
            tableBuilder.StartRow();
            tableBuilder.StartCell();
            builder.Writeln("Test");
            tableBuilder.EndTable();

            table.Title = "Test Title";
            table.Description = "Test description";

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestTitle.docx", null, false);
            table = doc.FirstSection.Body.Tables[0];

            Assert.That(table.Title, Is.EqualTo("Test Title"));
            Assert.That(table.Description, Is.EqualTo("Test description"));
        }


        private static void UnifiedTestVerticalPosition(string path, LoadFormat lf, SaveFormat sf)
        {
            UnifiedScenario scenario = TestUtil.BuildScenario(lf, sf, (sf != SaveFormat.Odt));
            Document doc = TestUtil.OpenSaveOpen(path, scenario);
            TablePr tblPr = doc.FirstSection.Body.Tables[2].FirstRow.TablePr;

            if (sf != SaveFormat.Odt)
            {
                Assert.That((RelativeVerticalPosition)tblPr[TableAttr.RelativeVerticalPosition], Is.EqualTo(RelativeVerticalPosition.Paragraph));
                Assert.That(tblPr[TableAttr.VerticalAlignment], Is.EqualTo(VerticalAlignment.Outside));
                if (sf == SaveFormat.Doc)
                {
                    // Reader reads "-1" in presence of alignment for some reason. See ReadFrameTop().
                    // Leave it be for now.
                    Assert.That(tblPr[TableAttr.FrameTop], Is.EqualTo(-1));
                    // However, it will be removed by TableValidator.
                }
                else
                {
                    // MS Word behavior: in presence of alignment, position attribute is removed. (WORDSNET-25085).
                    Assert.That(tblPr.Contains(TableAttr.FrameTop), Is.False);
                }
            }
            else
            {
                // "ODT" does not write "TableAttr.RelativeVerticalPosition" and "TableAttr.FrameTop" attributes.
                Assert.That(tblPr.Contains(TableAttr.RelativeVerticalPosition), Is.False);
                Assert.That(tblPr.Contains(TableAttr.FrameTop), Is.False);
            }
        }



        /// <summary>
        /// WORDSNET-18162 Incorrect table style (with conditional formatting) was applied
        /// for the second row. We must apply a conditional style with TableStyleOverrideType.FirstRow for the heading rows.
        /// </summary>
        /// <remarks>
        /// Check <see cref="Row.IsHeadingRow"/> for details.
        /// </remarks>
        [Test]
        public void TestJira18162()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestJira18162.docx");

            // Verify requested by customer import of Font.AllCaps.
            Run run = doc.FirstSection.Body.Tables[0].Rows[1].FirstCell.FirstParagraph.FirstRun;
            Assert.That(run.Font.AllCaps, Is.EqualTo(true));
        }


        /// <summary>
        /// WORDSNET-12204 Add feature to set/get positioning of floating table
        /// Added public setters for AbsoluteHorizontalDistance, AbsoluteVerticalDistance,
        /// RelativeHorizontalAlignment and RelativeVerticalAlignment.
        /// </summary>
        [Test]
        public void Test12204()
        {
            const string testFile = @"Model\Table\Test12204.docx";

            Document doc = TestUtil.Open(testFile);

            // Move first table to the position of second table.
            Table table0 = doc.FirstSection.Body.Tables[0];

            Assert.That(table0.AbsoluteHorizontalDistance, Is.EqualTo(209.5).Within(double.Epsilon));
            table0.RelativeHorizontalAlignment = HorizontalAlignment.Center;
            Assert.That(table0.FirstRow.TablePr[TableAttr.FrameLeft], Is.Null);

            Assert.That(table0.AbsoluteVerticalDistance, Is.EqualTo(81.65).Within(double.Epsilon));
            table0.RelativeVerticalAlignment = VerticalAlignment.Center;
            Assert.That(table0.FirstRow.TablePr[TableAttr.FrameTop], Is.Null);

            // Move second table to the position of first table.
            Table table1 = doc.FirstSection.Body.Tables[1];
            Assert.That(table1.RelativeHorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center));
            table1.AbsoluteHorizontalDistance = 209.5;
            Assert.That(table1.FirstRow.TablePr[TableAttr.HorizontalAlignment], Is.Null);

            Assert.That(table1.RelativeVerticalAlignment, Is.EqualTo(VerticalAlignment.Center));
            table1.AbsoluteVerticalDistance = 81.65;
            Assert.That(table1.FirstRow.TablePr[TableAttr.VerticalAlignment], Is.Null);

            TestUtil.Save(doc, testFile, SaveOptions.CreateSaveOptions(SaveFormat.Docx), true);
        }

        /// <summary>
        /// WORDSNET-19873 Tests vertical anchor allowed values.
        /// </summary>
        [TestCase(RelativeVerticalPosition.Margin, true)]
        [TestCase(RelativeVerticalPosition.Page, true)]
        [TestCase(RelativeVerticalPosition.Paragraph, true)]
        [TestCase(RelativeVerticalPosition.Line, false)]
        [TestCase(RelativeVerticalPosition.BottomMargin, false)]
        [TestCase(RelativeVerticalPosition.InsideMargin, false)]
        [TestCase(RelativeVerticalPosition.OutsideMargin, false)]
        [TestCase(RelativeVerticalPosition.TopMargin, false)]
        public void Test19873VerticalArgument(RelativeVerticalPosition vAnchor, bool allowed)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test19873Inline.docx");
            Table table = doc.FirstSection.Body.Tables[0];

            bool thrown = false;
            try
            {
                table.VerticalAnchor = vAnchor;
            }
            catch (ArgumentException)
            {
                thrown = true;
            }

            Assert.That(!thrown, Is.EqualTo(allowed));
        }

        /// <summary>
        /// WORDSNET-19873 Tests horizontal anchor allowed values.
        /// </summary>
        [TestCase(RelativeHorizontalPosition.Margin, true)]
        [TestCase(RelativeHorizontalPosition.Page, true)]
        [TestCase(RelativeHorizontalPosition.Column, true)]
        [TestCase(RelativeHorizontalPosition.Character, false)]
        [TestCase(RelativeHorizontalPosition.LeftMargin, false)]
        [TestCase(RelativeHorizontalPosition.InsideMargin, false)]
        [TestCase(RelativeHorizontalPosition.OutsideMargin, false)]
        [TestCase(RelativeHorizontalPosition.RightMargin, false)]
        public void Test19873HorizontalArgument(RelativeHorizontalPosition hAnchor, bool allowed)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test19873Inline.docx");
            Table table = doc.FirstSection.Body.Tables[0];

            bool thrown = false;
            try
            {
                table.HorizontalAnchor = hAnchor;
            }
            catch (ArgumentException)
            {
                thrown = true;
            }

            Assert.That(!thrown, Is.EqualTo(allowed));
        }

        /// <summary>
        /// WORDSNET-19873 Tests vertical anchor applied to inline table.
        /// </summary>
        [TestCase(RelativeVerticalPosition.Margin, @"Model\Table\Test19873InlineVerticalTableDefault.docx")]
        [TestCase(RelativeVerticalPosition.Page, @"Model\Table\Test19873InlineVerticalPage.docx")]
        [TestCase(RelativeVerticalPosition.Paragraph, @"Model\Table\Test19873InlineVerticalTextFrameDefault.docx")]
        public void Test19873VerticalResult(RelativeVerticalPosition vAnchor, string outputFile)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test19873Inline.docx");
            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.IsFloating, Is.False);

            table.VerticalAnchor = vAnchor;

            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.VerticalAnchor, Is.EqualTo(vAnchor));

            // Low level check that frame top is explicitly set.
            TablePr tablePr = table.Rows[0].TablePr;
            Assert.That(tablePr[TableAttr.FrameTop], Is.EqualTo(0));
            Assert.That(tablePr[TableAttr.FrameDistanceFromLeft], Is.EqualTo(180));
            Assert.That(tablePr[TableAttr.FrameDistanceFromRight], Is.EqualTo(180));

            TestUtil.SaveOpen(doc, outputFile);
        }

        /// <summary>
        /// WORDSNET-19873 Tests horizontal anchor applied to inline table.
        /// </summary>
        [TestCase(RelativeHorizontalPosition.Margin, @"Model\Table\Test19873InlineHorizontalMargin.docx")]
        [TestCase(RelativeHorizontalPosition.Page, @"Model\Table\Test19873InlineHorizontalPage.docx")]
        [TestCase(RelativeHorizontalPosition.Column, @"Model\Table\Test19873InlineHorizontalColumn.docx")]
        public void Test19873HorizontalResult(RelativeHorizontalPosition hAnchor, string outputFile)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test19873Inline.docx");
            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.IsFloating, Is.False);

            table.HorizontalAnchor = hAnchor;

            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.HorizontalAnchor, Is.EqualTo(hAnchor));

            // Low level check that frame top is explicitly set.
            // It is interesting that Word sets vertical position to force floating when horizontal anchor set.
            TablePr tablePr = table.Rows[0].TablePr;
            Assert.That(tablePr[TableAttr.FrameTop], Is.EqualTo(0));
            Assert.That(tablePr[TableAttr.RelativeVerticalPosition], Is.EqualTo(RelativeVerticalPosition.Paragraph));
            Assert.That(tablePr[TableAttr.FrameDistanceFromLeft], Is.EqualTo(180));
            Assert.That(tablePr[TableAttr.FrameDistanceFromRight], Is.EqualTo(180));

            TestUtil.SaveOpen(doc, outputFile);
        }

        [TestCase(RelativeVerticalPosition.Margin)]
        [TestCase(RelativeVerticalPosition.Page)]
        [TestCase(RelativeVerticalPosition.Paragraph)]
        public void Test19873VerticalFloating(RelativeVerticalPosition vAnchor)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test19873Floating.docx");

            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.AbsoluteVerticalDistance, Is.EqualTo(50));
            Assert.That(table.AbsoluteHorizontalDistance, Is.EqualTo(20));

            table.VerticalAnchor = vAnchor;

            // All floating attributes except VerticalAnchor are preserved.
            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.VerticalAnchor, Is.EqualTo(vAnchor));
            Assert.That(table.AbsoluteVerticalDistance, Is.EqualTo(50));
            Assert.That(table.AbsoluteHorizontalDistance, Is.EqualTo(20));
        }

        /// <summary>
        /// WORDSNET-19873 Tests horizontal anchor applied to floating table.
        /// </summary>
        [TestCase(RelativeHorizontalPosition.Margin)]
        [TestCase(RelativeHorizontalPosition.Page)]
        [TestCase(RelativeHorizontalPosition.Column)]
        public void Test19873HorizontalFloating(RelativeHorizontalPosition hAnchor)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test19873Floating.docx");

            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.AbsoluteVerticalDistance, Is.EqualTo(50));
            Assert.That(table.AbsoluteHorizontalDistance, Is.EqualTo(20));

            table.HorizontalAnchor = hAnchor;

            // All floating attributes except HorizontalAnchor are preserved.
            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.HorizontalAnchor, Is.EqualTo(hAnchor));
            Assert.That(table.AbsoluteVerticalDistance, Is.EqualTo(50));
            Assert.That(table.AbsoluteHorizontalDistance, Is.EqualTo(20));
        }





        /// <summary>
        /// WORDSNET-22097 <see cref="Table.ConvertToHorizontallyMergedCells"/> threw an
        /// <see cref="IndexOutOfRangeException"/> for a table created with <see cref="DocumentBuilder"/>.
        /// Now <see cref="Table.ConvertToHorizontallyMergedCells"/> does nothing for tables with zero width
        /// of cells.
        /// </summary>
        [Test]
        public void Test22097()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();

            builder.InsertCell();
            builder.Write("This is row 1 cell 1");

            builder.InsertCell();
            builder.Write("This is row 1 cell 2");

            builder.EndRow();
            builder.EndTable();

            // IndexOutOfRangeException was thrown on this line.
            table.ConvertToHorizontallyMergedCells();

            Assert.That(table.Rows.Count, Is.EqualTo(1));
            Assert.That(table.FirstRow.Cells.Count, Is.EqualTo(2));
        }


        /// <summary>
        /// WORDSNET-17523 How to determine maximum value for table border
        /// Added descriptive exception message.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException),
#if NETSTANDARD
        ExpectedMessage = "Must be non-negative and less than or equal to 31. (Parameter 'lineWidth')")]
#else
        ExpectedMessage = "Must be non-negative and less than or equal to 31.\r\nParameter name: lineWidth")]
#endif
        public void Test17523()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Writeln("test");
            builder.EndRow();
            builder.EndTable();

            table.SetBorder(BorderType.Bottom, LineStyle.Single, 32, Color.Red, false);
        }


        /// <summary>
        /// WORDSNET-25164 Wk: Floating table margins setters
        /// Implemented an ability to set DistanceXXX properties of a <see cref="Table"/>. If a table is not floating,
        /// it becomes floating after setting the properties.
        /// </summary>
        [Test]
        public void Test25164()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Generate text.
            for (int i = 0; i < 20; i++)
                builder.Write("Text1 ");

            builder.Writeln();

            // Insert a table.
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.Write("Cell1");
            builder.InsertCell();
            builder.Write("Cell2");
            builder.EndRow();
            builder.InsertCell();
            builder.Write("Cell3");
            builder.InsertCell();
            builder.Write("Cell4");
            builder.EndRow();
            builder.EndTable();

            // Generate text.
            for (int i = 0; i < 20; i++)
                builder.Write("Text2 ");

            builder.Writeln();

            table.AutoFit(AutoFitBehavior.AutoFitToContents);
            table.Alignment = TableAlignment.Center;

            Assert.That(table.IsFloating, Is.False);

            // Set table distance from text.
            table.DistanceLeft = 1;
            table.DistanceRight = 2;
            table.DistanceTop = 12;
            table.DistanceBottom = 14;

            Assert.That(table.IsFloating, Is.True);

            doc = TestUtil.SaveOpen(doc, @"Model\Table\Test25164", UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);

            table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.IsFloating, Is.True);
            Assert.That(table.DistanceLeft, Is.EqualTo(1.0).Within(0.01));
            Assert.That(table.DistanceRight, Is.EqualTo(2.0).Within(0.01));
            Assert.That(table.DistanceTop, Is.EqualTo(12.0).Within(0.01));
            Assert.That(table.DistanceBottom, Is.EqualTo(14.0).Within(0.01));
        }


        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestNegativeDistance()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert a table.
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.EndRow();

            // Set wrong distance.
            table.DistanceLeft = -0.5;
        }

        /// <summary>
        /// WORDSNET-25297 (fixed) - Provide public access to CellPr.HideMark property.
        /// Added new public property CellFormat.HideMark.
        /// </summary>
        [Test]
        public void Test25297()
        {
            Document doc = TestUtil.Open(@"Model\Table\TestHideMarkTable", LoadFormat.Docx);

            Cell cell = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell;
            Assert.That(cell.CellFormat.HideMark, Is.False);

            cell.CellFormat.HideMark = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Table\TestHideMarkTable", UnifiedScenario.Docx2DocxNoGold);
            cell = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell;
            Assert.That(cell.CellFormat.HideMark, Is.True);
        }


        private static void TestHtmlBlock18599(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\Test18599", lf, sf);

            int htmlBlockId = doc.FirstSection.Body.Tables[0].Rows[0].TablePr.HtmlBlockId;
            Assert.That(htmlBlockId, Is.EqualTo(sf != SaveFormat.Rtf ? 1501000681 : 2));
            Assert.That(doc.HtmlBlockCollection.Count, Is.EqualTo(2));
            Assert.That(doc.HtmlBlockCollection.GetHtmlBlockById(htmlBlockId).ParaPr[ParaAttr.HtmlMarginLeft], Is.EqualTo(-450));
        }


        /// <summary>
        /// WORDSNET-23404 (draft) - Read and write table row widthAfter and widthBefore specified in percent units
        /// Added roundtrip for WidthBefore/WidthAfter specified in percents.
        /// </summary>
        [Test]
        public void Test23404()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\Test23404", UnifiedScenario.Docx2Docx);

            Table table = doc.FirstSection.Body.Tables[0];
            PreferredWidth widthAfter = (PreferredWidth)table.Rows[1].TablePr[TableAttr.WidthAfter];

            Assert.That(widthAfter.IsPercent, Is.True);
            Assert.That(widthAfter.Value, Is.EqualTo(55.56).Within(0.001));
        }



        /// <summary>
        /// WORDSNET-23670 Table column width is changed after open/save document.
        /// Added roundtrip for WidthBefore/WidthAfter specified in percents.
        /// </summary>
        [Test]
        public void Test23670()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Table\Test23670", UnifiedScenario.Docx2DocxNoGold);

            Table table = doc.FirstSection.Body.Tables[0];
            PreferredWidth widthAfter = (PreferredWidth)table.Rows[0].TablePr[TableAttr.WidthAfter];
            Assert.That(widthAfter.IsPercent, Is.True);
            Assert.That(widthAfter.Value, Is.EqualTo(0.94).Within(0.001));

            PreferredWidth widthBefore = (PreferredWidth)table.Rows[1].TablePr[TableAttr.WidthBefore];
            Assert.That(widthBefore.IsPercent, Is.True);
            Assert.That(widthBefore.Value, Is.EqualTo(0.12).Within(0.001));
        }



        /// <summary>
        /// WORDSNET-25899 Add support to hide or unhide row in the table.
        /// Tests hide row and check if the attribute are preserved after roundtrip.
        /// </summary>
        [TestCase(LoadFormat.Docx, SaveFormat.Docx)]
        public void Test25899HideRow(LoadFormat loadFormat, SaveFormat saveFormat)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test25899", loadFormat);

            Row row = doc.FirstSection.Body.Tables[0].Rows[3];
            Assert.That(row.Hidden, Is.False);
            row.Hidden = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Table\Test25899HideRow", TestUtil.GetUnifiedScenario(loadFormat, saveFormat));

            row = doc.FirstSection.Body.Tables[0].Rows[3];
            Assert.That(row.Hidden, Is.True);

            foreach (Cell cell in row.Cells)
            {
                foreach (Paragraph para in cell.Paragraphs)
                {
                    foreach (Run run in para.Runs)
                        Assert.That(run.Font.Hidden, Is.True);
                }
            }
        }

        /// <summary>
        /// Relates to WORDSNET-25899.
        /// Tests unhide row and check if the attribute are preserved after roundtrip.
        /// </summary>
        [TestCase(LoadFormat.Docx, SaveFormat.Docx)]
        public void Test25899UnhideRow(LoadFormat loadFormat, SaveFormat saveFormat)
        {
            Document doc = TestUtil.Open(@"Model\Table\Test25899HiddenRow", loadFormat);

            Row row = doc.FirstSection.Body.Tables[0].Rows[3];
            Assert.That(row.Hidden, Is.True);
            row.Hidden = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Table\Test25899UnhideRow", TestUtil.GetUnifiedScenario(loadFormat, saveFormat));

            row = doc.FirstSection.Body.Tables[0].Rows[3];
            Assert.That(row.Hidden, Is.False);

            foreach (Cell cell in row.Cells)
            {
                foreach (Paragraph para in cell.Paragraphs)
                {
                    foreach (Run run in para.Runs)
                        Assert.That(run.Font.Hidden, Is.False);
                }
            }
        }



        private static Table Create1x1Table(Document doc)
        {
            Table table = new Table(doc);

            Row row = new Row(doc);
            table.AppendChild(row);

            Cell cell = new Cell(doc);
            row.AppendChild(cell);

            return table;
        }
    }
}
