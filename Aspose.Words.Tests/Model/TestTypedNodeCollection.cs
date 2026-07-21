// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test that typed wrappers for nodes work.
    /// </summary>
    [TestFixture]
    public class TestTypedNodeCollection
    {
        [Test]
        public void TestAdd()
        {
            Document doc = CreateTestDoc();
            Assert.That(doc.GetText(), Is.EqualTo("1\r2\r3\x0007\x00074\x000c"));
        }

        [Test]
        public void TestInsertAtStart()
        {
            Document doc = CreateTestDoc();
            Body body = doc.FirstSection.Body;

            //Insert at the beginning
            Paragraph para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "0"));
            body.Paragraphs.Insert(0, para);

            Assert.That(doc.GetText(), Is.EqualTo("0\r1\r2\r3\x0007\x00074\x000c"));
        }

        [Test]
        public void TestInsertInMiddle()
        {
            Document doc = CreateTestDoc();
            Body body = doc.FirstSection.Body;

            //Insert in the middle and after table
            Paragraph para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "xxx"));
            body.Paragraphs.Insert(2, para);

            Assert.That(doc.GetText(), Is.EqualTo("1\r2\r3\x0007\x0007xxx\r4\x000c"));
        }

        [Test]
        public void TestInsertAtEnd()
        {
            Document doc = CreateTestDoc();
            Body body = doc.FirstSection.Body;
            
            Paragraph para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "5"));
            body.Paragraphs.Insert(3, para);

            Assert.That(doc.GetText(), Is.EqualTo("1\r2\r3\x0007\x00074\r5\x000c"));
        }

        [Test]
        public void TestInsertOutOfRange()
        {
            Document doc = CreateTestDoc();
            Body body = doc.FirstSection.Body;
            
            //Negative indexes are allowed, counted from the end. More than count - means adding to the end.
            Paragraph para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "xxx"));
            body.Paragraphs.Insert(-9, para);

            //If index is more than count, add to the end.
            para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "yyy"));
            body.Paragraphs.Insert(9, para);

            Assert.That(doc.GetText(), Is.EqualTo("1\r2\r3\x0007\x00074\rxxx\ryyy\x000c"));
        }

        [Test]
        public void TestClear()
        {
            Document doc = CreateTestDoc();
            Body body = doc.FirstSection.Body;

            body.Paragraphs.Clear();

            //Insert at zero into an empty collection means InsertBefore null and appends to the end.
            Paragraph para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "xxx"));
            body.Paragraphs.Insert(0, para);    

            Assert.That(doc.GetText(), Is.EqualTo("3\x0007\x0007xxx\x000c"));
        }


        /// <summary>
        /// Creates the following document
        ///        Document
        ///            Section
        ///                Body
        ///                    Paragraph
        ///                        Run("1")
        ///                    Paragraph
        ///                        Run("2")
        ///                    Table
        ///                        Row
        ///                            Cell
        ///                                Paragraph
        ///                                    Run("3")
        ///                    Paragraph
        ///                        Run("4")
        /// </summary>
        private Document CreateTestDoc()
        {
            Document doc = new Document();

            Body body = doc.FirstSection.Body;
            Assert.That(body.Paragraphs.Count, Is.EqualTo(1));
            body.FirstParagraph.Runs.Add(new Run(doc, "1"));

            Paragraph para = new Paragraph(doc);
            para.Runs.Add(new Run(doc, "2"));
            body.Paragraphs.Add(para);
            Assert.That(body.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(body.Tables.Count, Is.EqualTo(0));

            Table table = new Table(doc);
            table.EnsureMinimum();
            table.FirstRow.FirstCell.FirstParagraph.Runs.Add(new Run(doc, "3"));
            //This is somewhat controversial (adding Table to Paragraphs), but allows to add to the children anyway.
            body.Paragraphs.Add(table);
            Assert.That(body.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(body.Tables.Count, Is.EqualTo(1));

            //TODO 2 Must ensure that section ends with a paragraph manually, make it automatic.
            para = new Paragraph(doc);
            body.Paragraphs.Add(para);
            para.Runs.Add(new Run(doc, "4"));
            
            return doc;
        }
    }
}
