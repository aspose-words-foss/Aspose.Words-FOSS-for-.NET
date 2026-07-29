// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2019 by Ilya Navrotskiy

using System;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// The factory class that helps to create markdown blocks.
    /// </summary>
    internal static class MarkdownBlockFactory
    {
        /// <summary>
        /// Creates a markdown Block object of a specified type.
        /// </summary>
        internal static Block Create(BlockType type)
        {
            switch (type)
            {
                case BlockType.Paragraph:
                    return new ParagraphBlock();
                case BlockType.EndOfLine:
                    return new EndOfLineBlock();
                case BlockType.AtxHeading:
                    return new AtxHeadingBlock();
                case BlockType.SetextHeading:
                    return new SetextHeadingBlock();
                case BlockType.BlankLine:
                    return new BlankLineBlock();
                case BlockType.Quote:
                    return new QuoteBlock();
                case BlockType.Table:
                    return new TableBlock();
                case BlockType.Row:
                    return new RowBlock();
                case BlockType.Cell:
                    return new CellBlock();
                case BlockType.HorizontalRule:
                    return new HorizontalRuleBlock();
                case BlockType.IndentedCode:
                    return new IndentedCodeBlock();
                case BlockType.FencedCode:
                    return new FencedCodeBlock();
                case BlockType.FootnoteDefinition:
                    return new FootnoteDefinitionBlock();
                case BlockType.BulletListItem:
                    return new BulletListItemBlock();
                case BlockType.OrderedListItem:
                    return new OrderedListItemBlock();
                case BlockType.LinkDefinition:
                    return new LinkDefinitionBlock();
                default:
                    throw new InvalidOperationException(string.Format("Invalid block type: {0}", type));
            }
        }
    }
}
