// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2016 by Alexey Morozov

using Aspose.Words.Formatting.Intern;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests attribute interning.
    /// </summary>
    [TestFixture]
    public class TestIntern
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests that collection uninterned before changed.
        /// </summary>
        [Test]
        public void TestIntern01()
        {
            InternManager internManager = new InternManager();

            RunPr runPr1 = new RunPr();
            RunPr runPr2 = new RunPr();

            runPr1.Italic = AttrBoolEx.True;
            runPr1.Bold = AttrBoolEx.True;
            runPr2.Italic = AttrBoolEx.True;
            runPr2.Bold = AttrBoolEx.True;

            Assert.That(runPr1.InternState, Is.EqualTo(InternState.None));
            Assert.That(runPr2.InternState, Is.EqualTo(InternState.None));

            internManager.Add(runPr1);
            Assert.That(runPr1.InternState, Is.EqualTo(InternState.Pooled));

            internManager.Add(runPr2);
            Assert.That(runPr2.InternState, Is.EqualTo(InternState.Interned));

            Assert.That(runPr1.Italic, Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr1.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr2.Italic, Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr2.Bold, Is.EqualTo(AttrBoolEx.True));

            runPr2.Italic = AttrBoolEx.False;

            // Collection is detached.
            Assert.That(runPr2.InternState, Is.EqualTo(InternState.None));

            Assert.That(runPr2.Italic, Is.EqualTo(AttrBoolEx.False));
            Assert.That(runPr2.Bold, Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Tests that collection unpooled before change.
        /// </summary>
        [Test]
        public void TestIntern02()
        {
            InternManager internManager = new InternManager();

            RunPr runPr1 = new RunPr();
            RunPr runPr2 = new RunPr();

            runPr1.Italic = AttrBoolEx.True;
            runPr1.Bold = AttrBoolEx.True;
            runPr2.Italic = AttrBoolEx.True;
            runPr2.Bold = AttrBoolEx.True;

            internManager.Add(runPr1);
            Assert.That(runPr1.InternState, Is.EqualTo(InternState.Pooled));

            internManager.Add(runPr2);
            Assert.That(runPr2.InternState, Is.EqualTo(InternState.Interned));

            Assert.That(runPr1.Italic, Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr1.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr2.Italic, Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr2.Bold, Is.EqualTo(AttrBoolEx.True));

            runPr1.Italic = AttrBoolEx.False;
            // First collection is removed from pool.
            Assert.That(runPr1.InternState, Is.EqualTo(InternState.None));

            Assert.That(runPr1.Italic, Is.EqualTo(AttrBoolEx.False));
            Assert.That(runPr2.Italic, Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Tests that collection with complex attribute uninterned before attribute part is changed.
        /// </summary>
        [Test]
        public void TestIntern03()
        {
            InternManager internManager = new InternManager();

            AttrCollection pr1 = new RunPr();
            AttrCollection pr2 = new RunPr();

            LineSpacing spacing1 = new LineSpacing(240, LineSpacingRule.Multiple);
            LineSpacing spacing2 = new LineSpacing(240, LineSpacingRule.Multiple);

            pr1.SetAttr(ParaAttr.LineSpacing, spacing1);
            pr2.SetAttr(ParaAttr.LineSpacing, spacing2);

            internManager.Add(pr1);
            internManager.Add(pr2);

            Assert.That(pr1.InternState, Is.EqualTo(InternState.Pooled));
            Assert.That(pr2.InternState, Is.EqualTo(InternState.Interned));

            spacing2.Value = 480;

            // After this we second collection should be uninterned.
            Assert.That(pr2.InternState, Is.EqualTo(InternState.None));

            // Verify that value will not notify anymore.
            Assert.That(spacing2.IsAttached, Is.False);

            spacing2.Value = 240;
        }

        /// <summary>
        /// Two complex attributes, one is changed.
        /// </summary>
        [Test]
        public void TestIntern04()
        {
            InternManager internManager = new InternManager();

            AttrCollection pr1 = new RunPr();
            LineSpacing spacing1 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr1.SetAttr(ParaAttr.LineSpacing, spacing1);
            Height height1 = new Height(HeightRule.AtLeast, 100);
            pr1.SetAttr(ParaAttr.FrameHeight, height1);
            internManager.Add(pr1);

            AttrCollection pr2 = new RunPr();
            LineSpacing spacing2 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr2.SetAttr(ParaAttr.LineSpacing, spacing2);
            Height height2 = new Height(HeightRule.AtLeast, 100);
            pr2.SetAttr(ParaAttr.FrameHeight, height2);
            internManager.Add(pr2);

            Assert.That(pr1.InternState, Is.EqualTo(InternState.Pooled));
            Assert.That(pr2.InternState, Is.EqualTo(InternState.Interned));

            spacing2.Value = 480;

            // After this we second collection should be uninterned.
            Assert.That(pr2.InternState, Is.EqualTo(InternState.None));

            // Verify that value will not notify anymore.
            Assert.That(spacing2.IsAttached, Is.False);

            height2.Value = 10;

            // Verify that first value remains unchanged.
            Assert.That(height1.Value, Is.EqualTo(100));
        }

        /// <summary>
        /// Two complex attributes, one is changed.
        /// </summary>
        [Test]
        public void TestIntern05()
        {
            InternManager internManager = new InternManager();

            AttrCollection pr1 = new RunPr();
            LineSpacing spacing1 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr1.SetAttr(ParaAttr.LineSpacing, spacing1);
            Height height1 = new Height(HeightRule.AtLeast, 100);
            pr1.SetAttr(ParaAttr.FrameHeight, height1);
            internManager.Add(pr1);

            AttrCollection pr2 = new RunPr();
            LineSpacing spacing2 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr2.SetAttr(ParaAttr.LineSpacing, spacing2);
            Height height2 = new Height(HeightRule.AtLeast, 100);
            pr2.SetAttr(ParaAttr.FrameHeight, height2);
            internManager.Add(pr2);

            Assert.That(pr1.InternState, Is.EqualTo(InternState.Pooled));
            Assert.That(pr2.InternState, Is.EqualTo(InternState.Interned));

            // Remove attribute from interned collection.
            pr2.Remove(ParaAttr.FrameHeight);

            // Collection now is uninterned.
            Assert.That(pr2.InternState, Is.EqualTo(InternState.None));

            // And value is still attached.
            Assert.That(spacing2.IsAttached, Is.True);

            // But detached after first set.
            spacing2.Rule = LineSpacingRule.AtLeast;
            Assert.That(spacing2.IsAttached, Is.False);
        }

        /// <summary>
        /// Tests that complex attributes are deep cloned when uninterned.
        /// </summary>
        [Test]
        public void TestIntern06()
        {
            InternManager internManager = new InternManager();

            ParaPr pr1 = new ParaPr();
            LineSpacing spacing1 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr1.SetAttr(ParaAttr.LineSpacing, spacing1);
            pr1.SetAttr(ParaAttr.Alignment, ParagraphAlignment.Center);
            internManager.Add(pr1);

            ParaPr pr2 = new ParaPr();
            LineSpacing spacing2 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr2.SetAttr(ParaAttr.LineSpacing, spacing2);
            pr2.SetAttr(ParaAttr.Alignment, ParagraphAlignment.Center);
            internManager.Add(pr2);

            Assert.That(pr1.InternState, Is.EqualTo(InternState.Pooled));
            Assert.That(pr2.InternState, Is.EqualTo(InternState.Interned));

            // Change alignment in second collection.
            pr2.SetAttr(ParaAttr.Alignment, ParagraphAlignment.Right);
            Assert.That(pr2.InternState, Is.EqualTo(InternState.None));

            // Change value of second complex attribute.
            pr2.LineSpacing = 360;

            // Check deep clone.
            Assert.That(ReferenceEquals(pr1[ParaAttr.LineSpacing], pr2[ParaAttr.LineSpacing]), Is.False);
            Assert.That(pr1.LineSpacing, Is.EqualTo(240));
            Assert.That(pr2.LineSpacing, Is.EqualTo(360));
        }

        /// <summary>
        /// Tests that we correctly deep clone when collection is unpooled.
        /// </summary>
        [Test]
        public void TestIntern07()
        {
            InternManager internManager = new InternManager();

            ParaPr pr1 = new ParaPr();
            LineSpacing spacing1 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr1.SetAttr(ParaAttr.LineSpacing, spacing1);
            pr1.SetAttr(ParaAttr.Alignment, ParagraphAlignment.Center);
            internManager.Add(pr1);

            ParaPr pr2 = new ParaPr();
            LineSpacing spacing2 = new LineSpacing(240, LineSpacingRule.Multiple);
            pr2.SetAttr(ParaAttr.LineSpacing, spacing2);
            pr2.SetAttr(ParaAttr.Alignment, ParagraphAlignment.Center);
            internManager.Add(pr2);

            Assert.That(pr1.InternState, Is.EqualTo(InternState.Pooled));
            Assert.That(pr2.InternState, Is.EqualTo(InternState.Interned));

            // Unpool first collection
            pr1.Alignment = ParagraphAlignment.Distributed;

            // Change complex attribute in it.
            pr1.LineSpacingRule = LineSpacingRule.Exactly;

            // Check that second collection is not changed.
            Assert.That(pr2.LineSpacingRule, Is.EqualTo(LineSpacingRule.Multiple));

            // Unintern second.
            pr2.Alignment = ParagraphAlignment.Left;

            Assert.That(ReferenceEquals(pr1[ParaAttr.LineSpacing], pr2[ParaAttr.LineSpacing]), Is.False);

            Assert.That(pr1.Alignment, Is.EqualTo(ParagraphAlignment.Distributed));
            Assert.That(pr2.Alignment, Is.EqualTo(ParagraphAlignment.Left));
        }

        /// <summary>
        /// Tests that after validator executed all collections are interned.
        /// </summary>
        [Test]
        public void Test1()
        {
            Document doc = TestUtil.Open(@"Model\Intern\Table.docx");

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Markdown);
            so.MemoryOptimization = true;
            DocumentValidator validator = TestUtil.ExecuteValidator(doc, so);

            InternManager internManager = doc.InternManager;
            Assert.That(internManager.PooledCount, Is.EqualTo(24));
            Assert.That(internManager.InternedCount, Is.EqualTo(107));
            Assert.That(internManager.EmptyCount, Is.EqualTo(6));

            Row row = doc.FirstSection.Body.Tables[0].FirstRow;

            // Get run from first cell. It should be pooled.
            RunPr runPr = row.Cells[0].FirstParagraph.FirstRun.RunPr;
            Assert.That(runPr.InternState, Is.EqualTo(InternState.Pooled));
            Assert.That(runPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(22));

            // Get run from second cell and it should be interned.
            runPr = row.Cells[1].FirstParagraph.FirstRun.RunPr;
            Assert.That(runPr.InternState, Is.EqualTo(InternState.Interned));
            Assert.That(runPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(22));

            validator.Revert();

            // InternManager is removed from document.
            Assert.That(doc.InternManager, Is.Null);

            Assert.That(internManager.UnPooledCount, Is.EqualTo(24));
            Assert.That(internManager.UnInternedCount, Is.EqualTo(107));

            // Cell properties are un-pooled and un-interned.
            Assert.That(row.Cells[0].FirstParagraph.FirstRun.RunPr.InternState, Is.EqualTo(InternState.None));
            Assert.That(row.Cells[1].FirstParagraph.FirstRun.RunPr.InternState, Is.EqualTo(InternState.None));
        }

        /// <summary>
        /// Tests that complex attributes are interned.
        /// </summary>
        [Test]
        public void TestComplex1()
        {
            Document doc = TestUtil.Open(@"Model\Intern\Complex1.docx");

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Markdown);
            so.MemoryOptimization = true;
            TestUtil.ExecuteValidator(doc, so);

            // FOSS: validator runs with Markdown (Pdf removed); it pools one extra collection vs the old Pdf pass.
            Assert.That(doc.InternManager.PooledCount, Is.EqualTo(7));
            Assert.That(doc.InternManager.InternedCount, Is.EqualTo(3));

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // First paragraph properties are pooled.
            Assert.That(paras[0].ParaPr.InternState, Is.EqualTo(InternState.Pooled));

            // and second paragraph is interned.
            Assert.That(paras[1].ParaPr.InternState, Is.EqualTo(InternState.Interned));
        }

        /// <summary>
        /// Tests more complex properties such as borders and shadings.
        /// </summary>
        [Test]
        public void TestComplex2()
        {
            Document doc = TestUtil.Open(@"Model\Intern\Complex2.docx");

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Markdown);
            so.MemoryOptimization = true;
            TestUtil.ExecuteValidator(doc, so);

            // FOSS: validator runs with Markdown (Pdf removed); it pools one extra collection vs the old Pdf pass.
            Assert.That(doc.InternManager.PooledCount, Is.EqualTo(14));
            Assert.That(doc.InternManager.InternedCount, Is.EqualTo(19));
        }

        /// <summary>
        /// Tests that cloned document gets uninterned collections.
        /// </summary>
        [Test]
        public void TestClone()
        {
            Document srcDoc = TestUtil.Open(@"Model\Intern\Complex2.docx");

            InternManager.Add(srcDoc);

            CheckInternState(srcDoc, true);

            Document clonedDoc = (Document)srcDoc.Clone(true);

            // Cloned doc gets normal properties.
            CheckInternState(clonedDoc, false);

            // And whole source document still interned.
            CheckInternState(srcDoc, true);
        }



        /// <summary>
        /// WORDSNET-19069 NullReferenceException when saving DOCX to PDF with MemoryOptimization enabled
        /// TableStyle cloning issue.
        /// </summary>
        [Test]
        public void Test19069()
        {
            Document doc = TestUtil.Open(@"Model\Intern\Test19069.docx");

            InternManager.Add(doc);

            Document doc1 = (Document)doc.Clone(false);

            TableStyle tableNormal = (TableStyle)doc1.Styles.GetBySti(StyleIdentifier.TableNormal, false);
            Assert.That(tableNormal.TablePr.InternState, Is.EqualTo(InternState.None));

            InternManager.Add(doc1);
            InternManager.Remove(doc1);

            InternManager.Remove(doc);
        }



        /// <summary>
        /// Checks that whole document is interned or not depending on given parameter.
        /// </summary>
        private static void CheckInternState(Document doc, bool interned)
        {
            foreach (Paragraph para in doc.FirstSection.Body.Paragraphs)
            {
                CheckInternState(para.ParaPr, interned);
                CheckInternState(para.ParagraphBreakRunPr, interned);

                foreach (Run run in para.Runs)
                    CheckInternState(run.RunPr, interned);
            }

            foreach (Table table in doc.FirstSection.Body.Tables)
            {
                foreach (Row row in table.Rows)
                {
                    CheckInternState(row.TablePr, interned);
                    foreach (Cell cell in row.Cells)
                        CheckInternState(cell.CellPr, interned);
                }
            }

            foreach (Style style in doc.Styles)
            {
                CheckInternState(style.RunPr, interned);

                if(style.Type == StyleType.Paragraph)
                    CheckInternState(style.ParaPr, interned);
            }
        }

        private static void CheckInternState(AttrCollection pr, bool interned)
        {
            if (pr.Count == 0)
                return;

            Assert.That((InternState.None != pr.InternState), Is.EqualTo(interned));
        }
    }
}
