// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2007 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Reflection;
using Aspose.Collections;
using Aspose.Fonts;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Ole.Ole2;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Words.Themes;

namespace Aspose.Words.Validation
{
    /// <summary>
    /// This class is invoked just after a document was loaded.
    /// Basically, it performs "cosmetic" validation tasks that are common for different readers.
    /// For the detailed list of tasks performed see code.
    /// </summary>
    internal class DocumentPostLoader : DocumentVisitor
    {
        internal void Execute(Document doc, LoadOptions loadOptions, LoadFormat detectedFormat)
        {
            mLoadOptions = loadOptions;

            // WORDSNET-16401 The language preferences are introduced to set up initial locales.
            ApplyLanguagePreferences(doc);

            mProgressProcessor = (loadOptions.LoadFormat != LoadFormat.Pdf) && (detectedFormat != LoadFormat.Pdf)
                ? new PostLoadingProgressProcessor(doc, loadOptions) : null;
            doc.Accept(this);

            // Reset reference to avoid progress call invocation for the glossary document.
            mProgressProcessor = null;

            if (doc.GlossaryDocument != null)
                doc.GlossaryDocument.Accept(this);

            // WORDSNET-25580 Validate styles numbering references.
            ValidateStyleListLinks(doc.Styles, doc.Lists);

            // Synchronize public font embedding options stored in FontInfos
            // and internal font embedding options stored in DocPr.
            doc.FontInfos.UpdateEmbedFontOptions(doc.DocPr);

            // WORDSNET-14959. MS Word versions 2007 - 2019 get LCID from Windows registry.
            if (loadOptions.UseSystemLcid)
                doc.FirstSection.SectPr.UpdateDefaultsByLcid();

            if ((doc.GetThemeInternal() == null) && doc.Styles.DefaultRunPr.HasThemeFont)
                doc.SetThemeInternal(Theme.BuiltInTheme.Clone());
        }

        public override VisitorAction VisitDocumentStart(Document doc)
        {
            if (mLoadOptions.UpdateCharacterUnits)
            {
                mUnitConverter = new UnitConverter();
                mUnitConverter.VisitDocumentStart(doc);
            }

            DocumentStartCore(doc);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitGlossaryDocumentStart(GlossaryDocument glossary)
        {
            DocumentStartCore(glossary);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitDocumentEnd(Document doc)
        {
            DocumentEndCore();

            if (mLoadOptions.IsLoadingBlankDocument)
                Debug.Assert(mFieldNodesStack.Count == 0);
            else
                VisitNonBlankDocumentEnd(doc);

            // This call need for the case when tracked within progress nodes count is zero.
            if (mProgressProcessor != null)
                mProgressProcessor.Execute(true);

            return VisitorAction.Continue;
        }

        private void VisitNonBlankDocumentEnd(Document doc)
        {
            // FOSS

            // WORDSNET-16638 Problematic document contains fields without ends nodes. Remove or repair such fields to fix
            // the problem.
            ResolveInvalidFieldParts(doc);
        }

        /// <summary>
        /// Resolves problematic fields which are accumulated while post loading stage.
        /// </summary>
        private void ResolveInvalidFieldParts(Document doc)
        {
            // There are not nodes of fields which pending to be resolved.
            if (mFieldNodesStack.Count == 0)
                return;

            while (mFieldNodesStack.Count > 0)
            {
                FieldChar fieldPart = mFieldNodesStack.Pop();
                bool isNextNodeFieldStart = (mFieldNodesStack.Count > 0) &&
                    (mFieldNodesStack.Peek().NodeType == NodeType.FieldStart);

                // Mimic Word and try to repair problematic field. For example, see TestDefect12929.
                if (isNextNodeFieldStart && (fieldPart.NodeType == NodeType.FieldSeparator))
                {
                    RepairFieldWithoutEnd(doc, (FieldSeparator)fieldPart);
                }
                else
                {
                    // See TestJira15795 for details.
                    fieldPart.Remove();
                }
            }

            WarningUtil.Warn(WarningCallback, WarningType.UnexpectedContent,
                WarningSource.Validator, "Invalid fields were updated or removed.");
        }

        /// <summary>
        /// Appends end for the field which is missed or removes problematic nodes according to Word behavior.
        /// </summary>
        /// <remarks>
        /// <para>Actually Word does not append ends for the field, however, when document is opened it looks like
        /// problematic field was repaired and can be updated. AW has to provide valid model at this case, so
        /// end nodes are inserted.</para>
        /// </remarks>
        private void RepairFieldWithoutEnd(Document doc, FieldSeparator fieldSeparator)
        {
            Paragraph parentPara = fieldSeparator.ParentParagraph;
            if (parentPara == null)
                return;

            Node fieldEndRefNode = fieldSeparator.NextSiblingOfType(NodeType.FieldStart);
            bool isLastFieldInThePara = (fieldEndRefNode == null);

            fieldEndRefNode = isLastFieldInThePara ? parentPara.LastChild : fieldEndRefNode.PreviousSibling;
            RunPr fieldEndRefNodeRunPr = (fieldSeparator.RunPr == null) ? new RunPr() : fieldSeparator.RunPr.Clone();
            FieldEnd fieldEnd = new FieldEnd(doc, fieldEndRefNodeRunPr, fieldSeparator.FieldType, true);

            // if last node determined is the field separator, then insert expected end after the current node.
            if (ReferenceEquals(fieldEndRefNode, fieldSeparator) || isLastFieldInThePara)
                parentPara.InsertAfter(fieldEnd, fieldEndRefNode);
            else
                parentPara.InsertBefore(fieldEnd, fieldEndRefNode);

            // Extract start of the field.
            FieldStart fieldStart = (FieldStart)mFieldNodesStack.Pop();
            // End node is missing for this field, so type of the field has to be updated from the start node.
            fieldEnd.FieldType = fieldStart.FieldType;
            fieldSeparator.FieldType = fieldStart.FieldType;
        }

        public override VisitorAction VisitGlossaryDocumentEnd(GlossaryDocument glossary)
        {
            DocumentEndCore();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitBodyStart(Body body)
        {
            mStoryTypeStack.Push(body.StoryType);
            return mLoadOptions.UpdateCharacterUnits
                ? mUnitConverter.VisitBodyStart(body)
                : VisitorAction.Continue;
        }

        public override VisitorAction VisitBodyEnd(Body body)
        {
            mStoryTypeStack.Pop(body.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            mStoryTypeStack.Push(headerFooter.StoryType);
            return mLoadOptions.UpdateCharacterUnits
                ? mUnitConverter.VisitHeaderFooterStart(headerFooter)
                : VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterEnd(HeaderFooter headerFooter)
        {
            mStoryTypeStack.Pop(headerFooter.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            mBookmarkValidator.VisitBookmarkStart(bookmarkStart, StoryType.MainText);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            mBookmarkValidator.VisitBookmarkEnd(bookmarkEnd, StoryType.MainText);

            return VisitorAction.Continue;
        }

        private void DocumentStartCore(DocumentBase doc)
        {
            FixUpListLevelCounts(doc.Lists);
            FixUpStyles(doc.Styles);
            ValidateHtmlBlocks(doc.HtmlBlockCollection);

            mAnnotationValidator = new AnnotationValidator(doc, null, WarningCallback);

            // Do not truncate bookmark names on load.
            mBookmarkValidator = new BookmarkValidator(WarningCallback, false);
        }

        private void DocumentEndCore()
        {
            mAnnotationValidator.VisitDocumentEnd();
            mBookmarkValidator.VisitDocumentEnd();
        }

        private static void FixUpListLevelCounts(ListCollection lists)
        {
            int listDefCount = lists.ListDefCount;
            for (int i = 0; i < listDefCount; i++)
            {
                ListDef listDef = lists.GetListDefByIndex(i);
                if (!listDef.IsListStyleReference)
                    listDef.Levels.FixUpLevelCount(listDef.ListType, lists.Document);
            }
        }

        private void FixUpStyles(StyleCollection styles)
        {
            styles.FixUpBasedOnStyles();

            foreach (Style style in styles)
                style.Validate(WarningCallback);
        }

        public override VisitorAction VisitSectionStart(Section section)
        {
            section.SectPr.ValidateColumns(WarningCallback);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitShapeStart(Shape shape)
        {
            mStoryTypeStack.Push(shape.StoryType);

            // Update size from relative just after loading document into the model,
            // this makes values of Width and Height of shapes to be up to date.
            shape.UpdateSizeAndPositionFromRelative();

            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLStart(shape);

            ConvertOleObjectWithoutDataIntoImage(shape);
            RestoreProgIdFromOleData(shape);
            FixUpImageShapeZeroSize(shape);

            // Office Math is not fully supported in DOC and WML and it is represented as shape
            // with special attribute which holds OfficeMath xml.
            // Convert shape into Office Math according to loading setting.
            if ((mLoadOptions != null) && mLoadOptions.ConvertShapeToOfficeMath)
                OfficeMathUtil.ConvertShapeToOfficeMath(shape, WarningCallback);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitShapeEnd(Shape shape)
        {
            mStoryTypeStack.Pop(shape.StoryType);

            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                mBookmarkValidator.VisitDrawingMLEnd(shape);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitGroupShapeStart(GroupShape shape)
        {
            // Update size from relative just after loading document into the model,
            // this makes values of Width and Height of shapes to be up to date.
            shape.UpdateSizeAndPositionFromRelative();

            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLStart(shape);

            return VisitorAction.Continue;
        }

        private VisitorAction VisitDrawingMLStart(ShapeBase drawingML)
        {
            mBookmarkValidator.VisitDrawingMLStart(drawingML);

            ProcessFallback(drawingML);
            return VisitorAction.Continue;
        }

        private void ProcessFallback(ShapeBase shape)
        {
            if (shape.FallbackShape != null)
            {
                shape.FallbackShape.Accept(this);

                // Call MarkLoaded method to save calculated hash code, which will be used to invalidate fallback.
                shape.MarkLoaded();
            }
        }

        /// <summary>
        /// Restores "ProgId" from the OLE data when it missing.
        /// </summary>
        private static void RestoreProgIdFromOleData(Shape shape)
        {
            if (!shape.IsOleObject)
                return;

            ShapePr shapePr = shape.ShapePr;
            string progId = (string)shapePr[ShapeAttr.OleProgID];
            OleObject oleObject = shapePr[ShapeAttr.OleObject] as OleObject;

            if (StringUtil.HasChars(progId) || (oleObject == null))
                return;

            // WORDSNET-18205 Try to obtain missing "ProgId" from the OLE data.
            CompObjStream compObj = CompObjStream.Read(oleObject.Data);

            if (compObj != null && StringUtil.HasChars(compObj.ProgId))
                shape.SetShapeAttrInternal(ShapeAttr.OleProgID, compObj.ProgId);
        }

        private void ConvertOleObjectWithoutDataIntoImage(Shape shape)
        {
            // WORDSNET-21788 Avoid caching OLE package while calculating OLE object type.
            if (!shape.IsOle)
                return;

            if (shape.OleFormat.EmbeddedObject != null && !shape.OleFormat.EmbeddedObject.IsEmpty)
                return;

            switch (shape.OleObjectType)
            {
                case OleObjectType.Embedded:
                case OleObjectType.Control:
                // WORDSNET-3466 There is an embedded OLE object in the document but it has no data.
                    // We fix it by turning the object into a simple image.
                    // This is what MS Word does when working with WordML anyway.
                        WarnUnexpected(WarningStrings.EmbeddedOleWithoutData);
                shape.OleFormat.EmbeddedObject = null;
                        shape.SetShapeType(ShapeType.Image);
                    break;
                case OleObjectType.Linked:
                    // Do nothing.
                    break;
                default:
                    throw new InvalidOperationException("Unexpected OLE object type.");
            }
        }

        private void FixUpImageShapeZeroSize(Shape shape)
        {
            if (shape.HasImage)
            {
                if (MathUtil.IsZero(shape.Width) && MathUtil.IsZero(shape.Height))
                {
                    // WORDSNET-11685 This seems to be the value that MS Word uses when image shape size is zero.
                    WarnUnexpected(WarningStrings.ZeroShapeSize);

                    shape.SetWidthSafe(Shape.DefaultShapeSize);
                    shape.SetHeightSafe(Shape.DefaultShapeSize);
                }

                // WORDSNET-21857 Word uses different sizes when width is absent and when width is zero.
                if (!shape.ShapePr.Contains(ShapeAttr.Width))
                    shape.SetWidthSafe(Shape.InvalidShapeSizeDefault);

                if (!shape.ShapePr.Contains(ShapeAttr.Height))
                    shape.SetHeightSafe(Shape.InvalidShapeSizeDefault);
            }
        }

        public override VisitorAction VisitCellStart(Cell cell)
        {
            if (ReadFormatting)
            {
                // RK This is a quick fix for a problem in some readers (RTF especially).
                // The reader tends to leave null here and I don't want to crash.
                if (cell.CellPr == null)
                    cell.CellPr = new CellPr();

                // WORDSNET-5164 RK Zero widths in normal non-merged cell are bad for us, correct them.
                cell.CellPr.ValidateWidth();
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            ParaPr paraPr = para.ParaPr;

            if (IsAcceptedOrDeadNumberRevision(para))
            {
                WarnUnexpected(WarningStrings.InvalidNumberRevision);
                paraPr.Remove(RevisionAttr.NumberRevision);
            }

            Style style = para.Document.Styles.GetByIstd(paraPr.Istd, false);
            if (style != null)
                ValidateParagraphStyle(style, para);

            if(mLoadOptions.UpdateCharacterUnits)
                mUnitConverter.VisitParagraphStart(para);

            // WORDSNET-11324 List level exceed maximum allowed.
            if (para.ParaPr.ListLevel > ListLevel.MaxLevels)
            {
                para.ParaPr.SetAttr(ParaAttr.ListId, 0);
                para.ParaPr.SetAttr(ParaAttr.ListLevel, 12);

                // Word shows the document differently depending on application version.
                if(mLoadOptions.MswVersion <= MsWordVersion.Word2013)
                    para.InsertBefore(new Run(para.Document, ControlChar.Tab), para.FirstChild);
            }

            if (mProgressProcessor != null)
                mProgressProcessor.Execute(false);

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Validates paragraph attributes depending on the applied paragraph style.
        /// </summary>
        private static void ValidateParagraphStyle(Style style, Paragraph para)
        {
            // WORDSNET-11034 Remove reference to style if style is a character style.
            if (style.Type == StyleType.Character)
            {
                para.ParaPr.Remove(ParaAttr.Istd);
            }
            else if (style.IsHeading)
            {
                // WORDSNET-19542 Mimic the Word behavior for paragraphs formatted with a "Heading" style and outline level.
                FixUpParagraphOutlineLevel(para, style);
            }
        }

        /// <summary>
        /// Makes changes in the paragraph outline level depending on the applied style.
        /// </summary>
        private static void FixUpParagraphOutlineLevel(Paragraph para, Style style)
        {
            Debug.Assert(style.IsHeading);

            ParaPr paraPr = para.ParaPr;
            if (!paraPr.Contains(ParaAttr.OutlineLevel))
                return;

            // Word removes directly set outline level value or sets value from style when paragraph is in cell.
            if (para.IsInCell)
                paraPr.OutlineLevel = style.ParaPr.OutlineLevel;
            else
                paraPr.Remove(ParaAttr.OutlineLevel);
        }

        private static bool IsAcceptedOrDeadNumberRevision(Paragraph para)
        {
            ParagraphNumberRevision numberRevision = para.ParaPr.NumberRevision;
            if (numberRevision != null)
            {
                // *** An old formatting revision that remains after changes were accepted?
                // ParaPr.ListId any?
                // ParaPr.NumberRevision.IsInserted = false
                // ParaPr.NumberRevision.WasNumbered = false
                // ParaPr.NumberRevision.NumberLocations, NumberStyles, NumberValues set to "previous" value?
                bool isDead = !numberRevision.IsInsertion && !numberRevision.WasNumbered;
                if (isDead)
                    return true;

                // *** List formatting accepted on a paragraph:
                // ParaPr.ListId is non-zero - but it is important to query the paragraph, not just the direct formatting.
                // ParaPr.NumberRevision.IsInserted = false
                // ParaPr.NumberRevision.WasNumbered = true
                // ParaPr.NumberRevision.NumberLocations, NumberStyles, NumberValues set to "previous" value.
                bool isAccepted = !numberRevision.IsInsertion && numberRevision.WasNumbered && para.IsListItem;
                if (isAccepted)
                    return true;

                // WORDSNET-5720 Seems to be another variation of "dead" revision.
                // ParaPr.NumberRevision.IsInserted = true - this is insertion of list.
                // ParaPr.NumberRevision.WasNumbered = false - was not numbered before.
                // para.IsListItem = false - but currently it's not list item.
                bool isZombie = numberRevision.IsInsertion && !numberRevision.WasNumbered && !para.IsListItem;
                if (isZombie)
                    return true;
            }
            return false;
        }

        public override VisitorAction VisitSmartTagStart(SmartTag smartTag)
        {
            UntangleSmartTag(smartTag);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagStart(StructuredDocumentTag sdt)
        {
            sdt.UpdatePlaceholderReference(false);

            // We follow MS Word in ID 'always present and unique' behavior,
            // meaning that if we've read document with no id, we will create our own one.
            sdt.UpdateId();

            SetSelectedValueBySdtContent(sdt);

            if (mLoadOptions.UpdateXmlMapping)
            {
                // WORDSNET-18112 Update SDT bound content.
                if (sdt.XmlMapping.ChecksumNotEqual(mXmlMappingContext))
                {
                    // WORDSNET-16209 Do not update SDT marked for deletion.
                if (((ITrackableNode)sdt).DeleteRevision == null)
                    sdt.XmlMapping.UpdateContent(mXmlMappingContext);
            }
            }

            return VisitorAction.Continue;
        }

        /// <summary>
        /// andrnosk: WORDSNET-6911 Try to set SelectedValue using sdtContent.
        /// </summary>
        private void SetSelectedValueBySdtContent(StructuredDocumentTag sdt)
        {
            // ListItems is only accessible for ComboBox or DropDownList SDT types.
            if ((sdt.SdtType == SdtType.DropDownList) || (sdt.SdtType == SdtType.ComboBox))
            {
                if (sdt.ListItems.SelectedValue == null)
                {
                    string sdtContentValue = sdt.ContentValue;

                    for (int idx = 0; idx < sdt.ListItems.Count; idx++)
                    {
                        if (sdt.ListItems[idx].DisplayText == sdtContentValue)
                        {
                            sdt.ListItems.SelectedValue = sdt.ListItems[idx];
                            return;
                        }
                    }
                    Warn(WarningType.MinorFormattingLoss, WarningSource.WordML, "Structured Document Tag content is messed up.");
                }
            }
        }

        private static void UntangleSmartTag(SmartTag smartTag)
        {
            // RK MS Word seems to write nested smart tags that start at the same character position
            // (e.g. address/City) in random order in different formats. Sometimes it writes the outer
            // smart tag first, sometimes the inner smart tag first. I cannot figure out the algorithm
            // that it uses, but I have to represent smart tags in the correct way in the model and
            // when writing to documents so the smart tags appear properly in MS Word.
            // This method tries to "untagle" some well-known smart tags.
            // If you come across smart tags that are not untangled and cause problems, add them to this method.

            Node tagChild = smartTag.FirstChild;
            if (tagChild is SmartTag)
            {
                SmartTag childSmartTag = (SmartTag) tagChild;
                if ((childSmartTag.Element == "address") ||
                    (childSmartTag.Element == "PersonName") ||
                    (childSmartTag.Element == "place"))
                {
                    smartTag.Swap(childSmartTag);
                }
            }
        }

        public override VisitorAction VisitCommentStart(Comment comment)
        {
            mStoryTypeStack.Push(comment.StoryType);
            return mAnnotationValidator.VisitComment(comment);
        }

        public override VisitorAction VisitCommentEnd(Comment comment)
        {
            mStoryTypeStack.Pop(comment.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            mStoryTypeStack.Push(footnote.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFootnoteEnd(Footnote footnote)
        {
            mStoryTypeStack.Pop(footnote.StoryType);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            return mAnnotationValidator.VisitCommentRangeStart(commentRangeStart);
        }

        public override VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            return mAnnotationValidator.VisitCommentRangeEnd(commentRangeEnd);
        }

        public override VisitorAction VisitEditableRangeStart(EditableRangeStart editableRangeStart)
        {
            return mAnnotationValidator.VisitEditableRangeStart(editableRangeStart);
        }

        public override VisitorAction VisitEditableRangeEnd(EditableRangeEnd editableRangeEnd)
        {
            return mAnnotationValidator.VisitEditableRangeEnd(editableRangeEnd);
        }

        public override VisitorAction VisitTableStart(Table table)
        {
            // WORDSNET-23913 Remove empty rows.
            List<Row> rows = table.Rows.ToList<Row>();
            foreach(Row row in rows)
            {
                if (row.Cells.Count == 0)
                {
                    Warn(WarningType.MinorFormattingLoss, WarningSource.Validator, WarningStrings.TableValidatorEmptyRow);
                    row.Remove();
                }
                // dmatv: ValidateAlignedIndent() moved to VisitTableEnd()
                // as it had been removing some indents needed to merge adjacent tables correctly.
            }

            if (ReadFormatting)
            {
                Row row = table.FindDifferentPositioningOrStyle();
                if (row != null)
                {
                    row.BreakTableBefore();
                    // Next sibling is cached in our visitor implementation so accept new table explicitly.
                    row.ParentTable.Accept(this);
                }

                table.FixNestedTableLayout();
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitTableEnd(Table table)
        {
            Table combinedTable = table;
            if (ReadFormatting)
            {
                // Do not bother with table merging and grid update if not in the main document.
                bool isMainDocument = table.Document is Document;
                if (isMainDocument)
                {
                    // Get grid and cell spans from cell widths for doc/rtf documents.
                    DocRtfGridHandler.GetGridAndSpansFromCellWidths(table);

                    // Combine the adjacent tables.
                    combinedTable =  mTableMerger.JoinAdjacentTablesCombineGrids(table);

                    // Sometimes, adjacent tables with similar properties are merged in the model.
                    // Grid must be calculated for the merged table,
                    // calculating for separate tables before merge may not be accurate.
                    bool waitForMergedTablesBeforeGridUpdate = (combinedTable == null);
                    if (!waitForMergedTablesBeforeGridUpdate)
                    {
                        // Now the grid can be updated for the merged table.

                        // WORDSNET-21928 remove unneeded gridBefore/widthBefore.
                        // Ideally that should be done by table grid calculation below,
                        // but currently it does not yet handle tables with gridBefore.
                        bool isMergedTable = (table != combinedTable);
                        // Do not strip gridBefore from merged tables however.
                        // Though it is probably OK, the case is tricky and it was not investigated per 21928.
                        // For now it seems safer just to keep the old logic for merged adjacent tables.
                        if (!isMergedTable)
                            RemoveSameWidthBefore(combinedTable);
                        // New grid algorithm may become applicable if same gridBefore attributes were actually removed.

                        UpdateNestedTableGrids(combinedTable, NestedTables, true);
                    }
                }
            }

            // Validate indents after combining adjacent tables.
            if (combinedTable != null)
                foreach (Row row in table.Rows)
                    row.ValidateAlignedIndent();

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Removes gridBefore/widthBefore attributes if they are the same for all table rows.
        /// </summary>
        /// <remarks>
        /// It seems, MS Word removes gridBefore/widthBefore if they are the same for all table rows.
        /// In such case, there is actually no point in having them. The attributes are normally used in jagged tables
        /// to indicate cells missing before the first row cell. If they are the same for all rows, then every row starts
        /// from the same column and the attributes are not needed.
        /// It seems, WPS Office can generate documents with tables having the same gridBefore/widthBefore for all rows for some reason.
        /// Removing them fixes WORDSNET-21928 and WORDSNET-21935
        /// It would be best to handle the situation during grid calculation,
        /// but tables with gridBefore are not handled by the new algorithm yet.
        /// </remarks>
        private static void RemoveSameWidthBefore(Table table)
        {
            // Nothing to do for no rows.
            if (table == null || table.FirstRow == null)
                return;

            TablePr firstRowPr = table.FirstRow.TablePr;

            if (firstRowPr == null)
                return;

            int gridBefore = firstRowPr.GridBefore;
            PreferredWidth widthBefore = firstRowPr.WidthBefore;
            PreferredWidth widthBeforeOriginal = firstRowPr.WidthBeforeOriginal;

            // No point scanning the table if there is no grid/width before on row 1.
            if (gridBefore == 0 && widthBefore.ValueRaw == 0 && widthBeforeOriginal.ValueRaw == 0)
                return;

            bool allRowsHaveSameAttrs = true;
            bool anyRowHasRevision = false;
            foreach (Row row in table.Rows)
            {
                TablePr rowPr = row.TablePr;

                if (rowPr == null)
                {
                    Debug.Fail("Null Row.TablePr should not happen.");
                    // Prevent removing gridBefore as default zeros in this case do not match the first row.
                    allRowsHaveSameAttrs = false;
                    break;
                }

                anyRowHasRevision = rowPr.HasRevisions || rowPr.HasEmptyFormatRevision;
                if (anyRowHasRevision)
                    break;

                allRowsHaveSameAttrs = (gridBefore == rowPr.GridBefore) &&
                    widthBefore.Equals(rowPr.WidthBefore) &&
                    widthBeforeOriginal.Equals(rowPr.WidthBeforeOriginal);

                // TODO actually, a situation with rows have zero widths before of different types is possible.

                if (!allRowsHaveSameAttrs)
                    break;
            }

            // Do not apply the logic to tables with revisions, it was not tested with them.
            if (!anyRowHasRevision && allRowsHaveSameAttrs)
            {
                foreach (Row row in table.Rows)
                {
                    TablePr rowPr = row.TablePr;
                    rowPr.Remove(TableAttr.Sys_GridBefore);
                    rowPr.Remove(TableAttr.WidthBeforeOriginal);
                }

                // The tables in WORDSNET-21928 and WORDSNET-21935 for which the logic applies also have
                // tblGrid elements with the first column corresponding to the invalid "gridBefore".
                // This column should be removed from tblGrid as well.
                // Otherwise it will affect taking the default width from tblGrid for
                // cells with "auto" preferred width in a fixed layout table. 21928 and 21935 both have this case.
                RemoveFirstColumnsFromGrid(table.TablePr, gridBefore);
            }
        }

        /// <summary>
        /// Removes the given number of columns from the beginning of grids stored in <see cref="TableAttr.Sys_TableGrid"/>
        /// and <see cref="TableAttr.Sys_TableGridForNewAlgorithm"/>.
        /// </summary>
        private static void RemoveFirstColumnsFromGrid(TablePr tablePr, int removeCount)
        {
            IntList grid = (IntList)tablePr[TableAttr.Sys_TableGrid];
            RemoveFirstColumnsFromList(grid, removeCount);

            TableGridColumnsAttr gridAttr = (TableGridColumnsAttr)tablePr[TableAttr.Sys_TableGridForNewAlgorithm];
            if (gridAttr != null)
                RemoveFirstColumnsFromList(gridAttr.GridColumns, removeCount);
        }

        /// <summary>
        /// Removes the given number of items from the specified <see cref="IntList"/>.
        /// </summary>
        private static void RemoveFirstColumnsFromList(IntList list, int removeCount)
        {
            if (list == null)
                return;

            int checkedCount = System.Math.Min(list.Count, removeCount);
            if (checkedCount > 0)
                list.RemoveRange(0, checkedCount);
        }

        public override VisitorAction VisitRowStart(Row row)
        {
            // WORDSNET-12368 Should verify that style type is 'Table'.
            // We do this check here as well as in DocumentValidator because some of operations
            // can be performed before DocumentValidator is invoked. For example,
            // document comparison has thrown an exception before this fix.
            row.EnsureTableStyleIsValid();

            TableValidator.RemoveInvalidAttributes(row);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            mFieldNodesStack.Push(fieldStart);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            mFieldNodesStack.Push(fieldSeparator);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            // Extract separator for the field, in case when it was found.
            mFieldNodesStack.PopIfInstanceOf<FieldChar>(typeof(FieldSeparator));

            // Extract start node for the field, in case when it was found.
            mFieldNodesStack.PopIfInstanceOf<FieldChar>(typeof(FieldStart));

            return VisitorAction.Continue;
        }

        /// <summary>
        /// Adds the given table to the stack of the given nested tables and updates grid for the whole stack if the table is not nested.
        /// </summary>
        /// <remarks>
        /// The method is supposed to be called from the document validator or postloader.
        /// For fixed table layout, the nested table grid can be built only after the container width is known.
        /// On the other hand, rows and cells must be processed by validator/postloader before grid calculation in order to have the correct values.
        /// So the most nested tables are added to the stack first.
        /// By the moment the top-level table is added, all the properties of the nested tables are OK.
        /// The grids are updated from the top table then, so the nested tables have the container width for the grid update.
        /// </remarks>
        internal static void UpdateNestedTableGrids(Table table, Stack<Table> nestedTables, bool isLoading)
        {
            nestedTables.Push(table);

            if (!table.IsNested)
            {
                // Build the grid for all nested tables, starting from the top.
                while (nestedTables.Count != 0)
                {
                    Table nextTable = nestedTables.Pop();
                    FixedGridCalculator.UpdateTableGrid(nextTable, isLoading);
                }
            }
        }

        /// <summary>
        /// Applies <see cref="LanguagePreferences"/> to a specified document.
        /// </summary>
        private void ApplyLanguagePreferences(Document doc)
        {
            if (LanguagePreferences.TestMode)
                return;

            if (mLoadOptions == null)
                return;

            LanguagePreferences preferences = mLoadOptions.LanguagePreferences;

            RunPr defaultRunPr = doc.Styles.DefaultRunPr;

            defaultRunPr.SetAttrNoOverride(FontAttr.LocaleId, preferences.LocaleId);
            defaultRunPr.SetAttrNoOverride(FontAttr.LocaleIdFarEast, preferences.LocaleIdFarEast);
            defaultRunPr.SetAttrNoOverride(FontAttr.LocaleIdBi, preferences.LocaleIdBi);

            // The setting is propagated to layout to imitate MS Word behavior with Asian preferred language set.
            // See Test26100Kinsoku().
            Language preferredEditingLanguage = (Language)preferences.LocaleIdFarEastByDefaultLanguage;
            doc.DocPr.PreferredEditingLanguage = preferredEditingLanguage;

            // WORDSNET-24172 Set additional properties for Asian languages to mimic Word.
            // Looks strange a bit, but Word only changes in one direction.
            // When one of the Asian languages is set as the default language it makes changes.
            // But, when NOT an Asian language is set, Word does nothing.
            if (LocaleClassifier.IsChineseOrJapanese((int)preferences.DefaultEditingLanguage))
            {
                doc.DocPr.CompatibilityOptions.UseFELayout = true;

                Theme theme = doc.GetThemeInternal();
                if ((theme != null) && (theme.ThemeFontLanguages != null))
                    theme.ThemeFontLanguages.EastAsia = preferredEditingLanguage;

                 // WORDSNET-25854 Do not override East Asian font if it is already so.
                string fontName = defaultRunPr.ComplexNameFarEast.Resolve(theme);
                if (!FontUtil.IsAsianFont(doc.FontProvider, fontName))
                    defaultRunPr.SetAttr(FontAttr.NameFarEast, ComplexFontName.FromTheme(ThemeFontCore.MinorEastAsia));

                // Dmatv: this ensures setting correct font for Chinese and Japanese.
                // Checked by Test23520().
                defaultRunPr.LocaleIdFarEast = (int)preferredEditingLanguage;
            }
        }

        /// <summary>
        /// Validates HtmlBlock collections.
        /// </summary>
        private static void ValidateHtmlBlocks(HtmlBlockCollection htmlBlocks)
        {
            for(int i = 0; i < htmlBlocks.Count; i++)
            {
                HtmlBlock htmlBlock = htmlBlocks.GetHtmlBlockByIndex(i);

                // Remove circular reference.
                if (htmlBlock.ParentId == htmlBlock.Id)
                    htmlBlock.ParentId = 0;
            }
        }

        /// <summary>
        /// Ensures that only one style is linked to each list level.
        /// </summary>
        /// <remarks>
        /// Although code looks complex actually we just collect styles referred to each list level and then
        /// "remove" extra references (style become not numbered with list level formatting applied).
        ///
        /// This logic was obtained by tests.
        /// </remarks>
        private void ValidateStyleListLinks(StyleCollection styles, ListCollection lists)
        {
            // Style to ListId + ListLevel references table.
            Dictionary<int, Dictionary<int, List<Style>>> refCount = new Dictionary<int, Dictionary<int, List<Style>>>();

            // Loop for all styles and collect references to list levels.
            for (int istd = 0; istd < styles.MaxIstd; istd++)
            {
                Style style = styles.GetByIstd(istd, false);

                if(style == null || style.BuiltIn || style.ParaPr.ListId == 0)
                    continue;

                int listId = style.ParaPr.ListId;
                List list = lists.GetListByListId(listId);

                if (list == null)
                    continue;

                // Seems lists that are definition for a style is not counted.
                if (list.ListDef.ListStyleIstd == istd)
                    continue;

                int listLevel = 0;

                // Find level referenced either by ListLevel attribute or ParaStyleIstd.
                if (style.ParaPr.ContainsKey(ParaAttr.ListLevel))
                    listLevel = (int)style.ParaPr[ParaAttr.ListLevel];
                else
                {
                    ListDef listDef = list.ListDef;
                    foreach (ListLevel level in listDef.Levels)
                    {
                        if (level.ParaStyleIstd == style.Istd)
                        {
                            listLevel = level.LevelNumber;
                            break;
                        }
                    }
                }

                // Fill reference count table.
                if (!refCount.ContainsKey(listId))
                    refCount[listId] = new Dictionary<int, List<Style>>();

                if (!refCount[listId].ContainsKey(listLevel))
                    refCount[listId][listLevel] = new List<Style>();

                refCount[listId][listLevel].Add(style);
            }

            // Unlink extra styles referenced to certain list level.
            foreach (List list in lists)
            {
                foreach (ListLevel level in list.ListDef.Levels)
                {
                    if(level.NumberStyle == NumberStyle.Bullet)
                        continue;

                    if(!refCount.ContainsKey(list.ListId) || !refCount[list.ListId].ContainsKey(level.LevelNumber))
                        continue;

                    List<Style> refStyles = refCount[list.ListId][level.LevelNumber];
                    if (refStyles == null)
                        continue;

                    if(refStyles.Count > 1)
                        for (int i = 1; i < refStyles.Count; i++)
                        {
                            Style invalidStyles = refStyles[i];
                            invalidStyles.ParaPr.Remove(ParaAttr.ListId);
                            invalidStyles.ParaPr.Remove(ParaAttr.ListLevel);
                            level.ParaPr.ExpandTo(invalidStyles.ParaPr);

                            Warn(WarningType.UnexpectedContent, WarningSource.Validator,
                                string.Format(WarningStrings.RemovedStyleListReference, invalidStyles.Name));
                        }
                }
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        private void Warn(WarningType warningType, WarningSource warningSource, string description)
        {
            if (WarningCallback != null)
                WarningCallback.Warning(new WarningInfo(warningType, warningSource, description));
        }

        private void WarnUnexpected(string description, params object[] args)
        {
            WarningUtil.WarnUnexpected(WarningCallback, description, args);
        }

        private IWarningCallback WarningCallback
        {
            get { return (mLoadOptions != null) ? mLoadOptions.WarningCallback : null; }
        }

        private bool ReadFormatting
        {
            get { return (mLoadOptions != null) && !mLoadOptions.SkipFormatting; }
        }

        /// <summary>
        /// Remembers created field start and separator nodes. Pops on field end.
        /// This collection contains nodes, which can be recognized as invalid while visiting end of the document.
        /// </summary>
        private readonly Stack<FieldChar> mFieldNodesStack = new Stack<FieldChar>();

        /// <summary>
        /// Strictly speaking all features of the comment validator are required before save only.
        /// Here, after loading, we only need to regenerate comment ids, but it is so easy by running the complete comment validator.
        /// andrnosk: And the same we should do for editable ranges.
        /// </summary>
        private AnnotationValidator mAnnotationValidator;

        private BookmarkValidator mBookmarkValidator;

        private LoadOptions mLoadOptions;

        /// <summary>
        /// Stores nested tables stack for the grid update.
        /// </summary>
        private Stack<Table> NestedTables
        {
            get { return mNestedTables; }
        }
        private readonly Stack<Table> mNestedTables = new Stack<Table>();

        /// <summary>
        /// Handles adjacent table merging.
        /// </summary>
        private readonly TableMerger mTableMerger = new TableMerger();

        private UnitConverter mUnitConverter = new UnitConverter();

        private readonly XmlMappingContext mXmlMappingContext = new XmlMappingContext();

        private PostLoadingProgressProcessor mProgressProcessor;

        private readonly StoryTypeStack mStoryTypeStack = new StoryTypeStack();
    }
}
