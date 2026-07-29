// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/07/2014 by Andrey Noskov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Represents class writing DrawingML shape style.
    /// </summary>
    internal static class DmlShapeStyleWriter
    {
        internal static void Write(DmlShapeBase dmlShapeBase, DocxDocumentWriterBase writer)
        {
            string prefix;
            switch (dmlShapeBase.DmlNodeType)
            {
                case DmlNodeType.Picture:
                    prefix = "pic";
                    break;
                case DmlNodeType.Shape:
                case DmlNodeType.ConnectorShape:
                    prefix = "a";
                    break;
                case DmlNodeType.WordprocessingShape:
                    prefix = "wps";
                    break;
                default:
                    throw new ArgumentException("Unexpected Dml node type.");
            }

            Write(prefix, dmlShapeBase.Style, writer);
        }

        internal static void Write(string prefix, DmlShapeStyle shapeStyle, DocxDocumentWriterBase writer)
        {
            if (shapeStyle == null || shapeStyle.IsEmpty)
                return;

            NrxXmlBuilder builder = writer.CurrentBuilder;

            string rootTagName = string.Format("{0}:style", prefix);

            builder.StartElement(rootTagName);

            WriteDmlLineReference(shapeStyle.LineReference, writer);
            WriteDmlFillReference(shapeStyle.FillReference, writer);
            WriteDmlEffectRef(shapeStyle.EffectReference, writer);
            WriteDmlFontReference(shapeStyle.FontReference, writer);

            builder.EndElement(rootTagName);
        }

        private static void WriteDmlFontReference(DmlFontReference fontRef, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:fontRef");
            builder.WriteAttribute("idx", DmlEnum.FontCollectionIndexToDml(fontRef.FontCollectionIndex), 0);
            DmlColorWriter.Write(fontRef.Color, writer);
            builder.EndElement("a:fontRef");
        }

        private static void WriteDmlFillReference(DmlFillReference fillRef, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:fillRef");
            builder.WriteAttribute("idx", fillRef.StyleMatrixIndex, 0);
            DmlColorWriter.Write(fillRef.Color, writer);
            builder.EndElement("a:fillRef");
        }

        private static void WriteDmlLineReference(DmlLineReference lineRef, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:lnRef");
            builder.WriteAttribute("idx", lineRef.StyleMatrixIndex, 0);
            DmlColorWriter.Write(lineRef.Color, writer);
            builder.EndElement("a:lnRef");
        }

        private static void WriteDmlEffectRef(DmlEffectReference effectRef, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;

            builder.StartElement("a:effectRef");
            builder.WriteAttribute("idx", effectRef.StyleMatrixIndex, 0);
            DmlColorWriter.Write(effectRef.Color, writer);
            builder.EndElement("a:effectRef");
        }
    }
}
