// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    using Aspose.Words.Fields;

    [TestFixture]
    public class TestNodeRange
    {
        [Test]
        public void TestEmptyRange()
        {
            Run testRun = GetTestRun(0);
            string testText = testRun.Text;

            TestEmptyRangeBody(1, 0);
            TestEmptyRangeBody(1, 1);
            TestEmptyRangeBody(0, 0);
            TestEmptyRangeBody(testText.Length, testText.Length);
            TestEmptyRangeBody(testText.Length - 1, testText.Length - 1);
            TestEmptyRangeBody(testText.Length, testText.Length + 1);
        }

        private static void TestEmptyRangeBody(int startOffset, int endOffset)
        {
            Run testRun = GetTestRun(0);

            NodeRange range = MakeRangeFromARun(testRun, startOffset, endOffset);
            string context = MakeSingleRunContext(testRun.Text, startOffset, endOffset);

            Assert.That(range.IsEmpty, Is.True, context);
        }

        private static NodeRange MakeRangeFromARun(Run run, int startOffset, int endOffset)
        {
            return MakeRangeFromRuns(run, startOffset, run, endOffset);
        }

        internal static NodeRange MakeRangeFromRuns(Run run0, int startOffset, Run run1, int endOffset)
        {
            DocumentPosition start = new DocumentPosition(run0, startOffset);
            DocumentPosition end = new DocumentPosition(run1, endOffset);
            return new NodeRange(start, end);
        }

        /// <summary>
        /// Tests splitting of a single run.
        /// </summary>
        [Test]
        public void TestSingleRunSplit()
        {
            Run testRun = GetTestRun(0);
            string testText = testRun.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");

            TestSingleRunSplitBody(0, 1, "T");
            TestSingleRunSplitBody(0, testText.Length - 1, "Tex");
            TestSingleRunSplitBody(0, testText.Length, "Text");
            TestSingleRunSplitBody(1, testText.Length - 1, "ex");
            TestSingleRunSplitBody(1, testText.Length, "ext");

            // Test out of range condition.
            TestSingleRunSplitBody(1, testText.Length + 1, "ext");

            // From the middle
            TestSingleRunSplitBody(1, 3, "ex");

            // TODO: ? empty run
        }

        private static void TestSingleRunSplitBody(int startOffset, int endOffset, string expectedText)
        {
            Document doc = gTestDocument.Clone();
            Run testRun = GetTestRun(doc, 0);

            NodeRange range = MakeRangeFromARun(testRun, startOffset, endOffset);

            string context = MakeSingleRunContext(expectedText, startOffset, endOffset);

            // This line will initialize the cached range.End.Length property.
            // Do not remove it.
            Debug.WriteLine("Source range end is end of run: " + range.End.IsEnd);

            range.Isolate();
            Run extractedRun = (Run)range.Start.Node;
            Assert.That(extractedRun.Text, Is.EqualTo(expectedText), context);
            Assert.That(range.Start.IsStart, Is.True, "Start is not zero: " + context);
            Assert.That(range.End.IsEnd, Is.True, "End is not end: " + context);

            Assert.That(range.End.Offset, Is.EqualTo(expectedText.Length), "End offset is not correct: " + context);
        }

        public static string MakeSingleRunContext(string expectedText, int startOffset, int endOffset)
        {
            return string.Format("Expected: '{0}', start: {1}, end: {2}", expectedText, startOffset, endOffset);
        }

        [Test]
        public void Test2RunsSplit()
        {
            Run run0 = GetTestRun(0);
            Run run1 = GetTestRun(1);
            string testText = run0.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");
            Assert.That("Text", Is.EqualTo(run1.Text), "Unexpected test run text");

            Test2RunsSplitBody(0, 1, "Text", "T");
            Test2RunsSplitBody(0, testText.Length - 1, "Text", "Tex");
            Test2RunsSplitBody(0, testText.Length, "Text", "Text");
            Test2RunsSplitBody(1, testText.Length - 1, "ext", "Tex");
            Test2RunsSplitBody(1, testText.Length, "ext", "Text");
            Test2RunsSplitBody(1, 1, "ext", "T");
            Test2RunsSplitBody(testText.Length - 1, 1, "t", "T");

            // From the middle:
            Test2RunsSplitBody(2, 2, "xt", "Te");

            // Nothing from the first:
            Test2RunsSplitBody(testText.Length, 1, "", "T");
        }

        [Test]
        public void Test2RunsSplitTo1Run()
        {
            Run run0 = GetTestRun(0);
            Run run1 = GetTestRun(1);
            string testText = run0.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");
            Assert.That("Text", Is.EqualTo(run1.Text), "Unexpected test run text");

            Test2RunsSplitTo1RunBody(0, 0, "Text", "");
            Test2RunsSplitTo1RunBody(testText.Length - 1, 0, "t", "");
            Test2RunsSplitTo1RunBody(testText.Length, 1, "", "T");
            Test2RunsSplitTo1RunBody(testText.Length, testText.Length, "", "Text");
        }

        [Test]
        public void TestButtonsFakeResultCorrection()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Field field = GetTestField(doc);

            NodeRange resultRange = field.GetFakeResult();

            Assert.That(resultRange.Start.Node.NodeType, Is.EqualTo(NodeType.FieldStart));
            Assert.That(resultRange.End.Node.NodeType, Is.EqualTo(NodeType.FieldEnd));
            Assert.That(((FieldEnd)resultRange.End.Node).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
        }

        private static Field GetTestField(Document doc)
        {
            Section sect = doc.AppendChild(new Section(doc));
            Body body = sect.AppendChild(new Body(doc));

            Paragraph para = body.AppendChild(new Paragraph(doc));

            Run run = new Run(doc, RunText);

            FieldStart fielsStart = para.AppendChild(new FieldStart(doc, run.RunPr, FieldType.FieldMacroButton));
            para.AppendChild(new Run(doc, " MACROBUTTON macro "));

            para.AppendChild(new FieldStart(doc, run.RunPr, FieldType.FieldHyperlink));
            para.AppendChild(new Run(doc, " HYPERLINK "));

            para.AppendChild(new FieldStart(doc, run.RunPr, FieldType.FieldGoToButton));
            para.AppendChild(new Run(doc, "GOTOBUTTON"));
            para.AppendChild(new FieldEnd(doc, run.RunPr, FieldType.FieldGoToButton, false));

            para.AppendChild(new FieldStart(doc, run.RunPr, FieldType.FieldFormula));
            para.AppendChild(new Run(doc, "=3"));
            para.AppendChild(new FieldSeparator(doc, run.RunPr, FieldType.FieldFormula));
            para.AppendChild(new FieldEnd(doc, run.RunPr, FieldType.FieldFormula, true));

            para.AppendChild(new FieldSeparator(doc, run.RunPr, FieldType.FieldHyperlink));

            para.AppendChild(new Run(doc, "Some text"));
            para.AppendChild(new BookmarkStart(doc, "_StartBookmark"));
            para.AppendChild(new Run(doc, "Link"));
            para.AppendChild(new BookmarkEnd(doc, "_EndBookmark"));

            para.AppendChild(new FieldEnd(doc, run.RunPr, FieldType.FieldHyperlink, true));
            para.AppendChild(new FieldEnd(doc, run.RunPr, FieldType.FieldMacroButton, false));

            return fielsStart.GetField();
        }

        private static void Test2RunsSplitBody(int startOffset, int endOffset, string expectedText0, string expectedText1)
        {
            Document doc = gTestDocument.Clone();
            Run testRun0 = GetTestRun(doc, 0);
            Run testRun1 = GetTestRun(doc, 1);

            NodeRange range = MakeRangeFromRuns(testRun0, startOffset, testRun1, endOffset);

            string context = MakeDualRunContext(expectedText0, startOffset, expectedText1, endOffset);

            // This line will initialize the cached range.End.Length property.
            // Do not remove it.
            Debug.WriteLine("Source range end is end of run: " + range.End.IsEnd);

            range.Isolate();
            Run extractedStart = (Run)range.Start.Node;
            Assert.That(extractedStart.Text.Substring(range.Start.Offset), Is.EqualTo(expectedText0), context);
            Assert.That(range.Start.IsStart || range.Start.IsEnd, Is.True, "Start is not zero or end: " + context);

            Run extractedEnd = (Run)range.End.Node;
            Assert.That(extractedEnd.Text, Is.EqualTo(expectedText1), context);
            Assert.That(range.End.IsEnd, Is.True, "End is not end: " + context);

            Assert.That(range.End.Offset, Is.EqualTo(expectedText1.Length), "End offset is not correct: " + context);
        }

        private static void Test2RunsSplitTo1RunBody(int startOffset, int endOffset, string expectedText0, string expectedText1)
        {
            // TODO remove duplication
            Document doc = gTestDocument.Clone();
            Run testRun0 = GetTestRun(doc, 0);
            Run testRun1 = GetTestRun(doc, 1);

            NodeRange range = MakeRangeFromRuns(testRun0, startOffset, testRun1, endOffset);

            string context = MakeDualRunContext(expectedText0, startOffset, expectedText1, endOffset);

            range.Isolate();
            Run extractedStart = (Run)range.Start.Node;
            if (!range.Start.IsEnd)
            {
                Assert.That(extractedStart.Text, Is.EqualTo(expectedText0), context);
                Assert.That(range.Start.IsStart, Is.True, "Start is not zero: " + context);
            }

            Run extractedEnd = (Run)range.End.Node;
            if (!range.End.IsStart)
            {
                Assert.That(extractedEnd.Text, Is.EqualTo(expectedText1), context);
                Assert.That(range.End.IsEnd, Is.True, "End is not end: " + context);
            }
        }

        public static string MakeDualRunContext(string expectedText0, int startOffset, string expectedText1, int endOffset)
        {
            return string.Format("Expected: '{0}' and '{3}', start: {1}, end: {2}", expectedText0, startOffset, endOffset, expectedText1);

        }

        /// <summary>
        /// Creates the following test document:
        ///
        /// doc
        ///        section
        ///            body
        ///                para
        ///                    run "Text"
        ///                    run "Text"
        ///                 run "Text"
        /// </summary>
        private static Document CreateTestDoc()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));

            Body body = sect.AppendChild(new Body(doc));
            Paragraph para1 = body.AppendChild(new Paragraph(doc));
            para1.AppendChild(new Run(doc, RunText));
            para1.AppendChild(new Run(doc, RunText));
            para1.AppendChild(new Run(doc, RunText));

            return doc;
        }

        public static Run GetTestRun(int runIdx)
        {
            return GetTestRun(gTestDocument, runIdx);
        }

        public static Run GetTestRun(Document doc, int runIdx)
        {
            return (Run)doc.GetChildNodes(NodeType.Run, true)[runIdx];
        }

        private const string RunText = "Text";

        private static readonly Document gTestDocument;

        static TestNodeRange()
        {
            // RK This must stay in a static ctor for Java autoporting because it can throw and Java porter generates try/catch here.
            gTestDocument = CreateTestDoc();
        }



        /// <summary>
        /// WORDSNET-13363 Document.AcceptAllRevisions throws "The bookmark range is invalid" exception
        ///
        /// If TableColumnBookmark is not included into table, we must ignore FirstColumn and LastColumn.
        /// </summary>
        [Test]
        public void TestJira13363()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create TableColumnBookmark not in table
            BookmarkStart bookmarkStart = builder.StartBookmark("testBookmark");
            bookmarkStart.FirstColumn = 0;
            bookmarkStart.LastColumn = 0;
            BookmarkEnd bookmarkEnd = builder.EndBookmark("testBookmark");

            Assert.That(bookmarkStart.IsColumn, Is.True);
            Assert.That(bookmarkStart.GetAncestor(NodeType.Table), Is.Null);

            NodeRange nodeRange = new NodeRange(bookmarkStart, false, bookmarkEnd, false, bookmarkStart);

            Assert.That(nodeRange.IsTableColumnBookmarkRange, Is.False);
            Assert.That(nodeRange.FirstTableColumnIndex, Is.EqualTo(-1));
            Assert.That(nodeRange.LastTableColumnIndex, Is.EqualTo(-1));
        }

        /// <summary>
        /// Relates to WORDSNET-16667 Checks that Range is updating in real-time.
        /// </summary>
        [Test]
        public void TestJira16667()
        {
            const int bookmarksCount = 5;

            Document doc = BookmarkTestUtil.CreateDocumentWithBookmarks(bookmarksCount);

            Range range = doc.Range;
            Assert.That(range.Bookmarks.Count, Is.EqualTo(bookmarksCount));

            BookmarkTestUtil.AppendBookmark(doc, "anotherOneBookmark");
            Assert.That(range.Bookmarks.Count, Is.EqualTo(bookmarksCount + 1));

            range.Bookmarks.Remove(range.Bookmarks[0].Name);
            Assert.That(range.Bookmarks.Count, Is.EqualTo(bookmarksCount));

            doc.FirstSection.Body.FirstParagraph.FirstRun.Remove();
            Assert.That(range.Bookmarks.Count, Is.EqualTo(bookmarksCount));
        }
    }
}
