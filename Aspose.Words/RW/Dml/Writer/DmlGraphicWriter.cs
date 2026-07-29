// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlGraphicWriter
    {
        internal static void WriteStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            if (IsGraphicWrappedShape(dml))
            {
                NrxXmlBuilder builder = writer.CurrentBuilder;
                bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

                builder.StartElement("a:graphic");
                builder.WriteAttribute("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));

                builder.StartElement("a:graphicData");
                builder.WriteAttribute("uri", DmlNamespaceUtil.GetNamespace(dml.DmlNode.DmlNodeType, isIsoStrict));
            }

            WriteDmlNodeStart(dml, writer);
        }

        internal static void WriteEnd(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            WriteDmlNodeEnd(dml, writer);

            if (IsGraphicWrappedShape(dml))
            {
                writer.CurrentBuilder.EndElement("a:graphicData");
                writer.CurrentBuilder.EndElement("a:graphic");
            }
        }

        /// <summary>
        /// Returns "true" when shape has to be wrapped into "graphicData" element.
        /// </summary>
        /// <param name="dml">Shape for check.</param>
        /// <returns>True when shape has to be wrapped.</returns>
        private static bool IsGraphicWrappedShape(ShapeBase dml)
        {
            Debug.Assert(dml != null);

            if (dml.IsTopLevel)
                return true;

            // Shape has to be wrapped into "graphicData" element when it is a child item of the graphic frame.
            Node parent = dml.ParentNode;
            return (parent.NodeType == NodeType.GroupShape) &&
                   (((ShapeBase)parent).DmlNode != null) &&
                   (((ShapeBase)parent).DmlNode.DmlNodeType == DmlNodeType.GraphicFrame);
        }

        private static void WriteDmlNodeStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            switch (dml.DmlNode.DmlNodeType)
            {
                case DmlNodeType.LockedCanvas:
                case DmlNodeType.WordprocessingCanvas:
                    DmlCanvasWriter.WriteStart(dml, writer);
                    break;
                case DmlNodeType.Picture:
                    DmlPictureWriter.Write(dml, writer);
                    break;
                case DmlNodeType.Chart:
                case DmlNodeType.ChartEx:
                    DmlChartWriter.Write(dml, writer);
                    break;
                case DmlNodeType.Diagram:
                    DmlDiagramWriter.Write(dml.DmlNode, writer);
                    break;
                case DmlNodeType.WordprocessingShape:
                case DmlNodeType.Shape:
                case DmlNodeType.ConnectorShape:
                    DmlShapeWriter.WriteStart(dml, writer);
                    break;   
                case DmlNodeType.WordprocessingGroupShape:
                case DmlNodeType.GroupShape:
                    DmlGroupShapeWriter.WriteStart(dml, writer);
                    break;
                case DmlNodeType.ContentPart:
                    DmlContentPartWriter.Write(dml, writer);
                    break;
                case DmlNodeType.GraphicFrame:
                    // According to spec can occur in group shape or canvas.
                    DmlGraphicFrameWriter.WriteStart(dml, writer);
                    break;
                default:
                    throw new ArgumentException("Unexpected DrawingML type.");
            }
        }

        private static void WriteDmlNodeEnd(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            switch (dml.DmlNode.DmlNodeType)
            {
                case DmlNodeType.LockedCanvas:
                case DmlNodeType.WordprocessingCanvas:
                    DmlCanvasWriter.WriteEnd(writer);
                    break;
                case DmlNodeType.Picture:
                case DmlNodeType.Chart:
                case DmlNodeType.ChartEx:
                case DmlNodeType.Diagram:
                case DmlNodeType.ContentPart:
                    // Do nothing because whole objects are written using corresponding Write method.
                    break;
                case DmlNodeType.WordprocessingShape:
                case DmlNodeType.Shape:
                case DmlNodeType.ConnectorShape:
                    DmlShapeWriter.WriteEnd(dml, writer);
                    break;               
                case DmlNodeType.WordprocessingGroupShape:
                case DmlNodeType.GroupShape:
                    DmlGroupShapeWriter.WriteEnd(writer);
                    break;
                case DmlNodeType.GraphicFrame:
                    DmlGraphicFrameWriter.WriteEnd(writer, dml);
                    break;
                default:
                    throw new ArgumentException("Unexpected DrawingML type.");
            }
        }
    }
}
