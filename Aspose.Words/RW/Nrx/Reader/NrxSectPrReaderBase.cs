// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2011 by Alexey Noskov

using Aspose.JavaAttributes;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.Sections;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Reader
{
    internal abstract class NrxSectPrReaderBase
    {
        /// <summary>
        /// Reads 'w:sectPr' element from the specified reader.
        /// Reader should be positioned to element start.
        /// </summary>
        /// <param name="reader">DocxReader to read from. Should be positioned to element start.</param>
        /// <param name="sectPr">Section where to read properties to.</param>
        internal void Read(NrxDocumentReaderBase reader, SectPr sectPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            // andrnosk: The document has several body/sectPr in the middle of the document.
            // MS Word takes HeadersFooters from the first sectPr and the other properties from the last sectPr.
            // That is why we need to preserve HeadersFooters from the first read sectPr.
            // IN. There can be SectPrs without any properties, but having HeaderFooter
            // references (see TestJira8816 for example). We should consider it too.
            // WORDSNET-27383, WORDSNET-27273 If a sectPr element is a direct child of a body, MS Word ignores
            // headers/footers of subsequent sectPr elements.
            bool hasBlockLevelSectPr = reader.IsBlockLevelGlobalSectPr;
            Section curSection = (Section)reader.CurContainer.GetAncestor(NodeType.Section);
            bool hasHeadersFooters = (curSection.HeadersFooters.Count > 0);

            // If we read DOCX document we need to read section id.
            if (reader.IsDocx)
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    switch (xmlReader.LocalName)
                    {
                        case "rsidSect":
                        {
                            int rsid = NrxXmlUtil.TryHexToInt(xmlReader.Value);
                            if (rsid != int.MinValue)
                                sectPr.SetAttr(SectAttr.Rsid, rsid);
                            break;
                        }
                        default:
                            // Do nothing.
                            break;
                    }
                }
            }

            while (xmlReader.ReadChild("sectPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "type":
                        sectPr.SetAttr(SectAttr.SectionStart, NrxSectEnum.XmlToSectionStart(xmlReader.ReadVal()));
                        break;
                    case "pgSz":
                        ReadPageSize(xmlReader, sectPr, complianceInfo);
                        break;
                    case "pgMar":
                        ReadPageMargins(xmlReader, sectPr, complianceInfo);
                        break;
                    case "paperSrc":
                        ReadPaperSource(xmlReader, sectPr);
                        break;
                    case "pgBorders":
                        ReadBorders(xmlReader, sectPr);
                        break;
                    case "lnNumType":
                        ReadLineNumberType(xmlReader, sectPr, complianceInfo);
                        break;
                    case "pgNumType":
                        ReadPageNumberType(reader, sectPr);
                        break;
                    case "cols":
                        ReadCols(xmlReader, sectPr, complianceInfo);
                        break;
                    case "formProt":
                        // WORDSNET-20831 SectAttr.Unlocked is true when the section is not protected with forms,
                        // but formProt = 0 (false) when the section is not protected (see 2.6.6 in OOXML spec). 
                        // So the correct value of SectAttr.Unlocked should be !formProt.
                        sectPr.SetAttr(SectAttr.Unlocked, !xmlReader.ReadBoolVal());
                        break;
                    case "vAlign":
                        sectPr.SetAttr(SectAttr.VerticalAlignment,
                            NrxSectEnum.XmlToPageVerticalAlignment(xmlReader.ReadVal()));
                        break;
                    case "noEndnote":
                        sectPr.SetAttr(SectAttr.SuppressEndnotes, xmlReader.ReadBoolVal());
                        break;
                    case "titlePg":
                        sectPr.SetAttr(SectAttr.DifferentFirstPageHeaderFooter, xmlReader.ReadBoolVal());
                        break;
                    case "textDirection": // DOCX
                    {
                        TextOrientation orientation = StyleConvertUtil.XmlToTextOrientation(xmlReader.ReadVal(),
                            complianceInfo);
                        sectPr.SetAttr(SectAttr.TextFlow, TextFlowOrientationConverter.FromTextOrientation(orientation));
                        break;
                    }
                    case "textFlow": // WML
                    {
                        TextOrientation orientation = StyleConvertUtil.XmlToTextOrientation(xmlReader.ReadVal(),
                            complianceInfo);
                        sectPr.SetAttr(SectAttr.TextFlow, TextFlowOrientationConverter.FromTextOrientation(orientation));
                        break;
                    }
                    case "bidi":
                        sectPr.SetAttr(SectAttr.Bidi, xmlReader.ReadBoolVal());
                        break;
                    case "rtlGutter":
                        sectPr.SetAttr(SectAttr.RtlGutter, xmlReader.ReadBoolVal());
                        break;
                    case "docGrid":
                        ReadDocGrid(xmlReader, sectPr);
                        break;
                    case "br":
                        xmlReader.RunTextBuilder.Append(NrxRunReaderBase.ReadBreak(xmlReader, new RunPr()));
                        break;
                    case "footnoteColumns":
                    {
                        int columnCount = xmlReader.ReadIntVal();
                        if (columnCount > 0)
                        {
                            sectPr.SetAttr(SectAttr.FootnoteColumns, columnCount);
                            complianceInfo.IsDocxExtensions = true;
                        }
                        break;
                    }
                    case "p":
                    {
                        // WORDSNET-16627 Resilency. Paragraph is inside w:sectPr.
                        ResilentParaRead(reader);
                        break;
                    }
                    case "tbl":
                    {
                        // WORDSNET-16627 Resilency. Table is inside w:sectPr.
                        DocxTableReader.Read(reader);
                        break;
                    }
                    // WORDSNET-19577 Added reading MultiplePagesType.
                    case "bookFoldPrinting":
                    case "bookFoldRevPrinting":
                    case "mirrorMargins":
                    case "printTwoOnOne":
                        NrxSettingsReader.ReadMultiplePagesType(reader, reader.Document.DocPr);
                        break;
                    default:
                        ReadFormatSpecific(xmlReader.LocalName, reader, sectPr, hasBlockLevelSectPr, hasHeadersFooters);
                        break;
                }
            }
        }

        /// <summary>
        /// Resiliently reads paragraph.
        /// </summary>
        /// <param name="reader">
        /// DocxReader to read from.
        /// </param>
        private static void ResilentParaRead(NrxDocumentReaderBase reader)
        {
            DocxHyperlinkReader hyperlinkReader = new DocxHyperlinkReader();

            DocxSmartTagReader smartTagReader = new DocxSmartTagReader();

            DocxInlineReader inlineReader = new DocxInlineReader(hyperlinkReader, smartTagReader);
            hyperlinkReader.SetInlineReader(inlineReader);
            
            DocxParaReader paraReader =
                new DocxParaReader(
                    DocxReaderFactory.ParaPrReader,
                    inlineReader);
            paraReader.Read(reader);
        }

        [JavaThrows(true)]
        protected abstract void ReadFormatSpecific(string localName, 
            NrxDocumentReaderBase reader, SectPr sectPr,
            bool hasSectPr, bool hasHeadersFooters);

        private static void ReadPageSize(NrxXmlReader xmlReader, SectPr sectPr, OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "w":
                        sectPr.SetAttr(SectAttr.PageWidth, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "h":
                        sectPr.SetAttr(SectAttr.PageHeight, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "orient":
                        sectPr.SetAttr(SectAttr.Orientation, NrxSectEnum.XmlToOrientation(xmlReader.Value));
                        break;
                    case "code":
                        sectPr.SetAttr(SectAttr.PaperCode, xmlReader.ValueAsInt);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadPageMargins(NrxXmlReader xmlReader, SectPr sectPr, OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        sectPr.SetAttr(SectAttr.TopMargin, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "right":
                        sectPr.SetAttr(SectAttr.RightMargin, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "bottom":
                        sectPr.SetAttr(SectAttr.BottomMargin, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "left":
                        sectPr.SetAttr(SectAttr.LeftMargin, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "header":
                        sectPr.SetAttr(SectAttr.HeaderDistance, MathUtil.CastIntToShort(xmlReader.GetValueAsTwips(complianceInfo)));
                        break;
                    case "footer":
                        sectPr.SetAttr(SectAttr.FooterDistance, MathUtil.CastIntToShort(xmlReader.GetValueAsTwips(complianceInfo)));
                        break;
                    case "gutter":
                        sectPr.SetAttr(SectAttr.Gutter, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadPaperSource(NrxXmlReader xmlReader, SectPr sectPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "first":
                        sectPr.SetAttr(SectAttr.FirstPageTray, xmlReader.ValueAsInt);
                        break;
                    case "other":
                        sectPr.SetAttr(SectAttr.OtherPagesTray, xmlReader.ValueAsInt);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadBorders(NrxXmlReader xmlReader, SectPr sectPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "zOrder": // DOCX
                    case "z-order": // WML
                    {
                        switch (xmlReader.Value)
                        {
                            case "front":
                                sectPr.SetAttr(SectAttr.BorderAlwaysInFront, true);
                                break;
                            case "back":
                                sectPr.SetAttr(SectAttr.BorderAlwaysInFront, false);
                                break;
                            default:
                                xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.Value);
                                break;
                        }
                        break;
                    }
                    case "display":
                        sectPr.SetAttr(SectAttr.BorderAppliesTo, NrxSectEnum.XmlToPageBorderAppliesTo(xmlReader.Value));
                        break;
                    case "offsetFrom": // DOCX
                    case "offset-from": // WML
                        sectPr.SetAttr(SectAttr.BorderDistanceFrom, NrxSectEnum.XmlToPageBorderDistanceFrom(xmlReader.Value));
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }

            while (xmlReader.ReadChild("pgBorders"))
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        sectPr.SetAttr(SectAttr.BorderTop, xmlReader.ReadBorder());
                        break;
                    case "left":
                        sectPr.SetAttr(SectAttr.BorderLeft, xmlReader.ReadBorder());
                        break;
                    case "bottom":
                        sectPr.SetAttr(SectAttr.BorderBottom, xmlReader.ReadBorder());
                        break;
                    case "right":
                        sectPr.SetAttr(SectAttr.BorderRight, xmlReader.ReadBorder());
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadLineNumberType(NrxXmlReader xmlReader, SectPr sectPr, 
            OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "countBy": // DOCX
                    case "count-by": // WML
                        sectPr.SetAttr(SectAttr.LineNumberCountBy, xmlReader.ValueAsInt);
                        break;
                    case "start":
                        sectPr.SetAttr(SectAttr.LineStartingNumber, xmlReader.ValueAsInt + 1);
                        break;
                    case "distance":
                        sectPr.SetAttr(SectAttr.LineNumberDistanceFromText, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "restart":
                        sectPr.SetAttr(SectAttr.LineNumberRestartMode, NrxSectEnum.XmlToLineNumberRestartMode(xmlReader.Value));
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadPageNumberType(NrxDocumentReaderBase reader, SectPr sectPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "fmt":
                        sectPr.PageNumberStyle = reader.IsDocx ? 
                            DocxEnum.DocxToNumberStyle(xmlReader.Value) : 
                            WmlEnum.WmlToNumberStyle(xmlReader.Value);
                        break;
                    case "start":
                        sectPr.PageStartingNumber = xmlReader.ValueAsInt;
                        sectPr.RestartPageNumbering = true;
                        break;
                    case "chapStyle": // DOCX
                    case "chap-style": // WML
                        sectPr.HeadingLevelForChapter = xmlReader.ValueAsInt;
                        break;
                    case "chapSep": // DOCX
                    case "chap-sep": // WML
                        sectPr.ChapterPageSeparator = NrxSectEnum.XmlToChapterPageSeparator(xmlReader.Value);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss,
                            reader.IsDocx ? WarningSource.Docx : WarningSource.WordML,
                            xmlReader.LocalName);
                        break;
                }
            }
        }

        private static void ReadCols(NrxXmlReader xmlReader, SectPr sectPr, OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "num":
                        sectPr.SetAttr(SectAttr.ColumnsCount, xmlReader.ValueAsInt);
                        break;
                    case "sep":
                        sectPr.SetAttr(SectAttr.ColumnsLineBetween, xmlReader.ValueAsBool);
                        break;
                    case "space":
                        sectPr.SetAttr(SectAttr.ColumnsSpacing, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "equalWidth":
                        sectPr.SetAttr(SectAttr.ColumnsEvenlySpaced, xmlReader.ValueAsBool);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }

            TextColumnCollectionInternal cols = new TextColumnCollectionInternal();

            while (xmlReader.ReadChild("cols"))
            {
                switch (xmlReader.LocalName)
                {
                    case "col":
                        cols.Add(ReadCol(xmlReader, complianceInfo));
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }

            if (cols.Count > 0)
                sectPr.SetAttr(SectAttr.Columns, cols);
        }

        private static TextColumn ReadCol(NrxXmlReader xmlReader, OoxmlComplianceInfo complianceInfo)
        {
            TextColumn col = new TextColumn();

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "w":
                        col.RawWidth = xmlReader.GetValueAsTwips(complianceInfo);
                        break;
                    case "space":
                        col.RawSpaceAfter = xmlReader.GetValueAsTwips(complianceInfo);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }

            return col;
        }

        private static void ReadDocGrid(NrxXmlReader xmlReader, SectPr sectPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "type":
                        SectionLayoutMode gridType = NrxSectEnum.XmlToDocGridType(xmlReader.Value);
                        sectPr.SetAttr(SectAttr.GridType, gridType);
                        break;
                    case "linePitch": // DOCX
                    case "line-pitch": // WML
                        sectPr.SetAttr(SectAttr.LinePitch, xmlReader.ValueAsInt);
                        break;
                    case "charSpace": // DOCX
                    case "char-space": // WML
                        sectPr.SetAttr(SectAttr.CharSpace, xmlReader.ValueAsInt);
                        break;
                    default:
                        xmlReader.Warn(WarningType.MinorFormattingLoss, WarningSource.Unknown, xmlReader.LocalName);
                        break;
                }
            }
        }
    }
}
