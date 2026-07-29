// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2021 by Alexander Zhiltsov

using System;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests properties and methods of the <see cref="ChartFormat"/> class.
    /// </summary>
    [TestFixture]
    public class TestChartFormat
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
        public void TestNewSeriesFillFormat()
        {
            Document doc = CreateDocumentWithColumnChart();
            ChartSeriesCollection series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            Fill fill1 = series[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.Solid();
            fill1.ForeColor = Color.Pink;
            fill1.Transparency = 0.5;
            CheckFill(fill1, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb), Color.Empty, true, 0.5, false, false);

            Fill fill2 = series[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.ForeColor = Color.Gray;
            CheckFill(fill2, FillType.Solid, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true, 0, false, false);

            fill2.Visible = false;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            // Check that a fill becomes visible on changing color.
            fill2.ForeColor = Color.Yellow;
            fill2.Transparency = 0.2;
            CheckFill(fill2, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true, 0.2, false, false);

            Fill fill3 = series[2].Format.Fill;
            fill3.Visible = false;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            Fill fill4 = series[3].Format.Fill;
            fill4.PresetTextured(PresetTexture.BrownMarble);
            CheckTexturedFill(fill4, PresetTexture.BrownMarble, Color.Empty, Color.Empty, true, 0);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewSeriesFillFormat.docx");
        }


        [Test]
        public void TestNewDataPointFillFormat()
        {
            Document doc = CreateDocumentWithColumnChart();
            ChartSeriesCollection series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            Fill fill1_1 = series[0].DataPoints[0].Format.Fill;
            CheckFill(fill1_1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1_1.Solid();
            fill1_1.ForeColor = Color.Pink;
            fill1_1.Transparency = 0.5;
            CheckFill(fill1_1, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb),
                Color.Empty, true, 0.5, false, false);

            Fill fill1_2 = series[0].DataPoints[1].Format.Fill;
            CheckFill(fill1_2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1_2.ForeColor = Color.Gray;
            CheckFill(fill1_2, FillType.Solid, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true, 0, false, false);

            fill1_2.Visible = false;
            CheckFill(fill1_2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            // Check that Visible == false can be replaced with solid fill.
            fill1_2.ForeColor = Color.Yellow;
            fill1_2.Transparency = 0.2;
            CheckFill(fill1_2, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true, 0.2, false, false);

            series[1].DataPoints[0].Format.Fill.Visible = false;

            Fill fill3_1 = series[2].DataPoints[0].Format.Fill;
            CheckFill(fill3_1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);
            fill3_1.ForeColor = Color.Black;

            Fill fill3_2 = series[2].DataPoints[1].Format.Fill;
            CheckFill(fill3_2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            Fill seriesFill4 = series[3].Format.Fill;
            CheckFill(seriesFill4, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            Fill fill4_1 = series[3].DataPoints[0].Format.Fill;
            CheckFill(fill4_1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);
            fill4_1.ForeColor = Color.Green;

            Fill fill4_2 = series[3].DataPoints[1].Format.Fill;
            CheckFill(fill4_2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            seriesFill4.ForeColor = Color.Blue;
            seriesFill4.Transparency = 0.3;
            CheckFill(seriesFill4, FillType.Solid, Color.FromArgb(0xff, 0, 0, 0xff), Color.Empty, true, 0.3, false, false);

            // Check that an individual point with non-default formatting is not affected after changing series format.
            CheckFill(fill4_1, FillType.Solid, Color.FromArgb(0xff, 0, 0x80, 0),
                Color.Empty, true, 0, false, false);
            // This data point uses format of the parent series: check that its format is changed after series format
            // changing.
            CheckFill(fill4_2, FillType.Solid, Color.FromArgb(0xff, 0, 0, 0xff),
                Color.Empty, true, 0.3, false, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewDataPointFillFormat.docx");
        }


        [Test]
        public void TestNewMarkerFillFormat()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            series.Marker.Size = 20;
            ChartDataPointCollection dataPoints = series.DataPoints;

            Fill seriesFill = series.Marker.Format.Fill;
            CheckFill(seriesFill, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            Fill fill1 = dataPoints[0].Marker.Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.Solid();
            fill1.ForeColor = Color.Pink;
            fill1.Transparency = 0.5;
            CheckFill(fill1, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb),
                Color.Empty, true, 0.5, false, false);

            Fill fill2 = dataPoints[1].Marker.Format.Fill;
            fill2.ForeColor = Color.Yellow;
            fill2.Transparency = 0.2;
            CheckFill(fill2, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true, 0.2, false, false);

            Fill fill3 = dataPoints[2].Marker.Format.Fill;
            fill3.Visible = false;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            Fill fill4 = dataPoints[3].Marker.Format.Fill;
            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            seriesFill.ForeColor = Color.Blue;
            seriesFill.Transparency = 0.3;

            CheckFill(seriesFill, FillType.Solid, Color.FromArgb(0xff, 0, 0, 0xff),
                Color.Empty, true, 0.3, false, false);
            CheckFill(fill4, FillType.Solid, Color.FromArgb(0xff, 0, 0, 0xff),
                Color.Empty, true, 0.3, false, false);

            // Check that markers of individual points are not changed after changing series marker.
            CheckFill(fill1, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb),
                Color.Empty, true, 0.5, false, false);
            CheckFill(fill2, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true, 0.2, false, false);
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewMarkerFillFormat.docx");
        }


        [Test]
        public void TestNewSeriesStrokeFormat()
        {
            Document doc = CreateDocumentWithTwoSeriesScatterChart();
            ChartSeriesCollection series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            Stroke stroke1 = series[0].Format.Stroke;
            CheckStroke(stroke1, Color.Empty, Color.Empty, false,
                1, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            stroke1.Visible = true;
            stroke1.ForeColor = Color.Red;
            stroke1.Transparency = 0.5;
            stroke1.Weight = 2.5;
            stroke1.DashStyle = DashStyle.DashDot;
            stroke1.LineStyle = ShapeLineStyle.Double;
            stroke1.JoinStyle = JoinStyle.Miter;
            stroke1.EndCap = EndCap.Square;
            stroke1.StartArrowType = ArrowType.Open;
            stroke1.StartArrowLength = ArrowLength.Long;
            stroke1.StartArrowWidth = ArrowWidth.Narrow;
            stroke1.EndArrowType = ArrowType.Arrow;
            stroke1.EndArrowLength = ArrowLength.Long;
            stroke1.EndArrowWidth = ArrowWidth.Narrow;
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0, 0), Color.Empty, true,
                0.5, 2.5, DashStyle.DashDot, ShapeLineStyle.Double, JoinStyle.Miter, EndCap.Square,
                ArrowType.Open, ArrowType.Arrow, ArrowWidth.Narrow, ArrowLength.Long,
                ArrowWidth.Narrow, ArrowLength.Long, false);

            Stroke stroke2 = series[1].Format.Stroke;
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            stroke2.Visible = true;
            stroke2.ForeColor = Color.Blue;
            CheckStroke(stroke2, Color.FromArgb(0xff, 0, 0, 0xff), Color.Empty, true,
                0, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewSeriesStrokeFormat.docx");
        }


        [Test]
        public void TestNewDataPointStrokeFormat()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            ChartDataPointCollection dataPoints = series.DataPoints;

            // A data point represents formatting of a line from the previous point to this one.
            Stroke stroke1 = dataPoints[1].Format.Stroke;
            CheckStroke(stroke1, Color.Empty, Color.Empty, false,
                1, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            stroke1.Visible = true;
            stroke1.ForeColor = Color.Pink;
            stroke1.Transparency = 0.1;
            stroke1.Weight = 4;
            stroke1.DashStyle = DashStyle.ShortDot;
            stroke1.LineStyle = ShapeLineStyle.Triple;
            stroke1.JoinStyle = JoinStyle.Bevel;
            stroke1.EndCap = EndCap.Square;
            stroke1.StartArrowType = ArrowType.Diamond;
            stroke1.StartArrowLength = ArrowLength.Short;
            stroke1.StartArrowWidth = ArrowWidth.Wide;
            stroke1.EndArrowType = ArrowType.Oval;
            stroke1.EndArrowLength = ArrowLength.Short;
            stroke1.EndArrowWidth = ArrowWidth.Wide;
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb), Color.Empty, true,
                0.1, 4, DashStyle.ShortDot, ShapeLineStyle.Triple, JoinStyle.Bevel, EndCap.Square,
                ArrowType.Diamond, ArrowType.Oval, ArrowWidth.Wide, ArrowLength.Short,
                ArrowWidth.Wide, ArrowLength.Short, false);

            Stroke stroke2 = dataPoints[2].Format.Stroke;
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            stroke2.ForeColor = Color.Gray;
            stroke2.Weight = 4;
            CheckStroke(stroke2, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true,
                0, 4, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            Stroke nonDefinedStroke = dataPoints[3].Format.Stroke;
            CheckStroke(nonDefinedStroke, Color.Empty, Color.Empty, false,
                1, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            Stroke seriesStroke = series.Format.Stroke;
            CheckStroke(seriesStroke, Color.Empty, Color.Empty, false,
                1, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            seriesStroke.Visible = true;
            seriesStroke.Weight = 0.5;
            seriesStroke.DashStyle = DashStyle.ShortDash;
            CheckStroke(seriesStroke, Color.Empty, Color.Empty, true,
                0, 0.5, DashStyle.ShortDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            // Check that defined line formatting of the data points is not changed after changing line formatting
            // of the entire series.
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb), Color.Empty, true,
                0.1, 4, DashStyle.ShortDot, ShapeLineStyle.Triple, JoinStyle.Bevel, EndCap.Square,
                ArrowType.Diamond, ArrowType.Oval, ArrowWidth.Wide, ArrowLength.Short,
                ArrowWidth.Wide, ArrowLength.Short, false);
            CheckStroke(stroke2, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true,
                0, 4, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            // If line formatting is not defined, formatting of parent series is taken.
            CheckStroke(nonDefinedStroke, Color.Empty, Color.Empty, true,
                0, 0.5, DashStyle.ShortDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Round,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewDataPointStrokeFormat.docx");
        }


        [Test]
        public void TestNewMarkerStrokeFormat()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            series.Marker.Size = 20;
            ChartDataPointCollection dataPoints = series.DataPoints;

            Stroke seriesStroke = series.Marker.Format.Stroke;
            CheckStroke(seriesStroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            Stroke stroke1 = dataPoints[0].Marker.Format.Stroke;
            CheckStroke(stroke1, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            stroke1.Visible = true;
            stroke1.ForeColor = Color.Red;
            stroke1.Transparency = 0.3;
            stroke1.Weight = 2;
            stroke1.DashStyle = DashStyle.ShortDot;
            stroke1.LineStyle = ShapeLineStyle.Double;
            stroke1.JoinStyle = JoinStyle.Bevel;
            stroke1.EndCap = EndCap.Square;
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0, 0), Color.Empty, true,
                0.3, 2, DashStyle.ShortDot, ShapeLineStyle.Double, JoinStyle.Bevel, EndCap.Square);

            Stroke stroke2 = dataPoints[1].Marker.Format.Stroke;
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            seriesStroke.Visible = true;
            seriesStroke.ForeColor = Color.Yellow;
            seriesStroke.Weight = 0.5;
            CheckStroke(seriesStroke, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true,
                0, 0.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            // Check that format of the first marker is not changed after changing marker formatting of the series.
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0, 0), Color.Empty, true,
                0.3, 2, DashStyle.ShortDot, ShapeLineStyle.Double, JoinStyle.Bevel, EndCap.Square);

            // Check that format of the second marker is the same as series default marker format.
            CheckStroke(stroke2, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true,
                0, 0.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewMarkerStrokeFormat.docx");
        }


        [Test]
        public void TestSeriesPatternedFill()
        {
            Document doc = CreateDocumentWithColumnChart();
            ChartSeriesCollection series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            Fill fill1 = series[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.Patterned(PatternType.Cross);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill2 = series[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.Patterned(PatternType.DashedDownwardDiagonal);
            fill2.ForeColor = Color.White;
            fill2.BackColor = Color.Black;
            CheckPatternedFill(fill2, PatternType.DashedDownwardDiagonal, DrColor.White.ToNativeColor(),
                DrColor.Black.ToNativeColor(), 0);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestSeriesPatternedFill.docx", null, true);
            series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            fill1 = series[0].Format.Fill;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            fill1.Patterned(PatternType.DiagonalBrick);
            CheckPatternedFill(fill1, PatternType.DiagonalBrick, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill3 = series[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.Patterned(PatternType.DarkHorizontal);
            fill3.ForeColor = Color.Black;
            fill3.BackColor = Color.White;
            CheckPatternedFill(fill3, PatternType.DarkHorizontal, DrColor.Black.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0);

            Fill fill4 = series[3].Format.Fill;
            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill4.Patterned(PatternType.DottedDiamond);
            CheckPatternedFill(fill4, PatternType.DottedDiamond, Color.FromArgb(0x4f, 0x81, 0xBD),
                DrColor.White.ToNativeColor(), 0);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestSeriesPatternedFill2.docx");
        }

        [Test]
        public void TestDataPointPatternedFill()
        {
            Document doc = CreateDocumentWithPieChart();
            ChartDataPointCollection dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            Fill fill1 = dataPoints[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.Patterned(PatternType.Cross);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill2 = dataPoints[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.Patterned(PatternType.DashedDownwardDiagonal);
            fill2.ForeColor = Color.White;
            fill2.BackColor = Color.Black;
            CheckPatternedFill(fill2, PatternType.DashedDownwardDiagonal, DrColor.White.ToNativeColor(),
                DrColor.Black.ToNativeColor(), 0);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataPointPatternedFill.docx", null, true);
            dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            fill1 = dataPoints[0].Format.Fill;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            fill1.Patterned(PatternType.DiagonalBrick);
            CheckPatternedFill(fill1, PatternType.DiagonalBrick, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill3 = dataPoints[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.Patterned(PatternType.DarkHorizontal);
            fill3.ForeColor = Color.Black;
            fill3.BackColor = Color.White;
            CheckPatternedFill(fill3, PatternType.DarkHorizontal, DrColor.Black.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0);

            Fill fill4 = dataPoints[3].Format.Fill;
            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill4.Patterned(PatternType.DottedDiamond);
            CheckPatternedFill(fill4, PatternType.DottedDiamond, Color.FromArgb(0x4f, 0x81, 0xBD),
                DrColor.White.ToNativeColor(), 0);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestDataPointPatternedFill2.docx");
        }

        [Test]
        public void TestMarkerPatternedFill()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            series.Marker.Size = 40;
            series.Marker.Symbol = MarkerSymbol.Square;
            ChartDataPointCollection dataPoints = series.DataPoints;

            Fill fill1 = dataPoints[0].Marker.Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.Patterned(PatternType.Cross);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill2 = dataPoints[1].Marker.Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.Patterned(PatternType.DashedDownwardDiagonal);
            fill2.ForeColor = Color.White;
            fill2.BackColor = Color.Black;
            CheckPatternedFill(fill2, PatternType.DashedDownwardDiagonal, DrColor.White.ToNativeColor(),
                DrColor.Black.ToNativeColor(), 0);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestMarkerPatternedFill.docx", null, true);
            dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            fill1 = dataPoints[0].Marker.Format.Fill;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            fill1.Patterned(PatternType.DiagonalBrick);
            CheckPatternedFill(fill1, PatternType.DiagonalBrick, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill3 = dataPoints[2].Marker.Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.Patterned(PatternType.DarkHorizontal);
            fill3.ForeColor = Color.Black;
            fill3.BackColor = Color.White;
            CheckPatternedFill(fill3, PatternType.DarkHorizontal, DrColor.Black.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0);

            Fill fill4 = dataPoints[3].Marker.Format.Fill;
            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill4.Patterned(PatternType.DottedDiamond);
            CheckPatternedFill(fill4, PatternType.DottedDiamond, Color.FromArgb(0x4f, 0x81, 0xBD),
                DrColor.White.ToNativeColor(), 0);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestMarkerPatternedFill2.docx");
        }


        [Test]
        public void TestSeriesOneColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart();
            ChartSeriesCollection series = doc.FirstSection.Body.Shapes[0].Chart.Series;
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = series[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = series[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill2.ForeColor = Color.Black;
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Black.ToNativeColor(),
                Color.FromArgb(0x4f, 0x81, 0xbd), 0, 60, 0.5, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestSeriesOneColorGradientFill.docx", null, true);
            series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            fill1 = series[0].Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.OneColorGradient(Color.Green, GradientStyle.Vertical, GradientVariant.Variant1, 0.25);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1,
                DrColor.Green.ToNativeColor(), 0, 0, 0, 0.5, themeProvider);

            Fill fill3 = series[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1,
                DrColor.Blue.ToNativeColor(), 0, 90, 0.5, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestSeriesOneColorGradientFill2.docx");
        }

        [Test]
        public void TestDataPointOneColorGradientFill()
        {
            Document doc = CreateDocumentWithPieChart();
            ChartDataPointCollection dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = dataPoints[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = dataPoints[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill2.ForeColor = Color.Black;
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Black.ToNativeColor(),
                Color.FromArgb(0x4f, 0x81, 0xbd), 0, 60, 0.5, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataPointOneColorGradientFill.docx", null, true);
            dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            fill1 = dataPoints[0].Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.OneColorGradient(Color.Green, GradientStyle.Vertical, GradientVariant.Variant1, 0.25);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1,
                DrColor.Green.ToNativeColor(), 0, 0, 0, 0.5, themeProvider);

            Fill fill3 = dataPoints[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1,
                DrColor.Blue.ToNativeColor(), 0, 90, 0.5, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestDataPointOneColorGradientFill2.docx");
        }

        [Test]
        public void TestMarkerOneColorGradientFill()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            series.Marker.Size = 40;
            series.Marker.Symbol = MarkerSymbol.Square;
            ChartDataPointCollection dataPoints = series.DataPoints;
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = dataPoints[0].Marker.Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = dataPoints[1].Marker.Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill2.ForeColor = Color.Black;
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Black.ToNativeColor(),
                Color.FromArgb(0x4f, 0x81, 0xbd), 0, 60, 0.5, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestMarkerOneColorGradientFill.docx", null, true);
            dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            fill1 = dataPoints[0].Marker.Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.OneColorGradient(Color.Green, GradientStyle.Vertical, GradientVariant.Variant1, 0.25);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1,
                DrColor.Green.ToNativeColor(), 0, 0, 0, 0.5, themeProvider);

            Fill fill3 = dataPoints[2].Marker.Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1,
                DrColor.Blue.ToNativeColor(), 0, 90, 0.5, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestMarkerOneColorGradientFill2.docx");
        }


        [Test]
        public void TestSeriesTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart();
            ChartSeriesCollection series = doc.FirstSection.Body.Shapes[0].Chart.Series;
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = series[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = series[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.TwoColorGradient(Color.Green, Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1);
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Green.ToNativeColor(),
                DrColor.Blue.ToNativeColor(), 0, 60, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestSeriesTwoColorGradientFill.docx", null, true);
            series = doc.FirstSection.Body.Shapes[0].Chart.Series;

            fill1 = series[0].Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.TwoColorGradient(Color.Brown, Color.White, GradientStyle.Vertical, GradientVariant.Variant1);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1, DrColor.Brown.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 0, 0, 0, themeProvider);

            Fill fill3 = series[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Blue.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 90, 0, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestSeriesTwoColorGradientFill2.docx");
        }

        [Test]
        public void TestDataPointTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithPieChart();
            ChartDataPointCollection dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = dataPoints[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = dataPoints[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.TwoColorGradient(Color.Green, Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1);
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Green.ToNativeColor(),
                DrColor.Blue.ToNativeColor(), 0, 60, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataPointTwoColorGradientFill.docx", null, true);
            dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            fill1 = dataPoints[0].Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.TwoColorGradient(Color.Brown, Color.White, GradientStyle.Vertical, GradientVariant.Variant1);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1, DrColor.Brown.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 0, 0, 0, themeProvider);

            Fill fill3 = dataPoints[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Blue.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 90, 0, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestDataPointTwoColorGradientFill2.docx");
        }

        [Test]
        public void TestMarkerTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            series.Marker.Size = 40;
            series.Marker.Symbol = MarkerSymbol.Square;
            ChartDataPointCollection dataPoints = series.DataPoints;
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = dataPoints[0].Marker.Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = dataPoints[1].Marker.Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.TwoColorGradient(Color.Green, Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1);
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Green.ToNativeColor(),
                DrColor.Blue.ToNativeColor(), 0, 60, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestMarkerTwoColorGradientFill.docx", null, true);
            dataPoints = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataPoints;

            fill1 = dataPoints[0].Marker.Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.TwoColorGradient(Color.Brown, Color.White, GradientStyle.Vertical, GradientVariant.Variant1);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1, DrColor.Brown.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 0, 0, 0, themeProvider);

            Fill fill3 = dataPoints[2].Marker.Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Blue.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 90, 0, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestMarkerTwoColorGradientFill2.docx");
        }







        /// <summary>
        /// WORDSNET-24941 Setting color of a gradient stop of data point changes color of entire series
        /// This happened because the <see cref="ChartDataPoint.SpPr"/> property of the data point inherits its value
        /// from <see cref="ChartSeries.DefaultDataPoint"/> and should be materialized before changing the format.
        /// </summary>
        [Test]
        public void TestSettingDataPointGradientStopColor()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;
            ChartSeries series = chart.Series[0];

            Fill seriesFill = series.Format.Fill;
            seriesFill.TwoColorGradient(Color.Yellow, Color.Blue, GradientStyle.Vertical, GradientVariant.Variant4);

            Assert.That(seriesFill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(0xff, 0xff, 0xff, 0)));

            ChartDataPoint dataPoint1 = series.DataPoints[0];
            Fill pointFill = dataPoint1.Format.Fill;

            Assert.That(pointFill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(0xff, 0xff, 0xff, 0)));

            // Before the fix, this changed color of the gradient stop of the entire series.
            pointFill.GradientStops[0].Color = Color.LightBlue;

            Assert.That(seriesFill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(0xff, 0xff, 0xff, 0)));
            Assert.That(pointFill.GradientStops[0].Color, Is.EqualTo(Color.FromArgb(0xff, 0xad, 0xd8, 0xe6)));
        }


        /// <summary>
        /// Tests the <see cref="ChartFormat.Fill"/> property of data labels of a created chart.
        /// </summary>
        [Test]
        public void TestNewDataLabelFillFormat()
        {
            Document doc = CreateDocumentWithColumnChart();
            ChartSeriesCollection seriesCollection = doc.FirstSection.Body.Shapes[0].Chart.Series;
            foreach (ChartSeries series in seriesCollection)
            {
                series.HasDataLabels = true;
                series.DataLabels.ShowValue = true;
            }

            Fill fill1_1 = seriesCollection[0].DataLabels[0].Format.Fill;
            CheckFill(fill1_1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1_1.Solid();
            fill1_1.ForeColor = Color.Pink;
            fill1_1.Transparency = 0.5;
            CheckFill(fill1_1, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb),
                Color.Empty, true, 0.5, false, false);

            Fill fill1_2 = seriesCollection[0].DataLabels[1].Format.Fill;
            CheckFill(fill1_2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1_2.ForeColor = Color.Gray;
            CheckFill(fill1_2, FillType.Solid, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true, 0, false, false);

            fill1_2.Visible = false;
            CheckFill(fill1_2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            // Check that Visible == false can be replaced with solid fill.
            fill1_2.ForeColor = Color.Yellow;
            fill1_2.Transparency = 0.2;
            CheckFill(fill1_2, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true, 0.2, false, false);

            Fill labelsFill2 = seriesCollection[1].DataLabels.Format.Fill;
            labelsFill2.Solid(Color.Yellow);
            CheckFill(labelsFill2, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.Empty, true, 0, false, false);
            labelsFill2.Visible = false;

            Fill fill3_1 = seriesCollection[2].DataLabels[0].Format.Fill;
            CheckFill(fill3_1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);
            fill3_1.ForeColor = Color.Aqua;

            Fill fill3_2 = seriesCollection[2].DataLabels[1].Format.Fill;
            CheckFill(fill3_2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            Fill labelsFill4 = seriesCollection[3].DataLabels.Format.Fill;
            CheckFill(labelsFill4, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            Fill fill4_1 = seriesCollection[3].DataLabels[0].Format.Fill;
            CheckFill(fill4_1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);
            fill4_1.ForeColor = Color.LightGreen;

            Fill fill4_2 = seriesCollection[3].DataLabels[1].Format.Fill;
            CheckFill(fill4_2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            labelsFill4.ForeColor = Color.LightBlue;
            labelsFill4.Transparency = 0.3;
            CheckFill(labelsFill4, FillType.Solid, Color.FromArgb(0xff, 0xad, 0xd8, 0xe6),
                Color.Empty, true, 0.3, false, false);

            // Check that an individual label with non-default formatting is not affected after changing series format.
            CheckFill(fill4_1, FillType.Solid, Color.FromArgb(0xff, 0x90, 0xee, 0x90),
                Color.Empty, true, 0, false, false);
            // This data label uses format of the parent collection: check that its format is changed after collection
            // format changing.
            CheckFill(fill4_2, FillType.Solid, Color.FromArgb(0xff, 0xad, 0xd8, 0xe6),
                Color.Empty, true, 0.3, false, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewDataLabelFillFormat.docx");
        }


        /// <summary>
        /// Tests the <see cref="ChartFormat.Stroke"/> property of data labels of a created chart.
        /// </summary>
        [Test]
        public void TestNewDataLabelStrokeFormat()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            ChartSeries series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            series.HasDataLabels = true;
            ChartDataLabelCollection dataLabels = series.DataLabels;
            dataLabels.ShowValue = true;
            dataLabels.Font.Size = 32;

            Stroke stroke1 = dataLabels[0].Format.Stroke;
            CheckStroke(stroke1, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            stroke1.Visible = true;
            stroke1.ForeColor = Color.Pink;
            stroke1.Transparency = 0.1;
            stroke1.Weight = 4;
            stroke1.DashStyle = DashStyle.ShortDot;
            stroke1.LineStyle = ShapeLineStyle.Triple;
            stroke1.JoinStyle = JoinStyle.Bevel;
            stroke1.EndCap = EndCap.Square;
            stroke1.StartArrowType = ArrowType.Diamond;
            stroke1.StartArrowLength = ArrowLength.Short;
            stroke1.StartArrowWidth = ArrowWidth.Wide;
            stroke1.EndArrowType = ArrowType.Oval;
            stroke1.EndArrowLength = ArrowLength.Short;
            stroke1.EndArrowWidth = ArrowWidth.Wide;
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb), Color.Empty, true,
                0.1, 4, DashStyle.ShortDot, ShapeLineStyle.Triple, JoinStyle.Bevel, EndCap.Square,
                ArrowType.Diamond, ArrowType.Oval, ArrowWidth.Wide, ArrowLength.Short,
                ArrowWidth.Wide, ArrowLength.Short, false);

            Stroke stroke2 = dataLabels[1].Format.Stroke;
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            stroke2.ForeColor = Color.Gray;
            stroke2.Weight = 4;
            CheckStroke(stroke2, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true,
                0, 4, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            Stroke nonDefinedStroke = dataLabels[2].Format.Stroke;
            CheckStroke(nonDefinedStroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            Stroke collectionStroke = dataLabels.Format.Stroke;
            CheckStroke(collectionStroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            collectionStroke.Visible = true;
            collectionStroke.Weight = 0.5;
            collectionStroke.DashStyle = DashStyle.ShortDash;
            CheckStroke(collectionStroke, Color.Empty, Color.Empty, true,
                0, 0.5, DashStyle.ShortDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            // Check that defined line formatting of the data labels is not changed after changing line formatting
            // of the collection.
            CheckStroke(stroke1, Color.FromArgb(0xff, 0xff, 0xc0, 0xcb), Color.Empty, true,
                0.1, 4, DashStyle.ShortDot, ShapeLineStyle.Triple, JoinStyle.Bevel, EndCap.Square,
                ArrowType.Diamond, ArrowType.Oval, ArrowWidth.Wide, ArrowLength.Short,
                ArrowWidth.Wide, ArrowLength.Short, false);
            CheckStroke(stroke2, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true,
                0, 4, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            // If line formatting is not defined, formatting of parent collection is taken.
            CheckStroke(nonDefinedStroke, Color.Empty, Color.Empty, true,
                0, 0.5, DashStyle.ShortDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewDataLabelStrokeFormat.docx");
        }

        /// <summary>
        /// Tests applying a patterned fill to data labels.
        /// </summary>
        [Test]
        public void TestDataLabelPatternedFill()
        {
            Document doc = CreateDocumentWithPieChart();
            ChartDataLabelCollection dataLabels = ShowDataLabels(doc.FirstSection.Body.Shapes[0].Chart.Series[0]);

            Fill fill1 = dataLabels[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.Patterned(PatternType.Cross);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill2 = dataLabels[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.Patterned(PatternType.DashedDownwardDiagonal);
            fill2.ForeColor = Color.White;
            fill2.BackColor = Color.Gray;
            CheckPatternedFill(fill2, PatternType.DashedDownwardDiagonal, DrColor.White.ToNativeColor(),
                DrColor.Gray.ToNativeColor(), 0);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataLabelPatternedFill.docx", null, true);
            dataLabels = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataLabels;

            Fill collectionFill = dataLabels.Format.Fill;
            CheckFill(collectionFill, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);
            collectionFill.Patterned(PatternType.LargeGrid, Color.Blue, Color.Orange);
            CheckFill(collectionFill, FillType.Patterned, DrColor.Blue.ToNativeColor(),
                DrColor.Orange.ToNativeColor(), true, 0, false, false);

            fill1 = dataLabels[0].Format.Fill;
            CheckPatternedFill(fill1, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            fill1.Patterned(PatternType.DiagonalBrick);
            CheckPatternedFill(fill1, PatternType.DiagonalBrick, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fill3 = dataLabels[2].Format.Fill;
            // Inherits format from the collection.
            CheckFill(fill3, FillType.Patterned, DrColor.Blue.ToNativeColor(),
                DrColor.Orange.ToNativeColor(), true, 0, false, false);

            fill3.Patterned(PatternType.DarkHorizontal);
            fill3.ForeColor = Color.Black;
            fill3.BackColor = Color.White;
            CheckPatternedFill(fill3, PatternType.DarkHorizontal, DrColor.Black.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0);

            Fill fill4 = dataLabels[3].Format.Fill;
            // Inherits format from the collection.
            CheckFill(fill4, FillType.Patterned, DrColor.Blue.ToNativeColor(),
                DrColor.Orange.ToNativeColor(), true, 0, false, false);

            fill4.Patterned(PatternType.DottedDiamond);
            CheckPatternedFill(fill4, PatternType.DottedDiamond, DrColor.Blue.ToNativeColor(),
                DrColor.Orange.ToNativeColor(), 0);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestDataLabelPatternedFill2.docx");
        }

        /// <summary>
        /// Tests applying a one-color gradient fill to data labels.
        /// </summary>
        [Test]
        public void TestDataLabelOneColorGradientFill()
        {
            Document doc = CreateDocumentWithPieChart();
            ChartDataLabelCollection dataLabels = ShowDataLabels(doc.FirstSection.Body.Shapes[0].Chart.Series[0]);
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = dataLabels[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = dataLabels[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fill2.ForeColor = Color.Black;
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Black.ToNativeColor(),
                Color.FromArgb(0x4f, 0x81, 0xbd), 0, 60, 0.5, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataLabelOneColorGradientFill.docx", null, true);
            dataLabels = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataLabels;

            fill1 = dataLabels[0].Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.OneColorGradient(Color.Green, GradientStyle.Vertical, GradientVariant.Variant1, 0.25);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1,
                DrColor.Green.ToNativeColor(), 0, 0, 0, 0.5, themeProvider);

            Fill fill3 = dataLabels[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1,
                DrColor.Blue.ToNativeColor(), 0, 90, 0.5, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestDataLabelOneColorGradientFill2.docx");
        }

        /// <summary>
        /// Tests applying a two-color gradient fill to data labels.
        /// </summary>
        [Test]
        public void TestDataLabelTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithPieChart();
            ChartDataLabelCollection dataLabels = ShowDataLabels(doc.FirstSection.Body.Shapes[0].Chart.Series[0]);
            IThemeProvider themeProvider = doc.Theme;

            Fill fill1 = dataLabels[0].Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill1.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            fill1.ForeColor = Color.Red;
            fill1.BackColor = Color.Yellow;
            fill1.Transparency = 0.5;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fill2 = dataLabels[1].Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill2.TwoColorGradient(Color.Green, Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1);
            fill2.GradientAngle = 60;
            CheckGradientFill(fill2, GradientStyle.None, GradientVariant.None, DrColor.Green.ToNativeColor(),
                DrColor.Blue.ToNativeColor(), 0, 60, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataLabelTwoColorGradientFill.docx", null, true);
            dataLabels = doc.FirstSection.Body.Shapes[0].Chart.Series[0].DataLabels;

            fill1 = dataLabels[0].Format.Fill;
            CheckGradientFill(fill1, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fill1.TwoColorGradient(Color.Brown, Color.White, GradientStyle.Vertical, GradientVariant.Variant1);
            CheckGradientFill(fill1, GradientStyle.Vertical, GradientVariant.Variant1, DrColor.Brown.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 0, 0, 0, themeProvider);

            Fill fill3 = dataLabels[2].Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            fill3.ForeColor = Color.Blue;
            fill3.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            CheckGradientFill(fill3, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Blue.ToNativeColor(),
                DrColor.White.ToNativeColor(), 0, 90, 0, 0, themeProvider);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestDataLabelTwoColorGradientFill2.docx");
        }


        /// <summary>
        /// Tests that all <see cref="ChartShapeType"/> elements are correctly displayed in data labels.
        /// </summary>
        [Test]
        public void TestAllChartShapeTypes()
        {
            Document doc = new Document();
            doc.FirstSection.PageSetup.Orientation = Orientation.Landscape;

            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Scatter, 648, 432);
            Chart chart = shape.Chart;
            ChartSeriesCollection seriesCollection = chart.Series;
            seriesCollection.Clear();

            ChartShapeType[] shapeTypes = (ChartShapeType[])Enum.GetValues(typeof(ChartShapeType));
            double[] x = new double[shapeTypes.Length];
            double[] y = new double[shapeTypes.Length];

            // Fill data.
            for (int i = 0; i < shapeTypes.Length; i++)
            {
                x[i] = i + 1;
                const int pointLineCount = 8;
                y[i] = 4 * (i % pointLineCount) + 1;
            }

            ChartSeries series = seriesCollection.Add("Series 1", x, y);

            series.HasDataLabels = true;
            series.DataLabels.ShowValue = true;
            series.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, ChartDataLabelPosition.Above);

            // Generate callouts.
            for (int i = 0; i < shapeTypes.Length; i++)
            {
                ChartFormat format = series.DataLabels[i].Format;
                format.Stroke.ForeColor = Color.DarkGray;
                format.ShapeType = shapeTypes[i];

                Assert.That(format.ShapeType, Is.EqualTo(shapeTypes[i]));
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestAllChartShapeTypes.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartFormat.Fill"/> property of axis tick labels of an existing chart.
        /// </summary>
        [Test]
        public void TestAxisTickLabelFillFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestAxisFormat.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            Chart chart1 = shapes[0].Chart;
            Chart chart2 = shapes[1].Chart;
            Chart chart3 = shapes[2].Chart;

            Fill patternFill = chart1.AxisX.Format.Fill;
            CheckPatternedFill(patternFill, PatternType.Percent50, Color.FromArgb(0xff, 0x8d, 0xa9, 0xdb),
                Color.FromArgb(0xff, 0xff, 0xff, 0xff), 0);

            Fill texturedFill = chart1.AxisY.Format.Fill;
            CheckFill(texturedFill, FillType.Textured, Color.Empty, Color.Empty, true, 0, true, true);

            Fill gradientFill = chart2.AxisX.Format.Fill;
            CheckFill(gradientFill, FillType.Gradient, Color.FromArgb(0xff, 0xbb, 0xd6, 0xee),
                Color.FromArgb(0xff, 0x25, 0x5d, 0x91), true, 0, true, false);

            Fill solidFill = chart2.AxisY.Format.Fill;
            CheckFill(solidFill, FillType.Solid, Color.FromArgb(0xff, 0xb3, 0xc6, 0xe7),
                Color.Empty, true, 0.15, false, false);

            Fill noFill = chart2.Axes[2].Format.Fill;
            CheckFill(noFill, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            // Word 2016 charts do not support fill of axis tick labels: check that the fill is empty.
            Fill fill2016 = chart3.AxisX.Format.Fill;
            CheckFill(fill2016, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);

            // Change format.

            patternFill.ForeColor = Color.Black;
            patternFill.BackColor = Color.Lime;
            CheckPatternedFill(patternFill, PatternType.Percent50, Color.FromArgb(0xff, 0, 0, 0),
                Color.FromArgb(0xff, 0, 0xff, 0), 0);

            texturedFill.Opacity = 0.55;
            CheckFill(texturedFill, FillType.Textured, Color.Empty, Color.Empty, true, 0.45, true, true);

            gradientFill.ForeColor = Color.Blue;
            gradientFill.BackColor = Color.Green;
            CheckFill(gradientFill, FillType.Gradient, Color.FromArgb(0xff, 0, 0, 0xff),
                Color.FromArgb(0xff, 0, 0x80, 0), true, 0, true, false);

            solidFill.Transparency = 0;
            CheckFill(solidFill, FillType.Solid, Color.FromArgb(0xff, 0xb3, 0xc6, 0xe7),
                Color.Empty, true, 0, false, false);

            noFill.ForeColor = Color.Red;
            CheckFill(noFill, FillType.Solid, Color.FromArgb(0xff, 0xff, 0, 0),
                Color.Empty, true, 0, false, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestAxisTickLabelFillFormat.docx");
        }

        /// <summary>
        /// Tests that an exception is generated when changing fill of axis tick labels of a Word 2016 chart.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException),
             ExpectedMessage = "Fill is not supported by this chart element.")]
        public void TestChangingFillOfWord2016ChartTickLabel()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestAxisFormat.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            Chart chart3 = shapes[2].Chart;

            chart3.AxisX.Format.Fill.ForeColor = Color.Red;
        }

        /// <summary>
        /// Tests the <see cref="ChartFormat.Fill"/> property of axis tick labels of a created chart.
        /// </summary>
        [Test]
        public void TestNewChartAxisTickLabelFillFormat()
        {
            Document doc = CreateDocumentWithColumnChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            Fill xFill = chart.AxisX.Format.Fill;
            CheckFill(xFill, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            xFill.TwoColorGradient(Color.Yellow, Color.Lime, GradientStyle.DiagonalDown, GradientVariant.Variant1);
            xFill.Transparency = 0.5;
            CheckFill(xFill, FillType.Gradient, Color.FromArgb(0xff, 0xff, 0xff, 0x00),
                Color.FromArgb(0xff, 0, 0xff, 0x00), true, 0.5, true, false);

            Fill yFill = chart.AxisY.Format.Fill;
            CheckFill(yFill, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            yFill.ForeColor = Color.LightGray;
            CheckFill(yFill, FillType.Solid, Color.FromArgb(0xff, 0xd3, 0xd3, 0xd3), Color.Empty, true, 0, false, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewChartAxisTickLabelFillFormat.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartFormat.Stroke"/> property of axes of an existing chart.
        /// </summary>
        [Test]
        public void TestAxisStrokeFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestAxisFormat.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            Chart chart1 = shapes[0].Chart;
            Chart chart2 = shapes[1].Chart;
            Chart chart3 = shapes[2].Chart;

            Stroke strokeX1 = chart1.AxisX.Format.Stroke;
            CheckStroke(strokeX1, Color.FromArgb(0xff, 0xbb, 0xd6, 0xee), Color.FromArgb(0xff, 0x25, 0x5d, 0x91), true,
                0, 5, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            Stroke strokeY1 = chart1.AxisY.Format.Stroke;
            CheckStroke(strokeY1, Color.FromArgb(0xff, 0x2f, 0x54, 0x96), Color.Empty, true,
                0, 1.25, DashStyle.ShortDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat, ArrowType.None,
                ArrowType.Stealth, ArrowWidth.Medium, ArrowLength.Medium, ArrowWidth.Medium, ArrowLength.Long, false);

            Stroke strokeX2 = chart2.AxisX.Format.Stroke;
            CheckStroke(strokeX2, Color.FromArgb(0xff, 0x2f, 0x54, 0x96), Color.Empty, true,
                0.5, 3, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat, ArrowType.Oval,
                ArrowType.Oval, ArrowWidth.Medium, ArrowLength.Medium, ArrowWidth.Medium, ArrowLength.Medium, false);

            Stroke strokeY2 = chart2.AxisY.Format.Stroke;
            CheckStroke(strokeY2, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            Stroke strokeYR2 = chart2.Axes[2].Format.Stroke;
            CheckStroke(strokeYR2, Color.FromArgb(0xff, 0x2f, 0x54, 0x96), Color.Empty, true,
                0, 1.75, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            Stroke strokeX3 = chart3.AxisX.Format.Stroke;
            CheckStroke(strokeX3, Color.FromArgb(0xff, 0xbf, 0x90, 0x00), Color.FromArgb(0xff, 0xff, 0xc0, 0x00), true,
                0, 5.25, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            Stroke strokeY3 = chart3.AxisY.Format.Stroke;
            CheckStroke(strokeY3, Color.FromArgb(0xff, 0x2f, 0x54, 0x96), Color.Empty, true,
                0, 2.5, DashStyle.Dash, ShapeLineStyle.Single, JoinStyle.Bevel, EndCap.Square);

            // Change format.

            strokeX1.ForeColor = Color.Red;
            strokeX1.Opacity = 0.2;
            CheckStroke(strokeX1, Color.FromArgb(0xff, 0xff, 0, 0), Color.FromArgb(0xff, 0x25, 0x5d, 0x91), true,
                0.8, 5, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            strokeY1.Visible = false;
            CheckStroke(strokeY1, Color.Empty, Color.Empty, false,
                1, 1.25, DashStyle.ShortDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat, ArrowType.None,
                ArrowType.Stealth, ArrowWidth.Medium, ArrowLength.Medium, ArrowWidth.Medium, ArrowLength.Long, false);

            strokeX2.ForeColor = Color.Green;
            strokeX2.DashStyle = DashStyle.LongDashDotDot;
            strokeX2.LineStyle = ShapeLineStyle.Double;
            strokeX2.JoinStyle = JoinStyle.Bevel;
            strokeX2.EndCap = EndCap.Square;
            strokeX2.StartArrowType = ArrowType.Arrow;
            strokeX2.StartArrowLength = ArrowLength.Short;
            strokeX2.StartArrowWidth = ArrowWidth.Wide;
            strokeX2.EndArrowType = ArrowType.Arrow;
            strokeX2.EndArrowLength = ArrowLength.Short;
            strokeX2.EndArrowWidth = ArrowWidth.Wide;
            CheckStroke(strokeX2, Color.FromArgb(0xff, 0, 0x80, 0), Color.Empty, true,
                0, 3, DashStyle.LongDashDotDot, ShapeLineStyle.Double, JoinStyle.Bevel, EndCap.Square, ArrowType.Arrow,
                ArrowType.Arrow, ArrowWidth.Wide, ArrowLength.Short, ArrowWidth.Wide, ArrowLength.Short, false);

            strokeY2.Visible = true;
            CheckStroke(strokeY2, Color.Empty, Color.Empty, true,
                0, 0.75, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            strokeYR2.Fill.TwoColorGradient(Color.Yellow, Color.Lime, GradientStyle.Horizontal, GradientVariant.Variant2);
            strokeYR2.Transparency = 0.25;
            CheckStroke(strokeYR2, Color.FromArgb(0xff, 0xff, 0xff, 0), Color.FromArgb(0xff, 0, 0xff, 0), true,
                0.25, 1.75, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            strokeX3.DashStyle = DashStyle.LongDash;
            strokeX3.LineStyle = ShapeLineStyle.Triple;
            strokeX3.Weight = 10;
            CheckStroke(strokeX3, Color.FromArgb(0xff, 0xbf, 0x90, 0x00), Color.FromArgb(0xff, 0xff, 0xc0, 0x00), true,
                0, 10, DashStyle.LongDash, ShapeLineStyle.Triple, JoinStyle.Round, EndCap.Flat);

            strokeY3.ForeColor = Color.Orange;
            strokeY3.JoinStyle = JoinStyle.Miter;
            strokeY3.EndCap = EndCap.Round;
            strokeY3.StartArrowType = ArrowType.Diamond;
            strokeY3.StartArrowLength = ArrowLength.Long;
            strokeY3.StartArrowWidth = ArrowWidth.Narrow;
            strokeY3.EndArrowType = ArrowType.Diamond;
            strokeY3.EndArrowLength = ArrowLength.Long;
            strokeY3.EndArrowWidth = ArrowWidth.Narrow;
            CheckStroke(strokeY3, Color.FromArgb(0xff, 0xff, 0xa5, 0), Color.Empty, true,
                0, 2.5, DashStyle.Dash, ShapeLineStyle.Single, JoinStyle.Miter, EndCap.Round, ArrowType.Diamond,
                ArrowType.Diamond, ArrowWidth.Narrow, ArrowLength.Long, ArrowWidth.Narrow, ArrowLength.Long, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestAxisStrokeFormat.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartFormat.Stroke"/> property of axes of a created chart.
        /// </summary>
        [Test]
        public void TestNewChartAxisStrokeFormat()
        {
            Document doc = CreateDocumentWithColumnChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            Stroke strokeX = chart.AxisX.Format.Stroke;
            CheckStroke(strokeX, Color.FromArgb(0xff, 0xd8, 0xd8, 0xd8), Color.Empty, true,
                0, 0.75, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            strokeX.ForeColor = Color.Gray;
            strokeX.Weight = 2;
            CheckStroke(strokeX, Color.FromArgb(0xff, 0x80, 0x80, 0x80), Color.Empty, true,
                0, 2, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            Stroke strokeY = chart.AxisY.Format.Stroke;
            CheckStroke(strokeY, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            strokeY.Visible = true;
            strokeY.ForeColor = Color.Blue;
            strokeY.Transparency = 0.1;
            strokeY.Weight = 2;
            strokeY.DashStyle = DashStyle.ShortDot;
            strokeY.LineStyle = ShapeLineStyle.Double;
            strokeY.JoinStyle = JoinStyle.Bevel;
            strokeY.EndCap = EndCap.Square;
            strokeY.StartArrowType = ArrowType.Oval;
            strokeY.StartArrowLength = ArrowLength.Short;
            strokeY.StartArrowWidth = ArrowWidth.Narrow;
            strokeY.EndArrowType = ArrowType.Stealth;
            strokeY.EndArrowLength = ArrowLength.Long;
            strokeY.EndArrowWidth = ArrowWidth.Narrow;
            CheckStroke(strokeY, Color.FromArgb(0xff, 0x00, 0x00, 0xff), Color.Empty, true,
                0.1, 2, DashStyle.ShortDot, ShapeLineStyle.Double, JoinStyle.Bevel, EndCap.Square,
                ArrowType.Oval, ArrowType.Stealth, ArrowWidth.Narrow, ArrowLength.Short,
                ArrowWidth.Narrow, ArrowLength.Long, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewChartAxisStrokeFormat.docx");
        }

        /// <summary>
        /// Tests applying a patterned fill to axis tick labels.
        /// </summary>
        [Test]
        public void TestAxisTickLabelPatternedFill()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            chart.AxisX.TickLabels.Font.Size = 24;
            chart.AxisY.TickLabels.Font.Size = 24;

            Fill fillX = chart.AxisX.Format.Fill;
            CheckFill(fillX, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            fillX.Patterned(PatternType.Cross);
            fillX.ForeColor = Color.Red;
            fillX.BackColor = Color.Yellow;
            fillX.Transparency = 0.5;
            CheckPatternedFill(fillX, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            Fill fillY = chart.AxisY.Format.Fill;
            CheckFill(fillY, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            fillY.Patterned(PatternType.DashedDownwardDiagonal);
            fillY.ForeColor = Color.White;
            fillY.BackColor = Color.Gray;
            CheckPatternedFill(fillY, PatternType.DashedDownwardDiagonal, DrColor.White.ToNativeColor(),
                DrColor.Gray.ToNativeColor(), 0);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestAxisTickLabelPatternedFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;

            fillX = chart.AxisX.Format.Fill;
            CheckPatternedFill(fillX, PatternType.Cross, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5);

            fillY = chart.AxisY.Format.Fill;
            CheckPatternedFill(fillY, PatternType.DashedDownwardDiagonal, DrColor.White.ToNativeColor(),
                DrColor.Gray.ToNativeColor(), 0);
        }

        /// <summary>
        /// Tests applying a one-color gradient fill to axis tick labels.
        /// </summary>
        [Test]
        public void TestAxisTickLabelOneColorGradientFill()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            chart.AxisX.TickLabels.Font.Size = 24;
            chart.AxisY.TickLabels.Font.Size = 24;
            IThemeProvider themeProvider = doc.Theme;

            Fill fillX = chart.AxisX.Format.Fill;
            CheckFill(fillX, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            fillX.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fillX.ForeColor = Color.Red;
            fillX.BackColor = Color.Yellow;
            fillX.Transparency = 0.5;
            CheckGradientFill(fillX, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fillY = chart.AxisY.Format.Fill;
            CheckFill(fillY, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            fillY.OneColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1, 0.75);
            fillY.ForeColor = Color.Black;
            fillY.GradientAngle = 60;
            CheckGradientFill(fillY, GradientStyle.None, GradientVariant.None, DrColor.Black.ToNativeColor(),
                Color.FromArgb(0x4f, 0x81, 0xbd), 0, 60, 0.5, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestAxisTickLabelOneColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            themeProvider = doc.Theme;

            fillX = chart.AxisX.Format.Fill;
            CheckGradientFill(fillX, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fillY = chart.AxisY.Format.Fill;
            CheckGradientFill(fillY, GradientStyle.None, GradientVariant.None, DrColor.Black.ToNativeColor(),
                Color.FromArgb(0x4f, 0x81, 0xbd), 0, 60, 0.5, 0, themeProvider);
        }

        /// <summary>
        /// Tests applying a two-color gradient fill to axis tick labels.
        /// </summary>
        [Test]
        public void TestAxisTickLabelTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            chart.AxisX.TickLabels.Font.Size = 24;
            chart.AxisY.TickLabels.Font.Size = 24;
            chart.AxisY.TickLabels.Font.Color = Color.White;
            IThemeProvider themeProvider = doc.Theme;

            Fill fillX = chart.AxisX.Format.Fill;
            CheckFill(fillX, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            fillX.TwoColorGradient(GradientStyle.Horizontal, GradientVariant.Variant1);
            fillX.ForeColor = Color.Red;
            fillX.BackColor = Color.Yellow;
            fillX.Transparency = 0.5;
            CheckGradientFill(fillX, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            Fill fillY = chart.AxisY.Format.Fill;
            CheckFill(fillY, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);

            fillY.TwoColorGradient(Color.Green, Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1);
            fillY.GradientAngle = 60;
            CheckGradientFill(fillY, GradientStyle.None, GradientVariant.None, DrColor.Green.ToNativeColor(),
                DrColor.Blue.ToNativeColor(), 0, 60, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestAxisTickLabelTwoColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            themeProvider = doc.Theme;

            fillX = chart.AxisX.Format.Fill;
            CheckGradientFill(fillX, GradientStyle.Horizontal, GradientVariant.Variant1, DrColor.Red.ToNativeColor(),
                DrColor.Yellow.ToNativeColor(), 0.5, 90, 0, 0, themeProvider);

            fillY = chart.AxisY.Format.Fill;
            CheckGradientFill(fillY, GradientStyle.None, GradientVariant.None, DrColor.Green.ToNativeColor(),
                DrColor.Blue.ToNativeColor(), 0, 60, 0, 0, themeProvider);
        }



        /// <summary>
        /// WORDSNET-23584 Support the "callout" data labels
        /// Tests roundtrip and changing shape type for individual data labels and the entire collection.
        /// </summary>
        [Test]
        public void Test23584()
        {
            const string fileName = @"Model\Charts\Test23584.docx";
            Document doc = TestUtil.Open(fileName);
            ChartSeriesCollection seriesCollection = doc.FirstSection.Body.Shapes[0].Chart.Series;
            ChartDataLabelCollection dataLabels1 = seriesCollection[0].DataLabels;

            Assert.That(dataLabels1.Format.ShapeType, Is.EqualTo(ChartShapeType.Default));
            Assert.That(dataLabels1[0].Format.ShapeType, Is.EqualTo(ChartShapeType.WedgeRectCallout));

            doc = TestUtil.SaveOpen(doc, fileName);
            seriesCollection = doc.FirstSection.Body.Shapes[0].Chart.Series;
            dataLabels1 = seriesCollection[0].DataLabels;

            Assert.That(dataLabels1.Format.ShapeType, Is.EqualTo(ChartShapeType.Default));
            Assert.That(dataLabels1[0].Format.ShapeType, Is.EqualTo(ChartShapeType.WedgeRectCallout));

            // Changing shape type.

            dataLabels1.ShowValue = true;
            dataLabels1.Format.ShapeType = ChartShapeType.Star;
            dataLabels1.Format.Stroke.ForeColor = Color.Red;
            dataLabels1[0].Format.ShapeType = ChartShapeType.Rectangle;
            dataLabels1[1].Format.ShapeType = ChartShapeType.Ellipse;
            dataLabels1[2].Font.Bold = true;

            Assert.That(dataLabels1.Format.ShapeType, Is.EqualTo(ChartShapeType.Star));
            Assert.That(dataLabels1[0].Format.ShapeType, Is.EqualTo(ChartShapeType.Rectangle));
            Assert.That(dataLabels1[1].Format.ShapeType, Is.EqualTo(ChartShapeType.Ellipse));
            Assert.That(dataLabels1[2].Format.ShapeType, Is.EqualTo(ChartShapeType.Star));
            Assert.That(dataLabels1[2].Font.Bold, Is.True);
            Assert.That(dataLabels1[3].Format.ShapeType, Is.EqualTo(ChartShapeType.Star));

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\Test23584Changed.docx");
            seriesCollection = doc.FirstSection.Body.Shapes[0].Chart.Series;
            dataLabels1 = seriesCollection[0].DataLabels;

            Assert.That(dataLabels1.Format.ShapeType, Is.EqualTo(ChartShapeType.Star));
            Assert.That(dataLabels1[0].Format.ShapeType, Is.EqualTo(ChartShapeType.Rectangle));
            Assert.That(dataLabels1[1].Format.ShapeType, Is.EqualTo(ChartShapeType.Ellipse));
            Assert.That(dataLabels1[2].Format.ShapeType, Is.EqualTo(ChartShapeType.Star));
            Assert.That(dataLabels1[2].Font.Bold, Is.True);
            Assert.That(dataLabels1[3].Format.ShapeType, Is.EqualTo(ChartShapeType.Star));
        }


        /// <summary>
        /// Tests the public <see cref="ChartFormat.SetDefaultFill"/> method.
        /// </summary>
        [Test]
        public void TestSettingDefaultFill()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestCopyingDataPointFormat.docx");
            ChartSeries series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            ChartDataPoint dataPoint11 = series1.DataPoints[0];
            ChartDataPoint dataPoint12 = series1.DataPoints[1];
            ChartSeries series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            ChartDataPoint dataPoint21 = series2.DataPoints[0];
            ChartDataPoint dataPoint22 = series2.DataPoints[1];

            Assert.That(dataPoint11.Format.IsDefined, Is.False);
            Assert.That(dataPoint21.Format.IsDefined, Is.False);
            Assert.That(dataPoint12.Format.IsDefined, Is.True);
            Assert.That(dataPoint22.Format.IsDefined, Is.True);

            // Fill of these data points is not defined.
            dataPoint11.Format.SetDefaultFill();
            dataPoint21.Format.SetDefaultFill();
            // Fill of these data points is defined.
            dataPoint12.Format.SetDefaultFill();
            dataPoint22.Format.SetDefaultFill();

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestSettingDefaultFillForItem.docx");

            series1.Format.Fill.TwoColorGradient(Color.Green, Color.Blue,
                GradientStyle.DiagonalDown, GradientVariant.Variant1);
            series2.Format.Fill.TwoColorGradient(Color.Green, Color.Blue,
                GradientStyle.DiagonalDown, GradientVariant.Variant1);
            series1.Format.SetDefaultFill();
            series2.Format.SetDefaultFill();

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestSettingDefaultFillForCollection.docx");
        }

        /// <summary>
        /// Tests the <see cref="Chart.Format"/> property of an existing chart.
        /// </summary>
        [Test]
        public void TestChartFillAndStrokeFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestChartFormat.docx");
            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            Chart chart1 = shape1.Chart;

            Paragraph paragraph1 = (Paragraph)shape1.GetAncestor(NodeType.Paragraph);
            Paragraph paragraph2 = (Paragraph)paragraph1.Clone(true);
            paragraph1.ParentNode.InsertAfter(paragraph2, paragraph1);
            paragraph1.ParentNode.InsertAfter(new Paragraph(doc), paragraph1);
            Shape shape2 = doc.FirstSection.Body.Shapes[1];
            Chart chart2 = shape2.Chart;

            Fill fill1 = chart1.Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.FromArgb(0x0c, 0x0c, 0x0c), Color.Empty, true, 0, false, false);

            // Check the chart sub-elements that fill is not defined.
            CheckFill(chart1.AxisX.Format.Fill, FillType.Solid, Color.Empty,
                Color.Empty, true, 0, false, false);
            CheckFill(chart1.DataTable.Format.Fill, FillType.Solid, Color.Empty,
                Color.Empty, true, 0, false, false);
            CheckFill(chart1.Series[0].DataLabels.Format.Fill, FillType.Solid, Color.Empty,
                Color.Empty, true, 0, false, false);

            Stroke stroke1 = chart1.Format.Stroke;
            CheckStroke(stroke1, Color.FromArgb(0x82, 0, 0), Color.Empty, true,
                0, 4, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            // Check the chart sub-elements that stroke is not defined.
            CheckStroke(chart1.AxisX.Format.Stroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);
            CheckStroke(chart1.DataTable.Format.Stroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);
            CheckStroke(chart1.Series[0].DataLabels.Format.Stroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            Fill fill2 = chart2.Format.Fill;
            CheckFill(fill2, FillType.Solid, Color.FromArgb(0x0c, 0x0c, 0x0c), Color.Empty, true, 0, false, false);

            Stroke stroke2 = chart2.Format.Stroke;
            CheckStroke(stroke2, Color.FromArgb(0x82, 0, 0), Color.Empty, true,
                0, 4, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            Shape shape3 = doc.FirstSection.Body.Shapes[2];
            Chart chart3 = shape3.Chart;

            Paragraph paragraph3 = (Paragraph)shape3.GetAncestor(NodeType.Paragraph);
            Paragraph paragraph4 = (Paragraph)paragraph3.Clone(true);
            paragraph3.ParentNode.InsertAfter(paragraph4, paragraph3);
            paragraph3.ParentNode.InsertAfter(new Paragraph(doc), paragraph3);
            Shape shape4 = doc.FirstSection.Body.Shapes[3];
            Chart chart4 = shape4.Chart;

            Fill fill3 = chart3.Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.FromArgb(0, 0, 0), Color.Empty, true, 0, false, false);

            Stroke stroke3 = chart3.Format.Stroke;
            CheckStroke(stroke3, Color.FromArgb(0xc0, 0, 0), Color.Empty, true,
                0, 3, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            Fill fill4 = chart4.Format.Fill;
            CheckFill(fill4, FillType.Solid, Color.FromArgb(0, 0, 0), Color.Empty, true, 0, false, false);

            Stroke stroke4 = chart4.Format.Stroke;
            CheckStroke(stroke4, Color.FromArgb(0xc0, 0, 0), Color.Empty, true,
                0, 3, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            // Change format.

            fill1.Color = Color.DarkBlue;
            fill1.Opacity = 0.55;
            stroke1.ForeColor = Color.Red;
            stroke1.Opacity = 0.2;
            stroke1.Weight = 5;
            stroke1.LineStyle = ShapeLineStyle.Triple;

            CheckFill(fill1, FillType.Solid, Color.FromArgb(0, 0, 0x8b), Color.Empty, true, 0.45, false, false);
            CheckStroke(stroke1, Color.FromArgb(0xff, 0, 0), Color.Empty, true,
                0.8, 5, DashStyle.Solid, ShapeLineStyle.Triple, JoinStyle.Round, EndCap.Flat);

            fill2.Visible = false;
            stroke2.Visible = false;

            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 4, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            fill3.Color = Color.DarkBlue;
            fill3.Opacity = 0.55;
            stroke3.ForeColor = Color.Red;
            stroke3.Opacity = 0.7;
            stroke3.Weight = 2;
            stroke3.LineStyle = ShapeLineStyle.Single;
            stroke3.DashStyle = DashStyle.ShortDot;

            CheckFill(fill3, FillType.Solid, Color.FromArgb(0, 0, 0x8b), Color.Empty, true, 0.45, false, false);
            CheckStroke(stroke3, Color.FromArgb(0xff, 0, 0), Color.Empty, true,
                0.3, 2, DashStyle.ShortDot, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill4.Visible = false;
            stroke4.Visible = false;

            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke4, Color.Empty, Color.Empty, false,
                1, 3, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestChartFillAndStrokeFormat.docx");
        }

        /// <summary>
        /// Tests the <see cref="Chart.Format"/> property of a created chart.
        /// </summary>
        [Test]
        public void TestNewChartFillAndStrokeFormat()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            Fill fill = chart.Format.Fill;
            CheckFill(fill, FillType.Solid, Color.FromArgb(0xff, 0xff, 0xff), Color.Empty, true, 0, false, false);

            Stroke stroke = chart.Format.Stroke;
            CheckStroke(stroke, Color.FromArgb(0xd8, 0xd8, 0xd8), Color.Empty, true,
                0, 0.75, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            fill.Solid();
            fill.ForeColor = Color.Cyan;
            fill.Transparency = 0.45;
            stroke.Color = Color.Blue;
            stroke.Transparency = 0.35;
            stroke.Weight = 2.5;
            stroke.DashStyle = DashStyle.ShortDash;
            stroke.LineStyle = ShapeLineStyle.Double;

            CheckFill(fill, FillType.Solid, Color.FromArgb(0, 0xff, 0xff),
                Color.Empty, true, 0.45, false, false);
            CheckStroke(stroke, Color.FromArgb(0, 0, 0xff), Color.Empty, true,
                0.35, 2.5, DashStyle.ShortDash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat,
                ArrowType.None, ArrowType.None, ArrowWidth.Medium, ArrowLength.Medium,
                ArrowWidth.Medium, ArrowLength.Medium, false);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestNewChartFillAndStrokeFormat.docx");
        }

        /// <summary>
        /// Tests applying a patterned fill to a chart.
        /// </summary>
        [Test]
        public void TestChartPatternedFill()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Format.Fill;

            fill.Patterned(PatternType.HorizontalBrick);
            fill.ForeColor = Color.Red;
            fill.BackColor = Color.Yellow;
            fill.Transparency = 0.8;
            CheckPatternedFill(fill, PatternType.HorizontalBrick, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.8);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestChartPatternedFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Format.Fill;

            CheckPatternedFill(fill, PatternType.HorizontalBrick, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.8);
        }

        /// <summary>
        /// Tests applying a one-color gradient fill to a chart.
        /// </summary>
        [Test]
        public void TestChartOneColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Format.Fill;
            IThemeProvider themeProvider = doc.Theme;

            fill.OneColorGradient(Color.Yellow, GradientStyle.Horizontal, GradientVariant.Variant2, 0.25);
            fill.Transparency = 0.6;
            CheckGradientFill(fill, GradientStyle.Horizontal, GradientVariant.Variant2, Color.FromArgb(0xff, 0xff, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.6, 270, 0, 0.5, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestChartOneColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Format.Fill;
            themeProvider = doc.Theme;

            CheckGradientFill(fill, GradientStyle.Horizontal, GradientVariant.Variant2, Color.FromArgb(0xff, 0xff, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.6, 270, 0, 0.5, themeProvider);
        }

        /// <summary>
        /// Tests applying a two-color gradient fill to a chart.
        /// </summary>
        [Test]
        public void TestChartTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Format.Fill;
            IThemeProvider themeProvider = doc.Theme;

            fill.TwoColorGradient(Color.Red, Color.Yellow, GradientStyle.Vertical, GradientVariant.Variant1);
            fill.Transparency = 0.7;
            CheckGradientFill(fill, GradientStyle.Vertical, GradientVariant.Variant1, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.7, 0, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestChartTwoColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Format.Fill;
            themeProvider = doc.Theme;

            CheckGradientFill(fill, GradientStyle.Vertical, GradientVariant.Variant1, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.7, 0, 0, 0, themeProvider);
        }


        /// <summary>
        /// Tests the <see cref="ChartTitle.Format"/> and <see cref="ChartAxisTitle.Format"/> properties of
        /// an existing chart.
        /// </summary>
        [Test]
        public void TestTitleFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestTitleFormat.docx");
            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            Chart chart1 = shape1.Chart;

            Paragraph paragraph1 = (Paragraph)shape1.GetAncestor(NodeType.Paragraph);
            Paragraph paragraph2 = (Paragraph)paragraph1.Clone(true);
            paragraph1.ParentNode.InsertAfter(paragraph2, paragraph1);
            paragraph1.ParentNode.InsertAfter(new Paragraph(doc), paragraph1);
            Shape shape2 = doc.FirstSection.Body.Shapes[1];
            Chart chart2 = shape2.Chart;

            Fill fill1 = chart1.Title.Format.Fill;
            CheckFill(fill1, FillType.Gradient, Color.FromArgb(0xff, 0xff, 0xff), Color.FromArgb(0xec, 0x7c, 0x31),
                true, 0.4, true, false);

            Stroke stroke1 = chart1.Title.Format.Stroke;
            CheckStroke(stroke1, Color.FromArgb(0xc5, 0x5a, 0x11), Color.Empty, true,
                0.15, 3.5, DashStyle.ShortDot, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            Fill axisFill1 = chart1.AxisX.Title.Format.Fill;
            CheckFill(axisFill1, FillType.Gradient, Color.FromArgb(0xbb, 0xd6, 0xee), Color.FromArgb(0x25, 0x5d, 0x91),
                true, 0.4, true, false);

            Stroke axisStroke1 = chart1.AxisX.Title.Format.Stroke;
            CheckStroke(axisStroke1, Color.FromArgb(0x7b, 0x7b, 0x7b), Color.Empty, true,
                0.2, 2.75, DashStyle.LongDash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat);

            Fill fill2 = chart2.Title.Format.Fill;
            Stroke stroke2 = chart2.Title.Format.Stroke;
            Fill axisFill2 = chart2.AxisX.Title.Format.Fill;
            Stroke axisStroke2 = chart2.AxisX.Title.Format.Stroke;

            Shape shape3 = doc.FirstSection.Body.Shapes[2];
            Chart chart3 = shape3.Chart;

            Paragraph paragraph3 = (Paragraph)shape3.GetAncestor(NodeType.Paragraph);
            Paragraph paragraph4 = (Paragraph)paragraph3.Clone(true);
            paragraph3.ParentNode.InsertAfter(paragraph4, paragraph3);
            paragraph3.ParentNode.InsertAfter(new Paragraph(doc), paragraph3);
            Shape shape4 = doc.FirstSection.Body.Shapes[3];
            Chart chart4 = shape4.Chart;

            Fill fill3 = chart3.Title.Format.Fill;
            CheckFill(fill3, FillType.Gradient, Color.FromArgb(0xf7, 0xca, 0xac), Color.FromArgb(0x9d, 0x48, 0x0d),
                true, 0.4, true, false);

            Stroke stroke3 = chart3.Title.Format.Stroke;
            CheckStroke(stroke3, Color.FromArgb(0x83, 0x3c, 0x0b), Color.Empty, true,
                0.25, 3.75, DashStyle.Dash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat);

            Fill axisFill3 = chart3.AxisX.Title.Format.Fill;
            CheckFill(axisFill3, FillType.Gradient, Color.FromArgb(0xff, 0xff, 0xff), Color.FromArgb(0x5a, 0x9a, 0xd5),
                true, 0.4, true, false);

            Stroke axisStroke3 = chart3.AxisX.Title.Format.Stroke;
            CheckStroke(axisStroke3, Color.FromArgb(0x52, 0x52, 0x52), Color.Empty, true,
                0.3, 3.25, DashStyle.LongDashDot, ShapeLineStyle.ThinThick, JoinStyle.Round, EndCap.Flat);

            Fill fill4 = chart4.Title.Format.Fill;
            Stroke stroke4 = chart4.Title.Format.Stroke;
            Fill axisFill4 = chart4.AxisX.Title.Format.Fill;
            Stroke axisStroke4 = chart4.AxisX.Title.Format.Stroke;

            // Change format.

            fill1.ForeColor = Color.DarkBlue;
            fill1.BackColor = Color.DarkGreen;
            fill1.Opacity = 0.35;
            stroke1.Fill.OneColorGradient(Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1, 0.3);
            stroke1.Weight = 2;
            stroke1.DashStyle = DashStyle.ShortDot;
            stroke1.LineStyle = ShapeLineStyle.Single;

            CheckFill(fill1, FillType.Gradient, Color.FromArgb(0, 0, 0x8b), Color.FromArgb(0, 0x64, 0),
                true, 0.65, true, false);
            CheckStroke(stroke1, Color.FromArgb(0, 0, 0xff), Color.FromArgb(0, 0, 0x99), true,
                0, 2, DashStyle.ShortDot, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            axisFill1.Solid(Color.Cyan);
            axisFill1.Opacity = 0.35;
            axisStroke1.Color = Color.Blue;
            axisStroke1.Weight = 1.5;
            axisStroke1.LineStyle = ShapeLineStyle.Single;

            CheckFill(axisFill1, FillType.Solid, Color.FromArgb(0, 0xff, 0xff), Color.Empty,
                true, 0.65, false, false);
            CheckStroke(axisStroke1, Color.FromArgb(0, 0, 0xff), Color.Empty, true,
                0, 1.5, DashStyle.LongDash, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill2.Visible = false;
            stroke2.Visible = false;
            axisFill2.Visible = false;
            axisStroke2.Visible = false;

            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 3.5, DashStyle.ShortDot, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);
            CheckFill(axisFill2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(axisStroke2, Color.Empty, Color.Empty, false,
                1, 2.75, DashStyle.LongDash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat);

            fill3.ForeColor = Color.DarkBlue;
            fill3.BackColor = Color.DarkGreen;
            fill3.Opacity = 0.35;
            stroke3.Fill.OneColorGradient(Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant1, 0.3);
            stroke3.Weight = 2;
            stroke3.DashStyle = DashStyle.ShortDot;
            stroke3.LineStyle = ShapeLineStyle.Single;

            CheckFill(fill3, FillType.Gradient, Color.FromArgb(0, 0, 0x8b), Color.FromArgb(0, 0x64, 0),
                true, 0.65, true, false);
            CheckStroke(stroke3, Color.FromArgb(0, 0, 0xff), Color.FromArgb(0, 0, 0x99), true,
                0, 2, DashStyle.ShortDot, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            axisFill3.Solid(Color.Cyan);
            axisFill3.Opacity = 0.35;
            axisStroke3.Color = Color.Blue;
            axisStroke3.Weight = 1.5;
            axisStroke3.LineStyle = ShapeLineStyle.Single;

            CheckFill(axisFill3, FillType.Solid, Color.FromArgb(0, 0xff, 0xff), Color.Empty,
                true, 0.65, false, false);
            CheckStroke(axisStroke3, Color.FromArgb(0, 0, 0xff), Color.Empty, true,
                0, 1.5, DashStyle.LongDashDot, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill4.Visible = false;
            stroke4.Visible = false;
            axisFill4.Visible = false;
            axisStroke4.Visible = false;

            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke4, Color.Empty, Color.Empty, false,
                1, 3.75, DashStyle.Dash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat);
            CheckFill(axisFill4, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(axisStroke4, Color.Empty, Color.Empty, false,
                1, 3.25, DashStyle.LongDashDot, ShapeLineStyle.ThinThick, JoinStyle.Round, EndCap.Flat);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestTitleFormat.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartTitle.Format"/> and <see cref="ChartAxisTitle.Format"/> properties of
        /// a created chart.
        /// </summary>
        [Test]
        public void TestTitleFormatInNewChart()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            Fill fill = chart.Title.Format.Fill;
            Fill axisFill = chart.AxisX.Title.Format.Fill;
            Stroke stroke = chart.Title.Format.Stroke;
            Stroke axisStroke = chart.AxisX.Title.Format.Stroke;

            CheckFill(fill, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckFill(axisFill, FillType.Solid, Color.Empty, Color.Empty, true, 0, false, false);
            CheckStroke(stroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);
            CheckStroke(axisStroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill.Solid();
            fill.ForeColor = Color.Cyan;
            fill.Transparency = 0.45;
            axisFill.TwoColorGradient(Color.Cyan, Color.Blue, GradientStyle.DiagonalUp, GradientVariant.Variant1);
            axisFill.Transparency = 0.45;

            stroke.Color = Color.Blue;
            stroke.Transparency = 0.35;
            stroke.Weight = 2.5;
            stroke.DashStyle = DashStyle.ShortDash;
            stroke.LineStyle = ShapeLineStyle.Double;
            axisStroke.Fill.TwoColorGradient(Color.DarkBlue, Color.Blue, GradientStyle.Vertical, GradientVariant.Variant2);
            axisStroke.Transparency = 0.15;
            axisStroke.Weight = 1.5;

            CheckFill(fill, FillType.Solid, Color.FromArgb(0, 0xff, 0xff),
                Color.Empty, true, 0.45, false, false);
            CheckFill(axisFill, FillType.Gradient, Color.FromArgb(0, 0xff, 0xff),
                Color.FromArgb(0, 0, 0xff), true, 0.45, true, false);
            CheckStroke(stroke, Color.FromArgb(0, 0, 0xff), Color.Empty, true,
                0.35, 2.5, DashStyle.ShortDash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat);
            CheckStroke(axisStroke, Color.FromArgb(0, 0, 0x8b), Color.FromArgb(0, 0, 0xff), true,
                0.15, 1.5, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestTitleFormatInNewChart.docx");
        }

        /// <summary>
        /// Tests applying a patterned fill to chart and axis titles.
        /// </summary>
        [Test]
        public void TestTitlePatternedFill()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Title.Format.Fill;
            Fill axisFill = chart.AxisX.Title.Format.Fill;

            fill.Patterned(PatternType.HorizontalBrick);
            fill.ForeColor = Color.Red;
            fill.BackColor = Color.Yellow;
            fill.Transparency = 0.8;

            axisFill.Patterned(PatternType.DottedGrid);
            axisFill.ForeColor = Color.DarkBlue;
            axisFill.BackColor = Color.LightGreen;
            axisFill.Transparency = 0.1;

            CheckPatternedFill(fill, PatternType.HorizontalBrick, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.8);
            CheckPatternedFill(axisFill, PatternType.DottedGrid, Color.FromArgb(0, 0, 0x8b),
                Color.FromArgb(0x90, 0xee, 0x90), 0.1);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTitlePatternedFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Title.Format.Fill;
            axisFill = chart.AxisX.Title.Format.Fill;

            CheckPatternedFill(fill, PatternType.HorizontalBrick, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.8);
            CheckPatternedFill(axisFill, PatternType.DottedGrid, Color.FromArgb(0, 0, 0x8b),
                Color.FromArgb(0x90, 0xee, 0x90), 0.1);
        }

        /// <summary>
        /// Tests applying a one-color gradient fill to chart and axis titles.
        /// </summary>
        [Test]
        public void TestTitleOneColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Title.Format.Fill;
            Fill axisFill = chart.AxisX.Title.Format.Fill;
            IThemeProvider themeProvider = doc.Theme;

            fill.OneColorGradient(Color.Yellow, GradientStyle.Horizontal, GradientVariant.Variant2, 0.25);
            fill.Transparency = 0.6;
            axisFill.OneColorGradient(Color.Cyan, GradientStyle.Vertical, GradientVariant.Variant2, 0.35);
            axisFill.Transparency = 0.4;

            CheckGradientFill(fill, GradientStyle.Horizontal, GradientVariant.Variant2, Color.FromArgb(0xff, 0xff, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.6, 270, 0, 0.5, themeProvider);
            CheckGradientFill(axisFill, GradientStyle.Vertical, GradientVariant.Variant2, Color.FromArgb(0, 0xff, 0xff),
                Color.FromArgb(0, 0xff, 0xff), 0.4, 180, 0, 0.7, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTitleOneColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Title.Format.Fill;
            axisFill = chart.AxisX.Title.Format.Fill;
            themeProvider = doc.Theme;

            CheckGradientFill(fill, GradientStyle.Horizontal, GradientVariant.Variant2, Color.FromArgb(0xff, 0xff, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.6, 270, 0, 0.5, themeProvider);
            CheckGradientFill(axisFill, GradientStyle.Vertical, GradientVariant.Variant2, Color.FromArgb(0, 0xff, 0xff),
                Color.FromArgb(0, 0xff, 0xff), 0.4, 180, 0, 0.7, themeProvider);
        }

        /// <summary>
        /// Tests applying a two-color gradient fill to chart and axis titles.
        /// </summary>
        [Test]
        public void TestTitleTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Title.Format.Fill;
            Fill axisFill = chart.AxisX.Title.Format.Fill;
            IThemeProvider themeProvider = doc.Theme;

            fill.TwoColorGradient(Color.Red, Color.Yellow, GradientStyle.Vertical, GradientVariant.Variant1);
            fill.Transparency = 0.7;
            axisFill.TwoColorGradient(Color.Blue, Color.Green, GradientStyle.Vertical, GradientVariant.Variant1);
            axisFill.Transparency = 0.65;

            CheckGradientFill(fill, GradientStyle.Vertical, GradientVariant.Variant1, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.7, 0, 0, 0, themeProvider);
            CheckGradientFill(axisFill, GradientStyle.Vertical, GradientVariant.Variant1, Color.FromArgb(0, 0, 0xff),
                Color.FromArgb(0, 0x80, 0), 0.65, 0, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestTitleTwoColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Title.Format.Fill;
            axisFill = chart.AxisX.Title.Format.Fill;
            themeProvider = doc.Theme;

            CheckGradientFill(fill, GradientStyle.Vertical, GradientVariant.Variant1, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.7, 0, 0, 0, themeProvider);
            CheckGradientFill(axisFill, GradientStyle.Vertical, GradientVariant.Variant1, Color.FromArgb(0, 0, 0xff),
                Color.FromArgb(0, 0x80, 0), 0.65, 0, 0, 0, themeProvider);
        }


        /// <summary>
        /// Tests the <see cref="ChartLegend.Format"/> property of an existing chart.
        /// </summary>
        [Test]
        public void TestChartLegendFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestChartLegendFormat.docx");
            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            Chart chart1 = shape1.Chart;

            Paragraph paragraph1 = (Paragraph)shape1.GetAncestor(NodeType.Paragraph);
            Paragraph paragraph2 = (Paragraph)paragraph1.Clone(true);
            paragraph1.ParentNode.InsertAfter(paragraph2, paragraph1);
            paragraph1.ParentNode.InsertAfter(new Paragraph(doc), paragraph1);
            Shape shape2 = doc.FirstSection.Body.Shapes[1];
            Chart chart2 = shape2.Chart;

            Fill fill1 = chart1.Legend.Format.Fill;
            CheckFill(fill1, FillType.Solid, Color.FromArgb(0xb3, 0xc6, 0xe7), Color.Empty,
                true, 0, false, false);

            Stroke stroke1 = chart1.Legend.Format.Stroke;
            CheckStroke(stroke1, Color.FromArgb(0x2a, 0x4b, 0x86), Color.FromArgb(0x8d, 0xa9, 0xdb), true,
                0, 3.5, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            Fill fill2 = chart2.Legend.Format.Fill;
            Stroke stroke2 = chart2.Legend.Format.Stroke;

            Shape shape3 = doc.FirstSection.Body.Shapes[2];
            Chart chart3 = shape3.Chart;

            Paragraph paragraph3 = (Paragraph)shape3.GetAncestor(NodeType.Paragraph);
            Paragraph paragraph4 = (Paragraph)paragraph3.Clone(true);
            paragraph3.ParentNode.InsertAfter(paragraph4, paragraph3);
            paragraph3.ParentNode.InsertAfter(new Paragraph(doc), paragraph3);
            Shape shape4 = doc.FirstSection.Body.Shapes[3];
            Chart chart4 = shape4.Chart;

            Fill fill3 = chart3.Legend.Format.Fill;
            CheckFill(fill3, FillType.Solid, Color.FromArgb(0xf7, 0xca, 0xac), Color.Empty,
                true, 0.25, false, false);

            Stroke stroke3 = chart3.Legend.Format.Stroke;
            CheckStroke(stroke3, Color.FromArgb(0xb0, 0x50, 0x0f), Color.FromArgb(0xf4, 0xb0, 0x81), true,
                0, 3.75, DashStyle.Solid, ShapeLineStyle.ThinThick, JoinStyle.Round, EndCap.Flat);

            Fill fill4 = chart4.Legend.Format.Fill;
            Stroke stroke4 = chart4.Legend.Format.Stroke;

            // Change format.

            fill1.TwoColorGradient(Color.Yellow, Color.LightBlue, GradientStyle.FromCenter, GradientVariant.Variant1);
            fill1.Opacity = 0.75;
            stroke1.Fill.Solid(Color.DarkBlue);
            stroke1.Weight = 1.75;
            stroke1.DashStyle = DashStyle.ShortDot;
            stroke1.LineStyle = ShapeLineStyle.Single;
            stroke1.Opacity = 0.7;

            CheckFill(fill1, FillType.Gradient, Color.FromArgb(0xff, 0xff, 0), Color.FromArgb(0xad, 0xd8, 0xe6),
                true, 0.25, true, false);
            CheckStroke(stroke1, Color.FromArgb(0, 0, 0x8b), Color.Empty, true,
                0.3, 1.75, DashStyle.ShortDot, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill2.Visible = false;
            stroke2.Visible = false;

            CheckFill(fill2, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke2, Color.Empty, Color.Empty, false,
                1, 3.5, DashStyle.Solid, ShapeLineStyle.ThickThin, JoinStyle.Round, EndCap.Flat);

            fill3.TwoColorGradient(Color.Yellow, Color.LightBlue, GradientStyle.FromCenter, GradientVariant.Variant1);
            fill3.Opacity = 0.75;
            stroke3.Fill.Solid(Color.DarkBlue);
            stroke3.Weight = 1.75;
            stroke3.DashStyle = DashStyle.ShortDot;
            stroke3.LineStyle = ShapeLineStyle.Single;
            stroke3.Opacity = 0.7;

            CheckFill(fill3, FillType.Gradient, Color.FromArgb(0xff, 0xff, 0), Color.FromArgb(0xad, 0xd8, 0xe6),
                true, 0.25, true, false);
            CheckStroke(stroke3, Color.FromArgb(0, 0, 0x8b), Color.Empty, true,
                0.3, 1.75, DashStyle.ShortDot, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill4.Visible = false;
            stroke4.Visible = false;

            CheckFill(fill4, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke4, Color.Empty, Color.Empty, false,
                1, 3.75, DashStyle.Solid, ShapeLineStyle.ThinThick, JoinStyle.Round, EndCap.Flat);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestChartLegendFormat.docx");
        }

        /// <summary>
        /// Tests the <see cref="ChartLegend.Format"/> property of a created chart.
        /// </summary>
        [Test]
        public void TestChartLegendFormatInNewChart()
        {
            Document doc = CreateDocumentWithColumnChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            Fill fill = chart.Legend.Format.Fill;
            Stroke stroke = chart.Legend.Format.Stroke;

            CheckFill(fill, FillType.Solid, Color.Empty, Color.Empty, false, 1, false, false);
            CheckStroke(stroke, Color.Empty, Color.Empty, false,
                1, 0, DashStyle.Solid, ShapeLineStyle.Single, JoinStyle.Round, EndCap.Flat);

            fill.ForeColor = Color.Cyan;
            fill.Transparency = 0.45;

            stroke.Color = Color.Blue;
            stroke.Transparency = 0.35;
            stroke.Weight = 2.5;
            stroke.DashStyle = DashStyle.ShortDash;
            stroke.LineStyle = ShapeLineStyle.Double;

            CheckFill(fill, FillType.Solid, Color.FromArgb(0, 0xff, 0xff),
                Color.Empty, true, 0.45, false, false);
            CheckStroke(stroke, Color.FromArgb(0, 0, 0xff), Color.Empty, true,
                0.35, 2.5, DashStyle.ShortDash, ShapeLineStyle.Double, JoinStyle.Round, EndCap.Flat);

            TestUtil.SaveCheckGold(doc, @"Model\Charts\TestChartLegendFormatInNewChart.docx");
        }

        /// <summary>
        /// Tests applying a patterned fill to a chart legend.
        /// </summary>
        [Test]
        public void TestChartLegendPatternedFill()
        {
            Document doc = CreateDocumentWithColumnChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Legend.Format.Fill;

            fill.Patterned(PatternType.DiagonalBrick);
            fill.ForeColor = Color.Red;
            fill.BackColor = Color.Yellow;
            fill.Transparency = 0.75;

            CheckPatternedFill(fill, PatternType.DiagonalBrick, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.75);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestChartLegendPatternedFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Legend.Format.Fill;

            CheckPatternedFill(fill, PatternType.DiagonalBrick, Color.FromArgb(0xff, 0, 0),
                Color.FromArgb(0xff, 0xff, 0), 0.75);
        }

        /// <summary>
        /// Tests applying a one-color gradient fill to a chart legend.
        /// </summary>
        [Test]
        public void TestChartLegendOneColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Legend.Format.Fill;
            IThemeProvider themeProvider = doc.Theme;

            fill.OneColorGradient(Color.LightGreen, GradientStyle.Horizontal, GradientVariant.Variant2, 0.25);
            fill.Transparency = 0.25;

            CheckGradientFill(fill, GradientStyle.Horizontal, GradientVariant.Variant2, Color.FromArgb(0x90, 0xee, 0x90),
                Color.FromArgb(0x90, 0xee, 0x90), 0.25, 270, 0, 0.5, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestChartLegendOneColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Legend.Format.Fill;
            themeProvider = doc.Theme;

            CheckGradientFill(fill, GradientStyle.Horizontal, GradientVariant.Variant2, Color.FromArgb(0x90, 0xee, 0x90),
                Color.FromArgb(0x90, 0xee, 0x90), 0.25, 270, 0, 0.5, themeProvider);
        }

        /// <summary>
        /// Tests applying a two-color gradient fill to a chart legend.
        /// </summary>
        [Test]
        public void TestChartLegendTwoColorGradientFill()
        {
            Document doc = CreateDocumentWithColumnChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            Fill fill = chart.Legend.Format.Fill;
            IThemeProvider themeProvider = doc.Theme;

            fill.TwoColorGradient(Color.Blue, Color.Yellow, GradientStyle.Vertical, GradientVariant.Variant2);
            fill.Transparency = 0.7;

            CheckGradientFill(fill, GradientStyle.Vertical, GradientVariant.Variant2, Color.FromArgb(0, 0, 0xff),
                Color.FromArgb(0xff, 0xff, 0), 0.7, 180, 0, 0, themeProvider);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestChartLegendTwoColorGradientFill.docx", null, true);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            fill = chart.Legend.Format.Fill;
            themeProvider = doc.Theme;

            CheckGradientFill(fill, GradientStyle.Vertical, GradientVariant.Variant2, Color.FromArgb(0, 0, 0xff),
                Color.FromArgb(0xff, 0xff, 0), 0.7, 180, 0, 0, themeProvider);
        }


        /// <summary>
        /// Tests creating a chart with a dark background and light font color.
        /// </summary>
        [Test]
        public void TestCreatingDarkChart()
        {
            Document doc = CreateDocumentWithColumnChart(true);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            chart.Format.Fill.Solid(Color.DarkBlue);
            chart.Title.Font.Color = Color.Yellow;
            chart.Series[0].DataLabels.Font.Color = Color.Yellow;
            chart.DataTable.Font.Color = Color.Yellow;
            chart.AxisX.Title.Font.Color = Color.Yellow;
            chart.AxisY.TickLabels.Font.Color = Color.Yellow;
            chart.Legend.Font.Color = Color.Yellow;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestCreatingDarkChart.docx");
        }

        /// <summary>
        /// Checks <see cref="ChartFormat.ShapeType"/> property of data labels of the document being tested in
        /// the <see cref="TestChartShapeTypeApplying"/> method.
        /// </summary>
        private static void CheckChartShapeTypeApplying(Document doc)
        {
            ChartSeriesCollection seriesCollection = doc.FirstSection.Body.Shapes[0].Chart.Series;
            ChartDataLabelCollection dataLabels1 = seriesCollection[0].DataLabels;

            Assert.That(dataLabels1.Format.ShapeType, Is.EqualTo(ChartShapeType.WedgeRectCallout));
            Assert.That(dataLabels1[0].Format.ShapeType, Is.EqualTo(ChartShapeType.WedgeRectCallout));
            Assert.That(dataLabels1[1].Format.ShapeType, Is.EqualTo(ChartShapeType.UpArrowCallout));
            Assert.That(dataLabels1[2].Format.ShapeType, Is.EqualTo(ChartShapeType.Default));
            Assert.That(dataLabels1[3].Format.ShapeType, Is.EqualTo(ChartShapeType.Default));

            ChartDataLabelCollection dataLabels2 = seriesCollection[1].DataLabels;

            Assert.That(dataLabels2.Format.ShapeType, Is.EqualTo(ChartShapeType.DownArrowCallout));
            Assert.That(dataLabels2[0].Format.ShapeType, Is.EqualTo(ChartShapeType.DownArrowCallout));
            Assert.That(dataLabels2[1].Format.ShapeType, Is.EqualTo(ChartShapeType.DownArrowCallout));
            Assert.That(dataLabels2[2].Format.ShapeType, Is.EqualTo(ChartShapeType.DownArrowCallout));
            Assert.That(dataLabels2[3].Format.ShapeType, Is.EqualTo(ChartShapeType.DownArrowCallout));
        }

        /// <summary>
        /// Checks properties of an image <see cref="Fill"/> instance.
        /// </summary>
        private static void CheckImageFill(Fill fill, double expectedTransparency, int expectedImageLength)
        {
            Assert.That(fill.FillType, Is.EqualTo(FillType.Picture));
            Assert.That(fill.Visible, Is.EqualTo(true));
            Assert.That(fill.Transparency, Is.EqualTo(expectedTransparency).Within(0.005));
            Assert.That(fill.Opacity, Is.EqualTo(1 - expectedTransparency).Within(0.005));
            Assert.That(fill.RotateWithObject, Is.EqualTo(true));
            Assert.That(fill.ImageBytes, IsNot.Null());
            Assert.That(fill.ImageBytes.Length, Is.EqualTo(expectedImageLength));
        }

        /// <summary>
        /// Checks properties of a gradient <see cref="Fill"/> instance.
        /// </summary>
        private static void CheckGradientFill(Fill fill, GradientStyle expectedStyle, GradientVariant expectedVariant,
            Color expectedColor, double expectedTransparency, double expectedGradientAngle, double expectedTint,
            double expectedShade, IThemeProvider themeProvider)
        {
            CheckGradientFill(fill, expectedStyle, expectedVariant, expectedColor, expectedColor, expectedTransparency,
                expectedGradientAngle, expectedTint, expectedShade, themeProvider);
        }

        /// <summary>
        /// Checks properties of a gradient <see cref="Fill"/> instance.
        /// </summary>
        /// <dev>
        /// Not all combinations of <paramref name="expectedStyle"/> and <paramref name="expectedVariant"/> are
        /// currently supported.
        /// </dev>
        private static void CheckGradientFill(Fill fill, GradientStyle expectedStyle, GradientVariant expectedVariant,
            Color expectedForeColor, Color expectedBackColor, double expectedTransparency, double expectedGradientAngle,
            double expectedTint, double expectedShade, IThemeProvider themeProvider)
        {
            // BackColor is checked below.
            CheckFill(fill, FillType.Gradient, expectedForeColor, fill.BackColor, true,
                expectedTransparency, true, false);

            Assert.That(fill.GradientStyle, Is.EqualTo(expectedStyle));
            Assert.That(fill.GradientVariant, Is.EqualTo(expectedVariant));
            Assert.That(fill.GradientAngle, Is.EqualTo(expectedGradientAngle));

            bool isVariant3Or4 =
                ((expectedVariant == GradientVariant.Variant3) || (expectedVariant == GradientVariant.Variant4));
            int expectedGradientStopCount = isVariant3Or4 ? 3 : 2;
            Assert.That(fill.GradientStops.Count, Is.EqualTo(expectedGradientStopCount));

            GradientStop firstGradientStop = fill.GradientStops[0];
            GradientStop middleGradientStop = fill.GradientStops[1];
            GradientStop lastGradientStop = fill.GradientStops[expectedGradientStopCount - 1];

            switch (expectedVariant)
            {
                case GradientVariant.Variant3:
                    CheckGradientStop(firstGradientStop, expectedForeColor, 0, 0, 0, themeProvider);
                    CheckGradientStop(middleGradientStop, expectedBackColor, 0.5, expectedTint, expectedShade, themeProvider);
                    CheckGradientStop(lastGradientStop, expectedForeColor, 1, 0, 0, themeProvider);
                    break;
                case GradientVariant.Variant4:
                    CheckGradientStop(firstGradientStop, expectedForeColor, 0.5, 0, 0, themeProvider);
                    CheckGradientStop(middleGradientStop, expectedBackColor, 0, expectedTint, expectedShade, themeProvider);
                    CheckGradientStop(lastGradientStop, expectedBackColor, 1, expectedTint, expectedShade, themeProvider);
                    break;
                default:
                    CheckGradientStop(firstGradientStop, expectedForeColor, 0, 0, 0, themeProvider);
                    CheckGradientStop(lastGradientStop, expectedBackColor, 1, expectedTint, expectedShade, themeProvider);
                    break;
            }

        }

        private static void CheckGradientStop(GradientStop gradientStop, Color expectedColor,
            double expectedPosition, double expectedTint, double expectedShade, IThemeProvider themeProvider)
        {
            Assert.That(gradientStop.Position, Is.EqualTo(expectedPosition).Within(0.01));

            DmlColor color = gradientStop.DmlGradientStop.Color;
            Assert.That(color, IsNot.Null());
            if (expectedColor != Color.Empty)
                Assert.That(color.CreateUnmodifiedDrColor(themeProvider).ToNativeColor(), Is.EqualTo(expectedColor));

            DmlTint tintModifier = (DmlTint)color.GetColorModifier(DmlColorModifierType.Tint);
            if (MathUtil.AreEqual(expectedTint, 0))
            {
                Assert.That(tintModifier, Is.Null);
            }
            else
            {
                Assert.That(tintModifier, IsNot.Null());
                Assert.That(tintModifier.Value, Is.EqualTo(expectedTint).Within(0.01));
            }

            DmlShade shadeModifier = (DmlShade)color.GetColorModifier(DmlColorModifierType.Shade);
            if (MathUtil.AreEqual(expectedShade, 0))
            {
                Assert.That(shadeModifier, Is.Null);
            }
            else
            {
                Assert.That(shadeModifier, IsNot.Null());
                Assert.That(shadeModifier.Value, Is.EqualTo(expectedShade).Within(0.01));
            }
        }

        /// <summary>
        /// Checks properties of a patterned <see cref="Fill"/> instance.
        /// </summary>
        private static void CheckPatternedFill(Fill fill, PatternType expectedPatternType, Color expectedForeColor,
            Color expectedBackColor, double expectedTransparency)
        {
            CheckFill(fill, FillType.Patterned, expectedForeColor, expectedBackColor, true,
                expectedTransparency, false, false);

            Assert.That(fill.Pattern, Is.EqualTo(expectedPatternType));
        }

        /// <summary>
        /// Checks properties of a textured <see cref="Fill"/> instance.
        /// </summary>
        private static void CheckTexturedFill(Fill fill, PresetTexture expectedTexture, Color expectedForeColor,
            Color expectedBackColor, bool expectedVisible, double expectedTransparency)
        {
            CheckFill(fill, FillType.Textured, expectedForeColor, expectedBackColor, expectedVisible,
                expectedTransparency, true, true);

            Assert.That(fill.PresetTexture, Is.EqualTo(expectedTexture));
        }

        /// <summary>
        /// Checks properties of a <see cref="Fill"/> instance.
        /// </summary>
        private static void CheckFill(Fill fill, FillType expectedFillType, Color expectedForeColor,
            Color expectedBackColor, bool expectedVisible, double expectedTransparency, bool expectedRotateWithObject,
            bool expectedHavingImage)
        {
            Assert.That(fill.FillType, Is.EqualTo(expectedFillType));
            Assert.That(fill.ForeColor, Is.EqualTo(expectedForeColor));
            Assert.That(fill.Color, Is.EqualTo(expectedForeColor));
            Assert.That(fill.BackColor, Is.EqualTo(expectedBackColor));
            Assert.That(fill.Visible, Is.EqualTo(expectedVisible));
            Assert.That(fill.Transparency, Is.EqualTo(expectedTransparency).Within(0.005));
            Assert.That(fill.Opacity, Is.EqualTo(1 - expectedTransparency).Within(0.005));
            Assert.That(fill.RotateWithObject, Is.EqualTo(expectedRotateWithObject));
            Assert.That(fill.ImageBytes != null, Is.EqualTo(expectedHavingImage));
        }

        /// <summary>
        /// Checks properties of a <see cref="Stroke"/> instance.
        /// </summary>
        private static void CheckStroke(Stroke stroke, Color expectedForeColor, Color expectedBackColor,
            bool expectedVisible, double expectedTransparency, double expectedWeight, DashStyle expectedDashStyle,
            ShapeLineStyle expectedLineStyle, JoinStyle expectedJoinStyle, EndCap expectedEndCap)
        {
            CheckStroke(stroke, expectedForeColor, expectedBackColor, expectedVisible, expectedTransparency, expectedWeight,
                expectedDashStyle, expectedLineStyle, expectedJoinStyle, expectedEndCap, ArrowType.None, ArrowType.None,
                ArrowWidth.Medium, ArrowLength.Medium, ArrowWidth.Medium, ArrowLength.Medium, false);
        }

        /// <summary>
        /// Checks properties of a <see cref="Stroke"/> instance.
        /// </summary>
        private static void CheckStroke(Stroke stroke, Color expectedForeColor, Color expectedBackColor,
            bool expectedVisible, double expectedTransparency, double expectedWeight, DashStyle expectedDashStyle,
            ShapeLineStyle expectedLineStyle, JoinStyle expectedJoinStyle, EndCap expectedEndCap,
            ArrowType expectedStartArrowType, ArrowType expectedEndArrowType, ArrowWidth expectedStartArrowWidth,
            ArrowLength expectedStartArrowLength, ArrowWidth expectedEndArrowWidth, ArrowLength expectedEndArrowLength,
            bool expectedHavingImage)
        {
            Assert.That(stroke.ForeColor, Is.EqualTo(expectedForeColor));
            Assert.That(stroke.Color, Is.EqualTo(expectedForeColor));
            Assert.That(stroke.BackColor, Is.EqualTo(expectedBackColor));
            Assert.That(stroke.Color2, Is.EqualTo(expectedBackColor));
            Assert.That(stroke.Visible, Is.EqualTo(expectedVisible));
            Assert.That(stroke.On, Is.EqualTo(expectedVisible));
            Assert.That(stroke.Transparency, Is.EqualTo(expectedTransparency).Within(0.005));
            Assert.That(stroke.Opacity, Is.EqualTo(1 - expectedTransparency).Within(0.005));
            Assert.That(stroke.Weight, Is.EqualTo(expectedWeight).Within(0.005));
            Assert.That(stroke.DashStyle, Is.EqualTo(expectedDashStyle));
            Assert.That(stroke.LineStyle, Is.EqualTo(expectedLineStyle));
            Assert.That(stroke.JoinStyle, Is.EqualTo(expectedJoinStyle));
            Assert.That(stroke.EndCap, Is.EqualTo(expectedEndCap));
            Assert.That(stroke.StartArrowType, Is.EqualTo(expectedStartArrowType));
            Assert.That(stroke.EndArrowType, Is.EqualTo(expectedEndArrowType));
            Assert.That(stroke.StartArrowWidth, Is.EqualTo(expectedStartArrowWidth));
            Assert.That(stroke.StartArrowLength, Is.EqualTo(expectedStartArrowLength));
            Assert.That(stroke.EndArrowWidth, Is.EqualTo(expectedEndArrowWidth));
            Assert.That(stroke.EndArrowLength, Is.EqualTo(expectedEndArrowLength));
            Assert.That(stroke.ImageBytes != null, Is.EqualTo(expectedHavingImage));
        }

        /// <summary>
        /// Creates a document that contains a column chart.
        /// </summary>
        private static Document CreateDocumentWithColumnChart()
        {
            return CreateDocumentWithColumnChart(false);
        }

        /// <summary>
        /// Creates a document that contains a column chart with ability to show data labels, data table, axis title.
        /// </summary>
        private static Document CreateDocumentWithColumnChart(bool showAllChartSubElements)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;
            ChartSeriesCollection series = chart.Series;

            // Delete default generated series.
            series.Clear();

            string[] categories = new string[] { "Category 1", "Category 2" };

            series.Add("Series 1", categories, new double[] { 1, 2 });
            series.Add("Series 2", categories, new double[] { 3, 4 });
            series.Add("Series 3", categories, new double[] { 5, 6 });
            series.Add("Series 4", categories, new double[] { 7, 8 });
            series.Add("Series 5", categories, new double[] { 9, 10 });

            if (showAllChartSubElements)
            {
                series[0].HasDataLabels = true;
                series[0].DataLabels.ShowValue = true;
                chart.DataTable.Show = true;
                chart.AxisX.Title.Show = true;
            }

            return doc;
        }

        /// <summary>
        /// Creates a document that contains a scatter chart including one series.
        /// </summary>
        private static Document CreateDocumentWithOneSeriesScatterChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Scatter, 432, 252);
            Chart chart = shape.Chart;
            ChartSeriesCollection series = chart.Series;

            // Delete default generated series.
            series.Clear();

            series.Add("AW Series 1", new double[] { 0.7, 1.8, 2.6, 3.4, 4.3, 5, 5.8 },
                new double[] { 2.7, 3.2, 0.8, 1.5, 4.4, 2, 3 });

            return doc;
        }

        /// <summary>
        /// Creates a document that contains a scatter chart including two series.
        /// </summary>
        private static Document CreateDocumentWithTwoSeriesScatterChart()
        {
            Document doc = CreateDocumentWithOneSeriesScatterChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            chart.Series.Add("AW Series 2", new double[] { 1.2, 2.2, 3.2, 4.2, 5.2, 6.2, 7.2 },
                new double[] { 0.7, 6.4, 7.1, 1.9, 3.9, 7.6, 4 });

            return doc;
        }

        /// <summary>
        /// Creates a document that contains a pie chart.
        /// </summary>
        private static Document CreateDocumentWithPieChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Pie, 432, 252);
            Chart chart = shape.Chart;
            ChartSeriesCollection series = chart.Series;

            // Delete default generated series.
            series.Clear();

            string[] categories = new string[] { "Grapefruit", "Oranges", "Plums", "Pears", "Apples", "Kiwi", "Mango" };

            series.Add("Series 1", categories, new double[] { 2.7, 3.2, 0.8, 1.5, 4.4, 2, 3 });

            return doc;
        }

        /// <summary>
        /// Shows data labels of the series with setting their font size to 32.
        /// </summary>
        private static ChartDataLabelCollection ShowDataLabels(ChartSeries series)
        {
            series.HasDataLabels = true;
            ChartDataLabelCollection dataLabels = series.DataLabels;
            dataLabels.ShowValue = true;
            dataLabels.Font.Size = 32;
            return dataLabels;
        }
    }
}
