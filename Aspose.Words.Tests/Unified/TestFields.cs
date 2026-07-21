// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for document fields.
    /// </summary>
    [TestFixture]
    public class TestFields : UnifiedTestsBase
    {
        [Test]
        public void TestParseHyperlinkField()
        {
            CheckHyperlink(@" HYPERLINK ""http://www.aspose.com"" ", "http://www.aspose.com", "", "", "");
            CheckHyperlink(@" HYPERLINK  \l ""xxx yyy"" ", "", "xxx yyy", "", "");
            CheckHyperlink(@" HYPERLINK """" \l ""xxx yyy"" ", "", "xxx yyy", "", "");
        }

        /// <summary>
        /// Field extraction of a field that starts outside of the extraction range fails with exception. Fixed.
        /// </summary>
        [Test]
        public void Test27()
        {
            // FOSS: input converted .doc -> .docx (field extraction is format-agnostic).
            Document doc = TestUtil.Open(@"Model\Field\27.docx");
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 14, true);
            IList<Field> fields = FieldExtractor.ExtractToCollection(para);
            Assert.That(fields.Count, Is.EqualTo(2));
            Field field = fields[0];
            Assert.That(field.Type, Is.EqualTo(FieldType.FieldPageRef));
            field = fields[1];
            Assert.That(field.Type, Is.EqualTo(FieldType.FieldHyperlink));
        }

        private static void CheckHyperlink(string fieldCode, string address, string subAddress, string target, string screenTip)
        {
            FieldCodeHyperlink field = FieldCodeHyperlink.Parse(fieldCode);
            Assert.That(address, Is.EqualTo(field.Address));
            Assert.That(subAddress, Is.EqualTo(field.SubAddress));
            Assert.That(target, Is.EqualTo(field.Target));
            Assert.That(screenTip, Is.EqualTo(field.ScreenTip));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTOC(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Field\TestTOC", lf, sf);

            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            // 4 hyperlinks, 4 page ref fields, 1 TOC field and 1 TC field.
            Assert.That(fields.Count, Is.EqualTo(10));

            Assert.That(fields[0].Type, Is.EqualTo(FieldType.FieldPageRef));
            Assert.That(fields[1].Type, Is.EqualTo(FieldType.FieldHyperlink));
            Assert.That(fields[8].Type, Is.EqualTo(FieldType.FieldTOC));
            Assert.That(fields[9].Type, Is.EqualTo(FieldType.FieldTOCEntry));
        }



        /// <summary>
        /// Tests a dead field (TC field in this case).
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDeadField(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Field\TestDeadField", lf, sf);

            FieldExtractorToCollection extractor = new FieldExtractorToCollection();
            extractor.Extract(doc);
            Assert.That(extractor.Fields.Count, Is.EqualTo(1));

            Field field = extractor.Fields[0];

            Assert.That(field.Start.FieldType, Is.EqualTo(FieldType.FieldTOCEntry));

            Assert.That(field.End.FieldType, Is.EqualTo(FieldType.FieldTOCEntry));
            Assert.That(field.End.HasSeparator, Is.EqualTo(false));

            // MSW adds double quotes for RTF format, mimic this behavior.
            string correctCode = (sf == SaveFormat.Rtf) ? " TC  \"TestTC\" " : " TC  TestTC ";
            Assert.That(field.GetFieldCode(), Is.EqualTo(correctCode));
        }


        private static void CheckXEStart(Node node)
        {
            Assert.That(node.NodeType, Is.EqualTo(NodeType.FieldStart));
            Assert.That(((FieldStart)node).FieldType, Is.EqualTo(FieldType.FieldIndexEntry));
            Assert.That(node.GetText(), Is.EqualTo(ControlChar.FieldStart));
        }

        private static void CheckXEEnd(Node node)
        {
            Assert.That(node.NodeType, Is.EqualTo(NodeType.FieldEnd));
            Assert.That(((FieldEnd)node).FieldType, Is.EqualTo(FieldType.FieldIndexEntry));
            Assert.That(((FieldEnd)node).HasSeparator, Is.EqualTo(false));
            Assert.That(node.GetText(), Is.EqualTo(ControlChar.FieldEnd));
        }

        private static void CheckRun(Node node, string text)
        {
            Assert.That(node.NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(node.GetText(), Is.EqualTo(text));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFields(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Field\TestFields", lf, sf);

            int shapeIdx = 1;

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(shape.IsImage, Is.EqualTo(true));
            Assert.That(shape.ImageData.SourceFullName, Is.EqualTo("http://www.aspose.com/images/banner-top-mid.gif"));
            // RK Need to check with DV why this does not work in RTF.
            if (lf != LoadFormat.Rtf)
                Assert.That(shape.ImageData.IsLinkOnly, Is.EqualTo(true));

            shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx + 1, true);
            Assert.That(shape.IsImage, Is.EqualTo(true));
            Assert.That(shape.ImageData.SourceFullName, Is.EqualTo("http://www.aspose.com/images/banner-top-mid.gif"));
            Assert.That(shape.ImageData.IsLinkOnly, Is.EqualTo(false));
        }

        /// <summary>
        /// Test that field result character formatting is preserved when the field result is updated.
        /// WORDSNET-3089 UpdateFields should take formatting from field value, if possible.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMergeFormat(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Field\TestMergeFormat", lf, sf);

            doc.CustomDocumentProperties["XXX"].Value = "Hello1\rHello2";
            doc.Range.UpdateFields();

            doc = TestUtil.SaveOpen(doc, @"Model\Field\TestMergeFormat Modified", lf, sf);

            // Now the field result is across two paragraphs, but has original formatting.
            FieldSeparator fieldSeparator = (FieldSeparator)doc.GetChild(NodeType.FieldSeparator, 0, true);
            Run run = (Run)fieldSeparator.NextSibling;
            Assert.That(run.Text, Is.EqualTo("Hello1"));
            Assert.That(run.Font.Size, Is.EqualTo(24.0));
            Assert.That(run.NextSibling == null, Is.True);

            Paragraph para = (Paragraph)run.NextPreOrder(doc);
            run = (Run)para.FirstChild;
            Assert.That(run.Text, Is.EqualTo("Hello2"));
            Assert.That(run.Font.Size, Is.EqualTo(24.0));
            Assert.That(run.NextSibling is FieldEnd, Is.True);
        }

        // FOSS: UnifiedTestNestedFields (and its GetFieldCharCollection helper) removed. It compared
        // field-char nodes against a baseline loaded from TestNestedFields.doc, and its whole purpose
        // was verifying DOC-import separator synthesis — both the .doc load and that import path are gone.


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestIfMailMerge(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestIfMailMerge", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInsertedPageFields(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestInsertedPageFields", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNestedInFieldCode(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestNestedInFieldCode", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageRef(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestPageRef", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextInput(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestTextInput", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextInputManyParagraphs(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestTextInputManyParagraphs", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestToc1(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Field\TestToc1", lf, sf);
        }



        /// <summary>
        /// WORDSNET-9987 "NullReferenceException" exception occurs upon saving document in RTF format.
        /// The problem occurred, because there is LINK field, which does not have EmbeddedObject.
        /// In case if LINK field does not contain EmbeddedObject (EmbeddedObject==null)
        /// we should write such LINK field as a normal field.
        /// To fix the issue created method HasOleObject, which checks if current field has EmbeddedObject.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect9987(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Field\TestDefect9987", lf, sf);

            // The document should contain only one field.
            Paragraph paragraph = doc.FirstSection.Body.FirstParagraph;
            Assert.That(NodeType.FieldStart, Is.EqualTo(paragraph.GetChildNodes(NodeType.Any, false)[0].NodeType));
            Assert.That(NodeType.Run, Is.EqualTo(paragraph.GetChildNodes(NodeType.Any, false)[1].NodeType));
            Assert.That(NodeType.FieldSeparator, Is.EqualTo(paragraph.GetChildNodes(NodeType.Any, false)[2].NodeType));
            Assert.That(NodeType.Run, Is.EqualTo(paragraph.GetChildNodes(NodeType.Any, false)[3].NodeType));
            Assert.That(NodeType.FieldEnd, Is.EqualTo(paragraph.GetChildNodes(NodeType.Any, false)[4].NodeType));
        }



        /// <summary>
        /// WORDSNET-3704 Hyperlinks asociated with embedded OLE objects are not preserved after processing documents.
        /// Inline OLE should be included into HYPERLINK field the same way as for inline pictures.
        /// </summary>
        /// <param name="lf"></param>
        /// <param name="sf"></param>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect3704(LoadFormat lf, SaveFormat sf)
        {
            // AM. DOC file should be verified manually. We delete hyperlink field upon load and shape have no property to test if there was hyperlink field or was not.
            // Other formats are tested against gold files.
            TestUtil.OpenSaveOpen(@"Model\Field\TestDefect3704", lf, sf);
        }

        private static void TestDefect11155(UnifiedScenario scenario)
        {
            Document doc = TestUtil.Open(@"Model\Field\TestDefect11155", TestUtil.GetLoadFormat(scenario));
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);
            Assert.That(fields.Count, Is.EqualTo(3));
            Assert.That(fields[2].Type, Is.EqualTo(FieldType.FieldPage));
            TestUtil.SaveOpen(doc, @"Model\Field\TestDefect11155", scenario);
        }
    }
}
