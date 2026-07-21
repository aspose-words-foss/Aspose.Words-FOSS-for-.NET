// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/04/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Inline block.
    /// </summary>
    internal class InlineBlock : Block
    {
        /// <summary>
        /// Creates InlineBlock object with a specified text.
        /// </summary>
        internal InlineBlock(string text)
        {
            Parse(text, 0);
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return Parse(txtLine, start);
        }
        
        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            return false;
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
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Inline; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }
    }
}
