// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2020 by Ilya Navrotskiy

using System.Text;
using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing InlineCode into Markdown.
    /// </summary>
    internal class MarkdownInlineCodeWriter
    {
        /// <summary>
        /// Initializes a new instance with the specified content lines.
        /// </summary>
        internal MarkdownInlineCodeWriter(TxtContentLines contentLines, IInline src)
        {
            Debug.Assert(contentLines != null);

            mContentLines = contentLines;
            mIsEmphasized = (src != null) &&
                            InlineHelper.GetCharacterStyle(src).Name.Contains(MarkdownUtil.InlineCodeStyleName);
        }

        /// <summary>
        /// Opens the InlineCode.
        /// </summary>
        internal void Open()
        {
            // If text ends with the same character as InlineCode delimiter, then we need to escape this character
            // with space to avoid it to be a part of the opening sequence of the InlineCode delimiter.
            WriteEscaping();

            // If current content line consists of the only whitespace characters,
            // we need to preserve them in Markdown by moving inside the InlineCode.
            int position = StringUtil.ContainsOnlyWhitespaces(CurrentLine)
                ? 0
                : CurrentLine.Length;
            CurrentLine.Insert(position, Delimiter);
            IsOpened = true;
        }

        /// <summary>
        /// Closes the InlineCode.
        /// </summary>
        internal void Close()
        {
            // If text ends with the same character as InlineCode delimiter, then we need to escape this character
            // with space to avoid it to be a part of the closing sequence of the InlineCode delimiter.
            WriteEscaping();
            CurrentLine.Append(Delimiter);

            State = MarkdownEmphasisWriterState.None;
            IsOpened = false;
        }

        /// <summary>
        /// Updates state of the writer in accordance with a specified inline.
        /// </summary>
        internal void UpdateState(IInline inline)
        {
            string styleName = (inline != null) ? InlineHelper.GetCharacterStyle(inline).Name : string.Empty;

            bool isEmphasized = styleName.Contains(MarkdownUtil.InlineCodeStyleName);
            if (mIsEmphasized == isEmphasized)
                State = MarkdownEmphasisWriterState.None;
            else
                State = (isEmphasized) ? MarkdownEmphasisWriterState.Opening : MarkdownEmphasisWriterState.Closing;

            // If a new InlineCode is starting, then we need to update writer's delimiter
            // with a new number of delimiter characters.
            if (State == MarkdownEmphasisWriterState.Opening)
                UpdateDelimiter(styleName);

            mIsEmphasized = isEmphasized;
        }

        /// <summary>
        /// Updates delimiter string in accordance with a specified style name.
        /// </summary>
        private void UpdateDelimiter(string styleName)
        {
            int delimiterLength = (styleName != string.Empty)
                ? System.Math.Max(MarkdownUtil.GetNumberAfterSubstring(styleName, MarkdownUtil.InlineCodeStyleName), 1)
                : 1;
            mDelimiter = new string(InlineCodeDelimiter.Character, delimiterLength);
        }

        /// <summary>
        /// Escapes delimiter with the same adjacent character in text.
        /// </summary>
        private void WriteEscaping()
        {
            if ((CurrentLine.Length > 0) && (CurrentLine[CurrentLine.Length - 1] == Delimiter[0]))
                CurrentLine.Append(" ");
        }

        /// <summary>
        /// The current state of the writer.
        /// </summary>
        internal MarkdownEmphasisWriterState State { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating either the InlineCode is opened.
        /// </summary>
        internal bool IsOpened { get; private set; }

        /// <summary>
        /// Gets or sets a string representing delimiter of the emphasis.
        /// </summary>
        private string Delimiter
        {
            get
            {
                return StringUtil.HasChars(mDelimiter)
                    ? mDelimiter
                    : InlineCodeDelimiter.Character.ToString();
            }
        }

        /// <summary>
        /// Gets current line to write into.
        /// </summary>
        private StringBuilder CurrentLine
        {
            get { return mContentLines.CurrentLine; }
        }

        /// <summary>
        /// The content lines to write into.
        /// </summary>
        private readonly TxtContentLines mContentLines;

        /// <summary>
        /// A string value, representing InlineCode delimiter.
        /// </summary>
        private string mDelimiter;

        /// <summary>
        /// A boolean value indicating either last processed run was emphasized (either it has a style with
        /// a corresponding name).
        /// </summary>
        private bool mIsEmphasized;
    }
}
