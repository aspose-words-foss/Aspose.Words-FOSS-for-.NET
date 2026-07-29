// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2020 by Ilya Navrotskiy

using System;
using System.Text;
using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The base class responsible for writing Markdown emphasis.
    /// </summary>
    internal abstract class MarkdownEmphasisWriterBase : IComparable<MarkdownEmphasisWriterBase>
    {
        /// <summary>
        /// Initializes a new instance with specified content lines.
        /// </summary>
        protected MarkdownEmphasisWriterBase(TxtContentLines contentLines)
        {
            mContentLines = contentLines;
        }

        #region IComparable implementation
        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates
        /// whether the current instance precedes, follows, or occurs in the same position in the sort order as the
        /// other object.
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
        /// Value Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.
        /// Zero This instance occurs in the same position in the sort order as <paramref name="other" />.
        /// Greater than zero This instance follows <paramref name="other" /> in the sort order.
        /// </returns>
        public int CompareTo(MarkdownEmphasisWriterBase other)
        {
            if (!ReferenceEquals(this, other))
            {
                if (mOpeningLineIndex < other.mOpeningLineIndex)
                    return -1;

                if (mOpeningLineIndex > other.mOpeningLineIndex)
                    return 1;

                if (mOpeningPosition < other.mOpeningPosition)
                    return -1;

                if (mOpeningPosition > other.mOpeningPosition)
                    return 1;
            }

            return 0;
        }
        #endregion

        /// <summary>
        /// Opens the emphasis.
        /// </summary>
        internal void Open()
        {
            // Check if the current line starts with whitespace characters,
            // then wrap it into InlineCode to preserve in exported markdown paragraph.
            if ((CurrentLine.Length > 0) && StringUtil.ContainsOnlyWhitespaces(CurrentLine))
            {
                CurrentLine.Insert(0, InlineCodeDelimiter.Character);
                CurrentLine.Append(InlineCodeDelimiter.Character);
            }

            WriteDelimiter();
        }

        /// <summary>
        /// Closes the emphasis.
        /// </summary>
        internal void Close()
        {
            int lineIndex = CurrentLineIndex;
            StringBuilder line = CurrentLine;

            // There should not be whitespace characters before closing delimiters,
            // so find the position where to insert.
            int position = StringUtil.LastIndexOfNonWhitespace(line);
            while ((position == -1) && (lineIndex > 0))
            {
                line = mContentLines[--lineIndex];
                position = StringUtil.LastIndexOfNonWhitespace(line);
            }
            position++;

            line.Insert(position, CloseDelimiter);
            mOpeningLineIndex = -1;
            State = MarkdownEmphasisWriterState.None;
        }

        /// <summary>
        /// Updates state of the writer in accordance with a specified IRunAttrSource.
        /// </summary>
        internal void UpdateState(IRunAttrSource src)
        {
            bool isEmphasized = GetIsEmphasized(src);

            if ((isEmphasized == IsEmphasized) && !IsPendingForOpening)
                State = MarkdownEmphasisWriterState.None;
            else
                State = (isEmphasized) ? MarkdownEmphasisWriterState.Opening : MarkdownEmphasisWriterState.Closing;

            IsEmphasized = isEmphasized;
        }

        /// <summary>
        /// Swaps this emphasis with a specified one.
        /// </summary>
        internal bool Swap(MarkdownEmphasisWriterBase emphasis)
        {
            if (emphasis == null)
                return false;

            if (!(IsOpened && emphasis.IsOpened))
                return false;

            // Note, this operation is safe only for sibling emphases.
            // Otherwise, it can invalidate other inner emphases.
            if (!IsSibling(emphasis))
                return false;

            MarkdownEmphasisWriterBase left = (CompareTo(emphasis) < 0) ? this : emphasis;
            MarkdownEmphasisWriterBase right = (left == this) ? emphasis : this;

            // The emphases are sibling, so they are located at the same content line.
            StringBuilder line = mContentLines[right.mOpeningLineIndex];

            // First replace RIGHT emphasis delimiter.
            line.Remove(right.mOpeningPosition, right.Delimiter.Length);
            line.Insert(right.mOpeningPosition, left.Delimiter);

            // Then replace LEFT emphasis delimiter.
            line.Remove(left.mOpeningPosition, left.Delimiter.Length);
            line.Insert(left.mOpeningPosition, right.Delimiter);

            // Swap positions within lines.
            int tmp = left.mOpeningPosition;
            left.mOpeningPosition = right.mOpeningPosition;
            right.mOpeningPosition = tmp;

            return true;
        }

        /// <summary>
        /// Switches delimiter string to underscores.
        /// </summary>
        internal virtual bool SwitchToUnderscore()
        {
            return false;
        }

        /// <summary>
        /// Returns true, if a specified emphasis and this one have the same delimiter characters.
        /// </summary>
        internal bool IsSameDelimiterChar(MarkdownEmphasisWriterBase emphasis)
        {
            return (Delimiter[0] == emphasis.Delimiter[0]);
        }

        /// <summary>
        /// Gets string representing this emphasis.
        /// </summary>
        internal string GetString()
        {
            return string.Format("{0}, {1}:{2}", Delimiter, CurrentLineIndex, CurrentLine.Length);
        }

        /// <summary>
        /// Replaces opening delimiter with a specified string.
        /// </summary>
        protected void ReplaceOpening(string value)
        {
            if (IsOpened)
            {
                StringBuilder line = mContentLines[mOpeningLineIndex];
                line.Remove(mOpeningPosition, value.Length);
                line.Insert(mOpeningPosition, value);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether the emphasis is applied to the specified run source.
        /// </summary>
        protected abstract bool GetIsEmphasized(IRunAttrSource src);

        /// <summary>
        /// Writes delimiter to the end of the current content line.
        /// </summary>
        private void WriteDelimiter()
        {
            mOpeningLineIndex = CurrentLineIndex;
            mOpeningPosition = CurrentLine.Length;
            CurrentLine.Append(Delimiter);
        }

        /// <summary>
        /// Returns true, if a specified emphasis is sibling to this one.
        /// </summary>
        private bool IsSibling(MarkdownEmphasisWriterBase emphasis)
        {
            if (mOpeningLineIndex != emphasis.mOpeningLineIndex)
                return false;

            return (mOpeningPosition < emphasis.mOpeningPosition)
                ? ((mOpeningPosition + Delimiter.Length) == emphasis.mOpeningPosition)
                : ((emphasis.mOpeningPosition + emphasis.Delimiter.Length) == mOpeningPosition);
        }

        /// <summary>
        /// Gets a boolean value indicating the emphasis is opened (has only opening delimiter written).
        /// </summary>
        internal bool IsOpened
        {
            get { return (mOpeningLineIndex != -1); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating the emphasis is pending for opening.
        /// </summary>
        internal bool IsPendingForOpening
        {
            get
            {
                return ((State == MarkdownEmphasisWriterState.Opening) && !IsOpened);
            }
            set
            {
                mOpeningLineIndex = -1;
                State = (value) ? MarkdownEmphasisWriterState.Opening : MarkdownEmphasisWriterState.None;
            }
        }

        /// <summary>
        /// The current state of the emphasis.
        /// </summary>
        internal MarkdownEmphasisWriterState State { get; private set; }

        /// <summary>
        /// Returns true, if the current writer is in Html syntax mode.
        /// </summary>
        internal bool UseHtmlSyntax;

        /// <summary>
        /// Gets Flanking type of the opening delimiter for the emphasis.
        /// </summary>
        protected FlankingType OpeningFlanking
        {
            get
            {
                Debug.Assert(IsOpened);

                int prevCharIdx = mOpeningPosition - 1;
                int nextCharIdx = mOpeningPosition + Delimiter.Length;

                StringBuilder line = mContentLines[mOpeningLineIndex];
                char prevChar = (prevCharIdx < 0) ? InlineContainerBlock.LineSeparator : line[prevCharIdx];
                char nextChar = (nextCharIdx < line.Length) ? line[nextCharIdx] : InlineContainerBlock.LineSeparator;

                return MarkdownUtil.GetFlankingType(prevChar, nextChar);
            }
        }

        /// <summary>
        /// Gets a string value representing delimiter of the emphasis.
        /// </summary>
        protected abstract string Delimiter { get; }

        /// <summary>
        /// Gets a string value representing the emphasis closing delimiter.
        /// </summary>
        protected abstract string CloseDelimiter { get; }

        /// <summary>
        /// Gets a current content line to write into.
        /// </summary>
        private StringBuilder CurrentLine
        {
            get { return mContentLines.CurrentLine; }
        }

        /// <summary>
        /// Gets an index of current content line to write into.
        /// </summary>
        private int CurrentLineIndex
        {
            get { return mContentLines.CurrentLineIndex; }
        }

        /// <summary>
        /// A boolean value indicating either last processed run was emphasized (either the value of a corresponding
        /// FontAttr key property of the run was 'true' or 'false').
        /// </summary>
        protected bool IsEmphasized;

        /// <summary>
        /// The underlying content lines to write into.
        /// </summary>
        private readonly TxtContentLines mContentLines;

        /// <summary>
        /// The index of line where opening delimiter is written.
        /// </summary>
        private int mOpeningLineIndex = -1;

        /// <summary>
        /// The position within content line where opening delimiter is written.
        /// </summary>
        private int mOpeningPosition;
    }
}
