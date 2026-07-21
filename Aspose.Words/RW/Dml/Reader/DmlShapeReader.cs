// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Represents a class reading DrawingML shape by xml.
    /// </summary>
    internal class DmlShapeReader : DmlReaderBase
    {
        private DmlShapeReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        internal static Shape Read(DocxDocumentReaderBase reader)
        {
            DmlShapeReader shapeReader = new DmlShapeReader(reader);
            return shapeReader.ReadCore();
        }

        private Shape ReadCore()
        {
            try
            {
                switch (XmlReader.LocalName)
                {
                    case "sp":
                    case "wsp":
                    case "cxnSp":
                        ReadShape();
                        break;
                    default:
                        return null;
                }

                return mShape;
            }
            catch (Exception e)
            {
                // We are failed to read shape from xml.
                // Lets warn and rollback made changes, removing partially read shape.
                WarnShapeIgnored(XmlReader, e.Message);
                CompositeNode curContainer = mDocumentReader.CurContainer;
                mDocumentReader.PopContainer(NodeType.Shape);
                curContainer.Remove();

                return null;
            }
        }

        /// <summary>
        /// Reads the shape.
        /// </summary>
        /// <remarks>
        /// 20.1.2.2.33 sp (Shape)
        /// This element specifies the existence of a single shape. A shape can either be a preset or a
        /// custom geometry, defined using the DrawingML framework. In addition to a geometry each shape
        /// can have both visual and non-visual properties attached. Text and corresponding styling
        /// information can also be attached to a shape. This shape is specified along with all other
        /// shapes within either the shape tree or group shape elements.
        /// </remarks>
        private void ReadShape()
        {
            string shapeTag = XmlReader.LocalName;
            mShape = new Shape(mDocumentReader.Document, ShapeMarkupLanguage.Dml);
            mDocumentReader.AddAndPushContainer(mShape);
            DmlShape dmlShapeNode = new DmlShape(GetShapeType(shapeTag));
            mShape.DmlNode = dmlShapeNode;

            // Set IsDrawingExtensions because there is ISO 29500 extension Wordprocessing shape.
            if (dmlShapeNode.DmlNodeType == DmlNodeType.WordprocessingShape)
                ComplianceInfo.IsDrawingExtensions = true;

            dmlShapeNode.NormalEastAsianFlow = XmlReader.ReadBoolAttribute("normalEastAsianFlow", false);

            // Read model Id for diagram drawing shapes.
            dmlShapeNode.ModelId = DmlDiagramEnum.DmlToModelId(XmlReader.ReadAttribute("modelId", ""));
            XmlReader.MoveToElement();

            while (XmlReader.ReadChild(shapeTag))
            {
                switch (XmlReader.LocalName)
                {
                    case "spPr":
                        DmlNodePropertiesReader.ReadShapeProperties(mDocumentReader, dmlShapeNode);
                        break;
                    case "style":
                        dmlShapeNode.Style = DmlShapeStyleReader.Read(XmlReader, ComplianceInfo);
                        break;
                    case "txbx":
                        ReadTextbox();
                        break;
                    case "linkedTxbx":
                        {
                            mShape.LinkedTextboxId = XmlReader.ReadIntAttribute("id", 0);
                            mShape.LinkedTextboxSeq = XmlReader.ReadIntAttribute("seq", 0);
                            break;
                        }
                    case "txSp":
                        dmlShapeNode.TextShape = DmlTextShapeReader.Read(mDocumentReader);
                        break;
                    case "txBody":
                        {
                            // txBody element occur directly in sp element in chart's drawing,
                            // so simply generate text shape to handle it.
                            if (dmlShapeNode.TextShape == null)
                                dmlShapeNode.TextShape = new DmlTextShape();

                            DmlTextShapeReader.ReadTextBody(mDocumentReader, dmlShapeNode.TextShape.TextBody, "txBody");
                            break;
                        }
                    case "bodyPr":
                        {
                            // bodyPr element occur directly in wsp element in chart's drawing,
                            // so simply generate text shape to handle it.
                            if (dmlShapeNode.TextShape == null)
                                dmlShapeNode.TextShape = new DmlTextShape();

                            DmlTextShapeReader.ReadTextBodyProperties(mDocumentReader,
                                dmlShapeNode.TextShape.TextBody.Properties, ComplianceInfo);
                            break;
                        }
                    case "txXfrm":
                        {
                            // I did not found this element in the specification,
                            // but it seems it defines target rectangle of the text shape.
                            if (dmlShapeNode.TextShape == null)
                                dmlShapeNode.TextShape = new DmlTextShape();

                            dmlShapeNode.TextShape.Transform =
                                DmlTransformReader.ReadTransform(XmlReader, ComplianceInfo);
                            break;
                        }
                    case "nvSpPr":
                        {
                            if (dmlShapeNode.NonVisualPr == null)
                                dmlShapeNode.NonVisualPr = new DmlNvPrShape();
                            DmlNodePropertiesReader.ReadNonVisualProperties(dmlShapeNode.NonVisualPr, mDocumentReader);
                            break;
                        }
                    case "nvCxnSpPr":
                        {
                            if (dmlShapeNode.NonVisualPr == null)
                                dmlShapeNode.NonVisualPr = new DmlNvPrConnectionShape();
                            DmlNodePropertiesReader.ReadNonVisualProperties(dmlShapeNode.NonVisualPr, mDocumentReader);
                            break;
                        }
                    case "cNvPr":
                        {
                            if (dmlShapeNode.NonVisualPr == null)
                            {
                                if (dmlShapeNode.DmlNodeType == DmlNodeType.ConnectorShape)
                                    dmlShapeNode.NonVisualPr = new DmlNvPrConnectionShape();
                                else
                                    dmlShapeNode.NonVisualPr = new DmlNvPrShape();
                            }

                            dmlShapeNode.NonVisualPr.NvDrawingProperties =
                                DmlNodePropertiesReader.ReadNonVisualDrawingProperties(mDocumentReader);
                            break;
                        }
                    case "cNvSpPr":
                        {
                            if (dmlShapeNode.NonVisualPr == null)
                                dmlShapeNode.NonVisualPr = new DmlNvPrShape();
                            dmlShapeNode.NonVisualPr.CNvProperties =
                                DmlNodePropertiesReader.ReadNonVisualConnectorProperties(mDocumentReader);
                            break;
                        }
                    case "cNvCnPr":
                        {
                            if (dmlShapeNode.NonVisualPr == null)
                                dmlShapeNode.NonVisualPr = new DmlNvPrConnectionShape();
                            dmlShapeNode.NonVisualPr.CNvProperties =
                                DmlNodePropertiesReader.ReadNonVisualConnectorProperties(mDocumentReader);
                            break;
                        }
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            mDocumentReader.PopContainer(NodeType.Shape);
        }

        /// <summary>
        /// Reads 'txbx' element that specifies the textual contents of the shape if the shape
        /// is the first in the series of shapes for the same text box story.
        /// </summary>
        private void ReadTextbox()
        {
            mShape.TextboxId = XmlReader.ReadIntAttribute("id", 0);
            while (XmlReader.ReadChild("txbx"))
            {
                switch (XmlReader.LocalName)
                {
                    case "txbxContent": // w:txbxContent
                    {
                        DocxReaderFactory.StoryReader.Read(mDocumentReader, mDocumentReader.CurContainer);
                        break;
                    }
                    default:
                    {
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                }
            }
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private OoxmlComplianceInfo ComplianceInfo
        {
            get { return mDocumentReader.ComplianceInfo; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
        private Shape mShape;
    }
}
