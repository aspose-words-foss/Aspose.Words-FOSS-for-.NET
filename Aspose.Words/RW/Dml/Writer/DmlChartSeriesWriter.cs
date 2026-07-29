// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/22/2014 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Instance class for writing DrawingML chart series.
    /// </summary>
    internal static class DmlChartSeriesWriter
    {
        internal static void Write(DmlChart chart, DocxDocumentWriterBase writer)
        {
            IList<ChartSeries> series = (IList<ChartSeries> )chart.ChartPr.GetDirectProperty(DmlChartAttrs.Series);
            if (series == null || series.Count == 0)
                return;

            switch (chart.ChartType)
            {
                case DmlChartType.AreaChart:
                case DmlChartType.Area3DChart:
                    WriteAreaChartSeries(series, writer);
                    break;
                case DmlChartType.LineChart:
                case DmlChartType.Line3DChart:
                case DmlChartType.StockChart:
                    WriteLineChartSeries(series, writer);
                    break;
                case DmlChartType.RadarChart:
                    WriteRadarChartSeries(series, writer);
                    break;
                case DmlChartType.ScatterChart:
                    WriteScatterChartSeries(series, writer);
                    break;
                case DmlChartType.PieChart:
                case DmlChartType.Pie3DChart:
                case DmlChartType.DoughnutChart:
                case DmlChartType.OfPieChart:
                    WritePieChartSeries(series, writer);
                    break;
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                    WriteBarChartSeries(series, writer);
                    break;
                case DmlChartType.SurfaceChart:
                case DmlChartType.Surface3DChart:
                    WriteSurfaceChartSeries(series, writer);
                    break;
                case DmlChartType.BubbleChart:
                    WriteBubbleChartSeries(series, writer);
                    break;
                case DmlChartType.ChartExChart:
                    WriteChartExSeries(series, writer);
                    break;
                default:
                    throw new ArgumentException("Unexpected Dml chart type");
            }
        }

        private static void WriteBubbleChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                Debug.Assert(ChartDataPointCollection.SupportsInvertIfNegative(ser.Chart.ChartType));
                builder.WriteElementWithAttributes(GetTagName("invertIfNegative", false), "val", ser.DefaultDataPoint.InvertIfNegative);
                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteTrendLines(ser.Trendlines, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.XErrorBars, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.YErrorBars, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("xVal", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("yVal", false), ser.Y.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("bubbleSize", false), ser.Size.DataSource,
                    writer, false);
                builder.WriteElementWithAttributes(GetTagName("bubble3D", false), "val", ser.DefaultDataPoint.Bubble3D);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WriteLineChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);

                if (ser.DefaultDataPoint.HasMarker)
                    DmlChartCommonWriter.WriteMarker(ser.DefaultDataPoint.Marker, writer, false);

                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteTrendLines(ser.Trendlines, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.XErrorBars, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.YErrorBars, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("cat", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("val", false), ser.Y.DataSource, writer, false);
                builder.WriteElementWithAttributes(GetTagName("smooth", false), "val", ser.Smooth);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WritePieChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                if (ser.DefaultDataPoint.Explosion != -1)
                    builder.WriteElementWithAttributes(GetTagName("explosion", false), "val",
                        ser.DefaultDataPoint.Explosion);
                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("cat", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("val", false), ser.Y.DataSource, writer, false);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WriteSurfaceChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("cat", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("val", false), ser.Y.DataSource, writer, false);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WriteScatterChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                DmlChartCommonWriter.WriteMarker(ser.DefaultDataPoint.Marker, writer, false);
                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteTrendLines(ser.Trendlines, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.XErrorBars, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.YErrorBars, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("xVal", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("yVal", false), ser.Y.DataSource, writer, false);
                builder.WriteElementWithAttributes(GetTagName("smooth", false), "val", ser.Smooth);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WriteRadarChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                DmlChartCommonWriter.WriteMarker(ser.DefaultDataPoint.Marker, writer, false);
                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("cat", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("val", false), ser.Y.DataSource, writer, false);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WriteAreaChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                DmlChartCommonWriter.WritePictureOptions(ser.DefaultDataPoint.PictureOptions, builder, false);
                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteTrendLines(ser.Trendlines, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.XErrorBars, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.YErrorBars, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("cat", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("val", false), ser.Y.DataSource, writer, false);
                WriteSeriesEnd(ser, writer, false);
            }
        }

        private static void WriteBarChartSeries(IList<ChartSeries> series, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries ser in series)
            {
                builder.StartElement(GetTagName("ser", false));
                WriteSeriesShared(ser, writer, false);
                Debug.Assert(ChartDataPointCollection.SupportsInvertIfNegative(ser.Chart.ChartType));
                builder.WriteElementWithAttributes(GetTagName("invertIfNegative", false), "val", ser.DefaultDataPoint.InvertIfNegative);
                DmlChartCommonWriter.WritePictureOptions(ser.DefaultDataPoint.PictureOptions, builder, false);
                DmlChartCommonWriter.WriteDataPoints(ser, writer, false);
                if (ser.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(ser.DataLabels, writer, false);
                DmlChartCommonWriter.WriteTrendLines(ser.Trendlines, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.XErrorBars, writer, false);
                DmlChartCommonWriter.WriteErrorBars(ser.YErrorBars, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("cat", false), ser.X.DataSource, writer, false);
                DmlChartCommonWriter.WriteDataSource(GetTagName("val", false), ser.Y.DataSource, writer, false);
                // Write Shape of series only if it differs from the parent chart shape.
                if ((ser.Shape != BarShape.None) &&
                    (ser.Shape != (BarShape)ser.Chart.ChartPr.GetProperty(DmlChartAttrs.Shape)))
                    builder.WriteElementWithAttributes(GetTagName("shape", false), "val",
                        DmlChartsEnum.ShapeToDml(ser.Shape));
                WriteSeriesEnd(ser, writer, false);
            }
        }

        /// <summary>
        /// Writes series of a chart of the http://schemas.microsoft.com/office/drawing/2014/chartex schema
        /// [MS-ODRAWXML].
        /// </summary>
        private static void WriteChartExSeries(IList<ChartSeries> seriesCollection, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            foreach (ChartSeries series in seriesCollection)
            {
                builder.StartElement(GetTagName("series", true));

                builder.WriteAttribute("layoutId", DmlChartsEnum.SeriesLayoutToDml(series.LayoutType));
                if (series.Hidden)
                    builder.WriteAttribute("hidden", series.Hidden);
                if (series.Owner != null)
                {
                    int ownerIndex = seriesCollection.IndexOf(series.Owner);
                    if (ownerIndex >= 0)
                        builder.WriteAttribute("ownerIdx", ownerIndex);
                }
                builder.WriteAttribute("uniqueId", series.UniqueId);
                if (series.FormatIndex >= 0)
                    builder.WriteAttribute("formatIdx", series.FormatIndex);

                WriteSeriesShared(series, writer, true);
                DmlChartCommonWriter.WriteValueColors(series.ValueColors, writer);
                DmlChartCommonWriter.WriteDataPoints(series, writer, true);
                if (series.HasDataLabels)
                    DmlChartCommonWriter.WriteDataLabels(series.DataLabels, writer, true);

                if (series.DataId >= 0)
                    builder.WriteElementWithAttributes(GetTagName("dataId", true), "val", series.DataId);
                DmlChartCommonWriter.WriteLayoutProperties(series.LayoutPr, writer);
                if (series.AxisId >= 0)
                    builder.WriteElementWithAttributes(GetTagName("axisId", true), "val", series.AxisId);

                WriteSeriesEnd(series, writer, true);
            }
        }

        /// <summary>
        /// Writes common properties of all series.
        /// </summary>
        private static void WriteSeriesShared(ChartSeries series, DocxDocumentWriterBase writer, bool isChartEx)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            if (!isChartEx)
            {
                builder.WriteElementWithAttributes(GetTagName("idx", isChartEx), "val", series.Index);
                builder.WriteElementWithAttributes(GetTagName("order", isChartEx), "val", series.Order);
            }
            DmlChartCommonWriter.WriteTx(series.Tx, writer, isChartEx);
            DmlChartCommonWriter.WriteShapeProperties(series.DefaultDataPoint.SpPr, writer, isChartEx);
        }

        /// <summary>
        /// Closes series tag and writes extensions if present.
        /// </summary>
        private static void WriteSeriesEnd(ChartSeries series, DocxDocumentWriterBase writer, bool isChartEx)
        {
            StringToObjDictionary<DmlExtension> extensions = ((IDmlExtensionListSource)series).Extensions;

            if (extensions != null)
                DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), extensions, writer);

            writer.CurrentBuilder.EndElement(GetTagName(isChartEx ? "series" : "ser", isChartEx));
        }

        /// <summary>
        /// Generates a tag name with a prefix depending on chart type.
        /// </summary>
        /// <param name="localName">Name without prefix.</param>
        /// <param name="isChartEx">A flag indicating that the chart is of the chartEx schema.</param>
        /// <returns>Tag name with prefix.</returns>
        private static string GetTagName(string localName, bool isChartEx)
        {
            return DmlChartCommonWriter.GetTagName(localName, isChartEx);
        }

        /// <summary>
        /// Gets a tag prefix depending on chart type.
        /// </summary>
        private static string GetTagPrefix(bool isChartEx)
        {
            return DmlChartCommonWriter.GetTagPrefix(isChartEx);
        }
    }
}
