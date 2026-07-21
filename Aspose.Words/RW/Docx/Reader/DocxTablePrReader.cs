// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/08/2007 by Vladimir Averkin

using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Styles;
using Aspose.Words.Tables;


namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading table properties from different document parts.
    /// </summary>
    internal static class DocxTablePrReader
    {
        /// <summary>
        /// Reads table 'w:tblPr' element from the specified reader.
        /// </summary>
        internal static void ReadTblPr(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            ReadTblPr(reader, tablePr, false);
        }

        /// <summary>
        /// Reads style 'w:tblPr' element from the specified reader.
        /// </summary>
        internal static void ReadStyleTblPr(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            ReadTblPr(reader, tablePr, true);
        }

        /// <summary>
        /// Reads 'w:tblPr' element from the specified reader.
        /// </summary>
        /// <param name="reader">DocxReader to read from. Should be positioned to element start.</param>
        /// <param name="tablePr">Table properties.</param>
        /// <param name="isStyle">True when reading a style, otherwise false.</param>
        private static void ReadTblPr(NrxDocumentReaderBase reader, TablePr tablePr, bool isStyle)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;
            xmlReader.MoveToElement();

            if (reader.LoadOptions.SkipFormatting)
            {
                // WORDSNET-13301 Table layout type should be read to determine if merged cells should be combined.
                // Otherwise, different text may be returned in the skip-formatting mode.
                ReadTableLayoutIfExists(reader, tablePr);
                return;
            }

            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "tblStyle":
                        int istd = reader.ResolveStyleIdToIstd(xmlReader.ReadVal(), StyleIndex.TableNormal);
                        tablePr.Istd = istd;
                        break;
                    case "tblpPr":
                        // MS-OI29500 17.7.6.4.
                        // Word does not allow tblpPr element to be children of the tblPr element in style table properties.
                        if (!isStyle)
                            ReadTblpPr(xmlReader, tablePr, complianceInfo);
                        break;
                    case "tblOverlap":
                        if (xmlReader.ReadVal() == "never")
                            tablePr.SetAttr(TableAttr.AllowOverlap, false);
                        break;
                    case "bidiVisual":
                        tablePr.SetAttr(TableAttr.Bidi, xmlReader.ReadBoolVal());
                        break;
                    case "tblStyleRowBandSize":
                        tablePr.SetAttr(TableAttr.StyleRowBandSize, xmlReader.ReadIntVal());
                        break;
                    case "tblStyleColBandSize":
                        tablePr.SetAttr(TableAttr.StyleColBandSize, xmlReader.ReadIntVal());
                        break;
                    default:
                        ReadTblPrCommon(reader, tablePr);
                        break;
                }
            }

            // WORDSNET-22587 When TableNormal is not default paragraph style and there is no style defined at the table,
            // Word applies default style to this table explicitly.
            if (!isStyle)
                reader.ApplyDefaultStyle(tablePr, StyleType.Table);
        }

        /// <summary>
        /// Reads a tblLayout element if it exists in children of current XML node.
        /// </summary>
        private static void ReadTableLayoutIfExists(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;

            while (xmlReader.ReadChild(tagName))
            {
                if (xmlReader.LocalName == "tblLayout")
                    ReadTblLayout(xmlReader, tablePr);
                else
                    xmlReader.IgnoreElementNoWarn();
            }
        }

        /// <summary>
        /// Reads 'w:tblpPr' element from the specified reader. Reader should be positioned to element start.
        /// </summary>
        private static void ReadTblpPr(NrxXmlReader xmlReader, TablePr tablePr, OoxmlComplianceInfo complianceInfo)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "leftFromText":
                        tablePr.SetAttr(TableAttr.FrameDistanceFromLeft, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "rightFromText":
                        tablePr.SetAttr(TableAttr.FrameDistanceFromRight, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "topFromText":
                        tablePr.SetAttr(TableAttr.FrameDistanceFromTop, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "bottomFromText":
                        tablePr.SetAttr(TableAttr.FrameDistanceFromBottom, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "vertAnchor":
                        tablePr.SetAttr(TableAttr.RelativeVerticalPosition, StyleConvertUtil.XmlToRelativeVerticalPosition(xmlReader.Value));
                        break;
                    case "horzAnchor":
                        tablePr.SetAttr(TableAttr.RelativeHorizontalPosition, StyleConvertUtil.XmlToTableRelativeHorizontalPosition(xmlReader.Value));
                        break;
                    case "tblpXSpec":
                        tablePr.SetAttr(TableAttr.HorizontalAlignment, StyleConvertUtil.XmlToHorizontalAlignment(xmlReader.Value));
                        break;
                    case "tblpX":
                        NrxTableUtil.SetPositionAttribute(tablePr, TableAttr.FrameLeft, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    case "tblpYSpec":
                        tablePr.SetAttr(TableAttr.VerticalAlignment, StyleConvertUtil.XmlToVerticalAlignment(xmlReader.Value));
                        break;
                    case "tblpY":
                        NrxTableUtil.SetPositionAttribute(tablePr, TableAttr.FrameTop, xmlReader.GetValueAsTwips(complianceInfo));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'w:tblPrEx' element from the specified reader. Reader should be positioned to element start.
        /// </summary>
        internal static void ReadTblPrEx(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            if (reader.LoadOptions.SkipFormatting)
            {
                xmlReader.IgnoreElementNoWarn();
                return;
            }

            while (xmlReader.ReadChild("tblPrEx"))
                ReadTblPrCommon(reader, tablePr);
        }

        /// <summary>
        /// Reads child elements common to 'w:tblPr' and 'w:tblPrEx'. Reader should be positioned to child element start.
        /// </summary>
        private static void ReadTblPrCommon(NrxDocumentReaderBase reader, TablePr tablePr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            switch (xmlReader.LocalName)
            {
                case "tblW":
                    tablePr.SetAttr(TableAttr.PreferredWidth, xmlReader.ReadLength(reader.ComplianceInfo));
                    break;
                case "jc":
                    tablePr.Alignment = NrxTableEnum.XmlToTableAlignment(xmlReader.ReadVal(), reader.ComplianceInfo);
                    break;
                case "tblCellSpacing":
                    {
                        PreferredWidth cellSpacing = xmlReader.ReadCellSpacing(reader.ComplianceInfo);
                        if (cellSpacing != null)
                            tablePr.SetAttr(TableAttr.CellSpacing, cellSpacing);
                        break;
                    }
                case "tblInd":
                    tablePr.SetAttr(TableAttr.LeftIndent, xmlReader.ReadLengthInTwips(reader.ComplianceInfo));
                    break;
                case "tblBorders":
                    ReadBorders(xmlReader, tablePr, reader.ComplianceInfo);
                    break;
                case "shd":
                    tablePr.SetAttr(TableAttr.Shading, xmlReader.ReadShading());
                    break;
                case "tblLayout":
                    ReadTblLayout(xmlReader, tablePr);
                    break;
                case "tblCellMar":
                    ReadMargins(xmlReader, tablePr, reader.ComplianceInfo);
                    break;
                case "tblLook":
                    tablePr.SetAttr(TableAttr.StyleOptions,
                        DocxTableStyleLookReader.ReadTableStyleLook(xmlReader, reader.ComplianceInfo));
                    break;
                case "tblPrChange":     // This occurs when reading tbl.tblPr
                case "tblPrExChange":   // This occurs when reading tr.tblPrEx
                    DocxAnnotationReader.ReadTablePrFormatRevision(reader, tablePr);
                    break;
                case "tblDescription":
                    tablePr.Description = xmlReader.ReadVal();
                    break;
                case "tblCaption":
                    tablePr.Title = xmlReader.ReadVal();
                    break;
                default:
                    xmlReader.IgnoreElement();
                    break;
            }
        }

        private static void ReadBorders(NrxXmlReader xmlReader, TablePr tablePr, OoxmlComplianceInfo cInfo)
        {
            while (xmlReader.ReadChild("tblBorders"))
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        tablePr.SetAttr(TableAttr.BorderTop, xmlReader.ReadBorder());
                        break;
                    case "left":
                        tablePr.SetAttr(TableAttr.BorderLeft, xmlReader.ReadBorder());
                        break;
                    case "start": // iso 29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            tablePr.SetAttr(TableAttr.BorderLeft, xmlReader.ReadBorder());
                            break;
                        }
                    case "bottom":
                        tablePr.SetAttr(TableAttr.BorderBottom, xmlReader.ReadBorder());
                        break;
                    case "right":
                        tablePr.SetAttr(TableAttr.BorderRight, xmlReader.ReadBorder());
                        break;
                    case "end": // iso29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            tablePr.SetAttr(TableAttr.BorderRight, xmlReader.ReadBorder());
                            break;
                        }
                    case "insideH":
                        tablePr.SetAttr(TableAttr.BorderHorizontal, xmlReader.ReadBorder());
                        break;
                    case "insideV":
                        tablePr.SetAttr(TableAttr.BorderVertical, xmlReader.ReadBorder());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadTblLayout(NrxXmlReader xmlReader, TablePr tablePr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "type":
                    {
                        // RK In the OOXML spec there it is not clear whether the value is "auto" or "autofit" so work from the opposite.
                        tablePr.SetAttr(TableAttr.AllowAutoFit, (xmlReader.Value != "fixed"));
                        break;
                    }
                    default:
                        break;
                }
            }
        }

        private static void ReadMargins(NrxXmlReader xmlReader, TablePr tablePr, OoxmlComplianceInfo cInfo)
        {
            while (xmlReader.ReadChild("tblCellMar"))
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        tablePr.TopPadding = xmlReader.ReadLengthInTwips(cInfo);
                        break;
                    case "left":
                        tablePr.LeftPadding = xmlReader.ReadLengthInTwips(cInfo);
                        break;
                    case "start": // iso29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            tablePr.LeftPadding = xmlReader.ReadLengthInTwips(cInfo);
                            break;
                        }
                    case "bottom":
                        tablePr.BottomPadding = xmlReader.ReadLengthInTwips(cInfo);
                        break;
                    case "right":
                        tablePr.RightPadding = xmlReader.ReadLengthInTwips(cInfo);
                        break;
                    case "end": // iso29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            tablePr.RightPadding = xmlReader.ReadLengthInTwips(cInfo);
                            break;
                        }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }
    }
}
