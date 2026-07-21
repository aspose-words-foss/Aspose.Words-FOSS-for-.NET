// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2009 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Removes nodes in a range.
    /// </summary>
    internal class NodeRemover : NodeTraverser
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="joinMode"></param>
        /// <param name="processBoundBlockAnnotationAsInline">If <c>true</c>, when the range has block level annotation
        /// nodes at bounds, removal gives same result as when the annotation nodes are moved to inline level.</param>
        internal NodeRemover(NodeRange range, NodeJoinMode joinMode, bool processBoundBlockAnnotationAsInline)
            : base(range)
        {
            // The only place where a bookmark range can be removed at the moment is Bookmark.Text setter.
            // But it seems to be an odd scenario to replace a table part with a text. So we do not provide
            // table column bookmark range processing at the moment. Let's implement it on a request.
            if (range.IsTableColumnBookmarkRange)
                throw new ArgumentException("Table column bookmark range removal is not supported.", "range");

            mJoinMode = joinMode;
            mProcessBoundBlockAnnotationAsInline = processBoundBlockAnnotationAsInline;
        }

        /// <summary>
        /// Removes nodes from start up to but not including the end node.
        /// Start and end nodes must have the same parent.
        /// </summary>
        /// <remarks>
        /// Moved from <see cref="CompositeNode"/>. This method is used in plenty of places throughout the project,
        /// so it was reasonable leaving it yet it has nothing to do with <see cref="NodeEnumerator"/> etc.
        /// </remarks>
        /// <param name="start">Deletes from this node. If null, then end should be null too and nothing is done.</param>
        /// <param name="end">Deletes up to, but excluding this node. Can be null to indicate delete to end of parent.</param>
        internal static void RemoveSameParent(Node start, Node end)
        {
            if ((end != null) && (end.ParentNode != start.ParentNode))
                throw new ArgumentException("Start and end nodes must have the same parent.");

            Node curChild = start;
            while (curChild != end)
            {
                Node nextChild = curChild.NextSibling;
                curChild.Remove();
                curChild = nextChild;
            }
        }

        /// <summary>
        /// Removes all nodes between start and end nodes inclusively.
        /// </summary>
        /// <param name="start">The start node of the range.</param>
        /// <param name="end">The end node of the range.</param>
        /// <returns></returns>
        internal static void Remove(Node start, Node end)
        {
            Remove(start, true, end, true);
        }

        /// <summary>
        /// Removes all nodes between start and end nodes inclusively or exclusively.
        /// </summary>
        /// <param name="start">The start node of the range.</param>
        /// <param name="isRemoveStart"></param>
        /// <param name="end">The end node of the range.</param>
        /// <param name="isRemoveEnd"></param>
        /// <returns></returns>
        internal static void Remove(Node start, bool isRemoveStart, Node end, bool isRemoveEnd)
        {
            Remove(start, isRemoveStart, end, isRemoveEnd, NodeJoinMode.JoinToPreviousSibling);
        }

        /// <summary>
        /// Removes all nodes between start and end nodes inclusively or exclusively. Controls whether to
        /// join parent nodes if they are different.
        /// </summary>
        /// <param name="start">The start node of the range.</param>
        /// <param name="isRemoveStart"></param>
        /// <param name="end">The end node of the range.</param>
        /// <param name="isRemoveEnd"></param>
        /// <param name="joinMode"></param>
        /// <returns></returns>
        internal static void Remove(Node start, bool isRemoveStart, Node end, bool isRemoveEnd, NodeJoinMode joinMode)
        {
            Remove(new NodeRange(start, isRemoveStart, end, isRemoveEnd), joinMode, false);
        }

        /// <summary>
        /// Removes nodes in the specified range.
        /// </summary>
        /// <param name="range">A range whose nodes to remove.</param>
        /// <param name="joinMode"></param>
        /// <param name="processBoundBlockAnnotationAsInline">If <c>true</c>, when the range has block level annotation
        /// nodes at bounds, removal gives same result as when the annotation nodes are moved to inline level.</param>
        /// <returns></returns>
        internal static void Remove(NodeRange range, NodeJoinMode joinMode, bool processBoundBlockAnnotationAsInline)
        {
            using (NodeRemover remover = new NodeRemover(range, joinMode, processBoundBlockAnnotationAsInline))
                remover.RemoveCore();
        }

        /// <summary>
        /// Cannot be called "Remove" because will clash with inherited Iterator.Remove in Java.
        /// </summary>
        internal void RemoveCore()
        {
            Traverse();
            RemoveNodes();
            JoinNodes(mJoinMode);
        }

        /// <summary>
        /// Removes nodes pending removal.
        /// </summary>
        protected virtual void RemoveNodes()
        {
            // Remove everything except last parent nodes, if present.
            foreach (Node node in mNodesToRemove)
                node.Remove();
        }

        /// <summary>
        /// Joins ancestor nodes for previously removed nodes using the specified join mode.
        /// </summary>
        internal void JoinNodes(NodeJoinMode joinMode)
        {
            switch (joinMode)
            {
                case NodeJoinMode.DontJoin:
                    // Do nothing.
                    break;
                case NodeJoinMode.JoinToPreviousSibling:
                    JoinToPreviousSibling();
                    break;
                case NodeJoinMode.JoinToNextSibling:
                    JoinToNextSibling();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("joinMode");
            }
        }

        private void JoinToPreviousSibling()
        {
            // In mNodesToJoin we have the last paragraph or the last section, then last paragraph.
            // They will be joined with their previous siblings.
            foreach (CompositeNode sourceNode in mNodesToJoin)
            {
                if (sourceNode.ParentNode == null)
                    continue;

                Node prevSibling = sourceNode.NodeLevel != NodeLevel.Inline
                    ? sourceNode.PreviousNonAnnotationSibling
                    : sourceNode.PreviousSibling;

                if (prevSibling == null)
                    continue;

                // Not sure that we need to join with tables.
                if ((prevSibling.NodeType == NodeType.Table) || // WORDSNET-13874
                    (sourceNode.NodeType == NodeType.Table))    // WORDSNET-14353
                    continue;

                // WORDSNET-15194 Do not join paragraph with StructuredDocumentTag.
                if (prevSibling.NodeType == NodeType.StructuredDocumentTag)
                    continue;

                // RESILIENCY WORDSNET-4691 The problem occurred because bookmark end is in SmartTag,
                // in this case previous sibling might be inline (as in the test case). To fix we make sure
                // the previous sibling is composite, if not join content of source node to the parent of previous sibling.

                Node insertAfter;

                if (prevSibling.IsComposite)
                {
                    insertAfter = ((CompositeNode)prevSibling).LastChild;
                }
                else
                {
                    insertAfter = prevSibling;
                    prevSibling = prevSibling.ParentNode;
                }

                if (sourceNode.NodeType == NodeType.Section)
                {
                    // We join section bodies and then remove the source section with all its stories.
                    // I can't see another scenario.
                    Section destinationSection = (Section)prevSibling;
                    Section sourceSection = (Section)sourceNode;
                    Body sourceBody = sourceSection.Body;
                    Body destinationBody = destinationSection.Body;

                    destinationBody.InsertAfter(sourceBody.FirstChild, null, destinationBody.LastChild);

                    if (sourceSection.NextSibling != null)
                        JoinSectionHeaderFootersToNextSibling(sourceSection, (Section)sourceSection.NextSibling);
                }
                else
                {
                    CompositeNode composite = (CompositeNode)prevSibling;
                    composite.InsertAfter(sourceNode.FirstChild, null, insertAfter);

                    // Move annotations that are located between merged nodes.
                    if (sourceNode.NodeLevel != NodeLevel.Inline)
                        composite.InsertAfter(sourceNode.PreviousNonAnnotationSibling.NextSibling,
                            sourceNode, insertAfter);
                }

                sourceNode.Remove();
            }
        }

        private void JoinToNextSibling()
        {
            // In mNodesToJoin we have the last paragraph or the last section, then last paragraph.
            // They will be joined with their previous siblings.
            foreach (CompositeNode joinNode in mNodesToJoin)
            {
                Node previousSibling = joinNode.NodeLevel != NodeLevel.Inline
                    ? joinNode.PreviousNonAnnotationSibling
                    : joinNode.PreviousSibling;

                if (joinNode.ParentNode == null || previousSibling == null)
                    continue;

                // Not sure that we need to join with tables.
                if ((joinNode.NodeType == NodeType.Table) || (previousSibling.NodeType == NodeType.Table))
                    continue;

                // Do not join paragraph with StructuredDocumentTag.
                if (joinNode.NodeType == NodeType.StructuredDocumentTag)
                    continue;

                // WORDSNET-25105 Do not join OfficeMath.
                if (joinNode.NodeType == NodeType.OfficeMath)
                    continue;

                // RESILIENCY WORDSNET-4691 The problem occurred because bookmark end is in SmartTag,
                // in this case previous sibling might be inline (as in the test case). To fix we make sure
                // the previous sibling is composite, if not join content of source node to the parent of previous sibling.

                if (!previousSibling.IsComposite)
                    previousSibling = previousSibling.ParentNode;

                if (joinNode.NodeType == NodeType.Section)
                    JoinSectionToNextSibling((Section)previousSibling, (Section)joinNode);
                else
                    JoinNodeToNextSibling((CompositeNode)previousSibling, joinNode);
            }
        }

        private static void JoinSectionToNextSibling(Section removingSection, Section nextSection)
        {
            // We join section bodies, headers and footers. And then remove the source section with all its stories.
            // I can't see another scenario.

            Body sourceBody = removingSection.Body;
            Body destinationBody = nextSection.Body;

            destinationBody.InsertBefore(sourceBody.FirstChild, null, destinationBody.FirstChild);
            JoinSectionHeaderFootersToNextSibling(removingSection, nextSection);

            removingSection.Remove();
        }

        private static void JoinNodeToNextSibling(CompositeNode removingNode, CompositeNode nextNode)
        {
            // Move annotations that are located between the merged nodes.
            if (nextNode.NodeLevel != NodeLevel.Inline)
                nextNode.InsertBefore(removingNode.NextSibling, nextNode, nextNode.FirstChild);

            // Do not join a paragraph with a SmartTag. (Fix for TestBookmarksRunner.UnifiedTestDefect4691.)
            if ((nextNode.NodeType == NodeType.SmartTag) && (removingNode == nextNode.ParentNode))
            {
                using (new SuspendTrackRevisionsDocument(nextNode.Document))
                    removingNode.InsertBefore(nextNode.FirstChild, null, nextNode);

                nextNode.Remove();
            }
            else
            {
                MoveChildrenAndRemoveNode(removingNode, nextNode);
            }
        }

        private static void MoveChildrenAndRemoveNode(CompositeNode node, CompositeNode childrenDestination)
        {
            Document document = node.FetchDocument();
            bool useDeleteRevision = document.IsTrackRevisionsEnabled && (node is ITrackableNode);
            if (!useDeleteRevision)
            {
                using (new SuspendTrackRevisionsDocument(document))
                    childrenDestination.InsertBefore(node.FirstChild, null, childrenDestination.FirstChild);

                node.Remove();
            }
            else
            {
                RevisionTrackingUtil.AddDeleteRevision((ITrackableNode)node, document.EditSession);
            }
        }

        private static void JoinSectionHeaderFootersToNextSibling(Section source, Section destination)
        {
            HeaderFooter[] sourceheaderFooters = source.HeadersFooters.ToArray();
            foreach (HeaderFooter sourceHeaderFooter in sourceheaderFooters)
            {
                HeaderFooter destinationHeaderFooter = destination.HeadersFooters[sourceHeaderFooter.HeaderFooterType];
                if (destinationHeaderFooter != null && !destinationHeaderFooter.IsLinkedToPrevious)
                    continue;

                if (destinationHeaderFooter != null)
                    destinationHeaderFooter.Remove();

                destination.HeadersFooters.Add(sourceHeaderFooter);
            }
        }

        protected void LockRemove()
        {
            mIsRemoveLocked = true;
        }

        protected void UnlockRemove()
        {
            mIsRemoveLocked = false;
        }

        protected override void OnMiddleNodeAncestor()
        {
            if (mIsRemoveLocked)
                return;

            // Middle parent nodes are removed except if start bound is expected to be a child of it.
            if (IsLogicalAncestorOfStartNode(CurrentNode))
            {
                CompositeNode deepestComposite = (CompositeNode)CurrentNode;
                while (true)
                {
                    Node node = deepestComposite.FirstNonAnnotationChild;
                    if ((node != null) && node.IsComposite && (node.NodeLevel != NodeLevel.Inline))
                        deepestComposite = (CompositeNode)node;
                    else
                        break;

                    // Remove non-first children.
                    node = node.NextSibling;
                    while (node != null)
                    {
                        AddNodeToRemove(node);
                        node = node.NextSibling;
                    }
                }

                foreach (Node node in deepestComposite)
                    AddNodeToRemove(node);
            }
            else
            {
                AddNodeToRemove(CurrentNode);
            }
        }

        /// <summary>
        /// Adds node to the collection of nodes pending removal.
        /// </summary>
        protected virtual void AddNodeToRemove(Node node)
        {
            mNodesToRemove.Add(node);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node is root ancestor of the block range start cross-structure
        /// annotation that is logically expected to be at inline level as descendant of it.
        /// </summary>
        private bool IsLogicalAncestorOfStartNode(Node node)
        {
            Node startNode = Range.Start.Node;
            return mProcessBoundBlockAnnotationAsInline &&
                NodeUtil.IsCrossStructureAnnotation(startNode) &&
                (startNode.NextNonAnnotationSibling == node) &&
                (startNode.NodeLevel != NodeLevel.Inline);
        }

        protected override void OnEndNodeAncestor()
        {
            // Last parent node is joined except if start bound is expected to be a child of it.
            // Last section is joined with its previous sibling (more precisely, their bodies are joined),
            // last paragraph is joined with its previous sibling.
            if (!IsLogicalAncestorOfStartNode(CurrentNode))
                mNodesToJoin.Add((CompositeNode)CurrentNode);
        }

        protected override void OnNonCompositeNode()
        {
            if (mIsRemoveLocked)
                return;

            // Child nodes are always removed.
            AddNodeToRemove(CurrentNode);
        }

        /// <summary>
        /// Used to collect nodes to remove in <see cref="OnMiddleNodeAncestor"/>, <see cref="OnEndNodeAncestor"/>
        /// and <see cref="OnNonCompositeNode"/> methods, overridden in inherited classes.
        /// </summary>
        protected IList<Node> NodesToRemove
        {
            get { return mNodesToRemove; }
        }

        private readonly NodeJoinMode mJoinMode;
        private readonly List<Node> mNodesToRemove = new List<Node>();
        private readonly List<CompositeNode> mNodesToJoin = new List<CompositeNode>();
        /// <summary>
        /// If <c>true</c>, when the range has block level annotation nodes at bounds, removal gives same result
        /// as when the annotation nodes are moved to inline level.
        /// </summary>
        private readonly bool mProcessBoundBlockAnnotationAsInline;

        private bool mIsRemoveLocked;
    }
}
