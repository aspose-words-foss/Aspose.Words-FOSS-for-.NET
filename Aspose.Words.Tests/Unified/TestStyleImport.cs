// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Tests related to importing of styles between different documents.
    /// </summary>
    [TestFixture]
    public class TestStyleImport : UnifiedTestsBase
    {
        /// <summary>
        /// Test importing of paragraphs and runs that creates new styles in the destination document.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestImportStyle_UseDestinationStyles(LoadFormat lf, SaveFormat sf)
        {
            Document dst = DoTestImportStyle(ImportFormatMode.UseDestinationStyles, lf, sf);

            //These styles were not created because destination styles were used instead.
            Assert.That(dst.Styles["Normal_0"], Is.Null);
            Assert.That(dst.Styles["Default Paragraph Format_0"], Is.Null);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestImportStyle_KeepSourceFormatting(LoadFormat lf, SaveFormat sf)
        {
            Document dst = DoTestImportStyle(ImportFormatMode.KeepSourceFormatting, lf, sf);

            // Make sure Normal style is not copied.
            Assert.That(dst.Styles["Normal_0"], Is.Null);

            // Make sure DefaultParagraphFont style is not copied.
            // There is no need to because it cannot be modified.
            // Also, if we copy it and try to modify in Microsoft Word, it crashes.
            Assert.That(dst.Styles["Default Paragraph Font_0"], Is.Null);

            //This built in style was copied.
            Style style = dst.Styles["Heading 4"];
            Assert.That(style, IsNot.Null());
        }

        private static Document DoTestImportStyle(ImportFormatMode importFormatMode, LoadFormat lf, SaveFormat sf)
        {
            Document dst = new Document();
            Document src = TestUtil.OpenSaveOpen(@"Model\Style\TestImportStyle", lf, sf);

            Paragraph srcPara = src.FirstSection.Body.Paragraphs[0];
            Assert.That(srcPara.ParagraphBreakFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

            dst.Sections.Clear();
            dst.Sections.Add(dst.ImportNode(src.Sections[0], true, importFormatMode));
            TestUtil.SaveOpen(dst, string.Format(@"Model\Style\TestImportStyle_{0}", ImportFormatModeToStr(importFormatMode)), lf, sf);
            ParagraphCollection paras = dst.FirstSection.Body.Paragraphs;
            if (importFormatMode == ImportFormatMode.KeepSourceFormatting)
            {
                // Heading 4 source style should be expanded into the direct attributes and removed from the paragraph,
                // when 'KeepSourceFormatting' mode is used.
                Assert.That(paras[0].ParaPr[ParaAttr.Istd], Is.Null);

                // Importing this node expands Title and Hyperlink properties into the direct formatting,
                // when 'KeepSourceFormatting' mode is used.
                Assert.That(paras[1].ParaPr[ParaAttr.Istd], Is.Null);
                Assert.That(paras[1].FirstRun.RunPr[FontAttr.Istd], Is.Null);

                // Importing this node expands MyStyle, MyCharStyle, Signature and Heading 9 properties into direct formatting.
                Assert.That(paras[2].ParaPr[ParaAttr.Istd], Is.Null);
                Assert.That(paras[2].Runs[1].RunPr[FontAttr.Istd], Is.Null);
            }
            else
            {
                Assert.That(paras[0].ParaPr[ParaAttr.Istd], Is.EqualTo(dst.Styles["Heading 4"].Istd));

                Assert.That(paras[1].ParaPr[ParaAttr.Istd], Is.EqualTo(dst.Styles["Title"].Istd));
                Assert.That(paras[1].FirstRun.RunPr[FontAttr.Istd], Is.EqualTo(dst.Styles["Hyperlink"].Istd));

                Assert.That(paras[2].ParaPr[ParaAttr.Istd], Is.EqualTo(dst.Styles["MyStyle"].Istd));
                Assert.That(paras[2].Runs[1].RunPr[FontAttr.Istd], Is.EqualTo(dst.Styles["MyCharStyle"].Istd));
            }

            Assert.That(paras[0].ParagraphBreakFont.ColorInternal, Is.EqualTo(DrColor.Red));

            Assert.That(paras[1].ParagraphBreakFont.ColorInternal, Is.EqualTo(DrColor.Blue));
            Assert.That(paras[1].FirstRun.Font.Underline, Is.EqualTo(Underline.DottedHeavy));

            Assert.That(paras[2].ParagraphBreakFont.ColorInternal, Is.EqualTo(DrColor.Green));
            Assert.That(paras[2].Runs[1].Font.Bold, Is.True);

            return dst;
        }

        /// <summary>
        /// Must do this instead of enum.ToString because on Java this is just an integer.
        /// </summary>
        private static string ImportFormatModeToStr(ImportFormatMode ifm)
        {
            switch (ifm)
            {
                case ImportFormatMode.KeepSourceFormatting:
                    return "KeepSourceFormatting";
                case ImportFormatMode.UseDestinationStyles:
                    return "UseDestinationStyles";
                default:
                    throw new InvalidOperationException("Unknown import format mode.");
            }
        }

        /// <summary>
        /// Test importing some nodes when the style already exists. The references to the
        /// style must be correctly updated, but the style in the destination document is not modified.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestImportStyleExists_UseDestinationStyles(LoadFormat lf, SaveFormat sf)
        {
            Document dst = TestUtil.OpenSaveOpen(@"Model\Style\TestImportStyleExistsDst", lf, sf);
            Document src = TestUtil.OpenSaveOpen(@"Model\Style\TestImportStyleExistsSrc", lf, sf);

            //This is a simple case where translation of istd is not required.
            //Heading 1 is black in the destination
            Assert.That(dst.Styles["Heading 1"].Font.Color.ToArgb(), Is.EqualTo(0));

            //Paragraph is red Heading 1 in the source.
            Paragraph srcPara = src.FirstSection.Body.Paragraphs[0];
            Assert.That(srcPara.ParagraphFormat.StyleName, Is.EqualTo("Heading 1"));
            Assert.That(srcPara.ParagraphBreakFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

            //When imported, Heading 1 is not changed (black in the destination).
            Paragraph dstPara = (Paragraph)dst.ImportNode(srcPara, true);
            Assert.That(dstPara.ParagraphFormat.StyleName, Is.EqualTo("Heading 1"));
            Assert.That(dstPara.ParagraphBreakFont.Color.ToArgb(), Is.EqualTo(0));
            dst.FirstSection.Body.AppendChild(dstPara);


            //Another case when a built in paragraph and character styles already exists in the destination,
            //but istds must be translated.
            //Verify istds are initially different.
            Assert.That(dst.Styles["Title"].Istd, Is.EqualTo(15));
            Assert.That(dst.Styles["Hyperlink"].Istd, Is.EqualTo(17));
            Assert.That(src.Styles["Title"].Istd, Is.EqualTo(20));
            Assert.That(src.Styles["Hyperlink"].Istd, Is.EqualTo(22));

            //Check Title and Hyperlink in the source
            srcPara = src.FirstSection.Body.Paragraphs[1];
            Assert.That(srcPara.ParagraphFormat.StyleName, Is.EqualTo("Title"));
            Assert.That(srcPara.ParagraphBreakFont.Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
            Assert.That(srcPara.Runs[0].Font.StyleName, Is.EqualTo("Hyperlink"));
            Assert.That(srcPara.Runs[0].Font.Underline, Is.EqualTo(Underline.Single));

            //When imported, it remains Title and Hyperlink and the styles are not changed
            //in the destination, this however required istd of the paragraph and run properties translated.
            dstPara = (Paragraph)dst.ImportNode(srcPara, true);
            Assert.That(dstPara.ParagraphFormat.StyleName, Is.EqualTo("Title"));
            Assert.That(dstPara.ParagraphBreakFont.Color.ToArgb(), Is.EqualTo(0));
            Assert.That(dstPara.Runs[0].Font.StyleName, Is.EqualTo("Hyperlink"));
            Assert.That(dstPara.Runs[0].Font.Underline, Is.EqualTo(Underline.Double));
            dst.FirstSection.Body.AppendChild(dstPara);


            //Test of a user defined paragraph style. It's green in the source document.
            srcPara = src.FirstSection.Body.Paragraphs[2];
            Assert.That(srcPara.ParagraphFormat.StyleName, Is.EqualTo("MyStyle"));
            Assert.That(srcPara.ParagraphBreakFont.ColorInternal, Is.EqualTo(DrColor.Green));

            //When imported, it remains MyStyle and the style is not changed in the destination.
            dstPara = (Paragraph)dst.ImportNode(srcPara, true);
            Assert.That(dstPara.ParagraphFormat.StyleName, Is.EqualTo("MyStyle"));
            Assert.That(dstPara.ParagraphBreakFont.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            dst.FirstSection.Body.AppendChild(dstPara);

            TestUtil.SaveOpen(dst, @"Model\Style\TestImportStyleExists", lf, sf);
        }


        private static void CheckLinkedHeading1(Document doc)
        {
            Assert.That(doc.Styles["Heading 1"].Istd, Is.EqualTo(1));
            Assert.That(doc.Styles["Heading 1 Char"].Istd, Is.EqualTo(15));
            Assert.That(doc.Styles["Heading 1 Char"].LinkedIstd, Is.EqualTo(1));
        }

        /// <summary>
        /// WORDSNET-3658 "Cannot add a style because a style with the same name already exists”
        /// exception occurs during inserting one document into another.
        /// RK The problem occurs when destination document contains a user defined style with a name
        /// of a built in style.
        /// </summary>
        [Test]
        public void TestDefect3658()
        {
            // Destination document with a user defined style with a name of a built in style.
            Document dstDoc = new Document();
            dstDoc.Styles.Add(StyleType.Paragraph, "Balloon Text");
            dstDoc.RemoveAllChildren();

            // Source document with a built in style.
            Document srcDoc = TestUtil.Open(@"Model\Style\TestDefect3658.docx");

            // There was an exception during import.
            foreach (Section srcSection in srcDoc.Sections)
            {
                Node dstSection = dstDoc.ImportNode(srcSection, true, ImportFormatMode.KeepSourceFormatting);
                dstDoc.Sections.Add(dstSection);
            }

            Paragraph para = (Paragraph)dstDoc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.GetText(), Is.EqualTo("Balloon Text\x000c"));
            // A new user defined style is created, because the destination document already contains a style with this name.
            // WORDSNET-14587 All properties are expanded into direct paragraph formatting
            // and style of paragraph is changed to Normal.
            Assert.That(para.ParagraphFormat.StyleName, Is.EqualTo("Normal"));
            Assert.That(para.ParagraphFormat.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));

            TestUtil.Save(dstDoc, @"Model\Style\TestDefect3658.docx");
        }





        /// <summary>
        /// WORDSNET-5736 InvalidOperationException: occurs when appending two documents
        /// </summary>
        [Test]
        public void TestDefect5736()
        {
            Document doc = new Document();
            Document doc2 = TestUtil.Open(@"Model\Style\TestDefect5736.docx");

            doc.AppendDocument(doc2, ImportFormatMode.KeepSourceFormatting);

            TestUtil.SaveOpen(doc, @"Model\Style\TestDefect5736.docx");
        }


        /// <summary>
        /// WORDSNET-6842 Style is imported incorrectly in destination document during appending documents.
        /// andrnosk: Test how rPr defaults resolved upon import style with KeepSourceFormatting.
        /// </summary>
        [Test]
        public void TestJira6842B()
        {
            Document dst = TestUtil.Open(@"Model\Style\TestJira6842dstB.docx");
            Document src = TestUtil.Open(@"Model\Style\TestJira6842srcB.docx");

            // Check rPr defaults.
            Assert.That(dst.Styles.DefaultRunPr.NameAscii, Is.EqualTo("Times New Roman"));
            Assert.That(src.Styles.DefaultRunPr.NameAscii, Is.EqualTo("Arial"));

            Style dstStyle = dst.Styles["Normal"];
            Assert.That(dstStyle.RunPr.NameAscii, Is.EqualTo("Times New Roman"));

            dst.AppendDocument(src, ImportFormatMode.KeepSourceFormatting);

            RunPr runPr = dst.Sections[1].Body.FirstParagraph.FirstRun.RunPr;
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(36));
            Assert.That(runPr[FontAttr.SizeBi], Is.Null);
            Assert.That(runPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0xFF, 0xFF, 0x00, 0x00)));
            Assert.That(runPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromName("Arial")));

            TestUtil.SaveOpen(dst, @"Model\Style\TestJira6842B.docx");
        }


        /// <summary>
        /// Tests how content with themes is imported if themes are different in source and destination.
        /// </summary>
        [Test]
        public void TestJira3312_ImportDifferentThemes()
        {
            Document dst = TestUtil.Open(@"Model\Style\TestThemedStyles1.docx");
            Document src = TestUtil.Open(@"Model\Style\TestThemedStyles2.docx");
            dst.AppendDocument(src, ImportFormatMode.KeepSourceFormatting);
            dst.JoinRunsWithSameFormatting();

            Body importedBody = dst.Sections[1].Body;

            // [0]
            Paragraph para = importedBody.Paragraphs[0];

            // This run has no attributes. So "Book Antiqua (Body)" font should applied to this run.
            // Because themes are different this difference should be applied to imported paragraph style as normal font name.
            // WORDSNET-14587 Actually, Word expands all properties into direct paragraph/run formatting and removes style from them.
            Run run = para.Runs[0];
            Style style = dst.Styles.GetByIstd(para.ParaPr.Istd, false);
            Assert.That(style.Name, Is.EqualTo("Normal"));

            Assert.That(run.RunPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromName("Book Antiqua")));

            run = para.Runs[1];
            Assert.That(run.RunPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromName("Arial Narrow")));

            // [1]
            para = importedBody.Paragraphs[1];
            style = dst.Styles.GetByIstd(para.ParaPr.Istd, false);
            Assert.That(style.Name, Is.EqualTo("Normal"));

            // This run has "Heading 2" style which has "Lucida Sans (Headings)" font.
            // This also should be resolved to simple font name in direct run properties.
            ComplexFontName fontNameLucidaSans = ComplexFontName.FromName("Lucida Sans");
            run = para.Runs[0];
            Assert.That(run.RunPr[FontAttr.NameAscii], Is.EqualTo(fontNameLucidaSans));
            // ThemeColor attribute had to be expanded into direct run properties.
            Assert.That(run.RunPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0xFF, 0xCE, 0xB9, 0x66)));

            // [3]
            // This run had directly applied theme font so it has to be converted to simple font.
            para = importedBody.Paragraphs[3];
            run = para.Runs[1];
            Assert.That(run.RunPr[FontAttr.NameAscii], Is.EqualTo(fontNameLucidaSans));

            // Visual check.
            TestUtil.Save(dst, @"Model\Style\TestThemedStyles.docx");
        }

        /// <summary>
        /// WORDSNET-8863 Style inheritance does not work when copying style to another document
        /// Consider Normal style for every style imported.
        /// </summary>
        [Test]
        public void TestJira8863()
        {
            Document dst = new Document();
            Document src = TestUtil.Open(@"Model\Style\TestJira8863.docx");
            dst.AppendDocument(src, ImportFormatMode.KeepSourceFormatting);

            // WORDSNET-14587 Style properties had to be expanded into direct run properties.
            Paragraph para = dst.Sections[1].Body.LastParagraph;
            // The problematic run with the text "Intense".
            // The customer complains about the font is changed from 'Calibri' to 'Georgia'.
            Run run = para.Runs[1];
            RunPr runPr = run.RunPr;
            Assert.That(runPr[FontAttr.Istd], Is.Null);
            Assert.That(runPr.ComplexNameAscii.Name, Is.EqualTo("Calibri"));
            Assert.That(runPr.ComplexNameOther.Name, Is.EqualTo("Calibri"));
            Assert.That(runPr.ComplexNameFarEast.Name, Is.EqualTo("Calibri"));
            Assert.That(runPr.ComplexNameBi.IsThemeFont, Is.True);
            Assert.That(runPr.ComplexNameBi.ThemeFontCore, Is.EqualTo(ThemeFontCore.MinorBidi));
        }





        /// <summary>
        /// WORDSNET-9441 (fixed by WORDSNET-8638) - Font size in table cells is not preserved during saving DOCX to PDF/HTML.
        /// Test with default FontSize = 10.
        /// </summary>
        [Test]
        public void TestJira9441()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira9441.docx");

            Table table = doc.FirstSection.Body.Tables[0];
            RunPr dstRunPr = new RunPr();
            table.Rows[1].Cells[1].FirstParagraph.FirstRun.ExpandRunPr(dstRunPr, RunPrExpandFlags.Normal);

            Assert.That(dstRunPr[FontAttr.Size], Is.EqualTo(20));
            Assert.That(table.Style.RunPr[FontAttr.Size], Is.EqualTo(24));
            Assert.That(doc.Styles.DefaultRunPr[FontAttr.Size], Is.EqualTo(22));
        }

        /// <summary>
        /// WORDSNET-10697 Test that special base styles are handled differently during style import.
        /// e.g. DefaultParagraphFont.
        /// </summary>
        [Test]
        public void TestJira11294_1()
        {
            Document srcDoc = new Document();
            srcDoc.Styles.GetBySti(StyleIdentifier.DefaultParagraphFont, true).Font.AllCaps = true;

            Style source = Style.Create(StyleType.Character, 100, StyleIdentifier.User, "myStyle1");
            source.BasedOnIstd = StyleIndex.DefaultParagraphFont;
            srcDoc.Styles.Add(source);

            Document dstDoc = new Document();
            dstDoc.Styles.GetBySti(StyleIdentifier.DefaultParagraphFont, true).Font.Color = Color.Aqua;
            Style destination = Style.Create(StyleType.Character, 101, StyleIdentifier.User, "myStyle1");
            destination.BasedOnIstd = StyleIndex.DefaultParagraphFont;
            dstDoc.Styles.Add(destination);

            Assert.That(source.Equals(destination), Is.True);

            Style importedStyle = dstDoc.Styles.ImportStyle(
                new ImportContext(srcDoc, dstDoc, ImportFormatMode.KeepDifferentStyles),
                source);

            Assert.That(dstDoc.Styles.GetByName("myStyle1_0", false), Is.Null);
            Assert.That(importedStyle, Is.SameAs(destination));

        }

        /// <summary>
        /// WORDSNET-10697 Test that styles with same original names from multiple document get reused.
        /// </summary>
        [Test]
        public void TestJira11294_2()
        {

            Document doc1 = new Document();
            Style style1 = PopulateTestDocument(doc1, "Line1");
            style1.Font.Bold = true;

            Document doc2 = new Document();
            Style style2 = PopulateTestDocument(doc2, "Line2");
            style2.Font.Italic = true;

            Document doc3 = new Document();
            Style style3 = PopulateTestDocument(doc3, "Line3");
            style3.Font.Bold = true;

            doc1.AppendDocument(doc2, ImportFormatMode.KeepDifferentStyles);
            doc1.AppendDocument(doc3, ImportFormatMode.KeepDifferentStyles);
            doc1.AppendDocument(doc2, ImportFormatMode.KeepDifferentStyles);

            Assert.That(doc1.Styles.GetByName("myStyle1_0", false), IsNot.Null());
            Assert.That(doc1.Styles.GetByName("myStyle1_1", false), Is.Null);
        }









        /// <summary>
        /// WORDSNET-21218 Left Indent lost after Appending Document.
        /// This issue duplicates a part of WORDSNET-20704
        /// </summary>
        [Test]
        public void Test21218()
        {
            Document dstDoc = new Document();
            Document srcDoc = TestUtil.Open(@"Model\Style\Test21218.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepDifferentStyles);

            // The problematic style.
            Style style = dstDoc.Styles["PIB-Modulüberschrift"];
            Assert.That(style.ParaPr[ParaAttr.LeftIndent], Is.EqualTo(0));
            Assert.That(style.ParaPr[ParaAttr.FirstLineIndent], Is.EqualTo(-227));
        }

        /// <summary>
        /// WORDSNET-21615 ImportNode() function causes Exception: "Index was out of range".
        /// The reference style of the numbered style is not yet set properly, but we try to get list properties from it.
        /// Solved by using formatting properties from the source list in StyleCollection.CopyParaPr()
        /// instead of using destination list.
        /// </summary>
        [Test]
        public void Test21615()
        {
            Document dstDoc = new Document();
            Document srcDoc = TestUtil.Open(@"Model\Style\Test21615.docx");

            dstDoc.RemoveAllChildren();
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            dstDoc.UpdateListLabels();

            // The problematic paragraphs are inside sdt.
            StructuredDocumentTag sdt = (StructuredDocumentTag)dstDoc.FirstSection.Body.FirstChild;
            Paragraph para = (Paragraph)sdt.FirstChild;
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1."));
            para = (Paragraph)para.NextSibling;
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("a)"));
        }


        private static Style PopulateTestDocument(Document doc, string text)
        {
            Style result = doc.Styles.Add(StyleType.Character, "myStyle1");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Font.Style = result;
            builder.Writeln(text);
            return result;
        }

        /// <summary>
        /// Imports tables into the new document specified number times, using ImportFormatMode.KeepDifferentStyles.
        /// </summary>
        private static Document ImportDocument(Document srcDoc, int count)
        {
            Document dstDoc = new Document();

            NodeCollection srcTables = srcDoc.FirstSection.Body.GetChildNodes(NodeType.Table, true);

            for (int i = 0; i < count; i++)
                foreach (Table table in srcTables)
                {
                    Node importedNode = dstDoc.ImportNode(table, true, ImportFormatMode.KeepDifferentStyles);
                    dstDoc.FirstSection.Body.AppendChild(importedNode);
                }

            return dstDoc;
        }

        /// <summary>
        /// Checks equality of the following styles paragraph format properties:
        /// FirstLineIndent, LeftIndent.
        /// </summary>
        private static void CheckStylesParaFormatsEqual(Document srcDoc, Document dstDoc, string styleName)
        {
            ParagraphFormat paraFormatA = srcDoc.Styles[styleName].ParagraphFormat;
            ParagraphFormat paraFormatB = dstDoc.Styles[styleName].ParagraphFormat;

            Assert.That(paraFormatB.FirstLineIndent, Is.EqualTo(paraFormatA.FirstLineIndent));
            Assert.That(paraFormatB.LeftIndent, Is.EqualTo(paraFormatA.LeftIndent));


        }

        /// <summary>
        /// Checks equality of the following styles list format properties:
        /// NumberFormat, NumberPosition, TabPosition, TextPosition.
        /// </summary>
        private static void CheckStylesListFormatsEqual(Document srcDoc, Document dstDoc, string styleName)
        {
            ListLevel listLevelA = srcDoc.Styles[styleName].ListFormat.ListLevel;
            ListLevel listLevelB = dstDoc.Styles[styleName].ListFormat.ListLevel;

            Assert.That(listLevelB.NumberFormat, Is.EqualTo(listLevelA.NumberFormat));
            Assert.That(listLevelB.NumberPosition, Is.EqualTo(listLevelA.NumberPosition));
            Assert.That(listLevelB.TabPosition, Is.EqualTo(listLevelA.TabPosition));
            Assert.That(listLevelB.TextPosition, Is.EqualTo(listLevelA.TextPosition));
        }

        /// <summary>
        /// Checks font size after <see cref="TableFormattingExpander.ExpandRunPr"/> executed.
        /// </summary>
        private static Document CheckExpandedFontSize(string fileName, int runIdxToCheck, int expectedFontSize)
        {
            Document doc = TestUtil.Open(Path.Combine(@"Model\Style\", fileName));

            // We are using DocumentValidator here instead of Run.ExpandRunPr() because it may
            // additionally change 'overrideTableStyleFontSizeAndJustification' option and also makes to expand document defaults.
            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);

            Run run = (Run)doc.GetChild(NodeType.Run, runIdxToCheck, true);
            Assert.That(run.RunPr.Size, Is.EqualTo(expectedFontSize));

            return doc;
        }
    }
}
