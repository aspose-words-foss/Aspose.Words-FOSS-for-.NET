// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlDiagramWriter
    {
        internal static void Write(DmlNode dml, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement("dgm:relIds");
            builder.WriteAttribute("xmlns:dgm", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, isIsoStrict));
            builder.WriteAttribute("xmlns:r", 
                DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));

            DmlDiagram diagram = (DmlDiagram)dml;

            string dmId = WriteDiagramData(diagram.Data, writer);
            string loId = WriteDiagramLayout(diagram.Layout, writer);
            string qsId = WriteDiagramStyle(diagram.StyleDefinition, writer);
            string csId = WriteDiagramColors(diagram.ColorTransformDefinition, writer);

            builder.WriteAttribute("r:dm", dmId);
            builder.WriteAttribute("r:lo", loId);
            builder.WriteAttribute("r:qs", qsId);
            builder.WriteAttribute("r:cs", csId);

            builder.EndElement("dgm:relIds");
        }

        private static string WriteDiagramData(DmlDiagramDataModel data, DocxDocumentWriterBase writer)
        {
            string drawingId = WriteDiagramDrawing(data.Drawing, writer);

            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string relId;
            DocxBuilder chartBuilder = CreateChildPart(
                writer,
                DocxRelationshipTypes.GetType(DocxRelationshipType.DiagramData, isIsoStrict),
                DocxContentType.DiagramData,
                "/word/diagrams/data{0}.xml",
                out relId);

            writer.PushBuilder(chartBuilder);
            DmlDiagramDataWriter.Write(data, drawingId, writer);
            writer.PopBuilder();

            return relId;
        }

        private static string WriteDiagramDrawing(ShapeBase drawing, DocxDocumentWriterBase writer)
        {
            if (drawing == null)
                return null;

            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string relId;
            DocxBuilder partBuilder = CreateChildPart(
                writer,
                DocxRelationshipTypes.GetType(DocxRelationshipType.DiagramDrawing, isIsoStrict),
                DocxContentType.DiagramDrawing,
                "/word/diagrams/drawing{0}.xml",
                out relId);

            writer.PushBuilder(partBuilder);
            DmlDiagramDrawingWriter.Write(drawing, writer);
            writer.PopBuilder();

            return relId;
        }

        private static string WriteDiagramLayout(DmlDiagramLayout layout, DocxDocumentWriterBase writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string relId;
            DocxBuilder partBuilder = CreateChildPart(
                writer,
                DocxRelationshipTypes.GetType(DocxRelationshipType.DiagramLayout, isIsoStrict),
                DocxContentType.DiagramLayout,
                "/word/diagrams/layout{0}.xml",
                out relId);

            writer.PushBuilder(partBuilder);
            DmlDiagramLayoutWriter.Write(layout, writer);
            writer.PopBuilder();

            return relId;
        }

        private static string WriteDiagramStyle(DmlDiagramStyleDefinition styleDefinition, DocxDocumentWriterBase writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string relId;
            DocxBuilder partBuilder = CreateChildPart(
                writer,
                DocxRelationshipTypes.GetType(DocxRelationshipType.DiagramStyle, isIsoStrict),
                DocxContentType.DiagramStyle,
                "/word/diagrams/quickStyle{0}.xml",
                out relId);

            writer.PushBuilder(partBuilder);
            DmlDiagramStyleWriter.Write(styleDefinition, writer);
            writer.PopBuilder();

            return relId;
        }

        private static string WriteDiagramColors(DmlDiagramColorTransform colorTransformDefinition, DocxDocumentWriterBase writer)
        {
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string relId;
            DocxBuilder partBuilder = CreateChildPart(
                writer,
                DocxRelationshipTypes.GetType(DocxRelationshipType.DiagramColors, isIsoStrict),
                DocxContentType.DiagramColors,
                "/word/diagrams/colors{0}.xml",
                out relId);

            writer.PushBuilder(partBuilder);
            DmlDiagramColorWriter.Write(colorTransformDefinition, writer);
            writer.PopBuilder();

            return relId;
        }

        private static DocxBuilder CreateChildPart(
            DocxDocumentWriterBase writer,
            string relType,
            string contentType,
            string partNameTemplate,
            out string relId)
        {
            int partNumber = writer.GetNextEmbeddedPartNumber(relType);
            string partName = string.Format(partNameTemplate, partNumber);

            return writer.CreateChildPartAndBuilder(writer.CurrentBuilder.Part, partName, contentType, relType, out relId);
        }
    }
}
