// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a EndOfLine block.
    /// </summary>
    internal class EndOfLineBlock : Block
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return (start == txtLine.Length);
        }

        /// <summary>
        /// Tries to append a specified block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.EndOfLine:
                {
                    // When we try to append one EOL to another and there is a closed Quote,
                    // it means we have encountered an empty Quote. It has some special cases for
                    // blocks that can be `lazy continued`.
                    ContainerBlock closedQuote = GetParent(BlockType.Quote, false);
                    if (closedQuote != null)
                    {
                        IndentedCodeBlock indentedCodeBlock = closedQuote.Last as IndentedCodeBlock;
                        if (indentedCodeBlock != null)
                        {
                            indentedCodeBlock.ContentLines.Add(block.ContentLine);
                            OpenParents(BlockType.Quote);
                            return true;
                        }

                        ParagraphBlock paraBlock = (ParagraphBlock)closedQuote.GetDeepestOpenedChildBlock(BlockType.Paragraph);
                        if (paraBlock != null)
                            paraBlock.Close();

                        InvertParents(BlockType.Quote);
                    }

                    return false;
                }
                case BlockType.SetextHeading:
                {
                    // A Setext heading can be constituted only from a paragraph block.
                    if ((Parent.Type == BlockType.Paragraph) && SetextHeadingBlock.CanSetContent((ParagraphBlock)Parent))
                        block.Action = BlockAction.None;

                    return false;
                }
                case BlockType.Table:
                {
                    // The row of table can be instantiated only from a paragraph block.
                    if (Parent.Type != BlockType.Paragraph)
                        return false;

                    ParagraphBlock paragraph = (ParagraphBlock)Parent;
                    if (((TableBlock)block).IsValid(paragraph))
                    {
                        // The following allows the table to be appended to the parent
                        // paragraph (which will be converted to a row block).
                        block.Action = BlockAction.None;
                        return false;
                    }

                    // If the table is not valid due to it is inside a Quote and previous paragraph is multiline,
                    // then we need to move last added line of the paragraph out of the Quote.
                    // Otherwise, the table becomes valid.
                    if (paragraph.IsInQuote)
                    {
                        // Check if the table is at the same Quote level and paragraph is multiline.
                        QuoteBlock parentQuote = (QuoteBlock)paragraph.GetParent(BlockType.Quote);
                        int lastContentLineIndex = paragraph.ContentLines.Count - 1;
                        if (!parentQuote.IsOpened && (lastContentLineIndex > 0))
                        {
                            parentQuote.CloseChildren();
                            // We should move out of the Quote the last content line
                            // to disallow it to constitute a table (if we here, then this is not valid).
                            ContainerBlock containerBlock = paragraph.GetTopmostParent(BlockType.Quote, false).Parent;

                            // Create new paragraph and move there last content line of previous paragraph.
                            ParagraphBlock newParagraph = new ParagraphBlock();
                            newParagraph.TryParse(paragraph.ContentLines[lastContentLineIndex], 0);
                            containerBlock.Add(newParagraph).Close();
                            paragraph.ContentLines.RemoveAt(lastContentLineIndex);

                            // All parent Quotes should be repeated to place a new paragraph (that will be
                            // parsed from this not valid table) into an appropriate nesting level of Quote.
                            ContainerBlock curQuote = parentQuote;
                            while (curQuote != null)
                            {
                                containerBlock = (ContainerBlock)containerBlock.Add(new QuoteBlock());
                                curQuote = curQuote.GetParent(BlockType.Quote, false);
                            }
                        }
                    }

                    return false;
                }
                case BlockType.Quote:
                {
                    // Count a nesting level of the quote being appended.
                    QuoteBlock quote = (QuoteBlock)GetTopmostParent(BlockType.Quote, true);
                    if (quote != null)
                    {
                        quote.Close();

                        // To be the same Quote there should not be any other parent blocks (for example, List blocks).
                        Block parent = quote.Parent;
                        if ((parent.Type == BlockType.Quote) && parent.IsOpened)
                        {
                            quote.CloseChildren();
                            return false;
                        }

                        if (((parent.Type == BlockType.BulletListItem) || (parent.Type == BlockType.OrderedListItem)) &&
                        (parent.Action != BlockAction.ReparseListItem))
                        {
                            quote.CloseChildren();
                            return false;
                        }

                        return true;
                    }

                    OpenParents(BlockType.Quote);
                    return false;
                }
                default:
                {
                    // We invert states of parent Quotes to allow append blocks
                    // into an appropriate nested Quote level, if any.
                    InvertParents(BlockType.Quote);

                    // Also close all child blocks of Quote, except of paragraph as it can be lazy continued.
                    ContainerBlock closedQuote = GetTopmostParent(BlockType.Quote, false);
                    if (closedQuote != null)
                        closedQuote.CloseChildrenNonInclusive(BlockType.Paragraph);

                    // IN: This is a very simplified processing till we are not handling complex blocks nesting,
                    // such as Quotes or Lists with Footnotes. Actually, I think we need some processing in MarkdownDocument
                    // similar to MarkdownDocument.TryReparseAsListItem().
                    if ((block.Type == BlockType.Paragraph) && (Parent.Type == BlockType.FootnoteDefinition))
                        Parent.Close();

                    return false;
                }
            }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.EndOfLine; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.None; }
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of indentation characters.
        /// </summary>
        protected override bool IsIndentationChar(char c)
        {
            return false;
        }
    }
}
