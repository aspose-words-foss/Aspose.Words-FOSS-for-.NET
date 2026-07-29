// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2009 by Dmitry Vorobyev

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Enumerates a range of nodes.
    /// </summary>
#if CPLUSPLUS
    public
#else
    internal
#endif
    class NodeEnumerator : IEnumerator<Node>, IDocumentPositionListener
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal NodeEnumerator(NodeRange range)
            : this(range, false)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        protected NodeEnumerator(NodeRange range, bool stopAtInvalidRangeNodes)
        {
            Range = range;
            mStopAtInvalidRangeNodes = stopAtInvalidRangeNodes;
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        internal bool MoveToNextNode()
        {
            return MoveToNextNode(true);
        }

        internal bool MoveToNextNode(bool isDeep)
        {
            return MoveToNextNode(isDeep, true);
        }

        internal bool MoveToNextNode(bool isDeep, bool isByNode)
        {
            if (Range.IsVoid || Range.IsEmpty)
                return false;

            // First move?
            if (CurrentPosition == null)
            {
                CurrentPosition = Range.Start.Clone();
                if (Range.IsStartIncluded && CanStopAtCurrentNode())
                    return true;
            }

            if (IsEof(true))
                return false;

            do
            {
                Node oldNode = CurrentNode;
                do
                {
                    if (!CurrentPosition.Move(null, true, isDeep, true, !isByNode, true, this))
                        return false;

                    if (IsEof(isByNode))
                        return false;
                }
                while (isByNode && ((CurrentPosition.IsEnd) || (CurrentNode == oldNode)));
            }
            while (!CanStopAtCurrentNode());

            return true;
        }

        private bool IsEof(bool isByNode)
        {
            // If this is not the last node, it is certainly not the end.
            if (!IsEndNode)
                return false;

            // If this is the last node and it is not the same as the first node and the end is included, it's EOF.
            if (!IsStartNode && !Range.IsEndIncluded)
                return true;

            // If this is the last node and we move by node, exit.
            if (CurrentPosition.IsEnd && isByNode)
                return true;

            // We may need to stop inside a run.
            if (!isByNode && CurrentPosition.IsEqual(Range.End))
                return true;

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the enumeration can stop at the current node i.e. it is valid or
        /// stops at invalid nodes are requested.
        /// </summary>
        private bool CanStopAtCurrentNode()
        {
            return (mStopAtInvalidRangeNodes || IsCurrentNodeValid());
        }

        /// <summary>
        /// Returns a value indicating whether the current node and all of its descendants are valid for enumeration
        /// i.e. belong to a valid column in case of a table column bookmark range being enumerated.
        /// </summary>
        protected bool IsCurrentNodeValid()
        {
            return IsRangeNodeValid(CurrentNode);
        }

        /// <summary>
        /// Returns a value indicating whether the specified node within the enumerated range and all of its descendants
        /// are valid for enumeration i.e. belong to a valid column in case of a table column bookmark range being enumerated.
        /// </summary>
        /// <remarks>
        /// Always returns <c>true</c> in case of a non-table-column-bookmark range being enumerated even if the node
        /// does not fall within the range.
        /// </remarks>
        protected bool IsRangeNodeValid(Node rangeNode)
        {
            // If a non-table-column-bookmark range is enumerated then the node is always valid.
            // If the node is the table of a table column bookmark range being enumerated then it is valid.
            if (!Range.IsTableColumnBookmarkRange || (rangeNode == Range.BookmarkTable))
                return true;

            // We may deal with nested tables. Let's consider the top nested table in this case as
            // all of its descendants are valid or invalid together with it.
            Node node = rangeNode;
            while (true) // break inside.
            {
                Node parentTable = node.GetAncestor(NodeType.Table);
                if (parentTable == null)
                    return false;

                if (parentTable == Range.BookmarkTable)
                    break;

                node = parentTable;
            }

            // If the node is a row of the table of a table column bookmark range being enumerated then it is valid.
            if (node.NodeLevel == NodeLevel.Row)
                return !NodeUtil.IsCrossStructureAnnotation(node);

            // Get a cell of the table of a table column bookmark range being enumerated which is an ancestor of the node and
            // then determine whether it is valid or invalid together with all of its descendants depending on a column index.
            Node cellLevelNode = (node.NodeLevel == NodeLevel.Cell) ? node : node.GetAncestor(NodeType.Cell);
            if (cellLevelNode == null)
                return false;
            // Use parent SDT if the cell inside it.
            while (cellLevelNode.ParentNode.NodeLevel == NodeLevel.Cell)
                cellLevelNode = cellLevelNode.ParentNode;

            // Get nearest left cell if node is cross structure annotation.
            bool isAnnotation = NodeUtil.IsCrossStructureAnnotation(cellLevelNode);
            if (isAnnotation)
            {
                cellLevelNode = cellLevelNode.PreviousNonAnnotationSibling;
                if (cellLevelNode == null) // the node is before first column
                    return false;
            }
            // If the node is annotation, we have moved to cell at left of it. So, end bound column should not be
            // included in check at this case: correct end bound index for it.
            int lastValidColumnIndex = Range.LastTableColumnIndex - (isAnnotation ? 1 : 0);

            int columnIndex = cellLevelNode.ParentNode.IndexOfChildByDescendant(cellLevelNode, true);
            return ((columnIndex >= Range.FirstTableColumnIndex) && (columnIndex <= lastValidColumnIndex));
        }

        /// <summary>
        /// Returns a value indicating whether the specified node is partially valid i.e. some of its descendants are valid
        /// but some of them are not.
        /// </summary>
        /// <remarks>
        /// Always returns <c>false</c> in case of a non-table-column-bookmark range being enumerated.
        /// </remarks>
        private bool IsRangeNodePartiallyValid(Node rangeNode)
        {
            // If a non-table-column-bookmark range is enumerated then the node is always fully valid.
            if (!Range.IsTableColumnBookmarkRange)
                return false;

            // Only the table of a table column bookmark range being enumerated and its rows are partially valid
            // other nodes are fully valid or fully invalid.
            Node bookmarkTable = Range.BookmarkTable;
            return ((rangeNode == bookmarkTable) ||
                    (NodeUtil.IsRowLevelNode(rangeNode) && !NodeUtil.IsCrossStructureAnnotation(rangeNode) &&
                        (rangeNode.FirstNonMarkupParentNode == bookmarkTable)));
        }

        /// <summary>
        /// Extracts the current node. Correctly cuts the node text accordingly to the source range if needed.
        /// </summary>
        internal Node ExtractCurrentNode(INodeModifier modifier, NodeExtractBehavior behavior)
        {
            bool cloneNode = NeedCloneNodeOnExtract(behavior);
            bool modifyChildren = NeedModifyChildrenOnExtract(behavior);
            return ExtractRangeNode(CurrentNode, modifier, null, cloneNode, modifyChildren);
        }

        /// <summary>
        /// Extracts the specified node within the enumerated range. Correctly cuts the node text accordingly to
        /// the source range if needed.
        /// </summary>
        /// <remarks>
        /// Does not check whether the node falls within the range being enumerated.
        /// </remarks>
        protected Node ExtractRangeNode(
            Node rangeNode,
            INodeModifier modifier,
            INodeCloningListener cloningListener,
            bool cloneNode,
            bool modifyChildren)
        {
            Node node = ExtractRangeNode(rangeNode, cloningListener, cloneNode, modifyChildren, true);
            if (node == null)
                return null;

            if (rangeNode.NodeType == NodeType.Run)
            {
                DocumentPosition startPosition = Range.Start;
                DocumentPosition endPosition = Range.End;
                Run run = (Run)node;

                int startOffset = IsStartNode ? startPosition.Offset : 0;
                int endOffset = IsEndNode
                    // WORDSNET-26019 ArgumentOutOfRangeException when updating TOC
                    ? endPosition.IsEnd ? run.Text.Length : endPosition.Offset
                    : run.Text.Length;
                int cutLength = endOffset - startOffset;

                // Cut if needed only.
                if (cutLength < run.Text.Length)
                    node = run.CutText(startOffset, cutLength, !cloneNode);
            }

            if (modifier != null)
                node = modifier.Modify(rangeNode, node, modifyChildren, cloningListener);

            return node;
        }

        /// <summary>
        /// Extracts the specified node within the enumerated range.
        /// </summary>
        /// <remarks>
        /// Does not check whether the node falls within the range being enumerated.
        /// </remarks>
        private Node ExtractRangeNode(
            Node rangeNode,
            INodeCloningListener cloningListener,
            bool cloneNode,
            bool cloneChildren,
            bool checkValidity)
        {
            if (checkValidity && !IsRangeNodeValid(rangeNode))
                return null;

            if (NeedSkipRangeNodeExtract(rangeNode))
                return null;

            // If the node should not be cloned, simply return it. Its descendants are not filtered in this case.
            if (!cloneNode)
                return rangeNode;

            // Otherwise, clone the node with its descendants if needed, performing all of the required checks.
            Node clone = rangeNode.Clone(false, cloningListener);

            if (cloneChildren && rangeNode.IsComposite)
            {
                CompositeNode parent = (CompositeNode)rangeNode;
                CompositeNode parentClone = (CompositeNode)clone;

                // Do not check validity for descendants, if the parent node is fully valid.
                checkValidity = IsRangeNodePartiallyValid(rangeNode);

                for (Node child = parent.FirstChild; child != null; child = child.NextSibling)
                {
                    Node childClone = ExtractRangeNode(child, cloningListener, true, true, checkValidity);
                    if (childClone != null)
                        parentClone.AppendChildForLoad(childClone);
                }
            }

            return clone;
        }

        /// <summary>
        /// Returns a value indicating whether the specified range node should be skipped on extract.
        /// </summary>
        protected virtual bool NeedSkipRangeNodeExtract(Node rangeNode)
        {
            return false;
        }

        /// <summary>
        /// Returns a value indicating if the current node should be cloned on extract
        /// due to the specified extracting behavior.
        /// </summary>
        private bool NeedCloneNodeOnExtract(NodeExtractBehavior behavior)
        {
            switch (behavior)
            {
                case NodeExtractBehavior.ModifyChildrenCloneAll:
                    return true;
                case NodeExtractBehavior.DontModifyChildrenCloneRuns:
                    return (CurrentNode.NodeType == NodeType.Run);
                default:
                    throw new ArgumentOutOfRangeException("behavior");
            }
        }

        /// <summary>
        /// Returns a value indicating if child nodes of the current node should be modified on extract
        /// due to the specified extracting behavior.
        /// </summary>
        private static bool NeedModifyChildrenOnExtract(NodeExtractBehavior behavior)
        {
            switch (behavior)
            {
                case NodeExtractBehavior.ModifyChildrenCloneAll:
                    return true;
                case NodeExtractBehavior.DontModifyChildrenCloneRuns:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("behavior");
            }
        }

        protected virtual void OnMoved(DocumentPositionMovement movement)
        {
            //  Implementation is not required.
        }

        protected static Node GetValidParent(Node node)
        {
            if (node == null)
                return null;

            return (node.NodeType != NodeType.Body ? node : node.ParentNode);
        }

        [JavaConvertCheckedExceptions]
        protected static Node GetValidChild(Node node)
        {
            if (node == null)
                return null;

            // If we move down from the section level, we should enter body because earlier we have exited body, too.
            // Section breaks are only allowed in a body.
            switch (node.NodeType)
            {
                case NodeType.HeaderFooter:
                {
                    Body body = ((Section)node.ParentNode).Body;
                    return (body != null)
                        ? body.FirstChild
                        : null;
                }
                case NodeType.Body:
                {
                    Body body = (Body)node;
                    // WORDSNET-27816 if Body is empty, we go to the next node in the hierarchy.
                    return body.HasChildNodes
                        ? body.FirstChild
                        : body.NextPreOrder(body.Document);
                }
                default:
                    return node;
            }
        }

        [CppForceSharedApi]
        bool IEnumerator.MoveNext()
        {
            return MoveToNextNode();
        }

        public void Reset()
        {
            CurrentPosition = null;
        }

        [CppForceSharedApi]
        Node IEnumerator<Node>.Current
        {
            get { return CurrentNode; }
        }

        [JavaDelete, CppSkipEntity]
        object IEnumerator.Current
        {
            get { return CurrentNode; }
        }

        [CppForceSharedApi]
        void IDocumentPositionListener.NotifyMoved(DocumentPositionMovement movement)
        {
            switch (movement)
            {
                case DocumentPositionMovement.Above:
                {
                    Node newNode = GetValidParent(CurrentNode);
                    if (newNode != CurrentNode)
                        CurrentPosition = DocumentPosition.CreatePositionAfter(newNode);
                    break;
                }
                case DocumentPositionMovement.Below:
                {
                    Node newNode = GetValidChild(CurrentNode);
                    // WORDSNET-27816 here we jump over an empty Body.
                    if ((CurrentNode.NodeType == NodeType.Body) && (newNode == null))
                        CurrentPosition.MoveNodeEnd();
                    else if (newNode != CurrentNode)
                        CurrentPosition = DocumentPosition.CreatePositionBefore(newNode);
                    break;
                }
                default:
                    break;
            }

            OnMoved(movement);
        }

        /// <summary>
        /// Gets whether the current node is the start node of the range.
        /// </summary>
        private bool IsStartNode
        {
            get { return Range.Start.IsSameNode(CurrentPosition); }
        }

        /// <summary>
        /// Gets whether the current node is the end node of the range.
        /// </summary>
        protected bool IsEndNode
        {
            get { return Range.End.IsSameNode(CurrentPosition); }
        }

        /// <summary>
        /// Gets the node range being enumerated.
        /// </summary>
        internal NodeRange Range { get; }

        internal Node CurrentNode
        {
            [CppConstMethod]
            get { return (CurrentPosition != null) ? CurrentPosition.Node : null; }
        }

        internal DocumentPosition CurrentPosition { get; set; }

        private readonly bool mStopAtInvalidRangeNodes;
    }
}
