// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2011 by Dmitry Matveenko

using System.Collections.Generic;
using Aspose.Words.Fields;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestFieldRemover
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestFieldRemoving()
        {
            const string FileName = @"Model\Nodes\TestFieldRemoving.docx";
            Document doc = TestUtil.Open(FileName);

            NodeRange docRange = new NodeRange(doc, doc);
            FieldRemover.ReplaceFieldsWithResults(docRange, FieldRemoverDefaultFilter.Instance);

            CheckNoFieldNodes(doc);

            TestUtil.SaveOpen(doc, FileName.Replace(".doc", " Modified.doc"));
        }

        [Test]
        public void TestBadFieldRemoving()
        {
            const string FileName = @"Model\Nodes\TestBadFieldRemoving.docx";
            Document doc = TestUtil.Open(FileName);

            // Insert extra separators.
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            InsertExtraSeparator(fields[0]);
            InsertExtraSeparator(fields[1]);

            // Insert extra field nodes in the beginning.
            FieldEnd extraFieldEnd = new FieldEnd(doc, new RunPr(), FieldType.FieldTitle, false);
            NodeCollection paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);
            Paragraph firstParagraph = (Paragraph)paragraphs[0];
            firstParagraph.InsertBefore(extraFieldEnd, firstParagraph.FirstChild);
            FieldSeparator extraSeparator = new FieldSeparator(doc, new RunPr(), FieldType.FieldTime);
            firstParagraph.InsertAfter(extraSeparator, firstParagraph.FirstChild);

            // Insert extra field nodes in the beginning.
            extraFieldEnd = new FieldEnd(doc, new RunPr(), FieldType.FieldTitle, false);
            Paragraph lastParagraph = (Paragraph)paragraphs[paragraphs.Count - 1];
            lastParagraph.InsertAfter(extraFieldEnd, lastParagraph.LastChild);
            extraSeparator = new FieldSeparator(doc, new RunPr(), FieldType.FieldTime);
            lastParagraph.InsertAfter(extraSeparator, lastParagraph.LastChild);

            NodeRange docRange = new NodeRange(doc, doc);
            FieldRemover.ReplaceFieldsWithResults(docRange, FieldRemoverRetainFwrFieldsFilter.Instance);

            CheckNoFieldNodes(doc);

            TestUtil.SaveOpen(doc, FileName.Replace(".doc", " Modified.doc"));
        }

        private static void CheckNoFieldNodes(Document doc)
        {
            CheckNoNodes(doc, NodeType.FieldStart);
            CheckNoNodes(doc, NodeType.FieldSeparator);
            CheckNoNodes(doc, NodeType.FieldEnd);
        }

        private static void CheckNoNodes(Document doc, NodeType nodeType)
        {
            Assert.That(doc.GetChildNodes(nodeType, true).Count, Is.EqualTo(0));
        }


        private static void InsertExtraSeparator(Field field)
        {
            FieldSeparator extraSeparator = new FieldSeparator(field.Document, new RunPr(), FieldType.FieldFormula);
            field.Separator.InsertNext(extraSeparator);
        }

        [Test]
        public void TestBoundaryFieldRemoving()
        {
            const string FileName = @"Model\Nodes\TestBoundaryFieldRemoving.docx";
            Document doc = TestUtil.Open(FileName);

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // Test removing of the first node of the interval.
            Field field = fields[0];
            Node rangeStart = field.Start;
            Node rangeEnd = field.End.NextSibling;
            Node expectedRangeStart = field.Separator.NextSibling;
            Node expectedRangeEnd = rangeEnd;
            NodeRange newRange = FieldRemover.ReplaceFieldsWithResults(new NodeRange(rangeStart, rangeEnd), FieldRemoverRetainFwrFieldsFilter.Instance);
            Assert.That(newRange.End.Node, Is.SameAs(expectedRangeEnd));
            Assert.That(newRange.Start.Node, Is.SameAs(expectedRangeStart));

            // Test removing of the unfinished field.
            field = fields[1];
            rangeStart = field.Start.PreviousSibling;
            rangeEnd = field.Separator.PreviousSibling;
            expectedRangeStart = rangeStart;
            expectedRangeEnd = rangeStart;
            newRange = FieldRemover.ReplaceFieldsWithResults(new NodeRange(rangeStart, rangeEnd), FieldRemoverRetainFwrFieldsFilter.Instance);
            Assert.That(newRange.End.Node, Is.SameAs(expectedRangeEnd));
            Assert.That(newRange.Start.Node, Is.SameAs(expectedRangeStart));

            // Test removing of the last node of the interval.
            rangeStart = field.Separator;
            field = fields[2];
            rangeEnd = field.End;
            expectedRangeStart = rangeStart.NextSibling;
            expectedRangeEnd = rangeEnd.PreviousSibling;
            newRange = FieldRemover.ReplaceFieldsWithResults(new NodeRange(rangeStart, rangeEnd), FieldRemoverRetainFwrFieldsFilter.Instance);
            Assert.That(newRange.End.Node, Is.SameAs(expectedRangeEnd));
            Assert.That(newRange.Start.Node, Is.SameAs(expectedRangeStart));

            CheckNoFieldNodes(doc);
        }

        [Test]
        public void TestRemoveAll()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Field fieldWithoutResult = builder.InsertField("TC TocEntry");
            Assert.That(fieldWithoutResult.HasSeparator, Is.False);

            NodeRange fieldRange = new NodeRange(fieldWithoutResult.Start, fieldWithoutResult.End);
            NodeRange nullRange = FieldRemover.ReplaceFieldsWithResults(fieldRange, FieldRemoverDefaultFilter.Instance);
            Assert.That(nullRange, Is.Null);
        }

        [Test]
        public void TestNull()
        {
            Assert.That(FieldRemover.ReplaceFieldsWithResults(null, FieldRemoverRetainFwrFieldsFilter.Instance), Is.Null);
        }
    }
}
