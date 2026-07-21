// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/25/2014 by Alexey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Simple open/save/compare tests for DOCX document with charts.
    /// Used to make sure charts are written properly from the model.
    /// Use test documents from customers charts rendering tests.
    /// </summary>
    [TestFixture]
    public class TestDmlCustomersCharts
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
        public void TestJira6960()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Customers\TestJira6960.docx");
        }










        [Test]
        public void TestJira7408()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Customers\TestJira7408.docx");
        }



        [Test]
        public void TestJira7657()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Customers\TestJira7657.docx");
        }















        [Test]
        public void TestJira8362()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Customers\TestJira8362.docx");
        }

























        [Test]
        public void TestJira9289()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Customers\TestJira9289.docx");
        }





        [Test]
        public void TestJira9254()
        {
            TestUtil.OpenSaveOpen(@"Model\Charts\Customers\TestJira9254.docx");
        }





















        /// <summary>
        /// Tests removing an embedded XLSX file containing chart data in a <see cref="DmlChartType.ChartExChart"/> chart.
        /// </summary>
        [Test]
        public void TestRemovingEmbeddedXlsxInChartExChart()
        {
            const string fileName = @"Model\Charts\Word2016Charts\BoxWhisker.docx";
            Document doc = TestUtil.Open(fileName);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            Chart chart = shape.Chart;

            chart.ChartSpace.RemoveExternalDataLinkage();

            TestUtil.Save(doc, fileName, null, true, GoldLevel.ExportOnly);

            CheckForNoEmbeddedXlsx(doc);
        }




        /// <summary>
        /// WORDSNET-26356 Font Control in Bar Chart
        /// The customer needs to set font properties for various chart elements. The required features have been added
        /// to the API. And this test generates a chart similar to the one contained in the document that the customer
        /// sent us as an example.
        /// </summary>
        [Test]
        public void Test26356()
        {
            Document doc = new Document();
            doc.FirstSection.PageSetup.PaperSize = PaperSize.A4;
            doc.FirstSection.PageSetup.LeftMargin = 72;

            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertChart(ChartType.Column, 432, 252);
            Chart chart = shape.Chart;

            ChartSeriesCollection series = chart.Series;
            series.Clear();
            double[] xValues = new double[] { 2020, 2021, 2022, 2023 };
            series.Add("Equity Fund", xValues, new double[] { 21.05, 18.89, 1.03, 12.56 });
            series.Add("Debt Fund", xValues, new double[] { 6.01, 7.23, 7.56, 6.05 });
            series.Add("Index", xValues, new double[] { 14.23, 12.32, 8.09, 14.11 });

            foreach (ChartSeries currentSeries in series)
            {
                currentSeries.HasDataLabels = true;
                ChartDataLabelCollection dataLabels = currentSeries.DataLabels;
                dataLabels.ShowSeriesName = false;
                dataLabels.ShowCategoryName = false;
                dataLabels.ShowLegendKey = false;
                dataLabels.ShowValue = true;
                // This just makes dataLabels.TxPr non empty to write the rotation value set below to the document.
                dataLabels.Font.Size = 9;

                // This is not currently supported in API.
                dataLabels.TxPr.BodyPr.Rotation = new DmlAngle(-5400000);
            }

            series[0].Format.Fill.ForeColor = Color.FromArgb(0xff, 0x8f, 0xaa, 0xdc);
            series[1].DataPoints[0].Format.Fill.ForeColor = Color.FromArgb(0xff, 0xbf, 0x90, 0);
            series[2].Format.Fill.ForeColor = Color.FromArgb(0xff, 0x76, 0x71, 0x71);

            ChartTitle title = chart.Title;
            title.Text = "Performance Comparison";
            title.Font.Size = 16;
            title.Font.Bold = true;

            chart.DataTable.Show = true;
            Font dataTableFont = chart.DataTable.Font;
            dataTableFont.Name = "Aptos Display";
            dataTableFont.Bold = true;

            ChartAxisTitle axisXTitle = chart.AxisX.Title;
            axisXTitle.Show = true;
            axisXTitle.Text = "Calendar Year";
            axisXTitle.Font.Size = 14;
            axisXTitle.Font.Italic = true;
            ChartAxisTitle axisYTitle = chart.AxisY.Title;
            axisYTitle.Show = true;
            axisYTitle.Text = "Performance";
            axisYTitle.Font.Size = 12;
            axisYTitle.Font.Italic = true;

            chart.Legend.Position = LegendPosition.Left;

            chart.SeriesGroups[0].Overlap = 0;

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Charts\Customers\Test26356.docx");
        }



        /// <summary>
        /// Checks that the document doesn't have an embedded XLSX file containing chart data.
        /// </summary>
        private static void CheckForNoEmbeddedXlsx(Document doc)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                doc.Save(stream, SaveFormat.Docx);
                stream.Position = 0;
                OpcPackage package = new OpcPackage(stream);
                Assert.That(package.GetPartByName("/word/embeddings/Microsoft_Excel_Worksheet1.xlsx"), Is.Null);
            }
        }
    }
}
