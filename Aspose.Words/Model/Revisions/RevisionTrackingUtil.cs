// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/11/2020 by Alexander Zhiltsov

using System;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Drawing;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Represents methods to mark nodes with revisions when document revision tracking
    /// (<see cref="Document.StartTrackRevisions(string,System.DateTime)"/>) is ON.
    /// </summary>
    [CppOverrideAccessModifier(AccessModifiers.Public)]
    internal static class RevisionTrackingUtil
    {
        /// <summary>
        /// Process deletion tracking.
        /// </summary>
        /// <returns>Returns false if deletion was NOT tracked so real deletion should be performed.</returns>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static bool TrackDeletion(Node node)
        {
            // Changes are not tracked in comments.
            if (node.GetAncestor(NodeType.Comment) != null)
                return false;

            if (node.NodeType == NodeType.Cell)
                return false;

            HashSetGeneric<Node> delayedDeletion = new HashSetGeneric<Node>();

            TrackDeletionCore(node, delayedDeletion);

            // If start node is in delayed node list we just return false and all subtree will be deleted.
            if (delayedDeletion.Contains(node))
                return false;

            // Process delayed deletion.
            // Nodes should be actually deleted so suspend tracking during this operation.
            using (new SuspendTrackRevisionsDocument(node.Document))
            {
                foreach (Node nodeToDelete in delayedDeletion)
                    nodeToDelete.Remove();
            }

            return true;
        }

        /// <summary>
        /// Adds insert revision to given ITrackRevision node.
        /// </summary>
        internal static void AddInsertRevision(ITrackableNode node, EditSession editSession)
        {
            EditRevision editRevision =
                new EditRevision(EditRevisionType.Insertion, editSession.Author, editSession.DateTime);
            node.InsertRevision = editRevision;
        }

        /// <summary>
        /// Adds delete revision to given ITrackRevision node.
        /// </summary>
        internal static void AddDeleteRevision(ITrackableNode node, EditSession editSession)
        {
            EditRevision editRevision =
                new EditRevision(EditRevisionType.Deletion, editSession.Author, editSession.DateTime);
            node.DeleteRevision = editRevision;
        }

        /// <summary>
        /// Tracks revisions for the specified node, which has been inserted into the document.
        /// </summary>
        /// <remarks>
        /// If the node is moved within the same document, move revisions may be generated.
        /// </remarks>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static void TrackInsertion(Node node, CompositeNode oldParent, Node oldNextSibling)
        {
            if ((oldParent == null) || (oldParent.Document != node.Document))
            {
                // The old position of the node is not in the same document: just mark as inserted.
                MarkInserted(node);
                return;
            }

            // On moving cells with tracking changes in MS Word, it merges cells with generating deletion/insertion
            // revisions for cell contents.
            // Now we generate move ranges on moving cell nodes. On accepting/rejecting revisions in MS Word, it
            // displays a warning that the table is corrupted (not always reproduced), but everything looks like good.
            // When generating such movement as deletion/insertion revisions, I got that accepting in MS Word works
            // well, but rejecting causes generation of wrong table grid.
            // Thus let's generate move revisions for now. It seems cell movement tracking has no practical profit
            // and this case will not be used often by customers.

            if (node.Document.IsMoveRevisionsTracked && TrackMoving(node, oldParent, oldNextSibling))
                return;

            // This node insertion cannot be tracked as a move revision, so generate deletion + insertion revisions.

            if ((node is ITrackableNode) || node.IsComposite) // If trackable or may contain trackable children.
            {
                // Create deletion revision node. Any move revision should be kept in the node.
                Node deletionRevisionNode = node.Clone(true);
                using (new SuspendTrackRevisionsDocument(node.Document))
                    oldParent.InsertBefore(deletionRevisionNode, oldNextSibling);

                MarkDeleted(deletionRevisionNode);
            }

            MarkInserted(node);
        }

        /// <summary>
        /// Creates move-from and move-to ranges and places them around the specified nodes.
        /// </summary>
        internal static void CreateMoveRanges(NodeRange moveToNodeRange, NodeRange moveFromNodeRange,
            string author, DateTime dateTime)
        {
            Document doc = (Document)moveToNodeRange.Document;
            // It is needed to generate unique move range name. When generating name included entire GUID number,
            // MS Word causes errors on accepting/rejecting changes. So, let's generate name using 16-digit hex.
            string name = "move" +
                          RandomUtil.NewGuid().GetHashCode().ToString("X8") +
                          RandomUtil.NewGuid().GetHashCode().ToString("X8");

            int moveFromRangeId = doc.GetNextAnnotationId();
            MoveFromRangeStart moveFromRangeStart =
                new MoveFromRangeStart(doc, moveFromRangeId, author, name, dateTime, DisplacedByType.Unspecified);
            InsertMoveRangeNode(moveFromRangeStart, moveFromNodeRange.Start.Node, false);

            MoveFromRangeEnd moveFromRangeEnd = new MoveFromRangeEnd(doc, moveFromRangeId, DisplacedByType.Unspecified);
            InsertMoveRangeNode(moveFromRangeEnd, moveFromNodeRange.End.Node, true);

            int moveToRangeId = doc.GetNextAnnotationId();
            MoveToRangeStart moveToRangeStart =
                new MoveToRangeStart(doc, moveToRangeId, author, name, dateTime, DisplacedByType.Unspecified);
            InsertMoveRangeNode(moveToRangeStart, moveToNodeRange.Start.Node, false);

            MoveToRangeEnd moveToRangeEnd = new MoveToRangeEnd(doc, moveToRangeId, DisplacedByType.Unspecified);
            InsertMoveRangeNode(moveToRangeEnd, moveToNodeRange.End.Node, true);
        }

        /// <summary>
        /// Marks node and all its child nodes as inserted.
        /// </summary>
        private static void MarkInserted(Node node)
        {
            if (node.IsComposite)
            {
                CompositeNode compositeNode = (CompositeNode)node;
                foreach (Node childNode in compositeNode.GetChildNodes(NodeType.Any, false))
                    MarkInserted(childNode);
            }

            ITrackableNode trackable = node as ITrackableNode;
            if (trackable == null)
                return;

            // Remove any move revisions if present. The 'node' may be moved to a place outside of a move range.
            trackable.RemoveMoveRevisions();

            AddInsertRevision(trackable, ((Document)node.Document).EditSession);
        }

        /// <summary>
        /// Marks node and all its child nodes as deleted.
        /// </summary>
        private static void MarkDeleted(Node node)
        {
            if (node.IsComposite)
            {
                CompositeNode compositeNode = (CompositeNode)node;
                foreach (Node childNode in compositeNode.GetChildNodes(NodeType.Any, false))
                    MarkDeleted(childNode);
            }

            ITrackableNode trackable = node as ITrackableNode;
            if (trackable != null)
                AddDeleteRevision(trackable, ((Document)node.Document).EditSession);
        }

        /// <summary>
        /// Marks node and all its child nodes as moved.
        /// </summary>
        private static void MarkMoved(Node node, MoveRevisionType revisionType)
        {
            // MS Word does not generate revisions in comments on moving: do the same. At this case a comment is visible
            // as in move-from range as in move-to when opening in MS Word.
            // And MS Word does not generate revisions in contents of text box shapes.
            if (node.IsComposite && !(node is ShapeBase) && (node.NodeType != NodeType.Comment))
            {
                CompositeNode compositeNode = (CompositeNode)node;

                foreach (Node childNode in compositeNode.GetChildNodes(NodeType.Any, false))
                {
                    if (compositeNode is InlineStory)
                    {
                        // This child node is located in another story than the move range: mark with
                        // deletion/insertion instead.
                        if (revisionType == MoveRevisionType.MoveTo)
                            MarkInserted(childNode);
                        else
                            MarkDeleted(childNode);
                    }
                    else
                    {
                        MarkMoved(childNode, revisionType);
                    }
                }
            }

            ITrackableNode trackable = node as ITrackableNode;
            if (trackable != null)
                AddMoveRevision(trackable, ((Document)node.Document).EditSession, revisionType);
        }

        /// <summary>
        /// Checks whether the specified node can be marked as a move revision, and if it can, performs it with generating
        /// the necessary move ranges, and clones the node to represent a move-from revision.
        /// </summary>
        private static bool TrackMoving(Node node, CompositeNode oldParent, Node oldNextSibling)
        {
            // The following cases are checked:
            // 1. Whether the node is inserted before a move-to range or to start of a move-to range, and whether
            // its old position was before a move-from range or at start of a move-from range. If it is so, and
            // the move-to range have the same name as the move-from range, and the range is created at the current
            // revision tracking session, the node is added to the existing move range.
            // (The both cases: when the node is inserted before a move-to range, and when to start of a move-to
            // range, are supported to not restrict a customer in a way how to move multiple nodes.)
            // 2. The same as #1, but at the end of move-to and move-from ranges.
            // 3. If neither #1 nor #2 happens, and as the insertion position as the old position are outside of
            // any move range, a new move range is created.
            // Otherwise, Insert and Delete revisions are generated.

            // Get previous and next nodes (trackable or move range start/end). They are not nodes of the same level but
            // logically next/previous with child traversal at first, i.e. where a paragraph is after all its children.
            // This nodes allow to determine whether the node is located in a move range.
            Node prevNode = GetPreviousTrackableOrMoveRange(node, false);
            Node nextNode = GetNextTrackableOrMoveRange(node, false);
            Node oldNextSiblingOrParent = (oldNextSibling != null) ? oldNextSibling : oldParent;
            Node oldPrevNode = ((oldNextSibling == null) && (oldParent.LastChild != null))
                ? GetPreviousTrackableOrMoveRange(oldParent.LastChild, true)
                : GetPreviousTrackableOrMoveRange(oldNextSiblingOrParent, false);
            Node oldNextNode = GetNextTrackableOrMoveRange(oldNextSiblingOrParent, true);

            MoveRangeStart moveToRangeStart = prevNode as MoveToRangeStart;
            moveToRangeStart = (moveToRangeStart == null) ? nextNode as MoveToRangeStart : moveToRangeStart;

            MoveRangeStart moveFromRangeStart = oldPrevNode as MoveFromRangeStart;
            moveFromRangeStart = (moveFromRangeStart == null) ? oldNextNode as MoveFromRangeStart : moveFromRangeStart;

            MoveToRangeEnd moveToRangeEnd = prevNode as MoveToRangeEnd;
            moveToRangeEnd = (moveToRangeEnd == null) ? nextNode as MoveToRangeEnd : moveToRangeEnd;

            MoveFromRangeEnd moveFromRangeEnd = oldPrevNode as MoveFromRangeEnd;
            moveFromRangeEnd = (moveFromRangeEnd == null) ? oldNextNode as MoveFromRangeEnd : moveFromRangeEnd;

            bool createNewMoveRange = false;
            bool needMoveMoveToRangeStart = false;
            bool needMoveMoveToRangeEnd = false;
            bool needMoveMoveFromRangeStart = false;
            bool needMoveMoveFromRangeEnd = false;

            // 1. Check whether the node is moved to the start of a move-to range.
            if (AreCorrespondingRangesOfCurrentTracking(moveToRangeStart, moveFromRangeStart))
            {
                // If the node is inserted before the move-to range start node, move the range start to include
                // the node.
                needMoveMoveToRangeStart = moveToRangeStart == nextNode;

                // If the node was located before the move-from range start node, move the range start to include
                // the move-from revision node.
                needMoveMoveFromRangeStart = moveFromRangeStart == oldNextNode;
            }
            // 2. Check whether the node is moved to the end of a move-to range.
            else if (AreCorrespondingRangesOfCurrentTracking(moveToRangeEnd, moveFromRangeEnd))
            {
                // If the node is inserted after the move-to range end node, move the range end to include
                // the node.
                needMoveMoveToRangeEnd = moveToRangeEnd == prevNode;

                // If the node was located after the move-from range end node, move the range end to include
                // the move-from revision node.
                needMoveMoveFromRangeEnd = moveFromRangeEnd == oldPrevNode;
            }
            // 3. Check whether the node is not inserted into an existing move range.
            else if (!IsInMoveRange(prevNode, nextNode) && !IsInMoveRange(oldPrevNode, oldNextNode))
            {
                // Let's do not create a move range if the node is not trackable and cannot contain trackable children.
                if (!node.IsComposite && !(node is IMoveTrackableNode))
                    return false;

                // If the node is not in a move range, a new move range will be created.
                createNewMoveRange = true;
            }
            else
            {
                return false; // Insertion/Deletion revisions will be generated.
            }

            // Let's skip generating move-from revision node for annotations at least for now. They need regenerating
            // IDs and keeping the same ID for start and end nodes. Also bookmarks need unique name.
            bool skipMoveFromRevision = (node is INodeWithAnnotationId) || (node is IBookmarkNode);

            Document doc = (Document)node.Document;
            using (new SuspendTrackRevisionsDocument(doc))
            {
                Node moveFromRevisionNode = null;
                if (!skipMoveFromRevision)
                {
                    // Create move-from revision node.
                    moveFromRevisionNode = node.Clone(true);
                    oldParent.InsertBefore(moveFromRevisionNode, oldNextSibling);
                    MarkMoved(moveFromRevisionNode, MoveRevisionType.MoveFrom);
                }

                MarkMoved(node, MoveRevisionType.MoveTo);

                // All movements are performed after cloning the 'node' since a move range node may be needed to place
                // into it. Without this the move range node might be cloned too.
                if (needMoveMoveToRangeStart)
                    InsertMoveRangeNode(moveToRangeStart, node, false);
                if (needMoveMoveToRangeEnd)
                    InsertMoveRangeNode(moveToRangeEnd, node, true);
                if (needMoveMoveFromRangeStart)
                    InsertMoveRangeNode(moveFromRangeStart, moveFromRevisionNode, false);
                if (needMoveMoveFromRangeEnd)
                    InsertMoveRangeNode(moveFromRangeEnd, moveFromRevisionNode, true);

                if (createNewMoveRange)
                {
                    Debug.Assert(moveFromRevisionNode != null);
                    CreateMoveRanges(new NodeRange(node, node),
                        new NodeRange(moveFromRevisionNode, moveFromRevisionNode),
                        doc.EditSession.Author,
                        doc.EditSession.DateTime);
                }
            }

            return true;
        }

        /// <summary>
        /// Inserts the move range node before or after the specified node. If the move range node cannot be inserted at
        /// the level of the node, it is placed to the block level.
        /// </summary>
        private static void InsertMoveRangeNode(Node moveRangeNode, Node revisionNode, bool insertAfter)
        {
            if (revisionNode == null)
                return;

            CompositeNode parent = revisionNode.ParentNode;
            if (!parent.CanInsert(moveRangeNode) && revisionNode.IsComposite)
            {
                // E.g. for a section, we place move range node before/after the first/last paragraph of the section.
                parent = (CompositeNode)revisionNode;
                while (!parent.CanInsert(moveRangeNode) && parent.FirstChild.IsComposite)
                {
                    // Position of the move range node is expected before/after revisionNode. It would be incorrect if
                    // we go through a trackable node end.
                    Debug.Assert(!(parent is ITrackableNode));
                    parent = (CompositeNode)(insertAfter ? parent.LastChild : parent.FirstChild);
                }

                // Position of the move range node is expected before/after revisionNode. It would be incorrect if
                // we go through a trackable node end.
                Debug.Assert(!(parent is ITrackableNode));
                parent.Insert(moveRangeNode, null, !insertAfter);
            }
            else
            {
                parent.Insert(moveRangeNode, revisionNode, insertAfter);
            }
        }

        /// <summary>
        /// Returns a flag indicating whether a node located between the specified nodes is within a move range.
        /// </summary>
        private static bool IsInMoveRange(Node prevNode, Node nextNode)
        {
            return
                (prevNode is MoveToRangeStart) ||
                (prevNode is MoveFromRangeStart) ||
                (nextNode is MoveToRangeEnd) ||
                (nextNode is MoveFromRangeEnd) ||
                HasAnyMoveRevision(prevNode) ||
                HasAnyMoveRevision(nextNode);
        }

        /// <summary>
        /// Returns a flag indicating whether the specified nodes are start (or end) nodes of the corresponding move
        /// ranges (i.e. ranges of the same movement), and which have been created during the current session of
        /// revision tracking.
        /// </summary>
        private static bool AreCorrespondingRangesOfCurrentTracking(Node moveToRangeBound, Node moveFromRangeBound)
        {
            if ((moveToRangeBound == null) || (moveFromRangeBound == null))
                return false;

            MoveRangeStart moveToRangeStart = (moveToRangeBound.NodeType == NodeType.MoveToRangeEnd)
                ? MoveRangeFinder.FindMoveRangeStart(moveToRangeBound, true)
                : (MoveToRangeStart)moveToRangeBound;

            MoveRangeStart moveFromRangeStart = (moveFromRangeBound.NodeType == NodeType.MoveFromRangeEnd)
                ? MoveRangeFinder.FindMoveRangeStart(moveFromRangeBound, false)
                : (MoveFromRangeStart)moveFromRangeBound;

            Document doc = (Document)moveToRangeBound.Document;

            return
                (moveToRangeStart.Name == moveFromRangeStart.Name) &&
                (moveToRangeStart.Author == doc.EditSession.Author) &&
                (moveToRangeStart.Date == doc.EditSession.DateTime);
        }

        /// <summary>
        /// Finds the nearest trackable node or start/end node of a move range in the backward direction.
        /// </summary>
        private static Node GetPreviousTrackableOrMoveRange(Node node, bool includeNode)
        {
            Node prevNode = includeNode ? node : GetPreviousNode(GetFirstDescendantOrSelf(node));
            while (prevNode != null)
            {
                if ((prevNode.NodeType == NodeType.MoveFromRangeStart) ||
                    (prevNode.NodeType == NodeType.MoveFromRangeEnd) ||
                    (prevNode.NodeType == NodeType.MoveToRangeStart) ||
                    (prevNode.NodeType == NodeType.MoveToRangeEnd) ||
                    (prevNode is ITrackableNode))
                {
                    return prevNode;
                }

                prevNode = GetPreviousNode(prevNode);
            }

            return null;
        }

        /// <summary>
        /// Gets a node before the specified node. It is expected that a parent node is located after all its child nodes.
        /// </summary>
        private static Node GetPreviousNode(Node node)
        {
            if (node.IsComposite && ((CompositeNode)node).HasChildNodes)
                return ((CompositeNode)node).LastChild;

            while ((node.PreviousSibling == null) && (node.ParentNode != null))
                node = node.ParentNode;

            return node.PreviousSibling;
        }

        /// <summary>
        /// Finds the nearest trackable node or start/end node of a move range in the forward direction.
        /// </summary>
        private static Node GetNextTrackableOrMoveRange(Node node, bool includeNode)
        {
            Node nextNode = includeNode ? GetFirstDescendantOrSelf(node) : GetNextNode(node);

            while (nextNode != null)
            {
                if ((nextNode.NodeType == NodeType.MoveFromRangeStart) ||
                    (nextNode.NodeType == NodeType.MoveFromRangeEnd) ||
                    (nextNode.NodeType == NodeType.MoveToRangeStart) ||
                    (nextNode.NodeType == NodeType.MoveToRangeEnd) ||
                    (nextNode is ITrackableNode))
                {
                    return nextNode;
                }

                nextNode = GetNextNode(nextNode);
            }

            return null;
        }

        /// <summary>
        /// Gets a node after the specified node. It is expected that a parent node is located after all its child nodes.
        /// </summary>
        private static Node GetNextNode(Node node)
        {
            Node nextNode = node.NextSibling;
            return (nextNode != null) ? GetFirstDescendantOrSelf(nextNode) : node.ParentNode;
        }

        /// <summary>
        /// Gets the deepest first descendant of the specified node or the node itself, if it has no children.
        /// </summary>
        private static Node GetFirstDescendantOrSelf(Node node)
        {
            while (node.IsComposite && ((CompositeNode)node).HasChildNodes)
                node = ((CompositeNode)node).FirstChild;

            return node;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified node has a move-from or move-to revision.
        /// </summary>
        private static bool HasAnyMoveRevision(Node node)
        {
            IMoveTrackableNode trackable = node as IMoveTrackableNode;
            if (trackable == null)
                return false;

            return (trackable.MoveToRevision != null) || (trackable.MoveFromRevision != null);
        }

        /// <summary>
        /// Adds a move revision to the given ITrackRevision node.
        /// </summary>
        private static void AddMoveRevision(IMoveTrackableNode node, EditSession editSession, MoveRevisionType revisionType)
        {
            MoveRevision revision = new MoveRevision(revisionType, editSession.Author, editSession.DateTime);

            if (revisionType == MoveRevisionType.MoveTo)
                node.MoveToRevision = revision;
            else
                node.MoveFromRevision = revision;
        }

        private static void TrackDeletionCore(Node node, HashSetGeneric<Node> delayedDeletion)
        {
            // Special case. Do not mark nodes in comment.
            if (node.GetAncestor(NodeType.Comment) != null)
                return;

            // Special case. Do not mark nodes in textbox.
            if (node.ParentNode.NodeType == NodeType.Shape)
                return;

            ITrackableNode trackableNode = node as ITrackableNode;
            if (trackableNode != null)
            {
                EditSession editSession = ((Document)node.Document).EditSession;

                if (IsInsertedBySameAuhor(trackableNode, editSession))
                {
                    // Nodes were inserted by the same author should be deleted.
                    delayedDeletion.Add(node);
                }
                else
                {
                    // Otherwise mark node as deleted
                    AddDeleteRevision(trackableNode, editSession);

                    // This node is preserved so all its parents should be preserved as well.
                    RemoveAncestorsFromDelayedDeletion(node, delayedDeletion);
                }
            }

            if (node.IsComposite)
            {
                CompositeNode compositeNode = (CompositeNode)node;
                foreach (Node childNode in compositeNode.GetChildNodes(NodeType.Any, false))
                    TrackDeletionCore(childNode, delayedDeletion);
            }
        }

        /// <summary>
        /// Removes all ancestors of node from delayed deletion list.
        /// </summary>
        private static void RemoveAncestorsFromDelayedDeletion(Node node, HashSetGeneric<Node> delayedDeletion)
        {
            while (node.ParentNode != null)
            {
                node = node.ParentNode;
                delayedDeletion.Remove(node);
            }
        }

        /// <summary>
        /// Returns true if given node was inserted by the same author.
        /// </summary>
        private static bool IsInsertedBySameAuhor(ITrackableNode node, EditSession editSession)
        {
            EditRevision editRevision = node.InsertRevision;

            if (editRevision == null)
                return false;

            // Word just compares author names rather than Rsid as I thought before.
            return (editSession.Author == editRevision.Author);
        }
    }
}
