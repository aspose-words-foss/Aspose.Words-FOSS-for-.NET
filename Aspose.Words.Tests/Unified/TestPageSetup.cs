// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.IO;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for PageSetup and text columns.
    /// </summary>
    [TestFixture]
    public class TestPageSetup : UnifiedTestsBase
    {
        /// <summary>
        /// WORDSNET-3301 Allow specifying "Text Flow" through PageSetup.
        /// </summary>
        [Test]
        public void TestOrientations()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            foreach (TextOrientation to in Enum.GetValues(typeof(TextOrientation)))
            {
                builder.CurrentSection.PageSetup.TextOrientation = to;
                builder.Writeln(to.ToString());
                builder.InsertSection(SectionStart.NewPage);
            }

            const string testName = @"Model\Page\orientations";

            // FOSS: only the Docx roundtrip survives — Doc/Rtf/WordML save were removed.
            AssertOrientations(TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold));
        }

        private static void AssertOrientations(Document saved)
        {
            int count = 0;
            foreach (TextOrientation to in Enum.GetValues(typeof (TextOrientation)))
            {
                Section current = saved.Sections[count++];
                Assert.That(current.PageSetup.TextOrientation, Is.EqualTo(to));
            }
        }

        [Test]
        public void TestDefaults()
        {
            ISectionAttrSource sectionAttrSource = new SectPr();
            PageSetup ps = new PageSetup(sectionAttrSource, new DocPr());

            Assert.That(ps.SectionStart, Is.EqualTo(SectionStart.NewPage));
            Assert.That(ps.DifferentFirstPageHeaderFooter, Is.EqualTo(false));
            Assert.That(ps.SuppressEndnotes, Is.EqualTo(false));
            Assert.That(ps.VerticalAlignment, Is.EqualTo(PageVerticalAlignment.Top));
            Assert.That(ps.TextFlow, Is.EqualTo(TextFlow.Horizontal));
            Assert.That(ps.Bidi, Is.EqualTo(false));

            Assert.That(ps.PageWidth, Is.EqualTo(ConvertUtil.InchToPoint(8.5)));
            Assert.That(ps.PageHeight, Is.EqualTo(ConvertUtil.InchToPoint(11.0)));
            Assert.That(ps.Orientation, Is.EqualTo(Orientation.Portrait));
            Assert.That(ps.PaperCode, Is.EqualTo(0));

            Assert.That(ps.LeftMargin, Is.EqualTo(ConvertUtil.InchToPoint(1.25)));
            Assert.That(ps.RightMargin, Is.EqualTo(ConvertUtil.InchToPoint(1.25)));
            Assert.That(ps.TopMargin, Is.EqualTo(ConvertUtil.InchToPoint(1.0)));
            Assert.That(ps.BottomMargin, Is.EqualTo(ConvertUtil.InchToPoint(1.0)));
            Assert.That(ps.HeaderDistance, Is.EqualTo(ConvertUtil.InchToPoint(0.5)));
            Assert.That(ps.FooterDistance, Is.EqualTo(ConvertUtil.InchToPoint(0.5)));
            Assert.That(ps.Gutter, Is.EqualTo(0d));

            Assert.That(ps.FirstPageTray, Is.EqualTo(0));
            Assert.That(ps.OtherPagesTray, Is.EqualTo(0));

            Assert.That(ps.ChapterPageSeparator, Is.EqualTo(ChapterPageSeparator.Hyphen));
            Assert.That(ps.HeadingLevelForChapter, Is.EqualTo(0));
            Assert.That(ps.PageNumberStyle, Is.EqualTo(NumberStyle.Arabic));
            Assert.That(ps.RestartPageNumbering, Is.EqualTo(false));
            Assert.That(ps.PageStartingNumber, Is.EqualTo(1));

            Assert.That(ps.LineNumberRestartMode, Is.EqualTo(LineNumberRestartMode.RestartPage));
            Assert.That(ps.LineNumberCountBy, Is.EqualTo(0));
            Assert.That(ps.LineNumberDistanceFromText, Is.EqualTo(0d));
            Assert.That(ps.LineStartingNumber, Is.EqualTo(1));    

            Assert.That(ps.TextColumns.EvenlySpaced, Is.EqualTo(true));
            Assert.That(ps.TextColumns.Spacing, Is.EqualTo(ConvertUtil.InchToPoint(0.5)));
            Assert.That(ps.TextColumns.Width, Is.EqualTo(ConvertUtil.InchToPoint(8.5 - 1.25 * 2)));
            Assert.That(ps.TextColumns.Count, Is.EqualTo(1));
            Assert.That(ps.TextColumns.LineBetween, Is.EqualTo(false));

            Assert.That(ps.RtlGutter, Is.EqualTo(false));
            Assert.That(ps.Unlocked, Is.EqualTo(false));

            Assert.That(ps.Borders[BorderType.Left].LineStyle, Is.EqualTo(LineStyle.None));
            Assert.That(ps.BorderAppliesTo, Is.EqualTo(PageBorderAppliesTo.AllPages));
            Assert.That(ps.BorderDistanceFrom, Is.EqualTo(PageBorderDistanceFrom.Text));
            Assert.That(ps.BorderAlwaysInFront, Is.EqualTo(true));

            Assert.That(ps.CharSpace, Is.EqualTo(0));
            Assert.That(ps.LinePitch, Is.EqualTo(0d));
            Assert.That(ps.LayoutMode, Is.EqualTo(SectionLayoutMode.Default));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestMinOneColumn()
        {
            ISectionAttrSource sectionAttrSource = new SectPr();
            PageSetup ps = new PageSetup(sectionAttrSource, new DocPr());
            TextColumnCollection cs = ps.TextColumns;
            cs.SetCount(0);
        }

        [Test]
        public void TestDefaultColumnsNotChanged()
        {
            //Modify columns of a section.
            ISectionAttrSource sectionAttrSource1 = new SectPr();
            PageSetup ps1 = new PageSetup(sectionAttrSource1, new DocPr());
            TextColumnCollection cs1 = ps1.TextColumns;
            cs1.SetCount(2);
            cs1[0].Width = 100;

            //Just make sure we didn't modify the default attributes.
            ISectionAttrSource sectionAttrSource2 = new SectPr();
            PageSetup ps2 = new PageSetup(sectionAttrSource2, new DocPr());
            TextColumnCollection cs2 = ps2.TextColumns;
            Assert.That(cs2.Count, Is.EqualTo(1));
            Assert.That(cs2[0].Width, Is.EqualTo(0d));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestChangeColumns(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            TextColumnCollection cs = doc.FirstSection.PageSetup.TextColumns;
            
            //Initially the columns are evenly spaced.
            cs.SetCount(2);

            //Check that individual columns can be accessed in evenly spaced mode and can be changed.
            Assert.That(cs.Count, Is.EqualTo(2));
            Assert.That(cs[0].Width, Is.EqualTo(0d));
            Assert.That(cs[0].SpaceAfter, Is.EqualTo(0d));
            Assert.That(cs[1].Width, Is.EqualTo(0d));
            Assert.That(cs[1].SpaceAfter, Is.EqualTo(0d));

            //Set column widths.
            cs[0].Width = 100;
            cs[0].SpaceAfter = 20;
            cs[1].Width = 300;

            //Make sure the evenly spaced flag does not get automatically reset.
            Assert.That(cs.EvenlySpaced, Is.EqualTo(true));
            cs.EvenlySpaced = false;

            //Read and write and check the data.
            doc = TestUtil.SaveOpen(doc, @"Model\Page\TestChangeColumns", lf, sf);
            cs = doc.FirstSection.PageSetup.TextColumns;

            Assert.That(cs.Count, Is.EqualTo(2));
            Assert.That(cs.EvenlySpaced, Is.EqualTo(false));
            Assert.That(cs[0].Width, Is.EqualTo(100d));
            Assert.That(cs[0].SpaceAfter, Is.EqualTo(20d));
            Assert.That(cs[1].Width, Is.EqualTo(300d));
        }

        [Test]
        public void TestCloneColumns()
        {
            Document doc = new Document();

            //Modify columns in the first section.
            TextColumnCollection cs1 = doc.FirstSection.PageSetup.TextColumns;
            cs1.SetCount(2);
            cs1.LineBetween = true;
            cs1[0].Width = 100;
            cs1[0].SpaceAfter = 10;
            cs1[1].Width = 200;
            
            //Clone the section, second section should have same columns as the first now.
            doc.AppendChild(doc.FirstSection.Clone(true));

            //Modify first section.
            cs1.LineBetween = false;
            cs1.SetCount(1);
            cs1[0].Width = 999;

            //Check second section remains original.
            TextColumnCollection cs2 = doc.LastSection.PageSetup.TextColumns;
            Assert.That(cs1 != cs2, Is.True);
            Assert.That(cs1[0] != cs2[0], Is.True);
            Assert.That(cs2.Count, Is.EqualTo(2));
            Assert.That(cs2.LineBetween, Is.EqualTo(true));
            Assert.That(cs2[0].Width, Is.EqualTo(100d));
            Assert.That(cs2[0].SpaceAfter, Is.EqualTo(10d));
            Assert.That(cs2[1].Width, Is.EqualTo(200d));
        }

        [Test]
        public void TestDefaultBordersNotChanged()
        {
            //Modify borders one a section.
            ISectionAttrSource sectionAttrSource1 = new SectPr();
            PageSetup ps1 = new PageSetup(sectionAttrSource1, new DocPr());
            ps1.Borders[BorderType.Left].LineStyle = LineStyle.Double;

            //Make sure that when we modify page border, it modified the border of the section, not the default border.
            ISectionAttrSource sectionAttrSource2 = new SectPr();
            PageSetup ps2 = new PageSetup(sectionAttrSource2, new DocPr());
            Assert.That(ps2.Borders[BorderType.Left].LineStyle, Is.EqualTo(LineStyle.None));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestChangeBorders(LoadFormat lf, SaveFormat sf)
        {
            // TODO 1 DV Implement reading page borders.
            if (lf == LoadFormat.Rtf)
                return;

            Document doc = new Document();
            PageSetup ps = doc.FirstSection.PageSetup;
            
            //Modify borders
            ps.Borders[BorderType.Left].LineStyle = LineStyle.Double;

            //Read and write
            doc = TestUtil.SaveOpen(doc, @"Model\Page\TestChangeBorders", lf, sf);
            ps = doc.FirstSection.PageSetup;

            //Check the borders
            Assert.That(ps.Borders[BorderType.Left].LineStyle, Is.EqualTo(LineStyle.Double));
        }

        [Test]
        public void TestCloneBorders()
        {
            Document doc = new Document();

            //Modify borders in the first section.
            BorderCollection b1 = doc.FirstSection.PageSetup.Borders;
            //Top will be an explicit border.
            b1[BorderType.Top].LineWidth = 4;    
            //Left will be an inherited border, inherited borders should not be cloned at all.
            Border left1 = b1[BorderType.Left];    
            
            //Clone the section, second section should now have same borders as the first.
            doc.AppendChild(doc.FirstSection.Clone(true));

            //Modify borders in the first section again.
            b1[BorderType.Top].LineWidth = 10;

            //Check second section borders remain original.
            BorderCollection b2 = doc.LastSection.PageSetup.Borders;
            Assert.That(b1 != b2, Is.True);
            Assert.That(b1[BorderType.Top] != b2[BorderType.Top], Is.True);
            Assert.That(b2[BorderType.Top].LineWidth, Is.EqualTo(4d));

            //This is actually a new inherited border created for section 2.
            Border left2 = b2[BorderType.Left];
            Assert.That(left1 != left2, Is.True);

            //Just a sanity check that modifying one inherited border does not affect the other.
            left1.LineStyle = LineStyle.Dot;
            Assert.That(left2.LineStyle, Is.EqualTo(LineStyle.None));
        }

        /// <summary>
        /// Tests the <see cref="FootnoteOptions.Columns"/> property.
        /// </summary>
        [Test]
        public void TestFootnoteColumns()
        {
            Document doc = new Document();

            doc.FirstSection.SectPr.FootnoteColumns = 2;
            Assert.That(doc.FirstSection.PageSetup.FootnoteOptions.Columns, Is.EqualTo(2));
            doc.FirstSection.PageSetup.FootnoteOptions.Columns = 0;
            Assert.That(doc.FirstSection.SectPr.FootnoteColumns, Is.EqualTo(0));

            doc.FirstSection.SectPr.FootnoteColumns = 2;
            // FootnoteColumns are not written in ECMA-376.
            doc.ComplianceInfo = new OoxmlComplianceInfo();
            doc.ComplianceInfo.MarkAsIsoTransitional();

            doc = TestUtil.SaveOpen(doc, @"Model\Footnote\TestFootnoteColumns", UnifiedScenario.Docx2DocxNoGold);

            Assert.That(doc.FirstSection.PageSetup.FootnoteOptions.Columns, Is.EqualTo(2));
            Assert.That(doc.FirstSection.SectPr.FootnoteColumns, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that values of the <see cref="PageSetup.CharactersPerLine"/> and <see cref="PageSetup.LinesPerPage"/>
        /// properties are calculated correctly.
        /// </summary>
        [Test]
        public void TestDocumentGridCalculation()
        {
            Document doc = TestUtil.Open(@"Model\Page\TestDocumentGrid.docx");
            SectionCollection sections = doc.Sections;

            CheckDocumentGrid(sections[0], SectionLayoutMode.LineGrid, 32, 28);
            CheckDocumentGrid(sections[1], SectionLayoutMode.Grid, 20, 24);
            CheckDocumentGrid(sections[2], SectionLayoutMode.SnapToChars, 30, 20);
            CheckDocumentGrid(sections[3], SectionLayoutMode.Grid, 7, 32);
            CheckDocumentGrid(sections[4], SectionLayoutMode.Grid, 7, 32);
        }

        /// <summary>
        /// Tests setters of the <see cref="PageSetup.CharactersPerLine"/> and <see cref="PageSetup.LinesPerPage"/>
        /// properties.
        /// </summary>
        [Test]
        public void TestSettingDocumentGridOptions()
        {
            Document doc = new Document();

            PageSetup pageSetup = doc.FirstSection.PageSetup;
            pageSetup.LayoutMode = SectionLayoutMode.Grid;
            pageSetup.CharactersPerLine = 32;
            pageSetup.LinesPerPage = 28;
            CheckDocumentGrid(doc.FirstSection, SectionLayoutMode.Grid, 32, 28);

            pageSetup.TextOrientation = TextOrientation.Downward;
            pageSetup.CharactersPerLine = 32;
            pageSetup.LinesPerPage = 25;
            CheckDocumentGrid(doc.FirstSection, SectionLayoutMode.Grid, 32, 25);
        }

        /// <summary>
        /// Checks document grid options of the specified section.
        /// </summary>
        private static void CheckDocumentGrid(Section section, SectionLayoutMode expectedGridType,
            int expectedCharsPerLine, int expectedLinesPerPage)
        {
            PageSetup pageSetup = section.PageSetup;
            Assert.That(pageSetup.LayoutMode, Is.EqualTo(expectedGridType));
            Assert.That(pageSetup.CharactersPerLine, Is.EqualTo(expectedCharsPerLine));
            Assert.That(pageSetup.LinesPerPage, Is.EqualTo(expectedLinesPerPage));
        }

        /// <summary>
        /// WORDSNET-16349 The PageSetup.ContentXXX properties are changed to use 
        /// the <see cref="PageMarginCalculator"/> object that performs more correct calculation 
        /// of margin size including gutter.
        /// </summary>
        [Test]
        public void TestMarginCalculation()
        {
            Document doc = TestUtil.Open(@"Model\Page\TestMarginCalculation.docx");
            CheckContent(doc.FirstSection.PageSetup, 56.7, 85.05, 113.4, 198.45);
            CheckContent(doc.LastSection.PageSetup, 56.7, 56.7, 198.45, 113.4);

            // Check shape coordinates that are calculated as percent of content size.
            Shape shape = doc.FirstSection.Body.Shapes[0];
            const double delta = 0.001d;
            Assert.That(shape.Left, Is.EqualTo(51.03).Within(delta));
            Assert.That(shape.Top, Is.EqualTo(102.06).Within(delta));
        }

        /// <summary>
        /// WORDSNET-19250Issues with PaperSize.Ledger and PaperSize.Quarto.
        /// Paper sizes are incorrect. Corrected values for paper sizes.
        /// </summary>
        [TestCase(PaperSize.Ledger, 1224.0f, 792.0f)]
        [TestCase(PaperSize.Quarto, 609.849976f, 779.75f)]
        [TestCase(PaperSize.Folio, 612.0f, 936.0f)]
        public void Test19250(PaperSize paperSize, float expectedWidth, float expectedHeight)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.PageSetup.PaperSize = paperSize;
            
            Assert.That(builder.PageSetup.PageSize.Width, Is.EqualTo(expectedWidth));
            Assert.That(builder.PageSetup.PageSize.Height, Is.EqualTo(expectedHeight));
        }

        /// <summary>
        /// WORDSNET-20656 A generic error occurred in GDI+ when converting DOCX to PNG. 
        /// Incorrect (bigger than Word's limit) page sizes now truncated on validation.
        /// </summary>
        [Test]
        public void Test20656()
        {
            Document doc = TestUtil.Open(@"Model\Page\Test20656.docx");

            using (MemoryStream ms = new MemoryStream())
            {
                doc.Save(ms, SaveFormat.Docx);   // FOSS: was Png; the page-size truncation runs on document validation.
            }

            const double maximumSize = 3052.8;

            Assert.That(doc.FirstSection.PageSetup.PageWidth, Is.EqualTo(maximumSize));
            Assert.That(doc.FirstSection.PageSetup.PageHeight, Is.EqualTo(maximumSize));
        }

        /// <summary>
        /// Checks ContentXXX properties of the specified page setup object.
        /// </summary>
        private void CheckContent(PageSetup pageSetup, double expectedLeft, 
            double expectedTop, double expectedRight, double expectedBottom)
        {
            const double delta = 0.001d;
            Assert.That(pageSetup.ContentLeft, Is.EqualTo(expectedLeft).Within(delta));
            Assert.That(pageSetup.ContentTop, Is.EqualTo(expectedTop).Within(delta));
            Assert.That(pageSetup.ContentRight, Is.EqualTo(expectedRight).Within(delta));
            Assert.That(pageSetup.ContentBottom, Is.EqualTo(expectedBottom).Within(delta));
        }

        /// <summary>
        /// WORDSNET-870 TextColumns.SetCount does not work when setting count lesser than current.
        /// Forum thread: 'textcolumns count changes not working'
        /// https://www.aspose.com/Community/Forums/44376/ShowPost.aspx
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect870(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder builder = new DocumentBuilder();
        
            builder.Font.Size = 36;
    
            // write text to first section of document
            builder.Write("This text should be in 1 column.");

            // insert new section
            builder.InsertBreak(BreakType.SectionBreakContinuous);

            // set number of columns for this section to 3
            builder.PageSetup.TextColumns.SetCount(3);

            // write text to 3-column section
            builder.Write("This text should be in 3 columns.");
            
            // insert new section
            builder.InsertBreak(BreakType.SectionBreakContinuous);

            // set number of columns for this section to 1
            builder.PageSetup.TextColumns.SetCount(1);

            // write text
            builder.Write("This text shoudl be in 1 column.");

            TestUtil.SaveOpen(builder.Document, @"Model\Page\TestDefect870", lf, sf);

            // check the actual number of columns for the last section
            Assert.That(builder.PageSetup.TextColumns.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-7689 Support Book Fold multiple page setup.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira7689(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            PageSetup ps = doc.FirstSection.PageSetup;

            // Check defaults.
            Assert.That(ps.MultiplePages, Is.EqualTo(MultiplePagesType.Default));
            Assert.That(ps.SheetsPerBooklet, Is.EqualTo(0));

            // Set BookFoldPrinting and SheetsPerBooklet.
            ps.MultiplePages = MultiplePagesType.BookFoldPrinting;
            ps.SheetsPerBooklet = 4;
            doc = TestUtil.SaveOpen(doc, @"Model\Page\TestJira7689A", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            ps = doc.FirstSection.PageSetup;
            Assert.That(ps.MultiplePages, Is.EqualTo(MultiplePagesType.BookFoldPrinting));
            Assert.That(ps.SheetsPerBooklet, Is.EqualTo(4));

            // Set BookFoldPrintingReverse.
            ps.MultiplePages = MultiplePagesType.BookFoldPrintingReverse;
            ps.SheetsPerBooklet = 6;
            doc = TestUtil.SaveOpen(doc, @"Model\Page\TestJira7689B", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            ps = doc.FirstSection.PageSetup;
            Assert.That(ps.MultiplePages, Is.EqualTo(MultiplePagesType.BookFoldPrintingReverse));
            Assert.That(ps.SheetsPerBooklet, Is.EqualTo(6));

            // Set MirrorMargins, in this case SheetsPerBooklet should not be written.
            ps.MultiplePages = MultiplePagesType.MirrorMargins;
            ps.SheetsPerBooklet = 6;
            doc = TestUtil.SaveOpen(doc, @"Model\Page\TestJira7689C", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            ps = doc.FirstSection.PageSetup;
            Assert.That(ps.MultiplePages, Is.EqualTo(MultiplePagesType.MirrorMargins));
            Assert.That(ps.SheetsPerBooklet, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-10869 Introduced page number formatting options.
        /// </summary>
        [Test]
        public void Test10869()
        {
            Document document = new Document();

            PageSetup pageSetup = document.FirstSection.PageSetup;
            pageSetup.PageNumberStyle = NumberStyle.UppercaseRoman;
            pageSetup.ChapterPageSeparator = ChapterPageSeparator.Colon;
            pageSetup.HeadingLevelForChapter = 1;

            document = TestUtil.SaveOpen(document, @"Model\Page\Test10869.docx");

            pageSetup = document.FirstSection.PageSetup;
            Assert.That(pageSetup.PageNumberStyle, Is.EqualTo(NumberStyle.UppercaseRoman));
            Assert.That(pageSetup.ChapterPageSeparator, Is.EqualTo(ChapterPageSeparator.Colon));
            Assert.That(pageSetup.HeadingLevelForChapter, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-23931 Margins property added.
        /// Checks obtaining preset Margins from PageSetup.
        /// </summary>
        [Test]
        public void TestGetMargins()
        {
            SectPr.TestMode = false;
            string currentCulture = SystemPal.GetCurrentCultureName();
            SystemPal.SetCulture("en-US");

            Document doc = TestUtil.Open(@"Model\Page\TestMargins.docx");

            Assert.That(doc.Sections[0].PageSetup.Margins, Is.EqualTo(Margins.Normal));
            Assert.That(doc.Sections[1].PageSetup.Margins, Is.EqualTo(Margins.Narrow));
            Assert.That(doc.Sections[2].PageSetup.Margins, Is.EqualTo(Margins.Moderate));
            Assert.That(doc.Sections[3].PageSetup.Margins, Is.EqualTo(Margins.Wide));
            Assert.That(doc.Sections[4].PageSetup.Margins, Is.EqualTo(Margins.Mirrored));
            Assert.That(doc.Sections[5].PageSetup.Margins, Is.EqualTo(Margins.Custom));

            SystemPal.SetCulture(currentCulture);
            SectPr.TestMode = true;
        }

        /// <summary>
        /// WORDSNET-23931 Margins property added.
        /// Checks applying preset Margins to PageSetup.
        /// </summary>
        [TestCase(Margins.Normal, 1440, 1440, 1440, 1440)]
        [TestCase(Margins.Narrow, 720, 720, 720, 720)]
        [TestCase(Margins.Moderate, 1080, 1080, 1440, 1440)]
        [TestCase(Margins.Wide, 2880, 2880, 1440, 1440)]
        [TestCase(Margins.Mirrored, 1800, 1440, 1440, 1440)]
        public void TestSetMargins(Margins margins, int left, int right, int top, int bottom)
        {
            SectPr.TestMode = false;
            string currentCulture = SystemPal.GetCurrentCultureName();
            SystemPal.SetCulture("en-US");

            Document doc = new Document();

            doc.FirstSection.PageSetup.Margins = margins;
            Assert.That(doc.FirstSection.SectPr.LeftMargin, Is.EqualTo(left));
            Assert.That(doc.FirstSection.SectPr.RightMargin, Is.EqualTo(right));
            Assert.That(doc.FirstSection.SectPr.TopMargin, Is.EqualTo(top));
            Assert.That(doc.FirstSection.SectPr.BottomMargin, Is.EqualTo(bottom));
            if (margins == Margins.Mirrored)
                Assert.That(doc.DocPr.MultiplePages, Is.EqualTo(MultiplePagesType.MirrorMargins));
            SystemPal.SetCulture(currentCulture);
            SectPr.TestMode = true;
        }

        // FOSS: Test22749 removed — it inferred PaperSize from EditingLanguage while loading an HTML file
        // (a format with no page setup); HTML load was removed, so the inference scenario can't be set up.

        /// <summary>
        /// WORDSNET-27516 Add JISB4, JISB5 sizes to PaperSize.
        /// Testing new paper sizes.
        /// </summary>
        [TestCase(PaperSize.JisB4, 14570, 20636)]
        [TestCase(PaperSize.JisB5, 10318, 14570)]
        public void Test27516(PaperSize paperSize, int expectedWidth, int expectedHeight)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.PageSetup.PaperSize = paperSize;

            Section sect = builder.Document.FirstSection;
            Assert.That(sect.SectPr[SectAttr.PageWidth], Is.EqualTo(expectedWidth));
            Assert.That(sect.SectPr[SectAttr.PageHeight], Is.EqualTo(expectedHeight));
            Assert.That(sect.PageSetup.PaperSize, Is.EqualTo(paperSize));
        }
    }
}
