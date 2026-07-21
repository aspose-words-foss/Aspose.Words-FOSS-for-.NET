// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestBuildingBlocks
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestCreate()
        {
            Document doc = new Document();
            Assert.That(doc.GlossaryDocument, Is.Null);

            GlossaryDocument glossary = new GlossaryDocument();
            doc.GlossaryDocument = glossary;

            BuildingBlock block = new BuildingBlock(glossary);
            glossary.AppendChild(block);

            Assert.That(block.Name, Is.EqualTo("(Empty Name)"));
            Assert.That(block.Category, Is.EqualTo("(Empty Category)"));
            Assert.That(block.Behavior, Is.EqualTo(BuildingBlockBehavior.Content));

            block.Name = "Test1";
            block.Gallery = BuildingBlockGallery.QuickParts;
            block.Category = "General";
            block.Behavior = BuildingBlockBehavior.Page;

            Section section = new Section(glossary);
            block.AppendChild(section);

            Body body = new Body(glossary);
            section.AppendChild(body);

            Paragraph para = new Paragraph(glossary);
            body.AppendChild(para);

            Run run = new Run(glossary, "Hello!");
            para.AppendChild(run);

            // Check after open save open.
            doc = TestUtil.SaveOpen(doc, @"Model\BuildingBlocks\TestCreate.dotx");
            glossary = doc.GlossaryDocument;
            block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 0, true);
            Assert.That(block.GetText(), Is.EqualTo("Hello!\x000c"));
        }

        [Test]
        public void TestNoEmptyName()
        {
            try
            {
                Document doc = new Document();
                doc.GlossaryDocument = new GlossaryDocument();
                BuildingBlock block = new BuildingBlock(doc.GlossaryDocument);
                block.Name = "";
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message.IndexOf("cannot be null or empty string") > 0, Is.True);
            }
        }

        [Test]
        public void TestNoEmptyCategory()
        {
            try
            {
                Document doc = new Document();
                doc.GlossaryDocument = new GlossaryDocument();
                BuildingBlock block = new BuildingBlock(doc.GlossaryDocument);
                block.Category = "";
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message.IndexOf("cannot be null or empty string") > 0, Is.True);
            }
        }

        /// <summary>
        /// Open, save open and check gold plus some basic checks that glossary is a separate document
        /// and contains its styles etc.
        /// </summary>
        [Test]
        public void TestBasic()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\BuildingBlocks\TestBuildingBlocks.dotx");
            GlossaryDocument glossary = doc.GlossaryDocument;

            Assert.That(doc.GetText(), Is.EqualTo("\x000c"));
            Assert.That(glossary, IsNot.Null());
            Assert.That(doc != (DocumentBase)glossary, Is.True);
            Assert.That(doc.FontInfos != glossary.FontInfos, Is.True);
            Assert.That(doc.Styles != glossary.Styles, Is.True);
            Assert.That(doc.Styles[0] != glossary.Styles[0], Is.True);
            Assert.That(glossary.Styles[0].Document, Is.SameAs(glossary));
        }

        [Test]
        public void TestInline()
        {
            Document doc = TestUtil.Open(@"Model\BuildingBlocks\TestBuildingBlocks.dotx");
            GlossaryDocument glossary = doc.GlossaryDocument;

            // Check an inline part.
            BuildingBlock block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 0, true);
            Assert.That(block.Behavior, Is.EqualTo(BuildingBlockBehavior.Content));
            Assert.That(block.Gallery, Is.EqualTo(BuildingBlockGallery.QuickParts));
            Assert.That(block.Category, Is.EqualTo("General"));
            Assert.That(block.Name, Is.EqualTo("Simple inline"));
            Assert.That(block.Description, Is.EqualTo(""));
            Assert.That(block.Guid.ToString(), Is.EqualTo("59a69c94-f85c-470c-b306-3af54dbf86b9"));
            Assert.That(block.Style, Is.EqualTo("Normal"));
            Assert.That(block.Type, Is.EqualTo(BuildingBlockType.None));

            Assert.That(block.GetText(), Is.EqualTo("Simple inline level.\x000c"));
            Assert.That(block.GetChildNodes(NodeType.Paragraph, true).Count, Is.EqualTo(1));

            Paragraph para = (Paragraph)block.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.ParagraphStyle, Is.EqualTo(glossary.Styles["Normal"]));

            // Although this is just a run block we have a paragraph and a section in the model.
            Assert.That(block.Sections.Count, Is.EqualTo(1));
            Assert.That(block.FirstSection.Body.Paragraphs.Count, Is.EqualTo(1));

            Assert.That(glossary.GetBuildingBlock(BuildingBlockGallery.QuickParts, "General", "Simple inline"), Is.EqualTo(block));
            Assert.That(glossary.GetBuildingBlock(BuildingBlockGallery.QuickParts, "General", "simple inline"), Is.EqualTo(null));
        }


        [Test]
        public void TestParagraph()
        {
            Document doc = TestUtil.Open(@"Model\BuildingBlocks\TestBuildingBlocks.dotx");
            GlossaryDocument glossary = doc.GlossaryDocument;

            // Check a paragraph part that contains a comment, endnote, list numbering and a bookmark.
            BuildingBlock block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 1, true);
            Assert.That(block.Behavior, Is.EqualTo(BuildingBlockBehavior.Paragraph));
            Assert.That(block.Gallery, Is.EqualTo(BuildingBlockGallery.CustomQuickParts));
            Assert.That(block.Category, Is.EqualTo("TestCategory"));

            Paragraph para = (Paragraph)block.GetChild(NodeType.Paragraph, 0, true);
            Assert.That(para.GetText(), Is.EqualTo("A quick part with a comment\x0005Comment to a quick part.\r, endnote\x0002 Endnote to a quick part.\r,\r"));

            Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(0));
            Assert.That(glossary.Range.Bookmarks.Count, Is.EqualTo(1));
            Assert.That(glossary.Range.Bookmarks["bmk1"].Text, Is.EqualTo("bookmark"));

            // The selection is only two paragraphs, but the second paragraph includes the paragraph mark,
            // therefore there is one empty paragraph at the end according to the OOXML spec.
            Assert.That(block.Sections.Count, Is.EqualTo(1));
            Assert.That(block.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));

            Assert.That(glossary.GetBuildingBlock(BuildingBlockGallery.CustomQuickParts, "TestCategory", "QuickPartWithCommentEtc"), Is.EqualTo(block));
        }

        [Test]
        public void TestSection()
        {
            Document doc = TestUtil.Open(@"Model\BuildingBlocks\TestBuildingBlocks.dotx");
            GlossaryDocument glossary = doc.GlossaryDocument;

            // Check a block with one section break and text in two sections.
            BuildingBlock block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 3, true);

            Assert.That(block.GetText(), Is.EqualTo("Building Block with a Section Break.\r\x000cNext section.\r\x000c"));

            // Two sections it is correct. Last section contains text in one paragraph
            // and it also contains one dummy paragraph mark according to the OOXML spec.
            Assert.That(block.Sections.Count, Is.EqualTo(2));
            Assert.That(block.Sections[0].Body.Paragraphs.Count, Is.EqualTo(2));
            Assert.That(block.Sections[1].Body.Paragraphs.Count, Is.EqualTo(2));

            PageSetup ps = block.Sections[0].PageSetup;
            Assert.That(ps.TextColumns.Count, Is.EqualTo(2));

            ps = block.Sections[1].PageSetup;
            Assert.That(ps.TextColumns.Count, Is.EqualTo(1));
        }

        [Test]
        public void TestSectionWithHeader()
        {
            Document doc = TestUtil.Open(@"Model\BuildingBlocks\TestBuildingBlocks.dotx");
            GlossaryDocument glossary = doc.GlossaryDocument;

            // Check a block with one section, a section break at the end and a header.
            BuildingBlock block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 4, true);

            Assert.That(block.GetText(), Is.EqualTo("Header in Building Block\rSection with a Header in a Building Block\x000c\x000c"));

            // There is only one section in the building block in the document, but because
            // the section break is the last char, MS Word adds one more paragraph break according
            // to the OOXML spec and that is stored as yet another section in our model.
            Assert.That(block.Sections.Count, Is.EqualTo(2));
            Assert.That(block.Sections[0].Body.Paragraphs.Count, Is.EqualTo(1));
            Assert.That(block.Sections[1].Body.Paragraphs.Count, Is.EqualTo(1));

            HeaderFooter header = block.Sections[0].HeadersFooters[HeaderFooterType.HeaderPrimary];
            Assert.That(header.GetText(), Is.EqualTo("Header in Building Block\r"));
        }

        /// <summary>
        /// This is a file that comes with MS Word 2007. Has lots of blocks, lets check we can read it okay.
        /// </summary>
        [Test]
        public void TestBuiltIn()
        {
            Document doc = TestUtil.Open(@"Model\BuildingBlocks\TestBuiltIn.dotx");

            GlossaryDocument glossary = doc.GlossaryDocument;

            BuildingBlock block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 2, true);
            Assert.That(block.Behavior, Is.EqualTo(BuildingBlockBehavior.Page));
            Assert.That(block.Gallery, Is.EqualTo(BuildingBlockGallery.CoverPage));
            Assert.That(block.Category, Is.EqualTo("Built-In"));
            Assert.That(block.Name, Is.EqualTo("Conservative"));
            Assert.That(block.Description, Is.EqualTo("Top-aligned information block with accent line between title and subtitle; bottom-aligned abstract"));
            Assert.That(block.Style, Is.EqualTo("No Spacing"));
            Assert.That(block.Type, Is.EqualTo(BuildingBlockType.None));

            block = (BuildingBlock)glossary.GetChild(NodeType.BuildingBlock, 3, true);
            Assert.That(block.Behavior, Is.EqualTo(BuildingBlockBehavior.Content));
            Assert.That(block.Gallery, Is.EqualTo(BuildingBlockGallery.StructuredDocumentTagPlaceholderText));
            Assert.That(block.Category, Is.EqualTo("General"));
            Assert.That(block.Name, Is.EqualTo("3A5B8D0E64CA4985BBFCEFDF165F36CC"));
            Assert.That(block.Description, Is.EqualTo(""));
            Assert.That(block.Style, Is.EqualTo(""));
            Assert.That(block.Type, Is.EqualTo(BuildingBlockType.StructuredDocumentTagPlaceholderText));
        }

        /// <summary>
        /// RK This is supposed to be a method available in the public API later to allow easy insertion of building blocks.
        /// Need to finish this.
        /// </summary>
        /// <param name="dstContainer">The container node where the insertion takes place.</param>
        /// <param name="srcBlock">The building block that is being inserted.</param>
        /// <param name="dstRefNode">The reference node (cursor) before or after which the
        /// insertion takes place. If not null, must be a child of the container.</param>
        /// <param name="isAfter">True to insert after the cursor, false to insert before.</param>
        private static void InsertSmart(
            CompositeNode dstContainer,
            BuildingBlock srcBlock,
            Node dstRefNode,
            bool isAfter)
        {
            if (dstContainer == null)
                throw new ArgumentNullException("container");
            if (srcBlock == null)
                throw new ArgumentNullException("block");

            if (dstContainer.Document == srcBlock.Document)
                throw new ArgumentException("The block to be inserted is expected to be from a different document.");

            if ((dstRefNode != null) && (dstRefNode.ParentNode != dstContainer))
                throw new ArgumentException("The reference node should be a child of the container node.");

            // We need both nodes before and after the cursor because we are trying to mostly
            // use CompositeNode.InsertAfter because currently it is faster than InsertBefore.
            // When later InsertBefore is improved, then we can delete this code and use
            // both InsertAfter and InsertBefore where appropriate.
            Node nodeAfterCursor;
            Node nodeBeforeCursor;
            if (isAfter)
            {
                nodeBeforeCursor = dstRefNode;
                nodeAfterCursor = (dstRefNode == null) ? dstContainer.FirstChild : dstRefNode.NextSibling;
            }
            else
            {
                nodeAfterCursor = dstRefNode;
                nodeBeforeCursor = (dstRefNode == null) ? dstContainer.LastChild : dstRefNode.PreviousSibling;
            }

            NodeImporter importer = new NodeImporter(srcBlock.Document, dstContainer.Document, ImportFormatMode.KeepSourceFormatting);
            foreach (Node srcBlockChild in srcBlock)
            {
                // This clones the building block nodes.
                Section section = (Section)importer.ImportNode(srcBlockChild, true);

                Node bodyChild = section.Body.FirstChild;
                while (bodyChild != null)
                {
                    // This has to be cached now because we move, not clone building block nodes.
                    Node nextBodyChild = bodyChild.NextSibling;

                    switch (bodyChild.NodeType)
                    {
                        case NodeType.Paragraph:
                        {
                            Paragraph para = (Paragraph)bodyChild;

                            if (para.IsLastChild)
                            {
                                // For the last paragraph we do not need the paragraph itself.
                                // We just move all inline nodes from it to be before the cursor.
                                dstContainer.InsertAfter(para.FirstChild, null, nodeBeforeCursor);
                            }
                            else
                            {
                                // This has to be cached before we move the paragraph.
                                bool isFirstPara = para.IsFirstChild;

                                // Just move the paragraph to be before the destination para.
                                dstContainer.InsertPrevious(para);

                                if (isFirstPara)
                                {
                                    // For the first paragraph move all inlines from the destination para
                                    // up to the cursor to the beginning of the first para.
                                    para.InsertAfter(dstContainer.FirstChild, nodeAfterCursor, null);
                                }
                            }

                            break;
                        }
                        default:
                            throw new InvalidOperationException("Unexpected node type.");
                    }

                    bodyChild = nextBodyChild;
                }
            }
        }




        /// <summary>
        /// WORDSNET-28455 InvalidOperationException upon saving document after appending text to tables in a Building Block
        /// The error occurred in the <see cref="ParagraphMeasurer"/> class because <see cref="Node.FetchDocument"/>
        /// was used for a node of a glossary document. Now it has been changed to calling the method
        /// <see cref="Node.FetchDocumentOrGlossaryMain"/>.
        /// </summary>
        [Test]
        public void Test28455()
        {
            Document doc = new Document();
            GlossaryDocument glossaryDoc = new GlossaryDocument();
            doc.GlossaryDocument = glossaryDoc;

            const string buildingBlockName = "ProductList";

            BuildingBlock block = new BuildingBlock(glossaryDoc)
            {
                Name = buildingBlockName,
                Gallery = BuildingBlockGallery.QuickParts,
                Category = "General",
                Behavior = BuildingBlockBehavior.Paragraph,
            };
            glossaryDoc.AppendChild(block);

            Section section = new Section(glossaryDoc);
            block.Sections.Add(section);

            Body body = new Body(glossaryDoc);
            section.AppendChild(body);

            body.AppendParagraph("AAA");

            var newTable = new Table(glossaryDoc);
            body.AppendChild(newTable);
            newTable.EnsureMinimum();

            var headerCell1 = newTable.FirstRow.FirstCell;
            headerCell1.EnsureMinimum();

            var headerCell2 = new Cell(glossaryDoc);
            newTable.FirstRow.Cells.Add(headerCell2);
            headerCell2.EnsureMinimum();

            newTable.StyleIdentifier = StyleIdentifier.ListTable4Accent1;

            headerCell1.FirstParagraph.AppendChild(new Run(glossaryDoc, "Header 1"));
            headerCell2.FirstParagraph.AppendChild(new Run(glossaryDoc, "Header 2"));

            // Just check that no exception occurs.
            doc = TestUtil.SaveOpen(doc, @"Model\Markup\Test28455.dotx", null, false);

            block = (BuildingBlock)doc.GlossaryDocument.GetChild(NodeType.BuildingBlock, 0, false);
            Assert.That(block.Name, Is.EqualTo(buildingBlockName));
        }

        // todo 0 normal_0 font size 1638 !!!

        // todo 0 maybe allow insertion without passing the container?

        // todo 0 tables

        // block has only one para or multiple
        // last para is empty or not
        // insertion point at beginning of para/in the middle/end of para

        // block has only one section or multiple
        // last section consists of one empty para or more

        // insert a block containing a table

        // insert in a table, shape, comment, footnote, headerfooter
    }
}
