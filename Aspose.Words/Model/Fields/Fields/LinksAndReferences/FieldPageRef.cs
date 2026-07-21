// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2005 by Roman Korchagin

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the PAGEREF field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the number of the page containing the specified bookmark for a cross-reference.
    /// </remarks>
    public class FieldPageRef : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            if (!BookmarkNameHasChars)
                return new FieldUpdateActionInsertErrorMessage(this, "Error! No bookmark name given.");

            Constant value = Updater.DataProviders.GetValue(this);
            if (value == null)
            {
                return BookmarkNode == null
                    ? new FieldUpdateActionInsertErrorMessage(this, "Error! Bookmark not defined.")
                    : null;
            }

            if (InsertRelativePosition)
            {
                // Specifying some number format makes the field ignore relative position.
                bool isFormatSpecified = (Format.GeneralFormats.GetNumericFormat() != GeneralFormat.None) || FieldCodeCache.HasSwitch("\\#");
                if (!isFormatSpecified)
                    return new FieldUpdateActionApplyResult(this, GetRelativePosition((Int32Constant)value));
            }

            return new FieldUpdateActionApplyResult(this, value);
        }

        private string GetRelativePosition(Int32Constant bookmarkPage)
        {
            int bookmarkPageValue = (int)bookmarkPage.ValueDouble;

            // FOSS: the relative position ("above"/"below") requires the layout engine to know on which page
            // this field is rendered relative to the bookmark. Without layout, fall back to the absolute
            // "on page N" wording, consistent with the unavailable-relative-position path above.
            return GetOnPageNString(bookmarkPageValue);
        }

        private static string GetOnPageNString(int pageNumber)
        {
            return string.Format("on page {0}", pageNumber);
        }

        internal override Section GetPageNumberFormatSection()
        {
            return (Section)((BookmarkNode != null) ? BookmarkNode.GetAncestor(NodeType.Section) : null);
        }

        internal Node GetChapterTitleNode()
        {
            Bookmark bookmark = FieldUtil.GetCachedBookmark(this, BookmarkName);

            BookmarkStart bookmarkStart = bookmark.BookmarkStart;
            BookmarkEnd bookmarkEnd = bookmark.BookmarkEnd;

            if (bookmarkStart.ParentNode == bookmarkEnd.ParentNode)
                return bookmarkStart;

            foreach (Node node in bookmark.GetNodeRange())
            {
                if (node is Inline)
                    return node;
            }

            return bookmarkStart;
        }

        private bool BookmarkNameHasChars
        {
            get { return StringUtil.HasChars(BookmarkName); }
        }

        internal Node BookmarkNode
        {
            get
            {
                if ((mBookmarkNode == null) || (mBookmarkNode.ParentNode == null))
                {
                    if (!BookmarkNameHasChars)
                        return null;

                    // SPEED Get a bookmark from a cache.
                    Bookmark bookmark = FieldUtil.GetCachedBookmark(this, BookmarkName);
                    if (bookmark == null)
                        return null;

                    mBookmarkNode = bookmark.BookmarkStart;
                }

                return mBookmarkNode;
            }
            set { mBookmarkNode = value; }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetArgumentAsString(BookmarkNameArgumentIndex); }
            set { FieldCodeCache.SetArgument(BookmarkNameArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert a hyperlink to the bookmarked paragraph.
        /// </summary>
        public bool InsertHyperlink
        {
            get { return FieldCodeCache.HasSwitch(InsertHyperlinkSwitch); }
            set { FieldCodeCache.SetSwitch(InsertHyperlinkSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to insert a relative position of the bookmarked paragraph.
        /// </summary>
        public bool InsertRelativePosition
        {
            get { return FieldCodeCache.HasSwitch(InsertRelativePositionSwitch); }
            set { FieldCodeCache.SetSwitch(InsertRelativePositionSwitch, value); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case InsertHyperlinkSwitch:
                case InsertRelativePositionSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        private Node mBookmarkNode;

        private const int BookmarkNameArgumentIndex = 0;

        internal const string InsertHyperlinkSwitch = "\\h";
        private const string InsertRelativePositionSwitch = "\\p";
    }
}
