// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/06/2021 by Edward Voronov

using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A TOC entry that is represented by another TOC entry in the referenced document specified in the <see cref="FieldRD"/> field.
    /// </summary>
    internal class RefDocTocEntry : ITocEntry
    {
        internal RefDocTocEntry(ITocEntry entry, Document document, Lazy<FieldSeqValueEvaluator> fieldSeqValueEvaluator)
        {
            Document = document;
            RefTocEntry = entry;
            mFieldSeqValueEvaluator = fieldSeqValueEvaluator;

            mBookmarkName = "_" + RandomUtil.NewGuid().ToString("N");
            mEntryRange = RefTocEntry.InsertBookmark(mBookmarkName);
        }

        /// <summary>
        /// Gets the referenced document.
        /// </summary>
        internal Document Document { get; }

        /// <summary>
        /// Gets the referenced TOC entry.
        /// </summary>
        internal ITocEntry RefTocEntry { get; }

        NodeRange ITocEntry.InsertBookmark(string bookmarkName)
        {
            return mEntryRange;
        }

        int ITocEntry.Level
        {
            get { return RefTocEntry.Level; }
        }

        bool ITocEntry.OmitPageNumber
        {
            get { return RefTocEntry.OmitPageNumber; }
        }

        Paragraph ITocEntry.Paragraph
        {
            get { return null; }
        }

        string ITocEntry.GetDocumentOutlineTitle()
        {
            return null;
        }

        NodeRange ITocEntry.GetLabelRange()
        {
            return RefTocEntry.GetLabelRange();
        }

        bool ITocEntry.IsInFieldCode
        {
            get { return false; }
        }

        bool ITocEntry.HasBookmark
        {
            get { return false; }
        }

        bool ITocEntry.IsLinkedStyleTocEntry
        {
            get { return false; }
        }

        int ITocEntry.GetSequenceValue(string sequenceIdentifier)
        {
            FieldSeqValueEvaluator evaluator = mFieldSeqValueEvaluator.Value;
            evaluator.AddSequenceBookmark(mBookmarkName);
            NullableInt32 value = evaluator.GetBookmarkSequenceValue(mBookmarkName, sequenceIdentifier);
            return value.GetValueOrDefault();
        }

        int ITocEntry.GetPageNumber()
        {
            // FOSS
            return 0;
        }

        private readonly Lazy<FieldSeqValueEvaluator> mFieldSeqValueEvaluator;
        private readonly string mBookmarkName;
        private readonly NodeRange mEntryRange;
    }
}
