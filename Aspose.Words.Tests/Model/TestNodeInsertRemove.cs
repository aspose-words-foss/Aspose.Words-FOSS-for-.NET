// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test that node inserting and removing works.
    /// </summary>
    [TestFixture]
    public class TestNodeInsertRemove
    {
        /// <summary>
        /// Test the fast method of appending children.
        /// </summary>
        [Test]
        public void TestAppendChildForLoad()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            //Appends when there are no children
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            //Appends when there are some children
            Node sect2 = doc.AppendChildForLoad(new Section(doc));

            //Check document knows children
            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));

            //Check sections know the document
            Assert.That(sect1.ParentNode, Is.EqualTo(doc));
            Assert.That(sect1.Document, Is.EqualTo(doc));
            Assert.That(sect2.ParentNode, Is.EqualTo(doc));
            Assert.That(sect2.Document, Is.EqualTo(doc));

            //Check sections know each other
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestAppendSingleChildForLoad()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            //Appends when there are no children
            Node sect1 = doc.AppendChildForLoad(new Section(doc));

            //Check document knows children
            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect1));

            //Check sections know the document
            Assert.That(sect1.ParentNode, Is.EqualTo(doc));
            Assert.That(sect1.Document, Is.EqualTo(doc));

            //Check sections know each other
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestAppendManyChildsForLoad()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            //Appends when there are no children
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            //Appends when there are some children
            Node sect2 = doc.AppendChildForLoad(new Section(doc));
            Node sect3 = doc.AppendChildForLoad(new Section(doc));

            //Check document knows children
            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));

            //Check sections know the document
            Assert.That(sect1.ParentNode, Is.EqualTo(doc));
            Assert.That(sect1.Document, Is.EqualTo(doc));

            //Check sections know each other
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));
        }

        /// <summary>
        /// Test insert after last child.
        /// </summary>
        [Test]
        public void TestAppendChild()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);

            //Appends when there are no children
            Node sect1 = doc.AppendChild(new Section(doc));

            //Check document is valid after insert
            Assert.That(doc.HasChildNodes, Is.EqualTo(true));
            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect1));

            //Check section is valid after insert
            Assert.That(sect1.ParentNode, Is.EqualTo(doc));
            Assert.That(sect1.Document, Is.EqualTo(doc));
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(null));

            //Appends when there are some children
            Node sect2 = doc.AppendChild(new Section(doc));

            //Check both sections siblings are working
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(null));

            //Appends third child
            Node sect3 = doc.AppendChild(new Section(doc));

            //Check sections siblings are working
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestAppendInvalidNodeType()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));
            sect.AppendChild(new Section(doc));
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot add a node to self.")]
        public void TestAppendSelf()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            doc.AppendChild(doc);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The newChild is an ancestor of this node.")]
        public void TestAppendAncestor()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));
            sect.AppendChild(doc);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "The newChild was created from a different document than the one that created this node.")]
        public void TestAppendFromDifferentDoc()
        {
            Document doc1 = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect = new Section(doc1);

            Document doc2 = new Document(DocumentCtorMode.EmptyDocumentNode);
            doc2.AppendChild(sect);
        }

        [Test]
        public void TestAppendEvents()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            AppendEventsContext context = new AppendEventsContext();
            context.Doc = doc;
            context.Doc.NodeChangingCallback = new HandleTestAppendEvents(context);

            context.InsertedNode = new Section(doc);
            context.Doc.AppendChild(context.InsertedNode);

            Assert.That(context.IsInsertingFired, Is.EqualTo(true));
            Assert.That(context.IsInsertedFired, Is.EqualTo(true));
        }

        private class AppendEventsContext
        {
            public bool IsInsertingFired { get; set; }
            public bool IsInsertedFired { get; set; }
            public Node InsertedNode { get; set; }
            [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
            public Document Doc { get; set; }
        }

        private class HandleTestAppendEvents : INodeChangingCallback
        {
            public HandleTestAppendEvents(AppendEventsContext context)
            {
                mContext = context;
            }

            void INodeChangingCallback.NodeInserting(NodeChangingArgs e)
            {
                Assert.That(e.Action, Is.EqualTo(NodeChangingAction.Insert));
                Assert.That(e.OldParent, Is.EqualTo(null));
                Assert.That(e.NewParent, Is.EqualTo(mContext.Doc));
                Assert.That(e.Node, Is.EqualTo(mContext.InsertedNode));
                mContext.IsInsertingFired = true;
            }

            void INodeChangingCallback.NodeInserted(NodeChangingArgs e)
            {
                Assert.That(e.Action, Is.EqualTo(NodeChangingAction.Insert));
                Assert.That(e.OldParent, Is.EqualTo(null));
                Assert.That(e.NewParent, Is.EqualTo(mContext.Doc));
                Assert.That(e.Node, Is.EqualTo(mContext.InsertedNode));
                mContext.IsInsertedFired = true;
            }

            void INodeChangingCallback.NodeRemoving(NodeChangingArgs e)
            {
                // Do nothing.
            }

            void INodeChangingCallback.NodeRemoved(NodeChangingArgs e)
            {
                // Do nothing.
            }

            private AppendEventsContext mContext;
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "This node is not a parent of the oldChild node.")]
        public void TestRemoveNonChild()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            doc.RemoveChild(doc);
        }

        [Test]
        public void TestRemoveSingleChild()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect = doc.AppendChild(new Section(doc));

            Assert.That(doc.RemoveChild(sect), Is.EqualTo(sect));

            //Check the document no longer has the child
            Assert.That(doc.FirstChild, Is.EqualTo(null));
            Assert.That(doc.LastChild, Is.EqualTo(null));

            //Check the removed element is detached
            Assert.That(sect.ParentNode, Is.EqualTo(null));
            Assert.That(sect.Document, Is.EqualTo(doc));
        }

        [Test]
        public void TestRemoveLastChild()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChild(new Section(doc));
            Node sect2 = doc.AppendChild(new Section(doc));

            doc.RemoveChild(sect2);

            //Check the parent
            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect1));

            //Check the remaining element
            Assert.That(sect1.NextSibling, Is.EqualTo(null));
            //Single element is looped to self in the list.
            Assert.That(sect1.NextNode, Is.EqualTo(null));
            Assert.That(sect1.PrevNode, Is.EqualTo(null));

            //Check the deleted element
            Assert.That(sect2.NextSibling, Is.EqualTo(null));
            //But a single element that is not in a tree is not a list.
            Assert.That(sect2.NextNode, Is.EqualTo(null));
            Assert.That(sect2.PrevNode, Is.EqualTo(null));
        }

        [Test]
        public void TestRemoveFirstChild()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChild(new Section(doc));
            Node sect2 = doc.AppendChild(new Section(doc));

            doc.RemoveChild(sect1);

            //Check the parent
            Assert.That(doc.FirstChild, Is.EqualTo(sect2));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));

            //Check the remaining element
            Assert.That(sect2.NextSibling, Is.EqualTo(null));
            Assert.That(sect2.NextNode, Is.EqualTo(null));
            Assert.That(sect2.PrevNode, Is.EqualTo(null));

            //Check the deleted element
            Assert.That(sect1.NextSibling, Is.EqualTo(null));
            Assert.That(sect1.NextNode, Is.EqualTo(null));
            Assert.That(sect1.PrevNode, Is.EqualTo(null));
        }

        [Test]
        public void TestRemoveMiddleChild()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChild(new Section(doc));
            Node sect2 = doc.AppendChild(new Section(doc));
            Node sect3 = doc.AppendChild(new Section(doc));

            doc.RemoveChild(sect2);

            //Check the remaining elements
            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));
        }

        /// <summary>
        /// WORDSNET-7087 Null reference exception occurs when adding a new run to paragraph.
        /// Added appropriate exception to CompositeNode.Insert.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestInsertSameNodeTwice()
        {
            Document doc = new Document();
            // Get the paragraph from the document, we will be adding runs of text to it.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);

            // Add a run of normal text.
            Run run = new Run(doc, "Normal text");
            para.AppendChild(run);
            para.AppendChild(run); // Exception occurs here
        }

        [Test]
        public void TestInsertAfterInMiddle()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            Node sect2 = doc.AppendChildForLoad(new Section(doc));
            Node sect4 = doc.AppendChildForLoad(new Section(doc));

            Node sect3 = new Section(doc);
            Assert.That(doc.InsertAfter(sect3, sect2), Is.EqualTo(sect3));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(sect4));
            Assert.That(sect4.PreviousSibling, Is.EqualTo(sect3));
            Assert.That(sect4.NextSibling, Is.EqualTo(null));

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect4));
        }

        [Test]
        public void TestInsertAfterFirst()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            Node sect3 = doc.AppendChildForLoad(new Section(doc));

            Node sect2 = new Section(doc);
            Assert.That(doc.InsertAfter(sect2, sect1), Is.EqualTo(sect2));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));
        }

        [Test]
        public void TestInsertAfterLast()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            Node sect2 = doc.AppendChildForLoad(new Section(doc));

            Node sect3 = new Section(doc);
            Assert.That(doc.InsertAfter(sect3, sect2), Is.EqualTo(sect3));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));
        }

        [Test]
        public void TestInsertAfterSingle()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChildForLoad(new Section(doc));

            Node sect2 = new Section(doc);
            Assert.That(doc.InsertAfter(sect2, sect1), Is.EqualTo(sect2));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(null));

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));
        }

        [Test]
        public void TestInsertAfterNull()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect2 = doc.InsertAfter(new Section(doc), null);

            //Inserting after null actually places the child in front of the list.
            Node sect1 = doc.InsertAfter(new Section(doc), null);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));

            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestInsertBefore()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect3 = doc.AppendChildForLoad(new Section(doc));

            //This tests insert before first child
            Node sect1 = doc.InsertBefore(new Section(doc), sect3);
            //This tests insert before some other child
            Node sect2 = doc.InsertBefore(new Section(doc), sect3);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));

            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestInsertBeforeSingle()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect2 = doc.AppendChildForLoad(new Section(doc));

            // Single node is looped to null.
            Assert.That(doc.FirstChild, Is.EqualTo(sect2));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));
            Assert.That(sect2.PrevNode, Is.EqualTo(null));
            Assert.That(sect2.NextNode, Is.EqualTo(null));

            // This tests insert before single child.
            Node sect1 = doc.InsertBefore(new Section(doc), sect2);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestInsertBeforeFirst()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect2 = doc.AppendChildForLoad(new Section(doc));
            Node sect3 = doc.AppendChildForLoad(new Section(doc));

            Assert.That(doc.FirstChild, Is.EqualTo(sect2));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));

            Node sect1 = doc.InsertBefore(new Section(doc), sect2);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestInsertBeforeLast()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            Node sect3 = doc.AppendChildForLoad(new Section(doc));

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));

            Node sect2 = doc.InsertBefore(new Section(doc), sect3);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect3));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestInsertBeforeInMiddle()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChildForLoad(new Section(doc));
            Node sect2 = doc.AppendChildForLoad(new Section(doc));
            Node sect4 = doc.AppendChildForLoad(new Section(doc));

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect4));

            Node sect3 = doc.InsertBefore(new Section(doc), sect4);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect4));

            Assert.That(sect1.PreviousSibling, Is.EqualTo(null));
            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.PreviousSibling, Is.EqualTo(sect1));
            Assert.That(sect2.NextSibling, Is.EqualTo(sect3));
            Assert.That(sect3.PreviousSibling, Is.EqualTo(sect2));
            Assert.That(sect3.NextSibling, Is.EqualTo(sect4));
            Assert.That(sect4.PreviousSibling, Is.EqualTo(sect3));
            Assert.That(sect4.NextSibling, Is.EqualTo(null));
        }

        [Test]
        public void TestInsertBeforeNull()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.InsertBefore(new Section(doc), null);

            //Inserting before null actually places the child at the end.
            Node sect2 = doc.InsertBefore(new Section(doc), null);

            Assert.That(doc.FirstChild, Is.EqualTo(sect1));
            Assert.That(doc.LastChild, Is.EqualTo(sect2));

            Assert.That(sect1.NextSibling, Is.EqualTo(sect2));
            Assert.That(sect2.NextSibling, Is.EqualTo(null));
        }


    }
}
