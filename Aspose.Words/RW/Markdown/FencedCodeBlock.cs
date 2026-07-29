// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown FencedCode block.
    /// </summary>
    internal class FencedCodeBlock : CodeBlockBase
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            if (Opening.Length < MinOpeningLength)
                return false;

            if (OpeningIndentationLength > MaxIndentationLength)
                return false;

            // All the characters in the fence sequence must be the same.
            if (StringUtil.IndexOfNotEqualTo(Opening, Opening[0], 1) != -1)
                return false;

            // Fences with backtick can contain backticks only in Opening.
            if ((Opening[0] == '`') && (ContentLine.IndexOf('`') != -1))
                return false;

            mInfo = ContentLine.TrimStart();
            int infoEnd = StringUtil.IndexOfWhitespace(mInfo);
            if (infoEnd != -1)
                mInfo = mInfo.Substring(0, infoEnd);

            return true;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.FencedCode:
                {
                    // We can append closing code fence of the same type as the code block began with
                    // (backticks or tildes), and with at least as many backticks or tildes as the opening code fence.
                    if ((block.Opening.Length < Opening.Length) || (Opening[0] != block.Opening[0]))
                    {
                        block.Action = BlockAction.Exclude;
                        return false;
                    }

                    // A closing code fence can be followed only by spaces.
                    if (((FencedCodeBlock)block).mInfo.Length > 0)
                    {
                        block.Action = BlockAction.Exclude;
                        return false;
                    }

                    // This is a valid Closing fence that closes FencedCode.
                    Close();
                    return true;
                }
                case BlockType.IndentedCode:
                case BlockType.AtxHeading:
                case BlockType.SetextHeading:
                case BlockType.Quote:
                // WORDSNET-26214 ListItems can be inside FencedCode as well.
                case BlockType.BulletListItem:
                case BlockType.OrderedListItem:
                // WORDSNET-27291 HorizontalRule can be inside FencedCode.
                case BlockType.HorizontalRule:
                {
                    // All blocks are allowed inside a FencedCode. But they should be treated as a simple text.
                    // The easiest way to achieve this, is to skip it now and process the block as a regular paragraph later.
                    block.Action = BlockAction.Exclude;
                    return false;
                }
                case BlockType.BlankLine:
                case BlockType.Paragraph:
                {
                    ContentLines.Add(StringUtil.TrimStart(block.ContentLine, OpeningIndentationLength));
                    OpenParentQuotes();
                    return true;
                }
                default:
                    return base.TryAppend(block);
            }
        }

        /// <summary>
        /// Gets a closing part from a text line starting at a specified position.
        /// </summary>
        protected override string GetClosing(string txtLine, int start, int end)
        {
            return "";
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return ((c == '`') || (c == '~'));
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.FencedCode; }
        }

        /// <summary>
        /// The line with the opening code fence may optionally contain some text following the code fence. This is
        /// trimmed of leading and trailing whitespace and called the info string.
        /// </summary>
        internal string Info
        {
            get { return mInfo; }
        }

        private string mInfo;

        /// <summary>
        /// A FencedCode min opening sequence length.
        /// </summary>
        private const int MinOpeningLength = 3;
    }
}
