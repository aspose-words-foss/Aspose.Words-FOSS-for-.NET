// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2020 by Ilya Navrotskiy

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown Row block.
    /// </summary>
    internal class RowBlock : ContainerBlock
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal RowBlock()
        {
        }

        /// <summary>
        /// Creates a Row from a specified text.
        /// </summary>
        internal RowBlock(string text)
        {
            List <CellBlock> cells = Parse(text); 
            foreach (CellBlock cell in cells)
                Add(cell);
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            if (ContentLine.Length > 0)
                return false;

            List<CellBlock> cells = Parse(Opening);
            if (IsValidDelimiter(cells))
            {
                Debug.Assert(IsEmpty);
                foreach (CellBlock cell in cells)
                    Add(cell);
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses a specified text onto Cell blocks.
        /// </summary>
        internal static List<CellBlock> Parse(string text)
        {
            List<CellBlock> cells = new List<CellBlock>();

            CellBlock curCell = new CellBlock();
            int start = 0;
            while (curCell.TryParse(text, start))
            {
                cells.Add(curCell);
                start += (curCell.Length - 1);

                curCell = new CellBlock();
            }

            return cells;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return (c == CellBlock.SeparatorChar || c == CellBlock.ContentChar || c == CellBlock.AlignmentChar || c == ' ');
        }

        /// <summary>
        /// Returns true, if a specified list of cells can constitute the valid row delimiter. 
        /// </summary>
        private static bool IsValidDelimiter(List<CellBlock> cells)
        {
            foreach (CellBlock cell in cells)
            {
                if (!cell.IsValidDelimiter())
                    return false;
            }

            return (cells.Count > 0);
        }

#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            string isOpened = (IsOpened) ? "opened" : "closed";
            return string.Format("{0} ({1}): '{2}'", Type, isOpened, Text);
        }
#endif

        /// <summary>
        /// Gets block text.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        internal override string Text
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (Count > 0)
                    sb.Append(CellBlock.SeparatorChar);

                foreach (Block block in this)
                {
                    CellBlock cell = (CellBlock)block;
                    sb.Append(cell.Text);
                    sb.Append(CellBlock.SeparatorChar);
                }

                return sb.ToString();
            }
        }

#if CPLUSPLUS
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        private string GetTextInternal()
        {
            // Workaround for C++
        }
#endif

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Row; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Block; }
        }
    }
}
