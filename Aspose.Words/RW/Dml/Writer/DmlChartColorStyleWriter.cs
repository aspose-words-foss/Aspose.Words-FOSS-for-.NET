// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/05/2016 by Alexander Zhiltsov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// This class allows writing a color style part of a chart.
    /// </summary>
    internal static class DmlChartColorStyleWriter
    {
        /// <summary>
        /// Writes a <see cref="DmlChartColorStyle"/> object into the element 2.8.1.2 colorStyle [MS-ODRAWXML] that is
        /// a root element of the word/charts/colorsX.xml part.
        /// </summary>
        internal static void Write(DmlChartColorStyle colorStyle, DocxDocumentWriterBase writer)
        {
            if (colorStyle == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cs:colorStyle");
            builder.WriteAttribute("xmlns:cs", DmlExtensionsNamespace.ChartStyle);
            builder.WriteAttribute("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain,
                writer.Compliance == OoxmlComplianceCore.IsoStrict));

            builder.WriteAttribute("meth", colorStyle.Method);
            builder.WriteAttribute("id", colorStyle.Id);

            if (colorStyle.Colors != null)
            {
                foreach (DmlColor color in colorStyle.Colors) 
                    DmlColorWriter.Write(color, writer);
            }

            if (colorStyle.Variations != null)
            {
                foreach (DmlChartColorStyleVariation variation in colorStyle.Variations)
                    WriteVariation(variation, writer);
            }

            DmlExtensionListWriter.Write("cs", colorStyle.Extensions, writer);

            builder.EndElement("cs:colorStyle");
            builder.Flush();
        }

        /// <summary>
        /// Writes the variation element of the 2.8.3.3 CT_ColorStyleVariation type.
        /// </summary>
        private static void WriteVariation(DmlChartColorStyleVariation variation, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cs:variation");
            DmlColorModifiersWriter.Write("a", variation.ColorModifiers, writer);
            builder.EndElement("cs:variation");
        }
    }
}
