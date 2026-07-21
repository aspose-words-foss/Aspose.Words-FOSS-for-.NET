// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2010 by Dmitry Vorobyev

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Extracts a collection of TOC entries (paragraphs with appropriate styles or TC fields) from a document.
    /// </summary>
    internal class TocEntryExtractor : FieldExtractor
    {
        private TocEntryExtractor(ITocEntryExtractorOptions tocEntryExtractorOptions)
        {
            mTocEntryExtractorOptions = tocEntryExtractorOptions;

            if (mTocEntryExtractorOptions.IsBookmarkRangeSpecified)
            {
                // Remember the bookmark, it is used many times.
                mRangeBookmark = mTocEntryExtractorOptions.GetRangeBookmark();
            }
            else
            {
                // Consider the bookmark range started.
                mIsBookmarkRangeStarted = true;
            }
        }

        internal static IList<ITocEntry> ExtractTocEntries(Document document, ITocEntryExtractorOptions fieldToc)
        {
            TocEntryExtractor extractor = new TocEntryExtractor(fieldToc);
            extractor.ExtractTocEntries(document);
            return extractor.mTocEntries;
        }

        internal static IList<ITocEntry> ExtractTocEntries(Document document, bool extractFromTables)
        {
            TocEntryExtractorOptions options = new TocEntryExtractorOptions();
            options.SkipTables = !extractFromTables;
            TocEntryExtractor extractor = new TocEntryExtractor(options);
            document.UpdateListLabels();
            extractor.ExtractTocEntries(document);
            return extractor.mTocEntries;
        }

        internal static bool IsParagraphTocEntry(Paragraph paragraph, ITocEntryExtractorOptions options)
        {
            TocEntryExtractor extractor = new TocEntryExtractor(options);
            extractor.AddParagraphTocEntries(paragraph);
            return extractor.mTocEntries.Count > 0;
        }

        private void ExtractTocEntries(Document document)
        {
            // TOC entries cannot reside in header/footer stories.
            foreach (Section section in document.Sections)
            {
                if (!section.Body.Accept(this))
                {
                    // Break if bookmark range is finished.
                    break;
                }
            }
        }

        protected override void OnFieldExtracted()
        {
            // WORDSNET-7616, 18083: We have to check that TC/SEQ field belongs to range of TOC's bookmark
            if (mTocEntryExtractorOptions.IsBookmarkRangeSpecified && !mIsBookmarkRangeStarted)
                return;

            switch (CurrentFieldType)
            {
                case FieldType.FieldTOCEntry:
                    ProcessFieldTC((FieldTC)CurrentField);
                    break;
                case FieldType.FieldSequence:
                    ProcessFieldSeq((FieldSeq)CurrentField);
                    break;
                case FieldType.FieldRefDoc:
                    ProcessFieldRD((FieldRD)CurrentField);
                    break;
                default:
                    break;
            }
        }

        private void ProcessFieldTC(FieldTC field)
        {
            if (!mTocEntryExtractorOptions.IncludeTocEntryFields && !mTocEntryExtractorOptions.IsEntryLevelRangeSpecified)
                return;

            // TC's level is out of the range specified by the TOC?
            if (!mTocEntryExtractorOptions.TocEntryLevelRange.IsInRange(((ITocEntry)field).Level))
                return;

            // Type identifier present but does not match the one specified by the TOC?
            int entryType = FieldIndexAndTablesUtil.GetTocEntryType(field.FieldCodeCache, FieldTC.TypeIdentifierSwitch);
            if (!MatchEntryTypes(mTocEntryExtractorOptions.EntryType, entryType))
                return;

            mTocEntries.Add(field);
        }

        private static bool MatchEntryTypes(int x, int y)
        {
            if (x == y)
                return true;

            bool xIsMissing = x == FieldIndexAndTablesUtil.MissingEntryType;
            bool xIsNull = x == FieldIndexAndTablesUtil.NullEntryType;
            bool yIsMissing = y == FieldIndexAndTablesUtil.MissingEntryType;
            bool yIsNull = y == FieldIndexAndTablesUtil.NullEntryType;

            if (xIsMissing && yIsNull)
                return true;

            if (xIsNull && yIsMissing)
                return true;

            const int specialEntryType = 'C';

            bool xIsSpecial = x == specialEntryType;
            bool yIsSpecial = y == specialEntryType;

            if (xIsSpecial && (yIsMissing || yIsNull))
                return true;

            if (yIsSpecial && (xIsMissing || xIsNull))
                return true;

            return false;
        }

        private void ProcessFieldSeq(FieldSeq field)
        {
            if (!mTocEntryExtractorOptions.IsTableOfFigures)
                return;

            if (!IsTableOfFiguresEntry(field))
                return;

            Paragraph paragraph = field.Start.ParentParagraph;

            Node firstNode = string.IsNullOrEmpty(mTocEntryExtractorOptions.CaptionlessTableOfFiguresLabel)
                ? paragraph.FirstChild
                : GetCaptionlessFigureEntryFirstNode(field);
            Node lastNode = paragraph.LastChild;

            ParagraphTocEntryInfo entry = firstNode != null
                ? new ParagraphTocEntryInfo(1, firstNode, lastNode)
                : ParagraphTocEntryInfo.EmptyTocEntryInfoInstance;

            AddTocEntryForParagraph(paragraph, entry);
        }

        /// <summary>
        /// Trims single leading punctuation mark.
        /// </summary>
        private static Node GetCaptionlessFigureEntryFirstNode(Field field)
        {
            Node node = GetNextSiblingEmptyRunAware(field);

            Run run = node as Run;
            if (run == null)
                return node;

            if (!StringUtil.IsPunctuationMark(run.Text[0]))
                return run;

            return run.SplitBefore(1) != null
                ? run
                : run.NextSibling;
        }

        private static Node GetNextSiblingEmptyRunAware(Field field)
        {
            for (Node current = field.End.NextSibling; current != null; current = current.NextSibling)
            {
                if (NodeUtil.IsCrossStructureAnnotation(current))
                    continue;

                if (current.NodeType != NodeType.Run)
                    return current;

                if (((Run)current).Text.Length != 0)
                    return current;
            }

            return null;
        }

        private void ProcessFieldRD(FieldRD field)
        {
            if (!mTocEntryExtractorOptions.IncludeRefDocFields)
                return;

            Document referenceDocument = FieldIndexAndTablesUtil.OpenRefDocument(field);
            if (referenceDocument == null)
                return;

            IList<ITocEntry> entries = ExtractTocEntries(
                referenceDocument,
                new RefDocTocEntryExtractorOptions(mTocEntryExtractorOptions));

            if (entries.Count == 0)
                return;

            referenceDocument.UpdateListLabels();
            LazyFieldSeqValueEvaluator provider = new LazyFieldSeqValueEvaluator(referenceDocument);
            foreach (ITocEntry entry in entries)
                mTocEntries.Add(new RefDocTocEntry(entry, referenceDocument, provider));
        }

        private bool IsTableOfFiguresEntry(FieldSeq field)
        {
            // WORDSNET-18448 Skip hidden and preceding numbers
            if (field.HideFieldResult || field.InsertClosestPrecedingNumber)
                return false;

            if (StringUtil.EqualsIgnoreCase(field.SequenceIdentifier, TableOfFiguresLabel))
                return true;

            if (StringUtil.EqualsIgnoreCase(field.SequenceIdentifier, CaptionlessTableOfFiguresLabel))
                return true;

            return false;
        }

        private string TableOfFiguresLabel
        {
            get
            {
                return mTableOfFiguresLabel ?? (mTableOfFiguresLabel =
                    AdjustTableOfFiguresLabel(mTocEntryExtractorOptions.TableOfFiguresLabel));
            }
        }

        private string CaptionlessTableOfFiguresLabel
        {
            get
            {
                return mCaptionlessTableOfFiguresLabel ?? (mCaptionlessTableOfFiguresLabel =
                    AdjustTableOfFiguresLabel(mTocEntryExtractorOptions.CaptionlessTableOfFiguresLabel));
            }
        }

        private static string AdjustTableOfFiguresLabel(string label)
        {
            return string.IsNullOrEmpty(label)
                ? string.Empty
                : gWhiteSpacesRegex.Replace(label, "_");
        }

        public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            mIsBookmarkRangeStarted = mIsBookmarkRangeStarted || IsBookmarkRangeStart(bookmarkStart);

            return VisitorAction.Continue;
        }

        private bool IsBookmarkRangeStart(Node node)
        {
            if (mRangeBookmark == null)
                return false;

            return node == mRangeBookmark.BookmarkStart;
        }

        public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            return IsBookmarkRangeEnd(bookmarkEnd)
                ? VisitorAction.Stop
                : VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            mFieldCharCounter.VisitNode(fieldStart);

            return base.VisitFieldStart(fieldStart);
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            mFieldCharCounter.VisitNode(fieldSeparator);

            return base.VisitFieldSeparator(fieldSeparator);
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            mFieldCharCounter.VisitNode(fieldEnd);

            return base.VisitFieldEnd(fieldEnd);
        }

        private bool IsInsideFieldCode
        {
            get { return mFieldCharCounter.IsInFieldCode; }
        }

        private bool IsBookmarkRangeEnd(Node node)
        {
            if (mRangeBookmark == null)
                return false;

            return node == mRangeBookmark.BookmarkEnd;
        }

        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            AddParagraphTocEntries(paragraph);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Adds ParagraphTocEntry objects with appropriate level for the specified paragraph to the list.
        /// </summary>
        private void AddParagraphTocEntries(Paragraph paragraph)
        {
            if (IsEmptySectionBreakParagraph(paragraph))
                return;

            // WORDSNET-4929 we should not add items, if there are inside field code
            if (IsInsideFieldCode && !paragraph.IsListItemFinal)
                return;

            // WORDSNET-5091 It seems that Word requires outline level specified explicitly if a TOC entry switch is present.
            bool isExplicitLevelRangeRequired = mTocEntryExtractorOptions.IsEntryLevelRangeSpecified ||
                                                mTocEntryExtractorOptions.IncludeTocEntryFields;
            if (isExplicitLevelRangeRequired &&
                !mTocEntryExtractorOptions.IsHeadingLevelRangeSpecified &&
                !mTocEntryExtractorOptions.AreCustomStylesSpecified)
            {
                return;
            }

            if (AddTocEntryFromCustomStyle(paragraph))
                return;

            if (AddTocEntryFromOutlineLevel(paragraph))
                return;

            if (AddTocEntriesFromHeadingStyle(paragraph))
                return;

            AddTocEntriesFromLinkedStyle(paragraph);
        }

        private static bool IsEmptySectionBreakParagraph(Paragraph paragraph)
        {
            return paragraph.IsSectionBreakParagraph && (paragraph.Runs.Count == 0);
        }

        /// <summary>
        /// Adds a <see cref="ParagraphTocEntry"/> or <see cref="HiddenParagraphTocEntry"/> made from given paragraph and ParagraphTocEntryInfo.
        /// </summary>
        private void AddTocEntryForParagraph(Paragraph paragraph, ParagraphTocEntryInfo entryInfo)
        {
            // WORDSNET-14226 MS Word does not use paragraphs inside tables in PDF bookmarks.
            if (mTocEntryExtractorOptions.SkipTables && (paragraph.GetAncestor(NodeType.Table) != null))
                return;

            if (mLastParagraph == paragraph)
                return;

            mLastParagraph = paragraph;

            if (IsInsideFieldCode)
                AddHiddenParagraphTocEntry(paragraph, entryInfo.Level);
            else
                AddParagraphTocEntry(paragraph, entryInfo);
        }

        private void AddHiddenParagraphTocEntry(Paragraph paragraph, int level)
        {
            mTocEntries.Add(new HiddenParagraphTocEntry(paragraph, level));
        }

        private void AddParagraphTocEntry(Paragraph paragraph, ParagraphTocEntryInfo entryInfo)
        {
            Node firstNode = (entryInfo.FirstChild != null) || entryInfo.IsEmptyTocEntry
                ? entryInfo.FirstChild
                : paragraph.FirstNonMarkupDescendant;
            Node lastNode = (entryInfo.LastChild != null) || entryInfo.IsEmptyTocEntry
                ? entryInfo.LastChild
                : paragraph.LastNonMarkupDescendant;

            bool isFirstNodeMet = false;
            bool isTocEndMet = false;
            bool isParagraphPartAdded = false;

            if ((firstNode != null) && (lastNode != null))
            {
                NodeRange range = new NodeRange(firstNode, lastNode);
                foreach (Node node in range)
                {
                    if (node == firstNode)
                        isFirstNodeMet = true;

                    if (node == mTocEntryExtractorOptions.End)
                        isTocEndMet = true;

                    if ((node == mTocEntryExtractorOptions.Start) && (node != firstNode) && isFirstNodeMet)
                    {
                        AddParagraphTocEntryCore(paragraph, entryInfo.Level, firstNode, mTocEntryExtractorOptions.Start.PreviousSibling);
                        isParagraphPartAdded = true;
                    }
                    else if ((node == lastNode) && (node != mTocEntryExtractorOptions.End) && isTocEndMet)
                    {
                        AddParagraphTocEntryCore(paragraph, entryInfo.Level, mTocEntryExtractorOptions.End.NextSibling, lastNode);
                        isParagraphPartAdded = true;
                    }
                }
            }

            if (!isParagraphPartAdded)
                AddParagraphTocEntryCore(paragraph, entryInfo);
        }

        private void AddParagraphTocEntryCore(Paragraph paragraph, ParagraphTocEntryInfo entryInfo)
        {
            mTocEntries.Add(new ParagraphTocEntry(paragraph, entryInfo));
        }

        private void AddParagraphTocEntryCore(Paragraph paragraph, int entryLevel, Node firstNode, Node lastNode)
        {
            AddParagraphTocEntryCore(paragraph, new ParagraphTocEntryInfo(entryLevel, firstNode, lastNode));
        }

        /// <summary>
        /// Adds a ParagraphTocEntry for the given paragraph and outline level if outline level is valid.
        /// </summary>
        /// <returns>
        /// A boolean value indicating whether an entry was actually added.
        /// </returns>
        private bool AddParagraphTocEntryFromLevel(Paragraph paragraph, int level)
        {
            if (!LevelRange.IsLevelValid(level))
                return false;

            ParagraphTocEntryInfo tocEntryInfo = GetTocEntryInfoBookmarkRangeAware(paragraph, level);
            if (tocEntryInfo == null)
                return false;

            AddTocEntryForParagraph(paragraph, tocEntryInfo);

            return true;
        }

        private ParagraphTocEntryInfo GetTocEntryInfoBookmarkRangeAware(Paragraph paragraph, int level)
        {
            if (!LevelRange.IsLevelValid(level))
                return null;

            if (mIsBookmarkRangeStarted)
                return new ParagraphTocEntryInfo(level);

            // Actually, range can start deeper in a composite node. There is a CR for that: 4930.
            if (mRangeBookmark == null)
                return null;

            if (!IsBookmarkStartsInTheBeginningOfParagraph(paragraph, mRangeBookmark.BookmarkStart))
                return null;

            return new ParagraphTocEntryInfo(level, mRangeBookmark.BookmarkStart, null);
        }

        private static bool IsBookmarkStartsInTheBeginningOfParagraph(Paragraph paragraph, BookmarkStart bookmarkStart)
        {
            if (!bookmarkStart.IsAncestorNode(paragraph))
                return false;

            Node node = bookmarkStart;
            while (node != paragraph)
            {
                while (node.PreviousSibling != null)
                {
                    node = node.PreviousSibling;
                    if (node is IInline)
                    {
                        Run run = node as Run;
                        if ((run != null) && (run.Text == ControlChar.PageBreak))
                            break;

                        return false;
                    }
                }

                node = node.ParentNode;
            }

            return true;
        }

        /// <summary>
        /// Adds a ParagraphTocEntry the given paragraph if paragraph custom style is in the TOC field properties.
        /// </summary>
        /// <returns>
        /// A boolean value indicating whether an entry was actually added.
        /// </returns>
        private bool AddTocEntryFromCustomStyle(Paragraph paragraph)
        {
            // Check if the paragraph has one of the specified custom styles.
            if (!CanUseCustomStyle())
                return false;

            int level = mTocEntryExtractorOptions.GetLevelForCustomStyle(
                paragraph,
                paragraph.GetParagraphStyle(RevisionsView.Final));

            return AddParagraphTocEntryFromLevel(paragraph, level);
        }

        private bool CanUseCustomStyle()
        {
            return mTocEntryExtractorOptions.AreCustomStylesSpecified;
        }

        /// <summary>
        /// Adds a ParagraphTocEntry for outline level of the given paragraph if paragraph's outline level should go to TOC.
        /// </summary>
        /// <returns>
        /// A boolean value indicating whether an entry was actually added.
        /// </returns>
        private bool AddTocEntryFromOutlineLevel(Paragraph paragraph)
        {
            if (!mTocEntryExtractorOptions.UseParagraphOutlineLevel)
                return false;

            if (mTocEntryExtractorOptions.AreCustomStylesSpecified && !mTocEntryExtractorOptions.IsHeadingLevelRangeSpecified)
                return false;

            int level = GetParagraphLevelFromOutlineLevel(paragraph);
            if (!mTocEntryExtractorOptions.HeadingLevelRange.IsInRange(level))
                return false;

            return AddParagraphTocEntryFromLevel(paragraph, level);
        }

        /// <summary>
        /// Adds ParagraphTocEntries for runs having heading style in the beginning of a paragraph or page/column.
        /// </summary>
        /// <remarks>
        /// There can be multiple TOC entries for a single paragraph.
        /// </remarks>
        private bool AddTocEntriesFromHeadingStyle(Paragraph paragraph)
        {
            if (!CanUseHeadingStyle())
                return false;

            // WORDSNET-19435 Skip if the \\u switch is specified and paragraph outline level is body text.
            if (mTocEntryExtractorOptions.UseParagraphOutlineLevel)
            {
                int outlineLevel = GetParagraphLevelFromOutlineLevel(paragraph);
                if (outlineLevel == LevelRange.InvalidLevel)
                    return false;
            }

            // Get outline level from paragraph.
            int level = GetParagraphLevelFromStyle(paragraph);
            if (!mTocEntryExtractorOptions.HeadingLevelRange.IsInRange(level))
                return false;

            return AddParagraphTocEntryFromLevel(paragraph, level);
        }

        private bool CanUseHeadingStyle()
        {
            // Not included if table of figures.
            if (mTocEntryExtractorOptions.IsTableOfFigures && !mTocEntryExtractorOptions.IsHeadingLevelRangeSpecified)
                return false;

            // Bail out if nothing to do.
            if (mTocEntryExtractorOptions.AreCustomStylesSpecified && !mTocEntryExtractorOptions.IsHeadingLevelRangeSpecified)
                return false;

            return true;
        }

        private void AddTocEntriesFromLinkedStyle(Paragraph paragraph)
        {
            // WORDSNET-17102 If first runs of a paragraph have HeadingN as a linked style,
            // then they are considered as a TOC entry.

            bool resolveHeadingStyle = CanUseHeadingStyle();
            bool resolveCustomStyle = CanUseCustomStyle();

            if (!resolveHeadingStyle && !resolveCustomStyle)
                return;

            if (paragraph.GetAncestor(NodeType.StructuredDocumentTag) != null)
                return;

            bool isLocked = false;
            bool isBookmarkRangeStarted = mIsBookmarkRangeStarted;

            NodeRange range = new NodeRange(paragraph, paragraph);
            foreach (Node currentNode in range)
            {
                switch (currentNode.NodeType)
                {
                    case NodeType.FieldStart:
                        if (((FieldChar)currentNode).FieldType == FieldType.FieldTOC)
                            isLocked = true;
                        break;
                    case NodeType.FieldEnd:
                        if (((FieldChar)currentNode).FieldType == FieldType.FieldTOC)
                            isLocked = false;
                        break;
                    default:
                        break;
                }

                if (isLocked)
                    continue;

                switch (currentNode.NodeType)
                {
                    case NodeType.Run:
                        if (isBookmarkRangeStarted && IsTocEntryStartCandidate((Run)currentNode))
                        {
                            AddTocEntryFromLinkedStyle(
                                    (Run)currentNode,
                                    paragraph,
                                    resolveHeadingStyle,
                                    resolveCustomStyle);

                            return;
                        }

                        break;
                    case NodeType.BookmarkStart:
                        if (!isBookmarkRangeStarted)
                        {
                            isBookmarkRangeStarted = IsBookmarkRangeStart(currentNode) &&
                                                     IsBookmarkStartsInTheBeginningOfParagraph(
                                                         paragraph,
                                                         (BookmarkStart)currentNode);
                        }
                        break;
                    case NodeType.BookmarkEnd:
                        // Stop looking for the new entries after range end.
                        if (IsBookmarkRangeEnd(currentNode))
                            return;

                        break;
                    default:
                        // Do nothing for the other node types.
                        break;
                }
            }
        }

        /// <summary>
        /// Adds a TOC entry created from run(s) based on a linked style.
        /// </summary>
        private void AddTocEntryFromLinkedStyle(
            Run run,
            Paragraph paragraph,
            bool resolveHeadingStyle,
            bool resolveCustomStyle)
        {
            int level = GetParagraphLevelFromLinkedStyle(run, paragraph, resolveHeadingStyle, resolveCustomStyle);
            if (!LevelRange.IsLevelValid(level))
                return;

            Node lastRunOfTocEntry = GetLastRunOfTocEntry(
                run,
                level,
                paragraph,
                resolveHeadingStyle,
                resolveCustomStyle);

            ParagraphTocEntryInfo tocEntryInfo = new ParagraphTocEntryInfo(level, run, lastRunOfTocEntry, true);
            AddTocEntryForParagraph(run.ParentParagraph, tocEntryInfo);
        }

        /// <summary>
        /// Looks for TOC entry start candidate in the given run.
        /// </summary>
        private static bool IsTocEntryStartCandidate(Run run)
        {
            foreach (char c in run.Text)
            {
                // WORDSNET-26724 Skip leading spaces and breaks, with any style.
                if (!IsTextChar(c))
                    continue;

                // WORDSNET-20677 Skip hidden runs.
                if (run.IsHiddenOrDeleted)
                    continue;

                return true;
            }

            return false;
        }

        private static bool IsTextChar(char c)
        {
            switch (c)
            {
                case ControlChar.PageBreakChar:
                case ControlChar.ColumnBreakChar:
                    return false;
                default:
                    return !char.IsWhiteSpace(c);
            }
        }

        /// <summary>
        /// Finds the last sibling of the given run having the given level.
        /// </summary>
        /// <remarks>
        /// The run itself is returned if there is no sibling with the given level.
        /// </remarks>
        private Node GetLastRunOfTocEntry(
            Run firstRunOfTocEntry,
            int level,
            Paragraph paragraph,
            bool resolveHeadingStyle,
            bool resolveCustomStyle)
        {
            if (firstRunOfTocEntry == null)
                return null;

            Run lastRunOfTocEntry = firstRunOfTocEntry;

            // Look for the last consecutive run with the given style.
            NodeRange range = new NodeRange(firstRunOfTocEntry, false, paragraph, true);
            foreach (Node possibleTocEntryNode in range)
            {
                NodeType nodeType = possibleTocEntryNode.NodeType;
                if (nodeType == NodeType.Run)
                {
                    Run possibleTocEntryRun = (Run)possibleTocEntryNode;

                    int linkedStyleLevel = GetParagraphLevelFromLinkedStyle(
                        possibleTocEntryRun,
                        paragraph,
                        resolveHeadingStyle,
                        resolveCustomStyle);

                    if (level == linkedStyleLevel)
                    {
                        lastRunOfTocEntry = possibleTocEntryRun;
                    }
                    else
                    {
                        // Bail out, TOC entry end is found.
                        break;
                    }
                }
                else if (nodeType == NodeType.CommentRangeEnd)
                {
                    // Comment range end breaks the TOC entry.
                    break;
                }
                else
                {
                    // For other node types, just skip them.
                }
            }

            return lastRunOfTocEntry;
        }

        private static int GetParagraphLevelFromStyle(Paragraph paragraph)
        {
            Style style = paragraph.GetParagraphStyle(RevisionsView.Final);

            return GetParagraphLevelFromStyle(style);
        }

        private static int GetParagraphLevelFromStyle(Style style)
        {
            if (style.ParagraphFormat == null)
                return LevelRange.InvalidLevel;

            return GetParagraphLevelFromOutlineLevel(style.ParagraphFormat.OutlineLevel);
        }

        internal static int GetParagraphLevelFromOutlineLevel(Paragraph paragraph)
        {
            // We don't use ParagraphFormat facade because there is no "Final" analog for the ParagraphFormat.OutlineLevel property.
            OutlineLevel outlineLevel = (OutlineLevel)paragraph.FetchParaAttr(ParaAttr.OutlineLevel, RevisionsView.Final);
            return GetParagraphLevelFromOutlineLevel(outlineLevel);
        }

        private static int GetParagraphLevelFromOutlineLevel(OutlineLevel outlineLevel)
        {
            switch (outlineLevel)
            {
                case OutlineLevel.Level1:
                    return 1;
                case OutlineLevel.Level2:
                    return 2;
                case OutlineLevel.Level3:
                    return 3;
                case OutlineLevel.Level4:
                    return 4;
                case OutlineLevel.Level5:
                    return 5;
                case OutlineLevel.Level6:
                    return 6;
                case OutlineLevel.Level7:
                    return 7;
                case OutlineLevel.Level8:
                    return 8;
                case OutlineLevel.Level9:
                    return 9;
                default:
                    return LevelRange.InvalidLevel;
            }
        }

        private int GetParagraphLevelFromLinkedStyle(
            IInline inline,
            Paragraph paragraph,
            bool resolveHeadingStyle,
            bool resolveCustomStyle)
        {
            RunPr runPr = inline.RunPr_IInline;
            StyleCollection styles = inline.Document_IInline.Styles;
            Style style = styles.FetchByIstd(runPr.Istd, StyleIndex.DefaultParagraphFont);
            Style linkedStyle = styles.GetByIstd(style.LinkedIstd, false);

            if (linkedStyle == null)
                return LevelRange.InvalidLevel;

            if (resolveHeadingStyle)
            {
                int level = GetParagraphLevelFromStyle(linkedStyle);
                if (mTocEntryExtractorOptions.HeadingLevelRange.IsInRange(level))
                    return level;
            }

            if (resolveCustomStyle)
                return mTocEntryExtractorOptions.GetLevelForCustomStyle(paragraph, linkedStyle);

            return LevelRange.InvalidLevel;
        }

        private readonly ITocEntryExtractorOptions mTocEntryExtractorOptions;
        private readonly Bookmark mRangeBookmark;
        private readonly IList<ITocEntry> mTocEntries = new List<ITocEntry>();
        private readonly FieldCharCounter mFieldCharCounter = new FieldCharCounter(FieldCharCounterOptions.SafeMode);
        private bool mIsBookmarkRangeStarted;
        private Paragraph mLastParagraph;
        private string mTableOfFiguresLabel;
        private string mCaptionlessTableOfFiguresLabel;

        // The \p{Zs} group must be used to match all Unicode space characters in Java.
#if JAVA
        private const string RegexPattern = "[\\s\\p{Z}]";
#else
        private const string RegexPattern = @"\s";
#endif

        private static readonly Regex gWhiteSpacesRegex = new Regex(RegexPattern, RegexOptions.Compiled);

        private class LazyFieldSeqValueEvaluator : Lazy<FieldSeqValueEvaluator>
        {
            internal LazyFieldSeqValueEvaluator(Document document)
            {
                mDocument = document;
            }

            protected override FieldSeqValueEvaluator InitValue()
            {
                return new FieldSeqValueEvaluator(mDocument, new BookmarkCache(mDocument));
            }

            private readonly Document mDocument;
        }
    }
}
