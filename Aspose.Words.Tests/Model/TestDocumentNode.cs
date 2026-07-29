// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Document node specific tests.
    /// </summary>
    [TestFixture]
    public class TestDocumentNode
    {
        [Test]
        public void TestImportNode()
        {
            Document doc1 = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect1 = doc1.AppendChild(new Section(doc1));
            Body body1 = sect1.AppendChild(new Body(doc1));

            Document doc2 = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect2 = (Section)doc2.ImportNode(sect1, true);

            //It is a clone, not the same section now.
            Assert.That(sect1 != sect2, Is.True);

            //When the node is imported, it is not yet part of the tree, but the owner document is set.
            Assert.That(sect2.ParentNode, Is.EqualTo(null));
            Assert.That(sect2.Document, Is.EqualTo(doc2));

            //Check the child of the section
            Node body2 = sect2.FirstChild;
            Assert.That(body2, IsNot.Null());
            Assert.That(body1 != body2, Is.True);
            Assert.That(body2.Document, Is.EqualTo(doc2));

            //Now can successfully append the imported node.
            doc2.AppendChild(sect2);

            Assert.That(sect2.ParentNode, Is.EqualTo(doc2));
        }

        [Test]
        public void TestCloneDocument()
        {
            Document doc1 = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc1.AppendChild(new Section(doc1));
            Body body = new Body(doc1);
            sect.AppendChild(body);

            Document doc2 = (Document)doc1.Clone(true);
            Assert.That(doc2.ParentNode, Is.EqualTo(null));
            Assert.That(doc2.Document, Is.EqualTo(doc2));
            Assert.That(doc2.FirstChild.NodeType, Is.EqualTo(NodeType.Section));
        }
    }
}
