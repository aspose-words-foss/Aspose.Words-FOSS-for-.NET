// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2012 by Denis Darkin

using System.Collections.Generic;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Tables;

namespace Aspose.Words.Revisions
{
    /// <summary>
    /// Keeps together all methods related to accept/reject revision procedures.
    /// Knows how to handle revisions of each node type.
    /// </summary>
    /// <remarks>
    /// RK: MS Word seems to block some "nasty" delete scenarios so
    /// it simplifies our task. Here is what I've noticed MS Word does:
    ///
    /// 1. Does not allow to delete a paragraph or section break before a table.
    /// 2. Does not allow to delete last paragraph break of a non main text story (such as header or footer).
    /// 3. Does not allow to delete cell end mark (last paragraph in a cell).
    /// 4. Does not mark complete cell delete as a revision.
    ///</remarks>
    internal static class RevisionUtil
    {
        /// <summary>
        /// Perform accept/reject revision procedures on a node. Delay deletion of nodes during mass accept/reject scenarios.
        /// </summary>
        internal static void HandleNodeRevision(Node node, RevisionHandlingContext context)
        {
            context.CurrentNode = node;

            // Nodes should be actually inserted/deleted here so suspend tracking during this operation.
            using (new SuspendTrackRevisionsDocument(node.Document))
            {
                if (node is IInline)
                    ProcessInlineRevision(node, context);
                else if (node is Paragraph)
                    ProcessParagraphRevision(node, context);
                else if (node is Section)
                    ProcessSectionRevision(node, context);
                else if (node is Row)
                    ProcessRowRevision(node, context);
                else if (node is Cell)
                    ProcessCellRevision(node, context);
                else if (node is StructuredDocumentTag)
                    ProcessSdtRevision(node, context);
                else
                    Debug.Fail(string.Format("Unknown node type {0}", node.NodeType));

                if (context.IsSingleRevision)
                    ProcessDelayedNodes(context, node.Document);
            }
        }

        /// <summary>
        /// Deletes all inline/para/row/cell/sdt nodes that were collected during the rejection/acceptance of revision(s).
        /// </summary>
        internal static void ProcessDelayedNodes(RevisionHandlingContext context, DocumentBase document)
        {
            IList<Node> deferredDeletionParagraphs = new List<Node>();

            DelayedDeleteSdts(context.DelayedSdts);
            DelayedDeleteInlines(context.DelayedInlines, context);
            DelayedDeleteParas(context.DelayedParagraphs, context, deferredDeletionParagraphs);
            DelayedDeleteRows(context.DelayedRows);
            DelayedDeleteCells(context.DelayedCells);
            DelayedDeleteParas(deferredDeletionParagraphs, context, null);

            ProcessSdtNesting(context);

            ProcessInlinedSdts(context.InlinedSdts);

            if (context.IsSingleRevision)
            {
                MoveRange range = context.CurrentMoveRange;
                if ((range != null) && range.IsEmpty)
                    range.Remove();
            }
            else
            {
                DeleteMoveRanges(document);
            }
        }

        /// <summary>
        /// Returns collection of formatting attributes as generic <see cref="WordAttrCollection"/>.
        /// </summary>
        /// <remarks>
        /// 1) This is limited implementation implemented for Revision API purposes. It does not deal with
        /// other nodes having properties e.g. ShapeBase et al.
        /// 2) Some nodes have more than one formatting collection, e.g. <see cref="Paragraph"/> contains both
        /// <see cref="ParaPr"/> (which is returned by this method) and
        /// <see cref="Paragraph.ParagraphBreakRunPr"/> (which is not returned).
        /// </remarks>
        internal static WordAttrCollection GetNodeFormatting(Node node)
        {
            return TryGetInlineAttrs(node) ??
                   TryGetParagraphAttrs(node) ??
                   TryGetSectionAttrs(node) ??
                   TryGetRowAttrs(node) ??
                   TryGetCellAttrs(node) ??
                   TryGetSdtAttrs(node);
        }

        /// <summary>
        /// Returns True if the specified node has revision.
        /// </summary>
        internal static bool HasRevision(Node node)
        {
            bool result = false;
            WordAttrCollection formatting = GetNodeFormatting(node);
            if (formatting != null)
                result = formatting.HasRevisions || formatting.HasEmptyFormatRevision;

            // Paragraph has extra formatting that is checked for revisions separately.
            if (!result && (node is Paragraph))
            {
                Paragraph par = (Paragraph)node;
                result = par.HasRevisions;
                result |= par.ParagraphBreakRunPr.HasEmptyFormatRevision;
            }

            if (!result && (node is FieldEnd))
            {
                FieldEnd fieldEnd = ((FieldEnd)node);
                result = fieldEnd.RunPr.HasNumberRevision;
            }

            if (!result && (node is StructuredDocumentTag))
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                result = sdt.ContentsRunPr.HasRevisions || sdt.EndCharacterRunPr.HasRevisions;
                result |= sdt.EndCharacterRunPr.HasEmptyFormatRevision;
            }

            return result;
        }

        private static WordAttrCollection TryGetInlineAttrs(Node node)
        {
            IInline inline = node as IInline;
            return inline != null
                ? inline.RunPr_IInline
                : null;
        }

        private static WordAttrCollection TryGetParagraphAttrs(Node node)
        {
            Paragraph para = node as Paragraph;
            return para != null
                ? para.ParaPr
                : null;
        }

        private static WordAttrCollection TryGetSectionAttrs(Node node)
        {
            Section section = node as Section;
            return section != null
                ? section.SectPr
                : null;
        }

        private static WordAttrCollection TryGetRowAttrs(Node node)
        {
            Row row = node as Row;
            return row != null
                ? row.TablePr
                : null;
        }

        private static WordAttrCollection TryGetCellAttrs(Node node)
        {
            Cell cell = node as Cell;
            return cell != null
                ? cell.CellPr
                : null;
        }

        private static WordAttrCollection TryGetSdtAttrs(Node node)
        {
            StructuredDocumentTag sdt = node as StructuredDocumentTag;
            return sdt != null
                ? sdt.ContentsRunPr
                : null;
        }

        /// <summary>
        /// Returns a flag indicating if the specified node or any of its children have revisions.
        /// </summary>
        internal static bool HasRevisions(Node node)
        {
            return NodeUtil.FindChildOrSelf(node, new RevisionNodeMatcher()) != null;
        }

        /// <summary>
        /// Processes SDT revision.
        /// </summary>
        private static void ProcessSdtRevision(Node node, RevisionHandlingContext context)
        {
            StructuredDocumentTag sdt = (StructuredDocumentTag)node;
            if (context.IsAcceptance)
            {
                sdt.EndCharacterRunPr.AcceptFormatRevision();
                AcceptRevision(sdt, sdt.ContentsRunPr, context);
            }
            else
            {
                sdt.EndCharacterRunPr.RejectFormatRevision();
                RejectRevision(sdt, sdt.ContentsRunPr, context);
            }
        }

        private static void ProcessCellRevision(Node node, RevisionHandlingContext context)
        {
            Cell cell = (Cell)node;
            if (context.IsAcceptance)
                AcceptRevision(cell, cell.CellPr, context);
            else
                RejectRevision(cell, cell.CellPr, context);
        }

        private static void ProcessSectionRevision(Node node, RevisionHandlingContext context)
        {
            SectPr sectPr = ((Section)node).SectPr;
            if (context.IsAcceptance)
                sectPr.AcceptFormatRevision();
            else
                sectPr.RejectFormatRevision();
        }

        private static void ProcessParagraphRevision(Node node, RevisionHandlingContext context)
        {
            Paragraph para = (Paragraph)node;
            if (context.IsAcceptance)
                AcceptParagraphRevision(para, context);
            else
                RejectParagraphRevision(para, context);
        }

        private static void ProcessInlineRevision(Node node, RevisionHandlingContext context)
        {
            if (node.ParentNode == null)
                return;

            if (context.IsAcceptance)
                AcceptInlineRevision(node, context);
            else
                RejectInlineRevision(node, context);
        }

        private static void ProcessRowRevision(Node node, RevisionHandlingContext context)
        {
            Row row = (Row)node;
            if (context.IsAcceptance)
                AcceptRevision(row, row.TablePr, context);
            else
                RejectRevision(row, row.TablePr, context);

            // In order to get typed access to row cells it is required to use row.Cells collection.
            foreach (Cell cell in row.Cells)
            {
                CellPr cellPr = cell.CellPr;
                if (IsFakeCellFormatRevision(cellPr))
                    cellPr.Remove(RevisionAttr.FormatRevision);
            }
        }

        /// <summary>
        /// Accepts revision defined in the specified run properties.
        /// </summary>
        private static void AcceptRevision(Node node, WordAttrCollection properties, RevisionHandlingContext context)
        {
            properties.AcceptFormatRevision();

            // To accept an inserted/movedTo revision simply remove the revision object.
            properties.Remove(RevisionAttr.InsertRevision);
            properties.Remove(RevisionAttr.MoveToRevision);

            if (NeedDeleteNodeOnAccepting(properties, context))
            {
                context.AddDelayedNode(node);
                if (properties.HasMoveFromRevision)
                    ProcessNextUnmovedNodes(context);
            }
        }

        /// <summary>
        /// Rejects revision defined in the specified run properties.
        /// </summary>
        private static void RejectRevision(Node node, WordAttrCollection properties, RevisionHandlingContext context)
        {
            properties.RejectFormatRevision();

            // To reject an deleted/movedFrom revision simply remove the revision object.
            properties.Remove(RevisionAttr.DeleteRevision);
            properties.Remove(RevisionAttr.MoveFromRevision);

            if (NeedDeleteNodeOnRejecting(properties, context))
            {
                context.AddDelayedNode(node);
                if (properties.HasMoveToRevision)
                    ProcessNextUnmovedNodes(context);
            }
        }

        /// <summary>
        /// When writing row format revisions MS Word adds fake format revisions of cells.
        /// They not visible in Word DOM and should be removed together with Row, otherwise row is messed up upon accepting/rejecting changes.
        /// </summary>
        private static bool IsFakeCellFormatRevision(CellPr cellPr)
        {
            return !cellPr.HasFormatRevision && (cellPr.FormatRevision != null);
        }

        private static void AcceptParagraphRevision(Paragraph para, RevisionHandlingContext context)
        {
            // To accept paragraph formatting, apply the revision attributes.
            para.ParaPr.AcceptFormatRevision();

            // To accept bullet and numbered revision simply remove the revision object.
            para.ParaPr.Remove(RevisionAttr.NumberRevision);

            RunPr pBreakRunPr = para.ParagraphBreakRunPr;

            // To accept run formatting on the paragraph, apply the revision attributes.
            pBreakRunPr.AcceptFormatRevision();

            // WORDSNET-22956 Do not process other than formatting revisions
            // when requested to accept only formatting single revision.
            if (!context.IsSingleRevision || (context.RevisionType != RevisionType.FormatChange))
            {
                // To accept an inserted/movedTo paragraph simply remove the revision object.
                pBreakRunPr.Remove(RevisionAttr.InsertRevision);
                pBreakRunPr.Remove(RevisionAttr.MoveToRevision);

                // To accept a deleted paragraph, cannot delete the paragraph now, do it later.
                if (NeedDeleteNodeOnAccepting(pBreakRunPr, context))
                    ProcessDeletingParagraph(para, context, pBreakRunPr.HasMoveFromRevision);
            }
        }

        private static void RejectParagraphRevision(Paragraph para, RevisionHandlingContext context)
        {
            // To reject paragraph formatting, remove the revision attributes.
            para.ParaPr.RejectFormatRevision();

            // To reject bullet and numbered revision simply remove the revision object.
            para.ParaPr.Remove(RevisionAttr.NumberRevision);

            RunPr pBreakRunPr = para.ParagraphBreakRunPr;

            // To reject run formatting on the paragraph, remove the revision attributes.
            pBreakRunPr.RejectFormatRevision();

            // WORDSNET-22956 Do not process other than formatting revisions
            // when requested to reject only formatting single revision.
            if (!context.IsSingleRevision || (context.RevisionType != RevisionType.FormatChange))
            {
                // To reject a deleted/moved paragraph simply remove the revision object.
                pBreakRunPr.Remove(RevisionAttr.DeleteRevision);
                pBreakRunPr.Remove(RevisionAttr.MoveFromRevision);

                // To reject an inserted paragraph, cannot delete the paragraph now, do it later.
                if (NeedDeleteNodeOnRejecting(pBreakRunPr, context))
                    ProcessDeletingParagraph(para, context, pBreakRunPr.HasMoveToRevision);
            }
        }

        /// <summary>
        /// Processes a revised paragraph, which is about to be deleted during handling revision.
        /// A move-revised paragraph may be preserved in some cases.
        /// </summary>
        private static void ProcessDeletingParagraph(Paragraph para, RevisionHandlingContext context, bool isMove)
        {
            MoveRange moveRange = context.CurrentMoveRange;
            if (isMove && moveRange.IsDefined)
            {
                MoveRangeRevisionHelper helper = new MoveRangeRevisionHelper(moveRange);
                bool delayDeletion = !helper.IsNonRevisedNodePreserved ||
                             !HasUnmovedTrackables(para, moveRange.IsMoveTo) ||
                             AreAllNextParagraphsDeleted(para, context);
                if (delayDeletion)
                {
                    context.AddDelayedNode(para);
                    ProcessNextUnmovedNodes(context);
                }
                else
                {
                    // Looks like Word removes the original paragraph and creates a new one with keeping
                    // some formatting at this case.
                    int revisionAttr = moveRange.IsMoveTo ? RevisionAttr.MoveToRevision : RevisionAttr.MoveFromRevision;
                    para.ParagraphBreakRunPr.Remove(revisionAttr);

                    // maybe it will be changed to removal of some particular attributes
                    para.ParagraphBreakRunPr.Clear();
                }
            }
            else
            {
                context.AddDelayedNode(para);
            }
        }

        /// <summary>
        /// Returns <c>true</c> if all the next paragraphs in the move range have move revision.
        /// </summary>
        private static bool AreAllNextParagraphsDeleted(Paragraph para, RevisionHandlingContext context)
        {
            MoveRange moveRange = context.CurrentMoveRange;

            Node node = para.NextNonMarkupCompositeLimited;
            while ((node != null) && node.IsAbove(moveRange.End))
            {
                if (HasNoMoveRevision(node, moveRange.IsMoveTo))
                    return false;

                node = node.NextNonMarkupCompositeLimited;
            }

            return true;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node is a trackable node and has a move revision.
        /// </summary>
        internal static bool HasMoveRevision(Node node, bool checkForMoveTo)
        {
            IMoveTrackableNode trackable = node as IMoveTrackableNode;
            if (trackable == null)
                return false;

            return checkForMoveTo
                ? trackable.MoveToRevision != null
                : trackable.MoveFromRevision != null;
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node is a trackable node and has no move revision.
        /// </summary>
        private static bool HasNoMoveRevision(Node node, bool checkForMoveTo)
        {
            IMoveTrackableNode trackable = node as IMoveTrackableNode;
            if (trackable == null)
                return false;

            return checkForMoveTo
                ? trackable.MoveToRevision == null
                : trackable.MoveFromRevision == null;
        }

        /// <summary>
        /// Checks whether the node contains trackable children without move revision.
        /// </summary>
        private static bool HasUnmovedTrackables(CompositeNode composite, bool isMoveTo)
        {
            foreach (Node node in composite)
            {
                if (HasNoMoveRevision(node, isMoveTo))
                    return true;

                if (node.IsComposite && HasUnmovedTrackables((CompositeNode)node, isMoveTo))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Accepts revisions for inline nodes.
        /// </summary>
        /// <param name="node">node with revisions to be accepted.</param>
        /// <param name="context">Revision acceptance/rejection context.</param>
        private static void AcceptInlineRevision(Node node, RevisionHandlingContext context)
        {
            RunPr runPr = ((IInline)node).RunPr_IInline;

            //To accept run formatting, apply the revision attributes.
            runPr.AcceptRunSpecificFormatRevision(Run.IsSymbolicCharacter(node.GetText()));

            runPr.Remove(RevisionAttr.NumberRevision);

            // WORDSNET-22956 Do not process other than formatting revisions
            // when requested to accept only formatting single revision.
            if (!context.IsSingleRevision || (context.RevisionType != RevisionType.FormatChange))
            {
                //To accept an inserted/movedTo run simply remove the revision object.
                runPr.Remove(RevisionAttr.InsertRevision);
                runPr.Remove(RevisionAttr.MoveToRevision);

                //To accept a deleted/movedFrom inline, delete it. If cannot delete the inline, do it later.
                if (NeedDeleteNodeOnAccepting(runPr, context))
                {
                    // WORDSNET-11175 The customer document contains mail-merge field, in which field start and
                    // field end nodes are marked as deleted, but field run is not. It should be deleted too.
                    // WORDSNET-23209 Mark for deletion whole Field when accepting delete revision on any of FieldChar.
                    FieldChar fieldChar = node as FieldChar;
                    if (fieldChar != null)
                        AddDelayedField(fieldChar, context);
                    else
                        context.AddDelayedNode(node);

                    if (runPr.HasMoveFromRevision)
                        ProcessNextUnmovedNodes(context);
                }
            }
        }

        /// <summary>
        /// Rejects revisions for inline nodes.
        /// </summary>
        /// <param name="node">node with revisions to be deleted.</param>
        /// <param name="context">Revision acceptance/rejection context.</param>
        private static void RejectInlineRevision(Node node, RevisionHandlingContext context)
        {
            RunPr runPr = ((IInline)node).RunPr_IInline;

            //To reject run formatting, apply the revision attributes.
            runPr.RejectFormatRevision();

            runPr.Remove(RevisionAttr.NumberRevision);

            // WORDSNET-22956 Do not process other than formatting revisions
            // when requested to reject only formatting single revision.
            if (!context.IsSingleRevision || (context.RevisionType != RevisionType.FormatChange))
            {
                //To reject a deleted/moved run simply remove the revision object.
                runPr.Remove(RevisionAttr.DeleteRevision);
                runPr.Remove(RevisionAttr.MoveFromRevision);

                //To reject an inserted/moved inline delete it. if cannot delete the inline, do it later.
                if (NeedDeleteNodeOnRejecting(runPr, context))
                {
                    // WORDSNET-23209 Mark for deletion whole Field when rejecting insert revision on any of FieldChar.
                    FieldChar fieldChar = node as FieldChar;
                    if (fieldChar != null)
                        AddDelayedField(fieldChar, context);
                    else
                        context.AddDelayedNode(node);

                    if (runPr.HasMoveToRevision)
                        ProcessNextUnmovedNodes(context);
                }
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the node containing the specified attribute collection should be deleted
        /// in the current operation of accepting revisions.
        /// </summary>
        private static bool NeedDeleteNodeOnAccepting(WordAttrCollection attrCollection, RevisionHandlingContext context)
        {
            if (context.IsSingleRevision)
            {
                // Delete only for the processing revision.
                if ((context.RevisionType == RevisionType.Deletion) && attrCollection.HasDeleteRevision)
                    return true;

                if ((context.RevisionType == RevisionType.Moving) && attrCollection.HasMoveFromRevision)
                    return true;

                return false;
            }

            return attrCollection.HasDeleteRevision || attrCollection.HasMoveFromRevision;
        }

        /// <summary>
        /// Returns a flag indicating whether the node containing the specified attribute collection should be deleted
        /// in the current operation of rejecting revisions.
        /// </summary>
        private static bool NeedDeleteNodeOnRejecting(WordAttrCollection attrCollection, RevisionHandlingContext context)
        {
            if (context.IsSingleRevision)
            {
                // Delete only for the processing revision.
                if ((context.RevisionType == RevisionType.Insertion) && attrCollection.HasInsertRevision)
                    return true;

                if ((context.RevisionType == RevisionType.Moving) && attrCollection.HasMoveToRevision)
                    return true;

                return false;
            }

            return attrCollection.HasInsertRevision || attrCollection.HasMoveToRevision;
        }

        /// <summary>
        /// Adds all nodes of the Field to which the specified FieldChar node belongs
        /// to a corresponding delayed deletion list.
        /// </summary>
        private static void AddDelayedField(FieldChar fieldChar, RevisionHandlingContext context)
        {
            // Here "FieldEnd" marked as deleted. If there is inline content which has not revision or marked as "inserted",
            // then MSW removes whole field with the content, although other content is not marked as deleted.

            // WORDSNET-16242 Remove all inline content include nested fields, when "FieldEnd" element
            // was added to the "Delete" revision.
            FieldBundle bundle = FieldBundle.GetFieldBundle(fieldChar);
            Field field = new Field(bundle);

            foreach (Node fieldNode in field.GetFieldRange())
            {
                IInline curInline = fieldNode as IInline;
                RunPr curRunPr = curInline != null
                    ? curInline.RunPr_IInline
                    : null;

                if (curRunPr == null)
                    continue;

                context.AddDelayedNode(fieldNode);
            }
        }

        private static void DelayedDeleteInlines(ICollection<Node> deletedInlines, RevisionHandlingContext context)
        {
            foreach (Node inlineNode in deletedInlines)
                DeleteInline(inlineNode, context);

            deletedInlines.Clear();
        }

        private static void DelayedDeleteParas(IList<Node> deletedParas, RevisionHandlingContext context,
            IList<Node> deferredDeletionParagraphs)
        {
            Paragraph pendingParagraph = null;
            foreach (Node node in deletedParas)
            {
                Paragraph para = (Paragraph) node;
                if (!DeleteRevisedParagraph(para, context, deferredDeletionParagraphs))
                {
                    Debug.Assert(pendingParagraph == null);
                    pendingParagraph = para;
                    continue;
                }

                if (pendingParagraph != null)
                {
                    if (DeleteRevisedParagraph(pendingParagraph, context, deferredDeletionParagraphs))
                        pendingParagraph = null;
                }
            }

            deletedParas.Clear();
        }

        /// <summary>
        /// Deletes rows that are contained in the specified list.
        /// </summary>
        private static void DelayedDeleteRows(IList<Node> deletedRows)
        {
            foreach (Node row in deletedRows)
                DeleteNode((Row)row);

            deletedRows.Clear();
        }

        /// <summary>
        /// Deletes rows that are contained in the specified list.
        /// </summary>
        private static void DelayedDeleteCells(IList<Node> deletedCells)
        {
            foreach (Node cell in deletedCells)
                DeleteNode(cell);

            deletedCells.Clear();
        }

        /// <summary>
        /// Deletes SDTs that are contained in the specified list.
        /// </summary>
        private static void DelayedDeleteSdts(IList<Node> deletedSdts)
        {
            foreach (Node sdt in deletedSdts)
                DeleteSdt((StructuredDocumentTag)sdt);

            deletedSdts.Clear();
        }

        /// <summary>
        /// Deletes node marked with deleted or moved revision.
        /// </summary>
        private static void DeleteNode(Node node)
        {
            if (node.ParentNode != null)
                node.Remove();
        }

        /// <summary>
        /// Deletes inline node marked with deleted or moved revision.
        /// </summary>
        private static void DeleteInline(Node inline, RevisionHandlingContext context)
        {
            CompositeNode parent = inline.ParentNode;

            // Remove deleted/moved node.
            inline.Remove();

            // WORDSNET-9525 It seems like if the content of smart tag has been deleted, the smart tag should be deleted too.
            if (parent.NodeType == NodeType.SmartTag || parent.NodeType == NodeType.Comment)
                RemoveEmptyParent(parent, parent.NodeType, context);
        }

        /// <summary>
        /// Deletes SDT node marked with deleted or moved revision.
        /// </summary>
        private static void DeleteSdt(StructuredDocumentTag sdt)
        {
            // IN. It seems MS Word does not update content when accepts deleted SDTs.
            // Also, in general case I think when user removes SDT in MS Word GUI, it should
            // remove all SDT's content as well, so there is no need to update it.
            sdt.RemoveSelfOnly(false);
        }

        private static void RemoveEmptyParent(CompositeNode compositeNode, NodeType nodeType,
            RevisionHandlingContext context)
        {
            if (context.HasPreservedTrackables(compositeNode, false))
                return;

            if (compositeNode.ParentNode == null)
                return;

            if (compositeNode.ParentNode.NodeType == nodeType)
                RemoveEmptyParent(compositeNode.ParentNode, nodeType, context);

            compositeNode.Remove();
        }

        /// <summary>
        /// Usually MS Word removes unrevised trackables in move-from range on acceptance of revisions, but may
        /// preserve them in some cases.
        /// It is implemented that unrevised nodes are processed only till the next revised node to remove nodes little
        /// by little on accepting single revisions and for compatibility with the old code.
        /// </summary>
        private static void ProcessNextUnmovedNodes(RevisionHandlingContext context)
        {
            MoveRange moveRange = context.CurrentMoveRange;
            if (!moveRange.IsDefined)
                return;

            MoveRangeRevisionHelper helper = new MoveRangeRevisionHelper(moveRange);
            if (helper.IsNonRevisedNodePreserved)
                return;

            Node node = GetNextNodeOfMoveRange(context.CurrentNode, helper);
            while (node != null)
            {
                if (node is IMoveTrackableNode)
                {
                    // Process nodes till the next move revision only.
                    if (HasMoveRevision(node, moveRange.IsMoveTo))
                        break;

                    if (IsNodeForRemoval(node, helper))
                    {
                        context.AddDelayedNode(node);
                    }
                    // Check preconditions whether the current node (SDT) may be nested in the first SDT of the move
                    // range and store it in the context. Final decision about the nesting will be made after removing
                    // child nodes of the SDTs. See the cases 7, 8 (nested) and 2 (not nested) of WORDSNET-15250
                    else if (IsToNestInFirstSdt(node, helper))
                    {
                        context.AddSdtNesting(
                            (StructuredDocumentTag)helper.TopStartNode,
                            (StructuredDocumentTag)helper.TopEndNode,
                            helper.CanNestInlineSdts);
                    }
                }

                node = GetNextNodeOfMoveRange(node, helper);

                if (node == context.CurrentNode)
                    break;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this unrevised node is about to be deleted during handling revisions.
        /// </summary>
        private static bool IsNodeForRemoval(Node node, MoveRangeRevisionHelper helper)
        {
            if (node.NodeType == NodeType.StructuredDocumentTag)
            {
                StructuredDocumentTag sdt = (StructuredDocumentTag)node;
                return
                    !helper.IsMiddleSdtPreserved &&
                    !(helper.IsFirstSdtPreserved && helper.IsFirstSdt(sdt)) &&
                    !(helper.IsLastSdtPreserved && helper.IsLastSdt(sdt));
            }

            if ((node.NodeType == NodeType.Paragraph) || (node is IInline))
            {
                // If move range end is at this paragraph, then the paragraph is after move range, preserve it.
                if ((node.NodeType == NodeType.Paragraph) && helper.EndNode.IsAncestorNode(node))
                    return false;

                // Looks like unrevised inlines are preserved by MS Word with the same rules as "middle" paragraphs.
                if (helper.IsMiddleParagraphPreserved)
                    return false;

                if (node.NodeType != NodeType.Paragraph)
                    return true;

                if (!helper.IsLastParagraphPreserved)
                    return true;

                if (!helper.IsLastParagraph((Paragraph)node))
                    return true;

                return false;
            }

            // Cells/rows/tables are processed on paragraph removal.
            return false;
        }

        /// <summary>
        /// Returns <c>true</c> if this unrevised node is last SDT of the move range, which, if it has no children,
        /// is intended to move into the first SDT of the range.
        /// </summary>
        private static bool IsToNestInFirstSdt(Node node, MoveRangeRevisionHelper helper)
        {
            if (node.NodeType != NodeType.StructuredDocumentTag)
                return false;

            if (!helper.IsLastSdtNestedInFirst)
                return false;

            if (!helper.IsLastSdt((StructuredDocumentTag)node))
                return false;

            if (helper.TopStartNode.NodeType != NodeType.StructuredDocumentTag)
                return false;

            return true;
        }

        /// <summary>
        /// Get next node of the specified node with returning deepest child at first.
        /// </summary>
        private static Node GetNextNodeOfMoveRange(Node node, MoveRangeRevisionHelper helper)
        {
            // WORDSNET-18520 Do not search inside empty MoveRange.
            if (helper.Range.IsEmpty)
                return null;

            if (!NodeUtil.IsAncestorOrSelf(helper.EndNode, node) && (node.NextSibling != null))
                return GetDeepestFirstChild(node.NextSibling);

            Node parentNode = node.ParentNode;
            return parentNode == helper.RangeParent
                ? GetDeepestFirstChild(helper.StartNode)
                : parentNode;
        }

        /// <summary>
        /// Gets deepest first descendant of the specified node.
        /// </summary>
        private static Node GetDeepestFirstChild(Node node)
        {
            while (node.IsComposite && ((CompositeNode)node).HasChildNodes)
                node = ((CompositeNode)node).FirstChild;
            return node;
        }

        /// <summary>
        /// Deletes paragraph marked with revision.
        /// </summary>
        private static bool DeleteRevisedParagraph(Paragraph para, RevisionHandlingContext context,
            IList<Node> deferredDeletionParagraphs)
        {
            if (para.IsEndOfCell)
            {
                DeleteLastParaFromCell(para, context);
                return true;
            }

            if (para.IsEndOfSection)
            {
                DeleteLastParaFromSection(para, context);
                return true;
            }

            return DeleteParagraphCore(para, context, deferredDeletionParagraphs);
        }

        /// <summary>
        /// Deleting last paragraph from a cell in a revision means the whole cell is deleted.
        /// I also delete the row and the table if they become empty.
        /// </summary>
        private static void DeleteLastParaFromCell(Paragraph para, RevisionHandlingContext context)
        {
            Cell cell = para.ParentCell;
            Row row = cell.ParentRow;
            Table table = row.ParentTable;

            cell.Remove();

            if (!row.IsRemoved)
            {
                // AllRevisions was passed only on acceptance of all revisions, keep the old behaviour for now.
                IList<Revision> revisions = context.IsAcceptance ? context.AllRevisions : null;
                if (IsRowEmpty(row, revisions))
                    row.Remove();
            }

            if ((table != null) && (table.FirstRow == null))
                table.Remove();
        }

        /// <summary>
        /// Checks that row is empty considering conflicts.
        /// </summary>
        /// <remarks>
        /// AM. WORDSNET-8664 Column is inserted and row is deleted.
        /// We have some kind of conflicts here i.e cell is inserted into deleted row.
        /// This method checks this case.
        /// </remarks>
        private static bool IsRowEmpty(Row row, IList<Revision> allRevisions)
        {
            if (row.FirstCell == null)
                return true;

            // andrnosk: WORDSNET-9656 If row contains deleted cell, consider this row is not empty.
            foreach (Cell cell in row.Cells)
            {
                if (!IsInsertedCell(cell, allRevisions) || IsDeletedCell(cell, allRevisions))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Indicates that given cell is insertion revision.
        /// </summary>
        private static bool IsInsertedCell(Cell cell, IList<Revision> allRevisions)
        {
            return CheckCellRevision(cell, allRevisions, RevisionType.Insertion);
        }

        /// <summary>
        /// Indicates that given cell is deletion revision.
        /// </summary>
        private static bool IsDeletedCell(Cell cell, IList<Revision> allRevisions)
        {
            return CheckCellRevision(cell, allRevisions, RevisionType.Deletion);
        }

        private static bool CheckCellRevision(Cell cell, IList<Revision> allRevisions, RevisionType revisionType)
        {
            if (allRevisions == null)
                return false;

            foreach (Revision revision in allRevisions)
            {
                if (revision.RevisionType != revisionType)
                    continue;

                if (revision.ParentNode == null)
                    continue;

                if (cell.LastParagraph != revision.ParentNode)
                    continue;

                return true;
            }

            return false;
        }

        /// <summary>
        /// If the last run in the last paragraph of the main story was deleted,
        /// move the contents to the next section and delete this section.
        /// </summary>
        private static void DeleteLastParaFromSection(Paragraph para, RevisionHandlingContext context)
        {
            Body srcBody = (Body)para.FirstNonMarkupParentNode;
            Section srcSection = srcBody.ParentSection;

            Section dstSection = (Section)srcSection.NextSibling;
            if (dstSection != null)
            {
                Body dstBody = dstSection.Body;
                Paragraph dstPara = dstBody.FirstParagraph;

                //Move all inline nodes from the paragraph to the beginning of the first paragraph in the next section.
                if (para.HasChildNodes)
                    dstPara.InsertAfter(para.FirstChild, null, null);

                if (!object.ReferenceEquals(para.ParentNode, srcBody))
                {
                    // WORDSNET-15042 End of section paragraph is a child of SDT.
                    dstBody.InsertAfter(srcBody.FirstChild, null, null);
                    para.ParagraphBreakRunPr.Remove(RevisionAttr.DeleteRevision);
                }
                else
                {
                    //Move all block-level nodes from the body to the beginning of the next section body.
                    dstBody.InsertAfter(srcBody.FirstChild, para, null);
                }

                // Headers/footers are moved only if destination section has no headers/footers.
                if (dstSection.HeadersFooters.Count == 0)
                {
                    // Move headers and footers.
                    while (srcSection.HeadersFooters.Count > 0)
                    {
                        HeaderFooter srcHeaderFooter = srcSection.HeadersFooters[0];

                        // Move whole header/footer to destination section.
                        dstSection.InsertBefore(srcHeaderFooter, dstSection.Body);
                    }
                }

                srcSection.Remove();
            }
            else
            {
                if (!DeleteParagraphCore(para, context, null))
                    RemoveEditRevisions(para);
            }
        }

        /// <summary>
        /// The paragraph mark was deleted, delete the paragraph and join with next.
        /// If the method returns <c>false</c>, behaviour depends on the next deleting paragraph: the next paragraph
        /// should be processed at first.
        /// </summary>
        private static bool DeleteParagraphCore(
            Paragraph para,
            RevisionHandlingContext context,
            IList<Node> deferredDeletionParagraphs)
        {
            Node nextNode = para.NextNonMarkupCompositeLimited;
            Paragraph nextPara = nextNode as Paragraph;
            if (nextPara != null)
            {
                if (IsIndividualStdChild(para, nextPara))
                {
                    CompositeNode ancestor = (CompositeNode)Node.GetCommonAncestor(para, nextPara);
                    CompositeNode paraAncestor = para.ParentNode == ancestor
                        ? para
                        : GetSdtAncestorAsChildOf(para, ancestor);
                    CompositeNode nextParaAncestor = nextPara.ParentNode == ancestor
                        ? nextPara
                        : GetSdtAncestorAsChildOf(nextPara, ancestor);

                    bool processDeletingParagraph = NeedProcessDeletingParagraphWithInliningSdts(
                        paraAncestor,
                        nextParaAncestor,
                        para,
                        nextPara,
                        context);

                    if (processDeletingParagraph)
                    {
                        return ProcessDeletingParagraphWithInliningSdts(
                            para,
                            nextPara,
                            paraAncestor,
                            nextParaAncestor,
                            context);
                    }
                }
            }
            else if (nextNode is Table)
            {
                // If the next node is a table, delete the paragraph after rows/cells/paragraphs of the table will be
                // processed, since the table or its rows/cells may be deleted and contents of the currently deleting
                // paragraph will be lost.
                if (deferredDeletionParagraphs != null)
                {
                    deferredDeletionParagraphs.Add(para);
                    return true;
                }

                nextPara = NodeUtil.FindNextParagraph(para);
            }

            MoveNextNonSdtAnnotations(para, nextPara);
            MoveChildrenAndRemove(para, nextPara);

            return true;
        }

        private static bool IsIndividualStdChild(Node first, Node second)
        {
            CompositeNode firstParent = first.ParentNode;
            CompositeNode secondParent = second.ParentNode;

            if (firstParent == secondParent)
                return false;

            if (firstParent.NodeType == NodeType.StructuredDocumentTag)
                return true;

            if (secondParent.NodeType == NodeType.StructuredDocumentTag)
                return true;

            return false;
        }

        private static bool NeedProcessDeletingParagraphWithInliningSdts(
            CompositeNode paraAncestor,
            CompositeNode nextParaAncestor,
            Paragraph para,
            Paragraph nextPara,
            RevisionHandlingContext context)
        {
            if (paraAncestor == null)
                return false;

            if (nextParaAncestor == null)
                return false;

            if ((paraAncestor == para) && (para.FirstNonAnnotationChild == null))
                return false;

            if (IsAnyAncestorSdtDestinationToNestBlockSdts(para, paraAncestor, context))
                return false;

            if (IsAnyAncestorSdtDestinationToNestBlockSdts(nextPara, nextParaAncestor, context))
                return false;

            return true;
        }

        /// <summary>
        /// Moves annotations except SDT range nodes located after the reference node to the beginning of
        /// the destination node.
        /// </summary>
        private static void MoveNextNonSdtAnnotations(Node referenceNode, CompositeNode destinationNode)
        {
            if (destinationNode == null)
                return;

            Node nextNode = referenceNode.NextSibling;
            Node destinationRefNode = null;

            while ((nextNode != null) && NodeUtil.IsCrossStructureAnnotation(nextNode))
            {
                Node movingNode = nextNode;
                nextNode = nextNode.NextSibling;

                if (NodeUtil.IsStructuredDocumentTagNode(movingNode))
                    continue;

                destinationNode.InsertAfter(movingNode, destinationRefNode);
                destinationRefNode = movingNode;
            }
        }

        /// <summary>
        /// Moves all children of the deleting paragraph into the specified destination paragraph and deletes it.
        /// If there is no destination paragraph and the deleting paragraph has child nodes, it is preserved,
        /// edit revisions are removed.
        /// </summary>
        private static void MoveChildrenAndRemove(Paragraph deletingPara, Paragraph destinationPara)
        {
            // Move all from the deleted paragraph into the next paragraph.
            if (deletingPara.HasChildNodes && (destinationPara != null))
                destinationPara.InsertAfter(deletingPara.FirstChild, null, null);

            // If there is no suitable paragraph to place content in, leave this paragraph and just remove deletion mark.
            if (deletingPara.HasChildNodes)
            {
                RemoveEditRevisions(deletingPara);
            }
            else
            {
                if (deletingPara.ParentNode != null)
                    deletingPara.Remove();
            }
        }

        private static bool IsAnyAncestorSdtDestinationToNestBlockSdts(
            Node node,
            CompositeNode endAncestorSdt,
            RevisionHandlingContext context)
        {
            if (endAncestorSdt.NodeType != NodeType.StructuredDocumentTag)
                return false;

            do
            {
                node = node.ParentNode;
                if ((node == null) || (node.NodeType != NodeType.StructuredDocumentTag))
                    return false;

                foreach (StructuredDocumentTag child in context.GetMovingNestedSdts((StructuredDocumentTag)node))
                {
                    foreach (Node para in child.GetChildNodes(NodeType.Paragraph, true))
                    {
                        if (!context.IsDeletingNode(para))
                            return true;
                    }
                }
            }
            while (node != endAncestorSdt);

            return false;
        }

        /// <summary>
        /// Gets ancestor SDT of the specified node as child of the specified parent node.
        /// </summary>
        /// <param name="node">A node to go through parents to get a result SDT node.</param>
        /// <param name="parent">
        /// An ancestor of a node defined by the parameter above which direct child is searched for.
        /// </param>
        /// <returns>An achieved SDT node or null if any ancestor is not an SDT.</returns>
        private static CompositeNode GetSdtAncestorAsChildOf(Node node, CompositeNode parent)
        {
            if (parent == null)
                return null;

            do
            {
                node = node.ParentNode;
                if ((node == null) || (node.NodeType != NodeType.StructuredDocumentTag))
                    return null;
            }
            while (node.ParentNode != parent);

            return (CompositeNode)node;
        }

        /// <summary>
        /// Clears all edit revisions in the specified run properties.
        /// </summary>
        internal static void RemoveEditRevisions(RunPr runPr)
        {
            runPr.Remove(RevisionAttr.DeleteRevision);
            runPr.Remove(RevisionAttr.InsertRevision);
            runPr.Remove(RevisionAttr.MoveFromRevision);
            runPr.Remove(RevisionAttr.MoveToRevision);
        }

        /// <summary>
        /// Clears all revisions in the specified run properties.
        /// </summary>
        internal static void RemoveRevisions(RunPr runPr)
        {
            RemoveEditRevisions(runPr);
            runPr.Remove(RevisionAttr.FormatRevision);
        }

        /// <summary>
        /// Clears all edit revisions of the specified paragraph.
        /// </summary>
        private static void RemoveEditRevisions(Paragraph para)
        {
            RemoveEditRevisions(para.ParagraphBreakRunPr);
        }

        /// <summary>
        /// Clears all edit revisions of the paragraph and copies formatting from the specified source paragraph.
        /// </summary>
        private static void RemoveEditRevisions(Paragraph para, Paragraph paraToCopyFormattingFrom)
        {
            para.ParagraphBreakRunPr = paraToCopyFormattingFrom.ParagraphBreakRunPr.Clone();
            para.ParaPr = paraToCopyFormattingFrom.ParaPr.Clone();

            RemoveEditRevisions(para);
        }

        /// <summary>
        /// Processes special cases with paragraphs inside SDTs to mimic MS Word behaviour on accepting revisions.
        /// </summary>
        /// <remarks>
        /// A parent block SDT of the deleting paragraph if present is converted to inline one to place into the
        /// destination paragraph. And a parent SDT of the destination paragraph if present is converted to inline
        /// one if any inlines are moved from the deleting paragraph into the destination one.
        /// Returns <c>false</c> if it is not possible to process the deleting paragraph because the next deleting
        /// paragraph should be processed at first. It happen if a parent SDT of the destination has several paragraphs
        /// and cannot be converted to inline yet.
        /// </remarks>
        private static bool ProcessDeletingParagraphWithInliningSdts(Paragraph deletingPara, Paragraph destinationPara,
            CompositeNode ancestorOfDeletingPara, CompositeNode ancestorOfDestinationPara, RevisionHandlingContext context)
        {
            // Converts <sdt><p> node structure of the deleting paragraph and/or the destination paragraph into <p><sdt>
            // and moves contents of the deleting paragraph into the destination one.

            // If parent SDT either of the deleting paragraph or of the destination one is not only paragraph in SDT,
            // MS Word preserves deleting paragraph except special case on moving revisions.
            if ((ancestorOfDeletingPara != deletingPara) &&
                    !IsOnlyNonMarkupCompositeInAncestor(deletingPara, ancestorOfDeletingPara))
            {
                RemoveEditRevisions(deletingPara);
                return true;
            }

            // A deleting paragraph may have no revisions at all if it is in a move range: check for delete and insert.
            bool isMoveRevision =
                !deletingPara.ParagraphBreakRunPr.HasInsertRevision &&
                !deletingPara.ParagraphBreakRunPr.HasDeleteRevision;
            if (isMoveRevision && (ancestorOfDestinationPara != destinationPara))
            {
                // If available place to move an empty SDT to is in an SDT after a move range, MS Word doesn't move
                // the SDT, just keeps it empty. After resaving in MS Word, a new paragraph is created with formatting
                // of the next paragraph and the empty SDT is placed into it. Emulate the same.

                // Move range may be entirely in the deleting paragraph now after deletion of other nodes: start
                // searching from a child of the paragraph (from the last child since MoveRangeFinder searches backward).
                Node searchStart = deletingPara.HasChildNodes ? deletingPara.LastChild : deletingPara;
                MoveRange range = MoveRangeFinder.FindMoveRange(searchStart, !context.IsAcceptance);
                if ((range != null) &&
                    (range.End != null) &&
                    (range.End.IsAncestorNode(destinationPara) || range.End.IsAbove(destinationPara)))
                {
                    RemoveEditRevisions(deletingPara, destinationPara);
                    return true;
                }
            }

            if ((ancestorOfDestinationPara != destinationPara) &&
                !IsOnlyNonMarkupCompositeInAncestor(destinationPara, ancestorOfDestinationPara))
            {
                if (isMoveRevision)
                {
                    // The paragraph is not preserved if it is in a move revision/range: need to delete paragraphs of
                    // the destination SDT at first.
                    foreach (Node para in ancestorOfDestinationPara.GetChildNodes(NodeType.Paragraph, true))
                    {
                        if (context.IsDeletingNode(para))
                            return false;
                    }
                }

                RemoveEditRevisions(deletingPara);
                return true;
            }

            // Check nodes between ancestors of the deleting and destination paragraphs.
            for (Node node = ancestorOfDeletingPara.NextSibling;
                 node != ancestorOfDestinationPara;
                 node = node.NextSibling)
            {
                if (NodeUtil.IsCrossStructureAnnotation(node))
                    continue;

                if ((node.NodeType == NodeType.StructuredDocumentTag) &&
                    ((CompositeNode)node).FirstNonMarkupCompositeDescendant == null)
                    continue;

                RemoveEditRevisions(deletingPara);
                return true;
            }

            if (ancestorOfDeletingPara != deletingPara)
                ExchangeParagraphWithAncestorSdt(deletingPara, (StructuredDocumentTag)ancestorOfDeletingPara, context);
            if (ancestorOfDestinationPara != destinationPara)
                ExchangeParagraphWithAncestorSdt(destinationPara, (StructuredDocumentTag)ancestorOfDestinationPara, context);

            // Move SDTs and annotations between the paragraphs into the destination paragraph to preserve original order.
            Node currentNode = destinationPara.PreviousSibling;
            while (currentNode != deletingPara)
            {
                Node previousNode = currentNode.PreviousSibling;

                if (currentNode.NodeType == NodeType.StructuredDocumentTag)
                {
                    currentNode.Remove();
                    ((StructuredDocumentTag)currentNode).SetLevel(MarkupLevel.Inline);
                }

                destinationPara.InsertAfter(currentNode, null);
                currentNode = previousNode;
            }

            destinationPara.InsertAfter(deletingPara.FirstChild, null, null);
            deletingPara.Remove();

            return true;
        }

        /// <summary>
        /// Exchanges the specified paragraph and its ancestor SDT in the way that the paragraph becomes a parent
        /// of the SDT.
        /// </summary>
        private static void ExchangeParagraphWithAncestorSdt(Paragraph descendant, StructuredDocumentTag ancestor,
            RevisionHandlingContext context)
        {
            Debug.Assert(descendant.IsAncestorNode(ancestor));

            StructuredDocumentTag parent = (StructuredDocumentTag)descendant.ParentNode;
            Node nextNode = descendant.NextSibling;

            // Make the ancestor SDT of the destination paragraph to be of the inline level and move contents of
            // the paragraph into it. Place the SDT into the paragraph.
            ancestor.InsertNext(descendant);

            ancestor.Remove();
            ancestor.SetLevel(MarkupLevel.Inline);

            parent.InsertBefore(descendant.FirstChild, null, nextNode);

            descendant.AppendChild(ancestor);

            context.InlinedSdts.Add(parent);
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node is the only non-markup composite node of the specified ancestor node.
        /// </summary>
        private static bool IsOnlyNonMarkupCompositeInAncestor(CompositeNode node, CompositeNode ancestor)
        {
            return
                (ancestor.FirstNonMarkupCompositeDescendant == node) &&
                (ancestor.LastNonMarkupCompositeDescendant == node);
        }

        /// <summary>
        /// Makes the specified empty SDT to be inline level and moves it to appropriate place.
        /// </summary>
        private static void ProcessSdtNesting(RevisionHandlingContext context)
        {
            foreach (StructuredDocumentTag sdt in context.ParentsForNestedSdts)
            {
                if (sdt.FirstNonMarkupCompositeDescendant != null)
                    continue;

                // If a destination is several nested SDTs, insert into the deepest child SDT.
                StructuredDocumentTag destination = GetDeepestNestedSdt(sdt);

                foreach (StructuredDocumentTag child in context.GetMovingNestedSdts(sdt))
                {
                    if (CanNestSdt(child, sdt, context))
                        destination.AppendChild(child);
                    destination = child;
                }

                destination.InsertDefaultContentIfEmpty();
            }
        }

        /// <summary>
        /// Returns the deepest SDT of possible nested SDT chain started at the specified SDT node.
        /// </summary>
        private static StructuredDocumentTag GetDeepestNestedSdt(StructuredDocumentTag sdt)
        {
            // Expect simple SDT chains only for now and take the last SDT.
            StructuredDocumentTag result = (StructuredDocumentTag)sdt.GetChild(NodeType.StructuredDocumentTag, -1, true);
            return result != null ? result : sdt;
        }

        /// <summary>
        /// Additional checks for necessity to nest one SDT into another. Mimics behaviour of MS Word.
        /// </summary>
        /// <remarks>
        /// See the cases 7, 8 (nested) and 2 (not nested) of WORDSNET-15250 for more information.
        /// Some conditions have already been checked in <see cref="ProcessNextUnmovedNodes"/>.
        /// </remarks>
        private static bool CanNestSdt(StructuredDocumentTag childSdt, StructuredDocumentTag parentSdt,
            RevisionHandlingContext context)
        {
            // Checks whether there are preserved nodes between the SDTs.
            Node node = childSdt.PreviousPreOrder(parentSdt.Document);
            while ((node != null) && !NodeUtil.IsAncestorOrSelf(node, parentSdt))
            {
                if ((node is ITrackableNode) && !context.IsDeletingNode(node))
                    return false;

                node = node.PreviousPreOrder(parentSdt.Document);
            }

            if (childSdt.Level != MarkupLevel.Inline)  // case 7 of WORDSNET-15250
                return true;
            if (!context.CanNestInlineSdts(parentSdt)) // case 8 of WORDSNET-15250
                return false;

            MoveRange range = MoveRangeFinder.FindMoveRange(childSdt, !context.IsAcceptance);

            // Variations of case 2 of WORDSNET-15250
            if (!range.IsDefined)
                return true;

            // Check presence of an opposite move range.
            MoveRevisionType moveRevisionType = context.IsAcceptance
                ? MoveRevisionType.MoveTo
                : MoveRevisionType.MoveFrom;
            if (MoveRangeFinder.FindMoveRangeStartByName(childSdt.Document, range.Start.Name, moveRevisionType) == null)
                return true;

            // Checks whether the destination paragraph has runs.
            Node destination = childSdt.NextNonMarkupCompositeLimited;
            while ((destination != null) &&
                   ((destination.NodeType != NodeType.Paragraph) || context.IsDeletingNode(destination)))
            {
                destination = destination.NextNonMarkupCompositeLimited;
            }

            return (destination == null) || !context.HasPreservedTrackables(destination, true);
        }

        /// <summary>
        /// Inserts default contents into structured document tags that have become empty and inline-level during
        /// handling revisions. Mimics to MS Word.
        /// </summary>
        private static void ProcessInlinedSdts(IList<StructuredDocumentTag> inlinedSdts)
        {
            foreach (StructuredDocumentTag sdt in inlinedSdts)
            {
                StructuredDocumentTag destination = GetDeepestNestedSdt(sdt);
                destination.InsertDefaultContentIfEmpty();
            }
        }

        /// <summary>
        /// Deletes all move range nodes of empty ranges from the document.
        /// </summary>
        private static void DeleteMoveRanges(DocumentBase document)
        {
            RemoveEmptyMoveRanges(document, NodeType.MoveFromRangeStart);
            RemoveEmptyMoveRanges(document, NodeType.MoveToRangeStart);
        }

        /// <summary>
        /// Searches for move range start nodes of the specified type and removes move ranges if they are empty.
        /// </summary>
        private static void RemoveEmptyMoveRanges(DocumentBase document, NodeType rangeStartNodeType)
        {
            List<Node> rangeStarts = document.GetChildNodes(rangeStartNodeType, true).ToNodeList();
            foreach (Node start in rangeStarts)
            {
                MoveRange range = new MoveRange((MoveRangeStart)start);
                if (range.IsEmpty)
                    range.Remove();
            }
        }
    }
}
