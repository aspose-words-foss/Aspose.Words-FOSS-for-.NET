// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2006 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.Formatting.Intern;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Properties;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Factories;
using Aspose.Words.RW.Ole;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// There are certain tasks that must be performed before a document is saved
    /// into DOC, WordML or RTF for the document to be valid in MS Word.
    /// Since we have several exporters, this logic is separated into this class.
    /// It is automatically invoked during save, before the exporter is invoked.
    /// </summary>
    internal class DocumentValidator : DocumentVisitor
    {
        /// <summary>
        /// Call this method to validate a document. Fills a <see cref="SaveInfo"/> that contains
        /// information needed by most of the exporters.
        /// </summary>
        internal void Execute(SaveInfo saveInfo)
        {
            Debug.Assert(saveInfo != null);

            Document doc = saveInfo.Document;

            // DocumentValidator can do node manipulations, for example updating SDT,
            // converting MathObject to Shapes and so on.
            // These operations should not be tracked so suspend tracking.
            using (new SuspendTrackRevisionsDocument(doc))
            {
                using (new SuspendMappedCustomXmlUpdateDocument(doc))
                {
                    mSaveInfo = saveInfo;
                    mIso29500ComplianceEnforcer = new Iso29500ComplianceEnforcer(mSaveInfo.SaveOptions, WarningCallback,
                                                                                 doc.ComplianceInfo);
                    mSaveActions = FetchSaveActions(mSaveInfo.SaveFormat);
                    // FOSS
                    mShapeValidator = new ShapeValidator(saveInfo, mSaveActions, this);
                    mListValidator = new ListValidator(doc.Lists, mSaveInfo, WarningCallback);
                    mFootnoteValidator = new FootnoteValidator();
                    mCustomXmlValidator = new CustomXmlValidator(mSaveInfo);
                    mUnitConverter = new UnitConverter();
                    mOfficeMathToShapeConverter = WriterFactory.CreateOfficeMathToShapeConverter(mSaveInfo.Document,
                        mSaveInfo.SaveOptions);
                    mTableFormattingExpanders = new Dictionary<DocumentBase, TableFormattingExpander>();

                    mRubyConverter = new RubyConverter(this, saveInfo);

                    mEQFieldConverter = new EQFieldConverter(this, saveInfo);

                    mRsidTable = doc.DocPr.Rsids;

                    mXmlMappingContext = new XmlMappingContext();
                    mXmlMappingContext.CustomTimeZone = saveInfo.SaveOptions.CustomTimeZoneInfo;
                    mProgressProcessor = new SavingProgressProcessor(doc, mSaveInfo.SaveOptions, SavingStageType.Processing);

                    // WORDSNET-23153 Sets the required Compliance if compatibilityMode is "15".
                    CustomCompatibilitySetting compatMode = doc.CompatibilityOptions.CustomCompatibilitySettings["compatibilityMode"];
                    if ((compatMode != null) && (compatMode.Value == "15") && (saveInfo.OoxmlCompliance == OoxmlComplianceCore.Ecma376))
                    {
                        if (doc.ComplianceInfo == null)
                            doc.ComplianceInfo = new OoxmlComplianceInfo();
                        // Starting from 2013 version MS Word sets the default value of Iso29500_2008_Transitional to a new document.
                        doc.ComplianceInfo.MarkAsIsoTransitional();
                    }

                    // AM. Don't really know should we update before or after actual validator processing.
                    if (mSaveInfo.SaveOptions.UpdateXmlMapping)
                        UpdateMappedCustomXml(doc);

                    doc.Accept(this);

                    // Reset reference to avoid progress call invocation for the glossary document.
                    mProgressProcessor = null;

                    if (doc.GlossaryDocument != null)
                        doc.GlossaryDocument.Accept(this);

                    RemoveUnusedSdtPlaceholders(doc);

                    // This performs a visiting over the entire document. Can it be reworked to work "inline" as all other validators here?
                    if (IsSaveAction(DocumentValidatorActions.UpdateListLabels))
                    {
                        ListLabelUpdater.UpdateListLabels(doc);
                        mListLabelsUpdated = true;
                    }

                    mShapeValidator.Finish();

                    // This map is needed for HTML export
                    saveInfo.BookmarkNamesSet = mBookmarkValidator.GetBookmarkNames();

                    ValidateCustomization(doc);

                    CheckCanSaveMacros();

                    // WORDSNET-26606 Rebuild MacroNames structure to start AutoOpen macros properly.
                    if ((doc.VbaProject != null) && (doc.VbaProject.MacroNames == null))
                        doc.VbaProject.RebuildMacroNames();

                    // Synchronize public font embedding options stored in FontInfos
                    // and internal font embedding options stored in DocPr.
                    doc.DocPr.UpdateEmbedFontOptions(doc.FontInfos);

                    if (mSaveInfo.SaveOptions.MemoryOptimization)
                        InternManager.Add(doc);
                }
            }
        }

        /// <summary>
        /// Updates content controls mapped to CustomXML.
        /// </summary>
        /// <dev>
        /// AM. I think we need update outside DocumentVisitor pattern during post-loading stage as well but lets postone.
        /// </dev>
        private void UpdateMappedCustomXml(Document doc)
        {
            NodeType[] sdtNodeTypes =
                new NodeType[] { NodeType.StructuredDocumentTag, NodeType.StructuredDocumentTagRangeStart };

            List<Node> sdtList = doc.GetChildNodes(sdtNodeTypes, true).ToList<Node>();

            // AM. Use following simple algorithm:
            // First we collect all document markup into the list.
            // Before actual update of the SDT remove all its nested markups from the list as might be replaced during update.
            // After the SDT has been updated restore the list by adding all actual nesting markup back to the list.

            // Do not use enumerator here, list will be modified inside loop.
            for(int i = 0; i < sdtList.Count; i++)
            {
                Node sdtNode = sdtList[i];

                StructuredDocumentTag sdt = sdtNode is StructuredDocumentTagRangeStart
                    ? ((StructuredDocumentTagRangeStart)sdtNode).InternalSdt
                    : (StructuredDocumentTag)sdtNode;

                // WORDSNET-16209 Do not update deleted SDTs.
                if (((ITrackableNode)sdt).DeleteRevision != null)
                    continue;

                // Update data bound SDT.
                if (sdt.XmlMapping.ChecksumNotEqual(mXmlMappingContext))
                {
                    // We are going to update content.
                    // Remove all nested markups from list as it might be replaced.
                    foreach (Node node in (sdt as CompositeNode).GetChildNodes(sdtNodeTypes, true))
                        sdtList.Remove(node);

                    if (sdt.XmlMapping.UpdateContent(mXmlMappingContext))
                    {
                        // Update Word2019 custom XML CRC checksum.
                        // Note, we do not update original checksum during post-loading to track mapped XML possible changes.
                        // WORDSNET-22617 Write checksum for the mapped content by default as the latest Word does.
                        if ((sdt.XmlMapping != null) &&
                            (sdt.XmlMapping.CustomXmlPart != null) &&
                            // In the case of the Ecma376 compliance the AW ignores data binding while writing the  document.
                            (mSaveInfo.OoxmlCompliance != OoxmlComplianceCore.Ecma376) &&
                            // Valid only for rich textbox.
                            (sdt.SdtType == SdtType.RichText))
                        {
                            sdt.XmlMapping.StoreItemChecksum =
                                Crypto.Crc32Xfer.GetBase64String(sdt.XmlMapping.CustomXmlPart.Data);
                        }
                    }
                    else
                    {
                        Warn(
                            WarningType.DataLoss,
                            WarningSource.Validator,
                        WarningStrings.DocumentValidatorSdtXmlMapping);
                    }

                    // Collect all, probably newly created, nested markup.
                    sdtList.AddRange((sdt as CompositeNode).GetChildNodes(sdtNodeTypes, true));
                }
            }
        }

        /// <summary>
        /// Check that the file format allow save macros or writer don't check it.
        /// </summary>
        private void CheckCanSaveMacros()
        {
            if (!mSaveInfo.Document.HasMacros)
                return;

            switch (mSaveInfo.SaveFormat)
            {
                case SaveFormat.Docx:
                case SaveFormat.Dotx:
                case SaveFormat.FlatOpc:
                    throw new InvalidOperationException(
                        "This document contains macros (VBA project) and you are " +
                        "attempting to save it in a Macro-Free format. " +
                        "Such document will be invalid if created. " +
                        "You need to either save it in a Macro-Enabled format (.DOCM or .DOTM) or " +
                        "remove macros before saving using the Document.RemoveMacros method.");
                default:
                    // Do nothing, all ok.
                    break;
            }
        }

        /// <summary>
        /// Reverts attributes modified by expanding procedures.
        /// </summary>
        /// <remarks>TableFormattingExpander may modify in table nodes.
        /// All these changes are reverted after document is saved.</remarks>
        internal void Revert()
        {
            using (new SuspendTrackRevisionsDocument(mSaveInfo.Document))
            {
                if (mSaveInfo.SaveOptions.MemoryOptimization)
                    InternManager.Remove(mSaveInfo.Document);

                // Revert changes made by table expanders in document and glossary document.
                foreach (KeyValuePair<DocumentBase, TableFormattingExpander> entry in mTableFormattingExpanders)
                    entry.Value.RestoreDirectAttrs();

                // WORDSNET-27614 Do not leave the DML shape replaced with its fallback.
                mShapeValidator.Revert();

                mOfficeMathToShapeConverter.Revert();
                mRubyConverter.Revert();
                mEQFieldConverter.Revert();
                mTableValidator.Revert();
            }
        }

        public override VisitorAction VisitDocumentStart(Document doc)
        {
            // AM. I tried to avoid accept lot of golds by do not writing rsid table.
            // Unfortunately RTF golds already contains rsid table collected during writing.
            // So I clear original rsid table and it will be rebuilt in test mode.
            if (!mSaveInfo.SaveOptions.WriteRsidTable)
                mSaveInfo.Document.DocPr.Rsids.Clear();

            if (doc.DocPr.LinkStyles)
            {
                // AM. There is some issue related to RTF format.
                // RTF format supports update style option but Word does not update style from custom template.
                // If custom template is specified then Word shows "Automatically update styles" checkbox unchecked
                // ignoring /linkstyles keyword in document.
                // So RTF can update styles only from default template specified in RTF as empty string.
                // Currently I decided to do not mimic this issue.
                doc.UpdateStyles(mSaveInfo.SaveOptions);
            }

            ResetOptionsIfOptimizedForVersion(doc);

            DocumentStartCore(doc);
            mIso29500ComplianceEnforcer.UpdateCompatibilityOptions(doc.CompatibilityOptions);

            mCustomXmlValidator.VisitDocumentStart(doc);

            if (doc.DigitalSignatures.Count > 0)
                Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.DocumentIsDigitallySigned);

            // FOSS

            UpdateDocumentSecurityProperty();
            doc.BuiltInDocumentProperties.TrimSpaces();
            doc.EnsureMinimum();

            doc.Styles.Validate();

            if (!doc.SdtPlaceholderManager.IsGlossaryEmpty)
                doc.SdtPlaceholderManager.StartReferenceCounter();

            mUnitConverter.VisitDocumentStart(doc);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitDocumentEnd(Document doc)
        {
            DocumentEndCore(doc);

            // Remove non-closed SDT ranges.
            while (mSdtRangeStack.Count > 0)
                mSdtRangeStack.Pop().Remove();

            if (doc.CompatibilityOptions.IsOptimized &&
                mSaveInfo.IsDocxFormat &&
                (OoxmlCompliance == OoxmlComplianceCore.Ecma376) &&
                (doc.CompatibilityOptions.MswVersion >= MsWordVersionCore.Word2010))
            {
                Warn(WarningType.Hint, WarningSource.Validator, WarningStrings.HintToRemoveCompatibilityMode);
            }

            // This call need to be sure that validation stage finished with the 100% value.
            if (mProgressProcessor != null)
                mProgressProcessor.Complete();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitGlossaryDocumentStart(GlossaryDocument glossary)
        {
            glossary.SdtPlaceholderManager.StartReferenceCounter();

            DocumentStartCore(glossary);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitGlossaryDocumentEnd(GlossaryDocument glossary)
        {
            DocumentEndCore(glossary);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Performs re-initialization for the main document and then for the glossary document.
        /// </summary>
        private void DocumentStartCore(DocumentBase doc)
        {
            // We allow long bookmark names for several document formats.
            bool allowLongBookmarkNames = IsSaveAction(DocumentValidatorActions.LongBookmarkNames);

            mBookmarkValidator = new BookmarkValidator(WarningCallback, !allowLongBookmarkNames);
            mAnnotationValidator = new AnnotationValidator(doc, mSaveInfo, WarningCallback);
            mFieldValidator = new FieldValidator(WarningCallback, mSaveInfo.SaveFormat);
            mMathValidator = new MathValidator(doc);
            mTableValidator = new TableValidator(mSaveActions, mSaveInfo);
            mShapeValidator.DocumentStart(doc, mBookmarkValidator);
            mUsedFontNames = IsUpdateFontInfos ? new CaseInsensitiveHashSet() : null;
            mStoryTypeStack = new StoryTypeStack();

            // It only makes sense to expand table styles if the load format supports them.
            // If we expand table styles when the load format does not support them, we get several problems in golds.
            // These are probably because some codecs read table styles partially (e.g. table-level run and paragraph properties only).
            // This results in incorrect (incomplete) expansion. So we pretend we don't have table styles in this case.
            if (LoadFormatSupportsTableStyles(mSaveInfo.Document.OriginalLoadFormat))
            {
                // Ideally we should only expand table style formatting when it is required for this save format.
                // But (as with WordML for example) we also should expand table styles if document default formatting
                // will be expanded. If we expand only document defaults without expanding table styles we can create
                // situations where default formatting will "override" table style formatting (because it is expanded
                // into paragraph styles) and the output will appear different to the original.
                if (IsSaveAction(DocumentValidatorActions.ExpandTableStyles))
                {
                    // RK This looks like an optimization. Do we need it?
                    if (HasTableStyleFormatting(doc))
                        mTableFormattingExpanders[doc] = new TableFormattingExpander();
                }
            }

            if ((doc.People.Count > 0) && mSaveInfo.IsPreIsoTransitionalWordFormat())
                WarnOfUnsupportedByFormat(WarningType.DataLoss, WarningStrings.NotSupportedPeoplePart);

            WebExtensionValidator.VisitDocumentStart(doc, mSaveInfo, WarningCallback);

            WarnOfUnsupportedSettings(doc);

            mListValidator.VisitDocumentStart();
        }

        /// <summary>
        /// Resets document options that are not supported by desired version of MS Word if optimization of document
        /// contents for the version is performed by customer.
        /// </summary>
        private static void ResetOptionsIfOptimizedForVersion(Document doc)
        {
            if (!doc.CompatibilityOptions.IsOptimized)
                return;

            MsWordVersionCore version = doc.CompatibilityOptions.MswVersion;
            DocPr docPr = doc.DocPr;

            if (version < MsWordVersionCore.Word2010)
            {
                docPr.DiscardImageEditingData = false;
                docPr.DefaultImageDpi = 0;
                docPr.DocId = null;
            }

            if (version < MsWordVersionCore.Word2013)
            {
                docPr.ChartTrackingRefBased = false;
                docPr.DocumentSetId = null;
            }
        }

        /// <summary>
        /// Generates warnings if some settings are not supported by destination format.
        /// Works only for OOXML, DOC, RTF, WML formats.
        /// </summary>
        private void WarnOfUnsupportedSettings(DocumentBase doc)
        {
            DocPr docPr = doc.DocPr;
            SaveFormat format = mSaveInfo.SaveFormat;

            // [MS-DOCX] settings
            if (mSaveInfo.IsPreIsoTransitionalWordFormat())
            {
                if (docPr.DiscardImageEditingData)
                    WarnOfUnsupportedSetting("discardImageEditingData");

                if (docPr.DefaultImageDpi > 0)
                    WarnOfUnsupportedSetting("defaultImageDpi");

                if (docPr.ChartTrackingRefBased)
                    WarnOfUnsupportedSetting("chartTrackingRefBased");

                if (StringUtil.HasChars(docPr.DocumentSetId))
                    WarnOfUnsupportedSetting("w15:docId");

                if (StringUtil.HasChars(docPr.DocId))
                    WarnOfUnsupportedSetting("w14:docId");
            }

            if (format == SaveFormat.WordML)
            {
                if (docPr.DoNotAutoCompressPictures)
                    WarnOfUnsupportedSetting("doNotAutoCompressPictures");

                if (docPr.RemoveDateAndTime)
                    WarnOfUnsupportedSetting("removeDateAndTime");
            }
            else if (format == SaveFormat.Rtf)
            {
                if (docPr.DoNotAutoCompressPictures)
                    WarnOfUnsupportedSettingByAw("doNotAutoCompressPictures");

                if (docPr.RemoveDateAndTime)
                    WarnOfUnsupportedSettingByAw("removeDateAndTime");
            }

            if (mSaveInfo.IsLegacyFormat)
            {
                if (docPr.UpdateFields)
                    WarnOfUnsupportedSetting("updateFields");

                if (docPr.ForceUpgrade)
                    WarnOfUnsupportedSetting("forceUpgrade");
            }
        }

        /// <summary>
        /// Performs finalization for the main document and then for the glossary document.
        /// </summary>
        private void DocumentEndCore(DocumentBase doc)
        {
            // If the document has a theme we need to update theme colors.
            bool isThemeColorsNeedUpdate = (doc.GetThemeInternal() != null);

            // If the font info collection must be updated, for example for ODT documents to work, let's do.
            if (IsUpdateFontInfos || isThemeColorsNeedUpdate)
            {
                RunPrValidator runPrValidator = new RunPrValidator();
                runPrValidator.Execute(doc, mUsedFontNames);
            }

            if (IsUpdateFontInfos)
                doc.FontInfos.UpdateToUsedFonts(mUsedFontNames);

            MailMergeSettings mailMergeSettings = doc.DocPr.MailMergeSettings;
            if (!mailMergeSettings.IsEmpty)
                mailMergeSettings.Odso.FieldMapDatas.MakeValid();

            mFieldValidator.VisitDocumentEnd();
            mAnnotationValidator.VisitDocumentEnd();
            mBookmarkValidator.VisitDocumentEnd();
            mListValidator.VisitDocumentEnd();
            mShapeValidator.DocumentEnd(doc);

            // WORDSNET-21167 Word adds default footnote separators if it missing and
            // there any footnote in the document.
            if (mSaveInfo.HasFootnotes)
            {
                EnsureFootnoteSeparator(doc, FootnoteSeparatorType.FootnoteSeparator);
                EnsureFootnoteSeparator(doc, FootnoteSeparatorType.FootnoteContinuationSeparator);
            }

            if (mSaveInfo.HasEndnotes)
            {
                EnsureFootnoteSeparator(doc, FootnoteSeparatorType.EndnoteSeparator);
                EnsureFootnoteSeparator(doc, FootnoteSeparatorType.EndnoteContinuationSeparator);
            }
        }

        /// <summary>
        /// A part of the hack that determines whether to expand table formatting.
        /// </summary>
        private static bool LoadFormatSupportsTableStyles(LoadFormat loadFormat)
        {
            switch (loadFormat)
            {
                case LoadFormat.Doc:
                case LoadFormat.Dot:
                case LoadFormat.Docx:
                case LoadFormat.Docm:
                case LoadFormat.Dotx:
                case LoadFormat.Dotm:
                case LoadFormat.FlatOpc:
                case LoadFormat.WordML:
                case LoadFormat.Rtf:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if there is at least one formatting attribute specified for at least one table style.
        /// </summary>
        private static bool HasTableStyleFormatting(DocumentBase doc)
        {
            foreach (Style style in doc.Styles)
            {
                if (style is TableStyle && style.HasFormatting())
                    return true;
            }
            return false;
        }

        private void UpdateDocumentSecurityProperty()
        {
            DocumentSecurity security = DocumentSecurity.None;

            if (mSaveInfo.Document.DocPr.WriteProtection.ReadOnlyRecommended)
                security |= DocumentSecurity.ReadOnlyRecommended;

            if (mSaveInfo.Document.DocPr.WriteProtection.IsWriteProtected)
                security |= DocumentSecurity.ReadOnlyEnforced;

            // This seems to be what MS Word is doing.
            switch (mSaveInfo.Document.ProtectionType)
            {
                case ProtectionType.AllowOnlyComments:
                case ProtectionType.ReadOnly:
                    security |= DocumentSecurity.ReadOnlyExceptAnnotations;
                    break;
                case ProtectionType.AllowOnlyFormFields:
                case ProtectionType.AllowOnlyRevisions:
                case ProtectionType.NoProtection:
                    // Do nothing.
                    break;
                default:
                    throw new InvalidOperationException("Unknown protection type.");
            }

            mSaveInfo.Document.BuiltInDocumentProperties.Security = security;
        }

        public override VisitorAction VisitSectionStart(Section section)
        {
            // A section needs a body with one paragraph to be valid.
            section.EnsureMinimum();

            // RK I am not so sure that we need to enforce compliance from 29500 to other file formats.
            // Maybe we just need to make sure all other formats can correctly handle the new values.
            mIso29500ComplianceEnforcer.EnforceSectionCompliance(section);

            if ((section.SectPr.FootnoteColumns > 0) && mSaveInfo.IsPreIsoTransitionalWordFormat())
            {
                WarnOfUnsupportedByFormat(WarningType.MinorFormattingLoss,
                    WarningStrings.NotSupportedFootnoteColumns);
            }

            RemoveLineNumberingDefaults(section);

            foreach (int key in SectPr.BorderAttrs)
                ValidateBorder((Border)section.SectPr[key]);

            return VisitorAction.Continue;
        }

        // FOSS

        /// <summary>
        /// Removes specific section global default values.
        /// </summary>
        /// <remarks>
        /// Open office does not handle these defaults correctly.
        /// </remarks>
        private static void RemoveLineNumberingDefaults(Section section)
        {
            foreach (int key in gLineNumberingAttrs)
            {
                if (object.Equals(section.SectPr.FetchAttr(key), SectPr.FetchDefaultAttr(key)))
                    section.SectPr.Remove(key);
            }
        }

        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            // Check whether paragraph list is a legacy list and update document list labels once if needed.
            if (!mListLabelsUpdated && IsSaveAction(DocumentValidatorActions.ConvertLegacyListFormatting))
            {
                if (paragraph.IsListItem)
                {
                    ListLevel level = paragraph.ListLevel;
                    // If LegacySpace is specified we need to know the length of the label text to calculate
                    // correct leftOfText distance.
                    // To calculate the length of the label of text we need to get its formatting
                    // that is why we need to call UpdateListLabels method.
                    if (level.Legacy && (level.LegacySpace != 0))
                    {
                        mSaveInfo.Document.UpdateListLabels();
                        mListLabelsUpdated = true;
                    }
                }
            }

            // WORDSNET-17459 The paragraph graphic extensions for ODT are located in extra namespace 'xmlns:loext'.
            // We should add this, if there are any paragraphs with such feature.
            mSaveInfo.HasParagraphGraphicsExtension |= paragraph.ParagraphBreakRunPr.Contains(FontAttr.EffectFill);

            ProcessParaId(paragraph);

            RemoveNilShading(paragraph.ParaPr, ParaAttr.Shading);

            if (paragraph.IsListItemOriginal || paragraph.IsListItemFinal)
                mListValidator.VisitParagraphStart(paragraph);

            mUnitConverter.VisitParagraphStart(paragraph);

            // WORDSNET-7015 Word removes floating attributes for paragraphs inside text boxes.
            // WORDSNET-12216 It does so even if the paragraph is inside a table.
            // Frames simply cannot be inside text boxes.
            bool isNestedInsideShape = (paragraph.GetAncestor(NodeType.Shape) != null);
            if (isNestedInsideShape)
                paragraph.ParaPr.RemoveFloatingAttrs();

            // Don't write HtmlBlockCollection if there are no paragraphs which are referenced to it.
            // AM. The primary reason is to reduce Gold files affected.
            if (!mSaveInfo.HasHtmlBlockReferences)
            {
                if (paragraph.ParaPr.Contains(ParaAttr.HtmlBlockId))
                    mSaveInfo.HasHtmlBlockReferences = true;
            }

            ValidateFramePr(paragraph);

            foreach (int key in ParaPr.BorderAttrs)
                ValidateBorder((Border)paragraph.ParaPr[key]);

            // WORDSNET-20040 Don't merge paragraphs in header/footer.
            if (!mStoryTypeStack.IsInHeaderOrFooter)
                ValidateDropCapPosition(paragraph);

            ValidateTabStop(paragraph);

            if (paragraph.ParaPr.Collapsed && mSaveInfo.IsPreIsoTransitionalWordFormat())
            {
                WarnOfUnsupportedByFormat(WarningType.MinorFormattingLoss, WarningStrings.NotSupportedCollapsed);
            }

            mRsidTable.Add(paragraph.ParaPr[ParaAttr.RsidP]);
            mRsidTable.Add(paragraph.ParagraphBreakRunPr[FontAttr.RsidR]);
            mRsidTable.Add(paragraph.ParagraphBreakRunPr[FontAttr.RsidRPr]);

            if (mProgressProcessor != null)
                mProgressProcessor.Step();

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Validates and collects unique paragraph identifiers.
        /// </summary>
        private void ProcessParaId(Paragraph para)
        {
            if (para.ParaId == 0)
                return;

            // Get list for current story type.
            HashSetGeneric<int> usedParaId = (HashSetGeneric<int>)mSaveInfo.UsedParaIdArray[(int)mStoryTypeStack.Current];

            if (usedParaId.Contains(para.ParaId))
            {
                // Reset duplicates.
                para.ParaId = 0;
            }
            else
            {
                usedParaId.Add(para.ParaId);
            }
        }

        /// <summary>
        /// Validates and collects unique comment durable identifiers.
        /// </summary>
        private void ProcessDurableId(Comment comment)
        {
            if (comment.DurableId == 0)
                return;

            if (mSaveInfo.UsedCommentDurableIds.Contains(comment.DurableId))
                comment.DurableId = 0; // Reset duplicates.
            else
                mSaveInfo.UsedCommentDurableIds.Add(comment.DurableId);
        }

        /// <summary>
        /// Validates and collects unique row identifiers.
        /// </summary>
        private void ProcessRowParaId(Row row)
        {
            if (row.ParaId == 0)
                return;

            if (mSaveInfo.UsedRowParaIds.Contains(row.ParaId))
            {
                // Generate new value.
                for (int i = row.ParaId + 1; i < int.MaxValue; i++)
                {
                    if (!mSaveInfo.UsedRowParaIds.Contains(i))
                    {
                        row.ParaId = i;
                        break;
                    }
                }
            }

            mSaveInfo.UsedRowParaIds.Add(row.ParaId);
        }

        private static void ValidateFramePr(Paragraph paragraph)
        {
            ParaPr paraPr = paragraph.ParaPr;

            // andrnosk: WORDSNET-7626 This code violates what is written in specification ECMA 376 (2.3.1.11),
            // but it seems MS Word behaves in this way.
            //
            // AM. We do it because FrameLeft + FrameHorizontalAlignment
            // should be merged into one complex attribute to avoid inheritance problems.
            // The same for FrameTop + FrameVerticalAlignment.
            if (paraPr.FrameLeft > 0)
                paraPr.FrameHorizontalAlignment = HorizontalAlignment.None;

            if (paraPr.FrameTop > 0)
                paraPr.FrameVerticalAlignment = VerticalAlignment.None;
        }

        private static void ValidateBorder(Border border)
        {
            if (border != null)
            {
                // WORDSNET-11509 System.ArgumentOutOfRangeException is thrown while saving Docx to Pdf.
                // DistanceFromText has 5 bits, we get only 5 bits
                // WORDSNET-25489 Update directly in RawDistanceFromText instead of DistanceFromText
                // to not affect Border.IsInherited flag.
                border.RawDistanceFromText = (int)border.DistanceFromText & Border.MaxDistanceFromText;
                if (border.LineWidth > Border.MaxLineWidth)
                    border.SetLineWidthSafe(Border.MaxLineWidth);; // WORDSNET-21911
            }
        }

        private static void ValidateTabStop(Paragraph paragraph)
        {
            if (paragraph.ParentSection == null)
                return;

            TabStopCollection tabStops = paragraph.ParaPr.TabStops;

            // WORDSNET-11458 Document.Save generates huge size html for Odt
            //Prepare position of TabStop. Position value is 16bit.
            if (tabStops != null)
            {
                for (int i = 0; i < tabStops.Count; i++)
                {
                    tabStops[i].PositionTwips = (short)(tabStops[i].PositionTwips & MaxTabStopPosition);
                }
            }
        }

        public override VisitorAction VisitBodyStart(Body body)
        {
            mStoryTypeStack.Push(body.StoryType);
            mUnitConverter.VisitBodyStart(body);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitBodyEnd(Body body)
        {
            mStoryTypeStack.Pop(body.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            mStoryTypeStack.Push(headerFooter.StoryType);
            mUnitConverter.VisitHeaderFooterStart(headerFooter);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterEnd(HeaderFooter headerFooter)
        {
            mStoryTypeStack.Pop(headerFooter.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentStart(Comment comment)
        {
            if (RemoveIfNotInMainStory(comment))
                return VisitorAction.SkipThisNode;

            mAnnotationValidator.VisitComment(comment);
            mStoryTypeStack.Push(comment.StoryType);

            if (comment.Done || (comment.ParentId != Comment.NoParent))
                OoxmlComplianceInfo.MarkAsHasDocxExtensions(mSaveInfo.Document);

            if (comment.DurableId != 0)
                OoxmlComplianceInfo.MarkAsHasDocxExtensionsOf(mSaveInfo.Document, MsWordVersionCore.Word2016);

            if (comment.IsDateTimeUtcDefined ||
                comment.HasReactions ||
                comment.IsIntelligentPlaceholder ||
                (mSaveInfo.Document.CompatibilityOptions.MswVersion >= MsWordVersionCore.Word2019))
            {
                OoxmlComplianceInfo.MarkAsHasDocxExtensionsOf(mSaveInfo.Document, MsWordVersionCore.Word2019);
            }

            ProcessDurableId(comment);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentEnd(Comment comment)
        {
            mStoryTypeStack.Pop(comment.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            if (RemoveIfNotInMainStory(commentRangeStart))
                return VisitorAction.SkipThisNode;

            return mAnnotationValidator.VisitCommentRangeStart(commentRangeStart);
        }

        public override VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            if (RemoveIfNotInMainStory(commentRangeEnd))
                return VisitorAction.SkipThisNode;

            return mAnnotationValidator.VisitCommentRangeEnd(commentRangeEnd);
        }

        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            // Similar to comments, footnote is an inline node and we have to check what story the footnote is in.
            if (mStoryTypeStack.Current != StoryType.MainText)
                throw new InvalidOperationException("Footnotes are only allowed inside the main text of the document.");

            // Whether the document has footnotes/endnotes is needed later for WordML export.
            if (footnote.FootnoteType == FootnoteType.Footnote)
                mSaveInfo.HasFootnotes = true;
            else
                mSaveInfo.HasEndnotes = true;

            footnote.EnsureMinimum();
            mStoryTypeStack.Push(footnote.StoryType);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFootnoteEnd(Footnote footnote)
        {
            mStoryTypeStack.Pop(footnote.StoryType);

            mFootnoteValidator.VisitFootnote(footnote);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellStart(Cell cell)
        {
            RemoveNilShading(cell.CellPr, CellAttr.Shading);

            return mTableValidator.VisitCellStart(cell);
        }

        public override VisitorAction VisitRowStart(Row row)
        {
            row.EnsureTableStyleIsValid();

            ProcessRowParaId(row);

            return mTableValidator.VisitRowStart(row);
        }

        public override VisitorAction VisitRowEnd(Row row)
        {
            return mTableValidator.VisitRowEnd(row);
        }

        public override VisitorAction VisitTableStart(Table table)
        {
            CheckIsoProperties(table);

            table.FixNestedTableLayout();

            return mTableValidator.VisitTableStart(table);
        }

        /// <summary>
        /// If the specified table has ISO/IEC 29500 properties defined, enforces the document to ISO format or
        /// generates a warning if not possible.
        /// </summary>
        private void CheckIsoProperties(Table table)
        {
            // Table.Title and Table.Description are supported only by ISO/IEC 29500 DOCX formats.
            // The properties are not taken directly from table to not affect styles by Table.FetchFirstRowAttr.
            // (The Table style is added into document styles if none yet after calling table.Title or table.Description.)
            if (table.FirstRow == null)
                return;

            TablePr tablePr = table.FirstRow.TablePr;
            if ((!StringUtil.HasChars(tablePr.Title) &&
                 !StringUtil.HasChars(tablePr.Description)) ||
                (!mSaveInfo.IsPreIsoTransitionalWordFormat() &&
                 (mSaveInfo.SaveFormat != SaveFormat.Odt) &&
                 (mSaveInfo.SaveFormat != SaveFormat.Ott)))
            {
                return;
            }

            if (mSaveInfo.IsOoxmlFormat && !IsUserDefinedCompliance)
            {
                // Enforce format to ISO.
                Document document = mSaveInfo.Document;
                if (document.ComplianceInfo == null)
                    document.ComplianceInfo = new OoxmlComplianceInfo();
                document.ComplianceInfo.MarkAsIsoTransitional();
            }
            else
            {
                // Generate warnings.
                if (StringUtil.HasChars(tablePr.Title))
                    WarnOfUnsupportedByFormat(WarningType.DataLoss, WarningStrings.NotSupportedTableTitle);
                if (StringUtil.HasChars(tablePr.Description))
                    WarnOfUnsupportedByFormat(WarningType.DataLoss, WarningStrings.NotSupportedTableDescription);
            }
        }

        public override VisitorAction VisitTableEnd(Table table)
        {
            // Do not re-calculate table grid until adjacent tables are joined.
            VisitorAction result = VisitorAction.Continue;
            Table combinedTable = mTableMerger.JoinAdjacentTablesDiscardGrids(table);
            if (combinedTable != null)
            {
                // WORDSNET-21630 Update table layout before direct attributes will be saved to "SysDirectAttrs".
                DocumentPostLoader.UpdateNestedTableGrids(combinedTable, NestedTables, false);

                // WORDSNET-5262 Expander should be invoked at table end to preserve any resilience corrections made (for example WORDSNET-905).
                TableFormattingExpander tableFormattingExpander =
                    mTableFormattingExpanders.GetValueOrNull(combinedTable.Document);

                if (tableFormattingExpander != null)
                    tableFormattingExpander.Expand(combinedTable);

                // WORDSNET-20640 Validate vertical position of the table.
                // Note, Word does this for all legacy formats, except of RTF.
                if (mSaveInfo.IsLegacyFormat && (mSaveInfo.SaveFormat != SaveFormat.Rtf))
                    ValidateTableVerticalPosition(combinedTable);

                result = mTableValidator.VisitTableEnd(combinedTable);
            }

            return result;
        }

        public override VisitorAction VisitShapeStart(Shape shape)
        {
            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                ForceShowingPlaceholderIfNeeded();

                // WORDSNET-24308 Update compliance to properly write required namespaces.
                if (mSaveInfo.IsOoxmlFormat)
                {
                    if (shape.RunPr.AlternateContent != null)
                        OoxmlComplianceInfo.MarkAsHasDrawingExtensions(mSaveInfo.Document);
                }
            }

            VisitorAction result = mShapeValidator.VisitShapeStart(shape, mStoryTypeStack);

            if (result == VisitorAction.Continue)
                mStoryTypeStack.Push(shape.StoryType);

            mRsidTable.Add(shape.RunPr.RsidR);

            return result;
        }

        public override VisitorAction VisitShapeEnd(Shape shape)
        {
            VisitorAction result = mShapeValidator.VisitShapeEnd(shape);

            mStoryTypeStack.Pop(shape.StoryType);
            return result;
        }

        public override VisitorAction VisitGroupShapeStart(GroupShape groupShape)
        {
            mRsidTable.Add(groupShape.RunPr.RsidR);

            return mShapeValidator.VisitGroupShapeStart(groupShape, mStoryTypeStack);
        }

        public override VisitorAction VisitGroupShapeEnd(GroupShape groupShape)
        {
            return mShapeValidator.VisitGroupShapeEnd(groupShape);
        }

        public override VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            VisitorAction result = mOfficeMathToShapeConverter.VisitOfficeMathStart(officeMath, this);
            return (result != VisitorAction.SkipThisNode) ?
                mMathValidator.VisitOfficeMathStart(officeMath) : // Only do validation of OfficeMath node if we didn't turn it into a shape.
                result;
        }

        public override VisitorAction VisitOfficeMathEnd(OfficeMath officeMath)
        {
            mOfficeMathToShapeConverter.VisitOfficeMathEnd(officeMath, this);
            return mMathValidator.VisitOfficeMathEnd(officeMath);
        }

        public override VisitorAction VisitRun(Run run)
        {
            NilBorderHack(run);

            // WORDSNET-27050 Handle broken surrogate pair.
            if (IsBrokenSurrogatePair(run))
            {
                Run nextRun = (Run)run.NextSibling;

                run.Text = string.Format("{0}{1}", run.Text, nextRun.Text[0]);
                nextRun.Text = nextRun.Text.Remove(0, 1);
            }

            if (run.RunPr.Contains(FontAttr.Ruby))
            {
                mRubyConverter.Convert(run);
            }
            else
            {
                // Runs with empty text cause rendering to throw exception. This is a quick fix. I think good enough.
                if (run.Text == "")
                    run.Remove();
            }

            if (mSaveInfo.SaveOptions.UpdateAmbiguousTextFont)
                ComplexScriptRunUpdater.DoProcess(run);

            mFootnoteValidator.VisitRun(run, mStoryTypeStack);

            mRsidTable.Add(run.RunPr.RsidR);
            mRsidTable.Add(run.RunPr.RsidRPr);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            if (fieldStart.FieldType == FieldType.FieldHyperlink)
                ForceShowingPlaceholderIfNeeded();

            mFieldValidator.VisitFieldStart(fieldStart);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            mFieldValidator.VisitFieldEnd(fieldEnd);

            // WORDSNET-16333 Convert EQ field to OfficeMath if required.
            if (fieldEnd.FieldType == FieldType.FieldEquation)
                mEQFieldConverter.Convert(fieldEnd.GetField());

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            // Normally, OLE objects are shapes and not stored in fields in the model.
            // There are only a few cases where linked OLE objects are stored in a field.
            // Because these are linked objects only, add them to AllOleObjects.
            OleObject oleObject = fieldSeparator.OleObject;
            if (oleObject != null)
                mSaveInfo.AllOleObjects[OleUtil.GetMsWordId(oleObject)] = oleObject.Data;

            mFieldValidator.VisitFieldSeparator(fieldSeparator);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFormField(FormField formField)
        {
            ForceShowingPlaceholderIfNeeded();

            mFieldValidator.VisitFormField(formField);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Informs the acceptor node if deleted nodes should be given to the visitor.
        /// </summary>
        /// <remarks>
        /// DocumentValidator does not visit deleted nodes.
        /// </remarks>
        internal override bool VisitsDeletedNodes
        {
            get { return false; }
        }

        public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            // WORDSNET-25159 Bookmark part might be removed during table validation.
            if (IsInsideEmptyTable(bookmarkStart))
                return VisitorAction.Continue;

            ForceShowingPlaceholderIfNeeded();

            mBookmarkValidator.VisitBookmarkStart(bookmarkStart, mStoryTypeStack.Current);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            // WORDSNET-25159 Bookmark part might be removed during table validation.
            if (IsInsideEmptyTable(bookmarkEnd))
                return VisitorAction.Continue;

            mBookmarkValidator.VisitBookmarkEnd(bookmarkEnd, mStoryTypeStack.Current);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitEditableRangeStart(EditableRangeStart editableRangeStart)
        {
            return mAnnotationValidator.VisitEditableRangeStart(editableRangeStart);
        }

        public override VisitorAction VisitEditableRangeEnd(EditableRangeEnd editableRangeEnd)
        {
            return mAnnotationValidator.VisitEditableRangeEnd(editableRangeEnd);
        }

        public override VisitorAction VisitSmartTagStart(SmartTag smartTag)
        {
            // WORDSNET-7105 An RTF document has smart tags without namespaces, opening it in Word is okay.
            // But when we convert to DOC or DOCX, then the it cannot be opened in MS Word.
            // It seems uri is required, so let's provide default one.
            if (!StringUtil.HasChars(smartTag.Uri))
                smartTag.Uri = "urn:schemas-microsoft-com:office:smarttags";

            AddSmartTagType(smartTag);

            // Smart tag namespaces do not seem to be added to attached schemas by MS Word, we do the same.
            AddXmlNamespace(smartTag.Uri, false, true);
            AddXmlNamespace(smartTag.Properties, false, true);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagStart(StructuredDocumentTag sdt)
        {
            ValidateStructuredDocumentTagStart(sdt);

            mSdtIds.Add(sdt.Id);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Checks the structured document tag and makes any necessary changes or issues warnings if needed.
        /// </summary>
        private void ValidateStructuredDocumentTagStart(StructuredDocumentTag sdt)
        {
            // Custom XML validator should be called before any other code in this method.
            mCustomXmlValidator.VisitStructuredDocumentTagStart(sdt, mXmlMappingContext);

            ForceShowingPlaceholderIfNeeded();
            mSdtStack.Push(sdt);

            // When saving to OOXML format we need compliance data in writers and DocxBuilder to properly construct document.
            if (mSaveInfo.IsOoxmlFormat && sdt.IsDocxExtension)
            {
                if (!IsUserDefinedCompliance)
                    OoxmlComplianceInfo.MarkAsHasDocxExtensions(mSaveInfo.Document);

                // MS Word asks to remove mapping of a rich-text SDT on saving if compatibility mode is less than 15.
                // It does not happen with other [MS-DOCX] extensions.
                CompatibilityOptions compatibility = mSaveInfo.Document.CompatibilityOptions;
                if ((sdt.SdtType == SdtType.RichText) && !sdt.XmlMapping.IsEmpty &&
                    !compatibility.IsOptimized && (compatibility.MswVersion < MsWordVersionCore.Word2013))
                {
                    compatibility.OptimizeFor(MsWordVersion.Word2013);
                }

                if (!sdt.XmlMapping.IsEmpty && StringUtil.HasChars(sdt.XmlMapping.StoreItemChecksum))
                    OoxmlComplianceInfo.MarkAsHasDocxExtensionsOf(mSaveInfo.Document, MsWordVersionCore.Word2019);
            }
            // In contrast with checkbox SDT, we do not allow entity picker creation. So do not change compliance,
            // just generate a warning. The same is with Web Extensions and Appearance.
            if (mSaveInfo.IsPreIsoTransitionalWordFormat())
            {
                if (sdt.SdtType == SdtType.EntityPicker)
                    WarnOfUnsupportedByFormat(WarningType.DataLoss, WarningStrings.NotSupportedEntityPicker);

                if (sdt.WebExtensionRelationship != SdtWebExtensionRelationship.None)
                    WarnOfUnsupportedByFormat(WarningType.DataLoss, WarningStrings.NotSupportedWebExtensions);

                if (sdt.Appearance != SdtAppearance.Default)
                    WarnOfUnsupportedByFormat(WarningType.MinorFormattingLoss, WarningStrings.NotSupportedSdtAppearance);
            }

            // WORDSNET-25482 Update placeholder reference regardless of edit revisions.
            sdt.UpdatePlaceholderReference(false);

            // WORDSNET-16209 Do not update deleted SDTs.
            if (((ITrackableNode)sdt).DeleteRevision == null)
            {
                // WORDSNET-23287, WORDSNET-23487 Add required namespaces when checksum should be written.
                if (!sdt.XmlMapping.IsEmpty && StringUtil.HasChars(sdt.XmlMapping.StoreItemChecksum))
                    OoxmlComplianceInfo.MarkAsHasDocxExtensionsOf(mSaveInfo.Document, MsWordVersionCore.Word2019);

                if (mSaveInfo.IsOoxmlFormat)
                    ValidateSdtContent(sdt);
            }
        }

        /// <summary>
        /// Validates contents of the specified SDT and fixes it if necessary.
        /// </summary>
        private static void ValidateSdtContent(StructuredDocumentTag sdt)
        {
            // WORDSNET-23826 If an SDT ends with a table, all the cells of the last row of which are merged vertically,
            // MS Word gives an error when opening the document. Append an empty paragraph in this case.

            Table table = sdt.LastChild as Table;
            if (table == null)
                return;

            foreach (Cell cell in table.LastRow.Cells)
            {
                if (cell.CellFormat.VerticalMerge != CellMerge.Previous)
                    return;
            }

            sdt.AppendChild(new Paragraph(sdt.Document));
        }

        public override VisitorAction VisitStructuredDocumentTagEnd(StructuredDocumentTag sdt)
        {
            mSdtStack.Pop();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagRangeStart(StructuredDocumentTagRangeStart sdtRangeStart)
        {
            ValidateStructuredDocumentTagStart(sdtRangeStart.InternalSdt);
            VisitStructuredDocumentTagEnd(sdtRangeStart.InternalSdt);

            mSdtRangeStack.Push(sdtRangeStart);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagRangeEnd(StructuredDocumentTagRangeEnd sdtRangeEnd)
        {
            ValidateStructuredDocumentTagRangeEnd(sdtRangeEnd);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Checks the structured document tag range end and regenerates range ID if necessary. If there is an invalid
        /// SDT range start/end sequence, the invalid nodes are removed.
        /// </summary>
        private void ValidateStructuredDocumentTagRangeEnd(StructuredDocumentTagRangeEnd sdtRangeEnd)
        {
            if (!IsOpenSdtRange(sdtRangeEnd.Id))
            {
                // Wrong SDT range nesting: remove the range end.
                sdtRangeEnd.Remove();
                return;
            }

            do
            {
                StructuredDocumentTagRangeStart start = mSdtRangeStack.Pop();
                if (start.Id == sdtRangeEnd.Id)
                {
                    if (mSdtIds.Contains(sdtRangeEnd.Id))
                    {
                        start.SetId(UniqueIdManager.GenerateInteger());
                        sdtRangeEnd.SetIdInternal(start.Id);
                    }

                    mSdtIds.Add(start.Id);
                    break;
                }

                // Wrong SDT range nesting: remove the range start.
                start.Remove();
            }
            while (mSdtRangeStack.Count > 0);
        }

        /// <summary>
        /// Gets a flag indicating whether an SDT range start node with the specified identifier exists in the open SDT
        /// range stack.
        /// </summary>
        private bool IsOpenSdtRange(int rangeId)
        {
            foreach (StructuredDocumentTagRangeStart start in mSdtRangeStack)
            {
                if (start.Id == rangeId)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a smart tag to the collection of smart tag types in <see cref="SaveInfo"/>.
        /// We only need to store one instance to provide us with uri and element name for writing later on.
        /// </summary>
        private void AddSmartTagType(SmartTag smartTag)
        {
            string key = smartTag.Uri + smartTag.Element;
            mSaveInfo.SmartTagTypes[key] = smartTag;
        }

        /// <summary>
        /// Adds an XML namespace to the collection of namespaces in <see cref="SaveInfo"/> if this namespace has not yet been encountered.
        /// </summary>
        /// <param name="uri">The namespace used.</param>
        /// <param name="isAttachedSchema">True if this namespace is also to be added to the list of attached schemas for DOCX.</param>
        /// <param name="isSmartTag">True if origin of namespace is a SmartTag.</param>
        private void AddXmlNamespace(string uri, bool isAttachedSchema, bool isSmartTag)
        {
            if (!StringUtil.HasChars(uri))
                return;

            if (!mSaveInfo.XmlNamespaces.ContainsKey(uri))
            {
                // It is enough to just use the count as an id for the entry.
                // But the ID=0 is reserved by MS Word, it seems to indicate "the parent element's" namespace.
                int id = mSaveInfo.XmlNamespaces.Count + 1;
                mSaveInfo.XmlNamespaces.Add(uri, id);
                mSaveInfo.IsSmartTagOriginated.Add(id, isSmartTag);
            }

            if (isAttachedSchema)
                mSaveInfo.AttachedSchemas[uri] = uri;
        }

        private void AddXmlNamespace(CustomXmlPropertyCollection properties, bool isAttachedSchema, bool isSmartTag)
        {
            foreach (CustomXmlProperty prop in properties)
                AddXmlNamespace(prop.Uri, isAttachedSchema, isSmartTag);
        }

        /// <summary>
        /// If the node is not in the main text story, removes the node and returns true.
        /// </summary>
        private bool RemoveIfNotInMainStory(Node node)
        {
            if (mStoryTypeStack.Current != StoryType.MainText)
            {
                node.Remove();
                Warn(WarningType.DataLoss, WarningSource.Validator,
                string.Format(WarningStrings.DocumentValidatorWrongStory, Node.NodeTypeToString(node.NodeType)));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Validates MS Word customization (allocated commands, key maps and toolbars).
        /// </summary>
        /// <param name="doc"></param>
        private void ValidateCustomization(Document doc)
        {
            if (doc.AllocatedCommands == null)
                return;

            foreach (AllocatedCommand command in doc.AllocatedCommands)
            {
                // None Fci value is not allowed.
                if (command.FciBasedOn == FixedCommandIdentifier.None)
                {
                    Warn(WarningType.DataLoss, WarningSource.Unknown, WarningStrings.DocumentValidatorAllocatedCommandInvalid);

                    // Unfortunately we can't just remove invalid command because there can be toolbar referring to this command.
                    // Currently we read/write toolbar customization only as raw bytes.
                    // So there is just quick workaround - we change invalid command to "ApplyStyle Normal" command.
                    command.FciBasedOn = FixedCommandIdentifier.ApplyStyleName;
                    command.ArgValue = new byte[] { 0x01, 0x00 /* style by sti */, 0x00, 0x00, 0x00, 0x00 /* sti of Normal */};
                }
            }
        }

        /// <summary>
        /// Returns true if all bits of the specified save actions are set in the current save operation.
        /// </summary>
        private bool IsSaveAction(DocumentValidatorActions actions)
        {
            return (mSaveActions & actions) == actions;
        }

        /// <summary>
        /// Process placeholder usage information and remove unused placeholders.
        /// </summary>
        /// <remarks>
        /// This actually affects a few existing GOLD documents, that contain lost placeholders. Lost placeholders get removed by this function, because their
        /// reference count tracked in SdtPlaceholderManager is 0.
        /// This follows our logic about placeholders: after we delete Sdt, we don't delete placeholders right away, but do this here in validator.
        /// So here in DocumentValidator we can not distinguish between AW-created placeholders and non-AW created placeholders.
        /// </remarks>
        private void RemoveUnusedSdtPlaceholders(Document doc)
        {
            SdtPlaceholderManager placeholderManager = doc.SdtPlaceholderManager;
            if (!placeholderManager.IsGlossaryEmpty)
            {
                GlossaryDocument glossary = doc.GlossaryDocument;
                if ((glossary != null) && !glossary.SdtPlaceholderManager.IsGlossaryEmpty)
                    placeholderManager.AppendPlaceholderReferences(glossary.SdtPlaceholderManager.PlaceholdersUsed);

                if (placeholderManager.RemoveUnusedPlaceholders())
                    Warn(WarningType.DataLoss, WarningSource.Validator, WarningStrings.DocumentValidatorSdtPlaceholderUnused);
            }
        }

        /// <summary>
        /// Concatenates DropCap frame paragraph with next paragraph if DropCapPosition equals None.
        /// </summary>
        private static void ValidateDropCapPosition(Paragraph paragraph)
        {
            ParaPr paraPr = paragraph.ParaPr;
            if (paraPr.ContainsKey(ParaAttr.DropCapPosition) && (paraPr.DropCapPosition == DropCapPosition.None))
            {
                Paragraph nextPara = (Paragraph)paragraph.NextSiblingOfType(NodeType.Paragraph);

                if (nextPara != null)
                {
                    while (paragraph.HasChildNodes)
                    {
                        Node nextNode = paragraph.FirstChild;
                        // We have to restore run formatting, that is why copy RunPr from
                        // the first run of the next paragraph.
                        if ((nextNode != null) && (nextNode.NodeType == NodeType.Run))
                        {
                            ((Run)nextNode).RunPr = nextPara.FirstRun != null
                                                        ? nextPara.FirstRun.RunPr
                                                        : nextPara.ParagraphBreakRunPr;
                        }

                        nextPara.PrependChild(nextNode);
                    }
                    paragraph.Remove();
                }
            }
        }

        private static void RemoveNilShading(AttrCollection attrs, int key)
        {
            Shading shading = (Shading)attrs[key];
            if ((shading != null) && (shading.Texture == TextureIndex.TextureNil))
                attrs.Remove(key);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType type, WarningSource source, string description)
        {
            if (WarningCallback != null)
                WarningCallback.Warning(new WarningInfo(type, source, description));
        }

        /// <summary>
        /// Logs a warning of a feature, which is not supported by saving format, to the user-provided warning callback.
        /// </summary>
        /// <param name="warningType">
        /// Generating warning type.
        /// </param>
        /// <param name="description">
        /// Warning text that may contain a format item, which will be replaced with saving format name.
        /// </param>
        private void WarnOfUnsupportedByFormat(WarningType warningType, string description)
        {
            string format = mSaveInfo.SaveFormat.ToString();
            if (mSaveInfo.IsOoxmlFormat)
            {
                switch (OoxmlCompliance)
                {
                    case OoxmlComplianceCore.Ecma376:
                        format = Ecma376FormatTitle;
                        break;
                    case OoxmlComplianceCore.IsoTransitional:
                        format = "OOXML ISO/IEC 29500 Transitional";
                        break;
                    case OoxmlComplianceCore.IsoStrict:
                        format = "OOXML ISO/IEC 29500 Strict";
                        break;
                    default:
                        Debug.Assert(false, "Unknown value.");
                        break;
                }
            }

            Warn(warningType, WarningSource.Validator, string.Format(description, format));
        }

        /// <summary>
        /// Logs a warning of a setting that is not supported by saving format.
        /// </summary>
        private void WarnOfUnsupportedSetting(string settingName)
        {
            WarnOfUnsupportedByFormat(WarningType.DataLoss, string.Format(WarningStrings.NotSupportedSetting, settingName));
        }

        /// <summary>
        /// Logs a warning of a setting that is not supported by Aspose.Words for saving format.
        /// </summary>
        private void WarnOfUnsupportedSettingByAw(string settingName)
        {
            WarnOfUnsupportedByFormat(WarningType.DataLoss, string.Format(WarningStrings.NotSupportedSettingByAw, settingName));
        }

        /// <summary>
        /// Forces ShowingPlaceholder property for parent SDT.
        /// </summary>
        /// <remarks>
        /// WORDSNET-17319 For unknown reason Word is unable to open document if plain text SDT contains some nodes and
        /// seems this can be fixed by writing ShowingPlaceholder property.
        /// </remarks>
        private void ForceShowingPlaceholderIfNeeded()
        {
            if (mSdtStack.Count > 0)
                mSaveInfo.ForceShowingPlaceholder.Add(mSdtStack.Peek());
        }

        /// <summary>
        /// Validates vertical position of a specified table.
        /// </summary>
        private static void ValidateTableVerticalPosition(Table table)
        {
            Row firstRow = table.FirstRow;
            if (firstRow == null)
                return;

            // Check if table is vertically anchored to a paragraph, then this paragraph must exist.
            // If it is not so, then create a new one before the table.
            if (firstRow.TablePr.RelativeVerticalPosition == RelativeVerticalPosition.Paragraph)
            {
                Node prevNode = table.PreviousNonAnnotationSibling;
                if ((prevNode != null) && (prevNode.NodeType != NodeType.Paragraph))
                {
                    Paragraph paragraph = new Paragraph(table.Document);
                    table.ParentNode.Insert(paragraph, prevNode, true);
                    paragraph.ParagraphBreakRunPr.Hidden = AttrBoolEx.True;
                }
            }
        }

        /// <summary>
        /// Removes Nil border from run.
        /// </summary>
        /// <remarks>
        /// This is hack solution for WORDSNET-22140 and WORDSNET-23785
        /// Actually seems we need to make Nil border public eventually.
        /// </remarks>
        private void NilBorderHack(Run run)
        {
            if (mSaveInfo.SaveFormat == SaveFormat.Docx)
            {
                if (run.RunPr.ContainsKey(FontAttr.Border))
                {
                    Border border = run.RunPr.Border;

                    if (border.IsNil)
                        border.IsNil = false;
                }
            }
        }

        /// <summary>
        /// Checks if sequential runs contains broken surrogate pair.
        /// </summary>
        private static bool IsBrokenSurrogatePair(Run run)
        {
            // Check if run ends with high surrogate.
            if ((run.Text.Length == 0) || !char.IsHighSurrogate(run.Text[run.Text.Length - 1]))
                return false;

            Run nextRun = run.NextSibling as Run;

            if (nextRun == null)
                return false;

            // Check if next run start with low surrogate.
            return (nextRun.Text.Length > 0) && char.IsLowSurrogate(nextRun.Text[0]);
        }

        /// <summary>
        /// Maps converted OfficeMath objects to original ShapeBase objects
        /// when document is saved to format which doesn't support OMML.
        /// Keys are instances of <see cref="OfficeMath"/>.
        /// Values are instances of <see cref="Shape"/>.
        /// </summary>
        internal IDictionary<OfficeMath, Shape> ConvertedOfficeMathToShape
        {
            get { return mOfficeMathToShapeConverter.ConvertedOfficeMathToShape; }
        }

        /// <summary>
        /// Holds Shape converted from OfficeMath when document is saved to format which doesn't support OfficeMath.
        /// </summary>
        internal IDictionary<ShapeBase, OfficeMath> ShapeToConvertedOfficeMath
        {
            get { return mOfficeMathToShapeConverter.ShapeToConvertedOfficeMath; }
        }

        private IWarningCallback WarningCallback
        {
            get { return mSaveInfo.Document.WarningCallback; }
        }

        /// <summary>
        /// Specifies the OOXML version for the output document on saving as DOCX formats.
        /// </summary>
        private OoxmlComplianceCore OoxmlCompliance
        {
            get
            {
                return mSaveInfo.OoxmlCompliance;
            }
        }

        /// <summary>
        /// Gets a flag indicating that user has set compliance explicitly.
        /// </summary>
        private bool IsUserDefinedCompliance
        {
            get
            {
                OoxmlSaveOptions ooxmlSaveOptions = mSaveInfo.SaveOptions as OoxmlSaveOptions;
                return (ooxmlSaveOptions != null) && ooxmlSaveOptions.UserSetCompliance;
            }
        }

        /// <summary>
        /// Returns true if we need to update font info collection to used fonts.
        /// </summary>
        private bool IsUpdateFontInfos
        {
            get
            {
                // WORDSNET-14322 In addition, let's update font infos when embed fonts into DOCX formats.
                // It looks like MS Word behaves similarly.
                return IsSaveAction(DocumentValidatorActions.UpdateFontInfo) ||
                    (mSaveInfo.IsDocxFormat && mSaveInfo.Document.DocPr.EmbedTrueTypeFonts);
            }
        }

        private static void EnsureFootnoteSeparator(DocumentBase doc, FootnoteSeparatorType footnoteSeparatorType)
        {
            if (doc.FootnoteSeparators[footnoteSeparatorType] != null)
                return;

            FootnoteSeparator footnoteSeparator = new FootnoteSeparator(doc, footnoteSeparatorType);

            char controlChar = (footnoteSeparatorType == FootnoteSeparatorType.FootnoteContinuationSeparator) ||
                               (footnoteSeparatorType == FootnoteSeparatorType.EndnoteContinuationSeparator)
                ? ControlChar.FootnoteContinuationChar
                : ControlChar.FootnoteSeparatorChar;

            // The SpaceAfter/LineSpacing values are taken from the Word then it adds the missing footnotes.
            Paragraph para = footnoteSeparator.AppendChild(new Paragraph(doc));

            para.ParaPr.SpaceAfter = 0;
            para.ParaPr.LineSpacing = 240;
            para.AppendChild(new SpecialChar(doc, controlChar, new RunPr()));

            doc.FootnoteSeparators.Add(footnoteSeparator);
        }

        /// <summary>
        /// Checks if node is located inside empty table.
        /// </summary>
        private static bool IsInsideEmptyTable(Node node)
        {
            Table parentTable = node.ParentNode as Table;

            return (parentTable != null) && (parentTable.Rows.Count == 0);
        }

        private SavingProgressProcessor mProgressProcessor;

        // *** These fields are common for the whole document.
        private SaveInfo mSaveInfo;
        private ShapeValidator mShapeValidator;
        private CustomXmlValidator mCustomXmlValidator;
        /// <summary>
        /// Handle all pre-save ISO29500 specifics in a dedicated class.
        /// This mainly includes manipulating model values to enforce compliance to ISO29500 or older formats.
        /// </summary>
        private Iso29500ComplianceEnforcer mIso29500ComplianceEnforcer;
        /// <summary>
        /// Specifies the actions/flags that the validator must take into account because it is saving in a particular save format.
        /// </summary>
        private DocumentValidatorActions mSaveActions;

        // *** These fields are reset for the main document and then for the glossary document.
        private BookmarkValidator mBookmarkValidator;
        private AnnotationValidator mAnnotationValidator;
        private FieldValidator mFieldValidator;
        private MathValidator mMathValidator;
        private TableValidator mTableValidator;
        private ListValidator mListValidator;
        private FootnoteValidator mFootnoteValidator;

        /// <summary>
        /// Font names used in the document.
        /// </summary>
        private ISetGeneric<string> mUsedFontNames;

        /// <summary>
        /// Maintains a stack of story types. Allows to detect some invalid model structures,
        /// such as a comment object outside of the main story.
        /// </summary>
        private StoryTypeStack mStoryTypeStack;

        /// <summary>
        /// Collection of TableFormattingExpander objects used to expand/revert in the main document and glossary document.
        /// </summary>
        private Dictionary<DocumentBase, TableFormattingExpander> mTableFormattingExpanders;

        /// <summary>
        /// Converts units spacing/indents into twips spacing/indents.
        /// </summary>
        private UnitConverter mUnitConverter;

        /// <summary>
        /// Knows when to convert office math to shapes.
        /// </summary>
        private OfficeMathToShapeConverter mOfficeMathToShapeConverter;
        /// <summary>
        /// List of adjacent tables which will be merged at document end.
        /// </summary>
        private readonly TableMerger mTableMerger = new TableMerger();

        /// <summary>
        /// Stores nested tables stack for the grid update.
        /// </summary>
        private Stack<Table> NestedTables
        {
            get { return mNestedTables; }
        }

        private readonly Stack<Table> mNestedTables = new Stack<Table>();

        private RubyConverter mRubyConverter;

        private EQFieldConverter mEQFieldConverter;

        private RsidTable mRsidTable;

        private readonly Stack<StructuredDocumentTag> mSdtStack = new Stack<StructuredDocumentTag>();
        private readonly HashSetGeneric<int> mSdtIds = new HashSetGeneric<int>();
        private readonly Stack<StructuredDocumentTagRangeStart> mSdtRangeStack =
            new Stack<StructuredDocumentTagRangeStart>();

        private XmlMappingContext mXmlMappingContext;

        /// <summary>
        /// Indicates list label update process has been executed.
        /// </summary>
        private bool mListLabelsUpdated;

        internal const string Ecma376FormatTitle = "OOXML ECMA-376";

        // [MS-DOC]: Word Binary File Format (.doc) Structure Specification
        // The XAS value is a 16-bit signed integer that specifies horizontal distance in twips. This value MUST
        // be greater than or equal to -31680 and less than or equal to 31680.
        private const int MaxTabStopPosition = 0xFFFF;

        private static readonly int[] gLineNumberingAttrs = new int[]
        {
            SectAttr.LineStartingNumber,
            SectAttr.LineNumberDistanceFromText,
            SectAttr.LineNumberCountBy,
            SectAttr.LineNumberRestartMode
        };

        private static DocumentValidatorActions FetchSaveActions(SaveFormat saveFormat)
        {
            DocumentValidatorActions result;

            if (!gSaveActions.TryGetValue(saveFormat, out result))
                throw new ArgumentOutOfRangeException("saveFormat");

            return result;
        }

        static DocumentValidator()
        {
            const DocumentValidatorActions docActions =
                DocumentValidatorActions.ExpandTableStyles |        // Although we support table styles in DOC, we need to write both expanded and direct, therefore expand.
                DocumentValidatorActions.ConvertDmlPicturesToVml |  // DOC does not support DML.
                DocumentValidatorActions.ExplicitAutoPreferredWidth |
                DocumentValidatorActions.WordTableLimits;

            gSaveActions.Add(SaveFormat.Doc, docActions);
            gSaveActions.Add(SaveFormat.Dot, docActions);

            const DocumentValidatorActions docxActions =
                DocumentValidatorActions.NormalizeHorizontalMerge |
                DocumentValidatorActions.WordTableLimits;

            gSaveActions.Add(SaveFormat.Docx, docxActions);
            gSaveActions.Add(SaveFormat.Dotx, docxActions);
            gSaveActions.Add(SaveFormat.Docm, docxActions);
            gSaveActions.Add(SaveFormat.Dotm, docxActions);

            const DocumentValidatorActions fopcActions =
                DocumentValidatorActions.NormalizeHorizontalMerge |
                DocumentValidatorActions.WordTableLimits;
            gSaveActions.Add(SaveFormat.FlatOpc, fopcActions);
            gSaveActions.Add(SaveFormat.FlatOpcMacroEnabled, fopcActions);
            gSaveActions.Add(SaveFormat.FlatOpcTemplate, fopcActions);
            gSaveActions.Add(SaveFormat.FlatOpcTemplateMacroEnabled, fopcActions);

            const DocumentValidatorActions rtfActions =
                DocumentValidatorActions.ExpandTableStyles | // We don't support table styles in RTF and need to expand.
                DocumentValidatorActions.ConvertDmlPicturesToVml | // RTF does not support DML.
                DocumentValidatorActions.UpdateListLabels | // We want to be nice to users and write list labels here.
                DocumentValidatorActions.ExplicitAutoPreferredWidth;

            // AM. It is interesting that Word allows table to have more than 63 cells for RTF format.
            gSaveActions.Add(SaveFormat.Rtf, rtfActions);

            // We support table styles in WordML hence no need to expand them.
            const DocumentValidatorActions wmlActions =
                DocumentValidatorActions.ConvertDmlPicturesToVml |  // WML does not support DML.
                DocumentValidatorActions.UpdateListLabels |         // We want to be nice to users and write list labels here.
                DocumentValidatorActions.NormalizeHorizontalMerge |
                DocumentValidatorActions.WordTableLimits;
            gSaveActions.Add(SaveFormat.WordML, wmlActions);

            // RK I think expanding default is not necessary here because they are resolved during normal attribute expansion. Should we try?
            gSaveActions.Add(SaveFormat.Pdf, DocumentValidatorActions.Rendering |
                                             DocumentValidatorActions.LongBookmarkNames);
            gSaveActions.Add(SaveFormat.Xps, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.OpenXps, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.XamlFixed, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.Svg, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.HtmlFixed, DocumentValidatorActions.Rendering);

            gSaveActions.Add(SaveFormat.Tiff, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.Png, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.Bmp, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.Emf, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.Jpeg, DocumentValidatorActions.Rendering);
            gSaveActions.Add(SaveFormat.Gif, DocumentValidatorActions.Rendering);
#if NETSTANDARD || NET462_OR_GREATER || JAVA
            gSaveActions.Add(SaveFormat.WebP, DocumentValidatorActions.Rendering);
#endif
            // When saving to a HTML-based format, DML shapes are handled by the HTML writer itself and the document validator
            // must not replace them with VML fallback shapes.
            const DocumentValidatorActions htmlBasedActions =
                DocumentValidatorActions.NonMSWordFormat & ~DocumentValidatorActions.ConvertDmlPicturesToVml;
            gSaveActions.Add(SaveFormat.Html, htmlBasedActions);
            gSaveActions.Add(SaveFormat.Mhtml, htmlBasedActions);
            gSaveActions.Add(SaveFormat.Epub, htmlBasedActions);
            gSaveActions.Add(SaveFormat.Azw3, htmlBasedActions);
            gSaveActions.Add(SaveFormat.Mobi, htmlBasedActions);

            const DocumentValidatorActions odtActions = DocumentValidatorActions.NonMSWordFormat |
                DocumentValidatorActions.UpdateFontInfo |
                DocumentValidatorActions.NormalizeHorizontalMerge;
            gSaveActions.Add(SaveFormat.Odt, odtActions);
            gSaveActions.Add(SaveFormat.Ott, odtActions);

            gSaveActions.Add(SaveFormat.Text, DocumentValidatorActions.NonMSWordFormat);
            gSaveActions.Add(SaveFormat.XamlFlow, DocumentValidatorActions.NonMSWordFormat);
            gSaveActions.Add(SaveFormat.XamlFlowPack, DocumentValidatorActions.NonMSWordFormat);
            gSaveActions.Add(SaveFormat.Markdown, DocumentValidatorActions.NonMSWordFormat |
                                                  DocumentValidatorActions.LongBookmarkNames);

            const DocumentValidatorActions doclingActions =
                DocumentValidatorActions.ExpandTableStyles |
                DocumentValidatorActions.ConvertLegacyListFormatting |
                DocumentValidatorActions.UpdateListLabels |
                DocumentValidatorActions.NormalizeHorizontalMerge;

            gSaveActions.Add(SaveFormat.Docling, doclingActions);
        }

        /// <summary>
        /// Key is <see cref="SaveFormat"/>, value is <see cref="DocumentValidatorActions"/>.
        /// </summary>
        private static readonly Dictionary<SaveFormat, DocumentValidatorActions> gSaveActions =
            new Dictionary<SaveFormat, DocumentValidatorActions>();
    }
}
