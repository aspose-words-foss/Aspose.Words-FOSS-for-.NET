// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2009 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for table styles.
    /// At the moment table styles are supported only in DOCX and WML and expanded to direct formatting when saving to other formats.
    /// When table styles are supported in DOC and RTF then most of the tests here should be reworked into unified tests.
    ///
    /// RK Some of these tests now should be reworked to unified.
    /// </summary>
    [TestFixture]
    public class TestStyleTable
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Check how built-in table styles are loaded and saved in DOCX.
        /// </summary>
        [Test]
        public void TestBuiltInTableStylesDOCX()
        {
            TestUtil.OpenSaveOpen(@"Model\Style\Table\TestBuiltInTableStyles.docx");
        }

        [Test]
        public void TestBuiltInTableStylesDOC()
        {
            Document doc = TestUtil.Open(@"Model\Style\Table\TestBuiltInTableStyles.docx");
            TestUtil.Save(doc, @"Model\Style\Table\TestBuiltInTableStyles.docx");
        }

        [Test]
        public void TestWideBandsDOC()
        {
            Document doc = TestUtil.Open(@"Model\Style\Table\TestWideBands.docx");
            TestUtil.Save(doc, @"Model\Style\Table\TestWideBands.docx");
        }




        /// <summary>
        /// WORDSNET-18400 Tests public properties of a table style.
        /// </summary>
        [Test]
        public void TestTableStyleProperties()
        {
            const string fileName = @"Model\Style\Table\TestTableStyleProperties.docx";
            Document doc = TestUtil.Open(fileName);
            TableStyle style = (TableStyle)doc.Styles["TableStyle1"];

            Assert.That(style.AllowBreakAcrossPages, Is.False);
            Border border = style.Borders.Top;
            Assert.That(border.LineWidth, Is.EqualTo(1.5));
            Assert.That(border.Color, Is.EqualTo(Color.FromArgb(142, 170, 219)));
            Assert.That(style.TopPadding, Is.EqualTo(36));
            Assert.That(style.Alignment, Is.EqualTo(TableAlignment.Center));
            Assert.That(style.TablePr.Alignment, Is.EqualTo(TableAlignment.Center));
            Assert.That(style.RowPr.Alignment, Is.EqualTo(TableAlignment.Center));
            Assert.That(style.CellSpacing, Is.EqualTo(3));
            Assert.That(style.TablePr.CellSpacing, Is.EqualTo(PreferredWidth.FromTwipsSafe(60)));
            Assert.That(style.RowPr.CellSpacing, Is.EqualTo(PreferredWidth.FromTwipsSafe(60)));
            Assert.That(style.LeftIndent, Is.EqualTo(18));
            Assert.That(style.Shading.Texture, Is.EqualTo(TextureIndex.Texture10Percent));
            Assert.That(style.RowStripe, Is.EqualTo(2));
            Assert.That(style.ColumnStripe, Is.EqualTo(3));

            style.AllowBreakAcrossPages = true;
            border.LineWidth = 1;
            border.Color = Color.Red;
            style.TopPadding = 12;
            style.Alignment = TableAlignment.Right;
            Assert.That(style.TablePr.Alignment, Is.EqualTo(TableAlignment.Right));
            Assert.That(style.RowPr.Alignment, Is.EqualTo(TableAlignment.Right));
            style.CellSpacing = 1;
            Assert.That(style.TablePr.CellSpacing, Is.EqualTo(PreferredWidth.FromTwipsSafe(20)));
            Assert.That(style.RowPr.CellSpacing, Is.EqualTo(PreferredWidth.FromTwipsSafe(20)));
            style.LeftIndent = 9;
            style.Shading.Texture = TextureIndex.Texture20Percent;
            style.RowStripe = 1;
            style.ColumnStripe = 2;

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            style = (TableStyle)doc.Styles["TableStyle1"];

            Assert.That(style.AllowBreakAcrossPages, Is.True);
            border = style.Borders.Top;
            Assert.That(border.LineWidth, Is.EqualTo(1));
            Assert.That(border.Color, Is.EqualTo(Color.FromArgb(255, 0, 0)));
            Assert.That(style.TopPadding, Is.EqualTo(12));
            Assert.That(style.Alignment, Is.EqualTo(TableAlignment.Right));
            Assert.That(style.TablePr.Alignment, Is.EqualTo(TableAlignment.Right));
            Assert.That(style.RowPr.Alignment, Is.EqualTo(TableAlignment.Right));
            Assert.That(style.CellSpacing, Is.EqualTo(1));
            Assert.That(style.TablePr.CellSpacing, Is.EqualTo(PreferredWidth.FromTwipsSafe(20)));
            Assert.That(style.RowPr.CellSpacing, Is.EqualTo(PreferredWidth.FromTwipsSafe(20)));
            Assert.That(style.LeftIndent, Is.EqualTo(9));
            Assert.That(style.Shading.Texture, Is.EqualTo(TextureIndex.Texture20Percent));
            Assert.That(style.RowStripe, Is.EqualTo(1));
            Assert.That(style.ColumnStripe, Is.EqualTo(2));
        }

        /// <summary>
        /// WORDSNET-18400 Tests public properties of a table style that values are correctly taken from
        /// a base style.
        /// </summary>
        [Test]
        public void TestTableStyleFormattingFromBaseStyle()
        {
            const string fileName = @"Model\Style\Table\TestTableStyleProperties.docx";
            Document doc = TestUtil.Open(fileName);
            TableStyle style = (TableStyle)doc.Styles["InheritedFromTableStyle1"];
            style.CellPr.Clear(); // to remove shading definition that MS Word copies from a base style

            Assert.That(style.AllowBreakAcrossPages, Is.False);
            Border border = style.Borders.Top;
            Assert.That(border.LineWidth, Is.EqualTo(1.5));
            Assert.That(border.Color, Is.EqualTo(Color.FromArgb(142, 170, 219)));
            Assert.That(style.TopPadding, Is.EqualTo(36));
            Assert.That(style.Alignment, Is.EqualTo(TableAlignment.Center));
            Assert.That(style.CellSpacing, Is.EqualTo(3));
            Assert.That(style.LeftIndent, Is.EqualTo(18));
            Assert.That(style.Shading.Texture, Is.EqualTo(TextureIndex.Texture10Percent));
            Assert.That(style.RowStripe, Is.EqualTo(2));
            Assert.That(style.ColumnStripe, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-18400 Tests public properties of a table style.
        /// </summary>
        [Test]
        public void TestTableStyleCreation()
        {
            Document doc = new Document();
            TableStyle style1 = (TableStyle)doc.Styles.Add(StyleType.Table, "TableStyle1");
            TableStyle style2 = (TableStyle)doc.Styles.AddCopy(doc.Styles[StyleIdentifier.GridTable6ColorfulAccent3]);
            style2.Name = "TableStyle2";

            // Check default value of the property.
            Assert.That(style1.AllowBreakAcrossPages, Is.EqualTo(true));

            style1.Font.Color = Color.FromArgb(128, 0, 128);
            style1.ParagraphFormat.FirstLineIndent = 54;
            style1.AllowBreakAcrossPages = false;
            style1.RightPadding = 36;
            style2.Alignment = TableAlignment.Right;
            style2.LeftIndent = 24;
            style2.ConditionalStyles.FirstColumn.Font.Bold = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Style\Table\TestTableStyleCreation.docx",
                UnifiedScenario.Docx2DocxNoGold);
            style1 = (TableStyle)doc.Styles["TableStyle1"];
            style2 = (TableStyle)doc.Styles["TableStyle2"];

            Assert.That(style1.Font.Color, Is.EqualTo(Color.FromArgb(128, 0, 128)));
            Assert.That(style1.ParagraphFormat.FirstLineIndent, Is.EqualTo(54));
            Assert.That(style1.AllowBreakAcrossPages, Is.False);
            Assert.That(style1.RightPadding, Is.EqualTo(36));
            Assert.That(style2.Alignment, Is.EqualTo(TableAlignment.Right));
            Assert.That(style2.LeftIndent, Is.EqualTo(24));
            Assert.That(style2.ConditionalStyles.FirstColumn.Font.Bold, Is.True);
        }

        /// <summary>
        /// WORDSNET-18400 Tests public properties of a table conditional style.
        /// </summary>
        [Test]
        public void TestTableConditionalStyle()
        {
            const string fileName = @"Model\Style\Table\TestTableStyleProperties.docx";
            Document doc = TestUtil.Open(fileName);
            TableStyle style = (TableStyle)doc.Styles["TableStyle2"];
            Assert.That(style.Shading.Texture, Is.EqualTo(TextureIndex.Texture5Percent));
            Assert.That(style.Borders.Bottom.LineWidth, Is.EqualTo(0.5));
            Assert.That(style.Font.Name, Is.EqualTo("Calibri"));

            ConditionalStyleCollection conditionals = style.ConditionalStyles;
            ConditionalStyle conditional = conditionals.FirstRow;

            Assert.That(conditional.Type, Is.EqualTo(ConditionalStyleType.FirstRow));
            Assert.That(conditional.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(conditional.Font.Name, Is.EqualTo("Aharoni"));
            Assert.That(conditional.Font.Size, Is.EqualTo(24));
            Border border = conditional.Borders.Top;
            Assert.That(border.LineWidth, Is.EqualTo(1.5)); // from conditional style
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.Double));
            Assert.That(conditional.Shading.Texture, Is.EqualTo(TextureIndex.Texture10Percent));
            Assert.That(conditional.TopPadding, Is.EqualTo(18));
            Assert.That(conditional.LeftPadding, Is.EqualTo(4));
            Assert.That(conditional.RightPadding, Is.EqualTo(2));
            Assert.That(conditional.BottomPadding, Is.EqualTo(5));

            conditional = conditionals.LastRow;
            Assert.That(conditional.Shading.Texture, Is.EqualTo(TextureIndex.Texture5Percent)); // from table style
            Assert.That(conditional.Font.Name, Is.EqualTo("Calibri")); // from table style
            Assert.That(conditional.Font.Size, Is.EqualTo(12));
            Assert.That(conditional.Borders.Bottom.LineStyle, Is.EqualTo(LineStyle.None)); // borders are not taken from table style

            Assert.That(conditionals.FirstColumn.Font.Size, Is.EqualTo(13));
            Assert.That(conditionals.LastColumn.Font.Size, Is.EqualTo(14));
            Assert.That(conditionals.OddRowBanding.Font.Size, Is.EqualTo(15));
            Assert.That(conditionals.EvenRowBanding.Font.Size, Is.EqualTo(16));
            Assert.That(conditionals.OddColumnBanding.Font.Size, Is.EqualTo(17));
            Assert.That(conditionals.EvenColumnBanding.Font.Size, Is.EqualTo(18));
            Assert.That(conditionals.TopLeftCell.Font.Size, Is.EqualTo(19));
            Assert.That(conditionals.TopRightCell.Font.Size, Is.EqualTo(20));
            Assert.That(conditionals.BottomLeftCell.Font.Size, Is.EqualTo(21));
            Assert.That(conditionals.BottomRightCell.Font.Size, Is.EqualTo(22));

            Assert.That(conditionals[0].Type, Is.EqualTo(ConditionalStyleType.FirstRow));

            conditional = conditionals.FirstRow;
            conditional.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            conditional.Font.Name = "Arial New";
            conditional.Font.Size = 10;
            border = conditional.Borders.Top;
            border.LineWidth = 2;
            conditional.Shading.Texture = TextureIndex.Texture20Percent;
            conditional.TopPadding = 8;

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            style = (TableStyle)doc.Styles["TableStyle2"];

            conditional = style.ConditionalStyles.FirstRow;

            Assert.That(conditional.Type, Is.EqualTo(ConditionalStyleType.FirstRow));
            Assert.That(conditional.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(conditional.Font.Name, Is.EqualTo("Arial New"));
            Assert.That(conditional.Font.Size, Is.EqualTo(10));
            border = conditional.Borders.Top;
            Assert.That(border.LineWidth, Is.EqualTo(2));
            Assert.That(conditional.Shading.Texture, Is.EqualTo(TextureIndex.Texture20Percent));
            Assert.That(conditional.TopPadding, Is.EqualTo(8));
        }

        /// <summary>
        /// WORDSNET-18400 Tests public properties of a table conditional style that values are correctly taken
        /// from a base conditional style.
        /// </summary>
        [Test]
        public void TestConditionalFormattingFromBaseConditional()
        {
            // The test document has generated manually since MS Word copies formatting from base conditional styles to
            // inherited ones.
            const string fileName = @"Model\Style\Table\TestConditionalFormattingFromBaseConditional.docx";
            Document doc = TestUtil.Open(fileName);
            TableStyle style = (TableStyle)doc.Styles["InheritedStyle1"];
            ConditionalStyle conditional = style.ConditionalStyles.FirstRow;

            Assert.That(conditional.Font.Name, Is.EqualTo("Aharoni"));
            Assert.That(conditional.Font.Size, Is.EqualTo(14));
            Assert.That(conditional.Font.Bold, Is.True);
            Assert.That(conditional.Font.Italic, Is.False);

            Assert.That(conditional.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(conditional.ParagraphFormat.LineSpacingInLines, Is.EqualTo(2));

            Assert.That(conditional.Shading.Texture, Is.EqualTo(TextureIndex.Texture30Percent));
            Assert.That(conditional.Shading.ForegroundPatternColor, Is.EqualTo(Color.FromArgb(255, 255, 0)));
            Assert.That(conditional.Shading.BackgroundPatternColor, Is.EqualTo(Color.FromArgb(128, 128, 255)));

            Border border = conditional.Borders.Top;
            Assert.That(border.LineWidth, Is.EqualTo(0.5));
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.Triple));
            border = conditional.Borders.Right;
            Assert.That(border.LineWidth, Is.EqualTo(0.5));
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.Triple));
            border = conditional.Borders.Bottom;
            Assert.That(border.LineWidth, Is.EqualTo(0.5));
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.Triple));
            border = conditional.Borders.Left;
            Assert.That(border.LineWidth, Is.EqualTo(0.5));
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.Triple));

            // Border is not taken from a parent table style.
            border = style.ConditionalStyles.LastRow.Borders.Top;
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.None));

            Assert.That(conditional.TopPadding, Is.EqualTo(4.5));
            Assert.That(conditional.LeftPadding, Is.EqualTo(1.5));
            Assert.That(conditional.RightPadding, Is.EqualTo(6));
            Assert.That(conditional.BottomPadding, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-18400 Tests public properties of a table conditional style that values are correctly taken
        /// from a table style.
        /// </summary>
        [Test]
        public void TestConditionalFormattingFromTableStyle()
        {
            // The test document has generated manually since MS Word copies formatting from base conditional styles to
            // inherited ones.
            const string fileName = @"Model\Style\Table\TestConditionalFormattingFromTableStyle.docx";
            Document doc = TestUtil.Open(fileName);
            TableStyle style = (TableStyle)doc.Styles["InheritedStyle1"];
            ConditionalStyle conditional = style.ConditionalStyles.FirstRow;

            Assert.That(conditional.Font.Name, Is.EqualTo("Aharoni"));
            Assert.That(conditional.Font.Size, Is.EqualTo(11));
            Assert.That(conditional.Font.Bold, Is.True);
            Assert.That(conditional.Font.Italic, Is.False);

            Assert.That(conditional.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(conditional.ParagraphFormat.LineSpacingInLines, Is.EqualTo(2));

            Assert.That(conditional.Shading.Texture, Is.EqualTo(TextureIndex.Texture30Percent));
            Assert.That(conditional.Shading.ForegroundPatternColor, Is.EqualTo(Color.FromArgb(255, 255, 0)));
            Assert.That(conditional.Shading.BackgroundPatternColor, Is.EqualTo(Color.FromArgb(128, 128, 255)));

            // Border is not taken from a parent table style.
            Border border = conditional.Borders.Top;
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.None));

            Assert.That(conditional.TopPadding, Is.EqualTo(4.5));
            Assert.That(conditional.LeftPadding, Is.EqualTo(1.5));
            Assert.That(conditional.RightPadding, Is.EqualTo(6));
            Assert.That(conditional.BottomPadding, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-18400 Tests the <see cref="ConditionalStyleCollection.ClearFormatting"/> public method.
        /// </summary>
        [Test]
        public void TestClearingFormattingOfConditionalStyles()
        {
            const string fileName = @"Model\Style\Table\TestTableStyleProperties.docx";
            Document doc = TestUtil.Open(fileName);
            TableStyle style = (TableStyle)doc.Styles["TableStyle2"];
            ConditionalStyleCollection conditionals = style.ConditionalStyles;

            Assert.That(conditionals.DefinedStyles.Count, Is.EqualTo(12));

            conditionals.ClearFormatting();

            Assert.That(conditionals.DefinedStyles.Count, Is.EqualTo(0));
        }

        [TestCase(@"Model\Style\Table\TestJira17857_tblpPr_style.docx")]
        public void TestJira17857tblpPrStyle(string fileName)
        {
            Document doc = TestUtil.Open(fileName);

            TableStyle style = (TableStyle)doc.Styles["Table Floating"];

            Debug.Assert(style.TablePr.RelativeVerticalPosition == RelativeVerticalPosition.TableDefault);
        }

        /// <summary>
        /// WORDSNET-18708 There was a IndexOutOfRangeException exception on conditional style enumeration.
        /// Fixed now.
        /// </summary>
        [Test]
        public void TestJira18708()
        {
            Document doc = new Document();
            TableStyle tableStyle = (TableStyle)doc.Styles.Add(StyleType.Table, "MyTableStyle1");

            // There was an error on enumeration.
            foreach (ConditionalStyle conditional in tableStyle.ConditionalStyles);

            Assert.That(tableStyle.ConditionalStyles.Count, Is.EqualTo(12));
        }


        /// <summary>
        /// WORDSNET-21796, case 4 Disabled First Row conditional style was applied to a table, if
        /// <see cref="TablePr.HeadingFormat"/> is <c>true</c> for the first row(s) of it.
        /// </summary>
        [Test]
        public void Test21796Table4()
        {
            Document doc = TestUtil.Open(@"Model\Table\Test21796Table4.docx");
            Table table = doc.FirstSection.Body.Tables[0];
            Cell cell = table.FirstRow.FirstCell;
            Border border = cell.CellFormat.Borders.Bottom;

            Assert.That(border.IsVisible, Is.True);
            Assert.That(border.LineWidth, Is.EqualTo(0.5));

            // Run validator to generate direct formatting using TableFormattingExpander.
            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);
            border = cell.CellPr.BorderBottom;
            // The bolder is null, which indicates that First Row conditional style is not applied.
            Assert.That(border, Is.Null);
        }

        /// <summary>
        /// WORDSNET-21114 Add feature to get or set Cell Vertical Alignment using TableStyle.
        /// Added new public property TableStyle.VerticalAlignment.
        /// </summary>
        [Test]
        public void Test21114()
        {
            const string tableStyle = "Custom Table 1";
            const string fileName = @"Model\Table\Test21114";

            // Check read values.
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            TableStyle style = (TableStyle)doc.Styles[tableStyle];
            Assert.That(style.VerticalAlignment, Is.EqualTo(CellVerticalAlignment.Bottom));
            Assert.That(style.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));

            // Check modified values.
            style.VerticalAlignment = CellVerticalAlignment.Center;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            doc = TestUtil.SaveOpen(doc, fileName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);
            style = (TableStyle)doc.Styles[tableStyle];
            Assert.That(style.VerticalAlignment, Is.EqualTo(CellVerticalAlignment.Center));
            Assert.That(style.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));
        }



        /// <summary>
        /// WORDSNET-28402 Remove obsolete TableStyle.Bidi property.
        /// Removed obsolete BiDi property from <see cref="TableStyle"/> class.
        /// </summary>
        [Test]
        public void Test28402()
        {
#if !JAVA && !CPLUSPLUS
            // Check there is no BiDi property in TableStyle class.
            Type tableStyleType = typeof(TableStyle);
            Assert.That(tableStyleType.GetMember("Bidi"), Is.Empty);
#endif
        }

        private static void CheckTable22728(Table table)
        {
            Cell firstRowCell = table.FirstRow.FirstCell;
            Cell secondRowCell = table.Rows[1].FirstCell;

            // Test TableStyle methods that get formatting.

            Shading firstRowShading = firstRowCell.CellFormat.Shading;
            Shading secondRowShading = secondRowCell.CellFormat.Shading;
            Assert.That(firstRowShading.BackgroundPatternColor.ToArgb(), Is.EqualTo(0));
            Assert.That(secondRowShading.BackgroundPatternColor.ToArgb(), Is.EqualTo(unchecked((int)0xffe6faff)));

            // Test TableFormattingExpander.

            // Run validator to generate direct formatting using TableFormattingExpander.
            TestUtil.ExecuteValidator(table.FetchDocument(), SaveFormat.Markdown);
            // Get and check direct attributes.
            firstRowShading = (Shading)firstRowCell.CellPr.GetDirectAttr(CellAttr.Shading);
            secondRowShading = (Shading)secondRowCell.CellPr.GetDirectAttr(CellAttr.Shading);
            Assert.That(firstRowShading.BackgroundPatternColor.ToArgb(), Is.EqualTo(0));
            Assert.That(secondRowShading.BackgroundPatternColor.ToArgb(), Is.EqualTo(unchecked((int)0xffe6faff)));
        }

        /// <summary>
        /// Checks properties of the specified shading object.
        /// </summary>
        private static void CheckShading(Shading shading, int expectedBackgroundColor,
            string expectedThemeFill, string expectedThemeFillTint)
        {
            Assert.That(shading.BackgroundPatternColor.ToArgb(), Is.EqualTo(expectedBackgroundColor));
            Assert.That(shading.ThemeFill, Is.EqualTo(expectedThemeFill));
            Assert.That(shading.ThemeFillTint, Is.EqualTo(expectedThemeFillTint));
        }

        /// <summary>
        /// Check text in specified cell is not italic.
        /// </summary>
        private static void CheckNotItalic(Table table, int columnIndex, int rowIndex)
        {
            Cell cell = table.GetCell(columnIndex, rowIndex);
            RunPr runPr = cell.FirstParagraph.FirstRun.RunPr;

            Assert.That(runPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.False));
        }

        /// <summary>
        /// Check text in specified cell is not bold.
        /// </summary>
        private static void CheckNotBold(Table table, int columnIndex, int rowIndex)
        {
            Cell cell = table.GetCell(columnIndex, rowIndex);
            RunPr runPr = cell.FirstParagraph.FirstRun.RunPr;

            Assert.That(runPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.False));
        }

        private static void CheckImportTableStyle(ImportFormatMode mode, string fileName)
        {
            Document srcDoc = TestUtil.Open(@"Model\Style\Table\TestImportTableStyle.docx");
            Document dstDoc = new Document();
            dstDoc.RemoveAllChildren();
            dstDoc.AppendDocument(srcDoc, mode);
            TestUtil.SaveOpen(dstDoc, fileName);
        }

        /// <summary>
        /// Returns total number of table styles.
        /// </summary>
        private static int GetTableStylesCount(StyleCollection styles)
        {
            int tableStylesCount = 0;
            foreach (Style style in styles)
                if (style.Type == StyleType.Table)
                    tableStylesCount++;

            return tableStylesCount;
        }

        /// <summary>
        /// WORDSJAVA-2491 The customer wishes to read the table style margins from a DOCX file,
        /// but the values returned for paddings are '0.0'.
        /// </summary>
        [Test]
        public void TestJava2491()
        {
            Document doc = TestUtil.Open(@"Model\Style\Table\TestJava2491.docx");
            double defaultPaddingCm = 1.27;
            double multiplierPointToCm = 0.0352777776;
            foreach (Style style in doc.Styles)
            {
                if (style.Name.Equals("TableStyle1"))
                {
                    Assert.That(defaultPaddingCm - ((TableStyle)style).BottomPadding * multiplierPointToCm, Is.LessThanOrEqualTo(0.0001));
                    Assert.That(defaultPaddingCm - ((TableStyle)style).TopPadding * multiplierPointToCm, Is.LessThanOrEqualTo(0.0001));
                    Assert.That(defaultPaddingCm - ((TableStyle)style).RightPadding * multiplierPointToCm, Is.LessThanOrEqualTo(0.0001));
                    Assert.That(defaultPaddingCm - ((TableStyle)style).LeftPadding * multiplierPointToCm, Is.LessThanOrEqualTo(0.0001));
                }
            }
        }
    }
}
