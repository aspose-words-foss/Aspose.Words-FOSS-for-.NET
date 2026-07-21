// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2019 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Removes the underlying node range but skips the nodes of the specified bookmark. Bookmark start and end nodes are preserved.
    /// </summary>
    internal class BookmarkExclusiveNodeRemover : NodeRemover
    {
        internal BookmarkExclusiveNodeRemover(NodeRange range, string bookmarkName) : base(range, NodeJoinMode.JoinToNextSibling, false)
        {
            mBookmarkName = bookmarkName;
        }

        [JavaAttributes.JavaThrows(false)]
        protected override void OnNonCompositeNode()
        {
            if (IsBookmarkStart)
            {
                LockRemove();
                base.OnNonCompositeNode();
                HasBookmark = true;
            }
            else if (IsBookmarkEnd)
            {
                base.OnNonCompositeNode();
                UnlockRemove();
            }
            else
            {
                base.OnNonCompositeNode();
            }
        }

        private bool IsBookmarkStart
        {
            get
            {
                return ((CurrentNode.NodeType == NodeType.BookmarkStart) && (((BookmarkStart)CurrentNode).Name == mBookmarkName));
            }
        }

        private bool IsBookmarkEnd
        {
            get
            {
                return ((CurrentNode.NodeType == NodeType.BookmarkEnd) && (((BookmarkEnd)CurrentNode).Name == mBookmarkName));
            }
        }

        internal bool HasBookmark { get; private set; }

        private readonly string mBookmarkName;
    }
}
