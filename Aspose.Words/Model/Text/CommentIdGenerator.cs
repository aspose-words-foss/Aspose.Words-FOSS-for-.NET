// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2017 by Alexander Zhiltsov

namespace Aspose.Words
{
    /// <summary>
    /// Class to generate unique ID for a comment.
    /// </summary>
    internal class CommentIdGenerator : DocumentVisitor
    {
        /// <summary>
        /// Cannot create instance of this class.
        /// </summary>
        private CommentIdGenerator()
        {
        }

        /// <summary>
        /// Generates unique ID that can be assigned to a comment.
        /// </summary>
        internal static int Generate(DocumentBase document)
        {
            CommentIdGenerator generator = new CommentIdGenerator();

            document.Accept(generator);

            return generator.mMaxId + 1;
        }

        public override VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            if (commentRangeStart.Id > mMaxId)
                mMaxId = commentRangeStart.Id;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentStart(Comment comment)
        {
            if (comment.Id > mMaxId)
                mMaxId = comment.Id;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            if (commentRangeEnd.Id > mMaxId)
                mMaxId = commentRangeEnd.Id;
            return VisitorAction.Continue;
        }

        private int mMaxId = -1;
    }
}
