// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/18/2015 by Andrey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder
{
    [TestFixture]
    public class TestDocBuilderCharts
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            SystemPal.SetStandardCulture();
            SystemPal.SetStandardUICulture();
        }

        [TearDown]
        public void TearDown()
        {
            SystemPal.RestoreCulture();
            SystemPal.RestoreUICulture();
        }

        [Test]
        public void TestInsertChartAll()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
                chart.Series.Clear();
                HandleChartTypeSpecifics(chartType, chart);
            }

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestInsertChartAll.docx");

            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            Assert.That(shapes.Count, Is.EqualTo(42));
        }

        [Test]
        public void TestInsertChartAllWithDefaultData()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"DocBuilder\Chart\TestInsertChartAllWithDefaultData.docx");
        }

        [Test]
        public void TestInsertChartColumn()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            Assert.That(chart.Title.Show, Is.True);

            // Setting chart Title.
            chart.Title.Text = "Sample Column Chart Title";

            ChartSeriesCollection seriesColl = chart.Series;

            // Check default generated series count.
            Assert.That(seriesColl.Count, Is.EqualTo(3));

            // Delete default generated series.
            seriesColl.Clear();

            // Check Series count after clear.
            Assert.That(seriesColl.Count, Is.EqualTo(0));

            // Create category names array.
            string[] categories = new string[] {"AW Category 1", "AW Category 2"};

            // Adding new series
            seriesColl.Add("AW Series 1", categories, new double[] { 1, 2 });
            seriesColl.Add("AW Series 2", categories, new double[] { 3, 4 });
            seriesColl.Add("AW Series 3", categories, new double[] { 5, 6 });
            seriesColl.Add("AW Series 4", categories, new double[] { 7, 8 });
            seriesColl.Add("AW Series 5", categories, new double[] { 9, 10 });

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestInsertChartColumn.docx");

            Shape s1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            // Check size is correct.
            Assert.That(s1.Width, Is.EqualTo(432).Within(0.5));
            Assert.That(s1.Height, Is.EqualTo(252).Within(0.5));

            chart = s1.Chart;

            seriesColl = chart.Series;

            // Check chart Title text.
            Assert.That(chart.Title.Text, Is.EqualTo("Sample Column Chart Title"));

            // Check Series count.
            Assert.That(seriesColl.Count, Is.EqualTo(5));

            // Get first chart series.
            ChartSeries firstChartSeries = seriesColl[0];

            // Check chart series name.
            Assert.That(firstChartSeries.Name, Is.EqualTo("AW Series 1"));

            // Get ChartDataPoint collection for the first series.
            ChartDataPointCollection dmlChartDataPointCollection = firstChartSeries.DataPoints;
            Assert.That(dmlChartDataPointCollection.MaterializedDataPoints.GetEnumerator().MoveNext(), Is.False);

            // Get default ChartDataPoint for the first series.
            Assert.That(firstChartSeries.Explosion, Is.EqualTo(-1));
            Assert.That(firstChartSeries.Index, Is.EqualTo(0));
            Assert.That(firstChartSeries.InvertIfNegative, Is.False);
            Assert.That(firstChartSeries.Marker.Size, Is.EqualTo(7));
            Assert.That(firstChartSeries.Marker.Symbol, Is.EqualTo(MarkerSymbol.Default));
        }

        [Test]
        public void TestShowAutoTitle()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;
            Assert.That(chart.Title.Show, Is.True);

            chart.Title.Show = false;
            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestShowAutoTitle.docx");

            Shape s1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(s1.Chart.Title.Show, Is.False);
        }

        [Test]
        public void TestPieChartExplosion()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape1 = builder.InsertChart(ChartType.Pie, 432, 252);
            Chart shape1Chart = shape1.Chart;
            shape1Chart.Title.Text = "Pie Chart Explosion";

            Shape shape2 = builder.InsertChart(ChartType.Pie3D, 432, 252);
            Chart shape2Chart = shape2.Chart;
            shape2Chart.Title.Text = "Pie3D Chart Explosion";

            // Delete default generated series.
            shape1Chart.Series.Clear();
            shape2Chart.Series.Clear();

            CreatePieChartData(shape1Chart);
            CreatePieChartData(shape2Chart);

            // Change default explosion.
            shape1Chart.Series[0].Explosion = 20;
            shape2Chart.Series[0].Explosion = 5;

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestPieChartExplosion.docx");

            shape1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape1.Chart.Series[0].Explosion, Is.EqualTo(20));

            shape2 = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape2.Chart.Series[0].Explosion, Is.EqualTo(5));
        }

        [Test]
        public void TestLineChartMarkers()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            chart.Title.Text = "Line Chart With Markers";

            // Delete default generated series.
            chart.Series.Clear();

            CreateStandardChartData(chart);

            // Change default marker size and symbol.
            ChartMarker marker = chart.Series[0].Marker;
            marker.Size = 15;
            marker.Symbol = MarkerSymbol.Diamond;

            marker = chart.Series[1].Marker;
            marker.Size = 10;
            marker.Symbol = MarkerSymbol.Circle;

            marker = chart.Series[2].Marker;
            marker.Size = 5;
            marker.Symbol = MarkerSymbol.Triangle;

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestLineChartMarkers.docx");

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            marker = shape.Chart.Series[0].Marker;
            Assert.That(marker.Size, Is.EqualTo(15));
            Assert.That(marker.Symbol, Is.EqualTo(MarkerSymbol.Diamond));

            marker = shape.Chart.Series[1].Marker;
            Assert.That(marker.Size, Is.EqualTo(10));
            Assert.That(marker.Symbol, Is.EqualTo(MarkerSymbol.Circle));

            marker = shape.Chart.Series[2].Marker;
            Assert.That(marker.Size, Is.EqualTo(5));
            Assert.That(marker.Symbol, Is.EqualTo(MarkerSymbol.Triangle));
        }

        [Test]
        public void TestChartDataPoints1()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            chart.Title.Text = "Line Chart With Custom Data Points";

            // Delete default generated series.
            chart.Series.Clear();

            CreateStandardChartData(chart);

            ChartSeries series0 = chart.Series[0];
            ChartSeries series1 = chart.Series[1];

            // Change first series name.
            series0.Name = "NewSeries1Name";

            Assert.That(series0.DataPoints.MaterializedDataPoints.GetEnumerator().MoveNext(), Is.False);
            Assert.That(series0.DataPoints.Count, Is.EqualTo(4));

            // Get DataPoint of the first and second point of the first series.
            ChartDataPoint dataPoint00 = series0.DataPoints[0];
            ChartDataPoint dataPoint01 = series0.DataPoints[1];

            // Change some DataPoint props.
            dataPoint00.Explosion = 50;
            dataPoint00.Marker.Symbol = MarkerSymbol.Circle;
            dataPoint00.Marker.Size = 15;

            dataPoint01.Marker.Symbol = MarkerSymbol.Diamond;
            dataPoint01.Marker.Size = 20;

            // Get DataPoint of the third point of the second series.
            ChartDataPoint dataPoint12 = series1.DataPoints[2];
            dataPoint12.InvertIfNegative = true;
            dataPoint12.Marker.Symbol = MarkerSymbol.Star;
            dataPoint12.Marker.Size = 20;

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartDataPoints1.docx");

            // Check everything were preserved.
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            series0 = shape.Chart.Series[0];
            series1 = shape.Chart.Series[1];

            Assert.That(series0.DataPoints.Count, Is.EqualTo(4));
            Assert.That(series1.DataPoints.Count, Is.EqualTo(4));

            dataPoint00 = series0.DataPoints[0];

            // There is no way to set explosion for line chart, that is why -1.
            Assert.That(dataPoint00.Explosion, Is.EqualTo(-1));
            Assert.That(dataPoint00.Marker.Symbol, Is.EqualTo(MarkerSymbol.Circle));
            Assert.That(dataPoint00.Marker.Size, Is.EqualTo(15));

            dataPoint01 = series0.DataPoints[1];
            Assert.That(dataPoint01.Explosion, Is.EqualTo(-1));
            Assert.That(dataPoint01.Marker.Symbol, Is.EqualTo(MarkerSymbol.Diamond));
            Assert.That(dataPoint01.Marker.Size, Is.EqualTo(20));

            dataPoint12 = series1.DataPoints[2];
            Assert.That(dataPoint12.InvertIfNegative, Is.True);
        }

        [Test]
        public void TestLineChartSmooth()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Assert.That(shape.HasChart, Is.True);
            Chart chart = shape.Chart;
            chart.Title.Text = "Line Chart Smooth";

            // Delete default generated series.
            chart.Series.Clear();

            CreateStandardChartData(chart);

            ChartSeries series0 = chart.Series[0];
            Assert.That(series0.Smooth, Is.False);
            series0.Smooth = true;

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestLineChartSmooth.docx");

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            series0 = shape.Chart.Series[0];
            Assert.That(series0.Smooth, Is.True);
        }

        [Test]
        public void TestChartDataLabels()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            chart.Title.Text = "Line Chart With Data Labels";

            // Delete default generated series.
            chart.Series.Clear();

            CreateStandardChartData(chart);

            ChartSeries series0 = chart.Series[0];

            // Count returns 0 since data labels are not displayed.
            Assert.That(series0.DataLabels.Count, Is.EqualTo(0));

            series0.HasDataLabels = true;
            Assert.That(series0.DataLabels.Count, Is.EqualTo(4));

            // Get data labels of the first and second point of the first series.
            ChartDataLabel chartDataLabel00 = series0.DataLabels[0];
            ChartDataLabel chartDataLabel01 = series0.DataLabels[1];

            // Set some properties.
            chartDataLabel00.ShowLegendKey = true;
            chartDataLabel00.ShowLeaderLines = true;
            chartDataLabel00.ShowCategoryName = true;
            chartDataLabel00.ShowPercentage = true;
            chartDataLabel00.ShowSeriesName = true;
            chartDataLabel00.ShowValue = true;
            chartDataLabel00.Separator = "/";

            chartDataLabel01.ShowValue = true;

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartDataLabels.docx");

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            series0 = shape.Chart.Series[0];

            Assert.That(series0.DataLabels.Count, Is.EqualTo(4));

            chartDataLabel00 = series0.DataLabels[0];
            Assert.That(chartDataLabel00.IsVisible, Is.True);
            Assert.That(chartDataLabel00.ShowLegendKey, Is.True);
            Assert.That(chartDataLabel00.ShowLeaderLines, Is.True);
            Assert.That(chartDataLabel00.ShowCategoryName, Is.True);
            Assert.That(chartDataLabel00.ShowPercentage, Is.True);
            Assert.That(chartDataLabel00.ShowSeriesName, Is.True);
            Assert.That(chartDataLabel00.ShowValue, Is.True);
            Assert.That(chartDataLabel00.Separator, Is.EqualTo("/"));

            chartDataLabel01 = series0.DataLabels[1];
            Assert.That(chartDataLabel01.ShowValue, Is.True);
        }

        [Test]
        public void TestChartDataLegend()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            chart.Title.Text = "Line Chart With Legend on/off";

            Assert.That(chart.Legend.Overlay, Is.False);
            Assert.That(chart.Legend.Position, Is.EqualTo(LegendPosition.Bottom));

            chart.Legend.Position = LegendPosition.Left;
            chart.Legend.Overlay = true;

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartDataLegend.docx", UnifiedScenario.Docx2DocxNoGold);

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            chart = shape.Chart;

            Assert.That(chart.Legend.Overlay, Is.True);
            Assert.That(chart.Legend.Position, Is.EqualTo(LegendPosition.Left));
        }

        [Test]
        public void TestChartTitleFont()
        {
            // Default Chart Title depends on the culture.
            string currentCulture = SystemPal.GetCurrentCultureName();
            SystemPal.SetCulture("en-US");

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

            Assert.That(shape.Chart.Title.Text, Is.EqualTo("Chart Title"));

            Chart chart = shape.Chart;
            Font cFont = chart.Title.Font;

            // Check defaults.
            Assert.That(cFont.Size, Is.EqualTo(14));
            Assert.That(cFont.Kerning, Is.EqualTo(12));
            Assert.That(cFont.Spacing, Is.EqualTo(0));
            Assert.That(cFont.Name, Is.EqualTo("Calibri"));
            Assert.That(cFont.NameAscii, Is.EqualTo("Calibri"));
            Assert.That(cFont.NameBi, Is.EqualTo("Times New Roman"));
            Assert.That(cFont.NameFarEast, Is.EqualTo("Times New Roman"));
            Assert.That(cFont.NameOther, Is.EqualTo("Times New Roman"));
            Assert.That(cFont.Bold, Is.False);
            Assert.That(cFont.Italic, Is.False);
            Assert.That(cFont.StrikeThrough, Is.False);
            Assert.That(cFont.DoubleStrikeThrough, Is.False);
            Assert.That(cFont.Color.ToArgb(), Is.EqualTo(unchecked((int)0xFF595959)));
            Assert.That(cFont.Underline, Is.EqualTo(Underline.None));

            // Setting chart Title.
            cFont.Size = 10;
            cFont.Kerning = 11;
            cFont.Spacing = 12;
            cFont.Name = "Arial";
            cFont.Bold = true;
            cFont.Italic = true;
            cFont.StrikeThrough = false;
            cFont.DoubleStrikeThrough = true;
            cFont.AllCaps = false;
            cFont.SmallCaps = true;
            cFont.Color = Color.Red;
            cFont.Underline = Underline.Double;
            chart.Title.Text = "Sample Column Chart Title";

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartTitleFont.docx");

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            chart = shape.Chart;
            cFont = chart.Title.Font;

            Assert.That(cFont.Size, Is.EqualTo(10));
            Assert.That(cFont.Kerning, Is.EqualTo(11));
            Assert.That(cFont.Spacing, Is.EqualTo(12));
            Assert.That(cFont.Name, Is.EqualTo("Arial"));
            Assert.That(cFont.NameAscii, Is.EqualTo("Arial"));
            Assert.That(cFont.NameBi, Is.EqualTo("Arial"));
            Assert.That(cFont.NameFarEast, Is.EqualTo("Arial"));
            Assert.That(cFont.NameOther, Is.EqualTo("Arial"));
            Assert.That(cFont.Bold, Is.True);
            Assert.That(cFont.Italic, Is.True);
            Assert.That(cFont.StrikeThrough, Is.False);
            Assert.That(cFont.DoubleStrikeThrough, Is.True);
            Assert.That(cFont.AllCaps, Is.False);
            Assert.That(cFont.SmallCaps, Is.True);
            Assert.That(cFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(cFont.Underline, Is.EqualTo(Underline.Double));

            SystemPal.SetCulture(currentCulture);

            chart.Title.Text = null;
            cFont.Size = 12;

            chart.Title.Text = "123";

            Assert.That(cFont.Size, Is.EqualTo(12));
            Assert.That(cFont.Kerning, Is.EqualTo(11));
            Assert.That(cFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
        }


        [Test]
        public void TestChartImport()
        {
            Document dstDoc = new Document();
            Document srcDoc = new Document();
            DocumentBuilder builder = new DocumentBuilder(srcDoc);

            // Add chart with default data.
            builder.InsertChart(ChartType.Column, 432, 252);

            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.UseDestinationStyles);

            foreach (Node node in srcDoc.FirstSection.Body.GetChildNodes(NodeType.Any, false))
            {
                dstDoc.LastSection.Body.AppendChild(importer.ImportNode(node, true));
            }

            Node[] shapes = dstDoc.GetChildNodes(NodeType.Shape, true).ToArray();
            Assert.That(shapes.Length, Is.EqualTo(1));
        }

        [Test]
        public void TestChartClone()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;
            chart.Title.Text = "TestName";
            DmlChartSpace space = (DmlChartSpace)shape.DmlNode;
            DmlChartFormat format = space.ChartFormat;
            DmlChartPlotArea area = format.PlotArea;
            IList<DmlChart> charts = area.Charts;
            DmlChart dmlChart = charts[0];

            Shape shapeClone = (Shape)shape.Clone(true);
            Chart chartClone = shapeClone.Chart;
            DmlChartSpace spaceClone = (DmlChartSpace)shapeClone.DmlNode;
            DmlChartFormat formatClone = spaceClone.ChartFormat;
            DmlChartPlotArea areaClone = formatClone.PlotArea;
            IList<DmlChart> chartsClone = areaClone.Charts;
            DmlChart dmlChartClone = chartsClone[0];

            Assert.That(spaceClone, IsNot.SameAs(space));
            Assert.That(formatClone, IsNot.SameAs(format));
            Assert.That(areaClone, IsNot.SameAs(area));
            Assert.That(chartsClone, IsNot.SameAs(charts));
            Assert.That(dmlChartClone, IsNot.SameAs(dmlChart));

            Assert.That(chartClone, IsNot.SameAs(chart));
            Assert.That(chartClone.Series, IsNot.SameAs(chart.Series));

            ChartSeries series = chart.Series[0];
            ChartSeries seriesClone = chartClone.Series[0];

            Assert.That(seriesClone, IsNot.SameAs(series));
            Assert.That(seriesClone.DefaultDataPoint, IsNot.SameAs(series.DefaultDataPoint));
            Assert.That(seriesClone.DataLabels, IsNot.SameAs(series.DataLabels));
            Assert.That(chartClone.Title, IsNot.SameAs(chart.Title));
            Assert.That(chartClone.Title.Font, IsNot.SameAs(chart.Title.Font));

            // Test DmlTextBody clone.
            Assert.That(chartClone.Title.Text, Is.EqualTo(chart.Title.Text));

            DmlParagraph paragraph = (DmlParagraph)(format.DCTitle.Tx.RichText.Paragraphs[0]);
            DmlParagraph paragraphCloned = (DmlParagraph)(formatClone.DCTitle.Tx.RichText.Paragraphs[0]);

            Assert.That(paragraphCloned, IsNot.SameAs(paragraph));
        }

        [Test]
        public void TestChartAppendDocument()
        {
            Document dstDoc = new Document();
            Document srcDoc = new Document();
            DocumentBuilder builder = new DocumentBuilder(srcDoc);

            // Add chart with default data.
            builder.InsertChart(ChartType.Column, 432, 252);

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            TestUtil.SaveOpen(dstDoc, @"DocBuilder\Chart\TestChartAppendDocument.docx");
        }

        [Test]
        public void TestChartImportFormatMode()
        {
            Document dstDoc = TestUtil.Open(@"DocBuilder\Chart\ChartMsWord2013.docx");
            Document srcDoc = TestUtil.Open(@"DocBuilder\Chart\ChartMsWord2007.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);
            Shape shape = (Shape)dstDoc.GetChild(NodeType.Shape, 0, true);

            // ThemeOverride is null, destination theme will be used.
            Assert.That(((DmlChartSpace)shape.DmlNode).ThemeOverride, Is.Null);

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);
            shape = (Shape)dstDoc.GetChild(NodeType.Shape, 1, true);
            Assert.That(((DmlChartSpace)shape.DmlNode).ThemeOverride, IsNot.Null());
        }

        [Test]
        public void TestChartWithEmptySeriesName()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            ChartSeriesCollection seriesColl = chart.Series;

            // Get first chart series.
            ChartSeries firstChartSeries = seriesColl[0];

            // Set name as string.Empty.
            firstChartSeries.Name = string.Empty;

            // In MS Word there is no way to set empty series name, that is why generate name as Series plus one based index.
            Assert.That(firstChartSeries.Name, Is.EqualTo("Series1"));
        }

        /// <summary>
        /// Adding single series vs adding multiple series.
        /// </summary>
        [Test]
        public void TestChartAddingSingleSeries()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            chart.Series.Clear();
            Assert.That(chart.Series.Count, Is.EqualTo(0));

            // Create category names array.
            string[] categories = new string[] { "AW Category 1", "AW Category 2" };

            // Add just one single series.
            chart.Series.Add("Single Series Name", categories, new double[] { 1, 2 });
            Assert.That(chart.Series.Count, Is.EqualTo(1));

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartAddingSingleSeries.docx");
            Shape s1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            chart = s1.Chart;

            // Check Series count.
            Assert.That(chart.Series.Count, Is.EqualTo(1));

            chart.Series.Add("One More Series Name", categories, new double[] { 3, 4 });
            Assert.That(chart.Series.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Adding only default data markers vs. custom data markers.
        /// </summary>
        [Test]
        public void TestChartDataMarkers()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;

            // Get first chart series.
            ChartSeries firstChartSeries = chart.Series[0];

            // Get ChartDataPoint collection for the first series.
            ChartDataPointCollection dmlChartDataPointCollection = firstChartSeries.DataPoints;
            Assert.That(dmlChartDataPointCollection.MaterializedDataPoints.GetEnumerator().MoveNext(), Is.False);

            // Check default ChartMarker symbol.
            Assert.That(firstChartSeries.Marker.Symbol, Is.EqualTo(MarkerSymbol.None));

            // Change default data marker for this series.
            firstChartSeries.Marker.Size = 12;
            firstChartSeries.Marker.Symbol = MarkerSymbol.Star;

            // Gets DataPoint of the first and second point of the first series.
            ChartDataPoint dataPoint00 = firstChartSeries.DataPoints[0];
            ChartDataPoint dataPoint01 = firstChartSeries.DataPoints[1];

            // Add custom data marker for the first data point.
            dataPoint00.Marker.Symbol = MarkerSymbol.Circle;
            dataPoint00.Marker.Size = 15;

            // Add custom data marker for the second data point.
            dataPoint01.Marker.Symbol = MarkerSymbol.Diamond;
            dataPoint01.Marker.Size = 20;

            TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartDataMarkers.docx");
        }

        /// <summary>
        /// Adding data labels.
        /// </summary>
        [Test]
        public void TestAddChartDataLabels()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            chart.Series.Clear();

            // Create category names array.
            string[] categories = new string[] { "AW Category 1", "AW Category 2", "AW Category 3" };

            // Add just one single series.
            ChartSeries series0 = chart.Series.Add("AW Series 1", categories, new double[] { 1, 7, 3 });

            // Count returns 0 since data labels are not displayed.
            Assert.That(series0.DataLabels.Count, Is.EqualTo(0));

            series0.HasDataLabels = true;
            Assert.That(series0.DataLabels.Count, Is.EqualTo(3));

            ChartDataLabel chartDataLabel00 = series0.DataLabels[0];
            ChartDataLabel chartDataLabel01 = series0.DataLabels[1];
            ChartDataLabel chartDataLabel02 = series0.DataLabels[2];

            // Set some properties.
            chartDataLabel00.ShowLegendKey = true;
            chartDataLabel00.ShowValue = true;

            // Set some properties.
            chartDataLabel01.ShowValue = true;

            // Set some properties.
            chartDataLabel02.ShowCategoryName = true;
            chartDataLabel02.ShowPercentage = true;
            chartDataLabel02.ShowValue = true;
            chartDataLabel02.Separator = "/";

            TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestAddChartDataLabels.docx");
        }

        /// <summary>
        /// Customizing title.
        /// </summary>
        [Test]
        public void TestChartCustomizingTitle()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            Assert.That(chart.Title.Show, Is.True);

            // Setting chart Title.
            chart.Title.Text = "Sample Column Chart Title";
            chart.Title.Overlay = true;

            // Add chart with default data.
            Shape shape1 = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart1 = shape1.Chart;

            Assert.That(chart1.Title.Show, Is.True);

            // Setting chart Title.
            chart1.Title.Text = "Sample Column Chart Title";
            chart1.Title.Overlay = false;

            TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartCustomizingTitle.docx");
        }

        [Test]
        public void TestChartSeriesIndex()
        {
            string currentCulture = SystemPal.GetCurrentCultureName();
            SystemPal.SetCulture("en-US");
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            Assert.That(shape.Chart.Title.Text, Is.EqualTo("Chart Title"));

            int i = 0;
            foreach (ChartSeries series in chart.Series)
                Assert.That(series.Index, Is.EqualTo(i++));

            i = 0;
            foreach (ChartSeries series in chart.Series)
                Assert.That(series.Order, Is.EqualTo(i++));

            SystemPal.SetCulture(currentCulture);
        }

        [Test]
        public void TestChartSeriesName()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            chart.Series[0].Name = "TestName";

            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestChartSeriesName.docx");
            Shape s1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            chart = s1.Chart;

            // Check Series name.
            Assert.That(chart.Series[0].Name, Is.EqualTo("TestName"));
            Assert.That(chart.Series[1].Name, Is.EqualTo("Series 2"));
        }

        /// <summary>
        /// WORDSNET-13428 Add feature to insert empty values in chart series.
        /// </summary>
        [Test]
        public void TestEmptyCategorAndDataValues()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;

            ChartSeriesCollection seriesColl = chart.Series;
            seriesColl.Clear();

            // Create category names array.
            string[] categories = new string[] { "Cat1", null, "Cat3", "Cat4", "Cat5" };

            // Adding new series
            seriesColl.Add("AW Series 1", categories, new double[] { 1, 2, double.NaN, 4, 5 });
            seriesColl.Add("AW Series 2", categories, new double[] { 2, 3, double.NaN, 5, 6 });
            seriesColl.Add("AW Series 3", categories, new double[] { double.NaN, 4, 5, double.NaN, double.NaN });

            TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestEmptyCategorAndDataValues.docx");
        }


        /// <summary>
        /// WORDSNET-27204 InvalidOperationException is raised when saving document optimized for Word 2013 with added
        /// Word 2016 charts
        /// A VML fallback shape was generated for a Word 2016 chart shape that <see cref="DocxDocumentWriterBase"/>
        /// could not process because a DML fallback is expected.
        /// </summary>
        [Test]
        public void Test27204()
        {
            Document doc = new Document();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2013);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertChart(ChartType.Waterfall, 432, 252);
            builder.InsertChart(ChartType.Funnel, 432, 252);

            // Check that no exception occurs.
            doc = TestUtil.SaveOpen(doc, @"DocBuilder\Chart\Test27204.docx", null, false);

            Shape shape = doc.FirstSection.Body.Shapes[0];

            // In the FOSS version there are no fallback shapes because there is no rendering code to create them.
        }

        /// <summary>
        /// WORDSNET-15521 Adding the data labels format in a chart.
        /// </summary>
        [Test]
        public void TestDataLabelsDifferentNumberFormat()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Add chart with default data.
            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            chart.Title.Text = "Data Labels With Different Number Format";

            // Delete default generated series.
            chart.Series.Clear();

            // Add new series
            ChartSeries series0 = chart.Series.Add(
                "AW Series 1",
                new string[] { "AW1", "AW2", "AW3", "AW4" ,"AW5", "AW6", "AW7", "AW8" , "AW9", "AW10", "AW11", "AW12", "AW13"},
                new double[] {2.5, 1.5, 3.5, 2.5, 4.5, 3.5, 5.5, 4.5, 6.5, 5.5, 7.5, 6.5, 8.5});

            series0.HasDataLabels = true;
            ChartDataLabel chartDataLabel = series0.DataLabels[0];

            // Check default values.
            Assert.That(chartDataLabel.NumberFormat, IsNot.Null());
            Assert.That(chartDataLabel.NumberFormat.IsLinkedToSource, Is.True);
            Assert.That(string.Empty, Is.EqualTo(chartDataLabel.NumberFormat.FormatCode));

            SetChartDataLabel(series0, 0, NumberFormatCode);
            SetChartDataLabel(series0, 1, CurrencyFormatCode);
            SetChartDataLabel(series0, 2, TimeFormatCode);
            SetChartDataLabel(series0, 3, DateFormatCode);
            SetChartDataLabel(series0, 4, PercentageFormatCode);
            SetChartDataLabel(series0, 5, FractionFormatCode);
            SetChartDataLabel(series0, 6, ScientificFormatCode);
            SetChartDataLabel(series0, 7, TextFormatCode);
            SetChartDataLabel(series0, 8, AccountingFormatCode);
            SetChartDataLabel(series0, 9, CustomFormatCode);
            SetChartDataLabel(series0, 10, ""); // Set empty string
            SetChartDataLabel(series0, 11, null); // Set null

            chartDataLabel = series0.DataLabels[12];

            chartDataLabel.ShowValue = true;
            chartDataLabel.NumberFormat.IsLinkedToSource = false;

            TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestDataLabelsDifferentNumberFormat.docx");

            series0 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            Assert.That(series0.DataLabels.Count, Is.EqualTo(13));

            CheckNumberFormat(series0, 0, NumberFormatCode);
            CheckNumberFormat(series0, 1, CurrencyFormatCode);
            CheckNumberFormat(series0, 2, TimeFormatCode);
            CheckNumberFormat(series0, 3, DateFormatCode);
            CheckNumberFormat(series0, 4, PercentageFormatCode);
            CheckNumberFormat(series0, 5, FractionFormatCode);
            CheckNumberFormat(series0, 6, ScientificFormatCode);
            CheckNumberFormat(series0, 7, TextFormatCode);
            CheckNumberFormat(series0, 8, AccountingFormatCode);
            CheckNumberFormat(series0, 9, CustomFormatCode);
            CheckNumberFormat(series0, 10, GeneralFormatCode);
            CheckNumberFormat(series0, 11, GeneralFormatCode);
            CheckNumberFormat(series0, 12, GeneralFormatCode);
        }

        /// <summary>
        /// WORDSNET-27030 Cannot add Word 2016 chart to Aspose.Words-generated document in MS Word even if the document
        /// has charts of the same type inserted using <see cref="DocumentBuilder"/>
        /// When using the InsertChart methods to add a Word 2016 chart, the document is now automatically optimized for
        /// Word 2013, which resolves the issue.
        /// </summary>
        [Test]
        public void Test27030()
        {
            Document doc = new Document();
            TestWarningCallback warningCallback = new TestWarningCallback();
            doc.WarningCallback = warningCallback;

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertChart(ChartType.Treemap, 432, 252);

            Assert.That(doc.CompatibilityOptions.MswVersion, Is.EqualTo(MsWordVersionCore.Word2013));
            Assert.That(warningCallback.Contains(WarningSource.Unknown, WarningType.MinorFormattingLoss,
                WarningStrings.OptimizedForWord2013), Is.True);
        }

        private static void SetChartDataLabel(ChartSeries series, int lableIndex, string numberFormat)
        {
            ChartDataLabel chartDataLabel = series.DataLabels[lableIndex];
            chartDataLabel.ShowValue = true;

            // Set new format code.
            chartDataLabel.NumberFormat.FormatCode = numberFormat;
        }

        private void CheckNumberFormat(ChartSeries series, int lableIndex, string numberFormat)
        {
            ChartDataLabel dataLable = series.DataLabels[lableIndex];

            Assert.That(numberFormat, Is.EqualTo(dataLable.NumberFormat.FormatCode));
            Assert.That(dataLable.NumberFormat.IsLinkedToSource, Is.False);
        }

        private const string NumberFormatCode = "#,##0.00";
        private const string CurrencyFormatCode = "\"$\"#,##0.00";
        private const string TimeFormatCode = "[$-x-systime]h:mm:ss AM/PM";
        private const string DateFormatCode = "d/mm/yyyy";
        private const string PercentageFormatCode = "0.00%";
        private const string FractionFormatCode = "# ?/?";
        private const string ScientificFormatCode = "0.00E+00";
        private const string TextFormatCode = "@";
        private const string AccountingFormatCode = "_-\"$\"* #,##0.00_-;-\"$\"* #,##0.00_-;_-\"$\"* \"-\"??_-;_-@_-";
        private const string CustomFormatCode = "[Red]-#,##0.0";
        private const string GeneralFormatCode = "General";

        /// <summary>
        /// WORDSNET-15521 Adding the data labels format in a chart.
        /// </summary>
        [Test]
        public void TestJira15521()
        {
            Document doc = TestUtil.Open(@"DocBuilder\Chart\TestJira15521.docx");

            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            Chart chart = ((Shape)shapes[0]).Chart;

            foreach (ChartDataLabel dataLabel in chart.Series[0].DataLabels)
            {
                Assert.That(dataLabel.NumberFormat.IsLinkedToSource, Is.False);

                dataLabel.NumberFormat.IsLinkedToSource = true;
                Assert.That(dataLabel.NumberFormat.IsLinkedToSource, Is.True);
                Assert.That(dataLabel.NumberFormat.FormatCode, Is.EqualTo(string.Empty));
            }

            TestUtil.SaveOpen(doc, @"DocBuilder\Chart\TestJira15521.docx");
        }

        /// <summary>
        /// WORDSNET-15923 DocumentBuilder.InsertChart throws System.NullReferenceException
        /// When create chart with negative or zero dimensions, scale default size to parents paragraph/cell size
        /// </summary>
        [Test]
        public void TestJira15923()
        {
            // Load document with DML Textbox and insert into it new chart with negative dimensions,
            // need to load from file because there is no way to crete it dynamically
            Document doc = TestUtil.Open(@"DocBuilder\Chart\TestJira15923.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(1, 0);
            Shape shape = builder.InsertChart(ChartType.Area3D, -2, -3);
            Shape parentShape = (Shape)shape.GetAncestor(NodeType.Shape);
            Assert.That(shape.Width, Is.GreaterThan(0));
            Assert.That(shape.Height, Is.GreaterThan(0));
            // One dimension must match parent shape, others scales to it
            Assert.That((shape.Width == parentShape.Width) || (shape.Height == parentShape.Height), Is.True);

            // Check chart creation with zero/negative size
            builder.InsertSection(SectionStart.NewPage);
            shape = builder.InsertChart(ChartType.Column, 0, 0);
            Assert.That(shape.Width, Is.GreaterThan(0));
            Assert.That(shape.Height, Is.GreaterThan(0));
            Shape newShape = builder.InsertChart(ChartType.Column, -1, -1);
            Assert.That(newShape.Width, Is.EqualTo(shape.Width));
            Assert.That(newShape.Height, Is.EqualTo(shape.Height));

            // Check creation with zero/negative dimensions in table
            Cell cell = builder.InsertCell();
            // Set cell width to 100 points
            cell.CellFormat.PreferredWidth = PreferredWidth.FromPoints(100);
            shape = builder.InsertChart(ChartType.Column, 0, 0);
            Assert.That(shape.Width, Is.GreaterThan(0));
            Assert.That(shape.Height, Is.GreaterThan(0));

            // Set second cell width to 2*100 points
            cell = builder.InsertCell();
            cell.CellFormat.PreferredWidth = PreferredWidth.FromPoints(200);
            newShape = builder.InsertChart(ChartType.Column, -100, -1);
            // Check that second shape is 2 times (+-10 percents) larger than first
            Assert.That((newShape.Width * 0.9) < (shape.Width * 2) && (shape.Width * 2) < newShape.Width * 1.1, Is.True);
            Assert.That((newShape.Height * 0.9) < (shape.Height * 2) && (shape.Height * 2) < newShape.Height * 1.1, Is.True);
            builder.EndRow();
            builder.EndTable();
        }

        /// <summary>
        /// Tests that when a parameter of the <see cref="ChartStyle"/> type is provided when inserting a chart,
        /// the style is applied to all chart elements.
        /// </summary>
        [TestCase(ChartStyle.Muted)]
        [TestCase(ChartStyle.Saturated)]
        [TestCase(ChartStyle.Shaded)]
        [TestCase(ChartStyle.Flat)]
        [TestCase(ChartStyle.Shadowed)]
        [TestCase(ChartStyle.Gradient)]
        [TestCase(ChartStyle.Original)]
        [TestCase(ChartStyle.Transparent1)]
        [TestCase(ChartStyle.Transparent2)]
        [TestCase(ChartStyle.Outline)]
        [TestCase(ChartStyle.OutlineBlack)]
        [TestCase(ChartStyle.Black)]
        [TestCase(ChartStyle.Grey)]
        [TestCase(ChartStyle.Blue)]
        [TestCase(ChartStyle.ShadedPlot)]
        public void TestChartStyle(ChartStyle chartStyle)
        {
            // DocWithActualTheme.docx is used to be able to compare output chart with a chart created in MS Word.
            Document doc = TestUtil.Open(@"DocBuilder\Chart\DocWithActualTheme.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252, chartStyle);
                Assert.That(shape.Chart.Style, Is.EqualTo(chartStyle));
            }

            // The same file names are also used for the TestSettingStyle tests.
            string fileName = string.Format(@"DocBuilder\Chart\Test{0}ChartStyle.docx", chartStyle.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests that a style specified for a chart is applied to chart elements that are added/displayed on the chart.
        /// Only styles for which it is easy to identify that the style is being applied incorrectly or that there are
        /// possible issues with the style (for example, color of data labels for some label positions blends with
        /// the background) are included in the test.
        /// </summary>
        [TestCase(ChartStyle.Saturated)]
        [TestCase(ChartStyle.Original)]
        [TestCase(ChartStyle.Black)]
        [TestCase(ChartStyle.Grey)]
        [TestCase(ChartStyle.Blue)]
        public void TestApplyingStyleToAddedChartElements(ChartStyle chartStyle)
        {
            // DocWithActualTheme.docx is used to be able to compare output chart with a chart created in MS Word.
            Document doc = TestUtil.Open(@"DocBuilder\Chart\DocWithActualTheme.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            List<ChartType> chartTypes = new List<ChartType>();
            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                builder.InsertChart(chartType, 432, 252, chartStyle);
                chartTypes.Add(chartType);
            }

            // Insert a combo chart.
            Shape comboChartShape = builder.InsertChart(ChartType.Column, 432, 252, chartStyle);
            Chart comboChart = comboChartShape.Chart;
            ChartSeriesGroup group = comboChart.SeriesGroups.Add(ChartSeriesType.Scatter);
            group.Series.Add("Scatter", new double[] { 1, 2, 3 }, new double[] { 5, 10, 3 });
            chartTypes.Add(ChartType.Column);

            // Hide chart elements to clear their properties after resaving.
            foreach (Shape shape in doc.FirstSection.Body.Shapes)
            {
                Chart chart = shape.Chart;
                chart.Title.Show = false;
                chart.Legend.Position = LegendPosition.None;
                foreach (ChartAxis axis in chart.Axes)
                    axis.Hidden = true;
            }

            string fileName = string.Format(@"DocBuilder\Chart\TestApplying{0}StyleToAddedChartElements.docx", chartStyle.ToString());
            doc = TestUtil.SaveOpen(doc, fileName, null, false);

            for (int i = 0; i < chartTypes.Count; i++)
            {
                Shape shape = doc.FirstSection.Body.Shapes[i];
                Chart chart = shape.Chart;

                chart.Title.Show = true;
                chart.Title.Text = "Chart Title";
                chart.Legend.Position = LegendPosition.Bottom;
                if (chart.DataTable.IsDataTableSupported())
                    chart.DataTable.Show = true;

                foreach (ChartAxis axis in chart.Axes)
                {
                    axis.Hidden = false;
                    axis.Title.Show = true;
                    axis.Title.Text = axis.Type.ToString();
                }

                chart.Series.Clear();
                HandleChartTypeSpecifics(chartTypes[i], chart);

                if (i == chartTypes.Count - 1)
                {
                    // Combo chart.
                    chart.SeriesGroups[1].Series.Add("Scatter", new double[] { 1, 2, 3 }, new double[] { 5, 10, 3 });
                    chart.SeriesGroups.Add(ChartSeriesType.Line);
                    chart.SeriesGroups[2].Series.Add(
                        "Line",
                        new string[] { "AW Category 1", "AW Category 2", "AW Category 3", "AW Category 4" },
                        new double[] { 1, 3, 3.5, 2 });
                }

                foreach (ChartSeries series in chart.Series)
                {
                    series.HasDataLabels = true;
                    series.DataLabels.ShowValue = false;
                    series.DataLabels.ShowCategoryName = false;
                    series.DataLabels.ShowSeriesName = false;
                    series.DataLabels.ShowLegendKey = false;
                    if ((series.SeriesType == ChartSeriesType.Treemap) || (series.SeriesType == ChartSeriesType.Sunburst))
                        series.DataLabels.ShowCategoryName = true;
                    else
                        series.DataLabels.ShowValue = true;
                }
            }

            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests that setting the property <see cref="Chart.Style"/> applies the style to all chart elements.
        /// </summary>
        [TestCase(ChartStyle.Muted)]
        [TestCase(ChartStyle.Saturated)]
        [TestCase(ChartStyle.Shaded)]
        [TestCase(ChartStyle.Flat)]
        [TestCase(ChartStyle.Shadowed)]
        [TestCase(ChartStyle.Gradient)]
        [TestCase(ChartStyle.Original)]
        [TestCase(ChartStyle.Transparent1)]
        [TestCase(ChartStyle.Transparent2)]
        [TestCase(ChartStyle.Outline)]
        [TestCase(ChartStyle.OutlineBlack)]
        [TestCase(ChartStyle.Black)]
        [TestCase(ChartStyle.Grey)]
        [TestCase(ChartStyle.Blue)]
        [TestCase(ChartStyle.ShadedPlot)]
        public void TestSettingStyle(ChartStyle chartStyle)
        {
            // DocWithActualTheme.docx is used to be able to compare output chart with a chart created in MS Word.
            Document doc = TestUtil.Open(@"DocBuilder\Chart\DocWithActualTheme.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (ChartType chartType in Enum.GetValues(typeof(ChartType)))
            {
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;
                chart.Style = chartStyle;
            }

            // The same file names are used for the TestChartStyle tests.
            string fileName = string.Format(@"DocBuilder\Chart\Test{0}ChartStyle.docx", chartStyle.ToString());
            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Tests that the <see cref="ChartStyleResolver.GetChartStyle"/> method returns the <see cref="ChartStyle.Normal"/>
        /// value for all unknown style IDs.
        /// </summary>
        [Test]
        public void TestChartStyleOfUnknownStyleId()
        {
            Assert.That(ChartStyleResolver.GetChartStyle(int.MaxValue), Is.EqualTo(ChartStyle.Normal));
        }

        private static void HandleChartTypeSpecifics(ChartType chartType, Chart chart)
        {
            switch (chartType)
            {
                case ChartType.Area:
                case ChartType.AreaStacked:
                case ChartType.AreaPercentStacked:
                case ChartType.Area3D:
                case ChartType.Area3DStacked:
                case ChartType.Area3DPercentStacked:
                case ChartType.Radar:
                    CreateAreaChartData(chart);
                    break;
                case ChartType.Bar:
                case ChartType.BarStacked:
                case ChartType.BarPercentStacked:
                case ChartType.Bar3D:
                case ChartType.Bar3DStacked:
                case ChartType.Bar3DPercentStacked:
                case ChartType.Column:
                case ChartType.ColumnStacked:
                case ChartType.ColumnPercentStacked:
                case ChartType.Column3D:
                case ChartType.Column3DClustered:
                case ChartType.Column3DStacked:
                case ChartType.Column3DPercentStacked:
                case ChartType.Line:
                case ChartType.LineStacked:
                case ChartType.LinePercentStacked:
                case ChartType.Line3D:
                case ChartType.Surface:
                case ChartType.Surface3D:
                    CreateStandardChartData(chart);
                    break;
                case ChartType.Bubble:
                case ChartType.Bubble3D:
                    CreateBubbleChartData(chart);
                    break;
                case ChartType.Pie:
                case ChartType.PieOfBar:
                case ChartType.PieOfPie:
                case ChartType.Pie3D:
                case ChartType.Doughnut:
                    CreatePieChartData(chart);
                    break;
                case ChartType.Scatter:
                    CreateScatterChartData(chart);
                    break;
                case ChartType.Stock:
                    CreateStockChartData(chart);
                    break;
                case ChartType.Treemap:
                case ChartType.Sunburst:
                    CreateMultilevelChartData(chart);
                    break;
                case ChartType.Histogram:
                    CreateHistogramChartData(chart);
                    break;
                case ChartType.Pareto:
                    CreateParetoChartData(chart);
                    break;
                case ChartType.BoxAndWhisker:
                    CreateBoxAndWhiskerChartData(chart);
                    break;
                case ChartType.Waterfall:
                    CreateWaterfallChartData(chart);
                    break;
                case ChartType.Funnel:
                    CreateFunnelChartData(chart);
                    break;
                default:
                    break;
            }
        }

        private static void CreateStandardChartData(Chart chart)
        {
            // Create category names array
            string[] categories = new string[] { "AW Category 1", "AW Category 2", "AW Category 3", "AW Category 4" };

            // Adding new series
            chart.Series.Add("AW Series 1", categories, new double[] { 4.3, 2.5, 3.5, 4.5 });
            chart.Series.Add("AW Series 2", categories, new double[] { 2.4, 4.4, 1.8, 2.8 });
            chart.Series.Add("AW Series 3", categories, new double[] { 2, 2, 3, 5 });
        }

        private static void CreateStockChartData(Chart chart)
        {
            // Adding new series, java.util.Date for Java to check public api change.
#if JAVA
            chart.Series.add("AW High", gDateTimes, new double[] { 55, 57, 57, 58, 58 });
            chart.Series.add("AW Low", gDateTimes, new double[] { 11, 12, 13, 11, 35 });
            ChartSeries closeSeries = chart.Series.add("AW Close", gDateTimes, new double[] { 32, 35, 34, 35, 43 });
#else
            chart.Series.Add("AW High", gDateTimes, new double[] { 55, 57, 57, 58, 58 });
            chart.Series.Add("AW Low", gDateTimes, new double[] { 11, 12, 13, 11, 35 });
            ChartSeries closeSeries = chart.Series.Add("AW Close", gDateTimes, new double[] { 32, 35, 34, 35, 43 });
#endif
            // Set the marker symbol to MarkerSymbol.Default to show the marker and apply its style. MarkerSymbol.Default
            // is the default value of the property, thus, if it is not changed during applying style, remove the
            // corresponding attribute to not have troubles with export/import gold.
            closeSeries.Marker.Symbol = MarkerSymbol.Default;
            if (closeSeries.Marker.Symbol == MarkerSymbol.Default)
                closeSeries.Marker.MarkerPr.RemoveProperty(DmlChartMarkerAttr.Symbol);
        }

        private static void CreateScatterChartData(Chart chart)
        {
            // Adding new series
            chart.Series.Add("AW Series 1", new double[] { 0.7, 1.8, 2.6 }, new double[] { 2.7, 3.2, 0.8 });
        }

        private static void CreatePieChartData(Chart chart)
        {
            // Create category names array
            string[] categoriesDoughnut = new string[] { "AW 1st Qtr", "AW 2nd Qtr", "AW 3rd Qtr", "AW 4th Qtr" };

            // Adding new series
            chart.Series.Add("AW Sales", categoriesDoughnut, new double[] { 8.2, 3.2, 1.4, 1.2 });
        }

        private static void CreateBubbleChartData(Chart chart)
        {
            // Adding new series
            ChartSeries series = chart.Series.Add(
                "AW Series 1",
                new double[] { 0.7, 1.8, 2.6 },
                new double[] { 2.7, 3.2, 0.8 },
                new double[] { 10, 4, 8 });

            series.Bubble3D = ((DmlBubbleChart)chart.ChartSpace.FirstChart).Bubble3D;
        }

        private static void CreateAreaChartData(Chart chart)
        {
            // Adding new series, java.util.Date for Java to check public api change.
#if JAVA
            chart.Series.add("AW Series 1", gDateTimes, new double[] { 32, 32, 28, 12, 15 });
            chart.Series.add("AW Series 2", gDateTimes, new double[] { 12, 12, 12, 21, 28 });
#else
            chart.Series.Add("AW Series 1", gDateTimes, new double[] { 32, 32, 28, 12, 15 });
            chart.Series.Add("AW Series 2", gDateTimes, new double[] { 12, 12, 12, 21, 28 });
#endif
        }

        /// <summary>
        /// Creates chart data for the <see cref="ChartType.Treemap"/> and <see cref="ChartType.Sunburst"/> chart types
        /// that have multi-level categories.
        /// </summary>
        private static void CreateMultilevelChartData(Chart chart)
        {
            ChartMultilevelValue[] xValues = new ChartMultilevelValue[]
            {
                new ChartMultilevelValue("Asia", "Japan", "Tokyo"),
                new ChartMultilevelValue("Asia", "Japan", "Osaka"),
                new ChartMultilevelValue("Asia", "India", "Delhi"),
                new ChartMultilevelValue("Asia", "India", "Mumbai"),
                new ChartMultilevelValue("Asia", "China", "Shanghai"),
                new ChartMultilevelValue("Asia", "China", "Beijing"),
                new ChartMultilevelValue("Asia", "Bangladesh", "Dhaka"),
                new ChartMultilevelValue("Asia", "Pakistan", "Karachi"),
                new ChartMultilevelValue("America", "Brazil", "São Paulo"),
                new ChartMultilevelValue("America", "Mexico", "Mexico City"),
                new ChartMultilevelValue("America", "United States", "New York-Newark"),
                new ChartMultilevelValue("Africa", "Egypt", "Cairo")
            };

            double[] yValues = new double[]
            {
                37468000, 19281000, 28514000, 19980000, 25582000, 19618000,
                19578000, 15400000, 21650000, 21581000, 18819000, 20076000
            };

            ChartSeries series = chart.Series.Add("Largest Cities", xValues, yValues);
            series.HasDataLabels = true;
            series.DataLabels.ShowCategoryName = true;
            series.DataLabels.ShowValue = true;
        }

        /// <summary>
        /// Creates data for a Histogram chart.
        /// </summary>
        private static void CreateHistogramChartData(Chart chart)
        {
            chart.Series.Add(
                "AW Series 1",
                new double[]
                {
                    -5, -4.5, -4, -3.9, -3.3, -3.3, -3, -2.9, -2.8, -1.9, -1.8, -1, -0.8, -0.7, 0, 0.5, 1.1, 1.3,
                    1.9, 2.6
                });
            chart.Series.Add(
                "AW Series 2",
                new double[]
                {
                    -6, -5, -4, -4, -3, -3, -3, -2, -2, -1, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 4
                });

            chart.Legend.Position = LegendPosition.Bottom;
        }

        /// <summary>
        /// Creates data for a Pareto chart.
        /// </summary>
        private static void CreateParetoChartData(Chart chart)
        {
            chart.Series.Add(
                "Sales",
                new string[]
                {
                    "AW", "GD", "AW", "AW", "App", "AW", "GD", "App", "AW", "AW", "App", "AW", "GD", "AW", "AW"
                },
                new double[]
                {
                    5, 1, 1, 2, 3, 3, 2, 2, 1, 10, 1, 3, 1, 1, 4
                });
        }

        /// <summary>
        /// Creates data for a Box and Whisker chart.
        /// </summary>
        private static void CreateBoxAndWhiskerChartData(Chart chart)
        {
            chart.Series.Add(
                "Points",
                new string[]
                {
                    "WC", "WC", "WC", "WC", "WC", "WC",
                    "NR", "NR", "NR", "NR", "NR", "NR",
                    "NA", "NA", "NA", "NA", "NA", "NA"
                },
                new double[]
                {
                    91, 80, 100, 77, 90, 104,
                    114, 107, 110, 60, 79, 78,
                    94, 93, 84, 71, 80, 103
                });
        }

        /// <summary>
        /// Creates data for a Waterfall chart.
        /// </summary>
        private static void CreateWaterfallChartData(Chart chart)
        {
            ChartSeries series = chart.Series.Add(
                "New Zealand GDP",
                new string[] { "2018", "2019 growth", "2020 growth", "2020", "2021 growth", "2022 growth", "2022" },
                new double[] { 100, 0.57, -0.25, 100.32, 20.22, -2.92, 117.62 },
                new bool[] { true, false, false, true, false, false, true });

            series.HasDataLabels = true;
        }

        /// <summary>
        /// Creates data for a Funnel chart.
        /// </summary>
        private static void CreateFunnelChartData(Chart chart)
        {
            CreatePieChartData(chart);
            chart.Series[0].HasDataLabels = true;
        }

        // Create DateTimes array, java.util.Date for Java to check public api change.
#if PLAIN_JAVA
        private static java.util.Date[] gDateTimes = new java.util.Date[]
                        {
                            new java.util.GregorianCalendar(2002, 5-1, 1).getTime(),
                            new java.util.GregorianCalendar(2002, 6-1, 1).getTime(),
                            new java.util.GregorianCalendar(2002, 7-1, 1).getTime(),
                            new java.util.GregorianCalendar(2002, 8-1, 1).getTime(),
                            new java.util.GregorianCalendar(2002, 9-1, 1).getTime(),
                        };
#else
        private static readonly DateTime[] gDateTimes = new DateTime[]
                        {
                            new DateTime(2002, 05, 01),
                            new DateTime(2002, 06, 01),
                            new DateTime(2002, 07, 01),
                            new DateTime(2002, 08, 01),
                            new DateTime(2002, 09, 01)
                        };
#endif
    }
}
