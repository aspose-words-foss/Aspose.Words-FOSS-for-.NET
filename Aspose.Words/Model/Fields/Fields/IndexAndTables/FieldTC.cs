// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2010 by Dmitry Vorobyev

using System;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the TC field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Defines the text and page number for a table of contents (including a table of figures) entry, which
    /// is used by a TOC field.
    /// </remarks>
    [CodePorting.Translator.Cs2Cpp.CppForceForwardDeclaration("Aspose.Common.NullableInt32")]
    public sealed class FieldTC : Field, IFieldCodeTokenInfoProvider, ITocEntry
    {
        internal override void Initialize(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            base.Initialize(start, separator, end);
            Parse();
        }

        private void Parse()
        {
            FieldArgument text = FieldCodeCache.GetArgument(TextArgumentIndex);
            // Set a flag indicating if the entry shows in TOC
            mShowInToc = (text != null) && (text.GetNormalizedText().Length > 0);

            // WORDSNET-22419 NullReferenceException occurs during UpdateFields.
            // Even if the entry does not show it TOC, the correct argument range is needed for document conversion.
            TextRange = FieldCodeCache.GetArgumentRange(TextArgumentIndex);

            // The level should be an integer number from 1 to 9.
            int level = LevelRange.MinLevel;
            if (HasEntryLevelSwitch)
            {
                // TODO MS Word updates a TOC field with following error if a TC field level range switch is zero or negative or not a number:
                // Error! Not a valid heading level in TOC entry on page 1.

                level = EntryLevelAsInt32.GetValueOrDefault(level);
                level = LevelRange.IsLevelValid(level)
                    ? level
                    : LevelRange.MinLevel;
            }

            mLevel = level;
        }

        /// <summary>
        /// Calls Node.InsertAfter() or Node.InsertBefore depending on insertAfter parameter.
        /// Node.Insert() could be used instead, but it's private.
        /// </summary>
        /// <param name="newChild"></param>
        /// <param name="refChild"></param>
        /// <param name="insertAfter"></param>
        private static void InsertNode(Node newChild, Node refChild, bool insertAfter)
        {
            if (insertAfter)
                refChild.InsertNext(newChild);
            else
                refChild.InsertPrevious(newChild);
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case OmitPageNumberSwitch:
                    return FieldSwitchType.Flag;
                case TypeIdentifierSwitch:
                case EntryLevelSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Gets the node range for this field's text.
        /// </summary>
        internal NodeRange TextRange { get; private set; }

        /// <summary>
        /// Gets or sets the text of the entry.
        /// </summary>
        public string Text
        {
            get { return FieldCodeCache.GetArgumentAsString(TextArgumentIndex); }
            set { FieldCodeCache.SetArgument(TextArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets a type identifier for this field (which is typically a letter).
        /// </summary>
        internal NodeRange Range
        {
            get { return TextRange; }
        }

        /// <summary>
        /// Gets or sets a type identifier for this field (which is typically a letter).
        /// </summary>
        public string TypeIdentifier
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TypeIdentifierSwitch); }
            set { FieldCodeCache.SetSwitch(TypeIdentifierSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the level of the entry.
        /// </summary>
        public string EntryLevel //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryLevelSwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(EntryLevelSwitch, value); }
        }

        internal bool HasEntryLevelSwitch
        {
            get { return FieldCodeCache.HasSwitch(EntryLevelSwitch); }
        }

        internal NullableInt32 EntryLevelAsInt32
        {
            get { return FieldCodeCache.GetSwitchArgumentAsInt32(EntryLevelSwitch); }
        }

        /// <summary>
        /// Gets or sets whether page number in TOC should be omitted for this field.
        /// </summary>
        public bool OmitPageNumber
        {
            get { return FieldCodeCache.HasSwitch(OmitPageNumberSwitch); }
            set { FieldCodeCache.SetSwitch(OmitPageNumberSwitch, value); }
        }

        Paragraph ITocEntry.Paragraph
        {
            get
            {
                NodeRange range = GetEntryRange();
                if (range == null)
                    return null;

                return (Paragraph)range.Start.Node.GetAncestor(NodeType.Paragraph);
            }
        }

        NodeRange ITocEntry.InsertBookmark(string bookmarkName)
        {
            NodeRange range = GetEntryRange();
            if (range == null)
                return null;

            BookmarkStart bookmarkStart = new BookmarkStart(FetchDocument(), bookmarkName);
            Node startNode = range.Start.Node;
            InsertNode(bookmarkStart, startNode, range.Start.IsEnd);

            BookmarkEnd bookmarkEnd = new BookmarkEnd(FetchDocument(), bookmarkName);
            Node endNode = range.End.Node;
            InsertNode(bookmarkEnd, endNode, !range.End.IsStart);

            return Bookmark.GetNodeRange(bookmarkStart, bookmarkEnd);
        }

        int ITocEntry.Level
        {
            get { return mLevel; }
        }

        string ITocEntry.GetDocumentOutlineTitle()
        {
            throw new InvalidOperationException();
        }

        NodeRange ITocEntry.GetLabelRange()
        {
            return null;
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
            get { return false; }
        }

        int ITocEntry.GetSequenceValue(string sequenceIdentifier)
        {
            throw new InvalidOperationException();
        }

        int ITocEntry.GetPageNumber()
        {
            throw new InvalidOperationException();
        }

        private NodeRange GetEntryRange()
        {
            if (!mShowInToc || TextRange.IsEmpty)
                return null;

            TextRange.Isolate();

            return TextRange;
        }

        private int mLevel;
        /// <summary>
        /// Tells if the entry should be shown in table of contents.
        /// Entries with empty text should not be shown even if containing range is not empty ("", for example).
        /// </summary>
        private bool mShowInToc;

        internal const int TextArgumentIndex = 0;

        internal const string TypeIdentifierSwitch = "\\f";
        private const string EntryLevelSwitch = "\\l";
        private const string OmitPageNumberSwitch = "\\n";
    }
}
