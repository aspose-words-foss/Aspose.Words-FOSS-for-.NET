// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2011 by Dmitry Matveenko

using System.Collections.Generic;
using Aspose.Words.Fields;

namespace Aspose.Words
{
    /// <summary>
    /// Implements removing field nodes and field codes from a given node range.
    /// Field result is not removed.
    /// </summary>
    internal class FieldRemover
    {
        private FieldRemover()
        {
            // It is called from static method only.
        }

        /// <summary>
        /// Replaces fields with field results in the given node range. Preserves fields filtered by <paramref name="filter"/>.
        /// </summary>
        /// <param name="range">The node range.</param>
        /// <param name="filter">Determines wherever certain field should be replaced with result or not.</param>
        /// <remarks>
        /// A field must start inside the range to be processed correctly.
        /// Any stranded field characters (for example, a field separator without a matching field start inside the range), are removed.
        /// </remarks>
        /// <returns>
        /// A NodeRange object containing the remaining nodes of the original range. Null is returned if everything is removed.
        /// </returns>
        internal static NodeRange ReplaceFieldsWithResults(NodeRange range, IFieldRemoverFilter filter)
        {
            if (range == null)
            {
                // Nothing to do.
                return null;
            }

            // Starting/ending range in the middle of the run causes range enumerator to clone the node.
            // For starting, it does not make any difference. A field cannot start before such node because it's the first node of the range.
            // For ending, the node may not be removed because range enumerator will split and clone such node.
            Debug.Assert(range.End.IsStart || range.End.IsEnd, "Split runs may not be removed correctly.");

            using (NodeEnumerator rangeEnumerator = new NodeEnumerator(range))
            {
                FieldRemover fieldRemover = new FieldRemover();

                Node lastVisitedNode = null;
                Node firstRemainingNode = null;
                Node lastRemainingNode = null;

                // Collect nodes to be removed.
                while (rangeEnumerator.MoveToNextNode())
                {
                    lastVisitedNode = rangeEnumerator.CurrentNode;

                    switch (lastVisitedNode.NodeType)
                    {
                        case NodeType.FieldStart:
                        {
                            FieldStart fieldStart = (FieldStart)lastVisitedNode;
                            if (filter.NeedReplaceFieldWithResult(fieldStart))
                                fieldRemover.ProcessFieldStart(fieldStart);

                            break;
                        }
                        case NodeType.FieldSeparator:
                        {
                            FieldSeparator fieldSeparator = (FieldSeparator)lastVisitedNode;
                            if (filter.NeedReplaceFieldWithResult(fieldSeparator))
                                fieldRemover.ProcessFieldSeparator(fieldSeparator);

                            break;
                        }
                        case NodeType.FieldEnd:
                        {
                            FieldEnd fieldEnd = (FieldEnd)lastVisitedNode;
                            if (filter.NeedReplaceFieldWithResult(fieldEnd))
                                fieldRemover.ProcessFieldEnd(fieldEnd);

                            break;
                        }
                        default:
                        {
                            if (!fieldRemover.IsInsideField)
                            {
                                // Remember the last node that was not removed.
                                lastRemainingNode = lastVisitedNode;
                                // Set the first node that was not removed.
                                if (firstRemainingNode == null)
                                    firstRemainingNode = lastVisitedNode;
                            }

                            break;
                        }
                    }
                }

                // Remove nodes.
                fieldRemover.RemoveFieldNodes();
                fieldRemover.RemoveLastRangeForIncompleteField(lastVisitedNode);

                return (firstRemainingNode != null)
                    ? new NodeRange(firstRemainingNode, lastRemainingNode)
                    : null;
            }
        }

        private void ProcessFieldStart(FieldStart fieldStart)
        {
            mFieldChars.Push(fieldStart);
        }

        private void ProcessFieldSeparator(FieldSeparator fieldSeparator)
        {
            if (IsInsideField)
            {
                FieldChar lastFieldChar = mFieldChars.Pop();
                if (lastFieldChar.NodeType == NodeType.FieldStart)
                {
                    if (IsInsideField)
                    {
                        // Remember that separator for this level is already met.
                        mFieldChars.Push(fieldSeparator);
                    }
                    else if (FieldUtil.IsFieldResultInvisible((FieldStart)lastFieldChar, fieldSeparator))
                    {
                        // Field with invisible result must be treated as it has no separator, its result to be ignored.
                        mFieldChars.Push(lastFieldChar);
                    }
                    else
                    {
                        mNodeRangesToRemove.Add(new NodePair(lastFieldChar, fieldSeparator));
                    }
                }
                else
                {
                    // A stranded separator in a field result of an inner field, ignore, push back.
                    mFieldChars.Push(lastFieldChar);
                }
            }
            else
            {
                // Remove a stranded separator.
                mNodesToRemove.Add(fieldSeparator);
            }
        }

        private void ProcessFieldEnd(FieldEnd fieldEnd)
        {
            if (IsInsideField)
            {
                FieldChar lastFieldChar = mFieldChars.Pop();
                if (lastFieldChar.NodeType == NodeType.FieldStart)
                {
                    // Field had no separator.
                    if (!IsInsideField)
                        mNodeRangesToRemove.Add(new NodePair(lastFieldChar, fieldEnd));
                }
            }
            else
            {
                // A stranded field end, or maybe field separator is already met.
                mNodesToRemove.Add(fieldEnd);
            }
        }

        /// <summary>
        /// Removes the range for the last field if it was not removed range ended before the field ended.
        /// </summary>
        /// <param name="leftBoundary"></param>
        private void RemoveLastRangeForIncompleteField(Node leftBoundary)
        {
            if (IsInsideField)
            {
                // Get the bottom of the stack
                FieldChar outmostFieldStart;
                do
                {
                    outmostFieldStart = mFieldChars.Pop();
                } while (mFieldChars.Count > 0);

                NodeRemover.Remove(outmostFieldStart, leftBoundary);
            }
        }

        /// <summary>
        /// Removes the field nodes and ranges collected during range traversal.
        /// </summary>
        private void RemoveFieldNodes()
        {
            foreach (Node node in mNodesToRemove)
                node.Remove();

            // WORDSNET-16204 MS Word uses formating of next sibling paragraph.
            foreach (NodePair pair in mNodeRangesToRemove)
                NodeRemover.Remove(pair.Start, true, pair.End, true, NodeJoinMode.JoinToNextSibling);
        }

        /// <summary>
        /// Gets a boolean value indicating whether the current position is inside field code.
        /// </summary>
        private bool IsInsideField
        {
            get { return mFieldChars.Count > 0; }
        }

        private class NodePair
        {
            internal NodePair(Node start, Node end)
            {
                Start = start;
                End = end;
            }

            internal readonly Node Start;
            internal readonly Node End;
        }

        private readonly Stack<FieldChar> mFieldChars = new Stack<FieldChar>();
        private readonly List<Node> mNodesToRemove = new List<Node>();
        private readonly List<NodePair> mNodeRangesToRemove = new List<NodePair>();
    }
}
