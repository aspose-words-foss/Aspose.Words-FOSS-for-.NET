// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using System.IO;
using Aspose.Common;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Tables;
using NUnit.Framework;
using List = Aspose.Words.Lists.List;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests index and tables fields.
    /// </summary>
    [TestFixture]
    public class TestIndexAndTables : TestFieldsBase
    {
        /// <summary>
        /// Tests how the TOC field is updated.
        ///
        /// JAVAGOLD PNG are encoded differently on Java.
        /// </summary>
        [Test]
        public void TestToc()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\IndexAndTables\TestToc.docx");
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestToc Modified.docx");
        }

        /// <summary>
        /// Tests how the TOC field with no entries in the document is updated.
        /// </summary>
        [Test]
        public void TestTocNoEntries()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\IndexAndTables\TestTocNoEntries.docx");
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocNoEntries Modified.docx");
        }

        /// <summary>
        /// Tests how TOC fields residing in a paragraph with a Heading style are updated.
        /// </summary>
        [Test]
        public void TestTocHeadingStyle()
        {
            Document doc = OpenSaveOpenUpdateFields(@"Fields\IndexAndTables\TestTocHeadingStyle.docx");
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocHeadingStyle Modified.docx");
        }

        /// <summary>
        /// WORDSNET-16258 "No table of contents entries found" text is shown instead of TOC after updating fields.
        /// </summary>
        [Test]
        public void TestDefect16258()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect16258.docx");

            // Set some weird culture with ";" as a list separator.
            SystemPal.SetCulture("de-De");
            VerifyExpectedListSeparator(';');

            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect16258 Modified.docx");
        }

        /// <summary>
        /// WORDSNET-15752 Aspose.Words hangs if insert TOC into heading paragraph.
        /// </summary>
        [Test]
        public void TestDefect15752()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Document.UpdateFields();
            // Nothing to assert, just should not hang here.
        }

        /// <summary>
        /// WORDSNET-15774 Unexpected behavior upon updating TOC if heading paragraphs contains pagebreaks.
        /// </summary>
        [Test]
        public void TestDefect15774()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect15774.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect15774 Modified.docx");
        }







        /// <summary>
        /// WORDSNET-19076 Table of Content looks incorrect.
        /// </summary>
        [Test]
        public void TestDefect19076()
        {
            Document dstDoc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect19076_dst.dotx");
            Document srcDoc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect19076_src.dotx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);
            dstDoc.UpdateFields();

            TestUtil.SaveOpenDocxExportOnly(dstDoc, @"Fields\IndexAndTables\TestDefect19076 Modified.docx");
        }

        /// <summary>
        /// WORDSNET-24890 Column and page breaks appear inside TOC
        /// </summary>
        [Test]
        public void TestDefect24890TocAndColumnBreaks()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect24890TocAndColumnBreaks.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect24890TocAndColumnBreaks Modified.docx");
        }

        /// <summary>
        /// WORDSNET-24890 Column and page breaks appear inside TOC
        /// </summary>
        [Test]
        public void TestDefect24890TocAndPageBreaks()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect24890TocAndPageBreaks.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect24890TocAndPageBreaks Modified.docx");
        }



        /// <summary>
        /// WORDSNET-16998 Extra items have been added to TOC after UpdateFields.
        /// </summary>
        [Test]
        public void TestDefect16998ExtraEntriesInToc()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect16998ExtraEntriesInToc.docx");

            // Set a culture with ',' as a list separator.
            SystemPal.SetCulture("en-nz");
            VerifyExpectedListSeparator(',');

            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect16998ExtraEntriesInToc Modified.docx");
        }

        private static void VerifyExpectedListSeparator(char expectedSeparator)
        {
            Assert.That(FormatterPal.GetListSeparatorCurrent(), Is.EqualTo(expectedSeparator), string.Format(                "This test must get {0} as a list separator from MiscUtil.GetListSeparator() in order to work!",                 expectedSeparator));
        }

        /// <summary>
        /// WORDSNET-16998 Extra items have been added to TOC after UpdateFields.
        /// </summary>
        [Test]
        public void TestDefect16998CustomListSeparator()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect16998ExtraEntriesInToc.docx");

            // Set some weird culture with ";" as a list separator.
            SystemPal.SetCulture("ru-Ru");
            VerifyExpectedListSeparator(';');

            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect16998ListSeparator Modified.docx");
        }

        /// <summary>
        /// WORDSNET-16998 \\t switch cancels \\u switch
        /// </summary>
        [Test]
        public void TestDefect16998TswitchCancelsUswitch()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect16998TswitchCancelsUswitch.docx");

            // Set a culture with ',' as a list separator.
            SystemPal.SetCulture("en-nz");
            VerifyExpectedListSeparator(',');

            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect16998TswitchCancelsUswitch Modified.docx");
        }

        /// <summary>
        /// WORDSNET-26975 Font formatting of TOC is lost after updating fields.
        /// </summary>
        [Test]
        [TestCase(@"Fields\IndexAndTables\TestDefect26975TocHSwitchVsDirectFormatting.docx")]
        [TestCase(@"Fields\IndexAndTables\TestDefect26975TocNoHSwitchVsDirectFormatting.docx")]
        public void TestDefect26975Body(string fileName)
        {
            Document doc = TestUtil.Open(fileName);

            doc.UpdateFields();

            TestUtil.SaveOpenDocxExportOnly(doc, fileName.Replace(".docx", " Modified.docx"));
        }

        /// <summary>
        /// WORDSNET-26975 Checks different font attributes when Hyperlink style is changed.
        /// </summary>
        [Test]
        public void TestDefect26975HyperlinkStyleChanged()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect26975HyperlinkStyleChanged.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect26975HyperlinkStyleChanged Modified.docx");
        }

        /// <summary>
        /// WORDSNET-26975 Checks cases when character style is applied to the TOC entries.
        /// </summary>
        [Test]
        public void TestDefect26975HyperLinkStyleHasNoColor()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect26975HyperLinkStyleHasNoColor.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect26975HyperLinkStyleHasNoColor Modified.docx");
        }

        /// <summary>
        /// WORDSNET-26975 Checks cases when character style is applied to the TOC entries.
        /// </summary>
        [Test]
        public void TestDefect26975CharStyle()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect26975CharStyle.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect26975CharStyle Modified.docx");
        }

        /// <summary>
        /// WORDSNET-26975 Checks more cases when character style is applied to the TOC entries.
        /// </summary>
        [Test]
        public void TestDefect26975CharStyleAttributes()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect26975CharStyleAttributes.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect26975CharStyleAttributes Modified.docx");
        }

        /// <summary>
        /// WORDSNET-26975 Checks more cases when character style is applied to the TOC entries.
        /// </summary>
        [Test]
        public void TestDefect26975CharStyleInherited()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect26975CharStyleInherited.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect26975CharStyleInherited Modified.docx");
        }


        /// <summary>
        /// WORDSNET-3786 Page numbers lost in TOC when multiple \\n switches are present.
        /// </summary>
        [Test]
        public void TestDefect3786TocSwitchOrder()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect3786TocSwitchOrder.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect3786TocSwitchOrder Modified.docx");
        }

        /// <summary>
        /// Tests how table of figures is updated.
        /// </summary>
        [Test]
        public void TestTableOfFigures()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTableOfFigures.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTableOfFigures Modified.docx");
        }

        [Test]
        public void TestCaptionlessTableOfFiguresWithTrailingSeqField()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestCaptionlessTableOfFiguresWithTrailingSeqField.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestCaptionlessTableOfFiguresWithTrailingSeqField.docx");
        }

        /// <summary>
        /// Tests how table of figures without entries is updated.
        /// </summary>
        [Test]
        public void TestTableOfFiguresNoEntries()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTableOfFiguresNoEntries.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTableOfFiguresNoEntries Modified.docx");
        }

        /// <summary>
        /// WORDSNET-3511 Support TOC of bookmarked content.
        /// </summary>
        [Test]
        public void TestDefect3511TocBookmarkedRange()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect3511TocBookmarkedRange.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect3511TocBookmarkedRange Modified.docx");
        }

        /// <summary>
        /// WORDSNET-3511 Support TOC of bookmarked content.
        /// </summary>
        [Test]
        public void TestDefect3511TocBookmarkedRangeStart()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect3511TocBookmarkedRangeStart.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect3511TocBookmarkedRangeStart Modified.docx");
        }

        /// <summary>
        /// WORDSNET-3511 Support TOC of bookmarked content.
        /// </summary>
        [Test]
        public void TestDefect3511TocBookmarkedRangeSpecial()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect3511TocBookmarkedRangeSpecial.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect3511TocBookmarkedRangeSpecial Modified.docx");
        }


        /// <summary>
        /// WORDSNET-5009 NullReferenceException occurs during UpdateFields.
        /// </summary>
        [Test]
        public void TestDefect5009NullReference()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect5009NullReference.docx");
            doc.UpdateFields();
        }

        /// <summary>
        /// WORDSNET-23793 Quotation marks are missed from TOC entries.
        /// All TOC entries are unescaped during copying to TOC while only TC argument should be.
        /// </summary>
        [Test]
        public void TestDefect23793()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert TOC.
            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");
            builder.Writeln();

            // Insert Heading pagegraph with quotation marks.
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Write("Test \"content\" bla bla");

            // Update TOC.
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestDefect23793.docx");
        }


        /// <summary>
        /// WORDSNET-5091 Extra item in TOC.
        /// </summary>
        [Test]
        public void TestDefect5091TocExtraItem()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect5091TocExtraItem.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestDefect5091TocExtraItem Modified.docx");
        }


        /// <summary>
        /// Tests how tab position is calculated for a normal TOC.
        /// </summary>
        [Test]
        public void TestTocTabPositionNormal()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocTabPositionNormal.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocTabPositionNormal Modified.docx");
        }

        /// <summary>
        /// Tests how tab position is calculated for a TOC with a wide entry's label.
        /// </summary>
        [Test]
        public void TestTocTabPositionWideLabel()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocTabPositionWideLabel.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocTabPositionWideLabel Modified.docx");
        }

        /// <summary>
        /// Tests how tab position is calculated for a TOC based on a modified Normal style with a large font size.
        /// </summary>
        [Test]
        public void TestTocTabPositionLargeFontSize()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocTabPositionLargeFontSize.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocTabPositionLargeFontSize Modified.docx");
        }

        /// <summary>
        /// Tests how tab position is calculated for a TOC that has different font size for each entry.
        /// </summary>
        [Test]
        public void TestTocTabPositionDifferentFontSize()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocTabPositionDifferentFontSize.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocTabPositionDifferentFontSize Modified.docx");
        }

        /// <summary>
        /// Tests how tab stop and paragraph break font is set in a document having a theme.
        /// </summary>
        [Test]
        public void TestTocEntryFontWithTheme()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocEntryFontWithTheme.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocEntryFontWithTheme Modified.docx");
        }

        /// <summary>
        /// Tests how tab stop and paragraph break font is set in a document having no theme.
        /// </summary>
        [Test]
        public void TestTocEntryFontWithoutTheme()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocEntryFontWithoutTheme.docx");
            doc.UpdateFields();
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestTocEntryFontWithoutTheme Modified.docx");
        }




        /// <summary>
        /// WORDSNET-6150 Calling UpdateFields twice corrupts TOС/SEQ fields.
        /// </summary>
        [Test]
        public void TestJira6150()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertField(@"SEQ Table \* ARABIC \* MERGEFORMAT");
            builder.Writeln();
            builder.InsertTableOfContents(@"\h \z \c ""Table""");
            builder.Document.UpdateFields();
            builder.Document.UpdateFields();

            TestUtil.SaveOpenDocxExportOnly(builder.Document, @"Fields\IndexAndTables\TestJira6150 Modified.docx");
        }

        /// <summary>
        /// WORDSNET-5497 Code of XE field is shown in TOC after updating fields.
        /// </summary>
        [Test]
        public void TestJira5497()
        {
            Document doc = TestUtil.OpenUpdateFields(@"Fields\IndexAndTables\TestJira5497.docx");
            TestUtil.SaveOpenDocxExportOnly(doc, @"Fields\IndexAndTables\TestJira5497 Modified.docx");
        }


        /// <summary>
        /// The initial test bundle for INDEX field update.
        /// </summary>
        /// <remarks>
        /// Each of the used input documents contains description in its beginning.
        /// </remarks>
        [Test]
        [TestCase("TestFieldIndexColumns.docx", false)]
        [TestCase("TestFieldIndexColumnsUpdated.docx", false)]
        [TestCase("TestFieldIndexConstraints.docx", false)]
        [TestCase("TestFieldIndexEntrySort.docx", false)]
        [TestCase("TestFieldIndexFontAttrs.docx", false)]
        [TestCase("TestFieldIndexForbiddenPlacements.docx", false)]
        [TestCase("TestFieldIndexMisc.docx", false)]
        [TestCase("TestFieldIndexPageFormats.docx", false)]
        [TestCase("TestFieldIndexPageNumberMerge.docx", false)]
        [TestCase("TestFieldIndexRtl.docx", true)]
        [TestCase("TestFieldIndexSeparators.docx", false)]
        public void TestIndexGeneral(string fileName, bool useBidiText)
        {
            string filePath = Path.Combine(@"Fields\IndexAndTables\FieldIndexGeneral", fileName);

            Document doc = TestUtil.Open(filePath);

            doc.FieldOptions.IsBidiTextSupportedOnUpdate = useBidiText;
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, filePath);
        }

        /// <summary>
        /// Part of TestIndexGeneral test. If this document is included to TestCase of TestIndexGeneral test it fails.
        /// So this separate test was created to avoid this problem on Jekins.
        /// </summary>
        [Test]
        public void TestIndexGeneralForJenkins()
        {
            const string filePath = @"Fields\IndexAndTables\FieldIndexGeneral\TestFieldIndexSubentries.docx";

            Document doc = TestUtil.Open(filePath);

            doc.FieldOptions.IsBidiTextSupportedOnUpdate = false;
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, filePath);
        }


        /// <summary>
        /// Tests how tab stop positions are calculated while updating of INDEX and TOC fields contained in sections
        /// with multiple columns.
        /// </summary>
        [Test]
        public void TestIndexTocMultipleColumns()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestIndexTocMultipleColumns.docx");
            doc.UpdateFields();
            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestIndexTocMultipleColumns.docx");
        }

        /// <summary>
        /// WORDSNET-9466 custom style separator in \t switch
        /// </summary>
        [Test]
        public void TestDefect9466()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect9466.docx");
            doc.FieldOptions.CustomTocStyleSeparator = "#";
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestDefect9466.docx");
        }


        /// <summary>
        /// WORDSNET-10280 update only page numbers for TOC.
        /// </summary>
        [Test]
        public void TestDefect10280_1()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect10280_1.docx");

            FieldStart fieldStart = (FieldStart)doc.GetChildNodes(NodeType.FieldStart, true)[0];
            FieldToc tocField = (FieldToc)fieldStart.GetField();

            Assert.That(tocField.UpdatePageNumbers(), Is.True);
            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestDefect10280_1.docx");
        }

        /// <summary>
        /// WORDSNET-10280 UpdatePageNumbers should return false, if one of related TOC bookmarks removed.
        /// </summary>
        [Test]
        public void TestDefect10280_2()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect10280_2.docx");

            FieldStart fieldStart = (FieldStart)doc.GetChildNodes(NodeType.FieldStart, true)[0];
            FieldToc tocField = (FieldToc)fieldStart.GetField();

            Assert.That(tocField.UpdatePageNumbers(), Is.False);
            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestDefect10280_2.docx");
        }

        /// <summary>
        /// WORDSNET-10465 TOC field ("Überschrift 1;1;Überschrift 2;2;Überschrift 3;3") does not work for German language
        /// </summary>
        [Test]
        public void TestDefect10465()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect10465.docx");
            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestDefect10465.docx");
        }

        /// <summary>
        /// WORDSNET-10610 White spaces are lost when updating a TOC
        /// </summary>
        [Test]
        public void TestDefect10610()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestDefect10610.docx");
            doc.UpdateFields();

            // With trailing character
            Assert.That(GetTocEntryText(doc, "0.0.0"), Is.EqualTo("Book 1 Test 1\r"));
            Assert.That(GetTocEntryText(doc, "1.0.0"), Is.EqualTo("Chapter 1 Test 1.1\r"));
            Assert.That(GetTocEntryText(doc, "2.0.0"), Is.EqualTo("Article 1\tTest 1.1.1\r"));
            Assert.That(GetTocEntryText(doc, "3.0.0"), Is.EqualTo("Book 2 Test 2\r"));
            Assert.That(GetTocEntryText(doc, "4.0.0"), Is.EqualTo("Chapter 2 Test 2.2\r"));
            Assert.That(GetTocEntryText(doc, "5.0.0"), Is.EqualTo("Article 2\tTest 2.2.2\r"));

            // Without trailing character
            Assert.That(GetTocEntryText(doc, "6.0.0"), Is.EqualTo("Book 3\r"));
            Assert.That(GetTocEntryText(doc, "7.0.0"), Is.EqualTo("Chapter 3\r"));
            Assert.That(GetTocEntryText(doc, "8.0.0"), Is.EqualTo("Article 3\r"));

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestDefect10610.docx");
        }

        private static string GetTocEntryText(Document doc, string paraId)
        {
            Node para = doc.GetNodeById(paraId);
            return NodeTextCollector.GetText(para, true, para, true, true);
        }

        /// <summary>
        /// WORDSNET-11387 TOC displays "No table of contents entries found" message in genereted output
        /// </summary>
        [Test]
        public void TestJira11387()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestJira11387.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestJira11387.docx");
            // Dmatv: a gold with field value run attributes expanded accepted per WORDSNET-19791
            // See WORDSNET-24486 for the behavior explanation.
        }

        /// <summary>
        /// WORDSNET-11449 UpdateFields does not update TOC field values
        /// </summary>
        [Test]
        public void TestJira11449()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestJira11449.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertTableOfContents("\\o \"1-3\" \\h \\z \\u");

            doc.UpdateFields();

            // Dmatv: I've accepted a new gold with tblGrids calculated by the new algorithm.
            // However the values in the calculated grid may not match what MS Word saves for the test document.
            // This is because the document is missing page margins in the section properties.
            // MS Word behavior in this case depends on locale, or English 1 inch (72 points) is normally set.
            // AW imitates this behavior in regular mode, but defaults to 90 points in test mode.
            // The gold was accepted with the grid values calculated from 90pt margins.
            // The grids match MS Word save result perfectly if 90pt margins are set in the document via UI.
            int expectedMargin = 90 * 20;
            SectPr sectionPr = doc.FirstSection.SectPr;
            Assert.That(sectionPr.LeftMargin, Is.EqualTo(expectedMargin));
            Assert.That(sectionPr.RightMargin, Is.EqualTo(expectedMargin));

            // Dmatv one of the aut-fit grids becomes incorrect on reading/writing the export result without updating layout.
            // I did not analyze how model manages to do that. The grid becomes correct again with layout update.
            // I changed the test to verify export only.
            TestUtil.SaveCheckGoldExportOnly(doc, @"Fields\IndexAndTables\TestJira11449.docx");
        }

        /// <summary>
        /// WORDSNET-11271 TabStops, Left and Hanging indent values of TOC items are not preserved by UpdateFields.
        /// </summary>
        [Test]
        public void TestJira11271()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestJira11271.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestJira11271.docx");
        }

        /// <summary>
        /// Tests how the TOC field trims leading and trailing whitespaces.
        /// </summary>
        [Test]
        public void TestTocTrimLeadingAndTrailingWhitespaces()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocTrimLeadingAndTrailingWhitespaces.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestTocTrimLeadingAndTrailingWhitespaces.docx");
        }

        /// <summary>
        /// Tests how the TOC field is updated by using TC entries.
        /// </summary>
        [Test]
        public void TestTocWithTc()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocWithTC.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestTocWithTC.docx");
        }



        /// <summary>
        /// WORDSNET-12288 Document.UpdateFields does not update TOC field
        /// </summary>
        [Test]
        public void TestJira12288()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestJira12288.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestJira12288.docx");
        }


        /// <summary>
        /// Test how the TOC field updates tab leaders.
        /// </summary>
        [Test]
        public void TestTocTabLeader()
        {
            Document doc = TestUtil.Open(@"Fields\IndexAndTables\TestTocTabLeader.docx");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Fields\IndexAndTables\TestTocTabLeader.docx");
        }



        /// <summary>
        /// WORDSNET-12905 UpdateFields produces TOC that is different to Word.
        /// </summary>
        [Test]
        public void TestJira12905()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira12905.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira12905.docx");
        }


        /// <summary>
        /// WORDSNET-13268 Creating TOC for text with non-BodyText OutlineLevel works unexpected
        /// </summary>
        [Test]
        public void TestJira13268()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira13268.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira13268.docx");
        }

        /// <summary>
        /// WORDSNET-13419 Document.UpdateFields corrupts the document.
        /// </summary>
        [Test]
        public void TestJira13419()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira13419.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira13419.docx");
        }

        /// <summary>
        /// WORDSNET-13586 TOC update shows Error Bookmark not defined.
        /// </summary>
        [Test]
        public void TestJira13586()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira13586.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira13586.docx");
        }

        /// <summary>
        /// WORDSNET-13587 TOC update shows Error Bookmark not defined.
        /// </summary>
        [Test]
        public void TestJira13587()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira13587.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira13587.docx");
        }

        /// <summary>
        /// WORDSNET-11066 TOC is built incorrectly.
        /// </summary>
        [Test]
        public void TestJira11066()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira11066.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira11066.docx");
        }


        /// <summary>
        /// WORDSNET-14422 Document.UpdateFields updates the INDEX field incorrectly.
        /// </summary>
        [Test]
        public void TestJira14422()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira14422.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira14422.docx");
        }


        /// <summary>
        /// WORDSNET-14865 TOC with \b switch should not add entry for paragraph with bookmark start at the middle.
        /// </summary>
        [Test]
        public void TestJira14865()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira14865.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira14865.docx");
        }

        /// <summary>
        /// WORDSNET-14873 Switch \\u does not work, if TOC contains \\t switch.
        /// </summary>
        [Test]
        public void TestJira14873()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira14873.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira14873.docx");
        }

        /// <summary>
        /// WORDSNET-14911 Incorrect finding of entry start/finish.
        /// </summary>
        [Test]
        public void TestJira14911()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira14911.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira14911.docx");
        }


        /// <summary>
        /// WORDSNET-4930 Outline level should not be ignored when heading range is specified.
        /// </summary>
        [Test]
        public void TestJira4930OutlineLevelsWithHeadingRangeAndCustomStyles()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira4930OutlineLevelsWithHeadingRangeAndCustomStyles.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira4930OutlineLevelsWithHeadingRangeAndCustomStyles.docx");
        }

        /// <summary>
        /// Tests how the TOC field is updated with revisions.
        /// </summary>
        [Test]
        public void TestTocRevisions()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestTocRevisions.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestTocRevisions.docx");
        }

        [Test]
        public void TestTocEntriesFinalRevision()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestTocEntriesFinalRevision.xml");

            IList<ITocEntry> entries = TocEntryExtractor.ExtractTocEntries(document, false);

            Assert.That(entries.Count, Is.EqualTo(3));
            Assert.That(entries[0].Level, Is.EqualTo(2));
            Assert.That(entries[0].GetDocumentOutlineTitle(), Is.EqualTo("a)	Numbering was changed by style change."));
            Assert.That(entries[1].Level, Is.EqualTo(2));
            Assert.That(entries[1].GetDocumentOutlineTitle(), Is.EqualTo("b)	Just a list item but revised because above item is revised."));
            Assert.That(entries[2].Level, Is.EqualTo(1));
            Assert.That(entries[2].GetDocumentOutlineTitle(), Is.EqualTo("Numbering is removed by style change."));
        }

        /// <summary>
        /// WORDSNET-15052 Simplifiy OMath when copying it into toc entry.
        /// </summary>
        [Test]
        public void TestJira15052()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira15052.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira15052.docx");
        }

        [Test]
        public void TestToa()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToa.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToa.docx");
        }

        [Test]
        public void TestToaSectionResetNumbering()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToaSectionResetNumbering.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToaSectionResetNumbering.docx");
        }

        [Test]
        public void TestToaPassim()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToaPassim.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToaPassim.docx");
        }

        [Test]
        public void TestToaBoldItalic()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToaBoldItalic.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToaBoldItalic.docx");
        }

        [Test]
        public void TestToaPageRange()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToaPageRange.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToaPageRange.docx");
        }

        [Test]
        public void TestToaSequencedPageRange()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToaSequencedPageRange.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToaSequencedPageRange.docx");
        }

        [Test]
        public void TestToaBetweenText()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestToaBetweenText.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestToaBetweenText.docx");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void TestToaCategoriesOption(bool defaultOptionsCase)
        {
            DocumentBuilder builder = new DocumentBuilder();
            ToaCategories categories;
            if (defaultOptionsCase)
            {
                categories = ToaCategories.DefaultCategories;
            }
            else
            {
                builder.Document.FieldOptions.ToaCategories = new ToaCategories();
                categories = builder.Document.FieldOptions.ToaCategories;
            }

            try
            {
                builder.InsertField("TA \\c 1 \\l entry");
                builder.Writeln();
                Field field = builder.InsertField("TOA \\c 1 \\h", null);
                categories[1] = "Blah";

                field.Update();

                Assert.That(field.Result, Is.EqualTo("Blah\rentry\t0\r"));
            }
            finally
            {
                categories[1] = "Cases";
            }
        }


        [Test]
        public void TestTocCustomStyles()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TocCustomStyles.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TocCustomStyles.docx");
        }

        /// <summary>
        /// WORDSNET-15662 AW produces slightly different TOC than MS Word.
        /// </summary>
        [Test]
        public void TestJira15662()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira15662.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira15662.docx");
        }

        /// <summary>
        /// WORDSNET-15778 Document.UpdateFields throws System.NullReferenceException.
        /// </summary>
        [Test]
        public void TestJira15778()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveToDocumentStart();
            builder.InsertField("INDEX \\e \"\t\" \\c \"1\"");
            builder.InsertField("XE test");

            doc.UpdateFields();

            // There is no exception
        }



        /// <summary>
        ///  Checks that specified "Istd" and content were applied to the run.
        /// </summary>
        private static void CheckRunStyleAndContent(Run run, int istd, string text)
        {
            Assert.That(run.Text, Is.EqualTo(text));
            Assert.That(run.RunPr.Istd, Is.EqualTo(istd));
        }


        /// <summary>
        /// WORDSNET-17249 TOC isn't rendered properly in PDF output
        /// </summary>
        [Test]
        public void TestJira17249()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira17249.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira17249.docx");
        }


        /// <summary>
        /// WORDSNET-17663 UpdateFields method recreates bookmarks used in the ToC.
        /// </summary>
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(7)]
        public void TestJira17663(int updateCount)
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira17663.docx");

            AssertNotNullBookmarks(document,
                "_Toc529527385",
                "_Toc529527386",
                "_Toc529527387",
                "_Toc529527388",
                "_Toc529527389",
                "_Toc529527390",
                "_Toc529527391",
                "_Toc529527392",
                "_Toc529527393",
                "_Toc529527394",
                "_Toc529527395",
                "_Toc529527396",
                "_Toc529527397",
                "_Toc529527398",
                "_Toc529527399",
                "_Toc529527400",
                "_Toc529527401",
                "_Toc529527402");

            for (int i = 0; i < updateCount; i++)
                document.UpdateFields();

            Assert.That(document.Range.Bookmarks.Count, Is.EqualTo(19 + 3 * (updateCount - 1)));

            AssertNotNullBookmarks(document,
                "_Toc529527389",
                "_Toc529527392",
                "_Toc529527394",
                "_Toc529527395",
                "_Toc529527396",
                "_Toc529527398",
                "_Toc529527401");
            AssertNullBookmarks(document,
                "_Toc529527385",
                "_Toc529527386",
                "_Toc529527387",
                "_Toc529527388",
                "_Toc529527390",
                "_Toc529527391",
                "_Toc529527393",
                "_Toc529527397",
                "_Toc529527399",
                "_Toc529527400",
                "_Toc529527402");
        }

        private static void AssertNotNullBookmarks(Document document, params string[] expected)
        {
            BookmarkCollection bookmarks = document.Range.Bookmarks;
            foreach (string bookmark in expected)
                Assert.That(bookmarks[bookmark], IsNot.Null());
        }

        private static void AssertNullBookmarks(Document document, params string[] expected)
        {
            BookmarkCollection bookmarks = document.Range.Bookmarks;
            foreach (string bookmark in expected)
                Assert.That(bookmarks[bookmark], Is.Null);
        }


        [Test]
        public void TestTocHiddenOutlineEntries()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestTocHiddenOutlineEntries.docx");
            Field field = document.Range.Fields[0];

            field.Update();

            Assert.That(field.Result, Is.EqualTo("No table of contents entries found."));
        }


        /// <summary>
        /// WORDSNET-17952 Document.UpdateFields throws System.NotSupportedException.
        /// </summary>
        [Test]
        public void TestJira17952()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestJira17952.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestJira17952.docx");
        }


        /// <summary>
        /// WORDSNET-18448 List of Tables adds continuation entries in PDF after calling UpdateFields.
        /// </summary>
        [Test]
        public void Test18448()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test18448.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test18448.docx");
        }


        /// <summary>
        /// WORDSNET-18715 ToC is indented differently by Aspose.Words vs MS Word.
        /// </summary>
        [Test]
        [TestCase(0)]
        [TestCase(60)]
        [TestCase(120)]
        public void Test18715(int hanging)
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test18715.docx");

            document.Styles[StyleIdentifier.TableOfFigures].ParagraphFormat.FirstLineIndent = -hanging;
            document.Styles[StyleIdentifier.TableOfFigures].ParagraphFormat.LeftIndent = hanging;
            document.Styles[StyleIdentifier.Toc1].ParagraphFormat.FirstLineIndent= -hanging;
            document.Styles[StyleIdentifier.Toc1].ParagraphFormat.LeftIndent= hanging;

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, string.Format(@"Fields\IndexAndTables\Test18715 {0}.docx", hanging));
        }


        /// <summary>
        /// WORDSNET-18988 Advance field is lost after calling Document.UpdateFields method.
        /// </summary>
        [Test]
        public void Test18988()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test18988.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test18988.docx");
        }


        /// <summary>
        /// WORDSNET-18084 TOC field sequence separator does not display properly until manual field update.
        /// </summary>
        [Test]
        public void Test18084()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test18084.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test18084.docx");
        }

        /// <summary>
        /// WORDSNET-19435 Document.UpdateFields updates the table of content incorrectly.
        /// </summary>
        [Test]
        public void Test19435()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test19435.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test19435.docx");
        }

        /// <summary>
        /// WORDSNET-20130 UpdateFields does not process TOC field.
        /// </summary>
        [Test]
        public void Test20130()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test20130.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test20130.docx");
        }

        private static string[] GetIndexEntries(Document document, int indexFieldIndex, params int[] subEntryIndexes)
        {
            FieldIndex field = (FieldIndex)FieldExtractor.ExtractToCollection(document, false, FieldType.FieldIndex)[indexFieldIndex];

            FieldSeqDataProvider provider = field.HasSequenceName ? new FieldSeqDataProvider(document) : null;
            IndexEntry entry = IndexEntry.GetRootEntry(new FieldCodeIndex(field), provider, document);

            foreach (int subEntryIndex in subEntryIndexes)
                entry = entry.GetSubentry(subEntryIndex);

            string[] entries = new string[entry.SubentryCount];
            for (int i = 0; i < entry.SubentryCount; i++)
                entries[i] = entry.GetSubentryText(i);

            return entries;
        }

        private static string[] GetTocEntries(Node document, int tocFieldIndex)
        {
            FieldToc field = (FieldToc)FieldExtractor.ExtractToCollection(document, false, FieldType.FieldTOC)[tocFieldIndex];
            return GetTocEntries(field);
        }

        private static string[] GetTocEntries(FieldToc field)
        {
            Document document = field.FetchDocument();

            document.UpdateListLabels();

            IList<ITocEntry> entries = TocEntryExtractor.ExtractTocEntries(document, field.ParseOptions());

            string[] result = new string[entries.Count];

            for (int i = 0; i < entries.Count; i++)
            {
                ITocEntry entry = entries[i];

                int level = entry.Level;
                NodeRange labelRange = entry.GetLabelRange();
                NodeRange titleRange = entry.InsertBookmark(string.Empty);

                result[i] = string.Format(
                    "{0}{1}{2}",
                    new string('\t', level - 1),
                    labelRange != null ? NodeTextCollector.GetText(labelRange) : string.Empty,
                    NodeTextCollector.GetText(titleRange));
            }

            return result;
        }

        private static string[] GetDocumentOutlineEntries(Document document, bool extractFromTables)
        {
            IList<ITocEntry> entries = TocEntryExtractor.ExtractTocEntries(document, extractFromTables);

            string[] result = new string[entries.Count];
            for (int i = 0; i < entries.Count; i++)
                result[i] = entries[i].GetDocumentOutlineTitle();

            return result;
        }

        private static string[] GetDocumentOutlineEntries(Document document)
        {
            return GetDocumentOutlineEntries(document, false);
        }

        /// <summary>
        /// WORDSNET-20374 INDEX entries have incorrect Font Style
        /// WORDSNET-20117 Table of Contents does use wrong style
        /// </summary>
        [Test]
        public void TestEntriesWithMinorHAnsiTheme()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestEntriesWithMinorHAnsiTheme.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestEntriesWithMinorHAnsiTheme.docx");
        }


        /// <summary>
        /// WORDSNET-20691 Document.UpdateFields does not update the TOC correctly.
        /// </summary>
        [Test]
        public void Test20691()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test20691.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test20691.docx");
        }

        [Test]
        [TestCase(@"Fields\IndexAndTables\TestIdentifyFieldIndexFormat Bulleted.docx", FieldIndexFormat.Bulleted)]
        [TestCase(@"Fields\IndexAndTables\TestIdentifyFieldIndexFormat Classic.docx", FieldIndexFormat.Classic)]
        [TestCase(@"Fields\IndexAndTables\TestIdentifyFieldIndexFormat Fancy.docx", FieldIndexFormat.Fancy)]
        [TestCase(@"Fields\IndexAndTables\TestIdentifyFieldIndexFormat Formal.docx", FieldIndexFormat.Formal)]
        [TestCase(@"Fields\IndexAndTables\TestIdentifyFieldIndexFormat Modern.docx", FieldIndexFormat.Modern)]
        [TestCase(@"Fields\IndexAndTables\TestIdentifyFieldIndexFormat Simple.docx", FieldIndexFormat.Simple)]
        public void TestIdentifyFieldIndexFormat(string file, FieldIndexFormat format)
        {
            Document document = TestUtil.Open(file);

            Assert.That(document.FieldOptions.FieldIndexFormat, Is.EqualTo(format));
        }

        [Test]
        [TestCase(FieldIndexFormat.Template)]
        [TestCase(FieldIndexFormat.Bulleted)]
        [TestCase( FieldIndexFormat.Classic)]
        [TestCase(FieldIndexFormat.Fancy)]
        [TestCase(FieldIndexFormat.Formal)]
        [TestCase(FieldIndexFormat.Modern)]
        [TestCase(FieldIndexFormat.Simple)]
        public void TestApplyFieldIndexFormat(FieldIndexFormat format)
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestApplyFieldIndexFormat.docx");

            document.FieldOptions.FieldIndexFormat = format;

            TestUtil.SaveCheckGold(
                document,
                string.Format(@"Fields\IndexAndTables\TestApplyFieldIndexFormat {0}.docx", format.ToString()));
        }

        [Test]
        [TestCase(FieldIndexFormat.Template)]
        [TestCase(FieldIndexFormat.Bulleted)]
        [TestCase( FieldIndexFormat.Classic)]
        [TestCase(FieldIndexFormat.Fancy)]
        [TestCase(FieldIndexFormat.Formal)]
        [TestCase(FieldIndexFormat.Modern)]
        [TestCase(FieldIndexFormat.Simple)]
        public void TestFieldIndexFormatRoundtrip(FieldIndexFormat format)
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestApplyFieldIndexFormat.docx");

            document.FieldOptions.FieldIndexFormat = format;

            Assert.That(document.FieldOptions.FieldIndexFormat, Is.EqualTo(format));
        }

        /// <summary>
        /// WORDSNET-21564 TabStop Leader is lost after updating TOC.
        /// </summary>
        [Test]
        public void Test21564()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test21564.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test21564.docx");
        }


        /// <summary>
        /// WORDSNET-18083 TOC field with designated bookmark picks up SEQ entries from outside of it.
        /// </summary>
        [Test]
        public void Test18083()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test18083.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test18083.docx");
        }

        /// <summary>
        /// WORDSNET-22438 Option for Include label and number in Table of Figures.
        /// </summary>
        [Test]
        public void Test22438()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test22438.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test22438.docx");
        }


        /// <summary>
        /// WORDSNET-22509 Make TOA field consider RD entries.
        /// </summary>
        [Test]
        public void Test22509()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test22509.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test22509.docx");
        }

        /// <summary>
        /// WORDSNET-22508 Make INDEX field consider RD entries.
        /// </summary>
        [Test]
        public void Test22508()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test22508.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test22508.docx");
        }

        /// <summary>
        /// WORDSNET-22978 Unexpected behavior of UpdateFields().
        /// </summary>
        [Test]
        public void Test22978()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test22978.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test22978.docx");
        }

        /// <summary>
        /// WORDSNET-23924 InvalidCastException is thrown upon updating fields.
        /// </summary>
        [Test]
        public void Test23924()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test23924.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test23924.docx");
        }

        /// <summary>
        /// WORDSNET-24373 "No table of figures entries found" is shown after updating fields in the document.
        /// </summary>
        [Test]
        public void Test24373()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test24373.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test24373.docx");
        }

        /// <summary>
        /// WORDSNET-24910 Aspose.Words does not include an empty heading paragraph with numbering into the TOC.
        /// </summary>
        [Test]
        public void Test24910()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test24910.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test24910.docx");
        }

        /// <summary>
        /// WORDSNET-25056 Items are missed in INDEX after updating fields.
        /// </summary>
        [Test]
        public void Test25056()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test25056.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test25056.docx");
        }



        /// <summary>
        /// WORDSNET-26492 InvalidOperationException is thrown upon updating fields.
        /// </summary>
        [Test]
        public void Test26492()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test26492.docx");

            document.UpdateFields();

            IStructuredDocumentTag sdt = document.Range.StructuredDocumentTags[1];
            Assert.That(sdt.IsMultiSection, Is.True);

            StructuredDocumentTagRangeStart sdtStart = (StructuredDocumentTagRangeStart)sdt.Node;
            StructuredDocumentTagRangeEnd sdtEnd = sdtStart.RangeEnd;
            Field field = FieldExtractor.ExtractToCollection(document, false, FieldType.FieldIndex)[0];

            Assert.That(sdtStart.IsAbove(field.Start), Is.True);
            Assert.That(sdtEnd.IsAbove(field.End), Is.False);
        }

        /// <summary>
        /// WORDSNET-26327 InvalidOperationException is thrown upon updating fields.
        /// </summary>
        [Test]
        public void Test26327()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test26327.docx");

            document.UpdateFields();

            TestUtil.Save(document, @"Fields\IndexAndTables\Test26327.docx");
        }

        /// <summary>
        /// Tests whether or not TOC field error message has leading paragraph break.
        /// </summary>
        [Test]
        [TestCase("TOC", "\rNo table of contents entries found.")]
        [TestCase("TOC \\l 1", "Error! Not a valid heading level range.")]
        public void TestTocFieldErrorMessage(string fieldCode, string result)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Write("TOC:");
            Field field = builder.InsertField(fieldCode, null);

            field.Update();

            Assert.That(field.Result, Is.EqualTo(result));
        }


        [Test]
        [TestCase(@" TOC ", new string[] { "Heading" })]
        [TestCase(@" TOC \l  ", new string[] {})]
        [TestCase(@" TOC \l 1-1 ", new string[] { "E", "G" })]
        [TestCase(@" TOC \f  ", new string[] { "E", "\tF", "G", "\tH" })]
        [TestCase(@" TOC \f  \l  ", new string[] { "E", "\tF", "G", "\tH" })]
        [TestCase(@" TOC \f  \l 1-1 ", new string[] { "E", "G" })]
        [TestCase(@" TOC \f """" ", new string[] { "C", "\tD" })]
        [TestCase(@" TOC \f """" \l  ", new string[] { "C", "\tD" })]
        [TestCase(@" TOC \f """" \l 1-1 ", new string[] { "C" })]
        [TestCase(@" TOC \f 1 ", new string[] { "A", "\tB" })]
        [TestCase(@" TOC \f 1 \l  ", new string[] { "A", "\tB" })]
        [TestCase(@" TOC \f 1 \l 1-1 ", new string[] { "A" })]
        public void TestTocFieldTCEntries(string fieldCode, string[] result)
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.InsertField(@"TC A \f 1 \l 1");
            builder.InsertField(@"TC B \f 1 \l 2");
            builder.InsertField(@"TC C \f """" \l 1");
            builder.InsertField(@"TC D \f """" \l 2");
            builder.InsertField(@"TC E \f \l 1");
            builder.InsertField(@"TC F \f \l 2");
            builder.InsertField(@"TC G \l 1");
            builder.InsertField(@"TC H \l 2");
            builder.Writeln();

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln("Heading");
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Normal;

            FieldToc field = (FieldToc)builder.InsertField(fieldCode);

            string[] actual = GetTocEntries(field);

            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        [TestCase(@"TOC \f", new string[] { "ItemMissed", "ItemNull", "ItemC" })]
        [TestCase(@"TOC \f """"", new string[] { "ItemEmpty" })]
        [TestCase(@"TOC \f C", new string[] { "ItemMissed", "ItemNull", "ItemC" })]
        public void TestTocCategoryC(string fieldCode, string[] result)
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.InsertField(@"TC ItemMissed \l 1");
            builder.InsertField(@"TC ItemNull \f \l 1");
            builder.InsertField(@"TC ItemEmpty \f """" \l 1");
            builder.InsertField(@"TC ItemA \f A \l 1");
            builder.InsertField(@"TC ItemB \f B \l 1");
            builder.InsertField(@"TC ItemC \f C \l 1");
            builder.InsertField(@"TC ItemD \f D \l 1");
            builder.Writeln();

            FieldToc field = (FieldToc)builder.InsertField(fieldCode);

            string[] actual = GetTocEntries(field);

            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        public void TestLinkedStyleEntryStyleFormatting()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestLinkedStyleEntryStyleFormatting.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestLinkedStyleEntryStyleFormatting.docx");
        }

        [Test]
        public void TestLinkedStyleEntryDirectFormatting()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestLinkedStyleEntryDirectFormatting.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestLinkedStyleEntryDirectFormatting.docx");
        }

        [Test]
        public void TestLinkedCustomStyles()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestLinkedCustomStyles.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestLinkedCustomStyles.docx");
        }

        [Test]
        public void TestListLabelFormatting()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\TestListLabelFormatting.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\TestListLabelFormatting.docx");
        }




        /// <summary>
        /// WORDSNET-27759 Extra empty TOC item appears after updating fields.
        /// </summary>
        [Test]
        public void Test27759()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test27759.docx");

            document.UpdateFields();

            TestUtil.SaveCheckGold(document, @"Fields\IndexAndTables\Test27759.docx");
        }

        /// <summary>
        /// WORDSNET-27595 Page numbers in INDEX field are incorrect.
        /// WORDSNET-27606 Page numbers in INDEX are incorrect if call update field once.
        /// </summary>
        [Test]
        public void Test27595()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test27595.docx");

            document.UpdateFields();

            Field field = FieldExtractor.ExtractToCollection(document, false, FieldType.FieldIndex)[0];

            Assert.That(field.Result, Is.EqualTo("Index Entry, 0\r"));
        }

        /// <summary>
        /// WORDSNET-28790 TOC field includes invalid entry.
        /// </summary>
        [Test]
        public void Test28790()
        {
            Document document = TestUtil.Open(@"Fields\IndexAndTables\Test28790.docx");

            string[] actual = GetTocEntries(document, 0);
            string[] expected =
            {
                "Item 1 (para style)",
                "Item 2 (para style)",
                "Item 3 (char style)",
                "Item 3 (para style)"
            };

            Assert.That(actual, Is.EqualTo(expected));
        }


    }
}
