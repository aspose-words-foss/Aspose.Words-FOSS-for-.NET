// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2010 by Denis Darkin
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Aspose.Common;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Test Structured Document Tags (SDT).
    /// See Defect 4295 - Support Structured Document Tags (SDT) (Content Controls).
    /// Sdt required a lot of changes across all AW code, not only conversions, which required additional testing.
    /// </summary>
    /// <remarks>
    /// Additional tests related to Model and public API parts of Sdt feature are placed in the Model testing area
    /// <see cref="TestSdt"/>
    /// </remarks>
    [TestFixture]
    public class TestImportDocxSdt
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
        /// Validate that placeholder elements of sdt are read if Sdt is a part of GlossaryDocument itself.
        /// </summary>
        [Test]
        public void TestGlossarySdtPlaceholderText()
        {
            Document doc = TestUtil.Open(@"Model\BuildingBlocks\TestBuiltIn.dotx");
            GlossaryDocument glossary = doc.GlossaryDocument;

            NodeCollection sdtCollection = glossary.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdtCollection, IsNot.Null());

            StructuredDocumentTag sdt = ((StructuredDocumentTag)(sdtCollection[100]));
            Assert.That(sdt.Placeholder.GetText(), Is.EqualTo("[Type text]\f"));
        }

        /// <summary>
        /// Test that properties of sdt-wrapped paragraphs are correct.
        /// </summary>
        [Test]
        public void TestSdtPara()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Markup\TestSdtPara.docx");
            Paragraph paragraph = (Paragraph)
                ((CompositeNode) doc.FirstSection.HeadersFooters[0].FirstChild).FirstNonMarkupCompositeDescendant;
            Assert.That(paragraph.GetText(), Is.EqualTo("Para wrapped in double Sdt present in header\r"));
            Assert.That(paragraph.IsEndOfHeaderFooter, Is.True);

            Table table0 = doc.FirstSection.Body.Tables[0];
            paragraph = (Paragraph)table0.Rows[0].Cells[0].FirstNonMarkupCompositeDescendant;
            Assert.That(paragraph.GetText(), Is.EqualTo("First Cell-nested para wrapped with sdt\r"));
            Assert.That(paragraph.IsStartOfCell, Is.True);
            Assert.That(paragraph.IsStartOfRow, Is.True);
            Assert.That(paragraph.IsStartOfTable, Is.True);
            Assert.That(paragraph.IsEndOfCell, Is.False);

            paragraph = (Paragraph)paragraph.NextNonMarkupCompositeLimited;
            Assert.That(paragraph.GetText(), Is.EqualTo("Second cell-nested para wrapped with sdt\x0007"));
            Assert.That(paragraph.IsStartOfCell, Is.False);
            Assert.That(paragraph.IsStartOfRow, Is.False);
            Assert.That(paragraph.IsStartOfTable, Is.False);
            Assert.That(paragraph.IsEndOfCell, Is.True);

            //check that non sdt para props also work after sdt modifications
            paragraph = (Paragraph)table0.Rows[0].Cells[1].FirstNonMarkupCompositeDescendant;
            Assert.That(paragraph.GetText(), Is.EqualTo("Simple para, no sdt\x0007"));
            Assert.That(paragraph.IsStartOfCell, Is.True);
            Assert.That(paragraph.IsEndOfRow, Is.True);
            Assert.That(paragraph.IsStartOfTable, Is.False);
            Assert.That(paragraph.IsEndOfCell, Is.True);

            paragraph = (Paragraph)table0.Rows[1].Cells[1].LastNonMarkupCompositeDescendant;
            Assert.That(paragraph.GetText(), Is.EqualTo("Last-row, last-cell, para wrapped with sdt\x0007"));
            Assert.That(paragraph.IsStartOfCell, Is.True);
            Assert.That(paragraph.IsEndOfCell, Is.True);
            Assert.That(paragraph.IsStartOfRow, Is.False);
            Assert.That(paragraph.IsEndOfRow, Is.True);
            Assert.That(paragraph.IsStartOfTable, Is.False);
            Assert.That(paragraph.IsEndOfTable, Is.True);

            paragraph = (Paragraph)table0.NextSibling.NextSibling.NextNonMarkupCompositeLimited;
            Assert.That(paragraph.IsInMainTextStory, Is.True);
            Assert.That(paragraph.GetText(), Is.EqualTo("Para wrapped in RichText Sdt present in main story\r"));

            paragraph = (Paragraph)paragraph.NextNonMarkupCompositeLimited.NextNonMarkupCompositeLimited;
            Assert.That(paragraph.PreviousSibling.NodeType, Is.EqualTo(NodeType.CommentRangeStart));
            Assert.That(paragraph.FirstChild.GetText(), Is.EqualTo("Commented "));
            Assert.That(paragraph.FirstChild.NextSibling.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(paragraph.FirstChild.NextSibling.GetText(), Is.EqualTo("text"));
            Assert.That(paragraph.FirstChild.NextSibling.NextSibling.NodeType, Is.EqualTo(NodeType.Comment));

            paragraph = (Paragraph)paragraph.NextNonMarkupCompositeLimited.NextNonMarkupCompositeLimited;
            Assert.That(paragraph.GetText(), Is.EqualTo(" HYPERLINK \"http://www.sharewareplaza.com/hdd-repairer-seagate-barracuda-7200-7-downloads_51134.html\" My hyperlink wrapped with Sdt node\r"));
        }

        /// <summary>
        /// Verify that elements at various levels are accessible if wrapped with Sdt parent node.
        /// </summary>
        [Test]
        public void TestSdtLevels()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\TestSdtLevels.docx");
            Assert.That(doc.Sections[0].Body.Tables.Count, Is.EqualTo(1));
            Table testedTable = doc.Sections[0].Body.Tables[0];
            Assert.That(testedTable.Rows.Count, Is.EqualTo(2));

            const string testedRowContents = "Table row rich text sdt control test\x0007";
            Assert.That(testedTable.LastRow.FirstCell.GetText(), Is.EqualTo(testedRowContents));
            Assert.That(testedTable.FirstRow.NextRow.FirstCell.GetText(), Is.EqualTo(testedRowContents));

            const string testedTableContents = "Table wide sdt control\x0007";
            Assert.That(testedTable.FirstRow.FirstCell.GetText(), Is.EqualTo(testedTableContents));
            Assert.That((testedTable.LastRow.PreviousRow).FirstCell.GetText(), Is.EqualTo(testedTableContents));

            const string testedCellText = "Cell richText sdt control\x0007";
            Assert.That(testedTable.FirstRow.FirstCell.NextNonMarkupCompositeLimited.GetText(), Is.EqualTo(testedCellText));
            Assert.That(testedTable.FirstRow.LastCell.PreviousNonMarkupCompositeLimited.GetText(), Is.EqualTo(testedCellText));
        }

        /// <summary>
        /// Verify that node collection enumeration works in presence of wrapping Sdt nodes in both deep and flat modes.
        /// in the test doc two tables reside inside an Sdt and one is inside main story.
        /// </summary>
        [Test]
        public void TestMultipleTablesInSdt()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\TestMultipleTablesInSdt.docx");
            ValidateNodeCollection(new NodeCollection(doc.Sections[0].Body, NodeType.Table, true));
            ValidateNodeCollection(new NodeCollection(doc.Sections[0].Body, NodeType.Table, false));
        }

        private static void ValidateNodeCollection(NodeCollection nodes)
        {
            Assert.That(nodes.Count, Is.EqualTo(3));
            for (int i = 0; i < nodes.Count; i++)
                Assert.That(nodes[i].NodeType, Is.EqualTo(NodeType.Table));

            Assert.That(nodes[0].GetText(), Is.EqualTo("Table 1\x0007\x0007\x0007"));
            Assert.That(nodes[1].GetText(), Is.EqualTo("Table 2\x0007\x0007\x0007\x0007"));
            Assert.That(nodes[2].GetText(), Is.EqualTo("Table 3\x0007\x0007\x0007"));
        }


        /// <summary>
        /// Validates particular test <see cref="TestSdtFullCollection"/>.
        /// Is called twice to validate that sdt titles are as expected.
        /// </summary>
        /// <param name="allSdt"></param>
        private static void ValidateTitles(NodeCollection allSdt)
        {
            Assert.That(allSdt.Count, Is.EqualTo(gAliasesForTestSdtFullCollection.Length));
            for (int i = 0; i < gAliasesForTestSdtFullCollection.Length; i++)
            {
                try
                {
                    Assert.That(((StructuredDocumentTag)allSdt[i]).Title, Is.EqualTo(gAliasesForTestSdtFullCollection[i]));
                }
                catch
                {
                    throw new InvalidOperationException(String.Format(
                        "Mismatch at {0}-tht Sdt, required {1} Title name but was {2}.",
                        i, gAliasesForTestSdtFullCollection[i], ((StructuredDocumentTag)allSdt[i]).Title));
                }
            }
        }


        // FOSS TestLayout removed: it verifies the layout engine doesn't crash on SDT nodes by rendering to
        // Pdf, a removed format (the FOSS build has no fixed-page rendering).


        /// <summary>
        /// Verify that sdt content is read.
        /// </summary>
        [Test]
        public void TestContentControls()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Markup\TestContentControls.docx");

            // Inline content control.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.GetText(), Is.EqualTo("Inline-level content control: Test text\r"));

            // Block level content control with two paragraphs.
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(para.GetText(), Is.EqualTo("Block-level content control in body.\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            Assert.That(para.GetText(), Is.EqualTo("Contains two paragraphs.\r"));

            // Block level content control contains a table.
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.GetText(), Is.EqualTo("Block-level content control in body.\x0007Contains a 2x2 table\x0007\x0007\x0007\x0007\x0007"));

            table = (Table)doc.GetChild(NodeType.Table, 1, true);

            // Row-level content control
            Row row = table.Rows[0];
            Assert.That(row.GetText(), Is.EqualTo("Row-level content control\x0007\x0007\x0007"));

            // Cell-level content control
            row = table.Rows[1];
            Cell cell = row.Cells[0];
            Assert.That(cell.GetText(), Is.EqualTo("Cell-level content control\x0007"));

            // Cell level content control with nested block level content control with nested inline level control.
            cell = row.Cells[1];
            Assert.That(cell.GetText(), Is.EqualTo("Block level-content control in cell.\rContains two paragraphs. " +
                "Also a nested content control.\r\x0007"));
        }

        /// <summary>
        /// Test1: Validate  the following child sdt elements of the spec:
        /// - dataBinding,
        ///     - xPath,
        ///     - storeItemID
        /// - alias,
        /// - tag,
        /// - id,
        /// - placeholder,
        /// - showingPlcHdr
        /// - date
        ///     - dateFormat,
        ///     - lid,
        ///     - storeMappedDataAs,
        ///     - calendar
        /// - comboBox
        ///     - listItem
        /// - dropDownList
        ///     - listItem
        /// </summary>
        [Test]
        public void TestSdtOpenSave1()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\TestSdtOpenSave1.docx");
            ValidateSdtNodesTest1(doc.FirstSection.Body.GetChildNodes(NodeType.Any, false));
            doc = TestUtil.SaveOpen(doc, @"ImportDocx\Markup\TestSdtOpenSave1.docx", LoadFormat.Docx, SaveFormat.Docx);
            ValidateSdtNodesTest1(doc.FirstSection.Body.GetChildNodes(NodeType.Any, false));
        }

        /// <summary>
        /// Tests that Office2010 SDT checkbox elements are read/written properly.
        /// </summary>
        [Test]
        public void TestSdtCheckbox()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Markup\TestSdtCheckbox.docx");

            // First checkbox has custom characters and font.
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetNodeById("0.0.0");
            Assert.That(sdt.ControlProperties.Type, Is.EqualTo(SdtType.Checkbox));
            SdtCheckBox checkbox = (SdtCheckBox)sdt.ControlProperties;
            Assert.That(checkbox.Checked, Is.True);
            SdtCheckBoxStateInfo stateInfo = checkbox.CheckedStateInfo;
            Assert.That(stateInfo.FontName, Is.EqualTo("Times New Roman"));
            Assert.That(stateInfo.CharacterCode, Is.EqualTo(0x00A9));
            stateInfo = checkbox.UncheckedStateInfo;
            Assert.That(stateInfo.FontName, Is.EqualTo("Times New Roman"));
            Assert.That(stateInfo.CharacterCode, Is.EqualTo(0x00AE));

            // Second is default checkbox.
            sdt = (StructuredDocumentTag)doc.GetNodeById("1.0.0");
            Assert.That(sdt.ControlProperties.Type, Is.EqualTo(SdtType.Checkbox));
            checkbox = (SdtCheckBox)sdt.ControlProperties;
            Assert.That(checkbox.Checked, Is.False);
            stateInfo = checkbox.CheckedStateInfo;
            Assert.That(stateInfo.FontName, Is.EqualTo("MS Gothic"));
            Assert.That(stateInfo.CharacterCode, Is.EqualTo(0x2612));
            stateInfo = checkbox.UncheckedStateInfo;
            Assert.That(stateInfo.FontName, Is.EqualTo("MS Gothic"));
            Assert.That(stateInfo.CharacterCode, Is.EqualTo(0x2610));
        }

        /// <summary>
        /// Core body of Test 1
        /// </summary>
        private static void ValidateSdtNodesTest1(NodeCollection nodes)
        {
            StructuredDocumentTag testTag = GetValidSdt(nodes, 5);
            Assert.That(testTag.ControlProperties.Type, Is.EqualTo(SdtType.Date));
            Assert.That(testTag.XmlMapping.IsEmpty, Is.False);
            Assert.That(testTag.XmlMapping.XPath, Is.EqualTo(@"/root[1]/sale[1]/type[1]"));
            Assert.That(testTag.XmlMapping.StoreItemId, Is.EqualTo("{DD7E7B92-2A46-4758-BAF1-556DA4C1931C}"));
            Assert.That(testTag.XmlMapping.PrefixMappings, Is.EqualTo(""));
            Assert.That(testTag.Title, Is.EqualTo("SaleDate"));
            Assert.That(testTag.Tag, Is.EqualTo("saleDateTag"));
            Assert.That(testTag.Id, Is.EqualTo(91945806));
            Assert.That(testTag.PlaceholderName, Is.EqualTo("DefaultPlaceholder_22675705"));
            Assert.That(testTag.Placeholder.GetText(), Is.EqualTo("Click here to enter a date.\f"));
            Assert.That(testTag.IsShowingPlaceholderText, Is.True);

            SdtDate date = (SdtDate)testTag.ControlProperties;
            Assert.That(date.FullDate, Is.EqualTo(FormatterPal.XmlToDateTime("2010-07-07T00:00:00Z")));
            Assert.That(date.DateFormat, Is.EqualTo("M/d/yyyy"));
            Assert.That(date.Lid, Is.EqualTo(LocaleConverter.DocxTagToLocale("en-US")));
            Assert.That(date.CalendarType, Is.EqualTo(SdtCalendarType.Gregorian));

            testTag = GetValidSdt(nodes, 9);
            Assert.That(testTag.ControlProperties.Type, Is.EqualTo(SdtType.ComboBox));
            VerifyDropDownListBase(((SdtComboBox)testTag.ControlProperties).ListItems);

            testTag = GetValidSdt(nodes, 12);
            Assert.That(testTag.ControlProperties.Type, Is.EqualTo(SdtType.DropDownList));
            VerifyDropDownListBase(((SdtDropDownList)testTag.ControlProperties).ListItems);
        }

        /// <summary>
        /// Verify that we look at an Sdt node and return it.
        /// </summary>
        private static StructuredDocumentTag GetValidSdt(NodeCollection nodes, int index)
        {
            Assert.That(nodes[index] is StructuredDocumentTag, Is.True);
            return (StructuredDocumentTag)nodes[index];
        }

        /// <summary>
        /// Verify comboBox and dropDownList.
        /// </summary>
        private static void VerifyDropDownListBase(SdtListItemCollection listItemCollection)
        {
            Assert.That(listItemCollection.SelectedValue, Is.EqualTo(null));
            Assert.That(listItemCollection.Count, Is.EqualTo(1));
            Assert.That(listItemCollection[0].Value, Is.EqualTo("Choose an item."));

            // andrnosk: MS Word does not write DisplayText for "Choose an item.", we do the same now.
            Assert.That(listItemCollection[0].DisplayText, Is.EqualTo(""));
        }

        /// <summary>
        /// Test that roundtrip of a lot of nested sdts is working and also a check that
        /// flat node collection with skipping sdts is working properly.
        /// </summary>
        [Test]
        public void TestNestedInlineSdt()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Markup\TestNestedInlineSdt.docx");

            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            NodeCollection runs = para.GetChildNodes(NodeType.Run, false);

            // Build a string from start to end to check forward iteration.
            StringBuilder startToBack = new StringBuilder();
            for (int i = 0; i < runs.Count; i++)
                startToBack.Append(runs[i].GetText());

            // Build a string from end to start to check backward iteration.
            StringBuilder backToStart = new StringBuilder();
            for (int i = runs.Count - 1; i >= 0; i--)
                backToStart.Insert(0, runs[i].GetText());

            string expectedString = para.GetChildrenText();
            Assert.That(expectedString.Length, Is.EqualTo(574));
            Assert.That(expectedString, Is.EqualTo(startToBack.ToString()));
            Assert.That(expectedString, Is.EqualTo(backToStart.ToString()));
        }

        /// <summary>
        /// Test that a really lot of nested sdts at various levels and flat collections still enumerates nodes properly.
        /// </summary>
        [Test]
        public void TestNestedVariousSdt()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Markup\TestNestedVariousSdt.docx");

            const string row0Text = "AAA\rClick here to enter text.BBB\x0007CCC\x0007\x0007DDD\x0007EEE\x0007\x0007FFF\x0007GGG\x0007";
            const string row1Text = "HHH\x0007III\rJJJ\x0007";
            const string table0Text = row0Text + "\x0007" + row1Text + "\x0007";

            // Test at the row level.
            Table table0 = doc.FirstSection.Body.Tables[0];
            Assert.That(table0.GetText(), Is.EqualTo(table0Text));
            CheckFlatCollectionText(table0.Rows, table0Text);

            // Test at the cell level.
            Row row0 = table0.FirstRow;
            CheckFlatCollectionText(row0.Cells, row0Text);

            // Test at the block level inside cells.
            Cell cell0 = row0.FirstCell;
            CheckFlatCollectionText(cell0.Paragraphs, "AAA\rFFF\x0007");
            CheckFlatCollectionText(cell0.Tables, "Click here to enter text.BBB\x0007CCC\x0007\x0007DDD\x0007EEE\x0007\x0007");

            // Test at the inline level in a nested table.
            Cell cellDDD = (Cell)doc.GetChild(NodeType.Cell, 3, true);
            Assert.That(cellDDD.GetText(), Is.EqualTo("DDD\x0007"));
            CheckFlatCollectionText(cellDDD.GetChildNodes(NodeType.Any, false), "DDD\x0007");
        }


        /// <summary>
        /// This is just a way to check that iterating over a flat collection and skipping SDT nodes works.
        /// The code loops and appends GetText of each node in the collection and compares with the expected string.
        /// </summary>
        private static void CheckFlatCollectionText(NodeCollection nodes, string expectedString)
        {
            // Check from to back enumeration is correct.
            StringBuilder startToBack = new StringBuilder();
            for (int i = 0; i < nodes.Count; i++)
                startToBack.Append(nodes[i].GetText());

            // Check back to front enumeration is correct.
            StringBuilder backToStart = new StringBuilder();
            for (int i = nodes.Count - 1; i >= 0; i--)
                backToStart.Insert(0, nodes[i].GetText());

            Assert.That(startToBack.ToString(), Is.EqualTo(expectedString));
            Assert.That(backToStart.ToString(), Is.EqualTo(expectedString));
        }


        private const string Term1 = "Term-1", term2 = "Term-2";
        private const string Guid1 = "34e9f913-0364-4e37-a4a7-c278ba81f3f9", guid2 = "c0fa27e1-9949-4b70-8ba6-122d77d6f0ab";

        /// <summary>
        /// WORDSSP-243 - SharePoint 2013 ListItem Metadata and Word Quickparts not in sync
        /// It is necessary to introduce new logic for Get/Set taxomony SDT fields
        /// </summary>
        [Test]
        public void TestSdtGetSet()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\TestSdtGetSet.docx");
            string sdtValue = GetSdtValue(doc, "TestColumnTaxHTField0");
            Assert.That(Term1, Is.EqualTo(sdtValue));

            List<string> guids = new List<string>() { guid2 };
            List<string> termLabels = new List<string>() { term2 };
            SetSdtValue(doc, "TestColumnTaxHTField0", "TestColumn", guids, termLabels);
            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            doc = TestUtil.SaveOpen(doc, @"ImportDocx\Markup\TestSdtGetSet.docx", so, false);
            string sdtNewValue = GetSdtValue(doc, "TestColumnTaxHTField0");
            Assert.That(term2, Is.EqualTo(sdtNewValue));
        }

        /// <summary>
        /// WORDSSP-243 - SharePoint 2013 ListItem Metadata and Word Quickparts not in sync.
        /// It is necessary to introduce new logic for Get/Set taxomony SDT fields.
        /// </summary>
        [Test]
        public void TestSdtGetSetMulti()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\TestSdtGetSetMulti.docx");
            string sdtValue1 = GetSdtValue(doc, "TestColumnTaxHTField0");
            string sdtValue2 = GetSdtValue(doc, "TestColumnMultiTaxHTField0");
            Assert.That(Term1, Is.EqualTo(sdtValue1));
            Assert.That(Term1 + "; " + term2, Is.EqualTo(sdtValue2));

            List<string> termGuids1 = new List<string>() { guid2 };
            List<string> termLabels1 = new List<string>() { term2 };
            List<string> termGuids2 = new List<string>() { guid2, Guid1 };
            List<string> termLabels2 = new List<string>() { term2, Term1 };
            SetSdtValue(doc, "TestColumnTaxHTField0", "TestColumn", termGuids1, termLabels1);
            SetSdtValue(doc, "TestColumnMultiTaxHTField0", "TestColumnMulti", termGuids2, termLabels2);

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            doc = TestUtil.SaveOpen(doc, @"ImportDocx\Markup\TestSdtGetSetMulti.docx", so, false);
            string sdtValueNew1 = GetSdtValue(doc, "TestColumnTaxHTField0");
            string sdtValueNew2 = GetSdtValue(doc, "TestColumnMultiTaxHTField0");
            Assert.That(term2, Is.EqualTo(sdtValueNew1));
            Assert.That(term2 + "; " + Term1, Is.EqualTo(sdtValueNew2));
        }

        // FOSS TestJira16659 removed: it verifies how an SDT flushes relative to an altChunk reference node;
        // the input's altChunk had to be flattened via Word for the FOSS build, which removes that behavior.


        /// <summary>
        /// WORDSNET-18731 DOCX to HtmlFixed conversion issue with inline content control.
        /// Implemented inserting Shape node into inline SDT in DocumentInserter that allows to update bound SDT data properly.
        /// </summary>
        [Test]
        public void Test18731()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\Test18731.docx");

            Paragraph para = doc.FirstSection.Body.Paragraphs[1];
            StructuredDocumentTag sdt = (StructuredDocumentTag)para.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Shape shape = (Shape)sdt.FirstChild;

            Assert.That(shape, IsNot.Null());
        }


        /// <summary>
        /// Related with WORDSNET-24520
        /// Checks how AW reads and writes "storeItemChecksum" attribute for different SDT types.
        /// </summary>
        [Test]
        public void Test24520SdtTypes()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Markup\Test24520SdtTypes.docx");
            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            // Check checksum after import.
            foreach (StructuredDocumentTag sdt in sdts)
                 Assert.That(StringUtil.HasChars(sdt.XmlMapping.StoreItemChecksum), Is.EqualTo(sdt.SdtType == SdtType.RichText));

            // Check exported markup.
            XmlDocument xmlDoc = DocxExportContext.SaveAndGetXmlDocument(doc, new OoxmlSaveOptions(), @"word/document.xml");
            List<XmlNode> bindings = new List<XmlNode>();

            XmlNodeList binding = xmlDoc.GetElementsByTagName("w15:dataBinding");
            for (int i= 0; i < binding.Count; i++)
                bindings.Add(binding[i]);

            binding = xmlDoc.GetElementsByTagName("w:dataBinding");
            for (int i = 0; i < binding.Count; i++)
                bindings.Add(binding[i]);

            Assert.That(bindings.Count, Is.EqualTo(10));
            foreach(XmlNode xmlNode in bindings)
            {
                XmlAttribute storeItemChecksum = (XmlAttribute)xmlNode.Attributes["w16sdtdh:storeItemChecksum"];

                // Checksum must be written only for the reach textbox SDT.
                if (xmlNode.Attributes["w:xpath"].Value.Contains("richtextbox"))
                    Assert.That(StringUtil.HasChars(storeItemChecksum.Value), Is.True);
                else
                    Assert.That(storeItemChecksum, Is.Null, "unexpected storeItemChecksum");
            }
        }

        /// <summary>
        /// Related with WORDSNET-24520
        /// Checks that appropriate namespace is writing for the "storeItemChecksum" attribute.
        /// </summary>
        [Test]
        public void Test24520Namespace()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Markup\Test24520Namespace", UnifiedScenario.Docx2DocxNoGold);
            DocxExportContext exporter = new DocxExportContext(doc, @"word/document.xml");

            // There are related issues like WORDSNET-23287, WORDSNET-23386 and etc.
            // Which no longer write "storeItemChecksum" attribute. So, check namespace for the rich textbox SDT type.
            XmlNode docNode = exporter.GetXmlNode("//w:document");
            Assert.That(docNode.Attributes["xmlns:w16sdtdh"], IsNot.Null());
        }

        /// <summary>
        /// Get text value of SDT field with a given tag name.
        /// </summary>
        /// <returns>Text value (or null if SDT field with given tag name was not found)</returns>
        private static string GetSdtValue(Document doc, string tagName)
        {
            NodeCollection nodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (Node node in nodes)
            {
                StructuredDocumentTag sdtNode = (StructuredDocumentTag)node;
                if (!sdtNode.Tag.Equals(tagName))
                    continue;

                Node sdtContentNode = sdtNode.GetChild(NodeType.Run, 0, true);
                if (sdtContentNode == null)
                    return null;

                Run run = (Run)sdtContentNode;
                return run.Text;
            }
            return null;
        }

        /// <summary>
        /// Set text value for SDT field with a given tag/alias name.
        /// </summary>
        /// <returns>1 - text value was set, -1 - SDT field with given tag name was not found</returns>
        public static int SetSdtValue(Document doc, string tagName, string aliasName,
            IList<string> termGuids, IList<string> termLabels)
        {
            NodeCollection nodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (Node node in nodes)
            {
                StructuredDocumentTag sdtNode = (StructuredDocumentTag)node;
                if (!sdtNode.Tag.Equals(tagName))
                    continue;

                Node sdtContentNode = sdtNode.GetChild(NodeType.Run, 0, true);
                if (sdtContentNode == null)
                    return -2;

                // Update XmlMapping
                if (!sdtNode.XmlMapping.IsEmpty)
                    sdtNode.XmlMapping.SetValues(termLabels);

                // Update run text
                Run run = (Run)sdtContentNode;
                run.Text = ConcatenateTermLabels(termLabels);

                // Update custom properties as well
                doc.CustomDocumentProperties[aliasName].Value = ConcatenateTermValues(termGuids, termLabels);

                return 1;
            }
            return -1;
        }

        private static string ConcatenateTermLabels(IList<string> termLabels)
        {
            string result = "";
            int length = termLabels.Count;
            for (int i = 0; i < length; i++)
            {
                result += termLabels[i];
                if (i != length - 1)
                    result += "; ";
            }
            return result;
        }

        private static string ConcatenateTermValues(IList<string> termGuids, IList<string> termLabels)
        {
            string result = "";
            int length = termGuids.Count;
            for (int i = 0; i < length; i++)
            {
                result += (i + 1) + ";#" + termLabels[i] + "|" + termGuids[i];
                if (i != length - 1)
                    result += ";#";
            }
            return result;
        }


        /// <summary>
        /// Title names for testing sdt nodes in the <see cref="TestSdtFullCollection"/>
        /// </summary>
        private static readonly string[] gAliasesForTestSdtFullCollection = new string[]
            {
                // first page header
                "", // empty alias for docPartObj
                "Account Number",
                "Statement Date",

                // first page content
                "", // empty alias for group obj
                "Client Contact",
                "Client Name",
                "Address 1",
                "AddressLine2",
                "AddressLine3",
                "AddressLine4",
                "AddressLine5",
                "Postcode",
                "Title",
                "Previous Balance",
                "Payments Received",
                "Invoice Number",
                "Invoice Ammount",
                "Invoice Adjustments",
                "DiscountPercentage",
                "DiscountAmount",
                "Outstanding Balance",
                "Payment Due Date",
                "Remittance Account Number",
                "Remittance Invoice Number",
                "Remittance Invoice Date",
                "Remittance Invoice Amount Due ",
                "CHNumber",

                // second page header Sdt
                "", // docpart
                "", // group
                "Invoice Date",
                "Account Number",
                "Invoice Number",
                "", // one more group
                "Customer Name",
                "Address 1",
                "Address 2",
                "Address 3",
                "Address 4",
                "Address 5",
                "Postcode",

                // second page content
                "Trasaction Order Number",
                "Transaction Order Date",
                "Transaction Description",
                "Transaction Customer Reference",
                "Transaction Company Number",
                "Transaction Company Name",
                "Transaction Document Date",
                "Transaction Document Type",
                "Transaction Cost",
                "TotalTransactions"
            };
    }
}
