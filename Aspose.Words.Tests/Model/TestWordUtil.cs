// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestWordUtil
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        [AndroidDelete("Performance issues")]
        public void ParagraphCanBeRemovedTest()
        {
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase1());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase2());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase3());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase4());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase5());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase6());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase7());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase8());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase9());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase10());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase11());
            ParagraphCanBeRemovedTest(ParagraphCanBeRemovedTestCase12());
        }

        private void ParagraphCanBeRemovedTest(Paragraph para)
        {
            Node parent = para;
            while (parent != null &&
                   parent.NodeType != NodeType.Body &&
                   parent.NodeType != NodeType.Cell &&
                   parent.NodeType != NodeType.Comment &&
                   parent.NodeType != NodeType.Footnote)
                parent = parent.ParentNode;

            parent = parent as CompositeNode;
            if (parent == null)
                Assert.Fail("parent not found");

            Assert.That(false, Is.EqualTo(WordUtil.IsAddParagraphNeeded((CompositeNode)parent)));
            bool canBeRemoved = WordUtil.ParagraphCanBeRemoved(para);
            para.Remove();
            Assert.That(canBeRemoved, Is.EqualTo(!WordUtil.IsAddParagraphNeeded((CompositeNode)parent)));
        }

        private Paragraph ParagraphCanBeRemovedTestCase1()
        {
            mDoc = new Document();
            mDoc.FirstSection.Body.AppendChild(new Paragraph(mDoc));
            return mDoc.FirstSection.Body.Paragraphs[0];
        }


        private Paragraph ParagraphCanBeRemovedTestCase2()
        {
            mDoc = new Document();
            mDoc.FirstSection.Body.AppendChild(new Paragraph(mDoc));
            return mDoc.FirstSection.Body.Paragraphs[1];
        }


        private Paragraph ParagraphCanBeRemovedTestCase3()
        {
            mDoc = new Document();
            return mDoc.FirstSection.Body.FirstParagraph;
        }


        private Paragraph ParagraphCanBeRemovedTestCase4()
        {
            mDoc = new Document();
            mDoc.FirstSection.Body.FirstParagraph.Remove();
            StructuredDocumentTag tag = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            mDoc.FirstSection.Body.AppendChild(tag);
            tag.AppendChild(new Paragraph(mDoc));
            return (Paragraph)mDoc.GetNodeById("0.0.0.0");
        }

        private Paragraph ParagraphCanBeRemovedTestCase5()
        {
            mDoc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            mDoc.FirstSection.Body.AppendChild(tag);
            tag.AppendChild(new Paragraph(mDoc));
            return (Paragraph)mDoc.GetNodeById("0.1.0.0");
        }

        private Paragraph ParagraphCanBeRemovedTestCase6()
        {
            mDoc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            mDoc.FirstSection.Body.AppendChild(tag);
            tag.AppendChild(new Paragraph(mDoc));
            mDoc.FirstSection.Body.AppendChild(new Paragraph(mDoc));
            return (Paragraph)mDoc.GetNodeById("2.0.0");
        }

        private Paragraph ParagraphCanBeRemovedTestCase7()
        {
            mDoc = new Document();
            StructuredDocumentTag tag = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            mDoc.FirstSection.Body.AppendChild(tag);
            tag.AppendChild(new Paragraph(mDoc));
            return (Paragraph)mDoc.GetNodeById("0.0.0");
        }

        private Paragraph ParagraphCanBeRemovedTestCase8()
        {
            mDoc = new Document();
            StructuredDocumentTag tag1 = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            mDoc.FirstSection.Body.AppendChild(tag1);
            StructuredDocumentTag tag2 = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            tag1.AppendChild(tag2);
            tag2.AppendChild(new Paragraph(mDoc));
            return (Paragraph)mDoc.GetNodeById("0.0.1.0.0");
        }

        private Paragraph ParagraphCanBeRemovedTestCase9()
        {
            mDoc = new Document();
            StructuredDocumentTag tag1 = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            mDoc.FirstSection.Body.AppendChild(tag1);
            StructuredDocumentTag tag2 = new StructuredDocumentTag(mDoc, MarkupLevel.Block);
            tag1.AppendChild(tag2);
            tag2.AppendChild(new Paragraph(mDoc));
            mDoc.FirstSection.Body.AppendChild(new Paragraph(mDoc));
            return (Paragraph)mDoc.GetNodeById("0.0.1.0.0");
        }

        private Paragraph ParagraphCanBeRemovedTestCase10()
        {
            mDoc = new Document();
            Table table = new Table(mDoc);
            mDoc.FirstSection.Body.AppendChild(table);
            Row row = new Row(mDoc);
            table.AppendChild(row);
            Cell cell = new Cell(mDoc);
            row.AppendChild(cell);
            Paragraph para = new Paragraph(mDoc);
            cell.AppendChild(para);
            return para;
        }

        private Paragraph ParagraphCanBeRemovedTestCase11()
        {
            mDoc = new Document();
            Table table = new Table(mDoc);
            mDoc.FirstSection.Body.AppendChild(table);
            Row row = new Row(mDoc);
            table.AppendChild(row);
            Cell cell = new Cell(mDoc);
            row.AppendChild(cell);
            Paragraph para = new Paragraph(mDoc);
            cell.AppendChild(para);
            cell.AppendChild(new Paragraph(mDoc));
            return para;
        }

        private Paragraph ParagraphCanBeRemovedTestCase12()
        {
            mDoc = new Document();
            Table table = new Table(mDoc);
            mDoc.FirstSection.Body.AppendChild(table);
            Row row = new Row(mDoc);
            table.AppendChild(row);
            Cell cell = new Cell(mDoc);
            row.AppendChild(cell);
            cell.AppendChild(new Paragraph(mDoc));
            Paragraph para = new Paragraph(mDoc);
            cell.AppendChild(para);
            return para;
        }

        // alexnosk: global variable is declared for C++ to make document available upon processing separate node.
        private Document mDoc;
    }
}
