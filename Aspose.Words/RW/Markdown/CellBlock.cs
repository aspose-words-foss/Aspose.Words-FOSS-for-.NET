// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2020 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Cell block.
    /// </summary>
    internal class CellBlock : InlineContainerBlock
    {
        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            TableBlock parentTable = (TableBlock)GetParent(BlockType.Table);
            Debug.Assert(parentTable != null);

            // The spec says, that if there are a number of cells greater than the number of cells
            // in the header row the excess is ignored.
            if (Index >= parentTable.ColumnsCount)
                return;

            base.Write(context);
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            // There should be either opening or closing cell separator (|).
            if ((Opening.Length == 0) && (Closing.Length == 0))
                return false;

            // Check if this is just a single trailing separator.
            // WORDSNET-26368 Trim whitespace characters after single trailing separator.
            if ((Opening.Length != 0) && (Closing.Length == 0) && (ContentLine.Trim().Length == 0))
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
                case BlockType.Inline:
                case BlockType.BoldInline:
                case BlockType.ItalicInline:
                case BlockType.Strikethrough:
                case BlockType.Underline:
                case BlockType.InlineCode:
                    return base.TryAppend(block);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true, if cell is valid for the delimiter row.
        /// </summary>
        internal bool IsValidDelimiter()
        {
            bool hasContent = false;

            string content = ContentLine.Trim();
            for (int i = 0; i < content.Length; i++)
            {
                switch (content[i])
                {
                    case AlignmentChar:
                    {
                        // The cell alignment character can be only either at the very start or very end.
                        if (i == 0)
                        {
                            mAlignment = ParagraphAlignment.Left;
                            break;
                        }

                        if (i == (content.Length - 1))
                        {
                            mAlignment = (mAlignment == ParagraphAlignment.Left)
                                ? ParagraphAlignment.Center
                                : ParagraphAlignment.Right;
                            break;
                        }

                        return false;
                    }
                    case ContentChar:
                    {
                        hasContent = true;
                        break;
                    }
                    default:
                    {
                        // Any other character is not valid in delimiter cell.
                        return false;
                    }
                }
            }

            return hasContent;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return (c == SeparatorChar);
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
        /// Gets integer value that limits a number of opening characters to search.
        /// </summary>
        protected override int OpeningSearchLimit
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets a boolean value indicating either closing sequence is allowed without an opening one.
        /// </summary>
        protected override bool AllowClosingWithoutOpening
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a closing part from a text line moving from a specified end toward to start position.
        /// </summary>
        protected override string GetClosing(string txtLine, int start, int end)
        {
            int i = start;
            while ((i <= end) && !IsOpeningChar(txtLine[i]))
                i++;

            if (i > end)
                return "";

            return txtLine.Substring(i, end - i + 1);
        }

        /// <summary>
        /// Gets total length of all block parts.
        /// </summary>
        internal override int Length
        {
            get
            {
                return (OpeningIndentation.Length +
                        Opening.Length +
                        ContentLine.Length +
                        // Closing in cell starts at the closing pipe | and lasts to the end of whole line.
                        // So, need to limit it to just a closing pipe.
                        System.Math.Min(Closing.Length, 1));
            }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Cell; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Leaf; }
        }

        /// <summary>
        /// Gets alignment of the cell content.
        /// </summary>
        internal ParagraphAlignment Alignment
        {
            get
            {
                TableBlock parentTable = (TableBlock)GetParent(BlockType.Table);
                if ((parentTable == null) || (Index >= parentTable.ColumnsCount))
                    return mAlignment;

                return ((CellBlock)parentTable.DelimiterRow[Index]).Alignment;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char AlignmentChar = ':';
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char ContentChar = '-';
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char SeparatorChar = '|';

        private ParagraphAlignment mAlignment = ParagraphAlignment.Center;
    }
}
