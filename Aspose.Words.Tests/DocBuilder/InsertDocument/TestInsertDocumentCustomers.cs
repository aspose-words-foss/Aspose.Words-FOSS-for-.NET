// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/22/2016 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// Class for testing issues reported by customers related to DocumentBuilder.InsertDocument().
    /// </summary>
    public class TestInsertDocumentCustomers
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-14073 DocumentBuilder.InsertDocument changes page margins in output document.
        /// This special case when inserting document into the
        /// last empty section is processing correctly now.
        /// </summary>
        [Test]
        public void TestJira14073()
        {
            Document dstDoc = OpenTestDoc("TestJira14073dst.docx");
            Document srcDoc = OpenTestDoc("TestJira14073src.docx");

            InsertAtEndWithBreak(dstDoc, srcDoc, BreakType.SectionBreakContinuous, ImportFormatMode.KeepSourceFormatting);

            PageSetup expectedPgSetup = srcDoc.LastSection.PageSetup;
            PageSetup actualPgSetup = dstDoc.LastSection.PageSetup;

            Assert.That(actualPgSetup.TopMargin, Is.EqualTo(expectedPgSetup.TopMargin));
            Assert.That(actualPgSetup.BottomMargin, Is.EqualTo(expectedPgSetup.BottomMargin));
            Assert.That(actualPgSetup.LeftMargin, Is.EqualTo(expectedPgSetup.LeftMargin));
            Assert.That(actualPgSetup.RightMargin, Is.EqualTo(expectedPgSetup.RightMargin));
        }

        /// <summary>
        /// WORDSNET-14438 Unexpected empty space is appeared after using DocumentBuilder.InsertDocument.
        /// Should consider '\f' character as whitespace in DocumentInserter.PostInsertFormatting().
        /// </summary>
        [Test]
        public void TestJira14438()
        {
            Paragraph firstInsertedPara = (Paragraph)InsertAtEndWithBreak("TestJira14438dst.docx", "TestJira14438src.docx",
                BreakType.PageBreak, ImportFormatMode.UseDestinationStyles);

            Assert.That(firstInsertedPara.GetText(), Is.EqualTo("\fTHIS IS OME HEADLINE.\r"));
        }


        /// <summary>
        /// WORDSNET-16127 Different behavior of Section New Page Break in 16.7 and 17.11
        /// When inserting into the last empty section, BreakType should be preserved from the destination section.
        /// </summary>
        [TestCase(BreakType.SectionBreakContinuous)]
        [TestCase(BreakType.SectionBreakNewPage)]
        [TestCase(BreakType.SectionBreakEvenPage)]
        [TestCase(BreakType.SectionBreakOddPage)]
        [TestCase(BreakType.SectionBreakNewColumn)]
        public void TestJira16127(BreakType breakType)
        {
            Document dstDoc = OpenTestDoc("TestJira16127dst.docx");
            Document srcDoc = OpenTestDoc("TestJira16127src.docx");

            InsertAtEndWithBreak(dstDoc, srcDoc, breakType, ImportFormatMode.KeepSourceFormatting);

            Assert.That(dstDoc.LastSection.SectPr[SectAttr.SectionStart], Is.EqualTo(ToSectionStart(breakType)));
        }




        /// <summary>
        /// WORDSNET-19351 List number does not restart when document is inserted using InsertDocument method.
        /// An ImportFormatOptions was not properly passed into a NodeImporter while inserting a document.
        /// </summary>
        [TestCase(true, "1.")]
        [TestCase(false, "2.")]
        public void Test19351(bool isKeepSourceNumbering, string expectedLabelString)
        {
            // Open the same document twice in order to list definitions be the same in source and destination documents.
            Document dstDoc = OpenTestDoc("Test19351.docx");
            Document srcDoc = OpenTestDoc("Test19351.docx");

            DocumentBuilder builder = new DocumentBuilder(dstDoc);
            builder.MoveToDocumentEnd();
            builder.InsertBreak(BreakType.SectionBreakNewPage);

            ImportFormatOptions options = new ImportFormatOptions();
            options.KeepSourceNumbering = isKeepSourceNumbering;
            Paragraph para = (Paragraph)builder.InsertDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);

            dstDoc.UpdateListLabels();

            Assert.That(para.ListLabel.LabelString, Is.EqualTo(expectedLabelString));
        }

        /// <summary>
        /// WORDSNET-19532 Issues with output formatting when using KeepSourceFormatting and KeepDifferentStyles
        /// of ImportFormatMode enum.
        /// When concatenating last paragraph before inserted content with a first imported paragraph,
        /// the style of last one should be preserved.
        /// </summary>
        [TestCase(ImportFormatMode.KeepSourceFormatting)]
        [TestCase(ImportFormatMode.KeepDifferentStyles)]
        public void Test19532(ImportFormatMode importFormatMode)
        {
            Document dstDoc = OpenTestDoc("Test19532Dst.docx");
            Document srcDoc = OpenTestDoc("Test19532Src.docx");

            DocumentBuilder builder = new DocumentBuilder(dstDoc);

            // Insert into the middle of the paragraph.
            Run run = TestUtil.GetRunWithText(dstDoc.FirstSection.Body.FirstParagraph, "...ipsum...");
            builder.MoveTo(run);
            builder.InsertDocument(srcDoc, importFormatMode);

            // Check style is preserved.
            ParagraphCollection paras = dstDoc.FirstSection.Body.Paragraphs;
            Assert.That(paras[0].ParagraphStyle.Name, Is.EqualTo("Heading 1"));

            // Check formatting of the concatenated paragraph.
            // ParaPr:
            Assert.That(paras[0].ParaPr[ParaAttr.SpaceBefore], Is.EqualTo(333));
            // RunPr:
            RunPr paragraphBreakRunPr = paras[0].ParagraphBreakRunPr;
            Assert.That(paragraphBreakRunPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0x2E, 0x74, 0xB5)));
            Assert.That(paragraphBreakRunPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromName("MingLiU_HKSCS")));
            Assert.That(paragraphBreakRunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.False));
            Assert.That(paragraphBreakRunPr[FontAttr.Size], Is.EqualTo(32));
            Assert.That(paragraphBreakRunPr[FontAttr.SizeBi], Is.Null);

            // Check formatting of the left part of concatenated paragraph.
            run = paras[0].FirstRun;
            RunPr runRunPr = run.RunPr;
            Assert.That(runRunPr[FontAttr.Color], Is.Null);
            Assert.That(runRunPr[FontAttr.NameAscii], Is.Null);
            Assert.That(runRunPr[FontAttr.Bold], Is.Null);

            // Check formatting of the right part of concatenated paragraph.
            runRunPr = ((Run)run.NextSibling).RunPr;
            Assert.That(runRunPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0x2E, 0x74, 0xB5)));
            Assert.That(runRunPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromName("MingLiU_HKSCS")));
            Assert.That(runRunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.False));
            Assert.That(runRunPr[FontAttr.Size], Is.EqualTo(32));
            Assert.That(runRunPr[FontAttr.SizeBi], Is.Null);
        }

        /// <summary>
        /// WORDSNET-20413 Comment becomes part of content.
        /// Implemented inserting of comment nodes in DocumentInserter.
        /// </summary>
        [Test]
        public void Test20413()
        {
            Document doc = TestUtil.Open(@"DocBuilder\InsertDocument\Customers\Test20413.docx");

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            StructuredDocumentTag sdt = (StructuredDocumentTag)para.GetChild(NodeType.StructuredDocumentTag, 0, false);

            // Check there is a run before comment.
            Run run = (Run)sdt.FirstChild;
            Assert.That(run.Text, Is.EqualTo("Test content "));

            // Check there is a comment range start inside SDT.
            CommentRangeStart commentRangeStart = (CommentRangeStart)run.NextSibling;
            Assert.That(commentRangeStart, IsNot.Null());

            // Check the problematic run is inside the comment range.
            run = (Run)commentRangeStart.NextSibling;
            Assert.That(run.Text, Is.EqualTo("1"));

            // Check there is a corresponding comment range end inside SDT.
            CommentRangeEnd commentRangeEnd = (CommentRangeEnd)run.NextSibling;
            Assert.That(commentRangeEnd.Id, Is.EqualTo(commentRangeStart.Id));

            // Check there is also a comment itself.
            Comment comment = (Comment)commentRangeEnd.NextSibling;
            Assert.That(comment.FirstParagraph.FirstRun.Text, Is.EqualTo("Test comment "));
        }


        /// <summary>
        /// WORDSNET-22446 InsertDocument does not preserve HeaderFooters and formatting when inserting into empty section.
        /// Reworked algorithm of inserting document into empty destination section.
        /// </summary>
        [TestCase("DstBlank")]
        [TestCase("DstNonBlank")]
        [TestCase("DstBlankWithHeader")]
        [TestCase("DstBlankMultiSections")]
        [TestCase("DstBlankMultiWithAllSectHeader")]
        [TestCase("DstBlankMultiWithSect0Header")]
        [TestCase("DstBlankMultiWithSect1Header")]
        [TestCase("DstBlankMultiWithSect2Header")]
        [TestCase("DstNonEmptyBody0")]
        [TestCase("DstNonEmptyBody1")]
        [TestCase("DstNonEmptyBody2")]
        public void Test22446(string testName)
        {
            const string testDir = @"DocBuilder\InsertDocument\DstBlank";
            const string srcFileName = "Src.docx";
            string srcPath = Path.Combine(testDir, srcFileName);
            string dstFileName = string.Format("{0}.docx", testName);
            string dstPath = Path.Combine(testDir, dstFileName);

            Document srcDoc = TestUtil.Open(srcPath);

            // Check document insertion in both modes: 'UseDestination' and 'KeepSource'.
            ImportFormatMode[] importFormatModes = { ImportFormatMode.UseDestinationStyles, ImportFormatMode.KeepSourceFormatting };
            foreach (ImportFormatMode mode in importFormatModes)
            {
                string suffix = (mode == ImportFormatMode.UseDestinationStyles) ? "ms_UseDestination" : "ms_KeepSource";

                // Check document insertion into every section of destination document.
                int sectCount = 1;
                int curSectIdx = 0;
                do
                {
                    Document dstDoc = TestUtil.Open(dstPath);
                    if (curSectIdx == 0)
                        sectCount = dstDoc.Sections.Count;

                    // Insert document into the current destination section.
                    DocumentBuilder builder = new DocumentBuilder(dstDoc);
                    builder.MoveToSection(curSectIdx);
                    builder.InsertDocument(srcDoc, mode);

                    // Check all sections against gold document produced by Word.
                    string sectNum = string.Format("Sect{0}", curSectIdx);
                    string goldName = string.Format("{0} {1}{2}.docx", dstFileName, suffix, sectNum);
                    CheckSections(dstDoc, Path.Combine(testDir, goldName));

                    curSectIdx++;
                }
                while (curSectIdx < sectCount);
            }
        }







        /// <summary>
        /// Checks that specified ParaPr is as expected in <see cref="Test22149"/>.
        /// </summary>
        private static void CheckParaPr22149(Paragraph paragraph, string expectedText)
        {
            Assert.That(paragraph.GetText().StartsWith(expectedText), Is.True);
            Assert.That(paragraph.ListLabel.LabelString, Is.EqualTo(""));

            ParaPr paraPr = paragraph.ParaPr;

            Assert.That(paraPr[ParaAttr.ListId], Is.EqualTo(0));
            Assert.That(paraPr[ParaAttr.ListLevel], Is.EqualTo(0));
            Assert.That(paraPr[ParaAttr.LeftIndent], Is.EqualTo(113));

            Assert.That(paraPr.Contains(ParaAttr.Istd), Is.True);
            Assert.That(paraPr.Contains(ParaAttr.RsidP), Is.True);

            Assert.That(paraPr.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// The helper method for <see cref="TestJira14397"/>.
        /// It checks ListId, LeftIndent and SpaceBefore of specified paragraph attributes.
        /// </summary>
        private static void CheckParaPr14397(ParaPr paraPr)
        {
            Assert.That(paraPr.ListId, Is.EqualTo(2));
            Assert.That(paraPr.LeftIndent, Is.EqualTo(1440));
            Assert.That(paraPr.SpaceBefore, Is.EqualTo(600));
        }

        /// <summary>
        /// Checks <see cref="TestJira17083"/> with a specified options.
        /// </summary>
        private static void CheckTestJira17083(ImportFormatOptions options)
        {
            Document dstDoc = OpenTestDoc(@"TestJira17083Dst.docx");
            Document srcDoc = OpenTestDoc(@"TestJira17083Src.docx");

            DocumentBuilder builder = new DocumentBuilder(dstDoc);

            builder.MoveToMergeField("MERGE-TANF-LINE", false, true);
            Paragraph para = (Paragraph)builder.InsertDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);

            // This is first imported run before which we check run with white space.
            Run run = TestUtil.GetRunWithText(para, "...Because you receive TANF cash and medical benefits...");
            Assert.That(((Run)run.PreviousSibling).Text.Equals(" "), Is.EqualTo(options.AdjustSentenceAndWordSpacing));
        }

        /// <summary>
        /// Converts BreakType to SectionStart.
        /// </summary>
        private static SectionStart ToSectionStart(BreakType breakType)
        {
            switch (breakType)
            {
                case BreakType.SectionBreakContinuous:
                    return SectionStart.Continuous;
                case BreakType.SectionBreakNewPage:
                    return SectionStart.NewPage;
                case BreakType.SectionBreakEvenPage:
                    return SectionStart.EvenPage;
                case BreakType.SectionBreakOddPage:
                    return SectionStart.OddPage;
                case BreakType.SectionBreakNewColumn:
                    return SectionStart.NewColumn;
                default:
                    throw new InvalidOperationException(string.Format("Unexpected break type: {0}", breakType));
            }
        }

        /// <summary>
        /// Inserts one document at the end of another with break between them.
        /// </summary>
        /// <returns>First inserted node inside destination document.</returns>
        private static Node InsertAtEndWithBreak(Document dstDoc, Document srcDoc, BreakType breakType, ImportFormatMode mode)
        {
            DocumentBuilder builder = new DocumentBuilder(dstDoc);
            builder.MoveToDocumentEnd();
            builder.InsertBreak(breakType);

            return builder.InsertDocument(srcDoc, mode);
        }

        /// <summary>
        /// Inserts one document at the end of another with break between them.
        /// </summary>
        /// <returns>First inserted node inside destination document.</returns>
        private static Node InsertAtEndWithBreak(string dstFileName, string srcFileName, BreakType breakType, ImportFormatMode mode)
        {
            Document dstDoc = OpenTestDoc(dstFileName);
            Document srcDoc = OpenTestDoc(srcFileName);

            return InsertAtEndWithBreak(dstDoc, srcDoc, breakType, mode);
        }

        private static Document OpenTestDoc(string filename)
        {
            return TestUtil.Open(GetTestPath(filename));
        }

        /// <summary>
        /// Returns path relative to 'DocBuilder\InsertDocument\Customers'.
        /// </summary>
        private static string GetTestPath(string filename)
        {
            return Path.Combine(@"DocBuilder\InsertDocument\Customers\", filename);
        }

        /// <summary>
        /// Checks sections of a specified document against gold document.
        /// </summary>
        private static void CheckSections(Document doc, string goldPath)
        {
            Document goldDoc = TestUtil.Open(TestUtil.GetInTestGoldPath(goldPath));

            Assert.That(doc.Sections.Count, Is.EqualTo(goldDoc.Sections.Count));

            for (int i = 0; i < goldDoc.Sections.Count; i++)
                AreEqual(goldDoc.Sections[i], doc.Sections[i]);
        }

        /// <summary>
        /// Returns true, if text and formatting of specified sections are equal.
        /// </summary>
        private static void AreEqual(Section sectionA, Section sectionB)
        {
            Assert.That(sectionB.GetText(), Is.EqualTo(sectionA.GetText()));
            Assert.That(sectionA.SectPr.Equals(sectionB.SectPr), Is.True);
        }
    }
}
