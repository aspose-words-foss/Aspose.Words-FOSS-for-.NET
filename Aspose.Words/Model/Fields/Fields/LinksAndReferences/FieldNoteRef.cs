// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2005 by Alexey Morozov

using System.Collections.Generic;
using Aspose.Words.Notes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the NOTEREF field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Inserts the mark of the footnote or endnote that is marked by the specified bookmark.
    /// </remarks>
    public class FieldNoteRef : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            if (!StringUtil.HasChars(BookmarkName))
                return new FieldUpdateActionInsertErrorMessage(this, NoBookmarkNameGivenErrorMessage);

            Bookmark bookmark = FieldUtil.GetCachedBookmark(this, BookmarkName);
            if (bookmark == null)
                return new FieldUpdateActionInsertErrorMessage(this, BookmarkNotDefinedErrorMessage);

            IList<Node> footnotes = NodeFinder.FindNodes(bookmark.GetNodeRange(), NodeType.Footnote);
            if (footnotes.Count == 0)
                return new FieldUpdateActionInsertErrorMessage(this, BookmarkNotDefinedErrorMessage);

            Footnote footnote = (Footnote)footnotes[0];

            NodeRange nodeRange = GetResultRange(bookmark, footnote);
            if (nodeRange == null)
                return new FieldUpdateActionInsertErrorMessage(this, BookmarkNotDefinedErrorMessage);

            return new FieldUpdateActionApplyResult(this, new NodeRangeFieldResult(nodeRange));
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
        /// Inserts the reference mark with the same character formatting as the Footnote Reference
        /// or Endnote Reference style.
        /// </summary>
        public bool InsertReferenceMark
        {
            get { return FieldCodeCache.HasSwitch(InsertReferenceMarkSwitch); }
            set { FieldCodeCache.SetSwitch(InsertReferenceMarkSwitch, value); }
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
                case InsertReferenceMarkSwitch:
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

        private NodeRange GetResultRange(Bookmark bookmark, Footnote footnote)
        {
            Paragraph para = new Paragraph(Document);

            Run start = GetReferenceMarkRun(footnote);
            if (start == null)
                return null;

            para.AppendChild(start);

            Run end = GetRelativePositionRun(bookmark);
            if (end != null)
                para.AppendChild(end);
            else
                end = start;

            return new NodeRange(start, end);
        }

        private Run GetReferenceMarkRun(Footnote footnote)
        {
            // FOSS
            return null;
        }

        private Run GetRelativePositionRun(Bookmark bookmark)
        {
            if (InsertRelativePosition && !IsInHeaderFooter)
            {
                string position = " " + FieldRefUtil.GetRelativeBookmarkPosition(this, bookmark);
                return new Run(Document, position);
            }

            return null;
        }

        private const int BookmarkNameArgumentIndex = 0;

        private const string InsertReferenceMarkSwitch = "\\f";
        private const string InsertHyperlinkSwitch = "\\h";
        private const string InsertRelativePositionSwitch = "\\p";

        private const string NoBookmarkNameGivenErrorMessage = "Error! No bookmark name given.";
        private const string BookmarkNotDefinedErrorMessage = "Error! Bookmark not defined.";
    }
}
