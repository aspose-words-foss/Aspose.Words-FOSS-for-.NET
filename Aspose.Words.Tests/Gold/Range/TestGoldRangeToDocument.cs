// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2012 by Alexey Butalov
using Aspose.Words.Loading;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Gold.Range
{
    /// <summary>
    /// Gold tests of public ToDocument() method of the Range class.
    /// It tests ranges attached to a single node only.
    /// </summary>
    [TestFixture]
    public class TestGoldRangeToDocument : TestGoldBase
    {
        [Test]
        public void TestDocumentToDocument()
        {
            const string docFileName = @"Document\TestSimple.docx";
            Document doc = Open(TestUtil.GetInModelPath(docFileName));

            VerifyNodeRangeToDocument(docFileName, "Document", doc);
        }

        [Test]
        public void TestParagraphToDocument()
        {
            VerifyNodeRangeToDocument(@"Document\TestSimple.docx", "Paragraph", NodeType.Paragraph, 2);
        }

        [Test]
        public void TestRunToDocument()
        {
            VerifyNodeRangeToDocument(@"Document\TestSimple.docx", "Run", NodeType.Run, 7);
        }

        [Test]
        public void TestRunWithStyleToDocument()
        {
            VerifyNodeRangeToDocument(@"Document\TestSimple.docx", "RunWithStyle", NodeType.Run);
        }

        [Test]
        public void TestBookmarkToDocument()
        {
            VerifyNodeRangeToDocument(@"Bookmark\TestBookmarks.docx", "Bookmark", NodeType.BookmarkStart);
        }


        [Test]
        public void TestTableToDocument()
        {
            VerifyNodeRangeToDocument(@"Table\TestSimple.docx", "Table", NodeType.Table);
        }

        [Test]
        public void TestRowToDocument()
        {
            VerifyNodeRangeToDocument(@"Table\TestSimple.docx", "Row", NodeType.Row, 1);
        }

        [Test]
        public void TestCellToDocument()
        {
            VerifyNodeRangeToDocument(@"Table\TestSimple.docx", "Cell", NodeType.Cell);
        }

        [Test]
        public void TestSectionToDocument()
        {
            VerifyNodeRangeToDocument(@"Section\TestSections.docx", "Section", NodeType.Section, 1);
        }

        [Test]
        public void TestBodyToDocument()
        {
            VerifyNodeRangeToDocument(@"Stories\TestBody.docx", "Body", NodeType.Body);
        }

        [Test]
        public void TestFootnoteToDocument()
        {
            VerifyNodeRangeToDocument(@"Stories\TestBodyFootnoteEndnote.docx", "Footnote", NodeType.Footnote);
        }

        [Test]
        public void TestHeaderToDocument()
        {
            VerifyNodeRangeToDocument(@"Stories\TestBodyHeaderFooter.docx", "Header", NodeType.HeaderFooter);
        }

        [Test]
        public void TestFooterToDocument()
        {
            VerifyNodeRangeToDocument(@"Stories\TestBodyHeaderFooter.docx", "Footer", NodeType.HeaderFooter, 1);
        }

        [Test]
        public void TestImageToDocument()
        {
            VerifyNodeRangeToDocument(@"Shape\Image\TestImageInline.docx", "Shape", NodeType.Shape);
        }

        [Test]
        public void TestFieldStartToDocument()
        {
            VerifyNodeRangeToDocument(@"Field\TestFields.docx", "FieldStart", NodeType.FieldStart);
        }

        [Test]
        public void TestFieldEndToDocument()
        {
            VerifyNodeRangeToDocument(@"Field\TestFields.docx", "FieldEnd", NodeType.FieldEnd);
        }

        [Test]
        public void TestFormFieldTextInputToDocument()
        {
            VerifyNodeRangeToDocument(@"Field\TestFields.docx", "FormFieldTextInput", NodeType.FormField);
        }

        [Test]
        public void TestFormFieldCheckBoxToDocument()
        {
            VerifyNodeRangeToDocument(@"Field\TestFields.docx", "FormFieldCheckBox", NodeType.FormField, 1);
        }

        [Test]
        public void TestFormFieldDropDownToDocument()
        {
            VerifyNodeRangeToDocument(@"Field\TestFields.docx", "FormFieldDropDown", NodeType.FormField, 2);
        }


        [Test]
        public void TestSdtParagraphToDocument()
        {
            VerifyNodeRangeToDocument(@"Markup\TestSdtPlaceholders.docx", "SDTPara", "0.0.0.0.0");
        }

        [Test]
        public void TestSdtShapeToDocument()
        {
            VerifyNodeRangeToDocument(@"Markup\TestSdtPlaceholders.docx", "SDTShape", NodeType.StructuredDocumentTag, 3);
        }

        [Test]
        public void TestDrawingMLToDocument()
        {
            VerifyNodeRangeToDocument(@"DrawingML\TestImageData.docx", "DrawingMLImage", NodeType.Shape);
        }


        [Test]
        public void TestBuildingBlockToDocument()
        {
            const string docFileName = @"BuildingBlocks\TestBuildingBlocks.dotx";
            Document doc = Open(docFileName);
            Debug.Assert(doc.GlossaryDocument != null);
            Debug.Assert(doc.GlossaryDocument.FirstBuildingBlock != null);
            VerifyNodeRangeToDocument(docFileName, "BuildingBlock", doc.GlossaryDocument.FirstBuildingBlock);
        }

        [Test]
        public void TestGlossaryDocumentRunToDocument()
        {
            const string docFileName = @"BuildingBlocks\TestBuildingBlocks.dotx";
            Document doc = Open(docFileName);
            Node node = doc.GlossaryDocument.GetChild(NodeType.Run, 0, true);
            Debug.Assert(node != null);
            VerifyNodeRangeToDocument(docFileName, "GlossaryDocumentRun", node);
        }

        [Test]
        public void TestGlossaryDocumentToDocument()
        {
            const string docFileName = @"BuildingBlocks\TestBuildingBlocks.dotx";
            Document doc = Open(docFileName);
            VerifyNodeRangeToDocument(docFileName, "GlossaryDocument", doc.GlossaryDocument);
        }

        /// <summary>
        /// This is a standard test for (<see cref="Range.ToDocument()"></see>) with compare against a gold file.
        /// It tests ranges attached to a single node.
        /// It creates a document from a file, finds node of a specified type, builds a new document from this node,
        /// saves this document into the Docx format and verifies against a gold file.
        /// </summary>
        /// <param name="docFileName">Name of a original file, relative to the TestData\Model folder.</param>
        /// <param name="suffix">File name suffix for a output file.</param>
        /// <param name="nodeType">Specifies type of a node to test a range based on it.</param>
        /// <param name="index">Zero based index of a node to select.
        /// Negative indexes are also allowed and indicate access from the end,
        /// that is -1 means the last node.</param>
        private static void VerifyNodeRangeToDocument(string docFileName,
            string suffix,
            NodeType nodeType,
            int index)
        {
            Document doc = Open(TestUtil.GetInModelPath(docFileName));
            Node node = doc.GetChild(nodeType, index, true);
            Debug.Assert(node != null);
            VerifyNodeRangeToDocument(docFileName, suffix, node);
        }

        /// <summary>
        /// This is a standard test for (<see cref="Range.ToDocument()"></see>) with compare against a gold file.
        /// It tests ranges attached to a single node.
        /// It creates a document from a file, finds node of a specified type, builds a new document from this node,
        /// saves this document into the Docx format and verifies against a gold file.
        /// </summary>
        /// <param name="docFileName">Name of a original file, relative to the TestData\Model folder.</param>
        /// <param name="suffix">File name suffix for a output file.</param>
        /// <param name="nodeType">Specifies type of a node to test a range based on it.</param>
        private static void VerifyNodeRangeToDocument(string docFileName,
            string suffix,
            NodeType nodeType)
        {
            LoadOptions lo = new LoadOptions();
            if (nodeType == NodeType.OfficeMath)
                lo.ConvertShapeToOfficeMath = true;

            Document doc = Open(TestUtil.GetInModelPath(docFileName), lo);
            Node node = doc.GetChild(nodeType, 0, true);
            Debug.Assert(node != null);
            VerifyNodeRangeToDocument(docFileName, suffix, node);
        }

        /// <summary>
        /// This is a standard test for (<see cref="Range.ToDocument()"></see>) with compare against a gold file.
        /// It tests ranges attached to a single node.
        /// It creates a document from a file, finds node of a specified type, builds a new document from this node,
        /// saves this document into the Docx format and verifies against a gold file.
        /// </summary>
        /// <param name="docFileName">Name of a original file, relative to the TestData\Model folder.</param>
        /// <param name="suffix">File name suffix for a output file.</param>
        /// <param name="id">Node string id</param>
        private static void VerifyNodeRangeToDocument(string docFileName, string suffix, string id)
        {
            Document doc = Open(TestUtil.GetInModelPath(docFileName));
            Node node = doc.GetNodeById(id);
            Debug.Assert(node != null);
            VerifyNodeRangeToDocument(docFileName, suffix, node);
        }

        /// <summary>
        /// This is a standard test for (<see cref="Range.ToDocument()"></see>) with compare against a gold file.
        /// It tests ranges attached to a single node.
        /// It builds a document from a node, saves this document into the Docx format and verifies against a gold file.
        /// </summary>
        /// <param name="docFileName">Name of a original file, relative to the TestData\Model folder.</param>
        /// <param name="suffix">File name suffix for a output file.</param>
        /// <param name="node">Node to test.</param>
        private static void VerifyNodeRangeToDocument(string docFileName, string suffix, Node node)
        {
            docFileName = TestUtil.GetInModelPath(docFileName);
            suffix = " NodeToDoc " + suffix;
            string outFileName = TestUtil.BuildOutFileName(docFileName, suffix, SaveFormat.Docx);
            TestUtil.EnsureDirectoryForFileExists(outFileName);

            Document doc = node.Range.ToDocument();
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.SetTestMode();
            doc.Save(outFileName, saveOptions);

            string goldFileName = TestUtil.BuildGoldFileName(docFileName, suffix, SaveFormat.Docx);
            TestUtil.VerifyGold(docFileName, outFileName, goldFileName, "", saveOptions, false);
        }
    }
}
