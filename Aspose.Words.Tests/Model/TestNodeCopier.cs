// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestNodeCopier
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestRunsCopying()
        {
            mDocument = TestUtil.Open(@"Model\Nodes\TestNodeCopierRuns.docx");

            const int SourceRunIdx = 1;
            Run sourceRun = GetTestRun(SourceRunIdx);
            Run nextSourceRun = GetTestRun(SourceRunIdx + 1);
            Run previousSourceRun = GetTestRun(SourceRunIdx - 1);

            const int ReferenceRunIndex = 3;
            Run referenceRun = GetTestRun(ReferenceRunIndex);

            // Copy a single run, whole.
            NodeRange copyRange = new NodeRange(sourceRun, true, sourceRun, true);
            NodeCopier.Copy(copyRange, referenceRun);

            // Copy a single run, whole, end on the next run
            copyRange = new NodeRange(sourceRun, true, nextSourceRun, false);
            NodeCopier.Copy(copyRange, referenceRun);

            // Copy a single run, whole, start on the previous run
            copyRange = new NodeRange(previousSourceRun, false, sourceRun, true);
            NodeCopier.Copy(copyRange, referenceRun);

            // Copy a single run, whole, start on the previous run and end on the next run.
            copyRange = new NodeRange(previousSourceRun, false, nextSourceRun, false);
            NodeCopier.Copy(copyRange, referenceRun);

            // Copy a single run, whole, start on the previous run and end on the next run.
            copyRange = new NodeRange(previousSourceRun, false, nextSourceRun, false);
            NodeCopier.Copy(copyRange, referenceRun);

            // One symbol from the beginning, and one from the end.
            DocumentPosition start = new DocumentPosition(previousSourceRun, previousSourceRun.Text.Length - 1);
            DocumentPosition end = new DocumentPosition(nextSourceRun, 1);
            copyRange = new NodeRange(start, end);
            NodeCopier.Copy(copyRange, referenceRun);

            TestUtil.SaveOpen(mDocument, @"Model\Nodes\TestNodeCopierRuns Modified.docx");
        }

        [Test]
        public void TestParagraphCopying11()
        {
            // Copy a single paragraph
            TestParagraphCopyingBody(true, true);
        }

        [Test]
        public void TestParagraphCopying10()
        {
            // Copy a single paragraph, do not include the paragraph after
            TestParagraphCopyingBody(true, false);
        }

        [Test]
        public void TestParagraphCopying01()
        {
            // Copy a single paragraph, do not include the paragraph before
            TestParagraphCopyingBody(false, true);
        }

        [Test]
        public void TestParagraphCopying00()
        {
            // Copy a single paragraph, do not include the paragraphs before and after
            TestParagraphCopyingBody(false, false);
        }

        private void TestParagraphCopyingBody(bool startFromSourceParagraph, bool endOnSourceParagraph)
        {
            const int SourceParagraphIndex = 1;
            const int ReferenceRunIndex = 8;
            TestNodeCopyingBody(SourceParagraphIndex, startFromSourceParagraph, endOnSourceParagraph, ReferenceRunIndex, "Paragraphs");
        }

        private void TestNodeCopyingBody(int sourceNodeIndex, bool startFromSourceNode, bool endOnSourceNode, int referenceNodeIndex, string testFileName)
        {
            mDocument = TestUtil.Open(@"Model\Nodes\TestNodeCopier" + testFileName + ".docx");

            Debug.WriteLine(string.Format("Start from source node: {0}; End on source node: {1}", startFromSourceNode, endOnSourceNode));

            Node sourceNode = mDocument.FirstSection.Body.GetChildNodes(NodeType.Any, false)[sourceNodeIndex];
            Node startNode = startFromSourceNode ? sourceNode : sourceNode.PreviousSibling;
            Node endNode = endOnSourceNode ? sourceNode : sourceNode.NextSibling;

            Run referenceRun = GetTestRun(referenceNodeIndex);

            // Copy a single paragraph
            NodeRange copyRange = new NodeRange(startNode, startFromSourceNode, endNode, endOnSourceNode);
            NodeCopier.Copy(copyRange, referenceRun);

            TestUtil.SaveOpen(mDocument, @"Model\Nodes\TestNodeCopier" + testFileName + " Modified.docx");
        }

        [Test]
        public void TestParagraphCopying2()
        {
            mDocument = TestUtil.Open(@"Model\Nodes\TestNodeCopierParagraphs2.docx");

            const int SourceParagraphIndex = 1;
            Paragraph sourceParagraph = GetTestParagraph(SourceParagraphIndex);
            Paragraph previousParagraph = GetTestParagraph(SourceParagraphIndex - 1);

            Run startRun = (Run)previousParagraph.GetChildNodes(NodeType.Any, false)[0];
            Run endRun = (Run)sourceParagraph.GetChildNodes(NodeType.Any, false)[1];

            Run referenceRun = GetReferenceRun2();

            DocumentPosition startPosition = new DocumentPosition(startRun, startRun.Text.Length / 2);
            DocumentPosition endPosition = new DocumentPosition(endRun, endRun.Text.Length / 2);
            NodeRange copyRange = new NodeRange(startPosition, endPosition);
            NodeCopier.Copy(copyRange, referenceRun);

            TestUtil.SaveOpen(mDocument, @"Model\Nodes\TestNodeCopierParagraphs2 Modified.docx");
        }

        [Test]
        public void TestParagraphCopying200()
        {
            // Empty paragraph
            TestParagraphCopyingBody2(false, false);
        }

        [Test]
        public void TestParagraphCopying201()
        {
            // All from the second paragraph
            TestParagraphCopyingBody2(false, true);
        }

        [Test]
        public void TestParagraphCopying210()
        {
            // All from the first paragraph
            TestParagraphCopyingBody2(true, false);
        }

        [Test]
        public void TestParagraphCopying211()
        {
            // All from the both paragraphs
            TestParagraphCopyingBody2(true, true);
        }

        private void TestParagraphCopyingBody2(bool startOnFirstRun, bool endOnFirstRun)
        {
            mDocument = TestUtil.Open(@"Model\Nodes\TestNodeCopierParagraphs2.docx");

            Debug.WriteLine(string.Format("Start from first run of par 0: {0}; End on first run of par 1: {1}", startOnFirstRun, endOnFirstRun));

            const int SourceParagraphIndex = 0;
            Paragraph startParagraph = GetTestParagraph(SourceParagraphIndex);
            DocumentPosition startPosition = GetParagraphRun(startParagraph, startOnFirstRun);
            
            Paragraph endParagraph = (Paragraph)startParagraph.NextSibling;
            DocumentPosition endPosition = GetParagraphRun(endParagraph, endOnFirstRun);

            Run referenceRun = GetReferenceRun2();

            // Copy a single paragraph
            NodeRange copyRange = new NodeRange(startPosition, endPosition);
            NodeCopier.Copy(copyRange, referenceRun);

            string saveSuffix = (startOnFirstRun ? "1" : "0") +
                (endOnFirstRun ? "1" : "0") +
                " Modified.docx";

            TestUtil.SaveOpen(mDocument, @"Model\Nodes\TestNodeCopierParagraphs2 " + saveSuffix);
        }

        [Test]
        public void TestTableCopying11()
        {
            // Copy a single table
            TestTableCopyingBody(true, true);
        }

        [Test]
        public void TestTableCopying10()
        {
            // Copy a single table, do not include the paragraph after
            TestTableCopyingBody(true, false);
        }

        [Test]
        public void TestTableCopying01()
        {
            // Copy a single table, do not include the paragraph before
            TestTableCopyingBody(false, true);
        }

        [Test]
        public void TestTableCopying00()
        {
            // Copy a single table, do not include the paragraphs before and after
            TestTableCopyingBody(false, false);
        }

        private void TestTableCopyingBody(bool startFromSourceNode, bool endOnSourceNode)
        {
            const int SourceTableIndex = 1;
            const int ReferenceRunIndex = 12;
            TestNodeCopyingBody(SourceTableIndex, startFromSourceNode, endOnSourceNode, ReferenceRunIndex, "Table");
        }

        [Test]
        public void TestLastParagraph()
        {
            const int SourceParagraphIndex = 3;
            const int ReferenceRunIndex = 1;
            const bool StartFromSourceNode = true;
            const bool EndOnSourceNode = true;
            TestNodeCopyingBody(SourceParagraphIndex, StartFromSourceNode, EndOnSourceNode, ReferenceRunIndex, "LastParagraph");
        }

        [Test]
        public void TestNoSiblingForTheReferenceNode()
        {
            const int SourceParagraphIndex = 2;
            const int ReferenceRunIndex = 2;
            TestNodeCopyingBody(SourceParagraphIndex, true, true, ReferenceRunIndex, "NoSibling");
        }

        /// <summary>
        /// Copying paragraphs from one document to another.
        /// </summary>
        [Test]
        public void TestParagraphCopying3()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestNodeCopierParagraphs.docx");

            // Source document with two paragraphs.
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestNodeCopierParagraphs2.docx");

            NodeCopierCore(srcDoc, dstDoc);

            Assert.That(dstDoc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(8));
        }

        /// <summary>
        /// Copying sections from one document to another.
        /// </summary>
        [Test]
        public void TestSectionCopying()
        {
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestNodeCopierParagraphs.docx");

            // Source document with three sections.
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestNodeCopierSections.docx");

            NodeCopierCore(srcDoc, dstDoc);

            Assert.That(dstDoc.Sections[0].Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(dstDoc.Sections[1].Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(dstDoc.Sections[2].Body.Paragraphs.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Upon UpdateFields AW uses NodeCopier to replace fields with the field results.
        /// Test this when field result contains section break. 
        /// </summary>
        [Test]
        public void TestFieldResultCopying()
        {
            Document doc = TestUtil.Open(@"Model\Nodes\TestFieldCopying.docx");
            doc.UpdateFields();

            // Last section should contain one paragraph after update fields.
            Assert.That(doc.LastSection.Body.Paragraphs.Count, Is.EqualTo(1));
        }

        private static void NodeCopierCore(Document srcDoc, Document dstDoc)
        {
            NodeRange sourceRange = new NodeRange(srcDoc.FirstSection.Body.FirstChild, srcDoc.LastSection.Body.LastChild);

            ExternalDocumentModifier modifier = new ExternalDocumentModifier(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting);
            NodeCopier.Copy(sourceRange, dstDoc.FirstSection.Body.FirstParagraph, modifier);
        }

        private static DocumentPosition GetParagraphRun(Paragraph par, bool firstRun)
        {
            Run run = (Run)(firstRun ? par.FirstChild : par.LastChild);
            return new DocumentPosition(run, (firstRun ? 0 : run.Text.Length));
        }

        private Run GetTestRun(int runIdx)
        {
            return (Run)GetTestNode(mDocument, NodeType.Run, runIdx);
        }

        private Paragraph GetTestParagraph(int idx)
        {
            return (Paragraph)GetTestNode(mDocument, NodeType.Paragraph, idx);
        }

        private static Node GetTestNode(Document doc, NodeType nodeType, int nodeIdx)
        {
            return doc.GetChildNodes(nodeType, true)[nodeIdx];
        }

        private Run GetReferenceRun2()
        {
            const int ReferenceRunIndex = 8;
            return GetTestRun(ReferenceRunIndex);
        }

        private Document mDocument;
    }
}
