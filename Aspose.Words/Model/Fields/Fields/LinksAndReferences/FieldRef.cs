// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/06/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the REF field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the text or graphics represented by the specified bookmark.
    /// </remarks>
    public class FieldRef : Field, IFieldCodeTokenInfoProvider, IMergeFieldSurrogate
    {
        internal override FieldUpdateStage GetUpdateStage()
        {
            bool isParagraphNumberInAnyContext = InsertParagraphNumberInAnyContext;
            if (!isParagraphNumberInAnyContext && InsertRelativePosition)
                return FieldUpdateStage.MainLoop;

            return FieldRefUtil.GetFieldUpdateStage(this, BookmarkName, isParagraphNumberInAnyContext);
        }

        internal override FieldUpdateAction UpdateCore()
        {
            // WORDSNET-6330 Exception is thrown on bookmark seek if name of bookmark is not specified.
            if (!StringUtil.HasChars(BookmarkName))
                return new FieldUpdateActionInsertErrorMessage(this, NoBookmarkNameGivenErrorMessage);

            // SPEED Get a bookmark from a cache.
            Bookmark bookmark = FieldUtil.GetCachedBookmark(this, BookmarkName);
            if (bookmark == null)
                return new FieldUpdateActionInsertErrorMessage(this, ReferenceSourceNotFoundErrorMessage);

            // WORDSNET-22358 Get bookmark node range before old field result is removed because bookmark may be inside.
            NodeRange bookmarkRange = bookmark.GetNodeRange();
            if (bookmarkRange.IsVoid)
                return new FieldUpdateActionInsertErrorMessage(this, ReferenceSourceNotFoundErrorMessage);

            bookmarkRange = AdjustBookmarkRangeIfNeeded(bookmarkRange);

            bool hasBookmarkInResult;
            Paragraph bookmarkParagraph = FieldRefUtil.BeginFieldUpdate(this, bookmark, InsertParagraphNumberInAnyContext, out hasBookmarkInResult);

            string result = GetParagraphNumberOrRelativePosition(bookmark, bookmarkParagraph);
            if (result != null)
                return new FieldUpdateActionApplyResult(this, result);

            Debug.Assert(bookmarkRange != null);

            return FieldRefUtil.EndFieldUpdate(
                this,
                bookmark,
                bookmarkRange,
                bookmarkParagraph,
                hasBookmarkInResult,
                IncludeNoteOrComment);
        }

        private string GetParagraphNumberOrRelativePosition(Bookmark bookmark, Paragraph bookmarkParagraph)
        {
            if (InsertParagraphNumber)
            {
                // It seems like the trailing dot is the only thing that does not go to the REF field. That is, "11."
                // becomes "11", but "11.2" stays "11.2" and "11*" stays "11*". Maybe need to investigate more.
                return FieldRefUtil.GetParagraphNumber(
                    this,
                    bookmark,
                    bookmarkParagraph,
                    SuppressNonDelimiters,
                    InsertRelativePosition);
            }

            if (InsertParagraphNumberInRelativeContext)
            {
                // It seems like the trailing dot is the only thing that does not go to the REF field. That is, "11."
                // becomes "11", but "11.2" stays "11.2" and "11*" stays "11*". Maybe need to investigate more.
                return FieldRefUtil.GetParagraphNumberInRelativeContext(
                    this,
                    bookmark,
                    bookmarkParagraph,
                    Start.ParentParagraph,
                    SuppressNonDelimiters,
                    NumberSeparator,
                    InsertRelativePosition);
            }

            if (InsertParagraphNumberInFullContext)
            {
                // It seems like the trailing dot is the only thing that does not go to the REF field. That is, "11."
                // becomes "11", but "11.2" stays "11.2" and "11*" stays "11*". Maybe need to investigate more.
                return FieldRefUtil.GetParagraphNumberInFullContext(
                    this,
                    bookmark,
                    bookmarkParagraph,
                    SuppressNonDelimiters,
                    NumberSeparator,
                    InsertRelativePosition);
            }

            if (InsertRelativePosition)
            {
                return FieldRefUtil.GetRelativeBookmarkPosition(this, bookmark);
            }

            return null;
        }

        private NodeRange AdjustBookmarkRangeIfNeeded(NodeRange bookmarkRange)
        {
            if (InsertParagraphNumberOrRelativePosition)
                return bookmarkRange;

            // MS Word copies whole field result even if bookmark range does not include whole field.
            return IncludeIncompleteFields(bookmarkRange);
        }

        private static NodeRange IncludeIncompleteFields(NodeRange range)
        {
            int depth = 0;
            FieldStart fieldStart = null;
            FieldEnd fieldEnd = null;

            foreach (Node node in range)
            {
                switch (node.NodeType)
                {
                    case NodeType.FieldStart:
                        if (depth == 0)
                            fieldStart = (FieldStart)node;
                        depth++;
                        break;
                    case NodeType.FieldEnd:
                        if (depth != 0)
                            depth--;
                        else
                            fieldEnd = (FieldEnd)node;

                        if (depth == 0)
                            fieldStart = null;
                        break;
                    default:
                        break;
                }
            }

            if ((fieldStart == null) && (fieldEnd == null))
                return range;

            Node rangeStart = fieldEnd != null
                ? fieldEnd.GetField().Start
                : null;

            Node rangeEnd = fieldStart != null
                ? fieldStart.GetField().End
                : null;

            return new NodeRange(
                rangeStart ?? range.Start.Node,
                rangeStart != null || range.IsStartIncluded,
                rangeEnd ?? range.End.Node,
                rangeEnd != null || range.IsEndIncluded);
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IncludeNoteOrCommentSwitch:
                case InsertHyperlinkSwitch:
                case InsertParagraphNumberSwitch:
                case InsertRelativePositionSwitch:
                case InsertParagraphNumberInRelativeContextSwitch:
                case SuppressNonDelimitersSwitch:
                case InsertParagraphNumberInFullContextSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                case NumberSeparatorSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        string IMergeFieldSurrogate.GetMergeFieldName()
        {
            return BookmarkName;
        }

        bool IMergeFieldSurrogate.CanWorkAsMergeField()
        {
            return true;
        }

        bool IMergeFieldSurrogate.IsMergeValueRequired()
        {
            return true;
        }

        /// <summary>
        /// Gets or sets the referenced bookmark's name.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetArgumentAsString(BookmarkNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(BookmarkNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate sequence numbers and page numbers.
        /// </summary>
        public string NumberSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(NumberSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(NumberSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to increment footnote, endnote, and annotation numbers that are
        /// marked by the bookmark, and insert the corresponding footnote, endnote, and comment text.
        /// </summary>
        public bool IncludeNoteOrComment
        {
            get { return FieldCodeCache.HasSwitch(IncludeNoteOrCommentSwitch); }
            set { FieldCodeCache.SetSwitch(IncludeNoteOrCommentSwitch, value); }
        }

        private bool InsertParagraphNumberInAnyContext
        {
            get { return (InsertParagraphNumber || InsertParagraphNumberInRelativeContext || InsertParagraphNumberInFullContext); }
        }

        private bool InsertParagraphNumberOrRelativePosition
        {
            get { return InsertParagraphNumberInAnyContext || InsertRelativePosition; }
        }

        /// <summary>
        /// Gets or sets whether to create a hyperlink to the bookmarked paragraph.
        /// </summary>
        public bool InsertHyperlink
        {
            get { return FieldCodeCache.HasSwitch(InsertHyperlinkSwitch); }
            set { FieldCodeCache.SetSwitch(InsertHyperlinkSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph exactly as it appears in the document.
        /// </summary>
        public bool InsertParagraphNumber
        {
            get { return FieldCodeCache.HasSwitch(InsertParagraphNumberSwitch); }
            set { FieldCodeCache.SetSwitch(InsertParagraphNumberSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the relative position of the referenced paragraph.
        /// </summary>
        public bool InsertRelativePosition
        {
            get { return FieldCodeCache.HasSwitch(InsertRelativePositionSwitch); }
            set { FieldCodeCache.SetSwitch(InsertRelativePositionSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph in relative context.
        /// </summary>
        public bool InsertParagraphNumberInRelativeContext
        {
            get { return FieldCodeCache.HasSwitch(InsertParagraphNumberInRelativeContextSwitch); }
            set { FieldCodeCache.SetSwitch(InsertParagraphNumberInRelativeContextSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to suppress non-delimiter characters.
        /// </summary>
        public bool SuppressNonDelimiters
        {
            get { return FieldCodeCache.HasSwitch(SuppressNonDelimitersSwitch); }
            set { FieldCodeCache.SetSwitch(SuppressNonDelimitersSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert the paragraph number of the referenced paragraph in full context.
        /// </summary>
        public bool InsertParagraphNumberInFullContext
        {
            get { return FieldCodeCache.HasSwitch(InsertParagraphNumberInFullContextSwitch); }
            set { FieldCodeCache.SetSwitch(InsertParagraphNumberInFullContextSwitch, value); }
        }

        private const int BookmarkNameArgumentIndex = 0;

        private const string NumberSeparatorSwitch = "\\d";
        private const string IncludeNoteOrCommentSwitch = "\\f";
        internal const string InsertHyperlinkSwitch = "\\h";
        private const string InsertParagraphNumberSwitch = "\\n";
        private const string InsertRelativePositionSwitch = "\\p";
        private const string InsertParagraphNumberInRelativeContextSwitch = "\\r";
        private const string SuppressNonDelimitersSwitch = "\\t";
        private const string InsertParagraphNumberInFullContextSwitch = "\\w";

        private const string NoBookmarkNameGivenErrorMessage = "Error! No bookmark name given.";
        private const string ReferenceSourceNotFoundErrorMessage = "Error! Reference source not found.";
    }
}
