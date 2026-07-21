// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2023 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests members of the <see cref="ChartTitle"/> and <see cref="ChartAxisTitle"/> classes.
    /// </summary>
    [TestFixture]
    public class TestChartTitle
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

            SystemPal.SaveCulture();
            // Set standard culture because default chart title depends on current culture. 
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void TestTearDown()
        {
            SystemPal.RestoreCulture();
        }

        /// <summary>
        /// Tests the <see cref="ChartTitle.Font"/> property.
        /// </summary>
        [Test]
        public void TestChartTitleFont()
        {
            const string fileName = @"Model\Charts\TestChartTitleFont";

            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            // Check font.
            ChartTitle title2 = shapes[1].Chart.Title;
            Color defaultColor = Color.FromArgb(89, 89, 89);
            CheckFontProperties(title2.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 14, 12, 0,
                false, false, false, false, false, false, defaultColor, Underline.None);
            ChartTitle title3 = shapes[2].Chart.Title;
            Color title3Color = Color.FromArgb(68, 114, 196);
            CheckFontProperties(title3.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 10,
                12, 0, false, false, false, false, false, false, title3Color, Underline.None);
            ChartTitle title6 = shapes[5].Chart.Title;
            CheckFontProperties(title6.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman", 14, 0,
                0, false, false, false, false, false, false, defaultColor, Underline.None);
            ChartTitle title7 = shapes[6].Chart.Title;
            Color title7Color = Color.FromArgb(255, 192, 0);
            CheckFontProperties(title7.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman", 16,
                0, 0, false, false, false, false, false, false, title7Color, Underline.None);

            // Change chart title from default to explicitly specified, and then check that font properties are defined
            // correctly in this case.
            title2.Text = "Explicitly Defined Title (in code)";
            CheckFontProperties(title2.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 14, 12, 0,
                false, false, false, false, false, false, defaultColor, Underline.None);
            title6.Text = "Explicitly Defined Title (in code)";
            CheckFontProperties(title6.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 14, 0,
                0, false, false, false, false, false, false, defaultColor, Underline.None);

            // Change chart title from explicitly specified to default (setting empty string sets the default title text),
            // and then check that font properties are defined correctly in this case.
            ChartTitle title4 = shapes[3].Chart.Title;
            CheckFontProperties(title4.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 10, 12, 0,
                false, false, false, false, false, false, title3Color, Underline.None);
            title4.Text = "";
            CheckFontProperties(title4.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 10, 12, 0,
                false, false, false, false, false, false, title3Color, Underline.None);
            ChartTitle title8 = shapes[7].Chart.Title;
            CheckFontProperties(title8.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman", 16,
                0, 0, false, false, false, false, false, false, title7Color, Underline.None);
            title8.Text = "";
            CheckFontProperties(title8.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman", 16,
                0, 0, false, false, false, false, false, false, title7Color, Underline.None);

            // Add new charts at runtime.

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            builder.InsertChart(ChartType.Line, 432, 200);

            builder.InsertParagraph();
            builder.InsertParagraph();

            Shape shape10 = builder.InsertChart(ChartType.Line, 432, 200);
            ChartTitle title10 = shape10.Chart.Title;
            CheckFontProperties(title10.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 14, 12, 0,
                false, false, false, false, false, false, defaultColor, Underline.None);
            title10.Text = "Explicitly Defined Title (in code)";

            // Change title font.

            foreach (Shape shape in shapes)
            {
                Font font = shape.Chart.Title.Font;
                SetFontProperties(font);
                CheckFontProperties(font);
            }

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);
            shapes = doc.FirstSection.Body.Shapes;

            foreach (Shape shape in shapes)
            {
                Font font = shape.Chart.Title.Font;
                CheckFontProperties(font);

                // Check that font format is kept when switching to the default title text.
                shape.Chart.Title.Text = "";
                CheckFontProperties(font);
            }
        }

        /// <summary>
        /// Tests the <see cref="ChartAxisTitle.Font"/> property.
        /// </summary>
        [Test]
        public void TestChartAxisTitleFont()
        {
            const string fileName = @"Model\Charts\TestChartAxisTitleFont";

            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            // Check font.
            ChartAxisTitle titleX1 = shapes[0].Chart.AxisX.Title;
            Color defaultColor = Color.FromArgb(89, 89, 89);
            CheckFontProperties(titleX1.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 10, 12, 0,
                false, false, false, false, false, false, defaultColor, Underline.None);
            ChartAxisTitle titleX2 = shapes[1].Chart.AxisX.Title;
            Color titleX2Color = Color.FromArgb(237, 125, 49);
            CheckFontProperties(titleX2.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 8, 12, 0,
                false, false, false, false, false, false, titleX2Color, Underline.None);
            ChartAxisTitle titleY2 = shapes[1].Chart.AxisY.Title;
            CheckFontProperties(titleY2.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 11, 12, 0,
                false, false, false, false, false, false, defaultColor, Underline.None);
            ChartAxisTitle titleX3 = shapes[2].Chart.AxisX.Title;
            CheckFontProperties(titleX3.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman",
                9, 0, 0, false, false, false, false, false, false, defaultColor, Underline.None);
            ChartAxisTitle titleX4 = shapes[3].Chart.AxisX.Title;
            CheckFontProperties(titleX4.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman",
                8, 0, 0, false, false, false, false, false, false, titleX2Color, Underline.None);
            ChartAxisTitle titleY4 = shapes[3].Chart.AxisY.Title;
            CheckFontProperties(titleY4.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman",
                11, 0, 0, false, false, false, false, false, false, defaultColor, Underline.None);

            // Change chart title from default to explicitly specified, and then check that font properties are defined
            // correctly in this case.
            ChartAxisTitle titleY1 = shapes[0].Chart.AxisY.Title;
            titleY1.Show = true;
            titleY1.Text = "Y axis";
            CheckFontProperties(titleY1.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman",
                10, 12, 0, false, false, false, false, false, false, defaultColor, Underline.None);
            ChartAxisTitle titleY3 = shapes[2].Chart.AxisY.Title;
            titleY3.Show = true;
            titleY3.Text = "Y axis";
            CheckFontProperties(titleY3.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman",
                9, 0, 0, false, false, false, false, false, false, defaultColor, Underline.None);

            // Change chart title from explicitly specified to default (setting empty string sets the default title text),
            // and then check that font properties are defined correctly in this case.
            titleY2.Text = "";
            CheckFontProperties(titleY2.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman", 11, 12, 0,
                false, false, false, false, false, false, defaultColor, Underline.None);
            titleY4.Text = "";
            CheckFontProperties(titleY4.Font, "Calibri", "Times New Roman", "Times New Roman", "Times New Roman",
                11, 0, 0, false, false, false, false, false, false, defaultColor, Underline.None);

            // Add a new chart at runtime.

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            Shape shape5 = builder.InsertChart(ChartType.Line, 432, 200);

            ChartAxisTitle titleX5 = shape5.Chart.AxisX.Title;
            titleX5.Show = true;
            CheckFontProperties(titleX5.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman",
                10, 12, 0, false, false, false, false, false, false, defaultColor, Underline.None);
            titleX5.Text = "X axis";
            CheckFontProperties(titleX5.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman",
                10, 12, 0, false, false, false, false, false, false, defaultColor, Underline.None);
            ChartAxisTitle titleY5 = shape5.Chart.AxisY.Title;
            titleY5.Show = true;
            CheckFontProperties(titleY5.Font, "Calibri", "Times New Roman", "游明朝", "Times New Roman",
                10, 12, 0, false, false, false, false, false, false, defaultColor, Underline.None);

            // Change title font.

            foreach (Shape shape in shapes)
            {
                Font fontX = shape.Chart.AxisX.Title.Font;
                SetFontProperties(fontX);
                CheckFontProperties(fontX);

                Font fontY = shape.Chart.AxisY.Title.Font;
                SetFontProperties(fontY);
                CheckFontProperties(fontY);
            }

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);
            shapes = doc.FirstSection.Body.Shapes;

            foreach (Shape shape in shapes)
            {
                Chart chart = shape.Chart;
                Font fontX = chart.AxisX.Title.Font;
                CheckFontProperties(fontX);

                Font fontY = chart.AxisY.Title.Font;
                CheckFontProperties(fontY);

                // Check that font format is kept when switching to the default title text.
                chart.AxisX.Title.Text = "";
                chart.AxisY.Title.Text = "";
                CheckFontProperties(fontX);
                CheckFontProperties(fontY);
            }
        }

        /// <summary>
        /// Tests font size of a chart title, in which font properties are not specified directly. MS Word uses
        /// 1.2 * [font size of chart space] value at this case.
        /// </summary>
        [Test]
        public void TestChartTitleRelativeFontSize()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestChartFormat.docx");
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            CheckFontProperties(chart.Title.Font, "Arial", "Arial", "Times New Roman", "Times New Roman", 11, 4, 0.4,
                true, false, false, false, false, false, Color.FromArgb(0xed, 0xed, 0xed), Underline.None);
        }

        /// <summary>
        /// Tests chart and axis title orientation and rotation in a new chart.
        /// </summary>
        [Test]
        public void TestTitleOrientationInNewChart()
        {
            Document doc = new Document();
            doc.FirstSection.PageSetup.LeftMargin = 20;
            doc.FirstSection.PageSetup.RightMargin = 20;
            doc.FirstSection.PageSetup.TopMargin = 20;
            doc.FirstSection.PageSetup.BottomMargin = 20;

            DocumentBuilder builder = new DocumentBuilder(doc);

            const ShapeTextOrientation maxOrientation = ShapeTextOrientation.WordArtVerticalRightToLeft;
            const int startRotation = -15;

            for (int i = 0; i <= (int)maxOrientation; i++)
            {
                ShapeTextOrientation orientation = (ShapeTextOrientation)i;
                int rotation = startRotation - i;

                Shape shape = builder.InsertChart(ChartType.Column, 285, 370);
                Chart chart = shape.Chart;
                chart.Legend.Position = LegendPosition.None;
                ChartTitle title = chart.Title;

                title.Orientation = orientation;
                title.Rotation = rotation;
                title.Text = "Orientation " + orientation;
                title.Font.Size = 8;

                foreach (ChartAxis axis in chart.Axes)
                {
                    ChartAxisTitle axisTitle = axis.Title;
                    axisTitle.Show = true;
                    axisTitle.Orientation = (ShapeTextOrientation)i;
                    axisTitle.Rotation = rotation;
                    axisTitle.Text = "Orientation " + orientation;
                    axisTitle.Font.Size = 6;
                }
            }

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTitleOrientationInNewChart.docx", null, true);

            for (int i = 0; i <= (int)maxOrientation; i++)
            {
                ShapeTextOrientation orientation = (ShapeTextOrientation)i;
                int rotation = startRotation - i;

                Shape shape = doc.FirstSection.Body.Shapes[i];
                Chart chart = shape.Chart;

                CheckOrientation(chart.Title.Orientation, chart.Title.Rotation, orientation, rotation);

                foreach (ChartAxis axis in chart.Axes)
                    CheckOrientation(axis.Title.Orientation, axis.Title.Rotation, orientation, rotation);
            }
        }

        /// <summary>
        /// Tests chart and axis title orientation and rotation in an existing chart.
        /// </summary>
        [Test]
        public void TestTitleOrientationInExistingChart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestTitleOrientation.docx");

            Chart chart1 = doc.FirstSection.Body.Shapes[0].Chart;
            ChartTitle title1 = chart1.Title;
            ChartAxisTitle x1Title = chart1.AxisX.Title;
            ChartAxisTitle y1Title = chart1.AxisY.Title;
            ChartAxisTitle secondaryY1Title = chart1.Axes[2].Title;

            CheckOrientation(title1.Orientation, title1.Rotation, ShapeTextOrientation.Horizontal, 30);
            CheckOrientation(x1Title.Orientation, x1Title.Rotation, ShapeTextOrientation.Horizontal, 90);
            CheckOrientation(y1Title.Orientation, y1Title.Rotation, ShapeTextOrientation.Horizontal, -90);
            CheckOrientation(secondaryY1Title.Orientation, secondaryY1Title.Rotation, ShapeTextOrientation.Horizontal, 90);

            Chart chart2 = doc.FirstSection.Body.Shapes[1].Chart;
            ChartTitle title2 = chart2.Title;
            ChartAxisTitle x2Title = chart2.AxisX.Title;
            ChartAxisTitle y2Title = chart2.AxisY.Title;

            CheckOrientation(title2.Orientation, title2.Rotation, ShapeTextOrientation.VerticalFarEast, 0);
            CheckOrientation(x2Title.Orientation, x2Title.Rotation, ShapeTextOrientation.Horizontal, 0);
            CheckOrientation(y2Title.Orientation, y2Title.Rotation, ShapeTextOrientation.Horizontal, -75);

            title1.Orientation = ShapeTextOrientation.WordArtVerticalRightToLeft;
            x1Title.Orientation = ShapeTextOrientation.VerticalFarEast;
            x1Title.Rotation = -90;
            y1Title.Orientation = ShapeTextOrientation.Upward;
            y1Title.Rotation = 0;
            secondaryY1Title.Rotation = -45;

            CheckOrientation(title1.Orientation, title1.Rotation, ShapeTextOrientation.WordArtVerticalRightToLeft, 30);
            CheckOrientation(x1Title.Orientation, x1Title.Rotation, ShapeTextOrientation.VerticalFarEast, -90);
            CheckOrientation(y1Title.Orientation, y1Title.Rotation, ShapeTextOrientation.Upward, 0);
            CheckOrientation(secondaryY1Title.Orientation, secondaryY1Title.Rotation, ShapeTextOrientation.Horizontal, -45);

            title2.Orientation = ShapeTextOrientation.Horizontal;
            title2.Rotation = 45;
            x2Title.Orientation = ShapeTextOrientation.WordArtVertical;
            x2Title.Rotation = -22;
            y2Title.Orientation = ShapeTextOrientation.Downward;

            CheckOrientation(title2.Orientation, title2.Rotation, ShapeTextOrientation.Horizontal, 45);
            CheckOrientation(x2Title.Orientation, x2Title.Rotation, ShapeTextOrientation.WordArtVertical, -22);
            CheckOrientation(y2Title.Orientation, y2Title.Rotation, ShapeTextOrientation.Downward, -75);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTitleOrientation.docx", null, true);

            chart1 = doc.FirstSection.Body.Shapes[0].Chart;
            title1 = chart1.Title;
            x1Title = chart1.AxisX.Title;
            y1Title = chart1.AxisY.Title;
            secondaryY1Title = chart1.Axes[2].Title;

            CheckOrientation(title1.Orientation, title1.Rotation, ShapeTextOrientation.WordArtVerticalRightToLeft, 30);
            CheckOrientation(x1Title.Orientation, x1Title.Rotation, ShapeTextOrientation.VerticalFarEast, -90);
            CheckOrientation(y1Title.Orientation, y1Title.Rotation, ShapeTextOrientation.Upward, 0);
            CheckOrientation(secondaryY1Title.Orientation, secondaryY1Title.Rotation, ShapeTextOrientation.Horizontal, -45);

            chart2 = doc.FirstSection.Body.Shapes[1].Chart;
            title2 = chart2.Title;
            x2Title = chart2.AxisX.Title;
            y2Title = chart2.AxisY.Title;

            CheckOrientation(title2.Orientation, title2.Rotation, ShapeTextOrientation.Horizontal, 45);
            CheckOrientation(x2Title.Orientation, x2Title.Rotation, ShapeTextOrientation.WordArtVertical, -22);
            CheckOrientation(y2Title.Orientation, y2Title.Rotation, ShapeTextOrientation.Downward, -75);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The property cannot be set in a Word 2016 chart.")]
        public void TestExceptionWheSettingTitleOrientationOfWord2016Chart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Waterfall.docx");
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            // This throws an exception.
            chart.Title.Orientation = ShapeTextOrientation.Downward;
        }

        /// <summary>
        /// Checks the specified orientation and rotation values.
        /// </summary>
        private static void CheckOrientation(ShapeTextOrientation actualOrientation, int actualRotation,
            ShapeTextOrientation expectedOrientation, int expectedRotation)
        {
            Assert.That(actualOrientation, Is.EqualTo(expectedOrientation));
            Assert.That(actualRotation, Is.EqualTo(expectedRotation));
        }

        /// <summary>
        /// Sets some predefined values to the properties of the specified <see cref="Font"/> instance.
        /// </summary>
        private static void SetFontProperties(Font font)
        {
            Color color = Color.FromArgb(60, 120, 220);
            TestChartUtil.SetFontProperties(font, "Arial", 11, 4, 3, true, true, true, false, true, false, color,
                Underline.DotDotDash, (RunVerticalAlignment)(-1), 0);
        }

        /// <summary>
        /// Checks properties of the specified <see cref="Font"/> instance for the values defined in the
        /// <see cref="SetFontProperties(Font)"/> method.
        /// </summary>
        private static void CheckFontProperties(Font font)
        {
            Color color = Color.FromArgb(60, 120, 220);
            CheckFontProperties(font, "Arial", "Arial", "Arial", "Arial", 11, 4, 3, true, true, true, false, true,
                false, color, Underline.DotDotDash);
        }

        /// <summary>
        /// Checks properties of the specified <see cref="Font"/> instance.
        /// </summary>
        private static void CheckFontProperties(Font font, string name, string nameBi, string nameFarEast,
            string nameOther, double size, double kerning, double spacing, bool bold, bool italic, bool strikeThrough,
            bool doubleStrikeThrough, bool allCaps, bool smallCaps, Color color, Underline underline)
        {
            TestChartUtil.CheckFontProperties(font, name, nameBi, nameFarEast, nameOther, size, kerning, spacing,
                bold, italic, strikeThrough, doubleStrikeThrough, allCaps, smallCaps, color, Color.Empty, underline,
                RunVerticalAlignment.Baseline, 0);
        }
    }
}
