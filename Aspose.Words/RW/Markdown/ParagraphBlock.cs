// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a Markdown Paragraph block.
    /// </summary>
    internal class ParagraphBlock : InlineContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            ContentLines.Add(ContentLine);

            return true;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.SetextHeading:
                {
                    // If content was successfully set to a SetextHeading block, then this paragraph is actually
                    // a content of the SetextHeading and thus should be removed. Otherwise, this is not
                    // a SetextHeading and we should try another block types.
                    ((SetextHeadingBlock)block).SetContent(this);
                    Action = BlockAction.Remove;

                    OpenParentQuotes();

                    // The SetextHeading is still not added to a tree.
                    return false;
                }
                case BlockType.Table:
                {
                    Debug.Assert(ContentLines.Count > 0);

                    string text = ContentLines[ContentLines.Count - 1];
                    ((TableBlock)block).Add(new RowBlock(text));
                    ContentLines.RemoveAt(ContentLines.Count - 1);
                    if (ContentLines.Count == 0)
                        Action = BlockAction.Remove;

                    OpenParentQuotes();

                    return false;
                }
                case BlockType.OrderedListItem:
                {
                    // In order to be attracted to a Principle of Uniformity, the spec at https://spec.commonmark.org/
                    // says, that: "In order to solve of unwanted lists in paragraphs with hard-wrapped numerals, we allow
                    // only lists starting with 1 to interrupt paragraphs."
                    OrderedListItemBlock listItem = (OrderedListItemBlock)block;
                    if (listItem.StartAt != 1)
                    {
                        ListItemBlock parentListItem =
                            (ListItemBlock)GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);
                        // The spec clause is actual for the very start of the list.
                        if ((parentListItem == null) ||
                            ((!parentListItem.IsInList) && (GetListInReparseState(parentListItem) != null)))
                        {
                            // This will allow list item to be re-parsed as some another
                            // block type to be a lazy continuation of the paragraph block.
                            Open();
                            block.Action = BlockAction.Exclude;
                        }
                    }

                    return false;
                }
                default:
                    return base.TryAppend(block);
            }
        }

        /// <summary>
        /// Gets an indentation part from a left side of a text line starting at a specified position.
        /// </summary>
        protected override string GetLeftIndentation(string txtLine, int start)
        {
            return "";
        }

        /// <summary>
        /// Gets an indentation part from a right side of a text line starting at the end
        /// of text up to a specified start position.
        /// </summary>
        protected override string GetRightIndentation(string txtLine, int start)
        {
            return "";
        }

        /// <summary>
        /// Gets an opening part from a text line starting at a specified position.
        /// </summary>
        protected override string GetOpening(string txtLine, int start)
        {
            return "";
        }

        /// <summary>
        /// Gets a closing part from a text line starting at a specified position.
        /// </summary>
        protected override string GetClosing(string txtLine, int start, int end)
        {
            return "";
        }

        /// <summary>
        /// Returns parent list that is in 'Reparse' state.
        /// </summary>
        private static ListItemBlock GetListInReparseState(Block startBlock)
        {
            ListItemBlock listItem = (startBlock.HasType(BlockType.BulletListItem, BlockType.OrderedListItem))
                ? (ListItemBlock)startBlock
                : (ListItemBlock)startBlock.GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);

            while (listItem != null)
            {
                if (listItem.Action == BlockAction.ReparseListItem)
                    return listItem;

                listItem = (ListItemBlock)listItem.GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);
            }

            return null;
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Paragraph; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Leaf; }
        }

        /// <summary>
        /// Gets block indentation.
        /// </summary>
        internal override int Indentation
        {
            get { return ContentLineIndentationLength; }
        }
    }
}
