// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/04/2019 by Ilya Navrotskiy

using System.Collections.Generic;
using System.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.RW.Txt.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for saving document or a fragment of a document in markdown format.
    /// </summary>
    internal class MarkdownWriter : TxtWriterBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MarkdownWriter"/> class.
        /// </summary>
        internal MarkdownWriter()
        {
            RawHtmlSaveOptions.OfficeMathOutputMode = HtmlOfficeMathOutputMode.MathML;
        }

        /// <summary>
        /// Called when enumeration of a header or footer in a section has started.
        /// </summary>
        public override VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            mPrevParagraphWriter = null;
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table has started.
        /// </summary>
        public override VisitorAction VisitTableStart(Table table)
        {
            if (IsInTable)
            {
                TableWriter = TableWriter.StartNestedTable(table);
            }
            else
            {
                TableWriter = new MarkdownTableWriter(table, this);
                if (TableWriter.HtmlSyntaxRequired)
                    mHtmlSyntaxConsumers++;

                TableWriter.OnTableStart();
            }

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a Office Math object has started.
        /// </summary>
        public override VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            switch (SaveOptions.OfficeMathExportMode)
            {
                // FOSS MathML export routed OfficeMath through the removed HTML writer; degrade
                // gracefully by skipping (emit nothing), consistent with the Latex/MarkItDown modes.
                case MarkdownOfficeMathExportMode.MathML:

                // WORDSNET-28492 Added export OfficeMath to Latex.
                case MarkdownOfficeMathExportMode.Latex:
                // WORDSNET-28766 Added export OfficeMath to MarkItDown-compatible Latex.
                case MarkdownOfficeMathExportMode.MarkItDown:
                {
                    // FOSS
                    return VisitorAction.SkipThisNode;
                }

                default:
                    return VisitorAction.Continue;
            }
        }

        /// <summary>
        /// Called when enumeration of a table has ended.
        /// </summary>
        public override VisitorAction VisitTableEnd(Table table)
        {
            TableWriter.OnTableEnd();

            if (TableWriter.HtmlSyntaxRequired)
                mHtmlSyntaxConsumers--;

            mTableWriters.Pop();

            if (IsInTable)
                TableWriter.OnNestedTableEnd();

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table row has started.
        /// </summary>
        public override VisitorAction VisitRowStart(Row row)
        {
            TableWriter.OnRowStart(row);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table cell has started.
        /// </summary>
        public override VisitorAction VisitCellStart(Cell cell)
        {
            TableWriter.OnCellStart(cell);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table row has ended.
        /// </summary>
        public override VisitorAction VisitRowEnd(Row row)
        {
            TableWriter.OnRowEnd(row);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a table cell has ended.
        /// </summary>
        public override VisitorAction VisitCellEnd(Cell cell)
        {
            TableWriter.OnCellEnd(cell);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a paragraph has started.
        /// </summary>
        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            ParagraphWriter = new MarkdownParagraphWriter(paragraph, this);

            if (ParagraphWriter.HtmlSyntaxRequired)
                mHtmlSyntaxConsumers++;

            // WORDSNET-24544 A new option is introduced. When it is enabled, we should write
            // list labels as raw text.
            if (SaveOptions.ListExportMode == MarkdownListExportMode.PlainText)
            {
                // To process possible emphasizes in list item, lets add dummy run and process it in a usual way.
                // Then we remove it in VisitParagraphEnd().
                Run listLabelRun = GetListLabelAsRun(paragraph);
                if (listLabelRun != null)
                    paragraph.PrependChild(listLabelRun);
            }

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a paragraph has ended.
        /// </summary>
        public override VisitorAction VisitParagraphEnd(Paragraph paragraph)
        {
            // Flush all opened fields.
            foreach (MarkdownFieldWriter fieldWriter in mFieldWriters)
                ParagraphWriter.AppendText(fieldWriter.FlushText(), false);

            // Write closing emphases for a very last run.
            ParagraphWriter.OnRun(null);

            // WORDSNET-24544 Remove dummy run we created in VisitParagraphStart().
            if ((SaveOptions.ListExportMode == MarkdownListExportMode.PlainText) && HasNonEmptyListLabel(paragraph))
                paragraph.RemoveChild(paragraph.FirstRun);

            if (IsInTable)
            {
                TableWriter.AddParagraph(ParagraphWriter);
            }
            else if (IsInFootnote)
            {
                FootnoteWriter.AddParagraph(ParagraphWriter);
            }
            else
            {
                ParagraphWriter.Write();
            }

            if (ParagraphWriter.HtmlSyntaxRequired)
                mHtmlSyntaxConsumers--;

            ParagraphWriter = null;
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a run is encountered in the document.
        /// </summary>
        public override VisitorAction VisitRun(Run run)
        {
            // WORDSNET-24585 Don't write hidden runs.
            if (InlineHelper.FetchAttr(run, FontAttr.Hidden) == AttrBoolEx.True)
                return VisitorAction.Continue;

            // WORDSNET-25772 Don't write runs that are child for TextBox.
            if (IsInTextBox(run))
                return VisitorAction.Continue;

            // Writes all the open bookmarks before the upcoming Run.
            FlushBookmarks();

            if (IsInField)
                FieldWriter.OnRun(run);
            else
                ParagraphWriter.OnRun(run);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a start of a bookmark is encountered in the document.
        /// </summary>
        public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            if (bookmarkStart.Name != "_GoBack")
                // ToLowerInvariant() is a workaround for html import of mixed case links.
                // See TestExportMixedCaseLinks() for more details.
                mBookmarks.Add(bookmarkStart.Name.ToLowerInvariant());

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a field starts in the document.
        /// </summary>
        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            FieldWriter = MarkdownFieldWriter.Create(fieldStart, LinkDefinitionWriter, this);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a field separator is encountered in the document.
        /// </summary>
        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            FieldWriter.OnFieldSeparator(fieldSeparator);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when a field ends in the document.
        /// </summary>
        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            string text = FieldWriter.OnFieldEnd(fieldEnd);
            mFieldWriters.Pop();

            // If the field is nested, then append its text to a parent Field.
            if (IsInField)
                FieldWriter.AppendToFieldResult(text);
            else
                AppendText(text, false);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a shape has started.
        /// </summary>
        public override VisitorAction VisitShapeStart(Shape shape)
        {
            if ((shape.HorizontalRule != null) && shape.HorizontalRule.On)
                AppendText(HorizontalRule);
            else if (ImageWriter.OnShape(shape))
                // WORDSNET-24969 We don't need to escape characters (* and _) for ImageWriter, as there is either file name, or base64 image data.
                AppendText(ImageWriter.Text, false);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a group shape has started.
        /// </summary>
        public override VisitorAction VisitGroupShapeStart(GroupShape group)
        {
            if (ImageWriter.OnShape(group))
                AppendText(ImageWriter.Text);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a footnote or endnote text has started.
        /// </summary>
        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            FootnoteWriter.OnFootnoteStart(footnote);

            // Append emphases to the parent paragraph of the footnote.
            ParagraphWriter.AppendEmphases(footnote);

            // Append footnote reference mark.
            ParagraphWriter.AppendText(FootnoteWriter.Reference, false);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of a footnote or endnote text has ended.
        /// </summary>
        public override VisitorAction VisitFootnoteEnd(Footnote footnote)
        {
            FootnoteWriter.OnFootnoteEnd();
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Called when enumeration of the document has finished.
        /// </summary>
        public override VisitorAction VisitDocumentEnd(Document doc)
        {
            FootnoteWriter.WriteDefinitions(this);

            LinkDefinitionWriter.WriteDefinitions(this);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Updates list labels of the Document being written.
        /// </summary>
        internal void UpdateListLabels()
        {
            if (mIsListLabelsUpdated)
                return;

            ((Document)Document).UpdateListLabels();
            mIsListLabelsUpdated = true;
        }

        /// <summary>
        /// Gets <see cref="MarkdownParagraphWriter"/> corresponded to a specified paragraph node.
        /// </summary>
        internal MarkdownParagraphWriter GetParagraphWriterByNode(Paragraph paragraph)
        {

            if (mParagraphWriters.Count > 0)
                return mParagraphWriters.Peek().GetParagraphWriterByNode(paragraph);

            if (PrevParagraphWriter != null)
                return PrevParagraphWriter.GetParagraphWriterByNode(paragraph);

            return null;
        }

        /// <summary>
        /// Writes all headers and footers of a specified section.
        /// </summary>
        protected override void WriteAllHeadersFooters(Section section)
        {
            // In markdown we need to separate a content of the HeadersFooters from a content of the Bodies
            // using a blank line to avoid undesirable lazy paragraph continuation.
            if (section.IsFirstSection)
                HeadersFootersBuilder.Append(SaveOptions.ParagraphBreak);

            base.WriteAllHeadersFooters(section);
        }

        /// <summary>
        /// Writes all the pending bookmarks.
        /// </summary>
        private void FlushBookmarks()
        {
            foreach (string bookmarkName in mBookmarks)
            {
                // Most Markdown editors don't currently process the markup inside
                // wide-range anchor html tag. Therefore, BookmarkEnd (</a>) comes right after BookmarkStart (<a name="bmk">).
                string htmlAnchor = string.Format(@"<a name=""{0}""></a>", bookmarkName);
                AppendText(htmlAnchor, false);
            }

            // Since we're writing BookmarkEnd right after BookmarkStart, there's no need to resolve
            // bookmark nesting. We just write the entire list and clear it.
            mBookmarks.Clear();
        }

        /// <summary>
        /// Appends a specified text.
        /// </summary>
        private void AppendText(string text, bool isNeedEscaping = true)
        {
            Debug.Assert(ParagraphWriter != null);
            ParagraphWriter.AppendText(text, isNeedEscaping);
        }

        /// <summary>
        /// Returns list label of the numbered paragraph as standalone run.
        /// </summary>
        private Run GetListLabelAsRun(Paragraph paragraph)
        {
            string label = GetListLabel(paragraph);
            if (string.IsNullOrEmpty(label))
                return null;

            // Let's add by default space character at the end of the list label, if it is not there yet.
            // This improves visual looking and also avoids possible collisions when list item is emphasized.
            if (!StringUtil.IsWhiteSpace(label[label.Length - 1]))
                label = string.Format("{0} ", label);

            RunPr runPr = paragraph.ListLabel.GetExpandedRunPr(true, RunPrExpandFlags.DocumentDefaults);
            return new Run(Document, label, runPr);
        }

        /// <summary>
        /// Returns list label string of a specified paragraph.
        /// </summary>
        private string GetListLabel(Paragraph paragraph)
        {
            if (!paragraph.IsListItem)
                return null;

            UpdateListLabels();

            return paragraph.ListLabel.LabelString;
        }

        /// <summary>
        /// Returns true, if a specified paragraph has non-empty list label string.
        /// </summary>
        private bool HasNonEmptyListLabel(Paragraph paragraph)
        {
            return !string.IsNullOrEmpty(GetListLabel(paragraph));
        }

        /// <summary>
        /// Returns true if the specified node is in a TextBox.
        /// </summary>
        private static bool IsInTextBox(Node node)
        {
            Shape shape = node.GetAncestor(NodeType.Shape) as Shape;
            return (shape != null) && (shape.ShapeType == ShapeType.TextBox);
        }

        /// <summary>
        /// Gets markdown save options.
        /// </summary>
        internal MarkdownSaveOptions SaveOptions
        {
            get { return (MarkdownSaveOptions)SaveOptionsBase; }
        }

        /// <summary>
        /// The ParagraphWriter of the previously processed paragraph.
        /// </summary>
        internal MarkdownParagraphWriter PrevParagraphWriter
        {
            get { return mPrevParagraphWriter; }
            private set
            {
                if (value != null)
                    mPrevParagraphWriter = value;
            }
        }

        /// <summary>
        /// Gets MarkdownFootnoteWriter object.
        /// </summary>
        internal MarkdownFootnoteWriter FootnoteWriter
        {
            get
            {
                if (mFootnoteWriter == null)
                    mFootnoteWriter = new MarkdownFootnoteWriter();
                return mFootnoteWriter;
            }
        }

        /// <summary>
        /// Gets MarkdownFieldWriter object for most inner started field.
        /// </summary>
        internal MarkdownFieldWriter FieldWriter
        {
            get { return (IsInField) ? mFieldWriters.Peek() : null; }
            set { mFieldWriters.Push(value); }
        }

        /// <summary>
        /// Returns true, if the current writer is in Html syntax mode.
        /// </summary>
        internal bool UseHtmlSyntax
        {
            get { return mHtmlSyntaxConsumers > 0; }
        }

        /// <summary>
        /// Gets a boolean value indicating either any table is started.
        /// </summary>
        private bool IsInTable
        {
            get { return (mTableWriters.Count > 0); }
        }

        /// <summary>
        /// Gets a boolean value indicating either any field is started.
        /// </summary>
        private bool IsInField
        {
            get { return (mFieldWriters.Count > 0); }
        }

        /// <summary>
        /// Gets a boolean value indicating either a Footnote is started.
        /// </summary>
        private bool IsInFootnote
        {
            get { return (mFootnoteWriter.IsInFootnote); }
        }

        /// <summary>
        /// Gets MarkdownTableWriter object for most inner started table.
        /// </summary>
        private MarkdownTableWriter TableWriter
        {
            get { return (IsInTable) ? mTableWriters.Peek() : null; }
            set { mTableWriters.Push(value); }
        }

        /// <summary>
        /// The writer of a currently processing paragraph.
        /// </summary>
        private MarkdownParagraphWriter ParagraphWriter
        {
            get { return mParagraphWriters.Top(); }
            set
            {
                PrevParagraphWriter = ParagraphWriter;
                if (value == null)
                {
                    if (mParagraphWriters.Count > 0)
                        mParagraphWriters.Pop();
                }
                else
                {
                    mParagraphWriters.Push(value);
                }
            }
        }

        /// <summary>
        /// Gets MarkdownImageWriter object.
        /// </summary>
        private MarkdownImageWriter ImageWriter
        {
            get
            {
                if (mImageWriter == null)
                {
                    // WORDSNET-24219 Set FileName in automatic mode when saving into FileStream.
                    string fileName = SaveInfo.FileName;
                    if ((fileName == null) && (SaveInfo.Stream is FileStream))
                        fileName = ((FileStream)(SaveInfo.Stream)).Name;

                    mImageWriter = new MarkdownImageWriter(fileName, SaveOptions, LinkDefinitionWriter);
                }

                return mImageWriter;
            }
        }

        /// <summary>
        /// Gets MarkdownLinkDefinitionWriter object.
        /// </summary>
        private MarkdownLinkDefinitionWriter LinkDefinitionWriter
        {
            get
            {
                if (mLinkDefinitionWriter == null)
                    mLinkDefinitionWriter = new MarkdownLinkDefinitionWriter(this);

                return mLinkDefinitionWriter;
            }
        }

        /// <summary>
        /// Specifies HTML save options to write nodes as raw HTML.
        /// </summary>
        internal readonly HtmlSaveOptions RawHtmlSaveOptions = HtmlSaveOptions.CreateSaveFragmentOptions();

        /// <summary>
        /// The counter of objects that require Html syntax use.
        /// </summary>
        private int mHtmlSyntaxConsumers = 0;

        /// <summary>
        /// The collection of table writers.
        /// </summary>
        private readonly Stack<MarkdownTableWriter> mTableWriters = new Stack<MarkdownTableWriter>();

        /// <summary>
        /// The collection of field writers.
        /// </summary>
        private readonly Stack<MarkdownFieldWriter> mFieldWriters = new Stack<MarkdownFieldWriter>();

        /// <summary>
        /// The collection of paragraph writers.
        /// </summary>
        private readonly Stack<MarkdownParagraphWriter> mParagraphWriters = new Stack<MarkdownParagraphWriter>();

        /// <summary>
        /// ParagraphWriter for previously processed paragraph.
        /// </summary>
        private MarkdownParagraphWriter mPrevParagraphWriter;

        /// <summary>
        /// The writer for images.
        /// </summary>
        private MarkdownImageWriter mImageWriter;

        private readonly List<string> mBookmarks = new List<string>();

        /// <summary>
        /// The writer for footnotes.
        /// </summary>
        private MarkdownFootnoteWriter mFootnoteWriter;

        /// <summary>
        /// The writer for link definitions.
        /// </summary>
        private MarkdownLinkDefinitionWriter mLinkDefinitionWriter;

        /// <summary>
        /// Indicates whether list labels for the Document were updated.
        /// </summary>
        private bool mIsListLabelsUpdated;

        private const string HorizontalRule = "-----";
    }
}
