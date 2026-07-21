// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.RW.Vml;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Docx.Reader
{
    internal abstract class DocxDocumentReaderBase : NrxDocumentReaderBase, INrxDmlReader
    {
        protected DocxDocumentReaderBase(
            OpcPackageBase package,
            OpcPackagePart documentPart,
            DocumentBase doc,
            LoadOptions loadOptions,
            OoxmlComplianceInfo complianceInfo,
            DocxStylesReader stylesReader,
            DocxStoryReader storyReader,
            DocxNumberingReader numberingReader,
            DocxSectPrReader sectPrReader) :
            base(doc, loadOptions, WarningSource.Docx, complianceInfo, storyReader, sectPrReader)
        {
            Debug.Assert(package != null);
            Debug.Assert(documentPart != null);
            Debug.Assert(stylesReader != null);
            Debug.Assert(numberingReader != null);
            Debug.Assert(sectPrReader != null);

            mPackage = package;
            mDocumentPart = documentPart;
            mStylesReader = stylesReader;
            mNumberingReader = numberingReader;
            mXmlReader = new DocxXmlReader(mDocumentPart, loadOptions.WarningCallback, WarningSource.Docx, complianceInfo);
            ParaToCommentMapper = new ParagraphToCommentMapper();
        }

        internal void Read()
        {
            DocxFontTableReader.Read(this);
            DocxSettingsReader.Read(this);
            DocxWebSettingsReader.Read(this);
            mStylesReader.Read(this);
            mNumberingReader.Read(this);
            DocxPeopleReader.Read(this);
            DocxCommentsExReader.Read(this);
            DocxCommentsIdsReader.Read(this);
            DocxCommentsExtensibleReader.Read(this);
            DocxCommentsReader.Read(this);
            mFootnotesReader = new DocxFootnotesReader(this);

            DoRead();

            foreach (Style style in Document.Styles)
                FormatRevisionUtil.ConvertToPositiveDifference(style);

            UpdateCommentTree();
            UpdateZOrders();
            ResolveDuplicateShapeIds();
            UpdateTextboxNextShapeIds();
            UpdateDmlNextShapeIds();
            ResolveDocPrStyles();

            InsertAltChunks();
            Document.Lists.FixUpListsWithCircularReferences();
            EnsureStylePointsToExistingList();
            EnsureUsedListsExist();
            UpdateTablesWithMissedTableGrid();
            UpdateConnectors();
            Document.Lists.ClearListStyleReferencesLevels();

            ResolveToggleIssues();
        }

        /// <summary>
        /// Maps the specified ParaId and DurableId.
        /// </summary>
        internal void MapParaIdAndDurableId(int paraId, int durableId)
        {
            ParaToCommentMapper.MapParaIdAndDurableId(paraId, durableId);
        }

        /// <summary>
        /// Maps the specified ParaId and paragraph. Resolves duplicate ParaIds used in comments if needed.
        /// </summary>
        internal void MapParaId(int paraId, Paragraph para)
        {
            Debug.Assert(para != null);

            bool isInComment = (para.GetAncestor(NodeType.Comment) != null);

            if (isInComment)
                paraId = ParaToCommentMapper.MapParaIdAndResolveDuplicateIfNeeded(paraId);

            para.ParaId = paraId;

            if (isInComment)
                ParaToCommentMapper.MapParaIdAndParagraph(paraId, para);
        }

        /// <summary>
        /// Maps the specified ParaId and ParaId of the parent paragraph.
        /// </summary>
        internal void MapParaIdAndParentParaId(int paraId, int paraIdParent, bool done)
        {
            ParaToCommentMapper.MapParaIdAndParentParaId(paraId, paraIdParent, done);
        }

        /// <summary>
        /// Finds a <see cref="CommentExtensible"/> object stored on reading commentsIds document part.
        /// </summary>
        internal CommentExtensible FindCommentExtensible(int durableId)
        {
            return ParaToCommentMapper.FindCommentExtensible(durableId);
        }

        internal DocxRelationshipTypes RelTypes
        {
            get
            {
                if (mRelTypes == null)
                    mRelTypes = new DocxRelationshipTypes(ComplianceInfo.IsIsoStrict);
                return mRelTypes;
            }
        }

        /// <summary>
        /// Ensure list style points to existing list.
        /// When definition of a list points to some style but this style points to unexisting list,
        /// we have to mimic MS Word behavior and make this style pointing to the list that uses this definition.
        /// </summary>
        private void EnsureStylePointsToExistingList()
        {
            foreach (List list in Document.Lists)
            {
                FixListStyle(list);
            }
        }

        /// <summary>
        /// Determines TerminalList for the source List.
        /// </summary>
        private List GetTerminalList(List list)
        {
            if (list == null)
                return null;

            if (list.Style == null)
                return list;

            List styleList = Document.Lists.GetListByListId(list.Style.ParaPr.ListId);
            if (styleList == null)
                return null;

            if (styleList.ListId == list.ListId)
                return list;

            return GetTerminalList(styleList);
        }

        /// <summary>
        /// Ensure list style points to an existing list, otherwise fixes the list.
        /// </summary>
        private void FixListStyle(List list)
        {
            Style style = Document.Styles.GetByIstd(list.ListDef.ListStyleIstd, false);
            if ((style != null) && (style.Type == StyleType.List) && (style.Istd != StyleIndex.NoList))
            {
                List styleList = Document.Lists.GetListByListId(style.ParaPr.ListId);

                List terminalStyleList = GetTerminalList(styleList);
                if (terminalStyleList == null)
                    style.ParaPr.ListId = list.ListId;

                // WORDSNET-11410 MS Word removes reference to style from the list definition if
                // list definition of this style hasn't reference to the style.
                if ((styleList != null) && (styleList.ListDef.ListStyleIstd == StyleIndex.NoList))
                {
                    list.ListDef.ListStyleIstd = StyleIndex.NoList;
                }
            }
        }

        /// <summary>
        /// Import and insert altChunk content to the model.
        /// </summary>
        private void InsertAltChunks()
        {
            if (mAltChunkCollection.Count > 0)
            {
                // WORDSNET-18541 Word creates missed in main document reserved styles from the scratch even if they are
                // importing with KeepSourceStyles mode.
                Document.Styles.EnsureMinimum();

                for (int i = 0; i < mAltChunkCollection.Count; i++)
                {
                    DocxAltChunk altChunk = mAltChunkCollection[i];
                    Document altChunkDocument = altChunk.AltChunkDocument;
                    CompositeNode referenceContainer = altChunk.ReferenceContainer;
                    Node referenceNode = altChunk.ReferenceNode;
                    ImportFormatMode importFormatMode = altChunk.ImportFormatMode;
                    bool isInline = altChunk.IsInline;

                    bool isFirstChildInBody = false;
                    Node refParent = referenceContainer.ParentNode;
                    if ((refParent != null) && (refParent.NodeType == NodeType.Body))
                        isFirstChildInBody = ReferenceEquals(((Body)refParent).FirstChild, referenceContainer);

                    // WORDSNET-16555 Word takes in attention headers/footers of "AltChunk" document, which placed
                    // as first child in the section body. Related tests: TestJira7382, TestJira11451.
                    // WORDSNET-20804 Word also always processes headers/footers of multi-section AltChunk documents.
                    if (isFirstChildInBody || (altChunkDocument.Sections.Count > 1))
                        ResolveHeadersFooters(altChunkDocument, referenceContainer, importFormatMode);

                    if (isInline)
                        CopyInlineAltChunkContent(altChunkDocument, referenceContainer, referenceNode);
                    else
                        CopyAltChunkContent(altChunkDocument, referenceContainer, importFormatMode);

                    if (isFirstChildInBody && (altChunkDocument.Sections.Count > 1))
                        PostProcessHeadersFooters(altChunkDocument, referenceContainer, importFormatMode);

                    // WORDSNET-23496 Copy all HTML blocks from altChunk documents to the model.
                    // Some HTML blocks may be duplicated in different altChunks. HTML block collection resolves duplicates.
                    Document.HtmlBlockCollection.Add(altChunkDocument.HtmlBlockCollection);

                    // Remove unneeded container.
                    referenceContainer.Remove();

                    Warn(WarningType.DataLoss, WarningSource.Docx, "The altChunk is replaced with its content.");
                }

                mAltChunkCollection.Clear();
            }
        }

        /// <summary>
        /// AltChunk might occur on inline level, in this case all altChunk content should be inserted after reference node.
        /// </summary>
        private static void CopyInlineAltChunkContent(
            Document altChunkDocument,
            CompositeNode referenceContainer,
            Node referenceNode)
        {
            // Try to get paragraph which contains altChunk.
            Paragraph paragraphWithAltChunk = referenceContainer.PreviousNonAnnotationSibling as Paragraph;

            // Try to get SDT which contains altChunk.
            StructuredDocumentTag sdtWithAltChunk = referenceContainer.PreviousNonAnnotationSibling as StructuredDocumentTag;

            // Mimic MS Word behavior, use destination styles upon copying inline AltChunkContent.
            CopyAltChunkContent(altChunkDocument, referenceContainer, ImportFormatMode.UseDestinationStyles);

            if (paragraphWithAltChunk != null)
            {
                // Get first paragraph inserted from altChunk.
                Paragraph firstAltChunkParagraph = paragraphWithAltChunk.NextNonAnnotationSibling as Paragraph;

                // WORDSNET-18893 Preserve frame attrs from paragraph with altChunk.
                if (firstAltChunkParagraph != null)
                    MirrorFrameAttrs(paragraphWithAltChunk, firstAltChunkParagraph);

                // We have to clear this paragraph formatting, because it will be inherited from
                // the first AltChunk paragraph.
                paragraphWithAltChunk.ParagraphFormat.ClearFormatting();

                if (firstAltChunkParagraph != null)
                {
                    // WORDSNET-10134 Preserve ParaPr from the first AltChunk paragraph.
                    paragraphWithAltChunk.ParaPr = firstAltChunkParagraph.ParaPr.Clone();
                    paragraphWithAltChunk.ParagraphBreakRunPr = firstAltChunkParagraph.ParagraphBreakRunPr.Clone();

                    while (firstAltChunkParagraph.HasChildNodes)
                        paragraphWithAltChunk.InsertAfter(firstAltChunkParagraph.LastChild, referenceNode);

                    // Move annotations between the paragraphs into the left one.
                    paragraphWithAltChunk.InsertAfter(paragraphWithAltChunk.NextSibling,
                        firstAltChunkParagraph, referenceNode);

                    firstAltChunkParagraph.Remove();
                }
            }

            if (sdtWithAltChunk != null)
            {
                // Get first paragraph inserted from altChunk.
                Paragraph sdtParagraph = sdtWithAltChunk.ParentNode.NextNonAnnotationSibling as Paragraph;

                if (sdtParagraph != null)
                {
                    while (sdtParagraph.HasChildNodes)
                        sdtWithAltChunk.InsertAfter(sdtParagraph.LastChild, referenceNode);

                    // Move annotations between the paragraphs into the left one.
                    sdtWithAltChunk.InsertAfter(sdtWithAltChunk.ParentNode.NextSibling,
                        sdtParagraph, referenceNode);

                    // WORDSNET-19346 Copy the style of the first altChunk para to the parent sdt para.
                    Paragraph sdtParentPara = sdtWithAltChunk.ParentNode as Paragraph;

                    // WORDSNET-24805 Resiliency, do not assign wrong type style.
                    if ((sdtParentPara != null) && (sdtParagraph.ParagraphStyle.Type == StyleType.Paragraph))
                        sdtParentPara.ParagraphFormat.Style = sdtParagraph.ParagraphStyle;

                    sdtParagraph.Remove();
                }
            }
        }

        private static void MirrorFrameAttrs(Paragraph srcPara, Paragraph dstPara)
        {
            srcPara.ParaPr.MirrorTo(dstPara.ParaPr, ParaPr.FloatingAttrs);
            if (srcPara.ParaPr.ContainsAnyKey(ParaPr.FloatingAttrs))
                srcPara.ParaPr.MirrorTo(dstPara.ParaPr, ParaPr.BorderAttrs);
        }

        /// <summary>
        /// Copy content of AltChunk document to destination document.
        /// </summary>
        private static void CopyAltChunkContent(
            Document altChunkDocument,
            CompositeNode referenceContainer,
            ImportFormatMode importFormatMode)
        {
            // WORDSNET-28630 Ensure minimum altChunk document.
            altChunkDocument.EnsureMinimum();

            // WORDSNET-16262 Validate current content of the last section body before insertion to avoid exception.
            Body lastSectBody = altChunkDocument.LastSection.Body;
            lastSectBody.EnsureMinimum();

            NodeRange sourceRange = new NodeRange(altChunkDocument.FirstSection.Body.FirstChild, lastSectBody.LastChild);

            ExternalDocumentModifier modifier =
                new ExternalDocumentModifier(altChunkDocument, referenceContainer.Document, importFormatMode);
            NodeCopier.Copy(sourceRange, referenceContainer, modifier, null, NodeCopierOptions.None);
        }

        /// <summary>
        /// Concatenate AltChunk headers/footers with destination headers/footers.
        /// This is experimental code to mimic MS Word behavior.
        /// </summary>
        private static void ResolveHeadersFooters(Document altChunkDocument, CompositeNode referenceContainer, ImportFormatMode importFormatMode)
        {
            NodeImporter srcToDstImporter = new NodeImporter(altChunkDocument, referenceContainer.Document, importFormatMode);

            Section dstSection = (Section)referenceContainer.GetAncestor(NodeType.Section);

            // Copy header/footer from the first section of source document to destination document if headers/footers exist.
            if (altChunkDocument.FirstSection.HeadersFooters.Count > 0)
                AppendPrependHeadersFooters(altChunkDocument.FirstSection.HeadersFooters, dstSection, srcToDstImporter, true);

            //If AltChunk document contains more then one section and there is section after the current section
            //in destination document, we need to copy headers/footers from the next section
            //of destination document to all (except first) sections of source document. (Mimic MS Word behavior)
            if ((altChunkDocument.Sections.Count > 1) && (dstSection.NextSibling != null))
            {
                NodeImporter dstToSrcImporter = new NodeImporter(referenceContainer.Document, altChunkDocument, importFormatMode);

                // Section that follows the destination section.
                Section nextSection = (Section)dstSection.NextSibling;

                foreach (Section section in altChunkDocument.Sections)
                {
                    if (section.IsFirstSection || section.IsLastChild)
                        continue;

                    AppendPrependHeadersFooters(nextSection.HeadersFooters, section, dstToSrcImporter, false);
                }
            }
        }

        /// <summary>
        /// Copy header/footer from the last section of AltChunk document to the last inserted section in the document.
        /// </summary>
        private static void PostProcessHeadersFooters(Document altChunkDocument, CompositeNode referenceContainer, ImportFormatMode importFormatMode)
        {
            Section lastInsertedSection = (Section)referenceContainer.GetAncestor(NodeType.Section);
            NodeImporter srcToDstImporter = new NodeImporter(altChunkDocument, referenceContainer.Document, importFormatMode);
            AppendPrependHeadersFooters(altChunkDocument.LastSection.HeadersFooters, lastInsertedSection, srcToDstImporter, true);
        }

        /// <summary>
        /// Append or prepend the content of source header/footer to destination header/footer, or just add them.
        /// </summary>
        /// <param name="headersFooters">Header/footer collection of source section.</param>
        /// <param name="dstSection">Destination section.</param>
        /// <param name="nodeImporter">Node importer.</param>
        /// <param name="addToEnd">Indicate whether content of source header/footer must be added to the end of destination header/footer.</param>
        private static void AppendPrependHeadersFooters(HeaderFooterCollection headersFooters, Section dstSection, NodeImporter nodeImporter, bool addToEnd)
        {
            foreach (HeaderFooter headerFooter in headersFooters)
            {
                HeaderFooter sourceHeaderFooter = (HeaderFooter)nodeImporter.ImportNode(headerFooter, true);

                // Get specific HeaderFooter of current section.
                HeaderFooter destinationHeaderFooter = dstSection.HeadersFooters[headerFooter.HeaderFooterType];

                // If header/footer of specific type exists in current section of destination document,
                // then just append/prepend the content of source header/footer to it. Else add whole source header/footer.
                if (destinationHeaderFooter != null)
                {
                    if (addToEnd)
                    {
                        // Append empty paragraph - mimic MS Word behavior.
                        destinationHeaderFooter.AppendParagraph("");
                        nodeImporter.ImportAndAppendChildren(headerFooter, destinationHeaderFooter);
                    }
                    else
                    {
                        // Prepend empty paragraph - mimic MS Word behavior.
                        destinationHeaderFooter.PrependChild(new Paragraph(destinationHeaderFooter.Document));

                        while (sourceHeaderFooter.HasChildNodes)
                            destinationHeaderFooter.PrependChild(sourceHeaderFooter.LastChild);
                    }
                }
                else
                {
                    dstSection.HeadersFooters.Add(sourceHeaderFooter);
                }
            }
        }

        /// <summary>
        /// Updates comments tree.
        /// </summary>
        private void UpdateCommentTree()
        {
            ParaToCommentMapper.ApplyCommentExData();
            ParaToCommentMapper.ApplyCommentExtensibleData();
        }

        [JavaThrows(true)]  // IO Exceptions
        protected abstract void DoRead();

        protected void ReadBody(DocumentBase doc)
        {
            LoadingProgressProcessor progressProcessor = new LoadingProgressProcessor(doc, LoadOptions);

            StartSection(doc);

            string tagName = XmlReader.LocalName;
            while (XmlReader.ReadChild(tagName))
            {
                // WORDSNET-14472 Adding a Callback event that notifies the progress of loading document.
                progressProcessor.Execute(XmlReader.UnderlyingStream);

                if (XmlReader.LocalName == "sectPr")
                {
                    // When sectPr occurs in body it contains properties for the last section of the document.
                    // We just apply those properties to the current section and assume it is last.

                    // WORDSNET-8383 Final sectPr is not last element of body but Word reads them.
                    // Preserve and apply final section properties at document reading ends.
                    ReadGlobalSectPr(true);
                }
                else
                {
                    DocxReaderFactory.StoryReader.ReadChild(this);
                }
            }

            // WORDSNET-10436 <w:br> element might be read at the end of the body, so need to add new paragraph to write it.
            if (XmlReader.RunTextBuilder.Length > 0)
                FlushRun(Document);
        }

        /// <summary>
        /// Starts section if it is not started already. Section can be already started if there is content before body.
        /// <remarks>andrnosk: WORDSNET-5252</remarks>
        /// </summary>
        protected void StartSection(DocumentBase doc)
        {
            if (CurContainer.NodeType != NodeType.Body)
            {
                AddAndPushContainer(new Section(doc));
                AddAndPushContainer(new Body(doc));
            }
        }

        /// <summary>
        /// Closes the last section of the document. Call this method when reading whole document is finished,
        /// because there might be some content outside body.
        /// <remarks>andrnosk: WORDSNET-5252</remarks>
        /// </summary>
        protected void EndSection()
        {
            if (CurContainer.NodeType == NodeType.Body)
            {
                PopContainer(NodeType.Body);
                // If section properties were accumulated, but were not flushed into LastSectPr yet,
                // then we just use them as properties for last section.
                ((Section)CurContainer).SectPr = (GlobalSectPr.Count > 0) ? GlobalSectPr : LastSectPr;

                PopContainer(NodeType.Section);
            }
        }

        public override NrxXmlReader XmlReader
        {
            get { return mXmlReader; }
        }

        private OpcPackagePart GetRelationshipTargetPart(string relId)
        {
            string partName = GetRelationshipTarget(relId);
            return StringUtil.HasChars(partName)
                ? mPackage.GetPartByName(partName)
                : null;
        }

        public override IEmbeddedObject GetEmbeddedObject(string id)
        {
            OpcPackagePart olePart = GetRelationshipTargetPart(id);
            // WORDSNET-Defect 1581. We should check the presence of OLE object data before preceding.
            if (olePart == null)
                return null;

            // WORDSNET-15758 System.InvalidCastException is thrown while loading DOC file.
            // One OLE package may be referred more than once in document. Need to reset position.
            olePart.Stream.Position = 0;

            if (FileSystem.IsStructuredStorage(olePart.Stream))
            {
                // This is an embedded OLE object.
                FileSystem fs = new FileSystem(olePart.Stream);
                return new OleObject(fs.Root);
            }
            else
            {
                // This must be an embedded OOXML package.
                return new OoxmlObject(olePart.Stream, olePart.ContentType, olePart.Name);
            }
        }

        /// <summary>
        /// <para>Returns image data by relationship id.</para>
        /// <para>Memory optimization is used so equal data is not stored twice.
        /// In return, it's readonly data, do not modify it.</para>
        /// </summary>
        public override byte[] GetBinData(string relId)
        {
            OpcPackagePart imagePart = GetRelationshipTargetPart(relId);
            string partName = (imagePart == null) ? string.Empty : imagePart.Name;

            byte[] cachedBinData = mImageBytesByPartName.GetValueOrNull(partName);
            if (cachedBinData == null)
            {
                string contentType;
                cachedBinData = GetBinData(imagePart, out contentType);
                mImageBytesByPartName.Add(partName, cachedBinData);
            }

            return cachedBinData;
        }

        /// <summary>
        /// Add new paragraph and add run element with text <see cref="Run"/>.
        /// </summary>
        /// <param name="doc">Document</param>
        internal void FlushRun(DocumentBase doc)
        {
            Paragraph par = new Paragraph(doc);
            AddAndPushContainer(par);
            NrxRunReaderBase.FlushRun(this, new RunPr());
            PopContainer(NodeType.Paragraph);
        }

        /// <summary>
        /// <para>Returns image data by relationship id.</para>
        /// <para>Memory optimization is used so equal data is not stored twice.
        /// In return, it's readonly data, do not modify it.</para>
        /// <para>Returns ContentType of OpcPackagePart as output parameter.</para>
        /// </summary>
        internal byte[] GetBinData(OpcPackagePart imagePart, out string contentType)
        {
            contentType = "";

            byte[] binData;
            if (IsValidImagePart(imagePart))
            {
                if (LoadOptions.ConvertMetafilesToPng && ImageUtil.IsMetafile(FileFormatCore.FromExt(imagePart.Extension)))
                {
                    ImageSizeCore imageSizeCore = ImageUtil.GetMetafileSize(CompressedData.GetStream(imagePart.Stream));
                    SizeF resolution = new SizeF((float)imageSizeCore.HorizontalResolution, (float)imageSizeCore.VerticalResolution);

                    contentType = OpcContentType.ImagePng;
                    imagePart.Stream.Position = 0;
                    using (Stream imagePartStream = CompressedData.GetStream(imagePart.Stream))
                        binData = BitmapUtilPal.ConvertMetafileToBitmapUsingGdiPlus(imagePartStream, resolution, false, null);
                }
                else
                {
                    contentType = imagePart.ContentType;
                    imagePart.Stream.Position = 0;

                    if (contentType == DocxContentType.ImagePictCompressed)
                    {
                        binData = VmlUtil.UnpackPictImage(StreamUtil.CopyStreamToByteArray(imagePart.Stream));
                    }
                    else if (ImageUtil.IsStandardMetafile(imagePart.Stream))
                    {
                        // andrnosk: WORDSNET-6777 Add a placeable header to metafiles.
                        // For some reason, some wmf pictures do not render correctly without that header.
                        imagePart.Stream.Position = 0;
                        var metafileSize = ImageUtil.GetMetafileSize(imagePart.Stream);

                        imagePart.Stream.Position = 0;
                        binData = ImageUtil.PrependWmfPlaceableHeader(imagePart.Stream, metafileSize);
                    }
                    else
                    {
                        binData = StreamUtil.CopyStreamToByteArray(imagePart.Stream);
                    }
                }
            }
            else
            {
                // WORDSNET-8253 The document contains rels with Target="NULL"
                // I found nothing about such target in DOCX specification,
                // but I suppose MS Word consider such target as noimage, or just ignore it
                // let's load NoImage if we encounter such Target of an image
                // imagePartName is empty string if Target="NULL"
                binData = ImageUtil.GetNoImageBytes();
            }

            return GetNonDuplicatedBytes(binData);
        }

        /// <summary>
        /// <para>Returns bin data by relationship id.</para>
        /// <para>Guarantees that there are no other references to returned byte array
        /// so data may be modified without side effects.</para>
        /// <para>It's common case to attach data after modification back to reader by
        /// <see cref="NrxDocumentReaderBase.GetNonDuplicatedBytes"/>.</para>
        /// </summary>
        internal byte[] GetDetachedBinData(string relId)
        {
            OpcPackagePart binPart = GetRelationshipTargetPart(relId);

            // check binPart == null covers case, when document contains rels with Target="NULL". I'm not sure
            // if it's possible in common case and added the check as analogue for image data in GetBinData.

            return (binPart == null) ? null : StreamUtil.CopyStreamToByteArray(binPart.Stream);
        }

        public override string GetRelationshipTarget(string relId)
        {
            return mXmlReader.Part.GetRelationshipTarget(relId);
        }

        public override bool IsExternalImage(string src)
        {
            OpcRelationship rel = mXmlReader.Part.Rels.GetById(src);

            // WORDSNET-23938 Resilience for missing relationship part.
            return (rel == null) || rel.IsExternal;
        }

        internal bool IsExternalPackage(string src)
        {
            OpcRelationship rel = mXmlReader.Part.Rels.GetById(src);
            return rel.IsExternal;
        }

        public override void ReadBinData()
        {
            // Do nothing. This is applicable for WordML implementation only.
        }

        public override bool ReadTextboxContent(ShapeBase shape)
        {
            while (mXmlReader.ReadChild("textbox"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "txbxContent": // w:txbxContent
                    {
                        StoryRevisionStack.PushStory();
                        DocxReaderFactory.StoryReader.Read(this, shape);
                        StoryRevisionStack.PopStory();
                        break;
                    }
                    default:
                    {
                        mXmlReader.IgnoreElement();
                        break;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets next not-used footnote or endnote by an ID. Returns null if the node is not found. Raises an exception
        /// if all footnotes with the specified ID are already inserted into a document. Allows correctly process
        /// documents that has several footnotes with same ID: next not-used footnote/endnote is returned.
        /// </summary>
        internal override Footnote GetFootnoteById(FootnoteType footnoteType, int id)
        {
            return mFootnotesReader.GetFootnoteById(footnoteType, id);
        }

        internal override void AddComment(Comment comment)
        {
            // WORDSNET-12268 Mimic MS Word behavior and ignore next comment with the same ID.
            if (!mIdToComment.ContainsKey(comment.Id))

                // We add comments to a dictionary, not to the document tree straight away.
                mIdToComment.Add(comment.Id, comment);
        }

        internal override Comment GetCommentById(int id)
        {
            return mIdToComment[id];
        }

        internal override bool IsSmartTag
        {
            get { throw new InvalidOperationException("Not expected to call this."); }
        }

        internal override bool IsCustomXmlTag
        {
            get { throw new InvalidOperationException("Not expected to call this."); }
        }

        /// <summary>
        /// Needed for DOCX only.
        /// Switches the XML reader to read the part specified by the relationship id.
        /// Saves the current XML reader so it can be restored.
        /// </summary>
        internal DocxXmlReader SwitchToPartReaderByRelId(string relId)
        {
            OpcPackagePart part = GetPartByName(GetRelationshipTarget(relId));

            if (part == null)
                return null;

            mXmlReaderStack.Push(mXmlReader);
            mXmlReader = new DocxXmlReader(part, mXmlReader.WarningCallback, WarningSource.Docx, ComplianceInfo);
            return mXmlReader;
        }

        /// <summary>
        /// Creates a DOCX XML reader for a part that is a child of the document part and
        /// at the end of the specified relationship. Returns null if there is no such part.
        /// </summary>
        internal virtual DocxXmlReader SwitchToPartReaderByRelType(string relType)
        {
            return SwitchToPartReaderByRelType(mDocumentPart, relType);
        }

        /// <summary>
        /// Creates a DOCX XML reader for a part that is a child of the specified part and
        /// at the end of the specified relationship. Returns null if there is no such part.
        /// </summary>
        internal DocxXmlReader SwitchToPartReaderByRelType(OpcPackagePart parentPart, string relType)
        {
            OpcPackagePart part = GetPartByRelationshipType(parentPart, relType);
            if (part == null)
                return null;

            return PushPartReader(new DocxXmlReader(part, LoadOptions.WarningCallback, WarningSource.Docx, ComplianceInfo));
        }

        /// <summary>
        /// Restores the previously saved XML reader.
        /// </summary>
        internal virtual DocxXmlReader RestorePartReader()
        {
            mXmlReader = (mXmlReaderStack.Count > 0) ? mXmlReaderStack.Pop() : null;
            return mXmlReader;
        }

        internal DocxXmlReader PushPartReader(DocxXmlReader reader)
        {
            mXmlReaderStack.Push(mXmlReader);
            mXmlReader = reader;
            return mXmlReader;
        }

        /// <summary>
        /// Creates <see cref="DocxXmlReader"/> using specified XML string and collection of
        /// namespaces mapping and pushes it into part reader stack.
        /// </summary>
        internal DocxXmlReader CreateAndPushPartReader(string xml, IDictionary<string, string> namespaceMap)
        {
            // Expected that current reader can not be "null".
            Debug.Assert((mXmlReader != null) && (namespaceMap != null));

            DocxXmlReader reader = new DocxXmlReader(mXmlReader.Part, ComplianceInfo, xml, namespaceMap);
            return PushPartReader(reader);
        }

        internal override bool IsDocx
        {
            get { return true; }
        }

        internal OpcPackagePart GetPartByRelationshipType(OpcPackagePart parentPart, string relType)
        {
            return mPackage.GetPartByRelationshipType(parentPart, relType);
        }

        internal OpcPackagePart FetchPartByName(string name)
        {
            return mPackage.FetchPartByName(name);
        }

        /// <summary>
        /// Gets part by given name.
        /// </summary>
        /// <returns>Returns null if part is not found.</returns>
        internal OpcPackagePart GetPartByName(string name)
        {
            return mPackage.GetPartByName(name);
        }

        /// <summary>
        /// This is called when NrxRunReaderBase encounters a w:drawing element.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        ShapeBase INrxDmlReader.ReadDrawing(RunPr runPr)
        {
            DmlDrawingReader dmlReader = new DmlDrawingReader();
            return dmlReader.ReadDrawing(this);
        }

        /// <summary>
        /// Add AltChunk object to collection.
        /// </summary>
        /// <param name="altChunk">AltChunk object.</param>
        internal void AddAltChunk(DocxAltChunk altChunk)
        {
            mAltChunkCollection.Add(altChunk);
        }

        protected OpcPackageBase Package
        {
            get { return mPackage; }
        }

        internal OpcPackagePart DocumentPart
        {
            get { return mDocumentPart; }
        }

        /// <summary>
        /// Holds list of picture bullet ids to translate it to zero based values.
        /// </summary>
        internal List<int> PictureBulletTranslation
        {
            get { return mPictureBulletTranslation; }
        }

        internal List<DocxAltChunk> AltChunks
        {
            get { return mAltChunkCollection; }
        }

        internal bool IsValidImagePart(string relId)
        {
            return IsValidImagePart(GetRelationshipTargetPart(relId));
        }

        /// <summary>
        /// Checks that given OpcPackagePart is valid image part.
        /// </summary>
        private static bool IsValidImagePart(OpcPackagePart imagePart)
        {
            if (imagePart == null)
                return false;

            // andrnosk: WORDSNET-10229 Consider XML OpcPackagePart as invalid image part.
            if ((imagePart.ContentType == OpcContentType.Xml) ||
                (imagePart.Extension == "xml"))
            {
                return false;
            }

            // Word preserves octet streams even if they are zero sized for unknown reason.
            if (imagePart.ContentType == DocxContentType.OctetStream)
                return true;

            // For image content types zero size seems to be invalid.
            if (imagePart.Stream.Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Updates layout of tables with missed TableGrid.
        ///  </summary>
        private void UpdateTablesWithMissedTableGrid()
        {
            foreach (Table table in TablesWithMissedTableGrid)
            {
                table.UpdateLayout();
            }
        }

        /// <summary>
        /// Helper for mapping paragraphs to comments.
        /// </summary>
        private ParagraphToCommentMapper ParaToCommentMapper { get; }

        private DocxRelationshipTypes mRelTypes;

        private readonly OpcPackageBase mPackage;
        /// <summary>
        /// The document part.
        /// </summary>
        private readonly OpcPackagePart mDocumentPart;

        private readonly DocxStylesReader mStylesReader;
        private readonly DocxNumberingReader mNumberingReader;

        /// <summary>
        /// Current document part reader.
        /// </summary>
        private DocxXmlReader mXmlReader;
        /// <summary>
        /// During read we can switch from part to part, such switches are stored in this stack.
        /// </summary>
        private readonly Stack<DocxXmlReader> mXmlReaderStack = new Stack<DocxXmlReader>();
        /// <summary>
        /// All comments are read here before the main document is read.
        /// Key is integer comment id, value is a Comment object.
        /// </summary>
        private readonly IntToObjDictionary<Comment> mIdToComment = new IntToObjDictionary<Comment>();
        private DocxFootnotesReader mFootnotesReader;

        /// <summary>
        /// Collection of AltChunk.
        /// </summary>
        private readonly List<DocxAltChunk> mAltChunkCollection = new List<DocxAltChunk>();

        private readonly List<int> mPictureBulletTranslation = new List<int>();

        private readonly Dictionary<string, byte[]> mImageBytesByPartName = new Dictionary<string, byte[]>();
    }
}
