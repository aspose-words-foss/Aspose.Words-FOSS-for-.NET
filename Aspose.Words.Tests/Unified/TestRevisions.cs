// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Revisions;
using Aspose.Words.RW;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Tests for revisions, includes both unified and regular tests.
    /// </summary>
    [TestFixture]
    public class TestRevisions : UnifiedTestsBase
    {
        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }


        /// <summary>
        /// Test that applying style in revision is correctly processed in the model.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionAll(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionAll", lf, sf);

            // ListId = 1 is correct here, because this is what the paragraph had before a revision.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.ParagraphFormat.ListId, Is.EqualTo(1));

            doc.AcceptAllRevisions();

            // ListBullet style was applied to paragraph in a revision, so after accept the ListId should be 2.
            Assert.That(para.ParagraphFormat.ListId, Is.EqualTo(2));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionRun(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionRun", lf, sf);

            NodeList runs = doc.SelectNodes("//Run");

            //Test inserted text
            Run run = (Run)runs[1];
            Assert.That(run.Text, Is.EqualTo("inserted "));

            EditRevision editRevision = run.RunPr.InsertRevision;
            Assert.That(editRevision, IsNot.Null());
            Assert.That(editRevision.Author, Is.EqualTo("RK"));
            Assert.That(editRevision.DateTime, Is.EqualTo(new DateTime(2005, 3, 8, 15, 29, 0)));


            //Test deleted text
            run = (Run)runs[3];
            Assert.That(run.Text, Is.EqualTo("fore"));

            editRevision = run.RunPr.DeleteRevision;
            Assert.That(editRevision, IsNot.Null());
            Assert.That(editRevision.Author, Is.EqualTo("RK"));
            Assert.That(editRevision.DateTime, Is.EqualTo(new DateTime(2005, 3, 8, 15, 37, 0)));


            // Test formatted text
            run = (Run)runs[5];
            Assert.That(run.Text, Is.EqualTo("revisions"));
            FormatRevision formatRevision = run.RunPr.FormatRevision;
            Assert.That(run.RunPr.InsertRevision, Is.Null);
            Assert.That(run.RunPr.DeleteRevision, Is.Null);

            Assert.That(formatRevision, IsNot.Null());
            Assert.That(formatRevision.RevPr.ContainsKey(FontAttr.Bold), Is.EqualTo(true));

            //Test inserted and formatted
            run = (Run)runs[6];
            Assert.That(run.Text, Is.EqualTo("xxx"));
            Assert.That(run.RunPr.InsertRevision, IsNot.Null());
            Assert.That(run.RunPr.DeleteRevision, Is.Null);

            Assert.That(formatRevision, IsNot.Null());

            //Test how simple changes within one paragraph were accepted.
            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionRun Accepted", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("Text inserted be revisionsxxx.\x000c"));
        }

        /// <summary>
        /// Test two paragraphs got joined.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionJoinPara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionJoinPara", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionJoinPara Accepted", lf, sf);

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(2));
            Assert.That(paras[0].GetText(), Is.EqualTo("Para 2.\r"));
            Assert.That(paras[1].GetText(), Is.EqualTo("Para 3.\x000c"));
        }

        /// <summary>
        /// Test one paragraph got completely deleted and two others got joined.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionDeletePara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionDeletePara", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionDeletePara Accepted", lf, sf);

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(1));
            Assert.That(paras[0].GetText(), Is.EqualTo("Para 3.\x000c"));
        }

        /// <summary>
        /// Interesting enough, MS Word does not allow to delete a paragraph mark
        /// that is just before a table when track changes is turned on.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionDeleteParaBeforeTable(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionDeleteParaBeforeTable", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionDeleteParaBeforeTable Accepted", lf, sf);

            //One empty before table, one in the table and one after.
            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(3));
            Assert.That(doc.GetText(), Is.EqualTo("\r\x0007\x0007\x000c"));
        }

        /// <summary>
        /// Test inserting a paragraph accepted okay.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionInsertPara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionInsertPara", lf, sf);
            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionInsertPara Accepted", lf, sf);

            NodeList paras = doc.SelectNodes("//Paragraph");
            Assert.That(paras.Count, Is.EqualTo(2));
            Assert.That(paras[0].GetText(), Is.EqualTo("Para 2.\r"));
            Assert.That(paras[1].GetText(), Is.EqualTo("Para 1.\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionJoinSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionJoinSection", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionJoinSection Accepted", lf, sf);

            Assert.That(doc.Sections.Count, Is.EqualTo(2));
            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("Section 2.\r\x000c"));
            Assert.That(doc.Sections[1].GetText(), Is.EqualTo("Section 3.\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionDeleteHeader(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionDeleteHeader", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionDeleteHeader Accepted", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("xxx\rSection 1.\x000cHeader 2.\rSection 2.\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionDeleteRow(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionDeleteRow", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionDeleteRow Accepted", lf, sf);

            Assert.That(doc.SelectNodes("//Row").Count, Is.EqualTo(1));
            Assert.That(doc.GetText(), Is.EqualTo("Row 1\x0007\x0007\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionDeleteTable(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionDeleteTable", lf, sf);

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionDeleteTable Accepted", lf, sf);

            Assert.That(doc.SelectNodes("//Table").Count, Is.EqualTo(0));
            Assert.That(doc.GetText(), Is.EqualTo("\x000c"));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionPara(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionPara", lf, sf);

            //Check before the revision
            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph");
            Assert.That(para.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Left));

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionPara Accepted", lf, sf);

            //Test after accepting the revision
            Assert.That(para.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionSection", lf, sf);

            //Check paper size before the revision.
            Section section = doc.Sections[0];
            Assert.That(section.PageSetup.PaperSize, Is.EqualTo(PaperSize.A4));

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionSection Accepted", lf, sf);

            //Check the paper size was changed by accepting the revision.
            Assert.That(section.PageSetup.PaperSize, Is.EqualTo(PaperSize.A5));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionNumbered(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionNumbered", lf, sf);

            // Inserted numbering.
            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");
            ParagraphNumberRevision rev = para.ParaPr.NumberRevision;
            Assert.That(rev, IsNot.Null());
            Assert.That(para.ParaPr.ListId, Is.EqualTo(5));
            Assert.That(rev.IsInsertion, Is.EqualTo(true));
            Assert.That(rev.WasNumbered, Is.EqualTo(false));
            Assert.That(rev.NumberFormat, Is.EqualTo(""));

            // Deleted numbering.
            // RK MS Word does not seem to store deleted numbering revision in RTF.
            if ((lf != LoadFormat.Rtf) && (sf != SaveFormat.Rtf))
            {
                para = (Paragraph)doc.SelectSingleNode("//Paragraph[3]");
                rev = para.ParaPr.NumberRevision;
                Assert.That(rev, IsNot.Null());
                Assert.That(para.ParaPr.ListId, Is.EqualTo(0));
                Assert.That(rev.IsInsertion, Is.EqualTo(false));
                Assert.That(rev.WasNumbered, Is.EqualTo(true));
                Assert.That(rev.NumberFormat, Is.EqualTo("\x0000."));
            }

            Assert.That(doc.HasRevisions, Is.EqualTo(true));
            doc.AcceptAllRevisions();
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionNumbered Accepted", lf, sf);

            //After accepting the revision, the numbering revision is simply deleted.
            para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");
            Assert.That(para.ParaPr.ListId, Is.EqualTo(5));
            Assert.That(para.ParaPr.ContainsKey(RevisionAttr.NumberRevision), Is.EqualTo(false));

            para = (Paragraph)doc.SelectSingleNode("//Paragraph[3]");
            Assert.That(para.ParaPr.ListId, Is.EqualTo(0));
            Assert.That(para.ParaPr.ContainsKey(RevisionAttr.NumberRevision), Is.EqualTo(false));
        }



        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionNumberAdded(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionNumberAdded", lf, sf);

            // When list formatting was added to a paragraph:
            CheckNumberRevision(doc, 0,
                true,    // list id is set
                true,    // is inserted true
                false,    // was numbered false
                "");    // format etc fields in number revision are empty
            Assert.That(doc.HasRevisions, Is.EqualTo(true));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionNumberAccepted(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionNumberAccepted", lf, sf);

            CheckNoNumberRevision(doc, 0);
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionNumberRemoved(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionNumberRemoved", lf, sf);

            // When list formatting was deleted from a paragraph:
            CheckNumberRevision(doc, 0,
                false,    // list id is no longer set
                false,    // is inserted false
                true,    // was numbered true
                "\x0000.");    // format etc fields in number revisions are set
            Assert.That(doc.HasRevisions, Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-2078 Accepted revisions still affect HasRevisions.
        /// Same as TestRevisionNumberAccepted, but list is set in the style, not in the paragraph.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect2078(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestDefect2078", lf, sf);

            CheckNoNumberRevision(doc, 0);
            Assert.That(doc.HasRevisions, Is.EqualTo(false));
        }

        private static void CheckNumberRevision(
            Document doc,
            int paraIndex,
            bool isListItem,
            bool isInserted,
            bool wasNumbered,
            string numberFormat)
        {
            Paragraph para = (Paragraph)doc.FirstSection.Body.GetChild(NodeType.Paragraph, paraIndex, true);
            Assert.That(para.IsListItem, Is.EqualTo(isListItem));
            Assert.That(para.ParaPr.NumberRevision.IsInsertion, Is.EqualTo(isInserted));
            Assert.That(para.ParaPr.NumberRevision.WasNumbered, Is.EqualTo(wasNumbered));
            Assert.That(para.ParaPr.NumberRevision.NumberFormat, Is.EqualTo(numberFormat));
        }

        private static void CheckNoNumberRevision(Document doc, int paraIndex)
        {
            Paragraph para = (Paragraph)doc.FirstSection.Body.GetChild(NodeType.Paragraph, paraIndex, true);
            Assert.That(para.ParaPr.NumberRevision, Is.Null);
        }

        /// <summary>
        /// WORDSNET-5515 Enable "Track changes" automatically when protect document with AllowOnlyRevisions protection type.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedAutoUpdateTrackRevisions(LoadFormat lf, SaveFormat sf)
        {
            // Test the legacy protect method.
            Document doc = new Document();
            Assert.That(doc.TrackRevisions, Is.False);
            // Setting this protection turns on revision tracking.
            doc.Protect(ProtectionType.AllowOnlyRevisions);
            Assert.That(doc.TrackRevisions, Is.True);

            // Test the new protect method.
            doc = new Document();
            Assert.That(doc.TrackRevisions, Is.False);
            // Setting this protection turns on revision tracking.
            doc.Protect(ProtectionType.AllowOnlyRevisions, "password");
            Assert.That(doc.TrackRevisions, Is.True);

            // When you open this document in MS Word, you should be able to edit it and the track changes will be on.
            TestUtil.SaveOpen(doc, @"Model\Revision\TestAutoUpdateTrackRevisions", lf, sf);

            // WORDSNET-7715 Setting another protection type does not turn it off if tracking is on already.
            Assert.That(doc.TrackRevisions, Is.True);
            doc.Protect(ProtectionType.ReadOnly);
            Assert.That(doc.TrackRevisions, Is.True);
        }

        /// <summary>
        /// WORDSNET-5483 Track change disappears after open/save document using Aspose.Words.
        /// RK Our DOC importer used to skip zero highlight color for unknown reason. Fixed.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionHighlight(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRevisionHighlight", lf, sf);

            Run run = (Run)doc.GetChild(NodeType.Run, -1, true);
            Assert.That(run.GetText(), Is.EqualTo("Test."));
            Assert.That(run.Font.HighlightColor.ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));

            doc.AcceptAllRevisions();

            Assert.That(run.Font.HighlightColor.ToArgb(), Is.EqualTo(0));

            TestUtil.SaveOpen(doc, @"Model\Revision\TestRevisionHighlight Accepted", lf, sf);
        }

        /// <summary>
        /// WORDSNET-9012 When imported from RTF, TA fields are not deleted when revisions are accepted.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeletedTAFields(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestDeletedTAFields", lf);

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            const int NotDeletedFieldCount = 3;
            Assert.That(fields.Count, Is.EqualTo(NotDeletedFieldCount));

            NodeCollection fieldStarts = doc.GetChildNodes(NodeType.FieldStart, true);
            NodeCollection fieldEnds = doc.GetChildNodes(NodeType.FieldEnd, true);
            const int AllFieldsCount = 6;
            Assert.That(fieldStarts.Count, Is.EqualTo(AllFieldsCount));
            Assert.That(fieldEnds.Count, Is.EqualTo(AllFieldsCount));

            FieldStart fieldStart = (FieldStart)fieldStarts[2];
            Assert.That(fieldStart.IsDeleteRevision, Is.True);
            Run run = (Run)fieldStart.NextSibling;
            Assert.That(run.Text, Is.EqualTo(" TA \\s \"voluptua\" "));
            Assert.That(run.IsDeleteRevision, Is.True);

            FieldEnd fieldEnd = (FieldEnd)fieldEnds[2];
            Assert.That(fieldEnd.IsDeleteRevision, Is.True);

            doc.AcceptAllRevisions();

            fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields.Count, Is.EqualTo(NotDeletedFieldCount));
        }


        /// <summary>
        /// WORDSNET-9525 SmartTags are not removed when accept all revisions.
        /// Added deletion of smart tags when they become empty after deleting revisions.
        /// </summary>
        [Test]
        public void TestDeleteRevisionWithSmartTags()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestDeleteRevisionWithSmartTags.docx");
            doc.AcceptAllRevisions();
            Assert.That(doc.GetChildNodes(NodeType.SmartTag, true).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-19791 Accepting revisions in the cloned document affects the original document.
        /// Fixed. The problem was that paragraph break run properties were not cloned inside Paragraph.Clone.
        /// </summary>
        [Test]
        public void TestRevisionClone()
        {
            Document docA = TestUtil.Open(@"Model\Revision\TestRevisionClone.docx");
            Document docB = docA.Clone();

            // Check run insert revision is cloned.
            Paragraph p0A = (Paragraph)docA.GetChild(NodeType.Paragraph, 0, true);
            Paragraph p0B = (Paragraph)docB.GetChild(NodeType.Paragraph, 0, true);
            CheckRunRevisionCloned(p0A.ParagraphBreakRunPr, p0B.ParagraphBreakRunPr, RevisionAttr.InsertRevision);
            CheckRunRevisionCloned(p0A.Runs[1].RunPr, p0B.Runs[1].RunPr, RevisionAttr.InsertRevision);

            // Check run delete revision is cloned.
            Paragraph p1A = (Paragraph)docA.GetChild(NodeType.Paragraph, 1, true);
            Paragraph p1B = (Paragraph)docB.GetChild(NodeType.Paragraph, 1, true);
            CheckRunRevisionCloned(p1A.ParagraphBreakRunPr, p1B.ParagraphBreakRunPr, RevisionAttr.DeleteRevision);
            CheckRunRevisionCloned(p1A.Runs[1].RunPr, p1B.Runs[1].RunPr, RevisionAttr.DeleteRevision);

            // Check run format revision is cloned.
            Paragraph p2A = (Paragraph)docA.GetChild(NodeType.Paragraph, 2, true);
            Paragraph p2B = (Paragraph)docB.GetChild(NodeType.Paragraph, 2, true);
            CheckRunRevisionCloned(p2A.ParagraphBreakRunPr, p2B.ParagraphBreakRunPr, RevisionAttr.FormatRevision);
            CheckRunRevisionCloned(p2A.Runs[1].RunPr, p2B.Runs[1].RunPr, RevisionAttr.FormatRevision);

            // Check para format revision is cloned.
            Paragraph p3A = (Paragraph)docA.GetChild(NodeType.Paragraph, 3, true);
            Paragraph p3B = (Paragraph)docB.GetChild(NodeType.Paragraph, 3, true);
            CheckRunRevisionCloned(p3A.ParaPr, p3B.ParaPr, RevisionAttr.FormatRevision);
        }

        private static void CheckRunRevisionCloned(AttrCollection a, AttrCollection b, int key)
        {
            object revA = a.GetDirectAttr(key);
            object revB = b.GetDirectAttr(key);
            Assert.That(revA, IsNot.Null());
            Assert.That(revB, IsNot.Null());
            Assert.That(revB, IsNot.SameAs(revA));
        }

        /// <summary>
        /// WORDSNET-1095 Nodes with revisions might not copy well into a different document.
        /// Fixed. Revision attributes collection is stored inside the main attributes collection and the revision
        /// attributes collection can contain a reference to a list or to a style and that needs importing too, done that.
        /// There are some differences in formatting possible, but they seem to be not related to revisions.
        /// </summary>
        [Test]
        public void TestRevisionImport()
        {
            Document srcDoc = TestUtil.Open(@"Model\Revision\TestRevisionClone.docx");
            Document dstDoc = new Document();

            dstDoc.RemoveAllChildren();
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            TestUtil.SaveOpen(dstDoc, @"Model\Revision\TestRevisionImport.docx");
        }


        /// <summary>
        /// Relates to WORDSNET-8423
        /// Tests that PositiveDifference is calculated properly for runs.
        /// </summary>
        [Test]
        public void TestJira8423_Runs()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestJira8423_Runs.docx");

            // 1
            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);
            RunPr revRunPr = (RunPr)run.RunPr.FormatRevision.RevPr;

            Assert.That(run.RunPr[FontAttr.Italic], Is.Null);
            Assert.That(revRunPr.Count, Is.EqualTo(1));
            Assert.That(revRunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));

            // 2
            run = (Run)doc.GetChild(NodeType.Run, 1, true);
            revRunPr = (RunPr)run.RunPr.FormatRevision.RevPr;

            Assert.That(run.RunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(revRunPr.Count, Is.EqualTo(2));
            Assert.That(revRunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.False));
        }

        /// <summary>
        /// Relates to WORDSNET-8423
        /// Deep test PositiveDifference calculation if both style and paragraph was revised.
        /// </summary>
        [Test]
        public void TestJira8423_BothStyleAndParagraph()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestJira8423_BothStyleAndParagraph.docx");

            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            ParaPr revParaPr = (ParaPr)para.ParaPr.FormatRevision.RevPr;

            // Revised attributes is inherited from revised style.
            // AM. Actually we don't need any attributes here because they all inherited but so far it's OK.
            Assert.That(revParaPr[ParaAttr.Istd], Is.EqualTo(0x01));
            Assert.That(revParaPr[ParaAttr.Alignment], Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(revParaPr.Count, Is.EqualTo(2));

            Style heading1 = doc.Styles.GetByIstd(0x01, false);

            revParaPr = (ParaPr)heading1.ParaPr.FormatRevision.RevPr;
            Assert.That(revParaPr[ParaAttr.Alignment], Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(revParaPr[ParaAttr.SpaceAfter], Is.EqualTo(0));
            Assert.That(revParaPr.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Relates to WORDSNET-8423
        /// Tests style PositiveDifference calculation for attribute defined in base style.
        /// </summary>
        [Test]
        public void TestJira8423_TestStyleRevisions()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestStyleRevisions.docx");

            // Style has Italic=true and at seems that we need to compensate it by Italic=false in positive-difference but it's wrong.
            // Italic=true is also defined in base "SubtleEmphasis" style and revised properties should inherit from this base.
            // That's why we need to collapse over fully resolved attributes.
            Style emphasis = doc.Styles.GetByIstd(0x1c, false);
            RunPr revRunPr = (RunPr)emphasis.RunPr.FormatRevision.RevPr;
            Assert.That(emphasis.RunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));
            Assert.That(revRunPr[FontAttr.Italic], Is.Null);

            // This file is also perfect example why we need to join ThemeColor and ThemeTint into one attribute.
            // We inherit ThemeTint here as standalone attribute so result color is different: we have "text1" + "7F" tint in original properties and
            // "accent1" in positive-difference and get "accent1" + "7F" after revision accepted but need only "accent1".
            // Of course we can made some hack but we need it in AcceptRevision, in Style.ExpandRunPr, in Paragraph.GetExpandedRunPr and so on.
            Assert.That(emphasis.RunPr[FontAttr.ThemeColor], Is.EqualTo("text1"));
            Assert.That(emphasis.RunPr[FontAttr.ThemeTint], Is.EqualTo("7F"));

            Assert.That(revRunPr[FontAttr.ThemeColor], Is.EqualTo("accent1"));
            Assert.That((string)revRunPr[FontAttr.ThemeTint], Is.Empty);

            emphasis.RunPr.AcceptFormatRevision();
            Assert.That(emphasis.RunPr[FontAttr.ThemeColor], Is.EqualTo("accent1"));

            // Default value is expected.
            Assert.That((string)emphasis.RunPr[FontAttr.ThemeTint], Is.Empty);
        }


        /// <summary>
        /// Relates to WORDSNET-8423
        /// Shows problem with style format revision and document defaults.
        /// </summary>
        [Test]
        public void TestJira8423_StyleAndDefaults()
        {
            // Very weird Word behavior. Seems to be some of compatibility hack.
            // If style has at least one property defined for either paragraph or run in original attributes,
            // paragraphs of this style should not use Document Defaults.
            for (int i = 1; i <= 3; i++)
            {
                Document doc = TestUtil.Open(string.Format(@"Model\Revision\TestJira8423_StyleAndDefaults{0}.xml", i));

                Paragraph para = doc.FirstSection.Body.FirstParagraph;
                Run run = para.FirstRun;
                ParaPr paraPr = para.GetExpandedParaPr(ParaPrExpandFlags.Layout);
                RunPr runPr = run.GetExpandedRunPr(RunPrExpandFlags.Layout);

                // Expanded properties is absolutely the same for every document. This is wrong.
                // Word renders original markup differently:
                //  1. Style has no attributes in both original and final. Everything fine.
                //  2. Style has only Size in original. Word shows SpaceAfter = 0, 10pt Times New Roman.
                //  3. Style has only SpaceAfter in original. Word shows 10pt Times New Roman.
                //
                // Word does this hack only for styles. Paragraph or Run revision is OK.
                Assert.That(paraPr[ParaAttr.SpaceAfter], Is.EqualTo(1600));
                Assert.That(((ComplexFontName)runPr[FontAttr.NameAscii]).ThemeFontCore, Is.EqualTo(ThemeFontCore.MinorHAnsi));
            }

            // Additional test for this issue.
            Document awRejectedDoc = TestUtil.Open(@"Model\Revision\TestJira8423_StyleAndDefaults2.xml");
            // Verify that no spacing or font are specified in originals before rejection.
            Style style = awRejectedDoc.Styles.GetByName("Normal Test", false);
            Assert.That(style.ParaPr[ParaAttr.SpaceAfter], Is.Null);
            Assert.That(style.RunPr[FontAttr.NameAscii], Is.Null);

            awRejectedDoc.Revisions.RejectAll();

            // Spacing and font are still not redefined and thus are inherited from DocumentDefaults.
            Style awStyle = awRejectedDoc.Styles.GetByName("Normal Test", false);
            Assert.That(awStyle.ParaPr[ParaAttr.SpaceAfter], Is.Null);
            Assert.That(awStyle.RunPr[FontAttr.NameAscii], Is.Null);

            // This file is saved in Word after all revisions are rejected in UI.
            Document msRejectedDoc = TestUtil.Open(@"Model\Revision\TestJira8423_StyleAndDefaults2 ms Rejected.xml");

            // Word makes "hack" and writes SpaceAfter = 0, Single LineSpacing and Times New Roman directly to style.
            Style msStyle = msRejectedDoc.Styles.GetByName("Normal Test", false);
            Assert.That(msStyle.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(0));
            Assert.That(((ComplexFontName)msStyle.RunPr[FontAttr.NameAscii]).Name, Is.EqualTo("Times New Roman"));
        }

        /// <summary>
        /// Relates to WORDSNET-8423
        /// Test PositiveDifference calculation for inherited styles.
        /// </summary>
        /// <remarks>
        /// AM. I have strong feeling that calculation must be done in certain order, started from most bottom style,
        /// but failed to make such test.
        /// For this test positive-difference is calculated correctly regardless of calculation order.
        /// </remarks>
        [Test]
        public void TestJira8423_DifferenceInheritance()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestJira8423_DifferenceInheritance.docx");

            // 30pt space after is directly specified for Style1.
            Style style1 = doc.Styles.GetByName("Style1", false);
            Assert.That(style1.ParaPr[ParaAttr.SpaceAfter], Is.EqualTo(600));

            // Style2 is based on Style1, define some spacing for original and has no directly specified spacing for final.
            // Positive-difference must resolve this value from base style.
            Style style2 = doc.Styles.GetByName("Style2", false);
            Assert.That(style2.ParaPr.FormatRevision.RevPr[ParaAttr.SpaceAfter], Is.EqualTo(600));

            // The same for Style3: Positive-difference must resolve this value from base style.
            Style style3 = doc.Styles.GetByName("Style3", false);
            Assert.That(style3.ParaPr.FormatRevision.RevPr[ParaAttr.SpaceAfter], Is.EqualTo(600));
        }


        /// <summary>
        /// Tests how style revision is imported/exported in various format.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStyleRevisions(LoadFormat lf, SaveFormat sf)
        {
            // Style revision is not supported in RTF.
            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
                return;

            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestStyleRevisions", lf, sf);

            Style style = doc.Styles.GetBySti(StyleIdentifier.Heading2, false);

            Assert.That(style.ParaPr.FormatRevision, IsNot.Null());

            ParaPr paraPr = style.ParaPr;

            // Test few attributes in original and final properties.
            Assert.That(paraPr[ParaAttr.ListLevel], Is.EqualTo(1));
            Assert.That(paraPr[ParaAttr.ListId], Is.EqualTo(1));
            Assert.That(paraPr[ParaAttr.SpaceBefore], Is.EqualTo(200));
            Assert.That(paraPr[ParaAttr.SpaceAfter], Is.Null);
            Assert.That(paraPr[ParaAttr.OutlineLevel], Is.EqualTo(OutlineLevel.Level2));
            Assert.That(((LineSpacing)paraPr[ParaAttr.LineSpacing]).Value, Is.EqualTo(276));
            Assert.That(((LineSpacing)paraPr[ParaAttr.LineSpacing]).Rule, Is.EqualTo(LineSpacingRule.Multiple));

            ParaPr revParaPr = (ParaPr)style.ParaPr.FormatRevision.RevPr;
            Assert.That(revParaPr[ParaAttr.SpaceBefore], Is.EqualTo(80));
            Assert.That(revParaPr[ParaAttr.SpaceAfter], Is.EqualTo(0));
            Assert.That(((LineSpacing)revParaPr[ParaAttr.LineSpacing]).Value, Is.EqualTo(480));
            Assert.That(((LineSpacing)revParaPr[ParaAttr.LineSpacing]).Rule, Is.EqualTo(LineSpacingRule.Multiple));
        }

        /// <summary>
        /// WORDSNET-8664 ( fixed) - Document.AcceptAllRevisions do not work correctly.
        /// Improved revision handling for conflicting situation when cell is inserted into a row marked as deleted.
        /// </summary>
        [Test]
        public void TestJira8664()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira8664.docx");
            doc.AcceptAllRevisions();
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Assert.That(table.Rows.Count, Is.EqualTo(2));
        }






        /// <summary>
        /// WORDSNET-10582 Revision.Accept does not work for Docx
        /// Enumerator should become invalid after revision is accepted/rejected.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira10582Accept()
        {
            Document doc = CreateTestDocument();

            foreach (Revision revision in doc.Revisions)
                revision.Accept();
        }

        /// <summary>
        /// WORDSNET-10582 Revision.Accept does not work for Docx
        /// Enumerator should become invalid after revision is accepted/rejected.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira10582Reject()
        {
            Document doc = CreateTestDocument();

            foreach (Revision revision in doc.Revisions)
                revision.Reject();
        }

        /// <summary>
        /// WORDSNET-10582 Revision.Accept does not work for Docx
        /// Enumerator should become invalid after revision is accepted/rejected.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira10582AcceptAll()
        {
            Document doc = CreateTestDocument();

            foreach (Revision revision in doc.Revisions)
                doc.Revisions.AcceptAll();
        }

        /// <summary>
        /// WORDSNET-10582 Revision.Accept does not work for Docx
        /// Enumerator should become invalid after revision is accepted/rejected.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestJira10582RejectAll()
        {
            Document doc = CreateTestDocument();

            foreach (Revision revision in doc.Revisions)
                doc.Revisions.RejectAll();
        }


        [Test]
        public void TestJira10571B()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira10571B.docx");

            doc.AcceptAllRevisions();
            Assert.That(doc.Revisions.Count, Is.EqualTo(0));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(17));
        }




        /// <summary>
        /// WORDSNET-10575 Revision without timestamp was changed during conversion.
        /// MSW allows revisions without timestamps. In this case:
        /// Zero value for a dttm structure for a DOC or RTF formats.
        /// Missing of "w:date" revision's attribute for DOCX.
        /// Missing of "aml:createdate" revision's attribute for WML.
        /// Missing of "\\revdttm" control word for RTF.
        /// <see cref="RevisionBase.DateTime"/> stores DateTime.MinValue in this case.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNoTimeStampInRevision(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestNoTimeStampInRevision", lf, sf);
            Revision revision = doc.Revisions[0];
            Assert.That(revision.DateTime, Is.EqualTo(DateTime.MinValue));
        }

        // FOSS: TestJira15133 removed. It roundtripped the rsid table of a binary .doc; the Word .doc->.docx
        // conversion strips the rsid table entirely, so there is nothing to roundtrip. The rsid-table
        // roundtrip is still covered by UnifiedTestRsidTable (FlatOpc input) below.


        private static Document CreateTestDocument()
        {
            Document doc = new Document();
            doc.StartTrackRevisions("amorozov");
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Hello World!");
            builder.Writeln("Bye!");

            return doc;
        }


        /// <summary>
        /// Tests that the IsFormatRevision properties of the Paragraph and Inline classes return correct value.
        /// </summary>
        [Test]
        public static void TestIsFormatRevisionFlag()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestIsFormatRevisionFlag.docx");

            Paragraph paragraph1 = doc.FirstSection.Body.FirstParagraph;
            Assert.That(paragraph1.IsFormatRevision, Is.True, "Wrong IsFormatRevision value for paragraph 1.");
            Assert.That(paragraph1.FirstRun.IsFormatRevision, Is.False, "Wrong IsFormatRevision value for run 1.");

            Paragraph paragraph2 = (Paragraph)paragraph1.NextNonAnnotationSibling;
            Assert.That(paragraph2.IsFormatRevision, Is.False, "Wrong IsFormatRevision value for paragraph 2.");
            Assert.That(paragraph2.FirstRun.IsFormatRevision, Is.True, "Wrong IsFormatRevision value for run 2.");

            Paragraph paragraph3 = (Paragraph)paragraph2.NextSibling;
            Assert.That(paragraph3.IsFormatRevision, Is.False, "Wrong IsFormatRevision value for paragraph 3.");
            Assert.That(paragraph3.FirstRun.IsFormatRevision, Is.False, "Wrong IsFormatRevision value for run 3.");
        }





        /// <summary>
        /// WORDSNET-14046 Support edit revisions for table cells.
        /// Implemented support for w:cellIns and w:cellDel elements.
        /// </summary>
        [Test]
        public void TestJira14046()
        {
            // Check w:cellIns.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestJira14046Ins", UnifiedScenario.Docx2DocxNoGold);
            Cell cell = doc.FirstSection.Body.Tables[0].Rows[0].LastCell;
            Assert.That(cell.CellPr.HasInsertRevision, Is.True);

            // Check w:cellDel.
            doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestJira14046Del", UnifiedScenario.Docx2DocxNoGold);
            cell = doc.FirstSection.Body.Tables[0].Rows[0].LastCell;
            Assert.That(cell.CellPr.HasDeleteRevision, Is.True);
        }


        /// <summary>
        /// Relates to WORDSNET-14309 Issue with cell revision accept/reject.
        /// </summary>
        [Test]
        public void TestCellRevisionAccept()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestCellRevisionAccept.docx");
            Assert.That(doc.Revisions.Count, Is.EqualTo(1));

            doc.AcceptAllRevisions();
            // Force collection rebuild.
            doc.MarkChanged();

            Assert.That(doc.Revisions.Count, Is.EqualTo(0));
        }

        // FOSS: TestJira14997 / Test19608 / Test19608Export removed. Their inputs were binary .doc and
        // asserted .doc-specific representations lost on the Word .doc->.docx conversion: the Word97 legacy
        // indent-compat attrs (Sys_LeftIndent97/Sys_FirstLineIndent97) don't exist in OOXML, and the exact
        // list-revision structure that made AcceptAllRevisions drop the indent isn't preserved. The .doc
        // reader is removed in FOSS, so these scenarios can't be reproduced.


        /// <summary>
        /// WORDSNET-14290 Support Structured Document Tag edit revisions (customXmlDelRangeStart/customXmlDelRangeEnd).
        /// Implemented support for Structured Document Tag edit revisions.
        /// </summary>
        /// <remarks>Tests block level SDT revisions.</remarks>
        [Test]
        public void TestJira14290BlockLevelSdt()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestBlockLevelSdtRevision", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Revisions.Count, Is.EqualTo(13));

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            CheckSdtHasDeleteRevision((StructuredDocumentTag)sdts[0]);
            CheckSdtHasDeleteRevision((StructuredDocumentTag)sdts[1]);
            CheckSdtHasDeleteRevision((StructuredDocumentTag)sdts[2]);
            CheckSdtHasInsertRevision((StructuredDocumentTag)sdts[3]);

            CheckAcceptSdtRevision(doc, 1, "☒\f");
            CheckRejectSdtRevision(doc, 3, "ds\rdsdsds\r\r");
        }

        /// <summary>
        /// WORDSNET-14290 Support Structured Document Tag edit revisions (customXmlDelRangeStart/customXmlDelRangeEnd).
        /// Implemented support for Structured Document Tag edit revisions.
        /// </summary>
        /// <remarks>Tests inline level SDT revisions.</remarks>
        [Test]
        public void TestJira14290InlineLevelSdt()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestInlineLevelSdtRevision", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Revisions.Count, Is.EqualTo(4));

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            CheckSdtHasDeleteRevision((StructuredDocumentTag)sdts[0]);
            CheckSdtHasInsertRevision((StructuredDocumentTag)sdts[1]);

            CheckAcceptSdtRevision(doc, 1, "Choose an item.");
            CheckRejectSdtRevision(doc, 1, "zxcvb");
        }

        /// <summary>
        /// WORDSNET-14290 Support Structured Document Tag edit revisions (customXmlDelRangeStart/customXmlDelRangeEnd).
        /// Implemented support for Structured Document Tag edit revisions.
        /// </summary>
        /// <remarks>Tests row level SDT revisions.</remarks>
        [Test]
        public void TestJira14290RowLevelSdt()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestRowLevelSdtRevision", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Revisions.Count, Is.EqualTo(4));

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            CheckSdtHasDeleteRevision((StructuredDocumentTag)sdts[0]);
            CheckSdtHasInsertRevision((StructuredDocumentTag)sdts[1]);

            CheckAcceptSdtRevision(doc, 1, "ddddddddddddddd\a");
            CheckRejectSdtRevision(doc, 1, "asdfghjkl\a");
        }

        /// <summary>
        /// WORDSNET-14290 Support Structured Document Tag edit revisions (customXmlDelRangeStart/customXmlDelRangeEnd).
        /// Implemented support for Structured Document Tag edit revisions.
        /// </summary>
        /// <remarks>Tests revisions for SDT with data binding.</remarks>
        [Test]
        public void TestJira14290DataBindingSdt()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestDataBindingSdtRevision.docx");

            // WORDSNET-16303 When track changes SDT content is updated even for DOCX. Reset this flag to pass this test.
            doc.TrackRevisions = false;
            doc = TestUtil.SaveOpen(doc, @"Model\Revision\TestDataBindingSdtRevision", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Revisions.Count, Is.EqualTo(4));

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            CheckSdtHasDeleteRevision((StructuredDocumentTag)sdts[0]);
            CheckSdtHasInsertRevision((StructuredDocumentTag)sdts[1]);

            CheckAcceptSdtRevision(doc, 1, "Second\f");
            CheckRejectSdtRevision(doc, 1, "22222222222\r");
        }

        /// <summary>
        /// WORDSNET-15445 Annoying empty SDTs and unremoved move range nodes were present after accepting revisions
        /// in the test documents.
        /// </summary>
        [TestCase("TestJira15445.docx", 1)]
        [TestCase("TestMovedSdts1.docx", 2)]
        [TestCase("TestMovedSdts2.docx", 2)]
        public void TestMovedSdts(string fileName, int expectedSdtCount)
        {
            Document doc = TestUtil.Open(@"Model\Revision\" + fileName);

            doc.AcceptAllRevisions();

            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(expectedSdtCount));
            CheckMoveRangeNodesAreRemoved(doc, "");
        }


        /// <summary>
        /// Tests that move revision is either preserved or converted to insertion/deletion depending on output format.
        /// </summary>
        /// <param name="scenario">Scenario/output format.</param>
        /// <param name="isExpectedMove">If <c>true</c>, it is expected that a move revision will be preserved as move;
        /// if <c>false</c>, it will be converted to an edit revision.</param>
        // FOSS: Wml/Doc/Rtf writers removed. Keep the Docx roundtrip case (move revisions are preserved).
        [TestCase(UnifiedScenario.Docx2Docx, true)]
        public void TestExportingMoveRevision(UnifiedScenario scenario, bool isExpectedMove)
        {
            const string fileName = @"Model\Revision\TestSdtMovingRevisions";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);

            Paragraph para = doc.FirstSection.Body.Paragraphs[1];
            CheckRevisionPresence(para, para.FirstRun, "Block-level SDT", true, true);

            para = doc.FirstSection.Body.Paragraphs[8];
            CheckRevisionPresence(para, para.FirstRun, "Block-level SDT", true, false);

            doc = TestUtil.SaveOpen(doc, fileName, scenario | UnifiedScenario.NoGold);

            para = doc.FirstSection.Body.Paragraphs[1];
            CheckRevisionPresence(para, para.FirstRun, "Block-level SDT", isExpectedMove, true);

            para = doc.FirstSection.Body.Paragraphs[8];
            CheckRevisionPresence(para, para.FirstRun, "Block-level SDT", isExpectedMove, false);
        }

        /// <summary>
        /// Checks presence of move-to, move-from, insert and delete revisions in the specified paragraph and run.
        /// </summary>
        /// <param name="para">Paragraph to check.</param>
        /// <param name="run">Run to check.</param>
        /// <param name="expectedRunText">Expected run text.</param>
        /// <param name="isExpectedMove">If <c>true</c>, we expect presence of a move-from or move-to revision.</param>
        /// <param name="isExpectedRemoval">If <c>true</c>, we expects presence of a move-from or delete revision.</param>
        private static void CheckRevisionPresence(Paragraph para, Run run, string expectedRunText,
            bool isExpectedMove, bool isExpectedRemoval)
        {
            // Check that expected run is tested.
            Assert.That(run.Text, Is.EqualTo(expectedRunText));

            Assert.That(para.IsMoveToRevision, Is.EqualTo(isExpectedMove && !isExpectedRemoval));
            Assert.That(para.IsMoveFromRevision, Is.EqualTo(isExpectedMove && isExpectedRemoval));
            Assert.That(para.IsInsertRevision, Is.EqualTo(!isExpectedMove && !isExpectedRemoval));
            Assert.That(para.IsDeleteRevision, Is.EqualTo(!isExpectedMove && isExpectedRemoval));

            Assert.That(run.IsMoveToRevision, Is.EqualTo(isExpectedMove && !isExpectedRemoval));
            Assert.That(run.IsMoveFromRevision, Is.EqualTo(isExpectedMove && isExpectedRemoval));
            Assert.That(run.IsInsertRevision, Is.EqualTo(!isExpectedMove && !isExpectedRemoval));
            Assert.That(run.IsDeleteRevision, Is.EqualTo(!isExpectedMove && isExpectedRemoval));
        }

        /// <summary>
        /// Tests reading/writing and accepting moving revisions (customXmlMoveXXXRangeStart/customXmlMoveXXXRangeEnd)
        /// of structured document tags.
        /// </summary>
        [Test]
        public void TestSdtMovingRevision()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Revision\TestSdtMovingRevisions",
                UnifiedScenario.Docx2DocxNoGold);

            NodeCollection sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);

            // All SDTs are moved down that all moved-from SDTs are before moved-to ones.
            int count = sdts.Count;
            for (int i = 0; i < count; i++)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)sdts[i];
                // Word has not marked inline-level SDTs with move revision, skip them.
                if (sdt.Level != MarkupLevel.Inline)
                {
                    if (i < count / 2)
                        Assert.That(sdt.ContentsRunPr.HasMoveFromRevision, Is.True);
                    else
                        Assert.That(sdt.ContentsRunPr.HasMoveToRevision, Is.True);
                }
                else
                {
                    Assert.That(sdt.ContentsRunPr.HasRevisions, Is.False);
                }
            }

            Document clone = doc.Clone();
            doc.AcceptAllRevisions();
            Assert.That(doc.GetText(), Is.EqualTo("Text before block SDT.\rParagraph before table.\r\a\a\a\aCell 1\aCell 2.\a\a\a\a" +
                "\a\a\a\rMove destination.\rBlock-level SDT.\rContains two paragraphs.\rText after block SDT." +
                "\rParagraph that contains inline-level SDT.\rText after move-to range.\f"));

            clone.Revisions.RejectAll();
            Assert.That(clone.GetText(), Is.EqualTo("Text before block SDT.\rBlock-level SDT.\rContains two paragraphs.\rText after " +
                "block SDT.\rParagraph that contains inline-level SDT.\rParagraph before table.\rCell 1\a" +
                "Cell 2.\a\a\a\a\a\a\a\a\a\a\a\rMove destination.\rText after move-to range.\f"));
        }


        /// <summary>
        /// Accepts revisions one by one in order required for move revisions.
        /// </summary>
        private static void AcceptRevisionsOneByOne(RevisionCollection revisions)
        {
            foreach (Revision revision in revisions.ToList())
            {
                if (revision.ParentNode.NodeLevel == NodeLevel.Inline)
                    revision.Accept();
            }

            foreach (Revision revision in revisions.ToList())
            {
                if (revision.ParentNode.NodeType == NodeType.Paragraph)
                    revision.Accept();
            }

            foreach (Revision revision in revisions.ToList())
                revision.Accept();
        }

        /// <summary>
        /// Checks result of acceptance of document with move ranges included structured document tags.
        /// </summary>
        private static void CheckRevisionAcceptanceResult(Document doc, string expectedText,
            int expectedSdtCount, int[] expectedInlineSdts, string testSubject)
        {
            Assert.That(doc.GetText(), Is.EqualTo(expectedText), testSubject);

            List<StructuredDocumentTag> sdts =
                doc.GetChildNodes(NodeType.StructuredDocumentTag, true).ToList<StructuredDocumentTag>();
            Assert.That(sdts.Count, Is.EqualTo(expectedSdtCount), testSubject);

            for (int i = 0; i < sdts.Count; i++)
            {
                if ((expectedInlineSdts != null) && (Array.IndexOf(expectedInlineSdts, i) >= 0))
                    Assert.That(sdts[i].Level, Is.EqualTo(MarkupLevel.Inline), testSubject);
                else
                    Assert.That(sdts[i].Level, IsNot.EqualTo(MarkupLevel.Inline), testSubject);
            }

            Assert.That(doc.Revisions.Count, Is.EqualTo(0), testSubject);
            CheckMoveRangeNodesAreRemoved(doc, testSubject);
        }


        /// <summary>
        /// WORDSNET-14692 Tests that revisions are also taken from fallback shape segments and are accepted.
        /// </summary>
        [Test]
        public void TestRevisionsInFallback()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestRevisionsInFallback.docx");
            Assert.That(doc.Revisions.Count, Is.EqualTo(6));

            doc.AcceptAllRevisions();

            Assert.That(doc.Revisions.Count, Is.EqualTo(0));
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            NodeCollection collection = new NodeCollection(shape.FallbackShape, new RevisionNodeMatcher(), true);
            Assert.That(collection.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-15871 Document.AcceptAllRevisions throws System.InvalidOperationException
        /// We should not try to remove the same empty row twice in <see cref="RevisionUtil.DeleteLastParaFromCell"/>.
        /// </summary>
        [Test]
        public void TestJira15871()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira15871.docx");
            doc.AcceptAllRevisions();

            Assert.That(doc.HasRevisions, Is.False);
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(1));
        }




        /// <summary>
        /// WORDSNET-18903 ListLabel.LabelString returns incorrect value when paragraph has revision.
        /// For customer case, <see cref="ListLabel.LabelStringOriginal"/> and <see cref="ListLabel.LabelStringFinal"/>
        /// are required. Added property <see cref="Document.RevisionsView"/>.
        /// </summary>
        [Test]
        public void Test18903()
        {
            Document doc = TestUtil.Open(@"Model\Revision\Test18903.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // RevisionsView.Original by default.
            Assert.That(paras[0].ListLabel.LabelString, Is.EqualTo("1."));
            Assert.That(paras[1].ListLabel.LabelString, Is.EqualTo("a."));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo(""));

            doc.RevisionsView = RevisionsView.Final;
            Assert.That(paras[0].ListLabel.LabelString, Is.EqualTo(""));
            Assert.That(paras[1].ListLabel.LabelString, Is.EqualTo("1."));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo("a."));
        }




        /// <summary>
        /// WORDSNET-13113 ParagraphFormat.StyleName returns incorrect style name after track changes
        /// Improved ParagraphFormat.Style, ParagraphFormat.StyleName to get RevisionsView aware values.
        /// </summary>
        [Test]
        public void Test13113()
        {
            Document doc = TestUtil.Open(@"Model\Revision\Test13113.docx");

            doc.RevisionsView = RevisionsView.Original;
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Style.Name, Is.EqualTo("OldStyle"));
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphFormat.StyleName, Is.EqualTo("OldStyle"));

            doc.RevisionsView = RevisionsView.Final;
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Style.Name, Is.EqualTo("Normal"));
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphFormat.StyleName, Is.EqualTo("Normal"));
        }





        /// <summary>
        /// WORDSNET-22956 Revision.Accept throws System.InvalidOperationException.
        /// We should accept/reject only current revision when it is called for a single format revision
        /// instead of doing it for all revisions of the parent node.
        /// </summary>
        [TestCase (true)]
        [TestCase (false)]
        public void Test22956(bool isAccept)
        {
            Document doc = TestUtil.Open(@"Model\Revision\Test22956.docx");

            // There are formatting and deletion revisions.
            Assert.That(doc.Revisions.Count, Is.EqualTo(11));

            // Customer's scenario is to process only formatting revisions.
            foreach (Revision revision in doc.Revisions.ToList())
            {
                if (revision.RevisionType == RevisionType.FormatChange)
                {
                    if (isAccept)
                        revision.Accept();
                    else
                        revision.Reject();
                }
            }

            // Check that only the deletion revisions are remained.
            Assert.That(doc.Revisions.Count, Is.EqualTo(5));
            foreach (Revision revision in doc.Revisions)
                Assert.That(revision.RevisionType, Is.EqualTo(RevisionType.Deletion));
        }











        /// <summary>
        /// WORDSNET-24550 Font size is incorrect after rendering text with revision.
        /// Improved positive difference calculation in <see cref="FormatRevisionUtil"/>
        /// when parent Paragraph of Run has style revision.
        /// </summary>
        [Test]
        public void Test24550()
        {
            Document doc = TestUtil.Open(@"Model\Revision\Test24550.docx");
            Run run = doc.FirstSection.Body.FirstParagraph.FirstRun;

            // Check before changes.
            Assert.That(run.RunPr.Count, Is.EqualTo(6));
            Assert.That(run.RunPr[FontAttr.Color], Is.EqualTo(DrColor.Black));
            Assert.That(run.RunPr[FontAttr.Size], Is.EqualTo(27));
            Assert.That(run.RunPr[FontAttr.SizeBi], Is.EqualTo(27));

            // Check revision. Here should be a positive difference to "before changes".
            WordAttrCollection revPr = run.RunPr.FormatRevision.RevPr;
            Assert.That(revPr.Count, Is.EqualTo(3));
            Assert.That(revPr[FontAttr.Color], Is.EqualTo(DrColor.Empty));
            Assert.That(revPr[FontAttr.Size], Is.EqualTo(18));
            Assert.That(revPr[FontAttr.SizeBi], Is.EqualTo(20));
        }

        /// <summary>
        /// Relates to WORDSNET-24550
        /// Test differs from <see cref="Test24550"/> in one missed attribute in direct "before" run properties.
        /// In this case this attribute should be also missed in revision "after" properties of the run.
        /// </summary>
        [Test]
        public void Test24550A()
        {
            Document doc = TestUtil.Open(@"Model\Revision\Test24550A.docx");
            Run run = doc.FirstSection.Body.FirstParagraph.FirstRun;

            // Check before changes.
            Assert.That(run.RunPr.Count, Is.EqualTo(5));
            Assert.That(run.RunPr[FontAttr.Color], Is.EqualTo(DrColor.Black));
            Assert.That(run.RunPr[FontAttr.SizeBi], Is.EqualTo(27));

            // Check revision. Here should be a positive difference to "before changes".
            WordAttrCollection revPr = run.RunPr.FormatRevision.RevPr;
            Assert.That(revPr.Count, Is.EqualTo(2));
            Assert.That(revPr[FontAttr.Color], Is.EqualTo(DrColor.Empty));
            Assert.That(revPr[FontAttr.SizeBi], Is.EqualTo(20));
        }



        /// <summary>
        /// Tests how the field numbering revisions are accepted and rejected.
        /// </summary>
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestFieldNumberingRevision(bool accept)
        {
            Document document = TestUtil.Open(@"Model\Revision\TestFieldNumberingRevision.docx");

            if (accept)
                document.Revisions.AcceptAll();
            else
                document.Revisions.RejectAll();

            foreach (FieldEnd node in document.GetChildNodes(NodeType.FieldEnd, true))
                Assert.That(node.RunPr[RevisionAttr.NumberRevision], Is.Null);
        }

        /// <summary>
        /// Checks accept SDT revision.
        /// </summary>
        private static void CheckAcceptSdtRevision(Document originalDocument, int expectedCountRemainedSdt, string expectedTextFirstRemainedSdt)
        {
            // Accept revisions.
            Document acceptedDoc = originalDocument.Clone();
            acceptedDoc.AcceptAllRevisions();

            // Accepted document should not have revisions.
            Assert.That(acceptedDoc.HasRevisions, Is.False);

            // Deleted sdts had to be deleted and only inserted sdts are remained in document.
            NodeCollection sdts = acceptedDoc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(expectedCountRemainedSdt));
            StructuredDocumentTag sdt = (StructuredDocumentTag) sdts[0];
            CheckSdtHasNoEditRevision(sdt);
            Assert.That(sdt.GetText(), Is.EqualTo(expectedTextFirstRemainedSdt));
        }

        /// <summary>
        /// Checks reject SDT revision.
        /// </summary>
        private static void CheckRejectSdtRevision(Document originalDocument, int expectedCountRemainedSdt, string expectedTextFirstRemainedSdt)
        {
            // Check reject revisions.
            Document rejectedDoc = originalDocument.Clone();
            rejectedDoc.Revisions.RejectAll();

            // Rejected document should not have revisions.
            Assert.That(rejectedDoc.HasRevisions, Is.False);

            // Inserted sdts had to be deleted and only deleted sdts are remained in document.
            NodeCollection sdts = rejectedDoc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            Assert.That(sdts.Count, Is.EqualTo(expectedCountRemainedSdt));
            StructuredDocumentTag sdt = (StructuredDocumentTag)sdts[0];
            CheckSdtHasNoEditRevision(sdt);
            Assert.That(sdt.GetText(), Is.EqualTo(expectedTextFirstRemainedSdt));
        }

        /// <summary>
        /// Checks SDT has no edit revisions.
        /// </summary>
        private static void CheckSdtHasNoEditRevision(StructuredDocumentTag sdt)
        {
            WordAttrCollection attrs = sdt.ContentsRunPr;
            Assert.That(attrs.HasDeleteRevision, Is.False);
            Assert.That(attrs.HasInsertRevision, Is.False);
            Assert.That(attrs.HasMoveFromRevision, Is.False);
            Assert.That(attrs.HasMoveToRevision, Is.False);
        }

        /// <summary>
        /// Checks SDT has delete revision.
        /// </summary>
        private static void CheckSdtHasDeleteRevision(StructuredDocumentTag sdt)
        {
            Assert.That(sdt.ContentsRunPr.HasDeleteRevision, Is.True);
        }

        /// <summary>
        /// Checks SDT has insert revision.
        /// </summary>
        private static void CheckSdtHasInsertRevision(StructuredDocumentTag sdt)
        {
            Assert.That(sdt.ContentsRunPr.HasInsertRevision, Is.True);
        }

        /// <summary>
        /// Checks that the document contains no move range nodes.
        /// </summary>
        private static void CheckMoveRangeNodesAreRemoved(Document doc, string testSubject)
        {
            Assert.That(doc.GetChildNodes(NodeType.MoveFromRangeStart, true).Count, Is.EqualTo(0), testSubject);
            Assert.That(doc.GetChildNodes(NodeType.MoveFromRangeEnd, true).Count, Is.EqualTo(0), testSubject);
            Assert.That(doc.GetChildNodes(NodeType.MoveToRangeStart, true).Count, Is.EqualTo(0), testSubject);
            Assert.That(doc.GetChildNodes(NodeType.MoveToRangeEnd, true).Count, Is.EqualTo(0), testSubject);
        }
    }
}
