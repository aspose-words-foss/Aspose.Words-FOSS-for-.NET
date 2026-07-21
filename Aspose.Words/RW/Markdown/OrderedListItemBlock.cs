// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2019 by Ilya Navrotskiy

using Aspose.Common;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown OrderedListItem block.
    /// </summary>
    internal class OrderedListItemBlock : ListItemBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            // There must be at least one digit and either '.' or ')'.
            if (Opening.Length < 2)
                return false;

            if (OpeningIndentationLength > MaxIndentationLength)
                return false;

            // The opening sequence MUST start from a digit. 
            if (!StringUtil.IsDigit(Opening[0]))
                return false;

            int markerDelimiterIndex = Opening.Length - 1;
            if (char.IsWhiteSpace(Opening[markerDelimiterIndex]))
                markerDelimiterIndex--;
            
            // The number of digits must be less than 10.
            if (markerDelimiterIndex > 9)
                return false;
            
            // The marker delimiter must be at the very end or just before ending whitespace.
            if (Opening.IndexOfAny(new char[] {'.', ')'}) != markerDelimiterIndex)
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
            SetMarker(MarkdownUtil.ToListMarker(Opening[markerDelimiterIndex]));
            SetStartAt(FormatterPal.ParseInt(Opening.Substring(0, markerDelimiterIndex)));
            
            return true;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return (StringUtil.IsDigit(c) || (c == '.') || (c == ')'));
        }

        /// <summary>
        /// Gets length of the list label.
        /// </summary>
        internal override int ListLabelLength
        {
            get
            {
                // The opening length is always greater or equal to 3 characters:
                // the first is label number, the second is a marker('.', ')') and the third one is space or tab.
                // Even if the list item is completely empty (i.e. has only digits and marker), the spec editor
                // at https://spec.commonmark.org/dingus/ parses it as three characters.
                int length = OpeningIndentationLength + System.Math.Max(3, OpeningLength);
                
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
            get { return BlockType.OrderedListItem; }
        }
    }
}
