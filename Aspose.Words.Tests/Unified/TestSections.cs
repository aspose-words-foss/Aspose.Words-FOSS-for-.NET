// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/12/2003 by Roman Korchagin

using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for sections.
    /// 
    /// These tests were important in the old object model because I had to maintain two parallel
    /// collections in the model and facade layers. Now this is simpler because there is no 
    /// facade and synchornization between collections to perform, but keep the tests.
    /// </summary>
    [TestFixture]
    public class TestSections : UnifiedTestsBase
    {
        /// <summary>
        /// WORDSNET-1474 Setting PaperSize to Custom throws an exception.
        /// </summary>
        [Test]
        public void TestDefect1474()
        {
            DocumentBuilder builder = new DocumentBuilder();
            PageSetup ps = builder.PageSetup;

            Assert.That(ps.PaperSize, Is.EqualTo(PaperSize.Letter));

            // These properties are interrelated.
            // Setting custom paper width and height will return us custom paper size.
            ps.PageWidth = 100;
            ps.PageHeight = 100;
            Assert.That(ps.PaperSize, Is.EqualTo(PaperSize.Custom));

            // Setting custom paper size does not change paper width and height.
            ps.PaperSize = PaperSize.Custom;
            Assert.That(ps.PageWidth, Is.EqualTo(100));
            Assert.That(ps.PageHeight, Is.EqualTo(100));
            Assert.That(ps.PaperSize, Is.EqualTo(PaperSize.Custom));
        }

        /// <summary>
        /// WORDSNET-4254 “NullReferenceException” occurs when try to save document with empty section.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMinimum(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            Section section = new Section(doc);
            doc.Sections.Add(section);
            TestUtil.SaveOpen(doc, @"Model\Section\TestMinimum", lf, sf);
        }

        /// <summary>
        /// Checks that the source document is as we expect it.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPrecondition(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Section\TestSections", lf, sf);

            Assert.That(doc.Sections.Count, Is.EqualTo(4));

            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("Section 1\r\rNext page section break.\x0c"));
            Assert.That(doc.Sections[1].GetText(), Is.EqualTo("Section 2\rSame page section break.\x0c"));
            Assert.That(doc.Sections[2].GetText(), Is.EqualTo("Section 3\rNext page section break.\x0c"));
            Assert.That(doc.Sections[3].GetText(), Is.EqualTo("Section 4\r\x0c"));
        }

        /// <summary>
        /// Note it is okay if the user deletes the first section that contains the watermark,
        /// the document will still have a spread of garbage text throughout the document.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeleteFirstSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);

            //Delete the section.
            doc.Sections[0].Range.Delete();
            Assert.That(doc.Sections.Count, Is.EqualTo(3));

            //Reload to check that doc was valid and survived save an open.
            doc = TestUtil.SaveOpen(doc, @"Model\Section\TestDeleteFirstSection", lf, sf);

            Assert.That(doc.Sections.Count, Is.EqualTo(3));
            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("Section 2\rSame page section break.\x0c"));
            Assert.That(doc.Sections[1].GetText(), Is.EqualTo("Section 3\rNext page section break.\x0c"));
            Assert.That(doc.Sections[2].GetText(), Is.EqualTo("Section 4\r\x0c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeleteThreeSections(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);

            //Delete sections via range just to see if it works.
            doc.Sections[0].Range.Delete();
            doc.Sections[0].Range.Delete();
            doc.Sections[0].Range.Delete();
            //The sections were indeed deleted. 
            Assert.That(doc.Sections.Count, Is.EqualTo(1));
            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("Section 4\r\x0c"));
        }

        /// <summary>
        /// Notw it is possible to delete last section.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeleteLastSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            doc.RemoveChild(doc.Sections[3]);
            Assert.That(doc.Sections.Count, Is.EqualTo(3));

            //Reload and check that the document was valid so it survived save and open.
            doc = TestUtil.SaveOpen(doc, @"Model\Section\TestDeleteLastSection", lf, sf);
            Assert.That(doc.Sections.Count, Is.EqualTo(3));
            Assert.That(doc.Sections[2].GetText(), Is.EqualTo("Section 3\rNext page section break.\x0c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeleteAllSections(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            doc.RemoveAllChildren();
            Assert.That(doc.Sections.Count, Is.EqualTo(0));
            
            //Reload, one section will be recreated.
            doc = TestUtil.SaveOpen(doc, @"Model\Section\TestDeleteAllSections", lf, sf);
            Assert.That(doc.Sections.Count, Is.EqualTo(1));
            Assert.That(doc.GetText(), Is.EqualTo("\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeleteDocumentRange(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            doc.Range.Delete();
            Assert.That(doc.Sections.Count, Is.EqualTo(0));
            Assert.That(doc.Range.Text, Is.EqualTo(""));
        }

        /// <summary>
        /// Tests how add and remove section work.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMoveSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            Section s1 = doc.Sections[0];
            Section s4 = doc.Sections[3];
            
            //Try add to end and insert to front
            doc.AppendChild(s1);
            doc.PrependChild(s4);

            doc = TestUtil.SaveOpen(doc, @"Model\Section\TestMoveSection", lf, sf);
            Assert.That(doc.Sections.Count, Is.EqualTo(4));

            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("Section 4\r\x0c"));
            Assert.That(doc.Sections[1].GetText(), Is.EqualTo("Section 2\rSame page section break.\x0c"));
            Assert.That(doc.Sections[2].GetText(), Is.EqualTo("Section 3\rNext page section break.\x0c"));
            Assert.That(doc.Sections[3].GetText(), Is.EqualTo("Section 1\r\rNext page section break.\x0c"));
        }

        /// <summary>
        /// Adding a section that is not yet removed from the document is now possible.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddSectionNotRemoved(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            Section s1 = doc.Sections[0];
            doc.AppendChild(s1);
            //This is now the last section.
            Assert.That(doc.Sections[3], Is.EqualTo(s1));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCloneSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            Section s1 = doc.Sections[0].Clone();
            doc.InsertBefore(s1, doc.Sections[1]);

            doc = TestUtil.SaveOpen(doc, @"Model\Section\TestCloneSection", lf, sf);
            Assert.That(doc.Sections.Count, Is.EqualTo(5));

            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("Section 1\r\rNext page section break.\x0c"));
            Assert.That(doc.Sections[1].GetText(), Is.EqualTo("Section 1\r\rNext page section break.\x0c"));
            Assert.That(doc.Sections[2].GetText(), Is.EqualTo("Section 2\rSame page section break.\x0c"));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestClearContent(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            doc.Sections[0].ClearContent(); 
            Assert.That(doc.Sections.Count, Is.EqualTo(4));
            Assert.That(doc.Sections[0].GetText(), Is.EqualTo("\x0c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCopyContent(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Section\TestSections", lf);
            doc.Sections[2].PrependContent(doc.Sections[3]);
            doc.Sections[2].AppendContent(doc.Sections[3]);
            doc = TestUtil.SaveOpen(doc, @"Model\Section\TestCopyContent", lf, sf);
            
            Assert.That(doc.Sections.Count, Is.EqualTo(4));
            Assert.That(doc.Sections[2].GetText(), Is.EqualTo("Section 4\r\rSection 3\rNext page section break.\rSection 4\r\x0c"));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextDirection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Section\TestTextDirection", lf, sf);
            CheckTextDirection(doc, 0, TextFlow.Horizontal);
            CheckTextDirection(doc, 1, TextFlow.Vertical);
            CheckTextDirection(doc, 2, TextFlow.HorizontalRotatedFarEast);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFirstEvenOdd(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestFirstEvenOdd", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFirstPage(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestFirstPage", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderClear(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderClear", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderMany(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderMany", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderSame(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderSame", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderSimple(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestHeaderSimple", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLinked(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestLinked", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPushesMargin(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestPushesMargin", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSimpleHeader(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Header\TestSimpleHeader", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEqualColumns(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestEqualColumns", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomColumns(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestCustomColumns", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestColumnBreakStandalone(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestColumnBreakStandalone", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestColumnBreakWithText(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestColumnBreakWithText", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingExactly(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestLineSpacingExactly", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingMultiple(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestLineSpacingMultiple", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageBreakBefore(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestPageBreakBefore", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSimple(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestSimple", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSpaceAfter(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Column\TestSpaceAfter", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderAppliesToOther(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestBorderAppliesToOther", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderFromPage(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestBorderFromPage", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderFromPageDisjoint(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestBorderFromPageDisjoint", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderFromText(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestBorderFromText", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderFromTextDisjoint(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestBorderFromTextDisjoint", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderNoSurround(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestBorderNoSurround", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGutterLeft(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestGutterLeft", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGutterRight(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestGutterRight", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGutterTop(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Page\TestGutterTop", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestColumns(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestColumns", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestColumns1(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestColumns1", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDisabledFirstFooter(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestDisabledFirstFooter", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFirstOddEvenHeader(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestFirstOddEvenHeader", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFooterFirstBig(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestFooterFirstBig", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeaderFromEdge(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestHeaderFromEdge", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineNumbers(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestLineNumbers", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLinkedHeader(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestLinkedHeader", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNote(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestNote", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestOddEvenHeader(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestOddEvenHeader", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageBorders(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestPageBorders", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageNumbers(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestPageNumbers", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageNumberStyles(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestPageNumberStyles", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageSizeAndMargins(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestPageSizeAndMargins", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPaperSource(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestPaperSource", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisions(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestRevisions", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakAfterPageBreak(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakAfterPageBreak", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakCont(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakCont", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakContEmptyPara(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakContEmptyPara", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakContPara(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakContPara", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakContStandalone(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakContStandalone", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakEndOfPage(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakEndOfPage", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakNewColumn(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakNewColumn", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreaksEmpty(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreaksEmpty", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakStandalone(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakStandalone", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakWithPageBreakBefore(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakWithPageBreakBefore", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreakWithText(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionBreakWithText", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionStart(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Section\TestSectionStart", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBody(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBody", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBodyComment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBodyComment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBodyFootnoteEndnote(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBodyFootnoteEndnote", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBodyHeaderFooter(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBodyHeaderFooter", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBodyHeaderFooterFootnoteEndnoteComment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBodyHeaderFooterFootnoteEndnoteComment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBodyTextbox(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBodyTextbox", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBodyTextboxHeaderTextbox(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Stories\TestBodyTextboxHeaderTextbox", lf, sf);
        }


        private static void CheckPageBorderArt(Section section, double lineWidth, PageBorderArt borderArt)
        {
            Border border = section.PageSetup.Borders.Top;
            Assert.That(true, Is.EqualTo(border.IsPageBorderArt));
            Assert.That((LineStyle)borderArt, Is.EqualTo(border.LineStyle));
             Assert.That(lineWidth, Is.EqualTo(border.LineWidth).Within(2));
        }

        private static void CheckTextDirection(Document doc, int sectionIndex, TextFlow flow)
        {
            Assert.That(flow, Is.EqualTo(doc.Sections[sectionIndex].PageSetup.TextFlow));
        }
    }
}
