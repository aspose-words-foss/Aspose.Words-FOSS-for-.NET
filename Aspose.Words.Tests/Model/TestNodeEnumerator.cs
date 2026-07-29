// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test that node lists and enumeration works.
    /// </summary>
    [TestFixture]
    public class TestNodeEnumerator
    {
        /// <summary>
        /// Test moving in a single run.
        /// </summary>
        /// <remarks>
        /// The end postion of the range is interesting. Offset n means that symbol at offset n is also included.
        /// But offset string.Length is legal as well. So offset String.Length - 1 and String.Length mean the same.
        /// Weird.
        /// </remarks>
        [Test]
        public void TestSingleRun()
        {
            Run testRun = TestNodeRange.GetTestRun(0);
            string testText = testRun.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");

            TestSingleRunBody(0, 1, "T");
            TestSingleRunBody(0, testText.Length - 1, "Tex");
            TestSingleRunBody(0, testText.Length, "Text");
            TestSingleRunBody(1, testText.Length - 1, "ex");
            
            TestSingleRunBody(1, testText.Length, "ext");

            TestSingleRunBody(testText.Length - 1, testText.Length, "t");
        }

        private static void TestSingleRunBody(int startOffset, int endOffset, string expectedString)
        {
            Run run = TestNodeRange.GetTestRun(0);
            NodeEnumerator enumerator = MakeTestEnumerator(run, startOffset, run, endOffset);

            string context = TestNodeRange.MakeSingleRunContext(expectedString, startOffset, endOffset);

            Assert.That(enumerator.MoveToNextNode(), Is.True, context);

            Run extracted = (Run)enumerator.ExtractCurrentNode(null, NodeExtractBehavior.ModifyChildrenCloneAll);
            Assert.That(extracted.Text, Is.EqualTo(expectedString), context);
        }

        private static NodeEnumerator MakeTestEnumerator(Run run0, int startOffset, Run run1, int endOffset)
        {
            NodeRange range = TestNodeRange.MakeRangeFromRuns(run0, startOffset, run1, endOffset);
            return new NodeEnumerator(range);
        }

        [Test]
        public void TestFailOnFirstMove()
        {
            Run testRun = TestNodeRange.GetTestRun(0);
            string testText = testRun.Text;

            // These should fail on the first move.
            TestFailOnFirstMoveBody(0, 0);
            TestFailOnFirstMoveBody(1, 1);

            TestFailOnFirstMoveBody(1, 0);
            TestFailOnFirstMoveBody(testText.Length - 1, testText.Length - 1);
            TestFailOnFirstMoveBody(testText.Length, testText.Length);
            TestFailOnFirstMoveBody(testText.Length, testText.Length + 1);
        }

        private static void TestFailOnFirstMoveBody(int startOffset, int endOffset)
        {
            Run testRun = TestNodeRange.GetTestRun(0);
            NodeEnumerator enumerator = MakeTestEnumerator(testRun, startOffset, testRun, endOffset);

            string context = TestNodeRange.MakeSingleRunContext(string.Empty, startOffset, endOffset);

            Assert.That(enumerator.MoveToNextNode(), Is.False, context);
        }

        [Test]
        public void Test2Runs()
        {
            Run run0 = TestNodeRange.GetTestRun(0);
            Run run1 = TestNodeRange.GetTestRun(1);
            string testText = run0.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");
            Assert.That("Text", Is.EqualTo(run1.Text), "Unexpected test run text");

            Test2RunsBody(0, 1, "Text", "T");
            Test2RunsBody(0, testText.Length - 1, "Text", "Tex");
            Test2RunsBody(0, testText.Length, "Text", "Text");
            Test2RunsBody(1, testText.Length - 1, "ext", "Tex");
            Test2RunsBody(1, testText.Length, "ext", "Text");
            Test2RunsBody(1, 1, "ext", "T");
            Test2RunsBody(testText.Length - 1, 1, "t", "T");

        }

        // Actually, this behavior causes issue 26152 as FieldCodeParser makes such ranges and cosiders them valid.
        // It cannot be fixed easily however, because there are tests already relying on this behaviour.
        [Test]
        public void TestSecondMoveFailure()
        {
            Run run0 = TestNodeRange.GetTestRun(0);
            Run run1 = TestNodeRange.GetTestRun(1);
            string testText = run0.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");
            Assert.That("Text", Is.EqualTo(run1.Text), "Unexpected test run text");

            // Should fail on the second move:
            Test2RunsFailOnTheSecondMoveBody(0, 0, "Text");
            Test2RunsFailOnTheSecondMoveBody(testText.Length - 1, 0, "t");
            // These ones move right to the "Text" in the second run on the first move.
            Test2RunsFailOnTheSecondMoveBody(testText.Length, 2, "Te");
            Test3RunsFailOnTheSecondMoveBody(testText.Length, 0, "Text");
        }

        private static void Test2RunsBody(int startOffset, int endOffset, string expectedText0, string expectedText1)
        {
            Test2RunsBody(startOffset, endOffset, expectedText0, expectedText1, 1);
        }

        private static void Test3RunsBody(int startOffset, int endOffset, string expectedText0, string expectedText1)
        {
            Test2RunsBody(startOffset, endOffset, expectedText0, expectedText1, 2);
        }

        private static void Test2RunsBody(int startOffset, int endOffset, string expectedText0, string expectedText1, int run1Index)
        {
            Run run0 = TestNodeRange.GetTestRun(0);
            Run run1 = TestNodeRange.GetTestRun(run1Index);
            NodeEnumerator enumerator = MakeTestEnumerator(run0, startOffset, run1, endOffset);

            string context = TestNodeRange.MakeDualRunContext(expectedText0, startOffset, expectedText1, endOffset);

            Assert.That(enumerator.MoveToNextNode(), Is.True, "First move failed. " + context);
            Run extracted = (Run)enumerator.ExtractCurrentNode(null, NodeExtractBehavior.ModifyChildrenCloneAll);
            Assert.That(extracted.Text, Is.EqualTo(expectedText0), context);

            Assert.That(enumerator.MoveToNextNode(), Is.True, "Second move failed. " + context);
            extracted = (Run)enumerator.ExtractCurrentNode(null, NodeExtractBehavior.ModifyChildrenCloneAll);
            Assert.That(extracted.Text, Is.EqualTo(expectedText1), context);
        }

        private static void Test2RunsFailOnTheSecondMoveBody(int startOffset, int endOffset, string expectedText)
        {
            Test2RunsFailOnTheSecondMoveBody(startOffset, endOffset, expectedText, 1);
        }

        private static void Test3RunsFailOnTheSecondMoveBody(int startOffset, int endOffset, string expectedText)
        {
            Test2RunsFailOnTheSecondMoveBody(startOffset, endOffset, expectedText, 2);
        }

        private static void Test2RunsFailOnTheSecondMoveBody(int startOffset, int endOffset, string expectedText, int run1Index)
        {
            // TODO remove duplication
            Run run0 = TestNodeRange.GetTestRun(0);
            Run run1 = TestNodeRange.GetTestRun(run1Index);
            NodeEnumerator enumerator = MakeTestEnumerator(run0, startOffset, run1, endOffset);

            string context = TestNodeRange.MakeDualRunContext(expectedText, startOffset, string.Empty, endOffset);

            Assert.That(enumerator.MoveToNextNode(), Is.True, "First move failed. " + context);
            Run extracted = (Run)enumerator.ExtractCurrentNode(null, NodeExtractBehavior.ModifyChildrenCloneAll);
            Assert.That(extracted.Text, Is.EqualTo(expectedText), context);

            Assert.That(enumerator.MoveToNextNode(), Is.False, "Second move Not failed. " + context);
        }

        /// <summary>
        /// Just verifies a move to the middle of the second run.
        /// </summary>
        [Test]
        public void Test3Runs()
        {
            Run run0 = TestNodeRange.GetTestRun(0);
            Run run2 = TestNodeRange.GetTestRun(2);
            string testText = run0.Text;
            Assert.That("Text", Is.EqualTo(testText), "Unexpected test run text");
            Assert.That("Text", Is.EqualTo(run2.Text), "Unexpected test run text");

            Test3RunsBody(0, 1, "Text", "Text");
            Test3RunsBody(1, 1, "ext", "Text");
            Test3RunsBody(testText.Length - 1, 1, "t", "Text");
            // This one moves right to the "Text" in the second run on the first move.
            Test3RunsBody(testText.Length, 1, "Text", "T");
        }

        /// <summary>
        /// Relates to WORDSNET-27816.
        /// Tests NodeFinder work with an empty Body.
        /// </summary>
        [Test]
        public void TestNodeFinderWithEmptyBody()
        {
            Document doc = new Document(DocumentCtorMode.BlankDocumentNode);
            doc.FirstSection.Body.LastParagraph.Remove();
            NodeRange nodeRange = new NodeRange(doc.FirstSection, true, doc.FirstSection, true);

            // Checks NullReferenceException here.
            IList<Node> nodes = NodeFinder.FindNodes(nodeRange, NodeType.Paragraph);
            Assert.That(nodes.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Relates to WORDSNET-27816.
        /// Tests iteration over empty bodies.
        /// </summary>
        [Test]
        public void TestNodeEnumeratorOverEmptyBody()
        {
            Document doc = new Document(DocumentCtorMode.BlankDocumentNode);
            doc.FirstSection.Body.LastParagraph.Remove();
            doc.Sections.Add(doc.FirstSection.Clone(true));

            NodeRange nodeRange = new NodeRange(doc.FirstSection, true, doc.LastSection, true);
            NodeEnumerator nodeEnumerator = new NodeEnumerator(nodeRange);

            // Checks iteration over empty bodies.
            nodeEnumerator.MoveToNextNode();
            Assert.That(nodeEnumerator.CurrentNode, Is.EqualTo(doc.FirstSection));
            nodeEnumerator.MoveToNextNode();
            Assert.That(nodeEnumerator.CurrentNode, Is.EqualTo(doc.LastSection));

            // Checks whether iterations have been completed correctly without causing void DocumentPosition.
            Assert.That(nodeEnumerator.MoveToNextNode(), Is.False);
        }
    }
}
