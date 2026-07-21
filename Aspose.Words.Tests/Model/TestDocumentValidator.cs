// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/08/2012 by Alexey Butalov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestDocumentValidator
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Empty tables should be removed on validation stage.
        /// </summary>
        [Test]
        public void TestValidateEmptyTable()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();
            Table emptyTable = new Table(doc);
            body.AppendChild(emptyTable);

            Assert.That(body.Tables[0], Is.EqualTo(emptyTable));
            DocumentValidator validator = new DocumentValidator();
            validator.Execute(new SaveInfo(doc, null, null, new HtmlSaveOptions()));
            Assert.That(body.Tables.Count, Is.EqualTo(0));
        }


        /// <summary>
        /// WORDSNET-11133 (fixed) The text formatting of DrawingML is lost after converting from Docx to Pdf/HTML.
        /// andrnosk: The problem occurred because Dml was replaced with Vml fallback.
        /// Now after validation we still have Dml (with textboxes) in the model,
        /// and then during converting to Html ShapeRenderer converts these Dml to simple images.
        /// </summary>
        [Test]
        public void TestJira11133()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira11133.docx");
            DocumentValidator validator = new DocumentValidator();
            validator.Execute(new SaveInfo(doc, null, null, new HtmlSaveOptions()));

            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.MarkupLanguage, Is.EqualTo(ShapeMarkupLanguage.Dml));
        }

        /// <summary>
        /// WORDSNET-11458 Document.Save generates huge size html for Odt
        /// TabStop value in document is very big, more than page size.
        /// Word and OO.Writer don't display this value and use default tabstop.
        /// On validation we remove tabstop if value of it more than page size.
        /// </summary>
        // FOSS: TestJira11458 removed — it loads .odt (removed) and validates tab stops against an HTML save
        // target (removed).




        /// <summary>
        /// WORDSNET-13768 Aspose.Words allows any string in the CustomXmlPart.Id property like old versions of
        /// MS Word, but Microsoft Word Online fails to open a document created with a non-GUID value.
        /// Now on saving to ISO/IEC 29500 formats we replace text values with GUIDs during document validation.
        /// </summary>
        [Test]
        public void TestJira13768()
        {
            Document doc = TestUtil.Open(@"Model\Markup\TestJira13768.docx");
            TestUtil.ExecuteValidator(doc, OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoTransitional));

            string id = doc.CustomXmlParts[0].Id;
            bool isGuid = id.StartsWith("{") && id.EndsWith("}") && (id.Length == 38);
            Assert.That(isGuid, Is.True);

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdt.XmlMapping.StoreItemId, Is.EqualTo(id));

            // Check that correct GUID value is not changed.
            doc = TestUtil.Open(@"Model\Markup\TestDefect10445.docx");
            TestUtil.ExecuteValidator(doc, OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoTransitional));

            Assert.That(doc.CustomXmlParts[0].Id, Is.EqualTo("{31A4723B-B9FA-494A-A75D-8D07FD6A44F7}"));
        }


        /// <summary>
        /// Related to WORDSNET-16910.
        /// Checks cases related with formatting and attribute inheritance.
        /// </summary>
        [Test]
        public void TestJira16910Simplified()
        {
            Document doc = TestUtil.Open(@"Model\Run\Font\TestJira16910S.docx");
            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            so.UpdateAmbiguousTextFont = true;
            TestUtil.ExecuteValidator(doc, so);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // 1. Character style is applied to the run, which has not direct formatting.
            // Check that "BoldBi" and "SizeCs" attributes will be set directly for the run.
            Run run = paras[0].FirstRun;
            Assert.That(run.RunPr.Contains(FontAttr.Bold), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.Size), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.Italic), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.ItalicBi), Is.False);
            Assert.That((int)run.RunPr[FontAttr.SizeBi], Is.EqualTo(16));
            Assert.That((AttrBoolEx)run.RunPr[FontAttr.BoldBi], Is.EqualTo(AttrBoolEx.True));

            // 2. Run formatting contains direct "Size" and "SizeCs" attributes.
            // Check that "SizeCs" will be updated for the run.
            run = paras[1].FirstRun;
            Assert.That((int)run.RunPr[FontAttr.Size], Is.EqualTo(14));
            Assert.That((int)run.RunPr[FontAttr.SizeBi], Is.EqualTo(14));
            Assert.That(run.RunPr.Contains(FontAttr.ItalicBi), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.BoldBi), Is.False);

            // 3. "ComplexScript" attribute is set for the run.
            // Check that at this case formatting and size are not updated.
            run = paras[2].FirstRun;
            Assert.That((int)run.RunPr[FontAttr.Size], Is.EqualTo(14));
            Assert.That((int)run.RunPr[FontAttr.SizeBi], Is.EqualTo(20));
            Assert.That(run.RunPr.Contains(FontAttr.ItalicBi), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.BoldBi), Is.False);

            // 4. Check that "Italic" attribute updates. Also font size does not update, when
            // "Size" and "SizeBi" have the same values.
            run = paras[3].FirstRun;
            Assert.That(run.RunPr.Contains(FontAttr.Bold), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.Italic), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.Size), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.SizeBi), Is.False);
            Assert.That((AttrBoolEx)run.RunPr[FontAttr.ItalicBi], Is.EqualTo(AttrBoolEx.True));
            Assert.That((AttrBoolEx)run.RunPr[FontAttr.BoldBi], Is.EqualTo(AttrBoolEx.False));

            // 5. Font of the run is out of the font list, which requires the updating of size and formatting.
            run = paras[4].FirstRun;
            Assert.That(run.RunPr.Contains(FontAttr.Bold), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.BoldBi), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.Italic), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.ItalicBi), Is.False);
            Assert.That(run.RunPr.Contains(FontAttr.SizeBi), Is.False);
            Assert.That((int)run.RunPr[FontAttr.Size], Is.EqualTo(14));
        }



        /// <summary>
        /// Related to WORDSNET-17046.
        /// Case when "FontInfos" does not contain information about the font, which is checked to be overridden.
        /// </summary>
        [Test]
        public void TestJira17046NoFontInfo()
        {
            Document doc = TestUtil.Open(@"Model\Run\Font\TestJira17046NoFontInfo.docx");
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Run run = doc.FirstSection.Body.FirstParagraph.FirstRun;
            Assert.That(((ComplexFontName)run.RunPr[FontAttr.NameAscii]).Name, Is.EqualTo("Times New Roman"));
            Assert.That(((ComplexFontName)run.RunPr[FontAttr.NameBi]).Name, Is.EqualTo("Cordia New"));
        }



        /// <summary>
        /// Replates to WORDSNET-19668 Checks that the font "Segoe UI Emoji" is set for all emojis from
        /// the emoticons block (0x1F600 - 0x1F64F).
        /// </summary>
        [Test]
        public void Test19668Emoticons()
        {
            Document doc = TestUtil.Open(@"Model\Run\Font\Test19668Emoticons.docx");
            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            so.UpdateAmbiguousTextFont = true;
            TestUtil.ExecuteValidator(doc, so);

            NodeCollection nodes = doc.FirstSection.Body.GetChildNodes(NodeType.Run, true);
            List<Run> runs = nodes.ToList<Run>();

            Assert.That(runs.Count, Is.EqualTo(1));
            Assert.That(runs[0].Font.Name, Is.EqualTo("Segoe UI Emoji"));
        }

        // FOSS: Test20143_DMLFloatingStory / _DMLFloatingInTextbox / _VMLFloatingInFootnote / _VMLFloatingInTextbox
        // removed — they exercised the fixed-format (PDF) rendering-prep validation that makes/moves floating
        // shapes in unexpected stories inline; that layout/rendering-prep path is gone.


        /// <summary>
        /// Related with WORDSNET-24026.
        /// This test related with the case when a shape is shrink by AW when it has large size.
        /// Comment was the following:
        /// "We normally validate shape size when width or height is set, but we also allow validation
        /// of the shape size before document save. This is needed in case some child-level
        /// shape with huge local coordinates was turned into a top-level shape by the user."
        /// However, the Word does not shrink a shape at this case. This test checks that AW behavior mimics the Word.
        /// </summary>
        // FOSS: Test24026UnGroup removed — it loads a WordML (.wml) input (removed) and validates for the
        // removed Doc format.


        /// <summary>
        /// WORDSNET-23156 Vertical table cell merge disappears on saving to docx, pdf.
        /// The problem related with the vertical merge algorithm which calling on a table validation.
        /// Update vertical merge algorithm to fix the issue.
        /// </summary>
        [Test]
        public void Test23156()
        {
            Document doc = TestUtil.Open(@"Model\Table\Test23156.docx");
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            Table tbl = doc.FirstSection.Body.Tables[1];

            CheckVertMergeAndGridSpan(tbl.FirstRow.FirstCell.CellPr, CellMerge.First, 2);
            CheckVertMergeAndGridSpan(tbl.FirstRow.LastCell.CellPr, CellMerge.None, 1);

            CheckVertMergeAndGridSpan(tbl.LastRow.FirstCell.CellPr, CellMerge.Previous, 1);
            CheckVertMergeAndGridSpan(tbl.LastRow.LastCell.CellPr, CellMerge.None, 2);

            tbl = doc.FirstSection.Body.Tables[2];

            CheckVertMergeAndGridSpan(tbl.FirstRow.FirstCell.CellPr, CellMerge.First, 2);
            CheckVertMergeAndGridSpan(tbl.FirstRow.LastCell.CellPr, CellMerge.None, 1);

            CheckVertMergeAndGridSpan(tbl.LastRow.FirstCell.CellPr, CellMerge.Previous, 1);
            CheckVertMergeAndGridSpan(tbl.LastRow.LastCell.CellPr, CellMerge.None, 2);
        }

        /// <summary>
        /// WORDSNET-25489 Accessing paragraph border creates None border in the output document.
        /// We should not reset <see cref="Border.IsInherited"/> flag in validator,
        /// as we rely on it when writing border in writers.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf save were removed.
        [TestCase(SaveFormat.Docx)]
        public void Test25489(SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Write("This paragraph should have border.");

            // This getter creates inherited border, that should not affect output.
            Border border = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Borders.Bottom;
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.None));

            // Validator should not reset Border.IsInherited flag.
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);
            Assert.That(border.IsInherited, Is.True);

            // Otherwise, setting border in style will be ignored, that is incorrect.
            Style style = doc.Styles.Add(StyleType.Paragraph, "Heading1");
            style.ParagraphFormat.Borders.Bottom.LineStyle = LineStyle.Single;
            style.ParagraphFormat.Borders.Bottom.LineWidth = 5;

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.ParagraphFormat.Style = style;

            TestUtil.SaveOpen(doc, string.Format(@"Model\ParaBorder\Test25489.{0}", FileFormatUtil.SaveFormatToExtension(sf)));
        }

        /// <summary>
        /// Relates to WORDSNET-25489.
        /// Checks roundtrip of the document when border of the paragraph is set explicitly to 'none'.
        /// </summary>
        // FOSS: only Docx2Docx survives — Doc/Rtf load+save were removed.
        [TestCase(UnifiedScenario.Docx2Docx)]
        public void Test25489A(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\ParaBorder\Test25489None", scenario);
            Paragraph para = doc.FirstSection.Body.FirstParagraph;

            Border border = para.ParagraphFormat.Borders.Bottom;
            Assert.That(border.LineStyle, Is.EqualTo(LineStyle.None));
            Assert.That(border.IsInherited, Is.False);
        }

        // FOSS: Test26634 removed — it verified REF-field preservation specifically for DOTM->DOC conversion;
        // DOC save is removed, and the DOTM->DOCX path yields a different field breakdown (field[2] becomes
        // FieldNone), so the DOC-specific scenario no longer applies.


        /// <summary>
        /// WORDSNET-26819 UpdateFields raises 'IndexOutOfRangeException'.
        /// Missed custom footnote reference mark causes exception in layout.
        /// </summary>
        [Test]
        public void Test26819_Footnotes()
        {
            Document document = TestUtil.Open(@"Model\Footnote\Test26819 Footnotes.docx");

            TestUtil.ExecuteValidator(document, SaveFormat.Docx);

            Assert.That(document.GetChildNodes(NodeType.Footnote, true).Count, Is.EqualTo(9));

            ParagraphCollection paragraphs = document.FirstSection.Body.Paragraphs;
            AssertFootnote(paragraphs[2], true, string.Empty, "\u0002 Footnote 1\r");
            AssertFootnote(paragraphs[5], false, "Move", "Footnote 2\r");
            AssertFootnote(paragraphs[8], true, string.Empty, "\u0002 Footnote 3\r");
            AssertFootnote(paragraphs[9], true, string.Empty, "\u0002 Footnote 4\r");
            AssertFootnote(paragraphs[12], false, "Move to", "\u0002 Footnote 6\r\u0002 Footnote 5\r");
            AssertFootnote(paragraphs[14], false, "Move to", "Footnote 8\rFootnote 7\r");
            AssertFootnote(paragraphs[16], false, "Move to", "Footnote 10\r\u0002 Footnote 9\r");
            AssertFootnote(paragraphs[18], false, "Move to", "\u0002 Footnote 12\rFootnote 11\r");

            AssertFootnote(
                document.FirstSection.Body.Tables[0].LastRow.LastCell.Paragraphs[0],
                true,
                string.Empty,
                "\u0002 Footnote 13\r");
        }

        /// <summary>
        /// WORDSNET-26819 UpdateFields raises 'IndexOutOfRangeException'.
        /// Missed custom footnote reference mark causes exception in layout.
        /// </summary>
        [Test]
        public void Test26819_Endnotes()
        {
            Document document = TestUtil.Open(@"Model\Footnote\Test26819 Endnotes.docx");

            TestUtil.ExecuteValidator(document, SaveFormat.Docx);

            Assert.That(document.GetChildNodes(NodeType.Footnote, true).Count, Is.EqualTo(9));

            ParagraphCollection paragraphs = document.FirstSection.Body.Paragraphs;
            AssertFootnote(paragraphs[2], false, "Move", "\u0002 Endnote 1\r");
            AssertFootnote(paragraphs[5], false, "Move", "Endnote 2\r");
            AssertFootnote(paragraphs[8], false, "Move from and to", "\u0002 Endnote 3\r");
            AssertFootnote(paragraphs[9], false, "Move to", "\u0002 Endnote 4\r");
            AssertFootnote(paragraphs[12], false, "Move to", "\u0002 Endnote 6\r\u0002 Endnote 5\r");
            AssertFootnote(paragraphs[14], false, "Move to", "Endnote 8\rEndnote 7\r");
            AssertFootnote(paragraphs[16], false, "Move to", "Endnote 10\r\u0002 Endnote 9\r");
            AssertFootnote(paragraphs[18], false, "Move to", "\u0002 Endnote 12\rEndnote 11\r");

            AssertFootnote(
                document.FirstSection.Body.Tables[0].LastRow.LastCell.Paragraphs[0],
                false,
                "Move to",
                "\u0002 Endnote 13\r");
        }

        private static void AssertFootnote(Paragraph paragraph, bool isAuto, string referenceMark, string footnoteText)
        {
            Footnote footnote = (Footnote)paragraph.FirstChild;

            Assert.That(footnote.IsAuto, Is.EqualTo(isAuto));
            Assert.That(footnote.ReferenceMark, Is.EqualTo(referenceMark));
            Assert.That(footnote.GetText(), Is.EqualTo(footnoteText));
        }

        private static void CheckVertMergeAndGridSpan(CellPr cellPr, CellMerge expectedMerge, int expectedSpan)
        {
            Assert.That(cellPr.VerticalMerge, Is.EqualTo(expectedMerge));
            Assert.That(cellPr.GridSpan, Is.EqualTo(expectedSpan));
        }
    }
}
