// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Drawing;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Framesets;
using Aspose.Words.Loading;
using Aspose.Words.RW.Factories;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unsorted unified tests. At some stage should look at them and move into appropriate places.
    /// </summary>
    [TestFixture]
    public class TestOther : UnifiedTestsBase
    {
        /// <summary>
        /// Check that output contains generator name by default.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGeneratorName(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            SaveOptions opt = SaveOptions.CreateSaveOptions(sf);
            Assert.That(opt.ExportGeneratorName, Is.EqualTo(true));
            opt.PrettyFormat = true;
            opt.SetBuiltInThemeIfNull = false;
            TestUtil.SaveOpen(doc, @"Model\Other\TestGeneratorName", TestUtil.GetUnifiedScenario(lf, sf), opt);
        }

        /// <summary>
        /// Check that the document has no pretty format by default.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNoPrettyFormat(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            SaveOptions opt = SaveOptions.CreateSaveOptions(sf);
            opt.ExportGeneratorName = false;
            opt.SetBuiltInThemeIfNull = false;
            Assert.That(opt.PrettyFormat, Is.EqualTo(false));
            TestUtil.SaveOpen(doc, @"Model\Other\TestNoPrettyFormat", TestUtil.GetUnifiedScenario(lf, sf), opt);
        }









        /// <summary>
        /// WORDSNET-761 Support subdocuments.
        /// </summary>
        /// <remarks>
        /// AM. There is some problem to get normal unified testing.
        /// When Word saved original document into another format it re-saves subdocuments to the same format and
        /// also changes file links in master document. So gold comparsion doesn't work.
        /// </remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedSubDocuments(LoadFormat lf, SaveFormat sf)
        {
            const string masterFile = @"Model\SubDocument\Master document";
            Document doc = TestUtil.Open(masterFile, lf);

            Assert.That(doc.Sections.Count, Is.EqualTo(3));

            // First subdocument is referred by absolute filename.
            VerifyDirectoryAndFileName(doc, 0, TestEnvironment.GetUserHome() + "Documents", "Absolute referred subdocument");

            // Second subdocument is referred by relative filename, converted to absolute on load.
            VerifyDirectoryAndFileName(doc, 1, TestEnvironment.GetTestData() + @"Model\SubDocument", "Relative referred subdocument");

            // Document is saved to another directory.
            string savedMasterFile = TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(masterFile), "", sf);
            doc.Save(savedMasterFile);
            doc = TestUtil.Open(savedMasterFile);

            // First subdocument remains absolute referred.
            VerifyDirectoryAndFileName(doc, 0, TestEnvironment.GetUserHome() + "Documents", "Absolute referred subdocument");

            // Second subdocument is now referred by absolute filename.
            VerifyDirectoryAndFileName(doc, 1, TestEnvironment.GetTestData() + @"Model\SubDocument", "Relative referred subdocument");

            // Make document from scratch.
            const string newFile = @"Model\SubDocument\New document";

            doc = new Document();
            Body body = doc.AppendChild(new Section(doc)).AppendChild(new Body(doc));
            Paragraph para = body.AppendParagraph("Document with subdocuments");
            para = body.AppendChild(new Paragraph(doc));
            para.AppendChild(new SubDocument(doc, @"X:\Invalid.doc"));
            para = body.AppendChild(new Paragraph(doc));
            para.AppendChild(new SubDocument(doc, @":\not a file name at all/:"));

            string savedNewFile = TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(newFile), "", sf);
            doc.Save(savedNewFile);
            doc = TestUtil.Open(savedNewFile);

            VerifyDirectoryAndFileName(doc, 0, @"X:\", "Invalid");
            // This throws in VerifyDirectoryAndFileName so check manually.
            Assert.That(((SubDocument)doc.GetChildNodes(NodeType.SubDocument, true)[1]).FileName, Is.EqualTo(@":\not a file name at all/:"));
        }

        /// <summary>
        /// WORDSNET-5320 Fix locale reading.
        /// Verifies languages information is read/written properly.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedLanguages(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Other\TestLanguages", lf, sf);

            // Verify styles.
            VerifyLocales(doc.Styles.GetByName("StyleKorean", false).RunPr, Language.EnglishUS, Language.KoreanKorea, false);
            VerifyLocales(doc.Styles.GetByName("StyleKoreanNoProof", false).RunPr, Language.NoProof, Language.KoreanKorea, true);

            VerifyLocales(doc.Styles.GetByName("StyleRussian", false).RunPr, Language.RussianRussia, Language.NoProof, false);
            VerifyLocales(doc.Styles.GetByName("StyleRussianNoProof", false).RunPr, Language.RussianRussia, Language.NoProof, true);

            VerifyLocales(((Run)doc.GetNodeById("0.0.0.0")).RunPr, Language.FrenchFrance, Language.NoProof, false);
            VerifyLocales(((Run)doc.GetNodeById("0.1.0.0")).RunPr, Language.FrenchFrance, Language.NoProof, true);

            VerifyLocales(((Run)doc.GetNodeById("0.4.0.0")).RunPr, Language.EnglishUS, Language.KoreanKorea, false);
            VerifyLocales(((Run)doc.GetNodeById("0.5.0.0")).RunPr, Language.EnglishUS, Language.KoreanKorea, true);
        }


        // FOSS: Test21179 removed — it verified non-seekable-stream saving for Doc/Rtf/WordML, all of which
        // were removed. The surviving Docx non-seek case is covered by Test21179DocxGoldCheck.


        // FOSS: TestUpdatingFrame / TestUpdatingFrameDefaultUrlOfFramesPage /
        // TestUpdatingIsFrameLinkToFileOfFramesPage removed — framesets can only be loaded from .doc or
        // HTML, both removed in FOSS, so the frameset scenarios cannot be constructed.


        /// <summary>
        /// WORDSNET-24982 PPTX document is wrongly loaded into Document as Text document.
        /// Word recognizes ZIP signature and if recognized, then does not fall back to TXT format,
        /// when it further cannot be properly recognized as one of the known Word formats.
        /// </summary>
        [Test, ExpectedException(typeof(UnsupportedFileFormatException))]
        public void Test24982()
        {
            Document doc = TestUtil.Open(@"Other\Test24982.pptx");
            Assert.That(doc, Is.Null);
        }

        /// <summary>
        /// WORDSNET-25109 ZIP archive is loaded as TXT document into Aspose.Words.
        /// Duplicates WORDSNET-24982
        /// </summary>
        [Test, ExpectedException(typeof(UnsupportedFileFormatException))]
        public void Test25109()
        {
            Document doc = TestUtil.Open(@"Other\Test25109.zip");
            Assert.That(doc, Is.Null);
        }

        /// <summary>
        /// WORDSNET-25454 File is mistakenly detected as PDF by Aspose.Words.
        /// We should check PDF file trailer '%%EOF' along with '%PDF-' signature in header
        /// to filter out non-valid PDF files.
        /// </summary>
        [Test]
        public void Test25454()
        {
            Document doc = TestUtil.Open(@"Other\Test25454.mso");
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(LoadFormat.Text));
        }

        /// <summary>
        /// Relates to WORDSNET-25454
        /// Another file that is successfully opened by Word as TXT format.
        /// </summary>
        [Test]
        public void Test25454A()
        {
            Document doc = TestUtil.Open(@"Other\Test25454A.mso");
            Assert.That(doc.OriginalLoadFormat, Is.EqualTo(LoadFormat.Text));
        }

        /// <summary>
        /// WORDSNET-25512 Check whether customXml part is required in AllStyles2003.docx resource.
        /// Checks whether CustomXml parts are absent in AllStyles2003.docx and AllStyles2007.docx resource documents.
        /// </summary>
        [Test]
        public void Test25512()
        {
            CheckCustomXmlPartsIsEmpty("Aspose.Words.Resources.AllStyles2003.docx");
            CheckCustomXmlPartsIsEmpty("Aspose.Words.Resources.AllStyles2007.docx");
        }

        /// <summary>
        /// WORDSNET-27211 Corrupted DOCX document is loaded as TXT if load from stream.
        /// Don't fallback to TXT when format is set explicitly and auto-detection is failed with returning format 'Unknown'.
        /// </summary>
        [Test, ExpectedException(typeof(UnsupportedFileFormatException))]
        public void Test27211()
        {
            string fileName = TestUtil.BuildTestFileName(@"Other\Test27211.docx");
            byte[] docBytes = File.ReadAllBytes(fileName);
            using (MemoryStream stream = new MemoryStream(docBytes))
            {
                LoadOptions options = new LoadOptions() { LoadFormat = LoadFormat.Docx };
                Document doc = new Document(stream, options);
                Assert.That(doc.OriginalLoadFormat, Is.EqualTo(LoadFormat.Unknown));
            }
        }

        /// <summary>
        /// WORDSNET-26255 Corrupted file is loaded as TXT by Aspose.Words.
        /// Added weight to long strings in <see cref="FileFormatDetector.DetectPlainText"/>.
        /// The longer the string, the more weight it has for now.
        /// </summary>
        [Test, ExpectedException(typeof(UnsupportedFileFormatException))]
        public void Test26255()
        {
            Document doc = TestUtil.Open(@"Other\Test26255.docx");
            Assert.That(doc, Is.Null);
        }

        public static IEnumerable<TestCaseData> UnifiedResourceLoadingCallbackCases
        {
            get
            {
                List<TestCaseData> cases = new List<TestCaseData>();
                // FOSS: only Docx survives — Doc/Rtf/WordML load were removed.
                cases.Add(new TestCaseData(LoadFormat.Docx, new UserProvidedDataHandler()));
                return cases;
            }
        }

        /// <summary>
        /// Helper method for UnifiedLanguages test.
        /// </summary>
        private static void VerifyLocales(RunPr runPr, Language localeId, Language localeIdFarEast, bool noProof)
        {
            Assert.That((Language)runPr.LocaleId, Is.EqualTo(localeId));
            Assert.That((Language)runPr.LocaleIdFarEast, Is.EqualTo(localeIdFarEast));
            Assert.That(runPr.NoProofing.ToBool(), Is.EqualTo(noProof));
        }

        /// <summary>
        /// Verifies path and name without file extension.
        /// </summary>
        private static void VerifyDirectoryAndFileName(Document doc, int index, string directoryName, string fileName)
        {
            SubDocument subDocument = (SubDocument)doc.GetChildNodes(NodeType.SubDocument, true)[index];

            Assert.That(Path.GetFileNameWithoutExtension(subDocument.FileName), Is.EqualTo(fileName));
            Assert.That(Path.GetDirectoryName(subDocument.FileName), Is.EqualTo(directoryName));
        }

        private static void CheckCustomXmlPartsIsEmpty(string resourceName)
        {
            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                LoadOptions lo = new LoadOptions();
                lo.LoadFormat = LoadFormat.Docx;
                Document doc = new Document(stream, lo);

                Assert.That(doc.CustomXmlParts.Count, Is.EqualTo(0));
            }
        }
    }
}
