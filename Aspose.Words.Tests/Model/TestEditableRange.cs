// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/06/2013 by Andrey Noskov

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Basic EditableRange functionality tests.
    /// </summary>
    [TestFixture]
    public class TestEditableRange
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// Test editable ranges located inside SDT.
        /// </summary>
        [Test]
        public void EditableRangeInsideSdt()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\EditableRangeInsideSdt.docx");
#pragma warning disable CS0612
#pragma warning restore CS0612

            List<EditableRangeStart> starts =
                doc.GetChildNodes(NodeType.EditableRangeStart, true).ToList<EditableRangeStart>();
            List<EditableRangeEnd> ends =
                doc.GetChildNodes(NodeType.EditableRangeEnd, true).ToList<EditableRangeEnd>();
            Assert.That(starts.Count, Is.EqualTo(3));
            Assert.That(ends.Count, Is.EqualTo(3));

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, -1, true);
            Assert.That(starts[2].ParentNode, Is.SameAs(sdt));
            Assert.That(ends[2].ParentNode, Is.SameAs(sdt));

            Assert.That(starts[2].DisplacedBy, Is.EqualTo(DisplacedByType.Prev));
            Assert.That(ends[2].DisplacedBy, Is.EqualTo(DisplacedByType.Next));
        }

        /// <summary>
        /// Test editable ranges located inside Footnote.
        /// According to MS Word behavior, there is no way to insert editable ranges inside Footnote.
        /// But according to specification ISO29500-1 p.17.13.7.2 it is possible.
        /// So I have created this test document manually.
        /// </summary>
        [Test]
        public void EditableRangeInsideFootnote()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\EditableRange\EditableRangeInsideFootnote", UnifiedScenario.Docx2Docx);
            Assert.That(doc.GetChildNodes(NodeType.EditableRangeStart, true).Count, Is.EqualTo(1));
            Assert.That(doc.GetChildNodes(NodeType.EditableRangeEnd, true).Count, Is.EqualTo(1));
        }



        /// <summary>
        /// WORDSNET-9000 Add an ability to define new EditableRanges in API.
        /// </summary>
        [Test]
        public void TestEditableRangeStartEnd()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            EditableRangeStart start = builder.StartEditableRange();
            builder.Writeln("Some Text Inside EditableRange");
            builder.Writeln("Some Text Inside EditableRange");
            builder.EndEditableRange(start);

            builder.Writeln("Some Text outside EditableRange");
            builder.Writeln("Some Text outside EditableRange");

            start.EditableRange.EditorGroup = EditorType.Everyone;
            doc.Protect(ProtectionType.ReadOnly, "password");

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeStartEnd.docx", new OoxmlSaveOptions(SaveFormat.Docx));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(1));
            Assert.That(ends.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestEditableRangeWithoutEnd()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            EditableRangeStart start = builder.StartEditableRange();

            builder.Writeln("Some Text Inside EditableRange");

            start.EditableRange.EditorGroup = EditorType.Everyone;
            doc.Protect(ProtectionType.ReadOnly, "4444");

            DocumentValidator validator = new DocumentValidator();
            validator.Execute(new SaveInfo(doc, null, null, new OoxmlSaveOptions()));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable start was removed by validator.
            Assert.That(starts.Count, Is.EqualTo(0));
            Assert.That(ends.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestEditableRangeInDifferentSections()
        {
            Document doc = TestUtil.Open(@"Model\EditableRange\TestJira9000.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            EditableRangeStart start = builder.StartEditableRange();
            builder.Writeln("Some Text Inside EditableRange");

            builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));

            builder.MoveTo(doc.LastSection.Body.FirstParagraph, 0);
            builder.EndEditableRange(start);
            start.EditableRange.EditorGroup = EditorType.Everyone;
            doc.Protect(ProtectionType.ReadOnly, "4444");
            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeInDifferentSections.docx", new OoxmlSaveOptions(SaveFormat.Docx));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(1));
            Assert.That(ends.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestEditableRangeInTable()
        {
            Document doc = TestUtil.Open(@"Model\EditableRange\TestJira9000.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);

            Table table = doc.LastSection.Body.Tables[0];
            builder.MoveTo(table.Rows[0].Cells[2].FirstParagraph, 0);

            EditableRangeStart start = builder.StartEditableRange();

            builder.Writeln("EditableRange");

            builder.MoveTo(table.Rows[2].Cells[4].FirstParagraph, 0);
            builder.EndEditableRange(start);

            DocumentValidator validator = new DocumentValidator();
            validator.Execute(new SaveInfo(doc, null, null, new OoxmlSaveOptions()));

            table = doc.LastSection.Body.Tables[0];
            NodeCollection starts = table.Rows[0].Cells[2].GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = table.Rows[2].Cells[4].GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(1));
            Assert.That(ends.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestEditableRangeNested()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // [ {} ] Range inside range.
            EditableRangeStart startRange1 = builder.StartEditableRange();
            builder.Writeln("EditableRange1");

            EditableRangeStart startRange2 = builder.StartEditableRange();
            builder.Writeln("EditableRange2");
            builder.EndEditableRange(startRange2);

            builder.Writeln("EditableRange1");
            builder.EndEditableRange(startRange1);

            builder.Writeln("Simple Text Separator");

            // [ {] } Overlaped ranges.
            EditableRangeStart startRange3 = builder.StartEditableRange();
            builder.Writeln("EditableRange3");

            EditableRangeStart startRange4 = builder.StartEditableRange();
            builder.Writeln("EditableRange4");

            builder.EndEditableRange(startRange3);

            builder.Writeln("EditableRange4");
            builder.EndEditableRange(startRange4);

            startRange1.EditableRange.EditorGroup = EditorType.Everyone;
            startRange2.EditableRange.EditorGroup = EditorType.Editors;

            doc.Protect(ProtectionType.ReadOnly, "4444");

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeNested.docx", new OoxmlSaveOptions(SaveFormat.Docx));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable start was removed by validator.
            Assert.That(starts.Count, Is.EqualTo(4));
            Assert.That(ends.Count, Is.EqualTo(4));
        }

        [Test]
        public void TestEditableRangeEndWithoutParam()
        {
            const string textInsideRange = "Some Text inside EditableRange";
            const string textOutsideRange = "Some Text outside EditableRange";

            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.StartEditableRange();
            builder.Writeln(textInsideRange);
            builder.EndEditableRange();

            builder.Writeln(textOutsideRange);

            builder.StartEditableRange();
            builder.Writeln(textInsideRange);
            builder.EndEditableRange();

            builder.Writeln(textOutsideRange);

            builder.StartEditableRange();
            builder.Writeln(textInsideRange);
            builder.EndEditableRange();

            builder.Writeln(textOutsideRange);

            builder.StartEditableRange();
            builder.Writeln(textInsideRange);
            builder.EndEditableRange();

            builder.Writeln(textOutsideRange);

            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\TestEditableRangeEndWithoutParam.docx", new OoxmlSaveOptions(SaveFormat.Docx));

            NodeCollection starts = doc.GetChildNodes(NodeType.EditableRangeStart, true);
            NodeCollection ends = doc.GetChildNodes(NodeType.EditableRangeEnd, true);

            // Check editable range are present.
            Assert.That(starts.Count, Is.EqualTo(4));
            Assert.That(ends.Count, Is.EqualTo(4));
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestEditableRangeWithoutStart()
        {
            DocumentBuilder builder = new DocumentBuilder();
            builder.EndEditableRange();
        }


        /// <summary>
        /// WORDSNET-18124 FileCorruptedException was thrown on loading created RTF.
        /// Fixed a string constant for the case, when SingleUser is empty and EditorGroup is not specified.
        /// </summary>
        [Test]
        public void TestJira18124()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.MoveTo(doc.Sections[0].Body.FirstParagraph);
            builder.StartEditableRange();
            builder.InsertNode(new Run(doc));
            builder.EndEditableRange();

            doc.Protect(ProtectionType.ReadOnly, "somePassword");

            // Verify no exception on document open.
            // FOSS: Rtf writer removed; verify the EditableRange roundtrips through Docx instead.
            doc = TestUtil.SaveOpen(doc, @"ExportRtf\TestJira18124.docx", null, false);

            // Verify correct import of EditableRange's SingleUser and EditorGroup.
            // FOSS: swapped Rtf->Docx; Docx preserves an unspecified editor group as Unspecified
            // (the removed Rtf writer instead defaulted it to None).
            EditableRangeStart start = (EditableRangeStart)doc.GetChild(NodeType.EditableRangeStart, 0, true);
            Assert.That(start.SingleUser, Is.EqualTo(""));
            Assert.That(start.EditorGroup, Is.EqualTo(EditorType.Unspecified));
        }

        /// <summary>
        /// WORDSNET-18924 EditableRange seems not working when saving to DOC format.
        /// Wrong default value of the <see cref="EditorType"/>. Fixed default value.
        /// </summary>
        [TestCase(EditorType.None)]
        [TestCase(EditorType.Current)]
        [TestCase(EditorType.Editors)]
        [TestCase(EditorType.Owners)]
        [TestCase(EditorType.Contributors)]
        [TestCase(EditorType.Administrators)]
        [TestCase(EditorType.Everyone)]
        public void Test18924(EditorType editorType)
        {
            DocumentBuilder builder = new DocumentBuilder();
            Document doc = builder.Document;
            Paragraph para = builder.InsertParagraph();

            Run run = new Run(doc);
            run.Text = "Test";
            para.AppendChild(run);
            EditableRangeStart start = builder.StartEditableRange();
            para.InsertBefore(start, run);
            para.InsertAfter(builder.EndEditableRange(), run);

            doc.Protect(ProtectionType.ReadOnly);

            Assert.That(start.SingleUser, Is.EqualTo(""));
            Assert.That(start.EditorGroup, Is.EqualTo(EditorType.Unspecified));

            start.EditorGroup = editorType;

            // FOSS: Doc writer removed; roundtrip through Docx (no committed .docx gold).
            doc = TestUtil.SaveOpen(doc, @"Model\EditableRange\Test18924.docx", (SaveOptions)null, false);
            start = (EditableRangeStart)doc.GetChild(NodeType.EditableRangeStart, 0, true);

            Assert.That(start.SingleUser, Is.EqualTo(""));
            Assert.That(start.EditorGroup, Is.EqualTo(editorType));
        }

    }
}
