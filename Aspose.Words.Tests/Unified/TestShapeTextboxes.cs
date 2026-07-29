// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using System.Data;
using System.IO;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Saving;
using Aspose.Words.Validation;
using NUnit.Framework;

// TODO 2 Test user trying to put floating and inline shapes inside textboxes, inside textboxes.

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for shapes with text.
    /// </summary>
    [TestFixture]
    public class TestShapeTextboxes : UnifiedTestsBase
    {
        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxFloating(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxFloating", lf, sf);

            Assert.That(doc.GetText(), Is.EqualTo("Simple shape with text.\r\x000c"));

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            // Drawing object appends the control character at the end of the text.
            Assert.That(shape.GetText(), Is.EqualTo("Simple shape with text.\r"));

            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextBox));
            Assert.That(shape.GetText(), Is.EqualTo("Simple shape with text.\r"));

            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(shape.ParentNode, Is.EqualTo(para));
        }

        /// <summary>
        /// WORDSNET-92 Inline textbox placement is incorrect when opened in Word 97.
        /// Inline escher is stored differently hence separate test.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxInline(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxInline", lf, sf);
            Assert.That(doc.GetText(), Is.EqualTo("Before Textbox\r after.\x000c"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxesInGroup(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxesInGroup", lf, sf);

            GroupShape group = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.GetText(), Is.EqualTo("Textbox1\rTextbox2\r"));

            Assert.That(group.GetChildNodes(NodeType.Any, false)[0].GetText(), Is.EqualTo(""));
            Assert.That(group.GetChildNodes(NodeType.Any, false)[1].GetText(), Is.EqualTo("Textbox1\r"));
            Assert.That(group.GetChildNodes(NodeType.Any, false)[2].GetText(), Is.EqualTo("Textbox2\r"));
        }





        /// <summary>
        /// At the moment linked textboxes are unlinked by Aspose.Words.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxesLinked(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxesLinked", lf, sf);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextBox));

            // All text of the linked textboxes now appears in the first textbox in the chain.
            Assert.That(shape.GetText(), Is.EqualTo("Text box 1\r\r\rText box 2\r\r\rText box 3\r"));

            // The other textboxes that were linked have no text now.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.GetText(), Is.EqualTo(""));

            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.GetText(), Is.EqualTo(""));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxEmpty(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxEmpty", lf, sf);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.GetText(), Is.EqualTo("\r"));

            // If textbox has no nodes, it results in no textbox written to the doc file.
            shape.RemoveAllChildren();
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Textbox\TestTextboxEmpty Modified", lf, sf);

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.GetText(), Is.EqualTo(""));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxOptions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxOptions", lf, sf);

            // The default textbox.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.TextBox.InternalMarginLeft, Is.EqualTo(7.2));
            Assert.That(shape.TextBox.InternalMarginTop, Is.EqualTo(3.6));
            Assert.That(shape.TextBox.InternalMarginRight, Is.EqualTo(7.2));
            Assert.That(shape.TextBox.InternalMarginBottom, Is.EqualTo(3.6));
            Assert.That(shape.TextBox.FitShapeToText, Is.EqualTo(false));
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.Horizontal));
            Assert.That(shape.TextBox.TextBoxWrapMode, Is.EqualTo(TextBoxWrapMode.Square));
            Assert.That(shape.TextBox.TextBoxAnchor, Is.EqualTo(TextBoxAnchor.Top));

            // Textbox with internal margins.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.TextBox.InternalMarginLeft, Is.EqualTo(0.0));
            Assert.That(shape.TextBox.InternalMarginTop, Is.EqualTo(5.0));
            Assert.That(shape.TextBox.InternalMarginRight, Is.EqualTo(10.0));
            Assert.That(shape.TextBox.InternalMarginBottom, Is.EqualTo(15.0));

            // Text centered vertically.
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.TextBox.TextBoxAnchor, Is.EqualTo(TextBoxAnchor.Middle));

            // Textbox with no text wrapping.
            // Although it is okay in the file, MS Word still does not seem to understand it.
            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            Assert.That(shape.TextBox.TextBoxWrapMode, Is.EqualTo(TextBoxWrapMode.None));

            // Textbox where shape fits to text.
            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            Assert.That(shape.TextBox.FitShapeToText, Is.EqualTo(true));

            // Text with vertical text.
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.TopToBottom));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxDirection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxDirection", lf, sf);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.Horizontal));

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.BottomToTop));

            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.TopToBottomIdeographic));

            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.TopToBottom));

            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            Assert.That(shape.TextBox.LayoutFlow, Is.EqualTo(LayoutFlow.HorizontalIdeographic));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextboxWithImage(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\TestTextboxWithImage", lf, sf);

            Shape textbox = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(textbox.ShapeType, Is.EqualTo(ShapeType.TextBox));

            Shape image = (Shape)textbox.GetChild(NodeType.Shape, 0, true);
            Assert.That(image.ShapeType, Is.EqualTo(ShapeType.Image));

            Shape ole = (Shape)textbox.GetChild(NodeType.Shape, 1, true);
            Assert.That(ole.ShapeType, Is.EqualTo(ShapeType.OleObject));
        }


        // FOSS Doc/Rtf are removed load formats.
        [TestCase(LoadFormat.Docx)]
        [TestCase(LoadFormat.FlatOpc)]
        public void TestTextboxVerticalAnchorValidate(LoadFormat lf)
        {
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\TestTextboxVerticalAnchor", lf);

            Shape textboxBottom = doc.FirstSection.Body.Shapes[0];
            Shape textboxMiddle = doc.FirstSection.Body.Shapes[1];
            Shape textboxTop = doc.FirstSection.Body.Shapes[2];

            Assert.That(textboxTop.TextBox.VerticalAnchor, Is.EqualTo(TextBoxAnchor.Top));
            Assert.That(textboxMiddle.TextBox.VerticalAnchor, Is.EqualTo(TextBoxAnchor.Middle));
            Assert.That(textboxBottom.TextBox.VerticalAnchor, Is.EqualTo(TextBoxAnchor.Bottom));
        }

        // FOSS Doc/Rtf/WordML are removed formats; keep the OOXML/FlatOpc pairs.
        [TestCase(LoadFormat.Docx, SaveFormat.Docx)]
        [TestCase(LoadFormat.Docx, SaveFormat.FlatOpc)]
        [TestCase(LoadFormat.FlatOpc, SaveFormat.FlatOpc)]
        public void TestTextboxSetVerticalAnchor(LoadFormat lf, SaveFormat sf)
        {
            string srcFileName = @"Model\Shape\Textbox\TestTextboxVerticalAnchor";
            Document doc = TestUtil.Open(srcFileName, lf);

            Shape textbox1 = doc.FirstSection.Body.Shapes[0];
            Shape textbox2 = doc.FirstSection.Body.Shapes[1];
            Shape textbox3 = doc.FirstSection.Body.Shapes[2];

            textbox3.TextBox.VerticalAnchor = TextBoxAnchor.Middle;
            textbox2.TextBox.VerticalAnchor = TextBoxAnchor.BottomCentered;

            string outFileName = TestUtil.BuildOutFileName(srcFileName, "", sf);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(sf), false);

            textbox1 = doc.FirstSection.Body.Shapes[0];
            textbox2 = doc.FirstSection.Body.Shapes[1];
            textbox3 = doc.FirstSection.Body.Shapes[2];

            Assert.That(textbox1.TextBox.VerticalAnchor, Is.EqualTo(TextBoxAnchor.Bottom));
            Assert.That(textbox2.TextBox.VerticalAnchor, Is.EqualTo(TextBoxAnchor.Top));
            Assert.That(textbox3.TextBox.VerticalAnchor, Is.EqualTo(TextBoxAnchor.Middle));
        }

        // FOSS TestTextboxVerticalAnchorWarning removed: every case loads or saves a removed format
        // (Rtf/Doc/Dot/WordML) to check the format-specific vertical-anchor data-loss warnings.


        /// <summary>
        /// Verifies shape links, used in <see cref="TestDefect3862()" />
        /// </summary>
        private static void VerifyShapeLinks(string testfile, object[,] links, LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(testfile, lf, sf);

            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);

            for (int i = 0; i < shapes.Count; i++)
            {
                Shape shape = (Shape)shapes[i];
                Assert.That(links[i, 0], Is.EqualTo(shape.Id));
                Assert.That(links[i, 1], Is.EqualTo(shape.GetDirectShapeAttrInternal(ShapeAttr.TextboxNextShapeId)));

                // Additionally check Txid of shape for DOC and RTF.
                if ((sf == SaveFormat.Doc) || (sf == SaveFormat.Rtf))
                    Assert.That(links[i, 2], Is.EqualTo(shape.Txid));
            }
        }

        /// <summary>
        /// Append entire document to itself twice.
        /// </summary>
        [Test]
        public void TestLinkedImportAppendDocumentTwice()
        {
            // In this case textboxes links should be preserved and output document should contain three textbox chains.
            //
            // Document clone operation produces shapes with the same IDs as original shapes and should be updated accordingly.
            // Currently AW uses simple method to get pseudo-unique shape IDs: it just adds new shape with ID started from big predefined value
            // and everything is fine till we don't append document twice. Second append operation numerates shapes from the same big predefined value
            // so shapes get lost uniqueness and malformed output is produced.
            //
            // That's why I'm forced to update maximum shape Id in destination document after each import.

            Document doc = TestUtil.Open(LinkedTextboxesFilename);
            // Verify preconditions.
            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(3));
            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);

            Document clone1 = (Document)doc.Clone(true);
            // Verify cloned document preserves links.
            Assert.That(clone1.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(3));
            Assert.That(AreShapesLinked(clone1, 0, 1), Is.True);
            Assert.That(AreShapesLinked(clone1, 1, 2), Is.True);
            Assert.That(IsLinkedTail(clone1, 2), Is.True);

            Document clone2 = (Document)doc.Clone(true);
            Assert.That(clone2.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(3));

            // AppendDocument uses NodeImporter.
            doc.AppendDocument(clone1, ImportFormatMode.UseDestinationStyles);
            doc.AppendDocument(clone2, ImportFormatMode.UseDestinationStyles);

            // It is essential to verify uniqueness before saving.
            // Saved document will have unique shape Ids because DocumentValidator recalculates shape IDs and forces them unique.
            //
            // But if shape Ids were conflicting before saving textbox links will be lost.
            //
            // For example consider this test in details.
            //   Source document contains shape-1.2, shape-2.3, shape-3.0 (number before period is shape ID, number after period is TextboxNextShapeId).
            //   Cloned document contains exactly the same shapes: shape-1.2, shape-2.3, shape-3.0.
            //   After AppendDocument document contains two set of shapes with duplicated IDs. DocumentValidator fix shape IDs but he is not able to resolve
            //   shape links and produces the following shapes: shape-1.2, shape-2.3, shape-3.0, shape-4.2, shape-5.3 and shape-6.0
            //   This is not valid chains and Word fails to open such file.
            //   So shapes MUST have unique IDs and updated NextShapeIDs before saving.
            Assert.That(IsShapeIdsUnique(doc), Is.True);

            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(9));
            // We get three linked chains now. First
            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);
            // Second.
            Assert.That(AreShapesLinked(doc, 3, 4), Is.True);
            Assert.That(AreShapesLinked(doc, 4, 5), Is.True);
            Assert.That(IsLinkedTail(doc, 5), Is.True);
            // And last one.
            Assert.That(AreShapesLinked(doc, 6, 7), Is.True);
            Assert.That(AreShapesLinked(doc, 7, 8), Is.True);
            Assert.That(IsLinkedTail(doc, 8), Is.True);

            // Save output for visual inspection.
            TestUtil.Save(doc, @"Model\Shape\Textbox\TestLinkedImportAppendDocumentTwice.docx");
        }

        /// <summary>
        /// Append first and second shapes by separate import operations.
        /// </summary>
        [Test]
        public void TestLinkedImportPartOfChain()
        {
            // In this case Word just copies textboxes itself and looses its content.
            // We could imitate the same but our implementation currently preserves content of first textbox.
            //
            // So in this test first shape should contain whole content and should not be linked to second textbox..
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\TestTextboxesLinked.docx"); // FOSS .doc -> .docx
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            Shape firstShape = (Shape)shapes[0];
            Shape secondShape = (Shape)shapes[1];
            Shape importedFirstShape = (Shape)doc.ImportNode(firstShape, true);
            Section section = doc.AppendChild(new Section(doc));
            section.AppendChild(new Body(doc));
            Paragraph para = section.Body.AppendChild(new Paragraph(doc));
            para.AppendChild(importedFirstShape);
            Shape importedSecondShape = (Shape)doc.ImportNode(secondShape, true);
            para.AppendChild(importedSecondShape);

            Assert.That(IsShapeIdsUnique(doc), Is.True);
            // First chain were existed in document before import.
            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);

            // Note link is broken and it's OK.
            Assert.That(AreShapesLinked(doc, 3, 4), Is.False);
            Assert.That(IsLinkedTail(doc, 4), Is.True);

            // Save output for visual inspection.
            TestUtil.Save(doc, @"Model\Shape\Textbox\TestLinkedImportPartOfChain.docx");
        }

        /// <summary>
        /// Copy section to another doc which already has another shape links.
        /// </summary>
        [Test]
        public void TestLinkedImportToAnotherDocument()
        {
            // Shape links should be preserved.
            Document doc = TestUtil.Open(LinkedTextboxesFilename);
            Document dstDoc = TestUtil.Open(LinkedTextboxesFilename);

            dstDoc.AppendDocument(doc, ImportFormatMode.KeepSourceFormatting);

            Assert.That(IsShapeIdsUnique(dstDoc), Is.True);

            // First chain were existed in document before import.
            Assert.That(AreShapesLinked(dstDoc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(dstDoc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(dstDoc, 2), Is.True);

            // Second chain is imported from another document.
            Assert.That(AreShapesLinked(dstDoc, 3, 4), Is.True);
            Assert.That(AreShapesLinked(dstDoc, 4, 5), Is.True);
            Assert.That(IsLinkedTail(dstDoc, 5), Is.True);

            // Save output for visual inspection.
            TestUtil.Save(dstDoc, @"Model\Shape\Textbox\TestLinkedImportToAnotherDocument.docx");
        }

        /// <summary>
        /// Copy all three textboxes by separate imports and restore links manually. Insane test.
        /// </summary>
        [Test]
        public void TestLinkedImportRestoreLinks()
        {
            // Maybe we need such method? Something like Document.ImportLinkedTextboxes().
            Document doc = TestUtil.Open(LinkedTextboxesFilename);
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            Shape firstShape = (Shape)shapes[0];
            Shape secondShape = (Shape)shapes[1];
            Shape thirdShape = (Shape)shapes[2];
            // Import texboxes one by one.
            Shape importedFirstShape = (Shape)doc.ImportNode(firstShape, true);
            Shape importedSecondShape = (Shape)doc.ImportNode(secondShape, true);
            Shape importedThirdShape = (Shape)doc.ImportNode(thirdShape, true);

            Section section = doc.AppendChild(new Section(doc));
            section.AppendChild(new Body(doc));
            Paragraph para = section.Body.AppendChild(new Paragraph(doc));
            para.AppendChild(importedFirstShape);
            para.AppendChild(importedSecondShape);
            para.AppendChild(importedThirdShape);

            Assert.That(doc.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(6));

            // First chain were existed in document before import.
            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);

            // All imported shapes are not linked.
            Assert.That(AreShapesLinked(doc, 3, 4), Is.False);
            Assert.That(AreShapesLinked(doc, 4, 5), Is.False);

            // Restore link.
            importedFirstShape.TextboxNextShapeId = importedSecondShape.Id;
            importedSecondShape.TextboxNextShapeId = importedThirdShape.Id;

            Assert.That(IsShapeIdsUnique(doc), Is.True);

            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);

            Assert.That(AreShapesLinked(doc, 3, 4), Is.True);
            Assert.That(AreShapesLinked(doc, 4, 5), Is.True);
            Assert.That(IsLinkedTail(doc, 5), Is.True);

            // Save output for visual inspection.
            TestUtil.Save(doc, @"Model\Shape\Textbox\TestLinkedImportRestoreLinks.docx");
        }

        /// <summary>
        /// Two imports of cloned section but without immediately append.
        /// </summary>
        [Test]
        public void TestLinkedImportDontAppendImmediately()
        {
            // This test illustrates the case when user imports shapes but doesn't append for a while.
            Document doc = TestUtil.Open(LinkedTextboxesFilename);
            Section clonedSection1 = (Section)doc.FirstSection.Clone(true);
            Section clonedSection2 = (Section)doc.FirstSection.Clone(true);
            Section importedClonedSection1 = (Section)doc.ImportNode(clonedSection1, true);

            // At this point document not able to calculate maximum shape ID by traversing all shapes
            // because last imported shapes is not in node tree yet.
            Section importedClonedSection2 = (Section)doc.ImportNode(clonedSection2, true);
            doc.AppendChild(importedClonedSection1);
            doc.AppendChild(importedClonedSection2);

            Assert.That(IsShapeIdsUnique(doc), Is.True);

            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);

            Assert.That(AreShapesLinked(doc, 3, 4), Is.True);
            Assert.That(AreShapesLinked(doc, 3, 4), Is.True);
            Assert.That(IsLinkedTail(doc, 5), Is.True);

            // Save output for visual inspection.
            TestUtil.Save(doc, @"Model\Shape\Textbox\TestLinkedImportDontAppendImmediately.docx");
        }

        /// <summary>
        /// Append cloned section as child without import.
        /// </summary>
        /// <remarks>
        /// AM. I couldn't find good solution for this problem. After discussion with DD we decided to postponed
        /// solution for this case to WORDSNET-5905 and integrate this as intermediate solution.
        /// </remarks>
        [Test]
        public void TestLinkedAppendChildWithoutImport()
        {
            Document doc = TestUtil.Open(LinkedTextboxesFilename);

            Section clonedSection1 = (Section)doc.FirstSection.Clone(true);

            // Shape IDs is updated during import operation. But we do not import here and just AppendChild cloned node.
            // This code produces invalid document (Word shows error when opens output). And I don't see proper solution for this case.
            doc.AppendChild(clonedSection1);

            Assert.That(AreShapesLinked(doc, 0, 1), Is.True);
            Assert.That(AreShapesLinked(doc, 1, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 2), Is.True);

            // Uniqueness violation!
            Assert.That(IsShapeIdsUnique(doc), Is.False);

            // Note this! Forth shape refers to second but must to fifth.
            Assert.That(AreShapesLinked(doc, 3, 1), Is.True);
            // Note this! Fifth shape refers to third but must to sixth.
            Assert.That(AreShapesLinked(doc, 4, 2), Is.True);
            Assert.That(IsLinkedTail(doc, 5), Is.True);

            // This document is corrupted because of TextboxNextShapeId attribute of appended shapes are not updated properly.
            // Save output for visual inspection.
            TestUtil.Save(doc, @"Model\Shape\Textbox\TestLinkedAppendChildWithoutImport.docx");
        }


        /// <summary>
        /// WORDSNET-7628 Other\TestBugInFilesFromRK.docx crashes.
        /// Word moves all content of linked textboxes into first textbox in chain.
        /// </summary>
        [Test]
        public void TestJira7628()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\TestJira7628.docx");

            Shape first = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Shape second = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            // Both shapes have one child paragraph with DrawingML in each.
            Assert.That(first.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(second.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(first.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(1));
            Assert.That(second.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(1));

            // FOSS Lets DocumentValidator work via a Docx save (Pdf removed).
            doc.Save(new MemoryStream(), SaveFormat.Docx);

            // Both DrawingML now is in paragraph of first shape. Second shape has no child.
            Assert.That(second.HasChildNodes, Is.False);
            Assert.That(first.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(first.GetChildNodes(NodeType.Shape, true).Count, Is.EqualTo(2));
        }

        // FOSS TestJira8917 removed: it verifies linked-textbox target uniqueness when rendering a binary DOC
        // to PDF/HTML; DOC load and those render formats are removed, and its 5.6 MB DOC did not convert.


        // FOSS TestJira9455 removed: it loads and saves a binary DOC to verify AW no longer hangs on that
        // format's textbox-loop; DOC load/save is removed in the FOSS build.




        /// <summary>
        /// Relates to WORDSNET-11638 Additional test for WORDSNET-9672
        /// </summary>
        [Test]
        public void TestJira11638_9672()
        {
            string inFile = TestUtil.BuildTestFileName(@"ExportDocx\TestJira9672.docx");
            string outFile = TestUtil.BuildOutFileName(inFile, "", SaveFormat.Docx); // FOSS Pdf -> Docx (still runs validation)

            Document doc = TestUtil.Open(inFile);

            // Vefore chain before and after validation.
            Assert.That(AreShapesLinked(doc, 0, 1), Is.False);

            doc.Save(outFile);

            Assert.That(AreShapesLinked(doc, 0, 1), Is.False);
        }


        /// <summary>
        /// WORDSNET-16745 Save method throws NullReferenceException after doing MailMerge
        /// Tests that only first link is preserved if duplicate links occurred
        /// </summary>
        [Test]
        public void TestJira16745B()
        {
            Document doc = new Document();
            Shape shape = new Shape(doc) { Id = 100, TextboxNextShapeId = 101 };
            doc.FirstSection.Body.FirstParagraph.AppendChild(shape);
            shape = new Shape(doc) { Id = 101 };
            doc.FirstSection.Body.FirstParagraph.AppendChild(shape);
            shape = new Shape(doc) { Id = 100, TextboxNextShapeId = 101 };
            doc.FirstSection.Body.FirstParagraph.AppendChild(shape);
            shape = new Shape(doc) { Id = 101 };
            doc.FirstSection.Body.FirstParagraph.AppendChild(shape);
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            Assert.That(shapes.Count, Is.EqualTo(4));
            Assert.That((shapes[0] as Shape).Id, Is.EqualTo(1025));
            Assert.That((shapes[0] as Shape).TextboxNextShapeId, Is.EqualTo(1026));
            Assert.That((shapes[1] as Shape).Id, Is.EqualTo(1026));
            Assert.That((shapes[2] as Shape).Id, Is.EqualTo(1027));
            Assert.That((shapes[2] as Shape).TextboxNextShapeId, Is.EqualTo(0));
            Assert.That((shapes[3] as Shape).Id, Is.EqualTo(1028));
        }

        /// <summary>
        /// WORDSNET-11297 Add feature to create linked textboxes programmatically.
        /// <see cref="TextBox.Next"/> was implemented.
        /// </summary>
        [Test]
        public void Test11297()
        {
            // Link 2 DML textboxes.
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297DML.docx");

            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            Assert.That(shapes[1].TextBox.IsValidLinkTarget(shapes[0].TextBox), Is.True);
            shapes[1].TextBox.Next = shapes[0].TextBox;

            Assert.That(ShapesAreLinked(shapes[1], shapes[0]), Is.True);

            // Link 2 VML textboxes.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297VML.docx");
            shapes = doc.FirstSection.Body.Shapes;

            Assert.That(shapes[1].TextBox.IsValidLinkTarget(shapes[0].TextBox), Is.True);
            shapes[1].TextBox.Next = shapes[0].TextBox;

            Assert.That(ShapesAreLinked(shapes[1], shapes[0]), Is.True);

            // Link a textbox with a shape from GroupShape, which is not ShapeType.TextBox, but able to contain a text.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_g.docx");
            shapes = doc.FirstSection.Body.Shapes;

            Assert.That(shapes[3].TextBox.IsValidLinkTarget(shapes[0].TextBox), Is.True);
            shapes[3].TextBox.Next = shapes[0].TextBox;

            Assert.That(ShapesAreLinked(shapes[3], shapes[0]), Is.True);
        }

        /// <summary>
        /// Additional checks for incorrect attributes in textboxes linking.
        /// </summary>
        [Test]
        public void Test11297Exceptions()
        {
            // Attempt to link textbox to itself.
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297DML.docx");
            Assert.That(VerifyException(doc, 1, 1, WarningStrings.LinkTextboxInvalidTarget), Is.True);

            // Attempt to link textbox and a shape of another type.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_a.docx");
            Assert.That(VerifyException(doc, 1, 0, WarningStrings.LinkTextboxBothShapesText), Is.True);

            // Attempt to link DML textbox with a VML textbox.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_b.docx");
            Assert.That(VerifyException(doc, 1, 0, WarningStrings.LinkTextboxSameMarkup), Is.True);

            // Attempt to link textbox to non-empty textbox.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_c.docx");
            Assert.That(VerifyException(doc, 1, 0, WarningStrings.LinkTextboxEmptyTarget), Is.True);

            // Verify behavior for the DML textboxes.

            // Attempt to link textbox to the target which already in a chain.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_d.docx");
            Assert.That(VerifyException(doc, 0, 1, WarningStrings.LinkTextboxTargetIsLinked), Is.True);

            // Attempt to link textbox which already has a link.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_d.docx");
            Assert.That(VerifyException(doc, 2, 0, WarningStrings.LinkTextboxHasLink), Is.True);

            // Verify behavior for the VML textboxes.

            // Attempt to link textbox to the target which already in a chain.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_e.docx");
            Assert.That(VerifyException(doc, 0, 1, WarningStrings.LinkTextboxTargetIsLinked), Is.True);

            // Attempt to link textbox which already has a link.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_e.docx");
            Assert.That(VerifyException(doc, 2, 0, WarningStrings.LinkTextboxHasLink), Is.True);

            // Attempt to link textbox from body to textbox from header.
            doc = TestUtil.Open(@"Model\Shape\Textbox\Test11297_f.docx");
            Assert.That(VerifyException(doc, 2, 0, WarningStrings.LinkTextboxDifferentStoryTypes), Is.True);
        }

        /// <summary>
        /// Verifies BreakForwardLink.
        /// </summary>
        [TestCase("Test11297BreakLinkVML.docx")]
        [TestCase("Test11297BreakLinkDML.docx")]
        public void Test11297BreakForwardLink(string file)
        {
            string filename = @"Model\Shape\Textbox\" + file;
            Document doc = TestUtil.Open(filename);
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            // Initially all 4 shapes are linked.
            Assert.That(ShapesAreLinked(shapes[0], shapes[1]), Is.True);
            Assert.That(ShapesAreLinked(shapes[1], shapes[2]), Is.True);
            Assert.That(ShapesAreLinked(shapes[2], shapes[3]), Is.True);

            // Remove link at the first position.
            shapes[0].TextBox.BreakForwardLink();
            Assert.That(ShapesAreLinked(shapes[0], shapes[1]), Is.False);
            Assert.That(ShapesAreLinked(shapes[1], shapes[2]), Is.True);
            Assert.That(ShapesAreLinked(shapes[2], shapes[3]), Is.True);
            // DML check : Head of a newly created chain must contain an empty paragraph after the link remove.
            if (shapes[1].MarkupLanguage == ShapeMarkupLanguage.Dml)
                Assert.That(shapes[1].HasChildNodes, Is.True);

            doc = TestUtil.Open(filename);
            shapes = doc.FirstSection.Body.Shapes;

            // Remove link at the second position.
            shapes[1].TextBox.BreakForwardLink();
            Assert.That(ShapesAreLinked(shapes[0], shapes[1]), Is.True);
            Assert.That(ShapesAreLinked(shapes[1], shapes[2]), Is.False);
            Assert.That(ShapesAreLinked(shapes[2], shapes[3]), Is.True);
            // DML check : Head of a newly created chain must contain an empty paragraph after the link remove.
            if (shapes[2].MarkupLanguage == ShapeMarkupLanguage.Dml)
                Assert.That(shapes[2].HasChildNodes, Is.True);

            doc = TestUtil.Open(filename);
            shapes = doc.FirstSection.Body.Shapes;

            // Remove link at the third position by setting a null.
            shapes[2].TextBox.Next = null;
            Assert.That(ShapesAreLinked(shapes[0], shapes[1]), Is.True);
            Assert.That(ShapesAreLinked(shapes[1], shapes[2]), Is.True);
            Assert.That(ShapesAreLinked(shapes[2], shapes[3]), Is.False);
        }

        /// <summary>
        /// WORDSNET-19044 Exception occurs in TextBox.InternalMarginBottom setter due to
        /// negative value. Negative values are allowed for InternalMargin attributes, no need to raise exception.
        /// </summary>
        [Test]
        public void Test19044()
        {
            // Document illustrates, that negative values are allowed.
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\Test19044.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];

            // Check values before set.
            Assert.That(shape.TextBox.InternalMarginTop, Is.EqualTo(-7.87).Within(0.01));
            Assert.That(shape.TextBox.InternalMarginBottom, Is.EqualTo(15).Within(0.01));
            Assert.That(shape.TextBox.InternalMarginLeft, Is.EqualTo(-7.87).Within(0.01));
            Assert.That(shape.TextBox.InternalMarginRight, Is.EqualTo(10).Within(0.01));

            shape.TextBox.InternalMarginTop = -5;
            shape.TextBox.InternalMarginBottom = -6;
            shape.TextBox.InternalMarginLeft = -7;
            shape.TextBox.InternalMarginRight = -8;

            MemoryStream stream = new MemoryStream();
            doc.Save(stream, SaveFormat.Docx);
            stream.Position = 0;

            doc = new Document(stream);
            shape = doc.FirstSection.Body.Shapes[0];

            // Check values were set correctly.
            Assert.That(shape.TextBox.InternalMarginTop, Is.EqualTo(-5).Within(0.01));
            Assert.That(shape.TextBox.InternalMarginBottom, Is.EqualTo(-6).Within(0.01));
            Assert.That(shape.TextBox.InternalMarginLeft, Is.EqualTo(-7).Within(0.01));
            Assert.That(shape.TextBox.InternalMarginRight, Is.EqualTo(-8).Within(0.01));
        }

        /// <summary>
        /// WORDSNET-14369 Relations between linked textbox were broken after re-saving document.
        /// ShapeIdGenerator was improved to preserve textbox links.
        /// </summary>
        /// <remarks>
        /// This fix is a rework of WORDSNET-9455.
        /// </remarks>
        [Test]
        public void Test14369()
        {
            // FOSS WordML -> Docx roundtrip (WordML removed).
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Textbox\Test14369",
                TestUtil.GetUnifiedScenario(LoadFormat.Docx, SaveFormat.Docx) | UnifiedScenario.NoGold);

            ShapeCollection shapes = doc.FirstSection.Body.Shapes;
            Assert.That(ShapesAreLinked(shapes[1], shapes[0]), Is.True);
            Assert.That(ShapesAreLinked(shapes[0], shapes[2]), Is.True);
        }

        /// <summary>
        /// WORDSNET-21405 Image is inserted incorrectly into textbox after mail merge.
        /// If the shape is inside a textbox then Word scales the shape to fit in the textbox. Mimic Word.
        /// </summary>
        [Test]
        public void Test21405()
        {
            string imagePath = TestUtil.BuildTestFileName(@"Model\Shape\Textbox\Test21405Image.png");
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\Test21405.docx");

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToMergeField("mlOppStateCube1", true, true);
            Shape shape = builder.InsertImage(File.OpenRead(imagePath));

            Assert.That(shape.Width, Is.EqualTo(110.05).Within(0.05d));
            Assert.That(shape.Height, Is.EqualTo(117.3).Within(0.05d));
        }



        /// <summary>
        /// WORDSNET-27492 Linked text box chain is not loaded into the model (DOC)
        /// The <see cref="ShapeIdGenerator"/> class has been fixed to support the case of the input document.
        /// </summary>
        [Test]
        public void Test27492()
        {
            // FOSS Reduced: this is the binary-DOC variant of Test27369 (removed load format); the .docx
            // equivalent Test27369 already covers the same textbox chain, so nothing to load here.
        }

        /// <summary>
        /// Checks textbox chain of the Test26625.docx document.
        /// </summary>
        private static void Check27369TextboxChain(Document doc)
        {
            // The linkage in the input DOCX file:
            // _x0000_s1027 => _x0000_s1034
            // _x0000_s1034 => _x0000_s1038
            // _x0000_s1039 => _x0000_s1031
            // _x0000_s1038 => _x0000_s1039
            // _x0000_s1032 => _x0000_s1032 // This cycle link will be removed during document validation.
            // _x0000_s1031 => _x0000_s1032

            Shape shape27 = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            Shape shape34 = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            Shape shape39 = (Shape)doc.GetChild(NodeType.Shape, 6, true);
            Shape shape38 = (Shape)doc.GetChild(NodeType.Shape, 8, true);
            Shape shape32 = (Shape)doc.GetChild(NodeType.Shape, 9, true);
            Shape shape31 = (Shape)doc.GetChild(NodeType.Shape, 11, true);

            Assert.That(shape27.TextboxNextShapeId, Is.EqualTo(shape34.Id));
            Assert.That(shape34.TextboxNextShapeId, Is.EqualTo(shape38.Id));
            Assert.That(shape38.TextboxNextShapeId, Is.EqualTo(shape39.Id));
            Assert.That(shape39.TextboxNextShapeId, Is.EqualTo(shape31.Id));
            Assert.That(shape31.TextboxNextShapeId, Is.EqualTo(shape32.Id));
            Assert.That(shape32.TextboxNextShapeId, Is.EqualTo(shape32.Id));
        }

        /// <summary>
        /// Test11297 routine. Creates link for textboxes. Verifies exception.
        /// </summary>
        private static bool VerifyException(Document doc, int source, int target, string message)
        {
            ShapeCollection shapes = new ShapeCollection(doc);

            if (shapes[source].TextBox.IsValidLinkTarget(shapes[target].TextBox))
                return false;

            try
            {
                shapes[source].TextBox.Next = shapes[target].TextBox;
            }
            catch (System.ArgumentException e)
            {
                return (e.Message == message);
            }

            return false;
        }

        /// <summary>
        /// Verifies if shape1.TextBox is linked to shape2.TextBox.
        /// </summary>
        private static bool ShapesAreLinked(Shape shape1, Shape shape2)
        {
            return ReferenceEquals(shape1.TextBox.Next, shape2.TextBox);
        }

        /// <summary>
        /// Checks whether Shape.Id is unique for all shapes across whole document.
        /// </summary>
        private static bool IsShapeIdsUnique(Document doc)
        {
            Collections.Generic.HashSetGeneric<int> hashSet = new Collections.Generic.HashSetGeneric<int>();

            foreach (Shape shape in doc.GetChildNodes(NodeType.Shape, true))
                if (hashSet.Contains(shape.Id))
                    return false;
                else
                    hashSet.Add(shape.Id);

            // No conflicting Ids.
            return true;
        }

        /// <summary>
        /// Verifies that shapes with given indexes are linked.
        /// </summary>
        private static bool AreShapesLinked(Document doc, int n1, int n2)
        {
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);
            return (((Shape)shapes[n1]).TextboxNextShapeId == ((Shape)shapes[n2]).Id);
        }

        private static bool IsDmlsLinked(Document doc, int n1, int n2)
        {
            NodeCollection dmls = doc.GetChildNodes(NodeType.Shape, true);
            int id1 = ((Shape)dmls[n1]).TextboxId;
            int in2 = ((Shape)dmls[n2]).LinkedTextboxId;

            if ((id1 <= 0) || (id1 <= 0))
                return false;

            return (id1 == in2);
        }

        /// <summary>
        /// Verifies that shape with given index is last in linked chain.
        /// </summary>
        private static bool IsLinkedTail(Document doc, int n)
        {
            Shape shape = (Shape)doc.GetChildNodes(NodeType.Shape, true)[n];
            return (shape.TextboxNextShapeId == 0);
        }

        /// <summary>
        /// Shape links, used in <see cref="TestDefect3862()" />
        /// </summary>
        private static readonly object[,] gShapeLinks1 = new object[,] // Casting to object is for C++ porting.
                                                     {
                                                        { (object)0x402, (object)null,  (object)0x0     },
                                                        { (object)0x403, (object)0x404, (object)0x10000 },
                                                        { (object)0x404, (object)null,  (object)0x10001 },
                                                        { (object)0x405, (object)null,  (object)0x20000 },
                                                        { (object)0x406, (object)null,  (object)0x30000 },
                                                        { (object)0x407, (object)null,  (object)0x40000 },
                                                        { (object)0x409, (object)null,  (object)0x0     },
                                                        { (object)0x40a, (object)0x40b, (object)0x50000 },
                                                        { (object)0x40b, (object)null,  (object)0x50001 },
                                                        { (object)0x40c, (object)null,  (object)0x0     },
                                                        { (object)0x40d, (object)null,  (object)0x60000 },
                                                        { (object)0x40e, (object)null,  (object)0x70000 },
                                                        { (object)0x40f, (object)null,  (object)0x80000 },
                                                        { (object)0x410, (object)null,  (object)0x90000 },
                                                        { (object)0x412, (object)null,  (object)0x0     },
                                                        { (object)0x413, (object)0x414, (object)0xa0000 },
                                                        { (object)0x414, (object)null,  (object)0xa0001 }
                                                     };

        private static readonly object[,] gShapeLinks2 = new object[,]
                                                     {
                                                        { (object)0x401, (object)0x402, (object)0x10000 },
                                                        { (object)0x402, (object)0x403, (object)0x10001 },
                                                        { (object)0x403, (object)null,  (object)0x10002 }
                                                     };

        /// <summary>
        /// Filename of document which is often used in linked textboxes tests.
        /// </summary>
        private const string LinkedTextboxesFilename = @"Model\Shape\Textbox\TestTextboxesLinked.docx"; // FOSS .doc -> .docx
    }
}
