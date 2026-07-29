// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/04/2013 by Ivan Lyagin

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Collections;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents an item in an index entry/subentry tree.
    /// </summary>
    /// <remarks>
    /// An index entry/subentry tree is represented by a single index entry called a root entry.
    /// This is an entry which does not correspond to any XE field entry. Instead, it contains entries of the min
    /// level (corresponding to main entry texts of XE fields, for instance to "main" in { XE "main:sub1:sub2" })
    /// which in turn contain subentries of the next level (corresponding to the nearest subentry texts of XE fields,
    /// for instance to "sub1" in { XE "main:sub1:sub2" }) and so on.
    /// </remarks>
    internal class IndexEntry
    {
        /// <summary>
        /// A factory method to create a root entry for the whole document.
        /// See <see cref="IndexEntry"/> remarks for any details.
        /// </summary>
        internal static IndexEntry GetRootEntry(FieldCodeIndex fieldCodeIndex, FieldSeqDataProvider fieldSeqDataProvider, DocumentBase document)
        {
            IndexEntry rootEntry = new IndexEntry();
            EntryExtractor extractor = new EntryExtractor(fieldCodeIndex, rootEntry, true, document, fieldSeqDataProvider);
            extractor.Extract(document);
            return rootEntry;
        }

        /// <summary>
        /// A factory method to create a root entry for the specified node range of a document.
        /// See <see cref="IndexEntry"/> remarks for any details.
        /// </summary>
        internal static IndexEntry GetRootEntry(FieldCodeIndex fieldCodeIndex, FieldSeqDataProvider fieldSeqDataProvider, NodeRange range)
        {
            IndexEntry rootEntry = new IndexEntry();
            EntryExtractor extractor = new EntryExtractor(fieldCodeIndex, rootEntry, true, range, fieldSeqDataProvider);
            extractor.Extract(range);
            return rootEntry;
        }

        /// <summary>
        /// Ctor to create a root entry.
        /// </summary>
        private IndexEntry()
            : this(null, null)
        {
        }

        private IndexEntry(FieldCodeXE refFieldCode, NodeRange nodeRange)
        {
            RefFieldCode = refFieldCode;
            NodeRange = nodeRange;
        }

        /// <summary>
        /// Calculates and adds an <see cref="IndexEntryPageNumberInfo"/> object for the specified XE field.
        /// </summary>
        private void AddPageNumberInfo(
            FieldXE field,
            FieldCodeXE fieldCode,
            FieldSeq lastFieldSeq,
            BookmarkSequencesCache bookmarkSequencesCache,
            FieldSeqDataProvider fieldSeqDataProvider)
        {
            // Create on the first demand.
            if (!HasPageNumberInfos)
                mPageNumberInfos = new List<IndexEntryPageNumberInfo>();

            IndexEntryPageNumberInfo page = BuildPageNumberInfo(field, fieldCode, lastFieldSeq, bookmarkSequencesCache, fieldSeqDataProvider);
            mPageNumberInfos.Add(page);

            if (fieldCode.HasPageNumberReplacement)
                HasPageNumberReplacements = true;
        }

        private static IndexEntryPageNumberInfo BuildPageNumberInfo(
            FieldXE field,
            FieldCodeXE fieldCode,
            FieldSeq lastFieldSeq,
            BookmarkSequencesCache bookmarkSequencesCache,
            FieldSeqDataProvider fieldSeqDataProvider)
        {
            // FOSS: page numbers cannot be calculated without the layout engine. Build the page number info
            // using the neutral page number 0 (see ToaEntryExtractor.GetPageNumber and RefDocTocEntry.GetPageNumber),
            // so that the INDEX field still updates and produces its entries.
            return new IndexEntryPageNumberInfo(
                0,
                IndexEntryPageNumberInfo.InvalidPageNumber,
                lastFieldSeq,
                null,
                fieldCode,
                field,
                fieldSeqDataProvider);
        }

        /// <summary>
        /// Gets a subentry of this entry with the specified text. If not found, creates a new one, stores it
        /// for a further usage and returns it.
        /// </summary>
        private IndexEntry GetOrCreateSubentry(string subentryText, FieldCodeXE refFieldCode, NodeRange range)
        {
            IndexEntry subentry = null;
            if (HasSubentries)
            {
                // Get an existing subentry.
                subentry = mSubentries.GetValueOrNull(subentryText);
            }
            else
            {
                // Create on the first demand.
                mSubentries = new SortedList<string, IndexEntry>(IndexEntryTextComparer.Instance);
            }

            if (subentry == null)
            {
                // Subentry is not found. Create a new one and remember it.
                subentry = new IndexEntry(refFieldCode, range);
                mSubentries[subentryText] = subentry;
            }

            return subentry;
        }

        /// <summary>
        /// Gets a subentry text for a subentry with the specified index.
        /// </summary>
        /// <param name="index">A zero-based index. It should be less than <see cref="SubentryCount"/>.</param>
        internal string GetSubentryText(int index)
        {
            return mSubentries.Keys[index];
        }

        /// <summary>
        /// Gets a subentry with the specified index.
        /// </summary>
        /// <param name="index">A zero-based index. It should be less than <see cref="SubentryCount"/>.</param>
        internal IndexEntry GetSubentry(int index)
        {
            return mSubentries.Values[index];
        }

        /// <summary>
        /// Gets the count of subentries contained within this entry.
        /// </summary>
        internal int SubentryCount
        {
            get { return HasSubentries ? mSubentries.Count : 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this entry contains any subentries.
        /// </summary>
        internal bool HasSubentries
        {
            get { return mSubentries != null; }
        }

        /// <summary>
        /// Gets a <see cref="FieldCodeXE"/> object that describes the entry.
        /// </summary>
        /// <remarks>
        /// It corresponds to the first XE field referencing the entry.
        /// </remarks>
        internal FieldCodeXE RefFieldCode { get; }

        /// <summary>
        /// Gets the entry's node range.
        /// </summary>
        internal NodeRange NodeRange { get; }

        /// <summary>
        /// Gets the collection of <see cref="IndexEntryPageNumberInfo"/> objects describing page numbers
        /// directly referencing the entry.
        /// </summary>
        /// <remarks>
        /// Let's consider { XE "main:sub" } for instance. An entry corresponding to the "sub" part is
        /// directly referenced by this XE field while an entry corresponding to the "main" part is not.
        /// </remarks>
        internal IList<IndexEntryPageNumberInfo> PageNumberInfos
        {
            get { return mPageNumberInfos; }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PageNumberInfos"/> collection contains any items.
        /// </summary>
        internal bool HasPageNumberInfos
        {
            get { return mPageNumberInfos != null; }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="PageNumberInfos"/> collection contains any items
        /// which correspond to page number replacements.
        /// </summary>
        /// <remarks>
        /// See also <see cref="FieldXE.PageNumberReplacement"/>.
        /// </remarks>
        internal bool HasPageNumberReplacements { get; private set; }

        private SortedList<string, IndexEntry> mSubentries;
        private List<IndexEntryPageNumberInfo> mPageNumberInfos;

        /// <summary>
        /// The character separating index subentries of different levels.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char SubentrySeparator = ':';
        /// <summary>
        /// The min subentry level, i.e. the level of a main entry text of XE field.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MinSubentryLevel = 1;
        /// <summary>
        /// The max subentry level which is allowed by MS Word.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxSubentryLevel = 7;

        /// <summary>
        /// Fills a root entry by extracting of XE fields contained within a document or its node range.
        /// </summary>
        private class EntryExtractor : FieldExtractor
        {
            internal EntryExtractor(
                FieldCodeIndex fieldCodeIndex,
                IndexEntry rootEntry,
                bool includeRefDocFields,
                Node node,
                FieldSeqDataProvider fieldSeqDataProvider)
                : this(
                    fieldCodeIndex,
                    rootEntry,
                    includeRefDocFields,
                    BookmarkSequencesCache.Build(node, fieldCodeIndex.SequenceName),
                    fieldSeqDataProvider)
            {
            }

            internal EntryExtractor(
                FieldCodeIndex fieldCodeIndex,
                IndexEntry rootEntry,
                bool includeRefDocFields,
                NodeRange range,
                FieldSeqDataProvider fieldSeqDataProvider)
                : this(
                    fieldCodeIndex,
                    rootEntry,
                    includeRefDocFields,
                    BookmarkSequencesCache.Build(range, fieldCodeIndex.SequenceName),
                    fieldSeqDataProvider)
            {
            }

            private EntryExtractor(
                FieldCodeIndex fieldCodeIndex,
                IndexEntry rootEntry,
                bool includeRefDocFields,
                BookmarkSequencesCache bookmarkSequencesCache,
                FieldSeqDataProvider fieldSeqDataProvider)
            {
                mFieldCodeIndex = fieldCodeIndex;
                mRootEntry = rootEntry;
                mBookmarkSequencesCache = bookmarkSequencesCache;
                mIncludeRefDocFields = includeRefDocFields;
                mFieldSeqDataProvider = fieldSeqDataProvider;

                Debug.Assert(mFieldCodeIndex.HasSequenceName == (mFieldSeqDataProvider != null));
            }

            protected override void OnFieldExtracted()
            {
                switch (CurrentFieldType)
                {
                    case FieldType.FieldIndexEntry:
                        ProcessFieldXE((FieldXE)CurrentField);
                        break;
                    case FieldType.FieldSequence:
                        ProcessFieldSequence((FieldSeq)CurrentField);
                        break;
                    case FieldType.FieldRefDoc:
                        ProcessFieldRD((FieldRD)CurrentField);
                        break;
                    default:
                        // Do nothing
                        break;
                }
            }

            private void ProcessFieldXE(FieldXE field)
            {
                FieldCodeXE fieldCodeXE = new FieldCodeXE(field);
                if (IsFieldXEMatch(field, fieldCodeXE))
                    ProcessFieldXE(field, fieldCodeXE);
            }

            private void ProcessFieldSequence(FieldSeq field)
            {
                if (!mFieldCodeIndex.HasSequenceName)
                    return;

                if (mFieldCodeIndex.SequenceName != field.SequenceIdentifier)
                    return;

                mLastFieldSeqFound = field;
            }

            /// <summary>
            /// Returns a value indicating whether the specified XE field instance satisfies conditions of the INDEX
            /// field in the context of which the extraction is performed.
            /// </summary>
            private bool IsFieldXEMatch(FieldXE fieldXE, FieldCodeXE fieldCode)
            {
                if (mFieldCodeIndex.EntryType != fieldCode.EntryTypeCore)
                    return false;

                if (!fieldXE.ShouldBeProcessed(fieldCode))
                    return false;

                if (mFieldCodeIndex.HasLetterRange)
                {
                    // If the entry is accessed then it has a non-whitespace text.
                    int firstLetterIndex = StringUtil.IndexOfNonWhitespace(fieldCode.Text);
                    char firstLetter = char.ToUpperInvariant(fieldCode.Text[firstLetterIndex]);

                    if (firstLetter < mFieldCodeIndex.LetterRange[0])
                        return false;

                    if (firstLetter > mFieldCodeIndex.LetterRange[1])
                        return false;
                }

                return true;
            }

            /// <summary>
            /// Extracts index entries and subentries from the given XE field entry.
            /// </summary>
            /// <remarks>
            /// It is assumed that the method is called only if the specified XE field entry satisfies conditions of the INDEX
            /// field in the context of which the extraction is performed.
            /// </remarks>
            private void ProcessFieldXE(FieldXE field, FieldCodeXE fieldCode)
            {
                mCurrentSubentry = mRootEntry;
                mCurrentSubentryLevel = MinSubentryLevel;

                if (fieldCode.Text.IndexOf(SubentrySeparator) == -1)
                {
                    // Use a simplified approach when there is no subentry separators for sure.
                    MoveToSubentry(fieldCode.Text, fieldCode, fieldCode.TextRange);
                }
                else
                {
                    // Use a full evaluation process when there are suspicious characters
                    // which may (or may not) split an entry to subentries.
                    SubentryExtractHelper helper = new SubentryExtractHelper(this, fieldCode);
                    using (NodeModifierApplier applier = new NodeModifierApplier(fieldCode.TextRange, helper))
                        applier.ApplyModifier();
                    helper.FinalizeExtract();
                }

                // Add a page number info to the directly referenced subentry. There is no sense to do that for a root entry.
                if (mCurrentSubentry != mRootEntry)
                    mCurrentSubentry.AddPageNumberInfo(field, fieldCode, mLastFieldSeqFound, mBookmarkSequencesCache, mFieldSeqDataProvider);
            }

            /// <summary>
            /// Advances from the current subentry to a subentry of the next level with the specified subentry text.
            /// Creates the subentry if it does not exist.
            /// </summary>
            private void MoveToSubentry(string subentryText, FieldCodeXE refFieldCode, NodeRange range)
            {
                // MS Word does not process subentries with level more than MaxSubentryLevel.
                if (mCurrentSubentryLevel > MaxSubentryLevel)
                    return;

                mCurrentSubentry = mCurrentSubentry.GetOrCreateSubentry(subentryText, refFieldCode, range);
                mCurrentSubentryLevel++;
            }

            private void ProcessFieldRD(FieldRD field)
            {
                if (!mIncludeRefDocFields)
                    return;

                Document referenceDocument = FieldIndexAndTablesUtil.OpenRefDocument(field);
                if (referenceDocument == null)
                    return;

                FieldSeqDataProvider fieldSeqDataProvider = mFieldCodeIndex.HasSequenceName
                    ? new FieldSeqDataProvider(referenceDocument)
                    : null;
                EntryExtractor extractor = new EntryExtractor(mFieldCodeIndex, mRootEntry, false, referenceDocument, fieldSeqDataProvider);
                extractor.Extract(referenceDocument);
            }

            private readonly FieldCodeIndex mFieldCodeIndex;
            private readonly IndexEntry mRootEntry;
            private readonly BookmarkSequencesCache mBookmarkSequencesCache;
            private IndexEntry mCurrentSubentry;
            private int mCurrentSubentryLevel;
            private FieldSeq mLastFieldSeqFound;
            private readonly bool mIncludeRefDocFields;
            private readonly FieldSeqDataProvider mFieldSeqDataProvider;

            /// <summary>
            /// Helps to extract entries and subentries from a XE field text argument.
            /// </summary>
            private class SubentryExtractHelper : FieldXETextArgumentDecoderNodeModifier
            {
                /// <summary>
                /// Ctor.
                /// </summary>
                internal SubentryExtractHelper(EntryExtractor entryExtractor, FieldCodeXE fieldCode)
                    : base(fieldCode.TextRange)
                {
                    mEntryExtractor = entryExtractor;
                    mFieldCode = fieldCode;
                }

                protected override bool PrepareModify(Node referenceNode, Node nodeToModify)
                {
                    bool result = base.PrepareModify(referenceNode, nodeToModify);

                    if (result && !IsInFieldCode && (referenceNode.NodeType == NodeType.Run))
                    {
                        // Remember the reference run to be processed.
                        mCurrentRun = referenceNode;
                    }

                    return result;
                }

                protected override StringBuilder ProcessDecodedTokenChar(StringBuilder builder, char c, int positionInTokenPart)
                {
                    // Do not process a nested field's code.
                    if (IsInFieldCode)
                    {
                        // Simply return null not to initialize any extra text buffers.
                        return null;
                    }

                    if ((c == SubentrySeparator) && !IsTokenCharEscaped)
                    {
                        // Process a subentry separator.
                        DocumentPosition subentryStartPosition = GetSubentryStartPosition();

                        // Calculate the offset within the current reference run. Note, that positionInToken relates
                        // to the current run to modify which can be cut comparing to the current reference run.
                        int offset = positionInTokenPart;
                        if (mCurrentRun == mFieldCode.TextRange.Start.Node)
                            offset += mFieldCode.TextRange.Start.Offset;

                        // Note, that it is normal for DocumentPosition.Offset to be more than DocumentPosition.Length.
                        mNextSubentryStartPosition = new DocumentPosition(mCurrentRun, offset + 1);

                        // An end position should point to the next character (i.e. subentry separator).
                        DocumentPosition subentryEndPosition = new DocumentPosition(mCurrentRun, offset);

                        ExtractSubentry(subentryStartPosition, subentryEndPosition);
                    }
                    else
                    {
                        // Append the current character to the current subentry text buffer.
                        mSubentryTextBuilder.Append(c);
                    }

                    // Simply return null not to initialize any extra text buffers.
                    return null;
                }

                protected override RichStringBuilder ProcessDecodedTokenChar(RichStringBuilder builder, RichChar c, int positionInTokenPart)
                {
                    Debug.Fail("Rich strings are not supported");

                    return base.ProcessDecodedTokenChar(builder, c, positionInTokenPart);
                }

                /// <summary>
                /// Returns a document position at which the current subentry starts.
                /// </summary>
                private DocumentPosition GetSubentryStartPosition()
                {
                    return (mNextSubentryStartPosition != null)
                        ? mNextSubentryStartPosition
                        : mFieldCode.TextRange.Start.Clone();
                }

                /// <summary>
                /// Extracts the current subentry.
                /// </summary>
                private void ExtractSubentry(DocumentPosition subentryStartPosition, DocumentPosition subentryEndPosition)
                {
                    // Flush the current subentry text.
                    string subentryText = mSubentryTextBuilder.ToString();
                    mSubentryTextBuilder.Length = 0;

                    // Create a subentry node range.
                    NodeRange subentryRange = new NodeRange(subentryStartPosition, subentryEndPosition);

                    mEntryExtractor.MoveToSubentry(subentryText, mFieldCode, subentryRange);
                }

                /// <summary>
                /// Extracts the last subentry.
                /// </summary>
                internal void FinalizeExtract()
                {
                    DocumentPosition subentryStartPosition = GetSubentryStartPosition();
                    DocumentPosition subentryEndPosition = mFieldCode.TextRange.End.Clone();

                    ExtractSubentry(subentryStartPosition, subentryEndPosition);
                }

                private readonly EntryExtractor mEntryExtractor;
                private readonly FieldCodeXE mFieldCode;
                private readonly StringBuilder mSubentryTextBuilder = new StringBuilder();
                private Node mCurrentRun;
                private DocumentPosition mNextSubentryStartPosition;
            }
        }

        private class IndexEntryTextComparer : IComparer<string>
        {
            private IndexEntryTextComparer()
            {
            }

            public int Compare(string x, string y)
            {
                if (ReferenceEquals(x, y))
                    return 0;

#pragma warning disable CA1309 // By design
                int compareResult = string.Compare(x, y, StringComparison.CurrentCulture);
#pragma warning restore CA1309
                if (compareResult != 0)
                    return compareResult;

                return StringOrdinalComparer.Default.Compare(x, y);
            }

            internal static readonly IndexEntryTextComparer Instance = new IndexEntryTextComparer();
        }

        private class BookmarkSequencesCache : FieldExtractor
        {
            private BookmarkSequencesCache(string sequenceName)
            {
                mSequenceName = sequenceName;
            }

            internal static BookmarkSequencesCache Build(Node node, string sequenceName)
            {
                if (sequenceName == null)
                    return null;

                BookmarkSequencesCache cache = new BookmarkSequencesCache(sequenceName);
                cache.Extract(node);
                return cache;
            }

            internal static BookmarkSequencesCache Build(NodeRange range, string sequenceName)
            {
                if (sequenceName == null)
                    return null;

                BookmarkSequencesCache cache = new BookmarkSequencesCache(sequenceName);
                cache.Extract(range);
                return cache;
            }

            public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
            {
                mStartCache[bookmarkStart.Name] = mLastFieldSeqFound;
                return base.VisitBookmarkStart(bookmarkStart);
            }

            public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
            {
                mEndCache[bookmarkEnd.Name] = mLastFieldSeqFound;
                return base.VisitBookmarkEnd(bookmarkEnd);
            }

            protected override void OnFieldExtracted()
            {
                if (CurrentFieldType != FieldType.FieldSequence)
                    return;

                FieldSeq fieldSeq = (FieldSeq)CurrentField;
                if (fieldSeq.SequenceIdentifier != mSequenceName)
                    return;

                mLastFieldSeqFound = fieldSeq;
            }

            internal FieldSeq GetCachedStartSequence(string bookmarkName)
            {
                return mStartCache[bookmarkName];
            }

            internal FieldSeq GetCachedEndSequence(string bookmarkName)
            {
                return mEndCache[bookmarkName];
            }

            private readonly StringToObjDictionary<FieldSeq> mStartCache = new StringToObjDictionary<FieldSeq>(false);
            private readonly StringToObjDictionary<FieldSeq> mEndCache = new StringToObjDictionary<FieldSeq>(false);
            private readonly string mSequenceName;

            private FieldSeq mLastFieldSeqFound;
        }
    }
}
