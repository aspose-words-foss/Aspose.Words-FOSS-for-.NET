// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2020 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests behavior of the <see cref="ChartDataPoint"/> and <see cref="ChartDataPointCollection"/> classes.
    /// </summary>
    [TestFixture]
    public class TestChartDataPoint
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestInheritedPropertyValues()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;

            chart.Series.Clear();
            ChartSeries series = chart.Series.Add("AW Series 1",
                new DateTime[] { new DateTime(2002, 01, 01), new DateTime(2002, 06, 01), new DateTime(2002, 07, 01),
                    new DateTime(2002, 08, 01), new DateTime(2002, 09, 01) },
                new double[] { 640, 120, 280, 120, 150 });

            Assert.That(((IChartDataPoint)series).Marker.Symbol, Is.EqualTo(MarkerSymbol.None));

            ChartDataPoint point1 = series.DataPoints[1];
            point1.InvertIfNegative = true;
            point1.Marker.Size = 12;

            Assert.That(point1.Marker.Symbol, Is.EqualTo(MarkerSymbol.None));

            ((IChartDataPoint)series).Marker.Symbol = MarkerSymbol.Diamond;
            ((IChartDataPoint)series).Marker.Size = 24;

            Assert.That(point1.Marker.Symbol, Is.EqualTo(MarkerSymbol.Diamond));
            Assert.That(point1.Marker.MarkerPr.GetDirectProperty(DmlChartMarkerAttr.Symbol), Is.Null);

            ChartDataPoint point2 = series.DataPoints[2];
            point2.InvertIfNegative = true;
            Assert.That(point2.Marker.Symbol, Is.EqualTo(MarkerSymbol.Diamond));

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestInheritedPropertyValues.docx", null, false);

            chart = doc.FirstSection.Body.Shapes[0].Chart;
            series = chart.Series[0];
            point1 = series.DataPoints[1];

            Assert.That(point1.Marker.Symbol, Is.EqualTo(MarkerSymbol.Diamond));
            Assert.That(point1.Marker.MarkerPr.GetDirectProperty(DmlChartMarkerAttr.Symbol), Is.Null);
            Assert.That(point1.Marker.Size, Is.EqualTo(12));
            Assert.That(point1.InvertIfNegative, Is.True);

            point2 = series.DataPoints[2];
            Assert.That(point2.InvertIfNegative, Is.True);
            Assert.That(point2.Marker.Symbol, Is.EqualTo(MarkerSymbol.Diamond));
        }


        /// <summary>
        /// WORDSNET-18874 Tests the <see cref="ChartDataPoint.ClearFormat"/> and
        /// <see cref="ChartDataPointCollection.ClearFormat"/> public methods.
        /// </summary>
        [Test]
        public void TestClearFormat()
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            ChartSeries series = chart.Series[0];
            ChartDataPointCollection points = series.DataPoints;

            ChartDataPoint point1 = points[0];
            ChartDataPoint point2 = points[1];

            point1.InvertIfNegative = true;
            point1.Marker.Symbol = MarkerSymbol.Square;
            point2.InvertIfNegative = true;
            point2.Marker.Symbol = MarkerSymbol.Diamond;

            point1.ClearFormat();

            Assert.That(point1.InvertIfNegative, Is.False);
            Assert.That(point1.Marker.Symbol, Is.EqualTo(series.DefaultDataPoint.Marker.Symbol));

            points.ClearFormat();
            Assert.That(point2.InvertIfNegative, Is.False);
            Assert.That(point2.Marker.Symbol, Is.EqualTo(series.DefaultDataPoint.Marker.Symbol));
        }

        /// <summary>
        /// WORDSNET-18874 Tests data point count if a collection contains a point with index larger than
        /// the index of the last point that can be displayed.
        /// </summary>
        [Test]
        public void TestPointCountWhenContainingOutsidePoint()
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartSeries series = chart.Series[0];

            CheckPointCount(series, 3);

            ChartDataPoint point = series.DataPoints[5];
            CheckPointCount(series, 3);

            point.Marker.Symbol = MarkerSymbol.Plus;
            // Now the data point #5 has non-default value and is included in Count value.
            CheckPointCount(series, 4);

            series.DataPoints[5000].InvertIfNegative = true;
            CheckPointCount(series, 5);
        }

        /// <summary>
        /// WORDSNET-24940 Wrong value of ChartDataPoint.InvertIfNegative property
        /// The 'invertIfNegative' attribute is processed in a different way in MS Word than the other ones: its
        /// default value depends on a document version and when resolving a value of the attribute for a data point,
        /// the parent collection (series) is not taken into account. The necessary fixes has been implement in
        /// Aspose.Word to bypass these differences.
        /// </summary>
        [TestCase((int)MsWordVersionCore.Word2007)]
        [TestCase((int)MsWordVersionCore.Word2010)]
        public void TestInvertIfNegative(int documentVersion)
        {
            Document doc = new Document();
            doc.BuiltInDocumentProperties.Version =
                (doc.BuiltInDocumentProperties.Version & 0xff) + (documentVersion << 16);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertChart(ChartType.Bar, 432, 252);

            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            ChartSeries series1 = chart.Series[0];
            // Define some formatting.
            series1.DataPoints[0].Format.Fill.Solid(Color.Green);
            series1.DataPoints[1].Format.Fill.Solid(Color.Cyan);

            series1.DefaultDataPoint.PointPr.RemoveProperty(DmlChartDataPointAttr.InvertIfNegative);

            MsWordVersionCore version = (MsWordVersionCore)documentVersion;
            bool expectedDefaultValue = (version > MsWordVersionCore.Word2007);
            CheckInvertIfNegative(series1, expectedDefaultValue, expectedDefaultValue, expectedDefaultValue);

            series1.InvertIfNegative = true;
            CheckInvertIfNegative(series1, true, true, true);

            series1.DataPoints[1].InvertIfNegative = false;
            CheckInvertIfNegative(series1, true, true, false);

            ChartSeries series2 = chart.Series[1];
            // Define some formatting.
            series2.DataPoints[0].Format.Fill.Solid(Color.Green);
            series2.DataPoints[1].Format.Fill.Solid(Color.Cyan);

            series2.InvertIfNegative = false;
            CheckInvertIfNegative(series2, false, false, false);

            series2.DataPoints[1].InvertIfNegative = true;
            CheckInvertIfNegative(series2, false, false, true);

            doc = TestUtil.SaveOpen(doc, string.Format(@"Model\Charts\TestInvertIfNegative{0}", version.ToString()),
                UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);

            chart = doc.FirstSection.Body.Shapes[0].Chart;

            series1 = chart.Series[0];
            CheckInvertIfNegative(series1, true, true, false);

            series2 = chart.Series[1];
            CheckInvertIfNegative(series2, false, false, true);
        }

        /// <summary>
        /// Tests the public <see cref="ChartDataPointCollection.CopyFormat"/> and <see cref="ChartSeries.CopyFormatFrom"/>
        /// methods.
        /// </summary>
        [Test]
        public void TestCopyingFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestCopyingDataPointFormat.docx");
            ChartSeries series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            ChartDataPointCollection dataPoints1 = series1.DataPoints;
            ChartSeries series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            ChartDataPointCollection dataPoints2 = series2.DataPoints;

            Assert.That(dataPoints1.HasDefaultFormat(1), Is.False);
            Assert.That(dataPoints1.HasDefaultFormat(2), Is.True);
            Assert.That(dataPoints2.HasDefaultFormat(1), Is.False);
            Assert.That(dataPoints2.HasDefaultFormat(2), Is.True);

            dataPoints1.CopyFormat(1, 2);
            dataPoints2.CopyFormat(1, 2);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestCopyingDataPointFormatForItem.docx");

            series1.CopyFormatFrom(1);
            series2.CopyFormatFrom(1);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestCopyingDataPointFormatForCollection.docx");
        }

        /// <summary>
        /// Checks the <see cref="ChartSeries.InvertIfNegative"/> property of the specified series and the
        /// <see cref="ChartDataPoint.InvertIfNegative"/> property of the two first data points.
        /// </summary>
        private static void CheckInvertIfNegative(ChartSeries series, bool expectedSeriesValue,
            bool expectedDataPoint1Value, bool expectedDataPoint2Value)
        {
            Assert.That(series.InvertIfNegative, Is.EqualTo(expectedSeriesValue));
            Assert.That(series.DataPoints[0].InvertIfNegative, Is.EqualTo(expectedDataPoint1Value));
            Assert.That(series.DataPoints[1].InvertIfNegative, Is.EqualTo(expectedDataPoint2Value));
        }

        /// <summary>
        /// Checks point count of the series.
        /// </summary>
        private static void CheckPointCount(ChartSeries series, int expectedCount)
        {
            Assert.That(series.DataPoints.Count, Is.EqualTo(expectedCount));

            // Check enumerator too.
            int count = 0;
            foreach (ChartDataPoint point in series.DataPoints)
                count++;
            Assert.That(count, Is.EqualTo(expectedCount));
        }

        /// <summary>
        /// Creates a document containing a chart.
        /// </summary>
        private static Document CreateDocumentWithChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;

            // Delete default generated series.
            chart.Series.Clear();

            string[] categories = new string[] { "AW Category 1", "AW Category 2", "AW Category 3" };
            chart.Series.Add("AW Series 1", categories, new double[] { 4.3, 2.5, 3.5 });

            return doc;
        }
    }
}
