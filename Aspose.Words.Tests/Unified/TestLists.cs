// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;
using List = Aspose.Words.Lists.List;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for lists.
    /// </summary>
    [TestFixture]
    public class TestLists : UnifiedTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// Tests how lists are copied inside one document.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCopyInSameDoc(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestListCopyNumber", lf, sf);

            //Copy using all available methods.
            doc.Sections[0].PrependContent(doc.Sections[0]);
            doc.Sections[0].AppendContent(doc.Sections[0]);
            doc.AppendChild(doc.Sections[0].Clone());

            TestUtil.SaveOpen(doc, @"Model\List\TestListCopyNumber Modified", lf, sf);

            // Copying list within the same document does not create new lists.
            // This allows to achieve the same effect as in Microsoft Word.
            // If you select a paragraph formatted with a list number and copy it anywhere
            // in the document, it will continue numbering, meaning it refers to the same list.
            Assert.That(doc.Lists.Count, Is.EqualTo(1));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(1));

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(((Paragraph)paras[0]).ParagraphFormat.ListId, Is.EqualTo(1));    //First prepended item
            Assert.That(((Paragraph)paras[2]).ParagraphFormat.ListId, Is.EqualTo(1));    //First original item
            Assert.That(((Paragraph)paras[4]).ParagraphFormat.ListId, Is.EqualTo(1));    //First appended item
            Assert.That(((Paragraph)paras[6]).ParagraphFormat.ListId, Is.EqualTo(1));    //First item in the added section
        }

        /// <summary>
        /// Tests how lists are copied between different documents.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCopyInDifferentDoc(LoadFormat lf, SaveFormat sf)
        {
            Document dstDoc = new Document();

            Document srcDoc1 = TestUtil.OpenSaveOpen(@"Model\List\TestListCopyNumber", lf, sf);
            //Copy using all available methods
            dstDoc.Sections[0].PrependContent(srcDoc1.Sections[0]);
            dstDoc.Sections[0].AppendContent(srcDoc1.Sections[0]);
            dstDoc.AppendChild(dstDoc.ImportNode(srcDoc1.Sections[0], true));

            Document srcDoc2 = TestUtil.OpenSaveOpen(@"Model\List\TestListCopyBullet", lf, sf);
            //Throw in some extra from yet another document.
            dstDoc.AppendChild(dstDoc.ImportNode(srcDoc2.Sections[0], true));

            TestUtil.SaveOpen(dstDoc, @"Model\List\TestListCopyInDifferentDoc", lf, sf);

            Assert.That(dstDoc.Lists.Count, Is.EqualTo(2));
            Assert.That(dstDoc.Lists.ListDefCount, Is.EqualTo(2));

            // It is expected that first imported list was reused.
            NodeList paras = dstDoc.SelectNodes("//Paragraph");
            Assert.That(((Paragraph)paras[0]).ParagraphFormat.ListId, Is.EqualTo(1));    //First prepended item
            //There is one empty para break between first two lists - it is the original empty section para break.
            Assert.That(((Paragraph)paras[3]).ParagraphFormat.ListId, Is.EqualTo(1));    //First appended item
            Assert.That(((Paragraph)paras[5]).ParagraphFormat.ListId, Is.EqualTo(1));    //First copied item
            Assert.That(((Paragraph)paras[7]).ParagraphFormat.ListId, Is.EqualTo(2));    //List from the second document.
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestImportWithLinkedParagraphStyles(LoadFormat lf, SaveFormat sf)
        {
            Document srcDoc = TestUtil.OpenSaveOpen(@"Model\List\TestImportWithLinkedParagraphStyles", lf, sf);

            // Check the original document is as expected.
            Assert.That(srcDoc.Lists.Count, Is.EqualTo(1));
            Assert.That(srcDoc.Lists.ListDefCount, Is.EqualTo(1));
            List srcList = srcDoc.Lists[0];
            Assert.That(srcList.ListLevels[0].LinkedStyle.Name, Is.EqualTo("MyStyle1"));
            Assert.That(srcList.ListLevels[1].LinkedStyle.Name, Is.EqualTo("MyStyle2"));

            Document dstDoc = new Document();
            // Import the list into the destination document.
            List dstList = dstDoc.Lists.AddCopy(srcList);
            Assert.That(dstDoc.Lists.Count, Is.EqualTo(1));
            Assert.That(srcDoc.Lists.ListDefCount, Is.EqualTo(1));

            // Check the paragraph styles referenced by the list were imported too.
            Assert.That(dstDoc.Styles["MyStyle1"], IsNot.Null());
            Assert.That(dstDoc.Styles["MyStyle2"], IsNot.Null());
            Assert.That(dstList.ListLevels[0].LinkedStyle.Name, Is.EqualTo("MyStyle1"));
            Assert.That(dstList.ListLevels[1].LinkedStyle.Name, Is.EqualTo("MyStyle2"));
        }

        [Test]
        public void TestImportWithNodeImporter()
        {
            Document srcDoc = new Document();
            DocumentBuilder builder = new DocumentBuilder(srcDoc);
            builder.ListFormat.ApplyNumberDefault();
            builder.Writeln("Line 1");
            builder.Writeln("Line 2");

            Document dstDoc = new Document();
            dstDoc.RemoveAllChildren();

            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.UseDestinationStyles);

            // Import and append a section twice.
            // Because we are importing using the same importer (same import context),
            // only one list is created in the destination document.
            dstDoc.AppendChild(importer.ImportNode(srcDoc.FirstSection, true));
            dstDoc.AppendChild(importer.ImportNode(srcDoc.FirstSection, true));

            Assert.That(dstDoc.Lists.Count, Is.EqualTo(1));

            TestUtil.Save(dstDoc, @"Model\List\TestImportWithNodeImporter.docx", null, false);   // FOSS: was .doc
        }

        /// <summary>
        /// The file had multiple sections and numbered list going across the sections.
        /// Deleting one section created a document with list which did not automatically
        /// renumber in word and when opening list formatting properties caused Word crashing.
        ///
        /// I'm not exactly sure what fixed it, so opening the file in MS Word and visually checking
        /// list numbering is continious and going into list formatting properties does not crash
        /// is a good idea.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeleteSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestListDeleteSection", lf, sf);
            doc.Sections[1].Range.Delete();
            doc = TestUtil.SaveOpen(doc, @"Model\List\TestListDeleteSection Modified", lf, sf);

            //Just a simple check that all numbered paragraphs refer to the same list.
            //Not sure if it is actually enough to make sure the problem is fixed.
            Assert.That(doc.Lists.Count, Is.EqualTo(1));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(1));

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(((Paragraph)paras[0]).ParagraphFormat.ListId, Is.EqualTo(1));
            Assert.That(((Paragraph)paras[3]).ParagraphFormat.ListId, Is.EqualTo(1));
            Assert.That(((Paragraph)paras[6]).ParagraphFormat.ListId, Is.EqualTo(1));
        }


        /// <summary>
        /// Test creation of a default bulleted list.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSimpleBulleted(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("List heading paragraph.");
            //This starts a bulleted list.
            builder.ListFormat.ApplyBulletDefault();
            builder.Writeln("List item 1.");
            builder.Writeln("List item 2.");
            //Switches to second level in the list.
            builder.ListFormat.ListIndent();
            builder.Writeln("List item 1.1");
            //Returns to the first level.
            builder.ListFormat.ListOutdent();
            builder.Writeln("List item 3.");
            //Stop the bulleted list.
            builder.ListFormat.RemoveNumbers();
            builder.Writeln("Normal text.");
            doc = TestUtil.SaveOpen(doc, @"Model\List\TestSimpleBulleted", lf, sf);

            //At least a few integrity checks.
            Assert.That(doc.Lists.Count, Is.EqualTo(1));

            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(1));
            ListDef listDef = doc.Lists.GetListDefByIndex(0);
            Assert.That(listDef.Levels.Count, Is.EqualTo(9));

            //First list item
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(para.ListFormat.ListId, Is.EqualTo(1));
            Assert.That(para.ListFormat.ListLevelNumber, Is.EqualTo(0));

            //List item at second level.
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 3, true);
            Assert.That(para.ListFormat.ListId, Is.EqualTo(1));
            Assert.That(para.ListFormat.ListLevelNumber, Is.EqualTo(1));
        }

        /// <summary>
        /// Test creation of a default numbered list.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSimpleNumbered(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("List heading paragraph.");
            builder.ListFormat.ApplyNumberDefault();
            builder.Writeln("List item 1.");
            builder.Writeln("List item 2.");
            builder.ListFormat.ListIndent();
            builder.Writeln("List item 1.1");
            builder.ListFormat.ListOutdent();
            builder.Writeln("List item 3.");
            builder.ListFormat.RemoveNumbers();
            builder.Writeln("Normal text.");
            doc = TestUtil.SaveOpen(doc, @"Model\List\TestSimpleNumbered", lf, sf);

            // At least some basic list property checks.
            Assert.That(doc.Lists.Count, Is.EqualTo(1));

            List list = doc.Lists[0];
            Assert.That(list.ListLevels[0].NumberStyle, Is.EqualTo(NumberStyle.Arabic));
            Assert.That(list.ListLevels[1].NumberStyle, Is.EqualTo(NumberStyle.LowercaseLetter));
            Assert.That(list.ListLevels[2].NumberStyle, Is.EqualTo(NumberStyle.LowercaseRoman));
        }

        /// <summary>
        /// Test that inserting into existing list inserts numbered or bulleted paragraphs.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInsertIntoExistingList(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestInsertIntoExistingList", lf, sf);
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToBookmark("Line2");
            builder.Writeln("Inserted line.");
            doc = TestUtil.SaveOpen(doc, @"Model\List\TestInsertIntoExistingList Modified", lf, sf);

            //The paragraph was set to be numbered.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(para.GetText(), Is.EqualTo("Inserted line.\r"));
            Assert.That(para.ListFormat.ListId, Is.EqualTo(1));
        }

        /// <summary>
        /// A customer complained when a list is created inside a table it does not work well.
        /// The problem was that lists were getting the same lsid assigned to them.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMultipleLists(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ListFormat.ApplyBulletDefault();
            builder.Writeln("List 1.1");
            builder.Writeln("List 1.2");
            builder.ListFormat.RemoveNumbers();

            builder.ListFormat.ApplyBulletDefault();
            builder.Writeln("List 2.1");
            builder.Writeln("List 2.2");
            builder.ListFormat.RemoveNumbers();

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestDocBuilderListMultiple", lf, sf);

            //List id within same lists are the same.
            Assert.That(GetListDefForParagraph(doc, 1), Is.EqualTo(GetListDefForParagraph(doc, 0)));
            Assert.That(GetListDefForParagraph(doc, 3), Is.EqualTo(GetListDefForParagraph(doc, 2)));
            //But they are different between the lists
            Assert.That(GetListDefForParagraph(doc, 0) != GetListDefForParagraph(doc, 2), Is.True);
        }

        private static int GetListDefForParagraph(Document doc, int paragraphIndex)
        {
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, paragraphIndex, true);
            int listId = para.ListFormat.ListId;
            return doc.Lists.GetListByListId(listId).ListDefId;
        }

        /// <summary>
        /// Create a custom list and excercise its basic properties.
        /// </summary>
        [Test]
        public void TestListProperties()
        {
            Document doc = new Document();
            // Check basic Lists properties.
            Assert.That(doc.Lists.Document, Is.EqualTo(doc));
            Assert.That(doc.Lists.Count, Is.EqualTo(0));

            // Create a list and check basic properties.
            List list1 = doc.Lists.Add(ListTemplate.NumberDefault);
            Assert.That(doc.Lists.Count, Is.EqualTo(1));
            Assert.That(doc.Lists[0], Is.EqualTo(list1));
            Assert.That(list1.Document, Is.EqualTo(doc));
            Assert.That(list1.IsMultiLevel, Is.EqualTo(true));

            // Check list has levels.
            ListLevelCollection levels = list1.ListLevels;
            Assert.That(levels.Count, Is.EqualTo(9));

            // Check properties of one level.
            ListLevel level = levels[0];
            Assert.That(level.StartAt, Is.EqualTo(1));
            Assert.That(level.NumberStyle, Is.EqualTo(NumberStyle.Arabic));
            Assert.That(level.Alignment, Is.EqualTo(ListLevelAlignment.Left));
            Assert.That(level.IsLegal, Is.EqualTo(false));
            Assert.That(level.NumberFormat, Is.EqualTo("\x0000."));
            Assert.That(level.RestartAfterLevel, Is.EqualTo(-1));
            Assert.That(level.Font, Is.EqualTo(level.Font));    //Check Font object is created only once.
            Assert.That(level.Font.Bold, Is.EqualTo(false));
            Assert.That(level.Font.Style, Is.EqualTo(doc.Styles["Default Paragraph Font"]));
            Assert.That(level.TabPosition, Is.EqualTo(36d));
            Assert.That(level.NumberPosition, Is.EqualTo(18d));
            Assert.That(level.TextPosition, Is.EqualTo(36d));
            Assert.That(level.LinkedStyle, Is.EqualTo(null));

            // A new list id was generated and assigned to the list.
            // At the moment it is a 1-based index of the list, so we know the value.
            Assert.That(list1.ListId, Is.EqualTo(1));
            Assert.That(doc.Lists.GetListByListId(list1.ListId), Is.EqualTo(list1));

            // Lists with these IDs are not found.
            Assert.That(doc.Lists.GetListByListId(0), Is.EqualTo(null));
            Assert.That(doc.Lists.GetListByListId(2), Is.EqualTo(null));

            // Create one more list.
            List list2 = doc.Lists.Add(ListTemplate.BulletDefault);
            Assert.That(doc.Lists.Count, Is.EqualTo(2));
            Assert.That(doc.Lists[1], Is.EqualTo(list2));
            Assert.That(list2.ListId, Is.EqualTo(2));
            Assert.That(list2.Document, Is.EqualTo(doc));

            // Test enumerator
            int i = 0;
            foreach (List list in doc.Lists)
            {
                Assert.That(list, IsNot.Null());    // This just silences the "list declared by not used warning".
                i++;
            }
            Assert.That(i, Is.EqualTo(2));
        }

        /// <summary>
        /// Test modify properties of a list level.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestModifyList(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder builder = new DocumentBuilder();
            // This creates and applies a default list.
            builder.ListFormat.ApplyNumberDefault();

            // Modify the default list slightly.
            List list = builder.ListFormat.List;

            ListLevel level = list.ListLevels[0];
            level.StartAt = 2;
            level.Font.Color = Color.Red;
            level.NumberStyle = NumberStyle.UppercaseRoman;
            level.NumberFormat = "\x0000.";
            level.LinkedStyle = builder.Document.Styles["Heading 1"];

            builder.Writeln("Item 1");
            builder.Writeln("Item 2");

            builder.ListFormat.RemoveNumbers();
            Document doc = TestUtil.SaveOpen(builder.Document, @"Model\List\TestModifyList", lf, sf);

            // Check at least something that we set in the list formatting.
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.IsListItem, Is.EqualTo(true));

            list = para.ListFormat.List;
            Assert.That(doc.Lists[0], Is.EqualTo(list));

            level = list.ListLevels[0];
            Assert.That(level.StartAt, Is.EqualTo(2));
        }

        /// <summary>
        /// Test how paragraph indents and list text/tab/number positions correlate.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListIndentsAndPositions(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.ListFormat.ApplyNumberDefault();

            // These levels go at default positions.
            builder.ListFormat.ListLevelNumber = 0;
            builder.Writeln("Item 1");
            builder.ListFormat.ListLevelNumber = 1;
            builder.Writeln("Item 2");
            builder.ListFormat.ListLevelNumber = 2;
            builder.Writeln("Item 3");

            // Test setting custom positions works.
            builder.ListFormat.ListLevelNumber = 3;
            builder.ListFormat.ListLevel.NumberPosition = 10;
            builder.ListFormat.ListLevel.TextPosition = 40;
            builder.ListFormat.ListLevel.TabPosition = 55;
            builder.Writeln("Item 4");

            builder.ListFormat.RemoveNumbers();
            Document doc = TestUtil.SaveOpen(builder.Document, @"Model\List\TestListIndentsAndPositions", lf, sf);

            // 1st list level paragraph
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            CheckIndentsAndPositions(para, 36, -18, 18, 36, 36);

            // 2nd list level paragraph
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            CheckIndentsAndPositions(para, 72, -18, 54, 72, 72);

            // 3rd list level paragraph
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            CheckIndentsAndPositions(para, 108, -9, 99, 108, 108);

            // 4th list level paragraph
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 3, true);
            CheckIndentsAndPositions(para, 40, -30, 10, 55, 40);
        }

        private static void CheckIndentsAndPositions(
            Paragraph para,
            double leftIndent,
            double firstLineIndent,
            double numberPosition,
            double tabPosition,
            double textPosition)
        {
            Assert.That(para.ParagraphFormat.LeftIndent, Is.EqualTo(leftIndent));
            Assert.That(para.ParagraphFormat.FirstLineIndent, Is.EqualTo(firstLineIndent));

            ListLevel level = para.ListFormat.ListLevel;
            Assert.That(level.NumberPosition, Is.EqualTo(numberPosition));
            Assert.That(level.TabPosition, Is.EqualTo(tabPosition));
            Assert.That(level.TextPosition, Is.EqualTo(textPosition));
        }

        /// <summary>
        /// Test that we can assign list formatting to a paragraph.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSetList(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            List list = doc.Lists.Add(ListTemplate.BulletDefault);

            // Apply list formatting by settings the List property.
            builder.ListFormat.List = list;
            builder.ListFormat.ListLevelNumber = 0;
            builder.Writeln("Item 1");
            builder.ListFormat.ListLevelNumber = 1;
            builder.Writeln("Item 2");
            // End list formatting.
            builder.ListFormat.List = null;

            doc = TestUtil.SaveOpen(builder.Document, @"Model\List\TestSetList", lf, sf);

            // Check the document that was just created.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.ListFormat.ListLevelNumber, Is.EqualTo(0));
            Assert.That(para.ListFormat.ListId, Is.EqualTo(1));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(para.ListFormat.ListLevelNumber, Is.EqualTo(1));
            Assert.That(para.ListFormat.ListId, Is.EqualTo(1));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            Assert.That(para.ListFormat.ListLevelNumber, Is.EqualTo(0));
            Assert.That(para.ListFormat.ListId, Is.EqualTo(0));
        }

        /// <summary>
        /// Test attempt to set ListFormat.List to a list that belongs to a different document.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The list belongs to a different document.")]
        public void TestSetListWrongDocument()
        {
            DocumentBuilder b1 = new DocumentBuilder();
            DocumentBuilder b2 = new DocumentBuilder();

            b1.ListFormat.ApplyNumberDefault();
            b1.Writeln("Item 1");

            List list = b1.ListFormat.List;

            // The list is from another document, must throw.
            b2.ListFormat.List = list;
        }

        /// <summary>
        /// Set ListFormat.List to a list that is a reference to a list style.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSetListToListStyleReference(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestSetListToListStyle", lf, sf);

            // Check the lists are as we expect them.
            Assert.That(doc.Lists.Count, Is.EqualTo(2));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(2));

            Assert.That(doc.Lists[0].IsListStyleDefinition, Is.EqualTo(true));
            Assert.That(doc.Lists[0].IsListStyleReference, Is.EqualTo(false));

            Assert.That(doc.Lists[1].IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(doc.Lists[1].IsListStyleReference, Is.EqualTo(true));

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            // Makes the paragraph a list item (with a list style applied to it).
            builder.ListFormat.List = doc.Lists[1];
            builder.Write("Item 3");

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestSetListToListStyle Modified", lf, sf);

            // There are still only 2 lists in the document.
            Assert.That(doc.Lists.Count, Is.EqualTo(2));

            // First paragraph that was in the document still referns to the list that references a list style.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.ListFormat.ListId, Is.EqualTo(2));

            // The paragraph that was bulleted also refers to the same list.
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            Assert.That(para.GetText(), Is.EqualTo("Item 3\x000c"));
            Assert.That(para.ListFormat.ListId, Is.EqualTo(2));
        }

        /// <summary>
        /// Test attempt to set ListFormat.List to a list that is a list style definition.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The list is a definition of a list style.")]
        public void TestSetListToListStyleDefinition()
        {
            Document doc = TestUtil.Open(@"Model\List\TestSetListToListStyle ms.docx");   // FOSS: was .doc

            List list = doc.Lists[0];
            Assert.That(list.IsListStyleDefinition, Is.EqualTo(true));

            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);

            // A paragraph cannot be applied a list that is a definition of a list style, this must throw.
            para.ListFormat.List = list;
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestModifyListStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestModifyListStyle", lf, sf);

            // There are two lists and two list definitions.
            Assert.That(doc.Lists.Count, Is.EqualTo(2));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(2));

            Style style = doc.Styles["MyListStyle"];
            // This list is a definition of the list style.
            Assert.That(style.List, Is.EqualTo(doc.Lists[0]));
            Assert.That(style.List.ListLevels[0].Font.Color.ToArgb(), Is.EqualTo(0));

            // Lets modify the formatting of the list style.
            style.List.ListLevels[0].Font.Color = Color.Red;

            // But note that the properties of the list that references the list style do not get modified.
            // The net result is that it looks okay in MS Word 2003, but earlier versions that
            // did not have list styles do not show the formatting changed.
            // I might have to update lists that are references to list styles during just before writing to disk.

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestModifyListStyle Modified", lf, sf);

            style = doc.Styles["MyListStyle"];
            Assert.That(style.List.ListLevels[0].Font.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
        }

        /// <summary>
        /// Create a new list from an existing list.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddCopy(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            List list1 = doc.Lists.Add(ListTemplate.BulletDefault);
            // This makes a copy of a list and its list definition.
            List list2 = doc.Lists.AddCopy(list1);

            builder.ListFormat.List = list1;
            builder.Writeln("Item 1 of List 1");
            builder.ListFormat.List = list2;
            builder.Writeln("Item 1 of List 2");
            builder.ListFormat.RemoveNumbers();

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestAddCopy", lf, sf);

            // Check the document has two lists and two list definitions.
            Assert.That(doc.Lists.Count, Is.EqualTo(2));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(2));
            Assert.That(doc.Lists.GetListDefByIndex(0), Is.EqualTo(doc.Lists[0].ListDef));
            Assert.That(doc.Lists.GetListDefByIndex(1), Is.EqualTo(doc.Lists[1].ListDef));
        }

        /// <summary>
        /// Create a new list from a list that is defined in another document.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddCopyFromOtherDocument(LoadFormat lf, SaveFormat sf)
        {
            Document doc1 = new Document();
            List list1 = doc1.Lists.Add(ListTemplate.BulletDefault);

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ListFormat.ApplyNumberDefault();
            builder.Writeln("Numbered");

            // Make a copy of a list from a different document.
            List list2 = doc.Lists.AddCopy(list1);
            builder.ListFormat.List = list2;
            builder.Writeln("Bulleted");
            builder.ListFormat.RemoveNumbers();

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestAddCopyFromOtherDocument", lf, sf);

            // Check the document has two lists and two list definitions.
            Assert.That(doc.Lists.Count, Is.EqualTo(2));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(2));
        }

        /// <summary>
        /// Create a new list based on a list that references a list style.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddCopyFromListStyleReference(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestAddFromListStyle", lf, sf);

            List list1 = doc.Lists[1];
            Assert.That(list1.IsListStyleReference, Is.EqualTo(true));

            // The newly created list does not reference the original list style in any way.
            List list2 = doc.Lists.AddCopy(list1);
            Assert.That(list2.IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(list2.IsListStyleReference, Is.EqualTo(false));
            Assert.That(list2.Style, Is.EqualTo(null));
        }

        /// <summary>
        /// Create a new list based on a list that defines a list style.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddCopyFromListStyleDefinition(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestAddFromListStyle", lf, sf);

            List list1 = doc.Lists[0];
            Assert.That(list1.IsListStyleDefinition, Is.EqualTo(true));

            // The newly created list does not reference the original list style in any way.
            List list2 = doc.Lists.AddCopy(list1);
            Assert.That(list2.IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(list2.IsListStyleReference, Is.EqualTo(false));
            Assert.That(list2.Style, Is.EqualTo(null));
        }

        /// <summary>
        /// Create a new list that references a list style.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddFromListStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestAddFromListStyle", lf, sf);

            // This creates a list that references a list style.
            List list = doc.Lists.Add(doc.Styles["MyListStyle"]);

            Assert.That(doc.Lists.Count, Is.EqualTo(3));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(3));
            Assert.That(list.ListId, Is.EqualTo(3));
            Assert.That(list.IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(list.IsListStyleReference, Is.EqualTo(true));
        }

        /// <summary>
        /// Attempt to create a list from a non list style.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The style is not a list style.")]
        public void TestAddFromNonListStyle()
        {
            Document doc = TestUtil.Open(@"Model\List\TestAddFromListStyle ms.docx");   // FOSS: was .doc
            // This throws.
            doc.Lists.Add(doc.Styles["Normal"]);
        }

        /// <summary>
        /// Attempt to create a list from a style that belongs to a different document.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The list style belongs to a different document.")]
        public void TestAddFromListStyleWrongDocument()
        {
            Document doc1 = TestUtil.Open(@"Model\List\TestAddFromListStyle ms.docx");   // FOSS: was .doc
            Document doc2 = new Document();
            // This throws.
            doc2.Lists.Add(doc1.Styles["MyListStyle"]);
        }

        /// <summary>
        /// Test that we can create bulleted lists that look like in MS Word.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBulletedTemplates(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.BulletDisk);
            builder.Writeln("Disk");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.BulletCircle);
            builder.Writeln("Circle");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.BulletSquare);
            builder.Writeln("Square");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.BulletDiamonds);
            builder.Writeln("Diamonds");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.BulletArrowHead);
            builder.Writeln("Arrow");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.BulletTick);
            builder.Writeln("Tick");
            builder.ListFormat.RemoveNumbers();

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestBulletedTemplates", lf, sf);
            CheckBulletedTemplate(doc.Lists[0], "Symbol", "\xf0b7");    //disc
            CheckBulletedTemplate(doc.Lists[1], "Courier New", "o");    //circle
            CheckBulletedTemplate(doc.Lists[2], "Wingdings", "\xf0a7");    //square
            CheckBulletedTemplate(doc.Lists[3], "Wingdings", "\xf076");    //diamonds
            CheckBulletedTemplate(doc.Lists[4], "Wingdings", "\xf0d8");    //arrow
            CheckBulletedTemplate(doc.Lists[5], "Wingdings", "\xf0fc");    //tick
        }

        private static void CheckBulletedTemplate(List list, string font, string bullet)
        {
            Assert.That(list.ListLevels[0].Font.Name, Is.EqualTo(font));
            Assert.That(list.ListLevels[0].NumberFormat, Is.EqualTo(bullet));
        }

        /// <summary>
        /// Test that we can create numbered lists that look like lists in MS Word.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNumberedTemplates(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberArabicDot);
            builder.Writeln("1.");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberArabicParenthesis);
            builder.Writeln("1)");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberUppercaseRomanDot);
            builder.Writeln("I.");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberUppercaseLetterDot);
            builder.Writeln("A.");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberLowercaseLetterParenthesis);
            builder.Writeln("a)");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberLowercaseLetterDot);
            builder.Writeln("a.");
            builder.ListFormat.List = doc.Lists.Add(ListTemplate.NumberLowercaseRomanDot);
            builder.Writeln("i.");
            builder.ListFormat.RemoveNumbers();

            TestUtil.SaveOpen(doc, @"Model\List\TestNumberedTemplates", lf, sf);

            // Can't be bothered to programmatically check these.
        }


        private static void ApplyOutlineListToParagraphs(ParagraphCollection paras, List list, int startParaIndex)
        {
            for (int i = 0; i < 9; i++)
            {
                Paragraph para = paras[startParaIndex + i];
                para.ListFormat.List = list;
                para.ListFormat.ListLevelNumber = i;
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestOutlineHeadingTemplates(LoadFormat lf, SaveFormat sf)
        {
            // Verify golds as created by MS Word.
            TestUtil.OpenSaveOpen(@"Model\List\TestOutlineHeadingTemplates", lf, sf);

            // Create new programmatically.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            List list = doc.Lists.Add(ListTemplate.OutlineHeadingsArticleSection);
            AddOutlineHeadingParagraphs(builder, list, "Aspose.Words Outline 1");

            list = doc.Lists.Add(ListTemplate.OutlineHeadingsLegal);
            AddOutlineHeadingParagraphs(builder, list, "Aspose.Words Outline 2");

            builder.InsertBreak(BreakType.PageBreak);

            list = doc.Lists.Add(ListTemplate.OutlineHeadingsNumbers);
            AddOutlineHeadingParagraphs(builder, list, "Aspose.Words Outline 3");

            list = doc.Lists.Add(ListTemplate.OutlineHeadingsChapter);
            AddOutlineHeadingParagraphs(builder, list, "Aspose.Words Outline 4");

            TestUtil.SaveOpen(doc, @"Model\List\TestOutlineHeadingTemplates Modified", lf, sf);
        }

        private static void AddOutlineHeadingParagraphs(DocumentBuilder builder, List list, string title)
        {
            builder.ParagraphFormat.ClearFormatting();
            builder.Writeln(title);

            for (int i = 0; i < 9; i++)
            {
                builder.ListFormat.List = list;
                builder.ListFormat.ListLevelNumber = i;

                string styleName = "Heading " + (i + 1).ToString();
                builder.ParagraphFormat.StyleName = styleName;
                builder.Writeln(styleName);
            }

            builder.ListFormat.RemoveNumbers();
        }




        /// <summary>
        /// Tests picture bullets are loaded and saved in all formats.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPictureBullet(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestPictureBullet", lf, sf);
            Assert.That(doc.Lists.PictureBulletCount, Is.EqualTo(6));

            // This is the list in a textbox.
            ListDef listDef = doc.Lists.GetListDefByIndex(1);
            Assert.That(listDef.Levels[0].PictureBulletId, Is.EqualTo(5));
            Assert.That(listDef.Levels[1].PictureBulletId, Is.EqualTo(-1));
            // It uses an imported WMF picture as a picture bullet.
            Shape shape = doc.Lists.GetPictureBullet(5);
            Assert.That(shape.ImageData.ImageType, Is.EqualTo(ImageType.Wmf));

            // This is a system bookmark used in a DOC file, we don't have it in the model.
            Assert.That(doc.Range.Bookmarks["_PictureBullets"], Is.EqualTo(null));
        }

        /// <summary>
        /// Test cloning a complete document with picture bullets works.
        /// </summary>
        [Test]
        public void TestPictureBulletClone()
        {
            Document srcDoc = TestUtil.Open(@"Model\List\TestPictureBulletClone.docx");
            Document dstDoc = srcDoc.Clone();

            // Check the number of picture bullets was copied correctly.
            Assert.That(srcDoc.Lists.PictureBulletCount, Is.EqualTo(1));
            Assert.That(dstDoc.Lists.PictureBulletCount, Is.EqualTo(1));

            Shape srcPictureBullet = srcDoc.Lists.GetPictureBullet(0);
            Assert.That(srcPictureBullet.Document, Is.EqualTo(srcDoc));

            // Check the picture bullet shape was cloned correctly and belongs to the new document.
            Shape dstPictureBullet = dstDoc.Lists.GetPictureBullet(0);
            Assert.That(dstPictureBullet.Document, Is.EqualTo(dstDoc));

            // Check the image bytes themselves were not duplicated.
            Assert.That(dstPictureBullet.ImageData.ImageBytes, Is.EqualTo(srcPictureBullet.ImageData.ImageBytes));
        }

        [Test]
        public void TestPictureBulletCloneInSameDoc()
        {
            Document doc = TestUtil.Open(@"Model\List\TestPictureBulletClone.docx");

            doc.AppendChild(doc.Sections[0].Clone());
            TestUtil.Save(doc, @"Model\List\TestPictureBulletCloneInSameDoc.docx", null, false);   // FOSS: was .doc

            // Copying list within the same document does not create new lists.
            Assert.That(doc.Lists.Count, Is.EqualTo(1));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(1));
            Assert.That(doc.Lists.PictureBulletCount, Is.EqualTo(1));
        }

        [Test]
        public void TestPictureBulletCloneInDifferentDoc()
        {
            Document srcDoc = TestUtil.Open(@"Model\List\TestPictureBulletClone.docx");
            Document dstDoc = TestUtil.Open(@"Model\List\TestPictureBulletClone.docx");

            dstDoc.AppendChild(dstDoc.ImportNode(srcDoc.FirstSection, true));
            TestUtil.Save(dstDoc, @"Model\List\TestPictureBulletCloneInDifferentDoc.docx");

            // Check that AW mimics MSW and reuses lists and definitions of lists.
            Assert.That(dstDoc.Lists.Count, Is.EqualTo(1));
            Assert.That(dstDoc.Lists.ListDefCount, Is.EqualTo(1));
            Assert.That(dstDoc.Lists.PictureBulletCount, Is.EqualTo(1));

            // This is the original paragraph. It points to the original list.
            Paragraph oldPara = (Paragraph)dstDoc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(oldPara.ListFormat.ListId, Is.EqualTo(1));
            // This is the original list. It points to the original picture bullet.
            List oldList = dstDoc.Lists.GetListByListId(1);
            oldList.ListDef.Levels[0].PictureBulletId = 0;
            // This is the original picture bullet.
            Shape oldShape = dstDoc.Lists.GetPictureBullet(0);

            // This is the imported paragraph. It points to the reused list.
            Paragraph newPara = (Paragraph)dstDoc.GetChild(NodeType.Paragraph, 2, true);
            Assert.That(newPara.ListFormat.ListId, Is.EqualTo(1));
            // This is the reused list. It points to the imported picture bullet.
            List newList = dstDoc.Lists.GetListByListId(1);
            newList.ListDef.Levels[0].PictureBulletId = 1;
            // This is the reused picture bullet.
            Shape newShape = dstDoc.Lists.GetPictureBullet(0);

            // Check the shape was not cloned and was reused from the destination document.
            Assert.That(newShape == oldShape, Is.True);
            Assert.That(newShape.Document, Is.EqualTo(dstDoc));

            // Check that the image bytes were actually duplicated.
            Assert.That(newShape.ImageData.ImageBytes, Is.EqualTo(oldShape.ImageData.ImageBytes));
        }





        /// <summary>
        /// Test for <see cref="ListLabel" /> class.
        /// Checks basic constraints and set/get operations.
        /// </summary>
        [Test]
        public void TestListLabelBasicManip()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.ListFormat.ApplyBulletDefault();
            builder.Writeln("List item 1.");
            builder.Writeln("List item 2.");
            builder.ListFormat.RemoveNumbers();

            NodeCollection paras = doc.GetChildNodes(NodeType.Paragraph, true);
            Debug.Assert((paras != null) && (paras.Count == 3));

            Paragraph item1 = (Paragraph)paras[0];
            Paragraph item2 = (Paragraph)paras[1];

            // Checking basic constraints.
            ListLabel label1 = item1.ListLabel;
            Assert.That(item1.ListLabel, Is.SameAs(label1));                    // ListLabel is lazily inited, should be the same every time is queried.
            Paragraph item1Clone = (Paragraph)item1.Clone(true);
            Assert.That(item1Clone.ListLabel, IsNot.SameAs(label1));            // Should be a different object after paragraph cloning.
            Font label1Font = label1.Font;
            Assert.That(label1.Font, Is.SameAs(label1Font));                    // Font is lazily inited, should be the same every time is queried.

            // Checking how defaults are retrieved.
            Assert.That(label1Font.Name, Is.EqualTo("Symbol"));                 // Default font
            Assert.That(label1Font.Size, Is.EqualTo(12));                       // Default size
            Assert.That(label1Font.Color.ToArgb(), Is.EqualTo(0));              // Default color
            Assert.That(label1Font.Bold, Is.EqualTo(false));                    // Default bold (BoolEx attrs are tricky)
            Assert.That(label1Font.Underline, Is.EqualTo(Underline.None));      // Default underline

            Assert.That(label1.ListLevel.RunPr.Count, Is.EqualTo(2));     // How many direct attributes are there in default list label?

            // Let's manipulate with first one and check how this affects the second.
            label1Font.Size = 14;
            label1Font.Color = Color.Blue;
            label1Font.Bold = true;
            label1Font.Underline = Underline.Single;
            ListLabel label2 = item2.ListLabel;
            Font label2Font = label2.Font;
            Assert.That(label2Font.Size, Is.EqualTo(14));
            Assert.That(label2Font.Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
            Assert.That(label2Font.Bold, Is.EqualTo(true));
            Assert.That(label2Font.Underline, Is.EqualTo(Underline.Single));

            Assert.That(label2.ListLevel.RunPr.Count, Is.EqualTo(6));

            TestUtil.Save(doc, @"Model\List\TestListLabelBasicManip.docx", null, false);   // FOSS: was .doc

            // Clear the direct formatting and check again.
            label1Font.ClearFormatting();

            Assert.That(label2.ListLevel.RunPr.Count, Is.EqualTo(0));           // It's questionable. Maybe we should preserve some attributes (font name).

            // Some checks with complex attrs.
            Shading shd1 = label1Font.Shading;
            Assert.That(label1Font.Shading, Is.SameAs(shd1));                   // Lazily inited, same object.

            Assert.That(label2.ListLevel.RunPr.Count, Is.EqualTo(1));           // Implicitly added to the direct attrs.
            Shading shd2 = label2Font.Shading;
            Assert.That(shd2, Is.SameAs(shd1));                                 // Also same objects because they are stored in one collection.
            shd1.BackgroundPatternColor = DrColor.Green.ToNativeColor();
            Assert.That(shd2.BackgroundPatternColorInternal, Is.EqualTo(DrColor.Green));

            Assert.That(label2.ListLevel.RunPr.Count, Is.EqualTo(1));           // Shading properties are not written to the collection, stored in the Shading object.

            label1Font.Name = "Symbol";                                     // Restore it before saving.
            TestUtil.Save(doc, @"Model\List\TestListLabelBasicManip2.docx", null, false);   // FOSS: was .doc
        }

        /// <summary>
        /// WORDSNET-9984 List label is generated incorrectly for list levels marked with IsLegal.
        /// (Don't forget calling numGen.ProcessListItem on any preceding list items for proper tesing results.)
        /// </summary>
        [Test]
        public void TestListIsLegal()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            string[] prompts = { "A multilevel list with different labels:", "The same but on 2nd level (1st from zero) IsLegal is applied:" };
            string[] labels = { "II.a.", "2.1." };
            for (int i = 0; i < 2; ++i)
            {
                builder.Writeln(prompts[i]);
                builder.ListFormat.ApplyNumberDefault();
                builder.ListFormat.ListLevel.NumberStyle = NumberStyle.UppercaseRoman;
                builder.Writeln("Item 1");
                builder.Writeln("Item 2");
                builder.ListFormat.ListIndent();
                builder.ListFormat.ListLevel.NumberFormat = "\x0000.\x0001.";
                builder.ListFormat.ListLevel.IsLegal = (i != 0);
                ListLabelUpdater.UpdateListLabels(doc);
                Assert.That(builder.CurrentParagraph.ListLabel.LabelString, Is.EqualTo(labels[i]));
                builder.Writeln("Item 2.1");
                builder.Writeln("Item 2.2");
                builder.ListFormat.ListIndent();
                builder.ListFormat.ListLevel.NumberFormat = "\x0000.\x0001.\x0002.";
                builder.Writeln("Item 2.2.1");
                builder.Writeln("Item 2.2.2");
                builder.ListFormat.RemoveNumbers();
            }

            builder.Writeln("Testing all number styles:");
            builder.InsertCell();
            builder.Write("Normal lists");
            builder.InsertCell();
            builder.Write("Lists with IsLegal = true");
            builder.EndRow();
            for (NumberStyle ns = NumberStyle.Arabic; ns <= NumberStyle.None; ns = (ns == NumberStyle.UppercaseRussian) ? NumberStyle.None : (ns + 1))
            {
                string nsName = DocxEnum.NumberStyleToDocx(ns);
                builder.InsertCell();
                builder.ListFormat.ApplyNumberDefault();
                builder.ListFormat.ListLevel.NumberStyle = ns;
                ListLabelUpdater.UpdateListLabels(doc);
                string labelWithoutIsLegal = builder.CurrentParagraph.ListLabel.LabelString;
                builder.Write(nsName);
                builder.InsertCell();
                builder.ListFormat.ApplyNumberDefault();
                builder.ListFormat.ListLevel.NumberStyle = ns;
                builder.ListFormat.ListLevel.IsLegal = true;
                ListLabelUpdater.UpdateListLabels(doc);
                string labelWithIsLegal = builder.CurrentParagraph.ListLabel.LabelString;
                builder.Write(nsName);
                builder.EndRow();

                // Bullet and None are the special cases. But we expect that they are not affected by IsLegal flag.
                bool isRetained = (ns == NumberStyle.Arabic) || (ns == NumberStyle.LeadingZero) || (ns == NumberStyle.Bullet) || (ns == NumberStyle.None);
                string expectedLabel = isRetained ? labelWithoutIsLegal : "1.";
                Assert.That(labelWithIsLegal, Is.EqualTo(expectedLabel));
            }
            builder.EndTable();

            TestUtil.Save(doc, @"Model\List\TestListIsLegal.docx", null, false);   // FOSS: was .doc
        }

        /// <summary>
        /// WORDSNET-4284 Enumeration of multilevel list is exported from WML to DOC not properly.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRestartAfterHigher(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\TestRestartAfterHigher", lf, sf);

            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(2));

            // This is a default outline list.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            List list = para.ListFormat.List;
            Assert.That(list.ListLevels[0].IsRestartAfterLevelCustom, Is.EqualTo(false));
            Assert.That(list.ListLevels[0].RestartAfterLevel, Is.EqualTo(-1));
            Assert.That(list.ListLevels[1].IsRestartAfterLevelCustom, Is.EqualTo(false));
            Assert.That(list.ListLevels[1].RestartAfterLevel, Is.EqualTo(0));
            Assert.That(list.ListLevels[2].IsRestartAfterLevelCustom, Is.EqualTo(false));
            Assert.That(list.ListLevels[2].RestartAfterLevel, Is.EqualTo(1));

            // This is a modified outline list.
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 7, true);
            list = para.ListFormat.List;
            // These are still default values.
            Assert.That(list.ListLevels[0].IsRestartAfterLevelCustom, Is.EqualTo(false));
            Assert.That(list.ListLevels[0].RestartAfterLevel, Is.EqualTo(-1));
            Assert.That(list.ListLevels[1].IsRestartAfterLevelCustom, Is.EqualTo(false));
            Assert.That(list.ListLevels[1].RestartAfterLevel, Is.EqualTo(0));

            // This restart was explicitly switched off. Level 8 does not restart.
            Assert.That(list.ListLevels[7].IsRestartAfterLevelCustom, Is.EqualTo(true));
            Assert.That(list.ListLevels[7].RestartAfterLevel, Is.EqualTo(-1));

            // This restart was explicitly set. Level 9 restart after level 5.
            // RK The actual restart level is lost in RTF. MS Word loses it too.
            Assert.That(list.ListLevels[8].IsRestartAfterLevelCustom, Is.EqualTo(true));
            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
                Assert.That(list.ListLevels[8].RestartAfterLevel, Is.EqualTo(-1));
            else
                Assert.That(list.ListLevels[8].RestartAfterLevel, Is.EqualTo(4));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBulletDefault(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestBulletDefault", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListAlignment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListAlignment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListBulleted(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListBulleted", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListIndents(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListIndents", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListLabelFormatting(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListLabelFormatting", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNasty2(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNasty2", lf, sf);
        }



        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNonTab(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNonTab", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNumberAndBullet(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNumberAndBullet", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNumbered(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNumbered", lf, sf);
        }



        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNumberedNestedWithText(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNumberedNestedWithText", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNumberedRestart(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNumberedRestart", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNumberedRestartAfterBreak(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNumberedRestartAfterBreak", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListNumberStyles(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListNumberStyles", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListTab(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListTab", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListWithSection(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListWithSection", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListWithTable(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListWithTable", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestOutlineDefault(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestOutlineDefault", lf, sf);
        }


        /// <summary>
        /// <para>Defect 18603 Output list labels to various formats.</para>
        /// <para>List labels for lists with the same id but in different document areas
        /// (see enum <see cref="ListLabelUpdater.ListNumberingArea"/>) were not exported correctly, fixed.</para>
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListsInDifferentAreas(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\List\TestListsInDifferentAreas", lf, sf);
        }

        /// <summary>
        /// Verifies import/export of <see cref="ListDef.IsRestartAtEachSection"/>
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListRestartAtEachSection(LoadFormat lf, SaveFormat sf)
        {
            // Not supported for WML format.
            if (sf != SaveFormat.WordML)
            {
                Document doc = TestUtil.Open(@"Model\List\TestRestartEachSection", lf);

                SaveOptions saveOptions = SaveOptions.CreateSaveOptions(sf);

                if (sf == SaveFormat.Docx)
                    saveOptions = OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoStrict);

                doc = TestUtil.SaveOpen(doc, @"Model\List\TestRestartEachSection", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold, saveOptions);

                Assert.That(doc.Lists[0].ListDef.IsRestartAtEachSection, Is.True);
            }
        }

        /// <summary>
        /// Tests how <see cref="ListLabelUpdater"/> works.
        /// </summary>
        [Test]
        public void TestListLabelUpdater()
        {
            Document doc = TestUtil.Open(@"Model\List\TestListLabelUpdater.docx");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            foreach (Paragraph para in paras)
            {
                Assert.That(para.ListLabel.LabelValue, Is.EqualTo(0));
                Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));
            }

            doc.UpdateListLabels();

            Assert.That(paras[0].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[0].ListLabel.LabelString, Is.EqualTo("1."));

            Assert.That(paras[1].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[1].ListLabel.LabelString, Is.EqualTo("a."));

            Assert.That(paras[2].ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo(""));

            Assert.That(paras[3].ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(paras[3].ListLabel.LabelString, Is.EqualTo("2."));

            Assert.That(paras[4].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[4].ListLabel.LabelString, Is.EqualTo("a."));

            Assert.That(paras[5].ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(paras[5].ListLabel.LabelString, Is.EqualTo(""));

            Assert.That(paras[6].ListLabel.LabelValue, Is.EqualTo(3));
            Assert.That(paras[6].ListLabel.LabelString, Is.EqualTo("3."));

            Assert.That(paras[7].ListLabel.LabelValue, Is.EqualTo(4));
            Assert.That(paras[7].ListLabel.LabelString, Is.EqualTo("4."));

            Assert.That(paras[8].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[8].ListLabel.LabelString, Is.EqualTo("a."));

            Assert.That(paras[9].ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(paras[9].ListLabel.LabelString, Is.EqualTo("b."));

            Assert.That(paras[10].ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(paras[10].ListLabel.LabelString, Is.EqualTo(""));

            Assert.That(paras[11].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[11].ListLabel.LabelString, Is.EqualTo("1."));

            Assert.That(paras[12].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[12].ListLabel.LabelString, Is.EqualTo("\xf0b7")); // Bullet sign.

            Assert.That(paras[13].ListLabel.LabelValue, Is.EqualTo(5));
            Assert.That(paras[13].ListLabel.LabelString, Is.EqualTo("5th."));
            Assert.That(paras[13].ListLabel.Font.Name, Is.EqualTo("Arial"));
            Assert.That(paras[13].ListLabel.Font.Bold, Is.True);

            Assert.That(paras[14].ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(paras[14].ListLabel.LabelString, Is.EqualTo("\xf0b7")); // Bullet sign.

            // The paragraph is a list item but only contains a section break and therefore has no list label.
            Assert.That(paras[15].IsListItem, Is.True);
            Assert.That(paras[15].ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(paras[15].ListLabel.LabelString, Is.EqualTo(""));
        }

        /// <summary>
        /// Tests how <see cref="ListLabelUpdater"/> works.
        /// </summary>
        [Test]
        public void TestListNumbersComplex()
        {
            Document doc = TestUtil.Open(@"Model\List\TestListNumbersComplex.docx");

            doc.UpdateListLabels();

            // Test headers/footers.

            ListLabel listLabel = doc.Sections[0].HeadersFooters[HeaderFooterType.HeaderFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(5));
            Assert.That(listLabel.LabelString, Is.EqualTo("5."));

            listLabel = doc.Sections[0].HeadersFooters[HeaderFooterType.FooterFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(6));
            Assert.That(listLabel.LabelString, Is.EqualTo("6."));

            listLabel = doc.Sections[1].HeadersFooters[HeaderFooterType.HeaderFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(9));
            Assert.That(listLabel.LabelString, Is.EqualTo("9."));

            listLabel = doc.Sections[1].HeadersFooters[HeaderFooterType.FooterFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(10));
            Assert.That(listLabel.LabelString, Is.EqualTo("10."));

            listLabel = doc.Sections[2].HeadersFooters[HeaderFooterType.HeaderFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(13));
            Assert.That(listLabel.LabelString, Is.EqualTo("13."));

            listLabel = doc.Sections[2].HeadersFooters[HeaderFooterType.FooterFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(14));
            Assert.That(listLabel.LabelString, Is.EqualTo("14."));

            listLabel = doc.Sections[3].HeadersFooters[HeaderFooterType.HeaderFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(15));
            Assert.That(listLabel.LabelString, Is.EqualTo("15."));

            listLabel = doc.Sections[3].HeadersFooters[HeaderFooterType.FooterFirst].FirstParagraph.ListLabel;
            Assert.That(listLabel.LabelValue, Is.EqualTo(16));
            Assert.That(listLabel.LabelString, Is.EqualTo("16."));

            // Test body paragraphs.

            // Section 1.

            NodeCollection paras = doc.Sections[0].Body.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(paras.Count, Is.EqualTo(4));

            Paragraph para = (Paragraph)paras[0];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));

            para = (Paragraph)paras[1];
            Assert.That(para.ParentNode.NodeType == NodeType.Shape, Is.True);  // The paragraph in a textbox.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));

            para = (Paragraph)paras[2];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("2."));

            para = (Paragraph)paras[3];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));

            // Section 2.

            paras = doc.Sections[1].Body.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(paras.Count, Is.EqualTo(12));

            para = (Paragraph)paras[0];
            Assert.That(para.ParaPr.IsFloating, Is.True); // The text frame.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(3));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("3."));

            para = (Paragraph)paras[1];
            Assert.That(para.ParentNode.NodeType == NodeType.Footnote, Is.True);  // The paragraph in a footnote.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));

            para = (Paragraph)paras[2];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(4));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("4."));

            para = (Paragraph)paras[3];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(5));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("5."));

            para = (Paragraph)paras[4];
            Assert.That(para.ParentNode.NodeType == NodeType.Footnote, Is.True);  // The paragraph in a footnote.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("2."));

            para = (Paragraph)paras[5];
            Assert.That(para.ParentNode.NodeType == NodeType.Cell, Is.True);  // The paragraph in a table cell.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(6));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("6."));

            para = (Paragraph)paras[6];
            Assert.That(para.ParentNode.NodeType == NodeType.Comment, Is.True);  // The paragraph in a comment.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));

            para = (Paragraph)paras[7];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(7));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("7."));

            para = (Paragraph)paras[8];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(8));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("8."));

            para = (Paragraph)paras[9];
            Assert.That(para.ParentNode.NodeType == NodeType.Footnote, Is.True);  // The paragraph in an endnote.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));

            para = (Paragraph)paras[10];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));

            para = (Paragraph)paras[11];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));

            // Section 3.

            paras = doc.Sections[2].Body.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(paras.Count, Is.EqualTo(1));

            para = (Paragraph)paras[0];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));

            // Section 4.
            // WORDSNET-4826 AW does not support section inside markup so SDT node is deleted and content partially moved to next section.

            paras = doc.Sections[3].Body.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(paras.Count, Is.EqualTo(1));

            para = (Paragraph)paras[0];
            Assert.That(para.ParentNode.NodeType == NodeType.StructuredDocumentTag, Is.False);  // SDT node was deleted.
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));

            // Section 5.

            paras = doc.Sections[4].Body.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(paras.Count, Is.EqualTo(2));

            para = (Paragraph)paras[0];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(11));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("11."));

            para = (Paragraph)paras[1];
            Assert.That(para.ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("2."));
        }


        /// <summary>
        /// Tests updating list labels by <see cref="Document.UpdateListLabels"/> after changing of document.
        /// </summary>
        [Test]
        public void TestListNumbersUpdating()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ListFormat.ApplyNumberDefault();
            builder.Writeln("First");
            builder.Writeln("Second");

            NodeCollection paras = doc.Sections[0].Body.GetChildNodes(NodeType.Paragraph, true);

            doc.UpdateListLabels();
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelString, Is.EqualTo("2."));

            ((Paragraph)paras[1]).ListFormat.RemoveNumbers();
            doc.UpdateListLabels();
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelValue, Is.EqualTo(0));
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelString, Is.EqualTo(""));

            ((Paragraph)paras[0]).ListFormat.RemoveNumbers();
            ((Paragraph)paras[1]).ListFormat.ApplyNumberDefault();
            doc.UpdateListLabels();
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelString, Is.EqualTo("1."));

            ((Paragraph)paras[0]).ListFormat.ListId = ((Paragraph)paras[1]).ListFormat.ListId;
            doc.UpdateListLabels();
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelString, Is.EqualTo("2."));

            ((Paragraph)paras[0]).ListFormat.ApplyBulletDefault();
            doc.UpdateListLabels();
            Assert.That(((Paragraph)paras[0]).ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(((Paragraph)paras[0]).ListLabel.LabelString, Is.EqualTo("\xF0B7"));
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(((Paragraph)paras[1]).ListLabel.LabelString, Is.EqualTo("1."));

        }


        /// <summary>
        /// <para>WORDSNET-24241 Restarting of list numbering depending on change
        /// of number of higher level was incorrect, fixed.</para>
        /// <para>Tests small document with custom Restart List After property for various list levels.</para>
        /// </summary>
        [Test]
        public void TestDefect24241()
        {
            Document doc = TestUtil.Open(@"Model\List\TestDefect24241.docx");
            doc.UpdateListLabels();

            string[] labelStrings = {
                    "1.", "i.", "ii.", "1st.", "iii.", "2nd.", "b.",
                    "iv.", "2.", "1st.", "ii.", "3.", "4."
                };

            NodeCollection paras = doc.Sections[0].Body.GetChildNodes(NodeType.Paragraph, true);

            for (int i = 0; i < labelStrings.Length; i++)
                Assert.That(((Paragraph)paras[i]).ListLabel.LabelString, Is.EqualTo(labelStrings[i]), "for i = " + i);
        }

        /// <summary>
        /// WORDSNET-18137 Aspose.Words returns incorrect LeftIndent of paragraph.
        /// Fixed. Turned out to be same earlier fixed 1545, but this one was not fixed in the bottom-up attribute expansion.
        /// The problem was that when a paragraph is explicitly marked as non list item, then MS Word resets its indents
        /// to zero in the attribute expansion algorithm just before expanding the direct formatting.
        /// </summary>
        [Test]
        public void TestDefect18137()
        {
            Document doc = TestUtil.Open(@"Model\List\TestDefect18137.docx");

            // This uses the bottom-up attribute expansion. There was a problem in it.
            Paragraph para = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(para.ParagraphFormat.LeftIndent, Is.EqualTo(0.0));            // This was returned incorrect.
            Assert.That(para.ParagraphFormat.FirstLineIndent, Is.EqualTo(10.8));

            // This uses the top-bottom attribute expansion, it already had a fix for 1545 which as the same problem.
            ParaPr paraPr = para.GetExpandedParaPr(ParaPrExpandFlags.Normal);
            Assert.That(ConvertUtilCore.TwipToPoint(paraPr.LeftIndent), Is.EqualTo(0));
            Assert.That(ConvertUtilCore.TwipToPoint(paraPr.FirstLineIndent), Is.EqualTo(10.8));

            // This paragraph had correct values, just check them.
            para = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.ParagraphFormat.LeftIndent, Is.EqualTo(1.0));
            Assert.That(para.ParagraphFormat.FirstLineIndent, Is.EqualTo(10.8));
        }


        /// <summary>
        /// WORDSNET-5272 Custom footnote marks are rendered as numbers.
        /// Added conversion from ChicagoManualOfStyle to Unicode numbering format.
        /// </summary>
        [Test]
        public void TestJira5272()
        {
            string[] etalons = new string[] {"*", "†", "‡", "§", "**", "††", "‡‡", "§§", "***"};

            const int nSamples = 9;

            for (int number = 1; number <= nSamples; number++)
                Assert.That(NumberConverter.NumberToString(number, NumberStyle.ChicagoManual, true), Is.EqualTo(etalons[number - 1]));
        }


        /// <summary>
        /// WORDSNET-7856 MS Word 2007 incorrectly layouts List at second ListLevel in DOC.
        /// HybridMultiLevel type is changed to MultiLevel when ListFactory creates Number list.
        /// </summary>
        /// <remarks>
        /// AM. HybridMultiLevel list type causes such behavior probably because SttbRgtplc structure which specifies
        /// the bullet/numbering formats for a hybrid bulleted/numbered multi-level list is not processed.
        /// I carefully tested output files including binary DOC and it seems that everything OK.
        /// </remarks>
        [Test]
        public void TestJira7856()
        {
            Document doc = new Document();

            List list = doc.Lists.Add(ListTemplate.NumberArabicDot);

            ListLevel level1 = list.ListLevels[0];
            level1.NumberStyle = NumberStyle.Arabic;
            level1.NumberFormat = "\x0000";

            ListLevel level2 = list.ListLevels[1];
            level2.NumberStyle = NumberStyle.Arabic;
            level2.NumberFormat = "\x0000.\x0001";

            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ListFormat.List = list;
            builder.Writeln("The quick brown fox...");
            builder.Writeln("The quick brown fox...");

            builder.ListFormat.ListIndent();
            builder.Writeln("jumped over the lazy dog.");
            builder.Writeln("jumped over the lazy dog.");

            builder.ListFormat.ListOutdent();
            builder.Writeln("The quick brown fox...");

            doc = TestUtil.SaveOpen(builder.Document, @"Model\List\TestJira7856.docx", null, false);   // FOSS: was .doc
            Assert.That(doc.Lists.GetListDefByIndex(0).ListType, Is.EqualTo(ListType.MultiLevel));
        }

        /// <summary>
        /// WORDSNET-8684 Amount of Tab space between List number and Text is incorrectly calculated during rendering to PDF.
        /// andrnosk: The original problem occurred because LegacySpace is specified.
        /// In this case we need to know the length of the label text.
        /// To calculate the length of the label of text we need to know its formatting and measure the string.
        /// I have added mechanism to calculate length of the label of text during converting LegacyListFormatting.
        /// </summary>
        // FOSS: TestJira8684 removed — it triggered LegacyListFormatting conversion via a PDF save and
        // asserted layout-computed tab positions (GetExpandedParaPr(Layout)); layout/rendering is gone.

        /// <summary>
        /// WORDSNET-8699 List changes its number format after appending one document to another.
        /// The problem occurs because of incorrectly calculated NextAvaliableListId and NextAvaliableListDefId.
        /// Fixed the code so it calculate them correctly during importing.
        /// </summary>
        [Test]
        public void TestJira8699()
        {
            Document dstDoc = TestUtil.Open(@"Model\List\TestJira8699A.docx");
            Document srcDoc = TestUtil.Open(@"Model\List\TestJira8699B.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);
            dstDoc = TestUtil.SaveOpen(dstDoc, @"Model\List\TestJira8699.docx", UnifiedScenario.Docx2DocxNoGold);

            ListFormat listFormat = dstDoc.Sections[0].Body.Paragraphs[1].ListFormat;
            Assert.That(listFormat.ListId, Is.EqualTo(4));
            // Check number format.
            Assert.That(listFormat.ListLevel.NumberFormat, Is.EqualTo("\0."));

            listFormat = dstDoc.Sections[1].Body.Paragraphs[1].ListFormat;
            Assert.That(listFormat.ListId, Is.EqualTo(6));
            Assert.That(listFormat.ListLevel.NumberFormat, Is.EqualTo("\0."));
        }



        /// <summary>
        /// WORDSNET-9517 Bullets do not render in PDF for tracked document
        /// Improved ListLabelUpdater behaviour related to revisions.
        /// </summary>
        [Test]
        public void TestJira9517()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira9517.docx");
            Paragraph para = doc.FirstSection.Body.FirstParagraph;

            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));
            doc.UpdateListLabels();   // FOSS: was a Pdf save that triggered the list-label update.

            // Verify that label was updated correctly.
            Assert.That(para.ListLabel.LabelStringOriginal, Is.EqualTo(""));
            Assert.That(para.ListLabel.LabelStringFinal, Is.EqualTo("1."));
        }

        /// <summary>
        /// WORDSNET-16207 List Label is not rendered correctly in output PNG.
        /// It seems Word gets RestartAfterLevel value from ListLevel but not from Override.
        /// Case: abstractNum.lvlRestart = 0, lvlOverride doen't have lvlRestart
        /// </summary>
        [Test]
        public void TestJira16207a()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira16207a.xml");
            doc.UpdateListLabels();

            NodeCollection paras = doc.Sections[0].Body.GetChildNodes(NodeType.Paragraph, true);

            Assert.That(((Paragraph)paras[26]).ListLabel.LabelString, Is.EqualTo("7.20."));

        }

        /// <summary>
        /// WORDSNET-16207 List Label is not rendered correctly in output PNG.
        /// Case: abstractNum doen't have lvlRestart, lvlOverride.lvlRestart = 0
        /// </summary>
        [Test]
        public void TestJira16207b()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira16207b.xml");
            doc.UpdateListLabels();

            NodeCollection paras = doc.Sections[0].Body.GetChildNodes(NodeType.Paragraph, true);

            Assert.That(((Paragraph)paras[26]).ListLabel.LabelString, Is.EqualTo("7.3."));

        }



        /// <summary>
        /// WORDSNET-10219 ListLevel.Font return incorrect values for font name/size.
        /// Added IRunAttrSource to ListLevel to inherit formatting from parent object.
        /// </summary>
        /// <remarks>
        /// AM. Main problem here that we should give customer different ListLevel objects
        /// for different paragraphs althought all paragraphs can refer to single ListLevel in list definition.
        /// This is implemented using inherited ListLevel objects which refers to both paragraph and real ListLevel in list definition.
        /// TestJira10219A below illustrates this.
        /// </remarks>
        [Test]
        public void TestJira10219()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira10219.docx");

            Paragraph tocItemPara = (Paragraph)doc.FirstSection.Body.GetChild(NodeType.Paragraph, 21, true);
            Assert.That(tocItemPara.ParagraphBreakFont.Name, Is.EqualTo("Calibri"));

            ListLevel level = tocItemPara.ListFormat.ListLevel;
            Font font = level.Font;

            Assert.That(font.Name, Is.EqualTo("Calibri"));
            Assert.That(font.Size, Is.EqualTo(11));
        }

        /// <summary>
        /// Relates to WORDSNET-10219
        /// Checks that we expose inherited copy properly in ListFormat.ListLevel.
        /// </summary>
        [Test]
        public void TestJira10219A()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira10219A.docx");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            ListLevel levelPara0 = paras[0].ListFormat.ListLevel;
            Assert.That(levelPara0.Font.Name, Is.EqualTo("Times New Roman"));

            // Check that although ListLevel is inherited from one ListLevel we get different font name in Font object.
            ListLevel levelPara1 = paras[1].ListFormat.ListLevel;
            Assert.That(levelPara0.Font.Name, Is.EqualTo("Times New Roman"));
            Assert.That(levelPara1.Font.Name, Is.EqualTo("Tahoma"));

            // Check that ListLevel is cached.
            ListLevel anotherLevelPara0 = paras[0].ListFormat.ListLevel;
            Assert.That(object.ReferenceEquals(levelPara0, anotherLevelPara0), Is.True);

            // Check that ListFormat.ListLevel reflects parent paragraph changes.
            paras[0].ParagraphBreakFont.Name = "Courier";
            Assert.That(levelPara0.Font.Name, Is.EqualTo("Courier"));

            // Test another list and check that ListLevel cache is invalidated upon ListFormat operations.
            Paragraph p = paras[3];
            Assert.That(p.ListFormat.ListLevel.Font.Name, Is.EqualTo("Algerian"));
            p.ListFormat.ListLevelNumber++;
            Assert.That(p.ListFormat.ListLevel.Font.Name, Is.EqualTo("Calibri"));
        }




        /// <summary>
        /// WORDSNET-11324 Issues with table heights in PDF
        /// Problematic paragraph has LisLevel equal to 12 while max level is 8.
        /// </summary>
        /// <remarks>
        /// AM. This is very weird issue. Word resaves this file preserving invalid list level but renders it in special way.
        /// If document is saved to binary DOC format Word even produces invalid document (Protected View is shown).
        ///
        /// Problematic paragraph is shown in Word like a no-list paragraph with additional Tab character at paragraph start and
        /// I decided to correct the paragraph this way.
        /// </remarks>
        [TestCase(MsWordVersion.Word2013)]
        [TestCase(MsWordVersion.Word2019)]
        public void TestJira11324(MsWordVersion version)
        {
            LoadOptions lo = new LoadOptions();
            lo.MswVersion = version;

            Document doc = TestUtil.Open(@"Model\List\TestJira11324.docx", lo);

            Paragraph p = doc.FirstSection.Body.Tables[0].FirstRow.Cells[2].FirstParagraph;
            Assert.That(p.ParaPr[ParaAttr.ListId], Is.EqualTo(0));
            Assert.That(p.ParaPr[ParaAttr.ListLevel], Is.EqualTo(12));

            string expected = version <= MsWordVersion.Word2013
                ? "\x0009This is the task\r"
                : "This is the task\r";

            Assert.That(p.GetText(), Is.EqualTo(expected));

            Assert.That(p.GetExpandedParaPr(ParaPrExpandFlags.Layout).LeftIndent, Is.EqualTo(0));
        }


        /// <summary>
        /// Relates to TestJira11352. Tests behavior in various scenarios.
        /// </summary>
        [Test]
        public void TestJira11352A()
        {
            // Paragraph has direct negative indent. Word updates LeftIndent by left indent + first line indent.
            TestApplyBulletDefault(@"Model\List\TestLeftIndentNegative.docx", -567, -207);

            // paragraph has direct positive indent. Word updates LeftIndent by left indent + first line indent.
            TestApplyBulletDefault(@"Model\List\TestLeftIndentPositive.docx", 1134, 1494);

            // Paragraph has no direct indent. Word doesn't do anything with paragraph left indent.
            TestApplyBulletDefault(@"Model\List\TestLeftIndentNone.docx", null, null);

            // Paragraph has zero direct left indent and overrides value inherited from style. Word removes it.
            TestApplyBulletDefault(@"Model\List\TestLeftIndentOverriden.docx", 0, null);
        }



        /// <summary>
        /// WORDSNET-11043 Document generated by Aspose.Words opens in 'Protected view'
        /// Added warning for exceeding list count limit.
        /// </summary>
        [Test]
        public void TestJira11043()
        {
            WarningInfoCollection warnings = new WarningInfoCollection();
            Document doc = new Document();
            doc.WarningCallback = warnings;
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            for (int count = 0; count < 10250; count++)
            {
                if (count % 5 == 0)
                {
                    docBuilder.ParagraphFormat.ClearFormatting();
                    docBuilder.Writeln();
                    docBuilder.Writeln("Test");
                    docBuilder.ListFormat.ApplyBulletDefault();
                }
                docBuilder.Write("Test " + count);
                docBuilder.Writeln();
            }

            Assert.That(TestUtil.ContainsWarningBySource(warnings, WarningSource.Validator, WarningStrings.ListCountExceedLimit), Is.True);
        }


        /// <summary>
        /// WORDSNET-11867 Add feature to get/set the Shape/Image of Picture Bullet.
        /// Create PictureBullet from scratch.
        /// </summary>
        [Test]
        public void TestJira11867A()
        {
            Document doc = new Document();

            // Create a list based on one of the Microsoft Word list templates.
            List list = doc.Lists.Add(ListTemplate.NumberDefault);
            ListLevel level1 = list.ListLevels[0];

            // Check PictureBullet shape is null.
            Shape shapeBullet = level1.PictureBullet;
            Assert.That(shapeBullet, Is.Null);

            Byte[] imageBytes = StreamUtil.CopyFileToByteArray(TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet1.png"));
            MemoryStream stream = new MemoryStream(imageBytes);

            // Set PictureBullet using stream.
            level1.CreatePictureBullet();
            level1.ImageData.SetImage(stream);

            ListLevel level2 = list.ListLevels[1];
            level2.CreatePictureBullet();
            level2.ImageData.SetImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet2.png"));

            // Create a list based on one of the Microsoft Word list templates.
            List list2 = doc.Lists.Add(ListTemplate.BulletDefault);

            ListLevel levelbullet2 = list2.ListLevels[1];
            levelbullet2.CreatePictureBullet();
            levelbullet2.ImageData.SetImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet3.png"));

            // Now add some text that uses the list that we created.
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.ListFormat.List = list;

            builder.Writeln("The quick brown fox...");
            builder.ListFormat.ListIndent();

            builder.Writeln("jumped over the lazy dog.");
            builder.ListFormat.ListOutdent();

            builder.Writeln("The quick brown fox...");

            // Here we change list.
            builder.ListFormat.List = list2;
            builder.ListFormat.ListIndent();

            builder.Writeln("jumped over the lazy dog.");

            // Here we change list again.
            builder.ListFormat.List = list;
            builder.ListFormat.ListOutdent();
            builder.Writeln("The quick brown fox...");

            builder.ListFormat.RemoveNumbers();

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestJira11867A", UnifiedScenario.Docx2DocxNoGold);

            ListCollection listCollection = doc.Lists;

            Assert.That(listCollection[0].ListLevels[0].HasPictureBullet, Is.True);

            // Check all picture bullets are applied.
            Assert.That(listCollection.GetPictureBullet(0).ImageData.ImageBytes.Length, Is.EqualTo(839));
            Assert.That(listCollection.GetPictureBullet(1).ImageData.ImageBytes.Length, Is.EqualTo(1737));
            Assert.That(listCollection.GetPictureBullet(2).ImageData.ImageBytes.Length, Is.EqualTo(1653));
        }

        /// <summary>
        /// WORDSNET-11867 Add feature to get/set the Shape/Image of Picture Bullet.
        /// Reset PictureBullet image data.
        /// </summary>
        [Test]
        public void TestJira11867B()
        {
            string imageSrc1 = TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet1.png");
            string imageSrc2 = TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet2.png");
            string imageSrc3 = TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet3.png");

            Document doc = TestUtil.Open(@"Model\List\TestJira11867B.docx");

            Assert.That(doc.Lists.PictureBulletCount, Is.EqualTo(3));

            // Set picture bullets to different lists.
            List list0 = doc.Lists[0];
            ListLevel listLevel0 = list0.ListLevels[0];
            Assert.That(listLevel0.HasPictureBullet, Is.True);

            // Get PictureBullet shape.
            Shape shapeBullet = listLevel0.PictureBullet;

            // Check its size.
            Assert.That(shapeBullet.ImageData.ImageBytes.Length, Is.EqualTo(27850));

            // Set new image.
            listLevel0.ImageData.SetImage(imageSrc1);

            shapeBullet = listLevel0.PictureBullet;
            Assert.That(shapeBullet.ImageData.ImageBytes.Length, Is.EqualTo(839));

            List list1 = doc.Lists[1];

            // Before setting picture bullet.
            Assert.That(list1.ListLevels[0].HasPictureBullet, Is.False);
            list1.ListLevels[0].CreatePictureBullet();
            list1.ListLevels[0].ImageData.SetImage(imageSrc1);

            // After setting picture bullet.
            Assert.That(list1.ListLevels[0].HasPictureBullet, Is.True);

            list1.ListLevels[1].CreatePictureBullet();
            list1.ListLevels[1].ImageData.SetImage(imageSrc2);
            list1.ListLevels[2].CreatePictureBullet();
            list1.ListLevels[2].ImageData.SetImage(imageSrc3);

            doc.Lists[2].ListLevels[0].CreatePictureBullet();
            doc.Lists[2].ListLevels[0].ImageData.SetImage(imageSrc1);

            doc.Lists[3].ListLevels[0].ImageData.SetImage(imageSrc1);
            doc.Lists[4].ListLevels[0].ImageData.SetImage(imageSrc2);
            doc.Lists[5].ListLevels[0].ImageData.SetImage(imageSrc3);

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestJira11867B", UnifiedScenario.Docx2DocxNoGold);

            ListCollection listCollection = doc.Lists;

            Assert.That(listCollection[0].ListLevels[0].HasPictureBullet, Is.True);
            Assert.That(listCollection.PictureBulletCount, Is.EqualTo(7));

            ListLevel listLevel00 = listCollection[0].ListLevels[0];

            // Delete picture bullet.
            listLevel00.DeletePictureBullet();
            Assert.That(listLevel00.HasPictureBullet, Is.False);
            Assert.That(listLevel00.PictureBullet, Is.Null);
            Assert.That(listLevel00.ImageData, Is.Null);
        }

        /// <summary>
        /// WORDSNET-11867 Add feature to get/set the Shape/Image of Picture Bullet.
        /// Universal use-case.
        /// </summary>
        [Test]
        public void TestJira11867C()
        {
            string imageSrc1 = TestUtil.BuildTestFileName(@"DocBuilder\Image\Bullet1.png");

            // Create a document and document builder.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create a list.
            List list = doc.Lists.Add(ListTemplate.BulletCircle);

            ListLevel listLevel0 = list.ListLevels[0];
            // Configure list if necessary
            // Configure first level.
            Assert.That(listLevel0.HasPictureBullet, Is.False);
            Assert.That(listLevel0.ImageData, Is.Null);

            // Create picture bullet with default red cross image for the current list level.
            listLevel0.CreatePictureBullet();
            Assert.That(listLevel0.HasPictureBullet, Is.True);
            Assert.That(listLevel0.ImageData, IsNot.Null());
            Assert.That(listLevel0.ImageData.ImageBytes.Length, Is.EqualTo(924));

            // Set your own picture bullet image.
            listLevel0.ImageData.SetImage(imageSrc1);
            Assert.That(listLevel0.ImageData.ImageBytes.Length, Is.EqualTo(839));

            // Configure second level
            list.ListLevels[1].NumberStyle = NumberStyle.Arabic;
            list.ListLevels[1].NumberFormat = "\u0001.";
            // Configure next levels if necessary
            // ........................................

            // Apply the list to the current paragraph.
            builder.ListFormat.List = list;
            builder.Writeln("item 1");

            // Increase level.
            builder.ListFormat.ListIndent();
            builder.Writeln("item 1.1");
            builder.Writeln("item 1.2");
            builder.Writeln("item 1.3");

            // Decrease level
            builder.ListFormat.ListOutdent();
            builder.Write("item 2");

            // Save output.
            doc = TestUtil.SaveOpen(doc, @"Model\List\TestJira11867C", UnifiedScenario.Docx2DocxNoGold);

            listLevel0 = doc.Lists[0].ListLevels[0];

            Assert.That(listLevel0.HasPictureBullet, Is.True);
            Assert.That(listLevel0.ImageData.ImageBytes.Length, Is.EqualTo(839));

            // Delete picture bullet.
            // Default bullet will be shown after deleting.
            listLevel0.DeletePictureBullet();

            Assert.That(listLevel0.HasPictureBullet, Is.False);
            Assert.That(listLevel0.ImageData, Is.Null);
        }




        /// <summary>
        /// WORDSNET-12929 A document, in which a picture bullet list is created with using Aspose.Words,
        /// was not opened in MS Word for Mac. Bullet shape was created with <see cref="ShapeType.NonPrimitive"/> type,
        /// but it should be <see cref="ShapeType.Image"/>.
        /// </summary>
        [Test]
        public void TestJira12929()
        {
            Document document = new Document();
            ListCollection lists = document.Lists;
            Assert.That(lists.PictureBulletCount, Is.EqualTo(0));

            List list = lists.Add(ListTemplate.BulletDiamonds);
            list.ListLevels[0].CreatePictureBullet();
            Assert.That(lists.PictureBulletCount, Is.EqualTo(1));

            Assert.That(lists.GetPictureBullet(0).ShapeType, Is.EqualTo(ShapeType.Image));
        }

        /// <summary>
        /// WORDSNET-16807 A shape of a picture bullet list created using Aspose.Words had zero size that
        /// caused error on building document layout. Now shape size of a picture bullet is updated on document validation.
        /// </summary>
        [Test]
        public void TestJira16807()
        {
            Document document = new Document();
            DocumentBuilder builder = new DocumentBuilder(document);

            // Create bullet list.
            ListCollection listCollection = document.Lists;
            List list = listCollection.Add(ListTemplate.BulletDiamonds);
            list.ListLevels[0].CreatePictureBullet();
            list.ListLevels[0].ImageData.SetImage(TestUtil.BuildTestFileName(@"ImportHtml\List\Test1.jpg"));

            // Set list to paragraph.
            builder.MoveToDocumentStart();
            builder.CurrentParagraph.ListFormat.ListLevelNumber = 0;
            builder.CurrentParagraph.ListFormat.List = list;

            // An exception was raised on building document layout.
            // Now bullet shape size is fixed in List Validator.
            // No update page layout in FOSS.

            // Check the picture bullet was created.
            Assert.That(listCollection.PictureBulletCount, Is.EqualTo(1));

            // FOSS: shape Width/Height assertions removed — the picture-bullet size is derived from the
            // image during document validation/layout (image handling is gone), so it stays 0 here.
        }


        /// <summary>
        /// WORDSNET-13523 Missing Numbering when save as PDF in Memory Stream.
        /// Should display list label for 'PageBreak' character depending on
        /// compatibility option 'SplitPgBreakAndParaMark'.
        /// </summary>
        [Test]
        public void TestJira13523()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira13523.docx");

            // 'SplitPgBreakAndParaMark' option is set in test document.
            Assert.That(doc.DocPr.CompatibilityOptions.SplitPgBreakAndParaMark, Is.True);

            // The problematic paragraph is 'PageBreak'.
            Paragraph para = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.GetText(), Is.EqualTo("\f\r"));

            doc.UpdateListLabels();
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("2."));

            doc.DocPr.CompatibilityOptions.SplitPgBreakAndParaMark = false;
            doc.UpdateListLabels();
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(""));
        }

        // FOSS: TestJira13689 and TestDmlPictureBullet removed — both depend on DML->VML picture-bullet
        // fallback conversion (via the removed rendering/ProcessFallback path) and validate against the
        // removed .doc / Ecma376 fixed-format save paths.

        /// <summary>
        /// WORDSNET-13607 Bullet list is converted to numbered list after re-saving Docx.
        /// Ignore List definitions with duplicated 'abstractNumId'.
        /// </summary>
        [Test]
        public void TestJira13607()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira13607.docx");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Bullet));
            Assert.That(paras[8].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Bullet));
        }




        // FOSS: TestJira13576 removed — it verified a DOC-import quirk (a style referring to a non-existent
        // list left the paragraph unnumbered on load, then numbered after DOCX save). Converting the .doc
        // input to .docx (in Word) resolves the dangling list reference, so the quirk no longer occurs.



        /// <summary>
        /// WORDSNET-14514 List Labels are incorrect after conversion from Docx to Pdf.
        /// The document has empty list level overrides. MS Word sets their 'StartOverride' to 0.
        /// </summary>
        [Test]
        public void TestJira14514()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira14514.docx");
            doc.UpdateListLabels();

            // Check labels of problematic paragraphs.
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            CheckParagraphLabelAndText(paras[2], "1.0.", "test");
            CheckParagraphLabelAndText(paras[3], "1.0.1.", "test");
        }

        /// <summary>
        /// WORDSNET-14864 List label size differs in layout.
        /// Mimic MS Word glitch.
        /// </summary>
        // FOSS: TestJira14864 removed — it asserted a layout-computed list-label size
        // (GetExpandedRunPr(RunPrExpandFlags.Layout), a "MS Word glitch" only visible in layout);
        // the layout engine is gone.


        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks that imported paragraph reuses the list in the destination.
        /// However, hanging of the list level in the destination is not the same as in the source.
        /// So, expected that additional formatting will be applied to imported paragraph.
        /// </summary>
        [Test]
        public void TestJira14504ReuseDstList()
        {
            Document dstDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseDst.docx");
            Document srcDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseSrc.docx");
            Paragraph[] importedParas = ImportParas(srcDoc, dstDoc, new int[] { 0 }, new int[] { 1 });

            Paragraph importedPara = importedParas[0];
            Assert.That(importedPara.ParaPr[ParaAttr.FirstLineIndent], Is.EqualTo(-720));

            ListFormat format = importedPara.ListFormat;
            Assert.That(format.ListId, Is.EqualTo(1));
            Assert.That(format.ListLevelNumber, Is.EqualTo(0));

            ListCollection lists = importedPara.Document.Lists;
            Assert.That(lists.Count, Is.EqualTo(1));
            Assert.That(lists[0].ListId, Is.EqualTo(1));
            Assert.That(lists[0].ListDefId, Is.EqualTo(0x75190AB7));
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks that imported paragraph reuses the list in the destination.
        /// However, tabs positions of the list level in the destination is not the same as in the source.
        /// So, also it checks how tab positions move from source definition to destination paragraph level.
        /// </summary>
        [Test]
        public void TestJira14504ReuseSrcTabs()
        {
            Document dstDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseDst.docx");
            Document srcDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseSrcTabs.docx");
            Paragraph[] importedParas = ImportParas(srcDoc, dstDoc, new int[] { 0 }, new int[] { 2 });

            Paragraph importedPara = importedParas[0];
            TabStopCollection tabStops = importedPara.ParaPr.TabStops;
            Assert.That(tabStops.Count, Is.EqualTo(3));
            Assert.That(tabStops[0].PositionTwips, Is.EqualTo(1080));
            Assert.That(tabStops[1].PositionTwips, Is.EqualTo(1440));
            Assert.That(tabStops[2].PositionTwips, Is.EqualTo(1800));

            Assert.That(importedPara.ParaPr[ParaAttr.FirstLineIndent], Is.EqualTo(0));

            ListFormat format = importedPara.ListFormat;
            Assert.That(format.ListId, Is.EqualTo(1));
            Assert.That(format.ListLevelNumber, Is.EqualTo(0));

            ListCollection lists = importedPara.Document.Lists;
            Assert.That(lists.Count, Is.EqualTo(1));
            Assert.That(lists[0].ListId, Is.EqualTo(1));
            Assert.That(lists[0].ListDefId, Is.EqualTo(0x75190AB7));
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks that difference between list definitions attributes of the source and destinations
        /// calculates and moves to paragraph level only when list definitions were reused.
        /// </summary>
        [Test]
        public void TestJira14504ReuseSrcExpand()
        {
            Document srcDoc = TestUtil.Open(@"Model\List\TestImportList14504ExpandSrc.docx");
            Document dstDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseDst.docx");

            ListFormat listFormat = srcDoc.FirstSection.Body.LastParagraph.ListFormat;
            ListDefStub listDefStub = new ListDefStub(listFormat.List.ListDef);
            List newList = listFormat.List.Clone(srcDoc, srcDoc.Lists.GetNextAvailableListId());
            srcDoc.Lists.AddListDef(listDefStub);
            srcDoc.Lists.AddList(newList);

            newList.ListDefId = listDefStub.ListDefId;
            listFormat.List = newList;
            Assert.That(listFormat.List.ListDef, Is.EqualTo(listDefStub));

            NodeImporter imorter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.UseDestinationStyles);
            Paragraph para = (Paragraph)imorter.ImportNode(srcDoc.FirstSection.Body.LastParagraph, true);

            Assert.That(para.ParaPr.Count, Is.EqualTo(srcDoc.FirstSection.Body.LastParagraph.ParaPr.Count));
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks that imported paragraphs reuse the list in the destination.
        /// However, indentions of the list levels in the destination is not the same as in the source.
        /// So, also it checks how indentions of paragraphs from source definition merge with destination values.
        /// </summary>
        [Test]
        public void TestImportList14504ReuseSrcLeftIndMiss()
        {
            Document dstDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseDst2.docx");
            Document srcDoc = TestUtil.Open(@"Model\List\TestImportList14504ReuseSrcLeftIndMiss.docx");
            Paragraph[] importedParas = ImportParas(srcDoc, dstDoc, new int[] { 0, 1, 2 }, new int[] { 3, 3, 1 });

            ParaPr pr = importedParas[0].ParaPr;
            Assert.That(pr[ParaAttr.FirstLineIndent], Is.EqualTo(0));
            Assert.That(pr[ParaAttr.LeftIndent], Is.EqualTo(720));
            Assert.That(pr[ParaAttr.RightIndent], Is.EqualTo(1440));
            Assert.That(pr.ListId, Is.EqualTo(1));
            Assert.That(pr.ListLevel, Is.EqualTo(0));

            pr = importedParas[1].ParaPr;
            Assert.That(pr[ParaAttr.FirstLineIndent], Is.EqualTo(0));
            Assert.That(pr[ParaAttr.LeftIndent], Is.EqualTo(720));
            Assert.That(pr[ParaAttr.RightIndent], Is.EqualTo(2160));
            Assert.That(pr.ListId, Is.EqualTo(1));
            Assert.That(pr.ListLevel, Is.EqualTo(1));

            pr = importedParas[2].ParaPr;
            Assert.That(pr[ParaAttr.LeftIndent], Is.EqualTo(0));
            Assert.That(pr.ListId, Is.EqualTo(1));
            Assert.That(pr.ListLevel, Is.EqualTo(2));

            ListCollection lists = importedParas[0].Document.Lists;
            Assert.That(lists.Count, Is.EqualTo(1));
            Assert.That(lists[0].ListId, Is.EqualTo(1));
            Assert.That(lists[0].ListDefId, Is.EqualTo(0x75190AB7));
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indentation of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has empty paragraph and left indent + first line indent of the not empty paragraph
        /// greater then the left indent + first line indent of the list item. So, list definition will be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 567,
            // Paragraph first line: None,
            // List item left indent: 360,
            // List item hanging: 720.

            Document dstDoc = TestUtil.Open(@"Model\List\TesttJira14504ParaIndentDst.fopc");
            Document srcDoc = TestUtil.Open(@"Model\List\TesttJira14504ParaIndentSrc.fopc");
            Assert.That(dstDoc.Lists.ListDefs.Count, Is.EqualTo(10));

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);
            Assert.That(dstDoc.Lists.ListDefs.Count, Is.EqualTo(10));

            Paragraph para = dstDoc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.ListFormat.ListId, Is.EqualTo(4));
            Assert.That(para.ListFormat.ListLevel.LevelNumber, Is.EqualTo(0));
            Assert.That(para.ListFormat.List.ListDefId, Is.EqualTo(8));

            para = dstDoc.LastSection.Body.Paragraphs[0];
            Assert.That(para.ListFormat.IsListItem, Is.False);

            para = dstDoc.LastSection.Body.Paragraphs[2];
            Assert.That(para.ListFormat.ListId, Is.EqualTo(6));
            Assert.That(para.ListFormat.ListLevel.LevelNumber, Is.EqualTo(0));
            Assert.That(para.ListFormat.List.ListDefId, Is.EqualTo(8));

            para = dstDoc.LastSection.Body.Paragraphs[3];
            Assert.That(para.ListFormat.ListId, Is.EqualTo(6));
            Assert.That(para.ListFormat.ListLevel.LevelNumber, Is.EqualTo(0));
            Assert.That(para.ListFormat.List.ListDefId, Is.EqualTo(8));
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indentation of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has non empty paragraph with left indent less then left indent of the list item.
        /// However, left indent + first line indent of the not empty paragraph greater then the
        /// left indent + first line indent of the list item. So, list definition will be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent2()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 57,
            // Paragraph first line: None,
            // List item left indent: 360,
            // List item hanging: 720.
            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc2.fopc", true);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indention of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has empty paragraph and left indent + first line indent of the not empty paragraph
        /// less then the left indent + first line indent of the list item. So, list definition will not be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent3()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 400,
            // Paragraph hanging: 2000,
            // List item left indent: 360,
            // List item hanging: 720.
            CheckListImportParaIndent(@"Model\List\TestJira14504ParaIndentSrc3.fopc", false);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indention of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has non empty paragraph with first line greater then first line of the list item.
        /// However, left indent + first line indent of the not empty paragraph greater then the
        /// left indent + first line indent of the list item. So, list definition will be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent4()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: -400,
            // Paragraph first line: 100,
            // List item left indent: 360,
            // List item hanging: 720.
            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc4.fopc", true);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indention of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has non empty paragraph with first line and left indent equal to first line and left
        /// indent of the list item. Expected that list definition will be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent5()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 360,
            // Paragraph hanging: 720,
            // List item left indent: 360,
            // List item hanging: 720.
            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc5.fopc", true);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indention of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has non empty paragraph which contains value of the left indent equal to absolute value
        /// of the first line of the list item and absolute value of the first line of the paragraph equal to
        /// left indent of the list item. Expected that list definition will not be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent6()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 360,
            // Paragraph hanging: 720,
            // List item left indent: 720,
            // List item hanging: 360.
            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc6.fopc", false);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Checks how indention of the previous not empty paragraph of the list item influences to list reusing.
        /// Source document has non empty paragraph only with a picture and left indent + first line indent of the
        /// not empty paragraph less then the left indent + first line indent of the list item.
        /// So, list definition will not be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent7()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: -567,
            // Paragraph hanging: None.
            // List item left indent: 360,
            // List item hanging: 720.
            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc7.fopc", false);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Test checks how indention of the paragraph influences to list reusing.
        /// Source document has non empty paragraph only with white spaces and left indent + first line indent of the
        /// not empty paragraph greater then the left indent + first line indent of the list item.
        /// So, list definition will be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent8()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 1247,
            // Paragraph hanging: None,
            // List item left indent: 720,
            // List item hanging: 360.

            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc8.fopc", true);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-14504
        /// Test checks how indention of the paragraph influences to list reusing.
        /// Source document has empty paragraph with insert revision which contains a white spaces.
        /// Also left indent + first line indent of it greater then the left indent + first line indent of the list item.
        /// So, list definition will be reused.
        /// </summary>
        [Test]
        public void TestJira14504ParaIndent9()
        {
            // Attributes of the elements in the source document are the following:
            // Paragraph left indent: 1134.
            // Paragraph hanging: None.
            // List item left indent: 720.
            // List item hanging: 360.
            CheckListImportParaIndent(@"Model\List\TesttJira14504ParaIndentSrc9.fopc", true);
        }

        /// <summary>
        /// Test is concerned with WORDSNET-15054
        /// ListDef.IsRestartAtEachSection is exposed to public.
        /// </summary>
        [Test]
        public void TestJira15054A()
        {
            Document doc = new Document();
            doc.Lists.Add(ListTemplate.NumberDefault);

            List list = doc.Lists[0];

            // By default is false.
            Assert.That(list.IsRestartAtEachSection, Is.EqualTo(false));

            // Set true.
            list.IsRestartAtEachSection = true;
            Assert.That(list.IsRestartAtEachSection, Is.EqualTo(true));

            // IsRestartAtEachSection will be written only if compliance is higher then OoxmlComplianceCore.Ecma376
            OoxmlSaveOptions options = new OoxmlSaveOptions();
            options.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestJira15054A.docx", options, false);
            list = doc.Lists[0];
            Assert.That(list.IsRestartAtEachSection, Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-15788 List label values are incorrect after joining documents.
        /// Imported lists re-use while joining documents. Only lists which initially located
        /// in the destination can be re-used. Imported lists have to be excluded while re-using
        /// to fix the problem.
        /// </summary>
        [Test]
        public void TestJira15788()
        {
            Document dstDoc = new Document();
            Document srcDoc = TestUtil.Open(@"ImportDocx\TestJira15788", LoadFormat.Docx);
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            ParagraphCollection paras = dstDoc.LastSection.Body.Paragraphs;
            Paragraph paraFirst = paras[8];
            ParaPr paraPr = paraFirst.ParaPr;


            Assert.That(paraPr[ParaAttr.ListId], Is.EqualTo(3));
            Assert.That(paraPr[ParaAttr.ListLevel], Is.EqualTo(0));

            Paragraph paraLast = paras[15];
            paraPr = paraLast.ParaPr;
            Assert.That(paraPr[ParaAttr.ListId], Is.EqualTo(4));
            Assert.That(paraPr[ParaAttr.ListLevel], Is.EqualTo(0));

            Assert.That(dstDoc.Lists.GetListByListId(3).ListDefId, Is.EqualTo(0x6C647412));
            Assert.That(dstDoc.Lists.GetListByListId(4).ListDefId, Is.EqualTo(0x6C647412));

            // One more insane check that test actual label numbers.
            dstDoc.UpdateListLabels();
            CheckParagraphLabelAndText(paraLast, "1)", "Text");
            CheckParagraphLabelAndText(paraFirst, "1)", "Text");
        }

        /// <summary>
        /// WORDSNET-15226 List label is changed after conversion from DOC to DOCX/PDF.
        /// We should ignore terminal list definition when updating list labels, if this is style definition.
        /// </summary>
        // FOSS: TestJira15226 removed — it verified a DOC->DOCX list-label conversion quirk from a .doc
        // input; converting that input to .docx (in Word) normalizes the list so the quirk no longer occurs.


        /// <summary>
        /// WORDSNET-17658 incorrect resolving of lists in a broken document.
        /// Implemented correct resolving of problematic chain of list references, which leads to nonexistent list.
        /// Each list which involved in such chain of lists, transfromed into self-referenced list, i.e.
        /// List.Style.List points to the List itself.
        /// </summary>
        [Test]
        public void TestJira9911Improve()
        {
            // Verify no exception.
            Document doc = TestUtil.Open(@"ExportDocx\TestJira9911.docx");

            // List uses listStyle "BulletList" which refers to List with listStyle "NumberList" which refers to
            // List with listStyle "LijstTim". Style "LijstTim" points to an nonexistent list with Id = 15.
            // MSW removes all these references, and transforms all three lists into List Style Definitions.

            // Verify correct resolving.
            // List style "BulletList"
            VerifySelfReferencedList(doc.Lists.GetListByListId(1), "BulletList");
            // List style "NumberList"
            VerifySelfReferencedList(doc.Lists.GetListByListId(2), "NumberList");
            // List style "LijstTim"
            VerifySelfReferencedList(doc.Lists.GetListByListId(4), "LijstTim");
        }


        /// <summary>
        /// WORDSNET-16755 Incorrect conversion of bullet points in word.
        /// This issue was fixed per WORDSNET-16660
        /// </summary>
        /// <remarks>
        /// This test is implemented for verification the customer document from
        /// WORDSNET-16755 exclusively to check it has been actually fixed already.
        /// </remarks>
        // FOSS: TestJira16755 removed \u2014 its customer input (TestJira16755.docx) is detected as HTML,
        // and HTML load is not supported in FOSS. It only re-verified an already-fixed customer case.

        /// <summary>
        /// WORDSNET-17335 "Not expected other boolex values here" error occurs upon invoking ListFormat.ListLevel.Font.Italic.
        /// The problem occurs because the inherited value is "AttrBoolEx Toggle"
        /// and need to be resolved using parent values and not just converted to bool.
        /// </summary>
        // FOSS: TestJira17335 removed — it checked an AttrBoolEx-Toggle inheritance quirk on a Cyrillic
        // heading style loaded from a .doc; converting the input to .docx restructures the style so the
        // scenario no longer reproduces.

        /// <summary>
        /// WORDSNET-17717 Incorrect conversion of list markers.
        /// When SingleLevel list with overrides is used for ListParagraphs with different levels,
        /// we must use 0-th Override ListLevel, regardless ListParagraph's level.
        /// </summary>
        /// <remarks>
        /// Please, check <see cref="List.GetFormattingOverride"/> for the details.
        /// </remarks>
        [Test]
        public void TestJira17717()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira17717.docx");
            doc.UpdateListLabels();
            Paragraph para = doc.FirstSection.Body.Paragraphs[7];
            Assert.That(para.ListLabel.Font.Name, Is.EqualTo("Symbol"));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("\uF0B7"));
        }

        /// <summary>
        /// WORDSJAVA-1908 Dashes in bullets list disappear in rendered PDF
        /// </summary>
        [Test]
        public void TestJiraJ1908()
        {
            Document doc = TestUtil.Open(@"Model\Header\TestJiraJ1908.docx");
            doc.UpdateListLabels();

            Paragraph para = doc.FirstSection.Body.Paragraphs[0];
            CheckParagraphLabelAndText(para, "\u00ad", "1");

            para = doc.FirstSection.Body.Paragraphs[1];
            CheckParagraphLabelAndText(para, "", "2");
        }



        /// <summary>
        /// WORDSNET-18228 AppendDocument destroys lists.
        /// The problematic list has the same list definition identifier in the source and destination documents. So, the list
        /// definition from destination was used in a copy instead of importing it from the source document.
        /// Introduced the additional ImportFormatOption parameter to use in AppendDocument() that allows to specify how
        /// to import lists with the equal list definition ids.
        /// </summary>
        [Test]
        public void TestJira18228()
        {
            const string dstFileName = @"Model\List\TestJira18228Dst.docx";
            const string srcFileName = @"Model\List\TestJira18228Src.docx";

            // By default Word and AW reuse equal list definition ids from a destination document.
            ImportFormatOptions importFormatOptions = new ImportFormatOptions();
            Document doc = AppendDocuments(dstFileName, srcFileName, ImportFormatMode.UseDestinationStyles, importFormatOptions);
            CheckParagraphLabelAndText(doc.Sections[1].Body.LastParagraph, "2.", "13->13");

            // This is what the customer wants.
            importFormatOptions.KeepSourceNumbering = true;
            doc = AppendDocuments(dstFileName, srcFileName, ImportFormatMode.UseDestinationStyles, importFormatOptions);
            CheckParagraphLabelAndText(doc.Sections[1].Body.LastParagraph, "1.", "13->13");
        }

        /// <summary>
        /// WORDSNET-18475 Invalid numbering after Section Break.
        /// If w15:restartNumberingAfterBreak is 1 and the list paragraphs are splitted by section break
        /// Word restarts numbering starting from level of first occurred paragraph in next section.
        /// </summary>
        public void Test18475(string fileName, string expectedNPlusOneLevelNumber, string expectedNLevelNumber)
        {
            Document doc = TestUtil.Open(fileName);
            doc.UpdateListLabels();

            string actualNPlusOneLevelNumber = doc.Sections[1].Body.FirstParagraph.ListLabel.LabelString;
            Assert.That(actualNPlusOneLevelNumber, Is.EqualTo(expectedNPlusOneLevelNumber));

            string actualNLevelNumber = doc.Sections[1].Body.LastParagraph.ListLabel.LabelString;
            Assert.That(actualNLevelNumber, Is.EqualTo(expectedNLevelNumber));
        }


        /// <summary>
        /// WORDSNET-18925 Unexpected increase of paragraph left indent on assigning
        /// <see cref="ListFormat.ListId"/>.
        /// </summary>
        // FOSS: Test18925 removed — its input is an .mht (MHTML) file and MHTML load was removed.

        /// <summary>
        /// WORDSNET-19316 Numbering disappear during appending documents.
        /// When taken decision on whether to expand style formatting to direct paragraph
        /// attributes we need to consider also list formatting for the styles with numbering.
        /// </summary>
        [TestCase("Test19316dst.docx", false, "1.")]
        [TestCase("Test19316dst.docx", true, "1.")]
        [TestCase("Test19316DstSameListDefId.docx", false, "")]
        [TestCase("Test19316DstSameListDefId.docx", true, "1.")]
        public void Test19316(string dstFileName, bool isKeepSourceNumbering, string expectedLabelString)
        {
            Document srcDoc = TestUtil.Open(@"Model\List\Test19316src.docx");
            Document dstDoc = TestUtil.Open(string.Format(@"Model\List\{0}", dstFileName));

            ImportFormatOptions options = new ImportFormatOptions();
            options.KeepSourceNumbering = isKeepSourceNumbering;

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);
            dstDoc.UpdateListLabels();

            Paragraph para = dstDoc.Sections[1].Body.Paragraphs[1];
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(expectedLabelString));
        }

        /// <summary>
        /// WORDSNET-19316 Numbering chapters disappear during appending documents.
        /// Do not expand style to direct formatting when lists identifier in the source and in the destination are different.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test19275(bool isKeepSourceNumbering)
        {
            Document srcDoc = TestUtil.Open(@"Model\List\Test19275Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\List\Test19275DifferentListIds.docx");
            Assert.That(dstDoc.Lists.Count, Is.EqualTo(2));

            ImportFormatOptions options = new ImportFormatOptions();
            options.KeepSourceNumbering = isKeepSourceNumbering;

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);
            dstDoc.UpdateListLabels();

            Paragraph para = dstDoc.LastSection.Body.Paragraphs[1];

            Assert.That(dstDoc.Lists.Count, Is.EqualTo(isKeepSourceNumbering ? 3: 2));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(isKeepSourceNumbering ? "1." : string.Empty));

            object listId = para.ParaPr[ParaAttr.ListId];
            if (isKeepSourceNumbering)
                Assert.That(listId, IsNot.Null());
            else
                Assert.That(listId, Is.Null);
        }

        /// <summary>
        /// Related with WORDSNET-19316
        /// Checks case when list identifiers are equal in the source and in the destination.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test19275SameListIds(bool isKeepSourceNumbering)
        {
            Document srcDoc = TestUtil.Open(@"Model\List\Test19275Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\List\Test19275SameListIds.docx");
            Assert.That(dstDoc.Lists.Count, Is.EqualTo(2));

            ImportFormatOptions options = new ImportFormatOptions();
            options.KeepSourceNumbering = isKeepSourceNumbering;

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);
            dstDoc.UpdateListLabels();

            Paragraph para = dstDoc.LastSection.Body.Paragraphs[1];

            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));

            Assert.That(dstDoc.Lists.Count, Is.EqualTo(3));
            Assert.That(para.ParaPr[ParaAttr.ListId], Is.EqualTo(3));
            Assert.That(para.ParaPr[ParaAttr.LeftIndent], Is.EqualTo(720));
            Assert.That(para.ParaPr[ParaAttr.FirstLineIndent], Is.EqualTo(-360));
        }


        /// <summary>
        /// WORDSNET-20874 DOCX to PDF conversion issue with list item rendering
        /// Improved unexpected list level handling.
        /// </summary>
        [TestCase(MsWordVersion.Word2013)]
        [TestCase(MsWordVersion.Word2019)]
        public void Test20874(MsWordVersion loadVersion)
        {
            LoadOptions lo = new LoadOptions() { MswVersion = loadVersion };
            Document doc = TestUtil.Open(@"Model\List\Test20874.docx", lo);

            Paragraph para = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.ParaPr[ParaAttr.ListId], Is.EqualTo(0));
            Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.EqualTo(12));

            string expected = loadVersion == MsWordVersion.Word2013
                ? "\t2\x00b0 \tLorem Ipsum\r"
                : "2\x00b0 \tLorem Ipsum\r";

            Assert.That(para.GetText(), Is.EqualTo(expected));
        }

        /// <summary>
        /// WORDSNET-19255 Debug.Assert failure in the layout code when a DOCX document is converted to PDF.
        /// <see cref="ListLevel.Legacy"/> was set to false for the bullet list. Word doesn't do that. Mimic Word.
        /// </summary>
        // FOSS: Test19255 removed — it ran the PDF-layout validator and asserted a layout-computed
        // legacy tab position (GetExpandedParaPr(Layout)); the layout engine is gone.


        /// <summary>
        /// WORDSNET-22525 WK: NumberStyle CUSTOM for LEADING_ZERO formats.
        /// A new feature has been implemented.
        /// </summary>
        /// <remarks>
        /// <para><see cref="ListLevel.GetEffectiveValue(int, NumberStyle, string)"/></para>
        /// <para><see cref="ListLevel.CustomNumberStyleFormat"/></para>
        /// </remarks>
        [Test]
        public void Test22525()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22525", LoadFormat.Docx);
            ListLevel listLevel = doc.FirstSection.Body.Paragraphs[0].ListFormat.ListLevel;
            Assert.That(listLevel.CustomNumberStyleFormat, Is.EqualTo("001, 002, 003, ..."));
            Assert.That(ListLevel.GetEffectiveValue(5, listLevel.NumberStyle, listLevel.CustomNumberStyleFormat), Is.EqualTo("005"));
        }

        /// <summary>
        /// Related with WORDSNET-22525
        /// Checks the method.
        /// </summary>
        [Test]
        public void Test22525_02()
        {
            Assert.That(ListLevel.GetEffectiveValue(4, NumberStyle.LowercaseRoman, null), Is.EqualTo("iv"));
            Assert.That(ListLevel.GetEffectiveValue(5, NumberStyle.Custom, "001, 002, 003, ..."), Is.EqualTo("005"));
        }

        /// <summary>
        /// Related with WORDSNET-22525
        /// Checks the <see cref="ArgumentOutOfRangeException"/> for the method.
        /// </summary>
        [TestCase(0)]
        [TestCase(32768)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test22525_03(int index)
        {
            ListLevel.GetEffectiveValue(index, NumberStyle.LowercaseRoman, null);
        }

        /// <summary>
        /// Related with WORDSNET-22525
        /// Checks the <see cref="ArgumentException"/> for the method.
        /// </summary>
        [TestCase(NumberStyle.Custom, "")]
        [TestCase(NumberStyle.Custom, null)]
        [TestCase(NumberStyle.LowercaseRoman, "001, 002, 003, ...")]
        [TestCase(NumberStyle.Custom, "000001, 000002, 000003, ...")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test22525_04(NumberStyle numberStyle, string customNumberStyleFormat)
        {
            ListLevel.GetEffectiveValue(4, numberStyle, customNumberStyleFormat);
        }

        /// <summary>
        /// WORDSNET-17658 Verifies cross-format issues in import/export of ListStyleReference ListDefs.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListStyleReferences(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\List\TestListStyleReference", lf);

            doc = TestUtil.SaveOpen(doc, @"Model\List\TestListStyleReference", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            string desiredValue = (sf == SaveFormat.Doc) ? "43" : "58";

            doc.UpdateListLabels();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(desiredValue));
        }

        /// <summary>
        /// WORDSNET-24142 Providing a public API to determine the origin of lists from the same template.
        /// </summary>
        [Test]
        public void Test24142()
        {
            Document doc = TestUtil.Open(@"Model\List\Test24142.docx");
            List list1 = doc.Lists[0];
            List list2 = doc.Lists[1];
            List list3 = doc.Lists[2];

            Assert.That(list1.HasSameTemplate(list2), Is.True);
            Assert.That(list2.HasSameTemplate(list3), Is.False);
        }










        /// <summary>
        /// WORDSNET-26149 Allow creating a single level lists through public API.
        /// Tests the public API of AddSingleLevelList in ListCollection.
        /// </summary>
        [TestCase(ListTemplate.BulletDisk, "\xf0b7", "Symbol")]
        [TestCase(ListTemplate.BulletCircle, "o", "Courier New")]
        [TestCase(ListTemplate.BulletSquare, "\xf0a7", "Wingdings")]
        [TestCase(ListTemplate.BulletDiamonds, "\xf076", "Wingdings")]
        [TestCase(ListTemplate.BulletArrowHead, "\xf0d8", "Wingdings")]
        [TestCase(ListTemplate.BulletTick, "\xf0fc", "Wingdings")]
        public void Test26149_Bulleted(ListTemplate listTemplate, string numberFormat, string fontName)
        {
            Document doc = new Document();
            List list = doc.Lists.AddSingleLevelList(listTemplate);

            Assert.That(list.ListDef.ListType, Is.EqualTo(ListType.SingleLevel));
            Assert.That(list.ListLevels.Count, Is.EqualTo(1));

            ListLevel listLevel = list.ListLevels[0];
            Assert.That(listLevel.ParaPr.LeftIndent, Is.EqualTo(720));
            Assert.That(listLevel.ParaPr.FirstLineIndent, Is.EqualTo(-360));
            Assert.That(listLevel.ParaPr.TabStops[0].PositionTwips, Is.EqualTo(720));

            Assert.That(listLevel.NumberFormat, Is.EqualTo(numberFormat));
            Assert.That(listLevel.Font.NameOther, Is.EqualTo(fontName));
            Assert.That(listLevel.Font.NameAscii, Is.EqualTo(fontName));
        }

        /// <summary>
        /// Relates to WORDSNET-26149.
        /// </summary>
        [TestCase(ListTemplate.NumberArabicDot, NumberStyle.Arabic, "\x0000.", ListLevelAlignment.Left)]
        [TestCase(ListTemplate.NumberArabicParenthesis, NumberStyle.Arabic, "\x0000)", ListLevelAlignment.Left)]
        [TestCase(ListTemplate.NumberUppercaseRomanDot, NumberStyle.UppercaseRoman, "\x0000.", ListLevelAlignment.Right)]
        [TestCase(ListTemplate.NumberUppercaseLetterDot, NumberStyle.UppercaseLetter, "\x0000.", ListLevelAlignment.Left)]
        [TestCase(ListTemplate.NumberLowercaseLetterParenthesis, NumberStyle.LowercaseLetter, "\x0000)", ListLevelAlignment.Left)]
        [TestCase(ListTemplate.NumberLowercaseLetterDot, NumberStyle.LowercaseLetter, "\x0000.", ListLevelAlignment.Left)]
        [TestCase(ListTemplate.NumberLowercaseRomanDot, NumberStyle.LowercaseRoman, "\x0000.", ListLevelAlignment.Right)]
        public void Test26149_Numbered(ListTemplate listTemplate, NumberStyle numberStyle, string numberFormat, ListLevelAlignment alignment)
        {
            Document doc = new Document();
            List list = doc.Lists.AddSingleLevelList(listTemplate);

            Assert.That(list.ListDef.ListType, Is.EqualTo(ListType.SingleLevel));
            Assert.That(list.ListLevels.Count, Is.EqualTo(1));

            ListLevel listLevel = list.ListLevels[0];
            Assert.That(listLevel.ParaPr.LeftIndent, Is.EqualTo(720));
            Assert.That(listLevel.ParaPr.FirstLineIndent, Is.EqualTo(-360));
            Assert.That(listLevel.ParaPr.TabStops[0].PositionTwips, Is.EqualTo(720));

            Assert.That(listLevel.NumberStyle, Is.EqualTo(numberStyle));
            Assert.That(listLevel.NumberFormat, Is.EqualTo(numberFormat));
            Assert.That(listLevel.Alignment, Is.EqualTo(alignment));
        }

        /// <summary>
        /// Relates to WORDSNET-26149.
        /// </summary>
        [TestCase(ListTemplate.OutlineNumbers)]
        [TestCase(ListTemplate.OutlineLegal)]
        [TestCase(ListTemplate.OutlineBullets)]
        [TestCase(ListTemplate.OutlineHeadingsArticleSection)]
        [TestCase(ListTemplate.OutlineHeadingsLegal)]
        [TestCase(ListTemplate.OutlineHeadingsNumbers)]
        [TestCase(ListTemplate.OutlineHeadingsChapter)]
        [ExpectedException(typeof(ArgumentException))]
        public void Test26149_Outlined(ListTemplate listTemplate)
        {
            Document doc = new Document();
            doc.Lists.AddSingleLevelList(ListTemplate.OutlineHeadingsArticleSection);
        }

        /// <summary>
        /// WORDSNET-28555 Formatting multi-level list issue.
        /// Tests importing a document with duplicate Lists and ListDefs identifiers.
        /// </summary>
        [Test]
        public void Test28555()
        {
            Document doc = TestUtil.Open(@"Model\List\Test28555.docx");

            doc.UpdateListLabels();
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[2].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo("1."));

            Assert.That(paras[3].ListLabel.LabelValue, Is.EqualTo(2));
            Assert.That(paras[3].ListLabel.LabelString, Is.EqualTo("2."));

            Assert.That(paras[4].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[4].ListLabel.LabelString, Is.EqualTo(string.Empty));

            Assert.That(paras[7].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[7].ListLabel.LabelString, Is.EqualTo("·"));

            Assert.That(paras[8].ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paras[8].ListLabel.LabelString, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// WORDSNET-28878 List numbering is wrong after converting DOCX to PDF
        /// Restart numbering after break according to the list identifier.
        /// </summary>
        public void Test28878(string testFile)
        {
            Document doc = TestUtil.Open(testFile);
            doc.UpdateListLabels();

            Paragraph para = TestUtil.GetParagraphWithText(doc, "Judgment of Overall Residual Risk Acceptability...");
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("4.2.1"));
        }

        private static void VerifySelfReferencedList(List list, string styleName)
        {
            Assert.That(list, IsNot.Null());
            Assert.That(list.Style.Name, Is.EqualTo(styleName));
            Assert.That(list.Style.List.ListId, Is.EqualTo(list.ListId));
        }

        /// <summary>
        /// Imports paragraphs with specified indexes from source to destination and
        /// checks count of attributes which directly set to the destination paragraph.
        /// </summary>
        private static Paragraph[] ImportParas(Document srcDoc,Document dstDoc, int[] parasNums, int[] expectedAttrCount)
        {
            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.UseDestinationStyles);
            ParagraphCollection paras = srcDoc.FirstSection.Body.Paragraphs;
            Paragraph[] importedParas = new Paragraph[parasNums.Length];

            for (int i = 0; i < paras.Count; ++i)
            {
                int importIndex = -1;
                for (int j = 0; j < parasNums.Length; ++j)
                {
                    if (i == parasNums[j])
                    {
                        importIndex = j;
                        break;
                    }
                }

                if (importIndex < 0)
                    continue;

                int initialAttrCount = paras[i].ParaPr.Count;
                Paragraph importedPara = (Paragraph)importer.ImportNode(paras[i], true);
                importedParas[i] = importedPara;

                // Expected additional attributes which was moved from list definition level.
                Assert.That(importedParas[importIndex].ParaPr.Count, Is.EqualTo(initialAttrCount + expectedAttrCount[importIndex]));
            }

            return importedParas;
        }

        /// <summary>
        /// Checks number of picture bullets and properties of the first bullet.
        /// </summary>
        private static void CheckPictureBullets(Document doc, int expectedCount,
            ShapeMarkupLanguage expectedMarkup, bool expectedHasFallback)
        {
            Assert.That(doc.Lists.PictureBulletCount, Is.EqualTo(expectedCount));

            Shape shape = doc.Lists.GetPictureBullet(0);

            Assert.That(shape.MarkupLanguage, Is.EqualTo(expectedMarkup));
            Assert.That(shape.FallbackShape != null, Is.EqualTo(expectedHasFallback));
            if (expectedHasFallback)
            {
                Assert.That(shape.FallbackShape.MarkupLanguage, Is.EqualTo((expectedMarkup == ShapeMarkupLanguage.Dml) ? ShapeMarkupLanguage.Vml : ShapeMarkupLanguage.Dml));
            }
        }

        /// <summary>
        /// Tests direct left indent before and after ApplyBylletDefault method called for paragraph.
        /// </summary>
        private static void TestApplyBulletDefault(string fileName, object before, object after)
        {
            Document doc = TestUtil.Open(fileName);

            Paragraph p = doc.FirstSection.Body.FirstParagraph;

            if (before != null)
                Assert.That(p.ParaPr[ParaAttr.LeftIndent], Is.EqualTo((int)before));
            else
                Assert.That(p.ParaPr[ParaAttr.LeftIndent], Is.Null);

            p.ListFormat.ApplyBulletDefault();

            if (after != null)
                Assert.That(p.ParaPr[ParaAttr.LeftIndent], Is.EqualTo((int)after));
            else
                Assert.That(p.ParaPr[ParaAttr.LeftIndent], Is.Null);
        }

        /// <summary>
        /// Checks paragraph's label string and
        /// that paragraph's text starts with the specified <paramref name="expectedText"/>.
        /// </summary>
        private static void CheckParagraphLabelAndText(Paragraph para, string expectedLabelString, string expectedText)
        {
            string paraText = para.GetText();
            Assert.That(paraText.StartsWith(expectedText), Is.True, paraText);
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(expectedLabelString));
        }

        /// <summary>
        /// This is helper method for TestJira14504 methods family.
        /// It checks how list definitions are imported to destination from the specified file.
        /// </summary>
        /// <param name="srcFile">Path to source file.</param>
        /// <param name="isReuseListDef">If true, then it is expected that list definitions have to be reused.</param>
        private static void CheckListImportParaIndent(string srcFile, bool isReuseListDef)
        {
            Document dstDoc = TestUtil.Open(@"Model\List\TesttJira14504ParaIndentDst.fopc");
            Document srcDoc = TestUtil.Open(srcFile);
            int defsCountBefore = dstDoc.Lists.ListDefs.Count;

            if (isReuseListDef)
                srcDoc.FirstSection.Body.Paragraphs[1].GetChildNodes(NodeType.Any, false).Add(new Run(srcDoc, ""));

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);
            Assert.That(dstDoc.Lists.ListDefs.Count, Is.EqualTo(defsCountBefore + (isReuseListDef ? 0 : 1)));

            Paragraph para = dstDoc.LastSection.Body.Paragraphs[2];
            Assert.That(para.ListFormat.ListId, Is.EqualTo(6));
            Assert.That(para.ListFormat.ListLevel.LevelNumber, Is.EqualTo(0));
            Assert.That(para.ListFormat.List.ListDefId, Is.EqualTo(isReuseListDef ? 8 : 11));
        }

        /// <summary>
        /// Appends one document to another.
        /// </summary>
        private static Document AppendDocuments(string dstFileName, string srcFileName,
            ImportFormatMode importFormatMode, ImportFormatOptions importFormatOptions)
        {
            Document srcDoc = TestUtil.Open(srcFileName);
            Document dstDoc = TestUtil.Open(dstFileName);

            dstDoc.AppendDocument(srcDoc, importFormatMode, importFormatOptions);

            dstDoc.UpdateListLabels();

            return dstDoc;
        }
    }
}
