// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Basic node functionality tests.
    /// </summary>
    [TestFixture]
    public class TestNode
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestDocumentCtor()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);

            //General features
            Assert.That(doc.NodeType, Is.EqualTo(NodeType.Document));
            Assert.That(doc.IsComposite, Is.EqualTo(true));
            Assert.That(doc.ParentNode, Is.EqualTo(null));
            Assert.That(doc.Document, Is.EqualTo(doc));
            Assert.That(doc.NullNode, IsNot.Null());

            //Sibling features
            Assert.That(doc.PreviousSibling, Is.EqualTo(null));
            Assert.That(doc.NextSibling, Is.EqualTo(null));

            //Child features
            Assert.That(doc.HasChildNodes, Is.EqualTo(false));
            Assert.That(doc.FirstChild, Is.EqualTo(null));
            Assert.That(doc.LastChild, Is.EqualTo(null));
        }

        [Test]
        public void TestSectionCtor()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);

            Section sect = new Section(doc);

            //General features
            Assert.That(sect.NodeType, Is.EqualTo(NodeType.Section));
            Assert.That(sect.IsComposite, Is.EqualTo(true));
            Assert.That(sect.ParentNode, Is.EqualTo(null));        //No parent at this stage.
            Assert.That(sect.Document, Is.EqualTo(doc));    //But owner document is still accessible

            //Sibling features
            Assert.That(sect.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect.NextSibling, Is.EqualTo(null));

            //Child features
            Assert.That(sect.HasChildNodes, Is.EqualTo(false));
            Assert.That(sect.FirstChild, Is.EqualTo(null));
            Assert.That(sect.LastChild, Is.EqualTo(null));
        }

        [Test]
        public void TestCloneNode()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect1 = doc.AppendChild(new Section(doc));
            sect1.AppendChild(new Body(doc));

            //Clone without children
            Section sect2 = (Section)sect1.Clone(false);
            Assert.That(sect2.ParentNode, Is.EqualTo(null));
            Assert.That(sect2.Document, Is.EqualTo(doc));
            Assert.That(sect2.FirstChild, Is.EqualTo(null));

            //Clone with children, check the child was cloned
            Section sect3 = (Section)sect1.Clone(true);
            Assert.That(sect3.FirstChild.NodeType, Is.EqualTo(NodeType.Body));
        }



        /// <summary>
        /// Tests Node.IsAbove() method.
        /// </summary>
        [Test]
        public void TestIsAbove()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Paragraph par0 = builder.InsertParagraph();
            Run run00 = new Run(doc, "00 ");
            builder.InsertNode(run00);
            Run run01 = new Run(doc, "01 ");
            builder.InsertNode(run01);
            Run run02 = new Run(doc, "01 ");
            builder.InsertNode(run02);


            Paragraph par1 = builder.InsertParagraph();
            Run run10 = new Run(doc, "10 ");
            builder.InsertNode(run10);
            Run run11 = new Run(doc, "11 ");
            builder.InsertNode(run11);
            Run run12 = new Run(doc, "12 ");
            builder.InsertNode(run12);

            // Null cases:
            Assert.That(Node.NodeAIsAboveNodeB(null, null), Is.False);
            Assert.That(Node.NodeAIsAboveNodeB(null, run00), Is.False);
            Assert.That(run00.IsAbove(null), Is.False);

            // Same node cases.
            Assert.That(run00.IsAbove(run00), Is.False);
            Assert.That(par0.IsAbove(par0), Is.False);
            Assert.That(doc.IsAbove(doc), Is.False);

            // Leaf cases.
            Assert.That(run00.IsAbove(run01), Is.True);
            Assert.That(run01.IsAbove(run00), Is.False);
            Assert.That(run00.IsAbove(par1), Is.True);

            // Ancestor cases.
            Assert.That(par1.IsAbove(run12), Is.True);
            Assert.That(doc.IsAbove(run00), Is.True);
            Assert.That(par1.IsAbove(doc), Is.False);
            Assert.That(doc.FirstSection.Body.IsAbove(run12), Is.True);

            // Composite node cases.
            Assert.That(par0.IsAbove(par1), Is.True);

            // Different tree cases.
            Paragraph parentlessClone = (Paragraph)par0.Clone(true);
            Assert.That(parentlessClone.ParentNode, Is.Null);
            Assert.That(parentlessClone.IsAbove(run00), Is.False);
            Assert.That(doc.IsAbove(parentlessClone), Is.False);
            Document doc2 = new Document();
            Assert.That(doc.IsAbove(doc2.FirstSection.Body), Is.False);
        }


        [Test]
        public void TestJira5551GetChildGoesToSibling()
        {
            const string fileName = @"Model\Nodes\TestJira5551GetChild.docx";
            Document doc = TestUtil.Open(fileName);

            Paragraph par = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);

            // The second child is Run:
            // WORDSNET-10749 There is now CustomXml in document now. Hence, FirstChild is Run itself.
            Node firstChild = par.FirstChild;
            Assert.That(firstChild.NodeType, Is.EqualTo(NodeType.Run));

            // Get the first run. *Dont'* go deep.
            Node firstRun = par.GetChild(NodeType.Run, 0, false);
            Assert.That(firstRun, IsNot.Null());
        }

        [Test]
        public void TestJira455()
        {
            Document doc = new Document();

            // The run is NOT added to the document. This was causing an exception.
            Run run = new Run(doc, "test");
            Assert.That(run.ToString(SaveFormat.Text), Is.EqualTo("test"));
        }

        /// <summary>
        /// Tests the <see cref="Node.NodeLevel"/> property.
        /// </summary>
        [Test]
        public void TestNodeLevel()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            Assert.That(body.NodeLevel, Is.EqualTo(NodeLevel.SectionStory));

            BookmarkStart bookmarkStart1 = new BookmarkStart(doc, "BM1");
            Assert.That(bookmarkStart1.NodeLevel, Is.EqualTo(NodeLevel.Inline));
            body.AppendChild(bookmarkStart1);
            Assert.That(bookmarkStart1.NodeLevel, Is.EqualTo(NodeLevel.Block));

            Paragraph paragraph1 = new Paragraph(doc);
            body.AppendChild(paragraph1);
            Assert.That(paragraph1.NodeLevel, Is.EqualTo(NodeLevel.Block));

            StructuredDocumentTag sdt1 = new StructuredDocumentTag(doc, MarkupLevel.Block);
            BookmarkEnd bookmarkEnd1 = new BookmarkEnd(doc, "BM1");
            sdt1.AppendChild(bookmarkEnd1);
            Assert.That(bookmarkEnd1.NodeLevel, Is.EqualTo(NodeLevel.Block));
            body.AppendChild(sdt1);
            Assert.That(sdt1.NodeLevel, Is.EqualTo(NodeLevel.Block));

            Paragraph paragraph2 = new Paragraph(doc);
            sdt1.AppendChild(paragraph2);
            Run run1 = new Run(doc);
            paragraph2.AppendChild(run1);
            Assert.That(run1.NodeLevel, Is.EqualTo(NodeLevel.Inline));

            StructuredDocumentTag sdt2 = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            paragraph2.AppendChild(sdt2);
            Assert.That(sdt2.NodeLevel, Is.EqualTo(NodeLevel.Inline));

            BookmarkStart bookmarkStart2 = new BookmarkStart(doc, "BM2");
            // Incorrect node place, but check for NodeLevel.Inline.
            sdt2.AppendChild(bookmarkStart2);
            Assert.That(bookmarkStart2.NodeLevel, Is.EqualTo(NodeLevel.Inline));

            Table table1 = new Table(doc);
            body.AppendChild(table1);
            Assert.That(table1.NodeLevel, Is.EqualTo(NodeLevel.Block));

            BookmarkEnd bookmarkEnd2 = new BookmarkEnd(doc, "BM2");
            table1.AppendChild(bookmarkEnd2);
            Assert.That(bookmarkEnd2.NodeLevel, Is.EqualTo(NodeLevel.Row));

            StructuredDocumentTag sdt3 = new StructuredDocumentTag(doc, MarkupLevel.Row);
            table1.AppendChild(sdt3);
            Assert.That(sdt3.NodeLevel, Is.EqualTo(NodeLevel.Row));

            Row row1 = new Row(doc);
            table1.AppendChild(row1);
            Assert.That(row1.NodeLevel, Is.EqualTo(NodeLevel.Row));

            BookmarkStart bookmarkStart3 = new BookmarkStart(doc, "BM3");
            row1.AppendChild(bookmarkStart3);
            Assert.That(bookmarkStart3.NodeLevel, Is.EqualTo(NodeLevel.Cell));

            StructuredDocumentTag sdt4 = new StructuredDocumentTag(doc, MarkupLevel.Cell);
            row1.AppendChild(sdt4);
            Assert.That(sdt4.NodeLevel, Is.EqualTo(NodeLevel.Cell));

            BookmarkEnd bookmarkEnd3 = new BookmarkEnd(doc, "BM3");
            sdt4.AppendChild(bookmarkEnd3);
            Assert.That(bookmarkEnd3.NodeLevel, Is.EqualTo(NodeLevel.Cell));

            Cell cell1 = new Cell(doc);
            sdt4.AppendChild(cell1);
            Assert.That(cell1.NodeLevel, Is.EqualTo(NodeLevel.Cell));

            BookmarkStart bookmarkStart4 = new BookmarkStart(doc, "BM4");
            cell1.AppendChild(bookmarkStart4);
            Assert.That(bookmarkStart4.NodeLevel, Is.EqualTo(NodeLevel.Block));

            Paragraph paragraph3 = new Paragraph(doc);
            cell1.AppendChild(paragraph3);
            BookmarkEnd bookmarkEnd4 = new BookmarkEnd(doc, "BM4");
            paragraph3.AppendChild(bookmarkEnd4);
            Assert.That(bookmarkEnd4.NodeLevel, Is.EqualTo(NodeLevel.Inline));
        }

        /// <summary>
        /// Relates to WORDSNET-17082 Checks links for inserted node without tracking revisions and with it.
        /// </summary>
        [Test]
        public void TestJira17082()
        {
            // Tests when track revisions is disabled.
            TestJira17082Core(false);

            // Tests when track revisions is enabled.
            TestJira17082Core(true);
        }

        /// <summary>
        /// Tests the <see cref="CompositeNode.AcceptStart"/> and <see cref="CompositeNode.AcceptEnd"/> methods.
        /// </summary>
        [Test]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        public void TestAcceptStartAcceptEnd()
        {
            Document doc = new Document();
            doc.FirstSection.Body.AppendChild(new TestCompositeNode(doc));

            TestDocumentVisitor visitor = new TestDocumentVisitor();

            doc.Accept(visitor);

            Assert.That(visitor.IsTestCompositeNodeStartVisited, Is.True);
            Assert.That(visitor.IsTestCompositeNodeEndVisited, Is.True);
        }

        /// <summary>
        /// Core method for TestJira17082.
        /// Creates new test document, then inserts run from the second paragraph into the first paragraph
        /// and checks correctness of all necessary node links.
        /// </summary>
        private static void TestJira17082Core(bool trackRevisions)
        {
            Document doc = CreateTestJira17082Document();

            if (trackRevisions)
                doc.StartTrackRevisions("Author");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            paras[0].InsertAfter(paras[1].Runs[1], paras[0].FirstRun);

            Paragraph firstPara = doc.FirstSection.Body.FirstParagraph;
            Paragraph secondPara = (Paragraph)firstPara.NextSibling;

            Assert.That(firstPara.Runs.Count, Is.EqualTo(3));
            // The run movement is represented as move ranges: a copy of the run exists in the source paragraph.
            Assert.That(secondPara.Runs.Count, Is.EqualTo(trackRevisions ? 3 : 2));

            // The paragraphs contains move range start/end nodes: have to use XXXNonAnnotationSibling methods.
            Assert.That(firstPara.Runs[1].PreviousNonAnnotationSibling, Is.EqualTo(firstPara.Runs[0]));
            Assert.That(firstPara.Runs[0].NextNonAnnotationSibling, Is.EqualTo(firstPara.Runs[1]));
            Assert.That(firstPara.Runs[2].PreviousNonAnnotationSibling, Is.EqualTo(firstPara.Runs[1]));
            Assert.That(firstPara.Runs[1].NextNonAnnotationSibling, Is.EqualTo(firstPara.Runs[2]));

            Assert.That(secondPara.Runs[1].PreviousNonAnnotationSibling, Is.EqualTo(secondPara.Runs[0]));
            Assert.That(secondPara.Runs[0].NextNonAnnotationSibling, Is.EqualTo(secondPara.Runs[1]));
        }

        /// <summary>
        /// Creates WORDSNET-17082 test document.
        /// </summary>
        private static Document CreateTestJira17082Document()
        {
            DocumentBuilder builder = new DocumentBuilder(new Document());

            builder.Write("First para. First run.");
            builder.Writeln("First para. Second run.");

            builder.Write("Second para. First run.");
            builder.Write("Second para. Second run.");
            builder.Write("Second para. Third run.");

            return builder.Document;
        }

        /// <summary>
        /// Used to test the <see cref="CompositeNode.AcceptStart"/> and <see cref="CompositeNode.AcceptEnd"/> methods
        /// and ability to inherit from <see cref="CompositeNode"/>.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        private class TestCompositeNode : CompositeNode
        {
            internal TestCompositeNode(Document doc)
                : base(doc)
            {
            }

            public override bool Accept(DocumentVisitor visitor)
            {
                return AcceptCore(visitor);
            }

            public override VisitorAction AcceptStart(DocumentVisitor visitor)
            {
                TestDocumentVisitor testVisitor = visitor as TestDocumentVisitor;
                return (testVisitor != null) ? testVisitor.VisitTestCompositeNodeStart(this) : VisitorAction.Continue;
            }

            public override VisitorAction AcceptEnd(DocumentVisitor visitor)
            {
                TestDocumentVisitor testVisitor = visitor as TestDocumentVisitor;
                return (testVisitor != null) ? testVisitor.VisitTestCompositeNodeEnd(this) : VisitorAction.Continue;
            }

            public override NodeType NodeType
            {
                get { return NodeType.System; }
            }
        }

        /// <summary>
        /// Represents a document visitor with support of <see cref="TestCompositeNode"/>.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity]
        private class TestDocumentVisitor : DocumentVisitor
        {
            internal VisitorAction VisitTestCompositeNodeStart(TestCompositeNode node)
            {
                IsTestCompositeNodeStartVisited = true;
                return VisitorAction.Continue;
            }

            internal VisitorAction VisitTestCompositeNodeEnd(TestCompositeNode node)
            {
                IsTestCompositeNodeEndVisited = true;
                return VisitorAction.Continue;
            }

            internal bool IsTestCompositeNodeStartVisited { get; private set; }
            internal bool IsTestCompositeNodeEndVisited { get; private set; }
        }
    }
}
