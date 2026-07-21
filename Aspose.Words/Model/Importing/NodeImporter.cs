// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/09/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Fonts;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Tables;
using Aspose.Words.Themes;
using Aspose.Xml;

namespace Aspose.Words
{
    /// <summary>
    /// Allows to efficiently perform repeated import of nodes from one document to another.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>Aspose.Words provides functionality for easy copying and moving fragments
    /// between Microsoft Word documents. This is known as "importing nodes".
    /// Before you can insert a fragment from one document into another, you need to "import" it.
    /// Importing creates a deep clone of the original node, ready to be inserted into the
    /// destination document.</p>
    ///
    /// <p>The simplest way to import a node is to use the <see cref="DocumentBase.ImportNode(Node, bool)"/> method
    /// provided by the <see cref="Aspose.Words.DocumentBase"/> object.</p>
    ///
    /// <p>However, when you need to import nodes from one document to another multiple times,
    /// it is better to use the <see cref="NodeImporter"/> class. The <see cref="NodeImporter"/>
    /// class allows to minimize the number of styles and lists created in the destination document.</p>
    ///
    /// <p>Copying or moving fragments from one Microsoft Word document to another presents a number
    /// of technical challenges for Aspose.Words. In a Word document, styles and list formatting
    /// are stored centrally, separately from the text of the document. The paragraphs
    /// and runs of text merely reference the styles by internal unique identifiers.</p>
    ///
    /// <p>The challenges arise from the fact that styles and lists are different in different documents.
    /// For example, to copy a paragraph formatted with the Heading 1 style from one document to another,
    /// a number of things must be taken into account: decide whether to copy the Heading 1 style from
    /// the source document to the destination document, clone the paragraph, update the cloned
    /// paragraph so it refers to the correct Heading 1 style in the destination document.
    /// If the style had to be copied, all the styles that it references (based on style
    /// and next paragraph style) should be analyzed and possibly copied too and so on.
    /// Similar issues exist when copying bulleted or numbered paragraphs because Microsoft Word
    /// stores list definitions separately from text.</p>
    ///
    /// <p>The <see cref="NodeImporter"/> class is like a context, that holds the "translation tables"
    /// during the import. It correctly translates between styles and lists in the source and
    /// destination documents.</p>
    ///
    /// <seealso cref="Document"/>
    /// <seealso cref="DocumentBase.ImportNode(Node, bool)"/>
    /// </remarks>
    public class NodeImporter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeImporter"/> class.
        /// </summary>
        /// <param name="srcDoc">The source document.</param>
        /// <param name="dstDoc">The destination document that will be the owner of imported nodes.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        public NodeImporter(DocumentBase srcDoc, DocumentBase dstDoc, ImportFormatMode importFormatMode)
            : this (srcDoc, dstDoc, new ImportContext(srcDoc, dstDoc, importFormatMode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeImporter"/> class.
        /// </summary>
        /// <param name="srcDoc">The source document.</param>
        /// <param name="dstDoc">The destination document that will be the owner of imported nodes.</param>
        /// <param name="importFormatMode">Specifies how to merge style formatting that clashes.</param>
        /// <param name="importFormatOptions">Specifies various options to format imported node.</param>
        public NodeImporter(DocumentBase srcDoc, DocumentBase dstDoc, ImportFormatMode importFormatMode,
            ImportFormatOptions importFormatOptions)
            : this (srcDoc, dstDoc, new ImportContext(srcDoc, dstDoc, importFormatMode, importFormatOptions))
        {
        }

        internal NodeImporter(DocumentBase srcDoc, DocumentBase dstDoc, ImportContext context)
        {
            if (srcDoc == null)
                throw new ArgumentNullException("srcDoc");
            if (dstDoc == null)
                throw new ArgumentNullException("dstDoc");

            mContext = context;

            CopyThemeNoOverride();

            // AS It's used to copy all embedded fonts from srcDoc to dstDoc. Here we can copy fonts which will not be
            // actually used in imported nodes. The main reason of doing so is two stages of importing. When we do
            // ImportNode, it's difficult to determine font that will be used in destination node because the imported
            // node is not bound to exact place in destination document. And when we do AppendChild we would visit all
            // nested nodes of appended node again. I tried these alternates but didn't managed to do it well.
            //
            // - Embedded fonts binary data is not cloned by this operation, so it don't add much overhead.
            // - It would be good to remove extra fonts on saving.
            // - It makes possible to use all fonts of source document during following editing of destination document,
            // even those which could be skipped during importing.
            dstDoc.FontInfos.Merge(srcDoc.FontInfos);

            mFormattingDifferenceCalculator = new FormattingDifferenceCalculator(mContext);
        }

        /// <summary>
        /// <p>Imports a node from one document into another.</p>
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.ImportNodeCommon"]/*'/>
        /// </remarks>
        /// <param name="srcNode">The node to import.</param>
        /// <param name="isImportChildren"><c>true</c> to import all child nodes recursively; otherwise, <c>false</c>.</param>
        /// <returns>The cloned, imported node. The node belongs to the destination document, but has no parent.</returns>
        public Node ImportNode(Node srcNode, bool isImportChildren)
        {
            return ImportNode(srcNode, isImportChildren, null);
        }

        /// <summary>
        /// <p>Imports a node from one document into another.</p>
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Document.ImportNodeCommon"]/*'/>
        /// </remarks>
        /// <param name="srcNode">The node to import.</param>
        /// <param name="isImportChildren"><c>true</c> to import all child nodes recursively; otherwise, <c>false</c>.</param>
        /// <param name="nodeCloningListener">The listener to the node cloning events.</param>
        /// <returns>The cloned, imported node. The node belongs to the destination document, but has no parent.</returns>
        [JavaConvertCheckedExceptions]
        internal Node ImportNode(Node srcNode, bool isImportChildren, INodeCloningListener nodeCloningListener)
        {
            mContext.TopmostNode = srcNode;
            mIsApplyFormattingDifference = (mContext.ImportFormatMode == ImportFormatMode.KeepSourceFormatting) &&
                                           !mContext.ImportFormatOptions.ForceCopyStyles &&
                                           !CanPreserveSourceStyles(srcNode);
            mIsTopmostNodeIncluded = true;

            using (new SuspendMappedCustomXmlUpdateDocument(mContext.DstDoc))
            {
                Node node = ImportNodeCore(srcNode, isImportChildren, nodeCloningListener);

                UpdateShapes();
                RemoveGoBackBookmarks();

                return node;
            }
        }

        /// <summary>
        /// Updates shapes imported during last import operation.
        /// Shape Ids must be unique in the document. This was not important before linked textboxes support has been implemented
        /// because AW forces shape Ids to be unique during saving. But textboxes Ids must be updated synchronously in
        /// order to preserve links between them.
        /// </summary>
        /// <remarks>
        /// AM. Called only after externally called import operations (is not called during recursive calls).
        /// This allows to treat externally called import as single operation i.e
        /// distinguish for example section importing which should preserves textboxes links from
        /// single textbox importing which should break links.
        ///
        /// This is intermediate solution covers several importing issue and helps to reduce corrupted document user can get.
        /// WORDSNET-5905 was risen for full support of linked textboxes.
        /// </remarks>
        private void UpdateShapeIds()
        {
            if (mImportedShapes.Count == 0)
                return;

            DocumentBase doc = mContext.DstDoc;
            // Base index. Every imported shape will be updated to this base.
            int baseShapeId = doc.GetNextShapeId();
            // Avoid to put identifiers of the linked shapes to different identifier ranges.
            baseShapeId = DocumentBase.GetNearestMapId(baseShapeId);
            // Maximum updated shape Id. Actually it's new base index of this document after import will be completed.
            int maxShapeId = baseShapeId;

            foreach (ShapeBase shape in mImportedShapes.Values)
            {
                shape.Id += baseShapeId;
                // I don't care for a while about Ids can possible go out of range. Just care about they are unique.
                maxShapeId = System.Math.Max(maxShapeId, shape.Id);

                if (shape.TextboxNextShapeId != 0)
                {
                    if (mImportedShapes[shape.TextboxNextShapeId] == null)
                    {
                        // This means that referred shape has not been imported during last import operation so break link.
                        shape.TextboxNextShapeId = 0;
                    }
                    else
                    {
                        // Referred shape has been imported so update link to new id.
                        shape.TextboxNextShapeId += baseShapeId;
                        maxShapeId = System.Math.Max(maxShapeId, shape.TextboxNextShapeId);
                    }
                }
            }

            // Update maximum shape Id used so next import operation will use it as new base.
            mContext.DstDoc.SetNextShapeId(++maxShapeId);
        }

        /// <summary>
        /// Updates drawingMl imported during last import operation.
        /// DrawingMl textboxIds and linkedIds must be unique in the document.
        /// </summary>
        private void UpdateDmlTextboxIds()
        {
            // WORDSNET-13866 Textbox identifier update algorithm was changed.

            // Go through textboxes and update identifiers.
            foreach (ShapeBase dml in mDmlTxbxByTxbxId.Values)
            {
                ShapePr pr = dml.ShapePr;
                DocumentBase doc = mContext.DstDoc;

                // Try to find appropriate linked textbox chain.
                SortedIntegerListGeneric<ShapeBase> lnkItems = mDmlLinkedTxbxByLnkId[dml.TextboxId];

                int nShapeId = doc.GetNextShapeId(dml);
                nShapeId = (lnkItems != null) ? doc.MapShapeToRange(nShapeId, lnkItems.Count) : nShapeId;

                // Update shape identifier for unique value in destination document.
                pr[ShapeAttr.ShapeId] = nShapeId;
                int prevTxbxId = dml.TextboxId;

                // When textbox has identifier of the next shape but linked textbox chain for this
                // element does not exist then reset next textbox identifier.
                if (lnkItems == null && pr[ShapeAttr.TextboxNextShapeId] != null)
                {
                    dml.TextboxId = 0;
                    pr[ShapeAttr.TextboxNextShapeId] = 0;
                }

                // Update drawingMl TextboxId for unique value in destination document.
                if ((dml.TextboxId > 0) && (lnkItems != null))
                    dml.TextboxId = doc.GetNextDmlTextBoxId();

                // Update linked textboxes identifiers.
                if (lnkItems != null)
                {
                    UpdateTxBxNextShapeId(dml, lnkItems);

                    // Textboxes chain was updated, remove for post processing only broken chains.
                    if (prevTxbxId > 0)
                        mDmlLinkedTxbxByLnkId.Remove(prevTxbxId);
                }
            }

            // Go through linked textboxes which grouped by linked textbox identifier.
            // If group has not appropriate textbox in collection then update
            // liked items as broken.
            foreach (SortedIntegerListGeneric<ShapeBase> lnkItems in mDmlLinkedTxbxByLnkId.Values)
                ProcessBrokenTxbxChain(lnkItems);
        }

        /// <summary>
        /// Process skipped linked textboxes.
        /// </summary>
        /// <param name="lnkItems">Collection of linked textboxes of one chain.</param>
        private void ProcessBrokenTxbxChain(SortedIntegerListGeneric<ShapeBase> lnkItems)
        {
            Debug.Assert(lnkItems != null);

            // It looks like imported node contains separate linked textbox elements chain
            // without appropriate textbox. Remove links between such items.
            for (int i = 0; i < lnkItems.Count; ++i)
            {
                ShapeBase lnkTxBx = lnkItems.GetByIndex(i);

                lnkTxBx.LinkedTextboxId = 0;
                lnkTxBx.LinkedTextboxSeq = 0;
                lnkTxBx.ShapePr[ShapeAttr.TextboxNextShapeId] = 0;
                lnkTxBx.ShapePr[ShapeAttr.ShapeId] = mContext.DstDoc.GetNextShapeId();
            }
        }

        /// <summary>
        /// Updates next shape identifier for textbox and related linked textbox chain.
        /// </summary>
        /// <param name="dml">First shape in the linked textbox chain.</param>
        /// <param name="lnkItems">Collection of linked textboxes of one chain.</param>
        private void UpdateTxBxNextShapeId(ShapeBase dml, SortedIntegerListGeneric<ShapeBase> lnkItems)
        {
            Debug.Assert((dml != null) && (lnkItems != null));

            int lnkCount = lnkItems.Count;
            int lnkTxbxId = dml.TextboxId;
            int newNextId = mContext.DstDoc.GetNextShapeId();
            dml.ShapePr[ShapeAttr.TextboxNextShapeId] = newNextId;

            for (int i = 0; i < lnkCount; ++i)
            {
                ShapeBase lnkTxBx = lnkItems.GetByIndex(i);

                // Case when textbox sequence number is invalid.
                if (lnkTxBx.LinkedTextboxSeq > (i + 1))
                {
                    lnkTxbxId = 0;
                    lnkTxBx.LinkedTextboxSeq = 0;
                    dml.ShapePr[ShapeAttr.TextboxNextShapeId] = 0;
                }

                lnkTxBx.LinkedTextboxId = lnkTxbxId;
                lnkTxBx.ShapePr[ShapeAttr.ShapeId] = newNextId;

                if (i < (lnkCount - 1))
                {
                    newNextId = (lnkTxbxId > 0) ? mContext.DstDoc.GetNextShapeId() : 0;
                    lnkTxBx.ShapePr[ShapeAttr.TextboxNextShapeId] = newNextId;
                }
            }
        }

        /// <summary>
        /// Removes imported _GoBack bookmarks.
        /// </summary>
        private void RemoveGoBackBookmarks()
        {
            // Do not remove bookmarks if something goes wrong.
            if (mGoBackBookmarkStarts.Count == mGoBackBookmarkEnds.Count)
            {
                for (int i = 0; i < mGoBackBookmarkStarts.Count; i++)
                {
                    BookmarkStart bookmarkStart = mGoBackBookmarkStarts[i];
                    BookmarkEnd bookmarkEnd = mGoBackBookmarkEnds[i];
                    bookmarkStart.Remove();
                    bookmarkEnd.Remove();
                }
            }

            mGoBackBookmarkStarts.Clear();
            mGoBackBookmarkEnds.Clear();
        }

        private Node ImportNodeCore(Node srcNode, bool isImportChildren, INodeCloningListener nodeCloningListener)
        {
            if (srcNode == null)
                throw new ArgumentNullException("srcNode");

            Node dstNode = srcNode.Clone(false, nodeCloningListener);
            dstNode.SetDocument(mContext.DstDoc);

            // Import children of the source node if needed.
            if (isImportChildren && srcNode.IsComposite)
                ImportAndAppendChildrenCore((CompositeNode)srcNode, (CompositeNode)dstNode, nodeCloningListener);

            ImportNodeWithoutChildren(srcNode, dstNode);

            // Postconditions of a correct import.
            Debug.Assert(dstNode.Document == mContext.DstDoc);
            Debug.Assert(dstNode.ParentNode == null);

            return dstNode;
        }

        /// <summary>
        /// Imports children from the source container to the destination document and
        /// adds them to the destination container.
        /// </summary>
        internal void ImportAndAppendChildren(CompositeNode srcContainer, CompositeNode dstContainer)
        {
            mContext.TopmostNode = srcContainer;
            mIsApplyFormattingDifference = (mContext.ImportFormatMode == ImportFormatMode.KeepSourceFormatting) &&
                                           !mContext.ImportFormatOptions.ForceCopyStyles &&
                                           !CanPreserveSourceStyles(srcContainer);
            mIsTopmostNodeIncluded = false;

            ImportAndAppendChildrenCore(srcContainer, dstContainer, null);
            UpdateShapes();
            RemoveGoBackBookmarks();
        }

        /// <summary>
        /// Imports list specified in <paramref name="srcParaPr"/> and applies it to <paramref name="dstParaPr"/>.
        /// </summary>
        internal static void ImportList(ParaPr srcParaPr, ParaPr dstParaPr, ImportContext context)
        {
            int srcListId;
            // WORDSNET-27842 Always expand and import listId to direct attributes in Merger.
            if (context.ImportFormatOptions.IsMerger)
            {
                srcListId = GetListId(srcParaPr, context.SrcStyles);
                if (srcListId == -1)
                    return;
            }
            else
            {
                srcListId = srcParaPr.ListId;
            }

            if (srcListId == 0)
            {
                // WORDSNET-24305 Check that when there is no list defined in direct paragraph`s attributes,
                // but its style is numbered, then corresponding style's list level refers to this style.
                // Otherwise, Word does not show numbering at all and thus forced to set numbering properties
                // into the direct paragraph's attributes.
                Style dstStyle = context.DstStyles.GetByIstd(dstParaPr.Istd, false);
                if (dstStyle != null)
                {
                    int listId = dstStyle.ParaPr.ListId;
                    List dstList = context.DstLists.GetListByListId(listId);
                    if (dstList == null)
                        return;

                    // WORDSNET-25249 Consider inherited listLevel.
                    object levelObj = dstStyle.GetParaAttr(ParaAttr.ListLevel, RevisionsView.Final);
                    if (levelObj == null)
                        return;

                    int level = (int)levelObj;
                    ListLevel listLevel = dstList.GetListLevelOverrideAware(level);
                    Style linkedStyle = listLevel.LinkedStyle;
                    if (linkedStyle == null)
                        return;

                    if (linkedStyle.Istd != dstStyle.Istd)
                    {
                        dstParaPr[ParaAttr.ListId] = listId;
                        dstParaPr[ParaAttr.ListLevel] = level;
                    }
                }

                return;
            }

            bool isSameDoc = (context.DstDoc == context.SrcDoc);

            dstParaPr[ParaAttr.ListId] = isSameDoc ? srcListId : context.DstLists.ImportList(context, srcListId);

            List srcList = context.SrcLists.GetListByListId(srcListId);
            if (srcList == null)
                return;

            // List was imported successfully and neither of the list definitions in the destination were not reused.
            if (isSameDoc || context.ImportedListDefIds.ContainsKey(srcList.ListDefId))
                return;

            // Mimic MSW and preserve formatting which implemented in the source definition of the list.
            // Calculate difference between source and destination definitions and apply it for paragraph
            // to complete this task.
            ListLevel dstListLevel = context.DstLists.GetDirectListLevel(dstParaPr);
            ListLevel srcListLevel = context.SrcLists.GetDirectListLevel(srcParaPr);
            ImportParaPrSaver saver = new ImportParaPrSaver(srcListLevel.ParaPr, dstListLevel.ParaPr);

            // Calculate formatting difference.
            ParaPr srcListLevelParaPr = srcListLevel.ParaPr.Clone();
            srcListLevelParaPr.Collapse(dstListLevel.ParaPr);

            // Apply formatting to paragraph properties.
            srcListLevelParaPr.ExpandTo(dstParaPr, saver);
        }

        /// <summary>
        /// Imports Citation sources.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal void ImportCitationSources()
        {
            CustomXmlPartCollection srcXmlParts = mContext.SrcDoc.FetchDocumentOrGlossaryMain().CustomXmlParts;
            CustomXmlPart srcSources = srcXmlParts.GetCitationSources();
            if (srcSources == null)
                return;

            CustomXmlPartCollection dstXmlParts = mContext.DstDoc.FetchDocumentOrGlossaryMain().CustomXmlParts;
            CustomXmlPart dstSources = dstXmlParts.GetCitationSources();
            if (dstSources != null)
            {
                AppendCitations(dstSources, srcSources);
                return;
            }

            if (!IsEmptyCitationCollection(srcSources))
                dstXmlParts.Add(srcSources.Clone());
        }

        /// <summary>
        /// Update identifiers of the shapes.
        /// </summary>
        private void UpdateShapes()
        {
            UpdateShapeIds();
            UpdateDmlTextboxIds();

            mImportedShapes.Clear();
            mDmlTxbxByTxbxId.Clear();
            mDmlLinkedTxbxByLnkId.Clear();
        }

        /// <summary>
        /// Imports children from the source container to the destination document and
        /// adds them to the destination container.
        /// </summary>
        private void ImportAndAppendChildrenCore(
            CompositeNode srcContainer,
            CompositeNode dstContainer,
            INodeCloningListener nodeCloningListener)
        {
            if (srcContainer == null)
                throw new ArgumentNullException("srcContainer");
            if (srcContainer.Document != mContext.SrcDoc)
                throw new ArgumentException("srcContainer should belong to the source document.");

            if (dstContainer == null)
                throw new ArgumentNullException("dstContainer");
            if (dstContainer.Document != mContext.DstDoc)
                throw new ArgumentException("dstContainer should belong to the destination document.");

            for (Node child = srcContainer.FirstChild; child != null; child = child.NextSibling)
            {
                dstContainer.AppendChild(ImportNodeCore(child, true, nodeCloningListener));
            }
        }

        /// <summary>
        /// <para>Performs all operations needed to import a node (excluding its children).</para>
        /// </summary>
        private void ImportNodeWithoutChildren(Node srcNode, Node dstNode)
        {
            if (srcNode is ShapeBase)
            {
                ImportShapeBase((ShapeBase)srcNode, (ShapeBase)dstNode);
            }
            else if (srcNode is IInline)
            {
                mFormattingDifferenceCalculator.VisitNode(srcNode);
                ImportIInline((IInline)srcNode, (IInline)dstNode);
            }
            else if (srcNode is Paragraph)
            {
                ImportParagraph((Paragraph)srcNode, (Paragraph)dstNode);
            }
            else if (srcNode is Row)
            {
                ImportRow(dstNode);
            }
            else if (srcNode is Cell)
            {
                ImportCell(dstNode);
            }
            else if (srcNode is StructuredDocumentTag)
            {
                ImportStructuredDocumentTag((StructuredDocumentTag)srcNode, (StructuredDocumentTag)dstNode);
            }
            else if ((srcNode is StructuredDocumentTagRangeStart) || (srcNode is StructuredDocumentTagRangeEnd))
            {
                ImportStructuredDocumentTagRange(srcNode, dstNode);
            }
            else if (srcNode is BookmarkStart)
            {
                if(mContext.ImportFormatOptions.RenameDuplicateBookmarks)
                    UpdateBookmarkNameIfNeeded((BookmarkStart)srcNode, (BookmarkStart)dstNode);
            }
            else if (srcNode is BookmarkEnd)
            {
                if(mContext.ImportFormatOptions.RenameDuplicateBookmarks)
                    UpdateBookmarkNameIfNeeded((BookmarkEnd)srcNode, (BookmarkEnd)dstNode);
            }
            if (srcNode is INodeWithAnnotationId)
            {
                ImportINodeWithCommentId((INodeWithAnnotationId)srcNode, (INodeWithAnnotationId)dstNode);
            }
        }

        private void ImportShapeBase(ShapeBase srcShape, ShapeBase dstShape)
        {
            if (srcShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                ImportDmlNode(srcShape, dstShape);
            }
            else if (srcShape is Shape)
            {
                int dstShapeId = ((Shape)dstShape).Id;

                if (mImportedShapes[dstShapeId] != null)
                {
                    // WORDSNET-6171 Source document has non unique shape Ids.
                    // This is not a problem till it's not linked textbox. In this case textbox link can be lost.
                    // We should warn customer about this but I'm not sure about runtime warning.

                    // WARN. Source document has non unique shape Ids. Textbox links can be lost.
                }
                else
                {
                    // Collect shapes to update them after import.
                    mImportedShapes.Add(dstShapeId, dstShape);
                }
            }

            // WORDSNET-23458 Seems Word always makes imported ZOrders greater, than any of the destination ZOrders.
            object zOrder = dstShape.ShapePr[ShapeAttr.ZOrder];
            if (zOrder != null)
                dstShape.ZOrder = MaxDstZOrder + (int)zOrder;
        }

        private void ImportCell(Node dstNode)
        {
            if (mContext.ImportFormatMode == ImportFormatMode.KeepSourceFormatting)
            {
                // Remove all theme attributes of table cell to preserve source formatting
                // and avoid its updating from destination document themes.
                ThemeColorRemover.ResetCellThemeAttrs((Cell)dstNode);
            }
            else
            {
                // Update direct color formatting of table cell using destination document themes.
                ThemeColorUpdater.Update((Cell)dstNode);
            }
        }

        private void ImportRow(Node dstNode)
        {
            // Note, that at this point dstNode is the same as srcNode.
            Row dstRow = (Row)dstNode;
            Style tableStyle = ImportStyle(dstRow);
            dstRow.TablePr.SetAttr(TableAttr.Istd, tableStyle.Istd);

            if (mContext.ImportFormatMode == ImportFormatMode.KeepSourceFormatting)
            {
                // Reset all theme attributes of table row to preserve source formatting
                // and avoid its updating from destination document themes.
                ThemeColorRemover.ResetRowThemeAttrs((Row)dstNode);
            }
            else
            {
                // Update direct color formatting of table row using destination document themes.
                ThemeColorUpdater.Update((Row)dstNode);
            }
        }

        private void ImportDmlNode(ShapeBase srcDml, ShapeBase dstDml)
        {
            DmlNodeType dmlType = srcDml.DmlNode.DmlNodeType;

            // Remove FallbackShape to generate it from scratch upon validation.
            // We need this to correctly update linked textboxes in fallback shape.
            // Graphic frame can not be placed as top level element, so does not have fallback shape.
            if (((dmlType == DmlNodeType.WordprocessingGroupShape) || (dmlType == DmlNodeType.WordprocessingShape))
              && (dstDml.FallbackShape != null))
                dstDml.RunPr.AlternateContent.FallBack = null;

            switch (dstDml.DmlNode.DmlNodeType)
            {
                case DmlNodeType.WordprocessingShape:
                    {
                        CollectTextBoxes(dstDml);
                        break;
                    }
                case DmlNodeType.Chart:
                case DmlNodeType.ChartEx:
                    {
                        DmlChartSpace chartSpace = (DmlChartSpace) dstDml.DmlNode;
                        if (mContext.ImportFormatMode == ImportFormatMode.KeepSourceFormatting)
                        {
                            if ((chartSpace.ThemeOverride == null) && (srcDml.Document.GetThemeInternal() != null))
                                chartSpace.ThemeOverride = srcDml.Document.GetThemeInternal().CreateThemeOverride();
                        }
                        break;
                    }
                default:
                    {
                        // Other Dml nodes do not needed additional processing upon importing. So do nothing.
                        break;
                    }
            }

            // WORDSNET-22479 Resolve theme colors. Actually, this should be done for all colors in shape,
            // but at the moment it is only done for colors of the shape fills.
            // WORDSNET-28610 Added resolving theme colors for shape Outline.
            if (mIsApplyFormattingDifference && (dstDml.DmlShape != null))
            {
                dstDml.DmlShape.Fill.ResolveThemeColors(srcDml.DocumentTheme);
                dstDml.DmlShape.Outline.ResolveThemeColors(srcDml.DocumentTheme, dstDml.DmlShape.Style);
            }
        }

        /// <summary>
        /// Collects drawingMls to update their linked textboxes after import.
        /// </summary>
        /// <param name="dstDml">Clone of the source shape which will be imported.</param>
        private void CollectTextBoxes(ShapeBase dstDml)
        {
            Debug.Assert(dstDml != null);

            // Current reading algorithm assigns unique identifiers for textboxes (shape ID and textbox ID),
            // so this method does not implement processing shapes with duplicate identifiers.
            if (dstDml.LinkedTextboxId > 0)
                CollectLinkedTxbx(dstDml);

            if (dstDml.TextboxId > 0)
                mDmlTxbxByTxbxId.Add(dstDml.TextboxId, dstDml);
        }

        /// <summary>
        /// Insert linked textbox to collection.
        /// </summary>
        /// <param name="lnkTxbx">Linked textbox to insert.</param>
        private void CollectLinkedTxbx(ShapeBase lnkTxbx)
        {
            Debug.Assert(lnkTxbx != null);

            int lnkTxbxId = lnkTxbx.LinkedTextboxId;

            if (!mDmlLinkedTxbxByLnkId.ContainsKey(lnkTxbxId))
                mDmlLinkedTxbxByLnkId[lnkTxbxId] = new SortedIntegerListGeneric<ShapeBase>();

            mDmlLinkedTxbxByLnkId[lnkTxbxId][lnkTxbx.LinkedTextboxSeq] = lnkTxbx;
        }

        /// <summary>
        /// Imports run properties of nodes which implements <see cref="IInline"/>.
        /// </summary>
        private void ImportIInline(IInline srcInline, IInline dstInline)
        {
            // 1. Word imports styles always using UseDestinationMode first,
            // i.e. it does not override existing styles.
            //  a) original attributes.
            RunPr srcRunPr = srcInline.RunPr_IInline;
            RunPr dstRunPr = dstInline.RunPr_IInline;
            ImportStyleUseDestinationMode(srcRunPr, dstRunPr, FontAttr.Istd);
            //  b) revised attributes.
            if (srcInline.RunPr_IInline.HasFormatRevision)
                ImportStyleUseDestinationMode(srcRunPr.FormatRevision.RevPr, dstRunPr.FormatRevision.RevPr, FontAttr.Istd);

            if (mIsApplyFormattingDifference && CanApplyFormattingDifference(srcInline))
                mFormattingDifferenceCalculator.Apply(srcInline, dstInline);
        }

        /// <summary>
        /// Imports paragraph and run properties of <see cref="Paragraph"/>.
        /// </summary>
        private void ImportParagraph(Paragraph srcPara, Paragraph dstPara)
        {
            if (srcPara.IsListItem && (mContext.ImportedListIds.Count == 0))
                mContext.SetNonReusableListDefId(GetNonReusableListDefId(srcPara));

            // Word creates missed BuiltIn styles instead of importing it inside original revisions.
            bool isStyleInOriginalRevision = (srcPara.ParaPr.HasFormatRevision && (srcPara.ParaPr.Istd != 0));
            if (isStyleInOriginalRevision)
                CreateMissingBuiltInParagraphStyles(srcPara);

            // 1. Word imports styles and lists always using UseDestinationMode first,
            // i.e. it does not override existing styles.
            //   a) original attributes.
            ParaPr srcParaPr = srcPara.ParaPr;
            ParaPr dstParaPr = dstPara.ParaPr;
            ImportStyleUseDestinationMode(srcParaPr, dstParaPr, ParaAttr.Istd);
            ImportList(srcParaPr, dstParaPr);
            //   b) revised attributes.
            if (srcParaPr.HasFormatRevision)
            {
                WordAttrCollection srcRevPr = srcParaPr.FormatRevision.RevPr;
                WordAttrCollection dstRevPr = dstParaPr.FormatRevision.RevPr;
                ImportStyleUseDestinationMode(srcRevPr, dstRevPr, ParaAttr.Istd);
                ImportList((ParaPr)srcRevPr, (ParaPr)dstRevPr);
            }

            // 2. Deal with ParagraphBreakRunPr.
            //   a) original attributes.
            RunPr srcBreakRunPr = srcPara.ParagraphBreakRunPr;
            RunPr dstBreakRunPr = dstPara.ParagraphBreakRunPr;
            ImportStyleUseDestinationMode(srcBreakRunPr, dstBreakRunPr, FontAttr.Istd);
            //   b) revised attributes.
            if (srcBreakRunPr.HasFormatRevision)
            {
                WordAttrCollection srcRevPr = srcBreakRunPr.FormatRevision.RevPr;
                WordAttrCollection dstRevPr = dstBreakRunPr.FormatRevision.RevPr;
                ImportStyleUseDestinationMode(srcRevPr, dstRevPr, FontAttr.Istd);
            }

            // WORDSNET-13302 Word removes paragraph break properties equal to its style
            // properties upon importing with UseDestinationStyles option.
            if (mContext.ImportFormatMode == ImportFormatMode.UseDestinationStyles)
                dstBreakRunPr.RemoveEquals(srcPara.ParagraphStyle.RunPr);

            // WORDSNET-24437 Word always sets direct relative horizontal position into a non-empty Frame.
            if (dstParaPr.HasFrameAttributes && !dstParaPr.HasFrameRelativeHorizontalPosition)
            {
                dstParaPr[ParaAttr.FrameRelativeHorizontalPosition] =
                    srcPara.FetchInheritedParaAttr(ParaAttr.FrameRelativeHorizontalPosition, RevisionsView.Original);
            }

            if (mIsApplyFormattingDifference && CanApplyFormattingDifference(srcPara))
                mFormattingDifferenceCalculator.Apply(srcPara, dstPara);
        }

        /// <summary>
        /// Imports <see cref="StructuredDocumentTag"/>.
        /// </summary>
        /// <remarks>
        /// RK We don't expand run properties here, I don't think it is needed.
        /// </remarks>
        private void ImportStructuredDocumentTag(StructuredDocumentTag srcSdt, StructuredDocumentTag dstSdt)
        {
            if (srcSdt.Document != dstSdt.Document) // we want to try to maintain ids when importing to new document.
                dstSdt.SetId(srcSdt.Id);

            if (!mContext.ImportedSdtIds.ContainsKey(srcSdt.Id))
                mContext.ImportedSdtIds.Add(srcSdt.Id, dstSdt.Id);

            if (srcSdt.ContentsRunPr.Count != 0) // prevent importing of default styles here.
                ImportStyleUseDestinationMode(srcSdt.ContentsRunPr, dstSdt.ContentsRunPr, FontAttr.Istd);

            if (srcSdt.EndCharacterRunPr.Count != 0) // prevent importing of default styles here.
                ImportStyleUseDestinationMode(srcSdt.EndCharacterRunPr, dstSdt.EndCharacterRunPr, FontAttr.Istd);

            // WORDSNET-28759 Apply formatting difference to mimic Word.
            if (mIsApplyFormattingDifference && CanApplyFormattingDifference(srcSdt.ParentNode as Paragraph))
            {
                StructuredDocumentTagToInlineAdapter srcInlineAdapter = new StructuredDocumentTagToInlineAdapter(srcSdt);
                StructuredDocumentTagToInlineAdapter dstInlineAdapter = new StructuredDocumentTagToInlineAdapter(dstSdt);
                mFormattingDifferenceCalculator.Apply(srcInlineAdapter, dstInlineAdapter);
            }

            ImportPlaceholderIfNeeded(dstSdt, dstSdt.Placeholder);
            HandleCustomXmlParts(dstSdt);
        }

        /// <summary>
        /// Imports range for ranged structured document tag.
        /// </summary>
        private void ImportStructuredDocumentTagRange(Node srcSdtRange, Node dstSdtRange)
        {
            if (srcSdtRange.NodeType == NodeType.StructuredDocumentTagRangeStart)
            {
                StructuredDocumentTagRangeStart srcRangeStart = (StructuredDocumentTagRangeStart)srcSdtRange;
                StructuredDocumentTagRangeStart dstRangeStart = (StructuredDocumentTagRangeStart)dstSdtRange;

                dstRangeStart.InternalSdt = (StructuredDocumentTag)srcRangeStart.InternalSdt.Clone(false);

                dstRangeStart.SetId(dstRangeStart.InternalSdt.Id);
                // FIX 25758. Check whether this StructuredDocumentTagRangeStart has already been imported before.
                if (mContext.ImportedSdtIds.ContainsKey(srcRangeStart.Id))
                    mContext.ImportedSdtIds.Remove(srcRangeStart.Id);

                mContext.ImportedSdtIds.Add(srcRangeStart.Id, dstRangeStart.Id);

                HandleCustomXmlParts(dstRangeStart.InternalSdt);
            }
            else
            {
                StructuredDocumentTagRangeEnd srcRangeEnd = (StructuredDocumentTagRangeEnd)srcSdtRange;
                StructuredDocumentTagRangeEnd dstRangeEnd = (StructuredDocumentTagRangeEnd)dstSdtRange;

                int newId = mContext.ImportedSdtIds.ContainsKey(srcRangeEnd.Id)
                    ? mContext.ImportedSdtIds[srcRangeEnd.Id]
                    : srcRangeEnd.Id;

                dstRangeEnd.SetIdInternal(newId);
            }
        }

        /// <summary>
        /// SDT placeholder is a reference to a building block. If this reference is non-empty and is not contained in the glossary
        /// of the target document, then import this building block and reassign placeholder reference in dstSdt.
        /// </summary>
        private static void ImportPlaceholderIfNeeded(StructuredDocumentTag dstSdt, BuildingBlock placeholderReference)
        {
            if (placeholderReference != null)
            {
                GlossaryDocument dstDocumentGlossary = GetOrCreateGlossaryDocument(dstSdt.Document);

                if (IsImportPlaceholder(dstDocumentGlossary, placeholderReference))
                {
                    BuildingBlock dstBuildingBlock = (BuildingBlock)dstDocumentGlossary.ImportNode(placeholderReference, true);
                    dstDocumentGlossary.AppendChild(dstBuildingBlock);
                    dstSdt.UpdatePlaceholderReference(true);
                }
                else
                {
                    dstSdt.UpdatePlaceholderReference(true);
                }
            }
        }

        /// <summary>
        /// Checks that placeholder should be imported to destination glossary.
        /// </summary>
        private static bool IsImportPlaceholder(GlossaryDocument glossary, BuildingBlock placeholderReference)
        {
            // if placeholder block is not contained in the target doc glossary.
            if (placeholderReference.Document == glossary)
                return false;

            if (placeholderReference.Type == BuildingBlockType.StructuredDocumentTagPlaceholderText)
            {
                // Do not import if placeholder's guid already exists.
                foreach (BuildingBlock buildingBlock in glossary.BuildingBlocks)
                    if (placeholderReference.Guid == buildingBlock.Guid)
                        return false;

                // Reuse placeholder with predefined name.
                if (SdtPlaceholderManager.IsPredefinedPlaceholderName(placeholderReference.Name))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Handles custom XML parts collection.
        /// </summary>
        private void HandleCustomXmlParts(StructuredDocumentTag sdt)
        {
            if (sdt.XmlMapping.IsEmpty || !StringUtil.HasChars(sdt.XmlMapping.StoreItemId))
                return;

            CustomXmlPartCollection dstCollection = mContext.DstDoc.FetchDocumentOrGlossaryMain().CustomXmlParts;
            if (ContainsId(dstCollection, sdt.XmlMapping.StoreItemId))
                return;

            // Finding the CustomXmlParts with proper structure.
            List<CustomXmlPart> xmlParts = GetXmlPartsWithMatchingXmlMapping(dstCollection, sdt.XmlMapping);

            // Changing the StoreItemId for the only CustomXmlPart without FlatOPC inside.
            if ((xmlParts != null) && (xmlParts.Count == 1) && !ContainsFlatOpc(xmlParts[0], sdt.XmlMapping))
            {
                sdt.XmlMapping.StoreItemId = xmlParts[0].Id;
                return;
            }

            // Otherwise, copying the CustomXmlPart with proper structure and corresponding StoreItemId.
            CustomXmlPartCollection srcCollection = mContext.SrcDoc.FetchDocumentOrGlossaryMain().CustomXmlParts;
            foreach (CustomXmlPart srcPart in GetXmlPartsWithMatchingXmlMapping(srcCollection, sdt.XmlMapping))
            {
                if (ReferenceEquals(srcPart, srcCollection.GetById(sdt.XmlMapping.StoreItemId)))
                {
                    dstCollection.Add(srcPart.Clone());
                    break;
                }
            }
        }

        /// <summary>
        /// Checks that collection contain CustomXmlPart with given Id.
        /// </summary>
        private static bool ContainsId(CustomXmlPartCollection collection, string id)
        {
            foreach (CustomXmlPart part in collection)
                if (part.Id == id)
                    return true;

            return false;
        }

        /// <summary>
        /// Gets non-null document glossary from this DocumentBase, creates one if needed.
        /// </summary>
        private static GlossaryDocument GetOrCreateGlossaryDocument(DocumentBase doc)
        {
            GlossaryDocument documentGlossary = (doc.NodeType == NodeType.GlossaryDocument) ? (GlossaryDocument)doc : ((Document)doc).GlossaryDocument;
            if (documentGlossary == null)
            {
                Document targetDoc = (Document)doc;
                targetDoc.GlossaryDocument = new GlossaryDocument();
                documentGlossary = targetDoc.GlossaryDocument;
            }

            return documentGlossary;
        }

        /// <summary>
        /// Imports a comment for comments and comment ranges.
        /// </summary>
        private void ImportINodeWithCommentId(INodeWithAnnotationId src, INodeWithAnnotationId dst)
        {
            IntToIntDictionary importedCommentIds = mContext.ImportedCommentIds;
            int dstId = importedCommentIds[src.IdInternal];
            if (IntToIntDictionary.IsNullSubstitute(dstId))
            {
                dstId = mContext.DstDoc.GetNextAnnotationId();
                importedCommentIds[src.IdInternal] = dstId;
            }
            dst.IdInternal = dstId;

            // Comment also can have a parent annotation, so we have to update its Id.
            if (src.ParentIdInternal != Comment.NoParent)
                dst.ParentIdInternal = importedCommentIds[src.ParentIdInternal];
        }

        /// <summary>
        /// Copies the theme if it is present in the source document and non present in the destination document.
        /// This helps to solve some (but not all) formatting loss problems during copying of nodes.
        /// </summary>
        private void CopyThemeNoOverride()
        {
            Document srcDoc = mContext.SrcDoc as Document;
            // If src document is not a main document, then there is no theme to copy.
            if (srcDoc == null)
                return;

            // If dst document is not a main document, there is nowhere to copy the theme to.
            Document dstDoc = mContext.DstDoc as Document;
            if (dstDoc == null)
                return;

            if ((srcDoc.GetThemeInternal() != null) && (dstDoc.GetThemeInternal() == null))
                dstDoc.SetThemeInternal(srcDoc.GetThemeInternal().Clone());
        }

        /// <summary>
        /// Renames bookmark start being imported if such bookmark name already exists in document.
        /// </summary>
        private void UpdateBookmarkNameIfNeeded(BookmarkStart srcBookmarkStart, BookmarkStart dstBookmarkStart)
        {
            // Names of bookmarks of building blocks can be the same as names of bookmarks of other parts of a document.
            // Therefore, if a bookmark is inside a building block that is being imported too, let's assume that bookmark
            // names are correct in it and do not rename them.
            if (IsWithinImportingBuildingBlock(srcBookmarkStart))
                return;

            string srcBookmarkName = srcBookmarkStart.Name;

            if (BookmarkNames.Contains(srcBookmarkName))
            {
                if (srcBookmarkStart.Name == GoBackBookmark)
                    mGoBackBookmarkStarts.Add(dstBookmarkStart);

                string newName = GetNewBookmarkName(srcBookmarkName);
                BookmarkNames.Add(newName);

                // We reset new name of bookmark if encounter bookmark start with the same name without end,
                // this looks like nested bookmarks with the same name. In Ms Word we cannot create such document,
                // only using DocumentBuilder. Anyways this is invalid situation, but we need to avoid exception.
                mBookmarkNamesTranslationTable[srcBookmarkName] = newName;

                dstBookmarkStart.SetNameInternal(newName);
            }
            else
            {
                BookmarkNames.Add(srcBookmarkName);
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified node is within an importing building block.
        /// </summary>
        private bool IsWithinImportingBuildingBlock(Node sourceNode)
        {
            if (!(sourceNode.Document is GlossaryDocument))
                return false;

            BuildingBlock buildingBlock = (BuildingBlock)sourceNode.GetAncestor(NodeType.BuildingBlock);
            if (buildingBlock == null)
                return false;

            return
                !mContext.TopmostNode.IsAncestorNode(buildingBlock) &&
                ((mContext.TopmostNode != buildingBlock) || mIsTopmostNodeIncluded);
        }

        /// <summary>
        /// Renames bookmark end being imported if corresponding start was renamed.
        /// </summary>
        private void UpdateBookmarkNameIfNeeded(BookmarkEnd srcBookmarkEnd, BookmarkEnd dstBookmarkEnd)
        {
            string newName = mBookmarkNamesTranslationTable.GetValueOrNull(srcBookmarkEnd.Name);
            if (newName != null)
            {
                if (srcBookmarkEnd.Name == GoBackBookmark)
                    mGoBackBookmarkEnds.Add(dstBookmarkEnd);

                dstBookmarkEnd.SetNameInternal(newName);
                mBookmarkNamesTranslationTable.Remove(srcBookmarkEnd.Name);
            }
        }

        /// <summary>
        /// Returns new bookmark name which is unique in destination document.
        /// </summary>
        private string GetNewBookmarkName(string name)
        {
            string newName;

            for (int i = 0; /* infinite loop */; i++)
            {
                newName = string.Format("{0}_{1}", name, i);
                if (!BookmarkNames.Contains(newName))
                    break;
            }

            return newName;
        }

        /// <summary>
        /// Imports style of the specified row.
        /// </summary>
        private Style ImportStyle(Row row)
        {
            int istd = (int)row.TablePr.FetchAttr(TableAttr.Istd);

            Style srcStyle = mContext.SrcStyles.FetchByIstd(istd, StyleIndex.TableNormal);
            Style dstStyle = mContext.DstStyles.ImportStyle(mContext, srcStyle);

            return dstStyle;
        }

        /// <summary>
        /// Imports style specified in <paramref name="srcAttrs"/> and sets it to the <paramref name="dstAttrs"/>.
        /// It ignores mode in <see cref="mContext"/> and replaces KeepSourceFormatting mode with the
        /// UseDestinationStyles mode during import.
        /// </summary>
        private void ImportStyleUseDestinationMode(AttrCollection srcAttrs, AttrCollection dstAttrs, int istdKey)
        {
            int srcIstd = (int)srcAttrs.FetchAttr(istdKey);
            Style srcStyle = mContext.SrcStyles.GetByIstd(srcIstd, false);

            if ((srcStyle != null) && !StyleIndex.IsNonModifiable(srcStyle))
            {
                ImportFormatMode originalFormatMode = mContext.ImportFormatMode;
                // Force to import with UseDestinationStyles mode.
                // WORDSNET-19268 The new option is introduced that allows to copy styles in KeepSourceFormatting mode.
                // In this case we should not force UseDestinationStyles mode.
                if ((mContext.ImportFormatMode == ImportFormatMode.KeepSourceFormatting) &&
                    !mContext.ImportFormatOptions.ForceCopyStyles)
                {
                    mContext.ImportFormatMode = ImportFormatMode.UseDestinationStyles;
                }

                Style dstStyle = mContext.DstStyles.ImportStyle(mContext, srcStyle);

                // No need to set istd explicitly for Normal as it is topmost base style.
                // WORDSNET-21101 Should check destination style identifier instead of source one.
                if (dstStyle.StyleIdentifier == StyleIdentifier.Normal)
                    dstAttrs.Remove(istdKey);
                else
                    dstAttrs.SetAttr(istdKey, dstStyle.Istd);

                mContext.ImportFormatMode = originalFormatMode;
            }
            else
            {
                // No need to set istd explicitly for non-modifiable styles as they are topmost base styles.
                dstAttrs.Remove(istdKey);
            }
        }

        /// <summary>
        /// Imports list specified in <paramref name="srcParaPr"/> and applies it to <paramref name="dstParaPr"/> using current import context.
        /// </summary>
        private void ImportList(ParaPr srcParaPr, ParaPr dstParaPr)
        {
            ImportList(srcParaPr, dstParaPr, mContext);
        }

        /// <summary>
        /// Creates missing in destination styles paragraph BuiltIn style
        /// for both: original and revised properties.
        /// </summary>
        private void CreateMissingBuiltInParagraphStyles(Paragraph paragraph)
            {
            // Paragraph style from the original revision.
            Style style = paragraph.GetParagraphStyle(RevisionsView.Original);
            CreateMissedBuiltInStyle(style);

            // Paragraph style from the final revision.
            style = paragraph.GetParagraphStyle(RevisionsView.Final);
            CreateMissedBuiltInStyle(style);
        }

        /// <summary>
        /// Creates missed built-in style in <see cref="ImportContext.DstStyles"/> collection.
        /// </summary>
        private void CreateMissedBuiltInStyle(Style builtinStyle)
        {
            if (!builtinStyle.BuiltIn)
                return;

            StyleCollection dstStyles = mContext.DstStyles;
            Style existingStyle = dstStyles.FindLocaleIndependentMatch(builtinStyle);
            if (existingStyle != null)
                return;

            Style builtInStyle = dstStyles.BuiltInStyles.GetBySti(builtinStyle.StyleIdentifier, false);
            dstStyles.AddCopy(builtInStyle);
        }

        /// <summary>
        /// Returns ListDefId that cannot be reused during importing or
        /// <see cref="long.MaxValue"/> if there is no such Id.
        /// </summary>
        private long GetNonReusableListDefId(Paragraph curPara)
        {
            Debug.Assert(curPara != null);

            if (mContext.ImportFormatOptions.UseExistingLists)
                return long.MaxValue;

            Paragraph prevPara = GetPrevNonEmptyPara(curPara);
            if (prevPara != null)
            {
                int curParaIndent = GetEffectiveParaIndent(curPara);
                int prevParaIndent = GetEffectiveParaIndent(prevPara);

                // Word does not reuse list definition of very first list in the
                // importing scope of nodes, when indention of the previous not empty
                // paragraph less then indention of the first list item.
                if (prevParaIndent < curParaIndent)
                    return curPara.ListFormat.List.ListDefId;
            }

            return long.MaxValue;
        }

        /// <summary>
        /// Calculates total indention for specified paragraph.
        /// </summary>
        private static int GetEffectiveParaIndent(IParaAttrSource para)
        {
            Debug.Assert(para != null);

            int leftIndent = (int)para.FetchParaAttr(ParaAttr.LeftIndent);
            int firstLineIndent = (int)para.FetchParaAttr(ParaAttr.FirstLineIndent);

            return leftIndent + firstLineIndent;
        }

        /// <summary>
        /// Returns previous paragraph that has non-empty run or any other non-
        /// cross-structure annotation node (<see cref="NodeUtil.IsCrossStructureAnnotation(Node)"/>).
        /// </summary>
        private static Paragraph GetPrevNonEmptyPara(Paragraph curPara)
        {
            Node node = curPara.PreviousSibling;
            while (node != null)
            {
                if (node.NodeType == NodeType.Paragraph)
                {
                    Paragraph para = (Paragraph)node;

                    if (!IsContainsOnlyEmptyRunsOrCrossAnnotation(para))
                        return para;
                }

                node = node.PreviousSibling;
            }

            return null;
        }

        /// <summary>
        /// Returns true, if the specified <paramref name="para"/> contains
        /// only empty runs or bookmark and comment range nodes.
        /// </summary>
        private static bool IsContainsOnlyEmptyRunsOrCrossAnnotation(Paragraph para)
        {
            foreach (Node node in para.GetChildNodes(NodeType.Any, false))
            {
                if (!NodeUtil.IsCrossStructureAnnotation(node) && (node.NodeType != NodeType.Run))
                    return false;

                if (node.NodeType == NodeType.Run)
                {
                    Run run = (Run)node;
                    if (StringUtil.HasChars(run.Text))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns true when formatting difference in source and destination documents can be applied to the specified node.
        /// </summary>
        private bool CanApplyFormattingDifference(IInline inline)
        {
            return CanApplyFormattingDifference(inline.ParentParagraph_IInline);
        }

        /// <summary>
        /// Returns true when formatting difference in source and destination documents can be applied to the specified node.
        /// </summary>
        private bool CanApplyFormattingDifference(Paragraph para)
        {
            if (para == null)
                return false;

            // According to Word behavior, the attributes cannot be applied when:
            // Paragraph is inside glossary,
            if (para.Document is GlossaryDocument)
                return false;

            Story parentStory = para.ParentStory;

            // Paragraph is not inside Body (only when importing entire document),
            // WORDSNET-19796 OR when importing entire sections as well.
            bool isDocumentLevelImport = (mContext.TopmostNode.NodeType == NodeType.Document) || (mContext.TopmostNode.NodeType == NodeType.Section);

            // WORDSNET-20376 Word behavior is different for header/footers and body.
            // Behavior is not to apply formatting difference for headers and footers
            // the IgnoreHeaderFooter option has been added to control that.
            // True is default, this mimics Word behavior and do not apply difference.
            // In other words, when entire document or section imported, copy source formatting depending on import option.
            bool isHeaderFooter = (parentStory != null) && (parentStory.StoryType != StoryType.MainText);

            if (isDocumentLevelImport && isHeaderFooter && mContext.ImportFormatOptions.IgnoreHeaderFooter)
                return false;

            Node parentNode = para.ParentNode;

            // When importing nodes from the nodes of the following types along with these nodes itself,
            // we should not apply formatting difference.
            if ((parentNode != null) && (NodeUtil.IsAncestorOrSelf(parentNode, mContext.TopmostNode)))
            {
                // Paragraph is inside Footnote,
                if (parentNode.NodeType == NodeType.Footnote)
                    return false;

                // Paragraph is inside Comment,
                if (parentNode.NodeType == NodeType.Comment)
                    return false;

                // Paragraph is inside Shape.
                // WORDSNET-18202 Ignore text boxes on option.
                if (para.IsInShape && IsIgnoreParaInTextBox(para))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether a paragraph should be ignored in the text box,
        /// given the <see cref = "ImportFormatOptions.IgnoreTextBoxes" /> option.
        /// </summary>
        private bool IsIgnoreParaInTextBox(Paragraph para)
        {
            if (mContext.ImportFormatOptions.IgnoreTextBoxes)
                return true;

            Shape shape = para.FirstNonMarkupParentNode as Shape;
            if (shape == null)
                return true;

            return !shape.HasTextbox;
        }

        /// <summary>
        /// Replaces non-ascii font names with their ascii alternates from a specified 'fontInfos'.
        /// </summary>
        private static void ReplaceWithAsciiFontNames(RunPr runPr, FontInfoCollection fontInfos)
        {
            ReplaceWithAsciiFontName(runPr, FontAttr.NameAscii, fontInfos);
            ReplaceWithAsciiFontName(runPr, FontAttr.NameFarEast, fontInfos);
            ReplaceWithAsciiFontName(runPr, FontAttr.NameBi, fontInfos);
            ReplaceWithAsciiFontName(runPr, FontAttr.NameOther, fontInfos);
        }

        /// <summary>
        /// Replaces non-ascii font name with its ascii alternate from a specified 'fontInfos'.
        /// </summary>
        private static void ReplaceWithAsciiFontName(RunPr runPr, int key, FontInfoCollection fontInfos)
        {
            ComplexFontName fontName = runPr[key] as ComplexFontName;
            if ((fontName == null) || fontName.IsThemeFont || StringUtil.ContainsOnlyAscii(fontName.Name))
                return;

            foreach (FontInfo fontInfo in fontInfos)
            {
                // WORDSNET-25387 Check font alternative name is not empty.
                if ((fontName.Name == fontInfo.Name) &&
                    (fontInfo.AltName.Length != 0) && StringUtil.ContainsOnlyAscii(fontInfo.AltName))
                {
                    runPr[key] = ComplexFontName.FromName(fontInfo.AltName);
                    break;
                }
            }
        }

        /// <summary>
        /// Returns true, if source styles can be preserved when import using KeepSourceFormatting mode.
        /// </summary>
        /// <remarks>
        /// Word tries to preserve styles on the imported nodes. It checks all used inside specified node and all its child
        /// nodes styles for their equality in source and destination documents. If at least one of the styles is different,
        /// then Word expands all importing styles.
        /// </remarks>
        private bool CanPreserveSourceStyles(Node node)
        {
            StyleCollection srcStyles = mContext.SrcStyles;
            StyleCollection dstStyles = mContext.DstStyles;

            // Save original defaults and clone them to prepare for comparison.
            RunPr srcDefaultRunPrSaved = srcStyles.DefaultRunPr;
            srcStyles.DefaultRunPr = srcStyles.DefaultRunPr.Clone();
            RunPr dstDefaultRunPrSaved = dstStyles.DefaultRunPr;
            dstStyles.DefaultRunPr = dstStyles.DefaultRunPr.Clone();

            Theme srcTheme = mContext.SrcDoc.GetThemeInternal();
            Theme dstTheme = mContext.DstDoc.GetThemeInternal();

            // WORDSNET-23703 Resolve themes in defaults before comparison.
            Theme.Apply(srcTheme, srcStyles.DefaultRunPr);
            Theme.Apply(dstTheme, dstStyles.DefaultRunPr);

            // WORDSNET-25032 Bring font names to Ascii strings before comparison.
            ReplaceWithAsciiFontNames(srcStyles.DefaultRunPr, mContext.SrcDoc.FontInfos);
            ReplaceWithAsciiFontNames(dstStyles.DefaultRunPr, mContext.DstDoc.FontInfos);

            try
            {
                HashSetGeneric<int> srcUsedStyles = GetUsedStyles(node);
                foreach (int srcIstd in srcUsedStyles)
                {
                    Style srcStyle = srcStyles.GetByIstd(srcIstd, false);
                    if ((srcStyle == null) || StyleIndex.IsNonModifiable(srcStyle))
                        continue;

                    Style dstStyle = dstStyles.FindLocaleIndependentMatch(srcStyle);
                    if (dstStyle != null)
                    {
                        // If documents defaults are equal, then Word just checks attributes equality of all used styles.
                        // Otherwise, it also tries to check if paragraph style overrides defaults
                        // by checking its expanded attributes.
                        RunPrExpandFlags runPrExpandFlags = RunPrExpandFlags.GlobalDefaults;
                        ParaPrExpandFlags paraPrExpandFlags = ParaPrExpandFlags.GlobalDefaults;
                        // WORDSNET-27457 This case shows, that defaults should be considered even
                        // when they are equal, at least for paragraph styles.
                        if (srcStyle.Type == StyleType.Paragraph)
                        {
                            runPrExpandFlags |= RunPrExpandFlags.DocumentDefaults;
                            paraPrExpandFlags |= ParaPrExpandFlags.DocumentDefaults;
                        }

                        RunPr srcExpandedRunPr = srcStyle.GetExpandedRunPr(runPrExpandFlags);
                        RunPr dstExpandedRunPr = dstStyle.GetExpandedRunPr(runPrExpandFlags);

                        // WORDSNET-23703 Apply themes before comparison.
                        Theme.Apply(srcTheme, srcExpandedRunPr);
                        Theme.Apply(dstTheme, dstExpandedRunPr);
                        if (!srcExpandedRunPr.Equals(dstExpandedRunPr, Style.ComparisonIgnorableKeys))
                            return false;

                        if (srcStyle.Type != StyleType.Character)
                        {
                            ParaPr srcExpandedParaPr = srcStyle.GetExpandedParaPr(paraPrExpandFlags);
                            ParaPr dstExpandedParaPr = dstStyle.GetExpandedParaPr(paraPrExpandFlags);
                            if (!srcExpandedParaPr.Equals(dstExpandedParaPr, Style.ComparisonIgnorableKeys))
                                return false;

                            // WORDSNET-19316 Check also lists formatting of the styles.
                            List srcList = srcStyle.GetListInternal();
                            List dstList = dstStyle.GetListInternal();

                            if (!AreListsEqual(srcList, dstList))
                                return false;
                        }
                    }
                    else
                    {
                        // Word does not preserve original styles if some of used built-in linked styles
                        // did not exist in destination collection originally.
                        if (srcStyle.BuiltIn && (srcStyle.LinkedIstd != StyleIndex.Nil))
                            return false;
                    }
                }
            }
            finally
            {
                // Restore original defaults.
                srcStyles.DefaultRunPr = srcDefaultRunPrSaved;
                dstStyles.DefaultRunPr = dstDefaultRunPrSaved;
            }

            return true;
        }

        /// <summary>
        /// Returns true if a specified lists are equal or can be reused in a current importing context.
        /// </summary>
        private bool AreListsEqual(List srcList, List dstList)
        {
            if (ArgumentUtil.OneIsNull(srcList, dstList))
                return false;

            if (ArgumentUtil.BothAreNull(srcList, dstList))
                return true;

            // WORDSNET-19275 Compare list formatting only for list with the same ids.
            if (srcList.ListId != dstList.ListId && !mContext.ImportFormatOptions.KeepSourceNumbering)
               return true;

            // First check, if a list definition can be reused.
            ListDef dstListDef = mContext.DstLists.GetListDefByListDefId(srcList.ListDef.ListDefId);
            if ((dstListDef != null) && mContext.CanReuseListDefId(dstListDef.ListDefId))
                return true;

            return srcList.Equals(dstList);
        }

        /// <summary>
        /// Returns used styles.
        /// </summary>
        private static HashSetGeneric<int> GetUsedStyles(Node node)
        {
            HashSetGeneric<int> usedStyleIds = new HashSetGeneric<int>();
            CollectUsedStyles(node, usedStyleIds);

            // When importing inline node without parent paragraph,
            // we always need also Normal style.
            if (node is IInline)
                usedStyleIds.Add(StyleIndex.Normal);

            return usedStyleIds;
        }

        /// <summary>
        /// Collects Istd used in this and all child nodes.
        /// </summary>
        private static void CollectUsedStyles(Node node, HashSetGeneric<int> usedStyleIds)
        {
            if (node.IsComposite)
                foreach (Node childNode in (CompositeNode)node)
                    CollectUsedStyles(childNode, usedStyleIds);

            IInline inlineNode = node as IInline;
            if (inlineNode != null)
                AddStyle(usedStyleIds, inlineNode.RunPr_IInline, FontAttr.Istd);
            else if (node.NodeType == NodeType.Paragraph)
                AddStyle(usedStyleIds, ((Paragraph)node).ParaPr, ParaAttr.Istd);
        }

        /// <summary>
        /// Adds original and final istds from the specified collection to the hashSet.
        /// </summary>
        private static void AddStyle(HashSetGeneric<int> hashSet, WordAttrCollection attrs, int istdKey)
        {
            hashSet.Add((int)attrs.FetchAttr(istdKey, RevisionsView.Original));
            hashSet.Add((int)attrs.FetchAttr(istdKey, RevisionsView.Final));
        }

        /// <summary>
        /// Gets custom XML parts from the specified collection that contain the XML nodes
        /// that match the specified XmlMapping.
        /// </summary>
        private static List<CustomXmlPart> GetXmlPartsWithMatchingXmlMapping(
            CustomXmlPartCollection dstCollection, XmlMapping xmlMapping)
        {
            List<CustomXmlPart> xmlParts = new List<CustomXmlPart>();
            foreach (CustomXmlPart xmlPart in dstCollection)
            {
                if (CustomXmlUtil.ExtractXmlNodes(
                    xmlPart.Data, xmlMapping.XPath, xmlMapping.PrefixMappings, null, null).Count > 0)
                {
                    xmlParts.Add(xmlPart);
                }
            }
            return xmlParts;
        }

        /// <summary>
        /// Returns true if the specified custom XML part with the specified XmlMapping contains XML in FlatOpc format.
        /// </summary>
        private static bool ContainsFlatOpc(CustomXmlPart xmlPart, XmlMapping xmlMapping)
        {
            IList<XmlNode> xmlNodes =
                CustomXmlUtil.ExtractXmlNodes(xmlPart.Data, xmlMapping.XPath, xmlMapping.PrefixMappings, null, null);
            if (xmlNodes.Count > 0)
            {
                string newContent = CustomXmlUtil.GetOverallInnerText(xmlNodes);
                if (StringUtil.HasChars(newContent) && newContent.Contains("<pkg:package"))
                {
                    using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(newContent)))
                    {
                        LoadFormat lf = FileFormatUtil.DetectFileFormat(stream).LoadFormat;
                        return ((lf != LoadFormat.Unknown) && (lf != LoadFormat.Text));
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Appends source citations to the destination citations.
        /// </summary>
        private static void AppendCitations(CustomXmlPart dstCitations, CustomXmlPart srcCitations)
        {
            using (MemoryStream dstStream = new MemoryStream(dstCitations.Data))
            {
                XmlDocument dstXml = XmlUtilPal.LoadXml(dstStream, true);
                if ((dstXml.DocumentElement == null) ||
                    (dstXml.DocumentElement.Name != CustomXmlPartCollection.CitationSourcesElementName))
                    return;

                using (MemoryStream srcStream = new MemoryStream(srcCitations.Data))
                {
                    XmlDocument srcXml = XmlUtilPal.LoadXml(srcStream, true);
                    if ((srcXml.DocumentElement == null) ||
                        (srcXml.DocumentElement.Name != CustomXmlPartCollection.CitationSourcesElementName))
                        return;

                    for (int i = 0; i < srcXml.DocumentElement.ChildNodes.Count; i++)
                    {
                        XmlNode srcNode = srcXml.DocumentElement.ChildNodes[i];
                        XmlNode dstNode = dstXml.ImportNode(srcNode, true);
                        dstXml.DocumentElement.AppendChild(dstNode);
                    }

                    MemoryStream dstData = new MemoryStream();
                    XmlUtilPal.SaveXml(dstXml, dstData);
                    dstCitations.Data = StreamUtil.CopyStreamToByteArray(dstData);
                }
            }
        }

        /// <summary>
        /// Returns true, if a specified <see cref="CustomXmlPart"/> is empty Citation collection.
        /// </summary>
        private static bool IsEmptyCitationCollection(CustomXmlPart citations)
        {
            if (citations == null)
                return true;

            using (MemoryStream stream = new MemoryStream(citations.Data))
            {
                XmlDocument xmlDocument = XmlUtilPal.LoadXml(stream, true);
                if ((xmlDocument.DocumentElement == null) ||
                    (xmlDocument.DocumentElement.Name != CustomXmlPartCollection.CitationSourcesElementName))
                    return true;


                return (xmlDocument.DocumentElement.ChildNodes.Count == 0);
            }
        }

        /// <summary>
        /// Returns listId for a specified ParaPr.
        /// If there is no direct listId, then returns inherited one from a specified styles collection.
        /// Returns -1, if not found.
        /// </summary>
        private static int GetListId(ParaPr paraPr, StyleCollection styles)
        {
            object listId = paraPr[ParaAttr.ListId];
            if (listId != null)
                return (int)listId;

            Style style = styles.GetByIstd(paraPr.Istd, false);
            if (style != null)
            {
                listId = style.GetParaAttr(ParaAttr.ListId, RevisionsView.Final);
                if (listId != null)
                    return (int)listId;
            }

            return -1;
        }

        /// <summary>
        /// Gets an underlying importing context.
        /// </summary>
        internal ImportContext Context
        {
            get { return mContext; }
        }

        /// <summary>
        /// Collection of bookmark names in destination document.
        /// </summary>
        private CaseInsensitiveHashSet BookmarkNames
        {
            get
            {
                if(mBookmarkNamesCache == null)
                {
                    // WORDSNET-26114 Compare case insensitive.
                    mBookmarkNamesCache = new CaseInsensitiveHashSet();

                    // WORDSNET-25817 Improved bookmark handling.
                    Dictionary<string, BookmarkEnd> bookmarkEnds = new Dictionary<string, BookmarkEnd>();
                    foreach(BookmarkEnd bookmarkEnd in mContext.DstDoc.GetChildNodes(NodeType.BookmarkEnd, true))
                        bookmarkEnds[bookmarkEnd.Name] = bookmarkEnd;

                    foreach (BookmarkStart bookmarkStart in mContext.DstDoc.GetChildNodes(NodeType.BookmarkStart, true))
                    {
                        // Do not add incomplete bookmarks.
                        if(bookmarkEnds.ContainsKey(bookmarkStart.Name))
                            mBookmarkNamesCache.Add(bookmarkStart.Name);
                    }
                }

                return mBookmarkNamesCache;
            }
        }

        /// <summary>
        /// Gets maximum of the destination ZOrders.
        /// </summary>
        private int MaxDstZOrder
        {
            get
            {
                if (mMaxDstZOrder == int.MinValue)
                {
                    mMaxDstZOrder = 0;
                    NodeCollection shapes = Context.DstDoc.GetChildNodes(new NodeType[] { NodeType.Shape, NodeType.GroupShape }, true);
                    foreach (Node shape in shapes)
                    {
                        int zOrder = ((ShapeBase)shape).ZOrder;
                        if (zOrder > mMaxDstZOrder)
                            mMaxDstZOrder = zOrder;
                    }
                }

                return mMaxDstZOrder;
            }
        }

        private readonly ImportContext mContext;

        /// <summary>
        /// Collects shapes need to be updated after single import operation.
        /// </summary>
        private readonly IntToObjDictionary<ShapeBase> mImportedShapes = new IntToObjDictionary<ShapeBase>();

        /// <summary>
        /// Collects drawingMls with textboxes which need to be updated after single import operation.
        /// </summary>
        private readonly IntToObjDictionary<ShapeBase> mDmlTxbxByTxbxId = new IntToObjDictionary<ShapeBase>();

        /// <summary>
        /// Collects drawingMls with linked textboxes which need to be updated after single import operation.
        /// </summary>
        private readonly IntToObjDictionary<SortedIntegerListGeneric<ShapeBase>> mDmlLinkedTxbxByLnkId =
            new IntToObjDictionary<SortedIntegerListGeneric<ShapeBase>>();

        private CaseInsensitiveHashSet mBookmarkNamesCache;
        private readonly Dictionary<string, string> mBookmarkNamesTranslationTable = new Dictionary<string, string>();
        private readonly List<BookmarkStart> mGoBackBookmarkStarts = new List<BookmarkStart>();
        private readonly List<BookmarkEnd> mGoBackBookmarkEnds = new List<BookmarkEnd>();
        private bool mIsTopmostNodeIncluded;

        private const string GoBackBookmark = "_GoBack";

        /// <summary>
        /// True, when source styles should be expanded into direct node properties.
        /// </summary>
        private bool mIsApplyFormattingDifference;

        private readonly FormattingDifferenceCalculator mFormattingDifferenceCalculator;

        private int mMaxDstZOrder = int.MinValue;
    }
}
