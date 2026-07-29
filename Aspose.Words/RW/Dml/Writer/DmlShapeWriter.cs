// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Represents class writing shape "a:sp" or WordProccessingShape "wps:wsp"
    /// </summary>
    internal static class DmlShapeWriter
    {
        internal static void WriteStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlNode dmlNode = dml.DmlNode;

            if (dmlNode.DmlNodeType != DmlNodeType.Shape &&
                dmlNode.DmlNodeType != DmlNodeType.WordprocessingShape &&
                dmlNode.DmlNodeType != DmlNodeType.ConnectorShape)
                throw new ArgumentException("Unexpected Dml node type.");

            DmlShape dmlShape = (DmlShape)dmlNode;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            string prefix;
            string rootTagName;

            switch (dmlShape.DmlNodeType)
            {
                case DmlNodeType.Shape:
                    {
                        prefix = "a";
                        rootTagName = "a:sp";
                        break;
                    }
                case DmlNodeType.WordprocessingShape:
                    {
                        prefix = isIsoStrict ? "wp" : "wps";
                        rootTagName = string.Format("{0}:wsp", prefix);
                        break;
                    }
                case DmlNodeType.ConnectorShape:
                    {
                        prefix = "a";
                        rootTagName = "a:cxnSp";
                        break;
                    }
                default:
                    throw new ArgumentException("Unexpected Dml node type.");
            }

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(rootTagName);

            if ((dmlShape.DmlNodeType == DmlNodeType.WordprocessingShape) && !isIsoStrict)
                builder.WriteAttribute("xmlns:wps", DmlNamespaceUtil.GetNamespace(DmlNodeType.WordprocessingShape, false));

            DmlNonVisualPropertiesWriter.WriteNvPr(prefix, dmlShape, writer);
            DmlShapePropertiesWriter.Write(dmlShape, writer);
            if (dml.DmlNode.DmlNodeType == DmlNodeType.Shape)
                DmlTextShapeWriter.Write((DmlShape)dml.DmlNode, writer);

            DmlShapeStyleWriter.Write(dmlShape, writer);
            string tagName;

            if (dml.HasChildNodes)
            {
                tagName = isIsoStrict ? "wp:txbx" : "wps:txbx";
                builder.StartElement(tagName);
                builder.WriteAttributeIfNotZero("id", dml.TextboxId);

                tagName = isIsoStrict ? "wne:txbxContent" : "w:txbxContent";
                builder.StartElement(tagName);
            }
            else if (dml.LinkedTextboxId > 0)
            {
                tagName = isIsoStrict ? "wp:linkedTxbx" : "wps:linkedTxbx";
                builder.StartElement(tagName);
                builder.WriteAttributeIfNotZero("id", dml.LinkedTextboxId);
                builder.WriteAttributeIfNotZero("seq", dml.LinkedTextboxSeq);
                builder.EndElement(tagName);
            }
        }

        internal static void WriteEnd(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DocxBuilder currentBuilder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            if (dml.HasChildNodes)
            {
                currentBuilder.EndElement(isIsoStrict ? "wne:txbxContent" : "w:txbxContent");
                currentBuilder.EndElement(isIsoStrict ? "wp:txbx" : "wps:txbx");
            }

            if (dml.DmlNode.DmlNodeType == DmlNodeType.WordprocessingShape)
                DmlTextShapeWriter.Write((DmlShape)dml.DmlNode, writer);

            currentBuilder.EndElement(); // Closes either "a:sp" or "wps:wsp".
        }
    }
}
