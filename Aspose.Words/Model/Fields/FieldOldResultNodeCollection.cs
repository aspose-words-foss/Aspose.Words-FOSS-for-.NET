// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2013 by Ivan Lyagin

using Aspose.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a collection of old result nodes for a single field.
    /// </summary>
    internal class FieldOldResultNodeCollection
    {

        /// <summary>
        /// Creates an instance of <see cref="FieldOldResultNodeCollection"/> for the specified field if its old result
        /// nodes will be used while its update.
        /// </summary>
        internal static FieldOldResultNodeCollection Create(Field field)
        {
            return new FieldOldResultNodeCollection(field);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private FieldOldResultNodeCollection(Field field)
        {
            NodeRange range = field.GetFieldResultRange();
            if (range.IsVoid)
            {
                Range = NodeRange.Void;
                return;
            }

            Node start = range.Start.Node;
            Node end = range.End.Node;

            EdgeCloningListener listener = new EdgeCloningListener(start, end);

            mClonedRoot = CloneSubTree(start, end, listener);

            Range = new NodeRange(listener.StartClone, false, listener.EndClone, false);
        }

        private static Node CloneSubTree(Node start, Node end, INodeCloningListener listener)
        {
            Node root = Node.GetCommonAncestor(start, end);

            ISetGeneric<Node> startAncestors = GetAncestors(start, root);
            ISetGeneric<Node> endAncestors = GetAncestors(end, root);

            return CloneSubTree(root, startAncestors, endAncestors, listener);
        }

        private static ISetGeneric<Node> GetAncestors(Node node, Node root)
        {
            HashSetGeneric<Node> ancestors = new HashSetGeneric<Node>();

            Node currentNode = node;
            while (currentNode != null)
            {
                ancestors.Add(currentNode);

                if (currentNode == root)
                    break;

                currentNode = currentNode.ParentNode;
            }

            return ancestors;
        }

        private static Node CloneSubTree(Node node, ISetGeneric<Node> startAncestors, ISetGeneric<Node> endAncestors, INodeCloningListener listener)
        {
            Node clone = node.Clone(false, listener);

            if (node.IsComposite)
            {
                CompositeNode compositeNode = (CompositeNode)node;
                CompositeNode compositeClone = (CompositeNode)clone;

                bool isSrartBranchFound = startAncestors == null;
                for (Node child = compositeNode.FirstChild; child != null; child = child.NextSibling)
                {
                    if (isSrartBranchFound)
                    {
                        if (endAncestors != null && endAncestors.Contains(child))
                        {
                            compositeClone.AppendChildForLoad(CloneSubTree(child, null, endAncestors, listener));
                            break;
                        }

                        compositeClone.AppendChildForLoad(child.Clone(true));
                    }
                    else
                    {
                        if (startAncestors.Contains(child))
                        {
                            compositeClone.AppendChildForLoad(CloneSubTree(child, startAncestors, null, listener));
                            isSrartBranchFound = true;
                        }
                    }
                }
            }

            return clone;
        }

        internal NodeRange Range { get; }

        // ReSharper disable once NotAccessedField.Local : C++ workaround, extends lifetime of cloned nodes
        private readonly Node mClonedRoot;

        private class EdgeCloningListener : INodeCloningListener
        {
            public EdgeCloningListener(Node start, Node end)
            {
                mStartSource = start;
                mEndSource = end;
            }

            public void NotifyNodeCloned(Node source, Node clone)
            {
                if (source == mStartSource)
                    StartClone = clone;

                if (source == mEndSource)
                    EndClone = clone;
            }

            internal Node StartClone { get; private set; }

            internal Node EndClone { get; private set; }

            private readonly Node mStartSource;
            private readonly Node mEndSource;
        }
    }
}
