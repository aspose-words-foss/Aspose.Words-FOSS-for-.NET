// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown SetextHeading block.
    /// </summary>
    internal class SetextHeadingBlock : HeadingBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            if (OpeningIndentationLength > MaxIndentationLength)
                return false;

            if (Opening.Length == 0)
                return false;

            // It is not allowed to have different characters in opening sequence.
            char openingChar = Opening[0];
            for (int i = 1; i < Opening.Length; i++)
            {
                if (Opening[i] != openingChar)
                    return false;
            }

            // SetextHeading MUST not have anything after opening sequence.
            if (!StringUtil.ContainsOnlyWhitespaces(ContentLine) || !StringUtil.ContainsOnlyWhitespaces(Closing))
                return false;

            // SetextHeading MUST have a content. So, set an initial state to exclude this block
            // from a markdown document tree until it is empty.
            Action = BlockAction.Exclude;

            Level = (openingChar == Heading1Char) ? 1 : 2;

            return true;
        }

        /// <summary>
        /// Sets a content of a specified InlineContainer to the SetextHeading.
        /// </summary>
        internal void SetContent(InlineContainerBlock inlineContainer)
        {
            ContentLines.AddRange(inlineContainer.ContentLines);
            
            ApplyOpeningIndentation(inlineContainer);
        }

        /// <summary>
        /// Returns true, if a content of a specified inline container can be set to this block.
        /// </summary>
        internal static bool CanSetContent(InlineContainerBlock inlineContainer)
        {
            // A SetextHeading opening sequence MUST be at the same nesting level as its content if it is inside a Quote.
            if (inlineContainer.IsInQuote && (inlineContainer.GetParent(BlockType.Quote, true) != null))
                return false;

            // A SetextHeading opening sequence can constitute a SetextHeading
            // with a list item only when it falls into this list item too.     
            ListItemBlock listItemBlock =
                (ListItemBlock)inlineContainer.GetParent(BlockType.BulletListItem, BlockType.OrderedListItem);
            if ((listItemBlock != null) && (listItemBlock.Action != BlockAction.ReparseListItem))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return ((c == Heading1Char) || (c == Heading2Char));
        }

        /// <summary>
        /// Applies an opening indentation from a specified block.
        /// </summary>
        private void ApplyOpeningIndentation(Block block)
        {
            mOriginalOpeningIndentation = OpeningIndentation;
            OpeningIndentation = block.OpeningIndentation;
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.SetextHeading; }
        }

        /// <summary>
        /// Gets total length of all block parts.
        /// </summary>
        internal override int Length
        {
            get { return (OriginalOpeningIndentation.Length + Opening.Length); }
        }

        /// <summary>
        /// Gets block indentation.
        /// </summary>
        internal override int Indentation
        {
            get
            {
                int indentation = OpeningIndentationLength;
                if (ContentLines.Count > 0)
                {
                    string contentLine = ContentLines[0];
                    int nonWhitespaceIndex = StringUtil.IndexOfNonWhitespace(contentLine);
                    if (nonWhitespaceIndex != -1)
                        indentation += MarkdownUtil.GetLength(contentLine.Substring(0, nonWhitespaceIndex));
                }

                return indentation;
            }
        }

        /// <summary>
        /// Gets original opening indentation of the block it has before the content was applied.
        /// </summary>
        /// <remarks>
        /// When SetextHeading gets its content with <see cref="SetContent"/> method, an opening
        /// indentation is also changed to an indentation of a block being set as a new content.
        /// </remarks>
        private string OriginalOpeningIndentation
        {
            get { return (mOriginalOpeningIndentation == null) ? OpeningIndentation : mOriginalOpeningIndentation; }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Heading1Char = '=';
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char Heading2Char = '-';

        private string mOriginalOpeningIndentation;
    }
}
