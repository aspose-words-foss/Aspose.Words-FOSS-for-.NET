// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test that node lists and enumeration works.
    /// </summary>
    [TestFixture]
    public class TestNodeCollection
    {
        [Test]
        public void TestGetChildNode()
        {
            Document doc = CreateTestDoc();

            Paragraph para = (Paragraph)doc.FirstSection.Body.GetChildNodes(NodeType.Any, false)[0];

            //Test access from the start
            Assert.That(para.GetChild(NodeType.Run, 0, false).GetText(), Is.EqualTo("Body1"));
            Assert.That(para.GetChild(NodeType.Run, 1, false).GetText(), Is.EqualTo("Body2"));
            Assert.That(para.GetChild(NodeType.Run, 2, false), Is.EqualTo(null));

            //Test access from the end
            Assert.That(para.GetChild(NodeType.Run, -1, false).GetText(), Is.EqualTo("Body2"));
            Assert.That(para.GetChild(NodeType.Run, -2, false).GetText(), Is.EqualTo("Body1"));
            Assert.That(para.GetChild(NodeType.Run, -3, false), Is.EqualTo(null));
        }

        /// <summary>
        /// Tests the flat child nodes collection.
        /// </summary>
        [Test]
        public void TestFlatNodes()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChild(new Section(doc));
            Node sect2 = doc.AppendChild(new Section(doc));

            NodeCollection nodes = doc.GetChildNodes(NodeType.Any, false);
            Assert.That(nodes.Count, Is.EqualTo(2));
            Assert.That(nodes[0], Is.EqualTo(sect1));
            Assert.That(nodes[1], Is.EqualTo(sect2));

            //Also test access from the back.
            Assert.That(nodes[-1], Is.EqualTo(sect2));
            Assert.That(nodes[-2], Is.EqualTo(sect1));

            //It is a live collection, check it is updated when the doc is modified.
            Node sect0 = doc.PrependChild(new Section(doc));
            Assert.That(nodes.Count, Is.EqualTo(3));
            Assert.That(nodes[0], Is.EqualTo(sect0));
            Assert.That(nodes[1], Is.EqualTo(sect1));
            Assert.That(nodes[2], Is.EqualTo(sect2));

            //Also test access from the back.
            Assert.That(nodes[-1], Is.EqualTo(sect2));
            Assert.That(nodes[-2], Is.EqualTo(sect1));
            Assert.That(nodes[-3], Is.EqualTo(sect0));
        }

        /// <summary>
        /// Out of range access to flat NodeCollection returns nulls.
        /// </summary>
        [Test]
        public void TestFlatNodesOutOfRange()
        {
            Document doc = CreateTestDoc();
            
            NodeCollection sections = doc.GetChildNodes(NodeType.Any, false);
            Assert.That(sections.Count, Is.EqualTo(1));
            Assert.That(sections[-2], Is.Null);    //-1 means the last element, so have to go one further
            Assert.That(sections[0], IsNot.Null());
            Assert.That(sections[1], Is.Null);

            doc.RemoveAllChildren();
            Assert.That(sections.Count, Is.EqualTo(0));
            Assert.That(sections[0], Is.Null);
            Assert.That(sections[-1], Is.Null);
        }

        [Test]
        public void TestFlatNodeEnumerator()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChild(new Section(doc));
            Node sect2 = doc.AppendChild(new Section(doc));
            Node sect3 = doc.AppendChild(new Section(doc));

            // I have to work with the explicit enumerator type directly here because I am testing IEnumerator methods,
            // but in Java it is autoported as Iterator and the methods are not available on an Iterator.
            NodeCollectionEnumerator<Node> e = (NodeCollectionEnumerator<Node>)doc.GetChildNodes(NodeType.Any, false).GetEnumerator();

            //Run over an unmodified collection
            e.Reset();
            Assert.That(e.MoveNext(), Is.EqualTo(true));
            Assert.That(e.Current, Is.EqualTo(sect1));
            Assert.That(e.MoveNext(), Is.EqualTo(true));
            Assert.That(e.Current, Is.EqualTo(sect2));
            Assert.That(e.MoveNext(), Is.EqualTo(true));
            Assert.That(e.Current, Is.EqualTo(sect3));
            Assert.That(e.MoveNext(), Is.EqualTo(false));

            //Run over a modified collection
            e.Reset();
            Assert.That(e.MoveNext(), Is.EqualTo(true));
            Assert.That(e.Current, Is.EqualTo(sect1));
            doc.RemoveChild(sect2);
            Assert.That(e.MoveNext(), Is.EqualTo(true));
            Assert.That(e.Current, Is.EqualTo(sect3));
            Assert.That(e.MoveNext(), Is.EqualTo(false));
        }

        /// <summary>
        /// Check that node collection enumerators do not share any state and can work independently.
        /// </summary>
        [Test]
        public void TestTwoNodeEnumerators()
        {
            // Check both flat and deep collections.
            CheckTwoNodeEnumerators(false);
            CheckTwoNodeEnumerators(true);
        }

        /// <summary>
        /// Check that can retrieve nodes by type, that they appear in the correct order 
        /// and the live collection is updated when more nodes are inserted.
        /// </summary>
        [Test]
        public void TestDeepNodes()
        {
            Document doc = CreateTestDoc();

            //This is a live collection of all runs in the document.
            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);

            //Test before modifying the document.
            Assert.That(runs.Count, Is.EqualTo(3));
            Assert.That(runs[0].GetText(), Is.EqualTo("Body1"));
            Assert.That(runs[1].GetText(), Is.EqualTo("Body2"));
            Assert.That(runs[2].GetText(), Is.EqualTo("Header1"));

            //Also test access from the back of the collection
            Assert.That(runs[-1].GetText(), Is.EqualTo("Header1"));
            Assert.That(runs[-2].GetText(), Is.EqualTo("Body2"));
            Assert.That(runs[-3].GetText(), Is.EqualTo("Body1"));

            //Create one more paragraph and run
            Body body = doc.FirstSection.Body;
            Assert.That(NodeType.Body, Is.EqualTo(body.NodeType));
            Paragraph para = body.AppendChild(new Paragraph(doc));
            para.AppendChild(new Run(doc, "Body3"));

            //Test the collection was updated
            Assert.That(runs.Count, Is.EqualTo(4));
            Assert.That(runs[0].GetText(), Is.EqualTo("Body1"));
            Assert.That(runs[1].GetText(), Is.EqualTo("Body2"));
            Assert.That(runs[2].GetText(), Is.EqualTo("Body3"));
            Assert.That(runs[3].GetText(), Is.EqualTo("Header1"));

            //Also test access from the back of the collection
            Assert.That(runs[-1].GetText(), Is.EqualTo("Header1"));
            Assert.That(runs[-2].GetText(), Is.EqualTo("Body3"));
            Assert.That(runs[-3].GetText(), Is.EqualTo("Body2"));
            Assert.That(runs[-4].GetText(), Is.EqualTo("Body1"));
        }

        /// <summary>
        /// Out of range access to deep NodeCollection returns nulls.
        /// </summary>
        [Test]
        public void TestDeepNodesOutOfRange()
        {
            Document doc = CreateTestDoc();
            
            NodeCollection sections = doc.GetChildNodes(NodeType.Section, true);
            Assert.That(sections.Count, Is.EqualTo(1));
            Assert.That(sections[-2], Is.Null);    //-1 means the last element, so have to go one further
            Assert.That(sections[0], IsNot.Null());
            Assert.That(sections[1], Is.Null);

            doc.RemoveAllChildren();
            Assert.That(sections.Count, Is.EqualTo(0));
            Assert.That(sections[0], Is.Null);
            Assert.That(sections[-1], Is.Null);    
        }

        [Test]
        public void TestDeepNodesEnumerator()
        {
            Document doc = CreateTestDoc();

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            IEnumerator<Node> e = runs.GetEnumerator();

            //Test iteration over existing elements works.
            e.MoveNext();
            Assert.That(((Node)e.Current).GetText(), Is.EqualTo("Body1"));
            e.MoveNext();
            Assert.That(((Node)e.Current).GetText(), Is.EqualTo("Body2"));

            //Insert a node that will not invalidate iteration.
            doc.LastSection.Body.PrependChild(new Paragraph(doc));

            //Test iteration can continue after inserting a node of different type.
            e.MoveNext();
            Assert.That(((Node)e.Current).GetText(), Is.EqualTo("Header1"));
            e.MoveNext();
        }

        /// <summary>
        /// Test forward and backward preorder iteration over the tree.
        /// </summary>
        [Test]
        public void TestPreOrderTraversal()
        {
            Document doc = CreateTestDoc();

            //Try iterating over all nodes of a node
            Section section = doc.FirstSection;
            NodeCollection all = section.GetChildNodes(NodeType.Any, true);

            Assert.That(all.Count, Is.EqualTo(9));

            //This will excercise forward preforder
            Assert.That(all[0].NodeType, Is.EqualTo(NodeType.Body));
            Assert.That(all[1].NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(all[2].NodeType, Is.EqualTo(NodeType.BookmarkStart));
            Assert.That(all[3].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(all[4].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(all[5].NodeType, Is.EqualTo(NodeType.Paragraph));

            //This will excercise backward preorder
            Assert.That(all[8].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(all[7].NodeType, Is.EqualTo(NodeType.Paragraph));
            Assert.That(all[6].NodeType, Is.EqualTo(NodeType.HeaderFooter));
        }

        /// <summary>
        /// Tests that XPathNodeCollection works.
        /// </summary>
        [Test]
        public void TestXPathNodeCollection()
        {
            Document doc = CreateTestDoc();
            NodeList runs = doc.SelectNodes("//Run");
            Assert.That(runs.Count, Is.EqualTo(3));

            //Test forward indexing
            Assert.That(runs[0].GetText(), Is.EqualTo("Body1"));
            Assert.That(runs[1].GetText(), Is.EqualTo("Body2"));
            Assert.That(runs[2].GetText(), Is.EqualTo("Header1"));
            Assert.That(runs[3], Is.EqualTo(null));    //Out of range

            //Test backward indexing
            Assert.That(runs[-1].GetText(), Is.EqualTo("Header1"));
            Assert.That(runs[-2].GetText(), Is.EqualTo("Body2"));
            Assert.That(runs[-3].GetText(), Is.EqualTo("Body1"));
            Assert.That(runs[-4], Is.EqualTo(null));    //Out of range
        }

        [Test]
        public void TestXPathNodeCollectionFromNode()
        {
            Document doc = CreateTestDoc();
            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");

            //Select not from document, but from 
            NodeList runs = para.SelectNodes("Run");
            Assert.That(runs.Count, Is.EqualTo(2));
            Assert.That(runs[0].GetText(), Is.EqualTo("Body1"));
            Assert.That(runs[1].GetText(), Is.EqualTo("Body2"));
        }

        /// <summary>
        /// Test that can enumerate over XPathNodeCollection.
        /// </summary>
        [Test]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("XPath is not supported in C++ yet.")]
        public void TestXPathNodeEnumerator()
        {
            Document doc = CreateTestDoc();
            NodeList runs = doc.SelectNodes("//Run");
            EnumeratorWrapperPalGeneric<Node> e = new EnumeratorWrapperPalGeneric<Node>(runs.GetEnumerator());
            
            Assert.That(e.Current, Is.EqualTo(null));
            e.MoveNext();
            Assert.That(((Run)e.Current).GetText(), Is.EqualTo("Body1"));
            e.MoveNext();
            Assert.That(((Run)e.Current).GetText(), Is.EqualTo("Body2"));
            e.MoveNext();
            Assert.That(((Run)e.Current).GetText(), Is.EqualTo("Header1"));
            e.MoveNext();
            Assert.That(e.Current, Is.EqualTo(null));
        }

        /// <summary>
        /// Just test some XPath paths to see if it works.
        /// </summary>
        [Test]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("XPath is not supported in C++ yet.")]
        public void TestLastRunInFirstSection()
        {
            Document doc = CreateTestDoc();
            Node run = doc.SelectSingleNode("Section[1]/Body/Paragraph/Run[last()]");
            Assert.That(run.GetText(), Is.EqualTo("Body2"));

            //Just test it can also return nulls
            run = doc.SelectSingleNode("Section[1]/Body/Paragraph/Run[3]");
            Assert.That(run, Is.EqualTo(null));
        }

        [Test]
        public void TestToArray()
        {
            Document doc = CreateTestDoc();

            NodeCollection paras = doc.GetChildNodes(NodeType.Paragraph, true);
            Assert.That(paras.Count, Is.EqualTo(3));
            
            // Check fixed size node array was created.
            Node[] nodes = paras.ToArray();
            Assert.That(nodes.Length, Is.EqualTo(3));
            Assert.That(paras[0], Is.EqualTo(nodes[0]));
            Assert.That(paras[1], Is.EqualTo(nodes[1]));

            // It is now easy to add/remove nodes while iterating over the fixed size array.
            nodes[0].Remove();
            // See how live collection is updated at the same time.
            Assert.That(paras.Count, Is.EqualTo(2));
        }


        /// <summary>
        /// WORDSNET-6825 Check removing the current node inside 'foreach' over NodeCollection.
        /// </summary>
        [Test]
        public void TestRemoveCurNodeInsideForeach()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Writeln("The first paragraph");
            builder.Writeln("The second paragraph");
            builder.Writeln("The third paragraph");

            foreach (Paragraph para in builder.Document.FirstSection.Body.Paragraphs)
                if (StringUtil.Contains(para.ToString(SaveFormat.Text), "second", false))
                    para.Remove();

            Assert.That(builder.Document.ToString(SaveFormat.Text).Trim(), Is.EqualTo("The first paragraph\r\nThe third paragraph"));
        }

        /// <summary>
        /// WORDSNET-6825 Check removing the previous node inside 'foreach' over NodeCollection.
        /// </summary>
        [Test]
        public void TestRemovePrevNodeInsideForeach()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Writeln("The first paragraph");
            builder.Writeln("The second paragraph");
            builder.Writeln("The third paragraph");

            foreach (Paragraph para in builder.Document.FirstSection.Body.Paragraphs)
                if (StringUtil.Contains(para.ToString(SaveFormat.Text), "third", false))
                    para.PreviousSibling.Remove();

            Assert.That(builder.Document.ToString(SaveFormat.Text).Trim(), Is.EqualTo("The first paragraph\r\nThe third paragraph"));
        }

        /// <summary>
        /// WORDSNET-6825 Check removing the next node inside 'foreach' over NodeCollection.
        /// </summary>
        [Test]
        public void TestRemoveNextNodeInsideForeach()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Writeln("The first paragraph");
            builder.Writeln("The second paragraph");
            builder.Writeln("The third paragraph");

            foreach (Paragraph para in builder.Document.FirstSection.Body.Paragraphs)
                if (StringUtil.Contains(para.ToString(SaveFormat.Text), "first", false))
                    para.NextSibling.Remove();

            Assert.That(builder.Document.ToString(SaveFormat.Text).Trim(), Is.EqualTo("The first paragraph\r\nThe third paragraph"));
        }

        /// <summary>
        /// WORDSNET-6825 Check direct access to methods of <see cref="NodeCollectionEnumerator{T}"/>.
        /// </summary>
        [JavaAttributes.JavaGenericArguments("Iterator<Paragraph>")]
        [Test]
        public void TestReadFromEnumerator()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Writeln("The first paragraph");
            builder.Write("The second paragraph");

            IEnumerator<Node> e = builder.Document.FirstSection.Body.Paragraphs.GetEnumerator();

            //In .Net MoveNext() moves cursor, Current - just returns current.
            //In Java MoveNext (hasNext()) - just returns boolean, Current (next()) - moves cursor and returns current.
#if PLAIN_JAVA
            boolean isSuccess = e.hasNext();
            Node next = e.next();
            msAssert.areEqual(true, isSuccess);
            
            isSuccess = e.hasNext();
            next = e.next();
            msAssert.areEqual(true, isSuccess);
            
            isSuccess = e.hasNext();
            msAssert.areEqual(false, isSuccess);
#else
            bool isSuccess = e.MoveNext();
            Assert.That(isSuccess, Is.EqualTo(true));

            isSuccess = e.MoveNext();
            Assert.That(isSuccess, Is.EqualTo(true));

            isSuccess = e.MoveNext();
            Assert.That(isSuccess, Is.EqualTo(false));
#endif
        }

        /// <summary>
        /// WORDSNET-6825 Check reading beyond the end of an enumerator.
        /// </summary>
        [JavaAttributes.JavaGenericArguments("Iterator<Paragraph>")]
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestReadBeyondEnumeratorEnd()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.Writeln("The first paragraph");
            builder.Write("The second paragraph");

            IEnumerator<Node> e = builder.Document.FirstSection.Body.Paragraphs.GetEnumerator();

            //In java MoveNext (hasNext) doesn't move cursor. Current (next() in java) does move cursor.
#if PLAIN_JAVA
            e.next();
            e.next();
            e.next();
#else
            e.MoveNext();
            e.MoveNext();
            e.MoveNext();
#endif
            object beyoundCollectionEnd = e.Current;
            Assert.That(beyoundCollectionEnd, IsNot.Null());
        }

        /// <summary>
        /// Create the following test document:
        /// 
        /// doc
        ///        section
        ///            body
        ///                para
        ///                    bookmark
        ///                    run "Body1"
        ///                    run "Body2"
        ///                para
        ///            header
        ///                para
        ///                    run    "Header1"
        /// </summary>
        private static Document CreateTestDoc()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));

            Body body = sect.AppendChild(new Body(doc));
            Paragraph para1 = body.AppendChild(new Paragraph(doc));
            para1.AppendChild(new BookmarkStart(doc, "xxx"));
            para1.AppendChild(new Run(doc, "Body1"));
            para1.AppendChild(new Run(doc, "Body2"));

            body.AppendChild(new Paragraph(doc));
            
            HeaderFooter header = sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.HeaderPrimary));
            Paragraph para3 = header.AppendChild(new Paragraph(doc));
            para3.AppendChild(new Run(doc, "Header1"));

            //Check that we've built what we want: basically some runs in different paragraphs and in different stories.
            const string Result = "Body1Body2\r\x000cHeader1\r";
            Assert.That(doc.GetText(), Is.EqualTo(Result));

            return doc;
        }

        private static void CheckTwoNodeEnumerators(bool isDeep)
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Node sect1 = doc.AppendChild(new Section(doc));
            Node sect2 = doc.AppendChild(new Section(doc));
            Node sect3 = doc.AppendChild(new Section(doc));

            NodeCollection nodes = doc.GetChildNodes(NodeType.Any, isDeep);
            IEnumerator<Node> e1 = nodes.GetEnumerator();
            IEnumerator<Node> e2 = nodes.GetEnumerator();

            e1.MoveNext();
            Assert.That(e1.Current, Is.EqualTo(sect1));

            e2.MoveNext();
            Assert.That(e2.Current, Is.EqualTo(sect1));

            e1.MoveNext();
            Assert.That(e1.Current, Is.EqualTo(sect2));
            e1.MoveNext();
            Assert.That(e1.Current, Is.EqualTo(sect3));

            // This "advances" the inner state inside the collection. Just to make sure the state of the collection
            // does not affect the work of iterators.
            Assert.That(nodes[0], Is.EqualTo(sect1));

            e2.MoveNext();
            Assert.That(e2.Current, Is.EqualTo(sect2));
            e2.MoveNext();
            Assert.That(e2.Current, Is.EqualTo(sect3));
        }
    }
}
