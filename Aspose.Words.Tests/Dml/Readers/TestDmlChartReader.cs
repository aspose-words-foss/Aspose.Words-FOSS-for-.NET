// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/07/2012 by Alexey Noskov
using System.IO;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlChartReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlChartReader
    {
        [Test]
        public void TestAreaChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestAreaChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));
            
            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.AreaChart));
            Assert.That(chart.Series, IsNot.Null());
            Assert.That(chart.Series.Count, Is.EqualTo(2));

            DmlAreaChart areaChart = (DmlAreaChart)chart;
            Assert.That(areaChart.AxX, IsNot.Null());
            Assert.That(areaChart.AxX.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(areaChart.AxX.CategoryType, Is.EqualTo(AxisCategoryType.Automatic));
            Assert.That(areaChart.AxX.IsDateCategoryAxis, Is.True);
            Assert.That(areaChart.AxY, IsNot.Null());
            Assert.That(areaChart.AxY.Type, Is.EqualTo(ChartAxisType.Value));
        }

        [Test]
        public void TestBarChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestBarChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.BarChart));
            Assert.That(chart.Series, IsNot.Null());
            Assert.That(chart.Series.Count, Is.EqualTo(3));

            DmlBarChart areaChart = (DmlBarChart)chart;
            Assert.That(areaChart.AxX, IsNot.Null());
            Assert.That(areaChart.AxX.Type, Is.EqualTo(ChartAxisType.Value));
            Assert.That(areaChart.AxY, IsNot.Null());
            Assert.That(areaChart.AxY.Type, Is.EqualTo(ChartAxisType.Category));
            Assert.That(areaChart.AxY.CategoryType, Is.EqualTo(AxisCategoryType.Automatic));
            Assert.That(areaChart.AxX.IsDateCategoryAxis, Is.False);
        }


        [Test]
        public void TestBubbleChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestBubbleChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.BubbleChart));
        }

        [Test]
        public void TestDoughnutChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestDoughnutChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.DoughnutChart));
        }

        [Test]
        public void TestLineChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestLineChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.LineChart));
        }

        [Test]
        public void TestPieChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestPieChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.PieChart));
        }

        [Test]
        public void TestRadar()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestRadar.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;
            
            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.RadarChart));
        }

        [Test]
        public void TestScatterChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestScatterChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.ScatterChart));
        }

        [Test]
        public void TestStockChart()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestStockChart.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.StockChart));
        }

        [Test]
        public void TestSurface3D()
        {
            DmlChartSpace chartSpace = OpenChart(@"Model\Charts\RawXml\TestSurface3d.xml");
            Assert.That(chartSpace, IsNot.Null());
            Assert.That(chartSpace.ChartFormat, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea, IsNot.Null());
            Assert.That(chartSpace.ChartFormat.PlotArea.Charts.Count, Is.EqualTo(1));

            DmlChart chart = chartSpace.FirstChart;

            Assert.That(chart, IsNot.Null());
            Assert.That(chart.ChartType, Is.EqualTo(DmlChartType.Surface3DChart));
        }

        private DmlChartSpace OpenChart(string fileName)
        {
            fileName = TestUtil.BuildTestFileName(fileName);

            DmlChartSpace chartSpace = new DmlChartSpace();

            using (FileStream fs = File.OpenRead(fileName))
            {
                DocxDocumentReaderStub reader = new DocxDocumentReaderStub(fs);
                DmlChartReader.Read(reader, chartSpace);
            }

            return chartSpace;
        }
    }
}
