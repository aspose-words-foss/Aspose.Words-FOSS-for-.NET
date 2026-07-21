// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramDrawingWriter
    {
        internal static void Write(ShapeBase drawing, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartDocument("dsp:drawing");
            builder.WriteAttribute("xmlns:dgm", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, isIsoStrict));
            builder.WriteAttribute("xmlns:dsp", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram2008, isIsoStrict));
            builder.WriteAttribute("xmlns:a", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));

            WriteGroupShape("dsp:spTree", drawing, writer);

            builder.EndDocument();
        }

        private static void WriteShape(DmlShape shape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("dsp:sp");
            builder.WriteAttribute("modelId", DmlDiagramEnum.ModelIdToDml(shape.ModelId));

            DmlNonVisualPropertiesWriter.WriteNvPr("dsp", shape, writer);
            DmlShapePropertiesWriter.Write("dsp", shape, writer);
            DmlShapeStyleWriter.Write("dsp", shape.Style, writer);
            if (shape.TextShape != null)
            {
                DmlTextShapeWriter.WriteDmlShapeTextBody("dsp:txBody", shape.TextShape.TextBody, writer);
                // WORDSNET-13947 Write proper txXfrm.
                DmlXfrmWriter.Write("dsp:txXfrm", shape.TextShape.Transform, writer);
            }

            builder.EndElement("dsp:sp");
        }

        private static void WriteGroupShape(string tagName, ShapeBase group, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(tagName);

            DmlNonVisualPropertiesWriter.WriteNvPr("dsp", group.DmlNode, writer);
            DmlGroupShapeWriter.WriteGroupShapeProperties("dsp", (DmlCompositeNode)group.DmlNode, writer);

            foreach (Shape node in group.GetChildNodes(NodeType.Any, false))
            {
                // According to the scheme only Shape and Group Shape can be children of dsp:spTree and dsp:grpSp.
                // Do not process other nodes.
                switch (node.DmlNode.DmlNodeType)
                {
                    case DmlNodeType.Shape:
                        WriteShape((DmlShape)node.DmlNode, writer);
                        break;
                    case DmlNodeType.GroupShape:
                        WriteGroupShape("dsp:grpSp", group, writer);
                        break;
                    default:
                        break;
                }
            }

            builder.EndElement(tagName);
        }
    }
}
