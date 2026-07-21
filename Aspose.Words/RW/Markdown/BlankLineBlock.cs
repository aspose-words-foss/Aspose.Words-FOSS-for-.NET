// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown blank line.
    /// </summary>
    internal class BlankLineBlock : Block
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            // Check if this is EndOfLine.
            if ((start > 0) && (start == txtLine.Length))
                return false;

            return StringUtil.ContainsOnlyWhitespaces(ContentLine);
        }

        /// <summary>
        /// Tries to append a specified block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            // The blank line is always closed, so it cannot be appended.
            return false;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of indentation characters.
        /// </summary>
        protected override bool IsIndentationChar(char c)
        {
            return false;
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.BlankLine; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.None; }
        }
    }
}
