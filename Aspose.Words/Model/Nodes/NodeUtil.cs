// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Michael Morozoff

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Common methods used on nodes.
    /// </summary>
    [CppOverrideAccessModifier(AccessModifiers.Public)]
    internal static class NodeUtil
    {
        /// <summary>
        /// Returns logical length of the node. This is not cumulative length of child nodes, but length of node
        /// itself. The returned length depends solely on the node type and not it's parent story.<para/>
        /// Value is 0 for Zlns, text length for runs and 1 for all other nodes.
        /// </summary>
        internal static int GetLength(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Run:
                {
                    Debug.Assert(0 < node.GetText().Length);
                    return node.GetText().Length;
                }
#if DEBUG
                case NodeType.GroupShape:       // Exists in Main text and Group shape stories
                case NodeType.Shape:            // Exists in Main text and Text box stories
                {
                    // MF It seems that shapes always have length of 1.
                    return /*(((ShapeBase) node).WrapType != WrapType.Inline) ? 0 : */ 1;
                }
                case NodeType.Section:          // Exists in Main text story
                case NodeType.Document:         // Exists in Main text story
                case NodeType.HeaderFooter:     // Exists in Header/Footer stories
                case NodeType.Footnote:         // Exists in Main text and Footnote/Endnote stories
                case NodeType.FieldStart:
                case NodeType.FieldSeparator:
                case NodeType.FieldEnd:
                case NodeType.FormField:
                case NodeType.Paragraph:
                case NodeType.SpecialChar:
                case NodeType.Table:
                case NodeType.Cell:
                case NodeType.Row:
                case NodeType.Comment:
                {
                    return 1;
                }
#endif
                default:
                {
                    if (IsZln(node))
                        return 0;
#if DEBUG
                    throw new ArgumentOutOfRangeException("node", node.NodeType.ToString());
#else
                    return 1;
#endif
                }
            }
        }

        /// <summary>
        /// Returns true if one node is an ancestor of the other node or they are the same.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodeToCheck"></param>
        /// <returns></returns>
        internal static bool IsAncestorOrSelf(Node node, Node nodeToCheck)
        {
            return ((node == nodeToCheck) || (node.IsAncestorNode(nodeToCheck)));
        }

        /// <summary>
        /// Returns an ancestor of the node with the specified type or the node itself if the ancestor is null.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ancestorType"></param>
        /// <returns></returns>
        internal static Node GetAncestorOrSelf(Node node, NodeType ancestorType)
        {
            Node ancestor = node.GetAncestor(ancestorType);
            return (ancestor != null ? ancestor : node);
        }

        /// <summary>
        /// Returns true if the specified node has an ancestor of the specified type.
        /// </summary>
        internal static bool HasAncestor(Node node, NodeType ancestorType)
        {
            return node != GetAncestorOrSelf(node, ancestorType);
        }

        /// <summary>
        /// Returns nesting level of the node. Doesn't ascend inline stories.
        /// </summary>
        internal static int GetNestingLevel(Node node)
        {
            int result = 0;
            while (null != node)
            {
                if (NodeType.Table == node.NodeType)
                    result++;

                node = node.ParentNode;
                if (null != node)
                {
                    switch (node.NodeType)
                    {
                        case NodeType.Comment:
                        case NodeType.Footnote:
                        case NodeType.GroupShape:
                        case NodeType.Shape:
                            node = null;
                            break;
                        default:
                            // Do nothing.
                            break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns True if node has zero length.
        /// </summary>
        internal static bool IsZln(Node node)
        {
            Debug.Assert(null != node);
            switch (node.NodeType)
            {
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:
                case NodeType.SmartTag:
                case NodeType.StructuredDocumentTag:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified node is a run with a non-whitespace text
        /// or a node of a non-zero length type.
        /// </summary>
        internal static bool IsNonWhitespace(Node node)
        {
            Debug.Assert(node != null);

            return (node.NodeType == NodeType.Run)
                ? !StringUtil.ContainsOnlyWhitespaces(node.GetText())
                : !IsZln(node);
        }

        /// <summary>
        /// Returns a value indicating whether the specified node is a run with a whitespace text
        /// or a node of a zero length type.
        /// </summary>
        internal static bool IsWhitespace(Node node)
        {
            return !IsNonWhitespace(node);
        }

        /// <summary>
        /// Indicates that given node is either BookmarkStart or BookmarkEnd node.
        /// </summary>
        internal static bool IsBookmarkNode(Node node)
        {
            return (node.NodeType == NodeType.BookmarkStart) || (node.NodeType == NodeType.BookmarkEnd);
        }

        /// <summary>
        /// Returns a value indicating whether the specified node is a cross-structure annotation (i.e. bookmark
        /// or comment range start/end).
        /// </summary>
        internal static bool IsCrossStructureAnnotation(Node node)
        {
            return IsCrossStructureAnnotation(node.NodeType);
        }

        /// <summary>
        /// Returns a value indicating whether the specified node type is a cross-structure annotation (i.e. bookmark
        /// or comment range start/end).
        /// </summary>
        internal static bool IsCrossStructureAnnotation(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:
                case NodeType.StructuredDocumentTagRangeStart:
                case NodeType.StructuredDocumentTagRangeEnd:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns True if specified node type is story master type.
        /// </summary>
        internal static bool IsStoryNodeType(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Document:         // RK This is MF's code. I think it should probably be NodeType.Body.
                    // MF No it should be Document because we need to skip over bodies to continue movements in main text.
                case NodeType.GlossaryDocument:
                case NodeType.HeaderFooter:
                case NodeType.Footnote:
                case NodeType.Comment:
                case NodeType.Shape:
                case NodeType.GroupShape:
                    return true;
                case NodeType.System:       // We lived without it for years. Maybe we do not need that, but it is correct to have it.
                    return node is FootnoteSeparator;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if element is block-level type.
        /// </summary>
        internal static bool IsBlockLevelNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Paragraph:
                case NodeType.Table:
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:
                    return true;
                case NodeType.StructuredDocumentTag:
                    return (((StructuredDocumentTag)node).Level == MarkupLevel.Block);
                case NodeType.System:
                    return (node is SdtMarkerStart) || (node is SdtMarkerEnd);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if element is row-level type. That means can be inserted inside a table on row-level.
        /// </summary>
        internal static bool IsRowLevelNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Row:
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:
                    return true;
                case NodeType.StructuredDocumentTag:
                    return (((StructuredDocumentTag)node).Level == MarkupLevel.Row);
                default:
                    return false;
            }
        }

        /// <summary>
        /// returns true if element is cell-level type. That means can be inserted inside a row at a cell-level.
        /// </summary>
        internal static bool IsCellLevelNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Cell:
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:
                    return true;
                case NodeType.StructuredDocumentTag:
                    return (((StructuredDocumentTag)node).Level == MarkupLevel.Cell);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true for inline-level nodes that can occur inside a paragraph, smart tag, sdt.
        /// </summary>
        internal static bool IsInlineLevelNode(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:

                case NodeType.EditableRangeStart:
                case NodeType.EditableRangeEnd:

                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:

                case NodeType.FieldStart:
                case NodeType.FieldSeparator:
                case NodeType.FieldEnd:
                case NodeType.FormField:

                case NodeType.Comment:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.Footnote:

                case NodeType.Run:
                case NodeType.SpecialChar:

                case NodeType.GroupShape:
                case NodeType.Shape:

                case NodeType.SmartTag:
                case NodeType.OfficeMath:
                case NodeType.SubDocument:
                    return true;

                case NodeType.System:
                    return node is SdtMarkerStart || node is SdtMarkerEnd;

                case NodeType.StructuredDocumentTag:
                    return (((StructuredDocumentTag)node).Level == MarkupLevel.Inline);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified node is a drawing object, i.e.
        /// <see cref="ShapeBase"/> descendant.
        /// </summary>
        internal static bool IsDrawingObject(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.Shape:
                case NodeType.GroupShape:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool CanInsertIntoMarkupNode(IMarkupNode parentMarkupNode, Node newChild)
        {
            // AM. Think we should not allow to insert range start/end into another SDT.
            if ((newChild.NodeType == NodeType.StructuredDocumentTagRangeStart) ||
                (newChild.NodeType == NodeType.StructuredDocumentTagRangeEnd))
                return false;

            if (newChild is IMarkupNode)
            {
                return parentMarkupNode.Level_IMarkupNode == ((IMarkupNode)newChild).Level_IMarkupNode;
            }
            else
            {
                switch (parentMarkupNode.Level_IMarkupNode)
                {
                    case MarkupLevel.Block:
                        return IsBlockLevelNode(newChild);
                    case MarkupLevel.Cell:
                        return IsCellLevelNode(newChild);
                    case MarkupLevel.Inline:
                        return IsInlineLevelNode(newChild);
                    case MarkupLevel.Row:
                        return IsRowLevelNode(newChild);
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns false if node type is one of the following:
        /// - <see cref="NodeType.Any"/>  - means that collection must return Sdt nodes along with others
        /// - <see cref="NodeType.StructuredDocumentTag"/> - means collection must return only sdt nodes
        /// - <see cref="NodeType.SmartTag"/> - means collection must return only smartTag nodes
        /// </summary>
        internal static bool IsSkipMarkupNodesInFlatEnumeration(NodeType nodeType)
        {
            return !((nodeType == NodeType.Any) || IsMarkupNode(nodeType));
        }

        /// <summary>
        /// Returns true if the given <see cref="NodeType"/> represents nodes used for Markup in the document tree.
        /// </summary>
        internal static bool IsMarkupNode(NodeType nodeType)
        {
            return (nodeType == NodeType.StructuredDocumentTag) || (nodeType == NodeType.SmartTag);
        }

        /// <summary>
        /// Returns true if the node is not null and is a markup node:
        /// <see cref="StructuredDocumentTag"/> or
        /// <see cref="SmartTag"/>.
        /// </summary>
        internal static bool IsMarkupNode(Node node)
        {
            return (node != null) && IsMarkupNode(node.NodeType);
        }

        /// <summary>
        /// Returns <c>true</c> if the node is not null and is a structured document tag node:
        /// <see cref="StructuredDocumentTag"/> or
        /// <see cref="StructuredDocumentTagRangeStart"/> or
        /// <see cref="StructuredDocumentTagRangeEnd"/>.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static bool IsStructuredDocumentTagNode(Node node)
        {
            return
                (node != null) &&
                ((node.NodeType == NodeType.StructuredDocumentTag) ||
                 (node.NodeType == NodeType.StructuredDocumentTagRangeStart) ||
                 (node.NodeType == NodeType.StructuredDocumentTagRangeEnd));
        }

        /// <summary>
        /// Returns true if the node is not null and is an Office Math node.
        /// </summary>
        internal static bool IsOfficeMath(Node node)
        {
            return (node != null) && (node.NodeType == NodeType.OfficeMath);
        }

        /// <summary>
        /// Gets the first non-markup node in depth-first traverse of the parent node, starting from the given node.
        /// </summary>
        /// <param name="startNode">
        /// The node to start the traverse with.
        /// If this is a non-markup node, it is returned.
        /// </param>
        /// <param name="compositeNodeOnly">
        /// Flag indicating whether it searches for a composite node.
        /// </param>
        /// <returns>
        /// If all traversed nodes are markups, null is returned.
        /// </returns>
        internal static Node GetFirstNonMarkupNodeInDepthFirstTraverse(Node startNode, bool compositeNodeOnly)
        {
            Node nonMarkup = null;
            for (Node current = startNode; (current != null) && (nonMarkup == null); current = current.NextSibling)
            {
                if (IsMarkupNode(current))
                {
                    CompositeNode compositeCurrent = (CompositeNode)current;
                    nonMarkup = compositeNodeOnly
                        ? compositeCurrent.FirstNonMarkupCompositeDescendant
                        : compositeCurrent.FirstNonMarkupDescendant;
                }
                else
                {
                    nonMarkup = (!compositeNodeOnly || current.IsComposite) ? current : null;
                }
            }

            return nonMarkup;
        }

        /// <summary>
        /// Gets the last non-markup node in a depth-first traverse of the parent node, which goes before or equals the given node.
        /// </summary>
        /// <param name="endNode">
        /// The node to end the traverse with.
        /// If this is a non-markup node, it is returned.
        /// </param>
        /// <param name="compositeNodeOnly">
        /// Flag indicating whether it searches for a composite node.
        /// </param>
        /// <returns>
        /// If all traversed nodes are markups, null is returned.
        /// </returns>
        internal static Node GetLastNonMarkupNodeInDepthFirstTraverse(Node endNode, bool compositeNodeOnly)
        {
            Node nonMarkup = null;
            for (Node current = endNode; (current != null) && (nonMarkup == null); current = current.PreviousSibling)
            {
                if (IsMarkupNode(current))
                {
                    CompositeNode compositeCurrent = (CompositeNode)current;
                    nonMarkup = compositeNodeOnly
                        ? compositeCurrent.LastNonMarkupCompositeDescendant
                        : compositeCurrent.LastNonMarkupDescendant;
                }
                else
                {
                    nonMarkup = (!compositeNodeOnly || current.IsComposite) ? current : null;
                }
            }

            return nonMarkup;
        }

        /// <summary>
        /// If node is table returns it, otherwise returns first grand parent which is table.
        /// This method doesn't ascend inline stories.
        /// </summary>
        internal static Table GetTable(Node node)
        {
            Debug.Assert(null != node);
            return (Table)(node.NodeType == NodeType.Table ? node :
                (Table)node.GetStoryAncestor(NodeType.Table));
        }

        /// <summary>
        /// Returns index of node in parent.
        /// </summary>
        internal static int GetNodeIndex(Node node)
        {
            Debug.Assert(node != null);
            if (node.ParentNode == null)
                return 0;

            int result = 0;
            for (Node n = node.ParentNode.FirstChild; n != node; n = n.NextSibling)
                result++;
            return result;
        }

        /// <summary>
        /// Returns node depth level.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static int GetDepth(Node node)
        {
            int depth = 0;

            while (node != null)
            {
                node = node.ParentNode;
                depth++;
            }

            return depth;
        }

        /// <summary>
        /// Gets node N-th parent.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        internal static Node GetNthParent(Node node, int n)
        {
            while ((n > 0) && (node != null))
            {
                n--;
                node = node.ParentNode;
            }

            return node;
        }

        /// <summary>
        /// Returns a value indicating whether the specified nodes belong to the same story.
        /// </summary>
        internal static bool AreNodesInSameStory(Node node1, Node node2)
        {
            return (node1.GetStoryAncestor(NodeType.Any) == node2.GetStoryAncestor(NodeType.Any));
        }

        /// <summary>
        /// Returns rotation of text in node. Value is in degrees.
        /// </summary>
        internal static float GetTextRotation(Node node)
        {
            // Text frames inside of a text box are ignored. So we first need to find parent text box
            // and only if it's not present check frame attributes.

            // Word doesn't let you have shapes nested inside a text box, but only inline images.
            // So we really need to get first shape above and check if it's a text box.
            // node<-paragraph<-shape
            Shape textBox = (Shape)node.GetAncestor(NodeType.Shape);
            // FOSS
            if (textBox != null)
                return 0;

            // Now check that there is no frame.
            // node<-frame paragraph
            Paragraph textFrame = (Paragraph)node.GetAncestor(NodeType.Paragraph);

            // Shape is inline thus it must have parent paragraph. However it may not have parent when rendered alone.
            if (textFrame == null)
                return 0;

            TextOrientation textOrientation = textFrame.ParaPr.FrameTextOrientation;
            if (!textFrame.ParaPr.IsFloating)
            {
                // Check if node is inside cell.
                Cell cell = (Cell)node.GetAncestor(NodeType.Cell);
                if (cell == null)
                    return 0;
                textOrientation = cell.CellPr.Orientation;
            }

            switch (textOrientation)
            {
                case TextOrientation.Downward:
                    return 90;
                case TextOrientation.Upward:
                    return 270;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// If cannot insert a shape into the parent of refNode, method inserts the shape into the first ancestor
        /// of refNode where the shape can be inserted.
        /// </summary>
        internal static void InsertShapeAtCompatibleTreeLevel(Node shapeNode, Node refNode)
        {
            // Search for node where we can insert newNode.
            while (refNode.ParentNode != null)
            {
                if (refNode.ParentNode.CanInsert(shapeNode))
                    break;

                refNode = refNode.ParentNode;
            }

            if (refNode.ParentNode == null)
                throw new InvalidOperationException("Parent cannot be null.");

            refNode.InsertNext(shapeNode);
        }

        /// <summary>
        /// Searches through the specified node and its descendants with using the specified matcher.
        /// </summary>
        internal static Node FindChildOrSelf(Node node, NodeMatcher matcher)
        {
            if (matcher.IsMatch(node))
                return node;

            if (node.IsComposite)
            {
                foreach (Node childNode in ((CompositeNode)node).GetChildNodes(NodeType.Any, false))
                {
                    Node foundNode = FindChildOrSelf(childNode, matcher);
                    if (foundNode != null)
                        return foundNode;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches for a paragraph next to the specified node. Traverses up to the node tree within
        /// body or story if necessary.
        /// </summary>
        /// <param name="node">Node to start search from.</param>
        /// <returns>Found paragraph or <c>null</c> if it is not found.</returns>
        internal static Paragraph FindNextParagraph(Node node)
        {
            return FindNextParagraph(node, true);
        }

        /// <summary>
        /// Searches for a paragraph prior to the specified node. Traverses up to the node tree within
        /// body or story if necessary.
        /// </summary>
        /// <param name="node">Node to start search from.</param>
        /// <returns>Found paragraph or <c>null</c> if it is not found.</returns>
        internal static Paragraph FindPreviousParagraph(Node node)
        {
            return FindNextParagraph(node, false);
        }

        private static Paragraph FindNextParagraph(Node node, bool forward)
        {
            int getChildIndex = forward ? 0 : -1;

            Node paragraph = null;
            Node currentLevel = node;
            do
            {
                Node sibling = forward
                    ? currentLevel.NextNonMarkupCompositeLimited
                    : currentLevel.PreviousNonMarkupCompositeLimited;

                while ((paragraph == null) && (sibling != null))
                {
                    if (sibling.NodeType == NodeType.Paragraph)
                        paragraph = sibling;
                    else if (sibling.IsComposite)
                        paragraph = ((CompositeNode)sibling).GetChild(NodeType.Paragraph, getChildIndex, true);

                    sibling = forward
                        ? sibling.NextNonMarkupCompositeLimited
                        : sibling.PreviousNonMarkupCompositeLimited;
                }

                currentLevel = currentLevel.FirstNonMarkupParentNode;
            }
            while (TryFindNextParagraph(paragraph, currentLevel));

            return (Paragraph)paragraph;
        }

        private static bool TryFindNextParagraph(Node paragraph, Node currentLevel)
        {
            if (paragraph != null)
                return false;

            if (currentLevel == null)
                return false;

            return IsBlockLevelNode(currentLevel) || IsCellLevelNode(currentLevel) || IsRowLevelNode(currentLevel);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified paragraph is inside a cell and it is the only paragraph of it.
        /// </summary>
        internal static bool IsSingleParagraphInCell(Paragraph paragraph)
        {
            Cell cell = paragraph.FirstNonMarkupParentNode as Cell;

            return
                (cell != null) &&
                (cell.FirstParagraph == paragraph) &&
                (cell.LastParagraph == paragraph);
        }

        /// <summary>
        /// Resets values of the <see cref="IDisplaceableByCustomXml.DisplacedByCustomXml"/> property of annotations
        /// that reference the specified structured document tag/structured document tag range node.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal static void ResetDisplacedAnnotationReferences(Node sdtNode, bool areChildNodesPreserved)
        {
            Debug.Assert(IsStructuredDocumentTagNode(sdtNode));
            CompositeNode sdt = sdtNode as CompositeNode;

            // If there is another SDT after/before the specified sdtNode in DisplacedByCustomXml direction,
            // DisplacedByCustomXml should be preserved. For example, if a bookmark start has DisplacedByType.Next,
            // an after it contains another SDT as the first child, and the SDT is removed using RemoveSelfOnly,
            // the bookmark start will reference the child SDT, and its DisplacedByCustomXml should be preserved.

            // Reset annotations before the SDT, DisplacedByCustomXml of which is DisplacedByType.Next.
            // Skip if there is a child SDT or another SDT after the processing one.
            Node nextToStart = ((sdt != null) && areChildNodesPreserved) ? sdt.FirstChild : sdtNode.NextSibling;
            if (!HasSdtNodeIgnoringAnnotations(nextToStart, true))
                ResetAnnotationDisplacement(sdtNode.PreviousSibling, DisplacedByType.Next, false);

            // Reset annotations after the SDT, DisplacedByCustomXml of which is DisplacedByType.Prev.
            // Skip if there is a child SDT or another SDT before the processing one.
            Node prevFromEnd = ((sdt != null) && areChildNodesPreserved) ? sdt.LastChild : sdtNode.PreviousSibling;
            if (!HasSdtNodeIgnoringAnnotations(prevFromEnd, false))
                ResetAnnotationDisplacement(sdtNode.NextSibling, DisplacedByType.Prev, true);

            if (sdt != null)
            {
                // Reset first-child annotations, DisplacedByCustomXml of which is DisplacedByType.Prev.
                // Skip if there is a parent SDT or another SDT before the processing one.
                if (!HasSdtNodeIgnoringAnnotations(sdt.PreviousSibling, false) &&
                    ((sdt.PreviousNonAnnotationSibling != null) ||
                     (sdt.ParentNode.NodeType != NodeType.StructuredDocumentTag)))
                {
                    ResetAnnotationDisplacement(sdt.FirstChild, DisplacedByType.Prev, true);
                }

                // Reset last-child annotations, DisplacedByCustomXml of which is DisplacedByType.Next.
                // Skip if there is a parent SDT or another SDT after the processing one.
                if (!HasSdtNodeIgnoringAnnotations(sdt.NextSibling, true) &&
                    ((sdt.NextNonAnnotationSibling != null) ||
                     (sdt.ParentNode.NodeType != NodeType.StructuredDocumentTag)))
                {
                    ResetAnnotationDisplacement(sdt.LastChild, DisplacedByType.Next, false);
                }
            }
        }

        /// <summary>
        /// Returns <c>true</c> if there is a structured document tag/structured document tag range node in the specified
        /// direction starting with the specified node.
        /// </summary>
        private static bool HasSdtNodeIgnoringAnnotations(Node node, bool inForwardDirection)
        {
            while (node != null)
            {
                if (IsStructuredDocumentTagNode(node))
                    return true;

                if (!IsCrossStructureAnnotation(node))
                    break;

                node = inForwardDirection ? node.NextSibling : node.PreviousSibling;
            }

            return false;
        }

        /// <summary>
        /// Resets value of <see cref="IDisplaceableByCustomXml.DisplacedByCustomXml"/> property of annotations
        /// starting with the node in the specified direction if the property has the specified value.
        /// </summary>
        private static void ResetAnnotationDisplacement(Node node, DisplacedByType displacedByType, bool inForwardDirection)
        {
            while ((node != null) && IsCrossStructureAnnotation(node))
            {
                IDisplaceableByCustomXml displaceableByCustomXml = node as IDisplaceableByCustomXml;
                if ((displaceableByCustomXml != null) && (displaceableByCustomXml.DisplacedByCustomXml == displacedByType))
                    displaceableByCustomXml.DisplacedByCustomXml = DisplacedByType.Unspecified;

                node = inForwardDirection ? node.NextSibling : node.PreviousSibling;
            }
        }
    }
}
