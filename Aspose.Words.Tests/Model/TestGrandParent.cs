// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// I perform some operations where the node should have same grand parent,
    /// this covers most of situations with fields and bookmarks for me.
    /// </summary>
    [TestFixture]
    public class TestGrandParent
    {
        /// <summary>
        /// CompositeNode.GetTextFromSequence() was superseded by <see cref="NodeTextCollector.GetText()"/>.
        /// </summary>
        [Test]
        public void TestGetTextFromSequence()
        {
            Document doc = CreateTestDoc();

            NodeList runs = doc.SelectNodes("//Run");

            // Get text inside one parent
            // End node is excluded, therefore nothing.
            Assert.That(NodeTextCollector.GetText(runs[0], true, runs[0], false), Is.EqualTo(""));
            Assert.That(NodeTextCollector.GetText(runs[0], true, runs[2], false), Is.EqualTo("[0][1]"));

            // Get text across several parents
            Assert.That(NodeTextCollector.GetText(runs[0], true, runs[3], false), Is.EqualTo("[0][1][2]\r"));
            Assert.That(NodeTextCollector.GetText(runs[0], true, runs[4], false), Is.EqualTo("[0][1][2]\r[3]"));
            Assert.That(NodeTextCollector.GetText(runs[0], true, runs[8], false), Is.EqualTo("[0][1][2]\r[3][4][5]\r[6][7]"));
        }

        [Test]
        public void TestRemoveWithCollapseInOneParent()
        {
            Document doc = CreateTestDoc();
            NodeList runs = doc.SelectNodes("//Run");

            // Remove one run in the second paragraph, together with the bookmark.
            NodeRemover.Remove(runs[3], true, runs[4], false);            
            Assert.That(doc.SelectSingleNode("//Paragraph[2]").GetText(), Is.EqualTo("[4][5]\r"));
            Assert.That(doc.SelectNodes("//BookmarkStart").Count, Is.EqualTo(1));
        }

        [Test]
        public void TestRemoveWithCollapseInTwoParents()
        {
            Document doc = CreateTestDoc();
            NodeList runs = doc.SelectNodes("//Run");

            //This delete merge first and second paragraphs.
            NodeRemover.Remove(runs[0], true, runs[4], false);            
            Assert.That(doc.SelectNodes("//Paragraph").Count, Is.EqualTo(2));
            Assert.That(doc.SelectSingleNode("//Paragraph[1]").GetText(), Is.EqualTo("[4][5]\r"));
            Assert.That(doc.SelectSingleNode("//Paragraph[2]").GetText(), Is.EqualTo("[6][7][8]\x000c"));
            //The bookmark between the runs was deleted.
            Assert.That(doc.SelectNodes("//BookmarkStart").Count, Is.EqualTo(1));
        }

        [Test]
        public void TestRemoveWithCollapseInThreeParents()
        {
            Document doc = CreateTestDoc();
            NodeList runs = doc.SelectNodes("//Run");

            //This will delete merge first and second paragraphs.
            NodeRemover.Remove(runs[1], true, runs[8], false);            
            Assert.That(doc.SelectNodes("//Paragraph").Count, Is.EqualTo(1));
            Assert.That(doc.SelectSingleNode("//Paragraph[1]").GetText(), Is.EqualTo("[0][8]\x000c"));
            //All bookmarks including the one between paragraphs were deleted.
            Assert.That(doc.SelectNodes("//BookmarkStart").Count, Is.EqualTo(0));
        }

        [Test]
        public void TestRemoveToEndOfAnotherParent()
        {
            Document doc = CreateTestDoc();
            NodeList runs = doc.SelectNodes("//Run");

            //Delete some from the first paragraph and till the end of the second paragraph.
            NodeRange range = new NodeRange(runs[1], true, runs[5].ParentNode, true);
            range.Remove();            
            
            Assert.That(doc.SelectSingleNode("//Paragraph[1]").GetText(), Is.EqualTo("[0]\r"));
            Assert.That(doc.SelectSingleNode("//Paragraph[2]").GetText(), Is.EqualTo("[6][7][8]\x000c"));
        }

        /// <summary>
        /// Creates the following doc
        /// Section
        ///        Body
        ///            Para 0
        ///                Run 0
        ///                Run 1
        ///                Run 2
        ///            Para 1
        ///                Run 3
        ///                BookmarkStart
        ///                Run 4
        ///                Run 5
        ///            Para 2
        ///                BookmarkStart
        ///                Run 6
        ///                Run 7
        ///                Run 8
        /// </summary>
        /// <returns></returns>
        private Document CreateTestDoc()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);

            Section sect = doc.AppendChild(new Section(doc));
            Body body = sect.AppendChild(new Body(doc));

            Paragraph p1 = body.AppendChild(new Paragraph(doc));
            p1.AppendChild(new Run(doc, "[0]"));
            p1.AppendChild(new Run(doc, "[1]"));
            p1.AppendChild(new Run(doc, "[2]"));

            Paragraph p2 = body.AppendChild(new Paragraph(doc));
            p2.AppendChild(new Run(doc, "[3]"));
            p2.AppendChild(new BookmarkStart(doc, "bmk1"));
            p2.AppendChild(new Run(doc, "[4]"));
            p2.AppendChild(new Run(doc, "[5]"));

            Paragraph p3 = body.AppendChild(new Paragraph(doc));
            p3.AppendChild(new BookmarkStart(doc, "bmk2"));
            p3.AppendChild(new Run(doc, "[6]"));
            p3.AppendChild(new Run(doc, "[7]"));
            p3.AppendChild(new Run(doc, "[8]"));

            return doc;
        }
    }
}
