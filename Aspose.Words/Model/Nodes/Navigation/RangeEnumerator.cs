// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/12/2024 by Vadim Saltykov

using Aspose.Words.Markup;

namespace Aspose.Words
{
    /// <summary>
    /// Enumerates a range of nodes.
    /// </summary>
    internal class RangeEnumerator : NodeEnumerator, IDocumentPositionListener
    {
        internal RangeEnumerator(NodeRange range)
            : base(range, false)
        {
        }

        /// <summary>
        /// Returns the suitable node for changing position when moving above.
        /// </summary>
        private Node FindValidNodeAbove()
        {
            return GetValidParent(CurrentNode);
        }

        /// <summary>
        /// Returns the suitable node for changing position when moving below.
        /// </summary>
        private Node FindValidNodeBelow()
        {
            if (CurrentNode.NodeType == NodeType.Body)
                return CurrentNode;

            if (CurrentNode.NodeType == NodeType.HeaderFooter)
                return ((Section)CurrentNode.ParentNode).Body;

            return GetValidChild(CurrentNode);
        }

        /// <summary>
        /// Returns True if the current range ends with the included StructuredDocumentTagRangeStart.
        /// </summary>
        private bool IsEndingRangedSdt()
        {
            return Range.IsEndIncluded &&
                   (Range.End.Node.NodeType == NodeType.StructuredDocumentTagRangeStart);
        }

        /// <summary>
        /// Returns True if the end node of the range is achieved and it is StructuredDocumentTagRangeStart.
        /// </summary>
        private bool IsRangedSdtStartAchieved()
        {
            return IsEndingRangedSdt() &&
                   IsEndNode && (CurrentNode.NodeType == NodeType.StructuredDocumentTagRangeStart);
        }

        /// <summary>
        /// Returns True if StructuredDocumentTagRangeEnd node is achieved and it matches the start node of the ranged SDT.
        /// </summary>
        private bool IsRangedSdtEndAchieved()
        {
            if (!IsEndingRangedSdt() || (CurrentNode.NodeType != NodeType.StructuredDocumentTagRangeEnd))
                return false;

            StructuredDocumentTagRangeStart sdtStart = (StructuredDocumentTagRangeStart)Range.End.Node;
            StructuredDocumentTagRangeEnd sdtEnd = (StructuredDocumentTagRangeEnd)CurrentNode;

            return sdtStart.RangeEnd == sdtEnd;
        }

        void IDocumentPositionListener.NotifyMoved(DocumentPositionMovement movement)
        {
            switch (movement)
            {
                case DocumentPositionMovement.Above:
                {
                    Node newNode = FindValidNodeAbove();
                    if (newNode != CurrentNode)
                        CurrentPosition = DocumentPosition.CreatePositionAfter(newNode);

                    break;
                }
                case DocumentPositionMovement.Below:
                {
                    Node newNode = FindValidNodeBelow();
                    if (newNode != CurrentNode)
                        CurrentPosition = DocumentPosition.CreatePositionBefore(newNode);

                    break;
                }
                case DocumentPositionMovement.StartEnd:
                {
                    if (IsRangedSdtStartAchieved())
                        CurrentPosition.Move(null, true, true, true, false, true, this);
                    else if (IsRangedSdtEndAchieved())
                        CurrentPosition = DocumentPosition.CreatePositionAfter(Range.End.Node);

                    break;
                }
                default:
                    break;
            }
        }
    }
}
