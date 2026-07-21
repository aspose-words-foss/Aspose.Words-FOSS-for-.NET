// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2018 by Alexander Zhiltsov

using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests the <see cref="Bookmark.Text"/> property.
    /// </summary>
    [TestFixture]
    public class TestBookmarkText
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests setting text. Contents of a paragraph is removed and the paragraph is merged with the next one.
        /// </summary>
        [TestCase(@"Model\Bookmark\TestSettingTextWithParagraphMerge1.docx")]
        [TestCase(@"Model\Bookmark\TestSettingTextWithParagraphMerge2.docx")]
        [TestCase(@"Model\Bookmark\TestSettingTextWithParagraphMerge3.docx")]
        [TestCase(@"Model\Bookmark\TestSettingTextWithParagraphMerge4.docx")]
        [TestCase(@"Model\Bookmark\TestSettingTextWithParagraphMerge5.docx")]
        public void TestSettingTextWithParagraphMerge(string fileName)
        {
            Document doc = TestUtil.Open(fileName);
            CheckSettingTextWithParagraphMerge(doc);
        }

        /// <summary>
        /// Tests that returned text is the same as set.
        /// </summary>
        [TestCase(@"Fields\EquationsAndFormulas\TestJira5671.docx", "a")]
        [TestCase(@"ImportDocx\TestJira5758.docx", "_last_block")]
        public void TestSettingText(string fileName, string bookmarkName)
        {
            Document doc = TestUtil.Open(fileName);
            Bookmark bookmark = doc.Range.Bookmarks[bookmarkName];

            const string newText = "New";
            bookmark.Text = newText;
            Assert.That(bookmark.Text, Is.EqualTo(newText));
        }


        /// <summary>
        /// Tests getting/setting text on bookmarks displaced by SDT.
        /// </summary>
        public void TestDisplacedBySdt(string fileName)
        {
            Document doc = TestUtil.Open(fileName);

            Bookmark bookmark = doc.Range.Bookmarks["PubDate"];
            // Start node is either before the SDT or the first child of it.
            NodeType parentNodeType = bookmark.BookmarkStart.ParentNode.NodeType;
            NodeType nextNodeType = bookmark.BookmarkStart.NextSibling.NodeType;
            Assert.That(bookmark.Text, Is.EqualTo("14 February 2018"));

            const string newText = "New";
            bookmark.Text = newText;
            Assert.That(bookmark.Text, Is.EqualTo(newText));
            Assert.That(bookmark.BookmarkStart.ParentNode.NodeType, Is.EqualTo(parentNodeType));
            Assert.That(bookmark.BookmarkStart.NextSibling.NodeType, Is.EqualTo(nextNodeType));

            CompositeNode sdt = (CompositeNode)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.FirstNonAnnotationChild.NodeType, Is.EqualTo(NodeType.Cell));
            CompositeNode cell = (CompositeNode)sdt.FirstNonAnnotationChild;
            Assert.That(cell.FirstChild.NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(bookmark.BookmarkEnd.ParentNode, Is.SameAs(sdt));
        }






        /// <summary>
        /// Tests value of the <see cref="Bookmark.Text"/> property if bookmark end is at the end of story.
        /// </summary>
        [TestCase(@"Model\Revision\RevisionGroups\TestJira15785_DeletedTextBox.docx", "_GoBack",
            170, "[Grab your reader’s", "just drag it.]")]
        [TestCase(@"Model\Bookmark\TestBookmarkTextAtEndOfStory.docx", "header",
            23, "", "Header line\rHeader line")]
        [TestCase(@"Model\Bookmark\TestBookmarkTextAtEndOfStory.docx", "footnote",
            32, "", " Bookmark in footnote\rNext line")]
        public void TestBookmarkTextAtEndOfStory(string fileName, string bookmarkName,
            int expectedTextLength, string startsWith, string endsWith)
        {
            Document doc = TestUtil.Open(fileName);

            Bookmark bm = doc.Range.Bookmarks[bookmarkName];
            string text = bm.Text;

            // Test Explorer does not show test cases if string parameter contains non-readable chars e.g. '\u0005', so
            // it may be not possible to specify entire bookmark text and it may be too long: several parameters are used.
            Assert.That(text.Length, Is.EqualTo(expectedTextLength));
            Assert.That(text.StartsWith(startsWith), Is.True);
            Assert.That(text.EndsWith(endsWith), Is.True);
        }








        /// <summary>
        /// Relates to WORDSNET-19518.
        /// Checks the insertion text and the retrieving text for the bookmark, which is in the second row.
        /// </summary>
        [Test]
        public void Test19518StartWithSecondRow()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test19518StartWithSecondRow.docx");
            Bookmark bookmark = doc.Range.Bookmarks["SIGNERSTAMP1"];

            Assert.That(bookmark.Text, Is.EqualTo("Row1_Cell0_Text0\aRow1_Cell1_Text0\a"));

            Cell cell = doc.FirstSection.Body.Tables[0].LastRow.FirstCell;
            CheckSetTextInColumnBookmark(bookmark, cell);
        }

        /// <summary>
        /// Relates to WORDSNET-19518.
        /// Checks that the range text is empty if the first column and last column are greater
        /// than the count of row cells.
        /// </summary>
        [Test]
        public void Test19518FirstColumnAndLastColumnMoreThanColumnsCount()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test19518ColumnsOutOfRange.docx");
            Bookmark bookmark = doc.Range.Bookmarks["SIGNERSTAMP1"];

            Assert.That(bookmark.Text, Is.EqualTo(string.Empty));
            bookmark.Text = "test";
            Assert.That(bookmark.Text, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// Relates to WORDSNET-19518.
        /// Checks that the last column equals row last column if last column greater than the count of row cells.
        /// </summary>
        [Test]
        public void Test19518LastColumnMoreThanColumnsCount()
        {
            const string bookmarkText =
                "Row0_Cell1_Text0Row0_Cell1_Text1\aRow0_Cell2_Text0Row0_Cell2_Text1Row0_Cell2_Text2\a\a" +
                "Row1_Cell0_Text0\aRow1_Cell1_Text0\aRow1_Cell2_Text0\a";

            Document doc = TestUtil.Open(@"Model\Bookmark\Test19518LastColumnOutOfRange.docx");
            Bookmark bookmark = doc.Range.Bookmarks["SIGNERSTAMP1"];

            Assert.That(bookmark.Text, Is.EqualTo(bookmarkText));

            Cell cell = doc.FirstSection.Body.Tables[0].FirstRow.Cells[1];
            CheckSetTextInColumnBookmark(bookmark, cell);
        }

        /// <summary>
        /// Relates to WORDSNET-19518.
        /// Checks that the first column equals the last column if the last column is less than the first column.
        /// </summary>
        [TestCase(@"Model\Bookmark\Test19518LastColumnLessThanFirstColumnA.docx", "Row0_Cell1_Text0Row0_Cell1_Text1\a")]
        [TestCase(@"Model\Bookmark\Test19518LastColumnLessThanFirstColumnB.docx",
            "Row0_Cell0\aRow0_Cell1\aRow0_Cell2\a\aRow1_Cell0\a")]
        public void Test19518LastColumnLessThanFirstColumn(string path, string bookmarkText)
        {
            Document doc = TestUtil.Open(path);
            Bookmark bookmark = doc.Range.Bookmarks["SIGNERSTAMP1"];

            Assert.That(bookmark.Text, Is.EqualTo(bookmarkText));

            Cell cell = doc.FirstSection.Body.Tables[0].FirstRow.Cells[bookmark.LastColumn];
            CheckSetTextInColumnBookmark(bookmark, cell);
        }

        /// <summary>
        /// Relates to WORDSNET-19518. Checks that BookmarkEnd may be not in a row.
        /// </summary>
        [TestCase(@"Model\Bookmark\Test19518BookmarkEndNotInRowA.docx",
            "Row1_Cell0\aRow1_Cell1\aRow1_Cell2\a\aRow2_Cell0\aRow2_Cell1\a")]
        [TestCase(@"Model\Bookmark\Test19518BookmarkEndNotInRowB.docx",
            "Row1_Cell0\aRow1_Cell1\aRow1_Cell2\a\aRow2_Cell0\aRow2_Cell1\a")]
        [TestCase(@"Model\Bookmark\Test19518BookmarkEndNotInRowC.docx", "Row1_Cell0\aRow1_Cell1\a")]
        public void Test19518BookmarkNotInRow(string path, string bookmarkText)
        {
            Document doc = TestUtil.Open(path);
            Bookmark bookmark = doc.Range.Bookmarks["SIGNERSTAMP1"];

            Assert.That(bookmark.Text, Is.EqualTo(bookmarkText));

            Cell cell = doc.FirstSection.Body.Tables[0].Rows[1].FirstCell;
            CheckSetTextInColumnBookmark(bookmark, cell);
        }

        /// <summary>
        /// Relates to WORDSNET-19518. Checks that BookmarkEnd may be not in next row.
        /// </summary>
        public void Test19518BookmarkEndInNextRow(string path, string bookmarkText)
        {
            Document doc = TestUtil.Open(path);

            Bookmark bm1 = doc.Range.Bookmarks["BM1"];
            Assert.That(bm1.Text, Is.EqualTo(bookmarkText));

            Bookmark bm2 = doc.Range.Bookmarks["BM2"];
            Assert.That(bm2.Text, Is.EqualTo(bookmarkText));

            CheckSetTextInColumnBookmark(bm1, doc.FirstSection.Body.Tables[0].Rows[0].Cells[1]);
            CheckSetTextInColumnBookmark(bm2, doc.FirstSection.Body.Tables[0].Rows[1].Cells[1]);
        }

        /// <summary>
        /// Relates to WORDSNET-19518. Checks that BookmarkStarts move to
        /// the first paragraph from other paragraphs within the cell when inserting text.
        /// </summary>
        [Test]
        public void Test19518BookmarkStartsMovingInFirstPara()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test19518BookmarkStartsMovingInFirstPara.docx");

            Bookmark bm1 = doc.Range.Bookmarks["BM1"];
            CheckSetTextInColumnBookmark(bm1, doc.FirstSection.Body.Tables[0].Rows[0].Cells[0]);

            Paragraph para = doc.FirstSection.Body.Tables[0].Rows[0].Cells[0].FirstParagraph;
            Assert.That(doc.Range.Bookmarks["BM1"].BookmarkStart.ParentNode, Is.EqualTo(para));
            Assert.That(doc.Range.Bookmarks["BM2"].BookmarkStart.ParentNode, Is.EqualTo(para));
        }





        /// <summary>
        /// WORDSNET-21830 Replacing overlapping Bookmarks behaves unexpected or causes an error with Java.
        /// Fixed by restoring deleted overlapping bookmarks if they were deleted.
        /// </summary>
        [Test]
        public void Test21830()
        {
            const string testFileNameWithoutExtension = @"Model\Bookmark\Test21830";

            Document doc = TestUtil.Open(testFileNameWithoutExtension, LoadFormat.Docm);
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.Range.Bookmarks["mark1"].Text, Is.EqualTo("Aaaaaaa\rXxx"));
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("Xxx\rbbbbbbb"));

            // The first customer's experiment.
            doc.Range.Bookmarks["mark1"].Text = "1111111\r";
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.Range.Bookmarks["mark1"].Text, Is.EqualTo("1111111\r"));
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("1111111\r\rbbbbbbb"));

            // WORDSNET-22642 Word removes `mark1` as its both End and Start are located inside 'mark2'.
            doc.Range.Bookmarks["mark2"].Text = "2222222\r";
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(1));
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("2222222\r"));

            // Re-saving confirms the result.
            doc = TestUtil.SaveOpen(doc, testFileNameWithoutExtension, UnifiedScenario.Docm2DocmNoGold);
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(1));
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("2222222\r"));

            // The second customer's experiment (different order of replacements).
            doc = TestUtil.Open(testFileNameWithoutExtension, LoadFormat.Docm);
            doc.Range.Bookmarks["mark2"].Text = "2222222\r";
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.Range.Bookmarks["mark1"].Text, Is.EqualTo("Aaaaaaa\r"));
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("2222222\r"));

            doc.Range.Bookmarks["mark1"].Text = "1111111\r";
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.Range.Bookmarks["mark1"].Text, Is.EqualTo("1111111\r"));
            // This is wrong. Actually Word in this scenario displays "1111111\r2222222\r" for this bookmark.
            // The issue seems in UntagleStart/End methods.
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("2222222\r"));

            // Re-saving confirms the result.
            doc = TestUtil.SaveOpen(doc, testFileNameWithoutExtension, UnifiedScenario.Docm2DocmNoGold);
            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(2));
            Assert.That(doc.Range.Bookmarks["mark1"].Text, Is.EqualTo("1111111\r"));
            Assert.That(doc.Range.Bookmarks["mark2"].Text, Is.EqualTo("2222222\r"));
        }

        /// <summary>
        /// WORDSNET-22642 Wrong Replacement of Text Content if Bookmarks in Word DOCM Document are enclosing each other.
        /// The enclosed bookmarks, that have its both Start and End inside the range being removed, should be removed.
        /// This is follow up of incorrect WORDSNET-21830, where ALL removed bookmarks were restored.
        /// </summary>
        [Test]
        public void Test22642()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test22642.docm");

            doc.Range.Bookmarks["mark2"].Text = "22222";

            // Check document structure: the first node is BookmarkStart, then Run with specified text "22222"
            // and at the last BookmarkEnd. There should not be any another nodes (i.e. bookmarks).
            BookmarkCollection bookmarks = doc.Range.Bookmarks;
            Assert.That(bookmarks.Count, Is.EqualTo(1));
            Assert.That(bookmarks[0].Name, Is.EqualTo("mark2"));

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.FirstChild, Is.EqualTo(bookmarks[0].BookmarkStart));

            Run run = (Run)bookmarks[0].BookmarkStart.NextSibling;
            Assert.That(run.Text, Is.EqualTo("22222"));

            Assert.That(run.NextSibling, Is.EqualTo(bookmarks[0].BookmarkEnd));

            // It will be also convenient to check gold here.
            TestUtil.SaveCheckGold(doc, @"Model\Bookmark\Test22642.docm");
        }






        /// <summary>
        /// WORDSNET-24140 Nested bookmark is removed after setting text of outer bookmark.
        /// Checks whether nested bookmarks are not deleted when a text is inserted.
        /// </summary>
        [Test]
        public void Test24140()
        {
            Document doc = TestUtil.Open(@"Model\Bookmark\Test24140.dotx");
            Assert.That(doc.Range.Bookmarks["True"], IsNot.Null());
            Bookmark bookmark = doc.Range.Bookmarks["False2"];
            bookmark.Text = "";
            doc = TestUtil.SaveOpen(doc, "Test24140", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Range.Bookmarks["True"], IsNot.Null());
        }

        private static void CheckBookmarkOfTest19767(Bookmark bookmark, string bookmarkNewText)
        {
            BookmarkStart bookmarkStart = bookmark.BookmarkStart;
            BookmarkEnd bookmarkEnd = bookmark.BookmarkEnd;
            Paragraph paragraph1 = (Paragraph)bookmarkStart.ParentNode;
            Paragraph paragraph2 = (Paragraph)paragraph1.NextNonAnnotationSibling;

            int beforeParagraph1 = paragraph1.ParaPr.SpaceBefore;
            int afterParagraph1 = paragraph1.ParaPr.SpaceAfter;
            int beforeParagraph2 = paragraph2.ParaPr.SpaceBefore;
            int afterParagraph2 = paragraph2.ParaPr.SpaceAfter;
            int paragraph1FontSize = paragraph1.ParagraphBreakRunPr.Size;
            int paragraph2FontSize = paragraph2.ParagraphBreakRunPr.Size;

            bool isEndedInFirstParagraph = (bookmarkEnd.ParentNode == paragraph1);

            bookmark.Text = bookmarkNewText;

            bool isParagraphInserted = bookmarkNewText.Contains("\r");
            Paragraph bookmarkStartParagraph = (Paragraph)bookmarkStart.ParentNode;
            Paragraph bookmarkEndParagraph = (Paragraph)bookmarkEnd.ParentNode;
            if (!isParagraphInserted)
                Assert.That(bookmarkEndParagraph, Is.SameAs(bookmarkStartParagraph));

            ParaPr resultParaPr = bookmarkStartParagraph.ParaPr;
            Run nextRun = (Run)bookmarkEnd.NextNonAnnotationSibling;

            if (bookmarkNewText != "")
            {
                Run bookmarkRun = (Run)bookmarkStart.NextNonAnnotationSibling;
                Assert.That(bookmarkRun.RunPr.Size, Is.EqualTo(paragraph1FontSize));
            }

            if (isEndedInFirstParagraph)
            {
                Assert.That(bookmarkEndParagraph, Is.SameAs(paragraph1));
                Assert.That(resultParaPr.SpaceBefore, Is.EqualTo(beforeParagraph1));
                Assert.That(resultParaPr.SpaceAfter, Is.EqualTo(afterParagraph1));
                Assert.That(nextRun.RunPr.Size, Is.EqualTo(paragraph1FontSize));
            }
            else
            {
                Assert.That(bookmarkEndParagraph, Is.SameAs(paragraph2));
                Assert.That(resultParaPr.SpaceBefore, Is.EqualTo(beforeParagraph2));
                Assert.That(resultParaPr.SpaceAfter, Is.EqualTo(afterParagraph2));
                Assert.That(nextRun.RunPr.Size, Is.EqualTo(paragraph2FontSize));
            }

            Assert.That(bookmarkStartParagraph.ParagraphBreakRunPr.Size, Is.EqualTo((isEndedInFirstParagraph || isParagraphInserted) ? paragraph1FontSize : paragraph2FontSize));
            Assert.That(bookmarkEndParagraph.ParagraphBreakRunPr.Size, Is.EqualTo(isEndedInFirstParagraph ? paragraph1FontSize : paragraph2FontSize));
        }

        private static void CheckTableStructure(Table table)
        {
            BookmarkCollection bookmarks = table.Range.Bookmarks;
            RowCollection rows = table.Rows;

            Assert.That(rows.Count, Is.EqualTo(3));
            Assert.That(rows[0].Cells.Count, Is.EqualTo(3));
            Assert.That(rows[1].Cells.Count, Is.EqualTo(4));
            Assert.That(rows[2].Cells.Count, Is.EqualTo(5));

            Bookmark bmTitel = bookmarks["titel"];
            Assert.That(bmTitel, IsNot.Null());
            Assert.That(bookmarks["dok"], IsNot.Null());
            Assert.That(bookmarks["dat"], IsNot.Null());
            Assert.That(bookmarks["rev"], IsNot.Null());
            Assert.That(bookmarks["utf"], IsNot.Null());
            Assert.That(bookmarks["app1"], IsNot.Null());

            Assert.That(bmTitel.BookmarkStart.ParentNode, Is.EqualTo(table.FirstRow.FirstCell.FirstParagraph));
            for (int i = 1; i < bookmarks.Count; i++)
                Assert.That(bookmarks[i].BookmarkStart.ParentNode, Is.EqualTo(table.LastRow.FirstCell.FirstParagraph));
        }

        private static void CheckSetTextInColumnBookmark(Bookmark bookmark, Cell cell)
        {
            bookmark.Text = "test";
            Assert.That(cell.Range.Text, Is.EqualTo("test\a"));
        }

        /// <summary>
        /// Checks result of setting text for simple case with merging paragraphs.
        /// </summary>
        private static void CheckSettingTextWithParagraphMerge(Document doc)
        {
            Bookmark bookmark = doc.Range.Bookmarks["Test"];
            Assert.That(bookmark.Text, Is.EqualTo("Paragraph 2\r"));
            NodeLevel startLevel = bookmark.BookmarkStart.NodeLevel;

            Node childOf3Para = doc.FirstSection.Body.Paragraphs[2].FirstChild;
            NodeType nodeTypeAfterBookmark = childOf3Para != null ? childOf3Para.NodeType : NodeType.Null;
            string textAfterBookmark = childOf3Para != null ? childOf3Para.GetText() : null;

            const string newText = "New";
            bookmark.Text = newText;
            bookmark = doc.Range.Bookmarks["Test"];
            Assert.That(bookmark.Text, Is.EqualTo(newText));

            BookmarkEnd end = bookmark.BookmarkEnd;
            if (nodeTypeAfterBookmark != NodeType.Null)
            {
                Assert.That(end.NextSibling.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(((Run)end.NextSibling).Text, Is.EqualTo("Paragraph 3"));
            }
            else
            {
                Assert.That(end.NextSibling, Is.Null);
            }
            Assert.That(((Paragraph)end.ParentNode).ParaPr.LeftIndent, Is.EqualTo(1701));

            BookmarkStart start = bookmark.BookmarkStart;
            Assert.That(start.NodeLevel, Is.EqualTo(startLevel));
            if (startLevel == NodeLevel.Block)
            {
                Assert.That(start.NextSibling, Is.SameAs(end.ParentNode));
            }
            else
            {
                Assert.That(start.NextSibling.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(((Run)start.NextSibling).Text, Is.EqualTo(newText));
            }
        }
    }
}
