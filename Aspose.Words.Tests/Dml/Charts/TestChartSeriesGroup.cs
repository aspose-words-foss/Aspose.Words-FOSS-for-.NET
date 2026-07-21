// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2024 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests functionality related to chart series groups and combo charts.
    /// </summary>
    [TestFixture]
    public class TestChartSeriesGroup
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// Tests creating a combo chart with secondary Y axis.
        /// </summary>
        [Test]
        public void TestCreatingComboChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Line, 450, 250);
            Chart chart = shape.Chart;

            Assert.That(chart.SeriesGroups.Count, Is.EqualTo(1));
            ChartSeriesGroup seriesGroup = chart.SeriesGroups[0];
            Assert.That(seriesGroup.SeriesType, Is.EqualTo(ChartSeriesType.Line));
            Assert.That(seriesGroup.AxisGroup, Is.EqualTo(AxisGroup.Primary));

            ChartSeriesCollection series = chart.Series;
            series.Clear();

            string[] categories = new string[] { "Category 1", "Category 2", "Category 3" };
            series.Add("Primary Group Series 1", categories, new double[] { 2, 3, 4 });
            series.Add("Primary Group Series 2", categories, new double[] { 5, 2, 3 });

            ChartSeriesGroup newSeriesGroup = chart.SeriesGroups.Add(ChartSeriesType.Line);
            newSeriesGroup.AxisGroup = AxisGroup.Secondary;
            newSeriesGroup.AxisX.Hidden = true;
            newSeriesGroup.AxisY.Title.Show = true;
            newSeriesGroup.AxisY.Title.Text = "Secondary Y axis";

            newSeriesGroup.Series.Add("Secondary Group Series", categories, new double[] { 13, 11, 16 });

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestCreatingComboChart.docx");
        }

        /// <summary>
        /// Tests creating a combo chart with secondary X and Y axes.
        /// </summary>
        [Test]
        public void TestComboChartWithSecondaryAxes()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;

            ChartSeriesCollection series = chart.Series;
            series.Clear();

            string[] categories = new string[] { "Category 1", "Category 2", "Category 3" };
            series.Add("Series 1", categories, new double[] { 2, 3, 4 });
            series.Add("Series 2", categories, new double[] { 5, 2, 3 });

            ChartSeriesGroup seriesGroup = chart.SeriesGroups.Add(ChartSeriesType.Scatter);
            seriesGroup.AxisGroup = AxisGroup.Secondary;
            ChartAxisTitle axisTitle = seriesGroup.AxisX.Title;
            axisTitle.Show = true;
            axisTitle.Overlay = false;
            axisTitle.Text = "Secondary X axis";
            seriesGroup.Series.Add("Series 3", new double[] { 10, 15, 24 }, new double[] { 3, 1, 6 });

            chart.Title.Overlay = false;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestComboChartWithSecondaryXAxis.docx");
        }

        /// <summary>
        /// Tests creating a Volume-Open-High-Low-Close stock chart similar to the one that can be created in MS Word.
        /// </summary>
        [Test]
        public void TestStockComboChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;

            ChartSeriesCollection series = chart.Series;
            series.Clear();

            DateTime[] categories = new DateTime[] { new DateTime(2002, 5, 1), new DateTime(2002, 6, 1),
                new DateTime(2002, 7, 1), new DateTime(2002, 8, 1), new DateTime(2002, 9, 1) };
            series.Add("Volume", categories, new double[] { 70, 120, 150, 135, 148 });

            ChartSeriesGroup stockSeriesGroup = chart.SeriesGroups.Add(ChartSeriesType.Stock);
            stockSeriesGroup.AxisGroup = AxisGroup.Secondary;
            stockSeriesGroup.AxisX.Hidden = true;

            ChartSeriesCollection stockSeries = stockSeriesGroup.Series;
            ChartSeries open = stockSeries.Add("Open", categories, new double[] { 44, 25, 38, 50, 34 });
            stockSeries.Add("High", categories, new double[] { 55, 57, 57, 58, 58 });
            stockSeries.Add("Low", categories, new double[] { 12, 12, 13, 11, 25 });
            ChartSeries close = stockSeries.Add("Close", categories, new double[] { 25, 38, 50, 35, 43 });

            // AW doesn't allow showing Up-Down bars, so use markers to display open and close values.
            open.Marker.Symbol = MarkerSymbol.Dash;
            open.Marker.Size = 20;
            open.Marker.Format.Fill.Solid(Color.Yellow);
            open.Marker.Format.Stroke.Visible = false;
            close.Marker.Symbol = MarkerSymbol.Dash;
            close.Marker.Size = 20;
            close.Marker.Format.Fill.Solid(Color.Red);
            close.Marker.Format.Stroke.Visible = false;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestStockComboChart.docx");
        }

        /// <summary>
        /// Tests saving a chart with series groups that have no series, and then using it as a template for
        /// a combo chart.
        /// </summary>
        [Test]
        public void TestSavingSeriesGroupWithoutSeries()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;

            ChartSeriesCollection series = chart.Series;
            series.Clear();

            string[] categories = new string[] { "Category 1", "Category 2", "Category 3" };
            series.Add("Series 1", categories, new double[] { 2, 3, 4 });
            series.Add("Series 2", categories, new double[] { 5, 2, 3 });

            ChartSeriesGroup lineSeriesGroup = chart.SeriesGroups.Add(ChartSeriesType.Line);
            Assert.That(lineSeriesGroup.AxisGroup, Is.EqualTo(AxisGroup.Primary));
            ChartSeriesGroup scatterSeriesGroup = chart.SeriesGroups.Add(ChartSeriesType.Scatter);
            // Because column and line series have category X values, but a scatter series has numeric X values, the
            // added scatter series group is automatically linked to the secondary axis group like MS Word does.
            Assert.That(scatterSeriesGroup.AxisGroup, Is.EqualTo(AxisGroup.Secondary));

            const string fileName = @"Model\Charts\TestSavingSeriesGroupWithoutSeries.docx";
            doc = TestUtil.SaveOpen(doc, fileName, null, false);

            chart = doc.FirstSection.Body.Shapes[0].Chart;
            Assert.That(chart.SeriesGroups[1].SeriesType, Is.EqualTo(ChartSeriesType.Line));
            chart.SeriesGroups[1].Series.Add("Line Group Series", categories, new double[] { 7, 5, 8 });
            Assert.That(chart.SeriesGroups[2].SeriesType, Is.EqualTo(ChartSeriesType.Scatter));
            chart.SeriesGroups[2].Series.Add("Scatter Group Series",
                new double[] { 10, 50, 100 }, new double[] { 17, 25, 8 });

            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }





        /// <summary>
        /// Tests creating a combo chart that contains a series group which has no axes (pie series group).
        /// </summary>
        [Test]
        public void TestComboChartWithNoAxesSeriesGroup()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape1 = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart1 = shape1.Chart;

            string[] categories = new string[] { "Category 1", "Category 2", "Category 3" };
            double[] values = new double[] { 7, 3, 4 };

            chart1.Series.Clear();
            ChartSeries columnSeries1 = chart1.Series.Add("Series 1", categories, values);
            columnSeries1.Format.Stroke.Color = Color.White;
            columnSeries1.Format.Fill.Color = Color.FromArgb(0xffc000);

            ChartSeriesGroup pieSeriesGroup = chart1.SeriesGroups.Add(ChartSeriesType.Pie);
            pieSeriesGroup.Series.Add("Series 2", categories, values);
            Assert.That(pieSeriesGroup.AxisGroup, Is.EqualTo(AxisGroup.Primary));

            // And do the same with adding series groups in reverse order.

            Shape shape2 = builder.InsertChart(ChartType.Pie, 450, 250);
            Chart chart2 = shape2.Chart;

            chart2.Series.Clear();
            chart2.Series.Add("Series 1", categories, values);

            ChartSeriesGroup columnSeriesGroup = chart2.SeriesGroups.Add(ChartSeriesType.Column);
            ChartSeries columnSeries2 = columnSeriesGroup.Series.Add("Series 2", categories, values);
            columnSeries2.Format.Stroke.Color = Color.White;
            columnSeries2.Format.Fill.Color = Color.FromArgb(0xffc000);
            Assert.That(columnSeriesGroup.AxisGroup, Is.EqualTo(AxisGroup.Primary));

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestComboChartWithPieSeriesGroup.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartSeriesGroup.GapWidth"/> and <see cref="ChartSeriesGroup.Overlap"/> properties
        /// in created pre-Word 2016 charts.
        /// </summary>
        [Test]
        public void TestSeriesGroupGapWidthAndOverlapInNewChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Column chart.
            Shape columnShape = builder.InsertChart(ChartType.Column, 450, 250);
            ChartSeriesGroup columnSeriesGroup = columnShape.Chart.SeriesGroups[0];

            Assert.That(columnSeriesGroup.GapWidth, Is.EqualTo(219));
            Assert.That(columnSeriesGroup.Overlap, Is.EqualTo(-27));

            columnSeriesGroup.GapWidth = 100;
            columnSeriesGroup.Overlap = 10;

            // Bar chart.
            Shape barShape = builder.InsertChart(ChartType.Bar, 450, 250);
            ChartSeriesGroup barSeriesGroup = barShape.Chart.SeriesGroups[0];

            Assert.That(barSeriesGroup.GapWidth, Is.EqualTo(182));
            Assert.That(barSeriesGroup.Overlap, Is.EqualTo(0));

            barSeriesGroup.GapWidth = 100;
            barSeriesGroup.Overlap = -10;

            // Pie-of-pie chart.
            Shape ofPieShape = builder.InsertChart(ChartType.PieOfPie, 450, 250);
            ChartSeriesGroup ofPieSeriesGroup = ofPieShape.Chart.SeriesGroups[0];

            Assert.That(ofPieSeriesGroup.GapWidth, Is.EqualTo(100));

            ofPieSeriesGroup.GapWidth = 50;

            // Pie-of-bar chart.
            Shape ofBarShape = builder.InsertChart(ChartType.PieOfBar, 450, 250);
            ChartSeriesGroup ofBarSeriesGroup = ofBarShape.Chart.SeriesGroups[0];

            Assert.That(ofBarSeriesGroup.GapWidth, Is.EqualTo(100));

            ofBarSeriesGroup.GapWidth = 200;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestSeriesGroupGapWidthAndOverlap.docx");
        }


        /// <summary>
        /// Tests the <see cref="ChartSeriesGroup.BubbleScale"/> property.
        /// </summary>
        [Test]
        public void TestSeriesGroupBubbleScale()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Bubble3D, 450, 250);
            ChartSeriesGroup seriesGroup = shape.Chart.SeriesGroups[0];

            Assert.That(seriesGroup.BubbleScale, Is.EqualTo(100));

            seriesGroup.BubbleScale = 10;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestSeriesGroupBubbleScale.docx");
        }

        /// <summary>
        /// Tests that an exception occurs when the <see cref="ChartSeriesGroup.GapWidth"/> property is set in a series
        /// group with an unsupported series type.
        /// </summary>
        [TestCase(@"Word2016Charts\Treemap.docx")]
        [TestCase(@"TestLineChart.docx")]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The property cannot be set for a series group of this type.")]
        public void TestGapWidthInUnsupportedSeriesGroup(string fileName)
        {
            Document doc = TestUtil.Open(@"Model\Charts\" + fileName);
            ChartSeriesGroup seriesGroup = doc.FirstSection.Body.Shapes[0].Chart.SeriesGroups[0];
            seriesGroup.GapWidth = 100;
        }

        /// <summary>
        /// Tests that an exception occurs when the <see cref="ChartSeriesGroup.Overlap"/> property is set in a series
        /// group with an unsupported series type.
        /// </summary>
        [TestCase(@"Word2016Charts\Treemap.docx")]
        [TestCase(@"TestLineChart.docx")]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The property cannot be set for a series group of this type.")]
        public void TestOverlapInUnsupportedSeriesGroup(string fileName)
        {
            Document doc = TestUtil.Open(@"Model\Charts\" + fileName);
            ChartSeriesGroup seriesGroup = doc.FirstSection.Body.Shapes[0].Chart.SeriesGroups[0];
            seriesGroup.Overlap = 15;
        }

        /// <summary>
        /// Tests that an exception occurs when the <see cref="ChartSeriesGroup.BubbleScale"/> property is set in a series
        /// group with an unsupported series type.
        /// </summary>
        [TestCase(@"Word2016Charts\Treemap.docx")]
        [TestCase(@"TestLineChart.docx")]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The property cannot be set for a series group of this type.")]
        public void TestBubbleScaleInUnsupportedSeriesGroup(string fileName)
        {
            Document doc = TestUtil.Open(@"Model\Charts\" + fileName);
            ChartSeriesGroup seriesGroup = doc.FirstSection.Body.Shapes[0].Chart.SeriesGroups[0];
            seriesGroup.BubbleScale = 15;
        }

        /// <summary>
        /// Tests that an exception occurs when the <see cref="ChartSeriesGroup.GapWidth"/> property is set to a value
        /// outside the allowable range.
        /// </summary>
        [TestCase(-1)]
        [TestCase(501)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGapWidthValueRange(int gapWidth)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            ChartSeriesGroup seriesGroup = shape.Chart.SeriesGroups[0];
            seriesGroup.GapWidth = gapWidth;
        }

        /// <summary>
        /// Tests that an exception occurs when the <see cref="ChartSeriesGroup.Overlap"/> property is set to a value
        /// outside the allowable range.
        /// </summary>
        [TestCase(-101)]
        [TestCase(101)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestOverlapValueRange(int overlap)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            ChartSeriesGroup seriesGroup = shape.Chart.SeriesGroups[0];
            seriesGroup.Overlap = overlap;
        }

        /// <summary>
        /// Tests that an exception occurs when the <see cref="ChartSeriesGroup.BubbleScale"/> property is set to a value
        /// outside the allowable range.
        /// </summary>
        [TestCase(-1)]
        [TestCase(301)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestBubbleScaleValueRange(int bubbleScale)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Bubble3D, 450, 250);
            ChartSeriesGroup seriesGroup = shape.Chart.SeriesGroups[0];
            seriesGroup.BubbleScale = bubbleScale;
        }

        /// <summary>
        /// Tests that an exception occurs when adding a new series group to a Word 2016 chart. Word 2016 charts do not
        /// support combo charts and multiple series groups.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "You cannot add series groups to charts of this type.")]
        public void TestAddingChartGroupToWord2016Chart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\BoxWhisker.docx");
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            chart.SeriesGroups.Add(ChartSeriesType.Column);
        }

        /// <summary>
        /// Tests that an exception occurs when adding a new series group to a 3D chart. 3D charts do not support combo
        /// charts and multiple series groups.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "You cannot add series groups to charts of this type.")]
        public void TestAddingSeriesGroupTo3dChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column3D, 450, 250);
            Chart chart = shape.Chart;
            chart.SeriesGroups.Add(ChartSeriesType.Column);
        }

        /// <summary>
        /// Tests that an exception occurs when adding a new series group with a series type that is not supported by
        /// combo charts.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "You cannot add series groups of this type.")]
        public void TestAdding3dSeriesGroup()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;
            chart.SeriesGroups.Add(ChartSeriesType.Column3D);
        }

        /// <summary>
        /// Tests that an exception occurs when removing all series groups from a chart.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "There must be at least one series group in a chart.")]
        public void TestRemovingAllChartGroups()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;
            chart.SeriesGroups.RemoveAt(0);
        }

        /// <summary>
        /// Tests that an exception occurs when changing the <see cref="ChartSeriesGroup.AxisGroup"/> property in a
        /// non-combo chart.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "Only a combo chart, that is, a chart with multiple series groups can have secondary axes.")]
        public void TestChangingAxisGroupInNonComboChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;
            chart.SeriesGroups[0].AxisGroup = AxisGroup.Secondary;
        }

        /// <summary>
        /// Tests that an exception occurs when changing the <see cref="ChartSeriesGroup.AxisGroup"/> property in a
        /// combo chart, in which only one series group has axes.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage =
            "You cannot create secondary axes for a combo chart with a single series group that can have axes.")]
        public void TestChangingAxisGroupWithSingleSeriesGroupWithAxes()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;
            chart.SeriesGroups.Add(ChartSeriesType.Pie);
            chart.SeriesGroups[0].AxisGroup = AxisGroup.Secondary;
        }

        /// <summary>
        /// Tests that an exception occurs when changing the <see cref="ChartSeriesGroup.AxisGroup"/> property in a
        /// series group that has no axes.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "Axes are not supported by this type of series group.")]
        public void TestChangingAxisGroupInSeriesGroupWithoutAxes()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 450, 250);
            Chart chart = shape.Chart;
            chart.SeriesGroups.Add(ChartSeriesType.Line);
            ChartSeriesGroup pieSeriesGroup = chart.SeriesGroups.Add(ChartSeriesType.Pie);
            pieSeriesGroup.AxisGroup = AxisGroup.Secondary;
        }

        /// <summary>
        /// WORDSNET-17134 Manipulation of Doughnut Chart Style
        /// New properties have been added to the <see cref="ChartSeriesGroup"/> class to define pie chart style.
        /// </summary>
        [Test]
        public void TestPieChartStyle()
        {
            const string fileName = @"Model\Charts\TestPieChartStyle";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            ChartSeriesGroup seriesGroup1 = shapes[0].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup1, 20, 120, -1, -1);

            ChartSeriesGroup seriesGroup2 = shapes[1].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup2, 4, 3, -1, -1);

            ChartSeriesGroup seriesGroup3 = shapes[2].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup3, -1, 10, 90, -1);

            ChartSeriesGroup seriesGroup4 = shapes[3].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup4, 5, -1, -1, 125);

            ChartSeriesGroup seriesGroup5 = shapes[4].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup5, -1, -1, -1, 95);

            // Update the charts.
            ChangePieChartProperties(seriesGroup1, 0, 270, -1, -1);
            ChangePieChartProperties(seriesGroup2, 20, 90, -1, -1);
            ChangePieChartProperties(seriesGroup3, 5, 0, 50, -1);
            ChangePieChartProperties(seriesGroup4, 80, -1, -1, 20);
            ChangePieChartProperties(seriesGroup5, 40, -1, -1, 50);

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            shapes = doc.FirstSection.Body.Shapes;

            seriesGroup1 = shapes[0].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup1, 0, 270, -1, -1);

            seriesGroup2 = shapes[1].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup2, 20, 90, -1, -1);

            seriesGroup3 = shapes[2].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup3, 5, 0, 50, -1);

            seriesGroup4 = shapes[3].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup4, 80, -1, -1, 20);

            seriesGroup5 = shapes[4].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup5, 40, -1, -1, 50);
        }

        /// <summary>
        /// Tests defining style of new pie charts.
        /// </summary>
        [Test]
        public void TestStyleOfNewPieChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Pie, 450, 250);
            ChartSeriesGroup seriesGroup1 = shape.Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup1, -1, 0, -1, -1);

            shape = builder.InsertChart(ChartType.Pie3D, 450, 250);
            ChartSeriesGroup seriesGroup2 = shape.Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup2, -1, 0, -1, -1);

            shape = builder.InsertChart(ChartType.Doughnut, 450, 250);
            ChartSeriesGroup seriesGroup3 = shape.Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup3, -1, 0, 75, -1);

            shape = builder.InsertChart(ChartType.PieOfPie, 450, 250);
            ChartSeriesGroup seriesGroup4 = shape.Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup4, -1, -1, -1, 75);

            shape = builder.InsertChart(ChartType.PieOfBar, 450, 250);
            ChartSeriesGroup seriesGroup5 = shape.Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup5, -1, -1, -1, 75);

            // Update the charts.
            ChangePieChartProperties(seriesGroup1, 1, 90, -1, -1);
            ChangePieChartProperties(seriesGroup2, 10, 20, -1, -1);
            ChangePieChartProperties(seriesGroup3, 0, 180, 10, -1);
            ChangePieChartProperties(seriesGroup4, 0, -1, -1, 150);
            ChangePieChartProperties(seriesGroup5, 4, -1, -1, 50);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestStyleOfNewPieChart", UnifiedScenario.Docx2DocxNoGold);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            seriesGroup1 = shapes[0].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup1, 1, 90, -1, -1);

            seriesGroup2 = shapes[1].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup2, 10, 20, -1, -1);

            seriesGroup3 = shapes[2].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup3, 0, 180, 10, -1);

            seriesGroup4 = shapes[3].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup4, 0, -1, -1, 150);

            seriesGroup5 = shapes[4].Chart.SeriesGroups[0];
            CheckPieChartProperties(seriesGroup5, 4, -1, -1, 50);
        }

        /// <summary>
        /// Checks pie chart specific properties.
        /// </summary>
        private static void CheckPieChartProperties(ChartSeriesGroup seriesGroup, int expectedExplosion,
            int expectedFirstSliceAngle, int expectedDoughnutHoleSize, int expectedSecondSectionSize)
        {
            Assert.That(seriesGroup.Series[0].Explosion, Is.EqualTo(expectedExplosion));

            if (expectedFirstSliceAngle >= 0)
                Assert.That(seriesGroup.FirstSliceAngle, Is.EqualTo(expectedFirstSliceAngle));

            if (expectedDoughnutHoleSize >= 0)
                Assert.That(seriesGroup.DoughnutHoleSize, Is.EqualTo(expectedDoughnutHoleSize));

            if (expectedSecondSectionSize >= 0)
                Assert.That(seriesGroup.SecondSectionSize, Is.EqualTo(expectedSecondSectionSize));
        }

        /// <summary>
        /// Changes pie chart specific properties.
        /// </summary>
        private static void ChangePieChartProperties(ChartSeriesGroup seriesGroup, int explosion,
            int firstSliceAngle, int doughnutHoleSize, int secondSectionSize)
        {
            seriesGroup.Series[0].Explosion = explosion;

            if (firstSliceAngle >= 0)
                seriesGroup.FirstSliceAngle = firstSliceAngle;

            if (doughnutHoleSize >= 0)
                seriesGroup.DoughnutHoleSize = doughnutHoleSize;

            if (secondSectionSize >= 0)
                seriesGroup.SecondSectionSize = secondSectionSize;
        }
    }
}
