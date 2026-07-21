// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/09/2018 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Words.Markup;

namespace Aspose.Words.Revisions
{
    internal class MoveRangeRevisionHelper
    {
        internal MoveRangeRevisionHelper(MoveRange range)
        {
            Range = range;
            mStartBound = AnnotationUtil.GetAnnotationRangeBound(range.Start, true) ?? new RangeBound(range.Start, true);
            mEndBound = AnnotationUtil.GetAnnotationRangeBound(range.End, false) ?? new RangeBound(range.End, true);
            CalcBoundNodes();
        }

        /// <summary>
        /// Returns <c>true</c> if the specified structured document tag is in a continuous chain of nested
        /// structured document tags started at the top node of begin of the move range.
        /// </summary>
        internal bool IsFirstSdt(StructuredDocumentTag sdt)
        {
            if (TopStartNode.NodeType != NodeType.StructuredDocumentTag)
                return false;

            while ((sdt != null) &&
                (sdt != TopStartNode) &&
                ((sdt.PreviousNonAnnotationSibling == null) || (Range.Start.NextNonAnnotationSibling == sdt) ||
                    Range.Start.IsAncestorNode(sdt)) &&
                (sdt.ParentNode.NodeType == NodeType.StructuredDocumentTag) &&
                (!sdt.IsAbove(Range.Start) || Range.Start.IsAncestorNode(sdt)))
                sdt = (StructuredDocumentTag)sdt.ParentNode;

            return sdt == TopStartNode;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified structured document tag is in a continuous chain of nested
        /// structured document tags at the end of the move range.
        /// </summary>
        internal bool IsLastSdt(StructuredDocumentTag sdt)
        {
            if (TopEndNode.NodeType != NodeType.StructuredDocumentTag)
                return false;

            while ((sdt != null) &&
                (sdt != TopEndNode) &&
                (sdt.ParentNode.NodeType == NodeType.StructuredDocumentTag) &&
                ((sdt.NextNonAnnotationSibling == null) || (Range.End.PreviousNonAnnotationSibling == sdt) ||
                    Range.End.IsAncestorNode(sdt)) &&
                sdt.IsAbove(Range.End))
                sdt = (StructuredDocumentTag)sdt.ParentNode;

            return sdt == TopEndNode;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified paragraph is the last paragraph in the move range, and top end node of
        /// the move range is this paragraph or its ancestor.
        /// </summary>
        internal bool IsLastParagraph(Paragraph para)
        {
            if (TopEndNode == para)
                return true;
            if (!TopEndNode.IsComposite)
                return false;

            List<Node> paragraphs = ((CompositeNode)TopEndNode).GetChildNodes(NodeType.Paragraph, true).ToNodeList();
            for (int i = paragraphs.Count - 1; i >= 0; i--)
            {
                Node node = paragraphs[i];
                if (node.IsAbove(Range.End))
                    return node == para;
            }

            return false;
        }

        /// <summary>
        /// Calculates real bound nodes of the range.
        /// </summary>
        private void CalcBoundNodes()
        {
            StartNode = GetStartNodeInternal();
            EndNode = GetEndNodeInternal();
            Node start = StartNode;
            Node end = EndNode;

            // Move start/end up to be in same node level.
            if (start != end)
            {
                Node ancestor = Node.GetCommonAncestor(start, end);
                if (ancestor == null)
                    throw new InvalidOperationException("Wrong move range.");

                if (start != ancestor)
                {
                    while (start.ParentNode != ancestor)
                        start = start.ParentNode;
                }

                if (end != ancestor)
                {
                    while (end.ParentNode != ancestor)
                        end = end.ParentNode;
                }
            }

            Node node;
            // If range start is last child of 'start', move 'start' to the next node.
            if (StartNode.IsAncestorNode(start) && (end != start) && !ContainsMoveRevision(start))
            {
                node = StartNode;
                while ((node != start) &&
                       (node.NextNonAnnotationSibling == null) &&
                       // Do not exclude a paragraph from the range if it has move revision of the same type.
                       ((node.ParentNode.NodeType != NodeType.Paragraph) || !HasMoveRevision(node.ParentNode)))
                    node = node.ParentNode;

                if (node == start)
                    start = start.NextSibling;
            }

            // If range end is first child of 'end', move 'end' to the previous node.
            if (EndNode.IsAncestorNode(end) && (end != start) && !ContainsMoveRevision(end))
            {
                node = EndNode;
                while ((node != end) && (node.PreviousNonAnnotationSibling == null))
                    node = node.ParentNode;

                if (node == end)
                    end = end.PreviousSibling;
            }

            // Skip annotations.
            node = start;
            while (NodeUtil.IsCrossStructureAnnotation(node) && (node != end))
                node = node.NextSibling;

            if (node != end)
                start = node;

            // Skip annotations.
            node = end;
            while (NodeUtil.IsCrossStructureAnnotation(node) && (node != start))
                node = node.PreviousSibling;

            if (node != start)
                end = node;

            TopStartNode = start;
            TopEndNode = end;
            RangeParent = start.ParentNode;

            // Fix start/end nodes if outside of top nodes.
            if (StartNode.IsAbove(TopStartNode))
            {
                StartNode = TopStartNode;
                // Move start to deepest child since start/end nodes are in order from deepest child to parents.
                while (StartNode.IsComposite && ((CompositeNode)StartNode).HasChildNodes)
                    StartNode = ((CompositeNode)StartNode).FirstChild;
            }

            if (!NodeUtil.IsAncestorOrSelf(EndNode, TopEndNode) && TopEndNode.IsAbove(EndNode))
                EndNode = TopEndNode;
        }

        private bool ContainsMoveRevision(Node node)
        {
            if (HasMoveRevision(node))
                return true;

            if (!node.IsComposite)
                return false;

            foreach (Node child in (CompositeNode)node)
            {
                if (ContainsMoveRevision(child))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node has move revision of the same type as this move range.
        /// </summary>
        private bool HasMoveRevision(Node node)
        {
            IMoveTrackableNode trackable = node as IMoveTrackableNode;
            if (trackable == null)
                return false;

            MoveRevision revision = IsMoveTo ? trackable.MoveToRevision : trackable.MoveFromRevision;

            return revision != null;
        }

        /// <summary>
        /// Gets start node calculated for range bound.
        /// </summary>
        private Node GetStartNodeInternal()
        {
            if (mStartBound.DisplacingNode == null)
            {
                Node node = mStartBound.Node;
                if (mStartBound.IsNodeIncluded || NodeUtil.IsCrossStructureAnnotation(node))
                    return node;

                while ((node != null) && (node.NextSibling == null))
                    node = node.ParentNode;

                return node != null ? node.NextSibling : mStartBound.Node;
            }

            // Do not include displacing SDT, if range start bound is end of the SDT.
            bool isSdtIncluded = mStartBound.IsAnnotationAChildOfSdt
                ? mStartBound.DisplacedByType == DisplacedByType.Prev
                : mStartBound.DisplacedByType == DisplacedByType.Next;
            return isSdtIncluded ? mStartBound.DisplacingNode : mStartBound.DisplacedAnnotation;
        }

        /// <summary>
        /// Gets end node calculated for range bound.
        /// </summary>
        private Node GetEndNodeInternal()
        {
            if (mEndBound.DisplacingNode == null)
            {
                Node node = mEndBound.Node;
                if (mEndBound.IsNodeIncluded || NodeUtil.IsCrossStructureAnnotation(node))
                    return node;

                while ((node != null) && (node.PreviousSibling == null))
                    node = node.ParentNode;

                return node != null ? node.PreviousSibling : mEndBound.Node;
            }

            // Do not include displacing SDT, if range end bound is begin of the SDT.
            bool isSdtIncluded = mEndBound.IsAnnotationAChildOfSdt
                ? mEndBound.DisplacedByType == DisplacedByType.Next
                : mEndBound.DisplacedByType == DisplacedByType.Prev;
            return isSdtIncluded ? mEndBound.DisplacingNode : mEndBound.DisplacedAnnotation;
        }

        /// <summary>
        /// Indicates whether any unrevised node is preserved during accepting/rejecting revisions.
        /// </summary>
        internal bool IsNonRevisedNodePreserved
        {
            get
            {
                return IsMiddleParagraphPreserved && IsMiddleSdtPreserved;
            }
        }

        /// <summary>
        /// Indicates whether the first SDT if it has no move revision is preserved during accepting/rejecting revisions.
        /// See WORDSNET-15250 for info of MS Word behaviour.
        /// </summary>
        internal bool IsFirstSdtPreserved
        {
            get
            {
                if (IsMiddleSdtPreserved)
                    return true;
                if (!IsRangeStartInFirstSdt)
                    return false;

                return !IsRangeEndDisplaced || IsRangeEndInLastSdt || !IsInsideSdt;
            }
        }

        /// <summary>
        /// Indicates whether a middle SDT that has no move revision is preserved during accepting/rejecting revisions.
        /// See WORDSNET-15250 for info of MS Word behaviour.
        /// </summary>
        internal bool IsMiddleSdtPreserved
        {
            get
            {
                return IsInsideSdt && IsRangeEndDisplaced && !IsRangeEndInLastSdt;
            }
        }

        /// <summary>
        /// Indicates whether the last SDT if it has no move revision is preserved during accepting/rejecting revisions.
        /// See WORDSNET-15250 for info of MS Word behaviour.
        /// </summary>
        internal bool IsLastSdtPreserved
        {
            get
            {
                if (IsMiddleSdtPreserved)
                    return true;

                if (IsRangeEndDisplaced && IsRangeEndInLastSdt)
                    return true;

                if (!IsRangeEndInLastSdt && Range.End.IsAncestorNode(TopEndNode))
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Indicates whether the last SDT when has no children is nested in the first SDT of the move range after
        /// accepting/rejecting revisions.
        /// </summary>
        /// <remarks>
        /// See WORDSNET-15250 for info of MS Word behaviour.
        /// </remarks>
        internal bool IsLastSdtNestedInFirst
        {
            get
            {
                return IsRangeStartInFirstSdt && IsRangeEndDisplaced && IsRangeEndInLastSdt;
            }
        }

        /// <summary>
        /// Indicates whether a non-last paragraph that has no move revision is preserved during accepting/rejecting
        /// revisions.
        /// See WORDSNET-15250 for info of MS Word behaviour.
        /// </summary>
        internal bool IsMiddleParagraphPreserved
        {
            get
            {
                return IsInsideSdt && IsRangeEndDisplaced && !IsRangeEndInLastSdt;
            }
        }

        /// <summary>
        /// Indicates whether the last paragraph that has no move revision is preserved during accepting/rejecting
        /// revisions.
        /// See WORDSNET-15250 for info of MS Word behaviour.
        /// </summary>
        internal bool IsLastParagraphPreserved
        {
            get
            {
                if (IsMiddleParagraphPreserved)
                    return true;

                // May be preserved only if it is a child of the last SDT.
                return (TopEndNode.NodeType == NodeType.StructuredDocumentTag) && IsRangeEndDisplaced;
            }
        }

        /// <summary>
        /// Returns the processing move range.
        /// </summary>
        internal MoveRange Range { get; }

        /// <summary>
        /// Indicates whether this is a move-to or move-from range.
        /// </summary>
        internal bool IsMoveTo
        {
            get { return Range.IsMoveTo; }
        }

        /// <summary>
        /// Gets topmost-level non-annotation node that is included at least partially into the move range at start.
        /// </summary>
        internal Node TopStartNode { get; private set; }

        /// <summary>
        /// Gets topmost-level non-annotation node that is included at least partially into the move range at end.
        /// </summary>
        internal Node TopEndNode { get; private set; }

        /// <summary>
        /// Returns node that completely contains the move range.
        /// </summary>
        internal CompositeNode RangeParent { get; private set; }

        /// <summary>
        /// Returns real start node of the range in order from deepest child to parent.
        /// </summary>
        internal Node StartNode { get; private set; }

        /// <summary>
        /// Returns real end node of the range in order from deepest child to parent.
        /// </summary>
        internal Node EndNode { get; private set; }

        /// <summary>
        /// MS Word may nest the last SDT of a move range into the first SDT on accepting revisions. This is
        /// an additional flag for <see cref="IsLastSdtNestedInFirst"/> for a case when the first and the last
        /// SDTs become inline level on accepting revisions.
        /// See the cases 2 (not nested) and 8 (nested) of WORDSNET-15250
        /// </summary>
        internal bool CanNestInlineSdts
        {
            get { return IsInsideSdt; }
        }

        /// <summary>
        /// Returns <c>true</c> if parent of the range is an SDT.
        /// </summary>
        private bool IsInsideSdt
        {
            get { return RangeParent.NodeType == NodeType.StructuredDocumentTag; }
        }

        /// <summary>
        /// Returns <c>true</c> if top start node of the range is an SDT and range start node is a descendant of it.
        /// </summary>
        private bool IsRangeStartInFirstSdt
        {
            get
            {
                if (TopStartNode.NodeType != NodeType.StructuredDocumentTag)
                    return false;

                if (!Range.Start.IsAncestorNode(TopStartNode))
                    return false;

                if (Range.Start.ParentNode.NodeType != NodeType.StructuredDocumentTag)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the range end node is displaced by a custom XML (SDT) node.
        /// </summary>
        private bool IsRangeEndDisplaced
        {
            get { return mEndBound.DisplacingNode == TopEndNode; }
        }

        /// <summary>
        /// Returns <c>true</c> if top end node of the range is an SDT and range end node is a descendant of it.
        /// </summary>
        private bool IsRangeEndInLastSdt
        {
            get
            {
                if (TopEndNode.NodeType != NodeType.StructuredDocumentTag)
                    return false;

                if (!Range.End.IsAncestorNode(TopEndNode))
                    return false;

                if (Range.End.ParentNode.NodeType == NodeType.StructuredDocumentTag)
                    return true;

                if (Range.End.DisplacedBy == DisplacedByType.Next)
                    return true;

                return false;
            }
        }

        private readonly RangeBound mStartBound;
        private readonly RangeBound mEndBound;
    }
}
