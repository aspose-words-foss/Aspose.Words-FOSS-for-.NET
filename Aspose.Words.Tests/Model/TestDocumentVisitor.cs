// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.Fields;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// This really belongs to examples.
    /// </summary>
    [TestFixture]
    public class TestDocumentVisitor
    {
        [Test]
        public void TestExtractNormal()
        {
            TestUtil.SetUpTests();
            Document doc = TestUtil.Open(@"Model\Nodes\TestDocumentVisitor.docx");

            NormalExtractingVisitor visitor = new NormalExtractingVisitor();
            doc.Accept(visitor);

            Assert.That(visitor.GetExtractedText(), Is.EqualTo("Normal line 1.\r" +
                "Normal line 2.\r" +
                "\x000c"));
        }


        private static void CheckTotalNodesCount(Document doc, TestNodeCountingVisitor nodeCounter, NodeType nodeType)
        {
            int expectedNodeCount = doc.GetChildNodes(nodeType, true).Count;
            Assert.That(nodeCounter.GetVisitedNodesCounter(nodeType), Is.EqualTo(expectedNodeCount));
        }
    }

    /// <summary>
    /// A sample class that shows how to implement DocumentVisitor
    /// to extract text from all paragraphs of Normal style.
    /// </summary>
    internal class NormalExtractingVisitor : DocumentVisitor
    {
        public override VisitorAction VisitDocumentStart(Document doc)
        {
            extractedText = new System.Text.StringBuilder();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            if (para.ParagraphFormat.StyleName == "Normal")
                extractedText.Append(para.GetText());
            return VisitorAction.Continue;
        }

        public string GetExtractedText()
        {
            return extractedText.ToString();
        }

        private System.Text.StringBuilder extractedText;
    }

    /// <summary>
    /// Counts visits to the nodes of different types.
    /// </summary>
    internal class TestNodeCountingVisitor : DocumentVisitor
    {
        public override VisitorAction VisitFieldStart(FieldStart fieldChar)
        {
            IncrementCounter(fieldChar.NodeType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldChar)
        {
            IncrementCounter(fieldChar.NodeType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldChar)
        {
            IncrementCounter(fieldChar.NodeType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRun(Run run)
        {
            IncrementCounter(run.NodeType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphEnd(Paragraph paragraph)
        {
            IncrementCounter(paragraph.NodeType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellEnd(Tables.Cell cell)
        {
            IncrementCounter(cell.NodeType);
            return VisitorAction.Continue;
        }

        public int GetVisitedNodesCounter(NodeType nodeType)
        {
            return GetNodeTypeCounter(mNodeTypeCounters, nodeType);
        }

        internal static int GetNodeTypeCounter(Dictionary<NodeType, int> nodeTypeCounters, NodeType nodeType)
        {
            return nodeTypeCounters.GetValueOrDefault(nodeType,0);
        }

        private void IncrementCounter(NodeType nodeType)
        {
            IncrementCounter(mNodeTypeCounters, nodeType);
        }

        private static void IncrementCounter(IDictionary<NodeType, int> nodeTypeCounters, NodeType nodeType)
        {
            int counter = nodeTypeCounters.GetValueOrDefault(nodeType,0);
            nodeTypeCounters[nodeType] = counter + 1;
        }

        private readonly Dictionary<NodeType, int> mNodeTypeCounters = new Dictionary<NodeType, int>();
    }

}
