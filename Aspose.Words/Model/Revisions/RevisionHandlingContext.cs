// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/02/2016 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Words.Markup;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// This class is used on accepting/rejection revisions to store necessary data.
    /// </summary>
    internal class RevisionHandlingContext
    {
        internal RevisionHandlingContext(IList<Revision> allRevisions, bool isAcceptance)
        {
            AllRevisions = allRevisions;
            IsSingleRevision = false;
            IsAcceptance = isAcceptance;
        }

        internal RevisionHandlingContext(bool isAcceptance, RevisionType revisionType)
        {
            IsSingleRevision = true;
            IsAcceptance = isAcceptance;
            RevisionType = revisionType;
        }

        /// <summary>
        /// Clones this instance of the class.
        /// </summary>
        internal RevisionHandlingContext Clone()
        {
            RevisionHandlingContext lhs = new RevisionHandlingContext(AllRevisions, IsAcceptance);

            lhs.mDelayedInlines.AddRange(mDelayedInlines);
            lhs.mDelayedParagraphs.AddRange(mDelayedParagraphs);
            lhs.mDelayedCells.AddRange(mDelayedCells);
            lhs.mDelayedRows.AddRange(mDelayedRows);
            lhs.mDelayedTables.AddRange(mDelayedTables);
            lhs.mDelayedSdts.AddRange(mDelayedSdts);

            foreach (KeyValuePair<StructuredDocumentTag, ParentSdtInfo> pair in mSdtNesting)
                lhs.mSdtNesting.Add(pair.Key, pair.Value);
            lhs.mInlinedSdts.AddRange(mInlinedSdts);

            return lhs;
        }

        /// <summary>
        /// Adds the node to a corresponding delayed deletion list.
        /// </summary>
        internal void AddDelayedNode(Node node)
        {
            // Check whether the node has already been added.
            if (IsDeletingNode(node))
                return;

            switch (node.NodeType)
            {
                case NodeType.Paragraph:
                    mDelayedParagraphs.Add(node);
                    break;
                case NodeType.Cell:
                    mDelayedCells.Add(node);
                    break;
                case NodeType.Row:
                    mDelayedRows.Add(node);
                    break;
                case NodeType.Table:
                    mDelayedTables.Add(node);
                    break;
                case NodeType.StructuredDocumentTag:
                    mDelayedSdts.Add(node);
                    break;
                default:
                    if (node is IInline)
                        mDelayedInlines.Add(node);
                    else
                        throw new InvalidOperationException("Wrong node type.");
                    break;
            }
        }

        /// <summary>
        /// Checks whether the node is in a delayed deletion list.
        /// </summary>
        internal bool IsDeletingNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Paragraph:
                    return mDelayedParagraphs.Contains(node);
                case NodeType.Cell:
                    return mDelayedCells.Contains(node);
                case NodeType.Row:
                    return mDelayedRows.Contains(node);
                case NodeType.Table:
                    return mDelayedTables.Contains(node);
                case NodeType.StructuredDocumentTag:
                    return mDelayedSdts.Contains(node);
                default:
                    return (node is IInline) && mDelayedInlines.Contains(node);
            }
        }

        /// <summary>
        /// Stores SDT nodes that may become nested if will be empty after accepting/rejecting revisions.
        /// MS Word has strange behaviour of making SDTs nested in some cases.
        /// </summary>
        internal void AddSdtNesting(StructuredDocumentTag parentSdt, StructuredDocumentTag childSdt,
            bool canNestInlineSdts)
        {
            if (mSdtNesting.ContainsKey(childSdt))
                return;

            mSdtNesting.Add(childSdt, new ParentSdtInfo(parentSdt, canNestInlineSdts));
        }

        /// <summary>
        /// Gets a flag indicating whether the specified parent SDT intended as destination to move nested SDTs
        /// can nest inline level SDTs.
        /// </summary>
        /// <seealso cref="ParentsForNestedSdts"/>
        /// <seealso cref="GetMovingNestedSdts"/>
        internal bool CanNestInlineSdts(StructuredDocumentTag parentSdt)
        {
            foreach (ParentSdtInfo info in mSdtNesting.Values)
            {
                if (info.ParentSdt == parentSdt)
                    return info.CanNestInlineSdts;
            }

            return false;
        }

        /// <summary>
        /// Returns list of nodes that are intended to be moved into the specified SDT node during handling revisions.
        /// SDTs may become nested on accepting revisions.
        /// </summary>
        internal List<StructuredDocumentTag> GetMovingNestedSdts(StructuredDocumentTag parentSdt)
        {
            List<StructuredDocumentTag> list = new List<StructuredDocumentTag>();
            foreach (KeyValuePair<StructuredDocumentTag, ParentSdtInfo> pair in mSdtNesting)
            {
                // Skip nodes that are already nested.
                if ((pair.Value.ParentSdt == parentSdt) && !pair.Key.IsAncestorNode(parentSdt))
                    list.Add(pair.Key);
            }

            NodeSorter.Sort(list);

            return list;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node has children that are not marked for deletion.
        /// </summary>
        internal bool HasPreservedTrackables(Node node, bool skipMarkups)
        {
            if (!node.IsComposite)
                return false;

            Node currentNode = ((CompositeNode)node).FirstChild;

            while (currentNode != null)
            {
                if (!(NodeUtil.IsMarkupNode(currentNode) && skipMarkups) &&
                    (currentNode is ITrackableNode) &&
                    !IsDeletingNode(currentNode))
                    return true;

                currentNode = currentNode.NextPreOrder(node);
            }

            return false;
        }

        /// <summary>
        /// Gets/sets currently processing node.
        /// </summary>
        internal Node CurrentNode
        {
            get
            {
                return mCurrentNode;
            }
            set
            {
                mCurrentNode = value;

                // WORDSNET-18519 Comment node can contain move revision.
                IMoveTrackableNode trackable = value as IMoveTrackableNode;
                CurrentMoveRange =
                    ((trackable != null) && ((trackable.MoveFromRevision != null) || (trackable.MoveToRevision != null)))
                    ? MoveRangeFinder.FindMoveRange(value, trackable.MoveToRevision != null)
                    : null;
            }
        }

        /// <summary>
        /// Returns move range of the current node if it has move-from or move-to revision.
        /// </summary>
        internal MoveRange CurrentMoveRange { get; private set; }

        /// <summary>
        /// List that stores deleted inlines.
        /// </summary>
        internal IList<Node> DelayedInlines
        {
            get { return mDelayedInlines; }
        }

        /// <summary>
        /// List that stores deleted paragraphs.
        /// </summary>
        internal IList<Node> DelayedParagraphs
        {
            get { return mDelayedParagraphs; }
        }

        /// <summary>
        /// List that stores deleted rows.
        /// </summary>
        internal IList<Node> DelayedRows
        {
            get { return mDelayedRows; }
        }

        /// <summary>
        /// List that stores deleted cells.
        /// </summary>
        internal IList<Node> DelayedCells
        {
            get { return mDelayedCells; }
        }

        /// <summary>
        /// List that stores deleted tables.
        /// </summary>
        internal IList<Node> DelayedTables
        {
            get { return mDelayedTables; }
        }

        /// <summary>
        /// List that stores deleted structured document tags.
        /// </summary>
        internal IList<Node> DelayedSdts
        {
            get { return mDelayedSdts; }
        }

        /// <summary>
        /// Stores structured document tags that have become inline-level during handling revisions.
        /// </summary>
        internal IList<StructuredDocumentTag> InlinedSdts
        {
            get { return mInlinedSdts; }
        }

        /// <summary>
        /// Returns SDTs that are intended as destination for moving other SDTs during handling revisions.
        /// </summary>
        internal IList<StructuredDocumentTag> ParentsForNestedSdts
        {
            get
            {
                List<StructuredDocumentTag> list = new List<StructuredDocumentTag>();
                foreach (ParentSdtInfo info in mSdtNesting.Values)
                {
                    if (!list.Contains(info.ParentSdt))
                        list.Add(info.ParentSdt);
                }

                return list;
            }
        }

        /// <summary>
        /// Returns all revisions being accepted/rejected. Can be null on handling a single revision.
        /// </summary>
        internal IList<Revision> AllRevisions { get; }

        /// <summary>
        /// Returns <c>true</c> if acceptance/rejection of a single revision is being performed.
        /// </summary>
        internal bool IsSingleRevision { get; }

        /// <summary>
        /// Indicates whether acceptance (<c>true</c>) or rejection (<c>false</c>) of revisions is being performed.
        /// </summary>
        internal bool IsAcceptance { get; }

        /// <summary>
        /// Specifies <see cref="Aspose.Words.RevisionType"/> of a single handling revision.
        /// </summary>
        internal RevisionType RevisionType { get; }

        private readonly List<Node> mDelayedInlines = new List<Node>();
        private readonly List<Node> mDelayedParagraphs = new List<Node>();
        private readonly List<Node> mDelayedRows = new List<Node>();
        private readonly List<Node> mDelayedCells = new List<Node>();
        private readonly List<Node> mDelayedTables = new List<Node>();
        private readonly List<Node> mDelayedSdts = new List<Node>();

        /// <summary>
        /// Stores possible nesting SDTs. Key: child SDT, value: parent SDT info.
        /// </summary>
        private readonly Dictionary<StructuredDocumentTag, ParentSdtInfo> mSdtNesting =
            new Dictionary<StructuredDocumentTag, ParentSdtInfo>();
        private readonly List<StructuredDocumentTag> mInlinedSdts = new List<StructuredDocumentTag>();

        private Node mCurrentNode;

        /// <summary>
        /// Sort nodes by their location in a document.
        /// </summary>
        private sealed class NodeSorter : IComparer<StructuredDocumentTag>
        {
            internal static void Sort(List<StructuredDocumentTag> nodeList)
            {
                nodeList.Sort(new NodeSorter());
            }

            /// <summary>
            /// Compares nodes by their location in a document.
            /// </summary>
            int IComparer<StructuredDocumentTag>.Compare(StructuredDocumentTag node1, StructuredDocumentTag node2)
            {
                if (node1 == node2)
                    return 0;

                return node1.IsAbove(node2) ? -1 : 1;
            }
        }

        private class ParentSdtInfo
        {
            internal ParentSdtInfo(StructuredDocumentTag parentSdt, bool canNestInlineSdts)
            {
                ParentSdt = parentSdt;
                CanNestInlineSdts = canNestInlineSdts;
            }

            internal StructuredDocumentTag ParentSdt { get; }

            internal bool CanNestInlineSdts { get; }
        }
    }
}
