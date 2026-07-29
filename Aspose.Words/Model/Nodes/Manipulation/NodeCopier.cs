// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/11/2009 by Dmitry Vorobyev

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Math;

namespace Aspose.Words
{
    /// <summary>
    /// Copies (clones and inserts) nodes from a source range to some point in the document.
    /// Nodes being copied may be modified using an <see cref="INodeModifier"/> implementor.
    /// </summary>
    internal class NodeCopier : NodeTraverser
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private NodeCopier(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCloningListener nodeCloningListener,
            NodeCopierOptions options)
            : base(sourceRange)
        {
            mReferenceNode = referenceNode;
            mModifier = modifier;
            mNodeCloningListener = nodeCloningListener;
            mOptions = options;

            // Whether need to copy from block to inline level.
            Node startNode = sourceRange.Start.Node;
            mMoveToInlineLevel =
                ProcessBoundBlockAnnotationAsInline &&
                (referenceNode.NodeLevel == NodeLevel.Inline) &&
                (startNode.NodeLevel == NodeLevel.Block) &&
                NodeUtil.IsCrossStructureAnnotation(startNode);

            mCurrentReferenceNode = referenceNode.PreviousSibling;
            mBuilder = new DocumentBuilder(referenceNode.FetchDocument());

            Paragraph referenceParagraph = (Paragraph)mReferenceNode.GetAncestor(NodeType.Paragraph);
            AttrCollection paragraphFrameAttrs = FetchParagraphFrameAttrs(referenceParagraph);
            if (paragraphFrameAttrs.ContainsAnyKey(ParaPr.FloatingAttrs) || IsRangeWithinSingleFrame(sourceRange))
                mReferenceParagraphFrameAttrs = paragraphFrameAttrs;
        }

        /// <summary>
        /// Clones paragraph attributes but preserves frame attributes.
        /// </summary>
        /// <remarks>
        /// EV: Probably, it should be an instance method of the Paragraph class, but I'm not sure it's a common case.
        /// Also, I really don't want to introduce one more XxxUtils class for this method, but I need to reuse it in FieldRefUtil.
        /// </remarks>
        internal static void CloneParaPrPreserveFrameAttrs(Paragraph paragraph, ParaPr paraPr)
        {
            AttrCollection frameAttrs = FetchParagraphFrameAttrs(paragraph);
            paragraph.ParaPr = paraPr.Clone();
            MirrorFrameAttrs(frameAttrs, paragraph.ParaPr);
        }

        private static bool IsRangeWithinSingleFrame(NodeRange range)
        {
            AttrCollection lastFrameAttrs = null;

            IList<Node> list = NodeFinder.FindNodes(range, NodeType.Paragraph);

            Node startParagraph = range.Start.Node.GetAncestor(NodeType.Paragraph);
            if (startParagraph != null)
                list.Insert(0, startParagraph);

            for (int i = 0; i < list.Count; i++)
            {
                Node node = list[i];
                if (i == list.Count - 1 && range.End.Node.IsAncestorNode(node) && range.End.Node.IsFirstNonZeroLengthChild)
                    continue;

                AttrCollection frameAttrs = FetchParagraphFrameAttrs((Paragraph)node);
                if (!frameAttrs.ContainsAnyKey(ParaPr.FloatingAttrs))
                    return false;

                if (lastFrameAttrs != null && !frameAttrs.Equals(lastFrameAttrs))
                    return false;

                lastFrameAttrs = frameAttrs;
            }

            return true;
        }

        private static AttrCollection FetchParagraphFrameAttrs(Paragraph paragraph)
        {
            AttrCollection attrs = new ParaPr();
            if (paragraph == null)
                return attrs;

            paragraph.ParaPr.MirrorTo(attrs, gParaFrameAttrs);

            foreach (int key in gParaFrameAttrs)
            {
                if (attrs.ContainsKey(key))
                    continue;

                object value = ((IParaAttrSource)paragraph).FetchParaAttr(key);
                object defaultValue = ParaPr.FetchDefaultAttr(key);
                if (value.Equals(defaultValue))
                    continue;

                attrs.Add(key, value);
            }

            return attrs;
        }

        private static void MirrorFrameAttrs(AttrCollection frameAttrs, AttrCollection target)
        {
            if (frameAttrs.ContainsAnyKey(ParaPr.FloatingAttrs) || target.ContainsAnyKey(ParaPr.FloatingAttrs))
                frameAttrs.MirrorTo(target, ParaPr.BorderAttrs);
            frameAttrs.MirrorTo(target, ParaPr.FloatingAttrs);
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        internal static void Copy(NodeRange sourceRange, Node referenceNode)
        {
            Copy(sourceRange, referenceNode, null, null);
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        /// <param name="modifier">A node modifier object that modifies nodes being copied.</param>
        internal static void Copy(NodeRange sourceRange, Node referenceNode, INodeModifier modifier)
        {
            Copy(sourceRange, referenceNode, modifier, null);
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        /// <param name="nodeCloningListener">A node cloning listener object that listens nodes being copied.</param>
        internal static void Copy(NodeRange sourceRange, Node referenceNode, INodeCloningListener nodeCloningListener)
        {
            Copy(sourceRange, referenceNode, null, nodeCloningListener);
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        /// <param name="modifier">A node modifier object that modifies nodes being copied.</param>
        /// <param name="nodeCloningListener">A node cloning listener object that listens nodes being copied.</param>
        internal static void Copy(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCloningListener nodeCloningListener)
        {
            const NodeCopierOptions options = NodeCopierOptions.SkipCrossStructureAnnotations | NodeCopierOptions.CloneNode;
            Copy(sourceRange, referenceNode, modifier, nodeCloningListener, options);
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        /// <param name="modifier">A node modifier object that modifies nodes being copied.</param>
        /// <param name="nodeCloningListener">A node cloning listener object that listens nodes being copied.</param>
        /// <param name="options">Allows to define copying options.</param>
        internal static void Copy(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCloningListener nodeCloningListener,
            NodeCopierOptions options)
        {
            if (sourceRange.IsVoid)
                return;

            // In most cases, source range itself will work.
            NodeRange copierRange = sourceRange;

            // This is a workaround for copying ranges ending on a composite node when the ending composite node is included.
            bool isEndingCompositeNodeIncluded = sourceRange.End.Node.IsComposite && sourceRange.End.IsEnd;
            if (isEndingCompositeNodeIncluded)
            {
                // Move to the start of the next node on the same level or higher.
                // This works for the last node as well (the move is OK to fail).
                DocumentPosition newEnd = sourceRange.End.Clone();

                Node root = null;

                if (sourceRange.End.Node.Document is GlossaryDocument)
                    root = GetAncestorBelowBuildingBlock(sourceRange.End.Node);

                newEnd.Move(root, true, false, false, false, false);

                copierRange = new NodeRange(sourceRange.Start, newEnd);
            }

            using (NodeCopier copier = new NodeCopier(
                copierRange,
                referenceNode,
                modifier,
                nodeCloningListener,
                options))
            {
                copier.Copy();
            }
        }

        private static Node GetAncestorBelowBuildingBlock(Node node)
        {
            Node cur = node;
            while (true)
            {
                if (cur.ParentNode == null)
                    return null;

                if (cur.ParentNode.NodeType == NodeType.BuildingBlock)
                    return cur;

                cur = cur.ParentNode;
            }
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document, replacing fields with the field results.
        /// WORDSNET-6913 Preserves field codes of FWR-fields if their removal is not forced.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        /// <param name="modifier">A node modifier object that modifies nodes being copied.</param>
        /// <param name="nodeCopierListener">A node copier listener object that listens nodes being copied.</param>
        /// <param name="removeFwrFields">TRUE to force FWR-fields' removal.</param>
        internal static NodeRange CopyWithoutFields(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCopierListener nodeCopierListener,
            bool removeFwrFields)
        {
            return CopyWithoutFields(
                sourceRange,
                referenceNode,
                modifier,
                nodeCopierListener,
                removeFwrFields ? FieldRemoverDefaultFilter.Instance : FieldRemoverRetainFwrFieldsFilter.Instance);
        }

        internal static NodeRange CopyWithoutFields(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCopierListener nodeCopierListener,
            bool removeFwrFields,
            NodeCopierOptions options)
        {
            return CopyWithoutFields(
                sourceRange,
                referenceNode,
                modifier,
                nodeCopierListener,
                removeFwrFields ? FieldRemoverDefaultFilter.Instance : FieldRemoverRetainFwrFieldsFilter.Instance,
                options);
        }

        internal static NodeRange CopyWithoutFields(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCopierListener nodeCopierListener,
            IFieldRemoverFilter fieldRemoverFilter)
        {
            return CopyWithoutFields(
                sourceRange,
                referenceNode,
                modifier,
                nodeCopierListener,
                fieldRemoverFilter,
                NodeCopierOptions.SkipCrossStructureAnnotations | NodeCopierOptions.CloneNode);
        }

        /// <summary>
        /// Copies (clones and inserts) nodes from a source range to some point in the document, replacing fields with the field results.
        /// WORDSNET-6913 Preserves field codes of FWR-fields if their removal is not forced.
        /// </summary>
        /// <param name="sourceRange">A node range to copy nodes from.</param>
        /// <param name="referenceNode">A node in the document before which the cloned nodes will be inserted.</param>
        /// <param name="modifier">A node modifier object that modifies nodes being copied.</param>
        /// <param name="nodeCopierListener">A node copier listener object that listens nodes being copied.</param>
        /// <param name="fieldRemoverFilter">A field remover filter.</param>
        /// <param name="options">Allows to define copying options.</param>
        internal static NodeRange CopyWithoutFields(
            NodeRange sourceRange,
            Node referenceNode,
            INodeModifier modifier,
            INodeCopierListener nodeCopierListener,
            IFieldRemoverFilter fieldRemoverFilter,
            NodeCopierOptions options)
        {
            DocumentPosition beforeInsertedRange = DocumentPosition.CreatePositionBefore(referenceNode);
            beforeInsertedRange.Move(null, false, false, false, false, false);

            Copy(sourceRange, referenceNode, modifier, nodeCopierListener, options);

            DocumentPosition insertedRangeEnd = DocumentPosition.CreatePositionBefore(referenceNode);

            NodeRange insertedRange = new NodeRange(beforeInsertedRange, insertedRangeEnd);

            if (nodeCopierListener != null)
                nodeCopierListener.NotifyNodeRangeCopied(sourceRange, insertedRange);

            return FieldRemover.ReplaceFieldsWithResults(insertedRange, fieldRemoverFilter);
        }

        /// <summary>
        /// Performs actual copying.
        /// </summary>
        private void Copy()
        {
            CopyStartNodeAncestors();
            Traverse();
        }

        /// <summary>
        /// Copies the start node's ancestors that should wrap the start node it to be inserted in a right place.
        /// For instance, contents of a table should be wrapped.
        /// </summary>
        private void CopyStartNodeAncestors()
        {
            // SPEED Do not traverse through the range if it is surely unneeded.
            if (!HasPossibleStartNodeAncestorToCopy())
                return;

            Node topAncestor;
            // Find the top start node ancestor encountered while traversal.
            using (TopStartNodeAncestorFinder finder = new TopStartNodeAncestorFinder(Range))
                topAncestor = finder.GetTopStartNodeAncestor();

            // If it is not found then simply return as we deal with a plain node range.
            if (topAncestor == null)
                return;

            // Exclude ancestors that are not used as wrappers.
            Node startNode = Range.Start.Node;
            while ((topAncestor != startNode) && !NeedCopyEdgeNodeAncestor(topAncestor))
                topAncestor = GetTopAncestorNodeBelow(startNode, topAncestor);

            // If wrapping is not required, return.
            if (topAncestor == startNode)
                return;

            // Include ancestors to perform additional wrapping if needed.
            while (NeedWrapEdgeNodeAncestorByParent(topAncestor))
                topAncestor = topAncestor.ParentNode;

            // Insert valid wrapping ancestors from the top to the bottom.
            for (Node ancestor = topAncestor;
                 (ancestor != startNode) && IsRangeNodeValid(ancestor);
                 ancestor = GetTopAncestorNodeBelow(startNode, ancestor))
            {
                CloneAndInsertEdgeNodeAncestor(ancestor);
            }

            mTopStartNodeAncestor = topAncestor;
        }

        /// <summary>
        /// Returns a value indicating whether the start of the range being copied has an ancestor which
        /// should be copied if its boundary is crossed while traversing of the range.
        /// </summary>
        private bool HasPossibleStartNodeAncestorToCopy()
        {
            for (Node ancestor = Range.Start.Node; ancestor != null; ancestor = ancestor.ParentNode)
            {
                if (NeedCopyEdgeNodeAncestor(ancestor))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating whether the specified edge (i.e. start or end) node ancestor
        /// should be copied to wrap its descendants.
        /// </summary>
        private static bool NeedCopyEdgeNodeAncestor(Node ancestor)
        {
            switch (ancestor.NodeType)
            {
                case NodeType.Table:
                case NodeType.Shape:
                case NodeType.OfficeMath:
                    return true;
                default:
                    return (NodeUtil.IsRowLevelNode(ancestor) || NodeUtil.IsCellLevelNode(ancestor)) &&
                        !NodeUtil.IsCrossStructureAnnotation(ancestor);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified edge (i.e. start or end) node ancestor
        /// requires extra wrapping by the parent node.
        /// </summary>
        private bool NeedWrapEdgeNodeAncestorByParent(Node ancestor)
        {
            if ((ancestor.NodeLevel == NodeLevel.Row) && (mReferenceNode.NodeLevel != NodeLevel.Row))
                return true;

            if ((ancestor.NodeLevel == NodeLevel.Cell) && (mReferenceNode.NodeLevel != NodeLevel.Cell))
                return true;

            if (ancestor.NodeType == NodeType.OfficeMath && !((OfficeMath)ancestor).IsTopLevel)
                return true;

            return false;
        }

        /// <summary>
        /// Returns the top ancestor node for the specified descendant node just right below the specified ancestor node.
        /// </summary>
        private static Node GetTopAncestorNodeBelow(Node descendant, Node topAncestor)
        {
            Node node = descendant;
            while (node.ParentNode != topAncestor)
                node = node.ParentNode;

            return node;
        }

        /// <summary>
        /// Clones the specified edge (i.e. start or end) node ancestor and inserts it where appropriate.
        /// </summary>
        private void CloneAndInsertEdgeNodeAncestor(Node ancestor)
        {
            Node ancestorToInsert = ExtractRangeNode(ancestor, false);
            // Note, that at the moment there is no INodeModifier implementation which can reject wrapping nodes' copying.
            // If this will be changed then the class should be reworked to reject wrapped nodes' copying also for example.
            Debug.Assert(ancestorToInsert != null);

            PrepareAncestorNodeInsertion(ancestor);
            InsertNode(ancestor, ancestorToInsert);
            mCurrentReferenceParentNode = (CompositeNode)ancestorToInsert;
        }

        /// <summary>
        /// Shortcut overload.
        /// </summary>
        private Node ExtractRangeNode(Node rangeNode, bool modifyChildren)
        {
            Node node = ExtractRangeNode(rangeNode, mModifier, mNodeCloningListener, CloneNode, modifyChildren);

            if ((mReferenceParagraphFrameAttrs != null) && (node != null) && (node.NodeType == NodeType.Paragraph))
                MirrorFrameAttrs(mReferenceParagraphFrameAttrs, ((Paragraph)node).ParaPr);

            return node;
        }

        /// <summary>
        /// Prepares a composite node insertion by separating the reference node with a section or paragraph break if needed.
        /// </summary>
        private void PrepareAncestorNodeInsertion(Node sourceAncestor)
        {
            if (HasCurrentReferenceParentNode)
                return;

            switch (sourceAncestor.NodeType)
            {
                case NodeType.Paragraph:
                case NodeType.Table:
                case NodeType.StructuredDocumentTag:
                    {
                        if (!Range.IsInSameAncestorOrSelf(NodeType.Section))
                        {
                            // andrnosk: We should not insert section break if the current node is last child of source section,
                            // or it is inline node (!IsComposite - it means that reference node is Field or Bookmark)
                            if ((sourceAncestor.NextNonAnnotationSibling != null) || !mReferenceNode.IsComposite)
                            {
                                // The current node is in the middle, and it seems like the end node of source
                                // range is in another section. Insert a section break before the reference node if needed.
                                SeparateReferenceNodeWithSectionBreak();
                            }
                        }
                        else
                        {
                            // The current node is in the middle, and it seems like the end node of source
                            // range is in the same section. Insert a paragraph break before the reference node if needed.
                            SeparateReferenceNodeWithParagraphBreak(sourceAncestor);
                        }

                        break;
                    }
                case NodeType.Section:
                    {
                        // The current section is in the middle, insert a section break before the reference node if needed.
                        SeparateReferenceNodeWithSectionBreak();
                        break;
                    }
                default:
                    {
                        // Do nothing.
                        break;
                    }
            }
        }

        protected override void OnStartNodeAncestor()
        {
            // Do not process an invalid node.
            if (!IsCurrentNodeValid())
                return;

            // Do not process a start node ancestor if wrapping is not required.
            if (!HasCurrentReferenceParentNode)
            {
                if (ProcessBoundBlockAnnotationAsInline &&
                    NodeUtil.IsCrossStructureAnnotation(Range.End.Node) &&
                    (Range.End.Node.PreviousNonAnnotationSibling == CurrentNode))
                    SeparateReferenceNode(CurrentNode);

                return;
            }

            // Set the current reference node for a sibling node if no more wrapping is required.
            mCurrentReferenceNode = mCurrentReferenceParentNode;

            // Set the current parent reference node to wrap a sibling node if needed.
            mCurrentReferenceParentNode = (CurrentNode != mTopStartNodeAncestor)
                ? mCurrentReferenceParentNode.ParentNode
                : null;
        }

        protected override void OnMiddleNodeAncestor()
        {
            // Do not process an invalid node.
            if (!IsCurrentNodeValid())
                return;

            if (NeedToMoveToInlineLevel())
            {
                // Move to compatible node level (to inline) on start of copying only.
                mMoveToInlineLevel = false;

                foreach (Node node in ((CompositeNode)CurrentNode).GetChildNodes(NodeType.Any, false))
                {
                    if (node.IsComposite)
                        CopyMiddleNode(node);
                    else
                        CloneAndInsert(node);
                }

                SeparateReferenceNode(CurrentNode);
            }
            else
            {
                CopyMiddleNode(CurrentNode);
            }
        }

        /// <summary>
        /// Copies a node in a middle of copying range into the destination.
        /// </summary>
        private void CopyMiddleNode(Node node)
        {
            // Clone and insert the encountered ancestor node.
            PrepareAncestorNodeInsertion(node);
            CloneAndInsert(node);
        }

        /// <summary>
        /// Returns <c>true</c> if contents of the current node (paragraph) should be copied to the destination
        /// to inline level without adding a new paragraph.
        /// </summary>
        private bool NeedToMoveToInlineLevel()
        {
            Node referenceNode = mCurrentReferenceNode != null ? mCurrentReferenceNode : mReferenceNode;
            return mMoveToInlineLevel && (CurrentNode.NodeLevel == NodeLevel.Block) && (referenceNode.NodeLevel == NodeLevel.Inline);
        }

        protected override void OnEndNodeAncestor()
        {
            // Stop traversal if the current node is invalid as all of its descendants are invalid either.
            if (!IsCurrentNodeValid())
            {
                StopTraverse();
                return;
            }

            // If the current end node ancestor's descendants should be wrapped by the ancestor,
            // clone and insert it where appropriate.
            if (HasCurrentReferenceParentNode || NeedCopyEdgeNodeAncestor(CurrentNode))
            {
                CloneAndInsertEdgeNodeAncestor(CurrentNode);
                return;
            }

            Node startNode = Range.Start.Node;
            if (!(ProcessBoundBlockAnnotationAsInline &&
                    NodeUtil.IsCrossStructureAnnotation(startNode) &&
                    (startNode.NodeLevel == NodeLevel.Block) &&
                    (startNode.NextNonAnnotationSibling == CurrentNode)))
                SeparateReferenceNode(CurrentNode);
        }

        /// <summary>
        /// Separates the reference node with a section or paragraph break if needed.
        /// </summary>
        private void SeparateReferenceNode(Node closedSourceNode)
        {
            switch (closedSourceNode.NodeType)
            {
                case NodeType.Paragraph:
                    {
                        SeparateReferenceNodeWithParagraphBreak(closedSourceNode);
                        mCurrentReferenceNode = mReferenceNode;
                        mCurrentReferenceNode = FindValidReferenceNode();
                        mIsInLastParent = true;
                        break;
                    }
                case NodeType.Section:
                    {
                        SeparateReferenceNodeWithSectionBreak();
                        mCurrentReferenceNode = mReferenceNode;
                        mCurrentReferenceNode = FindValidReferenceNode();
                        mIsInLastParent = true;
                        break;
                    }
                default:
                    {
                        // Do nothing.
                        break;
                    }
            }
        }

        protected override void OnNonCompositeNode()
        {
            CloneAndInsert(CurrentNode);
        }

        /// <summary>
        /// Clones the specified node and inserts it where appropriate.
        /// </summary>
        private void CloneAndInsert(Node node)
        {
            // WORDSNET-11803 Do not insert paragraph with section break, because extra paragraph already has been inserted in SeparateReferenceNodeWithPageBreak.
            if (mIsPageBreakInserted && IsSectionBreakParagraph(node))
                return;

            Node nodeToInsert = ExtractRangeNode(node, true);
            InsertNode(node, nodeToInsert);
        }

        protected override bool NeedSkipRangeNodeExtract(Node rangeNode)
        {
            return
                ((mOptions & NodeCopierOptions.SkipCrossStructureAnnotations) != 0) &&
                NodeUtil.IsCrossStructureAnnotation(rangeNode);
        }

        /// <summary>
        /// Inserts the specified node.
        /// </summary>
        private void InsertNode(Node sourceNode, Node nodeToInsert)
        {
            if (nodeToInsert == null)
                return;

            if (HasCurrentReferenceParentNode)
            {
                InsertChildNode(nodeToInsert);
            }
            else if ((mCurrentReferenceNode == null) || mIsInLastParent)
            {
                InsertSiblingNodeBefore(sourceNode, nodeToInsert);
            }
            else
            {
                InsertSiblingNodeAfter(sourceNode, nodeToInsert);
            }
        }

        /// <summary>
        /// Appends the specified node as a child to the current destination parent node.
        /// </summary>
        private void InsertChildNode(Node nodeToInsert)
        {
            mCurrentReferenceParentNode.AppendChild(nodeToInsert);
        }

        /// <summary>
        /// Inserts the specified node after the current destination node and then makes it current destination node.
        /// </summary>
        private void InsertSiblingNodeAfter(Node sourceNode, Node nodeToInsert)
        {
            mCurrentReferenceNode = FindValidReferenceNode(sourceNode);
            mCurrentReferenceNode = mCurrentReferenceNode.InsertNext(nodeToInsert);
        }

        /// <summary>
        /// Inserts the specified node before the current destination node and then makes it current destination node.
        /// </summary>
        private void InsertSiblingNodeBefore(Node sourceNode, Node nodeToInsert)
        {
            bool isFirstChild = (mCurrentReferenceNode == null);

            mCurrentReferenceNode = FindValidReferenceNode(sourceNode);
            mCurrentReferenceNode.InsertPrevious(nodeToInsert);

            if (isFirstChild)
                mCurrentReferenceNode = nodeToInsert;
        }

        protected override void OnMoved(DocumentPositionMovement movement)
        {
            if (HasCurrentReferenceParentNode)
                return;

            switch (movement)
            {
                case DocumentPositionMovement.Above:
                case DocumentPositionMovement.Below:
                    mCurrentReferenceNode = FindValidReferenceNode();
                    break;
                default:
                    break;
            }
        }

        private void SeparateReferenceNodeWithPageBreak(bool insertParagraph)
        {
            if (mIsReferenceNodeSeparated)
                return;

            mBuilder.MoveTo(mReferenceNode);

            if (insertParagraph)
                mBuilder.InsertParagraph();

            mBuilder.Write(ControlChar.PageBreak);

            mIsReferenceNodeSeparated = true;
            mIsPageBreakInserted = true;
        }

        private void SeparateReferenceNodeWithParagraphBreak(Node sourceNode)
        {
            // andrnosk: do not insert paragraph, if source node type is the same as reference node type,
            // for example inserting run inside paragraph. And also if reference node is the only one run inside paragraph.
            if (mIsReferenceNodeSeparated || (mReferenceNode.NodeLevel == sourceNode.NodeLevel)
                || ((mReferenceNode.NodeType == NodeType.Run) && mReferenceNode.IsFirstChild))
                return;

            mBuilder.MoveTo(mReferenceNode);

            // WORDSNET-16519 Workaround. The DocumentBuilder.InsertParagraph loses ParagraphBreakRunPr.
            RunPr currentParaBreakPr = mBuilder.CurrentParagraph.ParagraphBreakRunPr.Clone();
            mBuilder.InsertParagraph();
            mBuilder.CurrentParagraph.ParagraphBreakRunPr = currentParaBreakPr;

            SetStartParaPr(mBuilder.CurrentParagraph.PreviousNonAnnotationSibling);

            mIsReferenceNodeSeparated = true;
        }

        /// <summary>
        /// If the specified start node is paragraph, sets its properties equal to the ones owned by the start paragraph
        /// of the source range to fit MS Word behavior. However, if <see cref="UseSourceStartAncestorPr"/> is
        /// <c>false</c>, does nothing.
        /// </summary>
        private void SetStartParaPr(Node startNode)
        {
            if (!UseSourceStartAncestorPr)
                return;

            if ((startNode == null) || (startNode.NodeType != NodeType.Paragraph))
                return;

            Paragraph startParagraph = (Paragraph)startNode;

            Node rangeStartNode = Range.Start.Node;
            Node sourceStartParagraphNode =
                NodeUtil.IsCrossStructureAnnotation(rangeStartNode) && (rangeStartNode.NodeLevel != NodeLevel.Inline)
                    ? NodeUtil.FindNextParagraph(rangeStartNode)
                    : NodeUtil.GetAncestorOrSelf(rangeStartNode, NodeType.Paragraph);
            if ((sourceStartParagraphNode == null) || (sourceStartParagraphNode.NodeType != NodeType.Paragraph))
                return;

            Paragraph sourceStartParagraph = (Paragraph)sourceStartParagraphNode;

            if (mReferenceParagraphFrameAttrs != null)
                CloneParaPrPreserveFrameAttrs(startParagraph, sourceStartParagraph.ParaPr);
            else
                startParagraph.ParaPr = sourceStartParagraph.ParaPr.Clone();

            startParagraph.ParagraphBreakRunPr = sourceStartParagraph.ParagraphBreakRunPr.Clone();
        }

        private void SeparateReferenceNodeWithSectionBreak()
        {
            if (mReferenceNode.GetAncestor(NodeType.Body) == null)
                return;

            if (mReferenceNode.GetAncestor(NodeType.Footnote) != null)
            {
                // WORDSNET-11803 MS Word does not insert section break inside footnotes. Insert page break instead.
                SeparateReferenceNodeWithPageBreak(true);
                return;
            }

            if (mReferenceNode.GetAncestor(NodeType.Shape) != null)
            {
                // WORDSNET-21966 MS Word does not insert section break inside shape. Insert page break instead.
                SeparateReferenceNodeWithPageBreak(false);
                return;
            }

            if (mIsReferenceNodeSeparated)
                return;

            mBuilder.MoveTo(mReferenceNode);

            // We have to detect the section start type of the last source section.
            Section section = (Section)NodeUtil.GetAncestorOrSelf(Range.End.Node, NodeType.Section);
            SectionStart sectionStart = section.PageSetup.SectionStart;

            mBuilder.InsertSection(sectionStart);

            if (ProlongRangeStartSectionHeadersFooters)
            {
                Section startSection = (Section)NodeUtil.GetAncestorOrSelf(Range.Start.Node, NodeType.Section);

                if (section != startSection)
                {
                    foreach (HeaderFooter headerFooter in section.HeadersFooters.ToArray())
                        mBuilder.CurrentSection.HeadersFooters.Add(headerFooter);

                    foreach (HeaderFooter headerFooter in startSection.HeadersFooters.ToArray())
                    {
                        if (!IsDefaultHeaderFooterContent(headerFooter))
                            section.HeadersFooters.Add(headerFooter.Clone(true));
                    }
                }
            }

            // andrnosk: When we move cursor to paragraph, its placed at the end of paragraph,
            // that is why section break is inserted at the end of paragraph,
            // but we need to insert section break before the paragraph, so move reference paragraph to the next (inserted) section.
            if (mReferenceNode.NodeType == NodeType.Paragraph)
                mBuilder.CurrentSection.Body.PrependChild(mReferenceNode);

            SetStartSectPr();

            // andrnosk: Remove empty paragraph which was created upon inserting new section.
            if (!mBuilder.CurrentParagraph.HasChildNodes)
                mBuilder.CurrentParagraph.Remove();

            mIsReferenceNodeSeparated = true;
        }

        private static bool IsDefaultHeaderFooterContent(HeaderFooter headerFooter)
        {
            if (!headerFooter.HasChildNodes)
                return false;

            if (headerFooter.Count != 1)
                return false;

            Paragraph paragraph = headerFooter.FirstParagraph;
            if (paragraph == null)
                return false;

            return !paragraph.HasChildNodes;
        }

        /// <summary>
        /// Sets properties of the first destination ancestor section equal to the ones owned by the start section
        /// of the source range to fit MS Word behavior. However, if <see cref="UseSourceStartAncestorPr"/> is
        /// <c>false</c>, does nothing.
        /// </summary>
        private void SetStartSectPr()
        {
            if (!UseSourceStartAncestorPr)
                return;

            Section startSection = (Section)mBuilder.CurrentSection.PreviousSibling;
            if (startSection == null)
                return;

            Section sourceStartSection = (Section)NodeUtil.GetAncestorOrSelf(Range.Start.Node, NodeType.Section);
            startSection.SectPr = sourceStartSection.SectPr.Clone();

            // If the last child of the start section is a paragraph, set its properties either.
            SetStartParaPr(startSection.Body.LastNonAnnotationChild);
        }

        /// <summary>
        /// Returns a reference node of the same level as the new node to be inserted.
        /// </summary>
        private Node FindValidReferenceNode()
        {
            Node sourceNode = (CurrentNode != null) ? CurrentNode : Range.Start.Node;
            return FindValidReferenceNode(sourceNode);
        }

        /// <summary>
        /// Returns a reference node of the same level as the new node to be inserted.
        /// </summary>
        private Node FindValidReferenceNode(Node sourceNode)
        {
            if (mCurrentReferenceNode == null)
                mCurrentReferenceNode = mReferenceNode;

            if (sourceNode.NodeLevel == mCurrentReferenceNode.NodeLevel)
                return mCurrentReferenceNode;

            if (IsCurrentReferenceValidForCrossStructureAnnotation(sourceNode))
                return mCurrentReferenceNode;

            if (!mIsInLastParent)
            {
                Node referenceNode = FindValidReferenceNodeAbove(sourceNode, mCurrentReferenceNode);
                return (referenceNode != null) ? referenceNode : FindValidReferenceNodeBelow(sourceNode, mCurrentReferenceNode);
            }
            else
            {
                return FindValidReferenceNodeAbove(sourceNode, mReferenceNode);
            }
        }

        private bool IsCurrentReferenceValidForCrossStructureAnnotation(Node sourceNode)
        {
            return mMoveToInlineLevel &&
                   sourceNode.NodeLevel == NodeLevel.Block &&
                   NodeUtil.IsCrossStructureAnnotation(sourceNode) &&
                   mCurrentReferenceNode.NodeLevel == NodeLevel.Inline;
        }

        /// <summary>
        /// Returns a reference node of the same level as the new node to be inserted.
        /// Looks for it in above levels (node parents).
        /// </summary>
        private static Node FindValidReferenceNodeAbove(Node sourceNode, Node referenceNode)
        {
            while ((referenceNode != null) && (referenceNode.NodeLevel != sourceNode.NodeLevel))
            {
                // WORDSNET-24236 Place SDT ranges to appropriate parent.
                if ((sourceNode.NodeLevel == NodeLevel.Other) && referenceNode.ParentNode.CanInsert(sourceNode))
                    return referenceNode;

                referenceNode = GetValidParent(referenceNode.ParentNode);
            }

            return referenceNode;
        }

        /// <summary>
        /// Returns a reference node of the same level as the new node to be inserted.
        /// Looks for it in below levels (node children).
        /// </summary>
        private static Node FindValidReferenceNodeBelow(Node sourceNode, Node referenceNode)
        {
            while ((referenceNode != null) && (referenceNode.IsComposite) && (referenceNode.NodeLevel != sourceNode.NodeLevel))
                referenceNode = GetValidChild(((CompositeNode)referenceNode).LastChild);

            return referenceNode;
        }

        private bool HasCurrentReferenceParentNode
        {
            get { return (mCurrentReferenceParentNode != null); }
        }

        private static bool IsSectionBreakParagraph(Node node)
        {
            if (node.NodeType != NodeType.Paragraph)
                return false;

            return ((Paragraph)node).IsSectionBreakParagraph;
        }

        /// <summary>
        /// Finds the top start node ancestor encountered while a node range traversal.
        /// </summary>
        private class TopStartNodeAncestorFinder : NodeTraverser
        {
            internal TopStartNodeAncestorFinder(NodeRange range)
                : base(range)
            {
            }

            internal Node GetTopStartNodeAncestor()
            {
                Traverse();
                return mTopStartNodeAncestor;
            }

            protected override void OnStartNodeAncestor()
            {
                mTopStartNodeAncestor = CurrentNode;
            }

            protected override void OnEndNodeAncestor()
            {
                StopTraverse();
            }

            private Node mTopStartNodeAncestor;
        }

        /// <summary>
        /// Indicates whether to replace every destination start node ancestor's properties with the corresponding
        /// source ones in case when any ancestor of the same type can be met while a source range traversal.
        /// </summary>
        private bool UseSourceStartAncestorPr
        {
            get { return (mOptions & NodeCopierOptions.UseSourceStartAncestorPr) != 0; }
        }

        private bool ProlongRangeStartSectionHeadersFooters
        {
            get { return (mOptions & NodeCopierOptions.ProlongRangeStartSectionHeadersFooters) != 0; }
        }

        /// <summary>
        /// When true, then node need to be cloned while extracting it within the enumerated range.
        /// </summary>
        private bool CloneNode
        {
            get { return (mOptions & NodeCopierOptions.CloneNode) != 0; }
        }

        /// <summary>
        /// If <c>true</c>, when the range has block level annotation nodes at bounds, copying gives same result
        /// as when the annotation nodes are moved to inline level.
        /// </summary>
        private bool ProcessBoundBlockAnnotationAsInline
        {
            get { return (mOptions & NodeCopierOptions.ProcessBoundBlockAnnotationAsInline) != 0; }
        }

        /// <summary>
        /// A node before which source nodes will be copied.
        /// </summary>
        private readonly Node mReferenceNode;
        private readonly AttrCollection mReferenceParagraphFrameAttrs;
        /// <summary>
        /// An object modifying nodes while copying.
        /// </summary>
        private readonly INodeModifier mModifier;
        private readonly INodeCloningListener mNodeCloningListener;
        private readonly DocumentBuilder mBuilder;
        private readonly NodeCopierOptions mOptions;
        private Node mTopStartNodeAncestor;
        private CompositeNode mCurrentReferenceParentNode;
        /// <summary>
        /// The current reference node.
        /// </summary>
        private Node mCurrentReferenceNode;
        /// <summary>
        /// True if we are in the last parent (that owns the last node).
        /// </summary>
        private bool mIsInLastParent;
        /// <summary>
        /// True if we have one or more paragraph or section breaks in the sequence and we have already inserted one
        /// to separate the reference node.
        /// </summary>
        private bool mIsReferenceNodeSeparated;
        /// <summary>
        /// True if paragraph with page break has been inserted instead of section break.
        /// </summary>
        private bool mIsPageBreakInserted;

        /// <summary>
        /// True means that the range is started with a block level node but the reference node is inline.
        /// The first composite node of the range will not be copied, just its contents. The flag may be set only
        /// if <see cref="ProcessBoundBlockAnnotationAsInline"/> is <c>true</c>.
        /// </summary>
        private bool mMoveToInlineLevel;

        private static readonly int[] gParaFrameAttrs = BuildFrameAttrs();

        private static int[] BuildFrameAttrs()
        {
            IntList frameAttrs = new IntList();
            frameAttrs.AddRange(ParaPr.FloatingAttrs);
            frameAttrs.AddRange(ParaPr.BorderAttrs);
            return frameAttrs.ToArray();
        }
    }
}
