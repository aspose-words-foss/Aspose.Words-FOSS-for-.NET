// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2020 by Ilya Navrotskiy

using System.Collections.Generic;
using Aspose.Words.RW.Txt.Writer;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing markdown emphases.
    /// </summary>
    internal class MarkdownEmphasesWriter
    {
        /// <summary>
        /// Initializes a new instance of the MarkdownEmphasesWriter class.
        /// </summary>
        internal MarkdownEmphasesWriter(TxtContentLines contentLines, MarkdownWriter writer, IWarningCallback warningCallback)
            : this (contentLines, (IInline)null, writer)
        {
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// Initializes a new instance of the MarkdownEmphasesWriter class.
        /// </summary>
        internal MarkdownEmphasesWriter(TxtContentLines contentLines, IInline inline, MarkdownWriter writer)
        {
            if (inline != null)
                mWarningCallback = inline.Document_IInline.WarningCallback;

            mWriter = writer;

            mEmphasesWriters = new List<MarkdownEmphasisWriterBase>();
            // Create emphases writers.
            mEmphasesWriters.Add(new MarkdownBoldEmphasisWriter(contentLines, inline));
            mEmphasesWriters.Add(new MarkdownItalicEmphasisWriter(contentLines, inline));
            mEmphasesWriters.Add(new MarkdownStrikeThroughWriter(contentLines, inline));
            mEmphasesWriters.Add(new MarkdownSubscriptEmphasisWriter(contentLines, inline));
            mEmphasesWriters.Add(new MarkdownSuperscriptEmphasisWriter(contentLines, inline));
            // WORDSNET-26248 Implemented export of underline formatting.
            if (mWriter.SaveOptions.ExportUnderlineFormatting)
                mEmphasesWriters.Add(new MarkdownUnderlineWriter(contentLines, inline));

            // Create InlineCode writer.
            mInlineCodeWriter = new MarkdownInlineCodeWriter(contentLines, inline);
        }

        /// <summary>
        /// Flushes collection of pending opening emphases delimiters.
        /// </summary>
        internal void FlushPendingOpeningEmphases()
        {
            foreach (MarkdownEmphasisWriterBase writer in mEmphasesWriters)
            {
                if (writer.IsPendingForOpening)
                    writer.Open();
            }
        }

        /// <summary>
        /// Appends emphases for the specified IInline object.
        /// </summary>
        internal void AppendEmphases(IInline src)
        {
            UpdateWritersState(src);
            WriteClosingDelimiters();

            // The InlineCode delimiter can be written immediately, because it is not affected by whitespace
            // characters. Also, as InlineCode delimiter is not a whitespace character itself, any other delimiters
            // can be written before InlineCode. So, flush emphases just now in this case.
            if (mInlineCodeWriter.State == MarkdownEmphasisWriterState.Opening)
            {
                FlushPendingOpeningEmphases();

                // Note, InlineCode delimiter should be written at the very end.
                // Otherwise, all other delimiters become just literals inside the InlineCode block.
                mInlineCodeWriter.Open();
            }
        }

        /// <summary>
        /// Updates states of the all writers for a specified inline.
        /// </summary>
        private void UpdateWritersState(IInline inline)
        {
            foreach (MarkdownEmphasisWriterBase writer in mEmphasesWriters)
            {
                writer.UseHtmlSyntax = mWriter.UseHtmlSyntax;
                writer.UpdateState(inline);
            }

            mInlineCodeWriter.UpdateState(inline);
        }

        /// <summary>
        /// Writes closing delimiters for all writers.
        /// </summary>
        private void WriteClosingDelimiters()
        {
            if (mInlineCodeWriter.State == MarkdownEmphasisWriterState.Closing)
                mInlineCodeWriter.Close();

            // Tries to close opened emphases using different strategies.
            TryCloseProbeAllClosings();
            TryCloseSwappingOpenings();
            TryCloseWithCloseOpen();
        }

        /// <summary>
        /// Tries to close emphases using 'ProbeAllClosings' strategy.
        /// </summary>
        /// <remarks>
        /// As we cannot close emphasis for which there is any still opened emphasis inside it, we are iterating through
        /// the all emphases being closed and try to close each of them. If something was closed successfully, we repeat
        /// the operation and check again if we can close something else.
        /// </remarks>
        private void TryCloseProbeAllClosings()
        {
            bool isSuccessClosed;
            do
            {
                isSuccessClosed = false;
                foreach (MarkdownEmphasisWriterBase writer in mEmphasesWriters)
                {
                    if (writer.State != MarkdownEmphasisWriterState.Closing)
                        continue;

                    // There should not be inner opened emphases. Otherwise, they never will be closed.
                    if (GetNextOpened(writer) == null)
                    {
                        writer.Close();
                        isSuccessClosed = true;
                    }
                }
            }
            while (isSuccessClosed);
        }

        /// <summary>
        /// Tries to close emphases using 'SwapOpenings' strategy.
        /// </summary>
        /// <remarks>
        /// As we cannot close emphasis for which there is any still opened emphasis inside it, we try to swap
        /// positions of neighbored opening delimiters. The order of openings in this case is not important, but the
        /// problematic opening delimiter will be moved outside of closing range and this will allow to complete
        /// closing operation successfully.
        /// </remarks>
        private void TryCloseSwappingOpenings()
        {
            foreach (MarkdownEmphasisWriterBase writer in mEmphasesWriters)
            {
                if (writer.State != MarkdownEmphasisWriterState.Closing)
                    continue;

                MarkdownEmphasisWriterBase nextOpened = GetNextOpened(writer);
                while (writer.Swap(nextOpened))
                    nextOpened = GetNextOpened(writer);

                if (nextOpened == null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Tries to close emphases using 'CloseOpen' strategy.
        /// </summary>
        /// <remarks>
        /// As we cannot close emphasis for which there is any still opened emphasis inside it, we try to close
        /// still opened problematic emphasis and then send it for opening again.
        /// </remarks>
        private void TryCloseWithCloseOpen()
        {
            foreach (MarkdownEmphasisWriterBase writer in mEmphasesWriters)
            {
                if (writer.State == MarkdownEmphasisWriterState.Closing)
                    TryCloseWithCloseOpen(writer);
            }
        }

        /// <summary>
        /// Tries to close a specified emphasis using 'CloseOpen' strategy.
        /// </summary>
        private bool TryCloseWithCloseOpen(MarkdownEmphasisWriterBase writer)
        {
            if (!writer.IsOpened)
                return true;

            // Fist try to close still opened inner emphasis.
            MarkdownEmphasisWriterBase nextOpened = GetNextOpened(writer);
            if (nextOpened != null)
            {
                if (!TryCloseWithCloseOpen(nextOpened))
                    return false;
            }

            // Check either the emphasis should be just closed (without opening).
            if (writer.State == MarkdownEmphasisWriterState.Closing)
                return Close(writer);

            // Close and then open the emphasis.
            writer.Close();
            writer.IsPendingForOpening = true;

            return true;
        }

        /// <summary>
        /// Closes a specified emphasis writer.
        /// </summary>
        private bool Close(MarkdownEmphasisWriterBase closingWriter)
        {
            Debug.Assert(closingWriter.IsOpened);
            Debug.Assert(GetNextOpened(closingWriter) == null);

            // The emphasis can be closed successfully, if it has delimiter char different from the opening
            // delimiter of the next following emphasis. Note, that if we here, then there cannot be already
            // opened inner emphases, but there is pending ones in opening queue.
            foreach (MarkdownEmphasisWriterBase openingWriter in mEmphasesWriters)
            {
                if (!openingWriter.IsPendingForOpening)
                    continue;

                // Try to change delimiter to make it to be different with adjacent opening emphasis.
                if (closingWriter.IsSameDelimiterChar(openingWriter) && !closingWriter.SwitchToUnderscore())
                {
                    // We encounter a pending for opening emphasis that cannot be properly opened.
                    // But we still can close the target emphasis.
                    closingWriter.Close();

                    // This can be improved by checking each pending emphasis independently,
                    // but for a moment just clear the whole opening queue.
                    ClearOpeningQueue();
                    return false;
                }
            }

            // The emphasis can be successfully closed.
            closingWriter.Close();
            return true;
        }

        /// <summary>
        /// Clears queue of emphases writers pending for opening.
        /// </summary>
        private void ClearOpeningQueue()
        {
            foreach (MarkdownEmphasisWriterBase writer in mEmphasesWriters)
            {
                if (writer.IsPendingForOpening)
                {
                    writer.IsPendingForOpening = false;
                    WarnLostFormatting(writer);
                }
            }
        }

        /// <summary>
        /// Returns next opened emphasis writer.
        /// </summary>
        private MarkdownEmphasisWriterBase GetNextOpened(MarkdownEmphasisWriterBase writer)
        {
            Debug.Assert(writer != null);

            MarkdownEmphasisWriterBase nextEmphasis = null;
            foreach (MarkdownEmphasisWriterBase curWriter in mEmphasesWriters)
            {
                if (curWriter.IsOpened && (curWriter.CompareTo(writer) > 0))
                {
                    if ((nextEmphasis == null) || (curWriter.CompareTo(nextEmphasis) < 0))
                        nextEmphasis = curWriter;
                }
            }

            return nextEmphasis;
        }

        /// <summary>
        /// Issues lost formatting warning for a specified emphasis.
        /// </summary>
        private void WarnLostFormatting(MarkdownEmphasisWriterBase emphasis)
        {
            if (mWarningCallback != null)
            {
                string description = string.Format(WarningStrings.MarkdownFormattingLost, emphasis.GetString());
                mWarningCallback.Warning(new WarningInfo(WarningType.MajorFormattingLoss, WarningSource.Markdown, description));
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either InlineCode emphasis is in opening state.
        /// </summary>
        internal bool IsInlineCodeOpening
        {
            get { return (mInlineCodeWriter.State == MarkdownEmphasisWriterState.Opening); }
        }

        /// <summary>
        /// Gets a boolean value indicating either InlineCode emphasis is opened.
        /// </summary>
        internal bool IsInInlineCode
        {
            get { return (mInlineCodeWriter.IsOpened); }
        }

        /// <summary>
        /// The underlying writer.
        /// </summary>
        private readonly MarkdownWriter mWriter;

        /// <summary>
        /// The warning callback to log warnings.
        /// </summary>
        private readonly IWarningCallback mWarningCallback;

        /// <summary>
        /// Keeps all possible emphases writers.
        /// </summary>
        private readonly List<MarkdownEmphasisWriterBase> mEmphasesWriters;

        /// <summary>
        /// The InlineCode writer.
        /// </summary>
        private readonly MarkdownInlineCodeWriter mInlineCodeWriter;
    }
}
