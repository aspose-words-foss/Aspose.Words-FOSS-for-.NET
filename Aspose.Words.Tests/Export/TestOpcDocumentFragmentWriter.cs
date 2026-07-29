// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2021 by Dmitry Sokolov

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Properties;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Tests checks export of a document fragment to the FOPC format.
    /// </summary>
    [TestFixture]
    public class TestOpcDocumentFragmentWriter
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        ///  WORDSNET-22409 Requesting feature to get OOXML of content control.
        ///  <see cref="StructuredDocumentTag.WordOpenXML"> and <see cref="StructuredDocumentTagRangeStart.WordOpenXML">
        ///  properties implemented for ability to export a SDT content to FOPC format.
        /// </summary>
        [TestCase("Component 1", false)]
        [TestCase("Component 2", false)]
        [TestCase("Component 3", false)]
        [TestCase("Component 4", false)]
        [TestCase("Component 5", false)]
        [TestCase("Component 6", false)]
        [TestCase("Component 7", false)]
        [TestCase("Component 8", false)]
        [TestCase("Component 9", true)]
        [TestCase("Component 10", false)]
        [TestCase("Component 11", false)]
        [TestCase("Component 12", false)]
        [TestCase("Component 13", false)] // Content has picture, AW preserves it unlike the Word.
        public void Test22409(string tag, bool isRange)
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409.docx");
            int sdtLength = FindNodeByTag<Node>(doc, tag).Range.Text.Length;

            string fopcContent = isRange ?
                FindNodeByTag<StructuredDocumentTagRangeStart>(doc, tag).WordOpenXML :
                FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;

            // Use golds due to priority level.
            doc = SaveAndCheckGold(fopcContent, @"ExportDocx\Test22409" + tag);

            // There is not a simple way to obtain text for multi sections SDT. So, skip the case with range.
            Assert.That(isRange ? 0 : doc.GetText().Length, Is.EqualTo(sdtLength));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks simple cases for SDT on different levels.
        /// </summary>
        [TestCase("Cell", "Cell level\f")]
        [TestCase("Inline", "Inline SDT\f")]
        [TestCase("Block", "Block\rLevel\rSDT\f")]
        [TestCase("Row", "Amount\aTerm\aInterestFrequency\a\a\f")]
        public void Test22409Levels(string tag, string content)
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409Levels.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo(content));
            CheckSectPr(doc.FirstSection, 1134, 1701, 850, 708);
            CheckSettings(doc);
            CheckStyles(doc);
        }


        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT row level SDT contains two rows.
        /// </summary>
        [Test]
        public void Test22409RowLevelTwoRows()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409RowLevelTwoRows.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "Rows").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Table tbl = doc.FirstSection.Body.Tables[0];
            Assert.That(tbl.FirstRow.Cells[0].GetText(), Is.EqualTo("Amount\a"));
            Assert.That(tbl.FirstRow.Cells[1].GetText(), Is.EqualTo("Term\a"));
            Assert.That(tbl.FirstRow.Cells[2].GetText(), Is.EqualTo("InterestFrequency\a"));

            Assert.That(tbl.LastRow.Cells[0].GetText(), Is.EqualTo("Amount2\a"));
            Assert.That(tbl.LastRow.Cells[1].GetText(), Is.EqualTo("Term2\a"));
            Assert.That(tbl.LastRow.Cells[2].GetText(), Is.EqualTo("InterestFrequency2\a"));

            Assert.That(tbl.Rows.Count, Is.EqualTo(2));
            Assert.That(tbl.FirstRow.Cells.Count, Is.EqualTo(3));
            Assert.That(doc.FirstSection.Body.Tables.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT spreads to multiple sections.
        /// </summary>
        [Test]
        public void Test22409SdtRange()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409SdtRange.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTagRangeStart>(doc, "MultiSections").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.Sections.Count, Is.EqualTo(2));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo("\f"));
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphFormat.StyleName, Is.EqualTo("List Paragraph"));

            CheckPageSize(doc.FirstSection, 350, 350);

            Assert.That(doc.LastSection.Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(doc.LastSection.Body.FirstParagraph.GetText(), Is.EqualTo("Section break above is inside SDT\f"));
            Assert.That(doc.LastSection.Body.FirstParagraph.ParagraphFormat.StyleName, Is.EqualTo("List Paragraph"));

            CheckPageSize(doc.LastSection, 612, 792);
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT placed in the header and footer.
        /// </summary>
        [TestCase("Header", "Header SDT\f")]
        [TestCase("Footer", "Footer SDT\f")]
        [TestCase("Footnote", "Footnote SDT\f")]
        public void Test22409HeaderFooter(string tag, string expectedContent)
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409HeaderFooter.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo(expectedContent));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Check behavior when SDT has non-updated content.
        /// Looks like it is not possible to set mapping only for a row or cell.
        /// So, skip cases for row/cell level SD's.
        /// </summary>
        [TestCase("Block")]
        [TestCase("Inline")]
        public void Test22409ContentUpdate(string tag)
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409ContentUpdate.docx");
            doc.BuiltInDocumentProperties.Comments = "ABC";

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo("ABC\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT has not a content. AW writes empty document at this case.
        /// SDT node at this case still has reference to the document even node was not added.
        /// So, SDT may be exported when it is not in the document tree.
        /// </summary>
        [TestCase(MarkupLevel.Row)]
        [TestCase(MarkupLevel.Cell)]
        [TestCase(MarkupLevel.Block)]
        [TestCase(MarkupLevel.Inline)]
        public void Test22409EmptySdt(MarkupLevel level)
        {
            StructuredDocumentTag sdt = new StructuredDocumentTag(new Document(), level);
            Document fopcDoc = OpenDocFromString(sdt.WordOpenXML);

            Assert.That(fopcDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(fopcDoc.FirstSection.Body.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT range has not a content. AW writes empty document at this case.
        /// </summary>
        [Test]
        public void Test22409EmptySdtRange()
        {
            StructuredDocumentTagRangeStart sdtRangeStart = CreateEmptySdtRange();
            Document fopcDoc = OpenDocFromString(sdtRangeStart.WordOpenXML);

            Assert.That(fopcDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(fopcDoc.FirstSection.Body.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when range start or end is not in the document tree.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        [ExpectedException(
            typeof(InvalidOperationException),
            ExpectedMessage = "Structured document tag range ",
            MatchType = MessageMatch.StartsWith)]

        public void Test22409SdtRangeNotInTree(bool start)
        {
            StructuredDocumentTagRangeStart sdtRangeStart = CreateEmptySdtRange();

            Node nodeToRemove = start ? sdtRangeStart : (Node)sdtRangeStart.RangeEnd;
            nodeToRemove.Remove();

            OpenDocFromString(sdtRangeStart.WordOpenXML);
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT placed in the glossary document.
        /// AW does not write content for such SDT for a while.
        /// </summary>
        [Test]
        public void Test22409SdtInGlossary()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409SDTInGlossary.docx");
            StructuredDocumentTag sdt =
                (StructuredDocumentTag)doc.GlossaryDocument.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Document fopcDoc = OpenDocFromString(sdt.WordOpenXML);

            Assert.That(fopcDoc.Sections.Count, Is.EqualTo(1));
            Assert.That(fopcDoc.FirstSection.Body.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks that SDT export does not produce warnings notifications.
        /// </summary>
        [Test]
        public void Test22409Warns()
        {
            Document doc = new Document();
            TestWarningCallback wc = new TestWarningCallback();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, SdtType.PlainText, MarkupLevel.Inline);

            doc.EnsureMinimum();
            doc.WarningCallback = wc;
            doc.AutomaticallyUpdateStyles = true;

            sdt.AppendChild(new Run(doc, "test"));
            doc.FirstSection.Body.FirstParagraph.AppendChild(sdt);

            // Save the document.
            using (MemoryStream ms = new MemoryStream())
                doc.Save(ms, SaveFormat.FlatOpc);

            wc.Contains(WarningSource.Validator,
                WarningType.DataLoss, WarningStrings.EmptyTemplatePath);

            // Save SDTto FOPC.
            wc.Clear();
            doc = OpenDocFromString(sdt.WordOpenXML);

            Assert.That(wc.Count, Is.EqualTo(0));
            Assert.That(doc.GetText(), Is.EqualTo("Click here to enter text.test\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT spreads to three sections.
        /// </summary>
        [Test]
        public void Test22409SdtRangeThreeSections()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409SdtRangeThreeSections.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTagRangeStart>(doc, "MultiSections").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.Sections.Count, Is.EqualTo(3));
            CheckPageSize(doc.FirstSection, 350, 350);
            CheckPageSize(doc.Sections[1], 350, 350);
            CheckPageSize(doc.LastSection, 612, 792);

            Assert.That(doc.FirstSection.Body.GetText(), Is.EqualTo("a\f"));
            Assert.That(doc.Sections[1].Body.GetText(), Is.EqualTo("b\f"));
            Assert.That(doc.LastSection.Body.GetText(), Is.EqualTo("Section break above is inside SDT\f"));

            Assert.That(doc.FirstSection.HeadersFooters.Count, Is.GreaterThan(0));
            Assert.That(doc.Sections[1].HeadersFooters.Count, Is.GreaterThan(0));
            Assert.That(doc.LastSection.HeadersFooters.Count, Is.GreaterThan(0));

            Assert.That(doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("Header first section\r"));
            Assert.That(doc.Sections[1].HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("Header second section\r"));
            Assert.That(doc.LastSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText(), Is.EqualTo("Header third section\r"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when there is a content before the SDT range and this content should
        /// not be copied to the destination.
        /// </summary>
        [Test]
        public void Test22409SdtRangeContentBefore()
        {
            const string textBefore = "Text before";
            Document doc = TestUtil.Open(@"ExportDocx\Test22409SdtRangeContentBefore.docx");

            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo(textBefore + "\r"));
            string fopcContent = FindNodeByTag<StructuredDocumentTagRangeStart>(doc, "MultiSections").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when there is a content after the SDT range and this content should
        /// not be copied to the destination.
        /// </summary>
        [Test]
        public void Test22409SdtRangeContentAfter()
        {
            const string textAfter = "Text after";
            Document doc = TestUtil.Open(@"ExportDocx\Test22409SdtRangeContentAfter.docx");

            Assert.That(doc.LastSection.Body.LastParagraph.GetText(), Is.EqualTo(textAfter + "\f"));
            string fopcContent = FindNodeByTag<StructuredDocumentTagRangeStart>(doc, "MultiSections").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.LastSection.Body.LastParagraph.GetText(), Is.EqualTo("Section break above is inside SDT\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// The case when nested SDT has mapped content.
        /// </summary>
        [Test]
        public void Test22409Nested()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409Nested.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "Top").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            // Value mapped to document property. And in the new document this property is empty.
            // So, we will see placeholder text,
            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.FirstSection.Body.FirstParagraph.FirstChild;
            Assert.That(((Run)sdt.FirstChild).Text, Is.EqualTo("Click or tap here to enter text."));

            Assert.That(doc.Sections.Count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when a cell has two SDT ancestors with cell level.
        /// </summary>
        [Test]
        public void Test22409MultipleCellLevel()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409MultipleCellLevel.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "CellLevel").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo("RichText SDT inside Table inside other SDT Cell2\f"));
            // Actually here should be one SDT. Skip for a while.
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(0));
        }



        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "checkbox" SDT.
        /// </summary>
        [Test]
        public void Test22409CheckboxSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409CheckboxSdt.docx");

            string fopcContentUnchecked = FindNodeByTag<StructuredDocumentTag>(doc, "Unchecked").WordOpenXML;
            string fopcContentChecked = FindNodeByTag<StructuredDocumentTag>(doc, "Checked").WordOpenXML;

            doc = OpenDocFromString(fopcContentUnchecked);
            Assert.That(doc.GetText(), Is.EqualTo("☐\f"));

            doc = OpenDocFromString(fopcContentChecked);
            Assert.That(doc.GetText(), Is.EqualTo("☒\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "combobox" SDT.
        /// </summary>
        [Test]
        public void Test22409ComboboxSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409ComboboxSdt.docx");

            string fopcContentSelected = FindNodeByTag<StructuredDocumentTag>(doc, "ItemSelected").WordOpenXML;
            string fopcContentNoSelected = FindNodeByTag<StructuredDocumentTag>(doc, "NoItemSelected").WordOpenXML;

            doc = OpenDocFromString(fopcContentSelected);
            Assert.That(doc.GetText(), Is.EqualTo("Item2\f"));

            doc = OpenDocFromString(fopcContentNoSelected);
            Assert.That(doc.GetText(), Is.EqualTo("Choose an item.\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "date" SDT.
        /// </summary>
        [Test]
        public void Test22409DateSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409DateSdt.docx");

            string fopcContentSelected = FindNodeByTag<StructuredDocumentTag>(doc, "DateSelected").WordOpenXML;
            string fopcContentNoSelected = FindNodeByTag<StructuredDocumentTag>(doc, "NoDateSelected").WordOpenXML;

            doc = OpenDocFromString(fopcContentSelected);
            Assert.That(doc.GetText(), Is.EqualTo("12.11.2021\f"));

            doc = OpenDocFromString(fopcContentNoSelected);
            Assert.That(doc.GetText(), Is.EqualTo("Click or tap to enter a date.\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "dropdown" SDT.
        /// </summary>
        [Test]
        public void Test22409DropdownSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409DropdownSdt.docx");

            string fopcContentSelected = FindNodeByTag<StructuredDocumentTag>(doc, "ItemSelected").WordOpenXML;
            string fopcContentNoSelected = FindNodeByTag<StructuredDocumentTag>(doc, "NoItemSelected").WordOpenXML;

            doc = OpenDocFromString(fopcContentSelected);
            Assert.That(doc.GetText(), Is.EqualTo("Item2\f"));

            doc = OpenDocFromString(fopcContentNoSelected);
            Assert.That(doc.GetText(), Is.EqualTo("Choose an item.\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "picture" SDT.
        /// Actually the Word produces empty document in this case.
        /// </summary>
        [Test]
        public void Test22409ImgSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409ImgSdt.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "Img").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo("\f"));

            Shape shape = (Shape)doc.FirstSection.Body.FirstParagraph.FirstChild;
            Assert.That(shape.ImageData.ImageBytes, IsNot.Null());
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "repeating section" SDT.
        /// Actually the Word throws the exception in this case.
        /// </summary>
        [Test]
        public void Test22409RepeatingSectionSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409RepeatingSectionSdt.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "RepeatingSection").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo("\rRow\a\a\a\r\rRow2\a\a\a\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "group" SDT.
        /// </summary>
        [Test]
        public void Test22409GroupSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409GroupSdt.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "Group").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo("☒text\f"));

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(((StructuredDocumentTag)para.FirstChild).SdtType, Is.EqualTo(SdtType.Checkbox));
            Assert.That(((StructuredDocumentTag)para.LastChild).SdtType, Is.EqualTo(SdtType.PlainText));
        }



        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks export of the "docPartObj" SDT.
        /// Actually the Word produces empty document in this case.
        /// </summary>
        [Test]
        public void Test22409DocPartObjSdt()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409DocPartSdt.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "docPartObj").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo("Test\f"));
        }


        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT placed in a shape.
        /// </summary>
        [Test]
        public void Test22409SdtInShape()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409SdtInShape.docx");

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "ShapeSdt").WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.GetText(), Is.EqualTo("Sdt text\f"));
        }

        /// <summary>
        /// Related with WORDSNET-22409
        /// Checks the case when SDT has unknown level.
        /// </summary>
        [Test]
        public void Test22409SdtUnknownLevel()
        {
            Document doc = new Document();
            StructuredDocumentTag sdt = new StructuredDocumentTag(doc, MarkupLevel.Unknown);

            doc = OpenDocFromString(sdt.WordOpenXML);
            Assert.That(doc.GetText(), Is.EqualTo("\f"));
        }

        /// <summary>
        /// WORDSNET-23220 The paragraph is missing when using WordOpenXML.
        /// Paragraph was lost because of wrong iteration approach through nodes.
        /// </summary>
        [Test]
        public void Test23220()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test23220.docx");
            doc.Revisions.AcceptAll();

            StructuredDocumentTag sdt = (StructuredDocumentTag)doc.GetChild(NodeType.StructuredDocumentTag, 0, true);
            doc = OpenDocFromString(sdt.WordOpenXML);

            Body body = doc.FirstSection.Body;
            Assert.That(body.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(body.FirstParagraph.GetText(), Is.EqualTo("Lorem ipsum dolor sit amet\r"));
            Assert.That(body.LastParagraph.GetText(), Is.EqualTo("Maecenas porttitor congue massa. Fusce posuere.\f"));
        }

        /// <summary>
        /// WORDSNET-23347 Out of memory exception when creating custom XML.
        /// We should clear CustomXmlParts in destination document while getting WordOpenXML string value.
        /// </summary>
        [Test]
        [JavaDelete("Not portable to Java SerializableObject")]
        public void Test23347()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test23347.docx");

            // The following is customer's scenario to reproduce the issue.
            int i = 0;
            foreach (StructuredDocumentTag contentControl in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                string xmlns = "urn:in:item" + i;
                SerializableObject xmlPart = new SerializableObject();
                string componentContentXml = contentControl.WordOpenXML;
                xmlPart.C = componentContentXml;
                string customXmlPart = Serialize(xmlPart, xmlns);
                CustomXmlPart cxp = doc.CustomXmlParts.Add(Guid.NewGuid().ToString(), customXmlPart);
                contentControl.XmlMapping.SetMapping(cxp, "/ns0:I[1]", string.Format("xmlns:ns0='{0}'", xmlns));
                i++;
            }

            // There are 42 SDTs in the document.
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(42));
            Assert.That(doc.CustomXmlParts.Count, Is.EqualTo(42));
        }

        /// <summary>
        /// WORDSNET-23887 Caption/Ref present on first para get missing.
        /// The Word inserts bookmark start node while exporting SDT content as XML. It happens when bookmark "start" follows
        /// before a SDT and resulted content has just bookmark end node. Mimic Word to fix the issue.
        /// </summary>
        [TestCase("Test23887.docx")]
        [TestCase("Test23887NoDisplacedByCustomXml.docx")]
        public void Test23887(string fileName)
        {
            Document doc = TestUtil.Open(@"ExportDocx\" + fileName);

            const string tag = "Component 1";
            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            const string bookmarkName = "_Ref101801046";
            BookmarkEnd end = BookmarkFinder.FindBookmarkEnd(doc, bookmarkName);
            BookmarkStart start = BookmarkFinder.FindBookmarkStart(doc, bookmarkName);

            Assert.That(end, IsNot.Null());
            Assert.That(start, IsNot.Null());
            Assert.That(end.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
            Assert.That(start.DisplacedBy, Is.EqualTo(DisplacedByType.Unspecified));
        }

        /// <summary>
        /// Related with WORDSNET-23887
        /// Checks the case when problematic document contains two bookmarks.
        /// </summary>
        [Test]
        public void Test23887TwoBookmarks()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test23887TwoBookmarks.docx");

            const string tag = "Component 1";
            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            const string bookmarkName1 = "bookmark1";
            BookmarkStart firstStart = BookmarkFinder.FindBookmarkStart(doc, bookmarkName1);
            Assert.That(firstStart, IsNot.Null());
            Assert.That(BookmarkFinder.FindBookmarkEnd(doc, bookmarkName1), IsNot.Null());

            const string bookmarkName2 = "_Ref101801046";
            BookmarkStart secondStart = BookmarkFinder.FindBookmarkStart(doc, bookmarkName2);
            Assert.That(secondStart, IsNot.Null());
            Assert.That(BookmarkFinder.FindBookmarkEnd(doc, bookmarkName2), IsNot.Null());

            // Check sequence.
            Assert.That(ReferenceEquals(firstStart, secondStart.PreviousSibling), Is.True);
        }

        /// <summary>
        /// Related with WORDSNET-23887
        /// Checks the case when bookmark "start" does not exist at all.
        /// </summary>
        [Test]
        public void Test23887NoStart()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test23887NoStart.docx");

            const string tag = "Component 1";
            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, tag).WordOpenXML;
            doc = OpenDocFromString(fopcContent);

            const string bookmarkName = "_Ref101801046";
            Assert.That(BookmarkFinder.FindBookmarkStart(doc, bookmarkName), Is.Null);
            Assert.That(BookmarkFinder.FindBookmarkEnd(doc, bookmarkName), Is.Null);
        }

        /// <summary>
        /// WORDSNET-25558 - Add 'WordOpenXmlMinimal' Property to 'StructuredDocumentTagRangeStart' Class
        /// Added public WordOpenXMLMinimal for StructuredDocumentTagRangeStart node.
        /// </summary>
        [Test]
        public void Test25558()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test22409.docx");

            StructuredDocumentTagRangeStart sdtRangeStart =
                FindNodeByTag<StructuredDocumentTagRangeStart>(doc, "Component 9");
            string fopcContentMinimal = sdtRangeStart.WordOpenXMLMinimal;

            Assert.That(fopcContentMinimal.Length, Is.LessThan(18000));
        }

        /// <summary>
        /// WORDSNET-28539 WordOpenXMLMinimal output includes document-level protection in protected DOCX.
        /// Tests that FlatOpc output document obtained via WordOpenXMLMinimal does not preserve an active protection type.
        /// </summary>
        [Test]
        public void Test28539()
        {
            Document doc = TestUtil.Open(@"ExportDocx\Test28539.docx");
            // The input document has an active protection type.
            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.AllowOnlyRevisions));

            string fopcContent = FindNodeByTag<StructuredDocumentTag>(doc, "Component 1").WordOpenXMLMinimal;
            doc = OpenDocFromString(fopcContent);

            Assert.That(doc.ProtectionType, Is.EqualTo(ProtectionType.NoProtection));
        }

        /// <summary>
        /// Serializes object to be compatible with CustomXmlPart.
        /// </summary>
        /// <remarks>The helper method used in WORDSNET-23347 customer's scenario.</remarks>
        [JavaDelete("Not portable to Java SerializableObject")]
        private static string Serialize(SerializableObject customXmlPart, string ns)
        {
            string content = string.Empty;
            if (!string.IsNullOrEmpty(customXmlPart.C))
                content = customXmlPart.C;
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.Indent = true;
            settings.NewLineChars = "\r\n";
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                xmlWriter.WriteStartElement("I", ns);
                xmlWriter.WriteString(content);
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Represents a serializable object.
        /// </summary>
        /// <remarks>The helper class used in WORDSNET-23347 customer's scenario.</remarks>
        [XmlRoot]
        [JavaDelete("Not portable to Java SerializableObject")]
        private class SerializableObject
        {
            [XmlText]
            internal string C { get; set; }
        }

        private static void CheckPageSize(Section sect, double width, double height)
        {
            Assert.That(sect.PageSetup.PageWidth, Is.EqualTo(width).Within(0.001));
            Assert.That(sect.PageSetup.PageHeight, Is.EqualTo(height).Within(0.001));
        }

        private static StructuredDocumentTagRangeStart CreateEmptySdtRange()
        {
            Document doc = new Document();
            doc.EnsureMinimum();

            StructuredDocumentTagRangeStart sdtRangeStart = new StructuredDocumentTagRangeStart(doc, SdtType.RichText);
            StructuredDocumentTagRangeEnd sdtRangeEnd = new StructuredDocumentTagRangeEnd(doc, sdtRangeStart.Id);

            doc.FirstSection.Body.PrependChild(sdtRangeEnd);
            doc.FirstSection.Body.PrependChild(sdtRangeStart);

            return sdtRangeStart;
        }

        /// <summary>
        /// Finds node in the document using specified tag.
        /// </summary>
        /// <typeparam name="T">Node type which contains the tag.</typeparam>
        /// <param name="doc">Source document.</param>
        /// <param name="tag">Node tag value.</param>
        /// <returns>Node from the source document.</returns>
        [JavaThrows(true)]
        private static T FindNodeByTag<T>(Document doc, string tag) where T : Node
        {
            List<Node> nodes = new List<Node>();

            NodeCollection filteredNodes = doc.GetChildNodes(NodeType.StructuredDocumentTag, true);
            nodes.AddRange(filteredNodes);

            filteredNodes = doc.GetChildNodes(NodeType.StructuredDocumentTagRangeStart, true);
            nodes.AddRange(filteredNodes);

            foreach (Node node in nodes)
            {
                switch (node.NodeType)
                {
                    case NodeType.StructuredDocumentTag:
                    {
                        if (((StructuredDocumentTag)node).Tag == tag)
                            return (T) node;

                        break;
                    }

                    case NodeType.StructuredDocumentTagRangeStart:
                    {
                        if (((StructuredDocumentTagRangeStart)node).Tag == tag)
                            return (T) node;

                        break;
                    }
                }
            }

            throw new Exception("Node with tag \"" + tag + "\" was not found.");
        }

        private static Document SaveAndCheckGold(string fopcDocument, string fileName)
        {
            Document outDoc = OpenDocFromString(fopcDocument);

            // Check markup via gold.
            outDoc = TestUtil.SaveOpen(
                outDoc,
                fileName,
                TestUtil.BuildScenario(LoadFormat.FlatOpc, SaveFormat.FlatOpc, false));

            return outDoc;
        }

        private static Document OpenDocFromString(string fopcDocument)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms, new UTF8Encoding(false)))
            {
                sw.Write(fopcDocument);
                sw.Flush();

                LoadOptions lo = new LoadOptions();
                lo.LoadFormat = LoadFormat.FlatOpc;

                ms.Position = 0;
                Document outDoc = new Document(ms, lo);

                return outDoc;
            }
        }

        private static void CheckSettings(Document doc)
        {
            // Check some values explicitly.
            // Actually the Word does not adds "app.xml" and "core.xml" at all to a package.
            // AW writes default values for a while.
            Assert.That(doc.BuiltInDocumentProperties.Contains(PropertyName.Author), Is.False);
            Assert.That(doc.BuiltInDocumentProperties[PropertyName.Version].ToInt(), Is.EqualTo(786432)); // 12.0000

            Assert.That(doc.DocPr.ViewOptions.ZoomPercent, Is.EqualTo(149));
            Assert.That(doc.CompatibilityOptions.MswVersion, Is.EqualTo(MsWordVersionCore.Word2013));
        }

        private static void CheckStyles(Document doc)
        {
            Style style = doc.Styles.GetByName("MyCustomStyle_11", false);
            Assert.That(style.ParaPr.LeftIndent, Is.EqualTo(567));
        }

        private static void CheckSectPr(Section sect, int topMargin, int leftMargin, int rightMargin, int headerDistance)
        {
            SectPr sectPr = sect.SectPr;

            Assert.That(sectPr.TopMargin, Is.EqualTo(topMargin));
            Assert.That(sectPr.LeftMargin, Is.EqualTo(leftMargin));
            Assert.That(sectPr.RightMargin, Is.EqualTo(rightMargin));
            Assert.That(sectPr.HeaderDistance, Is.EqualTo(headerDistance));
            Assert.That(ReferenceEquals(((Document)sect.Document).LastSection, sect), Is.True);
        }
    }
}
