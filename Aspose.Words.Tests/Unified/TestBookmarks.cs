// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Collections.Generic;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for bookmarks.
    /// </summary>
    [TestFixture]
    public class TestBookmarks : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarks(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestBookmarks", lf, sf);

            NodeList starts = doc.SelectNodes("//BookmarkStart");
            NodeList ends = doc.SelectNodes("//BookmarkEnd");

            // Check bookmarks are present.
            Assert.That(starts.Count, Is.EqualTo(3));
            Assert.That(ends.Count, Is.EqualTo(3));

            // Check bookmarks have names and start and end where expected.
            BookmarkStart bmkStart = (BookmarkStart)starts[0];
            Assert.That(bmkStart.Name, Is.EqualTo("TestBookmark1"));
            Assert.That(bmkStart.NextSibling.GetText().IndexOf("starts in this") != -1, Is.True);

            BookmarkEnd bmkEnd = (BookmarkEnd)ends[0];
            Assert.That(bmkEnd.Name, Is.EqualTo("TestBookmark2"));
            Assert.That(bmkEnd.NextSibling.GetText().IndexOf("and another") != -1, Is.True);

            // This bookmark is inside bookmark 1.
            bmkStart = (BookmarkStart)starts[1];
            Assert.That(bmkStart.Name, Is.EqualTo("TestBookmark2"));
            Assert.That(bmkStart.NextSibling.GetText(), Is.EqualTo("paragraph "));

            // This bookmark is related to the form field.
            bmkStart = (BookmarkStart)starts[2];
            Assert.That(bmkStart.Name, Is.EqualTo("Text1"));
            Assert.That(bmkStart.NextSibling.GetText(), Is.EqualTo(" FORMTEXT "));
        }


        private static void CheckBookmarksSamePosition(Document doc, bool isBookmarkExpectedAtBlockLevel)
        {
            NodeList starts = doc.SelectNodes("//BookmarkStart");
            NodeList ends = doc.SelectNodes("//BookmarkEnd");

            // Check bookmarks are present.
            Assert.That(starts.Count, Is.EqualTo(3));
            Assert.That(ends.Count, Is.EqualTo(3));

            Paragraph firstParagraph = doc.FirstSection.Body.Paragraphs[0];

            BookmarkStart bmkStart1 = (BookmarkStart)starts[0];
            Assert.That(bmkStart1.Name, Is.EqualTo("Section1"));
            Assert.That(bmkStart1.ParentNode, Is.SameAs(firstParagraph));
            BookmarkStart bmkStart2 = (BookmarkStart)starts[1];
            Assert.That(bmkStart2.Name, Is.EqualTo("Section1And2"));
            Assert.That(bmkStart2.ParentNode, Is.SameAs(firstParagraph));

            // Start of Bmk2 is right after Bmk1.
            Assert.That(bmkStart1.NextSibling == bmkStart2, Is.True);
            Assert.That(bmkStart2.NextSibling.GetText().IndexOf("Section 1.") != -1, Is.True);

            BookmarkEnd bmkEnd2 = (BookmarkEnd)ends[1];
            Assert.That(bmkEnd2.Name, Is.EqualTo("Section1And2"));
            BookmarkEnd bmkEnd3 = (BookmarkEnd)ends[2];
            Assert.That(bmkEnd3.Name, Is.EqualTo("Section2"));

            // End of Bmk3 is right after end of Bmk2.
            Assert.That(bmkEnd2.NextSibling == bmkEnd3, Is.True);
            if (isBookmarkExpectedAtBlockLevel)
            {
                Assert.That(bmkEnd3.ParentNode.NodeType, Is.EqualTo(NodeType.Body));
                Assert.That(bmkEnd3.NextSibling, Is.SameAs(doc.LastSection.Body.LastChild));
            }
            else
            {
                Assert.That(bmkEnd3.NextSibling == null, Is.True);
                Assert.That(bmkEnd3.ParentNode, Is.SameAs(doc.LastSection.Body.LastChild));
            }
        }






        /// <summary>
        /// Test bookmarks in different stories of the document are supported.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarksInStories(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestBookmarksInStories", lf, sf);

            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(6));
            Assert.That(doc.Range.Bookmarks["BmkHeader"].Text, Is.EqualTo("Header"));
            // Note that if a bookmark in body encompasses a textbox, then the text of the textbox will
            // be included in the text of the bookmark. This is not really nice, but I cannot change now.
            Assert.That(doc.Range.Bookmarks["BmkBody"].Text, Is.EqualTo("Body"));
            Assert.That(doc.Range.Bookmarks["BmkTextbox"].Text, Is.EqualTo("Textbox"));
            Assert.That(doc.Range.Bookmarks["BmkEndnote"].Text, Is.EqualTo("Endnote"));
            Assert.That(doc.Range.Bookmarks["BmkFootnote"].Text, Is.EqualTo("Footnote"));
            Assert.That(doc.Range.Bookmarks["BmkComment"].Text, Is.EqualTo("Comment"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarkSetText(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestBookmarkSetText", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("This line contains a bookmark.\x000c"));
            doc.Range.Bookmarks["bmk"].Text = "";
            Assert.That(doc.GetText(), Is.EqualTo("This line contains a .\x000c"));
            doc.Range.Bookmarks["bmk"].Text = "hello";
            Assert.That(doc.GetText(), Is.EqualTo("This line contains a hello.\x000c"));
            Assert.That(doc.Range.Bookmarks["bmk"].Text, Is.EqualTo("hello"));

            TestUtil.SaveOpen(doc, @"Model\TestBookmarkSetText Modified", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarkRename(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestBookmarkRename", lf, sf);
            doc.Range.Bookmarks["Test"].Name = "XXX";
            doc = TestUtil.SaveOpen(doc, @"Model\TestBookmarkRename Modified", lf, sf);
            Assert.That(doc.Range.Bookmarks["Test"], Is.Null);
            Assert.That(doc.Range.Bookmarks["XXX"], IsNot.Null());
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarkRemove(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\TestBookmarks", lf);
            doc.Range.Bookmarks.Remove("testbookmark1");
            doc.Range.Bookmarks.Remove("Text1");
            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestBookmarkRemove", lf, sf);
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(1));
            Assert.That(doc.Range.Bookmarks["TestBookmark2"].Text, Is.EqualTo("paragraph "));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarkRemoveAll(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\TestBookmarks", lf);
            doc.Range.Bookmarks.Clear();
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Adjacent bookmarks are overlapped in the model, check that setting text does not delete overlapped bookmarks.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarkAdjacent(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestBookmarkAdjacent", lf, sf);
            doc.Range.Bookmarks["bmk1"].Text = "XXX";
            doc.Range.Bookmarks["bmk2"].Text = "YYY";

            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestBookmarkAdjacent Modified", lf, sf);
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.GetText(), Is.EqualTo("XXXYYY\x000c"));
        }


        /// <summary>
        /// There was a problem when font size was changed after setting bookmark text.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBookmarkFontSize(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestBookmarkFontSize", lf, sf);
            doc.Range.Bookmarks["bmk1"].Text = "Hello1";
            doc.Range.Bookmarks["bmk2"].Text = "Hello2";

            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestBookmarkFontSize Modified", lf, sf);

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);

            // Existing run of text before the bookmark - it was influencing the change of the font size.
            Run run = (Run)runs[0];
            Assert.That(run.Text, Is.EqualTo("OldText1"));
            Assert.That(run.Font.Size, Is.EqualTo(18.0));

            // New runs of text that have correct font sizes now.
            run = (Run)runs[1];
            Assert.That(run.Text, Is.EqualTo("Hello1"));
            Assert.That(run.Font.Size, Is.EqualTo(12.0));

            run = (Run)runs[2];
            Assert.That(run.Text, Is.EqualTo("Hello2"));
            Assert.That(run.Font.Size, Is.EqualTo(12.0));
        }

        /// <summary>
        /// Start and end of a bookmark are inline.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInline(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestInline", lf, sf);

            BookmarkStart bs = (BookmarkStart)doc.GetChild(NodeType.BookmarkStart, 0, true);
            Assert.That(bs.PreviousSibling.GetText(), Is.EqualTo("Te"));

            Node node = bs.NextSibling;
            Assert.That(node.GetText(), Is.EqualTo("xt with a boo"));

            node = node.NextSibling;
            Assert.That(node.NodeType, Is.EqualTo(NodeType.BookmarkEnd));

            node = node.NextSibling;
            Assert.That(node.GetText(), Is.EqualTo("kmark."));
        }

        /// <summary>
        /// Adjacent bookmarks are overlapped in the model, check that setting text does not delete overlapped bookmarks.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInlineAdjacent(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestInlineAdjacent", lf, sf);

            BookmarkStart bmk1Start = (BookmarkStart)doc.GetChild(NodeType.BookmarkStart, 0, true);
            Assert.That(bmk1Start.Name, Is.EqualTo("bmk1"));

            // bmk2 start is before bmk1 at the moment.
            BookmarkStart bmk2Start = (BookmarkStart)bmk1Start.NextSibling.NextSibling;
            Assert.That(bmk2Start.Name, Is.EqualTo("bmk2"));

            BookmarkEnd bmk1End = (BookmarkEnd)bmk2Start.NextSibling;
            Assert.That(bmk1End.Name, Is.EqualTo("bmk1"));

            bmk1Start.Bookmark.Text = "Hello!";

            // Check that setting bmk1 text has untangled the bookmarks. bmk2 now starts after bmk1 ends.
            Assert.That(bmk2Start, Is.EqualTo(bmk1End.NextSibling));
        }

        /// <summary>
        /// When bookmark end is next sibling of a paragraph, in inline bookmarks mode,
        /// the bookmark end is moved in the model to the beginning of the next paragraph.
        /// In block bookmarks mode, the position of the bookmark is preserved.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestIncludeEndOfPara(LoadFormat lf, SaveFormat sf)
        {
            bool isBookmarkSupportedAtBlockLevel =
                ((lf == LoadFormat.Docx) || (lf == LoadFormat.WordML)) &&
                ((sf == SaveFormat.Docx) || (sf == SaveFormat.WordML));

            Document doc = OpenSaveOpen(@"Model\Bookmark\TestIncludeEndOfPara", lf, sf,
                isBookmarkSupportedAtBlockLevel ? "_BlockBm" : "", 1);
            CheckIncludeEndOfPara(doc, isBookmarkSupportedAtBlockLevel);
        }

        private static void CheckIncludeEndOfPara(Document doc, bool isBookmarkExpectedAtBlockLevel)
        {
            BookmarkStart bs = (BookmarkStart)doc.GetChild(NodeType.BookmarkStart, 0, true);
            Assert.That(bs.Name, Is.EqualTo("bmk"));

            Node node = bs.NextSibling;
            Assert.That(node.NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(node.GetText(), Is.EqualTo("paragraph."));
            Assert.That(node.NextSibling, Is.EqualTo(null));    //No more nodes after this run.

            if (isBookmarkExpectedAtBlockLevel)
            {
                //The end of bookmark is next to paragraph of the bookmark start.
                Assert.That(bs.ParentNode.NextSibling.NodeType, Is.EqualTo(NodeType.BookmarkEnd));
            }
            else
            {
                //The end of bookmark is at the start of the next paragraph.
                Paragraph para = (Paragraph)node.ParentNode.NextSibling;
                Assert.That(para.FirstChild.NodeType, Is.EqualTo(NodeType.BookmarkEnd));
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestExcludeEndOfPara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestExcludeEndOfPara", lf, sf);

            BookmarkStart bs = (BookmarkStart)doc.GetChild(NodeType.BookmarkStart, 0, true);
            Assert.That(bs.Name, Is.EqualTo("bmk"));
            Assert.That(bs.PreviousSibling, Is.EqualTo(null)); //Bookmark start is the first item in the paragraph.

            Node node = bs.NextSibling.NextSibling;    //Skip the run, jump to the bookmark end.
            Assert.That(node.NodeType, Is.EqualTo(NodeType.BookmarkEnd));
            Assert.That(node.NextSibling, Is.EqualTo(null));    //Bookmark end is the last child in the paragraph.
        }

        /// <summary>
        /// Setting text of a bookmark that starts at the end of a paragraph used to throw.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSetTextAtEndOfPara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestSetTextAtEndOfPara", lf, sf);
            doc.Range.Bookmarks["bmk"].Text = "";
            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestSetTextAtEndOfPara Modified", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("Para1\x000c"));
        }

        /// <summary>
        /// WORDSNET-760 Setting Bookmark.Text property to string containing CRLF characters causes crash
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSetTextWithPara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestSetTextWithPara", lf, sf);
            doc.Range.Bookmarks["bmk"].Text = "XXX\rYYY";
            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestSetTextWithPara Modified", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("PaXXX\rYYYra2\x000c"));
            Assert.That(doc.GetChildNodes(NodeType.Paragraph, true).Count, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTableColumnBookmark(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bookmark\TestTableColumnBookmark", lf, sf);
            BookmarkStart bookmarkStart = doc.Range.Bookmarks["TableColumnBookmark"].BookmarkStart;

            Assert.That(bookmarkStart.IsColumn, Is.True);
            Assert.That(bookmarkStart.FirstColumn, Is.EqualTo(1));
            Assert.That(bookmarkStart.LastColumn, Is.EqualTo(3));
        }



        /// <summary>
        /// Creates new column using as a template the cloned cell with bookmark inside.
        /// </summary>
        internal Document CreateSrcDoc()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\TestJira9413.docx");

            DocumentBuilder documentBuilder = new DocumentBuilder(doc);
            documentBuilder.MoveToBookmark("IncomeStatement");

            Table table = (Table)documentBuilder.CurrentNode.GetAncestor(NodeType.Table);

            Cell hCell = (Cell)table.FirstRow.FirstCell.Clone(true);
            table.FirstRow.AppendChild(hCell);

            documentBuilder.MoveTo(hCell.FirstParagraph);
            documentBuilder.Write("NewColumn");

            return documentBuilder.Document;
        }

        /// <summary>
        /// WORDSNET-10653 System.ArgumentException occurs after appending one document to another.
        /// The problem occued because there are two bookmarks with the same name in different stories.
        /// Made code recilient.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect10653(LoadFormat lf, SaveFormat sf)
        {
            const string bookmarkName = "testBookmark";
            const string bookmarkName1 = "testBookmark1";

            DocumentBuilder builder = new DocumentBuilder();

            // To replicate the issue we should insert two bookmarks with the same name in different stories.
            // For instance, let's insert one bookmark in the main story and another in the header.
            builder.StartBookmark(bookmarkName);
            builder.EndBookmark(bookmarkName);
            builder.Writeln();

            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.StartBookmark(bookmarkName);
            builder.EndBookmark(bookmarkName);

            // Test story nesting.
            // Insert one bookmark in normal text and another in shape.
            builder.MoveToDocumentEnd();
            builder.StartBookmark(bookmarkName1);
            builder.Writeln();
            builder.Writeln();
            builder.Writeln();
            builder.EndBookmark(bookmarkName1);

            Shape textbox = new Shape(builder.Document, ShapeType.TextBox);
            textbox.Width = 100;
            textbox.Height = 100;
            textbox.WrapType = WrapType.Inline;
            Paragraph paragraph = new Paragraph(builder.Document);
            textbox.AppendChild(paragraph);

            ((Paragraph)builder.CurrentParagraph.PreviousSibling).AppendChild(textbox);
            builder.MoveTo(paragraph);
            builder.StartBookmark(bookmarkName1);
            builder.Write("This is text inside textbox");
            builder.EndBookmark(bookmarkName1);

            Assert.That(builder.Document.Range.Bookmarks.Count, Is.EqualTo(4));
            Assert.That(builder.Document.Range.Bookmarks[0].Name, Is.EqualTo(bookmarkName));
            Assert.That(builder.Document.Range.Bookmarks[1].Name, Is.EqualTo(bookmarkName1));
            Assert.That(builder.Document.Range.Bookmarks[2].Name, Is.EqualTo(bookmarkName1));
            Assert.That(builder.Document.Range.Bookmarks[3].Name, Is.EqualTo(bookmarkName));

            Document doc = TestUtil.SaveOpen(builder.Document, @"Model\Bookmark\TestDefect10653", lf, sf);

            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.Range.Bookmarks[0].Name, Is.EqualTo(bookmarkName));
            Assert.That(doc.Range.Bookmarks[1].Name, Is.EqualTo(bookmarkName1));
        }


        private static void CheckBookmarksAtRowEnd(Document doc, bool isBookmarkExpectedAtBlockLevel)
        {
            // This bookmark is before the row break character in the DOC file.
            Bookmark bmk = doc.Range.Bookmarks["RowETA1End"];
            Assert.That(bmk.Text, Is.EqualTo(""));

            // Check that the bookmark is in the appropriate place in the model.
            if (isBookmarkExpectedAtBlockLevel)
            {
                Cell cell = (Cell)bmk.BookmarkStart.PreviousSibling;
                Assert.That(cell.GetText(), Is.EqualTo("___________________\x0007"));
            }
            else
            {
                Cell cell = (Cell)bmk.BookmarkStart.ParentNode.ParentNode;
                Assert.That(cell.GetText(), Is.EqualTo("\x0013 FORMTEXT \x0001\x0014Ten (10) years\x0015\x0007"));
            }
        }

        /// <summary>
        /// Opens, saves and then opens document again with the specified parameters and with gold checking.
        /// </summary>
        private static Document OpenSaveOpen(string fileName, LoadFormat lf, SaveFormat sf,
            string goldFileNamePostfix, int expectedBookmarkCount)
        {
            Document doc = TestUtil.Open(fileName, lf);

            doc = TestUtil.SaveOpen(doc, fileName + goldFileNamePostfix, lf, sf);

            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(expectedBookmarkCount));

            return doc;
        }

        /// <summary>
        /// WORDSNET-4691 InvalidCastException occurs during Bookmark processing.
        /// The problem occurred because bookmark ends in SmartTag,
        /// i.e. start and end of bookmark are on different levels in the documents hierarchy.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect4691(LoadFormat lf, SaveFormat sf)
        {
            const string expectedContent = @"per Test in het";

            Document doc = TestUtil.Open(@"Model\Bookmark\TestDefect4691", lf);

            doc.Range.Bookmarks["ArtStartDate"].Text = "Test";

            Assert.That(doc.ToString(SaveFormat.Text).Trim(), Is.EqualTo(expectedContent));

            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestDefect4691", lf, sf);

            Assert.That(doc.ToString(SaveFormat.Text).Trim(), Is.EqualTo(expectedContent));

            // Make sure bookmark is still there.
            Assert.That(doc.Range.Bookmarks["ArtStartDate"].Text, Is.EqualTo("Test"));
        }


        /// <summary>
        /// Tests node level of a bookmark in a textbox.
        /// </summary>
        [Test]
        public void TestBlockLevelBookmarkInTextbox()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestJira15785_DeletedTextBox.docx");
            Bookmark bm = doc.Range.Bookmarks["_GoBack"];
            Assert.That(bm.BookmarkEnd.NodeLevel, Is.EqualTo(NodeLevel.Block));
        }


        /// <summary>
        /// Relates to WORDSNET-11024
        /// Slightly complex case when main story has bookmark which is duplicated in DrawingML and AlternateContent.
        /// </summary>
        [Test]
        public void TestJira11024A()
        {
            // Source document has DrawingML which contains two bookmarks having 'Test' and 'Test2' names.
            Document doc = TestUtil.Open(@"Model\Bookmark\TestJira11024A.docx");

            // Insert one more bookmark having 'Test2' name into main document body.
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.StartBookmark("Test2");
            builder.Write("New Test2 Bookmark");
            builder.EndBookmark("Test2");

            // Insert one more bookmark having 'Test' name into main document body.
            doc.FirstSection.Body.Paragraphs.Add(new Paragraph(doc));
            builder.MoveToDocumentEnd();
            builder.StartBookmark("Test");
            builder.Write("New Test Bookmark");
            builder.EndBookmark("Test");

            // Let validators work.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            // 'Test2' bookmark should be removed from main document body but 'Test' bookmark should be preserved.
            Assert.That(doc.FirstSection.Body.FirstParagraph.Range.Bookmarks["Test2"], Is.Null);
            Assert.That(doc.FirstSection.Body.LastParagraph.Range.Bookmarks["Test"], IsNot.Null());

            // 'Test' bookmark should be removed from both DrawingML and AlternateContent but 'Test2' bookmark should be preserved.
            Shape dml = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(dml.Range.Bookmarks["Test"], Is.Null);
            // FOSS no fallback shapes

            Assert.That(dml.Range.Bookmarks["Test2"], IsNot.Null());
            // FOSS no fallback shapes
        }

        /// <summary>
        /// WORDSNET-11558 BookmarkStart.remove() causes an IllegalStateException
        /// Made bookmark fetcher resilient to deletion of bookmark start.
        /// </summary>
        [Test]
        public void TestJira11558()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\TestJira11558.docx");

            BookmarkCollection bookmarks = doc.Range.Bookmarks;
            Bookmark bk = bookmarks[0];

            bk.BookmarkStart.Remove(); // remove the BookmarkStart

            BookmarkEnd end = bk.BookmarkEnd;
            Assert.That(end.Name, Is.EqualTo("bm"));
        }

        /// <summary>
        /// WORDSNET-12492 Allow creation of bookmarks over 40 chars long when the target document is PDF.
        /// This would allow to create long meaningful bookmark names for Pdf format,
        /// and perhaps less human-readable ones for another.
        /// </summary>
        [Test]
        public void TestJira12492()
        {
            const string fieldTextOrig = "FieldText";
            const string bookmarkText = "Text inside a bookmark.";
            const string longBookmarkName = "MyBookmarkMyBookmarkMyBookmarkMyBookmarkMyBookmark";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create bookmark.
            builder.StartBookmark(longBookmarkName);
            builder.Writeln(bookmarkText);
            builder.EndBookmark(longBookmarkName);

            // Append "Ref" field.
            Paragraph p = builder.InsertParagraph();
            FieldRef fieldRef = (FieldRef)p.AppendField(FieldType.FieldRef, false);
            fieldRef.BookmarkName = longBookmarkName;
            fieldRef.Result = fieldTextOrig;

            // Append "Unknown" field.
            p = builder.InsertParagraph();
            p.AppendChild(new FieldStart(doc, new RunPr(), FieldType.FieldRefNoKeyword));
            p.AppendChild(new Run(doc, longBookmarkName));
            p.AppendChild(new FieldEnd(doc, new RunPr(), FieldType.FieldRefNoKeyword, false));

            // Update fields.
            doc.UpdateFields();

            // Check bookmarks can be found after document fields were updated.
            CheckRefFieldText(doc, bookmarkText);
        }

        /// <summary>
        /// WORDSNET-14408 Deleting items of <see cref="BookmarkCollection"/> in the foreach
        /// cycle is supported now.
        /// </summary>
        [Test]
        public void TestJira14408()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create bookmarks
            for (int i = 0; i < 5; i++)
            {
                string bookmarkName = "Bookmark" + i.ToString();
                builder.StartBookmark(bookmarkName);
                builder.Writeln(bookmarkName);
                builder.EndBookmark(bookmarkName);
            }

            // Delete bookmarks
            foreach (Bookmark bookmark in doc.Range.Bookmarks)
                bookmark.Remove();

            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(0));
        }


        /// <summary>
        /// WORDSNET-15823 Deletion issue of bookmarks with duplicate name.
        /// Ends of bookmarks with duplicated names point to the same <see cref="BookmarkEnd"/>. It causes the
        /// exception while deleting. Ends of bookmarks selection algorithm is changed to fix the problem.
        /// </summary>
        [Test]
        public void TestJira15823()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            // Insert duplicated bookmarks.
            builder.StartBookmark("test1");
            builder.EndBookmark("test1");
            builder.StartBookmark("test1");
            builder.EndBookmark("test1");

            // Execute "BookmarkFinder" to check updated algorithm.
            BookmarkStart start1 = bookmarks[0].BookmarkStart;
            BookmarkEnd end1 = bookmarks[0].BookmarkEnd;
            BookmarkStart start2 = bookmarks[1].BookmarkStart;
            BookmarkEnd end2 = bookmarks[1].BookmarkEnd;

            // Check that we get different "end" nodes for duplicated bookmarks.
            Assert.That(end2.Name, Is.EqualTo(end1.Name));
            Assert.That(start2.Name, Is.EqualTo(start1.Name));
            Assert.That(ReferenceEquals(start1, start2), Is.False);
            Assert.That(ReferenceEquals(end1, end2), Is.False);

            // Try to remove and check that exception does not occur.
            start1.Remove();
            end1.Remove();
            start2.Remove();
            end2.Remove();

            Assert.That(bookmarks.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// This case concern with WORDSNET-15823 issue.
        /// Last occurred <see cref="BookmarkEnd"/> has to be returned when appropriate end of
        /// bookmark is not found.
        /// </summary>
        [Test]
        public void TestJira15823ManyStarts()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            builder.StartBookmark("test1");
            builder.StartBookmark("test1");
            builder.EndBookmark("test1");
            builder.StartBookmark("test1");
            builder.EndBookmark("test1");

            Assert.That(bookmarks.Count, Is.EqualTo(3));
            BookmarkStart start1 = bookmarks[0].BookmarkStart;
            BookmarkEnd end1 = bookmarks[0].BookmarkEnd;

            BookmarkStart start2 = bookmarks[1].BookmarkStart;
            BookmarkEnd end2 = bookmarks[1].BookmarkEnd;

            BookmarkStart start3 = bookmarks[2].BookmarkStart;
            BookmarkEnd end3 = bookmarks[2].BookmarkEnd;

            Assert.That(start1, IsNot.Null());
            Assert.That(start2, IsNot.Null());
            Assert.That(start3, IsNot.Null());
            Assert.That(end1, IsNot.Null());
            Assert.That(end2, IsNot.Null());
            Assert.That(end3, IsNot.Null());
            Assert.That(ReferenceEquals(start1, start2), Is.False);
            Assert.That(ReferenceEquals(start2, start3), Is.False);
            Assert.That(ReferenceEquals(end1, end2), Is.False);
            Assert.That(ReferenceEquals(end2, end3), Is.True);
        }

        /// <summary>
        /// This case concern with WORDSNET-15823 issue.
        /// Checks case when number of <see cref="BookmarkEnd"/> is greater than
        /// number of <see cref="BookmarkStart"/>.
        /// </summary>
        [Test]
        public void TestJira15823ManyEnds()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            builder.StartBookmark("test1");
            builder.EndBookmark("test1");
            BookmarkEnd wrongEnd = builder.EndBookmark("test1");
            builder.StartBookmark("test1");
            BookmarkEnd validEnd = builder.EndBookmark("test1");

            Assert.That(bookmarks.Count, Is.EqualTo(2));
            BookmarkStart start1 = bookmarks[0].BookmarkStart;
            BookmarkEnd end1 = bookmarks[0].BookmarkEnd;

            BookmarkStart start2 = bookmarks[1].BookmarkStart;
            BookmarkEnd end2 = bookmarks[1].BookmarkEnd;

            Assert.That(ReferenceEquals(start1, start2), Is.False);
            Assert.That(ReferenceEquals(end1, end2), Is.False);
            Assert.That(ReferenceEquals(wrongEnd, end2), Is.False);
            Assert.That(ReferenceEquals(validEnd, end2), Is.True);
        }

        /// <summary>
        /// This case concern with WORDSNET-15823 issue.
        /// Checks that AW does not take in attention <see cref="BookmarkStart"/>
        /// followed after referenced <see cref="BookmarkStart"/>.
        /// </summary>
        [Test]
        public void TestJira15823DifferentNames()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            builder.StartBookmark("test1");
            builder.StartBookmark("test2");
            BookmarkEnd validEnd1 = builder.EndBookmark("test1");
            BookmarkEnd validEnd2 = builder.EndBookmark("test2");
            builder.EndBookmark("test2");

            Assert.That(bookmarks.Count, Is.EqualTo(2));

            BookmarkEnd end1 = bookmarks[0].BookmarkEnd;
            BookmarkEnd end2 = bookmarks[1].BookmarkEnd;

            Assert.That(ReferenceEquals(validEnd1, end1), Is.True);
            Assert.That(ReferenceEquals(validEnd2, end2), Is.True);
        }

        /// <summary>
        /// This case concern with WORDSNET-15823 issue.
        /// Checks that search for <see cref="BookmarkStart"/> does not interrupt in unexpected place.
        /// </summary>
        [Test]
        public void TestJira15823StartSearch()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            BookmarkEnd end1 = builder.EndBookmark("test1");
            builder.StartBookmark("test1");

            BookmarkStart start1 = BookmarkFinder.FindBookmarkStart(builder.Document, "test1");
            Assert.That(start1, IsNot.Null());
            Assert.That(ReferenceEquals(start1, end1), Is.False);
        }

        /// <summary>
        /// This case concern with WORDSNET-15823 issue.
        /// Checks bookmark starts for document without <see cref="BookmarkEnd"/>.
        /// </summary>
        [Test]
        public void TestJira15823NoEndsA()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            builder.StartBookmark("test1");
            builder.StartBookmark("test1");

            Assert.That(bookmarks.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// This case concern with WORDSNET-15823 issue.
        /// Checks case with attempt to obtain the end of bookmark using <see cref="BookmarkFinder"/>
        /// directly for document where <see cref="BookmarkEnd"/> does not exist.
        /// </summary>
        [Test]
        public void TestJira15823NoEndsB()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            BookmarkCollection bookmarks = builder.Document.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(0));

            BookmarkStart start1 = builder.StartBookmark("test1");
            BookmarkEnd end1 = BookmarkFinder.FindBookmarkEnd(builder.Document, "test1");
            Assert.That(end1, Is.Null);

            end1 = BookmarkFinder.FindBookmarkEnd(builder.Document, "test1", start1);
            Assert.That(end1, Is.Null);
        }


        /// <summary>
        /// WORDSNET-15347 Bookmark.Text throws System.InvalidOperationException.
        /// BookmarkEnd is outside of cloned table, so bookmark becomes invalid.
        /// Fixed per WORDSNET-721, which allows bookmarks at the table level.
        /// </summary>
        [Test]
        public void TestJira15347A()
        {
            // There are two tables in the testing document. First of them has BookmarkEnd between last row and table end.
            // The second one has BookmarkEnd outside of the table (inside paragraph next to the table).
            Document doc = TestUtil.Open(@"Model\Bookmark\TestJira15347.docx");

            // First table range should return valid bookmark for both: original and cloned instance.
            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.Range.Bookmarks.Count, Is.EqualTo(1));
            Assert.That(table.Range.Bookmarks[0].Text, IsNot.Null());

            Table clonedTable = (Table)table.Clone(true);
            Assert.That(clonedTable.Range.Bookmarks.Count, Is.EqualTo(1));
            Assert.That(clonedTable.Range.Bookmarks[0].Text, IsNot.Null());
        }

        /// <summary>
        /// Relates to WORDSNET-15347
        /// Checks that missed BookmarkEnd throws an exception.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException),
            ExpectedMessage = "Cannot find bookmark 'OI_TABLE_SUB1' in the document.")]
        public void TestJira15347B()
        {
            // The same document as in TestJira15347A, but we check here clone of the second table,
            // where there is no valid BookmarkEnd.
            Document doc = TestUtil.Open(@"Model\Bookmark\TestJira15347.docx");

            Table clonedTable = (Table)doc.FirstSection.Body.Tables[1].Clone(true);
            Assert.That(clonedTable.Range.Bookmarks[0].Text, Is.Null);
        }



        /// <summary>
        /// Relates to WORDSNET-16667 Checks that BookmarkCollection is updating in real-time.
        /// </summary>
        [Test]
        public void TestJira16667()
        {
            const int bookmarksCount = 5;

            Document doc = BookmarkTestUtil.CreateDocumentWithBookmarks(bookmarksCount);

            BookmarkCollection bookmarks = doc.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(bookmarksCount));

            BookmarkTestUtil.AppendBookmark(doc, "anotherOneBookmark");
            Assert.That(bookmarks.Count, Is.EqualTo(bookmarksCount + 1));

            bookmarks.Remove(bookmarks[0].Name);
            Assert.That(bookmarks.Count, Is.EqualTo(bookmarksCount));
        }


        /// <summary>
        /// WORDSNET-17030 Document.Clone method keeps the reference on original document.
        /// We need to clear cache of Range when cloning nodes.
        /// </summary>
        [Test]
        public void TestJira17030()
        {
            const int bookmarksCount = 10;
            Document doc = BookmarkTestUtil.CreateDocumentWithBookmarks(bookmarksCount);

            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(bookmarksCount));

            Document clonedDoc = doc.Clone();
            clonedDoc.Range.Bookmarks.Clear();
            Assert.That(clonedDoc.Range.Bookmarks.Count, Is.EqualTo(0));
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(bookmarksCount));

            BookmarkTestUtil.AppendBookmark(doc, "anotherOneBookmarkInOriginal");
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(bookmarksCount + 1));
            Assert.That(clonedDoc.Range.Bookmarks.Count, Is.EqualTo(0));

            BookmarkTestUtil.AppendBookmark(clonedDoc, "BookmarkInCloned");
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(bookmarksCount + 1));
            Assert.That(clonedDoc.Range.Bookmarks.Count, Is.EqualTo(1));
        }



        /// <summary>
        /// WORDSNET-12678 Implemented API to get information related to a table column bookmark.
        /// </summary>
        [Test]
        public void TestTableColumnBookmarkApi()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\TestTableColumnBookmark.docx");
            Bookmark bookmark = doc.Range.Bookmarks["TableColumnBookmark"];

            Assert.That(bookmark.IsColumn, Is.True);
            Assert.That(bookmark.FirstColumn, Is.EqualTo(1));
            Assert.That(bookmark.LastColumn, Is.EqualTo(3));
        }


        /// <summary>
        /// WORDSNET-17309 Because of moving the bookmark to the inline level its position became wrong:
        /// its start node with defined attribute displacedByCustomXml="next" was moved as child of the SDT and became
        /// referring to its end. The issue has been fixed on implementation of WORDSNET-721, in which bookmarks are
        /// allowed to be on the block/cell/row levels.
        /// </summary>
        [Test]
        public void TestJira17309()
        {
            Document document = TestUtil.Open(@"Model\Bookmark\TestJira17309.docx");
            Node sdt = document.GetChild(NodeType.StructuredDocumentTag, 1, true);

            Node previousNode = sdt.PreviousSibling;
            Assert.That(previousNode.NodeType, Is.EqualTo(NodeType.BookmarkStart));
            Assert.That(((BookmarkStart)previousNode).Name, Is.EqualTo("test2"));

            Node nextNode = sdt.NextSibling;
            Assert.That(nextNode.NodeType, Is.EqualTo(NodeType.BookmarkEnd));
            Assert.That(((BookmarkEnd)nextNode).Name, Is.EqualTo("test2"));
        }

        /// <summary>
        /// Compare REF fields result with expected value.
        /// </summary>
        /// <param name="doc">Document with bookmark.</param>
        /// <param name="bookmarkText">Expected text in field result.</param>
        private static void CheckRefFieldText(Document doc, string bookmarkText)
        {
            ParagraphCollection ps = doc.FirstSection.Body.Paragraphs;

            Run fieldRun = GetFieldResultRun((Paragraph)ps[2]);
            Assert.That(fieldRun.Text, Is.EqualTo(bookmarkText));
            fieldRun = GetFieldResultRun((Paragraph)ps[4]);
            Assert.That(fieldRun.Text, Is.EqualTo(bookmarkText));
        }

        /// <summary>
        /// WORDSNET-18882 Ability to insert column bookmarks has been implemented in the public API.
        /// </summary>
        [Test]
        public void TestColumnBookmarkInsertion()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();

            builder.InsertCell();
            builder.StartColumnBookmark("Bookmark0");
            builder.StartColumnBookmark("Bookmark1");
            builder.Write("Cell 1");
            builder.EndColumnBookmark("Bookmark1");

            builder.InsertCell();
            builder.Write("Cell 2");
            builder.StartColumnBookmark("Bookmark2");
            builder.EndColumnBookmark("Bookmark2");

            builder.InsertCell();
            builder.StartColumnBookmark("Bookmark3");
            builder.EndColumnBookmark("Bookmark3");
            builder.Write("Cell 3");

            builder.EndRow();

            builder.InsertCell();
            builder.StartColumnBookmark("Bookmark4");
            builder.Write("Cell 4");
            builder.StartColumnBookmark("Bookmark5");

            builder.InsertCell();
            builder.EndColumnBookmark("Bookmark5");
            builder.EndColumnBookmark("Bookmark0");
            builder.Write("Cell 5");

            builder.InsertCell();
            builder.Write("Cell 6");
            builder.EndColumnBookmark("Bookmark4");

            builder.EndRow();

            builder.InsertCell();
            builder.Write("Cell 7");

            builder.InsertCell();
            builder.Write("Cell 8");

            builder.InsertCell();
            builder.Write("Cell 9");

            builder.EndTable();

            builder.MoveToCell(0, 2, 0, 0);
            builder.StartColumnBookmark("Bookmark6");
            builder.MoveToCell(0, 2, 2, -1);
            builder.EndColumnBookmark("Bookmark6");

            TestUtil.Save(doc, @"Model\Bookmark\TestColumnBookmarkInsertion.docx", null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-18882 Tests column bookmark behaviour on insertion to a table with merged cells.
        /// </summary>
        [Test]
        public void TestColumnBookmarkInMergedCells()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = builder.StartTable();

            for (int i = 1; i <= 2; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    builder.InsertCell();
                    builder.Write(string.Format("Cell{0}{1}", i, j));
                }

                builder.EndRow();
            }

            builder.EndTable();

            Row row = table.Rows[1];
            row.Cells[0].CellFormat.HorizontalMerge = CellMerge.First;
            row.Cells[1].CellFormat.HorizontalMerge = CellMerge.Previous;
            row.Cells[2].CellFormat.HorizontalMerge = CellMerge.First;
            row.Cells[3].CellFormat.HorizontalMerge = CellMerge.Previous;

            // Test bookmark in cells with HorizontalMerge.
            builder.MoveToCell(0, 0, 0, 0);
            builder.Write("Bookmark1Start");
            builder.StartColumnBookmark("Bookmark1");
            builder.MoveToCell(0, 1, 2, -1);
            builder.Write("Bookmark1End");
            builder.EndColumnBookmark("Bookmark1");

            // Resave to get merged cells formatted as GridSpan.
            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\TestColumnBookmarkInMergedCells",
                UnifiedScenario.Docx2DocxNoGold);

            builder = new DocumentBuilder(doc);

            // Test bookmark in cells with GridSpan.
            builder.MoveToCell(0, 0, 0, 0);
            builder.Write("Bookmark2Start");
            builder.StartColumnBookmark("Bookmark2");
            builder.MoveToCell(0, 1, 1, -1);
            builder.Write("Bookmark2End");
            builder.EndColumnBookmark("Bookmark2");

            Bookmark bm1 = doc.Range.Bookmarks["Bookmark1"];
            Bookmark bm2 = doc.Range.Bookmarks["Bookmark2"];
            BookmarkStart bm1Start = bm1.BookmarkStart;
            BookmarkStart bm2Start = bm2.BookmarkStart;
            table = doc.FirstSection.Body.Tables[0];

            CompositeNode bm1StartParent = bm1Start.ParentNode;
            Assert.That(bm1StartParent.NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(bm1StartParent.ParentNode, Is.SameAs(table.Rows[0].Cells[0]));
            Assert.That(bm2Start.ParentNode, Is.SameAs(bm1StartParent));

            Assert.That(bm1Start.FirstColumn, Is.EqualTo(0));
            Assert.That(bm1Start.LastColumn, Is.EqualTo(1));
            Assert.That(bm2Start.FirstColumn, Is.EqualTo(0));
            Assert.That(bm2Start.LastColumn, Is.EqualTo(1));

            Assert.That(bm1.BookmarkEnd.ParentNode, Is.SameAs(table));
            Assert.That(bm2.BookmarkEnd.ParentNode, Is.SameAs(table));
            Assert.That(bm1.BookmarkEnd.NextNonAnnotationSibling, Is.Null);
            Assert.That(bm2.BookmarkEnd.NextNonAnnotationSibling, Is.Null);
        }

        /// <summary>
        /// WORDSNET-18882 Tests that an exception is generated when a column bookmark is being started outside
        /// a table.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestColumnBookmarkStartOutsideTable()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.StartColumnBookmark("Bookmark1");
        }

        /// <summary>
        /// WORDSNET-18882 Tests that an exception is generated when a column bookmark is being ended outside
        /// a table.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestColumnBookmarkEndOutsideTable()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();
            builder.InsertCell();
            builder.StartColumnBookmark("Bookmark1");
            builder.EndTable();

            builder.EndColumnBookmark("Bookmark1");
        }

        /// <summary>
        /// WORDSNET-18882 Tests that an exception is generated when a column bookmark is being ended in
        /// a different table than the bookmark start is.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestColumnBookmarkEndInDifferentTable()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();
            builder.InsertCell();
            builder.StartColumnBookmark("Bookmark1");
            builder.EndTable();

            builder.StartTable();
            builder.InsertCell();
            builder.EndColumnBookmark("Bookmark1");
            builder.EndTable();
        }

        /// <summary>
        /// WORDSNET-18882 Tests that an exception is generated when a column bookmark is being ended at
        /// a column with an index less than a column index of the bookmark start.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestWrongBookmarkEndColumnIndex()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartTable();

            builder.InsertCell();
            builder.InsertCell();
            builder.StartColumnBookmark("Bookmark1");
            builder.EndRow();

            builder.InsertCell();
            builder.EndColumnBookmark("Bookmark1");
            builder.InsertCell();
            builder.EndRow();

            builder.EndTable();
        }

        /// <summary>
        /// WORDSNET-23756 InvalidCastException is thrown when set text of bookmark, which is inside OfficeMath.
        /// Improved Bookmark.Text handling.
        /// </summary>
        [Test]
        public void Test23756()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test23756.docx");

            Assert.That(doc.GetText(), Is.EqualTo("A=πr2\f"));
            doc.Range.Bookmarks["test"].Text = "test";
            Assert.That(doc.GetText(), Is.EqualTo("A=testπr2\f"));
        }


        /// <summary>
        /// WORDSNET-24087 Bookmark itself is removed after setting it's text to empty string.
        /// The row-level bookmark parent row should be taken as next sibling row in case when the bookmark
        /// is placed between rows, just inside a table node. This makes a sense in the case of nested tables.
        /// </summary>
        [Test]
        public void Test24087()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test24087.docx");

            // The problematic bookmark.
            doc.Range.Bookmarks["Cooling_Capacity_ck"].Text = "";

            // Check bookmark itself is not removed.
            Assert.That(doc.Range.Bookmarks["Cooling_Capacity_ck"], IsNot.Null());

            // These bookmarks intersect with problematic bookmark but should not be removed as well.
            Assert.That(doc.Range.Bookmarks["Cooling_DehumCapacity_ck"], IsNot.Null());
            Assert.That(doc.Range.Bookmarks["Cooling_CapacitySens_ck"], IsNot.Null());

            // Check also all bookmarks are in place after round-trip.
            doc = TestUtil.SaveOpen(doc, @"Model\Bookmark\Test24087", UnifiedScenario.Docx2DocxNoGold);

            Assert.That(doc.Range.Bookmarks["Cooling_Capacity_ck"], IsNot.Null());
            Assert.That(doc.Range.Bookmarks["Cooling_DehumCapacity_ck"], IsNot.Null());
            Assert.That(doc.Range.Bookmarks["Cooling_CapacitySens_ck"], IsNot.Null());
        }



        /// <summary>
        /// WORDSNET-24897 Do not rename bookmarks when import building blocks.
        /// MS Word allows duplicate bookmark names in different building blocks. The same has been implemented in
        /// Aspose.Words when importing nodes.
        /// </summary>
        [Test]
        public void Test24897()
        {
            const string fileName = @"Model\Bookmark\Test24897.dotx";
            Document doc = TestUtil.Open(fileName);
            Document sourceDocument = doc.Clone();

            CompositeNode sourceBuildingBlock = sourceDocument.GlossaryDocument.BuildingBlocks[0];
            GlossaryDocument destinationGlossary = doc.GlossaryDocument;

            BookmarkCollection bookmarks = destinationGlossary.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(1));
            string originalBookmarkName = bookmarks[0].Name;

            // Import sourceBuildingBlock several times.
            const int importCount = 5;
            for (int i = 0; i < importCount; i++)
            {
                Node importedNode = destinationGlossary.ImportNode(sourceBuildingBlock, true);
                destinationGlossary.BuildingBlocks.Add(importedNode);
            }

            // There are the original bookmark and the imported ones. The all bookmark must have the same name because
            // they are in different building blocks.
            CheckBookmarkCountAndNames(bookmarks, importCount + 1, 0, -1, originalBookmarkName);

            // Import building block contents without the building block itself. The bookmark is renamed at this case.
            CompositeNode destinationBuildingBlock = (CompositeNode)destinationGlossary.BuildingBlocks[0].Clone(false);
            destinationGlossary.BuildingBlocks.Add(destinationBuildingBlock);
            for (Node node = sourceBuildingBlock.FirstChild; node != null; node = node.NextSibling)
            {
                Node importedNode = destinationGlossary.ImportNode(node, true);
                destinationBuildingBlock.AppendChild(importedNode);
            }

            int lastBookmarkIndex = bookmarks.Count - 1;
            CheckBookmarkCountAndNames(bookmarks, importCount + 2, 0, lastBookmarkIndex - 1, originalBookmarkName);
            Assert.That(bookmarks[lastBookmarkIndex].Name, IsNot.EqualTo(originalBookmarkName));
            Assert.That(bookmarks[lastBookmarkIndex].Name.StartsWith(originalBookmarkName), Is.True);

            // Test saving.
            doc = TestUtil.SaveOpen(doc, fileName, null, false);
            bookmarks = doc.GlossaryDocument.Range.Bookmarks;

            CheckBookmarkCountAndNames(bookmarks, importCount + 2, 0, lastBookmarkIndex - 1, originalBookmarkName);
            Assert.That(bookmarks[lastBookmarkIndex].Name, IsNot.EqualTo(originalBookmarkName));
            Assert.That(bookmarks[lastBookmarkIndex].Name.StartsWith(originalBookmarkName), Is.True);
        }

        /// <summary>
        /// WORDSNET-25490 MS Word keeps the very last bookmark with a non-unique name.
        /// </summary>
        [Test]
        public void Test25490()
        {
            Document document = TestUtil.Open(@"Model\Bookmark\Test25490.docx");

            TestUtil.ExecuteValidator(document, SaveFormat.Docx);

            BookmarkCollection bookmarks = document.Range.Bookmarks;

            Assert.That(bookmarks.Count, Is.EqualTo(5));
            AssertBookmark(bookmarks[0], "Book0", "Item 3");
            AssertBookmark(bookmarks[1], "Book1A", "Item 4");
            AssertBookmark(bookmarks[2], "Book1B", "Item 5");
            AssertBookmark(bookmarks[3], "Book1C", "Item 6");
            AssertBookmark(bookmarks[4], "BookD", "Item 9");
        }

        /// <summary>
        /// Additional case for WORDSNET-25491
        /// More detailed case.
        /// </summary>
        [Test]
        public void Test25491A()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.StartBookmark("testbookmark");
            FormField formField = builder.InsertCheckBox(string.Empty, true, 12);

            Assert.That(formField, IsNot.Null());
        }

        /// <summary>
        /// Tests the <see cref="BookmarkFinder.FindBookmarkStart(Node,string,BookmarkEnd)"/> method.
        /// </summary>
        [Test]
        public void TestFindBookmarkStart()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            const string name = "test1";
            BookmarkStart start1 = builder.StartBookmark(name);
            BookmarkStart start2 = builder.StartBookmark(name);
            BookmarkEnd end2 = builder.EndBookmark(name);
            BookmarkStart start3 = builder.StartBookmark(name);
            BookmarkEnd end3 = builder.EndBookmark(name);
            BookmarkEnd end1 = builder.EndBookmark(name);

            Assert.That(ReferenceEquals(start1, BookmarkFinder.FindBookmarkStart(doc, name, end1)), Is.True);
            Assert.That(ReferenceEquals(start2, BookmarkFinder.FindBookmarkStart(doc, name, end1)), Is.False);
            Assert.That(ReferenceEquals(start3, BookmarkFinder.FindBookmarkStart(doc, name, end1)), Is.False);
            Assert.That(ReferenceEquals(start2, BookmarkFinder.FindBookmarkStart(doc, name, end2)), Is.True);
            Assert.That(ReferenceEquals(start3, BookmarkFinder.FindBookmarkStart(doc, name, end3)), Is.True);
        }

        /// <summary>
        /// WORDSNET-19052 When bookmarks are in nearby table cells, bookmark contents are not inserted correctly
        /// Support for column bookmarks has been added to the
        /// <see cref="DocumentBuilder.MoveToBookmark(string,bool,bool)"/> method.
        /// </summary>
        [Test]
        public void TestMoveToColumnBookmark()
        {
            const string fileName = @"Model\Bookmark\TestMoveToColumnBookmark.docx";
            Document doc = TestUtil.Open(fileName);
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Emulate cases where a bookmark end is moved to the next paragraph after its original position, as the old
            // versions of AW did.
            BookmarkEnd bookmarkEnd = doc.Range.Bookmarks["BM12-1-2-1row"].BookmarkEnd;
            Node nextNode = bookmarkEnd.NextNonAnnotationSibling;
            // Ensure that the bookmark end is at the row level, and a row exists after it.
            Debug.Assert(nextNode is Row);
            ((CompositeNode)((CompositeNode)nextNode).GetChild(NodeType.Paragraph, 0, true)).PrependChild(bookmarkEnd);
            bookmarkEnd = doc.Range.Bookmarks["BM11-0-0-2rows"].BookmarkEnd;
            CompositeNode parent = bookmarkEnd.ParentNode;
            // Ensure that the bookmark end is after the last row of the table.
            Debug.Assert((parent.NodeType == NodeType.Table) && (bookmarkEnd.NextNonAnnotationSibling == null));
            ((CompositeNode)parent.NextNonAnnotationSibling).PrependChild(bookmarkEnd);

            foreach (Bookmark bookmark in doc.Range.Bookmarks)
            {
                string name = bookmark.Name;
                const string textTemplate = "<{0} {1}>";

                builder.MoveToBookmark(name, true, true);
                builder.Write(string.Format(textTemplate, name, "AfterStart"));

                builder.MoveToBookmark(name, false, false);
                builder.Write(string.Format(textTemplate, name, "BeforeEnd"));

                builder.MoveToBookmark(name, false, true);
                builder.Write(string.Format(textTemplate, name, "AfterEnd"));

                builder.MoveToBookmark(name, true, false);
                builder.Write(string.Format(textTemplate, name, "BeforeStart"));
            }

            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        private static void AssertBookmark(Bookmark bookmark, string name, string text)
        {
            Assert.That(bookmark.Name, Is.EqualTo(name));
            Assert.That(bookmark.Text, Is.EqualTo(text));
        }

        /// <summary>
        /// Checks the number of bookmarks in the specified collection. And checks that all the bookmarks in the index
        /// range have the specified name.
        /// </summary>
        private static void CheckBookmarkCountAndNames(BookmarkCollection bookmarks, int expectedBookmarkCount,
            int startBookmarkIndex, int endBookmarkIndex, string expectedBookmarkName)
        {
            Assert.That(bookmarks.Count, Is.EqualTo(expectedBookmarkCount));

            if (endBookmarkIndex < 0)
                endBookmarkIndex = expectedBookmarkCount - 1;

            for (int i = startBookmarkIndex; i <= endBookmarkIndex; i++)
                Assert.That(bookmarks[i].Name, Is.EqualTo(expectedBookmarkName));
        }

        /// <summary>
        /// Get run containing field result.
        /// </summary>
        /// <param name="p">Paragraph containing field data.</param>
        /// <returns>Run with field result.</returns>
        private static Run GetFieldResultRun(Paragraph p)
        {
            // Get first run following separator.
            Node separator = p.GetChild(NodeType.FieldSeparator, 0, false);
            return (Run)p.GetChildNodes(NodeType.Any, false)[(p.IndexOf(separator) + 1)];
        }
    }
}
