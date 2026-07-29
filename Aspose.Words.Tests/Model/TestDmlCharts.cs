// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/25/2014 by Alexey Noskov

using System.IO;
using Aspose.Common;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Simple open/save/compare tests for DOCX document with charts.
    /// Used to make sure charts are written properly from the model.
    /// Use test documents from charts rendering tests, i.e. tests different types of charts.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class TestDmlCharts
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

        [Test]
        public void TestAllStylesBarChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAllStylesBarChart.docx");
        }

        [Test]
        public void TestStylesPieChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestStylesPieChart.docx");
        }

        [Test]
        public void TestPieChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestPieChart.docx");
        }

        [Test]
        public void TestScatterChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestScatterChart.docx");
        }

        [Test]
        public void TestLineChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLineChart.docx");
        }

        [Test]
        public void TestAreaChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAreaChart.docx");
        }

        [Test]
        public void TestBubbleChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestBubbleChart.docx");
        }

        [Test]
        public void TestStockChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestStockChart.docx");
        }

        [Test]
        public void TestRadarChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestRadarChart.docx");
        }

        [Test]
        public void TestAxisDelete()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisDelete.docx");
        }

        [Test]
        public void TestMarkers()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestMarkers.docx");
        }

        [Test]
        public void TestSurfaceChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestSurfaceChart.docx");
        }

        [Test]
        public void Test3DBarCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarCharts.docx");
        }

        [Test]
        public void Test3DBarChartsCone()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarChartsCone.docx");
        }

        [Test]
        public void Test3DBarChartsCylinder()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarChartsCylinder.docx");
        }

        [Test]
        public void Test3DBarChartsPyramid()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DBarChartsPyramid.docx");
        }

        [Test]
        public void Test3DColumnCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DColumnCharts.docx");
        }

        [Test]
        public void Test3DColumnChartsCone()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DColumnChartsCone.docx");
        }

        [Test]
        public void Test3DColumnChartsCylinder()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DColumnChartsCylinder.docx");
        }

        [Test]
        public void Test3DColumnChartsPyramid()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DColumnChartsPyramid.docx");
        }

        [Test]
        public void Test3DLineCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DLineCharts.docx");
        }

        [Test]
        public void Test3DAreaCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DAreaCharts.docx");
        }

        [Test]
        public void Test3DPieCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DPieCharts.docx");
        }

        [Test]
        public void Test3DSurfaceChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DSurfaceChart.docx");
        }

        [Test]
        public void TestSurfaceChartSegments()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestSurfaceChartSegments.docx");
        }

        [Test]
        public void Test3DSurfaceChartSegments()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Test3DSurfaceChartSegments.docx");
        }

        [Test]
        public void TestOfPieChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestOfPieChart.docx");
        }

        [Test]
        public void TestOfPieChartValue()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestOfPieChartValue.docx");
        }

        [Test]
        public void TestOfPieChartPercentage()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestOfPieChartPercentage.docx");
        }

        [Test]
        public void TestOfPieChartCustom()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestOfPieChartCustom.docx");
        }

        /// <summary>
        /// WORDSNET-16069 Incorrect ChartDataPoint Cloning.
        /// </summary>
        /// <remarks>
        /// After cloning a DmlChart object, the clone (more precisely, objects of which DmlChart consists of) had
        /// references to internal objects of the original DmlChart object. For example, after cloning, a ChartDataPoint
        /// object of the clone had their SpPr properties referenced to the same DmlChartSpPr object.
        /// Implemented missing cloning of internal objects.
        /// </remarks>
        [Test]
        public void TestJira16069()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestJira16069.docx");
            DmlChart chart = doc.FirstSection.Body.Shapes[0].Chart.ChartSpace.FirstChart;
            ChartSeries series = chart.Series[0];
            series.AddErrorBars(new DmlChartErrorBars());
            series.Tx.Formula = new DmlChartFormula();

            DmlChart clonedChart = chart.Clone();
            ChartSeries clonedSeries = (ChartSeries)clonedChart.Series[0];

            Assert.That(series, IsNot.SameAs(clonedSeries));
            Assert.That(clonedChart, Is.SameAs(clonedSeries.Chart));
            Assert.That(chart.DataLabels, IsNot.SameAs(clonedChart.DataLabels));
            Assert.That(clonedChart, Is.SameAs(clonedChart.DataLabels.Chart));
            DmlChartPr chartPr = clonedChart.ChartPr;
            Assert.That(chart.ChartPr.GetProperty(DmlChartAttrs.DropLines), IsNot.SameAs(chartPr.GetProperty(DmlChartAttrs.DropLines)));
            Assert.That(chart.ChartPr.GetProperty(DmlChartAttrs.BandFmts), IsNot.SameAs(chartPr.GetProperty(DmlChartAttrs.BandFmts)));

            Assert.That(clonedChart, Is.SameAs(clonedSeries.DefaultDataPoint.Chart));
            Assert.That(clonedSeries, Is.SameAs(clonedSeries.DataPoints.Series));
            Assert.That(clonedChart, Is.SameAs(clonedSeries.DataPoints[0].Chart));
            Assert.That(series.DataPoints[0].SpPr, IsNot.SameAs(clonedSeries.DataPoints[0].SpPr));
            Assert.That(clonedSeries, Is.SameAs(clonedSeries.YErrorBars.Series));
            Assert.That(series.Tx.Formula, IsNot.SameAs(clonedSeries.Tx.Formula));
            Assert.That(clonedChart, Is.SameAs(clonedSeries.DataLabels.Chart));

            ChartDataLabel dataLabel = clonedSeries.DataLabels[0];
            Assert.That(clonedChart, Is.SameAs(dataLabel.Chart));
            Assert.That(series.DataLabels[0].SpPr, IsNot.SameAs(dataLabel.SpPr));
            Assert.That(series.DataLabels[0].TxPr, IsNot.SameAs(dataLabel.TxPr));

            DmlChartTrendline trendLine = (DmlChartTrendline)series.Trendlines[0];
            DmlChartTrendline clonedTrendLine = (DmlChartTrendline)clonedSeries.Trendlines[0];
            Assert.That(trendLine.TrendlinePr, IsNot.SameAs(clonedTrendLine.TrendlinePr));
            Assert.That(trendLine.SpPr, IsNot.SameAs(clonedTrendLine.SpPr));
            Assert.That(trendLine.TrendlineLbl, IsNot.SameAs(clonedTrendLine.TrendlineLbl));
        }






        /// <summary>
        /// Tests default values of <see cref="Font"/> properties of chart elements.
        /// </summary>
        [Test]
        public void TestChartFontDefaults()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestChartFontDefaults.docx");

            // Pre-Word 2016 chart.
            Chart chart1 = doc.FirstSection.Body.Shapes[0].Chart;
            CheckFont(chart1.Title.Font, 18, true);
            CheckFont(chart1.AxisX.Title.Font, 10, true);
            CheckFont(chart1.AxisX.TickLabels.Font, 10, false);
            CheckFont(chart1.AxisY.DisplayUnit.Label.Font, 10, true);
            CheckFont(chart1.Series[0].DataLabels.Font, 10, false);
            CheckFont(chart1.Legend.Font, 10, false);

            // Word 2016 chart.
            Chart chart2 = doc.FirstSection.Body.Shapes[1].Chart;
            CheckFont(chart2.Title.Font, 14, false);
            CheckFont(chart2.AxisX.Title.Font, 9, false);
            CheckFont(chart2.AxisX.TickLabels.Font, 9, false);
            CheckFont(chart2.AxisY.DisplayUnit.Label.Font, 9, false);
            CheckFont(chart2.Series[0].DataLabels.Font, 9, false);
            CheckFont(chart2.Legend.Font, 9, false);
        }

        /// <summary>
        /// Checks some properties of the specified <see cref="Font"/> instance.
        /// </summary>
        private void CheckFont(Font font, int expectedSize, bool expectedBold)
        {
            Assert.That(font.Size, Is.EqualTo(expectedSize));
            Assert.That(font.Bold, Is.EqualTo(expectedBold));
        }
    }
}
