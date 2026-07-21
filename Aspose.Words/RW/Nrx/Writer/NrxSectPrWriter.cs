// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/03/2011 by Roman Korchagin

using System;
using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Sections;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Writes section properties to DOCX and WML.
    /// This class is static.
    /// </summary>
    internal class NrxSectPrWriter
    {
        /// <summary>
        /// No ctor.
        /// </summary>
        private NrxSectPrWriter()
        {
        }

        internal static void Write(Section section, INrxWriterContext writer)
        {
            SectPr sectPr = section.SectPr;
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement("w:sectPr");

            if (writer.IsDocx)
                builder.WriteAttribute("w:rsidSect", NrxXmlUtil.IntToHex(sectPr[SectAttr.Rsid]));

            if (sectPr.FormatRevision != null)
            {
                // Write header/footer references.
                WriteHeadersFooters(section, writer);

                // Write the AfterChanges attribute collection.
                SectPr afterChanges = sectPr.Clone();
                afterChanges.AcceptFormatRevision();
                WriteProps(afterChanges, false, writer);

                // Write BeforeChanges attribute collection.
                builder.WriteRevisionStart(sectPr.FormatRevision, "w:sectPrChange", writer.GetNextAnnotationId());

                builder.StartElement("w:sectPr");
                if (writer.IsDocx)
                    builder.WriteAttribute("w:rsidSect", NrxXmlUtil.IntToHex(sectPr[SectAttr.Rsid]));

                // WORDSNET-25040 Check if we need to additional handling for certain revised attributes.
                SectPr deltaSectPr = CalculateBeforeChangesDelta(sectPr, afterChanges);
                if (deltaSectPr.Count > 0)
                {
                    sectPr = sectPr.Clone();
                    deltaSectPr.ExpandTo(sectPr);
                }

                sectPr = ColSpacingHack(sectPr, afterChanges);

                // Save BeforeChanges attribute collection.
                WriteProps(sectPr, true, writer);

                builder.EndElement();   // w:sectPr

                builder.WriteRevisionEnd();
            }
            else
            {
                WriteHeadersFooters(section, writer);
                WriteProps(sectPr, false, writer);
            }

            builder.EndElement("w:sectPr");
        }

        /// <summary>
        /// Removes column spacing if equal column spacing values are in both original and after changes.
        /// </summary>
        /// <remarks>
        /// AM. We used to always write default colSpacing value for Nrx format and this causes roundtrip error, see
        /// UnifiedTestDefect713 for example. Removing default value writing caused hundreds of gold failed.
        /// </remarks>
        private static SectPr ColSpacingHack(SectPr sectPr, SectPr afterChanges)
        {
            const int key = SectAttr.ColumnsSpacing;
            if (sectPr.ContainsKey(key) && afterChanges.ContainsKey(key) && sectPr[key] == afterChanges[key])
            {
                sectPr = sectPr.Clone();
                sectPr.Remove(key);
            }

            return sectPr;
        }

        /// <summary>
        /// Calculates attribute collection delta to handle reading trick.
        /// </summary>
        /// <remarks>
        /// This methods calculates "compensation" delta to properly handle section revision reading logic
        /// found per WORDSNET-25040
        ///
        /// Actually this methods acts very similar to beforeChanges.Collapse(afterChanges) performed for
        /// certain attributes only.
        /// </remarks>
        private static SectPr CalculateBeforeChangesDelta(SectPr before, SectPr after)
        {
            SectPr newSectPr = new SectPr();
            int[] keys = new []
                {
                    SectAttr.SectionStart,
                    SectAttr.Orientation,
                    SectAttr.PageWidth,
                    SectAttr.PageHeight,
                    SectAttr.ColumnsSpacing
                };

            foreach (int key in keys)
            {
                object beforeValue = before.FetchAttr(key);
                object afterValue = after.FetchAttr(key);

                // Little trick, at this moment newSectPr contains default value for given key.
                object defaultValue = newSectPr.FetchAttr(key);

                if ((beforeValue != afterValue) && (afterValue != defaultValue) && (beforeValue == defaultValue))
                    newSectPr.SetAttr(key, defaultValue);
            }

            return newSectPr;
        }

        private static void WriteProps(AttrCollection attrs, bool isChange, INrxWriterContext writer)
        {
            // Attributes with null values are not written by the Builder.WriteXXX methods. Null check is performed in the builder.
            NrxXmlBuilder builder = writer.Builder;
            bool isDocx = writer.IsDocx;

            string pageWidth = null;
            string pageHeight = null;
            string orientation = null;
            string paperCode = null;

            string marginTop = null;
            string marginRight = null;
            string marginBottom = null;
            string marginLeft = null;
            string marginHeader = null;
            string marginFooter = null;
            string marginGutter = null;

            string trayFirst = null;
            string trayOther = null;

            string bordersZOrder = null;
            string bordersDisplay = null;
            string bordersOffset = null;
            Border borderTop = null;
            Border borderLeft = null;
            Border borderBottom = null;
            Border borderRight = null;

            string lineNumberCountBy = null;
            string lineNumberStart = null;
            string lineNumberDistance = null;
            string lineNumberRestart = null;

            string pageNumberFormat = null;
            string pageNumberStart = null;
            string chapStyle = null;
            string chapSeparator = null;

            object colEqualWidth = null;
            string colSpace = null;
            string colNum = null;
            object colSep = null;
            TextColumnCollectionInternal columns = null;

            string gridType = null;
            string linePitch = null;
            string charSpace = null;

            string sectionStart = null;
            string vertAlign = null;

            object bidi = null;
            object rtlGutter = null;
            object titlePg = null;
            object formProt = null;
            object noEndnote = null;
            object textDirection = null;
            object footnoteColumns = null;

            // This is the main loop to collect the properties.
            for (int k = 0; k < attrs.Count; k++)
            {
                int key = attrs.GetKey(k);
                object value = attrs.GetByIndex(k);

                switch (key)
                {
                    case SectAttr.Bidi:
                        bidi = value;
                        break;
                    case SectAttr.RtlGutter:
                        rtlGutter = value;
                        break;
                    case SectAttr.DifferentFirstPageHeaderFooter:
                        titlePg = value;
                        break;
                    case SectAttr.Unlocked:
                        // WORDSNET-20831 Value that should be written to DOCX should be !SectAttr.Unlocked.
                        formProt = !((bool)value);
                        break;
                    case SectAttr.SuppressEndnotes:
                        noEndnote = value;
                        break;
                    case SectAttr.SectionStart:
                        sectionStart = NrxSectEnum.SectionStartToXml((SectionStart)value, isDocx);
                        break;
                    case SectAttr.VerticalAlignment:
                        vertAlign = NrxSectEnum.PageVerticalAlignmentToXml((PageVerticalAlignment)value);
                        break;
                    case SectAttr.TextFlow:
                        textDirection =
                            StyleConvertUtil.TextOrientationToXml(TextFlowOrientationConverter.FromTextFlow((TextFlow)value),
                                isDocx, writer.Compliance);
                        break;
                    // docGrid attributes
                    case SectAttr.GridType:
                        gridType = NrxSectEnum.DocGridTypeToXml((SectionLayoutMode)value, isDocx);
                        break;
                    case SectAttr.LinePitch:
                        linePitch = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.CharSpace:
                        charSpace = FormatterPal.IntToXml((int)value);
                        break;

                    // pgSz attributes
                    case SectAttr.PageWidth:
                        pageWidth = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.PageHeight:
                        pageHeight = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.Orientation:
                        orientation = NrxSectEnum.OrientationToXml((Orientation)value);
                        break;
                    case SectAttr.PaperCode:
                        paperCode = FormatterPal.IntToXml((int)value);
                        break;

                    // pgMar attributes
                    case SectAttr.TopMargin:
                        marginTop = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.RightMargin:
                        marginRight = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.BottomMargin:
                        marginBottom = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.LeftMargin:
                        marginLeft = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.HeaderDistance:
                        // WORDSNET-24518 Write distance as "ushort" value.
                        marginHeader = FormatterPal.IntToXml((ushort)MathUtil.CastIntToShort((int)value));
                        break;
                    case SectAttr.FooterDistance:
                        marginFooter = FormatterPal.IntToXml((ushort)MathUtil.CastIntToShort((int)value));
                        break;
                    case SectAttr.Gutter:
                        marginGutter = FormatterPal.IntToXml((int)value);
                        break;

                    // paperSrc attributes
                    case SectAttr.FirstPageTray:
                        // Written as int number in WordML.
                        trayFirst = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.OtherPagesTray:
                        // Written as int number in WordML.
                        trayOther = FormatterPal.IntToXml((int)value);
                        break;

                    // pgBorders attributes
                    case SectAttr.BorderAlwaysInFront:
                        if ((bool)value)
                            bordersZOrder = "front";
                        else
                            bordersZOrder = "back";
                        break;
                    case SectAttr.BorderAppliesTo:
                        bordersDisplay = NrxSectEnum.PageBorderAppliesToToXml((PageBorderAppliesTo)value, isDocx);
                        break;
                    case SectAttr.BorderDistanceFrom:
                        bordersOffset = NrxSectEnum.PageBorderDistanceFromToXml((PageBorderDistanceFrom)value);
                        break;
                    case SectAttr.BorderTop:
                        borderTop = (Border)value;
                        break;
                    case SectAttr.BorderLeft:
                        borderLeft = (Border)value;
                        break;
                    case SectAttr.BorderBottom:
                        borderBottom = (Border)value;
                        break;
                    case SectAttr.BorderRight:
                        borderRight = (Border)value;
                        break;

                    // lnNumType attributes
                    case SectAttr.LineNumberCountBy:
                        lineNumberCountBy = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.LineStartingNumber:
                        lineNumberStart = FormatterPal.IntToXml((int)value - 1);
                        break;
                    case SectAttr.LineNumberDistanceFromText:
                        lineNumberDistance = FormatterPal.IntToXml(Convert.ToInt32(value));
                        break;
                    case SectAttr.LineNumberRestartMode:
                        lineNumberRestart = NrxSectEnum.LineNumberRestartModeToXml((LineNumberRestartMode)value, isDocx);
                        break;

                    // pgNumType attributes
                    case SectAttr.PageNumberStyle:
                        pageNumberFormat = (isDocx) ?
                            DocxEnum.NumberStyleToDocx((NumberStyle)value) :
                            WmlEnum.NumberStyleToWml((NumberStyle)value);
                        break;
                    case SectAttr.PageStartingNumber:
                    {
                        bool isRestart = (bool)attrs.FetchAttr(SectAttr.RestartPageNumbering);
                        if (isRestart)
                            pageNumberStart = FormatterPal.IntToXml((int)value);
                        break;
                    }
                    case SectAttr.RestartPageNumbering:
                    {
                        const int defaultRestartPageNumber = 1;
                        if ((pageNumberStart == null) && (bool)value)
                            pageNumberStart = FormatterPal.IntToXml(defaultRestartPageNumber);
                        break;
                    }
                    case SectAttr.HeadingLevelForChapter:
                        chapStyle = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.ChapterPageSeparator:
                        chapSeparator = NrxSectEnum.ChapterPageSeparatorToXml((ChapterPageSeparator)value, isDocx);
                        break;

                    // cols attributes
                    case SectAttr.ColumnsEvenlySpaced:
                        colEqualWidth = value;
                        break;
                    case SectAttr.ColumnsSpacing:
                        colSpace = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.ColumnsCount:
                        colNum = FormatterPal.IntToXml((int)value);
                        break;
                    case SectAttr.ColumnsLineBetween:
                        colSep = value;
                        break;
                    case SectAttr.Columns:
                        columns = (TextColumnCollectionInternal)value;
                        break;
                    case SectAttr.FootnoteColumns:
                        footnoteColumns = (int)value;
                        break;
                    default:
                        // Do nothing. Tests show enough for me.
                        break;
                }
            }

            if (!isChange)
            {
                // Seems that MS Word writes default values in WordML even if they are not set explicitly in DOC.
                // But only in the main attribute set. Change should not contain them.
                if (marginHeader == null)
                    marginHeader = FormatterPal.IntToXml((int)SectPr.FetchDefaultAttr(SectAttr.HeaderDistance));

                if (marginFooter == null)
                    marginFooter = FormatterPal.IntToXml((int)SectPr.FetchDefaultAttr(SectAttr.FooterDistance));

                if (colSpace == null)
                    colSpace = FormatterPal.IntToXml((int)SectPr.FetchDefaultAttr(SectAttr.ColumnsSpacing));
            }

            writer.WriteFootnotePr(attrs, false);

            builder.WriteVal("w:type", sectionStart);

            builder.WriteElementWithAttributes(
                "w:pgSz",
                "w:w", pageWidth,
                "w:h", pageHeight,
                "w:orient", orientation,
                "w:code", paperCode);

            builder.WriteElementWithAttributes(
                "w:pgMar",
                "w:top", marginTop,
                "w:right", marginRight,
                "w:bottom", marginBottom,
                "w:left", marginLeft,
                "w:header", marginHeader,
                "w:footer", marginFooter,
                "w:gutter", marginGutter);

            builder.WriteElementWithAttributes(
                "w:paperSrc",
                "w:first", trayFirst,
                "w:other", trayOther);

            if (borderTop != null || borderLeft != null || borderBottom != null || borderRight != null)
            {
                builder.StartElement("w:pgBorders");
                builder.WriteAttribute(isDocx ? "w:zOrder" : "w:z-order", bordersZOrder);
                builder.WriteAttribute("w:display", bordersDisplay);
                builder.WriteAttribute(isDocx ? "w:offsetFrom" : "w:offset-from", bordersOffset);
                builder.WriteBorder("w:top", borderTop);
                builder.WriteBorder("w:left", borderLeft);
                builder.WriteBorder("w:bottom", borderBottom);
                builder.WriteBorder("w:right", borderRight);
                builder.EndElement(); //w:pgBorders
            }

            builder.WriteElementWithAttributes(
                "w:lnNumType",
                isDocx ? "w:countBy" : "w:count-by", lineNumberCountBy,
                "w:start", lineNumberStart,
                "w:distance", lineNumberDistance,
                "w:restart", lineNumberRestart);

            builder.WriteElementWithAttributes(
                "w:pgNumType",
                "w:fmt", pageNumberFormat,
                "w:start", pageNumberStart,
                isDocx ? "w:chapStyle" : "w:chap-style", chapStyle,
                isDocx ? "w:chapSep" : "w:chap-sep", chapSeparator);

            // TODO 6139. See "[MS-OI29500] 2.1.212 Part 1 Section 17.6.4, cols (Column Definitions)".
            // Word restricts the value of the num attribute to a value between 1 and 45.
            if (builder.WriteElementWithAttributesStart(
                "w:cols",
                "w:num", colNum,
                "w:sep", colSep,
                "w:space", colSpace,
                // WORDSNET-28007 Let's do not write the 'equalWidth' attribute if it contains the default value ('true').
                "w:equalWidth", ((colEqualWidth is bool) && (bool)colEqualWidth) ? null : colEqualWidth))
            {
                if ((columns != null) && columns.HasData)
                {
                    for (int i = 0; i < columns.Count; i++)
                    {
                        builder.WriteElementWithAttributes(
                            "w:col",
                            "w:w", (columns[i].RawWidth > 0) ? (object)columns[i].RawWidth : null,
                            // WORDSNET-24120 Google Docs cannot open a document in the edit mode, if the space attribute
                            // is not defined when cols/equalWidth is false. Let's always write the attribute.
                            // ISO/IEC 29500 doesn't specify a default value of the attribute; 0 is the default value
                            // in MS Word ([MS-OI29500]).
                            "w:space", (object)columns[i].RawSpaceAfter);
                    }
                }

                builder.EndElement(); //w:cols
            }

            builder.WriteVal("w:formProt", formProt);

            builder.WriteVal("w:vAlign", vertAlign);

            builder.WriteVal("w:noEndnote", noEndnote);

            builder.WriteVal("w:titlePg", titlePg);

            builder.WriteVal(isDocx ? "w:textDirection" : "w:textFlow", textDirection);

            builder.WriteVal("w:bidi", bidi);

            builder.WriteVal("w:rtlGutter", rtlGutter);

            builder.WriteElementWithAttributes(
                "w:docGrid",
                "w:type", gridType,
                isDocx ? "w:linePitch" : "w:line-pitch", linePitch,
                isDocx ? "w:charSpace" : "w:char-space", charSpace);

            if (isDocx && writer.Compliance != OoxmlComplianceCore.Ecma376)
                builder.WriteElementWithAttributes("w15:footnoteColumns", "w:val", footnoteColumns);
        }

        /// <summary>
        /// Write all headers/footers defined for the specified section in the correct order.
        /// </summary>
        private static void WriteHeadersFooters(Section section, INrxWriterContext writer)
        {
            WriteHeaderFooter(section, HeaderFooterType.HeaderEven, writer);
            WriteHeaderFooter(section, HeaderFooterType.HeaderPrimary, writer);
            WriteHeaderFooter(section, HeaderFooterType.FooterEven, writer);
            WriteHeaderFooter(section, HeaderFooterType.FooterPrimary, writer);
            WriteHeaderFooter(section, HeaderFooterType.HeaderFirst, writer);
            WriteHeaderFooter(section, HeaderFooterType.FooterFirst, writer);
        }

        private static void WriteHeaderFooter(Section section, HeaderFooterType headerFooterType, INrxWriterContext writer)
        {
            HeaderFooter headerFooter = section.HeadersFooters[headerFooterType];
            if ((headerFooter != null) && (!headerFooter.IsLinkedToPrevious))
                writer.WriteHeaderFooter(headerFooter);
        }
    }
}
