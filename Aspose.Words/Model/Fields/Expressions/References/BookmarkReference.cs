// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a reference to a bookmark.
    /// </summary>
    internal class BookmarkReference : IExecutionItem
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="context">The field context object.</param>
        /// <param name="bookmarkName">The name of a bookmark to refer to.</param>
        internal BookmarkReference(FieldContext context, string bookmarkName)
        {
            mContext = context;
            mBookmarkName = bookmarkName;
        }

        Constant IExecutionItem.Evaluate(ConstantStack calculationStack)
        {
            if (Bookmark == null)
                return ErrorConstant.CreateBookmarkError(mBookmarkName);

            string bookmarkText = Bookmark.GetText(true);

            DoubleConstant result = ExpressionEvaluator.EvaluateReferenceExpression(mContext, bookmarkText);
            if (result != null)
                return result;

            if (mContext.Field.Type == FieldType.FieldFormula)
                return new DoubleConstant(0);

            return new StringConstant(bookmarkText);
        }

        private Bookmark Bookmark
        {
            // SPEED Get a bookmark from a cache.
            get { return FieldUtil.GetCachedBookmark(mContext.Field, mBookmarkName); }
        }

        private readonly FieldContext mContext;
        private readonly string mBookmarkName;
    }
}
