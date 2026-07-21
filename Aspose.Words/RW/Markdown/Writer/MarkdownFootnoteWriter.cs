// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2021 by Mikhail Nepreteamov

using System.Collections.Generic;
using Aspose.Words.Notes;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for writing Footnotes into markdown.
    /// </summary>
    internal class MarkdownFootnoteWriter
    {
        /// <summary>
        /// Processes the start of the specified footnote.
        /// </summary>
        internal void OnFootnoteStart(Footnote footnote)
        {
            // IN. I think footnote definitions most frequently contain one single paragraph, so init with capacity 1.
            mCurFootnoteParagraphs = new List<MarkdownParagraphWriter>(1);
            mFootnotes.Add(mCurFootnoteParagraphs);

            Footnote = footnote;
        }

        /// <summary>
        /// Processes the end of the specified footnote.
        /// </summary>
        internal void OnFootnoteEnd()
        {
            Footnote = null;
        }

        /// <summary>
        /// Adds a specified paragraph to the list of paragraphs pending for writing.
        /// </summary>
        /// <remarks>
        /// Footnote definitions must be written at the very end of the document,
        /// so we just collect them till the document processing is finished.
        /// </remarks>
        internal void AddParagraph(MarkdownParagraphWriter paragraphWriter)
        {
            mCurFootnoteParagraphs.Add(paragraphWriter);
        }

        /// <summary>
        /// Writes collected paragraphs of definitions to a specified writer.
        /// </summary>
        internal void WriteDefinitions(MarkdownWriter writer)
        {
            for (int i = 0; i < mFootnotes.Count; i++)
            {
                writer.Builder.Append(writer.SaveOptions.ParagraphBreak);

                // Append reference mark.
                writer.Builder.Append(string.Format("{0}: ", GetReference(i + 1)));

                List<MarkdownParagraphWriter> paragraphWriters = mFootnotes[i];
                for (int j = 0; j < paragraphWriters.Count; j++)
                {
                    paragraphWriters[j].Write();

                    // Write paragraph break for all paragraphs except of the very last.
                    if (j < (paragraphWriters.Count - 1))
                        writer.Builder.Append(writer.SaveOptions.ParagraphBreak);
                }
            }
        }

        /// <summary>
        /// Returns string representing a reference mark for a specified reference index.
        /// </summary>
        private static string GetReference(int refIndex)
        {
            return string.Format("{0}{1}{2}",
                FootnoteReferenceBlock.OpeningDelimiter, refIndex, FootnoteReferenceBlock.ClosingDelimiter);
        }

        /// <summary>
        /// The currently processing footnote.
        /// </summary>
        internal Footnote Footnote { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating either any Footnote is currently processing.
        /// </summary>
        internal bool IsInFootnote
        {
            get { return (Footnote != null); }
        }

        /// <summary>
        /// Gets a Footnote reference mark in brackets for the currently processing footnote.
        /// </summary>
        internal string Reference
        {
            get { return GetReference(mFootnotes.Count); }
        }

        /// <summary>
        /// The collection of paragraph writers of the current footnote.
        /// </summary>
        private List<MarkdownParagraphWriter> mCurFootnoteParagraphs;

        /// <summary>
        /// The collection of paragraph writers of all footnotes.
        /// </summary>
        private readonly List<List<MarkdownParagraphWriter>> mFootnotes = new List<List<MarkdownParagraphWriter>>();
    }
}
