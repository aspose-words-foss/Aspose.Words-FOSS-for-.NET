// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Aspose.Common;
using Aspose.TestFx;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Replacing;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests revision tracking.
    /// </summary>
    [TestFixture]
    public class TestTrackRevisions
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void TestSetUp()
        {
            RandomUtil.SetTestMode();
        }

        /// <summary>
        /// Tests how a paragraph with single run in it is removed.
        /// Run should be marked as removed. Paragraph should be marked as removed.
        /// </summary>
        [Test]
        public void TestRemoveSingleParagraph()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveSingleParagraph.docx";
            Document doc = TestUtil.Open(testName);

            // Start tracking.
            doc.StartTrackRevisions("aw", gTestDateTime);

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            paragraph.Remove();

            CheckMarkedForDeletion(paragraph.FirstRun);
            CheckMarkedForDeletion(paragraph);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how single run is removed. Paragraph has few runs. First run is removed.
        /// Run should be marked as removed. Paragraph should not be marked as removed.
        /// </summary>
        [Test]
        public void TestRemoveSingleRun()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveSingleRun.docx";
            Document doc = TestUtil.Open(testName);

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            Run run = paragraph.FirstRun;
            CheckNotMarked(run);
            CheckNotMarked(paragraph);

            doc.StartTrackRevisions("aw", gTestDateTime);

            run.Remove();

            CheckMarkedForDeletion(run);
            CheckNotMarked(paragraph);

            TestUtil.Save(doc, testName);
        }


        /// <summary>
        /// Tests how NodeRemover is tracked.
        /// </summary>
        [Test]
        public void TestNodeRemover()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestNodeRemoverDontJoin.docx";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Para1");
            builder.Writeln("Para2");
            builder.Writeln("Para3");
            builder.Writeln("Para4");

            doc.StartTrackRevisions("aw", gTestDateTime);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            NodeRange range = new NodeRange(paras[1], true, paras[2], true);

            NodeRemover nodeRemover = new NodeRemover(range, NodeJoinMode.DontJoin, false);
            nodeRemover.RemoveCore();

            CheckNotMarked(paras[0]);
            CheckNotMarked(paras[0].FirstRun);

            CheckMarkedForDeletion(paras[1]);
            CheckMarkedForDeletion(paras[1].FirstRun);

            CheckNotMarked(paras[2]);
            CheckMarkedForDeletion(paras[2].FirstRun);

            CheckNotMarked(paras[3]);
            CheckNotMarked(paras[3].FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how a run in comment is deleted.
        /// Word does not track changes in comments.
        /// </summary>
        [Test]
        public void TestRemoveRunInComment()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveRunInComment.docx";
            Document doc = TestUtil.Open(testName);

            Comment comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
            Paragraph paragraph = comment.FirstParagraph;
            Run run = paragraph.FirstRun;

            doc.StartTrackRevisions("aw", gTestDateTime);

            CheckNotMarked(run);
            CheckNotMarked(paragraph);
            Assert.That(run.GetText(), Is.EqualTo("Test comment"));

            run.Remove();

            // Verify that run is really removed, not marked.
            CheckNotMarked(run);
            CheckNotMarked(paragraph);
            Assert.That(run.ParentNode, Is.Null);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how shape is deleted. Shape.RunPr should be marked as deleted.
        /// </summary>
        [Test]
        public void TestRemoveShape()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveShape.docx";
            Document doc = TestUtil.Open(testName);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            doc.StartTrackRevisions("aw", gTestDateTime);

            CheckNotMarked(shape);
            shape.Remove();

            CheckMarkedForDeletion(shape);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how textbox is deleted.
        /// Shape.RunPr should be marked but inner paragraphs and runs should not.
        /// </summary>
        [Test]
        public void TestRemoveTextbox()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveTextbox.docx";
            Document doc = TestUtil.Open(testName);

            Shape textbox = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            doc.StartTrackRevisions("aw", gTestDateTime);

            CheckNotMarked(textbox);
            textbox.Remove();

            CheckMarkedForDeletion(textbox);

            // Verify that textbox content is not marked.
            foreach (Paragraph paragraph in textbox.GetChildNodes(NodeType.Paragraph, true))
            {
                CheckNotMarked(paragraph);
                foreach (Run run in paragraph.Runs)
                    CheckNotMarked(run);
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how paragraph with few runs is deleted.
        /// Paragraph should be marked and all runs are marked as well.
        /// </summary>
        [Test]
        public void TestRemoveParagraphA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveParagraphA.docx";
            Document doc = TestUtil.Open(testName);

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[1];

            CheckNotMarked(paragraph);

            doc.StartTrackRevisions("aw", gTestDateTime);
            paragraph.Remove();

            // Paragraph itself marked.
            CheckMarkedForDeletion(paragraph);

            // Each runs are marked too.
            foreach(Run run in paragraph.Runs)
                CheckMarkedForDeletion(run);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how paragraph with few runs and shape is deleted.
        /// Paragraph should be marked, runs should be marked and shape should be marked.
        /// </summary>
        [Test]
        public void TestRemoveParagraphB()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveParagraphB.docx";
            Document doc = TestUtil.Open(testName);

            Paragraph paragraph = doc.FirstSection.Body.Paragraphs[1];

            doc.StartTrackRevisions("aw", gTestDateTime);
            paragraph.Remove();

            CheckMarkedForDeletion(paragraph);

            foreach (Run run in paragraph.Runs)
                CheckMarkedForDeletion(run);

            foreach (Shape shape in paragraph.GetChildNodes(NodeType.Shape, true))
                CheckMarkedForDeletion(shape);

            TestUtil.Save(doc, testName);
        }


        /// <summary>
        /// Test that paragraph which inserted and removed during one edit session is really removed.
        /// Word does this way.
        /// </summary>
        [Test]
        public void TestRemoveParagraphC()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveParagraphC.docx";
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Para1");

            doc.StartTrackRevisions("Author", gTestDateTime);

            builder.Writeln("Para2");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(3));

            CheckNotMarked(paras[0]);
            CheckMarkedForInsertion(paras[1]);

            foreach(Paragraph paragraph in paras)
                paragraph.Remove();

            Assert.That(paras.Count, Is.EqualTo(2));
            CheckMarkedForDeletion(paras[0]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how row is deleted.
        /// Row is marked deleted, every paragraph and run are marked as deleted.
        /// </summary>
        [Test]
        public void TestRemoveRow()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveRow.docx";
            Document doc = TestUtil.Open(testName);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Row row = table.Rows[1];

            doc.StartTrackRevisions("aw", gTestDateTime);
            Assert.That(table.Rows.Count, Is.EqualTo(3));

            row.Remove();

            Assert.That(table.Rows.Count, Is.EqualTo(3));
            CheckMarkedForDeletion(row);

            // Verify that each paragraph and run in row is marked.
            foreach(Paragraph paragraph in row.GetChildNodes(NodeType.Paragraph, true))
            {
                CheckMarkedForDeletion(paragraph);
                foreach(Run run in paragraph.Runs)
                    CheckMarkedForDeletion(run);
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how last run is removed.
        /// This run is last not marked for deletion run in the last not marked for deletion paragraph.
        /// Run is marked for deletion but none parent paragraph and parent row are marked.
        /// </summary>
        [Test]
        public void TestRemoveLastRunA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveLastRunA.docx";
            Document doc = TestUtil.Open(testName);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Run run = (Run)table.Rows[1].Cells[1].Paragraphs[1].LastChild;

            doc.StartTrackRevisions("aw", gTestDateTime);
            run.Remove();

            CheckMarkedForDeletion(run);
            CheckNotMarked(run.ParentParagraph);
            CheckNotMarked(run.ParentParagraph.ParentRow);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Test how all runs are removed one by one from paragraph.
        /// All runs are marked eventually but parent paragraph is not.
        /// </summary>
        [Test]
        public void TestRemoveLastRunB()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveLastRunB.docx";
            Document doc = TestUtil.Open(testName);

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.Runs.Count, Is.EqualTo(7));

            doc.StartTrackRevisions("aw", gTestDateTime);
            foreach (Run run in paragraph.Runs)
                run.Remove();

            // None of runs are actually removed.
            Assert.That(paragraph.Runs.Count, Is.EqualTo(7));

            // But all are marked.
            foreach(Run run in paragraph.Runs)
                CheckMarkedForDeletion(run);

            // And paragraph is not.
            CheckNotMarked(paragraph);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Test how last run is deleted.
        /// Single run in paragraph in cell is deleted. Run is marked, paragraph is NOT marked. Row is NOT marked.
        /// </summary>
        [Test]
        public void TestRemoveLastRunC()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveLastRunC.docx";
            Document doc = TestUtil.Open(testName);

            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);
            Paragraph paragraph = run.ParentParagraph;
            Row row = paragraph.ParentRow;

            doc.StartTrackRevisions("aw", gTestDateTime);
            run.Remove();

            CheckMarkedForDeletion(run);
            CheckNotMarked(paragraph);
            CheckNotMarked(row);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how single paragraph in cell is deleted.
        /// Paragraph should be marked, child run should be marked, parent row should not be marked.
        /// </summary>
        [Test]
        public void TestRemoveLastParagraph()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveLastParagraph.docx";
            Document doc = TestUtil.Open(testName);

            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);
            Paragraph paragraph = run.ParentParagraph;
            Row row = paragraph.ParentRow;

            doc.StartTrackRevisions("aw", gTestDateTime);

            paragraph.Remove();

            CheckMarkedForDeletion(run);
            CheckMarkedForDeletion(paragraph);
            CheckNotMarked(row);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how run insertion is marked.
        /// INserted run should be marked for insertion.
        /// </summary>
        [Test]
        public void TestInsertRun()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertRun.docx";
            Document doc = TestUtil.Open(testName);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            doc.StartTrackRevisions("aw", gTestDateTime);

            builder.Write("another test");

            Run run1 = (Run)doc.GetChild(NodeType.Run, 0, true);
            Run run2 = (Run)doc.GetChild(NodeType.Run, 1, true);
            Paragraph paragraph = run1.ParentParagraph;

            // Run was in document already.
            CheckNotMarked(run1);

            // Parent paragraph is not marked.
            CheckNotMarked(paragraph);

            // Inserted run.
            CheckMarkedForInsertion(run2);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how paragraph is inserted. Insert paragraph using DocumentBuilder at the document end.
        /// </summary>
        [Test]
        public void TestInsertParagraphA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertParagraphA.docx";
            Document doc = TestUtil.Open(testName);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();

            doc.StartTrackRevisions("aw", gTestDateTime);

            builder.InsertParagraph();

            // First paragraph (whose after new paragraph is inserted should be marked).
            CheckMarkedForInsertion(doc.FirstSection.Body.Paragraphs[0]);
            CheckNotMarked(doc.FirstSection.Body.Paragraphs[1]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how paragraph is inserted with DocumentBuilder.
        /// Insert paragraph with text using DocumentBuilder between existed paragraphs.
        /// </summary>
        [Test]
        public void TestInsertParagraphB()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertParagraphB.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(0, -1);

            builder.InsertParagraph();
            builder.Write("Para1.5");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // Only first (whose after was inserted new paragraph) is marked.
            CheckMarkedForInsertion(paras[0]);
            CheckNotMarked(paras[1]);
            CheckNotMarked(paras[2]);

            // Inserted Run is marked.
            CheckMarkedForInsertion(paras[1].FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Paragraph is split in the middle.
        /// Actually it means that we insert paragraph break character.
        /// First paragraph is marked for insertion (in facts paragraph break character). Runs are not marked.
        /// </summary>
        [Test]
        public void TestInsertParagraphC()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertParagraphC.docx";
            Document doc = TestUtil.Open(testName);

            doc.StartTrackRevisions("aw", gTestDateTime);

            Body body = doc.FirstSection.Body;
            Paragraph paragraph = body.FirstParagraph;

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveTo(paragraph.Runs[1]);

            builder.InsertParagraph();

            Assert.That(body.Paragraphs.Count, Is.EqualTo(2));
            // Only first paragraph get InsertRevision.
            CheckMarkedForInsertion(body.Paragraphs[0]);
            CheckNotMarked(body.Paragraphs[1]);

            // None of run are marked.
            foreach (Run run in body.GetChildNodes(NodeType.Run, true))
                CheckNotMarked(run);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how paragraph is inserted. Few paragraph copy-pasted from source document.
        /// Every paragraph except last has insertion. Every run has insertion. Shape has insertion.
        /// </summary>
        [Test]
        public void TestInsertParagraphD()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertParagraphD.docx";
            Document doc = TestUtil.Open(testName);

            Document src = TestUtil.Open(@"Model\Revision\TrackRevisions\Source1.docx");

            doc.StartTrackRevisions("aw", gTestDateTime);

            // import whole document.
            foreach (Paragraph p in src.FirstSection.Body.Paragraphs)
                doc.FirstSection.Body.AppendChild(doc.ImportNode(p, true));

            // Verify content is properly imported.
            foreach (Paragraph paragraph in doc.GetChildNodes(NodeType.Paragraph, true))
            {
                if (!paragraph.IsFirstChild)
                {
                    CheckMarkedForInsertion(paragraph);

                    foreach (Run run in paragraph.Runs)
                        CheckMarkedForInsertion(run);

                    foreach (Shape shape in paragraph.GetChildNodes(NodeType.Shape, true))
                        Assert.That(shape.RunPr.HasInsertRevision, Is.True);
                }
            }

            TestUtil.Save(doc, testName);
        }


        /// <summary>
        /// Tests how run inserted under tracking is deleted. Insert and delete during one session.
        /// Run should be really deleted.
        /// </summary>
        [Test]
        public void TestDeleteInsertedRunA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteInsertedRunA.docx";
            Document doc = new Document();
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insertion text.
            builder.Write("inserted text");

            Paragraph p = doc.FirstSection.Body.FirstParagraph;

            // Verify that text was tracked.
            Assert.That(p.Runs.Count, Is.EqualTo(1));
            CheckMarkedForInsertion(p.FirstRun);

            // And remove it immediately.
            p.FirstRun.Remove();

            // Verify that run was really removed because
            // both insertion and deletion were made during one edit session.
            Assert.That(p.Runs.Count, Is.EqualTo(0));

            RevisionCollection revisions = new RevisionCollection(doc);
            Assert.That(revisions.Count, Is.EqualTo(0));

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how run inserted under tracking is deleted. Insert and delete during different sessions.
        /// Run should be marked as deleted.
        /// </summary>
        [Test]
        public void TestDeleteInsertedRunB()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteInsertedRunB.docx";
            Document doc = new Document();
            doc.StartTrackRevisions("John", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insertion text.
            builder.Write("inserted text");

            Paragraph p = doc.FirstSection.Body.FirstParagraph;

            // Verify that text was tracked.
            Assert.That(p.Runs.Count, Is.EqualTo(1));
            CheckMarkedForInsertion(p.FirstRun);

            // Session is always restarted regardless Author name and DateTime.
            doc.StartTrackRevisions("Tom", gTestDateTime);

            // And remove it in "another" session.
            p.FirstRun.Remove();

            // Verify that run is still here and marked for both insertion and deletion.
            Assert.That(p.Runs.Count, Is.EqualTo(1));
            CheckMarkedForInsertion(p.FirstRun);
            CheckMarkedForDeletion(p.FirstRun);

            RevisionCollection revisions = new RevisionCollection(doc);
            Assert.That(revisions.Count, Is.EqualTo(2));
            Assert.That(revisions[0].Author, Is.EqualTo("John"));
            Assert.That(revisions[1].Author, Is.EqualTo("Tom"));

            TestUtil.Save(doc, testName);
        }



        /// <summary>
        /// Tests how run text changes are tracked. New value is empty string.
        /// </summary>
        [Test]
        public void TestChangeRunTextEmpty()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestChangeRunTextEmpty.docx";
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("Hello!");

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.Runs.Count, Is.EqualTo(1));
            doc.StartTrackRevisions("aw", gTestDateTime);

            Run run = para.FirstRun;
            run.Text = "";

            // Verify that new run for old text is created and marked for deletion while
            // this run is marked for insertion and has empty text.
            Assert.That(para.Runs.Count, Is.EqualTo(2));

            // New text run.
            run = para.Runs[0];
            Assert.That(run.Text, Is.EqualTo(""));
            CheckMarkedForInsertion(run);

            // Old text run.
            run = para.Runs[1];
            Assert.That(run.Text, Is.EqualTo("Hello!"));
            CheckMarkedForDeletion(run);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how table insertion is tracked.
        /// Row is tracked, cell paragraph is tracked, run is tracked.
        /// </summary>
        [Test]
        public void TestInsertTable()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertTable.docx";
            Document doc = TestUtil.Open(testName);

            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.StartTable();
            builder.InsertCell();
            builder.Write("Cell");
            builder.EndRow();
            builder.EndTable();

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Row row = table.FirstRow;
            CheckMarkedForInsertion(row);

            Cell cell = row.FirstCell;
            Paragraph p = cell.FirstParagraph;
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests DocumentBuilder.WriteLn. It actually inserts paragraph and run at once.
        /// </summary>
        [Test]
        public void TestWriteLn()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestWriteLn.docx";
            Document doc = TestUtil.Open(testName);

            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(1, 0);
            builder.Writeln("3");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(5));

            // No revisions in para 0
            Paragraph p = paras[0];
            CheckNotMarked(p);
            CheckNotMarked(p.FirstRun);

            // para 1 and child run are marked
            p = paras[1];
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);
            Assert.That(p.FirstRun.GetText(), Is.EqualTo("3"));

            // No revisions in para 2 and all next
            p = paras[2];
            CheckNotMarked(p);
            CheckNotMarked(p.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how document is built when TrackRevisions option is changed during construction.
        /// </summary>
        [Test]
        public void TestTrackRevisionStateChanged()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestTrackRevisionStateChanged.docx";
            Document doc = new Document();

            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, "FirstRun"));

            doc.StartTrackRevisions("aw", gTestDateTime);

            doc.FirstSection.Body.AppendChild(new Paragraph(doc));
            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, "SecondRun"));

            doc.StopTrackRevisions();

            doc.FirstSection.Body.AppendChild(new Paragraph(doc));
            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, "ThirdRun"));

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(3));

            Paragraph p = paras[0];
            CheckNotMarked(p);
            CheckNotMarked(p.FirstRun);

            p = paras[1];
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);

            p = paras[2];
            CheckNotMarked(p);
            CheckNotMarked(p.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Test insertBefore and appending paragraph with a run inside.
        /// First should be marked and second should not.
        /// </summary>
        [Test]
        public void TestInsertBefore()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertBefore.docx";
            Document doc = new Document();

            doc.FirstSection.Body.LastParagraph.AppendChild(new Run(doc, "FirstRun"));

            doc.StartTrackRevisions("aw", gTestDateTime);

            Paragraph p = new Paragraph(doc);
            p.AppendChild(new Run(doc, "ZeroRun"));

            doc.FirstSection.Body.InsertBefore(p, doc.FirstSection.Body.FirstParagraph);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(2));

            p = paras[0];
            Assert.That(p.FirstRun.GetText(), Is.EqualTo("ZeroRun"));
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);

            p = paras[1];
            CheckNotMarked(p);
            CheckNotMarked(p.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how header tracked.
        /// </summary>
        [Test]
        public void TestWriteLnInHeader()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestWriteLnInHeader.docx";
            Document doc = TestUtil.Open(testName);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Writeln("WriteLn");

            // Verify that no marked insertion are made.
            foreach (Paragraph paragraph in doc.GetChildNodes(NodeType.Paragraph, true))
            {
                CheckNotMarked(paragraph);
                foreach (Run run in paragraph.Runs)
                    CheckNotMarked(run);
            }

            doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            builder = new DocumentBuilder(doc);
            builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
            builder.Writeln("WriteLn");

            // Asserts that every paragraphs and runs in header has insert revision
            foreach (Paragraph paragraph in doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Paragraphs)
            {
                CheckMarkedForInsertion(paragraph);
                foreach (Run run in paragraph.Runs)
                    CheckMarkedForInsertion(run);
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Test what's going on when we delete run already marked for deletion.
        /// Run should be marked once again with new user.
        /// </summary>
        [Test]
        public void TestDeleteAlreadyDeleted()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteAlreadyDeleted.docx";
            Document doc = TestUtil.Open(testName);

            RevisionCollection revisions = new RevisionCollection(doc);

            // Both run are marked for deletion.
            Assert.That(revisions.Count, Is.EqualTo(2));
            Assert.That(revisions[0].Author, Is.EqualTo("user"));
            Assert.That(revisions[1].Author, Is.EqualTo("user"));

            doc.StartTrackRevisions("William", gTestDateTime);

            doc.FirstSection.Body.FirstParagraph.FirstRun.Remove();

            // Assert that there are still two revisions but first is new one.
            Assert.That(revisions.Count, Is.EqualTo(2));
            Assert.That(revisions[0].Author, Is.EqualTo("William"));
            Assert.That(revisions[1].Author, Is.EqualTo("user"));

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how footnote is inserted.
        /// </summary>
        [Test]
        public void TestInsertFootnote()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertFootnote.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentEnd();
            builder.InsertFootnote(FootnoteType.Footnote, "Inserted footnote.");

            Footnote footnote = (Footnote)doc.GetChild(NodeType.Footnote, 0, true);
            Assert.That(footnote.RunPr.HasInsertRevision, Is.True);

            Paragraph p = footnote.FirstParagraph;
            CheckMarkedForInsertion(p.Runs[0]);
            CheckMarkedForInsertion(p.Runs[1]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how multi line footnote is inserted.
        /// All runs and paragraphs inside footnote should be marked.
        /// </summary>
        [Test]
        public void TestInsertMultilineFootnote()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertMultilineFootnote.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.InsertFootnote(FootnoteType.Footnote, "Para1\nPara2");

            Footnote footnote = (Footnote)doc.GetChild(NodeType.Footnote, 0, true);
            Assert.That(footnote.RunPr.HasInsertRevision, Is.True);
            foreach(Paragraph paragraph in footnote.Paragraphs)
            {
                CheckMarkedForInsertion(paragraph);
                foreach(Run run in paragraph.Runs)
                    CheckMarkedForInsertion(run);
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how bookmark is inserted.
        /// Run should be marked for insertion.
        /// </summary>
        [Test]
        public void TestInsertBookmark()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertBookmark.docx";
            Document doc = TestUtil.Open(testName);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.InsertParagraph();

            doc.StartTrackRevisions("aw", gTestDateTime);

            builder.StartBookmark("Test");
            builder.Write("BOOKMARK");
            builder.EndBookmark("Test");

            Paragraph paragraph = doc.FirstSection.Body.LastParagraph;
            CheckNotMarked(paragraph);
            CheckMarkedForInsertion(paragraph.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how DocumentBuilder.InsertBreak(BreakType.ColumnBreak) works under tracking.
        /// Column break character should be marked.
        /// </summary>
        [Test]
        public void TestInsertColumnBreak()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertColumnBreak.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(1, 0);

            builder.InsertBreak(BreakType.ColumnBreak);

            Paragraph p = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(p.Runs.Count, Is.EqualTo(2));
            // First run (column break character itself) is marked.
            Assert.That(p.Runs[0].GetText(), Is.EqualTo("\x0e"));
            CheckMarkedForInsertion(p.Runs[0]);

            // Second is not.
            CheckNotMarked(p.Runs[1]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how DocumentBuilder.InsertBreak(BreakType.PageBreak) works under tracking.
        /// PageBreak character should be tracked.
        /// </summary>
        [Test]
        public void TestInsertPageBreak()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertPageBreak.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(1, 0);

            builder.InsertBreak(BreakType.PageBreak);

            Paragraph p = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(p.Runs.Count, Is.EqualTo(2));

            // First run (page break character itself) is marked.
            CheckMarkedForInsertion(p.Runs[0]);

            // Second is not.
            CheckNotMarked(p.Runs[1]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how DocumentBuilder.InsertBreak(BreakType.LineBreak) works under tracking.
        /// </summary>
        [Test]
        public void TestInsertLineBreak()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertLineBreak.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(1, 0);

            builder.InsertBreak(BreakType.LineBreak);

            Paragraph p = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(p.Runs.Count, Is.EqualTo(2));

            // First run (page break character itself) is marked.
            CheckMarkedForInsertion(p.Runs[0]);

            // Second is not.
            CheckNotMarked(p.Runs[1]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how DocumentBuilder.InsertBreak(BreakType.SectionBreak) works under tracking.
        /// Last paragraph in section should be marked. New section runs and paragraphs should not.
        /// </summary>
        [Test]
        public void TestInsertSectionBreakA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertSectionBreakA.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToParagraph(1, 0);

            builder.InsertBreak(BreakType.SectionBreakNewPage);

            // Verify that new section is created.
            Assert.That(doc.Sections.Count, Is.EqualTo(2));

            // Had problem with DeleteRevision. Verify it absent.
            foreach (Paragraph paragraph in doc.GetChildNodes(NodeType.Paragraph, true))
            {
                CheckNotMarkedForDeletion(paragraph);
                foreach(Run run in paragraph.Runs)
                    CheckNotMarkedForDeletion(run);
            }

            // This is "section break" paragraph.
            Paragraph p = doc.FirstSection.Body.Paragraphs[1];
            // Has no child.
            Assert.That(p.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
            CheckMarkedForInsertion(p);

            // This (old content moved to new section) should not be marked.
            p = doc.Sections[1].Body.FirstParagraph;
            CheckNotMarked(p);
            Assert.That(p.Runs[0].GetText(), Is.EqualTo("After"));
            CheckNotMarked(p.Runs[0]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests DocumentBuilder.DeleteRow.
        /// Row should be marked. Every paragraphs and runs within row should be marked.
        /// </summary>
        [Test]
        public void TestRemoveRowA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveRowA.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.DeleteRow(0, 1);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            // Deleted row
            Row row = table.Rows[1];
            CheckMarkedForDeletion(row);

            foreach(Paragraph paragraph in row.GetChildNodes(NodeType.Paragraph, true))
            {
                CheckMarkedForDeletion(paragraph);
                foreach(Run run in paragraph.Runs)
                    CheckMarkedForDeletion(run);
            }

            TestUtil.Save(doc, testName);
        }


        /// <summary>
        /// Tests how section is removed.
        /// We have no analogue to remove section break because we have no section break at all, we have sections instead.
        /// So I think we should mark whole section content as deleted.
        /// </summary>
        [Test]
        public void TestDeleteSectionBreak()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteSectionBreak.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            doc.Sections[1].Remove();

            // Verify that there is still two sections.
            Assert.That(doc.Sections.Count, Is.EqualTo(2));

            Paragraph paragraph = doc.Sections[1].Body.FirstParagraph;
            CheckMarkedForDeletion(paragraph);
            CheckMarkedForDeletion(paragraph.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how StructuredDocumentTag is inserted.
        /// </summary>
        [Test]
        public void TestInsertSdt()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteSectionBreak.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, MarkupLevel.Block);
            sdt.UpdateId();
            Paragraph p = sdt.AppendChild(new Paragraph(doc));
            p.AppendChild(new Run(doc, "Sample text"));
            p.AppendChild(new Run(doc, "."));
            doc.FirstSection.Body.AppendChild(sdt);

            GlossaryDocument glossaryDocument = new GlossaryDocument();
            doc.GlossaryDocument = glossaryDocument;

            BuildingBlock buildingBlock = new BuildingBlock(doc.GlossaryDocument);
            buildingBlock.AppendChild(new Section(doc.GlossaryDocument));
            buildingBlock.FirstSection.AppendChild(new Body(doc.GlossaryDocument));
            buildingBlock.FirstSection.Body.AppendChild(new Paragraph(doc.GlossaryDocument));
            buildingBlock.FirstSection.Body.FirstParagraph.AppendChild(new Run(doc.GlossaryDocument, "Click here to enter text."));

            doc.GlossaryDocument.AppendChild(buildingBlock);

            // Verify that runs in glossary document is not marked.
            foreach(Run run in glossaryDocument.GetChildNodes(NodeType.Run, true))
                CheckNotMarked(run);

            // Verify that all runs in SDT marked.
            foreach (Run run in sdt.GetChildNodes(NodeType.Run, true))
                CheckMarkedForInsertion(run);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Load document with TrackRevision flag test.
        /// During load process some components may do node changing which caused tracking saved.
        /// Verify that no revisions are made during document loading process.
        /// </summary>
        [Test]
        public void TestLoadDocumentA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestLoadDocumentA.docx";
            Document doc = TestUtil.Open(testName);

            Assert.That(doc.TrackRevisions, Is.True);
            CheckNotMarked(doc.FirstSection.Body.Paragraphs);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests that EditSession class information data written into revisions.
        /// </summary>
        [Test]
        public void TestEditSessionA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestEditSessionA.docx";
            Document doc = new Document();

            // Build paragraphs with some runs and shape.
            Paragraph p = new Paragraph(doc);
            p.AppendChild(new Run(doc, "First run"));
            p.AppendChild(new Run(doc, "Second run"));

            DateTime dateTime = new DateTime(2014, 2, 2);
            doc.StartTrackRevisions("amorozov", dateTime);

            doc.FirstSection.Body.AppendChild(p);

            CheckMarkedForInsertion(p);
            foreach (Run run in p.Runs)
                CheckMarkedForInsertion(run);

            RevisionCollection revisions = new RevisionCollection(doc);

            Assert.That(revisions.Count, Is.EqualTo(3));
            foreach(Revision revision in revisions)
            {
                Assert.That(revision.Author, Is.EqualTo("amorozov"));
                Assert.That(revision.DateTime, Is.EqualTo(dateTime));
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests DrawingML insertion.
        /// DrawingML.RunPr should be marked for insertion.
        /// </summary>
        [Test]
        public void TestInsertDrawingML()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertDrawingML.docx";
            Document dst = new Document();
            Document src = TestUtil.Open(testName);

            dst.StartTrackRevisions("aw", gTestDateTime);

            dst.AppendDocument(src, ImportFormatMode.KeepSourceFormatting);

            Shape dml = (Shape)dst.GetChild(NodeType.Shape, 0, true);
            CheckMarkedForInsertion(dml);

            TestUtil.Save(dst, testName);
        }

        /// <summary>
        /// Tests how update fields tracked. It seems it's very hard to track field update.
        /// I think will can do limitation in the first version and do not track UpdateFields.
        /// </summary>
        [Test]
        public void TestUpdateField()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestUpdateField.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("aw", gTestDateTime);

            Assert.That(doc.BuiltInDocumentProperties.Title, Is.EqualTo("Test title"));
            doc.BuiltInDocumentProperties.Title = "Updated title";

            Paragraph p = doc.FirstSection.Body.FirstParagraph;
            Assert.That(p.Runs.Count, Is.EqualTo(2));       // field code and field result

            doc.UpdateFields();

            Assert.That(p.Runs.Count, Is.EqualTo(2));       // field code and field result

            CheckNotMarked(p);
            foreach(Run run in p.Runs)
                CheckNotMarked(run);

            TestUtil.Save(doc, testName);
        }

        // FOSS: TestLoadDocumentB removed — it verified that a binary-.doc SHAPE-field transformation on load
        // produces no spurious insert/delete revisions. Converting the .doc input to .docx (in Word) bakes in a
        // real insert revision, so the .doc-specific scenario no longer holds.

        /// <summary>
        /// Tests how table is deleted.
        /// Every row should be marked. Every paragraph and every run should be marked.
        /// </summary>
        [Test]
        public void TestDeleteTable()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteTable.docx";
            Document doc = TestUtil.Open(testName);
            doc.StartTrackRevisions("user", new DateTime(2014, 7, 4, 14, 19, 0));

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            table.Remove();

            // Table is still here.
            Assert.That(table.Document, IsNot.Null());

            foreach (Row row in table.Rows)
            {
                CheckMarkedForDeletion(row);
                foreach(Cell cell in row.Cells)
                {
                    foreach (Paragraph paragraph in cell.Paragraphs)
                    {
                        CheckMarkedForDeletion(paragraph);
                        foreach(Run run in paragraph.Runs)
                            CheckMarkedForDeletion(run);
                    }
                }
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how cell is deleted.
        /// Word doesn't track cell deletion  so cell should be deleted.
        /// </summary>
        [Test]
        public void TestDeleteCell()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDeleteCell.docx";
            Document doc = TestUtil.Open(testName);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.That(table.FirstRow.Cells.Count, Is.EqualTo(6));

            doc.StartTrackRevisions("aw", gTestDateTime);
            table.FirstRow.Cells[2].Remove();

            Assert.That(table.FirstRow.Cells.Count, Is.EqualTo(5));

            // Verify no revisions marks.
            CheckNotMarked(doc.FirstSection.Body.Paragraphs);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how cell is inserted.
        /// Paragraph inside cell is marked.
        /// </summary>
        [Test]
        public void TestInsertCell()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertCell.docx";
            Document doc = TestUtil.Open(testName);

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);

            Assert.That(table.FirstRow.Cells.Count, Is.EqualTo(6));

            Cell refCell = table.FirstRow.Cells[1];

            // Construct new cell.
            Cell newCell = new Cell(doc);
            newCell.AppendChild(new Paragraph(doc));
            newCell.LastParagraph.AppendChild(new Run(doc, "New Cell"));

            newCell.AppendChild(new Paragraph(doc));
            newCell.LastParagraph.AppendChild(new Run(doc, "inserted by DocumentBuilder"));

            doc.StartTrackRevisions("aw", gTestDateTime);

            table.FirstRow.InsertBefore(newCell, refCell);

            Assert.That(table.FirstRow.Cells.Count, Is.EqualTo(7));

            CheckMarkedForInsertion(newCell.Paragraphs[0]);
            CheckMarkedForInsertion(newCell.Paragraphs[0].FirstRun);

            CheckMarkedForInsertion(newCell.Paragraphs[1]);
            CheckMarkedForInsertion(newCell.Paragraphs[1].FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how Document.RemoveAllChildren works. Document has Comments.
        /// Paragraphs and runs within comments should not be marked. Other should be marked.
        /// </summary>
        [Test]
        public void TestRemoveAllChildren()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveAllChildren.docx";
            Document doc = TestUtil.Open(testName);

            doc.StartTrackRevisions("aw", gTestDateTime);

            doc.RemoveAllChildren();

            // All content except Comment is marked.
            foreach (Paragraph paragraph in doc.FirstSection.Body.Paragraphs)
            {
                CheckMarkedForDeletion(paragraph);

                foreach(Run run in paragraph.Runs)
                    CheckMarkedForDeletion(run);
            }

            // Comment content is not marked and not deleted in this case.
            Comment comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
            foreach (Paragraph paragraph in comment.Paragraphs)
            {
                CheckNotMarked(paragraph);

                foreach (Run run in paragraph.Runs)
                    CheckNotMarked(run);
            }

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how SDT is updated when document is saved to format where SDT is unsupported.
        /// </summary>
        [Test]
        public void TestUpdateSdt()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira9999.docx");
            doc.StartTrackRevisions("aw", gTestDateTime);

            // Let SDT updater work.
            doc.Save(new MemoryStream(), SaveFormat.Docx);   // FOSS: was Doc

            CheckNotMarked(doc.FirstSection.Body.Paragraphs);
        }

        /// <summary>
        /// Tests how DocumentBuilder works under tracking.
        /// </summary>
        [Test]
        public void TestDocumentBuilderA()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDocumentBuilderA.docx";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("TrackRevision test.");

            builder.StartTable();
            builder.InsertCell();
            builder.Writeln("Cell 1");

            builder.InsertCell();
            builder.Writeln("Cell 3");

            doc.StartTrackRevisions("Alexey Morozov", gTestDateTime);

            Cell cell = builder.CurrentParagraph.ParentCell;
            cell.RemoveAllChildren();
            builder.Writeln("Cell 2");

            builder.EndRow();

            builder.InsertCell();
            builder.Writeln("Second row first cell");
            builder.InsertCell();
            builder.Writeln("Second row second cell");
            builder.EndRow();

            builder.EndTable();

            builder.MoveToDocumentStart();
            builder.CurrentParagraph.Remove();
            builder.Writeln("This is test document to see how DocumentBuilder works when TrackRevision option is turned on.");

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            // First row is not tracked.
            CheckNotMarked(table.Rows[0]);
            // Second row was tracked.
            CheckMarkedForInsertion(table.Rows[1]);

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            CheckMarkedForInsertion(paragraph);

            cell = table.Rows[0].Cells[1];
            // Text was changed in this cell.
            Assert.That(cell.Paragraphs.Count, Is.EqualTo(3));
            paragraph = cell.Paragraphs[0];
            Assert.That(paragraph.FirstRun.GetText(), Is.EqualTo("Cell 3"));
            CheckMarkedForDeletion(paragraph);
            CheckMarkedForDeletion(paragraph.FirstRun);
            CheckNotMarkedForInsertion(paragraph);

            paragraph = cell.Paragraphs[1];
            Assert.That(paragraph.FirstRun.GetText(), Is.EqualTo("Cell 2"));
            CheckMarkedForInsertion(paragraph);
            CheckMarkedForInsertion(paragraph.FirstRun);

            TestUtil.Save(doc, testName);

            doc.AcceptAllRevisions();
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText().StartsWith("This is test"), Is.True);
            table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.Rows.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests revision made by few authors.
        /// </summary>
        [Test]
        public void TestDocumentBuilderB()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestDocumentBuilderB.docx";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            doc.StartTrackRevisions("Guildenstern", gTestDateTime);
            builder.Writeln("How long have you suffered from a bad memory?");
            doc.StartTrackRevisions("Rosencrantz", gTestDateTime);
            builder.Writeln("I can't remember.");

            doc.StartTrackRevisions("Tom", gTestDateTime);
            builder.Writeln("You're both dead!");
            doc.RemoveAllChildren();

            doc.StartTrackRevisions("Guildenstern", gTestDateTime);
            builder.Writeln(" What are you going to do now?");

            TestUtil.Save(doc, testName);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // First was inserted by Guildenstern and then deleted by Tom.
            Paragraph p = paras[0];
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);
            CheckMarkedForDeletion(p);
            CheckMarkedForDeletion(p.FirstRun);

            // Second was inserted by Rosencrantz and then deleted by Tom.
            p = paras[1];
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);
            CheckMarkedForDeletion(p);
            CheckMarkedForDeletion(p.FirstRun);

            // There should be "You're both dead!" paragraph inserted by Tom but because it was deleted
            // during the same session it was really deleted.
            Assert.That(doc.GetText().Contains("You're both dead!"), Is.False);

            // Third was last paragraph and marked for deletion during Tom's Document.RemoveAll.
            p = paras[2];
            CheckMarkedForDeletion(p);

            // But Guildenstern made new insertion to it.
            CheckMarkedForInsertion(p);
            CheckMarkedForInsertion(p.FirstRun);
            CheckMarkedForDeletion(p);
            CheckNotMarkedForDeletion(p.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests that tracking is enabled for FindReplace. Full run replacement.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void TestFindReplaceA(bool trackRevisions)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("amorozov");
            builder.Writeln("amorozov");

            if(trackRevisions)
                doc.StartTrackRevisions("aw", gTestDateTime);

            FindReplaceOptions options = new FindReplaceOptions()
            {
                MatchCase = true,
                FindWholeWordsOnly = true
            };
            FindReplace findReplace = new FindReplace(doc, "amorozov", "morzal", options);

            findReplace.Replace();

            // First two paragraphs should be the same.
            for(int i = 0; i < 2; i++)
            {
                Paragraph para = doc.FirstSection.Body.Paragraphs[i];

                CheckNotMarked(para);

                if (trackRevisions)
                {
                    Assert.That(para.Runs.Count, Is.EqualTo(2));
                    Assert.That(para.Runs[0].Text, Is.EqualTo("amorozov"));
                    CheckMarkedOnlyForDeletion(para.Runs[0]);

                    Assert.That(para.Runs[1].Text, Is.EqualTo("morzal"));
                    CheckMarkedOnlyForInsertion(para.Runs[1]);
                }
                else
                {
                    Assert.That(para.Runs.Count, Is.EqualTo(1));
                    Assert.That(para.Runs[0].Text, Is.EqualTo("morzal"));
                    CheckNotMarked(para.Runs[0]);
                }
            }
        }

        /// <summary>
        /// Tests that tracking is enabled for FindReplace. Part of run replacement.
        /// </summary>
        [Test]
        public void TestFindReplaceB()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestFindReplaceB.docx";
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("amorozov");
            doc.StartTrackRevisions("aw", gTestDateTime);

            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            Assert.That(paragraph.Runs.Count, Is.EqualTo(1));

            FindReplaceOptions options = new FindReplaceOptions()
            {
                MatchCase = true,
                FindWholeWordsOnly = false
            };
            FindReplace findReplace = new FindReplace(doc, "o", "0", options);
            int replacementCount = findReplace.Replace();
            Assert.That(replacementCount, Is.EqualTo(3));

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            CheckNotMarked(para);

            RunCollection runs = para.Runs;
            // "am"
            CheckNotMarked(runs[0]);

            // "o" - deleted, "0" inserted
            CheckMarkedOnlyForDeletion(runs[1]);
            CheckMarkedOnlyForInsertion(runs[2]);

            // "r"
            CheckNotMarked(runs[3]);

            // "o" - deleted, "0" inserted
            CheckMarkedOnlyForDeletion(runs[4]);
            CheckMarkedOnlyForInsertion(runs[5]);

            // "z".
            CheckNotMarked(runs[6]);

            // "o" - deleted, "0" inserted
            CheckMarkedOnlyForDeletion(runs[7]);
            CheckMarkedOnlyForInsertion(runs[8]);

            // "v".
            CheckNotMarked(runs[9]);
            Assert.That(runs.Count, Is.EqualTo(10));

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how Document.JoinRunsWithSameFormatting is tracked.
        /// </summary>
        [Test]
        public void TestJoinRunWithSameFormatting()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestJoinRunsWithSameFormatting.docx";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("1");
            builder.Write("2");
            builder.Write("3");
            builder.Write("4");

            doc.StartTrackRevisions("aw", gTestDateTime);

            doc.JoinRunsWithSameFormatting();

            RunCollection runs = doc.FirstSection.Body.FirstParagraph.Runs;

            CheckMarkedForDeletion(runs[0]);
            CheckMarkedForDeletion(runs[1]);
            CheckMarkedForDeletion(runs[2]);

            // JoinRunsWithSameFormatting method collects runs into last run.
            // When text of last run is changed new run before last is inserted.
            CheckMarkedForInsertion(runs[3]);

            CheckMarkedForDeletion(runs[4]);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how node insertion is tracked if TrackRevision turned on late.
        /// Row should be marked, paragraphs and run should be marked as well.
        /// </summary>
        [Test]
        public void TestInsertCompositeNodeTable()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestInsertCompositeNodeTable.docx";
            Document doc = new Document();

            Table table = new Table(doc);
            table.AppendChild(new Row(doc));

            Row firstRow = table.FirstRow;
            firstRow.AppendChild(new Cell(doc));
            firstRow.FirstCell.AppendChild(new Paragraph(doc));
            firstRow.FirstCell.FirstParagraph.AppendChild(new Run(doc, "Test run"));

            doc.StartTrackRevisions("aw", gTestDateTime);
            doc.FirstSection.Body.AppendChild(table);

            CheckMarkedForInsertion(firstRow);
            CheckMarkedForInsertion(firstRow.FirstCell.FirstParagraph);
            CheckMarkedForInsertion(firstRow.FirstCell.FirstParagraph.FirstRun);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how nested table is deleted when we delete outer table.
        /// Inner table should be marked.
        /// </summary>
        [Test]
        public void TestRemoveNestedTables()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveNestedTables.docx";
            Document doc = TestUtil.Open(testName);

            Table outerTable = (Table)doc.GetChild(NodeType.Table, 0, true);
            Table innerTable = (Table)doc.GetChild(NodeType.Table, 1, true);

            doc.StartTrackRevisions("Alexey Morozov", gTestDateTime);

            outerTable.Remove();

            foreach(Row row in innerTable.Rows)
                CheckMarkedForDeletion(row);

            TestUtil.Save(doc, testName);
        }

        /// <summary>
        /// Tests how inserted run is deleted when user changes.
        /// Run should not be deleted and should be marked instead.
        /// </summary>
        [Test]
        public void TestRemoveInsertedChildNewRsid()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveInsertedChild.docx";
            Document doc = TestUtil.Open(testName);

            doc.StartTrackRevisions("Will", gTestDateTime);

            Paragraph p = doc.FirstSection.Body.Paragraphs[1];
            p.AppendChild(new Run(doc, "+Run"));

            doc.StartTrackRevisions("Edgar", gTestDateTime);
            p.Remove();

            // Verify that there are still two runs.
            Assert.That(p.Runs.Count, Is.EqualTo(2));

            // Paragraph is marked for deletion.
            CheckMarkedForDeletion(p);

            // Both runs are also marked for deletion.
            CheckMarkedForDeletion(p.Runs[0]);
            CheckMarkedForDeletion(p.Runs[1]);

            // But last run is still marked for insertion.
            CheckMarkedForInsertion(p.Runs[1]);
        }

        /// <summary>
        /// Tests how inserted run is deleted when user is not changed.
        /// Run should be really deleted.
        /// </summary>
        [Test]
        public void TestRemoveInsertedChildSameRsid()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestRemoveInsertedChild.docx";
            Document doc = TestUtil.Open(testName);

            doc.StartTrackRevisions("Will", gTestDateTime);

            // Paragraph already has one run.
            Paragraph p = doc.FirstSection.Body.Paragraphs[1];

            // Add new run.
            p.AppendChild(new Run(doc, "+Run"));
            CheckNotMarked(p.Runs[0]);
            CheckMarkedForInsertion(p.Runs[1]);

            // Run is inserted and deleted during one session so it should be really deleted.
            p.Remove();

            // Verify that there is only one run remains.
            Assert.That(p.Runs.Count, Is.EqualTo(1));

            // Paragraph and first run are marked for deletion.
            CheckMarkedForDeletion(p);
            CheckMarkedForDeletion(p.Runs[0]);
        }


        /// <summary>
        /// Tests how inserted runs are deleted when user is changed.
        /// Run inserted by first author is really deleted other marked for deletion.
        /// </summary>
        [Test]
        public void TestChangeAuthor2()
        {
            const string testName = @"Model\Revision\TrackRevisions\TestChangeAuthor2.docx";
            Document doc = TestUtil.Open(testName);

            Paragraph para = doc.FirstSection.Body.Paragraphs[1];
            RunCollection runs = para.Runs;

            Assert.That(runs.Count, Is.EqualTo(3));
            Assert.That(para.GetText(), Is.EqualTo("WillTomErich\r"));

            CheckMarkedForInsertion(runs[0], "Will");
            CheckMarkedForInsertion(runs[1], "Tom");
            CheckMarkedForInsertion(runs[2], "Erich");

            doc.StartTrackRevisions("Will", gTestDateTime);
            para.Remove();

            // First run is really deleted while others two remains in document.
            Assert.That(runs.Count, Is.EqualTo(2));
            Assert.That(para.GetText(), Is.EqualTo("TomErich\r"));

            CheckMarkedForInsertion(para.Runs[0], "Tom");
            CheckMarkedForDeletion(para.Runs[0], "Will");

            CheckMarkedForInsertion(para.Runs[1], "Erich");
            CheckMarkedForDeletion(para.Runs[1], "Will");

            TestUtil.Save(doc, testName);
        }


        /// <summary>
        /// Tests how DocumentBuilder.InsertTableOfContents is tracked.
        /// Tracking should be locked for this operation.
        /// </summary>
        [Test]
        public void TestInsertToc()
        {
            Document doc = TestUtil.Open(@"DocBuilder\TestInsertTOC.docx");
            doc.StartTrackRevisions("aw", gTestDateTime);
            DocumentBuilder builder = new DocumentBuilder(doc);
            Field field = builder.InsertTableOfContents(@"\o ""1-3"" \h \z \u");

            foreach (Node inline in field.GetFieldRange())
                CheckMarkedForInsertion((Inline)inline);
        }

        /// <summary>
        /// Tests that cloned document continues tracking with original author and date time.
        /// </summary>
        [Test]
        public void TestClone()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            doc.StartTrackRevisions("amorozov", gTestDateTime);

            builder.Write("Run1");

            Document clone = doc.Clone();

            // Change author in the original document.
            doc.EditSession.Author = "ddarkin";

            builder = new DocumentBuilder(clone);
            builder.MoveToDocumentEnd();
            builder.Write("Run2");

            // Both runs are marked by original author.
            RunCollection runs = clone.FirstSection.Body.FirstParagraph.Runs;
            CheckMarkedForInsertion(runs[0], "amorozov");
            CheckMarkedForInsertion(runs[1], "amorozov");
        }

        /// <summary>
        /// WORDSNET-11695 Track Revisions set incorrect author name
        /// Improved Run.Text setter to get more clear behavior.
        /// </summary>
        [Test]
        public void TestJira11695()
        {
            Document doc = new Document();

            DocumentBuilder builder2 = new DocumentBuilder(doc);
            Paragraph para = builder2.InsertParagraph();

            Run run = new Run(doc);
            para.AppendChild(run);

            doc.StartTrackRevisions("Borris");      //first author inserted
            run.Text = "";
            run.Text = "BORRISSSSS";

            doc.StartTrackRevisions("Bonafort");    //second author deleted
            run.Remove();

            doc.StopTrackRevisions();

            para = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.Runs.Count, Is.EqualTo(1));

            run = para.FirstRun;
            Assert.That(run.Text, Is.EqualTo("BORRISSSSS"));
            CheckMarkedForInsertion(run, "Borris");
            CheckMarkedForDeletion(run, "Bonafort");
        }


        /// <summary>
        /// WORDSNET-20666 Implemented generation of move revisions on moving nodes when tracking revisions is ON.
        /// </summary>
        [Test]
        public void TestMoveRevisionGeneration()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            // Check that all kind of node moving works. Check moving to a position before and after a reference node;
            // check node moving in the forward and backward directions.

            // Generate paragraphs.
            for (int i = 1; i <= 4; i++)
            {
                body.AppendParagraph(string.Format("Paragraph before moved nodes {0}.", i));
                body.AppendParagraph(string.Format("Paragraph after moved nodes {0}.", i));
                body.AppendParagraph(string.Format("Move range start paragraph {0}.", i));
                body.AppendChild(new BookmarkStart(doc, string.Format("Bookmark{0}.", i)));
                body.AppendChild(new BookmarkEnd(doc, string.Format("Bookmark{0}.", i)));
                body.AppendParagraph(string.Format("Move range end paragraph {0}.", i));
            }

            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 11, 21, 16, 0));

            Paragraph[] paragraphs = body.Paragraphs.ToArray();

            // Insert before a reference node in the forward direction.
            body.InsertBefore(paragraphs[2], paragraphs[4], paragraphs[1]);

            // Insert after a reference node in the forward direction.
            body.InsertAfter(paragraphs[6], paragraphs[8], paragraphs[4]);

            // Insert before a reference node in the backward direction.
            Node referenceNode = paragraphs[9];
            Node movingNode = paragraphs[11];
            for (int i = 0; i < 4; i++)
            {
                Node prevNode = movingNode.PreviousSibling;
                referenceNode = body.InsertBefore(movingNode, referenceNode);
                movingNode = prevNode;
            }

            // Insert after a reference node in the backward direction.
            referenceNode = paragraphs[12];
            movingNode = paragraphs[15];
            for (int i = 0; i < 4; i++)
            {
                Node prevNode = movingNode.PreviousSibling;
                body.InsertAfter(movingNode, referenceNode);
                movingNode = prevNode;
            }

            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestMoveRevisionGeneration.docx", null, true,
                GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-20666 Tests generating move revisions for a table/row/cell.
        /// </summary>
        [Test]
        public void TestTableMoveRevisions()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Body body = doc.FirstSection.Body;

            Table table1 = builder.StartTable();
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    builder.InsertCell();
                    builder.Write(string.Format("Cell {0}:{1}", i, j));
                }
                builder.EndRow();
            }
            builder.EndTable();

            builder.Writeln("Paragraph 1");
            builder.Writeln("Paragraph 2");
            builder.Writeln("Paragraph 3");

            Table table2 = builder.StartTable();
            builder.InsertCell();
            builder.Write("Cell 1");
            builder.InsertCell();
            builder.Write("Cell 2");
            builder.EndTable();

            Node paragraph3 = table2.PreviousSibling;
            Node paragraph2 = paragraph3.PreviousSibling;

            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 26, 15, 56, 0));

            RowCollection rows = table1.Rows;
            CellCollection cells = rows[0].Cells;
            rows[0].InsertBefore(cells[2], null, cells[1]);

            table1.InsertAfter(rows[3], null, rows[1]);

            body.InsertBefore(paragraph3, table2.NextSibling, paragraph2);

            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestTableMoveRevisions.docx", null, true,
                GoldLevel.ExportOnly);

            Document clone = doc.Clone();
            doc.Revisions.AcceptAll();
            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestTableMoveRevisionsAccepted.docx", null, true,
                GoldLevel.ExportOnly);

            clone.Revisions.RejectAll();
            TestUtil.Save(clone, @"Model\Revision\TrackRevisions\TestTableMoveRevisionsRejected.docx", null, true,
                GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-20666 Tests generation of move revisions for SDT, field, Office math, shape, footnote and
        /// other nodes.
        /// </summary>
        [Test]
        public void TestOtherNodeMoveRevisions()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TrackRevisions\TestOtherNodeMoveRevisions.docx");
            Body body = doc.FirstSection.Body;
            ParagraphCollection paragraphs = body.Paragraphs;

            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 26, 15, 56, 0));

            body.InsertAfter(paragraphs[2], body.GetChildNodes(NodeType.Any, false)[body.GetChildNodes(NodeType.Any, false).Count - 1], paragraphs[0]);

            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestOtherNodeMoveRevisions.docx", null, true,
                GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-20666 Tests generating move revisions when a section is moved.
        /// </summary>
        [Test]
        public void TestSectionMoving()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            for (int i = 1; i <= 16; i++)
            {
                // Generate a section with two paragraphs.
                Body body = doc.LastSection.Body;
                builder.InsertBreak(BreakType.SectionBreakNewPage);
                body.Paragraphs[0].AppendChild(new Run(doc, "Section " + i));
                body.AppendChild(new Paragraph(doc));
            }

            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 27, 19, 40, 0));

            Section[] sections = doc.Sections.ToArray();

            // Insert before a reference node in the forward direction.
            doc.InsertBefore(sections[2], sections[4], sections[1]);

            // Insert after a reference node in the forward direction.
            doc.InsertAfter(sections[6], sections[8], sections[4]);

            // Insert before a reference node in the backward direction.
            Node referenceNode = sections[9];
            Node movingNode = sections[11];
            for (int i = 0; i < 2; i++)
            {
                Node prevNode = movingNode.PreviousSibling;
                referenceNode = doc.InsertBefore(movingNode, referenceNode);
                movingNode = prevNode;
            }

            // Insert after a reference node in the backward direction.
            referenceNode = sections[12];
            movingNode = sections[15];
            for (int i = 0; i < 2; i++)
            {
                Node prevNode = movingNode.PreviousSibling;
                doc.InsertAfter(movingNode, referenceNode);
                movingNode = prevNode;
            }

            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestSectionMoving.docx", null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-20666 Tests that Insert and Delete revisions are created when moving into a move range..
        /// </summary>
        [Test]
        public void TestInsertDeleteRevisionsOnMoving()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            body.AppendParagraph("Paragraph 1.");
            Paragraph para2 = body.AppendParagraph("Paragraph 2.");
            Paragraph para3 = body.AppendParagraph("Paragraph 3.");
            body.AppendParagraph("Paragraph 4.");
            Paragraph para5 = body.AppendParagraph("Paragraph 5.");

            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 11, 21, 16, 0));

            // Generate move ranges.
            body.AppendChild(para2);
            body.AppendChild(para3);

            // Insert para5 into the move-to range.
            body.InsertBefore(para5, para3);

            ITrackableNode inserted5 = para5;
            Assert.That(inserted5.InsertRevision, IsNot.Null());
            Assert.That(inserted5.DeleteRevision, Is.Null);
            Assert.That(inserted5.MoveToRevision, Is.Null);
            Assert.That(inserted5.MoveFromRevision, Is.Null);

            Paragraph deletedPara5 = body.Paragraphs[4];
            Assert.That(deletedPara5.FirstRun.Text, Is.EqualTo("Paragraph 5."));
            ITrackableNode deleted5 = deletedPara5;
            Assert.That(deleted5.InsertRevision, Is.Null);
            Assert.That(deleted5.DeleteRevision, IsNot.Null());
            Assert.That(deleted5.MoveToRevision, Is.Null);
            Assert.That(deleted5.MoveFromRevision, Is.Null);
        }

        /// <summary>
        /// WORDSNET-20666 Tests that when a node is moved from a move range, an insertion revision is assigned to it.
        /// </summary>
        [Test]
        public void TestMovingFromMoveRange()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            Paragraph para1 = body.AppendParagraph("Paragraph 1.");
            Paragraph para2 = body.AppendParagraph("Paragraph 2.");
            Paragraph para3 = body.AppendParagraph("Paragraph 3.");
            body.AppendParagraph("Paragraph 4.");
            body.AppendParagraph("Paragraph 5.");

            // Generate a move-to range.
            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 20, 17, 59, 0));
            body.AppendChild(para2);
            body.AppendChild(para3);

            // Move a node from the move range.
            doc.StartTrackRevisions("Author", new DateTime(2020, 11, 20, 18, 0, 0));
            body.InsertBefore(para2, para1);

            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestMovingFromMoveRange.docx", null, true,
                GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-20666 Tests generating a move range from one run to another in different paragraphs.
        /// </summary>
        [Test]
        public void TestRunLevelMoveRange()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Body body = doc.FirstSection.Body;

            builder.Write("Run 1. ");
            builder.Write("Run 2 (Move Range Start). ");
            Node rangeStart = builder.CurrentParagraph.LastChild;
            builder.Writeln("Run 3. ");
            builder.Writeln("Run 4. ");
            builder.Write("Run 5. ");
            builder.Write("Run 6 (Move Range End). ");
            Node rangeEnd = builder.CurrentParagraph.LastChild;
            builder.Writeln("Run 7. ");
            builder.Writeln("Run 8. ");
            builder.Write("Run 9. ");
            builder.Write("Run 10 (Move Destination - Before). ");
            Node moveDestination = builder.CurrentParagraph.LastChild;
            builder.Writeln("");

            CompositeNode rangeStartParent = rangeStart.ParentNode;
            CompositeNode rangeEndParent = rangeEnd.ParentNode;
            CompositeNode moveDestinationParent = moveDestination.ParentNode;

            // Temporary remove runs of the first moved paragraph to not assign a move revision to.
            List<Node> nodesBeforeMoveRange = new List<Node>();
            while (rangeStartParent.FirstChild != rangeStart)
            {
                nodesBeforeMoveRange.Add(rangeStartParent.FirstChild);
                rangeStartParent.FirstChild.Remove();
            }

            const string author = "Author";
            DateTime revisionDate = new DateTime(2020, 12, 2, 17, 24, 0);
            doc.StartTrackRevisions(author, revisionDate);

            // Move paragraphs, which paragraph-break char should be included into a move range.
            // Paragraph of rangeEnd is not included to a movement.
            body.InsertBefore(rangeStartParent, rangeEndParent, moveDestinationParent);

            doc.StopTrackRevisions();

            // Find move-from range start node and move it inside the subsequent paragraph. Insert the removed nodes.
            Node moveFromRangeStart = body.GetChild(NodeType.MoveFromRangeStart, 0, true);
            Debug.Assert(moveFromRangeStart.NodeLevel == NodeLevel.Block); // We place it on the block level now.
            CompositeNode moveFromRangeStartParent = (CompositeNode)moveFromRangeStart.NextSibling;
            moveFromRangeStartParent.InsertBefore(moveFromRangeStart, moveFromRangeStartParent.FirstChild);
            foreach (Node node in nodesBeforeMoveRange)
                moveFromRangeStartParent.InsertBefore(node, moveFromRangeStart);

            // Find move-to range start node and move it inside the subsequent paragraph. Move runs before the destination
            // run into the paragraph, they should be before the move-to range.
            Node moveToRangeStart = body.GetChild(NodeType.MoveToRangeStart, 0, true);
            Debug.Assert(moveToRangeStart.NodeLevel == NodeLevel.Block); // We place it on the block level now.
            rangeStartParent.InsertBefore(moveToRangeStart, rangeStartParent.FirstChild);
            if (moveDestinationParent.FirstChild != moveDestination)
                rangeStartParent.InsertBefore(moveDestinationParent.FirstChild, moveDestination, moveToRangeStart);

            // Move rangeEnd and all nodes before it. They will be automatically added to the existing move range.
            doc.StartTrackRevisions(author, revisionDate);
            moveDestinationParent.InsertBefore(rangeEndParent.FirstChild, rangeEnd.NextSibling, moveDestination);

            TestUtil.Save(doc, @"Model\Revision\TrackRevisions\TestRunLevelMoveRange.docx", null, true,
                GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-20666 Tests that a move range is not created on moving non-trackable nodes.
        /// </summary>
        [Test]
        public void TestMovingBookmark()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            body.AppendParagraph("Paragraph 1.");
            body.AppendParagraph("Paragraph 2.");
            BookmarkStart start = new BookmarkStart(doc, "Bookmark");
            body.AppendChild(start);
            BookmarkEnd end = new BookmarkEnd(doc, "Bookmark");
            body.AppendChild(end);

            doc.StartTrackRevisions("Author", new DateTime(2020, 12, 10, 22, 57, 0));

            body.InsertBefore(start, end.NextSibling, body.Paragraphs[1]);

            Assert.That(doc.GetChild(NodeType.MoveFromRangeStart, 0, true), Is.Null);
            Assert.That(doc.GetChild(NodeType.MoveFromRangeEnd, 0, true), Is.Null);
            Assert.That(doc.GetChild(NodeType.MoveToRangeStart, 0, true), Is.Null);
            Assert.That(doc.GetChild(NodeType.MoveToRangeEnd, 0, true), Is.Null);
        }

        /// <summary>
        /// WORDSNET-21481 Tests that move range date is written as local without conversion to UTC similar like
        /// MS Word does.
        /// </summary>
        [Test]
        public void TestMoveRangeDate()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            body.AppendParagraph("Paragraph 1.");
            Paragraph para2 = body.AppendParagraph("Paragraph 2.");
            Paragraph para3 = body.AppendParagraph("Paragraph 3.");
            body.AppendParagraph("Paragraph 4.");
            Paragraph para5 = body.AppendParagraph("Paragraph 5.");

            DateTime date = new DateTime(2020, 12, 3, 15, 47, 0, DateTimeKind.Local);

            // Generate move range.
            doc.StartTrackRevisions("Author", date);
            body.AppendChild(para2);
            body.AppendChild(para3);

            doc = TestUtil.SaveOpen(doc, @"Model\Revision\TrackRevisions\TestMoveRangeDate",
                UnifiedScenario.Docx2DocxNoGold);

            ITrackableNode para = doc.FirstSection.Body.Paragraphs[doc.FirstSection.Body.Paragraphs.Count - 1];
            Assert.That(para.MoveToRevision.DateTime, Is.EqualTo(date));

            MoveToRangeStart moveRangeStart = (MoveToRangeStart)doc.GetChild(NodeType.MoveToRangeStart, 0, true);
            Assert.That(moveRangeStart.Date, Is.EqualTo(date));
        }

        /// <summary>
        /// WORDSNET-22321 AW generated too complicated revisions in the revision tracking mode when replacing text
        /// containing a paragraph break.
        /// </summary>
        [Test]
        public void Test22321()
        {
            const string fileName = @"Model\Revision\TrackRevisions\Test22321.docx";
            Document doc = TestUtil.Open(fileName);

            doc.StartTrackRevisions("user", new DateTime(2021, 06, 16));
            doc.Range.Replace("one.&pThis", "replaced");
            doc.StopTrackRevisions();

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            Assert.That(paragraphs.Count, Is.EqualTo(2));

            Paragraph paragraph1 = paragraphs[0];
            Assert.That(paragraph1.Runs.Count, Is.EqualTo(2));

            ITrackableNode paragraph1Trackable = paragraph1;
            Assert.That(paragraph1Trackable.DeleteRevision, IsNot.Null());
            Assert.That(paragraph1Trackable.InsertRevision, Is.Null);
            Assert.That(paragraph1Trackable.MoveFromRevision, Is.Null);
            Assert.That(paragraph1Trackable.MoveToRevision, Is.Null);

            Assert.That(paragraphs[0].FirstRun.RunPr.HasRevisions, Is.False);

            Paragraph paragraph2 = paragraphs[1];
            Assert.That(paragraph2.Runs.Count, Is.EqualTo(3));
            Assert.That(paragraph2.ParaPr.HasRevisions, Is.False);

            TestUtil.Save(doc, fileName, null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-22311 AW generated too complicated revisions in the revision tracking mode when inserting
        /// multiple paragraph breaks using <see cref="Range.Replace(string,string)"/>.
        /// </summary>
        /// <remarks>The output after WORDSNET-26924 is exactly the same as in Word.</remarks>
        [Test]
        public void Test22311()
        {
            const string fileName = @"Model\Revision\TrackRevisions\Test22311.docx";
            Document doc = TestUtil.Open(fileName);

            doc.StartTrackRevisions("Author", new DateTime(2021, 06, 16));
            doc.Range.Replace(",", "&p&p");
            doc.StopTrackRevisions();

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Paragraph paragraph1 = paragraphs[0];
            Assert.That(paragraph1.Runs.Count, Is.EqualTo(2));

            ITrackableNode paragraph1Trackable = paragraph1;
            Assert.That(paragraph1Trackable.DeleteRevision, Is.Null);
            Assert.That(paragraph1Trackable.InsertRevision, IsNot.Null());
            Assert.That(paragraph1Trackable.MoveFromRevision, Is.Null);
            Assert.That(paragraph1Trackable.MoveToRevision, Is.Null);

            Assert.That(paragraph1.FirstRun.RunPr.HasRevisions, Is.False);

            Paragraph paragraph2 = paragraphs[1];
            Assert.That(paragraph2.Runs.Count, Is.EqualTo(0));

            ITrackableNode paragraph2Trackable = paragraph2;
            Assert.That(paragraph2Trackable.DeleteRevision, Is.Null);
            Assert.That(paragraph2Trackable.InsertRevision, IsNot.Null());
            Assert.That(paragraph2Trackable.MoveFromRevision, Is.Null);
            Assert.That(paragraph2Trackable.MoveToRevision, Is.Null);

            Paragraph paragraph3 = paragraphs[2];
            Assert.That(paragraph3.Runs.Count, Is.EqualTo(0));
            Assert.That(paragraph3.ParaPr.HasRevisions, Is.False);

            Paragraph paragraph4 = paragraphs[3];
            Assert.That(paragraph3.Runs.Count, Is.EqualTo(0));
            Assert.That(paragraph4.ParaPr.HasRevisions, Is.False);

            Paragraph paragraph5 = paragraphs[4];
            Assert.That(paragraph1.Runs.Count, Is.EqualTo(2));

            ITrackableNode paragraph5Trackable = paragraph5;
            Assert.That(paragraph5Trackable.DeleteRevision, Is.Null);
            Assert.That(paragraph5Trackable.InsertRevision, IsNot.Null());
            Assert.That(paragraph5Trackable.MoveFromRevision, Is.Null);
            Assert.That(paragraph5Trackable.MoveToRevision, Is.Null);

            Assert.That(paragraph5.FirstRun.RunPr.HasRevisions, Is.False);

            TestUtil.Save(doc, fileName, null, true, GoldLevel.ExportOnly);
        }

        /// <summary>
        /// WORDSNET-28443 Track changes are lost in dropdown content controls mapped to Custom XML when saving DOCX
        /// Check DropDownList content by item value.
        /// </summary>
        [Test]
        public void Test28443()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Markup\Test28443.docx");
            RevisionGroupCollection groups = doc.Revisions.Groups;

            Assert.That(groups.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Checks that given ITrackableNode is marked only for insertion.
        /// </summary>
        private static void CheckMarkedOnlyForInsertion(ITrackableNode node)
        {
            Assert.That(node.DeleteRevision, Is.Null);
            Assert.That(node.InsertRevision, IsNot.Null());
        }

        /// <summary>
        /// Checks that given ITrackableNode is marked only for deletion.
        /// </summary>
        private static void CheckMarkedOnlyForDeletion(ITrackableNode node)
        {
            Assert.That(node.DeleteRevision, IsNot.Null());
            Assert.That(node.InsertRevision, Is.Null);
        }

        /// <summary>
        /// Checks that given ITrackableNode is marked for deletion.
        /// </summary>
        private static void CheckMarkedForDeletion(ITrackableNode node)
        {
            Assert.That(node.DeleteRevision, IsNot.Null());
        }

        /// <summary>
        /// Checks that given ITrackableNode is marked for deletion by certain author.
        /// </summary>
        private static void CheckMarkedForDeletion(ITrackableNode node, string author)
        {
            Assert.That(node.DeleteRevision.Author, Is.EqualTo(author));
        }

        /// <summary>
        /// Checks that given ITrackableNode is marked for insertion by certain author.
        /// </summary>
        private static void CheckMarkedForInsertion(ITrackableNode node, string author)
        {
            Assert.That(node.InsertRevision.Author, Is.EqualTo(author));
        }

        /// <summary>
        /// Checks that given ITrackableNode is marked for insertion.
        /// </summary>
        private static void CheckMarkedForInsertion(ITrackableNode node)
        {
            Assert.That(node.InsertRevision, IsNot.Null());
        }

        /// <summary>
        /// Checks that given ITrackableNode is NOT marked for insertion.
        /// </summary>
        private static void CheckNotMarkedForInsertion(ITrackableNode node)
        {
            Assert.That(node.InsertRevision, Is.Null);
        }

        /// <summary>
        /// Checks that given ITrackableNode is NOT marked for deletion.
        /// </summary>
        private static void CheckNotMarkedForDeletion(ITrackableNode node)
        {
            Assert.That(node.DeleteRevision, Is.Null);
        }

        /// <summary>
        /// Checks that given ITrackableNode is NOT marked either for insertion or deletion.
        /// </summary>
        private static void CheckNotMarked(ITrackableNode node)
        {
            CheckNotMarkedForInsertion(node);
            CheckNotMarkedForDeletion(node);
        }

        /// <summary>
        /// Checks that paragraphs in given ParagraphCollection and their runs are NOT marked.
        /// </summary>
        private static void CheckNotMarked(ParagraphCollection paragraphCollection)
        {
            foreach (Paragraph p in paragraphCollection)
            {
                CheckNotMarked(p);
                foreach (Run run in p.Runs)
                    CheckNotMarked(run);
            }
        }

        private static readonly DateTime gTestDateTime = DateTime.ParseExact("2014-04-15", "yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
