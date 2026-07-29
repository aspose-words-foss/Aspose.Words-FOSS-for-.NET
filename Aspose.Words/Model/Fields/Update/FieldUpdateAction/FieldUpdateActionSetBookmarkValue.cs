// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2012 by Ivan Lyagin

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Sets the bookmark value contained in field result.
    /// </summary>
    internal class FieldUpdateActionSetBookmarkValue : FieldUpdateAction
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="bookmarkName"></param>
        /// <param name="bookmarkValue"></param>
        internal FieldUpdateActionSetBookmarkValue(Field field, string bookmarkName, string bookmarkValue)
            : this(
                field,
                bookmarkName,
                (bookmarkValue == null) ? null : new FieldResult(new StringConstant(bookmarkValue)))
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="bookmarkName"></param>
        /// <param name="bookmarkValue"></param>
        internal FieldUpdateActionSetBookmarkValue(Field field, string bookmarkName, IFieldResult bookmarkValue)
            : base(field)
        {
            mBookmarkName = bookmarkName;
            mBookmarkValue = bookmarkValue;
        }

        internal override void Perform()
        {
            if (!StringUtil.HasChars(mBookmarkName) || (mBookmarkValue == null))
                return;

            // SPEED Get a bookmark from a cache.
            Bookmark bookmark = FieldUtil.GetCachedBookmark(Field, mBookmarkName);
            if (bookmark != null)
            {
                // Remove old bookmark value.
                bookmark.Remove();
            }

            NodeRange resultRange = mBookmarkValue.GetFieldResultRange();
            if (resultRange == null)
            {
                // If result range is not defined, use text representation of the result.
                string resultString = mBookmarkValue.GetFieldResultValue().ValueString;

                // We use TextResultApplier in this case to handle multi-paragraph and RTL text properly.
                // Note that old result removal and separator insurance are performed inside TextResultApplier.ApplyResult().
                TextResultApplier resultApplier = new TextResultApplier(Field, resultString, false);
                resultApplier.ApplyResult();
            }
            else
            {
                // Remove old field result.
                Field.RemoveFieldResult();

                // Forcibly insert field separator here.
                Field.EnsureSeparator(true);

                // Copy result range to field result.
                // Note that we do not use NodeRangeResultApplier here as bookmark values contain all child field nodes
                // in contrast to field results, which contain child field results only.
                NodeCopier.Copy(resultRange, Field.End, new FieldTokenDecoderNodeModifier(resultRange), null,
                    NodeCopierOptions.UseSourceStartAncestorPr | NodeCopierOptions.SkipCrossStructureAnnotations |
                        NodeCopierOptions.CloneNode);
            }

            // Add bookmark marks to field result.
            Field.Separator.InsertNext(new BookmarkStart(Document, mBookmarkName));
            Field.End.InsertPrevious(new BookmarkEnd(Document, mBookmarkName));
        }

        private readonly string mBookmarkName;
        private readonly IFieldResult mBookmarkValue;
    }
}
