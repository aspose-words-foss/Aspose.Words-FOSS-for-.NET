// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2021 by Mikhail Nepreteamov

using System;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown FootnoteDefinition block.
    /// </summary>
    internal class FootnoteDefinitionBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            if ((Opening == "") || (Closing == ""))
                return false;

            if (Reference.Length < 1)
                return false;

            if (OpeningIndentationLength > MaxIndentationLength)
                return false;

            if (!FootnoteReferenceBlock.IsValid(Reference, 0, Reference.Length - 1))
                return false;

            return true;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            Debug.Assert(block != null);

            switch (block.Type)
            {
                case BlockType.FootnoteDefinition:
                    Close();
                    return false;
                case BlockType.IndentedCode:
                {
                    // IN. We mimic GitLab and there cannot be IndentedCode in FootnoteDefinition only at the very first line.
                    // Also note, that in order to fall under the Footnote,
                    // the paragraphs (and IndentedCode), starting from the second one, must be indented.
                    if (IsEmpty || (block.ContentLineIndentationLength <= MaxIndentationLength))
                    {
                        block.Action = BlockAction.Exclude;
                        return false;
                    }

                    Add(block);
                    return true;
                }
                default:
                {
                    // We don't want BlankLines in our tree.
                    Add((block.Type == BlockType.BlankLine) ? new EndOfLineBlock() : block);
                    return true;
                }
            }
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            // This block is written along with the FootnoteReferenceBlock.
        }

        /// <summary>
        /// Gets string representing a Reference part of this footnote definition.
        /// </summary>
        /// <remarks>The reference is actually a ContentLine between opening [^ and closing ]: delimiters.</remarks>
        internal string Reference
        {
            get { return ContentLine; }
        }

        /// <summary>
        /// Gets an opening part from a text line starting at a specified position.
        /// </summary>
        protected override string GetOpening(string txtLine, int start)
        {
            // Check the Opening fits in txtLine.
            if ((start + OpeningDelimiter.Length) > txtLine.Length)
                return "";

            // The Opening of Footnote definition can be only at the very first position of the specified offset in txtLine.
            if (txtLine.IndexOf(OpeningDelimiter, start, OpeningDelimiter.Length, StringComparison.InvariantCulture) == -1)
                return "";

            mContentIndex = start + OpeningDelimiter.Length;

            return OpeningDelimiter;
        }

        /// <summary>
        /// Gets a closing part from a text line starting at a specified position.
        /// </summary>
        protected override string GetClosing(string txtLine, int start, int end)
        {
            // The Closing is not allowed without Opening.
            if (Opening == "")
                return "";

            // The Closing index was calculated while getting RightIndentation.
            return mClosingIndex == -1 ? "" : ClosingDelimiter;
        }

        /// <summary>
        /// Gets a content part from a text line starting at a specified start position up to the end position.
        /// </summary>
        protected override string GetContent(string txtLine, int start, int end)
        {
            if (Opening == "")
                return "";

            if (mClosingIndex == -1)
                return "";

            return txtLine.Substring(mContentIndex, mClosingIndex - mContentIndex);
        }

        /// <summary>
        /// Gets an indentation part from a right side of a text line starting at the end
        /// of text up to a specified start position.
        /// </summary>
        protected override string GetRightIndentation(string txtLine, int start)
        {
            if (Opening == "")
                return "";

            mClosingIndex = txtLine.IndexOf(ClosingDelimiter, mContentIndex, StringComparison.InvariantCulture);
            if (mClosingIndex == -1)
                return "";

            // We mimic GitLab, so all whitespaces after Closing sequence ]: are considered as Closing (right) indentation.
            int indentStart = mClosingIndex + ClosingDelimiter.Length;
            int indentEnd = MarkdownUtil.IndexOfNonWhitespace(txtLine, indentStart, txtLine.Length - 1);
            if (indentEnd <= indentStart)
                return "";

            return txtLine.Substring(indentStart, indentEnd - indentStart);
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.FootnoteDefinition; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }

        /// <summary>
        /// Gets integer value that limits a number of opening characters to search.
        /// </summary>
        protected override int OpeningSearchLimit
        {
            get { return OpeningDelimiter.Length; }
        }

        /// <summary>
        /// The Opening delimiter.
        /// </summary>
        private const string OpeningDelimiter = "[^";

        /// <summary>
        /// The Closing delimiter.
        /// </summary>
        private const string ClosingDelimiter = "]:";

        /// <summary>
        /// The cached index of Closing delimiter.
        /// </summary>
        private int mClosingIndex;

        /// <summary>
        /// The cached index of ContentLine.
        /// </summary>
        private int mContentIndex;
    }
}
