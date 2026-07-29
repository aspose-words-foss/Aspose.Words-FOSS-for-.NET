// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2010 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Math;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the TOC field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Builds a table of contents (which can also be a table of figures) using the entries specified by TC fields,
    /// their heading levels, and specified styles, and inserts that table at this place in the document.
    /// </remarks>
    public class FieldToc : Field, IFieldCodeTokenInfoProvider
    {
        /// <summary>
        /// Updates the page numbers for items in this table of contents.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the operation is successful. If any of the related TOC bookmarks was removed, <c>false</c> will be returned.
        /// </returns>
        public bool UpdatePageNumbers()
        {
            NodeRange resultRange = GetFieldResultRange();

            IList<Field> pageRefFields = FieldExtractor.ExtractToCollection(resultRange, true, FieldType.FieldPageRef);

            if (pageRefFields.Count == 0)
                return false;

            foreach (Field field in pageRefFields)
            {
                if (((FieldPageRef)field).BookmarkNode == null)
                    return false;
            }

            FieldUpdater.UpdateFields(pageRefFields);

            return true;
        }

        internal override FieldUpdateStage GetUpdateStage()
        {
            return FieldUpdateStage.DeferredUpdateRef;
        }

        internal override FieldUpdateAction UpdateCore()
        {
            RemoveOldResultBookmarks();

            // The TOC field result is way too complex, so build it right away.
            UpdateContext.RemoveOldResultSafe();

            FieldUpdateAction result = UpdateParsed();
            if (result != null)
                return result;

            UpdateContext.JoinOldResultNodes();

            // WORDSNET-7485 If temporal paragraph was inserted it need to remove it.
            if (mIsExtraParaWasAdded)
                RemoveExtraParagraph();

            // Nothing else to do with the field.
            return new FieldUpdateActionDoNothing(this);
        }

#if DEBUG || JAVA
        /// <summary>
        /// For tests purpose only.
        /// </summary>
        internal ITocEntryExtractorOptions ParseOptions()
        {
            return ParsedFieldToc.TryParse(this);
        }
#endif

        private FieldUpdateAction UpdateParsed()
        {
            ParsedFieldToc options = ParsedFieldToc.TryParse(this);
            if (options == null)
                return new FieldUpdateActionInsertErrorMessage(this, LevelRange.ParsingErrorMessage);

            DocumentBuilder builder = FieldIndexAndTablesUtil.GetDocumentBuilder(this);
            bool isExtraParaAlreadyAdded = !FieldIndexAndTablesUtil.IsFirstNonZeroLengthChild(this);

            // WORDSNET-7485 Parent paragraph of the End node might be simultaneously a TOC entry.
            // In this case ParagraphTocEntry instance which will be built with such paragraph
            // will contain invalid mParagraph reference because of inserting of new paragraphs.
            // We insert temporal paragraph to separate End node and TOC entry.
            if (!isExtraParaAlreadyAdded &&
                Start.ParentParagraph != End.ParentParagraph &&
                TocEntryExtractor.IsParagraphTocEntry(End.ParentParagraph, options))
            {
                InsertExtraParagraph(builder);
            }

            IList<ITocEntry> entries = TocEntryExtractor.ExtractTocEntries(FetchDocument(), options);

            if (entries.Count == 0)
                return new FieldUpdateActionInsertErrorMessage(this, GetNoEntriesErrorMessage(options));

            // List labels should be actual before field value is constructed.
            Updater.RequestExternalAction(new ExternalActionUpdateListLabels(FetchDocument()));

            // FOSS

            if (!BuildTocEntries(builder, entries, options))
                return new FieldUpdateActionInsertErrorMessage(this, GetNoEntriesErrorMessage(options));

            return null;
        }

        internal override void BeforeUnlink()
        {
            NodeRange range = GetFieldResultRange();
            foreach (Node node in range)
            {
                Inline inline = node as Inline;
                if ((inline != null) && (inline.Font.StyleIdentifier == StyleIdentifier.Hyperlink))
                    inline.RunPr.Remove(FontAttr.Istd);
            }
        }

        private void RemoveOldResultBookmarks()
        {
            IList<Field> fields = FieldExtractor.ExtractToCollection(
                GetFieldResultRange(),
                true,
                FieldType.FieldPageRef,
                FieldType.FieldHyperlink,
                FieldType.FieldSequence);
            if (fields.Count == 0)
                return;

            ISetGeneric<string> removedBookmarks = new CaseInsensitiveHashSet(fields.Count);
            foreach (Field field in fields)
            {
                string bookmarkName = GetFieldBookmarkName(field);
                if (string.IsNullOrEmpty(bookmarkName))
                    continue;

                if(!removedBookmarks.Add(bookmarkName))
                    continue;

                Bookmark bookmark = Updater.GetCachedBookmark(bookmarkName);
                if (bookmark == null)
                    continue;

                bookmark.Remove();
            }
        }

        private static string GetFieldBookmarkName(Field field)
        {
            switch (field.Type)
            {
                case FieldType.FieldPageRef:
                    return ((FieldPageRef)field).BookmarkName;
                case FieldType.FieldHyperlink:
                    return ((FieldHyperlink)field).SubAddress;
                case FieldType.FieldSequence:
                    return ((FieldSeq)field).BookmarkName;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool BuildTocEntries(
            DocumentBuilder builder,
            IList<ITocEntry> entries,
            ParsedFieldToc options)
        {
            // SPEED 17460 Do not use BookmarkCollection in a loop! Collect bookmark names first.
            string[] bookmarkNames = GetDocumentBookmarkNames();
            Array.Sort(bookmarkNames);

            bool result = false;
            foreach (ITocEntry entry in entries)
                result = BuildTocEntry(builder, bookmarkNames, entry, options) || result;

            return result;
        }

        private string[] GetDocumentBookmarkNames()
        {
            NodeCollection bookmarkNodes = Document.GetChildNodes(new NodeType[] {NodeType.BookmarkStart, NodeType.BookmarkEnd}, true);
            string[] bookmarkNames = new string[bookmarkNodes.Count];

            for (int i = 0; i < bookmarkNodes.Count; i++)
            {
                IBookmarkNode bookmarkNode = (IBookmarkNode)bookmarkNodes[i];
                bookmarkNames[i] = bookmarkNode.Name;
            }

            return bookmarkNames;
        }

        private bool BuildTocEntry(DocumentBuilder builder, string[] bookmarkNames, ITocEntry entry, ParsedFieldToc options)
        {
            // Mark entry contents with a hidden bookmark to make PAGEREF fields that are a part of the TOC's result
            // reference them properly.
            string bookmarkName = GetUniqueTocBookmarkName(bookmarkNames);

            NodeRange bookmarkRange = entry.InsertBookmark(bookmarkName);

            // Failed to insert bookmark (for example, no valid child nodes in the paragraph)?
            if (bookmarkRange == null)
                return false;

            FieldIndexAndTablesUtil.ConvertOuterInlineSdtToBlock(this);

            StyleIdentifier styleIdentifier = GetStyleIdentifierForLevel(entry.Level, options.IsTableOfFigures);

            // WORDSNET-18533 Remember and restore field end paragraph break font
            RunPr breakRunPr = End.ParentParagraph.ParagraphBreakRunPr.Clone();

            // Set the current paragraph's properties.
            FieldIndexAndTablesUtil.SetUpEntryParagraph(this, builder, styleIdentifier);
            FieldIndexAndTablesUtil.SetFontForTabStopOrParagraphBreak(FetchDocument(), builder.CurrentParagraph);

            End.ParentParagraph.ParagraphBreakRunPr = breakRunPr;

            builder.ClearFont();

            Node dummyRefNode = FieldIndexAndTablesUtil.CreateAndInsertDummyRefNode(builder);
            Node refNode = InsertHyperlink(entry, dummyRefNode, builder, bookmarkName);

            BuildTocEntry(entry, bookmarkRange, refNode, options);

            builder.MoveTo(refNode);
            builder.ClearFont();

            BuildEntryPageNumber(builder, entry, bookmarkName, options);

            dummyRefNode.Remove();

            return true;
        }

        // WORDSNET-7485 Insert extra paragraph which will be temporal parent for End node.
        private void InsertExtraParagraph(DocumentBuilder builder)
        {
            builder.MoveTo(End.NextSibling ?? End.ParentNode);

            // WORDSNET-17217 Workaround. The DocumentBuilder.InsertParagraph loses ParagraphBreakRunPr.
            RunPr currentParaBreakPr = builder.CurrentParagraph.ParagraphBreakRunPr.Clone();

            builder.EnsureAtStructuredDocumentTagEnd();

            builder.Writeln();
            builder.CurrentParagraph.ParagraphBreakRunPr = currentParaBreakPr;

            Paragraph para = builder.CurrentParagraph;
            builder.MoveTo(builder.CurrentParagraph.PreviousNonAnnotationSibling);
            builder.ParagraphFormat.ClearFormatting();
            builder.CurrentParagraph.ParagraphBreakFont.ClearFormatting();
            para.CloneListLabelStringAndValueIfNeeded(builder.CurrentParagraph);

            mIsExtraParaWasAdded = true;
        }

        // WORDSNET-7485 Remove temporal parent paragraph of End node.
        private void RemoveExtraParagraph()
        {
            Paragraph endParagraph = End.ParentParagraph;
            if (endParagraph.NextSibling == null)
                return;

            Paragraph toRemove = (Paragraph)endParagraph.NextNonAnnotationSibling;

            if (toRemove.HasChildNodes)
            {
                NodeRemover.Remove(toRemove, true, toRemove.FirstChild, false, NodeJoinMode.JoinToNextSibling);
            }
            else
            {
                endParagraph.ParaPr = toRemove.ParaPr;
                endParagraph.ParagraphBreakRunPr = toRemove.ParagraphBreakRunPr;
                toRemove.Remove();
            }
        }

        private Node InsertHyperlink(ITocEntry entry, Node refNode, DocumentBuilder builder, string sourceEntryBookmarkName)
        {
            if (InsertHyperlinks && entry.HasBookmark)
            {
                builder.MoveTo(refNode);
                Field field = builder.InsertField(string.Format(" HYPERLINK \\l \"{0}\" ", sourceEntryBookmarkName), "");
                return field.End;
            }

            return refNode;
        }

        /// <summary>
        /// Copies TOC entry nodes to the TOC.
        /// </summary>
        private void BuildTocEntry(ITocEntry entry, NodeRange bookmarkRange, Node refNode, ITocEntryExtractorOptions options)
        {
            CompositeModifier compositeModifier = new CompositeModifier();

            FieldTC fieldTc = null;
            RefDocTocEntry refDocTocEntry = entry as RefDocTocEntry;
            if (refDocTocEntry != null)
            {
                compositeModifier.AddModifier(new ExternalDocumentModifier(refDocTocEntry.Document, Document, ImportFormatMode.UseDestinationStyles));
                fieldTc = refDocTocEntry.RefTocEntry as FieldTC;
            }

            // WORDSNET-23793 Only un-escape TC argument.
            fieldTc = fieldTc ?? (entry as FieldTC);
            if (fieldTc != null)
                compositeModifier.AddModifier(new FieldTokenDecoderNodeModifier(fieldTc.TextRange, FieldTokenDecoderOptions.All));

            TocEntryNodeModifier tocEntryNodeModifier = new TocEntryNodeModifier(
                PreserveTabs,
                PreserveLineBreaks,
                entry.Level,
                options.IsTableOfFigures);
            compositeModifier.AddModifier(tocEntryNodeModifier);

            INodeModifier attributeModifier = GetAttributeModifier(entry);
            compositeModifier.AddModifier(attributeModifier);

            NodeRange labelRange = entry.GetLabelRange();
            if (labelRange != null)
                NodeCopier.Copy(labelRange, refNode, compositeModifier);

            CompositeNodeCopierListener compositeListener = new CompositeNodeCopierListener();
            compositeListener.Add(new FieldFakeResultAppender(this, new FieldFakeResultNodeModifierAdapter(attributeModifier)));
            compositeListener.Add(new CompositeNodeReplacer(NodeType.StructuredDocumentTag, NodeType.SmartTag));
            compositeListener.Add(new OfficeMathReplacer());

            IFieldRemoverFilter retainAdvanceFieldsFilter = new FieldRemoverRetainCertainFieldsFilter(FieldType.FieldAdvance);

            NodeCopier.CopyWithoutFields(bookmarkRange, refNode, compositeModifier, compositeListener, retainAdvanceFieldsFilter);

            tocEntryNodeModifier.FinalizeEntry();
        }

        private void BuildEntryPageNumber(
            DocumentBuilder builder,
            ITocEntry entry,
            string sourceEntryBookmarkName,
            ParsedFieldToc options)
        {
            // WORDSNET-20691 MS Word applies tabstop regardless of presence of page number.
            FieldIndexAndTablesUtil.EnsureEntryParagraphTabStop(builder, Start.ParentParagraph);

            bool omitPageNumber =
                (IsPageNumberOmittingLevelRangeSpecified &&
                options.PageNumberOmittingLevelRange.IsInRange(entry.Level)) ||
                entry.OmitPageNumber;

            // Build this unless we are instructed to omit page number.
            if (omitPageNumber)
                return;

            string separator = StringUtil.HasChars(EntrySeparator)
                                   ? EntrySeparator[0].ToString() // Word only takes the first character of the custom separator.
                                   : ControlChar.Tab;
            builder.Write(separator);

            if (!string.IsNullOrEmpty(PrefixedSequenceIdentifier))
            {
                BuildEntryPageNumberPrefix(builder, entry, sourceEntryBookmarkName);

                string sequenceSeparator = StringUtil.HasChars(SequenceSeparator)
                    ? SequenceSeparator[0].ToString()
                    : "-";
                builder.Write(sequenceSeparator);
            }

            BuildEntryPageNumberPostfix(builder, entry, sourceEntryBookmarkName);

            Updater.FreezeTocEntryBookmark(sourceEntryBookmarkName);
        }

        private void BuildEntryPageNumberPrefix(DocumentBuilder builder, ITocEntry entry, string sourceEntryBookmarkName)
        {
            if (entry.HasBookmark)
            {
                builder.InsertField(
                    string.Format(@" SEQ {0} {1} \* ARABIC ", PrefixedSequenceIdentifier, sourceEntryBookmarkName),
                    PageNumberPlaceholder);

                FieldSeqDataProvider provider = (FieldSeqDataProvider)Updater.DataProviders.GetOfType(typeof(FieldSeqDataProvider));
                if (provider != null)
                    provider.AddSequenceBookmark(sourceEntryBookmarkName);
            }
            else
            {
                builder.Write(entry.GetSequenceValue(PrefixedSequenceIdentifier).ToString());
            }
        }

        private static void BuildEntryPageNumberPostfix(DocumentBuilder builder, ITocEntry entry, string sourceEntryBookmarkName)
        {
            if (entry.HasBookmark)
            {
                // Insert a PAGEREF field whose result is just a placeholder ("?") for the page number.
                builder.InsertField(
                    string.Format(" PAGEREF {0} \\h ", sourceEntryBookmarkName),
                    PageNumberPlaceholder);
            }
            else
            {
                builder.Write(entry.GetPageNumber().ToString());
            }
        }

        /// <summary>
        /// Makes an error message to display in TOC for the case when no TOC entries were found.
        /// </summary>
        private string GetNoEntriesErrorMessage(ITocEntryExtractorOptions options)
        {
            if (options.IsBookmarkRangeSpecified && (options.GetRangeBookmark() == null))
            {
                FieldArgument bookmarkArg = FieldCodeCache.GetSwitchArgument(BookmarkNameSwitch);
                bool isBookmarkNameReallySpecified = (bookmarkArg != null) && !bookmarkArg.Range.IsVoid;
                return isBookmarkNameReallySpecified
                    ? Bookmark.ErrorBookmarkNotDefined
                    : "Error! No bookmark name given.";
            }

            return options.IsTableOfFigures
                ? "No table of figures entries found."
                : "No table of contents entries found.";
        }

        private INodeModifier GetAttributeModifier(ITocEntry entry)
        {
            // WORDSNET-23594 Strip duplicate direct formatting from TOC entries
            // WORDSNET-26975 Wrong colors in TOC.
            StyleIdentifier tocStyleId = GetStyleIdentifierForLevel(entry.Level, false);

            return InsertHyperlinks && entry.HasBookmark
                       ? (INodeModifier) new TocHyperlinkEntryAttributeModifier(tocStyleId, Document.Styles)
                       : new TocNormalEntryAttributeModifier(tocStyleId, Document.Styles, !entry.IsLinkedStyleTocEntry);
        }

        /// <summary>
        /// Returns a unique name for a TOC bookmark.
        /// </summary>
        /// <param name="bookmarkNames">A list of existing bookmark names (those that existed before update).</param>
        private string GetUniqueTocBookmarkName(string[] bookmarkNames)
        {
            string bookmarkName;
            bool contains;
            do
            {
                bookmarkName = string.Format("_Toc{0}", FetchDocument().GetNextTocEntryBookmarkIndex());
                contains = ArrayUtil.BinarySearch(bookmarkNames, bookmarkName) >= 0;
            }
            while (contains);

            return bookmarkName;
        }

        internal static StyleIdentifier GetStyleIdentifierForLevel(int entryLevel, bool isTableOfFigures)
        {
            if (isTableOfFigures)
                return StyleIdentifier.TableOfFigures;

            switch (entryLevel)
            {
                case 1:
                    return StyleIdentifier.Toc1;
                case 2:
                    return StyleIdentifier.Toc2;
                case 3:
                    return StyleIdentifier.Toc3;
                case 4:
                    return StyleIdentifier.Toc4;
                case 5:
                    return StyleIdentifier.Toc5;
                case 6:
                    return StyleIdentifier.Toc6;
                case 7:
                    return StyleIdentifier.Toc7;
                case 8:
                    return StyleIdentifier.Toc8;
                case 9:
                    return StyleIdentifier.Toc9;
                default:
                    // Word works this way.
                    return StyleIdentifier.Toc1;
            }
        }

        FieldSwitchType IFieldCodeTokenInfoProvider.GetSwitchType(string switchName)
        {
            switch (switchName)
            {
                case InsertHyperlinksSwitch:
                case UseParagraphOutlineLevelSwitch:
                case PreserveTabsSwitch:
                case PreserveLineBreaksSwitch:
                case HideInWebLayoutSwitch:
                    return FieldSwitchType.Flag;
                case CaptionlessTableOfFiguresLabelSwitch:
                case BookmarkNameSwitch:
                case TableOfFiguresLabelSwitch:
                case SequenceSeparatorSwitch:
                case EntryIdentifierSwitch:
                case EntryLevelRangeSwitch:
                case PageNumberOmittingLevelRangeSwitch:
                case HeadingLevelRangeSwitch:
                case EntrySeparatorSwitch:
                case PrefixedSequenceIdentifierSwitch:
                case CustomStylesSwitch:
                    return FieldSwitchType.HasArgument;
                default:
                    return FieldSwitchType.Unknown;
            }
        }

        private bool IsPageNumberOmittingLevelRangeSpecified
        {
            get { return FieldCodeCache.HasSwitch(PageNumberOmittingLevelRangeSwitch); }
        }

        /// <summary>
        /// Gets or sets the name of the bookmark that marks the portion of the document used to build the table.
        /// </summary>
        public string BookmarkName
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(BookmarkNameSwitch); }
            set { FieldCodeCache.SetSwitch(BookmarkNameSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of the sequence identifier used when building a table of figures.
        /// </summary>
        public string TableOfFiguresLabel
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(TableOfFiguresLabelSwitch); }
            set { FieldCodeCache.SetSwitch(TableOfFiguresLabelSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the name of the sequence identifier used when building a table of figures that does not include caption's
        /// label and number.
        /// </summary>
        public string CaptionlessTableOfFiguresLabel
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(CaptionlessTableOfFiguresLabelSwitch); }
            set { FieldCodeCache.SetSwitch(CaptionlessTableOfFiguresLabelSwitch, value); }
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
        /// Gets or sets a string that should match type identifiers of TC fields being included.
        /// </summary>
        public string EntryIdentifier
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryIdentifierSwitch); }
            set { FieldCodeCache.SetSwitch(EntryIdentifierSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to make the table of contents entries hyperlinks.
        /// </summary>
        public bool InsertHyperlinks
        {
            get { return FieldCodeCache.HasSwitch(InsertHyperlinksSwitch); }
            set { FieldCodeCache.SetSwitch(InsertHyperlinksSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a range of levels of the table of contents entries to be included.
        /// </summary>
        public string EntryLevelRange
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntryLevelRangeSwitch); }
            set { FieldCodeCache.SetSwitch(EntryLevelRangeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a range of levels of the table of contents entries from which to omits page numbers.
        /// </summary>
        public string PageNumberOmittingLevelRange
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PageNumberOmittingLevelRangeSwitch); }
            set { FieldCodeCache.SetSwitch(PageNumberOmittingLevelRangeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a range of heading levels to include.
        /// </summary>
        public string HeadingLevelRange
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(HeadingLevelRangeSwitch); }
            set { FieldCodeCache.SetSwitch(HeadingLevelRangeSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a sequence of characters that separate an entry and its page number.
        /// </summary>
        public string EntrySeparator
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(EntrySeparatorSwitch); }
            set { FieldCodeCache.SetSwitch(EntrySeparatorSwitch, value); }
        }

        /// <summary>
        /// Gets or sets the identifier of a sequence for which a prefix should be added to the entry's page number.
        /// </summary>
        public string PrefixedSequenceIdentifier
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(PrefixedSequenceIdentifierSwitch); }
            set { FieldCodeCache.SetSwitch(PrefixedSequenceIdentifierSwitch, value); }
        }

        /// <summary>
        /// Gets or sets a list of styles other than the built-in heading styles to include in the table of contents.
        /// </summary>
        public string CustomStyles
        {
            get { return FieldCodeCache.GetSwitchArgumentAsString(CustomStylesSwitch); }
            set { FieldCodeCache.SetSwitch(CustomStylesSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to use the applied paragraph outline level.
        /// </summary>
        public bool UseParagraphOutlineLevel
        {
            get { return FieldCodeCache.HasSwitch(UseParagraphOutlineLevelSwitch); }
            set { FieldCodeCache.SetSwitch(UseParagraphOutlineLevelSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to preserve tab entries within table entries.
        /// </summary>
        public bool PreserveTabs
        {
            get { return FieldCodeCache.HasSwitch(PreserveTabsSwitch); }
            set { FieldCodeCache.SetSwitch(PreserveTabsSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to preserve newline characters within table entries.
        /// </summary>
        public bool PreserveLineBreaks
        {
            get { return FieldCodeCache.HasSwitch(PreserveLineBreaksSwitch); }
            set { FieldCodeCache.SetSwitch(PreserveLineBreaksSwitch, value); }
        }

        /// <summary>
        /// Gets or sets whether to hide tab leader and page numbers in Web layout view.
        /// </summary>
        public bool HideInWebLayout
        {
            get { return FieldCodeCache.HasSwitch(HideInWebLayoutSwitch); }
            set { FieldCodeCache.SetSwitch(HideInWebLayoutSwitch, value); }
        }

        // WORDSNET-7485
        private bool mIsExtraParaWasAdded;

        private const string CaptionlessTableOfFiguresLabelSwitch = "\\a";
        private const string BookmarkNameSwitch = "\\b";
        private const string TableOfFiguresLabelSwitch = "\\c";
        private const string SequenceSeparatorSwitch = "\\d";
        private const string EntryIdentifierSwitch = "\\f";
        private const string InsertHyperlinksSwitch = "\\h";
        private const string EntryLevelRangeSwitch = "\\l";
        private const string PageNumberOmittingLevelRangeSwitch = "\\n";
        private const string HeadingLevelRangeSwitch = "\\o";
        private const string EntrySeparatorSwitch = "\\p";
        private const string PrefixedSequenceIdentifierSwitch = "\\s";
        private const string CustomStylesSwitch = "\\t";
        private const string UseParagraphOutlineLevelSwitch = "\\u";
        private const string PreserveTabsSwitch = "\\w";
        private const string PreserveLineBreaksSwitch = "\\x";
        private const string HideInWebLayoutSwitch = "\\z";

        // It used to be a "??" placeholder, but it was changed to "?" per JIRA - 10635.
        // Lines with "??" placeholders wrapped differently from lines with single-digit page numbers.
        // It caused contents to shift down and incorrect page numbers in TOC as a result.
        private const string PageNumberPlaceholder = "?";

        private class FieldFakeResultNodeModifierAdapter : IFieldFakeResultNodeModifier
        {
            public FieldFakeResultNodeModifierAdapter(INodeModifier attributeModifier)
            {
                mAttributeModifier = attributeModifier;
            }

            void IFieldFakeResultNodeModifier.Modify(Node node)
            {
                mAttributeModifier.Modify(node, node, false, null);
            }

            private readonly INodeModifier mAttributeModifier;
        }

        private class OfficeMathReplacer : NodeReplacer
        {
            internal OfficeMathReplacer()
                : base(NodeType.OfficeMath)
            {
            }

            protected override void CollectClone(Node source, Node clone)
            {
                OfficeMath officeMath = (OfficeMath)source;
                if (!officeMath.IsTopLevel)
                    return;

                base.CollectClone(source, clone);
            }

            protected override void ReplaceCollectedNode(Node node)
            {
                OfficeMath officeMath = (OfficeMath)node;
                LinearizeOfficeMath(officeMath);
            }

            private static void LinearizeOfficeMath(OfficeMath officeMath)
            {
                if (officeMath.CanInsert(new Run(officeMath.Document)))
                {
                    Node[] runs = officeMath.GetChildNodes(NodeType.Run, true).ToArray();
                    officeMath.RemoveAllChildren();
                    foreach (Node run in runs)
                        officeMath.AppendChild(run);
                }
                else
                {
                    foreach (OfficeMath childOfficeMath in officeMath.GetChildNodes(NodeType.OfficeMath, false))
                        LinearizeOfficeMath(childOfficeMath);
                }
            }
        }

        private sealed class ParsedFieldToc : ITocEntryExtractorOptions
        {
            private ParsedFieldToc(
                FieldToc field,
                LevelRange entryLevelRange,
                LevelRange headingLevelRange,
                LevelRange pageNumberOmittingLevelRange)
            {
                mField = field;

                mCustomStylesToLevelsMap = ParseCustomStyles(field);
                TocEntryLevelRange = entryLevelRange;
                HeadingLevelRange = headingLevelRange;
                PageNumberOmittingLevelRange = pageNumberOmittingLevelRange;

                EntryType = FieldIndexAndTablesUtil.GetTocEntryType(field.FieldCodeCache, EntryIdentifierSwitch);

                IsBookmarkRangeSpecified = HasSwitch(BookmarkNameSwitch);
                IncludeTocEntryFields = HasSwitch(EntryIdentifierSwitch);
                IsEntryLevelRangeSpecified = HasSwitch(EntryLevelRangeSwitch);
                IsTableOfFigures = HasSwitch(CaptionlessTableOfFiguresLabelSwitch) ||
                                   HasSwitch(TableOfFiguresLabelSwitch);
                IsHeadingLevelRangeSpecified = HasSwitch(HeadingLevelRangeSwitch);
                AreCustomStylesSpecified = HasSwitch(CustomStylesSwitch);
                UseParagraphOutlineLevel = mField.UseParagraphOutlineLevel;
                TableOfFiguresLabel = mField.TableOfFiguresLabel;
                CaptionlessTableOfFiguresLabel = mField.CaptionlessTableOfFiguresLabel;
                End = mField.End;
                Start = mField.Start;

                // \l switch without \f switch works, but only if \l switch argument is specified.
                if (!IncludeTocEntryFields && (field.FieldCodeCache.GetSwitchArgument(EntryLevelRangeSwitch) == null))
                    TocEntryLevelRange = LevelRange.EmptyRange;
            }

            public bool IsBookmarkRangeSpecified { get; }
            public bool IncludeTocEntryFields { get; }
            public bool IsEntryLevelRangeSpecified { get; }
            public bool IsTableOfFigures { get; }
            public bool IsHeadingLevelRangeSpecified { get; }
            public bool AreCustomStylesSpecified { get; }
            public bool UseParagraphOutlineLevel { get; }
            public string TableOfFiguresLabel { get; }
            public LevelRange TocEntryLevelRange { get; }
            public LevelRange HeadingLevelRange { get; }
            internal LevelRange PageNumberOmittingLevelRange { get; }
            public string CaptionlessTableOfFiguresLabel { get; }
            public int EntryType { get; }
            public FieldEnd End { get; }
            public FieldStart Start { get; }

            public bool SkipTables
            {
                get { return false; }
            }

            public bool IncludeRefDocFields
            {
                get { return true; }
            }

            public Bookmark GetRangeBookmark()
            {

                if (mField.BookmarkName == null)
                    return null;

                Bookmark bookmark = mField.Document.Range.Bookmarks[mField.BookmarkName];
                if (bookmark == null)
                    return null;

                // Check if it is in the section body.
                bool bookmarkIsInSectionBody = NodeUtil.HasAncestor(bookmark.BookmarkStart, NodeType.Body) &&
                                               NodeUtil.HasAncestor(bookmark.BookmarkEnd, NodeType.Body);

                return bookmarkIsInSectionBody
                    ? bookmark
                    : null;
            }

            public int GetLevelForCustomStyle(Paragraph paragraph, Style style)
            {
                if (UseParagraphOutlineLevel)
                {
                    int outlineLevel = TocEntryExtractor.GetParagraphLevelFromOutlineLevel(paragraph);
                    if (!LevelRange.IsLevelValid(outlineLevel))
                        return LevelRange.InvalidLevel;
                }

                int level = mCustomStylesToLevelsMap[style.Name];
                if (!StringToIntDictionary.IsNullSubstitute(level))
                    return level;

                // WORDSNET-12288 Compare custom styles with style aliases.
                string[] styleAliases = style.GetAliasesInternal();
                foreach (string styleAlias in styleAliases)
                {
                    level = mCustomStylesToLevelsMap[styleAlias];
                    if (!StringToIntDictionary.IsNullSubstitute(level))
                        return level;
                }

                return LevelRange.InvalidLevel;
            }

            private bool HasSwitch(string switchName)
            {
                return mField.FieldCodeCache.HasSwitch(switchName);
            }

            internal static ParsedFieldToc TryParse(FieldToc field)
            {
                LevelRange entryLevelRange = ParseSwitchLevelRange(field, EntryLevelRangeSwitch);
                if (entryLevelRange == null)
                    return null;

                LevelRange headingLevelRange = ParseSwitchLevelRange(field, HeadingLevelRangeSwitch);
                if (headingLevelRange == null)
                    return null;

                LevelRange pageNumberOmittingLevelRange = ParseSwitchLevelRange(field, PageNumberOmittingLevelRangeSwitch);
                if (pageNumberOmittingLevelRange == null)
                    return null;

                return new ParsedFieldToc(field, entryLevelRange, headingLevelRange, pageNumberOmittingLevelRange);
            }

            private static LevelRange ParseSwitchLevelRange(Field field, string switchName)
            {
                if (!field.FieldCodeCache.HasSwitch(switchName))
                    return LevelRange.MaxRange;

                FieldArgument argument = field.FieldCodeCache.GetSwitchArgument(switchName);
                if (argument == null)
                    return LevelRange.MaxRange;

                return LevelRange.TryParse(argument.GetNormalizedText());
            }

            private static StringToIntDictionary ParseCustomStyles(FieldToc field)
            {
                StringToIntDictionary result = new StringToIntDictionary(false);

                string separator = field.FetchDocument().FieldOptions.CustomTocStyleSeparator;
                if (string.IsNullOrEmpty(separator))
                {
                    // WORDSNET-16998 Use list separators from the thread culture rather than hardcoded ones.
                    separator = FormatterPal.GetListSeparatorCurrent().ToString();
                }

                foreach (string customStyles in field.FieldCodeCache.GetSwitchArgumentsAsStrings(CustomStylesSwitch))
                {
                    if (string.IsNullOrEmpty(customStyles))
                        continue;

                    string[] customStylesParts = customStyles.Split(
                        new string[] { separator },
                        StringSplitOptions.RemoveEmptyEntries);

                    ParseCustomStyles(result, field, customStylesParts);
                }

                return result;
            }

            private static void ParseCustomStyles(StringToIntDictionary result, Field field, string[] customStylesParts)
            {
                int implicitStyleLevel = LevelRange.MinLevel;
                StyleCollection styles = field.Document.Styles;

                int i = 0;
                while (i < customStylesParts.Length)
                {
                    string styleName = customStylesParts[i].Trim();
                    // WORDSNET-10465 Use invariant built-in style name, if exist.
                    string invariantStyleName = StyleNameTranslator.GetInvariantStyleName(styleName);

                    if (i == customStylesParts.Length - 1)
                    {
                        MapCustomStyleToLevel(result, styleName, invariantStyleName, 1);
                        return;
                    }

                    string styleLevelString = customStylesParts[i + 1].Trim();
                    int styleLevel = FormatterPal.TryParseIntPortion(styleLevelString);

                    if (LevelRange.IsLevelValid(styleLevel))
                    {
                        i++;
                        implicitStyleLevel = LevelRange.MaxLevel;
                    }
                    else if (HasStyle(styles, styleName, invariantStyleName))
                    {
                        styleLevel = implicitStyleLevel;
                        if (implicitStyleLevel < LevelRange.MaxLevel)
                            implicitStyleLevel++;
                    }

                    if (LevelRange.IsLevelValid(styleLevel))
                        MapCustomStyleToLevel(result, styleName, invariantStyleName, styleLevel);

                    i++;
                }
            }

            private static bool HasStyle(StyleCollection styles, string styleName, string invariantStyleName)
            {
                return HasStyle(styles, styleName) || HasStyle(styles, invariantStyleName);
            }

            private static bool HasStyle(StyleCollection styles, string name)
            {
                if (string.IsNullOrEmpty(name))
                    return false;

                return styles[name] != null;
            }

            private static void MapCustomStyleToLevel(
                StringToIntDictionary result,
                string styleName,
                string invariantStyleName,
                int styleLevel)
            {
                // WORDSNET-13628 & WORDSNET-13235 The DOM may contain style names in non-invariant culture.
                MapCustomStyleToLevel(result, styleName, styleLevel);
                MapCustomStyleToLevel(result, invariantStyleName, styleLevel);
            }

            private static void MapCustomStyleToLevel(StringToIntDictionary result, string styleName, int styleLevel)
            {
                if (string.IsNullOrEmpty(styleName))
                    return;

                if (!result.ContainsKey(styleName)) // WORDSNET-23853 Avoid adding the same style twice.
                    result.Add(styleName, styleLevel);
            }

            private readonly FieldToc mField;
            private readonly StringToIntDictionary mCustomStylesToLevelsMap;
        }
    }
}
