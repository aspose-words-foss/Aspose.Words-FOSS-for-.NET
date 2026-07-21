// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2015 by Nikolay Eremin

using System;
using System.Collections.Generic;
using Aspose.Words.Loading;
using Aspose.Words.Notes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Basic node functionality tests.
    /// </summary>
    [TestFixture]
    public class TestFootnote
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-11107 Support Custom Mark for FootNote
        ///
        /// FootNote can contain ReferenceMark more than 1 char.
        /// </summary>
        [Test]
        public void TestFootnoteCustomMark()
        {
            string customMark = "test123456";
            string text = "testfootnote";
            string fileName = @"Model\Footnote\TestDefect11107";

            Document doc = new Document();

            Footnote footnote = new Footnote(doc, FootnoteType.Footnote);
            Assert.That(footnote.IsAuto, Is.True);
            footnote.ReferenceMark = customMark;
            Assert.That(footnote.IsAuto, Is.False);
            doc.FirstSection.Body.FirstParagraph.AppendChild(footnote);

            Paragraph pa = new Paragraph(doc);
            footnote.Paragraphs.Add(pa);
            pa.Runs.Add(new Run(doc, customMark));
            pa.Runs.Add(new Run(doc, " "));
            pa.Runs.Add(new Run(doc, text));

            string footnoteId = footnote.GetNodeId();
            string texttoCheck = customMark + " " + text;
            // FOSS: doc/odt/wml/rtf writers removed; keep the Docx custom-footnote-mark roundtrip.
            TestDefect11107SaveAndCheck(doc, fileName + ".docx", footnoteId, customMark, texttoCheck);
        }



        private void TestDefect11107SaveAndCheck(Document doc, string fileName, string nodeId, string referenceMark, string text)
        {
            WarningInfoCollection warnings = new WarningInfoCollection();
            doc.WarningCallback = warnings;

            Document resavedDoc = TestUtil.SaveOpen(doc, fileName, null, false);

            if (resavedDoc.OriginalLoadFormat == LoadFormat.Rtf)
                Assert.That(TestUtil.ContainsWarning(warnings, WarningType.DataLoss, WarningSource.Rtf,
                    "RTF format can only store 1 symbol as custom reference mark, other symbols will be truncated."), Is.True);

            Footnote fn = resavedDoc.GetNodeById(nodeId) as Footnote;
            Assert.That(fn, IsNot.Null());
            Assert.That(fn.ReferenceMark, Is.EqualTo(referenceMark));
            Assert.That(fn.FirstParagraph.GetText(), Is.EqualTo(text + "\r"));
        }


        /// <summary>
        /// WORDSNET-13982 Now an exception is thrown, if a document contains several references to the same
        /// footnote/endnote, like MS Word does.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FileCorruptedException))]
        public void TestTwoReferencesToFootnote()
        {
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.RecoveryMode = DocumentRecoveryMode.None;
            TestUtil.Open(@"Model\Footnote\TestTwoReferencesToFootnote.docx", loadOptions);
        }



        /// <summary>
        /// Additional test related to WORDSNET-22954
        /// </summary>
        // FOSS: Doc reader removed; dropped the three .doc cases (the assertion is format-agnostic).
        public void Test22954A(string testName)
        {
            Document doc = TestUtil.Open(testName);
            Assert.That(doc.FootnoteSeparators.IsDefault, Is.True);
        }

        /// <summary>
        /// WORDSNET-24128 InvalidOperationException on DOCX roundtrip.
        /// AW checks that separator has default formatting to make decision whether to write.
        /// Formatting revision attribute has not default value, so use final attribute version to fix the issue.
        /// </summary>
        [Test]
        public void Test24128()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Footnote\Test24128", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.FootnoteSeparators[FootnoteSeparatorType.FootnoteSeparator].IsDefault, Is.False);
        }

        /// <summary>
        /// Tests the <see cref="Footnote.ActualReferenceMark"/> property.
        /// </summary>
        [TestCase("TestActualReferenceMarks.docx", new string[]{"1", "xi", "xii", "1", "2", "6"})]
        [TestCase("TestActualReferenceMarksWithRevisions.docx", new string[] { "1", "2", "3", "4" })]
        public void TestActualReferenceMarks(string fileName, string[] expectedMarks)
        {
            Document doc = TestUtil.Open(@"Model\Footnote\" + fileName);
            List<Footnote> footnotes = doc.GetChildNodes(NodeType.Footnote, true).ToList<Footnote>();

            // ReferenceMark properties are not populated yet.
            Assert.That(footnotes[0].ReferenceMark, Is.EqualTo(string.Empty));

            // Populate actual reference marks.
            doc.UpdateActualReferenceMarks();

            Assert.That(footnotes.Count, Is.EqualTo(expectedMarks.Length));
            for (int i = 0; i < footnotes.Count; i++)
                Assert.That(footnotes[i].ActualReferenceMark, Is.EqualTo(expectedMarks[i]));
        }

        /// <summary>
        /// WORDSNET-28560 Opening of a corrupted document.
        /// Tests a new public API for load document in recovery mode.
        /// </summary>
        [Test]
        public void Test28560()
        {
            const string footnoteText = "\u0002 Canais avulsos sem promoção os valores serão aplicados de " +
                                        "acordo com os Termos e Condições de Uso do Plano. \r";

            WarningInfoCollection warnings = new WarningInfoCollection();
            LoadOptions lo = new LoadOptions();
            lo.RecoveryMode = DocumentRecoveryMode.TryRecover;
            lo.WarningCallback = warnings;

            Document doc = TestUtil.Open(@"Model\Footnote\Test28560.docx", lo);

            Assert.That(TestUtil.ContainsWarning(warnings, WarningType.DataLoss, WarningSource.Docx,
                string.Format(WarningStrings.FootnoteWithSeveralReferences, 5)), Is.True);

            Style footnoteReferenceStyle = doc.Styles.GetByName("Footnote Reference", false);
            NodeCollection footnotes = doc.GetChildNodes(NodeType.Footnote, true);

            Assert.That(footnotes[3].GetText(), Is.EqualTo(footnoteText));
            Assert.That(((Footnote)footnotes[3]).RunPr.Istd, Is.EqualTo(footnoteReferenceStyle.Istd));
            Assert.That(footnotes[4].GetText(), Is.EqualTo(string.Empty));
            Assert.That(((Footnote)footnotes[4]).RunPr.Istd, Is.EqualTo(footnoteReferenceStyle.Istd));
            Assert.That(footnotes[5].GetText(), Is.EqualTo(string.Empty));
            Assert.That(((Footnote)footnotes[5]).RunPr.Istd, Is.EqualTo(footnoteReferenceStyle.Istd));
        }

        /// <summary>
        /// Relates to WORDSNET-28560.
        /// Checks DocumentRecoveryMode.None behavior for the recovery mode.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FileCorruptedException))]
        public void Test28560None()
        {
            LoadOptions lo = new LoadOptions();
            lo.RecoveryMode = DocumentRecoveryMode.None;

            // Default value LoadOptions.RecoveryMode is equal to DocumentRecoveryMode.None.
            // FileCorruptedException is raised here.
            TestUtil.Open(@"Model\Footnote\Test28560.docx", lo);
        }
    }
}
