// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.Markup;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Basic node functionality tests.
    /// </summary>
    [TestFixture]
    public class TestNonMarkupTraverse
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestFirstNonMarkupDescendant()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Paragraph par = builder.InsertParagraph();
            Assert.That(par.FirstNonMarkupDescendant, Is.Null, "First descendant of an empty composite should be null.");

            StructuredDocumentTag tagFirst = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            StructuredDocumentTag tagNext = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            builder.InsertNode(tagFirst);
            builder.InsertNode(tagNext);
            Assert.That(tagFirst.NextSibling, Is.SameAs(tagNext));
            Assert.That(par.FirstNonMarkupDescendant, Is.Null, "First non-markup descendant of a composite having only markups inside should be null.");

            Run textRun = new Run(doc, "text");
            builder.InsertNode(textRun);
            Assert.That(tagNext.NextSibling, Is.SameAs(textRun));
            Assert.That(par.FirstNonMarkupDescendant, Is.SameAs(textRun), "Immediate child which is not a first child should be found.");

            Run runInDepth = new Run(doc, "depth");
            tagNext.InsertBefore(runInDepth, null);
            Assert.That(tagNext.FirstChild, Is.SameAs(runInDepth));
            Assert.That(par.FirstNonMarkupDescendant, Is.SameAs(runInDepth), "Depth-first traverse order should find deeper descendants of preceding children.");
        }

        [Test]
        public void TestNextNonMarkupNode()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Paragraph par = builder.InsertParagraph();
            Run firstRun = new Run(doc, "first Run");
            par.InsertBefore(firstRun, null);
            Assert.That(par.FirstChild, Is.SameAs(firstRun));
            Assert.That(firstRun.NextNonMarkupNodeLimited, Is.Null, "No siblings: should be null.");

            StructuredDocumentTag tagFirst = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            StructuredDocumentTag tagNext = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            builder.InsertNode(tagFirst);
            Assert.That(firstRun.NextSibling, Is.SameAs(tagFirst));
            builder.InsertNode(tagNext);
            Assert.That(tagFirst.NextSibling, Is.SameAs(tagNext));
            Assert.That(firstRun.NextNonMarkupNodeLimited, Is.Null, "No non-markup siblings: should be null.");

            Run siblingRun = new Run(doc, "sibling run");
            par.InsertAfter(siblingRun, tagNext);
            Assert.That(tagNext.NextSibling, Is.SameAs(siblingRun));
            Assert.That(firstRun.NextNonMarkupNodeLimited, Is.SameAs(siblingRun), "Non-markup sibling not found.");

            Run grandChildRun = new Run(doc, "grandchild Run");
            tagNext.InsertBefore(grandChildRun, null);
            Assert.That(tagNext.FirstChild, Is.SameAs(grandChildRun));
            Assert.That(firstRun.NextNonMarkupNodeLimited, Is.SameAs(grandChildRun), "Depth-first traverse order should find deeper descendants of preceding children.");

            Assert.That(tagNext.NextNonMarkupNodeLimited, Is.SameAs(siblingRun), "Search should not go inside the current node.");
            Assert.That(grandChildRun.NextNonMarkupNodeLimited, Is.SameAs(siblingRun), "Search should go up markup nodes.");
            
            Run grandGrandChildRun = new Run(doc, "grand grand child");
            StructuredDocumentTag grandChildTag = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            tagFirst.InsertBefore(grandChildTag, null);
            Assert.That(tagFirst.FirstChild, Is.SameAs(grandChildTag));
            grandChildTag.InsertBefore(grandGrandChildRun, null);
            Assert.That(grandChildTag.FirstChild, Is.SameAs(grandGrandChildRun));

            Assert.That(grandGrandChildRun.NextNonMarkupNodeLimited, Is.SameAs(grandChildRun), "Search should go deep after going up.");

            Paragraph nextPar = (Paragraph) par.ParentNode.InsertAfter(new Paragraph(doc), par);
            nextPar.InsertBefore(new Run(doc, "test"), null);
            Assert.That(siblingRun.NextNonMarkupNodeLimited, Is.Null, "Search should go up markup nodes only");
        }

        [Test]
        public void TestPreviousNonMarkupNode()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Paragraph par = builder.InsertParagraph();

            StructuredDocumentTag tagFirst = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            builder.InsertNode(tagFirst);
            Assert.That(par.FirstChild, Is.SameAs(tagFirst));
            Assert.That(tagFirst.PreviousNonMarkupNodeLimited, Is.Null, "No siblings: should be null.");

            StructuredDocumentTag tagNext = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            builder.InsertNode(tagNext);
            Assert.That(tagFirst.NextSibling, Is.SameAs(tagNext));
            Assert.That(tagNext.PreviousNonMarkupNodeLimited, Is.Null, "No non-markup siblings: should be null.");

            Run firstRun = new Run(doc, "first Run");
            par.InsertBefore(firstRun, tagFirst);
            Assert.That(par.FirstChild, Is.SameAs(firstRun));
            Assert.That(tagNext.PreviousNonMarkupNodeLimited, Is.SameAs(firstRun), "Non-markup sibling not found.");

            Run lastRun = new Run(doc, "last run");
            par.InsertAfter(lastRun, tagNext);
            Assert.That(tagNext.NextSibling, Is.SameAs(lastRun));

            Run grandChildRun = new Run(doc, "grandchild Run");
            tagNext.InsertBefore(grandChildRun, null);
            Assert.That(tagNext.FirstChild, Is.SameAs(grandChildRun));
            Assert.That(lastRun.PreviousNonMarkupNodeLimited, Is.SameAs(grandChildRun), "Depth-first traverse order should find deeper descendants of preceding children.");

            Assert.That(tagNext.PreviousNonMarkupNodeLimited, Is.SameAs(firstRun), "Search should not go inside the current node.");
            Assert.That(grandChildRun.PreviousNonMarkupNodeLimited, Is.SameAs(firstRun), "Search should go up markup nodes.");

            Run grandGrandChildRun = new Run(doc, "grand grand child");
            StructuredDocumentTag grandChildTag = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            tagFirst.InsertBefore(grandChildTag, null);
            Assert.That(tagFirst.FirstChild, Is.SameAs(grandChildTag));
            grandChildTag.InsertBefore(grandGrandChildRun, null);
            Assert.That(grandChildTag.FirstChild, Is.SameAs(grandGrandChildRun));

            Assert.That(grandChildRun.PreviousNonMarkupNodeLimited, Is.SameAs(grandGrandChildRun), "Search should go deep after going up.");

            Paragraph nextPar = (Paragraph) par.ParentNode.InsertAfter(new Paragraph(doc), par);
            Run nextParRun = new Run(doc, "next par run");
            nextPar.InsertBefore(nextParRun, null);
            Assert.That(nextPar.FirstChild, Is.SameAs(nextParRun));
            Assert.That(nextParRun.PreviousNonMarkupNodeLimited, Is.Null, "Search should go up markup nodes only");
        }

        [Test]
        public void TestLastNonMarkupDescendant()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Paragraph par = builder.InsertParagraph();
            Assert.That(par.LastNonMarkupDescendant, Is.Null, "Last descendant of an empty composite should be null.");

            StructuredDocumentTag tagFirst = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            StructuredDocumentTag tagNext = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            builder.InsertNode(tagFirst);
            builder.InsertNode(tagNext);
            Assert.That(tagFirst.NextSibling, Is.SameAs(tagNext));
            Assert.That(par.LastNonMarkupDescendant, Is.Null, "Last non-markup descendant of a composite having only markups inside should be null.");

            Run textRun = new Run(doc, "text");
            par.InsertBefore(textRun, tagFirst);
            Assert.That(textRun.NextSibling, Is.SameAs(tagFirst));
            Assert.That(par.LastNonMarkupDescendant, Is.SameAs(textRun), "Immediate child which is not the last child should be found.");

            Run runInDepth = new Run(doc, "depth");
            tagFirst.InsertBefore(runInDepth, null);
            Assert.That(tagFirst.FirstChild, Is.SameAs(runInDepth));
            Assert.That(par.LastNonMarkupDescendant, Is.SameAs(runInDepth), "Depth-first traverse order should find deeper descendants of succeeding children.");

            Run lastRun = new Run(doc, "last run");
            par.InsertAfter(lastRun, tagNext);
            Assert.That(tagNext.NextSibling, Is.SameAs(lastRun));
            Assert.That(par.LastNonMarkupDescendant, Is.SameAs(lastRun), "Last non-markup should be found on upper levels despite deeper preceding nodes.");
        }

        /// <summary>
        /// WORDSNET-15251 GetChildNodes was returning extra nodes.
        /// When a root node is a markup, <see cref="NodeCollection.TraverseFlatSkipMarkupNodes"/> was returning
        /// root-sibling node on a last iteration.
        /// </summary>
        [Test]
        public void TestJira15251()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertNode(new Run(doc, "First run"));

            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, MarkupLevel.Inline);
            builder.InsertNode(sdt);
            sdt.AppendChild(new Run(doc, "Inner run"));

            builder.InsertNode(new Run(doc, "Last run"));

            NodeCollection runsInsideSdtBlock = sdt.GetChildNodes(NodeType.Run, false);
            Node firstChild = runsInsideSdtBlock[0]; 

            Assert.That(runsInsideSdtBlock.Count, Is.EqualTo(1), "It must be only one NodeType.Run descendant.");

            // Verify forward sibling traverse.
            Assert.That(runsInsideSdtBlock.GetNthMatchingNode(firstChild, 1), Is.Null);

            // Verify backward sibling traverse.
            Assert.That(runsInsideSdtBlock.GetNthMatchingNode(firstChild, -1), Is.Null);
        }

        /// <summary>
        /// Tests the <see cref="Node.NextNonMarkupCompositeLimited"/>,
        /// <see cref="Node.PreviousNonMarkupCompositeLimited"/> properties.
        /// </summary>
        [Test]
        public void TestNextNonMarkupCompositeLimited()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;

            Paragraph paragraph1 = new Paragraph(doc);
            body.AppendChild(paragraph1);
            BookmarkStart bookmarkStart1 = new BookmarkStart(doc, "BM1");
            paragraph1.AppendChild(bookmarkStart1);
            Run run1 = new Run(doc);
            paragraph1.AppendChild(run1);

            BookmarkEnd bookmarkEnd1 = new BookmarkEnd(doc, "BM1");
            body.AppendChild(bookmarkEnd1);

            StructuredDocumentTag sdt1 = new StructuredDocumentTag(doc, MarkupLevel.Block);
            body.AppendChild(sdt1);
            Paragraph sdt1Paragraph1 = new Paragraph(doc);
            sdt1.AppendChild(sdt1Paragraph1);
            Run run2 = new Run(doc);
            sdt1Paragraph1.AppendChild(run2);
            BookmarkStart bookmarkStart2 = new BookmarkStart(doc, "BM2");
            sdt1.AppendChild(bookmarkStart2);

            Paragraph paragraph2 = new Paragraph(doc);
            body.AppendChild(paragraph2);
            Run run3 = new Run(doc);
            paragraph2.AppendChild(run3);
            BookmarkEnd bookmarkEnd2 = new BookmarkEnd(doc, "BM2");
            paragraph2.AppendChild(bookmarkEnd2);

            // Check with using NextNonMarkupNodeLimited.
            List<Node> nodeOrder = new List<Node>();
            Node node = paragraph1;
            while (node != null)
            {
                nodeOrder.Add(node);
                node = node.NextNonMarkupNodeLimited;
            }

            Node[] expectedOrder = new Node[] { paragraph1, bookmarkEnd1, sdt1Paragraph1, bookmarkStart2, paragraph2 };
            CheckForSame(expectedOrder, nodeOrder, false);

            // Check with using PreviousNonMarkupNodeLimited.
            nodeOrder.Clear();
            node = expectedOrder[expectedOrder.Length - 1];
            while (nodeOrder.Count != expectedOrder.Length)
            {
                nodeOrder.Add(node);
                node = node.PreviousNonMarkupNodeLimited;
            }

            CheckForSame(expectedOrder, nodeOrder, true);

            // Test NextNonMarkupCompositeLimited
            nodeOrder.Clear();
            node = paragraph1;
            while (node != null)
            {
                nodeOrder.Add(node);
                node = node.NextNonMarkupCompositeLimited;
            }

            expectedOrder = new Node[] { paragraph1, sdt1Paragraph1, paragraph2 };
            CheckForSame(expectedOrder, nodeOrder, false);

            // Test PreviousNonMarkupCompositeLimited
            nodeOrder.Clear();
            node = expectedOrder[expectedOrder.Length - 1];
            while (nodeOrder.Count != expectedOrder.Length)
            {
                nodeOrder.Add(node);
                node = node.PreviousNonMarkupCompositeLimited;
            }

            CheckForSame(expectedOrder, nodeOrder, true);
        }

        /// <summary>
        /// Checks that same nodes are at same positions in the specified array and list.
        /// </summary>
        private static void CheckForSame(Node[] expectedObjects, List<Node> actualObjects, bool reverseOrder)
        {
            Assert.That(actualObjects.Count, Is.EqualTo(expectedObjects.Length));
            for (int i = 0, j = expectedObjects.Length - 1; i < expectedObjects.Length; i++, j--)
                Assert.That(actualObjects[reverseOrder ? j : i], Is.SameAs(expectedObjects[i]));
        }

        /// <summary>
        /// Tests the <see cref="CompositeNode.FirstNonMarkupCompositeLimited"/>,
        /// <see cref="CompositeNode.LastNonMarkupCompositeLimited"/> properties.
        /// </summary>
        [Test]
        public void TestFirstNonMarkupCompositeLimited()
        {
            Document doc = new Document();
            Body body = doc.FirstSection.Body;
            body.RemoveAllChildren();

            StructuredDocumentTag sdt1 = new StructuredDocumentTag(doc, MarkupLevel.Block);
            body.AppendChild(sdt1);
            BookmarkStart bookmarkStart1 = new BookmarkStart(doc, "BM1");
            sdt1.AppendChild(bookmarkStart1);
            Paragraph sdt1Paragraph1 = new Paragraph(doc);
            sdt1.AppendChild(sdt1Paragraph1);
            Run run2 = new Run(doc);
            sdt1Paragraph1.AppendChild(run2);
            BookmarkEnd bookmarkEnd1 = new BookmarkEnd(doc, "BM1");
            sdt1.AppendChild(bookmarkEnd1);

            StructuredDocumentTag sdt2 = new StructuredDocumentTag(doc, MarkupLevel.Block);
            body.AppendChild(sdt2);
            BookmarkStart bookmarkStart2 = new BookmarkStart(doc, "BM2");
            sdt2.AppendChild(bookmarkStart2);

            BookmarkEnd bookmarkEnd2 = new BookmarkEnd(doc, "BM2");
            body.AppendChild(bookmarkEnd2);

            Assert.That(sdt1.FirstNonMarkupCompositeDescendant, Is.SameAs(sdt1Paragraph1));
            Assert.That(sdt1.LastNonMarkupCompositeDescendant, Is.SameAs(sdt1Paragraph1));

            Assert.That(sdt2.FirstNonMarkupCompositeDescendant, Is.Null);
            Assert.That(sdt2.LastNonMarkupCompositeDescendant, Is.Null);

            Assert.That(body.FirstNonMarkupCompositeDescendant, Is.SameAs(sdt1Paragraph1));
            Assert.That(body.LastNonMarkupCompositeDescendant, Is.SameAs(sdt1Paragraph1));
        }
    }
}
