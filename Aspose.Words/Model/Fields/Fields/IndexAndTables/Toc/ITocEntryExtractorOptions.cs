// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2014 by Edward Voronov

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Interface used to provide a <see cref="TocEntryExtractor"/> with options, needed to extract TOC entries.
    /// </summary>
    internal interface ITocEntryExtractorOptions
    {
        [JavaThrows(true)]
        bool IsBookmarkRangeSpecified { get; }

        [JavaThrows(true)]
        bool IncludeTocEntryFields { get; }

        [JavaThrows(true)]
        bool IsEntryLevelRangeSpecified { get; }

        [JavaThrows(true)]
        bool IsTableOfFigures { get; }

        [JavaThrows(true)]
        bool IsHeadingLevelRangeSpecified { get; }

        [JavaThrows(true)]
        bool AreCustomStylesSpecified { get; }

        [JavaThrows(true)]
        bool UseParagraphOutlineLevel { get; }

        [JavaThrows(true)]
        string TableOfFiguresLabel { get; }

        LevelRange TocEntryLevelRange { get; }

        LevelRange HeadingLevelRange { get; }

        [JavaThrows(true)]
        string CaptionlessTableOfFiguresLabel { get; }

        [JavaThrows(true)]
        Bookmark GetRangeBookmark();

        int EntryType { get; }

        [JavaThrows(true)]
        int GetLevelForCustomStyle(Paragraph paragraph, Style style);

        FieldEnd End { get; }

        FieldStart Start { get; }

        bool SkipTables { get; }

        bool IncludeRefDocFields { get; }
    }
}
