// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/11/2019 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Fonts;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Lists;
using Aspose.Words.Notes;
using Aspose.Words.RW.Markdown.Reader;
using Aspose.Words.RW.Txt.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Markdown.Writer
{
    /// <summary>
    /// The class responsible for saving paragraph in markdown format.
    /// </summary>
    internal class MarkdownParagraphWriter
    {
        /// <summary>
        /// Creates a new instance from a specified paragraph.
        /// </summary>
        internal MarkdownParagraphWriter(Paragraph paragraph, MarkdownWriter writer)
        {
            Debug.Assert(paragraph != null);
            Debug.Assert(writer != null);

            mParagraph = paragraph;
            mWriter = writer;
            mPrevParagraphWriter = mWriter.PrevParagraphWriter;
            mEmphasesWriter = new MarkdownEmphasesWriter(mContentLines, mWriter, Document.WarningCallback);

            Init();

            if (IsNeedParagraphBreak)
                AppendText(ControlChar.LineBreak);
        }

        /// <summary>
        /// Writes content lines of the paragraph into underlying writer.
        /// </summary>
        internal void Write()
        {
            // WORDSNET-28043 Added new export option for empty paragraphs.
            // Note, in CodeBlock and Footnote, we should write empty paragraphs as usual.
            if (mParagraph.IsEmptyOrContainsOnlyCrossAnnotation && !IsInCodeBlock && !IsInFootnote)
            {
                WriteEmptyParagraph();
                if (mWriter.SaveOptions.EmptyParagraphExportMode == MarkdownEmptyParagraphExportMode.None)
                    return;
            }

            // Let's add one empty line for empty paragraph in order to write its styles markers properly.
            if (mContentLines.Count == 0)
                CurrentLine.Append("");

            UpdateStylesMarkers();

            if (IsFencedCode)
                WriteFencedCodeOpening();

            WriteContentLines();

            if (IsFencedCode)
                WriteFencedCodeClosing();

            if (mIsSetextHeading)
                WriteSetextHeading();

            if (!IsInCell && !IsInFootnote)
                Builder.Append(ParagraphBreak);
        }

        /// <summary>
        /// Processes a specified run.
        /// </summary>
        internal void OnRun(Run run)
        {
            AppendEmphases(run);

            if (run != null)
            {
                string text = TxtWriterBase.GetText(run);

                // Don't write custom reference mark of footnote.
                if (IsFirstParaInFootnote && run.IsFirstChild)
                {
                    Footnote footnote = mWriter.FootnoteWriter.Footnote;
                    if (text == footnote.ReferenceMark)
                        return;
                }

                AppendText(text);
            }
        }

        /// <summary>
        /// Appends emphases for a specified inline node.
        /// </summary>
        internal void AppendEmphases(IInline inline)
        {
            // Ignore emphases inside code blocks.
            if (!IsIndentedCode && !IsFencedCode)
                mEmphasesWriter.AppendEmphases(inline);
        }

        /// <summary>
        /// Appends a specified text to the content lines.
        /// </summary>
        internal void AppendText(string text, bool isNeedEscaping = true)
        {
            if (!StringUtil.HasChars(text))
                return;

            // If text starts from the same character as InlineCode delimiter and InlineCode emphasis is
            // in 'Opening' state, then we need to escape this character with space to avoid it to be
            // a continuation of the opening sequence of the InlineCode delimiter.
            if ((text[0] == InlineCodeDelimiter.Character) && mEmphasesWriter.IsInlineCodeOpening)
                CurrentLine.Append(' ');

            foreach (char c in text)
            {
                switch (c)
                {
                    case ControlChar.LineBreakChar:
                    {
                        mContentLines.NewLine();
                        break;
                    }
                    case ControlChar.PageBreakChar:
                    {
                        if (mWriter.SaveOptions.ForcePageBreaks)
                            CurrentLine.Append(ControlChar.PageBreak);
                        break;
                    }
                    case '.':
                    {
                        // See TestListEscaping for more details.
                        // WORDSNET-25298 Consider 'isNeedEscaping' option.
                        if (isNeedEscaping &&
                            (GetBlockType(CurrentLine + ".") == BlockType.OrderedListItem) && (!mParagraph.IsListItem))
                        {
                            CurrentLine.Append(ControlChar.BackslashChar);
                        }

                        mEmphasesWriter.FlushPendingOpeningEmphases();
                        CurrentLine.Append('.');
                        break;
                    }
                    default:
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            // Check if the current line starts with whitespace characters,
                            // then wrap it into InlineCode to preserve in exported Markdown paragraph.
                            if ((CurrentLine.Length > 0) && StringUtil.ContainsOnlyWhitespaces(CurrentLine))
                            {
                                // Let's consider single whitespace in footnote description as a separator between
                                // its reference and its text and do not wrap it into InlineCode markers.
                                if (!IsInFootnote || CurrentLine.Length > 1)
                                    WrapIntoInlineCode();
                            }

                            mEmphasesWriter.FlushPendingOpeningEmphases();

                            if (isNeedEscaping)
                            {
                                bool isInCode = (IsInCodeBlock || mEmphasesWriter.IsInInlineCode);
                                if (!isInCode && !mWriter.UseHtmlSyntax && MarkdownUtil.IsEscapableMarkupCharacter(c))
                                    CurrentLine.Append(ControlChar.BackslashChar);
                            }
                        }

                        CurrentLine.Append(FontUtil.UnicodeToSymbol(c));
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns <paramref name="node"/> if it is numbered paragraph itself,
        /// or previous sibling numbered paragraph that has almost
        /// the same left indent as a specified node.
        /// <seealso cref="MaxLeftIndentDeviationPercent"/>.
        /// </summary>
        internal static Paragraph GetListItemSource(Node node)
        {
            if ((node.NodeType == NodeType.Paragraph) && ((Paragraph)node).IsListItem)
                return (Paragraph)node;

            // WORDSNET-26133 Consider also 'Markdown' list item.
            int leftIndent = MarkdownUtil.GetLeftIndent(node);
            if (leftIndent == -1)
                return null;

            Node prevNode = node.PreviousSibling;
            while (prevNode != null)
            {
                int prevLeftIndent = MarkdownUtil.GetLeftIndent(prevNode);
                if (prevLeftIndent == -1)
                    return null;

                // WORDSNET-26757 Allow non-strict comparision with specified allowed deviation.
                double deviation = System.Math.Abs(100.0 - (double)leftIndent / prevLeftIndent * 100.0);
                if (deviation > MaxLeftIndentDeviationPercent)
                    return null;

                if ((prevNode.NodeType == NodeType.Paragraph) && ((Paragraph)prevNode).IsListItem)
                    return (Paragraph)prevNode;

                prevNode = prevNode.PreviousSibling;
            }

            return null;
        }

        /// <summary>
        /// Returns <see cref="MarkdownParagraphWriter"/> corresponding to a specified paragraph node.
        /// </summary>
        internal MarkdownParagraphWriter GetParagraphWriterByNode(Paragraph paragraph)
        {
            if (paragraph == null)
                return null;

            if (mParagraph == paragraph)
                return this;

            MarkdownParagraphWriter prevWriter = mPrevParagraphWriter;
            while (prevWriter != null)
            {
                if (prevWriter.mParagraph == paragraph)
                    return prevWriter;

                prevWriter = prevWriter.mPrevParagraphWriter;
            }

            return null;
        }

        /// <summary>
        /// Gets a boolean value, indicating the paragraph is empty or contains only cross annotation.
        /// </summary>
        internal bool IsEmpty
        {
            get { return mParagraph.IsEmptyOrContainsOnlyCrossAnnotation; }
        }

        /// <summary>
        /// Initializes the writer.
        /// </summary>
        private void Init()
        {
            // Iterate over all styles to restore markdown feature blocks.
            Style style = mParagraph.ParagraphStyle;
            while (style != null)
            {
                BlockType blockType = MarkdownUtil.ToBlockType(style);
                switch (blockType)
                {
                    case BlockType.BulletListItem:
                    case BlockType.OrderedListItem:
                    {
                        // WORDSNET-24544 A new save option is introduced, consider it.
                        if (mWriter.SaveOptions.ListExportMode == MarkdownListExportMode.PlainText)
                            break;

                        if (ListStylesLabels.Count == 0)
                            mFirstListStyleLabelPosition = mStylesMarkers.Length;

                        string listLabel = GetListLabel(style.ParaPr);
                        mStylesMarkers.Append(listLabel);

                        if (listLabel.Length > 0)
                            ListStylesLabels.Add(listLabel);

                        if (mStartAt == 0)
                        {
                            List list = style.GetListInternal();
                            if (list != null)
                            {
                                int levelNumber = (int)((IParaAttrSource)style).FetchParaAttr(ParaAttr.ListLevel);
                                ListLevel level = list.ListLevels[levelNumber];
                                mStartAt = level.StartAt;
                            }
                        }

                        break;
                    }
                    case BlockType.Quote:
                    {
                        mStylesMarkers.Append('>');
                        // Do not separate quote marks with space inside an opening quote sequence.
                        BlockType nextBlockType = MarkdownUtil.ToBlockType(style.GetBaseStyle());
                        if (nextBlockType != BlockType.Quote)
                            mStylesMarkers.Append(' ');

                        mQuoteLevel++;
                        break;
                    }
                    case BlockType.AtxHeading:
                    {
                        // WORDSNET-25705 Do not override first occurred Heading style with any possible parent Heading style.
                        if (mAtxHeading == null)
                            mAtxHeading = style;
                        break;
                    }
                    case BlockType.FencedCode:
                    {
                        mFencedCode = style;
                        break;
                    }
                    case BlockType.IndentedCode:
                    {
                        mIsIndentedCode = true;
                        break;
                    }
                    case BlockType.SetextHeading:
                    {
                        mIsSetextHeading = true;
                        break;
                    }
                    default:
                        break;
                }

                style = style.GetBaseStyle();
            }

            // WORDSNET-19780 Append BlockQuote defined in HtmlBlock of the paragraph, if any.
            if (mParagraph.HtmlBlockQuoteLevel > 0)
            {
                mQuoteLevel = mParagraph.HtmlBlockQuoteLevel - 1;

                StringBuilder blockQuote = new StringBuilder(new string('>', mParagraph.HtmlBlockQuoteLevel));
                blockQuote.Append(' ');

                mStylesMarkers.Insert(0, blockQuote.ToString());
            }

            if (mWriter.SaveOptions.ListExportMode == MarkdownListExportMode.MarkdownSyntax)
            {
                if (mStartAt == 0)
                {
                    ListLevel listLevel = mParagraph.GetListLevel(false);
                    if (listLevel != null)
                        mStartAt = listLevel.StartAt;
                }

                // Need this update to determine properly either we need paragraph break in IsNeedParagraphBreak.
                // We should rework it there some day to remove this call.
                UpdateListLabels();
            }

            mParentFootnote = mWriter.FootnoteWriter.Footnote;
        }

        /// <summary>
        /// Writes content lines into underlying writer.
        /// </summary>
        private void WriteContentLines()
        {
            for (int i = 0; i < mContentLines.Count; i++)
            {
                StringBuilder contentLine = mContentLines[i];

                WriteStylesMarkers(contentLine);

                // Write spaces to make lazy continuation in multiline list item or footnote.
                if (IsNeedLazyContinuation(contentLine, i))
                {
                    // Lazy continuation in footnotes is blank line + indentation spaces.
                    if (IsInFootnote)
                        Builder.Append(ParagraphBreak);

                    Builder.Append(' ', Block.MaxIndentationLength + 1);
                }

                // Escape first character of the content line if it starts from a sequence that can be recognized
                // as a some non-regular paragraph block.
                if ((mStylesMarkers.Length == 0) && (contentLine.Length > 0))
                    EscapeContentLine(contentLine);

                // Append content line text.
                Builder.Append(contentLine.Replace(MarkdownUtil.HardLineBreakSlash, ParagraphBreak));

                // Write AtxHeading closing sequence.
                if (IsHeading)
                {
                    if (IsInCell)
                        WriteHtmlHeading(false);
                    else if (!mIsSetextHeading)
                        WriteAtxHeadingClosing();
                }

                // Write paragraph break for all lines except of the very last.
                if (i < (mContentLines.Count - 1))
                {
                    if (mParagraph.IsInCell)
                    {
                        // Line breaks are not allowed inside tables, so use html tag <br>.
                        Builder.Append(HtmlLineBreak);
                    }
                    else
                    {
                        // WORDSNET-18341 Implemented hard line breaks.
                        // We don't write line break inside code blocks and footnotes.
                        // Also, we don't write it in a very first line, if it is empty.
                        if (!IsInCodeBlock && !IsInFootnote && ((contentLine.Length > 0) || (i > 0)))
                        {
                            Builder.Append(
                                (mWriter.SaveOptions.LineBreakExportMode == MarkdownLineBreakExportMode.Backslash) ||
                                StringUtil.ContainsOnlyWhitespaces(contentLine)
                                    ? "\\"
                                    : "  ");
                        }

                        Builder.Append(ParagraphBreak);
                    }
                }

                // The non-blank list label ('-', '+', '*' or ordinal number) must be written only for a very first line of
                // a multiline paragraph. All the rest of lines should be written with a blank list label.
                if (HasDirectListLabel && (i == FirstContentLineIndex))
                {
                    for (int j = 0; j < DirectListLabel.Length - 1; j++)
                        mStylesMarkers[mDirectListLabelPosition + j] = ' ';
                }
            }
        }

        /// <summary>
        /// Returns <see cref="MarkdownParagraphWriter"/> corresponding to <see cref="GetListItemSource"/>
        /// list item of the previous sibling node of this <see cref="mParagraph"/>.
        /// </summary>
        private MarkdownParagraphWriter GetPrevListItemWriter()
        {
            Node prevNode = mParagraph.PreviousSibling;
            if (prevNode == null)
                return null;
            Paragraph prevListItem = GetListItemSource(prevNode);
            return GetParagraphWriterByNode(prevListItem);
        }

        /// <summary>
        /// Updates styles markers.
        /// </summary>
        private void UpdateStylesMarkers()
        {
            // WORDSNET-24544 A new save option is introduced, consider it.
            if (IsMarkdownListItem && (mWriter.SaveOptions.ListExportMode == MarkdownListExportMode.MarkdownSyntax))
            {
                UpdateListLabels();

                // There can be paragraph with list level greater than number of applied list styles. In this case
                // we need to append empty list markers. Also note, there cannot be missed list styles if previous
                // paragraph is not a list item.
                MarkdownParagraphWriter prevListItemWriter = GetPrevListItemWriter();
                if (prevListItemWriter != null)
                    InsertMissedListLevels(prevListItemWriter.FirstListStyleLabelPosition);

                // Append direct list label before very first Leaf block.
                AppendDirectListLabel();
            }

            AppendLeafBlocks();
        }

        /// <summary>
        /// Updates <see cref="ListLabels"/> for the underlying paragraph.
        /// </summary>
        private void UpdateListLabels()
        {
            int missedListLevelsCount = GetMissedListLevelsCount();

            MarkdownParagraphWriter prevListItemWriter = GetPrevListItemWriter();
            if (prevListItemWriter != null)
            {
                // Update missed list levels with blank label.
                for (int i = 0; i < missedListLevelsCount; i++)
                {
                    ListLabels[i] = (prevListItemWriter.ListLabels[i] == null)
                        ? ""
                        : new string(' ', prevListItemWriter.ListLabels[i].Length);
                }
            }

            // Update labels from the List styles of the paragraph.
            for (int i = missedListLevelsCount; i < ListStylesLabels.Count; i++)
                ListLabels[i] = ListStylesLabels[i];

            // At last, update direct list label.
            if (HasDirectListLabel)
                ListLabels[ListLevel] = DirectListLabel;
        }

        /// <summary>
        /// Writes styles markers for a content line with a specified index to a specified writer.
        /// </summary>
        private void WriteStylesMarkers(StringBuilder contentLine)
        {
            if (StringUtil.ContainsOnlyWhitespaces(contentLine))
            {
                // Do not write list marker for empty line.
                string stylesMarkers;
                if (ListStylesLabels.Count > 0)
                {
                    stylesMarkers = mStylesMarkers.ToString(0, FirstListStyleLabelPosition);
                } else if (HasDirectListLabel)
                {
                    stylesMarkers = mStylesMarkers.ToString(0, mStylesMarkers.Length - DirectListLabel.Length);
                }
                else
                {
                    stylesMarkers = mStylesMarkers.ToString();
                }

                // Do not write an extra spaces at the very end of styles markers for empty content line.
                Builder.Append(stylesMarkers.TrimEnd());
            }
            else
            {
                Builder.Append(mStylesMarkers);
            }
        }

        /// <summary>
        /// Writes fenced code closing sequence.
        /// </summary>
        private void WriteFencedCodeOpening()
        {
            // Fenced code is not allowed inside cell.
            if (IsInCell)
                return;

            Builder.Append(mStylesMarkers);
            Builder.Append(FencedCodeDelimiter);
            if (FencedCodeInfoString.Length > 0)
                Builder.Append(' ');
            Builder.Append(FencedCodeInfoString);
            Builder.Append(ParagraphBreak);
        }

        /// <summary>
        /// Writes fenced code closing sequence.
        /// </summary>
        private void WriteFencedCodeClosing()
        {
            // Fenced code is not allowed inside cell.
            if (IsInCell)
                return;

            Builder.Append(ParagraphBreak);
            Builder.Append(mStylesMarkers);
            Builder.Append(FencedCodeDelimiter);
        }

        /// <summary>
        /// Writes SetextHeading opening sequence.
        /// </summary>
        private void WriteSetextHeading()
        {
            // SetextHeading is not allowed inside cell.
            if (IsInCell)
                return;

            Builder.Append(ParagraphBreak);

            // There should not be a non-blank list item markers in SetextHeading opening sequence.
            if (HasDirectListLabel)
            {
                for (int j = 0; j < DirectListLabel.Length - 1; j++)
                    mStylesMarkers[mDirectListLabelPosition + j] = ' ';
            }

            Builder.Append(mStylesMarkers);
            Builder.Append(MarkdownUtil.LevelToSetextHeadingChar(SetextHeadingLevel), mContentLines.MaxLength);
        }

        /// <summary>
        /// Writes HTML heading tag.
        /// </summary>
        /// <param name="isOpen">When true, writes open tag. Otherwise, writes close tag.</param>
        private void WriteHtmlHeading(bool isOpen)
        {
            Debug.Assert(IsHeading);
            Builder.Append(string.Format((isOpen) ? HtmlHeadingOpen : HtmlHeadingClose, AtxHeadingLevel));
        }

        /// <summary>
        /// Appends AtxHeading opening sequence to styles markers.
        /// </summary>
        private void AppendAtxHeading()
        {
            mStylesMarkers.Append('#', AtxHeadingLevel);
            mStylesMarkers.Append(' ');
        }

        /// <summary>
        /// Appends IndentedCode opening sequence to styles markers.
        /// </summary>
        private void AppendIndentedCode()
        {
            mStylesMarkers.Append(' ', Block.MaxIndentationLength + 1);
        }

        /// <summary>
        /// Appends Leaf blocks opening sequences to styles markers.
        /// </summary>
        private void AppendLeafBlocks()
        {
            if (IsIndentedCode)
            {
                AppendIndentedCode();
            }
            else if (IsHeading)
            {
                if (IsInCell)
                {
                    // WORDSNET-20407 Headings are not allowed inside cells in markdown.
                    // Write them as corresponding <h1>-<h6> HTML tags.
                    WriteHtmlHeading(true);
                }
                else
                {
                    // An ATXHeading cannot be multiline, so it should be converted to a SetextHeading in this case.
                    if (IsMultiline)
                        mIsSetextHeading = true;

                    if (!mIsSetextHeading)
                        AppendAtxHeading();
                }
            }
        }

        /// <summary>
        /// Returns string representing list label for a specified ParaPr.
        /// </summary>
        private string GetListLabel(ParaPr paraPr)
        {
            return GetListLabel(paraPr.ListId, paraPr.ListLevel);
        }

        /// <summary>
        /// Returns string representing list label for a specified listId and level number.
        /// </summary>
        private string GetListLabel(int listId, int levelNumber)
        {
            if ((listId == 0) || (mWriter.SaveOptions.ListExportMode == MarkdownListExportMode.PlainText))
                return "";

            // WORDSNET-22782 Consider style reference.
            List list = Document.Lists.FetchListByListIdResolveStyleReference(listId);

            bool isBulletList = (list.ListDef.Levels[0].NumberStyle == NumberStyle.Bullet);

            // WORDSNET-20409 Lists are not allowed inside tables,
            // so take actual list label of the numbered paragraph.
            if (IsInCell && !isBulletList)
            {
                mWriter.UpdateListLabels();
                return string.Format("{0} ", mParagraph.ListLabel.LabelString);
            }

            // WORDSNET-27265 Added resilience by fetching list level instead of getting it directly by index.
            ListLevel listLevel = (isBulletList)
                ? list.ListDef.Levels.FetchListLevel(0)
                : list.ListDef.Levels.FetchListLevel(levelNumber);

            if (listLevel.NumberFormat.Length <= 0)
                return "";

            char listMarkerChar = listLevel.NumberFormat[listLevel.NumberFormat.Length - 1];

            ListMarker marker = (isBulletList)
                ? MarkdownUtil.ToBulletListMarker(listMarkerChar)
                : MarkdownUtil.ToOrderedListMarker(listMarkerChar);

            string startAt = (isBulletList) ? "" : FormatterPal.IntToStr(listLevel.StartAt);

            return string.Format("{0}{1} ", startAt, MarkdownUtil.ListMarkerToChar(marker));
        }

        /// <summary>
        /// Gets a boolean value indicating either a specified content line
        /// should be written as lazy continuation of the previous line.
        /// </summary>
        private bool IsNeedLazyContinuation(StringBuilder contentLine, int lineIndex)
        {
            // We need to indent all lines except of the very first line in footnote.
            if (IsInFootnote && ((lineIndex != 0) || !IsFirstParaInFootnote))
                return true;

            if (lineIndex <= FirstContentLineIndex)
                return false;

            if (!IsListItem)
                return false;

            if (MarkdownUtil.IsOrderedListItem(contentLine))
            {
                // The ordered list must start from '1' to break a paragraph.
                if ((contentLine[0] != '1') || !MarkdownUtil.IsOrderedListMarker(contentLine[1]))
                    return false;
            }
            else if (!MarkdownUtil.IsBulletListItem(contentLine))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Appends direct list label to styles markers.
        /// </summary>
        private void AppendDirectListLabel()
        {
            // Append direct list label.
            mDirectListLabelPosition = mStylesMarkers.Length;
            mStylesMarkers.Append(DirectListLabel);

            // Update first list style label position.
            if (mFirstListStyleLabelPosition == -1)
                mFirstListStyleLabelPosition = mDirectListLabelPosition;
        }

        /// <summary>
        /// Inserts list labels of the missed list levels into styles markers at a specified position.
        /// </summary>
        private void InsertMissedListLevels(int position)
        {
            Debug.Assert(IsMarkdownListItem);

            int missedListLevelsCount = GetMissedListLevelsCount();
            if (missedListLevelsCount < 1)
                return;

            Debug.Assert(position >= 0);
            mFirstListStyleLabelPosition = position;

            for (int i = 0; i < missedListLevelsCount; i++)
            {
                // Missed list levels should be filled with blank list marker.
                Debug.Assert((ListLabels[i] != null) && StringUtil.ContainsOnlyWhitespaces(ListLabels[i]));

                // Whitespace characters in cell will be stripped, so use HTML tag.
                string missedLevelFiller = (IsInCell) ? HtmlIndent : ListLabels[i];
                mStylesMarkers.Insert(mFirstListStyleLabelPosition, missedLevelFiller);
                // Update direct list marker position.
                mDirectListLabelPosition += missedLevelFiller.Length;
            }
        }

        /// <summary>
        /// Returns number of missed list levels for the paragraph.
        /// </summary>
        private int GetMissedListLevelsCount()
        {
            int labelsCount = ListStylesLabels.Count;
            if (HasDirectListLabel)
                labelsCount++;

            int missedListLevelsCount = (ListLevel + 1) - labelsCount;

            // There can be numbering specified only in list style, when direct numbering is missed. Or even list style
            // without numbering (it has 'List' in abbreviation, but the numbering is not specified).
            // In all such cases we treat it as there are no missed styles.
            return System.Math.Max(0, missedListLevelsCount);
        }

        /// <summary>
        /// Writes AtxHeading closing sequence to the underlying writer.
        /// </summary>
        /// <remarks>
        /// If line of text ends with the same substring as AtxHeading opening sequence,
        /// than we also need to write a closing sequence. Otherwise, this line ending
        /// will be lost, because it will be treated as the closing sequence in markdown.
        /// </remarks>
        private void WriteAtxHeadingClosing()
        {
            Debug.Assert(IsHeading && !mIsSetextHeading);

            string closingSequence = string.Format(" {0}", new string('#', AtxHeadingLevel));
            if (StringUtil.IsEndsWith(mContentLines[FirstContentLineIndex], closingSequence))
                Builder.Append(closingSequence);
        }

        /// <summary>
        /// Writes escaping character before content line to the underlying writer.
        /// </summary>
        private void EscapeContentLine(StringBuilder contentLine)
        {
            Debug.Assert((contentLine != null) && (contentLine.Length > 0));

            // WORDSNET-20408 Also ignore tables as there cannot be any other markdown features.
            if (IsFencedCode || IsHorizontalRule || IsInCell)
                return;

            // Avoid possible indentation code.
            StringUtil.TrimStart(contentLine);

            // WORDSNET-20394 Skip completely empty lines as there is nothing to escape.
            if (contentLine.Length == 0)
                return;

            if (GetBlockType(contentLine.ToString()) != BlockType.Paragraph)
                Builder.Append('\\');
        }

        /// <summary>
        /// Wraps current content line into InlineCode delimiters.
        /// </summary>
        private void WrapIntoInlineCode()
        {
            // Do not wrap if this is inside another code block already.
            if (IsFencedCode || IsIndentedCode)
                return;

            CurrentLine.Insert(0, InlineCodeDelimiter.Character);
            CurrentLine.Append(InlineCodeDelimiter.Character);
        }

        /// <summary>
        /// Returns the BlockType of the specified text fragment.
        /// </summary>
        private BlockType GetBlockType(string text)
        {
            return BlockParser.Parse(text, 0).Type;
        }

        /// <summary>
        /// Writes empty paragraph.
        /// </summary>
        private void WriteEmptyParagraph()
        {
            if (mWriter.SaveOptions.EmptyParagraphExportMode == MarkdownEmptyParagraphExportMode.None)
            {
                if (IsLastEmptyParagraph && IsLazyContinuation)
                    Builder.Append(ParagraphBreak);

                return;
            }

            if (mWriter.SaveOptions.EmptyParagraphExportMode == MarkdownEmptyParagraphExportMode.MarkdownHardLineBreak)
            {
                if (IsInCell)
                {
                    // &nbsp
                    Builder.Append('\u00A0');
                }
                else
                {
                    // If after an empty paragraph goes node that can be a lazy continuation for this last
                    // empty paragraph, then when we write Markdown hard line break '\' character,
                    // it is displayed 'as is' instead of making line break. So we need to write
                    // HTML <br> tag instead.
                    string breakStr = (IsLastEmptyParagraph && !IsLazyContinuation) ? HtmlLineBreak : "\\";
                    Builder.Append(breakStr);

                    if (IsFirstEmptyParagraph && (breakStr != HtmlLineBreak))
                    {
                        Builder.Append(ParagraphBreak);
                        Builder.Append(breakStr);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a boolean value, indicating either this paragraph is empty
        /// and there are no other empty paragraphs before it.
        /// </summary>
        private bool IsFirstEmptyParagraph
        {
            get
            {
                if (!mParagraph.IsEmptyOrContainsOnlyCrossAnnotation)
                    return false;

                Paragraph prevParagraph = mParagraph.PreviousSibling as Paragraph;
                if (prevParagraph == null)
                    return true;

                return !prevParagraph.IsEmptyOrContainsOnlyCrossAnnotation;
            }
        }

        /// <summary>
        /// Gets a boolean value, indicating either this paragraph is empty
        /// and there are no other empty paragraphs after it.
        /// </summary>
        private bool IsLastEmptyParagraph
        {
            get
            {
                if (!mParagraph.IsEmptyOrContainsOnlyCrossAnnotation)
                    return false;

                Paragraph nextParagraph = mParagraph.NextSibling as Paragraph;
                if (nextParagraph == null)
                    return true;

                return !nextParagraph.IsEmptyOrContainsOnlyCrossAnnotation;
            }
        }

        /// <summary>
        /// Gets a boolean value, indicating either this paragraph
        /// produces a lazy continuation with next sibling node.
        /// </summary>
        private bool IsLazyContinuation
        {
            get
            {
                Paragraph nextParagraph = mParagraph.NextSibling as Paragraph;
                if (nextParagraph == null)
                    return false;

                if (nextParagraph.IsListItem)
                    return false;

                BlockType blockType = MarkdownUtil.ToBlockType(nextParagraph.ParagraphStyle);
                return (blockType == BlockType.Paragraph) || (blockType == BlockType.InlineCode);
            }
        }

#if DEBUG
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            Style style = mParagraph.ParagraphStyle;
            while (style != null)
            {
                sb.Append(style.Name);
                if (style.StyleIdentifier != StyleIdentifier.Normal)
                    sb.Append("->");

                style = style.GetBaseStyle();
            }

            sb.AppendFormat(": {0}", StringUtil.Ellipsisize(mParagraph.GetText(), 50));

            return sb.ToString();
        }
#endif

        /// <summary>
        /// Returns True if Html syntax shall be used to correctly write the attributes of the current paragraph.
        /// </summary>
        internal bool HtmlSyntaxRequired
        {
            get
            {
                foreach (Run run in mParagraph.Runs)
                {
                    // MD syntax inside html-inline is not allowed. Sup/sub emphasis works only in html-inline syntax.
                    // Thus, if there are other emphases, they must also be in html-inline syntax.
                    if ((run.RunPr.VerticalAlignment != RunVerticalAlignment.Baseline) &&
                        (run.RunPr.Bold.ToBool() || run.RunPr.Italic.ToBool() || run.RunPr.StrikeThrough.ToBool()))
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets list label strings for every list level of the list to which this paragraph belongs.
        /// </summary>
        internal string[] ListLabels
        {
            get
            {
                if (mListLabels == null)
                    mListLabels = new string[9];

                return mListLabels;
            }
        }

        /// <summary>
        /// Gets a document being written.
        /// </summary>
        private DocumentBase Document
        {
            get { return mWriter.Document; }
        }

        /// <summary>
        /// Gets current builder.
        /// </summary>
        private StringBuilder Builder
        {
            get { return mWriter.Builder; }
        }

        /// <summary>
        /// Gets string representing current paragraph break value to use in the writer.
        /// </summary>
        private string ParagraphBreak
        {
            get { return mWriter.SaveOptions.ParagraphBreak; }
        }

        /// <summary>
        /// Gets current line of content lines.
        /// </summary>
        private StringBuilder CurrentLine
        {
            get { return mContentLines.CurrentLine; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is a HorizontalRule.
        /// </summary>
        private bool IsHorizontalRule
        {
            get
            {
                if (mParagraph == null)
                    return false;

                if ((mParagraph.FirstChild == null) || (mParagraph.FirstChild.NodeType != NodeType.Shape))
                    return false;

                HorizontalRule horizontalRule = ((Shape)(mParagraph.FirstChild)).HorizontalRule;
                if (horizontalRule == null)
                    return false;

                return horizontalRule.On;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is a list item.
        /// </summary>
        private bool IsListItem
        {
            get { return mParagraph.IsListItem; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is a list item in terms of Markdown.
        /// </summary>
        /// <remarks>
        /// We introduced this behavior in WORDSNET-26133. If non-numbered paragraph in model has the
        /// same left indent as the previous numbered paragraph, then we treat such paragraph as
        /// numbered in Markdown. This behavior is compliant with many third-party Markdown editors.
        /// </remarks>
        private bool IsMarkdownListItem
        {
            get { return IsListItem || IsSameLeftIndentAsPreviousListItem; }
        }

        /// <summary>
        /// Gets a boolean value, indicating the paragraph has ordered number style.
        /// </summary>
        private bool HasOrderedNumberStyle
        {
            get
            {
                Debug.Assert(IsMarkdownListItem);

                int listId = mParagraph.ParaPr.ListId;
                // WORDSNET-23981 Get list considering style reference.
                List list = Document.Lists.FetchListByListIdResolveStyleReference(listId);

                return (list.ListDef.Levels[0].NumberStyle == NumberStyle.Arabic);
            }
        }

        /// <summary>
        /// Gets a boolean value indicating the paragraph has direct list label.
        /// </summary>
        private bool HasDirectListLabel
        {
            get { return (DirectListLabel.Length > 0) && !StringUtil.ContainsOnlyWhitespaces(DirectListLabel); }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is an AtxHeading.
        /// </summary>
        private bool IsHeading
        {
            get { return (AtxHeadingLevel > 0); }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is a FencedCode.
        /// </summary>
        private bool IsFencedCode
        {
            get { return mFencedCode != null; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is an IndentedCode.
        /// </summary>
        private bool IsIndentedCode
        {
            get { return mIsIndentedCode; }
        }

        /// <summary>
        /// Gets a boolean value indicating the paragraph is one of the Code blocks.
        /// </summary>
        private bool IsInCodeBlock
        {
            get { return (IsFencedCode || IsIndentedCode); }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is inside a Cell.
        /// </summary>
        private bool IsInCell
        {
            get { return mParagraph.IsInCell; }
        }

        /// <summary>
        /// Gets a boolean value indicating either the paragraph is inside Footnote.
        /// </summary>
        private bool IsInFootnote
        {
            get { return mParentFootnote != null; }
        }

        /// <summary>
        /// Gets a boolean value indicating that the paragraph is the very first inside a Footnote.
        /// </summary>
        private bool IsFirstParaInFootnote
        {
            get
            {
                if (IsInFootnote)
                {
                    return (mPrevParagraphWriter == null) || (mParentFootnote != mPrevParagraphWriter.mParentFootnote);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets an integer value representing a topmost Quote level.
        /// </summary>
        private int QuoteLevel
        {
            get { return mQuoteLevel; }
        }

        /// <summary>
        /// The position of the first occurred list label inside output string.
        /// </summary>
        private int FirstListStyleLabelPosition
        {
            get { return mFirstListStyleLabelPosition; }
        }

        /// <summary>
        /// Gets an integer value representing a first level of the list this paragraph belongs which label is not blank.
        /// </summary>
        private int FirstNonBlankLabelLevel
        {
            get
            {
                for (int i = 0; i < ListLabels.Length; i++)
                {
                    if (!StringUtil.ContainsOnlyWhitespaces(ListLabels[i]))
                        return i;
                }

                return -1;
            }
        }

        /// <summary>
        /// Gets FencedCode info string.
        /// </summary>
        private string FencedCodeInfoString
        {
            get
            {
                if (!IsFencedCode)
                    return "";

                if (mFencedCodeInfoString == null)
                {
                    string styleName = mFencedCode.Name;

                    int start = MarkdownUtil.FencedCodeStyleName.Length;
                    // One dot (.) is allowed just after style name to increase readability.
                    if ((start < styleName.Length) && (styleName[start] == '.'))
                        start++;

                    mFencedCodeInfoString = styleName.Substring(start, (styleName.Length - start));
                }

                return mFencedCodeInfoString;
            }
        }

        /// <summary>
        /// Gets direct list marker of the current paragraph.
        /// </summary>
        private string DirectListLabel
        {
            get
            {
                if (mDirectListLabel == null)
                {
                    // WORDSNET-21727 There can be situation when there are numbered styles, but no one of them is started
                    // from 'List' and no direct listId is specified at the paragraph. In this case the paragraph is
                    // still numbered and we need to set direct list label.
                    if (mParagraph.ParaPr.Contains(ParaAttr.ListId))
                    {
                        // 1) If list is defined in direct attributes, then just use it.
                        mDirectListLabel = GetListLabel(mParagraph.ParaPr);
                    }
                    else
                    {
                        // 2) Otherwise, lets check that there is at least one numbered style with name started with 'List'.
                        // In this case the paragraph numbering will be accounted for such style and the label is added
                        // into mListStylesLabels (i.e. no need to set any direct list label, it is empty string).
                        int listId = 0;
                        Style style = mParagraph.ParagraphStyle;
                        while (style != null)
                        {
                            if (style.ParaPr.Contains(ParaAttr.ListId))
                            {
                                int curListId = style.ParaPr.ListId;
                                // Remember first occurred listId to avoid getting it below one more time.
                                if (listId == 0)
                                    listId = curListId;

                                // WORDSNET-22782 Check also whether style is set explicitly to not a list item.
                                if ((curListId == 0) || style.Name.StartsWith(MarkdownUtil.ListStyleName, StringComparison.Ordinal))
                                {
                                    mDirectListLabel = "";
                                    break;
                                }
                            }

                            style = style.GetBaseStyle();
                        }

                        // 3) But if there is no numbered style with name started with 'List', there still can
                        // be numbered style. Such style is not accounted in mListStylesLabels, but the paragraph
                        // is obviously numbered. In this case we should account it in a direct list label.
                        if (mDirectListLabel == null)
                            mDirectListLabel = GetListLabel(listId, ListLevel);
                    }

                    if ((mDirectListLabel.Length > 0) && StringUtil.ContainsOnlyWhitespaces(mDirectListLabel))
                        mDirectListLabel = "";
                }

                return mDirectListLabel;
            }
        }

        /// <summary>
        /// Gets an integer value representing an AtxHeading level.
        /// </summary>
        private int AtxHeadingLevel
        {
            get { return (mAtxHeading != null) ? (int)mAtxHeading.StyleIdentifier : 0; }
        }

        /// <summary>
        /// Gets an integer value representing a SetextHeading level.
        /// </summary>
        private int SetextHeadingLevel
        {
            get { return (mIsSetextHeading) ? System.Math.Min(System.Math.Max(AtxHeadingLevel, 1), 2) : 0; }
        }

        /// <summary>
        /// Gets block parser object.
        /// </summary>
        private BlockParser BlockParser
        {
            get
            {
                if (mBlockParser == null)
                    mBlockParser = new BlockParser();

                return mBlockParser;
            }
        }

        /// <summary>
        /// Gets list of strings representing labels for every List style of the paragraph.
        /// </summary>
        private List<string> ListStylesLabels
        {
            get
            {
                if (mListStylesLabels == null)
                    mListStylesLabels = new List<string>();

                return mListStylesLabels;
            }
        }

        /// <summary>
        /// Gets list level of the paragraph.
        /// </summary>
        private int ListLevel
        {
            get
            {
                if (mListLevel == -1)
                {
                    // WORDSNET-26133 Consider also 'Markdown' list item.
                    Paragraph listItem = GetListItemSource(mParagraph);
                    if (listItem != null)
                        mListLevel = (int)listItem.FetchParaAttr(ParaAttr.ListLevel, RevisionsView.Original);
                }

                return mListLevel;
            }
        }

        /// <summary>
        /// Gets an index of the first content line of the paragraph.
        /// </summary>
        /// <remarks>
        /// We separate some paragraphs with blank line (<see cref="IsNeedParagraphBreak"/>) by inserting one empty line
        /// before the array of content lines. But this line should not be treated as the content line, but is just a separator.
        /// </remarks>
        private int FirstContentLineIndex
        {
            get
            {
                if (mContentLines.Count <= 1)
                    return mContentLines.Count - 1;

                // Do not consider very first blank line, because it is not a content line, but paragraph separator.
                return (StringUtil.ContainsOnlyWhitespaces(mContentLines[0])) ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating that current paragraph is multiline.
        /// </summary>
        private bool IsMultiline
        {
            get
            {
                // Do not consider very first blank line, because it is not a content line, but paragraph separator.
                return ((mContentLines.Count > 2) ||
                        ((mContentLines.Count == 2) && !StringUtil.ContainsOnlyWhitespaces(mContentLines[0])));
            }
        }

        /// <summary>
        /// Gets a boolean value indicating whether we need to append a paragraph break.
        /// </summary>
        private bool IsNeedParagraphBreak
        {
            get
            {
                // Don't add blank line if it is a very first paragraph.
                if (mPrevParagraphWriter == null)
                    return false;

                // GitLab (as well as some other editors), which we try to mimic,
                // does not require the break between paragraphs in footnote definitions.
                // But note, it is required for multiline definitions.
                if (IsInFootnote)
                    return false;

                // There cannot be multiple paragraphs within cell in markdown format
                // (multiline can be achieved through the inline html blocks <p>, <br>).
                if (IsInCell)
                    return false;

                // If this is already a blank line, i.e. separator itself,
                // then there is no need to separate it once again.
                if (IsEmpty)
                    return false;

                // If previous paragraph is already a blank line, i.e. separator itself,
                // then there is no need to separate it once again.
                if (mPrevParagraphWriter.IsEmpty)
                    return false;

                // There is no need to add a blank line before or after Heading because it is not appendable by spec.
                if (IsHeading || mPrevParagraphWriter.IsHeading)
                    return false;

                // No need to separate Quotes when previous quote has less level.
                if (QuoteLevel > mPrevParagraphWriter.QuoteLevel)
                    return false;

                // There is no need to add a blank line after HorizontalRule because it is not appendable by spec.
                if (mPrevParagraphWriter.IsHorizontalRule)
                    return false;

                // There is no need to add a blank line before FencedCode.
                if (IsFencedCode)
                    return false;

                // There is no need to add a blank line after IndentedCode because it is not appendable by spec.
                if (mPrevParagraphWriter.IsIndentedCode)
                    return false;

                if (mPrevParagraphWriter.IsListItem)
                {
                    // A horizontal rule cannot be appended to a list item.
                    if (IsHorizontalRule)
                        return false;

                    if (IsListItem)
                    {
                        // When appending an ordered list that does not start at '1'
                        // we need a blank line. Otherwise, it can break the paragraph itself.
                        if ((HasOrderedNumberStyle && mPrevParagraphWriter.HasOrderedNumberStyle) &&
                            (FirstNonBlankLabelLevel > mPrevParagraphWriter.FirstNonBlankLabelLevel) &&
                            (mStartAt != 1))
                            return true;

                        // When current list can be determined unambiguously (it has non-empty label),
                        // then we do not need an additional blank line.
                        if (HasDirectListLabel || (GetMissedListLevelsCount() == 0))
                            return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either this paragraph has the
        /// same <see cref="ParaAttr.LeftIndent"/> as the previous numbered paragraph.
        /// </summary>
        private bool IsSameLeftIndentAsPreviousListItem
        {
            get { return (mParagraph.PreviousSibling != null) && (GetListItemSource(mParagraph.PreviousSibling) != null); }
        }

        /// <summary>
        /// The paragraph being written.
        /// </summary>
        private readonly Paragraph mParagraph;

        /// <summary>
        /// The content lines being collected from the underlying paragraph.
        /// </summary>
        private readonly TxtContentLines mContentLines = new TxtContentLines();

        /// <summary>
        /// The writer to write content into.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly MarkdownWriter mWriter;

        /// <summary>
        /// The previously written paragraph.
        /// </summary>
        private readonly MarkdownParagraphWriter mPrevParagraphWriter;

        /// <summary>
        /// The position of the first occurred list style label inside output string.
        /// </summary>
        private int mFirstListStyleLabelPosition = -1;

        /// <summary>
        /// A direct list label.
        /// </summary>
        private string mDirectListLabel;

        /// <summary>
        /// The Heading style of the paragraph.
        /// </summary>
        private Style mAtxHeading;

        /// <summary>
        /// The FencedCode style of the paragraph.
        /// </summary>
        private Style mFencedCode;

        /// <summary>
        /// The info string of the FencedCode.
        /// </summary>
        private string mFencedCodeInfoString;

        /// <summary>
        /// The nesting level of the topmost Quote.
        /// </summary>
        private int mQuoteLevel = -1;

        /// <summary>
        /// A boolean value indicating either the paragraph is an IndentedCode.
        /// </summary>
        private bool mIsIndentedCode;

        /// <summary>
        /// A boolean value indicating either the paragraph is a SetextHeading.
        /// </summary>
        private bool mIsSetextHeading;

        // The position of direct list label inside builder of styles markers.
        private int mDirectListLabelPosition;

        /// <summary>
        /// The parent footnote of the paragraph.
        /// </summary>
        private Footnote mParentFootnote;

        /// <summary>
        /// A string builder with styles markers of the paragraph.
        /// </summary>
        private readonly StringBuilder mStylesMarkers = new StringBuilder();

        /// <summary>
        /// Gets StartAt of the paragraph.
        /// </summary>
        /// <remarks>
        /// Note, this is a very first occurred StartAt of the paragraph among all its List styles and direct attributes.
        /// </remarks>
        private int mStartAt;
        private List<string> mListStylesLabels;
        private string[] mListLabels;
        private int mListLevel = -1;

        private BlockParser mBlockParser;
        private readonly MarkdownEmphasesWriter mEmphasesWriter;

        private const string HtmlLineBreak = "<br>";
        private const string HtmlHeadingOpen = "<h{0}>";
        private const string HtmlHeadingClose = "</h{0}>";
        private const string HtmlIndent = "&emsp;";
        private const string FencedCodeDelimiter = "~~~";

        /// <summary>
        /// The percent of maximum allowed deviation of the left indents of nodes when
        /// we take decision about either some node falls into a previous list.
        /// The allowed values are in range 0.0 - 100.0.
        /// </summary>
        private const double MaxLeftIndentDeviationPercent = 10.0;
    }
}
