// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/03/2011 by Alexey Morozov

using System.Collections.Generic;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words.RW
{
    /// <summary>
    /// Resolves block-level markup issues for DOC, RTF and DOCX formats.
    /// </summary>
    internal class MarkupResolver
    {
        internal MarkupResolver(LoadOptions loadOptions, WarningSource warningSource)
        {
            mLoadOptions = loadOptions;
            mWarningSource = warningSource;
            mInlineResolver = new InlineMarkupResolver(mMarkupStartsTable);
        }

        /// <summary>
        /// Collects markup nodes to be started on flush operation.
        /// </summary>
        internal void PendingStart(StructuredDocumentTag sdt)
        {
            if (sdt.Level == MarkupLevel.Inline)
                mInlineResolver.AddToSequence(sdt);
            else
                mPendingStarts.Add(sdt);
        }

        /// <summary>
        /// Process start for pending markup nodes.
        /// </summary>
        /// <param name="node"></param>
        internal void FlushPendingStarts(Node node)
        {
            foreach(IMarkupNode markup in mPendingStarts)
                ProcessStart(markup, node);

            mPendingStarts.Clear();
        }

        /// <summary>
        /// Processes IMarkupNode end event.
        /// </summary>
        internal void ProcessEnd(StructuredDocumentTag sdt, Node node)
        {
            if (sdt.Level == MarkupLevel.Inline)
            {
                mInlineResolver.ProcessInline(sdt);
            }
            else
            {
                if (mPendingStarts.Count > 0)
                {
                    foreach (IMarkupNode markup in mPendingStarts)
                        mEmptySdtList.Add(markup);

                    // This means that mark does not contain any paragraph inside. Flush all pending starts.
                    FlushPendingStarts(node);
                }

                // Pop custom XML node from markup start stack.
                IMarkupNode customXml = mMarkupStartStack.Pop();

                if (customXml.Level_IMarkupNode != MarkupLevel.Inline)
                {
                    // Push not inline nodes to "ready" stack to resolve it later.
                    mMarkupReadyStack.Push(customXml);
                    mMarkupEndsTable[customXml] = node;
                }
            }
        }

        /// <summary>
        /// Resolves all stored markup nodes and places it into document.
        /// </summary>
        internal void Resolve()
        {
            StructuredDocumentTag prevRepeatingSection = null;

            while (mMarkupReadyStack.Count > 0)
            {
                IMarkupNode cxm = mMarkupReadyStack.Pop();

                Node firstNode = mMarkupStartsTable[cxm];
                Node lastNode = mMarkupEndsTable[cxm];

                // WORDSNET-23544 Special case for empty content block SDT. Word inserts it in very peculiar way.
                if ((firstNode != null) && ReferenceEquals(firstNode, lastNode) && mEmptySdtList.Contains(cxm))
                {
                    if (HandleEmptyContent((StructuredDocumentTag)cxm, firstNode))
                        continue;
                }

                // Build common parent range from markup start and end.
                List<Node> range = GetReadMarkupRange(cxm);

                if (range == null)
                {
                    // Cannot find a place to insert markup.
                    Warn(WarningStrings.UnableToGetParent);
                    continue;
                }

                // WORDSNET-25872 Adjust repeating section/item markup level.
                StructuredDocumentTag sdt = (StructuredDocumentTag)cxm;

                if (sdt.SdtType == SdtType.RepeatingSection)
                    prevRepeatingSection = sdt;

                if ((sdt.Level == MarkupLevel.Cell) && HasMappedDataRepeatingSdt(sdt, prevRepeatingSection) &&
                  ((sdt.SdtType == SdtType.RepeatingSection) || (sdt.SdtType == SdtType.RepeatingSectionItem)))
                {
                    sdt.SetLevel(MarkupLevel.Block);
                }

                CompositeNode rangeParent = range[0].ParentNode;

                if (((rangeParent.NodeType == NodeType.Table) && (cxm.Level_IMarkupNode == MarkupLevel.Block)) ||
                    ((rangeParent.NodeType == NodeType.Row) && (cxm.Level_IMarkupNode == MarkupLevel.Row)) ||
                    ((rangeParent.NodeType == NodeType.Cell) && (cxm.Level_IMarkupNode == MarkupLevel.Cell)))
                {
                    // Additional processing for tables - Custom XML will be applied for parent node.
                    range.Clear();
                    range.Add(rangeParent);
                    rangeParent = rangeParent.ParentNode;
                }
                else if ((rangeParent.NodeType == NodeType.Cell) && (cxm.Level_IMarkupNode == MarkupLevel.Row) )
                {
                    // Special case for row level markup with single cell in row.
                    // Custom XML will be applied for parent row.
                    Cell parentCell = (Cell)rangeParent;

                    range.Clear();
                    range.Add(parentCell.ParentRow);
                    rangeParent = parentCell.ParentTable;
                }
                else if ((rangeParent.NodeType == NodeType.Row) && (cxm.Level_IMarkupNode == MarkupLevel.Block))
                {
                    Row parentRow = (Row)rangeParent;

                    // Special case for block level markup with single row in table.
                    // Custom XML will be applied for parent table.
                    range.Clear();
                    range.Add(parentRow.ParentTable);
                    rangeParent = parentRow.ParentTable.ParentNode;
                }

                if (rangeParent.CanInsert((Node)cxm) && CanInsertRange((CompositeNode)cxm, range))
                {
                    // Custom XML is supported by Model at this level.
                    InsertAsParent((CompositeNode)cxm, range);
                }
                else
                {
                    // WORDSNET-22616 Update markup end if needed.
                    lastNode = GetActualRangeEndNode(lastNode);

                    // Try to insert structured document tag as ranged SDT.
                    if (StructuredDocumentTag.IsRangeValid(firstNode, lastNode))
                        InsertAsRangedSdt(cxm, firstNode, lastNode);
                    else
                        Warn(WarningStrings.UnacceptableMarkup);
                }
            }
        }

        /// <summary>
        /// Returns true if either RepeatingSectionItem itself or its parent RepeatingSection is mapped.
        /// </summary>
        private bool HasMappedDataRepeatingSdt(StructuredDocumentTag tag, StructuredDocumentTag repeatingSection)
        {
            if (!tag.XmlMapping.IsEmpty)
                return true;

            if ((tag.SdtType != SdtType.RepeatingSectionItem) || (repeatingSection == null))
                return false;

            // Here we check that the current RepeatingSectionItem is a potential
            // child sdt for the previous RepeatingSection.
            return (mMarkupStartsTable[(IMarkupNode)tag] == mMarkupStartsTable[repeatingSection]) &&
                   (mMarkupEndsTable[(IMarkupNode)tag] == mMarkupEndsTable[repeatingSection]) &&
                   !repeatingSection.XmlMapping.IsEmpty;
        }

        /// <summary>
        /// Checks that all nodes in range can be inserted into target composite node.
        /// </summary>
        private static bool CanInsertRange(CompositeNode parentNode, List<Node> range)
        {
            foreach(Node node in range)
                if (!parentNode.CanInsert(node))
                    return false;

            return true;
        }

        /// <summary>
        /// Handles SDT with empty content.
        /// </summary>
        /// <remarks>
        /// Actually I think this logic is explained by flushing operation occurred while Word read SDT content,
        /// seems AW does flushing in other than Word locations.
        /// Lets postpone to find proper logic and move SDT location manually.
        /// </remarks>
        /// <returns>
        /// Returns true if SDT fully resolved and no further actions needed.
        /// </returns>
        private bool HandleEmptyContent(StructuredDocumentTag sdt, Node firstNode)
        {
            bool isMapped = sdt.XmlMapping.IsMapped;

            if (mLoadOptions.MswVersion <= MsWordVersion.Word2013)
            {
                Paragraph para = (Paragraph)firstNode.NextPreOrderOfType(firstNode.Document, NodeType.Paragraph);

                // Logic is different depending on SDT is mapped. See Test23828_1 for example.
                if ((para != null) && isMapped)
                {
                    // Very strange behavior in Word 2013. Probably this is a bug fixed in latest MS Word versions.
                    // Move SDT as inline into following appropriate paragraph.
                    sdt.SetLevel(MarkupLevel.Inline);

                    para.InsertAfter(sdt, null);
                    return true;
                }

                // Unable to find following appropriate paragraph.
                if (para == null)
                {
                    // Word inserts extra paragraph and uses it as SDT content.
                    para = new Paragraph(firstNode.Document);
                    firstNode.InsertNext(para);
                }

                // Resolve SDT content with following appropriate (maybe newly created) paragraph.
                mMarkupStartsTable[sdt] = para;
                mMarkupEndsTable[sdt] = para;
            }
            else
            {
                // MS Word 2019 logic is much simpler.
                Node node = !isMapped
                    ? firstNode.NextNonAnnotationSibling as Paragraph
                    : null;

                // WORDSNET-26378 MS Word moves the SDT with empty sdtContent located at the row level inside the
                // first cell encountered. This cell's Paragraph becomes the SDT content. If there is no paragraph
                // inside the cell, then a new empty paragraph is created.
                if ((sdt.Level == MarkupLevel.Row) && (firstNode.NodeType == NodeType.Table))
                {
                    Table table = (Table)firstNode;
                    if ((table.FirstRow != null) && (table.FirstRow.FirstCell != null))
                    {
                        Cell cell = table.FirstRow.FirstCell;
                        if (cell.FirstParagraph == null)
                        {
                            node = new Paragraph(firstNode.Document);
                            cell.Paragraphs.Add(node);
                        }

                        node = cell.FirstParagraph;
                    }
                    else
                    {
                        if (table.NextNonAnnotationSibling is CompositeNode)
                        {
                            node = table.NextNonAnnotationSibling;
                        }
                        else
                        {
                            node = new Paragraph(firstNode.Document);
                            table.InsertNext(node);
                        }
                    }

                    // The SDT becomes Block level after these updates.
                    sdt.SetLevel(MarkupLevel.Block);
                }

                if (node == null)
                    node = AddResiliencyParagraph(firstNode);

                // Resolve SDT content with newly created paragraph.
                mMarkupStartsTable[sdt] = node;
                mMarkupEndsTable[sdt] = node;
            }

            return false;
        }

        /// <summary>
        /// Adds paragraph and places it in proper location depending on surrounding nodes.
        /// </summary>
        private static Paragraph AddResiliencyParagraph(Node firstNode)
        {
            Paragraph para = new Paragraph(firstNode.Document);

            // If next sibling is table, markup is moved into first table cell.
            // It seems Word does not flush before tables.
            Table table = firstNode.NextNonAnnotationSibling as Table;
            if ((table != null) && (table.FirstRow != null) && (table.FirstRow.FirstCell != null))
            {
                table.FirstRow.FirstCell.InsertAfter(para, null);
            }
            else if (firstNode.NodeType == NodeType.Cell)
            {
                ((Cell)firstNode).Paragraphs.Add(para);
            }
            else if (firstNode is Story)
            {
                // WORDSNET-27397 Insert empty paragraph as first child of header/footer.
                //
                // AM. This case reveals interesting problem. What is we have few empty SDT here?
                // The only information we have is that header/footer is a parent of SDTs, can we place such SDTs
                // properly?
                ((Story)firstNode).InsertAfter(para, null);
            }
            else
                firstNode.InsertNext(para);

            return para;
        }

        /// <summary>
        /// Processes IMarkupNode start event.
        /// </summary>
        private void ProcessStart(IMarkupNode markup, Node node)
        {
            mMarkupStartStack.Push(markup);

            if (markup.Level_IMarkupNode != MarkupLevel.Inline)
                mMarkupStartsTable[markup] = node;
        }

        private static void InsertAsRangedSdt(IMarkupNode cxm, Node firstNode, Node lastNode)
        {
            DocumentBase doc = firstNode.Document;

            ((StructuredDocumentTag)cxm).UpdateId();
            int sdtId = ((StructuredDocumentTag)cxm).Id;

            StructuredDocumentTagRangeStart rangeStart =
                new StructuredDocumentTagRangeStart(doc, (StructuredDocumentTag)cxm);

            StructuredDocumentTagRangeEnd rangeEnd = new StructuredDocumentTagRangeEnd(doc, sdtId);

            firstNode.InsertPrevious(rangeStart);
            lastNode.InsertNext(rangeEnd);
        }

        /// <summary>
        /// Gets an appropriate node for the end of a structured document tag range. If a position after the specified
        /// node is not correct for <see cref="StructuredDocumentTagRangeEnd"/>, finds and returns its ancestor, which
        /// can be the last node in the range.
        /// </summary>
        private static Node GetActualRangeEndNode(Node node)
        {
            Node currentNode = node;
            while (currentNode.ParentNode != null)
            {
                if (currentNode.ParentNode.NodeType == NodeType.Body)
                    return currentNode;

                currentNode = currentNode.ParentNode;
            }

            return node;
        }

        /// <summary>
        /// Returns list of nodes (one-level) that are located inside the specified mark-up node in the reading document
        /// file. Final contents of the mark-up node may be different than the read node list (<see cref="Resolve"/>).
        /// </summary>
        internal List<Node> GetReadMarkupRange(IMarkupNode markupNode)
        {
            Node node1 = mMarkupStartsTable.GetValueOrNull(markupNode);
            Node node2 = mMarkupEndsTable.GetValueOrNull(markupNode);

            return CommonParentRange(node1, node2);
        }

        /// <summary>
        /// Content resolver for inline SDT.
        /// </summary>
        internal InlineMarkupResolver InlineResolver
        {
            get { return mInlineResolver; }
        }

        /// <summary>
        /// Gets the last ended markup.
        /// </summary>
        internal IMarkupNode LastMarkup
        {
            get { return mMarkupReadyStack.Top(); }
        }

        /// <summary>
        /// Gets end node of the last ended markup.
        /// </summary>
        internal Node EndNodeOfLastMarkup
        {
            get
            {
                if (mMarkupReadyStack.Count == 0)
                    return null;

                IMarkupNode lastMarkup = mMarkupReadyStack.Peek();
                return mMarkupEndsTable[lastMarkup];
            }
        }

        /// <summary>
        /// Gets a flag indicating that the first child of a new markup node is being read.
        /// </summary>
        internal bool IsInStartingMarkup
        {
            get { return mPendingStarts.Count > 0; }
        }

        /// <summary>
        /// Returns common parent node range.
        /// </summary>
        /// <remarks>
        /// AM. In most cases nodes with custom XML markup have the same depth in our model.
        /// But I could made test file where custom XML markup nodes have different depth, see TestCustomXmlDiffLevel.doc.
        /// In this file markup starts in paragraph before table and ends in last cell of table.
        /// This is the reason I use slightly more complex algorithm to find common parent range.
        /// </remarks>
        /// <returns>List of nodes or null if range couldn't be found.</returns>
        private static List<Node> CommonParentRange(Node node1, Node node2)
        {
            int nodeDepth1 = NodeUtil.GetDepth(node1);
            int nodeDepth2 = NodeUtil.GetDepth(node2);

            int nodeDepth = System.Math.Min(nodeDepth1, nodeDepth2);

            // Sync node levels
            node1 = NodeUtil.GetNthParent(node1, nodeDepth1 - nodeDepth);
            node2 = NodeUtil.GetNthParent(node2, nodeDepth2 - nodeDepth);

            // Node levels are the same so we can climb while common parent for nodes not found.
            while((node1 != null) && (node2 != null) && (node1.ParentNode != node2.ParentNode))
            {
                node1 = node1.ParentNode;
                node2 = node2.ParentNode;
            }

            if ((node1 != null) && (node2 != null))
            {
                // Found not null nodes with common parent, build range
                List<Node> range = new List<Node>();

                // andrnosk: WORDSNET-5422 Check if node is not null.
                while (node1 != null && node1 != node2)
                {
                    range.Add(node1);
                    node1 = node1.NextSibling;
                }

                range.Add(node2);
                return range;
            }
            else
                return null;
        }

        /// <summary>
        /// Insert a custom XML node as parent of nodes range which already is in the Model tree.
        /// This method removes all nodes within given range from their parent (all nodes in range have the same parent node),
        /// insert IMarkupNode node and load all range as child nodes of custom XML.
        /// </summary>
        private static void InsertAsParent(CompositeNode markup, List<Node> range)
        {
            Node firstInRange = range[0];

            // Insert custom XML node before first node in range.
            firstInRange.InsertPrevious(markup);

            // Move all nodes from range to custom XML.
            foreach (Node n in range)
                markup.AppendChild(n);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(string description)
        {
            if (mLoadOptions.WarningCallback != null)
                mLoadOptions.WarningCallback.Warning(new WarningInfo(WarningType.DataLoss, mWarningSource, description));
        }

        /// <summary>
        /// Content resolver for inline SDT's.
        /// </summary>
        private readonly InlineMarkupResolver mInlineResolver;

        /// <summary>
        /// All markup nodes are pushed into this stack on their starts. Main purpose of this stack to get markup start/end synced.
        /// </summary>
        private readonly Stack<IMarkupNode> mMarkupStartStack = new Stack<IMarkupNode>();

        /// <summary>
        /// All markup nodes except Inline are pushed into this stack on their ends and will be resolved later
        /// i.e it's ready to be applied markups.
        /// </summary>
        private readonly Stack<IMarkupNode> mMarkupReadyStack = new Stack<IMarkupNode>();

        /// <summary>
        /// Stores nodes where IMarkupNode of block level is started. Keys are IMarkupNode nodes and values are Node where
        /// markups were started.
        /// </summary>
        private readonly Dictionary<IMarkupNode, Node> mMarkupStartsTable = new Dictionary<IMarkupNode, Node>();
        private readonly Dictionary<IMarkupNode, Node> mMarkupEndsTable = new Dictionary<IMarkupNode, Node>();
        private readonly List<IMarkupNode> mPendingStarts = new List<IMarkupNode>();

        /// <summary>
        /// List of structured document tag with empty content.
        /// </summary>
        private readonly List<IMarkupNode> mEmptySdtList = new List<IMarkupNode>();

        private readonly WarningSource mWarningSource;
        private readonly LoadOptions mLoadOptions;
    }
}
