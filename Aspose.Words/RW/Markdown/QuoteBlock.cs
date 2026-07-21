// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Quote block.
    /// </summary>
    internal class QuoteBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            if (Opening.Length == 0)
                return false;

            if (OpeningIndentationLength > MaxIndentationLength)
                return false;

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
                    // If SetextHeading has no any content, then we still can't say with certainty that this is
                    // actually a heading. For example, it can be a HorizontalRule or just a regular Paragraph.
                    if (((SetextHeadingBlock)block).ContentLines.Count > 0)
                        Add(block);

                    return true;
                }
                default:
                {
                    // Anything can be added into a Quote. But we don't want BlankLines in our tree.
                    Block blockToAppend = (block.Type == BlockType.BlankLine) ? new EndOfLineBlock() : block;
                    Add(blockToAppend).OpenParents(BlockType.Quote);
                    return true;
                }
            }
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
            // Quote does not have any own text as a content. Instead, it contains other
            // composite blocks that constitute its content.
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
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return (c == '>');
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Quote; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }

        /// <summary>
        /// Gets integer value that limits a number of opening characters to search.
        /// </summary>
        protected override int OpeningSearchLimit
        {
            get { return 1; }
        }
    }
}
