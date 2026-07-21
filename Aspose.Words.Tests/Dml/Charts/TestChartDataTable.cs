// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/01/2024 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests members of the <see cref="ChartDataTable"/> class.
    /// </summary>
    [TestFixture]
    public class TestChartDataTable
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests the <see cref="ChartDataTable.Font"/> property.
        /// </summary>
        [Test]
        public void TestDataTableProperties()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestDataTableFormat.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            ChartDataTable dataTable1 = shapes[0].Chart.DataTable;
            ChartDataTable dataTable2 = shapes[1].Chart.DataTable;
            ChartDataTable dataTable3 = shapes[2].Chart.DataTable;
            ChartDataTable dataTable4 = shapes[3].Chart.DataTable;

            CheckDataTable(dataTable1, true, true, false, false, true);
            CheckDataTable(dataTable2, true, false, true, true, false);
            CheckDataTable(dataTable3, true, true, true, true, true);
            // This data table has borders, but Format.Stroke.Visible is 'false'.
            Assert.That(dataTable4.Format.Stroke.Visible, Is.False);
            CheckDataTable(dataTable4, true, false, true, true, true);

            // Change properties.

            dataTable1.HasLegendKeys = false;
            dataTable1.HasHorizontalBorder = true;
            dataTable1.HasVerticalBorder = true;
            dataTable1.HasOutlineBorder = false;

            dataTable2.HasLegendKeys = true;
            dataTable2.HasOutlineBorder = true;

            dataTable3.HasLegendKeys = false;
            dataTable3.HasHorizontalBorder = false;
            dataTable3.HasVerticalBorder = false;
            dataTable3.HasOutlineBorder = false;

            dataTable4.Show = false;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestDataTableProperties.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartDataTable.Font"/> property.
        /// </summary>
        [Test]
        public void TestFont()
        {
            const string fileName = @"Model\Charts\TestDataTableFont";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            Assert.That(shapes[0].Chart.DataTable.Show, Is.True);

            Font font1 = shapes[0].Chart.DataTable.Font;
            Font font2 = shapes[1].Chart.DataTable.Font;
            Font font3 = shapes[2].Chart.DataTable.Font;
            Font font4 = shapes[3].Chart.DataTable.Font;

            CheckFont(font1, "Arial", 11.5, Color.FromArgb(0x2E, 0x75, 0xB5), Color.Empty, true, true,
                Underline.DottedHeavy, 7, 1, false, false, false, true, RunVerticalAlignment.Subscript, 1024);
            CheckFont(font2, "Calibri", 10, Color.FromArgb(0x59, 0x59, 0x59), Color.Empty, false, true,
                Underline.None, 0, 0, true, false, true, false, RunVerticalAlignment.Baseline, 1024);
            // Data table of the 3rd chart uses chart space font.
            CheckFont(font3, "Times New Roman", 14, Color.Empty, Color.Empty, false, true,
                Underline.None, 0, 0.5, false, true, true, false, RunVerticalAlignment.Superscript, 1024);
            // Data table of the 4th chart uses font defaults.
            CheckFont(font4, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            ChangeFont(font1, "Courier New", 11, Color.DarkRed, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            font1.HighlightColor = Color.Cyan;

            font2.ClearFormatting();

            ChangeFont(font3, "Verdana", 12, Color.Green, true, false,
                Underline.None, 12, 4, false, true, false, true, RunVerticalAlignment.Baseline, 1049);

            ChangeFont(font4, "Times New Roman", 14, Color.Red, false, true,
                Underline.Dotted, 3, 3, false, true, true, false, RunVerticalAlignment.Subscript, 1049);

            CheckFont(font1, "Courier New", 11, DrColor.DarkRed.ToNativeColor(), DrColor.Cyan.ToNativeColor(),
                true, false, Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            CheckFont(font2, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(font3, "Verdana", 12, DrColor.Green.ToNativeColor(), Color.Empty, true, false,
                Underline.None, 12, 4, false, true, false, true, RunVerticalAlignment.Baseline, 1049);
            CheckFont(font4, "Times New Roman", 14, DrColor.Red.ToNativeColor(), Color.Empty,
                false, true, Underline.Dotted, 3, 3, false, true, true, false, RunVerticalAlignment.Subscript, 1049);

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);
            shapes = doc.FirstSection.Body.Shapes;
            font1 = shapes[0].Chart.DataTable.Font;
            font2 = shapes[1].Chart.DataTable.Font;
            font3 = shapes[2].Chart.DataTable.Font;
            font4 = shapes[3].Chart.DataTable.Font;

            CheckFont(font1, "Courier New", 11, DrColor.DarkRed.ToNativeColor(), DrColor.Cyan.ToNativeColor(),
                true, false, Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            CheckFont(font2, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(font3, "Verdana", 12, DrColor.Green.ToNativeColor(), Color.Empty, true, false,
                Underline.None, 12, 4, false, true, false, true, RunVerticalAlignment.Baseline, 1049);
            CheckFont(font4, "Times New Roman", 14, DrColor.Red.ToNativeColor(), Color.Empty,
                false, true, Underline.Dotted, 3, 3, false, true, true, false, RunVerticalAlignment.Subscript, 1049);
        }

        /// <summary>
        /// Tests the <see cref="ChartDataTable.Font"/> property in a created chart.
        /// </summary>
        [Test]
        public void TestFontInNewChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            ChartDataTable dataTable = shape.Chart.DataTable;

            Assert.That(dataTable.Show, Is.False);
            dataTable.Show = true;

            Font font = dataTable.Font;

            CheckFont(font, "Calibri", 9, Color.FromArgb(0x59, 0x59, 0x59), Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            // Change font.

            ChangeFont(font, "Arial", 12, Color.DarkRed, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            // Check.

            CheckFont(font, "Arial", 12, DrColor.DarkRed.ToNativeColor(), Color.Empty, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
        }

        /// <summary>
        /// Tests that an exception is generated when showing a data table for charts of unsupported types.
        /// </summary>
        [TestCase(@"Word2016Charts\BoxWhisker.docx")]
        [TestCase("TestScatterChart.docx")]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "This chart type does not support a data table.")]
        public void TestShowingDataTableInNonSupportedChart(string relativeFileName)
        {
            Document doc = TestUtil.Open(@"Model\Charts\" + relativeFileName);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            // This throws an exception.
            chart.DataTable.Show = true;
        }

        /// <summary>
        /// Checks properties of the specified <see cref="Font"/> instance.
        /// </summary>
        private static void CheckFont(Font font, string expectedFontName, double expectedFontSize, Color expectedColor,
            Color expectedHighlightColor, bool expectedBold, bool expectedItalic, Underline expectedUnderline,
            int expectedKerning, double expectedSpacing, bool expectedDoubleStrikeThrough, bool expectedStrikeThrough,
            bool expectedAllCaps, bool expectedSmallCaps, RunVerticalAlignment expectedVerticalAlignment,
            int expectedLocaleId)
        {
            TestChartUtil.CheckFontProperties(font, expectedFontName, null, null, null, expectedFontSize,
                expectedKerning, expectedSpacing, expectedBold, expectedItalic, expectedStrikeThrough,
                expectedDoubleStrikeThrough, expectedAllCaps, expectedSmallCaps, expectedColor, expectedHighlightColor,
                expectedUnderline, expectedVerticalAlignment, expectedLocaleId);
        }

        /// <summary>
        /// Changes properties of the specified <see cref="Font"/> instance.
        /// </summary>
        private static void ChangeFont(Font font, string fontName, double fontSize, Color color, bool bold,
            bool italic, Underline underline, int kerning, int spacing, bool doubleStrikeThrough,
            bool strikeThrough, bool allCaps, bool smallCaps, RunVerticalAlignment verticalAlignment, int localeId)
        {
            TestChartUtil.SetFontProperties(font, fontName, fontSize, kerning, spacing, bold, italic, strikeThrough,
                doubleStrikeThrough, allCaps, smallCaps, color, underline, verticalAlignment, localeId);
        }

        /// <summary>
        /// Checks properties of the specified data table.
        /// </summary>
        private void CheckDataTable(ChartDataTable dataTable, bool isShown, bool hasLegendKeys,
            bool hasHorizontalBorder, bool hasVerticalBorder, bool hasOutlineBorder)
        {
            Assert.That(dataTable.Show, Is.EqualTo(isShown));
            Assert.That(dataTable.HasLegendKeys, Is.EqualTo(hasLegendKeys));
            Assert.That(dataTable.HasHorizontalBorder, Is.EqualTo(hasHorizontalBorder));
            Assert.That(dataTable.HasVerticalBorder, Is.EqualTo(hasVerticalBorder));
            Assert.That(dataTable.HasOutlineBorder, Is.EqualTo(hasOutlineBorder));
        }
    }
}
