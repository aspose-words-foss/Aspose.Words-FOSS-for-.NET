// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/07/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown IndentedCode block.
    /// </summary>
    internal class IndentedCodeBlock : CodeBlockBase
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            if (OpeningLength <= MaxIndentationLength)
                return false;

            if (StringUtil.ContainsOnlyWhitespaces(ContentLine))
                return false;

            ContentLines.Add(ContentLine);

            return true;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            switch (block.Type)
            {
                case BlockType.Paragraph:
                {
                    // Paragraph interrupts IndentedCode.
                    return false;
                }
                case BlockType.IndentedCode:
                {
                    ContentLines.Add(block.ContentLine);
                    OpenParentQuotes();
                    return true;
                }
                case BlockType.BlankLine:
                {
                    // We can append blank line inside Quote only when both blocks are in Quotes.
                    if (IsInQuote)
                    {
                        // Otherwise interrupt parent Quotes.
                        if (GetParent(BlockType.Quote, false) == null)
                        {
                            CloseParents(BlockType.Quote);
                            return false;
                        }

                        OpenParentQuotes();
                    }

                    // BlankLines are allowed inside IndentedCode.
                    ContentLines.Add(StringUtil.TrimStart(block.ContentLine, MaxIndentationLength + 1));
                    return true;
                }
                default:
                    return base.TryAppend(block);
            }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.IndentedCode; }
        }

        /// <summary>
        /// Gets block indentation.
        /// </summary>
        internal override int Indentation
        {
            get { return OpeningLength + ContentLineIndentationLength; }
        }

        /// <summary>
        /// Gets a plain text from the content lines of the inline container.
        /// </summary>
        protected override string GetInlineText()
        {
            return base.GetInlineText().TrimEnd(LineSeparator);
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
            return ((c == ' ') || (c == '\t'));
        }

        /// <summary>
        /// Gets integer value that limits a number of opening characters to search.
        /// </summary>
        protected override int OpeningSearchLimit
        {
            get { return MaxIndentationLength + 1; }
        }
    }
}
