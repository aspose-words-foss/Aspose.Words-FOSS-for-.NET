// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using System;
using Aspose.JavaAttributes;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Finds a move range by Id or by Name.
    /// </summary>
    internal class MoveRangeFinder : DocumentVisitor
    {
        private MoveRangeFinder(int moveRangeId, bool isLookingForStart)
            : this(moveRangeId, isLookingForStart, string.Empty, false, MoveRevisionType.MoveTo)
        {
        }

        private MoveRangeFinder(string moveRangeName, bool isLookingForStart, MoveRevisionType revisionType)
            : this(-1, isLookingForStart, moveRangeName, true, revisionType)
        {
        }

        private MoveRangeFinder(
            int moveRangeId,
            bool isLookingForStart,
            string moveRangeName,
            bool searchByName,
            MoveRevisionType revisionType)
        {
            mMoveRangeId = moveRangeId;
            mIsLookingForStart = isLookingForStart;
            mMoveRangeName = moveRangeName;
            mDoSearchByName = searchByName;
            mRevisionType = revisionType;
        }

        internal static MoveRange FindMoveRange(Node node, bool isMoveTo)
        {
            return new MoveRange(node, isMoveTo);
        }

        /// <summary>
        /// Finds move range start.
        /// </summary>
        internal static MoveRangeStart FindMoveRangeStart(Node node, bool isMoveTo)
        {
            NodeType nodeType = isMoveTo ? NodeType.MoveToRangeStart : NodeType.MoveFromRangeStart;
            // Need to return range around end of a paragraph. Probably it is the same with other composites.
            if (node.IsComposite)
            {
                // Search for move range start as child of the node.
                MoveRangeStart start = (MoveRangeStart)((CompositeNode)node).GetChild(nodeType, -1, true);
                if (start != null)
                {
                    Node end = FindMoveRangeEnd(node.Document, start.Id);
                    // Looks like move ranges cannot be nested: if the range is ended inside the input node,
                    // just return null.
                    return (end != null) && !end.IsAncestorNode(node)
                        ? start
                        : null;
                }
            }

            return (MoveRangeStart)node.PreviousPreOrderOfType(node.Document, nodeType);
        }

        /// <summary>
        /// Helper function to find an move range start in the document by Id. Returns the move range start or null.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal static MoveRangeStart FindMoveRangeStart(Node node, int moveRangeId)
        {
            MoveRangeFinder finder = new MoveRangeFinder(moveRangeId, true);
            node.Accept(finder);

            return finder.mMoveRangeStart;
        }

        /// <summary>
        /// Helper function to find a move range start in the document by Name. Returns the move range start or null.
        /// </summary>
        /// <remarks>
        /// MoveRange Name is unique for a pair of MoveFromRange and MoveToRange. That why we should specify MoveRevisionType,
        /// when searching MoveToRangeStart or MoveFromRangeStart by Name.
        /// </remarks>
        [JavaConvertCheckedExceptions]
        internal static MoveRangeStart FindMoveRangeStartByName(Node node, string moveRangeName, MoveRevisionType revisionType)
        {
            MoveRangeFinder finder = new MoveRangeFinder(moveRangeName, true, revisionType);
            node.Accept(finder);

            return finder.mMoveRangeStart;
        }

        /// <summary>
        /// Helper function to find an move range end in the document by Id. Returns the move range end or null.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal static MoveRangeEnd FindMoveRangeEnd(Node node, int moveRangeId)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            MoveRangeFinder finder = new MoveRangeFinder(moveRangeId, false);
            node.Accept(finder);

            return finder.mMoveRangeEnd;
        }

        internal override VisitorAction VisitMoveToRangeStart(MoveRangeStart moveToRangeStart)
        {
            return VisitRangeStart(moveToRangeStart, MoveRevisionType.MoveTo);
        }

        internal override VisitorAction VisitMoveToRangeEnd(MoveRangeEnd moveToRangeEnd)
        {
            return VisitRangeEnd(moveToRangeEnd);
        }

        internal override VisitorAction VisitMoveFromRangeStart(MoveRangeStart moveFromRangeStart)
        {
            return VisitRangeStart(moveFromRangeStart, MoveRevisionType.MoveFrom);
        }

        internal override VisitorAction VisitMoveFromRangeEnd(MoveRangeEnd moveFromRangeEnd)
        {
            return VisitRangeEnd(moveFromRangeEnd);
        }

        private VisitorAction VisitRangeStart(MoveRangeStart moveRangeStart, MoveRevisionType revisionType)
        {
            if (!mIsLookingForStart)
                return VisitorAction.Continue;

            if (mDoSearchByName)
            {
                if ((mRevisionType == revisionType) && (mMoveRangeName == moveRangeStart.Name))
                {
                    mMoveRangeStart = moveRangeStart;
                    return VisitorAction.Stop;
                }
            }
            else
            {
                if (mMoveRangeId == moveRangeStart.Id)
                {
                    mMoveRangeStart = moveRangeStart;
                    return VisitorAction.Stop;
                }
            }

            return VisitorAction.Continue;
        }

        private VisitorAction VisitRangeEnd(MoveRangeEnd moveRangeEnd)
        {
            if (mIsLookingForStart)
                return VisitorAction.Continue;

            if (mMoveRangeId != moveRangeEnd.Id)
                return VisitorAction.Continue;

            mMoveRangeEnd = moveRangeEnd;
            return VisitorAction.Stop;
        }


        private readonly string mMoveRangeName;
        private readonly int mMoveRangeId;
        private readonly bool mIsLookingForStart;
        private readonly bool mDoSearchByName;
        private readonly MoveRevisionType mRevisionType;
        private MoveRangeStart mMoveRangeStart;
        private MoveRangeEnd mMoveRangeEnd;
    }
}
