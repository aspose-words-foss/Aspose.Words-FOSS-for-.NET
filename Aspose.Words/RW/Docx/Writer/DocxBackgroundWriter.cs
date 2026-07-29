// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/06/2016 by Alexander Zhiltsov

using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.RW.Vml;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// This class allows writing the 17.2.1 background (Document Background) element.
    /// </summary>
    internal static class DocxBackgroundWriter
    {
        /// <summary>
        /// Writes the w:background element with using the specified writer.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer)
        {
            Shape shape = writer.SaveInfo.Document.BackgroundShape;
            if (!Shape.IsVisibleAsBackground(shape))
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            builder.StartElement("w:background");

            DrColor color = (DrColor)shape.GetDirectShapeAttrInternal(ShapeAttr.FillColor);
            if (color == null)
                color = shape.FillCore.ColorInternal;
            builder.WriteAttributeString("w:color", DocxBuilder.GetColorString(color));

            if ((shape.FallbackShape != null) && (writer.Compliance == OoxmlComplianceCore.IsoStrict))
                WriteAlternateContent(shape, writer);
            else
                WriteShape(shape, writer);

            builder.EndElement(); //w:background
        }

        /// <summary>
        /// Writes a background shape as an alternate content block with VML shape (v:background) in a choice part
        /// and with DML shape in fallback. In this way a background shape is written in Strict OOXML format.
        /// </summary>
        private static void WriteAlternateContent(Shape shape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("mc:AlternateContent");
            builder.StartElement("mc:Choice");
            builder.WriteAttribute("Requires", DocxDocumentWriterBase.GetChoiceRequires(shape));

            WriteShape(shape, writer);

            builder.EndElement("mc:Choice");
            builder.StartElement("mc:Fallback");

            WriteShape(shape.FallbackShape, writer);

            builder.EndElement("mc:Fallback");
            builder.EndElement("mc:AlternateContent");
        }

        /// <summary>
        /// Writes a background shape.
        /// </summary>
        private static void WriteShape(ShapeBase shape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            if (shape.MarkupLanguage == ShapeMarkupLanguage.Vml)
            {
                VmlShapeWriter.Write(shape, true, builder, writer);
                builder.EndElement(); //v:background
            }
            else
            {
                DmlDrawingWriter.WriteStart(shape, writer);
                DmlDrawingWriter.WriteEnd(shape, writer);
            }
        }
    }
}
