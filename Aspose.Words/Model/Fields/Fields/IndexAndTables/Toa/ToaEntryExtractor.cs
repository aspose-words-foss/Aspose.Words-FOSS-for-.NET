// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2017 by Edward Voronov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    internal class ToaEntryExtractor : FieldExtractor
    {
        private ToaEntryExtractor(Document document, string entryCategory, Bookmark bookmark, string sequenceName, bool includeRefDocFields, FieldSeqDataProvider seqDataProvider)
        {
            mDocument = document;
            mEntryCategory = entryCategory;
            mBookmark = bookmark;
            mSequenceName = sequenceName;
            mSeqDataProvider = seqDataProvider;
            mIsBookmarkRangeStarted = mBookmark == null;
            mIncludeRefDocFields = includeRefDocFields;
            mBookmarkSequenceNumbersCache = BookmarkSequenceNumbersCache.Build(document, mSequenceName, mSeqDataProvider);
        }

        internal static IList<ToaEntry> ExtractToaEntries(Document document, string entryCategory, Bookmark bookmark, string sequenceName, FieldSeqDataProvider seqDataProvider)
        {
            return ExtractToaEntries(
                document,
                entryCategory,
                bookmark,
                sequenceName,
                true,
                seqDataProvider).Values;
        }

        private static SortedList<string, ToaEntry> ExtractToaEntries(Document document,
            string entryCategory,
            Bookmark bookmark,
            string sequenceName,
            bool includeRefDocFields,
            FieldSeqDataProvider seqDataProvider)
        {
            ToaEntryExtractor extractor = new ToaEntryExtractor(document,
                entryCategory,
                bookmark,
                sequenceName,
                includeRefDocFields,
                seqDataProvider);
            extractor.Extract(document);
            return extractor.mEntries;
        }

        public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            mIsBookmarkRangeStarted = mIsBookmarkRangeStarted || IsBookmarkRangeStart(bookmarkStart);

            return VisitorAction.Continue;
        }

        private bool IsBookmarkRangeStart(Node node)
        {
            return (mBookmark != null) && (node == mBookmark.BookmarkStart);
        }

        public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            return IsBookmarkRangeEnd(bookmarkEnd)
                ? VisitorAction.Stop
                : VisitorAction.Continue;
        }

        private bool IsBookmarkRangeEnd(Node node)
        {
            return (mBookmark != null) && (node == mBookmark.BookmarkEnd);
        }

        protected override void OnFieldExtracted()
        {
            switch (CurrentFieldType)
            {
                case FieldType.FieldTOAEntry:
                    ProcessFieldTA((FieldTA)CurrentField);
                    break;
                case FieldType.FieldSequence:
                    ProcessFieldSequence((FieldSeq)CurrentField);
                    break;
                case FieldType.FieldRefDoc:
                    ProcessFieldRD((FieldRD)CurrentField);
                    break;
                default:
                    break;
            }
        }

        private void ProcessFieldTA(FieldTA field)
        {
            if (!mIsBookmarkRangeStarted)
                return;

            string entryTitle = field.LongCitation;
            if (string.IsNullOrEmpty(entryTitle))
                return;

            if (field.EntryCategory != mEntryCategory)
                return;

            ToaEntry entry = GetOrCreateToaEntry(entryTitle, field);

            AddEntryPage(entry, field);
        }

        private ToaEntry GetOrCreateToaEntry(string entryTitle, FieldTA field)
        {
            ToaEntry entry = mEntries.GetValueOrNull(entryTitle);

            if (entry == null)
            {
                entry = new ToaEntry(field.LongCitationRange);
                mEntries[entryTitle] = entry;
            }

            return entry;
        }

        private void AddEntryPage(ToaEntry entry, FieldTA field)
        {
            bool isBold = field.IsBold;
            bool isItalic = field.IsItalic;

            if (field.HasPageRangeBookmarkNameSwitch)
            {
                // FOSS
                {
                    entry.AddPageRangeError(isBold, isItalic, GetPageNumber(field), GetSequenceNumber());
                }
            }
            else
            {
                entry.AddSinglePage(isBold, isItalic, GetPageNumber(field), GetSequenceNumber());
            }
        }

        private int GetSequenceNumber()
        {
            return GetSequenceNumber(mSequenceName, mLastFieldSeqFound, mSeqDataProvider);
        }

        private static int GetSequenceNumber(string sequenceName, FieldSeq lastFieldSeqFound, IFieldUpdateDataProvider seqDataProvider)
        {
            Debug.Assert((sequenceName == null) == (seqDataProvider == null));

            if (sequenceName == null)
                return ToaEntry.NullSequenceNumber;

            if (lastFieldSeqFound == null)
                return 0;

            Constant constant = seqDataProvider.GetValue(lastFieldSeqFound);
            if (constant == null)
                return 0;

            return (int)constant.ValueDouble;
        }

        private int GetSequenceNumber(string bookmarkName)
        {
            if (mBookmarkSequenceNumbersCache == null)
                return ToaEntry.NullSequenceNumber;

            return mBookmarkSequenceNumbersCache.GetCachedSequenceNumber(bookmarkName);
        }

        private int GetPageNumber(FieldTA field)
        {
            // FOSS
            return 0;
        }

        private void ProcessFieldSequence(FieldSeq field)
        {
            if (mSequenceName == null)
                return;

            if (field.SequenceIdentifier != mSequenceName)
                return;

            mLastFieldSeqFound = field;
        }

        private void ProcessFieldRD(FieldRD field)
        {
            if (!mIncludeRefDocFields)
                return;

            Document referenceDocument = FieldIndexAndTablesUtil.OpenRefDocument(field);
            if (referenceDocument == null)
                return;

            FieldSeqDataProvider seqDataProvider = mSeqDataProvider != null
                ? new FieldSeqDataProvider(referenceDocument)
                : null;
            SortedList<string, ToaEntry> entries = ExtractToaEntries(referenceDocument, mEntryCategory, null, mSequenceName, false, seqDataProvider);

            if (entries.Count == 0)
                return;

            foreach (KeyValuePair<string, ToaEntry> pair in entries)
            {
                ToaEntry entry = mEntries.GetValueOrNull(pair.Key);

                if (entry == null)
                    mEntries[pair.Key] = pair.Value;
                else
                    entry.CopyFrom(pair.Value);
            }
        }

        private readonly SortedList<string, ToaEntry> mEntries = new SortedList<string, ToaEntry>();
        private readonly Document mDocument;
        private readonly string mEntryCategory;
        private readonly Bookmark mBookmark;
        private readonly string mSequenceName;
        private readonly FieldSeqDataProvider mSeqDataProvider;
        private readonly BookmarkSequenceNumbersCache mBookmarkSequenceNumbersCache;
        private readonly bool mIncludeRefDocFields;

        private bool mIsBookmarkRangeStarted;
        private FieldSeq mLastFieldSeqFound;

        private class BookmarkSequenceNumbersCache : FieldExtractor
        {
            private BookmarkSequenceNumbersCache(string sequenceName, FieldSeqDataProvider seqDataProvider)
            {
                mSequenceName = sequenceName;
                mSeqDataProvider = seqDataProvider;
            }

            internal static BookmarkSequenceNumbersCache Build(Document document, string sequenceName, FieldSeqDataProvider seqDataProvider)
            {
                if (sequenceName == null)
                    return null;

                BookmarkSequenceNumbersCache cache = new BookmarkSequenceNumbersCache(sequenceName, seqDataProvider);
                cache.Extract(document);
                return cache;
            }

            public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
            {
                int sequenceNumber = GetSequenceNumber(mSequenceName, mLastFieldSeqFound, mSeqDataProvider);
                mCache[bookmarkStart.Name] = sequenceNumber;
                return base.VisitBookmarkStart(bookmarkStart);
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

            internal int GetCachedSequenceNumber(string bookmarkName)
            {
                int sequenceNumber = mCache[bookmarkName];
                return StringToIntDictionary.IsNullSubstitute(sequenceNumber)
                    ? 0
                    : sequenceNumber;
            }

            private readonly StringToIntDictionary mCache = new StringToIntDictionary(false);
            private readonly string mSequenceName;
            private readonly FieldSeqDataProvider mSeqDataProvider;

            private FieldSeq mLastFieldSeqFound;
        }
    }
}
