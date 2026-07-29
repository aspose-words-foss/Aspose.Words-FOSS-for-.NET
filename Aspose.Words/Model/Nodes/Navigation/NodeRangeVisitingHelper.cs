// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Helps <see cref="DocumentVisitor"/> visit a <see cref="NodeRange"/>.
    /// </summary>
    internal class NodeRangeVisitingHelper : NodeEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="visitor"></param>
        /// <param name="modifier"></param>
        /// <param name="extractBehavior"></param>
        internal NodeRangeVisitingHelper(
            NodeRange range,
            DocumentVisitor visitor,
            INodeModifier modifier,
            NodeExtractBehavior extractBehavior)
            : base(range)
        {
            mVisitor = visitor;
            mModifier = modifier;
            mExtractBehavior = extractBehavior;
        }

        /// <summary>
        /// Sends a document visitor to visit all nodes of node range.
        /// Note some nodes, such as range start or end runs, may be cloned and the clones will be visited
        /// to avoid source nodes modification.
        /// </summary>
        internal void SendVisitorToNodeRange()
        {
            while (MoveToNextNode())
            {
                if (!CurrentNode.IsComposite)
                {
                    // Clone the node if needed. This might be the case when the node is modified by the modifier
                    // or when the node is the start or end node of the run that is being cut.
                    Node currentNode = ExtractCurrentNode(mModifier, mExtractBehavior);
                    if (currentNode != null)
                        currentNode.Accept(mVisitor);
                }
                else
                {
                    ((CompositeNode)CurrentNode).AcceptStart(mVisitor);
                }
            }
        }

        [JavaConvertCheckedExceptions]
        protected override void OnMoved(DocumentPositionMovement movement)
        {
            if (movement == DocumentPositionMovement.Above)
                ((CompositeNode)CurrentNode).AcceptEnd(mVisitor);
        }

        private readonly DocumentVisitor mVisitor;
        private readonly INodeModifier mModifier;
        private readonly NodeExtractBehavior mExtractBehavior;
    }
}
