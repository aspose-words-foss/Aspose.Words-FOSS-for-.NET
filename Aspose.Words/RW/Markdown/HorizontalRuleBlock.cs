// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/04/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown horizontal rule (thematic break) block.
    /// </summary>
    internal class HorizontalRuleBlock : Block
    {
        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            context.Open(this);
            context.Builder.InsertHorizontalRule();
            context.Close(this);
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            // A horizontal rule can have only an Opening and optionally OpeningIndentation.
            if (ContentLine.Length > 0)
                return false;

            if (OpeningIndentationLength > MaxIndentationLength)
                return false;

            return IsOpeningValid;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            // HorizontalRule is not appendable.
            return false;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return ((c == '-') || (c == '_') || (c == '*') || (c == ' '));
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.HorizontalRule; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Leaf; }
        }

        /// <summary>
        /// Gets a boolean value indicating whether Opening is valid for a HorizontalRule block.
        /// </summary>
        private bool IsOpeningValid
        {
            // It is allowed any number of spaces inside an opening sequence of characters.
            // But the number of non-whitespace characters MUST be not less than MinOpeningLength and
            // all non-whitespace characters MUST be the same.
            get
            {
                Debug.Assert(MinOpeningLength > 0);

                if (Opening.Length < MinOpeningLength)
                    return false;

                int nonWhitespaceCount = 0;
                foreach (char c in Opening)
                {
                    if (c == ' ')
                        continue;

                    // Note, a first opening character is always non-whitespace. Otherwise, it will be opening indentation.
                    if (c != Opening[0])
                        return false;

                    nonWhitespaceCount++;
                }

                return (nonWhitespaceCount >= MinOpeningLength);
            }
        }

        /// <summary>
        /// Defines minimum length of characters sequence to consider
        /// it as a valid opening for the HorizontalRule block.
        /// </summary>
        private const int MinOpeningLength = 3;
    }
}
