// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2010 by Dmitry Vorobyev

using System;
using System.Text;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A TOC entry that is represented by a paragraph with a Heading (or other) style or an outline level.
    /// </summary>
    internal class ParagraphTocEntry : ITocEntry
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        internal ParagraphTocEntry(Paragraph paragraph, ParagraphTocEntryInfo entryInfo)
        {
            mParagraph = paragraph;
            mLevel = entryInfo.Level;
            mIsLinkedStyleTocEntry = entryInfo.IsLinkedStyleTocEntry;

            mFirstEntryNode = entryInfo.FirstChild;
            mLastEntryNode = entryInfo.LastChild;

            if (!entryInfo.IsEmptyTocEntry)
            {
                if (mFirstEntryNode == null)
                    mFirstEntryNode = paragraph.FirstNonMarkupDescendant;

                if (mLastEntryNode == null)
                    mLastEntryNode = paragraph.LastNonMarkupDescendant;
            }
        }

        NodeRange ITocEntry.InsertBookmark(string bookmarkName)
        {
            NodeRange range = GetEntryRange();
            if (range == null)
                return null;

            BookmarkStart bookmarkStart = new BookmarkStart(Document, bookmarkName);
            BookmarkEnd bookmarkEnd = new BookmarkEnd(Document, bookmarkName);

            if (range.Start.Node.NodeType == NodeType.Paragraph)
            {
                ((Paragraph)range.Start.Node).AppendChild(bookmarkStart);
                ((Paragraph)range.Start.Node).AppendChild(bookmarkEnd);
            }
            else
            {
                // WORDSNET-13419 Insert bookmark nodes to paragraph, like MS Word does.
                InsertStartToAncestorParagraph(range.Start.Node, bookmarkStart);
                InsertEndToAncestorParagraph(range.End.Node, bookmarkEnd);
            }

            return Bookmark.GetNodeRange(bookmarkStart, bookmarkEnd);
        }

        private static void InsertStartToAncestorParagraph(Node refNode, BookmarkStart start)
        {
            if (refNode.ParentNode.NodeType == NodeType.Paragraph)
            {
                refNode.InsertPrevious(start);
                return;
            }

            InsertStartToAncestorParagraph(refNode.ParentNode, start);
        }

        private static void InsertEndToAncestorParagraph(Node refNode, BookmarkEnd end)
        {
            if (refNode.ParentNode.NodeType == NodeType.Paragraph)
            {
                refNode.InsertNext(end);
                return;
            }

            InsertEndToAncestorParagraph(refNode.ParentNode, end);
        }

        int ITocEntry.Level
        {
            get { return mLevel; }
        }

        bool ITocEntry.OmitPageNumber
        {
            get { return false; }
        }

        Paragraph ITocEntry.Paragraph { get { return mParagraph; } }

        NodeRange ITocEntry.GetLabelRange()
        {
            // WORDSNET-10937 Skip if list label is empty.
            if (!HasLabelRange)
                return null;

            Run labelRun = new Run(Document, mParagraph.ListLabel.LabelStringFinal.Trim(), CloneRunPr());
            Paragraph labelPara = new Paragraph(Document);
            labelPara.ParagraphFormat.Style = mParagraph.GetParagraphStyle(RevisionsView.Final);
            labelPara.AppendChild(labelRun);

            string trailingCharacter;
            switch (mParagraph.ListFormat.ListLevelFinal.TrailingCharacter)
            {
                case ListTrailingCharacter.Tab:
                    trailingCharacter = ControlChar.Tab;
                    break;
                case ListTrailingCharacter.Space:
                case ListTrailingCharacter.Nothing:
                    trailingCharacter = ControlChar.Space;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Run trailingRun = new Run(Document, trailingCharacter, CloneRunPr());
            labelPara.AppendChild(trailingRun);

            return new NodeRange(labelRun, trailingRun);
        }

        private bool HasLabelRange
        {
            get { return mParagraph.HasListLabelFinal; }
        }

        private RunPr CloneRunPr()
        {
            RunPr runPr = mParagraph.ListLabel
                .GetExpandedRunPr(true, RunPrExpandFlags.Normal | RunPrExpandFlags.Revised)
                .Clone();

            // We should not use revision identifiers from source paragraph.
            foreach (int key in gIgnoreAttributeKeys)
                runPr.Remove(key);

            if (mIsLinkedStyleTocEntry)
            {
                NullableInt32 characterStyleIstd = GetCharacterStyleIstd();
                Debug.Assert(characterStyleIstd.HasValue);
                if (characterStyleIstd.HasValue)
                    runPr.Istd = characterStyleIstd.Value;
            }

            return runPr;
        }

        private NullableInt32 GetCharacterStyleIstd()
        {
            Inline inline = mFirstEntryNode as Inline;
            if (inline == null)
                return NullableInt32.Null;

            return inline.RunPr.ContainsKey(FontAttr.Istd)
                ? new NullableInt32((int)inline.RunPr[FontAttr.Istd])
                : NullableInt32.Null;
        }

        private NodeRange GetEntryRange()
        {
            EnsureEntryRange();
            return mEntryRange;
        }

        string ITocEntry.GetDocumentOutlineTitle()
        {
            OutlineLevel outlineLevel = (OutlineLevel)mParagraph.FetchParaAttr(ParaAttr.OutlineLevel, RevisionsView.Final);
            if (outlineLevel == OutlineLevel.BodyText)
                return null;

            StringBuilder builder = new StringBuilder();

            NodeRange range = GetEntryRange() ?? NodeRange.Void;

            NodeTextCollectorOptions options = new NodeTextCollectorOptions();
            options.AllowHiddenText = false;
            options.AllowDeletedText = false;
            options.IsFieldResultMode = true;

            string rangeText = NodeTextCollector.GetText(range, options)
                .Trim()
                .Replace('\v', ' ');

            if (HasLabelRange)
                builder.Append(mParagraph.ListLabel.LabelStringFinal);

            if (HasLabelRange && !string.IsNullOrEmpty(rangeText))
            {
                switch (mParagraph.ListFormat.ListLevelFinal.TrailingCharacter)
                {
                    case ListTrailingCharacter.Tab:
                        builder.Append(ControlChar.Tab);
                        break;
                    case ListTrailingCharacter.Space:
                    case ListTrailingCharacter.Nothing:
                        builder.Append(' ');
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            builder.Append(rangeText);

            return builder.ToString();
        }

        bool ITocEntry.IsInFieldCode
        {
            get { return false; }
        }

        bool ITocEntry.HasBookmark
        {
            get { return true; }
        }

        bool ITocEntry.IsLinkedStyleTocEntry
        {
            get { return mIsLinkedStyleTocEntry; }
        }

        int ITocEntry.GetSequenceValue(string sequenceIdentifier)
        {
            throw new InvalidOperationException();
        }

        int ITocEntry.GetPageNumber()
        {
            throw new InvalidOperationException();
        }

        private void EnsureEntryRange()
        {
            if (mIsEntryRangeInitialized)
                return;

            mIsEntryRangeInitialized = true;

            // Remove leading spaces and page breaks.
            Node startNode = TrimEntryLeft();

            // Remove trailing spaces and page breaks.
            Node endNode = TrimEntryRight();

            if (startNode == null || endNode == null)
            {
                if (!HasLabelRange)
                    return;

                if (mParagraph.ParagraphBreakFont.Hidden)
                    return;

                if ((mParagraph.NextNonAnnotationSibling == null) && mParagraph.ParentSection.IsLastChild && !mParagraph.HasInlineNodes())
                    return;

                mEntryRange = new NodeRange(mParagraph, mParagraph);
                return;
            }

            if (!HasLabelRange && IsEmptyEntryRange(startNode, endNode))
                return;

            mEntryRange = new NodeRange(startNode, endNode);
        }

        /// <summary>
        /// Trims leading page/column breaks white space from the TOC entry.
        /// </summary>
        /// <remarks>
        /// Skips any nodes other than runs.
        /// If bookmark a page break, then page numbering will be incorrect in TOC.
        /// </remarks>
        /// <returns>
        /// The first valid run to bookmark, or null.
        /// </returns>
        private Node TrimEntryLeft()
        {
            return TrimEntry(StepForward, mFirstEntryNode, mLastEntryNode, false);
        }

        /// <summary>
        /// Trims trailing page/column breaks and white space from the TOC entry.
        /// </summary>
        /// <remarks>
        /// Skips any nodes other than runs.
        /// </remarks>
        /// <returns>
        /// The last valid run to bookmark, or null.
        /// </returns>
        private Node TrimEntryRight()
        {
            // The order of the nodes passed to the method is correct: last, then first.
            return TrimEntry(StepBackward, mLastEntryNode, mFirstEntryNode, false);
        }

        /// <summary>
        /// Goes from firstEntryNode to lastEntryNode and skips leading/trailing spaces and page/column breaks.
        /// </summary>
        /// <returns>
        /// The first run, starting (or ending) with a valid character.
        /// The run is split if needed.
        /// Null is returned if a valid run is not found.
        /// </returns>
        private static Node TrimEntry(int step, Node firstEntryNode, Node lastEntryNode, bool skipSpecialChars)
        {
            bool isLastEntryNodeProcessed = false;
            Node currentNode = firstEntryNode;
            int fieldCharCount = 0;

            while ((currentNode != null) && !isLastEntryNodeProcessed)
            {
                // WORDSNET-6150 Field start and end are valid nodes for TOC entry's start or end.
                switch (currentNode.NodeType)
                {
                    case NodeType.FieldStart:
                    {
                        if ((step == StepForward) && NeedStopTrimEntry((FieldStart)currentNode, fieldCharCount))
                            return currentNode;

                        fieldCharCount++;
                        break;
                    }
                    case NodeType.FieldSeparator:
                    {
                        fieldCharCount--;
                        break;
                    }
                    case NodeType.FieldEnd:
                    {
                        FieldEnd fieldEnd = (FieldEnd)currentNode;
                        if (!fieldEnd.HasSeparator)
                        {
                            if ((step == StepBackward) && NeedStopTrimEntry(fieldEnd, fieldCharCount))
                                return currentNode;

                            fieldCharCount--;
                        }

                        break;
                    }
                    case NodeType.Run:
                    {
                        // Skip field code.
                        // Use not-equal check instead of more-than check as we can move backward.
                        if (fieldCharCount != 0)
                            break;

                        Run run = (Run)currentNode;

                        // WORDSNET-20677 Deleted and hidden runs are skipped.
                        if (!run.IsHiddenOrDeleted)
                        {
                            int validCharPosition = FindValidChar(run.Text, step);
                            if (validCharPosition != ValidCharNotFound)
                            {
                                // Split the run to insert a bookmark, if needed.
                                SplitRun(run, step, validCharPosition);

                                // Split or not, a valid run is found.
                                return run;
                            }
                        }

                        break;
                    }
                    case NodeType.SpecialChar:
                    {
                        if (skipSpecialChars)
                            break;

                        if (fieldCharCount != 0)
                            break;

                        SpecialChar specialChar = (SpecialChar)currentNode;

                        if (!specialChar.IsHiddenOrDeleted && IsValidChar(specialChar.Char))
                            return specialChar;

                        break;
                    }
                    // WORDSNET-7297 TOC entry can include Shape, GroupShape or DrawingML nodes at the beginning or end.
                    case NodeType.Shape:
                    {
                        if (((Shape)currentNode).IsInline)
                            return currentNode;
                        break;
                    }
                    case NodeType.GroupShape:
                    {
                        if (((GroupShape)currentNode).IsInline)
                            return currentNode;
                        break;
                    }
                    case NodeType.OfficeMath:
                        return currentNode;
                    default:
                        break;
                }

                // Check if we are looking at the last TOC entry node.
                isLastEntryNodeProcessed = (currentNode == lastEntryNode);

                currentNode = (step == StepForward)
                    ? currentNode.NextNonMarkupNodeLimited
                    : currentNode.PreviousNonMarkupNodeLimited;
            }

            // If everything is skipped, no TOC entry is created.
            return null;
        }

        private static bool NeedStopTrimEntry(FieldChar fieldChar, int fieldCharCount)
        {
            return (fieldCharCount == 0) &&
                FieldNumUtil.IsFieldNum(fieldChar.FieldType) &&
                !fieldChar.IsHiddenOrDeleted;
        }

        /// <summary>
        /// Splits the run to make it start or end on a valid character.
        /// </summary>
        /// <param name="run"></param>
        /// <param name="step"></param>
        /// <param name="validCharPosition"></param>
        private static void SplitRun(Run run, int step, int validCharPosition)
        {
            // Split the run to insert a bookmark...
            if (step == StepForward)
            {
                // ...after leading spaces.
                run.SplitBefore(validCharPosition);
            }
            else
            {
                // .. or before trailing spaces.
                run.SplitAfter(validCharPosition + 1);
            }
        }

        /// <summary>
        /// Looks for the first character that can start (or finish) a TOC entry.
        /// </summary>
        /// <remarks>Spaces and page breaks are skipped.</remarks>
        /// <param name="text"></param>
        /// <param name="step">1 to look forward from the beginning, -1 to look backward from the end. Use StepForward and StepBackward.</param>
        /// <returns>The position of the first valid char or ValidCharNotFound.</returns>
        private static int FindValidChar(string text, int step)
        {
            int currentPosition;
            int endPosition;

            switch (step)
            {
                case StepForward:
                    currentPosition = 0;
                    endPosition = text.Length;
                    break;
                case StepBackward:
                    // start from the end
                    currentPosition = text.Length - 1;
                    endPosition = -1;
                    break;
                default:
                    throw new ArgumentException("step");
            }

            bool isValidCharFound = false;
            while (currentPosition != endPosition)
            {
                isValidCharFound = IsValidChar(text[currentPosition]);

                if (isValidCharFound)
                {
                    // The found character position is in currentPosition.
                    break;
                }
                currentPosition += step;
            }

            return isValidCharFound ? currentPosition : ValidCharNotFound;
        }

        /// <summary>
        /// Checks if the given character should be present in the end or beginning of the TOC entry.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool IsValidChar(char c)
        {
            switch (c)
            {
                case ControlChar.PageBreakChar:
                case ControlChar.ColumnBreakChar:
                    return false;
                case ControlChar.DefaultTextInputChar:
                    return true;
                default:
                    return !char.IsWhiteSpace(c);
            }
        }

        private static bool IsEmptyEntryRange(Node start, Node end)
        {
            if (TrimEntry(StepForward, start, end, true) == null)
                return true;

            if (TrimEntry(StepBackward, end, start, true) == null)
                return true;

            return false;
        }

        private Document Document
        {
            get { return mParagraph.FetchDocument(); }
        }

        private NodeRange mEntryRange;
        private bool mIsEntryRangeInitialized;

        private readonly Paragraph mParagraph;
        private readonly int mLevel;
        private readonly Node mLastEntryNode;
        private readonly Node mFirstEntryNode;
        private readonly bool mIsLinkedStyleTocEntry;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int ValidCharNotFound = -1;
        private const int StepForward = 1;
        private const int StepBackward = -1;

        private static readonly int[] gIgnoreAttributeKeys = { FontAttr.RsidRPr, FontAttr.RsidR };
    }
}
