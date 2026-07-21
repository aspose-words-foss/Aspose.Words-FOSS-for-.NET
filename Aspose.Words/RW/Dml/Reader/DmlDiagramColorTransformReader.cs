// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class used to read DML diagram color transform.
    /// </summary>
    internal class DmlDiagramColorTransformReader : DmlReaderBase
    {
        private DmlDiagramColorTransformReader(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            mDocumentReader = reader;
            mXmlReader = reader.XmlReader;
            mComplianceInfo = complianceInfo;
        }

        internal static DmlDiagramColorTransform Read(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            DmlDiagramColorTransformReader colorTransformReader = new DmlDiagramColorTransformReader(reader, complianceInfo);
            return colorTransformReader.ReadCode();
        }

        private DmlDiagramColorTransform ReadCode()
        {
            if (mXmlReader.LocalName != "colorsDef")
                return null;

            DmlDiagramColorTransform transform = new DmlDiagramColorTransform();
            transform.UniqueId = mXmlReader.ReadAttribute("uniqueId", "");
            transform.MinVersion = mXmlReader.ReadAttribute("minVer", "");

            List<DmlColorTransformationStyleLabel> lableList = new List<DmlColorTransformationStyleLabel>();
            List<DmlDiagramString> titles = new List<DmlDiagramString>();
            List<DmlDiagramString> descriptions = new List<DmlDiagramString>();
            while (mXmlReader.ReadChild("colorsDef"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "title":
                        titles.Add(DmlDiagramComplexTypesReader.ReadString(mXmlReader));
                        break;
                    case "desc":
                        descriptions.Add(DmlDiagramComplexTypesReader.ReadString(mXmlReader));
                        break;
                    case "catLst":
                        transform.Categories = DmlDiagramComplexTypesReader.ReadCategories(mXmlReader);
                        break;
                    case "styleLbl":
                        lableList.Add(ReadStyleLabel());
                        break;
                    case "extLst":
                        transform.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            transform.StyleLabels = lableList.ToArray();
            transform.Titles = titles.ToArray();
            transform.Descriptions = descriptions.ToArray();

            return transform;
        }

        private DmlColorTransformationStyleLabel ReadStyleLabel()
        {
            DmlColorTransformationStyleLabel label = new DmlColorTransformationStyleLabel();
            label.Name = mXmlReader.ReadAttribute("name", "");

            while (mXmlReader.ReadChild("styleLbl"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "fillClrLst":
                        label.FillColorList = ReadColors();
                        break;
                    case "linClrLst":
                        label.LineColorList = ReadColors();
                        break;
                    case "effectClrLst":
                        label.EffectColorList = ReadColors();
                        break;
                    case "txLinClrLst":
                        label.TextLineColorList = ReadColors();
                        break;
                    case "txFillClrLst":
                        label.TextFillColorList = ReadColors();
                        break;
                    case "txEffectClrLst":
                        label.TextEffectColorList = ReadColors();
                        break;
                    case "extLst":
                        label.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }
            return label;
        }

        private DmlDiagramColors ReadColors()
        {
            string tag = mXmlReader.LocalName;

            DmlDiagramColors colors = new DmlDiagramColors();
            colors.ColorApplicationMethod =
                DmlDiagramEnum.DmlToColorApplicationMethod(mXmlReader.ReadAttribute("meth", "span"));
            colors.HueDirection = DmlDiagramEnum.DmlToHueDirection(mXmlReader.ReadAttribute("hueDir", "cw"));

            List<DmlColor> colorList = new List<DmlColor>();
            while (mXmlReader.ReadChild(tag))
            {
                DmlColor color = DmlColorReader.Read(mXmlReader, mComplianceInfo);
                if(color != null)
                    colorList.Add(color);
            }
            colors.Colors = colorList.ToArray();

            return colors;
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
        private readonly NrxXmlReader mXmlReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
