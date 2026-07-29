// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Builds locked canvases or groups shapes from XML.
    /// </summary>
    internal class DmlCompositeNodeReader : DmlReaderBase
    {
        private DmlCompositeNodeReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Reads locked canvases and word processing canvases from XML.
        /// </summary>
        /// <remarks> 20.3.2.1 lockedCanvas (Locked Canvas Container)
        /// The locked canvas element acts as a container for more advanced drawing objects.
        /// The notion of a locked canvas comes from the fact that the generating application
        /// opening the file cannot create this object and can thus not perform edits either.
        /// Thus the drawing object is locked from all UI adjustments that would normally take place.
        ///
        /// 2.3.31 wpc (Wordprocessing canvas) element that specifies a drawing canvas in WordprocessingML.
        /// </remarks>
        internal static GroupShape ReadLockedCanvas(DocxDocumentReaderBase reader)
        {
            string tagName = reader.XmlReader.LocalName;
            if ((tagName != "lockedCanvas") && (tagName != "wpc"))
                return null;

            return ReadGroupShape(reader, tagName);
        }

        /// <summary>
        /// Reads group shapes from XML.
        /// </summary>
        /// <remarks>
        /// 20.1.2.2.20 grpSp (Group shape)
        /// This element specifies a group shape that represents many shapes grouped together.
        /// This shape is to be treated just as if it were a regular shape but instead of being
        /// described by a single geometry it is made up of all the shape geometries encompassed
        /// within it. Within a group shape each of the shapes that make up the group are specified
        /// just as they normally would. The idea behind grouping elements however is that a single
        /// transform can apply to many shapes at the same time.
        ///
        /// 2.3.30 wgp (Wordprocessing Group Shape) element that specifies a group in WordprocessingML.
        /// </remarks>
        internal static GroupShape ReadGroup(DocxDocumentReaderBase reader)
        {
            string tagName = reader.XmlReader.LocalName;
            if ((tagName != "grpSp") && (tagName != "wgp") && (tagName != "spTree"))
                return null;

            GroupShape dml = ReadGroupShape(reader, tagName);

            // Set IsDrawingExtensions because there is ISO 29500 extension Wordprocessing group shape.
            if (dml.DmlNode.DmlNodeType == DmlNodeType.WordprocessingGroupShape)
                reader.ComplianceInfo.IsDrawingExtensions = true;

            return dml;
        }

        private void ReadCompositeNode(GroupShape dml)
        {
            string tagName = XmlReader.LocalName;
            DmlNodeType dmlNodeType = dml.DmlNode.DmlNodeType;

            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "grpSpPr":
                        DmlNodePropertiesReader.ReadVisualGroupShapeProperties(mDocumentReader, (DmlCompositeNode)dml.DmlNode);
                        break;
                    case "sp":
                    case "cxnSp":
                    case "wsp":
                    {
                        Shape childDml = DmlShapeReader.Read(mDocumentReader);
                        mDocumentReader.AddToDmlTextboxesMap(childDml);
                        break;
                    }
                    case "grpSp":
                    case "wgp":
                        ReadGroup(mDocumentReader);
                        break;
                    case "pic":
                        DmlPictureReader.Read(mDocumentReader);
                        break;
                    case "txSp":
                    {
                        if (dml.DmlNode is DmlGroupShape)
                            ((DmlGroupShape)dml.DmlNode).TextShape = DmlTextShapeReader.Read(mDocumentReader);
                        else
                            WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                    case "nvGrpSpPr":
                    {
                        if (dml.DmlNode.NonVisualPr == null)
                            dml.DmlNode.NonVisualPr = new DmlNvPrGroupShape();

                        DmlNodePropertiesReader.ReadNonVisualProperties(dml.DmlNode.NonVisualPr, mDocumentReader);
                        break;
                    }
                    case "cNvPr":
                        ReadNonVisualCanvasProperties(dml.DmlNode);
                        break;
                    case "cNvFrPr": // 20.4.2.25 cNvFrPr (Non-Visual Graphic Frame Drawing Properties).
                    case "cNvGrpSpPr":
                        ReadNonVisualDrawingProperties(dml.DmlNode);
                        break;
                    case "bg":
                    {
                        if (dmlNodeType == DmlNodeType.WordprocessingCanvas)
                            ((DmlLockedCanvas)dml.DmlNode).Fill = ReadCanvasBackground();
                        else
                            WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                    case "whole":
                    {
                        if (dmlNodeType == DmlNodeType.WordprocessingCanvas)
                            ((DmlLockedCanvas)dml.DmlNode).Outline = ReadCanvasOutline();
                        else
                            WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                    case "graphicFrame":
                        ReadGroupShape(mDocumentReader, XmlReader.LocalName);
                        break;
                    case "contentPart": // ISO 29500 20.4.2.29 contentPart (Content Part)
                        DmlContentPartReader.Read(mDocumentReader);
                        break;
                    case "xfrm":
                    {
                        // According to schema definition of the graphical object container,
                        // this element has transformation element of the type “CT_Transform2D” (not “CT_GroupTransform2D”).
                        if (dmlNodeType == DmlNodeType.GraphicFrame)
                            dml.DmlNode.Transform = DmlTransformReader.ReadTransform(XmlReader, mDocumentReader.ComplianceInfo);
                        else
                            WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                    case "graphic":
                    {
                        if (dmlNodeType == DmlNodeType.GraphicFrame)
                            DmlGraphicsReader.Read(mDocumentReader);
                        else
                            WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                    case "extLst":
                    {
                        if (dmlNodeType == DmlNodeType.GraphicFrame)
                            ((DmlGroupShape)dml.DmlNode).Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        else
                            WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                    }
                    case "nvGraphicFramePr":
                    {
                        Debug.Assert(dmlNodeType == DmlNodeType.GraphicFrame);
                        dml.DmlNode.NonVisualPr = new DmlNvPrGraphicFrame();
                        DmlNodePropertiesReader.ReadNonVisualProperties(dml.DmlNode.NonVisualPr, mDocumentReader);
                        break;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads non-visual canvas properties.
        /// </summary>
        /// <param name="dml">DrawingML shape.</param>
        private void ReadNonVisualCanvasProperties(DmlNode dml)
        {
            Debug.Assert(dml != null);

            CheckNonVisualProperties(dml);
            dml.NonVisualPr.NvDrawingProperties = DmlNodePropertiesReader.ReadNonVisualDrawingProperties(mDocumentReader);
        }

        /// <summary>
        /// Reads non-Visual drawing properties.
        /// </summary>
        /// <param name="dml">DrawingML shape.</param>
        private void ReadNonVisualDrawingProperties(DmlNode dml)
        {
            Debug.Assert(dml != null);

            CheckNonVisualProperties(dml);
            dml.NonVisualPr.CNvProperties = DmlNodePropertiesReader.ReadNonVisualConnectorProperties(mDocumentReader);
        }

        /// <summary>
        /// Initializes non-visual properties of the DrawingML shape.
        /// </summary>
        /// <param name="dml">DrawingML shape.</param>
        private static void CheckNonVisualProperties(DmlNode dml)
        {
            Debug.Assert(dml != null);

            bool isGraphicFrame = (dml.DmlNodeType == DmlNodeType.GraphicFrame);

            if (dml.NonVisualPr == null)
                dml.NonVisualPr = isGraphicFrame ? new DmlNvPrGraphicFrame() : (DmlNvPrBase)new DmlNvPrGroupShape();
        }

        /// <summary>
        /// Reads group shape data of the DrawingML shape.
        /// </summary>
        /// <param name="reader">Document reader.</param>
        /// <param name="tagName">Name of the tag.</param>
        private static GroupShape ReadGroupShape(DocxDocumentReaderBase reader, string tagName)
        {
            GroupShape dml = new GroupShape(reader.Document, ShapeMarkupLanguage.Dml);

            DmlNodeType dmlType = GetShapeType(tagName);
            bool isCanvas = (dmlType == DmlNodeType.WordprocessingCanvas) ||
                            (dmlType == DmlNodeType.LockedCanvas);

            reader.AddAndPushContainer(dml);
            dml.DmlNode = isCanvas ? new DmlLockedCanvas(dmlType) : (DmlNode)new DmlGroupShape(dmlType);
            DmlCompositeNodeReader compositeNodeReader = new DmlCompositeNodeReader(reader);
            compositeNodeReader.ReadCompositeNode(dml);
            reader.PopContainer(NodeType.GroupShape);

            return dml;
        }

        private DmlOutline ReadCanvasOutline()
        {
            DmlOutline outline = null;
            while (XmlReader.ReadChild("whole"))
            {
                switch (XmlReader.LocalName)
                {
                    case "ln":
                        outline = DmlOutlineReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return outline;
        }

        private DmlFill ReadCanvasBackground()
        {
            DmlFill fill = null;
            while (XmlReader.ReadChild("bg"))
            {
                switch (XmlReader.LocalName)
                {
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "noFill":
                    case "pattFill":
                    case "solidFill":
                        fill = DmlFillReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
            return fill;
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
