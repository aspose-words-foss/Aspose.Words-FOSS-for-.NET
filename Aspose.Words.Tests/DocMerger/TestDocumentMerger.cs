// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2022 by Ilya Navrotskiy

using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Notes;
using NUnit.Framework;

namespace Aspose.Words.Tests.DocMerger
{
    /// <summary>
    /// Class for testing issues related to DocumentMerger.
    /// </summary>
    public class TestDocumentMerger
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// Tests merging with an empty list of source documents.
        /// </summary>
        [Test]
        public void TestEmpty()
        {
            Document doc = new Document();
            doc.MergeDocuments();

            Assert.That(doc.GetText(), Is.EqualTo("\f"));
        }












        /// <summary>
        /// WORDSNET-26045 BiDi text is shown incorrectly in Header after Merge DOCX.
        /// Update SDT content BiDi aware in <see cref="Aspose.Words.Markup.SdtContentHelper.InsertInlineNode"/>
        /// that is used in <see cref="DocumentMerger.UpdateCorePropertiesSdts"/>.
        /// </summary>
        [Test]
        public void Test26045()
        {
            Document mergedDoc = MergeDocs("Test26045Dst.docx", "Test26045Src.docx");
            mergedDoc = TestUtil.SaveOpen(mergedDoc, @"Model\DocMerger\Test26045Merged", UnifiedScenario.Docx2Docx);

            HeaderFooter hf = mergedDoc.LastSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Assert.That(hf.FirstParagraph.GetText(), Is.EqualTo("الفصل الأول:                                                       الأسلوب الانشائي مفهومه وأقسامه\r"));
        }


        /// <summary>
        /// WORDSNET-26046 Wrong footnote separator alignment after Merge DOCX.
        /// Do the trick and if there are no footnotes in destination document,
        /// then replace footnote separator formatting with the one from the source document.
        /// </summary>
        [Test]
        public void Test26046()
        {
            Document mergedDoc = MergeDocs("Test26046Dst.docx", "Test26046Src.docx");
            mergedDoc = TestUtil.SaveOpen(mergedDoc, @"Model\DocMerger\Test26046Merged", UnifiedScenario.Docx2Docx);

            Paragraph para = (Paragraph)mergedDoc.FootnoteSeparators[FootnoteSeparatorType.FootnoteSeparator].FirstChild;
            Assert.That(para.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Right));
        }

        /// <summary>
        /// Relates to WORDSNET-26046.
        /// Checks when destination document has footnote.
        /// </summary>
        [Test]
        public void Test26046A()
        {
            Document mergedDoc = MergeDocs("Test26046DstA.docx", "Test26046SrcA.docx");
            mergedDoc = TestUtil.SaveOpen(mergedDoc, @"Model\DocMerger\Test26046AMerged", UnifiedScenario.Docx2Docx);

            Paragraph para = (Paragraph)mergedDoc.FootnoteSeparators[FootnoteSeparatorType.FootnoteSeparator].FirstChild;
            Assert.That(para.ParagraphFormat.Alignment, Is.EqualTo(ParagraphAlignment.Left));
        }



        /// <summary>
        /// Relates to WORDSNET-25831.
        /// Tests when destination and source document sections starts page numbering not from 1.
        /// </summary>
        [Test]
        public void Test25831A()
        {
            Document mergedDoc = MergeDocs("Test25831A_1.docx", "Test25831A_2.docx");
            mergedDoc = TestUtil.SaveOpen(mergedDoc, @"Model\DocMerger\Test25831AMerged", UnifiedScenario.Docx2Docx);

            Assert.That(mergedDoc.Sections[0].PageSetup.PageStartingNumber, Is.EqualTo(3));
            Assert.That(mergedDoc.Sections[1].PageSetup.PageStartingNumber, Is.EqualTo(8));
            Assert.That(mergedDoc.Sections[1].PageSetup.RestartPageNumbering, Is.True);
        }







        /// <summary>
        /// WORDSNET-26815 MergeDocuments throws exception for empty document.
        /// Need a resilience against completely empty destination document.
        /// </summary>
        [Test]
        public void Test26815()
        {
            Document srcDoc = new Document();
            Document dstDoc = (Document)srcDoc.Clone(false);

            dstDoc.MergeDocuments(srcDoc);

            Assert.That(dstDoc.GetText(), Is.EqualTo("\f"));
        }












        /// <summary>
        /// The core helper method to test WORDSNET-28311.
        /// </summary>
        private static void Test28311Core(HeaderFooter footer, string expectedText)
        {
            FieldCollection fields = footer.Range.Fields;
            // There must be only field `Page`. And a second field `NumPages` should be converted to a Run result.
            Assert.That(fields.Count, Is.EqualTo(1));
            Assert.That(fields[0].Type, Is.EqualTo(FieldType.FieldPage));

            Paragraph paragraph = footer.FirstParagraph;
            Assert.That(paragraph.GetText(), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Returns merged document.
        /// </summary>
        private static Document MergeDocs(string dstTestName, params string[] srcTestNames)
        {
            Document dstDoc = OpenTestDoc(dstTestName);

            Document[] srcDocs = new Document[srcTestNames.Length];
            for (int i = 0; i < srcTestNames.Length; i++)
            {
                string srcTestName = srcTestNames[i];
                srcDocs[i] = OpenTestDoc(srcTestName);
            }

            dstDoc.MergeDocuments(srcDocs);

            return dstDoc;
        }

        /// <summary>
        /// Returns test document with a specified file name.
        /// </summary>
        private static Document OpenTestDoc(string fileName)
        {
            return TestUtil.Open(GetTestPath(fileName));
        }

        /// <summary>
        /// Returns path relative to 'Model\DocMerger'.
        /// </summary>
        private static string GetTestPath(string filename)
        {
            return Path.Combine(@"Model\DocMerger\", filename);
        }

        /// <summary>
        /// Verifies HeadersFooters against gold.
        /// </summary>
        private static void VerifyGoldHfs(HeaderFooterCollection headersFooters, HeaderFooterCollection goldHeadersFooters)
        {
            Assert.That(headersFooters.Count, Is.EqualTo(goldHeadersFooters.Count));
            foreach (HeaderFooter hf in headersFooters)
            {
                HeaderFooter goldHf = goldHeadersFooters[hf.HeaderFooterType];
                Assert.That(hf.IsLinkedToPrevious, Is.EqualTo(goldHf.IsLinkedToPrevious));
                Assert.That(hf.GetText(), Is.EqualTo(goldHf.GetText()));
            }
        }

        /// <summary>
        /// Verifies most common properties of all sections of a specified document against gold document.
        /// </summary>
        private static void VerifyGoldSections(Document doc, Document goldDoc)
        {
            Assert.That(doc.Sections.Count, Is.EqualTo(goldDoc.Sections.Count));

            for (int i = 0; i < doc.Sections.Count; i++)
                VerifyGoldSection(doc.Sections[i], goldDoc.Sections[i], string.Format("Section {0}: \n", i));
        }

        /// <summary>
        /// Verifies section most common properties against gold.
        /// </summary>
        private static void VerifyGoldSection(Section section, Section goldSection, string msg = "")
        {
            Assert.That(section.Body.GetText(), Is.EqualTo(goldSection.Body.GetText()));
            VerifyGoldHfs(section.HeadersFooters, goldSection.HeadersFooters);
            VerifyGoldSectPr(section.SectPr, goldSection.SectPr, msg);
        }

        /// <summary>
        /// Verifies that specified section properties are equal to specified gold properties,
        /// ignoring difference in global defaults.
        /// </summary>
        private static void VerifyGoldSectPr(SectPr sectPr, SectPr goldSectPr, string msg = "")
        {
            SectPr extraAttrs = sectPr.Clone();
            extraAttrs.RemoveEquals(goldSectPr);
            extraAttrs.RemoveGlobalDefaults();

            SectPr missingAttrs = goldSectPr.Clone();
            missingAttrs.RemoveEquals(sectPr);
            missingAttrs.RemoveGlobalDefaults();

            StringBuilder sb = null;
            if (extraAttrs.Count > 0)
            {
                sb = new StringBuilder();
                sb.AppendLine("Extra or changed attributes found in SectPr, which are not exist in Gold:");
                sb.AppendLine(extraAttrs.ToString());
            }

            if (missingAttrs.Count > 0)
            {
                if (sb == null)
                    sb = new StringBuilder();

                sb.AppendLine("Missing or changed attributes found in SectPr, which are exist in Gold:");
                sb.AppendLine(missingAttrs.ToString());
            }

            if (sb != null)
                Assert.Fail(string.Format("{0}{1}", msg, sb));
        }

        /// <summary>
        /// Verifies most common properties of all sections of a specified document against gold document.
        /// </summary>
        private static void VerifyGoldSections(Document doc)
        {
            string testFileName = Path.GetFileNameWithoutExtension(doc.OriginalFileName.Replace("Dst", ""));
            string testExt = Path.GetExtension(doc.OriginalFileName);

            string outFileName = GetTestPath(string.Format("{0} Out{1}", testFileName, testExt));
            string outPath = TestUtil.GetInTestOutPath(outFileName);

            TestUtil.Save(doc, outPath);
            Document outDoc = TestUtil.Open(outPath);

            string goldFileName = GetTestPath(string.Format("{0} Gold{1}", testFileName, testExt));
            Document goldDoc = TestUtil.Open(TestUtil.GetInTestGoldPath(goldFileName));

            VerifyGoldSections(outDoc, goldDoc);
        }
    }
}
