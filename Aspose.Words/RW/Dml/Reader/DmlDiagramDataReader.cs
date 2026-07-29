// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class used to read DML diagram data model.
    /// </summary>
    internal class DmlDiagramDataReader : DmlReaderBase
    {
        private DmlDiagramDataReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        internal static DmlDiagramDataModel Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            if (xmlReader.LocalName != "dataModel")
                return null;

            DmlDiagramDataReader diagramDataReader = new DmlDiagramDataReader(reader);

            DmlDiagramDataModel data = new DmlDiagramDataModel();
            while (xmlReader.ReadChild("dataModel"))
            {
                switch (xmlReader.LocalName)
                {
                    case "ptLst":
                        data.PointList = diagramDataReader.ReadPointList();
                        break;
                    case "cxnLst":
                        data.ConnectionList = diagramDataReader.ReadConnectionList();
                        break;
                    case "whole":
                        data.WholeFormatting = diagramDataReader.ReadWhole();
                        break;
                    case "bg":
                        data.Background = diagramDataReader.ReadBackground();
                        break;
                    case "extLst":
                        data.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return data;
        }

        private DmlDiagramBackground ReadBackground()
        {
            DmlDiagramBackground result = new DmlDiagramBackground();
            while (XmlReader.ReadChild("bg"))
            {
                switch (XmlReader.LocalName)
                {
                    case "noFill":
                    case "solidFill":
                    case "gradFill":
                    case "blipFill":
                    case "pattFill":
                    case "grpFill":
                        result.Fill = DmlFillReader.Read(mDocumentReader);
                        break;
                    case "effectLst":
                        result.Effects = DmlShapeEffectReader.ReadEffects(mDocumentReader, false, false);
                        break;
                    case "effectDag":
                        result.Effects = DmlShapeEffectReader.ReadEffects(mDocumentReader, false, true);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return result;
        }

        private DmlDiagramWholeFormatting ReadWhole()
        {
            DmlDiagramWholeFormatting result = new DmlDiagramWholeFormatting();
            while (XmlReader.ReadChild("whole"))
            {
                switch (XmlReader.LocalName)
                {
                    case "ln":
                        result.Outline = DmlOutlineReader.Read(mDocumentReader);
                        break;
                    case "effectLst":
                        result.Effects = DmlShapeEffectReader.ReadEffects(mDocumentReader, false, false);
                        break;
                    case "effectDag":
                        result.Effects = DmlShapeEffectReader.ReadEffects(mDocumentReader, false, true);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return result;
        }

        private DmlDiagramConnection[] ReadConnectionList()
        {
            List<DmlDiagramConnection> result = new List<DmlDiagramConnection>();
            while (XmlReader.ReadChild("cxnLst"))
            {
                switch (XmlReader.LocalName)
                {
                    case "cxn":
                        result.Add(ReadConnection());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return result.ToArray();
        }

        private DmlDiagramConnection ReadConnection()
        {
            DmlDiagramConnection connection = new DmlDiagramConnection();
            connection.ModelId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("modelId", ""));
            connection.Type = DmlDiagramEnum.DmlToConnectionType(XmlReader.ReadAttribute("type", "parOf"));
            connection.SourceId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("srcId", ""));
            connection.DestinationId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("destId", ""));
            connection.SourceOrder = XmlReader.ReadIntAttribute("srcOrd", 0);
            connection.DestinationOrder = XmlReader.ReadIntAttribute("destOrd", 0);
            connection.ParentTransitionId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("parTransId", "0"));
            connection.SiblingTransitionId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("sibTransId", "0"));
            connection.PresentationId = XmlReader.ReadAttribute("presId", "");
            return connection;
        }

        private DmlDiagramPoint[] ReadPointList()
        {
            List<DmlDiagramPoint> result = new List<DmlDiagramPoint>();
            while (XmlReader.ReadChild("ptLst"))
            {
                switch (XmlReader.LocalName)
                {
                    case "pt":
                        result.Add(ReadPoint());
                        break;
                    case "AlternateContent":
                        result.Add(ReadPointAlternateContent());
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return result.ToArray();
        }

        private DmlDiagramPoint ReadPointAlternateContent()
        {
            DmlDiagramPoint point = new DmlDiagramPoint();

            // Preserve all attributes of AlternateContent to restore them on write.
            while (XmlReader.MoveToNextAttribute(false))
            {
                string attr = StringUtil.HasChars(XmlReader.Prefix)
                    ? string.Format("{0}:{1}", XmlReader.Prefix, XmlReader.LocalName)
                    : XmlReader.LocalName;
                point.AlternateContentAttributes.Add(attr, XmlReader.Value);
            }

            XmlReader.MoveToElement();
            while (XmlReader.ReadChild("AlternateContent"))
            {
                switch (XmlReader.LocalName)
                {
                    case "Choice":
                        ReadPointChoice(point);
                        break;
                    case "Fallback":
                        ReadPointFallback(point);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            return point;
        }

        private void ReadPointFallback(DmlDiagramPoint point)
        {
            point.AlternateContentFallbackPoint = new DmlDiagramPoint();
            while (XmlReader.ReadChild("Fallback"))
            {
                switch (XmlReader.LocalName)
                {
                    case "pt":
                        ReadPointCore(point.AlternateContentFallbackPoint);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private void ReadPointChoice(DmlDiagramPoint point)
        {
            while (XmlReader.MoveToNextAttribute(false))
            {
                string attr = StringUtil.HasChars(XmlReader.Prefix)
                    ? string.Format("{0}:{1}", XmlReader.Prefix, XmlReader.LocalName)
                    : XmlReader.LocalName;
                point.AlternateContentChoiceAttributes.Add(attr, XmlReader.Value);
            }

            XmlReader.MoveToElement();
            while (XmlReader.ReadChild("Choice"))
            {
                switch (XmlReader.LocalName)
                {
                    case "pt":
                        ReadPointCore(point);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private DmlDiagramPoint ReadPoint()
        {
            DmlDiagramPoint point = new DmlDiagramPoint();
            ReadPointCore(point);
            return point;
        }

        private void ReadPointCore(DmlDiagramPoint point)
        {
            point.ModelId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("modelId", ""));
            point.Type = DmlDiagramEnum.DmlToPointType(XmlReader.ReadAttribute("type", "node"));
            point.ConnectionId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("cxnId", "0"));

            while (XmlReader.ReadChild("pt"))
            {
                switch (XmlReader.LocalName)
                {
                    case "spPr":
                        point.ShapeProperties = new DmlShapeProperties(mDocumentReader.Document);
                        DmlNodePropertiesReader.ReadShapeProperties(mDocumentReader, point.ShapeProperties);
                        break;
                    case "t":
                        point.TextBody = new DmlTextBody();
                        DmlTextShapeReader.ReadTextBody(mDocumentReader, point.TextBody, "t");
                        break;
                    case "prSet":
                        point.PropertySet = ReadPropertySet();
                        break;
                    case "extLst":
                        point.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        private DmlPropertySet ReadPropertySet()
        {
            DmlPropertySet prSet = new DmlPropertySet();

            while (XmlReader.MoveToNextAttribute())
            {
                switch (XmlReader.LocalName)
                {
                    case "presAssocID":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.PresAssocId, DmlDiagramEnum.DmlToModelId(XmlReader.Value));
                        break;
                    case "presName":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.PresName, XmlReader.Value);
                        break;
                    case "presStyleLbl":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.PresStyleLbl, XmlReader.Value);
                        break;
                    case "presStyleIdx":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.PresStyleIdx, XmlReader.ValueAsInt);
                        break;
                    case "presStyleCnt":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.PresStyleCnt, XmlReader.ValueAsInt);
                        break;
                    case "loTypeId":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.LoTypeId, XmlReader.Value);
                        break;
                    case "loCatId":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.LoCatId, XmlReader.Value);
                        break;
                    case "qsTypeId":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.QsTypeId, XmlReader.Value);
                        break;
                    case "qsCatId":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.QsCatId, XmlReader.Value);
                        break;
                    case "csTypeId":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CsTypeId, XmlReader.Value);
                        break;
                    case "csCatId":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CsCatId, XmlReader.Value);
                        break;
                    case "coherent3DOff":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.Coherent3DOff, XmlReader.ValueAsBool);
                        break;
                    case "phldrT":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.PhldrT, XmlReader.Value);
                        break;
                    case "phldr":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.Phldr, XmlReader.ValueAsBool);
                        break;
                    case "custAng":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustAng, XmlReader.ValueAsInt);
                        break;
                    case "custFlipVert":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustFlipVert, XmlReader.ValueAsBool);
                        break;
                    case "custFlipHor":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustFlipHor, XmlReader.ValueAsBool);
                        break;
                    case "custSzX":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustSzX, XmlReader.ValueAsInt);
                        break;
                    case "custSzY":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustSzY, XmlReader.ValueAsInt);
                        break;
                    case "custScaleX":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustScaleX, FromPercent(XmlReader.Value));
                        break;
                    case "custScaleY":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustScaleY, FromPercent(XmlReader.Value));
                        break;
                    case "custT":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustT, XmlReader.ValueAsBool);
                        break;
                    case "custLinFactX":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustLinFactX, FromPercent(XmlReader.Value));
                        break;
                    case "custLinFactY":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustLinFactY, FromPercent(XmlReader.Value));
                        break;
                    case "custLinFactNeighborX":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustLinFactNeighborX, FromPercent(XmlReader.Value));
                        break;
                    case "custLinFactNeighborY":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustLinFactNeighborY, FromPercent(XmlReader.Value));
                        break;
                    case "custRadScaleRad":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustRadScaleRad, FromPercent(XmlReader.Value));
                        break;
                    case "custRadScaleInc":
                        prSet.PrSet.SetProperty(DmlPropertySetAttr.CustRadScaleInc, FromPercent(XmlReader.Value));
                        break;
                    default:
                        WarnUnexpected(XmlReader);
                        break;
                }
            }

            while (XmlReader.ReadChild("prSet"))
            {
                switch (XmlReader.LocalName)
                {
                    case "presLayoutVars":
                        prSet.LayoutVariablePropertySet = DmlDiagramComplexTypesReader.ReadLayoutVariableList(XmlReader);
                        break;
                    case "style":
                        prSet.Style = DmlShapeStyleReader.Read(XmlReader, mDocumentReader.ComplianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return prSet;
        }

        /// <summary>
        /// Converts string percent value in 1000th of percent, if no percent sign, or in whole percents with percent
        /// sign to integer value in 1000th of percent.
        /// </summary>
        private int FromPercent(string value)
        {
            double asFraction = DmlPercentageUtil.FromPercentOrDmlPercent(value, mDocumentReader.ComplianceInfo);
            return MathUtil.DoubleToInt(DmlPercentageUtil.ToDmlPercent(asFraction));
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
