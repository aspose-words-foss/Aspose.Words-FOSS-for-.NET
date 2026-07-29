// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/10/2014 by Edward Voronov

using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Finds nodes in a range.
    /// </summary>
    internal class NodeFinder : NodeEnumerator
    {
        /// <summary>
        /// Finds nodes of specific types in a range.
        /// </summary>
        internal static IList<Node> FindNodes(NodeRange range, params NodeType[] nodeTypes)
        {
            using (NodeFinder finder = new NodeFinder(range, nodeTypes))
                return finder.Find();
        }

        internal IList<Node> Find()
        {
            Nodes.Clear();

            Reset();

            while (MoveToNextNode())
            {
                if ((mNodeTypes.Contains(CurrentNode.NodeType) || mNodeTypes.Contains(NodeType.Any)) && OnNodeFinding())
                    Nodes.Add(CurrentNode);
            }

            return new List<Node>(Nodes);
        }

        protected NodeFinder(NodeRange range, params NodeType[] nodeTypes)
            : base(range)
        {
            mNodeTypes = new HashSetGeneric<NodeType>(nodeTypes.Length);
            foreach (NodeType nodeType in nodeTypes)
                mNodeTypes.Add(nodeType);

            Nodes = new List<Node>();
        }

        /// <summary>
        /// Called when a node of specific type was encountered. Returns false to skip a node otherwise true.
        /// </summary>
        protected virtual bool OnNodeFinding()
        {
            return true;
        }

        protected IList<Node> Nodes { get; }

        private readonly HashSetGeneric<NodeType> mNodeTypes;
    }
}
