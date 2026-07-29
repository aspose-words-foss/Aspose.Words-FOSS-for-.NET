// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Class used to write DrawingML canvas.
    /// </summary>
    internal static class DmlCanvasWriter
    {
        internal static void WriteStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlNode dmlNode = dml.DmlNode;
            DmlNodeType canvasType = dmlNode.DmlNodeType;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            if (canvasType != DmlNodeType.LockedCanvas && canvasType != DmlNodeType.WordprocessingCanvas)
                throw new ArgumentException("Unexpected canvas shape type.");

            // Use the same writer for writing legacy lockedCanvas and wpc.
            // Determine root tag name by group shape type.        
            string prefix = isIsoStrict ? "wp" : "wpc";
            string rootTagName = isIsoStrict ? "wp:wpc" : "wpc:wpc";

            if (canvasType == DmlNodeType.LockedCanvas)
            {
                prefix = "a";
                rootTagName = "lc:lockedCanvas";
            }
                       
            DmlLockedCanvas canvas = (DmlLockedCanvas)dmlNode;
            NrxXmlBuilder builder = writer.Builder;            

            builder.StartElement(rootTagName);

            if (canvasType == DmlNodeType.LockedCanvas)
                builder.WriteAttributeString("xmlns:lc", 
                    DmlNamespaceUtil.GetNamespace(DmlNodeType.LockedCanvas, isIsoStrict));

            if (canvasType == DmlNodeType.WordprocessingCanvas)
            {
                WriteCanvasBackground(canvas, writer);
                WriteCanvasOutline(canvas, writer);
            }

            DmlNonVisualPropertiesWriter.WriteNvPr(prefix, canvas, writer);

            if (canvasType != DmlNodeType.WordprocessingCanvas)
                DmlGroupShapeWriter.WriteGroupShapeProperties(prefix, canvas, writer);
        }

        private static void WriteCanvasBackground(DmlLockedCanvas canvas, DocxDocumentWriterBase writer)
        {
            if (canvas.Fill.DmlFillType == DmlFillType.StyleFill)
                return;

            NrxXmlBuilder builder = writer.Builder;
            string tagName = (writer.Compliance == OoxmlComplianceCore.IsoStrict) ? "wp:bg" : "wpc:bg";

            builder.StartElement(tagName);
            DmlFillWriter.Write(canvas.Fill, writer, false);
            builder.EndElement(tagName);
        }

        private static void WriteCanvasOutline(DmlLockedCanvas canvas, DocxDocumentWriterBase writer)
        {
            if (canvas.Outline == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            string tagName = (writer.Compliance == OoxmlComplianceCore.IsoStrict) ? "wp:whole" : "wpc:whole";

            builder.StartElement(tagName);
            DmlOutlineWriter.Write("a:ln", canvas.Outline, writer);
            builder.EndElement(tagName);
        }

        internal static void WriteEnd(DocxDocumentWriterBase writer)
        {
            writer.Builder.EndElement(); // End either  "lc:lockedCanvas" or "wpc:wpc".
        }
    }
}
