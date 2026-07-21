// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown InlineCode block (code span).
    /// </summary>
    internal class InlineCodeBlock : ContainerBlock
    {

        /// <summary>
        /// Creates the instance with a specified number of backticks.
        /// </summary>
        internal InlineCodeBlock(int delimiterLength)
        {
            mDelimiter = new string(InlineCodeDelimiter.Character, delimiterLength);
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return false;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            Debug.Assert(IsOpened);
            Debug.Assert((block.Type == BlockType.Inline) || (block.Type == BlockType.HtmlTag));

            string normalizedText = MarkdownUtil.Normalize(block.Text);
            Add(new InlineBlock(normalizedText));

            return true;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            context.Open(this);
            context.Builder.Write(base.Text);
            context.Close(this);
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.InlineCode; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Gets block text.
        /// </summary>
        /// <remarks>
        /// We probably need to UnNormalize text. See <see cref="MarkdownUtil.Normalize"/> for details.
        /// </remarks>
        internal override string Text
        {
            get { return string.Format("{0}{1}{2}", mDelimiter, base.Text, mDelimiter); }
        }

        /// <summary>
        /// Gets length of a delimiter.
        /// </summary>
        internal int DelimiterLength
        {
            get { return mDelimiter.Length; }
        }

        /// <summary>
        /// A string value that represents a delimiter for this InlineCode block.
        /// </summary>
        private readonly string mDelimiter;
    }
}
