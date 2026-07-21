// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2007 by Vladimir Averkin

using Aspose.Words.Framesets;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Settings;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading "Web Settings" package part
    /// </summary>
    internal static class DocxWebSettingsReader
    {
        /// <summary>
        /// Reads "Web Settings" part for the specified document.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.WebSettings);
            if (xmlReader == null)
                return;

            DocumentBase doc = reader.Document;
            DocPr docPr = doc.DocPr;

            while (xmlReader.ReadChild("webSettings"))    // w:webSettings
            {
                switch (xmlReader.LocalName)
                {
                    case "frameset":
                    {
                        Frameset frameset = new Frameset();
                        // Default value.
                        frameset.FramesetBorderType = FramesetBorderType.Raised;
                        ((Document)doc).Frameset = frameset;
                        NrxFramesetReader.Read(reader, frameset, true);
                        break;
                    }
                    case "divs":
                        NrxSettingsReader.ReadDivs(reader);
                        break;
                    case "encoding":
                        docPr.WebPageEncoding = xmlReader.ReadVal();
                        break;
                    case "optimizeForBrowser":
                        docPr.OptimizeForBrowser = xmlReader.ReadBoolVal();
                        while(xmlReader.MoveToNextAttribute())
                        {
                            if (xmlReader.LocalName == "target") // iso29500 specific.
                            {
                                reader.ComplianceInfo.MarkAsIsoTransitional();
                                docPr.WebTarget = DocxDopEnum.DocxToWebTarget(xmlReader.Value);
                            }
                        }
                        break;
                    case "relyOnVML":
                        docPr.RelyOnVml = xmlReader.ReadBoolVal();
                        break;
                    case "allowPNG":
                        docPr.AllowPng = xmlReader.ReadBoolVal();
                        break;
                    case "doNotRelyOnCSS":
                        docPr.DoNotRelyOnCss = xmlReader.ReadBoolVal();
                        break;
                    case "doNotSaveAsSingleFile":
                        docPr.DoNotSaveWebPagesAsSingleFile = xmlReader.ReadBoolVal();
                        break;
                    case "doNotOrganizeInFolder":
                        docPr.DoNotOrganizeInFolder = xmlReader.ReadBoolVal();
                        break;
                    case "doNotUseLongFileNames":
                        docPr.DoNotUseLongFileNames = xmlReader.ReadBoolVal();
                        break;
                    case "pixelsPerInch":
                        docPr.PixelsPerInch = xmlReader.ReadIntVal();
                        break;
                    case "targetScreenSz":
                        docPr.TargetScreenSize = StyleConvertUtil.XmlToScreenSize(xmlReader.ReadVal());
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }
    }
}
