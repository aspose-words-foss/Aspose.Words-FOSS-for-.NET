// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2023 by Alexander Zhiltsov

using System;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests behavior of the <see cref="ChartSeries"/> class.
    /// </summary>
    [TestFixture]
    public class TestChartSeries
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

        [TestCase(ChartType.Area, ChartSeriesType.Area)]
        [TestCase(ChartType.AreaStacked, ChartSeriesType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked, ChartSeriesType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D, ChartSeriesType.Area3D)]
        [TestCase(ChartType.Area3DStacked, ChartSeriesType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked, ChartSeriesType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar, ChartSeriesType.Bar)]
        [TestCase(ChartType.BarStacked, ChartSeriesType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked, ChartSeriesType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D, ChartSeriesType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked, ChartSeriesType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked, ChartSeriesType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble, ChartSeriesType.Bubble)]
        [TestCase(ChartType.Bubble3D, ChartSeriesType.Bubble3D)]
        [TestCase(ChartType.Column, ChartSeriesType.Column)]
        [TestCase(ChartType.ColumnStacked, ChartSeriesType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked, ChartSeriesType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D, ChartSeriesType.Column3D)]
        [TestCase(ChartType.Column3DStacked, ChartSeriesType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked, ChartSeriesType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered, ChartSeriesType.Column3DClustered)]
        [TestCase(ChartType.Doughnut, ChartSeriesType.Doughnut)]
        [TestCase(ChartType.LineStacked, ChartSeriesType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked, ChartSeriesType.LinePercentStacked)]
        [TestCase(ChartType.Line3D, ChartSeriesType.Line3D)]
        [TestCase(ChartType.Pie, ChartSeriesType.Pie)]
        [TestCase(ChartType.Pie3D, ChartSeriesType.Pie3D)]
        [TestCase(ChartType.PieOfBar, ChartSeriesType.PieOfBar)]
        [TestCase(ChartType.PieOfPie, ChartSeriesType.PieOfPie)]
        [TestCase(ChartType.Radar, ChartSeriesType.Radar)]
        [TestCase(ChartType.Scatter, ChartSeriesType.Scatter)]
        [TestCase(ChartType.Stock, ChartSeriesType.Stock)]
        [TestCase(ChartType.Surface, ChartSeriesType.Surface)]
        [TestCase(ChartType.Surface3D, ChartSeriesType.Surface3D)]
        public void TestNonWord2016ChartSeriesType(ChartType chartType, ChartSeriesType expectedSeriesType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);

            foreach (ChartSeries series in shape.Chart.Series)
                Assert.That(series.SeriesType, Is.EqualTo(expectedSeriesType));
        }


        [Test]
        public void TestParetoChartSeriesType()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];
            ChartSeriesCollection seriesCollection = shape.Chart.Series;

            Assert.That(seriesCollection.Count, Is.EqualTo(3));
            Assert.That(seriesCollection[0].SeriesType, Is.EqualTo(ChartSeriesType.ParetoLine));
            Assert.That(seriesCollection[1].SeriesType, Is.EqualTo(ChartSeriesType.Pareto));
            Assert.That(seriesCollection[2].SeriesType, Is.EqualTo(ChartSeriesType.ParetoLine));
        }


        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, string[], double[])"/> for all chart types
        /// with ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithStringX()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
                chart.Series.Clear();

                chart.Series.Add(
                    "Series 1",
                    new string[] { "Category 1", "Category 2", "Category 3", "Category 4" },
                    new double[] { 10, 7, 12, 15 });
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithStringX.docx");
        }

        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, double[], double[])"/> for all chart types
        /// with ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithDoubleX()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                    foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
                {
                    Shape shape = builder.InsertChart(chartType, 432, 252);
                    Chart chart = shape.Chart;

                    chart.Title.Text = chartType.ToString();
                    chart.Series.Clear();

                    chart.Series.Add(
                        "Series 1",
                        new double[] { 1, 2.5, 4, 5.5 },
                        new double[] { 10, 7, 12, 15 });
                }

              TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithDoubleX.docx");
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, DateTime[], double[])"/> for all chart types
        /// with ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithDateTimeX()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            SystemPal.SaveCulture();

            try
            {
                SystemPal.SetStandardCulture();

                foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
                {
                    Shape shape = builder.InsertChart(chartType, 432, 252);
                    Chart chart = shape.Chart;

                    chart.Title.Text = chartType.ToString();
                    chart.Series.Clear();

                    chart.Series.Add(
                        "Series 1",
                        new DateTime[] { new DateTime(2020, 1, 1), new DateTime(2020, 3, 1),
                            new DateTime(2020, 6, 1), new DateTime(2020, 12, 1) },
                        new double[] { 10, 7, 12, 15 });
                }

                TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithDateTimeX.docx");
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, double[], double[], double[])"/> for all chart
        /// types with ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithBubbleSize()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
                chart.Series.Clear();

                chart.Series.Add(
                    "Series 1",
                    new double[] { 1, 2.5, 3.5, 5 },
                    new double[] { 10, 7, 12, 15 },
                    new double[] { 1, 0.75, 1.25, 2 });
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithBubbleSize.docx");
        }

        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, ChartMultilevelValue[], double[])"/> for all
        /// chart types with ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithMultilevelX()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
                chart.Series.Clear();

                ChartSeries series = chart.Series.Add(
                    "Series 1",
                    new ChartMultilevelValue[]
                    {
                        new ChartMultilevelValue("Branch 1", "Stem 1", "Leaf 1"),
                        new ChartMultilevelValue("Branch 1", "Stem 1", "Leaf 2"),
                        new ChartMultilevelValue("Branch 1", "Stem 1", "Leaf 3"),
                        new ChartMultilevelValue("Branch 1", "Stem 2", "Leaf 4"),
                        new ChartMultilevelValue("Branch 2", "Stem 3", "Leaf 5"),
                        new ChartMultilevelValue("Branch 2", "Stem 3", "Leaf 6"),
                    },
                    new double[] { 10, 7, 12, 15, 14, 11 });

                series.HasDataLabels = true;
                series.DataLabels.ShowValue = true;
                if ((chartType == ChartType.Treemap) || (chartType == ChartType.Sunburst))
                    series.DataLabels.ShowCategoryName = true;
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithMultilevelX.docx");
        }

        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, double[])"/> for all chart types with
        /// ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithoutY()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
                chart.Series.Clear();

                chart.Series.Add("Series 1", new double[] { 10, 7, 12, 15, 14, 11 });
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithoutY.docx");
        }

        /// <summary>
        /// Tests the method <see cref="ChartSeriesCollection.Add(string, string[], double[], bool[])"/> for all chart
        /// types with ensuring that no exception is generated and the document can be open in MS Word.
        /// </summary>
        [Test]
        public void TestAddingSeriesWithIsSubtotal()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
                chart.Series.Clear();

                chart.Series.Add(
                    "Series 1",
                    new string[] { "Start Total", "Category 1", "Category 2", "Category 3", "End Total" },
                    new double[] { 100, 7, -12, 15, 110 },
                    new bool[] { true, false, false, false, true });
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAddingSeriesWithIsSubtotal.docx");
        }
    }
}
