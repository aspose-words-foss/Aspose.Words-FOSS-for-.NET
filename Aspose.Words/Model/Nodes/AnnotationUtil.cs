// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/09/2018 by Alexander Zhiltsov

using Aspose.Words.Markup;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    internal static class AnnotationUtil
    {
        /// <summary>
        /// Gets real range bound for the specified annotation node. Result depends on value of the DisplacedBy property.
        /// </summary>
        /// <remarks>
        /// It is mainly used for updating referencing fields.
        /// </remarks>
        internal static RangeBound GetAnnotationRangeBound(Node annotation, bool forRangeStart)
        {
            IDisplaceableByCustomXml displaceableAnnotation = annotation as IDisplaceableByCustomXml;

            if (displaceableAnnotation == null)
                return new RangeBound(annotation, false);

            switch (displaceableAnnotation.DisplacedByCustomXml)
            {
                case DisplacedByType.Next:
                    return GetBoundByNextSdt(annotation, forRangeStart);
                case DisplacedByType.Prev:
                    return GetBoundByPreviousSdt(annotation, forRangeStart);
                default:
                    return new RangeBound(annotation, false);
            }
        }

        /// <summary>
        /// Gets range bound of a annotation node that is displaced by next SDT.
        /// </summary>
        private static RangeBound GetBoundByNextSdt(Node annotation, bool forRangeStart)
        {
            // The code reproduces behaviour of MS Word to some extent.
            // if displacedByCustomXml = "next", looks like it searches for real position of a annotation node in this way:
            // 1. If a annotation node is located before an SDT, this is a searching displaced SDT.
            // 2. If a annotation node is a descending of SDT (not only a direct child), this is a searching displaced SDT.
            // (If the both #1 and #2 are present, looks like it is an error and the attribute is just removed/ignored.)
            // 3. If node is outside of SDT, it searches for the next SDT as far as necessary. If there is no SDT,
            // the annotation is treated as invalid and deleted.
            // If a displaced SDT has been found, annotation real position is at inline level inside it.

            bool isStartOfSdt = true; // i.e. before SDT
            StructuredDocumentTag sdt = annotation.NextNonAnnotationSibling as StructuredDocumentTag;

            if (sdt == null)
            {
                Node sdtRange = GetSdtRangeNode(annotation.NextSibling, true);
                if (sdtRange != null)
                {
                    return GetBound(sdtRange, forRangeStart, sdtRange.NodeType == NodeType.StructuredDocumentTagRangeStart,
                        annotation, sdtRange.NextSibling);
                }
            }

            if (sdt == null)
            {
                sdt = (StructuredDocumentTag)annotation.GetAncestor(NodeType.StructuredDocumentTag);
                isStartOfSdt = sdt == null;
            }

            if (sdt == null)
            {
                DocumentPosition dp = new DocumentPosition(annotation);
                while (dp.Move(annotation.Document, true, true, false, false, false))
                {
                    if (dp.Node.NodeType == NodeType.StructuredDocumentTag)
                    {
                        sdt = (StructuredDocumentTag)dp.Node;
                        break;
                    }
                }
            }

            if (sdt != null)
                return GetBoundBySdt(sdt, forRangeStart, isStartOfSdt, annotation);
            else
                return null;
        }

        /// <summary>
        /// Gets range bound of a annotation node that is displaced by previous SDT.
        /// </summary>
        private static RangeBound GetBoundByPreviousSdt(Node annotation, bool forRangeStart)
        {
            // The code reproduces behaviour of MS Word to some extent.
            // if displacedByCustomXml = "prev", looks like it searches for real position of a annotation node in this way:
            // 1. If a annotation node is located after an SDT, this is a searching displaced SDT.
            // 2. If a annotation node is a direct child of an SDT, this is a searching displaced SDT.
            // (If the both #1 and #2 are present, looks like it is an error and the attribute is just removed/ignored.)
            // 3. If a annotation node is not a direct child of an SDT, Word goes back in node hierarchy till the first SDT
            // or text node (or paragraph/cell/row end) is reached. If it is an SDT, this is a displaced SDT. At the other
            // case the real position is through one char of the reached node (paragraph break is treated as one char too):
            // run is split for it, or the annotation position is last child of paragraph/cell/row; displacedByCustomXml
            // attribute (of this node only) is reset.
            // If a displaced SDT has been found, annotation real position is at inline level inside it.

            bool isStartOfSdt = false; // i.e. first child of SDT
            StructuredDocumentTag sdt = annotation.PreviousNonAnnotationSibling as StructuredDocumentTag;

            if (sdt == null)
            {
                Node sdtRange = GetSdtRangeNode(annotation.PreviousSibling, false);
                if (sdtRange != null)
                {
                    return GetBound(sdtRange, forRangeStart, sdtRange.NodeType == NodeType.StructuredDocumentTagRangeStart,
                        annotation, sdtRange.PreviousSibling);
                }
            }

            if ((sdt == null) &&
                (annotation.ParentNode != null) &&
                (annotation.ParentNode.NodeType == NodeType.StructuredDocumentTag))
            {
                sdt = (StructuredDocumentTag)annotation.ParentNode;
                isStartOfSdt = true;
            }

            if (sdt == null)
            {
                DocumentPosition dp = new DocumentPosition(annotation);
                while (dp.Move(annotation.Document, false, true, false, false, false))
                {
                    Node node = dp.Node;
                    if (node.NodeType == NodeType.StructuredDocumentTag)
                    {
                        sdt = (StructuredDocumentTag)node;
                        break;
                    }

                    CompositeNode composite = node as CompositeNode;
                    if ((composite != null) && (composite.GetEndText() != string.Empty))
                    {
                        if (composite.LastChild != null)
                            return new RangeBound(composite.LastChild, !forRangeStart);
                        else
                            return new RangeBound(composite, forRangeStart);
                    }
                    else if (node is Inline)
                    {
                        bool isBeforeNode = node.GetText().Length == 1;
                        return new RangeBound(node, forRangeStart ? isBeforeNode : !isBeforeNode);
                    }
                }
            }

            if (sdt != null)
                return GetBoundBySdt(sdt, forRangeStart, isStartOfSdt, annotation);
            else
                return null;
        }

        /// <summary>
        /// Searches for a <see cref="StructuredDocumentTagRangeStart"/> or <see cref="StructuredDocumentTagRangeEnd"/>
        /// node near the specified node in the specified direction. The search is performed up to the first
        /// non-annotation node.
        /// </summary>
        private static Node GetSdtRangeNode(Node node, bool inForwardDirection)
        {
            Node sdtRange = null;

            while ((node != null) && NodeUtil.IsCrossStructureAnnotation(node))
            {
                if ((node.NodeType == NodeType.StructuredDocumentTagRangeStart) ||
                    (node.NodeType == NodeType.StructuredDocumentTagRangeEnd))
                {
                    sdtRange = node;
                }

                node = inForwardDirection ? node.NextSibling : node.PreviousSibling;
            }

            return sdtRange;
        }

        /// <summary>
        /// Gets range bound by the displacing SDT.
        /// </summary>
        private static RangeBound GetBoundBySdt(StructuredDocumentTag sdt, bool forRangeStart, bool forSdtStart,
            Node annotation)
        {
            Node node = GetDeepestChild(sdt, forSdtStart);
            return GetBound(sdt, forRangeStart, forSdtStart, annotation, node);
        }

        private static RangeBound GetBound(Node sdtNode, bool forRangeStart, bool forSdtStart, Node annotation, Node node)
        {
            if ((node == null) ||
                (NodeUtil.IsCrossStructureAnnotation(node) &&
                 (node.ParentNode == annotation.ParentNode) &&
                 (forRangeStart
                     ? (node.NextNonAnnotationSibling == annotation.NextNonAnnotationSibling)
                     : (node.PreviousNonAnnotationSibling == annotation.PreviousNonAnnotationSibling))))
                node = annotation;

            return new RangeBound(
                node,
                (node != annotation) &&
                    // Displaced annotation is intended at inline level. Composite is treated as a paragraph break char.
                    // Depending on it, include or not composite node.
                    ((forRangeStart)
                        ? (forSdtStart || node.IsComposite)
                        : (!forSdtStart && (!node.IsComposite || ((CompositeNode)node).HasChildNodes))),
                sdtNode,
                annotation);
        }

        /// <summary>
        /// Gets the deepest child node (non-annotation) of the specified node without going into inline stories.
        /// </summary>
        internal static Node GetDeepestChild(CompositeNode node, bool isGettingFirst)
        {
            while (true)
            {
                Node childNonAnnotation = isGettingFirst ? node.FirstNonAnnotationChild : node.LastNonAnnotationChild;
                if (childNonAnnotation == null)
                    return node.HasChildNodes ? (isGettingFirst ? node.FirstChild : node.LastChild) : node;

                if (childNonAnnotation.IsComposite && (childNonAnnotation.NodeLevel != NodeLevel.Inline))
                    node = (CompositeNode)childNonAnnotation;
                else
                    return isGettingFirst ? node.FirstChild : node.LastChild;
            }
        }
    }

    /// <summary>
    /// Represents necessary information for bound of a range: node and a flag indicating whether to include
    /// the node into the range.
    /// </summary>
    /// <remarks>
    /// For ranges specified by a cross-structure annotation node with defined DisplacedBy property, the
    /// <see cref="Node"/> property contains node that is effective start/end of a range. This is a child of
    /// a displacing SDT (if a displacing SDT is found).
    /// </remarks>
    internal class RangeBound
    {
        /// <summary>
        /// Ctor if range bound is not by displacing annotation node.
        /// </summary>
        internal RangeBound(Node node, bool includeNode)
            : this(node, includeNode, null, null)
        {
        }

        /// <summary>
        /// Ctor if range bound is by displacing annotation node.
        /// </summary>
        internal RangeBound(Node node, bool includeNode, Node displacingNode, Node annotation)
        {
            Node = node;
            PreviousSibling = node.PreviousSibling;
            NextSibling = node.NextSibling;
            Parent = node.ParentNode;
            IsNodeIncluded = includeNode;
            Debug.Assert((displacingNode == null) || NodeUtil.IsStructuredDocumentTagNode(displacingNode));
            DisplacingNode = displacingNode;
            DisplacedAnnotation = annotation;
            // Store mIsAnnotationAChildOfSdt to be able to remove the annotation during processing.
            IsAnnotationAChildOfSdt = (annotation != null) && annotation.IsAncestorNode(displacingNode);
        }

        internal Node Node { get; }

        internal bool IsNodeIncluded { get; }

        /// <summary>
        /// Range bound is formed by this displacing structured document tag or structured document tag range start/end.
        /// </summary>
        internal Node DisplacingNode { get; }

        /// <summary>
        /// If this property contains a value, this range bound is specified by a cross-structure annotation node with
        /// defined DisplacedBy property. Effective start/end of a range is stored in the <see cref="Node"/> property.
        /// </summary>
        internal Node DisplacedAnnotation { get; }

        internal DisplacedByType DisplacedByType
        {
            get
            {
                return (DisplacedAnnotation != null)
                    ? ((IDisplaceableByCustomXml)DisplacedAnnotation).DisplacedByCustomXml
                    : DisplacedByType.Unspecified;
            }
        }

        internal bool IsAnnotationAChildOfSdt { get; }

        // The following properties can help to get position after removing Node.

        internal Node PreviousSibling { get; }

        internal Node NextSibling { get; }

        internal CompositeNode Parent { get; }
    }
}
