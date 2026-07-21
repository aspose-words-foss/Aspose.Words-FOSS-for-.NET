// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Xml;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for how model handles comments (annotations) in a document.
    /// </summary>
    [TestFixture]
    public class TestComments : UnifiedTestsBase
    {

        /// <summary>
        /// Creates valid comments and comment ranges via the public API and saves to various formats.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCreateComment(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("A bit of text");

            // Create a comment that has no range.
            Comment comment = new Comment(doc, "author name 1", "an1", new DateTime(2008, 07, 27, 17, 01, 00));
            comment.SetText("This is my comment 1.");
            Assert.That(comment.Id, Is.EqualTo(0));
            builder.InsertNode(comment);
            builder.Write(" and a bit more text. ");

            // Create a comment that has a range.
            comment = new Comment(doc, "author name 2", "an2", new DateTime(2010, 07, 27, 17, 01, 00));
            comment.SetText("This is my comment 2.");
            Assert.That(comment.Id, Is.EqualTo(1));
            // This is what MS Word usually writes - the comment reference follows the comment range end.
            builder.InsertNode(new CommentRangeStart(doc, comment.Id));
            builder.Write("This is a commented range.");
            builder.InsertNode(new CommentRangeEnd(doc, comment.Id));
            builder.InsertNode(comment);
            builder.Writeln();

            // Create one more comment that has a range. Typically, the comment is just after the
            // comment range end node, but actially it can be anywhere. In this case inside the comment range.
            comment = new Comment(doc);
            comment.SetText("This is my comment 3.");
            builder.InsertNode(new CommentRangeStart(doc, comment.Id));
            builder.Write("Before comment.");
            builder.InsertNode(comment);
            builder.Write(" After comment.");
            builder.InsertNode(new CommentRangeEnd(doc, comment.Id));

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestCreateComment", lf, sf);

            // Check the comment without a range.
            comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
            Assert.That(comment.Id, Is.EqualTo(0));
            Assert.That(comment.Initial, Is.EqualTo("an1"));
            Assert.That(comment.Author, Is.EqualTo("author name 1"));
            Assert.That(comment.GetText(), Is.EqualTo("\x0005This is my comment 1.\r"));

            // Check the comment with a range.
            comment = (Comment)doc.GetChild(NodeType.Comment, 1, true);
            Assert.That(comment.Id, Is.EqualTo(1));
            CommentRangeEnd cre = (CommentRangeEnd)comment.PreviousSibling;
            Assert.That(cre.Id, Is.EqualTo(comment.Id));
            Run run = (Run)cre.PreviousSibling;
            Assert.That(run.GetText(), Is.EqualTo("This is a commented range."));
            CommentRangeStart crs = (CommentRangeStart)run.PreviousSibling;
            Assert.That(crs.Id, Is.EqualTo(comment.Id));
        }

        /// <summary>
        /// WORDSNET-4346 “System.NullReferenceException” exception occurs during saving document after inserting comment.
        /// RK The problem was caused by null author and initials. Changed defaults to empty strings.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect4346(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("test");

            Comment comment = new Comment(doc);
            builder.CurrentParagraph.AppendChild(comment);
            comment.Paragraphs.Add(new Paragraph(doc));
            comment.FirstParagraph.Runs.Add(new Run(doc, "Comment text."));

            Assert.That(comment.Initial, Is.EqualTo(""));
            Assert.That(comment.Author, Is.EqualTo(""));

            TestUtil.SaveOpen(doc, @"Model\Comment\TestDefect4346", lf, sf);
        }

        /// <summary>
        /// Check comment tree read/write.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCommentTree(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Comment\TestCommentTree", TestUtil.BuildScenario(lf, sf, true));

            // Extended comment data is not supported in WordML.
            bool readParents = (lf != LoadFormat.WordML) && (sf != SaveFormat.WordML);

            // Done marks is only supported in DOCX format.
            bool readDone = (lf == LoadFormat.Docx);

            VerifyCommentTree(doc, readParents, readDone);
        }

        /// <summary>
        /// Our validator removes comments and comment ranges from all stories except main text.
        /// </summary>
        [Test]
        public void TestCommentInWrongStories()
        {
            Document doc = new Document();
            doc.RemoveAllChildren();

            Section sect = doc.AppendChild(new Section(doc));
            Body body = sect.AppendChild(new Body(doc));

            // Create a comment in a comment.
            Paragraph para = body.AppendChild(new Paragraph(doc));
            Comment comment = para.AppendChild(new Comment(doc));
            comment.SetText("Comment in main text.");
            para = comment.AppendChild(new Paragraph(doc));
            comment = para.AppendChild(new Comment(doc));
            comment.SetText("Comment in a comment.");

            // Create a comment in a footnote.
            para = body.AppendChild(new Paragraph(doc));
            Footnote footnote = para.AppendChild(new Footnote(doc, FootnoteType.Footnote));
            para = footnote.AppendChild(new Paragraph(doc));
            para.AppendChild(new Run(doc, "Text in a footnote."));
            comment = para.AppendChild(new Comment(doc));
            comment.SetText("Comment in a footnote.");

            // Create a comment in a shape.
            para = body.AppendChild(new Paragraph(doc));
            Shape shape = para.AppendChild(new Shape(doc, ShapeType.Rectangle));
            shape.Width = 100;
            shape.Height = 100;
            para = shape.AppendChild(new Paragraph(doc));
            para.AppendChild(new Run(doc, "Text in a shape."));
            comment = para.AppendChild(new Comment(doc));
            comment.SetText("Comment in a shape.");

            // Create a comment with a range in a header.
            HeaderFooter header = sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.HeaderPrimary));
            para = header.AppendChild(new Paragraph(doc));
            para.AppendChild(new CommentRangeStart(doc, 123));
            para.AppendChild(new Run(doc, "Text in a header."));
            para.AppendChild(new CommentRangeEnd(doc, 123));
            para.AppendChild(CreateComment(doc, 123, "Comment in a header."));
            doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestCommentInWrongStories.docx");
            CheckCommentCounts(doc, 1, 0, 0);
        }

        /// <summary>
        /// Our validator removes comment ranges where end comes before start.
        /// </summary>
        [Test]
        public void TestCommentRangeEndBeforeStart()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.AppendChild(new CommentRangeEnd(doc, 123));    // End before start.
            para.AppendChild(new Run(doc, "Text in a comment."));
            para.AppendChild(new CommentRangeStart(doc, 123));
            para.AppendChild(CreateComment(doc, 123, "Comment 1."));

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestCommentRangeEndBeforeStart.docx");
            CheckCommentCounts(doc, 1, 0, 0);
        }

        /// <summary>
        /// Our validator removes empty comment ranges because MS Word sometimes fails to display comments with such ranges.
        /// </summary>
        [Test]
        public void TestCommentRangeEmpty()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.AppendChild(new CommentRangeStart(doc, 123));  // Start of the empty range.
            para.AppendChild(new CommentRangeStart(doc, 999));  // This is correctly skipped by our validator.
            para.AppendChild(new CommentRangeEnd(doc, 123));    // End of the empty range.
            para.AppendChild(CreateComment(doc, 123, "Comment 1."));
            para.AppendChild(new Run(doc, "Blah."));

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestCommentRangeEmpty.docx");
            CheckCommentCounts(doc, 1, 0, 0);
        }

        /// <summary>
        /// Our validator removes comment ranges that have no end/start or no associated cooment.
        /// </summary>
        [Test]
        public void TestCommentRangeOrphans()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;

            // Comment range without a comment.
            para.AppendChild(new CommentRangeStart(doc, 123));
            para.AppendChild(new Run(doc, "Blah 1."));
            para.AppendChild(new CommentRangeEnd(doc, 123));

            // Comment range start only.
            para.AppendChild(new CommentRangeStart(doc, 124));
            para.AppendChild(CreateComment(doc, 124, "Comment 1."));
            para.AppendChild(new Run(doc, "Blah 2."));

            // Comment range end only.
            para.AppendChild(new CommentRangeEnd(doc, 125));
            para.AppendChild(CreateComment(doc, 125, "Comment 2."));
            para.AppendChild(new Run(doc, "Blah 3."));

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestCommentRangeOrphans.docx");
            CheckCommentCounts(doc, 2, 0, 0);
        }

        /// <summary>
        /// Our validator removes comment ranges if duplicate ids are found,
        /// but preserves comments with duplicate ids by assigning them unique ids.
        /// </summary>
        [Test]
        public void TestCommentDuplicate()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.AppendChild(new CommentRangeStart(doc, 123));
            para.AppendChild(new Run(doc, "Blah 1."));
            para.AppendChild(new CommentRangeEnd(doc, 123));
            para.AppendChild(CreateComment(doc, 123, "Comment 1."));

            // Create a duplicate comment.
            para.AppendChild(new CommentRangeStart(doc, 123));
            para.AppendChild(new Run(doc, "Blah 2."));
            para.AppendChild(new CommentRangeEnd(doc, 123));
            para.AppendChild(CreateComment(doc, 123, "Comment 2."));

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestCommentDuplicate.docx");
            CheckCommentCounts(doc, 2, 1, 1);
        }

        private static Comment CreateComment(Document doc, int id, string text)
        {
            Comment comment = new Comment(doc);
            ((INodeWithAnnotationId)comment).IdInternal = id;
            comment.SetText(text);
            return comment;
        }

        private static void CheckCommentCounts(Document doc, int commentCount, int commentRangeStartCount, int commentRangeEndCount)
        {
            Assert.That(doc.GetChildNodes(NodeType.Comment, true).Count, Is.EqualTo(commentCount));
            Assert.That(doc.GetChildNodes(NodeType.CommentRangeStart, true).Count, Is.EqualTo(commentRangeStartCount));
            Assert.That(doc.GetChildNodes(NodeType.CommentRangeEnd, true).Count, Is.EqualTo(commentRangeEndCount));
        }

        /// <summary>
        /// Cloning of comments does not automatically generate unique ids.
        /// So if you clone and append a section to the same document and then save, the comments will be "fixed" by the validator,
        /// but the comment ranges in the cloned section will be lost. This is not very nice, but okay for now.
        /// </summary>
        [Test]
        public void TestCommentClone()
        {
            Document doc = new Document();
            Comment comment = new Comment(doc);
            Assert.That(comment.Id, Is.EqualTo(0));
            comment = (Comment)comment.Clone(true);
            Assert.That(comment.Id, Is.EqualTo(0));

            CommentRangeStart crs = new CommentRangeStart(doc, 123);
            Assert.That(crs.Id, Is.EqualTo(123));
            crs = (CommentRangeStart)crs.Clone(true);
            Assert.That(crs.Id, Is.EqualTo(123));
        }

        /// <summary>
        /// Check the comment ids are updated when comments are imported into another document.
        /// </summary>
        [Test]
        public void TestCommentImport()
        {
            Document srcDoc = new Document();
            Paragraph para = srcDoc.FirstSection.Body.FirstParagraph;
            para.AppendChild(CreateComment(srcDoc, 111, "Comment 1."));
            para.AppendChild(new CommentRangeStart(srcDoc, 123));
            para.AppendChild(new Run(srcDoc, "Blah."));
            para.AppendChild(new CommentRangeEnd(srcDoc, 123));
            para.AppendChild(CreateComment(srcDoc, 123, "Comment 2."));

            Document dstDoc = new Document();
            dstDoc.RemoveAllChildren();
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            Comment comment = (Comment)dstDoc.GetChild(NodeType.Comment, 0, true);
            Assert.That(comment.Id, Is.EqualTo(0));

            comment = (Comment)dstDoc.GetChild(NodeType.Comment, 1, true);
            Assert.That(comment.Id, Is.EqualTo(1));
            CommentRangeStart start = (CommentRangeStart)dstDoc.GetChild(NodeType.CommentRangeStart, 0, true);
            Assert.That(start.Id, Is.EqualTo(1));
            CommentRangeEnd end = (CommentRangeEnd)dstDoc.GetChild(NodeType.CommentRangeEnd, 0, true);
            Assert.That(end.Id, Is.EqualTo(1));
        }

        /// <summary>
        /// Our document postloader regenerates ids for all loaded comments so we can be sure
        /// we can generate unique ids for the new comments that the user will create.
        /// </summary>
        [Test]
        public void TestCommentAdd()
        {
            Document doc = TestUtil.Open(@"Model\Comment\TestCommentAdd.docx");
            Comment comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
            // In DOC this is a different number, but our postloader regenerates the numbers.
            Assert.That(comment.Id, Is.EqualTo(0));

            // So the next comment the user creates gets a unique id.
            comment = new Comment(doc);
            Assert.That(comment.Id, Is.EqualTo(1));
        }

        [Test]
        public void TestCommentDate()
        {

            Document doc = TestUtil.Open(@"Model\Comment\TestCommentDateTimeJ522.docx");
            NodeCollection nodes = doc.GetChildNodes(NodeType.Comment, true);
            Comment comment1 = (Comment)nodes[0];
            Comment comment2 = (Comment)nodes[1];

            Assert.That(comment1.DateTime, Is.EqualTo(new DateTime(2012, 3, 23, 18, 20, 0)));
            Assert.That(comment2.DateTime, Is.EqualTo(new DateTime(2012, 9, 27, 10, 57, 0)));
        }

        /// <summary>
        /// WORDSNET-14471 Detect comment resolved and comment replies and reply to which comment
        /// Added public API to read-only access to comment hierarchy.
        /// </summary>
        [Test]
        public void TestJira14471()
        {
            Document doc = TestUtil.Open(@"Model\Comment\TestJira14471.docx");
            NodeCollection comments = doc.GetChildNodes(NodeType.Comment, true);

            Comment parentComment = (Comment)comments[0];
            Assert.That(parentComment.Ancestor, Is.Null);
            Assert.That(parentComment.Replies.Count, Is.EqualTo(2));
            Assert.That(parentComment.Done, Is.False);

            Comment undoneComment = (Comment)comments[1];
            Assert.That(undoneComment.Ancestor, IsNot.Null());
            Assert.That(undoneComment.Replies.Count, Is.EqualTo(0));
            Assert.That(undoneComment.Done, Is.False);
            undoneComment.Done = true;
            Assert.That(undoneComment.Done, Is.True);

            Comment doneComment = (Comment)comments[2];
            Assert.That(doneComment.Ancestor, IsNot.Null());
            Assert.That(doneComment.Replies.Count, Is.EqualTo(0));
            Assert.That(doneComment.Done, Is.True);

            // Test that collection are live.
            doneComment.ParentId = Comment.NoParent;
            Assert.That(doneComment.Ancestor, Is.Null);
            Assert.That(parentComment.Replies.Count, Is.EqualTo(1));

            ((INodeWithAnnotationId)parentComment).IdInternal = 1000;
            Assert.That(parentComment.Replies.Count, Is.EqualTo(0));
        }


        /// <summary>
        /// WORDSNET-9959 Tests creation and removal of comment replies.
        /// Added public API to operate comment replies.
        /// </summary>
        [Test]
        public void TestJira9959()
        {
            Document doc = TestUtil.Open(@"Model\Comment\TestJira14471.docx");
            Comment mainComment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
            Assert.That(mainComment.Replies.Count, Is.EqualTo(2));

            DateTime replyDate = new DateTime(2017, 9, 19, 13, 0, 0);
            mainComment.AddReply("John Doe", "JD", replyDate, "New reply");
            Assert.That(mainComment.Replies.Count, Is.EqualTo(3));

            Comment reply = (Comment)doc.GetChild(NodeType.Comment, -1, true);
            Assert.That(reply.Author, Is.EqualTo("John Doe"));
            Assert.That(reply.Initial, Is.EqualTo("JD"));
            Assert.That(reply.DateTime, Is.EqualTo(replyDate));
            Assert.That(reply.GetText().TrimEnd(), Is.EqualTo(ControlChar.AnnotationRef + "New reply"));
            Assert.That(reply.Id, Is.EqualTo(3));
            Assert.That(mainComment.Replies[2], Is.SameAs(reply));

            CommentRangeStart replyStart = (CommentRangeStart)doc.GetChild(NodeType.CommentRangeStart, -1, true);
            Assert.That(replyStart.Id, Is.EqualTo(3));
            Node previousNode = replyStart.PreviousSibling;
            Assert.That(previousNode.NodeType, Is.EqualTo(NodeType.CommentRangeStart));
            Assert.That(((CommentRangeStart)previousNode).Id, Is.EqualTo(2));

            CommentRangeEnd replyEnd = (CommentRangeEnd)doc.GetChild(NodeType.CommentRangeEnd, -1, true);
            Assert.That(replyEnd.Id, Is.EqualTo(3));
            previousNode = replyEnd.PreviousSibling;
            Assert.That(previousNode.NodeType, Is.EqualTo(NodeType.Comment));
            Assert.That(((Comment)previousNode).Id, Is.EqualTo(2));
            Assert.That(previousNode, Is.SameAs(mainComment.Replies[1]));
            Assert.That(replyEnd, Is.SameAs(reply.PreviousSibling));

            mainComment.RemoveReply(mainComment.Replies[1]);
            Assert.That(mainComment.Replies.Count, Is.EqualTo(2));

            foreach (Comment comment in doc.GetChildNodes(NodeType.Comment, true))
                Assert.That(comment.Id, IsNot.EqualTo(2));
            foreach (CommentRangeStart commentStart in doc.GetChildNodes(NodeType.CommentRangeStart, true))
                Assert.That(commentStart.Id, IsNot.EqualTo(2));
            foreach (CommentRangeEnd commentEnd in doc.GetChildNodes(NodeType.CommentRangeEnd, true))
                Assert.That(commentEnd.Id, IsNot.EqualTo(2));

            mainComment.RemoveAllReplies();
            Assert.That(mainComment.Replies.Count, Is.EqualTo(0));

            NodeCollection collection = doc.GetChildNodes(NodeType.Comment, true);
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(collection[0], Is.SameAs(mainComment));

            collection = doc.GetChildNodes(NodeType.CommentRangeStart, true);
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(((CommentRangeStart)collection[0]).Id, Is.EqualTo(0));

            collection = doc.GetChildNodes(NodeType.CommentRangeEnd, true);
            Assert.That(collection.Count, Is.EqualTo(1));
            Assert.That(((CommentRangeEnd)collection[0]).Id, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-9959 Tests that adding a reply to a reply is not allowed like in MS Word.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestAddingReplyToReply()
        {
            Document doc = TestUtil.Open(@"Model\Comment\TestJira14471.docx");
            Comment reply = (Comment)doc.GetChild(NodeType.Comment, 1, true);
            reply.AddReply("John Doe", "JD", new DateTime(2017, 09, 23, 12, 30, 0), "New reply");
        }

        /// <summary>
        /// Tests block-level comment range start/end nodes.
        /// </summary>
        [Test]
        public void TestCommentRanges()
        {
            Document doc = TestUtil.Open(@"Model\Comment\TestBlockLevelCommentRanges.docx");

            List<CommentRangeStart> starts =
                doc.GetChildNodes(NodeType.CommentRangeStart, true).ToList<CommentRangeStart>();
            List<CommentRangeEnd> ends =
                doc.GetChildNodes(NodeType.CommentRangeEnd, true).ToList<CommentRangeEnd>();

            Assert.That(starts.Count, Is.EqualTo(3));
            Assert.That(ends.Count, Is.EqualTo(3));

            Assert.That(starts[0].ParentNode.NodeType, Is.EqualTo(NodeType.Body));
            Assert.That(starts[0].DisplacedBy, Is.EqualTo(DisplacedByType.Next));

            Assert.That(starts[1].ParentNode.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(starts[1].DisplacedBy, Is.EqualTo(DisplacedByType.Prev));

            Assert.That(ends[2].ParentNode.NodeType, Is.EqualTo(NodeType.Body));
            Assert.That(ends[2].DisplacedBy, Is.EqualTo(DisplacedByType.Prev));
        }

        /// <summary>
#pragma warning disable CS0612
#pragma warning restore CS0612
        /// WORDSNET-16092 Sometimes Comment.AddReply added a separate comment, not a reply.
        /// Wrong paragraph identifier might be generated for a comment (negative number).
        /// </summary>
        [Test]
        [NonParallelizable]
        public void TestJira16092()
        {
            Document doc = new Document();

            // Create a main comment.
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Comment mainComment = CreateComment(doc, 1, "Comment 1.");
            para.AppendChild(new CommentRangeStart(doc, 1));
            para.AppendChild(new Run(doc, "Blah."));
            para.AppendChild(new CommentRangeEnd(doc, 1));
            para.AppendChild(mainComment);

            // Generate replies.
            for (int i = 0; i < 20; i++)
            {
                mainComment.AddReply("John Doe", "JD", new DateTime(2017, 12, 14, 18, 48, 0),
                    string.Format("Reply {0}", i + 1));
            }

            // Need to reset RandomUtil test mode to generate real paragraph IDs.
            RandomUtil.ResetTestMode();
            try
            {
                // Save/load the document to paragraph IDs be generated.
                doc = TestUtil.SaveOpen(doc, @"Model\Comment\TestJira16092", UnifiedScenario.Docx2DocxNoGold);
            }
            finally
            {
                RandomUtil.SetTestMode();
            }

            // Check paragraph IDs.
            foreach (Comment comment in doc.GetChildNodes(NodeType.Comment, true))
                Assert.That(comment.LastParagraph.ParaId >= 0, Is.True);
        }

        /// <summary>
        /// WORDSNET-18695 Implementing MS Word behaviour that if a CommetRef node is located above than its
        /// CommentRange in the XML document structure, the CommentRange is moved above the CommetRef and becomes empty.
        /// Now the difference with MS Word is that Aspose.Words removes empty comment ranges like old versions of Word.
        /// </summary>
        [Test]
        public void Test18695()
        {
            TestWarningCallback warnings = new TestWarningCallback();
            LoadOptions options = new LoadOptions();
            options.WarningCallback = warnings;

            Document doc = TestUtil.Open(@"Model\Comment\Test18695.docx", options);

            // The comment range has been removed on annotation validation after loading the document.
            Assert.That(doc.FirstSection.Body.GetChild(NodeType.CommentRangeEnd, 0, true), Is.Null);
            Assert.That(doc.FirstSection.Body.GetChild(NodeType.Comment, 0, true), IsNot.Null());
            Assert.That(warnings.Contains(WarningSource.Unknown, WarningType.UnexpectedContent,
                WarningStrings.CommentAboveCommentRange), Is.True);
        }



        /// <summary>
        /// WORDSNET-21058 Implemented support of commentsIds and commentsExtensible document parts.
        /// Adding a comment to a new document with setting the new properties is tested.
        /// </summary>
        [Test]
        public void Test21058NewCommentsInEmptyDocument()
        {
            Document doc = new Document();

            // Create a comment.
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Comment comment = CreateComment(doc, 999, "Comment 1.");
            para.AppendChild(new CommentRangeStart(doc, 999));
            para.AppendChild(new Run(doc, "Blah."));
            para.AppendChild(new CommentRangeEnd(doc, 999));
            para.AppendChild(comment);

            comment.DateTime = new DateTime(2020, 9, 21, 23, 0, 0);
            comment.DateTimeUtc = new DateTime(2020, 9, 21, 18, 0, 0);
            comment.DurableId = 5;

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\Test21058", UnifiedScenario.Docx2DocxNoGold);

            comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);

            VerifyComment(comment, "Comment 1.", 5, "2020-09-21T18:00:00Z");
            Assert.That(FormatterPal.DateTimeToXmlUtc(comment.DateTime), Is.EqualTo("2020-09-21T23:00:00Z"));
        }

        /// <summary>
        /// WORDSJAVA-2406 Comment.getDateTime(), DigitalSignature.getSignTime() returns date plus current time zone
        /// </summary>
        [Test]
        public void TestJava2406()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            DateTime currentDate = new DateTime();

            Comment comment = new Comment(doc, "Amy Lee", "AL", currentDate);
            builder.CurrentParagraph.AppendChild(comment);

            string savedPath = TestUtil.Save(doc, @"Model\Comment\TestJava2406.docx");

            doc = new Document(savedPath);
            comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);

            Assert.That(comment.DateTime.ToString(), Is.EqualTo(currentDate.ToString()));
        }

        /// <summary>
        /// WORDSNET-21481 Comment time became wrong (UTC) after saving/loading a document.
        /// </summary>
        [Test]
        public void Test21481()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("Some text is added.");

            DateTime date = DateTime.Today;
            Comment comment = new Comment(doc, "Amy Lee", "AL", date);
            comment.SetText("Comment text.");
            builder.CurrentParagraph.AppendChild(comment);

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\Test21481.docx", null, false);
            comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);

            Assert.That(comment.DateTime, Is.EqualTo(date));
        }

        /// <summary>
        /// WORDSNET-21325 Comments date do not reflect to changed time zone.
        /// Comment's date now writes to DateTimeUtc property as well.
        /// </summary>
        [TestCase(DateTimeKind.Utc)]
        [TestCase(DateTimeKind.Local)]
        [TestCase(DateTimeKind.Unspecified)]
        public void Test21325(DateTimeKind kind)
        {
            // Looks like this test works only in Russia TZ Standard Time, does not work in Kyiv time zone.
            if (TimeZone.CurrentTimeZone.StandardName.StartsWith("Russia TZ "))
            {
                // Customer's code
                Document doc = new Document();
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.Write("Some text is added.");

                DateTime date = new DateTime(2020, 11, 26, 12, 30, 30, kind);

                Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

                Comment comment = new Comment(
                    doc,
                    "Author Name",
                    "ÄN",
                    date
                );
                comment.SetText("This is the comment text.");

                paragraph.AppendChild(new CommentRangeStart(doc, comment.Id));
                paragraph.AppendChild(comment);

                Run run = new Run(doc);
                run.Text = "This is the text the comment is applied to.";
                paragraph.AppendChild(run);

                paragraph.AppendChild(new CommentRangeEnd(doc, comment.Id));

                OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
                saveOptions.Compliance = OoxmlCompliance.Iso29500_2008_Strict;
                saveOptions.SaveFormat = SaveFormat.Docx;

                // commentExtensible part containing UTC date a Word2019 feature, we need to set it explicitly.
                doc.CompatibilityOptions.OptimizeFor(Settings.MsWordVersion.Word2019);

                string[] partNames = new string[] {
                @"word/comments.xml",
                @"word/commentsExtensible.xml"};

                XmlDocument[] xmlDocs = DocxExportContext.SaveAndGetXmlDocuments(doc, saveOptions, partNames);

                // Written local value always the same regardless of source DateTime.Kind.
                const string expectedLocal = "2020-11-26T12:30:30Z";

                // Written UTC value depends on source DateTime.Kind.
                // Cannot make this TestCase parameter as it depends on environment UTC offset and daylightSavingTime.
#if JAVA
            TimeSpan daylightSavingTimeDelta = TimeSpan.fromJava(CurrentThread.getTimeZone().getDSTSavings());
#else
                TimeSpan daylightSavingTimeDelta = TimeZone.CurrentTimeZone.GetDaylightChanges(DateTime.Now.Year).Delta;
#endif
                TimeSpan utcOffcet = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

                string expectedUtc = (kind != DateTimeKind.Utc)
                    ? FormatterPal.DateTimeToXmlUtc(date - utcOffcet + daylightSavingTimeDelta)
                    : expectedLocal;

                Assert.That(xmlDocs[0].GetElementsByTagName("w:comment")[0].Attributes["w:date"].Value, Is.EqualTo(expectedLocal));
                Assert.That(xmlDocs[1].GetElementsByTagName("w16cex:commentExtensible")[0].Attributes["w16cex:dateUtc"].Value, Is.EqualTo(expectedUtc));

                // Verify read values.
                doc = TestUtil.SaveOpen(doc, @"Model\Comment\Test21325", UnifiedScenario.Docx2DocxNoGold);
                comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
                Assert.That(FormatterPal.DateTimeToXmlUtc(comment.DateTime), Is.EqualTo(expectedLocal));
                Assert.That(FormatterPal.DateTimeToXmlUtc(comment.DateTimeUtc), Is.EqualTo(expectedUtc));
            }
        }

        /// <summary>
        /// WORDSNET-22313 Aspose.Words throws FileCorruptedException when loading a document.
        /// Fixed by resolving duplicate ParaIds used in comments.
        /// </summary>
        [Test]
        public void Test22313()
        {
            RandomUtil.Reset();

            const string expectedAuthor = "Chaitra Manjunatha";

            Document doc = TestUtil.Open(@"Model\Comment\Test22313", LoadFormat.Docx);
            NodeCollection comments = doc.GetChildNodes(NodeType.Comment, true);
            Assert.That(comments.Count, Is.EqualTo(2));

            Comment comment = comments[0] as Comment;
            Assert.That(comment.Id, Is.EqualTo(0));
            Assert.That(comment.ParentId, Is.EqualTo(-1));
            Assert.That(comment.DurableId, Is.EqualTo(644582707));
            Assert.That(comment.Author, Is.EqualTo(expectedAuthor));
            Assert.That(comment.FirstParagraph.ParaId, Is.EqualTo(1834080397));
            Assert.That(comment.IsDateTimeUtcDefined, Is.False);

            comment = comments[1] as Comment;
            Assert.That(comment.Id, Is.EqualTo(1));
            Assert.That(comment.ParentId, Is.EqualTo(-1));
            Assert.That(comment.DurableId, Is.EqualTo(603259337));
            Assert.That(comment.Author, Is.EqualTo(expectedAuthor));
            Assert.That(comment.FirstParagraph.ParaId, Is.EqualTo(815348239));
            Assert.That(comment.DateTimeUtc, Is.EqualTo(new DateTime(2021, 3, 11, 9, 53, 0)));
        }

        /// <summary>
        /// WORDSNET-22564 Comments for Content controls are messed up after clone.
        /// When cloning nodes, comment/comment range clones had the same ID as the original nodes. When saving a
        /// document, all comment range nodes with duplicate ID are removed. So, in the customer's case, the comment
        /// nodes were preserved, but the comment ranges were lost.
        /// </summary>
        [Test]
        public void Test22564()
        {
            RandomUtil.Reset();

            const string fileName = @"Model\Comment\Test22564.docx";
            Document doc = TestUtil.Open(fileName);

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            foreach (Node node in sdts)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                if (sdt.SdtType != SdtType.RepeatingSection)
                    continue;

                for (int repeat = 2; repeat > 0; repeat--)
                {
                    StructuredDocumentTag clone = (StructuredDocumentTag)sdt.Clone(true);
                    clone.Range.Replace("{{Repeat}}", repeat.ToString());

                    sdt.InsertNext(clone);
                    clone.RemoveSelfOnly();
                }

                sdt.Remove(); // remove section template
            }

            Assert.That(doc.GetChildNodes(NodeType.CommentRangeStart, true).Count, Is.EqualTo(12));

            doc = TestUtil.SaveOpenDocxExportOnly(doc, fileName);

            Assert.That(doc.GetChildNodes(NodeType.CommentRangeStart, true).Count, Is.EqualTo(12));
        }

        /// <summary>
        /// WORDSNET-23936 Reverse order of replies on the comment in the air
        /// A new reply was added after the last node of the <see cref="Comment.Replies"/> collection. But if the
        /// parent comment is not actually within the document, <see cref="Comment.Replies"/> can return wrong data and
        /// the new reply could be added to an unexpected place. Fixed by refusal of using <see cref="Comment.Replies"/>,
        /// but checking nodes after the parent comment node instead.
        /// </summary>
        [Test]
        public void Test23936()
        {
            RandomUtil.Reset();

            Document doc = new Document();

            // Create a paragraph and do not add to the document.
            Paragraph paragraph1 = new Paragraph(doc);

            Comment comment1 = new Comment(doc, "Author", "", new DateTime(2022, 06, 23, 14, 44, 0));
            comment1.SetText("Parent Comment #1");
            paragraph1.AppendChild(comment1);

            AddAndCheckReplies(comment1);

            // Add the parent paragraph to the document.
            doc.FirstSection.Body.AppendChild(paragraph1);

            // Create another paragraph and do not add to the document.
            Paragraph paragraph2 = new Paragraph(doc);

            Comment comment2 = new Comment(doc, "Author", "", new DateTime(2022, 06, 23, 14, 46, 0));
            comment2.SetText("Parent Comment #2");
            paragraph2.AppendChild(new CommentRangeStart(doc, comment2.Id));
            paragraph2.AppendChild(new Run(doc, "Text"));
            paragraph2.AppendChild(new CommentRangeEnd(doc, comment2.Id));
            paragraph2.AppendChild(comment2);

            AddAndCheckReplies(comment2);

            // Add the paragraph to the document.
            doc.FirstSection.Body.AppendChild(paragraph2);

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Comment\Test23936.docx");
        }

        /// <summary>
        /// WORDSNET-24100 Comment.AddReply adds reply as a separate comment instead of reply.
        /// Ignore negative ParaId on document loading.
        /// </summary>
        [Test]
        public void Test24100()
        {
            Document doc = TestUtil.Open(@"Model\Comment\Test24100.docx");
            Comment root = (Comment)doc.GetChild(NodeType.Comment, 0, true);

            // Add a reply, which will show up under its parent comment.
            root.AddReply("Joe Bloggs", "J.B.", new DateTime(2022, 08, 04), "New reply");

            doc = TestUtil.SaveOpen(doc, @"Model\Comment\Test24100.docx");

            Comment replyComment = (Comment)doc.GetChild(NodeType.Comment, 1, true);
            Assert.That(replyComment.ParentId, Is.EqualTo(0));
            Assert.That(replyComment.GetText(), Is.EqualTo("\u0005New reply\r"));
        }

        /// <summary>
        /// Additional test for WORDSNET-24100
        /// </summary>
        [Test]
        public void Test24100A()
        {
            Document doc = TestUtil.Open(@"Model\Comment\Test24100A1.xml");
            Comment comment = (Comment)doc.GetChild(NodeType.Comment, 1, true);

            Assert.That(comment.ParentId, Is.EqualTo(0));
            Assert.That(comment.Done, Is.True);

            doc = TestUtil.Open(@"Model\Comment\Test24100A2.xml");
            comment = (Comment)doc.GetChild(NodeType.Comment, 1, true);

            // Comment extended properties should be lost.
            Assert.That(comment.ParentId, Is.EqualTo(Comment.NoParent));
            Assert.That(comment.Done, Is.False);
        }

        /// <summary>
        /// Tests setting comment ID properties of the <see cref="Comment"/>, <see cref="CommentRangeStart"/>,
        /// <see cref="CommentRangeEnd"/> classes.
        /// </summary>
        [Test]
        public void TestSettingCommentIds()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("This test document contains a ");

            const int commentId = 10;
            CommentRangeStart commentRangeStart = new CommentRangeStart(doc, commentId);
            builder.CurrentParagraph.AppendChild(commentRangeStart);

            const int replyId = 11;
            CommentRangeStart replyRangeStart = new CommentRangeStart(doc, replyId);
            builder.CurrentParagraph.AppendChild(replyRangeStart);

            builder.Write("comment");

            CommentRangeEnd commentRangeEnd = new CommentRangeEnd(doc, commentId);
            builder.CurrentParagraph.AppendChild(commentRangeEnd);

            Comment comment = new Comment(doc, "Test1", "T1", new DateTime(2023, 10, 2, 20, 24, 0));
            comment.Id = commentId;
            builder.CurrentParagraph.AppendChild(comment);

            Paragraph commentParagraph = new Paragraph(doc);
            commentParagraph.AppendChild(new Run(doc, "This is comment text."));
            comment.AppendChild(commentParagraph);

            CommentRangeEnd replyRangeEnd = new CommentRangeEnd(doc, replyId);
            builder.CurrentParagraph.AppendChild(replyRangeEnd);

            Comment reply = new Comment(doc, "Test2", "T2", new DateTime(2023, 10, 2, 20, 25, 0));
            reply.Id = replyId;
            reply.ParentId = commentId;
            builder.CurrentParagraph.AppendChild(reply);

            Paragraph replyParagraph = new Paragraph(doc);
            replyParagraph.AppendChild(new Run(doc, "This is text in the reply."));
            reply.AppendChild(replyParagraph);

            builder.Writeln(" with reply.");

            TestUtil.SaveCheckGoldExportOnly(doc, @"Model\Comment\TestSettingCommentIds.docx");
        }

        /// <summary>
        /// WORDSNET-25975 Comment date is not updated in commentsExtensible.xml
        /// The <see cref="Comment.DateTimeUtc"/> property was not updated when setting comment datetime.
        /// </summary>
        [Test]
        public void Test25975()
        {
            Document doc = TestUtil.Open(@"Model\Comment\Test25975.docx");
            Comment comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);

            Assert.That(comment.DateTimeUtc, Is.EqualTo(new DateTime(2023, 9, 26, 7, 40, 0)));

            comment.DateTime = new DateTime(2020, 1, 1);

            // UtcDateTime differs from DateTime by the offset of the current time zone: just check that the difference
            // is less than one day.
            double difference = comment.DateTime.Subtract(comment.DateTimeUtc).TotalDays;
            Assert.That(System.Math.Abs(difference) < 1, Is.True);

            // Check that the date is written to commentsExtensible.xml part.

            DocxExportContext context = new DocxExportContext(doc, "word/commentsExtensible.xml");
            XmlNode xmlNode = context.GetXmlNode("//w16cex:commentsExtensible/w16cex:commentExtensible");

            // Written value depends on current timezone, thus use UtcDateTime that is already checked above.
            string expectedValue = comment.DateTimeUtc.ToString(@"yyyy-MM-dd\THH:mm:ss\Z");
            Assert.That(xmlNode.Attributes["w16cex:dateUtc"].Value, Is.EqualTo(expectedValue));
        }



        /// <summary>
        /// Adds a reply to the specified comment and checks that the first reply is placed after the comment itself
        /// and that the second reply is placed after the first one.
        /// </summary>
        private static void AddAndCheckReplies(Comment comment)
        {
            Comment previousReply = null;
            for (int i = 1; i <= 2; i++)
            {
                DateTime dateTime = new DateTime(2022, 06, 23, 14, 45, i);
                Comment reply = comment.AddReply("Author", string.Empty, dateTime, "#" + i);

                Assert.That(reply.PreviousNonAnnotationSibling, Is.SameAs((previousReply != null) ? previousReply : comment));
                previousReply = reply;
            }
        }

        /// <summary>
        /// Check comment tree in details.
        /// </summary>
        private static void VerifyCommentTree(Document doc, bool parentRead, bool doneRead)
        {
            VerifyComment(doc, 0, 0, -1, false, parentRead, doneRead);
            VerifyComment(doc, 1, 1, 0, true, parentRead, doneRead);
            VerifyComment(doc, 2, 2, 0, false, parentRead, doneRead);
            VerifyComment(doc, 3, 3, 0, false, parentRead, doneRead);
            VerifyComment(doc, 4, 4, 0, true, parentRead, doneRead);
            VerifyComment(doc, 5, 5, -1, false, parentRead, doneRead);
            VerifyComment(doc, 6, 6, 5, false, parentRead, doneRead);
        }

        private static void VerifyComment(Document doc, int idx, int id, int parentId, bool done, bool parentRead, bool doneRead)
        {
            Comment comment = (Comment)doc.FirstSection.Body.GetChild(NodeType.Comment, idx, true);
            Assert.That(comment.Id, Is.EqualTo(id));

            // ParentId should be -1 if not read.
            Assert.That(comment.ParentId, Is.EqualTo(parentRead ? parentId : -1));

            // Done should be false if not read.
            Assert.That(comment.Done, Is.EqualTo(doneRead ? done : false));
        }

        /// <summary>
        /// Verifies Word 2016+ properties of a comment.
        /// </summary>
        private static void VerifyComment(Comment comment, string text, int durableId, string utcDateTime)
        {
            Assert.That(comment.GetText(), Is.EqualTo("\x0005" + text + "\r"));
            Assert.That(comment.DurableId, Is.EqualTo(durableId));
            if (utcDateTime != null)
                Assert.That(FormatterPal.DateTimeToXmlUtc(comment.DateTimeUtc), Is.EqualTo(utcDateTime));
            else
                Assert.That(comment.IsDateTimeUtcDefined, Is.False);
            Assert.That(comment.IsIntelligentPlaceholder, Is.False);
        }
    }
}
