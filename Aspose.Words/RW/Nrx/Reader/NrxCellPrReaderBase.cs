// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/08/2007 by Vladimir Averkin

using Aspose.JavaAttributes;
using Aspose.Words.Nrx;
using Aspose.Words.Styles;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides base methods for reading cell properties from different document parts.
    /// NOTE: this class should be immutable (state cannot be modified after it is created).
    /// </summary>
    internal abstract class NrxCellPrReaderBase
    {
        protected NrxCellPrReaderBase(NrxParaReaderBase paraReader)
        {
            Debug.Assert(paraReader != null);
            mParaReader = paraReader;
        }

        /// <summary>
        /// Reads 'w:tcPr' element from the specified reader. Reader should be positioned to element start.
        /// </summary>
        internal void Read(NrxDocumentReaderBase reader, CellPr cellPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            xmlReader.MoveToElement();

            if (reader.LoadOptions.SkipFormatting)
            {
                // WORDSNET-13301 Text of second and later cells of a merged cell range in fixed table layout was included
                // in extracted text, although it should not be displayed.
                ReadHMergeOnly(xmlReader, cellPr);
                return;
            }

            string tagName = xmlReader.LocalName;

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "cnfStyle":
                        // This is a calculated "helper" value, ignore.
                        xmlReader.IgnoreElementNoWarn();
                        break;
                    case "tcW":
                        cellPr.SetPreferredWidthAndWidth(xmlReader.ReadLength(reader.ComplianceInfo));
                        break;
                    case "gridSpan":
                    {
                        // WORDSNET-22961 In accordance with [MS-OI29500] Word treats a gridSpan of 0 the same as a grid span of 1.
                        int gridSpan = xmlReader.ReadIntVal();
                        cellPr.SetAttr(CellAttr.Sys_CellSpan, (gridSpan == 0) ? 1 : gridSpan);
                        break;
                    }
                    case "hmerge": // WML
                    case "hMerge":
                        ReadHMerge(xmlReader, cellPr);
                        break;
                    case "vmerge": // WML
                    case "vMerge":
                    {
                        string vmerge = xmlReader.ReadVal();

                        if (vmerge == null)
                            vmerge = "continue";

                        cellPr.SetAttr(CellAttr.VerticalMerge, NrxTableEnum.XmlToCellMerge(vmerge));
                        break;
                    }
                    case "tcBorders":
                        ReadBorders(reader, cellPr, reader.ComplianceInfo);
                        break;
                    case "left":
                    case "right":
                    case "top":
                    case "bottom":
                        // WORDSNET-26675 Cell borders without surrounding tcBorders element.
                        cellPr.SetAttr(NrxXmlUtil.BorderNameToCellAttr(xmlReader.LocalName), xmlReader.ReadBorder());
                        break;
                    case "shd":
                        cellPr.SetAttr(CellAttr.Shading, xmlReader.ReadShading());
                        break;
                    case "noWrap":
                        cellPr.SetAttr(CellAttr.WrapText, !xmlReader.ReadBoolVal());
                        break;
                    case "hideMark":
                        cellPr.SetAttr(CellAttr.HideMark, xmlReader.ReadBoolVal());
                        break;
                    case "tcMar":
                        ReadMargins(xmlReader, cellPr, reader.ComplianceInfo);
                        break;
                    case "textFlow": // WML
                    case "textDirection":
                        cellPr.SetAttr(CellAttr.Orientation,
                            StyleConvertUtil.XmlToTextOrientation(xmlReader.ReadVal(), reader.ComplianceInfo));
                        break;
                    case "tcFitText":
                        cellPr.SetAttr(CellAttr.FitText, xmlReader.ReadBoolVal());
                        break;
                    case "vAlign":
                        cellPr.SetAttr(CellAttr.VerticalAlignment,
                            NrxTableEnum.XmlToCellVerticalAlignment(xmlReader.ReadVal()));
                        break;
                    case "p":
                    {
                        // WORDSNET-14871 Implement reading of the paragraph element from
                        // table cell properties level.
                        mParaReader.Read(reader);
                        break;
                    }
                    case "tblHeader":
                        // WORDSNET-19201 Implement reading of the 'tblHeader' element from
                        // table cell properties level.
                        reader.GlobalRowPr.SetAttr(TableAttr.HeadingFormat, xmlReader.ReadBoolVal());
                        break;
                    case "tcPr":
                        // WORDSNET-22773 Implemented reading of a nested 'tcPr' element from a parent 'tcPr'.
                        Read(reader, cellPr);
                        break;
                    default:
                        if (!ReadFormatSpecific(xmlReader.LocalName, reader, cellPr))
                            xmlReader.IgnoreElement();
                        break;
                }
            }

            // WORDSNET-14978 Allow preferred width for first cell in merge only and ignore rest.
            if(cellPr.HorizontalMerge == CellMerge.Previous)
                cellPr.Remove(CellAttr.PreferredWidth);
        }

        [JavaThrows(true)]
        protected abstract bool ReadFormatSpecific(string localName, NrxDocumentReaderBase reader, CellPr cellPr);

        /// <summary>
        /// Passes through all children of current XML node and reads the Horizontally Merged Cell element
        /// if it exists.
        /// </summary>
        private static void ReadHMergeOnly(NrxXmlReader xmlReader, CellPr cellPr)
        {
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                if ((xmlReader.LocalName == "hmerge") || // WML
                    (xmlReader.LocalName == "hMerge"))
                    ReadHMerge(xmlReader, cellPr);
                else
                    xmlReader.IgnoreElementNoWarn();
            }
        }

        /// <summary>
        /// Reads the Horizontally Merged Cell element.
        /// </summary>
        internal static void ReadHMerge(NrxXmlReader xmlReader, CellPr cellPr)
        {
            string hmerge = xmlReader.ReadVal();

            if (hmerge == null)
                hmerge = "continue";

            cellPr.SetAttr(CellAttr.HorizontalMerge, NrxTableEnum.XmlToCellMerge(hmerge));
        }

        private static void ReadBorders(NrxDocumentReaderBase reader, CellPr cellPr, OoxmlComplianceInfo cInfo)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("tcBorders"))
            {
                // Fix for C++, in C++ order of arguments in the method call is difference,
                // That is why xmlReader.ReadBorder() executes first and as a result xmlReader.LocalName returns incorrect value.
                // Fixed by caching xmlReader.LocalName value.
                string borderName = xmlReader.LocalName;
                switch (borderName)
                {
                    case "top":
                    case "bottom":
                    case "left":
                    case "right":
                    case "tl2br":
                    case "tr2bl":
                    case "insideH":
                    case "insideV":
                        cellPr.SetAttr(NrxXmlUtil.BorderNameToCellAttr(borderName), xmlReader.ReadBorder());
                        break;
                    case "start":   // iso29500
                    case "end":     // iso29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            cellPr.SetAttr(NrxXmlUtil.BorderNameToCellAttr(borderName), xmlReader.ReadBorder());
                            break;
                        }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadMargins(NrxXmlReader xmlReader, CellPr cellPr, OoxmlComplianceInfo cInfo)
        {
            while (xmlReader.ReadChild("tcMar"))
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        SetAttrMargin(xmlReader, cellPr, CellAttr.TopPadding, cInfo);
                        break;
                    case "left":
                        SetAttrMargin(xmlReader, cellPr, CellAttr.LeftPadding, cInfo);
                        break;
                    case "start": // iso29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            SetAttrMargin(xmlReader, cellPr, CellAttr.LeftPadding, cInfo);
                            break;
                        }
                    case "bottom":
                        SetAttrMargin(xmlReader, cellPr, CellAttr.BottomPadding, cInfo);
                        break;
                    case "right":
                        SetAttrMargin(xmlReader, cellPr, CellAttr.RightPadding, cInfo);
                        break;
                    case "end": // iso29500
                        {
                            cInfo.MarkAsIsoTransitional();
                            SetAttrMargin(xmlReader, cellPr, CellAttr.RightPadding, cInfo);
                            break;
                        }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Set margin attribute value for specified attribute collection.
        /// </summary>
        /// <param name="xmlReader"> Provide ability to read "xml" files.</param>
        /// <param name="cellPr"> Cell attribute collection.</param>
        /// <param name="atrKey"> Attribute key.</param>
        /// <param name="cInfo"> OOXML compliance info about the document.</param>
        private static void SetAttrMargin(NrxXmlReader xmlReader, CellPr cellPr, int atrKey, OoxmlComplianceInfo cInfo)
        {
            PreferredWidth width = xmlReader.ReadLength(cInfo);

            // WORDSNET-13442 In case when type of unit measure is "nil" and value does not contain additional info about
            // measure unit such length elements are skipped (same behavior as MSW).
            if (width.TypeOrig != 0)
                cellPr.SetAttr(atrKey, width.ValueRaw);
        }

        private readonly NrxParaReaderBase mParaReader;
    }
}
