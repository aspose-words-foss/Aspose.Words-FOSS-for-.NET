// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2009 by Roman Korchagin

using System;
using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Writes table/row properties to DOCX or WordML.
    /// </summary>
    internal static class NrxRowPrWriter
    {
        /// <summary>
        /// Writes tblPr or tblPrEx for a table or style.
        /// </summary>
        /// <param name="attrs">Can be null. If null, writes nothing.</param>
        /// <param name="context"></param>
        /// <param name="isTblPr">True for writing table properties. False for writing row properties.</param>
        /// <param name="isStyle">True when writing properties of a table style; false when writing properties of a table.</param>
        internal static void Write(
            TablePr attrs,
            bool isTblPr,
            bool isStyle,
            INrxWriterContext context)
        {
            if (attrs == null)
                return;

            NrxXmlBuilder builder = context.Builder;

            NrxRowPrAttrContainer attrContainer = new NrxRowPrAttrContainer(GetAfterChanges(attrs), isStyle, context);

            TablePr beforeChanges = GetBeforeChanges(attrs);

            // AM. Here I handle AllowAutoFit difference between model and DOCX default in revision.
            // We surely have to put unified support for such handling somewhere.
            if (beforeChanges != null)
            {
                if ((attrContainer.AllowAutoFit) != null && (bool)attrContainer.AllowAutoFit)
                    beforeChanges.AllowAutoFit = true;
            }

            if (isTblPr)
            {
                WriteTablePr(attrContainer, beforeChanges, builder, isStyle, context, true);
            }
            else
            {
                if (attrContainer.IsDocx)
                {
                    // We are writing row properties.
                    builder.WriteAttribute("w:rsidR", attrContainer.rsidR);
                    builder.WriteAttribute("w:rsidTr", attrContainer.rsidTr);
                }

                WriteTablePrEx(attrContainer, beforeChanges, builder, isStyle, context, true);

                WriteRowPr(attrContainer, beforeChanges, builder, isStyle, context, true);
            }
        }

        /// <summary>
        /// Returns TablePr bag after changes, accepted TablePr bag.
        /// </summary>
        private static TablePr GetAfterChanges(TablePr attrs)
        {
            TablePr afterChanges = attrs.Clone();
            afterChanges.AcceptFormatRevision();

            // AcceptFormatRevision above does not handle the case
            // when an attribute is present in the original version,
            // and is removed in the revised version.
            // This is a straightforward workaround that fixes the problem for TestJira6363.
            // WithBefore present in the revision is removed in the final version.
            ClearMissingFinalAttr(attrs, afterChanges, TableAttr.WidthBefore);
            ClearMissingFinalAttr(attrs, afterChanges, TableAttr.WidthAfter);

            return afterChanges;
        }

        private static void ClearMissingFinalAttr(TablePr source, TablePr dest, int attr)
        {
            if (null == source.RevisedPr.GetDirectAttr(attr, RevisionsView.Final))
                dest.Remove(attr);
        }

        /// <summary>
        /// Returns delta between attributes after accepting changes and attributes before changes.
        /// Used to write revision.
        /// Return null if there is no revision or delta is empty.
        /// </summary>
        private static TablePr GetBeforeChanges(TablePr attrs)
        {
            if (attrs.HasFormatRevision)
            {
                // Calculate delta between accepted revision and formatting before changes.
                // If nothing was changed do not write format revision.
                WordAttrCollection rejectedDelta = attrs.Clone();
                WordAttrCollection accepted = GetAfterChanges(attrs);

                rejectedDelta.RemoveEquals(accepted);

                // We do not need format revision because we already calculated delta.
                rejectedDelta.Remove(RevisionAttr.FormatRevision);

                // Do not write format revision if there is nothing to write.
                if (rejectedDelta.Count > 0)
                {
                    // If there were insert or delete revisions they would be preserved.
                    rejectedDelta.FormatRevision = attrs.FormatRevision;
                    return (TablePr)rejectedDelta;
                }
            }
            else if (attrs.HasDeleteRevision || attrs.HasInsertRevision)
            {
                return attrs;
            }
            return null;
        }

        /// <summary>
        /// Write TablePr attributes for table.
        /// </summary>
        private static void WriteTablePr(
            NrxRowPrAttrContainer attrContainer,
            TablePr beforeChanges,
            NrxXmlBuilder builder,
            bool isStyle,
            INrxWriterContext context,
            bool notWmlAnnotation)
        {
            // We are writing table properties. Start writing tblPr.
            // Do not write  'w:tblPr' for Wml annotations according to spec.
            if (notWmlAnnotation)
                builder.StartElement("w:tblPr");

            WriteTableAttributes(attrContainer, builder, context);

            // Write common attributes for 'w:tblPr'.
            if (attrContainer.TblPrExCount > 0)
                WriteCommonTableAttributes(attrContainer, builder, isStyle);

            WriteRevisions("w:tblPrChange", beforeChanges, builder, isStyle, context, context.IsDocx);

            if (notWmlAnnotation)
                builder.EndElement();
        }

        /// <summary>
        /// Write TablePr attributes for row.
        /// </summary>
        private static void WriteRowPr(
            NrxRowPrAttrContainer attrContainer,
            TablePr beforeChanges,
            NrxXmlBuilder builder,
            bool isStyle,
            INrxWriterContext context,
            bool notWmlAnnotation)
        {
            // Start writing 'w:trPr' if there are attributes in it or if there is Insert or delete revision.
            if ((attrContainer.TrPrCount > 0) || (beforeChanges != null) && (beforeChanges.HasDeleteRevision || beforeChanges.HasInsertRevision))
            {
                bool isIsoStrict = context.Compliance == OoxmlComplianceCore.IsoStrict;

                // Do not write  'w:trPr' for Wml annotations.
                if (notWmlAnnotation)
                    builder.StartElement("w:trPr");

                builder.WriteVal("w:divId", attrContainer.htmlBlockId);

                if (attrContainer.GridBefore != 0)
                    builder.WriteVal("w:gridBefore", FormatterPal.IntToXml(attrContainer.GridBefore));

                if (attrContainer.GridAfter > 0)
                    builder.WriteVal("w:gridAfter", FormatterPal.IntToXml(attrContainer.GridAfter));

                // It appears that MS Word does not write zero values, regardless of the type.
                if (PreferredWidth.HasPositiveValue(attrContainer.WidthBefore))
                    builder.WriteLength("w:wBefore", attrContainer.WidthBefore, isIsoStrict);

                // It appears that MS Word does not write zero values, regardless of the type.
                if (PreferredWidth.HasPositiveValue(attrContainer.WidthAfter))
                    builder.WriteLength("w:wAfter", attrContainer.WidthAfter, isIsoStrict);

                // Note WordML doesn't support this attribute.
                if (attrContainer.Hidden)
                    builder.WriteEmptyElement("w:hidden");

                // andrnosk: WORDSNET-9806 There can be explicitly set "w:cantSplit" attribute with zero value.
                // In this case we have to preserve this values as is, like MS Word does.
                if (attrContainer.AllowBreakAcrossPages != null)
                {
                    if ((bool)attrContainer.AllowBreakAcrossPages)
                        builder.WriteVal("w:cantSplit", 0);
                    else
                        builder.WriteEmptyElement("w:cantSplit");
                }

                builder.WriteElementWithAttributes(
                    "w:trHeight",
                    (attrContainer.IsDocx) ? "w:hRule" : "w:h-rule", attrContainer.wRowHeightRule,
                    "w:val", attrContainer.wRowHeight);

                if (attrContainer.wHeader != null)
                {
                    if ((bool)attrContainer.wHeader)
                        builder.WriteEmptyElement("w:tblHeader");
                    else
                        builder.WriteVal("w:tblHeader", 0);
                }

                builder.WriteLength("w:tblCellSpacing", attrContainer.wCellSpacing, isIsoStrict);
                builder.WriteVal("w:jc", attrContainer.wAlignment);

                WriteRevisions("w:trPrChange", beforeChanges, builder, isStyle, context, context.IsDocx);

                if (beforeChanges != null)
                {
                    // Write row delete revision.
                    if (beforeChanges.HasDeleteRevision)
                    {
                        builder.WriteRevisionStart(beforeChanges.DeleteRevision, context.GetNextAnnotationId());
                        builder.WriteRevisionEnd();
                    }

                    // Write row insert revision.
                    if (beforeChanges.HasInsertRevision)
                    {
                        builder.WriteRevisionStart(beforeChanges.InsertRevision, context.GetNextAnnotationId());
                        builder.WriteRevisionEnd();
                    }
                }

                if (notWmlAnnotation)
                    builder.EndElement();
            }
        }

        /// <summary>
        /// Write TablePr attributes for table-level property exceptions (TablePrEx).
        /// </summary>
        private static void WriteTablePrEx(
            NrxRowPrAttrContainer attrContainer,
            TablePr beforeChanges,
            NrxXmlBuilder builder,
            bool isStyle,
            INrxWriterContext context,
            bool notWmlAnnotation)
        {
            // Start writing 'w:tblPrEx' if there are attributes in it.
            // WORDSNET-14969 Style can not contain "w:tblPrEx" element according to OOXML spec.
            if (isStyle || (attrContainer.TblPrExCount <= 0))
                return;

            // Do not write 'w:tblPrEx' for Wml annotations.
            if (notWmlAnnotation)
                builder.StartElement("w:tblPrEx");

            // Write common attributes for 'w:tblPrEx'
            WriteCommonTableAttributes(attrContainer, builder, isStyle);

            WriteRevisions("w:tblPrExChange", beforeChanges, builder, isStyle,
                context, context.IsDocx);

            if (notWmlAnnotation)
                builder.EndElement();
        }

        /// <summary>
        /// Write revisions 'w:tblPrChange', 'w:tblPrExChange', 'w:trPrChange' for Docx, or 'aml:content' for Wml.
        /// </summary>
        private static void WriteRevisions(string tagName,
            TablePr beforeChanges,
            NrxXmlBuilder builder,
            bool isStyle,
            INrxWriterContext context,
            bool notWmlAnnotation)
        {
            // There can be Delete or Insert revisions that is why we need to check HasFormatRevision before writing.
            if ((beforeChanges != null) && beforeChanges.HasFormatRevision)
            {
                NrxRowPrAttrContainer attrContainer = new NrxRowPrAttrContainer(beforeChanges, isStyle, context);

                switch (tagName)
                {
                    case "w:tblPrChange":
                        if (attrContainer.TblPrExCount > 0)
                        {
                            builder.WriteRevisionStart(beforeChanges.FormatRevision, tagName, context.GetNextAnnotationId());
                            WriteTablePr(attrContainer, null, builder, isStyle, context, notWmlAnnotation);
                            builder.WriteRevisionEnd();
                        }
                        break;
                    case "w:tblPrExChange":
                        if (attrContainer.TblPrExCount > 0)
                        {
                            builder.WriteRevisionStart(beforeChanges.FormatRevision, tagName, context.GetNextAnnotationId());
                            WriteTablePrEx(attrContainer, null, builder, isStyle, context, notWmlAnnotation);
                            builder.WriteRevisionEnd();
                        }
                        break;
                    case "w:trPrChange":
                        // Do not write revision if there are no changed attributes.
                        if (attrContainer.TrPrCount > 0)
                        {
                            builder.WriteRevisionStart(beforeChanges.FormatRevision, tagName, context.GetNextAnnotationId());
                            WriteRowPr(attrContainer, null, builder, isStyle, context, notWmlAnnotation);
                            builder.WriteRevisionEnd();
                        }
                        break;
                    default:
                        throw new ArgumentException("Unexpected tag name.");
                }
            }
        }

        /// <summary>
        /// Write elements and attributes which does not exist inside 'w:tblPrEx'.
        /// </summary>
        private static void WriteTableAttributes(NrxRowPrAttrContainer attrContainer, NrxXmlBuilder builder,
            INrxWriterContext context)
        {
            builder.WriteVal("w:tblStyle", attrContainer.styleId);
            builder.WriteVal("w:tblStyleRowBandSize", attrContainer.styleRowBandSize);
            builder.WriteVal("w:tblStyleColBandSize", attrContainer.styleColBandSize);

            if (attrContainer.IsDocx && (context.Compliance != OoxmlComplianceCore.Ecma376))
            {

                // WORDSNET-22105 Word does not open document with table title and compatibilityMode < 12 (12 is Word2007).
                MsWordVersionCore mswVersion = context.Document.FetchDocumentOrGlossaryMain().CompatibilityOptions.MswVersion;
                bool writeTableCaption = ((mswVersion == MsWordVersionCore.Unspecified) || (mswVersion > MsWordVersionCore.Word2003));

                // we're writing ISO29500 compliant document
                if (writeTableCaption)
                    builder.WriteVal("w:tblCaption", attrContainer.wTblCaption);
                builder.WriteVal("w:tblDescription", attrContainer.wTblDescription);
            }

            builder.WriteElementWithAttributes(
                "w:tblpPr",
                "w:leftFromText", attrContainer.wDistanceFromLeft,
                "w:rightFromText", attrContainer.wDistanceFromRight,
                "w:topFromText", attrContainer.wDistanceFromTop,
                "w:bottomFromText", attrContainer.wDistanceFromBottom,
                "w:vertAnchor", attrContainer.wVertAnchor,
                "w:horzAnchor", attrContainer.wHorzAnchor,
                "w:tblpXSpec", attrContainer.wXSpec,
                "w:tblpX", attrContainer.wX,
                "w:tblpYSpec", attrContainer.wYSpec,
                "w:tblpY", attrContainer.wY);

            if (!attrContainer.wAllowOverlap)
                builder.WriteVal("w:tblOverlap", (attrContainer.IsDocx) ? "never" : "Never");

            if (attrContainer.wBidi)
                builder.WriteEmptyElement("w:bidiVisual");
        }

        /// <summary>
        /// Write common table attributes.
        /// </summary>
        private static void WriteCommonTableAttributes(NrxRowPrAttrContainer attrContainer, NrxXmlBuilder builder, bool isStyle)
        {
            bool isIsoStrict = attrContainer.IsDocx && (attrContainer.Compliance == OoxmlComplianceCore.IsoStrict);

            builder.WriteLength("w:tblW", attrContainer.wPrefferedWidth, isIsoStrict);
            builder.WriteVal("w:jc", attrContainer.wAlignment);
            builder.WriteLength("w:tblCellSpacing", attrContainer.wCellSpacing, isIsoStrict);
            builder.WriteLength("w:tblInd", attrContainer.wLeftIndent, isIsoStrict);

            builder.WriteBorders("w:tblBorders",
                                 "w:top", attrContainer.wBorderTop,
                                 (isIsoStrict) ? "w:start" : "w:left", attrContainer.wBorderLeft,
                                 "w:bottom", attrContainer.wBorderBottom,
                                 (isIsoStrict) ? "w:end" : "w:right", attrContainer.wBorderRight,
                                 "w:insideH", attrContainer.wBorderHorizontal,
                                 "w:insideV", attrContainer.wBorderVertical);

            builder.WriteShd(attrContainer.wShading);

            if (!isStyle && !(bool)attrContainer.AllowAutoFit)
                builder.WriteElementWithAttributes("w:tblLayout", "w:type", (attrContainer.IsDocx) ? "fixed" : "Fixed");

            builder.WriteMargins("w:tblCellMar", attrContainer.marginTop, attrContainer.marginLeft,
                attrContainer.marginBottom, attrContainer.marginRight, isIsoStrict);

            WriteTableStyleLook(attrContainer, builder, isStyle);
        }

        private static void WriteTableStyleLook(NrxRowPrAttrContainer attrContainer, NrxXmlBuilder builder,
            bool isStyle)
        {
            // Table style options default for WML is different than in Model.
            if (!attrContainer.IsDocx && !isStyle)
            {
                if (attrContainer.wStyleLook == null)
                {
                    // In case explicit table style option is missed write model default.
                    builder.WriteVal("w:tblLook",
                        FormatterPal.IntToStrX4(NrxTableUtil.TableStyleOptionsToNrx(TableStyleOptions.Default)));
                }
                else if (attrContainer.wStyleLook !=
                         FormatterPal.IntToStrX4(NrxTableUtil.TableStyleOptionsToNrx(TableStyleOptions.Default2003)))
                {
                    // Word doesn't write format default value. So we write only non-default too.
                    builder.WriteVal("w:tblLook", attrContainer.wStyleLook);
                }
            }
            else
            {
                // DOCX default is the same as Model default so write as usual.

                if (attrContainer.wStyleLook == null)
                    return;

                if ((attrContainer.IsDocx) &&
                    (attrContainer.Compliance == OoxmlComplianceCore.IsoStrict))
                {
                    TableStyleOptions options =
                        NrxTableUtil.NrxToTableStyleOptions(NrxXmlUtil.HexToInt(attrContainer.wStyleLook));
                    WriteIsoStrictTableStyleLook(options, builder);
                }
                else
                {
                    builder.WriteVal("w:tblLook", attrContainer.wStyleLook);
                }
            }
        }

        private static void WriteIsoStrictTableStyleLook(TableStyleOptions options, NrxXmlBuilder builder)
        {
            if (options == TableStyleOptions.None)
                return;

            // WORDSNET-19326 Write turned off values due to default value is turned on.
            builder.StartElement("w:tblLook");
            builder.WriteAttribute("w:firstColumn", (options & TableStyleOptions.FirstColumn) != 0);
            builder.WriteAttribute("w:firstRow", (options & TableStyleOptions.FirstRow) != 0);
            builder.WriteAttribute("w:lastColumn", (options & TableStyleOptions.LastColumn) != 0);
            builder.WriteAttribute("w:lastRow", (options & TableStyleOptions.LastRow) != 0);
            // In the model this is inverted.
            builder.WriteAttribute("w:noHBand", (options & TableStyleOptions.RowBands) == 0);
            // In the model this is inverted.
            builder.WriteAttribute("w:noVBand", (options & TableStyleOptions.ColumnBands) == 0);
            builder.EndElement();
        }
    }
}
