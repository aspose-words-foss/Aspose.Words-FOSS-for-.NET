// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2019 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using Aspose.Words.Loading;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a delimiter run markdown block.
    /// </summary>
    /// <remarks>
    /// This can be a sequence of the delimiters, such as for example: **_***___***,
    /// that will be further split onto individual <see cref="Delimiter"/> objects.
    /// See 6.4 Emphasis and strong emphasis at https://spec.commonmark.org for details.
    /// </remarks>
    internal class DelimiterRun : Block
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DelimiterRun"/> by default.
        /// </summary>
        internal DelimiterRun()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DelimiterRun"/> with a specified load options.
        /// </summary>
        internal DelimiterRun(MarkdownLoadOptions loadOptions)
        {
            mLoadOptions = loadOptions;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            return Opening;
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            if (!Parse(txtLine, start))
                return false;

            mText = txtLine;

            if (Opening.Length == 0)
                return false;

            Start = start + OpeningIndentation.Length;

            // Delimiter run cannot be escaped, except of InlineCode closing.
            return (!IsEscaped || (txtLine[Start] == InlineCodeDelimiter.Character));
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            return false;
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of opening characters.
        /// </summary>
        protected override bool IsOpeningChar(char c)
        {
            return ((c == AsteriskDelimiter.Character) ||
                    (c == UnderscoreDelimiter.Character) ||
                    (c == StrikeThroughDelimiter.Character) ||
                    // WORDSNET-27318 A new option is implemented to control over the Underline formatting.
                    ((c == UnderlineDelimiter.Character) && IsImportUnderlineFormatting) ||
                    (c == InlineCodeDelimiter.Character) ||
                    (c == AutoLinkOpeningDelimiter.Character) ||
                    (c == AutoLinkClosingDelimiter.Character) ||
                    (c == LinkTextOpeningDelimiter.Character) ||
                    (c == LinkTextClosingDelimiter.Character) ||
                    (c == LinkDestinationOpeningDelimiter.Character) ||
                    (c == LinkDestinationClosingDelimiter.Character) ||
                    (c == ImageDescriptionOpeningDelimiter.Character) ||
                    (c == FootnoteOpeningDelimiter.Character));
        }

        /// <summary>
        /// Returns true, if a specified character is allowed in a sequence of indentation characters.
        /// </summary>
        protected override bool IsIndentationChar(char c)
        {
            return !IsOpeningChar(c);
        }

        /// <summary>
        /// Gets a closing part from a text line starting at a specified position.
        /// </summary>
        protected override string GetClosing(string txtLine, int start, int end)
        {
            return "";
        }

        /// <summary>
        /// Gets a content part from a text line starting at a specified position.
        /// </summary>
        protected override string GetContent(string txtLine, int start, int end)
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
        /// Gets a Delimiter object at a specified offset.
        /// </summary>
        private Delimiter GetDelimiter(int offset)
        {
            char delimiterChar = Opening[offset];
            switch (delimiterChar)
            {
                case AsteriskDelimiter.Character:
                    return new AsteriskDelimiter(this, offset);

                case UnderscoreDelimiter.Character:
                    return new UnderscoreDelimiter(this, offset);

                case StrikeThroughDelimiter.Character:
                    return new StrikeThroughDelimiter(this, offset);

                case UnderlineDelimiter.Character:
                    return new UnderlineDelimiter(this, offset);

                case InlineCodeDelimiter.Character:
                    return new InlineCodeDelimiter(this, offset);

                case AutoLinkOpeningDelimiter.Character:
                    return new AutoLinkOpeningDelimiter(this, offset);

                case AutoLinkClosingDelimiter.Character:
                    return new AutoLinkClosingDelimiter(this, offset);

                case LinkTextOpeningDelimiter.Character:
                    return new LinkTextOpeningDelimiter(this, offset);

                case LinkTextClosingDelimiter.Character:
                    return new LinkTextClosingDelimiter(this, offset);

                case LinkDestinationOpeningDelimiter.Character:
                    return new LinkDestinationOpeningDelimiter(this, offset);

                case LinkDestinationClosingDelimiter.Character:
                    return new LinkDestinationClosingDelimiter(this, offset);

                case ImageDescriptionOpeningDelimiter.Character:
                    return new ImageDescriptionOpeningDelimiter(this, offset);

                case FootnoteOpeningDelimiter.Character:
                    return new FootnoteOpeningDelimiter(this, offset);

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected delimiter character '{0}'.",
                        delimiterChar));
            }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.DelimiterRun; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.None; }
        }

        /// <summary>
        /// Gets a collection of the <see cref="Delimiter"/> objects.
        /// </summary>
        internal List<Delimiter> Delimiters
        {
            get
            {
                if (mDelimiters == null)
                {
                    mDelimiters = new List<Delimiter>();
                    int offset = 0;
                    while (offset < Opening.Length)
                    {
                        Delimiter delimiter = GetDelimiter(offset);
                        offset += delimiter.Length;

                        mDelimiters.Add(delimiter);
                    }
                }

                return mDelimiters;
            }
        }

        /// <summary>
        /// Gets block text.
        /// </summary>
        internal override string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// Gets start position of the delimiter run inside an original text.
        /// </summary>
        internal int Start { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating the delimiter run is escaped.
        /// </summary>
        internal bool IsEscaped
        {
            get { return MarkdownUtil.IsEscaped(mText, Start); }
        }

        /// <summary>
        /// Gets a boolean value indicating either to recognize <see cref="UnderlineDelimiter"/>
        /// in accordance with <see cref="mLoadOptions"/>.
        /// </summary>
        private bool IsImportUnderlineFormatting
        {
            get { return (mLoadOptions != null) && mLoadOptions.ImportUnderlineFormatting; }
        }

        private string mText;
        private List<Delimiter> mDelimiters;

        private readonly MarkdownLoadOptions mLoadOptions;
    }
}
