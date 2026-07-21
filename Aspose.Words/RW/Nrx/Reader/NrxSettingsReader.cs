// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2017 by Alexey Morozov

using Aspose.Words.Nrx;
using Aspose.Words.Settings;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Implements reading common elements related to document setting for both DOCX and WordML formats.
    /// </summary>
    internal static class NrxSettingsReader
    {
        internal static void ReadRsids(NrxXmlReader xmlReader, DocPr docPr)
        {
            string attrVal;

            while (xmlReader.ReadChild("rsids"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rsidRoot":
                    {  
                        attrVal = xmlReader.ReadVal();
                        // WORDSNET-16208 Do not set document properties when attribute "val" is missed in "rsid" or "rsidRoot".
                        if (StringUtil.HasChars(attrVal))
                            docPr.RsidRoot = NrxXmlUtil.HexToInt(attrVal);
                        break;
                    }
                    case "rsid":
                    {
                        attrVal = xmlReader.ReadVal();
                        if (StringUtil.HasChars(attrVal))
                            docPr.Rsids.Add(NrxXmlUtil.HexToInt(attrVal));
                        break;
                    }
                    default:
                        // Ignore.
                        break;
                }
            }
        }

        internal static void ReadDivs(NrxDocumentReaderBase reader)
        {
            ReadDivs(reader, 0);
        }

        /// <summary>
        /// Reads <see cref="MultiplePagesType"/>.
        /// </summary>
        /// <remarks>
        /// Word seems to set the priority for types when reading several different types,
        /// but lets postpone this for a while.
        /// </remarks>
        internal static void ReadMultiplePagesType(NrxDocumentReaderBase reader, DocPr docPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            switch (xmlReader.LocalName)
            {
                case "bookFoldPrinting":
                {
                    if (xmlReader.ReadBoolVal())
                        docPr.MultiplePages = MultiplePagesType.BookFoldPrinting;
                    break;
                }
                case "bookFoldRevPrinting":
                {
                    if (xmlReader.ReadBoolVal())
                        docPr.MultiplePages = MultiplePagesType.BookFoldPrintingReverse;
                    break;
                }
                case "mirrorMargins":
                {
                    if (xmlReader.ReadBoolVal())
                        docPr.MultiplePages = MultiplePagesType.MirrorMargins;
                    break;
                }
                case "printTwoOnOne":
                {
                    if (xmlReader.ReadBoolVal())
                        docPr.MultiplePages = MultiplePagesType.TwoPagesPerSheet;
                    break;
                }
                default:
                    xmlReader.IgnoreElement();
                    break;
            }
        }

        private static void ReadDivs(NrxDocumentReaderBase reader, int parentId)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            if (reader.LoadOptions.SkipFormatting)
            {
                xmlReader.IgnoreElementNoWarn();
                return;
            }

            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                if (xmlReader.LocalName == "div")
                {
                    HtmlBlock htmlBlock = new HtmlBlock(xmlReader.ReadIntAttribute("id", -1));
                    htmlBlock.ParentId = parentId;
                    ReadDiv(reader, htmlBlock);
                    htmlBlock.Refine();
                    reader.Document.HtmlBlockCollection.Add(htmlBlock);
                }
                else
                    xmlReader.IgnoreElement();
            }
        }

        private static void ReadBorders(NrxDocumentReaderBase reader, HtmlBlock htmlBlock)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("divBdr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "top":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.BorderTop, xmlReader.ReadBorder());
                        break;
                    case "bottom":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.BorderBottom, xmlReader.ReadBorder());
                        break;
                    case "left":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.BorderLeft, xmlReader.ReadBorder());
                        break;
                    case "right":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.BorderRight, xmlReader.ReadBorder());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads single 'div' element .
        /// </summary>
        private static void ReadDiv(NrxDocumentReaderBase reader, HtmlBlock htmlBlock)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            while (xmlReader.ReadChild("div"))
            {
                switch (xmlReader.LocalName)
                {
                    case "blockQuote":
                        htmlBlock.HtmlBlockType = HtmlBlockType.BlockQuote;
                        break;
                    case "bodyDiv":
                        htmlBlock.HtmlBlockType = HtmlBlockType.Body;
                        break;
                    case "divsChild":
                        ReadDivs(reader, htmlBlock.Id);
                        break;
                    case "divBdr":
                        ReadBorders(reader, htmlBlock);
                        break;
                    case "marLeft":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.HtmlMarginLeft, xmlReader.ReadValAsTwips(complianceInfo));
                        break;
                    case "marRight":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.HtmlMarginRight, xmlReader.ReadValAsTwips(complianceInfo));
                        break;
                    case "marTop":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.HtmlMarginTop, xmlReader.ReadValAsTwips(complianceInfo));
                        break;
                    case "marBottom":
                        htmlBlock.ParaPr.SetAttr(ParaAttr.HtmlMarginBottom, xmlReader.ReadValAsTwips(complianceInfo));
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }
    }
}
