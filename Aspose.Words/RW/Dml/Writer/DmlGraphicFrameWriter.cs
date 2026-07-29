// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/10/2016 by Dmitry Sokolov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Implements writing of the graphic frame element.
    /// </summary>
    internal static class DmlGraphicFrameWriter
    {
        /// <summary>
        /// Starts graphic frame element.
        /// </summary>
        /// <param name="dml">Graphic frame shape.</param>
        /// <param name="writer">Document writer.</param>
        internal static void WriteStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            Debug.Assert((dml != null) && (writer != null));

            DmlNode dmlNode = dml.DmlNode;
            string prefix = GetPrefix(writer, dmlNode);
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(string.Format("{0}:graphicFrame", prefix));

            DmlGroupShape groupShape = (DmlGroupShape)dmlNode;
            DmlNonVisualPropertiesWriter.WriteNvPr(prefix, groupShape, writer);

            if (!groupShape.IsParentLockedCanvas)
                DmlXfrmWriter.Write(string.Format("{0}:xfrm", prefix), groupShape.TransformInternal, writer);
        }

        /// <summary>
        /// Closes graphic frame tag.
        /// </summary>
        /// <param name="writer">Document writer.</param>
        /// <param name="dml">Graphic frame shape.</param>
        internal static void WriteEnd(DocxDocumentWriterBase writer, ShapeBase dml)
        {
            Debug.Assert((writer != null) && (dml != null) && (dml.DmlNode != null));

            DmlNode dmlNode = dml.DmlNode;
            DmlGroupShape groupShape = (DmlGroupShape)dmlNode;
            string prefix = GetPrefix(writer, dmlNode);

            // See related TestJira17672 for details.
            if (groupShape.IsParentLockedCanvas)
                DmlXfrmWriter.Write(string.Format("{0}:xfrm", prefix), groupShape.TransformInternal, writer);

            // Write extensions for graphic frame.
            DmlExtensionListWriter.Write(((DmlGroupShape)dml.DmlNode).Extensions, writer);
            writer.CurrentBuilder.EndElement(); // Closes either "wp:graphicFrame" or "wpg:graphicFrame" or "a:graphicFrame".
        }

        private static string GetPrefix(DocxDocumentWriterBase writer, DmlNode dmlNode)
        {
            Debug.Assert(dmlNode.DmlNodeType == DmlNodeType.GraphicFrame);

            bool isIsoStrict = (writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string prefix = ((DmlGroupShape)dmlNode).IsParentLockedCanvas ? "a" : isIsoStrict ? "wp" : "wpg";
            return prefix;
        }
    }
}
