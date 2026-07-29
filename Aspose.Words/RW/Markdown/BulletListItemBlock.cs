// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/10/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown BulletListItem block.
    /// </summary>
    internal class BulletListItemBlock : ListItemBlock
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

            // The opening sequence MUST be followed with at least one whitespace character,
            // or the block should be entirely empty.
            if ((start + Opening.Length) < txtLine.Length)
            {
                char lastOpeningChar = Opening[Opening.Length - 1];
                if (!char.IsWhiteSpace(lastOpeningChar))
                    return false;
            }

            // Initially, list item block is awaiting another blocks to be parsed as its content.
            Action = BlockAction.ReparseListItem;
            SetMarker(MarkdownUtil.ToListMarker(Opening[0]));
            
            return true;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return ((c == '-') || (c == '+') || (c == '*'));
        }

        /// <summary>
        /// Gets length of the list label.
        /// </summary>
        internal override int ListLabelLength
        {
            get
            {
                // The opening length is always greater or equal to 2 characters:
                // the first one is a bullet marker('-', '+', '*') and the second one is space or tab.
                // Even if the list item is completely empty (i.e. has only bullet marker), the spec editor
                // at https://spec.commonmark.org/dingus/ parses it as two characters.
                int length = OpeningIndentationLength + System.Math.Max(2, OpeningLength);

                // We need to consider also indentation of the first content block,
                // if it is not IndentedCode (see rule #2 of the spec).
                if (!IsEmpty && (this[0].Type != BlockType.IndentedCode))
                    length += this[0].Indentation;

                return length;
            }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.BulletListItem; }
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
