// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2011 by Denis Darkin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Words.Tests.Import.Docx;
using Aspose.Words.Validation;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.Markup
{
    /// <summary>
    /// Test Sdt behavior related to public API and model.
    /// </summary>
    /// <remarks>
    /// 1. Additional tests related to roundtrip of Sdt are placed in the ImportDocx testing area here <see cref="TestImportDocxSdt"/>
    /// 2. Tests containing UC abbreviation in summary relate to Approved Use-cases defined in Project Wiki at the address below
    /// https://auckland.dynabic.com/wiki/display/orgArchive/SDT+Approved+Use+Cases+for+V1
    /// they are marked as UC1:, UC2:, etc.
    /// </remarks>
    [TestFixture]
    public class TestSdt
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
        /// WORDSNET-6902 StructuredDocumentTag.SdtType issue with RichText
        /// It seems MS Word 2010 omits writing w:richText element inside SdtPr making it silent default.
        /// </summary>
        [Test]
        public void TestJira6902()
        {
            const string testName = @"Model\Markup\TestJira6902.docx";
            Document doc = TestUtil.Open(testName);
            foreach (StructuredDocumentTag sdt in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
                Assert.That(sdt.SdtType, Is.EqualTo(SdtType.RichText));
        }

        /// <summary>
        /// UC1: Create an SDT
        /// </summary>
        [Test]
        public void TestCreateSdt()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Document doc = builder.Document;
            Paragraph para = builder.CurrentParagraph;

            // 1. Ctor.
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Inline);
            para.AppendChild(sdt);

            BuildingBlock block =
                doc.GlossaryDocument.GetBuildingBlock(BuildingBlockGallery.StructuredDocumentTagPlaceholderText,
                                                      SdtPlaceholderManager.BuildingBlockCategory,
                                                      SdtPlaceholderManager.TextPlaceholderName);
            Assert.That(block, IsNot.Null());
            Assert.That(sdt.Placeholder, Is.SameAs(block));
            Assert.That(sdt.PlaceholderName, Is.EqualTo(block.Name));
            Assert.That(sdt.Placeholder.GetText(), Is.EqualTo("Click here to enter text.\f"));

            TestUtil.SaveOpen(doc, @"Model\Markup\TestCreateSdt.docx");
        }

        /// <summary>
        /// WORDSNET-6168 Cannot properly create a Row level StructuredDocumentTag.
        /// Added extra wrapping of para content into row->cell.
        /// </summary>
        [Test]
        public void TestJira6168()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();
            builder.InsertCell();
            builder.InsertCell();
            builder.EndTable();

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Row);
            sdt.Tag = "row";
            sdt.RemoveAllChildren();

            sdt.AppendChild(table.FirstRow);
            table.AppendChild(sdt);

            // verify that generated docx is opened nicely in MS Word.
            TestUtil.SaveOpen(doc, @"Model\Markup\TestJira6168.docx");
        }

        /// <summary>
        /// UC2: Access Common Properties of an SDT
        /// </summary>
        [Test]
        public void TestAccessCommonProperties()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Picture, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            tag.LockContents = true;
            tag.LockContentControl = true;

            // I don't like tag.Tag
            tag.Tag = "My TestSdt tag";
            tag.Title = "A friendly name";
            tag.ContentsFont.Bold = true;
            tag.ContentsFont.Color = Color.Blue;
            tag.EndCharacterFont.Italic = true;
            tag.EndCharacterFont.Color = Color.Red;
            tag.IsShowingPlaceholderText = true;
            tag.IsTemporary = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\TestSetPropertiesTrue.docx");
            tag = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Assert.That(tag.LockContents, Is.True);
            Assert.That(tag.LockContentControl, Is.True);
            Assert.That(tag.Tag, Is.EqualTo("My TestSdt tag"));
            Assert.That(tag.Title, Is.EqualTo("A friendly name"));
            Assert.That(tag.ContentsFont.Bold, Is.True);
            Assert.That(tag.ContentsFont.Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
            Assert.That(tag.EndCharacterFont.Italic, Is.True);
            Assert.That(tag.EndCharacterFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(tag.IsShowingPlaceholderText, Is.True);
            Assert.That(tag.IsTemporary, Is.True);

            tag.IsShowingPlaceholderText = false;
            tag.LockContents = false;
            tag.LockContentControl = false;
            tag.IsTemporary = false;
            tag.Tag = "";
            tag.Title = "";
            TestUtil.SaveOpen(doc, @"Model\Markup\TestSetPropertiesFalse.docx");
        }

        /// <summary>
        /// UC3: RemoveSelfOnly
        /// </summary>
        [Test]
        public void TestRemoveSelfOnly()
        {
            Document doc = new Document();
            doc.FirstSection.Body.AppendChild(new Paragraph(doc));
            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, "First para. First run"));

            doc.FirstSection.Body.AppendChild(new Paragraph(doc));
            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, "Second para. First run."));

            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            doc.FirstSection.Body.AppendChild(new Paragraph(doc));
            const string para4 = "Fourth para. First run.";
            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, para4));

            tag.RemoveSelfOnly();
            Assert.That(doc.FirstSection.Body.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(0));

            TestUtil.SaveOpen(doc, @"Model\Markup\TestRemoveSelfOnly.docx");

            NodeCollection para = doc.FirstSection.Body.GetChildNodes(NodeType.Paragraph, false);
            Assert.That(para.Count, Is.EqualTo(5));
            Assert.That(para[3].GetText(), Is.EqualTo("Click here to enter text.\r"));
            Assert.That(para[4].GetText(), Is.EqualTo(para4 + "\f"));
        }


        /// <summary>
        /// UC4: Access specific properties of Calendar
        /// </summary>
        [Test]
        public void TestCalendarProps()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Date, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            tag.DateDisplayLocale = (int)Language.UzbekLatin;
            tag.DateDisplayFormat = "mm/yy/DD";
            tag.DateStorageFormat = SdtDateStorageFormat.Text;
            tag.CalendarType = SdtCalendarType.Taiwan;
            tag.FullDate = new DateTime(2011, 11, 26);

            Document resaved = TestUtil.SaveOpen(doc, @"Model\Markup\TestCalendarProps.docx");

            tag = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.DateDisplayLocale, Is.EqualTo((int)Language.UzbekLatin));
            Assert.That(tag.DateDisplayFormat, Is.EqualTo("mm/yy/DD"));
            Assert.That(tag.DateStorageFormat, Is.EqualTo(SdtDateStorageFormat.Text));
            Assert.That(tag.CalendarType, Is.EqualTo(SdtCalendarType.Taiwan));
            Assert.That(tag.FullDate, Is.EqualTo(new DateTime(2011, 11, 26)));
        }

        /// <summary>
        /// UC 5: Access specific properties of SdtBuildingBlockGallery (previously SdtDocPartList)
        /// </summary>
        [Test]
        public void TestBuildingBlockGalleryProps()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.BuildingBlockGallery, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            tag.BuildingBlockCategory = "General";
            tag.BuildingBlockGallery = "table of contents";

            Document resaved = TestUtil.SaveOpen(doc, @"Model\Markup\TestBuildingBlockGalleryProps.docx");

            tag = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.BuildingBlockGallery, Is.EqualTo("table of contents"));
            Assert.That(tag.BuildingBlockCategory, Is.EqualTo("General"));
        }

        /// <summary>
        /// UC 6: Access specific properties of ComboBox and DropDownList
        /// </summary>
        [Test]
        public void TestListProps()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.DropDownList, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            SdtListItemCollection listItems = tag.ListItems;
            listItems.Add(new SdtListItem("my disp text 1", "my value 1"));
            listItems.Add(new SdtListItem("my disp text 2"));
            listItems.SelectedValue = listItems[1];

            Document resaved = TestUtil.SaveOpen(doc, @"Model\Markup\TestListProps.docx");

            tag = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.ListItems.Count, Is.EqualTo(2));
            Assert.That(tag.ListItems[0].Value, Is.EqualTo("my value 1"));
            Assert.That(tag.ListItems[1].DisplayText, Is.EqualTo("my disp text 2"));

            tag.ListItems.RemoveAt(0);
            Assert.That(tag.ListItems.SelectedValue.Value, Is.EqualTo("my disp text 2"));

            tag.ListItems.RemoveAt(0);
            Assert.That(tag.ListItems.SelectedValue, Is.Null);
        }

        /// <summary>
        /// WORDSNET-6502 Allow programmatic access to Checked/Unchecked state of SDT Checkbox.
        /// Create Checkbox from scratch and set Checked to True. Save and verify.
        /// </summary>
        [Test]
        public void TestCheckboxChecked()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Checkbox, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            Assert.That(tag.Checked, Is.False);
            tag.Checked = true;
            Assert.That(tag.Checked, Is.True);
            Document resaved = TestUtil.SaveOpen(doc, @"Model\Markup\TestCheckboxChecked.docx", UnifiedScenario.Docx2DocxNoGold);
            tag = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.Checked, Is.True);
        }

        /// <summary>
        /// WORDSNET-6502 Allow programmatic access to Checked/Unchecked state of SDT Checkbox.
        /// Create Checkbox from scratch and save with default state. Verify it is false upon load.
        /// </summary>
        [Test]
        public void TestCheckboxUnchecked()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Checkbox, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            Assert.That(tag.Checked, Is.False);
            Document resaved = TestUtil.SaveOpen(doc, @"Model\Markup\TestCheckboxUnchecked.docx", UnifiedScenario.Docx2DocxNoGold);
            tag = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.Checked, Is.False);
        }




        /// <summary>
        /// WORDSNET-6502 Allow programmatic access to Checked/Unchecked state of SDT Checkbox.
        /// Verify exception.
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCheckboxException()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.DropDownList, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);
            tag.Checked = true;
        }

        /// <summary>
        /// WORDSNET-5779 SDTs combo box, SelectedItem doesn't show up.
        /// Added code into DocumentValidator to update Sdt node content with selected value text and setting IsShowingPlaceholderText to false.
        /// </summary>
        [Test]
        public void TestJira5779()
        {
            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.ComboBox, MarkupLevel.Block);
            SdtListItem sli = new SdtListItem("Item 1", "1");
            sdt.ListItems.Add(sli);
            sdt.ListItems.SelectedValue = sli;
            doc.FirstSection.Body.AppendChild(sdt);
            doc = TestUtil.SaveOpen(doc, @"Model\Markup\TestJira5779.docx");

            StructuredDocumentTag updatedSdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(updatedSdt.IsShowingPlaceholderText, Is.False);
            Assert.That(updatedSdt.GetText(), Is.EqualTo("Item 1\f"));
        }

        /// <summary>
        /// UC 7: Access specific properties of RichText and PlainText
        /// </summary>
        [Test]
        public void TestTextProps()
        {
            Document doc = new Document();
            StructuredDocumentTag plainText = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            StructuredDocumentTag richText = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(plainText);
            doc.FirstSection.Body.AppendChild(richText);

            plainText.Multiline = true;
            richText.Multiline = true;

            Document resaved = TestUtil.SaveOpen(doc, @"Model\Markup\TestTextProps.docx");

            plainText = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 0, true);
            richText = (StructuredDocumentTag)resaved.GetChild(NodeType.StructuredDocumentTag, 1, true);
            Assert.That(plainText.Multiline, Is.True);
            Assert.That(richText.Multiline, Is.True);
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWrongPropertyAccess()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            tag.DateDisplayLocale = 1;
        }

        /// <summary>
        /// Special test that verifies two things:
        /// 1) Id's are preserved where possible. That means if we insert a node into dstDoc and there are no id conflicts inside that dstDoc,
        /// then Id does not change
        /// 2) If there are conflicts: e.g. we clone a node into the same doc, then the id of the cloned sdt is changed to maintain uniqueness.
        /// </summary>
        [Test]
        public void TestIdPersistanceAndUniqueness()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Date, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            // import into other document, id should be retained
            {
                Document dstDoc = new Document();
                StructuredDocumentTag dstTag = (StructuredDocumentTag)dstDoc.ImportNode(tag, true);
                dstDoc.FirstSection.Body.AppendChild(dstTag);
                Assert.That(dstTag.Id, Is.EqualTo(tag.Id));
            }

            // now clone the whole doc, the id's should be retained
            {
                Document dstDoc = (Document)doc.Clone(true);
                NodeCollection sdts = dstDoc.GetChildNodes(NodeType.StructuredDocumentTag, true);
                Assert.That(sdts.Count, Is.EqualTo(1));
                StructuredDocumentTag dstTag = (StructuredDocumentTag)sdts[0];
                Assert.That(dstTag.Id, Is.EqualTo(tag.Id));
            }

            // clone to the same doc: id should change
            {
                StructuredDocumentTag tagClone = (StructuredDocumentTag)tag.Clone(true);
                Assert.That(tagClone.Id, IsNot.EqualTo(tag.Id));
            }

            // now import into the same doc, id should change
            {
                StructuredDocumentTag newTag = (StructuredDocumentTag)doc.ImportNode(tag, true);
                doc.FirstSection.Body.AppendChild(newTag);
                Assert.That(newTag.Id, IsNot.EqualTo(tag.Id));
                newTag.Remove();
            }
        }

        /// <summary>
        /// Verify that id is generated for sdts not having such in source doc.
        /// </summary>
        [Test]
        public void TestEmptyIdRegenerated()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestEmptyId.docx");
            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(1));
            StructuredDocumentTag tag = (StructuredDocumentTag)sdts[0];
            Assert.That(tag.Id, Is.EqualTo(0));
        }

        /// <summary>
        /// Suppose we have an sdt in the loaded document, and we want to create new sdt of the same type.
        /// Verify that this new sdt reuses existing placeholder.
        /// </summary>
        [Test]
        public void TestReusePlaceholderFromLoadedDoc()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdtPlaceholders.docx");

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Inline);
            string testTagName = "DD Test Tag";
            sdt.Title = testTagName;
            doc.FirstSection.Body.LastParagraph.AppendChild(sdt);
            TestUtil.SaveOpen(doc, @"Model\Markup\TestSdtReusePlaceholdersFromLoadedDoc.docx");

            NodeCollection c =  doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            StructuredDocumentTag testTag = ((StructuredDocumentTag)c[c.Count - 1]);
            Assert.That(testTag.Title, Is.EqualTo(testTagName));
            Assert.That(testTag.Placeholder, Is.SameAs(((StructuredDocumentTag)c[0]).Placeholder));
        }

        /// <summary>
        /// We create several sdt nodes and verify that placeholders are reused when applicable.
        /// </summary>
        [Test]
        public void TestReusePlaceholdersFromNewlyCreatedNodes()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Document doc = builder.Document;
            Paragraph para = builder.CurrentParagraph;

            // 1. Ctor.
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.ComboBox, MarkupLevel.Inline);
            para.AppendChild(sdt);

            Table t = builder.StartTable();
            Row r = new Row(doc);
            t.AppendChild(r);

            StructuredDocumentTag cellLevelSdt = new StructuredDocumentTag(doc, SdtType.DropDownList, MarkupLevel.Cell);
            r.AppendChild(cellLevelSdt);

            Assert.That(sdt.PlaceholderName, Is.EqualTo(SdtPlaceholderManager.ListboxPlaceholderName));
            Assert.That(cellLevelSdt.PlaceholderName, Is.EqualTo(sdt.PlaceholderName));
            Assert.That(cellLevelSdt.Placeholder, Is.SameAs(sdt.Placeholder));
            Assert.That(sdt.Placeholder.GetText(), Is.EqualTo("Choose an item.\f"));
        }

        /// <summary>
        /// Verify that placeholders are removed when we delete last sdt that uses them.
        /// </summary>
        [Test]
        public void TestPlaceholderRemoval()
        {
            Document doc = new Document();
            doc.FirstSection.Body.AppendChild(new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block));
            doc.FirstSection.Body.AppendChild(new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block));
            Assert.That(doc.GlossaryDocument.BuildingBlocks.Count, Is.EqualTo(1));

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            while (sdts.Count > 0)
                sdts[0].Remove();

            doc.FirstSection.Body.AppendChild(new StructuredDocumentTag(doc, SdtType.BuildingBlockGallery, MarkupLevel.Block));

            SaveOptions so = new OoxmlSaveOptions();
            WarningInfoCollection warnings = new WarningInfoCollection();
            doc.WarningCallback = warnings;

            Document savedDoc = TestUtil.SaveOpen(doc, @"Model\Markup\SdtPlaceholdersRemoved.docx", so);
            Assert.That(savedDoc.GlossaryDocument.BuildingBlocks.Count, Is.EqualTo(1));
            Assert.That(TestUtil.ContainsWarningBySource(warnings, WarningSource.Validator, WarningStrings.DocumentValidatorSdtPlaceholderUnused), Is.True);
        }

        /// <summary>
        /// Verify that sdts inside glossary work fine.
        /// </summary>
        [Test]
        public void TestSdtInsideGlossary()
        {
            Document doc = new Document();
            doc.GlossaryDocument = new GlossaryDocument();

            // Add sdt as block into glossary
            BuildingBlock block = CreateAndInsertBuildingBlock(doc.GlossaryDocument);
            StructuredDocumentTag tag = new StructuredDocumentTag(doc.GlossaryDocument, SdtType.DropDownList, MarkupLevel.Block);
            block.FirstSection.Body.AppendChild(tag);
            Assert.That(doc.GlossaryDocument.BuildingBlocks.Count, Is.EqualTo(2));

            // now insert this sdt into main doc.
            StructuredDocumentTag mainTag = (StructuredDocumentTag)doc.ImportNode(tag, true);
            doc.FirstSection.Body.AppendChild(mainTag);
            Assert.That(mainTag.PlaceholderName, Is.EqualTo(tag.PlaceholderName));
            Assert.That(mainTag.Placeholder, Is.SameAs(tag.Placeholder));
            Assert.That(mainTag, IsNot.SameAs(tag));

            TestUtil.SaveOpen(doc, @"Model\Markup\SdtInsideGlossary.docx");
        }

        /// <summary>
        /// Verify placeholder is imported during Sdt import.
        /// </summary>
        [Test]
        public void TestPlaceholderImport()
        {
            Document srcDoc = new Document();
            StructuredDocumentTag srcTag = new StructuredDocumentTag(srcDoc, SdtType.RichText, MarkupLevel.Block);
            srcDoc.FirstSection.Body.AppendChild(srcTag);

            // first import sdt into a newly created doc
            Document dstDoc = new Document();
            StructuredDocumentTag dstTag = (StructuredDocumentTag)dstDoc.ImportNode(srcTag, true);
            dstDoc.FirstSection.Body.AppendChild(dstTag);
            CheckImportedPlaceholder(srcTag, dstTag);
            TestUtil.SaveOpen(dstDoc, @"Model\Markup\TestPlaceholderImportIntoDocument.docx");

            // then import sdt into a glossary doc of a newly created doc
            Document dstDoc2 = new Document();
            dstDoc2.GlossaryDocument = new GlossaryDocument();
            StructuredDocumentTag dstTagGlossary = (StructuredDocumentTag)dstDoc2.GlossaryDocument.ImportNode(srcTag, true);
            BuildingBlock blockDoc2 = CreateAndInsertBuildingBlock(dstDoc2.GlossaryDocument);
            blockDoc2.FirstSection.Body.AppendChild(dstTagGlossary);
            CheckImportedPlaceholder(srcTag, dstTagGlossary);
            TestUtil.SaveOpen(dstDoc2, @"Model\Markup\TestPlaceholderImportIntoGlossary.docx");
        }


        /// <summary>
        /// UC: Create custom placeholder and assign it to SDT. Verify that this SDT is roundtripped and
        /// is shown in MS Word ok. Also verify that custom placeholder is not reused for newly created nodes.
        /// </summary>
        [Test]
        public void TestCustomPlaceholderBlock()
        {
            Document srcDoc = CreateDocWithCustomPlaceholder();

            // Now create a new sdt and verify that placeholder is default for this one.
            StructuredDocumentTag newTag = new StructuredDocumentTag(srcDoc, SdtType.PlainText, MarkupLevel.Block);
            srcDoc.FirstSection.Body.AppendChild(newTag);

            // roundtrip document.
            Document doc = TestUtil.SaveOpen(srcDoc, @"Model\Markup\TestCustomPlaceholderCopy.docx");
            Assert.That(((StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true)).PlaceholderName, Is.EqualTo("my fancy block"));

            Assert.That(doc.GlossaryDocument.Count, Is.EqualTo(2));
            Assert.That(((StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 1, true)).Placeholder.GetText(), Is.EqualTo("Click here to enter text.\f"));
        }

        /// <summary>
        /// UC: Create custom placeholder and import it to a doc with another SDT.
        /// Verify that both SDT now contain different placeholders and are roundtripped and shown in MS Word ok.
        /// </summary>
        [Test]
        public void TestCustomPlaceholderImport()
        {
            Document srcDoc = CreateDocWithCustomPlaceholder();
            StructuredDocumentTag tag = (StructuredDocumentTag)srcDoc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Document dstDoc = new Document();
            StructuredDocumentTag anotherTag = new StructuredDocumentTag(dstDoc, SdtType.PlainText, MarkupLevel.Block);
            dstDoc.FirstSection.Body.AppendChild(anotherTag);

            StructuredDocumentTag importedTag = (StructuredDocumentTag)dstDoc.ImportNode(tag, true);
            dstDoc.FirstSection.Body.AppendChild(importedTag);

            Assert.That(dstDoc.FirstSection.Body.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(2));
            Assert.That(dstDoc.GlossaryDocument.Count, Is.EqualTo(2));
            Assert.That(importedTag.Placeholder, IsNot.SameAs(anotherTag.Placeholder));
            Assert.That(importedTag.PlaceholderName, IsNot.EqualTo(anotherTag.PlaceholderName));
            Assert.That(importedTag.Placeholder.GetText(), Is.EqualTo("Enter your name here.\f"));
            Assert.That(importedTag.PlaceholderName, Is.EqualTo("my fancy block"));

            Assert.That(anotherTag.PlaceholderName, Is.EqualTo(SdtPlaceholderManager.TextPlaceholderName));
            Assert.That(anotherTag.Placeholder.GetText(), Is.EqualTo("Click here to enter text.\f"));

            TestUtil.SaveOpen(dstDoc, @"Model\Markup\TestCustomPlaceholderImport.docx");
        }

        /// <summary>
        /// Test Append document with Custom placeholder to an empty doc. Also insert some non-SDT bblock into
        /// glossary and verify that it also is imported into new doc after AppendDocument.
        ///
        /// DD: This test passes, but the MS Word does not show SDT with custom Placeholder in this doc.
        /// Preliminary analysis of BuildingBlock and Sdt tags did not show any major differences,
        /// but some small ones e.g. style names etc.
        /// TODO: Investigate more during SDT API V2 update.
        /// </summary>
        [Test]
        public void TestCustomPlaceholderBlockAppendDoc()
        {
            Document srcDoc = CreateDocWithCustomPlaceholder();
            Document dstDoc = new Document();

            // add arbitrary building block to glossary
            GlossaryDocument glossary = new GlossaryDocument();
            srcDoc.GlossaryDocument = glossary;
            BuildingBlock otherBlock = new BuildingBlock(glossary);
            otherBlock.Category = "Equation";
            otherBlock.AppendChild(new Section(glossary));
            otherBlock.FirstSection.AppendChild(new Body(glossary));
            otherBlock.FirstSection.Body.AppendChild(new Paragraph(glossary));
            otherBlock.LastSection.Body.LastParagraph.AppendChild(new Run(glossary, "This is not placeholder text."));

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            // DD: actually it should be equal to 2 below, but we don't presently import Glossary during AppendDocument
            Assert.That(dstDoc.GlossaryDocument.Count, Is.EqualTo(1));
            Assert.That(((StructuredDocumentTag)dstDoc.GetChild(NodeType.StructuredDocumentTag, 0, true)).PlaceholderName, Is.EqualTo("my fancy block"));

            // SaveOpen this document for SDT API V2 and validate
            // that MS Word shows SDT "Model\Markup\TestCustomPlaceholderAppendDocument.docx".
        }

        /// <summary>
        /// Verify that default content is changed if custom placeholder is set.
        /// </summary>
        [Test]
        public void TestCustomPlaceholderText()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);
            Assert.That(tag.GetText(), Is.EqualTo("Click here to enter text.\f"));

            tag.PlaceholderName = CreateCustomPlaceholder(doc);
            Assert.That(tag.GetText(), Is.EqualTo("Enter your name here.\f"));
        }

        /// <summary>
        /// Verify that:
        /// - placeholder text style is created in glossary doc similar to what MS Word does,
        /// - it is imported into main doc
        /// - it is referenced from both instances of runs in def Sdt content and in placeholder.
        /// </summary>
        [Test]
        public void TestPlaceholderStyle()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(tag);

            Run sdtDefaultRun = (Run)tag.GetChild(NodeType.Run, 0, true);
            VerifyStyleProperties(doc, sdtDefaultRun);

            Run placeholderRun = tag.Placeholder.FirstSection.Body.FirstParagraph.FirstRun;
            VerifyStyleProperties(doc.GlossaryDocument, placeholderRun);
        }

        /// <summary>
        /// A simple test for Reference counter class.
        /// </summary>
        [Test]
        public void TestReferenceCounter()
        {
            ReferenceCounter r = new ReferenceCounter();

            Document doc = new Document();
            Paragraph para1 = new Paragraph(doc);

            r.IncrementReference(para1);
            Assert.That(r.IsReferenced(para1), Is.True);

            r.DecrementReference(para1);
            Assert.That(r.IsReferenced(para1), Is.False);
        }


        /// <summary>
        /// Tests <see cref="CompositeNode.HasNonMarkupDescendants"/> function.
        /// </summary>
        [Test]
        public void TestHasNonMarkupChildNodes()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Document doc = builder.Document;

            Paragraph para = builder.CurrentParagraph;

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            para.AppendChild(sdt);

            Assert.That(para.HasNonMarkupDescendants, Is.False);

            SmartTag tag = new SmartTag(doc);
            sdt.AppendChild(tag);
            Assert.That(para.HasNonMarkupDescendants, Is.False);
            Assert.That(sdt.HasNonMarkupDescendants, Is.False);
            Assert.That(tag.HasNonMarkupDescendants, Is.False);

            Run r = new Run(doc, "simple text");
            tag.AppendChild(r);

            Assert.That(para.HasNonMarkupDescendants, Is.True);
            Assert.That(sdt.HasNonMarkupDescendants, Is.True);
            Assert.That(tag.HasNonMarkupDescendants, Is.True);
        }

        /// <summary>
        /// At least some checks for incorrect sdt insertion in the tree.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot insert a node of this type at this location.")]
        public void TestInsertInlineAtBlockLevel()
        {
            Document doc = new Document();
            doc.FirstSection.Body.AppendChild(new StructuredDocumentTag(doc, MarkupLevel.Inline));
        }

        /// <summary>
        /// At least some checks for incorrect sdt insertion in the tree.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot insert a node of this type at this location.")]
        public void TestInsertCellAtTableLevel()
        {
            Document doc = new Document();
            Table table = new Table(doc);
            doc.FirstSection.Body.AppendChild(table);
            table.AppendChild(new StructuredDocumentTag(doc, MarkupLevel.Cell));
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot insert a node of this type at this location.")]
        public void TestInsertSdtIntoSdtWrongLevel()
        {
            Document doc = new Document();
            StructuredDocumentTag blockSdt = new StructuredDocumentTag(doc, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(blockSdt);

            // This is the wrong level sdt we are trying to insert, should throw.
            blockSdt.AppendChild(new StructuredDocumentTag(doc, MarkupLevel.Inline));
        }

        /// <summary>
        /// Verify that copying an sdt node into another document also copies all formatting attributes correctly.
        /// </summary>
        [Test]
        public void TestImportSdtNodes()
        {
            Document srcDoc = new Document();
            StructuredDocumentTag srcSdt = new StructuredDocumentTag(srcDoc, SdtType.RichText, MarkupLevel.Block);
            srcDoc.Sections[0].Body.AppendChild(srcSdt);

            srcSdt.ContentsRunPr.Istd = srcDoc.Styles.Add(StyleType.Character, "My Contents Style").Istd;
            srcSdt.EndCharacterRunPr.Istd = srcDoc.Styles.Add(StyleType.Character, "My End Character Style").Istd;

            Document dstDoc = new Document();
            dstDoc.RemoveAllChildren();
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            // Verify that the two styles referenced by the sdt node have been copied to the new document correctly.
            Assert.That(srcDoc.Styles.Count, Is.EqualTo(7));
            Assert.That(dstDoc.Styles.Count, Is.EqualTo(7));
            StructuredDocumentTag dstSdt = (StructuredDocumentTag)dstDoc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(dstDoc.Styles.GetByIstd(dstSdt.ContentsRunPr.Istd, false).Name, Is.EqualTo("My Contents Style"));
            Assert.That(dstDoc.Styles.GetByIstd(dstSdt.EndCharacterRunPr.Istd, false).Name, Is.EqualTo("My End Character Style"));
            Assert.That(dstSdt.Placeholder, IsNot.Null());
            Assert.That(dstSdt.Id, Is.EqualTo(srcSdt.Id));
        }

        /// <summary>
        /// Test that cloning of StructuredDocumentTag results in deeply cloned instance.
        /// </summary>
        [Test]
        public void TestSdtClone()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.ComboBox, MarkupLevel.Block);

            SdtListItem item = new SdtListItem("item1display", "item1");
            tag.ListItems.Add(item);

            tag.XmlMapping.SetMapping("testStoreItemId", "testXPath", "testPrefix");
            tag.ListItems.SelectedValue = item;

            StructuredDocumentTag tagClone = (StructuredDocumentTag)tag.Clone(true);
            Assert.That(tagClone, IsNot.SameAs(tag));
            Assert.That(tagClone.ControlProperties, IsNot.SameAs(tag.ControlProperties));
            Assert.That(tagClone.XmlMapping, IsNot.SameAs(tag.XmlMapping));
            Assert.That(tagClone.ListItems[0], IsNot.SameAs(tag.ListItems[0]));
            Assert.That(tagClone.ContentsRunPr, IsNot.SameAs(tag.ContentsRunPr));
            Assert.That(tagClone.EndCharacterRunPr, IsNot.SameAs(tag.EndCharacterRunPr));
            Assert.That(tagClone.Id, IsNot.EqualTo(tag.Id));
            Assert.That(tag.Placeholder, Is.SameAs(tagClone.Placeholder)); // reuse sdt placeholder.
            Assert.That(tagClone.ListItems[0], Is.SameAs(tagClone.ListItems.SelectedValue));
            Assert.That(tagClone.ListItems.SelectedValue, IsNot.SameAs(tag.ListItems.SelectedValue));
        }

        /// <summary>
        /// Test that cloning of StructuredDocumentTag CheckBox results in deeply cloned instance.
        /// </summary>
        [Test]
        public void TestSdtCheckboxClone()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, MarkupLevel.Block);
            SdtCheckBox checkbox = new SdtCheckBox();
            tag.ControlProperties = checkbox;

            StructuredDocumentTag tagClone = (StructuredDocumentTag)tag.Clone(true);
            Assert.That(tagClone, IsNot.SameAs(tag));
            Assert.That(tagClone.ControlProperties, IsNot.SameAs(tag.ControlProperties));
            Assert.That(((SdtCheckBox)tagClone.ControlProperties).CheckedStateInfo, IsNot.SameAs(((SdtCheckBox)tag.ControlProperties).CheckedStateInfo));
            Assert.That(((SdtCheckBox)tagClone.ControlProperties).UncheckedStateInfo, IsNot.SameAs(((SdtCheckBox)tag.ControlProperties).UncheckedStateInfo));
        }

        /// <summary>
        /// WORDSNET-6911 StructuredDocumentTag Dropdown and ComboBox SelectedValue is null.
        /// andrnosk: Fixed by adding mechanism to set SelectedValue by sdtContent.
        /// </summary>
        [Test]
        public void TestJira6911()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira6911.docx");
            foreach (StructuredDocumentTag sdt in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                Assert.That(sdt.ListItems.SelectedValue, IsNot.Null());
            }

            StructuredDocumentTag dropDownList = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(dropDownList.ListItems.SelectedValue.DisplayText, Is.EqualTo("Dit is item 3"));
            Assert.That(dropDownList.ListItems.SelectedValue.Value, Is.EqualTo("id3"));
        }







        /// <summary>
        /// WORDSNET-8546 Date Type StructuredDocumentTag do not render correctly in output Pdf
        /// Updated DateTime formats for parsing time.
        /// </summary>
        [Test]
        public void TestJira8546()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                Document doc = TestUtil.Open(@"Model\Markup\TestJira8546.docx");
                doc.Save(new MemoryStream(), SaveFormat.Docx);   // FOSS: was Doc; DocumentValidator runs on the OOXML save.

                StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

                // Since we output local date as Word does result can be different depending on local timezone.
                int timeOffset = ((DateTime.Now - DateTime.UtcNow).Hours);

                string formattedDateTime = (timeOffset >= 12) ? "Apr 3, 2013" : "Apr 2, 2013";
                Assert.That(sdt.GetText(), Is.EqualTo(formattedDateTime));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }




        /// <summary>
        /// Update nested repeating sections on Row-Level.
        /// </summary>
        [Test]
        public void TestNestedRepeatingSection()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestNestedRepeatingSection", LoadFormat.Docx, SaveFormat.Docx);   // FOSS: WordML save removed
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.Rows.Count, Is.EqualTo(8));

            Paragraph para = table.LastRow.LastCell.Paragraphs[0];
            Run run = para.Runs[0];

            Assert.That(para.Runs.Count, Is.EqualTo(2));
            Assert.That(run.Text, Is.EqualTo("Col3:"));

            Assert.That(para.Runs[1].Text, Is.EqualTo("Child5.Value3"));
        }

        /// <summary>
        /// Tests that a nested repeating section is not moved as a direct child of a parent repeating section on updating
        /// as it happens with a test document of <see cref="TestNestedRepeatingSection"/>, if number of child repeating
        /// section items matches to number of referenced XML nodes (document #1) or if a nested repeating section contains
        /// one repeating section item (document #2).
        /// </summary>
        [TestCase("TestPreservedNestedRepeatingSection1.docx")]
        [TestCase("TestPreservedNestedRepeatingSection2.docx")]
        public void TestPreservedNestedRepeatingSection(string documentName)
        {
            Document doc = TestUtil.Open(@"Model\Markup\" + documentName);

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was Pdf; SDT-content update runs in document validation. // Updates SDT contents

            StructuredDocumentTag repeatingSection =
                (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(repeatingSection.SdtType, Is.EqualTo(SdtType.RepeatingSection));
            Assert.That(repeatingSection.XmlMapping.XPath, Is.EqualTo("/root[1]/Data[1]/Children[1]/Child"));

            Assert.That(repeatingSection.FirstChild.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            StructuredDocumentTag childSdt = (StructuredDocumentTag)repeatingSection.FirstChild;

            // Checks that the first child of the parent repeating section is not a repeating section.
            Assert.That(childSdt.SdtType, IsNot.EqualTo(SdtType.RepeatingSection));
            Assert.That(childSdt.IsRepeatingSectionItem, Is.True);
        }


        // FOSS: TestJira9716 removed — it verified that SDTs are unwrapped when converting DOCX->WordML
        // (validating with WordML2003SaveOptions, then asserting 0 SDTs after a WordML save); WordML is gone.

        /// <summary>
        /// WORDSNET-9999 Custom XML to SDT binding: leading slash required
        /// Word extracts data bound value even if slash missing at 'root' element.
        /// </summary>
        [Test]
        public void TestJira9999()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira9999.docx");

            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo("It's the real text that should display.\f"));
        }


        /// <summary>
        /// WORDSNET-10270 Custom XML to SDT binding issue with date and time format.
        /// The "tt" custom format specifier represents the entire AM/PM designator for DateTime.ToString(),
        /// but sdt's formatting string has "AM/PM", which is passed to this method.
        /// </summary>
        [Test]
        public void TestJira10270()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10270.docx");

            // Let SDT updater work.
            doc.Save(new MemoryStream(), SaveFormat.Docx);   // FOSS: was Doc; DocumentValidator runs on the OOXML save.

            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetCulture("en-US");

                // This date and time string was taken from the problematic sdt.
                string dt = FormatterPal.XmlToDateTimeExact("2013-04-02T06:52:20.9977400-06:00").ToLocalTime().ToString("MMM d, yyyy h:mm tt");
                // Check sdt's text has been calculated properly by SdtContentUpdater().
                Assert.That(doc.FirstSection.Body.FirstParagraph.Runs[1].Text, Is.EqualTo(dt));
                Assert.That(doc.FirstSection.Body.LastParagraph.Runs[1].Text, Is.EqualTo(dt));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// WORDSNET-10091 Custom XML to SDT binding: images inside XML are not working
        /// Basic support for SdtType.Picture data bound update.
        /// </summary>
        [Test]
        public void TestJira10091()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10091.docx");
            doc.Save(new MemoryStream(), SaveFormat.Docx);   // FOSS: was Doc; DocumentValidator runs on the OOXML save.

            // Verify that image data was updated.
            Shape shape = (Shape)doc.FirstSection.Body.GetChild(NodeType.Shape, 0, true);

            string md5 = ArrayUtil.DumpArray(HashUtil.ComputeHash(DigestAlgorithm.MD5, shape.ImageData.ImageBytes)).Trim();
            Assert.That(md5, Is.EqualTo("F8 DB 2A 7F 7F 46 70 2C AE 01 0F 26 A1 95 3D 86"));
        }

        /// <summary>
        /// WORDSNET-10445 Date formatting issue with Custom XML to SDT binding
        /// </summary>
        [Test]
        public void TestDefect10445()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                Document doc = TestUtil.Open(@"Model\Markup\TestDefect10445.docx");

                // Let SDT updater work.
                doc.Save(new MemoryStream(), SaveFormat.Docx);   // FOSS: was Pdf

                Assert.That(doc.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false)[5].GetText(), Is.EqualTo("Jul 4, 1970"));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }


        /// <summary>
        /// WORDSNET-10935 Document.UpdatePageLayout throws System.FormatException.
        /// String with image bytes contains illegal symbols in Base64.
        /// Additionally, decoded image bytes does not represents a valid image and shouldn't be updated.
        /// </summary>
        [Test]
        public void TestJira10935()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira10935.docx");
            // No update page layout in FOSS.

            Shape dml = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.That(dml.ImageData.ImageBytes.Length, Is.EqualTo(12302));
        }






        /// <summary>
        /// WORDSNET-11759 System.NullReferenceException is thrown while creating instance of StructuredDocumentTag
        ///
        /// Check situation when Placeholder contains paragraph but doesn't contain run with content.
        /// </summary>
        [Test]
        public void TestJira11759()
        {
            Document document = TestUtil.Open(@"Model\Markup\TestJira11759.dotm");
            StructuredDocumentTag sdt = new StructuredDocumentTag(document, SdtType.PlainText, MarkupLevel.Inline);
            Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-12052 SDT data is lost during saving to PDF.
        /// Should not replace sdt content if it's property 'IsShowingPlaceholderText' is true.
        /// </summary>
        [Test]
        public void TestJira12052()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira12052.docx");

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was a raw validator run with null SaveOptions (now asserts).

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            StructuredDocumentTag sdt = (StructuredDocumentTag) sdts[0];

            // Preserve IsShowingPlaceholderText.
            Assert.That(sdt.IsShowingPlaceholderText, Is.True);
            Assert.That(sdt.GetText(), Is.EqualTo("Placeholder text."));

            sdt = (StructuredDocumentTag)sdts[1];
            Assert.That(sdt.IsShowingPlaceholderText, Is.True);
            Assert.That(sdt.GetText(), Is.EqualTo("[Type the document title]"));
        }




        /// <summary>
        /// Currently there is no way to create RepeatingSection using public API.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void DoNotAllowCreateRepeatingSection()
        {
            StructuredDocumentTag structuredDocumentTag = new StructuredDocumentTag(
                new Document(),
                SdtType.RepeatingSection,
                MarkupLevel.Inline );
        }


        /// <summary>
        /// WORDSNET-12528 Text formatting is not correct inside SDT after using DocumentBuilder.InsertDocument.
        /// Looks like during inserting the content into inline SDT we have to use the style from the source document no matter
        /// UseDestinationStyles import format mode is set, mimic MS Word behavior.
        /// </summary>
        [Test]
        public void TestJira12528()
        {
            Document dstDoc = TestUtil.Open(@"Model\Markup\TestJira12528A.docx");
            Document srcDoc = TestUtil.Open(@"Model\Markup\TestJira12528B.docx");

            DocumentBuilder builder = new DocumentBuilder(dstDoc);

            foreach (StructuredDocumentTag sdt in dstDoc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                if (sdt.Tag == "sdtBlock")
                {
                    sdt.RemoveAllChildren();
                    Paragraph para = new Paragraph(dstDoc);
                    sdt.AppendChild(para);
                    builder.MoveTo(para);

                    builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles);
                }

                if (sdt.Tag == "sdtInline")
                {
                    sdt.RemoveAllChildren();
                    Run r = new Run(dstDoc);
                    sdt.AppendChild(r);
                    builder.MoveTo(r);
                    builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles);
                }
            }

            // It is better to use gold comparing here, because there are really strange
            // rules how MS Word process the content inside SDT.
            dstDoc = TestUtil.SaveOpen(dstDoc, @"Model\Markup\TestJira12528.docx");

            Node[] sdtNodes = dstDoc.GetChildNodes(NodeType.StructuredDocumentTag, true).ToArray();
            StructuredDocumentTag sdt2 = (StructuredDocumentTag)sdtNodes[2];
            Node[] sdtRuns = sdt2.GetChildNodes(NodeType.Run, true).ToArray();

            Assert.That(((Run)sdtRuns[1]).RunPr.Bold, Is.EqualTo(AttrBoolEx.True));

            // The style is applied. So we have to expand and check attributes.
            RunPr runPr = ((Run)sdtRuns[3]).GetExpandedRunPr(RunPrExpandFlags.Normal);
            Assert.That(runPr.Istd, IsNot.Null());

            // The size specified in the style equals 13 (26).
            Assert.That(runPr.Size, Is.EqualTo(26));
            Assert.That(runPr.Color.ToArgb(), Is.EqualTo(-13732683));

            Assert.That(((Run)sdtRuns[5]).RunPr.Underline, Is.EqualTo(Underline.Single));
        }


        /// <summary>
        /// Tests reading/writing the 'color' element of a structured document tag.
        /// </summary>
        [Test]
        public void TestReadingWritingColor()
        {
            const string docFileName = @"Model\Markup\TestSdtColor.docx";
            Document doc = TestUtil.Open(docFileName);

            NodeCollection nodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(nodes.Count, Is.EqualTo(3));
            StructuredDocumentTag sdt1 = (StructuredDocumentTag)nodes[0];
            CheckColor(sdt1, "993300", null, null, null);
            CheckColor((StructuredDocumentTag)nodes[1], "385623", "accent6", "80", null);
            CheckColor((StructuredDocumentTag)nodes[2], "9CC2E5", "accent1", null, "99");

            sdt1.BaseColor = DrColor.FromArgb(32, 64, 128);
            doc = TestUtil.SaveOpen(doc, docFileName, null, false);

            nodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(nodes.Count, Is.EqualTo(3));
            CheckColor((StructuredDocumentTag)nodes[0], "204080", null, null, null);
            CheckColor((StructuredDocumentTag)nodes[1], "385623", "accent6", "80", null);
            CheckColor((StructuredDocumentTag)nodes[2], "9CC2E5", "accent1", null, "99");
        }

        /// <summary>
        /// WORDSNET-14104 Tests reading/writing the 2.5.1.1 appearance, 2.5.1.12 webExtensionCreated,
        /// 2.5.1.13 webExtensionLinked, 2.6.1.15 entityPicker [MS-DOCX] elements of a structured document tag.
        /// </summary>
        [Test]
        public void TestJira14104()
        {
            // The extensions except 'appearance' are created manually in the source document.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\TestSdtExtensions", UnifiedScenario.Docx2DocxNoGold);

            NodeCollection nodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            Assert.That(((StructuredDocumentTag)nodes[0]).Appearance, Is.EqualTo(SdtAppearance.BoundingBox));
            Assert.That(((StructuredDocumentTag)nodes[1]).Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(((StructuredDocumentTag)nodes[2]).Appearance, Is.EqualTo(SdtAppearance.Hidden));

            Assert.That(((StructuredDocumentTag)nodes[3]).SdtType, Is.EqualTo(SdtType.EntityPicker));

            Assert.That(((StructuredDocumentTag)nodes[3]).WebExtensionRelationship, Is.EqualTo(SdtWebExtensionRelationship.None));
            Assert.That(((StructuredDocumentTag)nodes[4]).WebExtensionRelationship, Is.EqualTo(SdtWebExtensionRelationship.CreatedByWebExtension));
            Assert.That(((StructuredDocumentTag)nodes[5]).WebExtensionRelationship, Is.EqualTo(SdtWebExtensionRelationship.LinkedToWebExtension));

            // FOSS: the SDT-appearance / entity-picker / web-extension data-loss warnings were verified
            // by validating to the removed .doc format; that warning check is dropped. The SDT property
            // reads above still exercise the model.
        }



        /// <summary>
        /// WORDSNET-13458 Docx to Doc/Pdf conversion issue with bounded check-boxes
        /// Added support for data bound checkbox update.
        /// </summary>
        [Test]
        public void TestJira13458()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira13458.docx");

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts[0].GetText(), Is.EqualTo("\x2612"));
            Assert.That(sdts[1].GetText(), Is.EqualTo("\x2610"));
            Assert.That(sdts[2].GetText(), Is.EqualTo("\x2612"));
            Assert.That(sdts[3].GetText(), Is.EqualTo("\x2610"));
        }

        /// <summary>
        /// WORDSNET-13897 State of checkbox SDT is lost after conversion from Docx to Doc/Fixed file format
        /// Bound lowercase value 'true' also indicates that checkbox should be checked.
        /// </summary>
        [Test]
        public void TestJira13897()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira13897.docx");

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was Pdf; SDT-content update runs in document validation.

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts[0].GetText(), Is.EqualTo("\x2612"));
        }

        /// <summary>
        /// WORDSNET-13622 StructuredDocumentTag of type DROP_DOWN_LIST is not
        /// retaining its value when saving as PDF format.
        /// 'IsUpdateableType' must be 'true' for this sdt type.
        /// </summary>
        [Test]
        public void TestJira13622()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira13622.docx");
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was Pdf; SDT-content update runs in document validation.

            Table tbl = doc.FirstSection.Body.Tables[0];

            Cell cell = tbl.GetCell(1, 2);
            Assert.That(cell.FirstParagraph.FirstRun.Text, Is.EqualTo("False"));

            cell = tbl.GetCell(5, 2);
            Assert.That(cell.FirstParagraph.FirstRun.Text, Is.EqualTo("Masda"));
        }



        /// <summary>
        /// WORDSNET-14295 An image was lost after resaving the document. A 'pict' element of the image is
        /// direct child of a 'sdt' element that is not allowed by ISO 29500 but MS Word reads the document well.
        /// </summary>
        [Test]
        public void TestJira14295()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira14295.docx");
            Assert.That(doc.FirstSection.HeadersFooters[HeaderFooterType.FooterFirst].Shapes.Count, Is.EqualTo(1));
        }








        /// <summary>
        /// This test is the part of the WORDSNET-15466
        /// Checks that SDT without bound data does not update when it is placed into <see cref="GlossaryDocument"/>.
        /// </summary>
        [Test]
        public void TestJira15466NoBoundData()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira15466NoBoundData.docx");
            StructuredDocumentTag sdt = (StructuredDocumentTag)
                doc.GlossaryDocument.FirstBuildingBlock.FirstSection.Body.FirstChild;

            // Check the result of the updating and content of the SDT was not updated.
            sdt.XmlMapping.UpdateContent();
            Assert.That(((Paragraph)sdt.FirstChild).HasChildNodes, Is.False);

            // Check that document can be validate without errors.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was Pdf; SDT-content update runs in document validation.
        }



        /// <summary>
        /// Tests that a structured document tag is updated correctly when mapped to an extended property.
        /// </summary>
        [Test]
        public void TestUpdatingFromExtendedProperty()
        {
            Document doc = new Document();

            doc.BuiltInDocumentProperties[PropertyName.Company].Value = "Test Company";

            // Insert a SDT and map to the extended property.
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(sdt);
            sdt.XmlMapping.SetMapping("{6668398D-A668-4E3E-A5EB-62B293D839F1}", "/ns0:Properties[1]/ns0:Company[1]",
                "xmlns:ns0='http://schemas.openxmlformats.org/officeDocument/2006/extended-properties'");

            Assert.That(sdt.XmlMapping.UpdateContent(), Is.True);
            Assert.That(sdt.GetText().Trim(), Is.EqualTo("Test Company"));
        }

        /// <summary>
        /// WORDSNET-15651 StructuredDocumentTag.FullDate is not updating.
        /// When SDT has DataBinding and user changes its value through setter,
        /// we should update corresponding XmlNode value along with updating its content.
        /// </summary>
        [Test]
        public void TestJira15651()
        {
            const string testFile = @"Model\Markup\TestJira15651";
            Document doc = TestUtil.Open(testFile, LoadFormat.Docx);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[1];

            DateTime date = new DateTime(2017, 08, 09);
            sdt.FullDate = date;

            doc = TestUtil.SaveOpen(doc, testFile, UnifiedScenario.Docx2DocxNoGold);

            sdt = (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[1];
            Assert.That(sdt.FullDate, Is.EqualTo(date));

            string datetText = date.ToString(sdt.DateDisplayFormat);
            Assert.That(((Paragraph)sdt.FirstChild).FirstRun.Text, Is.EqualTo(datetText));
            Assert.That(sdt.XmlMapping.GetValue(), Is.EqualTo(datetText));
        }

        /// <summary>
        /// WORDSNET-15953 System.InvalidOperationException throws StructuredDocumentTag.PlaceholderName.
        /// StructuredDocumentTag can't found BuildingBlock by new name when it changes placeholder name. Added resilience.
        /// </summary>
        [Test]
        public void TestJira15953()
        {
            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(sdt);

            GlossaryDocument glossaryDocument = doc.GlossaryDocument;
            const string name = "testPlaceholder";

            BuildingBlock placeholder = new BuildingBlock(glossaryDocument);
            placeholder.Behavior = BuildingBlockBehavior.Content;
            placeholder.Type = BuildingBlockType.StructuredDocumentTagPlaceholderText;
            placeholder.Name = name;
            placeholder.Guid = Guid.NewGuid();
            placeholder.Category = "NetBrain";
            placeholder.Gallery = BuildingBlockGallery.CustomQuickParts;
            Section section = placeholder.FirstSection;
            if (section == null)
            {
                section = new Section(glossaryDocument);
                placeholder.Sections.Add(section);
            }

            Body body = section.Body;
            if (body == null)
            {
                body = new Body(glossaryDocument);
                section.AppendChild(body);
            }

            Paragraph para = body.FirstParagraph;
            if (para == null)
            {
                para = new Paragraph(glossaryDocument);
                body.AppendChild(para);
            }

            Run run = para.FirstChild as Run;
            if (run == null)
            {
                run = new Run(glossaryDocument);
                para.AppendChild(run);
            }
            run.Text = "[Test PlaceHolder]";

            glossaryDocument.BuildingBlocks.Add(placeholder);

            sdt.PlaceholderName = placeholder.Name;

            Assert.That(sdt.PlaceholderName, Is.EqualTo(name));

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\TestJira15953", UnifiedScenario.Docx2DocxNoGold);
            sdt = (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];
            Assert.That(sdt.PlaceholderName, Is.EqualTo(name));
            Assert.That(sdt.Placeholder.Name, Is.EqualTo(name));
        }



        /// <summary>
        /// Relates to WORDSNET-16155
        /// Document contains nested repeating section SDT with invalid XPath.
        /// </summary>
        [Test]
        public void TestJira16155Nested()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira16155_Nested.docx");
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was Pdf; SDT-content update runs in document validation.

            NodeCollection sdts = doc.FirstSection.Body.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(7));
            Assert.That(sdts[0].GetText().StartsWith("Repeating:"), Is.True);
            Assert.That(sdts[2].GetText().StartsWith("Repeating2"), Is.True);
        }


        /// <summary>
        /// WORDSNET-16532 Allow accessing Color property of StructuredDocumentTag objects.
        /// Added public property to get and set Color.
        /// </summary>
        [Test]
        public void TestJira16532()
        {
            const string fileName = @"Model\Markup\TestJira16532";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];

            // DrColor.Empty is used, when no color specified, or <w15:color w:val="auto" /> is specified.
            Assert.That(sdt.Color, Is.EqualTo((DrColor.Empty).ToNativeColor()));

            sdt.Color = Color.Red;

            TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);

            sdt = (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];

            Assert.That(sdt.Color, Is.EqualTo(Color.FromArgb(255, 255, 0, 0)));
        }


        /// <summary>
        /// WORDSNET-14601 Provide option to Use a style to format text typed into the SDT control.
        /// Added a public property to get and set the SDT style.
        /// </summary>
        [TestCase("Quote")]
        [TestCase("Strong")]
        public void TestJira14601(string styleName)
        {
            const string fileName = @"Model\Markup\TestJira14601";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);

            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];

            Assert.That("Default Paragraph Font", Is.EqualTo(sdt.Style.Name));

            sdt.Style = doc.Styles[styleName];
            Assert.That(styleName, Is.EqualTo(sdt.Style.Name));
            Assert.That(styleName, Is.EqualTo(sdt.StyleName));

            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions(SaveFormat.Docx);
            TestUtil.Save(doc, String.Format("{0}{1}.docx", fileName, styleName), saveOptions, true);
        }

        /// <summary>
        /// Relates to WORDSNET-14601
        /// Tests throwing exception when setting null as an SDT style.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestJira14601SetNull()
        {
            Document doc = new Document();

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.Date, MarkupLevel.Cell);
            sdt.Style = null;
        }

        /// <summary>
        /// Relates to WORDSNET-14601
        /// Tests throwing exception when setting not character and not linked style.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira14601SetNormal()
        {
            Document doc = new Document();

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.Date, MarkupLevel.Cell);
            sdt.Style = doc.Styles["Normal"];
        }

        /// <summary>
        /// Relates to WORDSNET-14601
        /// Tests throwing exception when setting not character and not linked style accesed by StyleName.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira14601SetNormalByStyleName()
        {
            Document doc = new Document();

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.Date, MarkupLevel.Cell);
            sdt.StyleName = "Normal";
        }

        /// <summary>
        /// Relates to WORDSNET-14601
        /// Tests getting the default style when no style is set.
        /// </summary>
        [Test]
        public void TestJira14601GetDefaultStyle()
        {
            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.Date, MarkupLevel.Cell);

            Assert.That(sdt.Style.Name, Is.EqualTo("Default Paragraph Font"));
        }

        /// <summary>
        /// Relates to WORDSNET-14601
        /// Setting the default style.
        /// </summary>
        [Test]
        public void TestJira14601ClearingStyle()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira14601", LoadFormat.Docx);

            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];

            sdt.StyleName = "Quote";
            Assert.That(sdt.ContentsRunPr[FontAttr.Istd], Is.EqualTo(doc.Styles["Quote Char"].Istd));

            sdt.StyleName = "Default Paragraph Font";
            Assert.That(sdt.ContentsRunPr[FontAttr.Istd], Is.EqualTo(StyleIndex.Nil));
        }

        /// <summary>
        /// Relates to WORDSNET-14601
        /// Setting linked char style.
        /// </summary>
        [Test]
        public void TestJira14601SetLinkedChar()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira14601", LoadFormat.Docx);

            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];

            sdt.StyleName = "Heading 1 Char";
            Assert.That(sdt.StyleName, Is.EqualTo("Heading 1"));
        }


        /// <summary>
        /// WORDSNET-17905 If an SDT is mapped to custom XML and a mapped XML node contains a FlatOpc
        /// document with SDTs inside, the SDTs were not inserted on updating the SDT of the main document, but their
        /// non-updated contents only. MS Word inserts SDTs into the updating SDT and if necessary moves the SDT to
        /// block level. The same has been implemented now.
        /// </summary>
        [TestCase("TestJira17905_InlineSdt.docx",
            "Updating SDT: Inserting SDT: XML node value\f", MarkupLevel.Inline, 1)]
        [TestCase("TestJira17905_BlockSdtAndParaToStartOfPara.docx",
            "XML node value\r- Inserting SDT\r- Updating SDT\f", MarkupLevel.Block, 1)]
        [TestCase("TestJira17905_ParaAndBlockSdtToMiddleOfPara.docx",
            "Updating SDT: \rInserting SDT: \rXML node value\r- Updating SDT\f", MarkupLevel.Block, 1)]
        [TestCase("TestJira17905_ParaAndBlockSdtToEndOfPara.docx",
            "Updating SDT: \rInserting SDT: \rXML node value\f", MarkupLevel.Block, 1)]
        [TestCase("TestJira17905_ParaAndBlockSdtWithReplacingPara.docx",
            "Inserting SDT: \rXML node value\f", MarkupLevel.Block, 1)]
        [TestCase("TestJira17905_BlockSdt.docx",
            "Updating SDT: XML node value\f", MarkupLevel.Inline, 1)]
        [TestCase("TestJira17905_TwoSections.docx",
            "Updating SDT: \rSection 1\rSection 2\r\f", MarkupLevel.Block, 0)] // See TestWordInconsistency() for this case.
        public void TestJira17905(string fileName, string expectedText, MarkupLevel expectedLevel, int expectedChildSdts)
        {
            Document doc = TestUtil.Open(@"Model\Markup\" + fileName);
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was Pdf; SDT-content update runs in document validation.

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.Level, Is.EqualTo(expectedLevel));
            Assert.That(sdt.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(expectedChildSdts));
            Assert.That(doc.GetText(), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Test MS Word inconsistency related to XML Mapping update.
        /// Tested on Microsoft® Word 2019 MSO (Version 2309 Build 16.0.16827.20166) 32-bit
        /// This is slightly complex case:
        /// Original file saved in Word will have three paragraphs.
        /// It can be saved again in Word and document layout will be preserved, still three paragraphs.
        /// But if we delete mapped SDT checksum in file produced by Word then extra paragraph will appear.
        ///
        /// This means that first SDT update from mapped XML was locked by checksum and if we remove checksum Word will update
        /// SDT mapped XML one more time and produces another result (extra paragraph).
        /// </summary>
        [Test]
        public void TestWordInconsistency()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestWordInconsistency.docx");

            // Original TestWordInconsistency.docx was saved in MS Word.
            Document docMs = TestUtil.Open(@"Model\Markup\TestWordInconsistency.docx ms.docx");

            // Original TestWordInconsistency.docx was saved in MS Word and storeItemChecksum element was manually removed.
            Document docMsNoChecksum = TestUtil.Open(@"Model\Markup\TestWordInconsistency.docx ms NoChecksum.docx");

            // Three paragraphs.
            Assert.That(doc.GetText(), Is.EqualTo("Updating SDT: \rSection 1\rSection 2\f"));

            // Still three paragraphs because mapping update is locked by checksum.
            Assert.That(docMs.GetText(), Is.EqualTo("Updating SDT: \rSection 1\rSection 2\f"));

            // Extra paragraph is appeared.
            Assert.That(docMsNoChecksum.GetText(), Is.EqualTo("Updating SDT: \rSection 1\rSection 2\r\f"));
        }

        /// <summary>
        /// WORDSNET-15602 StructuredDocumentTag.ContentsFont does not apply RunPr for SDT Content.
        /// Fixed by simultaneously applying attributes for both RunPrs - for SDT and for SDT Content.
        /// </summary>
        [Test]
        public void TestJira15602()
        {
            Document doc = new Document();

            StructuredDocumentTag checkbox = new StructuredDocumentTag(doc, SdtType.Checkbox, MarkupLevel.Inline);
            Run checkboxRun = (Run)checkbox.GetChild(NodeType.Run, 0, true);

            checkbox.ContentsFont.Bold = true;
            Assert.That(checkboxRun.Font.Bold, Is.EqualTo(true));

            checkbox.Checked = true;
            checkbox.ContentsFont.Name = "Arial";
            string desiredFontName = ((SdtCheckBox)checkbox.ControlProperties).CheckedStateInfo.FontName;
            Assert.That(checkboxRun.Font.Name, IsNot.EqualTo("Arial"));
            Assert.That(checkboxRun.Font.Name, Is.EqualTo(desiredFontName));
        }

        /// <summary>
        /// WORDSNET-18112 AW does not update SDT bound content.
        /// Fixed by updating SDT bound content on document load.
        /// </summary>
        [Test]
        public void TestJira18112()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira18112.docx");
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Node sdtContentNode = sdt.GetChild(NodeType.Run, 0, true);

            Assert.That(((Run)sdtContentNode).Text, Is.EqualTo("10"));
        }

        /// <summary>
        /// WORDSNET-18729 Document get corrupted after saving DOCX file with enable track changes and user comments.
        /// Document contains SDT which end with section break.
        /// </summary>
        [Test]
        public void Test18729()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test18729", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Sections.Count, Is.EqualTo(3));

            // SDT is a ranged STD.
            Assert.That(doc.FirstSection.Body.Count, Is.EqualTo(3));

            StructuredDocumentTagRangeStart sdtRangeStart =
                (StructuredDocumentTagRangeStart)doc.FirstSection.Body.FirstChild;
            Assert.That(sdtRangeStart.Tag, Is.EqualTo("IN:Schema"));
            Assert.That(sdtRangeStart.ChildNodes.Count, Is.EqualTo(3));

            // Tested on Microsoft® Word 2019 MSO (Version 2309 Build 16.0.16827.20166) 32-bit.
            // Now we have two paragraphs in second section.
            Assert.That(doc.Sections[1].GetText(), Is.EqualTo("\r\f"));
        }

        /// <summary>
        /// Related to WORDSNET-18729
        /// Word does not move SDT to the block level when mapped document contains only two paragraphs
        /// and last paragraph is empty.
        /// </summary>
        [Test]
        public void Test18729InlineSDT()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test18729InlineSDT.docx");

            // Check model structure.
            Body body = doc.FirstSection.Body;
            Assert.That(body.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            Assert.That(body.LastParagraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
            Assert.That(body.FirstParagraph.FirstRun.Text, Is.EqualTo(", "));

            // AM. Seems MS Word behavior has changed, now SDT is moved out of paragraph in case of updating from
            // multiline document.
            StructuredDocumentTag sdt = (StructuredDocumentTag)body.FirstParagraph.NextSibling;
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Block));

            Run innerRun = ((Paragraph)sdt.FirstChild).FirstRun;
            Assert.That(innerRun.Text, Is.EqualTo("=lorem(2000)"));
            Assert.That(innerRun.RunPr.HasInsertRevision, Is.True);

            Assert.That(sdt.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
        }

        /// <summary>
        /// WORDSNET-12655 Tests ability to create a repeating section and a repeating section item.
        /// </summary>
        [Test]
        public void Test12655()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            CustomXmlPart xmlPart = doc.CustomXmlParts.Add("1234",
                "<books><book><title>Everyday Italian</title><author>Giada De Laurentiis</author></book>" +
                "<book><title>Harry Potter</title><author>J K. Rowling</author></book>" +
                "<book><title>Learning XML</title><author>Erik T. Ray</author></book></books>");

            Table table = builder.StartTable();

            builder.InsertCell();
            builder.Write("Title");

            builder.InsertCell();
            builder.Write("Author");

            builder.EndRow();
            builder.EndTable();

            StructuredDocumentTag repeatingSectionSdt =
                new StructuredDocumentTag(doc, SdtType.RepeatingSection, MarkupLevel.Row);
            repeatingSectionSdt.XmlMapping.SetMapping(xmlPart, "/books[1]/book", "");
            table.AppendChild(repeatingSectionSdt);

            StructuredDocumentTag repeatingSectionItemSdt =
                new StructuredDocumentTag(doc, SdtType.RepeatingSectionItem, MarkupLevel.Row);
            repeatingSectionSdt.AppendChild(repeatingSectionItemSdt);

            Row row = new Row(doc, table.FirstRow.TablePr.Clone());
            repeatingSectionItemSdt.AppendChild(row);

            // SDT placeholder contains a cell.
            StructuredDocumentTag titleSdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Cell);
            titleSdt.XmlMapping.SetMapping(xmlPart, "/books[1]/book[1]/title[1]", "");
            row.AppendChild(titleSdt);

            StructuredDocumentTag authorSdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Cell);
            authorSdt.XmlMapping.SetMapping(xmlPart, "/books[1]/book[1]/author[1]", "");
            row.AppendChild(authorSdt);

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test12655", UnifiedScenario.Docx2Docx | UnifiedScenario.ExportOnly);

            StructuredDocumentTag repeatingSectionItem =
                (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 1, true);
            Assert.That(repeatingSectionItem.SdtType, Is.EqualTo(SdtType.RepeatingSectionItem));
        }

        /// <summary>
        /// WORDSNET-12655 Tests that exception is thrown on creation an inline repeating section.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInlineRepeatingSection()
        {
            Document doc = new Document();
            new StructuredDocumentTag(doc, SdtType.RepeatingSection, MarkupLevel.Inline);
        }

        /// <summary>
        /// WORDSNET-12655 Tests that exception is thrown on creation an inline repeating section item.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInlineRepeatingSectionItem()
        {
            Document doc = new Document();
            new StructuredDocumentTag(doc, SdtType.RepeatingSectionItem, MarkupLevel.Inline);
        }





        /// <summary>
        /// WORDSNET-20257 Aspose.Words does not import StructuredDocumentTag containing section break
        /// Partially reverted WORDSNET-18729
        /// </summary>
        [Test]
        public void Test20257()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test20257.docx");
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(4));

            // Check problematic SDT.
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.GetChildNodes(NodeType.Any, false)[3];
            Assert.That(sdt.Tag, Is.EqualTo("AAAA"));

            // Check ranged SDT.
            StructuredDocumentTagRangeStart rangeStart = (StructuredDocumentTagRangeStart)doc.Sections[3].Body.GetChildNodes(NodeType.Any, false)[1];
            StructuredDocumentTagRangeEnd rangeEnd = (StructuredDocumentTagRangeEnd)doc.Sections[4].Body.GetChildNodes(NodeType.Any, false)[1];

            Assert.That(ReferenceEquals(rangeStart.RangeEnd, rangeEnd), Is.True);
            Assert.That(rangeStart.Id, Is.EqualTo(0x51dd23bc));
            Assert.That(rangeEnd.Id, Is.EqualTo(0x51dd23bc));

            Assert.That(rangeStart.SdtType, Is.EqualTo(SdtType.RichText));
            Assert.That(rangeStart.Tag, Is.EqualTo("BBBB"));
            Assert.That(rangeStart.Title, Is.EqualTo("BBBB"));
        }


        /// <summary>
        /// Tests how range nodes are cloned when whole document cloned.
        /// </summary>
        [Test]
        public void TestSdtRangeClone()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test13519.docx");
            Document clonedDoc = (Document)doc.Clone(true);

            StructuredDocumentTagRangeStart rangeStart =
                (StructuredDocumentTagRangeStart)doc.FirstSection.Body.FirstChild;

            StructuredDocumentTagRangeStart clonedRangeStart =
                (StructuredDocumentTagRangeStart)clonedDoc.FirstSection.Body.FirstChild;

            // All range parts are cloned.
            Assert.That(ReferenceEquals(rangeStart, clonedRangeStart), Is.False);
            Assert.That(ReferenceEquals(rangeStart.InternalSdt, clonedRangeStart.InternalSdt), Is.False);
            Assert.That(ReferenceEquals(rangeStart.InternalSdt.Document, clonedRangeStart.InternalSdt.Document), Is.False);
        }

        /// <summary>
        /// Demonstrates that ranged structured document tag will created only when it is necessary.
        /// </summary>
        [Test]
        public void TestRangedNecessity()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdtRange2.docx");
            StructuredDocumentTagRangeStart sdtRangeStart =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);

            // Ranged SDT was loaded.
            Assert.That(sdtRangeStart.RangeEnd, IsNot.Null());
            Assert.That(sdtRangeStart.Tag, Is.EqualTo("ExhibitB"));

            // Move range end node to first section to remove range necessity.
            sdtRangeStart.ParentNode.AppendChild(sdtRangeStart.RangeEnd);

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\TestSdtRange2", UnifiedScenario.Docx2DocxNoGold);

            // Now we have no ranged SDT.
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTagRangeStart, true).Count, Is.EqualTo(0));

            // And have normal SDT instead.
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Demonstrates we can insert section breaks into normal SDT.
        /// </summary>
        [Test]
        public void TestSdtSectionImport()
        {
            Document dstDoc = TestUtil.Open(@"Model\Markup\TestSdtRange4 Dst.docx");
            Document srcDoc = TestUtil.Open(@"Model\Markup\TestSdtRange4 Src.docx");
            DocumentBuilder builder = new DocumentBuilder(dstDoc);

            StructuredDocumentTag sdt = (StructuredDocumentTag)dstDoc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            builder.MoveTo(sdt.FirstChild);
            builder.InsertDocument(srcDoc, ImportFormatMode.KeepDifferentStyles);

            // Check original SDT has been split and removed.
            Assert.That(sdt.IsRemoved, Is.True);
            StructuredDocumentTagRangeStart sdtStart =
                (StructuredDocumentTagRangeStart)dstDoc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTagRangeStart, 0,
                    false);
            Assert.That(sdtStart, IsNot.Null());
            StructuredDocumentTagRangeEnd sdtEnd =
                (StructuredDocumentTagRangeEnd)dstDoc.LastSection.Body.GetChild(NodeType.StructuredDocumentTagRangeEnd, 0,
                    false);
            Assert.That(sdtEnd, IsNot.Null());
        }

        /// <summary>
        /// Tests section break insertion into normal SDT.
        /// </summary>
        /// <remarks>
        /// Currently we split SDT content between sections leaving SDT with partial content in first section.
        /// Maybe we need to transform SDT into ranged but lets postpone for a while.
        /// </remarks>
        [Test]
        public void TestSdtInsertSectionBreak()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestSdtRange4 Dst.docx");

            DocumentBuilder builder = new DocumentBuilder(doc);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            builder.MoveTo(sdt.FirstChild);
            builder.InsertSection(SectionStart.NewPage);

            Assert.That(doc.Sections.Count, Is.EqualTo(2));
            Assert.That(doc.Sections[0].GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(1));
            Assert.That(doc.Sections[1].GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests case when ranged SDT is imported into another ranged SDT.
        /// </summary>
        [Test]
        public void TestRangedSdtImportToItself()
        {
            const string testName = @"Model\Markup\TestSdtRange5";

            Document doc = TestUtil.Open(testName, LoadFormat.Docx);

            // Originally we have one ranged SDT and it is ranged.
            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTagRangeStart, true);
            Assert.That(sdts.Count, Is.EqualTo(1));

            // Go to content of ranged SDT and insert document clone into.
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveTo(sdts[0]);
            builder.InsertDocument(doc.Clone(), ImportFormatMode.KeepDifferentStyles);

            // Check document is saved/loaded properly.
            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2Docx);

            sdts = doc.GetChildNodes(NodeType.StructuredDocumentTagRangeStart, true);

            // Now we have two ranged SDTs.
            Assert.That(sdts.Count, Is.EqualTo(2));

            // Both ranged.
            StructuredDocumentTagRangeStart sdtRangeStart = (StructuredDocumentTagRangeStart)sdts[0];
            Assert.That(sdtRangeStart.Id, Is.EqualTo(-1750807311));
            Assert.That(sdtRangeStart.Tag, Is.EqualTo("ExhibitB"));

            // Nested Sdt.Id updated during import.
            sdtRangeStart = (StructuredDocumentTagRangeStart)sdts[1];
            Assert.That(sdtRangeStart.Id, Is.EqualTo(1));
            Assert.That(sdtRangeStart.Tag, Is.EqualTo("ExhibitB"));
        }

        /// <summary>
        /// Tests that structured document tag is preserved during Docx2Docx roundtrip.
        /// </summary>
        [Test]
        public void Test15659()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test15659.docx");

            // Document contains one normal SDT and one ranged SDT.
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.Tag, Is.EqualTo("ExhibitA"));

            StructuredDocumentTagRangeStart sdtRangeStart =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            Assert.That(sdtRangeStart.Tag, Is.EqualTo("ExhibitB"));


            StructuredDocumentTagRangeEnd sdtRangeEnd = (StructuredDocumentTagRangeEnd)doc.LastSection.Body.GetChildNodes(NodeType.Any, false)[3];
            Assert.That(sdtRangeEnd.Id, Is.EqualTo(sdtRangeStart.Id));
            Assert.That(ReferenceEquals(sdtRangeStart.RangeEnd, sdtRangeEnd), Is.True);
        }

        /// <summary>
        /// WORDSNET-13519 Aspose.Words does not import StructuredDocumentTag containing section break
        /// </summary>
        [Test]
        public void Test13519()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test13519", UnifiedScenario.Docx2Docx);

            StructuredDocumentTagRangeStart tag = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.FirstChild;
            Assert.That(tag.Tag, Is.EqualTo("L2"));
            Assert.That(tag.Title, Is.EqualTo("L2"));
            Assert.That(tag.Level, Is.EqualTo(MarkupLevel.Block));

            // Check how range nodes located.
            Assert.That(ReferenceEquals(doc.Sections[0], tag.GetAncestor(NodeType.Section)), Is.True);
            Assert.That(ReferenceEquals(doc.Sections[1], tag.RangeEnd.GetAncestor(NodeType.Section)), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-13519 Checks that placeholder references are updated properly.
        /// </summary>
        [Test]
        public void Test13519A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test13519.docx");
            doc = doc.Clone();
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Assert.That(doc.GlossaryDocument.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
        }




        /// <summary>
        /// Demonstrates manual creation of ranged structured document tag.
        /// </summary>
        [Test]
        public void TestRangedSdtCreateManually()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestManual.docx");

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.FirstChild;

            // Say we want enclose both sections into structured document tag.
            sdt.ConvertToRange(doc.Sections[1].Body.FirstParagraph, true);

            TestUtil.SaveCheckGold(doc, @"Model\Markup\TestManual Modified.docx");
        }


        /// <summary>
        /// Tests that range start node cannot be Paragraph node child.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestCanInsert()
        {
            Document doc = new Document();
            doc.FirstSection.Body.FirstParagraph.AppendChild(
                new StructuredDocumentTagRangeStart(doc, new StructuredDocumentTag(doc, MarkupLevel.Block)));
        }


        /// <summary>
        /// WORDSNET-9908 RichText Content Control with Section Break are lost
        /// Resolved per WORDSNET-15659
        /// </summary>
        [Test]
        public void Test9908()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test9908.docx");

            // Document contains ranged SDT.
            StructuredDocumentTagRangeStart sdtRangeStart =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);

            Assert.That(sdtRangeStart.Tag, Is.EqualTo("contentcontrolwithsectionbreak"));
            Assert.That(sdtRangeStart.Title, Is.EqualTo("contentcontrolwithsectionbreak"));
            Assert.That(sdtRangeStart.Id, Is.EqualTo(-354802571));
        }


        /// <summary>
        /// WORDSNET-7150 SDT controls are getting lost during open/save
        /// Resolved per WORDSNET-15659
        /// </summary>
        [Test]
        public void Test7150()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test7150.docx");

            // Document contains two ranged SDT.
            StructuredDocumentTagRangeStart sdtRangeStart =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            Assert.That(sdtRangeStart.Tag, Is.EqualTo(""));
            Assert.That(sdtRangeStart.Title, Is.EqualTo(""));
            Assert.That(sdtRangeStart.Id, Is.EqualTo(12315546));

            sdtRangeStart = (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 1, true);
            Assert.That(sdtRangeStart.Tag, Is.EqualTo(""));
            Assert.That(sdtRangeStart.Title, Is.EqualTo(""));
            Assert.That(sdtRangeStart.Id, Is.EqualTo(12315552));
        }


        /// <summary>
        /// WORDSNET-19173 Content Control gets lost during open/save a DOCX
        /// Cannot reproduce issue.
        /// </summary>
        [Test]
        public void Test19173()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test19173.docx");

            IStructuredDocumentTag sdt = doc.Range.StructuredDocumentTags[0];

            Assert.That(sdt.Tag, Is.EqualTo("IN:Schedule of Activities"));
            Assert.That(sdt.Title, Is.EqualTo("Schedule of Activities"));
            Assert.That(sdt.Id, Is.EqualTo(-1419859338));
        }

        /// <summary>
        /// WORDSNET-19017 Content control lost during open/save a DOCX
        /// Cannot reproduce issue.
        /// </summary>
        [Test]
        public void Test19017()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test19017.docx");

            NodeCollection nodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            List <StructuredDocumentTag> tags = nodes.ToList<StructuredDocumentTag>();

            Assert.That(tags.Count, Is.EqualTo(3));

            Assert.That(tags[0].Tag, Is.EqualTo("type=ReportObject;reportobjectid=464;"));
            Assert.That(tags[0].Title, Is.EqualTo(""));
            Assert.That(tags[0].Id, Is.EqualTo(512048618));

            Assert.That(tags[1].Tag, Is.EqualTo("type=ReportObject;reportobjectid=462;"));
            Assert.That(tags[1].Title, Is.EqualTo(""));
            Assert.That(tags[1].Id, Is.EqualTo(1197336250));

            Assert.That(tags[2].Tag, Is.EqualTo("type=ReportObject;reportobjectid=463;"));
            Assert.That(tags[2].Title, Is.EqualTo(""));
            Assert.That(tags[2].Id, Is.EqualTo(1313943143));
        }

        /// <summary>
        /// WORDSNET-13601 (fixed partially) - Aspose.Words does not import content controls containing Table
        /// OOXML import/export resolved per WORDSNET-15659
        /// </summary>
        [Test]
        public void Test13601()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test13601.docx");

            // Document contains two ranged SDT.
            NodeCollection nodes = doc.GetChildNodes(NodeType.StructuredDocumentTagRangeStart, true);
            List <StructuredDocumentTagRangeStart> tags = nodes.ToList<StructuredDocumentTagRangeStart>();

            Assert.That(tags.Count, Is.EqualTo(2));

            Assert.That(tags[0].Title, Is.EqualTo(""));
            Assert.That(tags[0].Id, Is.EqualTo(1343885161));

            Assert.That(tags[1].Title, Is.EqualTo("תוכן"));
            Assert.That(tags[1].Id, Is.EqualTo(1698058318));
        }

        /// <summary>
        /// WORDSNET-20690 Empty Rich Text CustomXmlPart control pdf save issue.
        /// Added special handling for empty and whitespace only strings.
        /// </summary>
        [Test]
        public void Test20690()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20690.docx");

            CustomXmlPart customXmlPart = doc.CustomXmlParts[1];
            string customXmlPartData = System.Text.Encoding.UTF8.GetString(customXmlPart.Data);

            // Set bounded value to single space.
            customXmlPartData = customXmlPartData.Replace(">SimpleBooleanObject19.Value", "> ");
            customXmlPart.Data = System.Text.Encoding.UTF8.GetBytes(customXmlPartData);

            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 0, true);

            sdt.XmlMapping.UpdateContent();

            Assert.That(sdt.GetText(), Is.EqualTo(" \r"));
        }

        /// <summary>
        /// WORDSNET-21062 Implemented public <see cref="StructuredDocumentTagRangeStart.XmlMapping"/>
        /// property.
        /// </summary>
        [Test]
        public void TestSdtRangeXmlMapping()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21062.docx");

            StructuredDocumentTagRangeStart rangeStart =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            XmlMapping xmlMapping = rangeStart.XmlMapping;

            Assert.That(xmlMapping.StoreItemId, Is.EqualTo("{59080277-8942-4087-8692-E264018C2D64}"));
            Assert.That(xmlMapping.XPath, Is.EqualTo("/I[1]/C[1]"));

            // Generate document to put to custom XMl part as source of SDT XML mapping.
            DocumentBuilder builder = new DocumentBuilder();
            builder.Writeln("First section.");
            builder.InsertSection(SectionStart.NewPage);
            builder.Writeln("Next section.");

            string sdtDocXml;
            using (MemoryStream stream = new MemoryStream())
            {
                builder.Document.Save(stream, SaveFormat.FlatOpc);
                // Get data without BOM.
                const int bomLength = 3;
                sdtDocXml = Encoding.UTF8.GetString(stream.ToArray(), bomLength, (int)(stream.Length - bomLength));
            }

            // Use XmlDocument just to encode sdtDocXml to put into XML.
            XmlDocument xmlDoc = XmlUtilPal.LoadXml("<ns:parent xmlns:ns='someNamespace'><ns:child></ns:child></ns:parent>",true);
            xmlDoc.ChildNodes[0].ChildNodes[0].InnerText = sdtDocXml;

            string newPartId = Guid.NewGuid().ToString("B");
            CustomXmlPart newPart = doc.CustomXmlParts.Add(newPartId, XmlUtilPal.GetOuterXml(xmlDoc));

            xmlMapping.SetMapping(newPart, "/ns:parent/ns:child", "xmlns:ns='someNamespace'");

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test21062", UnifiedScenario.Docx2DocxNoGold);

            rangeStart = (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            xmlMapping = rangeStart.XmlMapping;

            Assert.That(xmlMapping.StoreItemId, Is.EqualTo(newPartId));
            Assert.That(xmlMapping.XPath, Is.EqualTo("/ns:parent/ns:child"));
            Assert.That(xmlMapping.PrefixMappings, Is.EqualTo("xmlns:ns='someNamespace'"));
        }

        /// <summary>
        /// WORDSNET-21062 Checks that <see cref="StructuredDocumentTagRangeStart.XmlMapping"/> data is copied
        /// correctly on importing a document containing <see cref="StructuredDocumentTagRangeStart"/> mapped to XML part.
        /// </summary>
        [Test]
        public void TestImportXmlMappedSdtRange()
        {
            Document srcDoc = TestUtil.Open(@"Model\Markup\Test21062.docx");
            Document dstDoc = new Document();
            dstDoc.RemoveAllChildren();
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            StructuredDocumentTagRangeStart rangeStart =
                (StructuredDocumentTagRangeStart)dstDoc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            XmlMapping xmlMapping = rangeStart.XmlMapping;

            Assert.That(xmlMapping.StoreItemId, Is.EqualTo("{59080277-8942-4087-8692-E264018C2D64}"));
            Assert.That(xmlMapping.CustomXmlPart, IsNot.Null());
            Assert.That(xmlMapping.XPath, Is.EqualTo("/I[1]/C[1]"));
        }

        /// <summary>
        /// WORDSNET-20969 Bullet is lost after re-saving DOCX
        /// Formatting of last paragraph of SDT content should be preserved.
        /// </summary>
        [Test]
        public void Test20969()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20969.docx");
            doc.UpdateListLabels();

            HeaderFooter header = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Assert.That(header.Paragraphs[0].ListLabel.LabelString, Is.EqualTo("\xf0b7"));
            Assert.That(header.Paragraphs[1].ListLabel.LabelString, Is.EqualTo("\xf0b7"));
            Assert.That(header.Paragraphs[2].ListLabel.LabelString, Is.EqualTo("\xf0b7"));
            Assert.That(header.Paragraphs[3].ListLabel.LabelString, Is.EqualTo(""));
            Assert.That(header.Paragraphs[4].ListLabel.LabelString, Is.EqualTo("\xf0b7"));
        }

        /// <summary>
        /// Additional test for WORDSNET-20969.
        /// </summary>
        [Test]
        public void Test20969A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20969A.docx");
            HeaderFooter header = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];

            // Tested on Microsoft® Word 2019 MSO (Version 2309 Build 16.0.16827.20166) 32-bit
            // Now last SDT paragraph is moved outside of SDT before update.
            // I think the reason is attempt to preserve at least one paragraph in story while SDT is being updated.
            Assert.That(header.Paragraphs[3].ParaPr[ParaAttr.Alignment], Is.Null);
            Assert.That(header.Paragraphs[4].ParaPr[ParaAttr.Alignment], Is.EqualTo(ParagraphAlignment.Center));
        }

        /// <summary>
        /// Additional test for WORDSNET-20969.
        /// </summary>
        [Test]
        public void Test20969B()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20969B.docx");
            Story headerPrimary = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Paragraph lastPara = headerPrimary.LastParagraph;

            Assert.That(lastPara.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(400));
        }

        /// <summary>
        /// Additional test for WORDSNET-20969.
        /// </summary>
        [Test]
        public void Test20969C()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20969C.docx");

            // Check primary header.
            Story headerPrimary = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            StructuredDocumentTag sdt = (StructuredDocumentTag)headerPrimary.FirstChild;

            List<Paragraph> paras = sdt.GetChildNodes(NodeType.Paragraph, false).ToList<Paragraph>();
            Assert.That(paras.Count, Is.EqualTo(4));

            Assert.That(paras[0].GetText(), Is.EqualTo("Header 1\r"));
            Assert.That(paras[1].GetText(), Is.EqualTo("Header 2\r"));
            Assert.That(paras[2].GetText(), Is.EqualTo("Header 3\r"));
            Assert.That(paras[3].GetText(), Is.EqualTo("Header 4\r"));

            foreach (Paragraph para in new Paragraph[] { paras[0], paras[1], paras[2] })
            {
                Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x0f));
                Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.EqualTo(0));
                Assert.That(para.ParaPr[ParaAttr.ListId], Is.EqualTo(1));
            }

            Assert.That(paras[3].ParaPr[ParaAttr.Istd], Is.Null);
            Assert.That(paras[3].ParaPr[ParaAttr.ListLevel], Is.Null);
            Assert.That(paras[3].ParaPr[ParaAttr.ListId], Is.Null);

            Paragraph lastPara = headerPrimary.LastParagraph;
            Assert.That(lastPara, Is.EqualTo(sdt.NextSibling));
            Assert.That(lastPara.GetText(), Is.EqualTo("\r"));

            Assert.That(lastPara.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(200));
            Assert.That(lastPara.ParaPr[ParaAttr.ListLevel], Is.Null);
            Assert.That(lastPara.ParaPr[ParaAttr.ListId], Is.Null);

            // Check primary footer.
            Story footerPrimary = doc.FirstSection.HeadersFooters[HeaderFooterType.FooterPrimary];
            sdt = (StructuredDocumentTag)footerPrimary.FirstChild;

            paras = sdt.GetChildNodes(NodeType.Paragraph, false).ToList<Paragraph>();
            Assert.That(paras.Count, Is.EqualTo(4));

            // All paragraph except last paragraph of SDT should have same formatting.
            foreach (Paragraph para in new Paragraph[] { paras[0], paras[1], paras[2], footerPrimary.LastParagraph })
            {
                Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x11));
                Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.EqualTo(0));
                Assert.That(para.ParaPr[ParaAttr.ListId], Is.EqualTo(2));
            }

            Assert.That(paras[3].ParaPr[ParaAttr.Istd], Is.Null);
            Assert.That(paras[3].ParaPr[ParaAttr.ListLevel], Is.Null);
            Assert.That(paras[3].ParaPr[ParaAttr.ListId], Is.Null);
        }

        /// <summary>
        /// Additional test for WORDSNET-20969.
        /// </summary>
        [Test]
        public void Test20969D()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20969D.docx");

            Story headerPrimary = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            StructuredDocumentTag sdt = (StructuredDocumentTag)headerPrimary.Range.StructuredDocumentTags[0];

            // Last paragraph of SDT is moved out regardless of first paragraph in header.
            Paragraph para = (Paragraph)sdt.NextSibling;
            Assert.That(para.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(200));
        }

        /// <summary>
        /// WORDSNET-21138 Content in last cell of table get duplicated after breaking the mapping between
        /// Content Control and CustomXMLPart.
        /// Fixed by removing all SDT child nodes when pasting content from SDT XML into a cell.
        /// </summary>
        /// <remarks>
        /// AM. Seems Microsoft is constantly changing logic.
        /// Tested on Microsoft® Word 2019 MSO (Version 2309 Build 16.0.16827.20130) 32-bit.
        /// </remarks>
        [Test]
        public void Test21138()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21138.docx");
            doc.UpdateListLabels();

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[0];

            // SDT content has got 4 paragraphs after the inserting from the SDT XML.
            Cell cell = sdt.FirstChild as Cell;
            Assert.That(cell.Paragraphs.Count, Is.EqualTo(4));

            // The third paragraph was copied from the SDT XML with using existing numbering with ListId equals to 2.
            Paragraph para = cell.Paragraphs[2];
            Assert.That(para.GetText(), Is.EqualTo("Bullet 2\r"));
            Assert.That(para.ParaPr[ParaAttr.Istd], Is.EqualTo(0x16));
            Assert.That(para.ParaPr[ParaAttr.ListId], Is.EqualTo(2));
            Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.EqualTo(0));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("\xf0b7"));

            // The fourth paragraph was copied from the SDT XML without numbering.
            // This paragraph received numbering of the previous content with ListId equals to 2.
            // (This is behaviour of the MS Word.)
            para = cell.Paragraphs[3];
            Assert.That(para.GetText(), Is.EqualTo("Bullet 3\a"));
            Assert.That(para.ParaPr[ParaAttr.Istd], Is.Null);
            Assert.That(para.ParaPr[ParaAttr.ListId], Is.Null);
            Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.Null);
        }


        /// <summary>
        /// WORDSNET-21246 Provide more properties/methods in StructuredDocumentTagRangeStart Class.
        /// Access Common Properties of an structured document tag range start.
        /// </summary>
        [Test]
        public void TestAccessSDTRangeStartCommonProperties()
        {
            const string placeholderName = "Custom placeholder name";
            const string tagName = "Custom tag";
            const string titleName = "Custom title";

            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Block);
            BuildingBlock block =
                doc.GlossaryDocument.GetBuildingBlock(BuildingBlockGallery.StructuredDocumentTagPlaceholderText,
                                                      SdtPlaceholderManager.BuildingBlockCategory,
                                                      SdtPlaceholderManager.TextPlaceholderName);
            block.Name = placeholderName;
            doc.FirstSection.Body.AppendChild(sdt);
            doc.AppendChild(new Section(doc)).AppendChild(new Body(doc));
            Paragraph para = new Paragraph(doc);
            doc.LastSection.Body.AppendChild(para);

            StructuredDocumentTagRangeStart sdtRangeStart = sdt.ConvertToRange(para, true);
            sdtRangeStart.Color = Color.Azure;
            sdtRangeStart.PlaceholderName = placeholderName;
            sdtRangeStart.Tag = tagName;
            sdtRangeStart.Title = titleName;
            sdtRangeStart.IsShowingPlaceholderText = true;
            sdtRangeStart.LockContents = true;
            sdtRangeStart.LockContentControl = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\TestSDTStartSetProperties.docx", UnifiedScenario.Docx2DocxNoGold);
            sdtRangeStart = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChildNodes(NodeType.Any, false)[1];
            block = doc.GlossaryDocument.GetBuildingBlock(BuildingBlockGallery.StructuredDocumentTagPlaceholderText,
                                                      SdtPlaceholderManager.BuildingBlockCategory,
                                                      SdtPlaceholderManager.TextPlaceholderName);

            Assert.That(sdtRangeStart.Color.ToArgb(), Is.EqualTo(Color.Azure.ToArgb()));
            Assert.That(sdtRangeStart.PlaceholderName, Is.EqualTo(placeholderName));
            Assert.That(sdtRangeStart.Placeholder, Is.EqualTo(block));
            Assert.That(sdtRangeStart.Tag, Is.EqualTo(tagName));
            Assert.That(sdtRangeStart.Title, Is.EqualTo(titleName));
            Assert.That(sdtRangeStart.IsShowingPlaceholderText, Is.True);
            Assert.That(sdtRangeStart.LockContents, Is.True);
            Assert.That(sdtRangeStart.LockContentControl, Is.True);
        }

        /// <summary>
        /// WORDSNET-21436 w14:paraId and w14:textId removed after re-saving the document.
        /// Fixed by copying ParaId and TextId properties of the paragraph when pasting content from SDT XML.
        /// </summary>
        [Test]
        public void Test21436()
        {
            const string fileNameWithoutExtension = @"Model\Markup\Test21436";

            Document doc = TestUtil.Open(fileNameWithoutExtension, LoadFormat.Docx);
            OoxmlSaveOptions so = new OoxmlSaveOptions() { IsTestMode = true, WriteExtendedIds = true };
            doc = TestUtil.SaveOpen(doc, fileNameWithoutExtension, so, false);

            Paragraph para = (doc.FirstSection.Body.FirstChild as StructuredDocumentTag).FirstChild as Paragraph;
            Assert.That(para.ParaId, Is.EqualTo(340224409));
            Assert.That(para.TextId, Is.EqualTo(359770520));
        }



        /// <summary>
        /// WORDSNET-21246 Provide more properties/methods in StructuredDocumentTagRangeStart Class.
        /// LastChild property added. AppendChild() added.
        /// </summary>
        [Test]
        public void Test21246_LastAppendChild()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_1.docx");

            // First level.
            StructuredDocumentTagRangeStart sdtStart =
                (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                    NodeType.StructuredDocumentTagRangeStart, 0, false);
            Paragraph expLastChild = doc.Sections[2].Body.Paragraphs[1];
            Assert.That(expLastChild.GetText(), Is.EqualTo("3 para inside sdtContent after SectionBreak para.\r"));
            Assert.That(sdtStart.LastChild, Is.EqualTo(expLastChild));
            Run run = new Run(doc);
            run.Text = "First level.";
            Paragraph para = new Paragraph(doc);
            para.AppendChild(run);
            sdtStart.AppendChild(para);
            Assert.That(sdtStart.LastChild, Is.EqualTo(para));
        }

        /// <summary>
        /// Relates to WORDSNET-21246
        /// </summary>
        [Test]
        public void Test21246_LastAppendChildNested()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_1.docx");
            // Nested level.
            StructuredDocumentTagRangeStart sdtStart = (StructuredDocumentTagRangeStart)doc.Sections[1].Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            Paragraph expLastChild = doc.Sections[2].Body.FirstParagraph;
            Assert.That(expLastChild.GetText().Substring(0, 56), Is.EqualTo("11 para insideInside sdtContent after SectionBreak para."));
            Assert.That(sdtStart.LastChild, Is.EqualTo(expLastChild));
            Run run = new Run(doc);
            run.Text = "Nested level.";
            Paragraph para = new Paragraph(doc);
            para.AppendChild(run);
            sdtStart.AppendChild(para);
            Assert.That(sdtStart.LastChild, Is.EqualTo(para));
        }

        /// <summary>
        /// WORDSNET-20863 Content control date is changed when DOCX is converted to PDF at Azure.
        /// SaveOptions.CustomTimeZoneInfo option has been added.
        /// </summary>
        [Test]
        [TestCase(14, "09/07/2020 13/00/00")]
        [TestCase(-14, "08/07/2020 09/00/00")]
        [TestCase(10, "09/07/2020 09/00/00")]
        [TestCase(-10, "08/07/2020 13/00/00")]
        [TestCase(0, "08/07/2020 23/00/00")]
        [JavaDelete]
        public void Test20863(int utcOffsetHours, string expectedText)
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test20863.docx");

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);   // FOSS: was Pdf; CustomTimeZoneInfo is format-agnostic.
            so.CustomTimeZoneInfo = TimeZoneInfo.CreateCustomTimeZone("Custom", new TimeSpan(utcOffsetHours, 0, 0), "Custom", "Custom");

            // Force to update SDT content.
            TestUtil.ExecuteValidator(doc, so);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[0];

            Assert.That(sdt.GetText(), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// WORDSNET-21770 StoreItemChecksum removed after re-saving document.
        /// The new property StoreItemChecksum has been added to the SDT XmlMapping.
        /// </summary>
        [Test]
        public void Test21770()
        {
            // This property is from the Word preview version, there is no spec for it.
            // Therefore at the moment only property reading/writing is implemented.
            // There is no any related behavior or public API changes, but it can be changed in the future.

            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test21770", UnifiedScenario.Docx2DocxNoGold);
            StructuredDocumentTag sdt = doc.FirstSection.Body.GetChildNodes(NodeType.StructuredDocumentTag, true)[0] as StructuredDocumentTag;

            Assert.That(sdt.XmlMapping.StoreItemChecksum, Is.EqualTo("D2D3xg=="));
        }

        /// <summary>
        /// WORDSNET-21591 Random content copied to the output after converting the DOCX file with Content Controls
        /// Do not update content of SDT loaded with mapped document in case of missing custom XML part.
        /// </summary>
        [Test]
        public void Test21591()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21591.docx");

            StructuredDocumentTag tag = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 1, true);
            Assert.That(tag.ParentNode.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));

            Assert.That(tag.GetText(), Is.EqualTo("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. " +
                            "Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero, sit amet commodo magna eros quis urna.\r" +
                            "Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus.\r"));
        }


        /// <summary>
        /// Tests nested ranged SDT ChildNodes property.
        /// </summary>
        [Test]
        public void Test21189A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21189A.docx");
            StructuredDocumentTagRangeStart outerTag = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.FirstChild;
            NodeCollection childNodes = outerTag.ChildNodes;

            // Check every child node of outer tag.
            Assert.That(childNodes.Count, Is.EqualTo(3));

            Assert.That(childNodes[0].NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(childNodes[0].GetText().StartsWith("First"), Is.True);

            Assert.That(childNodes[1].NodeType, Is.EqualTo(NodeType.StructuredDocumentTagRangeStart));

            Assert.That(childNodes[2].NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(childNodes[2].GetText().StartsWith("Second"), Is.True);

            // Check every child node of inner tag.
            StructuredDocumentTagRangeStart innerTag =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 1, true);
            childNodes = innerTag.ChildNodes;

            Assert.That(childNodes.Count, Is.EqualTo(2));

            Assert.That(childNodes[0].NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(childNodes[0].GetText().StartsWith("Inner First"), Is.True);

            Assert.That(childNodes[1].NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(childNodes[1].GetText().StartsWith("Inner Second"), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-21189
        /// Tests that ChildNodes and flat GetChildNodes are equal.
        /// </summary>
        [Test]
        public void Test21189GetChildNodes()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21189A.docx");

            foreach (StructuredDocumentTagRangeStart tag in
                doc.GetChildNodes(NodeType.StructuredDocumentTagRangeStart, true))
            {
                NodeCollection col1 = tag.ChildNodes;
                NodeCollection col2 = tag.GetChildNodes(NodeType.Any, false);

                Assert.That(col2.Count, Is.EqualTo(col1.Count));

                for (int i = 0; i < col1.Count; i++)
                    Assert.That(ReferenceEquals(col1[i], col2[i]), Is.True);
            }
        }

        /// <summary>
        /// Relates to WORDSNET-21189
        /// Tests deep collection behavior.
        /// </summary>
        [Test]
        public void Test21189GetChildNodesDeep()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21189A.docx");
            StructuredDocumentTagRangeStart outerTag = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.FirstChild;
            NodeCollection childNodes = outerTag.GetChildNodes(NodeType.Any, true);

            // Check few child node of outer tag.
            Assert.That(childNodes.Count, Is.EqualTo(12));

            Assert.That(childNodes[1].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[1].GetText(), Is.EqualTo("First"));

            Assert.That(childNodes[2].NodeType, Is.EqualTo(NodeType.StructuredDocumentTagRangeStart));

            Assert.That(childNodes[4].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[4].GetText(), Is.EqualTo("Inner "));

            Assert.That(childNodes[5].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[5].GetText(), Is.EqualTo("First"));

            Assert.That(childNodes[7].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[7].GetText(), Is.EqualTo("Inner "));

            Assert.That(childNodes[8].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[8].GetText(), Is.EqualTo("Second"));
        }

        /// <summary>
        /// Relates to WORDSNET-21189
        /// Tests deep collection behavior if NodeType specified.
        /// </summary>
        [Test]
        public void Test21189GetChildNodesDeepNodeType()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21189A.docx");
            StructuredDocumentTagRangeStart outerTag = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.FirstChild;
            NodeCollection childNodes = outerTag.GetChildNodes(NodeType.Run, true);

            // Check few child node of outer tag.
            Assert.That(childNodes.Count, Is.EqualTo(6));

            Assert.That(childNodes[0].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[0].GetText(), Is.EqualTo("First"));

            Assert.That(childNodes[1].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[1].GetText(), Is.EqualTo("Inner "));
        }

        /// <summary>
        /// WORDSNET-21636 Content formatting changed after delete XmlMapping.
        /// Several changes have been made in the SDT XML mapping.
        /// </summary>
        [Test]
        public void Test21636()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21636.docx");

            StructuredDocumentTag tag1 = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            StructuredDocumentTag tag2 = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 1, true);
            StructuredDocumentTag tag3 = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 2, true);

            // The first sdt should contain 4 paragraphs, and the last para should be list
            Assert.That(tag1.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
            Paragraph theLastPara = tag1.GetChildNodes(NodeType.Any, false)[3] as Paragraph;

            Assert.That(theLastPara.IsListItem, Is.True);

            // The second sdt should contain 4 paragraphs
            Assert.That(tag2.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(5));

            // The third sdt should contain 5 paragraphs
            Assert.That(tag3.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(6));
        }

        /// <summary>
        /// WORDSNET-21607 New style introduced after adding a new SDT.
        /// Fixed via using the same logic for inserting placeholder content as when inserting document into SDT.
        /// </summary>
        [Test]
        public void Test21607()
        {
            const string fileNameWithoutExtension = @"Model\Markup\Test21607";
            const string placeholderName = "DFF1698259834A289927FD7FC8BA2CC7";
            const string expectedPlaceholderText = "Click or tap here to enter text.\f";
            const int expectedIstd = 16;

            Document doc = TestUtil.Open(fileNameWithoutExtension, LoadFormat.Docx);
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.RichText, MarkupLevel.Block);
            doc.FirstSection.Body.AppendChild(sdt);

            // Setting the placeholder name triggers the SDT content update.
            sdt.PlaceholderName = (doc.GlossaryDocument.GetChildNodes(NodeType.Any, false)[0] as BuildingBlock).Name;

            // Placeholder style has NOT been imported.
            Assert.That(doc.Styles.GetByName(placeholderName, false), Is.Null);

            // But this style is in the GlossaryDocument.Styles.
            Assert.That(doc.GlossaryDocument.Styles.GetByName(placeholderName, false), IsNot.Null());

            Paragraph para = sdt.GetChildNodes(NodeType.Any, false)[0] as Paragraph;

            // There is no style in the updated SDT content.
            Assert.That(para.ParaPr.ContainsKey(ParaAttr.Istd), Is.False);
            Assert.That(para.GetText(), Is.EqualTo(expectedPlaceholderText));

            para = sdt.Placeholder.FirstSection.Body.FirstParagraph;

            // The style is still in the placeholder.
            Assert.That((int)para.ParaPr[ParaAttr.Istd], Is.EqualTo(expectedIstd));
            Assert.That(sdt.PlaceholderName, Is.EqualTo(placeholderName));

            // Saving and reading show the same the result.
            doc = TestUtil.SaveOpen(doc, fileNameWithoutExtension, UnifiedScenario.Docx2DocxNoGold);
            sdt = doc.GetChildNodes(NodeType.StructuredDocumentTag, true)[1] as StructuredDocumentTag;

            Assert.That(doc.Styles.GetByName(placeholderName, false), Is.Null);
            Assert.That(doc.GlossaryDocument.Styles.GetByName(placeholderName, false), IsNot.Null());
            Assert.That(sdt.PlaceholderName, Is.EqualTo(placeholderName));

            para = sdt.GetChildNodes(NodeType.Any, false)[0] as Paragraph;
            Assert.That(para.ParaPr.ContainsKey(ParaAttr.Istd), Is.False);
            Assert.That(para.GetText(), Is.EqualTo(expectedPlaceholderText));

            para = sdt.Placeholder.FirstSection.Body.FirstParagraph;
            Assert.That((int)para.ParaPr[ParaAttr.Istd], Is.EqualTo(expectedIstd));
            Assert.That(para.GetText(), Is.EqualTo(expectedPlaceholderText));
        }


        /// <summary>
        /// WORDSNET-21246
        /// Added public method RemoveSelfOnly to ranged SDT.
        /// </summary>
        [Test]
        public void TestRangeRemoveSelfOnly()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            StructuredDocumentTagRangeEnd end = start.RangeEnd;
            Node pseudoChild = start.NextSibling;
            start.RemoveSelfOnly();

            Assert.That(start.IsRemoved, Is.True);
            Assert.That(end.IsRemoved, Is.True);
            Assert.That(pseudoChild.IsRemoved, Is.False);
        }

        /// <summary>
        /// WORDSNET-21246
        /// Added public method RemoveAllChildren to ranged SDT.
        /// </summary>
        [Test]
        public void TestRangeRemoveAllChildren()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2", LoadFormat.Docx);

            // First level.
            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            Assert.That(start.Id, Is.EqualTo(0));

            Section sect = start.ParentNode.ParentNode as Section;
            Assert.That(sect.SectPr.PageWidth, Is.EqualTo(6240));

            start.RemoveAllChildren();

            // Original StructuredDocumentTagRangeStart jumps into the other section.
            start = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTagRangeStart,
                0, false);
            sect = start.ParentNode.ParentNode as Section;
            Assert.That(sect.SectPr.PageWidth, Is.EqualTo(12240));

            // Now both range nodes are located in one section. Check that RemoveAllChildren does not fail.
            Assert.That(ReferenceEquals(start.NextSibling, start.RangeEnd), Is.True);
            start.RemoveAllChildren();

            // Check the same but with single node between.
            start.ParentNode.InsertAfter(new Paragraph(doc), start);
            Assert.That(ReferenceEquals(start.NextSibling, start.RangeEnd), Is.False);
            start.RemoveAllChildren();
            Assert.That(ReferenceEquals(start.NextSibling, start.RangeEnd), Is.True);
        }

        /// <summary>
        /// WORDSNET-21246
        /// Test how RemoveAllChildren handles nested ranged SDT.
        /// </summary>
        [Test]
        public void TestRangeRemoveAllChildrenNested()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            int nodeCountBefore = doc.GetChildNodes(NodeType.Any, true).Count;
            Assert.That(nodeCountBefore, Is.EqualTo(28));
            // Nested level.
            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.Sections[1].Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            StructuredDocumentTagRangeEnd end = start.RangeEnd;
            start.RemoveAllChildren();

            int nodeCountAfter = doc.GetChildNodes(NodeType.Any, true).Count;

            Assert.That(start.IsRemoved, Is.False);
            Assert.That(end.IsRemoved, Is.False);
            Assert.That(start.NextSibling, Is.EqualTo(end));
            Assert.That(nodeCountAfter, Is.EqualTo(18));
        }

        /// <summary>
        /// WORDSNET-21246
        /// Tests IEnumerable implementing.
        /// </summary>
        [TestCase (0, new NodeType[] { NodeType.Paragraph, NodeType.Paragraph,
            NodeType.StructuredDocumentTagRangeStart, NodeType.Paragraph, NodeType.Paragraph,
            NodeType.StructuredDocumentTagRangeEnd, NodeType.Paragraph })]
        [TestCase(1, new NodeType[] { NodeType.Paragraph, NodeType.Paragraph })]
        public void TestSdtRangeEnumerable(int sectionIndex, NodeType[] types)
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.Sections[sectionIndex].Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);

            int nodeCount = 0;
            foreach (Node node in start)
            {
                Assert.That(node.NodeType, Is.EqualTo(types[nodeCount]));
                nodeCount++;
            }
            Assert.That(types.Length, Is.EqualTo(nodeCount));

            // Checks empty range.
            start.RemoveAllChildren();
            foreach (Node node in start)
                Assert.That(false, Is.True);
        }

        /// <summary>
        /// WORDSNET-21246
        /// Tests IEnumerable resilience when RangeEnd == null.
        /// </summary>
        [Test]
        [JavaDelete("Java porting still not support Linq Extension")]
        public void TestSdtRangeEnumerable1()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            // Simulate start.RangeEnd == null.
            start.SetId(42);

            Assert.That(start.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-21246
        /// Tests IEnumerable resilience when SDT start in air.
        /// </summary>
        [Test]
        [JavaDelete("Java porting still not support Linq Extension")]
        public void TestSdtRangeEnumerable2()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            start.Remove();

            Assert.That(start.Count(), Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-21246
        /// Tests IEnumerable resilience when SDT range Body container is in air.
        /// </summary>
        [Test]
        [JavaDelete("Java porting still not support Linq Extension")]
        public void TestSdtRangeEnumerable3()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);
            start.RemoveAllChildren();
            start.ParentNode.InsertAfter(new Paragraph(doc), start);
            doc.FirstSection.Body.Remove();

            Assert.That(start.Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-21246
        /// Tests simple LINQ query for ranged SDT.
        /// </summary>
        [Test]
        [JavaDelete("Java porting still not support Linq Extension")]
        public void TestSdtRangeLinq()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21246_2.docx");

            StructuredDocumentTagRangeStart rangedSdt = (StructuredDocumentTagRangeStart)doc.FirstSection.Body.GetChild(
                NodeType.StructuredDocumentTagRangeStart, 0, false);

            Assert.That(rangedSdt.LastOrDefault().NodeType, Is.EqualTo(NodeType.Paragraph));

            rangedSdt.RemoveAllChildren();
            Assert.That(rangedSdt.LastOrDefault(), Is.Null);
        }

        /// <summary>
        /// WORDSNET-21246
        /// Tests public constructors for SDT range start and SDT range end.
        /// </summary>
        [Test]
        public void TestSdtRangePublicCtors()
        {
            const string filename = @"Model\Markup\Test21246_3.docx";
            Document doc = TestUtil.Open(filename);

            StructuredDocumentTagRangeStart start = new StructuredDocumentTagRangeStart(doc, SdtType.RepeatingSectionItem);
            StructuredDocumentTagRangeEnd end = new StructuredDocumentTagRangeEnd(doc, start.Id);

            doc.FirstSection.Body.InsertAfter(start, doc.FirstSection.Body.FirstParagraph);
            doc.LastSection.Body.InsertBefore(end, doc.LastSection.Body.LastParagraph);

            Assert.That(start.RangeEnd, Is.EqualTo(end));
            Assert.That(end.Id, Is.EqualTo(start.Id));
            Assert.That(start.SdtType, Is.EqualTo(SdtType.RepeatingSectionItem));
            Assert.That(start.Level, Is.EqualTo(MarkupLevel.Block));

            TestUtil.Save(doc, filename, new OoxmlSaveOptions(SaveFormat.Docx), true);
        }

        /// <summary>
        /// WORDSNET-21979 Does not update SDT content in document.xml parts after calling UpdatePageLayout()
        /// In case mapped value is empty we need to update SDT content with the placeholder.
        /// </summary>
        [Test]
        [JavaDelete("Java porting still not support Linq Extension")]
        public void Test21979()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test21979.docx");

            StructuredDocumentTag sdt1 = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            StructuredDocumentTag sdt2 = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 1, true);

            Assert.That(sdt1.GetText(), Is.EqualTo("Rich text old content \r"));
            Assert.That(sdt2.GetText(), Is.EqualTo("Plain text old content\f"));

            foreach (StructuredDocumentTag sdt in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                CustomXmlPart cxp = sdt.XmlMapping.CustomXmlPart;
                if (cxp.Schemas.FirstOrDefault().Contains("urn:rich:text"))
                    cxp.Data = Encoding.ASCII.GetBytes("<I xmlns=\"urn:rich:text\"></I>");
                else
                    cxp.Data = Encoding.ASCII.GetBytes("<I xmlns=\"urn:plain:text\"></I>");
            }

            // FOSS: force the SDT placeholder re-evaluation via document validation (was layout-driven).
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            const string expectedPlaceholderText1 = "Click or tap here to enter text.\r";
            const string expectedPlaceholderText2 = "Click or tap here to enter text.\f";

            Assert.That(sdt1.GetText(), Is.EqualTo(expectedPlaceholderText1));
            Assert.That(sdt2.GetText(), Is.EqualTo(expectedPlaceholderText2));

            foreach (StructuredDocumentTag sdt in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                sdt.XmlMapping.Delete();
            }

            Assert.That(sdt1.GetText(), Is.EqualTo(expectedPlaceholderText1));
            Assert.That(sdt2.GetText(), Is.EqualTo(expectedPlaceholderText2));
        }

        /// <summary>
        /// WORDSNET-22091 Resaving DOCX file corrupts table in output file.
        /// Fixed by keeping the reference node if it is a paragraph after the table in the SDT content.
        /// </summary>
        [Test]
        public void Test22091()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test22091", LoadFormat.Docx);
            StructuredDocumentTag tag = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(tag.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(tag.GetChildNodes(NodeType.Any, false)[0].NodeType, Is.EqualTo(NodeType.Table));

            // Check that mandatory paragraph node exists after table.
            Assert.That(tag.GetChildNodes(NodeType.Any, false)[1].NodeType, Is.EqualTo(NodeType.Paragraph));
        }

        /// <summary>
        /// WORDSNET-21818 Add feature to set content control (checkbox) checked/unchecked symbol.
        /// Corresponding public methods for the <see cref="StructuredDocumentTag"/> class have been implemented.
        /// </summary>
        /// <remarks>
        /// See <see cref="StructuredDocumentTag.SetCheckedSymbol(int, string)"/> and
        /// <see cref="StructuredDocumentTag.SetUncheckedSymbol(int, string)"/>.
        /// </remarks>
        [Test]
        public void Test21818()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Checkbox, MarkupLevel.Block);
            doc.Sections[0].Body.AppendChild(tag);
            tag.SetCheckedSymbol(0x006F, "Wingdings");
            tag.SetUncheckedSymbol(0x00FC, "Wingdings");

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test21818", UnifiedScenario.Docx2DocxNoGold);
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Sections[0].Body.GetChildNodes(NodeType.Any, false)[1];
            Assert.That(sdt.ControlProperties.Type, Is.EqualTo(SdtType.Checkbox));
            SdtCheckBox checkbox = (SdtCheckBox)sdt.ControlProperties;
            Assert.That(checkbox.Checked, Is.False);
            SdtCheckBoxStateInfo stateInfo = checkbox.CheckedStateInfo;
            Assert.That(stateInfo.FontName, Is.EqualTo("Wingdings"));
            Assert.That(stateInfo.CharacterCode, Is.EqualTo(0x006F));
            stateInfo = checkbox.UncheckedStateInfo;
            Assert.That(stateInfo.FontName, Is.EqualTo("Wingdings"));
            Assert.That(stateInfo.CharacterCode, Is.EqualTo(0x00FC));
        }

        /// <summary>
        /// Related to WORDSNET-21818
        /// Checks thrown exceptions for unsuitable SDT types.
        /// </summary>
        [Test]
        public void Test21818_Exceptions()
        {
            const string invalidOpExceptionExpected = "An InvalidOperationException was expected but was not thrown.";
            const string unexpectedException = "Unexpected exception.";

            Document doc = new Document();

#if JAVA || CPLUSPLUS
            SdtType[] sdtTypes = Enum.GetValues(typeof(SdtType));
#else
            SdtType[] sdtTypes = (SdtType[])Enum.GetValues(SdtType.None.GetType());
#endif
            foreach (SdtType sdtType in sdtTypes)
            {
                if (sdtType == SdtType.Checkbox)
                    continue;
                try
                {
                    StructuredDocumentTag tag = new StructuredDocumentTag(doc, sdtType, MarkupLevel.Block);
                    try
                    {
                        tag.SetCheckedSymbol(0x006F, "Wingdings");
                        Assert.Fail(invalidOpExceptionExpected);
                    }
                    catch (InvalidOperationException) { }
                    catch { Assert.Fail(unexpectedException); }
                    try
                    {
                        tag.SetUncheckedSymbol(0x00FC, "Wingdings");
                        Assert.Fail(invalidOpExceptionExpected);
                    }
                    catch (InvalidOperationException) { }
                    catch { Assert.Fail(unexpectedException); }
                }
                catch (ArgumentException) { }
                catch { Assert.Fail(unexpectedException); }
            }
        }

        /// <summary>
        /// WORDSNET-22031 Content Control (dropdown) value is changed after re-saving DOCX.
        /// Fixed by adjusting the XmlMapping.StoreItemId of the imported SDT when loading the alt-chunk document.
        /// </summary>
        [TestCase(false)]
        [TestCase(true)]
        public void Test22031(bool doRoundTrip)
        {
            const string testFileNameWithoutExtension = @"Model\Markup\Test22031";
            const string storeItemId = "{63F03B72-BEF0-4E2E-A36D-39BD14C5C261}";

            // XML mapping updated during both post loading and validation stages and it useful to test with/without roundtrip.
            Document doc = doRoundTrip
                ? TestUtil.OpenSaveOpen(testFileNameWithoutExtension, UnifiedScenario.Docx2DocxNoGold)
                : TestUtil.Open(testFileNameWithoutExtension, LoadFormat.Docx);

            Assert.That(doc.CustomXmlParts.GetById(storeItemId), IsNot.Null());

            StructuredDocumentTag tag = doc.Sections[0].Body.GetChildNodes(NodeType.Any, false)[0] as StructuredDocumentTag;
            StructuredDocumentTag sdt = (tag.GetChildNodes(NodeType.Any, false)[0] as Paragraph).GetChildNodes(NodeType.Any, false)[3] as StructuredDocumentTag;

            // The 1st SDT imported from the alt-chunk document refers to the existing CustomXmlPart.
            Assert.That(sdt.XmlMapping.StoreItemId, Is.EqualTo(storeItemId));
            Assert.That((sdt.GetChildNodes(NodeType.Any, false)[0] as Run).Text, Is.EqualTo("Existing Client"));

            sdt = (tag.GetChildNodes(NodeType.Any, false)[0] as Paragraph).GetChildNodes(NodeType.Any, false)[5] as StructuredDocumentTag;

            // The 2nd SDT imported from the alt-chunk document refers to the existing CustomXmlPart.
            Assert.That(sdt.XmlMapping.StoreItemId, Is.EqualTo(storeItemId));
            Assert.That((sdt.GetChildNodes(NodeType.Any, false)[0] as Run).Text, Is.EqualTo("Test Contact"));
        }

        /// <summary>
        /// WORDSNET-20920 StructuredDocumentTag content is changed after UpdatePageLayout().
        /// Checks if the content has actually been replaced.
        /// </summary>
        [Test]
        public void Test20920()
        {
            Document doc = TestUtil.Open(@"Other\DocumentPager\TestGlossary", LoadFormat.Docx);
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag,
                4, true);

            Assert.That(sdt.IsShowingPlaceholderText, Is.True);
            // No update page layout in FOSS.
            Assert.That(sdt.IsShowingPlaceholderText, Is.True);
            Assert.That(((Run)sdt.GetChildNodes(NodeType.Any, false)[0]).Text, Is.EqualTo("Type the document title"));
        }

        /// <summary>
        /// WORDSNET-22241 Checked symbol of content control is changed after first click. Fixed by
        /// adjusting the mechanism to determine whether to write the contents of Checkbox SDT as a symbol or text.
        /// </summary>
        [Test]
        public void Test22241()
        {
            Document doc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(doc, SdtType.Checkbox, MarkupLevel.Block);
            doc.Sections[0].Body.AppendChild(tag);
            tag.SetCheckedSymbol(0x00A4, "Wingdings");
            tag.SetUncheckedSymbol(0x00A6, "Wingdings");
            tag.Checked = true;

            Run run = (tag.GetChildNodes(NodeType.Any, false)[0] as Paragraph).Runs[0];
            Assert.That(run.IsWriteAsSymbol, Is.True);
            Assert.That(run.IsSymbolic, Is.False);
            Assert.That(run.Text, Is.EqualTo("¤"));

            DocxExportContext ctx = new DocxExportContext(doc);
            XmlNodeList runs = ctx.XmlDocument.GetElementsByTagName("w:r");
            Assert.That(runs[0].ChildNodes.Count, Is.EqualTo(2));
            Assert.That(runs[0].ChildNodes[0].Name, Is.EqualTo("w:rPr"));
            Assert.That(runs[0].ChildNodes[1].Name, Is.EqualTo("w:sym"));
            Assert.That(runs[0].ChildNodes[1].Attributes.Count, Is.EqualTo(2));
            Assert.That(runs[0].ChildNodes[1].Attributes["w:font"].Value, Is.EqualTo("Wingdings"));
            Assert.That(runs[0].ChildNodes[1].Attributes["w:char"].Value, Is.EqualTo("00A4"));

            // Re-saving and opening confirm the result.
            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test22241", UnifiedScenario.Docx2Docx);
            tag = doc.GetChild(NodeType.StructuredDocumentTag, 0, true) as StructuredDocumentTag;
            run = (tag.GetChildNodes(NodeType.Any, false)[0] as Paragraph).Runs[0];
            Assert.That(run.IsWriteAsSymbol, Is.True);
            Assert.That(run.IsSymbolic, Is.False);
            Assert.That(run.Text, Is.EqualTo("¤"));
        }


        /// <summary>
        /// WORDSNET-20808 AW does not import content controls containing Table.
        /// ToString() method for StructuredDocumentTagRangeStart node has been implemented.
        /// </summary>
        [Test]
        public void Test20808()
        {
            const string stdContent =
                "A\r\n\r\n\r\n\r\n#\rZ\rY\rX\r1.\rZ1\rY1\rX1\r2.\rZ1\rY1\rX1\r3.\rZ1\rY1\rX1\r4.\rZ1\rY1\rX1\r5.\rZ1\rY1" +
                "\rX1\r6.\rZ1\rY1\rX1\r7.\rZ1\rY1\rX1\r8.\rZ1\rY1\rX1\r9.\rZ1\rY1\rX1\r10.\rZ1\rY1\rX1\r11.\r\n\rZ1\rY1" +
                "\rX1\r12.\rZ1\rY1\rX1\rCCC\r13.\rZ1\rY1\rX1\rBBB\r14.\rZ1\rY1\rX1\r15.\rZ1\rY1\rX1\r16.\rZ1\rY1\rX1\r17." +
                "\rZ1\rY1\rX1\r18.\rZ1\rY1\rX1\r\r\nA\r\n";

            Document doc = TestUtil.Open(@"Model\Markup\Test20808.docx");
            StructuredDocumentTagRangeStart start = doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTagRangeStart,
                    0, false) as StructuredDocumentTagRangeStart;
            Assert.That(start.ToString(new TxtSaveOptions()), Is.EqualTo(stdContent));
        }

        /// <summary>
        /// WORDSNET-22651 StructuredDocumentTag - control Appearance.
        /// Appearance property has been published.
        /// </summary>
        [Test]
        public void Test22651()
        {
            const string testFile = @"Model\Markup\Test22651.docx";
            Document doc = TestUtil.Open(testFile);

            StructuredDocumentTag sdt1 = doc.GetChild(NodeType.StructuredDocumentTag, 0, true) as StructuredDocumentTag;
            StructuredDocumentTag sdt2 = doc.GetChild(NodeType.StructuredDocumentTag, 1, true) as StructuredDocumentTag;
            StructuredDocumentTag sdt3 = doc.GetChild(NodeType.StructuredDocumentTag, 2, true) as StructuredDocumentTag;

            Assert.That(sdt1.Appearance, Is.EqualTo(SdtAppearance.BoundingBox));
            Assert.That(sdt2.Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(sdt3.Appearance, Is.EqualTo(SdtAppearance.Hidden));

            sdt1.Appearance = SdtAppearance.Tags;
            sdt2.Appearance = SdtAppearance.Hidden;
            sdt3.Appearance = SdtAppearance.BoundingBox;

            TestUtil.SaveCheckGold(doc, testFile);
        }

        /// <summary>
        /// WORDSNET-22616 StructuredDocumentTagRangeStart\End pair removed from document model
        /// The last node of the range, with which the error occurred, from the point of view of the reader, is
        /// a paragraph in a table. An end node of an SDT range cannot be inserted at this position. Therefore,
        /// an appropriate ancestor of a range last node is now used as the end of the range.
        /// </summary>
        [Test]
        public void Test22616()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test22616", UnifiedScenario.Docx2DocxNoGold);

            StructuredDocumentTagRangeStart rangeStart =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            Assert.That(rangeStart, IsNot.Null());
            Assert.That(rangeStart.PreviousSibling, Is.SameAs(doc.FirstSection.Body.FirstChild));

            StructuredDocumentTagRangeEnd rangeEnd =
                (StructuredDocumentTagRangeEnd)doc.LastSection.Body.GetChildNodes(NodeType.Any, false)[-2];

            Assert.That(rangeEnd, Is.SameAs(rangeStart.RangeEnd));
            Assert.That(rangeStart.Id, Is.EqualTo(1479353831));
            Assert.That(rangeEnd.Id, Is.EqualTo(1479353831));
            Assert.That(rangeStart.SdtType, Is.EqualTo(SdtType.RichText));
        }

        /// <summary>
        /// WORDSNET-22906 Rich text content control is not visible when the SetMapping function is used
        /// The SDT became empty after update and then lost on further opening/saving. Now the default contents is
        /// inserted if an SDT is empty after update, just like MS Word does.
        /// </summary>
        [Test]
        public void Test22906()
        {
            const string fileName = @"Model\Markup\Test22906.docx";
            Document document = TestUtil.Open(fileName);

            // Save empty word document to FlatOPC.
            string flatOpcXml;
            using (MemoryStream stream = new MemoryStream())
            {
                Document flatOpc = new Document();
                flatOpc.Save(stream, SaveFormat.FlatOpc);
                Encoding encoding = new UTF8Encoding(true);
                byte[] data = stream.ToArray();
                flatOpcXml = encoding.GetString(data);
            }

            // Map FlatOPC.
            string xml = string.Format("<i>{0}</i>", EscapeXmlText(flatOpcXml));
            CustomXmlPart customXmlPart = document.CustomXmlParts.Add("test", xml);
            NodeCollection sdts = document.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(1));
            StructuredDocumentTag sdt = (StructuredDocumentTag)sdts[0];
            sdt.XmlMapping.SetMapping(customXmlPart, "/i", "");

            // Update and check for non-empty string to determine whether placeholder is inserted.
            sdt.XmlMapping.UpdateContent();
            Assert.That(sdt.GetText(), Is.EqualTo("Click or tap here to enter text.\r"));

            document = TestUtil.SaveOpen(document, fileName, null, false);
            sdts = document.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(1));
            Assert.That(sdts[0].GetText(), Is.EqualTo("Click or tap here to enter text.\r"));
        }

        /// <summary>
        /// Test the <see cref="Document.HasCoverPage"/> method finds cover page in different circumstances.
        /// </summary>
        [Test]
        [TestCase("TestCoverPage.docx")]
        [TestCase("TestCoverPageEdited.docx")]
        [TestCase("TestCoverPageMultiple.docx")]
        [TestCase("TestCoverPageAfterBreaks.docx")]
        public void TestCoverPage(string file)
        {
            Document document = TestUtil.Open(Path.Combine(@"Model\Markup", file));

            bool hasCoverPage = document.HasCoverPage();

            Assert.That(hasCoverPage, Is.True);
        }

        /// <summary>
        /// Test the <see cref="Document.HasCoverPage"/> method ignore improper cover pages.
        /// </summary>
        [Test]
        public void TestNotCoverPage()
        {
            Document document = TestUtil.Open(@"Model\Markup\TestNotCoverPage.docx");

            bool hasCoverPage = document.HasCoverPage();

            Assert.That(hasCoverPage, Is.False);
        }

        /// <summary>
        /// WORDSNET-22883 Cross-reference not working after removing XML mapping
        /// Tests that <see cref="BookmarkStart.DisplacedBy"/> and <see cref="BookmarkEnd.DisplacedBy"/> properties of
        /// bookmarks that refer to a <see cref="StructuredDocumentTag"/> are reset after the SDT is removed.
        /// </summary>
        [TestCase(@"Model\Markup\Test22883.docx", "_Ref85734851", false)]
        [TestCase(@"Model\Markup\Test22883.docx", "_Ref85734870", false)]
        public void Test22883(string fileName, string bookmarkName, bool isStartInsideSdt)
        {
            Document document = TestUtil.Open(fileName);
            Document clone = document.Clone();

            // Test StructuredDocumentTag.RemoveSelfOnly.

            Bookmark bookmark = document.Range.Bookmarks[bookmarkName];
            string bookmarkText = bookmark.Text;

            BookmarkStart start = bookmark.BookmarkStart;
            BookmarkEnd end = bookmark.BookmarkEnd;
            StructuredDocumentTag sdt = isStartInsideSdt
                ? (StructuredDocumentTag)start.ParentNode
                : (StructuredDocumentTag)start.NextNonAnnotationSibling;

            sdt.RemoveSelfOnly(false);

            Assert.That(bookmark.Text, Is.EqualTo(bookmarkText));
            Assert.That(start.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
            Assert.That(end.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));

            // Test StructuredDocumentTag.Remove.

            bookmark = clone.Range.Bookmarks[bookmarkName];
            start = bookmark.BookmarkStart;
            end = bookmark.BookmarkEnd;
            sdt = isStartInsideSdt
                ? (StructuredDocumentTag)start.ParentNode
                : (StructuredDocumentTag)start.NextNonAnnotationSibling;

            sdt.Remove();

            Assert.That(start.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
            Assert.That(end.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
        }

        /// <summary>
        /// WORDSNET-22883 Cross-reference not working after removing XML mapping
        /// Tests that <see cref="BookmarkStart.DisplacedBy"/> and <see cref="BookmarkEnd.DisplacedBy"/> properties of
        /// bookmarks that refer to an SDT range are reset after start/end nodes of the range are removed.
        /// </summary>
        [Test()]
        public void Test22883SdtRange()
        {
            Document document = TestUtil.Open(@"Model\Markup\Test22883SdtRange.docx");

            CheckBookmarks(document, 4, "Text 1\fText 2", false, false);

            Node stdRangeStart = document.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            stdRangeStart.Remove();

            Node stdRangeEnd = document.GetChild(NodeType.StructuredDocumentTagRangeEnd, 0, true);
            stdRangeEnd.Remove();

            CheckBookmarks(document, 4, "Text 1\fText 2", true, false);
        }

        /// <summary>
        /// WORDSNET-22883 Cross-reference not working after removing XML mapping
        /// Tests that <see cref="BookmarkStart.DisplacedBy"/> and <see cref="BookmarkEnd.DisplacedBy"/> properties of
        /// bookmarks that refer to a <see cref="StructuredDocumentTag"/> are NOT reset after the SDT is removed if
        /// there is another SDT they can refer to.
        /// </summary>
        [Test()]
        public void Test22883MultipleSdts()
        {
            Document document = TestUtil.Open(@"Model\Markup\Test22883MultipleSdts.docx");
            Document clone = document.Clone();

            CheckBookmarks(document, 2, "123", false, true);

            StructuredDocumentTag std = (StructuredDocumentTag)document.GetChild(NodeType.StructuredDocumentTag, 0, true);
            std.RemoveSelfOnly();

            CheckBookmarks(document, 2, "123", false, true);

            std = (StructuredDocumentTag)clone.GetChild(NodeType.StructuredDocumentTag, 1, true);
            std.RemoveSelfOnly();

            CheckBookmarks(clone, 2, "123", false, true);
        }

        private static void CheckBookmarks(Document document, int expectedCount, string expectedText,
            bool checkForUnspecifiedDisplacedByType, bool checkForNonUnspecifiedDisplacedByType)
        {
            Assert.That(document.Range.Bookmarks.Count, Is.EqualTo(expectedCount));

            foreach (Bookmark bookmark in document.Range.Bookmarks)
            {
                Assert.That(bookmark.Text, Is.EqualTo(expectedText));

                if (checkForUnspecifiedDisplacedByType)
                {
                    Assert.That(bookmark.BookmarkStart.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
                    Assert.That(bookmark.BookmarkEnd.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
                }

                if (checkForNonUnspecifiedDisplacedByType)
                {
                    Assert.That(bookmark.BookmarkStart.DisplacedBy, IsNot.EqualTo(DisplacedByType.Unspecified));
                    Assert.That(bookmark.BookmarkEnd.DisplacedBy, IsNot.EqualTo(DisplacedByType.Unspecified));
                }
            }
        }

        /// <summary>
        /// WORDSNET-23213 The end-of-cell marker has been removed from the new row.
        /// When updating TextBox SDT, we should not remove last empty paragraph, if it is numbered.
        /// </summary>
        [Test]
        public void Test23213()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test23213.docx");
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            CustomXmlPart cxp = doc.CustomXmlParts[0];
            sdt.XmlMapping.SetMapping(cxp, "/ns0:I[1]", "xmlns:ns0='urn:in:item1'");

            // Force to update SDT content.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            // Check that cell marker is not deleted in problematic cell.
            Cell cell = doc.FirstSection.Body.Tables[0].FirstRow.Cells[1];
            Assert.That(cell.LastParagraph.IsDeleteRevision, Is.False);
            Assert.That(cell.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
        }


        /// <summary>
        /// WORDSNET-23301 Consider providing API to access SDTs by id or name.
        /// Tests getting the structured document tag by the title, tag and Id.
        /// </summary>
        [Test]
        public void Test23301()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test23301.docx");

            // Checks getting the structured document tag by Id.
            IStructuredDocumentTag sdt = doc.Range.StructuredDocumentTags.GetById(1160505028);
            StructuredDocumentTag sdtNode = sdt.Node as StructuredDocumentTag;
            Assert.That(sdt.IsMultiSection, Is.EqualTo(false));
            Assert.That(sdt.Id, Is.EqualTo(1160505028));
            Assert.That(sdt.Title, Is.EqualTo("Alias1"));
            Assert.That(sdt.Tag, Is.EqualTo("Tag1"));
            Assert.That(sdtNode.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            // Checks getting the ranged structured document tag by Id.
            IStructuredDocumentTag sdt1 = doc.Range.StructuredDocumentTags.GetById(1373447100);
            StructuredDocumentTagRangeStart sdtRangeNode = sdt1.Node as StructuredDocumentTagRangeStart;
            Assert.That(sdt1.IsMultiSection, Is.EqualTo(true));
            Assert.That(sdt1.Id, Is.EqualTo(1373447100));
            Assert.That(sdt1.Title, Is.EqualTo("Alias4"));
            Assert.That(sdt1.Tag, Is.EqualTo("Tag5"));
            Assert.That(sdtRangeNode.ChildNodes.Count, Is.EqualTo(3));
            // Checks getting the sdt by the tag. Normal.
            sdt = doc.Range.StructuredDocumentTags.GetByTag("Tag2");
            Assert.That(sdt.Id, Is.EqualTo(1491995098));
            // Checks getting the sdt by the title. Non-unique title.
            sdt = doc.Range.StructuredDocumentTags.GetByTitle("Alias4");
            Assert.That(sdt1.Title, Is.EqualTo(sdt.Title));
            Assert.That(sdt1.Id, IsNot.EqualTo(sdt.Id));
            Assert.That(sdt.Id, Is.EqualTo(515359460));
            // Checks getting the sdt by the title. Missing title.
            sdt = doc.Range.StructuredDocumentTags.GetByTitle("Alias6");
            Assert.That(sdt, Is.Null);
        }


        /// <summary>
        /// Relates to WORDSNET-23301
        /// Tests the structured document tag collection.
        /// </summary>
        [Test]
        public void TestStructuredDocumentTagsEnumerator()
        {
            List<int> sdtListId = new List<int> { 1160505028, 1491995098, 284420510, 515359460, 1373447100 };

            Document doc = TestUtil.Open(@"Model\Markup\Test23301.docx");

            Assert.That(doc.Range.StructuredDocumentTags.Count, Is.EqualTo(5));
            foreach (IStructuredDocumentTag sdt in doc.Range.StructuredDocumentTags)
            {
                // Checks IStructuredDocumentTag interface.
                if (sdt.IsMultiSection)
                {
                    StructuredDocumentTagRangeStart start = (StructuredDocumentTagRangeStart)sdt.Node;
                    Assert.That(start.Color, Is.EqualTo(sdt.Color));
                    Assert.That(start.Title, Is.EqualTo(sdt.Title));
                    Assert.That(start.Id, Is.EqualTo(sdt.Id));
                    Assert.That(start.IsShowingPlaceholderText, Is.EqualTo(sdt.IsShowingPlaceholderText));
                    Assert.That(start.Level, Is.EqualTo(sdt.Level));
                    Assert.That(start.LockContentControl, Is.EqualTo(sdt.LockContentControl));
                    Assert.That(start.LockContents, Is.EqualTo(sdt.LockContents));
                    Assert.That(start.Placeholder, Is.EqualTo(sdt.Placeholder));
                    Assert.That(start.PlaceholderName, Is.EqualTo(sdt.PlaceholderName));
                    Assert.That(start.SdtType, Is.EqualTo(sdt.SdtType));
                    Assert.That(start.Tag, Is.EqualTo(sdt.Tag));
                    Assert.That(start.WordOpenXML, Is.EqualTo(sdt.WordOpenXML));
                    Assert.That(start.XmlMapping, Is.EqualTo(sdt.XmlMapping));
                }
                else
                {
                    StructuredDocumentTag tag = (StructuredDocumentTag)sdt.Node;
                    Assert.That(tag.Color, Is.EqualTo(sdt.Color));
                    Assert.That(tag.Title, Is.EqualTo(sdt.Title));
                    Assert.That(tag.Id, Is.EqualTo(sdt.Id));
                    Assert.That(tag.IsShowingPlaceholderText, Is.EqualTo(sdt.IsShowingPlaceholderText));
                    Assert.That(tag.Level, Is.EqualTo(sdt.Level));
                    Assert.That(tag.LockContentControl, Is.EqualTo(sdt.LockContentControl));
                    Assert.That(tag.LockContents, Is.EqualTo(sdt.LockContents));
                    Assert.That(tag.Placeholder, Is.EqualTo(sdt.Placeholder));
                    Assert.That(tag.PlaceholderName, Is.EqualTo(sdt.PlaceholderName));
                    Assert.That(tag.SdtType, Is.EqualTo(sdt.SdtType));
                    Assert.That(tag.Tag, Is.EqualTo(sdt.Tag));
                    Assert.That(tag.WordOpenXML, Is.EqualTo(sdt.WordOpenXML));
                    Assert.That(tag.XmlMapping, Is.EqualTo(sdt.XmlMapping));
                }
                // Checks iteration of all structured document tags.
                sdtListId[sdtListId.IndexOf(sdt.Id)] = 0;
            }

            // Checks removing.
            doc.Range.StructuredDocumentTags.Remove(284420510);
            Assert.That(doc.Range.StructuredDocumentTags.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// WORDSNET-23774 Rework StructuredDocumentTagCollection to be indexed by index.
        /// </summary>
        [Test]
        public void TestStructuredDocumentTagsIndexer()
        {
            List<int> sdtListId = new List<int> { 1160505028, 1491995098, 284420510, 515359460, 1373447100 };

            Document doc = TestUtil.Open(@"Model\Markup\Test23301.docx");
            for (int i = 0; i < doc.Range.StructuredDocumentTags.Count; i++)
                Assert.That(doc.Range.StructuredDocumentTags[i].Id, Is.EqualTo(sdtListId[i]));

            Assert.That(doc.Range.StructuredDocumentTags.IndexOf(sdtListId[2]), Is.EqualTo(2));
            doc.Range.StructuredDocumentTags.RemoveAt(2);
            Assert.That(doc.Range.StructuredDocumentTags.IndexOf(sdtListId[2]), Is.EqualTo(-1));
        }

        /// <summary>
        /// WORDSNET-23826 Aspose.Words produces corrupted DOCX document.
        /// MS Word throws an error when opening a document containing an SDT that ends with a table, all the cells of
        /// the last row of which are merged vertically. Appending an empty paragraph has been implemented for this case.
        /// </summary>
        [Test]
        public void Test23826()
        {
            const string fileName = @"Model\Markup\Test23826.docx";
            Document doc = TestUtil.Open(fileName);
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            doc.FirstSection.Body.FirstParagraph.Remove();
            Assert.That(sdt.LastChild.NodeType, Is.EqualTo(NodeType.Table));

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);
            Assert.That(sdt.LastChild.NodeType, Is.EqualTo(NodeType.Paragraph));

            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// WORDSNET-24185 NullReferenceException is thrown upon getting ChildNodes from imported StructuredDocumentTagRangeStart.
        /// Added resilience in RangeEnd getter.
        /// </summary>
        [Test]
        public void Test24185()
        {
            Document srcDoc = TestUtil.Open(@"Model\Markup\Test24185.docx");
            Document dstDoc = TestUtil.Open(@"Model\Markup\Test24185.docx");
            dstDoc.Sections.Clear();

            Section importedSection = (Section)dstDoc.ImportNode(srcDoc.FirstSection, true);
            StructuredDocumentTagRangeStart sdtRangeStart = (StructuredDocumentTagRangeStart)importedSection.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);

            // Exception occurs while getting ChildNodes.
            Assert.That(sdtRangeStart.ChildNodes, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-24144 Allow creating SDT of Group type on Row level.
        /// </summary>
        [Test]
        public void Test24144()
        {
            // A document similar to that provided by the customer is generated.

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();

            StructuredDocumentTag groupSdt = new StructuredDocumentTag(doc, SdtType.Group, MarkupLevel.Row);
            table.AppendChild(groupSdt);

            groupSdt.IsShowingPlaceholderText = false;
            groupSdt.LockContents = true;
            groupSdt.RemoveAllChildren();

            Row row = new Row(doc);
            groupSdt.AppendChild(row);

            Cell cell = new Cell(doc);
            row.AppendChild(cell);

            builder.EndTable();

            cell.EnsureMinimum();
            builder.MoveTo(cell.LastParagraph);

            builder.Write("Added Content Inside a Table and locked using Group Type");

            builder.MoveTo(table.NextSibling);
            builder.Write("Text after the table.");

            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test24144.docx", null, true);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.SdtType, Is.EqualTo(SdtType.Group));
            Assert.That(sdt.Level, Is.EqualTo(MarkupLevel.Row));
        }

        /// <summary>
        /// WORDSNET-24458 Creating StructuredDocumentTag of SdtType.Citation type
        /// </summary>
        [Test]
        public void Test24458()
        {
            // A document similar to that provided by the customer is generated.

            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.Citation, MarkupLevel.Inline);
            doc.FirstSection.Body.FirstParagraph.AppendChild(sdt);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(0, -1);
            builder.InsertField(@"CITATION Dem22 \l 1033 ", "(Demo Author, 2022)");

            sdt.InsertBefore(sdt.NextSibling, null, null);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Markup\Test24458.docx");
        }

        /// <summary>
        /// WORDSNET-24793 SDT dropdown list item with empty value is lost after open/save document.
        /// Seems that empty list item value is valid for MS Word.
        /// </summary>
        [Test]
        public void Test24793()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test24793", UnifiedScenario.Docx2DocxNoGold);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags[1].Node;
            Assert.That(sdt.ListItems[0].DisplayText, Is.EqualTo("Choose an item."));
            Assert.That(sdt.ListItems[0].Value, Is.Null);
        }



        /// <summary>
        /// WORDSNET-25482 Placeholder text lost after saving
        /// SDT placeholder reference should be updated even for SDT marked for deletion.
        /// </summary>
        [Test]
        public void Test25482()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test25482", UnifiedScenario.Docx2DocxNoGold);

            // Check original placeholder is preserved.
            BuildingBlockCollection buildingBlocks = doc.GlossaryDocument.BuildingBlocks;
            Assert.That(buildingBlocks.Count, Is.EqualTo(1));

            BuildingBlock placeholder = buildingBlocks[0];
            Assert.That(placeholder.Name, Is.EqualTo("DefaultPlaceholder_-1854013440"));
            Assert.That(placeholder.Guid.ToString(), Is.EqualTo("3b9ca7f6-fcfd-44df-8a22-92ac373f2c3f"));
            Assert.That(placeholder.GetText(), Is.EqualTo("Click or tap here to enter text.\f"));
        }


        /// <summary>
        /// Simplified test for WORDSNET-26027.
        /// </summary>
        [Test]
        public void Test26027A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26027A.docx");
            Paragraph para = doc.FirstSection.Body.Tables[0].Rows[1].FirstCell.LastParagraph;

            Assert.That(para.GetText(), Is.EqualTo("|Test format and Size libero, sit amet commodo magna eros quis urna|.\u0007"));

            Assert.That(para.ParaPr[ParaAttr.Istd], Is.Null);
            Assert.That(para.ParaPr[ParaAttr.SpaceBefore], Is.Null);
            Assert.That(para.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(250));
            Assert.That(para.ParaPr[ParaAttr.LineSpacing], Is.EqualTo(new LineSpacing(300, LineSpacingRule.AtLeast)));

            Assert.That(para.ParagraphBreakRunPr[FontAttr.Istd], Is.EqualTo(0x29));
            Assert.That(para.ParagraphBreakRunPr[FontAttr.SizeBi], Is.EqualTo(22));
        }



        /// <summary>
        /// WORDSNET-26434 StructuredDocumentTagRangeStart class Appearance property.
        /// Appearance public property has been added to StructuredDocumentTagRangeStart.
        /// </summary>
        [Test]
        public void Test26434()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26434.docx");

            StructuredDocumentTagRangeStart start1 = doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true) as StructuredDocumentTagRangeStart;
            StructuredDocumentTagRangeStart start2 = doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 1, true) as StructuredDocumentTagRangeStart;
            StructuredDocumentTagRangeStart start3 = doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 2, true) as StructuredDocumentTagRangeStart;

            Assert.That(start1.Appearance, Is.EqualTo(SdtAppearance.BoundingBox));
            Assert.That(start2.Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(start3.Appearance, Is.EqualTo(SdtAppearance.Hidden));

            start1.Appearance = SdtAppearance.Tags;
            start2.Appearance = SdtAppearance.Hidden;
            start3.Appearance = SdtAppearance.BoundingBox;

            Assert.That(start1.Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(start2.Appearance, Is.EqualTo(SdtAppearance.Hidden));
            Assert.That(start3.Appearance, Is.EqualTo(SdtAppearance.BoundingBox));
        }

        /// <summary>
        /// Relates to WORDSNET-26434.
        /// Appearance public property has been added to IStructuredDocumentTag.
        /// </summary>
        [Test]
        public void Test26434A()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test26434.docx");

            IStructuredDocumentTag std1 = doc.Range.StructuredDocumentTags[0];
            IStructuredDocumentTag std2 = doc.Range.StructuredDocumentTags[1];
            IStructuredDocumentTag std3 = doc.Range.StructuredDocumentTags[2];
            IStructuredDocumentTag std4 = doc.Range.StructuredDocumentTags[3];

            Assert.That(std1.Appearance, Is.EqualTo(SdtAppearance.BoundingBox));
            Assert.That(std2.Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(std3.Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(std4.Appearance, Is.EqualTo(SdtAppearance.Hidden));

            std1.Appearance = SdtAppearance.Tags;
            std2.Appearance = SdtAppearance.Hidden;
            std3.Appearance = SdtAppearance.Hidden;
            std4.Appearance = SdtAppearance.BoundingBox;

            Assert.That(std1.Appearance, Is.EqualTo(SdtAppearance.Tags));
            Assert.That(std2.Appearance, Is.EqualTo(SdtAppearance.Hidden));
            Assert.That(std3.Appearance, Is.EqualTo(SdtAppearance.Hidden));
            Assert.That(std4.Appearance, Is.EqualTo(SdtAppearance.BoundingBox));
        }



        /// <summary>
        /// Relates to WORDSNET-27292
        /// Checks that new IDs are generated for cloned SDT range nodes during document validation.
        /// </summary>
        [Test]
        public void TestCloningSdtRanges()
        {
            Document doc = TestUtil.Open(@"Reporting\Test27292.docx");

            const int sdtId = -2086217063;

            StructuredDocumentTagRangeStart start =
                (StructuredDocumentTagRangeStart)doc.GetChild(NodeType.StructuredDocumentTagRangeStart, 0, true);
            Assert.That(start.Id, Is.EqualTo(sdtId));

            StructuredDocumentTagRangeEnd end =
                (StructuredDocumentTagRangeEnd)doc.GetChild(NodeType.StructuredDocumentTagRangeEnd, 0, true);
            Assert.That(end.Id, Is.EqualTo(sdtId));

            StructuredDocumentTagRangeStart start2 = (StructuredDocumentTagRangeStart)start.Clone(false);
            Assert.That(start2.Id, Is.EqualTo(sdtId));
            start.ParentNode.InsertAfter(start2, start);

            StructuredDocumentTagRangeStart start3 = (StructuredDocumentTagRangeStart)start.Clone(false);
            Assert.That(start3.Id, Is.EqualTo(sdtId));
            start.ParentNode.InsertAfter(start3, start2);

            StructuredDocumentTagRangeEnd end2 = (StructuredDocumentTagRangeEnd)end.Clone(false);
            Assert.That(end2.Id, Is.EqualTo(sdtId));
            end.ParentNode.InsertBefore(end2, end);

            StructuredDocumentTagRangeEnd end3 = (StructuredDocumentTagRangeEnd)end.Clone(false);
            Assert.That(end3.Id, Is.EqualTo(sdtId));
            end.ParentNode.InsertBefore(end3, end2);

            StructuredDocumentTagRangeStart nonClosedStart = (StructuredDocumentTagRangeStart)start.Clone(false);
            Assert.That(nonClosedStart.Id, Is.EqualTo(sdtId));
            end.ParentNode.InsertAfter(nonClosedStart, end);

            StructuredDocumentTagRangeEnd nonOpenedEnd = (StructuredDocumentTagRangeEnd)end.Clone(false);
            Assert.That(nonOpenedEnd.Id, Is.EqualTo(sdtId));
            start.ParentNode.InsertBefore(nonOpenedEnd, start);

            // Validator re-generates duplicate SDT range IDs and removes invalid start/end nodes.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Assert.That(end.Id, Is.EqualTo(start.Id));

            Assert.That(start2.Id, IsNot.EqualTo(start.Id));
            Assert.That(end2.Id, Is.EqualTo(start2.Id));

            Assert.That(start3.Id, IsNot.EqualTo(start.Id));
            Assert.That(start3.Id, IsNot.EqualTo(start2.Id));
            Assert.That(end3.Id, Is.EqualTo(start3.Id));

            // Check that the invalid start and end nodes are removed.
            Assert.That(nonClosedStart.ParentNode, Is.Null);
            Assert.That(nonOpenedEnd.ParentNode, Is.Null);
        }


        /// <summary>
        /// WORDSNET-27816 NullReferenceException is thrown upon removing nodes from SDT.
        /// Tests clearing the SDT ranged content which leaves an empty Body.
        /// </summary>
        [Test]
        public void Test27816()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test27816.docx");

            StructuredDocumentTagCollection sdts = doc.Range.StructuredDocumentTags;
            NodeCollection coll = sdts[0].GetChildNodes(NodeType.Any, false);

            Assert.That(sdts.Count, Is.EqualTo(3));
            Assert.That(coll.Count, Is.EqualTo(5));

            // Additionally NullReferenceException is being checked here.
            coll.Clear();

            Assert.That(coll.Count, Is.EqualTo(0));
            // Checks that only ranged SDT content has been removed.
            Assert.That(sdts.Count, Is.EqualTo(3));
        }


        private Paragraph FirstParagraph(NodeCollection paragraphs)
        {
            foreach (Paragraph firstParagraph in paragraphs)
                 return firstParagraph;
            return null;
        }

        /// <summary>
        /// WORDSNET-28344 Track changes are lost in dropdown content controls mapped to Custom XML when saving DOCX
        /// Do not update SDT content if mapped content is the same as current SDT content for Dropdown control.
        /// </summary>
        [Test]
        public void Test28344()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test28344", UnifiedScenario.Docx2DocxNoGold);

            Assert.That(doc.Revisions.Groups.Count, Is.EqualTo(1));
            Assert.That(doc.Revisions.Groups[0].Text, Is.EqualTo("sample 2"));
        }


        /// <summary>
        /// WORDSNET-28998 FileCorruptedException is thrown upon loading '.docx' document.
        /// Resilience paragraph is inserted incorrectly during empty SDT resolution.
        /// </summary>
        [Test]
        public void Test28998()
        {
            Document doc = TestUtil.Open(@"Model\Markup\Test28998.docx");
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.Range.StructuredDocumentTags.GetById(539474896);
            Assert.That(sdt.GetText(), Is.EqualTo("\r"));
        }

        /// <summary>
        /// Core body of <see cref="TestPlaceholderStyle"/>
        /// </summary>
        private static void VerifyStyleProperties(DocumentBase doc, Run sdtDefaultRun)
        {
            Assert.That(sdtDefaultRun.RunPr.ContainsKey(FontAttr.Istd), Is.True);
            Style sdtDefaultRunStyle = doc.Styles.GetByIstd((int)sdtDefaultRun.RunPr[FontAttr.Istd], false);
            Assert.That(sdtDefaultRunStyle.BasedOnIstd, Is.EqualTo(StyleIndex.DefaultParagraphFont));
            Assert.That(sdtDefaultRunStyle.Name, Is.EqualTo("Placeholder Text"));
        }

        private static Document CreateDocWithCustomPlaceholder()
        {
            Document srcDoc = new Document();
            StructuredDocumentTag srcTag = new StructuredDocumentTag(srcDoc, SdtType.RichText, MarkupLevel.Block);
            srcDoc.FirstSection.Body.AppendChild(srcTag);
            srcTag.PlaceholderName = CreateCustomPlaceholder(srcDoc);
            Assert.That(srcTag.Placeholder.Name, Is.EqualTo(srcTag.PlaceholderName));
            Assert.That(srcTag.Placeholder.GetText(), Is.EqualTo("Enter your name here.\f"));
            ((Run)((Paragraph)srcTag.GetChildNodes(NodeType.Any, false)[0]).GetChildNodes(NodeType.Any, false)[0]).Text = "Enter your name here.";
            return srcDoc;
        }

        /// <summary>
        /// This might be a good candidate to move into BuildingBlock class.
        /// </summary>
        private static BuildingBlock CreateAndInsertBuildingBlock(GlossaryDocument dstDocument)
        {
            BuildingBlock block = new BuildingBlock(dstDocument);
            dstDocument.AppendChild(block);

            Section section = new Section(dstDocument);
            block.AppendChild(section);

            Body b = new Body(dstDocument);
            section.AppendChild(b);


            return block;
        }

        /// <summary>
        /// This actually is not very simple use case, so it should either become a public function, or a text in learning examples.
        /// </summary>
        private static string CreateCustomPlaceholder(Document srcDoc)
        {
            if (srcDoc.GlossaryDocument == null)
                srcDoc.GlossaryDocument = new GlossaryDocument();
            BuildingBlock block = CreateAndInsertBuildingBlock(srcDoc.GlossaryDocument);
            block.Category = SdtPlaceholderManager.BuildingBlockCategory;
            block.Gallery = BuildingBlockGallery.StructuredDocumentTagPlaceholderText;
            block.Type = BuildingBlockType.StructuredDocumentTagPlaceholderText;
            block.Behavior = BuildingBlockBehavior.Content;
            block.Guid = RandomUtil.NewGuid(1234);
            block.Name = "my fancy block";
            Paragraph para = new Paragraph(srcDoc.GlossaryDocument);
            block.FirstSection.Body.AppendChild(para);
            Run r = new Run(srcDoc.GlossaryDocument, "Enter your name here.");

            Style style = srcDoc.GlossaryDocument.Styles.GetByName("Placeholder Text", true);
            r.RunPr.SetAttr(FontAttr.Istd, style.Istd);
            para.AppendChild(r);

            return block.Name;
        }

        private static void CheckImportedPlaceholder(StructuredDocumentTag srcTag, StructuredDocumentTag dstTag)
        {
            Assert.That(dstTag.Placeholder, IsNot.Null());
            Assert.That(dstTag.Placeholder, IsNot.SameAs(srcTag.Placeholder));
            Assert.That(dstTag.Placeholder.Name, Is.EqualTo(dstTag.PlaceholderName));
        }

        /// <summary>
        /// Checks sdt. Helper for TestJira11088().
        /// </summary>
        private static void CheckSdt(StructuredDocumentTag sdt, MarkupLevel expectedLevel, string expectedText)
        {
            Assert.That(sdt.Level, Is.EqualTo(expectedLevel));
            Assert.That(sdt.GetText().StartsWith(expectedText), Is.True);
        }

        /// <summary>
        /// Checks the color properties of a structured document tag.
        /// </summary>
        private static void CheckColor(StructuredDocumentTag sdt, string expectedColor, string expectedThemeColor,
            string expectedThemeShade, string expectedThemeTint)
        {
            Assert.That(NrxXmlUtil.ColorToXml(sdt.BaseColor), Is.EqualTo(expectedColor), "Color is wrong.");
            Assert.That(sdt.ThemeColor, Is.EqualTo(expectedThemeColor), "Theme color is wrong.");
            Assert.That(sdt.ThemeShade, Is.EqualTo(expectedThemeShade), "Theme shade is wrong.");
            Assert.That(sdt.ThemeTint, Is.EqualTo(expectedThemeTint), "Theme tint is wrong.");
        }

        /// <summary>
        /// Escapes text removing traces of XML offending characters that could be wrongfully interpreted as markup.
        /// </summary>
        private static string EscapeXmlText(string text)
        {
            string escapedText = text.Replace("&", "&amp;");
            escapedText = escapedText.Replace("'", "&apos;");
            escapedText = escapedText.Replace("\"", "&quot;");
            escapedText = escapedText.Replace(">", "&gt;");
            return escapedText.Replace("<", "&lt;");
        }
    }
}
