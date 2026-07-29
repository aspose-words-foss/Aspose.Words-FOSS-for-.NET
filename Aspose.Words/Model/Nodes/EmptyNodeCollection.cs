// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/08/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// This is an always empty collection of nodes, needed for Range.Bookmarks etc
    /// collections to work when Range is obtained from a non composite node.
    /// </summary>
    internal static class EmptyNodeCollection
    {
        /// <summary>
        /// Create new instance of empty collection.
        /// WORDSNET-28048 Don't use a singleton because <see cref="NodeCollection"/> is not thread safe.
        /// </summary>
        internal static NodeCollection CreateEmpty()
        {
            return new EmptyCompositeNode().GetChildNodes(NodeType.Any, false);
        }
    }

    internal class EmptyCompositeNode : CompositeNode
    {
        public override NodeType NodeType
        {
            get { return NodeType.System; }
        }

        public override bool Accept(DocumentVisitor visitor)
        {
            return true;
        }

        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return VisitorAction.SkipThisNode;
        }

        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return VisitorAction.SkipThisNode;
        }

        internal override bool CanInsert(Node node)
        {
            return false;
        }
    }
}
