// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/22/2014 by Alexey Noskov

using System;
using Aspose.Collections;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Instance class for writing DrawingML chart objects.
    /// </summary>
    internal static class DmlChartObjectWriter
    {
        internal static void Write(DmlChart chart, DocxDocumentWriterBase writer, bool isChartEx)
        {
            string tagName = GetChartTagName(chart.ChartType, isChartEx);

            writer.CurrentBuilder.StartElement(tagName);
            WriteChartProperties(chart, writer, isChartEx);
            writer.CurrentBuilder.EndElement(tagName);
        }

        private static string GetChartTagName(DmlChartType type, bool isChartEx)
        {
            switch (type)
            {
                case DmlChartType.AreaChart: return GetTagName("areaChart", isChartEx);
                case DmlChartType.Area3DChart: return GetTagName("area3DChart", isChartEx);
                case DmlChartType.LineChart: return GetTagName("lineChart", isChartEx);
                case DmlChartType.Line3DChart: return GetTagName("line3DChart", isChartEx);
                case DmlChartType.StockChart: return GetTagName("stockChart", isChartEx);
                case DmlChartType.RadarChart: return GetTagName("radarChart", isChartEx);
                case DmlChartType.ScatterChart: return GetTagName("scatterChart", isChartEx);
                case DmlChartType.PieChart: return GetTagName("pieChart", isChartEx);
                case DmlChartType.Pie3DChart: return GetTagName("pie3DChart", isChartEx);
                case DmlChartType.DoughnutChart: return GetTagName("doughnutChart", isChartEx);
                case DmlChartType.BarChart: return GetTagName("barChart", isChartEx);
                case DmlChartType.Bar3DChart: return GetTagName("bar3DChart", isChartEx);
                case DmlChartType.OfPieChart: return GetTagName("ofPieChart", isChartEx);
                case DmlChartType.SurfaceChart: return GetTagName("surfaceChart", isChartEx);
                case DmlChartType.Surface3DChart: return GetTagName("surface3DChart", isChartEx);
                case DmlChartType.BubbleChart: return GetTagName("bubbleChart", isChartEx);
                default:
                    throw new ArgumentException("Unexpected Dml chart type");
            }
        }

        private static void WriteChartProperties(DmlChart chart, DocxDocumentWriterBase writer, bool isChartEx)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            DmlChartPr pr = chart.ChartPr;

            // Specifies whether the series form a bar (horizontal) chart or a column (vertical) chart.
            // <see cref="BarDirection"/>.
            object barDir = pr.GetDirectProperty(DmlChartAttrs.BarDir);
            if (barDir != null)
                builder.WriteElementWithAttributes(GetTagName("barDir", isChartEx), "val",
                    DmlChartsEnum.BarDirectionToDml((BarDirection)barDir));

            // Specifies whether this chart is pie of pie or bar of pie.
            // <see cref="OfPieType"/>.
            object ofPieType = pr.GetDirectProperty(DmlChartAttrs.OfPieType);
            if (ofPieType != null)
                builder.WriteElementWithAttributes(GetTagName("ofPieType", isChartEx), "val",
                    DmlChartsEnum.OfPieTypeToDml((OfPieType)ofPieType));

            // Specifies what type of radar chart shall be drawn.
            // <see cref="RadarStyle"/>.
            object radarStyle = pr.GetDirectProperty(DmlChartAttrs.RadarStyle);
            if (radarStyle != null)
                builder.WriteElementWithAttributes(GetTagName("radarStyle", isChartEx), "val",
                    DmlChartsEnum.RadarStyleToDml((RadarStyle)radarStyle));

            // Specifies the type of lines for the scatter chart.
            // <see cref="ScatterStyle"/>.
            object scatterStyle = pr.GetDirectProperty(DmlChartAttrs.ScatterStyle);
            if (scatterStyle != null)
                builder.WriteElementWithAttributes(GetTagName("scatterStyle", isChartEx), "val",
                    DmlChartsEnum.ScatterStyleToDml((ScatterStyle)scatterStyle));

            // Specifies the surface chart is drawn as a wireframe.
            // Boolean.
            object wireframe = pr.GetDirectProperty(DmlChartAttrs.Wireframe);
            if (wireframe != null)
                builder.WriteElementWithAttributes(GetTagName("wireframe", isChartEx), "val", wireframe);

            // Specifies the type of grouping for a column, line, bar, or area chart.
            // <see cref="Grouping"/>.
            object grouping = pr.GetDirectProperty(DmlChartAttrs.Grouping);
            if (grouping != null)
                builder.WriteElementWithAttributes(GetTagName("grouping", isChartEx), "val",
                    DmlChartsEnum.GroupingToDml((Grouping)grouping));

            // Specifies that each data marker in the series shall have a different color.
            // Boolean.
            object varyColors = pr.GetDirectProperty(DmlChartAttrs.VaryColors);
            if (varyColors != null)
                builder.WriteElementWithAttributes(GetTagName("varyColors", isChartEx), "val", varyColors);

            // Specifies list of series of the chart.
            // IList{T} with items of <see cref="ChartSeries"/> type.
            object series = pr.GetDirectProperty(DmlChartAttrs.Series);
            if (series != null)
                DmlChartSeriesWriter.Write(chart, writer);

            // Specifies the settings for the data labels for the entire chart.
            // <see cref="ChartDataLabelCollection"/>.
            object dLbls = pr.GetDirectProperty(DmlChartAttrs.DLbls);
            if (dLbls != null)
                DmlChartCommonWriter.WriteDataLabels((ChartDataLabelCollection)dLbls, writer, isChartEx);

            // Specifies drop lines formatting.
            // <see cref="DmlChartSpPr"/>.
            object dropLines = pr.GetDirectProperty(DmlChartAttrs.DropLines);
            bool isDropLinesVisible = (bool)pr.GetProperty(DmlChartAttrs.IsDropLinesVisible);
            if (dropLines != null)
                DmlChartCommonWriter.WriteWrappedSpPr(GetTagName("dropLines", isChartEx), (DmlChartSpPr)dropLines,
                    writer, isDropLinesVisible, isChartEx);

            // Specifies the high-low lines formatting for the series.
            // <see cref="DmlChartSpPr"/>.
            object hiLowLines = pr.GetDirectProperty(DmlChartAttrs.HiLowLines);
            bool isHiLowLinesVisible = (bool)pr.GetProperty(DmlChartAttrs.IsHiLowLinesVisible);
            if (hiLowLines != null)
                DmlChartCommonWriter.WriteWrappedSpPr(GetTagName("hiLowLines", isChartEx), (DmlChartSpPr)hiLowLines,
                    writer, isHiLowLinesVisible, isChartEx);

            // Specifies the space between bar or column clusters, as a percentage of the bar or column width.
            // Integer [0;500].
            object gapWidth = pr.GetDirectProperty(DmlChartAttrs.GapWidth);
            if (gapWidth != null)
                builder.WriteElementWithAttributes(GetTagName("gapWidth", isChartEx), "val", gapWidth);

            // Specifies the space between bar or column clusters, as a percentage of the bar or column width.
            // Integer [0;500].
            object gapDepth = pr.GetDirectProperty(DmlChartAttrs.GapDepth);
            if (gapDepth != null)
                builder.WriteElementWithAttributes(GetTagName("gapDepth", isChartEx), "val", gapDepth);

            // Specifies the shape of a 3-D bar chart.
            object shape = pr.GetDirectProperty(DmlChartAttrs.Shape);
            if (shape != null)
                builder.WriteElementWithAttributes(GetTagName("shape", isChartEx), "val",
                    DmlChartsEnum.ShapeToDml((BarShape)shape));

            // Specifies how much bars and columns shall overlap on 2-D charts.
            // Integer [-100;100].
            object overlap = pr.GetDirectProperty(DmlChartAttrs.Overlap);
            if (overlap != null)
                builder.WriteElementWithAttributes(GetTagName("overlap", isChartEx), "val", overlap);

            // Specifies how to determine which data points are in the second pie or bar on a pie of pie or bar of pie chart.
            // <see cref="SplitType"/>.
            object splitType = pr.GetDirectProperty(DmlChartAttrs.SplitType);
            if (splitType != null)
                builder.WriteElementWithAttributes(GetTagName("splitType", isChartEx), "val",
                    DmlChartsEnum.SplitTypeToDml((SplitType)splitType));

            // Specifies a value that shall be used to determine which data points
            // are in the second pie or bar on a pie of pie or bar of pie chart.
            // Double.
            object splitPos = pr.GetDirectProperty(DmlChartAttrs.SplitPos);
            if (splitPos != null)
                builder.WriteElementWithAttributes(GetTagName("splitPos", isChartEx), "val", splitPos);

            // Contains the custom split information for a pie-of-pie or bar-of-pie chart with a custom split type.
            // Array of Integers.
            object custSplit = pr.GetDirectProperty(DmlChartAttrs.CustSplit);
            if (custSplit != null)
                DmlChartCommonWriter.WriteCustomSplit((int[])custSplit, builder, isChartEx);

            // specifies the size of the second pie or bar of a pie of pie chart or a bar of pie chart,
            // as a percentage of the size of the first pie.
            // Integer [5;200].
            object secondPieSize = pr.GetDirectProperty(DmlChartAttrs.SecondPieSize);
            if (secondPieSize != null)
                builder.WriteElementWithAttributes(GetTagName("secondPieSize", isChartEx), "val", secondPieSize);

            // Specifies series lines formatting for the chart.
            // <see cref="DmlChartSpPr"/>.
            object serLines = pr.GetDirectProperty(DmlChartAttrs.SerLines);
            if (serLines != null)
                DmlChartCommonWriter.WriteWrappedSpPr(GetTagName("serLines", isChartEx), (DmlChartSpPr)serLines, writer,
                    (bool)pr.GetProperty(DmlChartAttrs.ShowSerLine), isChartEx);

            // Specifies the scale factor for the bubble chart.
            // Integer [0; 300].
            object bubbleScale = pr.GetDirectProperty(DmlChartAttrs.BubbleScale);
            if (bubbleScale != null)
                builder.WriteElementWithAttributes(GetTagName("bubbleScale", isChartEx), "val", bubbleScale);

            // Specifies negative sized bubbles shall be shown on a bubble chart.
            // Boolean.
            object showNegBubbles = pr.GetDirectProperty(DmlChartAttrs.ShowNegBubbles);
            if (showNegBubbles != null)
                builder.WriteElementWithAttributes(GetTagName("showNegBubbles", isChartEx), "val", showNegBubbles);

            // Specifies how the bubble size values are represented on the chart.
            // <see cref="SizeRepresents"/>.
            object sizeRepresents = pr.GetDirectProperty(DmlChartAttrs.SizeRepresents);
            if (sizeRepresents != null)
                builder.WriteElementWithAttributes(GetTagName("sizeRepresents", isChartEx), "val",
                    DmlChartsEnum.SizeRepresentsToDml((SizeRepresents)sizeRepresents));

            // Specifies the angle of the first pie or doughnut chart slice, in degrees (clockwise from up).
            // Integer [0;300].
            object firstSliceAng = pr.GetDirectProperty(DmlChartAttrs.FirstSliceAng);
            if (firstSliceAng != null)
                builder.WriteElementWithAttributes(GetTagName("firstSliceAng", isChartEx), "val", firstSliceAng);

            // Specifies the size of the hole in a doughnut chart.
            // Integer [10;90].
            object holeSize = pr.GetDirectProperty(DmlChartAttrs.HoleSize);
            if (holeSize != null)
                builder.WriteElementWithAttributes(GetTagName("holeSize", isChartEx), "val", holeSize);

            // Specifies the up and down bars.
            // <see cref="DmlChartUpDownBars"/>.
            object upDownBars = pr.GetDirectProperty(DmlChartAttrs.UpDownBars);
            if (upDownBars != null)
                DmlChartCommonWriter.WriteUpDownBars((DmlChartUpDownBars)upDownBars, writer, isChartEx);

            // Specifies that the marker shall be shown.
            // Boolean.
            object showMarker = pr.GetDirectProperty(DmlChartAttrs.ShowMarker);
            if (showMarker != null)
                builder.WriteElementWithAttributes(GetTagName("marker", isChartEx), "val", showMarker);

            // Specifies the line connecting the points on the chart shall be smoothed using Catmull-Rom splines.
            // Boolean.
            object smooth = pr.GetDirectProperty(DmlChartAttrs.Smooth);
            if (smooth != null)
                builder.WriteElementWithAttributes(GetTagName("smooth", isChartEx), "val", smooth);

            // Contains a collection of formatting bands for a surface chart indexed from low to high.
            // <see cref="DmlChartBandFormats"/>.
            object bandFmts = pr.GetDirectProperty(DmlChartAttrs.BandFmts);
            if (bandFmts != null)
                DmlChartCommonWriter.WriteBandFormats((DmlChartBandFormats)bandFmts, writer, isChartEx);

            // Id of X axis.
            // Integer.
            object axIdX = pr.GetDirectProperty(DmlChartAttrs.AxIdX);
            if (axIdX != null)
                builder.WriteElementWithAttributes(GetTagName("axId", isChartEx), "val", axIdX);

            // Id of Y axis.
            // Integer.
            object axIdY = pr.GetDirectProperty(DmlChartAttrs.AxIdY);
            if (axIdY != null)
                builder.WriteElementWithAttributes(GetTagName("axId", isChartEx), "val", axIdY);

            // Id of Z axis (for 3D charts).
            // Integer.
            object axIdZ = pr.GetDirectProperty(DmlChartAttrs.AxIdZ);
            if (axIdZ != null)
                builder.WriteElementWithAttributes(GetTagName("axId", isChartEx), "val", axIdZ);

            object dmlExtensions = pr.GetDirectProperty(DmlChartAttrs.Extensions);
            if (dmlExtensions != null)
                DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), (StringToObjDictionary<DmlExtension>)dmlExtensions, writer);
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
