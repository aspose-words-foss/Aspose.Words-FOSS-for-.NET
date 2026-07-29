// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2013 by Ivan Lyagin

using Aspose.Words.Drawing;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Modifies nodes being copied in the way to produce MS-Word-like results for REF fields.
    /// </summary>
    internal class FieldRefNodeModifier : INodeModifier
    {
        internal FieldRefNodeModifier(bool includeNoteOrComment)
        {
            mIncludeNoteOrComment = includeNoteOrComment;
        }

        Node INodeModifier.Modify(Node referenceNode, Node nodeToModify, bool modifyChildren, INodeCloningListener cloningListener)
        {
            return Modify(nodeToModify, modifyChildren);
        }

        /// <summary>
        /// Performs actual modifying.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="modifyChildren"></param>
        /// <returns></returns>
        private Node Modify(Node node, bool modifyChildren)
        {
            if (NeedRemoveNode(node))
                return null;

            if (modifyChildren && node.IsComposite)
            {
                CompositeNode parentNode = (CompositeNode)node;
                Node childNode = parentNode.FirstChild;
                while (childNode != null)
                {
                    // Remember the child node.
                    Node childNodeToModify = childNode;
                    // The child node can be removed below so advance to the next one here.
                    childNode = childNode.NextSibling;

                    // If the child node should not appear in a REF field result remove it from a parent node.
                    if (Modify(childNodeToModify, true) == null)
                        childNodeToModify.Remove();
                }
            }

            return node;
        }

        /// <summary>
        /// Returns a value indicating whether the specified node should not appear in a REF field result.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool NeedRemoveNode(Node node)
        {
            // WORDSNET-11235 Skip comment ranges, comments and footnote nodes.
            if (NeedRemoveNodeType(node.NodeType))
                return true;

            // Nodes other than drawing objects should not be removed.
            if (!NodeUtil.IsDrawingObject(node))
                return false;

            // Get the topmost drawing object for the given one.
            Node topmostDrawingObject = node;
            while ((topmostDrawingObject.ParentNode != null) && NodeUtil.IsDrawingObject(topmostDrawingObject.ParentNode))
                topmostDrawingObject = topmostDrawingObject.ParentNode;

            // Determine whether the drawing object is inlined with text.
            // Note, that it makes sense only for the topmost drawing object.
            bool isInline = ((ShapeBase)topmostDrawingObject).IsInline;

            // If the drawing object is not inlined with text it should be removed.
            return !isInline;
        }

        private bool NeedRemoveNodeType(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.Comment:
                case NodeType.Footnote:
                    return !mIncludeNoteOrComment;
                default:
                    return NodeUtil.IsCrossStructureAnnotation(nodeType);
            }
        }

        private readonly bool mIncludeNoteOrComment;
    }
}
