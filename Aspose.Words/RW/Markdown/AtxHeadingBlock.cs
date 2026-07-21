// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2019 by Ilya Navrotskiy

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown AtxHeading.
    /// </summary>
    internal class AtxHeadingBlock : HeadingBlock
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

            if ((Opening.Length < MinOpeningLength) || (Opening.Length > MaxOpeningLength))
                return false;

            // It is allowed to have an empty content in AtxHeadings. But if it is not empty,
            // then opening sequence of # characters MUST be followed by a whitespace.
            if ((ContentLine.Length > 0) && !StringUtil.IsWhiteSpace(ContentLine[0]))
                return false;

            ContentLines.Add(ContentLine);

            Level = Opening.Length;
            
            return true;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return (c == '#');
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.AtxHeading; }
        }

        /// <summary>
        /// A AtxHeading min opening sequence length.
        /// </summary>
        private const int MinOpeningLength = 1;
  
        /// <summary>
        /// A AtxHeading max opening sequence length.
        /// </summary>
        private const int MaxOpeningLength = 6;
    }
}
