// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2017 by Edward Voronov

using System;

namespace Aspose.Words.Fields
{
    internal class TocEntryExtractorOptions : ITocEntryExtractorOptions
    {
        public bool IsBookmarkRangeSpecified
        {
            get { return false; }
        }

        public bool IncludeTocEntryFields
        {
            get { return false; }
        }

        public bool IsEntryLevelRangeSpecified
        {
            get { return false; }
        }

        public bool IsTableOfFigures
        {
            get { return false; }
        }

        public bool IsHeadingLevelRangeSpecified
        {
            get { return false; }
        }

        public bool AreCustomStylesSpecified
        {
            get { return false; }
        }

        public bool UseParagraphOutlineLevel
        {
            get { return true; }
        }

        public string TableOfFiguresLabel
        {
            get { throw new InvalidOperationException(); }
        }

        public LevelRange TocEntryLevelRange
        {
            get { throw new InvalidOperationException(); }
        }

        public LevelRange HeadingLevelRange
        {
            get { return LevelRange.MaxRange; }
        }

        public string CaptionlessTableOfFiguresLabel
        {
            get { throw new InvalidOperationException(); }
        }

        public Bookmark GetRangeBookmark()
        {
            throw new InvalidOperationException();
        }

        public int EntryType
        {
            get { throw new InvalidOperationException(); }
        }

        public int GetLevelForCustomStyle(Paragraph paragraph, Style style)
        {
            throw new InvalidOperationException();
        }

        public FieldEnd End
        {
            get { return null; }
        }

        public FieldStart Start
        {
            get { return null; }
        }

        public bool SkipTables { get; set; }

        public bool IncludeRefDocFields
        {
            get { return false; }
        }
    }
}
