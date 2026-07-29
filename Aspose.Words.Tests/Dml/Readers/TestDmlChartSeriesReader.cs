// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlChartSeriesReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlChartSeriesReader
    {
        [Test]
        public void TestLineChartSeries()
        {
            string xml =
                "<c:ser xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:idx val=\"1\" />" +
                    "<c:order val=\"1\" />" +
                    "<c:tx>" +
                        "<c:strRef>" +
                            "<c:f>Sheet1!$C$1</c:f>" +
                            "<c:strCache><c:ptCount val=\"1\" /><c:pt idx=\"0\"><c:v>Series 2</c:v></c:pt></c:strCache>" +
                        "</c:strRef>" +
                    "</c:tx>" +
                    "<c:marker>" +
                        "<c:symbol val=\"square\" />" +
                        "<c:size val=\"42\" />" +
                        "<c:spPr>" +
                            "<a:blipFill dpi=\"0\" rotWithShape=\"1\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                "<a:blip xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:embed=\"rId1\" />" +
                                "<a:srcRect />" +
                                "<a:tile tx=\"0\" ty=\"0\" sx=\"10000\" sy=\"10000\" flip=\"none\" algn=\"tl\" />" +
                            "</a:blipFill>" +
                        "</c:spPr>" +
                    "</c:marker>" +
                    "<c:dPt>" +
                        "<c:idx val=\"1\" />" +
                        "<c:marker>" +
                            "<c:symbol val=\"diamond\" />" +
                            "<c:size val=\"15\" />" +
                            "<c:spPr>" +
                                "<a:solidFill xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                    "<a:srgbClr val=\"1F497D\"><a:alpha val=\"50000\" /></a:srgbClr>" +
                                "</a:solidFill>" +
                                "<a:scene3d xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                    "<a:camera prst=\"orthographicFront\" />" +
                                    "<a:lightRig rig=\"threePt\" dir=\"t\" />" +
                                "</a:scene3d>" +
                                "<a:sp3d xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"><a:bevelT /></a:sp3d>" +
                            "</c:spPr>" +
                        "</c:marker>" +
                        "<c:spPr>" +
                            "<a:ln cmpd=\"dbl\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                "<a:solidFill><a:srgbClr val=\"FF0000\" /></a:solidFill>" +
                                "<a:prstDash val=\"sysDash\" />" +
                            "</a:ln>" +
                        "</c:spPr>" +
                    "</c:dPt>" +
                    "<c:cat>" +
                        "<c:strRef>" +
                            "<c:f>Sheet1!$A$2:$A$5</c:f>" +
                            "<c:strCache>" +
                                "<c:ptCount val=\"4\" />" +
                                "<c:pt idx=\"0\"><c:v>Category 1</c:v></c:pt>" +
                                "<c:pt idx=\"1\"><c:v>Category 2</c:v></c:pt>" +
                                "<c:pt idx=\"2\"><c:v>Category 3</c:v></c:pt>" +
                                "<c:pt idx=\"3\"><c:v>Category 4</c:v></c:pt>" +
                            "</c:strCache>" +
                        "</c:strRef>" +
                    "</c:cat>" +
                    "<c:val>" +
                        "<c:numRef>" +
                            "<c:f>Sheet1!$C$2:$C$5</c:f>" +
                            "<c:numCache>" +
                                "<c:formatCode>General</c:formatCode>" +
                                "<c:ptCount val=\"4\" />" +
                                "<c:pt idx=\"0\"><c:v>2.4</c:v></c:pt>" +
                                "<c:pt idx=\"1\"><c:v>4.4000000000000004</c:v></c:pt>" +
                                "<c:pt idx=\"2\"><c:v>1.8</c:v></c:pt>" +
                                "<c:pt idx=\"3\"><c:v>2.8</c:v></c:pt>" +
                            "</c:numCache>" +
                        "</c:numRef>" +
                    "</c:val>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:ser>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // C++ workaround extend chart object lifetime
            DmlAreaChart chart = new DmlAreaChart();
            chart.Document = reader.Document;

            ChartSeries series = DmlChartSeriesReader.Build(reader, chart, null);

            Assert.That(series, IsNot.Null());

            Assert.That(series.Index, Is.EqualTo(1));
            Assert.That(series.Order, Is.EqualTo(1));
            Assert.That(series.DefaultDataPoint.Explosion, Is.EqualTo(-1));
            Assert.That(series.DefaultDataPoint.InvertIfNegative, Is.EqualTo(false));
            Assert.That(series.DefaultDataPoint.Bubble3D, Is.EqualTo(false));
            Assert.That(series.Smooth, Is.EqualTo(false));
            Assert.That(series.Shape, Is.EqualTo(BarShape.None));
            Assert.That(series.Size, IsNot.Null());
            Assert.That(series.XErrorBars, Is.Null);
            Assert.That(series.YErrorBars, Is.Null);
            Assert.That(series.DefaultDataPoint.Marker, IsNot.Null());
            Assert.That(series.HasDataLabels, Is.False);
            Assert.That(series.DataLabels.Count, Is.EqualTo(0));
            Assert.That(series.DataPoints[1], IsNot.Null());
            Assert.That(series.DefaultDataPoint.PictureOptions, Is.Null);
            Assert.That(series.Trendlines, IsNot.Null());
            Assert.That(series.Trendlines.Count, Is.EqualTo(0));
            Assert.That(series.Tx, IsNot.Null());
            Assert.That(series.DefaultDataPoint.SpPr, IsNot.Null());

            Assert.That(series.X, IsNot.Null());
            Assert.That(series.X.Values, IsNot.Null());
            Assert.That(series.X.ValueType, Is.EqualTo(DmlChartValueType.String));
            VerifyDataSourceValues(new object[] { "Category 1", "Category 2", "Category 3", "Category 4" }, series.X);

            Assert.That(series.Y, IsNot.Null());
            Assert.That(series.Y.Values, IsNot.Null());
            Assert.That(series.Y.ValueType, Is.EqualTo(DmlChartValueType.Numeric));
            VerifyDataSourceValues(new object[] { 2.4d, 4.4000000000000004d, 1.8d, 2.8d }, series.Y);

            Assert.That(((IDmlExtensionListSource)series).Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestBarChartSeries()
        {
            string xml =
                "<c:ser xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:idx val=\"2\" />" +
                    "<c:order val=\"2\" />" +
                    "<c:tx>" +
                        "<c:strRef>" +
                            "<c:f>Sheet1!$B$1</c:f>" +
                            "<c:strCache><c:ptCount val=\"1\" /><c:pt idx=\"0\"><c:v>Series 1</c:v></c:pt></c:strCache>" +
                         "</c:strRef>" +
                    "</c:tx>" +
                    "<c:dLbls>" +
                        "<c:txPr>" +
                            "<a:bodyPr rot=\"0\" vert=\"eaVert\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                            "<a:lstStyle xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                            "<a:p xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                "<a:pPr><a:defRPr /></a:pPr><a:endParaRPr lang=\"en-US\" />" +
                            "</a:p>" +
                        "</c:txPr>" +
                        "<c:dLblPos val=\"outEnd\" />" +
                        "<c:showLegendKey val=\"1\" />" +
                        "<c:showSerName val=\"1\" />" +
                        "<c:separator>. </c:separator>" +
                    "</c:dLbls>" +
                    "<c:errBars>" +
                        "<c:errBarType val=\"both\" />" +
                        "<c:errValType val=\"stdErr\" />" +
                    "</c:errBars>" +
                    "<c:cat>" +
                        "<c:strRef>" +
                            "<c:f>Sheet1!$A$2:$A$5</c:f>" +
                            "<c:strCache>" +
                                "<c:ptCount val=\"4\" />" +
                                "<c:pt idx=\"0\"><c:v>Category 1</c:v></c:pt>" +
                                "<c:pt idx=\"1\"><c:v>Category 2</c:v></c:pt>" +
                                "<c:pt idx=\"2\"><c:v>Category 3</c:v></c:pt>" +
                                "<c:pt idx=\"3\"><c:v>Category 4</c:v></c:pt>" +
                            "</c:strCache>" +
                        "</c:strRef>" +
                    "</c:cat>" +
                    "<c:val>" +
                        "<c:numRef>" +
                            "<c:f>Sheet1!$B$2:$B$5</c:f>" +
                            "<c:numCache>" +
                                "<c:formatCode>General</c:formatCode>" +
                                "<c:ptCount val=\"4\" />" +
                                "<c:pt idx=\"0\"><c:v>4.3</c:v></c:pt>" +
                                "<c:pt idx=\"1\"><c:v>2.5</c:v></c:pt>" +
                                "<c:pt idx=\"2\"><c:v>3.5</c:v></c:pt>" +
                                "<c:pt idx=\"3\"><c:v>4.5</c:v></c:pt>" +
                            "</c:numCache>" +
                        "</c:numRef>" +
                    "</c:val>" +
                "</c:ser>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // C++ workaround extend chart object lifetime
            DmlAreaChart chart = new DmlAreaChart();
            chart.Document = reader.Document;

            ChartSeries series = DmlChartSeriesReader.Build(reader, chart, null);

            Assert.That(series, IsNot.Null());

            Assert.That(series.Index, Is.EqualTo(2));
            Assert.That(series.Order, Is.EqualTo(2));
            Assert.That(series.DefaultDataPoint.Explosion, Is.EqualTo(-1));
            Assert.That(series.DefaultDataPoint.InvertIfNegative, Is.EqualTo(false));
            Assert.That(series.DefaultDataPoint.Bubble3D, Is.EqualTo(false));
            Assert.That(series.Smooth, Is.EqualTo(false));
            Assert.That(series.Shape, Is.EqualTo(BarShape.None));
            Assert.That(series.Size, IsNot.Null());
            Assert.That(series.YErrorBars, IsNot.Null());
            Assert.That(series.DefaultDataPoint.Marker, IsNot.Null());
            Assert.That(series.HasDataLabels, Is.True);
            Assert.That(series.DataLabels.Count, Is.EqualTo(4));
            Assert.That(series.DataPoints.HasCustomDataPoints, Is.False);
            Assert.That(series.DefaultDataPoint.PictureOptions, Is.Null);
            Assert.That(series.Trendlines, IsNot.Null());
            Assert.That(series.Trendlines.Count, Is.EqualTo(0));
            Assert.That(series.Tx, IsNot.Null());
            Assert.That(series.DefaultDataPoint.SpPr, IsNot.Null());

            Assert.That(series.X, IsNot.Null());
            Assert.That(series.X.Values, IsNot.Null());
            Assert.That(series.X.ValueType, Is.EqualTo(DmlChartValueType.String));
            VerifyDataSourceValues(new object[] { "Category 1", "Category 2", "Category 3", "Category 4" }, series.X);

            Assert.That(series.Y, IsNot.Null());
            Assert.That(series.Y.Values, IsNot.Null());
            Assert.That(series.Y.ValueType, Is.EqualTo(DmlChartValueType.Numeric));
            VerifyDataSourceValues(new object[] { 4.3d, 2.5d, 3.5d, 4.5d }, series.Y);
        }

        private static void VerifyDataSourceValues(object[] expectedValues, DmlChartDimensionData data)
        {
            Assert.That(data.Values.ValueCount, Is.EqualTo(expectedValues.Length));

            for (int i = 0; i < expectedValues.Length; i++)
            {
                DmlChartValue val = data.GetValue(i);
                if (val.ValueType == DmlChartValueType.String)
                    Assert.That(val.StringValue, Is.EqualTo(expectedValues[i]));
                else
                    Assert.That(((DmlChartNumValue)val).Value, Is.EqualTo(expectedValues[i]));
            }
        }
    }
}
