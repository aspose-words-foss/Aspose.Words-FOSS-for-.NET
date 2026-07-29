// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2015 by Denis Darkin

using System.Collections.Generic;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Allows moving bookmarks from nodes that will be deleted during accepting or rejecting revisions.
    /// </summary>
    internal class BookmarkMover
    {
        internal BookmarkMover(DocumentBase doc, RevisionHandlingContext context)
        {
            mDocument = doc;
            mContext = context;
        }

        /// <summary>
        /// Moves bookmarks from nodes that are in deleting node lists.
        /// </summary>
        internal void MoveFromDeletingNodes()
        {
            MoveFromDeletingNodesCore(mDocument, false, new List<Node>());
        }

        /// <summary>
        /// Processes the composite node to move bookmarks from nodes that will be deleted.
        /// </summary>
        /// <param name="node">a composite node to process.</param>
        /// <param name="isNodeDeleted">the composite node or any of its parents is being deleted.</param>
        /// <param name="bookmarkNodesToMove">bookmark start and bookmark end nodes to move.</param>
        private void MoveFromDeletingNodesCore(CompositeNode node, bool isNodeDeleted,
            List<Node> bookmarkNodesToMove)
        {
            if (!isNodeDeleted)
                MoveNodesIfAllowedTo(node, node.FirstChild, bookmarkNodesToMove);

            Node curChild = node.FirstChild;
            while (curChild != null)
            {
                Node nextChild = curChild.NextSibling;

                if (curChild is CompositeNode)
                {
                    CompositeNode child = (CompositeNode)curChild;
                    MoveFromDeletingNodesCore(child, isNodeDeleted || mContext.IsDeletingNode(child),
                        bookmarkNodesToMove);

                    if (!isNodeDeleted)
                        MoveNodesIfAllowedTo(node, nextChild, bookmarkNodesToMove);
                }
                else if (isNodeDeleted &&
                         ((curChild.NodeType == NodeType.BookmarkStart) ||
                          (curChild.NodeType == NodeType.BookmarkEnd)))
                {
                    bookmarkNodesToMove.Add(curChild);
                }

                curChild = nextChild;
            }
        }

        /// <summary>
        /// Moves the bookmark nodes to the parent node if it is allowed.
        /// </summary>
        private static void MoveNodesIfAllowedTo(CompositeNode compositeNode, Node beforeNode, List<Node> movingNodes)
        {
            // Looks like MS Word moves bookmark start nodes to the nearest paragraph. Bookmark end is inserted
            // at any appropriate place. Let's do the same.
            int i = 0;
            while (i < movingNodes.Count)
            {
                Node node = movingNodes[i];
                if (compositeNode.CanInsert(node) &&
                    ((node.NodeType != NodeType.BookmarkStart) || CanInsertBookmarkStart(compositeNode)) &&
                    ((node.NodeType != NodeType.BookmarkEnd) || 
                        !ContainsBookmarkStart(movingNodes, i - 1, ((BookmarkEnd)node).Name)))
                {
                    compositeNode.InsertBefore(node, beforeNode);
                    movingNodes.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        /// <summary>
        /// Returns <c>true</c> if a bookmark start node can be inserted into the specified destination node.
        /// </summary>
        private static bool CanInsertBookmarkStart(CompositeNode destinationNode)
        {
            return destinationNode.NodeLevel == NodeLevel.Block;
        }

        /// <summary>
        /// Returns <c>true</c> if the node list contains a bookmark start node with the specified name.
        /// Search range is limited by the specified end index.
        /// </summary>
        private static bool ContainsBookmarkStart(List<Node> nodes, int endIndex, string bookmarkName)
        {
            for (int i = 0; i <= endIndex; i++)
            {
                if ((nodes[i].NodeType == NodeType.BookmarkStart) &&
                    StringUtil.EqualsIgnoreCase(((BookmarkStart)nodes[i]).Name, bookmarkName))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// A document, from which bookmarks are being filled.
        /// </summary>
        private readonly DocumentBase mDocument;
        /// <summary>
        /// Context of accepting/rejecting revisions that contains node deletion lists.
        /// </summary>
        private readonly RevisionHandlingContext mContext;
    }
}
