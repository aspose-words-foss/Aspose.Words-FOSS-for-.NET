// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2016 by Alexander Zhiltsov

using Aspose.Common;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// This class allows writing a style part of a chart.
    /// </summary>
    internal static class DmlChartStyleWriter
    {
        /// <summary>
        /// Writes a <see cref="DmlChartStyle"/> object into the element 2.8.1.1 chartStyle [MS-ODRAWXML] that is
        /// a root element of the word/charts/styleX.xml part.
        /// </summary>
        internal static void Write(DmlChartStyle style, DocxDocumentWriterBase writer)
        {
            if (style == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cs:chartStyle");
            builder.WriteAttribute("xmlns:cs", DmlExtensionsNamespace.ChartStyle);
            builder.WriteAttribute("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain,
                writer.Compliance == OoxmlComplianceCore.IsoStrict));

            builder.WriteAttribute("id", style.Id);

            WriteStyleEntry(style[DmlChartStyleItem.AxisTitle], "axisTitle", writer);
            WriteStyleEntry(style[DmlChartStyleItem.CategoryAxis], "categoryAxis", writer);
            WriteStyleEntry(style[DmlChartStyleItem.ChartArea], "chartArea", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataLabel], "dataLabel", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataLabelCallout], "dataLabelCallout", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataPoint], "dataPoint", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataPoint3D], "dataPoint3D", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataPointLine], "dataPointLine", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataPointMarker], "dataPointMarker", writer);
            WriteMarkerLayout(style.DataPointMarkerLayout, "dataPointMarkerLayout", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataPointWireframe], "dataPointWireframe", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DataTable], "dataTable", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DownBar], "downBar", writer);
            WriteStyleEntry(style[DmlChartStyleItem.DropLine], "dropLine", writer);
            WriteStyleEntry(style[DmlChartStyleItem.ErrorBar], "errorBar", writer);
            WriteStyleEntry(style[DmlChartStyleItem.Floor], "floor", writer);
            WriteStyleEntry(style[DmlChartStyleItem.GridlineMajor], "gridlineMajor", writer);
            WriteStyleEntry(style[DmlChartStyleItem.GridlineMinor], "gridlineMinor", writer);
            WriteStyleEntry(style[DmlChartStyleItem.HighLowLine], "hiLoLine", writer);
            WriteStyleEntry(style[DmlChartStyleItem.LeaderLine], "leaderLine", writer);
            WriteStyleEntry(style[DmlChartStyleItem.Legend], "legend", writer);
            WriteStyleEntry(style[DmlChartStyleItem.PlotArea], "plotArea", writer);
            WriteStyleEntry(style[DmlChartStyleItem.PlotArea3D], "plotArea3D", writer);
            WriteStyleEntry(style[DmlChartStyleItem.SeriesAxis], "seriesAxis", writer);
            WriteStyleEntry(style[DmlChartStyleItem.SeriesLine], "seriesLine", writer);
            WriteStyleEntry(style[DmlChartStyleItem.Title], "title", writer);
            WriteStyleEntry(style[DmlChartStyleItem.TrendLine], "trendline", writer);
            WriteStyleEntry(style[DmlChartStyleItem.TrendlineLabel], "trendlineLabel", writer);
            WriteStyleEntry(style[DmlChartStyleItem.UpBar], "upBar", writer);
            WriteStyleEntry(style[DmlChartStyleItem.ValueAxis], "valueAxis", writer);
            WriteStyleEntry(style[DmlChartStyleItem.Wall], "wall", writer);

            DmlExtensionListWriter.Write("cs", style.Extensions, writer);

            builder.EndElement("cs:chartStyle");
            builder.Flush();
        }

        /// <summary>
        /// Writes an element of the 2.8.3.7 CT_StyleEntry complex type.
        /// </summary>
        private static void WriteStyleEntry(DmlChartStyleEntry styleEntry, string tagName, 
            DocxDocumentWriterBase writer)
        {
            if (styleEntry == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetElementFullName(tagName));

            if ((styleEntry.Modifiers != null) && (styleEntry.Modifiers.Length > 0))
                builder.WriteAttribute("mods", string.Join(" ", styleEntry.Modifiers));

            WriteStyleReference(styleEntry.LineReference, "lnRef", writer);
            if (!MathUtil.AreEqual(styleEntry.LineWidthScale, DmlChartStyleEntry.LineWidthScaleDefault))
                builder.WriteElement("cs:lineWidthScale", 
                    FormatterPal.DoubleToStr9Decimals(styleEntry.LineWidthScale));
            WriteStyleReference(styleEntry.FillReference, "fillRef", writer);
            WriteStyleReference(styleEntry.EffectReference, "effectRef", writer);
            WriteFontReference(styleEntry.FontReference, "fontRef", writer);

            if (styleEntry.ShapePr != null)
                DmlShapePropertiesWriter.Write("cs", styleEntry.ShapePr, writer, true);
            if (styleEntry.DefaultRunPr != null)
                DmlTextShapeWriter.WriteDmlRunProperties("cs:defRPr", styleEntry.DefaultRunPr, writer);
            if (styleEntry.TextBodyPr != null)
                DmlTextShapeWriter.WriteDmlTextShapeBodyPr("cs:bodyPr", styleEntry.TextBodyPr, writer);
            if (styleEntry.HasExtensions)
                DmlExtensionListWriter.Write("cs", styleEntry.Extensions, writer);

            builder.EndElement(GetElementFullName(tagName));
        }

        /// <summary>
        /// Gets chart style element name with default prefix.
        /// </summary>
        private static string GetElementFullName(string nameWithoutPrefix)
        {
            return string.Format("cs:{0}", nameWithoutPrefix);
        }

        /// <summary>
        /// Writes an element of the 2.8.3.8 CT_StyleReference complex type.
        /// </summary>
        private static void WriteStyleReference(DmlChartStyleReference styleReference, string tagName, 
            DocxDocumentWriterBase writer)
        {
            WriteReference(FormatterPal.IntToXml(styleReference.StyleMatrixIndex), styleReference.Color,
                styleReference.Modifiers, tagName, writer);
        }

        /// <summary>
        /// Writes an element of reference type (CT_StyleReference or CT_FontReference [MS-ODRAWXML]).
        /// </summary>
        private static void WriteReference(string collectionIndex, DmlColor color, string[] modifiers, string tagName,
            DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetElementFullName(tagName));

            builder.WriteAttribute("idx", collectionIndex);
            if ((modifiers != null) && (modifiers.Length > 0))
                builder.WriteAttribute("mods", string.Join(" ", modifiers));

            if (color != null)
            {
                if (color.ColorType == DmlColorType.ChartStyleColor)
                    WriteStyleColor((DmlChartStyleColor)color, writer);
                else
                    DmlColorWriter.Write(color, writer);
            }

            builder.EndElement(GetElementFullName(tagName));
        }

        /// <summary>
        /// Writes an element of the 2.8.3.6 CT_StyleColor complex type.
        /// </summary>
        private static void WriteStyleColor(DmlChartStyleColor styleColor, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cs:styleClr");

            builder.WriteAttribute("val", styleColor.Value);
            DmlColorModifiersWriter.Write("a", styleColor.ColorModifiers, writer);

            builder.EndElement("cs:styleClr");
        }

        /// <summary>
        /// Writes an element of the 2.8.3.4 CT_FontReference complex type.
        /// </summary>
        private static void WriteFontReference(DmlChartFontReference fontReference, string tagName, 
            DocxDocumentWriterBase writer)
        {
            WriteReference(DmlEnum.FontCollectionIndexToDml(fontReference.FontCollectionIndex),
                fontReference.Color, fontReference.Modifiers, tagName, writer);
        }

        /// <summary>
        /// Writes an element of the 2.8.3.5 CT_MarkerLayout complex type.
        /// </summary>
        private static void WriteMarkerLayout(DmlChartMarkerLayout markerLayout, string tagName, 
            DocxDocumentWriterBase writer)
        {
            if (markerLayout == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetElementFullName(tagName));

            if (StringUtil.HasChars(markerLayout.Symbol))
                builder.WriteAttribute("symbol", markerLayout.Symbol);
            if (markerLayout.Size > 0)
                builder.WriteAttribute("size", markerLayout.Size);

            builder.EndElement(GetElementFullName(tagName));
        }

    }
}
