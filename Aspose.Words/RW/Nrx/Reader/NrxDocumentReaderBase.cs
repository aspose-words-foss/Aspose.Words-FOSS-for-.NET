// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/09/2008 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Crypto;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;
using Aspose.Words.Validation;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Base class for reading DOCX or WordML document.
    /// </summary>
    internal abstract class NrxDocumentReaderBase : INrxStyleIdToIstd, IVmlShapeReaderContext
    {
        protected NrxDocumentReaderBase(DocumentBase doc,
            LoadOptions loadOptions,
            WarningSource warningSource,
            OoxmlComplianceInfo complianceInfo,
            NrxStoryReaderBase storyReader,
            NrxSectPrReaderBase sectPrReader)
        {
            Debug.Assert(doc != null);
            Debug.Assert(storyReader != null);
            Debug.Assert(sectPrReader != null);

            mDoc = doc;
            mCurContainer = doc;
            mMarkupResolver = new MarkupResolver(loadOptions, warningSource);
            mLoadOptions = loadOptions;
            mComplianceInfo = complianceInfo;
            mStoryReader = storyReader;
            mSectPrReader = sectPrReader;
            StoryRevisionStack = new StoryRevisionStack(loadOptions.WarningCallback, warningSource);
            // WORDSNET-14959. MS Word versions 2007 - 2019 get LCID from Windows registry.
            if (loadOptions.UseSystemLcid)
                GlobalSectPr.UpdateDefaultsByLcid();
        }

        protected void UpdateZOrders()
        {
            UpdateZOrders(mZOrderListHeader);
            UpdateZOrders(mZOrderListMain);
        }

        /// <summary>
        /// Updates ZOrder of the document shapes because z-index in VML is not same as ZOrder in the model.
        /// Should be called after the document is fully read.
        /// </summary>
        private static void UpdateZOrders(List<ShapeBase> zOrderList)
        {
            if (zOrderList.Count == 0)
                return;

            ShapeZOrderSorter<ShapeBase>.Sort(zOrderList);

            for (int i = 0; i < zOrderList.Count; i++)
            {
                ShapeBase shape = zOrderList[i];
                shape.ZOrder = i;
            }
        }

        /// <summary>
        /// Resolves duplicate values of the <see cref="ShapeBase.Id"/> property of VML shapes.
        /// </summary>
        /// <remarks>
        /// WORDSNET-27369 When a VML shape is read, numeric part of its string ID is parsed and set to the
        /// <see cref="ShapeBase.Id"/> property. However, some two shapes may have different string IDs with the same
        /// numeric part, for example, '_x0000_i1027' and '_x0000_s1027', so these shapes get the same IDs in the model,
        /// which can cause errors in the document. This method assigns unique numeric IDs to shapes.
        /// </remarks>
        protected void ResolveDuplicateShapeIds()
        {
            Dictionary<int, ShapeBase> idMap = new Dictionary<int, ShapeBase>(mShapeOriginalIdsMap.Count);
            List<ShapeBase> duplicateIdShapes = new List<ShapeBase>();

            foreach (ShapeBase shape in mAllShapes)
            {
                if (idMap.ContainsKey(shape.Id))
                    duplicateIdShapes.Add(shape);
                else
                    idMap.Add(shape.Id, shape);
            }

            foreach (ShapeBase duplicateIdShape in duplicateIdShapes)
            {
                SetShapeUniqueId(duplicateIdShape, idMap);
                idMap.Add(duplicateIdShape.Id, duplicateIdShape);
            }
        }

        /// <summary>
        /// Gets an unique shape ID using the specified shape ID map and assigns it to the shape.
        /// </summary>
        private static void SetShapeUniqueId(ShapeBase shape, Dictionary<int, ShapeBase> idMap)
        {
            // Shape ID should be in some particular range to be correctly processed in the document (see the method
            // DocumentBase.HaveSameIdMap).
            int id = shape.Id / DocumentBase.MapDivider * DocumentBase.MapDivider;

            while (idMap.ContainsKey(id))
                id++;

            shape.Id = id;
        }

        /// <summary>
        /// Updates TextboxNextShapeId with Shape.Id used in the model.
        /// andrnosk: WORDSNET-7399 Use temporary TextboxNextShapeIdRaw to get appropriate shape Id.
        /// </summary>
        protected void UpdateTextboxNextShapeIds()
        {
            // Collect link target to avoid shape to be multiple link target.
            HashSetGeneric<int> linkTargetIds = new HashSetGeneric<int>();

            foreach (ShapeBase shape in mAllShapes)
            {
                string nextShapeId = (string)shape.ShapePr[ShapeAttr.Sys_TextboxNextShapeIdRaw];

                if (StringUtil.HasChars(nextShapeId))
                {
                    // Shape refers to itself, do nothing.
                    if (shape.Name == nextShapeId)
                        continue;

                    ShapeBase nextShape = mShapeOriginalIdsMap.GetValueOrNull(nextShapeId);
                    if ((nextShape != null) && !linkTargetIds.Contains(nextShape.Id))
                    {
                        shape.SetShapeAttrInternal(ShapeAttr.TextboxNextShapeId, nextShape.Id);
                        if(shape.Id != nextShape.Id)
                            linkTargetIds.Add(nextShape.Id);
                    }

                    // If no appropriate shape found the link will be broken, i.e no NextShapeId will be written.
                }
            }
        }

        /// <summary>
        /// Calculate and set ShapeId and TextboxNextShapeId in drawingML ShapePr.
        /// Also we have to change these values in fallback shape too.
        /// </summary>
        protected void UpdateDmlNextShapeIds()
        {
            // WORDSNET-13866 Processing textbox identifiers algorithm was changed.
            // Assign unique identifiers and try to link shapes by index.
            DrawingMLIdValidator.ProcessLinkedTxbxId(mDmlTextBoxes, mDmlLinkedTextboxes);

            foreach (SortedIntegerListGeneric<ShapeBase> lnkItems in mDmlLinkedTextboxes.Values)
            {
                // If linked textboxes collection exist then it is mean that collection
                // contains at least one value, so take it.
                Shape firstLnk = (Shape)lnkItems.GetByIndex(0);
                int prevTxbxId = firstLnk.LinkedTextboxId;

                ShapeBase txbx = mDmlTextBoxes[prevTxbxId];
                if (txbx == null)
                   ProcessBrokenTxbxChain(lnkItems, prevTxbxId);
            }
        }

        /// <summary>
        /// Updates shapes with connector rules.
        /// </summary>
        protected void UpdateConnectors()
        {
            foreach (KeyValuePair<string, ConnectorRule> entry in ConnectorRules)
            {
                string idref = entry.Key;
                ShapeBase ruleShape = mShapeOriginalIdsMap.GetValueOrNull(idref.Substring(1));

                // Shape is connector, assign rule to it.
                if (ruleShape != null)
                {
                    ConnectorRule rule = entry.Value;

                    // Rule can be defined for shape names so update them as well.
                    ShapeBase shapeA = mShapeOriginalIdsMap.GetValueOrNull(rule.ShapeAIdRaw.Substring(1));
                    if (shapeA != null)
                        rule.ShapeAId = shapeA.Id;

                    ShapeBase shapeB = mShapeOriginalIdsMap.GetValueOrNull(rule.ShapeBIdRaw.Substring(1));
                    if (shapeB != null)
                        rule.ShapeBId = shapeB.Id;

                    ruleShape.ShapePr.SetAttr(ShapeAttr.ConnectorRule, entry.Value);
                }
            }
        }

        /// <summary>
        /// Resolves stored ID of Click and Type and default table styles and writes them into document properties.
        /// </summary>
        protected void ResolveDocPrStyles()
        {
            if (StringUtil.HasChars(ClickAndTypeStyleId))
            {
                int istd = ResolveStyleIdToIstd(ClickAndTypeStyleId, -1);
                if (istd >= 0)
                    Document.DocPr.ClickTypeParaStyleIstd = istd;
            }
            if (StringUtil.HasChars(DefaultTableStyleId))
            {
                int istd = ResolveStyleIdToIstd(DefaultTableStyleId, -1);
                if (istd >= 0)
                    Document.DocPr.DefaultTableStyleIstd = istd;
            }
        }

        /// <summary>
        /// Try to relate between linked textboxes chain and imported textboxes by index.
        /// </summary>
        /// <param name="lnkItems">Collection of linked textboxes of one chain.</param>
        /// <param name="lnkTxbxId">Linked textbox identifier for passed items.</param>
        private void ProcessBrokenTxbxChain(SortedIntegerListGeneric<ShapeBase> lnkItems, int lnkTxbxId)
        {
            Debug.Assert(lnkItems != null);

            if (mDmlTxbxByIndex.Count > lnkTxbxId)
            {
                Shape txbx = mDmlTxbxByIndex[lnkTxbxId];
                // Identifiers were not changed for current shape, so do it.
                DrawingMLIdValidator.UpdateTxbxId(txbx, lnkItems, Document);
            }
        }

        /// <summary>
        /// If the same binary data is already in the cache, returns the object from the cache.
        /// If this binary data is unique, adds it to the cache and returns it.
        /// </summary>
        internal byte[] GetNonDuplicatedBytes(byte[] binData)
        {
            BytesHash guid = HashUtil.GetSHA512Hash(binData);
            byte[] cachedBinData = mImageBytesByGuidTable.GetValueOrNull(guid);
            if (cachedBinData == null)
            {
                cachedBinData = binData;
                mImageBytesByGuidTable.Add(guid, cachedBinData);
            }

            return cachedBinData;
        }

        /// <summary>
        /// Adds new node to the current container and makes new container current
        /// so further nodes will be added as children of it.
        /// Use this method instead of adding nodes to the model directly.
        /// </summary>
        internal void AddAndPushContainer(CompositeNode newContainer)
        {
            mCurContainer.AppendChildForLoad(newContainer);
            mCurContainer = newContainer;
        }

        /// <summary>
        /// Restores previous container.
        /// </summary>
        internal void PopContainer(NodeType expectedNodeType)
        {
            if (mCurContainer.NodeType != expectedNodeType)
                throw new InvalidOperationException(string.Format("End of node is out of sync. Expected {0} but got {1}.", expectedNodeType, mCurContainer.NodeType));

            // WORDSNET-19626 the problem occurred because we lost inline content at the end of the document
            // if inline content is direct child of body.
            // Fixed by adding this content to the last paragraph of Body once we finished reading Body.
            if ((mCurContainer.NodeType == NodeType.Body) &&
                ((mPendingInlines.Count > 0) || (mPendingCrossStructureAnnotations.Count > 0)))
            {
                EnsureBodyEnd((Body)mCurContainer);
            }

            mCurContainer = mCurContainer.ParentNode;
        }

        /// <summary>
        /// Ensures that the body ends with a paragraph. If not, adds an empty one and flushes the pending inline
        /// content to it.
        /// </summary>
        internal void EnsureBodyEnd(Body body)
        {
            Node lastComposite = body.LastNonAnnotationChild;
            if (lastComposite is Paragraph && (mPendingInlines.Count == 0) && (mPendingCrossStructureAnnotations.Count == 0))
                return;

            AddAndPushContainer(new Paragraph(body.Document));

            FlushPendingInlines();
            FlushPendingCrossStructureAnnotations();

            PopContainer(NodeType.Paragraph);
        }

        /// <summary>
        /// You must call this method when adding a bookmark or a comment start/end to the model.
        /// Such nodes are "cross-structure annotation markers" according to OOXML and can occur
        /// at the inline level, story level and also inside table or row elements, but in AW model
        /// they can only exist inside a paragraph or Office Math at the moment.
        /// This method queues all such markers so they can be added when the next paragraph starts.
        /// </summary>
        internal void AddCrossStructureAnnotation(Node node)
        {
            if (mCurContainer.CanInsert(node))
            {
                AddChild(node);

                // A block level SDT may be started/ended with an annotation, not only with a paragraph.
                if (node.NodeLevel != NodeLevel.Inline)
                {
                    FlushPendingMarkup(node);

                    XmlReader.PossibleSdtEnd = node;
                }
            }
            else
            {
                // WORDSNET-16477 If the node is a bookmark that is displaced by a previous SDT, move it to real position
                // inside the SDT in disabled block bookmarks mode.
                if (!InsertAnnotationDisplacedByPreviousSdt(node))
                    mPendingCrossStructureAnnotations.Add(node);
            }
        }

        /// <summary>
        /// Flushes accumulated annotations.
        /// </summary>
        internal void FlushPendingCrossStructureAnnotations()
        {

            if (mPendingCrossStructureAnnotations.Count == 0)
                return;

            // If current container is paragraph we can flush annotations there,
            // otherwise we try to get a last paragraph inside current container to flush.
            Paragraph para = (mCurContainer.NodeType == NodeType.Paragraph)
                                 ? (Paragraph)mCurContainer
                                 : (Paragraph)mCurContainer.GetChild(NodeType.Paragraph, -1, false);

            FlushPendingCrossStructureAnnotationsCore(para);
        }

        /// <summary>
        /// Flushes accumulated annotations into the specified <paramref name="para"/>.
        /// </summary>
        private void FlushPendingCrossStructureAnnotationsCore(Paragraph para)
        {
            // andrnosk: WORDSNET-5758 We should make sure that there is paragraph
            // where we can flush pending inline content.
            if (para == null)
            {
                para = new Paragraph(Document);
                mCurContainer.AppendChild(para);
            }

            foreach (Node node in mPendingCrossStructureAnnotations)
            {
                ResetDisplacedByIfMovedIntoSdt(node);

                para.AppendChildForLoad(node);
            }

            mPendingCrossStructureAnnotations.Clear();
        }

        /// <summary>
        /// Resets value of the DisplacedBy property of the specified annotation node if the value becomes wrong after
        /// moving the node to inline level.
        /// </summary>
        private void ResetDisplacedByIfMovedIntoSdt(Node annotationNode)
        {
            // Resetting is needed:
            // 1) if an annotation with DisplacedBy == DisplacedByType.Next that is located before its SDT is moved
            // into the SDT;
            // 2) if an annotation with DisplacedBy == DisplacedByType.Prev that is direct child of its SDT is moved
            // to inline level.
            // Some cases are processed by FlashDisplacedCrossStructureAnnotationsOnSdtEnd and
            // InsertBookmarkDisplacedByPreviousSdt.

            IDisplaceableByCustomXml displaceableAnnotationNode = annotationNode as IDisplaceableByCustomXml;

            if (displaceableAnnotationNode == null || !mMarkupResolver.IsInStartingMarkup)
                return;

            displaceableAnnotationNode.DisplacedByCustomXml = DisplacedByType.Unspecified;
        }

        /// <summary>
        /// Appends inline level node to pending list to be flushed at first appropriate container.
        /// </summary>
        internal void AddPendingInline(Node inline, Node ancestor)
        {
            if (ancestor == null)
                ancestor = mCurContainer;

            Node story = NodeUtil.IsStoryNodeType(ancestor) ? ancestor : ancestor.GetStoryAncestor(NodeType.Any);
            InlineInfo info = new InlineInfo(inline, story);

            mPendingInlines.Add(info);
        }

        /// <summary>
        /// Appends block level node to pending list to be flushed at the specified container or a story.
        /// </summary>
        internal void AddPendingBlock(Node node, CompositeNode pendingBlockDestination)
        {
            // The pending blocks are block nodes that are unexpectedly located within another block node in the document
            // being read. pendingBlockDestination is expected to be a correct parent for the pending blocks, to which
            // the pending blocks will be appended at the appropriate moment.
            // Currently we expect that there can be only one nesting level of such incorrectly defined block nodes.
            Debug.Assert((mPendingBlockDestination == null) || (mPendingBlockDestination == pendingBlockDestination));
            mPendingBlockDestination = pendingBlockDestination;

            mPendingBlocks.Add(node);
        }

        /// <summary>
        /// Appends paragraph to the collection which will be flushed at first appropriate container.
        /// </summary>
        internal void AddPendingParagraph(Paragraph para)
        {
            mPendingParagraphs.Add(para);
        }

        /// <summary>
        /// Flushes pending paragraphs nodes to specified container.
        /// </summary>
        /// <param name="container">Container to flush.</param>
        internal void FlushPendingParagraphs(CompositeNode container)
        {
            if ((container == null) ||
                (mPendingParagraphs.Count == 0) ||
                ((container.NodeType != NodeType.Row) && (container.NodeType != NodeType.Cell)))
            {
                return;
            }

            Cell cellToFlush = (container.NodeType == NodeType.Cell) ? (Cell)container : new Cell(container.Document);

            foreach (Paragraph para in mPendingParagraphs)
                cellToFlush.PrependChild(para);

            if (cellToFlush.ParentNode == null)
                container.AppendChild(cellToFlush);

            mPendingParagraphs.Clear();
        }

        /// <summary>
        /// Flushes pending inline level nodes to current paragraph.
        /// </summary>
        internal void FlushPendingInlines()
        {
            Paragraph para = mCurContainer as Paragraph;

            if (para == null)
                return;

            // Return if the current container is a temporary paragraph.
            Node story = para.GetStoryAncestor(NodeType.Any);
            if (story == null)
                return;

            // WORDSNET-22783 Flush only inlines that can be inserted into current paragraph.
            for (int i = 0; i < mPendingInlines.Count; /* updated within loop */)
            {
                Node node = mPendingInlines[i].Inline;
                if ((mPendingInlines[i].Story == story) && para.CanInsert(node))
                {
                    para.AppendChild(node);
                    mPendingInlines.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        /// <summary>
        /// Flushes pending block level nodes to the stored destination node or to the current story.
        /// </summary>
        internal void FlushPendingBlocks()
        {
            if (mPendingBlocks.Count == 0)
                return;

            if ((mCurContainer != mPendingBlockDestination) && !(mCurContainer is Story))
                return;

            Paragraph lastPara = (Paragraph)mCurContainer.GetChild(NodeType.Paragraph, -1, false);

            // It seems that Word inserts content of first paragraph into last paragraph and copy formatting.
            if ((lastPara != null) && (((Node)mPendingBlocks[0])).NodeType == NodeType.Paragraph)
            {
                Paragraph pendingPara = (Paragraph)mPendingBlocks[0];
                while (pendingPara.HasChildNodes)
                    lastPara.AppendChild(pendingPara.FirstChild);

                pendingPara.ParaPr.ExpandTo(lastPara.ParaPr);
                pendingPara.ParagraphBreakRunPr.ExpandTo(lastPara.ParagraphBreakRunPr);
                mPendingBlocks.RemoveAt(0);
            }

            // Other paragraphs is just appended.
            foreach (Node node in mPendingBlocks)
                mCurContainer.AppendChild(node);

            mPendingBlocks.Clear();
            mPendingBlockDestination = null;
        }

        /// <summary>
        /// Inserts header/footer into section. If header/footer of this type exists then merges them together.
        /// </summary>
        internal void InsertHeaderFooter(Section section, HeaderFooter headerFooter)
        {
            // andrnosk: WORDSNET-5307 There are several header/footer of the same type,
            // we need to concatenate them like MS Word does.
            if (section.HeadersFooters[headerFooter.HeaderFooterType] == null)
            {
                // Insert the new header footer only if the section does not have a header footer of the same type already.
                InsertChildBefore(section, headerFooter, section.Body);
            }
            else
            {
                // Get existing HeaderFooter of the same type.
                HeaderFooter existingHeaderFooter = section.HeadersFooters[headerFooter.HeaderFooterType];

                // Add empty paragraph like MS Word does.
                Paragraph paragraph = new Paragraph(existingHeaderFooter.Document);
                paragraph.ParaPr[ParaAttr.Istd] = section.Document.Styles.GetBySti(StyleIdentifier.Normal, true).Istd;

                // WORDSNET-24181 Insert empty paragraph before a first child (not a paragraph due to it may be null).
                existingHeaderFooter.InsertBefore(paragraph, existingHeaderFooter.FirstChild);

                // Insert nodes of current HeaderFooter, before nodes of the exiting HeaderFooter of the same type.
                while (headerFooter.HasChildNodes)
                    existingHeaderFooter.InsertBefore(headerFooter.LastChild, existingHeaderFooter.FirstChild);
            }
        }

        /// <summary>
        /// Adds a node to the current container.
        /// Use this method instead of adding nodes to the model directly.
        /// </summary>
        internal void AddChild(Node node)
        {
            InsertChildAfter(mCurContainer, node, mCurContainer.LastChild);
        }

        /// <summary>
        /// Inserts a node before the reference specified node.
        /// Use this method instead of adding nodes to the model directly.
        /// </summary>
        internal void InsertChildBefore(CompositeNode parent, Node newChild, Node refChild)
        {
            InsertChildCore(parent, newChild, refChild, false);
        }

        /// <summary>
        /// Inserts a node after the reference node.
        /// Use this method instead of adding nodes to the model directly.
        /// </summary>
        internal void InsertChildAfter(CompositeNode parent, Node newChild, Node refChild)
        {
            InsertChildCore(parent, newChild, refChild, true);
        }

        private void InsertChildCore(CompositeNode parent, Node newChild, Node refChild, bool isAfter)
        {
            // Add the new node to the tree.
            if (isAfter)
            {
                // RK This is a speed optimization. Normally during load, we should call
                // AppendChildForLoad due to speed, but when we have to insert in some other place,
                // we have to use normal (slow) insert methods.
                if (refChild == parent.LastChild)
                    parent.AppendChildForLoad(newChild);
                else
                    parent.InsertAfter(newChild, refChild);
            }
            else
            {
                // RK This is quite slow, but not used often. Only for synthesized field nodes.
                parent.InsertBefore(newChild, refChild);
            }

            // Apply any current delete or insert revision.
            IInline inline = newChild as IInline;
            if (inline != null)
            {
                RunPr runPr = inline.RunPr_IInline;

                StoryRevisionStack.Apply(runPr, newChild);
            }
        }

        /// <summary>
        /// INrxStyleIdToIstd.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ResolveStyleIdToIstd(object styleId, int defaultIstd)
        {
            return mStyleIdToIstd.GetValueOrDefault(styleId, defaultIstd);
        }

        /// <summary>
        /// INrxStyleIdToIstd.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddStyleIdToIstdMapping(object styleId, int istd)
        {
            mStyleIdToIstd[styleId] = istd;
        }

        /// <summary>
        /// IVmlShapeReaderContext
        /// The document object that we read into.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DocumentBase Document
        {
            get { return mDoc; }
        }

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract NrxXmlReader XmlReader
        {
            get;
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        int IVmlShapeReaderContext.GroupNestingLevel { get; set; }

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaThrows(true)]  // IO Exceptions
        public abstract IEmbeddedObject GetEmbeddedObject(string id);

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaThrows(true)]  // IO Exceptions
        public abstract byte[] GetBinData(string src);

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract string GetRelationshipTarget(string relId);

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract bool IsExternalImage(string src);

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddToZOrderList(ShapeBase shape)
        {
            if (mIsInHeaderFooter)
                mZOrderListHeader.Add(shape);
            else
                mZOrderListMain.Add(shape);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IVmlShapeReaderContext.AddToShapeMap(string shapeId, ShapeBase shape)
        {
            mShapeOriginalIdsMap[shapeId] = shape;

            // Collect all shapes in order to iterate through them regardless duplicate shape names.
            mAllShapes.Add(shape);
        }

        internal void AddToDmlTextboxesMap(ShapeBase dml)
        {
            if (dml == null)
                return;

            if(dml.DmlNode.DmlNodeType != DmlNodeType.WordprocessingShape)
                return;

            // Some textbox does not contain "TextboxId".
            Shape shape = (Shape)dml;
            if ((shape.TextBox != null) && (dml.LinkedTextboxId == 0))
                mDmlTxbxByIndex[mDmlTxbxByIndex.Count] = shape;

            if (dml.TextboxId > 0)
                mDmlTextBoxes[dml.TextboxId] = dml;

            if (dml.LinkedTextboxId > 0)
            {
                if (!mDmlLinkedTextboxes.ContainsKey(dml.LinkedTextboxId))
                    mDmlLinkedTextboxes[dml.LinkedTextboxId] = new SortedIntegerListGeneric<ShapeBase>();

                SortedIntegerListGeneric<ShapeBase> linkedTextboxes = mDmlLinkedTextboxes.GetValueOrNull(dml.LinkedTextboxId);
                linkedTextboxes[dml.LinkedTextboxSeq] = dml;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        ShapeBase IVmlShapeReaderContext.GetShapeById(string shapeId)
        {
            return mShapeOriginalIdsMap.GetValueOrNull(shapeId);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IVmlShapeReaderContext.AddShapeType(string id, ShapePr shapePr)
        {
            if(StringUtil.HasChars(id))
                mShapeTemplates[id] = shapePr;
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        ShapePr IVmlShapeReaderContext.GetShapeTypePr(string id)
        {
            if (id.StartsWith("#", StringComparison.Ordinal))
                id = id.Substring(1);

            return (StringUtil.HasChars(id) && mShapeTemplates.ContainsKey(id))
                ? mShapeTemplates[id]
                : null;
        }

        void IVmlShapeReaderContext.MarkMissingSource(Shape shape, string src)
        {
            mMissingShapeSources.Add(shape, src);
        }

        string IVmlShapeReaderContext.GetMissingSource(Shape shape)
        {
            string src;
            mMissingShapeSources.TryGetValue(shape, out src);
            return src;
        }

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaThrows(true)]  // IO Exceptions
        public abstract void ReadBinData();

        /// <summary>
        /// IVmlShapeReaderContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaThrows(true)]  // IO Exceptions
        public abstract bool ReadTextboxContent(ShapeBase shape);

        /// <summary>
        /// Needed for WML only.
        /// </summary>
        internal abstract bool IsSmartTag
        {
            get;
        }

        internal abstract bool IsCustomXmlTag
        {
            get;
        }

        /// <summary>
        /// Needed for DOCX only.
        /// Gets next not-used footnote or endnote by an ID. Returns null if the node is not found. Raises an exception
        /// if all footnotes with the specified ID are already inserted into a document. Allows correctly process
        /// documents that has several footnotes with same ID: next not-used footnote/endnote is returned.
        /// </summary>
        internal abstract Footnote GetFootnoteById(FootnoteType footnoteType, int id);

        /// <summary>
        /// Needed for DOCX only.
        /// Called when each comment is read from the comments part is read.
        /// The implementation needs to add these to a hashtable to later insert into the document
        /// in the appropriate places.
        /// </summary>
        internal abstract void AddComment(Comment comment);

        /// <summary>
        /// Needed for DOCX only.
        /// Gets a comment by an id. Returns null if the node is not found.
        /// </summary>
        internal abstract Comment GetCommentById(int id);

        internal abstract bool IsDocx { get; }

        internal CompositeNode CurContainer
        {
            get { return mCurContainer; }
            set { mCurContainer = value; }
        }

        internal bool IsInHeaderFooter
        {
            set { mIsInHeaderFooter = value; }
        }

        /// <summary>
        /// Gets dictionary used for getting bookmark name by its id.
        /// AW model does not deal with ids, bookmark start/end nodes are identified by name.
        /// In WordML bookmark start elements have both 'w:name' and 'aml:id' attributes set.
        /// Bookmark end elements have no 'w:name' attribute, only 'aml:id'.
        /// So we need to memorize (id, name) pairs when reading bookmark start elements
        /// and use this information later to retrieve names for bookmark end elements.
        /// </summary>
        internal IDictionary<int, string> IdToBookmarkName
        {
            get { return mIdToBookmarkName; }
        }

        internal IDictionary<int, ListDef> AbstractNumIdToListDef
        {
            get { return mAbstractNumIdToListDef; }
        }

        /// <summary>
        /// Indicates that we are currently reading AlternateContent section.
        /// </summary>
        internal bool IsInAlternateContent = false;

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        internal void Warn(WarningType warningType, WarningSource source, string description)
        {
            if (mLoadOptions.WarningCallback != null)
                mLoadOptions.WarningCallback.Warning(new WarningInfo(warningType, source, description));
        }

        /// <summary>
        /// Logs a "UnexpectedContent" warning to the user-provided warning callback.
        /// </summary>
        internal void WarnUnexpected(string description)
        {
            Warn(WarningType.UnexpectedContent, WarningSource.Nrx, description);
        }

        internal void MarkupStart(StructuredDocumentTag sdt)
        {
            if (sdt.Level == MarkupLevel.Inline)
                AddAndPushContainer(sdt);

            mMarkupResolver.PendingStart(sdt);
        }

        internal void FlushPendingMarkup(Node node)
        {
            mMarkupResolver.FlushPendingStarts(node);
        }

        internal void MarkupEnd(StructuredDocumentTag sdt, Node node)
        {
            if (sdt.Level == MarkupLevel.Inline)
                PopContainer(sdt.NodeType);

            mMarkupResolver.ProcessEnd(sdt, node);

            if (sdt.Level != MarkupLevel.Inline)
                FlushDisplacedCrossStructureAnnotationsOnSdtEnd(sdt);
        }

        /// <summary>
        /// Flushes pending annotations, which are children of their displaced SDT, and which DisplacedBy property has
        /// the value <see cref="DisplacedByType.Next"/>, into the last paragraph of the read displaced SDT.
        /// </summary>
        /// <remarks>
        /// Value of the DisplacedBy property is preserved since a bookmark node is in correct position for displacing.
        /// </remarks>
        private void FlushDisplacedCrossStructureAnnotationsOnSdtEnd(StructuredDocumentTag sdt)
        {
            // Collect annotations to flush.
            List<Node> displacedAnnotations = new List<Node>();
            for (int i = 0; i < mPendingCrossStructureAnnotations.Count; i++)
            {
                Node node = mPendingCrossStructureAnnotations[i];

                IDisplaceableByCustomXml displaceableNode = node as IDisplaceableByCustomXml;
                if ((displaceableNode != null) &&
                    (displaceableNode.DisplacedByCustomXml == DisplacedByType.Next))
                    displacedAnnotations.Add(node);
            }

            if (displacedAnnotations.Count == 0)
                return;

            // Get paragraph (last) to flush to.
            CompositeNode composite = GetLastParagraphOfSdt(sdt);

            // Flush.
            if (composite != null)
            {
                foreach (Node node in displacedAnnotations)
                {
                    composite.AppendChild(node);
                    mPendingCrossStructureAnnotations.Remove(node);
                }
            }
        }

        /// <summary>
        /// Gets last paragraph of the specified pending structured document tag.
        /// </summary>
        private CompositeNode GetLastParagraphOfSdt(IMarkupNode sdt)
        {
            List<Node> sdtChildren = mMarkupResolver.GetReadMarkupRange(sdt);
            CompositeNode composite = null;
            for (int i = sdtChildren.Count - 1; i >= 0; i--)
            {
                if (sdtChildren[i].IsComposite)
                {
                    composite = (CompositeNode)sdtChildren[i];
                    while ((composite != null) && (composite.NodeType != NodeType.Paragraph))
                        composite = composite.LastNonMarkupCompositeDescendant;
                    break;
                }
            }

            return composite;
        }

        /// <summary>
        /// Moves pending annotation node, which DisplacedBy property has <see cref="DisplacedByType.Prev"/> value, into
        /// a sibling structured document tag if it is present.
        /// </summary>
        private bool InsertAnnotationDisplacedByPreviousSdt(Node node)
        {
            IDisplaceableByCustomXml displaceableNode = node as IDisplaceableByCustomXml;
            if ((displaceableNode == null) ||
                (displaceableNode.DisplacedByCustomXml != DisplacedByType.Prev) ||
                mMarkupResolver.IsInStartingMarkup)
                return false;

            if ((CurContainer.LastChild == null) || (CurContainer.LastChild != mMarkupResolver.EndNodeOfLastMarkup))
                return false;

            CompositeNode composite = GetLastParagraphOfSdt(mMarkupResolver.LastMarkup);
            if (composite != null)
            {
                composite.AppendChild(node);
                // Need to reset DisplacedBy since node place is incorrect for displacing.
                displaceableNode.DisplacedByCustomXml = DisplacedByType.Unspecified;
                return true;
            }

            return false;
        }

        internal void ResolveMarkup()
        {
            mMarkupResolver.Resolve();
        }

        /// <summary>
        /// Reads 'w:sectPr' element into <see cref="GlobalSectPr"/>.
        /// Reader should be positioned to element start.
        /// </summary>
        /// <param name="isLastSectPr">Indicates that we read last SectPr. See <see cref="IsLastSectPrRead"/> for details.</param>
        internal void ReadGlobalSectPr(bool isLastSectPr)
        {
            mSectPrReader.Read(this, GlobalSectPr);

            if (isLastSectPr)
                mIsLastSectPrRead = true;

            if (CurContainer.NodeType == NodeType.Body)
                IsBlockLevelGlobalSectPr = true;
        }

        /// <summary>
        /// Reads all children of the current element into the specified node.
        /// </summary>
        internal void ReadStory(CompositeNode story)
        {
            // Flush pending annotations to the current story before started
            // to read new one (this is the end of previous store).
            FlushPendingCrossStructureAnnotations();

            // We are saving and restoring the current container, rather than just adding the story
            // straight away to the document tree because story nodes are usually added to the document later.
            // For example, comments are read first from the comments part into a hashtable, but they are
            // inserted into the document tree when inline nodes of the main document part are processed.
            CompositeNode savedContainer = CurContainer;
            CurContainer = story;

            mStoryReader.ReadChildren(this);

            // Flush pending annotations to the current story as we finished
            // to read it here (this is the end of current store).
            FlushPendingCrossStructureAnnotations();

            // Restore container.
            CurContainer = savedContainer;
        }

        /// <summary>
        /// Ends current section and starts new one.
        /// </summary>
        /// <remarks>The current reader container mast be a Body.</remarks>
        internal void StartNewSection()
        {
            // End current section.
            PopContainer(NodeType.Body);
            PopContainer(NodeType.Section);

            // Start new section.
            AddAndPushContainer(new Section(Document));
            AddAndPushContainer(new Body(Document));
        }

        /// <summary>
        /// Updates table when TableNormal style is missing.
        /// </summary>
        internal void FixMissingTableNormalIfNeeded(Table table)
        {
            if (Document.Styles.Contains(StyleIdentifier.TableNormal))
                return;

            if((table.Istd == StyleIndex.TableNormal))
            {
                // When TableNormal style is missing Word updates table rows with fixed direct padding.
                foreach (Row row in table.Rows)
                {
                    TablePr rowPr = row.TablePr;
                    rowPr.SetAttrNoOverride(TableAttr.LeftPadding, 10);
                    rowPr.SetAttrNoOverride(TableAttr.RightPadding, 10);
                    rowPr.SetAttrNoOverride(TableAttr.TopPadding, 0);
                    rowPr.SetAttrNoOverride(TableAttr.BottomPadding, 0);
                }
            }

            // Need to fix indentation only for previous versions (see TestDefect3309_109) or when version is unspecified.
            Document doc = Document as Document;
            if ((doc != null) && (doc.CompatibilityOptions.MswVersion < MsWordVersionCore.Word2013) && table.HasCells)
                FixUpTableIndentation(table);
        }

        private static void FixUpTableIndentation(Table table)
        {
            Debug.Assert(table != null);

            // First row may be empty at this code point.
            Cell cell = table.FirstRow.FirstCell;
            if (cell == null)
                return;

            TablePr tablePr = table.FirstRow.TablePr;
            StyleCollection styles = table.Document.Styles;
            TableStyle tblStyle = styles.GetByIstd(tablePr.Istd, false) as TableStyle;

            bool hasIndentation = (tablePr[TableAttr.LeftIndent] != null) ||
                ((tblStyle != null) && NrxTableUtil.HasAttrInTableStyles(cell.Document, tablePr, TableAttr.LeftIndent));

            // Looks like the Word update table indentation only when "TableNormal" style is missing and the indentation
            // is not specified in the hierarchy.
            // The style "TableNormal" have been appeared since the Word 2003. According to tests the Word detects cases
            // when a document generated by an old version and does not apply changes for indentation. XML documents also
            // introduced in the Word 2003, so for all XML documents this fix should be applied.
            if (hasIndentation || styles.Contains(StyleIdentifier.TableNormal))
                return;

            // WORDSNET-23747 Update table left indentation from cell padding.
            int leftPadding = (int)((ICellAttrSource)cell).FetchCellAttr(CellAttr.LeftPadding);
            if (leftPadding > 0)
               table.LeftIndent = ConvertUtilCore.TwipToPoint(leftPadding);

            // Need to remove style if it was added during fetching of padding it may affect reading of other tables.
            // See Test7128 for details.
            if ((tblStyle == null) &&
                (tablePr.Istd <= StyleCollection.MaxFixedIstd) &&
                (styles.GetByIstd(tablePr.Istd, false) != null))
            {
                styles.RemoveCore(table.Style);
            }

            // "TableNormal" should not appear after the indentation fixing.
            Debug.Assert(!styles.Contains(StyleIdentifier.TableNormal));
        }

        /// <summary>
        /// Checks that all used lists are exist and creates missed lists from predefined template.
        /// </summary>
        protected void EnsureUsedListsExist()
        {
            for (int i = 0; i < UsedListIds.Count; i++)
            {
                int listId = UsedListIds[i];

                if (Document.Lists.GetListByListId(listId) == null)
                {
                    AddPredefinedList(listId);
                }
            }
        }

        /// <summary>
        /// Adds predefined list and list definition.
        /// </summary>
        private void AddPredefinedList(int listId)
        {
            List list = new List(Document, listId);

            // All missed lists refers to one predefined list definition. Build it at first time.
            if (mMissedListsListDef == null)
            {
                int listDefId = Document.Lists.GetNextAvailableListDefId();
                mMissedListsListDef = new ListDef(Document, listDefId, ListType.HybridMultiLevel, listDefId);
                Document.Lists.AddListDef(mMissedListsListDef);

                SetupLevel(mMissedListsListDef.Levels[0], "\0.", 36, 720, -720);
                SetupLevel(mMissedListsListDef.Levels[1], "\x01.", 72, 1440, -720);
                SetupLevel(mMissedListsListDef.Levels[2], "\x02.", 108, 2160, -720);
                SetupLevel(mMissedListsListDef.Levels[3], "\x03.", 144, 2880, -720);
                SetupLevel(mMissedListsListDef.Levels[4], "\x04.", 180, 3600, -720);
                SetupLevel(mMissedListsListDef.Levels[5], "\x05.", 216, 4320, -720);
                SetupLevel(mMissedListsListDef.Levels[6], "\x06.", 252, 5040, -720);
                SetupLevel(mMissedListsListDef.Levels[7], "\a.", 288, 5760, -720);
                SetupLevel(mMissedListsListDef.Levels[8], "\b.", 324, 6480, -720);
            }
            else
            {
                // MS Word restarts each next list from '1' with list levels overrides.
                SetupListOverrides(list);
            }

            list.ListDefId = mMissedListsListDef.ListDefId;
            Document.Lists.AddList(list);
        }

        /// <summary>
        /// Helper method to easy setup list definition.
        /// </summary>
        private static void SetupLevel(ListLevel level, string numberFormat, int tabStop, int leftIndent, int firstLineIndent)
        {
            level.NumberStyle = NumberStyle.Arabic;
            level.NumberFormat = numberFormat;
            TabStopCollection tabStops = new TabStopCollection();
            tabStops.Add(new TabStop(tabStop * 20, TabAlignment.List, TabLeader.None));
            level.ParaPr.TabStops = tabStops;
            level.ParaPr.LeftIndent = leftIndent;
            level.ParaPr.FirstLineIndent = firstLineIndent;
        }

        /// <summary>
        /// Creates list levels overrides with <see cref="ListLevelOverride.IsStartAt"/>
        /// equal to 'true' and adds it to the specified list.
        /// </summary>
        private static void SetupListOverrides(List list)
        {
            for (int i = 0; i < 9; i++)
            {
                ListLevelOverride levelOverride = new ListLevelOverride(list.Document, i);
                levelOverride.IsStartAt = true;
                list.Overrides.Add(levelOverride);
            }
        }

        /// <summary>
        /// Collects styles which needs to be updated to avoid possible toggle issues.
        /// </summary>
        /// <remarks>
        /// AM. Workaround to avoid very problematic case described in wiki article https://auckland.dynabic.com/wiki/display/org/AttrBoolEx+nightmare
        /// We haven't choose toggle model yet but customer is impatient and I decided to make this little solution.
        /// Actually this is small step towards "DOC" model (model with Toggle/Same values).
        ///
        /// Not ready to make full solution, process only limited cases.
        ///
        /// If this solution considered acceptable, we don't need this detection at all and just need to update all styles.
        /// </remarks>
        internal void DetectToggleIssue(Run run)
        {
            foreach (int key in gToggleKeys)
            {
                StyleCollection styles = run.Document.Styles;

                // Do not update if run contains direct formatting.
                if (run.RunPr.Contains(key))
                    continue;

                // Do not update if run has no parent.
                Paragraph parentParagraph = run.ParentParagraph;
                if (parentParagraph == null)
                    continue;

                // Do not update if paragraph style missing.
                Style paraStyle = styles.GetByIstd(parentParagraph.ParaPr.Istd, false);
                if (paraStyle == null)
                    continue;

                // Do not update if paragraph style has no toggle attribute set.
                object paraValue = paraStyle.GetFontAttr(key, false);
                if (paraValue == null)
                    continue;

                // Check if we in table having other than TableNormal style.
                Table table = parentParagraph.ParentTable;
                bool canHaveTableStyleFormatting = (table != null) && (table.Istd != StyleIndex.TableNormal);

                if (canHaveTableStyleFormatting)
                {
                    // Do not investigate table style for toggles, just update paragraph style to avoid possible issues.
                    mToggleIssueStyles.Add(paraStyle.Istd);

                    Style charStyle = styles.GetByIstd(run.RunPr.Istd, false);
                    if (charStyle == null)
                        continue;

                    object charValue = charStyle.GetFontAttr(key, false);
                    if (charValue == null)
                        continue;

                    mToggleIssueStyles.Add(charStyle.Istd);
                }
                else
                {
                    // Outside of tables update only if both character and paragraph styles have toggle attribute set.
                    Style charStyle = styles.GetByIstd(run.RunPr.Istd, false);
                    if (charStyle == null)
                        continue;

                    object charValue = charStyle.GetFontAttr(key, false);
                    if (charValue == null)
                        continue;

                    mToggleIssueStyles.Add(paraStyle.Istd);
                    mToggleIssueStyles.Add(charStyle.Istd);
                }
            }
        }

        /// <summary>
        /// Applies default style of a specified type to the specified attributes collection.
        /// </summary>
        /// <dev>
        /// In accordance with ISO29500-1, 17.7.4.17 style (Style Definition):
        /// ...The paragraph style with the default attribute set is the paragraph style applied
        /// to all paragraphs which do not explicitly reference a paragraph style using the pStyle element...
        /// </dev>
        internal void ApplyDefaultStyle(AttrCollection attrs, StyleType type)
        {
            int key;
            StyleIdentifier styleIdentifier;
            switch (type)
            {
                case StyleType.Paragraph:
                {
                    key = ParaAttr.Istd;
                    styleIdentifier = StyleIdentifier.Normal;
                    break;
                }
                case StyleType.Table:
                {
                    key = TableAttr.Istd;
                    styleIdentifier = StyleIdentifier.TableNormal;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }

            if (!attrs.Contains(key))
            {
                Style defaultStyle = DefaultStyles[(int)type];
                if ((defaultStyle != null) && (defaultStyle.StyleIdentifier != styleIdentifier))
                    attrs.SetAttr(key, defaultStyle.Istd);
            }
        }

        /// <summary>
        /// Updates styles that caused toggle issue.
        /// </summary>
        /// <remarks>
        /// See <see cref="DetectToggleIssue"/> for details.
        /// </remarks>
        protected void ResolveToggleIssues()
        {
            foreach(int key in gToggleKeys)
                foreach (int istd in mToggleIssueStyles)
                {
                    Style style = Document.Styles.GetByIstd(istd, false);

                    ConvertToggles(style, key);

                    Style baseStyle = style.GetBaseStyle();
                    if (baseStyle != null)
                        ConvertToggles(baseStyle, key);
                }
        }

        /// <summary>
        /// Converts toggle attributes to Toggle/Same model.
        /// </summary>
        private static void ConvertToggles(Style style, int key)
        {
            AttrBoolEx value = (AttrBoolEx)style.RunPr[key];

            if ((value == AttrBoolEx.True) || (value == AttrBoolEx.False))
            {
                AttrBoolEx inheritedValue = (AttrBoolEx)((IRunAttrSource)style).FetchInheritedRunAttr(key);
                style.RunPr.SetAttr(key, (value == inheritedValue) ? AttrBoolEx.Same : AttrBoolEx.Toggle);
            }
        }

        /// <summary>
        /// Indicate whether leading and trailing white-spaces in w:t element must be preserved.
        /// Set upon reading WML document.
        /// </summary>
        internal bool PreserveSpace
        {
            get { return mPreserveSpace; }
            set { mPreserveSpace = value; }
        }

        internal LoadOptions LoadOptions
        {
            get { return mLoadOptions; }
        }

        internal OoxmlComplianceInfo ComplianceInfo
        {
            get { return mComplianceInfo; }
        }

        /// <summary>
        /// Content resolver for inline SDT's.
        /// </summary>
        internal InlineMarkupResolver InlineResolver
        {
            get { return mMarkupResolver.InlineResolver; }
        }

        /// <summary>
        /// Gets/sets an encoding that is detected on reading a math from equation XML.
        /// </summary>
        public Encoding DetectedEncoding
        {
            get { return mDetectedEncoding; }
            set { mDetectedEncoding = value; }
        }

        /// <summary>
        /// Currently started customXml revision.
        /// For a moment it is used for SDT's revisions only.
        /// </summary>
        /// <remarks>
        /// IN. MS Word wraps into customXmlRangeStart / customXmlRangeEnd starting and ending SDT's tags.
        /// So, these pair acts as starting and ending 'marker' for SDT's revision. As starting 'marker' occurs
        /// before any SDT is read, we need to store it in this temporary storage.
        /// Later, when we read SDT itself, we apply this revision to it.
        /// For details see ISO29500-1, 17.13.5.4 customXmlDelRangeEnd, 17.13.5.5 customXmlDelRangeStart,
        /// 17.13.5.6 customXmlInsRangeEnd, 17.13.5.7 customXmlInsRangeStart.
        /// </remarks>
        internal RevisionBase CurrentCustomXmlRevision
        {
            get { return mCurrentCustomXmlRevision; }
            set { mCurrentCustomXmlRevision = value; }
        }

        internal abstract bool IsEquationXmlReader { get; }

        /// <summary>
        /// Indicates that SectPr for last section is read.
        /// Word distinguishes section properties for very last section and for all others.
        /// It treats section properties as properties for very last section when they
        /// are located not inside ParaPr. For example, Word stops to read headers and
        /// footers when last SectPr is read (even if it is completely empty).
        /// Actually, Word starts to read them into footnote.
        /// </summary>
        internal bool IsLastSectPrRead
        {
            get { return mIsLastSectPrRead; }
        }

        /// <summary>
        /// The properties for very last section.
        /// </summary>
        internal SectPr LastSectPr
        {
            get { return mLastSectPr; }
        }

        /// <summary>
        /// Keeps last added to the collection default style of a certain type.
        /// The key is <see cref="StyleType"/>, the value is <see cref="Style"/>.
        /// </summary>
        internal IntToObjDictionary<Style> DefaultStyles
        {
            get { return mDefaultStyles; }
        }

        internal StoryRevisionStack StoryRevisionStack;

        /// <summary>
        /// Collects all ListId used in document.
        /// The order of ListIds is important, because MS Word creates missed lists
        /// for very first list in a different way with all other lists.
        /// So, do not sort collected ListIds.
        /// </summary>
        internal readonly List<int> UsedListIds = new List<int>();

        /// <summary>
        /// Collection of tables with missed TableGrid.
        /// </summary>
        internal readonly IList<Table> TablesWithMissedTableGrid = new List<Table>();

        /// <summary>
        /// Collects information about connector rules. Keys are shape name and values are connector rules.
        /// </summary>
        internal readonly Dictionary<string, ConnectorRule> ConnectorRules = new Dictionary<string, ConnectorRule>();

        /// <summary>
        /// Section properties read at document body level. See TestJira10940 for details.
        /// </summary>
        internal readonly SectPr GlobalSectPr = new SectPr();

        /// <summary>
        /// A flag indicating whether a 'sectPr' element located in a body as a direct child has been read.
        /// If such an element is present in a document, headers/footers of subsequent 'sectPr' elements are ignored.
        /// </summary>
        internal bool IsBlockLevelGlobalSectPr { get; private set; }

        /// <summary>
        /// Collects paragraph formatting specified outside of paragraph element.
        /// </summary>
        internal readonly ParaPr GlobalParaPr = new ParaPr();

        /// <summary>
        /// Collects table row formatting specified outside of the 'tr' element.
        /// </summary>
        internal readonly TablePr GlobalRowPr = new TablePr();

        /// <summary>
        /// Collects paragraph break formatting specified outside of paragraph element.
        /// </summary>
        internal readonly RunPr GlobalParagraphBreakRunPr = new RunPr();

        /// <summary>
        /// Click and Type paragraph style ID is stored here before document styles are read.
        /// </summary>
        internal string ClickAndTypeStyleId;
        /// <summary>
        /// Default table style ID is stored here before document styles are read.
        /// </summary>
        internal string DefaultTableStyleId;

        /// <summary>
        /// Collects styles caused AttrBoolEx issue. See <see cref="DetectToggleIssue"/>
        /// </summary>
        private readonly HashSetGeneric<int> mToggleIssueStyles = new HashSetGeneric<int>();

        private readonly LoadOptions mLoadOptions;
        /// <summary>
        /// The document we are building.
        /// </summary>
        private readonly DocumentBase mDoc;
        /// <summary>
        /// The current container node in the document tree to which we are adding child nodes.
        /// </summary>
        private CompositeNode mCurContainer;
        /// <summary>
        /// True if the current story is header or footer.
        /// </summary>
        private bool mIsInHeaderFooter;

        private readonly List<Node> mPendingCrossStructureAnnotations = new List<Node>();

        /// <summary>
        /// Collects inline level nodes read outside of paragraph.
        /// </summary>
        private readonly List<InlineInfo> mPendingInlines = new List<InlineInfo>();

        /// <summary>
        /// Collects block nodes read outside of story.
        /// </summary>
        private readonly List<Node> mPendingBlocks = new List<Node>();

        /// <summary>
        /// Collects paragraph nodes read on unexpected levels.
        /// </summary>
        private readonly List<Paragraph> mPendingParagraphs = new List<Paragraph>();

        /// <summary>
        /// Used by ShapeReader only. Maybe move there?
        /// </summary>
        private readonly List<ShapeBase> mZOrderListHeader = new List<ShapeBase>();
        /// <summary>
        /// Used by ShapeReader only. Maybe move there?
        /// </summary>
        private readonly List<ShapeBase> mZOrderListMain = new List<ShapeBase>();

        /// <summary>
        /// Maintains relationship between shape id (e.g. "_x0000_i1030") and shape object in the model.
        /// Used to retrieve shapes by id to assign them OLE data.
        /// (OLE data is defined separately from shapes in WordML.)
        /// </summary>
        private readonly Dictionary<string, ShapeBase> mShapeOriginalIdsMap = new Dictionary<string, ShapeBase>();

        // Collects all shape in document.
        private readonly List<ShapeBase> mAllShapes = new List<ShapeBase>();

        // Collects all DrawingML textboxes in document.
        private readonly IntToObjDictionary<ShapeBase> mDmlTextBoxes = new IntToObjDictionary<ShapeBase>();

        // Collects all linked DrawingML textboxes in document.
        private readonly Dictionary<int, SortedIntegerListGeneric<ShapeBase>> mDmlLinkedTextboxes =
            new Dictionary<int, SortedIntegerListGeneric<ShapeBase>>();

        /// <summary>
        /// Collects linked textboxes drawingMls with preserved sequence of reading shapes.
        /// </summary>
        private readonly Dictionary<int, Shape> mDmlTxbxByIndex = new Dictionary<int, Shape>();

        /// <summary>
        /// Key is style id string. Value is istd integer.
        /// </summary>
        private readonly Dictionary<object, int> mStyleIdToIstd = new Dictionary<object, int>();
        /// <summary>
        /// Key is bookmark id integer from XML. Value is string bookmark name.
        /// </summary>
        private readonly Dictionary<int, string> mIdToBookmarkName = new Dictionary<int, string>();
        /// <summary>
        /// Key is abstract num id. Value is a list definition object.
        /// </summary>
        private readonly Dictionary<int, ListDef> mAbstractNumIdToListDef = new Dictionary<int, ListDef>();

        /// <summary>
        /// Key is MD4 Guid hash of the image bytes. Value is the image bytes.
        /// </summary>
        private readonly Dictionary<BytesHash, byte[]> mImageBytesByGuidTable = new Dictionary<BytesHash, byte[]>();
        private readonly MarkupResolver mMarkupResolver;
        private bool mPreserveSpace;

        /// <summary>
        /// Key is string id of shape type. Value is ShapePr.
        /// See [ECMA-376] M.5.4 ShapeType Element.
        /// </summary>
        private readonly Dictionary<string, ShapePr> mShapeTemplates = new Dictionary<string, ShapePr>();
        private readonly OoxmlComplianceInfo mComplianceInfo;
        private readonly NrxStoryReaderBase mStoryReader;
        private readonly NrxSectPrReaderBase mSectPrReader;
        private Encoding mDetectedEncoding;

        /// <summary>
        /// Predefined list definition used for missed lists.
        /// </summary>
        private ListDef mMissedListsListDef;

        /// <summary>
        /// Currently started customXml revision.
        /// For a moment it is used for SDT's revisions only.
        /// </summary>
        private RevisionBase mCurrentCustomXmlRevision;

        private readonly SectPr mLastSectPr = new SectPr();
        private bool mIsLastSectPrRead;

        /// <summary>
        /// Enumerates toggle keys used to resolve toggle collision issue.
        /// </summary>
        /// <remarks>
        /// AM. So far this set is incomplete mostly because fix is experimental.
        /// </remarks>
        private static readonly int[] gToggleKeys = new[]
            {
                FontAttr.Bold,
                FontAttr.AllCaps,
                FontAttr.StrikeThrough,
                FontAttr.Hidden,
                FontAttr.Italic,
                FontAttr.ItalicBi,
                FontAttr.SmallCaps
            };

        /// <summary>
        /// Stores inline node and its parent story.
        /// </summary>
        private class InlineInfo
        {
            internal InlineInfo(Node inline, Node story)
            {
                Inline = inline;
                Story = story;
            }

            internal Node Inline { get; }
            internal Node Story { get; }
        }

        /// <summary>
        /// Keeps last added to the collection default style of a certain type.
        /// The key is <see cref="StyleType"/>, the value is <see cref="Style"/>.
        /// </summary>
        private readonly IntToObjDictionary<Style> mDefaultStyles = new IntToObjDictionary<Style>(4);

        private CompositeNode mPendingBlockDestination;

        private readonly Dictionary<Shape, string> mMissingShapeSources = new Dictionary<Shape, string>();
    }
}
