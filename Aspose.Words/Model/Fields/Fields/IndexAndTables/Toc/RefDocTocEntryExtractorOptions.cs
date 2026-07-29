// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2021 by Edward Voronov

namespace Aspose.Words.Fields
{
    internal class RefDocTocEntryExtractorOptions : ITocEntryExtractorOptions
    {
        internal RefDocTocEntryExtractorOptions(ITocEntryExtractorOptions options)
        {
            mOptions = options;
        }

        bool ITocEntryExtractorOptions.IsBookmarkRangeSpecified
        {
            get { return false; }
        }

        bool ITocEntryExtractorOptions.IncludeTocEntryFields
        {
            get { return mOptions.IncludeTocEntryFields; }
        }

        bool ITocEntryExtractorOptions.IsEntryLevelRangeSpecified
        {
            get { return mOptions.IsEntryLevelRangeSpecified; }
        }

        bool ITocEntryExtractorOptions.IsTableOfFigures
        {
            get { return mOptions.IsTableOfFigures; }
        }

        bool ITocEntryExtractorOptions.IsHeadingLevelRangeSpecified
        {
            get { return mOptions.IsHeadingLevelRangeSpecified; }
        }

        bool ITocEntryExtractorOptions.AreCustomStylesSpecified
        {
            get { return mOptions.AreCustomStylesSpecified; }
        }

        bool ITocEntryExtractorOptions.UseParagraphOutlineLevel
        {
            get { return mOptions.UseParagraphOutlineLevel; }
        }

        string ITocEntryExtractorOptions.TableOfFiguresLabel
        {
            get { return mOptions.TableOfFiguresLabel; }
        }

        LevelRange ITocEntryExtractorOptions.TocEntryLevelRange
        {
            get { return mOptions.TocEntryLevelRange; }
        }

        LevelRange ITocEntryExtractorOptions.HeadingLevelRange
        {
            get { return mOptions.HeadingLevelRange; }
        }

        string ITocEntryExtractorOptions.CaptionlessTableOfFiguresLabel
        {
            get { return mOptions.CaptionlessTableOfFiguresLabel; }
        }

        Bookmark ITocEntryExtractorOptions.GetRangeBookmark()
        {
            return null;
        }

        int ITocEntryExtractorOptions.EntryType
        {
            get { return mOptions.EntryType; }
        }

        int ITocEntryExtractorOptions.GetLevelForCustomStyle(Paragraph paragraph, Style style)
        {
            return mOptions.GetLevelForCustomStyle(paragraph, style);
        }

        FieldEnd ITocEntryExtractorOptions.End
        {
            get { return null; }
        }

        FieldStart ITocEntryExtractorOptions.Start
        {
            get { return null; }
        }

        bool ITocEntryExtractorOptions.SkipTables
        {
            get { return mOptions.SkipTables; }
        }

        bool ITocEntryExtractorOptions.IncludeRefDocFields
        {
            get { return false; }
        }

        private readonly ITocEntryExtractorOptions mOptions;
    }
}
