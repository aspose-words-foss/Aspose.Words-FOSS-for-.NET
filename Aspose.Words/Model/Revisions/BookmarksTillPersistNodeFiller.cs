// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2015 by Denis Darkin

using System.Collections.Generic;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Allows filling bookmarks that are on deleting text at the begin of a document. 
    /// </summary>
    internal class BookmarksTillPersistNodeFiller
    {
        internal BookmarksTillPersistNodeFiller(DocumentBase doc, RevisionHandlingContext context)
        {
            mDocument = doc;
            mContext = context;
        }

        /// <summary>
        /// Fills bookmarks that are on deleting text at the begin of a document.
        /// </summary>
        internal IDictionary<Node, Node> FillBookmarks()
        {
            mBookmarks = new Dictionary<Node, Node>();
            FillBookmarksForNode(mDocument, false);
            return mBookmarks;
        }

        /// <summary>
        /// Recursive method to fill bookmarks from the passed composite node and all its children. 
        /// </summary>
        /// <param name="node">a composite node to process.</param>
        /// <param name="isNodeDeleted">the composite node or any of its parents is being deleted.</param>
        /// <returns>false if a node that will not be deleted is achived and 
        /// the process should be stopped.</returns>
        private bool FillBookmarksForNode(CompositeNode node, bool isNodeDeleted)
        {
            bool first = true;
            Node curChild = node.FirstChild;
            while (curChild != null)
            {
                if (!isNodeDeleted && BookmarkDeleter.IsBookmarkTargetNode(curChild))
                {
                    if (!mContext.IsDeletingNode(curChild))
                    {
                        if (first)
                        {
                            // First node of document is not deleted: should not delete bookmarks
                            mBookmarks.Clear();
                        }
                        return false;
                    }
                    first = false;
                }

                if (curChild is CompositeNode)
                {
                    if (!FillBookmarksForNode((CompositeNode)curChild, 
                            isNodeDeleted || mContext.IsDeletingNode(curChild)))
                        return false;
                }
                else if (curChild.NodeType == NodeType.BookmarkStart)
                {
                    mBookmarks.Add(curChild, null);
                }
                else if (curChild.NodeType == NodeType.BookmarkEnd)
                {
                    Node start = FindBookmarkStart(mBookmarks, (BookmarkEnd)curChild);
                    if (start != null)
                        mBookmarks[start] = curChild;
                }

                curChild = curChild.NextSibling;
            }
            return true;
        }

        /// <summary>
        /// Searches bookmark start by bookmark end in a bookmark start-to-end dictionary.
        /// </summary>
        private static Node FindBookmarkStart(IDictionary<Node, Node> bookmarkStartToBookmarkEnd, BookmarkEnd bookmarkEnd)
        {
            foreach (Node node in bookmarkStartToBookmarkEnd.Keys)
            {
                BookmarkStart bookmarkStart = (BookmarkStart)node;
                if (StringUtil.EqualsIgnoreCase(bookmarkStart.Name, bookmarkEnd.Name))
                    return bookmarkStart;
            }
            return null;
        }

        /// <summary>
        /// A document, from which bookmarks are being filled.
        /// </summary>
        private readonly DocumentBase mDocument;
        /// <summary>
        /// Context of accepting/rejecting revisions that contains node deletion lists.
        /// </summary>
        private readonly RevisionHandlingContext mContext;
        /// <summary>
        /// Filling hash table. Has bookmark start as keys and bookmark end as values.
        /// </summary>
        private IDictionary<Node, Node> mBookmarks;
    }
}
