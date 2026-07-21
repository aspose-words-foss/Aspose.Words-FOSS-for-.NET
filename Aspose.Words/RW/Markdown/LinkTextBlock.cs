// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/05/2020 by Mikhail Nepreteamov

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkText block.
    /// </summary>
    internal class LinkTextBlock : ReferenceBlock
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
        /// Returns true, if specified delimiters can constitute a valid visible text of the link.
        /// </summary>
        internal static bool IsValid(Delimiter opening, Delimiter closing)
        {
            Debug.Assert((opening != null) && (closing != null));
            Debug.Assert(opening.IsBefore(closing));

            return MarkdownUtil.AreBalanced(opening, closing, OpeningDelimiter, ClosingDelimiter);
        }

        /// <summary>
        /// A type of the block.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.LinkText; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Opening string delimiter.
        /// </summary>
        protected override string OpeningStringDelimiter
        {
            get { return "["; }
        }

        /// <summary>
        /// Opening delimiter for LinkTextBlock.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char OpeningDelimiter = LinkTextOpeningDelimiter.Character;

        /// <summary>
        /// Closing delimiter for LinkTextBlock.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char ClosingDelimiter = LinkTextClosingDelimiter.Character;
    }
}
