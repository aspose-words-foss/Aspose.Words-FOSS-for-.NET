// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/25/2014 by Alexey Noskov

using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Simple open/save/compare tests for DOCX document with charts.
    /// Used to make sure charts are written properly from the model.
    /// Use test documents from charts features rendering tests.
    /// </summary>
    [TestFixture]
    public class TestDmlChartFeatures
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
        public void TestNegativeValues()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestNegativeValues.docx");
        }


        [Test]
        public void TestAxisPosition()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisPosition.docx");
        }

        [Test]
        public void TestAxisDirection()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisDirection.docx");
        }

        [Test]
        public void TestAxisDirection2()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisDirection2.docx");
        }

        [Test]
        public void TestLabelRotationA()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLabelRotationA.docx");
        }

        [Test]
        public void TestLabelRotationB()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLabelRotationB.docx");
        }


        [Test]
        public void TestOffsets()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestOffsets.docx");
        }



        [Test]
        public void TestStackedCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestStackedCharts.docx");
        }

        [Test]
        public void TestStackedBarCharts()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestStackedBarCharts.docx");
        }


        [Test]
        public void TestLegendPosition()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLegendPosition.docx");
        }


        /// <summary>
        /// WORDSNET-6935 Support Data Point formatting.
        /// </summary>
        [Test]
        public void TestDataPointFormattingBarChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataPointFormattingBarChart.docx");
        }

        /// <summary>
        /// WORDSNET-6935 Support Data Point formatting.
        /// </summary>
        [Test]
        public void TestDataPointFormattingLineChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataPointFormattingLineChart.docx");
        }




        /// <summary>
        /// WORDSNET-6935 Support Data Point formatting.
        /// </summary>
        [Test]
        public void TestDataPointFormattingStockChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataPointFormattingStockChart.docx");
        }

        /// <summary>
        /// WORDSNET-6961 Support gap width upon rendering bar charts.
        /// </summary>
        [Test]
        public void TestBarChartGapWidth()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestBarChartGapWidth.docx");
        }

        /// <summary>
        /// WORDSNET-6961 Support gap width upon rendering bar charts.
        /// </summary>
        [Test]
        public void TestStockChartGapWidth()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestStockChartGapWidth.docx");
        }

        /// <summary>
        /// WORDSNET-6959 Support explosion upon rendering Pie charts.
        /// </summary>
        [Test]
        public void TestPieChartExplosion()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestPieChartExplosion.docx");
        }

        /// <summary>
        /// WORDSNET-6959 Support explosion upon rendering Pie charts.
        /// </summary>
        [Test]
        public void TestDoughnutChartExplosion()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDoughnutChartExplosion.docx");
        }

        /// <summary>
        /// WORDSNET-6939 Support custom axis formatting.
        /// </summary>
        [Test]
        public void TestAxisFormatting()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisFormatting.docx");
        }

        /// <summary>
        /// WORDSNET-6938 Support elements overlapping on chart area.
        /// </summary>
        [Test]
        public void TestOverlapping()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestOverlapping.docx");
        }

        /// <summary>
        /// WORDSNET-6937 Support manual layout of chart elements.
        /// </summary>
        [Test]
        public void TestManualLayout()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestManualLayout.docx");
        }

        /// <summary>
        /// WORDSNET-6940 Support custom legend formatting.
        /// </summary>
        [Test]
        public void TestLegendFormatting()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLegendFormatting.docx");
        }

        /// <summary>
        /// WORDSNET-6936 Support custom font formatting.
        /// </summary>
        [Test]
        public void TestFontFormatting()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestFontFormatting.docx");
        }

        [Test]
        public void TestLegendEntry()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLegendEntry.docx");
        }

        /// <summary>
        /// WORDSNET-6942 Render data labels
        /// </summary>
        [Test]
        public void TestDataLabels()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataLabels.docx");
        }

        /// <summary>
        /// WORDSNET-6942 Render data labels
        /// </summary>
        [Test]
        public void TestDataLabelsBarChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataLabelsBarChart.docx");
        }

        /// <summary>
        /// WORDSNET-6942 Render data labels
        /// </summary>
        [Test]
        public void TestDataLabelsPieChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataLabelsPieChart.docx");
        }

        /// <summary>
        /// WORDSNET-6941 Render axis title.
        /// </summary>
        [Test]
        public void TestAxisTitle()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisTitle.docx");
        }

        /// <summary>
        /// WORDSNET-6776 Support fixed maximum and minimum values of axis.
        /// Tests scaling of value axis.
        /// </summary>
        [Test]
        public void TestAxisScalingA()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisScalingA.docx");
        }

        /// <summary>
        /// WORDSNET-6776 Support fixed maximum and minimum values of axis.
        /// Tests scaling of date axis.
        /// </summary>
        [Test]
        public void TestAxisScalingB()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisScalingB.docx");
        }

        /// <summary>
        /// WORDSNET-6776 Support fixed maximum and minimum values of axis.
        /// Tests scaling of radar chart value axis.
        /// </summary>
        [Test]
        public void TestAxisScalingC()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisScalingC.docx");
        }

        /// <summary>
        /// WORDSNET-6776 Support fixed maximum and minimum values of axis.
        /// Tests scaling of stacked charts.
        /// </summary>
        [Test]
        public void TestAxisScalingD()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisScalingD.docx");
        }

        /// <summary>
        /// Test auto axis scaling when minimum and maximum values of axis are close to each other.
        /// </summary>
        [Test]
        public void TestAxisScalingE()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisScalingE.docx");
        }

        /// <summary>
        /// WORDSNET-6946 Support rendering Pictures, Shapes and Textboxes on Chart Area
        /// </summary>
        [Test]
        public void TestChartShapes()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestChartShapes.docx");
        }

        /// <summary>
        /// WORDSNET-6945 Render Error Bars.
        /// </summary>
        [Test]
        public void TestErrorBarsLineChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestErrorBarsLineChart.docx");
        }

        /// <summary>
        /// WORDSNET-6945 Render Error Bars.
        /// </summary>
        [Test]
        public void TestErrorBarsBarChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestErrorBarsBarChart.docx");
        }

        /// <summary>
        /// WORDSNET-6945 Render Error Bars.
        /// </summary>
        [Test]
        public void TestErrorBarsScatterChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestErrorBarsScatterChart.docx");
        }

        /// <summary>
        /// WORDSNET-6945 Render Error Bars.
        /// </summary>
        [Test]
        public void TestErrorBarsAreaChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestErrorBarsAreaChart.docx");
        }

        /// <summary>
        /// WORDSNET-6945 Render Error Bars.
        /// </summary>
        [Test]
        public void TestErrorBarsBubbleChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestErrorBarsBubbleChart.docx");
        }

        /// <summary>
        /// WORDSNET-6945 Render Error Bars.
        /// </summary>
        [Test]
        public void TestErrorBarsStockChart()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestErrorBarsStockChart.docx");
        }

        /// <summary>
        /// WORDSNET-7262 Support rendering of minor grid lines.
        /// </summary>
        [Test]
        public void TestMinorGridLines()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestMinorGridLines.docx");
        }

        /// <summary>
        /// WORDSNET-7365 Support rendering of up-down bars.
        /// </summary>
        [Test]
        public void TestUpDownBars()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestUpDownBars.docx");
        }

        /// <summary>
        /// WORDSNET-7243 Support render legend markers in data labels.
        /// </summary>
        [Test]
        public void TestLegendMarkers()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLegendMarkers.docx");
        }

        /// <summary>
        /// WORDSNET-7366 Support rendering high-low lines.
        /// </summary>
        [Test]
        public void TestHighLowLines()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestHighLowLines.docx");
        }

        /// <summary>
        /// WORDSNET-7366 Support rendering high-low lines.
        /// </summary>
        [Test]
        public void TestDropLines()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDropLines.docx");
        }

        /// <summary>
        /// WORDSNET-7504 Improve bubble size calculation algorithm.
        /// </summary>
        [Test]
        public void TestBubbleSize()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestBubbleSize.docx");
        }

        /// <summary>
        /// WORDSNET-7407 The Chart's axis values are different after conversion from Docx to PDF
        /// </summary>
        [Test]
        public void TestVerticalAxisLabels()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestVerticalAxisLabels.docx");
        }

        /// <summary>
        /// WORDSNET-7407 The Chart's axis values are different after conversion from Docx to PDF
        /// </summary>
        [Test]
        public void TestVerticalAxisLabelsNegative()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestVerticalAxisLabelsNegative.docx");
        }

        /// <summary>
        /// WORDSNET-7457 Improve axis labels auto rotation algorithm
        /// </summary>
        [Test]
        public void TestAxisLabelRotation()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisLabelRotation.docx");
        }

        /// <summary>
        /// WORDSNET-7457 Improve axis labels auto rotation algorithm
        /// </summary>
        [Test]
        public void TestAxisLabelRotationStandardAngles()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisLabelRotationStandardAngles.docx");
        }

        /// <summary>
        /// WORDSNET-7457 Improve axis labels auto rotation algorithm
        /// </summary>
        [Test]
        public void TestAxisLabelRotationCustomAngles()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestAxisLabelRotationCustomAngles.docx");
        }

        /// <summary>
        /// WORDSNET-7244 Support rendering of leader lines upon rendering data labels.
        /// WORDSNET-13916 Support rendering of leader lines for all chart types.
        /// </summary>
        [TestCase(@"Model\Charts\TestLeaderLines.docx")]
        [TestCase(@"Model\Charts\TestLeaderLinesAreaChart.docx")]
        [TestCase(@"Model\Charts\TestLeaderLinesBarChart.docx")]
        [TestCase(@"Model\Charts\TestLeaderLinesLineChart.docx")]
        [TestCase(@"Model\Charts\TestLeaderLinesRadarChart.docx")]
        [TestCase(@"Model\Charts\TestLeaderLinesScatterChart.docx")]
        public void TestLeaderLines(string testFileName)
        {
            TestUtil.OpenSaveOpen(testFileName);
        }

        /// <summary>
        /// WORDSNET-6954 Improve work with brushes upon rendering charts.
        /// </summary>
        [Test]
        public void TestLinearGradient()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestLinearGradient.docx");
        }

        /// <summary>
        /// WORDSNET-8105 Support rendering of Linear trendlines.
        /// </summary>
        [Test]
        public void TestTrendlinesLinear()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesLinear.docx");
        }

        /// <summary>
        /// WORDSNET-8107 Support rendering of Moving Average trendlines.
        /// </summary>
        [Test]
        public void TestTrendlinesMovingAverage()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesMovingAverage.docx");
        }

        /// <summary>
        /// WORDSNET-8108 Support rendering of Polynomial trendlines.
        /// </summary>
        [Test]
        public void TestTrendlinesPolynomial()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesPolynomial.docx");
        }

        /// <summary>
        /// WORDSNET-8104 Support rendering of Exponential trendlines.
        /// </summary>
        [Test]
        public void TestTrendlinesExponential()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesExponential.docx");
        }

        /// <summary>
        /// WORDSNET-8106 Support rendering of Logarithmic trendlines.
        /// </summary>
        [Test]
        public void TestTrendlinesLogarithmic()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesLogarithmic.docx");
        }

        /// <summary>
        /// WORDSNET-8109 Support rendering of Power trendlines.
        /// </summary>
        [Test]
        public void TestTrendlinesPower()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesPower.docx");
        }

        /// <summary>
        /// WORDSNET-8191 Support Overlap option upon rendering bar charts.
        /// </summary>
        [Test]
        public void TestBarChartOverlap()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestBarChartOverlap.docx");
        }

        /// <summary>
        /// WORDSNET-6943 Render data table.
        /// </summary>
        [Test]
        public void TestDataTable()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataTable.docx");
        }

        /// <summary>
        /// WORDSNET-8867 Support "Display Units" upon rendering axis labels.
        /// </summary>
        [Test]
        public void TestDisplayUnits()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDisplayUnits.docx");
        }

        /// <summary>
        /// WORDSNET-8123 Support trendlines intercept.
        /// </summary>
        [Test]
        public void TestTrendlineIntercept()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlineIntercept.docx");
        }

        /// <summary>
        /// WORDSNET-8110 Support trendlines forecast.
        /// </summary>
        [Test]
        public void TestTrendlinesForecast()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTrendlinesForecast.docx");
        }

        /// <summary>
        /// WORDSNET-8974 Investigate rendering charts with random date value step.
        /// </summary>
        [Test]
        public void TestDateAxisRandomStep()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDateAxisRandomStep.docx");
        }

        /// <summary>
        /// WORDSNET-8999 Support tick mark type.
        /// </summary>
        [Test]
        public void TestTickMarkType()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestTickMarkType.docx");
        }

        /// <summary>
        /// WORDSNET-9178 Improve auto color mechanism for data points.
        /// </summary>
        [Test]
        public void TestDataPointAutoColor()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataPointAutoColor.docx");
        }

        /// <summary>
        /// WORDSNET-7245 Support BestFit position of Pie chart's data labels.
        /// </summary>
        [Test]
        public void TestDataLabelBestFitPosition()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\TestDataLabelBestFitPosition.docx");
        }
    }
}
