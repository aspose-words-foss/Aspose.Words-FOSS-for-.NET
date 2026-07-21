// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2017 by Alexander Zhiltsov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Finds comment range nodes by ID.
    /// </summary>
    internal class CommentRangeFinder : DocumentVisitor
    {
        /// <summary>
        /// Searches for comment range start and range end nodes that have the specified comment ID.
        /// Returns <c>true</c> if the both are found.
        /// </summary>
        internal bool Find(Node node, int commentId)
        {
            mCommentId = commentId;

            node.Accept(this);

            if ((mRangeStart != null) != (mRangeEnd != null))
                throw new InvalidOperationException(string.Format("Wrong comment range (ID = {0}).", commentId));

            return mRangeStart != null;
        }

        public override VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            if (commentRangeStart.Id == mCommentId)
            {
                if (mRangeStart != null)
                    throw new InvalidOperationException(string.Format("Wrong comment range (ID = {0}).", mCommentId));

                mRangeStart = commentRangeStart;
                if (mRangeEnd != null)
                    return VisitorAction.Stop;
            }
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            if (commentRangeEnd.Id == mCommentId)
            {
                if (mRangeEnd != null)
                    throw new InvalidOperationException(string.Format("Wrong comment range (ID = {0}).", mCommentId));

                mRangeEnd = commentRangeEnd;
                if (mRangeStart != null)
                    return VisitorAction.Stop;
            }
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Gets found comment range start node.
        /// </summary>
        internal CommentRangeStart RangeStart
        {
            get { return mRangeStart; }
        }

        /// <summary>
        /// Gets found comment range end node.
        /// </summary>
        internal CommentRangeEnd RangeEnd
        {
            get { return mRangeEnd; }
        }

        private int mCommentId;
        private CommentRangeStart mRangeStart;
        private CommentRangeEnd mRangeEnd;
    }
}
