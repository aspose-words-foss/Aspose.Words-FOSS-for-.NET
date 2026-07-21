// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2015 by Alexander Zhiltsov

using System.Collections.Generic;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Allows deleting bookmarks on nodes that are to be deleted. Also moves bookmarks, that should not be deleted, 
    /// from deleting nodes. The class is used on accepting and rejecting revisions.
    /// </summary>
    internal class BookmarkDeleter
    {
        private BookmarkDeleter(DocumentBase doc, RevisionHandlingContext context)
        {
            mDocument = doc;
            mBookmarkCache = new BookmarkCache((Document)doc);
            mContext = context.Clone();
            CompleteDeletionLists();
        }

        /// <summary>
        /// Deletes or moves bookmarks if necessary from nodes that are expected to be deleted later.
        /// </summary>
        internal static void DeleteBookmarksFromDeletingNodes(DocumentBase doc, RevisionHandlingContext context)
        {
            BookmarkDeleter bmDeleter = new BookmarkDeleter(doc, context);
            bmDeleter.Execute();
        }

        /// <summary>
        /// Processes deletion or movement of bookmarks.
        /// </summary>
        private void Execute()
        {
            DeleteBookmarksIfNeed();

            BookmarkMover bmMover = new BookmarkMover(mDocument, mContext);
            bmMover.MoveFromDeletingNodes();
        }

        /// <summary>
        /// Deletes bookmarks on text that will be deleted during accepting or rejecting revisions.
        /// </summary>
        private void DeleteBookmarksIfNeed()
        {
            // MS Word 2013 behavior is the following.
            // Selected-text bookmarks are deleted if the text is deleted.
            // Position bookmarks are deleted:
            // 1. At beginning of a document.
            // 2. If previous and next runs within a paragraph are deleted.
            // 3. If a bookmark is in a table cell and the cell is deleted.

            DeleteBookmarksAtBeginOfDoc();
            DeleteBookmarksOnDeletingText();
        }

        /// <summary>
        /// Deletes bookmarks till the first node that is not deleted.
        /// </summary>
        private void DeleteBookmarksAtBeginOfDoc()
        {
            BookmarksTillPersistNodeFiller bmFiller = new BookmarksTillPersistNodeFiller(mDocument, mContext);
            IDictionary<Node, Node> bmStartToBmEnd = bmFiller.FillBookmarks();

            foreach (KeyValuePair<Node, Node> entry in bmStartToBmEnd)
                if (entry.Value != null) // is the bookmark end achieved?
                {
                    ((BookmarkEnd)entry.Value).Remove();
                    ((BookmarkStart)entry.Key).Remove();
                }
        }

        /// <summary>
        /// Checks if the node can be bookmark target. 
        /// </summary>
        internal static bool IsBookmarkTargetNode(Node node)
        {
            // Also the same as in RevisionUtil.HandleNodeRevision.
            return
                node.NodeType == NodeType.Paragraph ||
                node is IInline;
        }

        /// <summary>
        /// Deletes bookmarks if need.
        /// </summary>
        private void DeleteBookmarksOnDeletingText()
        {
            NodeCollection nodes = mDocument.GetChildNodes(NodeType.BookmarkStart, true);
            IList<Node> bookmarkStarts = nodes.ToNodeList();
            foreach (Node node in bookmarkStarts)
            {
                BookmarkStart bookmarkStart = (BookmarkStart)node;
                if (CanDeleteBookmark(bookmarkStart.Name))
                {
                    Bookmark bookmark = mBookmarkCache[bookmarkStart.Name];
                    if (NeedDeleteBookmark(bookmark))
                        bookmark.Remove();
                }
            }
        }

        /// <summary>
        /// Returns true for bookmarks that can be deleted during accepting or rejecting revisions.
        /// </summary>
        private static bool CanDeleteBookmark(string bookmarkName)
        {
            return bookmarkName != Bookmark.GoBackBookmarkName;
        }

        /// <summary>
        /// Checks if the bookmark should be deleted.
        /// </summary>
        private bool NeedDeleteBookmark(Bookmark bookmark)
        {
            // Incomplete bookmark encountered. 
            if (bookmark == null)
                return false;

            NodeRange nodeRange = bookmark.GetNodeRange();
            bool coversNodes = false;

            foreach (Node node in nodeRange)
                if (IsBookmarkTargetNode(node))
                {
                    if (!IsDeletingNode(node))
                        return false;
                    coversNodes = true;
                }

            if (!coversNodes)
                return ArePrevAndNextNodesDeleted(bookmark) || IsInDeletingCell(bookmark.BookmarkStart);
            else
                return true;
        }

        /// <summary>
        /// Checks if the previous and the next text nodes are in deleted node lists.
        /// </summary>
        private bool ArePrevAndNextNodesDeleted(Bookmark bookmark)
        {
            bool prevDeleted = false;
            Node node = bookmark.BookmarkStart.PreviousSibling;
            while (node != null)
            {
                if (IsBookmarkTargetNode(node))
                {
                    prevDeleted = IsDeletingNode(node);
                    break;
                }
                node = node.PreviousSibling;
            }
            if (!prevDeleted)
                return false;

            bool nextDeleted = false;
            node = bookmark.BookmarkEnd.NextSibling;
            while (node != null)
            {
                if (IsBookmarkTargetNode(node))
                {
                    nextDeleted = IsDeletingNode(node);
                    break;
                }
                node = node.NextSibling;
            }

            return nextDeleted;
        }

        /// <summary>
        /// Checks if the node belongs to a deleting table cell.
        /// </summary>
        private bool IsInDeletingCell(Node node)
        {
            CompositeNode parentCell = node.GetAncestor(NodeType.Cell);
            return (parentCell != null) && IsDeletingNode(parentCell);
        }

        /// <summary>
        /// Checks if the node is in the deleted node lists.
        /// </summary>
        private bool IsDeletingNode(Node node)
        {
            return mContext.IsDeletingNode(node);
        }

        /// <summary>
        /// Fills missing cells/rows/tables which contents are to be completely removed but which are not present in
        /// the node deletion lists of <see cref="mContext"/>.
        /// </summary>
        private void CompleteDeletionLists()
        {
            FillMissingDeletingNodes(mContext.DelayedParagraphs, mContext.DelayedCells, NodeType.Cell);
            FillMissingDeletingNodes(mContext.DelayedCells, mContext.DelayedRows, NodeType.Row);
            FillMissingDeletingNodes(mContext.DelayedRows, mContext.DelayedTables, NodeType.Table);
        }

        /// <summary>
        /// Fills missing parent nodes which children are to be completely removed but which are not present in
        /// the node deletion list.
        /// </summary>
        private static void FillMissingDeletingNodes(IList<Node> deletingNodes, IList<Node> deletingParents, NodeType parentType)
        {
            Node previousParent = null;

            foreach (Node node in deletingNodes)
            {
                CompositeNode parent = node.FirstNonMarkupParentNode;
                if ((parent != previousParent) && (parent.NodeType == parentType) && !deletingParents.Contains(parent))
                {
                    previousParent = parent;

                    // Checks if all node.NodeType children are in the deletingNodes list.
                    bool areAllChildrenDeleting = true;
                    for (Node child = parent.FirstNonMarkupDescendant; 
                        child != null; 
                        child = child.NextNonMarkupNodeLimited)
                    {
                        if ((child.NodeType == node.NodeType) && !deletingNodes.Contains(child))
                        {
                            areAllChildrenDeleting = false;
                            break;
                        }
                    }

                    if (areAllChildrenDeleting)
                        deletingParents.Add(parent);
                }
            }
        }

        /// <summary>
        /// The document, from which bookmarks are being deleted.
        /// </summary>
        private readonly DocumentBase mDocument;
        /// <summary>
        /// Context of accepting/rejecting revisions that contains node deletion lists.
        /// </summary>
        private readonly RevisionHandlingContext mContext;
        /// <summary>
        /// Bookmarks cache.
        /// </summary>
        private readonly BookmarkCache mBookmarkCache;
    }
}
