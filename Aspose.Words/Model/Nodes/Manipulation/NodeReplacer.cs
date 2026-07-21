// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2016 by Edward Voronov

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Collects nodes of specified types in node range during copying
    /// and performs replace when node ragne copying is completed.
    /// </summary>
    internal abstract class NodeReplacer : INodeCopierListener
    {
        protected NodeReplacer(params NodeType[] nodeTypes)
        {
            Debug.Assert(nodeTypes.Length > 0);
            mNodeTypes = nodeTypes;
        }

        void INodeCloningListener.NotifyNodeCloned(Node source, Node clone)
        {
            if (Array.IndexOf(mNodeTypes, source.NodeType) == -1)
                return;

            CollectClone(source, clone);
        }

        void INodeCopierListener.NotifyNodeRangeCopied(NodeRange sourceRange, NodeRange insertedRange)
        {
            if (!HasClones)
                return;

            foreach (Node node in mClones)
                ReplaceCollectedNode(node);

            FinalizeReplace();
        }

        [JavaThrows(true)]
        protected abstract void ReplaceCollectedNode(Node node);

        protected virtual void CollectClone(Node source, Node clone)
        {
            if (!HasClones)
                mClones = new List<Node>();

            mClones.Add(clone);
        }

        [JavaThrows(true)]
        protected virtual void FinalizeReplace()
        {
        }

        private bool HasClones
        {
            get { return mClones != null; }
        }

        private List<Node> mClones;

        private readonly NodeType[] mNodeTypes;
    }
}
