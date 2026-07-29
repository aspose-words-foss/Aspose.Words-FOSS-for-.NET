// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using Aspose.Words.Revisions;
using Aspose.Words.Saving;
using Aspose.Words.Styles;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Writes cell properties to DOCX or WordML.
    /// This class is static.
    /// </summary>
    internal static class NrxCellPrWriter
    {
        /// <summary>
        /// Writes cell properties for DOCX or WML.
        /// </summary>
        /// <param name="cellPr">Can be null. If null, writes nothing.</param>
        /// <param name="context"></param>
        internal static void Write(
            CellPr cellPr, 
            INrxWriterContext context)
        {
            if (cellPr == null)
                return;

            if (cellPr.Count == 0)
                return;

            WordAttrCollection attrs = cellPr;
            NrxXmlBuilder builder = context.Builder;

            builder.StartElement("w:tcPr");

            if (attrs.FormatRevision != null)
            {
                // Calculate and write the AfterChanges attribute collection.
                WordAttrCollection afterChanges = attrs.Clone();
                afterChanges.AcceptFormatRevision();
                WriteProps(afterChanges, context);

                // IN. It seems that MS Word writes edit revision for cells twice.
                // One time in cellPr and second time in revised cellPr.
                EditRevision editRevision = (attrs.DeleteRevision != null) ? attrs.DeleteRevision : attrs.InsertRevision;
                if (editRevision != null)
                    builder.WriteCellRevision(editRevision, context.GetNextAnnotationId());

                // Write the revision
                builder.WriteRevisionStart(attrs.FormatRevision, "w:tcPrChange", context.GetNextAnnotationId());

                // Write the BeforeChanges attribute collection.
                if (context.IsDocx)
                    builder.StartElement("w:tcPr");

                WriteProps(attrs, context);

                if (editRevision != null)
                    builder.WriteCellRevision(editRevision, context.GetNextAnnotationId());

                if (context.IsDocx)
                    builder.EndElement();   // w:tcPr

                builder.WriteRevisionEnd();
            }
            else
            {
                EditRevision editRevision = (attrs.DeleteRevision != null) ? attrs.DeleteRevision : attrs.InsertRevision;
                if (editRevision != null)
                    builder.WriteCellRevision(editRevision, context.GetNextAnnotationId());

                WriteProps(attrs, context);
            }

            builder.EndElement();
        }

        private static void WriteProps(
            AttrCollection attrs, 
            INrxWriterContext context)
        {
            bool isDocx = context.IsDocx;
            bool isIsoStrict = isDocx && (context.Compliance == OoxmlComplianceCore.IsoStrict);
            NrxXmlBuilder builder = context.Builder;

            PreferredWidth preferredWidth = null;

            string mHMerge = null;
            string mVMerge = null;

            Border bdrTop = null;
            Border bdrLeft = null;
            Border bdrBottom = null;
            Border bdrRight = null;
            Border bdrTlbr = null;
            Border bdrTrbl = null;
            Border bdrInsideH = null;
            Border bdrInsideV = null;

            Shading mShading = null;
            object mNoWrap = null;

            PreferredWidth marginTop = null;
            PreferredWidth marginLeft = null;
            PreferredWidth marginBottom = null;
            PreferredWidth marginRight = null;

            string textDirection = null;
            object fitText = null;
            string vAlign = null;
            object hideMark = null;

            object gridSpan = null;

            // This is the main loop to collect the properties.
            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                switch (key)
                {
                    case CellAttr.PreferredWidth:
                        preferredWidth = (PreferredWidth)value;
                        break;
                    case CellAttr.HorizontalMerge:
                        mHMerge = NrxTableEnum.CellMergeToXml((CellMerge)value);
                        break;
                    case CellAttr.VerticalMerge:
                        mVMerge = NrxTableEnum.CellMergeToXml((CellMerge)value);
                        break;
                    case CellAttr.BorderTop:
                        bdrTop = (Border)value;
                        break;
                    case CellAttr.BorderLeft:
                        bdrLeft = (Border)value;
                        break;
                    case CellAttr.BorderBottom:
                        bdrBottom = (Border)value;
                        break;
                    case CellAttr.BorderRight:
                        bdrRight = (Border)value;
                        break;
                    case CellAttr.BorderDiagonalDown:
                        bdrTlbr = (Border)value;
                        break;
                    case CellAttr.BorderDiagonalUp:
                        bdrTrbl = (Border)value;
                        break;
                    case CellAttr.BorderHorizontal:
                        bdrInsideH = (Border)value;
                        break;
                    case CellAttr.BorderVertical:
                        bdrInsideV = (Border)value;
                        break;
                    case CellAttr.Shading:
                        mShading = (Shading)value;
                        break;
                    case CellAttr.TopPadding:
                        marginTop = PreferredWidth.FromTwipsSafe((int)value);
                        break;
                    case CellAttr.LeftPadding:
                        marginLeft = PreferredWidth.FromTwipsSafe((int)value);
                        break;
                    case CellAttr.BottomPadding:
                        marginBottom = PreferredWidth.FromTwipsSafe((int)value);
                        break;
                    case CellAttr.RightPadding:
                        marginRight = PreferredWidth.FromTwipsSafe((int)value);
                        break;
                    case CellAttr.WrapText:
                        mNoWrap = !(bool)value;
                        break;
                    case CellAttr.HideMark:
                        if ((bool)value)
                            hideMark = true;
                         break;
                    case CellAttr.FitText:
                        fitText = (bool)value;
                        break;
                    case CellAttr.Orientation:
                        textDirection = StyleConvertUtil.TextOrientationToXml((TextOrientation)value, isDocx, 
                            context.Compliance);
                        break;
                    case CellAttr.VerticalAlignment:
                        vAlign = NrxTableEnum.CellVerticalAlignmentToXml((CellVerticalAlignment)value);
                        break;
                    case RevisionAttr.FormatRevision:
                        // RK It looks like we are not writing table revisions yet?
                        break;
                    case CellAttr.Sys_CellSpan:
                        gridSpan = (int) value;
                        break;
                    default:
                        break;
                }
            }

            // RK We do not write w:cnfStyle. It is a calculated attribute to help consumers.

            builder.WriteLength("w:tcW", preferredWidth, isIsoStrict);

            // RK We do not output both gridSpan and hmerge on the same cell because is is wrong.
            // andrnosk: WORDSNET-4633 Do not output gridspan if hmerge is restart. 
            // Such situation occurs when read ODT with horizontally merged cells. 
            if ((mHMerge != "restart") && (gridSpan != null) && ((int)gridSpan > 1))
                builder.WriteVal("w:gridSpan", gridSpan);

            string hMergeElem = isDocx ? "w:hMerge" : "w:hmerge";
            if (mHMerge == "continue")
                builder.WriteEmptyElement(hMergeElem);
            else
                builder.WriteVal(hMergeElem, mHMerge);

            string vMergeElem = isDocx ? "w:vMerge" : "w:vmerge";
            if (mVMerge == "continue")
                builder.WriteEmptyElement(vMergeElem);
            else
                builder.WriteVal(vMergeElem, mVMerge);

            builder.WriteBorders("w:tcBorders",
                "w:top", bdrTop,
                (isIsoStrict) ? "w:start" : "w:left", bdrLeft,
                "w:bottom", bdrBottom,
                (isIsoStrict) ? "w:end" : "w:right", bdrRight,
                "w:insideH", bdrInsideH,
                "w:insideV", bdrInsideV,
                "w:tl2br", bdrTlbr,
                "w:tr2bl", bdrTrbl);

            builder.WriteShd(mShading);

            builder.WriteVal("w:noWrap", mNoWrap);
            builder.WriteMargins("w:tcMar", marginTop, marginLeft, marginBottom, marginRight, isIsoStrict);
            builder.WriteVal(isDocx ? "w:textDirection" : "w:textFlow", textDirection);
            builder.WriteVal("w:tcFitText", fitText);
            builder.WriteVal("w:vAlign", vAlign);
            builder.WriteVal("w:hideMark", hideMark);
        }
    }
}
