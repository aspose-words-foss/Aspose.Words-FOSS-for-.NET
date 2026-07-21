// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using System.Collections.Generic;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the TOA field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Builds a table of authorities (that is, a list of the references in a legal document, such as references
    /// to cases, statutes, and rules, along with the numbers of the pages on which the references appear) using the
    /// entries specified by TA fields.
    /// </remarks>
    public class FieldToa : Field, IFieldCodeTokenInfoProvider
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Bookmark bookmark = null;
            if (HasBookmarkNameSwitch)
            {
                string bookmarkName = BookmarkName;
                if (bookmarkName == null)
                    return new FieldUpdateActionInsertErrorMessage(this, NoBookmarkNameGivenError);

                bookmark = FetchBookmark(bookmarkName);
                if (bookmark == null)
                    return new FieldUpdateActionInsertErrorMessage(this, BookmarkNotDefinedError);
            }

            string entryCategory = EntryCategory;
            int categoryNumber = FormatterPal.TryParseInt(entryCategory);
            if (categoryNumber <= 0)
                return new FieldUpdateActionInsertErrorMessage(this, CategoryNumberNotFoundError);

            string sequenceName = SequenceName;
            FieldSeqDataProvider seqDataProvider = sequenceName != null
                ? Updater.DataProviders.Ensure(new FieldSeqDataProvider(Updater))
                : null;

            IList<ToaEntry> entries = ToaEntryExtractor.ExtractToaEntries(FetchDocument(), EntryCategory, bookmark, sequenceName, seqDataProvider);
            if (entries.Count == 0)
                return new FieldUpdateActionInsertErrorMessage(this, NoTableOfAuthoritiesEntriesFoundError);

            using (UpdateContext.RemoveOldResultSafe())
            {
                DocumentBuilder builder = FieldIndexAndTablesUtil.GetDocumentBuilder(this);
                FieldIndexAndTablesUtil.ConvertOuterInlineSdtToBlock(this);

                BuildHeading(categoryNumber, builder);
                BuildEntries(entries, builder);
            }

            return new FieldUpdateActionDoNothing(this);
        }

        private void BuildHeading(int category, DocumentBuilder builder)
        {
            if (!UseHeading)
                return;

            string heading = FetchDocument().FieldOptions.EffectiveToaCategories[category];

            FieldIndexAndTablesUtil.SetUpEntryParagraph(this, builder,  StyleIdentifier.ToaHeading);
            FieldIndexAndTablesUtil.EnsureEntryParagraphTabStop(builder, null);

            builder.Write(heading);
        }

        private void BuildEntries(IEnumerable<ToaEntry> entries, DocumentBuilder builder)
        {
            bool usePassim = UsePassim;

            string entrySeparator = NormalizeSeparator(EntrySeparator, ControlChar.Tab);
            string pageNumberListSeparator = NormalizeSeparator(PageNumberListSeparator, ", ");
            string sequenceSeparator = NormalizeSeparator(SequenceSeparator, "-");
            string pageRangeSeparator = NormalizeSeparator(PageRangeSeparator, "–");

            ToaEntryAttributeModifier toaEntryAttributeModifier = new ToaEntryAttributeModifier(Document.Styles, RemoveEntryFormatting);

            foreach (ToaEntry entry in entries)
            {
                BuildEntry(
                    builder,
                    entry,
                    toaEntryAttributeModifier,
                    usePassim,
                    entrySeparator,
                    pageNumberListSeparator,
                    sequenceSeparator,
                    pageRangeSeparator);
            }
        }

        private void BuildEntry(
            DocumentBuilder builder,
            ToaEntry entry,
            INodeModifier toaEntryAttributeModifier,
            bool usePassim,
            string entrySeparator,
            string pageNumberListSeparator,
            string sequenceSeparator,
            string pageRangeSeparator)
        {
            FieldIndexAndTablesUtil.SetUpEntryParagraph(this, builder, StyleIdentifier.TableOfAuthorities);
            FieldIndexAndTablesUtil.EnsureEntryParagraphTabStop(builder, null);

            CompositeModifier modifier = new CompositeModifier();

            DocumentBase entryDocument = entry.Range.Document;
            if (entryDocument != Document)
                modifier.AddModifier(new ExternalDocumentModifier(entryDocument, Document, ImportFormatMode.UseDestinationStyles));

            modifier.AddModifier(new FieldTokenDecoderNodeModifier(entry.Range, FieldTokenDecoderOptions.All));
            modifier.AddModifier(toaEntryAttributeModifier);

            Node dummyRefNode = FieldIndexAndTablesUtil.CreateAndInsertDummyRefNode(builder);
            NodeCopier.CopyWithoutFields(entry.Range, dummyRefNode, modifier, null, true);

            builder.Write(entrySeparator);

            IList<Run> range = entry.BuildPagesRuns(builder.Document, usePassim, pageNumberListSeparator, sequenceSeparator, pageRangeSeparator);
            foreach (Run run in range)
                builder.InsertNode(run);

            dummyRefNode.Remove();
        }

        private static string NormalizeSeparator(string switchValue, string defaultValue)
        {
            const int maxSeparatorLength = 15;

            return switchValue == null
                ? defaultValue
                : StringUtil.Truncate(switchValue, maxSeparatorLength);
        }

        private Bookmark FetchBookmark(string bookmarkName)
        {
            if (bookmarkName == string.Empty)
                return null;

            Document document = FetchDocument();
            Bookmark bookmark = document.Range.Bookmarks[bookmarkName];

            if (bookmark == null)
                return null;

            if (bookmark.BookmarkStart.GetAncestor(NodeType.Body) == null)
                return null;

            return bookmark;
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case RemoveEntryFormattingSwitch:
                case UsePassimSwitch:
                case HeadingSwitch:
                    return FieldSwitchType.Flag;
                case BookmarkNameSwitch:
                case EntryCategorySwitch:
                case SequenceSeparatorSwitch:
                case EntrySeparatorSwitch:
                case PageRangeSeparatorSwitch:
                case PageNumberListSeparatorSwitch:
                case SequenceNameSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark that marks the portion of the document used to build the table.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(BookmarkNameSwitch); }
            set { FieldCodeCache.SetSwitch(BookmarkNameSwitch, value); }
        }

        internal bool HasBookmarkNameSwitch
        {
            get { return FieldCodeCache.HasSwitch(BookmarkNameSwitch);}
        }

        /// <summary>
        /// Gets or sets the integral category for entries included in the table.
        /// </summary>
        public string EntryCategory //int
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryCategorySwitch); }
            set { FieldCodeCache.SetSwitchAsInt32(EntryCategorySwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate sequence numbers and page numbers.
        /// </summary>
        public string SequenceSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SequenceSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(SequenceSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate a table of authorities entry and its page number.
        /// </summary>
        public string EntrySeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntrySeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(EntrySeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to remove the formatting of the entry text in the document from the
        /// entry in the table of authorities.
        /// </summary>
        public bool RemoveEntryFormatting
        {
            get { return FieldCodeCache.HasSwitch(RemoveEntryFormattingSwitch); }
            set { FieldCodeCache.SetSwitch(RemoveEntryFormattingSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate the start and end of a page range.
        /// </summary>
        public string PageRangeSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageRangeSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(PageRangeSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to include the category heading for the entries in a table of authorities.
        /// </summary>
        public bool UseHeading
        {
            get { return FieldCodeCache.HasSwitch(HeadingSwitch); }
            set { FieldCodeCache.SetSwitch(HeadingSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the character sequence that is used to separate two page numbers in a page number list.
        /// </summary>
        public string PageNumberListSeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageNumberListSeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(PageNumberListSeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to replace five or more different page references to the same
        /// authority with "passim", which is used to indicate that a word or passage occurs frequently
        /// in the work cited.
        /// </summary>
        public bool UsePassim
        {
            get { return FieldCodeCache.HasSwitch(UsePassimSwitch); }
            set { FieldCodeCache.SetSwitch(UsePassimSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of a sequence whose number is included with the page number.
        /// </summary>
        public string SequenceName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(SequenceNameSwitch); }
            set { FieldCodeCache.SetSwitch(SequenceNameSwitch, value); }
        }

        private const string BookmarkNameSwitch = "\\b";
        private const string EntryCategorySwitch = "\\c";
        private const string SequenceSeparatorSwitch = "\\d";
        private const string EntrySeparatorSwitch = "\\e";
        private const string RemoveEntryFormattingSwitch = "\\f";
        private const string PageRangeSeparatorSwitch = "\\g";
        private const string HeadingSwitch = "\\h";
        private const string PageNumberListSeparatorSwitch = "\\l";
        private const string UsePassimSwitch = "\\p";
        private const string SequenceNameSwitch = "\\s";

        private const string CategoryNumberNotFoundError = "Error! Category number not found.";
        private const string NoTableOfAuthoritiesEntriesFoundError = "No table of authorities entries found.";
        private const string NoBookmarkNameGivenError = "Error! No bookmark name given.";
        private const string BookmarkNotDefinedError = "Error! Bookmark not defined.";
    }
}
