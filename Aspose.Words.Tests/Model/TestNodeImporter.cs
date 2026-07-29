// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/04/2016 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestNodeImporter
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
        /// WORDSNET-13302 DocumentBuilder.InsertDocument changes the font size of list label in Docx.
        /// Should remove paragraph break properties equal to its style properties.
        /// </summary>
        [Test]
        public void TestJira13302()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());
            // Customer's scenario.
            Style normal = builder.Document.Styles["Normal"];
            normal.Font.Name = "Verdana";
            normal.Font.Size = 8;

            Document importedDoc = TestUtil.Open(@"Model\Nodes\TestJira13302.docx");
            builder.InsertDocument(importedDoc, ImportFormatMode.UseDestinationStyles);

            Paragraph para = builder.Document.FirstSection.Body.Paragraphs[1];
            Assert.That(para.ParagraphBreakRunPr[FontAttr.Size], Is.Null);
            Assert.That(para.ParagraphBreakFont.Size, Is.EqualTo(8));
        }





        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that ALL nodes are expanded, even if attributes are different only in a some single node.
        /// </summary>
        [Test]
        public void TestJira14587CheckNeedExpandAll()
        {
            // The styles in source and destination documents are completely equal.
            Document dstDoc = AppendDocuments("TestJira14587AllStylesDst.xml", "TestJira14587AllStylesSrc.xml");
            CheckAllStylesPreserved(dstDoc.FirstSection.Body.Paragraphs);

            // This source file differs from the above source file only in one property in CharacterStyle.
            dstDoc = AppendDocuments("TestJira14587AllStylesDst.xml", "TestJira14587AllStylesSrcOneStyleDiff.xml");
            CheckAllStylesExpanded(dstDoc.FirstSection.Body.Paragraphs);

            // This source file is the same as previous, but different attribute in CharacterStyle is overridden
            // with the direct attribute in runs. So, expanded attributes are equal in source and destination files.
            // Despite on that, Word expands styles. It seems, Word always expand styles, if at least one of them
            // is different in source and destination documents.
            dstDoc = AppendDocuments("TestJira14587AllStylesDst.xml", "TestJira14587AllStylesSrcOneStyleDiff2.xml");
            CheckAllStylesExpanded(dstDoc.FirstSection.Body.Paragraphs);
        }


        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are not expanded inside header when import whole document.
        /// </summary>
        [Test]
        public void TestJira14587Header()
        {
            // The custom style ParagraphStyle is different in source and destination documents.
            // Source document has header with exactly the same content as in body.
            Document dstDoc = AppendDocuments("TestJira14587AllStylesDst.xml", "TestJira14587AllStylesWithHeaderSrc.xml");

            // Check styles are expanded in body,
            Assert.That(dstDoc.FirstSection.Body.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.False);
            // but are not expanded in header.
            CheckAllStylesPreserved(dstDoc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].Paragraphs);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are expanded inside header when import not whole document.
        /// </summary>
        [Test]
        public void TestJira14587HeaderSingleNode()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira14587AllStylesDst.xml");
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira14587AllStylesWithHeaderSrc.xml");

            NodeImporter nodeImporter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);

            // Import paragraph from header.
            HeaderFooter header = srcDoc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            Paragraph para = (Paragraph)nodeImporter.ImportNode(header.LastParagraph, true);
            Assert.That(para.ParaPr[ParaAttr.LeftIndent], Is.EqualTo(2100));
            Assert.That(para.FirstRun.RunPr[FontAttr.Size], Is.EqualTo(28));
            Assert.That(para.ParaPr.Contains(ParaAttr.Istd), Is.False);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are not expanded inside footnote when import whole document.
        /// </summary>
        [Test]
        public void TestJira14587Footnote()
        {
            // Source document has footnote with style FootnoteReference different from those in destination document.
            Document dstDoc = AppendDocuments("TestJira14587AllStylesExDst.xml", "TestJira14587FootnoteSrc.xml");

            Body body = dstDoc.FirstSection.Body;
            // Check styles are expanded in body,
            Assert.That(body.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.False);
            // but are not expanded in footnote.
            Footnote footnote = (Footnote)body.LastParagraph.LastChild;
            Assert.That(footnote.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are expanded inside footnote when import some single node.
        /// </summary>
        [Test]
        public void TestJira14587FootnoteSingleNodeA()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira14587AllStylesExDst.xml");
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira14587FootnoteSrc.xml");

            NodeImporter nodeImporter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);

            Footnote footnote = (Footnote)srcDoc.GetChild(NodeType.Footnote, 0, true);

            // Import footnote reference.
            Node newNode = nodeImporter.ImportNode(footnote.FirstParagraph.FirstChild, false);
            Assert.That(((SpecialChar)newNode).RunPr[FontAttr.Size], Is.EqualTo(30));

            // Import run from footnote.
            newNode = nodeImporter.ImportNode(footnote.FirstParagraph.FirstRun, false);
            Assert.That(((Run)newNode).RunPr.Contains(FontAttr.Size), Is.False);
            Assert.That(((Run)newNode).RunPr.Contains(FontAttr.SizeBi), Is.False);
        }


        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are not expanded inside comment when import whole document.
        /// </summary>
        [Test]
        public void TestJira14587Comment()
        {
            // Source document has comment with style CommentReference different from those in destination document.
            Document dstDoc = AppendDocuments("TestJira14587AllStylesExDst.xml", "TestJira14587CommentSrc.xml");

            Body body = dstDoc.FirstSection.Body;
            // Check styles are expanded in body,
            Assert.That(body.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.False);
            // but are not expanded in comment.
            Comment comment = (Comment)body.LastParagraph.LastChild;
            Assert.That(comment.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are expanded inside comment when import not whole document.
        /// </summary>
        [Test]
        public void TestJira14587CommentSingleNode()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira14587AllStylesExDst.xml");
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira14587CommentSrc.xml");

            NodeImporter nodeImporter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);

            Comment comment = (Comment)srcDoc.GetChild(NodeType.Comment, 0, true);

            // Import paragraph from comment.
            Paragraph para = (Paragraph)nodeImporter.ImportNode(comment.FirstParagraph, true);
            Assert.That(para.ParaPr[ParaAttr.LeftIndent], Is.EqualTo(2000));
            Assert.That(para.ParagraphBreakRunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Run run = para.FirstRun;
            Assert.That(run.RunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(run.RunPr.Contains(FontAttr.Size), Is.False);

            // Import run from comment (without parent paragraph).
            run = (Run)nodeImporter.ImportNode(comment.FirstParagraph.FirstRun, false);
            Assert.That(run.RunPr.Contains(FontAttr.Size), Is.False);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are not expanded inside shape when import whole document.
        /// </summary>
        [Test]
        public void TestJira14587Shape()
        {
            // The style ParagraphStyle is different in source and destination documents.
            Document dstDoc = AppendDocuments("TestJira14587AllStylesDst.xml", "TestJira14587ShapeSrc.xml");

            Paragraph para = dstDoc.FirstSection.Body.FirstParagraph;
            // Check styles are expanded outside of shape,
            Assert.That(para.ParaPr.Contains(ParaAttr.Istd), Is.False);
            // but are not expanded inside it.
            Shape shape = (Shape)para.FirstChild;
            Assert.That(shape.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are expanded inside shape when import not whole document.
        /// </summary>
        [Test]
        public void TestJira14587ShapeSingleNode()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira14587AllStylesExDst.xml");
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira14587ShapeSrc.xml");

            NodeImporter nodeImporter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);

            Shape shape = (Shape)srcDoc.GetChild(NodeType.Shape, 0, true);

            // Import paragraph from shape.
            Paragraph para = (Paragraph)nodeImporter.ImportNode(shape.FirstParagraph, true);
            Assert.That(para.ParaPr[ParaAttr.LeftIndent], Is.EqualTo(2100));
            Assert.That(para.ParaPr.Count, Is.EqualTo(2)); //LeftIndent and Rsid.
        }


        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that paragraph with BuiltIn style in original revision is not expanded.
        /// </summary>
        [Test]
        public void TestJira14587ParaBuiltinStyleRevision()
        {
            Document dstDoc = AppendDocuments(null, @"TestJira14587ParaBuiltinStyleRevision.docx");

            // If BuiltIn style does not exist in destination document, then it should be created from scratch (not imported!).
            // Run has no revisions, so in this case it is considered as 'final' revision
            // and should be expanded with corresponding flag in NodeImporter.
            Paragraph para = dstDoc.FirstSection.Body.FirstParagraph;
            CheckTestJira14587ParaStyleRevision(para);

            ParaPr paraPr = para.ParaPr;
            Assert.That(paraPr[ParaAttr.Istd], Is.EqualTo(2)); // Heading2
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that paragraph with custom style in original revision is not expanded.
        /// </summary>
        [Test]
        public void TestJira14587ParaUserDefinedStyleRevision()
        {
            Document dstDoc = AppendDocuments(null, "TestJira14587ParaUserDefinedStyleRevision.docx");

            // If custom style does not exist in destination document, then it should be imported.
            // Run has no revisions, so in this case it is considered as 'final' revision
            // and should be expanded with corresponding flag in NodeImporter.
            Paragraph para = dstDoc.FirstSection.Body.FirstParagraph;
            CheckTestJira14587ParaStyleRevision(para);

            ParaPr paraPr = para.ParaPr;
            Assert.That(paraPr[ParaAttr.Istd], Is.EqualTo(15)); // Style1
        }


        /// <summary>
        /// Relates to WORDSNET-14587
        /// Source document has paragraph with custom style, which does not exist inside destination document.
        /// In that case Word does not calculate difference for direct paragraph attributes.
        /// Instead, it imports style with calculated difference and does not remove this style from the paragraph.
        /// Test checks this case.
        /// </summary>
        [Test]
        public void TestJira14587NonExistentCustomStyle()
        {
            Document dstDoc = AppendDocuments(null, "TestJira14587NonExistentCustomStyle.docx");

            Paragraph para = dstDoc.FirstSection.Body.FirstParagraph;
            ParaPr paraPr = para.ParaPr;

            // Istd, RsidP.
            Assert.That(paraPr.Count, Is.EqualTo(2));
            Assert.That(paraPr[ParaAttr.Istd], Is.EqualTo(15)); //User defined style 'Answer'.
            Assert.That(paraPr.Contains(ParaAttr.RsidP), Is.True);

            // RsidR, RsidRpr.
            RunPr runPr = para.ParagraphBreakRunPr;
            Assert.That(runPr.Count, Is.EqualTo(2));
            Assert.That(runPr.Contains(FontAttr.RsidR), Is.True);
            Assert.That(runPr.Contains(FontAttr.RsidRPr), Is.True);

            runPr = para.FirstRun.RunPr;
            Assert.That(runPr.Count, Is.EqualTo(0));

            Style style = para.ParagraphStyle;

            ParaPr goldParaPr = new ParaPr() { SpaceBefore = 80, SpaceAfter = 140, LineSpacing = 220, LineSpacingRule = LineSpacingRule.Exactly, LeftIndent = 389 };
            TestPr(goldParaPr, style.ParaPr, gParaSet1);

            // Source Normal is 'Bold'. So, 'Bold' had to be expanded into style attributes.
            RunPr goldRunPr = new RunPr() { Size = 19, SizeBi = 20, Bold = AttrBoolEx.True };
            TestPr(goldRunPr, style.RunPr, gRunSet1);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that equal to parent table style attributes are removed.
        /// </summary>
        [Test]
        public void TestJira14587ParentTableStyleRemoveEqualAttrs()
        {
            Document dstDoc = AppendDocuments("TestJira14587ParentTableStyleDst.xml", "TestJira14587ParentTableStyleSrc.xml");

            // Paragraphs are inside a table with TableGrid1 style.
            // TableGrid1 style has spacing after=333 and font size=42.
            ParagraphCollection paras = dstDoc.FirstSection.Body.Tables[0].FirstRow.FirstCell.Paragraphs;

            // First paragraph in table has different from table style attributes, so it must be preserved.
            // SpaceAfter=222, break run FontSize=32
            Assert.That(paras[0].ParaPr.SpaceAfter, Is.EqualTo(222));
            Assert.That(paras[0].ParagraphBreakRunPr.Size, Is.EqualTo(32));
            // Second paragraph in table has the same as in table style attributes, so it must be removed.
            // SpaceAfter=333, FontSize=42
            Assert.That(paras[1].ParaPr.Contains(ParaAttr.SpaceAfter), Is.False);
            Assert.That(paras[1].ParagraphBreakRunPr.Contains(FontAttr.Size), Is.False);

            RunCollection runs = paras[0].Runs;
            // First run in table has the same as in table style attributes, so it must be removed.
            // FontSize=42
            Assert.That(runs[0].RunPr.Contains(FontAttr.Size), Is.False);
            // Second run in table has different from table style attributes, so it must be preserved.
            // FontSize=52
            Assert.That(runs[1].RunPr.Size, Is.EqualTo(52));
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are preserved at the nodes,
        /// when they are the same in destination and source documents
        /// </summary>
        [Test]
        public void TestJira14587PreserveStylesAllEquals()
        {
            // The styles in source and destination documents are completely equal.
            Document dstDoc = AppendDocuments("TestJira14587AllStylesDst.xml", "TestJira14587AllStylesSrc.xml");

            ParagraphCollection paras = dstDoc.FirstSection.Body.Paragraphs;

            // First para.
            Paragraph para = paras[0];
            Assert.That(para.ParaPr.Istd, Is.EqualTo(1)); // Heading 1
            Assert.That(para.FirstRun.RunPr.Istd, Is.EqualTo(17)); // Strong

            // Second para.
            Assert.That(paras[1].ParaPr.Istd, Is.EqualTo(16)); // NoSpacing

            // Third para.
            para = paras[2];
            Assert.That(paras[1].ParaPr.Istd, Is.EqualTo(16)); // NoSpacing
            Assert.That(para.ParagraphBreakRunPr.Istd, Is.EqualTo(17)); // Strong
            Assert.That(para.FirstRun.RunPr.Istd, Is.EqualTo(17)); // Strong

            // Last para.
            para = paras[5];
            Assert.That(para.ParaPr.Istd, Is.EqualTo(21)); // ParagraphStyle
            Assert.That(para.ParagraphBreakRunPr.Istd, Is.EqualTo(19)); // CharacterStyle
            Assert.That(para.FirstRun.RunPr.Istd, Is.EqualTo(19)); // CharacterStyle

        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are preserved at the nodes, when they are equal,
        /// despite documents contain other styles that are different, but were not used in imported nodes.
        /// </summary>
        [Test]
        public void TestJira14587CheckOnlyUsedStyles()
        {
            // The destination document contains custom character style that is different from those in source document.
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira14587AllStylesSrc.xml");
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira14587CustomCharStyleDiffDst.xml");

            // Import and append paragraph, which style is equal in source and destination documents (Heading1).
            NodeImporter nodeImporter = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);
            dstDoc.FirstSection.Body.AppendChild(nodeImporter.ImportNode(srcDoc.FirstSection.Body.FirstChild, true));

            // Check despite documents have different styles, the imported paragraph has direct style,
            // i.e. it was not expanded.
            Paragraph para = dstDoc.FirstSection.Body.LastParagraph;
            Assert.That(para.ParaPr.Contains(ParaAttr.Istd), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that if one of the used source styles is different from the style with the same name in destination,
        /// then source styles must be expanded.
        /// </summary>
        [TestCase("TestJira14587BuiltinCharStyleDiffDst.xml")] // There is different BuiltIn character style Strong.
        [TestCase("TestJira14587BuiltinParaStyleDiffDst.xml")] // There is different BuiltIn paragraph style NoSpacing.
        [TestCase("TestJira14587CustomCharStyleDiffDst.xml")] // There is different custom character style CharacterStyle.
        [TestCase("TestJira14587CustomParaStyleDiffDst.xml")] // There is different custom paragraph style ParagraphStyle.
        public void TestJira14587PreserveStylesOneOfStylesDifferent(string dstDocName)
        {
            const string srcDocName = "TestJira14587AllStylesSrc.xml";
            Document dstDoc = AppendDocuments(dstDocName, srcDocName);

            // Check that some styles (Heading1 and Strong) were expanded.
            CheckHeading1AndStrongExpanded(dstDoc.FirstSection.Body.FirstParagraph);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that if used source BuiltIn linked style is missed in destination,
        /// then source styles must be expanded.
        /// </summary>
        [Test]
        public void TestJira14587PreserveStylesMissedBuiltinLinked()
        {
            // The destination document has exactly the same styles as source document,
            // but there is missed BuiltIn linked style Heading1.
            Document dstDoc = AppendDocuments("TestJira14587BuiltinLinkedStyleMissedDst.xml", "TestJira14587AllStylesSrc.xml");

            // Check that some styles (Heading1 and Strong) were expanded.
            CheckHeading1AndStrongExpanded(dstDoc.FirstSection.Body.FirstParagraph);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that when any other than BuiltIn linked style, which is used in source document is missed in destination,
        /// then source styles can be preserved.
        /// </summary>
        [TestCase("TestJira14587CustomLinkedStyleMissedDst.xml")] // There is missed custom linked style LinkedStyle.
        [TestCase("TestJira14587BuiltinParaStyleMissedDst.xml")] // There is missed BuiltIn paragraph style NoSpacing.
        [TestCase("TestJira14587CustomCharStyleMissedDst.xml")] // There is missed custom character style CharacterStyle.
        [TestCase("TestJira14587BuiltinCharStyleMissedDst.xml")] // There is missed BuiltIn character style Strong.
        [TestCase("TestJira14587CustomParaStyleMissedDst.xml")] // There is missed custom paragraph style ParagraphStyle.
        public void TestJira14587PreserveStylesMissedOtherThanBuiltinLinked(string dstDocName)
        {
            const string srcDocName = "TestJira14587AllStylesSrc.xml";
            Document dstDoc = AppendDocuments(dstDocName, srcDocName);
            CheckAllStylesPreserved(dstDoc.FirstSection.Body.Paragraphs);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// It checks that styles are expanded when there are different defaults in documents
        /// and there is an imported node with the custom character style.
        /// </summary>
        [Test]
        public void TestJira14587DifferentDefaultsCustomCharacterStyle()
        {
            // There are different Defaults in these documents and run has custom style CharacterStyle.
            // Despite expanded attributes are equal, Word expands styles.
            Document dstDoc = AppendDocuments("TestJira14587DefaultsDiffDstA.xml", "TestJira14587DefaultsDiffSrcA.xml");

            // Check paragraph does not contain direct style that means it was expanded.
            Assert.That(dstDoc.FirstSection.Body.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.False);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are preserved even when defaults are different.
        /// </summary>
        [Test]
        public void TestJira14587DifferentDefaultsNoCharacterStyle()
        {
            // There are different Defaults in these documents,
            // but run has no character style (so it is DefaultParagraphFont).
            // Expanded attributes are equal, because of paragraph style,
            // so Word preserves all styles.
            Document dstDoc = AppendDocuments("TestJira14587DefaultsDiffDstB.xml", "TestJira14587DefaultsDiffSrcB.xml");
            // Check styles are preserved.
            Assert.That(dstDoc.FirstSection.Body.FirstParagraph.ParaPr.Contains(ParaAttr.Istd), Is.True);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that styles are not expanded in revisions
        /// and style in revision is created instead of been imported.
        /// </summary>
        [Test]
        public void TestJira14587RevisionStyle()
        {
            Document dstDoc = AppendDocuments("TestJira14587RevisionStyleDst.xml", "TestJira14587RevisionStyleSrc.xml");

            Paragraph para = dstDoc.FirstSection.Body.FirstParagraph;

            // Check style is preserved in revision.
            Style style = para.GetParagraphStyle(RevisionsView.Original);
            Assert.That(style.Name, Is.EqualTo("No Spacing"));

            // Check style in revision was created from the scratch instead of importing.
            Assert.That(style.RunPr.Contains(FontAttr.Size), Is.False);
        }

        /// <summary>
        /// Relates to WORDSNET-14587
        /// Checks that UpdateFields() with INCLUDETEXT uses UseDestinationStyles mode.
        /// </summary>
        [Test]
        public void TestJira14587UpdateField()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            string includeDoc = TestUtil.GetInTestDataPath(@"Fields\LinksAndReferences\TestJira14587Include.xml");
            builder.InsertField(string.Format(@"INCLUDETEXT ""{0}""", includeDoc.Replace(@"\", @"\\")));

            doc.UpdateFields();

            builder.MoveToDocumentEnd();
            builder.InsertDocument(TestUtil.Open(includeDoc), ImportFormatMode.KeepSourceFormatting);

            Paragraph para = doc.FirstSection.Body.FirstParagraph;

            Field field = para.Range.Fields[0];
            Run run = (Run)field.Separator.NextSibling;
            Assert.That(run.RunPr.Count, Is.EqualTo(0)); // Nothing is preserved from styles and defaults of the source 'include' file inside field.

            run = ((Paragraph)para.NextSibling).FirstRun;
            Assert.That(run.RunPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromName("Comic Sans MS"))); // Inherited from defaults.
            Assert.That(run.RunPr[FontAttr.Size], Is.EqualTo(36)); // Inherited from Normal.
        }



        /// <summary>
        /// WORDSNET-15561 List numbers missing in subsequent section.
        /// We should preserve style when expanding attributes.
        /// This is a duplicate part of WORDSNET-14587
        /// </summary>
        [Test]
        public void TestJira15561()
        {
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira15561Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira15561Dst.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            Paragraph para = (Paragraph)dstDoc.LastSection.Body.LastParagraph.PreviousNonAnnotationSibling;
            CheckParaStyleAndList(para, "Heading 1", 12, "6.");
        }





        /// <summary>
        /// Relates to WORDSNET-18541 Checks that missed Normal style should be imported from the
        /// source document as well as any other missed style.
        /// </summary>
        [Test]
        public void TestImportNormalStyle()
        {
            Document doc = AppendDocuments(@"Test18541Dst.docx", @"Test18541Src.docx");

            Style normalStyle = doc.Styles["Normal"];
            Assert.That(normalStyle.RunPr.Size, Is.EqualTo(32));
            Assert.That(normalStyle.RunPr.SizeBi, Is.EqualTo(44));
        }

        /// <summary>
        /// WORDSNET-23865 KeepSourceFormatting does not honor source document style.
        /// AW may to merge paragraphs with <see cref="ParaAttr.DropCapPosition"> attribute while saving the document.
        /// This attribute appears in the direct formating after formatting difference calculation due to expanded
        /// style attributes contain global defaults. Exclude global defaults from the style attributes expanding to
        /// fix the issue.
        /// </summary>
        [Test]
        public void Test23865()
        {
            Document src = TestUtil.Open(@"Model\Nodes\Test23865Src.docx");
            Document dst = TestUtil.Open(@"Model\Nodes\Test23865Dst.docx");

            dst.AppendDocument(src, ImportFormatMode.KeepSourceFormatting);

            ParaPr paraPr = dst.LastSection.Body.FirstParagraph.ParaPr;
            Assert.That(paraPr[ParaAttr.DropCapPosition], Is.Null);
            Assert.That(paraPr[ParaAttr.Istd], Is.Null);

            Assert.That(paraPr[ParaAttr.SpaceBefore], Is.EqualTo(400));
            Assert.That(paraPr[ParaAttr.SpaceAfter], Is.EqualTo(120));
        }


        /// <summary>
        /// Related to WORDSNET-18958
        /// Checks case when effective override values are equal however overrides are not identical(one of overrides is empty).
        /// </summary>
        [Test]
        public void Test18958EmptyOverrides()
        {
            Document doc = AppendDocuments(null, @"Test18873A1.docx", @"Test18873B1.docx");

            Assert.That((int)doc.LastSection.Body.FirstParagraph.ParaPr[ParaAttr.ListId], IsNot.EqualTo((int)doc.FirstSection.Body.FirstParagraph.ParaPr[ParaAttr.ListId]));
        }


        /// <summary>
        /// WORDSNET-19927 Incorrect font size and alignment inside an inserted table.
        /// FormattingDifferenceCalculator was improved in order to consider the
        /// "overrideTableStyleFontSizeAndJustification" compatibility setting.
        /// </summary>
        /// <remarks>
        /// Destination document contains a Normal style with "12pt" size and "Left" alignment.
        /// The overrideTableStyleFontSizeAndJustification setting is true.
        /// In this case, these values (12pt, Left) will override the values from the table style (11pt, Right).
        /// It is important to preserve correctly calculated direct attributes in a FormattingDifferenceCalculator,
        /// even if they are equals to the attributes from the table style.
        /// </remarks>
        [Test]
        public void Test19927()
        {
            Document doc = AppendDocuments(@"Test19927Dst.docx", @"Test19927Src.docx");
            Paragraph para = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.FirstParagraph;

            ParaPr paraPr = para.ParaPr;
            Assert.That(paraPr.Contains(ParaAttr.Alignment), Is.True);
            Assert.That(paraPr.Alignment, Is.EqualTo(ParagraphAlignment.Right));

            RunPr runPr = para.FirstRun.RunPr;
            Assert.That(runPr.Contains(FontAttr.Size), Is.True);
            Assert.That(runPr.Size, Is.EqualTo(22));
            Assert.That(runPr.Contains(FontAttr.SizeBi), Is.True);
            Assert.That(runPr.SizeBi, Is.EqualTo(22));
        }

        /// <summary>
        /// WORDSNET-19796 different output using ImportNode (Section) and AppendDocument.
        /// NodeImporter was fixed to not apply format difference for section.
        /// </summary>
        [Test]
        public void Test19796()
        {
            const string expectedFontName = "Times New Roman";

            Document usingAppendDocument = GetOutputUsingAppendDocument();
            Run run = usingAppendDocument.Sections[1].HeadersFooters[4].Paragraphs[1].Runs[0];
            Assert.That(run.Font.Name, Is.EqualTo(expectedFontName));

            Document usingImportNode = GetOutputUsingImportNode();
            run = usingImportNode.Sections[1].HeadersFooters[4].Paragraphs[1].Runs[0];
            Assert.That(run.Font.Name, Is.EqualTo(expectedFontName));
        }


        /// <summary>
        /// WORDSNET-19757 Calibri (Body) was changed to Calibri during appending documents.
        /// </summary>
        [Test]
        public void Test19757()
        {
            ImportFormatOptions importOptions = new ImportFormatOptions()
            {
                SmartStyleBehavior = true,
                KeepSourceNumbering = true
            };
            Document dst = AppendDocuments("Test19757Dst.docx", importOptions, "Test19757Src.docx");

            Paragraph para1 = dst.FirstSection.Body.Paragraphs[0];
            Assert.That(para1.GetText(), Is.EqualTo("AAA / AAA Aaaaaa Aaaaaaa\r"));

            Run run = para1.FirstRun;
            Assert.That(run.RunPr[FontAttr.NameAscii], Is.Null);
            Assert.That(run.RunPr[FontAttr.NameFarEast], Is.Null);
            Assert.That(run.RunPr[FontAttr.NameOther], Is.Null);
            Assert.That(run.RunPr[FontAttr.NameBi], Is.Null);

            // Check that inherited font is MinorAscii theme font.
            ComplexFontName fontName = (ComplexFontName)((IRunAttrSource)run).FetchInheritedRunAttr(FontAttr.NameAscii);
            Assert.That(fontName.IsThemeFont, Is.True);
            Assert.That(fontName.ThemeFontCore, Is.EqualTo(ThemeFontCore.MinorHAnsi));

            Paragraph para2 = dst.FirstSection.Body.Paragraphs[2];
            Assert.That(para2.GetText(), Is.EqualTo("Aaaaa aa aaa.\f"));

            RunPr runPr2 = para2.FirstRun.RunPr;
            Assert.That(((ComplexFontName)runPr2[FontAttr.NameAscii]).Name, Is.EqualTo("Calibri"));
            Assert.That(((ComplexFontName)runPr2[FontAttr.NameFarEast]).Name, Is.EqualTo("Times New Roman"));
            Assert.That(((ComplexFontName)runPr2[FontAttr.NameOther]).Name, Is.EqualTo("Calibri"));
            Assert.That(((ComplexFontName)runPr2[FontAttr.NameBi]).Name, Is.EqualTo("Times New Roman"));
        }

        /// <summary>
        /// WORDSNET-20376 Font style of header/footer content gets lost after appending two documents.
        /// IgnoreHeaderFooter property has been added.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test20376(bool ignoreHeaderFooter)
        {
            Document dstDocument = TestUtil.Open(@"Model\Nodes\Test20376_1.docx");
            Document srcDocument = TestUtil.Open(@"Model\Nodes\Test20376_2.docx");

            ImportFormatOptions importFormatOptions = new ImportFormatOptions();
            importFormatOptions.IgnoreHeaderFooter = ignoreHeaderFooter;

            dstDocument.AppendDocument(srcDocument, ImportFormatMode.KeepSourceFormatting, importFormatOptions);

            HeaderFooter header = dstDocument.Sections[1].HeadersFooters[HeaderFooterType.HeaderFirst];
            HeaderFooter footer = dstDocument.Sections[1].HeadersFooters[HeaderFooterType.FooterFirst];

            int expectedFontSize = ignoreHeaderFooter ? 20 : 22;
            string expectedFontName = ignoreHeaderFooter ? "Times New Roman" : "Arial";

            Assert.That(header.FirstParagraph.FirstRun.RunPr.Size, Is.EqualTo(expectedFontSize));
            Assert.That(header.FirstParagraph.FirstRun.RunPr.Name, Is.EqualTo(expectedFontName));

            Assert.That(footer.FirstParagraph.FirstRun.RunPr.Size, Is.EqualTo(expectedFontSize));
            Assert.That(footer.FirstParagraph.FirstRun.RunPr.Name, Is.EqualTo(expectedFontName));
        }




        /// <summary>
        /// WORDSNET-20912 KeepDifferentStyles behavior changed for Normal style.
        /// The issue duplicates WORDSNET-21101
        /// </summary>
        [Test]
        public void Test20912()
        {
            Document dstDoc = AppendDocuments("Test20912Dst.docx", "Test20912Src.docx", ImportFormatMode.KeepDifferentStyles);

            Paragraph paragraph = dstDoc.FirstSection.Body.FirstParagraph;
            Assert.That(paragraph.ParagraphStyle.Name, Is.EqualTo("Normal_0"));
            Assert.That(paragraph.FirstRun.Font.Name, Is.EqualTo("Arial Narrow"));
            Assert.That(paragraph.FirstRun.Font.Size, Is.EqualTo(8));
        }

        /// <summary>
        /// WORDSNET-21233 Table.InsertAfter throws System.InvalidOperationException.
        /// Attempting to add the Sdt with an already existing Id.
        /// </summary>
        [Test]
        public void Test21233()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\Test21233Dst.docx");
            Table dstTbl = dstDoc.FirstSection.Body.Tables[0];

            Document rowDoc = TestUtil.Open(@"Model\Nodes\Test21233Row.docx");
            Table srcTbl = rowDoc.FirstSection.Body.Tables[0];

            NodeImporter importer = new NodeImporter(rowDoc, dstDoc, ImportFormatMode.UseDestinationStyles);
            dstTbl.InsertAfter(importer.ImportNode(srcTbl.Rows[0], true), dstTbl.LastRow);

            // Trying to insert the Sdt with the same Id.
            dstTbl.InsertAfter(importer.ImportNode(srcTbl.Rows[0], true), dstTbl.LastRow);
            Assert.That(dstTbl.Rows.Count, Is.EqualTo(3));
        }


        /// <summary>
        /// WORDSNET-21485 List numbering updates when KeepSourceNumbering is true (20.12).
        /// Should import even equal styles when 'KeepSourceNumbering' option
        /// is enabled in 'KeepDifferentStyles' importing mode.
        /// </summary>
        [TestCase(true, "6.")]
        [TestCase(false, "7.")]
        public void Test21485(bool isKeepSourceNumbering, string expectedListLabel)
        {
            Document srcDoc = TestUtil.Open(@"Model\Nodes\Test21485.docx");
            Document dstDoc = srcDoc.Clone();

            ImportFormatOptions options = new ImportFormatOptions();
            options.KeepSourceNumbering = isKeepSourceNumbering;

            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepDifferentStyles, options);
            foreach (Node paragraph in srcDoc.FirstSection.Body.Paragraphs.ToArray())
            {
                Node importedNode = importer.ImportNode(paragraph, true);
                dstDoc.FirstSection.Body.AppendChild(importedNode);
            }

            dstDoc.UpdateListLabels();

            Paragraph para = dstDoc.LastSection.Body.LastParagraph;
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(expectedListLabel));
        }

        /// <summary>
        /// WORDSNET-22103 Merging two documents and list numbering doesn't continue.
        /// Reworked AppendDocument method to use internally DocumentInserter instead of just NodeImporter.
        /// The MergePastedLists option is made public.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test22103(bool isMergePastedLists)
        {
            Document srcDoc = TestUtil.Open(@"Model\Nodes\Test22103Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\Nodes\Test22103Dst.docx");

            ImportFormatOptions importFormatOptions = new ImportFormatOptions();
            importFormatOptions.MergePastedLists = isMergePastedLists;
            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles, importFormatOptions);
            dstDoc.UpdateListLabels();

            // Check the original list items of destination document are not changed.
            ParagraphCollection paras = dstDoc.FirstSection.Body.Paragraphs;
            Assert.That(paras[0].ListLabel.LabelString, Is.EqualTo("1."));
            Assert.That(paras[1].ListLabel.LabelString, Is.EqualTo("2."));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo("3."));

            // Check the imported list items of source document either continue the existing list
            // of destination document, or starts a new one depending on MergePastedLists option.
            paras = dstDoc.LastSection.Body.Paragraphs;
            Assert.That(paras[0].ListLabel.LabelString, Is.EqualTo(isMergePastedLists ? "4." : "1."));
            Assert.That(paras[1].ListLabel.LabelString, Is.EqualTo(isMergePastedLists ? "5." : "2."));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo(isMergePastedLists ? "6." : "3."));
            Assert.That(paras[3].ListLabel.LabelString, Is.EqualTo(isMergePastedLists ? "7." : "4."));
        }



        /// <summary>
        /// WORDSNET-23299 ArgumentException is thrown while inserting document.
        /// AW treats SDT range start node as annotation item and tries to move it to inline level of the paragraph.
        /// Stop copying when node with problematic type occurs to fix the issue.
        /// </summary>
        [TestCase(@"Model\Nodes\Test23299.docx")]
        [TestCase(@"Model\Nodes\Test23299Annotations.docx")]
        public void Test23299(string path)
        {
            Document srcDoc = TestUtil.Open(path);
            bool hasBookmarks = srcDoc.FirstSection.Body.FirstChild.NodeType == NodeType.BookmarkStart;

            DocumentBuilder builder = new DocumentBuilder();
            builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            // Check that bookmarks were copied.
            Assert.That(builder.Document.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(hasBookmarks ? 2 : 0));
        }

        /// <summary>
        /// WORDSNET-23366 ImportFormatMode.KeepSourceFormatting behaves differently in AW and MS Word.
        /// We should remove attributes equal to parent table style attributes only when they are not
        /// overriden by the corresponding Normal attributes when calculating formatting difference.
        /// </summary>
        [Test]
        public void Test23366()
        {
            Document doc = AppendDocuments("Test23366Dst.docx", "Test23366Src.docx");

            // The problematic run is located in the first imported cell.
            Run run = doc.LastSection.Body.Tables[0].FirstRow.FirstCell.FirstParagraph.FirstRun;
            Assert.That(((ComplexFontName)run.RunPr[FontAttr.NameAscii]).Name, Is.EqualTo("Calibri"));
        }





        /// <summary>
        /// WORDSNET-24305 Third level numbering is not preserved after appending document.
        /// Should ensure that list level linked style refers to this list level after import. If it is not so,
        /// then mimic Word by setting corresponding numbering properties into direct paragraph attributes.
        /// </summary>
        [Test]
        public void Test24305()
        {
            Document srcDoc = TestUtil.Open(@"Model\Nodes\Test24305Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\Nodes\Test24305Dst.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            // The first inserted paragraph is problematic one.
            Paragraph para1 = dstDoc.LastSection.Body.FirstParagraph;
            // Check listId is set into direct attributes of the paragraph.
            Assert.That(para1.ParaPr.ListId, Is.EqualTo(2));

            // This paragraph does not have the same problem as previous one,
            // so check list id is not set in direct attributes.
            Paragraph para2 = (Paragraph)para1.NextSibling;
            Assert.That(para2.ParaPr[ParaAttr.ListId], Is.Null);

            // At last, check actual list labels.
            dstDoc.UpdateListLabels();
            Assert.That(para1.ListLabel.LabelString, Is.EqualTo("3.1.3."));
            Assert.That(para2.ListLabel.LabelString, Is.EqualTo("3.2."));
        }


        /// <summary>
        /// WORDSNET-25249 Numbering is broken after importing section.
        /// Consider inherited attributes when take a decision either to set list's attributes
        /// into direct paragraph attributes in <see cref="NodeImporter.ImportList"/>.
        /// </summary>
        [Test]
        public void Test25249()
        {
            Document srcDoc = TestUtil.Open(@"Model\Nodes\Test25249.docx");
            Document dstDoc = (Document)srcDoc.Clone(false);

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            dstDoc.UpdateListLabels();
            Paragraph para = dstDoc.FirstSection.Body.Paragraphs[1];
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1.1"));
        }









        /// <summary>
        /// WORDSNET-27734 Paragraphs are not numbered after appending documents with ImportFormatMode.KeepSourceFormatting mode.
        /// Word collapses numbering properties of the destination style over a Normal style and
        /// applies it to the direct properties of imported paragraph. To put it simply, Word sets
        /// ListId=1 and ListLevel=0 in imported paragraph in this case.
        /// </summary>
        [Test]
        public void Test27734()
        {
            Document doc = AppendDocuments("Test27734Dst.docx", "Test27734Src.docx");

            doc.UpdateListLabels();
            Paragraph para = TestUtil.GetParagraphWithText(doc, "...Überschrift 1...");
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("1"));
        }

        /// <summary>
        /// WORDSNET-27678 Font size is changed after importing nodes with ImportFormatMode.KeepSourceFormatting.
        /// We should not collapse revised attributes that exist also in before-changes attributes collection over
        /// a destination 'Normal' style in <see cref="FormattingDifferenceCalculator.GetRunPrDiff"/>.
        /// </summary>
        [Test]
        public void Test27678()
        {
            Document doc = AppendDocuments(null, "Test27678.docx");
            doc = TestUtil.SaveOpen(doc, @"Model\Nodes\Test27678", UnifiedScenario.Docx2DocxNoGold);

            ParagraphCollection paras = doc.LastSection.Body.Paragraphs;
            Assert.That(paras[0].FirstRun.RunPr.HasFormatRevision, Is.False);
            Assert.That(paras[0].FirstRun.RunPr[FontAttr.Size], Is.EqualTo(20));
            Assert.That(paras[1].FirstRun.RunPr.FormatRevision.RevPr[FontAttr.Size], Is.EqualTo(22));
            Assert.That(paras[2].FirstRun.RunPr.FormatRevision.RevPr[FontAttr.Size], Is.EqualTo(24));
            Assert.That(paras[3].FirstRun.RunPr.FormatRevision.RevPr[FontAttr.Size], Is.EqualTo(26));
            Assert.That(paras[4].FirstRun.RunPr.FormatRevision.RevPr[FontAttr.Size], Is.EqualTo(28));
        }







        /// <summary>
        /// Returns Citation source by its Tag name.
        /// </summary>
        private static XmlNode GetCitationSourceByTagName(Document doc, string tagName)
        {
            CustomXmlPart citationSources = doc.CustomXmlParts.GetCitationSources();
            using (MemoryStream stream = new MemoryStream(citationSources.Data))
            {
                XmlDocument xmlDoc = XmlUtilPal.LoadXml(stream, true);
                IDictionary<string, string> xmlNames = new Dictionary<string, string>()
                {
                    { "b", "http://schemas.openxmlformats.org/officeDocument/2006/bibliography" }
                };
                return XmlUtilPal.SelectSingleNode(xmlDoc.DocumentElement, string.Format("b:Source[b:Tag='{0}']", tagName), xmlNames);
            }
        }

        /// <summary>
        /// Returns Title of the specified Citation.
        /// </summary>
        private static string GetCitationTitle(XmlNode citation)
        {
            IDictionary<string, string> xmlNames = new Dictionary<string, string>()
            {
                { "b", "http://schemas.openxmlformats.org/officeDocument/2006/bibliography" }
            };

            return XmlUtilPal.SelectSingleNode((XmlElement)citation, "b:Title", xmlNames).InnerText;
        }

        /// <summary>
        /// Returns Title of the Citation with a specified Tag name.
        /// </summary>
        private static string GetCitationTitle(Document doc, string citationTagName)
        {
            XmlNode citationSource = GetCitationSourceByTagName(doc, citationTagName);
            return GetCitationTitle(citationSource);
        }

        /// <summary>
        /// Returns document for the Test19796 using AppendDocument.
        /// </summary>
        private static Document GetOutputUsingAppendDocument()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\Test19796A.docx");
            Document srcDoc = TestUtil.Open(@"Model\Nodes\Test19796B.docx");

            srcDoc.FirstSection.PageSetup.SectionStart = SectionStart.NewPage;
            srcDoc.FirstSection.HeadersFooters.LinkToPrevious(false);
            srcDoc.FirstSection.PageSetup.RestartPageNumbering = true;
            srcDoc.FootnoteOptions.RestartRule = FootnoteNumberingRule.RestartSection;

            ImportFormatOptions options = new ImportFormatOptions();
            options.KeepSourceNumbering = true;

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting, options);

            return dstDoc;
        }

        /// <summary>
        /// Returns document for the Test19796 using ImportNode (Section).
        /// </summary>
        private static Document GetOutputUsingImportNode()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\Test19796A.docx");
            Document srcDoc = TestUtil.Open(@"Model\Nodes\Test19796B.docx");

            foreach (Section section in srcDoc.Sections)
            {
                Node importNode = dstDoc.ImportNode(section, true, ImportFormatMode.KeepSourceFormatting);
                dstDoc.AppendChild(importNode);
            }

            return dstDoc;
        }

        /// <summary>
        /// Checks paragraph style, listId and list label string.
        /// </summary>
        private static void CheckParaStyleAndList(Paragraph para, string styleName, int listId, string listLabel)
        {
            Assert.That(para.ParagraphStyle.Name, Is.EqualTo(styleName));
            Assert.That(para.ParagraphFormat.ListId, Is.EqualTo(listId));

            Document doc = para.Document as Document;
            Assert.That(doc, IsNot.Null());
            doc.UpdateListLabels();
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(listLabel));
        }

        /// <summary>
        /// This is helper method for checking common properties of paragraph in TestJira14587 family tests.
        /// </summary>
        private static void CheckTestJira14587ParaStyleRevision(Paragraph para)
        {
            ParaPr revParaPr = (ParaPr)para.ParaPr.FormatRevision.RevPr;
            // Must be 'Istd', 'FormatRevision', 'SpaceAfter' and 'LineSpacing'.
            Assert.That(revParaPr.Count, Is.EqualTo(4));
            ParaPr goldParaPr = new ParaPr() { SpaceAfter = 200, LineSpacing = 276, LineSpacingRule = LineSpacingRule.Multiple };
            TestPr(goldParaPr, revParaPr, gParaSet1);
            Assert.That(revParaPr[ParaAttr.Istd], Is.EqualTo(0));

            ParaPr paraPr = para.ParaPr;
            // Must be 'Istd', 'FormatRevision' and 'RSidP'.
            Assert.That(paraPr.Count, Is.EqualTo(3));

            RunPr breakRunPr = para.ParagraphBreakRunPr;
            Assert.That(breakRunPr.FormatRevision, Is.Null);

            // Must be 'RSidRpr', 'Size', 'SizeBi', Fonts x 4 and Lang x 3.
            Assert.That(breakRunPr.Count, Is.EqualTo(10));
            RunPr goldRunPr = new RunPr { Size = 22, SizeBi = 22 };
            TestPr(goldRunPr, breakRunPr, gRunSet1);
            goldRunPr = new RunPr { NameAscii = "Calibri", NameBi = "Arial", NameFarEast = "ＭＳ 明朝", NameOther = "Calibri" };
            TestPr(goldRunPr, breakRunPr, RunPr.FontNameAttributes);
            Assert.That((Language)breakRunPr[FontAttr.LocaleIdFarEast], Is.EqualTo(Language.JapaneseJapan));
        }

        /// <summary>
        /// Appends documents located in test directory "Model\Nodes\" with the
        /// specified names using import mode KeepSourceFormatting.
        /// If dstDocName is null, then creates a new destination document.
        /// </summary>
        private static Document AppendDocuments(string dstDocName, params string[] srcDocNames)
        {
            return AppendDocuments(dstDocName, null, srcDocNames);
        }

        /// <summary>
        /// Appends two specified documents located in test directory "Model\Nodes\".
        /// If dstDocName is null, then creates a new destination document.
        /// </summary>
        private static Document AppendDocuments(string dstDocName, string srcDocName, ImportFormatMode mode)
        {
            return AppendDocuments(dstDocName, mode, null, srcDocName);
        }

        /// <summary>
        /// Appends documents located in test directory "Model\Nodes\" with the
        /// specified names using import mode KeepSourceFormatting.
        /// If dstDocName is null, then creates a new destination document.
        /// </summary>
        private static Document AppendDocuments(string dstDocName, ImportFormatOptions options, params string[] srcDocNames)
        {
            return AppendDocuments(dstDocName, ImportFormatMode.KeepSourceFormatting, options, srcDocNames);
        }

        /// <summary>
        /// Appends documents located in test directory "Model\Nodes\".
        /// If dstDocName is null, then creates a new destination document.
        /// </summary>
        private static Document AppendDocuments(string dstDocName, ImportFormatMode mode, ImportFormatOptions options,
            params string[] srcDocNames)
        {
            const string testPath = @"Model\Nodes\";

            Document dstDoc = (dstDocName == null) ? new Document() : TestUtil.Open(Path.Combine(testPath, dstDocName));
            dstDoc.RemoveAllChildren();

            if (options == null)
                options = new ImportFormatOptions();

            foreach (string srcDocName in srcDocNames)
            {
                Document srcDoc = TestUtil.Open(Path.Combine(testPath, srcDocName));
                dstDoc.AppendDocument(srcDoc, mode, options);
            }

            return dstDoc;
        }

        /// <summary>
        /// Returns true if all specified nodes have no direct style attribute.
        /// If node is paragraph, then also checks all its runs.
        /// </summary>
        [JavaGenericParameter("T extends Node"), JavaGenericArguments("Iterable<T>")]
        private static void CheckAllStylesExpanded(IEnumerable<Node> nodes)
        {
            CheckDirectStyleExistence(nodes, false);
        }

        /// <summary>
        /// Returns true if all specified nodes have styles in direct attributes.
        /// If node is paragraph, then also checks all its runs.
        /// </summary>
        [JavaGenericParameter("T extends Node"), JavaGenericArguments("Iterable<T>")]
        private static void CheckAllStylesPreserved(IEnumerable<Node> nodes)
        {
            CheckDirectStyleExistence(nodes, true);
        }

        /// <summary>
        /// Returns true if all specified nodes contain direct style attribute.
        /// If node is paragraph, then also checks all its runs.
        /// </summary>
        [JavaGenericParameter("T extends Node"), JavaGenericArguments("Iterable<T>")]
        private static void CheckDirectStyleExistence(IEnumerable<Node> nodes, bool expectedStyleExistance)
        {
            foreach (Node node in nodes)
                switch (node.NodeType)
                {
                    case NodeType.Run:
                    {
                        Assert.That(((Run)node).RunPr.Contains(FontAttr.Istd), Is.EqualTo(expectedStyleExistance));
                        break;
                    }
                    case NodeType.Paragraph:
                    {
                        Paragraph para = (Paragraph)node;
                        Assert.That(para.ParaPr.Contains(ParaAttr.Istd), Is.EqualTo(expectedStyleExistance));

                        CheckDirectStyleExistence(para.Runs, expectedStyleExistance);
                        break;
                    }

                    default:
                        throw new InvalidOperationException((string.Format("Unexpected node type: {0}", node.NodeType)));
                }
        }

        /// <summary>
        /// The helper method for WORDSNET-14587 that checks specific properties of paragraph and its first run.
        /// </summary>
        /// <param name="para">
        /// The paragraph that has Heading1 style and contains run with Strong style in source document.
        /// </param>
        /// <remarks>
        /// When paragraph have such properties in destination document, it means that
        /// Heading1 and Strong styles are expanded into its direct properties.
        /// </remarks>
        private static void CheckHeading1AndStrongExpanded(Paragraph para)
        {
            Assert.That(para.ParaPr.Contains(ParaAttr.Istd), Is.False);
            ParaPr goldParaPr = new ParaPr() { SpaceBefore = 240, SpaceAfter = 0 };
            TestPr(goldParaPr, para.ParaPr, gParaSet1);
            RunPr goldRunPr = new RunPr()
            {
                Size = 32,
                SizeBi = 32,
                Color = DrColor.FromArgb(0xFF, 0x2F, 0x54, 0x96)
            };
            TestPr(goldRunPr, para.ParagraphBreakRunPr, gRunSet1);
            goldRunPr.Bold = AttrBoolEx.True;
            TestPr(goldRunPr, para.FirstRun.RunPr, gRunSet1);

            goldRunPr = new RunPr()
            {
                ThemeAscii = ThemeFontCore.MajorHAnsi,
                ThemeBi = ThemeFontCore.MajorBidi,
                ThemeFarEast = ThemeFontCore.MajorEastAsia,
                ThemeOther = ThemeFontCore.MajorHAnsi
            };
            TestPr(goldRunPr, para.ParagraphBreakRunPr, RunPr.FontNameAttributes);
            TestPr(goldRunPr, para.FirstRun.RunPr, RunPr.FontNameAttributes);
        }

        private static void TestPr(AttrCollection goldPr, AttrCollection testPr, int[] keySet)
        {
            foreach (int key in keySet)
                Assert.That(testPr[key], Is.EqualTo(goldPr[key]));
        }

        private static readonly int[] gRunSet1 = new[]
            {FontAttr.Size, FontAttr.SizeBi, FontAttr.Bold, FontAttr.Italic, FontAttr.Color };

        private static readonly int[] gParaSet1 = new[]
            {ParaAttr.SpaceBefore, ParaAttr.SpaceAfter, ParaAttr.LineSpacing, ParaAttr.LeftIndent};
    }
}
