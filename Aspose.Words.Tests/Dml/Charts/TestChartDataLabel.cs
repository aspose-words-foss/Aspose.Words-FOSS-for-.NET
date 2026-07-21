// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2022 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Charts
{
    /// <summary>
    /// Tests behavior of the <see cref="ChartDataLabel"/> and <see cref="ChartDataLabelCollection"/> classes.
    /// </summary>
    [TestFixture]
    public class TestChartDataLabel
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
        /// WORDSNET-19124 Tests label count if a collection contains a label with index larger than the last
        /// used index.
        /// </summary>
        [Test]
        public void TestDataLabelCountWithOutsideLabel()
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartSeries series = chart.Series[0];

            CheckLabelCount(series, 0);

            series.HasDataLabels = true;
            CheckLabelCount(series, 3);

            ChartDataLabel label = series.DataLabels[5];
            CheckLabelCount(series, 3);

            label.ShowCategoryName = true;
            // Now the label #5 has non-default value and is included in Count value.
            CheckLabelCount(series, 4);
        }

        /// <summary>
        /// WORDSNET-19124 Tests that <see cref="ChartDataLabel.ShowDataLabelsRange"/> property is
        /// written/read correctly.
        /// </summary>
        [Test]
        public void TestShowDataLabelsRangeOfLabel()
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartSeries series = chart.Series[0];
            ChartDataLabelCollection labels = series.DataLabels;

            series.HasDataLabels = true;
            labels.ShowValue = true;
            labels[1].ShowDataLabelsRange = true;

            series.DataLabelsRangeData.DataSource.ValueRef = new DmlChartValueRef(DmlChartValueType.String);
            series.DataLabelsRangeData.DataSource.ValueRef.Formula.Value = "Sheet1!$A$2:$A$5";

            DmlChartValueCollection valueCollection = series.DataLabelsRangeData.Values;
            valueCollection.Add(new DmlChartStrValue(0, "Text1"));
            valueCollection.Add(new DmlChartStrValue(1, "Text2"));
            valueCollection.Add(new DmlChartStrValue(2, "Text3"));
            valueCollection.ValueCount = 3;

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestShowDataLabelsRangeOfLabel.docx", null, true);

            chart = doc.FirstSection.Body.Shapes[0].Chart;
            series = chart.Series[0];
            labels = series.DataLabels;

            Assert.That(labels.ShowValue, Is.True);
            Assert.That(labels.ShowDataLabelsRange, Is.False);

            foreach (ChartDataLabel label in labels)
            {
                bool isLabel1 = label.Index == 1;
                Assert.That(label.ShowDataLabelsRange, Is.EqualTo(isLabel1));
                Assert.That(label.HasNonDefaultFormatting, Is.EqualTo(isLabel1));
            }
        }

        /// <summary>
        /// WORDSNET-19124 Tests that <see cref="ChartDataLabelCollection.ShowDataLabelsRange"/> property is
        /// written/read correctly.
        /// </summary>
        [Test]
        public void TestShowDataLabelsRangeOfCollection()
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;
            ChartSeries series = chart.Series[0];

            series.HasDataLabels = true;
            ChartDataLabelCollection labels = series.DataLabels;
            labels.ShowValue = true;
            labels.ShowDataLabelsRange = true;
            Assert.That(labels[1].ShowDataLabelsRange, Is.True);
            labels[1].ShowDataLabelsRange = false;

            series.DataLabelsRangeData.DataSource.ValueRef = new DmlChartValueRef(DmlChartValueType.String);
            series.DataLabelsRangeData.DataSource.ValueRef.Formula.Value = "Sheet1!$A$2:$A$5";

            DmlChartValueCollection valueCollection = series.DataLabelsRangeData.Values;
            valueCollection.Add(new DmlChartStrValue(0, "Text1"));
            valueCollection.Add(new DmlChartStrValue(1, "Text2"));
            valueCollection.Add(new DmlChartStrValue(2, "Text3"));
            valueCollection.ValueCount = 3;

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestShowDataLabelsRangeOfCollection.docx", null, true);

            chart = doc.FirstSection.Body.Shapes[0].Chart;
            series = chart.Series[0];
            labels = series.DataLabels;

            Assert.That(labels.ShowValue, Is.True);
            Assert.That(labels.ShowDataLabelsRange, Is.True);

            foreach (ChartDataLabel label in labels)
            {
                bool isLabel1 = label.Index == 1;
                Assert.That(label.ShowDataLabelsRange, Is.EqualTo(!isLabel1));
                Assert.That(label.HasNonDefaultFormatting, Is.EqualTo(isLabel1));
            }
        }

        /// <summary>
        /// WORDSNET-19124 Tests the <see cref="ChartDataLabel.ClearFormat"/> public method.
        /// </summary>
        [Test]
        public void TestDataLabelClearFormat()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            ChartSeries series = chart.Series[0];
            series.HasDataLabels = true;

            ChartDataLabelCollection labels = chart.Series[0].DataLabels;
            ChartDataLabel label = labels[1];

            labels.ShowValue = true;

            Assert.That(label.ShowDataLabelsRange, Is.False);
            Assert.That(label.ShowValue, Is.True);

            label.ShowDataLabelsRange = true;
            label.ShowValue = false;

            Assert.That(label.ShowDataLabelsRange, Is.True);
            Assert.That(label.ShowValue, Is.False);

            label.ClearFormat();

            Assert.That(label.ShowDataLabelsRange, Is.False);
            Assert.That(label.ShowValue, Is.True);
        }

        /// <summary>
        /// WORDSNET-19124 Tests the <see cref="ChartDataLabel.IsHidden"/> public property.
        /// </summary>
        [Test]
        public void TestHidingChartDataLabel()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            Chart chart = shape.Chart;
            ChartSeries series = chart.Series[0];

            Assert.That(series.HasDataLabels, Is.False);

            series.HasDataLabels = true;

            ChartDataLabel label1 = series.DataLabels[1];
            ChartDataLabel label2 = series.DataLabels[2];
            Assert.That(label1.IsHidden, Is.False);

            label1.IsHidden = true;
            label2.IsHidden = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDeletingChartDataLabel.docx", null, false);

            shape = doc.FirstSection.Body.Shapes[0];
            chart = shape.Chart;
            series = chart.Series[0];

            Assert.That(series.HasDataLabels, Is.True);

            label1 = series.DataLabels[1];
            label2 = series.DataLabels[2];

            Assert.That(label1.IsHidden, Is.True);
            Assert.That(label2.IsHidden, Is.False);
        }

        /// <summary>
        /// WORDSNET-20203 Implemented writing of ShowXXX and DLblPos property values even if a property has
        /// the same value as in the parent collection. It seems MS Word does not take parent values into account on
        /// displaying.
        /// </summary>
        [Test]
        public void Test20203()
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            ChartSeries series = chart.Series[0];

            series.HasDataLabels = true;
            series.DataLabels.ShowValue = true;
            series.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, ChartDataLabelPosition.Above);

            ChartDataLabel label = series.DataLabels[1];
            label.ShowCategoryName = true;
            label.Position = ChartDataLabelPosition.Below;
            Assert.That(label.ShowValue, Is.True); // Inherited value is retrieved.

            // It is expected that value and category name are displayed in the second data label.
            TestUtil.Save(doc, @"Model\Charts\Test20203.docx", null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-21286 Implemented writing ShowXXX property values of data label collection even if
        /// the value of the property is not defined, since default values in AW and MS Word are different on document
        /// versions higher than 12 (Word2007). Fixed issues with data label number format.
        /// </summary>
        [TestCase(12)]
        [TestCase(16)]
        public void Test21286(int docVersion)
        {
            Document doc = CreateDocumentWithChart();
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            doc.BuiltInDocumentProperties.Version = docVersion << 16;

            ChartSeries series = chart.Series[0];

            series.HasDataLabels = true;
            series.DataLabels.ShowValue = true;
            series.DataLabels.NumberFormat.FormatCode = "#,##0.00";

            series.DataLabels[1].ShowCategoryName = true;
            series.DataLabels[1].ShowValue = true;
            series.DataLabels[1].NumberFormat.FormatCode = "#,##0.000";

            series.DataLabels[2].NumberFormat.IsLinkedToSource = true;

            Assert.That(series.DataLabels.NumberFormat.FormatCode, Is.EqualTo("#,##0.00"));
            Assert.That(series.DataLabels[0].NumberFormat.FormatCode, Is.EqualTo("#,##0.00"));
            Assert.That(series.DataLabels[1].NumberFormat.FormatCode, Is.EqualTo("#,##0.000"));
            Assert.That(series.DataLabels.NumberFormat.IsLinkedToSource, Is.False);
            Assert.That(series.DataLabels[2].NumberFormat.IsLinkedToSource, Is.True);

            string fileName = string.Format(@"Model\Charts\Test20203Version{0}.docx", docVersion);
            TestUtil.Save(doc, fileName, null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-23582 Issue with How to Define Default Options for ChartDataLabels of ChartSeries sample
        /// Data labels were not visible in the output document.
        /// </summary>
        [Test]
        public void Test23582()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Pie, 432, 252);
            Chart chart = shape.Chart;
            chart.Series.Clear();

            ChartSeries series = chart.Series.Add("Series 1",
                new string[] { "Category1", "Category2", "Category3" },
                new double[] { 2.7, 3.2, 0.8 });

            Assert.That(series.HasDataLabels, Is.False);

            ChartDataLabelCollection labels = series.DataLabels;
            labels.ShowPercentage = true;
            labels.ShowValue = true;
            labels.ShowLeaderLines = false;
            labels.Separator = " - ";

            Assert.That(series.HasDataLabels, Is.True);

            TestUtil.Save(doc, @"Model\Charts\Test23582.docx", null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// Tests the <see cref="ChartDataLabel.Font"/> property.
        /// </summary>
        [Test]
        public void TestDataLabelFont()
        {
            const string fileName = @"Model\Charts\TestDataLabelFont";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);

            // Test pre-Word 2016 chart.

            ChartSeries series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            ChartDataLabelCollection labels1 = series1.DataLabels;

            Assert.That(labels1.Count, Is.EqualTo(6));

            // Label with index 0 has txPr element with Times New Roman italic font.
            Font font1 = labels1[0].Font;
            // Label with index 1 has tx element with changed font size and bold state for the first run (field).
            Font font2 = labels1[1].Font;
            // Label with index 2 has tx, txPr and spPr elements.
            Font font3 = labels1[2].Font;
            // Non-materialized label.
            Font font4 = labels1[3].Font;
            // Non-materialized label.
            Font font5 = labels1[4].Font;
            // Label with index 5 has tx element with Verdana 8pt bold font.
            Font font6 = labels1[5].Font;

            CheckFont(font1, "Times New Roman", 12, Color.FromArgb(0, 128, 0), Color.Empty, false, true,
                Underline.Single, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            // Font is taken from the first run.
            CheckFont(font2, "Verdana", 10, Color.FromArgb(99, 123, 156), Color.Empty, true, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(font3, "Arial", 18, Color.Empty, Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(font4, "Verdana", 12, Color.FromArgb(99, 123, 156), Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(font5, "Verdana", 12, Color.FromArgb(99, 123, 156), Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(font6, "Verdana", 8, Color.FromArgb(0, 0x80, 0), Color.Empty, true, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);

            ChangeFont(font1, "Courier New", 14, Color.DarkRed, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
            font1.HighlightColor = Color.Cyan;

            ChangeFont(font2, "Times New Roman", 12, Color.Green, false, true,
                Underline.Single, 12, 4, false, true, false, true, RunVerticalAlignment.Subscript, 1049);
            ChangeFont(font3, "Courier New", 10, Color.DarkBlue, true, true,
                Underline.Dotted, 2, 2, false, true, true, false, RunVerticalAlignment.Subscript, 1049);
            ChangeFont(font4, "Arial", 10, Color.Brown, false, false,
                Underline.Dash, 5, 1, true, false, false, true, RunVerticalAlignment.Superscript, 1049);

            font5.NameAscii = "Arial";

            font6.ClearFormatting();

            // Test Word 2016 chart.

            ChartSeries series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            ChartDataLabelCollection labels2 = series2.DataLabels;

            Assert.That(labels2.Count, Is.EqualTo(8));

            // Label with index 0 has txPr element with Times New Roman italic 12pt font.
            font1 = labels2[0].Font;
            // Non-materialized label.
            font2 = labels2[1].Font;
            // Label with index 2 has txPr element with Calibri bold font.
            font3 = labels2[2].Font;
            // Label with index 3 has spPr element only.
            font4 = labels2[3].Font;

            CheckFont(font1, "Times New Roman", 11, Color.FromArgb(0, 128, 0), Color.Empty, false, true,
                Underline.Single, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(font2, "Verdana", 12, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(font3, "Calibri", 10, Color.FromArgb(89, 89, 89), Color.Empty, true, true,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(font4, "Verdana", 12, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);

            ChangeFont(font1, "Courier New", 14, Color.DarkRed, true, false,
                Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            font1.HighlightColor = Color.Cyan;

            ChangeFont(font2, "Times New Roman", 9, Color.Green, false, true,
                Underline.Single, 12, 2, false, true, false, true, RunVerticalAlignment.Subscript, 1049);

            font3.ClearFormatting();

            font4.NameAscii = "Arial";

            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);
            series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            labels1 = series1.DataLabels;

            // Check results.

            Assert.That(labels1.Count, Is.EqualTo(6));

            CheckFont(labels1[0].Font, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), Color.FromArgb(0, 0xff, 0xff),
                true, false, Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
            CheckFont(labels1[1].Font, "Times New Roman", 12, Color.FromArgb(0, 0x80, 0), Color.Empty, false, true,
                Underline.Single, 12, 4, false, true, false, true, RunVerticalAlignment.Subscript, 1049);
            CheckFont(labels1[2].Font, "Courier New", 10, Color.FromArgb(0, 0, 0x8b), Color.Empty, true, true,
                Underline.Dotted, 2, 2, false, true, true, false, RunVerticalAlignment.Subscript, 1049);
            CheckFont(labels1[3].Font, "Arial", 10, Color.FromArgb(0xa5, 0x2a, 0x2a), Color.Empty, false, false,
                Underline.Dash, 5, 1, true, false, false, true, RunVerticalAlignment.Superscript, 1049);
            CheckFont(labels1[4].Font, "Arial", 12, Color.FromArgb(99, 123, 156), Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(labels1[5].Font, "Verdana", 12, Color.FromArgb(99, 123, 156), Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            labels2 = series2.DataLabels;

            Assert.That(labels2.Count, Is.EqualTo(8));

            CheckFont(labels2[0].Font, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), Color.FromArgb(0, 0xff, 0xff),
                true, false, Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            CheckFont(labels2[1].Font, "Times New Roman", 9, Color.FromArgb(0, 128, 0), Color.Empty, false, true,
                Underline.Single, 12, 2, false, true, false, true, RunVerticalAlignment.Subscript, 1049);
            CheckFont(labels2[2].Font, "Verdana", 12, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels2[3].Font, "Arial", 12, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
        }

        /// <summary>
        /// Tests the <see cref="ChartDataLabelCollection.Font"/> property.
        /// </summary>
        [Test]
        public void TestDataLabelCollectionFont()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestDataLabelFont.docx");

            // Test pre-Word 2016 chart.

            ChartSeries series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            ChartDataLabelCollection labels1 = series1.DataLabels;
            Font font1 = labels1.Font;
            CheckFont(font1, "Verdana", 12, Color.FromArgb(99, 123, 156), Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            ChangeFont(font1, "Courier New", 14, Color.DarkRed, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
            font1.HighlightColor = Color.Cyan;

            // Test Word 2016 chart.

            ChartSeries series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            ChartDataLabelCollection labels2 = series2.DataLabels;
            Font font2 = labels2.Font;

            CheckFont(font2, "Verdana", 12, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);

            ChangeFont(font2, "Courier New", 14, Color.DarkRed, true, false,
                Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            font2.HighlightColor = Color.Cyan;

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataLabelCollectionFont.docx",
                UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);

            // Check results.

            series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            labels1 = series1.DataLabels;
            font1 = labels1.Font;

            Color cyanColor = Color.FromArgb(0, 0xff, 0xff);

            CheckFont(font1, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            CheckFont(labels1[0].Font, "Times New Roman", 12, Color.FromArgb(0, 128, 0), Color.Empty, false, true,
                Underline.Single, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(labels1[1].Font, "Courier New", 10, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
            CheckFont(labels1[2].Font, "Arial", 18, Color.Empty, Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels1[3].Font, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
            CheckFont(labels1[4].Font, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
            CheckFont(labels1[5].Font, "Courier New", 8, Color.FromArgb(0, 0x80, 0), cyanColor, true, false,
                Underline.Double, 10, 3, true, false, true, false, RunVerticalAlignment.Baseline, 1033);

            series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            labels2 = series2.DataLabels;
            font2 = labels2.Font;

            CheckFont(font2, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);

            CheckFont(labels2[0].Font, "Times New Roman", 11, Color.FromArgb(0, 128, 0), cyanColor, false, true,
                Underline.Single, 10, 1, false, false, true, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels2[1].Font, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);
            CheckFont(labels2[2].Font, "Calibri", 10, Color.FromArgb(89, 89, 89), cyanColor, true, true,
                Underline.None, 10, 1, false, false, true, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels2[3].Font, "Courier New", 14, Color.FromArgb(0x8b, 0, 0), cyanColor, true, false,
                Underline.Double, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1049);

            // Test ClearFormatting.
            // After clearing formatting chart space font options are used for pre-Word 2016 charts.

            font1.ClearFormatting();
            font2.ClearFormatting();

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataLabelCollectionFontClearFormatting.docx",
                UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);
            series1 = doc.FirstSection.Body.Shapes[0].Chart.Series[0];
            labels1 = series1.DataLabels;
            font1 = labels1.Font;

            CheckFont(font1, "Calibri", 9, Color.Empty, Color.Empty, false, false,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            CheckFont(labels1[0].Font, "Times New Roman", 12, Color.FromArgb(0, 128, 0), Color.Empty, false, true,
                Underline.Single, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(labels1[1].Font, "Calibri", 10, Color.Empty, Color.Empty, true, false,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels1[2].Font, "Arial", 18, Color.Empty, Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels1[3].Font, "Calibri", 9, Color.Empty, Color.Empty, false, false,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(labels1[4].Font, "Calibri", 9, Color.Empty, Color.Empty, false, false,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(labels1[5].Font, "Calibri", 8, Color.FromArgb(0, 0x80, 0), Color.Empty, true, false,
                Underline.Single, 9, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);

            series2 = doc.FirstSection.Body.Shapes[1].Chart.Series[0];
            labels2 = series2.DataLabels;
            font2 = labels2.Font;

            CheckFont(font2, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            CheckFont(labels2[0].Font, "Times New Roman", 11, Color.FromArgb(0, 128, 0), Color.Empty, false, true,
                Underline.Single, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels2[1].Font, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
            CheckFont(labels2[2].Font, "Calibri", 10, Color.FromArgb(89, 89, 89), Color.Empty, true, true,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1033);
            CheckFont(labels2[3].Font, "Calibri", 9, Color.FromArgb(89, 89, 89), Color.Empty, false, false,
                Underline.None, 0, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);
        }

        /// <summary>
        /// Tests the <see cref="ChartDataLabel.Font"/> property when creating a new chart.
        /// </summary>
        [Test]
        public void TestDataLabelFontInNewChart()
        {
            // TODO: add testing of Word 2016 charts when they can be created in AW.

            Document doc = new Document();
            DocumentBuilder builder  = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Line, 432, 252);
            ChartSeries series = shape.Chart.Series[0];
            ChartDataLabelCollection labels = series.DataLabels;
            series.HasDataLabels = true;
            labels.ShowValue = true;

            Font font = labels.Font;
            Font font1 = labels[0].Font;

            CheckFont(font, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            CheckFont(font1, "Calibri", 10, Color.Empty, Color.Empty, false, false,
                Underline.None, 12, 0, false, false, false, false, RunVerticalAlignment.Baseline, 1024);

            // Change font.

            ChangeFont(font, "Arial", 11, Color.DarkRed, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            ChangeFont(font1, "Times New Roman", 14, Color.Red, true, false,
                Underline.Double, 0, 0, false, true, false, true, RunVerticalAlignment.Subscript, 1049);

            labels[1].Font.Size = 9;

            // Check.

            CheckFont(font, "Arial", 11, Color.FromArgb(0x8b, 0, 0), Color.Empty, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            CheckFont(font1, "Times New Roman", 14, Color.FromArgb(0xff, 0, 0), Color.Empty, true, false,
                Underline.Double, 0, 0, false, true, false, true, RunVerticalAlignment.Subscript, 1049);

            CheckFont(labels[1].Font, "Arial", 9, Color.FromArgb(0x8b, 0, 0), Color.Empty, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);

            CheckFont(labels[2].Font, "Arial", 11, Color.FromArgb(0x8b, 0, 0), Color.Empty, false, true,
                Underline.Single, 10, 1, true, false, true, false, RunVerticalAlignment.Superscript, 1033);
        }

        /// <summary>
        /// WORDSNET-25711 Data label font size and color changes are not applied
        /// Item txPr element was not generated correctly when the corresponding element of the collection is empty.
        /// </summary>
        [Test]
        public void Test25711()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Check font size.
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            ChartSeries series = shape.Chart.Series[0];

            series.HasDataLabels = true;
            series.DataLabels.ShowValue = true;
            series.DataLabels[1].Font.Size = 15;

            Assert.That(series.DataLabels[1].Font.Size, Is.EqualTo(15).Within(0.01));

            // Check font color.
            shape = builder.InsertChart(ChartType.Column, 432, 252);
            series = shape.Chart.Series[0];

            series.HasDataLabels = true;
            series.DataLabels.ShowValue = true;
            series.DataLabels[1].Font.Color = Color.Red;

            Assert.That(series.DataLabels[1].Font.Color, Is.EqualTo(Color.FromArgb(0xff, 0, 0)));

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\Test25711.docx");
        }

        /// <summary>
        /// Tests data label orientation and rotation in a new chart.
        /// </summary>
        [Test]
        public void TestDataLabelOrientationInNewChart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape1 = builder.InsertChart(ChartType.Column, 432, 252);
            ChartSeries series1 = shape1.Chart.Series[0];
            ChartDataLabelCollection dataLabels1 = series1.DataLabels;

            series1.HasDataLabels = true;
            dataLabels1.ShowValue = true;
            dataLabels1.ShowCategoryName = true;
            dataLabels1.Format.ShapeType = ChartShapeType.UpArrow;
            dataLabels1.Format.Stroke.Fill.Solid(Color.DarkBlue);

            dataLabels1.Orientation = ShapeTextOrientation.VerticalFarEast;
            dataLabels1.Rotation = -10;

            dataLabels1[1].Orientation = ShapeTextOrientation.Horizontal;
            dataLabels1[1].Rotation = 45;

            dataLabels1[2].Orientation = ShapeTextOrientation.WordArtVertical;

            dataLabels1[3].Rotation = 5;

            Assert.That(dataLabels1.Orientation, Is.EqualTo(ShapeTextOrientation.VerticalFarEast));
            Assert.That(dataLabels1.Rotation, Is.EqualTo(-10));
            CheckOrientation(dataLabels1[0], ShapeTextOrientation.VerticalFarEast, -10);
            CheckOrientation(dataLabels1[1], ShapeTextOrientation.Horizontal, 45);
            CheckOrientation(dataLabels1[2], ShapeTextOrientation.WordArtVertical, -10);
            CheckOrientation(dataLabels1[3], ShapeTextOrientation.VerticalFarEast, 5);

            // Test setting properties of individual data labels without setting properties of the collection.

            Shape shape2 = builder.InsertChart(ChartType.Column, 432, 252);
            ChartSeries series2 = shape2.Chart.Series[0];
            ChartDataLabelCollection dataLabels2 = series2.DataLabels;

            series2.HasDataLabels = true;
            dataLabels2.ShowValue = true;
            dataLabels2.ShowCategoryName = true;
            dataLabels2.Format.ShapeType = ChartShapeType.UpArrow;
            dataLabels2.Format.Stroke.Fill.Solid(Color.DarkBlue);

            Assert.That(dataLabels2.Orientation, Is.EqualTo(ShapeTextOrientation.Horizontal));
            Assert.That(dataLabels2.Rotation, Is.EqualTo(0));

            dataLabels2[1].Orientation = ShapeTextOrientation.Downward;
            dataLabels2[1].Rotation = 10;

            dataLabels2[2].Orientation = ShapeTextOrientation.Upward;

            dataLabels2[3].Rotation = 15;

            Assert.That(dataLabels2.Orientation, Is.EqualTo(ShapeTextOrientation.Horizontal));
            Assert.That(dataLabels2.Rotation, Is.EqualTo(0));
            CheckOrientation(dataLabels2[0], ShapeTextOrientation.Horizontal, 0);
            CheckOrientation(dataLabels2[1], ShapeTextOrientation.Downward, 10);
            CheckOrientation(dataLabels2[2], ShapeTextOrientation.Upward, 0);
            CheckOrientation(dataLabels2[3], ShapeTextOrientation.Horizontal, 15);

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestLabelOrientationInNewChart.docx", null, true);

            shape1 = doc.FirstSection.Body.Shapes[0];
            series1 = shape1.Chart.Series[0];
            dataLabels1 = series1.DataLabels;

            Assert.That(dataLabels1.Orientation, Is.EqualTo(ShapeTextOrientation.VerticalFarEast));
            Assert.That(dataLabels1.Rotation, Is.EqualTo(-10));
            CheckOrientation(dataLabels1[0], ShapeTextOrientation.VerticalFarEast, -10);
            CheckOrientation(dataLabels1[1], ShapeTextOrientation.Horizontal, 45);
            CheckOrientation(dataLabels1[2], ShapeTextOrientation.WordArtVertical, -10);
            CheckOrientation(dataLabels1[3], ShapeTextOrientation.VerticalFarEast, 5);

            shape2 = doc.FirstSection.Body.Shapes[1];
            series2 = shape2.Chart.Series[0];
            dataLabels2 = series2.DataLabels;

            Assert.That(dataLabels2.Orientation, Is.EqualTo(ShapeTextOrientation.Horizontal));
            Assert.That(dataLabels2.Rotation, Is.EqualTo(0));
            CheckOrientation(dataLabels2[0], ShapeTextOrientation.Horizontal, 0);
            CheckOrientation(dataLabels2[1], ShapeTextOrientation.Downward, 10);
            CheckOrientation(dataLabels2[2], ShapeTextOrientation.Upward, 0);
            CheckOrientation(dataLabels2[3], ShapeTextOrientation.Horizontal, 15);
        }

        /// <summary>
        /// Tests data label orientation and rotation in an existing chart.
        /// </summary>
        [Test]
        public void TestDataLabelOrientationInExistingChart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestDataLabelOrientation.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];
            ChartSeries series = shape.Chart.Series[0];
            ChartDataLabelCollection dataLabels = series.DataLabels;

            Assert.That(dataLabels.Orientation, Is.EqualTo(ShapeTextOrientation.Horizontal));
            Assert.That(dataLabels.Rotation, Is.EqualTo(0));
            CheckOrientation(dataLabels[0], ShapeTextOrientation.Horizontal, 15);
            CheckOrientation(dataLabels[1], ShapeTextOrientation.VerticalFarEast, 15);
            CheckOrientation(dataLabels[2], ShapeTextOrientation.VerticalRotatedFarEast, 15);
            CheckOrientation(dataLabels[3], ShapeTextOrientation.Downward, 15);
            CheckOrientation(dataLabels[4], ShapeTextOrientation.Upward, 15);
            CheckOrientation(dataLabels[5], ShapeTextOrientation.WordArtVertical, 15);
            CheckOrientation(dataLabels[6], ShapeTextOrientation.WordArtVerticalRightToLeft, 15);

            dataLabels[0].Orientation = ShapeTextOrientation.WordArtVerticalRightToLeft;
            dataLabels[0].Rotation = 0;

            dataLabels[1].Orientation = ShapeTextOrientation.Horizontal;

            dataLabels[2].Rotation = -15;

            doc = TestUtil.SaveOpen(doc, @"Model\Charts\TestDataLabelOrientation.docx", null, true);

            shape = doc.FirstSection.Body.Shapes[0];
            series = shape.Chart.Series[0];
            dataLabels = series.DataLabels;

            CheckOrientation(dataLabels[0], ShapeTextOrientation.WordArtVerticalRightToLeft, 0);
            CheckOrientation(dataLabels[1], ShapeTextOrientation.Horizontal, 15);
            CheckOrientation(dataLabels[2], ShapeTextOrientation.VerticalRotatedFarEast, -15);
        }

        /// <summary>
        /// Tests the data label position for all series types.
        /// </summary>
        [Test]
        public void TestPosition()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            List<ChartDataLabelPosition> supportedPositions = new List<ChartDataLabelPosition>();

            // Box&Whisker chart has many labels, let's set each position value to two labels.
            int[] boxWhiskerLabelStartIndexes = new[] { 9, 20, 30 };
            const int boxWhiskerNextTestLabelOffset = 2;

            foreach (ChartSeriesType seriesType in Enum.GetValues(typeof(ChartSeriesType)))
            {
                // These series types has no corresponding chart type; charts of these types cannot be created: skip them.
                if ((seriesType == ChartSeriesType.ParetoLine) || (seriesType == ChartSeriesType.RegionMap))
                    continue;

                if (!FillSupportedPositions(seriesType, supportedPositions))
                    continue;

                ChartType chartType = DmlChartUtil.SeriesTypeToChartType(seriesType);
                Shape shape = builder.InsertChart(chartType, 432, 252);
                Chart chart = shape.Chart;

                chart.Title.Text = chartType.ToString();

                // Prepare necessary number of data labels.

                ChartSeriesCollection seriesCollection = chart.Series;
                if (seriesType != ChartSeriesType.Stock)
                {
                    while (seriesCollection.Count > 1)
                        seriesCollection.RemoveAt(seriesCollection.Count - 1);
                }

                ChartSeries series =  seriesCollection[0];
                if (seriesType != ChartSeriesType.Histogram)
                {
                    while (series.ValueCount < supportedPositions.Count)
                    {
                        ChartXValue xValue = GetNewXValue(series);
                        ChartYValue yValue = ChartYValue.FromDouble(series.ValueCount);
                        if ((seriesType == ChartSeriesType.Bubble) || (seriesType == ChartSeriesType.Bubble3D))
                            series.Add(xValue, yValue, series.BubbleSizes[0]);
                        else
                            series.Add(xValue, yValue);
                    }

                    if (!chart.ChartSpace.IsChartEx && (seriesType != ChartSeriesType.PieOfPie) &&
                        (seriesType != ChartSeriesType.PieOfBar))
                    {
                        while (series.ValueCount > supportedPositions.Count)
                            series.Remove(series.ValueCount - 1);
                    }
                }

                // Show data labels and set their positions.

                series.HasDataLabels = true;
                ChartDataLabelCollection dataLabels = series.DataLabels;
                dataLabels.Position = supportedPositions[0];
                if (seriesType == ChartSeriesType.BoxAndWhisker)
                {
                    for (int i = 1; i < supportedPositions.Count; i++)
                    {
                        for (int j = 0; j <= 2; j += boxWhiskerNextTestLabelOffset)
                            dataLabels[boxWhiskerLabelStartIndexes[i - 1] + j].Position = supportedPositions[i];
                    }
                }
                else
                {
                    for (int i = 0; i < supportedPositions.Count; i++)
                    {
                        ChartDataLabel label = dataLabels[i];

                        // Write position value as label text (it doesn't work for Word 2016 charts).
                        if (!chart.ChartSpace.IsChartEx)
                        {
                            label.LabelPr.SetProperty(
                                DmlChartDataLabelAttrs.Tx,
                                DmlChartRenderingUtil.CreateTx(supportedPositions[i].ToString()));
                        }

                        // The first label gets value from the collection.
                        if (i > 0)
                            label.Position = supportedPositions[i];
                    }
                }
            }

            doc = TestUtil.SaveOpenDocxExportOnly(doc, @"Model\Charts\TestDataLabelPosition.docx");

            // Check the positions after saving/opening.

            foreach (Shape shape in doc.FirstSection.Body.Shapes)
            {
                ChartSeries series = shape.Chart.Series[0];
                FillSupportedPositions(series.SeriesType, supportedPositions);
                Assert.That(supportedPositions.Count > 0, Is.True);

                ChartDataLabelCollection dataLabels = series.DataLabels;
                Assert.That(dataLabels.Position, Is.EqualTo(supportedPositions[0]));
                if (series.SeriesType == ChartSeriesType.BoxAndWhisker)
                {
                    for (int i = 1; i < supportedPositions.Count; i++)
                    {
                        int startIndex = boxWhiskerLabelStartIndexes[i - 1];
                        for (int j = 0; j <= 2; j += boxWhiskerNextTestLabelOffset)
                            Assert.That(dataLabels[startIndex + j].Position, Is.EqualTo(supportedPositions[i]));
                    }
                }
                else
                {
                    for (int i = 0; i < supportedPositions.Count; i++)
                        Assert.That(dataLabels[i].Position, Is.EqualTo(supportedPositions[i]));
                }
            }
        }

        /// <summary>
        /// Tests specifying location of a data label using absolute coordinates.
        /// </summary>
        [Test]
        public void TestLeftTopWithAbsoluteCoordinates()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                Document doc = new Document();
                DocumentBuilder builder = new DocumentBuilder(doc);

                // Test moving labels of a doughnut chart so that they are outside of the chart ring as some
                // customers requested.
                // Let's generate a lots of values, and place the labels around a large circle.

                // All constants are in points.
                const int width = 432;
                const int height = 252;
                const int ringR = 88;
                const int ringCenterY = 139;
                const int titleArea = 22;
                const int marginY = 3;

                Shape shape = builder.InsertChart(ChartType.Doughnut, width, height);
                Chart chart = shape.Chart;
                chart.Title.Text = "Test moving labels";
                chart.Legend.Position = LegendPosition.None;

                ChartSeries series = chart.Series[0];
                series.ClearValues();
                double total = 0;
                for (double n = 5; n >= 1; n -= 0.2)
                {
                    series.Add(ChartXValue.FromString(string.Format("XYZ{0:F1}", n)), ChartYValue.FromDouble(n));
                    total += n;
                }

                series.HasDataLabels = true;
                ChartDataLabelCollection dataLabels = series.DataLabels;
                dataLabels.ShowCategoryName = true;
                dataLabels.ShowValue = false;
                dataLabels.ShowLeaderLines = true;

                double labelHeight = dataLabels.Font.Size * 1.65;
                double labelWidth = dataLabels.Font.Size * series.XValues[0].StringValue.Length * 0.5875;
                double newLabelRingR = ringR + labelWidth;
                // These coordinates are calculated with a center located at the chart ring center and the Y-axis
                // pointing upward.
                double maxY = ringCenterY - (titleArea + marginY + labelHeight / 2);
                double minY = ringCenterY - (height - marginY - labelHeight / 2);
                double totalAngle = 0;
                double previousX = 0;
                double previousY = 0;

                for (int i = 0; i < series.ValueCount; i++)
                {
                    ChartDataLabel dataLabel = dataLabels[i];
                    if ((i > 0) && dataLabels[i - 1].IsHidden)
                    {
                        dataLabel.IsHidden = true;
                        continue;
                    }

                    double angle = series.YValues[i].DoubleValue / total * 2 * System.Math.PI;
                    double labelAngle = angle / 2 + totalAngle;

                    // These coordinates are calculated with a center located at the chart ring center and the Y-axis
                    // pointing upward.
                    double newLocationX = newLabelRingR * System.Math.Sin(labelAngle);
                    double newLocationY = newLabelRingR * System.Math.Cos(labelAngle);
                    if (newLocationY > maxY)
                        newLocationY = maxY;
                    if (newLocationY < minY)
                        newLocationY = minY;
                    if ((i > 0) && (System.Math.Abs(newLocationY - previousY) < labelHeight))
                    {
                        if (MathUtil.AreEqual(newLocationY, minY))
                        {
                            if (newLocationX > previousX - labelWidth)
                                newLocationX = previousX - labelWidth;
                        }
                        else
                        {
                            newLocationY = previousY + ((labelAngle < System.Math.PI)
                                ? -labelHeight
                                : labelHeight);
                            if (newLocationY > maxY)
                            {
                                // Hide this and further data labels.
                                dataLabel.IsHidden = true;
                                continue;
                            }
                        }
                    }

                    dataLabel.Left = newLocationX + width / 2d - labelWidth / 2;
                    dataLabel.LeftMode = ChartDataLabelLocationMode.Absolute;
                    dataLabel.Top = ringCenterY - newLocationY - labelHeight / 2;
                    dataLabel.TopMode = ChartDataLabelLocationMode.Absolute;

                    totalAngle += angle;
                    previousX = newLocationX;
                    previousY = newLocationY;
                }

                TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestDataLabelLeftTopWithAbsoluteCoordinates.docx");
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Tests specifying location of a data label using coordinates relative to its position specified by the
        /// <see cref="ChartDataLabel.Position"/> property.
        /// </summary>
        [Test]
        public void TestLeftTopWithRelativeCoordinates()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Test moving data labels of a bar chart so that they are in front of bars.

            Shape shape = builder.InsertChart(ChartType.Bar, 432, 252);
            Chart chart = shape.Chart;
            chart.AxisX.Hidden = true;
            chart.AxisY.Scaling.Minimum = new AxisBound(-1);

            // Let's move the data labels so they are further apart vertically.
            const double verticalMovePerLabel = 0.75;
            double topOffset = (chart.Series.Count - 1) / 2 * verticalMovePerLabel;

            foreach (ChartSeries series in chart.Series)
            {
                series.HasDataLabels = true;
                ChartDataLabelCollection dataLabels = series.DataLabels;
                dataLabels.ShowValue = true;
                dataLabels.ShowLeaderLines = false;
                dataLabels.Position = ChartDataLabelPosition.InsideBase;

                foreach (ChartDataLabel dataLabel in dataLabels)
                {
                    dataLabel.LeftMode = ChartDataLabelLocationMode.Offset;
                    dataLabel.Left = -27;
                    dataLabel.TopMode = ChartDataLabelLocationMode.Offset;
                    dataLabel.Top = topOffset;
                }

                topOffset -= verticalMovePerLabel;
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestDataLabelLeftTopWithRelativeCoordinates.docx");
        }

        /// <summary>
        /// Tests data label position in an existing chart.
        /// </summary>
        [Test]
        public void TestPositionInExistingChart()
        {
            Document doc = TestUtil.Open(@"Model\Charts\TestDataLabelPosition.docx");
            Chart chart = doc.FirstSection.Body.Shapes[0].Chart;

            ChartDataLabelCollection dataLabels = chart.Series[0].DataLabels;
            Assert.That(dataLabels.Position, Is.EqualTo(ChartDataLabelPosition.Left));

            CheckDataLabelPosition(dataLabels[0], ChartDataLabelPosition.Left,
                0, ChartDataLabelLocationMode.Offset, 0, ChartDataLabelLocationMode.Offset);
            CheckDataLabelPosition(dataLabels[1], ChartDataLabelPosition.Below,
                0, ChartDataLabelLocationMode.Offset, 0, ChartDataLabelLocationMode.Offset);
            CheckDataLabelPosition(dataLabels[2], ChartDataLabelPosition.Above,
                0, ChartDataLabelLocationMode.Offset, 0, ChartDataLabelLocationMode.Offset);
            CheckDataLabelPosition(dataLabels[3], ChartDataLabelPosition.Right,
                15, ChartDataLabelLocationMode.Offset, 18, ChartDataLabelLocationMode.Offset);
            CheckDataLabelPosition(dataLabels[4], ChartDataLabelPosition.Right,
                -39.75, ChartDataLabelLocationMode.Offset, -15, ChartDataLabelLocationMode.Offset);
        }

        /// <summary>
        /// Tests specifying different location modes for left and top location of a data label.
        /// </summary>
        [Test]
        public void TestDifferentLocationModes()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            foreach (ChartSeries series in chart.Series)
            {
                series.HasDataLabels = true;
                ChartDataLabelCollection dataLabels = series.DataLabels;
                dataLabels.ShowValue = true;
                dataLabels.Position = ChartDataLabelPosition.OutsideEnd;

                foreach (ChartDataLabel dataLabel in dataLabels)
                {
                    dataLabel.LeftMode = ChartDataLabelLocationMode.Offset;
                    dataLabel.Left = 1.5;
                    dataLabel.TopMode = ChartDataLabelLocationMode.Absolute;
                    dataLabel.Top = 93;
                }
            }

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\TestDataLabelDifferentLocationModes.docx");
        }

        /// <summary>
        /// Checks the position properties of the data label.
        /// </summary>
        private static void CheckDataLabelPosition(ChartDataLabel dataLabel, ChartDataLabelPosition expectedPosition,
            double expectedLeft, ChartDataLabelLocationMode expectedLeftMode,
            double expectedTop, ChartDataLabelLocationMode expectedTopMode)
        {
            Assert.That(dataLabel.Position, Is.EqualTo(expectedPosition));
            Assert.That(dataLabel.Left, Is.EqualTo(expectedLeft).Within(0.15));
            Assert.That(dataLabel.LeftMode, Is.EqualTo(expectedLeftMode));
            Assert.That(dataLabel.Top, Is.EqualTo(expectedTop).Within(0.15));
            Assert.That(dataLabel.TopMode, Is.EqualTo(expectedTopMode));
        }

        /// <summary>
        /// Gets a new X value to add to the specified chart series.
        /// </summary>
        private static ChartXValue GetNewXValue(ChartSeries series)
        {
            int valueCount = series.ValueCount;
            Debug.Assert(valueCount > 1);

            switch (series.XValues[0].ValueType)
            {
                case ChartXValueType.String:
                    return ChartXValue.FromString("Category " + valueCount);
                case ChartXValueType.Double:
                    double lastX = series.XValues[valueCount - 1].DoubleValue;
                    return ChartXValue.FromDouble(lastX + (lastX - series.XValues[valueCount - 2].DoubleValue));
                case ChartXValueType.Multilevel:
                    ChartMultilevelValue lastValue = series.XValues[valueCount - 1].MultilevelValue;
                    return ChartXValue.FromMultilevelValue(
                        new ChartMultilevelValue(lastValue.Level1, lastValue.Level2, "Leaf " + valueCount));
                case ChartXValueType.DateTime:
                case ChartXValueType.Time:
                default:
                    throw new InvalidOperationException("Unexpected X value type.");
            }
        }

        /// <summary>
        /// Fills the list with supported data label positions for the specified chart series type. Returns a value
        /// indicating whether the series type allows setting data label position.
        /// </summary>
        private static bool FillSupportedPositions(ChartSeriesType seriesType, List<ChartDataLabelPosition> list)
        {
            list.Clear();

            foreach (ChartDataLabelPosition position in Enum.GetValues(typeof(ChartDataLabelPosition)))
            {
                if (DmlChartUtil.IsDataLabelPositionSupported(seriesType, position))
                    list.Add(position);
            }

            return (list.Count > 0);
        }

        /// <summary>
        /// Checks the <see cref="ChartDataLabel.Orientation"/> and <see cref="ChartDataLabel.Rotation"/>
        /// properties of a data label.
        /// </summary>
        private static void CheckOrientation(ChartDataLabel dataLabel, ShapeTextOrientation expectedOrientation,
            int expectedRotation)
        {
            Assert.That(dataLabel.Orientation, Is.EqualTo(expectedOrientation));
            Assert.That(dataLabel.Rotation, Is.EqualTo(expectedRotation));
        }

        /// <summary>
        /// Checks properties of a <see cref="Font"/> instance.
        /// </summary>
        private static void CheckFont(Font font, string expectedFontName, double expectedFontSize, Color expectedColor,
            Color expectedHighlightColor, bool expectedBold, bool expectedItalic, Underline expectedUnderline,
            int expectedKerning, int expectedSpacing, bool expectedDoubleStrikeThrough, bool expectedStrikeThrough,
            bool expectedAllCaps, bool expectedSmallCaps, RunVerticalAlignment expectedVerticalAlignment,
            int expectedLocaleId)
        {
            TestChartUtil.CheckFontProperties(font, expectedFontName, null, null, null, expectedFontSize,
                expectedKerning, expectedSpacing, expectedBold, expectedItalic, expectedStrikeThrough,
                expectedDoubleStrikeThrough, expectedAllCaps, expectedSmallCaps, expectedColor, expectedHighlightColor,
                expectedUnderline, expectedVerticalAlignment, expectedLocaleId);
        }

        /// <summary>
        /// Changes properties of a <see cref="Font"/> instance.
        /// </summary>
        private static void ChangeFont(Font font, string fontName, double fontSize, Color color, bool bold,
            bool italic, Underline underline, int kerning, int spacing, bool doubleStrikeThrough,
            bool strikeThrough, bool allCaps, bool smallCaps, RunVerticalAlignment verticalAlignment, int localeId)
        {
            TestChartUtil.SetFontProperties(font, fontName, fontSize, kerning, spacing, bold, italic, strikeThrough,
                doubleStrikeThrough, allCaps, smallCaps, color, underline, verticalAlignment, localeId);
        }

        /// <summary>
        /// Checks label count of the series.
        /// </summary>
        private static void CheckLabelCount(ChartSeries series, int expectedCount)
        {
            Assert.That(series.DataLabels.Count, Is.EqualTo(expectedCount));

            // Check enumerator too.
            int count = 0;
            foreach (ChartDataLabel label in series.DataLabels)
                count++;
            Assert.That(count, Is.EqualTo(expectedCount));
        }

        /// <summary>
        /// Creates a document that contains one chart shape.
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
