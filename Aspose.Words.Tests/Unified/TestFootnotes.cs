// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Notes;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests how for model handles footnotes and endnotes.
    /// </summary>
    [TestFixture]
    public class TestFootnotes : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFootnotesBasic(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\TestFootnotes", lf, sf);

            // One footnote and one endnote
            NodeList notes = doc.SelectNodes("//Footnote");
            Assert.That(notes.Count, Is.EqualTo(4));

            // Check the model of a footnote.
            Footnote footnote = (Footnote)notes[0];
            Assert.That(footnote.FootnoteType, Is.EqualTo(FootnoteType.Footnote));
            Assert.That(footnote.IsAuto, Is.EqualTo(true));
            Assert.That(footnote.Font.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteReference));
            Assert.That(footnote.GetText(), Is.EqualTo("\x0002 Footnote 1.\r"));

            // This footnote contains only one paragraph.
            Paragraph footnotePara = (Paragraph)footnote.FirstChild;
            Assert.That(footnote.FirstChild == footnote.LastChild, Is.True);
            Assert.That(footnotePara.ParagraphFormat.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteText));

            // The footnote paragraph contains one SpecialChar that represents the footnote number.
            SpecialChar footnoteNumber = (SpecialChar)footnotePara.FirstChild;
            Assert.That(footnoteNumber.Font.StyleIdentifier, Is.EqualTo(StyleIdentifier.FootnoteReference));
            Assert.That(footnoteNumber.GetText(), Is.EqualTo("\x0002"));

            // Then the footnote paragraph contains one Run that represents the footnote text.
            Run footnoteRun = (Run)footnoteNumber.NextSibling;
            Assert.That(footnoteRun.Font.StyleIdentifier, Is.EqualTo(StyleIdentifier.DefaultParagraphFont));
            Assert.That(footnoteRun.GetText(), Is.EqualTo(" Footnote 1."));


            // Check the model of an endnote.
            Footnote endnote = (Footnote)notes[1];
            Assert.That(endnote.FootnoteType, Is.EqualTo(FootnoteType.Endnote));
            Assert.That(endnote.IsAuto, Is.EqualTo(true));
            Assert.That(endnote.GetText(), Is.EqualTo("\x0002 Endnote 1.\r"));
        }


        /// <summary>
        /// WORDSNET-4993 Footnotes and endnotes with custom marks are not supported.
        /// Added support for footnotes with custom reference mark.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSuppressFootnoteRef(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\TestSuppressFootnoteRef", lf, sf);

            // Document contains 2 footnotes: with custom and with autonumbered reference mark.
            Assert.That(doc.GetChildNodes(NodeType.Footnote, true).Count, Is.EqualTo(2));


            // Check the fx2ootnote with custom reference mark.
            Paragraph fp = doc.FirstSection.Body.FirstParagraph;
            Assert.That(fp.NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(fp.FirstChild.NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(fp.FirstChild.GetText(), Is.EqualTo("Aaa bbb"));

            Footnote footnote = (Footnote)fp.FirstChild.NextSibling;
            Assert.That(footnote.NodeType, Is.EqualTo(NodeType.Footnote));
            Assert.That(footnote.IsAuto, Is.EqualTo(false));

            // rtf not fully supported yet, only first char of reference mark obtained.
            // Rest of reference text left just as text run.
            if (sf == SaveFormat.Rtf)
            {
                Assert.That(footnote.ReferenceMark, Is.EqualTo("e"));

                Run run = (Run)footnote.NextSibling;
                Assert.That(run.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(run.GetText(), Is.EqualTo("ditor"));

                run = (Run)run.NextSibling;
                Assert.That(run.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(run.GetText(), Is.EqualTo(" com"));
            }
            else
            {
                Assert.That(footnote.ReferenceMark, Is.EqualTo("editor com"));

                Run run = (Run)footnote.NextSibling;
                Assert.That(run.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(run.GetText(), Is.EqualTo(" ccc"));
            }


            // Check the footnote with autonumbered mark.
            {
                Paragraph np = (Paragraph)fp.NextSibling;
                Assert.That(np.NodeType, Is.EqualTo(NodeType.Paragraph));
                Assert.That(np.FirstChild.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(np.FirstChild.GetText(), Is.EqualTo("xxx"));

                footnote = (Footnote)np.FirstChild.NextSibling;
                Assert.That(footnote.NodeType, Is.EqualTo(NodeType.Footnote));
                Assert.That(footnote.IsAuto, Is.EqualTo(true));

                Run run = (Run)footnote.NextSibling;
                Assert.That(run.NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(run.GetText(), Is.EqualTo(" yyy"));
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFootnotePerSection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\TestFootnotePerSection", lf, sf);

            // Document-wide
            CheckFootnoteOptions(
                doc.FootnoteOptions,
                FootnotePosition.BeneathText,               // non default
                NumberStyle.Arabic,
                1,
                FootnoteNumberingRule.RestartPage);         // non default
            CheckEndnoteOptions(
                doc.EndnoteOptions,
                EndnotePosition.EndOfSection,              // non default
                NumberStyle.LowercaseRoman,
                1,
                FootnoteNumberingRule.RestartSection);      // non default

            // Section 1
            CheckFootnoteOptions(
                doc.FirstSection.PageSetup.FootnoteOptions,
                FootnotePosition.BeneathText,               // non default
                NumberStyle.Arabic,
                1,
                FootnoteNumberingRule.RestartPage);         // non default
            CheckEndnoteOptions(
                doc.FirstSection.PageSetup.EndnoteOptions,
                EndnotePosition.EndOfSection,              // non default
                NumberStyle.LowercaseRoman,
                1,
                FootnoteNumberingRule.RestartSection);      // non default

            // Section 2
            CheckFootnoteOptions(
                doc.LastSection.PageSetup.FootnoteOptions,
                FootnotePosition.BottomOfPage,
                NumberStyle.GB3,                            // non default
                5,                                          // non default
                FootnoteNumberingRule.Continuous);
            CheckEndnoteOptions(
                doc.LastSection.PageSetup.EndnoteOptions,
                EndnotePosition.EndOfSection,              // non default
                NumberStyle.UppercaseLetter,                // non default
                10,                                         // non default
                FootnoteNumberingRule.Continuous);
        }

        private static void CheckFootnoteOptions(
            FootnoteOptions options,
            FootnotePosition location,
            NumberStyle numberStyle,
            int startNumber,
            FootnoteNumberingRule restartRule)
        {
            Assert.That(options.Position, Is.EqualTo(location));
            Assert.That(options.NumberStyle, Is.EqualTo(numberStyle));
            Assert.That(options.StartNumber, Is.EqualTo(startNumber));
            Assert.That(options.RestartRule, Is.EqualTo(restartRule));
        }

        private static void CheckEndnoteOptions(
            EndnoteOptions options,
            EndnotePosition location,
            NumberStyle numberStyle,
            int startNumber,
            FootnoteNumberingRule restartRule)
        {
            Assert.That(options.Position, Is.EqualTo(location));
            Assert.That(options.NumberStyle, Is.EqualTo(numberStyle));
            Assert.That(options.StartNumber, Is.EqualTo(startNumber));
            Assert.That(options.RestartRule, Is.EqualTo(restartRule));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInsertFootnote(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder b = new DocumentBuilder();

            FootnoteOptions fo = b.Document.FootnoteOptions;
            fo.Position = FootnotePosition.BeneathText;
            fo.RestartRule = FootnoteNumberingRule.RestartPage;
            fo.NumberStyle = NumberStyle.UppercaseRoman;

            EndnoteOptions eo = b.Document.EndnoteOptions;
            eo.Position = EndnotePosition.EndOfDocument;
            eo.NumberStyle = NumberStyle.UppercaseLetter;
            eo.StartNumber = 5;

            b.InsertFootnote(FootnoteType.Footnote, "test footnote");
            b.InsertFootnote(FootnoteType.Endnote, "test endnote");
            b.Writeln();

            TestUtil.SaveOpen(b.Document, @"Model\Footnote\TestInsertFootnote", lf, sf);
        }


        /// <summary>
        /// WORDSNET-5858 NullReferenceException occurred on saving single space ReferenceMark into Docx.
        /// Field.Symbol in ReferenceMark was not supported.
        /// Field.Symbols converts into Runs and fixes this issue.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSymbolicReferenceMark(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\TestJira5858", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            Assert.That(doc.GetChildNodes(NodeType.Footnote, true).Count, Is.EqualTo(2));

            Footnote footnote1 = ((Footnote)doc.GetChild(NodeType.Footnote, 0, true));
            Assert.That(footnote1.ReferenceMark, Is.EqualTo(" "));

            Footnote footnote2 = ((Footnote)doc.GetChild(NodeType.Footnote, 1, true));

            Assert.That(footnote2.ReferenceMark, Is.EqualTo("\xf02a"));
            Assert.That(footnote2.RunPr.NameAscii, Is.EqualTo("Symbol"));
            Assert.That(footnote2.RunPr.NameOther, Is.EqualTo("Symbol"));
        }

        /// <summary>
        /// WORDSNET-11763 Footnote Continuation Notice was not preserved when saving a document.
        /// Added this FootnoteSeparatorType to <see cref="DocxFootnotesWriter"/> and <see cref="WmlFootnotesWriter"/>.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFootnoteContinuationNotice(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\TestJira11763", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            FootnoteSeparator separator = doc.FootnoteSeparators[FootnoteSeparatorType.FootnoteContinuationNotice];
            Assert.That(separator, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-15870 Exception on RTF export, exception on DOCX export.
        /// Invalid footnote inside source RTF document, which contains a ParagraphBreak as a ReferenceMark.
        /// </summary>
        /// <remarks>
        /// RTF reader was imported this document with a null ReferenceMark, which was leading to exceptions on export.
        /// </remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFootnoteParagraphBreakReferenceMark(LoadFormat lf, SaveFormat sf)
        {
            // Verifies no exception.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\TestJira15870", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            NodeCollection footnotes = doc.GetChildNodes(NodeType.Footnote, true);
            Assert.That(footnotes.Count, Is.EqualTo(1));

            // Verifies footnote's ReferenceMark.
            Assert.That(((Footnote)footnotes[0]).ReferenceMark, Is.EqualTo("\r"));
        }

        /// <summary>
        /// WORDSNET-17820 Provide API to Remove the Footnote Endnote Separator Line.
        /// Public API usage to remove separator line.
        /// </summary>
        [Test]
        public void Test17820()
        {
            Document doc = TestUtil.Open(@"Model\Footnote\Test20641.docx");

            FootnoteSeparator endnoteSeparator = doc.FootnoteSeparators[FootnoteSeparatorType.EndnoteSeparator];
            endnoteSeparator.FirstParagraph.FirstChild.Remove();

            doc = TestUtil.SaveOpen(doc, @"Model\Footnote\Test20641_17820.docx", new OoxmlSaveOptions(), false);
            endnoteSeparator = doc.FootnoteSeparators[FootnoteSeparatorType.EndnoteSeparator];
            Assert.That(endnoteSeparator.FirstParagraph.GetText(), Is.EqualTo("\r"));
        }

        /// <summary>
        /// WORDSNET-27239 Modifying paragraph style of the footnote separator and footnote continuation separator
        /// Public API usage to format separator line.
        /// </summary>
        [Test]
        public void Test27239()
        {
            Document doc = TestUtil.Open(@"Model\Footnote\Test20641.docx");
            FootnoteSeparator endnoteSeparator = doc.FootnoteSeparators[FootnoteSeparatorType.EndnoteSeparator];
            endnoteSeparator.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Center;

            doc = TestUtil.SaveOpen(doc, @"Model\Footnote\Test20641_27239.docx", new OoxmlSaveOptions(), false);
            endnoteSeparator = doc.FootnoteSeparators[FootnoteSeparatorType.EndnoteSeparator];
            Assert.That(endnoteSeparator.FirstParagraph.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Center));
        }
    }
}
