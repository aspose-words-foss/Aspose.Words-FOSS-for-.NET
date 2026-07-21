// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/06/2020 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Tests export emphases to markdown format.
    /// </summary>
    [TestFixture]
    public class TestExportMarkdownEmphases : TestMarkdownBase
    {
        /// <summary>
        /// Tests when opening and closing delimiters of different emphases are messed. 
        /// </summary>
        [Test]
        public void TestExportEmphasesA()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesA", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            
            Assert.That(paragraph.GetText(), Is.EqualTo("1234\f"));
            CheckRun(paragraph.Runs[0], "1", true, false);
            CheckRun(paragraph.Runs[1], "2", true, true);
            CheckRun(paragraph.Runs[2], "3", false, true);
            CheckRun(paragraph.Runs[3], "4", false, false);
        }

        /// <summary>
        /// Tests emphases with line break. 
        /// </summary>
        [Test]
        public void TestExportEmphasesB()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesB", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("12\v34\f"));
            CheckRun(paragraph.Runs[0], "1", true, false);
            CheckRun(paragraph.Runs[1], "2", true, true);
            CheckRun(paragraph.Runs[2], "\v", false, false);
            CheckRun(paragraph.Runs[3], "3", false, true);
            CheckRun(paragraph.Runs[4], "4", false, false);
        }

        /// <summary>
        /// Tests when Bold and Italic are opened together, but closed at different places. 
        /// </summary>
        [Test]
        public void TestExportEmphasesC()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesC", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("12\f"));
            CheckRun(paragraph.Runs[0], "1", true, true);
            CheckRun(paragraph.Runs[1], "2", false, true);
        }

        /// <summary>
        /// Tests when Bold and Italic are opened together, but closed at different places
        /// and one of them is closed just before LineBreak. 
        /// </summary>
        [Test]
        public void TestExportEmphasesD()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesD", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("123\v4\f"));
            CheckRun(paragraph.Runs[0], "12", true, true);
            CheckRun(paragraph.Runs[1], "3", false, true);
            CheckRun(paragraph.Runs[2], "\v4", false, false);
        }

        /// <summary>
        /// Tests when all delimiters are intraword.  
        /// </summary>
        [Test]
        public void TestExportEmphasesE()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesE", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("01234\f"));
            CheckRun(paragraph.Runs[0], "0", false, false);
            CheckRun(paragraph.Runs[1], "12", true, true);
            CheckRun(paragraph.Runs[2], "3", false, true);
            CheckRun(paragraph.Runs[3], "4", false, false);
        }

        /// <summary>
        /// Tests emphases are closed not in the same order as was opened, so that one of them (Bold)
        /// is located inside a closed one.
        /// </summary>
        [Test]
        public void TestExportEmphasesF()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesF", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("1234\f"));
            CheckRun(paragraph.Runs[0], "1", false, true);
            CheckRun(paragraph.Runs[1], "2", true, true);
            CheckRun(paragraph.Runs[2], "3", true, false);
            CheckRun(paragraph.Runs[3], "4", false, false);
        }

        /// <summary>
        /// Tests warning is issued when formatting cannot be preserved in markdown properly. 
        /// This is the case when emphases are closed not in the same order as was opened, so that one of them (Italic)
        /// is located inside another one that is closed. As the emphases are intraword, they cannot be changed to
        /// underscores and therefore cannot be written into markdown properly.
        /// </summary>
        [Test]
        public void TestExportEmphasesG()
        {
            Document doc = TestUtil.Open(@"ExportMarkdown\Emphases\TestExportEmphasesG.docx");

            TestWarningCallback warnings = new TestWarningCallback();
            doc.WarningCallback = warnings;

            doc = TestUtil.SaveOpen(doc, @"ExportMarkdown\Emphases\TestExportEmphasesG", UnifiedScenario.Docx2Md);
            
            Assert.That(warnings.Contains(WarningSource.Markdown, WarningType.MajorFormattingLoss,
                string.Format(WarningStrings.MarkdownFormattingLost, "*, 0:11")), Is.True);
            
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            
            Assert.That(paragraph.GetText(), Is.EqualTo("abcde\f"));
            CheckRun(paragraph.Runs[0], "a", false, false, true);
            CheckRun(paragraph.Runs[1], "b", true, false, true);
            CheckRun(paragraph.Runs[2], "c", true, true, true);
            CheckRun(paragraph.Runs[3], "de", false, false, true);
        }

        /// <summary>
        /// Tests Bold emphasis default delimiter is changed to '__' at the place where also strikethrough is opening.  
        /// </summary>
        [Test]
        public void TestExportEmphasesH()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesH", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("1234\f"));
            CheckRun(paragraph.Runs[0], "1", true, false);
            CheckRun(paragraph.Runs[1], "2", true, true);
            CheckRun(paragraph.Runs[2], "3", false, true, true);
            CheckRun(paragraph.Runs[3], "4", false, false, true);
        }

        /// <summary>
        /// Tests when all three types of emphases are messed (Bold, Italic and StrikeThrough). 
        /// </summary>
        [Test]
        public void TestExportEmphasesI()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesI", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("01234\f"));
            CheckRun(paragraph.Runs[0], "0", false, false);
            CheckRun(paragraph.Runs[1], "1", true, false);
            CheckRun(paragraph.Runs[2], "2", true, false, true);
            CheckRun(paragraph.Runs[3], "3", false, false, true);
            CheckRun(paragraph.Runs[4], "4", false, false);
        }

        /// <summary>
        /// Tests when all three types of emphases are messed (Bold, Italic and StrikeThrough),
        /// the same as above, but in another order.
        /// </summary>
        [Test]
        public void TestExportEmphasesJ()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesJ", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("12345\f"));
            CheckRun(paragraph.Runs[0], "1", true, false);
            CheckRun(paragraph.Runs[1], "2", true, false, true);
            CheckRun(paragraph.Runs[2], "3", true, true, true);
            CheckRun(paragraph.Runs[3], "4", false, true, true);
            CheckRun(paragraph.Runs[4], "5", false, true);
        }

        /// <summary>
        /// Tests various emphases with InlineCode in the middle of a text. 
        /// </summary>
        [Test]
        public void TestExportEmphasesK()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesK", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("123z45\f"));
            CheckRun(paragraph.Runs[0], "1", true, false);
            CheckRun(paragraph.Runs[1], "2", true, false, true);
            CheckRun(paragraph.Runs[2], "3", true, true, true);
            CheckRun(paragraph.Runs[3], "z", true, true, true);
            CheckRun(paragraph.Runs[4], "4", false, true, true);
            CheckRun(paragraph.Runs[5], "5", false, true);
        }

        /// <summary>
        /// Tests various emphases messed and Bold changes its delimiter to '__' properly before space.   
        /// </summary>
        [Test]
        public void TestExportEmphasesL()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ExportMarkdown\Emphases\TestExportEmphasesL", UnifiedScenario.Docx2Md);
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;

            Assert.That(paragraph.GetText(), Is.EqualTo("123 45\f"));
            CheckRun(paragraph.Runs[0], "1", true, false);
            CheckRun(paragraph.Runs[1], "2", true, false, true);
            CheckRun(paragraph.Runs[2], "3", true, true, true);
            CheckRun(paragraph.Runs[3], " ", false, false);
            CheckRun(paragraph.Runs[4], "4", false, true, true);
            CheckRun(paragraph.Runs[5], "5", false, true);
        }

    }
}
