// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using Aspose.TestFx;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for how model handles custom XML, smart tags and control controls.
    /// </summary>
    [TestFixture]
    public class TestMarkup : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestXmlSettings(LoadFormat lf, SaveFormat sf)
        {
            // Check defaults.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestXmlSettingsDefault", lf, sf);
            DocPr docPr = doc.DocPr;

            Assert.That(docPr.SaveXmlDataOnly, Is.EqualTo(false));
            Assert.That(docPr.UseXsltWhenSaving, Is.EqualTo(false));
            Assert.That(StringUtil.HasChars(docPr.SaveThroughXslt), Is.EqualTo(false));

            Assert.That(docPr.ValidateAgainstSchema, Is.EqualTo(true));
            Assert.That(docPr.DoNotUnderlineInvalidXml, Is.EqualTo(false));
            Assert.That(docPr.IgnoreMixedContent, Is.EqualTo(false));
            Assert.That(docPr.SaveInvalidXml, Is.EqualTo(false));

            Assert.That(docPr.AlwaysShowPlaceholderText, Is.EqualTo(false));


            // Check use XSLT settings.
            doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestXmlSettingsUseXslt", lf, sf);
            docPr = doc.DocPr;
            Assert.That(docPr.UseXsltWhenSaving, Is.EqualTo(true));
            Assert.That(docPr.SaveThroughXslt, Is.EqualTo(TestEnvironment.GetTestData(@"Model\Markup\Test.xslt")));
        }



        /// <summary>
        /// Verifies that Custom Xml is successfully read from document parts such as header, footer and body.
        /// </summary>
        /// <remarks>See remarks for <see cref="UnifiedTestCustomXmlMarkup"/>. The schema is applicable
        /// to the test file in this test as well.</remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomXmlDocumentParts(LoadFormat lf, SaveFormat sf)
        {
            CheckCustomXmlMarkup(@"Model\Markup\TestCustomXmlDocumentParts", lf, sf);
        }

        /// <summary>
        /// Verifies that Custom Xml is successfully read and markup levels are set properly.
        /// </summary>
        /// <remarks>See remarks for <see cref="UnifiedTestCustomXmlMarkup"/>. The schema is applicable
        /// to the test file in this test as well.</remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomXmlLevels(LoadFormat lf, SaveFormat sf)
        {
            CheckCustomXmlMarkup(@"Model\Markup\TestCustomXmlLevels", lf, sf);
        }

        /// <summary>
        /// Verifies block-level markups are put inside each other in several combinations.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomXmlInnerBlocks(LoadFormat lf, SaveFormat sf)
        {
            CheckCustomXmlMarkup(@"Model\Markup\TestCustomXmlInnerBlocks", lf, sf);
        }


        /// <summary>
        /// Verifies wordml-like markup used as custom xml.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomXmlWmlMarkup(LoadFormat lf, SaveFormat sf)
        {
            Document doc = CheckCustomXmlMarkup(@"Model\Markup\TestCustomXmlWmlMarkup", lf, sf);
            // verify table read correctly.
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.Rows.Count, Is.EqualTo(1));
        }


        /// <summary>
        /// This file contains custom xml markup which starts outside of table and ends within last table cell.
        /// It means that start and end of custom xml is occurred on different levels in model.
        ///
        /// Word treats this as custom xml wrap whole table.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomXmlDiffLevel(LoadFormat lf, SaveFormat sf)
        {
            Document doc = CheckCustomXmlMarkup(@"Model\Markup\TestCustomXmlDiffLevel", lf, sf);

            // check last paragraph is not wrapped by custom xml markup
            Assert.That(doc.GetNodeById("3.0.0").NodeType, Is.EqualTo(NodeType.Paragraph));

            Table table = (Table)doc.GetNodeById("2.0.0");
            Assert.That(table.Rows.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// This file contains two section and two markups.
        /// First markup wraps paragraphs within on section and should be preserved.
        /// Second wraps paragraphs in oth sections and should be ignored.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomXmlSections(LoadFormat lf, SaveFormat sf)
        {
            Document doc = CheckCustomXmlMarkup(@"Model\Markup\TestCustomXmlSections", lf, sf);

            // verify all sections were read
            Assert.That(doc.GetNodeById("0").NodeType, Is.EqualTo(NodeType.Section));
            Assert.That(doc.GetNodeById("1").NodeType, Is.EqualTo(NodeType.Section));
        }






        /// <summary>
        /// WORDSNET-10749 Remove Custom XML feature from the code.
        /// MS Word violates i4i's patent #5,787,449 and removes custom Xml from the doc, docx, rtf, wml.
        /// See details on http://support.microsoft.com/kb/2761189
        /// We do the same in AW now.
        /// </summary>
        [Test]
        // FOSS Doc/Rtf/Wml load+save are removed; keep only the Docx2Docx roundtrip.
        [TestCase(UnifiedScenario.Docx2Docx)]
        public void UnifiedTestJira10749(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenAndVerifyCustomXmlWarning(@"Model\Markup\TestJira10749", TestUtil.GetLoadFormat(scenario));
            doc = TestUtil.SaveOpen(doc, @"Model\Markup\TestJira10749", scenario | UnifiedScenario.NoGold);

            // Check that content inside custom xml was successfully round-tripped.
            Paragraph para = (Paragraph) doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.GetText(), Is.EqualTo("TextBefore Text in custom Xml TextAfter\f"));
        }

        // FOSS TestDefect7105 removed: RTF->DOC smart-tag namespace test; both formats removed.

        /// <summary>
        /// Utility verification. Used in <see cref="UnifiedTestDefect4995" />.
        /// </summary>
        private static void VerifyProperty(CustomXmlProperty property, string name, string uri, string value)
        {
            Assert.That(name, Is.EqualTo(property.Name));
            Assert.That(uri, Is.EqualTo(property.Uri));
            Assert.That(value, Is.EqualTo(property.Value));
        }

        /// <summary>
        /// WORDSNET-16454 Preserve Custom Payload in OOXML
        /// </summary>
        [Test]
        public void TestCustomParts()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestCustomParts.docx");

            Assert.That(doc.PackageCustomParts.Count, Is.EqualTo(2));

            // This part is an arbitrary part attached to the package.
            CustomPart part = doc.PackageCustomParts[0];
            Assert.That(part.Name, Is.EqualTo("/payload/payload_on_package.test"));
            Assert.That(part.ContentType, Is.EqualTo("mytest/somedata"));
            Assert.That(part.RelationshipType, Is.EqualTo("http://mytest.payload.internal"));
            Assert.That(part.IsExternal, Is.EqualTo(false));
            Assert.That(part.Data.Length, Is.EqualTo(18));

            // This is an external relationship attached to the package.
            part = doc.PackageCustomParts[1];
            Assert.That(part.Name, Is.EqualTo("http://www.aspose.com/Images/aspose-logo.jpg"));
            Assert.That(part.ContentType, Is.EqualTo(""));
            Assert.That(part.RelationshipType, Is.EqualTo("http://mytest.payload.external"));
            Assert.That(part.IsExternal, Is.EqualTo(true));
            Assert.That(part.Data.Length, Is.EqualTo(0));
        }





        // FOSS TestJira6148_NotOOXML removed: tests non-OOXML SDT data-binding via a Pdf save (removed fixed format).

        /// <summary>
        /// WORDSNET-9811 Paragraph and some content is incorrectly rendering to next page in PDF
        /// Cell level SDT is incorrectly updated from placeholder.
        /// </summary>
        /// <remarks>
        /// AM. This issue was introduced by WORDSNET-9337
        /// To quickly remove regression I decided to limit WORDSNET-9337 functionality by inline SDT only and
        /// raised separate WORDSNET-9886
        /// </remarks>
        [Test]
        public void TestJira9811()
        {
            // FOSS Save via Docx (Doc removed) to update SDTs.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestJira9811", LoadFormat.Docx, SaveFormat.Docx);

            // Verify problematic content.
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Row row = table.Rows[2];

            Assert.That(row.Cells.Count, Is.EqualTo(2));
            Assert.That(row.Cells[0].GetText(), Is.EqualTo("Help send the high school choir to sing \rin New York City\a"));
            Assert.That(row.Cells[1].GetText(), Is.EqualTo("Help send the high school choir to sing \rin New York City\a"));
        }



        // FOSS TestJira11496 removed: reads a glossary from a DOC (.dot) template; DOC load removed.

        // FOSS TestDocTwoBlocks removed: reads AutoText glossary from a DOC (.dot); DOC load removed.

        // FOSS TestDocTables removed: reads AutoText-with-tables glossary from a DOC (.dot); DOC load removed.

        // FOSS TestDocShapes removed: reads AutoText-with-shape glossary from a DOC (.dot); DOC load removed.

        /// <summary>
        /// Relates to WORDSNET-9962 Customer code snippet.
        /// </summary>
        private static void CreateCover(Document docx, Document docBuildingBlocks, string coverName)
        {
            GlossaryDocument glossaryDocument = docBuildingBlocks.GlossaryDocument;
            foreach (BuildingBlock buildingBlock in glossaryDocument.BuildingBlocks)
            {
                if ((buildingBlock.Gallery == BuildingBlockGallery.CoverPage) && (buildingBlock.Name == coverName))
                {
                    // clone section which you want to keep layout of
                    Section sec = docx.FirstSection.Clone();
                    // remove existing content of this section
                    sec.RemoveAllChildren();
                    // append content of buildingBlock to this section
                    foreach (Node node in ((Section)buildingBlock.FirstChild).GetChildNodes(NodeType.Any, false))
                        sec.AppendChild(docx.ImportNode(node, true));
                    docx.InsertAfter(sec, docx.FirstChild);
                    break;
                }
            }
        }

        private static void TestCustomXmlParts(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestCustomXmlParts", lf, sf);

            // There are several parts that are auto generated by MS Word and it seems to generate one less for DOC files.
            int expectedPartCount = (lf == LoadFormat.Doc) ? 5 : 6;
            Assert.That(doc.CustomXmlParts.Count, Is.EqualTo(expectedPartCount));

            // Check details of one of the Custom XML Data Storage parts.
            CustomXmlPart customXmlPart = doc.CustomXmlParts.GetById("{B0048027-F405-4DA1-AB3E-1A1F5695C61E}");
            Assert.That(customXmlPart.Data.Length, Is.EqualTo(8312));
            Assert.That(customXmlPart.Id, Is.EqualTo("{B0048027-F405-4DA1-AB3E-1A1F5695C61E}"));
            Assert.That(customXmlPart.Schemas.Count, Is.EqualTo(11));
            Assert.That(customXmlPart.Schemas[0], Is.EqualTo("http://schemas.microsoft.com/office/2006/metadata/contentType"));
            Assert.That(customXmlPart.Schemas[10], Is.EqualTo("http://schemas.microsoft.com/office/internal/2005/internalDocumentation"));
        }

        /// <summary>
        /// WORDSNET-15050 Old incorrect value is rendered inside a SDT in PDF
        /// Word extracts TermInfo/TermName element if Terms element was selected.
        /// </summary>
        [Test]
        public void TestJira15050()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira15050.docx");
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx); // FOSS Doc -> Docx validation

            Assert.That(doc.GetText(), Is.EqualTo("Test\r\rTestValue2\f"));
        }

        // FOSS TestJira13324 removed: RTF CustomXmlParts datastore roundtrip (Rtf2Rtf); RTF removed.

        // FOSS TestJira13324A removed: RTF CustomXmlParts datastore roundtrip (Rtf2Rtf); RTF removed.

        // FOSS TestJira16352 removed: loads CustomXmlParts from RTF; RTF load removed.

        /// <summary>
        /// Tests creation of a custom XML part by its XML code.
        /// </summary>
        [Test]
        public void TestCreationOfCustomXmlPartByXml()
        {
            Document doc = new Document();
            CustomXmlPart customXmlPart = doc.CustomXmlParts.Add("ID", "<a>Я</a>");

            Assert.That(customXmlPart.Id, Is.EqualTo("ID"));
            Assert.That(ArrayUtil.DumpArray(customXmlPart.Data), Is.EqualTo("3C 61 3E D0 AF 3C 2F 61 3E "));
        }




        /// <summary>
        /// WORDSNET-23544 Document missing sections after saving
        /// Empty content SDT should be handled in special way.
        /// </summary>
        [TestCase("//Body/Paragraph[2]/following-sibling::*", 1994680633)]
        [TestCase("//Body/Paragraph[3]/following-sibling::*", 2120024171)]
        [TestCase("//Body/Table/Row[1]/Cell[1]/Paragraph/StructuredDocumentTag", -220439517)]
        [TestCase("//Body/Table/Row[2]/Cell[1]/StructuredDocumentTag", 1437783286)]
        [TestCase("//Body/Paragraph[9]/following-sibling::*", -1122755837)]
        [TestCase("//Body/Paragraph[10]/following-sibling::*", -1662928133)]
        [TestCase("//Body/Paragraph[13]/StructuredDocumentTag", 854304033)]
        [TestCase("//Body/Paragraph[13]/following-sibling::*", -1100014938)]
        [TestCase("//Body/Paragraph[15]/following-sibling::*", -1412542219)]
        [TestCase("//Body/Paragraph[16]/following-sibling::*", -831825925)]
        [TestCase("//Body/Paragraph[19]/StructuredDocumentTag", 1941408926)]
        [TestCase("//Body/Paragraph[19]/following-sibling::*", -1405287596)]
        public void Test23544_2013(string xPath, int id)
        {
            CheckSdtLocation(@"Model\Markup\Test23544.docx", MsWordVersion.Word2013, xPath, id);
        }

        /// <summary>
        /// WORDSNET-23544 Document missing sections after saving
        /// Empty content SDT should be handled in special way.
        /// </summary>
        [TestCase("//Body/Paragraph[2]/following-sibling::*", 1994680633)]
        [TestCase("//Body/Paragraph[3]/following-sibling::*", 2120024171)]
        [TestCase("//Body/Table/Row[1]/Cell[1]/StructuredDocumentTag", -220439517)]
        [TestCase("//Body/Table/Row[2]/Cell[1]/StructuredDocumentTag", 1437783286)]
        [TestCase("//Body/Paragraph[9]/following-sibling::*", -1122755837)]
        [TestCase("//Body/Paragraph[10]/following-sibling::*", -1662928133)]
        [TestCase("//Body/Paragraph[12]/following-sibling::*", 854304033)]
        [TestCase("//Body/Paragraph[13]/following-sibling::*", -1100014938)]
        [TestCase("//Body/Paragraph[15]/following-sibling::*", -1412542219)]
        [TestCase("//Body/Paragraph[16]/following-sibling::*", -831825925)]
        [TestCase("//Body/Paragraph[18]/following-sibling::*", 1941408926)]
        [TestCase("//Body/Paragraph[19]/following-sibling::*", -1405287596)]
        public void Test23544_2019(string xPath, int id)
        {
            CheckSdtLocation(@"Model\Markup\Test23544.docx", MsWordVersion.Word2019, xPath, id);
        }







        /// <summary>
        /// WORDSNET-27401 Children count returned by GetChildNodes is incorrect for nested SDTs.
        /// Tests the correctness of NodeCollection filling in case of nested StructuredDocumentTags.
        /// </summary>
        [Test]
        public void Test27401()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test27401.docx");

            // Checks child paragraphs of the parent SDT.
            IStructuredDocumentTag sdt1 = doc.Range.StructuredDocumentTags[0];

            NodeCollection shallowParagraphs1 = sdt1.GetChildNodes(NodeType.Paragraph, false);
            Assert.That(shallowParagraphs1.Count, Is.EqualTo(2));
            NodeCollection deepParagraphs1 = sdt1.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(deepParagraphs1.Count, Is.EqualTo(4));

            // Checks child paragraphs of the child SDT.
            IStructuredDocumentTag sdt2 = doc.Range.StructuredDocumentTags[1];

            NodeCollection shallowParagraphs2 = sdt2.GetChildNodes(NodeType.Paragraph, false);
            Assert.That(shallowParagraphs2.Count, Is.EqualTo(2));
            NodeCollection deepParagraphs2 = sdt2.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(deepParagraphs2.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Checks SDT is placed into specified location and has expected Id.
        /// </summary>
        private static void CheckSdtLocation(string testName, MsWordVersion mswVersion, string xPath, int expectedId)
        {
            LoadOptions lo = new LoadOptions() { MswVersion = mswVersion };
            Document doc = TestUtil.Open(testName, lo);

            Node node = doc.SelectSingleNode(xPath);

            Assert.That(node.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(((StructuredDocumentTag)node).Id, Is.EqualTo(expectedId));
        }

        /// <summary>
        /// Round-trips document. Checks warning for CustomXmlMarkup and verifies gold.
        /// </summary>
        private static Document CheckCustomXmlMarkup(string filename, LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenAndVerifyCustomXmlWarning(filename, lf);
            return TestUtil.SaveOpen(doc, filename, lf, sf);
        }
    }
}
