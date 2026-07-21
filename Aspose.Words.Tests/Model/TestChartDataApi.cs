// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2022 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests Chart Data API: <see cref="ChartSeries.XValues"/>, <see cref="ChartSeries.YValues"/>,
    /// <see cref="ChartSeries.BubbleSizes"/> and other members and classes.
    /// </summary>
    [TestFixture]
    public class TestChartDataApi
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
            // Set default culture for tests to make tests independent on the current culture.
            // Required because most of chart tests uses string formatting, which depends on culture. 
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void TearDown()
        {
            SystemPal.RestoreCulture();
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using X values of the <see cref="ChartXValueType.Double"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithNumericX(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();

                for (int j = 1; j <= 5; j++)
                    series.Add(ChartXValue.FromDouble(j), ChartYValue.FromDouble(j + (chart.Series.Count - 1 - i)), j + 1);
            }

            string fileName = string.Format(NumericXTestFileNameTemplate, chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using X values of the <see cref="ChartXValueType.String"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithStringX(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();

                for (int j = 1; j <= 5; j++)
                {
                    int yValue = j + (chart.Series.Count - 1 - i);
                    series.Add(ChartXValue.FromString("Category " + j), ChartYValue.FromDouble(yValue), j + 1);
                }
            }

            string fileName = string.Format(@"Model\Charts\DataApi\TestFillData_{0}_StringX.docx", chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using X values of the <see cref="ChartXValueType.DateTime"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithDateTimeX(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();

                for (int j = 1; j <= 5; j++)
                {
                    int yValue = j + (chart.Series.Count - 1 - i);
                    series.Add(ChartXValue.FromDateTime(new DateTime(2022, j, 1)), ChartYValue.FromDouble(yValue), j + 1);
                }
            }

            string fileName = string.Format(@"Model\Charts\DataApi\TestFillData_{0}_DateTimeX.docx", chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests specifying datetime X and Y values containing milliseconds.
        /// </summary>
        [Test]
        public void TestDateTimeWithMilliseconds()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Scatter, 432, 252);
            Chart chart = shape.Chart;

            foreach (ChartSeries series in chart.Series)
            {
                series.Clear();

                for (int j = 1; j <= 5; j++)
                {
                    series.Add(
                        ChartXValue.FromDateTime(new DateTime(2023, 1, j, 0, 0, 0, 700)),
                        ChartYValue.FromDateTime(new DateTime(2023, 1, j, 0, 0, 0, 700)));
                }
            }

            builder.Writeln();
            shape = builder.InsertChart(ChartType.Scatter, 432, 252);
            chart = shape.Chart;

            foreach (ChartSeries series in chart.Series)
            {
                series.Clear();

                for (int j = 1; j <= 5; j++)
                {
                    series.Add(
                        ChartXValue.FromTimeSpan(new TimeSpan(0, 0, 0, j, 400)),
                        ChartYValue.FromTimeSpan(new TimeSpan(0, 0, 0, j, 600)));
                }
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\TestDateTimeWithMilliseconds.docx");
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using X values of the <see cref="ChartXValueType.Time"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithTimeX(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();

                for (int j = 1; j <= 5; j++)
                {
                    int yValue = j + (chart.Series.Count - 1 - i);
                    series.Add(ChartXValue.FromTimeSpan(new TimeSpan(9 + j, 0, 0)), ChartYValue.FromDouble(yValue), j + 1);
                }
            }

            string fileName = string.Format(@"Model\Charts\DataApi\TestFillData_{0}_TimeX.docx", chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using X values of the <see cref="ChartXValueType.Multilevel"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithMultilevelX(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();

                for (int j = 1; j <= 5; j++)
                {
                    int yValue = j + (chart.Series.Count - 1 - i);
                    series.Add(
                        ChartXValue.FromMultilevelValue(new ChartMultilevelValue(
                            (j <= 3) ? "First Part" : "Last Part",
                            ((j - 1 & 2) == 0) ? "Group 1" : "Group 2",
                            "Category " + j)),
                        ChartYValue.FromDouble(yValue),
                        j + 1);
                }
            }

            string fileName = string.Format(@"Model\Charts\DataApi\TestFillData_{0}_MultilevelX.docx", chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests that a warning is generated when adding chart values including bubble sizes if a series does not
        /// support bubble sizes. And that there is no warning if the series supports them.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestBubbleSizeWarnings(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            TestWarningCallback warningCallback = new TestWarningCallback();
            doc.WarningCallback = warningCallback;

            const int valueCount = 5;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();

                for (int j = 1; j <= valueCount; j++)
                    series.Add(ChartXValue.FromDouble(j), ChartYValue.FromDouble(j + (chart.Series.Count - 1 - i)), j + 1);
            }

            if (chart.ChartSpace.FirstChart.ChartType != DmlChartType.BubbleChart)
            {
                Assert.That(warningCallback.Count, Is.EqualTo(valueCount * chart.Series.Count));
                for (int i = 0; i < warningCallback.Count; i++)
                    Assert.That(warningCallback[i].Description, Is.EqualTo(WarningStrings.CannotSetBubbleSize));
            }
            else
            {
                Assert.That(warningCallback.Count, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using Y values of the <see cref="ChartYValueType.DateTime"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithDateTimeY(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();
                series.HasDataLabels = true;
                series.DataLabels.ShowValue = true;

                for (int j = 1; j <= 5; j++)
                {
                    int month = j + (chart.Series.Count - 1 - i);
                    series.Add(
                        ChartXValue.FromString("Category " + j),
                        ChartYValue.FromDateTime(new DateTime(2022, month, 1)),
                        j + 1);
                }
            }

            // When opening the output file in MS Word and select Edit Data, datetime values are displayed as numeric:
            // Looks like MS Word doesn't use format code when generates Y data in internal XLSX.

            string fileName = string.Format(@"Model\Charts\DataApi\TestFillData_{0}_DateTimeY.docx", chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests filling data to a <see cref="ChartSeries"/> using Y values of the <see cref="ChartYValueType.Time"/>
        /// type.
        /// </summary>
        [TestCase(ChartType.Area)]
        [TestCase(ChartType.AreaStacked)]
        [TestCase(ChartType.AreaPercentStacked)]
        [TestCase(ChartType.Area3D)]
        [TestCase(ChartType.Area3DStacked)]
        [TestCase(ChartType.Area3DPercentStacked)]
        [TestCase(ChartType.Bar)]
        [TestCase(ChartType.BarStacked)]
        [TestCase(ChartType.BarPercentStacked)]
        [TestCase(ChartType.Bar3D)]
        [TestCase(ChartType.Bar3DStacked)]
        [TestCase(ChartType.Bar3DPercentStacked)]
        [TestCase(ChartType.Bubble)]
        [TestCase(ChartType.Bubble3D)]
        [TestCase(ChartType.Column)]
        [TestCase(ChartType.ColumnStacked)]
        [TestCase(ChartType.ColumnPercentStacked)]
        [TestCase(ChartType.Column3D)]
        [TestCase(ChartType.Column3DStacked)]
        [TestCase(ChartType.Column3DPercentStacked)]
        [TestCase(ChartType.Column3DClustered)]
        [TestCase(ChartType.Doughnut)]
        [TestCase(ChartType.Line)]
        [TestCase(ChartType.LineStacked)]
        [TestCase(ChartType.LinePercentStacked)]
        [TestCase(ChartType.Line3D)]
        [TestCase(ChartType.Pie)]
        [TestCase(ChartType.Pie3D)]
        [TestCase(ChartType.PieOfBar)]
        [TestCase(ChartType.PieOfPie)]
        [TestCase(ChartType.Radar)]
        [TestCase(ChartType.Scatter)]
        [TestCase(ChartType.Stock)]
        [TestCase(ChartType.Surface)]
        [TestCase(ChartType.Surface3D)]
        public void TestFillDataWithTimeY(ChartType chartType)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(chartType, 432, 252);
            Chart chart = shape.Chart;

            for (int i = 0; i < chart.Series.Count; i++)
            {
                ChartSeries series = chart.Series[i];
                series.Clear();
                series.HasDataLabels = true;
                series.DataLabels.ShowValue = true;

                for (int j = 1; j <= 5; j++)
                {
                    int hours = j + (chart.Series.Count - 1 - i);
                    series.Add(
                        ChartXValue.FromString("Category " + j),
                        ChartYValue.FromTimeSpan(new TimeSpan(hours, 0, 0)),
                        j + 1);
                }
            }

            // When opening the output file in MS Word and select Edit Data, time values are displayed as numeric:
            // Looks like MS Word doesn't use format code when generates Y data in internal XLSX.

            string fileName = string.Format(@"Model\Charts\DataApi\TestFillData_{0}_TimeY.docx", chartType.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }



        /// <summary>
        /// Tests getting/setting empty (null) X and Y values.
        /// </summary>
        [Test]
        public void TestEmptyValues()
        {
            const string fileName = @"Model\Charts\DataApi\TestEmptyValues.docx";
            Document doc = TestUtil.Open(fileName);

            // Test data of column chart.

            Shape columnChartShape = doc.FirstSection.Body.Shapes[0];
            ChartSeriesCollection columnChartSeries = columnChartShape.Chart.Series;
            Assert.That(columnChartSeries.Count, Is.EqualTo(3));
            ChartSeries columnChartSeries1 = columnChartSeries[0];
            ChartSeries columnChartSeries2 = columnChartSeries[1];
            ChartSeries columnChartSeries3 = columnChartSeries[2];

            ChartXValueCollection xValues1 = columnChartSeries1.XValues;
            Assert.That(xValues1.Count, Is.EqualTo(4));
            Assert.That(xValues1[0], IsNot.Null());
            Assert.That(xValues1[0].ValueType, Is.EqualTo(ChartXValueType.String));
            Assert.That(xValues1[0].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues1[1], Is.Null);
            Assert.That(xValues1[3], Is.Null);

            ChartYValueCollection yValues1 = columnChartSeries1.YValues;
            Assert.That(yValues1.Count, Is.EqualTo(4));
            Assert.That(yValues1[0], Is.Null);
            Assert.That(yValues1[1], Is.Null);
            Assert.That(yValues1[2], IsNot.Null());
            Assert.That(yValues1[2].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues1[3], IsNot.Null());
            Assert.That(yValues1[2].DoubleValue, Is.EqualTo(3.5d));

            Assert.That(columnChartSeries2.YValues[1], Is.Null);
            Assert.That(columnChartSeries3.YValues[1], Is.Null);

            // Update data.
            xValues1[0] = null;
            xValues1[1] = ChartXValue.FromString("Updated Category 2");
            yValues1[1] = ChartYValue.FromDouble(6);
            yValues1[3] = null;

            columnChartSeries1.Insert(0, ChartXValue.FromString("Inserted Category"), ChartYValue.FromDouble(0.5));
            columnChartSeries1.Remove(1);
            columnChartSeries1.Add(ChartXValue.FromString("Last Category"), null);
            // Insert empty values.
            columnChartSeries1.Insert(4, null, null);
            Assert.That(xValues1.Count, Is.EqualTo(6));
            Assert.That(yValues1.Count, Is.EqualTo(6));

            Assert.That(columnChartSeries2.XValues.Count, Is.EqualTo(4));
            Assert.That(columnChartSeries2.YValues.Count, Is.EqualTo(4));
            // Add empty values.
            columnChartSeries2.Add(null, null);
            // X categories of the first series are displayed on the chart. This X value will not be displayed.
            columnChartSeries2.Insert(4, ChartXValue.FromString("Non-displayed value 1"), null);
            Assert.That(columnChartSeries2.XValues.Count, Is.EqualTo(6));
            Assert.That(columnChartSeries2.YValues.Count, Is.EqualTo(6));

            Assert.That(columnChartSeries3.XValues.Count, Is.EqualTo(4));
            Assert.That(columnChartSeries3.YValues.Count, Is.EqualTo(4));
            columnChartSeries3.Add(ChartXValue.FromString("Non-displayed value 2"), ChartYValue.FromDouble(5));
            columnChartSeries3.Insert(4, ChartXValue.FromString("Non-displayed value 3"), ChartYValue.FromDouble(4.5));
            Assert.That(columnChartSeries3.XValues.Count, Is.EqualTo(6));
            Assert.That(columnChartSeries3.YValues.Count, Is.EqualTo(6));

            // Test data of bubble chart.

            Shape bubbleChartShape = doc.FirstSection.Body.Shapes[1];
            ChartSeries series = bubbleChartShape.Chart.Series[0];

            ChartXValueCollection xValues = series.XValues;
            Assert.That(xValues.Count, Is.EqualTo(3));
            Assert.That(xValues[0], IsNot.Null());
            Assert.That(xValues[0].DoubleValue, Is.EqualTo(0.7).Within(0.01));
            Assert.That(xValues[1], IsNot.Null());
            Assert.That(xValues[2], Is.Null);

            ChartYValueCollection yValues = series.YValues;
            Assert.That(yValues.Count, Is.EqualTo(3));
            Assert.That(yValues[0], IsNot.Null());
            Assert.That(yValues[0].DoubleValue, Is.EqualTo(2.7).Within(0.01));
            Assert.That(yValues[1], IsNot.Null());
            Assert.That(yValues[1].DoubleValue, Is.EqualTo(3.2).Within(0.01));
            Assert.That(yValues[2], Is.Null);

            BubbleSizeCollection bubbleSizes = series.BubbleSizes;
            Assert.That(bubbleSizes.Count, Is.EqualTo(3));
            Assert.That(bubbleSizes[0], Is.EqualTo(10).Within(0.01));
            // Empty values in BubbleSizeCollection are represented as NaN.
            Assert.That(double.IsNaN(bubbleSizes[1]), Is.True);
            Assert.That(bubbleSizes[2], Is.EqualTo(8).Within(0.01));

            // Update data.
            bubbleSizes[0] = double.NaN;
            bubbleSizes[1] = 4;
            xValues[2] = ChartXValue.FromDouble(2.5);
            yValues[2] = ChartYValue.FromDouble(1.5);

            series.Insert(0, ChartXValue.FromDouble(0.1), ChartYValue.FromDouble(0.5), 1);
            series.Remove(1);
            // Check adding empty values.
            series.Add(ChartXValue.FromDouble(1), null, double.NaN);
            series.Add(null, ChartYValue.FromDouble(2), 2);
            // Check inserting empty values.
            series.Insert(4, ChartXValue.FromDouble(3), null, 3);
            series.Insert(5, null, ChartYValue.FromDouble(4), double.NaN);
            Assert.That(xValues.Count, Is.EqualTo(7));
            Assert.That(yValues.Count, Is.EqualTo(7));
            Assert.That(bubbleSizes.Count, Is.EqualTo(7));

            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests inserting and removing values to/from a chart series.
        /// </summary>
        [Test]
        public void TestInsertingAndRemovingValues()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Scatter, 432, 252);
            shape.Chart.Series.Clear();
            ChartSeries series = shape.Chart.Series.Add(
                "Series 1",
                new double[] { 0, 2, 3, 3.3, 3.7, 4, 5 },
                new double[] { 0, -2, 0.5, -1, -1, 4, 3 });

            series.HasDataLabels = true;
            series.DefaultDataPoint.Marker.Symbol = MarkerSymbol.Triangle;
            series.DefaultDataPoint.Marker.Size = 12;
            series.DefaultDataPoint.Marker.Format.Fill.Solid(Color.Blue);
            series.DefaultDataPoint.Format.Stroke.Color = Color.LightBlue;

            series.DataPoints[1].Marker.Format.Fill.Solid(Color.Red);
            series.DataLabels[1].ShowValue = true;
            series.DataPoints[5].Marker.Format.Fill.Solid(Color.Green);
            series.DataLabels[5].ShowValue = true;
            series.DataLabels[5].ShowLegendKey = true;
            series.Smooth = true;

            series.Remove(3);
            series.Insert(1, ChartXValue.FromDouble(1), ChartYValue.FromDouble(-1.5));
            series.Remove(4);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\TestInsertingAndRemovingValues.docx");
        }


        /// <summary>
        /// Tests chart data manipulations in a Pareto Word 2016 chart.
        /// </summary>
        [Test]
        public void TestPareto()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            Body body = doc.FirstSection.Body;
            Shape shape = body.Shapes[0];
            Assert.That(shape.Chart.Series.Count, Is.EqualTo(3));

            ChartSeries series1 = shape.Chart.Series[0];
            ChartSeries series2 = shape.Chart.Series[1];
            ChartSeries series3 = shape.Chart.Series[2];
            Assert.That(series1.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));
            Assert.That(series2.LayoutType, Is.EqualTo(SeriesLayout.ClusteredColumn));
            Assert.That(series3.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            Assert.That(series1.XValues.Count, Is.EqualTo(0));
            Assert.That(series1.YValues.Count, Is.EqualTo(0));
            Assert.That(series1.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(series3.XValues.Count, Is.EqualTo(0));
            Assert.That(series3.YValues.Count, Is.EqualTo(0));
            Assert.That(series3.BubbleSizes.Count, Is.EqualTo(0));

            // Check that no exception occurs in RemoveAll when called for a Pareto line series.
            series1.Clear();

            ChartXValueCollection xValues = series2.XValues;
            ChartYValueCollection yValues = series2.YValues;

            Assert.That(xValues.Count, Is.EqualTo(50));
            Assert.That(yValues.Count, Is.EqualTo(50));
            Assert.That(series2.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(xValues[0].ValueType, Is.EqualTo(ChartXValueType.String));
            Assert.That(xValues[0].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues[1].StringValue, Is.EqualTo("Category 3"));
            Assert.That(xValues[49].StringValue, Is.EqualTo("Category 4"));
            Assert.That(yValues[0].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues[0].DoubleValue, Is.EqualTo(1).Within(0.01));
            Assert.That(yValues[1].DoubleValue, Is.EqualTo(1).Within(0.01));
            Assert.That(yValues[49].DoubleValue, Is.EqualTo(1).Within(0.01));

            int count = 0;
            for (int i = xValues.Count - 1; i >= 0; i--)
            {
                if (xValues[i].StringValue == "Category 3")
                {
                    series2.Remove(i);
                    count++;
                }
            }

            Assert.That(count, Is.EqualTo(3));

            series2.YValues[2] = ChartYValue.FromDouble(2);
            series2.Insert(0, ChartXValue.FromString("New Category"), ChartYValue.FromDouble(5));
            series2.Add(ChartXValue.FromString("Last Category"), ChartYValue.FromDouble(4));

            Shape shape2 = (Shape)shape.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape2);

            shape2.Chart.Title.Text = "New Chart 1";

            ChartSeries newChartSeries2 = shape2.Chart.Series[1];
            newChartSeries2.Clear();
            newChartSeries2.HasDataLabels = false;

            for (int i = 1; i <= 12; i++)
            {
                newChartSeries2.Add(
                    ChartXValue.FromString((new DateTime(2022, i, 1)).ToString("yyyy-M-d")),
                    ChartYValue.FromDouble(i * 100));
            }

            Shape shape3 = (Shape)shape.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape3);

            shape3.Chart.Title.Text = "New Chart 2";

            ChartSeries newChartSeries3 = shape3.Chart.Series[1];
            newChartSeries3.Clear();

            for (int i = 1; i <= 12; i++)
            {
                newChartSeries3.Add(
                    ChartXValue.FromString((new DateTime(2022, i, 1)).ToString("yyyy-M-d")),
                    ChartYValue.FromTimeSpan(new TimeSpan(i, 0, 0)));
            }

            // Data and axis labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //shape3.Chart.AxisY.NumberFormat.FormatCode = "[$-x-systime]ч:мм";
            //newChartSeries3.DataLabels.NumberFormat.FormatCode = "[$-x-systime]ч:мм";

            Shape shape4 = (Shape)shape.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape4);

            shape4.Chart.Title.Text = "New Chart 3";

            ChartSeries newChartSeries4 = shape4.Chart.Series[1];
            newChartSeries4.Clear();

            for (int i = 1; i <= 4; i++)
            {
                newChartSeries4.Add(
                    ChartXValue.FromString(string.Format("Category {0}", i)),
                    ChartYValue.FromDateTime(new DateTime(2000 + i * 5, 1, 1)));
            }

            // Data and axis labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //shape4.Chart.AxisY.NumberFormat.FormatCode = "ДД.ММ.ГГГГ";
            //newChartSeries4.DataLabels.NumberFormat.FormatCode = "ДД.ММ.ГГГГ";

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\TestPareto.docx");
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to add a value to a pareto line series using the
        /// <see cref="ChartSeries.Add(ChartXValue)"/> method.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "This series type cannot have data.",
            MatchType = MessageMatch.Exact)]
        public void TestAddingValueToParetoLine1()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            series.Add(ChartXValue.FromString("Category"));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to add a value to a pareto line series using the
        /// <see cref="ChartSeries.Add(ChartXValue, ChartYValue)"/> method.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "This series type cannot have data.",
            MatchType = MessageMatch.Exact)]
        public void TestAddingValueToParetoLine2()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            series.Add(ChartXValue.FromString("Category"), ChartYValue.FromDouble(1));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to add a value to a pareto line series using the
        /// <see cref="ChartSeries.Add(ChartXValue, ChartYValue, double)"/> method.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "This series type cannot have data.",
            MatchType = MessageMatch.Exact)]
        public void TestAddingValueToParetoLine3()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            series.Add(ChartXValue.FromString("Category"), ChartYValue.FromDouble(1), double.NaN);
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to insert a value to a pareto line series using the
        /// <see cref="ChartSeries.Insert(int, ChartXValue)"/> method.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "This series type cannot have data.",
            MatchType = MessageMatch.Exact)]
        public void TestInsertingValueToParetoLine1()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            series.Insert(0, ChartXValue.FromString("Category"));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to insert a value to a pareto line series using the
        /// <see cref="ChartSeries.Insert(int, ChartXValue, ChartYValue)"/> method.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "This series type cannot have data.",
            MatchType = MessageMatch.Exact)]
        public void TestInsertingValueToParetoLine2()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            series.Insert(0, ChartXValue.FromString("Category"), ChartYValue.FromDouble(1));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to insert a value to a pareto line series using the
        /// <see cref="ChartSeries.Insert(int, ChartXValue, ChartYValue, double)"/> method.
        /// </summary>
        [Test]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "This series type cannot have data.",
            MatchType = MessageMatch.Exact)]
        public void TestInsertingValueToParetoLine3()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.ParetoLine));

            series.Insert(0, ChartXValue.FromString("Category"), ChartYValue.FromDouble(1), double.NaN);
        }


        /// <summary>
        /// Tests chart data manipulations in a Waterfall Word 2016 chart.
        /// </summary>
        [Test]
        public void TestWaterfall()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Waterfall.docx");
            Body body = doc.FirstSection.Body;
            Shape shape = body.Shapes[0];
            Assert.That(shape.Chart.Series.Count, Is.EqualTo(1));

            ChartSeries series = shape.Chart.Series[0];
            Assert.That(series.LayoutType, Is.EqualTo(SeriesLayout.Waterfall));

            ChartXValueCollection xValues = series.XValues;
            ChartYValueCollection yValues = series.YValues;

            Assert.That(xValues.Count, Is.EqualTo(8));
            Assert.That(yValues.Count, Is.EqualTo(8));
            Assert.That(series.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(xValues[0].ValueType, Is.EqualTo(ChartXValueType.String));
            Assert.That(xValues[0].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues[1].StringValue, Is.EqualTo("Category 2"));
            Assert.That(xValues[7].StringValue, Is.EqualTo("Category 8"));
            Assert.That(yValues[0].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues[0].DoubleValue, Is.EqualTo(10000).Within(0.01));
            Assert.That(yValues[1].DoubleValue, Is.EqualTo(2000).Within(0.01));
            Assert.That(yValues[7].DoubleValue, Is.EqualTo(14000).Within(0.01));

            xValues[7] = ChartXValue.FromString("Changed Category");
            yValues[7] = ChartYValue.FromDouble(-1000);
            series.Remove(1);
            series.Insert(2, ChartXValue.FromString("New Category"), ChartYValue.FromDouble(2700));
            series.Add(ChartXValue.FromString("Last Category"), ChartYValue.FromDouble(1500));

            Shape shape2 = (Shape)shape.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape2);

            shape2.Chart.Title.Text = "New Chart 1";

            ChartSeries newChartSeries2 = shape2.Chart.Series[0];
            newChartSeries2.Clear();
            // Reset label properties.
            newChartSeries2.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.SpPr, null);
            newChartSeries2.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, null);
            // Reset axis settings.
            shape2.Chart.AxisY.ChartAxisPr = new DmlChartAxisPr(doc, shape2.Chart.ChartSpace.IsChartEx);
            shape2.Chart.AxisY.AxId = 1;

            for (int i = 1; i <= 12; i++)
            {
                newChartSeries2.Add(
                    ChartXValue.FromString((new DateTime(2022, i, 1)).ToString("yyyy-M-d")),
                    ChartYValue.FromTimeSpan(new TimeSpan(i, 0, 0)));
            }

            // Data and axis labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //shape2.Chart.AxisY.NumberFormat.FormatCode = "[$-x-systime]ч:мм";
            //newChartSeries2.DataLabels.NumberFormat.FormatCode = "[$-x-systime]ч:мм";

            Shape shape3 = (Shape)shape2.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape3);

            shape3.Chart.Title.Text = "New Chart 2";

            ChartSeries newChartSeries3 = shape3.Chart.Series[0];
            newChartSeries3.Clear();
            // Hide axis to not show axis labels, which are sums of dates in this chart type.
            shape3.Chart.AxisY.Hidden = true;

            for (int i = 1; i <= 4; i++)
            {
                newChartSeries3.Add(
                    ChartXValue.FromString(string.Format("Category {0}", i)),
                    ChartYValue.FromDateTime(new DateTime(2000 + i * 5, 1, 1)));
            }

            // Data labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //newChartSeries3.DataLabels.NumberFormat.FormatCode = "ДД.ММ.ГГГГ";

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\TestWaterfall.docx");
        }

        /// <summary>
        /// Tests chart data manipulations in a Box & Whisker Word 2016 chart.
        /// </summary>
        [Test]
        public void TestBoxWhisker()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\BoxWhisker.docx");
            Body body = doc.FirstSection.Body;
            Shape shape = body.Shapes[0];
            Assert.That(shape.Chart.Series.Count, Is.EqualTo(3));

            ChartSeries series1 = shape.Chart.Series[0];
            ChartXValueCollection xValues1 = series1.XValues;
            ChartYValueCollection yValues1 = series1.YValues;
            Assert.That(series1.LayoutType, Is.EqualTo(SeriesLayout.BoxWhisker));

            Assert.That(xValues1.Count, Is.EqualTo(22));
            Assert.That(yValues1.Count, Is.EqualTo(22));
            Assert.That(series1.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(xValues1[0].ValueType, Is.EqualTo(ChartXValueType.String));
            Assert.That(xValues1[0].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues1[1].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues1[21].StringValue, Is.EqualTo("Category 3"));
            Assert.That(yValues1[0].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues1[0].DoubleValue, Is.EqualTo(-7).Within(0.01));
            Assert.That(yValues1[1].DoubleValue, Is.EqualTo(-10).Within(0.01));
            Assert.That(yValues1[21].DoubleValue, Is.EqualTo(-20).Within(0.01));

            ChartSeries series2 = shape.Chart.Series[1];
            ChartXValueCollection xValues2 = series2.XValues;
            ChartYValueCollection yValues2 = series2.YValues;

            Assert.That(xValues2.Count, Is.EqualTo(22));
            Assert.That(yValues2.Count, Is.EqualTo(22));
            Assert.That(series2.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(xValues2[0].ValueType, Is.EqualTo(ChartXValueType.String));
            Assert.That(xValues2[0].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues2[1].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues2[21].StringValue, Is.EqualTo("Category 3"));
            Assert.That(yValues2[0].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues2[0].DoubleValue, Is.EqualTo(-3).Within(0.01));
            Assert.That(yValues2[1].DoubleValue, Is.EqualTo(1).Within(0.01));
            Assert.That(yValues2[21].DoubleValue, Is.EqualTo(16).Within(0.01));

            ChartSeries series3 = shape.Chart.Series[2];
            ChartXValueCollection xValues3 = series3.XValues;
            ChartYValueCollection yValues3 = series3.YValues;

            Assert.That(xValues3.Count, Is.EqualTo(22));
            Assert.That(yValues3.Count, Is.EqualTo(22));
            Assert.That(series3.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(xValues3[0].ValueType, Is.EqualTo(ChartXValueType.String));
            Assert.That(xValues3[0].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues3[1].StringValue, Is.EqualTo("Category 1"));
            Assert.That(xValues3[21].StringValue, Is.EqualTo("Category 3"));
            Assert.That(yValues3[0].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues3[0].DoubleValue, Is.EqualTo(-24).Within(0.01));
            Assert.That(yValues3[1].DoubleValue, Is.EqualTo(11).Within(0.01));
            Assert.That(yValues3[21].DoubleValue, Is.EqualTo(-18).Within(0.01));

            // Update data.
            // When updating, it is needed to specify the same X values in all series otherwise there are issues with
            // the chart in MS Word.

            ChartXValue category1 = ChartXValue.FromString("Category 1");
            xValues1[9] = category1;
            xValues2[9] = category1;
            xValues3[9] = category1;

            yValues1[3] = ChartYValue.FromDouble(18);
            yValues2[5] = ChartYValue.FromDouble(-50);
            yValues3[0] = ChartYValue.FromDouble(-50);

            const int removedValueIndex = 20;
            series1.Remove(removedValueIndex);
            series2.Remove(removedValueIndex);
            series3.Remove(removedValueIndex);

            ChartXValue category3 = ChartXValue.FromString("Category 3");
            const int newValueIndex = 16;
            series1.Insert(newValueIndex, category3, ChartYValue.FromDouble(48));
            series2.Insert(newValueIndex, category3, ChartYValue.FromDouble(0));
            series3.Insert(newValueIndex, category3, ChartYValue.FromDouble(100));

            ChartXValue newCategory = ChartXValue.FromString("New Category");
            series1.Add(newCategory, ChartYValue.FromDouble(55));
            series1.Add(newCategory, ChartYValue.FromDouble(-10));
            series1.Add(newCategory, ChartYValue.FromDouble(14));
            series1.Add(newCategory, ChartYValue.FromDouble(7));
            series2.Add(newCategory, ChartYValue.FromDouble(6));
            series2.Add(newCategory, ChartYValue.FromDouble(7));
            series2.Add(newCategory, ChartYValue.FromDouble(-1));
            series2.Add(newCategory, ChartYValue.FromDouble(11));
            series3.Add(newCategory, ChartYValue.FromDouble(0));
            series3.Add(newCategory, ChartYValue.FromDouble(5));
            series3.Add(newCategory, ChartYValue.FromDouble(10));
            series3.Add(newCategory, ChartYValue.FromDouble(15));

            Shape shape2 = (Shape)shape.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape2);

            shape2.Chart.Title.Text = "New Chart 1";

            ChartSeries newChartSeries2 = shape2.Chart.Series[0];
            newChartSeries2.Clear();

            while (shape2.Chart.Series.Count > 1)
                shape2.Chart.Series.RemoveAt(1);

            for (int i = 1; i <= 20; i++)
            {
                newChartSeries2.Add(
                    ChartXValue.FromString((new DateTime(2020 + i / 11, 1, 1)).ToString("yyyy-M-d")),
                    ChartYValue.FromTimeSpan(new TimeSpan(i, 0, 0)));
            }

            // Data and axis labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //shape2.Chart.AxisY.NumberFormat.FormatCode = "[$-x-systime]ч:мм";
            //newChartSeries2.DataLabels.NumberFormat.FormatCode = "[$-x-systime]ч:мм";

            Shape shape3 = (Shape)shape2.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape3);

            shape3.Chart.Title.Text = "New Chart 2";

            ChartSeries newChartSeries3 = shape3.Chart.Series[0];
            newChartSeries3.Clear();

            while (shape3.Chart.Series.Count > 1)
                shape3.Chart.Series.RemoveAt(1);

            for (int i = 1; i <= 20; i++)
            {
                newChartSeries3.Add(
                    ChartXValue.FromString(string.Format("Category {0}", i / 11 + 1)),
                    ChartYValue.FromDateTime(new DateTime(2000 + i, 1, 1)));
            }

            // Data labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //shape3.Chart.AxisY.NumberFormat.FormatCode = "ДД.ММ.ГГГГ";
            //newChartSeries3.DataLabels.NumberFormat.FormatCode = "ДД.ММ.ГГГГ";

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\TestBoxWhisker.docx");
        }

        /// <summary>
        /// Tests chart data manipulations in a Treemap Word 2016 chart.
        /// </summary>
        [Test]
        public void TestTreemap()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Treemap.docx");
            Body body = doc.FirstSection.Body;
            Shape shape1 = body.Shapes[0];
            Assert.That(shape1.Chart.Series.Count, Is.EqualTo(1));

            ChartSeries series1 = shape1.Chart.Series[0];
            ChartXValueCollection xValues1 = series1.XValues;
            ChartYValueCollection yValues1 = series1.YValues;
            Assert.That(series1.LayoutType, Is.EqualTo(SeriesLayout.Treemap));

            Assert.That(xValues1.Count, Is.EqualTo(16));
            Assert.That(yValues1.Count, Is.EqualTo(16));
            Assert.That(series1.BubbleSizes.Count, Is.EqualTo(0));
            Assert.That(xValues1[0].ValueType, Is.EqualTo(ChartXValueType.Multilevel));
            ChartMultilevelValue xValue1Levels = xValues1[0].MultilevelValue;
            Assert.That(xValue1Levels.Level1, Is.EqualTo("Branch 1"));
            Assert.That(xValue1Levels.Level2, Is.EqualTo("Stem 1"));
            Assert.That(xValue1Levels.Level3, Is.EqualTo("Leaf 1"));
            ChartMultilevelValue xValue2Levels = xValues1[1].MultilevelValue;
            Assert.That(xValue2Levels.Level1, Is.EqualTo("Branch 1"));
            Assert.That(xValue2Levels.Level2, Is.EqualTo("Stem 1"));
            Assert.That(xValue2Levels.Level3, Is.EqualTo("Leaf 2"));
            ChartMultilevelValue lastXValueLevels = xValues1[15].MultilevelValue;
            Assert.That(lastXValueLevels.Level1, Is.EqualTo("Branch 3"));
            Assert.That(lastXValueLevels.Level2, Is.EqualTo("Stem 6"));
            Assert.That(lastXValueLevels.Level3, Is.EqualTo("Leaf 16"));

            Assert.That(yValues1[0].ValueType, Is.EqualTo(ChartYValueType.Double));
            Assert.That(yValues1[0].DoubleValue, Is.EqualTo(22).Within(0.01));
            Assert.That(yValues1[1].DoubleValue, Is.EqualTo(12).Within(0.01));
            Assert.That(yValues1[15].DoubleValue, Is.EqualTo(11).Within(0.01));

            // Update data.

            xValues1[7] = ChartXValue.FromMultilevelValue(new ChartMultilevelValue(
                xValues1[6].MultilevelValue.Level1,
                "New Stem 1",
                xValues1[6].MultilevelValue.Level3));

            // Set a value that already exists.
            xValues1[8] = xValues1[9];

            yValues1[1] = ChartYValue.FromDouble(50);
            yValues1[10] = ChartYValue.FromDouble(9);

            series1.Remove(6);

            const string newBranch = "New Branch";

            xValues1[13] = ChartXValue.FromMultilevelValue(new ChartMultilevelValue(
                newBranch,
                "New Stem 1",
                "Leaf 2"));

            series1.Insert(13,
                ChartXValue.FromMultilevelValue(new ChartMultilevelValue(newBranch, "New Stem 1", "Leaf 1")),
                ChartYValue.FromDouble(100));

            series1.Add(
                ChartXValue.FromMultilevelValue(new ChartMultilevelValue(newBranch, "New Stem 2", "Leaf 1")),
                ChartYValue.FromDouble(7));
            series1.Add(
                ChartXValue.FromMultilevelValue(new ChartMultilevelValue(newBranch, "New Stem 2", "Leaf 2")),
                ChartYValue.FromDouble(17));

            Shape shape2 = body.Shapes[1];
            Assert.That(shape2.Chart.Series.Count, Is.EqualTo(1));

            ChartSeries series2 = shape2.Chart.Series[0];
            series2.DataLabels.ShowCategoryName = true;
            series2.DataLabels.ShowValue = true;
            series2.Clear();

            for (int i = 1; i <= 5; i++)
            {
                series2.Add(
                    ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Group 1", string.Format("Item {0}", i))),
                    ChartYValue.FromTimeSpan(new TimeSpan(i, 0, 0)));
            }

            for (int i = 1; i <= 3; i++)
            {
                series2.Add(
                    ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Group 2", string.Format("Item {0}", i))),
                    ChartYValue.FromTimeSpan(new TimeSpan(i * 2, 0, 0)));
            }

            // Data labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //series2.DataLabels.NumberFormat.FormatCode = "[$-x-systime]ч:мм";

            Shape shape3 = (Shape)shape1.Clone(true);
            body.AppendChild(new Paragraph(doc));
            body.LastParagraph.AppendChild(shape3);

            shape3.Chart.Title.Text = "Single-Level Date Chart";

            ChartSeries series3 = shape3.Chart.Series[0];
            series3.Clear();

            series3.DataLabels.ShowCategoryName = true;
            series3.DataLabels.ShowValue = true;
            series3.Clear();

            for (int i = 1; i <= 10; i++)
            {
                series3.Add(
                    ChartXValue.FromMultilevelValue(new ChartMultilevelValue(string.Format("Item {0}", i))),
                    ChartYValue.FromDateTime(new DateTime(2000 + i - 1, 1, 1)));
            }

            // Data labels aren't displayed correctly in Russian locale. It can be resolved in this way:
            //series3.DataLabels.NumberFormat.FormatCode = "ДД.ММ.ГГГГ";

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\TestTreemap.docx");
        }



        /// <summary>
        /// Tests the correspondence between chart data values and data points/labels in a Treemap chart series.
        /// </summary>
        [Test]
        public void TestTreemapDataLabelIndexes()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Treemap.docx");
            Document clonedDocument = doc.Clone();
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            ChartSeries series = shapes[0].Chart.Series[0];

            Assert.That(series.XValues.Count, Is.EqualTo(16));

            // Remove data values starting from the end.
            // Not all labels of the series are materialized, they have string.Empty text in the method calls below.
            CheckDataLabelRemoval(series, 15, 24, "Leaf 16, 11");
            CheckDataLabelRemoval(series, 14, 23, "Leaf 15, 10");
            CheckDataLabelRemoval(series, 13, 21, string.Empty/*Stem 6*/, "Leaf 14, 86");
            CheckDataLabelRemoval(series, 12, 20, "Leaf 13, 19");
            CheckDataLabelRemoval(series, 11, 17, "Branch 3", string.Empty/*Stem 5*/, "Leaf 12, 16");
            CheckDataLabelRemoval(series, 10, 16, "Leaf 11, 89");
            CheckDataLabelRemoval(series, 9, 14, string.Empty/*Stem 4*/, "Leaf 10, 24");
            CheckDataLabelRemoval(series, 8, 13, "Leaf 9, 23");
            CheckDataLabelRemoval(series, 7, 10, "Branch 2", string.Empty/*Stem 3*/, "Leaf 8, 25");
            CheckDataLabelRemoval(series, 6, 9, "Leaf 7, 9");
            CheckDataLabelRemoval(series, 5, 8, "Leaf 6, 17");
            CheckDataLabelRemoval(series, 4, 7, "Leaf 5, 88");
            CheckDataLabelRemoval(series, 3, 5, string.Empty/*Stem 2*/, "Leaf 4, 87");
            CheckDataLabelRemoval(series, 2, 4, "Leaf 3, 18");
            CheckDataLabelRemoval(series, 1, 3, "Leaf 2, 12");
            CheckDataLabelRemoval(series, 0, 0, "Branch 1", string.Empty/*Stem 1*/, "Leaf 1, 22");

            EnsureNoDataPointsAndLabelsExist(series);

            doc = clonedDocument.Clone();
            shapes = doc.FirstSection.Body.Shapes;
            series = shapes[0].Chart.Series[0];

            Assert.That(series.XValues.Count, Is.EqualTo(16));

            // Remove data values starting from the beginning.
            // Not all labels of the series are materialized, they have string.Empty text in the method calls below.
            CheckDataLabelRemoval(series, 0, 2, "Leaf 1, 22");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 2, 12");
            CheckDataLabelRemoval(series, 0, 1, string.Empty/*Stem 1*/, "Leaf 3, 18");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 4, 87");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 5, 88");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 6, 17");
            CheckDataLabelRemoval(series, 0, 0, "Branch 1", string.Empty/*Stem 2*/, "Leaf 7, 9");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 8, 25");
            CheckDataLabelRemoval(series, 0, 1, string.Empty/*Stem 3*/, "Leaf 9, 23");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 10, 24");
            CheckDataLabelRemoval(series, 0, 0, "Branch 2", string.Empty/*Stem 4*/, "Leaf 11, 89");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 12, 16");
            CheckDataLabelRemoval(series, 0, 1, string.Empty/*Stem 5*/, "Leaf 13, 19");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 14, 86");
            CheckDataLabelRemoval(series, 0, 2, "Leaf 15, 10");
            CheckDataLabelRemoval(series, 0, 0, "Branch 3", string.Empty/*Stem 6*/, "Leaf 16, 11");

            EnsureNoDataPointsAndLabelsExist(series);

            doc = clonedDocument;
            shapes = doc.FirstSection.Body.Shapes;
            series = shapes[0].Chart.Series[0];

            Assert.That(series.XValues.Count, Is.EqualTo(16));

            ChartXValue xValue =
                ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Branch 1", "Stem 1", "New Leaf"));
            ChartYValue yValue = ChartYValue.FromDouble(50);
            CheckDataLabelInsertion(series, 3, xValue, yValue, 5, 1);
            xValue = ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Branch 1", "Stem 3", "New Leaf"));
            CheckDataLabelInsertion(series, 8, xValue, yValue, 11, 2);
            xValue = ChartXValue.FromMultilevelValue(new ChartMultilevelValue("New Branch", "Stem 1", "Leaf 1"));
            CheckDataLabelInsertion(series, 9, xValue, yValue, 13, 3);
            CheckDataLabelInsertion(series, 17, xValue, yValue, 29, 5);

            xValue = ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Branch 3", "Stem 6", "Leaf 1"));
            CheckDataLabelChange(series, 17, xValue, 29, 5, 1);
            xValue = ChartXValue.FromMultilevelValue(new ChartMultilevelValue("New Branch", "Stem 1", "Leaf 1"));
            CheckDataLabelChange(series, 14, xValue, 26, 0, 2);
            xValue = ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Branch 1", "Stem 2a", "Leaf 5"));
            CheckDataLabelChange(series, 5, xValue, 8, 1, 3);
        }


        /// <summary>
        /// Tests the correspondence between chart data values and data points/labels in a Pareto chart series.
        /// </summary>
        [Test]
        public void TestParetoDataLabelIndexes()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Pareto.docx");
            Document clonedDocument = doc.Clone();
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            ChartSeries series = shapes[0].Chart.Series[1];

            Assert.That(series.XValues.Count, Is.EqualTo(50));

            // Remove all duplicate values of X category.
            // Remove the first item. After it, the first 4 X values has different value.
            CheckDataLabelRemoval(series, 0, -1);
            // Remove all values except the first 4 ones.
            while (series.XValues.Count > 4)
                CheckDataLabelRemoval(series, 4, -1);

            // Now removing any value will cause removing the last label. This is the same behavior as MS Word has.
            CheckDataLabelRemoval(series, 3, 3, "300%");
            CheckDataLabelRemoval(series, 2, 2, "700%");
            CheckDataLabelRemoval(series, 1, 1, "1700%");
            CheckDataLabelRemoval(series, 0, 0, "2300%");

            EnsureNoDataPointsAndLabelsExist(series);

            doc = clonedDocument;
            shapes = doc.FirstSection.Body.Shapes;
            series = shapes[0].Chart.Series[1];

            Assert.That(series.XValues.Count, Is.EqualTo(50));

            ChartXValue xValue = ChartXValue.FromString("New Category");
            ChartYValue yValue = ChartYValue.FromDouble(50);
            CheckDataLabelInsertion(series, 5, xValue, yValue, 4, 1);
            yValue = ChartYValue.FromDouble(1);
            CheckDataLabelInsertion(series, 10, xValue, yValue, -1, 0);

            CheckDataLabelChange(series, 0, xValue, -1, 0, 0);
            xValue = ChartXValue.FromString("New Category 2");
            CheckDataLabelChange(series, 1, xValue, 5, 0, 1);
            xValue = ChartXValue.FromString("Category 2");
            CheckDataLabelChange(series, 1, xValue, 5, 1, 0);
        }



        /// <summary>
        /// Tests the correspondence between chart data values and data points/labels in a Waterfall chart series.
        /// </summary>
        [Test]
        public void TestWaterfallDataLabelIndexes()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Word2016Charts\Waterfall.docx");
            Document clonedDocument = doc.Clone();
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            ChartSeries series = shapes[0].Chart.Series[0];

            Assert.That(series.XValues.Count, Is.EqualTo(8));

            // Not all labels of the series are materialized, they have string.Empty text in the method calls below.
            CheckDataLabelRemoval(series, 0, 0, string.Empty);
            CheckDataLabelRemoval(series, 0, 0, string.Empty);
            CheckDataLabelRemoval(series, 0, 0, string.Empty);
            CheckDataLabelRemoval(series, 0, 0, "Series1\rCategory 4\r-40");
            CheckDataLabelRemoval(series, 0, 0, "Series1\rCategory 5\r130");
            CheckDataLabelRemoval(series, 0, 0, string.Empty);
            CheckDataLabelRemoval(series, 0, 0, string.Empty);
            CheckDataLabelRemoval(series, 0, 0, string.Empty);

            EnsureNoDataPointsAndLabelsExist(series);

            doc = clonedDocument;
            shapes = doc.FirstSection.Body.Shapes;
            series = shapes[0].Chart.Series[0];

            Assert.That(series.XValues.Count, Is.EqualTo(8));

            ChartXValue xValue = ChartXValue.FromString("New Category");
            ChartYValue yValue = ChartYValue.FromDouble(10);
            CheckDataLabelInsertion(series, 2, xValue, yValue, 2, 1);
            xValue = ChartXValue.FromString("New Category 2");
            yValue = ChartYValue.FromDouble(20);
            CheckDataLabelInsertion(series, 5, xValue, yValue, 5, 1);

            xValue = ChartXValue.FromString("Category 9");
            CheckDataLabelChange(series, 5, xValue, -1, 0, 0);
        }



        /// <summary>
        /// Tests the <see cref="ChartXValue.Equals(object)"/> and <see cref="ChartXValue.GetHashCode"/> methods.
        /// </summary>
        [Test]
        public void TestChartXValueEquality()
        {
            ChartXValue xValue1 = ChartXValue.FromString("123");
            ChartXValue xValue2 = ChartXValue.FromString("123");
            ChartXValue xValue3 = ChartXValue.FromString("1234");
            Assert.That(xValue1.Equals(xValue2), Is.True);
            Assert.That(xValue1.Equals(xValue3), Is.False);

            ChartXValue xValue4 = ChartXValue.FromDouble(10);
            ChartXValue xValue5 = ChartXValue.FromDouble(10);
            ChartXValue xValue6 = ChartXValue.FromDouble(10.01);
            Assert.That(xValue4.Equals(xValue5), Is.True);
            Assert.That(xValue4.Equals(xValue6), Is.False);

            ChartXValue xValue7 = ChartXValue.FromDateTime(new DateTime(2022, 10, 12, 11, 39, 0));
            ChartXValue xValue8 = ChartXValue.FromDateTime(new DateTime(2022, 10, 12, 11, 39, 0));
            ChartXValue xValue9 = ChartXValue.FromDateTime(new DateTime(2022, 10, 12, 11, 39, 1));
            Assert.That(xValue7.Equals(xValue8), Is.True);
            Assert.That(xValue7.Equals(xValue9), Is.False);

            ChartXValue xValue10 = ChartXValue.FromTimeSpan(new TimeSpan(2, 30, 0));
            ChartXValue xValue11 = ChartXValue.FromTimeSpan(new TimeSpan(2, 30, 0));
            ChartXValue xValue12 = ChartXValue.FromTimeSpan(new TimeSpan(2, 30, 1));
            Assert.That(xValue10.Equals(xValue11), Is.True);
            Assert.That(xValue10.Equals(xValue12), Is.False);

            HashSetGeneric<ChartXValue> hashSet = new HashSetGeneric<ChartXValue>();
            hashSet.Add(xValue1);
            hashSet.Add(xValue4);
            hashSet.Add(xValue7);
            hashSet.Add(xValue10);
            Assert.That(hashSet.Contains(xValue2), Is.True);
            Assert.That(hashSet.Contains(xValue3), Is.False);
            Assert.That(hashSet.Contains(xValue5), Is.True);
            Assert.That(hashSet.Contains(xValue6), Is.False);
            Assert.That(hashSet.Contains(xValue8), Is.True);
            Assert.That(hashSet.Contains(xValue9), Is.False);
            Assert.That(hashSet.Contains(xValue11), Is.True);
            Assert.That(hashSet.Contains(xValue12), Is.False);
        }

        /// <summary>
        /// Tests the <see cref="ChartYValue.Equals(object)"/> and <see cref="ChartYValue.GetHashCode"/> methods.
        /// </summary>
        [Test]
        public void TestChartYValueEquality()
        {
            ChartYValue yValue1 = ChartYValue.FromDouble(10);
            ChartYValue yValue2 = ChartYValue.FromDouble(10);
            ChartYValue yValue3 = ChartYValue.FromDouble(10.01);
            Assert.That(yValue1.Equals(yValue2), Is.True);
            Assert.That(yValue1.Equals(yValue3), Is.False);

            ChartYValue yValue4 = ChartYValue.FromDateTime(new DateTime(2022, 10, 12, 11, 39, 0));
            ChartYValue yValue5 = ChartYValue.FromDateTime(new DateTime(2022, 10, 12, 11, 39, 0));
            ChartYValue yValue6 = ChartYValue.FromDateTime(new DateTime(2022, 10, 12, 11, 39, 1));
            Assert.That(yValue4.Equals(yValue5), Is.True);
            Assert.That(yValue4.Equals(yValue6), Is.False);

            ChartYValue yValue7 = ChartYValue.FromTimeSpan(new TimeSpan(2, 30, 0));
            ChartYValue yValue8 = ChartYValue.FromTimeSpan(new TimeSpan(2, 30, 0));
            ChartYValue yValue9 = ChartYValue.FromTimeSpan(new TimeSpan(2, 30, 1));
            Assert.That(yValue7.Equals(yValue8), Is.True);
            Assert.That(yValue7.Equals(yValue9), Is.False);

            HashSetGeneric<ChartYValue> hashSet = new HashSetGeneric<ChartYValue>();
            hashSet.Add(yValue1);
            hashSet.Add(yValue4);
            hashSet.Add(yValue7);
            Assert.That(hashSet.Contains(yValue2), Is.True);
            Assert.That(hashSet.Contains(yValue3), Is.False);
            Assert.That(hashSet.Contains(yValue5), Is.True);
            Assert.That(hashSet.Contains(yValue6), Is.False);
            Assert.That(hashSet.Contains(yValue8), Is.True);
            Assert.That(hashSet.Contains(yValue9), Is.False);
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to add a chart X value of a non-supported type
        /// (<see cref="ChartXValueType"/>) to an empty series of a Word 2016 chart.
        /// </summary>
        [TestCase(@"Model\Charts\Word2016Charts\Pareto.docx", ChartXValueType.Double)]
        [TestCase(@"Model\Charts\Word2016Charts\Pareto.docx", ChartXValueType.DateTime)]
        [TestCase(@"Model\Charts\Word2016Charts\Pareto.docx", ChartXValueType.Time)]
        [TestCase(@"Model\Charts\Word2016Charts\Pareto.docx", ChartXValueType.Multilevel)]
        [TestCase(@"Model\Charts\Word2016Charts\Waterfall.docx", ChartXValueType.Double)]
        [TestCase(@"Model\Charts\Word2016Charts\Waterfall.docx", ChartXValueType.DateTime)]
        [TestCase(@"Model\Charts\Word2016Charts\Waterfall.docx", ChartXValueType.Time)]
        [TestCase(@"Model\Charts\Word2016Charts\Waterfall.docx", ChartXValueType.Multilevel)]
        [TestCase(@"Model\Charts\Word2016Charts\BoxWhisker.docx", ChartXValueType.Double)]
        [TestCase(@"Model\Charts\Word2016Charts\BoxWhisker.docx", ChartXValueType.DateTime)]
        [TestCase(@"Model\Charts\Word2016Charts\BoxWhisker.docx", ChartXValueType.Time)]
        [TestCase(@"Model\Charts\Word2016Charts\BoxWhisker.docx", ChartXValueType.Multilevel)]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The series cannot contain an X value of this type.",
            MatchType = MessageMatch.Exact)]
        public void TestAddingWrongXValueType2016(string fileName, ChartXValueType valueType)
        {
            Document doc = TestUtil.Open(fileName);

            // Find a series that can be updated.
            ChartSeries series = null;
            foreach (ChartSeries currentSeries in doc.FirstSection.Body.Shapes[0].Chart.Series)
            {
                // ParetoLine series cannot have data.
                if (currentSeries.LayoutType != SeriesLayout.ParetoLine)
                {
                    series = currentSeries;
                    break;
                }
            }

            Debug.Assert(series != null);

            series.Clear();
            series.Add(CreateXValue(valueType));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to add a chart value of a different type
        /// (<see cref="ChartXValueType"/>/<see cref="ChartYValueType"/>) than a series already contains.
        /// </summary>
        [TestCase(ChartXValueType.Double, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.Double, ChartYValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartYValueType.Double, ChartXValueType.Double, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartYValueType.Double, ChartXValueType.Double, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Double, ChartYValueType.Double, ChartXValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartYValueType.Double, ChartXValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Double, ChartYValueType.Double, ChartXValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartYValueType.Double, ChartXValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.DateTime)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Time)]
        [TestCase(ChartXValueType.String, ChartYValueType.DateTime, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.DateTime, ChartXValueType.String, ChartYValueType.Time)]
        [TestCase(ChartXValueType.String, ChartYValueType.Time, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Time, ChartXValueType.String, ChartYValueType.DateTime)]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The value must be of the same type as the existing items in the collection.",
            MatchType = MessageMatch.Exact)]
        public void TestAddingWrongTypeValue(ChartXValueType initialXValueType, ChartYValueType initialYValueType,
            ChartXValueType addingXValueType, ChartYValueType addingYValueType)
        {
            ChartSeries series = CreateAreaChartWithEmptySeries();

            // Add the first data point to the series. This specifies data types of X/Y values stored in the series.
            // Do in a try/catch block to ensure that the exception being checked in the test is not thrown.
            try
            {
                series.Add(CreateXValue(initialXValueType), CreateYValue(initialYValueType));
            }
            catch
            {
                Assert.Fail("This exception is not expected.");
            }

            series.Add(CreateXValue(addingXValueType), CreateYValue(addingYValueType));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to insert a chart value of a different type
        /// (<see cref="ChartXValueType"/>/<see cref="ChartYValueType"/>) than a series already contains.
        /// </summary>
        [TestCase(ChartXValueType.Double, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.Double, ChartYValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartYValueType.Double, ChartXValueType.Double, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartYValueType.Double, ChartXValueType.Double, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Double, ChartYValueType.Double, ChartXValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartYValueType.Double, ChartXValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartXValueType.Double, ChartYValueType.Double, ChartXValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartYValueType.Double, ChartXValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.DateTime)]
        [TestCase(ChartXValueType.String, ChartYValueType.Double, ChartXValueType.String, ChartYValueType.Time)]
        [TestCase(ChartXValueType.String, ChartYValueType.DateTime, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.DateTime, ChartXValueType.String, ChartYValueType.Time)]
        [TestCase(ChartXValueType.String, ChartYValueType.Time, ChartXValueType.String, ChartYValueType.Double)]
        [TestCase(ChartXValueType.String, ChartYValueType.Time, ChartXValueType.String, ChartYValueType.DateTime)]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The value must be of the same type as the existing items in the collection.",
            MatchType = MessageMatch.Exact)]
        public void TestInsertingWrongTypeValue(ChartXValueType initialXValueType, ChartYValueType initialYValueType,
            ChartXValueType addingXValueType, ChartYValueType addingYValueType)
        {
            ChartSeries series = CreateAreaChartWithEmptySeries();

            // Add the first data point to the series. This specifies data types of X/Y values stored in the series.
            // Do in a try/catch block to ensure that the exception being checked in the test is not thrown.
            try
            {
                series.Add(CreateXValue(initialXValueType), CreateYValue(initialYValueType));
            }
            catch
            {
                Assert.Fail("This exception is not expected.");
            }

            series.Insert(1, CreateXValue(addingXValueType), CreateYValue(addingYValueType));
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to set a chart X value of a different type
        /// (<see cref="ChartXValueType"/>) than a series already contains.
        /// </summary>
        [TestCase(ChartXValueType.Double, ChartXValueType.String)]
        [TestCase(ChartXValueType.DateTime, ChartXValueType.String)]
        [TestCase(ChartXValueType.Time, ChartXValueType.String)]
        [TestCase(ChartXValueType.String, ChartXValueType.Double)]
        [TestCase(ChartXValueType.DateTime, ChartXValueType.Double)]
        [TestCase(ChartXValueType.Time, ChartXValueType.Double)]
        [TestCase(ChartXValueType.String, ChartXValueType.DateTime)]
        [TestCase(ChartXValueType.Double, ChartXValueType.DateTime)]
        [TestCase(ChartXValueType.Time, ChartXValueType.DateTime)]
        [TestCase(ChartXValueType.String, ChartXValueType.Time)]
        [TestCase(ChartXValueType.Double, ChartXValueType.Time)]
        [TestCase(ChartXValueType.DateTime, ChartXValueType.Time)]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The value must be of the same type as the existing items in the collection.",
            MatchType = MessageMatch.Exact)]
        public void TestSettingWrongTypeXValue(ChartXValueType initialXValueType, ChartXValueType settingXValueType)
        {
            ChartSeries series = CreateAreaChartWithEmptySeries();

            // Add the first data point to the series. This specifies data types of X/Y values stored in the series.
            // Do in a try/catch block to ensure that the exception being checked in the test is not thrown.
            try
            {
                series.Add(CreateXValue(initialXValueType), ChartYValue.FromDouble(1));
            }
            catch
            {
                Assert.Fail("This exception is not expected.");
            }

            series.XValues[0] = CreateXValue(settingXValueType);
        }

        /// <summary>
        /// Tests that an exception is thrown when trying to set a chart Y value of a different type
        /// (<see cref="ChartYValueType"/>) than a series already contains.
        /// </summary>
        [TestCase(ChartYValueType.DateTime, ChartYValueType.Double)]
        [TestCase(ChartYValueType.Time, ChartYValueType.Double)]
        [TestCase(ChartYValueType.Double, ChartYValueType.DateTime)]
        [TestCase(ChartYValueType.Time, ChartYValueType.DateTime)]
        [TestCase(ChartYValueType.Double, ChartYValueType.Time)]
        [TestCase(ChartYValueType.DateTime, ChartYValueType.Time)]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "The value must be of the same type as the existing items in the collection.",
            MatchType = MessageMatch.Exact)]
        public void TestSettingWrongTypeYValue(ChartYValueType initialYValueType, ChartYValueType settingYValueType)
        {
            ChartSeries series = CreateAreaChartWithEmptySeries();

            // Add the first data point to the series. This specifies data types of X/Y values stored in the series.
            // Do in a try/catch block to ensure that the exception being checked in the test is not thrown.
            try
            {
                series.Add(ChartXValue.FromString("Category"), CreateYValue(initialYValueType));
            }
            catch
            {
                Assert.Fail("This exception is not expected.");
            }

            series.YValues[0] = CreateYValue(settingYValueType);
        }

        /// <summary>
        /// Tests the <see cref="ChartSeries.ClearValues"/> method.
        /// </summary>
        [Test]
        public void TestClearValues()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            ChartSeries series = shape.Chart.Series[0];

            series.HasDataLabels = true;
            // Set some label formatting to be checked after clearing values.
            const string numberFormat = "#,##0.00";
            series.DataLabels[1].NumberFormat.FormatCode = numberFormat;

            series.ClearValues();
            series.Add(ChartXValue.FromString("Category 1"), ChartYValue.FromDouble(10));
            series.Add(ChartXValue.FromString("Category 2"), ChartYValue.FromDouble(3.5));

            // Checks that series.ClearValues has not deleted the labels.
            Assert.That(series.DataLabels[1].NumberFormat.FormatCode, Is.EqualTo(numberFormat));
            Assert.That(series.DataLabels[0].NumberFormat.FormatCode, IsNot.EqualTo(numberFormat));

            series.Clear();
            series.Add(ChartXValue.FromString("Category 1"), ChartYValue.FromDouble(7));
            series.Add(ChartXValue.FromString("Category 2"), ChartYValue.FromDouble(5.2));

            // Checks that series.Clear has deleted the labels.
            Assert.That(series.DataLabels[1].NumberFormat.FormatCode, IsNot.EqualTo(numberFormat));
            Assert.That(series.DataLabels[0].NumberFormat.FormatCode, IsNot.EqualTo(numberFormat));
        }

        /// <summary>
        /// WORDSNET-26164 Errors with dates of 1900 year in charts
        /// When a series is being added using the <see cref="ChartSeriesCollection.Add(string,DateTime[],double[])"/>
        /// method, the <see cref="DmlChartUtil.GetDoubleFromDate"/> method is used now instead of
        /// <see cref="DateTime.ToOADate"/> to get numeric representation of datetime values. And the method
        /// <see cref="DmlChartUtil.GetDoubleFromDate "/> returned wrong value when converting the 01/03/1900 date.
        /// </summary>
        [Test]
        public void Test26164()
        {
            // Generate dates for X values of charts.

            DateTime[] dates = new DateTime[65];
            double[] values = new double[dates.Length];

            DateTime startValue = new DateTime(1899, 12, 31);
            for (int i = 0; i < dates.Length; i++)
            {
                dates[i] = startValue.AddDays(i);
                values[i] = i + 5;
            }

            Document doc = new Document();
            doc.FirstSection.PageSetup.Orientation = Orientation.Landscape;

            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add a chart and fill data using ChartSeriesCollection.Add.

            Shape shape1 = builder.InsertChart(ChartType.Column, 640, 240);
            Chart chart1 = shape1.Chart;
            chart1.Title.Text = "ChartSeriesCollection.Add";
            chart1.Series.Clear();

            ChartSeries series1 = chart1.Series.Add("Series 1", dates, values);

            // Check dates.
            for (int i = 0; i < dates.Length; i++)
                Assert.That(series1.XValues[i].DateTimeValue, Is.EqualTo(dates[i]));

            // Add a chart and fill data using ChartSeries.Add.

            CompositeNode paragraph = shape1.GetAncestor(NodeType.Paragraph);
            CompositeNode newParagraph = (CompositeNode)paragraph.Clone(true);
            paragraph.InsertNext(newParagraph);
            Shape shape2 = (Shape)newParagraph.GetChild(NodeType.Shape, 0, true);
            Chart chart2 = shape2.Chart;
            chart2.Title.Text = "ChartSeries.Add";
            ChartSeries series2 = chart2.Series[0];

            series2.ClearValues();
            for (int i = 0; i < dates.Length; i++)
                series2.Add(ChartXValue.FromDateTime(dates[i]), ChartYValue.FromDouble(values[i]));

            // Check dates.
            for (int i = 0; i < dates.Length; i++)
                Assert.That(series2.XValues[i].DateTimeValue, Is.EqualTo(dates[i]));

            // The charts in the output document contain the '31/12/1899' date without label, which is correct, because
            // MS Word does not support dates before 01/01/1900. And there are empty chart values that relate to the dates
            // 00/01/1900 and 29/02/1900 that are existing dates in MS Word but not in .NET.

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\Test26164.docx");
        }

        /// <summary>
        /// Tests the <see cref="DmlChartUtil.GetDoubleFromDate"/> and <see cref="DmlChartUtil.GetDateFromDouble"/>
        /// methods that are used in charts.
        /// </summary>
        [Test]
        public void TestDateTimeConversion()
        {
            Assert.That(DmlChartUtil.GetDoubleFromDate(new DateTime(1900, 2, 28)), Is.EqualTo(59).Within(0.001));
            Assert.That(DmlChartUtil.GetDoubleFromDate(new DateTime(1900, 3, 1)), Is.EqualTo(61).Within(0.001));

            const double oneMinute = 1d / 24 / 60;
            Assert.That(DmlChartUtil.GetDoubleFromDate(new DateTime(1900, 2, 28, 0, 1, 0)), Is.EqualTo(59 + oneMinute).Within(0.001));
            Assert.That(DmlChartUtil.GetDoubleFromDate(new DateTime(1900, 3, 1, 0, 1, 0)), Is.EqualTo(61 + oneMinute).Within(0.001));

            Assert.That(DmlChartUtil.GetDateFromDouble(59), Is.EqualTo(new DateTime(1900, 2, 28)));
            Assert.That(DmlChartUtil.GetDateFromDouble(61), Is.EqualTo(new DateTime(1900, 3, 1)));

            Assert.That(DmlChartUtil.GetDateFromDouble(59.125), Is.EqualTo(new DateTime(1900, 2, 28, 3, 0, 0)));
            Assert.That(DmlChartUtil.GetDateFromDouble(61.125), Is.EqualTo(new DateTime(1900, 3, 1, 3, 0, 0)));
        }

        /// <summary>
        /// WORDSNET-27533 Support for displaying numbers as decimals in Aspose.Words Charts
        /// An ability to set format code for X, Y, Buble Size data collections has been implemented.
        /// </summary>
        [Test]
        public void Test27533()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape1 = builder.InsertChart(ChartType.Bubble, 432, 252);
            ChartSeriesCollection seriesCollection1 = shape1.Chart.Series;
            seriesCollection1.Clear();

            ChartSeries series1 = seriesCollection1.Add(
                "Series1",
                new double[] { 1, 1.9, 2.45, 3 },
                new double[] { 4, 5.9, 1.82, 4 },
                new double[] { 2, 1.1, 2.95, 2 });

            series1.HasDataLabels = true;
            series1.DataLabels.ShowCategoryName = true;
            series1.DataLabels.ShowValue = true;
            series1.DataLabels.ShowBubbleSize = true;

            series1.XValues.FormatCode = "#,##0.0#";
            series1.YValues.FormatCode = "#,##0.0#";
            series1.BubbleSizes.FormatCode = "#,##0.0#";

            Shape shape2 = builder.InsertChart(ChartType.Column, 432, 252);
            ChartSeriesCollection seriesCollection2 = shape2.Chart.Series;
            seriesCollection2.Clear();

            ChartSeries series2_1 = seriesCollection2.Add(
                "Series1",
                new string[] { "Category1", "Category2", "Category3" },
                new double[] { 4, 5.9, 1.82 });

            series2_1.HasDataLabels = true;
            series2_1.DataLabels.ShowValue = true;

            series2_1.YValues.FormatCode = "#,##0.00";

            ChartSeries series2_2 = seriesCollection2.Add(
                "Series1",
                new string[] { "Category1", "Category2", "Category3" },
                new double[] { 1, 1.9, 2.45 });

            series2_2.HasDataLabels = true;
            series2_2.DataLabels.ShowValue = true;

            // Test setting empty values.
            series2_2.YValues.FormatCode = null;

            // Test time format.
            Shape shape3 = builder.InsertChart(ChartType.Scatter, 432, 252);
            ChartSeriesCollection seriesCollection3 = shape3.Chart.Series;
            seriesCollection3.Clear();

            ChartSeries series3 = seriesCollection3.Add("Series1", new double[] { 0 }, new double[] { 0 });
            series3.ClearValues();
            series3.Add(ChartXValue.FromTimeSpan(new TimeSpan(8, 0, 0)), ChartYValue.FromTimeSpan(new TimeSpan(1, 0, 0)));
            series3.Add(ChartXValue.FromTimeSpan(new TimeSpan(11, 45, 0)), ChartYValue.FromTimeSpan(new TimeSpan(2, 0, 0)));
            series3.Add(ChartXValue.FromTimeSpan(new TimeSpan(14, 30, 0)), ChartYValue.FromTimeSpan(new TimeSpan(3, 0, 0)));
            series3.Add(ChartXValue.FromTimeSpan(new TimeSpan(18, 0, 0)), ChartYValue.FromTimeSpan(new TimeSpan(4, 0, 0)));

            series3.HasDataLabels = true;
            series3.DataLabels.ShowCategoryName = true;
            series3.DataLabels.ShowValue = true;

            series3.XValues.FormatCode = "h:mm\\ am/pm";
            series3.YValues.FormatCode = "h:mm\\ am/pm";

            // Test date format.
            Shape shape4 = builder.InsertChart(ChartType.Scatter, 432, 252);
            ChartSeriesCollection seriesCollection4 = shape4.Chart.Series;
            seriesCollection4.Clear();

            ChartSeries series4 = seriesCollection4.Add("Series1", new double[] { 0 }, new double[] { 0 });
            series4.ClearValues();
            series4.Add(ChartXValue.FromDateTime(new DateTime(2024, 1, 1)),
                ChartYValue.FromDateTime(new DateTime(2024, 1, 1)));
            series4.Add(ChartXValue.FromDateTime(new DateTime(2024, 1, 2)),
                ChartYValue.FromDateTime(new DateTime(2024, 2, 1)));
            series4.Add(ChartXValue.FromDateTime(new DateTime(2024, 1, 3)),
                ChartYValue.FromDateTime(new DateTime(2024, 3, 1)));
            series4.Add(ChartXValue.FromDateTime(new DateTime(2024, 1, 4)),
                ChartYValue.FromDateTime(new DateTime(2024, 4, 1)));

            series4.HasDataLabels = true;
            series4.DataLabels.ShowCategoryName = true;
            series4.DataLabels.ShowValue = true;

            series4.XValues.FormatCode = "yyyy-mm-dd";
            series4.YValues.FormatCode = "yyyy-mm-dd";

            // Test Word 2016 charts.

            Shape shape5 = builder.InsertChart(ChartType.Funnel, 432, 252);
            ChartSeriesCollection seriesCollection5 = shape5.Chart.Series;
            seriesCollection5.Clear();

            ChartSeries series5 = seriesCollection5.Add(
                "Series1",
                new string[] { "Category1", "Category2", "Category3" },
                new double[] { 5, 3.1, 1.95 });

            series5.HasDataLabels = true;
            series5.DataLabels.ShowValue = true;

            series5.YValues.FormatCode = "#,##0.00"; // This should be "# ##0,00" in ru locale.

            // Test time format.
            Shape shape6 = builder.InsertChart(ChartType.Funnel, 432, 252);
            ChartSeriesCollection seriesCollection6 = shape6.Chart.Series;
            seriesCollection6.Clear();

            ChartSeries series6 = seriesCollection6.Add("Series1", new string[] { "" }, new double[] { 0 });
            series6.ClearValues();
            series6.Add(ChartXValue.FromString("Category1"), ChartYValue.FromTimeSpan(new TimeSpan(8, 0, 0)));
            series6.Add(ChartXValue.FromString("Category2"), ChartYValue.FromTimeSpan(new TimeSpan(12, 0, 0)));
            series6.Add(ChartXValue.FromString("Category3"), ChartYValue.FromTimeSpan(new TimeSpan(16, 0, 0)));

            series6.HasDataLabels = true;
            series6.DataLabels.ShowValue = true;

            series6.YValues.FormatCode = "h:mm\\ am/pm"; // This should be "ч:мм\\ AM/PM" in ru locale.

            // Test date format.
            Shape shape7 = builder.InsertChart(ChartType.Funnel, 432, 252);
            ChartSeriesCollection seriesCollection7 = shape7.Chart.Series;
            seriesCollection7.Clear();

            ChartSeries series7 = seriesCollection7.Add("Series1", new string[] { "" }, new double[] { 0 });
            series7.ClearValues();
            series7.Add(ChartXValue.FromString("Category1"), ChartYValue.FromDateTime(new DateTime(2024, 1, 1)));
            series7.Add(ChartXValue.FromString("Category2"), ChartYValue.FromDateTime(new DateTime(2024, 2, 1)));
            series7.Add(ChartXValue.FromString("Category3"), ChartYValue.FromDateTime(new DateTime(2024, 3, 1)));

            series7.HasDataLabels = true;
            series7.DataLabels.ShowValue = true;

            series7.YValues.FormatCode = "yyyy-mm-dd"; // This should be "ГГГГ\\-ММ\\-ДД" in ru locale.

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\DataApi\Test27533.docx");
        }

        /// <summary>
        /// Creates a test X value of the specified type.
        /// </summary>
        private static ChartXValue CreateXValue(ChartXValueType valueType)
        {
            switch (valueType)
            {
                case ChartXValueType.String:
                    return ChartXValue.FromString("Category");
                case ChartXValueType.Double:
                    return ChartXValue.FromDouble(1);
                case ChartXValueType.DateTime:
                    return ChartXValue.FromDateTime(DateTime.Today);
                case ChartXValueType.Time:
                    return ChartXValue.FromTimeSpan(TimeSpan.Zero);
                case ChartXValueType.Multilevel:
                    return ChartXValue.FromMultilevelValue(new ChartMultilevelValue("Level1", "Level2", "Level3"));
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Creates a test Y value of the specified type.
        /// </summary>
        private static ChartYValue CreateYValue(ChartYValueType valueType)
        {
            switch (valueType)
            {
                case ChartYValueType.Double:
                    return ChartYValue.FromDouble(1);
                case ChartYValueType.DateTime:
                    return ChartYValue.FromDateTime(DateTime.Today);
                case ChartYValueType.Time:
                    return ChartYValue.FromTimeSpan(TimeSpan.Zero);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Creates a test chart where the first series is empty.
        /// </summary>
        private static ChartSeries CreateAreaChartWithEmptySeries()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Area, 432, 252);
            ChartSeries series = shape.Chart.Series[0];
            series.Clear();

            return series;
        }

        /// <summary>
        /// Checks that labels are removed after removing data value with the specified index.
        /// </summary>
        /// <remarks>
        /// There may be zero, one or several labels that correspond to some chart data value.
        /// </remarks>
        /// <param name="series">The series whose label removal is being checked.</param>
        /// <param name="valueIndex">The index of the data value to remove.</param>
        /// <param name="firstRemovedLabelIndex">The index of the first label that is expected to be removed after the
        /// data value is removed. When a value is removed, there can only be a contiguous range of labels that match
        /// the value and should be removed.</param>
        /// <param name="removedLabelTexts">The array of text of the labels that are expected to be removed. Text should
        /// be <see cref="string.Empty"/> for non-materialized labels.</param>
        private static void CheckDataLabelRemoval(ChartSeries series, int valueIndex, int firstRemovedLabelIndex,
            params string[] removedLabelTexts)
        {
            Debug.Assert((firstRemovedLabelIndex >= 0) || (removedLabelTexts.Length == 0));

            ChartDataLabelCollection labels = series.DataLabels;
            // Use data point count to exclude possible not used labels left after data removal and extra label of
            // an OfPieChart chart.
            int pointCount = series.DataPoints.Count;
            ChartDataLabel[] oldLabels = new ChartDataLabel[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                if (labels.HasNonDefaultItemFormatting(i))
                    oldLabels[i] = labels[i];
            }

            bool isLabelRemovalExpected = (firstRemovedLabelIndex >= 0);
            int removedLabelCount = removedLabelTexts.Length;

            if (removedLabelTexts.Length > 0)
            {
                // Check that the label text is the same as the value itself to ensure that previous removals worked
                // correctly.

                bool isTreemapOrSunburst =
                    (series.LayoutType == SeriesLayout.Treemap) || (series.LayoutType == SeriesLayout.Sunburst);

                int valueLabelTextIndex = isTreemapOrSunburst ? removedLabelCount - 1 : 0;
                string valueLabelText = removedLabelTexts[valueLabelTextIndex];
                if ((valueLabelText != string.Empty) && (series.LayoutType != SeriesLayout.ClusteredColumn))
                {
                    string valueText = GetLabelValueText(series, valueIndex);
                    // A label may contain other information divided by the separator.
                    Assert.That(valueLabelText.StartsWith(valueText) || valueLabelText.EndsWith(valueText), Is.True);
                }
            }

            // Remove the value.
            series.Remove(valueIndex);

            Assert.That(series.DataPoints.Count, Is.EqualTo(pointCount - removedLabelCount));

            // When a value is removed, there can only be a contiguous range of labels that match the value and should be
            // removed: check that labels from firstRemovedLabelIndex to (firstRemovedLabelIndex + removedLabelCount - 1)
            // have been removed.
            int lastRemovedLabelIndex = isLabelRemovalExpected ? firstRemovedLabelIndex + removedLabelCount - 1 : -1;
            for (int i = 0; i < oldLabels.Length; i++)
            {
                ChartDataLabel label = oldLabels[i];
                if (label == null)
                {
                    if (isLabelRemovalExpected && (i >= firstRemovedLabelIndex) && (i <= lastRemovedLabelIndex))
                    {
                        // Check that it is expected that the label is not materialized.
                        Assert.That(removedLabelTexts[i - firstRemovedLabelIndex], Is.EqualTo(string.Empty));
                    }

                    continue;
                }

                if (!isLabelRemovalExpected || (i < firstRemovedLabelIndex))
                {
                    Assert.That(labels[i], Is.SameAs(label));
                    Assert.That(label.Index, Is.EqualTo(i));
                }
                else if (i <= lastRemovedLabelIndex)
                {
                    Assert.That(labels.Contains(label), Is.False);
                    string actualLabelText = label.TxPr.GetRunText();
                    Assert.That(actualLabelText, Is.EqualTo(removedLabelTexts[i - firstRemovedLabelIndex]));
                }
                else
                {
                    Assert.That(labels[i - removedLabelCount], Is.SameAs(label));
                    Assert.That(label.Index, Is.EqualTo(i - removedLabelCount));
                }
            }
        }

        /// <summary>
        /// Checks that labels are moved in the data label collection after inserting data value with the specified index.
        /// </summary>
        /// <remarks>
        /// There may be zero, one or several labels that correspond to some chart data value.
        /// </remarks>
        /// <param name="series">The series whose label removal is being checked.</param>
        /// <param name="valueIndex">The index to insert the data value.</param>
        /// <param name="xValue">The inserting X value.</param>
        /// <param name="yValue">The inserting Y value.</param>
        /// <param name="expectedLabelInsertionIndex">The index of the first label that is expected to be inserted after
        /// the data value insertion.</param>
        /// <param name="expectedInsertedLabelCount">The number of the labels that is expected to be inserted after the
        /// data value insertion.</param>
        private static void CheckDataLabelInsertion(ChartSeries series, int valueIndex, ChartXValue xValue,
            ChartYValue yValue, int expectedLabelInsertionIndex, int expectedInsertedLabelCount)
        {
            ChartDataLabelCollection labels = series.DataLabels;
            // Use data point count to exclude possible not used labels left after data removal and extra label of
            // an OfPieChart chart.
            int pointCount = series.DataPoints.Count;
            ChartDataLabel[] oldLabels = new ChartDataLabel[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                if (labels.HasNonDefaultItemFormatting(i))
                    oldLabels[i] = labels[i];
            }

            bool isLabelInsertionExpected = (expectedLabelInsertionIndex >= 0);

            // Insert the data values.
            series.Insert(valueIndex, xValue, yValue);

            Assert.That(series.DataPoints.Count, Is.EqualTo(pointCount + expectedInsertedLabelCount));

            int lastInsertedLabelIndex = isLabelInsertionExpected
                ? expectedLabelInsertionIndex + expectedInsertedLabelCount - 1
                : -1;

            foreach (ChartDataLabel label in labels.MaterializedDataLabels)
            {
                int index = label.Index;
                if (!isLabelInsertionExpected || (index < expectedLabelInsertionIndex))
                {
                    Assert.That(label, Is.SameAs(oldLabels[index]));
                    Assert.That(oldLabels[index].Index, Is.EqualTo(index));
                }
                else if (index <= lastInsertedLabelIndex)
                {
                    Assert.Fail("No materialized labels expected in this label range.");
                }
                else
                {
                    Assert.That(label, Is.SameAs(oldLabels[index - expectedInsertedLabelCount]));
                    Assert.That(oldLabels[index - expectedInsertedLabelCount].Index, Is.EqualTo(index));
                }
            }
        }

        /// <summary>
        /// Checks that labels are updated if necessary in the data label collection after changing data value with
        /// the specified index.
        /// </summary>
        /// <remarks>
        /// There may be zero, one or several labels that correspond to some chart data value.
        /// </remarks>
        /// <param name="series">The series whose label removal is being checked.</param>
        /// <param name="valueIndex">The index of the data value to change.</param>
        /// <param name="newXValue">The X value to set at the specified index.</param>
        /// <param name="expectedAffectedLabelIndex">The index of the start of the label range that is affected by the
        /// data value change.</param>
        /// <param name="expectedDeletedLabelCount">The number of the labels that is expected to be removed after the
        /// data value change.</param>
        /// <param name="expectedInsertedLabelCount">The number of the labels that is expected to be inserted after the
        /// data value change.</param>
        private static void CheckDataLabelChange(ChartSeries series, int valueIndex, ChartXValue newXValue,
            int expectedAffectedLabelIndex, int expectedDeletedLabelCount, int expectedInsertedLabelCount)
        {
            ChartDataLabelCollection labels = series.DataLabels;
            // Use data point count to exclude possible not used labels left after data removal and extra label of
            // an OfPieChart chart.
            int pointCount = series.DataPoints.Count;
            ChartDataLabel[] oldLabels = new ChartDataLabel[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                if (labels.HasNonDefaultItemFormatting(i))
                    oldLabels[i] = labels[i];
            }

            int labelCountChange = expectedInsertedLabelCount - expectedDeletedLabelCount;

            // Change the data values.
            series.XValues[valueIndex] = newXValue;

            Assert.That(series.DataPoints.Count, Is.EqualTo(pointCount + labelCountChange));

            int lastDeletedLabelIndex = (expectedDeletedLabelCount > 0)
                ? expectedAffectedLabelIndex + expectedDeletedLabelCount - 1
                : -1;

            for (int i = 0; i < oldLabels.Length; i++)
            {
                ChartDataLabel label = oldLabels[i];
                if (label == null)
                    continue;

                if (i < expectedAffectedLabelIndex)
                {
                    Assert.That(labels[i], Is.SameAs(label));
                    Assert.That(label.Index, Is.EqualTo(i));
                }
                else if (i <= lastDeletedLabelIndex)
                {
                    Assert.That(labels.Contains(label), Is.False);
                }
                else
                {
                    Assert.That(labels[i + labelCountChange], Is.SameAs(label));
                    Assert.That(label.Index, Is.EqualTo(i + labelCountChange));
                }
            }
        }

        /// <summary>
        /// Gets text representation of the specified chart series value to display in a data label.
        /// </summary>
        private static string GetLabelValueText(ChartSeries series, int valueIndex)
        {
            if ((series.LayoutType == SeriesLayout.Treemap) || (series.LayoutType == SeriesLayout.Sunburst))
            {
                DmlChartMultiLvlStrValue value = (DmlChartMultiLvlStrValue)series.X.Values[valueIndex];
                for (int level = 0; level < value.LevelsCount; level++)
                {
                    if (value.Levels[level] != null)
                        return (string)value.Levels[level];
                }

                return string.Empty;
            }
            else
            {
                double unit = ((IDmlChart2D)series.Chart).AxY.DisplayUnit.GetActualUnit();
                double value = series.Y.Values[valueIndex].Value / unit;
                return value.ToString().Replace(".", ",");
            }
        }

        /// <summary>
        /// Checks that no materialized data labels and data points exist in the series.
        /// </summary>
        private static void EnsureNoDataPointsAndLabelsExist(ChartSeries series)
        {
            // Check that there are no labels left in the collection.
            Assert.That(series.DataLabels.MaterializedDataLabels.GetEnumerator().MoveNext(), Is.False);
            // Check that there are no data points left in the collection.
            Assert.That(series.DataPoints.MaterializedDataPoints.GetEnumerator().MoveNext(), Is.False);
        }

        private const string NumericXTestFileNameTemplate = @"Model\Charts\DataApi\TestFillData_{0}_NumericX.docx";
    }
}
