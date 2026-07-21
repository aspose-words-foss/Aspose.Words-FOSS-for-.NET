// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2016 by Edward Voronov

namespace Aspose.Words
{
    /// <summary>
    /// Collects composite nodes of specified types in node range during copying
    /// and replaces them with children when node ragne copying is completed.
    /// </summary>
    internal class CompositeNodeReplacer : NodeReplacer
    {
        internal CompositeNodeReplacer(params NodeType[] nodeTypes)
            : base(nodeTypes)
        {
        }

        protected override void ReplaceCollectedNode(Node node)
        {
            CompositeNode compositeNode = (CompositeNode)node;

            while (compositeNode.FirstChild != null)
                compositeNode.InsertPrevious(compositeNode.FirstChild);

            compositeNode.Remove();
        }
    }
}
