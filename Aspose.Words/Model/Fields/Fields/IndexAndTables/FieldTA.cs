// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the TA field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Defines the text and page number for a table of authorities entry, which is used by a TOA field.
    /// </remarks>
    public class FieldTA : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Gets or sets whether to apply bold formatting to the page number for the entry.
        /// </summary>
        public bool IsBold
        {
            get { return FieldCodeCache.HasSwitch(IsBoldSwitch); }
            set { FieldCodeCache.SetSwitch(IsBoldSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the integral entry category, which is a number that corresponds to the order of
        /// categories.
        /// </summary>
        public string EntryCategory //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryCategorySwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(EntryCategorySwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to apply italic formatting to the page number for the entry.
        /// </summary>
        public bool IsItalic
        {
            get { return FieldCodeCache.HasSwitch(IsItalicSwitch); }
            set { FieldCodeCache.SetSwitch(IsItalicSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the long citation for the entry.
        /// </summary>
        public string LongCitation
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(LongCitationSwitch); }
            set { FieldCodeCache.SetSwitch(LongCitationSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark that marks a range of pages that is inserted as the entry's page number.
        /// </summary>
        public string PageRangeBookmarkName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageRangeBookmarkNameSwitch); }
            set { FieldCodeCache.SetSwitch(PageRangeBookmarkNameSwitch, value); }
        }

        internal bool HasPageRangeBookmarkNameSwitch
        {
            get { return FieldCodeCache.HasSwitch(PageRangeBookmarkNameSwitch); }
        }

        /// <summary>
        /// Gets or sets the short citation for the entry.
        /// </summary>
        public string ShortCitation
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(ShortCitationSwitch); }
            set { FieldCodeCache.SetSwitch(ShortCitationSwitch, value); }
        }

        internal NodeRange LongCitationRange
        {
            get { return FieldCodeCache.GetSwitchArgumentRange(LongCitationSwitch); }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case IsBoldSwitch:
                case IsItalicSwitch:
                {
                    return FieldSwitchType.Flag;
                }
                case EntryCategorySwitch:
                case LongCitationSwitch:
                case PageRangeBookmarkNameSwitch:
                case ShortCitationSwitch:
                {
                    return FieldSwitchType.HasArgument;
                }
                default:
                {
                    return FieldSwitchType.Unknown;
                }
            }
        }

        private const string IsBoldSwitch = "\\b";
        private const string EntryCategorySwitch = "\\c";
        private const string IsItalicSwitch = "\\i";
        internal const string LongCitationSwitch = "\\l";
        private const string PageRangeBookmarkNameSwitch = "\\r";
        private const string ShortCitationSwitch = "\\s";
    }
}
