// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlChartComplexTypesReader"/>.
    /// </summary>
    [TestFixture]
    public class TestComplexTypesReader
    {
        [Test]
        public void TestReadChartNumFormat()
        {
            string xml = "<c:numFmt formatCode=\"#,##0.00[$$-C09]\" sourceLinked=\"0\" xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\" />";

            NrxXmlReader reader = new NrxXmlReader(xml, null);

            DmlChartNumFormat numFormat = DmlChartComplexTypesReader.ReadChartNumFormat(reader);

            Assert.That(numFormat, IsNot.Null());
            Assert.That(numFormat.FormatCode, Is.EqualTo("#,##0.00[$$-C09]"));
            Assert.That(numFormat.SourceLinked, Is.False);
        }

        [Test]
        public void TestReadChartScaling()
        {
            string xml =
                "<c:scaling xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:logBase val=\"2\" />" +
                    "<c:orientation val=\"maxMin\" />" +
                    "<c:max val=\"6\" />" +
                    "<c:min val=\"1\" />" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:scaling>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            AxisScaling scaling = new AxisScaling();
            DmlChartComplexTypesReader.ReadChartScaling(reader, scaling);

            Assert.That(scaling, IsNot.Null());
            Assert.That(scaling.Type, Is.EqualTo(AxisScaleType.Logarithmic));
            Assert.That(scaling.LogBase, Is.EqualTo(2.0d));
            Assert.That(scaling.Minimum, Is.EqualTo(new AxisBound(1.0d)));
            Assert.That(scaling.Maximum, Is.EqualTo(new AxisBound(6.0d)));
            Assert.That(scaling.Orientation, Is.EqualTo(AxisOrientation.MaxMin));
            Assert.That(((IDmlExtensionListSource)scaling).Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartDisplayUnits()
        {
            // I cannot make MS Word to write something to dispUnitsLbl so for now test with empty dispUnitsLbl.
            string xml =
                "<c:dispUnits xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:builtInUnit val=\"hundreds\" />" +
                    "<c:dispUnitsLbl />" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:dispUnits>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            AxisDisplayUnit units = DmlChartComplexTypesReader.ReadChartDisplayUnits(null, reader);

            Assert.That(units, IsNot.Null());
            Assert.That(units.Unit, Is.EqualTo(AxisBuiltInUnit.Hundreds));
            Assert.That(((IDmlExtensionListSource)units).Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartLayout()
        {
            string xml =
                "<c:layout xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:manualLayout>" +
                        "<c:layoutTarget val=\"inner\" />" +
                        "<c:xMode val=\"edge\" />" +
                        "<c:yMode val=\"edge\" />" +
                        "<c:x val=\"3.6680154564012864E-2\" />" +
                        "<c:y val=\"0.18294650668666429\" />" +
                        "<c:w val=\"0.8000107538641007\" />" +
                        "<c:h val=\"0.73748343957005369\" />" +
                        "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                    "</c:manualLayout>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:layout>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartManualLayout layout = DmlChartComplexTypesReader.ReadChartLayout(reader);

            Assert.That(layout, IsNot.Null());

            Assert.That(layout.LeftMode, Is.EqualTo(LayoutMode.Edge));
            Assert.That(layout.TopMode, Is.EqualTo(LayoutMode.Edge));
            Assert.That(layout.WidthMode, Is.EqualTo(LayoutMode.Factor));
            Assert.That(layout.HeightMode, Is.EqualTo(LayoutMode.Factor));

            Assert.That(layout.Left, Is.EqualTo(0.036680154564012864d));
            Assert.That(layout.Top, Is.EqualTo(0.18294650668666429d));
            Assert.That(layout.Width, Is.EqualTo(0.8000107538641007d));
            Assert.That(layout.Height, Is.EqualTo(0.73748343957005369d));
            
            Assert.That(layout.LayoutTarget, Is.EqualTo(LayoutTarget.Inner));

            Assert.That(layout.Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartTxPlain()
        {
            string xml =
                "<c:tx xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:strRef>" +
                        "<c:f>Sheet1!$C$1</c:f>" +
                        "<c:strCache><c:ptCount val=\"1\" /><c:pt idx=\"0\"><c:v>Series 2</c:v></c:pt></c:strCache>" +
                    "</c:strRef>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:tx>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartTx tx = DmlChartComplexTypesReader.ReadChartTx(reader);

            Assert.That(tx, IsNot.Null());
            Assert.That(tx.TxType, Is.EqualTo(DmlChartTxType.SeriesText));
            Assert.That(tx.RichText, Is.Null);
            Assert.That(tx.PlainText, Is.EqualTo("Series 2"));
            Assert.That(tx.StrRef, IsNot.Null());
            Assert.That(tx.Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartTxReach()
        {
            string xml =
                "<c:tx xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:rich>" +
                        "<a:bodyPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                        "<a:lstStyle xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                        "<a:p xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                            "<a:pPr><a:defRPr /></a:pPr>" +
                            "<a:r><a:rPr lang=\"en-US\" i=\"1\" strike=\"sngStrike\" /><a:t>Axis Title</a:t></a:r>" +
                        "</a:p>" +
                    "</c:rich>" +
                "</c:tx>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartTx tx = DmlChartComplexTypesReader.ReadChartTx(reader);

            Assert.That(tx, IsNot.Null());
            Assert.That(tx.TxType, Is.EqualTo(DmlChartTxType.ChartText));
            Assert.That(tx.RichText, IsNot.Null());
            Assert.That(tx.PlainText, Is.Null);
            Assert.That(tx.StrRef, Is.Null);
        }

        [Test]
        public void TestReadChartStrRef()
        {
            string xml =
                "<c:strRef xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:f>Sheet1!$A$2:$A$5</c:f>" +
                    "<c:strCache>" +
                        "<c:ptCount val=\"4\" />" +
                        "<c:pt idx=\"0\"><c:v>Category 1</c:v></c:pt>" +
                        "<c:pt idx=\"1\"><c:v>Category 2</c:v></c:pt>" +
                        "<c:pt idx=\"2\"><c:v>Category 3</c:v></c:pt>" +
                        "<c:pt idx=\"3\"><c:v>Category 4</c:v></c:pt>" +
                    "</c:strCache>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:strRef>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartValueRef strRef = DmlChartComplexTypesReader.ReadChartValRef(reader);

            Assert.That(strRef, IsNot.Null());
            Assert.That(strRef.Formula.Value, Is.EqualTo("Sheet1!$A$2:$A$5"));
            Assert.That(strRef.Values, IsNot.Null());
            Assert.That(strRef.Values.ValueType, Is.EqualTo(DmlChartValueType.String));
            Assert.That(strRef.Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartNumRef()
        {
            string xml =
                "<c:numRef xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:f>Sheet1!$C$2:$C$5</c:f>" +
                    "<c:numCache>" +
                        "<c:formatCode>General</c:formatCode>" +
                        "<c:ptCount val=\"4\" />" +
                        "<c:pt idx=\"0\"><c:v>2.4</c:v></c:pt>" +
                        "<c:pt idx=\"1\"><c:v>4.4000000000000004</c:v></c:pt>" +
                        "<c:pt idx=\"2\"><c:v>1.8</c:v></c:pt>" +
                        "<c:pt idx=\"3\"><c:v>2.8</c:v></c:pt>" +
                    "</c:numCache>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:numRef>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartValueRef strRef = DmlChartComplexTypesReader.ReadChartValRef(reader);

            Assert.That(strRef, IsNot.Null());
            Assert.That(strRef.Formula.Value, Is.EqualTo("Sheet1!$C$2:$C$5"));
            Assert.That(strRef.Values, IsNot.Null());
            Assert.That(strRef.Values.ValueType, Is.EqualTo(DmlChartValueType.Numeric));

            Assert.That(((DmlChartNumValue)strRef.Values[0]).Value, Is.EqualTo(2.4d));
            Assert.That(((DmlChartNumValue)strRef.Values[1]).Value, Is.EqualTo(4.4000000000000004d));
            Assert.That(((DmlChartNumValue)strRef.Values[2]).Value, Is.EqualTo(1.8d));
            Assert.That(((DmlChartNumValue)strRef.Values[3]).Value, Is.EqualTo(2.8d));
            Assert.That(strRef.Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartStrCache()
        {
            string xml =
                "<c:strCache xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:ptCount val=\"4\" />" +
                    "<c:pt idx=\"0\"><c:v>Category 1</c:v></c:pt>" +
                    "<c:pt idx=\"1\"><c:v>Category 2</c:v></c:pt>" +
                    "<c:pt idx=\"2\"><c:v>Category 3</c:v></c:pt>" +
                    "<c:pt idx=\"3\"><c:v>Category 4</c:v></c:pt>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:strCache>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartValueCollection cache = new DmlChartValueCollection(DmlChartValueType.String);
            DmlChartComplexTypesReader.ReadChartValuesCache(reader, cache);

            Assert.That(cache, IsNot.Null());
            Assert.That(cache.ValueCount, Is.EqualTo(4));
            Assert.That(cache.Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadStringPoint()
        {
            string xml =
                "<c:pt idx=\"2\" xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:v>Category 3</c:v>" +
                "</c:pt>";

            NrxXmlReader reader = new NrxXmlReader(xml, null);

            DmlChartValue pt = DmlChartComplexTypesReader.ReadChartValPoint(reader, DmlChartValueType.String);

            Assert.That(pt, IsNot.Null());
            Assert.That(pt.Index, Is.EqualTo(2));
            Assert.That(pt.StringValue, Is.EqualTo("Category 3"));
        }

        [Test]
        public void TestReadMarker()
        {
            string xml =
                "<c:marker xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:symbol val=\"square\" />" +
                    "<c:size val=\"42\" />" +
                    "<c:spPr>" +
                        "<a:blipFill dpi=\"0\" rotWithShape=\"1\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                            "<a:blip xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" r:embed=\"rId1\" />" +
                            "<a:srcRect />" +
                            "<a:tile tx=\"100\" ty=\"200\" sx=\"10000\" sy=\"10000\" flip=\"none\" algn=\"tl\" />" +
                        "</a:blipFill>" +
                    "</c:spPr>" +
                "</c:marker>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            ChartMarker marker = new ChartMarker(reader.Document.GetThemeInternal());
            DmlChartComplexTypesReader.ReadChartMarker(reader, marker);

            Assert.That(marker, IsNot.Null());
            Assert.That(marker.Size, Is.EqualTo(42));
            Assert.That(marker.Symbol, Is.EqualTo(MarkerSymbol.Square));
            Assert.That(marker.SpPr, IsNot.Null());
            Assert.That(marker.SpPr.Fill, IsNot.Null());
            DmlBlipFillTile tile = (DmlBlipFillTile)((DmlBlipFill)marker.SpPr.Fill).BlipFillMode;
            Assert.That(tile.HorizontalOffset, Is.EqualTo(100d));
            Assert.That(tile.VerticalOffset, Is.EqualTo(200d));
        }

        /// <summary>
        /// Tests reading the CT_Marker complex type from ISO Strict format.
        /// Also checks reading values of the ST_UniversalMeasure simple type.
        /// </summary>
        [Test]
        public void TestReadMarkerAsIsoStrict()
        {
            string xml =
                "<c:marker xmlns:c=\"http://purl.oclc.org/ooxml/drawingml/chart\">" +
                    "<c:symbol val=\"square\" />" +
                    "<c:size val=\"42\" />" +
                    "<c:spPr>" +
                        "<a:blipFill dpi=\"0\" rotWithShape=\"1\" xmlns:a=\"http://purl.oclc.org/ooxml/drawingml/main\">" +
                            "<a:blip xmlns:r=\"http://purl.oclc.org/ooxml/officeDocument/relationships\" r:embed=\"rId1\" />" +
                            "<a:srcRect />" +
                            "<a:tile tx=\"1pt\" ty=\"0.5pc\" sx=\"10000\" sy=\"10000\" flip=\"none\" algn=\"tl\" />" +
                        "</a:blipFill>" +
                    "</c:spPr>" +
                "</c:marker>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            ChartMarker marker = new ChartMarker(reader.Document.GetThemeInternal());
            DmlChartComplexTypesReader.ReadChartMarker(reader, marker);

            Assert.That(marker, IsNot.Null());
            Assert.That(marker.Size, Is.EqualTo(42));
            Assert.That(marker.Symbol, Is.EqualTo(MarkerSymbol.Square));
            Assert.That(marker.SpPr, IsNot.Null());
            Assert.That(marker.SpPr.Fill, IsNot.Null());
            DmlBlipFillTile tile = (DmlBlipFillTile)((DmlBlipFill)marker.SpPr.Fill).BlipFillMode;
            Assert.That(tile.HorizontalOffset, Is.EqualTo(12700d));
            Assert.That(tile.VerticalOffset, Is.EqualTo(76200d));
        }

        [Test]
        public void TestReadDataPoint()
        {
            string xml =
                "<c:dPt xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
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
                            "<a:sp3d xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                                "<a:bevelT />" +
                            "</a:sp3d>" +
                        "</c:spPr>" +
                    "</c:marker>" +
                    "<c:spPr>" +
                        "<a:ln cmpd=\"dbl\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                            "<a:solidFill><a:srgbClr val=\"FF0000\" /></a:solidFill>" +
                            "<a:prstDash val=\"sysDash\" />" +
                        "</a:ln>" +
                    "</c:spPr>" +
                "</c:dPt>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChart chart = new DmlAreaChart();
            chart.Document = reader.Document;
            ChartDataPoint pt = DmlChartComplexTypesReader.ReadChartDataPoint(reader, chart);

            Assert.That(pt, IsNot.Null());
            Assert.That(pt.Index, Is.EqualTo(1));
            Assert.That(pt.Marker, IsNot.Null());
            Assert.That(pt.SpPr, IsNot.Null());
        }

        [Test]
        public void TestReadChartDataLabelCollection()
        {
            string xml =
                "<c:dLbls xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:txPr>" +
                        "<a:bodyPr rot=\"0\" vert=\"eaVert\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                        "<a:lstStyle xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" />" +
                        "<a:p xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"><a:pPr><a:defRPr /></a:pPr><a:endParaRPr lang=\"en-US\" /></a:p>" +
                    "</c:txPr>" +
                    "<c:dLblPos val=\"outEnd\" />" +
                    "<c:showLegendKey val=\"1\" />" +
                    "<c:showSerName val=\"1\" />" +
                    "<c:separator>. </c:separator>" +
                "</c:dLbls>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlBarChart chart = new DmlBarChart();
            chart.Document = reader.Document;
            ChartDataLabelCollection lbls = DmlChartComplexTypesReader.ReadChartDataLabelCollection(reader, chart, null);

            Assert.That(lbls, IsNot.Null());
            Assert.That(lbls.LabelPr.GetProperty(DmlChartDataLabelAttrs.DLblPos), Is.EqualTo(ChartDataLabelPosition.OutsideEnd));
            Assert.That(lbls.LabelPr.GetProperty(DmlChartDataLabelAttrs.ShowLegendKey), Is.EqualTo(true));
            Assert.That(lbls.LabelPr.GetProperty(DmlChartDataLabelAttrs.ShowSerName), Is.EqualTo(true));
            Assert.That(lbls.LabelPr.GetProperty(DmlChartDataLabelAttrs.Separator), Is.EqualTo(". "));
        }

        [Test]
        public void TestReadChartErrorBars()
        {
            string xml =
                "<c:errBars xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:errBarType val=\"both\" />" +
                    "<c:errValType val=\"fixedVal\" />" +
                    "<c:val val=\"0.1\" />" +
                    "<c:spPr>" +
                        "<a:ln xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                            "<a:solidFill><a:schemeClr val=\"tx1\" /></a:solidFill>" +
                        "</a:ln>" +
                    "</c:spPr>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:errBars>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartErrorBars errorBars = DmlChartComplexTypesReader.ReadChartErrorBars(reader);

            Assert.That(errorBars, IsNot.Null());
            Assert.That(errorBars.ErrValType, Is.EqualTo(ErrorValueType.FixedValue));
            Assert.That(errorBars.ErrBarType, Is.EqualTo(ErrorBarType.Both));
            Assert.That(errorBars.Val, Is.EqualTo(0.1d));
            Assert.That(errorBars.SpPr, IsNot.Null());
            Assert.That(errorBars.Plus, IsNot.Null());
            Assert.That(errorBars.Minus, IsNot.Null());
            Assert.That(errorBars.Extensions.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestReadChartTrendline()
        {
            string xml =
                "<c:trendline xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\">" +
                    "<c:name>test</c:name>" +
                    "<c:trendlineType val=\"linear\" />" +
                    "<c:forward val=\"2\" />" +
                    "<c:backward val=\"3\" />" +
                    "<c:intercept val=\"4\" />" +
                    "<c:dispRSqr val=\"1\" />" +
                    "<c:dispEq val=\"1\" />" +
                    "<c:trendlineLbl><c:numFmt formatCode=\"General\" sourceLinked=\"0\" /></c:trendlineLbl>" +
                    "<c:extLst xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"> " +
                        "<a:ext uri=\"{05A4C25C-085E-4340-85A3-A5531E510DB2}\"> " +
                            "<thm15:themeFamily id=\"{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}\" " +
                                "name=\"Office Theme\" vid=\"{4A3C46E8-61CC-4603-A589-7422A47A8E4A}\" " +
                                "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" /> " +
                        "</a:ext> " +
                    "</c:extLst> " +
                "</c:trendline>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            DmlChartTrendline trendline = DmlChartComplexTypesReader.ReadChartTrendline(reader);

            Assert.That(trendline, IsNot.Null());
            Assert.That(trendline.TrendlineLbl, IsNot.Null());
            Assert.That(trendline.TrendlineType, Is.EqualTo(TrendlineType.Linear));
            Assert.That(trendline.Forward, Is.EqualTo(2));
            Assert.That(trendline.Backward, Is.EqualTo(3));
            Assert.That(trendline.Intercept, Is.EqualTo(4));
            Assert.That(trendline.DispEq, Is.True);
            Assert.That(trendline.DispRSqr, Is.True);
            Assert.That(trendline.Extensions.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Checks the reading of the "scene3d" and "sp3d".
        /// </summary>
        [Test]
        public void TestRead3DChartSpPr()
        {
            string xml = "<c:spPr xmlns:c=\"http://schemas.openxmlformats.org/drawingml/2006/chart\"" +
                   " xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" >" +
                "<a:scene3d>" +
                  "<a:camera prst=\"orthographicFront\"/>" +
                  "<a:lightRig rig=\"threePt\" dir=\"t\"/>" +
                "</a:scene3d>" +
                "<a:sp3d contourW=\"25400\">" +
                  "<a:contourClr>" +
                    "<a:schemeClr val=\"lt1\"/>" +
                  "</a:contourClr>" +
                "</a:sp3d>" +
                "<c:extLst xmlns:c16r2=\"http://schemas.microsoft.com/office/drawing/2015/06/chart\">" +
                    "<c:ext uri=\"{C3380CC4-5D6E-409C-BE32-E72D297353CC}\""+
                            " xmlns:c16=\"http://schemas.microsoft.com/office/drawing/2014/chart\">" +
                        "<c16:uniqueId val=\"{00000001-84B0-46BC-8D9F-C0CDE4AB9DEC}\"/>" +
                    "</c:ext>" +
                "</c:extLst>" +
              "</c:spPr>";

            DmlChartSpPr spPr = new DmlChartSpPr();
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            DmlChartComplexTypesReader.ReadChartSpPr(reader, spPr);

            // Just check that reading was made.
            Assert.That(spPr.Extensions.Count, Is.EqualTo(1));
            Assert.That(spPr.Shape3DProp.ContourWidth, Is.EqualTo(25400));            
            Assert.That(spPr.Scene3DProp.Camera.PresetCameraType, Is.EqualTo(DmlPresetCameraType.OrthographicFront));
        }

     
    }
}
