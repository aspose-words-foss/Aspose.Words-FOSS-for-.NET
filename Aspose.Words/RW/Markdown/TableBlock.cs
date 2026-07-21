// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2020 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Table block.
    /// </summary>
    internal class TableBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!mDelimiterRow.TryParse(txtLine, start))
                return false;

            // The table must have at least header and delimiter rows.
            // Until they are recognized, lets exclude table block from processing.
            Action = BlockAction.Exclude;

            return true;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Paragraph:
                {
                    Debug.Assert(((ParagraphBlock)block).ContentLines.Count == 1);
                    RowBlock row = new RowBlock(((ParagraphBlock)block).ContentLines[0]);
                    if (row.IsEmpty)
                        return false;

                    Add(row);
                    return true;
                }
                case BlockType.Row:
                {
                    Add((RowBlock)block);
                    return true;
                }
                case BlockType.EndOfLine:
                {
                    Add(block).OpenParents(BlockType.Quote);
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns true, if a specified paragraph block can constitute the valid header row for this table.
        /// </summary>
        internal bool IsValid(ParagraphBlock paragraphBlock)
        {
            if (paragraphBlock.ContentLines.Count == 0)
                return false;

            // Table can be inside a Quote, but it has some restrictions.
            if (paragraphBlock.IsInQuote)
            {
                // The paragraph can not be multiline.
                if (paragraphBlock.ContentLines.Count > 1)
                    return false;

                // All rows must be at the same Quote level.
                if (paragraphBlock.GetParent(BlockType.Quote, true) != null)
                    return false;
            }

            string text = paragraphBlock.ContentLines[paragraphBlock.ContentLines.Count - 1];

            // The number of cols must be equal.
            if (mDelimiterRow.Count != RowBlock.Parse(text).Count)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return (c == CellBlock.SeparatorChar ||
                    c == CellBlock.ContentChar ||
                    c == CellBlock.AlignmentChar ||
                    c == ' ');
        }

        /// <summary>
        /// Gets total length of all block parts.
        /// </summary>
        internal override int Length
        {
            get { return mDelimiterRow.Length; }
        }

        /// <summary>
        /// Gets block indentation.
        /// </summary>
        internal override int Indentation
        {
            get { return mDelimiterRow.Indentation; }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Table; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }

        /// <summary>
        /// Gets delimiter row.
        /// </summary>
        internal RowBlock DelimiterRow
        {
            get { return mDelimiterRow; }
        }

        /// <summary>
        /// Gets total number of columns in the table.
        /// </summary>
        internal int ColumnsCount
        {
            get { return mDelimiterRow.Count; }
        }

        private readonly RowBlock mDelimiterRow = new RowBlock();
    }
}
