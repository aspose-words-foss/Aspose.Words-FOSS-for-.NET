// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/12/2015 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Aspose.Common;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Text.Bullets;
using Aspose.Words.Fonts;
using Aspose.Words.Loading;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Styles;
using Aspose.Words.Tables;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export.Docx
{
    /// <summary>
    /// Tests exporting to Strict Open XML Document format (ISO/IEC 29500:2012 Strict).
    /// </summary>
    [TestFixture]
    public class TestExportIsoStrictDocx
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests that if a document is loaded from ISO Strict format, it is saved as ISO Strict by default.
        /// </summary>
        [Test]
        public void TestImplicitIsoStrict()
        {
            Document doc = TestUtil.Open(@"ImportIso29500S\TestJira8049.docx");
            Assert.That(doc.ComplianceInfo.Compliance, Is.EqualTo(OoxmlComplianceCore.IsoStrict));

            // OoxmlComplianceInfo.GetCompliance() is used internally to get compliance value.
            Assert.That(OoxmlComplianceInfo.GetCompliance(doc.ComplianceInfo, new OoxmlSaveOptions()), Is.EqualTo(OoxmlComplianceCore.IsoStrict));

            DocxExportContext context = new DocxExportContext(doc, DocumentPart, new OoxmlSaveOptions());

            CheckForIsoStrict(context);
        }

        /// <summary>
        /// Checks that the document that is stored in the specified context is a Strict OOXML document.
        /// </summary>
        /// <param name="context"></param>
        private static void CheckForIsoStrict(DocxExportContext context)
        {
            XmlNode node = context.GetXmlNode("//w:document/@w:conformance");
            Assert.That(node, IsNot.Null());
            Assert.That(node.Value, Is.EqualTo("strict"));
        }

        /// <summary>
        /// Tests that the 'conformance' attribute of the 'document' element (§17.2.3) has the 'strict' value on saving
        /// as ISO Strict.
        /// </summary>
        [Test]
        public void TestDocumentConformanceAttribute()
        {
            DocxExportContext context = new DocxExportContext(new Document(), OoxmlComplianceCore.IsoStrict);

            CheckForIsoStrict(context);

            OoxmlSaveOptions saveOptions = CreateStrictSaveOptions();
            context = new DocxExportContext(new Document(), DocumentPart, saveOptions);

            CheckForIsoStrict(context);
        }

        /// <summary>
        /// Tests that the 'compatibilityMode' compatibility option has value 15.
        /// as ISO Strict.
        /// </summary>
        [Test]
        public void TestCompatibilityMode()
        {
            DocxExportContext context = new DocxExportContext(new Document(), SettingsPart,
                OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode("//w:compat/w:compatSetting[@w:name = 'compatibilityMode']");
            Assert.That(node, IsNot.Null());
            Assert.That(node.Attributes["w:val"].Value, Is.EqualTo("15"));
        }

        /// <summary>
        /// Tests that new attributes of the tblLook element (17.4.54 and 17.4.55 of ISO/IEC 29500-1:2012) are used
        /// on saving as Strict Open XML Document.
        /// </summary>
        [Test]
        public void TestTableStyleOptions()
        {
            DocxExportContext context =
                new DocxExportContext(TestUtil.Open(@"ExportDocx\TestTableBordersAndMargins.docx"),
                        OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode("descendant::w:tblLook");
            Assert.That(node, IsNot.Null());
            Assert.That(node.Attributes["w:val"], Is.Null, "The val attribute is written in the tblLook element on saving as DOCX ISO Strict.");
        }

        /// <summary>
        /// Tests that the following new ISO/IEC 29500-1:2012 elements are supported on writing:
        /// start (17.4.33, 17.4.34, 17.4.35, 17.4.36) and end (17.4.10, 17.4.11, 17.4.12, 17.4.13).
        /// </summary>
        [Test]
        public void TestTableBordersAndMargins()
        {
            DocxExportContext context =
                new DocxExportContext(TestUtil.Open(@"ExportDocx\TestTableBordersAndMargins.docx"),
                        OoxmlComplianceCore.IsoStrict);

            CheckStartEndElements(context, "tblBorders");
            CheckStartEndElements(context, "tcBorders");
            CheckStartEndElements(context, "tblCellMar");
            CheckStartEndElements(context, "tcMar");
        }

        /// <summary>
        /// Searches for the specified parent element and checks that it contains the start and end elements.
        /// </summary>
        private static void CheckStartEndElements(DocxExportContext context, string parentElement)
        {
            const string errMsg = "'{0}' element is not written in '{1}'.";

            XmlNode parentNode = context.GetXmlNode("descendant::w:" + parentElement);
            Assert.That(parentNode, IsNot.Null());
            XmlNode node = context.GetXmlNode((XmlElement)parentNode, "descendant::w:start");
            Assert.That(node, IsNot.Null(), string.Format(errMsg, "start", parentElement));
            node = context.GetXmlNode((XmlElement)parentNode, "descendant::w:end");
            Assert.That(node, IsNot.Null(), string.Format(errMsg, "end", parentElement));
        }

        /// <summary>
        /// Tests conversion of TextOrientation values to string and backward.
        /// </summary>
        [TestCase(TextOrientation.Upward, "btLr", "lr")]
        [TestCase(TextOrientation.Horizontal, "lrTb", "tb")]
        [TestCase(TextOrientation.HorizontalRotatedFarEast, "lrTbV", "tbV")]
        [TestCase(TextOrientation.VerticalFarEast, "tbRlV", "rlV")]
        [TestCase(TextOrientation.Downward, "tbRl", "rl")]
        [TestCase(TextOrientation.VerticalRotatedFarEast, "tbLrV", "lrV")]
        public void TestTextOrientationConversion(TextOrientation value, string asEcma, string asIsoStrict)
        {
            Assert.That(StyleConvertUtil.TextOrientationToXml(value, true, OoxmlComplianceCore.Ecma376), Is.EqualTo(asEcma), "Conversion to string (ECMA-376) is wrong.");
            Assert.That(StyleConvertUtil.TextOrientationToXml(value, true, OoxmlComplianceCore.IsoStrict), Is.EqualTo(asIsoStrict), "Conversion to string (ISO Strict) is wrong.");

            OoxmlComplianceInfo complianceInfo = new OoxmlComplianceInfo();
            Assert.That(StyleConvertUtil.XmlToTextOrientation(asEcma, complianceInfo), Is.EqualTo(value), "Conversion from string (ECMA-376) is wrong.");
            Assert.That(complianceInfo.Compliance, Is.EqualTo(OoxmlComplianceCore.Ecma376), "Wrong compliance value.");

            Assert.That(StyleConvertUtil.XmlToTextOrientation(asIsoStrict, complianceInfo), Is.EqualTo(value), "Conversion from string (ISO Strict) is wrong.");
            Assert.That(complianceInfo.Compliance, Is.EqualTo(OoxmlComplianceCore.IsoTransitional), "Wrong compliance value.");
        }

        /// <summary>
        /// Tests that new values are supported on writing attributes of the ST_TextDirection simple type (§17.18.93)
        /// when saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestTextOrientation()
        {
            DocxExportContext context = new DocxExportContext(TestUtil.Open(@"ExportDocx\TestTextOrientation.docx"),
                OoxmlComplianceCore.IsoStrict);

            IList<XmlNode> nodes = context.GetXmlNodes("descendant::w:textDirection");
            bool hasNodes = false;
            foreach (XmlNode node in nodes)
            {
                hasNodes = true;
                string textOrientation = node.Attributes["w:val"].Value;
                if (!IsIsoStrictTextOrientation(textOrientation))
                    Assert.Fail(string.Format("Wrong text orientation value is written to document: {0}.", textOrientation));
            }
            Assert.That(hasNodes, Is.True);
        }

        /// <summary>
        /// Returns true if the specified text orientation is allowed value by ISO/IEC 29500-1:2012.
        /// </summary>
        private static bool IsIsoStrictTextOrientation(string value)
        {
            return
                value == "tb" ||
                value == "rl" ||
                value == "lr" ||
                value == "tbV" ||
                value == "rlV" ||
                value == "lrV";
        }

        /// <summary>
        /// Tests that new values are supported on writing ISO/IEC 29500-1:2012 elements:
        /// 17.3.1.13 jc (Paragraph Alignment) and 17.4.27 jc (Table Row Alignment).
        /// </summary>
        [Test]
        public void TestParagraphAlignment()
        {
            DocxExportContext context =
                new DocxExportContext(TestUtil.Open(@"ExportDocx\TestParagraphAlignment.docx"),
                        OoxmlComplianceCore.IsoStrict);

            // Check of 17.3.1.13 jc (Paragraph Alignment)
            XmlNode node = context.GetXmlNode("descendant::w:jc");
            Assert.That(node, IsNot.Null());
            Assert.That(node.ParentNode.LocalName, Is.EqualTo("pPr"));
            Assert.That(node.Attributes["w:val"].Value, Is.EqualTo("end"), "Wrong paragraph alignment value.");

            // Check of 17.4.27 jc (Table Row Alignment)
            node = context.GetXmlNode("descendant::w:tblPr");
            Assert.That(node, IsNot.Null());
            node = context.GetXmlNode((XmlElement)node, "descendant::w:jc");
            Assert.That(node, IsNot.Null());
            Assert.That(node.ParentNode.LocalName, Is.EqualTo("tblPr"));
            Assert.That(node.Attributes["w:val"].Value, Is.EqualTo("end"), "Wrong table row alignment value.");
        }


        /// <summary>
        /// Tests that percentage type values are written with percent sign on saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestPercentageValues()
        {
            DocxExportContext context = new DocxExportContext(TestUtil.Open(@"ExportDocx\TestPercentageValues.docx"),
                OoxmlComplianceCore.IsoStrict);

            CheckAttributeValue(context, "//a:gs/@pos", "10%");
            CheckAttributeValue(context, "//a:alpha/@val", "80%");
            CheckAttributeValue(context, "//a:lumMod/@val", "10%");
            CheckAttributeValue(context, "//a:lumOff/@val", "90%");
            CheckAttributeValue(context, "//a:shade/@val", "50%");
            CheckAttributeValue(context, "//a:outerShdw/@sx", "80%");
            CheckAttributeValue(context, "//a:outerShdw/@sy", "80%");
            CheckAttributeValue(context, "//a:reflection/@stA", "45%");
            CheckAttributeValue(context, "//a:reflection/@endPos", "27%");
            CheckAttributeValue(context, "//a:reflection/@sy", "-100%");
        }

        /// <summary>
        /// Tests that attributes of the 20.1.2.3.30 scrgbClr (RGB Color Model - Percentage Variant) element with
        /// zero values are written with percent sign on saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestZeroPercentageRgbColor()
        {
            DocxExportContext context = new DocxExportContext(TestUtil.Open(@"Model\DrawingML\TestBlip.docx"),
                OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode("//a:style/a:lnRef/a:scrgbClr");
            CheckAttributeValue(node, "r", "0%");
            CheckAttributeValue(node, "g", "0%");
            CheckAttributeValue(node, "b", "0%");
        }

        /// <summary>
        /// Tests that the 21.3.2.21 relSizeAnchor (Relative Anchor Shape Size) element is written with correct
        /// namespace on saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestNamespaceOfRelativeAnchorShapeSize()
        {
            DocxExportContext context = new DocxExportContext(
                TestUtil.Open(@"Model\DrawingML\TestChartDrawingBlipAndBlipFill.docx"),
                "word/drawings/drawing1.xml", OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.XmlDocument.ChildNodes[Index].ChildNodes[0];
            Assert.That(node.LocalName, Is.EqualTo("relSizeAnchor"));
            Assert.That(node.NamespaceURI, Is.EqualTo(DmlUrlPrefix + "chartDrawing"));
        }

        /// <summary>
        /// Tests that percentage values of the elements: 2.18.1.1 pctPosHOffset, 2.18.1.2 pctPosVOffset,
        /// pctWidth of 2.18.3.1 CT_SizeRelH and pctHeight of 2.18.3.2 CT_SizeRelV [MS-ODRAWXML] are written
        /// in correct format on saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestWordprocessingDrawingElements()
        {
            const string fileName = @"ExportDocx\TestJira10070.docx";
            Document doc = TestUtil.Open(fileName);
            DocxExportContext context = new DocxExportContext(doc, OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode("//wp14:pctPosHOffset");
            Assert.That(node.InnerText, Is.EqualTo("44%"));
            node = context.GetXmlNode("//wp14:pctPosVOffset");
            Assert.That(node.InnerText, Is.EqualTo("2.5%"));
            node = context.GetXmlNode("//wp14:pctWidth");
            Assert.That(node.InnerText, Is.EqualTo("40%"));
            node = context.GetXmlNode("//wp14:pctHeight");
            Assert.That(node.InnerText, Is.EqualTo("70%"));

            doc = TestUtil.SaveOpen(doc, fileName,
                OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoStrict), false);
            ShapePr shapePr = doc.FirstSection.Body.Shapes[0].ShapePr;

            Assert.That((int)shapePr[ShapeAttr.LeftPercent], Is.EqualTo(440));
            Assert.That((int)shapePr[ShapeAttr.TopPercent], Is.EqualTo(25));
            Assert.That((int)shapePr[ShapeAttr.WidthPercent], Is.EqualTo(400));
            Assert.That((int)shapePr[ShapeAttr.HeightPercent], Is.EqualTo(700));
        }

        /// <summary>
        /// Searches for attribute node that is specified by parent element and XPath. Checks that value of
        /// the found attribute equals to the expected value.
        /// </summary>
        private static void CheckAttributeValue(DocxExportContext context, XmlElement parentNode, string xPath,
            string expectedValue)
        {
            XmlNode node = context.GetXmlNode(parentNode, xPath);
            Assert.That(node, IsNot.Null(), string.Format("Cannot find the node '{0}'.", xPath));
            Assert.That(node, Is.InstanceOf(typeof(XmlAttribute)), string.Format("Wrong type of the node '{0}'.", xPath));
            Assert.That(node.Value, Is.EqualTo(expectedValue), string.Format("Value of the node '{0}' is wrong.", xPath));
        }

        /// <summary>
        /// Searches for attribute node that is specified by XPath from the document root. Checks that value of
        /// the found attribute equals to the expected value.
        /// </summary>
        private static void CheckAttributeValue(DocxExportContext context, string xPath, string expectedValue)
        {
            CheckAttributeValue(context, context.XmlDocument.DocumentElement, xPath, expectedValue);
        }

        /// <summary>
        /// Gets attribute of the node and checks that its value equals to <see cref="expectedValue"/>.
        /// </summary>
        private static void CheckAttributeValue(XmlNode node, string attributeName, string expectedValue)
        {
            XmlAttribute attribute = (XmlAttribute)node.Attributes[attributeName];//casting for java
            Assert.That(attribute, IsNot.Null());
            Assert.That(attribute.Value, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Tests that value of the 'percent' attribute on the 'zoom' element is written in format with percent sign
        /// on saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestZoomValue()
        {
            Document doc = new Document();
            doc.ViewOptions.ZoomPercent = 75;
            DocxExportContext context = new DocxExportContext(doc, SettingsPart, OoxmlComplianceCore.IsoStrict);

            CheckAttributeValue(context, "//w:zoom/@w:percent", "75%");
        }


        /// <summary>
        /// Tests that the minVer attribute of the 2.10.1.1 dataModelExt [MS-ODRAWXML] element has transitional
        /// diagram drawing namespace as value on saving as ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestDataModelExtension()
        {
            DocxExportContext context =
                new DocxExportContext(TestUtil.Open(@"Model\TextEffects\TestTextEffectFillSmartArt.docx"),
                    "word/diagrams/data1.xml", OoxmlComplianceCore.IsoStrict);

            CheckAttributeValue(context, "//dsp:dataModelExt/@minVer",
                "http://schemas.openxmlformats.org/drawingml/2006/diagram");
        }

        /// <summary>
        /// Tests that buSzPct (Bullet Size Percentage) element is written correctly.
        /// </summary>
        [TestCase(false, "200000")]
        [TestCase(true, "200%")]
        public void TestWritingBulletSize(bool isIsoStrict, string expectedPercentValue)
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TextBox\TestDmlListsBulletSize.docx");
            DocxExportContext context = new DocxExportContext(doc, DocumentPart,
                (isIsoStrict ? OoxmlComplianceCore.IsoStrict : OoxmlComplianceCore.Ecma376));
            CheckAttributeValue(context, "//a:buSzPct/@val", expectedPercentValue);
        }

        /// <summary>
        /// Tests that buSzPct (Bullet Size Percentage) element is read correctly.
        /// </summary>
        [Test]
        public void TestReadingBulletSize()
        {
            const string docFileName = @"Model\DrawingML\TextBox\TestDmlListsBulletSize.docx";
            Document doc = TestUtil.Open(docFileName);

            CheckBulletSize(GetTextShapeParagraph(doc, 0, 2), DmlTextBulletSizeType.Percentage, 200000d);

            // Reading from ISO 29500 Strict
            OoxmlSaveOptions options = new OoxmlSaveOptions();
            options.ComplianceCore = OoxmlComplianceCore.IsoStrict;
            doc = TestUtil.SaveOpen(doc, docFileName, options, false);

            CheckBulletSize(GetTextShapeParagraph(doc, 0, 2), DmlTextBulletSizeType.Percentage, 200000d);
        }

        /// <summary>
        /// Checks bullet size of DML paragraph.
        /// </summary>
        private static void CheckBulletSize(DmlParagraph paragraph, DmlTextBulletSizeType expectedSizeType,
            double expectedSize)
        {
            DmlTextBulletSize bulletSize = paragraph.Properties.BulletSize;
            Assert.That(bulletSize.SizeType, Is.EqualTo(expectedSizeType));
            Assert.That(bulletSize.Value, Is.EqualTo(expectedSize));
        }

        /// <summary>
        /// Gets paragraph of DML text shape that are specified by their indexes.
        /// </summary>
        private static DmlParagraph GetTextShapeParagraph(Document doc, int shapeIndex, int paraIndex)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIndex, true);
            DmlShape dmlShape = (DmlShape)shape.DmlNode;
            return (DmlParagraph)dmlShape.TextShape.TextBody.Paragraphs[paraIndex];
        }

        /// <summary>
        /// Tests that ds (Dash Stop) element is written correctly.
        /// </summary>
        [TestCase(false, "10000", "5000")]
        [TestCase(true, "10%", "5%")]
        public void TestDashStop(bool isIsoStrict, string expectedDash, string expectedSpace)
        {
            Document doc = TestUtil.Open(@"ExportDocx\TestDashStop.docx");

            DmlParagraph paragraph = GetTextShapeParagraph(doc, 0, 0);
            DmlRun run = (DmlRun)paragraph.Elements[0];
            DmlDashStop dashStop = ((DmlCustomDash)run.RunProperties.Outline.Dash).DashStops[0];
            Assert.That(dashStop.DashLength, Is.EqualTo(0.1d));
            Assert.That(dashStop.SpaceLength, Is.EqualTo(0.05d));

            DocxExportContext context = new DocxExportContext(doc, DocumentPart,
                (isIsoStrict ? OoxmlComplianceCore.IsoStrict : OoxmlComplianceCore.Ecma376));
            CheckAttributeValue(context, "//a:ds/@d", expectedDash);
            CheckAttributeValue(context, "//a:ds/@sp", expectedSpace);
        }

        /// <summary>
        /// Tests that elements of the CT_TblWidth simple type are read and written correctly.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public void TestReadWriteOfTableWidth(bool isIsoStrict)
        {
            Document testDoc = new Document();
            DocumentBuilder builder = new DocumentBuilder(testDoc);

            Table table1 = InsertTable(builder);
            SetPreferredWidth(table1, PreferredWidth.FromPointsSafe(720));
            Table table2 = InsertTable(builder);
            SetPreferredWidth(table2, PreferredWidth.FromPercentSafe(60));
            Table table3 = InsertTable(builder);
            SetPreferredWidth(table3, PreferredWidth.FromRawSafe(PreferredWidthType.Auto, 3600));

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.ComplianceCore = isIsoStrict ? OoxmlComplianceCore.IsoStrict : OoxmlComplianceCore.Ecma376;
            Document tmpDoc =
                TestUtil.SaveOpen(testDoc, @"ExportDocx\TestReadWriteOfTableWidth.docx", saveOptions, false);
            NodeCollection tables = tmpDoc.GetChildNodes(NodeType.Table, true);
            Assert.That(tables.Count, Is.EqualTo(3));

            CheckPreferredWidth(table1, (Table)tables[0], "table1");
            CheckPreferredWidth(table2, (Table)tables[1], "table2");
            CheckPreferredWidth(table3, (Table)tables[2], "table3");
        }

        /// <summary>
        /// Sets preferred width to the table.
        /// </summary>
        private static void SetPreferredWidth(Table table, PreferredWidth width)
        {
            table.FirstRow.TablePr.PreferredWidth = width;
        }

        /// <summary>
        /// Checks that the first rows of the specified tables have equal preferred widths.
        /// </summary>
        private static void CheckPreferredWidth(Table expectedTable, Table actualTable, string tableName)
        {
            PreferredWidth expected = expectedTable.FirstRow.TablePr.PreferredWidth;
            PreferredWidth actual = actualTable.FirstRow.TablePr.PreferredWidth;

            Assert.That(actual.Type, Is.EqualTo(expected.Type), string.Format("Wrong value type ({0}).", tableName));
            Assert.That(actual.ValueRaw, Is.EqualTo(expected.ValueRaw), string.Format("Wrong value ({0}).", tableName));
        }

        /// <summary>
        /// Tests that elements of the CT_TblWidth simple type are written correctly on saving as
        /// ISO/IEC 29500-1:2012 format.
        /// </summary>
        [Test]
        public void TestTableWidthValues()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table1 = InsertTable(builder);
            SetPreferredWidth(table1, PreferredWidth.FromPointsSafe(720));
            Table table2 = InsertTable(builder);
            SetPreferredWidth(table2, PreferredWidth.FromPercentSafe(60));
            Table table3 = InsertTable(builder);
            SetPreferredWidth(table3,
                PreferredWidth.FromRawSafe(PreferredWidthType.Auto, 3600)); // 3600 twips = 180 points

            table1.TopPadding = 20;
            Cell cell = table1.GetCell(0, 0);
            cell.CellPr.TopPadding = 300; // 300 twips = 15 points

            DocxExportContext context = new DocxExportContext(doc, DocumentPart, OoxmlComplianceCore.IsoStrict);
            IList<XmlNode> nodes = context.GetXmlNodes("//w:tr/w:tblPrEx/w:tblW");
            Assert.That(nodes.Count, Is.EqualTo(3));

            CheckAttributeValue(context, (XmlElement)nodes[0], "@w:w", "720pt");
            CheckAttributeValue(context, (XmlElement)nodes[0], "@w:type", "dxa");
            CheckAttributeValue(context, (XmlElement)nodes[1], "@w:w", "60%");
            CheckAttributeValue(context, (XmlElement)nodes[1], "@w:type", "pct");
            CheckAttributeValue(context, (XmlElement)nodes[2], "@w:w", "180pt");
            CheckAttributeValue(context, (XmlElement)nodes[2], "@w:type", "auto");

            XmlElement node = (XmlElement)context.GetXmlNode("//w:tblCellMar/w:top");
            Assert.That(node, IsNot.Null());
            CheckAttributeValue(context, node, "@w:w", "20pt");
            CheckAttributeValue(context, node, "@w:type", "dxa");

            node = (XmlElement)context.GetXmlNode("//w:tcMar/w:top");
            Assert.That(node, IsNot.Null());
            CheckAttributeValue(context, node, "@w:w", "15pt");
            CheckAttributeValue(context, node, "@w:type", "dxa");
        }

        /// <summary>
        /// Inserts a table of one cell with using the <see cref="builder"/> object.
        /// </summary>
        private static Table InsertTable(DocumentBuilder builder)
        {
            Table table = builder.StartTable();
            builder.InsertCell();
            builder.EndRow();
            builder.EndTable();
            builder.InsertParagraph();
            return table;
        }

        /// <summary>
        /// Tests the <see cref="NrxXmlReader.IsUniversalMeasure"/> method.
        /// </summary>
        [TestCase((string)null, false)] // casting for C++
        [TestCase("", false)]
        [TestCase("0", false)]
        [TestCase("-25.75pt", true)]
        [TestCase("1cm", true)]
        [TestCase("1dm", false)]
        [TestCase("0.9pc", true)]
        [TestCase("-0.9pi", true)]
        public void TestIsUniversalMeasure(string value, bool expectedIsUniversalMeasure)
        {
            Assert.That(NrxXmlReader.IsUniversalMeasure(value), Is.EqualTo(expectedIsUniversalMeasure), string.Format(                "Error on checking '{0}'.", value));
        }

        /// <summary>
        /// Tests that all compatibility settings except adjustLineHeightInTable, applyBreakingRules,
        /// balanceSingleByteDoubleByteWidth, compatSetting, doNotExpandShiftReturn, doNotLeaveBackslashAlone,
        /// spaceForUL, ulTrailSpace are not written in ISO Strict format.
        /// </summary>
        [Test]
        public void TestRemovedCompatibilitySettings()
        {
            DocxExportContext context =
                new DocxExportContext(TestUtil.Open(@"ExportDocx\TestRemovedCompatibilitySettings.docx"),
                        SettingsPart, OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode("descendant::w:compat");
            Assert.That(node, IsNot.Null());
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                XmlNode compatNode = node.ChildNodes[i];
                if (compatNode.NodeType == XmlNodeType.Element)
                {
                    Assert.That(IsIsoStrictCompatibilitySetting(compatNode.LocalName), Is.True, string.Format(                        "The compatibility setting {0} is not supported in ISO 29500 Strict.", compatNode.LocalName));
                }
            }
        }

        /// <summary>
        /// Returns the 'true' value if the specified compatibility setting is supported in ISO 29500 Strict.
        /// </summary>
        private static bool IsIsoStrictCompatibilitySetting(string settingName)
        {
            return
                (settingName == "compatSetting") ||
                (settingName == "adjustLineHeightInTable") ||
                (settingName == "applyBreakingRules") ||
                (settingName == "balanceSingleByteDoubleByteWidth") ||
                (settingName == "doNotExpandShiftReturn") ||
                (settingName == "doNotLeaveBackslashAlone") ||
                (settingName == "spaceForUL") ||
                (settingName == "ulTrailSpace");
        }


        /// <summary>
        /// Returns the 'true' value if the specified attribute is supported attribute of the protection elements
        /// in ISO 29500 Strict.
        /// </summary>
        private static bool IsIsoStrictProtectionAttribute(string attributeName)
        {
            return
                (attributeName == "algorithmName") ||
                (attributeName == "edit") ||
                (attributeName == "enforcement") ||
                (attributeName == "formatting") ||
                (attributeName == "hashValue") ||
                (attributeName == "recommended") ||
                (attributeName == "saltValue") ||
                (attributeName == "spinCount");
        }

        /// <summary>
        /// Tests conversion of cryptographic algorithm name to/from algorithm SID.
        /// </summary>
        [TestCase("MD2")]
        [TestCase("MD4")]
        [TestCase("MD5")]
        [TestCase("RIPEMD-128")]
        [TestCase("RIPEMD-160")]
        [TestCase("SHA-1")]
        [TestCase("SHA-256")]
        [TestCase("SHA-384")]
        [TestCase("SHA-512")]
        [TestCase("WHIRLPOOL")]
        public void TestCryptAlgorithmNameFromSid(string algorithmName)
        {
            int sid = PasswordHash.CryptAlgorithmSidFromAlgorithmName(algorithmName);
            Assert.That(PasswordHash.CryptAlgorithmNameFromSid(sid), Is.EqualTo(algorithmName));
        }

        /// <summary>
        /// Tests that the hdrShapeDefaults and shapeDefaults elements are not written in ISO Strict format.
        /// </summary>
        /// <summary>
        /// Tests that the val attribute of the stylePaneFormatFilter element (§17.15.1.85) is not written
        /// in ISO Strict format.
        /// </summary>
        [Test]
        public void TestStylePaneFormatFilter()
        {
            Document doc = new Document();
            doc.DocPr.StylePaneFormatFilterSettings.Data = doc.DocPr.StylePaneFormatFilterSettings.Data + 1;

            DocxExportContext context = new DocxExportContext(doc, SettingsPart, OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode("descendant::w:stylePaneFormatFilter");
            Assert.That(node, IsNot.Null());
            node = node.Attributes.GetNamedItem("w:val");
            Assert.That(node, Is.Null, "The val attribute of the stylePaneFormatFilter element is written in ISO Strict format.");
        }

        /// <summary>
        /// Tests the <see cref="NrxXmlUtil.ConvertMeasure"/> method.
        /// </summary>
        [TestCase(-1d, (int)NrxUnit.Inch, (int)NrxUnit.Millimeter, -25.4d)] // Casting for C++.
        [TestCase(1d, (int)NrxUnit.Inch, (int)NrxUnit.Centimeter, 2.54d)]
        [TestCase(1d, (int)NrxUnit.Inch, (int)NrxUnit.Inch, 1d)]
        [TestCase(0d, (int)NrxUnit.Inch, (int)NrxUnit.Millimeter, 0d)]
        [TestCase(254d, (int)NrxUnit.Millimeter, (int)NrxUnit.Inch, 10d)]
        [TestCase(-2d, (int)NrxUnit.Centimeter, (int)NrxUnit.Millimeter, -20d)]
        [TestCase(480d, (int)NrxUnit.Twips, (int)NrxUnit.Pica, 2d)]
        [TestCase(2d, (int)NrxUnit.Point, (int)NrxUnit.Point, 2d)]
        [TestCase(24d, (int)NrxUnit.HalfPoints, (int)NrxUnit.Twips, 240d)]
        [TestCase(24d, (int)NrxUnit.Point, (int)NrxUnit.HalfPoints, 48d)]
        [TestCase(2d, (int)NrxUnit.Twips, (int)NrxUnit.HundredthsOfPoint, 10d)]
        [TestCase(1d, (int)NrxUnit.HundredthsOfPoint, (int)NrxUnit.Emus, 127d)]
        [TestCase(12700d, (int)NrxUnit.Emus, (int)NrxUnit.Point, 1d)]
        [TestCase(24d, (int)NrxUnit.Pica, (int)NrxUnit.Inch, 4d)]
        public void TestMeasureConversion(double value, int sourceUnit, int targetUnit, double expectedResult)
        {
            Assert.That(System.Math.Round(NrxXmlUtil.ConvertMeasure(value, (NrxUnit)sourceUnit, (NrxUnit)targetUnit), 3), Is.EqualTo(System.Math.Round(expectedResult, 3)), string.Format(                "Error on conversion of {0} from {1} to {2}.", value, sourceUnit, targetUnit));
        }

        /// <summary>
        /// Tests the <see cref="NrxXmlBuilder.ToUniversalMeasure"/> method.
        /// </summary>
        /// <remarks>
        /// Conversion itself is tested in <see cref="TestMeasureConversion"/>.
        /// </remarks>
        [TestCase(-1.05d, (int)NrxUnit.Millimeter, "-1.05mm")] // Casting for C++.
        [TestCase(1d, (int)NrxUnit.Centimeter, "1cm")]
        [TestCase(-999999.999999999d, (int)NrxUnit.Inch, "-999999.999999999in")]
        [TestCase(2d, (int)NrxUnit.Point, "2pt")]
        [TestCase(1.11d, (int)NrxUnit.Pica, "1.11pc")]
        public void TestFormingUniversalMeasure(double value, int unit, string expectedResult)
        {
            Assert.That(NrxXmlBuilder.ToUniversalMeasure(value, (NrxUnit)unit, (NrxUnit)unit), Is.EqualTo(expectedResult), string.Format(                "Error on {0} ({1}).", value, unit));
        }

        /// <summary>
        /// Tests the <see cref="NrxXmlBuilder.ToUniversalMeasure"/> method.
        /// </summary>
        /// <remarks>
        /// Conversion itself is tested in <see cref="TestMeasureConversion"/>.
        /// Test cases with Exception.
        /// </remarks>
        [TestCase(0d, (int)NrxUnit.Emus, "")] // Casting for C++.
        [TestCase(0d, (int)NrxUnit.HundredthsOfPoint, "")]
        [TestCase(0d, (int)NrxUnit.Twips, "")]
        [TestCase(0d, (int)NrxUnit.HalfPoints, "")]
        [TestCase(0d, (int)NrxUnit.None, "")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestFormingUniversalMeasure1(double value, int unit, string expectedResult)
        {
            Assert.That(NrxXmlBuilder.ToUniversalMeasure(value, (NrxUnit)unit, (NrxUnit)unit), Is.EqualTo(expectedResult), string.Format(                "Error on {0} ({1}).", value, unit));
        }

        /// <summary>
        /// Tests that the charset (Character Set Supported By Font) element is read and written correctly from/to
        /// ECMA-376 and ISO Strict formats..
        /// </summary>
        [TestCase(@"Model\Revision\TestRevisionNumberRemoved ms.docx", "numberingChange", 0)]
        public void TestElementNotWritten(string documentFile, string element, int warningIndex)
        {
            Document doc = TestUtil.Open(documentFile);
            TestWarningCallback warningCallback = new TestWarningCallback();
            doc.WarningCallback = warningCallback;

            DocxExportContext context = new DocxExportContext(doc, OoxmlComplianceCore.IsoStrict);

            XmlNode node = context.GetXmlNode(string.Format("//w:{0}", element));
            Assert.That(node, Is.Null);

            Assert.That(warningCallback.Count > warningIndex, Is.True);
            Assert.That(warningCallback[warningIndex].Description.Contains(element), Is.True);
        }

        /// <summary>
        /// Tests that ISO Strict namespaces are written in ISO Strict format.
        /// </summary>
        [Test]
        public void TestWritingCommonNamespaces()
        {
            DocxExportContext context = new DocxExportContext(new Document(), OoxmlComplianceCore.IsoStrict);

            XmlNode documentNode = context.XmlDocument.ChildNodes[Index];
            CheckAttributeValue(documentNode, "xmlns:w", UrlPrefix + "wordprocessingml/main");
            CheckAttributeValue(documentNode, "xmlns:m", DocUrlPrefix + "math");
            CheckAttributeValue(documentNode, "xmlns:r", DocUrlPrefix + "relationships");
            CheckAttributeValue(documentNode, "xmlns:wp", DmlUrlPrefix + "wordprocessingDrawing");
        }

        /// <summary>
        /// Tests that ISO Strict form of namespace of the 'graphicData' element is written in ISO Strict format.
        /// </summary>
        [TestCase(@"Model\Charts\Test3DAreaCharts.docx", DmlUrlPrefix + "chart")]
        [TestCase(@"Model\DrawingML\TestDiagramBlip.docx", DmlUrlPrefix + "diagram")]
        [TestCase(@"Model\DrawingML\TestImageData.docx", DmlUrlPrefix + "picture")]
        public void TestWritingGraphicDataNamespace(string docFileName, string expectedNamespace)
        {
            DocxExportContext context = new DocxExportContext(TestUtil.Open(docFileName),
                OoxmlComplianceCore.IsoStrict);
            CheckAttributeValue(context, "//a:graphicData/@uri", expectedNamespace);
        }

        /// <summary>
        /// Tests that correct namespaces of extended properties are written in ISO Strict format.
        /// </summary>
        [Test]
        public void TestWritingExtendedPropertiesNamespaces()
        {
            DocxExportContext context = new DocxExportContext(new Document(), AppDocPropsPart,
                OoxmlComplianceCore.IsoStrict);

            XmlNode rootNode = context.XmlDocument.ChildNodes[Index];
            CheckAttributeValue(rootNode, "xmlns", DocUrlPrefix + "extendedProperties");
            CheckAttributeValue(rootNode, "xmlns:vt", DocUrlPrefix + "docPropsVTypes");
        }


        /// <summary>
        /// Tests reading/writing document background from a Strict OOXML document.
        /// </summary>
        [TestCase(@"Model\Shape\Background\TestIsoStrictBackground.docx", "gradFill")]
        [TestCase(@"Model\Shape\Background\TestBackgroundSolid_Strict_EmptyChoice.docx", "solidFill")]
        public void TestReadingWritingBackground(string fileName, string expectedFill)
        {
            Document doc = TestUtil.Open(fileName);

            Assert.That(doc.BackgroundShape, IsNot.Null());
            ShapeBase dml = doc.BackgroundShape.FallbackShape;
            Assert.That(dml, IsNot.Null());
            Assert.That(dml.DmlNode.DocPrExtensions, IsNot.Null());
            Assert.That(dml.DmlNode.DocPrExtensions.Count, Is.EqualTo(1));

            DocxExportContext context = new DocxExportContext(doc, OoxmlComplianceCore.IsoStrict);

            Assert.That(context.GetXmlNode("//w:background/mc:AlternateContent/mc:Choice/v:background"), IsNot.Null());
            XmlElement dmlNode = (XmlElement)context.GetXmlNode("//mc:AlternateContent/mc:Fallback/w:drawing");
            Assert.That(dmlNode, IsNot.Null());
            Assert.That(context.GetXmlNode(dmlNode, "//wp:docPr/a:extLst/a:ext"), IsNot.Null());
            Assert.That(context.GetXmlNode(dmlNode, "//a:" + expectedFill), IsNot.Null());
        }


        /// <summary>
        /// Tests relationship type of mail merge recipient data.
        /// </summary>
        [Test]
        public void TestMailMergeRecipientData()
        {
            Assert.That(DocxRelationshipTypes.GetType(DocxRelationshipType.MailMergeRecipientData, true), Is.EqualTo("http://purl.oclc.org/ooxml/officeDocument/relationships/mailMergeRecipientData"));
        }


        /// <summary>
        /// Tests that correct namespaces of the theme part are written in ISO Strict format.
        /// </summary>
        [Test]
        public void TestWritingThemeNamespaces()
        {
            Document doc = new Document();
            doc.Theme.CreateThemeOverride();
            DocxExportContext context = new DocxExportContext(doc, ThemePart, OoxmlComplianceCore.IsoStrict);

            XmlElement rootNode = (XmlElement)context.XmlDocument.ChildNodes[Index];
            CheckAttributeValue(rootNode, "xmlns:a", DmlUrlPrefix + "main");
            XmlNode fmtSchemeNode = context.GetXmlNode(rootNode, "//a:themeElements/a:fmtScheme");
            Assert.That(fmtSchemeNode, IsNot.Null());
            Assert.That(fmtSchemeNode.NamespaceURI, Is.EqualTo(DmlUrlPrefix + "main"));
        }





        /// <summary>
        /// Tests that all relationship types are defined for Strict OOXML format.
        /// </summary>
        [Test]
        public void TestRelationshipTypes()
        {
            DocxRelationshipType[] relationshipTypes =
#if JAVA || CPLUSPLUS
                Enum.GetValues(typeof(DocxRelationshipType));
#else
                (DocxRelationshipType[])Enum.GetValues(DocxRelationshipType.OfficeDocument.GetType());
#endif
            foreach (DocxRelationshipType relationshipType in relationshipTypes)
            {
                Assert.That(StringUtil.HasChars(DocxRelationshipTypes.GetType(relationshipType, true)), Is.True, string.Format(                    "No relationship type for {0}.", relationshipType));
            }
        }




        private OoxmlSaveOptions CreateStrictSaveOptions()
        {
            return new OoxmlSaveOptions
            {
                Compliance = OoxmlCompliance.Iso29500_2008_Strict,
                SaveFormat = SaveFormat.Docx
            };
        }

        // Remove preprocessor after fix WORDSJAVA-1370.
        private const int Index =
#if JAVA
            0;
#else
            1;
#endif

        private const string DocumentPart = "word/document.xml";
        private const string SettingsPart = "word/settings.xml";
        private const string FontsPart = "word/fontTable.xml";
        private const string DocumentRelsPart = "word/_rels/document.xml.rels";
        private const string AppDocPropsPart = "docProps/app.xml";
        private const string CustomDocPropsPart = "docProps/custom.xml";
        private const string ThemePart = "word/theme/theme1.xml";
        private const string NumberingPart = "word/numbering.xml";
        private const string StylesPart = "word/styles.xml";
        private const string ChartColorStylePart = "word/charts/chart/colors1.xml";
        private const string ChartStylePart = "word/charts/chart/style1.xml";
        private const string ThemeOverridePart = "word/theme/themeOverride1.xml";

        private const string UrlPrefix = "http://purl.oclc.org/ooxml/";
        private const string DmlUrlPrefix = "http://purl.oclc.org/ooxml/drawingml/";
        private const string DocUrlPrefix = "http://purl.oclc.org/ooxml/officeDocument/";
    }
}
