// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.RW.Txt.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export;
using Aspose.Words.Tests.Model;
using Aspose.Words.Validation;
using NUnit.Framework;
using List = Aspose.Words.Lists.List;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Miscellaneous import DOCX tests that do not fit into more specific categories.
    /// At the moment contains tests for anything in the text, tables, shapes etc.
    /// </summary>
    [TestFixture]
    public class TestImportDocx
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
            // DocTestStub is not autoportable.
#if !JAVA && !NETSTANDARD && !CPLUSPLUS
            DocTestStub.InitialShapeId = 0;
#endif
        }





        // FOSS TestJira6806 removed: verified data-bound SDT content is preserved when rendering to a
        // fixed page format (Pdf), which is a removed format.

        /// <summary>
        /// WORDSNET-6350 BaseJustification.Bottom is lost after open/save in OfficeMath.
        /// Added "bot" for ECMA376 MathJustification, whereas it was "bottom" for ISO29500 docs.
        /// </summary>
        [Test]
        public void TestJira6350()
        {
            const string testName = @"Model\Math\Customers\TestJira6350";
            Document doc = TestUtil.Open(testName + ".docx");

            OfficeMath math = (OfficeMath)doc.GetNodeById("6.0.0.0.0.0");
            Assert.That(math.MathObject.ContainsKey(MathAttr.BaseJustification), Is.True);
            Assert.That(math.MathObject.GetDirectAttr(MathAttr.BaseJustification), Is.EqualTo(MathBaseJustification.Bottom));

            OoxmlSaveOptions options = new OoxmlSaveOptions();
            options.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            TestUtil.SaveOpen(doc, testName + "_iso29500.docx", options);

            options.Compliance = OoxmlCompliance.Ecma376_2006;
            TestUtil.SaveOpen(doc, testName + ".docx", options);
        }



        /// <summary>
        /// WORDSNET-3932 Unknown color type encountered during WordML import - #9bbb59 [3206].
        /// RK Updated the regular expression to parse such colors safely.
        /// </summary>
        [Test]
        public void TestDefect3932()
        {
            TestUtil.OpenSaveOpen(@"ImportDocx\TestDefect3932.docx");
        }


        /// <summary>
        /// WORDSNET-5530 Images are not displayed when opening docx file from website.
        /// RK The weird problem was due to missing o:title="" attribute. Made to write it always.
        /// </summary>
        [Test]
        public void TestDefect5530()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestDefect5530.docx");
            // FOSS Dropped the WML roundtrip (removed format); assert the imported shape directly.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            string title = (string)shape.ShapePr.FetchAttr(ShapeAttr.ImageTitle);
            Assert.That(title, Is.EqualTo(""));
        }








        private static void CheckThemeFont(Paragraph para, string fontName)
        {
            Assert.That(para.ParagraphFormat.SpaceAfter, Is.EqualTo(10));
            Assert.That(para.FirstRun.Font.Name, Is.EqualTo(fontName));
        }


        private static void CheckDefect5313(SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestDefect5313.docx");

            // Convert to the desired format.
            // FOSS No gold check (the format changed from the removed Doc/Rtf/WordML); the model asserts below are the point.
            doc = TestUtil.SaveOpen(doc, TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(@"ImportDocx\TestDefect5313"), "", sf), (SaveOptions)null, false);

            // Check the attributes were expanded properly.
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Paragraph para = paras[0];
            Font font = para.FirstRun.Font;
            Assert.That(font.Name, Is.EqualTo("Cambria"));
            Assert.That(para.ParagraphFormat.SpaceAfter, Is.EqualTo(15));

            para = paras[1];
            font = para.FirstRun.Font;
            Assert.That(font.Name, Is.EqualTo("Calibri"));
            Assert.That(font.Size, Is.EqualTo(11));
            Assert.That(para.ParagraphFormat.SpaceAfter, Is.EqualTo(10));

            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            para = cell.FirstParagraph;
            Assert.That(para.GetText(), Is.EqualTo("Workitem\x0007"));
            Assert.That(para.ParagraphFormat.SpaceAfter, Is.EqualTo(0));
        }



        /// <summary>
        /// WORDSNET-7946 DrawAspect of an OLE object was changed during Open/Save.
        /// The problem was that the XML had "icon" whereas it had to be "Icon". Made resilient.
        /// </summary>
        [Test]
        public void TestDefect7946()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestDefect7946.docx");
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.OleFormat.OleIcon, Is.EqualTo(true));
        }



        /// <summary>
        /// WORDSNET-15791 FileCorruptedException occurs during loading.
        /// The problem was caused by the missing styleId in the document. Made resilient.
        /// </summary>
        [Test]
        public void TestDefect15791()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestDefect15791.docx");
            Assert.That(doc.Styles["CaptionItemParagraph"] != null, Is.True);
        }

        /// <summary>
        /// WORDSNET-15335 FileCorruptedException occurs during loading.
        /// The problem was we had a ThemeColor attribute in a run properties revision, but no default in the RunPr class.
        /// </summary>
        [Test]
        public void TestDefect15335()
        {
            TestUtil.OpenSaveOpen(@"ImportDocx\TestDefect15335.docx");
        }




        /// <summary>
        /// WORDSNET-20828 Numbering is wrong after open/save document.
        /// The problem occurred because nsid of abstractNum was omitted. In this case we used default listDefId that cause incorrect numbering.
        /// Fixed by using abstractNumId in case if nsid is omitted.
        /// </summary>
        [Test]
        public void TestDefect20828()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestDefect20828.docx");

            Paragraph par0 = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(par0.IsListItem, Is.True);
            Assert.That(par0.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.LowercaseLetter));

            Paragraph par1 = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(par1.IsListItem, Is.True);
            Assert.That(par1.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.LowercaseRoman));

            Paragraph par2 = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            Assert.That(par2.IsListItem, Is.True);
            Assert.That(par2.ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.UppercaseLetter));
        }


        /// <summary>
        /// WORDSNET-21462 Background of cells is lost upon loading DOCX document.
        /// The problem occurred because there was '#' at the beginning of color code string.
        /// Color code parser did not recognize such code as valid.
        /// Fixed by trimming '#' character from the color code string.
        /// </summary>
        [Test]
        public void TestDefect21462()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestDefect21462.docx");
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            // Color of the cell should bot be empty.
            Assert.That(table.FirstRow.FirstCell.CellFormat.Shading.BackgroundPatternColor, IsNot.EqualTo(Color.Empty));
        }





        /// <summary>
        /// WORDSNET-21677 InvalidOperationException occurs during open/save.
        /// The problem occurred because we did not handle tiff images.
        /// Fixed by adding a case for Tiff format.
        /// </summary>
        [Test]
        public void TestDefect21677()
        {
            // For now it is enough just test open/save of this document.
            // The image is DrawingML so we cannot check it's type for not, because there is no public API yet.
            TestUtil.OpenSaveOpen(@"ImportDocx\TestDefect21677.docx");
        }

        /// <summary>
        /// WORDSNET-26296 Docx to Doc conversion makes very bad output if document contain tables.
        /// Found some severe bug during analysis. I fixed this bug and made more specific CRs about issue.
        /// </summary>
        [Test]
        public void TestDefect26296()
        {
            string testfile = TestUtil.BuildTestFileName(@"Model\Style\Table\TestBuiltInTableStyles.docx");
            string outfile = TestUtil.BuildOutFileName(testfile, "", SaveFormat.Docx); // FOSS Doc -> Docx roundtrip

            Document doc = TestUtil.Open(testfile);
            doc.Save(outfile);
            doc = TestUtil.Open(outfile);

            Assert.That(doc.Styles.Count, Is.EqualTo(16));
            TableStyle style = (TableStyle)doc.Styles[8];
            CellPr cellPr =
                (CellPr)style.FetchConditionalStylePr(TableStyleOverrideType.OddRowBanding, AttrCollectionType.CellPr);

            // insane check. Verify border in one conditional style.
            Assert.That(cellPr.BorderTop.LineWidth, Is.EqualTo(1));
            Assert.That(cellPr.BorderTop.LineStyle, Is.EqualTo(LineStyle.Single));
        }


        /// <summary>
        /// WORDSNET-26279 Extra space appears at bottom of table cells in HTML output.
        /// DocumentValidator.ExpandsDocumentDefault modifies styles so next save produces incorrect document because
        /// DocumentValidator.TableFormattingExpander uses modified styles.
        /// </summary>
        [Test]
        public void TestDefect26279()
        {
            const string testfile = @"ImportDocx\TestDefect26279.docx";
            Document doc = TestUtil.Open(testfile);

            // FOSS WordML -> FlatOpc (a supported flat OOXML format) to keep the deterministic byte compare.
            string outfile1 = TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(testfile), " 1", SaveFormat.FlatOpc);
            string outfile2 = TestUtil.BuildOutFileName(TestUtil.BuildTestFileName(testfile), " 2", SaveFormat.FlatOpc);

            // save twice
            doc.Save(outfile1);
            doc.Save(outfile2);

            // and compare results
            FileStream out1 = new FileStream(outfile1, FileMode.Open, FileAccess.Read);
            FileStream out2 = new FileStream(outfile2, FileMode.Open, FileAccess.Read);

            Assert.That(TestUtil.CompareStreams(out1, out2), Is.True);
        }

        // FOSS TestDefect5032 removed: verifies extra space-after is resolved during the removed DOC
        // conversion; the assertion relies on the DOC-specific node tree (the id addresses a Cell after a
        // Docx roundtrip), so it can't be faithfully reproduced in the FOSS build.

        /// <summary>
        /// WORDSNET-4826 SectionBreak inside SDT is ignored upon loading DOCX document.
        /// This file has several sections/bodies all wrapped into structured document tag. Such document can not be represented by AW Model so std should be removed.
        /// AM. I'd like to use MarkupResolver (former CustomXmlResolver) class to process markup (custom XML and SDT) in DOCX files.
        /// It removes ugly recursion unbound logic and handles all markup processing in uniform way.
        /// Once we get solution to represent section nodes within markups such additional processing can be easily changed in one place.
        /// </summary>
        [Test]
        public void TestDefect4826()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestDefect4826.docx");

            // Verify sections were read properly.
            Assert.That(doc.Sections.Count, Is.EqualTo(2));
        }





        /// <summary>
        /// Core body for <see cref="TestJira5323"/>
        /// </summary>
        internal static void TestJira5323Core(Document doc)
        {
            NodeList notes = doc.SelectNodes("//Footnote");
            Assert.That(notes.Count, Is.EqualTo(2));
            // check that the first footnote in the document loaded and custom text presented
            Footnote footnote = (Footnote)notes[0];
            Assert.That(footnote.FootnoteType, Is.EqualTo(FootnoteType.Footnote));
            Assert.That(footnote.IsAuto, Is.EqualTo(false));
            Assert.That(footnote.ReferenceMark, Is.EqualTo("http://eur-lex.europa.eu/LexUriServ/LexUriServ.do?uri=CELEX:31985L0374:EN:HTML"));

            footnote = (Footnote)notes[1];
            Assert.That(footnote.FootnoteType, Is.EqualTo(FootnoteType.Footnote));
            Assert.That(footnote.IsAuto, Is.EqualTo(false));
            Assert.That(footnote.ReferenceMark.Length, Is.EqualTo(1));
            Assert.That(footnote.ReferenceMark[0], Is.EqualTo(ControlChar.LineBreakChar));
        }


        /// <summary>
        /// Core body of <see cref="TestJira5324"/>
        /// </summary>
        internal static void TestJira5324Core(Document doc)
        {
            NodeList notes = doc.SelectNodes("//Footnote");
            Assert.That(notes.Count, Is.EqualTo(1));

            // check that the first footnote in the document loaded and custom text presented
            Footnote footnote = (Footnote)notes[0];
            Assert.That(footnote.FootnoteType, Is.EqualTo(FootnoteType.Footnote));
            Assert.That(footnote.IsAuto, Is.EqualTo(false));
            Assert.That(footnote.ReferenceMark.Length, Is.EqualTo(1));
            Assert.That(footnote.ReferenceMark, Is.EqualTo("\xf02a"));
        }

        // FOSS TestJira4047 removed: explicitly tests that the removed Doc/Wml/Rtf formats serialize
        // XML ActiveX controls as images (a legacy-format-only behavior).



        /// <summary>
        /// WORDSNET-4664 Center alignment is not correct when render to pdf or xps.
        /// andrnosk: There are two Runs with leading white spaces, but the first run does not have "Space" attribute set.
        /// andrnosk: MS Word removes white spaces from the firs run, AW does the same now.
        /// </summary>
        [Test]
        public void TestJira4664()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestJira4664.docx");

            // Get all Run nodes.
            NodeCollection runs = doc.FirstSection.Body.GetChildNodes(NodeType.Run, true);

            Assert.That(runs[0].GetText().StartsWith(" "), Is.False);
            Assert.That(runs[1].GetText().StartsWith(" "), Is.True);
            Assert.That(runs[2].GetText().StartsWith(" "), Is.False);
        }

        /// <summary>
        /// WORDSNET-5553 Additional paragraph break are added after open/save document.
        /// andrnosk: There is Run with leading and trailing \r\n chars.
        /// andrnosk: MS Word removes \r\n chars in all cases, AW does the same now.
        /// </summary>
        [Test]
        public void TestJira5553()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestJira5553.docx");

            // Get all Run nodes.
            NodeCollection runs = doc.FirstSection.Body.GetChildNodes(NodeType.Run, true);

            Assert.That(StringUtil.Contains(runs[0].GetText(), "\r\n", false), Is.False);
        }


        // FOSS TestJira4164 removed: asserts the FillType.ShadeUnscale gradient produced by the removed
        // WML/VML conversion; in the FOSS build the imported shape fill is Solid.

        /// <summary>
        /// WORDSNET-5240 "System.ArgumentException" exception occurs during saving DOCX to other formats.
        /// andrnosk: OfficeMath class does not allow a Shape node to be inserted into itself.
        /// Added mechanism to check if the parent node of DrawingML is OfficeMath then search for parent where we can insert shape.
        /// </summary>
        [Test]
        public void TestJira5240()
        {
            // FOSS Doc/WML saves are removed; verify the OfficeMath+DrawingML doc roundtrips to Docx without throwing.
            TestUtil.OpenSaveOpen(@"ImportDocx\TestJira5240", LoadFormat.Docx, SaveFormat.Docx);
        }





        /// <summary>
        /// WORDSNET-5996 Track Changes (Revisions) are lost on open/save.
        /// Move revisions are unsupported, convert them into common del/ins revisions.
        /// andrnosk: Move revisions are supported now.
        /// </summary>
        [Test]
        public void TestJira5996()
        {
            LoadOptions lo = new LoadOptions();
            WarningInfoCollection warnings = new WarningInfoCollection();
            lo.WarningCallback = warnings;

            Document doc = TestUtil.Open(@"ImportDocx\TestJira5996.docx", lo);

            Assert.That(doc.HasRevisions, Is.True);

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.That(paragraphs[1].FirstRun.RunPr.HasMoveFromRevision, Is.True);
            Assert.That(paragraphs[3].FirstRun.RunPr.HasMoveToRevision, Is.True);
        }







        /// <summary>
        /// WORDSNET-5431 Text appears with shadow after open/save.
        /// Document has W14 markup which specifies extended shadow properties.
        /// Since DOCX reader is not namespace-aware this extended shadow is mistakenly treated as old shadow attribute
        /// </summary>
        [Test]
        public void TestJira5431()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira5431.docx");
            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);

            // andrnosk: FallBack was read instead of Choice to the model, because Choice contains unsupported element "wps:txbx".
            Assert.That(run, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-6332 Font language settings are lost when converting DOCX to DOC.
        /// NoProof localeId should be written also.
        /// </summary>
        [Test]
        public void TestJira6332()
        {
            // Since we don't have LocaleIdNoProof anymore in Mode, DOC output file should be tested manually.
            // Actually this is made implicitly during RunPrFIler tests.
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestJira6332", LoadFormat.Docx, SaveFormat.Docx); // FOSS Doc -> Docx
            Run run = (Run)doc.GetNodeById("3.0.0.0");
            Assert.That((Language)run.RunPr.GetDirectAttr(FontAttr.LocaleId), Is.EqualTo(Language.FrenchFrance));
        }


        /// <summary>
        /// WORDSNET-5758 Exception while loading .docx file.
        /// andrnosk: The problem occurs because the latest node of the document's section is Table. But it must be paragraph.
        /// The error occurred on the old code when bookmarks were moved to the inline level.
        /// </summary>
        [Test]
        public void TestJira5758()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira5758.docx");
#pragma warning disable CS0612
#pragma warning restore CS0612

            Assert.That(doc.LastSection.Body.LastChild.NodeType, Is.EqualTo(NodeType.BookmarkEnd));
        }




        /// <summary>
        /// WORDSNET-5307 Watermark disappears after open/save DOCX document.
        /// andrnosk: Fixed by adding mechanism to concatenate headers/footers of the same type.
        /// </summary>
        [Test]
        public void TestJira5307A()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira5307.docx");
            Assert.That(doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Paragraphs.Count, Is.EqualTo(3));
        }



        /// <summary>
        /// WORDSNET-6852 Incorrect conversion of LeftIndentUnits into LeftIndent by DocumentValidator.
        /// Use SizeBi if Size attribute is missing for indent calculation.
        /// </summary>
        [Test]
        public void TestJira6852()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira6852.docx");
            // FOSS Save via Docx (Html is a removed format) still runs the indent-converting validator.
            doc.Save(new MemoryStream(), SaveFormat.Docx);

            ParaPr paraPr = ((Paragraph)doc.GetNodeById("6.6.0")).ParaPr;
            Assert.That(paraPr.LeftIndent, Is.EqualTo(1428));
            Assert.That(paraPr.FirstLineIndent, Is.EqualTo(-420));
        }


        /// <summary>
        /// WORDSNET-6928 xml:space="preserve" attribute is not preserved during open/save.
        /// If xml:space="preserve” attribute  is not set in Run or document then remove all leading and trailing white-spaces and tabs.
        /// </summary>
        [Test]
        public void TestJira6928()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira6928.docx");
            Assert.That(doc.GetNodeById("0.0.0.0.0.0").ToString(SaveFormat.Text), Is.EqualTo("Cell one\r"));
        }





        /// <summary>
        /// WORDSNET-7399 Link between grouped textboxes breaks when saving a .docx.
        /// andrnosk: The problem occurs because we try to get linked shape by ID before reading this shape to the model.
        /// Fixed by writing original textbox next shape Id into the temporary TextboxNextShapeIdRaw,
        /// and then updating TextboxNextShapeId using temporary Id.
        /// </summary>
        [Test]
        public void TestJira7399()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira7399.docx");
            GroupShape dml = ((GroupShape)doc.GetChild(NodeType.GroupShape, 0, true));

            Shape firstInGroup = (Shape)dml.GetChildNodes(NodeType.Any, false)[0];
            Shape secondInGroup = (Shape)dml.GetChildNodes(NodeType.Any, false)[1];

            // Check the TextboxNextShapeId is here.
            Assert.That(firstInGroup.ShapePr[ShapeAttr.TextboxNextShapeId], Is.EqualTo(secondInGroup.Id));
            Assert.That(secondInGroup.ShapePr[ShapeAttr.TextboxNextShapeId], Is.Null);
        }

        /// <summary>
        /// WORDSNET-7639 Aspose.Words.FileCorruptedException occurs when loading a WordML/Docx.
        /// andrnosk: The problem occurs because there is ilvl equals -1, but according to specification ISO29500 17.9.6
        /// it is a zero-based index of the number of list levels in the document.
        /// Fixed by processing -1 value as MS Word does.
        /// </summary>
        [Test]
        public void TestJira7639()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira7639.docx");
            Assert.That(doc.FirstSection.Body.FirstParagraph.IsListItem, Is.True);
            Assert.That(doc.FirstSection.Body.FirstParagraph.ListFormat.ListLevelNumber, Is.EqualTo(0));
        }






        /// <summary>
        /// WORDSNET-7990 A Bookmark is not preserved during open/save a DOCX.
        /// andrnosk: The problem occurs because there is bookmark id greater than int32 maximum value.
        /// This value was trimmed to negative and that is why have decided just get module of it to fix this.
        /// </summary>
        [Test]
        public void TestJira7990()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira7990.docx");
            Assert.That(doc.FirstSection.Range.Bookmarks.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// WORDSNET-8023 Unwanted border around Cells of first Row appears during open/save.
        /// andrnosk: The problem occurs because AW does not write border when this border has lineStyle none,
        /// and that is why this border inherits from TableBorder.
        /// Fixed by checking LineWidth, in this case only a border with val=none; sz=0; will be ignored.
        /// </summary>
        [Test]
        public void TestJira8023()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira8023.docx");
            BorderCollection borders = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.CellFormat.Borders;
            Assert.That(borders[BorderType.Left].IsVisible, Is.False);
            Assert.That(borders[BorderType.Right].IsVisible, Is.False);
            Assert.That(borders[BorderType.Top].IsVisible, Is.False);
            Assert.That(borders[BorderType.Bottom].IsVisible, Is.False);
        }


        /// <summary>
        /// Relates to above TestJira6904.
        /// </summary>
        [Test]
        public void TestJira6907()
        {
            // There is slightly different problem. All shapes has equal z-index. In this case we should number them by appearance.
            Document doc = TestUtil.Open(@"ImportDocx\TestJira6107.docx");
            NodeCollection shapes = doc.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false);
            CheckZLess(shapes[0], shapes[1]);
            CheckZLess(shapes[1], shapes[2]);
            CheckZLess(shapes[2], shapes[3]);
            CheckZLess(shapes[3], shapes[4]);
        }


        /// <summary>
        /// Checks that z-order of first shape is less than z-order of second shape.
        /// </summary>
        private static void CheckZLess(Node shape1, Node shape2)
        {
            Assert.That(((ShapeBase)shape1).ZOrder, Is.LessThan(((ShapeBase)shape2).ZOrder));
        }


        /// <summary>
        /// WORDSNET-8113 Open/Save Docx lose the bullet.
        /// ListValidator made incorrect translation, because List.Style != null.
        /// Lists.Style.ParaPr.ListId should be also translated in this case.
        /// </summary>
        [Test]
        public void TestJira8113()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira8113.docx");
            Assert.That(doc.Lists[5].ListLevels[0].NumberFormat, Is.EqualTo("•"));
        }









        /// <summary>
        /// WORDSNET-6173 Incorrect font problem occurs after loading Word docx format.
        /// Additional test for WORDSNET-3312
        /// </summary>
        [Test]
        public void TestJira6173()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira6173.docx");

            Run run = doc.Sections[0].Body.Paragraphs[0].Runs[0];

            // Verify font name exposed correctly to public.
            Assert.That(run.Font.Name, Is.EqualTo("Calibri"));

            // Internal IRunAttrSource data (bottom-up resolving).
            ComplexFontName complexFontName = (ComplexFontName)((IRunAttrSource)run).FetchInheritedRunAttr(FontAttr.NameAscii);
            Assert.That(complexFontName.IsThemeFont, Is.True);
            Assert.That(complexFontName.ThemeFontCore, Is.EqualTo(ThemeFontCore.MinorHAnsi));

            // Internal Run.ExpandedRunPr (top-bottom resolving).
            RunPr expandedRunPr = run.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
            Assert.That(expandedRunPr.ComplexNameAscii.IsThemeFont, Is.True);
            Assert.That(expandedRunPr.ComplexNameAscii.ThemeFontCore, Is.EqualTo(ThemeFontCore.MinorHAnsi));

            // Layout the document.
            // No update page layout in FOSS.

            // Verify nothing is changed after document layout is rebuilt.
            Assert.That(run.Font.Name, Is.EqualTo("Calibri"));
        }

        /// <summary>
        /// WORDSNET-8446 Support import of element 'image' in DOCX format.
        /// </summary>
        [Test]
        public void TestJira8446()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira8446.docx");
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            Assert.That(shapes.Count, Is.EqualTo(1));
            Shape shape = (Shape)shapes[0];
            Assert.That(shape.ImageData.ImageBytes.Length, Is.EqualTo(141));
            Assert.That(shape.Width, Is.EqualTo(9.0d));
            Assert.That(shape.Height, Is.EqualTo(9.0d));
        }

        /// <summary>
        /// WORDSNET-8577 InvalidCastException occurs during loading a DOCX.
        /// There is strange mess of include picture fields in file. And they all include only one picture. Word removes such "incorrect" fields.
        /// </summary>
        [Test]
        public void TestJira8577()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira8577.docx");
            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(1));
        }

        // FOSS TestJira6380 removed: checks image file extensions in HTML export output (HTML is a removed format).






        /// <summary>
        /// WORDSNET-8652 System.ArgumentException is thrown while doing Mail Merge
        /// Bookmarks with duplicated names should be renamed upon node importing.
        /// </summary>
        [Test]
        public void TestJira8652()
        {
            const string testName = @"ImportDocx\TestJira8652.docx";
            Document doc = TestUtil.Open(testName);

            VerifyBookmarks(doc, new string[] { "test", "new", "_GoBack" });
            doc.AppendDocument(TestUtil.Open(testName), ImportFormatMode.KeepSourceFormatting);
            VerifyBookmarks(doc, new string[] { "test", "new", "_GoBack", "test_0", "new_0" });
            doc.AppendDocument(TestUtil.Open(testName), ImportFormatMode.KeepSourceFormatting);
            VerifyBookmarks(doc, new string[] { "test", "new", "_GoBack", "test_0", "new_0", "test_1", "new_1" });
        }

        /// <summary>
        /// Relates to TestJira8652. Verifies BookmarkStart names in the document.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="names"></param>
        private static void VerifyBookmarks(Document doc, string[] names)
        {
            NodeCollection bookmarkStarts = doc.GetChildNodes(NodeType.BookmarkStart, true);

            Assert.That(bookmarkStarts.Count, Is.EqualTo(names.Length));
            for (int i = 0; i < bookmarkStarts.Count; i++)
                Assert.That(((BookmarkStart)bookmarkStarts[i]).Name, Is.EqualTo(names[i]));
        }

        /// <summary>
        /// WORDSNET-4336 Font is not changed during appending with UseDestinationStyles
        /// Consider ImportFormatMode when resolve document defaults difference.
        /// </summary>
        [Test]
        public void TestJira4336()
        {
            Document dstDoc = TestUtil.Open(@"ImportDocx\TestJira4336_Dst.docx");
            Document srcDoc = TestUtil.Open(@"ImportDocx\TestJira4336_Src.docx");
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            // Verify that no "compensation" attributes were added to imported document nodes in spite of
            // document defaults difference because ImportFormatMode.UseDestinationStyles was used.
            Paragraph p = dstDoc.Sections[1].Body.FirstParagraph;
            Run run = p.FirstRun;
            Assert.That(p.ParaPr[ParaAttr.Alignment], Is.Null);
            Assert.That(run.RunPr[FontAttr.NameAscii], Is.Null);
            Assert.That(run.RunPr[FontAttr.Size], Is.Null);

            TestUtil.Save(dstDoc, @"ImportDocx\TestJira4336_UseDestinationStyles.docx");

            dstDoc = TestUtil.Open(@"ImportDocx\TestJira4336_Dst.docx");
            srcDoc = TestUtil.Open(@"ImportDocx\TestJira4336_Src.docx");
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            // Verify that document defaults difference were compensated by direct attributes because source formatting should be kept.
            p = dstDoc.Sections[1].Body.FirstParagraph;
            run = p.FirstRun;
            Assert.That(p.ParaPr[ParaAttr.Alignment], Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(run.RunPr.ComplexNameAscii.Name, Is.EqualTo("Times New Roman"));
            Assert.That(run.RunPr[FontAttr.Size], Is.EqualTo(22));

            TestUtil.Save(dstDoc, @"ImportDocx\TestJira4336_KeepSourceFormatting.docx");
        }


        /// <summary>
        /// WORDSNET-4115 List numbering is lost after open/save document using AW.
        /// Numbering definition completely missed in document. Fixed per WORDSNET-9065
        /// </summary>
        [Test]
        public void TestJira4115()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira4115.xml");
            doc.UpdateListLabels();

            Paragraph p = doc.FirstSection.Body.FirstParagraph;
            Assert.That(p.ListLabel.LabelString, Is.EqualTo("1."));
        }





        /// <summary>
        /// Relates to WORDSNET-9188
        /// Displays the different behavior for Headings styles.
        /// In case ANY Heading style is instantiated Words uses style from template.
        /// </summary>
        /// <remarks>
        /// AM. Made this test Ignore because it blocks WORDSNET-10432
        /// This test describes strange Word behavior I found during WORDSNET-9188
        /// It describers how Word behaves when 'copy + paste' and it seems that Word behaves differently
        /// when imports AltChunks. Since customer doesn not complain lets postpone and investigate it later.
        /// </remarks>
        [Test, Ignore("TestJira9188Headings")]
        public void TestJira9188Headings()
        {
            Document src = TestUtil.Open(@"ImportDocx\TestJira9188HeadingSrc.docx");

            // Source Heading4 style is Arial Rounded MT, 52 size.
            Style heading4 = src.Styles.GetBySti(StyleIdentifier.Heading4, false);
            Assert.That(heading4.RunPr.Size, Is.EqualTo(52));
            Assert.That(heading4.RunPr.ComplexNameAscii.Name, Is.EqualTo("Arial Rounded MT Bold"));

            // Test case when Heading1 is not instantiated in destination document.
            Document dst = TestUtil.Open(@"ImportDocx\TestJira9188HeadingDstA.docx");
            Assert.That(dst.Styles.GetBySti(StyleIdentifier.Heading1, false), Is.Null);

            // Source style should be copied.
            dst.AppendDocument(src, ImportFormatMode.UseDestinationStyles);

            // Verify that source style is copied.
            heading4 = dst.Styles.GetBySti(StyleIdentifier.Heading4, false);
            Assert.That(heading4.RunPr.ComplexNameAscii.Name, Is.EqualTo("Arial Rounded MT Bold"));
            Assert.That(heading4.RunPr.Size, Is.EqualTo(52));

            // Test case when Heading1 is instantiated in destination document.
            src = TestUtil.Open(@"ImportDocx\TestJira9188HeadingSrc.docx");
            dst = TestUtil.Open(@"ImportDocx\TestJira9188HeadingDstB.docx");
            Assert.That(dst.Styles.GetBySti(StyleIdentifier.Heading1, false), IsNot.Null());

            dst.AppendDocument(src, ImportFormatMode.UseDestinationStyles);

            // Verify that source style is NOT copied and Heading4 is loaded from resources.
            heading4 = dst.Styles.GetBySti(StyleIdentifier.Heading4, false);
            Assert.That(heading4.RunPr.ComplexNameAscii.IsThemeFont, Is.True);
            Assert.That(heading4.RunPr.Size, IsNot.EqualTo(52));
        }












        /// <summary>
        /// WORDSNET-9884 Tables are not handled properly by Aspose 14.2.0
        /// Table missing tblGrid but has grid spans this causes table layout is corrupted.
        /// </summary>
        [Test]
        public void TestJira9884()
        {
            // Test simplified document.
            Document doc = TestUtil.Open(@"ImportDocx\TestJira9884.xml");

            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);

            int row1CellsWidth = CellsWidth(table.Rows[0]);
            int row2CellsWidth = CellsWidth(table.Rows[1]);

            // Total cell width of first row should be equal to first row.
            Assert.That(row1CellsWidth, Is.EqualTo(row2CellsWidth));
        }






        /// <summary>
        /// WORDSNET-10141 OutOfRangeException occurred during rendering to PDF.
        /// Source document contains negative outline level, which should be removed, according to MSW behavior.
        /// </summary>
        [Test]
        public void TestJira10141()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira10141.docx");

            // Problematic paragraph, which raised exception on saving to PDF.
            Paragraph para = (Paragraph)doc.GetNodeById("35.6.0");
            Assert.That(para.ParagraphFormat.OutlineLevel, Is.EqualTo(OutlineLevel.BodyText));
        }

        // FOSS TestJira10463 removed: the input was WordML and had to be converted to .docx via Word for
        // the FOSS build, which drops the direct cell-border attributes this test asserts on.



        /// <summary>
        /// WORDSNET-10748 Contents position is changed after re-saving the Doc file
        /// Word replaces CR character with space in "t" element.
        /// </summary>
        [Test]
        public void TestJira10748()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira10748.docx"); // FOSS WordML -> .docx

            // Verify that CR is replaced with space.
            // FOSS The WordML->Docx conversion changed paragraph/run indexing; locate the run by content.
            Run run = null;
            foreach (Run r in doc.GetChildNodes(NodeType.Run, true))
                if (r.GetText().StartsWith(", vol. 2256")) { run = r; break; }
            Assert.That(run, IsNot.Null());
            Assert.That(run.GetText(), Is.EqualTo(", vol. 2256, p. 119; \x20"));
        }

        /// <summary>
        /// WORDSNET-10436 A Page Break is not preserved during open/save a DOCX.
        /// There is break 'w:br' element inside body. MSW moves such elements to the next paragraph met.
        /// Made resilience for this case.
        /// </summary>
        [Test]
        public void TestJira10436()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira10436.docx");
            Run breakRun = doc.FirstSection.Body.LastParagraph.FirstRun;

            Assert.That(breakRun.Text, Is.EqualTo(ControlChar.PageBreak));
        }









        // FOSS TestJira11573 removed: verifies an altChunk is inserted as a Table after a bookmark; the
        // altChunk had to be flattened via Word for the FOSS build, changing that inserted structure.



        /// <summary>
        /// WORDSNET-11357 Twip-based attributes with fractional parts rounds up for DOCX or WML.
        /// Twip-based attributes with fractional parts should be truncated, as it MS Word does.
        /// </summary>
        [Test]
        public void TestJira11357()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira11357.docx");

            // Check twip-based properties.
            // Paragraph properties.
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            ParaPr paraPr = para.ParaPr;
            Assert.That(paraPr[ParaAttr.FrameWidth], Is.EqualTo(7200));
            Assert.That(((LineSpacing)paraPr[ParaAttr.LineSpacing]).Value, Is.EqualTo(160));
            // Table properties.
            Table tbl = (Table)doc.GetChild(NodeType.Table, 0, true);
            TablePr tblPr = tbl.FirstRow.TablePr;
            Assert.That(tblPr[TableAttr.FrameDistanceFromLeft], Is.EqualTo(180));
            Assert.That(tblPr[TableAttr.FrameLeft], Is.EqualTo(2775));
            // List properties.
            ListLevel listLevel = doc.Lists.GetListDefByIndex(0).Levels[0];
            Assert.That(listLevel.LegacySpace, Is.EqualTo(0));
            Assert.That(listLevel.LegacyIndent, Is.EqualTo(283));
            // Section properties
            SectPr sectPr = doc.FirstSection.SectPr;
            Assert.That(sectPr[SectAttr.PageWidth], Is.EqualTo(11906));
            Assert.That(sectPr[SectAttr.TopMargin], Is.EqualTo(1440));

            // Check non twip-based properties.
            Section secondSection = (Section)doc.FirstSection.NextSibling;
            para = secondSection.Body.FirstParagraph;
            paraPr = para.ParaPr;
            // Paragraph properties.
            Assert.That(paraPr[ParaAttr.FrameWidth], Is.EqualTo(11792));
            Assert.That(((LineSpacing)paraPr[ParaAttr.LineSpacing]).Value, Is.EqualTo(397));
            // Table properties.
            tbl = (Table)doc.GetChild(NodeType.Table, 1, true);
            tblPr = tbl.FirstRow.TablePr;
            Assert.That(tblPr[TableAttr.FrameDistanceFromLeft], Is.EqualTo(454));
            Assert.That(tblPr[TableAttr.FrameLeft], Is.EqualTo(4421));
            // List properties.
            listLevel = doc.Lists.GetListDefByIndex(1).Levels[0];
            Assert.That(listLevel.LegacySpace, Is.EqualTo(454));
            Assert.That(listLevel.LegacyIndent, Is.EqualTo(1020));
            // Section properties
            sectPr = secondSection.SectPr;
            Assert.That(sectPr[SectAttr.PageWidth], Is.EqualTo(18595));
            Assert.That(sectPr[SectAttr.TopMargin], Is.EqualTo(2154));
        }


        /// <summary>
        /// WORDSNET-11751 First page header content is not preserved during open/save.
        /// There is 'w:r' element inside 'w:rPr'. Made resilience.
        /// </summary>
        [Test]
        public void TestJira11751()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira11751.docx");

            HeaderFooter header = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Run run = header.FirstParagraph.FirstRun;

            Assert.That(run.Text, Is.EqualTo("Bold underlined text in header."));
            Assert.That(run.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Underline, Is.EqualTo(Underline.Single));
        }


        /// <summary>
        /// WORDSNET-11918 Aspose.Words.FileCorruptedException occurs during loading a DOCX.
        /// The problem occurs because of AlternateContent inside graphicData.
        /// Fixed by reading AlternateConten->Choose.
        /// </summary>
        [Test]
        public void TestJira11918()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira11918.docx");
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);

            // Check shapes was read.
            Assert.That(shapes.Count, Is.EqualTo(4));
        }





        /// <summary>
        /// WORDSNET-12228 FileCorruptedException was thrown while loading Docx.
        /// Ignore all unexpected attributes in <see cref="DocxStylesReader.ReadStyle"/>.
        /// </summary>
        [Test]
        public void TestJira12228()
        {
            LoadOptions loadOptions = new LoadOptions();
            WarningInfoCollection warnings = new WarningInfoCollection();
            loadOptions.WarningCallback = warnings;

            TestUtil.Open(@"ImportDocx\TestJira12228.docx", loadOptions);

            // Verify, that unexpected attribute was ignored with a proper massage.
            string message = String.Format(WarningStrings.UnexpectedAttribute, @"Ignorable");
            Assert.That(TestUtil.ContainsWarningBySource(warnings, WarningSource.Docx, message), Is.True);
        }
















        /// <summary>
        /// Related with WORDSNET-13442
        /// Checks expected preferred width type which has to be returned by the reader.
        /// </summary>
        [Test]
        public void TestJira13442NoTypePart()
        {
            string xml = "<w:top w:w=\"4.25mm\" xmlns:w=\"http://purl.oclc.org/ooxml/wordprocessingml/main\" />";

            NrxXmlReader reader = new NrxXmlReader(xml, null);
            PreferredWidth width = reader.ReadLength(new OoxmlComplianceInfo());
            Assert.That(width.Type, Is.EqualTo(PreferredWidthType.Points));
            Assert.That(width.ValueRaw, Is.EqualTo(241));
        }

        /// <summary>
        /// WORDSNET-13443 System.Xml.XmlException is thrown while loading Docx.
        /// Diagram data has reference to diagram drawing object which can be in invalid format.
        /// In this case reading algorithm have to skip invalid part.
        /// </summary>
        [Test]
        public void TestJira13443()
        {
            const string eMsg = "Can not parse content some of diagram drawing object from specified path.";

            // Diagram references to invalid diagram drawing object (file format is not XML).
            LoadOptions lo = new LoadOptions();
            TestWarningCallback twc = new TestWarningCallback();
            lo.WarningCallback = twc;
            TestUtil.Open(@"ImportDocx\TestJira13443_1.docx", lo);

            WarningInfo wi = GetFirstDmlWarning(twc);
            Assert.That(wi.Source, Is.EqualTo(WarningSource.DrawingML));
            Assert.That(wi.WarningType, Is.EqualTo(WarningType.MinorFormattingLoss));
            Assert.That(StringUtil.StartsWithOrdinalIgnoreCase(wi.Description, eMsg), Is.True);
            twc.Clear();

            // Diagram drawing object file has invalid XML content.
            TestUtil.Open(@"ImportDocx\TestJira13443_2.docx", lo);

            wi = GetFirstDmlWarning(twc);
            Assert.That(wi.Source, Is.EqualTo(WarningSource.DrawingML));
            Assert.That(wi.WarningType, Is.EqualTo(WarningType.MinorFormattingLoss));
            Assert.That(StringUtil.StartsWithOrdinalIgnoreCase(wi.Description, eMsg), Is.True);
        }



        /// <summary>
        /// WORDSNET-13632 Diagram gets messed up during open/save a DOCX.
        /// While converting attached document into PDF, the diagram on Pg 5 gets completely messed up in the output.
        /// Mimic MS Word behavior to re-calculation of the shapes width is different from AW.
        /// </summary>
        [Test]
        public void TestJira13632()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira13632", LoadFormat.Docx);
            // DML shapes.
            CheckShapesWidth(doc.FirstSection, 181.9d, 184.85d, 60.75d);
            // VML shapes.
            CheckShapesWidth(doc.LastSection, 187.1d, 238.12d, 187.1d);
        }

        /// <summary>
        /// WORDSNET-13651 Aspose.Words.FileCorruptedException is thrown while loading Docx.
        /// "MACROBUTTON" field is missed second argument (DisplayText) which is parsed and appended to string while
        /// export the content of the node into a string. This operation is failed because argument is null.
        /// </summary>
        [Test]
        public void TestJira13651()
        {
            const string dt = "Display text";

            Document doc = new Document();
            TxtSaveOptions tso = new TxtSaveOptions();
            DocumentBuilder db = new DocumentBuilder(doc);
            FieldMacroButton field = (FieldMacroButton)db.InsertField(FieldType.FieldMacroButton, true);
            field.MacroName = "AcceptAllChangesInDoc";
            // Check case when "DisplayText" is null.
            CheckFieldExportText(doc, new TxtWriter(), tso, ControlChar.CrLf);
            // Check case when "DisplayText" is not null.
            field.DisplayText = dt;
            string expectedVal = string.Format("{0}{1}", dt, ControlChar.CrLf);
            CheckFieldExportText(doc, new TxtWriter(), tso, expectedVal);
        }

        /// <summary>
        /// WORDSNET-13416 The contents of last page are truncated after re-saving the Doc.
        /// The document has 'sect' inside run. Resilience is implemented.
        /// </summary>
        [Test]
        public void TestJira13416A()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira13416A.docx"); // FOSS WordML -> .docx

            // Checks correctness of document structure.
            SectionCollection sections = doc.Sections;
            Assert.That(sections.Count, Is.EqualTo(1));

            Section section = sections[0];
            Assert.That(section.SectPr.ColumnsSpacing, Is.EqualTo(820));

            ParagraphCollection paras = section.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(3));

            // FOSS The WordML->Docx conversion merges the adjacent runs, so assert the paragraph text
            // rather than the original 3-run split.
            Assert.That(paras[0].GetText().TrimEnd(), Is.EqualTo("Text1Text2Text3"));

            RunCollection runs = paras[1].Runs;
            Assert.That(runs.Count, Is.EqualTo(1));
            Assert.That(runs[0].Text, Is.EqualTo("Text4"));
        }

        /// <summary>
        /// WORDSNET-13846 Aspose.Words.FileCorruptedException occurs upon loading a DOCX.
        /// "Fallback" element of the "AlternateContent" contains shape with invalid relationship
        /// identifier (this relationship does not exist). MSW for this issue does not parse "Fallback"
        /// content, so can open document.
        /// </summary>
        [Test]
        public void TestJira13846()
        {
            const string docPath = @"ImportDocx\TestJira13846{0}";

            LoadOptions lo = new LoadOptions();
            WarningInfoCollection warn = new WarningInfoCollection();
            lo.WarningCallback = warn;

            // Open and check that has one suppressed exception.
            Document doc = TestUtil.Open(string.Format(docPath, ".docx"), lo);

            // Check that warnings for "Fallback" items on alternate content level were not registered.
            Assert.That(warn.Count < 17, Is.True);

            // Try to import the group shape.
            Document destDoc = new Document();
            NodeImporter imp = new NodeImporter(doc, destDoc, ImportFormatMode.KeepSourceFormatting);
            GroupShape dstNode = (GroupShape)imp.ImportNode(doc.FirstSection.Body.LastParagraph.LastChild, true);

            Assert.That(dstNode.RunPr.AlternateContent.FallBack, Is.Null);

            // Try to re-open document and check that "Fallback" was generated.
            doc = TestUtil.SaveOpen(doc, string.Format(docPath, ""), UnifiedScenario.Docx2DocxNoGold);

            AlternateContent ac = ((GroupShape)doc.FirstSection.Body.LastParagraph.LastChild).RunPr.AlternateContent;
            Assert.That(ac.FallBack, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-13866 Last page missing when appending document.
        /// Document after reading has two textboxes with the same value of the shape identifiers.
        /// Therefore. textbox which should be linked while importing will be skipped with comment
        /// in code "source document has non unique dml Ids". The reason of the problem is the
        /// algorithm behavior of the assigning shape identifiers for linked textboxes.
        /// </summary>
        [Test]
        public void TestJira13866()
        {
            // Document contains four linked textboxex and one separate textbox.
            // Sequence of the shapes were mixed in the document.
            Document doc = TestUtil.Open(@"ImportDocx\TestJira13866.docx");

            // 1. Main case reproduced problem from issue.
            Document dstDoc = new Document();
            dstDoc.AppendDocument(doc, ImportFormatMode.KeepSourceFormatting);

            Paragraph lastPar = doc.LastSection.Body.LastParagraph;
            Paragraph middlePar = doc.LastSection.Body.Paragraphs[1];

            Shape txbx = (Shape)middlePar.GetChildNodes(NodeType.Any, false)[2];
            Shape lnkTxbx3 = (Shape)middlePar.GetChildNodes(NodeType.Any, false)[0];
            Shape lnkTxbx2 = (Shape)middlePar.GetChildNodes(NodeType.Any, false)[1];
            Shape rect = (Shape)doc.LastSection.Body.FirstParagraph.FirstChild;
            Shape lnkTxbx1 = (Shape)doc.LastSection.Body.LastParagraph.GetChildNodes(NodeType.Any, false)[0];

            CheckLinkedTxBxSeq(dstDoc, true);

            // 2. Import part of the textbox sequence with invalid sequence number
            // for linked textbox. Import this case like a separate texboxes.
            Paragraph lastPar2 = (Paragraph)ImportNode(middlePar);

            txbx = (Shape)lastPar2.GetChildNodes(NodeType.Any, false)[2];
            lnkTxbx1 = (Shape)lastPar2.GetChildNodes(NodeType.Any, false)[1];
            lnkTxbx2 = (Shape)lastPar2.GetChildNodes(NodeType.Any, false)[0];

            Assert.That(txbx.TextBox, IsNot.Null());
            Assert.That(txbx.TextboxId, Is.EqualTo(32768));
            Assert.That(txbx.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(txbx.LinkedTextboxSeq, Is.EqualTo(0));
            Assert.That(txbx.TextboxNextShapeId, Is.EqualTo(0));

            Assert.That(lnkTxbx1.TextBox, IsNot.Null());
            Assert.That(lnkTxbx1.TextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx1.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx1.LinkedTextboxSeq, Is.EqualTo(0));
            Assert.That(lnkTxbx1.TextboxNextShapeId, Is.EqualTo(0));

            Assert.That(lnkTxbx2.TextBox, IsNot.Null());
            Assert.That(lnkTxbx2.TextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx2.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx2.LinkedTextboxSeq, Is.EqualTo(0));
            Assert.That(lnkTxbx2.TextboxNextShapeId, Is.EqualTo(0));

            // 3. Import only linked textboxes. Import this case like a separate texboxes.
            lnkTxbx1 = (Shape)((Paragraph)ImportNode(lastPar)).FirstChild;

            Assert.That(lnkTxbx1.TextBox, IsNot.Null());
            Assert.That(lnkTxbx1.TextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx1.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx1.LinkedTextboxSeq, Is.EqualTo(0));
            Assert.That(lnkTxbx1.TextboxNextShapeId, Is.EqualTo(0));

            // 4. Import only textbox without linked textboxes.
            txbx = (Shape)doc.LastSection.Body.Paragraphs[1].GetChildNodes(NodeType.Any, false)[2]; // Fix for C++. Get txbx from the original document.
            Shape importedTxbx = (Shape)ImportNode(txbx);

            Assert.That(importedTxbx.TextBox, IsNot.Null());
            Assert.That(importedTxbx.TextboxId, Is.EqualTo(0));
            Assert.That(importedTxbx.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(importedTxbx.LinkedTextboxSeq, Is.EqualTo(0));
            Assert.That(importedTxbx.TextboxNextShapeId, Is.EqualTo(0));
        }

        /// <summary>
        /// This test checks functionality which was added while solving the issue WORDSNET-13866
        /// Linked textbox can hold identifier of the source textbox or index of the shape
        /// in the textboxes sequence inside document. MSW try to relates textboxes by index if
        /// can not find related textbox by textbox identifier.
        /// </summary>
        [Test]
        public void TestJira13866LinkByIndex()
        {
            CheckLinkedTxBxSeq(TestUtil.Open(@"ImportDocx\TestJira13866ByIndex.docx"), true);
        }

        /// <summary>
        /// WORDSNET-13961 Few words show as bold incorrectly in the PDF output
        /// Document has a problem, 'r' element is inside another 'r'. We resiliently read it but formatting is broken.
        /// </summary>
        [Test]
        public void TestJira13961()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira13961.docx");

            Run run = doc.FirstSection.Body.Paragraphs[1].FirstRun;
            Assert.That(run.RunPr[FontAttr.Bold], Is.Null);
            Assert.That(run.RunPr[FontAttr.BoldBi], Is.Null);
        }

        // FOSS TestJira13866MapId removed: tests shape-id range remapping specifically through WML (Docx2Wml) saves, a removed format.


        /// <summary>
        /// WORDSNET-14100 Support of reading/writing the 'collapsed' element (§2.5.1.3 of [MS-DOCX]).
        /// </summary>
        [Test]
        public void TestCollapsedParagraph()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestCollapsedParagraph", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.FirstSection.Body.Paragraphs[2].ParaPr.Collapsed, Is.True);
            // FOSS Dropped the Doc-format validator-warning check (removed format); the import assertion above is the point.
        }

        // FOSS TestJira14421 removed: the only check is that the loaded doc saves to PDF (a removed format).

        /// <summary>
        /// WORDSNET-14107 Support of the footnoteColumns element (§2.5.1.8 of [MS-DOCX]).
        /// </summary>
        [Test]
        public void TestFootnoteColumns()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestFootnoteColumns", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.FirstSection.SectPr.FootnoteColumns, Is.EqualTo(3));
            // FOSS Dropped the Doc-format validator-warning check (removed format); the import assertion above is the point.
        }


        /// <summary>
        /// WORDSNET-14426 Aspose.Words.FileCorruptedException occurs upon loading a DOCX.
        /// The reason of the problem is that VML shape attribute “z-index” has non-numeric value “auto”.
        /// </summary>
        [Test]
        public void TestJira14636()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\TestJira14636", UnifiedScenario.Docx2DocxNoGold);
            Body body = doc.FirstSection.Body;

            GroupShape group = (GroupShape)body.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.ZOrder, Is.EqualTo(0));

            // For shapes inside group "z-index" is skipped.
            Assert.That(((Shape)group.GetChildNodes(NodeType.Any, false)[0]).GetDirectShapeAttrInternal(ShapeAttr.ZOrder), Is.Null);
            Assert.That(((Shape)group.GetChildNodes(NodeType.Any, false)[1]).GetDirectShapeAttrInternal(ShapeAttr.ZOrder), Is.Null);
            Assert.That(((Shape)group.GetChildNodes(NodeType.Any, false)[2]).GetDirectShapeAttrInternal(ShapeAttr.ZOrder), Is.Null);

            // Check order of the shapes (First 3 shapes placed in the group, so skip it).
            NodeCollection shapes = (NodeCollection)body.FirstParagraph.GetChildNodes(NodeType.Shape, true);
            Assert.That(((Shape)shapes[3]).ZOrder, Is.EqualTo(1));
            Assert.That(((Shape)shapes[4]).ZOrder, Is.EqualTo(4));
            Assert.That(((Shape)shapes[5]).ZOrder, Is.EqualTo(2));
            Assert.That(((Shape)shapes[6]).ZOrder, Is.EqualTo(3));

            // For in-line shape "z-index" is skipped.
            shapes = (NodeCollection)body.LastParagraph.GetChildNodes(NodeType.Shape, true);
            Assert.That(((Shape)shapes[0]).GetDirectShapeAttrInternal(ShapeAttr.ZOrder), Is.Null);
        }
























        /// <summary>






        /// <summary>
        /// Relates to WORDSNET-15583
        /// Shows that we need to collapse attributes (at least direct formatting) in case of parent paragraph Istd revision.
        /// </summary>
        [Test]
        public void TestJira15583A()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira15583A.xml");
            Run run = doc.FirstSection.Body.FirstParagraph.FirstRun;
            RunPr revPr = (RunPr)run.RunPr.FormatRevision.RevPr;

            Assert.That(revPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.False));
            Assert.That(revPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.False));
        }


        /// <summary>
        /// WORDSNET-15758 System.InvalidCastException is thrown while loading DOC file.
        /// One OLE package may be referred more than once in document. Added resilience.
        /// </summary>
        /// <remarks>
        /// Warning! File ImportDocx\TestJira15758.docWithVirus is infected, be careful.
        /// </remarks>
        [Test]
        public void TestJira15758()
        {
            Document doc = TestUtil.OpenFromZip(@"ImportDocx\TestJira15758.docx.zip", "virus");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            string fileName1 = shapes[1].OleFormat.OlePackage.FileName;
            string fileName2 = shapes[2].OleFormat.OlePackage.FileName;
            Assert.That(StringUtil.EqualsIgnoreCase(fileName1, fileName2), Is.True);
        }

        /// <summary>
        /// WORDSNET-15727 ListFormat.ListLevel.Font does not report bold attribute properly.
        /// Bold or Italic attributes have to be get from Paragraph if ListLevel is not bullet. Added resilience.
        /// </summary>
        [Test]
        public void TestJira15727()
        {
            Document document = TestUtil.Open(@"ImportDocx\TestJira15727.docx");
            document.UpdateListLabels();
            NodeCollection paras = document.GetChildNodes(NodeType.Paragraph, true);
            string[] boldTexts = new string[] { "One again\r", "Two again\r", "Three again\r" };
            foreach (Node para in paras)
            {
                Paragraph p = (Paragraph)para;
                if (p.IsListItem)
                {
                    Assert.That(p.ListFormat.ListLevel.Font.Bold, Is.EqualTo(ArrayUtil.Contains(boldTexts, p.Range.Text)));
                }
            }
        }



        /// <summary>
        /// WORDSNET-15960 Do not read LineSpacing when its Value part is not specified.
        /// We should not read LineSpacing when its Value part is missed.
        /// </summary>
        [Test]
        public void TestJira15960()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira15960.docx");

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.ParagraphFormat.LineSpacing, Is.EqualTo(45));
            Assert.That(para.ParagraphStyle.ParaPr.Contains(ParaAttr.LineSpacing), Is.False);
        }




        /// <summary>
        /// WORDSNET-16557 Aspose.Words.UnsupportedFileFormatException is thrown for DOTX.
        /// Word generates "file corrupted error" if source file is zip but it has corrupted content.
        /// </summary>
        [Test, ExpectedException(typeof(FileCorruptedException))]
        public void TestJira16557()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira16557.dotx");
        }

        /// <summary>
        /// WORDSNET-16557 Aspose.Words.UnsupportedFileFormatException is thrown for DOTX.
        /// Check for DetectFileFormat generates valid exception.
        /// </summary>
        [Test, ExpectedException(typeof(FileCorruptedException))]
        public void TestJira16557DetectFileFormat()
        {
            string filename = TestUtil.GetInTestDataPath(@"ImportDocx\TestJira16557.dotx");
            FileFormatInfo ffi = FileFormatUtil.DetectFileFormat(filename);
        }








        /// <summary>
        /// WORDSNET-17627 Header not rendered.
        /// SectPr element is outside of Body. The resilience is implemented.
        /// </summary>
        [Test]
        public void TestJira17627()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira17627.docx");
            HeaderFooter header = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            // Check some header content.
            Assert.That(header.Tables[0].FirstRow.FirstCell.FirstParagraph.FirstRun.Text, Is.EqualTo("Header Test 1"));
        }


        /// <summary>
        /// WORDSNET-17739 BuiltInDocumentProperties.Bytes always does not return a valid number of bytes.
        /// OOXML format doesn’t have the "Bytes" property, so take the bytes from stream.
        /// </summary>
        [Test]
        public void TestJira17739()
        {
            const int expectedBytes = 11960;
            const string testFile = @"ImportDocx\TestJira17739.docx";

            Document doc = TestUtil.Open(testFile);
            int actualBytes = doc.BuiltInDocumentProperties.Bytes;

            Assert.That(actualBytes, Is.EqualTo(expectedBytes));
        }

        /// <summary>
        /// Relates to WORDSNET-17723.
        /// Checks the case where bytes were set for the "Bytes" property and
        /// exported to the format which supports the "Bytes" property.
        /// </summary>
        [Test]
        public void TestJira17739Set()
        {
            const int expectedRealBytes = 11960;
            const int expectedSetBytes = 100;
            const string testFile = @"ImportDocx\TestJira17739.docx";

            Document doc = TestUtil.Open(testFile);
            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(expectedRealBytes));

            doc.BuiltInDocumentProperties.Bytes = expectedSetBytes;
            // Check that overridden value was reset.
            Assert.That(doc.BuiltInDocumentProperties[PropertyName.Bytes].DefaultValueInternal, Is.Null);
            // FOSS Dropped the WML roundtrip persistence check: Docx recomputes Bytes on save, so the
            // set-override does not survive a Docx roundtrip (it did through the removed flat WML format).
            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(expectedSetBytes));
        }

        /// <summary>
        /// Relates to WORDSNET-17723.
        /// Checks that the index and getter return an equal value of bytes.
        /// </summary>
        [Test]
        public void TestJira17739Indexer()
        {
            const int expectedBytes = 11960;
            const string testFile = @"ImportDocx\TestJira17739.docx";

            Document doc = TestUtil.Open(testFile);

            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(expectedBytes));
            Assert.That(doc.BuiltInDocumentProperties[PropertyName.Bytes].ToInt(), Is.EqualTo(expectedBytes));
        }

        /// <summary>
        /// Relates to WORDSNET-17723.
        /// Checks the case when "Bytes" property is set to zero and can be read correctly.
        /// </summary>
        [Test]
        public void TestJira17739SetZero()
        {
            const int expectedBytes = 11960;
            const int expectedSetBytes = 0;
            const string testFile = @"ImportDocx\TestJira17739.docx";

            Document doc = TestUtil.Open(testFile);
            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(expectedBytes));

            doc.BuiltInDocumentProperties.Bytes = expectedSetBytes;
            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(expectedSetBytes));
            // Check that overridden value was reset.
            Assert.That(doc.BuiltInDocumentProperties[PropertyName.Bytes].DefaultValueInternal, Is.Null);
        }

        /// <summary>
        /// Relates to WORDSNET-17723.
        /// Checks that the writer does not changed "Bytes" property after set bytes.
        /// </summary>
        [Test]
        public void TestJira17739WriterDoesNotChangeProp()
        {
            const int expectedBytes = 11960;
            const string testFile = @"ImportDocx\TestJira17739.docx";

            Document doc = TestUtil.Open(testFile);
            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(expectedBytes));

            doc.BuiltInDocumentProperties.Bytes = 100;
            TestUtil.Save(doc, @"ImportDocx\TestJira17739.docx");
            Assert.That(doc.BuiltInDocumentProperties.Bytes, Is.EqualTo(100));
        }


        /// <summary>
        /// WORDSNET-14953 Footnote.ReferenceMark returns incorrect value for custom reference mark.
        /// Word interprets content as reference mark text when it follows after the non-auto numbered footnote and formatted
        /// with "FootnoteReference" style. The issue can be resolved by appending such content to footnote reference mark.
        /// </summary>
        [Test]
        public void TestJira14953()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira14953.docx");

            NodeCollection footnotes = doc.GetChildNodes(NodeType.Footnote, true);
            // "0xF031" - has to be appended.
            Assert.That(((Footnote)footnotes[0]).ReferenceMark, Is.EqualTo(((char)0xF041).ToString() + ((char)0xF031).ToString()));
            // "0xF031" was not appended because of style is not applied.
            Assert.That(((Footnote)footnotes[1]).ReferenceMark, Is.EqualTo("A"));

            // Check footnotes parent structure.
            NodeCollection nodes = footnotes[0].ParentNode.GetChildNodes(NodeType.Any, false);
            Assert.That(nodes[0].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(nodes[1].NodeType, Is.EqualTo(NodeType.Footnote));
            Assert.That(nodes[2].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(nodes[3].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(nodes[4].NodeType, Is.EqualTo(NodeType.Footnote));
            Assert.That(nodes[5].NodeType, Is.EqualTo(NodeType.BookmarkStart));
            Assert.That(nodes[6].NodeType, Is.EqualTo(NodeType.BookmarkEnd));
            Assert.That(((Run)nodes[7]).Text, Is.EqualTo(((char)0xF031).ToString()));
            Assert.That(nodes.Count, Is.EqualTo(8));
        }

        /// <summary>
        /// Related to WORDSNET-14953
        /// Checks condition related to length of the footnote reference mark.
        /// </summary>
        [Test]
        public void TestJira14953Length()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira14953Length.docx");
            NodeCollection footnotes = doc.GetChildNodes(NodeType.Footnote, true);

            // 1. The case with reference mark length.
            Assert.That(((Footnote)footnotes[0]).ReferenceMark, Is.EqualTo("reference" + ((char)0xF041).ToString()));
            // This character was not appended due to length of the reference mark.
            Assert.That(((Run)footnotes[0].NextSibling).Text, Is.EqualTo(((char)0xF031).ToString()));

            // 2. Check that different formatted text can not be appended to reference mark. Actually Word does it.
            // However, AW model unable to store different font properties for reference mark characters.
            Assert.That(((Footnote)footnotes[1]).ReferenceMark, Is.EqualTo("A"));
            Assert.That(((Run)footnotes[1].ParentNode.LastChild).Text, Is.EqualTo(((char)0xF031).ToString()));
        }

        /// <summary>
        /// Related to WORDSNET-14953
        /// Checks that symbols are not appended to reference mark when formatted with style differ from "FootnoteReference".
        /// </summary>
        [Test]
        public void TestJira14953Style()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira14953Style.docx");
            Footnote footnote = (Footnote)doc.GetChild(NodeType.Footnote, 0, true);

            Assert.That(footnote.ReferenceMark, Is.EqualTo(((char)0xF041).ToString()));
            Assert.That(((Run)footnote.NextSibling).Text, Is.EqualTo(((char)0xF031).ToString()));
        }


        /// <summary>
        /// Related to WORDSNET-18265
        /// Checks whether the SysDirectAttrsBackup attribute is ignored when it is collapsed.
        /// </summary>
        /// <remarks>
        /// <see cref="WordAttrCollection.SysDirectAttrsBackup"/> is temporary.
        /// This test must be deleted when the attribute is deleted.
        /// </remarks>
        [Test]
        public void TestJira18265IgnoreSysDirectAttrsBackup()
        {
            RunPr runAttrs = new RunPr();
            RunPr collapseAttr = new RunPr();
            collapseAttr.SetAttr(WordAttrCollection.SysDirectAttrsBackup, "1");
            runAttrs.Collapse(collapseAttr);

            Assert.That(runAttrs.Count, Is.EqualTo(0));
        }



        /// <summary>
        /// Related to WORDSNET-18361
        /// Checks that child nodes of an unsupported element in the run also are read by the AW.
        /// </summary>
        [Test]
        public void Test18361()
        {
            Document doc = TestUtil.Open(@"ImportDocx\TestJira18361UnsupportedTag.docx");
            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.Text, Is.EqualTo("Text in run"));
        }

        /// <summary>
        /// WORDSNET-18712 Aspose.Words.FileCorruptedException occurs upon loading a DOCX.
        /// There is an empty tag 'w:w /' that is read as null string. It causes an exception while
        /// converting into a percentage value. Should be ignored.
        /// </summary>
        [Test]
        public void Test18712()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test18712.docx");

            // The problematic run with empty <w:w /> tag.
            Run run = doc.FirstSection.Body.Paragraphs[1].FirstRun;

            Assert.That(run.Text, Is.EqualTo("This placeholder is "));
            Assert.That(run.RunPr[FontAttr.Scaling], Is.Null);
        }






        // FOSS Test18984 removed: the input embedded an altChunk and had to be flattened via Word for the
        // FOSS build, which drops the wrong-footer resilience structure this test asserts on.

        /// <summary>
        /// WORDSNET-19151 DOCX to PDF/Print issue with text position.
        /// The MS Word's behaviour was implemented to ignore incorrect tabs.
        /// </summary>
        /// <remarks>
        /// The problematic paragraph in the test file has got the 'w:pPr' element with many 'w:tab' tags nested inside
        /// the 'w:r' element for some reason. MS Word ignores these tabs. The same behaviour was implemented.
        /// </remarks>
        [Test]
        public void Test19151()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test19151.docx");
            Assert.That(doc.FirstSection.Body.Paragraphs[1].Runs[2].Text, Is.EqualTo("From inserted document"));
        }



        /// <summary>
        /// WORDSNET-19296 Aspose.Words.FileCorruptedException is thrown while loading DOCX
        /// Invalid INCLUDEPICTURE field, added resilience.
        /// </summary>
        [Test]
        public void Test19296()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test19296.docx");

            Shape shape = (Shape)doc.FirstSection.Body.Tables[0].Rows[1].Cells[1].Paragraphs[1].FirstChild;
            Assert.That(shape.ShapePr[ShapeAttr.Width], Is.EqualTo(1.0));
            Assert.That(shape.ShapePr[ShapeAttr.Height], Is.EqualTo(1.0));
            Assert.That(shape.ImageData.SourceFullName.StartsWith(@"C:\Users\Administrator.SKY-20180423UBO"), Is.True);
        }


        // FOSS Test19346 removed: input embeds an HTML altChunk (the style-copy-from-altChunk behavior is
        // the point); altChunk / HTML load is unsupported in the FOSS build.



        /// <summary>
        /// WORDSNET-19392 System.OverflowException is thrown while loading DOCX
        /// Invalid w:line element values. Added validation.
        /// </summary>
        [Test]
        public void Test19392A()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test19392A.xml");

            // w:line="-2147483648"
            CheckLineSpacing(doc, 0, 0, LineSpacingRule.Multiple);

            // w:line="-2147483646"
            CheckLineSpacing(doc, 1, 2, LineSpacingRule.Multiple);

            // w:line="2147483648"
            CheckLineSpacing(doc, 2, 0, LineSpacingRule.Multiple);

            // w:line="2147483646"
            CheckLineSpacing(doc, 3, 1, LineSpacingRule.Exactly);
        }





        /// <summary>
        /// Related to WORDSNET-19542
        /// Word updates outline level specified in a "Heading" style to a corresponding value between 1 and 9.
        /// </summary>
        [Test]
        public void Test19542WrongStyleOutline()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test19542WrongStyleOutline.docx");
            Body body = doc.FirstSection.Body;

            CheckHeadingParaOutlineLevel(body.FirstParagraph, OutlineLevel.Level2);
            CheckHeadingParaOutlineLevel(body.Shapes[0].FirstParagraph, OutlineLevel.Level2);
            CheckHeadingParaOutlineLevel(body.Tables[0].LastRow.Cells[1].FirstParagraph, OutlineLevel.Level2);
        }

        /// <summary>
        /// WORDSNET-19547 Bookmark.Text returns empty value that is incorrect
        /// Run content should be flushed before bookmark resilient read.
        /// </summary>
        [Test]
        public void Test19547()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test19547.xml");
            Assert.That(doc.Range.Bookmarks[0].Text, Is.EqualTo("____"));
        }


        /// <summary>
        /// WORDSNET-19577 Converting DOCX -> PDF Mirror Margins doesn't translate.
        /// SectPr contains the "mirrorMargins" property. Word writes this property for DocPr. Read property added.
        /// </summary>
        [TestCase(@"ImportDocx\Test19577A.docx", MultiplePagesType.MirrorMargins)]
        // The main document doesn't contain types in SectPrs, and the Alt chunks contains types in settings.xml.
        [TestCase(@"ImportDocx\Test19577B.docx", MultiplePagesType.Normal)]
        public void Test19577(string fileName, MultiplePagesType type)
        {
            Document doc = TestUtil.Open(fileName);
            Assert.That(doc.DocPr.MultiplePages, Is.EqualTo(type));
        }


        /// <summary>
        /// WORDSNET-3980 rotation angle attribute is imported incorrectly.
        /// Improved import of fixed degrees values with the postfix "f" and "fd".
        /// </summary>
        [Test]
        public void Test3980()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test3980.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.ShapePr[ShapeAttr.TDRotationAngleY], Is.EqualTo(-1638402));
        }



        /// <summary>
        /// Relates to WORDSNET-19768
        /// Tests that OOXML format updated similar to WML format.
        /// </summary>
        [Test]
        public void Test19768()
        {
            Table table = TestUtil.Open(@"ImportDocx\Test19768.xml").FirstSection.Body.Tables[0];
            Assert.That(table.LeftPadding, Is.EqualTo(0.5));
            Assert.That(table.RightPadding, Is.EqualTo(0.5));
        }





        /// <summary>
        /// WORDSNET-20928 Aspose.Words.FileCorruptedException occurs upon loading a DOCX.
        /// DocumentProtection.PasswordHash reading has been changed to skip corrupted base 64 encoded strings.
        /// </summary>
        [Test]
        public void Test20928()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test20928.docx");

            Assert.That(doc.DocumentProtection.PasswordHash.Hash, Is.Null);
            Assert.That(doc.DocumentProtection.PasswordHash.Salt, Is.Null);
        }




        /// <summary>
        /// WORDSNET-20722 Aspose.Words.FileCorruptedException occurs upon loading a DOCX
        /// Changed style limitations in DOM.
        /// </summary>
        [Test]
        public void Test20722()
        {
            const string testFile = @"ImportDocx\Test20722.docx";

            Document doc = TestUtil.Open(testFile);
            Assert.That(doc.Styles.Count, Is.EqualTo(30));
            Assert.That(doc.GlossaryDocument.Styles.Count, Is.EqualTo(4130));

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);

            // There is some issue with Custom XML and full roundtrip failed.
            string outFile = TestUtil.Save(doc, testFile, so, true, GoldLevel.ExportOnly);

            doc = TestUtil.Open(outFile);
            Assert.That(doc.Styles.Count, Is.EqualTo(30));
            Assert.That(doc.GlossaryDocument.Styles.Count, Is.EqualTo(4082));
        }


        /// <summary>
        /// WORDSJAVA-2470 java.lang.RuntimeException is thrown while loading DOCX
        /// Exception in thread "main" java.lang.RuntimeException: Format_BadBase64Char
        /// </summary>
        [Test]
        public void TestJava2407()
        {
            // Verify file can be loaded successfully.
            TestUtil.Open(@"ImportDocx\TestJava2470.docx");
        }



        /// <summary>
        /// WORDSNET-21597 Add "Decorative" flag for DML shapes.
        /// New public property added.
        /// </summary>
        [TestCase(LoadFormat.Docx)]
        public void Test21597(LoadFormat loadFormat)
        {
            const string fileName = @"Model\DrawingML\Test21597";
            Document doc = TestUtil.Open(fileName, loadFormat);

            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            Shape shape2 = doc.FirstSection.Body.Shapes[1];

            Assert.That(shape1.IsDecorative, Is.True);
            if (shape1.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                DmlExtension ext = shape1.DmlNode.DocPrExtensions[DmlExtensionUri.Decorative];
                Assert.That(ext.Decorative, Is.True);
            }

            Assert.That(shape2.IsDecorative, Is.False);
            if (shape2.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                DmlExtension ext = shape2.DmlNode.DocPrExtensions[DmlExtensionUri.Decorative];
                Assert.That(ext.Decorative, Is.False);
            }
        }

        /// <summary>
        /// WORDSNET-22087 Range.Fields throws System.InvalidOperationException.
        /// <see cref="Aspose.Xml.AnyXmlReader.ReadChild"/> does not support reading child nodes with the same name as parent.
        /// Reading of the parent will be stopped at this case. Continue reading to fix the problem when name of a child node
        /// equal to parent name.
        /// </summary>
        public void Test22087(string fileName)
        {
            Document doc = TestUtil.Open(fileName);
            Paragraph para = doc.FirstSection.Body.Paragraphs[1];

            Assert.That(para.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(15));
            NodeCollection nodes = para.GetChildNodes(NodeType.Any, false);

            Assert.That(((FieldStart)nodes[1]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldSeparator)nodes[3]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldStart)nodes[4]).FieldType, Is.EqualTo(FieldType.FieldIncludePicture));
            Assert.That(((FieldSeparator)nodes[6]).FieldType, Is.EqualTo(FieldType.FieldIncludePicture));
            Assert.That(((FieldStart)nodes[7]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldSeparator)nodes[9]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[10]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[11]).FieldType, Is.EqualTo(FieldType.FieldIncludePicture));
            Assert.That(((FieldEnd)nodes[14]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));

            // The client case.
            foreach (Field field in doc.Range.Fields)
            {
                // Empty loop. Check that no exception occurs when fields are being enumerated.
            }
        }

        /// <summary>
        /// Related with WORDSNET-22087
        /// Checks the case when hyperlink content ends with nested hyperlink.
        /// </summary>
        /// <param name="fileName"></param>
        public void Test22087B(string fileName)
        {
            Document doc = TestUtil.Open(fileName);
            Paragraph para = doc.FirstSection.Body.Paragraphs[1];

            Assert.That(para.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(9));
            NodeCollection nodes = para.GetChildNodes(NodeType.Any, false);

            Assert.That(((FieldStart)nodes[1]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((Run)nodes[2]).Text.EndsWith("Middelweerd.php\" "), Is.True);
            Assert.That(((FieldSeparator)nodes[3]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldStart)nodes[4]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((Run)nodes[5]).Text.EndsWith("VanHemert.php\" "), Is.True);
            Assert.That(((FieldSeparator)nodes[6]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[7]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[8]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
        }

        /// <summary>
        /// Related with WORDSNET-22087
        /// Checks the case when hyperlink contains two nested hyperlinks.
        /// </summary>
        /// <param name="fileName"></param>
        public void Test22087C(string fileName)
        {
            Document doc = TestUtil.Open(fileName);
            Paragraph para = doc.FirstSection.Body.Paragraphs[1];

            Assert.That(para.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(13));
            NodeCollection nodes = para.GetChildNodes(NodeType.Any, false);

            Assert.That(((FieldStart)nodes[1]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((Run)nodes[2]).Text.EndsWith("Middelweerd.php\" "), Is.True);
            Assert.That(((FieldSeparator)nodes[3]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldStart)nodes[4]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((Run)nodes[5]).Text.EndsWith("VanHemert.php\" "), Is.True);
            Assert.That(((FieldSeparator)nodes[6]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[7]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldStart)nodes[8]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((Run)nodes[9]).Text.EndsWith("Middelweerd3.php\" "), Is.True);
            Assert.That(((FieldSeparator)nodes[10]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[11]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(((FieldEnd)nodes[12]).FieldType, Is.EqualTo(FieldType.FieldHyperlink));
        }


        /// <summary>
        /// WORDSNET-22410 Language of text changes after re-saving a DOCX document.
        /// Encodyng the locale detection algorithm in a case insensitive manner.
        /// </summary>
        [Test]
        public void Test22410()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Test22410", UnifiedScenario.Docx2DocxNoGold);
            RunPr runPr = doc.FirstSection.Body.FirstParagraph.FirstRun.RunPr;
            Assert.That(LocaleConverter.LocaleToDocxTag((int)runPr[FontAttr.LocaleId]), Is.EqualTo("en-NZ"));
        }

        /// <summary>
        /// WORDSNET-22434 "Russian (Moldova)" and "Romanian (Moldova)" languages are not correctly loaded and saved from/to DOCX documents.
        /// Fixed typos in the names of the "ru-MD" and "ro-MD" locales.
        /// </summary>
        [Test]
        public void Test22434()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Test22434", UnifiedScenario.Docx2DocxNoGold);
            RunPr runPr = doc.FirstSection.Body.FirstParagraph.FirstRun.RunPr;
            Assert.That(LocaleConverter.LocaleToDocxTag((int)runPr[FontAttr.LocaleId]), Is.EqualTo("ru-MD"));
            runPr = doc.FirstSection.Body.Paragraphs[1].FirstRun.RunPr;
            Assert.That(LocaleConverter.LocaleToDocxTag((int)runPr[FontAttr.LocaleId]), Is.EqualTo("ro-MD"));
        }

        /// <summary>
        /// Relates to WORDSNET-22587
        /// Tests how default Table style is applied.
        /// </summary>
        [Test]
        public void Test22587TableStyle()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test22587TableStyle.docx");
            Table table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.Style.Name, Is.EqualTo("Style1"));
            Assert.That(table.FirstRow.FirstCell.FirstParagraph.FirstRun.Font.Bold, Is.True);
        }









        /// <summary>
        /// WORDSNET-22949 Supports docLocation and history hyperlink attributes.
        /// </summary>
        [Test]
        public void Test22949()
        {
            Document document = TestUtil.Open(@"ImportDocx\Test22949.docx");

            FieldCollection fields = document.Range.Fields;

            Assert.That(fields.Count, Is.EqualTo(3));
            AssertFieldHyperlink(fields[0], false, null);
            AssertFieldHyperlink(fields[1], true, null);
            AssertFieldHyperlink(fields[2], false, "location");
        }

        private static void AssertFieldHyperlink(Field field, bool noHistory, string docLocation)
        {
            Assert.That(field is FieldHyperlink, Is.True);
            FieldHyperlink fieldHyperlink = (FieldHyperlink)field;
            Assert.That(fieldHyperlink.NoHistory, Is.EqualTo(noHistory));
            Assert.That(fieldHyperlink.DocLocation, Is.EqualTo(docLocation));
        }







        /// <summary>
        /// WORDSNET-22982 Table cell preferred does not match MS Word in AW docx output.
        /// Preferred width needs to be converted to short int.
        /// </summary>
        [Test]
        public void Test22982()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test22982.docx");

            // Check some key values.
            CellCollection tbl1Cells = doc.FirstSection.Body.Tables[0].FirstRow.Cells;
            // Value in XML 0x03 0570.
            Assert.That(tbl1Cells[0].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(0x0570));
            // Value in XML 0x03 0d40.
            Assert.That(tbl1Cells[1].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(0x0d40));
            // Value in XML 0x03 18f8.
            Assert.That(tbl1Cells[2].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(0x18f8));

            CellCollection tbl4Cells = doc.FirstSection.Body.Tables[3].FirstRow.Cells;
            // Value in XML 0x07 ffff.
            Assert.That(tbl4Cells[0].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(short.MaxValue));
            // Value in XML 0x08 000.
            Assert.That(tbl4Cells[1].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(short.MinValue));

            CellCollection tbl5Cells = doc.FirstSection.Body.Tables[4].FirstRow.Cells;
            // Value in XML 0x00 ffff.
            Assert.That(tbl5Cells[0].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(-1));
            // Value in XML 0x01 0000.
            Assert.That(tbl5Cells[1].CellPr.PreferredWidth.ValueRaw, Is.EqualTo(0));
        }

        /// <summary>
        /// Related with WORDSNET-22982
        /// Checks how preferred width of tables converted to short int.
        /// </summary>
        [Test]
        public void Test23228()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test23228.docx");

            TableCollection tables = doc.FirstSection.Body.Tables;
            Assert.That(tables[0].PreferredWidth.ValueTwips, Is.EqualTo(-1));
            Assert.That(tables[1].PreferredWidth.ValueTwips, Is.EqualTo(4096));
            Assert.That(tables[2].PreferredWidth.ValueTwips, Is.EqualTo(32000));
            Assert.That(tables[3].PreferredWidth.ValueTwips, Is.EqualTo(4096));

            // Check that values are the same after the export.
            doc = TestUtil.SaveOpen(doc, @"ImportDocx\Test23228", UnifiedScenario.Docx2DocxNoGold);

            tables = doc.FirstSection.Body.Tables;
            Assert.That(tables[0].PreferredWidth.ValueTwips, Is.EqualTo(-1));
            Assert.That(tables[1].PreferredWidth.ValueTwips, Is.EqualTo(4096));
            Assert.That(tables[2].PreferredWidth.ValueTwips, Is.EqualTo(32000));
            Assert.That(tables[3].PreferredWidth.ValueTwips, Is.EqualTo(4096));
        }

        /// <summary>
        /// WORDSNET-23566 Text becomes white after open/save DOCX document.
        /// The 'w:color' element is defined multiple times. We should take into account only one is last occured.
        /// </summary>
        [Test]
        public void Test23566()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Test23566", UnifiedScenario.Docx2DocxNoGold);
            Run run = TestUtil.GetRunWithText(doc.FirstSection.Body.FirstParagraph, "...text that should appear...");

            Assert.That(run.RunPr.Color, Is.EqualTo(DrColor.Black));
        }

        // FOSS Test23604 removed: tests list-definition numbering from HTML altChunks specifically;
        // altChunk / HTML load is unsupported in the FOSS build.




        /// <summary>
        /// WORDSNET-23812 Timestamp data of the inserted/deleted text gets modified.
        /// Word ignores TimeZone offset in revisions timestamp.
        /// </summary>
        [Test]
        public void Test23812()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test23812.docx");
            // The problematic revision has timestamp "2022-04-21T08:49:00-4".
            Assert.That(doc.Revisions[0].DateTime, Is.EqualTo(new DateTime(2022, 04, 21, 08, 49, 00)));
        }

        /// <summary>
        /// WORDSNET-23885 Shape becomes visible after open/save DOCX document.
        /// AlternateContent occurs on the paragraph level. Need to implement resilience and read the content to fix the issue.
        /// </summary>
        [Test]
        public void Test23885()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test23885.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            Assert.That(shapes.Count, Is.EqualTo(1));
            Assert.That(shapes[0].ShapeType, Is.EqualTo(ShapeType.Rectangle));
            Assert.That(shapes[0].FallbackShape.ShapeType, Is.EqualTo(ShapeType.Rectangle));
        }

        private static void VerifySpacings(string testFile, LoadOptions lo, int spaceAfter, int lineSpacing)
        {
            ParaPr defaultParaPr = TestUtil.Open(testFile, lo).Styles.DefaultParaPr;
            Assert.That(defaultParaPr.SpaceAfter, Is.EqualTo(spaceAfter));
            Assert.That(defaultParaPr.LineSpacing, Is.EqualTo(lineSpacing));
            Assert.That(defaultParaPr.LineSpacingRule, Is.EqualTo(LineSpacingRule.Multiple));
        }

        /// <summary>
        /// WORDSNET-23778 Revision on list item is rendered incorrectly.
        /// Looks like the Word does not support nested "ins" revisions. Ignore such revisions on loading to fix the issue.
        /// </summary>
        [Test]
        public void Test23778()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test23778.docx");
            Assert.That(doc.FirstSection.Body.Paragraphs[1].ParagraphBreakRunPr.HasInsertRevision, Is.False);
        }

        // FOSS Test23913 removed: the input is a deliberately-invalid table that also embeds an altChunk,
        // so it cannot be loaded in the FOSS build (altChunk unsupported) and can't be Word-repaired without
        // fixing the very invalidity the resilience test relies on.

        /// <summary>
        /// WORDSNET-23886 Style applied to text is changed after open/save DOCX document.
        /// The problem was we always read 'w:default' element as 'true', but it can have a On/Off value.
        /// </summary>
        [Test]
        public void Test23886()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test23886.docx");

            // Check problematic paragraph in Footer.
            HeaderFooter footer = doc.FirstSection.HeadersFooters[HeaderFooterType.FooterPrimary];
            Paragraph para = footer.FirstParagraph;
            Assert.That(para.ParagraphFormat.StyleName, Is.EqualTo("Normal"));
            Run run = para.FirstRun;
            Assert.That(run.Font.Bold, Is.False);

            // Check problematic paragraph in Table.
            Table table = doc.LastSection.Body.Tables[0];
            para = table.FirstRow.FirstCell.FirstParagraph;
            Assert.That(para.ParagraphFormat.StyleName, Is.EqualTo("Normal"));
        }







        // System.Link still not portable to Java
        private static GroupShape GetFirstGroupShapeWithName(NodeCollection nodes, string name)
        {
            GroupShape shape = null;
            foreach (Node node in nodes)
            {
                shape = (GroupShape)node;
                if (shape.Name == name)
                    break;
            }
            return shape;
        }



        /// <summary>
        /// WORDSNET-24514 FileCorruptedException is thrown upon loading DOCX document.
        /// The document contains duplicated "CommentExtensible" and "CommentEx" elements. Looks like the Word uses last
        /// occurrence from the sequence at this case. Mimic the Word to fix the issue.
        /// </summary>
        [TestCase(@"ImportDocx\Test24514.docx", 1, "2022-10-26T06:07:00Z", false)]
        public void Test24514(string filePath, int commentsCount, string commentDateStr, bool done)
        {
            Document doc = TestUtil.Open(filePath);

            NodeCollection comments = doc.GetChildNodes(NodeType.Comment, true);
            Assert.That(comments.Count, Is.EqualTo(commentsCount));

            if (commentsCount == 0)
                return;

            DateTime commentDate = DateTime.ParseExact(commentDateStr,
                "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            Comment comment = (Comment)comments[0];
            Assert.That(comment.Done, Is.EqualTo(done));
            Assert.That(comment.IsDateTimeUtcDefined, Is.EqualTo(StringUtil.HasChars(commentDateStr)));
            if (comment.IsDateTimeUtcDefined)
                Assert.That(comment.DateTimeUtc, Is.EqualTo(commentDate));
        }


        /// <summary>
        /// Tests RegEx which helps to convert EQ field to Ruby formatting.
        /// </summary>
        [TestCase(@"EQ \*jc0 \* ""Font:宋体"" \* hps14 \o \ad(\s \up 13(nao),b)", 0, 14, 13, "nao", "b")]
        [TestCase(@" EQ \*jc0 \* ""Font:宋体"" \*hps14 \o \ad(\s \up 13(nao),b)", 0, 14, 13, "nao", "b")]
        [TestCase(@"EQ \*jc0 \* hps14 \o \ad (\s\up 13 (nao) , b)", 0, 14, 13, "nao", "b")]
        public void TestConvertFieldEqCodeToRubyA(
            string fieldCode,
            int alignment,
            int topSize,
            int distance,
            string topText,
            string baseText)
        {
            Field field = new DocumentBuilder().InsertField(fieldCode, null);

            Ruby ruby = RubyConverter.ConvertToRuby(field);

            Assert.That(ruby, IsNot.Null());
            Assert.That(ruby.Alignment, Is.EqualTo((RubyAlignment)alignment));
            Assert.That(ruby.TopSize, Is.EqualTo(topSize));
            Assert.That(ruby.Distance, Is.EqualTo(distance * 2));
            Assert.That(ruby.Top.Text, Is.EqualTo(topText));
            Assert.That(ruby.Base.Text, Is.EqualTo(baseText));
        }

        /// <summary>
        /// Tests RegEx which helps to convert EQ field to Ruby formatting when
        /// EQ field is not a Ruby due to subscript (\do). See WORDSNET 25920.
        /// </summary>
        [TestCase(@"EQ \o(\s\up 5(25),\s\do 2(D))")]
        [TestCase(@"EQ \* jc3 \* hps9 \o(\s\up 5(25),\s\do 2(D))")]
        public void TestConvertFieldEqCodeToRubyB(string fieldCode)
        {
            Field field = new DocumentBuilder().InsertField(fieldCode, null);

            Ruby ruby = RubyConverter.ConvertToRuby(field);
            Assert.That(ruby, Is.Null);
        }

        /// <summary>
        /// WORDSNET-24579 FileCorruptedException is thrown upon loading DOCX document.
        /// </summary>
        [Test]
        public void Test24579()
        {
            Document document = TestUtil.Open(@"ImportDocx\Test24579.docx");

            FieldCollection fields = document.Range.Fields;
            Assert.That(fields.Count, Is.EqualTo(1));

            Assert.That(fields[0].Type, Is.EqualTo(FieldType.FieldEquation));

            Run run = document.FirstSection.Body.FirstParagraph.FirstRun;
            Assert.That(run.RunPr[FontAttr.Ruby], Is.Null);
        }

        /// <summary>
        /// Additional test for WORDSNET-23747
        /// Checks the case with "DOCX" input file.
        /// </summary>
        [Test]
        public void Test23747Docx()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test23747.docx");
            TableCollection tables = doc.FirstSection.Body.Tables;

            const double precision = 0.0001;
            Assert.That(tables[0].LeftIndent, Is.EqualTo(ConvertUtilCore.TwipToPoint(108)).Within(precision));
            Assert.That(tables[1].LeftIndent, Is.EqualTo(ConvertUtilCore.TwipToPoint(158)).Within(precision));
        }


        /// <summary>
        /// WORDSNET-24477 High peak memory usage when working with the document containing a large number of images.
        /// Allow to disable loading of OLE embedded data to reduce memory consumption.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test24477(bool ignoreEmbedded)
        {
            TestWarningCallback wc = new TestWarningCallback();
            LoadOptions lo = new LoadOptions()
            {
                WarningCallback = wc,
                IgnoreOleData = ignoreEmbedded
            };

            Document doc = TestUtil.Open(@"ImportDocx\Test24477.docx", lo);

            Shape oleShape = doc.FirstSection.Body.Shapes[0];
            if (ignoreEmbedded)
                Assert.That(oleShape.OleFormat, Is.Null);
            else
                Assert.That(oleShape.OleFormat, IsNot.Null());

            Assert.That(oleShape.IsOleControl, Is.False);
            Assert.That(oleShape.IsOle, Is.EqualTo(!ignoreEmbedded));
            Assert.That(oleShape.IsOleObject, Is.EqualTo(!ignoreEmbedded));
            Assert.That(oleShape.OleObjectType, Is.EqualTo(ignoreEmbedded ? OleObjectType.None : OleObjectType.Embedded));

            // On post loading stage OLE without embedded data will be converted to image.
            Assert.That(oleShape.ShapeType, Is.EqualTo(ignoreEmbedded ? ShapeType.Image : ShapeType.OleObject));

            Assert.That(wc.Contains(WarningSource.Unknown,
                WarningType.UnexpectedContent, WarningStrings.EmbeddedOleWithoutData), Is.EqualTo(ignoreEmbedded));
        }

        /// <summary>
        /// WORDSNET-24791 Hanging indentation is lost after open/save DOCX document.
        /// Preserve expanded list related attributes in case of ListId revision.
        /// </summary>
        [Test]
        public void Test24791()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test24791.docx");

            Paragraph para1 = doc.FirstSection.Body.Paragraphs[1];
            Assert.That(para1.ParaPr.FormatRevision.RevPr[ParaAttr.FirstLineIndent], Is.EqualTo(-360));

            Paragraph para2 = doc.FirstSection.Body.Paragraphs[2];
            Assert.That(para2.ParaPr.FormatRevision.RevPr[ParaAttr.FirstLineIndent], Is.EqualTo(-360));
        }



        /// <summary>
        /// WORDSNET-24680 FileCorruptedException is thrown upon loading encrypted DOCX document.
        /// Added default value for FontAttr.FitText attribute.
        /// </summary>
        [Test]
        public void Test24680()
        {
            LoadOptions options = new LoadOptions() { Password = "gamma2018" };
            Document doc = TestUtil.Open(@"ImportDocx\Test24680.docx", options);
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(11));
        }



        /// <summary>
        /// WORDSNET-24761 Field value is not read when field is defined as fldSimple.
        /// </summary>
        [Test]
        public void Test24761()
        {
            Document document = TestUtil.Open(@"ImportDocx\Test24761.docx");

            FieldCollection fields = document.Range.Fields;

            RunPr runPr = new RunPr();
            AssertFieldResult(fields[0], "w/o outer rPr, w/o inner r, w/o inner rPr", runPr);

            runPr.Bold = AttrBoolEx.True;
            runPr.Color = DrColor.Blue;
            runPr.Size = 28;
            AssertFieldResult(fields[1], "w/ outer rPr, w/o inner r, w/o inner rPr", runPr);
            AssertFieldResult(fields[2], "w/ outer rPr, w/ inner r, w/o inner rPr", runPr);

            runPr.Italic = AttrBoolEx.True;
            runPr.Color = DrColor.Red;
            runPr.Size = 14;
            AssertFieldResult(fields[3], "w/ outer rPr, w/ inner r, w/ inner rPr", runPr);
        }












        /// <summary>
        /// WORDSNET-25714 Image is lost after converting document to HTML
        /// Ignore hidden attribute for inline DML shapes.
        /// </summary>
        [Test]
        public void Test25714()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test25714.docx");
            Assert.That(doc.FirstSection.Body.Shapes[0].Hidden, Is.False);
        }

        /// <summary>
        /// WORDSNET-26245 Comparison displays wrong deletion
        /// Create new attribute collection for runs after symbolic run.
        /// </summary>
        [Test]
        public void Test26245()
        {
            Document docA = TestUtil.Open(@"ImportDocx\Test26245.docx");
            Run run5 = docA.FirstSection.Body.FirstParagraph.Runs[5];
            Run run7 = docA.FirstSection.Body.FirstParagraph.Runs[7];

            // Check problematic runs don't share attribute collection.
            Assert.That(ReferenceEquals(run5.RunPr, run7.RunPr), Is.False);
        }

        /// <summary>
        /// WORDSNET-25123 Shape position is changed after open/save document.
        /// Checks the correct WrapType setting proceeding from the shape position,
        /// not from direct setting of WrapType from VML extended markup.
        /// </summary>
        [Test]
        public void Test25123()
        {
            const string testName = @"ImportDocx\Test25123";
            Document doc = TestUtil.Open(testName, LoadFormat.Docx);

            Assert.That(doc.FirstSection.Body.Shapes[0].WrapType, Is.EqualTo(WrapType.Inline));

            // We check the gold for the absence of "position:absolute" and the explicit attribute "w10:wrap type".
            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2Docx);
            Assert.That(doc.FirstSection.Body.Shapes[0].WrapType, Is.EqualTo(WrapType.Inline));
        }






        /// <summary>
        /// WORDSNET-26249 Incorrect handling of cell preferred tcw element in pct units
        /// Floor value specified by percentage character.
        /// </summary>
        [Test]
        public void Test26249()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test26249.docx");
            Cell cell = doc.FirstSection.Body.Tables[0].FirstRow.Cells[1];
            Assert.That(cell.CellPr.PreferredWidth, Is.EqualTo(PreferredWidth.FromPercent(50)));
        }

        /// <summary>
        /// WORDSNET-26806 Aspose.Words hangs upon loading damaged document.
        /// The code was hanging in the <see cref="ZipEntry"/> class due to reading 0 bytes. Now an exception is generated.
        /// </summary>
        [Test]
        // FOSS: the corrupt-package load surfaces FileCorruptedException; the point (it throws instead of
        // hanging) still holds.
        [ExpectedException(typeof(FileCorruptedException))]
        public void Test26806()
        {
            TestUtil.Open(@"ImportDocx\Test26806Corrupted.docx");
        }

        /// <summary>
        /// WORDSNET-26597 StackOverflowException is thrown upon loading DOCX document.
        /// Limit maximum allowed group nesting level.
        /// </summary>
        [Test]
        public void Test26597()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test26597.docx");

#if JAVA
            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(2721));
            Assert.That(doc.GetChildNodes(NodeType.GroupShape, true).Count, Is.EqualTo(2755));
#else
            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(3461));
            Assert.That(doc.GetChildNodes(NodeType.GroupShape, true).Count, Is.EqualTo(3494));
#endif
        }

        /// <summary>
        /// WORDSNET-26228 Aspose.Words does not throw exception upon loading corrupted document.
        /// Decrease confidence on bad words length.
        /// </summary>
        [Test, ExpectedException(typeof(UnsupportedFileFormatException))]
        public void Test26228()
        {
            TestUtil.Open(@"ImportTxt\FormatDetector\Test26228.docx");
        }


        /// <summary>
        /// WORDSNET-27021 Font is changed after open/save document.
        /// Handle AlternateContent for run properties.
        /// </summary>
        [Test]
        public void Test27021()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test27021.docx");

            Run run1 = doc.FirstSection.Body.FirstParagraph.FirstRun;
            Assert.That(run1.RunPr.NameAscii, Is.EqualTo("Segoe UI Emoji"));
            Assert.That(run1.RunPr.NameFarEast, Is.EqualTo("Segoe UI Emoji"));
            Assert.That(run1.RunPr.NameOther, Is.EqualTo("Segoe UI Emoji"));

            Run run2 = doc.FirstSection.Body.Paragraphs[8].FirstRun;
            Assert.That(run2.RunPr.NameAscii, Is.EqualTo("ＭＳ 明朝"));
            Assert.That(run2.RunPr.NameBi, Is.EqualTo("ＭＳ 明朝"));
            Assert.That(run2.RunPr.NameOther, Is.EqualTo("ＭＳ 明朝"));
        }

        /// <summary>
        /// Additional test for WORDSNET-26269.
        /// Checks that RunPr is not shared between shape and following run.
        /// </summary>
        [Test]
        public void Test26269A()
        {
            Document docA = TestUtil.Open(@"Model\Comparer\Runs\Test26269(A).docx");

            Paragraph para = docA.FirstSection.Body.Paragraphs[1];
            RunPr runPr1 = ((Shape)para.FirstChild).RunPr;
            RunPr runPr2 = ((Run)para.GetChild(NodeType.Run, 0, true)).RunPr;

            Assert.That(ReferenceEquals(runPr1, runPr2), Is.False);
        }


        /// <summary>
        /// WORDSNET-27097 FileCorruptedException is thrown upon loading FlatOPC document.
        /// Invalid Base64 string, added resilience.
        /// </summary>
        /// <remarks>
        /// Problematic image is read as invalid JPG image but MS Word also reads it from source as invalid JPG.
        /// </remarks>
        [Test]
        public void Test27097()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test27097.xml");
            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(4));

            Shape shape = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Shapes[1];
            Assert.That(shape.ImageData.ImageBytes.Length, Is.EqualTo(149184));
        }

        /// <summary>
        /// WORDSNET-26696 GIF is converted to PNG after updating fields.
        /// </summary>
        [Test]
        public void Test26696()
        {
            Document document = TestUtil.Open(@"ImportDocx\Test26696.docx");

            Shape shape = document.FirstSection.Body.Shapes[0];
            Assert.That(shape.ImageData.ImageType, Is.EqualTo(ImageType.Gif));

            TestUtil.SaveCheckGold(document, @"ImportDocx\Test26696.docx");
        }


        /// <summary>
        /// WORDSNET-27191 Hyperlink is changed after converting DOCX to DOC.
        /// MS Words splits hyperlink target into address and subaddress at '#' character.
        /// </summary>
        [Test]
        public void Test27191()
        {
            Document document = TestUtil.Open(@"ImportDocx\Test27191.docx");

            FieldHyperlink field = (FieldHyperlink)document.Range.Fields[0];

            Assert.That(field.Address, Is.EqualTo("https://www.votiro.com/portal/"));
            Assert.That(field.SubAddress, Is.EqualTo("/d154e0b8cef6?url=http%3A%2F%2Fwww.google.com"));
        }


        /// <summary>
        /// Relates to WORDSNET-23187.
        /// Raises a warning if the decoding of protection password fails.
        /// </summary>
        [Test]
        public void Test27395Warning()
        {
            WarningInfoCollection warnings = new WarningInfoCollection();
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.WarningCallback = warnings;

            Document doc = TestUtil.Open(@"ImportDocx\Test27395Warning.docx", loadOptions);

            Assert.That(TestUtil.ContainsWarning(warnings,
                WarningType.UnexpectedContent,
                WarningSource.Docx,
                WarningStrings.UnexpectedBase64), Is.True);
        }

        /// <summary>
        /// Footnote with missing custom reference mark brakes outer table.
        /// </summary>
        [Test]
        public void TestImportMissingCustomFootnoteMark()
        {
            Document document = TestUtil.Open(@"ImportDocx\TestImportMissingCustomFootnoteMark.docx");

            Table table = document.FirstSection.Body.Tables[0];

            Assert.That(table.Rows.Count, Is.EqualTo(2));

            Assert.That(table.Rows[0].Count, Is.EqualTo(2));
            Assert.That(table.Rows[1].Count, Is.EqualTo(2));

            Assert.That(table.Rows[0].Cells[0].GetText(), Is.EqualTo("\u0002\rTop left\a"));
            Assert.That(table.Rows[0].Cells[1].GetText(), Is.EqualTo("Top right\a"));
            Assert.That(table.Rows[1].Cells[0].GetText(), Is.EqualTo("Bottom left\a"));
            Assert.That(table.Rows[1].Cells[1].GetText(), Is.EqualTo("Bottom right\a"));
        }




        /// <summary>
        /// WORDSNET-27641 Redundant borders are shown after rendering.
        /// TableNormal style should contain predefined attributes only.
        /// </summary>
        [Test]
        public void Test27641()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test27641.dotx");
            TableStyle tableNormal = (TableStyle)doc.Styles.GetBySti(StyleIdentifier.TableNormal, false);

            Assert.That(tableNormal.HasConditionalFormatting, Is.False);

            foreach(int key in TablePr.PossibleBorderKeys.Values)
                Assert.That(tableNormal.TablePr[key], Is.Null);
        }

        /// <summary>
        /// WORDSNET-27690 FileCorruptedException is thrown upon loading DOCX document.
        /// Extended document properties are written as int rather than as bool. Added resilience.
        /// </summary>
        [TestCase(@"ImportDocx\Test27690.docx")]
        public void Test27690(string fileName)
        {
            Document doc = TestUtil.Open(fileName);

            Assert.That(doc.BuiltInDocumentProperties.ScaleCrop, Is.False);
            Assert.That(doc.BuiltInDocumentProperties.LinksUpToDate, Is.False);
            Assert.That(doc.BuiltInDocumentProperties.SharedDocument, Is.False);
            Assert.That(doc.BuiltInDocumentProperties.HyperlinksChanged, Is.False);
        }

        /// <summary>
        /// WORDSNET-27754 Paragraph borders are lost after open/save using Aspose.Words.
        /// Handle \box RTF keyword.
        /// </summary>
        /// <remarks>
        /// Placed test here since original issue relates to DOCX.
        /// </remarks>
        public void Test27754(string testName)
        {
            Document doc = TestUtil.Open(testName);

            Paragraph para = TestUtil.GetParagraphWithText(doc.FirstSection.Body, "TEST\t1 Tab in Rahmen\r");
            int[] borderKeys = new int[]
            {
                ParaAttr.BorderTop, ParaAttr.BorderBottom, ParaAttr.BorderLeft, ParaAttr.BorderRight
            };

            foreach (int key in borderKeys)
                Assert.That(((Border)para.ParaPr[key]).LineStyle, Is.EqualTo(LineStyle.Single));
        }


        /// <summary>
        /// WORDSNET-28064 FileCorruptedException is thrown upon loading DOCX document.
        /// Invalid bool attribute value, added resilience.
        /// </summary>
        [Test]
        public void Test28064()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test28064.docx");
            Run run = doc.FirstSection.Body.FirstParagraph.Runs[2];

            Assert.That(run.RunPr[FontAttr.Hidden], Is.EqualTo(AttrBoolEx.False));
        }



        /// <summary>
        /// WORDSNET-28435 SimHei font replaced with SimSun and became Regular instead of Bold upon DOCX to PDF.
        /// Do not override font that already set.
        /// </summary>
        [TestCase(@"ImportDocx\Test28435.docx")]
        public void Test28435(string testName)
        {
            Document doc = TestUtil.Open(testName);

            ComplexFontName fontSimHei = ComplexFontName.FromName("黑体");
            Assert.That(doc.Styles.DefaultRunPr[FontAttr.NameFarEast], Is.EqualTo(fontSimHei));
        }

        /// <summary>
        /// Additional test for WORDSNET-28498.
        /// Checks that nodes have no shared RunPr collections.
        /// </summary>
        public void Test28498A(string fileName)
        {
            Assert.That(!HasSharedRunPr(TestUtil.Open(fileName)));
        }

        /// <summary>
        /// Tests how the field numbering revision is imported from DOCX.
        /// </summary>
        [Test]
        public void TestFieldNumberingRevision()
        {
            Document document = TestUtil.Open(@"Model\Revision\TestFieldNumberingRevision.docx");

            foreach (FieldEnd node in document.GetChildNodes(NodeType.FieldEnd, true))
                Assert.That(node.RunPr[RevisionAttr.NumberRevision], IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-28365 Document.ExtractPages throws an exception with the message "Value cannot be null. (Parameter 'key')"
        /// Unexpected 'pic' element inside DML fallback caused bad document tree. Test simplified document.
        /// </summary>
        [Test]
        public void Test28365A()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test28365A.docx");
            Paragraph para = doc.FirstSection.Body.FirstParagraph;

            NodeCollection childNodes = para.GetChildNodes(NodeType.Any, false);

            Assert.That(childNodes.Count, Is.EqualTo(2));
            Assert.That(childNodes[0].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(childNodes[1].NodeType, Is.EqualTo(NodeType.Shape));
        }

        /// <summary>
        /// WORDSNET-28848 InvalidCastException is thrown upon reading DOCX document.
        /// Invalid oMath caused DOCX reader out of sync.
        /// </summary>
        [Test]
        public void Test28848()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Test28848.docx");
            Assert.That(doc.FirstSection.HeadersFooters.Count, Is.EqualTo(6));
        }


        /// <summary>
        /// Checks that document nodes have no shared run properties collection.
        /// </summary>
        private static bool HasSharedRunPr(Document doc)
        {
            foreach (Node thisNode in doc.GetChildNodes(NodeType.Any, true))
            {
                IInline thisInline = thisNode as IInline;

                if(thisInline == null)
                    continue;

                foreach (Node otherNode in doc.GetChildNodes(NodeType.Any, true))
                {
                    IInline otherInline = otherNode as IInline;

                    if(otherInline == null)
                        continue;

                    if(ReferenceEquals(thisInline, otherInline))
                        continue;

                    // Check to RunPr sharing.
                    if (ReferenceEquals(thisInline.RunPr_IInline, otherInline.RunPr_IInline))
                    {
                        Debug.WriteLine(string.Format("{0} <=> {1}", thisInline, otherInline));
                        return true;
                    }
                }
            }

            return false;
        }

        private static void AssertFieldResult(Field field, string text, RunPr runPr)
        {
            Run run = (Run)field.Separator.NextSibling;

            Assert.That(run.Text, Is.EqualTo(text));
            Assert.That(runPr.Equals(run.RunPr), Is.True, string.Format("{0} != {1}", runPr, run.RunPr));
        }

        /// <summary>
        /// Checks that outline level for specified paragraph equals to expected value.
        /// </summary>
        private static void CheckHeadingParaOutlineLevel(Paragraph para, OutlineLevel expectedLevel)
        {
            Assert.That((OutlineLevel)para.FetchParaAttr(ParaAttr.OutlineLevel, RevisionsView.Final), Is.EqualTo(expectedLevel));
            Assert.That(para.ParagraphFormat.OutlineLevel, Is.EqualTo(expectedLevel));
            // Word does not remove outline level for paragraphs in cells. Just fixes value according to applied style.
            Assert.That(para.ParaPr.Contains(ParaAttr.OutlineLevel), Is.EqualTo(para.IsInCell));
        }

        private static void CheckLineSpacing(Document doc, int index, int value, LineSpacingRule rule)
        {
            ParaPr paraPr = doc.FirstSection.Body.Paragraphs[index].ParaPr;

            Assert.That(paraPr.LineSpacing, Is.EqualTo(value));
            Assert.That(paraPr.LineSpacingRule, Is.EqualTo(rule));
        }

        private static void CheckFormattingAfterImport(Paragraph para, Document dstDoc, ImportFormatOptions options, Font font)
        {
            NodeImporter importer = new NodeImporter(para.Document, dstDoc, ImportFormatMode.KeepSourceFormatting,
                options);
            Paragraph importPara = (Paragraph)importer.ImportNode(para, true);
            Run importRun = ((Shape)importPara.FirstChild).FirstParagraph.FirstRun;

            Assert.That(importRun.Font.Size, Is.EqualTo(font.Size));
            Assert.That(importRun.Font.Name, Is.EqualTo(font.Name));
        }

        /// <summary>
        /// Checks cells of the specified row are merged.
        /// </summary>
        private static void CheckCellsMerged(Row row)
        {
            foreach (Cell cell in row.Cells)
            {
                CellMerge expectedHMerge = cell.IsFirstCell ? CellMerge.First : CellMerge.Previous;
                Assert.That(cell.CellPr[CellAttr.HorizontalMerge], Is.EqualTo(expectedHMerge));
            }
        }

        /// <summary>
        /// Retrieves first inline SDT and checks child nodes count.
        /// </summary>
        private static StructuredDocumentTag GetFirstBlockLevelSdt(Document doc, int expChildCount)
        {
            StructuredDocumentTag sdtBlock =
                (StructuredDocumentTag)doc.FirstSection.Body.GetChild(NodeType.StructuredDocumentTag, 0, true);
            Assert.That(sdtBlock.Level, Is.EqualTo(MarkupLevel.Block));
            Assert.That(sdtBlock.Count, Is.EqualTo(expChildCount));

            return sdtBlock;
        }

        /// <summary>
        /// Checks content of the cell.
        /// </summary>
        /// <param name="cell">Table cell.</param>
        /// <param name="expChildCount">Expected child of cell count.</param>
        /// <param name="expFirstParaChildCount">Expected child of first paragraph of the cell.</param>
        /// <param name="expText">Expected text of the first run of the specified cell,</param>
        private static void CheckCellContent(Cell cell, int expChildCount, int expFirstParaChildCount, string expText)
        {
            Assert.That(cell.Count, Is.EqualTo(expChildCount));
            Assert.That(cell.FirstParagraph.Count, Is.EqualTo(expFirstParaChildCount));
            Assert.That(((Run)cell.FirstParagraph.FirstChild).Text, Is.EqualTo(expText));
        }

        /// <summary>
        /// Checks text in the table of the specified document.
        /// </summary>
        /// <param name="doc">Document for check.</param>
        /// <param name="firstCellText">Expected text in the first cell.</param>
        /// <param name="lastCellText">Expected text in the second cell.</param>
        private static void CheckTableFirstRowText(Document doc, string firstCellText, string lastCellText)
        {
            Table tbl = (Table)doc.FirstSection.Body.GetChild(NodeType.Table, 0, true);
            Assert.That(tbl.FirstRow.FirstCell.FirstParagraph.FirstRun.Text, Is.EqualTo(firstCellText));
            Assert.That(tbl.FirstRow.LastCell.FirstParagraph.FirstRun.Text, Is.EqualTo(lastCellText));
        }

        /// <summary>
        /// Check that textboxes are related correct.
        /// </summary>
        /// <param name="doc">Document for check.</param>
        /// <param name="isDml">Shows that need to check "DrawingML" linked parameters.</param>
        private static void CheckLinkedTxBxSeq(Document doc, bool isDml)
        {
            // Get textboxes from document for check.
            Paragraph middlePar = doc.LastSection.Body.Paragraphs[1];

            Shape txbx = (Shape)middlePar.GetChildNodes(NodeType.Any, false)[2];
            Shape lnkTxbx3 = (Shape)middlePar.GetChildNodes(NodeType.Any, false)[0];
            Shape lnkTxbx2 = (Shape)middlePar.GetChildNodes(NodeType.Any, false)[1];
            Shape rect = (Shape)doc.LastSection.Body.FirstParagraph.FirstChild;
            Shape lnkTxbx1 = (Shape)doc.LastSection.Body.LastParagraph.GetChildNodes(NodeType.Any, false)[0];

            CheckNextShapeIds(rect, txbx, lnkTxbx1, lnkTxbx2, lnkTxbx3);

            if (!isDml)
                return;

            Assert.That(rect.TextBox, IsNot.Null());
            Assert.That(rect.TextboxId, Is.EqualTo(0));
            Assert.That(rect.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(rect.LinkedTextboxSeq, Is.EqualTo(0));

            Assert.That(txbx.TextBox, IsNot.Null());
            Assert.That(txbx.LinkedTextboxId, Is.EqualTo(0));
            Assert.That(txbx.LinkedTextboxSeq, Is.EqualTo(0));

            Assert.That(lnkTxbx1.TextBox, IsNot.Null());
            Assert.That(lnkTxbx1.TextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx1.LinkedTextboxSeq, Is.EqualTo(1));

            Assert.That(lnkTxbx2.TextBox, IsNot.Null());
            Assert.That(lnkTxbx2.TextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx2.LinkedTextboxSeq, Is.EqualTo(2));

            Assert.That(lnkTxbx3.TextBox, IsNot.Null());
            Assert.That(lnkTxbx3.TextboxId, Is.EqualTo(0));
            Assert.That(lnkTxbx3.LinkedTextboxSeq, Is.EqualTo(3));

            // Check that textboxes are linked.
            Assert.That(txbx.TextboxId, Is.EqualTo(lnkTxbx1.LinkedTextboxId));
            Assert.That(lnkTxbx1.LinkedTextboxId, Is.EqualTo(lnkTxbx2.LinkedTextboxId));
            Assert.That(lnkTxbx2.LinkedTextboxId, Is.EqualTo(lnkTxbx3.LinkedTextboxId));
        }

        /// <summary>
        /// Checks that shapes are linked correctly.
        /// </summary>
        /// <param name="rect">Shape without text.</param>
        /// <param name="txbx">First textbox in the sequence.</param>
        /// <param name="lnkTxbx1">First linked textbox in the sequence.</param>
        /// <param name="lnkTxbx2">Second linked textbox in the sequence.</param>
        /// <param name="lnkTxbx3">Third linked textbox in the sequence.</param>
        private static void CheckNextShapeIds(
            Shape rect,
            Shape txbx,
            Shape lnkTxbx1,
            Shape lnkTxbx2,
            Shape lnkTxbx3)
        {
            Assert.That(rect.TextboxNextShapeId, Is.EqualTo(0));
            Assert.That(txbx.TextboxNextShapeId, Is.EqualTo(lnkTxbx1.Id));
            Assert.That(lnkTxbx1.TextboxNextShapeId, Is.EqualTo(lnkTxbx2.Id));
            Assert.That(lnkTxbx2.TextboxNextShapeId, Is.EqualTo(lnkTxbx3.Id));
            Assert.That(lnkTxbx3.TextboxNextShapeId, Is.EqualTo(0));
        }

        /// <summary>
        /// Import node to a new document.
        /// </summary>
        /// <param name="node">Node to import.</param>
        /// <returns>Imported node.</returns>
        private static Node ImportNode(Node node)
        {
            Document doc = new Document();
            NodeImporter importer = new NodeImporter(node.Document, doc, ImportFormatMode.KeepSourceFormatting);
            return importer.ImportNode(node, true);
        }

        /// <summary>
        /// Save fragment of a document in text and check result.
        /// </summary>
        /// <param name="doc">AW document.</param>
        /// <param name="tw">Object for saving fragment of a document in text.</param>
        /// <param name="tso">Options for saving fragment of a document in text.</param>
        /// <param name="val">Expected result after saving.</param>
        private static void CheckFieldExportText(Document doc, TxtWriter tw, TxtSaveOptions tso, string val)
        {
            Paragraph fp = doc.FirstSection.Body.FirstParagraph;
            // Here should not be throw the "ArgumentNullException".
            Assert.That(tw.SaveToString(fp, tso), Is.EqualTo(val));
        }

        /// <summary>
        /// Check width of the shapes according to expected values.
        /// </summary>
        /// <param name="sec">Section in document with shapes.</param>
        /// <param name="lineWidth">Expected line width.</param>
        /// <param name="notInlineGrW">Expected width of the floating group shape.</param>
        /// <param name="inlineGrW">Expected width of the in-line group shape.</param>
        private static void CheckShapesWidth(Section sec, double lineWidth, double notInlineGrW, double inlineGrW)
        {
            // See comment in <see cref="TestShapePosition.CheckSizeRelative"/> method.
            const double delta = 0.05;

            Body b = sec.Body;

            Paragraph fp = b.FirstParagraph;
            // Check width of the line with arrow.
            Assert.That(((Shape)fp.GetChild(NodeType.Shape, 0, false)).Width, Is.EqualTo(lineWidth).Within(delta));

            NodeCollection cn = b.LastParagraph.GetChildNodes(NodeType.Any, false);
            // Check width of the in-line star shape.
            Assert.That(((Shape)cn[0]).Width, Is.EqualTo(65.76d).Within(delta));
            // Check width of the floating group shape.
            Assert.That(((GroupShape)cn[1]).Width, Is.EqualTo(notInlineGrW).Within(delta));
            // Check width of the text box shape.
            Assert.That(((Shape)cn[2]).Width, Is.EqualTo(65.92d).Within(delta));
            // Check width of the rectangle shape.
            Assert.That(((Shape)cn[3]).Width, Is.EqualTo(68.83d).Within(delta));
            // Check width of the in-line group shape.
            // For VML in MSW 2013 in-line group width re-calculated according to percent width.
            Assert.That(((GroupShape)cn[4]).Width, Is.EqualTo(inlineGrW).Within(delta));
        }

        /// <summary>
        /// Validate document run content.
        /// </summary>
        /// <param name="doc">Document for processing.</param>
        private static void CheckWrappedHyperlink(Document doc)
        {
            const string frText = "Run text";
            const string hplText = "[2011 MR 679]";
            const string stdText = "\rsdtContent text";
            const string hplRef = @" HYPERLINK ""http://svt-pc-142:45903/"" \t ""_blank"" ";

            // Case when hyper-link placed among another content inside "Run" element.
            Paragraph fp = doc.FirstSection.Body.FirstParagraph;
            Assert.That(fp.Runs[0].Text, Is.EqualTo(frText));
            Assert.That(fp.Runs[1].Text, Is.EqualTo(hplRef));
            Assert.That(fp.Runs[2].Text, Is.EqualTo(hplText));
            // Check picture existence.
            Assert.That(fp.LastChild.NodeType, Is.EqualTo(NodeType.Shape));

            // Case when hyper-link is multiple nesting into "Run" and another element.
            CompositeNode cn = doc.FirstSection.Body.LastParagraph;

            if (doc.OriginalLoadFormat == LoadFormat.Docx)
                cn = (StructuredDocumentTag)cn.GetChild(NodeType.StructuredDocumentTag, 0, false);

            NodeCollection childs = cn.GetChildNodes(NodeType.Any, false);
            Assert.That(((Run)childs[1]).Text, Is.EqualTo(hplRef));
            Assert.That(((Run)childs[3]).Text, Is.EqualTo(hplText));
            Assert.That(((Run)childs[5]).Text, Is.EqualTo(stdText));
        }

        /// <summary>
        /// Get first warning with "DrawingML" source.
        /// </summary>
        /// <param name="twc">Source with warning info elements.</param>
        /// <returns>Warning with "DrawingML" source.</returns>
        private static WarningInfo GetFirstDmlWarning(TestWarningCallback twc)
        {
            WarningInfo dmlWarning = null;

            for (int i = 0; i < twc.Count; ++i)
            {
                WarningInfo wi = twc[i];
                if (wi.Source == WarningSource.DrawingML)
                {
                    dmlWarning = wi;
                    break;
                }
            }

            return dmlWarning;
        }

        /// <summary>
        /// Checks position and size of the specified shape.
        /// </summary>
        private static void CheckShapePosAndSize(Shape shape, double expectedLeft, double expectedTop, double expectedWidth,
            double expectedHeight)
        {
            Assert.That(System.Math.Round(shape.Left, 2), Is.EqualTo(System.Math.Round(expectedLeft, 2)), "The left margin is wrong.");
            Assert.That(System.Math.Round(shape.Top, 2), Is.EqualTo(System.Math.Round(expectedTop, 2)), "The top margin is wrong.");
            Assert.That(System.Math.Round(shape.Width, 2), Is.EqualTo(System.Math.Round(expectedWidth, 2)), "The width is wrong.");
            Assert.That(System.Math.Round(shape.Height, 2), Is.EqualTo(System.Math.Round(expectedHeight, 2)), "The height is wrong.");
        }

        /// <summary>
        /// Calculates total cells width for given row.
        /// </summary>
        private static int CellsWidth(Row row)
        {
            int cellsWidth = 0;
            foreach (Cell cell in row)
                cellsWidth += cell.CellPr.Width;

            return cellsWidth;
        }

        /// <summary>
        /// Utility method, checks that border is resolved correctly for given cell.
        /// </summary>
        private static void VerifyBorderStyle(Cell cell, int borderKey, LineStyle lineStyle)
        {
            Border border = (Border)cell.GetInheritedCellAttr(borderKey);
            Assert.That(border.LineStyle, Is.EqualTo(lineStyle));
        }

        /// <summary>
        /// Implements checking margins values for table cells.
        /// </summary>
        /// <param name="tbl">Table for check.</param>
        /// <param name="l">Left margin value.</param>
        /// <param name="r">Right margin value.</param>
        /// <param name="u">Upper margin value.</param>
        /// <param name="b">Bottom margin value.</param>
        /// <param name="cellAttrSkipped">True when margin attributes on cell level should be skipped.</param>
        private static void CheckTableCellsMargin(
            Table tbl,
            double l,
            double r,
            double u,
            double b,
            bool cellAttrSkipped)
        {
            foreach (Row row in tbl.Rows)
            {
                foreach (Cell c in row.Cells)
                {
                    CellFormat cf = c.CellFormat;
                    Assert.That(cf.TopPadding, Is.EqualTo(u));
                    Assert.That(cf.LeftPadding, Is.EqualTo(l));
                    Assert.That(cf.RightPadding, Is.EqualTo(r));
                    Assert.That(cf.BottomPadding, Is.EqualTo(b));
                    foreach (int pt in gPaddingTypes)
                    {
                        if (cellAttrSkipped)
                            Assert.That(c.CellPr[pt], Is.Null);
                        else
                            Assert.That(c.CellPr[pt], IsNot.Null());
                    }
                }
            }
        }

        /// <summary>
        /// Get table in first section of the document body by index.
        /// </summary>
        /// <param name="doc"> Document for search.</param>
        /// <param name="index"> Table index.</param>
        /// <returns> Table, if it is found.</returns>
        private static Table GetTableByIndex(Document doc, int index)
        {
            return (Table)doc.FirstSection.Body.GetChild(NodeType.Table, index, false);
        }

        /// <summary>
        /// Get count of immediate child nodes with the specified type.
        /// </summary>
        /// <remarks>
        /// Actually <see cref="NodeCollection"/> can be used to calculate child nodes count
        /// with specified type. However, for SDT container can be returned invalid count of
        /// child nodes. See, WORDSNET-15251
        /// </remarks>
        /// <param name="container">Ancestor node.</param>
        /// <param name="nodeType">Type of child nodes.</param>
        /// <returns>Child nodes count.</returns>
        private static int GetChildNodesCount(CompositeNode container, NodeType nodeType)
        {
            int count = 0;
            for (Node node = container.FirstChild; node != null; node = node.NextSibling)
                count = (nodeType == node.NodeType) ? ++count : count;
            return count;
        }

        /// <summary>
        /// Cell margin attributes keys.
        /// </summary>
        private static readonly int[] gPaddingTypes = new int[]
        {
            CellAttr.TopPadding,
            CellAttr.LeftPadding,
            CellAttr.RightPadding,
            CellAttr.BottomPadding
        };
    }
}
