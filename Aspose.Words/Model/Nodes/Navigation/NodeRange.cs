// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/11/2009 by Dmitry Vorobyev

using System.Collections;
using System.Collections.Generic;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a range limited by two positions in the document.
    /// </summary>
    internal class NodeRange : IEnumerable<Node>
    {
        /// <summary>
        /// Ctor. Instantiates a node range with inclusive start and end points.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal NodeRange(Node start, Node end)
            : this(start, true, end, true)
        {
        }

        /// <summary>
        /// Ctor. Instantiates a node range with inclusive start and end points.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="includeStart"></param>
        /// <param name="end"></param>
        /// <param name="includeEnd"></param>
        internal NodeRange(Node start, bool includeStart, Node end, bool includeEnd)
            : this(start, includeStart, end, includeEnd, null)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="includeStart"></param>
        /// <param name="end"></param>
        /// <param name="includeEnd"></param>
        /// <param name="bookmarkStart"></param>
        internal NodeRange(
            Node start,
            bool includeStart,
            Node end,
            bool includeEnd,
            BookmarkStart bookmarkStart)
            : this(
                includeStart ? DocumentPosition.CreatePositionBefore(start) : DocumentPosition.CreatePositionAfter(start),
                includeEnd ? DocumentPosition.CreatePositionAfter(end) : DocumentPosition.CreatePositionBefore(end),
                bookmarkStart)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        internal NodeRange(DocumentPosition start, DocumentPosition end)
            : this(start, end, null)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="bookmarkStart"></param>
        internal NodeRange(
            DocumentPosition start,
            DocumentPosition end,
            BookmarkStart bookmarkStart)
        {
            Start = start;
            End = end;

            // Satisfy Java final.
            BookmarkTable = null;
            FirstTableColumnIndex = -1;
            LastTableColumnIndex = -1;

            if ((bookmarkStart != null) && bookmarkStart.IsColumn)
            {
                BookmarkTable = bookmarkStart.GetAncestor(NodeType.Table);
                if (BookmarkTable != null)
                {
                    FirstTableColumnIndex = bookmarkStart.FirstColumn;
                    LastTableColumnIndex = bookmarkStart.LastColumn;
                }
            }
        }

        /// <summary>
        /// If the start or end points of the range refer to runs and the position is non-negative,
        /// splits the runs so that any of the points would refer to a complete run.
        /// </summary>
        /// <returns>
        /// The newly inserted run before the <see cref="Start"/> or null if the <see cref="Start"/> was not actually split.
        /// </returns>
        internal Run Isolate()
        {
            const int invalidPositionOffset = -1;

            Run result = null;

            // Split the starting run.
            bool isStartRun = (Start.Node.NodeType == NodeType.Run);
            // No need to split the run if the offset points to the end. Nothing is taken from it.
            if (isStartRun && !Start.IsEnd)
            {
                Run run = (Run)Start.Node;

                int startOffset = Start.Offset;
                result = run.SplitBefore(startOffset);
                // WORDSNET-8573 Reset the length cached in the DocumentPosition instance to prevent NodeRangePosition.IsEof failure.
                Start.ResetLength();
                Start.MoveNodeStart();

                // Update the end position if it is in the same node.
                if (IsSameNode && (End.Offset != invalidPositionOffset))
                {
                    End.Offset -= startOffset;
                }
            }

            // Split the ending run.
            bool isEndRun = (End.Node.NodeType == NodeType.Run);
            if (isEndRun && (End.Offset > 0))
            {
                Run run = (Run)End.Node;
                run.SplitAfter(End.Offset);
                // WORDSNET-8573 Reset the length cached in the DocumentPosition instance to prevent NodeRangePosition.IsEof failure.
                End.ResetLength();
                End.MoveNodeEnd();
            }

            return result;
        }

        /// <summary>
        /// Accepts a <see cref="DocumentVisitor"/> object and calls its methods appropriately.
        /// At the moment only calls VisitXXXStart methods for composite nodes.
        /// </summary>
        internal void Accept(DocumentVisitor visitor, INodeModifier modifier, NodeExtractBehavior extractBehavior)
        {
            using (NodeRangeVisitingHelper helper = new NodeRangeVisitingHelper(this, visitor, modifier, extractBehavior))
                helper.SendVisitorToNodeRange();
        }

        /// <summary>
        /// Removes nodes in the range. Joins parent nodes if needed.
        /// </summary>
        /// <returns>A node that "follows" the range to mark the place where the range used to be.</returns>
        internal Node Remove()
        {
            Isolate();

            Node referenceNode = GetReferenceNode();
            NodeRemover.Remove(this, NodeJoinMode.JoinToPreviousSibling, false);

            return referenceNode;
        }

        private Node GetReferenceNode()
        {
            Node endNode = End.Node;

            if (End.IsStart)
                return endNode;

            return endNode.NextOrParent;
        }

        /// <summary>
        /// Returns true if both start and end nodes of the range belong to the same ancestor.
        /// </summary>
        internal bool IsInSameAncestor(NodeType ancestorType)
        {
            return GetSameAncestor(ancestorType) != null;
        }

        internal CompositeNode GetSameAncestor(NodeType ancestorType)
        {
            CompositeNode startAncestor = Start.Node.GetAncestor(ancestorType);
            if (startAncestor == null)
                return null;

            CompositeNode endAncestor = End.Node.GetAncestor(ancestorType);
            if (endAncestor == null)
                return null;

            return startAncestor == endAncestor
                ? startAncestor
                : null;
        }

        /// <summary>
        /// Returns true if both start and end nodes of the range belong to the same ancestor or are the same node.
        /// </summary>
        internal bool IsInSameAncestorOrSelf(NodeType ancestorType)
        {
            return
                (NodeUtil.GetAncestorOrSelf(Start.Node, ancestorType) ==
                NodeUtil.GetAncestorOrSelf(End.Node, ancestorType));
        }

        public IEnumerator<Node> GetEnumerator()
        {
            return new NodeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets whether the start or end point does not reference a node.
        /// </summary>
        internal bool IsVoid
        {
            get { return (Start.IsVoid || End.IsVoid); }
        }

        /// <summary>
        /// Gets whether the start and end points of the range reference the same node.
        /// </summary>
        internal bool IsSameNode
        {
            get { return (Start.IsSameNode(End)); }
        }

        /// <summary>
        /// Gets whether the start and end points of the range have the same parent.
        /// </summary>
        internal bool IsSameParent
        {
            get { return (Start.Node.ParentNode == End.Node.ParentNode); }
        }

        internal bool IsStartIncluded
        {
            get { return !Start.IsEnd; }
        }

        internal bool IsEndIncluded
        {
            get { return !End.IsStart; }
        }

        /// <summary>
        /// Gets the first point of the range.
        /// </summary>
        internal DocumentPosition Start { get; }

        /// <summary>
        /// Gets the last point of the range.
        /// </summary>
        internal DocumentPosition End { get; }

        /// <summary>
        /// Gets the <see cref="Document"/> the range belongs to.
        /// </summary>
        internal DocumentBase Document
        {
            get
            {
                if (IsVoid)
                    return null;

                return Start.Node.Document;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the range is a table column bookmark range
        /// i.e. it references columns in a table.
        /// </summary>
        internal bool IsTableColumnBookmarkRange
        {
            get { return (BookmarkTable != null); }
        }

        /// <summary>
        /// Returns the bookmarked table if the range is a table column bookmark range
        /// or <c>null</c> otherwise.
        /// </summary>
        internal Node BookmarkTable { get; }

        /// <summary>
        /// Returns the index of the first valid table column if the range is a table
        /// column bookmark range or <b>-1</b> otherwise.
        /// </summary>
        internal int FirstTableColumnIndex { get; }

        /// <summary>
        /// Returns the index of the last valid table column if the range is a table
        /// column bookmark range or <b>-1</b> otherwise.
        /// </summary>
        internal int LastTableColumnIndex { get; }

        /// <summary>
        /// Returns the first effective node taking into account inclusion.
        /// </summary>
        internal Node FirstRangeNode
        {
            get
            {
                DocumentPosition p = Start.Clone();
                if (!IsStartIncluded)
                    p.MoveNext(true);

                return p.Node;
            }
        }

        /// <summary>
        /// Gets indication of whether range has nothing inside.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                // For most nodes, starting and ending on the same position means emptiness.
                return (End.Node.NodeType != NodeType.Run)
                    ? Start.IsEqual(End)
                    // Run is an exception:
                    :  Start.IsSameNode(End) &&
                       (Start.Offset >= End.Offset || Start.IsEnd);
            }
        }

        /// <summary>
        /// A node range consisting of null node points.
        /// </summary>
        internal static readonly NodeRange Void = new NodeRange(DocumentPosition.Void, DocumentPosition.Void);

        /// <summary>
        /// Returns node which is ancestor for all the nodes in the range
        /// </summary>
        internal Node GetCommonAncestor()
        {
            DocumentPosition p = Start.Clone();

            Node ancestor = p.Node;
            if (!IsStartIncluded)
            {
                if (!p.MoveNext(true))
                    return null;

                if (p.Node == End.Node && !IsEndIncluded)
                    return null;

                ancestor = p.Node;
            }

            while (p.MoveNext(true))
            {
                bool isEndNode = (p.Node == End.Node);
                if (isEndNode && !IsEndIncluded)
                    break;

                // If we moved to parent, its our new ancestor
                if (ancestor.ParentNode == p.Node)
                    ancestor = p.Node;

                if (isEndNode)
                    break;
            }

            return ancestor;
        }

#if DEBUG
        /// <summary>
        /// DEBUG. Prints nodes in the collection.
        /// </summary>
        [CppSkipEntity("Debug method for .Net")]
        internal void dd()
        {
            IEnumerator it = GetEnumerator();
            while (it.MoveNext())
            {
                Node node = (Node) it.Current;
                if (node == null)
                {
                    Debug.WriteLine("null");
                    continue;
                }
                int level = -1;
                for (Node temp = node; temp != null; temp = temp.ParentNode)
                    level++;
                Debug.WriteLine(new string(' ', 4 * level) + node.ToString());
            }
        }
#endif
    }
}
