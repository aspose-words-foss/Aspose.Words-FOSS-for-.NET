// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2023 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests members of the <see cref="ChartAxis"/> class.
    /// </summary>
    [TestFixture]
    public class TestChartAxis
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void TestSetUp()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// WORDSNET-16119 A category axis is marked as date/time on adding date/time category values.
        /// This promote that correct axis properties are written into a document file (on DOCX depending on 
        /// the <see cref="DmlChartAxisAttrs.IsDateCategoryAxis"/> attribute either catAx or dateAx element is generated).
        /// </summary>
        [Test]
        public void TestTimeCategoryAxis()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;
            ChartAxis xAxis = chart.AxisX;

            // Expected automatic-text category type.
            Assert.That(xAxis.IsDateCategoryAxis, Is.EqualTo(false));

            // Add series with DateTime data.
            chart.Series.Add("Series 1",
                new DateTime[] { new DateTime(2017, 11, 06), new DateTime(2017, 11, 09), new DateTime(2017, 11, 15),
                    new DateTime(2017, 11, 21), new DateTime(2017, 11, 25), new DateTime(2017, 11, 29) },
                new double[] { 1.2, 0.3, 2.1, 2.9, 4.2, 5.3 });

            // Expected automatic-date/time category type.
            Assert.That(xAxis.IsDateCategoryAxis, Is.EqualTo(true));

            chart.Series.Clear();

            // Expected automatic-text category type.
            Assert.That(xAxis.IsDateCategoryAxis, Is.EqualTo(false));
        }

        /// <summary>
        /// Tests the <see cref="ChartAxis.HasMajorGridlines"/> and <see cref="ChartAxis.HasMinorGridlines"/> properties.
        /// </summary>
        [Test]
        public void TestGridLines()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertChart(ChartType.Bar, 432, 252);
            CheckGridlines(doc, @"Model\Charts\Test{0}.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartAxis.HasMajorGridlines"/> and <see cref="ChartAxis.HasMinorGridlines"/> properties
        /// of a Word 2016 chart.
        /// </summary>
        [Test]
        public void TestGridLinesInWord2016Chart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            CheckGridlines(doc, @"Model\Charts\Word2016Charts\Pareto{0}.docx");
        }

        /// <summary>
        /// WORDSNET-24673 Provide public API to manipulate chart gridlines.
        /// Tests the customer's case as is.
        /// </summary>
        [Test]
        public void Test24673()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Bar, 432, 252);
            Chart chart = shape.Chart;
            chart.Legend.Position = LegendPosition.None;

            ChartSeriesCollection seriesColl = chart.Series;
            chart.AxisY.Hidden = true;
            chart.AxisY.HasMajorGridlines = false;
            chart.AxisX.Hidden = true;
            chart.Title.Show = false;

            seriesColl.Clear();
            chart.AxisY.Scaling.Maximum = new AxisBound(100);

            string[] categories = new string[] { "AW Category 1" };
            seriesColl.Add("AW Series 1", categories, new double[] { 30 });

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\Test24673.docx");
        }


        /// <summary>
        /// Tests the <see cref="ChartAxis.Title"/> property of a new chart.
        /// </summary>
        [Test]
        public void TestNewChartAxisTitle()
        {
            // Axis title text depends on current culture.
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
            try
            {
                Document doc = new Document();
                DocumentBuilder builder = new DocumentBuilder(doc);

                Shape shape = builder.InsertChart(ChartType.Bar, 432, 252);
                Chart chart = shape.Chart;

                ChartAxisTitle xTitle = chart.AxisX.Title;
                CheckTitle(xTitle, false, false, "Axis Title");
                ChartAxisTitle yTitle = chart.AxisY.Title;
                CheckTitle(yTitle, false, false, "Axis Title");

                xTitle.Text = "Category";
                xTitle.Overlay = true;
                xTitle.Show = true;

                yTitle.Show = true;
                yTitle.Overlay = true;
                yTitle.Text = "Value";

                CheckTitle(xTitle, true, true, "Category");
                CheckTitle(yTitle, true, true, "Value");

                doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestNewChartAxisTitle.docx", null, false);
                shape = doc.FirstSection.Body.Shapes[0];
                chart = shape.Chart;

                CheckTitle(chart.AxisX.Title, true, true, "Category");
                CheckTitle(chart.AxisY.Title, true, true, "Value");
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests the <see cref="ChartAxis.Title"/> property of an existing chart.
        /// </summary>
        [Test]
        public void TestExistingAxisTitle()
        {
            // Axis title text depends on current culture.
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
            try
            {
                Document doc = TestUtil.Open(@"Model\Charts\TestAxisTitle.docx");
                Chart chart1 = doc.FirstSection.Body.Shapes[0].Chart;

                ChartAxisTitle xTitle1 = chart1.AxisX.Title;
                CheckTitle(xTitle1, true, false, "Horizontal Axis");
                ChartAxisTitle yTitle1 = chart1.AxisY.Title;
                CheckTitle(yTitle1, true, false, "Vertical Axis ");

                xTitle1.Show = false;

                yTitle1.Overlay = true;
                yTitle1.Text = "Value";

                CheckTitle(xTitle1, false, false, "Horizontal Axis");
                CheckTitle(yTitle1, true, true, "Value");

                Chart chart4 = doc.FirstSection.Body.Shapes[3].Chart;

                ChartAxisTitle xTitle4 = chart4.AxisX.Title;
                CheckTitle(xTitle4, true, false, "Primary horizontal");
                ChartAxisTitle yTitle4 = chart4.AxisY.Title;
                CheckTitle(yTitle4, true, false, "Primary vertical");

                xTitle4.Text = null;
                yTitle4.Text = string.Empty;

                CheckTitle(xTitle4, true, false, "Axis Title");
                CheckTitle(yTitle4, true, false, "Axis Title");

                TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestExistingAxisTitle.docx");
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests a found issue: if an axis has no title and the <see cref="ChartAxisTitle.Show"/> property is set to
        /// <b>true</b> to display the title before setting title text, the title remains invisible.
        /// </summary>
        [Test]
        public void TestShowingAxisTitle()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

            Chart chart = shape.Chart;
            ChartAxisTitle xTitle = chart.AxisX.Title;
            ChartAxisTitle yTitle = chart.AxisY.Title;

            xTitle.Show = true;
            xTitle.Text = "Categories";
            yTitle.Show = true;
            yTitle.Text = "Values";

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestShowingAxisTitle.docx");
        }

        /// <summary>
        /// Tests the <see cref="AxisTickLabels.Font"/> property.
        /// </summary>
        [Test]
        public void TestTickLabelsFont()
        {
            const string fileName = @"Model\Charts\TestTickLabelsFont";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);

            // Clone the first and second charts, and a paragraph between them.
            Body body = doc.FirstSection.Body;
            Paragraph para1 = body.Paragraphs[0];
            body.AppendChild(para1.Clone(true));
            Paragraph para2 = body.Paragraphs[1];
            body.AppendChild(para2.Clone(true));
            Paragraph para3 = body.Paragraphs[2];
            body.AppendChild(para3.Clone(true));

            // Pre-Word 2016 chart.

            Chart chart1 = body.Shapes[0].Chart;

            // X axis font is changed.
            Font xFont1 = chart1.AxisX.TickLabels.Font;
            // Y axis font is taken from chart space.
            Font yFont1 = chart1.AxisY.TickLabels.Font;

            CheckFont(xFont1, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 12, 0.2, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(yFont1, "Times New Roman", 14, Color.Empty, Color.Empty, false, true,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Superscript, 1024);

            ChangeFont(xFont1, "Courier New", 11, Color.DarkRed, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            xFont1.HighlightColor = Color.Cyan;

            ChangeFont(yFont1, "Verdana", 12, Color.Green, true, false,
                Underline.None, 12, 4, false, true, false, true, RunVerticalAlignment.Baseline, 1049);

            // Word 2016 chart.

            Chart chart2 = body.Shapes[1].Chart;

            // X axis font is changed.
            Font xFont2 = chart2.AxisX.TickLabels.Font;
            // Default format is used for Y axis font.
            Font yFont2 = chart2.AxisY.TickLabels.Font;

            CheckFont(xFont2, "Calibri", 8, Color.FromArgb(68, 114, 196), Color.Empty, false, false,
                Underline.Dotted, 0, 0.2, false, false, true, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(yFont2, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            ChangeFont(xFont2, "Courier New", 10, Color.DarkRed, true, false,
                Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            xFont2.HighlightColor = Color.Cyan;

            ChangeFont(yFont2, "Times New Roman", 10, Color.Green, false, true,
                Underline.Single, 12, 2, false, true, false, true, RunVerticalAlignment.Subscript, 1049);

            // Pre-Word 2016 chart.

            Chart chart3 = body.Shapes[2].Chart;

            // Default font format is used for X axis.
            Font xFont3 = chart3.AxisX.TickLabels.Font;

            CheckFont(xFont3, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            // Pre-Word 2016 chart.

            Chart chart4 = body.Shapes[3].Chart;

            Font xFont4 = chart4.AxisX.TickLabels.Font;
            CheckFont(xFont4, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 12, 0.2, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            xFont4.ClearFormatting();

            // Word 2016 chart.

            Chart chart5 = body.Shapes[4].Chart;

            Font xFont5 = chart5.AxisX.TickLabels.Font;
            CheckFont(xFont5, "Calibri", 8, Color.FromArgb(68, 114, 196), Color.Empty, false, false,
                Underline.Dotted, 0, 0.2, false, false, true, false, RunVerticalAlignment.Baseline, 1024);

            xFont5.ClearFormatting();

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);
            body = doc.FirstSection.Body;
            chart1 = body.Shapes[0].Chart;
            chart2 = body.Shapes[1].Chart;
            chart3 = body.Shapes[2].Chart;
            chart4 = body.Shapes[3].Chart;
            chart5 = body.Shapes[4].Chart;

            // Check results.

            CheckFont(chart1.AxisX.TickLabels.Font, "Courier New", 11, Color.FromArgb(139, 0, 0),
                Color.FromArgb(0, 255, 255),
                true, false, Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            CheckFont(chart1.AxisY.TickLabels.Font, "Verdana", 12, Color.FromArgb(0, 128, 0), Color.Empty,
                true, false, Underline.None, 12, 4, false, true, false, true, RunVerticalAlignment.Baseline, 1049);
            CheckFont(chart2.AxisX.TickLabels.Font, "Courier New", 10, Color.FromArgb(139, 0, 0),
                Color.FromArgb(0, 255, 255),
                true, false, Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            CheckFont(chart2.AxisY.TickLabels.Font, "Times New Roman", 10, Color.FromArgb(0, 128, 0), Color.Empty,
                false, true, Underline.Single, 12, 2, false, true, false, true, RunVerticalAlignment.Subscript, 1049);
            CheckFont(chart3.AxisX.TickLabels.Font, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(chart4.AxisX.TickLabels.Font, "Times New Roman", 14, Color.Empty, Color.Empty, false, true,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Superscript, 1024);
            CheckFont(chart5.AxisX.TickLabels.Font, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
        }

        /// <summary>
        /// Tests the <see cref="AxisTickLabels.Font"/> property when creating a new chart.
        /// </summary>
        [TestCase(ChartType.Line)] // pre-Word 2016 chart
        [TestCase(ChartType.Waterfall)] // Word 2016 chart
        public void TestTickLabelsFontInNewChart(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            Font xFont = chart.AxisX.TickLabels.Font;
            Font yFont = chart.AxisY.TickLabels.Font;

            int defaultKerning = chart.ChartSpace.IsChartEx ? 0 : 12;

            CheckFont(xFont, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false, Underline.None,
                defaultKerning, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            CheckFont(yFont, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false, Underline.None,
                defaultKerning, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            // Change font.

            ChangeFont(xFont, "Arial", 11, Color.DarkRed, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            ChangeFont(yFont, "Times New Roman", 14, Color.Red, true, false,
                Underline.Double, 0, 0, false, true, false, true, RunVerticalAlignment.Subscript, 1049);

            // Check.

            CheckFont(xFont, "Arial", 11, Color.FromArgb(139, 0, 0), Color.Empty, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            CheckFont(yFont, "Times New Roman", 14, Color.FromArgb(255, 0, 0), Color.Empty, true, false,
                Underline.Double, 0, 0, false, true, false, true, RunVerticalAlignment.Subscript, 1049);
        }


        /// <summary>
        /// Tests tick labels orientation and rotation in a new chart.
        /// </summary>
        [Test]
        public void TestTickLabelsOrientationInNewChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape1 = builder.InsertChart(ChartType.Column, 432, 252);
            AxisTickLabels axisX1TickLabels = shape1.Chart.AxisX.TickLabels;
            AxisTickLabels axisY1TickLabels = shape1.Chart.AxisY.TickLabels;

            axisX1TickLabels.Orientation = ShapeTextOrientation.VerticalFarEast;
            axisX1TickLabels.Rotation = -10;
            axisY1TickLabels.Orientation = ShapeTextOrientation.Horizontal;
            axisY1TickLabels.Rotation = 45;

            Shape shape2 = builder.InsertChart(ChartType.Column, 432, 252);
            AxisTickLabels axisX2TickLabels = shape2.Chart.AxisX.TickLabels;
            AxisTickLabels axisY2TickLabels = shape2.Chart.AxisY.TickLabels;

            axisX2TickLabels.Orientation = ShapeTextOrientation.WordArtVertical;
            axisY2TickLabels.Rotation = 5;

            CheckOrientation(axisX1TickLabels, ShapeTextOrientation.VerticalFarEast, -10);
            CheckOrientation(axisY1TickLabels, ShapeTextOrientation.Horizontal, 45);
            CheckOrientation(axisX2TickLabels, ShapeTextOrientation.WordArtVertical, 0);
            CheckOrientation(axisY2TickLabels, ShapeTextOrientation.Horizontal, 5);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTickLabelsOrientationInNewChart.docx", null, true);

            shape1 = doc.FirstSection.Body.Shapes[0];
            axisX1TickLabels = shape1.Chart.AxisX.TickLabels;
            axisY1TickLabels = shape1.Chart.AxisY.TickLabels;
            shape2 = doc.FirstSection.Body.Shapes[1];
            axisX2TickLabels = shape2.Chart.AxisX.TickLabels;
            axisY2TickLabels = shape2.Chart.AxisY.TickLabels;

            CheckOrientation(axisX1TickLabels, ShapeTextOrientation.VerticalFarEast, -10);
            CheckOrientation(axisY1TickLabels, ShapeTextOrientation.Horizontal, 45);
            CheckOrientation(axisX2TickLabels, ShapeTextOrientation.WordArtVertical, 0);
            CheckOrientation(axisY2TickLabels, ShapeTextOrientation.Horizontal, 5);
        }

        /// <summary>
        /// Tests tick labels orientation and rotation in an existing chart.
        /// </summary>
        [Test]
        public void TestTickLabelsOrientationInExistingChart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestTickLabelsOrientation.docx");
            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            AxisTickLabels axisX1TickLabels = shape1.Chart.AxisX.TickLabels;
            AxisTickLabels axisY1TickLabels = shape1.Chart.AxisY.TickLabels;
            Shape shape2 = doc.FirstSection.Body.Shapes[1];
            AxisTickLabels axisX2TickLabels = shape2.Chart.AxisX.TickLabels;
            AxisTickLabels axisY2TickLabels = shape2.Chart.AxisY.TickLabels;

            CheckOrientation(axisX1TickLabels, ShapeTextOrientation.Horizontal, 85);
            CheckOrientation(axisY1TickLabels, ShapeTextOrientation.Horizontal, -90);
            CheckOrientation(axisX2TickLabels, ShapeTextOrientation.WordArtVertical, 20);
            CheckOrientation(axisY2TickLabels, ShapeTextOrientation.Horizontal, 0);

            axisX1TickLabels.Orientation = ShapeTextOrientation.WordArtVerticalRightToLeft;
            axisX1TickLabels.Rotation = 0;

            axisY1TickLabels.Rotation = -10;

            axisX2TickLabels.Orientation = ShapeTextOrientation.VerticalFarEast;

            axisY2TickLabels.Rotation = 10;

            CheckOrientation(axisX1TickLabels, ShapeTextOrientation.WordArtVerticalRightToLeft, 0);
            CheckOrientation(axisY1TickLabels, ShapeTextOrientation.Horizontal, -10);
            CheckOrientation(axisX2TickLabels, ShapeTextOrientation.VerticalFarEast, 20);
            CheckOrientation(axisY2TickLabels, ShapeTextOrientation.Horizontal, 10);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTickLabelsOrientation.docx", null, true);

            shape1 = doc.FirstSection.Body.Shapes[0];
            axisX1TickLabels = shape1.Chart.AxisX.TickLabels;
            axisY1TickLabels = shape1.Chart.AxisY.TickLabels;
            shape2 = doc.FirstSection.Body.Shapes[1];
            axisX2TickLabels = shape2.Chart.AxisX.TickLabels;
            axisY2TickLabels = shape2.Chart.AxisY.TickLabels;

            CheckOrientation(axisX1TickLabels, ShapeTextOrientation.WordArtVerticalRightToLeft, 0);
            CheckOrientation(axisY1TickLabels, ShapeTextOrientation.Horizontal, -10);
            CheckOrientation(axisX2TickLabels, ShapeTextOrientation.VerticalFarEast, 20);
            CheckOrientation(axisY2TickLabels, ShapeTextOrientation.Horizontal, 10);
        }

        /// <summary>
        /// Checks the <see cref="AxisTickLabels.Orientation"/> and <see cref="AxisTickLabels.Rotation"/>
        /// properties of axis tick labels.
        /// </summary>
        private static void CheckOrientation(AxisTickLabels tickLabels, ShapeTextOrientation expectedOrientation,
            int expectedRotation)
        {
            Assert.That(tickLabels.Orientation, Is.EqualTo(expectedOrientation));
            Assert.That(tickLabels.Rotation, Is.EqualTo(expectedRotation));
        }

        /// <summary>
        /// Checks properties of a <see cref="Font"/> instance.
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
        /// Changes properties of a <see cref="Font"/> instance.
        /// </summary>
        private static void ChangeFont(Font font, string fontName, double fontSize, Color color, bool bold,
            bool italic, Underline underline, int kerning, int spacing, bool doubleStrikeThrough,
            bool strikeThrough, bool allCaps, bool smallCaps, RunVerticalAlignment verticalAlignment, int localeId)
        {
            TestChartUtil.SetFontProperties(font, fontName, fontSize, kerning, spacing, bold, italic, strikeThrough,
                doubleStrikeThrough, allCaps, smallCaps, color, underline, verticalAlignment, localeId);
        }

        /// <summary>
        /// Sets the <see cref="ChartAxis.HasMajorGridlines"/> and <see cref="ChartAxis.HasMinorGridlines"/> properties
        /// of the first chart in the document and checks golds when gridlines are shown and when they are hidden.
        /// </summary>
        private static void CheckGridlines(Document doc, string outputFileNameTemplate)
        {
            Shape shape = doc.FirstSection.Body.Shapes[0];
            Chart chart = shape.Chart;
            ChartAxis xAxis = chart.AxisX;
            ChartAxis yAxis = chart.AxisY;

            xAxis.HasMajorGridlines = true;
            xAxis.HasMinorGridlines = true;
            yAxis.HasMajorGridlines = true;
            yAxis.HasMinorGridlines = true;

            TestUtil.SaveCheckGoldExportOnly(doc, string.Format(outputFileNameTemplate, "ShownGridLines"));

            xAxis.HasMajorGridlines = false;
            xAxis.HasMinorGridlines = false;
            yAxis.HasMajorGridlines = false;
            yAxis.HasMinorGridlines = false;

            TestUtil.SaveCheckGoldExportOnly(doc, string.Format(outputFileNameTemplate, "HiddenGridLines"));
        }

        /// <summary>
        /// Checks the properties of the passed <see cref="ChartAxisTitle"/> instance.
        /// </summary>
        private static void CheckTitle(ChartAxisTitle title, bool expectedShow, bool expectedOverlay, string expectedText)
        {
            Assert.That(title.Show, Is.EqualTo(expectedShow));
            Assert.That(title.Overlay, Is.EqualTo(expectedOverlay));
            Assert.That(title.Text, Is.EqualTo(expectedText));
        }
    }
}
