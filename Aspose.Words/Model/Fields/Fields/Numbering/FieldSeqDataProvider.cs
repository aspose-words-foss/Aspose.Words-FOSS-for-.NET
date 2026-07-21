// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides values for the SEQ fields.
    /// </summary>
    internal class FieldSeqDataProvider : IFieldUpdateDataProvider
    {
        internal FieldSeqDataProvider(FieldUpdater fieldUpdater)
        {
            mBookmarkCache = fieldUpdater.BookmarkCache;
            mDocument = fieldUpdater.Document;
        }

        internal FieldSeqDataProvider(Document document)
        {
            mBookmarkCache = new BookmarkCache(document);
            mDocument = document;
        }

        object IFieldUpdateDataProvider.GetData(Field field)
        {
            return null;
        }

        Constant IFieldUpdateDataProvider.GetValue(Field field)
        {
            switch (field.Type)
            {
                case FieldType.FieldSequence:
                    EnsureEvaluator();
                    return mValueEvaluator.GetValue((FieldSeq)field);
                default:
                    return null;
            }
        }

        internal void AddSequenceBookmark(string bookmarkName)
        {
            if (mValueEvaluator == null)
                return;

            mValueEvaluator.AddSequenceBookmark(bookmarkName);
        }

        internal void Invalidate()
        {
            mValueEvaluator = null;
        }

        /// <summary>
        /// Initializes <see cref="mValueEvaluator"/> member variable if it has not been initialized yet.
        /// </summary>
        private void EnsureEvaluator()
        {
            // Calculate SEQ fields' values on the first demand as it is too expensive to do it in ctor
            // since it is called for every SEQ field entry's update.
            if (mValueEvaluator != null)
                return;

            mValueEvaluator = new FieldSeqValueEvaluator(mDocument, mBookmarkCache);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly BookmarkCache mBookmarkCache;
        private readonly Document mDocument;
        private FieldSeqValueEvaluator mValueEvaluator;
    }
}
