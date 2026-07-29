// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Bullet or Ordered ListItem block.
    /// </summary>
    internal abstract class ListItemBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.SetextHeading:
                {
                    // If SetextHeading has no any content, then we still can't say with certainty that this is
                    // actually a heading. For example, it can be a HorizontalRule or just a regular Paragraph.
                    if (((SetextHeadingBlock)block).ContentLines.Count > 0)
                        Add(block);

                    return true;
                }
                case BlockType.BlankLine:
                {
                    // Blank line closes an empty list item.
                    if (IsEmpty || (this[0].Type == BlockType.EndOfLine))
                        return false;

                    // We don't want blank lines in our markdown tree, so just add an EOL.
                    Add(new EndOfLineBlock()).OpenParents(BlockType.Quote);
                    return true;
                }
                case BlockType.EndOfLine:
                {
                    // This is a special case: an empty bullet list cannot interrupt paragraph of the same level.
                    // So this bullet list item should be converted to the paragraph continuation.
                    if (IsEmpty)
                    {
                        ParagraphBlock prevParagraphBlock = PreviousSibling as ParagraphBlock;
                        if (prevParagraphBlock != null)
                        {
                            prevParagraphBlock.Open();
                            prevParagraphBlock.ContentLines.Add(Opening);

                            Action = BlockAction.Remove;
                            return true;
                        }
                    }

                    Add(block);
                    return true;
                }
                case BlockType.Table:
                {
                    return false;
                }
                default:
                {
                    // Everything can be added to the list item when it is in an appropriate state.
                    if (Action == BlockAction.ReparseListItem)
                    {
                        // This list item can be inside a Quote, so it might be closed
                        // to account its nesting level and we should open it now.
                        OpenParents(BlockType.Quote);

                        Add(block);
                        return true;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Gets list item level.
        /// </summary>
        internal int GetLevel()
        {
            int level = 0;
            Block parentListItem = GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);
            while (parentListItem != null)
            {
                level++;
                parentListItem = parentListItem.GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);
            }

            return level;
        }

        /// <summary>
        /// Returns a block that contains a list to which this list item belongs.
        /// </summary>
        internal ContainerBlock GetListContainer()
        {
            ListItemBlock firstBlockOfList;

            ListItemBlock curListItem = this;
            do
            {
                firstBlockOfList = curListItem;

                // If a list item is a first child, then this list item starts a new list.
                if (curListItem.IsFirstChild)
                    break;

                curListItem = (ListItemBlock)curListItem.GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);
            }
            while (curListItem != null);

            return firstBlockOfList.Parent;
        }

        /// <summary>
        /// Gets an opening part from a text line starting at a specified position.
        /// </summary>
        protected override string GetOpening(string txtLine, int start)
        {
            string opening = base.GetOpening(txtLine, start);
            if (opening.Length > 0)
            {
                // There can be one whitespace character after main opening sequence.
                int whitespaceIdx = start + opening.Length;
                if (whitespaceIdx < txtLine.Length)
                {
                    char c = txtLine[whitespaceIdx];
                    if (char.IsWhiteSpace(c))
                        opening = string.Format("{0}{1}", opening, c);
                }
            }

            return opening;
        }

        /// <summary>
        /// Gets a content part from a text line starting at a specified start position up to a end position.
        /// </summary>
        protected override string GetContent(string txtLine, int start, int end)
        {
            // ListItem does not have any own text as the content.
            // Instead, it contains other container blocks that constitute its content.
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
        /// Gets an indentation part from a right side of a text line starting at the end
        /// of text up to a specified start position.
        /// </summary>
        protected override string GetRightIndentation(string txtLine, int start)
        {
            return "";
        }

        /// <summary>
        /// Sets list marker.
        /// </summary>
        protected void SetMarker(ListMarker marker)
        {
            mMarker = marker;
        }

        /// <summary>
        /// Sets StartAt.
        /// </summary>
        protected void SetStartAt(int startAt)
        {
            mStartAt = startAt;
        }

        /// <summary>
        /// Gets list marker.
        /// </summary>
        internal ListMarker Marker
        {
            get { return mMarker; }
        }

        /// <summary>
        /// Gets StartAt index.
        /// </summary>
        internal int StartAt
        {
            get { return mStartAt; }
        }

        /// <summary>
        /// Gets length of the list label.
        /// </summary>
        internal abstract int ListLabelLength { get; }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }

        /// <summary>
        /// Gets a boolean value indicating either this list item block starts a level.
        /// </summary>
        internal bool IsLevelStart
        {
            get
            {
                Debug.Assert(Parent != null);

                foreach (Block block in Parent)
                {
                    if (block == this)
                        break;

                    if ((block.Type == BlockType.OrderedListItem) || (block.Type == BlockType.BulletListItem))
                        return false;
                }

                return true;
            }
        }

        private ListMarker mMarker;
        private int mStartAt;
    }
}
