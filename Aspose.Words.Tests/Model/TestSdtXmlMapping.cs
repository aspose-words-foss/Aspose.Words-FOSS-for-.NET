// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/03/2017 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Properties;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.Markup
{
    /// <summary>
    /// Tests mapping of a structured document tag to XML data.
    /// </summary>
    [TestFixture]
    public class TestSdtXmlMapping
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests that a correct dataBinding element is written on mapping a rich-text SDT to XML data.
        /// </summary>
        [Test]
        public void TestMappingToRichTextSdt()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.Insert(tag, null, false);

            tag.XmlMapping.SetMapping(doc.CustomXmlParts.Add("C8FFD3B6-7EF8-4963-895B-3565F68A03E7",
                "<root><text></text></root>"), "/root/text", "");

            Document doc2 = doc.Clone();
            doc2.ComplianceInfo = new OoxmlComplianceInfo();

            // Check that the necessary ISO Transitional format and Compatibility Mode are set.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Assert.That(tag.IsDocxExtension, Is.True);
            Assert.That(tag.XmlMapping.IsDocx15Extension, Is.True);
            Assert.That(doc.ComplianceInfo.IsDocxExtensions, Is.True);
            Assert.That(doc.ComplianceInfo.Compliance, Is.EqualTo(OoxmlComplianceCore.IsoTransitional));
            Assert.That(doc.CompatibilityOptions.MswVersion, Is.EqualTo(MsWordVersionCore.Word2013));

            // Check that if customer has explicitly set OOXML format and Compatibility Mode, they are not changed
            // on writing XML Mapping.
            doc2.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2010);
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.Compliance = OoxmlCompliance.Ecma376_2006;
            TestUtil.ExecuteValidator(doc2, saveOptions);

            Assert.That(doc2.ComplianceInfo.IsDocxExtensions, Is.False);
            Assert.That(doc2.ComplianceInfo.Compliance, Is.EqualTo(OoxmlComplianceCore.Ecma376));
            Assert.That(doc2.CompatibilityOptions.MswVersion, Is.EqualTo(MsWordVersionCore.Word2010));

            // Check that if customer has explicitly set OOXML format, incompatible XML Mapping is not written
            // and a warning is generated.
            TestWarningCallback warnings = new TestWarningCallback();
            doc2.WarningCallback = warnings;
            doc2 = TestUtil.SaveOpen(doc2, @"Model\Markup\TestMappingToRichTextSdt.docx", saveOptions, false);

            tag = (StructuredDocumentTag)doc2.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.XmlMapping.IsEmpty, Is.True);
            Assert.That(warnings.Contains(WarningSource.Docx, WarningType.DataLoss,
                WarningStrings.Ecma376NotSupportedXmlMapping), Is.True);
        }

        /// <summary>
        /// Tests whether an exception occurs on mapping a SDT of not supported type to XML data.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestMappingToBuildingBlockGallerySdt()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.BuildingBlockGallery, MarkupLevel.Block);
            tag.XmlMapping.SetMapping(doc.CustomXmlParts.Add("1", "<XMLNode/>"), "TestXPath", "TestPrefixes");
        }

        /// <summary>
        /// Tests whether an exception occurs if no XPath is defined on mapping to XML data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestNoXPath()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Inline);
            tag.XmlMapping.SetMapping(doc.CustomXmlParts.Add("1", "<XMLNode/>"), "", "TestPrefixes");
        }

        /// <summary>
        /// Tests whether an exception occurs if no custom XML part is specified on mapping to XML data.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestNoCustomXmlPart()
        {
            StructuredDocumentTag tag = new StructuredDocumentTag(new Document(), SdtType.PlainText, MarkupLevel.Inline);
            tag.XmlMapping.SetMapping((CustomXmlPart)null, "TestXPath", "TestPrefixes");
        }

        /// <summary>
        /// Tests mapping to correct and wrong XPath.
        /// </summary>
        [Test]
        public void TestMapping()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10270.docx");
            StructuredDocumentTag tag = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Assert.That(tag.XmlMapping.SetMapping(doc.CustomXmlParts[0], "/root/bad_path", "xmlns:ns='Namespace'"), Is.False);
            CheckMapping(tag, doc.CustomXmlParts[0], "/root/bad_path", "xmlns:ns='Namespace'", false);

            Assert.That(tag.XmlMapping.SetMapping(doc.CustomXmlParts[0], "/root/date", "xmlns:ns='Namespace'"), Is.True);
            CheckMapping(tag, doc.CustomXmlParts[0], "/root/date", "xmlns:ns='Namespace'", true);
        }

        /// <summary>
        /// Checks data of the <see cref="StructuredDocumentTag.XmlMapping"/> property of the specified structured
        /// document tag.
        /// </summary>
        private static void CheckMapping(StructuredDocumentTag tag, CustomXmlPart expectedPart, string expectedXPath,
            string expectedPrefixes, bool expectedIsMapped)
        {
            Assert.That(tag.XmlMapping.IsMapped, Is.EqualTo(expectedIsMapped));
            Assert.That(tag.XmlMapping.IsEmpty, Is.False);
            Assert.That(tag.XmlMapping.CustomXmlPart, Is.SameAs(expectedPart));
            Assert.That(tag.XmlMapping.XPath, Is.EqualTo(expectedXPath));
            Assert.That(tag.XmlMapping.PrefixMappings, Is.EqualTo(expectedPrefixes));
        }

        /// <summary>
        /// Tests mapping to correct and wrong XPath.
        /// </summary>
        [Test]
        public void TestMappingByStoreItemId()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10270.docx");
            StructuredDocumentTag tag = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            tag.XmlMapping.SetMapping(doc.CustomXmlParts[0].Id, "/root/bad_path", "xmlns:ns='Namespace'");
            CheckMapping(tag, doc.CustomXmlParts[0], "/root/bad_path", "xmlns:ns='Namespace'", false);

            tag.XmlMapping.SetMapping(doc.CustomXmlParts[0].Id, "/root/date", "xmlns:ns='Namespace'");
            CheckMapping(tag, doc.CustomXmlParts[0], "/root/date", "xmlns:ns='Namespace'", true);
        }

        /// <summary>
        /// Tests cloning XML mapping on cloning a document.
        /// </summary>
        [Test]
        public void TestCloningXmlMapping()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10270.docx");
            Document clonedDoc = doc.Clone();
            StructuredDocumentTag clonedTag =
                (StructuredDocumentTag)clonedDoc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Assert.That(doc.CustomXmlParts[0], IsNot.SameAs(clonedTag.XmlMapping.CustomXmlPart));
            CheckMapping(clonedTag, clonedDoc.CustomXmlParts[0], "/root[1]/date", "", true);
        }

        /// <summary>
        /// Tests getting and setting value of a SDT that is mapped to a custom XML part.
        /// </summary>
        [Test]
        public void TestGettingSettingValue()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10270.docx");
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Assert.That(sdt.XmlMapping.GetValue(), Is.EqualTo("2013-04-02T06:52:20.9977400-06:00"));

            const string newValue = "2017-06-30T15:30";
            sdt.XmlMapping.SetValue(newValue);

            CustomXmlPart customXmlPart = doc.CustomXmlParts.GetById(sdt.XmlMapping.StoreItemId);

            AnyXmlReader xmlReader = new AnyXmlReader(new MemoryStream(customXmlPart.Data));
            Assert.That(xmlReader.LocalName, Is.EqualTo("root"));
            xmlReader.ReadChild("root");
            Assert.That(xmlReader.LocalName, Is.EqualTo("date"));
            Assert.That(xmlReader.ReadString(), Is.EqualTo(newValue));
        }

        /// <summary>
        /// Tests getting and setting value of a SDT that is mapped to a document core property.
        /// </summary>
        [Test]
        public void TestGettingSettingValueFromToProperty()
        {
            Document doc = new Document();

            doc.BuiltInDocumentProperties[PropertyName.Comments].Value = "Test Comments"; // core property
            doc.BuiltInDocumentProperties[PropertyName.Company].Value = "Test Company";   // extended property

            // Insert a SDT and map to the core property.
            StructuredDocumentTag sdt1 = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(sdt1);
            sdt1.XmlMapping.SetMapping("{6C3C8BC8-F283-45AE-878A-BAB7291924A1}",
                "/ns1:coreProperties[1]/ns0:description[1]",
                "xmlns:ns0='http://purl.org/dc/elements/1.1/' " +
                "xmlns:ns1='http://schemas.openxmlformats.org/package/2006/metadata/core-properties'");

            // Insert a SDT and map to the extended property.
            StructuredDocumentTag sdt2 = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(sdt2);
            sdt2.XmlMapping.SetMapping("{6668398D-A668-4E3E-A5EB-62B293D839F1}", "/ns0:Properties[1]/ns0:Company[1]",
                "xmlns:ns0='http://schemas.openxmlformats.org/officeDocument/2006/extended-properties'");

            Assert.That(sdt1.XmlMapping.GetValue(), Is.EqualTo("Test Comments"));
            Assert.That(sdt2.XmlMapping.GetValue(), Is.EqualTo("Test Company"));

            sdt1.XmlMapping.SetValue("New Comments");
            Assert.That(doc.BuiltInDocumentProperties[PropertyName.Comments].Value, Is.EqualTo("New Comments"));

            sdt2.XmlMapping.SetValue("New Company");
            Assert.That(doc.BuiltInDocumentProperties[PropertyName.Company].Value, Is.EqualTo("New Company"));
        }

        /// <summary>
        /// WORDSNET-16888 Document.UpdatePageLayout does not stop.
        /// Mapped SDT data contains another SDT which references to itself. This cyclic
        /// reference causes the problem. Word maps data for the topmost SDT and skips
        /// nested mapping at this case. Mimic Word behavior to fix the problem.
        /// </summary>
        [Test]
        public void TestJira16888()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira16888.docx");

            StructuredDocumentTag outerSdt = (StructuredDocumentTag)doc.FirstSection.Body.FirstChild;

            Assert.That(outerSdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(outerSdt.GetChildNodes(NodeType.Any, false)[0].NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));

            StructuredDocumentTag innerSdt = (StructuredDocumentTag)outerSdt.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(innerSdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(innerSdt.FirstChild.NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(innerSdt.LastChild.NodeType, Is.EqualTo(NodeType.Paragraph));
        }



        /// <summary>
        /// WORDSNET-20775 StructuredDocumentTag.Checked throws InvalidOperationException (Custom XML part is not found)
        /// Mimic Word and skip custom xml update if mapping is present but invalid.
        /// </summary>
        [Test]
        public void Test20775()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20775.docx");

            StructuredDocumentTag sdt = doc.GetChild(NodeType.StructuredDocumentTag, 0, true) as StructuredDocumentTag;
            Assert.That(sdt.Checked, Is.False);
            Assert.That(sdt.ContentChecked, Is.False);

            sdt.Checked = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test20775", UnifiedScenario.Docx2DocxNoGold);
            sdt = doc.GetChild(NodeType.StructuredDocumentTag, 0, true) as StructuredDocumentTag;

            Assert.That(sdt.Checked, Is.EqualTo(true));
            Assert.That(sdt.ContentChecked, Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-22082 storeItemChecksum get/set method.
        /// Check the DataChecksum value for the opened document.
        /// </summary>
        [Test]
        public void Test22082_Existing()
        {
            long expChecksum = 0xed5B91F2;

            Document doc = TestUtil.Open(@"Model\Markup\Test22082.docx");
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expChecksum));

            // Check value after roundtrip.
            doc = TestUtil.SaveOpen(doc, "Test22082_Existing", UnifiedScenario.Docx2DocxNoGold);
            sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expChecksum));
        }

        /// <summary>
        /// WORDSNET-22082 storeItemChecksum get/set method.
        /// Check the DataChecksum value for the newly created sdt and after the mapping change.
        /// </summary>
        [Test]
        public void Test22082_Created()
        {
            Document doc = new Document();

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.Insert(sdt, null, false);

            sdt.XmlMapping.SetMapping(doc.CustomXmlParts.Add("C8FFD3B6-7EF8-4963-895B-3565F68A03E7",
                "<root><text>test</text></root>"), "/root/text", "");
            long expChecksum = 0x981484e0;
            Assert.That(sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expChecksum));

            sdt.XmlMapping.SetMapping(doc.CustomXmlParts.Add(("702E275F-454A-4A40-B627-53086ABDE80F"),
                "<root><text>changed test</text></root>"), "/root/text", "");
            expChecksum = 0xc900abc0;
            Assert.That(sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expChecksum));
        }

        /// <summary>
        /// WORDSNET-22082 storeItemChecksum get/set method.
        /// Check the DataChecksum value for the opened document and then after the CustomXmlPart.Data change.
        /// </summary>
        [Test]
        public void Test22082_Changed()
        {
            // Check the StoreItemChecksum value for the opened document.
            Document doc = TestUtil.Open(@"Model\Markup\Test22082.docx");

            // and then after the CustomXmlPart.Data change
            StructuredDocumentTag sdt = doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 0, true) as StructuredDocumentTag;
            long expChecksum = 0xed5b91F2;
            Assert.That(sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expChecksum));

            byte[] data = TestUtil.ReadAllBytes(@"Model\Markup\Test22082.xml");
            sdt.XmlMapping.CustomXmlPart.Data = data;
            expChecksum = 0xa6be6354;
            Assert.That(sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expChecksum));
        }


        /// <summary>
        /// WORDSNET-22449 Calculate storeItemChecksum attribute on Save
        /// Checksum now saved always for IsoStrict compliance.
        /// </summary>
        [Test]
        public void Test22449()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test22449.docx");

            const string xPathTemplate = "//w:body/w:sdt[{0}]/w:sdtPr/w15:dataBinding/@w16sdtdh:storeItemChecksum";

            DocxExportContext context = new DocxExportContext(doc, OoxmlComplianceCore.IsoStrict);
            context.AddCustomNamespaceMapping("w15", Nrx.DocxNamespaces.GetNamespace(Nrx.DocxNamespace.W15Markup, true));
            context.AddCustomNamespaceMapping("w16sdtdh", Nrx.DocxNamespaces.GetNamespace(Nrx.DocxNamespace.W16Sdtdh, true));

            Assert.That(context.GetXmlNode(string.Format(xPathTemplate, 1)).Value, Is.EqualTo("gifxXQ=="));
            Assert.That(context.GetXmlNode(string.Format(xPathTemplate, 2)).Value, Is.EqualTo("lRlr7A=="));
            // 3rd tag has no XmlMapping.
            Assert.That(context.GetXmlNode(string.Format(xPathTemplate, 4)).Value, Is.EqualTo("Np+DQQ=="));
        }

#if !CPLUSPLUS
        #region Test22449Customer


        [JavaDelete]
        public class Serializer
        {
            public static string Serialize(I newINPart, string ns)
            {
                return Serialize(newINPart.C, "I", ns);
            }

            internal static string Serialize(string content, string name, string ns)
            {
                if (content == null)
                    content = string.Empty;

                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;

                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    xmlWriter.WriteStartElement(name, ns);
                    xmlWriter.WriteString(content);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                return stringBuilder.ToString();
            }

            public static T LoadFromXmlWithoutNamespaces<T>(byte[] xmlBytes)
            {
                using (MemoryStream stream = new MemoryStream(xmlBytes))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(new NamespaceIgnorantXmlTextReader(stream));
                }
            }

        }

        [JavaDelete]
        public class NamespaceIgnorantXmlTextReader : XmlTextReader
        {
            public NamespaceIgnorantXmlTextReader(TextReader reader) : base(reader)
            { }
            public NamespaceIgnorantXmlTextReader(Stream reader) : base(reader)
            { }
            public override string NamespaceURI
            {
                get { return ""; }
            }
        }

        [XmlRoot]
        [JavaDelete]
        public class I
        {
            [XmlText]
            public string C { get; set; }
        }

        #endregion
#endif





        /// <summary>
        /// Related with WORDSNET-22617
        /// The Word does not update content of nested SDT when ancestor and child SDT mapped to OOXML document.
        /// This case also related with the document in the test <see cref="Import.Docx.TestImportDocx.Test20722" />.
        /// </summary>
        [Test]
        public void Test22617UpdateNestedSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test22617UpdateNestedSdt.docx");

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.GetText(), Is.EqualTo("Data2\f"));
        }


        /// <summary>
        /// WORDSNET-22919 Word table formatting issue using Customer XML data
        /// Reuse existing cell to update cell level SDT.
        /// </summary>
        [Test]
        public void Test22919()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test22919.docx");

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 1, true);

            Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(sdt.FirstChild.NodeType, Is.EqualTo(NodeType.Cell));
        }


        /// <summary>
        /// WORDSNET-23284 The end-of-cell marker has been removed and revisions have changed when creating custom XML.
        /// Word doesn't remove last paragraph in bound document, if it has some Built-In style.
        /// Also it doesn't remove reference Node in cell-level SDT if last paragraph was removed or
        /// its attributes cannot be overriden from the reference paragraph.
        /// </summary>
        [Test]
        public void Test23284()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test23284.docx");

            int originalRevisionsCount = doc.Revisions.Count;

            // Set mapping by the customer's scenario.
            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            for (int i = 0; i < sdts.Count; i++)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)sdts[i];
                CustomXmlPart cxp = doc.CustomXmlParts.GetById(string.Format("00000000-0000-0000-0000-00000000000{0}", i+1));
                sdt.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='urn:in:Component{0}'", i+1));
            }

            // Force to update SDT content.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Assert.That(sdts.Count, Is.EqualTo(9));
            for (int i = 0; i < sdts.Count; i++)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)sdts[i];
                Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));

                Cell cell = (Cell)sdt.FirstChild;

                Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
                ParagraphCollection paras = cell.Paragraphs;

                Assert.That(paras[0].GetText().StartsWith(string.Format("Item{0}", i+1)), Is.True);

                Assert.That(paras[1].GetText().StartsWith("Maecenas"), Is.True);
                Assert.That(paras[1].IsDeleteRevision, Is.True);

                Assert.That(paras[2].GetText(), Is.EqualTo("\a"));
                Assert.That(paras[2].IsDeleteRevision, Is.False);
            }

            Assert.That(doc.Revisions.Count, Is.EqualTo(originalRevisionsCount));
        }

        /// <summary>
        /// Relates to WORDSNET-23284
        /// Checks that non-empty paragraph is not removed, but reference node is removed in cell-level SDT.
        /// </summary>
        [Test]
        public void Test23284A()
        {
            Cell cell = Test23284Core(@"Model\Markup\Test23284A.docx");

            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            Assert.That(cell.LastChild.GetText(), Is.EqualTo("Last CustomXml\a"));
        }

        /// <summary>
        /// Relates to WORDSNET-23284
        /// Checks that non-empty paragraph is not removed and cannot be overriden as it has numbering,
        /// Thus reference node is also not removed in cell-level SDT.
        /// </summary>
        [Test]
        public void Test23284B()
        {
            Cell cell = Test23284Core(@"Model\Markup\Test23284B.docx");

            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
            Assert.That(cell.LastChild.GetText(), Is.EqualTo("\a"));
            Assert.That(cell.LastChild.PreviousSibling.GetText(), Is.EqualTo("Last CustomXml\r"));
        }

        /// <summary>
        /// Relates to WORDSNET-23284
        /// Checks that empty paragraph is not removed and cannot be overriden as it has numbering,
        /// Thus reference node is also not removed in cell-level SDT.
        /// </summary>
        [Test]
        public void Test23284C()
        {
            Cell cell = Test23284Core(@"Model\Markup\Test23284C.docx");

            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
            Assert.That(cell.LastChild.GetText(), Is.EqualTo("\a"));
            Assert.That(cell.LastChild.PreviousSibling.GetText(), Is.EqualTo("\r"));
        }

        /// <summary>
        /// Relates to WORDSNET-23284
        /// Checks that empty paragraph is removed and thus reference node is not removed in cell-level SDT.
        /// </summary>
        [Test]
        public void Test23284D()
        {
            Cell cell = Test23284Core(@"Model\Markup\Test23284D.docx");

            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            Assert.That(cell.LastChild.GetText(), Is.EqualTo("\a"));
        }

        /// <summary>
        /// Relates to WORDSNET-23284
        /// Checks that empty paragraph with non built-in style is removed and
        /// thus reference node is not removed in cell-level SDT.
        /// </summary>
        [Test]
        public void Test23284E()
        {
            Cell cell = Test23284Core(@"Model\Markup\Test23284E.docx");

            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            Assert.That(cell.LastChild.GetText(), Is.EqualTo("\a"));
        }

        /// <summary>
        /// Relates to WORDSNET-23284
        /// Checks that empty paragraph with built-in style is not removed and
        /// thus reference node is also not removed in cell-level SDT.
        /// </summary>
        [Test]
        public void Test23284F()
        {
            Cell cell = Test23284Core(@"Model\Markup\Test23284F.docx");

            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
            Assert.That(cell.LastChild.GetText(), Is.EqualTo("\a"));
            Assert.That(cell.LastChild.PreviousSibling.GetText(), Is.EqualTo("\r"));
        }

        /// <summary>
        /// WORDSNET-23284 Revisions changed after adding CustomXmlPart.
        /// AW should use "SmartStyleBehavior" while updating SDT from the mapped content.
        /// </summary>
        /// <remarks>
        /// AM. Test file has been updated, see WORDSNET-24739 for details.
        /// </remarks>
        [Test]
        public void Test23526()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test23526", UnifiedScenario.Docx2DocxNoGold);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 0, true);
            ParaPr paraPr = ((Cell)sdt.FirstChild).FirstParagraph.ParaPr;

            // Attributes should not be expanded while updating the SDT from the mapped content.
            Assert.That(paraPr[ParaAttr.ListId], Is.Null);
            Assert.That(paraPr[ParaAttr.Istd], Is.EqualTo(74));

            WordAttrCollection revPr = paraPr.FormatRevision.RevPr;
            Assert.That(revPr.Count, Is.EqualTo(1));
            Assert.That(revPr[ParaAttr.Istd], Is.EqualTo(91));
        }

        /// <summary>
        /// Opens a document and sets the mapping of the first SDT, specific to the Test23284 test suite.
        /// </summary>
        private static Cell Test23284Core(string testName)
        {
            Document doc = TestUtil.Open(testName);
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            CustomXmlPart cxp = doc.CustomXmlParts[0];
            sdt.XmlMapping.SetMapping(cxp, "/ns0:I[1]", "xmlns:ns0='urn:in:item1'");

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            return (Cell)sdt.FirstChild;
        }




        /// <summary>
        /// Additional test case for WORDSNET-23777
        /// </summary>
        [TestCase(@"Model\Markup\Test23777A1.xml", " ")]
        [TestCase(@"Model\Markup\Test23777A2.xml", "Klikk eller trykk her for å skrive inn tekst.")]
        [TestCase(@"Model\Markup\Test23777A3.xml", "Klikk eller trykk her for å skrive inn tekst.")]
        public void Test23777A(string testName, string expectedText)
        {
            Document doc = TestUtil.Open(testName);
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Assert.That(sdt.GetText(), Is.EqualTo(expectedText));
        }



        /// <summary>
        /// Simplified test for WORDSNET-24190
        /// </summary>
        [Test]
        public void Test24190A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test24190A.docx");

            StructuredDocumentTagCollection tags = doc.Range.StructuredDocumentTags;
            Assert.That(tags.Count, Is.EqualTo(2));

            // WORDSNET-25442 Seems MS has changed mapping behavior when inline SDT is updated from multiline document.
            // Now parent paragraph is moved into SDT as reference paragraph while all content of this paragraph including
            // SDT is moved out.
            StructuredDocumentTag tag0 = (StructuredDocumentTag)tags[0];
            Paragraph para = (Paragraph)tag0.FirstChild;
            Assert.That(para.ParaPr.Istd, Is.EqualTo(0xf3));
            Assert.That(para.ParagraphFormat.StyleName, Is.EqualTo("Text 2"));

            Assert.That(para.ParagraphBreakRunPr.Istd, Is.EqualTo(0x43));
            Assert.That(para.ParagraphBreakFont.StyleName, Is.EqualTo("Text Char"));

            StructuredDocumentTag tag1 = (StructuredDocumentTag)tags[1];
            para = (Paragraph)tag1.FirstChild;
            Assert.That(para.ParaPr.Istd, Is.EqualTo(0xf3));
            Assert.That(para.ParagraphFormat.StyleName, Is.EqualTo("Text 2"));
        }

        /// <summary>
        /// Simplified case for WORDSNET-22815
        /// </summary>
        [Test]
        public void Test22815A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test22815A.xml");
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(6));
        }

        /// <summary>
        /// WORDSNET-21964 Nested content control not deleted from document
        /// Implemented basic runtime mapped CustomXML update.
        /// </summary>
        [Test]
        public void Test21964()
        {
            const string fileName = @"Model\Markup\Test21964.docx";

            Document doc = TestUtil.Open(fileName);

            Assert.That(doc.Range.StructuredDocumentTags.Count, Is.EqualTo(2));

            IStructuredDocumentTag outerSdt = doc.Range.StructuredDocumentTags[0];
            IStructuredDocumentTag innerSdt = doc.Range.StructuredDocumentTags[1];

            byte[] dataBefore = (byte[])outerSdt.XmlMapping.CustomXmlPart.Data.Clone();

            innerSdt.Node.Remove();

            byte[] dataAfter = outerSdt.XmlMapping.CustomXmlPart.Data;

            // Check that mapped CustomXML of outer tag has been changed runtime.
            Assert.That(ArrayUtil.CompareBytes(dataBefore, dataAfter, dataBefore.Length), Is.False);

            doc = TestUtil.SaveOpen(doc, fileName, new OoxmlSaveOptions(), false);

            Assert.That(doc.Range.StructuredDocumentTags.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-24036 "w14:checked" is not Sdt.CheckBox direct value
        /// Word updates checkbox content only when value is changed.
        /// </summary>
        [TestCase(@"Model\Markup\Test24036.docx", "☒")]
        [TestCase(@"Model\Markup\Test13458 Checked Mapped 0.docx", "N")]
        [TestCase(@"Model\Markup\Test13458 Checked Mapped 1.docx", "X")]
        [TestCase(@"Model\Markup\Test13458 Unchecked Mapped 0.docx", "X")]
        public void Test24036(string fileName, string expContent)
        {
            Document doc = TestUtil.Open(fileName);

            IStructuredDocumentTag tag = doc.Range.StructuredDocumentTags[0];
            Assert.That(tag.Node.GetText(), Is.EqualTo(expContent));
        }

        /// <summary>
        /// WORDSNET-24529 Hidden SDT becomes visible after open/save document.
        /// Do not update RichText SDT if visible content equals to mapped.
        /// </summary>
        [Test]
        public void Test24529()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test24529.docx");

            StructuredDocumentTag tag = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0].Node;

            Run content = (Run)tag.FirstChild;

            Assert.That(content.Text, Is.EqualTo("Test"));
            Assert.That(content.Font.Hidden, Is.True);
        }







        /// <summary>
        /// WORDSNET-25242 SDT value is changed after rendering document.
        /// Update nested XPath to absolute values.
        /// </summary>
        public void Test25242(string fileName, int index, string xPath)
        {
            Document doc = TestUtil.Open(fileName);

            StructuredDocumentTag rs = (StructuredDocumentTag)doc.Range.StructuredDocumentTags.GetById(207924854);
            Assert.That(rs.SdtType, Is.EqualTo(SdtType.RepeatingSection));

            StructuredDocumentTag rsItem = (StructuredDocumentTag)rs.GetChildNodes(NodeType.Any, false)[index];
            StructuredDocumentTag nestedSdt = (StructuredDocumentTag)((Paragraph)rsItem.FirstChild).FirstChild;

            Assert.That(nestedSdt.XmlMapping.XPath, Is.EqualTo(xPath));
            Assert.That(nestedSdt.GetText(), Is.EqualTo((index == 0 ? "CA1000700-001" : "CA1000700-002")));
        }


        /// <summary>
        /// Simplified test for WORDSNET-25442
        /// </summary>
        /// <remarks>
        /// Seems that MS constantly changes XML Mapping logic.
        /// This behavior was tested on Microsoft® Word 2019 MSO (Version 2306 Build 16.0.16529.20100) 32-bit
        /// </remarks>
        [Test]
        public void Test25442A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25442A.docx");
            Assert.That(doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));

            Paragraph para = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].FirstParagraph;
            Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x47));
            Assert.That(para.ParaPr[ParaAttr.BorderBottom], IsNot.Null());

            Assert.That(para.Count, Is.EqualTo(2));
            Assert.That(para.GetChild(NodeType.Any, 0, false).NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(para.GetChild(NodeType.Any, 1, false).NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
        }


        /// <summary>
        /// Additional test for WORDSNET-25442
        /// Tests that adjacent bookmarks are correctly handled.
        /// </summary>
        [Test]
        public void Test25442Bookmarks1()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira17905_BlockSdtAndParaToStartOfPara.docx");

            // Check that bookmarks before SDT is preserved and moved before SDT.
            NodeCollection bodyChildNodes = doc.FirstSection.Body.GetChildNodes(NodeType.Any, false);
            Assert.That(bodyChildNodes[0].NodeType, Is.EqualTo(NodeType.BookmarkStart));
            Assert.That(bodyChildNodes[1].NodeType, Is.EqualTo(NodeType.BookmarkEnd));
            Assert.That(bodyChildNodes[2].NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
        }

        /// <summary>
        /// Additional test for WORDSNET-25442
        /// Tests that adjacent bookmarks are correctly handled.
        /// </summary>
        [Test]
        public void Test25442Bookmarks2()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira17905_ParaAndBlockSdtWithReplacingPara.docx");

            // Check that surrounding bookmarks are preserved and moved along with SDT.
            Bookmark bm1 = doc.Range.Bookmarks["bm1"];
            Assert.That(bm1.BookmarkEnd.NextSibling.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));

            Bookmark bm2 = doc.Range.Bookmarks["bm2"];
            Assert.That(bm2.BookmarkStart.PreviousSibling.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
        }

        /// <summary>
        /// WORDSNET-25160 Updating content controls causing incorrect formatting
        /// Fixed per WORDSNET-26027.
        /// </summary>
        [Test]
        public void Test25160()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25160.docx");

            // Both SDTs should have 'Table Center' style applied for the last paragraph.
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];
                Paragraph para = (Paragraph)sdt.LastChild;
                Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x12));

            sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[1];
            para = (Paragraph)sdt.LastChild;
            Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x12));
            }

        /// <summary>
        /// WORDSNET-25469 Nested content controls CXP is not updating
        /// Handle the case when more than one outer SDT should be updated.
        /// </summary>
        [Test]
        public void Test25469()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test25469.docx");

            // Update inner SDT.
            StructuredDocumentTag innerSdt = doc.Range.StructuredDocumentTags.GetById(1499471227) as StructuredDocumentTag;
            Paragraph para = (Paragraph)innerSdt.FirstChild;
            para.RemoveAllChildren();
            para.AppendChild(new Run(doc, "New Content Added"));

            // Extract mapped document for outer SDT.
            CustomXmlPart xmlPart = doc.CustomXmlParts.GetById("{1EB64CAA-E422-4916-9D40-2877F937F471}");
            XmlDocument xmlDoc = XmlUtilPal.LoadXml(new MemoryStream(xmlPart.Data), false);
            byte[] innerBytes = Encoding.UTF8.GetBytes(xmlDoc.FirstChild.InnerText);
            LoadOptions options = new LoadOptions() { UpdateXmlMapping = false };
            Document outerMappedDoc = new Document(new MemoryStream(innerBytes), options);

            // Check that mapped document for outer SDT has been updated properly.
            StructuredDocumentTag outerMappedSdt = (StructuredDocumentTag)outerMappedDoc.Range.StructuredDocumentTags[0];
            Assert.That(outerMappedSdt.GetText(), Is.EqualTo("New Content Added\r"));
        }

        /// <summary>
        /// WORDSNET-26419 Line break is added at the end of block level SDT.
        /// Ignore paragraph break character when PlainText SDT is updated from mapped XML.
        /// </summary>
        [Test]
        public void Test26419()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26419.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            var listOfSdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (StructuredDocumentTag sdt in listOfSdts)
            {
                sdt.RemoveAllChildren();
                // Make sure the is node we can move to in SDT.
                if (sdt.Level == MarkupLevel.Block)
                    sdt.AppendChild(new Paragraph(doc));
                else if (sdt.Level == MarkupLevel.Inline)
                    sdt.AppendChild(new Run(doc));

                // The following is not required to reproduce the problem.
                // But in such case the problem is seen in the output document.
                builder.MoveToStructuredDocumentTag(sdt, 0);
                builder.Write("A whole para");
                sdt.RemoveSelfOnly();
            }

            Assert.That(doc.GetText(), Is.EqualTo("A whole para\rHello: A whole para\f"));
        }

        /// <summary>
        /// Relates to WORDSNET-26419.
        /// Simplified case.
        /// </summary>
        [Test]
        public void Test26419A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26419A.docx");

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];
            Assert.That(sdt.GetText(), Is.EqualTo("A whole para\r"));
        }

        /// <summary>
        /// WORDSNET-26381 Inserting text to cell in XML-mapped cell-level structured document tag makes document corrupted
        /// The document was corrupted because the end-of-cell character was written to a custom XML part. Now the character
        /// is removed from the text.
        /// </summary>
        [Test]
        public void Test26381()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26381.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 2, true);
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Cell));

            // Add text to the SDT.
            Node run = sdt.GetChild(NodeType.Run, 0, true);
            builder.MoveTo(run);
            builder.Write("<Text>");

            // Check that the end-of-cell character is not written to the custom XML.
            CustomXmlPart xmlPart = sdt.XmlMapping.CustomXmlPart;
            string xmlPartText = Encoding.ASCII.GetString(xmlPart.Data);
#if JAVA
            const string expectedText = "<cell1>&lt;Text>Cell 1</cell1>";
#else
            const string expectedText = "<cell1>&lt;Text&gt;Cell 1</cell1>";
#endif
            Assert.That(xmlPartText.Contains(expectedText), Is.True);
        }

        /// <summary>
        /// Additional test for WORDSNET-26636.
        /// Simplified case.
        /// </summary>
        [Test]
        public void Test26636A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26636A.docx");

            // Both SDTs should have the same structure after update.
            foreach (IStructuredDocumentTag node  in doc.Range.StructuredDocumentTags)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Block));
                Assert.That(sdt.FirstChild.NodeType, Is.EqualTo(NodeType.Paragraph));
                Assert.That(sdt.FirstChild.NextSibling.NodeType, Is.EqualTo(NodeType.Table));
            }
        }


        /// <summary>
        /// Additional test for WORDSNET-26601.
        /// Tests that correct paragraph style is applied.
        /// </summary>
        public void Test26601A(string testName, int istd, string styleName)
        {
            Document doc = TestUtil.Open(testName);
            Paragraph para = (Paragraph)doc.Range.StructuredDocumentTags[0].GetChildNodes(NodeType.Paragraph, true)[0];

            Assert.That(para.ParaPr.Istd, Is.EqualTo(istd));
            Assert.That(para.ParagraphStyle.Name, Is.EqualTo(styleName));
        }

        /// <summary>
        /// WORDSNET-27003 Extra paragraph added after SetMapping
        /// Updated XML mapping logic.
        /// </summary>
        [Test]
        public void Test27003()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test27003.docx");
            Cell cell = doc.FirstSection.Body.Tables[0].Rows[1].FirstCell;

            Assert.That(cell.Paragraphs.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Simplified case for WORDSNET-26905.
        /// </summary>
        [Test]
        public void Test26905A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26905A.docx");
            List<CommentRangeStart> starts = doc.GetChildNodes(NodeType.CommentRangeStart, true).ToList<CommentRangeStart>();

            // Check that first two CommentRangeStart nodes is not moved into imported para.
            Assert.That(starts[0].ParentNode.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(starts[0].IsFirstChild, Is.True);

            Assert.That(starts[1].ParentNode.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(starts[1].IsFirstChild, Is.True);
        }

        /// <summary>
        /// WORDSNET-27328 Inconsistent XmlMapping.IsMapped property behavior with CustomXmlPart null value in SDTs
        /// Create runtime CustomXmlPart for core properties mapping.
        /// </summary>
        [Test]
        public void Test27328()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test27328.docx");

            StructuredDocumentTag sdt = doc.Range.StructuredDocumentTags.GetByTitle("Author") as StructuredDocumentTag;

            Assert.That(sdt.XmlMapping.CustomXmlPart, IsNot.Null());
            Assert.That(sdt.XmlMapping.CustomXmlPart.Schemas[0], Is.EqualTo("http://schemas.openxmlformats.org/package/2006/metadata/core-properties"));
            Assert.That(sdt.XmlMapping.CustomXmlPart.Schemas[1], Is.EqualTo("http://purl.org/dc/elements/1.1/"));
        }

        /// <summary>
        /// WORDSNET-27335 Comment replies separated.
        /// Update next annotation id before XML mapping update.
        /// </summary>
        [Test]
        [JavaDelete("Not portable to Java XmlWriter")]
        public void Test27335()
        {
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = MsWordVersion.Word2019;
            Document document = TestUtil.Open(@"Model\Markup\Test27335.docx", loadOptions);

            foreach (StructuredDocumentTag structuredDocumentTag in document.Range.StructuredDocumentTags)
            {
                string xmlns = Guid.NewGuid().ToString();
                string content = structuredDocumentTag.NodeType == NodeType.StructuredDocumentTag
                    ? structuredDocumentTag.WordOpenXMLMinimal
                    : structuredDocumentTag.WordOpenXML;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(content);
                string componentContentXml = xmlDocument.OuterXml;
                StringBuilder xmlData = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = false,
                    OmitXmlDeclaration = true,
                    NewLineChars = "\r\n",
                    ConformanceLevel = ConformanceLevel.Fragment
                };
                using (XmlWriter writer = XmlWriter.Create(xmlData, settings))
                {
                    writer.WriteStartElement("test", xmlns);
                    Char nullCharacter = '\0';
                    componentContentXml = componentContentXml.TrimEnd(nullCharacter);
                    writer.WriteString(componentContentXml);
                    writer.WriteEndElement();
                    writer.Flush();
                    writer.Close();
                }
                CustomXmlPart customXmlPart = document.CustomXmlParts.Add(Guid.NewGuid().ToString(), xmlData.ToString());
                structuredDocumentTag.XmlMapping.SetMapping(customXmlPart, "/ns0:test[1]", string.Format("xmlns:ns0='{0}'", xmlns));
            }
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            document = TestUtil.SaveOpen(document, @"Model\Markup\Test27335.docx", saveOptions, false);

            // Verify comment range presence.
            Paragraph para = document.FirstSection.Body.Paragraphs[10];
            NodeCollection childNodes = para.GetChildNodes(NodeType.Any, false);

            Assert.That(childNodes[0].NodeType, Is.EqualTo(NodeType.CommentRangeStart));
            Assert.That(childNodes[1].NodeType, Is.EqualTo(NodeType.CommentRangeStart));

            Comment comment1 = (Comment)childNodes[4];
            Comment comment2 = (Comment)childNodes[6];

            Assert.That(comment1.ParentId, Is.EqualTo(Comment.NoParent));
            Assert.That(comment2.ParentId, Is.EqualTo(comment1.Id));
        }

        /// <summary>
        /// WORDSNET-27447 System.InvalidOperationException when opening WordOpenXMLMinimal
        /// Remove macros when save WordOpenXML content.
        /// </summary>
        [Test]
        public void Test27447()
        {
            Document document = TestUtil.Open(@"Model\Markup\Test27447.docm");

            foreach (StructuredDocumentTag contentControl in document.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                if (contentControl.SdtType == SdtType.RichText && contentControl.Tag.Contains("ID"))
                {
                    Assert.That(contentControl.WordOpenXMLMinimal, IsNot.Empty());
                    Assert.That(contentControl.WordOpenXML, IsNot.Empty());
                }
            }
        }



        /// <summary>
        /// WORDSNET-27487 InvalidOperationException on Save with XML Mapping
        /// Already resolved per WORDSNET-27335, just added test.
        /// </summary>
        [Test]
        [JavaDelete("Not portable to Java XmlWriter")]
        public void Test27487()
        {
            const string testName = @"Model\Markup\Test27487.docx";

            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = MsWordVersion.Word2019;
            Document document = TestUtil.Open(testName, loadOptions);

            foreach (StructuredDocumentTag structuredDocumentTag in document.Range.StructuredDocumentTags)
            {
                string xmlns = Guid.NewGuid().ToString();
                string content = structuredDocumentTag.NodeType == NodeType.StructuredDocumentTag ? structuredDocumentTag.WordOpenXMLMinimal : structuredDocumentTag.WordOpenXML;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(content);
                string componentContentXml = xmlDocument.OuterXml;
                StringBuilder xmlData = new StringBuilder();
                var settings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = false,
                    OmitXmlDeclaration = true,
                    NewLineChars = "\r\n",
                    ConformanceLevel = ConformanceLevel.Fragment
                };
                using (XmlWriter writer = XmlWriter.Create(xmlData, settings))
                {
                    writer.WriteStartElement("test", xmlns);
                    Char nullCharacter = '\0';
                    componentContentXml = componentContentXml.TrimEnd(nullCharacter);
                    writer.WriteString(componentContentXml);
                    writer.WriteEndElement();
                    writer.Flush();
                    writer.Close();
                }
                CustomXmlPart customXmlPart = document.CustomXmlParts.Add(Guid.NewGuid().ToString(), xmlData.ToString());
                structuredDocumentTag.XmlMapping.SetMapping(customXmlPart, "/ns0:test[1]", string.Format("xmlns:ns0='{0}'", xmlns));
            }
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            document = TestUtil.SaveOpen(document, testName, saveOptions, false);

            Assert.That(document.Range.StructuredDocumentTags.Count, Is.EqualTo(2));
        }






        /// <summary>
        /// WORDSNET-28046 Unexpected loss of paragraph data and attributes after modifying SDT mapping and custom XML
        /// Preserve SDT paragraph attributes in case of update from placeholder.
        /// </summary>
        [Test]
        public void Test28046()
        {
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = MsWordVersion.Word2019;
            Document sourceDocument = TestUtil.Open(@"Model\Markup\Test28046", LoadFormat.Docx);
            // No update page layout in FOSS.

            foreach (Node node in sourceDocument.GetChildNodes(NodeType.Any, true))
            {
                if (node.NodeType == NodeType.StructuredDocumentTag)
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                    sdt.XmlMapping.Delete();
                }
                if (node.NodeType == NodeType.StructuredDocumentTagRangeStart)
                {
                    StructuredDocumentTagRangeStart sdt = (StructuredDocumentTagRangeStart)node;
                    sdt.XmlMapping.Delete();
                }
            }
            sourceDocument.CustomXmlParts.Clear();

            // AM. There is something wrong with Document cloning, have no time to figure it out,
            // use manual SaveOpen for a while.
            string outFileName =
                TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(@"Model\Markup\Test28046"), "", SaveFormat.Docx);
            sourceDocument.Save(outFileName);

            Document doc = TestUtil.Open(outFileName);

            StructuredDocumentTag sdt1510876971 =
                (StructuredDocumentTag)doc.Range.StructuredDocumentTags.GetById(1510876971);
            Paragraph para = (Paragraph)sdt1510876971.FirstChild;

            Assert.That(para.ParaId, Is.EqualTo(0x4d1b13ec));
            Assert.That(para.TextId, Is.EqualTo(0x71ff5351));

            Assert.That(para.ParaPr.Count, Is.EqualTo(1));
            Assert.That(para.ParaPr[ParaAttr.RsidP], Is.EqualTo(0x00e06078));

            Assert.That(para.ParagraphBreakRunPr.Count, Is.EqualTo(2));
            Assert.That(para.ParagraphBreakRunPr[FontAttr.RsidR], Is.EqualTo(0x00810263));
            Assert.That(para.ParagraphBreakRunPr[FontAttr.LocaleIdFarEast], Is.EqualTo((int)Language.JapaneseJapan));
        }

        /// <summary>
        /// WORDSNET-28393 Unexpected Bookmark Creation When Mapping Identical Content Controls to the Same Custom XML Part
        /// Seems that MS Word does not rename bookmarks during XML mapping update.
        /// </summary>
        [Test]
        [JavaDelete]
        public void Test28393()
        {
            const string fileName = @"Model\Markup\Test28393.docx";

            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Docx;
            loadOptions.MswVersion = MsWordVersion.Word2019;
            Document document = TestUtil.Open(fileName, loadOptions);

            NodeCollection structuredDocumentTagList = document.GetChildNodes(NodeType.StructuredDocumentTag, true);

            foreach(Node node in structuredDocumentTagList)
            {
                StructuredDocumentTag structuredDocumentTag = node as StructuredDocumentTag;
                string nameSpaceURI = structuredDocumentTag.Title;
                CustomXmlPart  customXmlPart = GetCustomPart(document, structuredDocumentTag, nameSpaceURI);
                if(customXmlPart!=null && structuredDocumentTag.XmlMapping!=null && !structuredDocumentTag.XmlMapping.IsMapped)
                    structuredDocumentTag.XmlMapping.SetMapping(customXmlPart, "/ns0:INPart[1]", string.Format("xmlns:ns0='{0}'", nameSpaceURI));
            }

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            document = TestUtil.SaveOpen(document, fileName, saveOptions, false);
            Assert.That(document.Range.Bookmarks.Count, Is.EqualTo(1));
        }

        [JavaDelete]
        private static CustomXmlPart GetCustomPart(Document document, StructuredDocumentTag componentControl, string nameSpaceURI)
        {
            CustomXmlPart customXmlPart = document.CustomXmlParts.OfType<CustomXmlPart>().
                FirstOrDefault(cxp => cxp.Schemas.OfType<string>().
                    Any(schema => schema.Equals(nameSpaceURI, StringComparison.OrdinalIgnoreCase)));

            if (customXmlPart == null)
            {
                string componentContentXml = string.Empty;
                if (componentControl.SdtType == SdtType.RichText)
                {
                    if (!componentControl.IsShowingPlaceholderText )
                    {
                        string content = componentControl.NodeType == NodeType.StructuredDocumentTag
                            ? componentControl.WordOpenXMLMinimal
                            : componentControl.WordOpenXML;
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(content);
                        componentContentXml = xmlDocument.OuterXml;
                    }
                }
                else
                    componentContentXml =componentControl.IsShowingPlaceholderText?"":componentControl.Range.Text;
                customXmlPart = CreateAndAddNewCxp(document, nameSpaceURI, componentContentXml);
                customXmlPart.Schemas.Add(nameSpaceURI);
            }
            return customXmlPart;
        }

        [JavaDelete]
        private static CustomXmlPart CreateAndAddNewCxp(Document document,  string nameSpaceURI, string content)
        {
            StringBuilder xmlData = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true,
                NewLineChars = "\r\n",
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (XmlWriter writer = XmlWriter.Create(xmlData, settings))
            {
                writer.WriteStartElement("INPart", nameSpaceURI);
                Char nullCharacter = '\0';
                content = content.TrimEnd(nullCharacter);
                writer.WriteString(content);
                writer.WriteEndElement();
                writer.Flush();
                writer.Close();
            }
            return document.CustomXmlParts.Add(Guid.NewGuid().ToString(), xmlData.ToString());
        }

        /// <summary>
        /// Relates to WORDSNET-28393.
        /// Tests that XML mapping updated correctly during import.
        /// </summary>
        [Test]
        public void Test28393A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test28393A.docx");

            // Check there is only only one bookmark.
            BookmarkCollection bookmarks = doc.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(1));
            Assert.That(bookmarks[0].Name, Is.EqualTo("Bookmark1"));

            // Check bookmark is located inside the last SDT.
            StructuredDocumentTag sdt = (StructuredDocumentTag)bookmarks[0].BookmarkStart
                .GetAncestor(NodeType.StructuredDocumentTag);

            Assert.That(sdt.Id, Is.EqualTo(-561253186));
        }

        /// <summary>
        /// WORDSNET-28292 InvalidOperationException when processing DOCX with XML-mapped StructuredDocumentTags
        /// Added basic support for row-level XML Mapping.
        /// </summary>
        [Test]
        [JavaDelete]
        public void Test28292()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test28292.docx");

            foreach (StructuredDocumentTag structuredDocumentTag in doc.Range.StructuredDocumentTags)
            {
                string ns = $"test:{Guid.NewGuid()}";
                StringBuilder stringBuilder = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineChars = "\r\n";
                using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
                {
                    xmlWriter.WriteStartElement("test", ns);
                    xmlWriter.WriteString(structuredDocumentTag.WordOpenXMLMinimal);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                CustomXmlPart customXmlPart = doc.CustomXmlParts.Add(Guid.NewGuid().ToString("B"), stringBuilder.ToString());
                customXmlPart.Schemas.Add(ns);
                structuredDocumentTag.XmlMapping.SetMapping(customXmlPart, "/ns0:test[1]", string.Format("xmlns:ns0='{0}'", ns));
            }

            // No update page layout in FOSS.

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test28292.docx", new OoxmlSaveOptions(), false);

            StructuredDocumentTag rowLevelSdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags.GetById(1976718228);

            Assert.That(rowLevelSdt.Level, Is.EqualTo(MarkupLevel.Row));
            Assert.That(rowLevelSdt.FirstChild.NodeType, Is.EqualTo(NodeType.Row));
            Assert.That(rowLevelSdt.FirstChild.NextSibling.NodeType, Is.EqualTo(NodeType.BookmarkEnd));

            Row row = (Row)rowLevelSdt.FirstChild;
            Assert.That(row.Cells.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// WORDSNET-28542 Removing and adding child nodes to Date SDT produced invalid output.
        /// Map Date SDT data as text.
        /// </summary>
        [Test]
        public void Test28542()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            values.Add("DocumentNumber", "TestDocument");
            values.Add("RevisionIssueDate", "11/08/2025");

            // fill template document tags and save
            Document doc = TestUtil.Open(@"Model\Markup\Test28542.docx");

            FillFields(doc, values);
            doc.UpdateFields();
            // No update page layout in FOSS.

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test28542.docx", new OoxmlSaveOptions(), false);

            // Check problematic SDT and mapped XML data.
            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.FirstSection.Body.Range.StructuredDocumentTags.GetByTag("RevisionIssueDate");

            Assert.That(sdt.GetText(), Is.EqualTo("11/08/2025"));

            XmlDocument xmlDoc = XmlUtilPal.LoadXml(new MemoryStream(sdt.XmlMapping.CustomXmlPart.Data), false);

            if (xmlDoc.ChildNodes[1] != null)
            {
                XmlNode docNumber = xmlDoc.ChildNodes[1].ChildNodes[0];
                Assert.That(docNumber.Name, Is.EqualTo("DocNumber"));
                Assert.That(docNumber.InnerText, Is.EqualTo("TestDocument"));

                XmlNode revisionIssueDate = xmlDoc.ChildNodes[1].ChildNodes[1];
                Assert.That(revisionIssueDate.Name, Is.EqualTo("RevisionIssueDate"));
                Assert.That(revisionIssueDate.InnerText, Is.EqualTo("11/08/2025"));
            }
        }








        private static void FillFields(Document document, Dictionary<string, string> reportValues)
        {
            NodeCollection nodeCollection = document.GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (StructuredDocumentTag structuredDocumentTag in nodeCollection)
            {
                string tagName = structuredDocumentTag.Tag;
                if (reportValues.ContainsKey(tagName))
                {
                    String value = reportValues[tagName];
                    if (structuredDocumentTag.FirstChild != null)
                    {
                        NodeType nodeType = structuredDocumentTag.FirstChild.NodeType;
                        switch (nodeType)
                        {
                            case NodeType.Run:
                                structuredDocumentTag.RemoveAllChildren();
                                Run run = new Run(document);
                                run.Text = value;
                                structuredDocumentTag.AppendChild(run);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private static Run GetFirstRunWithText(NodeCollection source, string text)
        {
            foreach (Node item in source)
            {
                if (item.GetText() == text)
                {
                    return (Run) item;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks read checksum and actual checksum are equal to expected values.
        /// </summary>
        private static void VerifyChecksum(StructuredDocumentTag sdt, long expReadChecksum, long expPublicChecksum)
        {
            Assert.That((long)BitConverter.ToUInt32(Convert.FromBase64String(sdt.XmlMapping.StoreItemChecksum), 0), Is.EqualTo(expReadChecksum));
            Assert.That((long)sdt.XmlMapping.CustomXmlPart.DataChecksum, Is.EqualTo(expPublicChecksum));
        }
    }
}
