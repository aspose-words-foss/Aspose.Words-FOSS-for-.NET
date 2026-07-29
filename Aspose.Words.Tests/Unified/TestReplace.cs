// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Aspose.Collections;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Replacing;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestReplace
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests replacement on run boundaries.
        /// </summary>
        [Test]
        public void TestRunBoundaries()
        {
            const string testName = @"Model\Replace2\TestRunBoundaries.docx";

            Document doc = TestUtil.Open(testName);
            int count = doc.Range.Replace("oro", "^r^");
            Assert.That(count, Is.EqualTo(1));

            RunCollection runs = doc.FirstSection.Body.FirstParagraph.Runs;
            Assert.That(runs[0].Text, Is.EqualTo("am"));
            Assert.That(runs[1].Text, Is.EqualTo("^r^"));
            Assert.That(runs[2].Text, Is.EqualTo("zov"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests only one paragraph break is replaced with simple text.
        /// </summary>
        [Test]
        public void TestParagraphBreak()
        {
            const string testName = @"Model\Replace2\TestParagraphBreak.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("t&ps", "new");
            Assert.That(count, Is.EqualTo(1));

            RunCollection runs = doc.FirstSection.Body.FirstParagraph.Runs;
            Assert.That(runs[0].Text, Is.EqualTo("Firs"));
            Assert.That(runs[1].Text, Is.EqualTo("new"));
            Assert.That(runs[2].Text, Is.EqualTo("econd"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests that textbox content is replaced.
        /// </summary>
        [Test]
        public void TestTextbox()
        {
            const string testName = @"Model\Replace2\TestTextbox.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("t", "(t)");
            Assert.That(count, Is.EqualTo(2));

            Assert.That(doc.GetText(), Is.EqualTo("Some (t)ex(t).\r\f"));

            TestOutput(doc, testName);
        }

        [Test]
        public void TestMultipleOccurenceRun()
        {
            const string testName = @"Model\Replace2\TestMultipleOccurenceRun.docx";

            // Test both directions.
            foreach (FindReplaceDirection direction in gDirections)
            {
                Document doc = TestUtil.Open(testName);

                FindReplaceOptions options = new FindReplaceOptions();
                options.Direction = direction;
                options.ApplyFont.Bold = true;

                int count = doc.Range.Replace("run", "(RUN)", options);
                Assert.That(count, Is.EqualTo(5));
                Assert.That(doc.GetText(), Is.EqualTo("One (RUN) with multiple occurrence of word (RUN), (RUN), (RUN) and one more (RUN).\f"));

                TestOutput(doc, @"Model\Replace2\TestMultipleOccurenceRun.docx");
            }
        }

        /// <summary>
        /// Tests that header textbox content is replaced.
        /// </summary>
        [Test]
        public void TestHeaderTextbox()
        {
            const string testName = @"Model\Replace2\TestHeaderTextbox.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("Some", "The");
            Assert.That(count, Is.EqualTo(2));

            Shape headerTextbox = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Shapes[0];
            Assert.That(headerTextbox.GetText(), Is.EqualTo("The text\r"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replacement on textbox boundaries.
        /// </summary>
        [Test]
        public void TestTextboxMixed()
        {
            const string testName = @"Model\Replace2\TestTextboxMixed.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("Normal textbox", "[replaced]");
            Assert.That(count, Is.EqualTo(0));

            count = doc.Range.Replace("textbox", "~");
            Assert.That(count, Is.EqualTo(1));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace with section break.
        /// </summary>
        [Test]
        public void TestWithSectionBreak()
        {
            const string testName = @"Model\Replace2\TestWithSectionBreak.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("o", "&b");
            Assert.That(count, Is.EqualTo(3));
            Assert.That(doc.Sections.Count, Is.EqualTo(4));

            Assert.That(doc.GetText(), Is.EqualTo("am\fr\fz\fv\f"));

            TestOutput(doc, testName);
        }
        /// <summary>
        /// Tests replace with paragraph break.
        /// </summary>
        [Test]
        public void TestWithParagraphBreak()
        {
            const string testName = @"Model\Replace2\TestWithParagraphBreak.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("[p]", "&p");
            Assert.That(count, Is.EqualTo(2));

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;
            Assert.That(pc[0].Runs[0].Text, Is.EqualTo("All "));
            Assert.That(pc[1].Runs[0].Text, Is.EqualTo(" occurrence needs to be replaced with real paragraph break. "));
            Assert.That(pc[2].Runs[0].Text, Is.EqualTo(" only!"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace simple text with doubled section break.
        /// </summary>
        [Test]
        public void TestWithDoubleSectionBreak()
        {
            const string testName = @"Model\Replace2\TestWithDoubleSectionBreak.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("o", "&b&b");

            Assert.That(count, Is.EqualTo(3));

            SectionCollection sc = doc.Sections;
            Assert.That(sc.Count, Is.EqualTo(7));
            Assert.That(sc[0].GetText(), Is.EqualTo("am\f"));
            Assert.That(sc[1].GetText(), Is.EqualTo("\f"));
            Assert.That(sc[2].GetText(), Is.EqualTo("r\f"));
            Assert.That(sc[3].GetText(), Is.EqualTo("\f"));
            Assert.That(sc[4].GetText(), Is.EqualTo("z\f"));
            Assert.That(sc[5].GetText(), Is.EqualTo("\f"));
            Assert.That(sc[6].GetText(), Is.EqualTo("v\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace with page break.
        /// </summary>
        [Test]
        public void TestWithPageBreak()
        {
            const string testName = @"Model\Replace2\TestWithPageBreak.docx";
            Document doc = TestUtil.Open(testName);
            int count = doc.Range.Replace("[page]", "&m");
            Assert.That(count, Is.EqualTo(2));

            Assert.That(doc.GetText(), Is.EqualTo("Test page break insertion.\f\rWe should get 3 page \f document eventually.\r\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace paragraph break with section break.
        /// </summary>
        [Test]
        public void TestParagraphWithSection()
        {
            const string testName = @"Model\Replace2\TestParagraphWithSection.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("&p", "&b");
            Assert.That(count, Is.EqualTo(4));

            SectionCollection sc = doc.Sections;

            Assert.That(sc.Count, Is.EqualTo(5));
            Assert.That(sc[0].GetText(), Is.EqualTo("1.\f"));
            Assert.That(sc[1].GetText(), Is.EqualTo("2.\f"));
            Assert.That(sc[2].GetText(), Is.EqualTo("3.\f"));
            Assert.That(sc[3].GetText(), Is.EqualTo("4.\f"));
            // Last paragraph break is also replaced.
            Assert.That(sc[4].GetText(), Is.EqualTo("\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace of two sequential paragraph break with one paragraph break.
        /// </summary>
        [Test]
        public void TestDoubleParagraphBreaks()
        {
            const string testName = @"Model\Replace2\TestDoubleParagraphBreaks.docx";

            Document doc = TestUtil.Open(testName);
            int count = doc.Range.Replace("&p&p", "&p");
            Assert.That(count, Is.EqualTo(2));

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;

            Assert.That(pc.Count, Is.EqualTo(3));
            Assert.That(pc[0].GetText(), Is.EqualTo("1\r"));
            Assert.That(pc[1].GetText(), Is.EqualTo("2\r"));
            Assert.That(pc[2].GetText(), Is.EqualTo("3.\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Copy-pasted from TestReplace to ensure that new method works for such case.
        /// </summary>
        [Test]
        public void TestReplaceOneRunWithEmpty()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInManyRuns.docx");

            //Replace one run completely with empty string and check its completely deleted (removed from the document).
            //Note this also tests how it works when Range.Replace is executed on the run node itself.
            Run run = (Run)doc.SelectSingleNode("//Run[2]");
            Assert.That(run.Range.Replace("Two", ""), Is.EqualTo(1));

            //Check run was removed from the document.
            Assert.That(doc.SelectNodes("//Run").Count, Is.EqualTo(6));
            Assert.That(run.ParentNode, Is.EqualTo(null));

            Assert.That(run.Text, Is.EqualTo(""));
        }

        /// <summary>
        /// Tests that text is replaced in headers/footers.
        /// </summary>
        [Test]
        public void TestHeaderText()
        {
            const string testName = @"Model\Replace2\TestHeaderText.docx";

            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("amorozov", "Morozov Alexey&p690034.");
            Assert.That(count, Is.EqualTo(3));
            Assert.That(doc.GetText(), Is.EqualTo("Morozov Alexey\r690034.\rMorozov Alexey\r690034.\rMorozov Alexey\r690034.\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests that formatting is applied to new runs.
        /// </summary>
        [Test]
        public void TestApplyHighlight()
        {
            const string testName = @"Model\Replace2\TestApplyHighlight.docx";

            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.HighlightColor = Color.Yellow;

            int count = doc.Range.Replace("the", "the", options);

            Assert.That(count, Is.EqualTo(3));
            foreach (Run run in doc.FirstSection.Body.FirstParagraph.Runs)
            {
                if (run.Text == "the")
                    Assert.That(run.RunPr[FontAttr.HighlightColor], IsNot.Null());
                else
                    Assert.That(run.RunPr[FontAttr.HighlightColor], Is.Null);
            }

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace two sequential paragraph breaks with run.
        /// </summary>
        [Test]
        public void TestDoubleParagraphBreaksWithRun()
        {
            const string testName = @"Model\Replace2\TestDoubleParagraphBreaksWithRun.docx";
            Document doc = TestUtil.Open(testName);
            int count = doc.Range.Replace("&p&p", "[double-break]");

            Assert.That(count, Is.EqualTo(4));
            Assert.That(doc.GetText(), Is.EqualTo("Sample[double-break]text[double-break]with[double-break]few[double-break]whitespaces.\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests that text is replace inside of table cells.
        /// </summary>
        [Test]
        public void TestCellText()
        {
            const string testName = @"Model\Replace2\TestCellText.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("C", "Cell");
            Assert.That(count, Is.EqualTo(4));

            foreach (Row row in doc.FirstSection.Body.Tables[0].Rows)
                foreach (Cell cell in row.Cells)
                    Assert.That(cell.FirstParagraph.FirstRun.Text.StartsWith("Cell"), Is.True);

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests paragraph formatting is applied to new content.
        /// </summary>
        [Test]
        public void TestApplyParagraphFormat()
        {
            const string testName = @"Model\Replace2\TestApplyParagraphFormat.docx";
            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyParagraphFormat.Alignment = ParagraphAlignment.Right;

            int count = doc.Range.Replace(".&p", ".&p", options);

            Assert.That(count, Is.EqualTo(2));
            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;

            Assert.That(pc[0].ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(pc[1].ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Left));
            Assert.That(pc[2].ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace all breaks with run.
        /// </summary>
        [Test]
        public void TestAllBreaks()
        {
            const string testName = @"Model\Replace2\TestAllBreaks.docx";

            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.Color = Color.Orange;

            doc.Range.Replace("&b", "[section-break]", options);
            doc.Range.Replace("&m", "[page-break]", options);
            doc.Range.Replace("&p", "[paragraph-break]", options);
            doc.Range.Replace("&l", "[line-break]", options);

            Assert.That(doc.GetText(), Is.EqualTo("After this word should be section break [section-break], then follows manual page break [page-break], " +
                "after that is paragraph break [paragraph-break] and finally is manual line break [line-break]. This is end of file." +
                "[paragraph-break]\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests replace tags with all supported breaks.
        /// </summary>
        [Test]
        public void TestWithAllBreaks()
        {
            const string testName = @"Model\Replace2\TestWithAllBreaks.docx";

            Document doc = TestUtil.Open(testName);

            doc.Range.Replace("[section-break]", "&b");
            doc.Range.Replace("[page-break]", "&m");
            doc.Range.Replace("[paragraph-break]", "&p");
            doc.Range.Replace("[line-break]", "&l");

            // Check document structure.
            Assert.That(doc.Sections.Count, Is.EqualTo(2));
            Assert.That(doc.FirstSection.Body.GetText(), Is.EqualTo("After this word should be section break \f"));

            Section section = doc.Sections[1];
            Assert.That(section.Body.Paragraphs.Count, Is.EqualTo(2));
            // Note page break in first paragraph.
            Assert.That(section.Body.Paragraphs[0].GetText(), Is.EqualTo(", then follows manual page break \f, after that is paragraph break \r"));
            // Note manual line break in second paragraph.
            Assert.That(section.Body.Paragraphs[1].GetText(), Is.EqualTo(" and finally is manual line break \v. This is end of file.\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests that section break cannot be inserted to header.
        /// </summary>
        [Test]
        public void TestWithSectionBreakInHeader()
        {
            const string testName = @"Model\Replace2\TestWithSectionBreakInHeader.docx";
            Document doc = TestUtil.Open(testName);

            Assert.That(doc.Sections.Count, Is.EqualTo(1));
            int count = doc.Range.Replace("amorozov", "a&bv");

            // Two matches found but no replacement actually made.
            Assert.That(count, Is.EqualTo(2));

            // Still one section.
            Assert.That(doc.Sections.Count, Is.EqualTo(1));

            Assert.That(doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("av\r"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests that section break cannot be inserted into cell.
        /// </summary>
        [Test]
        public void TestWithSectionBreakInCell()
        {
            const string testName = @"Model\Replace2\TestWithSectionBreakInCell.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("section break", "&b");

            // We returned actual match.
            Assert.That(count, Is.EqualTo(3));

            // Only one section break was inserted.
            Assert.That(doc.Sections.Count, Is.EqualTo(2));

            // Text is removed but break is not inserted.
            Assert.That(doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.GetText(), Is.EqualTo("Try to insert .\a"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests that formatting is taken from matched run.
        /// </summary>
        [Test]
        public void TestRunFormattingCopied()
        {
            const string testName = @"Model\Replace2\TestRunFormattingCopied.docx";

            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("amorozov", "amorozov");
            Assert.That(count, Is.EqualTo(1));

            Paragraph p = doc.FirstSection.Body.FirstParagraph;

            Assert.That(p.Runs.Count, Is.EqualTo(2));

            Run secondRun = p.Runs[1];
            Assert.That(secondRun.Text, Is.EqualTo("amorozov"));
            Assert.That(p.Runs[1].RunPr[FontAttr.Size], Is.EqualTo(96));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests section break replacement.
        /// </summary>
        [Test]
        public void TestSectionBreak()
        {
            const string testName = @"Model\Replace2\TestSectionBreak.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("&b", "[here-was-section]");

            Assert.That(count, Is.EqualTo(1));
            Assert.That(doc.Sections.Count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo("Section break [here-was-section]deletion\f"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests comment text replacement.
        /// </summary>
        [Test]
        public void TestCommentText()
        {
            const string testName = @"Model\Replace2\TestCommentText.docx";
            Document doc = TestUtil.Open(testName);

            int count = doc.Range.Replace("To be replaced!", "Done.");
            Assert.That(count, Is.EqualTo(1));

            Comment comment = (Comment)doc.GetChild(NodeType.Comment, 0, true);
            Assert.That(comment.GetText(), Is.EqualTo("\x0005Done.\r"));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// WORDSNET-1252 Improve Range.Replace to support breaks
        /// </summary>
        [Test]
        public void TestJira1252()
        {
            const string testName = @"Model\Replace2\TestJira1252.docx";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("11112222");

            doc.Range.Replace("1111", "EMK3 Sales&p15770 N Dallas Parkway&p");
            doc.Range.Replace("2222", "Dallas, TX 75248&pAttn:  Terry CoulterEMK3 Sales&p");

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(5));

            TestOutput(doc, testName);
        }

        /// <summary>
        /// Tests how tag with paragraph break before table is replaced with section break.
        /// </summary>
        [Test]
        public void TestWithSectionBreakBeforeTable()
        {
            const string testName = @"Model\Replace2\TestWithSectionBreakBeforeTable.docx";
            Document doc = TestUtil.Open(testName);

            Assert.That(doc.Sections.Count, Is.EqualTo(1));
            int count = doc.Range.Replace("[section~break]&p", "&b");

            Assert.That(count, Is.EqualTo(1));

            // Tag is successfully replaced with section break.
            Assert.That(doc.Sections.Count, Is.EqualTo(2));

            TestOutput(doc, testName);
        }


        /// <summary>
        /// WORDSNET-7456 NullReferenceException occurs when performing a find and replace operation
        /// </summary>
        [Test]
        public void TestJira7456()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira7456.docx");

            int count = doc.Range.Replace("[~pagebreak~]", "&m");
            Assert.That(count, Is.EqualTo(5));

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;

            Assert.That(pc[9].FirstRun.Text, Is.EqualTo("\f"));
            Assert.That(pc[13].FirstRun.Text, Is.EqualTo("\f"));
            Assert.That(pc[17].FirstRun.Text, Is.EqualTo("\f"));
            Assert.That(pc[19].FirstRun.Text, Is.EqualTo("\f"));
            Assert.That(pc[22].FirstRun.Text, Is.EqualTo("\f"));
        }

        /// <summary>
        /// Tests replacement of section break if document has only one section.
        /// </summary>
        [Test]
        public void TestReplaceOnlySection()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Hello");
            doc.EnsureMinimum();

            Assert.That(doc.GetText(), Is.EqualTo("Hello\r\f"));
            int count = doc.Range.Replace("&b", "");

            Assert.That(count, Is.EqualTo(0));
            Assert.That(doc.GetText(), Is.EqualTo("Hello\r\f"));
        }

        /// <summary>
        /// Tests replacement of section break if document has only one section.
        /// </summary>
        [Test]
        public void TestMergeSections()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Section1");
            builder.InsertBreak(BreakType.SectionBreakContinuous);
            builder.Writeln("Section2");

            Assert.That(doc.Sections.Count, Is.EqualTo(2));

            int count = doc.Range.Replace("&b", "");

            Assert.That(count, Is.EqualTo(1));
            Assert.That(doc.Sections.Count, Is.EqualTo(1));

            TestOutput(doc, @"Model\Replace2\TestMergeSections.docx");
        }


        /// <summary>
        /// Section break is inside structured document tag.
        /// </summary>
        [Test]
        public void TestJira4826()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestDefect4826.docx");

            int count = doc.Range.Replace("&b", "[section-break]");

            Assert.That(count, Is.EqualTo(1));

            // Section break inside of SDT is removed.
            Assert.That(doc.Sections.Count, Is.EqualTo(1));
        }


        /// <summary>
        /// Tests Wiki demonstration sample.
        /// </summary>
        /// <remarks>
        /// Demonstrates break support.
        /// </remarks>
        [Test]
        public void TestWiki01()
        {
            const string testName = @"Model\Replace2\TestWiki01.docx";
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Writeln("First section");
            builder.Writeln("  1st paragraph");
            builder.Writeln("  2nd paragraph");
            builder.Writeln("{insert-section}");
            builder.Writeln("Second section");
            builder.Writeln("  1st paragraph");

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyParagraphFormat.Alignment = ParagraphAlignment.Center;

            // Double each paragraph break after word "section", add dot to end of paragraph and make it centered.
            int count = doc.Range.Replace("section&p", "section.&p&p", options);
            Assert.That(count, Is.EqualTo(2));

            // Insert section break instead of custom text tag.
            count = doc.Range.Replace("{insert-section}", "&b", options);
            Assert.That(count, Is.EqualTo(1));

            TestOutput(doc, testName);
        }
        /// <summary>
        /// Tests code sample from public API comment.
        /// </summary>
        [Test]
        public void TestCodeSample_1()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Writeln("Numbers 1, 2, 3");

            // Inserts paragraph break after Numbers.
            doc.Range.Replace("Numbers", "Numbers&p");

            Assert.That(doc.GetText(), Is.EqualTo("Numbers\r 1, 2, 3\r\f"));
        }


        /// <summary>
        /// WORDSNET-15046 Range.Replace finds and replaces text in the incorrect order
        /// FindReplace method is updated to does replacement in table along with the document body.
        /// </summary>
        [Test]
        public void TestJira15046()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira15046.docx");
            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.HighlightColor = Color.Yellow;
            options.ReplacingCallback = new TestJira15046Callback();
            doc.Range.Replace("tag", "", options);

            Assert.That(doc.GetText(), Is.EqualTo("Tag#0\rTag#1\rtag#2\atag#3\a\a\rTag#4\r\f"));
        }

        internal class TestJira15046Callback : IReplacingCallback
        {
            private int mIndex;
            public ReplaceAction Replacing(ReplacingArgs args)
            {
                args.Replacement = string.Format("{0}#{1}", args.Match.Value, mIndex++);
                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// Relates to WORDSNET-14046
        /// Tests that content of adjacent cell in not joined to be replaced.
        /// </summary>
        [Test]
        public void TestJira15046A()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira15046A.docx");

            // No replacement should be made.
            int count = doc.Range.Replace("12", "XX");
            Assert.That(count, Is.EqualTo(0));
        }


        /// <summary>
        /// WORDSNET-15869 RTL text with spaces reversed after replacement
        /// Seems that we need to allow white-spaces when decide to force Bidi for strong RTL replacement.
        /// </summary>
        [Test]
        public void TestJira15869()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira15869.doc");
            doc.Range.Replace("IIFirstNameII", "ראשון שני");

            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.RunPr[FontAttr.Bidi], Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Tests sorting headers/footers in the node pending list.
        /// </summary>
        [Test]
        public void TestSortingPendingList()
        {
            Document doc = new Document();
            List<Node> testList = new List<Node>();

            Comment comment1 = new Comment(doc);
            testList.Add(comment1);

            Section section1 = new Section(doc);
            section1.PageSetup.DifferentFirstPageHeaderFooter = true;
            HeaderFooter firstHeader1 = new HeaderFooter(doc, HeaderFooterType.HeaderFirst);
            HeaderFooter primaryHeader1 = new HeaderFooter(doc, HeaderFooterType.HeaderPrimary);
            HeaderFooter evenHeader1 = new HeaderFooter(doc, HeaderFooterType.HeaderEven);
            HeaderFooter firstFooter1 = new HeaderFooter(doc, HeaderFooterType.FooterFirst);
            HeaderFooter primaryFooter1 = new HeaderFooter(doc, HeaderFooterType.FooterPrimary);
            HeaderFooter evenFooter1 = new HeaderFooter(doc, HeaderFooterType.FooterEven);
            AppendNodes(new Node[] { evenFooter1, primaryHeader1, firstFooter1, evenHeader1, primaryFooter1,
                firstHeader1 }, section1, testList);

            Comment comment2 = new Comment(doc);
            testList.Insert(2, comment2);
            Comment comment3 = new Comment(doc);
            testList.Insert(3, comment3);
            Comment comment4 = new Comment(doc);
            testList.Add(comment4);

            Section section2 = new Section(doc);
            section2.PageSetup.DifferentFirstPageHeaderFooter = false;
            HeaderFooter primaryHeader2 = new HeaderFooter(doc, HeaderFooterType.HeaderPrimary);
            HeaderFooter evenHeader2 = new HeaderFooter(doc, HeaderFooterType.HeaderEven);
            HeaderFooter primaryFooter2 = new HeaderFooter(doc, HeaderFooterType.FooterPrimary);
            HeaderFooter evenFooter2 = new HeaderFooter(doc, HeaderFooterType.FooterEven);
            AppendNodes(new Node[] { evenFooter2, primaryFooter2, evenHeader2, primaryHeader2 }, section2, testList);

            Comment comment5 = new Comment(doc);
            testList.Add(comment5);

            FindReplace.SortPendingList(testList);

            object[] expectedOrder = new object[] { comment1, firstHeader1, firstFooter1, evenHeader1, evenFooter1,
                primaryHeader1, primaryFooter1, comment2, comment3, comment4, primaryHeader2, primaryFooter2,
                evenHeader2, evenFooter2, comment5 };

            Assert.That(testList.Count, Is.EqualTo(expectedOrder.Length));
            for (int i = 0; i < testList.Count; i++ )
                Assert.That(testList[i], Is.SameAs(expectedOrder[i]), string.Format("Wrong object at index {0}", i));
        }

        /// <summary>
        /// Appends nodes from the specified array as children into the composite node and into the specified list.
        /// </summary>
        private void AppendNodes(Node[] nodes, CompositeNode compositeNode, IList<Node> list)
        {
            foreach (Node node in nodes)
            {
                compositeNode.AppendChildForLoad(node);
                list.Add(node);
            }
        }



        /// <summary>
        /// WORDSNET-16189 Range.Replace text within GroupShape does not Replace.
        /// Nodes of "GroupShape" type should not be ignored while processing in <see cref="FindReplaceIndexer"/>.
        /// </summary>
        [Test]
        public void TestJira16189()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira16189.docx");

            FindReplaceOptions findReplaceOptions = new FindReplaceOptions
            {
                FindWholeWordsOnly = false,
                MatchCase = false,
                Direction = FindReplaceDirection.Backward
            };
            doc.Range.Replace("foo", "bar", findReplaceOptions);

            GroupShape shape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Run run = (Run)shape.GetChild(NodeType.Run, 0, true);
            Assert.That(run.Text, Is.EqualTo("bar"));
        }

        /// <summary>
        /// Related to WORDSNET-16189
        /// Tests FindReplaceIndexer for GroupShape.
        /// </summary>
        [Test]
        public void TestJira16189FindReplaceIndexerForGroupShapeTest()
        {
            GroupShape shape = new GroupShape(new Document());
            FindReplaceIndexer replaceIndexer = new FindReplaceIndexer(shape, new FindReplaceOptions());

            IndexerAction result = replaceIndexer.OnNode(shape);
            Assert.That(result, Is.EqualTo(IndexerAction.None));
        }

        /// <summary>
        ///  WORDSNET-15840 ReplacingArgs.Replacement returns incorrect value.
        ///  Added replacement option to preserve meta-characters beginning with "&amp;".
        /// </summary>
        [Test]
        public void TestJira15840()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("text");

            // WORDSNET-20266 Obsolete FindReplaceOptions.PreserveMetaCharacters option removed.
            // & character should be escaped.
            doc.Range.Replace("text", @"&&ldquo;");
            Assert.That(doc.GetText(), Is.EqualTo("&ldquo;\f"));
        }

        /// <summary>
        /// WORDSNET-16272 Add a feature to escape ampersand symbol during replacement
        /// Added && meta character to eascape & character.
        /// </summary>
        [Test]
        public void TestJira16272()
        {
            Assert.That(DoReplace("text", "text", @"&&ldquo;"), Is.EqualTo("&ldquo;\f"));
            Assert.That(DoReplace("&&ldquo;", "&&ldquo;", "replaced"), Is.EqualTo("&replaced\f"));
        }

        private static string DoReplace(string text, string what, string with)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write(text);

            doc.Range.Replace(what, with);

            return doc.GetText();
        }



        /// <summary>
        /// Relates to WORDSNET-16217
        /// Tests Replace without Regex.
        /// </summary>
        [Test]
        public void TestJira16217Related()
        {
            Document doc = new Document();
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            docBuilder.Writeln("$IMPORT_TEXT");
            docBuilder.Writeln("$IMPORT_TEXTAREA");

            FindReplaceOptions findReplaceOptions = new FindReplaceOptions();
            findReplaceOptions.FindWholeWordsOnly = true;

            doc.Range.Replace("$IMPORT_TEXT", "testing", findReplaceOptions);
            doc.Range.Replace("$IMPORT_TEXTAREA", "textarea", findReplaceOptions);

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            Assert.That(((Run)runs[0]).Text, Is.EqualTo("testing"));
            Assert.That(((Run)runs[1]).Text, Is.EqualTo("textarea"));
        }

        private static void DoAsposeDocReplaceText(Document doc, string[] templateFields, string[] values)
        {
            FindReplaceOptions options = new FindReplaceOptions();
            options.MatchCase = false;
            options.FindWholeWordsOnly = false;
            for (int i = 0; i < values.Length; i++)
            {
                doc.Range.Replace(templateFields[i], values[i], options);
            }
        }

        /// <summary>
        /// Useful method to tests replace method.
        /// </summary>
        private static void TestOutput(Document doc, string fileName)
        {
            //const string tempPath = @"X:\_1252\";
            //ModelDump.Save(doc, tempPath + Path.GetFileName(fileName) + ".model");
            //TestUtil.SaveOpen(doc, Path.Combine(tempPath, Path.GetFileNameWithoutExtension(fileName)) + " Replaced" + Path.GetExtension(fileName));
        }

        /// <summary>
        /// Test case sensitive and insensitive replace within a single run.
        /// </summary>
        [Test]
        public void TestReplaceInOneRun()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInOneRun.docx");

            //Test case insensitive replace.
            Assert.That(doc.Range.Replace("text", "Replaced text"), Is.EqualTo(3));
            Assert.That(doc.GetText(), Is.EqualTo("Replaced text in header.\rReplaced text in body. Replaced text in body.\x000c"));

            //Test case sensitive replace.
            FindReplaceOptions options = new FindReplaceOptions() { MatchCase = true, FindWholeWordsOnly = false };
            Assert.That(doc.Range.Replace("Text", "XXX", options), Is.EqualTo(0));
        }

        [Test]
        public void TestReplaceAcrossTwoRuns()
        {
            //This document is used for several tests.
            //
            //   Bold      Underline    Italic
            //One[Two]Three[FourFive]Six[SevenEight]NineTen
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInManyRuns.docx");
            Assert.That(doc.SelectNodes("//Run").Count, Is.EqualTo(7));

            // Replace text that goes across two runs
            Assert.That(doc.Range.Replace("neTw", "xxx"), Is.EqualTo(1));

            //First run now contains some of the old plus new text.
            Run run = (Run)doc.SelectSingleNode("//Run[1]");
            Assert.That(run.Text, Is.EqualTo("O"));
            Assert.That(run.Font.Bold, Is.EqualTo(false));

            // Second run contains replacement.
            run = (Run)doc.SelectSingleNode("//Run[2]");
            Assert.That(run.Text, Is.EqualTo("xxx"));
            Assert.That(run.Font.Bold, Is.EqualTo(false));

            // Third run now contains only part of the old text.
            run = (Run)doc.SelectSingleNode("//Run[3]");
            Assert.That(run.Text, Is.EqualTo("o"));
            Assert.That(run.Font.Bold, Is.EqualTo(true));
        }

        [Test]
        public void TestReplaceAcrossManyRuns()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInManyRuns.docx");

            //One[Two]Three[FourFive]Six[SevenEight]NineTen
            Assert.That(doc.Range.Replace("FiveSixSeven", "xxx"), Is.EqualTo(1));
            Assert.That(doc.SelectNodes("//Run").Count, Is.EqualTo(7));

            Run run = (Run)doc.SelectSingleNode("//Run[5]");
            Assert.That(run.Text, Is.EqualTo("xxx"));
            Assert.That(run.Font.Underline, Is.EqualTo(Underline.Single));

            run = (Run)doc.SelectSingleNode("//Run[6]");
            Assert.That(run.Text, Is.EqualTo("Eight"));
            Assert.That(run.Font.Italic, Is.EqualTo(true));
        }

        [Test]
        public void TestReplaceOneRunWithText()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInManyRuns.docx");

            //Replace one run completely with new text and check its formatting stays.
            Assert.That(((Run)doc.SelectSingleNode("//Run[2]")).Range.Replace("Two", "AAA"), Is.EqualTo(1));
            Run run = (Run)doc.SelectSingleNode("//Run[2]");

            //Check run stayed in the document.
            Assert.That(doc.SelectNodes("//Run").Count, Is.EqualTo(7));
            Assert.That(run.ParentNode, IsNot.Null());

            Assert.That(run.Text, Is.EqualTo("AAA"));
            Assert.That(run.Font.Bold, Is.EqualTo(true));
        }

        [Test]
        public void TestReplaceManyRunsWithEmpty()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInManyRuns.docx");

            //Replace three runs completely with empty string, they should be deleted.
            Assert.That(doc.Range.Replace("TwoThreeFourFive", ""), Is.EqualTo(1));
            Assert.That(doc.SelectNodes("//Run").Count, Is.EqualTo(4));
        }

        [Test]
        public void TestReplaceManyRunsWithText()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceInManyRuns.docx");

            //Replace three runs completely with text and check the formatting stays.
            Assert.That(doc.Range.Replace("TwoThreeFourFive", "AAA"), Is.EqualTo(1));
            Run run = (Run)doc.SelectSingleNode("//Run[2]");
            Assert.That(run.Text, Is.EqualTo("AAA"));
            Assert.That(run.Font.Bold, Is.EqualTo(true));
        }

        [Test]
        public void TestReplaceWholeWord()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceWholeWord.docx");

            //Surrounded by spaces, replaced okay.
            Node para = doc.SelectSingleNode("//Paragraph[1]");
            FindReplaceOptions options = new FindReplaceOptions() { MatchCase = false, FindWholeWordsOnly = true };
            Assert.That(para.Range.Replace("aaa", "X", options), Is.EqualTo(1));
            Assert.That(para.GetText(), Is.EqualTo("Some X surrounded by spaces.\r"));

            //Part of word, not found.
            para = doc.SelectSingleNode("//Paragraph[2]");
            options = new FindReplaceOptions() { MatchCase = false, FindWholeWordsOnly = true };
            Assert.That(para.Range.Replace("aaa", "X", options), Is.EqualTo(0));
            Assert.That(para.GetText(), Is.EqualTo("Someaaa part of word.\r"));

            //In brackets, replaced okay.
            para = doc.SelectSingleNode("//Paragraph[3]");
            options = new FindReplaceOptions() { MatchCase = false, FindWholeWordsOnly = true };
            Assert.That(para.Range.Replace("aaa", "X", options), Is.EqualTo(1));
            Assert.That(para.GetText(), Is.EqualTo("Some[X] in brackets.\r"));

            //At end of sentence, replaced okay.
            para = doc.SelectSingleNode("//Paragraph[4]");
            options = new FindReplaceOptions() { MatchCase = false, FindWholeWordsOnly = true };
            Assert.That(para.Range.Replace("aaa", "X", options), Is.EqualTo(1));
            Assert.That(para.GetText(), Is.EqualTo("Some X. At end of sentence.\r"));
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The search string cannot be null or empty.")]
        public void TestReplaceEmpty()
        {
            Document doc = new Document();
            doc.Range.Replace("", "A");
        }

        public void TestReplaceAcrossParagraphUnescaped()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceAcrossParent.docx");
            //There is a way to pass unescaped \r, it is also caught.
            int replacementCount = doc.Range.Replace(new Regex("Para1\rPara2"), "");
            Assert.That(replacementCount, Is.EqualTo(0));
        }

        [Test]
        public void TestReplaceAcrossCell()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceAcrossParent.docx");
            int replacementCount = doc.Range.Replace("Cell1\x0007Cell2", "");
            Assert.That(replacementCount, Is.EqualTo(0));
        }

        [Test]
        public void TestReplaceAcrossSection()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceAcrossParent.docx");
            int replacementCount = doc.Range.Replace("Section1\x000cSection2", "");
            Assert.That(replacementCount, Is.EqualTo(0));
        }

        /// <summary>
        /// The bookmark start or end that is at the start or end of a match, should stay intact.
        /// </summary>
        [Test]
        public void TestReplaceWithBookmarkAroundMatch()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceWithBookmarks.docx");
            Assert.That(doc.Range.Bookmarks["bmk1"].Text, Is.EqualTo("Test1Word2"));

            doc.Range.Replace("Test1Word2", "xxx");
            //Check the bookmark stayed in the doc.
            Assert.That(doc.Range.Bookmarks["bmk1"].Text, Is.EqualTo("xxx"));
        }

        [Test]
        public void TestReplaceWithBookmarksInsideMatch()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceWithBookmarks.docx");
            Assert.That(doc.Range.Bookmarks["bmk2"].Text, Is.EqualTo("Test2Word1 Test2"));
            Assert.That(doc.Range.Bookmarks["bmk3"].Text, Is.EqualTo("Word2 Test2Word3"));
            Assert.That(doc.Range.Bookmarks["bmk4"].Text, Is.EqualTo("st2Wor"));

            doc.Range.Replace("Test2Word2", "xxx");

            //This actually works exactly like in MS Word.
            Assert.That(doc.Range.Bookmarks["bmk2"].Text, Is.EqualTo("Test2Word1 "));

            //This one is slightly different from how MS Word does it. MS Word will keep " Test2Word3" only.
            Assert.That(doc.Range.Bookmarks["bmk3"].Text, Is.EqualTo("xxx Test2Word3"));

            //This is different from how MS Word does it. In MS Word if a bookmark is completely inside
            //a match, it is deleted. I don't want to bother deleting the bookmark completely.
            Assert.That(doc.Range.Bookmarks["bmk4"].Text, Is.EqualTo(""));
        }

        /// <summary>
        /// WORDSNET-19439 Paragraph alignment is lost when Range.Replace is used with &p.
        /// Word copies props of the para to the new paras that are created during the replacement. Mimic Word.
        /// </summary>
        [Test]
        public void Test19439()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test19439.docx");

            FindReplaceOptions fr = new FindReplaceOptions();
            fr.ApplyParagraphFormat.SpaceAfter = 20;
            fr.ApplyFont.Underline = Underline.Dash;

            doc.Range.Replace("<Replace2>", "This text has&pmore than one line&pand some more", fr);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            for (int i = 2; i < 4; i++)
            {
                Assert.That(paras[i].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
                Assert.That(paras[i].FirstRun.Font.Bold, Is.True);

                // Checks that replacement options work.
                Assert.That(paras[i].ParaPr.SpaceAfter, Is.EqualTo(400));
                Assert.That(paras[i].FirstRun.Font.Underline, Is.EqualTo(Underline.Dash));
            }
        }


        /// <summary>
        /// WORDSNET-19543 Bullet list is lost after using Range.Replace with &p special meta-character
        /// We need to copy properties of paragraph being replaced.
        /// </summary>
        [Test]
        public void Test19543()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\Test19543.docx");

            // Customer case.
            foreach (Node node in doc.GetChildNodes(NodeType.Paragraph, true).ToArray())
            {
                Paragraph paragraph = node as Paragraph;
                paragraph.Range.Replace("&p", "~line-break~&p", new FindReplaceOptions());
            }

            // Check that all paragraphs are bullet now.
            doc.UpdateListLabels();
            foreach (Paragraph para in doc.FirstSection.Body.Paragraphs)
            {
                Assert.That(para.ListLabel.LabelString, Is.EqualTo("\xf0b7"));
            }
        }


        /// <summary>
        /// WORDSNET-20212 Range.Replace does not replace the numbers.
        /// We substitute different text breaks with our own substitution characters from the Unicode private area block.
        /// This causes regex engine to not recognize line breaks properly when RegexOptions.Multiline option is enabled.
        /// Fixed by splitting text onto lines manually when RegexOptions.Multiline option is enabled.
        /// </summary>
        [Test]
        public void Test20212()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test20212.docx");

            doc.Range.Replace(new Regex(@"^[1-9]{1}[0-9]{10}[&p]$", RegexOptions.Multiline), "REPLACED&p");

            Assert.That(doc.GetText(), Is.EqualTo("REPLACED\rREPLACED\rREPLACED\rREPLACED\rREPLACED\r\f"));
        }

        /// <summary>
        /// WORDSNET-20186 Document.Range.Replace regex string anchors not working.
        /// Duplicates WORDSNET-20212
        /// </summary>
        [Test]
        public void Test20186()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test20186.docx");

            doc.Range.Replace(new Regex(@"^Here", RegexOptions.Multiline), "REPLACED");

            Assert.That(doc.FirstSection.Body.Paragraphs[1].GetText(), Is.EqualTo("REPLACED is the second line.\f"));
        }

        /// <summary>
        /// WORDSNET-21018 Return deprecated replace algorithm for compatibility reason.
        /// Added compatibility flag that allows to call old algorithm version.
        /// </summary>
        [Test]
        public void Test21018()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            builder.Writeln("test");
            Run run = builder.Document.FirstSection.Body.Paragraphs[0].Runs[0];
            FindReplaceOptions replaceOptions = new FindReplaceOptions() { LegacyMode = true };
            run.Range.Replace("test", "TEST", replaceOptions);
            Assert.That(run.Text, Is.EqualTo("TEST"));
            Assert.That(run.ParentNode, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-20007 Parent node of Run node returns null after replacing text of Run node
        /// Use existing run for replacement.
        /// </summary>
        [Test]
        public void Test20007()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            builder.Writeln("test");
            Run run = builder.Document.FirstSection.Body.Paragraphs[0].Runs[0];
            run.Range.Replace("test", "TEST");
            Assert.That(run.Text, Is.EqualTo("TEST"));
            Assert.That(run.ParentNode, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-21588 IReplacingCallback is never executed.
        /// We should calculate offset of matches relative to whole text instead of current line
        /// when passing into callback method. Also don't split text onto lines when there are no
        /// any anchors (^, $) specified in regex pattern.
        /// </summary>
        [TestCase("Test21588.docx", "{{5{ Don’t Display  }}5}")]
        [TestCase("Test21588_WithBreak.docx", "{{5{ Don’t Display  }}5}")]
        public void Test21588(string testName, string expectedMatchValue)
        {
            Document doc = TestUtil.Open(string.Format(@"Model\Replace\{0}", testName));

            Test21588ReplacingCallback replacingCallback = new Test21588ReplacingCallback();
            FindReplaceOptions options = new FindReplaceOptions {ReplacingCallback = replacingCallback};
            doc.Range.Replace(new Regex("({{5{).*(}}5})", RegexOptions.Multiline), "", options);

            Assert.That(replacingCallback.MatchValue, Is.EqualTo(expectedMatchValue));

            // The main checks are performed in replacing callback method.
            Assert.That(replacingCallback.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Relates to WORDSNET-21588
        /// Checks various combinations of anchors with multiline regex.
        /// </summary>
        [TestCase("b", 2)]
        [TestCase("^b", 1)]
        [TestCase("d", 2)]
        [TestCase("^d", 1)]
        [TestCase("c", 2)]
        [TestCase("c&p", 1)]
        [TestCase("ab&pcd", 1)]
        [TestCase("d&pb", 1)]
        public void Test21588Anchors(string pattern, int expectedCount)
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Write("ab\ncd\nb\ndc");

            int count = builder.Document.Range.Replace(new Regex(pattern, RegexOptions.Multiline), "");
            Assert.That(count, Is.EqualTo(expectedCount));
        }


        /// <summary>
        /// WORDSNET-21639 Range.Replace throws System.IndexOutOfRangeException when LegacyMode = false.
        /// The actual problem is in FindWholeWordsOnly option. We should check that position of next character
        /// of matched text is not greater than the length of whole text
        /// in <see cref="FindReplace.IsWordBoundary(System.Text.RegularExpressions.Match,int)"/>.
        /// </summary>
        [Test]
        public void Test21639()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test21639.docx");

            FindReplaceOptions options = new FindReplaceOptions {FindWholeWordsOnly = true};
            Assert.That(doc.Range.Replace(new Regex(@".*"), "", options), Is.GreaterThan(0));
        }

        /// <summary>
        /// WORDSNET-21329 Range.Replace does not Remove CR Character in Cell with Nested Table.
        /// The new <see cref="FindReplaceOptions.SmartParagraphBreakReplacement"/> is introduced that allows to move
        /// all child nodes from the paragraph into NextPreOrder paragraph. This allows to remove the paragraph itself.
        /// </summary>
        [TestCase(false, NodeType.Paragraph)]
        [TestCase(true, NodeType.Table)]
        public void Test21329(bool forceReplaceParagraphBreak, NodeType expectedNodeType)
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test21329.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.SmartParagraphBreakReplacement = forceReplaceParagraphBreak;

            int count = doc.Range.Replace(new Regex(@"REMOVE_CR&p"), "", options);

            Assert.That(doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.FirstChild.NodeType, Is.EqualTo(expectedNodeType));

            // Count of replacement made equal for both cases regardless no replacement occurred if forceReplaceParagraphBreak = false.
            Assert.That(count, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-22364 Document word find and replace issue
        /// LineBreak character \v should be word boundary character.
        /// </summary>
        [Test]
        public void Test22364()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test22364.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.Direction = FindReplaceDirection.Forward;
            options.MatchCase = false;
            options.FindWholeWordsOnly = true;

            doc.Range.Replace("Variable.name", "Srinivas", options);
            doc.Range.Replace("Variable.city", "blo", options); //failed to replace
            doc.Range.Replace("Variable.zip", "1234", options); //failed to replace

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[3].GetText(), Is.EqualTo("Srinivas\r"));
            Assert.That(paras[5].GetText(), Is.EqualTo("blo\u000b1234\u000c"));
        }

        /// <summary>
        /// WORDSNET-22269 Quotes in Word document aren't matched by Replace
        /// Double quote in a pattern is considered equal to any curly/smart quote in text.
        /// </summary>
        [Test]
        public void Test22269()
        {
            Document doc1 = new Document();

            DocumentBuilder builder = new DocumentBuilder(doc1);
            builder.Writeln("\"quoted\"");
            builder.Writeln("“quoted”");
            builder.Writeln("”quoted“");

            Document doc2 = doc1.Clone();

            doc1.Range.Replace("\"quoted\"", "James Bond", new FindReplaceOptions());
            // Regex pattern should be checked exactly like in MS Word.
            doc2.Range.Replace(new Regex("\"quoted\""), "James Bond", new FindReplaceOptions());

            Assert.That(doc1.GetText(), Is.EqualTo("James Bond\rJames Bond\rJames Bond\r\f"));
            Assert.That(doc2.GetText(), Is.EqualTo("James Bond\r“quoted”\r”quoted“\r\f"));
        }

        /// <summary>
        /// WORDSNET-22353 Header and footer lost after replace with section break
        /// Implemented Word-like section break insertion.
        /// </summary>
        [Test]
        public void Test22353()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\Test22353.docx");
            doc.Range.Replace("~", "&b");

            // Properties of both sections are equal.
            Assert.That(doc.Sections[0].SectPr.Equals(doc.Sections[1].SectPr), Is.True);

            // Header moved into first newly created section.
            Assert.That(doc.Sections[0].HeadersFooters.Count, Is.EqualTo(1));
            Assert.That(doc.Sections[1].HeadersFooters.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-22371 Ignore Footnotes in Range.Replace
        /// Added new public option FindReplaceOptions.IgnoreFootnotes.
        /// </summary>
        [Test]
        public void Test22371()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\Test22371.docx");

            FindReplaceOptions options = new FindReplaceOptions() { IgnoreFootnotes = true };
            int count = doc.Range.Replace("Hello World", "other", options);

            // Check that replacement has occurred.
            Assert.That(count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.Text, Is.EqualTo("other"));
        }

        /// <summary>
        /// WORDSNET-22814 Range.Replace does not replace the numbers when Number ends with line, section and page break.
        /// We should additionally split onto lines by LineBreak ('\v') and PageBreak('\f') characters in multiline mode.
        /// </summary>
        [Test]
        [JavaDelete("Jdk16 did not support group matching with string")]
        public void Test22814()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test22814.docx");

            Test22814ReplacingCallback replacingCallback = new Test22814ReplacingCallback();
            FindReplaceOptions options = new FindReplaceOptions { ReplacingCallback = replacingCallback };
            int count = doc.Range.Replace(
                new Regex(@"^[1-9]{1}[0-9]{10}(?<line_end_metachars>[&p|&m|&b|&l]*)$", RegexOptions.Multiline),
                "REPLACED",
                options);

            // Check that problematic text for which customer complains is missing (i.e., it was replaced successfully).
            string docText = doc.GetText();
            // Text that ends with LineBreaks.
            Assert.That(docText.Contains("72345678901"), Is.False);
            Assert.That(docText.Contains("72345678902"), Is.False);
            // Text that ends with SectionBreak.
            Assert.That(docText.Contains("72345678903"), Is.False);
            // Text that ends with PageBreak.
            Assert.That(docText.Contains("62345678902"), Is.False);

            // Check total replacements count.
            Assert.That(count, Is.EqualTo(16));
        }


        /// <summary>
        /// WORDSNET-23258 IReplacingCallback is called before processing FindWholeWordsOnly option.
        /// Moved check of 'FindWholeWordsOnly' option up to before call to replacing callback method.
        /// </summary>
        [TestCase(true, 2)]
        [TestCase(false, 9)]
        public void Test23258(bool isFindWholeWordsOnly, int expectedReplacementsCount)
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test23258.docx");

            Test23258ReplacingCallback replacingCallback = new Test23258ReplacingCallback();
            FindReplaceOptions options =
                new FindReplaceOptions { ReplacingCallback = replacingCallback, FindWholeWordsOnly = isFindWholeWordsOnly };
            doc.Range.Replace("test", "test", options);

            Assert.That(replacingCallback.Count, Is.EqualTo(expectedReplacementsCount));
        }

        /// <summary>
        /// WORDSNET-23392 No way to apply Style to ApplyFont in FindReplaceOptions.
        /// Additional check in Font.Style setter added.
        /// </summary>
        [Test]
        public void Test23392()
        {
            int[] runIndexes = new int[] { 1, 3, 5, 7, 9 };
            Document doc = TestUtil.Open(@"Model\Replace\Test23392.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            Style style = doc.Styles.FetchByName("formlink");
            options.ApplyFont.Style = style;
            int count = doc.Range.Replace("run", "(RUN)", options);

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            foreach (int runIndex in runIndexes)
            {
                Assert.That((runs[runIndex] as Run).Text, Is.EqualTo("(RUN)"));
                Assert.That((runs[runIndex] as Run).Font.Istd, Is.EqualTo(style.Istd));
            }
        }

        /// <summary>
        /// WORDSJAVA-2665 Calculation of number fields with only dot separator (German Locale) is incorrect
        /// </summary>
        [Test]
        public void TestJava2665()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");

            Document doc = TestUtil.Open(@"Model\Replace\TestJava2665.docx");

            doc.Range.Replace("<<numbervalue1>>", "100.000");
            doc.Range.Replace("<<numbervalue2>>", "100000");
            doc.Range.Replace("<<numbervalue3>>", "100000,00");
            doc.Range.Replace("<<numbervalue4>>", "100.000,00");

            doc.UpdateFields();

            TestUtil.SaveCheckGold(doc, @"Model\Replace\TestJava2665.docx");
        }





        /// <summary>
        /// WORDSNET-24701 Aspose.Words replace behavior differs from MS Word when matched text contains a shape.
        /// We should consider shapes when building text with NodeIndexer.
        /// </summary>
        [TestCase("Test24701No.docx", 0)]
        [TestCase("Test24701Yes.docx", 1)]
        public void Test24701(string fileName, int expectedCount)
        {
            Document doc = TestUtil.Open(string.Format(@"Model\Replace\{0}", fileName));

            int count = doc.Range.Replace("5678", "AB");
            Assert.That(count, Is.EqualTo(expectedCount));
        }


        /// <summary>
        /// WORDSNET-25418 FindReplaceOptions.ApplyFont.Color is not applied.
        /// We should reset theme color when applying a color.
        /// </summary>
        [Test]
        public void Test25418()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test25418.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.Color = Color.Red;
            int count = doc.Range.Replace("PLACEHOLDER", "REPLACEMENT", options);

            Assert.That(count, Is.EqualTo(1));

            Run run = doc.FirstSection.Body.FirstParagraph.FirstRun;
            Assert.That(run.RunPr[FontAttr.Color], Is.EqualTo(DrColor.FromNativeColor(Color.Red)));
            Assert.That(run.RunPr[FontAttr.ThemeColor], Is.Null);
        }


        /// <summary>
        /// WORDSNET-26924 Find/replace paces replacement before the matched text instead of after as MS Word does.
        /// Reworked Find/Replace feature so that the replacement order matches Word.
        /// </summary>
        [Test]
        public void Test26924()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test26924.docx");

            doc.StartTrackRevisions("Changes");
            int count = doc.Range.Replace("Soft Costs", "abcd");
            doc.StopTrackRevisions();

            Assert.That(count, Is.EqualTo(1));

            Run deletedRun = TestUtil.GetRunWithText(doc.FirstSection.Body.FirstParagraph, "Soft Costs");
            Assert.That(((ITrackableNode)deletedRun).DeleteRevision, IsNot.Null());
            Run insertedRun = (Run)deletedRun.NextSibling;
            Assert.That(((ITrackableNode)insertedRun).InsertRevision, IsNot.Null());
            Assert.That(insertedRun.Text, Is.EqualTo("abcd"));
        }

        /// <summary>
        /// Relates to WORDSNET-26924.
        /// This is a slightly more complex customer case with paragraph break and
        /// runs that are needed to be split during replacing process.
        /// </summary>
        [Test]
        public void Test26924A()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test26924A.docx");

            doc.StartTrackRevisions("Changes");
            int count = doc.Range.Replace("oft&p Cost", "abcd");
            doc.StopTrackRevisions();

            Assert.That(count, Is.EqualTo(1));

            Run deletedRun = TestUtil.GetRunWithText(doc.FirstSection.Body.FirstParagraph, "o");
            Assert.That(((ITrackableNode)deletedRun).DeleteRevision, IsNot.Null());

            deletedRun = (Run)deletedRun.NextSibling;
            Assert.That(((ITrackableNode)deletedRun).DeleteRevision, IsNot.Null());
            Assert.That(deletedRun.Text, Is.EqualTo("ft"));

            Paragraph deletedParagraph = deletedRun.ParentParagraph;
            Assert.That(((ITrackableNode)deletedParagraph).DeleteRevision, IsNot.Null());

            deletedRun = ((Paragraph)deletedParagraph.NextSibling).FirstRun;
            Assert.That(((ITrackableNode)deletedRun).DeleteRevision, IsNot.Null());
            Assert.That(deletedRun.Text, Is.EqualTo(" Cost"));

            Run insertedRun = (Run)deletedRun.NextSibling;
            Assert.That(((ITrackableNode)insertedRun).InsertRevision, IsNot.Null());
            Assert.That(insertedRun.Text, Is.EqualTo("abcd"));

            Paragraph paragraph = insertedRun.ParentParagraph;
            Assert.That(paragraph.HasRevisions, Is.False);
        }

        /// <summary>
        /// WORDSNET-23740 Track revisions works not the same as in MS Word when use Replace function.
        /// Duplicates WORDSNET-26924.
        /// </summary>
        [Test]
        public void Test23740()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test23740.docx");

            doc.StartTrackRevisions("aspose");
            int count = doc.Range.Replace("Aspose", "aSPOSE");
            doc.StopTrackRevisions();

            Assert.That(count, Is.EqualTo(1));

            Run deletedRun = TestUtil.GetRunWithText(doc.FirstSection.Body.FirstParagraph, "Aspose");
            Assert.That(((ITrackableNode)deletedRun).DeleteRevision, IsNot.Null());

            Run insertedRun = (Run)deletedRun.NextSibling;
            Assert.That(((ITrackableNode)insertedRun).InsertRevision, IsNot.Null());
            Assert.That(insertedRun.Text, Is.EqualTo("aSPOSE"));

            Paragraph paragraph = insertedRun.ParentParagraph;
            Assert.That(paragraph.HasRevisions, Is.False);
            Assert.That(paragraph.GetText(), Is.EqualTo("AsposeaSPOSE.Words\f"));
        }

        /// <summary>
        /// WORDSNET-28047 ArgumentOutOfRangeException when using regex replacing inside Run.
        /// There are matches with zero length in customer regex at the very beginning of text.
        /// This means nothing to delete, but something to insert. In this case we cannot find
        /// very first node after which to insert a replacement.
        /// </summary>
        [Test]
        public void Test28047()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test28047.docx");

            // Customer scenario.
            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            foreach (Run run in runs)
                run.Range.Replace(new Regex("^\\s*"), "1");

            Assert.That(doc.GetText(), Is.EqualTo("\r1Hello World!\r\r1Hello Word!\r\r\r1Hello World!1This is a problem!\f"));
        }

        /// <summary>
        /// Relates to WORDSNET-28047.
        /// Checks when there actually was a deletion, and then
        /// we want to replace it again due to a specific regex.
        /// </summary>
        [Test]
        public void Test28047A()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test28047A.docx");

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            foreach (Run run in runs)
                // Note, here is missing '^' anchor in regex as it was in original case of the customer.
                run.Range.Replace(new Regex("\\s*"), "=");

#if JAVA
            Assert.That(doc.GetText(), Is.EqualTo("=AB==CD\f"));
#else
            Assert.That(doc.GetText(), Is.EqualTo("=A=B===C=D=\f"));
#endif
        }

        /// <summary>
        /// Tests replacing SDT content in LegacyMode.
        /// </summary>
        [TestCase("ame)", 1, "%(n***")]
        [TestCase("ame) ", 0, "%(name)")]
        public void TestReplaceSdtContentLegacy(string pattern, int expUpdateCount, string expReplaceText)
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestSdtContent.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.LegacyMode = true;
            int count = doc.Range.Replace(pattern, "***", options);

            Assert.That(count, Is.EqualTo(expUpdateCount));

            StructuredDocumentTag sdt = doc.GetChild(NodeType.StructuredDocumentTag, 0, true) as StructuredDocumentTag;
            Run updatedRun = sdt.FirstChild as Run;

            Assert.That(updatedRun.Text, Is.EqualTo(expReplaceText));
            Assert.That(sdt.XmlMapping.GetValue(), Is.EqualTo(expReplaceText));
        }





        /// <summary>
        /// The replacing callback method for <see cref="TestReplace.Test28070"/>.
        /// </summary>
        private class Test28070ReplacingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                StartNodeText = e.MatchNode.GetText();
                EndNodeText = e.MatchEndNode.GetText();

                return ReplaceAction.Replace;
            }

            internal string StartNodeText { get; private set; }
            internal string EndNodeText { get; private set; }
        }

        /// <summary>
        /// The replacing callback method for <see cref="TestReplace.Test23258"/>.
        /// </summary>
        private class Test23258ReplacingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
            {
                Count++;
                return ReplaceAction.Replace;
            }

            /// <summary>
            /// Gets an integer value, indicating how many times Replacing method was called.
            /// </summary>
            internal int Count { get; private set; }
        }

        /// <summary>
        /// The replacing callback method for WORDSNET-22814
        /// </summary>
        [JavaDelete("Jdk16 did not support group matching with string")]
        private class Test22814ReplacingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
            {
                if (args.Match.Groups["line_end_metachars"] != null)
                {
                    args.Replacement +=
                        args.Match.Groups["line_end_metachars"].Value; //preserve metacharacters when replacing stuff
                }

                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// The replacing callback method for WORDSNET-21588
        /// </summary>
        private class Test21588ReplacingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs args)
            {
                Assert.That(args.MatchOffset, Is.EqualTo(0));
                Assert.That(args.MatchNode.GetText(), Is.EqualTo("{{5{"));

                MatchValue = args.Match.Value;
                Count++;
                return ReplaceAction.Skip;
            }

            /// <summary>
            /// Gets string representing value of the match.
            /// </summary>
            internal string MatchValue { get; private set; }

            /// <summary>
            /// Gets an integer value, indicating how many times Replacing method was called.
            /// </summary>
            internal int Count { get; private set; }
        }

        private static readonly FindReplaceDirection[] gDirections = { FindReplaceDirection.Forward, FindReplaceDirection.Backward };
    }
}
