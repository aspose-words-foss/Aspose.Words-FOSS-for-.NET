// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/09/2012 by Alexey Butalov
using Aspose.TestFx;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Unit tests for the <see cref="RangeDocumentBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class TestRangeDocumentBuilder
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestDocumentNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(@"Document\TestSimple.docx"));
            Document newDoc = RangeDocumentBuilder.Build(doc.Range);

            Assert.That(newDoc.Sections.Count, Is.EqualTo(doc.Sections.Count));
            Assert.That(newDoc.FirstSection.Body.Count, Is.EqualTo(doc.FirstSection.Body.Count));
        }

        [Test]
        public void TestSectionNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(@"Section\TestSections.docx")));
            Node testNode = GetTestNode(doc, NodeType.Section, 1);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(GetNodeText(newDoc.FirstSection), Is.EqualTo(GetNodeText(testNode)));
        }

        [Test]
        public void TestBodyNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(@"Stories\TestBody.docx")));
            Node testNode = GetTestNode(doc, NodeType.Body, 0);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(newDoc.FirstSection.Body, IsNot.Null());
            Assert.That(GetNodeText(newDoc.FirstSection.Body), Is.EqualTo(GetNodeText(testNode)));
        }

        [Test]
        public void TestFootnoteNodeToDocument()
        {
            VerifyInlineNodeToDocument(@"Stories\TestBodyFootnoteEndnote.docx", NodeType.Footnote, 0);
        }

        [Test]
        public void TestHeaderNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(@"Stories\TestBodyHeaderFooter.docx")));
            Node testNode = GetTestNode(doc, NodeType.HeaderFooter, 0);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(newDoc.FirstSection.HeadersFooters.Count, Is.EqualTo(1));
            Assert.That(GetNodeText(newDoc.FirstSection.HeadersFooters[0]), Is.EqualTo(GetNodeText(testNode)));
        }

        [Test]
        public void TestFooterNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(@"Stories\TestBodyHeaderFooter.docx")));
            Node testNode = GetTestNode(doc, NodeType.HeaderFooter, 1);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(newDoc.FirstSection.HeadersFooters.Count, Is.EqualTo(1));
            Assert.That(GetNodeText(newDoc.FirstSection.HeadersFooters[0]), Is.EqualTo(GetNodeText(testNode)));
        }

        [Test]
        public void TestParagraphNodeToDocument()
        {
            VerifySectionChildNodeToDocument(@"Document\TestSimple.docx", NodeType.Paragraph, 2);
        }

        [Test]
        public void TestRunNodeToDocument()
        {
            VerifyInlineNodeToDocument(@"Document\TestSimple.docx", NodeType.Run, 7);
        }

        [Test]
        public void TestBookmarkNodeToDocument()
        {
            VerifyInlineNodeToDocument(@"Bookmark\TestBookmarks.docx", NodeType.BookmarkStart, 0);
        }


        [Test]
        public void TestImageNodeToDocument()
        {
            VerifyInlineNodeToDocument(@"Shape\Image\TestImageInline.docx", NodeType.Shape, 0);
        }

        [Test]
        public void TestTableNodeToDocument()
        {
            VerifySectionChildNodeToDocument(@"Table\TestSimple.docx", NodeType.Table, 0);
        }

        [Test]
        public void TestRowNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(@"Table\TestSimple.docx")));
            Node testNode = GetTestNode(doc, NodeType.Row, 1);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.FirstSection.Body.FirstChild, Is.InstanceOf(typeof(Table)));
            Table table = (Table) newDoc.FirstSection.Body.FirstChild;
            Assert.That(table.Rows.Count, Is.EqualTo(1));
            Assert.That(GetNodeText(table.FirstRow), Is.EqualTo(GetNodeText(testNode)));
        }

        [Test]
        public void TestCellNodeToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(@"Table\TestSimple.docx")));
            Node testNode = GetTestNode(doc, NodeType.Cell, 1);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.FirstSection.Body.FirstChild, Is.InstanceOf(typeof(Table)));
            Table table = (Table)newDoc.FirstSection.Body.FirstChild;
            Assert.That(table.Rows.Count, Is.EqualTo(1));
            Assert.That(table.Rows[0].Cells.Count, Is.EqualTo(1));
            Assert.That(GetNodeText(table.Rows[0].Cells[0]), Is.EqualTo(GetNodeText(testNode)));
        }

        [Test]
        public void TestNestedTableNodeToDocument()
        {
            VerifySectionChildNodeToDocument(@"Table\TestNested.docx", NodeType.Table, 1);
        }

        [Test]
        public void TestGlossaryDocumentToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(@"BuildingBlocks\TestBuildingBlocks.dotx"));
            Document newDoc = RangeDocumentBuilder.Build(doc.GlossaryDocument.Range);

            Assert.That(newDoc.GlossaryDocument, IsNot.Null());
        }

        // FOSS: TestCustomXmlMarkupToDocument removed — its input is a .doc (Doc reader removed)
        // with no .docx sibling, and it verifies the legacy custom-XML-markup import warning,
        // which a Word .doc->.docx conversion would strip.

        [Test]
        public void TestStdToDocument()
        {
            Node testNode = GetTestNode(@"Markup\TestSdtPlaceholders.docx", "0.0.0.0");
            Debug.Assert(testNode.NodeType == NodeType.StructuredDocumentTag);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Node node1 = newDoc.FirstSection.Body.FirstChild;
            Assert.That(node1.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));
            Assert.That(((CompositeNode) node1).FirstChild.NodeType, Is.EqualTo(NodeType.Paragraph));
        }

        [Test]
        public void TestBuildingBlockToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(@"BuildingBlocks\TestBuildingBlocks.dotx"));
            Document newDoc = RangeDocumentBuilder.Build(doc.GlossaryDocument.FirstBuildingBlock.Range);

            Assert.That(newDoc.GlossaryDocument, IsNot.Null());
            Assert.That(newDoc.GlossaryDocument.BuildingBlocks.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestGlossaryDocumentRunToDocument()
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(@"BuildingBlocks\TestBuildingBlocks.dotx"));
            Node node = doc.GlossaryDocument.GetChild(NodeType.Run, 0, true);
            Debug.Assert(node != null);
            Document newDoc = RangeDocumentBuilder.Build(node.Range);

            Assert.That(newDoc.GlossaryDocument, IsNot.Null());
            Assert.That(newDoc.GlossaryDocument.BuildingBlocks.Count, Is.EqualTo(1));
            Assert.That(newDoc.GlossaryDocument.FirstBuildingBlock.FirstSection, IsNot.Null());

            Node runNode = newDoc.GlossaryDocument.GetChild(NodeType.Run, 0, true);
            Assert.That(runNode, IsNot.Null());
            Assert.That(runNode.GetText(), Is.EqualTo(node.GetText()));
        }

        /// <summary>
        /// Gets a test node of a source document.
        /// </summary>
        /// <param name="doc">Original document.</param>
        /// <param name="nodeType">Specifies type of a node to select.</param>
        /// <param name="index">Zero based index of a node to select.
        /// Negative indexes are also allowed and indicate access from the end,
        /// that is -1 means the last node.</param>
        private static Node GetTestNode(Document doc, NodeType nodeType, int index)
        {
            Node node = doc.GetChild(nodeType, index, true);
            Debug.Assert(node != null);
            return node;
        }

        /// <summary>
        /// Gets a test node of a source document.
        /// </summary>
        /// <param name="srcDocFileName">Name of an original file, relative to the TestData\Model folder.</param>
        /// <param name="id">Node string id.</param>
        private static Node GetTestNode(string srcDocFileName, string id)
        {
            srcDocFileName = Aspose.TestFx.TestFxUtil.CorrectPath(srcDocFileName);
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(srcDocFileName));
            Node node = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(doc.GetNodeById(id), doc);
            Debug.Assert(node != null);
            return node;
        }

        /// <summary>
        /// Tests a document created from a section child node.
        /// </summary>
        /// <param name="srcDocFileName">Name of a original file, relative to the TestData\Model folder.</param>
        /// <param name="nodeType">Specifies type of a node to test.</param>
        /// <param name="index">Zero based index of a node to test.
        /// Negative indexes are also allowed and indicate access from the end,
        /// that is -1 means the last node.</param>
        private static void VerifySectionChildNodeToDocument(string srcDocFileName, NodeType nodeType, int index)
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(srcDocFileName)));
            Node testNode = GetTestNode(doc, nodeType, index);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);

            Assert.That(newDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(newDoc.FirstSection.Body.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(newDoc.FirstSection.Body.FirstChild, Is.InstanceOf(testNode.GetType()));

            string srcText = GetNodeText(testNode);
            string destText = GetNodeText(newDoc.FirstSection.Body.FirstChild);
            Assert.That(destText, Is.EqualTo(srcText));
        }

        /// <summary>
        /// Tests a document created from a inline-level node.
        /// </summary>
        /// <param name="srcDocFileName">Name of a original file, relative to the TestData\Model folder.</param>
        /// <param name="nodeType">Specifies type of a node to test.</param>
        /// <param name="index">Zero based index of a node to test.
        /// Negative indexes are also allowed and indicate access from the end,
        /// that is -1 means the last node.</param>
        private static void VerifyInlineNodeToDocument(string srcDocFileName, NodeType nodeType, int index)
        {
            Document doc = TestUtil.Open(TestUtil.GetInModelPath(TestFxUtil.CorrectPath(srcDocFileName)));
            Node testNode = GetTestNode(doc, nodeType, index);
            Document newDoc = RangeDocumentBuilder.Build(testNode.Range);
            Assert.That(newDoc.Sections.Count, Is.EqualTo(1));

            Body newBody = newDoc.FirstSection.Body;
            Assert.That(newBody, IsNot.Null());
            Assert.That(newBody.FirstChild, Is.InstanceOf(typeof(Paragraph)));
            Assert.That(GetNodeText(((Paragraph) newBody.FirstChild).FirstChild), Is.EqualTo(GetNodeText(testNode)));
        }

        /// <summary>
        /// Gets text of a node.
        /// </summary>
        private static string GetNodeText(Node node)
        {
            char[] breakChars = new char[] { '\f', '\r' };

            string text = node.GetText();
            if (node is Paragraph)
                text = text.TrimEnd(breakChars); // Remove end of paragraph characters.
            return text;
        }
    }
}
