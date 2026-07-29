// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2022 by Alexander Zhiltsov

using System;
using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests members of the <see cref="ChartLegend"/> class.
    /// </summary>
    [TestFixture]
    public class TestChartLegend
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
        /// WORDSNET-21265 /chart/ Add feature to show/hide items in the chart's legend.
        /// </summary>
        [Test]
        public void Test23210()
        {
            const string fileName = @"Model\Charts\Test23210.docx";
            Document doc = TestUtil.Open(fileName);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartLegendEntryCollection legendEntries = chart.Legend.LegendEntries;
            ChartSeriesCollection series = chart.Series;

            Assert.That(legendEntries.Count, Is.EqualTo(8));

            Assert.That(series[0].LegendEntry, Is.SameAs(legendEntries[0]));
            Assert.That(series[1].LegendEntry, Is.SameAs(legendEntries[1]));
            Assert.That(series[2].LegendEntry, Is.SameAs(legendEntries[2]));
            Assert.That(series[3].LegendEntry, Is.SameAs(legendEntries[3]));

            Assert.That(legendEntries[0].IsHidden, Is.False);
            Assert.That(legendEntries[6].IsHidden, Is.False);

            legendEntries[0].IsHidden = true;
            legendEntries[6].IsHidden = true;

            doc = TestUtil.SaveOpen(doc, fileName, null, false);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            legendEntries = chart.Legend.LegendEntries;

            Assert.That(chart.Legend.LegendEntries.Count, Is.EqualTo(8));
            Assert.That(legendEntries[0].IsHidden, Is.True);
            Assert.That(legendEntries[6].IsHidden, Is.True);
        }

        /// <summary>
        /// WORDSNET-23353 Legend entry not removed when deleting chart series
        /// </summary>
        [Test]
        public void Test23353()
        {
            const string fileName = @"Model\Charts\Test23210.docx";
            Document doc = TestUtil.Open(fileName);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartLegendEntryCollection legendEntries = chart.Legend.LegendEntries;

            Assert.That(legendEntries.Count, Is.EqualTo(8));

            Font font0 = legendEntries[0].Font;
            Font font2 = legendEntries[2].Font;
            Font font7 = legendEntries[7].Font;
            CheckFont(font0, "Calibri", 8, Color.Empty, false, false, Underline.None);
            CheckFont(font2, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font7, "Calibri", 14, Color.Empty, false, false, Underline.None);

            chart.Series.RemoveAt(1);

            // The original document contained 4 series and 4 trendlines. One series and one trendline has been removed.
            Assert.That(legendEntries.Count, Is.EqualTo(6));

            font0 = legendEntries[0].Font;
            Font font1 = legendEntries[1].Font;
            Font font5 = legendEntries[5].Font;
            CheckFont(font0, "Calibri", 8, Color.Empty, false, false, Underline.None);
            CheckFont(font1, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font5, "Calibri", 14, Color.Empty, false, false, Underline.None);

            doc = TestUtil.SaveOpen(doc, fileName, null, false);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            legendEntries = chart.Legend.LegendEntries;

            Assert.That(legendEntries.Count, Is.EqualTo(6));

            font0 = legendEntries[0].Font;
            font1 = legendEntries[1].Font;
            font5 = legendEntries[5].Font;
            CheckFont(font0, "Calibri", 8, Color.Empty, false, false, Underline.None);
            CheckFont(font1, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font5, "Calibri", 14, Color.Empty, false, false, Underline.None);

            chart.Series.RemoveAt(0);

            // One series and two trendlines has been removed.
            Assert.That(legendEntries.Count, Is.EqualTo(3));

            font0 = legendEntries[0].Font;
            font2 = legendEntries[2].Font;
            CheckFont(font0, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font2, "Calibri", 14, Color.Empty, false, false, Underline.None);

            chart.Series.Add(
                "Series 5",
                new string[] { "Category1", "Category2", "Category3", "Category4" },
                new double[] { 7, 6, 5, 4 });

            Assert.That(legendEntries.Count, Is.EqualTo(4));

            font0 = legendEntries[0].Font;
            font2 = legendEntries[2].Font; // Font of the added series.
            Font font3 = legendEntries[3].Font;
            CheckFont(font0, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font2, "Calibri", 10, Color.Empty, false, false, Underline.None);
            CheckFont(font3, "Calibri", 14, Color.Empty, false, false, Underline.None);
        }

        /// <summary>
        /// Tests the <see cref="ChartLegendEntry.Font"/> property.
        /// </summary>
        [Test]
        public void TestLegendEntryFont()
        {
            const string fileName = @"Model\Charts\Test23210.docx";
            Document doc = TestUtil.Open(fileName);
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartLegendEntryCollection legendEntries = chart.Legend.LegendEntries;

            Assert.That(legendEntries.Count, Is.EqualTo(8));

            Font font0 = legendEntries[0].Font;
            Font font2 = legendEntries[2].Font;
            Font font7 = legendEntries[7].Font;
            CheckFont(font0, "Calibri", 8, Color.Empty, false, false, Underline.None);
            CheckFont(font2, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font7, "Calibri", 14, Color.Empty, false, false, Underline.None);

            Assert.That(font2.Kerning, Is.EqualTo(12));
            Assert.That(font2.Spacing, Is.EqualTo(0));
            Assert.That(font2.NameBi, Is.EqualTo("Calibri Light"));
            Assert.That(font2.DoubleStrikeThrough, Is.EqualTo(false));
            Assert.That(font2.StrikeThrough, Is.EqualTo(false));
            Assert.That(font2.AllCaps, Is.EqualTo(false));
            Assert.That(font2.SmallCaps, Is.EqualTo(false));
            Assert.That(font2.VerticalAlignment, Is.EqualTo(RunVerticalAlignment.Baseline));
            Assert.That(font2.HighlightColor, Is.EqualTo(Color.Empty));
            Assert.That(font2.LocaleId, Is.EqualTo(1024));

            font2.Name = "Times New Roman";
            font2.NameBi = "Courier New";
            font2.NameFarEast = "Arial";
            font2.NameOther = "Wingdings";
            font2.Size = 14;
            font2.Color = Color.DarkRed;
            font2.HighlightColor = Color.Cyan;
            font2.Bold = false;
            font2.Italic = false;
            font2.Underline = Underline.None;
            font2.DoubleStrikeThrough = true;
            font2.AllCaps = true;
            font2.VerticalAlignment = RunVerticalAlignment.Superscript;
            font2.LocaleId = 1033;
            font2.Kerning = 10;
            font2.Spacing = 3;

            font7.Bold = true;
            font7.Italic = true;
            font7.StrikeThrough = true;
            font7.SmallCaps = true;

            doc = TestUtil.SaveOpen(doc, fileName, null, false);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            legendEntries = chart.Legend.LegendEntries;

            Assert.That(legendEntries.Count, Is.EqualTo(8));

            font2 = legendEntries[2].Font;
            font7 = legendEntries[7].Font;

            CheckFont(font2, "Times New Roman", 14, Color.FromArgb(0xFF, 0x8B, 0, 0), false, false, Underline.None);
            CheckFont(font7, "Calibri", 14, Color.Empty, true, true, Underline.None);

            Assert.That(font2.Kerning, Is.EqualTo(10));
            Assert.That(font2.Spacing, Is.EqualTo(3));
            Assert.That(font2.NameBi, Is.EqualTo("Courier New"));
            Assert.That(font2.NameFarEast, Is.EqualTo("Arial"));
            Assert.That(font2.NameOther, Is.EqualTo("Wingdings"));
            Assert.That(font2.DoubleStrikeThrough, Is.EqualTo(true));
            Assert.That(font2.StrikeThrough, Is.EqualTo(false));
            Assert.That(font2.AllCaps, Is.EqualTo(true));
            Assert.That(font2.SmallCaps, Is.EqualTo(false));
            Assert.That(font2.VerticalAlignment, Is.EqualTo(RunVerticalAlignment.Superscript));
            Assert.That(font2.HighlightColor, Is.EqualTo(Color.FromArgb(0xFF, 0, 0xFF, 0xFF)));
            Assert.That(font2.LocaleId, Is.EqualTo(1033));

            Assert.That(font7.DoubleStrikeThrough, Is.EqualTo(false));
            Assert.That(font7.StrikeThrough, Is.EqualTo(true));
            Assert.That(font7.AllCaps, Is.EqualTo(false));
            Assert.That(font7.SmallCaps, Is.EqualTo(true));
        }

        /// <summary>
        /// Tests getting inherited attributes of the <see cref="ChartLegendEntry.Font"/> property.
        /// </summary>
        [Test]
        public void TestLegendEntryFontInheritedProperties()
        {
            Document doc = TestUtil.Open(@"Model\Charts\Test23210.docx");
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartLegendEntryCollection legendEntries = chart.Legend.LegendEntries;

            chart.Series.Add(
                "Series 5",
                new string[] { "Category1", "Category2", "Category3", "Category4" },
                new double[] { 7, 6, 5, 4 });

            Assert.That(legendEntries.Count, Is.EqualTo(9));

            Font font0 = legendEntries[0].Font;
            Font font2 = legendEntries[2].Font;
            Font font4 = legendEntries[4].Font;
            Font font8 = legendEntries[8].Font;

            CheckFont(font0, "Calibri", 8, Color.Empty, false, false, Underline.None);
            CheckFont(font2, "Calibri Light", 10, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font4, "Calibri", 10, Color.Empty, false, false, Underline.None);
            CheckFont(font8, "Calibri", 14, Color.Empty, false, false, Underline.None);

            // Change chart space properties to check that if a property is not defined directly and is not defined in
            // legend, it is taken from chart space properties.

            DmlRunProperties chartSpaceProperties = chart.ChartSpace.TxPr.RunPr;
            chartSpaceProperties.LatinFont = new DmlFont();
            chartSpaceProperties.LatinFont.TextTypeface = "Times New Roman";
            chartSpaceProperties.FontSize = new DmlTextPoints(1200);
            chartSpaceProperties.Italics = true;
            chartSpaceProperties.Underline = Underline.Double;
            chartSpaceProperties.Fill = new DmlSolidFill(DmlColor.CreateFromArgb(1, 0, 0, 0x8B));
            chart.ChartSpace.TxPr.FirstParagraph.Properties.HasDefaultRunProperties = true;

            CheckFont(font0, "Times New Roman", 8, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
            CheckFont(font2, "Calibri Light", 12, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font4, "Times New Roman", 12, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
            CheckFont(font8, "Times New Roman", 14, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestLegendEntryFontInheritedProperties.docx", null, false);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            legendEntries = chart.Legend.LegendEntries;
            font0 = legendEntries[0].Font;
            font2 = legendEntries[2].Font;
            font4 = legendEntries[4].Font;
            font8 = legendEntries[8].Font;

            CheckFont(font0, "Times New Roman", 8, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
            CheckFont(font2, "Calibri Light", 12, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font4, "Times New Roman", 12, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
            CheckFont(font8, "Times New Roman", 14, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);

            // Change legend properties to check that if a property is not defined directly, it is taken from legend.

            DmlRunProperties legendProperties = chart.Legend.TxPr.RunPr;
            legendProperties.LatinFont = new DmlFont();
            legendProperties.LatinFont.TextTypeface = "Arial";
            legendProperties.FontSize = new DmlTextPoints(800);
            legendProperties.Bold = true;
            legendProperties.Italics = false;
            legendProperties.Underline = Underline.None;
            chart.Legend.TxPr.FirstParagraph.Properties.HasDefaultRunProperties = true;

            CheckFont(font0, "Times New Roman", 8, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
            CheckFont(font2, "Calibri Light", 12, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font4, "Arial", 8, Color.FromArgb(0xFF, 0, 0, 0x8B), true, false, Underline.None);
            CheckFont(font8, "Times New Roman", 14, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestLegendEntryFontInheritedProperties.docx", null, false);
            chart = doc.FirstSection.Body.Shapes[0].Chart;
            legendEntries = chart.Legend.LegendEntries;
            font0 = legendEntries[0].Font;
            font2 = legendEntries[2].Font;
            font4 = legendEntries[4].Font;
            font8 = legendEntries[8].Font;

            CheckFont(font0, "Times New Roman", 8, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
            CheckFont(font2, "Calibri Light", 12, Color.FromArgb(0xFF, 0, 0x80, 0), true, true, Underline.Single);
            CheckFont(font4, "Arial", 8, Color.FromArgb(0xFF, 0, 0, 0x8B), true, false, Underline.None);
            CheckFont(font8, "Times New Roman", 14, Color.FromArgb(0xFF, 0, 0, 0x8B), false, true, Underline.Double);
        }

        /// <summary>
        /// WORDSNET-23543 Legend entry text becomes hidden when updating font of a new/empty legend entry
        /// </summary>
        [Test]
        public void Test23543()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

            ChartSeries series = shape.Chart.Series[0];
            Font font = series.LegendEntry.Font;

            CheckFont(font, "Calibri", 9, Color.FromArgb(89, 89, 89), false, false, Underline.None);

            shape.Chart.Legend.TxPr.RunPr.Italics = true;
            shape.Chart.Legend.TxPr.RunPr.Underline = Underline.Dotted;
            CheckFont(font, "Calibri", 9, Color.FromArgb(89, 89, 89), false, true, Underline.Dotted);

            font.Size = 12;

            CheckFont(font, "Calibri", 12, Color.FromArgb(89, 89, 89), false, true, Underline.Dotted);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\Test23543",
                UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);

            series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            font = series.LegendEntry.Font;

            CheckFont(font, "Calibri", 12, Color.FromArgb(89, 89, 89), false, true, Underline.Dotted);

            font.ClearFormatting();
            CheckFont(font, "Calibri", 9, Color.FromArgb(89, 89, 89), false, true, Underline.Dotted);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\Test23543ClearFormat",
                UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);

            series = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            font = series.LegendEntry.Font;
            CheckFont(font, "Calibri", 9, Color.FromArgb(89, 89, 89), false, true, Underline.Dotted);
        }

        /// <summary>
        /// Tests resolved legend entry format when direct legend entry format, legend format and chart space format
        /// are defined at the same time.
        /// </summary>
        [Test]
        public void TestLegendEntryFormat()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestLegendEntryFormat.docx");

            CheckLegendEntryFont(doc, 0, 0, "Arial", 11, Color.FromArgb(0x80, 0, 0), true, true, Underline.Single);
            CheckLegendEntryFont(doc, 0, 1, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 0, 2, "Calibri Light", 7, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 0, 3, "Times New Roman", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 0, 4, "Calibri Light", 9, Color.FromArgb(0, 0x80, 0), true, false, Underline.Dash);

            CheckLegendEntryFont(doc, 1, 0, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 1, 1, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 1, 2, "Calibri Light", 7, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 1, 3, "Times New Roman", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 1, 4, "Calibri Light", 9, Color.FromArgb(0, 0x80, 0), true, false, Underline.Dash);

            CheckLegendEntryFont(doc, 2, 0, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 2, 1, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 2, 2, "Calibri Light", 7, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 2, 3, "Times New Roman", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 2, 4, "Calibri Light", 9, Color.FromArgb(0, 0x80, 0), true, false, Underline.Dash);

            CheckLegendEntryFont(doc, 3, 0, "Calibri", 10, Color.Empty, false, false, Underline.None);
            CheckLegendEntryFont(doc, 3, 1, "Calibri", 10, Color.Empty, false, false, Underline.None);
            CheckLegendEntryFont(doc, 3, 2, "Calibri", 7, Color.Empty, false, false, Underline.None);
            CheckLegendEntryFont(doc, 3, 3, "Times New Roman", 10, Color.Empty, false, false, Underline.None);
            CheckLegendEntryFont(doc, 3, 4, "Calibri", 10, Color.FromArgb(0, 0x80, 0), false, false, Underline.None);

            CheckLegendEntryFont(doc, 4, 0, "Calibri Light", 12, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 4, 1, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 4, 2, "Calibri Light", 7, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 4, 3, "Times New Roman", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 4, 4, "Calibri Light", 9, Color.FromArgb(0, 0x80, 0), true, false, Underline.Dash);

            CheckLegendEntryFont(doc, 5, 0, "Arial", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 5, 1, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 5, 2, "Calibri Light", 7, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 5, 3, "Times New Roman", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 5, 4, "Calibri Light", 9, Color.FromArgb(0, 0x80, 0), true, false, Underline.Dash);

            CheckLegendEntryFont(doc, 6, 0, "Calibri Light", 9, Color.FromArgb(0x80, 0, 0), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 6, 1, "Calibri Light", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 6, 2, "Calibri Light", 7, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 6, 3, "Times New Roman", 9, Color.FromArgb(0, 0, 0x80), true, false, Underline.Dash);
            CheckLegendEntryFont(doc, 6, 4, "Calibri Light", 9, Color.FromArgb(0, 0x80, 0), true, false, Underline.Dash);
        }

        /// <summary>
        /// Tests that no exception occurs when assigning <see cref="Font"/> properties supported by a legend entry.
        /// </summary>
        [Test]
        public void TestSupportedFontProperties()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

            ChartSeries series = shape.Chart.Series[0];
            Font font = series.LegendEntry.Font;

            font.Name = "Arial";
            font.NameAscii = "Arial";
            font.NameBi = "Times New Roman";
            font.NameFarEast = "Calibri";
            font.NameOther = "Windings";
            font.ThemeFont = ThemeFont.None;
            font.ThemeFontAscii = ThemeFont.None;
            font.ThemeFontBi = ThemeFont.None;
            font.ThemeFontFarEast = ThemeFont.None;
            font.ThemeFontOther = ThemeFont.None;
            font.Size = 12;
            font.SizeBi = 12;
            font.Bold = false;
            font.BoldBi = false;
            font.Italic = true;
            font.ItalicBi = true;
            font.Color = Color.Blue;
            font.StrikeThrough = false;
            font.DoubleStrikeThrough = true;
            font.Superscript = false;
            font.Subscript = true;
            font.SmallCaps = false;
            font.AllCaps = false;
            font.Underline = Underline.None;
            font.Spacing = 1;
            font.Kerning = 1;
            font.HighlightColor = Color.Yellow;
            font.LocaleId = 0x1409;
            font.LocaleIdBi = 0x1409;
            font.LocaleIdFarEast = 0x1409;

            font.Fill.Solid(Color.Blue);
            font.Border.LineStyle = LineStyle.None;

            CheckFont(font, "Arial", 12, Color.FromArgb(0, 0, 0xff), false, true, Underline.None);
            Assert.That(font.NameAscii, Is.EqualTo("Arial"));
            Assert.That(font.NameBi, Is.EqualTo("Times New Roman"));
            Assert.That(font.NameFarEast, Is.EqualTo("Calibri"));
            Assert.That(font.NameOther, Is.EqualTo("Windings"));
            Assert.That(font.ThemeFont, Is.EqualTo(ThemeFont.None));
            Assert.That(font.ThemeFontAscii, Is.EqualTo(ThemeFont.None));
            Assert.That(font.ThemeFontBi, Is.EqualTo(ThemeFont.None));
            Assert.That(font.ThemeFontFarEast, Is.EqualTo(ThemeFont.None));
            Assert.That(font.ThemeFontOther, Is.EqualTo(ThemeFont.None));
            Assert.That(font.SizeBi, Is.EqualTo(12));
            Assert.That(font.BoldBi, Is.False);
            Assert.That(font.ItalicBi, Is.True);
            Assert.That(font.StrikeThrough, Is.False);
            Assert.That(font.DoubleStrikeThrough, Is.True);
            Assert.That(font.Superscript, Is.False);
            Assert.That(font.Subscript, Is.True);
            Assert.That(font.SmallCaps, Is.False);
            Assert.That(font.AllCaps, Is.False);
            Assert.That(font.Spacing, Is.EqualTo(1));
            Assert.That(font.Kerning, Is.EqualTo(1));
            Assert.That(font.HighlightColor, Is.EqualTo(Color.FromArgb(0xff, 0xff, 0)));
            // DML has only one locale Id property.
            // Font.LocaleId/LocaleIdBi/LocaleIdFarEast refer to the same DML property.
            Assert.That(font.LocaleId, Is.EqualTo(0x1409));
            Assert.That(font.LocaleIdBi, Is.EqualTo(0x1409));
            Assert.That(font.LocaleIdFarEast, Is.EqualTo(0x1409));
            // FOSS: font.LineSpacing assertion removed - it now reflects the last-resort font
            // (font-specific metrics were removed with the font substitution engine).

            // Readonly properties and method:
            Assert.That(font.HasDmlEffect(TextDmlEffect.Glow), Is.False);
            Assert.That(font.Fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(font.AutoColor, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));
            // Now a new instance of Border and Shading are returned when accessing the corresponding Font properties.
            Assert.That(font.Border.LineStyle, Is.EqualTo(LineStyle.None));
            Assert.That(font.Shading.BackgroundPatternColor, Is.EqualTo(Color.Empty));

            // Currently unsupported properties:
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(font.TintAndShade, Is.EqualTo(0));
            Assert.That(font.Shadow, Is.False);
            Assert.That(font.Outline, Is.False);
            Assert.That(font.Emboss, Is.False);
            Assert.That(font.Engrave, Is.False);
            Assert.That(font.Hidden, Is.False);
            Assert.That(font.UnderlineColor, Is.EqualTo(Color.Empty));
            Assert.That(font.Scaling, Is.EqualTo(100));
            Assert.That(font.Position, Is.EqualTo(0));
            Assert.That(font.TextEffect, Is.EqualTo(TextEffect.None));
            Assert.That(font.Bidi, Is.False);
            Assert.That(font.ComplexScript, Is.False);
            Assert.That(font.NoProofing, Is.False);
            Assert.That(font.StyleName, Is.EqualTo("Default Paragraph Font"));
            Assert.That(font.StyleIdentifier, Is.EqualTo(StyleIdentifier.DefaultParagraphFont));
            Assert.That(font.SnapToGrid, Is.True);
            Assert.That(font.EmphasisMark, Is.EqualTo(EmphasisMark.None));
        }

        /// <summary>
        /// Tests that exception occurs when setting <see cref="Font"/> properties unsupported by a legend entry.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "The property is not supported on a chart object.")]
        public void TestUnsupportedFontProperties()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

            ChartSeries series = shape.Chart.Series[0];
            Font font = series.LegendEntry.Font;

            // Throws an exception.
            font.ThemeColor = ThemeColor.Accent1;
        }

        /// <summary>
        /// Tests the <see cref="Font.Fill"/> property of a legend entry.
        /// </summary>
        [Test]
        public void TestFontFill()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);

            ChartLegend legend = shape.Chart.Legend;
            DmlRunProperties legendRunProperties = legend.TxPr.RunPr;
            Font font1 = legend.LegendEntries[0].Font;
            Font font2 = legend.LegendEntries[1].Font;

            // Reset legend fill.
            legendRunProperties.Remove(DmlRunPropertiesIds.Fill);

            font1.Fill.Solid(Color.Blue);

            Assert.That(font1.Color, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));
            Assert.That(font1.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));

            legendRunProperties.Fill = new DmlSolidFill(DmlColor.CreateFromArgb(1, 0xff, 0, 0));

            Assert.That(font1.Color, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));
            Assert.That(font1.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));
            Assert.That(font2.Color, Is.EqualTo(Color.FromArgb(0xff, 0, 0)));
            Assert.That(font2.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0xff, 0, 0)));
            Assert.That(legendRunProperties.Fill.ColorInternal.ToNativeColor(), Is.EqualTo(Color.FromArgb(0xff, 0, 0)));

            font2.Color = Color.Green;

            Assert.That(font2.Color, Is.EqualTo(Color.FromArgb(0, 0x80, 0)));
            Assert.That(font2.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0, 0x80, 0)));
            Assert.That(legendRunProperties.Fill.ColorInternal.ToNativeColor(), Is.EqualTo(Color.FromArgb(0xff, 0, 0)));

            TestUtil.Save(doc, @"Model\Charts\TestFontFill.docx", null, true);
        }

        /// <summary>
        /// Tests the <see cref="ChartLegend.Font"/> property.
        /// </summary>
        // Font is not defined in legend, chart space (pre Word 2016 chart).
        [TestCase("Test23210", "Calibri", 10, 0u)]
        // Font is defined in legend (pre Word 2016 chart).
        [TestCase("TestLegendEntriesOrderStacked", "Calibri", 9, 0xff595959u)]
        // Font is defined in chart space (pre Word 2016 chart).
        [TestCase("TestChartSpaceFont", "Arial", 12, 0xff2F5496u)]
        // Chart space is not used in Word 2016 charts to get legend font properties.
        // Font is not defined in legend (Word 2016 chart).
        [TestCase(@"Word2016Charts\Pareto", "Calibri", 9, 0xff595959u)]
        // Font is defined in legend (Word 2016 chart).
        [TestCase(@"Word2016Charts\TestLegendFont", "Courier New", 11, 0xff6ca644u)]
        public void TestLegendFont(string fileName, string expectedName, int expectedSize, uint expectedColor)
        {
            Document doc = TestUtil.Open(string.Format(@"Model\Charts\{0}.docx", fileName));
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartLegend legend = chart.Legend;
            Font font = legend.Font;

            Color color = (expectedColor == 0) ? Color.Empty : Color.FromArgb(unchecked((int)expectedColor));

            CheckFont(font, expectedName, expectedSize, color, false, false, Underline.None);

            int expectedKerning = chart.ChartSpace.IsChartEx ? 0 : 12;
            Assert.That(font.Kerning, Is.EqualTo(expectedKerning));
            Assert.That(font.Spacing, Is.EqualTo(0));
            Assert.That(font.DoubleStrikeThrough, Is.False);
            Assert.That(font.StrikeThrough, Is.False);
            Assert.That(font.AllCaps, Is.False);
            Assert.That(font.SmallCaps, Is.False);
            Assert.That(font.VerticalAlignment, Is.EqualTo(RunVerticalAlignment.Baseline));
            Assert.That(font.HighlightColor, Is.EqualTo(Color.Empty));

            font.Name = "Times New Roman";
            font.NameBi = "Courier New";
            font.NameFarEast = "Arial";
            font.NameOther = "Wingdings";
            font.Size = 14;
            font.Color = Color.DarkRed;
            font.HighlightColor = Color.Cyan;
            font.Bold = false;
            font.Italic = true;
            font.Underline = Underline.None;
            font.DoubleStrikeThrough = true;
            font.AllCaps = true;
            font.VerticalAlignment = RunVerticalAlignment.Superscript;
            font.LocaleId = 1033;
            font.Kerning = 10;
            font.Spacing = 3;

            doc = TestUtil.SaveOpen(doc, string.Format(@"Model\Charts\{0}LegendFont.docx", fileName), null, false);
            legend = doc.FirstSection.Body.Shapes[0].Chart.Legend;
            font = legend.Font;

            CheckFont(font, "Times New Roman", 14, Color.FromArgb(0xFF, 0x8B, 0, 0), false, true, Underline.None);

            Assert.That(font.Kerning, Is.EqualTo(10));
            Assert.That(font.Spacing, Is.EqualTo(3));
            Assert.That(font.NameBi, Is.EqualTo("Courier New"));
            Assert.That(font.NameFarEast, Is.EqualTo("Arial"));
            Assert.That(font.NameOther, Is.EqualTo("Wingdings"));
            Assert.That(font.DoubleStrikeThrough, Is.True);
            Assert.That(font.StrikeThrough, Is.False);
            Assert.That(font.AllCaps, Is.True);
            Assert.That(font.SmallCaps, Is.False);
            Assert.That(font.VerticalAlignment, Is.EqualTo(RunVerticalAlignment.Superscript));
            Assert.That(font.HighlightColor, Is.EqualTo(Color.FromArgb(0xFF, 0, 0xFF, 0xFF)));
            Assert.That(font.LocaleId, Is.EqualTo(1033));
        }

        /// <summary>
        /// Tests the <see cref="ChartLegend.Font"/> property in a new chart.
        /// </summary>
        [Test]
        public void TestLegendFontInNewChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape1 = builder.InsertChart(ChartType.Column, 432, 252);

            ChartLegend legend1 = shape1.Chart.Legend;
            Font font1 = legend1.Font;

            CheckFont(font1, "Calibri", 9, Color.FromArgb(0x59, 0x59, 0x59), false, false, Underline.None);

            font1.Name = "Times New Roman";
            font1.NameBi = "Courier New";
            font1.NameFarEast = "Arial";
            font1.NameOther = "Wingdings";
            font1.Size = 12;
            font1.Color = Color.DarkRed;
            font1.HighlightColor = Color.Cyan;
            font1.Bold = true;
            font1.Italic = false;
            font1.Underline = Underline.Single;
            font1.StrikeThrough = true;
            font1.SmallCaps = true;
            font1.VerticalAlignment = RunVerticalAlignment.Subscript;
            font1.LocaleId = 1033;
            font1.Kerning = 3;
            font1.Spacing = 2;

            builder.InsertParagraph();

            Shape shape2 = builder.InsertChart(ChartType.Column, 432, 252);

            ChartLegend legend2 = shape2.Chart.Legend;
            Font font2 = legend2.Font;
            font2.Size = 15;
            font2.Bold = true;

            font2.Fill.TwoColorGradient(Color.Blue, Color.Orange, GradientStyle.DiagonalDown, GradientVariant.Variant2);

            Assert.That(font2.Color, Is.EqualTo(Color.Empty));
            Assert.That(font2.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));
            Assert.That(font2.Fill.BackColor, Is.EqualTo(Color.FromArgb(0xff, 0xa5, 0)));

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestLegendFontInNewChart.docx", null, false);
            legend1 = doc.FirstSection.Body.Shapes[0].Chart.Legend;
            font1 = legend1.Font;

            CheckFont(font1, "Times New Roman", 12, Color.FromArgb(0xFF, 0x8B, 0, 0), true, false, Underline.Single);

            Assert.That(font1.Kerning, Is.EqualTo(3));
            Assert.That(font1.Spacing, Is.EqualTo(2));
            Assert.That(font1.NameBi, Is.EqualTo("Courier New"));
            Assert.That(font1.NameFarEast, Is.EqualTo("Arial"));
            Assert.That(font1.NameOther, Is.EqualTo("Wingdings"));
            Assert.That(font1.DoubleStrikeThrough, Is.False);
            Assert.That(font1.StrikeThrough, Is.True);
            Assert.That(font1.AllCaps, Is.False);
            Assert.That(font1.SmallCaps, Is.True);
            Assert.That(font1.VerticalAlignment, Is.EqualTo(RunVerticalAlignment.Subscript));
            Assert.That(font1.HighlightColor, Is.EqualTo(Color.FromArgb(0xFF, 0, 0xFF, 0xFF)));
            Assert.That(font1.LocaleId, Is.EqualTo(1033));

            legend2 = doc.FirstSection.Body.Shapes[1].Chart.Legend;
            font2 = legend2.Font;

            CheckFont(font2, "Calibri", 15, Color.Empty, true, false, Underline.None);

            Assert.That(font2.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0, 0, 0xff)));
            Assert.That(font2.Fill.BackColor, Is.EqualTo(Color.FromArgb(0xff, 0xa5, 0)));
        }

        /// <summary>
        /// Tests the <see cref="ChartLegend.Position"/> property of a legend in a Word 2016 chart.
        /// </summary>
        [Test]
        public void TestWord2016ChartLegendPosition()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape1 = builder.InsertChart(ChartType.Treemap, 432, 252);

            ChartLegend legend = shape1.Chart.Legend;

            CheckChartExLegendPosition(legend, LegendPosition.Top, SidePosition.Top, PositionAlignment.Center);

            legend.Position = LegendPosition.TopRight;
            CheckChartExLegendPosition(legend, LegendPosition.TopRight, SidePosition.Top, PositionAlignment.Maximum);

            legend.Position = LegendPosition.Bottom;
            CheckChartExLegendPosition(legend, LegendPosition.Bottom, SidePosition.Bottom, PositionAlignment.Center);

            legend.SidePosition = SidePosition.Left;
            CheckChartExLegendPosition(legend, LegendPosition.Left, SidePosition.Left, PositionAlignment.Center);

            legend.PositionAlignment = PositionAlignment.Minimum;
            CheckChartExLegendPosition(legend, LegendPosition.Left, SidePosition.Left, PositionAlignment.Minimum);

            legend.Position = LegendPosition.Left;
            CheckChartExLegendPosition(legend, LegendPosition.Left, SidePosition.Left, PositionAlignment.Minimum);

            legend.Position = LegendPosition.Top;
            CheckChartExLegendPosition(legend, LegendPosition.Top, SidePosition.Top, PositionAlignment.Center);

            legend.PositionAlignment = PositionAlignment.Maximum;
            CheckChartExLegendPosition(legend, LegendPosition.TopRight, SidePosition.Top, PositionAlignment.Maximum);
        }

        /// <summary>
        /// Checks the position properties of a legend.
        /// </summary>
        private void CheckChartExLegendPosition(ChartLegend legend, LegendPosition expectedPosition,
            SidePosition expectedSidePosition, PositionAlignment expectedAlignment)
        {
            Assert.That(legend.Position, Is.EqualTo(expectedPosition));
            Assert.That(legend.SidePosition, Is.EqualTo(expectedSidePosition));
            Assert.That(legend.PositionAlignment, Is.EqualTo(expectedAlignment));
        }

        /// <summary>
        /// Checks font properties of a legend entry specified by chart shape index and legend entry index.
        /// </summary>
        private static void CheckLegendEntryFont(Document document, int shapeIndex, int entryIndex,
            string expectedFontName, double expectedFontSize, Color expectedColor, bool expectedBold,
            bool expectedItalic, Underline expectedUnderline)
        {
            Chart chart = document.FirstSection.Body.Shapes[shapeIndex].Chart;
            ChartLegendEntry legendEntry = chart.Legend.LegendEntries[entryIndex];

            CheckFont(legendEntry.Font, expectedFontName, expectedFontSize, expectedColor,
                expectedBold, expectedItalic, expectedUnderline);
        }

        /// <summary>
        /// Checks properties of a <see cref="Font"/> instance.
        /// </summary>
        private static void CheckFont(Font font, string expectedFontName, double expectedFontSize, Color expectedColor,
            bool expectedBold, bool expectedItalic, Underline expectedUnderline)
        {
            Assert.That(font.Name, Is.EqualTo(expectedFontName));
            Assert.That(font.Size, Is.EqualTo(expectedFontSize).Within(0.1));
            Assert.That(font.Color, Is.EqualTo(expectedColor));
            Assert.That(font.Bold, Is.EqualTo(expectedBold));
            Assert.That(font.Italic, Is.EqualTo(expectedItalic));
            Assert.That(font.Underline, Is.EqualTo(expectedUnderline));
        }
    }
}
