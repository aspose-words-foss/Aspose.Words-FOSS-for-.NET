// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2016 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// A collection of <see cref="RevisionGroup"/> objects that represent revision groups in the document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/track-changes-in-a-document/">Track Changes in a Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>You do not create instances of this class directly. Use the <see cref="RevisionCollection.Groups"/>
    /// property to get revision groups present in a document.</para>
    /// </remarks>
    public sealed class RevisionGroupCollection : IEnumerable<RevisionGroup>
    {
        internal RevisionGroupCollection(DocumentBase doc)
        {
            // Collect style definition revisions.
            foreach (Style style in doc.Styles)
            {
                if (style.HasRevisions)
                {
                    RevisionGroup revisionGroup = new RevisionGroup(style);
                    mItems.Add(revisionGroup);
                }
            }

            CollectEditRevisions(doc, EditRevisionType.Deletion, true);
            CollectEditRevisions(doc, EditRevisionType.Insertion, true);
            CollectMoveRevisions(doc);
            CollectFormatRevisions(doc, NodeType.Run);
            CollectFormatRevisions(doc, NodeType.Paragraph);
            CollectFormatRevisions(doc, NodeType.Section);
            CollectTableRevisions(doc);

            CollectExtraEditRevisions(doc);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<RevisionGroup> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns a revision group at the specified index.
        /// </summary>
        public RevisionGroup this[int index]
        {
            get { return mItems[index]; }
        }

        /// <summary>
        /// Returns the number of revision groups in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Returns a revision group by the specified revision.
        /// </summary>
        internal RevisionGroup this[RevisionBase revisionBase]
        {
            get
            {
                return mRevisionBaseToGroupMap.ContainsKey(revisionBase)
                    ? mRevisionBaseToGroupMap[revisionBase]
                    : null;
            }
        }

        /// <summary>
        /// Collects edit revision groups.
        /// </summary>
        private void CollectEditRevisions(Node doc, EditRevisionType type, bool ignoreGroupShapeContent)
        {
            Node node = DeepestNode(doc, ignoreGroupShapeContent);
            while (node != null && node != doc)
            {
                ITrackableNode trackable = node as ITrackableNode;
                if ((trackable != null))
                {
                    EditRevision revision = GetEditRevision(trackable, type);

                    node = (revision != null)
                        ? ExtractRevisionGroup(node, doc, revision, ignoreGroupShapeContent)
                        : NextNode(node, ignoreGroupShapeContent);
                }
                else
                {
                    node = NextNode(node, ignoreGroupShapeContent);
                }
            }
        }

        /// <summary>
        /// Collects move revision groups.
        /// </summary>
        private void CollectMoveRevisions(DocumentBase doc)
        {
            // In contrast with edit revision groups, a move group may contain an edit revision group inside, nodes of
            // which have no move revisions. Also, row and cell nodes in a move range may have no revisions. But a move
            // range has a start and end nodes: moveFromRangeStart/moveToRangeStart and moveFromRangeEnd/moveToRangeEnd.
            // Let's use them to get move group boundary.

            // Order of move groups is important for DocumentSpanConverter.

            Node node = DeepestNode(doc, false);
            while (node != null)
            {
                if (node.NodeType == NodeType.MoveFromRangeStart || node.NodeType == NodeType.MoveToRangeStart)
                    node = ExtractMoveRevisionGroup((MoveRangeStart)node);
                else
                    node = NextNode(node);
            }
        }

        /// <summary>
        /// Collects extra edit revisions that MS Word collects in group shapes in textboxes.
        /// </summary>
        /// <remarks>
        /// AM. Actually these revision groups looks like MS Word bug, but we have ES customer wanting exact match.
        /// </remarks>
        private void CollectExtraEditRevisions(DocumentBase doc)
        {
            // Collect extra revisions from group shapes.
            List<GroupShape> allGroupShapes = doc.GetChildNodes(NodeType.GroupShape, true).ToList<GroupShape>();
            List<GroupShape> topGroupShapes = new List<GroupShape>();

            // Filter nested group shapes.
            foreach(GroupShape groupShape in allGroupShapes)
                if(GetShapeContainer(groupShape) == groupShape)
                    topGroupShapes.Add(groupShape);

            foreach (GroupShape groupShape in topGroupShapes)
            {
                CollectEditRevisions(groupShape, EditRevisionType.Insertion, false);
                CollectEditRevisions(groupShape, EditRevisionType.Deletion, false);
            }

            // Collect extra revisions from textboxes.
            List<Shape> allShapes = doc.GetChildNodes(NodeType.Shape, true).ToList<Shape>();
            List<Shape> topTextboxes = new List<Shape>();

            // Filter out nested textboxes.
            foreach(Shape shape in allShapes)
                if(shape.HasTextbox && GetShapeContainer(shape) == shape)
                    topTextboxes.Add(shape);

            foreach (Shape textbox in topTextboxes)
            {
                if(textbox.RunPr.HasInsertRevision)
                    CollectEditRevisions(textbox, EditRevisionType.Insertion, false);
                if(textbox.RunPr.HasDeleteRevision)
                    CollectEditRevisions(textbox, EditRevisionType.Deletion, false);
            }
        }

        /// <summary>
        /// Collects format revision groups.
        /// </summary>
        private void CollectFormatRevisions(DocumentBase doc, NodeType nodeType)
        {
            Node node = DeepestNode(doc, false);

            while (node != null)
            {
                WordAttrCollection pr = GetPr(node, nodeType);

                node = (pr != null) && pr.HasFormatRevision
                    ? ExtractRevisionGroup(node, pr.FormatRevision, nodeType)
                    : NextNode(node);
            }
        }

        /// <summary>
        /// Collects format revision for tables.
        /// </summary>
        private void CollectTableRevisions(DocumentBase doc)
        {
            foreach (Table table in doc.GetChildNodes(NodeType.Table, true))
            {
                List<RevisionBase> revList = GetFormatRevisions(table);
                if (revList.Count > 0)
                {
                    List<Node> nodeList = new List<Node>();
                    nodeList.Add(table);

                    // Create group with first occurred revision.
                    RevisionGroup revisionGroup = new RevisionGroup(nodeList, (FormatRevision)revList[0]);
                    mItems.Add(revisionGroup);

                    MapRevisions(revList, revisionGroup);
                }
            }
        }

        /// <summary>
        /// Returns first revision from table rows/cells or null if not found.
        /// </summary>
        private static List<RevisionBase> GetFormatRevisions(Table table)
        {
            List<RevisionBase> revList = new List<RevisionBase>();

            foreach (Row row in table.Rows)
            {
                // WORDSNET-23305 Ignore empty format revisions.
                if ((row.TablePr.FormatRevision != null) && (row.TablePr.FormatRevision.RevPr.Count > 0))
                    revList.Add(row.TablePr.FormatRevision);

                foreach(Cell cell in row.Cells)
                    if (cell.CellPr.FormatRevision != null)
                        revList.Add(cell.CellPr.FormatRevision);
            }

            return revList;
        }

        /// <summary>
        /// Gets properties of specified type from given node.
        /// </summary>
        private static WordAttrCollection GetPr(Node node, NodeType desiredNodeType)
        {
            switch (desiredNodeType)
            {
                case NodeType.Paragraph:
                    return (node.NodeType == NodeType.Paragraph) ? ((Paragraph)node).ParaPr : null;

                case NodeType.Section:
                    return (node.NodeType == NodeType.Section) ? ((Section)node).SectPr : null;

                case NodeType.Run:
                {
                    IInline inline = node as IInline;

                    if (inline != null)
                        return inline.RunPr_IInline;

                    if (node.NodeType == NodeType.Paragraph)
                        return ((Paragraph)node).ParagraphBreakRunPr;

                    return null;
                }

                default:
                    // Unexpected desired node type.
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Extracts one edit revision group started from given node. Returns node next to group.
        /// </summary>
        private Node ExtractRevisionGroup(Node node, Node root, EditRevision revision, bool ignoreGroupShapeContent)
        {
            List<Node> nodeList = new List<Node>();
            bool isStartOfRow = false;
            bool takeEntireRow = false;

            CompositeNode lastStory = null;

            List<RevisionBase> revList = new List<RevisionBase>();

            while (node != null && node != root)
            {
                ITrackableNode trackable = node as ITrackableNode;
                if (trackable != null)
                {
                    EditRevision nodeRevision = GetEditRevision(trackable, revision.Type);
                    revList.Add(nodeRevision);

                    bool isEqualRevision = (nodeRevision != null) && AreEqual(nodeRevision, revision);
                    if (!(isEqualRevision || takeEntireRow))
                        break;

                    if (node.NodeType == NodeType.Row)
                    {
                        isStartOfRow = true; // next row is being started
                        takeEntireRow = false;
                    }
                    else if (isStartOfRow)
                    {
                        isStartOfRow = false;
                        // If the first run of a row next to a deleted/inserted row is deleted/inserted,
                        // the entire row is included into the same revision group.
                        takeEntireRow = node is IInline;
                    }

                    nodeList.Add(node);
#if CPLUSPLUS
                    lastStory = node.GetAncestorOf<Story>();
#else
                    lastStory = node.GetAncestor(typeof(Story));
#endif
                }

                node = NextNode(node, ignoreGroupShapeContent);
#if CPLUSPLUS
                CompositeNode currStory = node.GetAncestorOf<Story>();
#else
                CompositeNode currStory = node.GetAncestor(typeof(Story));
#endif
                // WORDSNET-28956 Break group when story type is changed.
                if ((currStory != null) && (lastStory != null) && (currStory.NodeType != lastStory.NodeType))
                    break;
            }

            RevisionGroup revisionGroup = new RevisionGroup(nodeList, revision);
            mItems.Add(revisionGroup);

            MapRevisions(revList, revisionGroup);

            return node;
        }


        /// <summary>
        /// Returns shape itself or topmost shape container in case shape is a part of group shape.
        /// </summary>
        private static ShapeBase GetShapeContainer(ShapeBase shape)
        {
            while (true)
            {
                ShapeBase parentShape = shape.ParentNode as ShapeBase;

                if (parentShape == null)
                    return shape;

                shape = parentShape;
            }
        }

        /// <summary>
        /// Extracts one format revision group started from given node. Returns node next to group.
        /// </summary>
        private Node ExtractRevisionGroup(Node node, FormatRevision revision, NodeType desiredNodeType)
        {
            List<Node> nodeList = new List<Node>();
            List<RevisionBase> revList = new List<RevisionBase>();

            while (node != null)
            {
                WordAttrCollection pr = GetPr(node, desiredNodeType);

                if (pr != null)
                {
                    if (pr.HasFormatRevision && AreEqual(revision, pr.FormatRevision))
                    {
                        nodeList.Add(node);
                        revList.Add(pr.FormatRevision);
                    }
                    else
                        break;
                }

                Node prevNode = node;
                node = NextNode(node);

                // Looks like the Word splits revision group when a content related to different rows.
                if (IsRowChanged(prevNode, node))
                    break;
            }

            // Process list revision in special way.
            if ((desiredNodeType == NodeType.Paragraph) && revision.RevPr.Contains(ParaAttr.ListId))
                revision = new FormatRevision(CalculateListRevision((Paragraph)nodeList[0]), revision.Author, revision.DateTime);

            RevisionGroup revisionGroup = new RevisionGroup(nodeList, revision);
            mItems.Add(revisionGroup);

            MapRevisions(revList, revisionGroup);

            return (node);
        }

        /// <summary>
        /// Returns true when node and next node related to different rows.
        /// </summary>
        private static bool IsRowChanged(Node node, Node nextNode)
        {
            if ((node == null) || (nextNode == null))
                return false;

            Node nodeParentRow = (node.NodeType == NodeType.Row) ? node : node.GetAncestor(NodeType.Row);
            if (nodeParentRow == null) // I.e. there is not a row among parents and the node is not a row.
                return false;

            // Next node is a row i.e. specified nodes can not be related with as one row.
            if (nextNode.NodeType == NodeType.Row)
                return false;

            Node nextNodeParentRow = nextNode.GetAncestor(NodeType.Row);
            // Parent row for the next node was not found or parent rows are different.
            if ((nextNodeParentRow == null) || ReferenceEquals(nodeParentRow, nextNodeParentRow))
                return false;

            return true;
        }

        /// <summary>
        /// Extracts one move revision group started from the specified start node of a move range.
        /// Returns a node next to the group.
        /// </summary>
        private Node ExtractMoveRevisionGroup(MoveRangeStart moveStartNode)
        {
            List<Node> nodeList = new List<Node>();
            bool isMoveFrom = moveStartNode.NodeType == NodeType.MoveFromRangeStart;
            NodeType endNodeType = isMoveFrom ? NodeType.MoveFromRangeEnd : NodeType.MoveToRangeEnd;
            Node node = NextNode(moveStartNode);

            while (node != null && ((node.NodeType != endNodeType) || (((MoveRangeEnd)node).Id != moveStartNode.Id)))
            {
                bool hasRevision = false;
                if (node is IMoveTrackableNode)
                {
                    IMoveTrackableNode moveTrackableNode = node as IMoveTrackableNode;
                    hasRevision = (moveTrackableNode.MoveFromRevision != null) || (moveTrackableNode.MoveToRevision != null) || (node is Paragraph);
                }

                if (!hasRevision && (node is ITrackableNode))
                {
                    ITrackableNode trackableNode = node as ITrackableNode;
                    hasRevision = (trackableNode.DeleteRevision != null) || (trackableNode.InsertRevision != null);
                }

                if (hasRevision)
                    nodeList.Add(node);

                node = NextNode(node);
            }

            // If the extraction is successful: range end node is found, create and add a revision group.
            if ((node != null) && (nodeList.Count > 0))
            {
                MoveRevision revision = new MoveRevision(isMoveFrom ? MoveRevisionType.MoveFrom : MoveRevisionType.MoveTo,
                    moveStartNode.Author, moveStartNode.Date);
                mItems.Add(new RevisionGroup(nodeList, revision, moveStartNode.Name));
                return NextNode(node);
            }
            else
            {
                return NextNode(moveStartNode);
            }
        }

        private static EditRevision GetEditRevision(ITrackableNode trackable, EditRevisionType type)
        {
            EditRevision revision = (type == EditRevisionType.Deletion)
                                        ? trackable.DeleteRevision
                                        : trackable.InsertRevision;

            // Special case for cells. It seems that edit revisions might be inherited from parent row.
            if ((revision == null) && (trackable is Cell))
                revision = GetEditRevision(((Cell)trackable).ParentRow, type);

            return revision;
        }

        /// <summary>
        /// Calculates revision for paragraph with list revision.
        /// </summary>
        /// <remarks>
        /// AM. When paragraph has list revision Word saves some formatting inherited from list definition.
        /// To get correct formatting changes we need to strip this inherited formatting first.
        /// </remarks>
        private static ParaPr CalculateListRevision(Paragraph p)
        {
            const ParaPrExpandFlags originalFlags = ParaPrExpandFlags.Normal;
            const ParaPrExpandFlags finalFlags = ParaPrExpandFlags.Revised | ParaPrExpandFlags.RemoveClearTabStops;

            ParaPr original = p.GetExpandedParaPr(originalFlags);
            ParaPr originalInherited = p.GetExpandedParaPr(originalFlags | ParaPrExpandFlags.NoDirectFormatting);
            original.RemoveEquals(originalInherited);

            ParaPr finalRevision = p.GetExpandedParaPr(finalFlags);
            ParaPr finalInherited = p.GetExpandedParaPr(finalFlags | ParaPrExpandFlags.NoDirectFormatting);
            finalRevision.RemoveEquals(finalInherited);

            finalRevision.RemoveEquals(original);

            if (finalRevision.IsExplicitlyNotListItem)
            {
                // When paragraph is set to explicitly not list we expand zero left indent and zero first line indent.
                // Remove this to get bare revision changes.
                if (finalRevision.LeftIndent == 0)
                    finalRevision.Remove(ParaAttr.LeftIndent);

                if (finalRevision.FirstLineIndent == 0)
                    finalRevision.Remove(ParaAttr.FirstLineIndent);
            }

            return finalRevision;
        }

        /// <summary>
        /// Compares two format revisions.
        /// </summary>
        private static bool AreEqual(FormatRevision formatRevision1, FormatRevision formatRevision2)
        {
            if(!StringUtil.EqualsIgnoreCase(formatRevision1.Author, formatRevision2.Author))
                return false;

            WordAttrCollection revPr1 = ThemeCollapseHack(formatRevision1.RevPr);
            WordAttrCollection revPr2 = ThemeCollapseHack(formatRevision2.RevPr);

            if (!revPr1.Equals(revPr2, gIgnoreKeys))
                return false;

            // There should equality operator for effect attribute values.
            // Postpone for a while and compare presence of these attributes only.
            foreach (int effectKey in gEffectKeys)
            {
                bool contains1 = revPr1.Contains(effectKey);
                bool contains2 = revPr2.Contains(effectKey);
                if (contains1 != contains2)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Fixes theme color collapse issue
        /// </summary>
        /// <remarks>
        /// AM. We store simple color and theme color as separate attribute and this may cause the issue
        /// after collapse procedure during PositiveDifference calculation.
        ///
        /// This method removes default theme color value, which is actually empty string, to properly collect revision group.
        /// </remarks>
        private static WordAttrCollection ThemeCollapseHack(WordAttrCollection revPr)
        {
            if (revPr.ContainsKey(FontAttr.Color) && revPr.ContainsKey(FontAttr.ThemeColor) &&
                (string)revPr[FontAttr.ThemeColor] == "")
            {
                revPr = revPr.Clone();
                revPr.Remove(FontAttr.ThemeColor);
            }

            return revPr;
        }

        /// <summary>
        /// Compares two edit revisions.
        /// </summary>
        private static bool AreEqual(EditRevision editRevision1, EditRevision editRevision2)
        {
            return
                (editRevision1.Type == editRevision2.Type) &&
                StringUtil.EqualsIgnoreCase(editRevision1.Author, editRevision2.Author);
        }

        private static Node NextNode(Node node)
        {
            return NextNode(node, false);
        }

        /// <summary>
        /// Returns next node in textual order.
        /// </summary>
        private static Node NextNode(Node node, bool ignoreGroupShapeContent)
        {
            Node nextSibling = node.NextSibling;

            // Look for the first not ignorable next sibling.
            while (nextSibling != null && IsIgnorableNode(nextSibling))
                nextSibling = nextSibling.NextSibling;

            // If no sibling go to parent node.
            if(nextSibling == null)
                return node.ParentNode;


            return nextSibling.IsComposite
                ? DeepestNode(nextSibling, ignoreGroupShapeContent)
                : nextSibling;
        }

        /// <summary>
        /// Returns true if node should be ignored while revision group is collected.
        /// </summary>
        private static bool IsIgnorableNode(Node node)
        {
            // Skip annotations.
            switch (node.NodeType)
        {
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:

                // We cannot have revisions in comments so skip it entirely to do not break revision chain.
                case NodeType.Comment:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns first deepest child node.
        /// </summary>
        private static Node DeepestNode(Node node, bool ignoreGroupShapeContent)
        {
            // AM. MS Word has strange logic for group shapes revision groups.
            // It seems it is collected separately but for top level group shapes only.
            if ((node.NodeType == NodeType.GroupShape) && ignoreGroupShapeContent)
                return node;

            if (node.IsComposite && ((CompositeNode)node).HasChildNodes)
                return DeepestNode(((CompositeNode)node).FirstChild, ignoreGroupShapeContent);

            return node;
        }

        private void MapRevisions(List<RevisionBase> revisions, RevisionGroup revisionGroup)
        {
            foreach (RevisionBase revision in revisions)
                if(revision != null)
                    mRevisionBaseToGroupMap[revision] = revisionGroup;
        }

        private static readonly int[] gEffectKeys =
        {
            FontAttr.EffectGlow         /* 810 */,
            FontAttr.EffectShadow       /* 815 */,
            FontAttr.EffectReflection   /* 820 */,
            FontAttr.EffectOutline      /* 825 */,
            FontAttr.EffectFill         /* 830 */,
            FontAttr.EffectScene3D      /* 835 */,
            FontAttr.EffectProps3D      /* 840 */,
        };

        private static readonly int[] gIgnoreKeys =
        {
            FontAttr.RsidRPr    /* 30 */,
            FontAttr.RsidR      /* 40 */,

            // WORDSNET-28093 Seems MS Word ignores RTl related attributes, at least for paragraph break mark.
            FontAttr.BoldBi,    /* 250 */
            FontAttr.ItalicBi,  /* 260 */
            FontAttr.NameBi,    /* 270 */
            FontAttr.SizeBi,    /* 350 */

            FontAttr.EffectGlow         /* 810 */,
            FontAttr.EffectShadow       /* 815 */,
            FontAttr.EffectReflection   /* 820 */,
            FontAttr.EffectOutline      /* 825 */,
            FontAttr.EffectFill         /* 830 */,
            FontAttr.EffectScene3D      /* 835 */,
            FontAttr.EffectProps3D      /* 840 */,

            ParaAttr.RsidP              /* 1580 */
        };

        private readonly Dictionary<RevisionBase, RevisionGroup> mRevisionBaseToGroupMap = new Dictionary<RevisionBase, RevisionGroup>();

        private readonly List<RevisionGroup> mItems = new List<RevisionGroup>();
    }
}
