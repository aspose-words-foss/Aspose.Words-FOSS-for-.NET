// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/07/2014 by Andrey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Xml;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Reader for DrawingML extensions.
    /// Element 20.1.2.2.15 extLst (Extension List, CT_OfficeArtExtensionList).
    /// </summary>
    internal class DmlExtensionListReader : DmlReaderBase
    {
        private DmlExtensionListReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
            mXmlReader = reader.XmlReader;
        }

        public static StringToObjDictionary<DmlExtension> Read(DocxDocumentReaderBase reader)
        {
            DmlExtensionListReader extensionListReader = new DmlExtensionListReader(reader);
            return extensionListReader.ReadCore();
        }

        /// <summary>
        /// Reads DmlExtension complex type value of the DML element from the XML reader.
        /// </summary>
        private StringToObjDictionary<DmlExtension> ReadCore()
        {
            StringToObjDictionary<DmlExtension> dmlExtensions = new StringToObjDictionary<DmlExtension>();
            string tagName = mXmlReader.LocalName;

            // This is the normal behavior when the position is on the element end after reading an element.
            if (!mXmlReader.ReadChild(tagName))
                return dmlExtensions;

            while (true)
            {
                // We need to exit if reached end of the parent. But if we are on the start of a next element,
                // then everything is already done, we just need to process the tag.
                if (mXmlReader.IsEndElement(tagName))
                    break;

                ReadChild(dmlExtensions);
            }

            return dmlExtensions;
        }

        /// <summary>
        /// Reads DML extension and adds it to the array of extensions.
        /// </summary>
        private void ReadChild(StringToObjDictionary<DmlExtension> dmlExtensions)
        {
            switch (mXmlReader.LocalName)
            {
                case DmlExtensionName.Ext:
                {
                    string extensionUri = mXmlReader.ReadAttribute(DmlExtensionName.Uri, string.Empty);
                    DmlExtension dmlExtension = new DmlExtension(extensionUri, null);
                    XmlTextReaderNamespaceStorage nameSpaceStorage = new XmlTextReaderNamespaceStorage(null);

                    // Whitespace preserving is significant, otherwise spaces in texts can be corrupted.
                    string extensionXml = mXmlReader.ReadOuterXml(null, nameSpaceStorage);
                    mXmlReader = mDocumentReader.CreateAndPushPartReader(extensionXml, nameSpaceStorage.ConvertTo());

                    switch (extensionUri)
                    {
                        case DmlExtensionUri.NonVisualPr:
                            dmlExtension.NvPr = ReadNonVisualPrExtension();
                            break;
                        case DmlExtensionUri.HiddenFill:
                            dmlExtension.FillPr = ReadHiddenFillExtension(dmlExtension);
                            break;
                        case DmlExtensionUri.UseLocalDpi:
                            dmlExtension.UseLocalDpi = ReadUseLocalDpiExtension();
                            break;
                        case DmlExtensionUri.DataLabels:
                            dmlExtension.DataLabelPr = DmlChartComplexTypesReader.ReadChartExtDataLabelPr(mDocumentReader);
                            break;
                        case DmlExtensionUri.Filtering:
                            ReadFilteringExtension(dmlExtension, extensionXml);
                            break;
                        case DmlExtensionUri.UniqueId:
                            ReadUniqueIdExtension(dmlExtension, extensionXml);
                            break;
                        case DmlExtensionUri.VideoPr:
                            dmlExtension.WebVideoPr = ReadWebVideoExtension();
                            break;
                        case DmlExtensionUri.DataModelExt:
                            dmlExtension.DrawingRelId = ReadDiagramDataModelExtension();
                            break;
                        case DmlExtensionUri.HiddenLine:
                            dmlExtension.OutlinePr = ReadHiddenLineExtension();
                            break;
                        case DmlExtensionUri.InvertSolidFillFmt:
                            dmlExtension.DmlChartSpPr = ReadDmlSpPrExtension();
                            break;
                        case DmlExtensionUri.SvgBlip:
                            dmlExtension.SvgBlip = ReadSvgBlip();
                            break;
                        case DmlExtensionUri.Decorative:
                            dmlExtension.Decorative = ReadDecorativeExtension();
                            break;
                        default:
                        {
                            // Whitespace preserving is significant, otherwise spaces in texts can be corrupted.
                            dmlExtension.XmlDoc = extensionXml;
                            ExtractEmbeddedImages(DmlExtensionName.Ext, dmlExtension);
                            break;
                        }
                    }

                    dmlExtensions[dmlExtension.Uri] = dmlExtension;
                    mXmlReader = mDocumentReader.RestorePartReader();
                    break;
                }
                default:
                {
                    WarnUnexpected(mXmlReader);

                    // WORDSNET-25518 The empty element cannot be skipped just with XmlReader.Skip().
                    // So, we need to move to the next node in this case forcibly.
                    if (mXmlReader.IsEmptyElement)
                        mXmlReader.ReadNextNode();
                    else
                        mXmlReader.IgnoreElementNoWarn();

                    break;
                }
            }
        }

        /// <summary>
        /// Reads dataModel extension.
        /// </summary>
        private string ReadDiagramDataModelExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.DataModelExt:
                        return mXmlReader.ReadAttribute("relId", "");
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Reads webVideo extension.
        /// </summary>
        private DmlWebVideoProperties ReadWebVideoExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.WebVideoPr:
                    {
                        if (mXmlReader.Prefix == "wp15")
                            mDocumentReader.ComplianceInfo.IsDrawingExtensions = true;

                        return ReadWebVideoPr();
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads webVideoPr element.
        /// </summary>
        /// <remarks>
        /// [MS-ODRAWXML] 2.20.1.1 webVideoPr.
        /// Element that specifies the properties for displaying an online video to the user.
        /// </remarks>
        private DmlWebVideoProperties ReadWebVideoPr()
        {
            DmlWebVideoProperties webVideoPr = new DmlWebVideoProperties();

            while (mXmlReader.MoveToNextAttribute())
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.EmbeddedHtml:
                        webVideoPr.EmbedHtml = mXmlReader.Value;
                        break;
                    case "w":
                        webVideoPr.FrameWidth = mXmlReader.ValueAsDouble;
                        break;
                    case "h":
                        webVideoPr.FrameHeight = mXmlReader.ValueAsDouble;
                        break;
                    default:
                        mXmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Nrx,
                            string.Format(WarningStrings.UnexpectedAttribute, mXmlReader.LocalName));
                        break;
                }
            }

            return webVideoPr;
        }

        /// <summary>
        /// Reads hiddenLine extension.
        /// </summary>
        private DmlOutline ReadHiddenLineExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.HiddenLine:
                        return DmlOutlineReader.Read(mDocumentReader);
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads useLocalDpi extension.
        /// </summary>
        private bool ReadUseLocalDpiExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.UseLocalDpi:
                    {
                        // TODO: Do we need to set IsDrawingExtensions here? It seems yes, but check DD comments in
                        // OoxmlComplianceInfo.GetCompliance() method. Inside test TestDmlPictureBullet it is needed to save
                        // document to Ecma376 but if IsDrawingExtensions is set it will be impossible.
                        return mXmlReader.ReadBoolAttribute("val", false);
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads Decorative extension.
        /// </summary>
        private bool ReadDecorativeExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.Decorative:
                        return mXmlReader.ReadBoolAttribute("val", false);
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads cNvPr extensions.
        /// </summary>
        private DmlNvDrawingProperties ReadNonVisualPrExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.CNvPr:
                    {
                        if (mXmlReader.Prefix == "dgm14")
                            mDocumentReader.ComplianceInfo.IsDrawingExtensions = true;

                        return ReadNvPr();
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads hiddenFill extension.
        /// </summary>
        private DmlFill ReadHiddenFillExtension(DmlExtension dmlExtension)
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.HiddenFill:
                    {
                        mDocumentReader.ComplianceInfo.IsDrawingExtensions = true;
                        return ReadHiddenFill(dmlExtension);
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads hiddenFill element.
        /// </summary>
        private DmlFill ReadHiddenFill(DmlExtension dmlExtension)
        {
            while (mXmlReader.ReadChild(DmlExtensionName.HiddenFill))
            {
                switch (mXmlReader.LocalName)
                {
                    case "noFill":
                    case "grpFill":
                    case "pattFill":
                    case "blipFill":
                    case "gradFill":
                    case "solidFill":
                    {
                        // Ignore content, when more then one fill defined.
                        if (dmlExtension.FillPr != null)
                        {
                            WarnUnexpectedAndIgnoreElement(mXmlReader);
                            break;
                        }

                        return DmlFillReader.Read(mDocumentReader);
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads "InvertSolidFillFmt" extension.
        /// </summary>
        private DmlChartSpPr ReadDmlSpPrExtension()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.InvertSolidFillFmt:
                        return ReadSpPr();
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads <see cref="DmlChartSpPr"/>.
        /// </summary>
        private DmlChartSpPr ReadSpPr()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.InvertSolidFillFmt))
            {
                switch (mXmlReader.LocalName)
                {
                    case "spPr":
                    {
                        mDocumentReader.ComplianceInfo.IsDrawingExtensions = true;
                        DmlChartSpPr spPr = new DmlChartSpPr();
                        DmlChartComplexTypesReader.ReadChartSpPr(mDocumentReader, spPr);
                        return spPr;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Reads cNvPr element.
        /// </summary>
        private DmlNvDrawingProperties ReadNvPr()
        {
            int id = mXmlReader.ReadIntAttribute("id", 0);
            string name = mXmlReader.ReadAttribute("name", null);

            DmlNvDrawingProperties nvPr = new DmlNvDrawingProperties(id, name);

            while (mXmlReader.ReadChild(DmlExtensionName.CNvPr))
            {
                switch (mXmlReader.LocalName)
                {
                    case "hlinkClick":
                        nvPr.HlinkClick = DmlHlinkReader.Read(mDocumentReader);
                        break;
                    case "hlinkHover":
                        nvPr.HlinkHover = DmlHlinkReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return nvPr;
        }

        /// <summary>
        /// Reads svgBlip extension.
        /// </summary>
        private DmlBlip ReadSvgBlip()
        {
            while (mXmlReader.ReadChild(DmlExtensionName.Ext))
            {
                switch (mXmlReader.LocalName)
                {
                    case DmlExtensionName.SvgBlip:
                    {
                        return DmlFillReader.ReadBlip(mDocumentReader);
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts embedded images and stores them in <see cref="DmlExtension.EmbeddedImages"/> in order
        /// to create correct relation during writing raw xml data.
        /// </summary>
        private void ExtractEmbeddedImages(string parentElement, DmlExtension dmlExtension)
        {
            while (mXmlReader.ReadChild(parentElement))
            {
                string embed = mXmlReader.ReadAttribute("embed", string.Empty);
                if (StringUtil.HasChars(embed))
                {
                    byte[] embedImageBytes = mDocumentReader.IsValidImagePart(embed) ? mDocumentReader.GetBinData(embed) : null;
                    dmlExtension.EmbeddedImages.Add(embed, embedImageBytes);
                }

                if (!mXmlReader.IsEmptyElement)
                    ExtractEmbeddedImages(mXmlReader.LocalName, dmlExtension);

                mXmlReader.IgnoreElementNoWarn();
            }
        }

        /// <summary>
        /// Reads filtering extension.
        /// </summary>
        private void ReadFilteringExtension(DmlExtension dmlExtension, string extensionXml)
        {
            DmlChartValueRef valueRef = DmlChartComplexTypesReader.ReadChartDataLabelsRange(mDocumentReader);

            // WORDSNET-15427 Read and write "filtering" extension as raw XML, when “datalabelsRange”
            // element is missing.
            if (valueRef != null)
                dmlExtension.DataLabelsRangeData.ValueRef = valueRef;
            else
                dmlExtension.XmlDoc = extensionXml;
        }

        /// <summary>
        /// Reads unique ID extension.
        /// </summary>
        private void ReadUniqueIdExtension(DmlExtension dmlExtension, string extensionXml)
        {
            dmlExtension.DataLabelId = DmlChartComplexTypesReader.ReadUniqueIdExtension(mDocumentReader);
            dmlExtension.XmlDoc = extensionXml;
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
        private NrxXmlReader mXmlReader;
    }
}
