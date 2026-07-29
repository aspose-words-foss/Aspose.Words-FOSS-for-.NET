// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2017 by Dmitry Sokolov

using System.Collections.Generic;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words.RW
{
    /// <summary>
    /// Implements ability to resolve unexpected nodes which occurred into inline SDT.
    /// </summary>
    internal class InlineMarkupResolver
    {
        /// <summary>
        /// Ctr.
        /// </summary>
        /// <param name="markupStartsTable">Dictionary of nodes where IMarkupNode of block level is started.</param>
        internal InlineMarkupResolver(IDictionary<IMarkupNode, Node> markupStartsTable) 
        {
            mMarkupStartsTable = markupStartsTable;
        }

        /// <summary>
        /// Adds new root to sequence for resolve.
        /// </summary>
        /// <param name="sdt">Node to append.</param>
        internal void AddToSequence(StructuredDocumentTag sdt)
        {
            if (mTopLevelInline != null)
                return;

            mTopLevelInline = sdt;
            mInlineSequence.Add(mTopLevelInline);
        }

        /// <summary>
        /// Initializes addition of the node with specified type to collection for resolve.
        /// </summary>
        /// <param name="sdt">SDT which contains node with specified type.</param>
        /// <param name="nodeType">Type of the node which will be read.</param>
        /// <returns>Index of the reserved place in the collection or -1 when reservation does not required.</returns>
        internal int StartPendingInline(StructuredDocumentTag sdt, NodeType nodeType)
        {
            Debug.Assert((sdt != null));
            const int emptyIndex = -1;
 
            if (sdt.Level != MarkupLevel.Inline)
                return emptyIndex;

            IList<Node> pendingNodes = null;

            // MSW builds path from top level inline SDT and goes deeper while is not occurred
            // last or valid inline nested node. So, collect nodes which is invalid and have to
            // be resolved on this path.
            if (mTopLevelInline != null)
            {
                // Create appropriate collection for nodes to resolve if it is not exist.
                if (!mInlinePendingTable.ContainsKey(mTopLevelInline))
                {
                    // This is first element inside SDT, so append parent paragraph for resolve.
                    pendingNodes = new List<Node>();
                    Paragraph para = (Paragraph)sdt.GetAncestor(NodeType.Paragraph);

                    // When parent paragraph already exist among nodes for resolve then skip it.
                    if (!CheckFirstParagraphIsUsed(para))
                        pendingNodes.Add(para);

                    // Set actual parent.
                    SetBlockLevelParent(mTopLevelInline, para);
                    // Set collection with nodes for resolve.
                    mInlinePendingTable[mTopLevelInline] = pendingNodes;
                }

                // Reserve place for element which will be read.
                pendingNodes = mInlinePendingTable[mTopLevelInline];
                pendingNodes.Add(null);
            }

            // Reset to process cell level correctly.
            if (nodeType == NodeType.Table)
                ResetInlineContext();

            return (pendingNodes != null) ? pendingNodes.Count - 1 : emptyIndex;
        }

        /// <summary>
        /// Completes addition of the node which was initialized with 
        /// <see cref="InlineMarkupResolver.StartPendingInline"/>.
        /// </summary>
        /// <param name="sdt">SDT which contains read node.</param>
        /// <param name="node">Node which was read.</param>
        /// <param name="index">Index in the collection which was returned by <see cref="InlineMarkupResolver.StartPendingInline"/>.</param>
        internal void EndPendingInline(StructuredDocumentTag sdt, Node node, int index)
        {
            Debug.Assert(node != null);

            if ((sdt != null) && (sdt.Level != MarkupLevel.Inline))
                return;

            // Inline SDT (root) was extracted, current node have to be added to block level.
            // When "index" < 0, then "sdr" equals to null.
            if (sdt == null)
            {
                SetBlockLevelParent(node, node);
                mInlineSequence.Add(node);
                return;
            }

            // Set value into reserved place. 
            mInlinePendingTable[sdt][index] = node;
            // Get actual count of the pending elements.
            int pendingAfter = GetPendingInlineCount();

            // If node which was read contains another SDT wrapped into inline then
            // after reading this node a pending elements count will be increased.
            // Check that passed node contains only inline elements, so current collection with nodes
            // to extract is complete.
            if ((index + 1) == pendingAfter)
                ResetInlineContext();
        }

        /// <summary>
        /// Resolves accumulated nodes.
        /// </summary>
        internal void ProcessInline(StructuredDocumentTag sdt)
        {
            Debug.Assert(sdt != null);

            if (sdt.Level != MarkupLevel.Inline)
                return;

            // When nothing to resolve SDT or passed markup node is not top level among
            // nested inline SDT, then skip the processing.
            if ((mInlineSequence.Count == 0) || (mInlineSequence[0] != sdt))
            {
                if (mTopLevelInline == sdt)
                    ResetInlineContext();

                return;
            }

            IList<Node> pendingNodes = null;
            CompositeNode inlineAncestor;
            Node resolveRoot;

            for (int i = 0; i < mInlineSequence.Count; ++i)
            {
                resolveRoot = mInlineSequence[i];
                pendingNodes = GetPendingInlineNodes(i);
                inlineAncestor = mBlockLevelParents.GetValueOrNull(resolveRoot);

                // It can be inline SDT without block level nodes, so is nothing to process.
                if (inlineAncestor == null)
                    break;

                // WORDSNET-15210 Implement reading of block level nodes inside inline SDT.
                ProcessPendingNodes(pendingNodes, i);
            }

            mParasToAppend.Clear();
            mInlineSequence.Clear();
            mBlockLevelParents.Clear();
            mInlinePendingTable.Clear();
            mParasToAppendParents.Clear();
            // Reset top level inline SDT.
            ResetInlineContext();
        }

        /// <summary>
        ///Process specified pending nodes collection.
        /// </summary>
        private void ProcessPendingNodes(IList<Node> pendingNodes, int seqIndex)
        {
            Node curNode;

            for (int j = 0; j < pendingNodes.Count; j++)
            {
                curNode = pendingNodes[j];

                switch (curNode.NodeType)
                {
                    case NodeType.Table:
                        ProcessNestedInlineTable((Table)curNode, seqIndex);
                        break;
                    case NodeType.Paragraph:
                        ProcessNestedInlinePara((Paragraph)curNode, seqIndex, j);
                        break;
                    default:
                        break;
                }

                // Flush paragraphs which have to be moved.
                FlushParagraphs(curNode);
            }
        }

        /// <summary>
        /// Moves paragraphs which have to be moved after specified node.
        /// </summary>
        private void FlushParagraphs(Node node)
        {
            Debug.Assert(node != null);

            if (!mParasToAppend.ContainsKey(node))
                return;

            IList<Node> nodesToAppend = mParasToAppend[node];

            foreach (Node para in nodesToAppend)
            {
                if (!mParasToAppendParents.ContainsKey(para))
                    continue;

                CompositeNode inlineAncestor = mParasToAppendParents[para];

                if ((inlineAncestor != null) && (para != inlineAncestor.LastChild))
                    inlineAncestor.InsertBefore(para, inlineAncestor.LastChild);
            }
        }

        /// <summary>
        /// Retrieves nodes count which pending to be resolved for current root.
        /// </summary>
        private int GetPendingInlineCount()
        {
            if ((mTopLevelInline == null) || !mInlinePendingTable.ContainsKey(mTopLevelInline))
                return 0;

            return mInlinePendingTable[mTopLevelInline].Count;
        }

        /// <summary>
        /// Sets current parent for specified node into map using the key.
        /// </summary>
        private void SetBlockLevelParent(Node key, Node curNode)
        {
            // Re-calculate block level parent when it have to be changed. For example,
            // it can happened when table occurred in inline level.
            if (mBlockLevelParent == null)
                mBlockLevelParent = GetBlockLevelParent(curNode);

            // Save block level parent for specified node.
            mBlockLevelParents[key] = mBlockLevelParent;
        }

        /// <summary>
        /// Checks that specified paragraph exist among first nodes in the collections for resolve.
        /// </summary>
        private bool CheckFirstParagraphIsUsed(Paragraph para)
        {
            NodeCollection sdts = para.GetChildNodes(NodeType.StructuredDocumentTag, false);

            foreach (Node key in sdts)
            {
                if (!mInlinePendingTable.ContainsKey(key))
                    continue;

                IList<Node> usedNodes = mInlinePendingTable[key];
                // Looks like it can be just first element in the sequence.
                if ((usedNodes.Count > 0) && (usedNodes[0] == para))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines actual parent for specified node.
        /// </summary>
        private static CompositeNode GetBlockLevelParent(Node curNode)
        {
            CompositeNode parent = curNode.ParentNode;

            while (parent != null)
            {
                if (parent.CanInsert(curNode) && ((parent.NodeType != NodeType.StructuredDocumentTag) ||
                    (((IMarkupNode)parent).Level_IMarkupNode != MarkupLevel.Inline)))
                {
                    return parent;
                }

                parent = parent.ParentNode;
            }

            return curNode.ParentNode;
        }
        
        /// <summary>
        /// Clears current context which determines how to accumulate nodes for resolve.
        /// </summary>
        private void ResetInlineContext()
        {
            mTopLevelInline = null;
            mBlockLevelParent = null;
        }

        /// <summary>
        /// Changes start position of the block level SDT to new position.
        /// </summary>
        private void MoveMarkupStartToNode(Node newStartNode, Node prevStartNode)
        {
            List<IMarkupNode> keys = new List<IMarkupNode>(mMarkupStartsTable.Keys.Count);

            // Retrieve all start positions for previous node and move it to a new position.
            foreach (IMarkupNode key in mMarkupStartsTable.Keys)
            {
                if (mMarkupStartsTable[key] == prevStartNode)
                    keys.Add(key);
            }

            foreach (IMarkupNode key in keys)
                mMarkupStartsTable[key] = newStartNode;
        }

        /// <summary>
        /// Checks that current root has pending nodes. Returns "null" and inserts
        /// node before last child of the ancestor when does not have.
        /// </summary>
        private Node CheckResolveRoot(int seqIndex)
        {
            Node resolveRoot = mInlineSequence[seqIndex];
            // Existence of the value for specified key was checked on the stack above.
            IList<Node> pendingNodes = mInlinePendingTable.GetValueOrNull(resolveRoot);
            CompositeNode inlineAncestor = mBlockLevelParents.GetValueOrNull(resolveRoot);

            bool emptyRoot = (pendingNodes == null);

            if (emptyRoot)
                inlineAncestor.InsertBefore(resolveRoot, inlineAncestor.LastChild);

            return emptyRoot ? null : resolveRoot;
        }

        /// <summary>
        /// Resolves table.
        /// </summary>
        private void ProcessNestedInlineTable(Table tbl, int seqIndex)
        {
            Debug.Assert(tbl != null);

            Node resolveRoot = CheckResolveRoot(seqIndex);

            // Node without pending child nodes.
            if (resolveRoot == null)
                return;

            // Currently table which was detected in inline SDT is in resolve.
            // Container for text of the first table paragraph have to be saved.
            CompositeNode textHolder = tbl.ParentNode;
            CompositeNode inlineAncestor = mBlockLevelParents.GetValueOrNull(resolveRoot);

            // MSW requires that cell has to contain a paragraph.
            // In general this code for resiliency purposes.
            tbl.FirstRow.FirstCell.EnsureMinimum();

            // Move block level structured document tag start position to the beginning of the table.
            Paragraph firstTblPara = tbl.FirstRow.FirstCell.FirstParagraph;
            MoveMarkupStartToNode(firstTblPara, inlineAncestor.LastChild);

            // Copy cell content of the first paragraph to inline SDT (mimic MSW).
            while (firstTblPara.FirstChild != null)
                textHolder.AppendChild(firstTblPara.FirstChild);

            FlushFirstParagraph(firstTblPara, seqIndex);

            // Insert table to block level.
            inlineAncestor.InsertBefore(tbl, inlineAncestor.LastChild);

            // Move current inline SDT to the table.
            firstTblPara.AppendChild(resolveRoot);

            IList<Node> pendingNodes;
            // Content of the paragraph was moved to inline SDT, however it can be used
            // in another element sequences (as first paragraph).
            for (int i = seqIndex + 1; i < mInlineSequence.Count; ++i)
            {
                pendingNodes = GetPendingInlineNodes(i);

                if ((pendingNodes.Count > 0) && (pendingNodes[0] == firstTblPara))
                {
                    Node rootBefore = mInlineSequence[i];

                    mInlineSequence[i] = resolveRoot;
                    mInlinePendingTable[resolveRoot] = pendingNodes;
                    CompositeNode node = mBlockLevelParents.GetValueOrNull(rootBefore);
                    mBlockLevelParents[resolveRoot] = node;

                    break;
                }
            }
        }

        /// <summary>
        /// Resolves paragraph.
        /// </summary>
        private void ProcessNestedInlinePara(Paragraph para, int seqIndex, int nodeIndex)
        {
            Debug.Assert(para != null);

            // Existence of the value for specified key was checked on the stack above.
            Node resolveRoot = CheckResolveRoot(seqIndex);

            // Node without pending nodes.
            if (resolveRoot == null)
                return;

            CompositeNode inlineAncestor = mBlockLevelParents.GetValueOrNull(resolveRoot);
            bool isParaMoved = nodeIndex == mInlinePendingTable[resolveRoot].Count - 1;
            Node lastBlockChild = DetectLastChild(para, seqIndex, nodeIndex);

            if ((lastBlockChild != null) && !isParaMoved)
            {
                if (!mParasToAppend.ContainsKey(lastBlockChild))
                    mParasToAppend.Add(lastBlockChild, new List<Node>());

                mParasToAppend[lastBlockChild].Add(para);
                mParasToAppendParents[para] = inlineAncestor;
            }

            // Copy content of the paragraph to inline SDT (mimic MSW).
            if ((nodeIndex != 0) || (para.ParentNode == resolveRoot))
            {
                while (para.FirstChild != null)
                    para.ParentNode.AppendChild(para.FirstChild);
            }

            // Move paragraph to block level when current node is the last node for resolve.
            if (isParaMoved)
            {
                // Move child of the first paragraph to extracted node.
                FlushFirstParagraph(para, seqIndex);
                // Move current inline SDT to the paragraph.
                para.Remove();
                para.AppendChild(resolveRoot);

                // Insert paragraph to block level.
                inlineAncestor.InsertBefore(para, inlineAncestor.LastChild);               
                MoveMarkupStartToNode(para, inlineAncestor.LastChild);
            }
        }

        /// <summary>
        /// Copies content of the first paragraph of the specified sequence to 
        /// destination container.
        /// </summary>
        private void FlushFirstParagraph(CompositeNode destContainer, int seqIndex)
        {
            Debug.Assert(destContainer != null);

            IList<Node> pendingNodes = GetPendingInlineNodes(seqIndex);

            if (pendingNodes.Count == 0)
                return;

            Paragraph src = pendingNodes[0] as Paragraph;
            if ((src == null) || (src.ParentNode == mInlineSequence[seqIndex]))
                return;

            // Copy content to destination container.
            while ((src.FirstChild != null) && CheckFirstParaContent(src.FirstChild, seqIndex))
                destContainer.AppendChild(src.FirstChild);
        }

        /// <summary>
        /// Detects that specified child has to copied as inline content. 
        /// </summary>
        private bool CheckFirstParaContent(Node child, int seqIndex)
        {
            Debug.Assert(child != null);

            if (!child.IsComposite)
                return true;

            if (child.IsComposite && (child.NodeType != NodeType.StructuredDocumentTag))
                return false;

            // It is can be inline SDT without block level nodes within child nodes.
            IList<Node> nodes = GetPendingInlineNodes(seqIndex);
            // Take the node for current root and check that child is not ancestor for it.
            // We have at least two nodes at this place in the collection (first paragraph and
            // one more table or paragraph).
            return !nodes[nodes.Count - 1].IsAncestorNode(child);
        }

        /// <summary>
        /// Retrieves pending nodes for specified sequence.
        /// </summary>
        private IList<Node> GetPendingInlineNodes(int seqIndex)
        {
            Node rootNode = mInlineSequence[seqIndex];

            if (!mInlinePendingTable.ContainsKey(rootNode))
            {
                return new List<Node>{ rootNode };
            }

            return mInlinePendingTable[rootNode];
        }

        /// <summary>
        /// Finds last pending node which placed inside specified container.
        /// </summary>
        private Node DetectLastChild(CompositeNode container, int seqIndex, int nodeIndex)
        {
            IList<Node> pendingNodes = null;
            Node lastBlockChild = null;

            for (int i = seqIndex; i < mInlineSequence.Count; ++i)
            {
                pendingNodes = GetPendingInlineNodes(i);

                for (int j = nodeIndex + 1; j < pendingNodes.Count; ++j)
                {
                    bool isAncestor = pendingNodes[j].IsAncestorNode(container);
                    lastBlockChild = isAncestor ? pendingNodes[j] : lastBlockChild;

                    if (!isAncestor)
                        break;
                }

                nodeIndex = -1;
            }

            return lastBlockChild;
        }

        /// <summary>
        /// Stores top level inline SDT for current context.
        /// </summary>
        internal StructuredDocumentTag TopLevelInline
        {
            get { return mTopLevelInline; }
        }

        /// <summary>
        /// Stores top level inline SDT for current context.
        /// </summary>
        /// <remarks>
        /// It can be reset and changed after the root is completed and can be started the next one.
        /// <see cref="InlineMarkupResolver.ResetInlineContext"/>.
        /// </remarks>
        private StructuredDocumentTag mTopLevelInline;

        /// <summary>
        /// Stores current block level parent for inline SDT.
        /// </summary>
        /// <remarks>
        /// It used to reduce call count of <see cref="SetBlockLevelParent"/>.
        /// </remarks>
        private CompositeNode mBlockLevelParent;

        /// <summary>
        /// The collection of items which preserve sequence of nodes occurrence while reading.
        /// </summary>
        /// <remarks> 
        /// In current implementation this sequence can be changed, while
        /// <see cref="ProcessInline"/> is in process. However, in the
        /// beginning of this method the sequence is preserved.
        /// </remarks>
        private readonly List<Node> mInlineSequence = new List<Node>();

        /// <summary>
        /// Stores nodes where IMarkupNode of block level is started.
        /// </summary>
        /// <remarks> 
        /// It is reference to object which declared into <see cref="MarkupResolver"/>.
        /// </remarks>
        private readonly IDictionary<IMarkupNode, Node> mMarkupStartsTable;

        /// <summary>
        /// The map of the nodes and paragraphs which have to be moved after these nodes.
        /// </summary>
        private readonly Dictionary<Node, IList<Node>> mParasToAppend = new Dictionary<Node, IList<Node>>();

        /// <summary>
        /// The map of paragraphs which have to be moved to a new positions and parents.
        /// </summary>
        private readonly Dictionary<Node, CompositeNode> mParasToAppendParents = new Dictionary<Node, CompositeNode>();

        /// <summary>
        /// Stores unresolved child nodes of inline SDT's. Keys are inline SDT's and values are 
        /// arrays of nodes which pending to be resolved.
        /// </summary>
        /// <remarks>
        /// Keys of inline SDT's retrieves while iterating through <see cref="mInlineSequence"/>.
        /// </remarks>
        private readonly Dictionary<Node, IList<Node>> mInlinePendingTable = 
            new Dictionary<Node, IList<Node>>();

        /// <summary>
        /// Stores composite nodes from  <see cref="mInlineSequence"/> and parents map. 
        /// </summary>
        private readonly Dictionary<Node, CompositeNode> mBlockLevelParents = 
            new Dictionary<Node, CompositeNode>();
    }
}
