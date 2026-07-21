// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2016 by Ilya Navrotskiy

using System;
using Aspose.Words.Tables;
using Aspose.Words.Tests.DocBuilder.InsertDocument;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests options of the <see cref="ImportFormatOptions"/> class.
    /// </summary>
    public class TestImportFormatOptions : TestInsertDocumentBase
    {


        /// <summary>
        /// Tests that when 'AdjustSentenceAndWordSpacing' option is enabled and content being inserted ends with SDT,
        /// Word does not merge lists even 'MergePastedLists' option is enabled.
        /// </summary>
        [Test]
        public void TestMergeListsLastSdt()
        {
            DocumentBuilder builder = OpenDstDocument(TestInsertDstDocLocation.BeginningOfListItem);
            Document srcDoc = OpenSrcDocument(TestInsertSrcDocType.BlockLevelSdtDocStartsWithPara);

            // Source document ends with SDT.
            Assert.That(srcDoc.LastSection.Body.LastChild.NodeType, Is.EqualTo(NodeType.StructuredDocumentTag));

            ImportFormatOptions options = new ImportFormatOptions();
            options.MergePastedLists = true;

            // Enable 'AdjustSentenceAndWordSpacing' option.
            options.AdjustSentenceAndWordSpacing = true;
            Paragraph para = (Paragraph)builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles, options);
            Assert.That(para.IsListItem, Is.False);

            builder = OpenDstDocument(TestInsertDstDocLocation.BeginningOfListItem);

            // Disable 'AdjustSentenceAndWordSpacing' option.
            options.AdjustSentenceAndWordSpacing = false;
            para = (Paragraph)builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles, options);
            Assert.That(para.IsListItem, Is.True);

            // Append paragraph to the end of source document and check that 'AdjustSentenceAndWordSpacing' option does
            // not longer affect on how lists are merged.
            srcDoc.LastSection.Body.AppendParagraph("some text");

            builder = OpenDstDocument(TestInsertDstDocLocation.BeginningOfListItem);

            // Enable 'AdjustSentenceAndWordSpacing' option.
            options.AdjustSentenceAndWordSpacing = true;
            para = (Paragraph)builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles, options);
            Assert.That(para.IsListItem, Is.True);

            builder = OpenDstDocument(TestInsertDstDocLocation.BeginningOfListItem);

            // Disable 'AdjustSentenceAndWordSpacing' option.
            options.AdjustSentenceAndWordSpacing = false;
            para = (Paragraph)builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles, options);
            Assert.That(para.IsListItem, Is.True);
        }

        /// <summary>
        /// Tests that source content that starts with a paragraph will be merged with a surrounding list only
        /// if it is inserted at the very beginning of a list item.
        /// </summary>
        [Test]
        public void TestMergeListsParagraph()
        {
            ImportFormatOptions options = new ImportFormatOptions();
            options.MergePastedLists = true;

            foreach (TestInsertDstDocLocation location in ListItemLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.OneParaDoc, location, options);

                // Only inserting at the beginning of list item merges inserted paragraph with a list.
                bool isListItem = (location == TestInsertDstDocLocation.BeginningOfListItem);
                Assert.That(FirstInsertedPara.IsListItem, Is.EqualTo(isListItem));
            }
        }

        /// <summary>
        /// Tests that source content that starts with a table will be merged with a surrounding list
        /// if it is inserted at any place inside list item. Also, only very first paragraph of inserted table is merged
        /// and all other not.
        /// </summary>
        [Test]
        public void TestMergeListsTable()
        {
            ImportFormatOptions options = new ImportFormatOptions();
            options.MergePastedLists = true;

            // Inserting at any place of list item merges first paragraph of inserted table with this list.
            foreach (TestInsertDstDocLocation location in ListItemLocations)
            {
                TestInsertDocument(TestInsertSrcDocType.TableDocStartsWithTable, location, options);

                Assert.That(FirstInsertedPara.IsListItem, Is.True);

                Cell secondCell = FirstInsertedPara.ParentCell.NextCell;
                Assert.That(secondCell.FirstParagraph.IsListItem, Is.False);
            }
        }



        /// <summary>
        /// WORDSNET-24960 Allow ImportFormatOptions.AdjustSentenceAndWordSpacing option in API.
        /// Changed access to <see cref="ImportFormatOptions.AdjustSentenceAndWordSpacing"/> option to public.
        /// </summary>
        [TestCase(true, ' ')]
        [TestCase(false, '.')]
        public void Test24960(bool isAdjust, char expectedChar)
        {
            // Check the option is exposed publicly.
#if !JAVA && !CPLUSPLUS
            Type importFormatOptions = typeof(ImportFormatOptions);
            Assert.That(importFormatOptions.IsPublic, Is.True);
            Assert.That(importFormatOptions.GetMember("AdjustSentenceAndWordSpacing"), IsNot.Empty());
#endif

            // When the option is enabled we have to insert ' ' before inserted content.
            Document srcDoc = new Document();
            Document dstDoc = new Document();

            DocumentBuilder builder = new DocumentBuilder(srcDoc);
            builder.Write("Dolor sit amet.");

            builder = new DocumentBuilder(dstDoc);
            builder.Write("Lorem ipsum.");

            ImportFormatOptions options = new ImportFormatOptions() { AdjustSentenceAndWordSpacing = isAdjust };
            Node node = builder.InsertDocument(srcDoc, ImportFormatMode.UseDestinationStyles, options);
            string text = node.GetText();

            int insertedIdx = text.IndexOf("Dolor sit amet.", StringComparison.InvariantCulture);
            Assert.That(text[insertedIdx - 1], Is.EqualTo(expectedChar));
        }

        /// <summary>
        /// A core method for testing KeepSourceNumbering importing option in <see cref="TestJira17534"/>.
        /// </summary>
        private static void TestKeepSourceNumbering(ImportFormatOptions importFormatOptions, string[] expectedListLabels)
        {
            Document srcDoc = TestUtil.Open(@"Model\Nodes\TestJira17534Src.docx");
            Document dstDoc = TestUtil.Open(@"Model\Nodes\TestJira17534Dst.docx");

            NodeImporter importer = new NodeImporter(srcDoc, dstDoc, ImportFormatMode.KeepSourceFormatting, importFormatOptions);

            ParagraphCollection srcParas = srcDoc.FirstSection.Body.Paragraphs;
            foreach (Paragraph srcPara in srcParas)
            {
                Node importedNode = importer.ImportNode(srcPara, false);
                dstDoc.FirstSection.Body.AppendChild(importedNode);
            }

            dstDoc.UpdateListLabels();

            ParagraphCollection dstParas = dstDoc.FirstSection.Body.Paragraphs;
            for (int i = 0; i < expectedListLabels.Length; i++)
                Assert.That(dstParas[i].ListLabel.LabelString, Is.EqualTo(expectedListLabels[i]));
        }
    }
}
