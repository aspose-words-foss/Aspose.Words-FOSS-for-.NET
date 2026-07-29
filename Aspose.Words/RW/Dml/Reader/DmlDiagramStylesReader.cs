// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class used to read DML diagram style definition.
    /// </summary>
    internal class DmlDiagramStylesReader : DmlReaderBase
    {
        private DmlDiagramStylesReader(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            mDocumentReader = reader;
            mXmlReader = reader.XmlReader;
            mComplianceInfo = complianceInfo;
        }

        internal static DmlDiagramStyleDefinition Read(DocxDocumentReaderBase reader, OoxmlComplianceInfo complianceInfo)
        {
            DmlDiagramStylesReader diagramStylesReader = new DmlDiagramStylesReader(reader, complianceInfo);
            return diagramStylesReader.ReadCore();
        }

        private DmlDiagramStyleDefinition ReadCore()
        {
            if (mXmlReader.LocalName != "styleDef")
                return null;

            DmlDiagramStyleDefinition styles = new DmlDiagramStyleDefinition();
            styles.UniqueId = mXmlReader.ReadAttribute("uniqueId", "");
            styles.MinVersion = mXmlReader.ReadAttribute("minVer", "");

            List<DmlStyleLabel> labelList = new List<DmlStyleLabel>();
            List<DmlDiagramString> titles = new List<DmlDiagramString>();
            List<DmlDiagramString> descriptions = new List<DmlDiagramString>();
            while (mXmlReader.ReadChild("styleDef"))
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
                        styles.Categories = DmlDiagramComplexTypesReader.ReadCategories(mXmlReader);
                        break;
                    case "styleLbl":
                        labelList.Add(ReadStyleLabel());
                        break;
                    case "scene3d":
                        styles.Scene3D = DmlScene3DReader.ReadScene3DProperties(mDocumentReader, mComplianceInfo);
                        break;
                    case "extLst":
                        styles.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            styles.StyleLabels = labelList.ToArray();
            styles.Titles = titles.ToArray();
            styles.Descriptions = descriptions.ToArray();

            return styles;
        }

        private DmlStyleLabel ReadStyleLabel()
        {
            DmlStyleLabel label = new DmlStyleLabel();
            label.Name = mXmlReader.ReadAttribute("name", "");

            while (mXmlReader.ReadChild("styleLbl"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "style":
                        label.Style = DmlShapeStyleReader.Read(mXmlReader, mComplianceInfo);
                        break;
                    case "txPr":
                        label.TextProperties = ReadTextProperties();
                        break;
                    case "scene3d":
                        label.Scene3D = DmlScene3DReader.ReadScene3DProperties(mDocumentReader, mComplianceInfo);
                        break;
                    case "sp3d":
                        label.Shape3D = DmlScene3DReader.ReadShape3DProperties(mDocumentReader, mComplianceInfo);
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

        private DmlDiagramTextPr ReadTextProperties()
        {
            DmlDiagramTextPr txPr = new DmlDiagramTextPr();

            while (mXmlReader.ReadChild("txPr"))
            {
                switch (mXmlReader.LocalName)
                {
                    case "flatTx":
                        txPr.FlatText = ReadFlatText();
                        break;
                    case "sp3d":
                        txPr.Shape3DProperties = DmlScene3DReader.ReadShape3DProperties(mDocumentReader, mComplianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(mXmlReader);
                        break;
                }
            }

            return txPr;
        }

        private DmlFlatText ReadFlatText()
        {
            DmlFlatText flatText = new DmlFlatText();
            flatText.ZCoordinate = mXmlReader.ReadAttributeAsEmus("z", 0, mComplianceInfo);
            return flatText;
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
        private readonly NrxXmlReader mXmlReader;
        private readonly OoxmlComplianceInfo mComplianceInfo;
    }
}
