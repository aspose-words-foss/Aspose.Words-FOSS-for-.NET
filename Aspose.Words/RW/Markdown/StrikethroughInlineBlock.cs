// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/09/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown StrikethroughInline block.
    /// </summary>
    internal class StrikethroughInlineBlock : ContainerBlock
    {
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

            Add(block);
            return true;
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Strikethrough; }
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
        internal override string Text
        {
            get { return string.Format("{0}{1}{2}", Delimiter, base.Text, Delimiter); }
        }

        /// <summary>
        /// Delimiter for StrikethroughInlineBlock.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const string Delimiter = "~~";
    }
}
