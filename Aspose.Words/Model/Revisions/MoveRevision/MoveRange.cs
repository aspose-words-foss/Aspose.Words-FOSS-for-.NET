// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2017 by Andrey Noskov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Wrapper class for MoveRangeStart, MoveRangeEnd nodes.
    /// </summary>
    internal class MoveRange
    {
        internal MoveRange(MoveRangeStart start)
        {
            Start = start;
            End = Start != null
                ? MoveRangeFinder.FindMoveRangeEnd(start.Document, start.Id)
                : null;
        }

        internal MoveRange(Node node, bool isMoveTo)
        {
            Start = MoveRangeFinder.FindMoveRangeStart(node, isMoveTo);
            End = Start != null
                ? MoveRangeFinder.FindMoveRangeEnd(node.Document, Start.Id)
                : null;
        }

        /// <summary>
        /// Move range start.
        /// </summary>
        internal MoveRangeStart Start { get; }

        /// <summary>
        /// Move range end.
        /// </summary>
        internal MoveRangeEnd End { get; }

        internal bool IsDefined
        {
            get
            {
                if (Start == null)
                    return false;

                if (End == null)
                    return false;

                if (Start.IsRemoved)
                    return false;

                if (End.IsRemoved)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Indicates whether move range is empty.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                if (!IsDefined)
                    return false;

                // Check that between move range start and end doesn't exist any node with corresponding move revision.
                // WORDSNET-15479 Also we can accept only one revision, in this case we must keep move range.
                // Such complex check because in some cases(see TestJira14682) move range start
                // might be located at the end of the previous node.
                Node node = Start.NextPreOrder(End);
                while (node != null)
                {
                    if (RevisionUtil.HasMoveRevision(node, IsMoveTo))
                        return false;

                    node = node.NextPreOrder(End);
                }

                return true;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this range is a move-to range.
        /// </summary>
        internal bool IsMoveTo
        {
            get { return Start is MoveToRangeStart; }
        }

        /// <summary>
        /// Removes move range start/end nodes.
        /// </summary>
        internal void Remove()
        {
            Start.Remove();
            End.Remove();
        }
    }
}
