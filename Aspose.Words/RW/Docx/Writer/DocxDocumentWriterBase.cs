// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Images;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.Nrx;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.RW.Ole;
using Aspose.Words.RW.Vml;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using Aspose.Words.Validation.DmlToVml;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Base class for main and glossary document writers.
    /// </summary>
    internal abstract class DocxDocumentWriterBase :
        DocumentVisitor,
        IVmlShapeWriterContext,
        IDmlShapeWriterContext,
        INrxWriterContext,
        IMathWriterContext
    {
        protected DocxDocumentWriterBase(
            DocxWriter writer,
            DocumentBase doc,
            IWarningCallback warningCallback)
        {
            Debug.Assert(writer != null);
            Debug.Assert(doc != null);

            mWriter = writer;
            mDoc = doc;

            mStyleIdGenerator = new NrxStyleIdGenerator(mDoc.Styles);
            mRunWriter = new DocxRunWriter(this);
            mFieldsWriter = new DocxFieldsWriter(this, mRunWriter);
            mWarningCallback = warningCallback;
        }

        /// <summary>
        /// Generates unique paraId value.
        /// </summary>
        internal int GenerateParaId(StoryType storyType)
        {
            while (true)
            {
                int paraId = RandomUtil.NewGuid().GetHashCode();
                // WORDSNET-16092 ID should be a positive number, otherwise MS Word cannot find such paragraph.
                paraId &= 0x7fffffff;

                if (paraId == 0)
                    continue;

                // Check that paraId is not used.
                HashSetGeneric<int> usedParaId = (HashSetGeneric<int>)mWriter.SaveInfo.UsedParaIdArray[(int)storyType];
                if (usedParaId.Contains(paraId))
                    continue;

                usedParaId.Add(paraId);
                return paraId;
            }
        }

        internal void Write()
        {
            mDocumentPart = DoCreateDocumentPart();

            mFootnotesWriter = new DocxFootnotesWriter(FootnoteType.Footnote, this);

            mEndnotesWriter = new DocxFootnotesWriter(FootnoteType.Endnote, this);

            DocxSettingsWriter.Write(this);
            DocxWebSettingsWriter.Write(this, SaveInfo.HasHtmlBlockReferences);
            DocxFontTableWriter.Write(this);

            PushBuilder(new DocxBuilder(mDocumentPart, SaveOptions.PrettyFormat, Compliance, MsWordExtensionsVersion,
                mWarningCallback));

            DoWrite();

            DocxNumberingWriter.Write(this);
            DocxStylesWriter.Write(this);
            DocxPeopleWriter.Write(this);

            // Close all auxiliary part writers.
            if (mCommentsWriter != null)
                mCommentsWriter.Close();

            mFootnotesWriter.Close();
            mEndnotesWriter.Close();

            PopBuilder();
        }

        [JavaThrows(true)]  // IO Exceptions
        protected abstract OpcPackagePart DoCreateDocumentPart();

        [JavaThrows(true)]  // IO Exceptions
        protected abstract void DoWrite();

        /// <summary>
        /// Writes an OOXML "body" of the main document or a building block.
        /// </summary>
        protected void WriteSections(CompositeNode sectionsContainer)
        {
            for (Node node = sectionsContainer.FirstChild; node != null; node = node.NextSibling)
            {
                CurrentSection = (Section)node;
                // RK I'd like to refactor so I can delete this member.
                mLastParagraphOfCurrentSection = CurrentSection.Body.LastParagraph;

                // This writes the main story of the section.
                CurrentSection.Body.Accept(this);
            }
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        public void Warn(WarningType warningType, string description)
        {
            Warn(warningType, WarningSource.WordML, description);
        }

        /// <summary>
        /// Logs a warning to the user-provided warning callback.
        /// </summary>
        public void Warn(WarningType warningType, WarningSource source, string description)
        {
            if (mWarningCallback != null)
                mWarningCallback.Warning(new WarningInfo(warningType, source, description));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitTableStart(Table table)
        {
            mBuilder.StartElement("w:tbl");

            NrxRowPrWriter.Write(table.FirstRow.TablePr, true, false, this);

            table.CellWidthsToTableGrid();
            NrxTableGrid.WriteTblGrid(mBuilder, table);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitTableEnd(Table table)
        {
            mBuilder.EndElement("w:tbl");
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Collapses table borders.
        /// </summary>
        /// <remarks>
        /// AM. The same as AttrCollection.Collapse but limited to borders attributes only.
        /// Actually we should process every row-level attributes here in the same way.
        /// </remarks>
        private static void CollapseAttrs(TablePr srcTablePr, TablePr dstTablePr)
        {
            foreach (int key in TablePr.PossibleBorderKeys.Values)
            {
                Border dst = (Border)dstTablePr.GetDirectAttr(key);
                Border src = (Border)srcTablePr.GetDirectAttr(key);

                if ((dst == null) && (src != null) && (src.LineStyle != LineStyle.None) && !src.IsInherited)
                    dstTablePr.SetAttr(key, TablePr.FetchDefaultAttr(key));
                else if (object.Equals(dst, src))
                    dstTablePr.Remove(key);
            }

            // There are additional conditions for borders above, lets do separate loop for the rest.
            // Collapse only limited attribute set to reduce affected golds.
            foreach (int key in new int[]
                {
                    TableAttr.TopPadding, TableAttr.LeftPadding, TableAttr.RightPadding, TableAttr.BottomPadding,
                    TableAttr.CellSpacing, TableAttr.Alignment
                })
            {
                object dst = dstTablePr.GetDirectAttr(key);
                object src = srcTablePr.GetDirectAttr(key);

                if ((dst == null) && (src != null))
                    dstTablePr.SetAttr(key, TablePr.FetchDefaultAttr(key));
                else if (object.Equals(dst, src))
                    dstTablePr.Remove(key);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitRowStart(Row row)
        {
            mBuilder.StartElement("w:tr");

            if (Compliance != OoxmlComplianceCore.Ecma376)
            {
                if (SaveOptions.WriteExtendedIds)
                {
                    if (row.ParaId > 0)
                        mBuilder.WriteAttribute("w14:paraId", NrxXmlUtil.IntToHex(row.ParaId));

                    if (row.TextId > 0)
                        mBuilder.WriteAttribute("w14:textId", NrxXmlUtil.IntToHex(row.TextId));
                }
            }

            TablePr effectiveRowPr = row.TablePr.Clone();

            // Collapse row attributes to get table in DOCX format.
            CollapseAttrs(row.ParentTable.FirstRow.TablePr, effectiveRowPr);

            NrxRowPrWriter.Write(effectiveRowPr, false, false, this);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitRowEnd(Row row)
        {
            mBuilder.EndElement("w:tr");
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitCellStart(Cell cell)
        {
            mBuilder.StartElement("w:tc");
            NrxCellPrWriter.Write(cell.CellPr, this);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitCellEnd(Cell cell)
        {
            mBuilder.EndElement("w:tc");
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            mBuilder.StartElement("w:p");

            // Actually we should save rsidRDefault here. But we have not found it yet.
            mBuilder.RsidR = NrxXmlUtil.IntToHex(para.ParagraphBreakRunPr[FontAttr.RsidR]);
            mBuilder.WriteAttribute("w:rsidR", mBuilder.RsidR);

            mBuilder.WriteAttribute("w:rsidRPr", NrxXmlUtil.IntToHex(para.ParagraphBreakRunPr[FontAttr.RsidRPr]));

            // Not found yet.
            // mBuilder.WriteAttribute("w:rsidRDefault", ...)

            mBuilder.WriteAttribute("w:rsidP", NrxXmlUtil.IntToHex(para.ParaPr[ParaAttr.RsidP]));

            if (Compliance != OoxmlComplianceCore.Ecma376)
            {
                if (CommentParagraphToParaId.ContainsKey(para))
                {
                    // Generated paraId which holds comment tree structure should be written regardless option set.
                    mBuilder.WriteAttribute("w14:paraId", NrxXmlUtil.IntToHex(CommentParagraphToParaId[para]));
                }
                else
                {
                    if (SaveOptions.WriteExtendedIds)
                    {
                        if (para.ParaId > 0)
                            mBuilder.WriteAttribute("w14:paraId", NrxXmlUtil.IntToHex(para.ParaId));

                        if (para.TextId > 0)
                            mBuilder.WriteAttribute("w14:textId", NrxXmlUtil.IntToHex(para.TextId));
                    }
                }
            }

            WriteParaPr(para);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitParagraphEnd(Paragraph para)
        {
            // Close "w:dir" and "w:bdo" elements for the current paragraph if any.
            Stack<string> bidiControlChars = mParaToBidiControlChars.GetValueOrNull(para);
            if (bidiControlChars != null)
                while (bidiControlChars.Count > 0)
                    mBuilder.EndElement(bidiControlChars.Pop());

            mBuilder.EndElement("w:p");

            if (ProgressProcessor != null)
                ProgressProcessor.Step();

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitSmartTagStart(SmartTag smartTag)
        {
            DocxSmartTagWriter.WriteStart(smartTag, mBuilder);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitSmartTagEnd(SmartTag smartTag)
        {
            DocxSmartTagWriter.WriteEnd(mBuilder);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitStructuredDocumentTagStart(StructuredDocumentTag sdt)
        {
            DocxSdtWriter.WriteStart(sdt, this);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagRangeStart(StructuredDocumentTagRangeStart rangeStart)
        {
            // Here we should write SDT.
            StructuredDocumentTag sdt = rangeStart.InternalSdt;
            StructuredDocumentTagRangeEnd rangeEnd = rangeStart.RangeEnd;

            // Write SDT only if all parts of "ranged" SDT is found.
            if (ArgumentUtil.BothAreNotNull(sdt, rangeEnd) && StructuredDocumentTag.IsRangeValid(rangeStart, rangeEnd))
            {
                DocxSdtWriter.WriteStart(sdt, this);
                mSdtRangeStartMap.Add(rangeStart.Id, rangeStart);
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitStructuredDocumentTagRangeEnd(StructuredDocumentTagRangeEnd rangeEnd)
        {
            int id = rangeEnd.Id;

            StructuredDocumentTagRangeStart rangeStart = mSdtRangeStartMap[id];
            StructuredDocumentTag sdt = (rangeStart != null) ? rangeStart.InternalSdt : null;

            // Write SDT only if all parts of "ranged" SDT is found.
            if (ArgumentUtil.BothAreNotNull(sdt, rangeStart) && StructuredDocumentTag.IsRangeValid(rangeStart, rangeEnd))
                DocxSdtWriter.WriteEnd(sdt, this);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitStructuredDocumentTagEnd(StructuredDocumentTag sdt)
        {
            DocxSdtWriter.WriteEnd(sdt, this);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitOfficeMathStart(OfficeMath officeMath)
        {
            DocxMathWriter.WriteStart(officeMath.MathObject, officeMath.RunPr, this);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitOfficeMathEnd(OfficeMath officeMath)
        {
            DocxMathWriter.WriteEnd(mBuilder);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaThrows(true)]
        public override VisitorAction VisitRun(Run run)
        {
            // Skip field code for HYPERLINK field if it is written as hyperlink.
            if (mFieldsWriter.SkipThisInline)
                return VisitorAction.Continue;

            mRunWriter.WriteInline(run);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [JavaThrows(true)]
        public override VisitorAction VisitSpecialChar(SpecialChar specialChar)
        {
            // WORDSNET-18081 Preserve presentation of “DateShortChar” special character in the document.
            if (SaveOptions.KeepLegacyControlChars && (specialChar.GetText() == ControlChar.NonBreakingHyphen))
            {
                WriteDateShortCharAsField(specialChar);
            }
            else
            {
                mRunWriter.WriteInline(specialChar);
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitAbsolutePositionTab(AbsolutePositionTab tab)
        {
            mBuilder.StartElement("w:r"); // ptab is stored in a run per spec.
            mBuilder.StartElement("w:ptab");
            mBuilder.WriteAttribute("w:relativeTo", DocxEnum.AbsolutePositionTabPositioningBaseToDocx(tab.PositioningBase));
            mBuilder.WriteAttribute("w:alignment", DocxEnum.AbsolutePositionTabAlignmentToDocx(tab.Alignment));
            mBuilder.WriteAttribute("w:leader", DocxEnum.AbsolutePositionTabLeaderCharToDocx(tab.LeaderChar));
            mBuilder.EndElement("w:ptab");
            mBuilder.EndElement("w:r");

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            mFieldsWriter.WriteFldChar(fieldStart);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            mFieldsWriter.WriteFldChar(fieldSeparator);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            mFieldsWriter.WriteFldChar(fieldEnd);
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitBookmarkStart(BookmarkStart bookmarkStart)
        {
            int id;

            string bookmarkKey = BookmarkValidatorItem.GetKey(bookmarkStart);

            // Use separate dictionary for bookmarks located in fallback.
            bool isFallback = bookmarkStart.GetTopmostAncestor().NodeType != NodeType.Document;
            StringToIntDictionary dict = isFallback ? mBookmarkIdsByNameFallback : mBookmarkIdsByName;

            if (dict.ContainsKey(bookmarkKey))
            {
                id = dict[bookmarkKey];
            }
            else
            {
                id = GetNextAnnotationId();
                dict.Add(bookmarkKey, id);
            }

            mBuilder.StartElement("w:bookmarkStart");

            mBuilder.WriteAttribute("w:id", id);
            // TestBookmarkEmptyName - Name should be written even if it is an empty string.
            mBuilder.WriteAttributeString("w:name", bookmarkStart.Name);

            if (bookmarkStart.IsColumn)
            {
                mBuilder.WriteAttribute("w:colFirst", bookmarkStart.FirstColumn);
                mBuilder.WriteAttribute("w:colLast", bookmarkStart.LastColumn);
            }

            WriteDisplacedByCustomXmlAttribute(bookmarkStart);

            mBuilder.EndElement("w:bookmarkStart");

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitBookmarkEnd(BookmarkEnd bookmarkEnd)
        {
            string bookmarkKey = BookmarkValidatorItem.GetKey(bookmarkEnd);

            // Use separate dictionary for bookmarks located in fallback.
            bool isFallback = bookmarkEnd.GetTopmostAncestor().NodeType != NodeType.Document;
            StringToIntDictionary dict = isFallback ? mBookmarkIdsByNameFallback : mBookmarkIdsByName;

            int id = dict[bookmarkKey];
            if (StringToIntDictionary.IsNullSubstitute(id))
                throw new InvalidOperationException();

            mBuilder.StartElement("w:bookmarkEnd");
            mBuilder.WriteAttribute("w:id", id);

            WriteDisplacedByCustomXmlAttribute(bookmarkEnd);

            mBuilder.EndElement("w:bookmarkEnd");

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitEditableRangeStart(EditableRangeStart editableRangeStart)
        {
            mBuilder.StartElement("w:permStart");

            mBuilder.WriteAttribute("w:id", IdToAnnotationId(editableRangeStart.Id));

            WriteDisplacedByCustomXmlAttribute(editableRangeStart);

            if (editableRangeStart.IsColumn)
            {
                mBuilder.WriteAttribute("w:colFirst", editableRangeStart.FirstColumn);
                mBuilder.WriteAttribute("w:colLast", editableRangeStart.LastColumn);
            }

            mBuilder.WriteAttribute("w:ed", editableRangeStart.SingleUser);
            mBuilder.WriteAttribute("w:edGrp", DocxDopEnum.EditorTypeToDocx(editableRangeStart.EditorGroup));

            mBuilder.EndElement("w:permStart");

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitEditableRangeEnd(EditableRangeEnd editableRangeEnd)
        {
            WriteAnnotation(editableRangeEnd, "w:permEnd");
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitCommentStart(Comment comment)
        {
            if(!CanBeWritten(comment))
                return VisitorAction.SkipThisNode;

            int annotationId = IdToAnnotationId(comment.Id);

            // Write comment reference.
            mRunWriter.WriteRunStart(comment.RunPr, comment.RunPr, true); // MS Word seems to write rsids for comment references, so do we.

            WriteElementWithId("w:commentReference", annotationId);
            mRunWriter.WriteRunEnd(comment.RunPr);

            if (mCommentsWriter == null)
                mCommentsWriter = new DocxCommentsWriter(this);

            mCommentsWriter.WriteStart(comment, annotationId);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitCommentEnd(Comment comment)
        {
            mCommentsWriter.WriteEnd();
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitCommentRangeStart(CommentRangeStart commentRangeStart)
        {
            WriteAnnotation(commentRangeStart, "w:commentRangeStart");
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitCommentRangeEnd(CommentRangeEnd commentRangeEnd)
        {
            WriteAnnotation(commentRangeEnd, "w:commentRangeEnd");
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override VisitorAction VisitMoveFromRangeStart(MoveRangeStart moveFromRangeStart)
        {
            mBuilder.StartElement("w:moveFromRangeStart");
            WriteMoveRangeStartCore(moveFromRangeStart);
            mBuilder.EndElement("w:moveFromRangeStart");

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override VisitorAction VisitMoveFromRangeEnd(MoveRangeEnd moveFromRangeEnd)
        {
            WriteAnnotation(moveFromRangeEnd, "w:moveFromRangeEnd");
            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override VisitorAction VisitMoveToRangeStart(MoveRangeStart moveToRangeStart)
        {
            mBuilder.StartElement("w:moveToRangeStart");
            WriteMoveRangeStartCore(moveToRangeStart);
            mBuilder.EndElement("w:moveToRangeStart");

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override VisitorAction VisitMoveToRangeEnd(MoveRangeEnd moveToRangeEnd)
        {
            WriteAnnotation(moveToRangeEnd, "w:moveToRangeEnd");
            return VisitorAction.Continue;
        }

        private void WriteMoveRangeStartCore(MoveRangeStart moveRangeStart)
        {
            mBuilder.WriteAttribute("w:id", IdToAnnotationId(moveRangeStart.Id));
            mBuilder.WriteAttribute("w:author", moveRangeStart.Author);
            // MS Word writes range start date as is, without conversion to UTC.
            mBuilder.WriteAttribute("w:date", moveRangeStart.Date, DateTimeKind.Unspecified);
            mBuilder.WriteAttribute("w:name", moveRangeStart.Name);

            WriteDisplacedByCustomXmlAttribute(moveRangeStart);

            if (moveRangeStart.IsColumn)
            {
                mBuilder.WriteAttribute("w:colFirst", moveRangeStart.FirstColumn);
                mBuilder.WriteAttribute("w:colLast", moveRangeStart.LastColumn);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitSubDocument(SubDocument subDocument)
        {
            OpcPackagePart docPart = Package.GetPartByRelationshipType(null, RelTypes.OfficeDocument);
            string relId = docPart.Rels.Add(RelTypes.SubDocument, UriUtil.EscapeHref(subDocument.FileName), true);
            mBuilder.WriteRelationshipId("w:subDoc", relId);

            return VisitorAction.Continue;
        }

        private static bool CanBeWritten(Comment comment)
        {
            // WORDSNET-13825 Although comment as direct child of cell is supported in RTF, it cannot be written to docx.
            if (comment.ParentNode.NodeType == NodeType.Cell)
                return false;

            return true;
        }

        /// <summary>
        /// We map model comment/editable range ids into DOCX annotation ids when writing.
        /// </summary>
        private int IdToAnnotationId(int objectId)
        {
            int annotationId = mIdToAnnotationId[objectId];
            if (!IntToIntDictionary.IsNullSubstitute(annotationId))
                return annotationId;

            annotationId = GetNextAnnotationId();
            mIdToAnnotationId[objectId] = annotationId;
            return annotationId;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitFootnoteStart(Footnote footnote)
        {
            mRunWriter.WriteRunStart(footnote.RunPr, footnote.RunPr, false);

            switch (footnote.FootnoteType)
            {
                case FootnoteType.Footnote:
                    mRunWriter.CurrentFootnoteType = FootnoteType.Footnote;
                    mFootnotesWriter.WriteFootnoteStart();
                    break;
                case FootnoteType.Endnote:
                    mRunWriter.CurrentFootnoteType = FootnoteType.Endnote;
                    mEndnotesWriter.WriteFootnoteStart();
                    break;
                default:
                    throw new InvalidOperationException("Unknown footnote type.");
            }

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitFootnoteEnd(Footnote footnote)
        {
            DocxFootnotesWriter writer;
            string elemName;

            switch (footnote.FootnoteType)
            {
                case FootnoteType.Footnote:
                    writer = mFootnotesWriter;
                    elemName = "w:footnoteReference";
                    break;
                case FootnoteType.Endnote:
                    writer = mEndnotesWriter;
                    elemName = "w:endnoteReference";
                    break;
                default:
                    throw new InvalidOperationException("Unknown footnote type.");
            }

            int id = writer.WriteFootnoteEnd();

            // WORDSNET-4993 writing customMarkFollows attribute if required along with element id.
            mBuilder.StartElement(elemName);
            mBuilder.WriteAttributeIfTrue("w:customMarkFollows", !footnote.IsAuto);
            mBuilder.WriteAttribute("w:id", id);
            mBuilder.EndElement(elemName);

            if (!footnote.IsAuto)
                WriteReferenceMark(mBuilder, footnote);

            mRunWriter.WriteRunEnd(footnote.RunPr);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// WORDSNET-4993, 5323, 5324, 12842
        /// Writing reference mark
        /// </summary>
        internal static void WriteReferenceMark(NrxXmlBuilder builder, Footnote footnote)
        {
            bool isWritten = false;

            if (footnote.ReferenceMark.Length == 1)
            {
                char refMark = footnote.ReferenceMark[0];
                if (refMark == ControlChar.LineBreakChar)
                {
                    isWritten = true;
                    builder.WriteElement("w:br", footnote.ReferenceMark);
                }
                if (refMark == ControlChar.TabChar)
                {
                    isWritten = true;
                    builder.WriteEmptyElement("w:tab");
                }
                else if (StringUtil.IsPrivateUseCategory(refMark))
                {
                    isWritten = true;
                    builder.WriteElementWithAttributes(
                        "w:sym",
                        "w:font", footnote.RunPr.NameAscii,
                        "w:char", FormatterPal.IntToStrX4(refMark));
                }
            }

            if (!isWritten)
            {
                string elementName = "w:t";

                if (footnote.IsDeleteRevision)
                    elementName = "w:delText";
                else if (footnote.IsInsertRevision)
                    elementName = "w:instrText";

                // WORDSNET-5858 We should specify xml:space="preserve", to preserve space char in footnote's referenceMark.
                builder.WriteElementWithAttributesStart(elementName, "xml:space", "preserve");
                builder.WriteString(footnote.ReferenceMark);
                builder.EndElement();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitGroupShapeStart(GroupShape groupShape)
        {
            if (groupShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLStart(groupShape);

            if (groupShape.IsTopLevel)
                WriteTopLevelShapeStart(groupShape);

            VmlShapeWriter.Write(groupShape, mBuilder, this);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitGroupShapeEnd(GroupShape groupShape)
        {
            if (groupShape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLEnd(groupShape);

            VmlShapeWriter.WriteWrapAndAnchorLock(groupShape, mBuilder);
            mBuilder.EndElement("v:group");

            if (groupShape.IsTopLevel)
                WriteTopLevelShapeEnd(groupShape);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitShapeStart(Shape shape)
        {
            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
            {
                Shape dmlShape = shape;
                if ((!shape.HasHyperlink) && mFieldsWriter.IsInsideField && mFieldsWriter.IsHyperlinkLastChild(shape))
                    dmlShape = mFieldsWriter.GetHyperlinkShape(shape);
                return VisitDrawingMLStart(dmlShape);
            }

            // WORDSNET-19075 Write as DML only top level images
            bool isNeedConvertToDml = mDoc.DocPr.CompatibilityOptions.IsNeedConvertToNewerVersion;
            // WORDSNET-11815 Do not convert shapes to DML when used Ecma376.
            isNeedConvertToDml &= Compliance != OoxmlComplianceCore.Ecma376;

            // FOSS

            // If the shape is a hyperlinked inline image, then we need to enclose it inside "w:hyperlink" declaration.
            if (shape.IsHyperlinkedInline)
                mFieldsWriter.WriteHyperlinkStart(shape);

            if (shape.IsTopLevel)
                WriteTopLevelShapeStart(shape);

            VmlShapeWriter.Write(shape, mBuilder, this);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override VisitorAction VisitShapeEnd(Shape shape)
        {
            if (mDmlsConvertedFromVmls.Count > 0)
            {
                Shape dml = mDmlsConvertedFromVmls.Pop();
                return VisitDrawingMLEnd(dml);
            }

            if (shape.MarkupLanguage == ShapeMarkupLanguage.Dml)
                return VisitDrawingMLEnd(shape);

            // If the shape has child nodes, they are written inside 'v:textbox', which is a child element of 'v:shape'.
            // 'v:textbox' and 'w:txbxContent' elements start is written inside VmlShapeWriter in this case.
            // That's why you don't see starting of these elements here in WmlWriter.
            if (shape.HasChildNodes)
            {
                mBuilder.EndElement("w:txbxContent");
                mBuilder.EndElement("v:textbox");
            }

            // shape element is NOT written for OLE controls that are inline.
            VmlShapeWriter.WriteWrapAndAnchorLock(shape, mBuilder);
            mBuilder.EndElement();    // "v:shape" or "v:oval", "v:rect" etc.
            if(shape.IsOle)
                WriteOle(shape);

            if (shape.IsTopLevel)
                WriteTopLevelShapeEnd(shape);

            if (shape.IsHyperlinkedInline)
                mFieldsWriter.WriteHyperlinkEnd();

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private VisitorAction VisitDrawingMLStart(ShapeBase drawingML)
        {
            Debug.Assert(drawingML != null);

            // Skip nested DrawingML nodes in charts, they are written as user shapes in a special way.
            // Chart can not contain nested shapes according to 5.7.2.26 part (Reference to Chart Part).
            // Keep DML fallbacks for chartEx shapes.
            if ((mCurrentDmlChart != null) &&
                !((drawingML == mCurrentDmlChart.FallbackShape) && DmlUtil.NeedDmlFallback(mCurrentDmlChart)))
                return VisitorAction.SkipThisNode;

            // Set current context, i.e. currently DML chart is in process.
            if ((drawingML.DmlNode.DmlNodeType == DmlNodeType.Chart) ||
                (drawingML.DmlNode.DmlNodeType == DmlNodeType.ChartEx))
                mCurrentDmlChart = drawingML;

            if (drawingML.IsTopLevel)
            {
                // FOSS We can't generate a fallback shape (DML->VML conversion needs rendering),
                // so only wrap in mc:AlternateContent when there is an existing FallbackShape to preserve.
                if (DmlUtil.NeedFallback(drawingML, Compliance) && drawingML.FallbackShape != null)
                {
                    mRunWriter.WriteRunStart(drawingML.RunPr, drawingML, true);
                    StartAlternateContent(drawingML);
                }
                else if (mFallbackDeeps == 0) // We do not need to open new run if it is already opened for AlternateContent.
                {
                    mRunWriter.WriteRunStart(drawingML.RunPr, drawingML, true);
                }
            }

            DmlDrawingWriter.WriteStart(drawingML, this);

            return VisitorAction.Continue;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private VisitorAction VisitDrawingMLEnd(ShapeBase drawingML)
        {
            Debug.Assert(drawingML != null);

            DmlDrawingWriter.WriteEnd(drawingML, this);

            if (drawingML.IsTopLevel)
            {
                // FOSS Mirror of the gate in VisitDrawingMLStart.
                if (DmlUtil.NeedFallback(drawingML, Compliance) && drawingML.FallbackShape != null)
                {
                    EndAlternateContent(drawingML);
                    mRunWriter.WriteRunEnd(drawingML.RunPr);
                }
                else if (mFallbackDeeps == 0)
                {
                    mRunWriter.WriteRunEnd(drawingML.RunPr);
                }
            }

            // Reset context.
            if ((drawingML.DmlNode.DmlNodeType == DmlNodeType.Chart) ||
                (drawingML.DmlNode.DmlNodeType == DmlNodeType.ChartEx))
                mCurrentDmlChart = null;

            return VisitorAction.Continue;
        }

        internal void StartAlternateContent(ShapeBase shape)
        {
            mBuilder.StartElement("mc:AlternateContent");
            mBuilder.StartElement("mc:Choice");
            mBuilder.WriteAttribute("Requires", GetChoiceRequires(shape));
        }

        internal void EndAlternateContent(ShapeBase shape)
        {
            mBuilder.EndElement("mc:Choice");
            mBuilder.StartElement("mc:Fallback");
            mFallbackDeeps++;

            // Try getting alternate content read from the document.
            if (shape.FallbackShape != null)
            {
                shape.FallbackShape.Accept(this);
            }
            else
            {
                // FOSS
            }

            mBuilder.EndElement("mc:Fallback");
            mFallbackDeeps--;

            mBuilder.EndElement("mc:AlternateContent");
        }

        /// <summary>
        /// Writes necessary elements for Ole controls and objects.
        /// </summary>
        private void WriteOle(Shape shape)
        {
            Debug.Assert(shape.IsOle);

            switch (shape.ShapeType)
            {
                case ShapeType.OleObject:
                    WriteOleObject(shape);
                    break;
                case ShapeType.OleControl:
                    WriteOleControl(shape);
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        /// <summary>
        /// Writes 'o:OLEObject' element for the specified shape.
        /// </summary>
        private void WriteOleObject(Shape shape)
        {
            Debug.Assert(shape.IsOleObject);

            //    <o:OLEObject Type="Embed" ProgID="Equation.3" ShapeID="_x0000_i1025" DrawAspect="Content" ObjectID="_1254857050" r:id="rId5" />

            OleFormat oleFormat = shape.OleFormat;

            mBuilder.StartElement("o:OLEObject");

            mBuilder.WriteAttribute("Type", oleFormat.NeedEmbeddedPart ? "Embed" : "Link");
            mBuilder.WriteAttribute("ProgID", oleFormat.ProgId);
            mBuilder.WriteAttribute("ShapeID", VmlUtil.GetOleObjectShapeId(shape));
            mBuilder.WriteAttribute("DrawAspect", oleFormat.OleIcon ? "Icon" : "Content");

            if (!oleFormat.NeedEmbeddedPart)
            {
                // Write a link to the object.
                string moniker = StringUtil.HasChars(oleFormat.SourceItem)
                    ? string.Format("{0}!{1}", oleFormat.SourceFullName, oleFormat.SourceItem)
                    : oleFormat.SourceFullName;

                string relId = mBuilder.Part.Rels.Add(RelTypes.OleObject, moniker, true);
                mBuilder.WriteAttribute("r:id", relId);

                mBuilder.WriteAttribute("UpdateMode", oleFormat.AutoUpdate ? "Always" : "OnCall");

                mBuilder.WriteElement("o:LinkType", VmlEnum.OleLinkTypeToVml(oleFormat.OleLinkType));

                mBuilder.WriteValIfTrue("o:LockedField", oleFormat.IsLocked);

                // RK Note in WordML this is o:WordFieldCodes
                mBuilder.WriteElement("o:FieldCodes", "\\f " + oleFormat.FormatUpdateType);
            }
            else
            {
                // Write an embedded OLE or OOXML object.
                mBuilder.WriteAttribute("ObjectID", OleUtil.GetMsWordId(oleFormat.EmbeddedObject));

                if (oleFormat.OleObject != null)
                    WriteEmbeddedOleObject(oleFormat);
                else if (oleFormat.OoxmlObject != null)
                    WriteEmbeddedOoxmlObject(oleFormat);
            }

            mBuilder.EndElement("o:OLEObject");
        }

        /// <summary>
        /// Writes an embedded object as an OLE object.
        /// </summary>
        private void WriteEmbeddedOleObject(OleFormat oleFormat)
        {
            OpcPackagePart part = CreateEmbeddedObjectPart(oleFormat, "oleObject", RelTypes.OleObject);
            FileSystem fs = new FileSystem(oleFormat.OleObject.Data);
            fs.Save(part.Stream);
        }

        /// <summary>
        /// Writes an embedded object as an OOXML package.
        /// </summary>
        private void WriteEmbeddedOoxmlObject(OleFormat oleFormat)
        {
            OpcPackagePart part = CreateEmbeddedObjectPart(oleFormat, "ooxmlPackage", RelTypes.Package);

            // andrnosk: WORDSNET-7678 We have to copy the stream before giving it to the part,
            // to avoid referencing two or more parts to the same stream (for example upon appending documents).
            oleFormat.OoxmlObject.Stream.Position = 0;
            StreamUtil.CopyStream(oleFormat.OoxmlObject.Stream, part.Stream);
            part.Stream.Position = 0;
        }


        /// <summary>
        /// Gets the content type valid for saving into an OOXML document.
        /// </summary>
        private static string GetContentType(OleFormat oleFormat)
        {
            OoxmlObject ooxmlObject = oleFormat.OoxmlObject;
            if ((ooxmlObject != null) && StringUtil.HasChars(ooxmlObject.ContentType))
                return ooxmlObject.ContentType;

            return OleUtil.GetContentType(oleFormat.ProgId);
        }

        private OpcPackagePart CreateEmbeddedObjectPart(OleFormat oleFormat, string fileNameBase, string relType)
        {
            string contentType = GetContentType(oleFormat);

            // OLE objects for both main document and glossary are stored in the same hardcoded place.
            string partName = string.Format("/word/embeddings/{0}{1}{2}",
                fileNameBase, GetNextEmbeddedPartNumber(relType), DocxEnum.ContentTypeToExtension(contentType));

            string relId;
            OpcPackagePart part = Package.CreateChildPart(mBuilder.Part, partName, contentType, relType, out relId);
            mBuilder.WriteAttribute("r:id", relId);

            return part;
        }

        /// <summary>
        /// Writes 'w:control' element for the specified shape.
        /// </summary>
        private void WriteOleControl(Shape shape)
        {
            Debug.Assert(shape.IsOleControl);
            Debug.Assert(shape.EmbeddedObject != null);

            mBuilder.StartElement("w:control");

            int partNumber = GetNextEmbeddedPartNumber(RelTypes.Control);

            string xmlRelId;
            OpcPackagePart xmlPart = Package.CreateChildPart(
                mBuilder.Part, string.Format("/word/activeX/activeX{0}.xml", partNumber), DocxContentType.Control, RelTypes.Control, out xmlRelId);

            // It can be either OOXmlObject or OleObject, or one of the its parsed versions of OleControl type.
            // See OleControl.Create() for details.
            string name = shape.IsOoxmlControl ? WriteOoxmlControl(shape, xmlPart) : WriteBinaryControl(shape, xmlPart, partNumber);

            // Write all attributes and close tag.
            mBuilder.WriteAttribute("r:id", xmlRelId);
            mBuilder.WriteAttributeString("w:name", name);
            mBuilder.WriteAttributeString("w:shapeid", NrxXmlUtil.GetShapeId(shape));
            mBuilder.EndElement("w:control");
        }

        /// <summary>
        /// Writes control represented as OoxmlObject.
        /// </summary>
        /// <returns>Name of written control.</returns>
        private static string WriteOoxmlControl(ShapeBase shape, OpcPackagePart xmlPart)
        {
            OoxmlObject ooxmlObject = shape.EmbeddedObject as OoxmlObject;
            Debug.Assert(ooxmlObject != null);

            ooxmlObject.Stream.Position = 0;
            StreamUtil.CopyStream(ooxmlObject.Stream, xmlPart.Stream);

            return ((IEmbeddedObject)ooxmlObject).GetName();
        }

        /// <summary>
        /// Writes ActiveX control represented as OleObject.
        /// </summary>
        /// <returns>Name of written control.</returns>
        private string WriteBinaryControl(ShapeBase shape, OpcPackagePart xmlPart, int partNumber)
        {
            IEmbeddedObject embeddedObject = shape.EmbeddedObject;
            Debug.Assert(embeddedObject != null);
            OleObject oleObject = embeddedObject.GetOleObject();
            Debug.Assert(oleObject != null);

            FileSystem fs = new FileSystem(oleObject.Data);

            xmlPart.Rels.Add(@"rId1", RelTypes.ControlBinary,
                             string.Format("activeX{0}.bin", partNumber), false);

            // XML control part is always the same so I decided to write it from constant string.
            string xml = string.Format(ControlXmlPartContent, fs.Root.Clsid.ToString().ToUpper(),
                                       DocxNamespaces.ActiveX, DocxNamespaces.Relationships);
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);
            xmlPart.Stream.Write(xmlBytes, 0, xmlBytes.Length);

            OpcPackagePart binPart = Package.CreateChildPart(
                xmlPart, string.Format("/word/activeX/activeX{0}.bin", partNumber), DocxContentType.ControlBinary,
                RelTypes.ControlBinary);

            fs.Save(binPart.Stream);
            binPart.Stream.Position = 0;

            // WORDSNET-11823 If name of shape was explicitly set, we should use it as the name for this OLE control.
            string name = (StringUtil.HasChars(shape.Name)) ? shape.Name: embeddedObject.GetName();

            // WORDSNET-18020 Generate default name of the OLE control if it is not specified.
            return (StringUtil.HasChars(name)) ? name : string.Format("OleControl {0}", shape.Id);
        }

        /// <summary>
        /// Writes the 'w:pPr' element.
        /// </summary>
        /// <param name="para">Paragraph to write properties for.</param>
        private void WriteParaPr(Paragraph para)
        {
            // TODO 6139. See "[MS-OI29500] 2.1.242 Part 1 Section 17.7.5.2, pPr (Paragraph Properties)".
            // The standard states that the cnfStyle, divId, pPrChange, pStyle, rPr, and sectPr elements are valid children of the pPr element.
            // Word does not allow these elements to be children of the pPr element.
            ParaPr paraPr = para.ParaPr;

            // The logic involving pPrStartWritten is implemented to avoid writing empty w:pPr elements.
            bool pPrStartWritten = NrxParaPrWriter.WriteStart(paraPr, this);

            // if w:pPr start element is not written yet - write it from RunPrWriter.
            // We pass !pPrStartWritten parameter to control that.
            bool rPrWritten = DocxRunPrWriter.Instance.WriteForNodes(para.ParagraphBreakRunPr, para, !pPrStartWritten, this);

            // If rPr was written inside mRunWriter.WriteProps, then we can be sure that pPr is already written too.
            pPrStartWritten = pPrStartWritten || rPrWritten;

            // if the paragraph is a last paragraph of non last section
            if (para == mLastParagraphOfCurrentSection && !mCurrentSection.IsLastChild)
            {
                // If w:pPr start element was not written yet, then we should write it here to enclose w:sectPr element.
                if (!pPrStartWritten)
                {
                    mBuilder.StartElement("w:pPr");
                    pPrStartWritten = true;
                }

                // In WordML section properties are written inside paragraph properties (w:pPr element) of the last paragraph of a section.
                // However, section properties of the last section of the document are written on document body level (w:body).
                NrxSectPrWriter.Write(mCurrentSection, this);
            }

            // if w:pPr start element was written, write end element for it here.
            if (pPrStartWritten)
                mBuilder.EndElement("w:pPr");
        }

        private void WriteTopLevelShapeStart(ShapeBase shape)
        {
            if (NeedFallbackForVml(shape))
            {
                mRunWriter.WriteRunStart(shape.RunPr, shape, true);
                StartAlternateContent(shape);
            }
            else if (mFallbackDeeps == 0) // We do not need to open new run if it is already opened for AlternateContent.
            {
                mRunWriter.WriteRunStart(shape.RunPr, shape, false);
            }

            // RK This is DOCX specific. Inline OLE objects are written as w:object,
            // but all other shapes are written as w:pict.
            if (shape.IsInline && (shape.IsOleControl || shape.IsOleObject))
            {
                mBuilder.StartElement("w:object");
                // RK There are w:dxaOrig and w:dyaOrig attributes that we don't write here.
                // They seem to be optional so we can live without.
                // If we really need them, they must be taked from Picf.dxaGoal and dyaGoal.
            }
            else
            {
                mBuilder.StartElement("w:pict");
            }
        }

        private void WriteTopLevelShapeEnd(ShapeBase shape)
        {
            mBuilder.EndElement();    //w:pict or w:object
            if (NeedFallbackForVml(shape))
            {
                EndAlternateContent(shape);
                mRunWriter.WriteRunEnd(shape.RunPr);
            }
            else if (mFallbackDeeps == 0)
            {
                mRunWriter.WriteRunEnd(shape.RunPr);
            }
        }

        /// <summary>
        /// Returns true if VML shape should be written as alternate content block.
        /// </summary>
        private bool NeedFallbackForVml(ShapeBase shape)
        {
            return shape.IsTopLevel && (shape.FallbackShape != null) && (Compliance == OoxmlComplianceCore.IsoStrict);
        }

        /// <summary>
        /// Returns value of the "Requires" attribute of the "Choice" element for specified shape.
        /// </summary>
        /// <param name="dml">Shape to obtain required value.</param>
        /// <returns>Requested value of the attribute.</returns>
        /// <remarks>
        /// This method called to obtain "Requires" attribute when fallback shape is needed.
        /// MS Word always writes fallback shape for Wordprocessing Shape, Group and Canvas - so we do the same.
        /// Graphic frame is not used as top level shape, so this method does not use frame value.
        /// </remarks>
        internal static string GetChoiceRequires(ShapeBase dml)
        {
            Debug.Assert(dml != null);

            if (dml.MarkupLanguage == ShapeMarkupLanguage.Vml)
                return "v";

            DmlNodeType dmlType = dml.DmlNode.DmlNodeType;

            switch (dmlType)
            {
                case DmlNodeType.WordprocessingCanvas:
                    return "wpc";
                case DmlNodeType.WordprocessingGroupShape:
                    return "wpg";
                case DmlNodeType.WordprocessingShape:
                    return "wps";
                case DmlNodeType.ContentPart:
                    return "wpi";
                case DmlNodeType.ChartEx:
                    return "cx1";
                default:
                    throw new ArgumentException("Unexpected DrawingML type.");
            }
        }

        /// <summary>
        /// IVmlShapeWriterContext, IDmlShapeWriterContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string WriteImageBinData(byte[] imageBytes)
        {
            string imagePartName = mWriter.GetImagePartName(imageBytes);
            if (!StringUtil.HasChars(imagePartName))
            {
                FileFormat imageType = ImageUtil.GetImageType(imageBytes);

                // WORDSNET-13400 Try DIB image.
                if ((imageType == FileFormat.Unknown) && ImageUtil.IsDib(imageBytes))
                {
                    BinaryReader reader = new BinaryReader(new MemoryStream(imageBytes));
                    imageBytes = ImageUtil.PrependBmpHeader(reader, imageBytes.Length);
                    imageType = FileFormat.Bmp;
                }
                else if (imageType == FileFormat.Pict)
                {
                    imageBytes = VmlUtil.PackPictImage(imageBytes);
                }

                // RK This is by design. Images for both main document and glossary are stored in one place.
                string partFileNameWithoutExt = "/word/media/image" + GetNextEmbeddedPartNumber(RelTypes.Image);
                OpcPackagePart part = CreateImagePart(imageType, imageBytes, partFileNameWithoutExt);
                Package.Parts.Add(part);

                imagePartName = part.Name;
                mWriter.AddImageBytes(imageBytes, imagePartName);
            }

            return mBuilder.Part.Rels.Add(RelTypes.Image, imagePartName, false);
        }

        internal static OpcPackagePart CreateImagePart(FileFormat imageType, byte[] bytesToWrite,
            string partFileNameWithoutExt)
        {
            string imageContentType = FileFormatCore.ToContentType(imageType);

            // Placed here to avoid write unknown content type for other than image document parts.
            if (!StringUtil.HasChars(imageContentType))
                imageContentType = "image/unknown";

            string imageExt = FileFormatCore.ToExt(imageType);
            if (!StringUtil.HasChars(imageExt))
                imageExt = "bin";

            switch (imageType)
            {
                case FileFormat.Bmp:
                case FileFormat.Gif:
                case FileFormat.Png:
                case FileFormat.Jpeg:
                case FileFormat.Emf:
                case FileFormat.Wmf:

                // WORDSNET-21107 Implement WebP image format rendering
                case FileFormat.WebP:

                // WORDSNET-19122 Added support for Ico format.
                case FileFormat.Ico:

                // WORDSNET-21677 The problem occurred because we did not handle tiff images.
                // Fixed by adding a case for Tiff format.
                case FileFormat.Tiff:

                // WORDSNET-16164 Added support for svgBlip drawing extension.
                case FileFormat.Svg:

                // andrnosk: WORDSNET-5024 (part 1) There is PDF in media in the DOCX document. To preserve it upon
                // roundtrip process it as an media data. Since we do not know what formats can also appear in media,
                // currently process only PDFs, otherwise throw an exception.
                // WORDSNET-8002 Added QuickTime MOV to be processed as media data too.
                case FileFormat.Pdf:
                case FileFormat.Mov:
                // WORDSNET-15267 Allow image with unknown format to be written.
                case FileFormat.Unknown:
                    break;

                case FileFormat.Pict:
                    imageExt = "pcz";
                    imageContentType = DocxContentType.ImagePictCompressed;
                    break;

                default:
                    throw new InvalidOperationException("Unrecognized image type encountered during DOCX export.");
            }

            string imagePartName = partFileNameWithoutExt + "." + imageExt;

            OpcPackagePart part = new OpcPackagePart(imagePartName, imageContentType);
            part.Stream.Write(bytesToWrite, 0, bytesToWrite.Length);

            return part;
        }

        /// <summary>
        /// IVmlShapeWriterContext, IDmlShapeWriterContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string WriteImageLink(string link)
        {
            // andrnosk: WORDSNET-7681 There is unescaped char in the link, we should escape this before writing to rels.
            return mBuilder.Part.Rels.Add(RelTypes.Image, UriUtil.EscapeHref(link), true);
        }

        /// <summary>
        /// Converts the specified <see cref="DrColor"/> to VML color string.
        /// </summary>
        /// <remarks>
        /// Logs a warning to the user-provided warning callback if this is a special color definition (system or scheme
        /// color or palette index) that is currently not supported.
        /// </remarks>
        public string ColorToVml(DrColor color)
        {
            string colorString = VmlColor.ColorToVml(color);
            if (colorString == null)
            {
                // Restore original 'A' value that was inverted in WordColorUtil.WordXPColorToColor.
                DrColor originalColor = new DrColor(~color.A & 0xff, color);
                string colorCode = FormatterPal.IntToStrX8(originalColor.ToArgb());
                string message = string.Format(WarningStrings.UnsupportedVmlColor, "DOCX", "0x" + colorCode);

                Warn(WarningType.MajorFormattingLoss, message);
            }

            return colorString;
        }

        /// <summary>
        /// Gets diagram member name by its hashcode.
        /// MS Word stores hashcodes, not names, in diagram relation tables. And in WordML we need to write names.
        /// </summary>
        /// <param name="key">Hashcode of the diagram member.</param>
        /// <returns>Name of the diagram member.</returns>
        string IVmlShapeWriterContext.GetDiagramMemberName(int key)
        {
            if (mDiagramReferences == null)
            {
                mDiagramReferences = new IntToObjDictionary<string>();

                foreach (ShapeBase shape in Document.GetChildNodes(NodeType.Shape, true))
                {
                    string name = shape.Name;
                    if (name.StartsWith("_s", StringComparison.Ordinal))
                    {
                        // WORDSNET-7710 The problem occurred because there are two shapes with the same name.
                        // Fixed by checking if name of the shape is already added to DiagramReferences.
                        // The same code is used in WmlContext class. Consider refactoring.
                        mDiagramReferences[VmlUtil.CalculateDiagramHash(name)] = name;
                    }
                }
            }

            return mDiagramReferences[key];
        }

        string IVmlShapeWriterContext.ImageSrcAttributeName
        {
            get { return "r:id"; }
        }

        string IVmlShapeWriterContext.ImageHrefAttributeName
        {
            get { return "r:href"; }
        }

        /// <summary>
        /// Keeps record of shapetype definitions already written to output document.
        /// </summary>
        IList<string> IVmlShapeWriterContext.ShapeTypesWritten
        {
            get { return mBuilder.ShapeTypesWritten; }
        }

        /// <summary>
        /// IVmlShapeWriterContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public SaveInfo SaveInfo
        {
            get { return mWriter.SaveInfo; }
        }

        /// <summary>
        /// INrxWriterContext
        ///
        /// Gets the WordML-compliant style identifier for the specified style's istd.
        /// Use for 'w:styleId', 'w:basedOn', 'w:next', 'w:link', 'w:pStyle' and 'w:rStyle' attributes.
        /// And also in 'w:styleLink' and 'w:listStyleLink' elements in list definitions.
        ///
        /// This value is for XML cross-reference only.
        /// It should be unique and be compliant with xml specification for id fields.
        /// Means it can include only english letters, digits or '-' sign. No other characters are allowed.
        /// Otherwise, xsl processors won't be able to handle the produced WordML file correctly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GetStyleId(int istd)
        {
            return mStyleIdGenerator.GetStyleId(istd);
        }

        /// <summary>
        /// INrxWriterContext, IMathWriterContext, IDmlShapeWriterContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public NrxXmlBuilder Builder
        {
            get { return mBuilder; }
        }

        /// <summary>
        /// INrxWriterContext, IMathWriterContext, IVmlShapeWriterContext
        /// </summary>
        public bool IsDocx
        {
            get { return true; }
        }

        /// <summary>
        /// INrxWriterContext, IMathWriterContext, IDmlShapeWriterContext. Based on the value of this data member we will
        /// write different attributes, elements into the document.
        /// </summary>
        public OoxmlComplianceCore Compliance
        {
            get { return OoxmlComplianceInfo.GetCompliance(mWriter.Document.ComplianceInfo, SaveOptions); }
        }

        /// <summary>
        /// INrxWriterContext, IMathWriterContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DocumentBase Document
        {
            get { return mDoc; }
        }

        void INrxWriterContext.WriteHeaderFooter(HeaderFooter headerFooter)
        {
            // WORDSNET-18748 By default, sections other than the first section inherit the previous header and footer
            // references, unless that section specifies header and footer references
            if (!headerFooter.IsLinkedToPrevious)
            {
                string partName;
                string contentType;
                string rootName;
                string refName;

                if (headerFooter.IsHeader)
                {
                    partName = string.Format("header{0}.xml", GetNextHeaderNumber());
                    contentType = DocxContentType.Header;
                    rootName = "w:hdr";
                    refName = "w:headerReference";
                }
                else
                {
                    partName = string.Format("footer{0}.xml", GetNextFooterNumber());
                    contentType = DocxContentType.Footer;
                    rootName = "w:ftr";
                    refName = "w:footerReference";
                }

                string relType = headerFooter.IsHeader ? RelTypes.Header : RelTypes.Footer;
                string relId;
                DocxBuilder builder = CreateChildPartAndBuilder(partName, contentType, relType, out relId);
                builder.StartDocumentWithStandardNamespaces(rootName);

                // Accept visitor for the header/footer builder.
                PushBuilder(builder);
                headerFooter.Accept(this);
                PopBuilder();

                builder.EndDocument();

                // Write the relationship source into the main document part.
                NrxXmlBuilder mainDocumentBuilder = CurrentBuilder;
                mainDocumentBuilder.StartElement(refName);
                mainDocumentBuilder.WriteAttributeString("w:type", NrxSectEnum.HeaderFooterTypeToXml(headerFooter.HeaderFooterType, true));
                mainDocumentBuilder.WriteAttributeString("r:id", relId);
                mainDocumentBuilder.EndElement();
            }
        }

        /// <summary>
        /// INrxWriterContext
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void WriteFootnotePr(AttrCollection attrs, bool writeSeparators)
        {
            mFootnotesWriter.WriteFootnotePr(attrs, writeSeparators);
            mEndnotesWriter.WriteFootnotePr(attrs, writeSeparators);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void WriteW14Attributes(INrxWriterContext writer, RunPr runPr)
        {
            Debug.Assert(writer.IsDocx);

            DocxDocumentWriterBase docxWriter = (DocxDocumentWriterBase)writer;

            // Do not write these attrs if customer requests for ECMA386 document.
            if (docxWriter.Compliance == OoxmlComplianceCore.Ecma376)
                return;

            NrxXmlBuilder builder = writer.Builder;

            object value = runPr.GetDirectAttr(FontAttr.EffectGlow);
            if (!IsNullDmlEffect(value))
                DmlShapeEffectsWriter.WriteGlow((DmlShapeGlowEffect)value, docxWriter, true);

            value = runPr.GetDirectAttr(FontAttr.EffectShadow);
            if (!IsNullDmlEffect(value))
                DmlShapeEffectsWriter.WriteOuterShadow((DmlShapeOuterShadowEffect)value, docxWriter, true);

            value = runPr.GetDirectAttr(FontAttr.EffectReflection);
            if (!IsNullDmlEffect(value))
                DmlShapeEffectsWriter.WriteReflection((DmlShapeReflectionEffect)value, docxWriter, true);

            value = runPr.GetDirectAttr(FontAttr.EffectOutline);
            if (!IsNullDmlEffect(value))
                DmlOutlineWriter.Write("w14:textOutline", (DmlOutline)value, docxWriter);

            value = runPr.GetDirectAttr(FontAttr.EffectFill);
            if (!IsNullDmlEffect(value))
            {
                builder.StartElement("w14:textFill");
                DmlFillWriter.Write((DmlFill)value, docxWriter, true);
                builder.EndElement("w14:textFill");
            }

            value = runPr.GetDirectAttr(FontAttr.EffectScene3D);
            if (!IsNullDmlEffect(value))
                Dml3DPropertiesWriter.WriteScene3D((DmlScene3DProperties)value, docxWriter, true);

            value = runPr.GetDirectAttr(FontAttr.EffectProps3D);
            if (!IsNullDmlEffect(value))
                Dml3DPropertiesWriter.WriteShape3D((DmlShape3DProperties)value, docxWriter, true);

            value = runPr.GetDirectAttr(FontAttr.OpenTypeLigature);
            if (value != null)
                builder.WriteVal("w14:ligatures", DocxEnum.LigatureToXml((Ligature)value));

            value = runPr.GetDirectAttr(FontAttr.OpenTypeNumForm);
            if (value != null)
                builder.WriteVal("w14:numForm", DocxEnum.NumFormToXml((NumForm)value));

            value = runPr.GetDirectAttr(FontAttr.OpenTypeNumSpacing);
            if (value != null)
                builder.WriteVal("w14:numSpacing", DocxEnum.NumSpacingToXml((NumSpacing)value));

            value = runPr.GetDirectAttr(FontAttr.OpenTypeStylisticSets);
            if (value != null)
                WriteStylisticSets(builder, (StylisticSets)value);

            if (runPr.Contains(FontAttr.OpenTypeContextualAlternates))
                builder.WriteBoolValExplicit("w14:cntxtAlts", (bool)runPr.GetDirectAttr(FontAttr.OpenTypeContextualAlternates));
        }

        public string CurrentParagraphListLabelString
        {
            get { return null; }
        }

        public string CurrentParagraphListLabelFontName
        {
            get { return null; }
        }

        void IMathWriterContext.WriteRunPr(RunPr runPr)
        {
            DocxRunPrWriter.Instance.WriteForNodes(runPr, null, false, this);
        }

        /// <summary>
        /// Writes an element of the specified name that has only the w:id attribute.
        /// Used for comments, footnotes and endnotes.
        /// </summary>
        /// <param name="elemName">String, containing the name of the element to write.</param>
        /// <param name="id">Reference id.</param>
        internal void WriteElementWithId(string elemName, int id)
        {
            mBuilder.StartElement(elemName);
            mBuilder.WriteAttribute("w:id", id);
            mBuilder.EndElement(elemName);
        }

        /// <summary>
        /// Check attribute with DmlEffect to null value.
        /// </summary>
        /// <param name="attr">Attribute to check.</param>
        /// <returns>true if the attribute is null.</returns>
        private static bool IsNullDmlEffect(object attr)
        {
            return (attr == null) || (attr is DmlNullEffect);
        }

        private int GetNextHeaderNumber()
        {
            mHeaderNumber++;
            return mHeaderNumber;
        }

        private int GetNextFooterNumber()
        {
            mFooterNumber++;
            return mFooterNumber;
        }

        /// <summary>
        /// Gets a next unique value for writing as w:id of annotation elements in DOCX/WordML
        /// (comments, bookmarks and revision marks, etc).
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GetNextAnnotationId()
        {
            return mAnnotationId++;
        }

        /// <summary>
        /// Generated names for embedded parts are sequentially numbered. There is one sequence per relationship type.
        /// One sequence goes across the main and glossary documents.
        /// </summary>
        internal int GetNextEmbeddedPartNumber(string relType)
        {
            return mWriter.GetNextEmbeddedPartNumber(relType);
        }

        /// <summary>
        /// Adds a hyperlink relationship to the current part and returns the id.
        /// </summary>
        public string AddHyperlinkRelationship(string dest)
        {
            return mBuilder.Part.Rels.Add(RelTypes.Hyperlink, dest, !UriUtil.IsSubAddressOnly(dest));
        }

        /// <summary>
        /// Adds a external relationship to the ole object to the current part and returns the id.
        /// </summary>
        internal string AddExternalOleObjectRelationship(string dest)
        {
            return mBuilder.Part.Rels.Add(RelTypes.OleObject, dest, true);
        }

        /// <summary>
        /// Creates a part that is a child to the document part and creates an XML builder for the new part.
        /// </summary>
        internal DocxBuilder CreateChildPartAndBuilder(string partName, string contentType, string relType)
        {
            string dummyRelId;
            DocxBuilder result = CreateChildPartAndBuilder(partName, contentType, relType, out dummyRelId);
            return result;  // Need to be on a separate line for autoporter to work.
        }

        /// <summary>
        /// Creates a part that is a child to the document part, creates an XML builder for the new part and returns the relationship id for the part.
        /// </summary>
        private DocxBuilder CreateChildPartAndBuilder(string partName, string contentType, string relType, out string relId)
        {
            return CreateChildPartAndBuilder(mDocumentPart, partName, contentType, relType, out relId);
        }

        internal DocxBuilder CreateChildPartAndBuilder(OpcPackagePart parentPart, string partName, string contentType, string relType, out string relId)
        {
            OpcPackagePart part = Package.CreateChildPart(parentPart, partName, contentType, relType, out relId);
            return new DocxBuilder(part, SaveOptions.PrettyFormat, Compliance, MsWordExtensionsVersion, mWarningCallback);
        }

        internal void PushBuilder(DocxBuilder builder)
        {
            mBuilderStack.Push(mBuilder);
            mBuilder = builder;

            // WORDSNET-21997, 21998. Remember current count of field beginnings and start new counting.
            if (NeedResetFieldStartContext)
                mFieldsWriter.PushFieldStartCounter();
        }

        internal void PopBuilder()
        {
            if (NeedResetFieldStartContext)
                mFieldsWriter.PopFieldStartCounter();

            mBuilder = mBuilderStack.Pop();
        }

        private bool NeedResetFieldStartContext
        {
            get
            {
                string contentType = mBuilder.Part.ContentType;
                return contentType == DocxContentType.Comments ||
                    contentType == DocxContentType.Header ||
                    contentType == DocxContentType.Footer;
            }
        }

        /// <summary>
        /// Writes rels of unparsed OPC part.
        /// used for writing themes and Dml Content parts.
        /// </summary>
        /// <param name="oldPart"></param>
        /// <param name="oldRelatedParts"></param>
        /// <param name="newPart"></param>
        /// <param name="writer"></param>
        internal static void WriteUnparsedRels(
            OpcPackagePart oldPart,
            Dictionary<string, OpcPackagePart> oldRelatedParts,
            OpcPackagePart newPart,
            DocxDocumentWriterBase writer)
        {
            // This all is actually quite a crude hack to preserve all the theme related parts that
            // were specified in the original DOCX. It is a hack because we do not parse the theme XML.
            foreach (OpcRelationship rel in oldPart.Rels)
            {
                if (rel.IsExternal)
                {
                    // For external relationships, it is easy, just use all the values from the relationship
                    // to add a relationship. There is no part to copy.
                    newPart.Rels.Add(rel.Id, rel.Type, rel.Target, rel.IsExternal);
                }
                else
                {
                    // For internal relationships, it is a bit more tricky.
                    // We need to add a part using the same relId, but we need to watch out and not override
                    // any existing part names with the same name.
                    string absolutePartName = newPart.GetRelatedPartName(rel);
                    if (writer.Package.GetPartByName(absolutePartName) != null)
                    {
                        // A part with this name already exists (say image1.jpeg that was written by
                        // the document writing code). We adjust our part name so it is unique.
                        absolutePartName =
                            Path.ChangeExtension(
                                absolutePartName,
                                Guid.NewGuid().ToString() + Path.GetExtension(absolutePartName));
                    }

                    // This is the part that was in the original DOCX.
                    OpcPackagePart oldRelatedPart = oldRelatedParts.GetValueOrNull(rel.Id);
                    string relativePartName = OpcPackageBase.MakeRelative(newPart.Name, absolutePartName);

                    if (oldRelatedPart != null)
                    {
                        // This is the new part.
                        OpcPackagePart newRelatedPart = new OpcPackagePart(absolutePartName, oldRelatedPart.ContentType);
                        newRelatedPart.Stream = oldRelatedPart.Stream;
                        writer.Package.Parts.Add(newRelatedPart);
                    }
                    else
                    {
                        // Looks like relation defined, but the part is missing.
                        relativePartName = "NULL";
                    }

                    newPart.Rels.Add(rel.Id, rel.Type, relativePartName, rel.IsExternal);
                }
            }
        }

        /// <summary>
        /// Returns existing or creates new BiDi control character stack for the specified paragraph.
        /// </summary>
        internal Stack<string> GetOrCreateBiDiControlCharStack(Paragraph paragraph)
        {
            Stack<string> bidiControlChars = mParaToBidiControlChars.GetValueOrNull(paragraph);
            if (bidiControlChars == null)
            {
                bidiControlChars = new Stack<string>();
                mParaToBidiControlChars[paragraph] = bidiControlChars;
            }

            return bidiControlChars;
        }

        /// <summary>
        /// Writes special character with code "DocControlChar.DateShortChar" as date field.
        /// </summary>
        private void WriteDateShortCharAsField(SpecialChar specialChar)
        {
            Debug.Assert((specialChar != null) && (specialChar.GetText() == ControlChar.NonBreakingHyphen));

            RunPr runPr = specialChar.RunPr;

            // 1. Field start.
            FieldType fldType = FieldType.FieldDate;
            VisitFieldStart(new FieldStart(Document, runPr.Clone(), fldType));

            // 2. Field code.
            string datePattern = FormatterPal.GetShortDatePatternCurrent();
            mRunWriter.WriteInline(new Run(Document, "date \\@ \"" + datePattern + "\"", runPr.Clone()));

            // 3. Field separator.
            VisitFieldSeparator(new FieldSeparator(Document, specialChar.RunPr.Clone(), fldType));

            // Skip writing of the field result, because looks like Word update it while opening the document.

            // 4. Field end.
            VisitFieldEnd(new FieldEnd(Document, runPr.Clone(), fldType, true));
        }

        /// <summary>
        /// Writes the displacedByCustomXml attribute of an annotation node.
        /// </summary>
        private void WriteDisplacedByCustomXmlAttribute(IDisplaceableByCustomXml node)
        {
            if (node.DisplacedByCustomXml != DisplacedByType.Unspecified)
            {
                mBuilder.WriteAttribute("w:displacedByCustomXml",
                    DocxDopEnum.DisplacedByTypeToDocx(node.DisplacedByCustomXml));
            }
        }

        /// <summary>
        /// Writes an element for cross-structure annotation node that has only the w:id and w:displacedByCustomXml
        /// attributes. Used for comment ranges and move range ends.
        /// </summary>
        private void WriteAnnotation(Node node, string elementName)
        {
            mBuilder.StartElement(elementName);
            mBuilder.WriteAttribute("w:id", IdToAnnotationId(((INodeWithAnnotationId)node).IdInternal));
            WriteDisplacedByCustomXmlAttribute((IDisplaceableByCustomXml)node);
            mBuilder.EndElement(elementName);
        }

        /// <summary>
        /// Writes the 'w14:stylisticSets' element.
        /// </summary>
        private static void WriteStylisticSets(NrxXmlBuilder builder, StylisticSets stylisticSets)
        {
            Debug.Assert(builder != null);

            if (stylisticSets == StylisticSets.Default)
            {
                builder.WriteRaw(
                    "<w14:stylisticSets xmlns:w14=\"http://schemas.microsoft.com/office/word/2010/wordml\" />");
                return;
            }

            // In this case, we don't need to write the namespace.
            // The namespace in the previous case is written to avoid changing old gold tests.
            builder.StartElement("w14:stylisticSets");
            for (int i = (int)StylisticSets.Set01; i <= (int)StylisticSets.Set20; i <<= 1)
            {
                StylisticSets set = (StylisticSets)i;
                if ((stylisticSets & set) != 0)
                    builder.WriteElementWithAttributes("w14:styleSet", "w14:id", DocxEnum.StylisticSetsToXml(set));
            }
            builder.EndElement("w14:stylisticSets");
        }

        /// <summary>
        /// Builder used for writing the current XML part.
        /// </summary>
        internal DocxBuilder CurrentBuilder
        {
            get { return mBuilder; }
        }

        internal OpcPackagePart DocumentPart
        {
            get { return mDocumentPart; }
            set { mDocumentPart = value; }
        }

        /// <summary>
        /// Indicates that field code is currently written.
        /// </summary>
        internal bool IsInsideFieldCode
        {
            get { return mFieldsWriter.IsInsideFieldCode; }
        }

        bool IVmlShapeWriterContext.IsInsideField
        {
            get { return mFieldsWriter.IsInsideField; }
        }

        internal Section CurrentSection
        {
            get { return mCurrentSection; }
            set { mCurrentSection = value; }
        }

        internal OpcPackageBase Package
        {
            get { return mWriter.Package; }
        }

        internal OoxmlSaveOptions SaveOptions
        {
            get { return (OoxmlSaveOptions)SaveInfo.SaveOptions; }
        }

        internal DocxRelationshipTypes RelTypes
        {
            get { return mWriter.RelTypes; }
        }

        internal DocxNamespaces DocxNamespaces
        {
            get { return mWriter.DocxNamespaces; }
        }

        protected DocxWriter Writer
        {
            get { return mWriter; }
        }

        /// <summary>
        /// Maps last paragraph in comment to paraId.
        /// </summary>
        /// <remarks>
        /// ParaId attribute exists in OOXML formats only.
        /// When source document is DOC for example we need to manually generate paraId for at least last paragraph in comment to be able to
        /// save comment tree structure (comment tree structure is specified using paraId).
        /// </remarks>
        internal ObjToIntDictionary<Paragraph> CommentParagraphToParaId
        {
            get { return mCommentParagraphToParaId; }
        }

        /// <summary>
        /// Returns MS Word version (max), which MS Word Extensions of the document have.
        /// </summary>
        internal MsWordVersionCore MsWordExtensionsVersion
        {
            get
            {
                Document doc = mWriter.Document;
                if (doc.ComplianceInfo != null)
                    return doc.ComplianceInfo.MsWordExtensionsVersion;

                return MsWordVersionCore.Unspecified;
            }
        }

        protected SavingProgressProcessor ProgressProcessor;

        /// <summary>
        /// Counter indicates whether we write FallBack, is used when we write shape to avoid opening redundant Run.
        /// WORDSNET-14513 AlternateContent inside Fallback VML shape have to be placed into the run.
        /// </summary>
        private int mFallbackDeeps;

        /// <summary>
        /// The current top-level DrawingML node. Used to skip nested DrawingML nodes if required.
        /// </summary>
        private ShapeBase mCurrentDmlChart;

        /// <summary>
        /// This provides the global context.
        /// </summary>
        private readonly DocxWriter mWriter;
        private readonly DocumentBase mDoc;

        private readonly NrxStyleIdGenerator mStyleIdGenerator;
        private readonly DocxRunWriter mRunWriter;
        private readonly DocxFieldsWriter mFieldsWriter;

        /// <summary>
        /// Stores case insensitive bookmark name->id pairs.
        /// Bookmark id is assigned when visiting bookmark start and we need to be able to get it later when visiting bookmark end.
        /// </summary>
        private readonly StringToIntDictionary mBookmarkIdsByName = new StringToIntDictionary(false);

        /// <summary>
        /// Additional dictionary for bookmarks located in fallback, used to continue bookmark id enumeration.
        /// </summary>
        private readonly StringToIntDictionary mBookmarkIdsByNameFallback = new StringToIntDictionary(false);

        /// <summary>
        /// The key is integer comment/editable range id (from the model).
        /// The value is integer annotation id that is valid for writing into the output file.
        /// </summary>
        private readonly IntToIntDictionary mIdToAnnotationId = new IntToIntDictionary();

        private readonly Stack<DocxBuilder> mBuilderStack = new Stack<DocxBuilder>();
        private DocxBuilder mBuilder;
        private OpcPackagePart mDocumentPart;
        private DocxCommentsWriter mCommentsWriter;
        private DocxFootnotesWriter mFootnotesWriter;
        private DocxFootnotesWriter mEndnotesWriter;

        private int mAnnotationId;

        private Paragraph mLastParagraphOfCurrentSection;
        private Section mCurrentSection;

        /// <summary>
        /// Header/footer document part names are formed using an incrementing suffix, like 'header1.xml', 'header2.xml', etc.
        /// </summary>
        private int mHeaderNumber;
        private int mFooterNumber;
        private IntToObjDictionary<string> mDiagramReferences;

        private readonly ObjToIntDictionary<Paragraph> mCommentParagraphToParaId = new ObjToIntDictionary<Paragraph>();

        private readonly IWarningCallback mWarningCallback;
        private const string ControlXmlPartContent =
            @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?><ax:ocx ax:classid=""{{{0}}}"" ax:persistence=""persistStorage"" r:id=""rId1"" xmlns:ax=""{1}"" xmlns:r=""{2}""/>";

        /// <summary>
        /// Stack of DML images that were converted from the corresponded VML images.
        /// </summary>
        private readonly Stack<Shape> mDmlsConvertedFromVmls = new Stack<Shape>();

        /// <summary>
        /// Maps paragraph to the stack of BiDi control characters in it.
        /// It is used to mimic Word behavior: adding PDF BiDi control character
        /// for each opened 'w:dir' and 'w:bdo' element to the end of paragraph.
        /// </summary>
        private readonly Dictionary<Paragraph, Stack<string>> mParaToBidiControlChars =
            new Dictionary<Paragraph, Stack<string>>();

        private readonly IntToObjDictionary<StructuredDocumentTagRangeStart> mSdtRangeStartMap =
            new IntToObjDictionary<StructuredDocumentTagRangeStart>();
    }
}
