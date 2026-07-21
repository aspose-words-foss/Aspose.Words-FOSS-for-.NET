// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/01/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Traverses over a range of nodes in a special manner. Calls appropriate methods while traversing.
    /// See summary for the <see cref="Traverse"/> method.
    /// </summary>
    internal class NodeTraverser : NodeEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        protected NodeTraverser(NodeRange range)
            : base(range, true)
        {
        }

        /// <summary>
        /// Moves from the start node to the end node of the range in a special manner. It tends to move upwards
        /// and does not move down unless the current composite node is an ancestor of the end node. It allows efficiently
        /// manipulating nodes in a range, e.g. copying or removing. Additionally, calls different protected virtual
        /// methods that signalize about some events during traversal.
        /// </summary>
        protected void Traverse()
        {
            mNeedStopTraverse = false;
            bool isDeep = true;
            Node oldNode = null;

            // isByNode should be set to false we could stop at ends of composite nodes.
            while (!mNeedStopTraverse && MoveToNextNode(isDeep, false))
            {
                // Do not stop at the same node twice.
                if (oldNode == CurrentNode)
                    continue;

                if (CurrentPosition.IsEnd)
                {
                    if (CurrentNode.IsComposite && NodeUtil.IsAncestorOrSelf(Range.Start.Node, CurrentNode))
                        OnStartNodeAncestor();
                }
                else if (CurrentNode.IsComposite)
                {
                    if (!NodeUtil.IsAncestorOrSelf(Range.End.Node, CurrentNode))
                    {
                        OnMiddleNodeAncestor();
                        isDeep = false;
                    }
                    else
                    {
                        OnEndNodeAncestor();
                        isDeep = true;
                    }
                }
                else
                {
                    OnNonCompositeNode();
                    isDeep = false;
                }

                oldNode = CurrentNode;
            }
        }

        /// <summary>
        /// Called when a composite node that owns the start node in the range is encountered.
        /// </summary>
        [JavaThrows(true)]
        protected virtual void OnStartNodeAncestor()
        {
            //  Implementation is not required.
        }

        /// <summary>
        /// Called when a composite node that does not own the start or end node in the range is encountered.
        /// </summary>
        [JavaThrows(true)]
        protected virtual void OnMiddleNodeAncestor()
        {
            //  Implementation is not required.
        }

        /// <summary>
        /// Called when a composite node that owns the end node in the range is encountered.
        /// </summary>
        [JavaThrows(true)]
        protected virtual void OnEndNodeAncestor()
        {
            //  Implementation is not required.
        }

        /// <summary>
        /// Called when a non-composite node is encountered.
        /// </summary>
        [JavaThrows(true)]
        protected virtual void OnNonCompositeNode()
        {
            //  Implementation is not required.
        }

        /// <summary>
        /// Stops traversal.
        /// </summary>
        protected void StopTraverse()
        {
            mNeedStopTraverse = true;
        }

        private bool mNeedStopTraverse;
    }
}
