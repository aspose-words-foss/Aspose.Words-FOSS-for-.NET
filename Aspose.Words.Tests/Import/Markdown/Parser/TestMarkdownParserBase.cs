// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using System.IO;
using Aspose.Words.Loading;
using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// The base class for testing markdown features parsing.
    /// </summary>
    /// <remarks>
    /// The implementation of markdown parser conforms to CommonMark Spec: https://spec.commonmark.org/
    /// To verify correctness of markdown tree please use the corresponding tool: https://spec.commonmark.org/dingus/
    /// </remarks>
    [TestFixture]
    public abstract class TestMarkdownParserBase
    {

        internal class ExpectedInline
        {
            public ExpectedInline(BlockType blockType, string text)
            {
                Type = blockType;
                Text = text;
            }

            public BlockType Type { get; }
            public string Text { get; }
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal virtual MarkdownDocument Open(string path)
        {
            return Open(path, null);
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal virtual MarkdownDocument Open(string path, MarkdownLoadOptions loadOptions)
        {
            string fileName = TestUtil.BuildTestFileName(path);
            StreamReader reader = new StreamReader(fileName);

            MarkdownDocument doc = new MarkdownDocument(loadOptions);
            doc.Read(reader, null);

            return doc;
        }

        /// <summary>
        /// Verifies SetextHeading block.
        /// </summary>
        internal static void CheckSetextHeading(Block heading, int expectedLevel, string expectedText)
        {
            CheckInlineContainer(heading, BlockType.SetextHeading, expectedText);
            Assert.That(((HeadingBlock)heading).Level, Is.EqualTo(expectedLevel));
        }

        /// <summary>
        /// Verifies Paragraph block.
        /// </summary>
        internal static void CheckParagraph(Block paragraph, string expectedText)
        {
            CheckInlineContainer(paragraph, BlockType.Paragraph, expectedText);
        }

        /// <summary>
        /// Verifies HorizontalRule block.
        /// </summary>
        internal static void CheckHorizontalRule(Block horizontalRule)
        {
            Assert.That(horizontalRule.Type, Is.EqualTo(BlockType.HorizontalRule));
        }

        /// <summary>
        /// Verifies IndentedCode block.
        /// </summary>
        internal static void CheckIndentedCode(Block indentedCode, string expectedText)
        {
            CheckInlineContainer(indentedCode, BlockType.IndentedCode, expectedText);
        }

        /// <summary>
        /// Verifies FencedCode block.
        /// </summary>
        internal static void CheckFencedCode(Block fencedCode, string expectedText, string expectedInfoString = "")
        {
            CheckInlineContainer(fencedCode, BlockType.FencedCode, expectedText);
            Assert.That(((FencedCodeBlock)fencedCode).Info, Is.EqualTo(expectedInfoString));
        }

        /// <summary>
        /// Verifies AtxHeading block.
        /// </summary>
        internal static void CheckAtxHeading(Block heading, int expectedLevel, string expectedText)
        {
            CheckInlineContainer(heading, BlockType.AtxHeading, expectedText);
            Assert.That(((HeadingBlock)heading).Level, Is.EqualTo(expectedLevel));
        }

        /// <summary>
        /// Verifies BulletListItem block.
        /// </summary>
        internal static void CheckBulletListItem(Block bulletListItem, ListMarker expectedMarker, string expectedText)
        {
            CheckInlineContainer(bulletListItem, BlockType.BulletListItem, expectedText);
            Assert.That(((BulletListItemBlock)bulletListItem).Marker, Is.EqualTo(expectedMarker));
        }

        /// <summary>
        /// Verifies OrderedListItem block.
        /// </summary>
        internal static void CheckOrderedListItem(Block orderedListItem,
            int expectedStartAt, ListMarker expectedMarker, string expectedText)
        {
            CheckInlineContainer(orderedListItem, BlockType.OrderedListItem, expectedText);
            Assert.That(((OrderedListItemBlock)orderedListItem).Marker, Is.EqualTo(expectedMarker));
            Assert.That(((OrderedListItemBlock)orderedListItem).StartAt, Is.EqualTo(expectedStartAt));
        }

        /// <summary>
        /// Verifies LinkText block.
        /// </summary>
        internal static void CheckLinkTextBlock(Block linkStartBlock,
            string expectedDisplayText, string expectedUri, string expectedTitle = "")
        {
            LinkTextBlock linkTextBlock = (LinkTextBlock)linkStartBlock;
            Assert.That(linkTextBlock.Text, Is.EqualTo(expectedDisplayText));
            Assert.That(linkTextBlock.LinkDestinationBlock.Uri, Is.EqualTo(expectedUri));
            Assert.That(linkTextBlock.LinkDestinationBlock.Title, Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Verifies ImageDescription block.
        /// </summary>
        internal static void CheckImageBlock(
            Block imageStartBlock,
            string expectedDescription, string expectedUri, string expectedTitle = "")
        {
            ImageDescriptionBlock imageDescriptionBlock = (ImageDescriptionBlock)imageStartBlock;
            Assert.That(imageDescriptionBlock.Text, Is.EqualTo(expectedDescription));
            Assert.That(imageDescriptionBlock.LinkDestinationBlock.Uri, Is.EqualTo(expectedUri));
            Assert.That(imageDescriptionBlock.LinkDestinationBlock.Title, Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Verifies Inline blocks inside a specified InlineContainer.
        /// </summary>
        internal static void CheckInlines(Block inlineContainer, ExpectedInline[] expectedInlines)
        {
            int blockCount = expectedInlines.Length;

            Assert.That(((ContainerBlock)inlineContainer).Count, Is.EqualTo(blockCount));

            for (int i = 0; i < ((ContainerBlock)inlineContainer).Count; i++)
            {
                Block inlineBlock = ((ContainerBlock)inlineContainer)[i];
                Assert.That(inlineBlock.Type, Is.EqualTo(expectedInlines[i].Type));
                Assert.That(inlineBlock.Text, Is.EqualTo(expectedInlines[i].Text));
            }
        }

        /// <summary>
        /// Verifies single Inline block.
        /// </summary>
        internal static void CheckInline(Block inline, string expectedText)
        {
            Assert.That(inline.Type, Is.EqualTo(BlockType.Inline));
            Assert.That(inline.Text, Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Verifies Italic Inline block.
        /// </summary>
        internal static void CheckItalicInline(Block inline, string expectedText)
        {
            Assert.That(inline.Type, Is.EqualTo(BlockType.ItalicInline));
            Assert.That(inline.Text, Is.EqualTo(expectedText));
        }

        /// <summary>
        /// Verifies LinkDefinition block.
        /// </summary>
        internal static void CheckLinkDefinition(Block block, string expectedDestination, string expectedUri = " ", string expectedTitle = " ")
        {
            LinkDefinitionBlock linkDefinitionBlock = (LinkDefinitionBlock)block;
            Assert.That(linkDefinitionBlock.Destination.Trim(), Is.EqualTo(expectedDestination));
            if (expectedUri != " ")
                Assert.That(linkDefinitionBlock.LinkDestination.Uri, Is.EqualTo(expectedUri));
            if (expectedTitle != " ")
                Assert.That(linkDefinitionBlock.LinkDestination.Title, Is.EqualTo(expectedTitle));
        }

        /// <summary>
        /// Verifies InlineContainer block.
        /// </summary>
        private static void CheckInlineContainer(Block inlineContainer,
            BlockType expectedContainerBlockType, string expectedText)
        {
            Assert.That(inlineContainer.Type, Is.EqualTo(expectedContainerBlockType));
            Assert.That(inlineContainer.Text, Is.EqualTo(expectedText));
        }

        protected static readonly string SoftBreak = MarkdownUtil.SoftLineBreakChar.ToString();
        protected static readonly string HardBreakSpaces = MarkdownUtil.HardLineBreakSpacesChar.ToString();
        protected static readonly string HardBreakSlash = MarkdownUtil.HardLineBreakSlashChar.ToString();
    }
}
